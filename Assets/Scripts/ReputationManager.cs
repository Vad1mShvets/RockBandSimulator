using UnityEngine;

public static class ReputationManager
{
    public static int CurrentReputation { get; private set; }
    public static int MaxReputation { get; private set; } = 1000;

    public static void Init()
    {
        ResetReputation();
        
        GameEvents.OnConcertFinished += () => AddReputation(ConcertScoreManager.OverallScore);
    }

    private static void ResetReputation()
    {
        CurrentReputation = 0;
        MaxReputation = 1000;
    }

    private static void AddReputation(int reputation)
    {
        SetReputation(CurrentReputation + reputation);
    }

    private static void SetReputation(int reputation)
    {
        CurrentReputation = Mathf.Clamp(reputation, 0, MaxReputation);
        GameEvents.OnReputationUpdated?.Invoke();
        
        if (CurrentReputation == MaxReputation)
            GameEvents.OnReputationFilled?.Invoke();
    }
}