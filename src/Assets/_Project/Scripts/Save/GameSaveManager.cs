using UnityEngine;

public static class GameSaveManager
{
    private const string HasSaveKey = "HasSave";
    private const string CurrentSceneKey = "CurrentScene";
    private const string IntroSeenKey = "IntroSeen";

    public static bool HasSave()
    {
        return PlayerPrefs.GetInt(HasSaveKey, 0) == 1;
    }

    public static void StartNewGame(string firstGameplaySceneName)
    {
        PlayerPrefs.SetInt(HasSaveKey, 1);
        PlayerPrefs.SetString(CurrentSceneKey, firstGameplaySceneName);
        PlayerPrefs.SetInt(IntroSeenKey, 0);
        PlayerPrefs.Save();
    }

    public static void MarkIntroSeen()
    {
        PlayerPrefs.SetInt(IntroSeenKey, 1);
        PlayerPrefs.Save();
    }

    public static bool HasSeenIntro()
    {
        return PlayerPrefs.GetInt(IntroSeenKey, 0) == 1;
    }

    public static string GetCurrentScene(string fallbackSceneName)
    {
        return PlayerPrefs.GetString(CurrentSceneKey, fallbackSceneName);
    }

    public static void SaveCurrentScene(string sceneName)
    {
        PlayerPrefs.SetInt(HasSaveKey, 1);
        PlayerPrefs.SetString(CurrentSceneKey, sceneName);
        PlayerPrefs.Save();
    }

    public static void DeleteSave()
    {
        PlayerPrefs.DeleteKey(HasSaveKey);
        PlayerPrefs.DeleteKey(CurrentSceneKey);
        PlayerPrefs.DeleteKey(IntroSeenKey);
        PlayerPrefs.Save();
    }
}