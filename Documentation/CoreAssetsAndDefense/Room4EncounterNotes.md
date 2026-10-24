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
- When the player disengages beyond `9.0 m` leash or exits `Room_4_Defensive` to recover,
  `UpdatePursueIntruderState` triggers `TransitionToReturnToPost()`.
<!-- END:R4_PRESSURE_NOTES -->
<!-- SECTION:R4_SHOTGUN_BALANCE -->
## 5. `Combat_Shotgun` 6-Pellet Damage & 220 HP Guard Durability Balance

### 5.1 Guard Health & Shotgun Parameters (`DefensiveGuardAI.Awake` & `GuardShooting.cs`)

| Parameter | Configured Value | Code Location | Balance Rationale |
| --- | --- | --- | --- |
| Max Health | `220 HP` (`"Aqqil [Defensive]"`) | `DefensiveGuardAI.cs:102` | Toughest single-room defender (vs. `180 HP` Search/Boss) |
| Weapon Type | `WeaponType.Combat_Shotgun` | `DefensiveGuardAI.cs:109` | Close-range Room 4 anchor weapon |
| Pellet Damage | `12 HP` per pellet (`6` pellets) | `DefensiveGuardAI.cs:109` | `72 HP` theoretical max burst before spread/cover |
| Fire Cooldown | `1.4 s` (`1` burst/shot) | `DefensiveGuardAI.cs:109` | Deliberate cadence allowing peek-and-cover counterplay |
| Max Firing Range | `10.0 m` (`3.8 deg` spread) | `DefensiveGuardAI.cs:109` | Matches `sightDistance = 10 m` & `5 m` policy peak |
| Magazine / Reload | `4` blasts / `1.4 s` reload | `GuardShooting.cs:34, 508` | Clear `1.4 s` tactical reload window to push or loot |
| Reaction Delay | `0.35 s` aim prep | `GuardShooting.cs:28` | Prevents instant frame-0 shot on doorway peek |
| Player Pickup Config | `12 x 6`, `0.80 s`, `8/24`, `4.5 deg`, `28 m` | `WeaponConfig.cs:97` | Higher sustained RoF & longer reach once looted |

### 5.2 Distance vs. 6-Pellet Expected Damage (`3.8 deg` Guard Spread)

| Engagement Distance | Typical Pellets on Target | Guard Body Damage | Player Pickup Headshot (`2x`) | Encounter Impact |
| --- | --- | --- | --- | --- |
| `1.0 m` | `6 / 6` | `72 HP` | `144 HP (pickup)` | Point-blank breach; full 6-pellet cluster |
| `1.5 m` | `6 / 6` | `72 HP` | `144 HP (pickup)` | Vault over center barrier; guaranteed full spread |
| `2.0 m` | `6 / 6` | `72 HP` | `144 HP (pickup)` | Tight barrier vault distance; severe burst |
| `2.5 m` | `5–6 / 6` | `60–72 HP` | `120–144 HP (pickup)` | Aggressive flank; slight peripheral edge pellet variance |
| `3.0 m` | `5 / 6` | `60 HP` | `120 HP (pickup)` | Close barrier duel; high threat |
| `4.0 m` | `4–5 / 6` | `48–60 HP` | `96–120 HP (pickup)` | Approaching policy optimum |
| `5.0 m` | `4 / 6` | `48 HP` | `96 HP (pickup)` | Optimal policy peak (`weaponRange = 1.000`) |
| `6.0 m` | `3–4 / 6` | `36–48 HP` | `72–96 HP (pickup)` | Barrier-to-aisle exchange |
| `7.0 m` | `3 / 6` | `36 HP` | `72 HP (pickup)` | Medium Room 4 spacing across barriers |
| `8.0 m` | `2–3 / 6` | `24–36 HP` | `48–72 HP (pickup)` | Outer defensive ring engagement |
| `8.5 m` | `2 / 6` | `24 HP` | `48 HP (pickup)` | Near Room 4 `leashDistance` (`9.0 m`) edge |
| `9.5 m` | `1–2 / 6` | `12–24 HP` | `24–48 HP (pickup)` | Fringe deterrent near west transit aisle |
| `10.0 m` | `1 / 6` | `12 HP` | `24 HP (pickup)` | Exact guard `maxFiringRange` (`10.0 m`) limit |
| `12.0 m` | `0 / 6 (Guard)` | `0 HP` | `12–24 HP (pickup)` | Beyond guard `10 m` range; player pickup reaches `28 m` |

### 5.3 Time-to-Kill (TTK) & Headshot Balance

