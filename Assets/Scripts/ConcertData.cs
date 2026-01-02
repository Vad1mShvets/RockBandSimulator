using System;

[Serializable]
public struct ConcertData
{
    public float ChooseWindowSeconds;
    public float PerfectTimingSeconds;

    public ConcertData(float chooseWindowSeconds, float perfectTimingSeconds)
    {
        ChooseWindowSeconds = chooseWindowSeconds;
        PerfectTimingSeconds = perfectTimingSeconds;
    }
}