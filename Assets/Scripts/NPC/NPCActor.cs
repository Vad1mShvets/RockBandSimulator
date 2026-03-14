using UnityEngine;

[RequireComponent(typeof(NPCNavMeshMover))]
public class NPCActor : MonoBehaviour
{
    private static readonly int InActionBool = Animator.StringToHash("InAction");
    private static readonly int InCombatBool = Animator.StringToHash("InCombat");

    public enum NPCType
    {
        None,
        Evgen,
        Diman
    }
    
    public NPCType NpcType => _npcType;
    public NPCNavMeshMover NavMeshMover => _navMeshMover;
    public NPCActivityRunner ActivityRunner => _activityRunner;
    public Animator Animator => _animator;
    
    [SerializeField] private NPCType _npcType;
    [SerializeField] private NPCNavMeshMover _navMeshMover;
    [SerializeField] private NPCActivityRunner _activityRunner;
    [SerializeField] private NPCRoutine _routine;
    [SerializeField] private Animator _animator;
    [SerializeField] private RagdollController _ragdoll;

    [Header("Concert NPC Only")] [SerializeField] private NPCActivitySpotGroup _concertActivity;
    
    private bool _isConcertNPC => _npcType is NPCType.Evgen or NPCType.Diman;
    
    private void OnEnable()
    {
        GameEvents.OnConcertStarted += OnConcertStarted;
        GameEvents.OnConcertFinished += OnConcertFinished;
    }

    private void OnDisable()
    {
        GameEvents.OnConcertStarted -= OnConcertStarted;
        GameEvents.OnConcertFinished -= OnConcertFinished;
    }

    public void Die()
    {
        _routine.ClearPlan();
        
        _activityRunner.Reset();
        
        _navMeshMover.Stop();
        ReleaseFromSnap();

        _navMeshMover.SetActiveNavMeshAgent(false);
        _navMeshMover.SetActiveAutoRotation(false);

        _animator.enabled = false;

        _ragdoll.EnableRagdoll();
    }
    
    public void SnapTo(Transform point)
    {
        _navMeshMover.SetActiveNavMeshAgent(false);
        transform.SetPositionAndRotation(point.position, point.rotation);
    }
    
    public void ReleaseFromSnap()
    {
        _navMeshMover.SetActiveNavMeshAgent(true);
    }

    public void EnterAnimationAction(string animation)
    {
        _animator.SetBool(InActionBool, true);
        PlayAnimation(animation);
    }

    public void PlayAnimation(string animation)
    {
        _animator.CrossFade(animation, 0.15f);
    }

    public void ExitAnimationAction()
    {
        _animator.SetBool(InActionBool, false);
    }

    public void EnterAnimatorCombatState()
    {
        _animator.SetBool(InCombatBool, true);
    }

    private void OnConcertStarted(ConcertData data)
    {
        if (!_isConcertNPC)
            return;

        var animationName = _npcType switch
        {
            NPCType.Evgen => "Bass",
            NPCType.Diman => "Drums",
            _ => ""
        };

        ActivityRunner.Interrupt(new UseSpotActivity(_concertActivity.GetSpotHard(), float.MaxValue, animationName,
            teleport: true));
    }

    private void OnConcertFinished()
    {
        if (!_isConcertNPC)
            return;
        
        ActivityRunner.Reset();
    }
}