using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Save")]
    [SerializeField] private RoomSaveController roomSaveController;

    [Header("Scenes")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public static bool IsPaused { get; private set; }
    private bool settingsOpen;

    private void Start()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        settingsOpen = false;

        pauseCanvas.SetActive(false);
    }

    private void Update()
    {
        if (MemoryUIController.IsMemoryOpen ||
            DeathRespawnController.IsRespawning)
        {
            return;
        }

        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleEscape();
        }
    }

    private void HandleEscape()
    {
        if (!IsPaused)
        {
            PauseGame();
            return;
        }

        if (settingsOpen)
        {
            CloseSettings();
            return;
        }

        ResumeGame();
    }

    public void PauseGame()
    {
        IsPaused = true;
        settingsOpen = false;

        pauseCanvas.SetActive(true);
        pausePanel.SetActive(true);
        settingsPanel.SetActive(false);

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        IsPaused = false;
        settingsOpen = false;

        Time.timeScale = 1f;

        pauseCanvas.SetActive(false);
    }

    public void OpenSettings()
    {
        settingsOpen = true;

        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsOpen = false;

        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        if (roomSaveController != null)
        {
            roomSaveController.SaveGame();
        }

        IsPaused = false;
        Time.timeScale = 1f;

        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}