using UnityEngine;

public class ConcertEntryFlow : MonoBehaviour
{
    [SerializeField] private PlayerSnapMover _playerSnapMover;
    [SerializeField] private PlayerCameraLook _playerCameraLook;
    
    [SerializeField] private Transform _concertStartPoint;

    private void OnEnable()
    {
        GameEvents.OnCallingConcertStart += EnterConcert;
    }

    private void OnDisable()
    {
        GameEvents.OnCallingConcertStart -= EnterConcert;
    }

    private void EnterConcert()
    {
        _playerSnapMover.SnapTo(_concertStartPoint);
        _playerCameraLook.LookInDirection(_concertStartPoint.forward);
    }
}