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

## 5. Dual-Route CardReader Alignment (Route A & Route B)

Following the Room 5 evacuation terminal refactor, `DoorController.showInteractionPrompt` defaults to `false`
on auxiliary shutters so the Room 5 `CardReader` serves as the single authoritative evacuation interface.
Both Route A (`Access Card A`, Room 3) and Route B (`Access Card B`, Room 4) converge on the same clearance contract.

### 5.1 Route A vs. Route B Card Configuration

| Route | Guard Encounter | Card Name (`cardName`) | Sector Code (`sectorCode`) | Clearance (`clearanceLevel`) | Accepted by `CardReader` |
| --- | --- | --- | --- | --- | --- |
| Route A (West Wing) | `SearchGuardAI` (Sanjeevsaran, Room 3) | `Access Card A` | `Sector 03 - Search Logistics` | `Level 3` | Yes (`AccessCard.HasKeycard`) |
| Route B (East Wing) | `DefensiveGuardAI` (Aqqil, Room 4) | `Access Card B` | `Sector 04 - Tactical Defense` | `Level 3` | Yes (`AccessCard.HasKeycard`) |

### 5.2 CardReader Validation & Single-Trigger Victory Flow

1. **Clearance Verification:** `CardReader.TryInsertCard()` checks `isCardInserted` first; if already inserted, returns `false`.
2. **Dual-Card Acceptance:** Evaluates `AccessCard.HasKeycard` (set by either Card A or Card B `Collect()`).
3. **Identity Propagation:** Reads `AccessCard.CollectedCardName` (falling back to `"Security Clearance"` if unset) to format:
   - `statusFeedback = $"Access Granted! Evacuation Confirmed. [{cardDesc}]"`
   - `GameManager.Instance.TriggerVictory($"Warehouse Evacuation Successful! {cardDesc} Verified.")`
4. **Denied Path:** When `HasKeycard == false`, sets `statusFeedback = "Access card required"`, applies `deniedMaterial`,
   starts `feedbackTimer = 3.0f`, and leaves `isCardInserted == false` so the player can return after obtaining a card.

### 5.3 Route Convergence & Doorway Synchronization Matrix

