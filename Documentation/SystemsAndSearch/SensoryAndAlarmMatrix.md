# Room 3 Sensory Perception & Alarm Integration Matrix

**Owner:** Sanjeevsaran J. (`IT24104155`)  
**Components:** `SearchGuardAI.cs`, `GuardPerception.cs`, `GuardPerceptionMemory.cs`, Room 3 Alarm Trap.

## 1. Visual Perception Thresholds (`SearchGuardAI`)

| Parameter | Value | Description |
| --- | --- | --- |
| `sightDistance` | `11.0 m` | Maximum line-of-sight acquisition range in Room 3 |
| `fieldOfView` | `90.0 deg` | Full horizontal vision cone (`+/- 45 deg` from guard forward) |
| `eyeHeight` | `0.6 m` | Ray origin vertical offset above guard transform |
| `obstacleMask` | `~0` (solid non-trigger colliders) | Blocks sight through racks, crates, walls, and closed doors |
| `repathInterval` | `0.25 s` | Pursuit destination update cadence while visually tracking player |

## 2. Acoustic Attenuation & Alarm Trap Integration (`GuardPerception.CanHearSound`)

- **Attenuation Formula:** `attenuationFactor = Mathf.Pow(0.55f, Mathf.Min(wallCount, 5))`
- Each non-trigger solid barrier intersected between `listenerPos` and `sound.position` reduces effective audible range by `45%`.
- Effective audibility returns `Mathf.Clamp01(1f - (dist / effectiveMaxRange))` when `dist <= effectiveMaxRange`.
- **Sound-Only Rule:** Hearing triggers investigation and hypothesis rebuilds (`0.75 s` cooldown) without granting visual aim or firing.

## 3. Comprehensive Sensory & Alarm Test Matrix

