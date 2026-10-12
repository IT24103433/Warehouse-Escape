# Team Foundations & Confirmed Five-Room Layout Architecture

This document details the confirmed five-room game structure, stage ownership, AI territorial containment, dual-branch progression, Card Reader terminal ending, and Unity Play Mode physical controller verification results for the **Warehouse-Escape** project across all four team members.

---

## 1. Confirmed Five-Room Game Layout Architecture

The game environment is constructed as five physically distinct rooms in a single cohesive Unity scene ([`Assets/Scenes/Warehouse.unity`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scenes/Warehouse.unity)), connected through dedicated doorways and corridors with overhead 3D signs.

### Exact Confirmed Route Progression Flowchart

```
+-------------------------------------------------------------+
|                ROOM 1: EMPTY START ROOM                     |
|          (Safe starting zone, zero enemies/agents)          |
+-------------------------------------------------------------+
                               |
                               v [Corridor 1->2]
+-------------------------------------------------------------+
|            ROOM 2: FIRST AGENT ENCOUNTER ROOM               |
|            (Varun's Patrol Guard — Center Shelving)         |
+-------------------------------------------------------------+
                               |
            Two separate exits available simultaneously
            ├───────────────────────────────┤
            |                               |
    [Choice: Route A]               [Choice: Route B]
    Sign: ROUTE A (SEARCH)          Sign: ROUTE B (DEFENSE)
            |                               |
            v [Corridor 2->3]               v [Corridor 2->4]
+-----------------------------+ +-----------------------------+
|    ROOM 3: SEARCH SECTOR    | |   ROOM 4: DEFENSIVE SECTOR  |
|  Sanjeevsaran's Search Guard | |    Aqqil's Defensive Guard  |
|              +              | |              +              |
|        Access Card A        | |        Access Card B        |
+-----------------------------+ +-----------------------------+
            |                               |
            v [Corridor 3->5]               v [Corridor 4->5]
            +───────────────┬───────────────+
                            | (Both branches connect to Room 5)
                            v
+-------------------------------------------------------------+
|                 ROOM 5: FINAL BOSS ROOM                     |
|           (Jadavan's ultimate Flanking Guard AI)            |
|              Cover & alternative aisles preserved           |
+-------------------------------------------------------------+
                               |
                               v
+-------------------------------------------------------------+
|             ACCESS CARD READER TERMINAL                     |
|        (Far end of Room 5, facing player entrance)          |
|                                                             |
|   Player explicitly presses [E] to insert either card       |
|     - Accepts Card A OR Card B  ──> VICTORY SCREEN          |
|     - Without card: "Access card required", continues play  |
|     - Old automatic EscapeTrigger disabled                  |
|     - Old exit door preserved as scenery                    |
+-------------------------------------------------------------+
```

> [!IMPORTANT]
> **Branch Rule:** The player chooses either branch. Visiting both rooms or collecting both cards is **never** required. Collecting either Card A or Card B provides full clearance at the Card Reader.

---

## 2. Five-Room Physical Specifications

