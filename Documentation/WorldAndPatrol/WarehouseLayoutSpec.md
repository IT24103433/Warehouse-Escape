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

## Cover and prop placement

Editor layout references from `ExpandRoom2` use X/Z positions in metres.

| Prop family | Placement reference | Design constraint |
| --- | --- | --- |
| West barricade | (−7.5, −11) | Preserve a path past each open end |
| East barricade | (7.5, −11) | Check the turn from storage to the northern approach |
| Southwestern and northwestern crate stacks | (−8.5, −15.5) and (−8.5, −6.5) | Check cover gaps at both crouched and standing eye heights |
| Southeastern and northeastern crate stacks | (8.5, −15.5) and (8.5, −6.5) | Keep the signed door face visible from the lane |
| Barrels | Verify scene instances in peripheral bays | Keep their collider silhouettes and interaction space off branch throats |
| Hardware details | Bands, fasteners, rings | Visual detail should preserve the parent prop's collision footprint |

Occlusion depends on collision, masks and the ray targets. A dark corner
can affect player readability while leaving the current AI sight test clear.
A trigger volume can overlap a route that is mechanically walkable.

## Branch and flank topology

```mermaid
flowchart TD
    H["Room 2 crossing"] --> A["Route A entrance"]
    H --> B["Route B entrance"]
    A --> S["Room 3 search"]
    B --> D["Room 4 defense"]
    S --> W["Room 5 west merge"]
    D --> E["Room 5 east merge"]
    W --> C["Room 5 loading hall"]
    E --> C
```

- Room 2: inspect the west and east lintels from positions still inside its predicate.
- Route A: follow the western branch, sector and connector into the west merge.
- Route B: follow the eastern branch, sector and connector into the east merge.
- Room 5: keep the center spine and two side aisles usable for alternate approaches.
- The northeast dead-end bay offers cover and a return path through its entrance.

## Retained route constraints

Keep the signed split, both storage aisle approaches and the center connectors.
The northeast partition remains an occluder and dead-end boundary. Drop shortcuts
that assume an opening through it or that rely on the safe-side floor for
the guard's return circuit. Record rejected fixture intent in the trace and
retain only supported connections in the layout summary.

A single closed crossing is an alternate-route test. A closed approach and
return pair is a disconnected destination test. Compare these cases with the
warehouse CSV rather than substituting straight-line distance for a route.

## Fallback inspection points

`EnsureNaturalRoute` creates or reuses these transforms if the assigned array
is missing or has four entries or fewer. Fallback Y is 1.08 m in the source,
and the navigation query can project the target onto a walkable surface.
The checked-in `Guard_Varun_Patrol` has four serialized waypoint references,
so the fallback is the expected startup route for this scene.

| CSV ID | Array index | X (m) | Z (m) | Inspection intent | Default pause |
| --- | ---: | ---: | ---: | --- | --- |
| R2_S | 0 | −3.0 | −15.5 | South crossing | 1 s |
| R2_ES | 1 | 4.5 | −14.0 | East south storage | 2 s |
| R2_E | 2 | 4.5 | −9.5 | East corridor | 1 s |
| R2_NE | 3 | 4.0 | −5.5 | Northeast look toward Route B | 2 s |
| R2_N | 4 | 0.0 | −6.5 | North crossing | 1 s |
| R2_NW | 5 | −4.5 | −5.5 | Northwest look toward Route A | 2 s |
| R2_W | 6 | −5.0 | −10.5 | West storage aisle | 1 s |
| R2_SW | 7 | −4.5 | −14.5 | Southwestern corner | 2 s |

The odd-index pause is hard-coded. The even-index pause uses the configured
`waypointWaitTime` with a 1 s code default. Changing the array order can change
which points receive a longer look and which tie candidate is visited first.

## Synthetic route dataset

