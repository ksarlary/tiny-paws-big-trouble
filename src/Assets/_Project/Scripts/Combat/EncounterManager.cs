using UnityEngine;

public class EncounterManager : MonoBehaviour
{
    [SerializeField] private EnemyHealth[] requiredEnemies;

    [Header("Exit")]
    [SerializeField] private GameObject exitBlocker;
    [SerializeField] private Animator exitDoorAnimator;

    [Header("Respawn")]
    [SerializeField] private EnemyRespawnManager respawnManager;

    private int remainingEnemies;

    private void Start()
    {
        foreach (EnemyHealth enemy in requiredEnemies)
        {
            enemy.Died += OnEnemyDied;
        }

        if (respawnManager != null)
        {
            respawnManager.EnemiesRespawned += ResetEncounter;
        }

        ResetEncounter();
    }

    private void OnEnemyDied(EnemyHealth enemy)
    {
        remainingEnemies--;

        if (remainingEnemies <= 0)
        {
            UnlockExit();
        }
    }

    private void ResetEncounter()
    {
        remainingEnemies = requiredEnemies.Length;

        if (exitBlocker != null)
        {
            exitBlocker.SetActive(true);
        }

        if (exitDoorAnimator != null)
        {
            exitDoorAnimator.SetBool("Open", false);
        }
    }

    private void UnlockExit()
    {
        if (exitBlocker != null)
        {
            exitBlocker.SetActive(false);
        }

        if (exitDoorAnimator != null)
        {
            exitDoorAnimator.SetBool("Open", true);
        }
    }
}