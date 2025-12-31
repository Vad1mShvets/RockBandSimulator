using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputReader _input;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _cameraTransform;

    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _externalForceDamping = 10f;

    private Vector3 _externalVelocity;

    private void FixedUpdate()
    {
        HandleMovement(_input.Move);
        ApplyExternalForces();
    }

    private void HandleMovement(Vector2 move)
    {
        var camForward = _cameraTransform.forward;
        var camRight = _cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        var moveDir =
            camForward * move.y +
            camRight * move.x;

        var currentVelocity = _rigidbody.velocity;
        
        var playerVelocity = moveDir * _moveSpeed;

        _rigidbody.velocity = new Vector3(
            playerVelocity.x + _externalVelocity.x,
            currentVelocity.y,
            playerVelocity.z + _externalVelocity.z
        );
    }

    private void ApplyExternalForces()
    {
        _externalVelocity = Vector3.Lerp(
            _externalVelocity,
            Vector3.zero,
            _externalForceDamping * Time.fixedDeltaTime
        );
    }
    
    public void AddImpulse(Vector3 force)
    {
        _externalVelocity += force;
    }
}