using System;

[Serializable]
public struct TrackMask
{
    public bool Guitar;
    public bool Drums;
    public bool Bass;

    public static TrackMask All =>
        new TrackMask { Guitar = true, Drums = true, Bass = true };

    public static TrackMask NoBass =>
        new TrackMask { Guitar = true, Drums = true, Bass = false };
}