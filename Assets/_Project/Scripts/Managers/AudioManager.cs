using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioMixer mainMixer;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private AudioMixerGroup ambienceGroup;
    [SerializeField] private AudioMixerGroup ambienceLoudGroup;


    private float masterVolume = 1f;

    private void Start()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlaySFX(AudioClip clip,float pitchRandomness=0.1f)
    {
        if (clip == null) return;
        float randomPitch = 1f + Random.Range(-pitchRandomness, pitchRandomness);
        sfxSource.pitch = randomPitch;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null) return;

        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void SetVolume(float volume)
    {
        masterVolume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();

        float dB = volume > 0 ? Mathf.Log10(volume) * 20 : -80f;
        if (mainMixer != null)
        {
            mainMixer.SetFloat("MasterVol", dB);
        }
    }
    public float GetVolume()
    {
        return masterVolume;
    }
    public AudioMixerGroup GetAmbienceGroup()
    {
        return ambienceGroup;
    }
    public AudioMixerGroup GetAmbienceLoudGroup()
    {
        return ambienceLoudGroup;
    }

    private void LoadVolume()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        SetVolume(masterVolume);
    }
}
