using UnityEngine;

public class ConcertService : MonoBehaviour
{
    // ───────── ENUMS ─────────

    private enum ConcertState
    {
        Idle,
        Intro,
        Playing,
        Finisher
    }

    private enum MidLoopState
    {
        Inactive,
        Active,
        Ended
    }

    public enum TimingState
    {
        Bad,
        Perfect
    }

    // ───────── SERIALIZED ─────────

    [Header("Timing")] [SerializeField] private float _chooseWindowSeconds = 2f;
    [SerializeField] private float _perfectTimingSeconds = 0.2f;
    [SerializeField] private float _midLoopTimingSeconds = 1f;

    [Header("Audio Packs")] [SerializeField]
    private ConcertPackAudioData _lopataPack;

    [SerializeField] private ConcertPackAudioData _gvozdiPack;

    [Header("Speakers")] [SerializeField] private SpeakerAudioSource[] _speakers;

    [Header("Track Masks")] [SerializeField]
    private TrackMask _concertMask = new(true, true, true);

    [SerializeField] private TrackMask _rehearsalMask = new(true, true, false);

    [Header("Common Sounds")] [SerializeField]
    private AudioClip[] _missSounds;

    // ───────── STATE ─────────

    private ConcertPackAudioData _currentPack;
    private TrackMask _currentMask;
    private TrackMask _liveMask;

    private ConcertState _concertState = ConcertState.Idle;
    private MidLoopState _midLoopState = MidLoopState.Inactive;
    private TimingState _timingState = TimingState.Bad;

    private LoopType _currentLoopType;
    private LoopType? _queuedLoopType;

    private AudioClip _currentLoop;
    private double _nextLoopDsp;

    private bool _concertStarted;
    private bool _timerShown;

    // ───────── DERIVED ─────────

    private bool IsPlaying => _concertState == ConcertState.Playing;
    private bool HasChoice => _queuedLoopType.HasValue;
    private bool MidLoopReady => _midLoopState == MidLoopState.Active;

    // ─────────────────────────────────────────────────────────────────
    // LIFECYCLE
    // ─────────────────────────────────────────────────────────────────

    private void OnEnable()
    {
        GameEvents.OnCallingConcertStart += OnConcertStart;
        GameEvents.OnCallingRehearsalStart += OnRehearsalStart;
        GameEvents.OnGuitarUpdate += OnGuitarUpdate;
        GameEvents.OnInstrumentStarted += OnInstrumentStarted;
        GameEvents.OnInstrumentStopped += OnInstrumentStopped;
        
        GameEvents.OnConcertFinishScreenClosed += StopConcert;

        GameEvents.OnLastNoteBonusPressed += OnLastNotePressed;
        
        InputReader.Instance.ALoop += ALoopPressed;
        InputReader.Instance.BLoop += BLoopPressed;
        InputReader.Instance.CLoop += CLoopPressed;
        InputReader.Instance.DLoop += DLoopPressed;
    }

    private void OnDisable()
    {
        GameEvents.OnCallingConcertStart -= OnConcertStart;
        GameEvents.OnCallingRehearsalStart -= OnRehearsalStart;
        GameEvents.OnGuitarUpdate -= OnGuitarUpdate;
        GameEvents.OnInstrumentStarted -= OnInstrumentStarted;
        GameEvents.OnInstrumentStopped -= OnInstrumentStopped;
        
        GameEvents.OnConcertFinishScreenClosed -= StopConcert;
        
        GameEvents.OnLastNoteBonusPressed -= OnLastNotePressed;
        
        InputReader.Instance.ALoop -= ALoopPressed;
        InputReader.Instance.BLoop -= BLoopPressed;
        InputReader.Instance.CLoop -= CLoopPressed;
        InputReader.Instance.DLoop -= DLoopPressed;
    }

    private void ALoopPressed() => OnLoopPressed(LoopType.A);
    private void BLoopPressed() => OnLoopPressed(LoopType.B);
    private void CLoopPressed() => OnLoopPressed(LoopType.C);
    private void DLoopPressed() => OnLoopPressed(LoopType.D);

