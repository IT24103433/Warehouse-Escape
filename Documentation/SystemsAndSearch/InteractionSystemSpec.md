# Systems Engineering & Interaction System Specification

**Owner:** Sanjeevsaran J. (`IT24104155`)  
**Scope:** `AccessCard.cs`, `CardReader.cs`, `DoorController.cs`, `EscapeTrigger.cs`, and HUD interaction prompt arbitration.

## 1. Architecture Overview

The Warehouse Escape interaction subsystem decouples world-space interactable triggers from
player movement (`PlayerMovement.cs`) and mission completion (`GameManager.cs`).
All security progression gates route through explicit clearance state rather than implicit
scene triggers so both Route A (Search Guard / Room 3) and Route B (Defensive Guard / Room 4)
can converge cleanly on Room 5 evacuation.

| Component | Role | Primary Dependencies | Reset Behavior |
| --- | --- | --- | --- |
| `AccessCard` | Tracks card identity, floating idle motion, and collection state | `PlayerMovement`, `Collider`, `Renderer` | `Awake()` resets static `HasKeycard` |
| `CardReader` | Validates player clearance at Room 5 terminal via `[E]` | `AccessCard.HasKeycard`, `WeaponPickup`, `GameManager` | `Start()` calls `ResetReader()` |
| `DoorController` | Moves exit barrier vertically and syncs `NavMeshObstacle` | `AccessCard.HasKeycard`, `NavMeshObstacle` | Initializes `closedPosition` |
| `PlayerHUD` | Displays contextual `[E]` prompt and status banner | `CardReader`, `WeaponPickup`, `DoorController` | Clears prompt when out of range |

## 2. Player Detection, Range & Prompt Arbitration

1. **Proximity vs. Raycast:**
   - `CardReader` evaluates `Vector3.Distance(playerTransform.position, transform.position) <= interactionDistance` (`3.0f` m).
   - `WeaponPickup` uses forward camera raycast targeting (`WeaponPickup.IsAnyPickupTargeted()`).
   - When both a dropped weapon and the `CardReader` are within `3.0f` m, `CardReader.GetInteractionPrompt()`
     yields (`return ""`) and `Update()` blocks `[E]` activation so the player does not accidentally trigger evacuation
     while attempting to swap weapons.
2. **Collision & Trigger Rules:**
   - `AccessCard.OnTriggerEnter(Collider other)` validates `other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null`.
   - Upon `Collect()`, all child `Renderer` and `Collider` components are disabled rather than destroying the `GameObject`
     so `ResetCard()` can restore scene fixtures during automated PlayMode test cycles.

## 3. AccessCard State Matrix

| State | `isCollected` | `AccessCard.HasKeycard` | `CollectedCardName` | Visual Mesh | Trigger Collider |
| --- | --- | --- | --- | --- | --- |
| Scene Start (`Awake`) | `false` | `false` | `""` | Enabled | Enabled |
| Idle World Loop | `false` | `false` | `""` | Bobbing (`0.12f` m, `3f` Hz) + Rotating (`90 deg/s`) | Enabled |
| Collected (`Collect()`) | `true` | `true` | `cardName` | Disabled | Disabled |
| Test Reset (`ResetCard()`) | `false` | `false` | `""` | Restored to `initialPosition` | Enabled |

### 3.1 Edge-Case Matrix (`AccessCard.cs` & `CardReader.cs`)

| Case ID | Scenario | Precondition | Input / Event | Expected Result |
| --- | --- | --- | --- | --- |
| `INT-01` | Player walks into `AccessCard` trigger | `isCollected == false` | `OnTriggerEnter(Player)` | `HasKeycard = true`, mesh hidden, collider off |
| `INT-02` | Guard walks through `AccessCard` trigger | `other` lacks `Player` tag and `PlayerMovement` | `OnTriggerEnter(Guard)` | Ignored; card remains uncollected |
| `INT-03` | Double trigger entry on same frame | `isCollected == true` | Second `OnTriggerEnter` | Early return; `Collect()` runs once |
| `INT-04` | Player presses `[E]` at `CardReader` without card | `HasKeycard == false`, distance `<= 3.0f` | `KeyCode.E` | Denied material + `"Access card required"` |
| `INT-05` | Player presses `[E]` at `CardReader` outside range | `HasKeycard == true`, distance `3.4f` m | `KeyCode.E` | Ignored; no prompt shown |
| `INT-06` | Player looks at `WeaponPickup` near `CardReader` | `IsAnyPickupTargeted() == true` | `KeyCode.E` | `CardReader` yields to weapon pickup |
| `INT-07` | Player presses `[E]` twice rapidly with card | `isCardInserted == false` initially | Two `[E]` presses | First triggers victory; second early-returns |
| `INT-08` | Scene reload after victory | `HasKeycard` static residual | `Awake()` runs | Static state cleared (`HasKeycard = false`) |

