using UnityEngine;

public class EnemyRespawnable : MonoBehaviour
{
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    private EnemyHealth enemyHealth;
    private Rigidbody2D rb;

    private void Awake()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        enemyHealth = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Respawn()
    {
        Debug.Log($"{name} respawn requested.");

        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (enemyHealth != null)
        {
            enemyHealth.ResetHealth();
        }
        else
        {
            Debug.LogWarning(
                $"{name}: EnemyHealth missing during respawn.",
                this
            );
        }

        Debug.Log($"{name} respawned at {spawnPosition}.");
    }
}