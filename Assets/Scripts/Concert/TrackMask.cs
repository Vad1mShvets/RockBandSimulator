using System;

[Serializable]
public struct TrackMask
{
    public bool Guitar;
    public bool Drums;
    public bool Bass;

    public TrackMask(bool guitar, bool drums, bool bass)
    {
        Guitar = guitar;
        Drums = drums;
        Bass = bass;
    }
}