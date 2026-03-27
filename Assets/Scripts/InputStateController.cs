using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputStateController : MonoBehaviour
{
    public static InputStateController Instance { get; private set; }

    public enum State
    {
        Gameplay,
        UI
    }

    public State CurrentState { get; private set; }

    public event Action<State> OnStateChanged;

    [SerializeField] private PlayerInput _playerInput;

    private void Awake()
    {
        Instance = this;
    }

    public void SetGameplay()
    {
        SetState(State.Gameplay);
    }

    public void SetUI()
    {
        SetState(State.UI);
    }

    private void SetState(State state)
    {
        CurrentState = state;

        ApplyState(state);
        OnStateChanged?.Invoke(state);
    }

    private void ApplyState(State state)
    {
        switch (state)
        {
            case State.Gameplay:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                _playerInput.SwitchCurrentActionMap("Gameplay");
                break;

            case State.UI:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _playerInput.SwitchCurrentActionMap("UI");
                break;
        }
    }
}