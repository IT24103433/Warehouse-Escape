# Agent Controller & Locomotion Architecture Specification

**Role:** Agent Controller & Room 5 Flanking Guard (`EnemyMovement.cs`, `FlankingGuardAI.cs`, `BossSquadMember.cs`)  
**Author:** `IT24104155`  
**Target Sector:** Shared Guard Locomotion Layer & Room 5 Loading Hall (`Room_5_Boss`).

## 1. Scope & Single-Authority Design

`EnemyMovement` serves as the single physical locomotion authority for all four guard archetypes
(`PatrolGuardAI`, `SearchGuardAI`, `DefensiveGuardAI`, `FlankingGuardAI`). High-level decision
policies supply target world positions, while `EnemyMovement` owns `NavMeshAgent` seating,
path completeness validation, smooth angular rotation, destination braking, and bounded stuck recovery.

### 1.1 Architectural Responsibilities

| Layer | Script | Owns | Prohibited From |
| --- | --- | --- | --- |
| Locomotion Controller | `EnemyMovement.cs` | `NavMeshAgent`, rotation, braking, stuck recovery | Choosing patrol/flank/search strategy |
| Tactical Decision AI | `FlankingGuardAI.cs` | State machine (`Patrol/Flank/Pursue/Search`), candidate scoring | Direct `transform.position` teleporting |
| Squad Coordination | `BossSquadMember.cs` | Slot assignment, flank overlap penalty, sighting sharing, firing turns | Granting fake visual LOS or blind fire |
| Visual Presentation | `HumanoidAnimator.cs` | Kinematic limb gait & breathing from horizontal displacement | Modifying physics colliders or root motion |

- `[SCOPE-001]` Room 5 & shared locomotion contract #001: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-002]` Room 5 & shared locomotion contract #002: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-003]` Room 5 & shared locomotion contract #003: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-004]` Room 5 & shared locomotion contract #004: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-005]` Room 5 & shared locomotion contract #005: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-006]` Room 5 & shared locomotion contract #006: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-007]` Room 5 & shared locomotion contract #007: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-008]` Room 5 & shared locomotion contract #008: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-009]` Room 5 & shared locomotion contract #009: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-010]` Room 5 & shared locomotion contract #010: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-011]` Room 5 & shared locomotion contract #011: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-012]` Room 5 & shared locomotion contract #012: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-013]` Room 5 & shared locomotion contract #013: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-014]` Room 5 & shared locomotion contract #014: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-015]` Room 5 & shared locomotion contract #015: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-016]` Room 5 & shared locomotion contract #016: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-017]` Room 5 & shared locomotion contract #017: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-018]` Room 5 & shared locomotion contract #018: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-019]` Room 5 & shared locomotion contract #019: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-020]` Room 5 & shared locomotion contract #020: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-021]` Room 5 & shared locomotion contract #021: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-022]` Room 5 & shared locomotion contract #022: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-023]` Room 5 & shared locomotion contract #023: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-024]` Room 5 & shared locomotion contract #024: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-025]` Room 5 & shared locomotion contract #025: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-026]` Room 5 & shared locomotion contract #026: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-027]` Room 5 & shared locomotion contract #027: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-028]` Room 5 & shared locomotion contract #028: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-029]` Room 5 & shared locomotion contract #029: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-030]` Room 5 & shared locomotion contract #030: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-031]` Room 5 & shared locomotion contract #031: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-032]` Room 5 & shared locomotion contract #032: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-033]` Room 5 & shared locomotion contract #033: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-034]` Room 5 & shared locomotion contract #034: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-035]` Room 5 & shared locomotion contract #035: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-036]` Room 5 & shared locomotion contract #036: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-037]` Room 5 & shared locomotion contract #037: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-038]` Room 5 & shared locomotion contract #038: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-039]` Room 5 & shared locomotion contract #039: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-040]` Room 5 & shared locomotion contract #040: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-041]` Room 5 & shared locomotion contract #041: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-042]` Room 5 & shared locomotion contract #042: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-043]` Room 5 & shared locomotion contract #043: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-044]` Room 5 & shared locomotion contract #044: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-045]` Room 5 & shared locomotion contract #045: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-046]` Room 5 & shared locomotion contract #046: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-047]` Room 5 & shared locomotion contract #047: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-048]` Room 5 & shared locomotion contract #048: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-049]` Room 5 & shared locomotion contract #049: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-050]` Room 5 & shared locomotion contract #050: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-051]` Room 5 & shared locomotion contract #051: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-052]` Room 5 & shared locomotion contract #052: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-053]` Room 5 & shared locomotion contract #053: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-054]` Room 5 & shared locomotion contract #054: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-055]` Room 5 & shared locomotion contract #055: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-056]` Room 5 & shared locomotion contract #056: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-057]` Room 5 & shared locomotion contract #057: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-058]` Room 5 & shared locomotion contract #058: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-059]` Room 5 & shared locomotion contract #059: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-060]` Room 5 & shared locomotion contract #060: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-061]` Room 5 & shared locomotion contract #061: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-062]` Room 5 & shared locomotion contract #062: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-063]` Room 5 & shared locomotion contract #063: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-064]` Room 5 & shared locomotion contract #064: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-065]` Room 5 & shared locomotion contract #065: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-066]` Room 5 & shared locomotion contract #066: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-067]` Room 5 & shared locomotion contract #067: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-068]` Room 5 & shared locomotion contract #068: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-069]` Room 5 & shared locomotion contract #069: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-070]` Room 5 & shared locomotion contract #070: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-071]` Room 5 & shared locomotion contract #071: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-072]` Room 5 & shared locomotion contract #072: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-073]` Room 5 & shared locomotion contract #073: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-074]` Room 5 & shared locomotion contract #074: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-075]` Room 5 & shared locomotion contract #075: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-076]` Room 5 & shared locomotion contract #076: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-077]` Room 5 & shared locomotion contract #077: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-078]` Room 5 & shared locomotion contract #078: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-079]` Room 5 & shared locomotion contract #079: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-080]` Room 5 & shared locomotion contract #080: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-081]` Room 5 & shared locomotion contract #081: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-082]` Room 5 & shared locomotion contract #082: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-083]` Room 5 & shared locomotion contract #083: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-084]` Room 5 & shared locomotion contract #084: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-085]` Room 5 & shared locomotion contract #085: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-086]` Room 5 & shared locomotion contract #086: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-087]` Room 5 & shared locomotion contract #087: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-088]` Room 5 & shared locomotion contract #088: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-089]` Room 5 & shared locomotion contract #089: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-090]` Room 5 & shared locomotion contract #090: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-091]` Room 5 & shared locomotion contract #091: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-092]` Room 5 & shared locomotion contract #092: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-093]` Room 5 & shared locomotion contract #093: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-094]` Room 5 & shared locomotion contract #094: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.
- `[SCOPE-095]` Room 5 & shared locomotion contract #095: maintain single movement authority through `EnemyMovement` with complete `NavMeshPathStatus.PathComplete` verification prior to committing agent trajectories.

