# Rock Band Simulator — Project Overview

## What Is This

First-person rhythm game about a garage rock band. Two gameplay modes: **Exploration** (walk around, interact with objects and NPCs) and **Concert** (timing-based rhythm gameplay with score and chaos mechanics).

Unity URP project. C#. New Input System. 3 scenes loaded additively: Bootstrap → UI → Gameplay.

---

## Architecture

Everything talks through a static event bus (`GameEvents`). Managers are static classes. Singletons for MonoBehaviours. Data lives in ScriptableObjects. NPC behaviour is activity-based (queue of commands).

Key patterns used throughout:

- **Static managers:** ChaosManager, ConcertScoreManager, ReputationManager, MoneyManager, InventoryManager, SoundsManager
- **Singleton MonoBehaviours:** GameService, InputReader, InputStateController, FadeScreenUI, TravelSystem
- **Event bus:** GameEvents — ~30 static Actions, all inter-system communication
- **ScriptableObjects:** InteractableData, LocationData, AudioConfig, ConcertAudioData
- **Interfaces:** IInteractable (interact system), IDamageable (combat), INPCActivity (NPC behaviour)
- **State machine:** PlayerStateController (Idle/Walking/Guitar/Combat/Busy), ConcertService (Idle/Intro/Playing/Finisher)

---

## Working Systems

### Concert Mode (ConcertService.cs — 425 lines)

The core gameplay. Player picks a guitar (Lopata or Gvozdi), then presses A/B/C/D to select loops. Timing determines score.

- Choose window: 2.0s, perfect window: 0.2s
- Perfect = +150 score, -20 chaos. Bad = -100 score, +20 chaos
- Loop D = final loop with special timing bar
- Mid-loop timing challenges appear during loops
- Audio is DSP-synced across all tracks
- NPCs (Evgen on Bass, Diman on Drums) snap to instrument positions during concerts

### Chaos (ChaosManager.cs)

Scale 0–100. Fills from bad timing and item use (Beer/Cigs = +20 each). Hits 100 → level restart. Essentially the "health bar" — too much chaos and the concert falls apart.

### Reputation (ReputationManager.cs)

Scale 0–1000. Increases from concert performance (last note bonus adds overall score). Will be used to gate quests and unlock locations.

### Money (MoneyManager.cs)

Simple int currency. Init at 100. AddMoney / TrySpendMoney. Fires OnMoneyChanged. Currently not spent on anything — placeholder for future shop, casino, bribe mechanics.

### Player

- **PlayerStateController** — state machine: Idle, Walking, Guitar, Combat, Busy. TryEnterBusy/ExitBusy blocks all actions (used for UI, cutscenes)
- **PlayerMovement** — CharacterController-based, camera-relative, with gravity
- **PlayerCameraLook** — mouse look with vertical clamping
- **PlayerInteractor** — raycast from camera (3m), detects IInteractable on layer 6 (Interactable). Focus/UnFocus/Interact lifecycle
- **PlayerCombat** — sphere overlap attack (0.6s cooldown, 5s exit-combat timer, 25 damage, +10 chaos per hit)
- **PlayerSnapMover** — teleport by disabling CharacterController

### Interaction System

Anything on **layer 6 (Interactable)** with a Collider and IInteractable component can be interacted with via E.

Currently implemented interactables:
- **Computer** — triggers OnCallingConcertStart (starts concert)
- **TVPlayer** — video player with channel switching from StreamingAssets (162 lines)
- **PickUpItem** — Beer/Cigs, uses item → adds chaos → destroys self
- **CarInteractable** — exists but EMPTY (no implementation yet)

### NPC System

Activity-based architecture. Two NPCs: Evgen (bass) and Diman (drums).

- **NPCActor** — main component. Types: Evgen, Diman. Has NavMeshMover, ActivityRunner, Animator, RagdollController. During concerts, snaps to instrument spots
- **NPCRoutine** — queue-based plan of activities (where to go, what animation, how long)
- **NPCActivityRunner** — executes current activity, supports interrupts
- **NPCNavMeshMover** — NavMeshAgent wrapper
- **NPCCombat** — IDamageable, 7 hits to die, triggers FightActivity on damage, enables ragdoll on death
- **FightActivity** — chase and punch target within 1.75m
- **UseSpotActivity** — move to reserved spot, play animation for duration

### Inventory (InventoryManager.cs)

Static dictionary. Currently only handles Beer and Cigs — both convert to +20 chaos and play a sound effect.

### Audio (SoundsManager.cs)

Loads AudioConfig from Resources. Sound types: StrikeHit, StrikeBreathe, CrowdCheering, CrowdBooing, DrinkBeer, SmokeCig. Creates temporary AudioSources per sound with clip variation.

### UI (in UI scene)

All HUD elements: ChaosBar, ReputationBar, InventoryBar, InteractableHighlight, ScoreFeedback, ConcertLoopGameplayUI, ConcertMidLoopTimingUI, ConcertFinalTimingUI, ConcertFinishUI, FadeScreen.

---

## In-Progress Systems

### Location / Travel System

