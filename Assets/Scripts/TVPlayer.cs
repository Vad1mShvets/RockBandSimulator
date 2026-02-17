using UnityEngine;
using UnityEngine.Video;

public class TVPlayer : MonoBehaviour, IInteractable
{
    [Header("Setup")]
    [SerializeField] private bool startEnabled;
    [SerializeField] private VideoPlayer _player;
    [SerializeField] private VideoClip[] _videos;
    [SerializeField] private VideoClip _transitionVideo;
    [SerializeField] private AudioSource _audioSource;

    private int _index;
    private bool _isOn;
    private bool _playingTransition;
    private bool _turningOff;

    private void Awake()
    {
        _player.isLooping = false;
        _player.playOnAwake = false;

        _player.audioOutputMode = VideoAudioOutputMode.AudioSource;
        _player.EnableAudioTrack(0, true);
        _player.SetTargetAudioSource(0, _audioSource);

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
        // закончился transition
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

        // закончился обычный канал → всегда переход
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

        var lastIndex = _index;
        
        while (_index.Equals(lastIndex))
            _index = Random.Range(0, _videos.Length);
        
        _player.clip = _videos[_index];
        _player.Play();
        _audioSource.Play();
    }

    private void PlayTransition()
    {
        _playingTransition = true;
        _player.clip = _transitionVideo;
        _player.Play();
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

    public InteractableTypes Type => InteractableTypes.TV;
    public void Focus() { }
    public void UnFocus() { }
}