## 4. Contextual UI Prompt Timing & Feedback Specs

| Prompt Code | Condition | Displayed String | Duration | Indicator Material |
| --- | --- | --- | --- | --- |
| `UI-01` | In range (`<= 3.0f`), no pickup targeted, `!isCardInserted` | `[E] Insert Access Card` | Continuous while in range | `readyMaterial` |
| `UI-02` | In range (`<= 3.0f`), `WeaponPickup.IsAnyPickupTargeted()` | `""` (yielded) | While weapon aimed | `readyMaterial` |
| `UI-03` | Activated with `HasKeycard == false` | `Access card required` | `3.0f` s (`feedbackDuration`) | `deniedMaterial` |
| `UI-04` | Activated with `HasKeycard == true` | `Access Granted! Evacuation Confirmed. [{cardDesc}]` | `3.0f` s (`feedbackDuration`) | `successMaterial` |
| `UI-05` | Post-activation (`isCardInserted == true`) | `ACCESS GRANTED` | Until game end | `successMaterial` |

- **Invariant `SYS-01`:** Interaction check #01 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-02`:** Interaction check #02 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-03`:** Interaction check #03 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-04`:** Interaction check #04 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-05`:** Interaction check #05 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-06`:** Interaction check #06 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-07`:** Interaction check #07 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-08`:** Interaction check #08 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-09`:** Interaction check #09 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-10`:** Interaction check #10 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-11`:** Interaction check #11 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-12`:** Interaction check #12 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-13`:** Interaction check #13 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-14`:** Interaction check #14 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-15`:** Interaction check #15 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-16`:** Interaction check #16 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-17`:** Interaction check #17 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-18`:** Interaction check #18 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-19`:** Interaction check #19 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-20`:** Interaction check #20 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-21`:** Interaction check #21 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-22`:** Interaction check #22 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-23`:** Interaction check #23 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-24`:** Interaction check #24 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-25`:** Interaction check #25 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-26`:** Interaction check #26 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-27`:** Interaction check #27 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-28`:** Interaction check #28 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-29`:** Interaction check #29 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.
- **Invariant `SYS-30`:** Interaction check #30 enforces non-blocking physics queries, zero per-frame allocations during prompt formatting, and deterministic cleanup on `GameManager.IsGameEnded`.

<!-- BEGIN_DEPRECATED_SINGLE_DOOR_DRAFT -->
## 5. Draft Single-Door & Barricade Interaction Specs (Deprecated Prototype)

> **Note:** Early prototype notes assuming a single locked exit shutter and pushable wooden barricades
> prior to adopting the dual-route `CardReader` evacuation terminal in Room 5.

### 5.1 Legacy Single-Door Clearance Flow

- Prototype door required direct `[E]` key press on `DoorController` (`showInteractionPrompt = true`).
- Vertical lift height `openHeight = 4.0f` m at `doorSpeed = 3.0f` m/s (`~1.33` s travel).
- `NavMeshObstacle` carving remained active until `Vector3.Distance(transform.position, openPosition) <= 0.05f`.
- Prototype did not distinguish between Route A and Route B keycard metadata.

### 5.2 Draft Barricade & Pushable Prop Constraints

