using UnityEngine;

public class FanDude : MonoBehaviour
{
    private static readonly int CrowdAction = Animator.StringToHash("CrowdAction");
    private static readonly int RandomSeed = Animator.StringToHash("RandomSeed");
    private static readonly int DoTransition = Animator.StringToHash("DoTransition");

    [SerializeField] private Animator _animator;
    [SerializeField] private float _seedChangeSpeed = 0.05f;
    [SerializeField] private Vector2 _speedMinMax = new(0.9f, 1.1f);

    private float _currentSeed;
    private float _targetSeed;

    private void Awake()
    {
        _currentSeed = Random.value;
        _targetSeed = _currentSeed;
        _animator.SetFloat(RandomSeed, _currentSeed);
    }

    public void SetAction(CrowdManager.CrowdActionType action)
    {
        _targetSeed = Random.value;

        _animator.SetInteger(CrowdAction, (int)action);
        _animator.SetTrigger(DoTransition);
        _animator.speed = Random.Range(_speedMinMax.x, _speedMinMax.y);
    }

    private void Update()
    {
        SmoothSeedTransition(Time.deltaTime);
    }

    private void SmoothSeedTransition(float dt)
    {
        _currentSeed = Mathf.MoveTowards(_currentSeed, _targetSeed, dt * _seedChangeSpeed);
        _animator.SetFloat(RandomSeed, _currentSeed);
    }
}