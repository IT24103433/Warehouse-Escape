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
<!-- SECTION:PANEL_BARRIER -->
<!-- SECTION:UV_CHECKS -->
<!-- SECTION:WEAPONS -->
<!-- SECTION:URP_MATERIALS -->
<!-- SECTION:ACCESS_CARD_VERIFY -->
