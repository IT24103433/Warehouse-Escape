# DefensiveGuardAI & DefenseDecisionPolicy Trace

**Owner:** Aqqil (`IT24104314`)
**Scripts:** `Assets/Scripts/DefensiveGuardAI.cs`, `Assets/Scripts/DefenseDecisionPolicy.cs`
**Assigned Sector:** `Room_4_Defensive` (Route B — Access Card B)

<!-- BLOCK:POLICY_INTRO -->
## 1. Scope and Design Goals

The Room 4 defensive guard (`Guard_Aqqil_Defensive`) protects the Access Card B security
terminal (`AccessCard_Pickup_Defensive`) without abandoning Room 4 to chase the player
across the warehouse. Unlike `PatrolGuardAI` (Room 2 circuit) or `SearchGuardAI` (Room 3
belief search), `DefensiveGuardAI` stays anchored to a defensive post, evaluates nearby
cover and shotgun engagement range, and returns to post when the intruder disengages.

### 1.1 Baseline Design Parameters

- **Sight Range / FOV:** `10.0 m` / `90 deg` (`eyeHeight = 0.6 m`)
- **Defense Perimeter:** `defenseRadius = 6.0 m` around `postPosition`
- **Pursuit Leash:** `leashDistance = 8.5 m` (code default) / `9.0 m` (Room 4 scene)
- **Weapon Loadout:** `WeaponType.Combat_Shotgun` (`12 dmg x 6 pellets`, `220 HP`)
- **Policy Module:** `DefenseDecisionPolicy.Score(...)` (pure static utility scorer)
- **Candidate Topology:** `17` points (`1` anchor + `2` concentric `8`-point rings)
<!-- END:POLICY_INTRO -->

<!-- SECTION:STATE_MACHINE -->
## 2. Initial Defensive State Outline (Room 4 Route B)

### 2.1 Conceptual Roles Mapped to `DefensiveGuardState`

- **GUARD (`GuardingPost`):** Hold anchor area around `Terminal_Desk`, run `+/- 28 deg`
  sinusoidal aim sweeps (`inspectionWaitTimer = 2.0–2.5 s`), and inspect local points.
- **ATTACK (`PursueIntruder`):** Triggered when `canSeePlayer && isPlayerInsideTerritory &&
  IsInsideRoom(playerTransform.position)`. Guard engages with `Combat_Shotgun`.
- **RETREAT (`ReturnToPost`):** Triggered when intruder leaves `leashDistance` or Room 4;
  guard navigates back to `postPosition` and resets facing to `postFacing (0, 0, -1)`.

<!-- BLOCK:CHASE_DRAFT -->
### 2.2 Early Simple-Chase Baseline (Draft v0.1 — Pending Replacement)

- `[DRAFT-CHASE-01]` In `PursueIntruder`, directly call `enemyMovement.SetDestination(playerTransform.position)`
  every `0.25 s` without scoring nearby cover.
- `[DRAFT-CHASE-02]` In `GuardingPost`, cycle a fixed 3-waypoint triangle around `(16, 1.08, 8)`:
  `WP_Def_0 = (16, 1.08, 8)`, `WP_Def_1 = (14, 1.08, 6)`, `WP_Def_2 = (18, 1.08, 6)`.
- `[DRAFT-CHASE-03]` On lost line-of-sight, pause `2.5 s` at last player pixel and return straight to anchor.
- `[DRAFT-CHASE-04]` No candidate ring evaluation; no torso/eye ray occlusion scoring.
- `[DRAFT-CHASE-05]` No optimal `5.0 m` shotgun distance balancing (`weaponRange`).
- `[DRAFT-CHASE-06]` No switching bonus or throttle, producing rapid destination resets during strafes.

#### Early Static 3-Waypoint & Direct-Chase Simulation Log (Draft v0.1)

| Tick | Draft State | Target Chosen | Target Coords | Failure Mode Observed |
| --- | --- | --- | --- | --- |
| `0.0 s` | `GuardingPost` | `WP_Def_0` | `(16.0, 1.08, 8.0)` | Exposed front of Terminal_Desk |
| `1.5 s` | `GuardingPost` | `WP_Def_1` | `(14.0, 1.08, 6.0)` | Fixed loop ignores east sound cue |
| `3.0 s` | `GuardingPost` | `WP_Def_2` | `(18.0, 1.08, 6.0)` | Walks past barrier near Room4_Shock area |
| `4.5 s` | `GuardingPost` | `WP_Def_0` | `(16.0, 1.08, 8.0)` | Predictable triangle pattern spotted from doorway |
| `5.5 s` | `PursueIntruder` | `Player_Raw` | `(13.0, 1.00, 3.5)` | Sprints straight across open center lane |
| `6.2 s` | `PursueIntruder` | `Player_Raw` | `(11.8, 1.00, 2.8)` | Ignores Defensive_Barrier_West torso cover |
| `7.0 s` | `PursueIntruder` | `Player_Raw` | `(10.5, 1.00, 2.0)` | Closes to 1.4 m suboptimal range |
| `8.0 s` | `PursueIntruder` | `Player_Raw` | `(9.0, 1.00, 1.2)` | Over-commits toward south doorway choke |
| `9.0 s` | `PursueIntruder` | `Player_Raw` | `(7.5, 1.00, 0.8)` | Leaves AccessCard_Pickup_Defensive desk unguarded |
| `10.0 s` | `PursueIntruder` | `Player_Raw` | `(6.2, 1.00, 0.2)` | Guard trapped near Room 2 corridor doorway |
| `11.0 s` | `ReturnToPost` | `WP_Def_0` | `(16.0, 1.08, 8.0)` | Back turned during entire 12.6 m walk back |
| `12.5 s` | `ReturnToPost` | `WP_Def_0` | `(16.0, 1.08, 8.0)` | Slow return lets player sprint around east rack |
| `14.0 s` | `ReturnToPost` | `WP_Def_0` | `(16.0, 1.08, 8.0)` | Player reaches Card B desk uncontested |
| `15.5 s` | `GuardingPost` | `WP_Def_1` | `(14.0, 1.08, 6.0)` | Resumes fixed triangle without adaptive threat sweep |
| `17.0 s` | `GuardingPost` | `WP_Def_2` | `(18.0, 1.08, 6.0)` | Ignores recent western gunshot sound memory |
<!-- END:CHASE_DRAFT -->
<!-- END:STATE_MACHINE -->
<!-- SECTION:DECISION_POLICY_BASE -->
<!-- SECTION:THREAT_COVER_RANGE -->
<!-- SECTION:LEASH_BOUNDS -->
<!-- SECTION:SWITCHING_BONUS -->
<!-- SECTION:PRESSURE_RETREAT -->
<!-- SECTION:VERIFICATION_LOG -->
