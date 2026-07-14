using System;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    private const string SaveFileName = "save.json";

    private static string SavePath =>
        Path.Combine(
            Application.persistentDataPath,
            SaveFileName
        );

    public static bool HasSave()
    {
        return File.Exists(SavePath);
    }

    public static void Save(GameSaveData data)
    {
        if (data == null)
        {
            Debug.LogWarning("SaveSystem: Tried to save null data.");
            return;
        }

        try
        {
            string json = JsonUtility.ToJson(
                data,
                true
            );

            File.WriteAllText(
                SavePath,
                json
            );

            Debug.Log($"SaveSystem: Game saved to {SavePath}");
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"SaveSystem: Failed to save game. {exception.Message}"
            );
        }
    }

    public static GameSaveData Load()
    {
        if (!File.Exists(SavePath))
        {
            return null;
        }

        try
        {
            string json = File.ReadAllText(SavePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                Debug.LogWarning(
                    "SaveSystem: Save file exists but is empty. Deleting corrupted save."
                );

                DeleteSave();
                return null;
            }

            GameSaveData data =
                JsonUtility.FromJson<GameSaveData>(json);

            if (data == null)
            {
                Debug.LogWarning(
                    "SaveSystem: Save file could not be parsed. Deleting corrupted save."
                );

                DeleteSave();
                return null;
            }

            return data;
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                $"SaveSystem: Failed to load save. Deleting corrupted save. {exception.Message}"
            );

            DeleteSave();
            return null;
        }
    }

    public static void DeleteSave()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                File.Delete(SavePath);
                Debug.Log("SaveSystem: Save deleted.");
            }
        }
        catch (Exception exception)
        {
            Debug.LogError(
                $"SaveSystem: Failed to delete save. {exception.Message}"
            );
        }
    }
}