Micro open world concept. All locations live in the same Gameplay scene — travel = fade + SetActive toggle + player teleport.

**What exists and works:**
- `TravelSystem.cs` — singleton, manages location unlocks by reputation, TravelRoutine (fade → deactivate → activate → snap → fade), fires events
- `LocationPoint.cs` — placed on scene, holds spawn point Transform and location root GameObject
- `LocationData.cs` — ScriptableObject with Type, DisplayName, MapIcon, UnlockedByDefault, UnlockReputation
- `LocationType.cs` — enum: Garage, Shop, Bar, Casino, EnemyGarage, Street, Warehouse
- Two LocationData assets: GaragesLocation (type 0, unlocked) and ShopLocation (type 1, unlocked)
- TravelSystem is on the Gameplay scene with both LocationPoints assigned and PlayerSnapMover linked

**What does NOT work yet:**
- `CarInteractable.cs` — class exists but all methods are empty (no-ops). Pressing E on the car does nothing
- No TravelMapUI — the UI for selecting a destination was deleted. There is no way for the player to choose where to travel
- No car model on the scene (just an invisible BoxCollider at position 5, 0.5, -28)

---

## Systems That Do NOT Exist Yet

These were planned but have zero code in the project:

- **Dialogue System** — no DialogueManager, no DialogueData, no DialogueUI, no folder
- **Quest System** — no QuestManager, no QuestData, no QuestGiverNPC, no folder
- **Concert Events** — no police raids, grandma complaints, equipment breaks, drunk fan fights
- **Daily Events** — no leaflet guy, shop runs, repair jobs
- **Casino** — no gambling mini-game
- **NPC Dialogue** — NPCs exist but cannot be talked to (no DialogueInteractable)

---

## GameEvents (full list)

```csharp
// Gameplay lifecycle
OnGameplayStarted

// Concert
OnCallingConcertStart, OnCallingRehearsalStart
OnConcertStarted(ConcertData), OnConcertFinished, OnConcertFinishScreenClosed
OnLoopChooseTimerStart, OnLoopChooseTimerUpdate(float), OnLoopChooseTimerEnd
OnNewLoopStart(LoopType)
OnLoopTimingPressed(TimingState) // Perfect or Bad
OnMidLoopTimingStarted(LoopType), OnMidLoopTimingUpdate(float), OnMidLoopTimingEnd
OnInstrumentStarted(NPCType), OnInstrumentStopped(NPCType)
OnConcertScoreUpdated, OnConcertAddScore(int), OnConcertRemoveScore(int)
OnLastNoteBonusPressed(bool)

// Chaos & Reputation
OnChaosChanged(float), OnChaosFilled
OnReputationUpdated, OnReputationFilled

// Interaction & Inventory
OnInteractableFocus(IInteractable), OnInteractableUnFocused
OnInventoryUpdate, OnInventoryItemUsed(InteractableTypes)

// Player
OnWalkingStart, OnWalkingEnd
OnCombatStart, OnCombatEnd, OnAttack
OnGuitarUpdate(GuitarType)

// Money
OnMoneyChanged(int)

// Travel
OnLocationChanged(LocationType), OnTravelStarted, OnTravelFinished
```

---

## File Structure

```
Assets/Scripts/
  Concert/             18 files — ConcertService, audio, loops, timing, lights, crowd, guitars
  NPC/                 12 files — NPCActor, activities, combat, nav, ragdoll, routines
  Location/             5 files — TravelSystem, LocationPoint/Data/Type, CarInteractable (empty)
  [root]               32 files — managers, player systems, UI, interactables, game init

Assets/Scenes/
  Bootstrap.unity      entry point
  UI.unity             all HUD elements
  Gameplay.unity       world, NPCs, interactables, TravelSystem, Car

Assets/Resources/
  AudioConfig.asset               sound definitions
  Interactables/                   Beer, Cigs, Computer, GuitarLopata, TV

Assets/Data/Loops/                 concert audio packs (Lopata, Gvozdi)
Assets/ (root)                     GaragesLocation.asset, ShopLocation.asset
```

---

## Key Rules for Developers / AI Agents

1. **Layer 6 = Interactable.** Any interactive object needs a Collider on this layer — PlayerInteractor raycasts only against `m_Bits: 64`
2. **TryEnterBusy / ExitBusy** — always bracket blocking actions (menus, travel, dialogues). Prevents movement, combat, interaction while in UI
3. **InputStateController.SetUI / SetGameplay** — switch cursor lock and action maps when opening/closing menus
4. **All locations in one scene** — don't add separate scenes. Use LocationPoint with SetActive on the root GameObject
5. **Static managers reset on Init** — called from GameService.StartGame. Don't rely on state surviving scene reloads
6. **CarInteractable is empty** — needs full implementation (open map UI, select destination, call TravelSystem.TravelTo)
7. **No dialogue or quest systems exist** — everything needs to be built from scratch
8. **Concert uses DSP timing** — ConcertService syncs audio precisely. Don't touch AudioSource timing directly
9. **AddReputation is public** — can be called externally (was made public for future quest rewards)