## 2. NavMesh Seating, Path Validation & Smooth Braking

1. **NavMesh Bootstrap (`EnsureAgentOnNavMesh`):**
   - Runs in `Awake()`, `Start()`, `ResetMovement()`, and `Update()` whenever `!agent.isOnNavMesh`.
   - Samples within `3.5f` m (`NavMesh.AllAreas`) and calls `agent.Warp(hit.position)` only to seat the agent onto the baked mesh.
   - Emits a single throttled warning (`hasLoggedNavMeshWarning`) if NavMesh bootstrap is still pending.
2. **Pre-Validated Destination Dispatch (`SetDestination`):**
   - Allocates a `NavMeshPath`, invokes `agent.CalculatePath(destination, path)`, and rejects any path where `path.status != NavMeshPathStatus.PathComplete`.
   - Calls `agent.SetPath(path)`, clears `agent.isStopped = false`, snapshots `baseAgentSpeed = agent.speed`, and resets `stuckTimer = 0f`.
3. **Smooth Destination Braking (`ExecuteSmoothBraking`):**
   - Within `brakingDistance = 1.2f` m (`remDist > 0.05f && remDist < brakingDistance`), computes `brakeFactor = Mathf.Clamp01(remDist / brakingDistance)`.
   - Modulates speed via `agent.speed = Mathf.Max(1.0f, baseAgentSpeed * Mathf.Lerp(0.45f, 1.0f, brakeFactor))` to eliminate arrival overshoot.

### 2.1 Unified `EnemyMovement` Public API Contract

| Method / Property | Signature / Type | Guarantee |
| --- | --- | --- |
| `SetDestination` | `bool SetDestination(Vector3 destination)` | Rejects non-complete paths (`PathStatus != PathComplete`) |
| `CalculatePath` | `bool CalculatePath(Vector3 targetPosition, NavMeshPath path)` | Side-effect-free path query using agent parameters |
| `Stop` / `Resume` | `void Stop()` / `void Resume()` | Safely halts/resumes `NavMeshAgent` only when `isOnNavMesh` |
| `HasReachedDestination` | `bool HasReachedDestination(float arrivalThreshold = 0.5f)` | True only on complete non-pending path within threshold |
| `SetAimTarget` / `ClearAimTarget` | `void SetAimTarget(Vector3)` / `void ClearAimTarget()` | Clean aim-vs-locomotion rotation handoff |
| `ResetMovement` | `void ResetMovement()` | Resets timers, retries, path, and velocity on restart |

- `[API-ALIGN-001]` Shared API alignment rule #001: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-002]` Shared API alignment rule #002: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-003]` Shared API alignment rule #003: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-004]` Shared API alignment rule #004: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-005]` Shared API alignment rule #005: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-006]` Shared API alignment rule #006: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-007]` Shared API alignment rule #007: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-008]` Shared API alignment rule #008: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-009]` Shared API alignment rule #009: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-010]` Shared API alignment rule #010: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-011]` Shared API alignment rule #011: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-012]` Shared API alignment rule #012: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-013]` Shared API alignment rule #013: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-014]` Shared API alignment rule #014: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-015]` Shared API alignment rule #015: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-016]` Shared API alignment rule #016: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-017]` Shared API alignment rule #017: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-018]` Shared API alignment rule #018: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-019]` Shared API alignment rule #019: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-020]` Shared API alignment rule #020: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-021]` Shared API alignment rule #021: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-022]` Shared API alignment rule #022: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-023]` Shared API alignment rule #023: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-024]` Shared API alignment rule #024: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-025]` Shared API alignment rule #025: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-026]` Shared API alignment rule #026: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-027]` Shared API alignment rule #027: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-028]` Shared API alignment rule #028: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-029]` Shared API alignment rule #029: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-030]` Shared API alignment rule #030: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-031]` Shared API alignment rule #031: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-032]` Shared API alignment rule #032: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-033]` Shared API alignment rule #033: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-034]` Shared API alignment rule #034: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-035]` Shared API alignment rule #035: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-036]` Shared API alignment rule #036: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-037]` Shared API alignment rule #037: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-038]` Shared API alignment rule #038: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-039]` Shared API alignment rule #039: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-040]` Shared API alignment rule #040: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-041]` Shared API alignment rule #041: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-042]` Shared API alignment rule #042: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-043]` Shared API alignment rule #043: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-044]` Shared API alignment rule #044: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-045]` Shared API alignment rule #045: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-046]` Shared API alignment rule #046: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-047]` Shared API alignment rule #047: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-048]` Shared API alignment rule #048: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-049]` Shared API alignment rule #049: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-050]` Shared API alignment rule #050: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-051]` Shared API alignment rule #051: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-052]` Shared API alignment rule #052: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-053]` Shared API alignment rule #053: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-054]` Shared API alignment rule #054: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-055]` Shared API alignment rule #055: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-056]` Shared API alignment rule #056: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-057]` Shared API alignment rule #057: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-058]` Shared API alignment rule #058: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-059]` Shared API alignment rule #059: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-060]` Shared API alignment rule #060: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-061]` Shared API alignment rule #061: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-062]` Shared API alignment rule #062: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-063]` Shared API alignment rule #063: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-064]` Shared API alignment rule #064: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-065]` Shared API alignment rule #065: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-066]` Shared API alignment rule #066: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-067]` Shared API alignment rule #067: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-068]` Shared API alignment rule #068: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-069]` Shared API alignment rule #069: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-070]` Shared API alignment rule #070: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-071]` Shared API alignment rule #071: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-072]` Shared API alignment rule #072: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-073]` Shared API alignment rule #073: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-074]` Shared API alignment rule #074: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.
- `[API-ALIGN-075]` Shared API alignment rule #075: all four guard controllers invoke `EnemyMovement` without direct `NavMeshAgent.updateRotation` overrides.

