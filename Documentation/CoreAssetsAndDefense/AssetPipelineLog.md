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

<!-- BLOCK:EXPORT_CHECKLIST -->
### 1.2 Initial Blender Export Checklist (Draft v0.1)

- [DRAFT-EXP-01] Verify Blender unit scale (`1.0 BU = 1.0 m`) prior to export.
- [DRAFT-EXP-02] Keep legacy `.obj`/`.mtl` exports beside `.fbx` during early scene blockout.
- [DRAFT-EXP-03] Test Unity `Scale Factor = 100` vs `apply_unit_scale=True` in FBX exporter.
- [DRAFT-EXP-04] Compare `Use Embedded Materials` vs explicit URP `Lit` material assignment.
- [DRAFT-EXP-05] Verify default mesh origin placement before NavMesh rebake.
- [DRAFT-EXP-06] Log initial draw calls and triangle counts in `ModelOptimizationInventory.csv`.
- [DRAFT-EXP-07] Confirm wall decorations remain excluded from runtime physics colliders.
- [DRAFT-EXP-08] Audit initial weapon forward orientation (`+Z` vs `-Z`) in hand socket.
- [DRAFT-EXP-09] Check `Apply Modifiers` vs live `BEVEL` / `WEIGHTED_NORMAL` stack on FBX export.
- [DRAFT-EXP-10] Validate whether `Generate Colliders` creates stepped NavMesh gaps around racks.
- [DRAFT-EXP-11] Check `Import Tangents = Calculate Mikktspace` on beveled steel panels.
- [DRAFT-EXP-12] Verify `.blend` source backups remain excluded from runtime build bundles.
<!-- END:EXPORT_CHECKLIST -->

<!-- BLOCK:DRAFT_TROUBLESHOOT -->
### 1.3 Early Axis & Scale Troubleshooting Scratchpad (Draft)

- `[SCRATCH-01]` Initial `.obj` exports from Blender appeared at `0.01x` scale in Unity until
  FBX `Apply Scalings: FBX Units Scale` was enabled.
- `[SCRATCH-02]` First `Warehouse_Cargo_Crate.obj` import showed rotated `X = -90 deg` transform
  when `Apply Transform` was unchecked in Blender export dialog.
- `[SCRATCH-03]` Early `MP5_SMG` test mesh pointed upward `+Y` inside guard hand before
  aligning receiver barrel along `+Z` with grip pivot at origin.
- `[SCRATCH-04]` Legacy `[Placeholder]_Guard_CharacterMesh` still active in early Room 4
  blockout pending `DefensiveGuardAI.CleanupRoom4PlaceholdersAndCapsule()`.
- `[SCRATCH-05]` Legacy `.mtl` sidecar files generated duplicate grey `Default-Material` assets
  inside `Assets/Meshes/Props/`; replace with shared `Assets/Materials/Mat_*.mat` references.
- `[SCRATCH-06]` Test whether `Generate Colliders` on import causes NavMesh gaps near wall
  conduits and `IndustrialServicePanel.fbx`; keep import colliders OFF for wall detail.
- `[SCRATCH-07]` `Warehouse_Steel_Rack.obj` vertex count inflated from split smooth shading
  islands across `UV0` borders (`44` UV islands before seam consolidation).
- `[SCRATCH-08]` Initial `Access_Card.fbx` at `1.0x` scale appeared too tiny against `Terminal_Desk`;
  schedule `2.5x` pickup scaling in `SetupWeaponsAndAccessCards` and `ExpandWarehouseLayout`.
- `[SCRATCH-09]` Verify `Combat_Shotgun` shell-tube clamp clipping when parented under
  `GuardShooting` right-hand socket transform.
- `[SCRATCH-10]` Confirm `Warehouse_Cargo_Crate` diagonal braces do not z-fight against
  recessed plank panels when viewed from the Room 4 south doorway (`22 m` distance).
- `[SCRATCH-11]` Audit `Warehouse_Security_Barrier` concrete chamfer normal smoothing (`35 deg`
  threshold vs `60 deg` caused dark gradient across lower Jersey barrier taper).

| Scratch Issue | Symptom Observed in Blockout | Source File | Planned Fix |
| --- | --- | --- | --- |
| `SCR-AXIS-01` | Crate resting on corner/center instead of floor | `Warehouse_Cargo_Crate.obj` | Move origin to bottom-center `(0,0,0)` |
| `SCR-AXIS-02` | Rack scaled 100x too small on `.obj` drop | `Warehouse_Steel_Rack.obj` | Switch to calibrated `.fbx` with `apply_unit_scale` |
| `SCR-AXIS-03` | Shotgun barrel skewed +25 deg in hand | `Combat_Shotgun.fbx` | Align `+Z` forward, grip origin `(0,-0.04,0.12)` |
| `SCR-AXIS-04` | Wall panel floating 9.5 cm off wall | `IndustrialServicePanel.blend` | Shift panel pivot to rear face `Z = -0.095 m` |
| `SCR-AXIS-05` | Barrier height ambiguous vs eye ray | `Warehouse_Security_Barrier.obj` | Calibrate height to `1.12 m` (`+0.8m` vs `+1.5m`) |
| `SCR-AXIS-06` | Duplicate `Default-Material` on import | `Props/*.mtl` | Strip `.mtl` workflow and assign `Mat_*.mat` |
| `SCR-AXIS-07` | Card text mirrored on rear badge face | `Access_Card.fbx` | Separate planar front/back `UV0` islands |
| `SCR-AXIS-08` | MP5 sights clipping `0.1 m` camera near-plane | `MP5_SMG.fbx` | Calibrate grip origin `(0,-0.04,0.08)` |
| `SCR-AXIS-09` | Rack tubular cross-braces bloating polycount | `Warehouse_Steel_Rack.blend` | Convert 16-seg tubes to 4-sided struts |
| `SCR-AXIS-10` | Service panel bolts consuming 768 tris | `generate_service_panel.py` | Cap cylinders at 12 verts (`384 tris`) |
<!-- END:DRAFT_TROUBLESHOOT -->

