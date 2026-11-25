# Verification Log — Automated PlayMode Suite (58 Tests) & Manual Viva Walkthrough

**Owner:** Sanjeevsaran J. (`IT24104155`)  
**Scope:** Distinction between the **58 automated NUnit PlayMode tests** in `Assets/Tests/` and manual viva demonstration checks for Route A (`Access Card A`, `SearchGuardAI`) and Room 5 (`CardReader`).

## 1. Automated PlayMode Test Suite Breakdown (58 Total `[Test]` / `[UnityTest]` Cases)

| Test File | Test Count | Primary Coverage |
| --- | --- | --- |
| `Assets/Tests/PhysicalWalkthroughTests.cs` | **41** | Route A & Route B physical walkthroughs, `AccessCard` trigger collection, `CardReader` `[E]` validation, weapon swaps, HUD/minimap layout, guard perception & audio |
| `Assets/Tests/InformedGuardDecisionTests.cs` | **6** | `PatrolDecisionPolicy`, `SearchDecisionPolicy` (observed heading, travel penalty, `0.08` negative-evidence discount), and `DefenseDecisionPolicy` |
| `Assets/Tests/TrapAndSquadTests.cs` | **6** | 5-room bypassable traps (`WarehouseTrap`), warning/pause/evasion, Room 5 dual-boss `1.3 s` suppression turns, confirmed-sighting sharing, partner-death fallback |
| `Assets/Tests/DisplayAndSurfaceTests.cs` | **2** | Pause menu resolution choices and surface detail / texture quality verification |
| `Assets/Tests/CombatControlsTests.cs` | **1** | Player health, crouch, blood effects, and guard hearing integration |
| `Assets/Tests/HeadshotAndShotgunTests.cs` | **1** | Double-damage headshots, shotgun pellet spread, and cover occlusion |
| `Assets/Tests/VisualQualityCaptureTests.cs` | **1** | 5-room player camera visual capture across saved scene |
| **Total Automated Tests** | **58** | Run via Unity Test Runner or **Warehouse > Run Physical Walkthrough Tests** (`scratch/verification-results.xml`) |

## 2. Key Automated Tests Covering Systems & Search Guard

| Test Name | File | Verified Behavior |
| --- | --- | --- |
| `Test_2_RouteA_Physical_Walkthrough_To_CardReader_Victory` | `PhysicalWalkthroughTests.cs` | Navigates Room 1 -> Room 2 -> Room 3, collects `Access Card A` via `OnTriggerEnter`, reaches Room 5 `CardReader`, triggers single victory |
| `Test_3_RouteB_Physical_Walkthrough_To_CardReader_Victory` | `PhysicalWalkthroughTests.cs` | Verifies independent Route B (`Access Card B`) clearance into the same Room 5 `CardReader` |
| `Test_33_Weapon_Pickup_Prompt_Formatting_And_Swap_Action` | `PhysicalWalkthroughTests.cs` | Confirms `WeaponPickup` `[E]` prompt priority and reload cancellation during swap |
| `Test_40_GuardPerception_DetectionRealism_CoverAndDoorways` | `PhysicalWalkthroughTests.cs` | Confirms `sightDistance`, horizontal `fieldOfView`, and solid-cover line-of-sight blocking |
| `Test_41_All_Guards_Investigate_Sound_Without_Blind_Fire` | `PhysicalWalkthroughTests.cs` | Confirms gunshot/alarm investigation without firing unless visual LOS is confirmed |
| `Search_Uses_Observed_Heading_Travel_And_Negative_Evidence` | `InformedGuardDecisionTests.cs` | Unit check on `SearchDecisionPolicy.Prior`, `Score`, and `NegativeObservation` (`* 0.08f`) |
| `Search_Reorders_Hypotheses_And_Reduces_Belief_After_Inspection` | `InformedGuardDecisionTests.cs` | Integration check verifying `SearchGuardAI` updates `RemainingBeliefMass` and reprioritizes candidates after an empty inspection |

## 3. Manual Viva Walkthrough Checks (Interactive Demonstration)

The following manual checks are performed interactively in Play Mode during the viva and are **distinct from** the 58 automated NUnit tests above:

| Check ID | Demo Action | What to Observe Live |
| --- | --- | --- |
| `VIVA-MAN-01` | Approach Room 5 `CardReader` without a card and press `[E]` | Prompt shows `[E] Insert Access Card`; status shows `Access card required` (`deniedMaterial`) and gameplay continues |
| `VIVA-MAN-02` | Drop/aim at a `WeaponPickup` within `3.0 m` of `CardReader` | `CardReader.GetInteractionPrompt()` yields (`""`) so `[E]` swaps the weapon instead of interacting with the reader |
| `VIVA-MAN-03` | Enter Room 3 (`Search & Card A`), trigger `SearchGuardAI` (`AK47_Rifle`), then break LOS behind shelving | Guard enters `Search`, generates up to `17` cyan gizmo points around `lastSeenPosition`, inspects highest `U_i` candidate, and applies `0.08` discount |
| `VIVA-MAN-04` | Remain hidden until `RemainingBeliefMass < 0.05` or `searchTimeout` expires | Search terminates cleanly without reading live player coordinates and returns `SearchGuardAI` to Route A patrol |
| `VIVA-MAN-05` | Walk into `Access Card A` trigger volume (`OnTriggerEnter`), return to Room 5, press `[E]` | Card auto-collects on contact (`AccessCard.HasKeycard = true`), `CardReader` shows `Access Granted! Evacuation Confirmed. [Access Card A]` and triggers Victory once |