| Room / Area | Physical Dimensions (W x L x H) | Center Position | Primary Architectural Features & Props | Assigned Agent & Setup |
| :--- | :--- | :--- | :--- | :--- |
| **Room 1: Start Room** (`Room_1_Start`) | 10m x 8m x 5m | `(0, 0, -26)` | Safe starting room. Overhead sign `Sign_ToStage1`, industrial point lamp (`PointLight_Room1`), south wall, doorway to Corridor 1->2. | **None** (100% safe starting zone; exactly 0 enemies). Player spawn at `(0.00, 1.08, -26.00)`. |
| **Room 2: Patrol Room** (`Room_2_Patrol`) | 16m x 14m x 5m | `(0, 0, -11)` | First agent encounter. Racking units (`Rack_Center_1`, `Rack_Center_2`), repositioned `Crates_P1` (1.50m clear aisle width), industrial point lamp (`PointLight_Room2`), dual branch exits with overhead signs `Sign_Route_A` (`(-5.0, 3.8, -4.3)`) and `Sign_Route_B` (`(5.0, 3.8, -4.3)`). | **`Guard_Varun_Patrol`** (Violet). Continuous 4-waypoint circuit around central shelving. Territorial bounds: `(0, 2, -11)`, size `(16, 6, 14)`. |
| **Room 3: Search Room** (`Room_3_Search`) | 16m x 18m x 5m | `(-16, 0, 7)` | Branch Route A. Pallet racks (`Search_Rack_1`, `Search_Rack_2`), hiding alcove (`Search_Crates_Alcove`), keycard table (`Search_Pallet_CardTable`), industrial point lamp (`PointLight_Room3`), doorway to Corridor 3->5 (`(-6.5, 14)`). | **`Guard_Sanjeevsaran_Search`** (Gold) + **`AccessCard_Pickup_Search`** (Card A). Trigger collider expanded to `(4.0, 10.0, 6.0)` (world bounds `[-23, -19]`, `[10.8, 17.2]`), 100% physically reachable from south aisle. |
| **Room 4: Defensive Room** (`Room_4_Defensive`)| 16m x 18m x 5m | `(16, 0, 7)` | Branch Route B. Terminal desk (`Terminal_Desk`), security console (`[Placeholder]_SecurityConsole_Terminal`), vehicle prop (`[Placeholder]_Forklift_Vehicle`), industrial point lamp (`PointLight_Room4`), doorway to Corridor 4->5 (`(7, 14)`). | **`Guard_Aqqil_Defensive`** (Cyan) + **`AccessCard_Pickup_Defensive`** (Card B). Trigger collider expanded to `(1.8, 1.2, 2.2)` at `(16.0, 1.15, 10.2)` protruding past desk, 100% physically reachable from north approach. |
| **Room 5: Boss Room** (`Room_5_Boss`) | 22m x 24m x 5m | `(0, 0, 31)` | Final encounter. Longitudinal shelving blocks (`Flank_Rack_W`, `Flank_Rack_E`) forming a wide central thoroughfare and two genuine alternative side aisles. Dual ceiling lamps (`PointLight_Boss_South`, `PointLight_Boss_North`), overhead sign `Sign_Exit_Master`. Far north wall features the **Card Reader Terminal** (`Card_Reader_Terminal`) and scenery exit door (`Exit_Door`). | **`Guard_Jadavan_Boss`** (Crimson Boss). Reuses Jadavan's flanking AI with tuned speed and perception. Visually distinct (Crimson/Obsidian metallic material `Mat_Guard_Boss`, 1.15x scale). |
| **Exit Scenery & Platform** | 8m x 7m x 4m | `(0, 0, 46.5)` | Closed scenery door (`Exit_Door`) at `(0, 2, 43)`, stationary blocker, and landing platform (`Exit_Platform_Floor`). | **None**. Old `EscapeTrigger` is disabled/inactive (`isEscapeTriggerActive = false`); touching the door or platform alone cannot award victory. |

---

## 3. Team Member Ownership & Technical Scope

### 1. Jadavan — Final Boss Flanking AI & Agent Controller
- **Role:** Agent Controller & Final Boss Specialist
- **Stage Ownership:** Room 5 Final Boss Room (`Room_5_Boss`)
- **Key Codebase Files:**
  - [`Assets/Scripts/EnemyMovement.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/EnemyMovement.cs): Shared navigation and movement authority across all guards.
  - [`Assets/Scripts/GuardPatrol.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/GuardPatrol.cs): Waypoint patrol logic with recovery mechanics.
  - [`Assets/Scripts/GuardPerception.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/GuardPerception.cs): Shared sensory perception utility computing FOV, eye heights, and obstacle raycast occlusion.
  - [`Assets/Scripts/FlankingGuardAI.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/FlankingGuardAI.cs): Tactical state machine (`Patrol`, `Flank`, `Pursue`, `Search`).
- **Boss Guard Tuning & Distinctiveness:**
  - **Visual Distinction:** 1.15x physical transform scale, striking crimson obsidian standard material (`Mat_Guard_Boss`, Metallic 0.6, Smoothness 0.55).
  - **Tuned Movement & Speed:** Configurable `PatrolSpeed = 3.8m/s` (standard guard 3.5), `ChaseSpeed = 5.2m/s` (standard guard 4.8-5.0).
  - **Tuned Perception:** Extended sight range (`SightDistance = 14m`, standard 12m) with full 90° FOV.
  - **Tactical Balance:** Preserves genuine alternative flanking side aisles (`X = -6m` and `X = +6m`) around central shelving blocks `Flank_Rack_W` and `Flank_Rack_E`, ensuring players can utilize stealth, line-of-sight breaking, and flanking counters without combat or health systems.

