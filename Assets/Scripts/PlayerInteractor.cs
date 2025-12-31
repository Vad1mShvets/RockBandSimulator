using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _interactDistance = 3f;
    [SerializeField] private LayerMask _interactableMask;
    [SerializeField] private InputReader _inputReader;

    private IInteractable _currentInteractable;

    private void OnEnable()
    {
        _inputReader.Interact += Interact;
    }

    private void OnDisable()
    {
        _inputReader.Interact -= Interact;
    }

    private void Update()
    {
        RaycastCheck();
    }

    private void RaycastCheck()
    {
        var ray = new Ray(_camera.transform.position, _camera.transform.forward);

        Debug.DrawRay(ray.origin, ray.direction * _interactDistance, Color.red);

        if (!Physics.Raycast(ray, out var hit, _interactDistance, _interactableMask))
        {
            ClearFocus();
            return;
        }

        var interactable = hit.collider.GetComponentInParent<IInteractable>();
        if (interactable == null)
        {
            ClearFocus();
            return;
        }

        if (ReferenceEquals(_currentInteractable, interactable))
            return;

        SetFocus(interactable);
    }
    
    private void SetFocus(IInteractable interactable)
    {
        ClearFocus();
        
        _currentInteractable = interactable;
        _currentInteractable.Focus();
        
        GameEvents.OnInteractableFocus?.Invoke(_currentInteractable);
    }

    private void ClearFocus()
    {
        if (_currentInteractable == null) return;
        
        _currentInteractable.UnFocus();
        _currentInteractable = null;
        
        GameEvents.OnInteractableUnFocused?.Invoke();
    }

    private void Interact()
    {
        if (_currentInteractable == null) return;

        _currentInteractable.Interact();
        
        ClearFocus();
    }
}