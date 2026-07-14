using System;
using UnityEngine;

public class EnemyRespawnManager : MonoBehaviour
{
    public event Action EnemiesRespawned;

    [SerializeField]
    private EnemyRespawnable[] enemies;

    public void RespawnAllEnemies()
    {
        foreach (EnemyRespawnable enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.Respawn();
            }
        }

        EnemiesRespawned?.Invoke();
    }
}