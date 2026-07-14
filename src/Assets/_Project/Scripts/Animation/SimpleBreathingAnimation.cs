using UnityEngine;

public class SimpleBreathingAnimation : MonoBehaviour
{
    [Header("Breathing")]
    [SerializeField] private float breathSpeed = 1.4f;
    [SerializeField] private float breathAmountY = 0.035f;
    [SerializeField] private float breathAmountX = 0.012f;

    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        float breath =
            (Mathf.Sin(Time.time * breathSpeed) + 1f) * 0.5f;

        float scaleX =
            originalScale.x * (1f + breath * breathAmountX);

        float scaleY =
            originalScale.y * (1f + breath * breathAmountY);

        transform.localScale = new Vector3(
            scaleX,
            scaleY,
            originalScale.z
        );
    }

    public void ResetOriginalScale()
    {
        originalScale = transform.localScale;
    }
}