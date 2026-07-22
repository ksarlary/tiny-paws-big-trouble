using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject pauseMenuRoot;
    [SerializeField] private GameObject settingsPanelRoot;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Awake()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuRoot != null)
        {
            pauseMenuRoot.SetActive(false);
        }

        if (settingsPanelRoot != null)
        {
            settingsPanelRoot.SetActive(false);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (DialogueUIController.IsDialogueOpen ||
                DialogueManager.IsDialogueBusy)
            {
                return;
            }

            if (settingsPanelRoot != null && settingsPanelRoot.activeSelf)
            {
                CloseSettings();
                return;
            }

            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;

        if (pauseMenuRoot != null)
        {
            pauseMenuRoot.SetActive(true);
        }

        if (settingsPanelRoot != null)
        {
            settingsPanelRoot.SetActive(false);
        }

        Debug.Log("Game paused.");
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuRoot != null)
        {
            pauseMenuRoot.SetActive(false);
        }

        if (settingsPanelRoot != null)
        {
            settingsPanelRoot.SetActive(false);
        }

        Debug.Log("Game resumed.");
    }

    public void OpenSettings()
    {
        if (!IsPaused)
        {
            PauseGame();
        }

        if (settingsPanelRoot != null)
        {
            settingsPanelRoot.SetActive(true);
        }

        Debug.Log("Pause settings opened.");
    }

    public void CloseSettings()
    {
        if (settingsPanelRoot != null)
        {
            settingsPanelRoot.SetActive(false);
        }

        Debug.Log("Pause settings closed.");
    }

    public void ReturnToMainMenu()
    {
        GoToMainMenu();
    }

    public void BackToMainMenu()
    {
        GoToMainMenu();
    }

    private void GoToMainMenu()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        if (pauseMenuRoot != null)
        {
            pauseMenuRoot.SetActive(false);
        }

        if (settingsPanelRoot != null)
        {
            settingsPanelRoot.SetActive(false);
        }

        SceneTransitionContext.Clear();

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}