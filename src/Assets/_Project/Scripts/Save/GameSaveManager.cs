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

    public static bool HasMemory(string memoryId)
    {
        GameSaveData data = SaveSystem.Load();

        if (data == null ||
            data.collectedMemoryIds == null)
        {
            return false;
        }

        return data.collectedMemoryIds.Contains(memoryId);
    }

    public static void CollectMemory(string memoryId)
    {
        GameSaveData data = LoadOrCreate();

        if (data.collectedMemoryIds == null)
        {
            data.collectedMemoryIds =
                new System.Collections.Generic.List<string>();
        }

        if (!data.collectedMemoryIds.Contains(memoryId))
        {
            data.collectedMemoryIds.Add(memoryId);
        }

        SaveSystem.Save(data);
    }

    public static void ActivateCheckpoint(
    string sceneName,
    string entryPointId,
    int fullHealth)
    {
        GameSaveData data = LoadOrCreate();

        data.hasCheckpoint = true;
        data.checkpointSceneName = sceneName;
        data.checkpointEntryPointId = entryPointId;
        data.currentHealth = fullHealth;

        SaveSystem.Save(data);
    }

    public static GameSaveData GetSaveData()
    {
        return LoadOrCreate();
    }
}