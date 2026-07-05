using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 11f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Animator animator;
    [SerializeField] private Transform visual;

    [Header("Audio")]
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioClip landingClip;

    private Rigidbody2D rb;
    private float horizontalInput;
    private bool jumpRequested;

    private bool wasGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        ReadMovementInput();
        ReadJumpInput();
        UpdateAnimator();
        UpdateFacingDirection();
    }

    private void UpdateAnimator()
    {
        bool grounded = IsGrounded();

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(horizontalInput));
            animator.SetFloat("VerticalVelocity", rb.linearVelocity.y);
            animator.SetBool("IsGrounded", grounded);
        }

        if (!wasGrounded && grounded)
        {
            PlayLandingSound();
        }

        wasGrounded = grounded;
    }

    private void PlayLandingSound()
    {
        if (sfxAudioSource == null || landingClip == null)
        {
            return;
        }

        sfxAudioSource.PlayOneShot(landingClip);
    }

    private void UpdateFacingDirection()
    {
        if (visual == null)
        {
            return;
        }

        Vector3 scale = visual.localScale;

        if (horizontalInput > 0f)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else if (horizontalInput < 0f)
        {
            scale.x = -Mathf.Abs(scale.x);
        }

        visual.localScale = scale;
    }

    private void Start()
    {
        wasGrounded = IsGrounded();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            horizontalInput * moveSpeed,
            rb.linearVelocity.y
        );

        if (jumpRequested)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            jumpRequested = false;
        }
    }

    private void ReadMovementInput()
    {
        horizontalInput = 0f;

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            horizontalInput -= 1f;
        }

        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            horizontalInput += 1f;
        }
    }

    private void ReadJumpInput()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        if (keyboard.spaceKey.wasPressedThisFrame && IsGrounded())
        {
            jumpRequested = true;
        }
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            return false;
        }

        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius
        );
    }
}