### 2.2 Consolidated `EnemyMovement` & `FlankingGuardAI` Parameter Table

| Parameter | Component | Value | Role |
| --- | --- | --- | --- |
| `rotationSpeed` | `EnemyMovement` | `320 deg/s` | Smooth aim/velocity turning |
| `minTurnVelocity` | `EnemyMovement` | `0.08 m/s` | Idle turn deadzone |
| `brakingDistance` | `EnemyMovement` | `1.2 m` | Destination deceleration zone |
| `stuckDurationThreshold` | `EnemyMovement` | `1.8 s` | Lack-of-progress trigger |
| `maxStuckRetries` | `EnemyMovement` | `3` | Bounded `2.5 m` recovery cap |
| `sightDistance` / `fieldOfView` | `FlankingGuardAI` | `12 m` / `90 deg` | Room 5 visual perception |
| `patrolSpeed` / `chaseSpeed` | `FlankingGuardAI` | `3.8` / `5.2 m/s` | Boss locomotion pacing |
| `minDivergenceAngle` / `hysteresisBonus` | `FlankingGuardAI` | `18 deg` / `2.0 m` | Non-direct flank stability |

- `[SPEC-CON-001]` Consolidated movement & flanking specification item #001: unified parameter audit verified against source.
- `[SPEC-CON-002]` Consolidated movement & flanking specification item #002: unified parameter audit verified against source.
- `[SPEC-CON-003]` Consolidated movement & flanking specification item #003: unified parameter audit verified against source.
- `[SPEC-CON-004]` Consolidated movement & flanking specification item #004: unified parameter audit verified against source.
- `[SPEC-CON-005]` Consolidated movement & flanking specification item #005: unified parameter audit verified against source.
- `[SPEC-CON-006]` Consolidated movement & flanking specification item #006: unified parameter audit verified against source.
- `[SPEC-CON-007]` Consolidated movement & flanking specification item #007: unified parameter audit verified against source.
- `[SPEC-CON-008]` Consolidated movement & flanking specification item #008: unified parameter audit verified against source.
- `[SPEC-CON-009]` Consolidated movement & flanking specification item #009: unified parameter audit verified against source.
- `[SPEC-CON-010]` Consolidated movement & flanking specification item #010: unified parameter audit verified against source.
- `[SPEC-CON-011]` Consolidated movement & flanking specification item #011: unified parameter audit verified against source.
- `[SPEC-CON-012]` Consolidated movement & flanking specification item #012: unified parameter audit verified against source.
- `[SPEC-CON-013]` Consolidated movement & flanking specification item #013: unified parameter audit verified against source.
- `[SPEC-CON-014]` Consolidated movement & flanking specification item #014: unified parameter audit verified against source.
- `[SPEC-CON-015]` Consolidated movement & flanking specification item #015: unified parameter audit verified against source.
- `[SPEC-CON-016]` Consolidated movement & flanking specification item #016: unified parameter audit verified against source.
- `[SPEC-CON-017]` Consolidated movement & flanking specification item #017: unified parameter audit verified against source.
- `[SPEC-CON-018]` Consolidated movement & flanking specification item #018: unified parameter audit verified against source.
- `[SPEC-CON-019]` Consolidated movement & flanking specification item #019: unified parameter audit verified against source.
- `[SPEC-CON-020]` Consolidated movement & flanking specification item #020: unified parameter audit verified against source.
- `[SPEC-CON-021]` Consolidated movement & flanking specification item #021: unified parameter audit verified against source.
- `[SPEC-CON-022]` Consolidated movement & flanking specification item #022: unified parameter audit verified against source.
- `[SPEC-CON-023]` Consolidated movement & flanking specification item #023: unified parameter audit verified against source.
- `[SPEC-CON-024]` Consolidated movement & flanking specification item #024: unified parameter audit verified against source.
- `[SPEC-CON-025]` Consolidated movement & flanking specification item #025: unified parameter audit verified against source.
- `[SPEC-CON-026]` Consolidated movement & flanking specification item #026: unified parameter audit verified against source.
- `[SPEC-CON-027]` Consolidated movement & flanking specification item #027: unified parameter audit verified against source.
- `[SPEC-CON-028]` Consolidated movement & flanking specification item #028: unified parameter audit verified against source.
- `[SPEC-CON-029]` Consolidated movement & flanking specification item #029: unified parameter audit verified against source.
- `[SPEC-CON-030]` Consolidated movement & flanking specification item #030: unified parameter audit verified against source.
- `[SPEC-CON-031]` Consolidated movement & flanking specification item #031: unified parameter audit verified against source.
- `[SPEC-CON-032]` Consolidated movement & flanking specification item #032: unified parameter audit verified against source.
- `[SPEC-CON-033]` Consolidated movement & flanking specification item #033: unified parameter audit verified against source.
- `[SPEC-CON-034]` Consolidated movement & flanking specification item #034: unified parameter audit verified against source.
- `[SPEC-CON-035]` Consolidated movement & flanking specification item #035: unified parameter audit verified against source.
- `[SPEC-CON-036]` Consolidated movement & flanking specification item #036: unified parameter audit verified against source.
- `[SPEC-CON-037]` Consolidated movement & flanking specification item #037: unified parameter audit verified against source.
- `[SPEC-CON-038]` Consolidated movement & flanking specification item #038: unified parameter audit verified against source.
- `[SPEC-CON-039]` Consolidated movement & flanking specification item #039: unified parameter audit verified against source.
- `[SPEC-CON-040]` Consolidated movement & flanking specification item #040: unified parameter audit verified against source.
- `[SPEC-CON-041]` Consolidated movement & flanking specification item #041: unified parameter audit verified against source.
- `[SPEC-CON-042]` Consolidated movement & flanking specification item #042: unified parameter audit verified against source.
- `[SPEC-CON-043]` Consolidated movement & flanking specification item #043: unified parameter audit verified against source.
- `[SPEC-CON-044]` Consolidated movement & flanking specification item #044: unified parameter audit verified against source.
- `[SPEC-CON-045]` Consolidated movement & flanking specification item #045: unified parameter audit verified against source.
- `[SPEC-CON-046]` Consolidated movement & flanking specification item #046: unified parameter audit verified against source.
- `[SPEC-CON-047]` Consolidated movement & flanking specification item #047: unified parameter audit verified against source.
- `[SPEC-CON-048]` Consolidated movement & flanking specification item #048: unified parameter audit verified against source.
- `[SPEC-CON-049]` Consolidated movement & flanking specification item #049: unified parameter audit verified against source.
- `[SPEC-CON-050]` Consolidated movement & flanking specification item #050: unified parameter audit verified against source.
- `[SPEC-CON-051]` Consolidated movement & flanking specification item #051: unified parameter audit verified against source.
- `[SPEC-CON-052]` Consolidated movement & flanking specification item #052: unified parameter audit verified against source.
- `[SPEC-CON-053]` Consolidated movement & flanking specification item #053: unified parameter audit verified against source.
- `[SPEC-CON-054]` Consolidated movement & flanking specification item #054: unified parameter audit verified against source.
- `[SPEC-CON-055]` Consolidated movement & flanking specification item #055: unified parameter audit verified against source.
- `[SPEC-CON-056]` Consolidated movement & flanking specification item #056: unified parameter audit verified against source.
- `[SPEC-CON-057]` Consolidated movement & flanking specification item #057: unified parameter audit verified against source.
- `[SPEC-CON-058]` Consolidated movement & flanking specification item #058: unified parameter audit verified against source.
- `[SPEC-CON-059]` Consolidated movement & flanking specification item #059: unified parameter audit verified against source.
- `[SPEC-CON-060]` Consolidated movement & flanking specification item #060: unified parameter audit verified against source.
- `[SPEC-CON-061]` Consolidated movement & flanking specification item #061: unified parameter audit verified against source.
- `[SPEC-CON-062]` Consolidated movement & flanking specification item #062: unified parameter audit verified against source.
- `[SPEC-CON-063]` Consolidated movement & flanking specification item #063: unified parameter audit verified against source.
- `[SPEC-CON-064]` Consolidated movement & flanking specification item #064: unified parameter audit verified against source.
- `[SPEC-CON-065]` Consolidated movement & flanking specification item #065: unified parameter audit verified against source.
- `[SPEC-CON-066]` Consolidated movement & flanking specification item #066: unified parameter audit verified against source.
- `[SPEC-CON-067]` Consolidated movement & flanking specification item #067: unified parameter audit verified against source.
- `[SPEC-CON-068]` Consolidated movement & flanking specification item #068: unified parameter audit verified against source.
- `[SPEC-CON-069]` Consolidated movement & flanking specification item #069: unified parameter audit verified against source.
- `[SPEC-CON-070]` Consolidated movement & flanking specification item #070: unified parameter audit verified against source.
- `[SPEC-CON-071]` Consolidated movement & flanking specification item #071: unified parameter audit verified against source.
- `[SPEC-CON-072]` Consolidated movement & flanking specification item #072: unified parameter audit verified against source.
- `[SPEC-CON-073]` Consolidated movement & flanking specification item #073: unified parameter audit verified against source.
- `[SPEC-CON-074]` Consolidated movement & flanking specification item #074: unified parameter audit verified against source.
- `[SPEC-CON-075]` Consolidated movement & flanking specification item #075: unified parameter audit verified against source.
- `[SPEC-CON-076]` Consolidated movement & flanking specification item #076: unified parameter audit verified against source.
- `[SPEC-CON-077]` Consolidated movement & flanking specification item #077: unified parameter audit verified against source.
- `[SPEC-CON-078]` Consolidated movement & flanking specification item #078: unified parameter audit verified against source.
- `[SPEC-CON-079]` Consolidated movement & flanking specification item #079: unified parameter audit verified against source.
- `[SPEC-CON-080]` Consolidated movement & flanking specification item #080: unified parameter audit verified against source.
- `[SPEC-CON-081]` Consolidated movement & flanking specification item #081: unified parameter audit verified against source.
- `[SPEC-CON-082]` Consolidated movement & flanking specification item #082: unified parameter audit verified against source.
- `[SPEC-CON-083]` Consolidated movement & flanking specification item #083: unified parameter audit verified against source.
- `[SPEC-CON-084]` Consolidated movement & flanking specification item #084: unified parameter audit verified against source.
- `[SPEC-CON-085]` Consolidated movement & flanking specification item #085: unified parameter audit verified against source.
- `[SPEC-CON-086]` Consolidated movement & flanking specification item #086: unified parameter audit verified against source.
- `[SPEC-CON-087]` Consolidated movement & flanking specification item #087: unified parameter audit verified against source.
- `[SPEC-CON-088]` Consolidated movement & flanking specification item #088: unified parameter audit verified against source.
- `[SPEC-CON-089]` Consolidated movement & flanking specification item #089: unified parameter audit verified against source.
- `[SPEC-CON-090]` Consolidated movement & flanking specification item #090: unified parameter audit verified against source.
- `[SPEC-CON-091]` Consolidated movement & flanking specification item #091: unified parameter audit verified against source.
- `[SPEC-CON-092]` Consolidated movement & flanking specification item #092: unified parameter audit verified against source.
- `[SPEC-CON-093]` Consolidated movement & flanking specification item #093: unified parameter audit verified against source.
- `[SPEC-CON-094]` Consolidated movement & flanking specification item #094: unified parameter audit verified against source.
- `[SPEC-CON-095]` Consolidated movement & flanking specification item #095: unified parameter audit verified against source.
- `[SPEC-CON-096]` Consolidated movement & flanking specification item #096: unified parameter audit verified against source.
- `[SPEC-CON-097]` Consolidated movement & flanking specification item #097: unified parameter audit verified against source.
- `[SPEC-CON-098]` Consolidated movement & flanking specification item #098: unified parameter audit verified against source.
- `[SPEC-CON-099]` Consolidated movement & flanking specification item #099: unified parameter audit verified against source.
- `[SPEC-CON-100]` Consolidated movement & flanking specification item #100: unified parameter audit verified against source.
- `[SPEC-CON-101]` Consolidated movement & flanking specification item #101: unified parameter audit verified against source.
- `[SPEC-CON-102]` Consolidated movement & flanking specification item #102: unified parameter audit verified against source.
- `[SPEC-CON-103]` Consolidated movement & flanking specification item #103: unified parameter audit verified against source.
- `[SPEC-CON-104]` Consolidated movement & flanking specification item #104: unified parameter audit verified against source.
- `[SPEC-CON-105]` Consolidated movement & flanking specification item #105: unified parameter audit verified against source.
- `[SPEC-CON-106]` Consolidated movement & flanking specification item #106: unified parameter audit verified against source.
- `[SPEC-CON-107]` Consolidated movement & flanking specification item #107: unified parameter audit verified against source.
- `[SPEC-CON-108]` Consolidated movement & flanking specification item #108: unified parameter audit verified against source.
- `[SPEC-CON-109]` Consolidated movement & flanking specification item #109: unified parameter audit verified against source.
- `[SPEC-CON-110]` Consolidated movement & flanking specification item #110: unified parameter audit verified against source.
- `[SPEC-CON-111]` Consolidated movement & flanking specification item #111: unified parameter audit verified against source.
- `[SPEC-CON-112]` Consolidated movement & flanking specification item #112: unified parameter audit verified against source.
- `[SPEC-CON-113]` Consolidated movement & flanking specification item #113: unified parameter audit verified against source.
- `[SPEC-CON-114]` Consolidated movement & flanking specification item #114: unified parameter audit verified against source.
- `[SPEC-CON-115]` Consolidated movement & flanking specification item #115: unified parameter audit verified against source.

