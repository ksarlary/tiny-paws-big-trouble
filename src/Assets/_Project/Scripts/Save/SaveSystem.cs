using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private const string SaveFileName = "save.json";

    private static string SavePath =>
        Path.Combine(Application.persistentDataPath, SaveFileName);

    public static bool HasSave()
    {
        return File.Exists(SavePath);
    }

    public static void Save(GameSaveData data)
    {
        if (data == null)
        {
            Debug.LogError("SaveSystem: Cannot save null data.");
            return;
        }

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(SavePath, json);

        Debug.Log($"Game saved: {SavePath}");
    }

    public static GameSaveData Load()
    {
        if (!HasSave())
        {
            return null;
        }

        string json = File.ReadAllText(SavePath);

        return JsonUtility.FromJson<GameSaveData>(json);
    }

    public static void DeleteSave()
    {
        if (HasSave())
        {
            File.Delete(SavePath);
        }
    }
}