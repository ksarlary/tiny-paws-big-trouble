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
    [SerializeField] private string restMessage = "A peaceful nap restores your strength...";
    [SerializeField] private float restMessageDuration = 1.5f;

    private bool playerNearby;
    private bool isResting;
    private Coroutine restCoroutine;

    private void Update()
    {
        if (!playerNearby)
        {
            return;
        }

        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryRest();
        }
    }

    private void TryRest()
    {
        if (isResting)
        {
            return;
        }

        if (restCoroutine != null)
        {
            StopCoroutine(restCoroutine);
        }

        restCoroutine = StartCoroutine(RestRoutine());
    }

    private IEnumerator RestRoutine()
    {
        isResting = true;

        Debug.Log("BASKET REST STARTED");

        if (playerHealth != null)
        {
            playerHealth.HealFull();
        }
        else
        {
            Debug.LogWarning(
                "CheckpointBasket: PlayerHealth is not assigned.",
                this
            );
        }

        if (enemyRespawnManager != null)
        {
            enemyRespawnManager.RespawnAllEnemies();
        }
        else
        {
            Debug.LogWarning(
                "CheckpointBasket: EnemyRespawnManager is not assigned.",
                this
            );
        }

        GameSaveManager.ActivateCheckpoint(
            SceneManager.GetActiveScene().name,
            checkpointEntryPointId,
            playerHealth != null ? playerHealth.MaxHealth : 4
        );

        Debug.Log(
            $"CHECKPOINT ACTIVATED | " +
            $"Scene: {SceneManager.GetActiveScene().name} | " +
            $"Entry: {checkpointEntryPointId}"
        );

        if (tutorialUI != null)
        {
            tutorialUI.ShowMessage(
                restMessage,
                restMessageDuration
            );
        }

        yield return new WaitForSecondsRealtime(restMessageDuration);

        isResting = false;
        restCoroutine = null;

        Debug.Log("BASKET REST FINISHED - CAN REST AGAIN");

        if (playerNearby && tutorialUI != null)
        {
            tutorialUI.ShowMessage(interactionMessage);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerNearby = true;

        if (!isResting && tutorialUI != null)
        {
            tutorialUI.ShowMessage(interactionMessage);
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