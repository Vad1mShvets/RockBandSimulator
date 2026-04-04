using System;
using UnityEngine;

public class DialogueInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueData _defaultDialogue;
    [SerializeField] private Transform _lookTarget;
    [SerializeField] private NPCActor _npcActor;

    public InteractableTypes Type => InteractableTypes.DialogueNPC;

    private DialogueData _overrideDialogue;
    private Action _onOverrideComplete;
    private bool _defaultUsed;
    private bool _isActive;

    private void OnEnable()
    {
        GameEvents.OnDialogueFinished += OnDialogueFinished;
    }

    private void OnDisable()
    {
        GameEvents.OnDialogueFinished -= OnDialogueFinished;
    }

    /// <summary>
    /// Set a temporary override dialogue (used by quest system).
    /// Auto-clears after the dialogue finishes and fires the callback.
    /// </summary>
    public void SetOverrideDialogue(DialogueData data, Action onComplete = null)
    {
        _overrideDialogue = data;
        _onOverrideComplete = onComplete;
    }

    public void ClearOverrideDialogue()
    {
        _overrideDialogue = null;
        _onOverrideComplete = null;
    }

    public void Focus() { }

    public void UnFocus() { }

    public void Interact()
    {
        if (DialogueManager.IsInDialogue)
            return;

        var dialogue = _overrideDialogue != null ? _overrideDialogue : _defaultDialogue;

        if (dialogue == null)
            return;

        // Default dialogue is one-shot
        if (_overrideDialogue == null && _defaultUsed)
            return;

        if (_npcActor != null)
            _npcActor.PauseForDialogue();

        _isActive = true;

        DialogueUI.Instance.StartDialogue(dialogue, transform, _lookTarget, dialogue.GetInstanceID());

        if (_overrideDialogue == null)
            _defaultUsed = true;
    }

    private void OnDialogueFinished()
    {
        if (!_isActive)
            return;

        _isActive = false;

        if (_npcActor != null)
            _npcActor.ResumeFromDialogue();

        if (_overrideDialogue != null)
        {
            var callback = _onOverrideComplete;
            ClearOverrideDialogue();
            callback?.Invoke();
        }
    }
}
