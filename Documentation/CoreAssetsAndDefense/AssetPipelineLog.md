# Core Developer 3D Asset Pipeline Log

**Owner:** Aqqil (`IT24104314`) — Core Developer
**Target Engine:** Unity URP (`Assets/Meshes/`, `Assets/Materials/`, `Assets/Textures/`)
**Source Tooling:** Blender 4.x (`1.0 BU = 1.0 m`, FBX `-Z` forward, `Y` up)

## 1. Pipeline Scope and Asset Ownership

This engineering log records Blender 3D modelling, topology decimation, UV unwrapping,
pivot normalization, normal-map bevel passes, and Unity Universal Render Pipeline (URP)
material setup for the warehouse props, Route A/B keycards, and hero firearms.

### 1.1 Assigned Core Asset Roster

| Asset ID | Category | Repository Path | Scene & Gameplay Role |
| --- | --- | --- | --- |
| `Warehouse_Cargo_Crate` | Prop | `Assets/Meshes/Props/Warehouse_Cargo_Crate.fbx` | East alcove cover stacks & crate blocks |
| `Warehouse_Steel_Rack` | Prop | `Assets/Meshes/Props/Warehouse_Steel_Rack.fbx` | Storage shelving & sightline breaks |
| `Warehouse_Security_Barrier` | Prop | `Assets/Meshes/Props/Warehouse_Security_Barrier.fbx` | Room 4 waist-high combat cover line |
| `Industrial_ServicePanel` | Prop | `Assets/Meshes/Props/IndustrialServicePanel.fbx` | Beveled wall electrical cabinet (visual only) |
| `Access_Card` | Objective | `Assets/Meshes/Props/Access_Card.fbx` | Route A & Route B keycard pickups |
| `MP5_SMG` | Weapon | `Assets/Meshes/Weapons/MP5_SMG.fbx` | Player primary SMG (`WeaponType.MP5_SMG`) |
| `Combat_Shotgun` | Weapon | `Assets/Meshes/Weapons/Combat_Shotgun.fbx` | Room 4 guard weapon & pickup (`WeaponType.Combat_Shotgun`) |

### 1.2 Consolidated Blender-to-Unity Production Checklist

- [x] Unit scale normalized (`1.0 BU = 1.0 m`, `apply_unit_scale=True`, transforms applied).
- [x] FBX axis standardized (`-Z` forward, `Y` up) across props and `generate_service_panel.py`.
- [x] Floor prop pivots locked at bottom-center `(0, 0, 0)` (`Crate`, `Rack`, `Barrier`).
- [x] Wall panel backplane pivot at `(0, 0, -0.095)` with zero NavMesh/physics obstacle.
- [x] `Access_Card` center pivot (`2.5x` pickup scale); `MP5_SMG` & `Combat_Shotgun` grip pivots (`+Z` barrel).
- [x] Controlled `BEVEL` + `WEIGHTED_NORMAL` geometry; `UV0` padding `>= 0.015`, `UV1` lightmaps.
- [x] `URP/Lit` materials with tangent-space normal maps, packed masks, and SRP Batcher support.


## 2. Low-Poly Topology & Shading Guidelines

All warehouse props are authored to hit real-time URP frame budgets while keeping clean
edge highlights under overhead spotlighting and baked ambient occlusion.

### 2.1 Mesh Construction Standards

1. **Quad-First Modeling:** Author base volumes with clean quad loops and avoid n-gons
   across curved or beveled silhouettes so Unity's FBX importer triangulates predictably.
2. **Controlled Bevels + Weighted Normals:** Replace dense Subdivision Surface stacks with
   1–3 segment `BEVEL` modifiers paired with `WEIGHTED_NORMAL`. Flat face normals stay
   planar without normal-interpolation bowing across large steel or concrete panels.
3. **Hidden-Face Removal:** Delete interior cap faces resting on `Y = 0` (crate bottom
   skids, rack footplates, barrier bases) to eliminate invisible overdraw and wasted tris.
