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
    
    [SerializeField] private FanDude _fanDudeBase;
    
    [Space]
    [SerializeField] private Transform _lookTarger;
    [SerializeField] private Vector3 _zoneSize;
    [SerializeField] private float _dudeZoneSize = 0.8f;
    [SerializeField] private float _dudeYOffset = -0.175f;

    [Space]
    [SerializeField] private int _minCount = 15;
    [SerializeField] private int _maxCount = 30;

    [Space]
    [SerializeField] private int _minFps = 5;
    [SerializeField] private int _maxFps = 22;
    
    [Space]
    [SerializeField] private AudioSource _crowdAmbient;

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
        GameEvents.OnLoopTimingPressed += OnTimingPressed;
    }

    private void OnDisable()
    {
        GameEvents.OnConcertStarted -= OnConcertStarted;
        GameEvents.OnNewLoopStart -= OnNewLoopStart;
        GameEvents.OnConcertFinished -= DestroyCrowd;
        GameEvents.OnLoopTimingPressed -= OnTimingPressed;
    }
    
    private void OnConcertStarted(ConcertData _)
    {
        SpawnCrowd();
        if (_crowdAmbient) _crowdAmbient.Play();
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

        ApplySteppedFps();
    }

    private void ApplySteppedFps()
    {
        if (_fanDudes.Count == 0)
            return;

        var targetPos = _lookTarger.position;
        var minDist = float.MaxValue;
        var maxDist = float.MinValue;

        foreach (var dude in _fanDudes)
        {
            var dist = Vector3.Distance(dude.transform.position, targetPos);
            if (dist < minDist) minDist = dist;
            if (dist > maxDist) maxDist = dist;
        }

        var range = maxDist - minDist;

        foreach (var dude in _fanDudes)
        {
            var dist = Vector3.Distance(dude.transform.position, targetPos);
            var t = range > 0.01f ? (dist - minDist) / range : 0f;
            var fps = Mathf.RoundToInt(Mathf.Lerp(_maxFps, _minFps, t));
            dude.SetFramerate(fps);
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

        if (_crowdAmbient) _crowdAmbient.Stop();
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

    private void OnTimingPressed(ConcertService.TimingState timingState)
    {
        switch (timingState)
        {
            case ConcertService.TimingState.Perfect:
                SoundsManager.PlaySound(SoundsManager.SoundType.CrowdCheering);
                break;
            case ConcertService.TimingState.Bad:
                SoundsManager.PlaySound(SoundsManager.SoundType.CrowdBooing);
                break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, _zoneSize);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_fanDudeBase.transform.position + Vector3.up, new Vector3(_dudeZoneSize, 2, _dudeZoneSize));
    }
}