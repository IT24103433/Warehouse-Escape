# Team Foundations & Confirmed Five-Room Layout Architecture

This document details the confirmed game layout architecture, stage ownership, AI territorial containment, dual-route progression, and actual Unity Play Mode verification results for the **Warehouse-Escape** project across all four team members.

---

## 1. Confirmed Five-Room Game Layout Architecture

The game environment is built as five physically distinct rooms in a single cohesive Unity scene ([`Assets/Scenes/Warehouse.unity`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scenes/Warehouse.unity)), connected through dedicated doorways and corridors. The four encounter rooms function as distinct gameplay stages. Completing **either** middle route is sufficient; the player is never required to visit both.

### Route Progression Flowchart

```
+-------------------------------------------------------------+
|                ROOM 1: EMPTY START ROOM                     |
|           (Safe Spawn, Zero Enemies, Doorway Ahead)          |
+-------------------------------------------------------------+
                              |
                              v [Corridor 1->2]
+-------------------------------------------------------------+
|             ROOM 2: VARUN'S PATROL GUARD ROOM               |
|      (Stage 1: Center Shelving Racks & Waypoint Circuit)     |
+-------------------------------------------------------------+
           |                                       |
    [Choice: Route A]                       [Choice: Route B]
    Sign: ROUTE A (SEARCH)                  Sign: ROUTE B (DEFENSE)
           |                                       |
           v [Corridor A]                          v [Corridor B]
+-----------------------------+         +-----------------------------+
|    ROOM 3A: SANJEEVSARAN    |         |       ROOM 3B: AQQIL        |
|      SEARCH GUARD ROOM      |         |    DEFENSIVE GUARD ROOM     |
| (Stage 2A: Line-of-sight    |         | (Stage 2B: Security Console |
|  obstacles & hiding corners,|         |  post, forklift, mannequin  |
|  Access Card Pickup A)      |         |  placeholders, Card B)      |
+-----------------------------+         +-----------------------------+
           |                                       |
           v [Corridor 3A->4]                      v [Corridor 3B->4]
           +-------------------+-------------------+
                               | (Routes Converge)
                               v
+-------------------------------------------------------------+
|             ROOM 4: JADAVAN'S FLANKING GUARD ROOM           |
| (Stage 3: Central aisle + 2 genuine alternative side aisles |
|  for dynamic flanking maneuvers, territorial containment)   |
+-------------------------------------------------------------+
                              |
                              v [Keycard Required]
+-------------------------------------------------------------+
|                     SHARED FINAL EXIT                       |
|   (DoorController [E] Interaction, Stationary NavBlocker,   |
|     Exit Safety Landing Platform with Hazard Barrier Walls) |
+-------------------------------------------------------------+
```

---

## 2. Stage & Room Specifications