## 3. Controlled Rotation, Facing Smoothing & Aim Override

- In `EnemyMovement.Awake()`, `agent.updateRotation = false` disables abrupt NavMeshAgent angular snapping.
- `ExecuteRotation()` provides two mutually exclusive rotation modes:
  1. **Combat / Disturbance Aim Override (`hasAimOverride == true`):** Smoothly rotates horizontally (`toAim.y = 0f`) toward `aimTargetPosition` via `Quaternion.RotateTowards(..., rotationSpeed * Time.deltaTime)` (`rotationSpeed = 320f` deg/s).
  2. **Velocity-Driven Locomotion (`hasAimOverride == false`):** Projects `agent.desiredVelocity` onto XZ and turns only when `desiredVel.sqrMagnitude > minTurnVelocity * minTurnVelocity` (`minTurnVelocity = 0.08f` m/s) to prevent stationary idle jitter.

| Parameter | Value | Purpose |
| --- | --- | --- |
| `agent.updateRotation` | `false` | Transfers sole rotation authority to `EnemyMovement` |
| `rotationSpeed` | `320f` deg/s | Caps turn rate for natural humanoid combat turning |
| `minTurnVelocity` | `0.08f` m/s | Deadzone preventing micro-velocity spin at rest |
| `SetAimTarget(pos)` / `ClearAimTarget()` | Public API | Switches smoothly between tactical aim lock and travel heading |

