# Room 2 patrol utility and decision traces



The patrol state machine uses Unity navigation to move and a utility policy to choose destinations. The controlled calculations below are reproducible examples of that policy.

## States and dispatch triggers

| State | Entry | While active | Exit |
| --- | --- | --- | --- |
| Patrol | Startup or completed/expired investigation | Select a waypoint, move, record arrival, pause and aim sweep | In-room visual contact or eligible in-room disturbance |
| Pursue | Visible player in Room 2 | Update last-seen position while visible, repath at nominal 0.25 s interval | Loss of sight or player/guard exits the Room 2 predicate |
| Investigate | Disturbance or last-seen position | Go toward observation, then stop and scan | In-room reacquisition, travel deadline or scan expiration |

A patrol arrival can use the movement component's complete-path arrival test
or a planar distance within the configured threshold (default 0.6 m).
Arrival writes the waypoint timestamp, then stops to sweep ±35° using the
arrival-facing direction. The inspection timeout clears aim before the next
utility decision. Perception is checked before handling the wait timer, so a
visible player can interrupt an inspection.

## Draft fixed-loop worksheet for comparison

The following worksheet assumes a fixed successor and uses hypothetical leg
times. It is a baseline for comparing coverage sequences before reading
the policy trace.

| Index | Point | Fixed successor | Travel assumption | Dwell | Example time since last visit at leave |
| --- | --- | --- | ---: | ---: | ---: |
| 0 | South crossing | East south | 3.1 | 1 | 0 |
| 1 | East south | East corridor | 2.0 | 2 | 0 |
| 2 | East corridor | Northeast | 1.8 | 1 | 0 |
| 3 | Northeast | North crossing | 1.9 | 2 | 0 |
| 4 | North crossing | Northwest | 1.8 | 1 | 0 |
| 5 | Northwest | West storage | 2.2 | 2 | 0 |
| 6 | West storage | Southwest | 1.9 | 1 | 0 |
| 7 | Southwest | South crossing | 1.1 | 2 | 0 |

### Draft loop variants

| Input event | Fixed-loop expectation | Observation to compare with utility selection |
| --- | --- | --- |
| Arrival at east south | Resume toward east corridor | Would a long-neglected west point outrank the adjacent one? |
| Northern visibility improves | Preserve array order | Does the improved visibility alter the ranking for equal ages? |
| West leg lengthens | Preserve array order | Can a short detour lower travel cost enough to change the winner? |
| Current entry is unreachable | Skip to next array entry | Confirm evaluation of all remaining candidates |
| Chase interrupts east inspection | Resume from next fixed entry | Record the latest real arrival and score the whole available set |
| Two candidates have same score | Advance to fixed successor | Keep first eligible entry under a strict greater-than comparison |
| One reachable alternative remains | Rotate past failures | Once one remains, inspect its eligibility relative to current index |
| Player is behind a shelf | Continue scheduled leg | Leave candidate openness separate from confirmed visual contact |

The fixed-loop baseline uses distance and elapsed time only for annotation.
The active implementation treats path and age as features used to choose the
next inspection. The data recorded at selection should be kept separate from
the time spent traveling to the selected point.

## Utility formula

For nonnegative age and path length and visibility already in [0, 1]:

```text
U = 0.65 * min(age / 30, 1) + 0.35 * visibility - 0.25 * min(pathLength / 30, 1)
```

`PatrolDecisionPolicy.Score` uses `Mathf.Clamp01` on all three inputs
(`age / 30`, `visibility`, `pathLength / 30`). Age uses seconds, and complete
path length uses metres. Neglect contributes 0 to 0.65, local visibility 0
to 0.35, and travel subtracts 0 to 0.25. Utilities can be negative. The search
starts with negative infinity and still considers the highest eligible score.

The per-index timestamp array initializes to `Time.time - 30` when created
or resized, so its initial age feature is saturated. Record the timestamp
upon arrival, before dwelling. During the dwell itself the timestamp ages.

## Rejected and special cases

| Controlled case | Path query | Sampled X/Z | Result |
| --- | --- | --- | --- |
| Safe-side goal | Complete | (0, −20) | Rejected by Room 2 endpoint check |
| Raw endpoint outside but projected inside | Complete | (−10.9, −10) | Passes boundary check on sampled endpoint |
| Point on isolated polygon | Partial | (8.2, −4.5) | Rejected before scoring |
| Agent absent from NavMesh | Unavailable | Any | Query returns false before sampling |
| Current index with multiple configured entries | Complete | In bounds | Recorded as current inspection point, skipped for scoring |
| Current index with exactly one configured entry | Complete | In bounds | Can be scored |
| All valid points fail dispatch | No destination accepted | Any | Stop and allow the next eligible attempt after the delay |

The diagnostics may call an outside endpoint `No complete path` because
reachability and the room predicate feed the same boolean. Inspect the
sampled coordinates and actual path status to distinguish causes.

## Visibility and travel features

`GuardDecisionQueries.Visibility` offsets the sampled position upward by
1.5 m and sends eight horizontal rays of 6 m, spaced by 45° starting at
world forward. Each clear ray contributes 0.125. The query ignores triggers,
ignores hits on the owning transform or its children and considers a filled
32-hit buffer occluded. This signal estimates local openness around a
candidate. It is independent of the live player sight test.

