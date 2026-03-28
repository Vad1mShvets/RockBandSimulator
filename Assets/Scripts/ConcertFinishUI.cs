using TMPro;
using UnityEngine;

public class ConcertFinishUI : MonoBehaviour
{
    [SerializeField] private GameObject _body;

    [SerializeField] private TextMeshProUGUI _perfectTimingsText;
    [SerializeField] private TextMeshProUGUI _missedTimingsText;

    [SerializeField] private ReputationBarUI _reputationBarUI;

    private void OnEnable()
    {
        GameEvents.OnConcertFinished += OnConcertFinished;
    }

    private void OnDisable()
    {
        GameEvents.OnConcertFinished -= OnConcertFinished;
    }

    private void OnConcertFinished()
    {
        InputReader.Instance.Submit += OnCloseButton;

        _body.SetActive(true);

        InputStateController.Instance.SetUI();

        _perfectTimingsText.text = $"Perfect timings: {ConcertScoreManager.PositiveScore}";
        _missedTimingsText.text = $"Missed timings: {ConcertScoreManager.NegativeScore}";
    }

    private void OnCloseButton()
    {
        InputReader.Instance.Submit -= OnCloseButton;

        _body.SetActive(false);
        InputStateController.Instance.SetGameplay();
    }
}