using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    [Header("Enemies")]
    [SerializeField] private EnemyHealth[] enemies;

    [Header("Exit Lock")]
    [SerializeField] private GameObject exitBlocker;

    [Header("Respawn")]
    [SerializeField] private EnemyRespawnManager enemyRespawnManager;

    private int defeatedEnemyCount;

    private void OnEnable()
    {
        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.Died += OnEnemyDied;
            }
        }

        if (enemyRespawnManager != null)
        {
            enemyRespawnManager.EnemiesRespawned += ResetEncounter;
        }
    }

    private void OnDisable()
    {
        foreach (EnemyHealth enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.Died -= OnEnemyDied;
            }
        }

        if (enemyRespawnManager != null)
        {
            enemyRespawnManager.EnemiesRespawned -= ResetEncounter;
        }
    }

    private void Start()
    {
        ResetEncounter();
    }

    private void OnEnemyDied()
    {
        defeatedEnemyCount++;

        Debug.Log(
            $"Encounter enemy defeated: {defeatedEnemyCount}/{enemies.Length}"
        );

        if (defeatedEnemyCount >= enemies.Length)
        {
            UnlockExit();
        }
    }

    private void ResetEncounter()
    {
        defeatedEnemyCount = 0;

        if (exitBlocker != null)
        {
            exitBlocker.SetActive(true);
        }

        Debug.Log("Encounter reset. Exit locked.");
    }

    private void UnlockExit()
    {
        if (exitBlocker != null)
        {
            exitBlocker.SetActive(false);
        }

        Debug.Log("Encounter complete. Exit unlocked.");
    }
}