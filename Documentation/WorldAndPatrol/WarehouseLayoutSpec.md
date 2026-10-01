# Warehouse layout and Room 2 patrol space



Directions use world X/Z, with north toward positive Z. Metres follow the scene's scale convention. Editor layout code supplies placement intent. The serialized scene is the authority for the loaded geometry.

## Scope and coordinate references

| Reference | X span (m) | Z span (m) | Use |
| --- | --- | --- | --- |
| Room 2 floor design | −11 to 11 | −20 to −2 | Expanded floor in the editor layout pass |
| Room 2 guard predicate | −11 to 11 | −18 to −2 | Inclusive X/Z bounds in `PatrolGuardAI.IsInsideRoom` |
| South safe-side approach | centered at 0 | below −18 | Outside the Room 2 guard predicate |
| Route A door | centered near −5 | near −2 | Player branch to the search sector |
| Route B door | centered near 5 | near −2 | Player branch to the defensive sector |

The predicate checks the sampled patrol endpoint. The serialized `RoomBounds`
property exists, while `IsInsideRoom` uses literal X/Z bounds and ignores Y.
For a strict corridor-containment check, inspect every route segment as well as
the endpoint. Read the [decision trace](PatrolUtilityTrace.md) for the gate order.

## Zone responsibilities

| Zone | Approach and purpose | Navigation and sight check |
| --- | --- | --- |
| Room 1 threshold | South entrance from the safe room | Compare positions just north and south of Z = −18 |
| Room 2 southern crossing | Move between east and west inspection lanes | Leave both lane entries usable around the first cover objects |
| Room 2 west storage | Shelves, cover bay and Route A approach | Confirm a return path from the shelf end before keeping a waypoint |
| Room 2 east storage | Shelves, cover bay and Route B approach | Keep the northeast dead end distinct from the signed exit |
| Room 2 main aisle | Central angle into both branch approaches | Preserve lateral crossing space and visible route markers |
| Room 3 / Route A | Search sector beyond the west branch | Use room-specific guard behavior inside the sector |
| Room 4 / Route B | Defensive sector beyond the east branch | Preserve the branch independently of Route A |
| Room 5 loading hall | Two merges, side aisles, central progress to card reader | Exercise both flank spines and a fallback if a cross aisle is blocked |

Either route can support the card-reader flow. The scene's card and reader
logic determines progression. The Room 2 guard selects goals from its own
waypoint array, and the larger connectivity graph supports design checks.

## Walls and sightline control

| Feature | Editor layout reference | Review |
| --- | --- | --- |
| East/west perimeter | Walls centered on X ±11, nominal 0.5 m thick | Use the interior collision faces for clear width |
| South wall split | Two walls near Z −20 with center lintel | Check the smaller guard boundary at Z −18 independently |
| North wall split | Center and outer wing segments near Z −2 | Keep the marked exit gaps and their approach turns clear |
| Northeast dead-end partition | `DeadEnd_Partition_NE` at (8.2, 1.8, −4.5) | Keep the occluding back wall and leave the approach reversible |
| Floor guides | `FloorGuide_RouteA` and `FloorGuide_RouteB` | Editor script disables their colliders |

Use the guard eye and the player's head/chest target heights when examining
cover. A ray at ankle height can report a different obstruction. A partition
can block perception and still leave a complete route around its end.

## Shelf spacing and doorway clearance

Measure the shortest horizontal face-to-face distance at each pinch point.
Record the obstacle transforms and the effective collider extents, especially
for rotated barricades, shelf endcaps and crate corners. Preserve room to
turn after exiting an aisle so outer corners do not intersect the new heading.

| Station | Record | Acceptance evidence |
| --- | --- | --- |
| South crossing | Clear face gap and two approach headings | Side-to-side complete path with space at both turns |
| East shelf end | Tip of shelf to nearest adjacent collider | Approach and return both complete around the cover stack |
| West shelf end | Clearance toward the shock-pad bypass | Path can turn without relying on the trap trigger |
| North split | Jamb, guide and signed doorway alignments | Both branch entrances remain independently usable |
| Loading hall merge | Central connector and outer aisle corners | Alternate route stays usable when one connector is blocked |

Use the [clearance worksheet](LightingAndOptimizationLog.md#navmesh-clearance)
for the nominal radius calculation. Inspect curved approaches with test
origins offset left, centered and offset right. A centered straight line only
tests one approach.

## Draft shortcuts to challenge

| Draft | Entry | Proposed exit | Walkable assumption | Sightline question | Failure expected |
| --- | --- | --- | --- | --- | --- |
| NE behind partition | East bay | Route B approach | Partition leaves enough room at east end | Is the far side visible from the main aisle? | Collider face and wall squeeze the turn |
| NE through center | East bay | Central junction | Remove middle strip of partition | Can the player see the branch from within the blind bay? | Defeats deliberate dead end |
| Shelf throat splice | East center | West storage | Access through shelf upright gap | Does standing eye clear the rack frame? | Centerline width omits the upright collider |
| West crate bypass | South crossing | West wall | Hug wall behind western crates | Can a low crate occlude torso without hiding the sign? | No reliable shoulder space on approach |
| South threshold cut | Safe-side corridor | Room 2 west | Use southern floor as an AI return loop | Can patrol see through center lintel? | Guard predicate rejects the endpoint |
| Early north connector | Main aisle | Other sector | Treat both branch doorways as an AI shortcut | Does the sign remain distinct? | Crosses the patrol boundary |

Draft checks to collect before accepting any shortcut:

1. Record a left-jamb start and a right-jamb start on the approach side.
2. Measure the exit turn from both approaches.
3. Inspect the baked polygon continuity around the collider corners.
4. Compare stationary perception at the lip and at the proposed midpoint.
5. Make one blockout change per trial to attribute the difference.
6. Preserve at least one complete reverse route for every kept point.

## Draft shelf throat variants

| Placement | Nominal centerline | Likely collision concern | Decision |
| --- | --- | --- | --- |
| Move west rack outboard | Hug west wall | Rack and wall may overlap the return funnel | Keep a reversal test before moving cover |
| Move east rack inboard | Head toward center crossing | Crate outcrop may intersect diagonal | Inspect at guard radius |
| Rotate shelf 45° | Cut behind corner | Projected width increases at the throat | Inspect collision corners |
| Put a barrel at jamb | Follow door axis | Curved silhouette still consumes turning radius | Place marker off-path instead |
| Offset north cover inward | Approach across both branches | Competes with signs and branch route readability | Test separate branch approaches |
| Keep an empty post near rack | End near the outboard shelf face | Sampling may snap to the far side | Label the sampled side if measured |
