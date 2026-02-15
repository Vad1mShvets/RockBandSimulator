using System.Collections.Generic;
using UnityEngine;

public class NPCActivitySpotGroup : MonoBehaviour
{
    [SerializeField] private List<NPCActivitySpot> spots = new();

    public NPCActivitySpot RequestFree(NPCActor actor)
    {
        foreach (var spot in spots)
        {
            if (spot.TryReserve(actor))
                return spot;
        }

        return null;
    }
}