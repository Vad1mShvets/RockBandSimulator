using UnityEngine;

[CreateAssetMenu(fileName = "AudioConfig", menuName = "Audio/AudioConfig")]
public class AudioConfig : ScriptableObject
{
    public SoundData[] Sounds;
}