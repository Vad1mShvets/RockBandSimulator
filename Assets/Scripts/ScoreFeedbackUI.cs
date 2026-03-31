using TMPro;
using UnityEngine;

public class ScoreFeedbackUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _addScoreText;
    [SerializeField] private Animation _animation;

    private void OnEnable()
    {
        GameEvents.OnConcertAddScore += ShowScoreFeedback;
        GameEvents.OnConcertRemoveScore += ShowScoreFeedback;
    }

    private void OnDisable()
    {
        GameEvents.OnConcertAddScore -= ShowScoreFeedback;
        GameEvents.OnConcertRemoveScore -= ShowScoreFeedback;
    }

    private void ShowScoreFeedback(int score)
    {
        var isScorePositive = score >= 0;
        var sign = score < 0 ? "-" : "+";
        
        _addScoreText.text = $"{sign}{Mathf.Abs(score)}";
        _addScoreText.color = isScorePositive ? Color.green : Color.red;
        
        _animation.Play();
    }
}