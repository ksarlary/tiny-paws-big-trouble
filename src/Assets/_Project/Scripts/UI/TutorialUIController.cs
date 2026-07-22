using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialUIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private CanvasGroup messageCanvasGroup;

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.2f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        messageCanvasGroup.alpha = 0f;
        messageCanvasGroup.interactable = false;
        messageCanvasGroup.blocksRaycasts = false;
    }

    public void ShowPersistentMessage(string message)
    {
        StopCurrentRoutine();

        messageText.text = message;
        currentRoutine = StartCoroutine(FadeTo(1f));
    }

    public void ShowTimedMessage(string message, float duration = 2f)
    {
        StopCurrentRoutine();

        messageText.text = message;
        currentRoutine = StartCoroutine(
            ShowTimedMessageRoutine(duration)
        );
    }

    public void HideMessage()
    {
        StopCurrentRoutine();
        currentRoutine = StartCoroutine(FadeTo(0f));
    }

    private IEnumerator ShowTimedMessageRoutine(float duration)
    {
        yield return FadeTo(1f);
        yield return new WaitForSecondsRealtime(duration);
        yield return FadeTo(0f);

        currentRoutine = null;
    }

    private IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = messageCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            messageCanvasGroup.alpha = Mathf.Lerp(
                startAlpha,
                targetAlpha,
                elapsed / fadeDuration
            );

            yield return null;
        }

        messageCanvasGroup.alpha = targetAlpha;
    }

    private void StopCurrentRoutine()
    {
        if (currentRoutine == null)
            return;

        StopCoroutine(currentRoutine);
        currentRoutine = null;
    }

    public void ShowMessage(string message)
    {
        ShowTimedMessage(message, 2.5f);
    }

    public void ShowMessage(string message, float duration)
    {
        ShowTimedMessage(message, duration);
    }
}