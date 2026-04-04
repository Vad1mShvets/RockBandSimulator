using System;
using System.Collections.Generic;

public static class QuestManager
{
    private static readonly List<Quest> _activeQuests = new();

    public static IReadOnlyList<Quest> ActiveQuests => _activeQuests;

    public static event Action OnQuestUpdated;
    public static event Action<QuestObjective> OnObjectiveCompleted;
    public static event Action OnQuestFailed;

    public static void Init()
    {
        _activeQuests.Clear();
        OnQuestUpdated = null;
        OnObjectiveCompleted = null;
        OnQuestFailed = null;
    }

    public static void RegisterQuest(Quest quest)
    {
        if (!_activeQuests.Contains(quest))
            _activeQuests.Add(quest);
    }

    public static void UnregisterQuest(Quest quest)
    {
        _activeQuests.Remove(quest);
    }

    public static void NotifyQuestUpdated()
    {
        OnQuestUpdated?.Invoke();
    }

    public static void NotifyObjectiveCompleted(QuestObjective objective)
    {
        OnObjectiveCompleted?.Invoke(objective);
    }

    public static void NotifyQuestFailed()
    {
        OnQuestFailed?.Invoke();
    }

    public static List<QuestObjective> GetActiveObjectives(int max = 2)
    {
        var result = new List<QuestObjective>();

        foreach (var quest in _activeQuests)
        {
            foreach (var obj in quest.Objectives)
            {
                if (obj.State == ObjectiveState.Active)
                {
                    result.Add(obj);
                    if (result.Count >= max)
                        return result;
                }
            }
        }

        return result;
    }
}
