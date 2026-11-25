# Room traps and final-room squad

The five-room layout and either-card exit remain unchanged. There are now five
enemy guards in total: one in each intermediate combat room and **two** instances
of the same flanking boss type in Room 5.

## Traps

| Room | Trap | Effect |
| --- | --- | --- |
| 1, safe start | Training pad | Demonstrates warning and activation; no damage and no alarm report |
| 2, patrol | Shock pad | 10 HP per pulse; reports an alarm when armed |
| 3, search route | Security alarm | Alerts guards to the trap location; no damage |
| 4, defensive route | Shock pad | 15 HP per pulse; reports an alarm when armed |
| 5, final | Two shock pads | 20 HP per pulse, one in each side aisle |

Pads are blue when ready, amber during a **1.25-second warning**, red for a
one-second active interval and grey during a five-second cooldown. Shock pulses
are spaced by at least half a second. Stepping off the trigger area avoids
shock damage. Pause and end-game states suspend the trap state machine.

The plate has no solid collider; the sensor is a trigger. Traps do not add
navigation barriers, close doorways or require a new NavMesh bake. Players can
move around the pad or wait for cooldown. Labels only display nearby with an
unobstructed sight line, to avoid signs from other rooms overlapping gameplay.
Security traps recognise the player's Health component; they do not damage guards.

`WarehouseTrap` owns the warning/active/cooldown lifecycle. Damage uses the same
Health system as weapons, retaining safe-room immunity and death-at-zero rules.
`SoundType.Alarm` uses the existing hearing/obstacle attenuation system. The
reported position is the trap location, not the player's hidden current location.

## Two coordinated flanking guards

Both final-room guards use `FlankingGuardAI`, 180 HP, SCAR weapons and the same
movement/perception architecture. The second guard has its own patrol waypoints
and spawn, rather than sharing the original guard's exact position.

`BossSquadMember` coordinates the pair:

- Slot 0 prefers the left flank; slot 1 prefers the right. Each selects the
  lowest-cost viable candidate on its assigned side. If that side has no viable
  route, normal reachable-flank fallback remains available.
- A position within three metres of the partner's reserved flank goal receives
  an additional 12-metre cost penalty, discouraging both guards from stacking.
- Confirmed sightings are shared at most twice per second. The receiving guard
  searches the reported last-known location; the report never grants visual
  contact or permission to shoot. Alarm/gunshot hearing also supplies search goals.
- When both guards can see the player, they alternate 1.3-second suppression
  windows. A guard may cover its partner's reload, or fire normally if its
  partner cannot see the player.
- Every shot still requires the shooter's own confirmed visual contact and a
  clear bullet path. Another guard physically obstructing the shot blocks it.
- When one guard dies or is disabled, its partner releases flank reservations
  and suppression scheduling and continues independently.

The original single-agent candidate costs and complete-path checks remain in
place. Cooperation adds role assignment, spatial reservations and fresh shared
observations; it does not replace the flanking agent with a different type.

## Rebuilding and verification

`Warehouse/Setup Traps And Boss Squad` creates the scene content and saves the
scene normally. Re-running it does not duplicate traps or create a third boss.
`TrapAndSquadTests` checks room coverage, trigger-only geometry, two matching
bosses, warning-before-damage, pause, evasion, safe-room protection, alternating
suppression, genuine sighting reports and partner-death recovery.
The full walkthrough suite also checks both card routes and a five-guard restart.

20 October verification: all six new focused tests passed. The full 53-test run
passed 51 tests, exposing a trap-label shader issue and an old moving-target
assumption in a controlled boss damage fixture. Labels now use depth-tested
materials and the fixture holds both bosses stationary during ray/cover checks.
All eight targeted rechecks passed, including evasion while still outside the
safe room and accumulated small updates to the partner's navigation goal.
All 53 unique checks are verified across the full run and rechecks.
