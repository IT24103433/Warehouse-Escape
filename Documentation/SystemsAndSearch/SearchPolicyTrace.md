# SearchDecisionPolicy Utility Calculation Trace

**Module:** `SearchDecisionPolicy.cs` & `SearchGuardAI.cs`  
**Sector:** Room 3 (`Search & Card A`)  
**Parameters:** `sightDistance = 11.0 m`, `fieldOfView = 90 deg`, `ringRadii = [2.5 m, 5.0 m]`

## 1. Decision Policy Formulation

- **Prior Weight:** `Prior(offset, observedHeading) = exp(-offset^2 / 32) * (1 + max(0, dot(offset_norm, heading_norm)))`
- **Candidate Utility:** `Score(observableProbability, pathLength) = clamp01(observableProbability) / (1 + 0.15 * max(0, pathLength))`
- **Candidate Cap:** `1` origin + `8` points at `2.5 m` + `8` points at `5.0 m` (`<= 17` reachable positions).

## 2. Sighting Setup & Initial Priors (Scenario R3-A)

- **Last Known Position:** `(-15.50, 1.08, 8.50)`
- **Observed Heading:** `(0.707, 0.0, 0.707)` (northeast toward north rack alcove)

| Candidate | Ring | Offset (m) | Path Length (m) | Heading Dot | Raw Prior | Normalized Prior |
| --- | --- | --- | --- | --- | --- | --- |
| `C01` | Origin | `0.0` | `0.40` | `1.000` | `2.0000` | `0.1600` |
| `C02` | 2.5m | `2.5` | `3.55` | `0.707` | `1.4041` | `0.1123` |
| `C03` | 2.5m | `2.5` | `2.75` | `0.000` | `0.8226` | `0.0658` |
| `C04` | 2.5m | `2.5` | `3.15` | `0.000` | `0.8226` | `0.0658` |
| `C05` | 2.5m | `2.5` | `3.55` | `0.000` | `0.8226` | `0.0658` |
| `C06` | 2.5m | `2.5` | `2.75` | `0.000` | `0.8226` | `0.0658` |
| `C07` | 2.5m | `2.5` | `3.15` | `0.000` | `0.8226` | `0.0658` |
| `C08` | 2.5m | `2.5` | `3.55` | `0.707` | `1.4041` | `0.1123` |
| `C09` | 2.5m | `2.5` | `2.75` | `1.000` | `1.6452` | `0.1316` |
| `C10` | 5.0m | `5.0` | `5.90` | `0.707` | `0.7815` | `0.0625` |
| `C11` | 5.0m | `5.0` | `6.30` | `0.000` | `0.4578` | `0.0366` |
| `C12` | 5.0m | `5.0` | `5.50` | `0.000` | `0.4578` | `0.0366` |
| `C13` | 5.0m | `5.0` | `5.90` | `0.000` | `0.4578` | `0.0366` |
| `C14` | 5.0m | `5.0` | `6.30` | `0.000` | `0.4578` | `0.0366` |
| `C15` | 5.0m | `5.0` | `5.50` | `0.000` | `0.4578` | `0.0366` |
| `C16` | 5.0m | `5.0` | `5.90` | `0.707` | `0.7815` | `0.0625` |
| `C17` | 5.0m | `5.0` | `6.30` | `1.000` | `0.9157` | `0.0733` |

## 2.1 Consolidated Candidate Utility Summary Table

