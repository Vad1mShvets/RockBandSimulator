using UnityEngine;

public class FightActivity : INPCActivity
{
    private Transform _target;
    public bool IsFinished { get; private set; }

    private float _attackTimer;

    public FightActivity(Transform target)
    {
        _target = target;
    }

    public void Start(NPCActor actor) { }

    public void Tick(NPCActor actor, float dt)
    {
        if (Vector3.Distance(actor.transform.position, _target.position) <= 3f)
        {
            _attackTimer -= dt;
            if (_attackTimer <= 0)
            {
                actor.Mover.Stop();
                actor.transform.LookAt(_target);
                actor.PlayAnimation("Punch");
                _attackTimer = 2f;
            }
        }
        else
        {
            _attackTimer = 0;
            actor.Mover.MoveTo(_target.position);
        }
    }

    public void Stop(NPCActor actor) { }
}