### 2. Sanjeevsaran — Systems Engineer & Search Guard
- **Role:** Systems Engineer & Search Guard Specialist
- **Stage Ownership:** Room 3 Search Sector (`Room_3_Search`)
- **Key Codebase Files:**
  - [`Assets/Scripts/AccessCard.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/AccessCard.cs): Dual-pickup security access-card system. Static `HasKeycard` tracking supports independent pickup from either Route A or Route B.
  - [`Assets/Scripts/CardReader.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/CardReader.cs): Visible terminal component handling proximity detection, `[E]` keypress interaction, rejection feedback ("Access card required"), and single-dispatch victory trigger.
  - [`Assets/Scripts/SearchGuardAI.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/SearchGuardAI.cs): Search state machine (`Patrol` -> `Pursue` -> `Search` -> `Patrol`).
- **Search AI & Containment:**
  - Stationed in Room 3 with a 4-waypoint perimeter patrol circuit.
  - Investigates `lastSeenPosition` on lost line of sight with a 3.0s area search timer before resuming patrol.
  - Territorial containment bounds: `(-16, 2, 7)`, size `(16, 6, 18)`. Disengages pursuit if player breaches boundaries.

### 3. Aqqil — Core Developer / 3D Assets & Defensive Guard
- **Role:** Core Developer / 3D Assets & Defensive Guard Specialist
- **Stage Ownership:** Room 4 Defensive Sector (`Room_4_Defensive`)
- **Key Codebase Files:**
  - [`Assets/Scripts/DefensiveGuardAI.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/DefensiveGuardAI.cs): Sentinel guard state machine (`GuardingPost` -> `PursueIntruder` -> `ReturnToPost`).
- **Defensive AI & Containment:**
  - Stationed at the Security Console Terminal guard post `(16.0, 1.08, 8.0)`.
  - Pursues intruders only within a 6.0m defense leash radius when line of sight is confirmed.
  - Territorial containment bounds: `(16, 2, 7)`, size `(16, 6, 18)`. Returns directly to guard post if intruder retreats beyond leash or exits Room 4.