The [CSV](NavMeshWaypointsDataset.csv) records deterministic planning fixtures.
Room 2 origins use the fallback X/Z anchors above. Warehouse anchors and
intermediate turns are illustrative positions based on the larger topology.
Lengths use shortest paths in an undirected weighted planar graph. Each edge
uses its X/Z length, and a closure removes the specified link from that fixture.
The modeled cost ignores elevation, Unity area weights, polygon erosion and
dynamic steering. Complete means connected in that fixture.

Visibility counts eight horizontal 6 m rays, spaced at 45° from +Z. Rectangles
in plan view stand in for geometry that reaches ray height. Denominators are
always eight. Room 2 fixtures include a near crossing and a north and south
connection so a closure can change length without automatically making a target
unreachable. A profile with multiple removed aisle links can isolate a node.

| Fixture field | Meaning |
| --- | --- |
| `fixture_id`, `provenance` | Unique fixture key and explicit synthetic marker |
| `scope`, `scenario` | Decision region and the named closure/cover perturbation |
| `origin`, `target` | Endpoints, with their X/Z positions in metres |
| `modeled_path_length_m` | Sum of graph edge distances, to four decimal places |
| `origin_visibility`, `target_visibility` | Endpoint openness, in eighths |
| `path_complete` | Whether the modeled graph connects the pair |
| `target_inside_room2` | Whether the X/Z predicate contains the target |
| `eligible` | Candidate in Room 2 or usable route pair for warehouse scope, as specified by `reason` |
| `direction` | `forward` for Room 2 decision context and `both` for consolidated warehouse pairs |
| `reason` | Candidate, complete route fixture or disconnection |

A blank length means disconnected. Blank target visibility means an unscored
target. Room 2 connectivity changes may be directional as a decision context
because which point is current changes, even though modeled lengths are
symmetric. Warehouse rows store both endpoint visibility values. To reverse a
warehouse pair, swap endpoint IDs, coordinates and visibility, then recompute
Room 2 membership from the new target. The path length stays the same.

Example links use R2_NW → A_ENTRY → A_BRANCH and R2_NE → B_ENTRY → B_BRANCH.
Room 5 branches merge near X ±6.5, Z 20 and can reach the reader through the
center spine or side approaches. For a runtime comparison, capture raw and
sampled target, agent type, area mask, path status and 3D corners in the same
record, and identify the scene revision.

### Graph reference

Each consecutive pair on a chain is an undirected edge. Edge length is
the X/Z distance between anchors. The Room 2 graph uses the eight-point
ring in array order plus the W–E, W–N and E–N cross-links. Warehouse
fixtures add the following chains to the unmodified Room 2 graph.

| Chain | Anchors |
| --- | --- |
| Route A | R2_NW → A_ENTRY → A_BRANCH → A_SEARCH → A_ELBOW → A_MERGE → R5_WEST |
| Route B | R2_NE → B_ENTRY → B_BRANCH → B_DEFENSE → B_ELBOW → B_MERGE → R5_EAST |
| South connector | A_MERGE → R5_SOUTH → B_MERGE |
| Center spine | R5_SOUTH → R5_CENTER → R5_READER |
| Cross aisle | R5_WEST → R5_CENTER → R5_EAST |
| West approach | R5_WEST → A_NORTH → R5_READER |
| East approach | R5_EAST → B_NORTH → R5_READER |

Coordinates for intermediate turns present only in the graph:

| Turn | X (m) | Z (m) |
| --- | ---: | ---: |
| A_ELBOW | -17 | 17 |
| B_ELBOW | 17 | 17 |
| A_NORTH | -6.5 | 39 |
| B_NORTH | 6.5 | 39 |
| R5_SOUTH | 0 | 23 |

#### Room 2 closure profiles

