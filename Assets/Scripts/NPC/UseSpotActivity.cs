using UnityEngine;

public class UseSpotActivity : INPCActivity
{
    private NPCActivitySpot _spot;
    private float _duration;
    private float _timer;
    private bool _snapped;

    private string _animation;

    public bool IsFinished { get; private set; }

    public UseSpotActivity(NPCActivitySpot spot, float duration, string animation)
    {
        _spot = spot;
        _duration = duration;
        _animation = animation;
    }

    public void Start(NPCActor actor)
    {
        IsFinished = false;
        _snapped = false;
        
        actor.NavMeshMover.MoveTo(_spot.transform.position);
    }

    public void Tick(NPCActor actor, float dt)
    {
        if (!_snapped)
        {
            if (!actor.NavMeshMover.Arrived)
                return;

            actor.SnapTo(_spot.transform);
            actor.EnterAnimationAction(_animation);

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
    }
}