using System.Collections;
using UnityEngine;

public class FadeScreenUI : MonoBehaviour
{
    public static FadeScreenUI Instance { get; private set; }

    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _duration = 0.3f;

    private Coroutine _current;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public Coroutine FadeIn() => StartFade(1f);
    public Coroutine FadeOut() => StartFade(0f);

    private Coroutine StartFade(float target)
    {
        if (_current != null)
            StopCoroutine(_current);

        _current = StartCoroutine(FadeRoutine(target));
        return _current;
    }

    private IEnumerator FadeRoutine(float target)
    {
        var start = _canvasGroup.alpha;
        var time = 0f;

        while (time < _duration)
        {
            time += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(start, target, time / _duration);
            yield return null;
        }

        _canvasGroup.alpha = target;
        _current = null;
    }
}