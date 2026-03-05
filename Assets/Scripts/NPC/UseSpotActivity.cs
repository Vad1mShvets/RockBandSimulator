using UnityEngine;

public class UseSpotActivity : INPCActivity
{
    private NPCActivitySpot _spot;
    private float _duration;
    private float _timer;
    private bool _snapped;

    public bool IsFinished { get; private set; }

    public UseSpotActivity(NPCActivitySpot spot, float duration)
    {
        _spot = spot;
        _duration = duration;
    }

    public void Start(NPCActor actor)
    {
        IsFinished = false;
        actor.NavMeshMover.MoveTo(_spot.transform.position);
    }

    public void Tick(NPCActor actor, float dt)
    {
        if (!_snapped)
        {
            if (!actor.NavMeshMover.Arrived)
                return;
            
            actor.SnapTo(_spot.transform);
            actor.EnterAnimationAction(_spot.name);

            _snapped = true;
            _timer = _duration;
            return;
        }

        _timer -= dt;
        if (_timer <= 0f)
            IsFinished = true;
    }

    public void Stop(NPCActor actor)
    {
        _spot.Release(actor);
        actor.ReleaseFromSnap();
        actor.ExitAnimationAction();
    }
}