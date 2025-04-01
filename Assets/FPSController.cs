using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float crouchSpeed = 2f;
    public float jumpHeight = 2f;
    public float gravity = -20f;

    [Header("Mouse Look Settings")]
    public float mouseSensitivity = 2f;
    public Transform playerCamera;

    [Header("Crouch Settings")]
    public float crouchHeight = 2f;
    public float standHeight = 4f;
    public float crouchTransitionSpeed = 6f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private float verticalRotation = 0f;
    private bool isGrounded;
    private bool isCrouching;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        isCrouching = false;

        //if (groundCheck != null)
       // {
       //     float safeCheckY = -(controller.height / 2f + groundDistance);
       //     groundCheck.localPosition = new Vector3(0f, Mathf.Max(safeCheckY, -2f), 0f);

      //  }
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleCrouch();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void HandleMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        float speed = isCrouching ? crouchSpeed :
                      Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        if (isGrounded && Input.GetButtonDown("Jump") && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove = move * speed + Vector3.up * velocity.y;
        controller.Move(finalMove * Time.deltaTime);
    }

    void HandleCrouch()
{
    if (Input.GetKeyDown(KeyCode.LeftControl))
    {
        isCrouching = !isCrouching;
    }

    float targetHeight = isCrouching ? crouchHeight : standHeight;
    float currentHeight = controller.height;
    float smoothedHeight = Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * crouchTransitionSpeed);

    controller.height = smoothedHeight;

    // Keep capsule centered
    controller.center = new Vector3(0f, smoothedHeight / 2f, 0f);

    // Adjust camera
    float targetCamY = smoothedHeight - 0.4f;
    Vector3 camPos = playerCamera.localPosition;
    camPos.y = Mathf.Lerp(camPos.y, targetCamY, Time.deltaTime * crouchTransitionSpeed);
    playerCamera.localPosition = camPos;
}


    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }
    
}