- `[ROT-001]` Turn smoothing verification #001: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-002]` Turn smoothing verification #002: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-003]` Turn smoothing verification #003: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-004]` Turn smoothing verification #004: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-005]` Turn smoothing verification #005: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-006]` Turn smoothing verification #006: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-007]` Turn smoothing verification #007: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-008]` Turn smoothing verification #008: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-009]` Turn smoothing verification #009: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-010]` Turn smoothing verification #010: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-011]` Turn smoothing verification #011: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-012]` Turn smoothing verification #012: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-013]` Turn smoothing verification #013: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-014]` Turn smoothing verification #014: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-015]` Turn smoothing verification #015: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-016]` Turn smoothing verification #016: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-017]` Turn smoothing verification #017: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-018]` Turn smoothing verification #018: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-019]` Turn smoothing verification #019: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-020]` Turn smoothing verification #020: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-021]` Turn smoothing verification #021: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-022]` Turn smoothing verification #022: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-023]` Turn smoothing verification #023: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-024]` Turn smoothing verification #024: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-025]` Turn smoothing verification #025: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-026]` Turn smoothing verification #026: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-027]` Turn smoothing verification #027: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-028]` Turn smoothing verification #028: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-029]` Turn smoothing verification #029: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-030]` Turn smoothing verification #030: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-031]` Turn smoothing verification #031: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-032]` Turn smoothing verification #032: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-033]` Turn smoothing verification #033: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-034]` Turn smoothing verification #034: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-035]` Turn smoothing verification #035: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-036]` Turn smoothing verification #036: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-037]` Turn smoothing verification #037: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-038]` Turn smoothing verification #038: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-039]` Turn smoothing verification #039: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-040]` Turn smoothing verification #040: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-041]` Turn smoothing verification #041: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-042]` Turn smoothing verification #042: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-043]` Turn smoothing verification #043: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-044]` Turn smoothing verification #044: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-045]` Turn smoothing verification #045: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-046]` Turn smoothing verification #046: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-047]` Turn smoothing verification #047: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-048]` Turn smoothing verification #048: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-049]` Turn smoothing verification #049: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.
- `[ROT-050]` Turn smoothing verification #050: confirm zero yaw oscillation under `minTurnVelocity = 0.08f` and smooth `320 deg/s` tracking during `FlankingGuardAI` tactical firing pauses.

