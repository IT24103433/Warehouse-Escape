# Warehouse lighting and optimization log



Scene settings, editor tooling and earlier test coverage are identified by source. The verification checklists record expected observations and specify the evidence needed to close them.

## Industrial palette and prop readability

Use concrete for broad floor and wall surfaces, painted or dark steel for
shelves and barriers, warmer wood tones for crate surfaces, and saturated
markings selectively at turns and routes. The layout pass loads shared
materials by role, including `Mat_Wall` and `Mat_Shelf_Level`. Check the actual
material and shader on the active renderer before grouping props.

| Prop family | Visual objective | Navigation relevance |
| --- | --- | --- |
| Floor and walls | Readable silhouettes and modest surface contrast | Give stable cues at bends and room boundaries |
| Shelves and barriers | Distinguish uprights from the open aisle | Keep gaps visibly aligned with collider openings |
| Crates and barrels | Break up sightlines, identify cover | Preserve the walkable margins around placed objects |
| Route stripes and signs | Distinguish the branch turn from the cover bay | Keep navigational markers readable without a solid trigger obstacle |
| Metallic trim | Add edge detail without excess object submissions | Keep the gameplay collider on the parent structure |

Compare accent readability across the two sides of Room 2 with the same
camera exposure. Inspect material variants through renderer state and the
frame debugger before attributing a draw-call change to the palette.

## Probe placement review

`ApplyIGIVisualAndLightingPass` configures warehouse light probes and named
per-room baked reflection probes. Its Room 2 reflection probe is centered at
(0, 2.5, −9) with volume size (20, 5.5, 20) in that editor pass.
The scene contains a baked Room 2 reflection texture. Confirm the active
probe's assigned texture and current bounds in the scene before rebaking.

Lightmaps store static illumination. Moving agents need interpolated diffuse
lighting through the scene's light-probe setup. Reflection probes supply a
different signal for materials such as metal. Test the guard crossing a
probe region alongside a static reflective prop so the two signals can be
diagnosed independently. The light and exposure settings change appearance,
while the patrol visibility feature casts collider queries.

## NavMesh clearance

The warehouse scene's build settings specify 0.5 m agent radius, 2 m height,
45° slope and 0.4 m climb. The scene serialization also records a 0.16666667 m
cell size and tile size 256, with manual overrides disabled. Compare these
with the active agent type. The baked radius erodes walkable routes, and
the runtime movement component can have its own avoidance parameters.

For a straight throat with ideal vertical boundaries, first estimate
`centerline clearance = measured internal width − 2 × bake radius`.
Allow margin for collider overshoot, voxelization and turning. Use the
serialized collider faces at actual placement for any measured value.

| Face-to-face test width | Ideal remaining center interval | Check to collect |
| ---: | ---: | --- |
| 3.0 m | 2.0 m | Nominal lintel-sized approach, both turning directions |
| 1.5 m | 0.5 m | Offset-start path test at each shelf tip |
| 1.0 m | 0 m | Edge condition, expect a fragile or absent route |
| 0.9 m | −0.1 m | Negative nominal clearance, reject from planning graph |

For doorway headroom compare the underside of the actual collision lintel
against the walkable surface. Keep the height and radius checks separate.
Actual gap measurements and scene-corner paths remain PlayMode checks.

## Active bake baseline

Follow the scene's `m_LightingSettings` reference to its active
`LightingSettings` object. `Warehouse.unity` also has embedded legacy
`m_LightmapEditorSettings` fields with differing values.

| Active field | Checked-in setting | Evidence |
| --- | --- | --- |
| Baked static lightmaps | Enabled | Referenced scene `LightingSettings` |
| Realtime lightmaps | Disabled | Referenced scene `LightingSettings` |
| Bake resolution | 8 | Referenced `m_BakeResolution` |
| Maximum lightmap size | 1024 | Referenced `m_LightmapMaxSize` |
| Packing padding | 2 | Referenced `m_Padding` |
| Mixed mode field | 2 | Referenced `m_MixedBakeMode` |
| Lighting data | Linked serialized asset | `Assets/Scenes/Warehouse/LightingData.asset` |
| Per-room baked reflection size | 256 in detail pass | `DetailWarehouseSurfaces` and the surface-detail notes |

