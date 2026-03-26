using System.Collections.Generic;

public static class InventoryManager
{
    public static IReadOnlyDictionary<InteractableTypes, int> InventoryItems => _inventoryItems;
    
    private static Dictionary<InteractableTypes, int> _inventoryItems = new();

    public static bool TryUseItem(InteractableTypes pickUpItem)
    {
        if (!PlayerStateController.CanUseItem()) return false;
        
        ConvertToChaos(pickUpItem);
        PlaySound(pickUpItem);
        
        GameEvents.OnInventoryItemUsed?.Invoke(pickUpItem);
        GameEvents.OnInventoryUpdate?.Invoke();
        
        return true;
    }

    private static void ConvertToChaos(InteractableTypes item)
    {
        switch (item)
        {
            case InteractableTypes.Beer:
            case InteractableTypes.Cigs:
                ChaosManager.AddChaos(20);
                break;
        }
    }

    private static void PlaySound(InteractableTypes item)
    {
        switch (item)
        {
            case InteractableTypes.Cigs:
                SoundsManager.PlaySound(SoundsManager.SoundType.SmokeCig);
                break;
            case InteractableTypes.Beer:
                SoundsManager.PlaySound(SoundsManager.SoundType.DrinkBeer);
                break;
        }
    }
}