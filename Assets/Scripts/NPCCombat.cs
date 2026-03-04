using UnityEngine;

public class NPCCombat : MonoBehaviour, IDamageable
{
    [SerializeField] private NPCActivityRunner _runner;
    [SerializeField] private NPCActor _actor;

    private Transform _target;

    public void TakeDamage(DamageData damage)
    {
        _target = damage.Attacker;
        _runner.Run(new FightActivity(_target));
        _actor.PlayAnimation("Hit");
    }
}