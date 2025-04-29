using UnityEngine;

/// <summary>
/// Manages all audio playback in the game using a singleton pattern
/// </summary>
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

        // Initialize audio sources in Awake to ensure they're available immediately
        InitializeAudioSources();
    }
    #endregion

    [Header("Audio Sources")]
    [SerializeField]
    private AudioSource musicSource;

    [SerializeField]
    private AudioSource sfxSource;

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

    private void Start() { }

    /// <summary>
    /// Ensures audio sources are properly initialized
    /// </summary>
    private void InitializeAudioSources()
    {
        // Ensure we have music source
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("MusicSource");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            Debug.Log("Created MusicSource in AudioManager");
        }

        // Ensure we have SFX source
        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFXSource");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            Debug.Log("Created SFXSource in AudioManager");
        }
    }

    #region Public Methods

    public static void PlayOneShot(AudioClip audio)
    {
        if (Instance != null && audio != null)
        {
            Instance.sfxSource.PlayOneShot(audio);
        }
    }

    /// <summary>
    /// Plays a sound with random pitch variation within the defined range
    /// </summary>
    /// <param name="audio">The audio clip to play</param>
    /// <param name="usePitchVariation">Whether to apply random pitch variation</param>
    public static void PlayOneShotWithPitch(AudioClip audio, bool usePitchVariation = true)
    {
        if (Instance != null && audio != null)
        {
            if (usePitchVariation)
            {
                Instance.PlayFromTempAudioSource(audio);
            }
            else
            {
                PlayOneShot(audio);
            }
        }
    }

    /// <summary>
    /// Creates a temporary AudioSource to play a sound with pitch variation
    /// </summary>
    /// <param name="audio">The audio clip to play</param>
    private void PlayFromTempAudioSource(AudioClip audio)
    {
        GameObject tempAudio = new GameObject("TempAudio");
        tempAudio.transform.SetParent(transform);
        AudioSource tempSource = tempAudio.AddComponent<AudioSource>();

        tempSource.clip = audio;
        tempSource.volume = sfxSource.volume;
        tempSource.spatialBlend = sfxSource.spatialBlend;

        tempSource.pitch = Random.Range(1f - pitchFlex, 1f + pitchFlex);

        tempSource.Play();

        Destroy(tempAudio, audio.length);
    }

    public static void PlayWeaponFire()
    {
        if (Instance != null)
            PlayOneShotWithPitch(Instance.weaponFiringSound);
    }

    public static void PlayButtonClickedSound()
    {
        if (Instance != null)
            PlayOneShotWithPitch(Instance.buttonClickedSound);
    }

    public static void PlayButtonHoveredSound()
    {
        if (Instance != null)
            PlayOneShotWithPitch(Instance.buttonHoveredSound);
    }

    public static void SetEngineVolume(float volume)
    {
        if (Instance != null)
        {
            Instance.sfxSource.volume = Mathf.Clamp01(volume);
        }
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
