using UnityEngine;

public class NPCActivityRunner : MonoBehaviour
{
    private NPCActor _actor;
    private INPCActivity _current;

    public bool IsBusy => _current is { IsFinished: false };

    private void Awake()
    {
        _actor = GetComponent<NPCActor>();
    }

    public void Run(INPCActivity activity)
    {
        Debug.Log($"RUN ACTIVITY: {activity.GetType().Name}");

        _current?.Stop(_actor);
        _current = activity;
        _current.Start(_actor);
    }

    public void Reset()
    {
        Debug.Log("RESET ACTIVITY");

        _current?.Stop(_actor);
        _current = null;
    }

    private void Update()
    {
        if (_current == null)
            return;

        _current.Tick(_actor, Time.deltaTime);

        if (_current.IsFinished)
        {
            Debug.Log($"FINISHED ACTIVITY: {_current.GetType().Name}");

            Reset();
        }
    }
}