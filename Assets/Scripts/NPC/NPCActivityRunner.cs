using UnityEngine;

public class NPCActivityRunner : MonoBehaviour
{
    private NPCActor _actor;
    private INPCActivity _current;
    private INPCActivity _interrupt;

    public bool IsBusy => _interrupt is { IsFinished: false } || _current is { IsFinished: false };

    private void Awake()
    {
        _actor = GetComponent<NPCActor>();
    }

    public void Run(INPCActivity activity)
    {
        Debug.Log($"RUN ACTIVITY: {activity.GetType().Name}");
        
        _actor.ExitAnimationAction();

        _current?.Stop(_actor);
        _current = activity;
        _current.Start(_actor);
    }

    public void Reset()
    {
        Debug.Log("RESET ACTIVITY");

        _current?.Stop(_actor);
        _current = null;

        _interrupt?.Stop(_actor);
        _interrupt = null;
    }
    
    public void Interrupt(INPCActivity activity)
    {
        Debug.Log($"INTERRUPT: {activity.GetType().Name}");

        Reset();

        _interrupt = activity;
        _interrupt.Start(_actor);
    }

    private void Update()
    {
        if (_interrupt != null)
        {
            _interrupt.Tick(_actor, Time.deltaTime);

            if (_interrupt.IsFinished)
            {
                _interrupt.Stop(_actor);
                _interrupt = null;
            }

            return;
        }

        if (_current == null)
            return;

        _current.Tick(_actor, Time.deltaTime);

        if (_current.IsFinished)
            Reset();
    }
}