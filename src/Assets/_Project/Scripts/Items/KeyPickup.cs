using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TutorialUIController tutorialUI;

    [Header("Message")]
    [SerializeField] private string pickupMessage =
        "You found a key. What use could it have?";

    [SerializeField] private float messageDuration = 2.5f;

    private bool collected;

    public void SetTutorialUI(TutorialUIController ui)
    {
        tutorialUI = ui;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        collected = true;

        GameSaveManager.CollectRoom03Key();

        if (tutorialUI != null)
        {
            tutorialUI.ShowMessage(
                pickupMessage,
                messageDuration
            );
        }
        else
        {
            Debug.LogWarning(
                "KeyPickup: TutorialUI is not assigned.",
                this
            );
        }

        Debug.Log("Player picked up Room 3 key.");

        Destroy(gameObject);
    }
}