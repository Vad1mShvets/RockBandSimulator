using System.Collections.Generic;
using UnityEngine;

public class InventoryBarUI : MonoBehaviour
{
    [SerializeField] private InventoryItemUI _baseItem;
    
    private List<InventoryItemUI> _items = new();

    private void OnEnable()
    {
        GameEvents.OnInventoryUpdate += Setup;
        
        Setup();
    }

    private void OnDisable()
    {
        GameEvents.OnInventoryUpdate -= Setup;
    }

    private void Setup()
    {
        foreach (var item in _items)
            if (item) Destroy(item.gameObject);
        
        _items.Clear();

        foreach (var item in InventoryManager.InventoryItems)
        {
            var newItem = Instantiate(_baseItem, _baseItem.transform.parent);
            newItem.Setup(InteractablesData.Instance.GetInteractableData(item.Key).Icon, item.Value);
            
            _items.Add(newItem);
            newItem.gameObject.SetActive(true);
        }
        
        _baseItem.gameObject.SetActive(false);
    }
}
