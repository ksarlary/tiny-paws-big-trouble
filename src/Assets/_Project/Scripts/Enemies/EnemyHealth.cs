using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    [Header("Hit Reaction")]
    [SerializeField] private float hitStunDuration = 0.25f;

    [Header("Death")]
    [SerializeField] private float destroyDelay = 1.2f;

    [Header("Optional Drop")]
    [SerializeField] private GameObject deathDropPrefab;
    [SerializeField] private Transform dropPoint;

    public int CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    public event Action<EnemyHealth> Died;

    private Animator animator;
    private MouseGuardAI ai;
    private Rigidbody2D rb;
    private Collider2D bodyCollider;

    private void Awake()
    {
        CurrentHealth = maxHealth;

        animator = GetComponent<Animator>();
        ai = GetComponent<MouseGuardAI>();
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
    }

    public void TakeDamage(int damage)
    {
        if (IsDead || damage <= 0)
        {
            return;
        }

        CurrentHealth = Mathf.Max(
            0,
            CurrentHealth - damage
        );

        if (CurrentHealth == 0)
        {
            Die();
            return;
        }

        animator.SetTrigger("Hit");
        ai.Stun(hitStunDuration);
    }

    private void Die()
    {
        IsDead = true;

        rb.linearVelocity = Vector2.zero;

        ai.enabled = false;
        bodyCollider.enabled = false;

        animator.SetTrigger("Death");

        Died?.Invoke(this);

        if (deathDropPrefab != null)
        {
            Vector3 spawnPosition =
                dropPoint != null
                    ? dropPoint.position
                    : transform.position;

            Instantiate(
                deathDropPrefab,
                spawnPosition,
                Quaternion.identity
            );
        }

        Destroy(gameObject, destroyDelay);
    }

    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
        IsDead = false;

        if (bodyCollider != null)
        {
            bodyCollider.enabled = true;
        }

        if (ai != null)
        {
            ai.enabled = true;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}