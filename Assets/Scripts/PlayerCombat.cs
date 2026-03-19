using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Vector3 OverlapPosition => transform.position + _camera.transform.forward + _radiusOffset;

    [SerializeField] private Camera _camera;
    [SerializeField] private float _attackCooldown = 0.6f;
    [SerializeField] private float _exitCombatCooldown = 5f;

    [Space]
    [SerializeField] private float _radius = 1;
    [SerializeField] private Vector3 _radiusOffset;
    
    private readonly HashSet<IDamageable> _hitTargets = new();

    private float _cooldown;
    private bool _inCombat;

    private void OnEnable()
    {
        InputReader.Instance.Fire += OnAttack;
    }

    private void OnDisable()
    {
        InputReader.Instance.Fire -= OnAttack;
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

        _hitTargets.Clear();

        var collidersInRange = Physics.OverlapSphere(OverlapPosition, _radius);
        
        SoundsManager.PlaySound(SoundsManager.SoundType.StrikeBreathe);

        foreach (var collider in collidersInRange)
        {
            var damageable = collider.GetComponentInParent<IDamageable>();
            if (damageable == null)
                continue;

            if (_hitTargets.Add(damageable))
            {
                StartCoroutine(DelayedDamage(damageable));
            }
        }
        
        GameEvents.OnAttack?.Invoke();
    }

    private IEnumerator DelayedDamage(IDamageable damageable)
    {
        yield return new WaitForSeconds(0.15f);
        
        ChaosManager.AddChaos(100);
        
        damageable.TakeDamage(new DamageData(25, transform));
        SoundsManager.PlaySound(SoundsManager.SoundType.StrikeHit);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(OverlapPosition, _radius);
    }
}