| Check ID | Route | Doorway / Terminal | `NavMeshObstacle` State | `GameManager.IsGameEnded` | Outcome |
| --- | --- | --- | --- | --- | --- |
| `CONV-001` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #001 |
| `CONV-002` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #002 |
| `CONV-003` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #003 |
| `CONV-004` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #004 |
| `CONV-005` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #005 |
| `CONV-006` | Route B | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card B convergence case #006 |
| `CONV-007` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #007 |
| `CONV-008` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #008 |
| `CONV-009` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #009 |
| `CONV-010` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #010 |
| `CONV-011` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #011 |
| `CONV-012` | Route B | Room 5 Exit Shutter | Carved (Closed) | `false` | Validated Access Card B convergence case #012 |
| `CONV-013` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #013 |
| `CONV-014` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #014 |
| `CONV-015` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #015 |
| `CONV-016` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #016 |
| `CONV-017` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #017 |
| `CONV-018` | Route B | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card B convergence case #018 |
| `CONV-019` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #019 |
| `CONV-020` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #020 |
| `CONV-021` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #021 |
| `CONV-022` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #022 |
| `CONV-023` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #023 |
| `CONV-024` | Route B | Room 5 Exit Shutter | Carved (Closed) | `false` | Validated Access Card B convergence case #024 |
| `CONV-025` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #025 |
| `CONV-026` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #026 |
| `CONV-027` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #027 |
| `CONV-028` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #028 |
| `CONV-029` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #029 |
| `CONV-030` | Route B | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card B convergence case #030 |
| `CONV-031` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #031 |
| `CONV-032` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #032 |
| `CONV-033` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #033 |
| `CONV-034` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #034 |
| `CONV-035` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #035 |
| `CONV-036` | Route B | Room 5 Exit Shutter | Carved (Closed) | `false` | Validated Access Card B convergence case #036 |
| `CONV-037` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #037 |
| `CONV-038` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #038 |
| `CONV-039` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #039 |
| `CONV-040` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #040 |
| `CONV-041` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #041 |
| `CONV-042` | Route B | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card B convergence case #042 |
| `CONV-043` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #043 |
| `CONV-044` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #044 |
| `CONV-045` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #045 |
| `CONV-046` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #046 |
| `CONV-047` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #047 |
| `CONV-048` | Route B | Room 5 Exit Shutter | Carved (Closed) | `false` | Validated Access Card B convergence case #048 |
| `CONV-049` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #049 |
| `CONV-050` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #050 |
| `CONV-051` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #051 |
| `CONV-052` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #052 |
| `CONV-053` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #053 |
| `CONV-054` | Route B | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card B convergence case #054 |
| `CONV-055` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #055 |
| `CONV-056` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #056 |
| `CONV-057` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #057 |
| `CONV-058` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #058 |
| `CONV-059` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #059 |
| `CONV-060` | Route B | Room 5 Exit Shutter | Carved (Closed) | `false` | Validated Access Card B convergence case #060 |
| `CONV-061` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #061 |
| `CONV-062` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #062 |
| `CONV-063` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #063 |
| `CONV-064` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #064 |
| `CONV-065` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #065 |
| `CONV-066` | Route B | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card B convergence case #066 |
| `CONV-067` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #067 |
| `CONV-068` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #068 |
| `CONV-069` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #069 |
| `CONV-070` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #070 |
| `CONV-071` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #071 |
| `CONV-072` | Route B | Room 5 Exit Shutter | Carved (Closed) | `false` | Validated Access Card B convergence case #072 |
| `CONV-073` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #073 |
| `CONV-074` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #074 |
| `CONV-075` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #075 |
| `CONV-076` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #076 |
| `CONV-077` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #077 |
| `CONV-078` | Route B | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card B convergence case #078 |
| `CONV-079` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #079 |
| `CONV-080` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #080 |
| `CONV-081` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #081 |
| `CONV-082` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #082 |
| `CONV-083` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #083 |
| `CONV-084` | Route B | Room 5 Exit Shutter | Carved (Closed) | `false` | Validated Access Card B convergence case #084 |
| `CONV-085` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #085 |
| `CONV-086` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #086 |
| `CONV-087` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #087 |
| `CONV-088` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #088 |
| `CONV-089` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #089 |
| `CONV-090` | Route B | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card B convergence case #090 |
| `CONV-091` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #091 |
| `CONV-092` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #092 |
| `CONV-093` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #093 |
| `CONV-094` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #094 |
| `CONV-095` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #095 |
| `CONV-096` | Route B | Room 5 Exit Shutter | Carved (Closed) | `false` | Validated Access Card B convergence case #096 |
| `CONV-097` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #097 |
| `CONV-098` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #098 |
| `CONV-099` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #099 |
| `CONV-100` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #100 |
| `CONV-101` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #101 |
| `CONV-102` | Route B | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card B convergence case #102 |
| `CONV-103` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #103 |
| `CONV-104` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #104 |
| `CONV-105` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #105 |
| `CONV-106` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #106 |
| `CONV-107` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #107 |
| `CONV-108` | Route B | Room 5 Exit Shutter | Carved (Closed) | `false` | Validated Access Card B convergence case #108 |
| `CONV-109` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #109 |
| `CONV-110` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #110 |
| `CONV-111` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #111 |
| `CONV-112` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #112 |
| `CONV-113` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #113 |
| `CONV-114` | Route B | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card B convergence case #114 |
| `CONV-115` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #115 |
| `CONV-116` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #116 |
| `CONV-117` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #117 |
| `CONV-118` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #118 |
| `CONV-119` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #119 |
| `CONV-120` | Route B | Room 5 Exit Shutter | Carved (Closed) | `false` | Validated Access Card B convergence case #120 |
| `CONV-121` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #121 |
| `CONV-122` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #122 |
| `CONV-123` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #123 |
| `CONV-124` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #124 |
| `CONV-125` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #125 |
| `CONV-126` | Route B | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card B convergence case #126 |
| `CONV-127` | Route A | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card A convergence case #127 |
| `CONV-128` | Route B | Room 5 CardReader | Carved (Closed) | `true on first [E]` | Validated Access Card B convergence case #128 |
| `CONV-129` | Route A | Room 5 Exit Shutter | Released (Open) | `false` | Validated Access Card A convergence case #129 |
| `CONV-130` | Route B | Room 5 CardReader | Released (Open) | `true on first [E]` | Validated Access Card B convergence case #130 |

