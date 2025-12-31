public interface IInteractable
{
    InteractableTypes Type { get; }
    void Focus();
    void UnFocus();
    void Interact();
}