| Clear rays | Visibility | Weighted term |
| ---: | ---: | ---: |
| 0 | 0.000 | 0.00000 |
| 2 | 0.250 | 0.08750 |
| 4 | 0.500 | 0.17500 |
| 6 | 0.750 | 0.26250 |
| 8 | 1.000 | 0.35000 |

| Complete path (m) | Normalized length | Penalty |
| ---: | ---: | ---: |
| 3 | 0.1 | 0.025 |
| 9 | 0.3 | 0.075 |
| 18 | 0.6 | 0.150 |
| 30 | 1 | 0.250 |
| 45 | 1 | 0.250 |

For a complete path with legs 4 m north, 3 m east and 2 m north, sum
the corners to get 9 m. The straight-line displacement is approximately
6.708 m. Feeding that displacement into utility would incorrectly reduce
the travel penalty from 0.075 to approximately 0.056.

## Candidate gate order

1. Skip null transforms and confirm the agent is on a NavMesh.
2. Sample within 1.5 m using the movement agent's type and area mask.
3. Require a successful path calculation with status `PathComplete`.
4. Sum Euclidean segment lengths using the returned 3D corners.
5. Check the sampled point against the inclusive Room 2 X/Z predicate.
6. Exclude the current index if the configured array has more than one entry.
7. Compute age, openness and utility for each remaining point.
8. Choose the strictly greatest score and send the sampled target to
   `EnemyMovement.SetDestination`, which recalculates and validates the path.
9. Record the index and `SelectedDecisionTarget` only after acceptance.

A completed query is a selection-time result. If destination submission
fails, the state retains previous selection fields and stops movement.
`nextDecisionAt` bounds subsequent attempts through `AdvanceWaypoint`
to approximately one-second spacing. Entering Patrol calls dispatch directly.
The patrol update's planar arrival fallback or a stopped previous path can
affect when the next attempt actually happens.

## Player sight is a state transition input

`GuardPerception.CanSeePlayer` checks 3D range, horizontal angle and cover
rays toward derived head/chest targets. Cached animated head direction can
replace body-forward. Defaults in `PatrolGuardAI` are 16 m range and a
90° total aperture. Within 2.5 m the shared perception helper uses at least
110° total aperture. The serialized values in a loaded scene can override
the code defaults. The checked-in Room 2 guard uses 16 m and 90°.

| Controlled player pose | Range | Angle from gaze | Cover | Expected default outcome |
| --- | ---: | ---: | --- | --- |
| In Room 2, open aisle | 10 m | 30° | Clear | Pursue on visual confirmation |
| In Room 2, side of gaze | 10 m | 50° | Clear | Stay in current patrol/inspection behavior |
| In Room 2, close periphery | 2 m | 50° | Clear | Close-range aperture can confirm |
| In Room 2, behind shelf | 10 m | 30° | Blocks head and chest | No visual confirmation |
| Beyond either border | Within 16 m | Within aperture | Clear | Patrol does not pursue the out-of-room target |

Keep the guard's pose and occluders fixed while introducing the player.
Compare `CanSeePlayer` with `HasConfirmedVisualContact()` at the boundary.
The latter additionally checks player membership in Room 2.

## Controlled comparisons

Inputs in this table are controlled mathematical cases. The visibility column shows clear rays divided by eight.
The 10-ray case deliberately feeds the scorer an out-of-range value to show the function's clamp.
Compare full precision before rounding. A tie uses the first eligible array entry encountered.

### Neglect beats proximity

| Candidate | Age (s) | Clear rays | Path (m) | Age term | Visibility term | Travel term | Utility |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| A | 30 | 4 | 20 | 0.650000 | 0.175000 | 0.166667 | 0.658333 |
| B | 0 | 4 | 1 | 0.000000 | 0.175000 | 0.008333 | 0.166667 |

Candidate A ranks higher.

### Local openness breaks an age tie

| Candidate | Age (s) | Clear rays | Path (m) | Age term | Visibility term | Travel term | Utility |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| A | 15 | 6 | 9 | 0.325000 | 0.262500 | 0.075000 | 0.512500 |
| B | 15 | 2 | 9 | 0.325000 | 0.087500 | 0.075000 | 0.337500 |

Candidate A ranks higher.

### Travel breaks an openness tie

| Candidate | Age (s) | Clear rays | Path (m) | Age term | Visibility term | Travel term | Utility |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| A | 15 | 4 | 3 | 0.325000 | 0.175000 | 0.025000 | 0.475000 |
| B | 15 | 4 | 18 | 0.325000 | 0.175000 | 0.150000 | 0.350000 |

Candidate A ranks higher.

### Age above the cap changes nothing

| Candidate | Age (s) | Clear rays | Path (m) | Age term | Visibility term | Travel term | Utility |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| A | 30 | 5 | 12 | 0.650000 | 0.218750 | 0.100000 | 0.768750 |
| B | 49 | 5 | 12 | 0.650000 | 0.218750 | 0.100000 | 0.768750 |

Equal utility. The earlier eligible entry wins.

### Path lengths above the cap tie on cost

| Candidate | Age (s) | Clear rays | Path (m) | Age term | Visibility term | Travel term | Utility |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| A | 21 | 6 | 30 | 0.455000 | 0.262500 | 0.250000 | 0.467500 |
| B | 21 | 6 | 45 | 0.455000 | 0.262500 | 0.250000 | 0.467500 |

Equal utility. The earlier eligible entry wins.
