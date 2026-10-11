# Team Foundations & Ownership Setup

This document outlines the shared starter foundations established across the **Warehouse-Escape** project for all four team members. These implementations represent modular, integrated baselines designed to support parallel development without merge conflicts or regressions.

---

## 1. Jadavan — Agent Controller & Flanking Guard

### Assigned Role & Scope
- **Role:** Agent Controller + Flanking Guard Specialist
- **Responsibility:** Core enemy movement actuator, rotation smoothing, and multi-state flanking tactical behavior.

### Associated Files
- [`Assets/Scripts/EnemyMovement.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/EnemyMovement.cs): Shared movement controller used by all four guards. Manages NavMesh path validation, smooth slerp rotation towards travel direction, and reliable arrival threshold detection.
- [`Assets/Scripts/GuardPatrol.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/GuardPatrol.cs): Waypoint patrol logic with robust recovery from invalid or unreachable paths.
- [`Assets/Scripts/FlankingGuardAI.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/FlankingGuardAI.cs): Stage 1 perception and state-transition foundation.

### Scene Objects
- **`Guard_Jadavan_Flanking`**: Capsule enemy styled with Amber/Orange material (`Mat_Guard_Jadavan`). Placed along the central warehouse thoroughfare.
- **`[Waypoints]/Waypoints_Jadavan`**: 4 central loop patrol waypoints orbiting warehouse shelving blocks (`WP_Jadavan_1` through `WP_Jadavan_4`).

### Current Behavior (Starter Foundation)
- Continuously executes waypoint patrol along the central warehouse aisle.
- Senses the player using sight distance (12m), horizontal FOV (90°), and raycast occlusion blocking by warehouse walls and shelving.
- Engages in active pursuit upon detection, updating destination at configurable intervals (`repathInterval = 0.25s`).
- When line of sight is broken, investigates the intruder's last known coordinates before returning to patrol after a search timeout.
- *Note:* Flanking maneuvers (coordinating with other guards to take alternative routes or cut off escapes) are pending future algorithmic implementation.

### Remaining Work
- Implement true multi-agent flanking logic: calculating flanking vectors, surrounding routes, and pincer maneuvers relative to other guards.
- Integrate attack state, combat range detection, and damage triggers.
- Integrate humanoid animations (walking, running, searching, attack swings) and Mecanim blend trees with `EnemyMovement`.

---

## 2. Sanjeevsaran — Systems Engineer & Search Guard

### Assigned Role & Scope
- **Role:** Systems Engineer + Search Guard Specialist
- **Responsibility:** Core game loop objectives, security access-card pickup, locked exit doors, and persistent search AI behavior.

### Associated Files
- [`Assets/Scripts/AccessCard.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/AccessCard.cs): Security access-card pickup trigger with idle hovering/rotation animations, audio-visual feedback, and player inventory state.
- [`Assets/Scripts/DoorController.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/DoorController.cs): Exit door controller managing smooth opening/closing, ground-level `[E]` interaction distance, keycard security requirement, on-screen feedback HUD, and stationary `NavMeshObstacle` synchronization.
- [`Assets/Scripts/SearchGuardAI.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/SearchGuardAI.cs): Persistent search guard state machine (`Patrol` -> `Pursue` -> `Search` -> `Patrol`).

### Scene Objects
- **`AccessCard_Pickup`**: Neon-green keycard with interaction volume, staged on the Security Console terminal at `(-7.5, 1.0, 7.5)`.
- **`Exit_Door` & `Exit_Door_NavBlocker`**: Heavy industrial security door at the South wall and its stationary carving obstacle blocking passage until unlocked and fully raised.
- **`Guard_Sanjeevsaran_Search`**: Capsule enemy styled with Gold/Yellow material (`Mat_Guard_Sanjeevsaran`). Stationed along the North perimeter corridor.
- **`[Waypoints]/Waypoints_Sanjeevsaran`**: 4 perimeter waypoints (`WP_Sanjeev_1` through `WP_Sanjeev_4`).

### Current Behavior (Starter Foundation)
- Exit door remains locked by default; interacting displays an on-screen "Access Denied: Security Keycard Required!" prompt.
- Picking up the access card grants clearance and updates the on-screen HUD badge.
- With the keycard collected, pressing `[E]` within 3m of the exit door smoothly raises it, releasing the stationary NavMesh blocker once fully clear.
- The Search Guard systematically patrols the North perimeter. Upon detecting an intruder, it pursues actively. When line of sight is broken, it travels directly to the last seen position and conducts an extended 3-second investigation before resuming patrol.

### Remaining Work
- Multi-tier keycard security levels (e.g., Level 1, Level 2) or color-coded terminal puzzles.
- Audio integration for keycard pickup chimes, door motorized servos, alarm sirens, and guard voice lines.
- Advanced search behavior: looking around local search radiuses, inspecting nearby hiding crates, and radioing alerts to teammates.

---

## 3. Aqqil — Core Developer / 3D Assets & Defensive Guard

### Assigned Role & Scope
- **Role:** Core Developer / Custom 3D Assets + Defensive Guard Specialist
- **Responsibility:** Primitive placeholders for future custom 3D models (Blender/Maya pipeline) and dedicated area defense AI protecting mission objectives.

