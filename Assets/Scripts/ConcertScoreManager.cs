public static class ConcertScoreManager
{
    public static int OverallScore => PositiveScore + NegativeScore;
    public static int PositiveScore { get; private set; }
    public static int NegativeScore { get; private set; }

    static ConcertScoreManager()
    {
        GameEvents.OnLoopTimingPressed += OnTimingPressed;
    }

    public static void ResetScore()
    {
        PositiveScore = 0;
        NegativeScore = 0;
    }
    
    private static void AddScore(int score)
    {
        PositiveScore += score;
    }
    
    private static void RemoveScore(int score)
    {
        NegativeScore += score;
    }
    
    private static void OnTimingPressed(ConcertService.TimingState timing)
    {
        switch (timing)
        {
            case ConcertService.TimingState.Perfect:
                AddScore(150);
                break;
            case ConcertService.TimingState.Bad:
                RemoveScore(100);
                break;
        }
    }
}