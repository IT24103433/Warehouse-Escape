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
