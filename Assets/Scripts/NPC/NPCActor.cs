using UnityEngine;

[RequireComponent(typeof(NPCNavMeshMover))]
public class NPCActor : MonoBehaviour
{
    public NPCNavMeshMover NavMeshMover => _navMeshMover;
    public NPCActivityRunner ActivityRunner => _activityRunner;
    public Animator Animator => _animator;
    
    [SerializeField] private NPCNavMeshMover _navMeshMover;
    [SerializeField] private NPCActivityRunner _activityRunner;
    [SerializeField] private Animator _animator;
    [SerializeField] private RagdollController _ragdoll;
    
    private static readonly int InAction = Animator.StringToHash("InAction");
    private static readonly int InCombat = Animator.StringToHash("InCombat");

    public void Die()
    {
        _activityRunner.Reset();

        _navMeshMover.Stop();

        _animator.enabled = false;
        _navMeshMover.SetActiveNavMeshAgent(false);
        _navMeshMover.SetActiveAutoRotation(false);

        _ragdoll.EnableRagdoll();
    }
    public void SnapTo(Transform point)
    {
        _navMeshMover.SetActiveNavMeshAgent(false);
        _navMeshMover.enabled = false;
        transform.SetPositionAndRotation(point.position, point.rotation);
    }
    
    public void ReleaseFromSnap()
    {
        _navMeshMover.SetActiveNavMeshAgent(true);
        _navMeshMover.enabled = true;
    }

    public void PlayAnimation(string animation)
    {
        _animator.CrossFade(animation, 0.15f);
    }

    public void EnterAnimationAction(string animation)
    {
        _animator.SetBool(InAction, true);
        _animator.CrossFade(animation, 0.15f);
    }

    public void ExitAnimationAction()
    {
        _animator.SetBool(InAction, false);
    }

    public void EnterAnimatorCombatState()
    {
        _animator.SetBool(InCombat, true);
    }
}