<!-- SECTION:TOPOLOGY_GUIDELINES -->
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

<!-- BLOCK:TOPO_DRAFT_NOTES -->
### 2.4 Legacy Topology Issues to Resolve (Draft)

- `[DRAFT-TOPO-01]` Early `.obj` exports split vertices along UV seams, doubling runtime vertex count.
- `[DRAFT-TOPO-02]` Cylindrical rack cross-braces used 16 segments per strut; convert to 4-sided tubes.
- `[DRAFT-TOPO-03]` Service panel bolts initially used 24-vertex cylinders; reduce to 12-vertex cylinders.
- `[DRAFT-TOPO-04]` Check crate inner plank insets for coplanar z-fighting at mip levels > 2.
- `[DRAFT-TOPO-05]` Remove legacy `Subsurf` modifiers left disabled in early `.blend` backups.
- `[DRAFT-TOPO-06]` Audit `Security_Barrier` base cutouts for inverted normals on inner lip.
- `[DRAFT-TOPO-07]` Collapse stacked longitudinal loops on `Warehouse_Cargo_Crate` corner stiles.
- `[DRAFT-TOPO-08]` Remove unseen back face of `IndustrialServicePanel` recessed interior cavity.
- `[DRAFT-TOPO-09]` Verify `MP5_SMG` perforated handguard holes use normal/bump relief instead of boolean cutouts.
- `[DRAFT-TOPO-10]` Simplify `Combat_Shotgun` forend ribs from 12 bevelled grooves to 6 clean loops.
- `[DRAFT-TOPO-11]` Audit `Access_Card` perimeter chamfer so card thickness reads at `2.5x` pickup scale.
- `[DRAFT-TOPO-12]` Replace high-poly bolted gusset brackets on `Warehouse_Steel_Rack` with stamped quads.
- `[DRAFT-TOPO-13]` Verify `MP5_SMG` curved magazine follower recess uses baked AO rather than 3D lip.
- `[DRAFT-TOPO-14]` Audit `Warehouse_Security_Barrier` top steel rail end-caps for zero-area degenerate tris.
- `[DRAFT-TOPO-15]` Standardize sharp-edge crease weights before running Unity `Import Normals = Import`.
<!-- END:TOPO_DRAFT_NOTES -->
<!-- END:TOPOLOGY_GUIDELINES -->
<!-- SECTION:CARGO_RACK -->
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

<!-- BLOCK:DUAL_FORMAT_NOTE -->
### 3.3 Temporary Dual-Format Export Log (To Prune)

- `[TEMP-FMT-01]` Retained `.obj`/`.mtl` exports (`Warehouse_Cargo_Crate.obj`,
  `Warehouse_Steel_Rack.obj`, `Warehouse_Security_Barrier.obj`) alongside `.fbx` while
  testing multi-material slot assignment order in Unity.
- `[TEMP-FMT-02]` `ExpandWarehouseLayout.cs` explicitly loads `.fbx` via `AssetDatabase.LoadAssetAtPath`.
- `[TEMP-FMT-03]` Remove `.obj` rows from inventory once `.fbx` pivot/scale pass completes.
- `[TEMP-FMT-04]` Legacy `.mtl` references in `Assets/Meshes/Props/` will remain untouched in
  Unity meta tracking; documentation inventory will drop deprecated `.obj` tracking rows.
- `[TEMP-FMT-05]` Verified `.obj` exports suffered vertex-normal splitting on UV seams (`+28%` verts)
  compared to binary `.fbx` with `Smoothing: Face` and `WEIGHTED_NORMAL` applied.
- `[TEMP-FMT-06]` Compared `.obj` ASCII parse time vs binary `.fbx` reimport across clean CI.
<!-- END:DUAL_FORMAT_NOTE -->
<!-- END:CARGO_RACK -->
<!-- SECTION:PANEL_BARRIER -->
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
<!-- END:PANEL_BARRIER -->
<!-- SECTION:UV_CHECKS -->
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
<!-- END:UV_CHECKS -->
<!-- SECTION:WEAPONS -->
<!-- SECTION:URP_MATERIALS -->
<!-- SECTION:ACCESS_CARD_VERIFY -->
