using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeAudioSources();
    }
    #endregion

    [Header("Audio Mixer")]
    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private AudioMixerGroup mainMixerGroup;

    [Header("Audio Sources")]
    [SerializeField]
    private AudioSource musicSource;

    [SerializeField]
    private AudioSource sfxSource;

    [SerializeField]
    private AudioSource uiSource;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip weaponFiringSound;

    [SerializeField]
    private AudioClip mainMenuBGM;

    [SerializeField]
    private AudioClip gameplayBGM;

    [SerializeField]
    private AudioClip buttonClickedSound;

    [SerializeField]
    private AudioClip buttonHoveredSound;

    [Header("Sound Variation")]
    [SerializeField, Range(0f, 1f)]
    private float pitchFlex = 0.1f;

    // channel names for the mixer
    public const string CHANNEL_MASTER = "Master";
    public const string CHANNEL_MUSIC = "Music";
    public const string CHANNEL_SFX = "SFX";
    public const string CHANNEL_UI = "UI";
    public const string CHANNEL_WEAPONS = "Weapons";

    private void InitializeAudioSources()
    {
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            if (mainMixerGroup != null)
                musicSource.outputAudioMixerGroup = mainMixerGroup;
            Debug.Log("Created MusicSource in AudioManager");
        }

        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            if (mainMixerGroup != null)
                sfxSource.outputAudioMixerGroup = mainMixerGroup;
            Debug.Log("Created SFXSource in AudioManager");
        }

        if (uiSource == null)
        {
            GameObject uiObj = new GameObject("UISource");
            uiObj.transform.SetParent(transform);
            uiSource = uiObj.AddComponent<AudioSource>();
            uiSource.playOnAwake = false;
            if (mainMixerGroup != null)
                uiSource.outputAudioMixerGroup = mainMixerGroup;
            Debug.Log("Created UISource in AudioManager");
        }
    }

    #region Public Methods

    public static void PlayOneShot(AudioClip audio, string channel = CHANNEL_SFX)
    {
        if (Instance != null && audio != null)
        {
            Instance.sfxSource.PlayOneShot(audio);
        }
    }

    public static void PlayUIOneShot(AudioClip audio)
    {
        if (Instance != null && audio != null)
        {
            Instance.uiSource.PlayOneShot(audio);
        }
    }

    public static void PlayOneShotWithPitch(
        AudioClip audio,
        bool usePitchVariation = true,
        string channel = CHANNEL_SFX
    )
    {
        if (Instance != null && audio != null)
        {
            if (usePitchVariation)
            {
                Instance.PlayFromTempAudioSource(audio, channel);
            }
            else
            {
                if (channel == CHANNEL_UI)
                {
                    Instance.uiSource.PlayOneShot(audio);
                }
                else if (channel == CHANNEL_WEAPONS)
                {
                    Instance.PlayFromTempAudioSource(audio, channel);
                }
                else
                {
                    Instance.sfxSource.PlayOneShot(audio);
                }
            }
        }
    }

    private void PlayFromTempAudioSource(AudioClip audio, string channel = CHANNEL_SFX)
    {
        GameObject tempAudio = new GameObject("TempAudio");
        tempAudio.transform.SetParent(transform);
        AudioSource tempSource = tempAudio.AddComponent<AudioSource>();

        tempSource.clip = audio;
        tempSource.volume = sfxSource.volume;
        tempSource.spatialBlend = sfxSource.spatialBlend;

        if (mainMixerGroup != null)
        {
            tempSource.outputAudioMixerGroup = mainMixerGroup;
        }

        tempSource.pitch = Random.Range(1f - pitchFlex, 1f + pitchFlex);

        tempSource.Play();

        Destroy(tempAudio, audio.length);
    }

    public static void PlayWeaponFire()
    {
        if (Instance != null)
            PlayOneShotWithPitch(Instance.weaponFiringSound, true, CHANNEL_WEAPONS);
    }

    public static void PlayButtonClickedSound()
    {
        if (Instance != null)
            PlayOneShotWithPitch(Instance.buttonClickedSound, true, CHANNEL_UI);
    }

    public static void PlayButtonHoveredSound()
    {
        if (Instance != null)
            PlayOneShotWithPitch(Instance.buttonHoveredSound, true, CHANNEL_UI);
    }

    private static float PercentToDB(float volume)
    {
        return volume > 0.0f ? 20.0f * Mathf.Log10(volume) : -80.0f;
    }

    private static float DBToPercent(float db)
    {
        return db > -80.0f ? Mathf.Pow(10.0f, db / 20.0f) : 0.0f;
    }

    public static void SetChannelVolume(string channelName, float volume)
    {
        if (Instance != null && Instance.audioMixer != null)
        {
            float dbValue = PercentToDB(volume);
            Instance.audioMixer.SetFloat(channelName + "Volume", dbValue);
        }
    }

    public static float GetChannelVolume(string channelName)
    {
        if (Instance != null && Instance.audioMixer != null)
        {
            float dbValue;
            if (Instance.audioMixer.GetFloat(channelName + "Volume", out dbValue))
            {
                return DBToPercent(dbValue);
            }
        }
        return 1.0f;
    }

    public static void SetEngineVolume(float volume)
    {
        SetChannelVolume(CHANNEL_SFX, volume);
    }

    public static void PlayMainMenuMusic()
    {
        if (Instance != null && Instance.mainMenuBGM != null)
        {
            StopMusic();
            Instance.musicSource.clip = Instance.mainMenuBGM;
            Instance.musicSource.Play();
        }
    }

    public static void PlayGameplayMusic()
    {
        if (Instance != null && Instance.gameplayBGM != null)
        {
            StopMusic();
            Instance.musicSource.clip = Instance.gameplayBGM;
            Instance.musicSource.Play();
        }
    }

    public static void StopMusic()
    {
        if (Instance != null && Instance.musicSource != null)
        {
            Instance.musicSource.Stop();
        }
    }
    #endregion
}
