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

<!-- BEGIN_DRAFT_DIRECT_TELEPORT_NOTES -->
### 2.1 Draft Legacy Path & Braking Experiments (Superseded)

- `[DRAFT-NAV-001]` Early NavMesh braking experiment #001: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-002]` Early NavMesh braking experiment #002: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-003]` Early NavMesh braking experiment #003: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-004]` Early NavMesh braking experiment #004: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-005]` Early NavMesh braking experiment #005: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-006]` Early NavMesh braking experiment #006: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-007]` Early NavMesh braking experiment #007: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-008]` Early NavMesh braking experiment #008: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-009]` Early NavMesh braking experiment #009: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-010]` Early NavMesh braking experiment #010: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-011]` Early NavMesh braking experiment #011: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-012]` Early NavMesh braking experiment #012: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-013]` Early NavMesh braking experiment #013: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-014]` Early NavMesh braking experiment #014: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-015]` Early NavMesh braking experiment #015: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-016]` Early NavMesh braking experiment #016: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-017]` Early NavMesh braking experiment #017: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-018]` Early NavMesh braking experiment #018: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-019]` Early NavMesh braking experiment #019: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-020]` Early NavMesh braking experiment #020: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-021]` Early NavMesh braking experiment #021: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-022]` Early NavMesh braking experiment #022: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-023]` Early NavMesh braking experiment #023: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-024]` Early NavMesh braking experiment #024: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-025]` Early NavMesh braking experiment #025: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-026]` Early NavMesh braking experiment #026: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-027]` Early NavMesh braking experiment #027: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-028]` Early NavMesh braking experiment #028: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-029]` Early NavMesh braking experiment #029: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-030]` Early NavMesh braking experiment #030: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-031]` Early NavMesh braking experiment #031: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-032]` Early NavMesh braking experiment #032: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-033]` Early NavMesh braking experiment #033: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-034]` Early NavMesh braking experiment #034: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-035]` Early NavMesh braking experiment #035: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-036]` Early NavMesh braking experiment #036: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-037]` Early NavMesh braking experiment #037: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-038]` Early NavMesh braking experiment #038: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-039]` Early NavMesh braking experiment #039: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-040]` Early NavMesh braking experiment #040: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-041]` Early NavMesh braking experiment #041: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-042]` Early NavMesh braking experiment #042: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-043]` Early NavMesh braking experiment #043: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-044]` Early NavMesh braking experiment #044: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-045]` Early NavMesh braking experiment #045: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-046]` Early NavMesh braking experiment #046: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-047]` Early NavMesh braking experiment #047: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-048]` Early NavMesh braking experiment #048: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-049]` Early NavMesh braking experiment #049: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-050]` Early NavMesh braking experiment #050: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-051]` Early NavMesh braking experiment #051: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-052]` Early NavMesh braking experiment #052: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-053]` Early NavMesh braking experiment #053: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-054]` Early NavMesh braking experiment #054: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-055]` Early NavMesh braking experiment #055: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-056]` Early NavMesh braking experiment #056: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-057]` Early NavMesh braking experiment #057: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-058]` Early NavMesh braking experiment #058: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-059]` Early NavMesh braking experiment #059: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-060]` Early NavMesh braking experiment #060: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-061]` Early NavMesh braking experiment #061: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-062]` Early NavMesh braking experiment #062: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-063]` Early NavMesh braking experiment #063: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-064]` Early NavMesh braking experiment #064: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-065]` Early NavMesh braking experiment #065: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-066]` Early NavMesh braking experiment #066: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-067]` Early NavMesh braking experiment #067: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-068]` Early NavMesh braking experiment #068: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-069]` Early NavMesh braking experiment #069: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-070]` Early NavMesh braking experiment #070: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-071]` Early NavMesh braking experiment #071: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-072]` Early NavMesh braking experiment #072: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-073]` Early NavMesh braking experiment #073: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-074]` Early NavMesh braking experiment #074: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-075]` Early NavMesh braking experiment #075: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-076]` Early NavMesh braking experiment #076: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-077]` Early NavMesh braking experiment #077: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-078]` Early NavMesh braking experiment #078: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-079]` Early NavMesh braking experiment #079: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-080]` Early NavMesh braking experiment #080: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-081]` Early NavMesh braking experiment #081: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-082]` Early NavMesh braking experiment #082: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-083]` Early NavMesh braking experiment #083: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-084]` Early NavMesh braking experiment #084: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-085]` Early NavMesh braking experiment #085: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-086]` Early NavMesh braking experiment #086: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-087]` Early NavMesh braking experiment #087: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-088]` Early NavMesh braking experiment #088: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-089]` Early NavMesh braking experiment #089: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-090]` Early NavMesh braking experiment #090: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-091]` Early NavMesh braking experiment #091: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-092]` Early NavMesh braking experiment #092: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-093]` Early NavMesh braking experiment #093: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-094]` Early NavMesh braking experiment #094: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-095]` Early NavMesh braking experiment #095: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-096]` Early NavMesh braking experiment #096: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-097]` Early NavMesh braking experiment #097: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-098]` Early NavMesh braking experiment #098: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-099]` Early NavMesh braking experiment #099: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-100]` Early NavMesh braking experiment #100: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-101]` Early NavMesh braking experiment #101: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-102]` Early NavMesh braking experiment #102: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-103]` Early NavMesh braking experiment #103: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-104]` Early NavMesh braking experiment #104: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-105]` Early NavMesh braking experiment #105: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-106]` Early NavMesh braking experiment #106: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-107]` Early NavMesh braking experiment #107: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-108]` Early NavMesh braking experiment #108: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-109]` Early NavMesh braking experiment #109: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-110]` Early NavMesh braking experiment #110: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-111]` Early NavMesh braking experiment #111: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-112]` Early NavMesh braking experiment #112: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-113]` Early NavMesh braking experiment #113: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-114]` Early NavMesh braking experiment #114: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-115]` Early NavMesh braking experiment #115: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-116]` Early NavMesh braking experiment #116: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-117]` Early NavMesh braking experiment #117: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-118]` Early NavMesh braking experiment #118: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-119]` Early NavMesh braking experiment #119: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-120]` Early NavMesh braking experiment #120: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-121]` Early NavMesh braking experiment #121: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-122]` Early NavMesh braking experiment #122: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-123]` Early NavMesh braking experiment #123: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-124]` Early NavMesh braking experiment #124: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-125]` Early NavMesh braking experiment #125: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-126]` Early NavMesh braking experiment #126: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-127]` Early NavMesh braking experiment #127: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-128]` Early NavMesh braking experiment #128: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-129]` Early NavMesh braking experiment #129: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-130]` Early NavMesh braking experiment #130: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-131]` Early NavMesh braking experiment #131: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-132]` Early NavMesh braking experiment #132: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-133]` Early NavMesh braking experiment #133: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-134]` Early NavMesh braking experiment #134: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
- `[DRAFT-NAV-135]` Early NavMesh braking experiment #135: raw `agent.SetDestination` without `PathComplete` gate, linear deceleration curve test, and provisional `stoppingDistance` tuning before `EnemyMovement` unified API.
<!-- END_DRAFT_DIRECT_TELEPORT_NOTES -->

