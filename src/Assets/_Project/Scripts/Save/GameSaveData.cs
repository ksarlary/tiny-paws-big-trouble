using System;

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
}