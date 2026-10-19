# Room 4 Defensive Encounter & Viva Notes

**Owner:** Aqqil (`IT24104314`)
**Scene Root:** `Room_4_Defensive` (`X [4, 28], Z [-2, 20]`)
**Key Objective:** `AccessCard_Pickup_Defensive` (`Access Card B (Sector 04)`)

## 1. Encounter Design Overview

Room 4 implements Route B of the warehouse branch after Room 2. Players entering through
the south doorway (`X [3.5, 6.5], Z = -2.0`) face a fortified defensive bay anchored by
`Guard_Aqqil_Defensive`. Collecting Access Card B unlocks the Room 5 exit terminal on
equal footing with Route A (Search Sector), keeping both routes independently completable.

### 1.1 Encounter Pillars (Initial Scope)

1. **Anchored Objective Defense:** Guard protects `Terminal_Desk` and Card B.
2. **Close-Quarters Shotgun Threat:** `Combat_Shotgun` rewards using barriers as cover.
3. **Fair Boundary & Escape Option:** Player can disengage beyond leash or flank via aisles.
4. **Visual Readability:** Distinct uniform (`Mat_Guard_Aqqil`), lighting, and prop silhouettes.

<!-- BLOCK:R4_EARLY_BLOCKOUT -->
### 1.2 Early Room 4 Blockout Assumptions (Draft)

- `[R4-DRAFT-01]` Initial room footprint tested at `16 m x 16 m` before `ExpandWarehouseLayout`
  expanded `Room_4_Defensive` to `24 m x 22 m` (`X [4, 28], Z [-2, 20]`).
- `[R4-DRAFT-02]` Placeholder guard capsule replaced at runtime by `HumanoidMeshBuilder`
  (`CharacterRole.Defensive_Aqqil`) inside `DefensiveGuardAI.Awake()`.
- `[R4-DRAFT-03]` Early blockout used primitive Unity cubes for cover prior to importing
  `Warehouse_Security_Barrier.fbx` and `Warehouse_Cargo_Crate.fbx`.
- `[R4-DRAFT-04]` Early `Room4_Shock` trap tested at doorway `(5, 0, 0)` before moving to
  `(17, 0, 5)` beside `Defensive_Barrier_Center` so south entry stayed fair.
- `[R4-DRAFT-05]` Early `leashDistance` tested at `14.0 m`, which let Aqqil chase into
  the south doorway choke; reduced to `9.0 m` (`8.5 m` script default).
- `[R4-DRAFT-06]` Early `Terminal_Desk` stood at `(16, 0.5, 14)` too far north behind the rack;
  shifted to `(16, 0.5, 10)` so the three barriers shield its front arc.
<!-- END:R4_EARLY_BLOCKOUT -->

<!-- SECTION:R4_STATES_AND_LAYOUT -->
## 2. Room 4 Coordinate Layout & Guard Anchor

| Scene Element | World Coordinates / Bounds | Source Reference |
| --- | --- | --- |
| `Room_4_Defensive` Floor | `X [4, 28]`, `Z [-2, 20]` (`24 m x 22 m`) | `ExpandWarehouseLayout.ExpandRoom4()` |
| South Route B Doorway | `X [3.5, 6.5]`, `Z = -2.0` | `ExpandWarehouseLayout.ExpandRoom4()` |
| `Guard_Aqqil_Defensive` Spawn | `(16.0, 1.08, 8.0)` | `ExpandWarehouseLayout.cs` line 642 |
| `DefensiveGuardAI` Code Default | `(-5.0, 1.0, 6.5)` (overridden by scene setup) | `DefensiveGuardAI.cs` line 37 |
| `Terminal_Desk` | `(16.0, 0.5, 10.0)` | `ExpandWarehouseLayout.cs` line 427 |
| `AccessCard_Pickup_Defensive` | `(16.0, 1.15, 10.0)` | `ExpandWarehouseLayout.cs` line 432 |

### 2.1 Attack / Guard / Retreat Decision Flow in Room 4

- Replacing direct player-chasing with the 17-candidate ring decision tree keeps
  `Guard_Aqqil_Defensive` anchored near `Terminal_Desk` while choosing cover behind
  `Defensive_Barrier_West/Center/East` during `PursueIntruder` (Attack) and cleanly
  disengaging via `ReturnToPost` (Retreat) when the player exits the perimeter.
<!-- END:R4_STATES_AND_LAYOUT -->
<!-- SECTION:R4_LEASH_NOTES -->
## 3. Room 4 Boundary & Leash Containment Rules

| Boundary / Zone | Coordinate Span | Guard Containment Effect |
| --- | --- | --- |
| `Room_4_Defensive` Bounds | `X [4, 28], Z [-2, 20]` | Hard `IsInsideRoom` X/Z gate for player & guard |
| Guard `defenseRadius` (`6.0 m`) | `X [10, 22], Z [2, 14]` around post | Entering with LOS transitions `GuardingPost -> PursueIntruder` |
| Guard `leashDistance` (`9.0 m`) | `X [7, 25], Z [-1, 17]` around post | Covers all 3 barriers + Card B desk; never blocks `X = 4..6.5` doorway |
| West Transit Aisle | `X [4, 7]`, `Z [-2, 18]` | Safe flanking / retreat lane outside `leashDistance` |
| Corridor 4->5 Exit | `X ~ 6.5`, `Z = 17.5..20.0` | Outside `9.0 m` leash; disengages pursuit cleanly |

This geometry guarantees players can disengage to Room 2 or Room 5 corridor without being
pursued across room boundaries, matching the Route B fairness specification.
<!-- END:R4_LEASH_NOTES -->
<!-- SECTION:R4_PRESSURE_NOTES -->
## 4. Repositioning Under Player Pressure

- Under aggressive `MP5_SMG` pushes, Aqqil dynamically rotates across `Defensive_Barrier_West`,
  `Defensive_Barrier_Center`, and `Defensive_Barrier_East` whenever flanking turns
  `torsoClear` to `true` or pushes range inside `2 m`.
<!-- BLOCK:R4_HP_DRAFT -->
- *[Draft Note]* Evaluated a `<= 66 HP` (`30%`) forced retreat threshold and `<= 55 HP`
  (`25%`) passive cover bias across `220 -> 10 HP` tests; flagged for pruning because
  turning away inside `defenseRadius` exposed the guard's back and allowed Card B steals.
- *[Draft Note]* Threshold state-flip at `66 HP` conflicted with `ReturnToPost -> PursueIntruder`
  sight re-engagement inside `6.0 m` perimeter.
<!-- END:R4_HP_DRAFT -->
- When the player disengages beyond `9.0 m` leash or exits `Room_4_Defensive` to recover,
  `UpdatePursueIntruderState` triggers `TransitionToReturnToPost()`.
<!-- END:R4_PRESSURE_NOTES -->
<!-- SECTION:R4_SHOTGUN_BALANCE -->
<!-- SECTION:R4_TRAPS_AND_CARD_B -->
<!-- SECTION:R4_PLAYMODE_CHECK -->
<!-- SECTION:R4_VIVA_QA -->
