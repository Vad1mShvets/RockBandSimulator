using System.Collections.Generic;
using UnityEngine;

public static class SoundsManager
{
    public enum SoundType
    {
        StrikeHit,
        StrikeBreathe,
        CrowdCheering,
        CrowdBooing,
        DrinkBeer,
        SmokeCig,
    }

    private static AudioConfig _audioConfig;
    
    private static Dictionary<SoundType, int> _lastPlayedClipIndex = new();

    public static void Init()
    {
        _audioConfig = Resources.Load<AudioConfig>("AudioConfig");
    }

    public static void PlaySound(SoundType soundType)
    {
        foreach (var sound in _audioConfig.Sounds)
        {
            if (sound.Type != soundType)
                continue;

            PlayClip(sound);
            return;
        }

        Debug.LogWarning($"Sound {soundType} not found in AudioConfig");
    }

    private static void PlayClip(SoundData sound)
    {
        var soundObject = new GameObject("Sound_" + sound.Type);

        int lastIndex = -1;
        _lastPlayedClipIndex.TryGetValue(sound.Type, out lastIndex);

        int newIndex;

        if (sound.Clips.Length == 1)
        {
            newIndex = 0;
        }
        else
        {
            do
            {
                newIndex = Random.Range(0, sound.Clips.Length);
            }
            while (newIndex == lastIndex);
        }

        _lastPlayedClipIndex[sound.Type] = newIndex;

        var clip = sound.Clips[newIndex];

        var source = soundObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = sound.Volume;

        source.Play();

        Object.Destroy(soundObject, clip.length);
    }
}