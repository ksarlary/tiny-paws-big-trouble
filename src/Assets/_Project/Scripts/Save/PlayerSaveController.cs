using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerSaveController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visual;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void WritePlayerData(GameSaveData data)
    {
        Vector3 position = transform.position;

        data.playerPositionX = position.x;
        data.playerPositionY = position.y;
        data.playerPositionZ = position.z;

        data.playerFacingRight =
            visual == null ||
            visual.localScale.x > 0f;

        data.hasPlayerState = true;
    }

    public void RestorePlayerData(GameSaveData data)
{
    if (data == null || !data.hasPlayerState)
    {
        Debug.LogWarning(
            "PlayerSaveController: No valid player state to restore."
        );

        return;
    }

    transform.position = new Vector3(
        data.playerPositionX,
        data.playerPositionY,
        data.playerPositionZ
    );

    rb.linearVelocity = Vector2.zero;

    RestoreFacingDirection(
        data.playerFacingRight
    );
}

    // Compatibility with previous GameSaveManager code
    public GameSaveData CreateSaveData()
    {
        GameSaveData data = new GameSaveData();

        WritePlayerData(data);

        return data;
    }

    // Compatibility with previous GameSaveManager code
    public void ApplySaveData(GameSaveData data)
    {
        RestorePlayerData(data);
    }

    private void RestoreFacingDirection(bool facingRight)
    {
        if (visual == null)
        {
            return;
        }

        Vector3 scale = visual.localScale;

        scale.x = facingRight
            ? Mathf.Abs(scale.x)
            : -Mathf.Abs(scale.x);

        visual.localScale = scale;
    }
}