using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TravelSystem : MonoBehaviour
{
    public static TravelSystem Instance { get; private set; }

    [Header("Locations")]
    [SerializeField] private LocationPoint[] _locations;
    [SerializeField] private LocationData[] _locationDataList;

    [Header("References")]
    [SerializeField] private PlayerSnapMover _playerSnapMover;

    [Header("Travel Settings")]
    [SerializeField] private float _travelFadeDelay = 0.5f;

    private LocationType _currentLocation = LocationType.Garage;
    private bool _isTraveling;

    private HashSet<LocationType> _unlockedLocations = new();

    public LocationType CurrentLocation => _currentLocation;
    public bool IsTraveling => _isTraveling;

    private void Awake()
    {
        Instance = this;

        // Unlock default locations
        foreach (var data in _locationDataList)
        {
            if (data.UnlockedByDefault)
                _unlockedLocations.Add(data.Type);
        }
    }

    private void OnEnable()
    {
        GameEvents.OnReputationUpdated += CheckUnlocks;
    }

    private void OnDisable()
    {
        GameEvents.OnReputationUpdated -= CheckUnlocks;
    }

    private void CheckUnlocks()
    {
        foreach (var data in _locationDataList)
        {
            if (_unlockedLocations.Contains(data.Type)) continue;
            if (data.UnlockReputation > 0 && ReputationManager.CurrentReputation >= data.UnlockReputation)
            {
                _unlockedLocations.Add(data.Type);
            }
        }
    }

    public bool IsLocationUnlocked(LocationType type)
    {
        return _unlockedLocations.Contains(type);
    }

    public LocationData[] GetAllLocationData()
    {
        return _locationDataList;
    }

    public HashSet<LocationType> GetUnlockedLocations()
    {
        return _unlockedLocations;
    }

    public void UnlockLocation(LocationType type)
    {
        _unlockedLocations.Add(type);
    }

    public void TravelTo(LocationType destination)
    {
        if (_isTraveling) return;
        if (destination == _currentLocation) return;
        if (!_unlockedLocations.Contains(destination)) return;

        StartCoroutine(TravelRoutine(destination));
    }

    private IEnumerator TravelRoutine(LocationType destination)
    {
        _isTraveling = true;
        GameEvents.OnTravelStarted?.Invoke();

        // 1. Lock player
        PlayerStateController.TryEnterBusy();

        // 2. Fade to black
        yield return FadeScreenUI.Instance.FadeIn();

        // Small delay for "travel feel"
        yield return new WaitForSeconds(_travelFadeDelay);

        // 3. Deactivate current location
        var currentPoint = GetLocationPoint(_currentLocation);
        if (currentPoint != null)
            currentPoint.Deactivate();

        // 4. Activate destination
        var targetPoint = GetLocationPoint(destination);
        if (targetPoint != null)
        {
            targetPoint.Activate();

            // 5. Teleport player
            if (targetPoint.SpawnPoint != null)
                _playerSnapMover.SnapTo(targetPoint.SpawnPoint);
        }

        _currentLocation = destination;

        // 6. Fade back in
        yield return FadeScreenUI.Instance.FadeOut();

        // 7. Unlock player
        PlayerStateController.ExitBusy();

        _isTraveling = false;

        GameEvents.OnLocationChanged?.Invoke(destination);
        GameEvents.OnTravelFinished?.Invoke();
    }

    private LocationPoint GetLocationPoint(LocationType type)
    {
        return _locations.FirstOrDefault(l => l.Type == type);
    }
}
