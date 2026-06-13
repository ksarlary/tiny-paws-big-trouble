using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Scene Names")]
    [SerializeField] private string firstGameSceneName = "PrototypeRoom";

    public void StartNewGame()
    {
        SceneManager.LoadScene(firstGameSceneName);
    }

    public void ContinueGame()
    {
        Debug.Log("Continue is not implemented yet.");
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

    public void QuitGame()
    {
        Debug.Log("Quit game requested.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}