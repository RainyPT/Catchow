using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float crouchSpeed = 3f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f;
    public Camera playerCamera; // now take the Camera directly

    [Header("Camera Offset")]
    public Vector3 cameraOffset = new Vector3(0f, 0.2f, 0.5f); // forward and slightly up
    public float cameraSmoothSpeed = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    [Header("Animation")]
    public Animator animator; // assign your animator in Inspector

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    private float xRotation = 0f;
    private Vector3 cameraInitialLocalPos;
    private bool isPlayingAction = false;
    // crouch handling
    private bool isCrouching = false;
    private float originalHeight;
    private Vector3 originalCenter;
    public float crouchHeight = 1.0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        animator.applyRootMotion = false;

        /* if (playerCamera != null)
        {
            cameraInitialLocalPos = playerCamera.transform.localPosition;
            playerCamera.transform.localPosition = cameraInitialLocalPos + cameraOffset;
        } */
        originalHeight = controller.height;
        originalCenter = controller.center;
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
        //UpdateCameraOffset();
        HandleCrouch();
        HandleActionAnimation();
    }

    private void HandleMovement()
    {
        if (isPlayingAction) return;
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
            
        // Input (WASD)
        Vector2 input = Keyboard.current != null
            ? new Vector2(
                (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0),
                (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0)
            )
            : Vector2.zero;

        // Cancel action animation if player moves
        if (isPlayingAction && input.magnitude > 0.1f)
        {
            isPlayingAction = false;
            animator.ResetTrigger("Action");
            animator.SetBool("isWalking", true);
        }

        float currentSpeed = isCrouching ? crouchSpeed : moveSpeed;

        Vector3 move = transform.right * input.x + transform.forward * input.y;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Animation: running only if moving on X/Z
        bool isMoving = move.magnitude > 0.1f;
        if (!isPlayingAction) // only set running if not doing action
            animator.SetBool("isWalking", isMoving && !isCrouching);

        // Jump
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

    }

    private void HandleLook()
    {
        if (playerCamera == null || Mouse.current == null) return;

        float mouseX = Mouse.current.delta.x.ReadValue() * mouseSensitivity * Time.deltaTime;
        float mouseY = Mouse.current.delta.y.ReadValue() * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -75f, 75f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void UpdateCameraOffset()
    {
        if (playerCamera == null) return;

        Vector3 targetPos = cameraInitialLocalPos + cameraOffset;
        playerCamera.transform.localPosition = Vector3.Lerp(playerCamera.transform.localPosition, targetPos, Time.deltaTime * cameraSmoothSpeed);
    }

    private void HandleActionAnimation()
    {
        // Trigger one-off animation on pressing number 1
        if (!isPlayingAction && Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            isPlayingAction = true;
            animator.SetTrigger("Action"); // make sure you have a Trigger called "Action" in Animator
        }

        // Check if the animation finished
        if (isPlayingAction)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if (!state.IsName("Action")) // assumes the action returns to idle
            {
                isPlayingAction = false;
            }
        }
    }
    
    private void HandleCrouch()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.leftCtrlKey.isPressed)
        {
            if (!isCrouching)
            {
                isCrouching = true;
                controller.height = crouchHeight; // shrink collider
                controller.center = new Vector3(0, (float)0.6, 0);
                animator.SetBool("isCrouched", true);
            }
        }
        else
        {
            if (isCrouching)
            {
                isCrouching = false;
                controller.height = originalHeight; // reset collider
                controller.center = originalCenter;
                animator.SetBool("isCrouched", false);
            }
        }
    }
}
