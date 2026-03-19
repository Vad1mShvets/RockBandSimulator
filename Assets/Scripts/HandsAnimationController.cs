using UnityEngine;
using UnityEngine.Video;

public class HandsAnimationController : MonoBehaviour
{
    private enum BaseState
    {
        Idle,
        Walking
    }

    private enum ModeState
    {
        Normal,
        Guitar,
        Combat
    }

    private enum OverrideState
    {
        None,
        Item,
        Attack
    }
    
    [SerializeField] private VideoPlayer _videoPlayer;

    [Header("Base Clips")]
    [SerializeField] private VideoClip _idleClip;
    [SerializeField] private VideoClip _walkClip;

    [Header("Mode Clips")]
    [SerializeField] private VideoClip _guitarClip;

    [Header("Override Clips")]
    [SerializeField] private VideoClip _beerClip;
    [SerializeField] private VideoClip _cigsClip;
    [SerializeField] private VideoClip[] _attackClips;

    [Header("Screens")]
    [SerializeField] private GameObject _bodyVideoScreen;
    [SerializeField] private GameObject _cameraVideoScreen;

    private BaseState _baseState = BaseState.Idle;
    private ModeState _mode = ModeState.Normal;
    private OverrideState _override = OverrideState.None;

    private VideoClip _currentClip;

    private void Awake()
    {
        _videoPlayer.playOnAwake = false;
        _videoPlayer.waitForFirstFrame = true;
        _videoPlayer.loopPointReached += OnVideoFinished;
    }
    
    private void OnEnable()
    {
        GameEvents.OnWalkingStart += OnWalkingStart;
        GameEvents.OnWalkingEnd += OnWalkingEnd;
        GameEvents.OnCallingConcertStart += OnConcertStart;
        GameEvents.OnConcertFinished += OnConcertFinished;
        GameEvents.OnCombatStart += OnCombatStart;
        GameEvents.OnCombatEnd += OnCombatEnd;
        GameEvents.OnInventoryItemUsed += OnItemUsed;
        GameEvents.OnAttack += PlayAttack;
    }

    private void OnDisable()
    {
        GameEvents.OnWalkingStart -= OnWalkingStart;
        GameEvents.OnWalkingEnd -= OnWalkingEnd;
        GameEvents.OnCallingConcertStart -= OnConcertStart;
        GameEvents.OnConcertFinished -= OnConcertFinished;
        GameEvents.OnCombatStart -= OnCombatStart;
        GameEvents.OnCombatEnd -= OnCombatEnd;
        GameEvents.OnInventoryItemUsed -= OnItemUsed;
        GameEvents.OnAttack -= PlayAttack;
    }

    private void Start()
    {
        Resolve();
    }

    // ========================= STATE =========================

    private void TryResolve()
    {
        if (_override != OverrideState.None)
            return;

        Resolve();
    }

    private void Resolve()
    {
        if (_override != OverrideState.None)
            return;

        if (_mode == ModeState.Guitar)
        {
            PlayLoop(_guitarClip);
            return;
        }

        PlayLoop(_baseState == BaseState.Walking ? _walkClip : _idleClip);
    }

    private void PlayLoop(VideoClip clip)
    {
        if (!clip || _currentClip == clip)
            return;

        _currentClip = clip;

        _videoPlayer.Stop();
        _videoPlayer.isLooping = true;
        _videoPlayer.clip = clip;
        _videoPlayer.Play();

        UpdateScreen(true);
    }

    // ========================= OVERRIDES =========================

    private void PlayAttack()
    {
        StartOverride(_attackClips[Random.Range(0, _attackClips.Length)], OverrideState.Attack);
    }

    private void OnItemUsed(InteractableTypes item)
    {
        if (_override != OverrideState.None)
            return;

        VideoClip clip = null;

        switch (item)
        {
            case InteractableTypes.Beer:
                clip = _beerClip;
                break;
            case InteractableTypes.Cigs:
                clip = _cigsClip;
                break;
        }

        if (!clip)
            return;

        StartOverride(clip, OverrideState.Item);
    }

    private void StartOverride(VideoClip clip, OverrideState state)
    {
        _override = state;
        _currentClip = clip;

        _videoPlayer.Stop();
        _videoPlayer.isLooping = false;
        _videoPlayer.clip = clip;

        _videoPlayer.prepareCompleted += OnPrepared;
        _videoPlayer.Prepare();

        // временно скрываем экран, чтобы не показать старый кадр
        _bodyVideoScreen.SetActive(false);
        _cameraVideoScreen.SetActive(false);
    }

    private void OnPrepared(VideoPlayer player)
    {
        _videoPlayer.prepareCompleted -= OnPrepared;

        _videoPlayer.Play();
        UpdateScreen(false);
    }

    private void OnVideoFinished(VideoPlayer player)
    {
        if (_override == OverrideState.None)
            return;

        _override = OverrideState.None;
        Resolve();
    }

    private void UpdateScreen(bool body)
    {
        _bodyVideoScreen.SetActive(body);
        _cameraVideoScreen.SetActive(!body);
    }
    
    // SUBSCRIPTION METHODS
    
    private void OnWalkingStart()
    {
        _baseState = BaseState.Walking;
        TryResolve();
    }

    private void OnWalkingEnd()
    {
        _baseState = BaseState.Idle;
        TryResolve();
    }

    private void OnConcertStart()
    {
        _mode = ModeState.Guitar;
        TryResolve();
    }

    private void OnConcertFinished()
    {
        _mode = ModeState.Normal;
        TryResolve();
    }

    private void OnCombatStart()
    {
        _mode = ModeState.Combat;
        TryResolve();
    }

    private void OnCombatEnd()
    {
        _mode = ModeState.Normal;
        TryResolve();
    }
}