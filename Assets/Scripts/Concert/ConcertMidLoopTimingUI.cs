using UnityEngine;
using UnityEngine.UI;

public class ConcertMidLoopTimingUI : MonoBehaviour
{
    [SerializeField] private GameObject _buttonsPanel;
    [SerializeField] private GameObject _buttonA;
    [SerializeField] private GameObject _buttonB;
    [SerializeField] private GameObject _buttonC;
    [SerializeField] private GameObject _buttonD;
    
    [Space] [SerializeField] private Image _timingFillImage;

    private void OnEnable()
    {
        GameEvents.OnMidLoopTimingStarted += OnMidLoopTimingStarted;
        GameEvents.OnMidLoopTimingEnd += OnMidLoopTimingEnd;
        GameEvents.OnMidLoopTimingUpdate += OnMidLoopTimingUpdate;
    }

    private void OnMidLoopTimingStarted(LoopType loopType)
    {
        DisableAllButtons();
        EnableButtonByLoopType(loopType);
        
        _buttonsPanel.SetActive(true);
        _timingFillImage.fillAmount = 1;
    }

    private void OnMidLoopTimingEnd()
    {
        _buttonsPanel.SetActive(false);
    }

    private void OnMidLoopTimingUpdate(float value)
    {
        _timingFillImage.fillAmount = 1 - value;
    }
    
    private void DisableAllButtons()
    {
        _buttonA.SetActive(false);
        _buttonB.SetActive(false);
        _buttonC.SetActive(false);
        _buttonD.SetActive(false);
    }

    private void EnableButtonByLoopType(LoopType loopType)
    {
        switch (loopType)
        {
            case LoopType.A: _buttonA.SetActive(true); break;
            case LoopType.B: _buttonB.SetActive(true); break;
            case LoopType.C: _buttonC.SetActive(true); break;
            case LoopType.D: _buttonD.SetActive(true); break;
        }
    }
}
