using UnityEngine;

[System.Serializable]
public class SoundData
{
    public SoundsManager.SoundType Type;
    public AudioClip[] Clips;
    [Range(0,1)] public float Volume = 1f;
}