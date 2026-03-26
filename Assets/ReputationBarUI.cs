public class ReputationBarUI : AnimatedBarBaseUI
{
    private void OnEnable()
    {
        SetRange(0f, ChaosManager.MaxChaos);
        SnapToValue(ChaosManager.CurrentChaos);

        //GameEvents.OnReputationChanged += SetValue;
    }

    private void OnDisable()
    {
        //GameEvents.OnReputationChanged -= SetValue;
    }

    private void SetValue(float value)
    {
        SetTargetValueInternal(value);
    }
}