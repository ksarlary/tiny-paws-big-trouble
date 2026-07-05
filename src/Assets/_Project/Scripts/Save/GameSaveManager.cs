using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameSaveManager
{
    private const string FirstGameplayScene = "Room01_Prison";

    public static bool HasSave()
    {
        return SaveSystem.HasSave();
    }

    public static void StartNewGame()
    {
        SaveSystem.DeleteSave();
    }

    public static void StartNewGame(string firstScene)
    {
        SaveSystem.DeleteSave();

        GameSaveData data = new GameSaveData
        {
            sceneName = firstScene,
            introSeen = false,
            hasPlayerState = false
        };

        SaveSystem.Save(data);
    }

    public static void MarkIntroSeen()
    {
        GameSaveData data = LoadOrCreate();

        data.introSeen = true;

        SaveSystem.Save(data);
    }

    public static string GetCurrentScene()
    {
        GameSaveData data = SaveSystem.Load();

        if (data == null || string.IsNullOrEmpty(data.sceneName))
        {
            return FirstGameplayScene;
        }

        return data.sceneName;
    }

    public static string GetCurrentScene(string fallbackScene)
    {
        GameSaveData data = SaveSystem.Load();

        if (data == null || string.IsNullOrEmpty(data.sceneName))
        {
            return fallbackScene;
        }

        return data.sceneName;
    }

    public static void SaveCurrentScene()
    {
        GameSaveData data = LoadOrCreate();

        data.sceneName = SceneManager.GetActiveScene().name;

        SaveSystem.Save(data);
    }

    public static void SaveCurrentScene(string sceneName)
    {
        GameSaveData data = LoadOrCreate();

        data.sceneName = sceneName;

        SaveSystem.Save(data);
    }

    public static void DeleteSave()
    {
        SaveSystem.DeleteSave();
    }

    private static GameSaveData LoadOrCreate()
    {
        return SaveSystem.Load() ?? new GameSaveData();
    }
}