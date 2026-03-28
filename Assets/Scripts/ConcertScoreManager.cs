public static class ConcertScoreManager
{
    public static int OverallScore => PositiveScore - NegativeScore;
    public static int PositiveScore { get; private set; }
    public static int NegativeScore { get; private set; }

    public static void Init()
    {
        GameEvents.OnLoopTimingPressed += OnTimingPressed;
        GameEvents.OnConcertStarted += _ => ResetScore();
        
        ResetScore();
    }

    private static void ResetScore()
    {
        PositiveScore = 0;
        NegativeScore = 0;
        
        GameEvents.OnConcertScoreUpdated?.Invoke();
    }
    
    private static void AddScore(int score)
    {
        PositiveScore += score;
        
        GameEvents.OnConcertScoreUpdated?.Invoke();
    }
    
    private static void RemoveScore(int score)
    {
        NegativeScore += score;
        
        GameEvents.OnConcertScoreUpdated?.Invoke();
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