using UnityEngine;

[CreateAssetMenu(fileName = "InteractableData", menuName = "Interactables/InteractableData")]
public class InteractableData : ScriptableObject
{
    public InteractableTypes Type;
    public string Name;
    public string Description;
    public Sprite Icon;
}
