using UnityEngine;

public class ConcertMusicManager : MonoBehaviour
{
    [SerializeField] private InputReader _input;

    [Header("Settings")]
    [SerializeField] private float _chooseWindowSeconds = 3f;
    [SerializeField] private float _perfectTimingSeconds = 1f;

    [Header("Audio Data")]
    [SerializeField] private ConcertAudioData _guitar;
    [SerializeField] private ConcertAudioData _drums;
    [SerializeField] private ConcertAudioData _bass;

    [Header("Speakers")]
    [SerializeField] private SpeakerAudioSource[] _speakers;

    private ConcertState _state = ConcertState.Idle;
    private TimingState _timingState = TimingState.Bad;

    private AudioClip _currentLoop;
    private AudioClip _queuedLoop;

    private double _nextLoopDsp;

    private bool _concertStarted;
    private bool _timerShown;

    private bool HasChoice => _queuedLoop != null;
    
    public struct ConcertData
    {
        public float ChooseWindowSeconds;
        public float PerfectTimingSeconds;

        public ConcertData(float chooseWindowSeconds, float perfectTimingSeconds)
        {
            ChooseWindowSeconds =  chooseWindowSeconds;
            PerfectTimingSeconds = perfectTimingSeconds;
        }
    }

    private void Awake()
    {
        GameEvents.OnCallingConcertStart += Init;
        
        _input.ALoop += () => SelectLoop(_guitar.ALoop);
        _input.BLoop += () => SelectLoop(_guitar.BLoop);
        _input.CLoop += () => SelectLoop(_guitar.CLoop);
        _input.DLoop += () => SelectLoop(_guitar.DLoop);
    }

    private void Init()
    {
        ResetState();

        _concertStarted = true;

        GameEvents.OnConcertStarted?.Invoke(
            new ConcertData(_chooseWindowSeconds, _perfectTimingSeconds)
        );
    }

    private void ResetState()
    {
        _state = ConcertState.Idle;
        _timingState = TimingState.Bad;

        _currentLoop = null;
        _queuedLoop = null;
        _timerShown = false;
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
    
    private void SelectLoop(AudioClip clip)
    {
        if (!CanSelectLoop(clip))
            return;

        if (_state == ConcertState.Idle)
        {
            StartIntroWithLoop(clip);
            return;
        }

        if (!_timerShown)
            return;

        ResolveTimingFeedback();
        QueueLoop(clip);
    }

    private bool CanSelectLoop(AudioClip clip)
    {
        return _concertStarted && clip && !HasChoice;
    }

    private void StartIntroWithLoop(AudioClip clip)
    {
        GameEvents.OnLoopChooseTimerEnd?.Invoke();
        _queuedLoop = clip;
        StartIntro();
    }

    private void ResolveTimingFeedback()
    {
        GameEvents.OnLoopTimingPressed?.Invoke(_timingState);

        if (_timingState == TimingState.Bad)
            foreach (var s in _speakers)
                s.PlayGuitarMiss(
                    _guitar.MissSounds[Random.Range(0, _guitar.MissSounds.Length)]
                );

        HideTimer();
    }

    private void QueueLoop(AudioClip clip)
    {
        _queuedLoop = clip;
    }

    private void StartIntro()
    {
        _state = ConcertState.Intro;

        var startDsp = AudioSettings.dspTime;
        ScheduleLoop(_guitar.ELoop, startDsp);
    }

    private void ScheduleNext()
    {
        HideTimer();

        if (HasChoice)
        {
            var next = _queuedLoop;
            _queuedLoop = null;

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
        
        ScheduleLoop(_currentLoop, _nextLoopDsp);
    }

    private void ScheduleLoop(AudioClip clip, double startDsp)
    {
        _currentLoop = clip;

        var duration = (double)clip.samples / clip.frequency;
        _nextLoopDsp = startDsp + duration;

        foreach (var s in _speakers)
        {
            s.Play(
                clip,
                GetDrums(clip),
                GetBass(clip),
                startDsp
            );
        }
    }
    
    private void HandleTimer(double dspNow)
    {
        if (_state != ConcertState.Playing || HasChoice)
            return;

        var timeLeft = _nextLoopDsp - dspNow;

        if (timeLeft <= _perfectTimingSeconds)
        {
            _timingState = TimingState.Perfect;
        }
        else
        {
            _timingState = TimingState.Bad;
        }

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
    }
    
    private bool IsLastLoop(AudioClip clip) => clip == _guitar.DLoop;

    private AudioClip GetDrums(AudioClip g)
    {
        if (g == _guitar.ALoop) return _drums.ALoop;
        if (g == _guitar.BLoop) return _drums.BLoop;
        if (g == _guitar.CLoop) return _drums.CLoop;
        if (g == _guitar.DLoop) return _drums.DLoop;
        return _drums.ELoop;
    }

    private AudioClip GetBass(AudioClip g)
    {
        if (g == _guitar.ALoop) return _bass.ALoop;
        if (g == _guitar.BLoop) return _bass.BLoop;
        if (g == _guitar.CLoop) return _bass.CLoop;
        if (g == _guitar.DLoop) return _bass.DLoop;
        return _bass.ELoop;
    }

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
