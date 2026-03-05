using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCNavMeshMover : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private NPCActor _actor;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private string _velocityParam = "Velocity";

    [Header("Tuning")]
    [SerializeField] private float _arriveDistance = 0.15f;

    private Vector3 _target;
    private bool _hasTarget;

    public bool Arrived { get; private set; }

    private void Update()
    {
        if (!_agent.enabled)
            return;
        
        UpdateArrived();
        UpdateAnimator();
    }

    // ========================= MOVEMENT =========================

    public void MoveTo(Vector3 point)
    {
        if (!_agent.enabled)
            return;
        
        _target = point;
        _hasTarget = true;
        Arrived = false;

        _agent.isStopped = false;
        _agent.SetDestination(point);
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

    // ========================= ARRIVAL =========================

    private void UpdateArrived()
    {
        if (!_hasTarget || !_agent.enabled)
            return;

        var pos = transform.position;
        var target = _target;

        pos.y = 0f;
        target.y = 0f;

        var dist = Vector3.Distance(pos, target);

        if (dist <= Mathf.Max(_arriveDistance, _agent.stoppingDistance))
        {
            Arrived = true;
            _hasTarget = false;
            _agent.ResetPath();
        }
    }

    // ========================= ANIMATION =========================

    private void UpdateAnimator()
    {
        _actor.Animator.SetFloat(_velocityParam, _agent.velocity.magnitude);
    }
}