    private void Update()
    {
        if (_concertState == ConcertState.Idle) return;

        var dspNow = AudioSettings.dspTime;

        TickTimers(dspNow);

        if (dspNow >= _nextLoopDsp)
            AdvanceLoop();
    }

    // ─────────────────────────────────────────────────────────────────
    // INIT
    // ─────────────────────────────────────────────────────────────────

    private void OnConcertStart() => Init(_concertMask);
    private void OnRehearsalStart() => Init(_rehearsalMask);

    private void Init(TrackMask mask)
    {
        ResetState();

        _currentMask = mask;
        _concertStarted = true;
        _liveMask = new TrackMask(true, true, true);

        OnGuitarUpdate(GuitarType.Lopata);
        ApplyMix();

        GameEvents.OnConcertStarted?.Invoke(new ConcertData(_chooseWindowSeconds, _perfectTimingSeconds));
    }

    private void ResetState()
    {
        _concertState = ConcertState.Idle;
        _timingState = TimingState.Bad;
        _midLoopState = MidLoopState.Inactive;
        _queuedLoopType = null;
        _currentLoop = null;
        _timerShown = false;
    }

    // ─────────────────────────────────────────────────────────────────
    // INPUT
    // ─────────────────────────────────────────────────────────────────

    private void OnLoopPressed(LoopType loop)
    {
        if (MidLoopReady)
        {
            ResolveMidLoopInput(loop);
            return;
        }

        SelectLoop(loop);
    }

    private void SelectLoop(LoopType loop)
    {
        if (!_concertStarted || HasChoice) return;

        if (_concertState == ConcertState.Idle)
        {
            StartIntroWithLoop(loop);
            return;
        }

        if (!_timerShown) return;

        ResolveEndLoopTiming();
        _queuedLoopType = loop;
    }

    // ─────────────────────────────────────────────────────────────────
    // CONCERT FLOW
    // ─────────────────────────────────────────────────────────────────

    private void StartIntroWithLoop(LoopType loop)
    {
        GameEvents.OnLoopChooseTimerEnd?.Invoke();
        _queuedLoopType = loop;
        _concertState = ConcertState.Intro;
        ScheduleLoop(LoopType.E, AudioSettings.dspTime);
    }

    private void AdvanceLoop()
    {
        HideTimer();

        if (HasChoice)
        {
            PlayQueuedLoop();
            return;
        }

        if (_concertState == ConcertState.Finisher)
        {
            StopConcert();
            return;
        }

        ScheduleLoop(_currentLoopType, _nextLoopDsp); // repeat current
    }

    private void PlayQueuedLoop()
    {
        var next = _queuedLoopType.Value;
        _queuedLoopType = null;

        _concertState = IsLastLoop(next) ? ConcertState.Finisher : ConcertState.Playing;
        ScheduleLoop(next, _nextLoopDsp);
    }

    private void ScheduleLoop(LoopType loop, double startDsp)
    {
        _currentLoopType = loop;
        _midLoopState = MidLoopState.Inactive;

        var guitar = _currentMask.Guitar ? _currentPack.GetGuitar(loop) : null;
        var drums = _currentMask.Drums ? _currentPack.GetDrums(loop) : null;
        var bass = _currentMask.Bass ? _currentPack.GetBass(loop) : null;

        _currentLoop = guitar;
        _nextLoopDsp = startDsp + GetClipDuration(guitar ?? drums ?? bass);

        foreach (var s in _speakers)
            s.Play(guitar, drums, bass, startDsp);

        GameEvents.OnNewLoopStart?.Invoke(_currentLoopType);
    }

    private void StopConcert()
    {
        StopSpeakers();

        ResetState();
        GameEvents.OnConcertFinished?.Invoke();
    }

    private void StopSpeakers()
    {
        foreach (var s in _speakers)
            s.StopAll();
    }

    private void OnLastNotePressed(bool value)
    {
        StopSpeakers();
        //Play F Loop on speakers here
    }

    // ─────────────────────────────────────────────────────────────────
    // TIMERS
    // ─────────────────────────────────────────────────────────────────

    private void TickTimers(double dspNow)
    {
        if (!IsPlaying || HasChoice || _currentLoop == null) return;

        var timeLeft = _nextLoopDsp - dspNow;

        TickMidLoopTimer(timeLeft);
        TickEndLoopTimer(timeLeft);
    }