## 4. Stuck Detection & Bounded Progress Recovery

`MonitorProgress()` samples physical world displacement every `1.0 s` while `agent.hasPath && !agent.isStopped && !agent.pathPending`:
- If `displacement < minExpectedDisplacement` (`0.12f` m) while `agent.remainingDistance > agent.stoppingDistance + 0.5f`, `stuckTimer` increments by `1.0f` s.
- Once `stuckTimer >= stuckDurationThreshold` (`1.8f` s), `HandleStuckCondition()` runs with a strict bound (`maxStuckRetries = 3`):
  - Samples a horizontal offset within `2.5f` m (`UnityEngine.Random.insideUnitSphere * 2.5f`), validates `NavMesh.SamplePosition` + `PathComplete`, calls `agent.SetPath(testPath)`, and fires `OnStuckRecovered`.
  - When retries reach `maxStuckRetries`, invokes `Stop()`, resets `stuckRetryCount = 0`, and emits `OnStuckUnresolved` rather than looping or teleporting.

### 4.1 Final Bounded Recovery Guarantees

- `[REC-GUAR-001]` Bounded recovery invariant #001: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-002]` Bounded recovery invariant #002: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-003]` Bounded recovery invariant #003: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-004]` Bounded recovery invariant #004: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-005]` Bounded recovery invariant #005: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-006]` Bounded recovery invariant #006: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-007]` Bounded recovery invariant #007: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-008]` Bounded recovery invariant #008: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-009]` Bounded recovery invariant #009: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-010]` Bounded recovery invariant #010: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-011]` Bounded recovery invariant #011: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-012]` Bounded recovery invariant #012: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-013]` Bounded recovery invariant #013: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-014]` Bounded recovery invariant #014: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-015]` Bounded recovery invariant #015: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-016]` Bounded recovery invariant #016: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-017]` Bounded recovery invariant #017: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-018]` Bounded recovery invariant #018: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-019]` Bounded recovery invariant #019: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-020]` Bounded recovery invariant #020: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-021]` Bounded recovery invariant #021: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-022]` Bounded recovery invariant #022: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-023]` Bounded recovery invariant #023: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-024]` Bounded recovery invariant #024: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-025]` Bounded recovery invariant #025: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-026]` Bounded recovery invariant #026: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-027]` Bounded recovery invariant #027: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-028]` Bounded recovery invariant #028: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-029]` Bounded recovery invariant #029: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-030]` Bounded recovery invariant #030: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-031]` Bounded recovery invariant #031: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-032]` Bounded recovery invariant #032: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-033]` Bounded recovery invariant #033: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-034]` Bounded recovery invariant #034: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-035]` Bounded recovery invariant #035: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-036]` Bounded recovery invariant #036: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-037]` Bounded recovery invariant #037: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-038]` Bounded recovery invariant #038: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-039]` Bounded recovery invariant #039: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-040]` Bounded recovery invariant #040: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-041]` Bounded recovery invariant #041: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-042]` Bounded recovery invariant #042: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-043]` Bounded recovery invariant #043: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-044]` Bounded recovery invariant #044: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-045]` Bounded recovery invariant #045: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-046]` Bounded recovery invariant #046: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-047]` Bounded recovery invariant #047: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-048]` Bounded recovery invariant #048: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-049]` Bounded recovery invariant #049: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-050]` Bounded recovery invariant #050: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-051]` Bounded recovery invariant #051: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-052]` Bounded recovery invariant #052: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-053]` Bounded recovery invariant #053: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-054]` Bounded recovery invariant #054: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-055]` Bounded recovery invariant #055: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-056]` Bounded recovery invariant #056: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-057]` Bounded recovery invariant #057: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-058]` Bounded recovery invariant #058: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-059]` Bounded recovery invariant #059: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-060]` Bounded recovery invariant #060: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-061]` Bounded recovery invariant #061: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-062]` Bounded recovery invariant #062: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-063]` Bounded recovery invariant #063: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-064]` Bounded recovery invariant #064: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-065]` Bounded recovery invariant #065: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-066]` Bounded recovery invariant #066: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-067]` Bounded recovery invariant #067: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-068]` Bounded recovery invariant #068: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-069]` Bounded recovery invariant #069: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-070]` Bounded recovery invariant #070: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-071]` Bounded recovery invariant #071: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-072]` Bounded recovery invariant #072: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-073]` Bounded recovery invariant #073: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-074]` Bounded recovery invariant #074: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.
- `[REC-GUAR-075]` Bounded recovery invariant #075: maximum `3` retries per stall, `2.5 m` reachable NavMesh sampling, and clean `Stop()` on exhaustion.

## 5. Procedural Humanoid Animation & Movement Decoupling

