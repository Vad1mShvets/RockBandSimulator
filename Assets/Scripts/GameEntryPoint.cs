using UnityEngine;
using UnityEngine.InputSystem;

public class GameEntryFlow : MonoBehaviour
{
    [SerializeField] private PlayerSnapMover _playerSnapMover;
    [SerializeField] private PlayerCameraLook _playerCameraLook;
    
    [SerializeField] private Transform _concertStartPoint;
    
    private GuitarType _guitarType;

    private void Start()
    {
        _guitarType = GuitarType.Lopata;
        GameEvents.OnGuitarUpdate?.Invoke(_guitarType);
    }

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

    private void Update()
    {
        if (Keyboard.current.digit7Key.wasPressedThisFrame)
        {
            _guitarType = GuitarType.Lopata;
            GameEvents.OnGuitarUpdate?.Invoke(_guitarType);
        }
        else if (Keyboard.current.digit8Key.wasPressedThisFrame)
        {
            _guitarType = GuitarType.Gvozdi;
            GameEvents.OnGuitarUpdate?.Invoke(_guitarType);
        }
    }
}