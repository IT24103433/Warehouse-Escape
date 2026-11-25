# Headshots and original-pitch shotgun recording

Player bullets that intersect a guard's head region now deal twice their usual damage. Body damage is unchanged. Shotgun pellets are evaluated independently: at 12 base damage each, a head pellet deals 24. The hit marker turns gold for an accepted headshot. Damage rejection, death state, safe-room protection and muzzle/cover validation still apply.

The head region is derived from the animated head mesh's height and width and projected onto the existing capsule hit surface. It is a practical hit-zone approximation, not triangle-perfect head-mesh collision. No extra character colliders or AI changes are introduced. A lateral head-height miss cannot earn the multiplier. Guards do not gain random critical damage against the player.

Shotguns now use the first original Mossberg recording in `N_26P.wav` at pitch 1.0, preserving a 2.2-second natural report tail. Both player and guard shotguns use the same recorded resource. The audio is from the CC0 Free Firearm Sound Library; see AudioSources.md and Documentation/Tools/prepare_original_shotgun.py. No commercial-game audio is copied.

Verification exercises genuine PlayerGun firing for body hits, head hits, zero-spread controlled shotgun pellets and intervening cover; it also checks gold feedback and both playback paths. Existing spread remains enabled during normal gameplay.

Validation: 58/58 PlayMode tests passed in the full run. Windows release build succeeded with zero errors and one existing unused-field warning. Standalone fullscreen startup exited normally without exceptions.
