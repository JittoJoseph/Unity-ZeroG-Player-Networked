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
        
        if (base.IsOwner)
        {
            Debug.Log($"Player {gameObject.name} is owned by the local client.");
            SetupLocalPlayer();
        }
        else
        {
            Debug.Log($"Player {gameObject.name} is not owned by the local client.");
            DisableComponentsForNonOwner();
        }
    }

    void SetupLocalPlayer()
    {
        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null)
        {
            Debug.LogError($"Player camera not found for {gameObject.name}!");
            return;
        }
        
        playerCamera.gameObject.SetActive(true);
        Debug.Log($"Camera activated for local player {gameObject.name}");

        // Lock and hide the cursor only for the owner
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void DisableComponentsForNonOwner()
    {
        Camera nonOwnerCamera = GetComponentInChildren<Camera>();
        if (nonOwnerCamera != null)
        {
            nonOwnerCamera.gameObject.SetActive(false);
            Debug.Log($"Camera deactivated for non-owner player {gameObject.name}");
        }

        if (controller != null)
        {
            controller.enabled = false;
        }

        this.enabled = false;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
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