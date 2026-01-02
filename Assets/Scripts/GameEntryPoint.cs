using UnityEngine;

public class GameEntryFlow : MonoBehaviour
{
    [SerializeField] private PlayerSnapMover _playerSnapMover;
    [SerializeField] private PlayerCameraLook _playerCameraLook;
    
    [SerializeField] private Transform _concertStartPoint;

    private void OnEnable()
    {
        GameEvents.OnGameStart += SetupPlayerPosition;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStart -= SetupPlayerPosition;
    }

    private void SetupPlayerPosition()
    {
        _playerSnapMover.SnapTo(_concertStartPoint);
        _playerCameraLook.LookInDirection(_concertStartPoint.forward);
    }
}