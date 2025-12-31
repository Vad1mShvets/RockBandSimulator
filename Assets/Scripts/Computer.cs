using UnityEngine;

public class Computer : MonoBehaviour, IInteractable
{
    public InteractableTypes Type => InteractableTypes.Computer;
    
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
        GameEvents.OnCallingConcertStart?.Invoke();
    }
}
