using UnityEngine;

public static class SoundsManager
{
    public enum SoundType
    {
        StrikeHit,
        StrikeMiss,
        StrikeBreathe,
    }

    private static AudioConfig _audioConfig;

    static SoundsManager()
    {
        _audioConfig = Resources.Load<AudioConfig>("AudioConfig");
    }

    public static void PlaySound(SoundType soundType)
    {
        if (_audioConfig == null)
        {
            Debug.LogError("AudioConfig not found in Resources!");
            return;
        }

        foreach (var sound in _audioConfig.Sounds)
        {
            if (sound.Type == soundType)
            {
                PlayClip(sound);
                return;
            }
        }

        Debug.LogWarning($"Sound {soundType} not found in AudioConfig");
    }

    private static void PlayClip(SoundData sound)
    {
        var soundObject = new GameObject("Sound_" + sound.Type);
        var clip = sound.Clips[Random.Range(0, sound.Clips.Length)];

        var source = soundObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = sound.Volume;

        source.Play();

        Object.Destroy(soundObject, clip.length);
    }
}