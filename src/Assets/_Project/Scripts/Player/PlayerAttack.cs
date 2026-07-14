using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Transform visual;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private GameObject attackVFX;

    [Header("Attack")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private float attackDuration = 0.35f;
    [SerializeField] private float attackCooldown = 0.15f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Attack VFX")]
    [SerializeField] private float attackVFXDuration = 0.25f;

    [Header("Facing")]
    [SerializeField] private float attackPointOffsetX = 0.8f;

    private static readonly int AttackHash =
        Animator.StringToHash("Attack");

    private bool isAttacking;
    private bool canAttack = true;
    private bool facingRight = true;

    private Coroutine attackRoutine;
    private Coroutine attackVFXCoroutine;

    private void Update()
    {
        UpdateFacingDirection();

        if (!CanAttack())
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartAttack();
        }
    }

    private bool CanAttack()
    {
        if (Mouse.current == null)
        {
            return false;
        }

        if (!canAttack || isAttacking)
        {
            return false;
        }

        if (PauseMenuController.IsPaused)
        {
            return false;
        }

        if (MemoryUIController.IsMemoryOpen)
        {
            return false;
        }

        if (DialogueUIController.IsDialogueOpen)
        {
            return false;
        }

        return true;
    }

    private void UpdateFacingDirection()
    {
        if (visual == null)
        {
            return;
        }

        facingRight = visual.localScale.x > 0f;

        if (attackPoint != null)
        {
            Vector3 attackPointPosition = attackPoint.localPosition;

            attackPointPosition.x = facingRight
                ? Mathf.Abs(attackPointOffsetX)
                : -Mathf.Abs(attackPointOffsetX);

            attackPoint.localPosition = attackPointPosition;
        }

        if (attackVFX != null)
        {
            Vector3 vfxPosition = attackVFX.transform.localPosition;

            vfxPosition.x = facingRight
                ? Mathf.Abs(attackPointOffsetX)
                : -Mathf.Abs(attackPointOffsetX);

            attackVFX.transform.localPosition = vfxPosition;

            Vector3 vfxScale = attackVFX.transform.localScale;

            vfxScale.x = facingRight
                ? Mathf.Abs(vfxScale.x)
                : -Mathf.Abs(vfxScale.x);

            attackVFX.transform.localScale = vfxScale;
        }
    }

    private void StartAttack()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
        }

        attackRoutine = StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        canAttack = false;

        if (playerAnimator != null)
        {
            playerAnimator.ResetTrigger(AttackHash);
            playerAnimator.SetTrigger(AttackHash);
        }
        else
        {
            Debug.LogError(
                "PlayerAttack: Player Animator is not assigned.",
                this
            );
        }

        ShowAttackVFX();

        Debug.Log("PLAYER ATTACK STARTED");

        yield return new WaitForSeconds(attackDuration);

        isAttacking = false;

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
        attackRoutine = null;

        Debug.Log("PLAYER ATTACK READY AGAIN");
    }

    private void ShowAttackVFX()
    {
        if (attackVFX == null)
        {
            return;
        }

        if (attackVFXCoroutine != null)
        {
            StopCoroutine(attackVFXCoroutine);
        }

        attackVFX.SetActive(false);
        attackVFX.SetActive(true);

        attackVFXCoroutine = StartCoroutine(
            HideAttackVFXAfterDelay()
        );
    }

    private IEnumerator HideAttackVFXAfterDelay()
    {
        yield return new WaitForSeconds(attackVFXDuration);

        if (attackVFX != null)
        {
            attackVFX.SetActive(false);
        }

        attackVFXCoroutine = null;
    }

    // Called by animation event on the active hit frame.
    public void PerformAttackHit()
    {
        if (attackPoint == null)
        {
            Debug.LogError(
                "PlayerAttack: AttackPoint is not assigned.",
                this
            );

            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRadius,
            enemyLayer
        );

        Debug.Log($"PLAYER ATTACK HIT CHECK | Hits: {hits.Length}");

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemyHealth =
                hit.GetComponentInParent<EnemyHealth>();

            if (enemyHealth == null)
            {
                continue;
            }

            enemyHealth.TakeDamage(damage);
        }
    }

    // Optional animation event. No longer required for reset.
    public void FinishAttack()
    {
        Debug.Log("PLAYER ATTACK FINISH EVENT RECEIVED");
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRadius
        );
    }
}