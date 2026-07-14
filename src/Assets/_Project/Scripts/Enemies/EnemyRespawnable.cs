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
        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        gameObject.SetActive(true);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (enemyHealth != null)
        {
            enemyHealth.ResetHealth();
        }
    }
}