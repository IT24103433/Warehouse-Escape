# Viva Guide — Core Developer & Defensive Guard (Aqqil M.)

**Student:** Aqqil Mohammed (`IT24104314`)  
**Dual Role:**
1. **SE3032 / Core Developer:** Blender 3D modeling (`Assets/Meshes/Source/*.blend`), UV/pivot normalization, decimation, and URP materials.
2. **SE3062 / Intelligent Systems (Room 4):** `DefensiveGuardAI.cs`, `DefenseDecisionPolicy.cs` (objective-preserving tactical defense).

---

## 1. Blender Asset & Script Map

| Asset / Script | Responsibility | Shared Dependencies |
| --- | --- | --- |
| `Assets/Meshes/Source/*.blend` | Source Blender scenes (`Access_Card.blend`, `IndustrialServicePanel.blend`, `Security_Rifle.blend`, `Tactical_Pistol.blend`, `Warehouse_Cargo_Crate.blend`, `Warehouse_Security_Barrier.blend`, `Warehouse_Steel_Rack.blend`) | Exported FBX under `Assets/Meshes/Props/` & `Assets/Meshes/Weapons/` |
| `Documentation/Tools/generate_service_panel.py` | Procedural Blender export for beveled `IndustrialServicePanel.fbx` | Blender 4.x Python API |
| `Assets/Scripts/DefensiveGuardAI.cs` | Room 4 `GuardingPost` / `PursueIntruder` / `ReturnToPost` state machine (`220 HP`, `Combat_Shotgun`, `6.0 m` defense radius, `8.5 m` leash) | `EnemyMovement`, `GuardPerception`, `GuardPerceptionMemory`, `GuardDecisionQueries`, `GuardShooting`, `Health`, `HumanoidAnimator` |
| `Assets/Scripts/DefenseDecisionPolicy.cs` | Pure static tactical utility scoring (`hasThreat` combat vs. idle guard weights) | `UnityEngine.Mathf` |

---

## 2. Blender Modeling, UV & Pivot Workflow

1. **Scale & Axis Convention:** `1.0 BU = 1.0 m` with transforms applied (`apply_unit_scale=True`); FBX export aligned to `-Z` forward, `Y` up so Unity instancing needs zero corrective offsets.
2. **Pivot Normalization:**
   - **Floor Props (`Crate`, `Steel_Rack`, `Security_Barrier`):** Bottom-center pivot `(0, 0, 0)` for deterministic floor placement and NavMesh bounds.
   - **Wall Prop (`IndustrialServicePanel`):** Backplane pivot `(0, 0, -0.095)`, collider-free visual detail (`Cinematic_Visual_Details`).
   - **Pickups & Weapons (`Access_Card`, `MP5_SMG`, `Combat_Shotgun`):** Card centered (`2.5x` scale); firearms pivoted at right-hand grip with `+Z` barrel alignment in `Hand_R`.
3. **Topology & UVs:** Quad-first base geometry, `BEVEL` (`1-3` segments) + `WEIGHTED_NORMAL` (`Keep Sharp = ON`) instead of heavy Subsurf, interior bottom-cap removal, `UV0` padding `>= 0.015` for `URP/Lit` textures/normal maps, and `UV1` non-overlapping lightmap UVs.

---

## 3. Tactical Defense AI (`DefensiveGuardAI` & `DefenseDecisionPolicy`)

- **Guard / Attack / Retreat States:**
  - `GuardingPost` (**Guard**): Evaluates `17` reachable positions (`postPosition` + `2.5 m` & `5.0 m` 8-point rings inside `roomBounds` and `leashDistance = 8.5 m`) and inspects with `+/-28 deg` aim sweeps.
  - `PursueIntruder` (**Attack**): Triggered when the player is visible inside `defenseRadius` (`6.0 m`); repositions every `0.75 s` (`nextTacticalDecision`) using threat-weighted utility.
  - `ReturnToPost` (**Retreat / Reset**): Triggered when the intruder or guard exceeds `leashDistance` (`8.5 m`), sight is lost outside `defenseRadius`, or either leaves Room 4.
- **Utility Scoring (`DefenseDecisionPolicy.Score`):**
  - **Without Recent Threat (`HasThreat == false`):**
    $$U = 0.45 \cdot \text{objective} + 0.30 \cdot \text{visibility} + 0.25 \cdot \text{freshness} - 0.15 \min\!\left(\frac{\text{pathLength}}{20}, 1\right)$$
  - **With Recent Visual or Sound Threat (`HasThreat == true`, `< 7.5 s`):**
    $$U = 0.30 \cdot \text{objective} + 0.25 \cdot \text{cover} + 0.30 \cdot \text{weaponRange} + 0.15 \cdot \text{freshness} - 0.15 \min\!\left(\frac{\text{pathLength}}{20}, 1\right)$$
  - **Cover Evaluation:** Torso ray (`+0.8 m`) blocked + eye ray (`+1.5 m`) clear scores `1.0` (ideal crouch/barrier cover); both blocked scores `0.5` with `weaponRange * 0.25`; torso clear scores `0.0`.
  - **Shotgun Range & Switching Bonus:** Peaks at `~5 m` (`1 - |d - 5| / 6`) for `Combat_Shotgun` (`6` pellets); adds `+0.04` switching bonus to `selectedTacticalIndex` during active threats to prevent post oscillation.

---

## 4. Live Demo Sequence (2 Minutes)

1. **Blender Assets:** Open `Assets/Meshes/Source/` and `Documentation/CoreAssetsAndDefense/AssetPipelineLog.md` to show normalized pivots, bevels, and `UV0`/`UV1` layout.
2. **Tactical Defense (`Room 4`):** Select `Guard_Aqqil_Defensive` in Play Mode to show cyan `defenseRadius` (`6 m`), blue `leashDistance` (`8.5 m`), and magenta `SelectedDecisionTarget`.
3. **Cover vs. Exposed Tradeoff & Leash:** Step into Room 4 so Aqqil shifts behind cover near `5 m` (`cover = 1.0`), then retreat past `8.5 m` to demonstrate `ReturnToPost` retreat and objective preservation (`Access Card B`).
