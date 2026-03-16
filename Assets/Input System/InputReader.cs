using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputReader : MonoBehaviour
{
    public static InputReader Instance { get; private set; }
    
    public Vector2 Move { get; private set; }
    public Vector2 Look { get; private set; }
    
    public event Action Fire;
    public event Action Interact;

    public event Action ALoop;
    public event Action BLoop;
    public event Action CLoop;
    public event Action DLoop;

    private void Awake()
    {
        Instance = this;
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        Look = context.ReadValue<Vector2>();
    }
    
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
            Fire?.Invoke();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            Interact?.Invoke();
    }

    public void OnALoop(InputAction.CallbackContext context)
    {
        if (context.performed)
            ALoop?.Invoke();
    }

    public void OnBLoop(InputAction.CallbackContext context)
    {
        if (context.performed)
            BLoop?.Invoke();
    }
    
    public void OnCLoop(InputAction.CallbackContext context)
    {
        if (context.performed)
            CLoop?.Invoke();
    }

    public void OnDLoop(InputAction.CallbackContext context)
    {
        if (context.performed)
            DLoop?.Invoke();
    }
    
    private void OnDisable()
    {
        Move = Vector2.zero;
        Look = Vector2.zero;
    }
}