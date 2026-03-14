using System.Collections.Generic;
using UnityEngine;

public class NPCRoutine : MonoBehaviour
{
    [SerializeField] private NPCActor _actor;
    [SerializeField] private List<NPCRoutineStep> _steps;

    private Queue<INPCActivity> _plan = new();

    private void Start()
    {
        BuildPlan();
    }

    private void Update()
    {
        if (_actor.ActivityRunner.IsBusy)
            return;

        if (_plan.Count == 0)
        {
            BuildPlan();
            return;
        }

        _actor.ActivityRunner.Run(_plan.Dequeue());
    }

    private void BuildPlan()
    {
        foreach (var step in _steps)
            step.Build(_actor, _plan);
    }
    
    public void ClearPlan()
    {
        _plan.Clear();
    }
}