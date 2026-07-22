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

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip landingSound;
    [SerializeField] private float landingSoundVolume = 0.7f;
    [SerializeField] private float minimumLandingSpeed = 2f;
    [SerializeField] private float landingSoundStartDelay = 0.2f;

    private Rigidbody2D rb;

    private float horizontalInput;
    private bool jumpRequested;
    private bool isGrounded;
    private bool wasGrounded;
    private bool hasUsedDoubleJump;

    private bool hasBeenAirborne;
    private float highestFallSpeed;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int VerticalVelocityHash = Animator.StringToHash("VerticalVelocity");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int LandHash = Animator.StringToHash("Land");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

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
            DialogueUIController.IsDialogueOpen ||
            DialogueManager.IsDialogueBusy)
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

        hasBeenAirborne = true;
        highestFallSpeed = 0f;

        if (animator != null && HasAnimatorParameter(JumpHash))
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

        if (!isGrounded)
        {
            hasBeenAirborne = true;

            if (rb != null)
            {
                float currentFallSpeed = Mathf.Max(0f, -rb.linearVelocity.y);
                highestFallSpeed = Mathf.Max(highestFallSpeed, currentFallSpeed);
            }
        }

        if (isGrounded)
        {
            hasUsedDoubleJump = false;
        }

        if (!wasGrounded && isGrounded)
        {
            if (animator != null && HasAnimatorParameter(LandHash))
            {
                animator.ResetTrigger(LandHash);
                animator.SetTrigger(LandHash);
            }

            PlayLandingSound();

            hasBeenAirborne = false;
            highestFallSpeed = 0f;
        }
    }

    private void PlayLandingSound()
    {
        if (!hasBeenAirborne)
        {
            return;
        }

        if (Time.timeSinceLevelLoad < landingSoundStartDelay)
        {
            return;
        }

        if (highestFallSpeed < minimumLandingSpeed)
        {
            return;
        }

        if (audioSource == null || landingSound == null)
        {
            return;
        }

        audioSource.PlayOneShot(
            landingSound,
            landingSoundVolume
        );
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

        if (HasAnimatorParameter(SpeedHash))
        {
            animator.SetFloat(
                SpeedHash,
                Mathf.Abs(horizontalInput)
            );
        }

        if (HasAnimatorParameter(IsGroundedHash))
        {
            animator.SetBool(
                IsGroundedHash,
                isGrounded
            );
        }

        if (HasAnimatorParameter(VerticalVelocityHash))
        {
            animator.SetFloat(
                VerticalVelocityHash,
                rb.linearVelocity.y
            );
        }
    }

    private bool HasAnimatorParameter(int parameterHash)
    {
        if (animator == null)
        {
            return false;
        }

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.nameHash == parameterHash)
            {
                return true;
            }
        }

        return false;
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