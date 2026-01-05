using UnityEngine;

public class ConcertMusicManager : MonoBehaviour
{
    [SerializeField] private InputReader _input;

    [Header("Settings")]
    [SerializeField] private float _chooseWindowSeconds = 3f;
    [SerializeField] private float _perfectTimingSeconds = 1f;
    [SerializeField] private float _midLoopTimingSeconds = 0.5f;

    [Header("Audio Packs")]
    [SerializeField] private ConcertPackAudioData _lopataPack;
    [SerializeField] private ConcertPackAudioData _gvozdiPack;

    [Header("Speakers")]
    [SerializeField] private SpeakerAudioSource[] _speakers;

    [Header("Track Masks")]
    [SerializeField] private TrackMask _concertMask = TrackMask.All;
    [SerializeField] private TrackMask _rehearsalMask = TrackMask.NoBass;

    [Header("Common Sounds")]
    [SerializeField] private AudioClip[] _missSounds;

    // ───────── STATE ─────────
    private ConcertPackAudioData _currentPack;
    private TrackMask _currentMask;

    private ConcertState _state = ConcertState.Idle;
    private TimingState _timingState = TimingState.Bad;

    private LoopType _currentLoopType;
    private LoopType? _queuedLoopType;

    private AudioClip _currentLoop;
    private double _nextLoopDsp;

    private bool _concertStarted;
    private bool _timerShown;

    // ───────── MID LOOP ─────────
    private bool _midLoopActive;
    private bool _midLoopWaitingForInput;
    private bool _midLoopEnded;

    private bool HasChoice => _queuedLoopType.HasValue;

    // ───────── LIFECYCLE ─────────

    private void Awake()
    {
        GameEvents.OnCallingConcertStart += () => Init(_concertMask);
        GameEvents.OnCallingRehearsalStart += () => Init(_rehearsalMask);
        GameEvents.OnGuitarUpdate += guitarType =>
        {
            if (guitarType is GuitarType.Lopata) _currentPack = _lopataPack;
            else if (guitarType is GuitarType.Gvozdi) _currentPack = _gvozdiPack; 
        };

        _input.ALoop += () => OnLoopPressed(LoopType.A);
        _input.BLoop += () => OnLoopPressed(LoopType.B);
        _input.CLoop += () => OnLoopPressed(LoopType.C);
        _input.DLoop += () => OnLoopPressed(LoopType.D);
    }

    private void Init(TrackMask mask)
    {
        ResetState();

        _currentMask = mask;
        _concertStarted = true;

        GameEvents.OnConcertStarted?.Invoke(
            new ConcertData(_chooseWindowSeconds, _perfectTimingSeconds)
        );
    }

    private void ResetState()
    {
        _state = ConcertState.Idle;
        _timingState = TimingState.Bad;

        _queuedLoopType = null;
        _currentLoop = null;

        _timerShown = false;
        ResetMidLoop();
    }

    private void ResetMidLoop()
    {
        _midLoopActive = false;
        _midLoopWaitingForInput = false;
        _midLoopEnded = false;
    }

    private void Update()
    {
        if (_state == ConcertState.Idle)
            return;

        var dspNow = AudioSettings.dspTime;

        HandleTimer(dspNow);

        if (dspNow >= _nextLoopDsp)
            ScheduleNext();
    }

    // ───────── INPUT ─────────

    private void OnLoopPressed(LoopType loop)
    {
        var guitarClip = _currentPack.GetGuitar(loop);

        // MID LOOP
        if (_midLoopActive && _midLoopWaitingForInput)
        {
            var result = guitarClip == _currentLoop
                ? TimingState.Perfect
                : TimingState.Bad;

            GameEvents.OnLoopTimingPressed?.Invoke(result);

            ResetMidLoop();
            _midLoopEnded = true;

            GameEvents.OnMidLoopTimingEnd?.Invoke();
            return;
        }

        SelectLoop(loop);
    }

    private void SelectLoop(LoopType loop)
    {
        if (!_concertStarted || HasChoice)
            return;

        if (_state == ConcertState.Idle)
        {
            StartIntroWithLoop(loop);
            return;
        }

        if (!_timerShown)
            return;

        ResolveTimingFeedback();
        _queuedLoopType = loop;
    }

    // ───────── FLOW ─────────

