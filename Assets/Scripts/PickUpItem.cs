using UnityEngine;

public class PickUpItem : MonoBehaviour, IInteractable
{
    public InteractableTypes Type => _interactableType;
    
    [SerializeField] private InteractableTypes _interactableType;
    
    public void Focus()
    {
        //
    }

    public void UnFocus()
    {
        //
    }

    public void Interact()
    {
        if (InventoryManager.TryUseItem(_interactableType))
            Destroy(gameObject);
    }
}
