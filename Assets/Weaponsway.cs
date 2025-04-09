using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Mouse Sway Settings")]
    public float swayAmount = 0.05f;
    public float maxSway = 0.1f;
    public float swaySmooth = 10f;

    [Header("Movement Bob Settings")]
    public float bobAmount = 0.02f;
    public float bobSpeed = 14f;
    public float runMultiplier = 1.6f;

    private Vector3 initialPosition;
    private float bobTimer = 0f;
    private CharacterController controller;

    void Start()
    {
        initialPosition = transform.localPosition;
        controller = FindObjectOfType<CharacterController>();
    }

    void Update()
    {
        HandleSway();
        HandleBob();
    }

    void HandleSway()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float swayX = Mathf.Clamp(-mouseX * swayAmount, -maxSway, maxSway);
        float swayY = Mathf.Clamp(-mouseY * swayAmount, -maxSway, maxSway);

        Vector3 swayOffset = new Vector3(swayX, swayY, 0f);
        transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition + swayOffset, Time.deltaTime * swaySmooth);
    }

    void HandleBob()
    {
        if (controller == null || !controller.isGrounded) return;

        Vector3 bobOffset = Vector3.zero;

        if (controller.velocity.magnitude > 0.1f)
        {
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            float speedMultiplier = isSprinting ? runMultiplier : 1f;

            bobTimer += Time.deltaTime * bobSpeed * speedMultiplier;
            bobOffset.y = Mathf.Sin(bobTimer) * bobAmount;
            bobOffset.x = Mathf.Cos(bobTimer * 2f) * bobAmount * 0.5f; // Slight side sway
        }
        else
        {
            bobTimer = 0f; // Reset when idle
        }

        transform.localPosition += bobOffset;
    }
}
