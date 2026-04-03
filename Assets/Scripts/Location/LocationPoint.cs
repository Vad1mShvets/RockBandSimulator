using UnityEngine;

public class LocationPoint : MonoBehaviour
{
    [SerializeField] private LocationType _type;
    [SerializeField] private Transform _playerSpawnPoint;
    [SerializeField] private GameObject _locationRoot;

    public LocationType Type => _type;
    public Transform SpawnPoint => _playerSpawnPoint;

    private void OnEnable()
    {
        TravelSystem.RegisterPoint(this);
    }

    private void OnDisable()
    {
        TravelSystem.UnregisterPoint(this);
    }

    public void Activate()
    {
        if (_locationRoot != null)
            _locationRoot.SetActive(true);
    }

    public void Deactivate()
    {
        if (_locationRoot != null)
            _locationRoot.SetActive(false);
    }
}
