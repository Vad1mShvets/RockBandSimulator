using System;

public static class GameEvents
{
    //Behaviour
    public static Action OnGameplayStarted;
    
    //Concert
    public static Action OnCallingConcertStart;
    public static Action OnCallingRehearsalStart;
    public static Action <ConcertData> OnConcertStarted;
    
    public static Action OnLoopChooseTimerStart;
    public static Action<float> OnLoopChooseTimerUpdate;
    public static Action OnLoopChooseTimerEnd;
    public static Action<LoopType> OnNewLoopStart;
    
    public static Action<ConcertService.TimingState> OnLoopTimingPressed;
    
    public static Action OnConcertFinished;
    
    public static Action<LoopType> OnMidLoopTimingStarted;
    public static Action<float> OnMidLoopTimingUpdate;
    public static Action OnMidLoopTimingEnd;
    
    public static Action<NPCActor.NPCType> OnInstrumentStarted;
    public static Action<NPCActor.NPCType> OnInstrumentStopped;

    public static Action OnConcertScoreUpdated;

    public static Action<bool> OnLastNoteBonusPressed;

    public static Action OnConcertFinishScreenClosed;
    
    //Reputation
    public static Action OnReputationUpdated;
    public static Action OnReputationFilled;
    
    //Interactable
    public static Action<IInteractable> OnInteractableFocus;
    public static Action OnInteractableUnFocused;

    //Inventory
    public static Action OnInventoryUpdate;
    public static Action<InteractableTypes> OnInventoryItemUsed;
    
    //Player
    public static Action OnWalkingStart;
    public static Action OnWalkingEnd;
    
    //Guitars
    public static Action<GuitarType> OnGuitarUpdate;
    
    //Combat
    public static Action OnCombatStart;
    public static Action OnCombatEnd;
    public static Action OnAttack;
    
    //Chaos
    public static Action<float> OnChaosChanged;
    public static Action OnChaosFilled;
}
