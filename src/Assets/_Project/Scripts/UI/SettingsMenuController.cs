using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Mixer Parameter Names")]
    [SerializeField] private string musicVolumeParameter = "MusicVolume";
    [SerializeField] private string sfxVolumeParameter = "SFXVolume";

    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SFXVolume";
    private const string FullscreenKey = "Fullscreen";

    private const float MinVolumeDb = -80f;

    // Maximum allowed loudness.
    // Music is intentionally capped lower because the source file is too loud.
    private const float MaxMusicVolumeDb = -12f;
    private const float MaxSfxVolumeDb = -6f;

    private void Start()
    {
        LoadSettings();

        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }
    }

    private void LoadSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.7f);
        float sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 0.7f);
        bool isFullscreen = PlayerPrefs.GetInt(FullscreenKey, Screen.fullScreen ? 1 : 0) == 1;

        if (musicSlider != null)
        {
            musicSlider.value = musicVolume;
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVolume;
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = isFullscreen;
        }

        ApplyMusicVolume(musicVolume);
        ApplySfxVolume(sfxVolume);
        Screen.fullScreen = isFullscreen;
    }

    private void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
        PlayerPrefs.Save();

        ApplyMusicVolume(value);
    }

    private void SetSfxVolume(float value)
    {
        PlayerPrefs.SetFloat(SfxVolumeKey, value);
        PlayerPrefs.Save();

        ApplySfxVolume(value);
    }

    private void SetFullscreen(bool isFullscreen)
    {
        PlayerPrefs.SetInt(FullscreenKey, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();

        Screen.fullScreen = isFullscreen;
    }

    private void ApplyMusicVolume(float value)
    {
        SetMixerVolume(musicVolumeParameter, value, MaxMusicVolumeDb);
    }

    private void ApplySfxVolume(float value)
    {
        SetMixerVolume(sfxVolumeParameter, value, MaxSfxVolumeDb);
    }

    private void SetMixerVolume(string parameterName, float sliderValue, float maxVolumeDb)
    {
        if (audioMixer == null)
        {
            Debug.LogWarning("AudioMixer is missing in SettingsMenuController.");
            return;
        }

        float volumeDb = ConvertSliderValueToDecibels(sliderValue, maxVolumeDb);
        bool parameterFound = audioMixer.SetFloat(parameterName, volumeDb);

        if (!parameterFound)
        {
            Debug.LogWarning("AudioMixer parameter not found: " + parameterName);
        }
    }

    private float ConvertSliderValueToDecibels(float sliderValue, float maxVolumeDb)
    {
        if (sliderValue <= 0.0001f)
        {
            return MinVolumeDb;
        }

        float normalizedDb = Mathf.Log10(sliderValue) * 20f;

        // Normal audio formula gives:
        // 1.0 -> 0 dB
        // 0.5 -> about -6 dB
        // 0.1 -> about -20 dB
        //
        // We add maxVolumeDb to cap the loudest possible value.
        return Mathf.Clamp(normalizedDb + maxVolumeDb, MinVolumeDb, maxVolumeDb);
    }
}