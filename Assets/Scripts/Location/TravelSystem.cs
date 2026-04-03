using System.Collections.Generic;
using System.Linq;

public static class TravelSystem
{
    private static readonly List<LocationPoint> _points = new();
    private static PlayerSnapMover _playerSnapMover;
    private static LocationType _currentLocation;

    public static LocationType CurrentLocation => _currentLocation;

    public static void Init()
    {
        _points.Clear();
        _playerSnapMover = null;
        _currentLocation = LocationType.Garage;
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
        if (destination == _currentLocation)
            return false;

        var targetPoint = _points.FirstOrDefault(p => p.Type == destination);
        if (targetPoint == null)
            return false;
        
        var currentPoint = _points.FirstOrDefault(p => p.Type == _currentLocation);
        if (currentPoint != null)
            currentPoint.Deactivate();
        
        targetPoint.Activate();
        
        if (_playerSnapMover != null && targetPoint.SpawnPoint != null)
            _playerSnapMover.SnapTo(targetPoint.SpawnPoint);

        _currentLocation = destination;

        GameEvents.OnLocationChanged?.Invoke(destination);
        return true;
    }
}
