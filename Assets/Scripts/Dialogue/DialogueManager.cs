using System.Collections.Generic;

public static class DialogueManager
{
    private static readonly HashSet<int> _completedDialogues = new();

    public static bool IsInDialogue { get; private set; }

    public static void Init()
    {
        _completedDialogues.Clear();
        IsInDialogue = false;
    }

    public static bool IsCompleted(int dialogueId)
    {
        return _completedDialogues.Contains(dialogueId);
    }

    public static void MarkCompleted(int dialogueId)
    {
        _completedDialogues.Add(dialogueId);
    }

    public static void SetInDialogue(bool value)
    {
        IsInDialogue = value;
    }
}
