public static class ChaosManager
{
    public static int CurrentChaos { get; private set; }
    public static int MaxChaos { get; private set; } = 100;

    public static void Init()
    {
        GameEvents.OnGameplayStarted += Reset;
        
        GameEvents.OnLoopTimingPressed += state =>
        {
            switch (state)
            {
                case ConcertService.TimingState.Perfect:
                    RemoveChaos(20);
                    break;
                case ConcertService.TimingState.Bad:
                    AddChaos(20);
                    break;
            }
        };
    }

    private static void Reset()
    {
        MaxChaos = 100;
        SetChaos(0);
    }
    
    public static void AddChaos(int amount)
    {
        SetChaos(CurrentChaos + amount);
    }
    
    public static void RemoveChaos(int amount)
    {
        SetChaos(CurrentChaos - amount);
    }

    private static void SetChaos(int amount)
    {
        CurrentChaos = amount;
        GameEvents.OnChaosChanged?.Invoke(amount);
        
        if (CurrentChaos >= MaxChaos)
            GameEvents.OnChaosFilled?.Invoke();
    }
}