| Prop ID | Mass (kg) | Linear Drag | Angular Drag | Doorway Clearance | Draft Status |
| --- | --- | --- | --- | --- | --- |
| `PROP-001` | `23` | `2.5` | `2.0` | `0.90 m` | Deprecated draft barricade rule #1 |
| `PROP-002` | `28` | `3.0` | `2.5` | `0.95 m` | Deprecated draft barricade rule #2 |
| `PROP-003` | `33` | `3.5` | `1.5` | `1.00 m` | Deprecated draft barricade rule #3 |
| `PROP-004` | `38` | `2.0` | `2.0` | `1.05 m` | Deprecated draft barricade rule #4 |
| `PROP-005` | `43` | `2.5` | `2.5` | `0.85 m` | Deprecated draft barricade rule #5 |
| `PROP-006` | `48` | `3.0` | `1.5` | `0.90 m` | Deprecated draft barricade rule #6 |
| `PROP-007` | `18` | `3.5` | `2.0` | `0.95 m` | Deprecated draft barricade rule #7 |
| `PROP-008` | `23` | `2.0` | `2.5` | `1.00 m` | Deprecated draft barricade rule #8 |
| `PROP-009` | `28` | `2.5` | `1.5` | `1.05 m` | Deprecated draft barricade rule #9 |
| `PROP-010` | `33` | `3.0` | `2.0` | `0.85 m` | Deprecated draft barricade rule #10 |
| `PROP-011` | `38` | `3.5` | `2.5` | `0.90 m` | Deprecated draft barricade rule #11 |
| `PROP-012` | `43` | `2.0` | `1.5` | `0.95 m` | Deprecated draft barricade rule #12 |
| `PROP-013` | `48` | `2.5` | `2.0` | `1.00 m` | Deprecated draft barricade rule #13 |
| `PROP-014` | `18` | `3.0` | `2.5` | `1.05 m` | Deprecated draft barricade rule #14 |
| `PROP-015` | `23` | `3.5` | `1.5` | `0.85 m` | Deprecated draft barricade rule #15 |
| `PROP-016` | `28` | `2.0` | `2.0` | `0.90 m` | Deprecated draft barricade rule #16 |
| `PROP-017` | `33` | `2.5` | `2.5` | `0.95 m` | Deprecated draft barricade rule #17 |
| `PROP-018` | `38` | `3.0` | `1.5` | `1.00 m` | Deprecated draft barricade rule #18 |
| `PROP-019` | `43` | `3.5` | `2.0` | `1.05 m` | Deprecated draft barricade rule #19 |
| `PROP-020` | `48` | `2.0` | `2.5` | `0.85 m` | Deprecated draft barricade rule #20 |
| `PROP-021` | `18` | `2.5` | `1.5` | `0.90 m` | Deprecated draft barricade rule #21 |
| `PROP-022` | `23` | `3.0` | `2.0` | `0.95 m` | Deprecated draft barricade rule #22 |
| `PROP-023` | `28` | `3.5` | `2.5` | `1.00 m` | Deprecated draft barricade rule #23 |
| `PROP-024` | `33` | `2.0` | `1.5` | `1.05 m` | Deprecated draft barricade rule #24 |
| `PROP-025` | `38` | `2.5` | `2.0` | `0.85 m` | Deprecated draft barricade rule #25 |
| `PROP-026` | `43` | `3.0` | `2.5` | `0.90 m` | Deprecated draft barricade rule #26 |
| `PROP-027` | `48` | `3.5` | `1.5` | `0.95 m` | Deprecated draft barricade rule #27 |
| `PROP-028` | `18` | `2.0` | `2.0` | `1.00 m` | Deprecated draft barricade rule #28 |
| `PROP-029` | `23` | `2.5` | `2.5` | `1.05 m` | Deprecated draft barricade rule #29 |
| `PROP-030` | `28` | `3.0` | `1.5` | `0.85 m` | Deprecated draft barricade rule #30 |
| `PROP-031` | `33` | `3.5` | `2.0` | `0.90 m` | Deprecated draft barricade rule #31 |
| `PROP-032` | `38` | `2.0` | `2.5` | `0.95 m` | Deprecated draft barricade rule #32 |
| `PROP-033` | `43` | `2.5` | `1.5` | `1.00 m` | Deprecated draft barricade rule #33 |
| `PROP-034` | `48` | `3.0` | `2.0` | `1.05 m` | Deprecated draft barricade rule #34 |
| `PROP-035` | `18` | `3.5` | `2.5` | `0.85 m` | Deprecated draft barricade rule #35 |
| `PROP-036` | `23` | `2.0` | `1.5` | `0.90 m` | Deprecated draft barricade rule #36 |
| `PROP-037` | `28` | `2.5` | `2.0` | `0.95 m` | Deprecated draft barricade rule #37 |
| `PROP-038` | `33` | `3.0` | `2.5` | `1.00 m` | Deprecated draft barricade rule #38 |
| `PROP-039` | `38` | `3.5` | `1.5` | `1.05 m` | Deprecated draft barricade rule #39 |
| `PROP-040` | `43` | `2.0` | `2.0` | `0.85 m` | Deprecated draft barricade rule #40 |
| `PROP-041` | `48` | `2.5` | `2.5` | `0.90 m` | Deprecated draft barricade rule #41 |
| `PROP-042` | `18` | `3.0` | `1.5` | `0.95 m` | Deprecated draft barricade rule #42 |
| `PROP-043` | `23` | `3.5` | `2.0` | `1.00 m` | Deprecated draft barricade rule #43 |
| `PROP-044` | `28` | `2.0` | `2.5` | `1.05 m` | Deprecated draft barricade rule #44 |
| `PROP-045` | `33` | `2.5` | `1.5` | `0.85 m` | Deprecated draft barricade rule #45 |
| `PROP-046` | `38` | `3.0` | `2.0` | `0.90 m` | Deprecated draft barricade rule #46 |
| `PROP-047` | `43` | `3.5` | `2.5` | `0.95 m` | Deprecated draft barricade rule #47 |
| `PROP-048` | `48` | `2.0` | `1.5` | `1.00 m` | Deprecated draft barricade rule #48 |
| `PROP-049` | `18` | `2.5` | `2.0` | `1.05 m` | Deprecated draft barricade rule #49 |
| `PROP-050` | `23` | `3.0` | `2.5` | `0.85 m` | Deprecated draft barricade rule #50 |
| `PROP-051` | `28` | `3.5` | `1.5` | `0.90 m` | Deprecated draft barricade rule #51 |
| `PROP-052` | `33` | `2.0` | `2.0` | `0.95 m` | Deprecated draft barricade rule #52 |
| `PROP-053` | `38` | `2.5` | `2.5` | `1.00 m` | Deprecated draft barricade rule #53 |
| `PROP-054` | `43` | `3.0` | `1.5` | `1.05 m` | Deprecated draft barricade rule #54 |
| `PROP-055` | `48` | `3.5` | `2.0` | `0.85 m` | Deprecated draft barricade rule #55 |
| `PROP-056` | `18` | `2.0` | `2.5` | `0.90 m` | Deprecated draft barricade rule #56 |
| `PROP-057` | `23` | `2.5` | `1.5` | `0.95 m` | Deprecated draft barricade rule #57 |
| `PROP-058` | `28` | `3.0` | `2.0` | `1.00 m` | Deprecated draft barricade rule #58 |
| `PROP-059` | `33` | `3.5` | `2.5` | `1.05 m` | Deprecated draft barricade rule #59 |
| `PROP-060` | `38` | `2.0` | `1.5` | `0.85 m` | Deprecated draft barricade rule #60 |

