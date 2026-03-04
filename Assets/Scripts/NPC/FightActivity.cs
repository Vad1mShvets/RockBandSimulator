using UnityEngine;

public class FightActivity : INPCActivity
{
    private Transform _target;
    public bool IsFinished { get; private set; }

    public FightActivity(Transform target)
    {
        _target = target;
    }

    public void Start(NPCActor actor)
    {
        IsFinished = false;
        actor.Mover.MoveTo(_target.position);
    }

    public void Tick(NPCActor actor, float dt)
    {
        actor.Mover.MoveTo(_target.position);
    }

    public void Stop(NPCActor actor) { }
}