The editor lighting pass has bake inputs of its own. Record the resulting
scene settings to reproduce a captured build. Use a fixed camera and collect
the bake duration, lightmap count, asset footprint and close-up seams when
evaluating a future change. Capture soft shadow continuity at shelf ends
and make sure the two signs remain visually distinct with baked lighting.

## Shared meshes, render submissions and colliders

`DetailWarehouseSurfaces` applies beveled meshes shared by size and creates
combined trim by room. The trim uses a shared steel material, has shadow
casting turned off and uses blended light probes. Its components are visual.
The existing surface-detail report records that the original gameplay
colliders and transforms were preserved. Reuse of a mesh helps memory and
may enable compatible batching, while actual draw submission behavior also
depends on materials, passes, render state and Unity's batching path.

| Review | Inspect | Evidence to retain |
| --- | --- | --- |
| Mesh reuse | Compare `sharedMesh` references for matching sizes | Renderer and shared mesh IDs |
| Shared material | Compare references and any per-instance overrides | Shared material IDs and shader variants |
| Combined trim | Check combined renderer count by room | Renderer count, shadow mode and bounds |
| Frame submission | Use matching camera, scene state and resolution | Draw calls, SetPass calls and batch breakdown |
| Collider isolation | Inspect gameplay collider shapes and trim components | Before/after collider counts and shape bounds |
| Occlusion consistency | Compare rays at shelf and crate silhouettes | Ray endpoint, mask and hit before/after |
| Navigation consistency | Use the same agent and sampled endpoints | Complete path, corners and arrival |
| Performance | Measure before/after under the same build settings | CPU/GPU frame-time medians and high-percentile frames |

Existing tests include
`DisplayAndSurfaceTests.Prop_Detail_Is_Batched_And_Texture_Quality_Is_Full`.
Read [SurfaceDetailAndDisplay](../SurfaceDetailAndDisplay.md) for the
team's results. New frame captures were outside this documentation run.

## Multi-route comparison worksheet

Use the warehouse-scope fixture pairs to choose which route obstruction
to simulate. The route graph models edge availability; PlayMode should
use actual scene colliders and path corners.

| Case | Origin / target | Runtime check |
| --- | --- | --- |
| West branch baseline | A_ENTRY / R5_WEST | Complete travel through search-side approach |
| East branch baseline | B_ENTRY / R5_EAST | Complete travel through defensive-side approach |
| West merge closed | A_MERGE / R5_WEST | Alternate compatible connection, or record disconnection |
| East merge closed | B_MERGE / R5_EAST | Alternate compatible connection, or record disconnection |
| Central reader leg closed | R5_CENTER / R5_READER | Inspect path corners for west/east side detour |
| Both crossings closed | R5_WEST / R5_EAST | Confirm the remaining graph links permit a route and compare with runtime |

`PhysicalWalkthroughTests` includes coverage for both card routes and
final-room flanking detours. Use the test's exact placement and
preconditions before comparing behavior with these independent fixture
anchors. Room 5 guard movement can choose detours across a larger zone
than the Room 2 patrol endpoint predicate allows.

## Verification evidence to capture

| Area | Current evidence | Still to collect on replay |
| --- | --- | --- |
| Lighting baseline | Scene and editor bake settings | Matched views through the aisles and both branches |
| Prop batching | Mesh and renderer construction plus upstream test references | Frame debugger and profiler comparison |
| Clearance | Build settings and ideal width arithmetic | Effective collider-face gap and paths from multiple headings |
| Route detour | Synthetic graph and upstream route test definitions | Agent corners in the loaded scene and recovery timing |
| Chase recovery | Script and [patrol checks](PatrolUtilityTrace.md#playmode-verification-record) | A fresh state and timestamp trace |

Review assets and test evidence against the branch base revision. A new
profiling record should include Unity version, rendering settings, camera,
hardware and sample length. This sequence did not perform a bake or run
Unity profiling.