| Case ID | Stimulus Type | Distance (m) | Angle (deg) | Solid Walls | Attenuation | Expected Guard Response |
| --- | --- | --- | --- | --- | --- | --- |
| `SEN-001` | Gunshot | `2.5` | `15` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-002` | Footstep | `3.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-003` | Room3Alarm | `4.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-004` | Visual | `5.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-005` | Gunshot | `6.5` | `75` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-006` | Footstep | `7.5` | `90` | `2` | `0.303` | Footstep inaudible |
| `SEN-007` | Room3Alarm | `8.5` | `105` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-008` | Visual | `9.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-009` | Gunshot | `10.5` | `135` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-010` | Footstep | `11.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-011` | Room3Alarm | `12.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-012` | Visual | `13.5` | `0` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-013` | Gunshot | `14.5` | `15` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-014` | Footstep | `15.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-015` | Room3Alarm | `16.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-016` | Visual | `17.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-017` | Gunshot | `18.5` | `75` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-018` | Footstep | `1.5` | `90` | `2` | `0.303` | Footstep detected (eff range 2.4 m) -> Suspicion / Turn |
| `SEN-019` | Room3Alarm | `2.5` | `105` | `3` | `0.166` | Alarm heard (eff range 4.0 m) -> Build Room 3 hypotheses |
| `SEN-020` | Visual | `3.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-021` | Gunshot | `4.5` | `135` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-022` | Footstep | `5.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-023` | Room3Alarm | `6.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-024` | Visual | `7.5` | `0` | `0` | `1.000` | Immediate visual acquisition -> Pursue / Engage |
| `SEN-025` | Gunshot | `8.5` | `15` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-026` | Footstep | `9.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-027` | Room3Alarm | `10.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-028` | Visual | `11.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-029` | Gunshot | `12.5` | `75` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-030` | Footstep | `13.5` | `90` | `2` | `0.303` | Footstep inaudible |
| `SEN-031` | Room3Alarm | `14.5` | `105` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-032` | Visual | `15.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-033` | Gunshot | `16.5` | `135` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-034` | Footstep | `17.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-035` | Room3Alarm | `18.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-036` | Visual | `1.5` | `0` | `0` | `1.000` | Immediate visual acquisition -> Pursue / Engage |
| `SEN-037` | Gunshot | `2.5` | `15` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-038` | Footstep | `3.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-039` | Room3Alarm | `4.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-040` | Visual | `5.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-041` | Gunshot | `6.5` | `75` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-042` | Footstep | `7.5` | `90` | `2` | `0.303` | Footstep inaudible |
| `SEN-043` | Room3Alarm | `8.5` | `105` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-044` | Visual | `9.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-045` | Gunshot | `10.5` | `135` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-046` | Footstep | `11.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-047` | Room3Alarm | `12.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-048` | Visual | `13.5` | `0` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-049` | Gunshot | `14.5` | `15` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-050` | Footstep | `15.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-051` | Room3Alarm | `16.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-052` | Visual | `17.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-053` | Gunshot | `18.5` | `75` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-054` | Footstep | `1.5` | `90` | `2` | `0.303` | Footstep detected (eff range 2.4 m) -> Suspicion / Turn |
| `SEN-055` | Room3Alarm | `2.5` | `105` | `3` | `0.166` | Alarm heard (eff range 4.0 m) -> Build Room 3 hypotheses |
| `SEN-056` | Visual | `3.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-057` | Gunshot | `4.5` | `135` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-058` | Footstep | `5.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-059` | Room3Alarm | `6.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-060` | Visual | `7.5` | `0` | `0` | `1.000` | Immediate visual acquisition -> Pursue / Engage |
| `SEN-061` | Gunshot | `8.5` | `15` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-062` | Footstep | `9.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-063` | Room3Alarm | `10.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-064` | Visual | `11.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-065` | Gunshot | `12.5` | `75` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-066` | Footstep | `13.5` | `90` | `2` | `0.303` | Footstep inaudible |
| `SEN-067` | Room3Alarm | `14.5` | `105` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-068` | Visual | `15.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-069` | Gunshot | `16.5` | `135` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-070` | Footstep | `17.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-071` | Room3Alarm | `18.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-072` | Visual | `1.5` | `0` | `0` | `1.000` | Immediate visual acquisition -> Pursue / Engage |
| `SEN-073` | Gunshot | `2.5` | `15` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-074` | Footstep | `3.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-075` | Room3Alarm | `4.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-076` | Visual | `5.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-077` | Gunshot | `6.5` | `75` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-078` | Footstep | `7.5` | `90` | `2` | `0.303` | Footstep inaudible |
| `SEN-079` | Room3Alarm | `8.5` | `105` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-080` | Visual | `9.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-081` | Gunshot | `10.5` | `135` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-082` | Footstep | `11.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-083` | Room3Alarm | `12.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-084` | Visual | `13.5` | `0` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-085` | Gunshot | `14.5` | `15` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-086` | Footstep | `15.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-087` | Room3Alarm | `16.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-088` | Visual | `17.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-089` | Gunshot | `18.5` | `75` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-090` | Footstep | `1.5` | `90` | `2` | `0.303` | Footstep detected (eff range 2.4 m) -> Suspicion / Turn |
| `SEN-091` | Room3Alarm | `2.5` | `105` | `3` | `0.166` | Alarm heard (eff range 4.0 m) -> Build Room 3 hypotheses |
| `SEN-092` | Visual | `3.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-093` | Gunshot | `4.5` | `135` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-094` | Footstep | `5.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-095` | Room3Alarm | `6.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-096` | Visual | `7.5` | `0` | `0` | `1.000` | Immediate visual acquisition -> Pursue / Engage |
| `SEN-097` | Gunshot | `8.5` | `15` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-098` | Footstep | `9.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-099` | Room3Alarm | `10.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-100` | Visual | `11.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-101` | Gunshot | `12.5` | `75` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-102` | Footstep | `13.5` | `90` | `2` | `0.303` | Footstep inaudible |
| `SEN-103` | Room3Alarm | `14.5` | `105` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-104` | Visual | `15.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-105` | Gunshot | `16.5` | `135` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-106` | Footstep | `17.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-107` | Room3Alarm | `18.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-108` | Visual | `1.5` | `0` | `0` | `1.000` | Immediate visual acquisition -> Pursue / Engage |
| `SEN-109` | Gunshot | `2.5` | `15` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-110` | Footstep | `3.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-111` | Room3Alarm | `4.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-112` | Visual | `5.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-113` | Gunshot | `6.5` | `75` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-114` | Footstep | `7.5` | `90` | `2` | `0.303` | Footstep inaudible |
| `SEN-115` | Room3Alarm | `8.5` | `105` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-116` | Visual | `9.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-117` | Gunshot | `10.5` | `135` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-118` | Footstep | `11.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-119` | Room3Alarm | `12.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-120` | Visual | `13.5` | `0` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-121` | Gunshot | `14.5` | `15` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-122` | Footstep | `15.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-123` | Room3Alarm | `16.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-124` | Visual | `17.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-125` | Gunshot | `18.5` | `75` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-126` | Footstep | `1.5` | `90` | `2` | `0.303` | Footstep detected (eff range 2.4 m) -> Suspicion / Turn |
| `SEN-127` | Room3Alarm | `2.5` | `105` | `3` | `0.166` | Alarm heard (eff range 4.0 m) -> Build Room 3 hypotheses |
| `SEN-128` | Visual | `3.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-129` | Gunshot | `4.5` | `135` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-130` | Footstep | `5.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-131` | Room3Alarm | `6.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-132` | Visual | `7.5` | `0` | `0` | `1.000` | Immediate visual acquisition -> Pursue / Engage |
| `SEN-133` | Gunshot | `8.5` | `15` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-134` | Footstep | `9.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-135` | Room3Alarm | `10.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-136` | Visual | `11.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-137` | Gunshot | `12.5` | `75` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-138` | Footstep | `13.5` | `90` | `2` | `0.303` | Footstep inaudible |
| `SEN-139` | Room3Alarm | `14.5` | `105` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-140` | Visual | `15.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-141` | Gunshot | `16.5` | `135` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-142` | Footstep | `17.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-143` | Room3Alarm | `18.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-144` | Visual | `1.5` | `0` | `0` | `1.000` | Immediate visual acquisition -> Pursue / Engage |
| `SEN-145` | Gunshot | `2.5` | `15` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-146` | Footstep | `3.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-147` | Room3Alarm | `4.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-148` | Visual | `5.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-149` | Gunshot | `6.5` | `75` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-150` | Footstep | `7.5` | `90` | `2` | `0.303` | Footstep inaudible |
| `SEN-151` | Room3Alarm | `8.5` | `105` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-152` | Visual | `9.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-153` | Gunshot | `10.5` | `135` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-154` | Footstep | `11.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-155` | Room3Alarm | `12.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-156` | Visual | `13.5` | `0` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-157` | Gunshot | `14.5` | `15` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-158` | Footstep | `15.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-159` | Room3Alarm | `16.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-160` | Visual | `17.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-161` | Gunshot | `18.5` | `75` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-162` | Footstep | `1.5` | `90` | `2` | `0.303` | Footstep detected (eff range 2.4 m) -> Suspicion / Turn |
| `SEN-163` | Room3Alarm | `2.5` | `105` | `3` | `0.166` | Alarm heard (eff range 4.0 m) -> Build Room 3 hypotheses |
| `SEN-164` | Visual | `3.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-165` | Gunshot | `4.5` | `135` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-166` | Footstep | `5.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-167` | Room3Alarm | `6.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-168` | Visual | `7.5` | `0` | `0` | `1.000` | Immediate visual acquisition -> Pursue / Engage |
| `SEN-169` | Gunshot | `8.5` | `15` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-170` | Footstep | `9.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-171` | Room3Alarm | `10.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-172` | Visual | `11.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-173` | Gunshot | `12.5` | `75` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-174` | Footstep | `13.5` | `90` | `2` | `0.303` | Footstep inaudible |
| `SEN-175` | Room3Alarm | `14.5` | `105` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-176` | Visual | `15.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-177` | Gunshot | `16.5` | `135` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-178` | Footstep | `17.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-179` | Room3Alarm | `18.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-180` | Visual | `1.5` | `0` | `0` | `1.000` | Immediate visual acquisition -> Pursue / Engage |
| `SEN-181` | Gunshot | `2.5` | `15` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-182` | Footstep | `3.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-183` | Room3Alarm | `4.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-184` | Visual | `5.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-185` | Gunshot | `6.5` | `75` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-186` | Footstep | `7.5` | `90` | `2` | `0.303` | Footstep inaudible |
| `SEN-187` | Room3Alarm | `8.5` | `105` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-188` | Visual | `9.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-189` | Gunshot | `10.5` | `135` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-190` | Footstep | `11.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-191` | Room3Alarm | `12.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-192` | Visual | `13.5` | `0` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-193` | Gunshot | `14.5` | `15` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-194` | Footstep | `15.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-195` | Room3Alarm | `16.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-196` | Visual | `17.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-197` | Gunshot | `18.5` | `75` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-198` | Footstep | `1.5` | `90` | `2` | `0.303` | Footstep detected (eff range 2.4 m) -> Suspicion / Turn |
| `SEN-199` | Room3Alarm | `2.5` | `105` | `3` | `0.166` | Alarm heard (eff range 4.0 m) -> Build Room 3 hypotheses |
| `SEN-200` | Visual | `3.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-201` | Gunshot | `4.5` | `135` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-202` | Footstep | `5.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-203` | Room3Alarm | `6.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-204` | Visual | `7.5` | `0` | `0` | `1.000` | Immediate visual acquisition -> Pursue / Engage |
| `SEN-205` | Gunshot | `8.5` | `15` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-206` | Footstep | `9.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-207` | Room3Alarm | `10.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-208` | Visual | `11.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-209` | Gunshot | `12.5` | `75` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-210` | Footstep | `13.5` | `90` | `2` | `0.303` | Footstep inaudible |
| `SEN-211` | Room3Alarm | `14.5` | `105` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-212` | Visual | `15.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-213` | Gunshot | `16.5` | `135` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-214` | Footstep | `17.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-215` | Room3Alarm | `18.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-216` | Visual | `1.5` | `0` | `0` | `1.000` | Immediate visual acquisition -> Pursue / Engage |
| `SEN-217` | Gunshot | `2.5` | `15` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-218` | Footstep | `3.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-219` | Room3Alarm | `4.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-220` | Visual | `5.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-221` | Gunshot | `6.5` | `75` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-222` | Footstep | `7.5` | `90` | `2` | `0.303` | Footstep inaudible |
| `SEN-223` | Room3Alarm | `8.5` | `105` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-224` | Visual | `9.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-225` | Gunshot | `10.5` | `135` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-226` | Footstep | `11.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-227` | Room3Alarm | `12.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-228` | Visual | `13.5` | `0` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-229` | Gunshot | `14.5` | `15` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-230` | Footstep | `15.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-231` | Room3Alarm | `16.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-232` | Visual | `17.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-233` | Gunshot | `18.5` | `75` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-234` | Footstep | `1.5` | `90` | `2` | `0.303` | Footstep detected (eff range 2.4 m) -> Suspicion / Turn |
| `SEN-235` | Room3Alarm | `2.5` | `105` | `3` | `0.166` | Alarm heard (eff range 4.0 m) -> Build Room 3 hypotheses |
| `SEN-236` | Visual | `3.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-237` | Gunshot | `4.5` | `135` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-238` | Footstep | `5.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-239` | Room3Alarm | `6.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-240` | Visual | `7.5` | `0` | `0` | `1.000` | Immediate visual acquisition -> Pursue / Engage |
| `SEN-241` | Gunshot | `8.5` | `15` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-242` | Footstep | `9.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-243` | Room3Alarm | `10.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-244` | Visual | `11.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-245` | Gunshot | `12.5` | `75` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-246` | Footstep | `13.5` | `90` | `2` | `0.303` | Footstep inaudible |
| `SEN-247` | Room3Alarm | `14.5` | `105` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-248` | Visual | `15.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-249` | Gunshot | `16.5` | `135` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-250` | Footstep | `17.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-251` | Room3Alarm | `18.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-252` | Visual | `1.5` | `0` | `0` | `1.000` | Immediate visual acquisition -> Pursue / Engage |
| `SEN-253` | Gunshot | `2.5` | `15` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-254` | Footstep | `3.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-255` | Room3Alarm | `4.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-256` | Visual | `5.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-257` | Gunshot | `6.5` | `75` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-258` | Footstep | `7.5` | `90` | `2` | `0.303` | Footstep inaudible |
| `SEN-259` | Room3Alarm | `8.5` | `105` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-260` | Visual | `9.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-261` | Gunshot | `10.5` | `135` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-262` | Footstep | `11.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-263` | Room3Alarm | `12.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-264` | Visual | `13.5` | `0` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-265` | Gunshot | `14.5` | `15` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-266` | Footstep | `15.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-267` | Room3Alarm | `16.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-268` | Visual | `17.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-269` | Gunshot | `18.5` | `75` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-270` | Footstep | `1.5` | `90` | `2` | `0.303` | Footstep detected (eff range 2.4 m) -> Suspicion / Turn |
| `SEN-271` | Room3Alarm | `2.5` | `105` | `3` | `0.166` | Alarm heard (eff range 4.0 m) -> Build Room 3 hypotheses |
| `SEN-272` | Visual | `3.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-273` | Gunshot | `4.5` | `135` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-274` | Footstep | `5.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-275` | Room3Alarm | `6.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-276` | Visual | `7.5` | `0` | `0` | `1.000` | Immediate visual acquisition -> Pursue / Engage |
| `SEN-277` | Gunshot | `8.5` | `15` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-278` | Footstep | `9.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-279` | Room3Alarm | `10.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-280` | Visual | `11.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-281` | Gunshot | `12.5` | `75` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-282` | Footstep | `13.5` | `90` | `2` | `0.303` | Footstep inaudible |
| `SEN-283` | Room3Alarm | `14.5` | `105` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-284` | Visual | `15.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-285` | Gunshot | `16.5` | `135` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-286` | Footstep | `17.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-287` | Room3Alarm | `18.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-288` | Visual | `1.5` | `0` | `0` | `1.000` | Immediate visual acquisition -> Pursue / Engage |
| `SEN-289` | Gunshot | `2.5` | `15` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-290` | Footstep | `3.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-291` | Room3Alarm | `4.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-292` | Visual | `5.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-293` | Gunshot | `6.5` | `75` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-294` | Footstep | `7.5` | `90` | `2` | `0.303` | Footstep inaudible |
| `SEN-295` | Room3Alarm | `8.5` | `105` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-296` | Visual | `9.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-297` | Gunshot | `10.5` | `135` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-298` | Footstep | `11.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-299` | Room3Alarm | `12.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-300` | Visual | `13.5` | `0` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-301` | Gunshot | `14.5` | `15` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-302` | Footstep | `15.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-303` | Room3Alarm | `16.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-304` | Visual | `17.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-305` | Gunshot | `18.5` | `75` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-306` | Footstep | `1.5` | `90` | `2` | `0.303` | Footstep detected (eff range 2.4 m) -> Suspicion / Turn |
| `SEN-307` | Room3Alarm | `2.5` | `105` | `3` | `0.166` | Alarm heard (eff range 4.0 m) -> Build Room 3 hypotheses |
| `SEN-308` | Visual | `3.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-309` | Gunshot | `4.5` | `135` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-310` | Footstep | `5.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-311` | Room3Alarm | `6.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-312` | Visual | `7.5` | `0` | `0` | `1.000` | Immediate visual acquisition -> Pursue / Engage |
| `SEN-313` | Gunshot | `8.5` | `15` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-314` | Footstep | `9.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-315` | Room3Alarm | `10.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-316` | Visual | `11.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-317` | Gunshot | `12.5` | `75` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-318` | Footstep | `13.5` | `90` | `2` | `0.303` | Footstep inaudible |
| `SEN-319` | Room3Alarm | `14.5` | `105` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-320` | Visual | `15.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-321` | Gunshot | `16.5` | `135` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-322` | Footstep | `17.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-323` | Room3Alarm | `18.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-324` | Visual | `1.5` | `0` | `0` | `1.000` | Immediate visual acquisition -> Pursue / Engage |
| `SEN-325` | Gunshot | `2.5` | `15` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-326` | Footstep | `3.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-327` | Room3Alarm | `4.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-328` | Visual | `5.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-329` | Gunshot | `6.5` | `75` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-330` | Footstep | `7.5` | `90` | `2` | `0.303` | Footstep inaudible |
| `SEN-331` | Room3Alarm | `8.5` | `105` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-332` | Visual | `9.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-333` | Gunshot | `10.5` | `135` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-334` | Footstep | `11.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-335` | Room3Alarm | `12.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-336` | Visual | `13.5` | `0` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-337` | Gunshot | `14.5` | `15` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-338` | Footstep | `15.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-339` | Room3Alarm | `16.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-340` | Visual | `17.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-341` | Gunshot | `18.5` | `75` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-342` | Footstep | `1.5` | `90` | `2` | `0.303` | Footstep detected (eff range 2.4 m) -> Suspicion / Turn |
| `SEN-343` | Room3Alarm | `2.5` | `105` | `3` | `0.166` | Alarm heard (eff range 4.0 m) -> Build Room 3 hypotheses |
| `SEN-344` | Visual | `3.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-345` | Gunshot | `4.5` | `135` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-346` | Footstep | `5.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-347` | Room3Alarm | `6.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-348` | Visual | `7.5` | `0` | `0` | `1.000` | Immediate visual acquisition -> Pursue / Engage |
| `SEN-349` | Gunshot | `8.5` | `15` | `1` | `0.550` | Gunshot heard (eff range 11.0 m) -> Sound cooldown check (0.75s) |
| `SEN-350` | Footstep | `9.5` | `30` | `2` | `0.303` | Footstep inaudible |
| `SEN-351` | Room3Alarm | `10.5` | `45` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-352` | Visual | `11.5` | `60` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-353` | Gunshot | `12.5` | `75` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-354` | Footstep | `13.5` | `90` | `2` | `0.303` | Footstep inaudible |
| `SEN-355` | Room3Alarm | `14.5` | `105` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-356` | Visual | `15.5` | `120` | `0` | `1.000` | Outside cone or range -> Remain in current state |
| `SEN-357` | Gunshot | `16.5` | `135` | `1` | `0.550` | Gunshot out of effective range |
| `SEN-358` | Footstep | `17.5` | `150` | `2` | `0.303` | Footstep inaudible |
| `SEN-359` | Room3Alarm | `18.5` | `165` | `3` | `0.166` | Alarm attenuated below threshold |
| `SEN-360` | Visual | `1.5` | `0` | `0` | `1.000` | Immediate visual acquisition -> Pursue / Engage |

