using UnityEngine;

public class EnemyKeyDropper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private GameObject keyPrefab;
    [SerializeField] private TutorialUIController tutorialUI;

    [Header("Drop Settings")]
    [SerializeField] private Vector3 dropOffset =
        new Vector3(0f, 0.6f, 0f);

    private bool droppedThisScene;

    private void Awake()
    {
        if (enemyHealth == null)
        {
            enemyHealth = GetComponent<EnemyHealth>();
        }
    }

    private void OnEnable()
    {
        if (enemyHealth != null)
        {
            enemyHealth.HiddenAfterDeath += DropKey;
        }
    }

    private void OnDisable()
    {
        if (enemyHealth != null)
        {
            enemyHealth.HiddenAfterDeath -= DropKey;
        }
    }

    private void DropKey(Vector3 corpsePosition)
    {
        if (droppedThisScene)
        {
            return;
        }

        if (GameSaveManager.HasRoom03Key())
        {
            return;
        }

        if (GameSaveManager.WasRoom03KeyDropped())
        {
            return;
        }

        if (keyPrefab == null)
        {
            Debug.LogError(
                "EnemyKeyDropper: Key prefab is not assigned.",
                this
            );

            return;
        }

        droppedThisScene = true;
        GameSaveManager.MarkRoom03KeyDropped();

        Vector3 spawnPosition =
            corpsePosition + dropOffset;

        GameObject keyInstance =
            Instantiate(
                keyPrefab,
                spawnPosition,
                Quaternion.identity
            );

        keyInstance.SetActive(true);

        KeyPickup keyPickup =
            keyInstance.GetComponent<KeyPickup>();

        if (keyPickup != null)
        {
            keyPickup.SetTutorialUI(tutorialUI);
        }

        Debug.Log($"Room 3 key dropped at {spawnPosition}.");
    }
}