| Matchup | Shot Damage | Shots to Kill (`220 HP` / `100 HP`) | Time-to-Kill | Tactical Note |
| --- | --- | --- | --- | --- |
| Player `MP5_SMG` (Body) -> Aqqil | `18 HP` (`0.11 s`) | `13` hits | `1.32 s` | Requires 2+ peeks against barrier cover |
| Player `MP5_SMG` (Head `2x`) -> Aqqil | `36 HP` (`0.11 s`) | `7` hits | `0.66 s` | Rewards precision headshots over `1.12 m` barrier |
| Player `MP5_SMG` (50/50 Mixed) -> Aqqil | `27 HP` avg | `9` hits | `0.88 s` | Balanced duel duration behind barriers |
| Aqqil `Combat_Shotgun` -> Player (`100 HP`) | `36–60 HP` (`1.4 s`) | `2–3` blasts | `1.40–2.80 s` | Punishes standing exposed; avoidable via cover/dash |
| Player Looted `Combat_Shotgun` -> Guard (`220 HP`) | `48–96 HP` (`0.80 s`) | `3–5` blasts | `1.60–3.20 s` | High reward in Room 5 finale after clearing Route B |

- **Defeat Drop (`GuardShooting.HandleDeath`):** Spawns `WeaponPickup` for `Combat_Shotgun`
  (`8` mag / `12` reserve, `12 x 6` pellets, `24 dmg` per head pellet, `N_26P.wav` report).
<!-- END:R4_SHOTGUN_BALANCE -->
<!-- SECTION:R4_TRAPS_AND_CARD_B -->
## 6. Room 4 Shock Pad Trap Placement & Access Card B Defense Flow

### 6.1 `Room4_Shock` (`WarehouseTrap.cs` & `SetupWarehouseTrapsAndSquad.cs`)

- **Exact Coordinates:** `Room4_Shock` placed at `(17.0, 0.0, 5.0)`, positioned right beside
  `Defensive_Barrier_Center` (`16.0, 0.0, 5.0`) along the central-east lane.
- **Telegraphed Lifecycle:**

| Phase | Visual Color | Duration | Damage / AI Effect |
| --- | --- | --- | --- |
| `Ready` | Blue plate | Until overlap | Trigger-only `BoxCollider` (`isTrigger = true`); zero NavMesh block |
| `Warning` | Amber plate | `1.25 s` | Emits `SoundType.Alarm` (`28.0 m`) -> triggers `HandleDisturbance` |
| `Active` | Red plate | `1.00 s` | Deals `15 HP` pulse every `0.5 s` to `Health.IsPlayer` (guards immune) |
| `Cooldown` | Grey plate | `5.00 s` | Safe crossing until `ResetTrap()` returns to `Ready` |

- **Guard Synergy:** `Room4_Shock`'s `28 m` alarm originates at `(17, 0, 5)` — only `3.16 m`
  from Aqqil's anchor `(16, 1.08, 8)`. `HandleDisturbance` shifts Aqqil toward an east-flank
  firing post without exposing hidden live player coordinates.

### 6.2 Room 4 Cover, Alcove, Lighting & Access Card B Inventory

| Object / Prop | World Coordinates | Rotation / Scale | Role in Route B Encounter |
| --- | --- | --- | --- |
| `Defensive_Barrier_West` | `(13.5, 0.0, 5.0)` | `0 deg`, `1.0x` | West cover anchor for player & guard |
| `Defensive_Barrier_Center` | `(16.0, 0.0, 5.0)` | `0 deg`, `1.0x` | Central `1.12 m` half-cover (`cover = 1.0`) |
| `Room4_Shock` | `(17.0, 0.0, 5.0)` | `0 deg`, `1.0x` | Punishes reckless center-east push; `28 m` alarm |
| `Defensive_Barrier_East` | `(20.0, 0.0, 7.5)` | `90 deg Y`, `1.0x` | Shields east desk approach |
| `DeadEnd_Crate_E1` | `(25.5, 0.0, 3.5)` | `0 deg`, `1.2x` | East forklift alcove crate cover |
| `DeadEnd_Crate_E2` | `(25.5, 0.0, 5.0)` | `0 deg`, `1.0x` | Secondary crate silhouette & sight break |
| `DeadEnd_Blender_Rack_E` | `(25.5, 0.0, 7.5)` | `0 deg`, `1.1x` | East bay vertical rack framing |
| `DeadEnd_Partition_East` | `(24.0, 2.0, 1.5)` | `(0.4, 4.0, 5.0)` | Occludes east alcove corner |
| `Terminal_Desk` | `(16.0, 0.5, 10.0)` | `—` | Central objective desk behind barrier line |
| `AccessCard_Pickup_Defensive` | `(16.0, 1.15, 10.0)` | `2.5x` | Grants `Access Card B (Sector 04)`, `Level 3` |
| `ReflectionProbe_Room4_Defensive` | `(16.0, 2.5, 9.0)` | `Box 24x8x22` | Box-projected metallic reflection probe (`128 px`) |
| Overhead Spots (`Warm`/`Cool`) | `Y = 6.5 m` bays | `spotAngle = 115` | `12f` warm objective / `11f` cool surveillance pools |

