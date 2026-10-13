# Warehouse Escape — Systems & Search Guard DevLog (Sanjeevsaran J.)

## 2026-09-28 — Initialize systems engineering and interaction architecture notes
Established the systems engineering scope covering player-world interactions, keycard progression, and Route A pursuit logic.
Outlined separation between interactive world triggers, inventory ownership state, and AI guard decision policies.

## 2026-09-29 — Define player interaction detection raycast and trigger range
Specified center-screen interaction raycast parameters and proximity bounds for reliable target acquisition.
Filtered non-interactive geometry layers so doorway frames and props do not occlude valid interactables.

## 2026-09-30 — Draft AccessCard pickup state and ownership tracking logic
Defined AccessCard identity tags and player ownership flags for Route A and Route B clearance tracking.
Ensured picked-up cards deactivate cleanly in the scene while persisting ownership across room transitions.

## 2026-10-01 — Add contextual UI prompt specs for E-key card pickup
Documented hover prompt formatting for E-key interaction cues when targeting AccessCard pickups and terminals.
Added clear prompt suppression rules when a card has already been collected or is out of reach.

## 2026-10-02 — Design CardReader clearance check for Route A and Route B cards
Structured CardReader validation to accept either Route A or Route B clearance without coupling the two paths.
Isolated reader verification logic so objective state updates deterministically on valid card presentation.

## 2026-10-03 — Document locked door feedback when access card is missing
Added feedback specifications for denied CardReader attempts when the player lacks the required clearance card.
Defined audio/UI indicator cues distinguishing missing-card denial from an unlocked transition.

## 2026-10-04 — Connect CardReader validation to single-trigger victory flow
Wired successful CardReader authorization to the escape gate activation and single-trigger victory state.
Guarded completion callbacks against duplicate E-key presses during the end-of-run transition.

## 2026-10-05 — Refine interactive door opening and collision response notes
Refined hinged and sliding warehouse door opening arcs alongside collider state updates during animation.
Ensured NavMesh obstacles and physical colliders remain synchronized as doors transition open or shut.

## 2026-10-06 — Log barricade and physics-based interactive prop constraints
Documented rigidbody mass, drag, and push-interaction constraints for movable warehouse barricades and crates.
Prevented lightweight props from wedging inside tight corridors or destabilizing player movement.

## 2026-10-07 — Verify player-object collision behavior around room doorways
Reviewed character controller step-offset and capsule clearance around warehouse doorway thresholds.
Confirmed interactive props and door frames allow smooth strafing without snagging during combat retreats.

## 2026-10-08 — Document end-to-end card pickup to terminal escape sequence
Mapped the full interaction flow from AccessCard collection through CardReader clearance to escape completion.
Verified state handoffs between HUD prompts, inventory flags, and escape terminal triggers.

## 2026-10-09 — Outline SearchGuardAI state machine for Route A sector
Outlined core SearchGuardAI states covering Route A patrol, visual chase, belief-guided search, and combat.
Separated high-level state transitions from the underlying movement goal selection policy.

## 2026-10-10 — Define player perception FOV and distance detection thresholds
Established detection cone angle and effective sighting range for SearchGuardAI in the Route A sector.
Tuned close-range peripheral awareness while preserving stealth flanking opportunities behind shelving.

## 2026-10-11 — Document physical line-of-sight checks against solid cover
Specified raycast occlusion checks from guard eye height to player torso and head targets against solid cover.
Blocked visual acquisition through warehouse racks, walls, and closed doors so sight matches geometry.

## 2026-10-12 — Add sound event investigation and barrier attenuation notes
Added acoustic event propagation rules for gunfire and trap alerts with attenuation across structural barriers.
Bounded sound-triggered hypothesis rebuilds to a 0.75-second cooldown to coalesce automatic weapon bursts.

## 2026-10-13 — Implement lastKnownPlayerPosition memory storage on sight loss
Recorded lastKnownPlayerPosition and estimated heading exclusively from confirmed visual or sound observations.
Prevented hidden live player coordinates from leaking into search origin initialization after line-of-sight breaks.
