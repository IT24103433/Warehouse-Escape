# Informed guard decisions and VIVA guide

Each agent retains its state machine for behaviour transitions. A separate,
role-specific utility policy now chooses movement goals using measurable world
features. Unity NavMesh supplies complete paths; this is **not a custom A***
implementation. The authored contribution is candidate generation, filtering,
scoring, memory updates and replanning above that navigation service.

## Varun — coverage-aware patrol

`PatrolGuardAI` evaluates the room's inspection points instead of advancing a
fixed circuit. `PatrolDecisionPolicy` maximises:

`U = 0.65 min(age / 30, 1) + 0.35 visibility - 0.25 min(pathLength / 30, 1)`

Age is seconds since the point was actually inspected, not since it was selected.
Visibility is the fraction of eight horizontal six-metre rays that are clear.
Path length is the sum of complete NavMesh path segment lengths in metres.
The current point is excluded when alternatives exist. Inaccessible points and
points outside Room 2 are rejected. No raw unreachable destination is used as
a fallback. A failed selection retries after one second.

The neglect weight exceeds the maximum visibility advantage and makes old
areas competitive against repeatedly visiting nearby open aisles. Travel cost
discourages expensive cross-room journeys without reducing the policy to
nearest-neighbour selection. The 30-second age scale matches several room
inspections; the 30-metre cost scale is comparable to crossing this warehouse
sector. These are initial design choices, not experimentally optimal values.

## Sanjeevsaran — belief-guided best-next-view search

`SearchGuardAI` creates up to 17 distinct reachable hypotheses: the last observed
location and two eight-point rings at 2.5 and 5 metres. Only observations update
the search origin; hidden live player coordinates never generate candidates.
The heading estimate uses successive **visible** observations. A sound-only
observation clears heading because a sound alone does not establish movement.

Initial discrete weights are normalised to sum to one:

`prior_i proportional to exp(-distance_i² / 32) × (1 + max(0, heading dot direction_i))`

The distance falloff favours nearby hypotheses; the directional term gives at
most a 2:1 preference for continuing in the observed direction. It cannot
eliminate other possible routes.

For each uninspected candidate, sum the belief at locations observable within
six metres, the inspection sweep's field of view and unoccluded sight lines.
Choose the highest:

`U_i = observableBelief_i / (1 + 0.15 × pathLength_i)`

This is a greedy informed best-next-view policy, not an exhaustive search of
every future observation sequence. At about 6.7 metres the denominator doubles,
so twice the predicted information is needed to justify that extra travel.

After an unsuccessful local inspection, multiply observable beliefs by `0.08`.
This negative-evidence likelihood discounts inspected locations without claiming
certainty of absence. Residual mass is deliberately **not** renormalised: it
represents remaining support for the player still being in this finite search
region. Stop when all candidates are inspected, residual mass falls below 0.05,
or the configured 12-second search deadline expires. New sound evidence rebuilds
the hypotheses at most once per 0.75 seconds, coalescing rapid automatic fire;
visual reacquisition immediately resumes pursuit.

The visibility/negative-evidence model is an approximation: it does not model
every occluded body part or continuously diffuse probability through the room.
Do not describe it as a calibrated probabilistic tracker.

## Aqqil — objective-preserving tactical defense

`DefensiveGuardAI` evaluates the protected anchor plus two eight-point rings.
Ring radii are 2.5 and 5 metres, capped inside the leash. A candidate must have
a complete path and lie inside both the assigned room and leash.

Without a recent threat:

`U = 0.45 objective + 0.30 visibility + 0.25 freshness - 0.15 travel`

With a recent visual or sound observation:

`U = 0.30 objective + 0.25 cover + 0.30 range + 0.15 freshness - 0.15 travel`

- Objective: proximity to the anchor protecting the access-card area.
- Cover: strongest for blocked torso-level sight with clear eye-level sight;
  fully occluded cover earns less because it limits engagement.
- Range: highest near five metres, suited to the guard's shotgun; positions
  with blocked eye-level sight receive a range discount.
- Freshness: time since inspecting that tactical point, normalised over 20 s.
- Travel: complete path length normalised over 20 m.

Threat observations expire after 7.5 seconds. Tactical decisions run at most
every 0.75 seconds, and a 0.04 switching bonus favours the current combat post
to reduce jitter. Hearing permits investigation, never firing without confirmed
visual contact. The player need not be killed to access either route.

## Inspecting and defending the implementation

During Play Mode, select a guard and inspect **Decision Candidates**. Each entry
records its sampled position, complete-path status, path length, feature values,
utility and rejection/reason text. `SelectedDecisionTarget` and `DecisionCount`
are available to tests/debuggers; magenta Scene-view gizmos mark the selected
goal. Policies are separate from visuals and world
queries, so their tradeoffs can be tested without a rendered scene.

For each member's VIVA, demonstrate a decision changing when one input changes:

1. Patrol: a neglected farther aisle beating a freshly visited nearby point.
2. Search: a larger observable belief beating a low-information location,
   followed by reduced priority after an empty inspection.
3. Defense: a protected close-range position beating an exposed distant one,
   with every selected point remaining inside the objective leash.

`InformedGuardDecisionTests` covers these tradeoffs and actual scene integration,
including bounds, complete paths, negative observations and hidden-player safety.
The existing physical walkthrough suite verifies both routes and game rules.

Candidate sets are bounded. Patrol work is O(k), defense O(17), and search
visibility comparisons O(n²), n <= 17. Path calculations and ray queries happen
at decision boundaries or the bounded defense interval, not for every candidate
on every rendered frame. Raycasts reuse a fixed buffer. This is a bounded-work
design rationale; actual frame-rate claims still require profiling on the target
machine. Weights should be compared against alternatives during playtesting.

## Reproducible score examples and verification

These are controlled feature examples from the policy tests, not measured player
positions or claims about optimal play:

| Policy | Candidate A | Candidate B | Utilities, A versus B |
| --- | --- | --- | --- |
| Patrol | age 30 s, visibility 0.5, path 20 m | age 0 s, visibility 0.5, path 1 m | 0.658 versus 0.167 |
| Search | observable belief 0.7, path 3 m | observable belief 0.2, path 3 m | 0.483 versus 0.138 |
| Threatened defense | objective 0.8, cover 1, range 1, freshness 0.5, path 5 m | same objective/freshness/path, cover 0, range 0 | 0.828 versus 0.278 |

19 October verification:

- Six new policy/integration tests passed, including actual NavMesh target
  selection and residual-belief reduction after inspection.
- Full regression run: 46 of 47 passed. The remaining melee fixture assumed
  the old patrol facing direction. It now explicitly establishes a frontal
  sighting, suppresses automatic attacks and tests health transitions in isolation.
  Its targeted recheck passed.
- Final nine-test recheck passed after bounding sound-triggered replanning:
  the six new tests plus existing hearing, memory-expiration and blind-fire checks.
- Thus all 47 unique checks are verified across the full run and targeted
  rechecks. Local NUnit evidence is under `scratch/informed-*.xml`.
