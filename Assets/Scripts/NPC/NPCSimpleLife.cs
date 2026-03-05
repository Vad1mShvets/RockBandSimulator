using System.Collections.Generic;
using UnityEngine;

public class NPCSimpleLife : MonoBehaviour
{
    [SerializeField] private NPCActor _actor;
    [SerializeField] private Transform _wanderPoint;
    [SerializeField] private NPCActivitySpotGroup _sofaSpots;

    private Queue<INPCActivity> _plan = new();

    private void Start()
    {
        var actor = GetComponent<NPCActor>();
        
        _plan.Enqueue(new GoToActivity(_wanderPoint));
        
        if (_sofaSpots.RequestFree(actor, out var sofaSpot))
            _plan.Enqueue(new UseSpotActivity(sofaSpot, 10f));
        
        _plan.Enqueue(new GoToActivity(_wanderPoint));
    }

    private void Update()
    {
        if (_actor.ActivityRunner.IsBusy || _plan.Count == 0)
            return;

        _actor.ActivityRunner.Run(_plan.Dequeue());
    }
}