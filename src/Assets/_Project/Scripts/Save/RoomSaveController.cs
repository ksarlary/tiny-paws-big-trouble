using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomSaveController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerSaveController playerSaveController;

    [Header("Tutorial Triggers")]
    [SerializeField] private TutorialTrigger movementTutorial;
    [SerializeField] private TutorialTrigger jumpTutorial;
    [SerializeField] private TutorialTrigger exitThoughtTutorial;

    [Header("Autosave")]
    [SerializeField] private float autosaveInterval = 2f;

    private Coroutine autosaveCoroutine;
    private bool isLoading;

    private void Start()
    {
        GameSaveData data = SaveSystem.Load();

        if (SceneTransitionContext.SpawnedFromEntryPoint)
        {
            Debug.Log(
                "Entry-point spawn detected. " +
                "Skipping saved Player position restore."
            );

            SceneTransitionContext.SpawnedFromEntryPoint = false;

            SaveGame();

            return;
        }

        if (data != null &&
            data.sceneName == SceneManager.GetActiveScene().name)
        {
            LoadGameState(data);
        }
        else
        {
            SaveGame();
        }
    }

    private void LoadOrCreateInitialSave()
    {
        GameSaveData data = SaveSystem.Load();

        bool canRestore =
            data != null &&
            data.hasPlayerState &&
            data.sceneName == SceneManager.GetActiveScene().name;

        if (canRestore)
        {
            isLoading = true;

            LoadGameState(data);

            isLoading = false;
        }
        else
        {
            SaveGame();
        }
    }

    private IEnumerator AutosaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(
                autosaveInterval
            );

            SaveGame();
        }
    }

    public void SaveGame()
    {
        if (isLoading)
        {
            return;
        }

        if (playerSaveController == null)
        {
            Debug.LogError(
                "RoomSaveController: PlayerSaveController is missing."
            );

            return;
        }

        GameSaveData data =
            SaveSystem.Load() ?? new GameSaveData();

        data.sceneName =
            SceneManager.GetActiveScene().name;

        playerSaveController.WritePlayerData(data);

        data.movementTutorialTriggered =
            movementTutorial != null &&
            movementTutorial.HasTriggered;

        data.jumpTutorialTriggered =
            jumpTutorial != null &&
            jumpTutorial.HasTriggered;

        data.exitThoughtTriggered =
            exitThoughtTutorial != null &&
            exitThoughtTutorial.HasTriggered;

        SaveSystem.Save(data);

        Debug.Log(
            $"AUTOSAVE | " +
            $"Scene: {data.sceneName} | " +
            $"Position: {data.playerPositionX:F2}, {data.playerPositionY:F2} | " +
            $"Movement: {data.movementTutorialTriggered} | " +
            $"Jump: {data.jumpTutorialTriggered}"
        );
    }

    private void LoadGameState(GameSaveData data)
    {
        playerSaveController.RestorePlayerData(data);

        if (movementTutorial != null)
        {
            movementTutorial.SetTriggeredState(
                data.movementTutorialTriggered
            );
        }

        if (jumpTutorial != null)
        {
            jumpTutorial.SetTriggeredState(
                data.jumpTutorialTriggered
            );
        }

        if (exitThoughtTutorial != null)
        {
            exitThoughtTutorial.SetTriggeredState(
                data.exitThoughtTriggered
            );
        }

        Debug.Log(
            $"LOAD | " +
            $"Position: {data.playerPositionX:F2}, {data.playerPositionY:F2} | " +
            $"Movement: {data.movementTutorialTriggered} | " +
            $"Jump: {data.jumpTutorialTriggered}"
        );
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void OnDestroy()
    {
        if (autosaveCoroutine != null)
        {
            StopCoroutine(autosaveCoroutine);
        }
    }
}