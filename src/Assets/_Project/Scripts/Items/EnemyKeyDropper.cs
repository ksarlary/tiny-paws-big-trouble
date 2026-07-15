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

    private GameObject spawnedKeyInstance;

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
        if (GameSaveManager.HasRoom03Key())
        {
            Debug.Log("Room 3 key already collected. No key drop.");
            return;
        }

        if (spawnedKeyInstance != null)
        {
            Debug.Log("Room 3 key is already spawned. No duplicate key drop.");
            return;
        }

        if (KeyPickup.Room03KeyExistsInScene)
        {
            Debug.Log("A Room 3 key already exists in the scene. No duplicate key drop.");
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

        Vector3 spawnPosition =
            corpsePosition + dropOffset;

        spawnedKeyInstance =
            Instantiate(
                keyPrefab,
                spawnPosition,
                Quaternion.identity
            );

        spawnedKeyInstance.SetActive(true);

        KeyPickup keyPickup =
            spawnedKeyInstance.GetComponent<KeyPickup>();

        if (keyPickup != null)
        {
            keyPickup.SetTutorialUI(tutorialUI);
        }

        Debug.Log($"Room 3 key dropped at {spawnPosition}.");
    }
}