4. **Consistent Winding & Manifold Checks:** Run Blender `Mesh -> Normals -> Recalculate
   Outside` and `Select Non-Manifold` prior to every FBX export.
5. **Physics vs. Render Isolation:** Small decorative parts (bolts, vent louvers, status
   lenses) are visual-only geometry; gameplay collision relies on clean box primitives or
   NavMesh obstacle carving on primary walls/racks.
6. **Modular Edge Loops:** Maintain uniform loop spacing around recessed crate planks and
   barrier chamfers so vertex lighting and lightmap UVs never produce dark diagonal banding.
7. **Smooth Angle & Hard-Edge Discipline:** Keep `use_auto_smooth = True` (`35 deg` props,
   `45 deg` weapons) and mark UV seams along sharp edges so tangent space stays consistent.

### 2.2 Target Triangle Budgets & Bevel Tiers

| Asset Class | Target Budget | Bevel Segments | Shading Rule | Collision Strategy |
| --- | --- | --- | --- | --- |
| Small Pickups (`Access_Card`) | `<= 250 tris` | `1` (chamfer) | Smooth + weighted face normals | Trigger `BoxCollider` only |
| Cover Props (`Crate`, `Barrier`) | `<= 1,200 tris` | `1–2` | Auto-smooth `35 deg` + bevels | Box collider / NavMesh carve |
| Heavy Racks (`Steel_Rack`) | `<= 3,000 tris` | `1` | Flat upright beams + bevel | Simplified box bounds |
| Wall Detail (`ServicePanel`) | `<= 2,500 tris` | `2–3` | Smooth + `WEIGHTED_NORMAL` | Zero collider (`Cinematic_Visual_Details`) |
| Hero Weapons (`MP5`, `Shotgun`) | `<= 5,000 tris` | `1–2` | Smooth barrel normals | Pickup trigger only |

### 2.3 Modifier & Decimation Checklist

| Modifier / Operation | Parameter Target | Expected Savings | Reason |
| --- | --- | --- | --- |
| `BEVEL` (Props) | `width = 0.01–0.035 m`, `segments = 1–3`, `Clamp Overlap = ON` | Baseline specular | Catch overhead spotlight rims without pinching corners |
| `WEIGHTED_NORMAL` | `Weighting Mode = Face Area`, `Keep Sharp = ON` | `0` extra tris | Keep planar cabinet/door panels flat with smooth bevels |
| `Merge by Distance` | `threshold = 0.0001 m` | `3–6%` verts | Remove overlapping duplicate vertices after boolean/inset |
| `Dissolve Limited` | `max_angle = 5 deg` on planar spans | `18–30%` tris | Collapse redundant edge loops on flat concrete/plank planes |
| `Delete Loose / Interior` | Bottom caps & occluded overlaps | `8–15%` tris | Eliminate floor-buried & interior backface geometry |
| `Cylinder Segment Cap` | `12` verts bolts / `16` verts barrels / `4`-sided struts | `45–60%` tris | Cut sub-part polycount without silhouette loss |

## 3. `Warehouse_Cargo_Crate` and `Warehouse_Steel_Rack` Base Meshes

### 3.1 `Warehouse_Cargo_Crate` (`Assets/Meshes/Props/Warehouse_Cargo_Crate.fbx`)

- **Physical Footprint:** `1.20 m x 1.20 m x 1.20 m` at `1.0x` scale. Placed in Room 4 east
  alcove as `DeadEnd_Crate_E1` (`1.2x` scale at `(25.5, 0, 3.5)`) and `DeadEnd_Crate_E2`
  (`1.0x` scale at `(25.5, 0, 5.0)`).