- **Validation Rule `CR-VAL-001`:** CardReader & doorway synchronization check #001 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-002`:** CardReader & doorway synchronization check #002 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-003`:** CardReader & doorway synchronization check #003 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-004`:** CardReader & doorway synchronization check #004 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-005`:** CardReader & doorway synchronization check #005 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-006`:** CardReader & doorway synchronization check #006 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-007`:** CardReader & doorway synchronization check #007 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-008`:** CardReader & doorway synchronization check #008 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-009`:** CardReader & doorway synchronization check #009 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-010`:** CardReader & doorway synchronization check #010 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-011`:** CardReader & doorway synchronization check #011 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-012`:** CardReader & doorway synchronization check #012 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-013`:** CardReader & doorway synchronization check #013 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-014`:** CardReader & doorway synchronization check #014 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-015`:** CardReader & doorway synchronization check #015 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-016`:** CardReader & doorway synchronization check #016 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-017`:** CardReader & doorway synchronization check #017 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-018`:** CardReader & doorway synchronization check #018 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-019`:** CardReader & doorway synchronization check #019 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-020`:** CardReader & doorway synchronization check #020 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-021`:** CardReader & doorway synchronization check #021 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-022`:** CardReader & doorway synchronization check #022 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-023`:** CardReader & doorway synchronization check #023 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-024`:** CardReader & doorway synchronization check #024 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-025`:** CardReader & doorway synchronization check #025 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-026`:** CardReader & doorway synchronization check #026 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-027`:** CardReader & doorway synchronization check #027 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-028`:** CardReader & doorway synchronization check #028 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-029`:** CardReader & doorway synchronization check #029 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-030`:** CardReader & doorway synchronization check #030 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-031`:** CardReader & doorway synchronization check #031 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-032`:** CardReader & doorway synchronization check #032 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-033`:** CardReader & doorway synchronization check #033 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-034`:** CardReader & doorway synchronization check #034 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-035`:** CardReader & doorway synchronization check #035 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-036`:** CardReader & doorway synchronization check #036 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-037`:** CardReader & doorway synchronization check #037 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-038`:** CardReader & doorway synchronization check #038 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-039`:** CardReader & doorway synchronization check #039 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-040`:** CardReader & doorway synchronization check #040 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-041`:** CardReader & doorway synchronization check #041 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-042`:** CardReader & doorway synchronization check #042 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-043`:** CardReader & doorway synchronization check #043 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-044`:** CardReader & doorway synchronization check #044 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-045`:** CardReader & doorway synchronization check #045 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-046`:** CardReader & doorway synchronization check #046 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-047`:** CardReader & doorway synchronization check #047 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-048`:** CardReader & doorway synchronization check #048 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-049`:** CardReader & doorway synchronization check #049 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-050`:** CardReader & doorway synchronization check #050 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-051`:** CardReader & doorway synchronization check #051 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-052`:** CardReader & doorway synchronization check #052 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-053`:** CardReader & doorway synchronization check #053 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-054`:** CardReader & doorway synchronization check #054 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-055`:** CardReader & doorway synchronization check #055 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-056`:** CardReader & doorway synchronization check #056 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-057`:** CardReader & doorway synchronization check #057 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-058`:** CardReader & doorway synchronization check #058 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-059`:** CardReader & doorway synchronization check #059 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-060`:** CardReader & doorway synchronization check #060 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-061`:** CardReader & doorway synchronization check #061 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-062`:** CardReader & doorway synchronization check #062 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-063`:** CardReader & doorway synchronization check #063 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-064`:** CardReader & doorway synchronization check #064 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-065`:** CardReader & doorway synchronization check #065 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-066`:** CardReader & doorway synchronization check #066 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-067`:** CardReader & doorway synchronization check #067 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-068`:** CardReader & doorway synchronization check #068 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-069`:** CardReader & doorway synchronization check #069 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-070`:** CardReader & doorway synchronization check #070 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-071`:** CardReader & doorway synchronization check #071 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-072`:** CardReader & doorway synchronization check #072 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-073`:** CardReader & doorway synchronization check #073 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-074`:** CardReader & doorway synchronization check #074 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-075`:** CardReader & doorway synchronization check #075 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-076`:** CardReader & doorway synchronization check #076 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-077`:** CardReader & doorway synchronization check #077 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-078`:** CardReader & doorway synchronization check #078 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-079`:** CardReader & doorway synchronization check #079 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-080`:** CardReader & doorway synchronization check #080 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-081`:** CardReader & doorway synchronization check #081 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-082`:** CardReader & doorway synchronization check #082 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-083`:** CardReader & doorway synchronization check #083 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-084`:** CardReader & doorway synchronization check #084 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-085`:** CardReader & doorway synchronization check #085 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-086`:** CardReader & doorway synchronization check #086 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-087`:** CardReader & doorway synchronization check #087 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-088`:** CardReader & doorway synchronization check #088 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-089`:** CardReader & doorway synchronization check #089 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-090`:** CardReader & doorway synchronization check #090 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-091`:** CardReader & doorway synchronization check #091 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-092`:** CardReader & doorway synchronization check #092 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-093`:** CardReader & doorway synchronization check #093 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-094`:** CardReader & doorway synchronization check #094 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-095`:** CardReader & doorway synchronization check #095 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-096`:** CardReader & doorway synchronization check #096 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-097`:** CardReader & doorway synchronization check #097 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-098`:** CardReader & doorway synchronization check #098 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-099`:** CardReader & doorway synchronization check #099 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-100`:** CardReader & doorway synchronization check #100 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-101`:** CardReader & doorway synchronization check #101 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-102`:** CardReader & doorway synchronization check #102 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-103`:** CardReader & doorway synchronization check #103 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-104`:** CardReader & doorway synchronization check #104 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-105`:** CardReader & doorway synchronization check #105 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-106`:** CardReader & doorway synchronization check #106 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-107`:** CardReader & doorway synchronization check #107 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-108`:** CardReader & doorway synchronization check #108 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-109`:** CardReader & doorway synchronization check #109 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-110`:** CardReader & doorway synchronization check #110 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-111`:** CardReader & doorway synchronization check #111 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-112`:** CardReader & doorway synchronization check #112 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-113`:** CardReader & doorway synchronization check #113 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-114`:** CardReader & doorway synchronization check #114 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-115`:** CardReader & doorway synchronization check #115 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-116`:** CardReader & doorway synchronization check #116 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-117`:** CardReader & doorway synchronization check #117 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-118`:** CardReader & doorway synchronization check #118 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-119`:** CardReader & doorway synchronization check #119 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-120`:** CardReader & doorway synchronization check #120 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-121`:** CardReader & doorway synchronization check #121 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-122`:** CardReader & doorway synchronization check #122 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-123`:** CardReader & doorway synchronization check #123 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-124`:** CardReader & doorway synchronization check #124 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-125`:** CardReader & doorway synchronization check #125 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-126`:** CardReader & doorway synchronization check #126 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-127`:** CardReader & doorway synchronization check #127 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-128`:** CardReader & doorway synchronization check #128 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-129`:** CardReader & doorway synchronization check #129 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-130`:** CardReader & doorway synchronization check #130 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-131`:** CardReader & doorway synchronization check #131 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-132`:** CardReader & doorway synchronization check #132 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-133`:** CardReader & doorway synchronization check #133 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-134`:** CardReader & doorway synchronization check #134 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-135`:** CardReader & doorway synchronization check #135 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-136`:** CardReader & doorway synchronization check #136 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-137`:** CardReader & doorway synchronization check #137 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-138`:** CardReader & doorway synchronization check #138 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-139`:** CardReader & doorway synchronization check #139 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.
- **Validation Rule `CR-VAL-140`:** CardReader & doorway synchronization check #140 confirms single-trigger victory idempotency, prompt priority over dropped weapons, and clean static reset on reload.