<!-- BEGIN_SENSORY_TODOS -->
## 4. Draft Sensory Calibration TODOs

- `[SEN-TODO-001]` Pending sensory calibration note #001: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-002]` Pending sensory calibration note #002: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-003]` Pending sensory calibration note #003: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-004]` Pending sensory calibration note #004: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-005]` Pending sensory calibration note #005: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-006]` Pending sensory calibration note #006: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-007]` Pending sensory calibration note #007: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-008]` Pending sensory calibration note #008: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-009]` Pending sensory calibration note #009: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-010]` Pending sensory calibration note #010: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-011]` Pending sensory calibration note #011: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-012]` Pending sensory calibration note #012: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-013]` Pending sensory calibration note #013: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-014]` Pending sensory calibration note #014: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-015]` Pending sensory calibration note #015: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-016]` Pending sensory calibration note #016: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-017]` Pending sensory calibration note #017: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-018]` Pending sensory calibration note #018: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-019]` Pending sensory calibration note #019: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-020]` Pending sensory calibration note #020: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-021]` Pending sensory calibration note #021: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-022]` Pending sensory calibration note #022: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-023]` Pending sensory calibration note #023: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-024]` Pending sensory calibration note #024: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-025]` Pending sensory calibration note #025: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-026]` Pending sensory calibration note #026: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-027]` Pending sensory calibration note #027: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-028]` Pending sensory calibration note #028: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-029]` Pending sensory calibration note #029: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-030]` Pending sensory calibration note #030: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-031]` Pending sensory calibration note #031: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-032]` Pending sensory calibration note #032: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-033]` Pending sensory calibration note #033: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-034]` Pending sensory calibration note #034: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-035]` Pending sensory calibration note #035: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-036]` Pending sensory calibration note #036: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-037]` Pending sensory calibration note #037: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-038]` Pending sensory calibration note #038: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-039]` Pending sensory calibration note #039: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-040]` Pending sensory calibration note #040: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-041]` Pending sensory calibration note #041: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-042]` Pending sensory calibration note #042: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-043]` Pending sensory calibration note #043: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-044]` Pending sensory calibration note #044: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-045]` Pending sensory calibration note #045: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-046]` Pending sensory calibration note #046: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-047]` Pending sensory calibration note #047: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-048]` Pending sensory calibration note #048: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-049]` Pending sensory calibration note #049: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-050]` Pending sensory calibration note #050: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-051]` Pending sensory calibration note #051: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-052]` Pending sensory calibration note #052: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-053]` Pending sensory calibration note #053: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-054]` Pending sensory calibration note #054: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-055]` Pending sensory calibration note #055: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-056]` Pending sensory calibration note #056: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-057]` Pending sensory calibration note #057: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-058]` Pending sensory calibration note #058: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-059]` Pending sensory calibration note #059: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-060]` Pending sensory calibration note #060: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-061]` Pending sensory calibration note #061: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-062]` Pending sensory calibration note #062: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-063]` Pending sensory calibration note #063: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-064]` Pending sensory calibration note #064: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-065]` Pending sensory calibration note #065: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-066]` Pending sensory calibration note #066: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-067]` Pending sensory calibration note #067: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-068]` Pending sensory calibration note #068: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-069]` Pending sensory calibration note #069: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-070]` Pending sensory calibration note #070: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-071]` Pending sensory calibration note #071: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-072]` Pending sensory calibration note #072: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-073]` Pending sensory calibration note #073: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-074]` Pending sensory calibration note #074: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-075]` Pending sensory calibration note #075: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-076]` Pending sensory calibration note #076: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-077]` Pending sensory calibration note #077: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-078]` Pending sensory calibration note #078: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-079]` Pending sensory calibration note #079: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-080]` Pending sensory calibration note #080: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-081]` Pending sensory calibration note #081: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-082]` Pending sensory calibration note #082: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-083]` Pending sensory calibration note #083: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-084]` Pending sensory calibration note #084: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-085]` Pending sensory calibration note #085: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-086]` Pending sensory calibration note #086: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-087]` Pending sensory calibration note #087: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-088]` Pending sensory calibration note #088: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-089]` Pending sensory calibration note #089: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-090]` Pending sensory calibration note #090: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-091]` Pending sensory calibration note #091: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-092]` Pending sensory calibration note #092: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-093]` Pending sensory calibration note #093: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-094]` Pending sensory calibration note #094: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-095]` Pending sensory calibration note #095: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-096]` Pending sensory calibration note #096: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-097]` Pending sensory calibration note #097: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
- `[SEN-TODO-098]` Pending sensory calibration note #098: cross-check Room 3 shelving collider layer masks and temporary debug gizmo colors.
<!-- END_SENSORY_TODOS -->
