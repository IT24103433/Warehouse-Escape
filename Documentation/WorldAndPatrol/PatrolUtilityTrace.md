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

## Selection, arrival and re-evaluation timeline

| Event | Example time (s) | Action | Memory consequence |
| --- | ---: | --- | --- |
| Dispatch | 100 | Rank two eligible candidates | Preserve their previous inspection timestamps |
| Accepted | 100 | Store winning index and sampled goal | No inspection timestamp change |
| Arrival | 104 | Stop movement and begin dwell | Winning point gets last-inspected time 104 |
| Dwell finish | 106 | Clear aim and rank alternatives | Winning point is current and excluded with a multi-entry array |
| Later re-evaluation | 118 | Compare available points again | If previously inspected winner is now eligible, its age is 14 s |

If movement is interrupted before arrival, carry the old inspection
timestamp into the next ranking. A change in destination order can come from
the measured path or visibility without any change in configured points.

## Room 2 combat defaults and evidence

When `Health` is missing, `Awake` creates it, names the character
`Varun [Patrol]` and sets max health to 120. When `GuardShooting` is missing,
the fallback configures a UMP SMG with damage 14, cooldown 1.1 s, range
13 m, burst count 2 and spread 2.5. Pre-existing components retain their
setup at this call site.

| Field | Checked-in Room 2 guard | Fallback on missing component |
| --- | ---: | ---: |
| Max HP | 120 | 120 |
| UMP damage per shot | 12 | 14 |
| Firing cooldown (s) | 1.35 | 1.1 |
| Firing range (m) | 13 | 13 |
| Shots per burst | 2 | 2 |
| Accuracy spread | 3 | 2.5 |

The scene values come from `Health` and `GuardShooting` serialized on
`Guard_Varun_Patrol`. The active guard must also be in Pursue for
`GuardShooting.HasVisualTarget` to permit it to fire.

With the default 16 m detection and 13 m attack ranges, a clear line at
14 m can support pursuit before closing to firing range. Test aim and
damage at a comfortably inside firing location and then repeat a little
outside it. Confirmed visual contact, weapon range and bullet obstruction
all contribute to whether the guard can land a shot. Compare each gate
individually before attributing missed damage to health or weapon balance.

## Losing sight and returning to patrol

While pursuing, sight loss or an out-of-room player/guard changes state to
Investigate using the stored `lastSeenPosition`. Visible pursuit refreshes
this memory. The investigation begins with a 2 m NavMesh sampling request
using all areas, tries the sampled point when available, and otherwise
submits the raw observation via `EnemyMovement.SetDestination`. That movement
method still rejects an incomplete path. This is a separate path from the
patrol candidate filter, with no equivalent sampled-endpoint Room 2 gate in
`TransitionToInvestigate`.

- Travel phase: allow up to 8 s and watch the movement-arrival test.
- Scan phase after arrival: stop and sweep ±45° for the configured investigation
  duration, which defaults to 2.5 s.
- Reacquisition: confirmed in-room player visibility can move directly to Pursue.
- Recovery: after timeout or scan completion, clear aim, reset perception and
  dispatch a new patrol choice. Separate the last observation from the player's
  hidden position when recording the test.

## Shock pad and alarm interaction

`SetupWarehouseTrapsAndSquad` places `Room2_Shock` at (−4, 0, −12)
with a configured 10 HP pulse. The scene instance keeps X/Z at (−4, −12),
with local Y approximately 0.057, and uses a 1.6 m × 1.8 m × 1.6 m trigger.
The scene values are 1.25 s warning, 1 s active and 5 s cooldown. The trigger
can be bypassed. The alarm reports the pad position. The patrol's visual-only memory
configuration still permits eligible gunshot and alarm sounds, subject to the
sound query and attenuation. `HandleDisturbance` accepts an inside-room
position while in Patrol or Investigate.

| Situation | Investigation input | Verification |
| --- | --- | --- |
| Audible pad and concealed player | Pad coordinates | Observe investigation without shooting at the concealed player |
| Audible pad during active pursuit | Existing pursuit inputs | Patrol disturbance handler does not replace active pursuit |
| Player steps off during warning | Trap state and player health | Verify the player avoids the shock pulse |
| Player walks around trigger | Independent movement route | Verify complete alternate approach and no trap activation |

Compare alarm location, last visual position and player position as three
separate observations. See [TrapsAndBossSquad](../TrapsAndBossSquad.md) for
trap timing and published team verification.

