using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuTransitionController : MonoBehaviour
{
    [Header("Fade Groups")]
    [SerializeField] private CanvasGroup menuFadeGroup;
    [SerializeField] private CanvasGroup blackFadeGroup;

    [Header("Scenes")]
    [SerializeField] private string introSceneName = "IntroCutscene";
    [SerializeField] private string firstGameplaySceneName = "PrototypeRoom";

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float titleHoldDuration = 0.8f;

    private bool isStartingGame;

    public void StartNewGame()
    {
        if (isStartingGame)
        {
            return;
        }

        StartCoroutine(StartNewGameRoutine());
    }

    private IEnumerator StartNewGameRoutine()
    {
        isStartingGame = true;

        // Create/reset save for a new game.
        GameSaveManager.StartNewGame(firstGameplaySceneName);

        if (menuFadeGroup != null)
        {
            menuFadeGroup.interactable = false;
            menuFadeGroup.blocksRaycasts = false;
        }

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / fadeDuration);

            if (menuFadeGroup != null)
            {
                menuFadeGroup.alpha = Mathf.Lerp(1f, 0f, progress);
            }

            if (blackFadeGroup != null)
            {
                blackFadeGroup.alpha = Mathf.Lerp(0f, 1f, progress);
            }

            yield return null;
        }

        if (menuFadeGroup != null)
        {
            menuFadeGroup.alpha = 0f;
        }

        if (blackFadeGroup != null)
        {
            blackFadeGroup.alpha = 1f;
        }

        yield return new WaitForSeconds(titleHoldDuration);

        if (!Application.CanStreamedLevelBeLoaded(introSceneName))
        {
            Debug.LogError(
                "Cannot load intro scene: " + introSceneName +
                ". Check that it is added to Build Settings."
            );

            yield break;
        }

        SceneManager.LoadScene(introSceneName);
    }
}