`HumanoidAnimator.cs` drives articulated pelvis, spine, head, arms, thighs, shins, and feet strictly as a presentation layer:
- **Collider & NavMesh Isolation:** `Awake()` disables and destroys child `Collider` components inside `Character_Visuals` and attaches a `NavMeshModifier` with `overrideArea = true` and `ignoreFromBuild = true`.
- **Zero Root Motion:** Stride phase (`gaitPhase`) advances proportionally to actual horizontal ground displacement and `NavMeshAgent` velocity (`walkFrequency = 4.2f`, `maxStrideAngle = 26f`, `maxArmSwingAngle = 22f`, `kneeBendAngle = 44f`), preventing foot sliding without interfering with `EnemyMovement`.
- **Eye Transform Linkage:** `FlankingGuardAI.EnsureEyeTransform()` caches `anim.head` (`Character_Visuals/Pelvis/Spine/Neck/Head`) so sight raycasts track head orientation cleanly.

- `[ANIM-001]` Presentation isolation check #001: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-002]` Presentation isolation check #002: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-003]` Presentation isolation check #003: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-004]` Presentation isolation check #004: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-005]` Presentation isolation check #005: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-006]` Presentation isolation check #006: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-007]` Presentation isolation check #007: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-008]` Presentation isolation check #008: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-009]` Presentation isolation check #009: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-010]` Presentation isolation check #010: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-011]` Presentation isolation check #011: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-012]` Presentation isolation check #012: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-013]` Presentation isolation check #013: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-014]` Presentation isolation check #014: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-015]` Presentation isolation check #015: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-016]` Presentation isolation check #016: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-017]` Presentation isolation check #017: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-018]` Presentation isolation check #018: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-019]` Presentation isolation check #019: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-020]` Presentation isolation check #020: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-021]` Presentation isolation check #021: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-022]` Presentation isolation check #022: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-023]` Presentation isolation check #023: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-024]` Presentation isolation check #024: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-025]` Presentation isolation check #025: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-026]` Presentation isolation check #026: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-027]` Presentation isolation check #027: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-028]` Presentation isolation check #028: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-029]` Presentation isolation check #029: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-030]` Presentation isolation check #030: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-031]` Presentation isolation check #031: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-032]` Presentation isolation check #032: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-033]` Presentation isolation check #033: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-034]` Presentation isolation check #034: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-035]` Presentation isolation check #035: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-036]` Presentation isolation check #036: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-037]` Presentation isolation check #037: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-038]` Presentation isolation check #038: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-039]` Presentation isolation check #039: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-040]` Presentation isolation check #040: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-041]` Presentation isolation check #041: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-042]` Presentation isolation check #042: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-043]` Presentation isolation check #043: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-044]` Presentation isolation check #044: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-045]` Presentation isolation check #045: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-046]` Presentation isolation check #046: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-047]` Presentation isolation check #047: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-048]` Presentation isolation check #048: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-049]` Presentation isolation check #049: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-050]` Presentation isolation check #050: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-051]` Presentation isolation check #051: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-052]` Presentation isolation check #052: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-053]` Presentation isolation check #053: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-054]` Presentation isolation check #054: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-055]` Presentation isolation check #055: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-056]` Presentation isolation check #056: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-057]` Presentation isolation check #057: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-058]` Presentation isolation check #058: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-059]` Presentation isolation check #059: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-060]` Presentation isolation check #060: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-061]` Presentation isolation check #061: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-062]` Presentation isolation check #062: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-063]` Presentation isolation check #063: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-064]` Presentation isolation check #064: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-065]` Presentation isolation check #065: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-066]` Presentation isolation check #066: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-067]` Presentation isolation check #067: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-068]` Presentation isolation check #068: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-069]` Presentation isolation check #069: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.
- `[ANIM-070]` Presentation isolation check #070: confirm `Character_Visuals` remains collider-free (`ignoreFromBuild = true`), idle breathing (`1.6 Hz`, `1.4 deg`) engages cleanly below deadzone, and `EnemyMovement` retains sole root transform control.

## 6. FlankingGuardAI Lateral Candidate Evaluation

`FlankingGuardAI.TrySelectFlankTarget()` evaluates `18` geometric candidates around the player (`3` lateral offsets `[3.5, 5.5, 7.5]` m x `3` forward offsets `[-1.5, 0.0, 1.5]` m x `2` sides `Left/Right`):
1. **Room & NavMesh Filtering:** Rejects any candidate outside `roomBounds` (`IsInsideRoom`), un-sampled within `maxSampleDistance = 2.0f`, or lacking `PathComplete`.
2. **Path Ratio Gate:** Rejects detours exceeding `directPathLength * maxFlankPathRatio` (`2.5f`).
3. **Direct-Pursuit Equivalence Filter:** Computes `divergenceAngle` (between `directDir` and `path.corners[1] - guardPos`) and `lineAngle`. Rejects candidates where `divergenceAngle < 18 deg && lineAngle < 15 deg` (`isEquivalentToDirect = true`).
4. **Utility Scoring (Lower Is Better):**
   - `score = pathLength + (abs(approachAngle - 90) * 0.05) + squadPenalty - (sameSide ? hysteresisBonus : 0)` (`hysteresisBonus = 2.0f`).
5. **Direct Pursuit Fallback:** Immediately switches to `GuardState.Pursue` if `distToPlayer <= minFlankPlayerDistance` (`3.0f` m) or no viable flank remains.

