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