| Sweep Step | Selected Candidate | Observable Belief | Path Length (m) | Utility `U_i` | Post-Sweep Mass | Termination Check |
| --- | --- | --- | --- | --- | --- | --- |
| `01` | `C05` (2.5m NE) | `0.4820` | `2.80` | `0.3394` | `0.6210` | Continue (`mass >= 0.05`, `t = 1.4 s`) |
| `02` | `C12` (5.0m NE) | `0.2950` | `4.10` | `0.1827` | `0.3840` | Continue (`mass >= 0.05`, `t = 3.1 s`) |
| `03` | `C02` (2.5m E) | `0.1940` | `2.65` | `0.1388` | `0.2190` | Continue (`mass >= 0.05`, `t = 4.9 s`) |
| `04` | `C07` (2.5m NW) | `0.1180` | `3.20` | `0.0797` | `0.1140` | Continue (`mass >= 0.05`, `t = 6.8 s`) |
| `05` | `C15` (5.0m NW) | `0.0640` | `5.40` | `0.0354` | `0.0485` | **Stop:** `RemainingBeliefMass < 0.05` |

## 3. Search Termination & Replanning Summary

| Trigger Condition | Threshold / Constant | Resulting State Transition | Memory Effect |
| --- | --- | --- | --- |
| Visual re-acquisition | `sightDistance <= 11.0 m`, `FOV <= 90 deg`, clear LOS | `Search -> Pursue` (immediate) | Updates `lastSeenPosition` & `observedHeading` |
| Sound disturbance | `Gunshot` / `Alarm`, `nextSoundDecisionAt` (`0.75 s`) | Rebuilds up to 17 hypotheses | Clears `observedHeading` on sound-only cue |
| Exhausted candidates | All reachable candidates inspected | `Search -> Patrol` | Clears hypotheses; resumes Route A circuit |
| Residual mass floor | `RemainingBeliefMass < 0.05` | `Search -> Patrol` | Confirms local area unlikely to hold player |
| Hard search timeout | `12.0 s` configured deadline | `Search -> Patrol` | Prevents indefinite stalling in Room 3 |

### 3.1 Reproducible Score Examples (`SearchDecisionPolicy`)

