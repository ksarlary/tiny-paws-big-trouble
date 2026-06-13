using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle fullscreenToggle;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SfxVolume";
    private const string FullscreenKey = "Fullscreen";

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
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.8f);
        float sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 0.8f);
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
        Debug.Log("Music volume: " + value);
    }

    private void ApplySfxVolume(float value)
    {
        Debug.Log("SFX volume: " + value);
    }
}