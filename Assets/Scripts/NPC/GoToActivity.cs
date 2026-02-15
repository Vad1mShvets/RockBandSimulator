using UnityEngine;

public class GoToActivity : INPCActivity
{
    private Transform _target;
    public bool IsFinished { get; private set; }

    public GoToActivity(Transform target)
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
        if (actor.Mover.Arrived)
            IsFinished = true;
    }

    public void Stop(NPCActor actor) { }
}