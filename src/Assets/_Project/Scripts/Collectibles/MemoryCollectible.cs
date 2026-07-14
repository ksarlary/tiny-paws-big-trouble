using UnityEngine;

public class MemoryCollectible : MonoBehaviour
{
    [Header("Memory Identity")]
    [SerializeField] private string memoryId;

    [Header("Memory Content")]
    [SerializeField] private string memoryTitle;

    [TextArea(4, 10)]
    [SerializeField] private string memoryText;

    private bool collected;

    private void Start()
    {
        if (GameSaveManager.HasMemory(memoryId))
        {
            collected = true;
            gameObject.SetActive(false);
        }
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

        Collect();
    }

    private void Collect()
    {
        collected = true;

        MemoryUIController.Instance.ShowMemory(
            memoryTitle,
            memoryText,
            OnMemoryClosed
        );
    }

    private void OnMemoryClosed()
    {
        GameSaveManager.CollectMemory(memoryId);
        gameObject.SetActive(false);
    }
}