using UnityEngine;

public class UIButtonSoundPlayer : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip buttonClickClip;

    [Header("Settings")]
    [SerializeField] private float volume = 1f;

    public void PlayButtonClick()
    {
        if (audioSource == null)
        {
            Debug.LogWarning("UIButtonSoundPlayer: AudioSource is missing.");
            return;
        }

        if (buttonClickClip == null)
        {
            Debug.LogWarning("UIButtonSoundPlayer: Button click clip is missing.");
            return;
        }

        audioSource.PlayOneShot(buttonClickClip, volume);
    }
}