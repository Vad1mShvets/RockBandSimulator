using System.Collections.Generic;

public static class InventoryManager
{
    public static IReadOnlyDictionary<InteractableTypes, int> InventoryItems => _inventoryItems;
    
    private static Dictionary<InteractableTypes, int> _inventoryItems = new();
    
    public static void AddItem(InteractableTypes pickUpItem)
    {
        if (!_inventoryItems.TryAdd(pickUpItem, 1))
            _inventoryItems[pickUpItem]++;
        
        TryUseItem(pickUpItem);
        
        GameEvents.OnInventoryUpdate?.Invoke();
    }

    public static bool TryUseItem(InteractableTypes pickUpItem)
    {
        if (!_inventoryItems.ContainsKey(pickUpItem) || _inventoryItems[pickUpItem] <= 0) return false;
        
        _inventoryItems[pickUpItem]--;
        
        ConvertToChaos(pickUpItem);
        
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
}