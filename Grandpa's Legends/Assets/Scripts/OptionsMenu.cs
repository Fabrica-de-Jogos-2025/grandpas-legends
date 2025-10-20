using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            masterSlider.value = AudioManager.Instance.masterVolume;
            musicSlider.value = AudioManager.Instance.musicVolume;
            sfxSlider.value = AudioManager.Instance.sfxVolume;

            masterSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
            musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
        }
        else
        {
            Debug.LogWarning("AudioManager não encontrado!");
        }
    }
}
