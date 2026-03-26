using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class HandsAnimationController : MonoBehaviour
{
    private enum OverrideState { None, Item, Attack }

    [SerializeField] private VideoPlayer _videoPlayer;

    [Header("Streaming")]
    [SerializeField] private string _folder = "Hands";

    [Header("Base Clips")]
    [SerializeField] private string _idleClip;
    [SerializeField] private string _walkClip;

    [Header("Mode Clips")]
    [SerializeField] private string _guitarClip;

    [Header("Override Clips")]
    [SerializeField] private string _beerClip;
    [SerializeField] private string _cigsClip;
    [SerializeField] private string[] _attackClips;

    [Header("Screens")]
    [SerializeField] private GameObject _bodyVideoScreen;
    [SerializeField] private GameObject _cameraVideoScreen;

    private OverrideState _override = OverrideState.None;

    private bool _isWalking;
    private bool _isConcertActive;

    private string _currentClip;

    private string BasePath => Path.Combine(Application.streamingAssetsPath, _folder);

    // =========================================================
    // LIFECYCLE
    // =========================================================

    private void Awake()
    {
        _videoPlayer.playOnAwake = false;
        _videoPlayer.waitForFirstFrame = true;
        _videoPlayer.source = VideoSource.Url;

        _videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnEnable()
    {
        GameEvents.OnWalkingStart += OnWalkingStart;
        GameEvents.OnWalkingEnd += OnWalkingEnd;

        GameEvents.OnCallingConcertStart += OnConcertStart;
        GameEvents.OnConcertFinished += OnConcertFinished;

        GameEvents.OnInventoryItemUsed += OnItemUsed;
        GameEvents.OnAttack += PlayAttack;
    }

    private void OnDisable()
    {
        GameEvents.OnWalkingStart -= OnWalkingStart;
        GameEvents.OnWalkingEnd -= OnWalkingEnd;

        GameEvents.OnCallingConcertStart -= OnConcertStart;
        GameEvents.OnConcertFinished -= OnConcertFinished;

        GameEvents.OnInventoryItemUsed -= OnItemUsed;
        GameEvents.OnAttack -= PlayAttack;
    }

    private void Start()
    {
        Resolve();
    }

    private void OnDestroy()
    {
        PlayerStateController.ExitBusy();
    }

    // =========================================================
    // CORE RESOLVE
    // =========================================================

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

        // 🎸 приоритет №1 — концерт
        if (_isConcertActive)
        {
            PlayLoop(_guitarClip);
            SetBodyScreen();
            return;
        }

        // 🚶 обычные состояния
        if (_isWalking)
        {
            PlayLoop(_walkClip);
        }
        else
        {
            PlayLoop(_idleClip);
        }

        SetBodyScreen();
    }

    // =========================================================
    // PLAY
    // =========================================================

    private void PlayLoop(string file)
    {
        if (string.IsNullOrEmpty(file) || _currentClip == file)
            return;

        _currentClip = file;

        _videoPlayer.Stop();
        _videoPlayer.isLooping = true;

        PlayVideo(file);
    }

    private void PlayOverride(string file)
    {
        _currentClip = file;

        _videoPlayer.Stop();
        _videoPlayer.isLooping = false;

        PlayVideo(file);

        SetCameraScreen();
    }

    private void PlayVideo(string fileName)
    {
        fileName = Normalize(fileName);

        var fullPath = Path.Combine(BasePath, fileName);

        _videoPlayer.url = fullPath;
        
        _videoPlayer.frame = 0;

        _videoPlayer.prepareCompleted -= OnPrepared;
        _videoPlayer.prepareCompleted += OnPrepared;

        _videoPlayer.Prepare();
    }

    private void OnPrepared(VideoPlayer player)
    {
        _videoPlayer.prepareCompleted -= OnPrepared;
        _videoPlayer.Play();
    }

    // =========================================================
    // OVERRIDE
    // =========================================================

    private void PlayAttack()
    {
        if (_attackClips.Length == 0) return;
        if (_override != OverrideState.None) return;
        if (!PlayerStateController.TryEnterBusy()) return;

        _override = OverrideState.Attack;

        string clip = _attackClips[Random.Range(0, _attackClips.Length)];
        PlayOverride(clip);
    }

    private void OnItemUsed(InteractableTypes item)
    {
        if (_override != OverrideState.None) return;
        if (!PlayerStateController.TryEnterBusy()) return;

        string clip = item switch
        {
            InteractableTypes.Beer => _beerClip,
            InteractableTypes.Cigs => _cigsClip,
            _ => null
        };

        if (string.IsNullOrEmpty(clip))
        {
            PlayerStateController.ExitBusy();
            return;
        }

        _override = OverrideState.Item;
        PlayOverride(clip);
    }

    private void OnVideoFinished(VideoPlayer player)
    {
        if (_override == OverrideState.None)
            return;

        _override = OverrideState.None;

        PlayerStateController.ExitBusy();

        // 🔥 всегда возвращаемся в актуальный режим
        Resolve();
    }

    // =========================================================
    // EVENTS
    // =========================================================

    private void OnWalkingStart()
    {
        _isWalking = true;
        TryResolve();
    }

    private void OnWalkingEnd()
    {
        _isWalking = false;
        TryResolve();
    }

    private void OnConcertStart()
    {
        _isConcertActive = true;
        Resolve();
    }

    private void OnConcertFinished()
    {
        _isConcertActive = false;
        Resolve();
    }

    // =========================================================
    // UTILS
    // =========================================================

    private void SetBodyScreen()
    {
        _bodyVideoScreen.SetActive(true);
        _cameraVideoScreen.SetActive(false);
    }

    private void SetCameraScreen()
    {
        _bodyVideoScreen.SetActive(false);
        _cameraVideoScreen.SetActive(true);
    }

    private string Normalize(string file)
    {
        return file.EndsWith(".mp4") ? file : file + ".mp4";
    }
}