using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathRespawnController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Rigidbody2D playerRigidbody;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Timing")]
    [SerializeField] private float deathAnimationDuration = 1.2f;
    [SerializeField] private float pauseBeforeFade = 0.2f;

    public static bool IsRespawning { get; private set; }

    [SerializeField] private SceneFadeController sceneFadeController;

    private static readonly int DieHash =
        Animator.StringToHash("Die");

    private bool respawning;

    private void OnEnable()
    {
        if (playerHealth != null)
        {
            playerHealth.Died += HandleDeath;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.Died -= HandleDeath;
        }
    }

    private void HandleDeath()
    {
        if (respawning)
        {
            return;
        }

        respawning = true;
        IsRespawning = true;

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        // Stop Player controls.
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Stop Player movement.
        if (playerRigidbody != null)
        {
            playerRigidbody.linearVelocity = Vector2.zero;
        }

        // Play death animation.
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger(DieHash);
        }

        // Wait for the death animation.
        yield return new WaitForSecondsRealtime(
            deathAnimationDuration
        );

        yield return new WaitForSecondsRealtime(
            pauseBeforeFade
        );

        // Fade to black.
        if (sceneFadeController != null)
        {
            Debug.Log("Starting death fade out");

            yield return sceneFadeController.FadeOut();

            Debug.Log("Death fade out finished");
        }
        else
        {
            Debug.LogError(
                "DeathRespawnController: SceneFadeController is not assigned.",
                this
            );
        }

        GameSaveData data =
            GameSaveManager.GetSaveData();
        Debug.Log(
$"DEATH SAVE DATA | " +
$"HasCheckpoint: {data.hasCheckpoint} | " +
$"Scene: {data.checkpointSceneName} | " +
$"Entry: {data.checkpointEntryPointId}"
);

        Time.timeScale = 1f;

        if (data != null && data.hasCheckpoint)
        {
            data.currentHealth =
                playerHealth.MaxHealth;

            SaveSystem.Save(data);

            SceneTransitionContext.EntryPointId =
                data.checkpointEntryPointId;

            Time.timeScale = 1f;

            SceneManager.LoadScene(
                data.checkpointSceneName
            );
        }
        else
        {
            // No activated basket yet:
            // reload current room.
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().name
            );
        }
    }
}