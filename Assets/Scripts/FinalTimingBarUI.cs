using System.Collections;
using UnityEngine;

public class FinalTimingBarUI : MonoBehaviour
{
    private const float PERFECT_ZONE_SIZE = 0.2f;

    [SerializeField] private RectTransform _barBack;
    [SerializeField] private RectTransform _perfectZone;
    [SerializeField] private RectTransform _arrow;
    [SerializeField] private float _arrowSpeed = 1f;

    private float _arrowStartX => -_barBack.sizeDelta.x / 2;
    private float _arrowEndX => -_arrowStartX;
    private float _timer;

    private bool _stopped;

    private void OnEnable()
    {
        Canvas.ForceUpdateCanvases();
    
        _perfectZone.sizeDelta = new Vector2(PERFECT_ZONE_SIZE * _barBack.sizeDelta.x, _perfectZone.sizeDelta.y);
        _arrow.localPosition = new Vector2(_arrowStartX, 0);
        _timer = 0f;
        _stopped = false;
    
        InputReader.Instance.DLoop += OnInteract;
    }

    private void OnDisable()
    {
        InputReader.Instance.DLoop -= OnInteract;
    }

    private void Update()
    {
        if (_stopped)
            return;
        
        _timer += Time.deltaTime * _arrowSpeed;
        
        var t = Mathf.PingPong(_timer, 1f);

        _arrow.localPosition = new Vector2(Mathf.Lerp(_arrowStartX, _arrowEndX, t), 0);
    }

    private void OnInteract()
    {
        _stopped = true;

        var halfPerfectZone = _perfectZone.sizeDelta.x / 2f;
        var arrowX = _arrow.localPosition.x;
        var isPerfectTiming = Mathf.Abs(arrowX) <= halfPerfectZone;

        StartCoroutine(DelayedTimingResult(isPerfectTiming));
    }

    private IEnumerator DelayedTimingResult(bool isPerfectTiming)
    {
        yield return new WaitForSeconds(0.5f);

        gameObject.SetActive(false);
        
        GameEvents.OnLastNoteBonusPressed?.Invoke(isPerfectTiming);
    }
}