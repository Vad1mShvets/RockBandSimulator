using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private InputReader _input;
    [SerializeField] private float _attackCooldown = 0.6f;
    [SerializeField] private float _exitCombatCooldown = 5f;

    private float _cooldown;
    private bool _inCombat;

    private void OnEnable()
    {
        _input.Fire += OnAttack;
    }

    private void OnDisable()
    {
        _input.Fire -= OnAttack;
    }

    private void Update()
    {
        if (_cooldown > 0f)
            _cooldown -= Time.deltaTime;
    }

    private void OnAttack()
    {
        if (_cooldown > 0f)
            return;

        EnterCombatMode();

        _cooldown = _attackCooldown;

        GameEvents.OnAttack?.Invoke();
    }

    private void EnterCombatMode()
    {
        if (_inCombat)
            return;

        _inCombat = true;
        GameEvents.OnCombatStart?.Invoke();
        
        Invoke(nameof(ExitCombat),  _exitCombatCooldown);
    }

    public void ExitCombat()
    {
        if (!_inCombat)
            return;

        _inCombat = false;
        GameEvents.OnCombatEnd?.Invoke();
    }
}