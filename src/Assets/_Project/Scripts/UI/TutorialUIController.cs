using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup messageGroup;
    [SerializeField] private TMP_Text messageText;

    [Header("Timing")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float defaultDisplayDuration = 3f;

    private Coroutine currentMessageRoutine;

    public void ShowMessage(string message)
    {
        ShowMessage(message, defaultDisplayDuration);
    }

    public void ShowMessage(string message, float duration)
    {
        if (currentMessageRoutine != null)
        {
            StopCoroutine(currentMessageRoutine);
        }

        currentMessageRoutine = StartCoroutine(
            ShowMessageRoutine(message, duration)
        );
    }

    private IEnumerator ShowMessageRoutine(string message, float duration)
    {
        messageText.text = message;

        yield return FadeMessage(0f, 1f);

        yield return new WaitForSeconds(duration);

        yield return FadeMessage(1f, 0f);

        currentMessageRoutine = null;
    }

    private IEnumerator FadeMessage(float startAlpha, float endAlpha)
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(
                timer / fadeDuration
            );

            messageGroup.alpha = Mathf.Lerp(
                startAlpha,
                endAlpha,
                progress
            );

            yield return null;
        }

        messageGroup.alpha = endAlpha;
    }
}