using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CheckpointBasket : MonoBehaviour
{
    [Header("Checkpoint")]
    [SerializeField] private string checkpointEntryPointId;

    [Header("Gameplay References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private EnemyRespawnManager enemyRespawnManager;

    [Header("UI")]
    [SerializeField] private TutorialUIController tutorialUI;

    [Header("Messages")]
    [SerializeField] private string interactionMessage = "Press E to cat nap";
    [SerializeField] private string restMessage =
        "A peaceful nap restores your strength...";

    [SerializeField] private float restMessageDuration = 1.5f;

    private bool playerNearby;
    private bool isResting;

    private void Update()
    {
        if (!playerNearby || isResting)
        {
            return;
        }

        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartCoroutine(RestRoutine());
        }
    }

    private IEnumerator RestRoutine()
    {
        isResting = true;

        // Restore Player health.
        if (playerHealth != null)
        {
            playerHealth.HealFull();
        }
        else
        {
            Debug.LogError(
                "CheckpointBasket: PlayerHealth is not assigned.",
                this
            );
        }

        // Respawn all room enemies.
        if (enemyRespawnManager != null)
        {
            enemyRespawnManager.RespawnAllEnemies();
        }

        // Save checkpoint.
        GameSaveManager.ActivateCheckpoint(
            SceneManager.GetActiveScene().name,
            checkpointEntryPointId,
            playerHealth != null
                ? playerHealth.MaxHealth
                : 4
        );

        Debug.Log(
            $"CHECKPOINT ACTIVATED | " +
            $"Scene: {SceneManager.GetActiveScene().name} | " +
            $"Entry: {checkpointEntryPointId}"
        );

        // Show rest message.
        if (tutorialUI != null)
        {
            tutorialUI.ShowMessage(
                restMessage,
                restMessageDuration
            );
        }

        yield return new WaitForSecondsRealtime(
            restMessageDuration
        );

        isResting = false;

        if (playerNearby && tutorialUI != null)
        {
            tutorialUI.ShowMessage(
                interactionMessage
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerNearby = true;

        if (tutorialUI != null)
        {
            tutorialUI.ShowMessage(
                interactionMessage
            );
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerNearby = false;

        if (tutorialUI != null)
        {
            tutorialUI.HideMessage();
        }
    }
}