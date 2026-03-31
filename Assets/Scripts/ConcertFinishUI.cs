using TMPro;
using UnityEngine;

public class ConcertFinishUI : MonoBehaviour
{
    [SerializeField] private GameObject _body;

    [SerializeField] private TextMeshProUGUI _perfectTimingsText;
    [SerializeField] private TextMeshProUGUI _missedTimingsText;
    [SerializeField] private TextMeshProUGUI _lastNoteBonusText;

    [SerializeField] private ReputationBarUI _reputationBarUI;

    private void Awake()
    {
        GameEvents.OnLastNoteBonusPressed += _ => OnConcertFinished();
        _body.SetActive(false);
    }
    private void OnConcertFinished()
    {
        InputReader.Instance.Submit += OnCloseButton;

        _body.SetActive(true);

        InputStateController.Instance.SetUI();
        
        _perfectTimingsText.text = $"Perfect timings: <size=60><color=green>+{ConcertScoreManager.PositiveScore}</color></size>";
        _missedTimingsText.text = $"Missed timings: <size=60><color=red>-{ConcertScoreManager.NegativeScore}</color></size>";
        _lastNoteBonusText.text = $"Last note bonus <size=60><color=green>+{ConcertScoreManager.LastNoteBonus}</color></size>";
    }

    private void OnCloseButton()
    {
        InputReader.Instance.Submit -= OnCloseButton;

        _body.SetActive(false);
        InputStateController.Instance.SetGameplay();
        
        GameEvents.OnConcertFinishScreenClosed?.Invoke();
    }
}