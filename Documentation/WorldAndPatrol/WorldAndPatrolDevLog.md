# World Builder and Room 2 patrol dev log



This is a retrospective documentation sequence organized by the requested milestone dates. Git dates label the sequence. Source and scene references use the branch base, and workbook-style waypoint examples use synthetic data. PlayMode cases list expected observations or identify earlier repository evidence.

## 2026-09-28 · Initialize World Builder layout plan and Room 2 patrol scope

Set the documentation boundary at Room 2 patrol and the warehouse navigation connections. Use [layout](WarehouseLayoutSpec.md) for design references and record the distinction between survey fixtures and scene measurements from the start.

## 2026-09-29 · Define warehouse zone divisions for storage, shelves, and main hall

Mapped the entrance, shelf rows, cross aisle, branch doors and far-side loading hall to their player and AI roles. The two player branches share the Room 2 junction and meet again at the final hall.

## 2026-09-30 · Draft internal wall placements and sightline blockers

Catalogued fixed wall faces, lintels and the northeast dead end. Wrote down the open-collider and clear-ray questions for the speculative shortcut variants.

## 2026-10-01 · Document shelf aisle spacing and primary navigation corridors

Identified face-to-face clearance as the measurement to record. Shelf mesh bounds and painted walkway lines can misrepresent clearance if the colliders differ.

## 2026-10-02 · Add crate and barrel obstacle placement layout for cover

Recorded cover positions from the layout script where available. Marked additional crate and barrel spacings as study variants and separated cast shadows from collider occlusion.

## 2026-10-03 · Design alternate flanking routes and checkpoint detours

Sketched branches as a connectivity graph with the patrol's inspection endpoints inside Room 2. The longer forking paths belong to the warehouse route fixtures.

## 2026-10-04 · Document low-poly material assignments and industrial palette

Grouped wall, shelf, barrier, crate, floor and marking materials by orientation and readability role. Captured which palette choices should be compared under the same exposure.

## 2026-10-05 · Draft URP baked lighting and reflection probe placement plan

Added lighting-source references, a probe checklist and draft comparison cases. The proposed comparison takes use matching camera positions and render scale.

## 2026-10-06 · Generate initial warehouse NavMesh waypoint dataset

Added modeled start/target pairs, separate reachability flags and a neutral visibility baseline. Early alternatives intentionally include disconnected nodes and endpoints outside the patrol sector.

## 2026-10-07 · Verify NavMesh agent radius and doorway clearance margins

Read the scene's bake values and compared the nominal doorway widths with agent diameter on paper. The actual approach turns and collider-face clearances are PlayMode checks.

## 2026-10-08 · Refactor warehouse layout spec to remove blocked corridor drafts

Dropped speculative shortcuts that depended on gaps through a partition, cover bay or protected room. The retained spec gives each approach a required open return path.

## 2026-10-09 · Outline PatrolGuardAI state machine for Room 2 patrol sector

Traced Patrol, Pursue and Investigate in source order. Captured which observations change state and where the AI writes arrival and memory state.

## 2026-10-10 · Define Room 2 patrol inspection points and loop transitions

Recorded the eight fallback transforms and the conditions under which they replace a short configured array. Added modeled origins and perturbations for variation in inspection order.

## 2026-10-11 · Document PatrolDecisionPolicy neglect-age weighting term

Worked through the neglect term with equal visibility so the effects of staleness and travel can be compared directly. Staleness is initialized to 30 seconds and updated on arrival.

## 2026-10-12 · Prune unreachable and out-of-room candidates from waypoint dataset

Removed raw boundary and disconnected proposals from the patrol-candidate matrix. Kept the distinct rejection mechanisms as small trace cases so they can be exercised deliberately.

## 2026-10-13 · Add local visibility scoring and path-length penalty trace

Replaced the neutral visibility baseline with eight-direction modeled occlusion, added more detours, and expanded the arithmetic examples with capped and competing terms.

## 2026-10-14 · Document complete-path validation for patrol candidate filtering

Recorded the sampling filter, corner-based length, Room 2 endpoint gate and final destination acceptance. Added checks for the difference between being on a NavMesh island and reaching it.

## 2026-10-15 · Add player detection FOV and patrol-to-chase transition notes

Mapped the sight test to the cached head direction, range, horizontal aperture and cover rays. A controlled example varies facing while candidate utility inputs stay fixed.

## 2026-10-16 · Refactor PatrolUtilityTrace to replace fixed-loop draft logs

Replaced the hand-stepped order worksheet with utility comparisons, selection events and arrival timestamps. The array order remains relevant as a tie-break and affects inspection pause lengths.

## 2026-10-17 · Document UMP attack range and 120 HP balance for Room 2 guard

Recorded the missing-component defaults and where to verify serialized overrides. Sight, firing eligibility, bullet obstruction and health are separate gates.

## 2026-10-18 · Log chase termination and return-to-patrol recovery behavior

Traced the change from visible-player tracking to the last-seen investigation. Distinguished the travel timeout from the stationary scan timer.

## 2026-10-19 · Document Room 2 shock pad trap placement and alarm synergy

Recorded the Room 2 shock pad position, warning, trigger and sound origin. Added expected responses for a visible player, an audible alarm without sight and a bypass.

## 2026-10-20 · Clean up legacy lighting drafts and consolidate URP bake settings

Retired competing lighting mockups and followed the scene's active LightingSettings reference. Separated baked lightmaps, light probes, reflection probes and visual readability checks.

## 2026-10-21 · Verify shared mesh batching and prop collider optimization

Checked shared bevel meshes and the combined trim construction in source. Specified measurements for render submissions and collision shape equivalence in PlayMode.

## 2026-10-22 · Test multi-path warehouse navigation for flanking guard support

Added synthetic fixtures for open routes and blocked links through the final hall. Their lengths come from the documented graph, with scene-level walkthrough coverage referenced separately.

## 2026-10-23 · Consolidate Room 2 NavMesh dataset and remove redundant entries

Folded reciprocal warehouse pairs into one row with two endpoint visibility values. Dropped current-point-only patrol samples, which are already distinguished by the trace cases.

## 2026-10-24 · Log PlayMode verification for Room 2 patrol and chase recovery

Indexed the existing patrol and route tests and wrote a manual recovery matrix with pending run status. The fixture arithmetic and the runtime checks have their own evidence fields.
