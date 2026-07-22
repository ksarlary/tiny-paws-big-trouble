using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public static bool Room03KeyExistsInScene { get; private set; }

    [Header("UI")]
    [SerializeField] private TutorialUIController tutorialUI;

    [Header("Message")]
    [SerializeField] private string pickupMessage =
        "You found a key. What use could it have?";

    [SerializeField] private float messageDuration = 2.5f;

    private bool collected;

    private void OnEnable()
    {
        Room03KeyExistsInScene = true;
    }

    private void OnDisable()
    {
        if (!collected)
        {
            Room03KeyExistsInScene = false;
        }
    }

    private void OnDestroy()
    {
        if (!collected)
        {
            Room03KeyExistsInScene = false;
        }
    }

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

        Room03KeyExistsInScene = false;

        Debug.Log("Player picked up Room 3 key.");

        Destroy(gameObject);
    }
}