## PlayMode verification record

| Check | Evidence available | Expected observation on replay |
| --- | --- | --- |
| Patrol utility monotonicity | Repository test `Patrol_Changes_Priority_With_Neglect_Visibility_And_Travel` | Terms move ranking as specified with independent inputs |
| Reject out-of-room patrol point | Repository test `Patrol_Rejects_Out_Of_Room_Candidate_And_Reports_Decision` | Valid in-room selection and rejected Room 1 candidate |
| Safe approach and branch entrances | Repository `PhysicalWalkthroughTests` and source review | Protected south side and usable Route A and B entrances |
| LOS loss then return | Manual replay pending | Investigate the last visible point, then return to Patrol |
| Failed investigation approach | Manual replay pending | No complete approach and return on the travel deadline |
| Reacquired in-room player | Manual replay pending | Investigate is immediately interrupted by Pursue |
| Doorway turn interruption | Manual replay pending | Record bounds, destination result, path status and actual arrival |

Read the repository reports for prior run results. This documentation work
inspected tests and source and generated planning fixtures. Unity PlayMode
was not run for this sequence.

For a recovery replay, start from a selected patrol target, expose the player
inside the room, then move behind the partition or beyond the boundary.
Record the last visible position before the state switches. Move the hidden
player again, compare it with `InvestigatePosition`, and collect the time
and `DecisionCount` when fresh patrol evaluation resumes. Run a separate
pass with visual reacquisition before the investigation scan ends.

## End-to-end example from the waypoint dataset

At origin R2_W, compare R2_E at age 13.6 s and R2_N at age 15 s.
Both candidates are inside and have a modeled complete path in each
scenario. These ages are controlled inputs added to the CSV observations.

| Scenario | Target | Length (m) | Openness | Age term | Openness term | Travel term | Utility |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: |
| `clear_graph` | R2_E | 9.5525 | 0.750 | 0.294667 | 0.262500 | 0.079604 | 0.477562 |
| `clear_graph` | R2_N | 6.4031 | 0.625 | 0.325000 | 0.218750 | 0.053359 | 0.490391 |
| `center_crossing_closed` | R2_E | 11.8115 | 0.750 | 0.294667 | 0.262500 | 0.098429 | 0.458737 |
| `center_crossing_closed` | R2_N | 6.4031 | 0.500 | 0.325000 | 0.175000 | 0.053359 | 0.446641 |

R2_N wins with the open crossing. In the closure fixture the east route
lengthens, but the added modeled cover removes one clear ray at R2_N.
The north candidate loses 0.04375 and the east candidate loses about
0.01883, flipping their order in this two-candidate comparison.
Use the path and occluder tables in the [layout spec](WarehouseLayoutSpec.md)
to reproduce the inputs.

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

### Clamping rejects out-of-range arithmetic

| Candidate | Age (s) | Clear rays | Path (m) | Age term | Visibility term | Travel term | Utility |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| A | -5 | 10 | -2 | 0.000000 | 0.350000 | 0.000000 | 0.350000 |
| B | 0 | 8 | 0 | 0.000000 | 0.350000 | 0.000000 | 0.350000 |

Equal utility. The earlier eligible entry wins.

### A reachable point can have negative utility

| Candidate | Age (s) | Clear rays | Path (m) | Age term | Visibility term | Travel term | Utility |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| A | 0 | 0 | 18 | 0.000000 | 0.000000 | 0.150000 | -0.150000 |
| B | 1 | 0 | 30 | 0.021667 | 0.000000 | 0.250000 | -0.228333 |

Candidate A ranks higher.

### Open but farther versus sheltered and near

| Candidate | Age (s) | Clear rays | Path (m) | Age term | Visibility term | Travel term | Utility |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| A | 14 | 7 | 21 | 0.303333 | 0.306250 | 0.175000 | 0.434583 |
| B | 14 | 2 | 4 | 0.303333 | 0.087500 | 0.033333 | 0.357500 |

Candidate A ranks higher.

### Near-equal scores must be ranked before display rounding

| Candidate | Age (s) | Clear rays | Path (m) | Age term | Visibility term | Travel term | Utility |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| A | 12 | 4 | 12.02 | 0.260000 | 0.175000 | 0.100167 | 0.334833 |
| B | 12 | 4 | 12 | 0.260000 | 0.175000 | 0.100000 | 0.335000 |

Candidate B ranks higher.
