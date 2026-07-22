using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomTransitionTrigger : MonoBehaviour
{
    [Header("Destination")]
    [SerializeField] private string targetSceneName;
    [SerializeField] private string targetEntryPointId;

    [Header("Save")]
    [SerializeField] private RoomSaveController roomSaveController;

    private bool isTransitioning;

    private void Awake()
    {
        BoxCollider2D triggerCollider =
            GetComponent<BoxCollider2D>();

        triggerCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        isTransitioning = true;

        if (roomSaveController != null)
        {
            roomSaveController.SaveGame();
        }

        SceneTransitionContext.EntryPointId =
            targetEntryPointId;

        Time.timeScale = 1f;

        SceneManager.LoadScene(targetSceneName);
    }
}