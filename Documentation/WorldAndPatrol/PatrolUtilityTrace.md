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
