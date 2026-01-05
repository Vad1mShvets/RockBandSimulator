using UnityEngine;

[CreateAssetMenu(fileName = "ConcertPackAudioData", menuName = "Concert/ConcertPackAudioData")]
public class ConcertPackAudioData : ScriptableObject
{
    public GuitarType GuitarType;

    public ConcertAudioData GuitarData;
    public ConcertAudioData DrumsData;
    public ConcertAudioData BassData;

    public AudioClip GetGuitar(LoopType loop) => GetClip(GuitarData, loop);
    public AudioClip GetDrums(LoopType loop)  => GetClip(DrumsData, loop);
    public AudioClip GetBass(LoopType loop)   => GetClip(BassData, loop);

    private AudioClip GetClip(ConcertAudioData data, LoopType loop)
    {
        return loop switch
        {
            LoopType.A => data.ALoop,
            LoopType.B => data.BLoop,
            LoopType.C => data.CLoop,
            LoopType.D => data.DLoop,
            LoopType.E => data.ELoop,
            _ => null
        };
    }
}