- **Sub-Mesh Component Breakdown (`864 tris` final):**
  - `CornerStiles` (`144 tris`) + `TopBottomRails` (`176 tris`): outer structural timbers
    extruded `0.045 m` proud of inner plank panels to catch high-bay directional shadows.
  - `PlankPanels_NS` (`120 tris`) + `PlankPanels_EW` (`120 tris`): recessed side boards
    mapped to `Tex_Wood_Crate.png`.
  - `DiagBraces_NS` (`96 tris`) + `DiagBraces_EW` (`96 tris`): 45-deg bracing struts.
  - `TopLidDeck` (`56 tris`) + `PalletSkids` (`56 tris`): top board & bottom `0.10 m` runners.
- **Decimation Result:** Collapsed coplanar loops across inner plank boards and reduced
  corner bevels from 3 segments to 1 chamfer, cutting `1,840 tris` -> `864 tris` (`-53.0%`).

### 3.2 `Warehouse_Steel_Rack` (`Assets/Meshes/Props/Warehouse_Steel_Rack.fbx`)

- **Physical Footprint:** `2.40 m` wide x `1.10 m` deep x `3.10 m` tall (`1.1x` scale in
  Room 4 east alcove `DeadEnd_Blender_Rack_E` at `(25.5, 0, 7.5)`).
- **Sub-Mesh Component Breakdown (`2,680 tris` final):**
  - `Uprights_West` (`380 tris`) + `Uprights_East` (`380 tris`): slotted L-channels (`Mat_Rack_Blue`).
  - `LoadBeams_T1..T3` (`640 tris` total): 3 horizontal stepped beam tiers (`Mat_Rack_Orange`).
  - `DeckPlate_T1..T3` (`540 tris` total): inset galvanized steel deck shelves (`Mat_Shelf_Level`).
  - `CrossBraces` (`480 tris`): 4-sided rectangular K-braces replacing cylindrical tubes.
  - `FootPlates` (`260 tris`): 4 floor anchor pads with bottom cap polygons culled.
- **Decimation Result:** Replacing 16-segment cylindrical cross-braces with 4-sided tubes
  and merging overlapping bracket vertices cut `6,420 tris` -> `2,680 tris` (`-58.3%`).

| Prop Sub-Assembly | Draft Tris | OBJ Interm. | Final FBX Tris | Reduction | Material Slot |
| --- | --- | --- | --- | --- | --- |
| `Cargo_Crate: Frame & Rails` | `710` | `420` | `320` | `-54.9%` | `Mat_Crate_Wood` |
| `Cargo_Crate: Plank Panels` | `540` | `330` | `240` | `-55.6%` | `Mat_Crate_Wood` |
| `Cargo_Crate: Braces & Skids` | `590` | `370` | `304` | `-48.5%` | `Mat_Crate_Dark` |
| `Steel_Rack: Uprights (W/E)` | `1,680` | `1,060` | `760` | `-54.8%` | `Mat_Rack_Blue` |
| `Steel_Rack: Load Beams (3x)` | `1,420` | `890` | `640` | `-54.9%` | `Mat_Rack_Orange` |
| `Steel_Rack: Deck Plates (3x)` | `1,180` | `760` | `540` | `-54.2%` | `Mat_Shelf_Level` |
| `Steel_Rack: Braces & Feet` | `2,140` | `1,270` | `740` | `-65.4%` | `Mat_Rack_Blue` |

## 4. `Industrial_ServicePanel` and `Warehouse_Security_Barrier` Modelling Notes

### 4.1 `Industrial_ServicePanel` (`Assets/Meshes/Props/IndustrialServicePanel.fbx`)

- **Reproducible Blender Source:** Authored via `Documentation/Tools/generate_service_panel.py`
  (Blender `--background --python`), saving `Assets/Meshes/Source/IndustrialServicePanel.blend`
  and exporting `Assets/Meshes/Props/IndustrialServicePanel.fbx` with `-Z` forward, `Y` up.
- **Modeled Sub-Part Geometry Table (`2,340 tris` total):**

