using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingsApplier : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Mixer Parameter Names")]
    [SerializeField] private string musicVolumeParameter = "MusicVolume";
    [SerializeField] private string sfxVolumeParameter = "SFXVolume";

    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SFXVolume";

    private const float MinVolumeDb = -80f;
    private const float MaxMusicVolumeDb = -12f;
    private const float MaxSfxVolumeDb = -6f;

    private void Start()
    {
        ApplySavedAudioSettings();
    }

    private void ApplySavedAudioSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.7f);
        float sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 0.7f);

        SetMixerVolume(musicVolumeParameter, musicVolume, MaxMusicVolumeDb);
        SetMixerVolume(sfxVolumeParameter, sfxVolume, MaxSfxVolumeDb);
    }

    private void SetMixerVolume(string parameterName, float sliderValue, float maxVolumeDb)
    {
        if (audioMixer == null)
        {
            Debug.LogWarning("AudioMixer is missing in AudioSettingsApplier.");
            return;
        }

        float volumeDb = ConvertSliderValueToDecibels(sliderValue, maxVolumeDb);
        audioMixer.SetFloat(parameterName, volumeDb);
    }

    private float ConvertSliderValueToDecibels(float sliderValue, float maxVolumeDb)
    {
        if (sliderValue <= 0.0001f)
        {
            return MinVolumeDb;
        }

        float normalizedDb = Mathf.Log10(sliderValue) * 20f;
        return Mathf.Clamp(normalizedDb + maxVolumeDb, MinVolumeDb, maxVolumeDb);
    }
}