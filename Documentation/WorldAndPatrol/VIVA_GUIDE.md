# Viva Guide — World Builder & Patrol Guard (Varun K.)

**Student:** Kaneswaran Varun (`IT24103696-KV`)  
**Dual Role:**
1. **SE3032 / World Builder:** 5-room warehouse layout, cover & barricade spacing, runtime/baked NavMesh connectivity, and URP industrial lighting.
2. **SE3062 / Intelligent Systems (Room 2):** `PatrolGuardAI.cs`, `PatrolDecisionPolicy.cs` (coverage-aware patrol).

---

## 1. Script & Documentation Map

| Asset / Script | Responsibility | Shared Dependencies |
| --- | --- | --- |
| `Assets/Scripts/PatrolGuardAI.cs` | Room 2 `Patrol` / `Pursue` / `Investigate` state machine (`16 m` sight, `90 deg` FOV, `120 HP`, `UMP_SMG`, visual-only default) | `EnemyMovement`, `GuardPerception`, `GuardPerceptionMemory`, `GuardDecisionQueries`, `GuardShooting`, `Health`, `HumanoidAnimator` |
| `Assets/Scripts/PatrolDecisionPolicy.cs` | Coverage-aware utility scoring favoring neglected, high-visibility inspection points with low travel penalty | `UnityEngine.Mathf` |
| `Assets/Editor/ExpandWarehouseLayout.cs` | 5-room warehouse geometry, Room 2 split exits (`Route A` at `X ~ -5`, `Route B` at `X ~ +5`), shelf/barricade clearances | `NavMeshSurface`, URP materials |
| `Assets/Editor/BakeAndVerifyNavMesh.cs` | NavMesh surface baking and corridor reachability verification | `Unity.AI.Navigation` |
| `Documentation/WorldAndPatrol/WarehouseLayoutSpec.md` | Coordinate references (`Room 2`: `X [-11..11]`, `Z [-18..-2]`), pinch-point clearances, and sightline breaks | Scene `Assets/Scenes/Warehouse.unity` |

---

## 2. World Builder Design Points

1. **5-Room Layout & Branching Flow:**
   - **Room 1 (Safe Start, `Z < -18`):** Player spawn and training pad; Room 2 guard boundary (`PatrolGuardAI.IsInsideRoom`) enforces `Z >= -18.0f` so safe-room health is protected.
   - **Room 2 (Patrol Hall, `X [-11..11]`, `Z [-18..-2]`):** Central shelving, east/west barricades (`+/-7.5, -11`), `DeadEnd_Partition_NE`, and north split doors leading west to **Route A (Room 3)** and east to **Route B (Room 4)**.
   - **Room 3 & Room 4:** Independent branching encounter rooms converging into **Room 5 (Final Loading Hall)** and the `CardReader` exit terminal.
2. **NavMesh & Cover Alignment:**
   - Shelving racks and barricades provide torso/eye occlusion for stealth while preserving turning clearance around shelf endcaps and doorway lintels (`FloorGuide_RouteA` / `FloorGuide_RouteB` colliders disabled).
3. **URP Industrial Lighting & Detail:**
   - Overhead industrial fluorescent/emergency lighting, baked directional lightmaps, and depth-tested room signage (`WorldSignOcclusion.cs`) so signs from adjacent rooms never bleed through walls.

---

## 3. Coverage-Aware Patrol (`PatrolGuardAI` & `PatrolDecisionPolicy`)

- **Inspection Waypoints (`StorageInspectionRoute`):** 8 inspection points across Room 2 (`X [-5.0..4.5]`, `Z [-15.5..-5.5]`). Odd indices pause `2.0 s` with horizontal head/aim scanning (`+/-35 deg`), even indices pause `1.0 s`.
- **Utility Formula (`PatrolDecisionPolicy.Score`):**
  $$U = 0.65 \min\!\left(\frac{\text{age}}{30}, 1\right) + 0.35 \cdot \text{visibility} - 0.25 \min\!\left(\frac{\text{pathLength}}{30}, 1\right)$$
  - `age`: seconds since the waypoint was physically inspected (`lastInspected[i]`), normalized over `30 s`.
  - `visibility`: fraction of 8 horizontal `6 m` rays around the candidate that are unobstructed (`GuardDecisionQueries.Visibility`).
  - `pathLength`: complete NavMesh path length in metres (`GuardDecisionQueries.TryReachable`), normalized over `30 m`.
- **Containment & Safety:** Excludes the current waypoint when alternatives exist, rejects unreachable or out-of-room candidates (`IsInsideRoom`), never falls back to raw unreachable coordinates, and retries after `1 s` if no candidate succeeds.
- **Visual-Only Detection:** `PatrolGuardAI.Awake()` sets `perceptionMemory.VisualOnlyDetection = true`, matching the Room 2 design where Varun relies primarily on sightlines and sound disturbances inside Room 2 without chasing into Room 1.

---

## 4. Live Demo Sequence (2 Minutes)

1. **World Layout & Clearances:** Show Room 1 -> Room 2 split (`FloorGuide_RouteA` / `FloorGuide_RouteB`) and `DeadEnd_Partition_NE` sightline break in Scene view.
2. **Coverage-Aware Patrol (`PatrolGuardAI`):** Select `Guard_Varun_Patrol` in Play Mode and inspect `DecisionCandidates` + magenta `SelectedDecisionTarget` gizmos — demonstrate an older neglected aisle outscoring a recently visited nearby aisle.
3. **Safe-Room Boundary:** Step into Room 2 (`Z > -18`) to trigger `Pursue`, then step south of `Z = -18` into Room 1 to show `PatrolGuardAI` breaking off pursuit to investigate `lastSeenPosition` and returning to patrol.
