using System.Collections.Generic;
using UnityEngine;

public enum QuestState
{
    NotStarted,
    Active,
    Completed,
    Failed
}

public abstract class Quest : MonoBehaviour
{
    public QuestState State { get; private set; } = QuestState.NotStarted;

    private readonly List<QuestObjective> _objectives = new();
    public IReadOnlyList<QuestObjective> Objectives => _objectives;

    public void StartQuest()
    {
        if (State != QuestState.NotStarted)
            return;

        State = QuestState.Active;
        QuestManager.RegisterQuest(this);
        OnQuestStarted();
        QuestManager.NotifyQuestUpdated();
    }

    protected QuestObjective AddObjective(string description)
    {
        var objective = new QuestObjective(description);
        _objectives.Add(objective);
        QuestManager.NotifyQuestUpdated();
        return objective;
    }

    protected void CompleteObjective(QuestObjective objective)
    {
        if (objective.State != ObjectiveState.Active)
            return;

        objective.Complete();
        QuestManager.NotifyObjectiveCompleted(objective);
    }

    protected void FailQuest()
    {
        if (State != QuestState.Active)
            return;

        State = QuestState.Failed;

        foreach (var obj in _objectives)
            if (obj.State == ObjectiveState.Active)
                obj.Fail();

        QuestManager.NotifyQuestFailed();
    }

    protected void CompleteQuest()
    {
        if (State != QuestState.Active)
            return;

        State = QuestState.Completed;
        QuestManager.UnregisterQuest(this);
        QuestManager.NotifyQuestUpdated();
    }

    protected abstract void OnQuestStarted();
}
