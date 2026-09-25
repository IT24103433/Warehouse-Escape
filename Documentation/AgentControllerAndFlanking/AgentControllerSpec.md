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