    private void TickMidLoopTimer(double timeLeft)
    {
        if (_midLoopState == MidLoopState.Ended) return;

        var halfTime = _currentLoop.length * 0.5f;
        var midStart = halfTime + _midLoopTimingSeconds;
        var midEnd = halfTime;

        if (_midLoopState == MidLoopState.Inactive && timeLeft <= midStart && timeLeft > midEnd)
        {
            _midLoopState = MidLoopState.Active;
            GameEvents.OnMidLoopTimingStarted?.Invoke(_currentLoopType);
        }

        if (_midLoopState == MidLoopState.Active)
        {
            var t = Mathf.InverseLerp((float)midStart, (float)midEnd, (float)timeLeft);
            GameEvents.OnMidLoopTimingUpdate?.Invoke(t);

            if (timeLeft <= midEnd)
                EndMidLoop(TimingState.Bad); // window expired — auto-miss
        }
    }

    private void TickEndLoopTimer(double timeLeft)
    {
        _timingState = timeLeft <= _perfectTimingSeconds ? TimingState.Perfect : TimingState.Bad;

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

    // ─────────────────────────────────────────────────────────────────
    // TIMING RESOLUTION
    // ─────────────────────────────────────────────────────────────────

    private void ResolveMidLoopInput(LoopType loop)
    {
        var pressedClip = _currentPack.GetGuitar(loop);
        var result = pressedClip == _currentLoop ? TimingState.Perfect : TimingState.Bad;
        EndMidLoop(result);
    }

    private void EndMidLoop(TimingState result)
    {
        _midLoopState = MidLoopState.Ended;

        GameEvents.OnLoopTimingPressed?.Invoke(result);
        GameEvents.OnMidLoopTimingEnd?.Invoke();

        if (result is TimingState.Bad)
            PlayGuitarMissOnSpeakers();
    }

    private void ResolveEndLoopTiming()
    {
        GameEvents.OnLoopTimingPressed?.Invoke(_timingState);

        PlayGuitarMissOnSpeakers();

        HideTimer();
    }

    private void PlayGuitarMissOnSpeakers()
    {
        if (_timingState == TimingState.Bad && _missSounds.Length > 0)
        {
            var miss = _missSounds[Random.Range(0, _missSounds.Length)];
            foreach (var s in _speakers)
                s.PlayGuitarMiss(miss);
        }
    }

    private void HideTimer()
    {
        if (!_timerShown) return;
        _timerShown = false;
        GameEvents.OnLoopChooseTimerEnd?.Invoke();
    }

    // ─────────────────────────────────────────────────────────────────
    // MIX
    // ─────────────────────────────────────────────────────────────────

    private void OnGuitarUpdate(GuitarType type)
    {
        _currentPack = type == GuitarType.Lopata ? _lopataPack : _gvozdiPack;
    }

    private void OnInstrumentStarted(NPCActor.NPCType type) => SetLiveMask(type, true);
    private void OnInstrumentStopped(NPCActor.NPCType type) => SetLiveMask(type, false);

    private void SetLiveMask(NPCActor.NPCType type, bool active)
    {
        switch (type)
        {
            case NPCActor.NPCType.Evgen: _liveMask.Bass = active; break;
            case NPCActor.NPCType.Diman: _liveMask.Drums = active; break;
            default: _liveMask.Guitar = active; break;
        }

        ApplyMix();
    }

    private void ApplyMix()
    {
        var guitar = _currentMask.Guitar && _liveMask.Guitar;
        var drums = _currentMask.Drums && _liveMask.Drums;
        var bass = _currentMask.Bass && _liveMask.Bass;

        foreach (var s in _speakers)
        {
            s.SetGuitarActive(guitar);
            s.SetDrumsActive(drums);
            s.SetBassActive(bass);
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // HELPERS
    // ─────────────────────────────────────────────────────────────────

    private static double GetClipDuration(AudioClip clip)
    {
        if (clip)
            return (double)clip.samples / clip.frequency;

        Debug.LogError("[ConcertService] Cannot compute loop duration — all track clips are null.");
        return 0;
    }

    private static bool IsLastLoop(LoopType loop) => loop == LoopType.D;
}