| Part Name | Dimensions / Count | Local Coords (m) | Material Slot | Bevel / Topology Spec |
| --- | --- | --- | --- | --- |
| `Enclosure` | `0.90 x 0.19 x 1.25` | `(0.00, 0.00, 0.00)` | `Panel_Steel` | `0.035 m` 3-seg bevel + `WEIGHTED_NORMAL` |
| `Recessed_Door` | `0.79 x 0.035 x 1.10` | `(0.00, -0.09, 0.00)` | `Panel_Recess` | `0.018 m` bevel; inset `0.105 m` forward |
| `Door_Plate` | `0.73 x 0.025 x 1.04` | `(0.00, -0.10, 0.00)` | `Panel_Steel` | `0.020 m` bevel; flush inner panel |
| `Captive_Bolt` | `4x` (`r = 0.022`) | `(±0.30, -0.16, ±0.43)` | `Panel_Amber` | `12`-vertex cylinders (cut from `24`) |
| `Vent_Slat` | `8x` (`0.46 x 0.012 x 0.018`) | `(0.00, -0.15, -0.35 + i*0.045)` | `Panel_Recess` | Angled louvers; `0.003 m` bevel |
| `Caution_Plaque` | `0.31 x 0.012 x 0.17` | `(-0.14, -0.15, 0.34)` | `Panel_Amber` | Upper-left placard; `0.006 m` bevel |
| `Status_Indicator` | `2x` (`0.045 x 0.025 x 0.045`) | `(0.18/0.26, -0.15, 0.34)` | `Panel_Green` | Indicator lenses at upper right |
| `Handle` | `0.07 x 0.045 x 0.18` | `(-0.30, -0.15, 0.02)` | `Panel_Recess` | Vertical latch bar; `0.008 m` bevel |
| `Hinge` | `2x` (`r = 0.018, d = 0.12`) | `(0.43, -0.13, ±0.38)` | `Panel_Recess` | `12`-vertex barrel hinges |
| `Cable_Port` | `2x` (`r = 0.038, d = 0.12`) | `(±0.24, 0.00, 0.67)` | `Panel_Amber` | Top conduit collars (`-Z` mount) |

- **Runtime Collision Isolation:** Placed under `Cinematic_Visual_Details` with zero
  physics colliders and zero NavMesh carving so corridor widths stay exact.

### 4.2 `Warehouse_Security_Barrier` (`Assets/Meshes/Props/Warehouse_Security_Barrier.fbx`)

- **Physical Footprint:** `2.20 m` length x `0.65 m` base depth x `1.12 m` height.
- **Tactical Cover Height Calibration (`DefensiveGuardAI.cs`):**
  - `1.12 m` sits squarely between the AI torso ray (`+0.80 m`) and eye ray (`+1.50 m`).
  - Behind `Defensive_Barrier_West` `(13.5, 0, 5.0)`, `Defensive_Barrier_Center` `(16.0, 0, 5.0)`,
    and `Defensive_Barrier_East` `(20.0, 0, 7.5)`, `torsoClear == false` and `eyesClear == true`,
    yielding maximum `cover = 1.0f` while keeping shotgun sightlines clear.
- **Topology Optimization (`1,080 tris`, `-56.1%`):** Tapered New-Jersey cross section with
  chamfered end caps and dual base forklift slots; culled hidden bottom face.
## 5. UV Mapping & Texel Density Verification

All prop meshes use clean, non-overlapping `UV0` layouts packed into `[0, 1]` with
matched world-space texel density so tiled albedo/normal maps stay sharp alongside
Room 4 concrete walls and floor slabs.

### 5.1 Unwrapping Standards & Lightmap Rules

- **Texel Density Target:** `256 px/m` baseline (`512x512` surface maps with `1x–2x` tiling).
- **UV Island Gutter:** Minimum `0.015` UV unit margin (`~8 px` at `512 px`) to prevent
  mipmap seam bleeding along `35 deg` auto-smooth bevel edges.
- **Seam Placement:** Cut seams strictly along `> 60 deg` hard bevel edges or rear/bottom
  faces occluded from player eye height (`~1.60 m`).