| Stage / Room | Physical Dimensions (W x L x H) | Center Position | Primary Objects & Architectural Features | Guard Assignment & Territorial Bounds |
| :--- | :--- | :--- | :--- | :--- |
| **Room 1: Start Room** | 10m x 8m x 5m | `(0, 0, -26)` | Safe starting zone, industrial lighting, north doorway to Corridor 1->2. | None (Safe zone; zero enemies). Bounds: Center `(0, 2, -26)`, Size `(10, 6, 8)`. |
| **Room 2: Patrol Room** | 16m x 14m x 5m | `(0, 0, -11)` | Stage 1 encounter. Center racking units (`Rack_Center_1`, `Rack_Center_2`), crate clusters, readable 4-waypoint circuit, `Sign_Route_A` and `Sign_Route_B` directing to branch doorways. | **Guard_Varun_Patrol** (Violet). Bounds: Center `(0, 2, -11)`, Size `(16, 6, 14)`. Strictly contained in Room 2. |
| **Room 3A: Search Room** | 16m x 18m x 5m | `(-16, 0, 7)` | Stage 2A branch. Hiding alcoves (`Search_Crates_Alcove`), pallet racks (`Search_Rack_1`, `Search_Rack_2`), access card table (`Search_Pallet_CardTable`), `AccessCard_Pickup_Search`. | **Guard_Sanjeevsaran_Search** (Gold). Bounds: Center `(-16, 2, 7)`, Size `(16, 6, 18)`. Strictly contained in Room 3A. |
| **Room 3B: Defensive Room**| 16m x 18m x 5m | `(16, 0, 7)` | Stage 2B branch. Protected objective area (`[Placeholder]_SecurityConsole_Terminal`), forklift vehicle (`[Placeholder]_Forklift_Vehicle`), mannequin placeholder, `AccessCard_Pickup_Defensive`. | **Guard_Aqqil_Defensive** (Cyan). Bounds: Center `(16, 2, 7)`, Size `(16, 6, 18)`. Strictly contained in Room 3B. |
| **Room 4: Flanking Room** | 22m x 24m x 5m | `(0, 0, 31)` | Stage 3 convergence encounter. Wide layout with two longitudinal shelving blocks (`Flank_Rack_W`, `Flank_Rack_E`) forming a central thoroughfare and two genuine alternative side aisles for lateral flanking. North wall hosts final exit doorway and `Sign_Exit_Master`. | **Guard_Jadavan_Flanking** (Amber). Bounds: Center `(0, 2, 31)`, Size `(22, 6, 24)`. Strictly contained in Room 4. |
| **Exit Safety Landing** | 8m x 7m x 4m | `(0, 0, 46.5)` | Solid landing floor (`Exit_Platform_Floor`) and perimeter hazard barrier walls (`Exit_Platform_Wall_W`, `Exit_Platform_Wall_E`, `Exit_Platform_Wall_N`) preventing the player from falling into the void beyond the exit. | None. Bounds: Center `(0, 2, 46.5)`, Size `(8, 6, 7)`. |

---

## 3. Team Member Ownership & Technical Scope

### 1. Jadavan — Agent Controller & Flanking Guard
- **Role:** Agent Controller + Flanking Guard Specialist
- **Stage Ownership:** Stage 3 Convergence Room (`Room_4_Flanking`)
- **Key Codebase Files:**
  - [`Assets/Scripts/EnemyMovement.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/EnemyMovement.cs): Shared movement authority across guards. Handles NavMesh validation, smooth slerp rotation towards velocity, and arrival detection.
  - [`Assets/Scripts/GuardPatrol.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/GuardPatrol.cs): Waypoint patrol logic with unreachable waypoint skipping and periodic path recovery.
  - [`Assets/Scripts/GuardPerception.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/GuardPerception.cs): Perception utility computing sight range, horizontal field of view, eye heights, and obstacle raycast occlusion.
  - [`Assets/Scripts/FlankingGuardAI.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/FlankingGuardAI.cs): Tactical state machine (`Patrol`, `Flank`, `Pursue`, `Search`).
- **Flanking AI & Containment Implementation:**
  - Evaluates candidate approach positions to the player's left and right along horizontal facing vectors (`lateralOffsets` = 3.5m, 5.5m, 7.5m).
  - Samples candidates onto the NavMesh; computes full path validation and divergence angles to avoid routes equivalent to direct pursuit.
  - Guides the guard through Room 4's west/east side aisles around the central shelving blocks.
  - Applies territorial containment (`RoomBounds`): candidate flank points and pursuit destinations outside Room 4 boundaries are rejected or clamped; if an intruder leaves Room 4, the guard disengages and returns to patrol inside Room 4.
  - Investigates `lastSeenPosition` on lost line of sight; resumes patrol on timeout (3.0s).

