using UnityEngine;

public class FightActivity : INPCActivity
{
    private const string PUNCH_TAG = "Punch";
    private const int ANIM_LAYER = 1;
    private const int PUNCH_ANIMATIONS_COUNT = 1;

    private const float ATTACK_RANGE = 1.75f;
    
    private Transform _target;

    public bool IsFinished { get; }

    public FightActivity(Transform target)
    {
        _target = target;
    }

    public void Start(NPCActor actor)
    {
        actor.NavMeshMover.SetActiveAutoRotation(false);
    }

    public void Tick(NPCActor actor, float deltaTime)
    {
        var dir = _target.position - actor.transform.position;
        var sqrDist = dir.sqrMagnitude;
        var attackSqr = ATTACK_RANGE * ATTACK_RANGE;

        RotateToTarget(actor, dir);

        if (sqrDist > attackSqr)
        {
            actor.NavMeshMover.MoveTo(_target.position);
        }
        else
        {
            actor.NavMeshMover.Stop();
            
            if (!IsPunchPlaying(actor))
                PlayRandomPunch(actor);
        }
    }
    
    private void RotateToTarget(NPCActor actor, Vector3 dir)
    {
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.0001f)
            return;

        actor.transform.rotation = Quaternion.LookRotation(dir);
    }

    private bool IsPunchPlaying(NPCActor actor)
    {
        var animator = actor.Animator;

        if (animator.IsInTransition(ANIM_LAYER))
            return true;

        var state = animator.GetCurrentAnimatorStateInfo(ANIM_LAYER);
        return state.IsTag(PUNCH_TAG) && state.normalizedTime < 1f;
    }

    private void PlayRandomPunch(NPCActor actor)
    {
        actor.PlayAnimation($"{PUNCH_TAG}{Random.Range(0, PUNCH_ANIMATIONS_COUNT)}");
    }

    public void Stop(NPCActor actor) { }
}