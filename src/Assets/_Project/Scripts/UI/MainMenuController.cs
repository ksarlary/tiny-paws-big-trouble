using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    [Header("Panels")]
    [SerializeField] private GameObject mainButtonsRoot;
    [SerializeField] private GameObject settingsPanelRoot;

    [Header("Scenes")]
    [SerializeField] private string firstRoomSceneName = "Room01_Prison";

    private void Start()
    {
        ShowMainButtons();
        RefreshContinueButton();
    }

    private void OnEnable()
    {
        RefreshContinueButton();
    }

    private void RefreshContinueButton()
    {
        if (continueButton == null)
        {
            Debug.LogWarning("MainMenuController: Continue button is not assigned.", this);
            return;
        }

        GameSaveData data = GameSaveManager.GetSaveData();

        bool hasValidSave =
            data != null &&
            !string.IsNullOrEmpty(data.sceneName);

        continueButton.interactable = hasValidSave;

        Debug.Log($"Continue button state: {hasValidSave}");
    }

    public void StartNewGame()
    {
        SaveSystem.DeleteSave();

        SceneTransitionContext.Clear();

        SceneManager.LoadScene(firstRoomSceneName);
    }

    public void ContinueGame()
    {
        GameSaveData data = GameSaveManager.GetSaveData();

        if (data == null || string.IsNullOrEmpty(data.sceneName))
        {
            SceneManager.LoadScene(firstRoomSceneName);
            return;
        }

        SceneTransitionContext.Clear();

        SceneManager.LoadScene(data.sceneName);
    }

    public void OpenSettings()
    {
        if (mainButtonsRoot != null)
        {
            mainButtonsRoot.SetActive(false);
        }

        if (settingsPanelRoot != null)
        {
            settingsPanelRoot.SetActive(true);
        }

        Debug.Log("Main menu settings opened.");
    }

    public void CloseSettings()
    {
        ShowMainButtons();

        Debug.Log("Main menu settings closed.");
    }

    private void ShowMainButtons()
    {
        if (mainButtonsRoot != null)
        {
            mainButtonsRoot.SetActive(true);
        }

        if (settingsPanelRoot != null)
        {
            settingsPanelRoot.SetActive(false);
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit game.");

        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}