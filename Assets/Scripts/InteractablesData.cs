using UnityEngine;

public static class InteractableManager
{
    private const string INTERACTABLES_PATH = "Interactables";
    
    private static InteractableData[] _interactablesData;
    
    private static bool _inited;

    public static InteractableData GetInteractableData(InteractableTypes interactableType)
    {
        if (!_inited)
        {
            _interactablesData = Resources.LoadAll<InteractableData>(INTERACTABLES_PATH);
            _inited = true;
        }
        
        for (int i = 0; i < _interactablesData.Length; i++)
        {
            if (!_interactablesData[i]) continue;

            if (_interactablesData[i].Type == interactableType)
                return _interactablesData[i];
        }
        
        return null;
    }
}