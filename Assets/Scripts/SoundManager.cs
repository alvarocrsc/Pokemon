using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource effectsSource, musicSource;

    public Vector2 pitchRange = Vector2.zero;

    public static SoundManager SharedInstance;

    private void Awake()
    {
        if (SharedInstance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            SharedInstance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(AudioClip clip)
    {
        effectsSource.Stop();
        effectsSource.clip = clip;
        effectsSource.Play();
    }
    
    public void PlayMusic(AudioClip clip)
    {
        effectsSource.Stop();
        effectsSource.clip = clip;
        effectsSource.Play();
    }

    public void RandomSoundEffect(params AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;

        int index = Random.Range(0, clips.Length);
        float pitch = Random.Range(pitchRange.x, pitchRange.y);

        effectsSource.pitch = pitch;
        PlaySound(clips[index]);
    }
}