| Scenario | Removed links | Added cover rectangles |
| --- | --- | --- |
| `clear_graph` | None | None |
| `west_aisle_closed` | R2_W–R2_SW | None |
| `east_aisle_closed` | R2_E–R2_NE | None |
| `center_crossing_closed` | R2_W–R2_E | rack_mid |
| `north_connector_closed` | R2_NE–R2_N, R2_N–R2_NW | north_screen |
| `south_connector_closed` | R2_S–R2_ES, R2_SW–R2_S | south_stack |
| `west_shelf_end_closed` | R2_NW–R2_W, R2_W–R2_SW | west_stack |
| `east_shelf_end_closed` | R2_ES–R2_E, R2_E–R2_NE | east_stack |
| `dual_cover_shift` | R2_W–R2_E, R2_W–R2_N | west_stack, east_stack |
| `north_west_detour` | R2_N–R2_NW, R2_NW–R2_W | west_stack, north_screen |
| `north_east_detour` | R2_E–R2_NE, R2_NE–R2_N | east_stack, north_screen |
| `cross_aisle_restricted` | R2_W–R2_E, R2_W–R2_N, R2_E–R2_N | rack_mid, south_stack |

#### Warehouse closure profiles

| Scenario | Removed links | Added cover rectangles |
| --- | --- | --- |
| `both_routes_open` | None | None |
| `west_merge_closed` | A_MERGE–R5_WEST | None |
| `east_merge_closed` | B_MERGE–R5_EAST | None |
| `west_inner_closed` | A_MERGE–R5_SOUTH | None |
| `east_inner_closed` | B_MERGE–R5_SOUTH | None |
| `south_center_closed` | R5_SOUTH–R5_CENTER | None |
| `west_cross_closed` | R5_WEST–R5_CENTER | None |
| `east_cross_closed` | R5_EAST–R5_CENTER | None |
| `west_reader_approach_closed` | A_NORTH–R5_READER | None |
| `east_reader_approach_closed` | B_NORTH–R5_READER | None |
| `central_reader_approach_closed` | R5_CENTER–R5_READER | None |
| `both_crossings_closed` | R5_WEST–R5_CENTER, R5_EAST–R5_CENTER | loaded_west, loaded_east |

Cover rectangle sizes are modeled, with axis-aligned bounds in metres.
The fixed rectangles apply to every fixture. Named overlays apply only
when listed in the closure profile.

| Occluder | X min | X max | Z min | Z max |
| --- | ---: | ---: | ---: | ---: |
| Fixed 1 | -11.2 | -10.8 | -20 | -2 |
| Fixed 2 | 10.8 | 11.2 | -20 | -2 |
| Fixed 3 | -3.65 | 3.65 | -2.25 | -1.75 |
| Fixed 4 | -11 | -6.5 | -2.25 | -1.75 |
| Fixed 5 | 6.5 | 11 | -2.25 | -1.75 |
| Fixed 6 | -3.5 | -1.6 | -13.9 | -8.7 |
| Fixed 7 | 1.6 | 3.45 | -13.4 | -8.5 |
| Fixed 8 | 6.95 | 9.45 | -4.7 | -4.3 |
| Fixed 9 | -8.65 | -6.7 | -11.35 | -10.65 |
| Fixed 10 | 6.7 | 8.65 | -11.35 | -10.65 |
| Fixed 11 | -7.5 | -6.9 | 24 | 37 |
| Fixed 12 | 6.9 | 7.5 | 24 | 37 |
| Fixed 13 | -4.5 | -3.85 | 27 | 35 |
| Fixed 14 | 3.85 | 4.5 | 27 | 35 |
| Fixed 15 | -21 | -19.5 | 5 | 13 |
| Fixed 16 | 19.5 | 21 | 5 | 13 |
| rack_mid | -0.6 | 0.6 | -11.5 | -9.5 |
| north_screen | -1 | 1 | -5.5 | -4.75 |
| south_stack | -0.65 | 0.65 | -16.3 | -14.75 |
| west_stack | -6.3 | -5.5 | -13 | -11.75 |
| east_stack | 5.05 | 6.15 | -12 | -10.75 |
| loaded_west | -5.9 | -5.1 | 30 | 32 |
| loaded_east | 5.1 | 5.9 | 30 | 32 |
