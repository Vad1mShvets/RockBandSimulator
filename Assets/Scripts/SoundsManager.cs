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

    private static Dictionary<SoundType, SoundData> _soundMap = new();
    private static Dictionary<SoundType, int> _lastClipIndex = new();

    // ───────── INIT ─────────

    public static void Init()
    {
        var config = Resources.Load<AudioConfig>("AudioConfig");

        if (config == null)
        {
            Debug.LogError("[SoundsManager] AudioConfig not found in Resources.");
            return;
        }

        _soundMap.Clear();
        _lastClipIndex.Clear();

        foreach (var sound in config.Sounds)
        {
            if (!_soundMap.TryAdd(sound.Type, sound))
                Debug.LogWarning($"[SoundsManager] Duplicate SoundType '{sound.Type}' in AudioConfig — skipping.");
        }
    }

    // ───────── PUBLIC API ─────────

    public static void PlaySound(SoundType soundType)
    {
        if (!_soundMap.TryGetValue(soundType, out var sound))
        {
            Debug.LogWarning($"[SoundsManager] Sound '{soundType}' not found in AudioConfig.");
            return;
        }

        if (sound.Clips == null || sound.Clips.Length == 0)
        {
            Debug.LogWarning($"[SoundsManager] Sound '{soundType}' has no clips assigned.");
            return;
        }

        PlayClip(sound);
    }

    // ───────── INTERNAL ─────────

    private static void PlayClip(SoundData sound)
    {
        var clip = PickClip(sound);

        var go = new GameObject($"Sound_{sound.Type}");
        var source = go.AddComponent<AudioSource>();

        source.clip = clip;
        source.volume = sound.Volume;
        source.spatialBlend = 0f; // explicitly 2D
        source.playOnAwake = false;
        source.loop = false;

        source.Play();

        Object.Destroy(go, clip.length);
    }

    /// <summary>Picks a random clip, avoiding repeating the same index twice in a row.</summary>
    private static AudioClip PickClip(SoundData sound)
    {
        var clips = sound.Clips;

        if (clips.Length == 1)
            return clips[0];

        _lastClipIndex.TryGetValue(sound.Type, out var lastIndex);

        // Build candidate list excluding the last played index
        var newIndex = Random.Range(0, clips.Length - 1);
        if (newIndex >= lastIndex) newIndex++; // shift past the excluded index

        _lastClipIndex[sound.Type] = newIndex;
        return clips[newIndex];
    }
}