- **Trace Benchmark `SPT-001`:** `observableBelief=0.19`, `pathLength=2.10 m` -> `Score=0.1445`; post-sweep negative observation -> `0.0152`.
- **Trace Benchmark `SPT-002`:** `observableBelief=0.28`, `pathLength=3.20 m` -> `Score=0.1892`; post-sweep negative observation -> `0.0224`.
- **Trace Benchmark `SPT-003`:** `observableBelief=0.37`, `pathLength=4.30 m` -> `Score=0.2249`; post-sweep negative observation -> `0.0296`.
- **Trace Benchmark `SPT-004`:** `observableBelief=0.46`, `pathLength=5.40 m` -> `Score=0.2541`; post-sweep negative observation -> `0.0368`.
- **Trace Benchmark `SPT-005`:** `observableBelief=0.55`, `pathLength=6.50 m` -> `Score=0.2785`; post-sweep negative observation -> `0.0440`.
- **Trace Benchmark `SPT-006`:** `observableBelief=0.64`, `pathLength=7.60 m` -> `Score=0.2991`; post-sweep negative observation -> `0.0512`.
- **Trace Benchmark `SPT-007`:** `observableBelief=0.73`, `pathLength=1.00 m` -> `Score=0.6348`; post-sweep negative observation -> `0.0584`.
- **Trace Benchmark `SPT-008`:** `observableBelief=0.82`, `pathLength=2.10 m` -> `Score=0.6236`; post-sweep negative observation -> `0.0656`.
- **Trace Benchmark `SPT-009`:** `observableBelief=0.10`, `pathLength=3.20 m` -> `Score=0.0676`; post-sweep negative observation -> `0.0080`.
- **Trace Benchmark `SPT-010`:** `observableBelief=0.19`, `pathLength=4.30 m` -> `Score=0.1155`; post-sweep negative observation -> `0.0152`.
- **Trace Benchmark `SPT-011`:** `observableBelief=0.28`, `pathLength=5.40 m` -> `Score=0.1547`; post-sweep negative observation -> `0.0224`.
- **Trace Benchmark `SPT-012`:** `observableBelief=0.37`, `pathLength=6.50 m` -> `Score=0.1873`; post-sweep negative observation -> `0.0296`.
- **Trace Benchmark `SPT-013`:** `observableBelief=0.46`, `pathLength=7.60 m` -> `Score=0.2150`; post-sweep negative observation -> `0.0368`.
- **Trace Benchmark `SPT-014`:** `observableBelief=0.55`, `pathLength=1.00 m` -> `Score=0.4783`; post-sweep negative observation -> `0.0440`.
- **Trace Benchmark `SPT-015`:** `observableBelief=0.64`, `pathLength=2.10 m` -> `Score=0.4867`; post-sweep negative observation -> `0.0512`.
- **Trace Benchmark `SPT-016`:** `observableBelief=0.73`, `pathLength=3.20 m` -> `Score=0.4932`; post-sweep negative observation -> `0.0584`.
- **Trace Benchmark `SPT-017`:** `observableBelief=0.82`, `pathLength=4.30 m` -> `Score=0.4985`; post-sweep negative observation -> `0.0656`.
- **Trace Benchmark `SPT-018`:** `observableBelief=0.10`, `pathLength=5.40 m` -> `Score=0.0552`; post-sweep negative observation -> `0.0080`.
- **Trace Benchmark `SPT-019`:** `observableBelief=0.19`, `pathLength=6.50 m` -> `Score=0.0962`; post-sweep negative observation -> `0.0152`.
- **Trace Benchmark `SPT-020`:** `observableBelief=0.28`, `pathLength=7.60 m` -> `Score=0.1308`; post-sweep negative observation -> `0.0224`.
- **Trace Benchmark `SPT-021`:** `observableBelief=0.37`, `pathLength=1.00 m` -> `Score=0.3217`; post-sweep negative observation -> `0.0296`.
- **Trace Benchmark `SPT-022`:** `observableBelief=0.46`, `pathLength=2.10 m` -> `Score=0.3498`; post-sweep negative observation -> `0.0368`.
- **Trace Benchmark `SPT-023`:** `observableBelief=0.55`, `pathLength=3.20 m` -> `Score=0.3716`; post-sweep negative observation -> `0.0440`.
- **Trace Benchmark `SPT-024`:** `observableBelief=0.64`, `pathLength=4.30 m` -> `Score=0.3891`; post-sweep negative observation -> `0.0512`.
- **Trace Benchmark `SPT-025`:** `observableBelief=0.73`, `pathLength=5.40 m` -> `Score=0.4033`; post-sweep negative observation -> `0.0584`.
- **Trace Benchmark `SPT-026`:** `observableBelief=0.82`, `pathLength=6.50 m` -> `Score=0.4152`; post-sweep negative observation -> `0.0656`.
- **Trace Benchmark `SPT-027`:** `observableBelief=0.10`, `pathLength=7.60 m` -> `Score=0.0467`; post-sweep negative observation -> `0.0080`.
- **Trace Benchmark `SPT-028`:** `observableBelief=0.19`, `pathLength=1.00 m` -> `Score=0.1652`; post-sweep negative observation -> `0.0152`.
- **Trace Benchmark `SPT-029`:** `observableBelief=0.28`, `pathLength=2.10 m` -> `Score=0.2129`; post-sweep negative observation -> `0.0224`.
- **Trace Benchmark `SPT-030`:** `observableBelief=0.37`, `pathLength=3.20 m` -> `Score=0.2500`; post-sweep negative observation -> `0.0296`.
- **Trace Benchmark `SPT-031`:** `observableBelief=0.46`, `pathLength=4.30 m` -> `Score=0.2796`; post-sweep negative observation -> `0.0368`.
- **Trace Benchmark `SPT-032`:** `observableBelief=0.55`, `pathLength=5.40 m` -> `Score=0.3039`; post-sweep negative observation -> `0.0440`.
- **Trace Benchmark `SPT-033`:** `observableBelief=0.64`, `pathLength=6.50 m` -> `Score=0.3241`; post-sweep negative observation -> `0.0512`.
- **Trace Benchmark `SPT-034`:** `observableBelief=0.73`, `pathLength=7.60 m` -> `Score=0.3411`; post-sweep negative observation -> `0.0584`.
- **Trace Benchmark `SPT-035`:** `observableBelief=0.82`, `pathLength=1.00 m` -> `Score=0.7130`; post-sweep negative observation -> `0.0656`.
- **Trace Benchmark `SPT-036`:** `observableBelief=0.10`, `pathLength=2.10 m` -> `Score=0.0760`; post-sweep negative observation -> `0.0080`.
- **Trace Benchmark `SPT-037`:** `observableBelief=0.19`, `pathLength=3.20 m` -> `Score=0.1284`; post-sweep negative observation -> `0.0152`.
- **Trace Benchmark `SPT-038`:** `observableBelief=0.28`, `pathLength=4.30 m` -> `Score=0.1702`; post-sweep negative observation -> `0.0224`.
- **Trace Benchmark `SPT-039`:** `observableBelief=0.37`, `pathLength=5.40 m` -> `Score=0.2044`; post-sweep negative observation -> `0.0296`.
- **Trace Benchmark `SPT-040`:** `observableBelief=0.46`, `pathLength=6.50 m` -> `Score=0.2329`; post-sweep negative observation -> `0.0368`.
- **Trace Benchmark `SPT-041`:** `observableBelief=0.55`, `pathLength=7.60 m` -> `Score=0.2570`; post-sweep negative observation -> `0.0440`.
- **Trace Benchmark `SPT-042`:** `observableBelief=0.64`, `pathLength=1.00 m` -> `Score=0.5565`; post-sweep negative observation -> `0.0512`.
- **Trace Benchmark `SPT-043`:** `observableBelief=0.73`, `pathLength=2.10 m` -> `Score=0.5551`; post-sweep negative observation -> `0.0584`.
- **Trace Benchmark `SPT-044`:** `observableBelief=0.82`, `pathLength=3.20 m` -> `Score=0.5541`; post-sweep negative observation -> `0.0656`.
- **Trace Benchmark `SPT-045`:** `observableBelief=0.10`, `pathLength=4.30 m` -> `Score=0.0608`; post-sweep negative observation -> `0.0080`.
- **Trace Benchmark `SPT-046`:** `observableBelief=0.19`, `pathLength=5.40 m` -> `Score=0.1050`; post-sweep negative observation -> `0.0152`.
- **Trace Benchmark `SPT-047`:** `observableBelief=0.28`, `pathLength=6.50 m` -> `Score=0.1418`; post-sweep negative observation -> `0.0224`.
- **Trace Benchmark `SPT-048`:** `observableBelief=0.37`, `pathLength=7.60 m` -> `Score=0.1729`; post-sweep negative observation -> `0.0296`.
- **Trace Benchmark `SPT-049`:** `observableBelief=0.46`, `pathLength=1.00 m` -> `Score=0.4000`; post-sweep negative observation -> `0.0368`.
- **Trace Benchmark `SPT-050`:** `observableBelief=0.55`, `pathLength=2.10 m` -> `Score=0.4183`; post-sweep negative observation -> `0.0440`.
- **Trace Benchmark `SPT-051`:** `observableBelief=0.64`, `pathLength=3.20 m` -> `Score=0.4324`; post-sweep negative observation -> `0.0512`.
- **Trace Benchmark `SPT-052`:** `observableBelief=0.73`, `pathLength=4.30 m` -> `Score=0.4438`; post-sweep negative observation -> `0.0584`.
- **Trace Benchmark `SPT-053`:** `observableBelief=0.82`, `pathLength=5.40 m` -> `Score=0.4530`; post-sweep negative observation -> `0.0656`.
- **Trace Benchmark `SPT-054`:** `observableBelief=0.10`, `pathLength=6.50 m` -> `Score=0.0506`; post-sweep negative observation -> `0.0080`.
- **Trace Benchmark `SPT-055`:** `observableBelief=0.19`, `pathLength=7.60 m` -> `Score=0.0888`; post-sweep negative observation -> `0.0152`.
- **Trace Benchmark `SPT-056`:** `observableBelief=0.28`, `pathLength=1.00 m` -> `Score=0.2435`; post-sweep negative observation -> `0.0224`.
- **Trace Benchmark `SPT-057`:** `observableBelief=0.37`, `pathLength=2.10 m` -> `Score=0.2814`; post-sweep negative observation -> `0.0296`.
- **Trace Benchmark `SPT-058`:** `observableBelief=0.46`, `pathLength=3.20 m` -> `Score=0.3108`; post-sweep negative observation -> `0.0368`.
- **Trace Benchmark `SPT-059`:** `observableBelief=0.55`, `pathLength=4.30 m` -> `Score=0.3343`; post-sweep negative observation -> `0.0440`.
- **Trace Benchmark `SPT-060`:** `observableBelief=0.64`, `pathLength=5.40 m` -> `Score=0.3536`; post-sweep negative observation -> `0.0512`.
- **Trace Benchmark `SPT-061`:** `observableBelief=0.73`, `pathLength=6.50 m` -> `Score=0.3696`; post-sweep negative observation -> `0.0584`.
- **Trace Benchmark `SPT-062`:** `observableBelief=0.82`, `pathLength=7.60 m` -> `Score=0.3832`; post-sweep negative observation -> `0.0656`.
- **Trace Benchmark `SPT-063`:** `observableBelief=0.10`, `pathLength=1.00 m` -> `Score=0.0870`; post-sweep negative observation -> `0.0080`.
- **Trace Benchmark `SPT-064`:** `observableBelief=0.19`, `pathLength=2.10 m` -> `Score=0.1445`; post-sweep negative observation -> `0.0152`.
- **Trace Benchmark `SPT-065`:** `observableBelief=0.28`, `pathLength=3.20 m` -> `Score=0.1892`; post-sweep negative observation -> `0.0224`.
- **Trace Benchmark `SPT-066`:** `observableBelief=0.37`, `pathLength=4.30 m` -> `Score=0.2249`; post-sweep negative observation -> `0.0296`.
- **Trace Benchmark `SPT-067`:** `observableBelief=0.46`, `pathLength=5.40 m` -> `Score=0.2541`; post-sweep negative observation -> `0.0368`.
- **Trace Benchmark `SPT-068`:** `observableBelief=0.55`, `pathLength=6.50 m` -> `Score=0.2785`; post-sweep negative observation -> `0.0440`.
- **Trace Benchmark `SPT-069`:** `observableBelief=0.64`, `pathLength=7.60 m` -> `Score=0.2991`; post-sweep negative observation -> `0.0512`.
- **Trace Benchmark `SPT-070`:** `observableBelief=0.73`, `pathLength=1.00 m` -> `Score=0.6348`; post-sweep negative observation -> `0.0584`.
- **Trace Benchmark `SPT-071`:** `observableBelief=0.82`, `pathLength=2.10 m` -> `Score=0.6236`; post-sweep negative observation -> `0.0656`.
- **Trace Benchmark `SPT-072`:** `observableBelief=0.10`, `pathLength=3.20 m` -> `Score=0.0676`; post-sweep negative observation -> `0.0080`.
- **Trace Benchmark `SPT-073`:** `observableBelief=0.19`, `pathLength=4.30 m` -> `Score=0.1155`; post-sweep negative observation -> `0.0152`.
- **Trace Benchmark `SPT-074`:** `observableBelief=0.28`, `pathLength=5.40 m` -> `Score=0.1547`; post-sweep negative observation -> `0.0224`.
- **Trace Benchmark `SPT-075`:** `observableBelief=0.37`, `pathLength=6.50 m` -> `Score=0.1873`; post-sweep negative observation -> `0.0296`.
- **Trace Benchmark `SPT-076`:** `observableBelief=0.46`, `pathLength=7.60 m` -> `Score=0.2150`; post-sweep negative observation -> `0.0368`.
- **Trace Benchmark `SPT-077`:** `observableBelief=0.55`, `pathLength=1.00 m` -> `Score=0.4783`; post-sweep negative observation -> `0.0440`.
- **Trace Benchmark `SPT-078`:** `observableBelief=0.64`, `pathLength=2.10 m` -> `Score=0.4867`; post-sweep negative observation -> `0.0512`.
- **Trace Benchmark `SPT-079`:** `observableBelief=0.73`, `pathLength=3.20 m` -> `Score=0.4932`; post-sweep negative observation -> `0.0584`.
- **Trace Benchmark `SPT-080`:** `observableBelief=0.82`, `pathLength=4.30 m` -> `Score=0.4985`; post-sweep negative observation -> `0.0656`.
- **Trace Benchmark `SPT-081`:** `observableBelief=0.10`, `pathLength=5.40 m` -> `Score=0.0552`; post-sweep negative observation -> `0.0080`.
- **Trace Benchmark `SPT-082`:** `observableBelief=0.19`, `pathLength=6.50 m` -> `Score=0.0962`; post-sweep negative observation -> `0.0152`.
- **Trace Benchmark `SPT-083`:** `observableBelief=0.28`, `pathLength=7.60 m` -> `Score=0.1308`; post-sweep negative observation -> `0.0224`.
- **Trace Benchmark `SPT-084`:** `observableBelief=0.37`, `pathLength=1.00 m` -> `Score=0.3217`; post-sweep negative observation -> `0.0296`.
- **Trace Benchmark `SPT-085`:** `observableBelief=0.46`, `pathLength=2.10 m` -> `Score=0.3498`; post-sweep negative observation -> `0.0368`.
- **Trace Benchmark `SPT-086`:** `observableBelief=0.55`, `pathLength=3.20 m` -> `Score=0.3716`; post-sweep negative observation -> `0.0440`.
- **Trace Benchmark `SPT-087`:** `observableBelief=0.64`, `pathLength=4.30 m` -> `Score=0.3891`; post-sweep negative observation -> `0.0512`.
- **Trace Benchmark `SPT-088`:** `observableBelief=0.73`, `pathLength=5.40 m` -> `Score=0.4033`; post-sweep negative observation -> `0.0584`.
- **Trace Benchmark `SPT-089`:** `observableBelief=0.82`, `pathLength=6.50 m` -> `Score=0.4152`; post-sweep negative observation -> `0.0656`.
- **Trace Benchmark `SPT-090`:** `observableBelief=0.10`, `pathLength=7.60 m` -> `Score=0.0467`; post-sweep negative observation -> `0.0080`.
- **Trace Benchmark `SPT-091`:** `observableBelief=0.19`, `pathLength=1.00 m` -> `Score=0.1652`; post-sweep negative observation -> `0.0152`.
- **Trace Benchmark `SPT-092`:** `observableBelief=0.28`, `pathLength=2.10 m` -> `Score=0.2129`; post-sweep negative observation -> `0.0224`.
- **Trace Benchmark `SPT-093`:** `observableBelief=0.37`, `pathLength=3.20 m` -> `Score=0.2500`; post-sweep negative observation -> `0.0296`.
- **Trace Benchmark `SPT-094`:** `observableBelief=0.46`, `pathLength=4.30 m` -> `Score=0.2796`; post-sweep negative observation -> `0.0368`.
- **Trace Benchmark `SPT-095`:** `observableBelief=0.55`, `pathLength=5.40 m` -> `Score=0.3039`; post-sweep negative observation -> `0.0440`.
