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
        
        _plan.Enqueue(new GoToActivity(wanderPoint));
        
        if (sofaSpots.RequestFree(actor, out var sofaSpot))
            _plan.Enqueue(new UseSpotActivity(sofaSpot, 10f));
        
        _plan.Enqueue(new GoToActivity(wanderPoint));
    }

    private void Update()
    {
        if (runner.IsBusy || _plan.Count == 0)
            return;

        runner.Run(_plan.Dequeue());
    }
}