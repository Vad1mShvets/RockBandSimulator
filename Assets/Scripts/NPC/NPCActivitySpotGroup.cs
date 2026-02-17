using System.Collections.Generic;
using UnityEngine;

public class NPCActivitySpotGroup : MonoBehaviour
{
    [SerializeField] private List<NPCActivitySpot> spots = new();

    public bool RequestFree(NPCActor actor, out NPCActivitySpot activitySpot)
    {
        activitySpot = null;
        
        foreach (var spot in spots)
        {
            if (spot.TryReserve(actor))
            {
                activitySpot = spot;
                return true;
            }
        }

        return false;
    }
}