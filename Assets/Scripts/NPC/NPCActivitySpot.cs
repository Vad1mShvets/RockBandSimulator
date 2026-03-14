using UnityEngine;

public class NPCActivitySpot : MonoBehaviour
{
    private NPCActor _occupant;

    public bool TryReserve(NPCActor actor)
    {
        if (_occupant)
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