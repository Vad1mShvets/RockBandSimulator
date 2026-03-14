using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCNavMeshMover : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private NPCActor _actor;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private string _velocityParam = "Velocity";

    private bool _hasTarget;
    private int _velocityHash;

    public bool Arrived { get; private set; }

    private void Awake()
    {
        _velocityHash = Animator.StringToHash(_velocityParam);
    }

    private void Update()
    {
        if (!_agent.enabled || !_agent.isOnNavMesh)
            return;

        UpdateArrived();
        UpdateAnimator();
    }

    public void MoveTo(Vector3 point)
    {
        if (!_agent.enabled)
            return;

        if (!_agent.SetDestination(point))
            return;

        Arrived = false;
        _hasTarget = true;

        _agent.isStopped = false;
    }

    public void Stop()
    {
        if (!_agent.enabled)
            return;

        _agent.isStopped = true;
        _agent.ResetPath();

        _hasTarget = false;
    }

    public void SetActiveNavMeshAgent(bool active)
    {
        if (_agent.enabled == active)
            return;

        if (!active)
            _agent.ResetPath();

        _agent.enabled = active;
    }

    public void SetActiveAutoRotation(bool value)
    {
        _agent.updateRotation = value;
    }

    private void UpdateArrived()
    {
        if (!_hasTarget)
            return;

        if (_agent.pathPending)
            return;

        if (_agent.pathStatus == NavMeshPathStatus.PathInvalid)
        {
            Stop();
            return;
        }

        if (_agent.remainingDistance > _agent.stoppingDistance)
            return;

        if (_agent.velocity.sqrMagnitude > 0.01f)
            return;

        Arrived = true;
        _hasTarget = false;

        _agent.isStopped = true;
        _agent.ResetPath();
    }

    private void UpdateAnimator()
    {
        _actor.Animator.SetFloat(_velocityHash, _agent.velocity.magnitude);
    }
}