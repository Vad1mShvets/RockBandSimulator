using TMPro;
using UnityEngine;

public class InteractableHighlightUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;

    private void Awake()
    {
        GameEvents.OnGameplayStarted += Clear;
    }

    private void OnEnable()
    {
        GameEvents.OnInteractableFocus += SetInteractable;
        GameEvents.OnInteractableUnFocused += Clear;
    }

    private void OnDisable()
    {
        GameEvents.OnInteractableFocus -= SetInteractable;
        GameEvents.OnInteractableUnFocused -= Clear;
    }

    private void Start()
    {
        Clear();
    }

    private void SetInteractable(IInteractable interactable)
    {
        var interactableData = InteractableManager.GetInteractableData(interactable.Type);
        
        _nameText.text = interactableData.Name;
        _descriptionText.text = interactableData.Description;
    }

    private void Clear()
    {
        _nameText.text = "";
        _descriptionText.text = "";
    }
}