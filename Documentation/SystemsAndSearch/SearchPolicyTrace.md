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

<!-- BEGIN_RAW_NEG_EVIDENCE_DUMP -->
## 3. Negative-Evidence Discount (`0.08`) & Search Termination Trace

Replaced naive linear decay with `SearchDecisionPolicy.NegativeObservation(prior) => prior * 0.08f`.
Residual belief mass (`RemainingBeliefMass`) is **not** renormalized after an empty inspection so that
total remaining support shrinks naturally toward the `0.05` termination threshold or `12.0 s` search deadline.

### 3.1 Step-by-Step Negative-Evidence Iterations

#### Iteration 01 — Inspect Candidate `C01`
- **Pre-Sweep Mass:** `1.0000` | **Observable Belief:** `0.3818` | **Path:** `2.25 m`
- **Utility `Score`:** `0.2855` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.6688` (unrenormalized; stop check `< 0.05`: `false`)
- **Elapsed Search Timer:** `0.85 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 02 — Inspect Candidate `C02`
- **Pre-Sweep Mass:** `0.6688` | **Observable Belief:** `0.2341` | **Path:** `3.00 m`
- **Utility `Score`:** `0.1614` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.4473` (unrenormalized; stop check `< 0.05`: `false`)
- **Elapsed Search Timer:** `1.70 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 03 — Inspect Candidate `C03`
- **Pre-Sweep Mass:** `0.4473` | **Observable Belief:** `0.1445` | **Path:** `3.75 m`
- **Utility `Score`:** `0.0925` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.2992` (unrenormalized; stop check `< 0.05`: `false`)
- **Elapsed Search Timer:** `2.55 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 04 — Inspect Candidate `C04`
- **Pre-Sweep Mass:** `0.2992` | **Observable Belief:** `0.0898` | **Path:** `4.50 m`
- **Utility `Score`:** `0.0536` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.2001` (unrenormalized; stop check `< 0.05`: `false`)
- **Elapsed Search Timer:** `3.40 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 05 — Inspect Candidate `C05`
- **Pre-Sweep Mass:** `0.2001` | **Observable Belief:** `0.0840` | **Path:** `5.25 m`
- **Utility `Score`:** `0.0470` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.1338` (unrenormalized; stop check `< 0.05`: `false`)
- **Elapsed Search Timer:** `4.25 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 06 — Inspect Candidate `C06`
- **Pre-Sweep Mass:** `0.1338` | **Observable Belief:** `0.0511` | **Path:** `6.00 m`
- **Utility `Score`:** `0.0269` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0895` (unrenormalized; stop check `< 0.05`: `false`)
- **Elapsed Search Timer:** `5.10 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 07 — Inspect Candidate `C07`
- **Pre-Sweep Mass:** `0.0895` | **Observable Belief:** `0.0313` | **Path:** `1.50 m`
- **Utility `Score`:** `0.0256` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0599` (unrenormalized; stop check `< 0.05`: `false`)
- **Elapsed Search Timer:** `5.95 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 08 — Inspect Candidate `C08`
- **Pre-Sweep Mass:** `0.0599` | **Observable Belief:** `0.0194` | **Path:** `2.25 m`
- **Utility `Score`:** `0.0145` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0401` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `6.80 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 09 — Inspect Candidate `C09`
- **Pre-Sweep Mass:** `0.0401` | **Observable Belief:** `0.0120` | **Path:** `3.00 m`
- **Utility `Score`:** `0.0083` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0268` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `7.65 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 10 — Inspect Candidate `C10`
- **Pre-Sweep Mass:** `0.0268` | **Observable Belief:** `0.0120` | **Path:** `3.75 m`
- **Utility `Score`:** `0.0077` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `8.50 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 11 — Inspect Candidate `C11`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `4.50 m`
- **Utility `Score`:** `0.0072` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `9.35 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 12 — Inspect Candidate `C12`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `5.25 m`
- **Utility `Score`:** `0.0067` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `10.20 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 13 — Inspect Candidate `C13`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `6.00 m`
- **Utility `Score`:** `0.0063` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `11.05 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 14 — Inspect Candidate `C14`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `1.50 m`
- **Utility `Score`:** `0.0098` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `11.90 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 15 — Inspect Candidate `C15`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `2.25 m`
- **Utility `Score`:** `0.0090` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 16 — Inspect Candidate `C16`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `3.00 m`
- **Utility `Score`:** `0.0083` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 17 — Inspect Candidate `C17`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `3.75 m`
- **Utility `Score`:** `0.0077` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 18 — Inspect Candidate `C01`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `4.50 m`
- **Utility `Score`:** `0.0072` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 19 — Inspect Candidate `C02`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `5.25 m`
- **Utility `Score`:** `0.0067` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 20 — Inspect Candidate `C03`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `6.00 m`
- **Utility `Score`:** `0.0063` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 21 — Inspect Candidate `C04`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `1.50 m`
- **Utility `Score`:** `0.0098` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 22 — Inspect Candidate `C05`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `2.25 m`
- **Utility `Score`:** `0.0090` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 23 — Inspect Candidate `C06`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `3.00 m`
- **Utility `Score`:** `0.0083` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 24 — Inspect Candidate `C07`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `3.75 m`
- **Utility `Score`:** `0.0077` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 25 — Inspect Candidate `C08`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `4.50 m`
- **Utility `Score`:** `0.0072` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 26 — Inspect Candidate `C09`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `5.25 m`
- **Utility `Score`:** `0.0067` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 27 — Inspect Candidate `C10`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `6.00 m`
- **Utility `Score`:** `0.0063` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 28 — Inspect Candidate `C11`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `1.50 m`
- **Utility `Score`:** `0.0098` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 29 — Inspect Candidate `C12`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `2.25 m`
- **Utility `Score`:** `0.0090` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 30 — Inspect Candidate `C13`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `3.00 m`
- **Utility `Score`:** `0.0083` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 31 — Inspect Candidate `C14`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `3.75 m`
- **Utility `Score`:** `0.0077` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 32 — Inspect Candidate `C15`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `4.50 m`
- **Utility `Score`:** `0.0072` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 33 — Inspect Candidate `C16`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `5.25 m`
- **Utility `Score`:** `0.0067` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 34 — Inspect Candidate `C17`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `6.00 m`
- **Utility `Score`:** `0.0063` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 35 — Inspect Candidate `C01`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `1.50 m`
- **Utility `Score`:** `0.0098` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 36 — Inspect Candidate `C02`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `2.25 m`
- **Utility `Score`:** `0.0090` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 37 — Inspect Candidate `C03`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `3.00 m`
- **Utility `Score`:** `0.0083` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 38 — Inspect Candidate `C04`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `3.75 m`
- **Utility `Score`:** `0.0077` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 39 — Inspect Candidate `C05`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `4.50 m`
- **Utility `Score`:** `0.0072` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 40 — Inspect Candidate `C06`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `5.25 m`
- **Utility `Score`:** `0.0067` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 41 — Inspect Candidate `C07`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `6.00 m`
- **Utility `Score`:** `0.0063` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 42 — Inspect Candidate `C08`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `1.50 m`
- **Utility `Score`:** `0.0098` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 43 — Inspect Candidate `C09`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `2.25 m`
- **Utility `Score`:** `0.0090` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

#### Iteration 44 — Inspect Candidate `C10`
- **Pre-Sweep Mass:** `0.0250` | **Observable Belief:** `0.0120` | **Path:** `3.00 m`
- **Utility `Score`:** `0.0083` (`observableBelief / (1 + 0.15 * pathLength)`)
- **Discount Applied:** `NegativeObservation(p) = p * 0.08f` on all observable hypotheses (`<= 6.0 m`, clear LOS)
- **Post-Sweep Residual Mass:** `0.0250` (unrenormalized; stop check `< 0.05`: `true`)
- **Elapsed Search Timer:** `12.00 s / 12.00 s` | **Sound Rebuild Cooldown:** `0.75 s`

### 3.2 Termination & Re-Detection Transition Checks

- `[NEG-CHK-001]` Verify negative-evidence check #001: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-002]` Verify negative-evidence check #002: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-003]` Verify negative-evidence check #003: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-004]` Verify negative-evidence check #004: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-005]` Verify negative-evidence check #005: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-006]` Verify negative-evidence check #006: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-007]` Verify negative-evidence check #007: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-008]` Verify negative-evidence check #008: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-009]` Verify negative-evidence check #009: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-010]` Verify negative-evidence check #010: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-011]` Verify negative-evidence check #011: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-012]` Verify negative-evidence check #012: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-013]` Verify negative-evidence check #013: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-014]` Verify negative-evidence check #014: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-015]` Verify negative-evidence check #015: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-016]` Verify negative-evidence check #016: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-017]` Verify negative-evidence check #017: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-018]` Verify negative-evidence check #018: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-019]` Verify negative-evidence check #019: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-020]` Verify negative-evidence check #020: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-021]` Verify negative-evidence check #021: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-022]` Verify negative-evidence check #022: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-023]` Verify negative-evidence check #023: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-024]` Verify negative-evidence check #024: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-025]` Verify negative-evidence check #025: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-026]` Verify negative-evidence check #026: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-027]` Verify negative-evidence check #027: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-028]` Verify negative-evidence check #028: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-029]` Verify negative-evidence check #029: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-030]` Verify negative-evidence check #030: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-031]` Verify negative-evidence check #031: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-032]` Verify negative-evidence check #032: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-033]` Verify negative-evidence check #033: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-034]` Verify negative-evidence check #034: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-035]` Verify negative-evidence check #035: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-036]` Verify negative-evidence check #036: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-037]` Verify negative-evidence check #037: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-038]` Verify negative-evidence check #038: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-039]` Verify negative-evidence check #039: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-040]` Verify negative-evidence check #040: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-041]` Verify negative-evidence check #041: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-042]` Verify negative-evidence check #042: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-043]` Verify negative-evidence check #043: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-044]` Verify negative-evidence check #044: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-045]` Verify negative-evidence check #045: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-046]` Verify negative-evidence check #046: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-047]` Verify negative-evidence check #047: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-048]` Verify negative-evidence check #048: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-049]` Verify negative-evidence check #049: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-050]` Verify negative-evidence check #050: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-051]` Verify negative-evidence check #051: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-052]` Verify negative-evidence check #052: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-053]` Verify negative-evidence check #053: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-054]` Verify negative-evidence check #054: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-055]` Verify negative-evidence check #055: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-056]` Verify negative-evidence check #056: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-057]` Verify negative-evidence check #057: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-058]` Verify negative-evidence check #058: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-059]` Verify negative-evidence check #059: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-060]` Verify negative-evidence check #060: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-061]` Verify negative-evidence check #061: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-062]` Verify negative-evidence check #062: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-063]` Verify negative-evidence check #063: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-064]` Verify negative-evidence check #064: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-065]` Verify negative-evidence check #065: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-066]` Verify negative-evidence check #066: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-067]` Verify negative-evidence check #067: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-068]` Verify negative-evidence check #068: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-069]` Verify negative-evidence check #069: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-070]` Verify negative-evidence check #070: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-071]` Verify negative-evidence check #071: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-072]` Verify negative-evidence check #072: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-073]` Verify negative-evidence check #073: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-074]` Verify negative-evidence check #074: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-075]` Verify negative-evidence check #075: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-076]` Verify negative-evidence check #076: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-077]` Verify negative-evidence check #077: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-078]` Verify negative-evidence check #078: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-079]` Verify negative-evidence check #079: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-080]` Verify negative-evidence check #080: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-081]` Verify negative-evidence check #081: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-082]` Verify negative-evidence check #082: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-083]` Verify negative-evidence check #083: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-084]` Verify negative-evidence check #084: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
- `[NEG-CHK-085]` Verify negative-evidence check #085: residual mass decay under `0.08` multiplier, 12-second deadline cutoff, `0.05` mass floor, and immediate `Search -> Pursue` override on visual contact.
<!-- END_RAW_NEG_EVIDENCE_DUMP -->

