public class UseSpotActivity : INPCActivity
{
    private NPCActivitySpot _spot;
    private float _duration;
    private float _timer;
    private bool _snapped;
    private bool _teleport;

    private string _animationName;

    public bool IsFinished { get; private set; }

    public UseSpotActivity(NPCActivitySpot spot, float duration, string animationName, bool teleport = false)
    {
        _spot = spot;
        _duration = duration;
        _animationName = animationName;
        _teleport = teleport;
    }

    public void Start(NPCActor actor)
    {
        IsFinished = false;
        _snapped = false;

        if (_teleport)
            Snap(actor);
        else
            actor.NavMeshMover.MoveTo(_spot.transform.position);
    }

    public void Tick(NPCActor actor, float dt)
    {
        if (!_snapped)
        {
            if (!_teleport && !actor.NavMeshMover.Arrived)
                return;

            Snap(actor);
            return;
        }

        _timer -= dt;

        if (_timer <= 0f)
            IsFinished = true;
    }

    private void Snap(NPCActor actor)
    {
        actor.NavMeshMover.Stop();
        actor.SnapTo(_spot.transform);
        actor.EnterAnimationAction(_animationName);

        _snapped = true;
        _timer = _duration;
    }

    public void Stop(NPCActor actor)
    {
        _spot.Release(actor);
        actor.ReleaseFromSnap();
    }
}