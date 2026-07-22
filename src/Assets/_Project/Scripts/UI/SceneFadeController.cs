using System.Collections;
using UnityEngine;

public class SceneFadeController : MonoBehaviour
{
    public static SceneFadeController Instance { get; private set; }

    [Header("Fade")]
    [SerializeField] private CanvasGroup fadeGroup;
    [SerializeField] private float fadeInDuration = 0.8f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (fadeGroup == null)
        {
            Debug.LogError(
                "SceneFadeController: Fade Group is not assigned.",
                this
            );

            return;
        }

        fadeGroup.alpha = 1f;

        StartCoroutine(FadeInRoutine());
    }

    private IEnumerator FadeInRoutine()
    {
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(
                elapsed / fadeInDuration
            );

            fadeGroup.alpha = Mathf.Lerp(
                1f,
                0f,
                progress
            );

            yield return null;
        }

        fadeGroup.alpha = 0f;
    }

    public IEnumerator FadeOut()
    {
        if (fadeGroup == null)
        {
            Debug.LogError(
                "SceneFadeController: fadeGroup is null.",
                this
            );

            yield break;
        }

        Debug.Log(
            $"FadeOut started. Initial alpha: {fadeGroup.alpha}"
        );

        float startAlpha = fadeGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(
                elapsed / fadeInDuration
            );

            fadeGroup.alpha = Mathf.Lerp(
                startAlpha,
                1f,
                progress
            );

            yield return null;
        }

        fadeGroup.alpha = 1f;

        Debug.Log("FadeOut complete. Alpha = 1");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}