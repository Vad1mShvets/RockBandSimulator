using System;

public static class GameEvents
{
    //Behaviour
    public static Action OnGameStart;
    
    //Concert
    public static Action OnCallingConcertStart;
    public static Action OnCallingRehearsalStart;
    public static Action <ConcertData> OnConcertStarted;
    public static Action OnLoopChooseTimerStart;
    public static Action<float> OnLoopChooseTimerUpdate;
    public static Action OnLoopChooseTimerEnd;
    public static Action<ConcertMusicManager.TimingState> OnLoopTimingPressed;
    
    //Interactable
    public static Action<IInteractable> OnInteractableFocus;
    public static Action OnInteractableUnFocused;

    //Inventory
    public static  Action OnInventoryUpdate;
}
