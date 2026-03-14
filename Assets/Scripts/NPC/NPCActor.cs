using UnityEngine;

[RequireComponent(typeof(NPCNavMeshMover))]
public class NPCActor : MonoBehaviour
{
    public NPCNavMeshMover NavMeshMover => _navMeshMover;
    public NPCActivityRunner ActivityRunner => _activityRunner;
    public Animator Animator => _animator;
    
    [SerializeField] private NPCNavMeshMover _navMeshMover;
    [SerializeField] private NPCActivityRunner _activityRunner;
    [SerializeField] private NPCRoutine _routine;
    [SerializeField] private Animator _animator;
    [SerializeField] private RagdollController _ragdoll;
    
    private static readonly int InActionBool = Animator.StringToHash("InAction");
    private static readonly int InCombatBool = Animator.StringToHash("InCombat");

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
}