using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private PlayerAbilities playerAbilities;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.18f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Visual")]
    [SerializeField] private Transform visual;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;

    private float horizontalInput;
    private bool jumpRequested;
    private bool isGrounded;
    private bool wasGrounded;
    private bool hasUsedDoubleJump;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int VerticalVelocityHash = Animator.StringToHash("VerticalVelocity");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int LandHash = Animator.StringToHash("Land");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (playerAbilities == null)
        {
            playerAbilities = GetComponent<PlayerAbilities>();
        }
    }

    private void Update()
    {
        ReadInput();
        UpdateGroundedState();
        UpdateFacingDirection();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        Move();
        HandleJump();
    }

    private void ReadInput()
    {
        if (Keyboard.current == null)
        {
            horizontalInput = 0f;
            jumpRequested = false;
            return;
        }

        if (PauseMenuController.IsPaused ||
    MemoryUIController.IsMemoryOpen ||
    DialogueUIController.IsDialogueOpen)
        {
            horizontalInput = 0f;
            jumpRequested = false;
            return;
        }

        horizontalInput = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
        {
            horizontalInput = -1f;
        }
        else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
        {
            horizontalInput = 1f;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            jumpRequested = true;
        }
    }

    private void Move()
    {
        if (rb == null)
        {
            return;
        }

        rb.linearVelocity = new Vector2(
            horizontalInput * moveSpeed,
            rb.linearVelocity.y
        );
    }

    private void HandleJump()
    {
        if (!jumpRequested)
        {
            return;
        }

        jumpRequested = false;

        if (rb == null)
        {
            return;
        }

        if (isGrounded)
        {
            PerformJump();
            hasUsedDoubleJump = false;
            return;
        }

        if (CanDoubleJump())
        {
            PerformJump();
            hasUsedDoubleJump = true;
        }
    }

    private bool CanDoubleJump()
    {
        if (playerAbilities == null)
        {
            return false;
        }

        if (!playerAbilities.HasDoubleJump)
        {
            return false;
        }

        if (hasUsedDoubleJump)
        {
            return false;
        }

        return true;
    }

    private void PerformJump()
    {
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            jumpForce
        );

        if (animator != null)
        {
            animator.ResetTrigger(JumpHash);
            animator.SetTrigger(JumpHash);
        }
    }

    private void UpdateGroundedState()
    {
        wasGrounded = isGrounded;

        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (isGrounded)
        {
            hasUsedDoubleJump = false;
        }

        if (!wasGrounded && isGrounded)
        {
            if (animator != null)
            {
                animator.ResetTrigger(LandHash);
                animator.SetTrigger(LandHash);
            }
        }
    }

    private void UpdateFacingDirection()
    {
        if (visual == null)
        {
            return;
        }

        if (horizontalInput > 0.01f)
        {
            Vector3 scale = visual.localScale;
            scale.x = Mathf.Abs(scale.x);
            visual.localScale = scale;
        }
        else if (horizontalInput < -0.01f)
        {
            Vector3 scale = visual.localScale;
            scale.x = -Mathf.Abs(scale.x);
            visual.localScale = scale;
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null || rb == null)
        {
            return;
        }

        animator.SetFloat(
            SpeedHash,
            Mathf.Abs(horizontalInput)
        );

        animator.SetBool(
            IsGroundedHash,
            isGrounded
        );

        animator.SetFloat(
            VerticalVelocityHash,
            rb.linearVelocity.y
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}