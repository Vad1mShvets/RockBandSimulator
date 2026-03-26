using System.IO;
using UnityEngine;
using UnityEngine.Video;

public class TVPlayer : MonoBehaviour, IInteractable
{
    [Header("Setup")]
    [SerializeField] private bool startEnabled;
    [SerializeField] private VideoPlayer _player;
    [SerializeField] private AudioSource _audioSource;

    [Header("Streaming Assets")]
    [SerializeField] private string[] _videos; // например: "IMG_2240.mp4"
    [SerializeField] private string _transitionVideo; // например: "TransitionTV.mp4"
    [SerializeField] private string _folder = "TV"; // StreamingAssets/TV

    private int[] _shuffled;
    private int _queueIndex;
    
    private bool _isOn;
    private bool _playingTransition;
    private bool _turningOff;

    private string BasePath => Path.Combine(Application.streamingAssetsPath, _folder);

    private void Awake()
    {
        _player.isLooping = false;
        _player.playOnAwake = false;

        _player.audioOutputMode = VideoAudioOutputMode.AudioSource;
        _player.EnableAudioTrack(0, true);
        _player.SetTargetAudioSource(0, _audioSource);

        _player.source = VideoSource.Url;

        _player.loopPointReached += OnVideoEnd;
    }

    private void Start()
    {
        _isOn = startEnabled;

        if (_isOn)
            StartTurnOn();
        else
            StopAll();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (_playingTransition)
        {
            _playingTransition = false;

            if (_turningOff)
            {
                StopAll();
                return;
            }

            PlayRandomChannel();
            return;
        }

        if (_isOn)
            PlayTransition();
    }

    // ───────── ACTIONS ─────────

    private void StartTurnOn()
    {
        _isOn = true;
        _turningOff = false;
        PlayTransition();
    }

    private void StartTurnOff()
    {
        _turningOff = true;
        PlayTransition();
    }

    private void PlayRandomChannel()
    {
        if (_videos.Length == 0) return;
        
        if (_shuffled == null || _queueIndex >= _shuffled.Length)
        {
            ShuffleVideos();
        }

        var videoIndex = _shuffled[_queueIndex];
        _queueIndex++;

        PlayVideo(_videos[videoIndex]);
    }

    private void PlayTransition()
    {
        _playingTransition = true;
        PlayVideo(_transitionVideo);
    }

    private void PlayVideo(string fileName)
    {
        string fullPath = Path.Combine(BasePath, fileName);

        _player.url = fullPath;
        _player.Prepare();

        _player.prepareCompleted -= OnPrepared;
        _player.prepareCompleted += OnPrepared;
    }

    private void OnPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnPrepared;
        vp.Play();
        _audioSource.Play();
    }

    private void StopAll()
    {
        _isOn = false;
        _turningOff = false;
        _playingTransition = false;

        _player.Stop();
        _audioSource.Stop();
    }

    public void Interact()
    {
        if (_isOn)
            StartTurnOff();
        else
            StartTurnOn();
    }

    private void ShuffleVideos()
    {
        var n = _videos.Length;

        _shuffled = new int[n];

        for (var i = 0; i < n; i++)
            _shuffled[i] = i;

        for (var i = n - 1; i > 0; i--)
        {
            var j = Random.Range(0, i + 1);
            (_shuffled[i], _shuffled[j]) = (_shuffled[j], _shuffled[i]);
        }

        _queueIndex = 0;
    }

    public InteractableTypes Type => InteractableTypes.TV;
    public void Focus() { }
    public void UnFocus() { }
}