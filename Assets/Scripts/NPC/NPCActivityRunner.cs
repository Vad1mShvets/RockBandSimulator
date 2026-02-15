using UnityEngine;

public class NPCActivityRunner : MonoBehaviour
{
    private NPCActor _actor;
    private INPCActivity _current;

    public bool IsBusy => _current != null;

    private void Awake()
    {
        _actor = GetComponent<NPCActor>();
    }

    public void Run(INPCActivity activity)
    {
        _current?.Stop(_actor);
        _current = activity;
        _current.Start(_actor);
    }

    private void Update()
    {
        if (_current == null)
            return;

        _current.Tick(_actor, Time.deltaTime);

        if (_current.IsFinished)
        {
            _current.Stop(_actor);
            _current = null;
        }
    }
}