- **Lightmap Channel (`UV1`):** Unity `Generate Lightmap UVs` enabled (`Hard Angle = 88`,
  `Pack Margin = 12`) for CPU Progressive lightmapping (`1024 px` atlas, `8 texels/m`).

### 5.2 Per-Prop UV & Texel Audit Table

| Asset ID | `UV0` Islands | Packing Efficiency | Texel Variance | Orientation & Distortion Check |
| --- | --- | --- | --- | --- |
| `Warehouse_Cargo_Crate` | `14` | `84.2%` | `+/- 3.8%` | Wood plank grain parallel to outer frame rails (`< 3%` stretch) |
| `Warehouse_Steel_Rack` | `26` | `78.5%` | `+/- 5.5%` | Brushed steel streaks aligned along beam span (`< 4%` stretch) |
| `Warehouse_Security_Barrier` | `12` | `86.0%` | `+/- 2.9%` | Concrete pores & warning stripe horizontal (`< 2.5%` stretch) |
| `Industrial_ServicePanel` | `28` | `76.4%` | `+/- 4.7%` | Distinct material slots; zero overlapping `UV0` islands |
| `Access_Card` | `4` | `91.0%` | `< 1.8%` | Planar front/back `UV0`; badge text non-mirrored |

### 5.3 Primary UV Island Coordinate Verification

| Asset / Island | `U Range` | `V Range` | Area Share | Verified Texture Mapping |
| --- | --- | --- | --- | --- |
| `Crate_SidePanels_NS` | `0.02–0.48` | `0.02–0.48` | `22%` | `Tex_Wood_Crate.png` upright plank grain |
| `Crate_SidePanels_EW` | `0.02–0.48` | `0.50–0.96` | `22%` | `Tex_Wood_Crate.png` upright plank grain |
| `Crate_FrameRails_All` | `0.50–0.98` | `0.02–0.48` | `30%` | `Tex_Wood_Crate.png` stile/rail trim |
| `Crate_DiagBraces_Skids` | `0.50–0.96` | `0.50–0.92` | `26%` | `Tex_Wood_Crate.png` dark brace grain |
| `Rack_Uprights_WestEast` | `0.02–0.38` | `0.02–0.96` | `32%` | `Tex_Metal_Plate.png` vertical steel grain |
| `Rack_LoadBeams_3x` | `0.40–0.98` | `0.02–0.34` | `26%` | `Tex_Metal_Plate.png` orange beam span |
| `Rack_DeckPlates_3x` | `0.40–0.98` | `0.36–0.78` | `30%` | `Tex_Metal_Plate.png` galvanized deck |
| `Rack_CrossBraces_Feet` | `0.40–0.92` | `0.80–0.98` | `12%` | `Tex_Metal_Plate.png` anchor brackets |
| `Barrier_SlopedFaces` | `0.02–0.96` | `0.02–0.54` | `58%` | `Tex_Concrete_Floor.png` continuous taper |
| `Barrier_EndCaps_Slots` | `0.02–0.62` | `0.56–0.96` | `28%` | `Tex_Concrete_Floor.png` chamfered ends |
| `Barrier_TopSteelRail` | `0.64–0.96` | `0.56–0.96` | `14%` | `Tex_Metal_Plate.png` top trim strip |
| `Panel_Enclosure_Door` | `0.02–0.64` | `0.02–0.96` | `68%` | `Tex_Wall_Panels.png` brushed steel casing |
| `Panel_Vent_Plaque_Bolts` | `0.66–0.98` | `0.02–0.96` | `32%` | `Mat_Placeholder_Accent` / amber highlights |
| `AccessCard_FrontFace` | `0.02–0.48` | `0.05–0.95` | `45%` | `Tex_Card_A.png` / `Tex_Card_B.png` badge face |
| `AccessCard_BackFace` | `0.52–0.98` | `0.05–0.95` | `45%` | `Tex_Card_A.png` / `Tex_Card_B.png` magnetic stripe |
| `AccessCard_Edge_Chip` | `0.02–0.98` | `0.01–0.04` | `10%` | Satin chamfer perimeter & contact pad |
## 6. Weapon Source Specs: `MP5_SMG` and `Combat_Shotgun`

