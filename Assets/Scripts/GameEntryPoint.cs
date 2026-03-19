using UnityEngine;
using UnityEngine.InputSystem;

public class GameEntryFlow : MonoBehaviour
{
    [SerializeField] private PlayerSnapMover _playerSnapMover;
    [SerializeField] private PlayerCameraLook _playerCameraLook;
    [SerializeField] private Transform _concertStartPoint;
    
    private void OnEnable()
    {
        GameEvents.OnGameplayStarted += SetupPlayerPosition;
    }

    private void OnDisable()
    {
        GameEvents.OnGameplayStarted -= SetupPlayerPosition;
    }

    private void SetupPlayerPosition()
    {
        _playerSnapMover.SnapTo(_concertStartPoint);
        _playerCameraLook.LookInDirection(_concertStartPoint.forward);
    }
}