    private void StartIntroWithLoop(LoopType loop)
    {
        GameEvents.OnLoopChooseTimerEnd?.Invoke();
        _queuedLoopType = loop;
        StartIntro();
    }

    private void StartIntro()
    {
        _state = ConcertState.Intro;
        ScheduleLoop(LoopType.E, AudioSettings.dspTime);
    }

    private void ScheduleNext()
    {
        HideTimer();

        if (HasChoice)
        {
            var next = _queuedLoopType.Value;
            _queuedLoopType = null;

            _state = IsLastLoop(next)
                ? ConcertState.Finisher
                : ConcertState.Playing;

            ScheduleLoop(next, _nextLoopDsp);
            return;
        }

        if (_state == ConcertState.Finisher)
        {
            StopConcert();
            return;
        }

        ScheduleLoop(_currentLoopType, _nextLoopDsp);
    }

    private void ScheduleLoop(LoopType loop, double startDsp)
    {
        _currentLoopType = loop;

        var guitar = _currentMask.Guitar ? _currentPack.GetGuitar(loop) : null;
        var drums  = _currentMask.Drums  ? _currentPack.GetDrums(loop)  : null;
        var bass   = _currentMask.Bass   ? _currentPack.GetBass(loop)   : null;

        _currentLoop = guitar;

        var duration = (double)guitar.samples / guitar.frequency;
        _nextLoopDsp = startDsp + duration;

        ResetMidLoop();

        foreach (var s in _speakers)
            s.Play(guitar, drums, bass, startDsp);
    }

    // ───────── TIMERS ─────────

    private void HandleTimer(double dspNow)
    {
        if (_state != ConcertState.Playing || HasChoice || !_currentLoop)
            return;

        var timeLeft = _nextLoopDsp - dspNow;
        var loopLength = _currentLoop.length;

        // MID LOOP
        var halfTime = loopLength * 0.5f;
        var midStart = halfTime + _midLoopTimingSeconds;
        var midEnd = halfTime;

        if (!_midLoopActive && !_midLoopEnded &&
            timeLeft <= midStart && timeLeft > midEnd)
        {
            _midLoopActive = true;
            _midLoopWaitingForInput = true;
            GameEvents.OnMidLoopTimingStarted?.Invoke();
        }

        if (_midLoopActive)
        {
            var t = Mathf.InverseLerp(midStart, midEnd, (float)timeLeft);
            GameEvents.OnMidLoopTimingUpdate?.Invoke(t);
        }

        if (_midLoopActive && timeLeft <= midEnd)
        {
            GameEvents.OnLoopTimingPressed?.Invoke(TimingState.Bad);
            ResetMidLoop();
            _midLoopEnded = true;
            GameEvents.OnMidLoopTimingEnd?.Invoke();
        }

        // END LOOP
        _timingState = timeLeft <= _perfectTimingSeconds
            ? TimingState.Perfect
            : TimingState.Bad;

        if (!_timerShown && timeLeft <= _chooseWindowSeconds && timeLeft > 0)
        {
            _timerShown = true;
            GameEvents.OnLoopChooseTimerStart?.Invoke();
        }

        if (_timerShown)
        {
            var t = Mathf.Clamp01((float)(timeLeft / _chooseWindowSeconds));
            GameEvents.OnLoopChooseTimerUpdate?.Invoke(t);
        }
    }

    private void ResolveTimingFeedback()
    {
        GameEvents.OnLoopTimingPressed?.Invoke(_timingState);

        if (_timingState == TimingState.Bad && _missSounds.Length > 0)
        {
            var miss = _missSounds[Random.Range(0, _missSounds.Length)];
            foreach (var s in _speakers)
                s.PlayGuitarMiss(miss);
        }

        HideTimer();
    }

    private void HideTimer()
    {
        if (!_timerShown) return;

        _timerShown = false;
        GameEvents.OnLoopChooseTimerEnd?.Invoke();
    }

    private void StopConcert()
    {
        foreach (var s in _speakers)
            s.StopAll();

        ResetState();
        GameEvents.OnConcertFinished?.Invoke();
    }

    // ───────── HELPERS ─────────

    private bool IsLastLoop(LoopType loop) => loop == LoopType.D;

    private enum ConcertState
    {
        Idle,
        Intro,
        Playing,
        Finisher
    }

    public enum TimingState
    {
        Bad,
        Perfect
    }
}