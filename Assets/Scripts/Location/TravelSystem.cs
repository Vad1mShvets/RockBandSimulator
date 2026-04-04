using System.Collections.Generic;
using System.Linq;

public static class TravelSystem
{
    public static LocationType CurrentLocation { get; private set; }
    
    private static readonly List<LocationPoint> _points = new();
    private static PlayerSnapMover _playerSnapMover;

    public static void Init()
    {
        _points.Clear();
        _playerSnapMover = null;
        CurrentLocation = LocationType.Garage;
    }

    public static void RegisterPoint(LocationPoint point)
    {
        if (!_points.Contains(point))
            _points.Add(point);
    }

    public static void UnregisterPoint(LocationPoint point)
    {
        _points.Remove(point);
    }

    public static void SetPlayerSnapMover(PlayerSnapMover mover)
    {
        _playerSnapMover = mover;
    }
    
    public static bool SwitchLocation(LocationType destination)
    {
        if (destination == CurrentLocation)
            return false;

        var targetPoint = _points.FirstOrDefault(p => p.Type == destination);
        if (!targetPoint)
            return false;
        
        var currentPoint = _points.FirstOrDefault(p => p.Type == CurrentLocation);
        if (currentPoint)
            currentPoint.Deactivate();
        
        targetPoint.Activate();
        
        if (_playerSnapMover && targetPoint.SpawnPoint)
            _playerSnapMover.SnapTo(targetPoint.SpawnPoint);

        CurrentLocation = destination;

        GameEvents.OnLocationChanged?.Invoke(destination);
        return true;
    }
}
