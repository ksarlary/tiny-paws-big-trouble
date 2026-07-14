using System;
using UnityEngine;

public class EnemyRespawnManager : MonoBehaviour
{
    [SerializeField] private EnemyRespawnable[] enemies;

    public event Action EnemiesRespawned;

    public void RespawnAllEnemies()
    {
        foreach (EnemyRespawnable enemy in enemies)
        {
            if (enemy == null)
            {
                continue;
            }

            enemy.Respawn();
        }

        EnemiesRespawned?.Invoke();

        Debug.Log("All enemies respawned.");
    }
}