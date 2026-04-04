using UnityEngine;

public class NPCCombat : MonoBehaviour, IDamageable
{
    private const string HIT_ANIMATION_PREFIX = "Hit";
    private const int  HIT_ANIMATIONS_COUNT = 2;
    
    [SerializeField] private NPCActor _actor;

    private Transform _target;

    private int _hitsToDie = 7;

    public void TakeDamage(DamageData damage)
    {
        _target = damage.Attacker;
        _actor.ActivityRunner.Run(new FightActivity(_target));
        _actor.PlayAnimation(HIT_ANIMATION_PREFIX + Random.Range(0, HIT_ANIMATIONS_COUNT));

        GameEvents.OnNPCDamaged?.Invoke(_actor);

        _hitsToDie--;
        if (_hitsToDie <= 0)
            _actor.Die();

        Debug.Log($"{gameObject.name} got hit {_hitsToDie}");
    }
}