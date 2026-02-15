using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCNavMeshMover : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Animator _animator;
    [SerializeField] private string _velocityParam = "Velocity";

    [Header("Tuning")]
    [SerializeField] private float _arriveDistance = 0.15f;

    [Header("Animations")]
    [SerializeField] private float _walkAnimationSpeed = 1.5f;

    private float _animSpeed;

    private Vector3 _target;
    private bool _hasTarget;

    public bool Arrived { get; private set; }

    private void Update()
    {
        UpdateArrived();
        UpdateAnimator();
    }

    public void SetActiveNavMeshAgent(bool value)
    {
        _agent.enabled = value;
    }

    // ========================= MOVEMENT =========================

    public void MoveTo(Vector3 worldPos)
    {
        Arrived = false;
        _hasTarget = true;

        if (NavMesh.SamplePosition(worldPos, out var hit, 2f, NavMesh.AllAreas))
            _target = hit.position;
        else
            _target = worldPos;

        _agent.isStopped = false;
        _agent.SetDestination(_target);
    }

    // ========================= ARRIVAL =========================

    private void UpdateArrived()
    {
        if (!_hasTarget)
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
        if (!_animator)
            return;

        var speed = _agent.velocity.magnitude;
        
        _animator.SetFloat(_velocityParam, speed);
        
        var playback = speed / Mathf.Max(_walkAnimationSpeed, 0.01f);
        
        playback = Mathf.Clamp(playback, 0f, 2.5f);

        _animator.speed = playback;
    }
}