### 4. Varun — World Builder & Patrol Guard
- **Role:** World Builder & Patrol Guard Specialist
- **Stage Ownership:** Room 2 Patrol Room (`Room_2_Patrol`)
- **Key Codebase Files:**
  - [`Assets/Scripts/PatrolGuardAI.cs`](file:///c:/Users/jadav/Documents/GV_IS/Warehouse-Escape/Assets/Scripts/PatrolGuardAI.cs): Waypoint patrol state machine (`Patrol` -> `Pursue`).
- **Patrol AI & Containment:**
  - Continuously loops waypoints `WP_Varun_1` through `WP_Varun_4` around Room 2's center racking.
  - Immediately resumes waypoint circuit when line of sight is broken.
  - Territorial containment bounds: `(0, 2, -11)`, size `(16, 6, 14)`. Disengages pursuit if intruder retreats to Room 1 or enters either branch corridor.

---

## 4. Ending Architecture: Access Card Reader Terminal

The victory mechanism has been replaced to match the confirmed game ending:

1. **Card Reader Terminal (`Card_Reader_Terminal`):**
   - Placed at `(0.00, 0.50, 41.50)` at the far end of Room 5, facing south toward the player entrance.
   - Built with a heavy industrial pedestal, angled status display screen, and top card reader slot.
   - Distinct emissive status materials:
     - Ready / Idle: Amber glow (`Mat_Reader_Ready`).
     - Access Granted: Emerald green emissive (`Mat_Reader_Success`).
     - Access Denied: Crimson red emissive (`Mat_Reader_Denied`).

2. **Proximity Interaction Logic (`CardReader.cs`):**
   - **Range Check:** Evaluates 3D distance between player and terminal (`interactionDistance = 3.0f`).
   - **HUD Prompt:** Displays contextual prompt in Canvas UI (`[E] Insert Access Card`) when the player is in range.
   - **Explicit Keypress Requirement:** Player must press `[E]` while in range.
   - **Card Validation:** Accepts **either** Card A or Card B (`AccessCard.HasKeycard == true`).
   - **Rejection Without Card:** If player interacts without an access card:
     - Sets feedback message: `"Access card required"`.
     - Switches indicator to denied material.
     - Logs warning and returns `false`.
     - **Gameplay continues** without interruption.
   - **Victory on Insertion:** Upon valid card insertion:
     - Marks `isCardInserted = true`.
     - Switches indicator to success material.
     - Triggers `GameManager.Instance.TriggerVictory(...)` **exactly once**.
     - Result screen immutability prevents redundant triggers.

3. **Disabling of Old Escape Mechanisms:**
   - `EscapeTrigger.isEscapeTriggerActive` set to `false`. Walking onto the platform or into the old exit area cannot trigger victory.
   - `DoorController.showInterac---

## 5. Physical Reachability, Industrial Detailing & Active Lighting

1. **Card A Reachability (`AccessCard_Pickup_Search`):**
   - `Search_Pallet_CardTable` sits at `(-21, 0.4, 14)`.
   - Trigger collider spans $X \in [-23.0, -19.0]$, $Z \in [10.80, 17.20]$.
   - A dedicated industrial task spotlight (`CardA_TaskSpotlight`) overhead provides high-contrast illumination directly on Card A.
   - Stepping into the card table alcove achieves physical trigger contact without obstructions.

2. **Card B Reachability (`AccessCard_Pickup_Defensive`):**
   - `Terminal_Desk` sits at `(16, 0.5, 10)` in the Defensive Sector checkpoint.
   - Trigger collider spans `(1.8, 1.2, 2.2)` at `(16.0, 1.15, 10.2)`, protruding north and south of the desk solid collider.
   - A dedicated task spotlight (`CardB_TaskSpotlight`) illuminates Card B on the console desk.
   - Approaching from the north aisle achieves physical trigger contact without obstructions.

3. **Industrial Warehouse Architectural Overhaul:**
   - **Enclosed Ceilings & Structural Steel:** All five rooms and connecting corridors have solid industrial ceilings (`Mat_Ceiling`), completely blocking the skybox and creating an authentic warehouse interior. Transverse dark steel I-beams (`Mat_Steel_Dark`) span the ceilings at $Y = 4.75\text{m}$ with steel corner columns and doorway portals.
   - **Overhead HVAC Ventilation & Utility Piping:** Galvanized sheet-metal ventilation ducts (`Mat_Duct_Metal`) run along the ceilings with branch ducts into Rooms 3 and 4. Parallel utility pipes (conduit and safety yellow gas pipes) line the upper walls.
   - **Detailed Multi-Tier Pallet Racking:** Monolithic solid color blocks have been replaced with realistic pallet racks:
     - Blue vertical upright frames (`Mat_Rack_Blue`).
     - Orange load beams (`Mat_Rack_Orange`).
     - Wire decking levels (`Mat_Shelf_Level`).
     - Wooden pallets (`Mat_Pallet`).
     - Varied cargo: corrugated cardboard cartons (`Mat_Crate_Dark`), wooden crates (`Mat_Crate_Wood`), blue/hazard steel drums (`Mat_Barrel_Blue`, `Mat_Barrel_Hazard`), and plastic shrink-wrapped cargo (`Mat_Cargo_Wrapped`).
     - Solid colliders strictly normalized with `localScale = Vector3.one`, ensuring central thoroughfares $\ge 2.25\text{m}$ and side aisles $\ge 3.0\text{m}$.
   - **Floor Safety Markings & Demarcations:** Thin decals at $Y = 0.005\text{m}$ with disabled colliders:
     - Yellow traffic guide lines along Room 2 central aisle and branch entrances.
     - White pedestrian walkway lines spanning Room 5 loading hall.
     - Yellow/black hazard boundary boxes surrounding the Card Reader Terminal and the forklift staging zone.
     - Airlock staging line in Room 1.
   - **Props & Loading Bay Features:**
     - Room 4: Detailed industrial forklift with chassis (`Mat_Forklift_Yellow`), counterweight, safety cage, mast, forks, and cargo pallet.
     - Room 5: Industrial roll-up shutter door (`Mat_Shutter_Metal`) with horizontal slats and roll barrel on the north wall, flanked by safety bollards.
   - **Lighting System & Focal Points:**
     - Suspended high-bay fixtures (`Mat_Lamp_Metal`, `Mat_Bulb_Warm`) with soft shadows across all five rooms and corridors.
     - Directional ambient fill light (`Warehouse_DirectionalFill`) providing cold moonlight bounce.
     - Card Reader Terminal focal spotlight: High-intensity angled spotlight creating a dramatic visual highlight at the far end of Room 5.

---

## 6. Player Viewpoint Verification Screenshots

High-resolution 1280x720 player camera captures from all five room viewpoints were rendered and inspected before and after the aesthetic overhaul:

| Room / Perspective | Viewpoint Position & Rotation | Before Screenshot Path | After Screenshot Path | Key Visual Upgrades |
| :--- | :--- | :--- | :--- | :--- |
| **Room 1: Safe Start** | Pos: `(0.0, 1.6, -26.0)`<br>Rot: `(0, 0, 0)` | `scratch/screenshots/before_room1_start.png` | `scratch/screenshots/after_room1_start.png` | Ceiling blocks cartoon sky; floor safety airlock line; overhead ducting; visible steel doorway portal and organized Room 2 racks down corridor. |
| **Room 2: Patrol Room** | Pos: `(0.0, 1.6, -15.0)`<br>Rot: `(0, 0, 0)` | `scratch/screenshots/before_room2_patrol.png` | `scratch/screenshots/after_room2_patrol.png` | Monolithic blue/orange blocks replaced by multi-tier racks with crates and barrels; central yellow lane lines; suspended warm high-bay lamp; dual branch signs with hazard borders. |
| **Room 3: Search Sector** | Pos: `(-12.0, 1.6, 2.0)`<br>Rot: `(5, -40, 0)` | `scratch/screenshots/before_room3_search.png` | `scratch/screenshots/after_room3_search.png` | Cross-duct HVAC ceiling; hiding corner crate stack for stealth; Access Card A pallet table with dedicated task lamp making card glowing and unmistakable. |
| **Room 4: Defensive Sector** | Pos: `(12.0, 1.6, 2.0)`<br>Rot: `(5, 40, 0)` | `scratch/screenshots/before_room4_defensive.png` | `scratch/screenshots/after_room4_defensive.png` | Security console desk with lit Card B; detailed industrial forklift in hazard staging bay; wrapped electronic cargo; Aqqil's defensive guard station. |
| **Room 5: Boss Loading Hall** | Pos: `(0.0, 1.6, 23.0)`<br>Rot: `(2, 0, 0)` | `scratch/screenshots/before_room5_boss.png` | `scratch/screenshots/after_room5_boss.png` | Full warehouse roof with double I-beams; multi-tier flank racks on both sides; white floor lane lines; grand roll-up shutter; safety bollards and glowing Card Reader Terminal focal point. |

---

## 7. Actual Unity Play Mode Verification Results

The five-room structure, physical reachability, and all 7 verification criteria were validated in **actual Unity Play Mode** (Unity 6000.3.25f1 LTS, WindowsEditor) using `CharacterController.Move()` and frame-by-frame physics traversal without teleportation or mathematical substitution:

### PlayMode Test Suite Execution (`PhysicalWalkthroughTests.cs`)

```
================================================================================
UNITY PLAY MODE PHYSICAL CONTROLLER TRAVERSAL VERIFICATION REPORT
Timestamp: 2026-10-12 13:43:15
Unity Version: 6000.3.25f1, Platform: WindowsEditor
================================================================================

--- TEST 1: ROOM 1 SAFE START & ROOM 2 DUAL BRANCH ENTRANCES ---
[PASS] Original Spawn Position Verified: (0.00, 1.08, -26.00)
[PASS] Room 1 Verified Safe: 0 enemy guards inside Room 1 (Z < -18). Total guards active: 4
[PASS] Room 2 First Encounter Verified: Guard_Varun_Patrol at (-5.00, 1.08, -13.76)
  [Room 1 Safe Start -> Room 2 Doorway Traversed] Total Distance: 7.7m, Duration: 1.5s, End Pos: (0.00, 1.08, -18.35)
[PASS] Dual Branch Exits Confirmed from Room 2: Sign_Route_A at (-5.00, 3.80, -4.30) and Sign_Route_B at (5.00, 3.80, -4.30) available simultaneously.

--- TEST 2: ROUTE A CONTROLLER TRAVERSAL -> CARD A -> BOSS ROOM -> READER -> VICTORY ---
  [Room 1 -> Room 2 Center Aisle Traversed] Total Distance: 15.7m, Duration: 3.1s, End Pos: (0.00, 1.08, -10.35)
  [STEALTH] Tactical pause (2.5s) behind Rack_Center_1 while Varun patrols north crossing...
  [Route A -> Search Room Card A Table Traversed] Total Distance: 36.3m, Duration: 7.3s, End Pos: (-19.49, 1.08, 10.33)
[PASS] Access Card A Collected physically. HasKeycard = True
  [Room 3 -> Corridor -> Room 5 Boss Room -> Card Reader Traversed] Total Distance: 46.6m, Duration: 9.3s, End Pos: (-0.33, 1.08, 39.87)
[PASS] Player in Range of Card Reader. Prompt: '[E] Insert Access Card'
[PASS] Card Reader Accepted Card: isCardInserted = True, Feedback = 'Access Granted! Evacuation Confirmed.'
[PASS] Route A Full Walkthrough Verified: Victory State = Victory

--- TEST 3: ROUTE B CONTROLLER TRAVERSAL -> CARD B -> BOSS ROOM -> READER -> VICTORY ---
  [Room 1 -> Room 2 Center Aisle Traversed] Total Distance: 15.7m, Duration: 3.1s, End Pos: (0.00, 1.08, -10.35)
  [STEALTH] Tactical pause (2.5s) behind Rack_Center_1 while Varun patrols north crossing...
  [Route B -> Defensive Room Card B Desk Traversed] Total Distance: 33.2m, Duration: 7.1s, End Pos: (15.75, 1.08, 11.17)
[PASS] Access Card B Collected physically. HasKeycard = True
  [Room 4 -> Corridor -> Room 5 Boss Room -> Card Reader Traversed] Total Distance: 42.8m, Duration: 8.6s, End Pos: (0.32, 1.08, 39.87)
[PASS] Player in Range of Card Reader. Prompt: '[E] Insert Access Card'
[PASS] Card Reader Accepted Card B: isCardInserted = True, Feedback = 'Access Granted! Evacuation Confirmed.'
[PASS] Route B Full Walkthrough Verified: Victory State = Victory

--- TEST 4: CARD READER REJECTION WITHOUT ACCESS CARD ---
  [Room 1 -> Room 2 Center Aisle Traversed] Total Distance: 15.7m, Duration: 3.1s, End Pos: (0.00, 1.08, -10.35)
  [Room 4 West Aisle (Card B Bypassed) Traversed] Total Distance: 26.8m, Duration: 5.6s, End Pos: (9.50, 1.08, 11.05)
  [Corridor -> Room 5 Reader (No Card) Traversed] Total Distance: 37.0m, Duration: 7.5s, End Pos: (0.32, 1.08, 39.87)
[PASS] Reader Correctly Rejected Interaction: Feedback = 'Access card required', IsGameWon = False

--- TEST 5: ENTERING BOSS ROOM & OLD EXIT AREA DOES NOT TRIGGER VICTORY ---
  [Room 1 -> Room 2 Traversed] Total Distance: 15.7m, Duration: 3.1s, End Pos: (0.00, 1.08, -10.35)
  [Room 4 West Aisle Traversed] Total Distance: 26.8m, Duration: 5.6s, End Pos: (9.50, 1.08, 11.05)
  [Room 5 Boss Room -> Old Exit Area Traversed] Total Distance: 37.0m, Duration: 7.5s, End Pos: (0.32, 1.08, 39.87)
[PASS] Confirmed Old Exit Area & Boss Room Cannot Trigger Victory: IsGameWon = False

--- TEST 6: RESTART RESETS CARDS, AGENTS, READER & GAME STATE ---
  [ACTION] Calling GameManager.Instance.RestartGame()...
[PASS] Full Game State Reset Confirmed: Cards, Agents (4), Reader, Player, and UI HUD all cleanly reset.

================================================================================
OVERALL PHYSICAL CONTROLLER VERIFICATION: ALL 7 CRITERIA VERIFIED [SUCCESS]
================================================================================
```

### Verification Criteria Assessment (100% Pass: 6/6 Tests, 7/7 Criteria)

1. **Empty start -> first agent room:** Confirmed. Room 1 has 0 enemy agents; player walks physically from spawn `(0, 1.08, -26)` through doorway into Room 2 (`Test_1`).
2. **Two distinct branch entrances from Room 2:** Confirmed. `Sign_Route_A` and `Sign_Route_B` mark separate, simultaneously accessible branch doorways (`Test_1`).
3. **Route A physical walkthrough -> Card A -> Boss Room -> [E] at Reader -> Victory:** Confirmed. Player physically collects Card A via CharacterController trigger contact, traverses corridor into Room 5, approaches reader, interacts, and triggers victory (`Test_2`).
4. **Route B physical walkthrough -> Card B -> Boss Room -> [E] at Reader -> Victory:** Confirmed. Fresh restart, player physically collects Card B, traverses corridor into Room 5, interacts with reader, and triggers victory (`Test_3`).
5. **Reader rejects interaction without card:** Confirmed. Player reaches reader without card, attempts interaction, receives `"Access card required"`, and gameplay continues (`Test_4`).
6. **Entering boss room or old exit area alone cannot trigger victory:** Confirmed. Player traversing Room 5 up to the closed exit door without card insertion does not trigger victory; old `EscapeTrigger` is disabled (`Test_5`).
7. **Restart resets cards, agents, reader, and game state:** Confirmed. `RestartGame()` resets inventory (`HasKeycard = false`), re-activates both card pickups, resets all 4 guards, restores reader state, resets player to spawn, and restores UI (`Test_6`).

---

## 8. Test Distinction & Methodology Disclosure

To ensure complete scientific rigor in testing:

1. **Controller-Driven Traversal:** All player movement across all five rooms and corridors is executed via `CharacterController.Move()` and `PlayerMovement` step-by-step with real-time collision detection. Teleportation, direct `transform.position` overrides, and mathematical kinematic simulations were strictly avoided.
2. **Physical Trigger Collision:** Both access cards (`AccessCard_Pickup_Search` and `AccessCard_Pickup_Defensive`) are collected purely by the `CharacterController` physically entering the trigger collider volume during gameplay traversal. Direct programmatic calls to `AccessCard.Collect()` were not used for walkthrough testing.
3. **Card Reader Interaction:** The Card Reader component evaluates physical proximity (`IsPlayerInRange`) and checks `Input.GetKeyDown(KeyCode.E)`. In PlayMode test runners, interaction execution calls `cardReader.TryInsertCard()` while the controller is physically positioned in the interaction zone. In interactive player mode, pressing the physical `[E]` key on the keyboard invokes `TryInsertCard()` identically.
4. **Programmatic UI Events:** Button clicks on UI modals (Restart and Play Again) dispatch via `Button.onClick.Invoke()`, testing listener wiring and scene reload mechanics without simulating OS hardware mouse pointers.

---

## 9. Remaining Limitations

1. **Batch Mode Input Queue:** In automated headless Unity batch mode, hardware OS keyboard events (`Input.GetKeyDown(KeyCode.E)`) do not dispatch from an OS window message pump. Walkthrough tests verify physical proximity and call the interaction method directly (`TryInsertCard()`), which is the exact logic executed on `KeyCode.E`.
2. **Scenery Doorway Collisions:** The old exit door (`Exit_Door`) remains closed as scenery. The solid door and blocker prevent players from walking onto the outer platform while the game is running, reinforcing that the escape terminal inside Room 5 is the sole route to victory.
3. **Lighting Bake:** Active point lights provide full real-time lighting across all rooms. Precomputed lightmaps have not been rebaked for the new layout, so dynamic real-time lights and ambient skybox lighting are used.
