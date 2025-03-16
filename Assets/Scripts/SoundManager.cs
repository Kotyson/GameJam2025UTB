using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    // Audio sources
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    // Object pooler
    [SerializeField] private int audioSourcePoolSize = 10;
    private List<AudioSource> audioSourcePool = new List<AudioSource>();

    public Sound[] sounds;
    private Dictionary<string, Sound> soundDictionary = new Dictionary<string, Sound>();

    private void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeAudioSources();
            InitializeSoundDictionary();
            InitializeAudioPool();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void InitializeAudioSources()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
    }

    private void InitializeAudioPool()
    {
        // Create pool of audio sources, used for getting available audio source
        for (int i = 0; i < audioSourcePoolSize; i++)
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            audioSourcePool.Add(newSource);
        }
    }

    private void InitializeSoundDictionary()
    {
        soundDictionary.Clear();
        foreach (Sound sound in sounds)
        {
            if (!soundDictionary.ContainsKey(sound.name))
            {
                soundDictionary.Add(sound.name, sound);
            }
            else
            {
                Debug.LogWarning($"Entry duplication: {sound.name}");
            }
        }
    }

    // Get available audio source
    private AudioSource GetAvailableAudioSource()
    {
        foreach (AudioSource source in audioSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        // If there is no audio source available, create a new one and put him into the pool of audio sources
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.playOnAwake = false;
        audioSourcePool.Add(newSource);
        return newSource;
    }

    // Play sound at position
    public void PlaySoundAtPosition(string soundName, Vector3 position, float spatialBlend = 1.0f)
    {
        if (!soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            Debug.LogWarning($"Zvuk '{soundName}' nebyl nalezen!");
            return;
        }

        AudioSource source = GetAvailableAudioSource();
        source.clip = sound.clip;
        source.volume = sound.volume;
        source.pitch = sound.pitch;
        source.spatialBlend = spatialBlend; // 0 = 2D, 1 = 3D
        source.transform.position = position;
        source.Play();
    }

    // Play clip sound at position
    public void PlayClipAtPosition(AudioClip clip, Vector3 position, float volume = 1.0f, float spatialBlend = 1.0f)
    {
        if (clip == null) return;

        AudioSource source = GetAvailableAudioSource();
        source.clip = clip;
        source.volume = volume;
        source.transform.position = position;
        source.spatialBlend = spatialBlend; // 0 = 2D, 1 = 3D
        source.Play();
    }

    public AudioSource PlayLoopingClipForDuration(AudioClip clip, float duration, float volume = 1.0f, float spatialBlend = 1.0f)
    {
        if (clip == null) return null;

        AudioSource source = GetAvailableAudioSource();
        source.clip = clip;
        source.volume = volume;
        source.spatialBlend = spatialBlend; // 0 = 2D, 1 = 3D
        source.loop = true;
        source.Play();
        StartCoroutine(StopLoopingClipAfterDuration(source, duration));
        return source;
    }

    private IEnumerator StopLoopingClipAfterDuration(AudioSource source, float duration)
    {
        yield return new WaitForSeconds(duration);
        source.Stop();
        source.loop = false;
    }

    // Playsould
    public void PlaySound(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            sfxSource.pitch = sound.pitch;
            sfxSource.volume = sound.volume;
            sfxSource.PlayOneShot(sound.clip);
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' was not found!");
        }
    }

    // Music player
    public void PlayMusic(string musicName)
    {
        if (soundDictionary.TryGetValue(musicName, out Sound music))
        {
            musicSource.clip = music.clip;
            musicSource.pitch = music.pitch;
            musicSource.volume = music.volume;
            musicSource.loop = true;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"No '{musicName}' was found!");
        }
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
}
