public enum ObjectiveState
{
    Active,
    Completed,
    Failed
}

public class QuestObjective
{
    public string Description { get; }
    public ObjectiveState State { get; private set; }

    public QuestObjective(string description)
    {
        Description = description;
        State = ObjectiveState.Active;
    }

    public void Complete() => State = ObjectiveState.Completed;
    public void Fail() => State = ObjectiveState.Failed;
}
