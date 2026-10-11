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
