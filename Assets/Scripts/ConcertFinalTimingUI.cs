using UnityEngine;

public class ConcertFinalTimingUI : MonoBehaviour
{
    [SerializeField] private FinalTimingBarUI _finalTimingBarUI;

    private void Awake()
    {
        _finalTimingBarUI.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GameEvents.OnNewLoopStart += OnNewLoopStart;
    }

    private void OnDisable()
    {
        GameEvents.OnNewLoopStart -= OnNewLoopStart;
    }

    private void OnNewLoopStart(LoopType loopType)
    {
        if (loopType is LoopType.D)
            ShowFinalTimingBar();
    }
    
    private void ShowFinalTimingBar()
    {
        _finalTimingBarUI.gameObject.SetActive(true);
    }
}
