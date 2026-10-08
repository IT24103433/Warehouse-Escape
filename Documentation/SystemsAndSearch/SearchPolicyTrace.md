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

- **Pre-Trace Observation `PRE-001`:** Raycast visibility matrix row 001 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-002`:** Raycast visibility matrix row 002 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-003`:** Raycast visibility matrix row 003 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-004`:** Raycast visibility matrix row 004 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-005`:** Raycast visibility matrix row 005 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-006`:** Raycast visibility matrix row 006 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-007`:** Raycast visibility matrix row 007 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-008`:** Raycast visibility matrix row 008 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-009`:** Raycast visibility matrix row 009 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-010`:** Raycast visibility matrix row 010 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-011`:** Raycast visibility matrix row 011 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-012`:** Raycast visibility matrix row 012 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-013`:** Raycast visibility matrix row 013 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-014`:** Raycast visibility matrix row 014 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-015`:** Raycast visibility matrix row 015 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-016`:** Raycast visibility matrix row 016 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-017`:** Raycast visibility matrix row 017 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-018`:** Raycast visibility matrix row 018 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-019`:** Raycast visibility matrix row 019 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-020`:** Raycast visibility matrix row 020 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-021`:** Raycast visibility matrix row 021 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-022`:** Raycast visibility matrix row 022 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-023`:** Raycast visibility matrix row 023 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-024`:** Raycast visibility matrix row 024 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-025`:** Raycast visibility matrix row 025 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-026`:** Raycast visibility matrix row 026 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-027`:** Raycast visibility matrix row 027 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-028`:** Raycast visibility matrix row 028 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-029`:** Raycast visibility matrix row 029 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-030`:** Raycast visibility matrix row 030 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-031`:** Raycast visibility matrix row 031 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-032`:** Raycast visibility matrix row 032 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-033`:** Raycast visibility matrix row 033 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-034`:** Raycast visibility matrix row 034 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-035`:** Raycast visibility matrix row 035 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-036`:** Raycast visibility matrix row 036 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-037`:** Raycast visibility matrix row 037 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-038`:** Raycast visibility matrix row 038 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-039`:** Raycast visibility matrix row 039 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-040`:** Raycast visibility matrix row 040 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-041`:** Raycast visibility matrix row 041 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-042`:** Raycast visibility matrix row 042 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-043`:** Raycast visibility matrix row 043 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-044`:** Raycast visibility matrix row 044 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-045`:** Raycast visibility matrix row 045 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-046`:** Raycast visibility matrix row 046 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-047`:** Raycast visibility matrix row 047 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-048`:** Raycast visibility matrix row 048 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-049`:** Raycast visibility matrix row 049 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-050`:** Raycast visibility matrix row 050 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-051`:** Raycast visibility matrix row 051 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-052`:** Raycast visibility matrix row 052 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-053`:** Raycast visibility matrix row 053 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-054`:** Raycast visibility matrix row 054 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-055`:** Raycast visibility matrix row 055 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-056`:** Raycast visibility matrix row 056 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-057`:** Raycast visibility matrix row 057 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-058`:** Raycast visibility matrix row 058 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-059`:** Raycast visibility matrix row 059 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-060`:** Raycast visibility matrix row 060 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-061`:** Raycast visibility matrix row 061 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-062`:** Raycast visibility matrix row 062 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-063`:** Raycast visibility matrix row 063 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-064`:** Raycast visibility matrix row 064 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-065`:** Raycast visibility matrix row 065 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-066`:** Raycast visibility matrix row 066 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-067`:** Raycast visibility matrix row 067 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-068`:** Raycast visibility matrix row 068 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-069`:** Raycast visibility matrix row 069 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-070`:** Raycast visibility matrix row 070 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-071`:** Raycast visibility matrix row 071 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-072`:** Raycast visibility matrix row 072 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-073`:** Raycast visibility matrix row 073 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-074`:** Raycast visibility matrix row 074 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-075`:** Raycast visibility matrix row 075 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-076`:** Raycast visibility matrix row 076 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-077`:** Raycast visibility matrix row 077 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-078`:** Raycast visibility matrix row 078 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-079`:** Raycast visibility matrix row 079 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-080`:** Raycast visibility matrix row 080 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-081`:** Raycast visibility matrix row 081 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-082`:** Raycast visibility matrix row 082 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-083`:** Raycast visibility matrix row 083 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-084`:** Raycast visibility matrix row 084 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-085`:** Raycast visibility matrix row 085 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-086`:** Raycast visibility matrix row 086 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-087`:** Raycast visibility matrix row 087 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-088`:** Raycast visibility matrix row 088 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-089`:** Raycast visibility matrix row 089 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-090`:** Raycast visibility matrix row 090 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-091`:** Raycast visibility matrix row 091 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-092`:** Raycast visibility matrix row 092 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-093`:** Raycast visibility matrix row 093 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-094`:** Raycast visibility matrix row 094 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-095`:** Raycast visibility matrix row 095 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-096`:** Raycast visibility matrix row 096 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-097`:** Raycast visibility matrix row 097 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-098`:** Raycast visibility matrix row 098 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-099`:** Raycast visibility matrix row 099 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-100`:** Raycast visibility matrix row 100 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-101`:** Raycast visibility matrix row 101 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-102`:** Raycast visibility matrix row 102 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-103`:** Raycast visibility matrix row 103 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-104`:** Raycast visibility matrix row 104 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-105`:** Raycast visibility matrix row 105 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-106`:** Raycast visibility matrix row 106 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-107`:** Raycast visibility matrix row 107 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-108`:** Raycast visibility matrix row 108 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-109`:** Raycast visibility matrix row 109 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-110`:** Raycast visibility matrix row 110 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-111`:** Raycast visibility matrix row 111 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-112`:** Raycast visibility matrix row 112 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-113`:** Raycast visibility matrix row 113 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-114`:** Raycast visibility matrix row 114 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-115`:** Raycast visibility matrix row 115 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-116`:** Raycast visibility matrix row 116 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-117`:** Raycast visibility matrix row 117 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-118`:** Raycast visibility matrix row 118 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-119`:** Raycast visibility matrix row 119 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-120`:** Raycast visibility matrix row 120 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-121`:** Raycast visibility matrix row 121 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-122`:** Raycast visibility matrix row 122 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-123`:** Raycast visibility matrix row 123 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-124`:** Raycast visibility matrix row 124 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-125`:** Raycast visibility matrix row 125 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-126`:** Raycast visibility matrix row 126 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-127`:** Raycast visibility matrix row 127 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-128`:** Raycast visibility matrix row 128 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-129`:** Raycast visibility matrix row 129 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-130`:** Raycast visibility matrix row 130 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-131`:** Raycast visibility matrix row 131 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-132`:** Raycast visibility matrix row 132 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-133`:** Raycast visibility matrix row 133 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-134`:** Raycast visibility matrix row 134 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-135`:** Raycast visibility matrix row 135 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-136`:** Raycast visibility matrix row 136 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-137`:** Raycast visibility matrix row 137 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-138`:** Raycast visibility matrix row 138 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-139`:** Raycast visibility matrix row 139 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-140`:** Raycast visibility matrix row 140 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-141`:** Raycast visibility matrix row 141 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-142`:** Raycast visibility matrix row 142 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-143`:** Raycast visibility matrix row 143 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-144`:** Raycast visibility matrix row 144 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-145`:** Raycast visibility matrix row 145 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-146`:** Raycast visibility matrix row 146 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-147`:** Raycast visibility matrix row 147 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-148`:** Raycast visibility matrix row 148 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-149`:** Raycast visibility matrix row 149 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-150`:** Raycast visibility matrix row 150 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-151`:** Raycast visibility matrix row 151 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-152`:** Raycast visibility matrix row 152 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-153`:** Raycast visibility matrix row 153 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-154`:** Raycast visibility matrix row 154 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-155`:** Raycast visibility matrix row 155 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-156`:** Raycast visibility matrix row 156 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-157`:** Raycast visibility matrix row 157 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-158`:** Raycast visibility matrix row 158 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).
- **Pre-Trace Observation `PRE-159`:** Raycast visibility matrix row 159 evaluated against Room 3 shelving colliders (`6.0 m` sweep radius, horizontal ray buffer reused without heap allocation).

<!-- BEGIN_LINEAR_DECAY_TRACE -->
## 3. Draft Linear-Decay Search Trace Logs (Legacy Prototype)

> **Legacy Note:** Initial draft used a linear subtraction step (`belief -= 0.15`) per empty inspection.

### 3.1 Legacy Linear Iteration #01
- **Selected Target:** `C02` at distance `2.00 m`
- **Observable Belief Before Sweep:** `0.8750`
- **Computed Utility Score:** `0.7000`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-01-1]` Candidate `C03` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-01-2]` Candidate `C04` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-01-3]` Candidate `C05` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-01-4]` Candidate `C06` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.2 Legacy Linear Iteration #02
- **Selected Target:** `C03` at distance `2.80 m`
- **Observable Belief Before Sweep:** `0.8500`
- **Computed Utility Score:** `0.6800`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-02-1]` Candidate `C04` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-02-2]` Candidate `C05` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-02-3]` Candidate `C06` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-02-4]` Candidate `C07` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.3 Legacy Linear Iteration #03
- **Selected Target:** `C04` at distance `3.60 m`
- **Observable Belief Before Sweep:** `0.8250`
- **Computed Utility Score:** `0.6600`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-03-1]` Candidate `C05` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-03-2]` Candidate `C06` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-03-3]` Candidate `C07` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-03-4]` Candidate `C08` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.4 Legacy Linear Iteration #04
- **Selected Target:** `C05` at distance `4.40 m`
- **Observable Belief Before Sweep:** `0.8000`
- **Computed Utility Score:** `0.6400`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-04-1]` Candidate `C06` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-04-2]` Candidate `C07` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-04-3]` Candidate `C08` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-04-4]` Candidate `C09` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.5 Legacy Linear Iteration #05
- **Selected Target:** `C06` at distance `5.20 m`
- **Observable Belief Before Sweep:** `0.7750`
- **Computed Utility Score:** `0.6200`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-05-1]` Candidate `C07` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-05-2]` Candidate `C08` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-05-3]` Candidate `C09` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-05-4]` Candidate `C10` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.6 Legacy Linear Iteration #06
- **Selected Target:** `C07` at distance `1.20 m`
- **Observable Belief Before Sweep:** `0.7500`
- **Computed Utility Score:** `0.6000`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-06-1]` Candidate `C08` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-06-2]` Candidate `C09` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-06-3]` Candidate `C10` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-06-4]` Candidate `C11` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.7 Legacy Linear Iteration #07
- **Selected Target:** `C08` at distance `2.00 m`
- **Observable Belief Before Sweep:** `0.7250`
- **Computed Utility Score:** `0.5800`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-07-1]` Candidate `C09` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-07-2]` Candidate `C10` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-07-3]` Candidate `C11` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-07-4]` Candidate `C12` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.8 Legacy Linear Iteration #08
- **Selected Target:** `C09` at distance `2.80 m`
- **Observable Belief Before Sweep:** `0.7000`
- **Computed Utility Score:** `0.5600`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-08-1]` Candidate `C10` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-08-2]` Candidate `C11` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-08-3]` Candidate `C12` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-08-4]` Candidate `C13` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.9 Legacy Linear Iteration #09
- **Selected Target:** `C10` at distance `3.60 m`
- **Observable Belief Before Sweep:** `0.6750`
- **Computed Utility Score:** `0.5400`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-09-1]` Candidate `C11` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-09-2]` Candidate `C12` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-09-3]` Candidate `C13` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-09-4]` Candidate `C14` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.10 Legacy Linear Iteration #10
- **Selected Target:** `C11` at distance `4.40 m`
- **Observable Belief Before Sweep:** `0.6500`
- **Computed Utility Score:** `0.5200`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-10-1]` Candidate `C12` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-10-2]` Candidate `C13` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-10-3]` Candidate `C14` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-10-4]` Candidate `C15` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.11 Legacy Linear Iteration #11
- **Selected Target:** `C12` at distance `5.20 m`
- **Observable Belief Before Sweep:** `0.6250`
- **Computed Utility Score:** `0.5000`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-11-1]` Candidate `C13` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-11-2]` Candidate `C14` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-11-3]` Candidate `C15` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-11-4]` Candidate `C16` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.12 Legacy Linear Iteration #12
- **Selected Target:** `C13` at distance `1.20 m`
- **Observable Belief Before Sweep:** `0.6000`
- **Computed Utility Score:** `0.4800`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-12-1]` Candidate `C14` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-12-2]` Candidate `C15` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-12-3]` Candidate `C16` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-12-4]` Candidate `C17` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.13 Legacy Linear Iteration #13
- **Selected Target:** `C14` at distance `2.00 m`
- **Observable Belief Before Sweep:** `0.5750`
- **Computed Utility Score:** `0.4600`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-13-1]` Candidate `C15` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-13-2]` Candidate `C16` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-13-3]` Candidate `C17` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-13-4]` Candidate `C01` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.14 Legacy Linear Iteration #14
- **Selected Target:** `C15` at distance `2.80 m`
- **Observable Belief Before Sweep:** `0.5500`
- **Computed Utility Score:** `0.4400`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-14-1]` Candidate `C16` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-14-2]` Candidate `C17` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-14-3]` Candidate `C01` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-14-4]` Candidate `C02` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.15 Legacy Linear Iteration #15
- **Selected Target:** `C16` at distance `3.60 m`
- **Observable Belief Before Sweep:** `0.5250`
- **Computed Utility Score:** `0.4200`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-15-1]` Candidate `C17` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-15-2]` Candidate `C01` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-15-3]` Candidate `C02` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-15-4]` Candidate `C03` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.16 Legacy Linear Iteration #16
- **Selected Target:** `C17` at distance `4.40 m`
- **Observable Belief Before Sweep:** `0.5000`
- **Computed Utility Score:** `0.4000`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-16-1]` Candidate `C01` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-16-2]` Candidate `C02` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-16-3]` Candidate `C03` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-16-4]` Candidate `C04` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.17 Legacy Linear Iteration #17
- **Selected Target:** `C01` at distance `5.20 m`
- **Observable Belief Before Sweep:** `0.4750`
- **Computed Utility Score:** `0.3800`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-17-1]` Candidate `C02` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-17-2]` Candidate `C03` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-17-3]` Candidate `C04` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-17-4]` Candidate `C05` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.18 Legacy Linear Iteration #18
- **Selected Target:** `C02` at distance `1.20 m`
- **Observable Belief Before Sweep:** `0.4500`
- **Computed Utility Score:** `0.3600`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-18-1]` Candidate `C03` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-18-2]` Candidate `C04` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-18-3]` Candidate `C05` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-18-4]` Candidate `C06` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.19 Legacy Linear Iteration #19
- **Selected Target:** `C03` at distance `2.00 m`
- **Observable Belief Before Sweep:** `0.4250`
- **Computed Utility Score:** `0.3400`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-19-1]` Candidate `C04` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-19-2]` Candidate `C05` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-19-3]` Candidate `C06` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-19-4]` Candidate `C07` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.20 Legacy Linear Iteration #20
- **Selected Target:** `C04` at distance `2.80 m`
- **Observable Belief Before Sweep:** `0.4000`
- **Computed Utility Score:** `0.3200`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-20-1]` Candidate `C05` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-20-2]` Candidate `C06` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-20-3]` Candidate `C07` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-20-4]` Candidate `C08` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.21 Legacy Linear Iteration #21
- **Selected Target:** `C05` at distance `3.60 m`
- **Observable Belief Before Sweep:** `0.3750`
- **Computed Utility Score:** `0.3000`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-21-1]` Candidate `C06` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-21-2]` Candidate `C07` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-21-3]` Candidate `C08` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-21-4]` Candidate `C09` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.22 Legacy Linear Iteration #22
- **Selected Target:** `C06` at distance `4.40 m`
- **Observable Belief Before Sweep:** `0.3500`
- **Computed Utility Score:** `0.2800`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-22-1]` Candidate `C07` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-22-2]` Candidate `C08` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-22-3]` Candidate `C09` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-22-4]` Candidate `C10` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.23 Legacy Linear Iteration #23
- **Selected Target:** `C07` at distance `5.20 m`
- **Observable Belief Before Sweep:** `0.3250`
- **Computed Utility Score:** `0.2600`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-23-1]` Candidate `C08` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-23-2]` Candidate `C09` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-23-3]` Candidate `C10` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-23-4]` Candidate `C11` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.24 Legacy Linear Iteration #24
- **Selected Target:** `C08` at distance `1.20 m`
- **Observable Belief Before Sweep:** `0.3000`
- **Computed Utility Score:** `0.2400`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-24-1]` Candidate `C09` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-24-2]` Candidate `C10` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-24-3]` Candidate `C11` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-24-4]` Candidate `C12` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.25 Legacy Linear Iteration #25
- **Selected Target:** `C09` at distance `2.00 m`
- **Observable Belief Before Sweep:** `0.2750`
- **Computed Utility Score:** `0.2200`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-25-1]` Candidate `C10` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-25-2]` Candidate `C11` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-25-3]` Candidate `C12` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-25-4]` Candidate `C13` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.26 Legacy Linear Iteration #26
- **Selected Target:** `C10` at distance `2.80 m`
- **Observable Belief Before Sweep:** `0.2500`
- **Computed Utility Score:** `0.2000`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-26-1]` Candidate `C11` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-26-2]` Candidate `C12` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-26-3]` Candidate `C13` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-26-4]` Candidate `C14` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.27 Legacy Linear Iteration #27
- **Selected Target:** `C11` at distance `3.60 m`
- **Observable Belief Before Sweep:** `0.2250`
- **Computed Utility Score:** `0.1800`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-27-1]` Candidate `C12` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-27-2]` Candidate `C13` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-27-3]` Candidate `C14` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-27-4]` Candidate `C15` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.28 Legacy Linear Iteration #28
- **Selected Target:** `C12` at distance `4.40 m`
- **Observable Belief Before Sweep:** `0.2000`
- **Computed Utility Score:** `0.1600`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-28-1]` Candidate `C13` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-28-2]` Candidate `C14` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-28-3]` Candidate `C15` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-28-4]` Candidate `C16` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.29 Legacy Linear Iteration #29
- **Selected Target:** `C13` at distance `5.20 m`
- **Observable Belief Before Sweep:** `0.1750`
- **Computed Utility Score:** `0.1400`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-29-1]` Candidate `C14` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-29-2]` Candidate `C15` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-29-3]` Candidate `C16` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-29-4]` Candidate `C17` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
### 3.30 Legacy Linear Iteration #30
- **Selected Target:** `C14` at distance `1.20 m`
- **Observable Belief Before Sweep:** `0.1500`
- **Computed Utility Score:** `0.1200`
- **Linear Decay Rule:** Subtracted fixed `0.15` from inspected nodes; clamped at `0.00`.
- **Legacy Residual Sum:** Renormalized across remaining uninspected slots (causes false certainty shift).
  - `[LIN-30-1]` Candidate `C15` linear-decay snapshot: prior=`0.0800`, path=`2.70 m`, linear_post=`0.0500`.
  - `[LIN-30-2]` Candidate `C16` linear-decay snapshot: prior=`0.0400`, path=`3.40 m`, linear_post=`0.0100`.
  - `[LIN-30-3]` Candidate `C17` linear-decay snapshot: prior=`0.0267`, path=`4.10 m`, linear_post=`0.0000`.
  - `[LIN-30-4]` Candidate `C01` linear-decay snapshot: prior=`0.0200`, path=`4.80 m`, linear_post=`0.0000`.
<!-- END_LINEAR_DECAY_TRACE -->
