using UnityEngine;

public class RehersalEntryFlow : MonoBehaviour
{
    [SerializeField] private PlayerSnapMover _playerSnapMover;
    [SerializeField] private PlayerCameraLook _playerCameraLook;
    
    [SerializeField] private Transform _rehersalStartPoint;

    private void OnEnable()
    {
        GameEvents.OnCallingRehearsalStart += EnterRehersal;
    }

    private void OnDisable()
    {
        GameEvents.OnCallingRehearsalStart -= EnterRehersal;
    }

    private void EnterRehersal()
    {
        _playerSnapMover.SnapTo(_rehersalStartPoint);
        _playerCameraLook.LookInDirection(_rehersalStartPoint.forward);
    }
}