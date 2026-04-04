using System.Collections;
using TMPro;
using UnityEngine;

public class QuestObjectiveUI : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private TMP_Text _line0;
    [SerializeField] private TMP_Text _line1;
    [SerializeField] private float _completedDisplayTime = 1.5f;

    private static readonly Color ActiveColor = Color.white;
    private static readonly Color CompletedColor = Color.green;
    private static readonly Color FailedColor = Color.red;

    private QuestObjective _shown0;
    private QuestObjective _shown1;

    private void Awake()
    {
        HideObjectives();
    }

    private void OnEnable()
    {
        QuestManager.OnInit += HideObjectives;
        QuestManager.OnQuestUpdated += Refresh;
        QuestManager.OnObjectiveCompleted += OnObjectiveCompleted;
        QuestManager.OnQuestFailed += OnQuestFailed;
    }

    private void OnDisable()
    {
        QuestManager.OnInit -= HideObjectives;
        QuestManager.OnQuestUpdated -= Refresh;
        QuestManager.OnObjectiveCompleted -= OnObjectiveCompleted;
        QuestManager.OnQuestFailed -= OnQuestFailed;
    }

    private void Refresh()
    {
        var active = QuestManager.GetActiveObjectives(2);

        _shown0 = active.Count > 0 ? active[0] : null;
        _shown1 = active.Count > 1 ? active[1] : null;

        SetLine(_line0, _shown0, ActiveColor);
        SetLine(_line1, _shown1, ActiveColor);

        _root.SetActive(_shown0 != null);
    }

    private void SetLine(TMP_Text line, QuestObjective obj, Color color)
    {
        if (obj == null)
        {
            line.gameObject.SetActive(false);
            return;
        }

        line.gameObject.SetActive(true);
        line.text = "- " + obj.Description;
        line.color = color;
    }

    private void OnObjectiveCompleted(QuestObjective obj)
    {
        // Find which line shows this objective and turn it green
        if (_shown0 == obj)
            _line0.color = CompletedColor;
        else if (_shown1 == obj)
            _line1.color = CompletedColor;

        StartCoroutine(RefreshAfterDelay());
    }

    private void OnQuestFailed()
    {
        // Turn all visible lines red
        if (_shown0 != null && _shown0.State == ObjectiveState.Failed)
            _line0.color = FailedColor;
        if (_shown1 != null && _shown1.State == ObjectiveState.Failed)
            _line1.color = FailedColor;

        StartCoroutine(RefreshAfterDelay());
    }

    private IEnumerator RefreshAfterDelay()
    {
        yield return new WaitForSeconds(_completedDisplayTime);
        Refresh();
    }

    private void HideObjectives()
    {
        _root.SetActive(false);
    }
}
