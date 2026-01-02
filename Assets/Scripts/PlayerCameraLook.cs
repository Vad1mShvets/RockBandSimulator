using Cinemachine;
using UnityEngine;

public class PlayerCameraLook : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;

    private CinemachinePOV _pov;

    private void Awake()
    {
        _pov = _virtualCamera.GetCinemachineComponent<CinemachinePOV>();
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