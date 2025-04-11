using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Mouse Sway Settings")]
    public float swayAmount = 0.05f;     // How sensitive the sway is to mouse movement
    public float maxSway = 0.1f;         // Max limit for sway in any direction
    public float swaySmooth = 10f;       // How smooth (or snappy) the sway returns to center

    [Header("Movement Bob Settings")]
    public float bobAmount = 0.02f;      // How much the weapon bobs up/down & side/side when moving
    public float bobSpeed = 14f;         // Speed of the bobbing motion
    public float runMultiplier = 1.6f;   // Extra bobbing intensity when sprinting

    private Vector3 initialPosition;     // Starting local position of the weapon
    private float bobTimer = 0f;         // Timer used for controlling bob movement
    private CharacterController controller; // Reference to player movement controller

    void Start()
    {
        // Store starting position of the weapon
        initialPosition = transform.localPosition;

        // Find the CharacterController in the scene
        controller = FindObjectOfType<CharacterController>();
    }

    void Update()
    {
        // Calls horizontal/vertical mouse-based weapon sway
        HandleSway();

        // Calls walking/running weapon bobbing motion
        HandleBob();
    }

    // Controls weapon sway when moving the mouse
    void HandleSway()
    {
        // Get mouse movement input
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Calculate sway offset based on input and clamp to max limits
        float swayX = Mathf.Clamp(-mouseX * swayAmount, -maxSway, maxSway);
        float swayY = Mathf.Clamp(-mouseY * swayAmount, -maxSway, maxSway);

        // Final sway movement applied relative to initial weapon position
        Vector3 swayOffset = new Vector3(swayX, swayY, 0f);

        // Smoothly interpolate weapon's position to apply sway effect
        transform.localPosition = Vector3.Lerp(transform.localPosition, initialPosition + swayOffset, Time.deltaTime * swaySmooth);
    }

    // Controls the weapon bobbing motion when walking/running(This took 300 years to not look insane)
    void HandleBob()
    {
        // Don't bob if not grounded or if CharacterController is missing
        if (controller == null || !controller.isGrounded) return;

        Vector3 bobOffset = Vector3.zero;

        // If player is moving (velocity is significant)
        if (controller.velocity.magnitude > 0.1f)
        {
            // Increase bob speed if sprinting
            bool isSprinting = Input.GetKey(KeyCode.LeftShift);
            float speedMultiplier = isSprinting ? runMultiplier : 1f;

            // Increase bob timer over time
            bobTimer += Time.deltaTime * bobSpeed * speedMultiplier;

            // Apply sine wave for vertical bob (up/down)
            bobOffset.y = Mathf.Sin(bobTimer) * bobAmount;

            // Apply cosine wave for slight side-to-side bob
            bobOffset.x = Mathf.Cos(bobTimer * 2f) * bobAmount * 0.5f;
        }
        else
        {
            // Reset bob timer when idle for consistency
            bobTimer = 0f;
        }

        // Apply bob offset to weapon's position
        transform.localPosition += bobOffset;
    }
}
