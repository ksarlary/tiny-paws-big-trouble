using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MouseGuardAI : MonoBehaviour
{
    private enum State
    {
        Patrol,
        Chase,
        Attack
    }

    [Header("References")]
    [SerializeField] private Transform visual;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Animator animator;

    [Header("Patrol")]
    [SerializeField] private float patrolSpeed = 1.2f;
    [SerializeField] private float patrolDistance = 3f;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 4.5f;
    [SerializeField] private float losePlayerRange = 7f;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 2f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.6f;
    [SerializeField] private float attackRadius = 0.45f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 1.4f;

    private float attackPointBaseX;
    [SerializeField] private LayerMask playerLayer;

    private Rigidbody2D rb;
    private Transform player;

    private State currentState;

    private Vector2 spawnPosition;

    private int patrolDirection = 1;
    [Header("Facing")]
    [SerializeField] private bool spriteFacesRightByDefault = true;

    private bool facingRight = true;

    private bool isAttacking;
    private float nextAttackTime;

    private bool isStunned;
    private float stunEndTime;

    private static readonly int SpeedHash =
        Animator.StringToHash("Speed");

    private static readonly int AttackHash =
        Animator.StringToHash("Attack");

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (attackPoint != null)
        {
            attackPointBaseX =
                Mathf.Abs(attackPoint.localPosition.x);
        }

        spawnPosition = transform.position;
    }

    private void Start()
    {
        FindPlayer();

        currentState = State.Patrol;
    }

    private void Update()
    {
        if (isStunned)
        {
            if (Time.time >= stunEndTime)
            {
                isStunned = false;
            }
            else
            {
                return;
            }
        }

        if (player == null)
        {
            FindPlayer();
            return;
        }

        float distanceToPlayer =
            Vector2.Distance(
                transform.position,
                player.position
            );

        UpdateState(distanceToPlayer);
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (isStunned)
        {
            StopHorizontalMovement();
            return;
        }

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Chase:
                Chase();
                break;

            case State.Attack:
                StopHorizontalMovement();
                break;
        }
    }

    private void UpdateState(float distanceToPlayer)
    {
        if (isAttacking)
        {
            currentState = State.Attack;
            return;
        }

        if (distanceToPlayer <= attackRange)
        {
            currentState = State.Attack;

            FacePlayer();
            TryAttack();

            return;
        }

        if (distanceToPlayer <= detectionRange)
        {
            currentState = State.Chase;
            return;
        }

        if (currentState == State.Chase &&
            distanceToPlayer <= losePlayerRange)
        {
            return;
        }

        currentState = State.Patrol;
    }

    private void Patrol()
    {
        float distanceFromSpawn =
            transform.position.x - spawnPosition.x;

        if (distanceFromSpawn >= patrolDistance)
        {
            patrolDirection = -1;
        }
        else if (distanceFromSpawn <= -patrolDistance)
        {
            patrolDirection = 1;
        }

        rb.linearVelocity = new Vector2(
            patrolDirection * patrolSpeed,
            rb.linearVelocity.y
        );

        UpdateFacingDirection(patrolDirection);
    }

    private void Chase()
    {
        if (player == null)
        {
            return;
        }

        float direction =
            Mathf.Sign(
                player.position.x - transform.position.x
            );

        rb.linearVelocity = new Vector2(
            direction * chaseSpeed,
            rb.linearVelocity.y
        );

        UpdateFacingDirection(direction);
    }

    private void TryAttack()
    {
        if (isAttacking)
        {
            return;
        }

        if (Time.time < nextAttackTime)
        {
            return;
        }

        isAttacking = true;

        StopHorizontalMovement();

        if (animator != null)
        {
            animator.SetTrigger(AttackHash);
        }
    }

    /// <summary>
    /// Call this from an Animation Event on the spear impact frame.
    /// </summary>
    public void PerformAttack()
    {
        Debug.Log($"{name}: PerformAttack event called");

        if (attackPoint == null)
        {
            Debug.LogError(
                $"{name}: AttackPoint is not assigned.",
                this
            );

            return;
        }

        Collider2D hit = Physics2D.OverlapCircle(
            attackPoint.position,
            attackRadius,
            playerLayer
        );

        if (hit == null)
        {
            Debug.Log(
                $"{name}: attack missed, no Player collider detected."
            );

            return;
        }

        Debug.Log(
            $"{name}: attack hit collider {hit.name}"
        );

        PlayerHealth playerHealth =
            hit.GetComponentInParent<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogError(
                $"{name}: hit {hit.name}, but no PlayerHealth was found."
            );

            return;
        }

        Debug.Log(
            $"{name}: dealing {attackDamage} damage to Player."
        );

        playerHealth.TakeDamage(attackDamage);
    }

    /// <summary>
    /// Call this from an Animation Event on the final attack frame.
    /// </summary>
    public void FinishAttack()
    {
        isAttacking = false;

        nextAttackTime =
            Time.time + attackCooldown;

        if (player == null)
        {
            currentState = State.Patrol;
            return;
        }

        float distanceToPlayer =
            Vector2.Distance(
                transform.position,
                player.position
            );

        if (distanceToPlayer <= detectionRange)
        {
            currentState = State.Chase;
        }
        else
        {
            currentState = State.Patrol;
        }
    }

    private void StopHorizontalMovement()
    {
        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );
    }

    public void Stun(float duration)
    {
        isAttacking = false;
        isStunned = true;

        stunEndTime = Time.time + duration;

        StopHorizontalMovement();

        if (animator != null)
        {
            animator.ResetTrigger(AttackHash);
        }
    }

    private void FacePlayer()
    {
        if (player == null)
        {
            return;
        }

        float direction =
            Mathf.Sign(
                player.position.x - transform.position.x
            );

        UpdateFacingDirection(direction);
    }

    private void UpdateFacingDirection(float direction)
    {
        if (direction > 0f)
        {
            SetFacingDirection(true);
        }
        else if (direction < 0f)
        {
            SetFacingDirection(false);
        }
    }


    private void FindPlayer()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null)
        {
            return;
        }

        float horizontalSpeed =
            Mathf.Abs(rb.linearVelocity.x);

        animator.SetFloat(
            SpeedHash,
            horizontalSpeed
        );
    }

    public void ResetAI()
    {
        spawnPosition = transform.position;

        isAttacking = false;
        isStunned = false;

        nextAttackTime = 0f;
        stunEndTime = 0f;

        currentState = State.Patrol;

        FindPlayer();

        StopHorizontalMovement();

        if (animator != null)
        {
            animator.ResetTrigger(AttackHash);
            animator.Rebind();
            animator.Update(0f);
        }

        Debug.Log($"{name} AI reset.");
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center =
            Application.isPlaying
                ? (Vector3)spawnPosition
                : transform.position;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
        );

        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );

        Gizmos.DrawLine(
            center + Vector3.left * patrolDistance,
            center + Vector3.right * patrolDistance
        );

        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(
                attackPoint.position,
                attackRadius
            );
        }
    }

    private void SetFacingDirection(bool faceRight)
    {
        facingRight = faceRight;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !facingRight;
        }

        if (attackPoint != null)
        {
            Vector3 position =
                attackPoint.localPosition;

            position.x = facingRight
                ? attackPointBaseX
                : -attackPointBaseX;

            attackPoint.localPosition =
                position;
        }
    }
}