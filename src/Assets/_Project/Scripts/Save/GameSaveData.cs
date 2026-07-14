using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public string sceneName;

    public bool introSeen;
    public bool hasPlayerState;

    public float playerPositionX;
    public float playerPositionY;
    public float playerPositionZ;

    public bool playerFacingRight;

    public bool movementTutorialTriggered;
    public bool jumpTutorialTriggered;
    public bool exitThoughtTriggered;

    public List<string> collectedMemoryIds = new List<string>();

    public int currentHealth = 4;

    public bool hasCheckpoint;
    public string checkpointSceneName;
    public string checkpointEntryPointId;
}