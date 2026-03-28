public class ReputationBarUI : AnimatedBarBaseUI
{
    private void OnEnable()
    {
        SetRange(0f, ReputationManager.MaxReputation);
        
        SnapToValue(0f);
        
        GameEvents.OnReputationUpdated += UpdateReputation;
        
        UpdateReputation();
    }
    
    private void OnDisable()
    {
        GameEvents.OnReputationUpdated -= UpdateReputation;
    }
    
    private void UpdateReputation()
    {
        SetTargetValueInternal(ReputationManager.CurrentReputation);
    }
}