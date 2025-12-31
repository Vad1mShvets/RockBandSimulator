using System.Collections;
using UnityEngine;

public class ConcertLights : MonoBehaviour
{
    [SerializeField] private Light _light;
    [SerializeField] private Color _perfectTimingColor;
    [SerializeField] private Color _badTimingColor;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private float _flashDuration = 0.5f;

    private Coroutine _lightRoutine;

    private void OnEnable()
    {
        GameEvents.OnLoopTimingPressed += OnTimingPressed;
    }

    private void OnDisable()
    {
        GameEvents.OnLoopTimingPressed -= OnTimingPressed;
    }

    private void OnTimingPressed(ConcertMusicManager.TimingState timing)
    {
        var color = timing == ConcertMusicManager.TimingState.Perfect
            ? _perfectTimingColor
            : _badTimingColor;

        if (_lightRoutine != null)
            StopCoroutine(_lightRoutine);

        _lightRoutine = StartCoroutine(LightAnimation(color));
    }

    private IEnumerator LightAnimation(Color lightColor)
    {
        _light.color = lightColor;
        _light.enabled = true;

        yield return new WaitForSeconds(_flashDuration);

        _light.color = _defaultColor;
        _lightRoutine = null;
    }
}