### 6.3 End-to-End Route B Objective Sequence

1. Enter `Room_4_Defensive` via south doorway (`X [3.5, 6.5], Z = -2.0`).
2. Use `Defensive_Barrier_West` / `Center` while baiting Aqqil's `1.4 s` shotgun reload.
3. Avoid stepping onto `Room4_Shock` at `(17, 0, 5)` during lateral pushes.
4. Collect `AccessCard_Pickup_Defensive` (`Access Card B`) at `(16.0, 1.15, 10.0)` and
   (optionally) swap to the dropped `Combat_Shotgun` pickup.
5. Exit north through Corridor 4->5 (`X ~ 6.5, Z = 17.5..20.0`), breaking Aqqil's `9.0 m`
   leash if still alive, to unlock the Room 5 exit terminal.
<!-- END:R4_TRAPS_AND_CARD_B -->
<!-- SECTION:R4_PLAYMODE_CHECK -->
## 7. PlayMode Verification Checklist (`Room_4_Defensive`)

| Check ID | Scenario | Expected & Verified Behavior | Suite / Reference |
| --- | --- | --- | --- |
| `R4-PM-01` | Gunshot / Alarm near `PostPosition` | `DispatchInspectionPoint` picks complete NavMesh path inside Room 4 & `<= LeashDistance` | `InformedGuardDecisionTests` |
| `R4-PM-02` | Hidden player behind wall | `GuardShooting.HasVisualTarget` stays `false`; no firing without confirmed LOS | `InformedGuardDecisionTests` |
| `R4-PM-03` | Cover & range tradeoff | `cover = 1, weaponRange = 1` outscores exposed `cover = 0, weaponRange = 0` | `InformedGuardDecisionTests` |
| `R4-PM-04` | Objective proximity when idle | `objective = 1` beats `objective = 0` when `hasThreat = false` | `InformedGuardDecisionTests` |
| `R4-PM-05` | Path-cost tradeoff | `pathLength = 1 m` beats `pathLength = 19 m` for equal tactical features | `InformedGuardDecisionTests` |
| `R4-PM-06` | Player exits Room 4 / > 9 m leash | `UpdatePursueIntruderState` triggers `ReturnToPost`, snaps `postFacing (0, 0, -1)` at `<= 0.6 m` | `DefensiveGuardAI.cs` |
| `R4-PM-07` | `Room4_Shock` arming | `1.25 s` amber warning -> `15 HP` active pulse; `28 m` `SoundType.Alarm` alerts Aqqil | `TrapAndSquadTests` |
| `R4-PM-08` | Guard shock immunity | `WarehouseTrap` checks `Health.IsPlayer`; guards ignore shock pulse damage | `TrapAndSquadTests` |
| `R4-PM-09` | Route B Card B escape | `AccessCard_Pickup_Defensive` unlocks Room 5 exit independently of Route A | `PhysicalWalkthroughTests` |
| `R4-PM-10` | Placeholder cleanup | `[Placeholder]_Guard_CharacterMesh` & `Mannequin_Body` destroyed on Awake/Editor fix | `FixRoom4AndSaveScene` |
| `R4-PM-11` | `220 HP` & `Combat_Shotgun` drop | Guard spawns with `220 HP`, fires `12 x 6` pellets, drops shotgun pickup on death | `DefensiveGuardAI.cs` / `GuardShooting.cs` |
| `R4-PM-12` | Switching hysteresis (`+0.04`) | Lateral strafe across `X = 14..18` throttled at `0.75 s` with zero oscillation | `DefensiveGuardAI.cs` |
| `R4-PM-13` | Inspector candidate telemetry | `DecisionCandidates`, `SelectedDecisionTarget`, and magenta target gizmos populate cleanly | `DefensiveGuardAI.OnDrawGizmosSelected` |
| `R4-PM-14` | Wall-panel zero obstacle | `IndustrialServicePanel` under `Cinematic_Visual_Details` leaves corridor NavMesh intact | `ExpandWarehouseLayout.cs` |
| `R4-PM-15` | Card B trigger radius | `4.0 x 4.0 x 4.0` pickup collider registers sprinting collection cleanly | `ExpandWarehouseLayout.cs` |
| `R4-PM-16` | URP SRP Batcher & lighting | Room 4 lightmaps, `ReflectionProbe_Room4_Defensive`, and `ACES` tonemapping active | `VisualQualityCaptureTests` |
<!-- END:R4_PLAYMODE_CHECK -->
<!-- SECTION:R4_VIVA_QA -->
