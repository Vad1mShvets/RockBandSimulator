using System.Collections.Generic;
using UnityEngine;

public class NPCSimpleLife : MonoBehaviour
{
    [SerializeField] private NPCActivityRunner runner;
    [SerializeField] private Transform wanderPoint;
    [SerializeField] private NPCActivitySpotGroup sofaSpots;

    private Queue<INPCActivity> _plan = new();

    private void Start()
    {
        var actor = GetComponent<NPCActor>();
        
        var sit = sofaSpots.RequestFree(actor);

        _plan.Enqueue(new GoToActivity(wanderPoint));
        _plan.Enqueue(new UseSpotActivity(sit, 10f));
        _plan.Enqueue(new GoToActivity(wanderPoint));
    }

    private void Update()
    {
        if (runner.IsBusy || _plan.Count == 0)
            return;

        runner.Run(_plan.Dequeue());
    }
}