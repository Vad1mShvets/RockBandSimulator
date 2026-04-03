using UnityEngine;

public class CarInteractable : MonoBehaviour, IInteractable
{
    public InteractableTypes Type => InteractableTypes.Car;

    public void Focus() { }

    public void UnFocus() { }

    public void Interact()
    {
        MapUI.Instance.Show();
    }
}