Both hero firearms are authored in Blender with merged static receiver/barrel sub-meshes
aligned down `+Z`, eliminating upward muzzle skew in both `PlayerGun.cs` (first-person
camera mount) and `GuardShooting.cs` (third-person guard right-hand mount).

### 6.1 `MP5_SMG` (`Assets/Meshes/Weapons/MP5_SMG.fbx`)

- **Gameplay Config (`WeaponConfig.CreateMP5()`):** `"MP5-A3 SMG"`, `damage = 18`,
  `fireRate = 0.11 s` (~545 RPM), `magazineCapacity = 30`, `defaultReserve = 90`,
  `reloadDuration = 1.5 s`, `spread = 1.5 deg`, `maxRange = 55 m`, prefab `Weapons/MP5_SMG`.
- **Mesh Breakdown (`4,920 tris`, `38 UV islands`):**
  - `UpperReceiver` (`920 tris`) + `LowerTriggerHousing` (`600 tris`).
  - `CollapsibleStock` (`620 tris`) + `CurvedMagazine` (`540 tris`).
  - `CockingTube` (`480 tris`) + `HandguardShroud` (`760 tris`).
  - `FrontSightHood` (`420 tris`) + `BarrelSuppressorLug` (`580 tris`).
- **Pivot & Muzzle Calibration:** Grip origin `(0.00, -0.04, 0.08)` frames iron sights
  cleanly in `PlayerGun.cs` without clipping the `0.1 m` camera near-plane; muzzle anchor
  sits at `(0.00, 0.03, 0.42)`.

### 6.2 `Combat_Shotgun` (`Assets/Meshes/Weapons/Combat_Shotgun.fbx`)

- **Gameplay Config (`WeaponConfig.CreateShotgun()` & `DefensiveGuardAI`):**
  - Guard (`DefensiveGuardAI.Awake`): `12 dmg x 6 pellets`, `cooldown = 1.4 s`,
    `maxFiringRange = 10 m`, `bursts = 1`, `spread = 3.8 deg`, `4 bursts/mag`, `1.4 s` reload.
  - Player pickup (`WeaponPickup`): `12 dmg x 6 pellets = 72 max`, `fireRate = 0.80 s`,
    `8` mag / `24` reserve, `pelletSpread = 4.5 deg`, `maxRange = 28 m`, prefab `Weapons/Combat_Shotgun`.
- **Mesh Breakdown (`4,380 tris`, `34 UV islands`):**
  - `SteelReceiver` (`960 tris`) + `BoltCarrier` (`460 tris`) + `PumpForend` (`980 tris`).
  - `MagTube` (`660 tris`) + `PolymerStock` (`580 tris`) + `GhostRingSight` (`260 tris`) + `SmoothBarrel` (`480 tris`).
- **Guard Hand Offset (`GuardShooting.cs`):** `localPosition = new Vector3(0.02f, -0.04f, 0.12f)`,
  `localRotation = Quaternion.identity`; muzzle anchor at `(0.00, 0.04, 0.62)`.

| Weapon Asset | Draft Tris | Final Tris | Sub-Meshes | Grip Pivot | Muzzle Anchor (`+Z`) |
| --- | --- | --- | --- | --- | --- |
| `MP5_SMG` | `9,840` | `4,920` (`-50.0%`) | `8` | `(0.00, -0.04, 0.08)` | `(0.00, 0.03, 0.42)` |
| `Combat_Shotgun` | `8,420` | `4,380` (`-48.0%`) | `7` | `(0.00, -0.04, 0.12)` | `(0.00, 0.04, 0.62)` |
## 7. Unity URP Material Configuration & Normal-Map Bevel Pass

