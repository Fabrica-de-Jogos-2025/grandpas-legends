using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Áudios")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Músicas")]
    public AudioClip defaultMusic;
    public AudioClip battleMusic;

    [Header("Volumes")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            LoadVolumeSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic(defaultMusic);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.ToLower().Contains("batalha"))
        {
            if (musicSource.clip != battleMusic)
                PlayMusic(battleMusic);
        }
        else
        {
            if (musicSource.clip != defaultMusic)
                PlayMusic(defaultMusic);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
        UpdateVolumes();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, masterVolume * sfxVolume);
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        UpdateVolumes();
        SaveVolumeSettings();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        UpdateVolumes();
        SaveVolumeSettings();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        UpdateVolumes();
        SaveVolumeSettings();
    }

    private void UpdateVolumes()
    {
        musicSource.volume = masterVolume * musicVolume;
        sfxSource.volume = masterVolume * sfxVolume;
    }

    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    private void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }
}
