using UnityEngine;

public class GuitarLopata : MonoBehaviour, IInteractable
{
    public InteractableTypes Type => InteractableTypes.GuitarLopata;
    
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
        GameEvents.OnCallingRehearsalStart?.Invoke();
    }
}