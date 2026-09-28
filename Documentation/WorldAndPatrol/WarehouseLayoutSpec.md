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
