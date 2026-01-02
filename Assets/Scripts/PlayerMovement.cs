using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputReader _input;
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _moveSpeed = 6f;
    [SerializeField] private float _gravity = -20f;
    [SerializeField] private float _rotationSpeed = 10f;

    private float _yVelocity;

    private void Update()
    {
        RotateToCamera();
        Move(_input.Move);
        ApplyGravity();
    }

    private void Move(Vector2 move)
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

        _characterController.Move(moveDir * _moveSpeed * Time.deltaTime);
    }

    private void RotateToCamera()
    {
        var forward = _cameraTransform.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 0.001f)
            return;

        var targetRotation = Quaternion.LookRotation(forward);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            _rotationSpeed * Time.deltaTime
        );
    }

    private void ApplyGravity()
    {
        if (_characterController.isGrounded && _yVelocity < 0f)
            _yVelocity = -2f;

        _yVelocity += _gravity * Time.deltaTime;
        _characterController.Move(Vector3.up * _yVelocity * Time.deltaTime);
    }
}