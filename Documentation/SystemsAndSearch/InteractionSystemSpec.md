# Systems Engineering & Interaction System Specification

**Owner:** Sanjeevsaran J. (`IT24104155`)  
**Scope:** `AccessCard.cs`, `CardReader.cs`, `DoorController.cs`, `EscapeTrigger.cs`, and HUD interaction prompt arbitration.

## 1. Architecture Overview

The Warehouse Escape interaction subsystem decouples world-space clearance pickups (`AccessCard.cs`) from terminal validation (`CardReader.cs`) and mission completion (`GameManager.cs`). Both Route A (Search Guard / Room 3) and Route B (Defensive Guard / Room 4) converge on the single Room 5 `CardReader` evacuation contract.

| Component | Role | Trigger / Input Mechanism | Primary Dependencies | Reset Behavior |
| --- | --- | --- | --- | --- |
| `AccessCard` | Keycard pickup & static clearance state | **Automatic `OnTriggerEnter`** (no `[E]` key press) | `PlayerMovement`, `Collider`, `Renderer` | `Awake()` resets static `HasKeycard` |
| `CardReader` | Room 5 terminal clearance check & victory trigger | **Manual `[E]` key** within `3.0f` m (`!WeaponPickup.IsAnyPickupTargeted()`) | `AccessCard.HasKeycard`, `WeaponPickup`, `GameManager` | `Start()` calls `ResetReader()` |
| `DoorController` | Vertical exit barrier & `NavMeshObstacle` sync | Optional `[E]` (`showInteractionPrompt = false` by default) | `AccessCard.HasKeycard`, `NavMeshObstacle` | Captures `closedPosition` in `Start()` |
| `EscapeTrigger` | Legacy threshold trigger (disabled by default) | `isEscapeTriggerActive = false` in favor of `CardReader` | `DoorController`, `GameManager` | `ResetTrigger()` |
| `PlayerHUD` | Contextual `[E]` prompt & status banner display | Polls `CardReader.GetInteractionPrompt()` / `WeaponPickup` | `CardReader`, `WeaponPickup`, `DoorController` | Clears prompt when out of range |

## 2. Collection vs. Activation Contract

### 2.1 `AccessCard.cs` — Automatic Trigger Collection
- **Collection Mechanism:** `AccessCard.OnTriggerEnter(Collider other)` checks `other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null` and immediately invokes `Collect()` with **no `[E]` key input required**.
- **State Updated on `Collect()`:** Sets instance `isCollected = true`, static `AccessCard.HasKeycard = true`, `AccessCard.CollectedCardName = cardName`, and `AccessCard.CollectedSector = sectorCode`.
- **Fixture Preservation:** Disables child `Renderer` and `Collider` components instead of destroying the `GameObject`, allowing `ResetCard()` to restore the pickup cleanly between PlayMode test runs.

### 2.2 `CardReader.cs` — `[E]`-Key Terminal Validation & Prompt Arbitration
- **Proximity Check:** `IsPlayerInRange` checks `Vector3.Distance(playerTransform.position, transform.position) <= interactionDistance` (`3.0f` m).
- **Weapon Pickup Yield:** If `WeaponPickup.IsAnyPickupTargeted()` is `true` while the player stands within `3.0f` m of the terminal, `CardReader.GetInteractionPrompt()` returns `""` and `Update()` ignores `KeyCode.E` so weapon swapping takes priority.
- **Validation (`TryInsertCard()`):**
  - Guards re-entry with `if (isCardInserted) return false;`.
  - **Valid Card (`AccessCard.HasKeycard == true`):** Sets `isCardInserted = true`, displays `"Access Granted! Evacuation Confirmed. [{cardDesc}]"`, applies `successMaterial`, and calls `GameManager.Instance.TriggerVictory(...)` once.
  - **Missing Card (`AccessCard.HasKeycard == false`):** Displays `"Access card required"` for `feedbackDuration = 3.0f` s, applies `deniedMaterial`, and keeps `isCardInserted = false` so gameplay continues.