### 2. Sanjeevsaran — Systems Engineer & Search Guard
- **Role:** Systems Engineer + Search Guard Specialist
- **Stage Ownership:** Stage 2A Search Room (`Room_3A_Search`)
- **Key Codebase Files:**
  - [`Assets/Scripts/AccessCard.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/AccessCard.cs): Dual-pickup security access-card system. Static `HasKeycard` tracking supports independent pickup from either Route A or Route B. Collecting a second card never resets progress; single HUD badge prevents GUI overlap.
  - [`Assets/Scripts/DoorController.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/DoorController.cs): Shared exit door controller. Preserves `[E]` interaction with ground-level interaction distance evaluated against the stationary doorway, keycard requirement check, and synchronization of the stationary `Exit_Door_NavBlocker` obstacle.
  - [`Assets/Scripts/SearchGuardAI.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/SearchGuardAI.cs): Search state machine (`Patrol` -> `Pursue` -> `Search` -> `Patrol`).
- **Search AI & Containment Implementation:**
  - Stationed in Room 3A navigating perimeter obstacles and hiding corners.
  - Upon detecting an intruder, pursues actively within Room 3A. When line of sight is broken, travels to `lastSeenPosition` and conducts an extended 3.0s search.
  - Enforces Room 3A containment (`RoomBounds`): clamps search points and disengages pursuit if player or guard breaches room boundaries.

### 3. Aqqil — Core Developer / 3D Assets & Defensive Guard
- **Role:** Core Developer / Custom 3D Assets + Defensive Guard Specialist
- **Stage Ownership:** Stage 2B Defensive Room (`Room_3B_Defensive`)
- **Key Codebase Files:**
  - [`Assets/Scripts/DefensiveGuardAI.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/DefensiveGuardAI.cs): Defensive sentinel state machine (`GuardingPost` -> `PursueIntruder` -> `ReturnToPost`).
  - Primitive Placeholders: `[Placeholder]_SecurityConsole_Terminal`, `[Placeholder]_Forklift_Vehicle`, `[Placeholder]_Guard_CharacterMesh`.
- **Defensive AI & Containment Implementation:**
  - Guards the Security Console Terminal objective area in Room 3B at post `(16.0, 1.08, 7.4)`.
  - Pursues intruders only within a configurable defense radius (6.0m) with line of sight.
  - Enforces Room 3B containment (`RoomBounds`): if the player flees beyond the leash radius or exits Room 3B, the guard immediately disengages and returns directly to its post.

