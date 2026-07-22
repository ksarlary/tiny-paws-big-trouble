using UnityEngine;

public class SceneEntryController : MonoBehaviour
{
    private void Awake()
    {
        if (string.IsNullOrEmpty(
                SceneTransitionContext.EntryPointId))
        {
            SceneTransitionContext.SpawnedFromEntryPoint = false;
            return;
        }

        PlayerSaveController player =
            FindFirstObjectByType<PlayerSaveController>();

        if (player == null)
        {
            Debug.LogError(
                "SceneEntryController: Player not found."
            );

            return;
        }

        RoomEntryPoint[] entryPoints =
            FindObjectsByType<RoomEntryPoint>(
                FindObjectsSortMode.None
            );

        foreach (RoomEntryPoint entryPoint in entryPoints)
        {
            if (entryPoint.EntryPointId !=
                SceneTransitionContext.EntryPointId)
            {
                continue;
            }

            player.transform.position =
                entryPoint.transform.position;

            Rigidbody2D rb =
                player.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            SceneTransitionContext.SpawnedFromEntryPoint = true;

            Debug.Log(
                $"Player entered at: " +
                $"{entryPoint.EntryPointId}"
            );

            SceneTransitionContext.ClearEntryPoint();

            return;
        }

        Debug.LogError(
            $"No RoomEntryPoint found with ID: " +
            $"{SceneTransitionContext.EntryPointId}"
        );
    }
}