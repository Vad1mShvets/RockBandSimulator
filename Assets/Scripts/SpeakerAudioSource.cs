using UnityEngine;

public class SpeakerAudioSource : MonoBehaviour
{
    [SerializeField] private AudioSource _guitar;
    [SerializeField] private AudioSource _guitarMiss;
    [SerializeField] private AudioSource _drums;
    [SerializeField] private AudioSource _bass;

    public void Play(AudioClip guitar, AudioClip drums, AudioClip bass, double dspTime)
    {
        PlayOne(_guitar, guitar, dspTime);
        PlayOne(_drums, drums, dspTime);
        PlayOne(_bass, bass, dspTime);
    }

    public void PlayGuitarMiss(AudioClip missSound)
    {
        if (!_guitarMiss || !missSound) return;

        _guitarMiss.clip = missSound;
        _guitarMiss.Play();
    }

    public void StopAll()
    {
        if (_guitar) _guitar.Stop();
        if (_drums) _drums.Stop();
        if (_bass) _bass.Stop();
    }

    private static void PlayOne(AudioSource source, AudioClip clip, double dspTime)
    {
        if (!source) return;

        if (!clip)
        {
            source.Stop();
            source.clip = null;
            return;
        }

        source.clip = clip;
        source.PlayScheduled(dspTime);
    }
}