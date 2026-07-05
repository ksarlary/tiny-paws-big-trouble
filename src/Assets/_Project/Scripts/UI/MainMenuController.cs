using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    [Header("Scenes")]
    [SerializeField] private string fallbackGameplaySceneName = "PrototypeRoom";

    private void Start()
    {
        UpdateContinueButtonState();
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void ContinueGame()
    {
        if (!GameSaveManager.HasSave())
        {
            Debug.Log("No save found.");
            return;
        }

        string sceneToLoad = GameSaveManager.GetCurrentScene(fallbackGameplaySceneName);

        if (!Application.CanStreamedLevelBeLoaded(sceneToLoad))
        {
            Debug.LogError(
                "Cannot continue. Scene is missing from Build Settings: " + sceneToLoad
            );
            return;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    public void QuitGame()
    {
        Debug.Log("Quit game requested.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void UpdateContinueButtonState()
    {
        if (continueButton != null)
        {
            continueButton.interactable = GameSaveManager.HasSave();
        }
    }
}