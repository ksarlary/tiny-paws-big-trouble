using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class IntroCutsceneController : MonoBehaviour
{
    [Header("Fade Elements")]
    [SerializeField] private CanvasGroup logoGroup;
    [SerializeField] private CanvasGroup firstFrameGroup;

    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Scenes")]
    [SerializeField] private string nextSceneName = "Room01_Prison";

    [Header("Timing")]
    [SerializeField] private float logoFadeDuration = 2.0f;
    [SerializeField] private float firstFrameHoldDuration = 0.4f;
    [SerializeField] private float firstFrameFadeOutDuration = 0.5f;

    private void Start()
    {
        StartCoroutine(PlayIntroRoutine());
    }

    private IEnumerator PlayIntroRoutine()
    {
        if (videoPlayer != null)
        {
            videoPlayer.playOnAwake = false;
            videoPlayer.Stop();
            videoPlayer.loopPointReached += OnVideoFinished;
            videoPlayer.Prepare();
        }

        SetAlpha(logoGroup, 1f);
        SetAlpha(firstFrameGroup, 0f);

        float timer = 0f;

        while (timer < logoFadeDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / logoFadeDuration);

            SetAlpha(logoGroup, Mathf.Lerp(1f, 0f, progress));
            SetAlpha(firstFrameGroup, Mathf.Lerp(0f, 1f, progress));

            yield return null;
        }

        SetAlpha(logoGroup, 0f);
        SetAlpha(firstFrameGroup, 1f);

        yield return new WaitForSeconds(firstFrameHoldDuration);

        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }

        timer = 0f;

        while (timer < firstFrameFadeOutDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / firstFrameFadeOutDuration);

            SetAlpha(firstFrameGroup, Mathf.Lerp(1f, 0f, progress));

            yield return null;
        }

        SetAlpha(firstFrameGroup, 0f);
    }

    private void OnVideoFinished(VideoPlayer player)
    {
        GameSaveManager.MarkIntroSeen();
        GameSaveManager.SaveCurrentScene(nextSceneName);

        SceneManager.LoadScene(nextSceneName);
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }

    private void SetAlpha(CanvasGroup group, float alpha)
    {
        if (group != null)
        {
            group.alpha = alpha;
        }
    }
}