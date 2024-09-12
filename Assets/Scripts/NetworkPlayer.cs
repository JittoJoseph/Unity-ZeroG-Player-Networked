using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class NetworkPlayer : MonoBehaviour
{
    public float thrustForce = 8f;
    public float maxVelocity = 10f;
    public float dragCoefficient = 0.7f; // New: controls how quickly the player slows down
    public float mouseSensitivity = 2f;
    public float upDownRange = 80f;
    public float interactionForce = 2f;

    private CharacterController controller;
    private Camera playerCamera;
    private float verticalRotation = 0f;
    private Vector3 velocity = Vector3.zero; // New: to store the player's velocity

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        if (playerCamera == null)
        {
            Debug.LogError("Player camera not found!");
            return;
        }

        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleRotation();
        HandleMovement();
        ApplyVelocity();
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    void HandleMovement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calculate forward direction based on camera angle
        Vector3 forward = Quaternion.Euler(verticalRotation, transform.eulerAngles.y, 0) * Vector3.forward;

        // Calculate movement vector
        Vector3 movement = (transform.right * moveHorizontal + forward * moveVertical).normalized;

        // Apply thrust to velocity
        velocity += movement * thrustForce * Time.deltaTime;

        // Clamp velocity to max speed
        velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
    }

    void ApplyVelocity()
    {
        // Apply drag
        velocity -= velocity * dragCoefficient * Time.deltaTime;

        // Move the player
        controller.Move(velocity * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody hitRigidbody = hit.collider.attachedRigidbody;

        // Check if we hit a rigidbody and it's not kinematic
        if (hitRigidbody != null && !hitRigidbody.isKinematic)
        {
            Vector3 pushDirection = hit.point - transform.position;
			//pushDirection.y = 0; //used to cancel force on the vertical axis

            // Apply the force
            hitRigidbody.AddForce(pushDirection.normalized * interactionForce, ForceMode.Impulse);
        }
    }
}