using System;
using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 4;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }

    public event Action<int, int> HealthChanged;
    public event Action Died;

    private bool isDead;

    [SerializeField]
    private float invulnerabilityDuration = 0.8f;

    private bool isInvulnerable;

    [Header("Hit Flash")]
    [SerializeField] private SpriteRenderer playerSpriteRenderer;
    [SerializeField]
    private Color hitFlashColor =
        new Color(1f, 0.45f, 0.45f, 1f);

    [SerializeField] private float flashDuration = 0.08f;
    [SerializeField] private int flashCount = 3;
    private Coroutine flashCoroutine;
    private Color originalSpriteColor;

    private void Awake()
    {
        CurrentHealth = maxHealth;

        if (playerSpriteRenderer != null)
        {
            originalSpriteColor = playerSpriteRenderer.color;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvulnerable || damage <= 0)
        {
            return;
        }

        CurrentHealth = Mathf.Max(
            0,
            CurrentHealth - damage
        );

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(HitFlashRoutine());

        HealthChanged?.Invoke(
            CurrentHealth,
            maxHealth
        );

        StartCoroutine(InvulnerabilityRoutine());

        if (CurrentHealth == 0)
        {
            Die();
        }
    }

    private IEnumerator HitFlashRoutine()
    {
        if (playerSpriteRenderer == null)
        {
            Debug.LogError(
                "PlayerHealth: SpriteRenderer is not assigned.",
                this
            );

            yield break;
        }

        for (int i = 0; i < flashCount; i++)
        {
            playerSpriteRenderer.color = hitFlashColor;

            yield return new WaitForSecondsRealtime(
                flashDuration
            );

            playerSpriteRenderer.color = originalSpriteColor;

            yield return new WaitForSecondsRealtime(
                flashDuration
            );
        }

        playerSpriteRenderer.color = originalSpriteColor;
        flashCoroutine = null;
    }

    public void HealFull()
    {
        if (isDead)
        {
            return;
        }

        CurrentHealth = maxHealth;

        HealthChanged?.Invoke(
            CurrentHealth,
            maxHealth
        );
    }

    public void RestoreHealth(int amount)
    {
        CurrentHealth = Mathf.Clamp(
            amount,
            0,
            maxHealth
        );

        isDead = false;

        HealthChanged?.Invoke(
            CurrentHealth,
            maxHealth
        );
    }

    private void Die()
    {
        isDead = true;

        Debug.Log("PLAYER DIED");

        Died?.Invoke();
    }

    private IEnumerator InvulnerabilityRoutine()
    {
        isInvulnerable = true;

        yield return new WaitForSeconds(
            invulnerabilityDuration
        );

        isInvulnerable = false;
    }
}