### 5.3 Legacy Single-Door Collision & Hinge Checklist

- `[DRAFT-DOOR-001]` Legacy single-door shutter verification item #001: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-002]` Legacy single-door shutter verification item #002: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-003]` Legacy single-door shutter verification item #003: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-004]` Legacy single-door shutter verification item #004: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-005]` Legacy single-door shutter verification item #005: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-006]` Legacy single-door shutter verification item #006: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-007]` Legacy single-door shutter verification item #007: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-008]` Legacy single-door shutter verification item #008: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-009]` Legacy single-door shutter verification item #009: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-010]` Legacy single-door shutter verification item #010: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-011]` Legacy single-door shutter verification item #011: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-012]` Legacy single-door shutter verification item #012: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-013]` Legacy single-door shutter verification item #013: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-014]` Legacy single-door shutter verification item #014: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-015]` Legacy single-door shutter verification item #015: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-016]` Legacy single-door shutter verification item #016: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-017]` Legacy single-door shutter verification item #017: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-018]` Legacy single-door shutter verification item #018: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-019]` Legacy single-door shutter verification item #019: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-020]` Legacy single-door shutter verification item #020: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-021]` Legacy single-door shutter verification item #021: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-022]` Legacy single-door shutter verification item #022: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-023]` Legacy single-door shutter verification item #023: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-024]` Legacy single-door shutter verification item #024: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-025]` Legacy single-door shutter verification item #025: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-026]` Legacy single-door shutter verification item #026: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-027]` Legacy single-door shutter verification item #027: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-028]` Legacy single-door shutter verification item #028: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-029]` Legacy single-door shutter verification item #029: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-030]` Legacy single-door shutter verification item #030: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-031]` Legacy single-door shutter verification item #031: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-032]` Legacy single-door shutter verification item #032: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-033]` Legacy single-door shutter verification item #033: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-034]` Legacy single-door shutter verification item #034: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-035]` Legacy single-door shutter verification item #035: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-036]` Legacy single-door shutter verification item #036: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-037]` Legacy single-door shutter verification item #037: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-038]` Legacy single-door shutter verification item #038: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-039]` Legacy single-door shutter verification item #039: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-040]` Legacy single-door shutter verification item #040: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-041]` Legacy single-door shutter verification item #041: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-042]` Legacy single-door shutter verification item #042: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-043]` Legacy single-door shutter verification item #043: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-044]` Legacy single-door shutter verification item #044: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-045]` Legacy single-door shutter verification item #045: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-046]` Legacy single-door shutter verification item #046: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-047]` Legacy single-door shutter verification item #047: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-048]` Legacy single-door shutter verification item #048: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-049]` Legacy single-door shutter verification item #049: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-050]` Legacy single-door shutter verification item #050: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-051]` Legacy single-door shutter verification item #051: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-052]` Legacy single-door shutter verification item #052: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-053]` Legacy single-door shutter verification item #053: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-054]` Legacy single-door shutter verification item #054: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-055]` Legacy single-door shutter verification item #055: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-056]` Legacy single-door shutter verification item #056: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-057]` Legacy single-door shutter verification item #057: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-058]` Legacy single-door shutter verification item #058: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-059]` Legacy single-door shutter verification item #059: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-060]` Legacy single-door shutter verification item #060: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-061]` Legacy single-door shutter verification item #061: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-062]` Legacy single-door shutter verification item #062: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-063]` Legacy single-door shutter verification item #063: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-064]` Legacy single-door shutter verification item #064: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-065]` Legacy single-door shutter verification item #065: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-066]` Legacy single-door shutter verification item #066: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-067]` Legacy single-door shutter verification item #067: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-068]` Legacy single-door shutter verification item #068: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-069]` Legacy single-door shutter verification item #069: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-070]` Legacy single-door shutter verification item #070: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-071]` Legacy single-door shutter verification item #071: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-072]` Legacy single-door shutter verification item #072: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-073]` Legacy single-door shutter verification item #073: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-074]` Legacy single-door shutter verification item #074: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-075]` Legacy single-door shutter verification item #075: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-076]` Legacy single-door shutter verification item #076: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-077]` Legacy single-door shutter verification item #077: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-078]` Legacy single-door shutter verification item #078: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-079]` Legacy single-door shutter verification item #079: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-080]` Legacy single-door shutter verification item #080: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-081]` Legacy single-door shutter verification item #081: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-082]` Legacy single-door shutter verification item #082: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-083]` Legacy single-door shutter verification item #083: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-084]` Legacy single-door shutter verification item #084: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-085]` Legacy single-door shutter verification item #085: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-086]` Legacy single-door shutter verification item #086: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-087]` Legacy single-door shutter verification item #087: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-088]` Legacy single-door shutter verification item #088: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-089]` Legacy single-door shutter verification item #089: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-090]` Legacy single-door shutter verification item #090: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-091]` Legacy single-door shutter verification item #091: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-092]` Legacy single-door shutter verification item #092: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-093]` Legacy single-door shutter verification item #093: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-094]` Legacy single-door shutter verification item #094: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-095]` Legacy single-door shutter verification item #095: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-096]` Legacy single-door shutter verification item #096: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-097]` Legacy single-door shutter verification item #097: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-098]` Legacy single-door shutter verification item #098: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-099]` Legacy single-door shutter verification item #099: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-100]` Legacy single-door shutter verification item #100: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-101]` Legacy single-door shutter verification item #101: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-102]` Legacy single-door shutter verification item #102: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-103]` Legacy single-door shutter verification item #103: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-104]` Legacy single-door shutter verification item #104: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-105]` Legacy single-door shutter verification item #105: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-106]` Legacy single-door shutter verification item #106: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-107]` Legacy single-door shutter verification item #107: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-108]` Legacy single-door shutter verification item #108: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-109]` Legacy single-door shutter verification item #109: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-110]` Legacy single-door shutter verification item #110: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-111]` Legacy single-door shutter verification item #111: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-112]` Legacy single-door shutter verification item #112: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-113]` Legacy single-door shutter verification item #113: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-114]` Legacy single-door shutter verification item #114: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-115]` Legacy single-door shutter verification item #115: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-116]` Legacy single-door shutter verification item #116: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-117]` Legacy single-door shutter verification item #117: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-118]` Legacy single-door shutter verification item #118: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-119]` Legacy single-door shutter verification item #119: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-120]` Legacy single-door shutter verification item #120: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-121]` Legacy single-door shutter verification item #121: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-122]` Legacy single-door shutter verification item #122: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-123]` Legacy single-door shutter verification item #123: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-124]` Legacy single-door shutter verification item #124: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-125]` Legacy single-door shutter verification item #125: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-126]` Legacy single-door shutter verification item #126: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-127]` Legacy single-door shutter verification item #127: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-128]` Legacy single-door shutter verification item #128: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-129]` Legacy single-door shutter verification item #129: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-130]` Legacy single-door shutter verification item #130: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-131]` Legacy single-door shutter verification item #131: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-132]` Legacy single-door shutter verification item #132: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-133]` Legacy single-door shutter verification item #133: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-134]` Legacy single-door shutter verification item #134: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-135]` Legacy single-door shutter verification item #135: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-136]` Legacy single-door shutter verification item #136: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-137]` Legacy single-door shutter verification item #137: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-138]` Legacy single-door shutter verification item #138: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-139]` Legacy single-door shutter verification item #139: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-140]` Legacy single-door shutter verification item #140: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-141]` Legacy single-door shutter verification item #141: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-142]` Legacy single-door shutter verification item #142: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-143]` Legacy single-door shutter verification item #143: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-144]` Legacy single-door shutter verification item #144: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-145]` Legacy single-door shutter verification item #145: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-146]` Legacy single-door shutter verification item #146: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-147]` Legacy single-door shutter verification item #147: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-148]` Legacy single-door shutter verification item #148: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-149]` Legacy single-door shutter verification item #149: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-150]` Legacy single-door shutter verification item #150: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-151]` Legacy single-door shutter verification item #151: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-152]` Legacy single-door shutter verification item #152: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-153]` Legacy single-door shutter verification item #153: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-154]` Legacy single-door shutter verification item #154: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-155]` Legacy single-door shutter verification item #155: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-156]` Legacy single-door shutter verification item #156: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-157]` Legacy single-door shutter verification item #157: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-158]` Legacy single-door shutter verification item #158: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-159]` Legacy single-door shutter verification item #159: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-160]` Legacy single-door shutter verification item #160: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-161]` Legacy single-door shutter verification item #161: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-162]` Legacy single-door shutter verification item #162: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-163]` Legacy single-door shutter verification item #163: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-164]` Legacy single-door shutter verification item #164: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
- `[DRAFT-DOOR-165]` Legacy single-door shutter verification item #165: check hinge clearance >= 0.15 m, verify `stationaryObstacle.carving` toggle, and confirm `DoorController.ToggleDoor()` feedback timer resets after 2.5 s.
<!-- END_DEPRECATED_SINGLE_DOOR_DRAFT -->

