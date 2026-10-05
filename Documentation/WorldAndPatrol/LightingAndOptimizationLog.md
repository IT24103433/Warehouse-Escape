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

## Draft lighting studies

These alternatives specify visual comparisons. Values are trial inputs for
a disposable bake, and the scene remains the source of the active settings.

| Variant | Change under study | Fixed controls | Inspect first |
| --- | --- | --- | --- |
| Aisle accents | Add overhead baked area fill over center crossing | Exposure, geometry, fog, camera | Shelf edges and route signs |
| Bay accents | Move emphasis to peripheral storage | Same material assignments and camera | Cover versus barrel/corner readability |
| Door fill | Raise light only at the north branch lips | Same built navigation | Sign legibility on west and east approaches |
| Low atlas draft | 4 texels/m planning trial | Atlas maximum and materials | Visible cross-aisle seams |
| Higher atlas draft | 12 texels/m planning trial | Atlas maximum and materials | Bake time and memory change |
| Dynamic probe draft | Add a short fill sample along turn | Same baked maps | Light continuity on the moving guard |
| Reflection draft | Compare two local probes versus one broad volume | Static lighting and metal material | Reflective transitions along the main aisle |
| Fog comparison | Compare far signage at fixed target distances | Same exposure and probes | Route marker loss at hall merges |
| Marking comparison | Simplify guide color accents | Same light intensities | Routes readable before crossing a doorway |
| Geometry comparison | Compare parent prop with and without decorative trim | Same materials, camera and profiler settings | Whether added hardware earns its draw cost |

Draft evidence per take:

1. Store the same camera and rendering resolution for each test.
2. Capture the south crossing toward both signs.
3. Capture the west and east shelf ends with the guard at comparable poses.
4. Record the active light settings asset and texture budget.
5. Check the farthest visible portal before changing the next variable.
6. Retain only observations that can be reproduced under the matching settings.

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
