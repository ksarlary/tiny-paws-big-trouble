using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    public bool HasDoubleJump { get; private set; }

    private void Start()
    {
        HasDoubleJump = GameSaveManager.HasDoubleJump();
    }

    public void UnlockDoubleJump()
    {
        if (HasDoubleJump)
        {
            return;
        }

        HasDoubleJump = true;
        GameSaveManager.UnlockDoubleJump();

        Debug.Log("Player unlocked Double Jump.");
    }
}