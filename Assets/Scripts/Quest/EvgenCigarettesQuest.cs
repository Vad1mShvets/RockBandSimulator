using UnityEngine;

public class EvgenCigarettesQuest : Quest
{
    [Header("NPCs")]
    [SerializeField] private DialogueInteractable _evgenDialogue;
    [SerializeField] private DialogueInteractable _artemDialogue;
    [SerializeField] private NPCActor _artemActor;
    [SerializeField] private NPCActor _shevActor;

    [Header("Dialogues")]
    [SerializeField] private DialogueData _evgenQuestDialogue;
    [SerializeField] private DialogueData _artemDialogue1;
    [SerializeField] private DialogueData _artemDialogue2;

    [Header("Reward")]
    [SerializeField] private GameObject _cigsPrefab;
    [SerializeField] private Transform _playerTransform;

    private QuestObjective _goToShop;
    private QuestObjective _talkToArtem1;
    private QuestObjective _beatShev;
    private QuestObjective _talkToArtem2;
    private QuestObjective _pickUpCigs;

    private void Awake()
    {
        // Set Evgen's dialogue to quest version — when it finishes, quest starts
        _evgenDialogue.SetOverrideDialogue(_evgenQuestDialogue, OnEvgenDialogueFinished);
    }

    private void OnEnable()
    {
        GameEvents.OnLocationChanged += OnLocationChanged;
        GameEvents.OnNPCDied += OnNPCDied;
        GameEvents.OnNPCDamaged += OnNPCDamaged;
        GameEvents.OnInventoryItemUsed += OnItemPickedUp;
    }

    private void OnDisable()
    {
        GameEvents.OnLocationChanged -= OnLocationChanged;
        GameEvents.OnNPCDied -= OnNPCDied;
        GameEvents.OnNPCDamaged -= OnNPCDamaged;
        GameEvents.OnInventoryItemUsed -= OnItemPickedUp;
    }

    // === STEP 1: Evgen dialogue finishes → quest starts ===

    private void OnEvgenDialogueFinished()
    {
        StartQuest();
    }

    protected override void OnQuestStarted()
    {
        _goToShop = AddObjective("Go to the Shop");
        _talkToArtem1 = AddObjective("Talk to Artem");

        _artemDialogue.SetOverrideDialogue(_artemDialogue1, OnArtemDialogue1Finished);
    }

    // === STEP 2: Reach Shop ===

    private void OnLocationChanged(LocationType location)
    {
        if (State != QuestState.Active)
            return;

        if (location == LocationType.Shop && _goToShop.State == ObjectiveState.Active)
            CompleteObjective(_goToShop);
    }

    // === STEP 3: Talk to Artem (first time) ===

    private void OnArtemDialogue1Finished()
    {
        CompleteObjective(_talkToArtem1);
        _beatShev = AddObjective("Beat up The Guy outside");
    }

    // === STEP 4: Beat Shev ===

    private void OnNPCDied(NPCActor npc)
    {
        if (State != QuestState.Active)
            return;

        if (npc == _shevActor && _beatShev != null && _beatShev.State == ObjectiveState.Active)
        {
            CompleteObjective(_beatShev);
            _talkToArtem2 = AddObjective("Talk to Artem");
            _artemDialogue.SetOverrideDialogue(_artemDialogue2, OnArtemDialogue2Finished);
        }
    }

    // === STEP 5: Talk to Artem (second time) → spawn cigs ===

    private void OnArtemDialogue2Finished()
    {
        CompleteObjective(_talkToArtem2);

        SpawnCigs();
        _pickUpCigs = AddObjective("Pick up the cigarettes");
    }

    private void SpawnCigs()
    {
        var spawnPos = _playerTransform.position + _playerTransform.forward * 1.5f;
        spawnPos.y = _playerTransform.position.y;

        Instantiate(_cigsPrefab, spawnPos, Quaternion.identity);
    }

    // === STEP 6: Pick up cigs → quest complete ===

    private void OnItemPickedUp(InteractableTypes item)
    {
        if (State != QuestState.Active)
            return;

        if (item == InteractableTypes.Cigs && _pickUpCigs != null && _pickUpCigs.State == ObjectiveState.Active)
        {
            CompleteObjective(_pickUpCigs);
            CompleteQuest();
        }
    }

    // === FAILURE: Attack Artem ===

    private void OnNPCDamaged(NPCActor npc)
    {
        if (State != QuestState.Active)
            return;

        if (npc == _artemActor)
            FailQuest();
    }
}