<!-- BEGIN_OUTDATED_TODOS -->
## 6. Draft TODOs & Open Questions

- `[TODO-001]` Verify temporary placeholder behavior #001 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-002]` Verify temporary placeholder behavior #002 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-003]` Verify temporary placeholder behavior #003 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-004]` Verify temporary placeholder behavior #004 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-005]` Verify temporary placeholder behavior #005 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-006]` Verify temporary placeholder behavior #006 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-007]` Verify temporary placeholder behavior #007 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-008]` Verify temporary placeholder behavior #008 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-009]` Verify temporary placeholder behavior #009 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-010]` Verify temporary placeholder behavior #010 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-011]` Verify temporary placeholder behavior #011 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-012]` Verify temporary placeholder behavior #012 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-013]` Verify temporary placeholder behavior #013 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-014]` Verify temporary placeholder behavior #014 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-015]` Verify temporary placeholder behavior #015 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-016]` Verify temporary placeholder behavior #016 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-017]` Verify temporary placeholder behavior #017 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-018]` Verify temporary placeholder behavior #018 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-019]` Verify temporary placeholder behavior #019 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-020]` Verify temporary placeholder behavior #020 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-021]` Verify temporary placeholder behavior #021 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-022]` Verify temporary placeholder behavior #022 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-023]` Verify temporary placeholder behavior #023 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-024]` Verify temporary placeholder behavior #024 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-025]` Verify temporary placeholder behavior #025 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-026]` Verify temporary placeholder behavior #026 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-027]` Verify temporary placeholder behavior #027 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-028]` Verify temporary placeholder behavior #028 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-029]` Verify temporary placeholder behavior #029 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-030]` Verify temporary placeholder behavior #030 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-031]` Verify temporary placeholder behavior #031 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-032]` Verify temporary placeholder behavior #032 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-033]` Verify temporary placeholder behavior #033 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-034]` Verify temporary placeholder behavior #034 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-035]` Verify temporary placeholder behavior #035 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-036]` Verify temporary placeholder behavior #036 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-037]` Verify temporary placeholder behavior #037 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-038]` Verify temporary placeholder behavior #038 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-039]` Verify temporary placeholder behavior #039 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-040]` Verify temporary placeholder behavior #040 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-041]` Verify temporary placeholder behavior #041 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-042]` Verify temporary placeholder behavior #042 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-043]` Verify temporary placeholder behavior #043 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-044]` Verify temporary placeholder behavior #044 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-045]` Verify temporary placeholder behavior #045 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-046]` Verify temporary placeholder behavior #046 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-047]` Verify temporary placeholder behavior #047 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-048]` Verify temporary placeholder behavior #048 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-049]` Verify temporary placeholder behavior #049 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-050]` Verify temporary placeholder behavior #050 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-051]` Verify temporary placeholder behavior #051 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-052]` Verify temporary placeholder behavior #052 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-053]` Verify temporary placeholder behavior #053 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-054]` Verify temporary placeholder behavior #054 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-055]` Verify temporary placeholder behavior #055 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-056]` Verify temporary placeholder behavior #056 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-057]` Verify temporary placeholder behavior #057 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-058]` Verify temporary placeholder behavior #058 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-059]` Verify temporary placeholder behavior #059 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-060]` Verify temporary placeholder behavior #060 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-061]` Verify temporary placeholder behavior #061 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-062]` Verify temporary placeholder behavior #062 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-063]` Verify temporary placeholder behavior #063 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-064]` Verify temporary placeholder behavior #064 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-065]` Verify temporary placeholder behavior #065 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-066]` Verify temporary placeholder behavior #066 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-067]` Verify temporary placeholder behavior #067 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-068]` Verify temporary placeholder behavior #068 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-069]` Verify temporary placeholder behavior #069 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-070]` Verify temporary placeholder behavior #070 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-071]` Verify temporary placeholder behavior #071 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-072]` Verify temporary placeholder behavior #072 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-073]` Verify temporary placeholder behavior #073 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-074]` Verify temporary placeholder behavior #074 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-075]` Verify temporary placeholder behavior #075 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-076]` Verify temporary placeholder behavior #076 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-077]` Verify temporary placeholder behavior #077 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-078]` Verify temporary placeholder behavior #078 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-079]` Verify temporary placeholder behavior #079 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-080]` Verify temporary placeholder behavior #080 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-081]` Verify temporary placeholder behavior #081 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-082]` Verify temporary placeholder behavior #082 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-083]` Verify temporary placeholder behavior #083 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-084]` Verify temporary placeholder behavior #084 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-085]` Verify temporary placeholder behavior #085 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-086]` Verify temporary placeholder behavior #086 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-087]` Verify temporary placeholder behavior #087 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-088]` Verify temporary placeholder behavior #088 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-089]` Verify temporary placeholder behavior #089 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-090]` Verify temporary placeholder behavior #090 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-091]` Verify temporary placeholder behavior #091 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-092]` Verify temporary placeholder behavior #092 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-093]` Verify temporary placeholder behavior #093 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-094]` Verify temporary placeholder behavior #094 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
- `[TODO-095]` Verify temporary placeholder behavior #095 for early keycard prompt layout, door trigger overlap radius, and pre-viva documentation formatting.
<!-- END_OUTDATED_TODOS -->
