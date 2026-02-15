using UnityEngine;

public class NPCActivitySpot : MonoBehaviour
{
    private NPCActor _occupant;

    public bool IsFree => _occupant == null;

    public bool TryReserve(NPCActor actor)
    {
        if (_occupant != null)
            return false;

        _occupant = actor;
        return true;
    }

    public void Release(NPCActor actor)
    {
        if (_occupant == actor)
            _occupant = null;
    }
}