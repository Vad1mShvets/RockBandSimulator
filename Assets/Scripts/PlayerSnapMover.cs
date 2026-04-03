using UnityEngine;

public class PlayerSnapMover : MonoBehaviour
{
    [SerializeField] private CharacterController _characterController;

    private void OnEnable()
    {
        TravelSystem.SetPlayerSnapMover(this);
    }

    public void SnapTo(Transform target)
    {
        if (!target) return;
        SnapTo(target.position);
    }

    public void SnapTo(Vector3 position)
    {
        _characterController.enabled = false;
        transform.position = position;
        _characterController.enabled = true;
    }
}