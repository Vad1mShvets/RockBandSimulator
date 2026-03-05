using UnityEngine;

public class RehearsalEntryFlow : MonoBehaviour
{
    [SerializeField] private PlayerSnapMover _playerSnapMover;
    [SerializeField] private PlayerCameraLook _playerCameraLook;
    
    [SerializeField] private Transform _rehearsalStartPoint;

    private void OnEnable()
    {
        GameEvents.OnCallingRehearsalStart += EnterRehearsal;
    }

    private void OnDisable()
    {
        GameEvents.OnCallingRehearsalStart -= EnterRehearsal;
    }

    private void EnterRehearsal()
    {
        _playerSnapMover.SnapTo(_rehearsalStartPoint);
        _playerCameraLook.LookInDirection(_rehearsalStartPoint.forward);
    }
}