using UnityEngine;

public static class MoneyManager
{
    public static int CurrentMoney { get; private set; }

    public static void Init(int startMoney = 100)
    {
        CurrentMoney = startMoney;
        GameEvents.OnMoneyChanged?.Invoke(CurrentMoney);
    }

    public static void AddMoney(int amount)
    {
        SetMoney(CurrentMoney + amount);
    }

    public static bool TrySpendMoney(int amount)
    {
        if (CurrentMoney < amount) return false;
        SetMoney(CurrentMoney - amount);
        return true;
    }

    private static void SetMoney(int amount)
    {
        CurrentMoney = Mathf.Max(0, amount);
        GameEvents.OnMoneyChanged?.Invoke(CurrentMoney);
    }
}
