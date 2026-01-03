using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class HandsAnimationController : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayer;

    [Header("Clips")]
    [SerializeField] private VideoClip _guitarClip;
    [SerializeField] private VideoClip _walkClip;
    [SerializeField] private VideoClip _idleClip;
    [SerializeField] private VideoClip _cigsClip;
    [SerializeField] private VideoClip _beerClip;

    [Header("Screens")]
    [SerializeField] private GameObject _bodyVideoScreen;
    [SerializeField] private GameObject _cameraVideoScreen;

    private readonly Dictionary<AnimationState, VideoClip> _clips = new();

    private AnimationState _currentState;
    private bool _isGuitarMode;
    private bool _isItemPlaying;
    private bool _isWalking;

    private void Awake()
    {
        _clips[AnimationState.GuitarPlaying] = _guitarClip;
        _clips[AnimationState.Walking] = _walkClip;
        _clips[AnimationState.Idle] = _idleClip;

        GameEvents.OnCallingConcertStart += EnterGuitarMode;
        GameEvents.OnCallingRehearsalStart += EnterGuitarMode;
        GameEvents.OnConcertFinished += ExitGuitarMode;

        GameEvents.OnWalkingStart += OnWalkingStart;
        GameEvents.OnWalkingEnd += OnWalkingEnd;

        GameEvents.OnInventoryItemUsed += OnInventoryItemUsed;
    }

    private void Start()
    {
        SetState(AnimationState.Idle, true);
    }

    private void EnterGuitarMode()
    {
        _isGuitarMode = true;
        SetState(AnimationState.GuitarPlaying, true);
    }

    private void ExitGuitarMode()
    {
        _isGuitarMode = false;
        ResolveBaseState();
    }

    private void OnWalkingStart()
    {
        _isWalking = true;
        SetState(AnimationState.Walking);
    }

    private void OnWalkingEnd()
    {
        _isWalking = false;
        SetState(AnimationState.Idle);
    }

    private void OnInventoryItemUsed(InteractableTypes item)
    {
        if (_isGuitarMode || _isItemPlaying)
            return;

        switch (item)
        {
            case InteractableTypes.Beer:
                PlayItemOnce(_beerClip);
                break;

            case InteractableTypes.Cigs:
                PlayItemOnce(_cigsClip);
                break;
        }
    }

    private void PlayItemOnce(VideoClip clip)
    {
        if (!clip)
            return;

        CancelInvoke(nameof(ReturnFromItem));

        _isItemPlaying = true;

        _videoPlayer.isLooping = false;
        _videoPlayer.clip = clip;
        _videoPlayer.Play();

        _cameraVideoScreen.SetActive(true);
        _bodyVideoScreen.SetActive(false);

        Invoke(nameof(ReturnFromItem), Mathf.Max(0.05f, (float)clip.length));
    }

    private void ReturnFromItem()
    {
        _isItemPlaying = false;
        ResolveBaseState();
    }

    private void ResolveBaseState()
    {
        if (_isGuitarMode)
        {
            SetState(AnimationState.GuitarPlaying, true);
            return;
        }

        SetState(_isWalking ? AnimationState.Walking : AnimationState.Idle, true);
    }

    private void SetState(AnimationState newState, bool force = false)
    {
        if (_isItemPlaying)
            return;

        if (_isGuitarMode && !force)
            return;

        if (!_clips.TryGetValue(newState, out var clip) || !clip)
            return;

        if (!force && _currentState == newState)
            return;

        _currentState = newState;

        _videoPlayer.isLooping = true;
        _videoPlayer.clip = clip;
        _videoPlayer.Play();

        UpdateVideoScreenPosition(newState);
    }

    private void UpdateVideoScreenPosition(AnimationState state)
    {
        var bodyVideo = state is AnimationState.GuitarPlaying
            or AnimationState.Walking
            or AnimationState.Idle;

        _bodyVideoScreen.SetActive(bodyVideo);
        _cameraVideoScreen.SetActive(!bodyVideo);
    }

    private enum AnimationState
    {
        GuitarPlaying,
        Walking,
        Idle
    }
}