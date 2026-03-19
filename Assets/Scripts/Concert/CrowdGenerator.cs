using System.Collections.Generic;
using UnityEngine;

public class CrowdGenerator : MonoBehaviour
{
    public enum CrowdActionType
    {
        Idle = 0,
        Dance = 1,
        Applause = 2
    }
    
    [SerializeField] private Transform _lookTarger;
    [SerializeField] private Vector3 _zoneSize;
    [SerializeField] private float _dudeZoneSize = 0.8f;
    [SerializeField] private float _dudeYOffset = -0.175f;

    [SerializeField] private int _minCount = 15;
    [SerializeField] private int _maxCount = 30;

    [SerializeField] private FanDude _fanDudeBase;

    private readonly List<FanDude> _fanDudes = new();

    private void Awake()
    {
        _fanDudeBase.gameObject.SetActive(false);
    }
    
    private void OnEnable()
    {
        GameEvents.OnConcertStarted += OnConcertStarted;
        GameEvents.OnNewLoopStart += OnNewLoopStart;
        GameEvents.OnConcertFinished += DestroyCrowd;
    }

    private void OnDisable()
    {
        GameEvents.OnConcertStarted -= OnConcertStarted;
        GameEvents.OnNewLoopStart -= OnNewLoopStart;
        GameEvents.OnConcertFinished -= DestroyCrowd;
    }
    
    private void OnConcertStarted(ConcertData _)
    {
        SpawnCrowd();
        SetActionForAll(CrowdActionType.Idle);
    }

    private void OnNewLoopStart(LoopType loopType)
    {
        switch (loopType)
        {
            case LoopType.E:
                SetActionForAll(CrowdActionType.Idle);
                break;
            case LoopType.D:
                SetActionForAll(CrowdActionType.Applause);
                break;
            default:
                SetActionForAll(CrowdActionType.Dance);
                break;
        }
    }
    
    private void SetActionForAll(CrowdActionType action)
    {
        foreach (var dude in _fanDudes)
            dude.SetAction(action);
    }

    private void SpawnCrowd()
    {
        DestroyCrowd();

        var count = Random.Range(_minCount, _maxCount + 1);
        var attemptsLimit = count * 10;

        for (var i = 0; i < count && attemptsLimit > 0; i++)
        {
            attemptsLimit--;

            var position = GetRandomPosition();

            if (!IsPositionValid(position))
                continue;

            var dude = Instantiate(_fanDudeBase, position, Quaternion.identity, transform);
            dude.transform.LookAt(_lookTarger);
            dude.gameObject.SetActive(true);
            _fanDudes.Add(dude);
        }
    }

    private void DestroyCrowd()
    {
        foreach (var dude in _fanDudes)
        {
            if (dude)
                Destroy(dude.gameObject);
        }

        _fanDudes.Clear();
    }

    private Vector3 GetRandomPosition()
    {
        var half = _zoneSize * 0.5f;

        return transform.position + new Vector3(
            Random.Range(-half.x, half.x),
            _dudeYOffset,
            Random.Range(-half.z, half.z)
        );
    }

    private bool IsPositionValid(Vector3 position)
    {
        foreach (var dude in _fanDudes)
        {
            if (Vector3.Distance(position, dude.transform.position) < _dudeZoneSize)
                return false;
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, _zoneSize);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_fanDudeBase.transform.position + Vector3.up, new Vector3(_dudeZoneSize, 2, _dudeZoneSize));
    }
}