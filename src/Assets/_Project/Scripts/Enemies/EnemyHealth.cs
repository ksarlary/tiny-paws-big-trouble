using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D enemyCollider;
    [SerializeField] private MouseGuardAI mouseGuardAI;

    [Header("Hit Flash")]
    [SerializeField] private Color hitFlashColor = new Color(1f, 0f, 0f, 1f);
    [SerializeField] private float flashDuration = 0.12f;
    [SerializeField] private int flashCount = 2;

    [Header("Death")]
    [SerializeField] private bool disappearOnDeath = true;
    [SerializeField] private float deathAnimationDuration = 0.45f;
    [SerializeField] private float corpseStayDuration = 1.0f;

    public event Action Died;

    public event Action<Vector3> HiddenAfterDeath;

    private static readonly int DieHash = Animator.StringToHash("Die");

    private int currentHealth;
    private Color originalColor;
    private Coroutine flashCoroutine;
    private Coroutine deathCoroutine;
    private bool isDead;

    private Rigidbody2D rb;
    private float originalGravityScale;
    private RigidbodyType2D originalBodyType;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (enemyCollider == null)
        {
            enemyCollider = GetComponent<Collider2D>();
        }

        if (mouseGuardAI == null)
        {
            mouseGuardAI = GetComponent<MouseGuardAI>();
        }

        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            originalGravityScale = rb.gravityScale;
            originalBodyType = rb.bodyType;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(
            0,
            currentHealth - damage
        );

        Debug.Log(
            $"{name} took {damage} damage. HP: {currentHealth}/{maxHealth}"
        );

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        if (currentHealth == 0)
        {
            if (deathCoroutine != null)
            {
                StopCoroutine(deathCoroutine);
            }

            deathCoroutine = StartCoroutine(DeathRoutine());
            return;
        }

        flashCoroutine = StartCoroutine(HitFlashRoutine());

        if (mouseGuardAI != null)
        {
            mouseGuardAI.Stun(0.12f);
        }
    }

    private IEnumerator HitFlashRoutine()
    {
        Debug.Log(
            $"ENEMY FLASH STARTED | Renderer: {spriteRenderer?.name}"
        );

        if (spriteRenderer == null)
        {
            Debug.LogError(
                $"{name}: EnemyHealth SpriteRenderer is not assigned.",
                this
            );

            yield break;
        }

        for (int i = 0; i < flashCount; i++)
        {
            spriteRenderer.color = hitFlashColor;

            yield return new WaitForSeconds(flashDuration);

            spriteRenderer.color = originalColor;

            yield return new WaitForSeconds(flashDuration);
        }

        spriteRenderer.color = originalColor;
        flashCoroutine = null;
    }

    private IEnumerator DeathRoutine()
    {
        if (isDead)
        {
            yield break;
        }

        isDead = true;

        Debug.Log($"{name} death started.");

        Died?.Invoke();

        // Final red impact flash on the lethal hit.
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = hitFlashColor;
        }

        yield return new WaitForSeconds(flashDuration);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        // Stop AI behavior.
        if (mouseGuardAI != null)
        {
            mouseGuardAI.enabled = false;
        }

        // Freeze physics so the guard does not fall through the floor.
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Disable collider so the corpse does not block or hurt the player.
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        // Play death animation.
        if (animator != null)
        {
            animator.ResetTrigger(DieHash);
            animator.SetTrigger(DieHash);
        }
        else
        {
            Debug.LogWarning(
                $"{name}: Animator is not assigned, skipping death animation.",
                this
            );
        }

        yield return new WaitForSeconds(deathAnimationDuration);

        Debug.Log($"{name} death animation finished. Corpse staying briefly.");

        yield return new WaitForSeconds(corpseStayDuration);

        if (disappearOnDeath)
        {
            HideEnemy();
        }

        deathCoroutine = null;
    }

    private void HideEnemy()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        if (mouseGuardAI != null)
        {
            mouseGuardAI.enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Debug.Log($"{name} hidden after death.");

        HiddenAfterDeath?.Invoke(transform.position);
    }

    public void ResetHealth()
    {
        // Very important: cancel unfinished death logic.
        // Otherwise, the old death coroutine can hide the enemy again after basket respawn.
        if (deathCoroutine != null)
        {
            StopCoroutine(deathCoroutine);
            deathCoroutine = null;
        }

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        isDead = false;
        currentHealth = maxHealth;

        gameObject.SetActive(true);

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.color = originalColor;
        }

        if (rb != null)
        {
            rb.bodyType = originalBodyType;
            rb.gravityScale = originalGravityScale;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (enemyCollider != null)
        {
            enemyCollider.enabled = true;
        }

        if (animator != null)
        {
            animator.ResetTrigger(DieHash);
            animator.Rebind();
            animator.Update(0f);
        }

        if (mouseGuardAI != null)
        {
            mouseGuardAI.enabled = true;

            // This will work if you add ResetAI() to MouseGuardAI.
            // It will not cause a compile error if the method does not exist.
            mouseGuardAI.SendMessage(
                "ResetAI",
                SendMessageOptions.DontRequireReceiver
            );
        }

        Debug.Log($"{name} health reset.");
    }

    [ContextMenu("Test Enemy Flash")]
    private void TestEnemyFlash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(HitFlashRoutine());
    }
}