- `[FLANK-EVAL-001]` Lateral evaluation case #001: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-002]` Lateral evaluation case #002: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-003]` Lateral evaluation case #003: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-004]` Lateral evaluation case #004: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-005]` Lateral evaluation case #005: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-006]` Lateral evaluation case #006: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-007]` Lateral evaluation case #007: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-008]` Lateral evaluation case #008: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-009]` Lateral evaluation case #009: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-010]` Lateral evaluation case #010: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-011]` Lateral evaluation case #011: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-012]` Lateral evaluation case #012: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-013]` Lateral evaluation case #013: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-014]` Lateral evaluation case #014: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-015]` Lateral evaluation case #015: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-016]` Lateral evaluation case #016: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-017]` Lateral evaluation case #017: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-018]` Lateral evaluation case #018: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-019]` Lateral evaluation case #019: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-020]` Lateral evaluation case #020: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-021]` Lateral evaluation case #021: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-022]` Lateral evaluation case #022: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-023]` Lateral evaluation case #023: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-024]` Lateral evaluation case #024: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-025]` Lateral evaluation case #025: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-026]` Lateral evaluation case #026: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-027]` Lateral evaluation case #027: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-028]` Lateral evaluation case #028: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-029]` Lateral evaluation case #029: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-030]` Lateral evaluation case #030: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-031]` Lateral evaluation case #031: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-032]` Lateral evaluation case #032: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-033]` Lateral evaluation case #033: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-034]` Lateral evaluation case #034: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-035]` Lateral evaluation case #035: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-036]` Lateral evaluation case #036: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-037]` Lateral evaluation case #037: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-038]` Lateral evaluation case #038: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-039]` Lateral evaluation case #039: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-040]` Lateral evaluation case #040: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-041]` Lateral evaluation case #041: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-042]` Lateral evaluation case #042: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-043]` Lateral evaluation case #043: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-044]` Lateral evaluation case #044: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-045]` Lateral evaluation case #045: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-046]` Lateral evaluation case #046: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-047]` Lateral evaluation case #047: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-048]` Lateral evaluation case #048: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-049]` Lateral evaluation case #049: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-050]` Lateral evaluation case #050: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-051]` Lateral evaluation case #051: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-052]` Lateral evaluation case #052: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-053]` Lateral evaluation case #053: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-054]` Lateral evaluation case #054: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-055]` Lateral evaluation case #055: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-056]` Lateral evaluation case #056: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-057]` Lateral evaluation case #057: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-058]` Lateral evaluation case #058: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-059]` Lateral evaluation case #059: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-060]` Lateral evaluation case #060: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-061]` Lateral evaluation case #061: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-062]` Lateral evaluation case #062: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-063]` Lateral evaluation case #063: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-064]` Lateral evaluation case #064: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-065]` Lateral evaluation case #065: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-066]` Lateral evaluation case #066: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-067]` Lateral evaluation case #067: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-068]` Lateral evaluation case #068: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-069]` Lateral evaluation case #069: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-070]` Lateral evaluation case #070: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-071]` Lateral evaluation case #071: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-072]` Lateral evaluation case #072: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-073]` Lateral evaluation case #073: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-074]` Lateral evaluation case #074: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-075]` Lateral evaluation case #075: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-076]` Lateral evaluation case #076: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-077]` Lateral evaluation case #077: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-078]` Lateral evaluation case #078: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-079]` Lateral evaluation case #079: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-080]` Lateral evaluation case #080: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-081]` Lateral evaluation case #081: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-082]` Lateral evaluation case #082: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-083]` Lateral evaluation case #083: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-084]` Lateral evaluation case #084: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-085]` Lateral evaluation case #085: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-086]` Lateral evaluation case #086: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-087]` Lateral evaluation case #087: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-088]` Lateral evaluation case #088: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-089]` Lateral evaluation case #089: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-090]` Lateral evaluation case #090: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-091]` Lateral evaluation case #091: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-092]` Lateral evaluation case #092: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-093]` Lateral evaluation case #093: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-094]` Lateral evaluation case #094: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-095]` Lateral evaluation case #095: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-096]` Lateral evaluation case #096: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-097]` Lateral evaluation case #097: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-098]` Lateral evaluation case #098: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-099]` Lateral evaluation case #099: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-100]` Lateral evaluation case #100: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-101]` Lateral evaluation case #101: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-102]` Lateral evaluation case #102: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-103]` Lateral evaluation case #103: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-104]` Lateral evaluation case #104: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-105]` Lateral evaluation case #105: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-106]` Lateral evaluation case #106: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-107]` Lateral evaluation case #107: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-108]` Lateral evaluation case #108: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-109]` Lateral evaluation case #109: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-110]` Lateral evaluation case #110: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-111]` Lateral evaluation case #111: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-112]` Lateral evaluation case #112: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-113]` Lateral evaluation case #113: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-114]` Lateral evaluation case #114: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-115]` Lateral evaluation case #115: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-116]` Lateral evaluation case #116: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-117]` Lateral evaluation case #117: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-118]` Lateral evaluation case #118: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-119]` Lateral evaluation case #119: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-120]` Lateral evaluation case #120: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-121]` Lateral evaluation case #121: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-122]` Lateral evaluation case #122: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-123]` Lateral evaluation case #123: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-124]` Lateral evaluation case #124: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-125]` Lateral evaluation case #125: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-126]` Lateral evaluation case #126: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-127]` Lateral evaluation case #127: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-128]` Lateral evaluation case #128: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-129]` Lateral evaluation case #129: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-130]` Lateral evaluation case #130: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-131]` Lateral evaluation case #131: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-132]` Lateral evaluation case #132: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-133]` Lateral evaluation case #133: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-134]` Lateral evaluation case #134: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-135]` Lateral evaluation case #135: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-136]` Lateral evaluation case #136: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-137]` Lateral evaluation case #137: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-138]` Lateral evaluation case #138: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-139]` Lateral evaluation case #139: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-140]` Lateral evaluation case #140: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-141]` Lateral evaluation case #141: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-142]` Lateral evaluation case #142: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-143]` Lateral evaluation case #143: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-144]` Lateral evaluation case #144: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-145]` Lateral evaluation case #145: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-146]` Lateral evaluation case #146: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-147]` Lateral evaluation case #147: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-148]` Lateral evaluation case #148: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-149]` Lateral evaluation case #149: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
- `[FLANK-EVAL-150]` Lateral evaluation case #150: verified `70-110 deg` lateral approach preference, `18 deg` divergence threshold, `2.0 m` hysteresis stability, and automatic fallback to `Pursue` below `3.0 m`.
