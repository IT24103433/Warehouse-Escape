# Combat controls and damage feedback

- Space jumps while grounded and standing. Air jumps, crouched jumps, and paused/dead jumping are blocked.
- C toggles crouch. The capsule keeps its feet fixed, movement halves, and the camera lowers. Standing is blocked under solid overhead obstacles.
- Hold right mouse to aim down sights with smooth 0.65x camera FOV and slower movement. This is an iron-sight zoom, not a telescopic optic added to every weapon.
- Accepted player damage triggers fading blood around the screen edges. Guard damage triggers a short blood-particle burst. Safe-room immunity still applies.
- Guard health: patrol 120, search 180, defensive 220, both final flankers 360. Player stays at 100.
- Shotgun pellet damage rises from 7 to 12 for player and guard: up to 72 per six-pellet shot before spread/cover.
- Gunshots already publish sound events. Each guard hears a distance- and obstacle-attenuated origin and investigates that location. Hearing does not grant visual confirmation, wall penetration, or knowledge of subsequent silent player movement.

Scene setup can be safely reapplied with ApplyCombatControls.Execute; health is assigned to explicit values rather than multiplied repeatedly.

## Verification

The 54-test full run passed 51 checks. The three remaining failures were death/restart fixtures still assuming 100 damage killed the former 60-HP guard. Updated them to apply the guard's remaining health and assert restart restores MaxHealth. Targeted rechecks verify those scenarios alongside grounded jumping, no mid-air/crouched jump, crouch overhead clearance, shader support, gunshot memory, doubled health, and HUD text.

Final verification: all 54 unique checks passed across the full run and targeted rechecks. Windows release build succeeded with zero errors (one existing unused-field warning). Standalone startup check exited normally with no exceptions.
