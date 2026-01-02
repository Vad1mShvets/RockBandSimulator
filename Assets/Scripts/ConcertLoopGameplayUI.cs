using UnityEngine;
using UnityEngine.UI;

public class ConcertLoopGameplayUI : MonoBehaviour
{
    [SerializeField] private GameObject _buttonsPanel;
    [SerializeField] private Image _timingFillImage;
    [SerializeField] private Image _perfectTimingFillImage;

    private void OnEnable()
    {
        _buttonsPanel.SetActive(false);

        GameEvents.OnCallingConcertStart += StartLoopChooseTimer;
        GameEvents.OnLoopChooseTimerStart += StartLoopChooseTimer;
        GameEvents.OnLoopChooseTimerUpdate += UpdateLoopChooseTimer;
        GameEvents.OnLoopChooseTimerEnd += EndLoopChooseTimer;
        GameEvents.OnCallingConcertStart += SetStartState;
        GameEvents.OnCallingRehearsalStart += SetStartState;
        GameEvents.OnConcertStarted += SetupPerfectTimingFill;
    }

    private void OnDisable()
    {
        GameEvents.OnCallingConcertStart -= StartLoopChooseTimer;
        GameEvents.OnLoopChooseTimerStart -= StartLoopChooseTimer;
        GameEvents.OnLoopChooseTimerUpdate -= UpdateLoopChooseTimer;
        GameEvents.OnLoopChooseTimerEnd -= EndLoopChooseTimer;
        GameEvents.OnCallingConcertStart -= SetStartState;
        GameEvents.OnCallingRehearsalStart -= SetStartState;
        GameEvents.OnConcertStarted -= SetupPerfectTimingFill;
    }

    private void SetStartState()
    {
        _buttonsPanel.SetActive(true);
        _timingFillImage.gameObject.SetActive(false);
        _timingFillImage.fillAmount = 0f;
    }

    private void StartLoopChooseTimer()
    {
        _timingFillImage.fillAmount = 1f;
        _buttonsPanel.SetActive(true);
        _timingFillImage.gameObject.SetActive(true);
    }

    private void UpdateLoopChooseTimer(float value)
    {
        _timingFillImage.fillAmount = value;
    }

    private void EndLoopChooseTimer()
    {
        _buttonsPanel.SetActive(false);
    }

    private void SetupPerfectTimingFill(ConcertData data)
    {
        _perfectTimingFillImage.fillAmount = data.PerfectTimingSeconds /  data.ChooseWindowSeconds;
    }
}