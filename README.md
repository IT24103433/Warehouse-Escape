# Warehouse-Escape
Interactive 3D Warehouse Escape shooting game developed for the SE3032 Graphics and Visualization and SE3062 Intelligent Systems joint project.

## Running the game

Open the project in Unity 6000.3.25f1 and load `Assets/Scenes/Warehouse.unity`.
Use WASD to move, the mouse to aim, LMB to fire, R to reload, and E to interact or swap a targeted weapon.
Collect either route's access card and use the reader in Room 5 to escape.

The player starts with an MP5 (30 rounds, 90 reserve; 0.11 seconds between shots).
Varun uses visual-only detection and a UMP; the other guards use an AK-47, shotgun, and SCAR.
A targeted weapon pickup takes interaction priority over the card reader.

## Verification and builds

- Run PlayMode tests from Unity's Test Runner, or use **Warehouse > Run Physical Walkthrough Tests**.
- The editor menu exports NUnit results to `scratch/verification-results.xml`; use the XML or Test Runner for pass/fail status.
- Build Windows using **Warehouse > Build Windows Standalone**. The output is `Builds/WarehouseEscape_Dev/WarehouseEscape.exe`, with a build summary in `scratch/build_result.txt`.
- Blender source models are under `Assets/Meshes/Source`; exported weapon FBX files are under `Assets/Meshes/Weapons`.
- Builds, local diagnostics, and Unity recovery copies are excluded from Git.

### Latest automated verification — 18 October 2026

All 28 PlayMode tests passed in a separate project copy imported without the existing Library cache (117.6 seconds).
This includes physical completion of both escape routes with normal guard combat, perception, cover protection, damage/death, pickups, restart, reload cancellation when swapping weapons, and ammo-panel label bounds.
The scripted route player aims and fires through the real weapon APIs; these checks do not represent a manual playtest or a fresh FPS benchmark.

The clean Windows Development build also succeeded with zero errors. Two compiler warnings report the existing unused maxHearingRange field.