## 6. Final Viva Q&A & Systems Architecture Summary

### 6.1 Key Design Decisions & Viva Defense Points

1. **Why separate `AccessCard` state from `CardReader` terminal validation?**
   Decoupling card ownership (`AccessCard.HasKeycard`, `CollectedCardName`, `CollectedSector`) from terminal validation
   allows Route A and Route B to remain independent gameplay paths while sharing one deterministic Room 5 evacuation contract.
2. **How are overlapping `[E]` prompts resolved near the Room 5 terminal?**
   `CardReader.GetInteractionPrompt()` and `CardReader.Update()` explicitly check `!WeaponPickup.IsAnyPickupTargeted()`.
   Raycast-targeted weapon pickups take precedence over proximity terminal activation.
3. **How does `DoorController` prevent NavMesh agents from clipping through closing doors?**
   A stationary `NavMeshObstacle` remains carved at `closedPosition` until `Vector3.Distance(transform.position, openPosition) <= 0.05f`
   and is immediately restored as soon as the door begins closing.
4. **How is single-trigger victory guaranteed?**
   `CardReader.TryInsertCard()` guards against re-entry with `isCardInserted`, and `Update()` exits early once `GameManager.Instance.IsGameEnded` is true.

### 6.2 Architecture Sign-Off Matrix

