# Viva Guide — Systems Engineer & Search Guard (Sanjeevsaran J.)

**Student:** Sanjeevsaran J. (`IT24104155`)  
**Dual Role:**
1. **SE3032 / Systems Engineering:** `AccessCard.cs`, `CardReader.cs`, `DoorController.cs`, `EscapeTrigger.cs`
2. **SE3062 / Intelligent Systems (Room 3):** `SearchGuardAI.cs`, `SearchDecisionPolicy.cs`

---

## 1. Script Responsibility Map & Shared Dependencies

| Script | Responsibility | Shared Dependencies |
| --- | --- | --- |
| `Assets/Scripts/AccessCard.cs` | Automatic `OnTriggerEnter` keycard collection (`HasKeycard`, `CollectedCardName`, `CollectedSector`) and `ResetCard()` | `PlayerMovement` |
| `Assets/Scripts/CardReader.cs` | Room 5 terminal `[E]`-key validation (`3.0f` m range), denied/granted feedback, single-trigger victory | `AccessCard`, `WeaponPickup.IsAnyPickupTargeted()`, `GameManager` |
| `Assets/Scripts/DoorController.cs` | Exit barrier lift (`openHeight = 4f`, `doorSpeed = 3f`) and stationary `NavMeshObstacle` carving sync | `AccessCard`, `NavMeshObstacle`, `GameManager` |
| `Assets/Scripts/EscapeTrigger.cs` | Legacy threshold trigger (`isEscapeTriggerActive = false` in favor of Room 5 `CardReader`) | `DoorController`, `GameManager` |
| `Assets/Scripts/SearchGuardAI.cs` | Room 3 `Patrol` / `Pursue` / `Search` state machine (`11 m` sight, `90 deg` FOV, `AK47_Rifle`, `0.75 s` sound cooldown) | `EnemyMovement`, `GuardPerception`, `GuardPerceptionMemory`, `GuardDecisionQueries`, `GuardShooting`, `Health`, `HumanoidAnimator` |
| `Assets/Scripts/SearchDecisionPolicy.cs` | Pure static belief & utility math (`Prior`, `Score`, `NegativeObservation`) | `UnityEngine.Mathf` / `Vector3` |

---

## 2. Key Technical Points for Viva Defense

### 2.1 Interaction Systems
- **Why does `AccessCard` use `OnTriggerEnter` while `CardReader` uses `[E]`?**
  Collecting a physical keycard on contact (`OnTriggerEnter` -> `Collect()`) keeps combat traversal fluid, whereas requiring explicit `[E]` confirmation at `CardReader` (`TryInsertCard()`) prevents accidental mission completion and yields cleanly to `WeaponPickup.IsAnyPickupTargeted()`.
- **How does `CardReader` support both routes without coupling them?**
  Both `Access Card A` (Room 3) and `Access Card B` (Room 4) set `AccessCard.HasKeycard = true` and populate `CollectedCardName` / `CollectedSector`. `CardReader` checks `AccessCard.HasKeycard` once and guards against repeat triggers via `isCardInserted`.

### 2.2 Belief-Guided Search (`SearchGuardAI` & `SearchDecisionPolicy`)
- **Hypothesis Generation (`GenerateSearchCandidates`):** Generates up to **17** reachable positions inside Room 3 (`center` + 8-point `2.5 m` ring + 8-point `5.0 m` ring), deduplicated (`0.75 m`) and filtered via `GuardDecisionQueries.TryReachable` + `IsInsideRoom`. Live hidden player coordinates are **never** read.
- **Prior (`SearchDecisionPolicy.Prior`):**
  $$\text{Prior}(\text{offset}, \mathbf{h}) = \exp\!\left(-\frac{\|\text{offset}\|^2}{32}\right) \cdot \bigl(1 + \max(0, \hat{\text{offset}} \cdot \hat{\mathbf{h}})\bigr)$$
  Sound-only events (`HandleDisturbance`) clear `observedHeading = Vector3.zero` and apply a `0.75 s` cooldown (`nextSoundDecisionAt`).
- **Best-Next-View Utility (`SearchDecisionPolicy.Score`):**
  $$U_i = \frac{\text{clamp01}(\text{observableProbability}_i)}{1 + 0.15 \cdot \text{pathLength}_i}$$
  Sums belief across uninspected candidates visible within `6 m` (`ObservableFrom`), penalizing long NavMesh detours.
- **Negative Evidence (`SearchDecisionPolicy.NegativeObservation`):**
  $$\text{post} = \text{prior} \times 0.08$$
  Multiplies all observable beliefs by `0.08` after an empty `1.1 s` inspection without renormalizing, so `RemainingBeliefMass` naturally decays toward the `< 0.05` stop condition (or `searchTimeout` expiry).

---

## 3. Live Demo Sequence (2 Minutes)

1. **CardReader Denied State:** Start in `Warehouse.unity`, walk to Room 5 `CardReader` (or test in Play Mode) without a card, press `[E]` -> verify `"Access card required"` feedback.
2. **Search Guard (`Room 3`):** Select `Guard_Sanjeev_Search` in Scene/Inspector while playing. Trigger sighting (`Pursue`), step behind a Room 3 rack to break LOS (`Search`) -> show cyan candidate rings, `DecisionCandidates`, `SelectedDecisionTarget`, and decreasing `RemainingBeliefMass` after inspection.
3. **Access Card A -> Victory:** Walk over `Access Card A` (`OnTriggerEnter` logs collection), return to `CardReader`, press `[E]` -> show single victory trigger (`Access Granted! Evacuation Confirmed. [Access Card A]`).
4. **Automated Suite Reference:** Point to **Warehouse > Run Physical Walkthrough Tests** (**58 automated tests** across `Assets/Tests/*.cs`, `41` in `PhysicalWalkthroughTests.cs` + `6` in `InformedGuardDecisionTests.cs` + `6` in `TrapAndSquadTests.cs` + `5` combat/display/capture tests).