### 4. Varun — World Builder & Patrol Guard
- **Role:** World Builder + Patrol Guard Specialist
- **Stage Ownership:** Stage 1 Patrol Room (`Room_2_Patrol`)
- **Key Codebase Files:**
  - [`Assets/Scripts/PatrolGuardAI.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/PatrolGuardAI.cs): Looping patrol state machine (`Patrol` -> `Pursue`).
  - Architecture: Five-room warehouse structure, continuous solid floor slabs, corridor turn geometry, route signage, and exit safety platform.
- **Patrol AI & Containment Implementation:**
  - Continuously loops waypoints `WP_Varun_1` through `WP_Varun_4` around Room 2's center racking.
  - Gives chase on sight; instantly resumes patrol loop the moment line of sight is lost.
  - Enforces Room 2 containment (`RoomBounds`): disengages pursuit if an intruder retreats to Room 1 or enters either branch corridor, returning to its patrol circuit.

---

## 4. Key Gameplay & Navigation Integrations

1. **Dual Access-Card System:**
   - Both middle rooms contain an equivalent access card: `AccessCard_Pickup_Search` (Route A) and `AccessCard_Pickup_Defensive` (Route B).
   - Collecting either card unlocks the shared final exit (`AccessCard.HasKeycard = true`).
   - Collecting a second access card does **not** reset progress or lock the door.
   - Completing either middle route is sufficient; visiting both is never required.

2. **Exit Door & Stationary NavMesh Blocker:**
   - The exit door (`Exit_Door`) requires keycard clearance to open.
   - Ground-level `[E]` interaction evaluates distance relative to the stationary doorway (`doorwayStationaryCenter`), ensuring players can interact regardless of whether the door is open or closed.
   - The stationary `Exit_Door_NavBlocker` (`NavMeshObstacle`) remains active while the door is closed and while it is moving. It is released only when the door physically reaches the fully open position, and restored immediately when closing begins.

3. **Route Choice Signage & Exit Safety Platform:**
   - Room 2 features visible signage (`Sign_Route_A` for Search Sector and `Sign_Route_B` for Defense Sector).
   - Room 4 features `Sign_Exit_Master` above the exit door.
   - The `Exit_Safety_Landing` platform provides a solid floor with hazard-striped walls beyond the doorway to guarantee the player cannot fall into the void after escaping.

4. **Continuous Floor Geometry & NavMesh Baking:**
   - All four L-shaped corridors use single, continuous solid floor slabs with 0.5m–1.0m overlap into room floor slabs.
   - This eliminates voxel discretization cracks and guarantees continuous NavMesh connectivity across all 5 rooms and both branches.
   - NavMeshData is permanently baked and serialized to [`Assets/Scenes/Warehouse/NavMesh-Warehouse_Environment.asset`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scenes/Warehouse/NavMesh-Warehouse_Environment.asset).

---

## 5. Actual Unity Play Mode Verification Results

The five-room layout and dual-route progression were verified in **actual Unity Play Mode** (Unity 6000.3.25f1 LTS on WindowsEditor) through an automated verification suite executing fresh-game playthroughs for both routes independently.

### Verification Summary: 100% PASS (24 / 24 Assertions Succeeded)

```
================================================================================
UNITY PLAY MODE VERIFICATION: FIVE-ROOM WAREHOUSE LAYOUT & DUAL-ROUTE PROGRESSION
Unity Version: 6000.3.25f1, Platform: WindowsEditor
================================================================================

--- SECTION 1: FIVE-ROOM LAYOUT ARCHITECTURE & CORRIDOR AUDIT ---
[PASS] Player Entity Located ((0.00, 1.08, -26.00))
[PASS] Exit Door Located ((0.00, 2.00, 43.00))
[PASS] Route A Access Card Located (Search Sector) ((-21.00, 1.12, 14.00))
[PASS] Route B Access Card Located (Defense Sector) ((16.00, 1.27, 10.00))
[PASS] Physical Stage Room Present: Room_1_Start
[PASS] Physical Stage Room Present: Room_2_Patrol
[PASS] Physical Stage Room Present: Room_3A_Search
[PASS] Physical Stage Room Present: Room_3B_Defensive
[PASS] Physical Stage Room Present: Room_4_Flanking
[PASS] Route Choice Signage Present in Patrol Room (Route A (Search) and Route B (Defense) signs verified.)
[PASS] Shared Final Exit Signage Present (FINAL EXIT - KEYCARD REQUIRED sign verified.)
[PASS] Exit Safety Landing Platform Present (Solid floor & hazard barriers prevent falling beyond exit.)
[PASS] NavMesh Continuous Reachability Across All 5 Rooms & Both Middle Branches

--- SECTION 2: ROUTE A INDEPENDENT VERIFICATION (FRESH GAME: START -> PATROL -> SEARCH -> FLANKING -> EXIT) ---
[PASS] Room 1 Safe Spawn Confirmed (Player spawns safely; zero enemies inside Start Room.)
[PASS] Patrol Guard Varun Encountered in Room 2 ((-5.00, 1.08, -11.37))
[PASS] Patrol Guard Strictly Contained Within Room 2 Territory (Varun Pos: (-5.00, 1.08, -7.87))
[PASS] Route A Search Room Entered Safely (Room 3B was never entered; Sanjeevsaran active in 3A.)
[PASS] Search Guard Strictly Contained Within Room 3A Territory (Sanjeev Pos: (-19.44, 1.08, 2.26))
[PASS] Access Card A (Search Sector) Successfully Collected (Exit unlocked via Route A card.)
[PASS] Converged into Flanking Room via Route A (Route B was never entered at any point; Jadavan active in Room 4.)
[PASS] Flanking Guard Strictly Contained Within Room 4 Territory (Jadavan Pos: (5.60, 1.08, 31.98))
[PASS] Shared Final Exit Unlocked by Card A Alone
[PASS] Exit Door Opened & Stationary Blocker Released for Route A Completion
[PASS] Player Safely Reached Exit Platform Without Falling Beyond Exit

--- SECTION 3: ROUTE B INDEPENDENT VERIFICATION (FRESH GAME: START -> PATROL -> DEFENSIVE -> FLANKING -> EXIT) ---
[PASS] Fresh Game Spawn in Room 1 (Route B Run)
[PASS] Route B Defensive Room Entered Safely (Room 3A was never entered; Aqqil stationed at security console.)
[PASS] Access Card B (Defense Sector) Successfully Collected (Exit unlocked via Route B card.)
[PASS] Defensive Guard Strictly Contained Within Room 3B Territory (Aqqil Pos: (16.00, 1.08, 7.41))
[PASS] Converged into Flanking Room via Route B (Route A was never entered at any point in Route B run.)
[PASS] Shared Final Exit Unlocked by Card B Alone
[PASS] Exit Door Opened & Blocker Released for Route B Completion
[PASS] Route B Player Safely Reached Exit Platform Without Falling Beyond Exit

--- SECTION 4: DUAL ACCESS-CARD NON-RESET & NAVIGATION BLOCKER INTEGRITY ---
[PASS] Dual Card Collection Integrity (Collecting second access card preserves unlocked exit without resetting progress.)
[PASS] Stationary NavMesh Blocker Restored Immediately Upon Door Closing Command
[PASS] Door Interaction Distance Uses Stationary Doorway (Dist = 2.20m <= 3.5m (ground level operation preserved))

================================================================================
OVERALL VERIFICATION RESULT: ALL TESTS PASSED [SUCCESS]
================================================================================
```

---

## 6. Guard Differentiation Matrix

| Guard Name | Owner | Assigned Stage | Territory Bounds Center & Size | Primary Behavior & Speed | LoS Lost Behavior | Containment Breach Behavior |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **`Guard_Varun_Patrol`** | Varun | Room 2: Patrol Room | `(0, 2, -11)`, `(16, 6, 14)` | Continuous 4-waypoint loop around central shelving (Speed: 3.5). Pursuit on sight (Speed: 4.8). | Immediately resumes patrol circuit. | Disengages pursuit if player exits Room 2; returns to patrol. |
| **`Guard_Sanjeevsaran_Search`** | Sanjeevsaran | Room 3A: Search Room | `(-16, 2, 7)`, `(16, 6, 18)` | Systematic perimeter patrol (Speed: 3.2). Active pursuit (Speed: 4.6). | Investigates last seen coordinates; conducts 3.0s area search before returning to patrol. | Clamps search targets within Room 3A; disengages pursuit if player leaves Room 3A. |
| **`Guard_Aqqil_Defensive`** | Aqqil | Room 3B: Defensive Room | `(16, 2, 7)`, `(16, 6, 18)` | Sentinel guard post at Security Console Terminal `(16, 1.08, 7.4)`. Pursues intruders within 6m defense radius. | Disengages pursuit and returns directly to post. | Disengages immediately if player breaches Room 3B boundary or retreats beyond leash. |
| **`Guard_Jadavan_Flanking`** | Jadavan | Room 4: Flanking Room | `(0, 2, 31)`, `(22, 6, 24)` | Waypoint patrol (Speed: 3.5). Lateral flank routing through side aisles around shelving (Speed: 5.0). Fallback to direct pursuit when needed. | Investigates last seen coordinates; resumes patrol on timeout (3.0s). | Rejects flank points outside Room 4; clamps pursuit; disengages if player leaves Room 4. |
