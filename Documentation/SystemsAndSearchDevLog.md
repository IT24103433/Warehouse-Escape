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
