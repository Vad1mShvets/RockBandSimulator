using UnityEngine;
using UnityEngine.UI;

public class ConcertMidLoopTimingUI : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Image _timingFillImage;

    private void OnEnable()
    {
        GameEvents.OnMidLoopTimingStarted += OnMidLoopTimingStarted;
        GameEvents.OnMidLoopTimingEnd += OnMidLoopTimingEnd;
        GameEvents.OnMidLoopTimingUpdate += OnMidLoopTimingUpdate;
    }

    private void OnMidLoopTimingStarted()
    {
        _panel.SetActive(true);
        _timingFillImage.fillAmount = 1;
    }

    private void OnMidLoopTimingEnd()
    {
        _panel.SetActive(false);
    }

    private void OnMidLoopTimingUpdate(float value)
    {
        _timingFillImage.fillAmount = 1 - value;
    }
}
