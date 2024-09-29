using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

[RequireComponent(typeof(CharacterController))]
public class NetworkPlayer : NetworkBehaviour
{
    public float thrustForce = 8f;
    public float maxVelocity = 10f;
    public float dragCoefficient = 0.7f;
    public float mouseSensitivity = 2f;
    public float upDownRange = 80f;
    public float interactionForce = 2f;

    private CharacterController controller;
    private Camera playerCamera;
    private float verticalRotation = 0f;
    private Vector3 velocity = Vector3.zero;

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        if (!base.IsOwner)
        {
            DisableComponentsForNonOwner();
        }
    }

    void DisableComponentsForNonOwner()
    {
        // Disable the camera for non-owner players
        if (playerCamera != null)
        {
            playerCamera.gameObject.SetActive(false);
        }

        // Disable the CharacterController for non-owner players
        if (controller != null)
        {
            controller.enabled = false;
        }

        // Optionally, you might want to disable this script for non-owner players
        this.enabled = false;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        if (playerCamera == null)
        {
            Debug.LogError("Player camera not found!");
            return;
        }

        // Lock and hide the cursor only for the owner
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!base.IsOwner) return;

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

        Vector3 forward = Quaternion.Euler(verticalRotation, transform.eulerAngles.y, 0) * Vector3.forward;
        Vector3 movement = (transform.right * moveHorizontal + forward * moveVertical).normalized;

        velocity += movement * thrustForce * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
    }

    void ApplyVelocity()
    {
        velocity -= velocity * dragCoefficient * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!base.IsOwner) return;

        Rigidbody hitRigidbody = hit.collider.attachedRigidbody;
        if (hitRigidbody != null && !hitRigidbody.isKinematic)
        {
            Vector3 pushDirection = hit.point - transform.position;
            hitRigidbody.AddForce(pushDirection.normalized * interactionForce, ForceMode.Impulse);
        }
    }
}