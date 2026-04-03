using System.Collections;
using Cinemachine;
using UnityEngine;

public class PlayerCameraLook : MonoBehaviour
{
    public static PlayerCameraLook Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private CinemachineInputProvider _inputProvider;

    private CinemachinePOV _pov;
    private Coroutine _lookCoroutine;

    private void Awake()
    {
        Instance = this;
        _pov = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
    }

    private void OnEnable()
    {
        InputStateController.Instance.OnStateChanged += OnInputStateChanged;
    }

    private void OnDisable()
    {
        InputStateController.Instance.OnStateChanged -= OnInputStateChanged;
    }

    private void OnInputStateChanged(InputStateController.State state)
    {
        _inputProvider.enabled = state is InputStateController.State.Gameplay;
    }

    public void SetLookEnabled(bool enabled)
    {
        _inputProvider.enabled = enabled;
    }

    public Coroutine SmoothLookAt(Vector3 worldPosition, float duration)
    {
        if (_lookCoroutine != null)
            StopCoroutine(_lookCoroutine);

        _lookCoroutine = StartCoroutine(SmoothLookAtRoutine(worldPosition, duration));
        return _lookCoroutine;
    }

    private IEnumerator SmoothLookAtRoutine(Vector3 worldPosition, float duration)
    {
        var direction = (worldPosition - _virtualCamera.transform.position).normalized;

        var targetYaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        var targetPitch = -Mathf.Asin(direction.y) * Mathf.Rad2Deg;

        var startYaw = _pov.m_HorizontalAxis.Value;
        var startPitch = _pov.m_VerticalAxis.Value;

        // Handle yaw wrapping (pick shortest rotation path)
        var yawDelta = Mathf.DeltaAngle(startYaw, targetYaw);

        var time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            var t = Mathf.SmoothStep(0f, 1f, time / duration);

            _pov.m_HorizontalAxis.Value = startYaw + yawDelta * t;
            _pov.m_VerticalAxis.Value = Mathf.Lerp(startPitch, targetPitch, t);

            yield return null;
        }

        _pov.m_HorizontalAxis.Value = startYaw + yawDelta;
        _pov.m_VerticalAxis.Value = targetPitch;

        _lookCoroutine = null;
    }

    public void LookInDirection(Vector3 worldDirection)
    {
        if (worldDirection.sqrMagnitude < 0.0001f)
            return;

        worldDirection.Normalize();

        var yaw = Mathf.Atan2(worldDirection.x, worldDirection.z) * Mathf.Rad2Deg;
        var pitch = -Mathf.Asin(worldDirection.y) * Mathf.Rad2Deg;
        pitch = NormalizeVerticalAngle(pitch);

        _pov.m_HorizontalAxis.Value = yaw;
        _pov.m_VerticalAxis.Value = pitch;
    }

    private float NormalizeVerticalAngle(float angle)
    {
        if (angle > 180f)
            angle -= 360f;

        return angle;
    }
}