<!-- BEGIN_SUPERSEDED_MOVEMENT_NOTES -->
### 2.2 Provisional Speed & Arrival Polling Notes (To Consolidate)

- `[PROV-MOVE-001]` Provisional movement note #001: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-002]` Provisional movement note #002: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-003]` Provisional movement note #003: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-004]` Provisional movement note #004: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-005]` Provisional movement note #005: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-006]` Provisional movement note #006: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-007]` Provisional movement note #007: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-008]` Provisional movement note #008: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-009]` Provisional movement note #009: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-010]` Provisional movement note #010: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-011]` Provisional movement note #011: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-012]` Provisional movement note #012: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-013]` Provisional movement note #013: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-014]` Provisional movement note #014: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-015]` Provisional movement note #015: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-016]` Provisional movement note #016: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-017]` Provisional movement note #017: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-018]` Provisional movement note #018: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-019]` Provisional movement note #019: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-020]` Provisional movement note #020: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-021]` Provisional movement note #021: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-022]` Provisional movement note #022: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-023]` Provisional movement note #023: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-024]` Provisional movement note #024: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-025]` Provisional movement note #025: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-026]` Provisional movement note #026: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-027]` Provisional movement note #027: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-028]` Provisional movement note #028: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-029]` Provisional movement note #029: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-030]` Provisional movement note #030: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-031]` Provisional movement note #031: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-032]` Provisional movement note #032: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-033]` Provisional movement note #033: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-034]` Provisional movement note #034: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-035]` Provisional movement note #035: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-036]` Provisional movement note #036: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-037]` Provisional movement note #037: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-038]` Provisional movement note #038: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-039]` Provisional movement note #039: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-040]` Provisional movement note #040: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-041]` Provisional movement note #041: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-042]` Provisional movement note #042: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-043]` Provisional movement note #043: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-044]` Provisional movement note #044: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-045]` Provisional movement note #045: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-046]` Provisional movement note #046: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-047]` Provisional movement note #047: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-048]` Provisional movement note #048: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-049]` Provisional movement note #049: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-050]` Provisional movement note #050: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-051]` Provisional movement note #051: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-052]` Provisional movement note #052: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-053]` Provisional movement note #053: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-054]` Provisional movement note #054: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-055]` Provisional movement note #055: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-056]` Provisional movement note #056: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-057]` Provisional movement note #057: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-058]` Provisional movement note #058: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-059]` Provisional movement note #059: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-060]` Provisional movement note #060: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-061]` Provisional movement note #061: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-062]` Provisional movement note #062: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-063]` Provisional movement note #063: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-064]` Provisional movement note #064: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-065]` Provisional movement note #065: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-066]` Provisional movement note #066: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-067]` Provisional movement note #067: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-068]` Provisional movement note #068: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-069]` Provisional movement note #069: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
- `[PROV-MOVE-070]` Provisional movement note #070: temporary speed clamping check for patrol (`3.8f`) vs chase (`5.2f`).
<!-- END_SUPERSEDED_MOVEMENT_NOTES -->