### Associated Files
- [`Assets/Scripts/DefensiveGuardAI.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/DefensiveGuardAI.cs): Defensive sentinel state machine (`GuardingPost` -> `PursueIntruder` -> `ReturnToPost`).
- Material assets: `Mat_Guard_Aqqil.mat`, `Mat_Placeholder_Asset.mat`, `Mat_Placeholder_Accent.mat`.

### Scene Objects
- **`[Aqqil_Placeholders]` Root**: Clearly named primitive assemblies staged as geometric placeholders:
  - `[Placeholder]_SecurityConsole_Terminal`: Desk pedestal, angled display monitor, and keyboard terminal hosting the Access Card at `(-7.5, 0.0, 7.5)`.
  - `[Placeholder]_Forklift_Vehicle`: Industrial forklift chassis, cab cage, upright mast, and forks staged in the East staging bay at `(7.5, 0.0, 7.0)`.
  - `[Placeholder]_Guard_CharacterMesh`: Modular humanoid mannequin (torso, head, stand) staged in the West security alcove at `(-8.0, 0.0, -2.0)`.
- **`Guard_Aqqil_Defensive`**: Capsule enemy styled with Cyan/Blue material (`Mat_Guard_Aqqil`). Stationed at post `(-5.5, 1.08, 6.5)` guarding the console entrance.

### Current Behavior (Starter Foundation)
- Primitive placeholders provide accurate physical scale, obstacle colliders, and visual framing without claiming to be final 3D meshes.
- Defensive Guard maintains a stationary sentinel post guarding the Access Card terminal.
- Engages intruders in active pursuit only when they breach the configurable defense radius (6.0m) with clear line of sight.
- If the player retreats beyond the leash radius or breaks line of sight outside the territory, the guard disengages and returns directly to its guard post.

### Remaining Work
- Replace primitive assemblies with textured 3D models (FBX format exported from Blender/Maya) with proper UV unwrapping and PBR textures.
- Replace capsule guard body with rigged 3D character mesh.
- Defensive alert states: raising shield/weapon, warning shouts, and setting up defensive barricades or locking blast doors.

---

## 4. Varun — World Builder & Patrol Guard

### Assigned Role & Scope
- **Role:** World Builder + Patrol Guard Specialist
- **Responsibility:** Warehouse architectural layout, alternative navigation routes, obstacle arrangement, and baseline looping patrol AI.

### Associated Files
- [`Assets/Scripts/PatrolGuardAI.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/PatrolGuardAI.cs): Fast-looping patrol state machine (`Patrol` -> `Pursue`). Immediately resumes waypoint circuit when line of sight is broken (no prolonged search).
- Scene: [`Assets/Scenes/Warehouse.unity`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scenes/Warehouse.unity)

### Scene Objects
- **`Warehouse_Environment` Hierarchy**:
  - Full perimeter warehouse walls, floor, and industrial ceiling.
  - Multi-tier pallet racking units with hazard markings and color-coded uprights.
  - Wooden crates, dark shipping containers, and stacked pallets.
  - Wide passages (width > 2.5m) ensuring smooth NavMesh traversal for multiple concurrent agents.
  - Multiple alternative routes connecting the South spawn, Central staging, East corridor, and North terminal area.
- **`Guard_Varun_Patrol`**: Capsule enemy styled with Violet/Purple material (`Mat_Guard_Varun`). Stationed on the West side of the warehouse at `(-6.0, 1.08, -1.0)`.
- **`[Waypoints]/Waypoints_Varun`**: 4 assigned patrol waypoints (`WP_Varun_1` through `WP_Varun_4`).

### Current Behavior (Starter Foundation)
- Loops assigned waypoints continuously across the West warehouse sector.
- When an intruder enters line of sight, immediately breaks off patrol and gives chase.
- The moment line of sight is lost, it immediately disengages pursuit and resumes its waypoint patrol loop without performing extended search investigations.

### Remaining Work
- Environmental props: ventilation ducts, overhead cranes, hanging industrial cables, safety signage, and security cameras.
- Lighting polish: baking lightmaps, ambient occlusion, volumetric fog, and emergency flashing beacons.
- Expanding waypoint routes into dynamic patrol paths with randomized patrol patterns.

---

## 5. Shared Architecture & Integration Summary

| System | Shared Component / Script | Implementation Details |
| :--- | :--- | :--- |
| **Movement Controller** | [`EnemyMovement.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/EnemyMovement.cs) | Single movement authority per guard. Smooth slerp rotation, NavMesh validation, arrival detection. |
| **Perception Utility** | [`GuardPerception.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/GuardPerception.cs) | Shared helper evaluating sight distance, horizontal FOV, eye heights, and obstacle raycast occlusion. |
| **Exit Security** | [`DoorController.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/DoorController.cs) | Stationary doorway `NavMeshObstacle` carving blocker synchronized with moving visual door and keycard requirement. |
| **Objective Item** | [`AccessCard.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/AccessCard.cs) | Pickable security item with floating animation, on-screen UI indicator, and global clearance status. |

### Guard Differentiation Matrix

| Guard Name | Owner | Color | Default State | Detection Action | LoS Lost Behavior |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `Guard_Jadavan_Flanking` | Jadavan | Amber / Orange | Central Patrol | Direct Pursuit | Investigates last seen coordinates, then resumes patrol |
| `Guard_Sanjeevsaran_Search` | Sanjeevsaran | Gold / Yellow | North Patrol | Direct Pursuit | Extended search (3s timer) at last seen coordinates, then resumes patrol |
| `Guard_Aqqil_Defensive` | Aqqil | Cyan / Blue | Stationed at Post | Pursues within 6m defense zone | Disengages and returns directly to guard post |
| `Guard_Varun_Patrol` | Varun | Violet / Purple | West Patrol | Direct Pursuit | Immediately resumes waypoint circuit (no extended search) |
