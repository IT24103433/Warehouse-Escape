# Viva Guide — Agent Controller & Room 5 Flanking Guard (Jadavan)

**Role:** Agent Controller & Room 5 Final Encounter (`EnemyMovement.cs`, `FlankingGuardAI.cs`, `BossSquadMember.cs`)  
**Encounter Sector:** Shared Guard Locomotion Layer & Room 5 Loading Hall (`Room_5_Boss`, dual `BossSquadMember`s with `WeaponType.SCAR_Rifle`, `360 HP` max health).

---

## 1. Script Responsibility Map & Shared Dependencies

| Script | Responsibility | Shared Dependencies |
| --- | --- | --- |
| `Assets/Scripts/EnemyMovement.cs` | Single locomotion authority across all guards: `NavMeshAgent` seating (`3.5 m`), `PathComplete` validation, smooth rotation (`320 deg/s`, `0.08 m/s` deadzone), braking (`1.2 m`), and bounded stuck recovery (`maxStuckRetries = 3`) | `NavMeshAgent`, `Health`, `GameManager` |
| `Assets/Scripts/FlankingGuardAI.cs` | Room 5 `Patrol` / `Flank` / `Pursue` / `Search` state machine (`12 m` sight, `90 deg` FOV, `3.8`/`5.2 m/s` speeds, `18` candidate lateral evaluation, `0.9 s` cover firing bursts) | `EnemyMovement`, `GuardPatrol`, `BossSquadMember`, `GuardPerception`, `GuardPerceptionMemory`, `GuardShooting`, `Health`, `HumanoidAnimator` |
| `Assets/Scripts/BossSquadMember.cs` | Dual-boss Room 5 coordination: opposite-side preference (`Left` slot 0 / `Right` slot 1), overlap penalty (`+8` side, `+12` within `3 m`), `0.5 s` throttled sighting broadcasts, `1.3 s` alternating suppression, and solo fallback | `FlankingGuardAI`, `GuardShooting`, `GuardPerceptionMemory`, `Health`, `GameManager` |

---

## 2. Key Technical Points for Viva Defense

### 2.1 Shared `EnemyMovement` Locomotion Authority
- **Zero Competing Rotation:** `Awake()` sets `agent.updateRotation = false` so `ExecuteRotation()` smoothly interpolates between `SetAimTarget()` (combat/scanning) and `agent.desiredVelocity` (travel) without snapping.
- **No Unreachable Fallbacks:** `SetDestination()` calls `agent.CalculatePath()` first and rejects `PathPartial` or `PathInvalid` targets.
- **Bounded Stuck Recovery:** `MonitorProgress()` checks displacement every `1.0 s`; after `1.8 s` of `< 0.12 m` progress while far from the goal, `HandleStuckCondition()` tests up to `3` reachable `2.5 m` NavMesh offsets before safely calling `Stop()`.

### 2.2 Lateral Flanking (`FlankingGuardAI`)
- **Candidate Set (`TrySelectFlankTarget`):** Generates `18` lateral points (`lateralOffsets = [3.5, 5.5, 7.5]` m x `forwardOffsets = [-1.5, 0.0, 1.5]` m x `Left/Right`), filtered by `IsInsideRoom`, `NavMesh.SamplePosition` (`2.0 m`), `PathComplete`, and `maxFlankPathRatio = 2.5`.
- **Direct-Pursuit Rejection:** Rejects routes where `divergenceAngle < 18 deg && lineAngle < 15 deg` so flanks genuinely use Room 5's West/East side aisles (`X = +/-5.5`) around the central cargo stacks.
- **Cost & Hysteresis:** Minimizes `pathLength + |approachAngle - 90| * 0.05 + squadPenalty - hysteresisBonus` (`2.0 m` side stability). Switches to `Pursue` when `distToPlayer <= 3.0 m` (`minFlankPlayerDistance`).

### 2.3 Two-Boss Squad Coordination (`BossSquadMember`)
- **Opposite-Side Pincer & Anti-Stacking:** Slot `0` (`Guard_Jadavan_Boss`) prefers `FlankSide.Left`; Slot `1` (`Guard_Jadavan_Boss_Wingman`) prefers `FlankSide.Right`. `FlankPenalty` adds `+8` for the non-preferred side and `+12` when `< 3 m` from the partner's `SelectedFlankPosition`, while keeping a reachable fallback if one side is blocked.
- **Confirmed-Sighting Sharing (`ShareConfirmedObservation`):** Broadcasts `PlayerTransform.position` at most twice/second (`0.5 s`) only when `HasConfirmedVisualContact()` is true. The receiver enters `Search` via `ReceiveSquadObservation` without gaining `HasVisualTarget` or firing authority.
- **Alternating Suppression (`OwnsSuppressionTurn`):** Alternates `1.3 s` firing windows (`Mathf.FloorToInt(clock / 1.3f) % 2 == slot % 2`), immediately covering a reloading or blind partner (`CanFireNow`). Each shot still requires the shooter's own `GuardShooting.HasClearLineOfSight()` check (blocked by cover or partner guard).
- **Solo Fallback:** When either guard dies (`health.IsDead`), `Partner` becomes `null`, `PreferredSide` reverts to `FlankSide.None`, and `CanFireNow` returns `true` so the survivor fights autonomously.

---

## 3. Live Demo Sequence (2 Minutes)

1. **Flank Selection & Gizmos:** Enter Room 5 (`Z > 23`), select `Guard_Jadavan_Boss` and `Guard_Jadavan_Boss_Wingman` in Scene view -> show blue (Left) vs. magenta (Right) candidate rings, cyan selected flank paths around `Boss_Central_Cargo_Stack_1/2`, and orange direct-equivalent rejections.
2. **Alternating Suppression & Sighting Share:** Step into view of one boss while keeping cover against the second -> show the second guard moving into `Search` (`ReportsReceived` increments) without blind firing, then show `1.3 s` alternating bursts when both have clear LOS.
3. **Solo Fallback & CardReader Escape:** Eliminate one Room 5 boss -> show the survivor immediately releasing side reservations (`PreferredSide = None`) and continuing independent flanking/firing until `CardReader` (`Z = 48.0`) evacuation.
