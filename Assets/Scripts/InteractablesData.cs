using UnityEngine;

public class InteractablesData : MonoBehaviour
{
    public static InteractablesData Instance;
    
    [SerializeField] private InteractableData[] _interactablesData;

    private void Awake()
    {
        Instance = this;
    }

    public InteractableData GetInteractableData(InteractableTypes interactableType)
    {
        for (int i = 0; i < _interactablesData.Length; i++)
        {
            if (!_interactablesData[i]) continue;

            if (_interactablesData[i].Type == interactableType)
                return _interactablesData[i];
        }
        
        return null;
    }
}