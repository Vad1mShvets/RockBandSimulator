using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject _root;
    [SerializeField] private TMP_Text _text;

    [Header("Settings")]
    [SerializeField] private float _letterDelay = 0.03f;
    [SerializeField] private float _cameraLookDuration = 0.4f;

    private Coroutine _dialogueCoroutine;
    private Transform _npcTransform;
    private int _dialogueId;
    private bool _isTyping;
    private bool _skipRequested;
    private bool _advanceRequested;

    private void Awake()
    {
        Instance = this;
        _root.SetActive(false);
    }

    public void StartDialogue(DialogueData data, Transform npcTransform, Transform lookTarget, int dialogueId)
    {
        if (DialogueManager.IsInDialogue)
            return;

        _dialogueId = dialogueId;
        _npcTransform = npcTransform;
        _skipRequested = false;
        _advanceRequested = false;

        // Block player
        PlayerStateController.TryEnterBusy();
        PlayerCameraLook.Instance.SetLookEnabled(false);
        DialogueManager.SetInDialogue(true);

        // Face NPC toward player camera
        var camPos = PlayerCameraLook.Instance.transform.position;
        var dirToPlayer = camPos - npcTransform.position;
        dirToPlayer.y = 0f;

        if (dirToPlayer.sqrMagnitude > 0.001f)
            npcTransform.rotation = Quaternion.LookRotation(dirToPlayer);

        // Subscribe to E key (Gameplay map stays active)
        InputReader.Instance.Interact += OnInteract;

        GameEvents.OnDialogueStarted?.Invoke();

        _dialogueCoroutine = StartCoroutine(DialogueRoutine(data, lookTarget));
    }

    private void OnInteract()
    {
        if (_isTyping)
            _skipRequested = true;
        else
            _advanceRequested = true;
    }

    private IEnumerator DialogueRoutine(DialogueData data, Transform lookTarget)
    {
        var targetPos = lookTarget ? lookTarget.position : _npcTransform.position + Vector3.up * 1.6f;
        yield return PlayerCameraLook.Instance.SmoothLookAt(targetPos, _cameraLookDuration);

        _root.SetActive(true);

        for (var i = 0; i < data.Lines.Length; i++)
        {
            yield return TypeLine(data.Lines[i]);
            
            _advanceRequested = false;

            while (!_advanceRequested)
            {
                if (!DialogueManager.IsInDialogue)
                    yield break;

                yield return null;
            }
        }

        EndDialogue(true);
    }

    private IEnumerator TypeLine(string line)
    {
        _isTyping = true;
        _skipRequested = false;
        _text.text = "";

        for (var i = 0; i < line.Length; i++)
        {
            if (_skipRequested || !DialogueManager.IsInDialogue)
            {
                _text.text = line;
                break;
            }

            _text.text += line[i];
            yield return new WaitForSeconds(_letterDelay);
        }

        _isTyping = false;
    }

    private void EndDialogue(bool completed)
    {
        if (!DialogueManager.IsInDialogue)
            return;

        DialogueManager.SetInDialogue(false);

        if (completed)
            DialogueManager.MarkCompleted(_dialogueId);

        if (_dialogueCoroutine != null)
        {
            StopCoroutine(_dialogueCoroutine);
            _dialogueCoroutine = null;
        }

        _root.SetActive(false);
        _text.text = "";

        InputReader.Instance.Interact -= OnInteract;

        PlayerCameraLook.Instance.SetLookEnabled(true);
        PlayerStateController.ExitBusy();

        GameEvents.OnDialogueFinished?.Invoke();
    }
}
