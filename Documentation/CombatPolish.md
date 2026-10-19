# Combat and visual polish

The five-room structure remains: safe start, introductory guard, choice of two
guarded card routes, then the final boss room. Either card opens the final exit.

| Encounter | Health | Sight | Aim preparation | Weapon | Burst |
| --- | ---: | ---: | ---: | --- | ---: |
| Varun, Room 2 | 60 | 16 m | 0.45 s | UMP | 2 |
| Sanjeevsaran, Room 3 | 90 | 18 m | 0.32 s | AK-47 | 3 |
| Aqqil, Room 4 | 110 | 18 m | 0.30 s | Shotgun | 1 |
| Jadavan, final room | 180 | 22 m | 0.22 s | SCAR | 4 |

The branch encounters offer different challenges: searching rifle fire versus
a durable defender with dangerous close-range shotgun fire. The boss has the
highest health, longest sight range, fastest reaction, and largest bursts.
Reloads and cover remain openings for the player.

All guards use their own field of view, occlusion and room limits before aiming
or firing. Each shot in a burst rechecks confirmed vision. Gunshot hearing
provides an investigation location; it cannot identify an unseen player or
permit blind firing. Interrupted bursts clear their active state so guards can
resume combat when they reacquire the player. Room 1 remains protected.

The player retains an MP5, finite magazines with unlimited reserves, weapon
pickup prompts, the crosshair, health display, minimap and retreat healing.
Recorded firearm and reload source credits are in [AudioSources.md](AudioSources.md).
Blender sources and exported FBX models remain under `Assets/Meshes`.

`Warehouse/Finalize Combat Difficulty` applies these settings and saves the
scene normally. Ordinary gameplay never invokes the development capture tool.
Launching a development build with `-capture-combat-polish` creates four staged
1280x720 offscreen URP screenshots under `scratch/combat-polish`, then exits normally.
These inspection captures freeze guards; route playability is checked separately
by the physical walkthrough tests. Screen-space overlay HUD is excluded from
the offscreen capture. The guard face fill is subdued to preserve facial shading;
the card's front and back UV orientation is corrected in its Blender source and FBX.

## Verification, 19 October 2026

- Full PlayMode run: 39 of 41 passed. Two tests used the previous enemy health
  values. Their expectations were updated for the intentional difficulty change.
- Targeted recheck: both remaining tests passed. All 41 checks are verified across
  the full run and recheck, including physical escape routes A and B, safe-room
  healing, weapon pickup/reload, occlusion, and sound-only investigation.
- Windows development build completed with zero errors. One existing compiler
  warning reports the unused `GuardPerceptionMemory.maxHearingRange` field.
- Rendered visual inspection is separate from route simulation; it does not
  establish human playthrough quality or a measured frame-rate target.

URP lighting and baked reflections remain the target for integrated graphics.
This pass does not enable hardware ray tracing or switch the rendering pipeline.
