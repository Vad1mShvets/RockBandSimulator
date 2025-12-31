using UnityEngine;

[CreateAssetMenu(fileName = "ConcertAudioData", menuName = "Concert/ConcertAudioData")]
public class ConcertAudioData : ScriptableObject
{
    [Header("Loops")]
    public AudioClip ELoop;
    public AudioClip ALoop;
    public AudioClip BLoop;
    public AudioClip CLoop;
    public AudioClip DLoop;
    
    [Header("Miss Sounds")]
    public AudioClip[] MissSounds;
}