All props and weapons render via `Universal Render Pipeline/Lit` materials stored in
`Assets/Materials/` paired with tangent-space normal maps and packed metallic/AO/smoothness
masks generated in `Assets/Textures/Cinematic/` by `ApplyCinematicWarehouse.cs`.

### 7.1 URP Material & Texture Assignment Matrix

| Asset ID | Material Asset(s) | Base Map (`_BaseMap`) | Normal Map (`_BumpMap`) | Packed Mask / Surface Response |
| --- | --- | --- | --- | --- |
| `Warehouse_Cargo_Crate` | `Mat_Crate_Wood`, `Mat_Crate_Dark` | `Tex_Wood_Crate.png` | `Tex_Wood_Crate_Normal.png` | `Wood_Mask.png` (`metal = 0.00`, `smooth = 0.08–0.24`) |
| `Warehouse_Steel_Rack` | `Mat_Rack_Blue`, `Mat_Rack_Orange`, `Mat_Shelf_Level` | `Tex_Metal_Plate.png` | `Tex_Metal_Plate_Normal.png` | `Steel_Mask.png` (`metal = 0.55`, `smooth = 0.25–0.52`) |
| `Warehouse_Security_Barrier` | `Mat_Wall`, `Mat_Steel_Dark` | `Tex_Concrete_Floor.png` | `Tex_Concrete_Floor_Normal.png` | `Concrete_Mask.png` (`metal = 0.04`, `smooth = 0.18–0.36`) |
| `Industrial_ServicePanel` | `Mat_Steel_Dark`, `Mat_Placeholder_Accent` | `Tex_Wall_Panels.png` | `Tex_Wall_Panels_Normal.png` | Baked `WEIGHTED_NORMAL` bevel specular edges |
| `Access_Card` | `Mat_AccessCard_A`, `Mat_AccessCard_B` | `Tex_Card_A.png`, `Tex_Card_B.png` | `Tex_Card_Normal.png` | Satin PVC card finish (`smooth = 0.65`) |
| `MP5_SMG`, `Combat_Shotgun` | `Mat_Cinematic_Gunmetal`, `Mat_Cinematic_Polymer` | `Tex_Gunmetal.png` | `Tex_Gunmetal_Normal.png` | Gunmetal normal relief + matte polymer grip |

### 7.2 URP Shader Parameter Calibration (`ApplyCinematicWarehouse.cs`)

| Material Asset | Shader | `_Metallic` | `_Smoothness` | `_BumpScale` | Lighting / Specular Role |
| --- | --- | --- | --- | --- | --- |
| `Mat_Crate_Wood` | `URP/Lit` | `0.00` | `0.18` | `1.00` | Matte timber highlight break along rails |
| `Mat_Rack_Blue` / `Orange` | `URP/Lit` | `0.55` | `0.44` | `0.85` | Cool overhead spotlight edge catch |
| `Mat_Wall` (Concrete) | `URP/Lit` | `0.04` | `0.26` | `1.10` | Low-glare cover barrier readability |
| `Mat_Cinematic_Gunmetal` | `URP/Lit` | `0.82` | `0.62` | `0.90` | Specular rim on receiver & barrel |
| `Mat_Cinematic_Polymer` | `URP/Lit` | `0.05` | `0.28` | `0.65` | Non-slip forend & stock contrast |
| `Mat_AccessCard_B` | `URP/Lit` | `0.15` | `0.65` | `0.50` | High-contrast pickup gleam on `Terminal_Desk` |

### 7.3 Normal-Map & Bevel Pass Verification

- **Weapon Surface Upgrade:** `ApplyCinematicWarehouse` / `FinishCinematicPresentation`
  recalculates smooth barrel normals and replaces flat imported weapon materials with
  `Mat_Cinematic_Gunmetal` and `Mat_Cinematic_Polymer` so Room 4 overhead spots (`12f` warm
  / `11f` cool, `spotAngle = 115`) catch crisp edges along the shotgun receiver.
