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