## 3. AccessCard State Matrix

| State | `isCollected` | `AccessCard.HasKeycard` | `CollectedCardName` | Visual Mesh | Trigger Collider |
| --- | --- | --- | --- | --- | --- |
| Scene Start (`Awake`) | `false` | `false` | `""` | Enabled | Enabled |
| Idle World Loop | `false` | `false` | `""` | Bobbing (`0.12f` m, `3f` Hz) + Rotating (`90 deg/s`) | Enabled |
| Collected (`OnTriggerEnter` -> `Collect()`) | `true` | `true` | `cardName` | Disabled | Disabled |
| Test Reset (`ResetCard()`) | `false` | `false` | `""` | Restored to `initialPosition` | Enabled |

## 4. Dual-Route Convergence & Edge-Case Matrix

| Case ID | Scenario | Precondition | Input / Event | Expected Result |
| --- | --- | --- | --- | --- |
| `INT-01` | Player walks into `AccessCard` trigger | `isCollected == false` | `OnTriggerEnter(Player)` | Auto-collected: `HasKeycard = true`, mesh hidden, collider off |
| `INT-02` | Guard walks through `AccessCard` trigger | `other` lacks `Player` tag and `PlayerMovement` | `OnTriggerEnter(Guard)` | Ignored; card remains uncollected |
| `INT-03` | Double trigger entry on same frame | `isCollected == true` | Second `OnTriggerEnter` | Early return; `Collect()` runs once |
| `INT-04` | Player presses `[E]` at `CardReader` without card | `HasKeycard == false`, distance `<= 3.0f` | `KeyCode.E` | Denied (`deniedMaterial`, `"Access card required"`) |
| `INT-05` | Player presses `[E]` at `CardReader` outside range | `HasKeycard == true`, distance `> 3.0f` m | `KeyCode.E` | Ignored; no prompt shown |
| `INT-06` | Player looks at `WeaponPickup` near `CardReader` | `IsAnyPickupTargeted() == true` | `KeyCode.E` | `CardReader` yields prompt and `[E]` input to weapon pickup |
| `INT-07` | Player presses `[E]` twice rapidly with card | `isCardInserted == false` initially | Two `[E]` presses | First triggers victory once; second early-returns |
| `INT-08` | Route A vs. Route B clearance convergence | Either Card A (`Sector 03`) or Card B (`Sector 04`) collected | `[E]` at `CardReader` | Both unlock Room 5 terminal via `AccessCard.HasKeycard` |
| `INT-09` | `DoorController` obstacle sync | `DoorController.requiresKeycard == true` | `ToggleDoor()` / `SetOpen()` | `stationaryObstacle` stays carved until within `0.05f` m of `openPosition` |
| `INT-10` | Scene reload after victory | Static `HasKeycard == true` residual | `Awake()` / `ResetReader()` | Static card flags and reader state cleared cleanly |

## 5. Contextual UI Prompt Timing & Feedback Specs

| Prompt Code | Condition | Displayed String | Duration | Indicator Material |
| --- | --- | --- | --- | --- |
| `UI-01` | In range (`<= 3.0f`), no pickup targeted, `!isCardInserted` | `[E] Insert Access Card` | Continuous while in range | `readyMaterial` |
| `UI-02` | In range (`<= 3.0f`), `WeaponPickup.IsAnyPickupTargeted()` | `""` (yielded) | While weapon aimed | `readyMaterial` |
| `UI-03` | Activated with `HasKeycard == false` | `Access card required` | `3.0f` s (`feedbackDuration`) | `deniedMaterial` |
| `UI-04` | Activated with `HasKeycard == true` | `Access Granted! Evacuation Confirmed. [{cardDesc}]` | `3.0f` s (`feedbackDuration`) | `successMaterial` |
| `UI-05` | Post-activation (`isCardInserted == true`) | `ACCESS GRANTED` | Until game end | `successMaterial` |
