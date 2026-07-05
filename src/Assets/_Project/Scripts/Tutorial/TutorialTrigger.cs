using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private TutorialUIController tutorialUI;

    [TextArea]
    [SerializeField] private string message;

    [SerializeField] private float duration = 3f;
    [SerializeField] private bool triggerOnlyOnce = true;

    private bool hasTriggered;

    public bool HasTriggered => hasTriggered;

    public void SetTriggeredState(bool triggered)
    {
        hasTriggered = triggered;
    }

    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (triggerOnlyOnce && hasTriggered)
        {
            return;
        }

        hasTriggered = true;

        Debug.Log(
            $"{name} triggered. State is now: {hasTriggered}"
        );

        if (tutorialUI != null)
        {
            tutorialUI.ShowMessage(
                message,
                duration
            );
        }
    }
}