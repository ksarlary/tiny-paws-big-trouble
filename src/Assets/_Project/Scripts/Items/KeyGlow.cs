using UnityEngine;

public class KeyGlow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Glow Pulse")]
    [SerializeField] private Color glowColor =
        new Color(1f, 0.85f, 0.35f, 1f);

    [SerializeField] private float pulseSpeed = 3f;
    [SerializeField] private float scalePulseAmount = 0.08f;

    private Color originalColor;
    private Vector3 originalScale;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        originalScale = transform.localScale;
    }

    private void Update()
    {
        float pulse =
            (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;

        if (spriteRenderer != null)
        {
            spriteRenderer.color =
                Color.Lerp(originalColor, glowColor, pulse);
        }

        float scaleMultiplier =
            1f + pulse * scalePulseAmount;

        transform.localScale =
            originalScale * scaleMultiplier;
    }
}