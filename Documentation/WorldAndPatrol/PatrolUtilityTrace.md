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
