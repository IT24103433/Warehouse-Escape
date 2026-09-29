# DefensiveGuardAI & DefenseDecisionPolicy Trace

**Owner:** Aqqil (`IT24104314`)
**Scripts:** `Assets/Scripts/DefensiveGuardAI.cs`, `Assets/Scripts/DefenseDecisionPolicy.cs`
**Assigned Sector:** `Room_4_Defensive` (Route B — Access Card B)

<!-- BLOCK:POLICY_INTRO -->
## 1. Scope and Design Goals

The Room 4 defensive guard (`Guard_Aqqil_Defensive`) protects the Access Card B security
terminal (`AccessCard_Pickup_Defensive`) without abandoning Room 4 to chase the player
across the warehouse. Unlike `PatrolGuardAI` (Room 2 circuit) or `SearchGuardAI` (Room 3
belief search), `DefensiveGuardAI` stays anchored to a defensive post, evaluates nearby
cover and shotgun engagement range, and returns to post when the intruder disengages.

### 1.1 Baseline Design Parameters

- **Sight Range / FOV:** `10.0 m` / `90 deg` (`eyeHeight = 0.6 m`)
- **Defense Perimeter:** `defenseRadius = 6.0 m` around `postPosition`
- **Pursuit Leash:** `leashDistance = 8.5 m` (code default) / `9.0 m` (Room 4 scene)
- **Weapon Loadout:** `WeaponType.Combat_Shotgun` (`12 dmg x 6 pellets`, `220 HP`)
- **Policy Module:** `DefenseDecisionPolicy.Score(...)` (pure static utility scorer)
- **Candidate Topology:** `17` points (`1` anchor + `2` concentric `8`-point rings)
<!-- END:POLICY_INTRO -->

<!-- SECTION:STATE_MACHINE -->
<!-- SECTION:DECISION_POLICY_BASE -->
<!-- SECTION:THREAT_COVER_RANGE -->
<!-- SECTION:LEASH_BOUNDS -->
<!-- SECTION:SWITCHING_BONUS -->
<!-- SECTION:PRESSURE_RETREAT -->
<!-- SECTION:VERIFICATION_LOG -->
