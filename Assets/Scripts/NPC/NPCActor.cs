using UnityEngine;

[RequireComponent(typeof(NPCNavMeshMover))]
public class NPCActor : MonoBehaviour
{
    public NPCNavMeshMover Mover { get; private set; }
    public Animator Animator { get; private set; }
    
    private static readonly int InAction = Animator.StringToHash("InAction");

    private void Awake()
    {
        Mover = GetComponent<NPCNavMeshMover>();
        Animator = GetComponentInChildren<Animator>();
    }

    public void SnapTo(Transform point)
    {
        Mover.SetActiveNavMeshAgent(false);
        Mover.enabled = false;
        transform.SetPositionAndRotation(point.position, point.rotation);
    }
    
    public void ReleaseFromSnap()
    {
        Mover.SetActiveNavMeshAgent(true);
        Mover.enabled = true;
    }

    public void EnterAnimationAction(string animation)
    {
        Animator.SetBool(InAction, true);
        Animator.CrossFade(animation, 0.15f);
    }

    public void ExitAnimationAction()
    {
        Animator.SetBool(InAction, false);
    }
}