using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LockedRoomTransitionTrigger : MonoBehaviour
{
    [Header("Lock")]
    [SerializeField] private bool requiresRoom03Key = true;
    [SerializeField] private string lockedMessage = "Closed...";
    [SerializeField] private float lockedMessageDuration = 1.5f;
    [SerializeField] private TutorialUIController tutorialUI;

    [Header("Transition")]
    [SerializeField] private string targetSceneName;
    [SerializeField] private string targetEntryPointId;
    [SerializeField] private RoomSaveController roomSaveController;

    private bool isTransitioning;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (requiresRoom03Key && !GameSaveManager.HasRoom03Key())
        {
            if (tutorialUI != null)
            {
                tutorialUI.ShowMessage(
                    lockedMessage,
                    lockedMessageDuration
                );
            }

            Debug.Log("Door is locked. Key required.");
            return;
        }

        StartCoroutine(TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {
        isTransitioning = true;

        if (roomSaveController != null)
        {
            roomSaveController.SaveGame();
        }

        SceneTransitionContext.EntryPointId =
            targetEntryPointId;

        Time.timeScale = 1f;

        if (SceneFadeController.Instance != null)
        {
            yield return SceneFadeController.Instance.FadeOut();
        }

        SceneManager.LoadScene(targetSceneName);
    }
}