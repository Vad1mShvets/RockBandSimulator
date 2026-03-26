public enum PlayerState
{
    Idle,
    Walking,
    Guitar,
    Combat,
    Busy // override (attack/item)
}

public static class PlayerStateController
{
    public static PlayerState CurrentState { get; private set; } = PlayerState.Idle;

    private static PlayerState _previousState;

    public static bool IsBusy => CurrentState == PlayerState.Busy;

    // ================= SET =================

    public static void SetState(PlayerState state)
    {
        if (IsBusy)
            return;

        CurrentState = state;
    }

    // ================= OVERRIDE =================

    public static bool TryEnterBusy()
    {
        if (IsBusy)
            return false;

        _previousState = CurrentState;
        CurrentState = PlayerState.Busy;
        return true;
    }

    public static void ExitBusy()
    {
        CurrentState = _previousState;
    }

    // ================= QUERIES =================

    public static bool CanUseItem()
    {
        return !IsBusy;
    }

    public static bool CanAttack()
    {
        return !IsBusy;
    }
}