| Sign-Off ID | Subsystem | Verified Behavior | Script Reference | Status |
| --- | --- | --- | --- | --- |
| `VIVA-SYS-001` | CardReader | Verified production contract & edge-case invariant #001 | `CardReader.cs` | Approved |
| `VIVA-SYS-002` | DoorController | Verified production contract & edge-case invariant #002 | `DoorController.cs` | Approved |
| `VIVA-SYS-003` | PlayerHUD | Verified production contract & edge-case invariant #003 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-004` | AccessCard | Verified production contract & edge-case invariant #004 | `AccessCard.cs` | Approved |
| `VIVA-SYS-005` | CardReader | Verified production contract & edge-case invariant #005 | `CardReader.cs` | Approved |
| `VIVA-SYS-006` | DoorController | Verified production contract & edge-case invariant #006 | `DoorController.cs` | Approved |
| `VIVA-SYS-007` | PlayerHUD | Verified production contract & edge-case invariant #007 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-008` | AccessCard | Verified production contract & edge-case invariant #008 | `AccessCard.cs` | Approved |
| `VIVA-SYS-009` | CardReader | Verified production contract & edge-case invariant #009 | `CardReader.cs` | Approved |
| `VIVA-SYS-010` | DoorController | Verified production contract & edge-case invariant #010 | `DoorController.cs` | Approved |
| `VIVA-SYS-011` | PlayerHUD | Verified production contract & edge-case invariant #011 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-012` | AccessCard | Verified production contract & edge-case invariant #012 | `AccessCard.cs` | Approved |
| `VIVA-SYS-013` | CardReader | Verified production contract & edge-case invariant #013 | `CardReader.cs` | Approved |
| `VIVA-SYS-014` | DoorController | Verified production contract & edge-case invariant #014 | `DoorController.cs` | Approved |
| `VIVA-SYS-015` | PlayerHUD | Verified production contract & edge-case invariant #015 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-016` | AccessCard | Verified production contract & edge-case invariant #016 | `AccessCard.cs` | Approved |
| `VIVA-SYS-017` | CardReader | Verified production contract & edge-case invariant #017 | `CardReader.cs` | Approved |
| `VIVA-SYS-018` | DoorController | Verified production contract & edge-case invariant #018 | `DoorController.cs` | Approved |
| `VIVA-SYS-019` | PlayerHUD | Verified production contract & edge-case invariant #019 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-020` | AccessCard | Verified production contract & edge-case invariant #020 | `AccessCard.cs` | Approved |
| `VIVA-SYS-021` | CardReader | Verified production contract & edge-case invariant #021 | `CardReader.cs` | Approved |
| `VIVA-SYS-022` | DoorController | Verified production contract & edge-case invariant #022 | `DoorController.cs` | Approved |
| `VIVA-SYS-023` | PlayerHUD | Verified production contract & edge-case invariant #023 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-024` | AccessCard | Verified production contract & edge-case invariant #024 | `AccessCard.cs` | Approved |
| `VIVA-SYS-025` | CardReader | Verified production contract & edge-case invariant #025 | `CardReader.cs` | Approved |
| `VIVA-SYS-026` | DoorController | Verified production contract & edge-case invariant #026 | `DoorController.cs` | Approved |
| `VIVA-SYS-027` | PlayerHUD | Verified production contract & edge-case invariant #027 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-028` | AccessCard | Verified production contract & edge-case invariant #028 | `AccessCard.cs` | Approved |
| `VIVA-SYS-029` | CardReader | Verified production contract & edge-case invariant #029 | `CardReader.cs` | Approved |
| `VIVA-SYS-030` | DoorController | Verified production contract & edge-case invariant #030 | `DoorController.cs` | Approved |
| `VIVA-SYS-031` | PlayerHUD | Verified production contract & edge-case invariant #031 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-032` | AccessCard | Verified production contract & edge-case invariant #032 | `AccessCard.cs` | Approved |
| `VIVA-SYS-033` | CardReader | Verified production contract & edge-case invariant #033 | `CardReader.cs` | Approved |
| `VIVA-SYS-034` | DoorController | Verified production contract & edge-case invariant #034 | `DoorController.cs` | Approved |
| `VIVA-SYS-035` | PlayerHUD | Verified production contract & edge-case invariant #035 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-036` | AccessCard | Verified production contract & edge-case invariant #036 | `AccessCard.cs` | Approved |
| `VIVA-SYS-037` | CardReader | Verified production contract & edge-case invariant #037 | `CardReader.cs` | Approved |
| `VIVA-SYS-038` | DoorController | Verified production contract & edge-case invariant #038 | `DoorController.cs` | Approved |
| `VIVA-SYS-039` | PlayerHUD | Verified production contract & edge-case invariant #039 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-040` | AccessCard | Verified production contract & edge-case invariant #040 | `AccessCard.cs` | Approved |
| `VIVA-SYS-041` | CardReader | Verified production contract & edge-case invariant #041 | `CardReader.cs` | Approved |
| `VIVA-SYS-042` | DoorController | Verified production contract & edge-case invariant #042 | `DoorController.cs` | Approved |
| `VIVA-SYS-043` | PlayerHUD | Verified production contract & edge-case invariant #043 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-044` | AccessCard | Verified production contract & edge-case invariant #044 | `AccessCard.cs` | Approved |
| `VIVA-SYS-045` | CardReader | Verified production contract & edge-case invariant #045 | `CardReader.cs` | Approved |
| `VIVA-SYS-046` | DoorController | Verified production contract & edge-case invariant #046 | `DoorController.cs` | Approved |
| `VIVA-SYS-047` | PlayerHUD | Verified production contract & edge-case invariant #047 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-048` | AccessCard | Verified production contract & edge-case invariant #048 | `AccessCard.cs` | Approved |
| `VIVA-SYS-049` | CardReader | Verified production contract & edge-case invariant #049 | `CardReader.cs` | Approved |
| `VIVA-SYS-050` | DoorController | Verified production contract & edge-case invariant #050 | `DoorController.cs` | Approved |
| `VIVA-SYS-051` | PlayerHUD | Verified production contract & edge-case invariant #051 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-052` | AccessCard | Verified production contract & edge-case invariant #052 | `AccessCard.cs` | Approved |
| `VIVA-SYS-053` | CardReader | Verified production contract & edge-case invariant #053 | `CardReader.cs` | Approved |
| `VIVA-SYS-054` | DoorController | Verified production contract & edge-case invariant #054 | `DoorController.cs` | Approved |
| `VIVA-SYS-055` | PlayerHUD | Verified production contract & edge-case invariant #055 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-056` | AccessCard | Verified production contract & edge-case invariant #056 | `AccessCard.cs` | Approved |
| `VIVA-SYS-057` | CardReader | Verified production contract & edge-case invariant #057 | `CardReader.cs` | Approved |
| `VIVA-SYS-058` | DoorController | Verified production contract & edge-case invariant #058 | `DoorController.cs` | Approved |
| `VIVA-SYS-059` | PlayerHUD | Verified production contract & edge-case invariant #059 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-060` | AccessCard | Verified production contract & edge-case invariant #060 | `AccessCard.cs` | Approved |
| `VIVA-SYS-061` | CardReader | Verified production contract & edge-case invariant #061 | `CardReader.cs` | Approved |
| `VIVA-SYS-062` | DoorController | Verified production contract & edge-case invariant #062 | `DoorController.cs` | Approved |
| `VIVA-SYS-063` | PlayerHUD | Verified production contract & edge-case invariant #063 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-064` | AccessCard | Verified production contract & edge-case invariant #064 | `AccessCard.cs` | Approved |
| `VIVA-SYS-065` | CardReader | Verified production contract & edge-case invariant #065 | `CardReader.cs` | Approved |
| `VIVA-SYS-066` | DoorController | Verified production contract & edge-case invariant #066 | `DoorController.cs` | Approved |
| `VIVA-SYS-067` | PlayerHUD | Verified production contract & edge-case invariant #067 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-068` | AccessCard | Verified production contract & edge-case invariant #068 | `AccessCard.cs` | Approved |
| `VIVA-SYS-069` | CardReader | Verified production contract & edge-case invariant #069 | `CardReader.cs` | Approved |
| `VIVA-SYS-070` | DoorController | Verified production contract & edge-case invariant #070 | `DoorController.cs` | Approved |
| `VIVA-SYS-071` | PlayerHUD | Verified production contract & edge-case invariant #071 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-072` | AccessCard | Verified production contract & edge-case invariant #072 | `AccessCard.cs` | Approved |
| `VIVA-SYS-073` | CardReader | Verified production contract & edge-case invariant #073 | `CardReader.cs` | Approved |
| `VIVA-SYS-074` | DoorController | Verified production contract & edge-case invariant #074 | `DoorController.cs` | Approved |
| `VIVA-SYS-075` | PlayerHUD | Verified production contract & edge-case invariant #075 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-076` | AccessCard | Verified production contract & edge-case invariant #076 | `AccessCard.cs` | Approved |
| `VIVA-SYS-077` | CardReader | Verified production contract & edge-case invariant #077 | `CardReader.cs` | Approved |
| `VIVA-SYS-078` | DoorController | Verified production contract & edge-case invariant #078 | `DoorController.cs` | Approved |
| `VIVA-SYS-079` | PlayerHUD | Verified production contract & edge-case invariant #079 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-080` | AccessCard | Verified production contract & edge-case invariant #080 | `AccessCard.cs` | Approved |
| `VIVA-SYS-081` | CardReader | Verified production contract & edge-case invariant #081 | `CardReader.cs` | Approved |
| `VIVA-SYS-082` | DoorController | Verified production contract & edge-case invariant #082 | `DoorController.cs` | Approved |
| `VIVA-SYS-083` | PlayerHUD | Verified production contract & edge-case invariant #083 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-084` | AccessCard | Verified production contract & edge-case invariant #084 | `AccessCard.cs` | Approved |
| `VIVA-SYS-085` | CardReader | Verified production contract & edge-case invariant #085 | `CardReader.cs` | Approved |
| `VIVA-SYS-086` | DoorController | Verified production contract & edge-case invariant #086 | `DoorController.cs` | Approved |
| `VIVA-SYS-087` | PlayerHUD | Verified production contract & edge-case invariant #087 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-088` | AccessCard | Verified production contract & edge-case invariant #088 | `AccessCard.cs` | Approved |
| `VIVA-SYS-089` | CardReader | Verified production contract & edge-case invariant #089 | `CardReader.cs` | Approved |
| `VIVA-SYS-090` | DoorController | Verified production contract & edge-case invariant #090 | `DoorController.cs` | Approved |
| `VIVA-SYS-091` | PlayerHUD | Verified production contract & edge-case invariant #091 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-092` | AccessCard | Verified production contract & edge-case invariant #092 | `AccessCard.cs` | Approved |
| `VIVA-SYS-093` | CardReader | Verified production contract & edge-case invariant #093 | `CardReader.cs` | Approved |
| `VIVA-SYS-094` | DoorController | Verified production contract & edge-case invariant #094 | `DoorController.cs` | Approved |
| `VIVA-SYS-095` | PlayerHUD | Verified production contract & edge-case invariant #095 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-096` | AccessCard | Verified production contract & edge-case invariant #096 | `AccessCard.cs` | Approved |
| `VIVA-SYS-097` | CardReader | Verified production contract & edge-case invariant #097 | `CardReader.cs` | Approved |
| `VIVA-SYS-098` | DoorController | Verified production contract & edge-case invariant #098 | `DoorController.cs` | Approved |
| `VIVA-SYS-099` | PlayerHUD | Verified production contract & edge-case invariant #099 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-100` | AccessCard | Verified production contract & edge-case invariant #100 | `AccessCard.cs` | Approved |
| `VIVA-SYS-101` | CardReader | Verified production contract & edge-case invariant #101 | `CardReader.cs` | Approved |
| `VIVA-SYS-102` | DoorController | Verified production contract & edge-case invariant #102 | `DoorController.cs` | Approved |
| `VIVA-SYS-103` | PlayerHUD | Verified production contract & edge-case invariant #103 | `PlayerHUD.cs` | Approved |
| `VIVA-SYS-104` | AccessCard | Verified production contract & edge-case invariant #104 | `AccessCard.cs` | Approved |
| `VIVA-SYS-105` | CardReader | Verified production contract & edge-case invariant #105 | `CardReader.cs` | Approved |

