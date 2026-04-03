using UnityEngine;

public class DialogueInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private DialogueData _dialogueData;
    [SerializeField] private Transform _lookTarget;
    [SerializeField] private NPCActor _npcActor;

    public InteractableTypes Type => InteractableTypes.DialogueNPC;

    private bool _used;

    private void OnEnable()
    {
        GameEvents.OnDialogueFinished += OnDialogueFinished;
    }

    private void OnDisable()
    {
        GameEvents.OnDialogueFinished -= OnDialogueFinished;
    }

    public void Focus() { }

    public void UnFocus() { }

    public void Interact()
    {
        if (_used)
            return;

        if (DialogueManager.IsCompleted(GetInstanceID()))
        {
            _used = true;
            return;
        }
        
        if (_npcActor != null)
            _npcActor.PauseForDialogue();

        DialogueUI.Instance.StartDialogue(_dialogueData, transform, _lookTarget, GetInstanceID());
        _used = true;
    }

    private void OnDialogueFinished()
    {
        if (_npcActor != null)
            _npcActor.ResumeFromDialogue();
    }
}