- **Tangent-Space Import Check:** All `*_Normal.png` textures verified as `Texture Type = Normal map`
  (`sRGB = Off`) to prevent gamma-shifted inverted bevel highlights under ACES tonemapping.
- **Global Post-Process Alignment:** `ACES` tonemapping (`postExposure = 0.65`, `contrast = 10`,
  `bloom = 0.28`, `vignette = 0.16`, `filmGrain = 0.025`) verified without motion blur.
## 8. `Access_Card` UV Alignment & Real-Time Rendering Optimization

### 8.1 `Access_Card.fbx` Dual-Route Texture & Trigger Verification

- Both `AccessCard_Pickup_Search` (Route A) and `AccessCard_Pickup_Defensive` (Route B)
  reference `Assets/Meshes/Props/Access_Card.fbx` (`164 tris`, `4 UV islands`) via
  `SetupWeaponsAndAccessCards.cs`.
- **UV0 Alignment Check:** Front and back planar faces map `1:1` onto `Tex_Card_A.png`
  (`Mat_AccessCard_A`) and `Tex_Card_B.png` (`Mat_AccessCard_B`) with `Tex_Card_Normal.png`
  adding subtle chip/magnetic-stripe relief. Both faces read upright during `90 deg/s` spin.
- **Trigger & Bobbing Calibration:** `AccessCard.cs` oscillates `+/- 0.12 m` at `3.0 rad/s`
  around `(16.0, 1.15, 10.0)` with `2.5x` visual scale and an enlarged trigger `BoxCollider`
  (`4.0 x 4.0 x 4.0` in `ExpandRoom4`) so sprinting past the desk reliably registers pickup.

### 8.2 URP Real-Time Rendering & Batching Audit

| Metric / Pass | Configured Budget / Setting | Verification Target | Result |
| --- | --- | --- | --- |
| SRP Batcher | `URP/Lit` shared shaders, no per-prop `MPB` | Room 4 static props | Verified single SRP batch pass |
| Triangle Budget (7 Assets) | `16,428 tris` combined LOD0 (`-52.2%` vs draft) | Props + Pickups + Weapons | Well under mobile/desktop frame cap |
| Baked Lightmap (`UV1`) | `1024 px` atlas, `8 texels/m`, `2` bounces | `Warehouse_Cargo_Crate`, `Barrier`, `Rack` | Zero lightmap UV overlap warnings |
| Reflection Probe | `128 px` box-projected `ReflectionProbe_Room4_Defensive` | Metallic racks & gunmetal | Crisp metallic floor/rack reflections |
| Shadow Cascades | `shadowDistance = 45 m`, `shadowCascadeCount = 2` | Overhead spotlights (`12f`/`11f`) | Cuts shadow pass cost without pop-in |
| Decorative Colliders | Disabled on `IndustrialServicePanel` & trim | `Cinematic_Visual_Details` | Zero spurious NavMesh carving |

## 9. Final Cross-References & Production Summary

| Artifact / Source | Purpose | Final Verified State |
| --- | --- | --- |
| [`ModelOptimizationInventory.csv`](ModelOptimizationInventory.csv) | Sub-part & LOD0 triangle dataset | `16,428 tris` total across `7` hero assets (`-52.2%`) |
| [`DefensivePolicyTrace.md`](DefensivePolicyTrace.md) | 17-ring utility, cover, range & leash traces | `hasThreat` / no-threat scoring & hysteresis verified |
| [`Room4EncounterNotes.md`](Room4EncounterNotes.md) | Room 4 layout, shotgun balance & viva Q&A | `220 HP`, `Room4_Shock`, Card B & PlayMode checks |
| [`generate_service_panel.py`](../Tools/generate_service_panel.py) | Reproducible Blender Python cabinet mesh | `IndustrialServicePanel.fbx` (`2,340 tris`, `-Z` forward) |
