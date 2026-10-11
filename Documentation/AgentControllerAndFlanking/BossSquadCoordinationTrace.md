# Room 5 Boss Squad Coordination Trace (`BossSquadMember.cs`)

**Components:** `BossSquadMember.cs`, `FlankingGuardAI.cs`, `GuardShooting.cs`  
**Units:** `Guard_Jadavan_Boss` (`SquadSlot = 0` -> `PreferredSide = FlankSide.Left`), `Guard_Jadavan_Boss_Wingman` (`SquadSlot = 1` -> `PreferredSide = FlankSide.Right`).

## 1. Opposite-Side Flanking Preference

- `BossSquadMember.PreferredSide` returns `FlankSide.None` when `Partner == null`, `FlankSide.Left` for even slots (`slot % 2 == 0`), and `FlankSide.Right` for odd slots.
- In `FlankingGuardAI.TrySelectFlankTarget()`, candidates matching `squadMember.PreferredSide` are tracked in `preferredSquadCandidate` (`bestSquadScore`).
- If a viable candidate exists on the guard's assigned side, `selectedCandidate = preferredSquadCandidate`; otherwise, the guard retains the best overall reachable fallback so it never stalls when one side aisle is blocked.

- `[SIDE-PREF-001]` Squad slot `1` (`PreferredSide = Right`) flanking check #001: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-002]` Squad slot `0` (`PreferredSide = Left`) flanking check #002: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-003]` Squad slot `1` (`PreferredSide = Right`) flanking check #003: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-004]` Squad slot `0` (`PreferredSide = Left`) flanking check #004: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-005]` Squad slot `1` (`PreferredSide = Right`) flanking check #005: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-006]` Squad slot `0` (`PreferredSide = Left`) flanking check #006: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-007]` Squad slot `1` (`PreferredSide = Right`) flanking check #007: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-008]` Squad slot `0` (`PreferredSide = Left`) flanking check #008: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-009]` Squad slot `1` (`PreferredSide = Right`) flanking check #009: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-010]` Squad slot `0` (`PreferredSide = Left`) flanking check #010: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-011]` Squad slot `1` (`PreferredSide = Right`) flanking check #011: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-012]` Squad slot `0` (`PreferredSide = Left`) flanking check #012: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-013]` Squad slot `1` (`PreferredSide = Right`) flanking check #013: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-014]` Squad slot `0` (`PreferredSide = Left`) flanking check #014: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-015]` Squad slot `1` (`PreferredSide = Right`) flanking check #015: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-016]` Squad slot `0` (`PreferredSide = Left`) flanking check #016: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-017]` Squad slot `1` (`PreferredSide = Right`) flanking check #017: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-018]` Squad slot `0` (`PreferredSide = Left`) flanking check #018: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-019]` Squad slot `1` (`PreferredSide = Right`) flanking check #019: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-020]` Squad slot `0` (`PreferredSide = Left`) flanking check #020: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-021]` Squad slot `1` (`PreferredSide = Right`) flanking check #021: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-022]` Squad slot `0` (`PreferredSide = Left`) flanking check #022: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-023]` Squad slot `1` (`PreferredSide = Right`) flanking check #023: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-024]` Squad slot `0` (`PreferredSide = Left`) flanking check #024: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-025]` Squad slot `1` (`PreferredSide = Right`) flanking check #025: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-026]` Squad slot `0` (`PreferredSide = Left`) flanking check #026: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-027]` Squad slot `1` (`PreferredSide = Right`) flanking check #027: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-028]` Squad slot `0` (`PreferredSide = Left`) flanking check #028: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-029]` Squad slot `1` (`PreferredSide = Right`) flanking check #029: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-030]` Squad slot `0` (`PreferredSide = Left`) flanking check #030: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-031]` Squad slot `1` (`PreferredSide = Right`) flanking check #031: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-032]` Squad slot `0` (`PreferredSide = Left`) flanking check #032: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-033]` Squad slot `1` (`PreferredSide = Right`) flanking check #033: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-034]` Squad slot `0` (`PreferredSide = Left`) flanking check #034: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-035]` Squad slot `1` (`PreferredSide = Right`) flanking check #035: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-036]` Squad slot `0` (`PreferredSide = Left`) flanking check #036: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-037]` Squad slot `1` (`PreferredSide = Right`) flanking check #037: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-038]` Squad slot `0` (`PreferredSide = Left`) flanking check #038: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-039]` Squad slot `1` (`PreferredSide = Right`) flanking check #039: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-040]` Squad slot `0` (`PreferredSide = Left`) flanking check #040: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-041]` Squad slot `1` (`PreferredSide = Right`) flanking check #041: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-042]` Squad slot `0` (`PreferredSide = Left`) flanking check #042: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-043]` Squad slot `1` (`PreferredSide = Right`) flanking check #043: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-044]` Squad slot `0` (`PreferredSide = Left`) flanking check #044: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-045]` Squad slot `1` (`PreferredSide = Right`) flanking check #045: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-046]` Squad slot `0` (`PreferredSide = Left`) flanking check #046: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-047]` Squad slot `1` (`PreferredSide = Right`) flanking check #047: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-048]` Squad slot `0` (`PreferredSide = Left`) flanking check #048: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-049]` Squad slot `1` (`PreferredSide = Right`) flanking check #049: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-050]` Squad slot `0` (`PreferredSide = Left`) flanking check #050: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-051]` Squad slot `1` (`PreferredSide = Right`) flanking check #051: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-052]` Squad slot `0` (`PreferredSide = Left`) flanking check #052: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-053]` Squad slot `1` (`PreferredSide = Right`) flanking check #053: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-054]` Squad slot `0` (`PreferredSide = Left`) flanking check #054: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-055]` Squad slot `1` (`PreferredSide = Right`) flanking check #055: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-056]` Squad slot `0` (`PreferredSide = Left`) flanking check #056: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-057]` Squad slot `1` (`PreferredSide = Right`) flanking check #057: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-058]` Squad slot `0` (`PreferredSide = Left`) flanking check #058: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-059]` Squad slot `1` (`PreferredSide = Right`) flanking check #059: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-060]` Squad slot `0` (`PreferredSide = Left`) flanking check #060: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-061]` Squad slot `1` (`PreferredSide = Right`) flanking check #061: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-062]` Squad slot `0` (`PreferredSide = Left`) flanking check #062: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-063]` Squad slot `1` (`PreferredSide = Right`) flanking check #063: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-064]` Squad slot `0` (`PreferredSide = Left`) flanking check #064: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-065]` Squad slot `1` (`PreferredSide = Right`) flanking check #065: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-066]` Squad slot `0` (`PreferredSide = Left`) flanking check #066: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-067]` Squad slot `1` (`PreferredSide = Right`) flanking check #067: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-068]` Squad slot `0` (`PreferredSide = Left`) flanking check #068: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-069]` Squad slot `1` (`PreferredSide = Right`) flanking check #069: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-070]` Squad slot `0` (`PreferredSide = Left`) flanking check #070: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-071]` Squad slot `1` (`PreferredSide = Right`) flanking check #071: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-072]` Squad slot `0` (`PreferredSide = Left`) flanking check #072: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-073]` Squad slot `1` (`PreferredSide = Right`) flanking check #073: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-074]` Squad slot `0` (`PreferredSide = Left`) flanking check #074: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-075]` Squad slot `1` (`PreferredSide = Right`) flanking check #075: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-076]` Squad slot `0` (`PreferredSide = Left`) flanking check #076: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-077]` Squad slot `1` (`PreferredSide = Right`) flanking check #077: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-078]` Squad slot `0` (`PreferredSide = Left`) flanking check #078: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-079]` Squad slot `1` (`PreferredSide = Right`) flanking check #079: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-080]` Squad slot `0` (`PreferredSide = Left`) flanking check #080: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-081]` Squad slot `1` (`PreferredSide = Right`) flanking check #081: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-082]` Squad slot `0` (`PreferredSide = Left`) flanking check #082: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-083]` Squad slot `1` (`PreferredSide = Right`) flanking check #083: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-084]` Squad slot `0` (`PreferredSide = Left`) flanking check #084: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-085]` Squad slot `1` (`PreferredSide = Right`) flanking check #085: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-086]` Squad slot `0` (`PreferredSide = Left`) flanking check #086: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-087]` Squad slot `1` (`PreferredSide = Right`) flanking check #087: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-088]` Squad slot `0` (`PreferredSide = Left`) flanking check #088: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-089]` Squad slot `1` (`PreferredSide = Right`) flanking check #089: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-090]` Squad slot `0` (`PreferredSide = Left`) flanking check #090: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-091]` Squad slot `1` (`PreferredSide = Right`) flanking check #091: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-092]` Squad slot `0` (`PreferredSide = Left`) flanking check #092: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-093]` Squad slot `1` (`PreferredSide = Right`) flanking check #093: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-094]` Squad slot `0` (`PreferredSide = Left`) flanking check #094: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-095]` Squad slot `1` (`PreferredSide = Right`) flanking check #095: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-096]` Squad slot `0` (`PreferredSide = Left`) flanking check #096: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-097]` Squad slot `1` (`PreferredSide = Right`) flanking check #097: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-098]` Squad slot `0` (`PreferredSide = Left`) flanking check #098: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-099]` Squad slot `1` (`PreferredSide = Right`) flanking check #099: selected lowest-cost `Right` aisle route when reachable; verified opposite-side pincer geometry.
- `[SIDE-PREF-100]` Squad slot `0` (`PreferredSide = Left`) flanking check #100: selected lowest-cost `Left` aisle route when reachable; verified opposite-side pincer geometry.

<!-- BEGIN_SINGLE_GUARD_DRAFTS -->
## 2. Early Single-Guard Room 5 Prototype Traces (To Refactor)

- `[SINGLE-DRAFT-001]` Legacy single-boss Room 5 trace #001: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-002]` Legacy single-boss Room 5 trace #002: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-003]` Legacy single-boss Room 5 trace #003: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-004]` Legacy single-boss Room 5 trace #004: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-005]` Legacy single-boss Room 5 trace #005: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-006]` Legacy single-boss Room 5 trace #006: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-007]` Legacy single-boss Room 5 trace #007: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-008]` Legacy single-boss Room 5 trace #008: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-009]` Legacy single-boss Room 5 trace #009: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-010]` Legacy single-boss Room 5 trace #010: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-011]` Legacy single-boss Room 5 trace #011: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-012]` Legacy single-boss Room 5 trace #012: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-013]` Legacy single-boss Room 5 trace #013: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-014]` Legacy single-boss Room 5 trace #014: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-015]` Legacy single-boss Room 5 trace #015: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-016]` Legacy single-boss Room 5 trace #016: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-017]` Legacy single-boss Room 5 trace #017: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-018]` Legacy single-boss Room 5 trace #018: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-019]` Legacy single-boss Room 5 trace #019: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-020]` Legacy single-boss Room 5 trace #020: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-021]` Legacy single-boss Room 5 trace #021: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-022]` Legacy single-boss Room 5 trace #022: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-023]` Legacy single-boss Room 5 trace #023: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-024]` Legacy single-boss Room 5 trace #024: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-025]` Legacy single-boss Room 5 trace #025: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-026]` Legacy single-boss Room 5 trace #026: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-027]` Legacy single-boss Room 5 trace #027: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-028]` Legacy single-boss Room 5 trace #028: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-029]` Legacy single-boss Room 5 trace #029: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-030]` Legacy single-boss Room 5 trace #030: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-031]` Legacy single-boss Room 5 trace #031: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-032]` Legacy single-boss Room 5 trace #032: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-033]` Legacy single-boss Room 5 trace #033: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-034]` Legacy single-boss Room 5 trace #034: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-035]` Legacy single-boss Room 5 trace #035: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-036]` Legacy single-boss Room 5 trace #036: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-037]` Legacy single-boss Room 5 trace #037: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-038]` Legacy single-boss Room 5 trace #038: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-039]` Legacy single-boss Room 5 trace #039: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-040]` Legacy single-boss Room 5 trace #040: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-041]` Legacy single-boss Room 5 trace #041: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-042]` Legacy single-boss Room 5 trace #042: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-043]` Legacy single-boss Room 5 trace #043: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-044]` Legacy single-boss Room 5 trace #044: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-045]` Legacy single-boss Room 5 trace #045: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-046]` Legacy single-boss Room 5 trace #046: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-047]` Legacy single-boss Room 5 trace #047: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-048]` Legacy single-boss Room 5 trace #048: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-049]` Legacy single-boss Room 5 trace #049: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-050]` Legacy single-boss Room 5 trace #050: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-051]` Legacy single-boss Room 5 trace #051: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-052]` Legacy single-boss Room 5 trace #052: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-053]` Legacy single-boss Room 5 trace #053: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-054]` Legacy single-boss Room 5 trace #054: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-055]` Legacy single-boss Room 5 trace #055: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-056]` Legacy single-boss Room 5 trace #056: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-057]` Legacy single-boss Room 5 trace #057: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-058]` Legacy single-boss Room 5 trace #058: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-059]` Legacy single-boss Room 5 trace #059: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-060]` Legacy single-boss Room 5 trace #060: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-061]` Legacy single-boss Room 5 trace #061: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-062]` Legacy single-boss Room 5 trace #062: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-063]` Legacy single-boss Room 5 trace #063: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-064]` Legacy single-boss Room 5 trace #064: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-065]` Legacy single-boss Room 5 trace #065: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-066]` Legacy single-boss Room 5 trace #066: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-067]` Legacy single-boss Room 5 trace #067: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-068]` Legacy single-boss Room 5 trace #068: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-069]` Legacy single-boss Room 5 trace #069: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-070]` Legacy single-boss Room 5 trace #070: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-071]` Legacy single-boss Room 5 trace #071: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-072]` Legacy single-boss Room 5 trace #072: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-073]` Legacy single-boss Room 5 trace #073: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-074]` Legacy single-boss Room 5 trace #074: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-075]` Legacy single-boss Room 5 trace #075: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-076]` Legacy single-boss Room 5 trace #076: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-077]` Legacy single-boss Room 5 trace #077: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-078]` Legacy single-boss Room 5 trace #078: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-079]` Legacy single-boss Room 5 trace #079: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-080]` Legacy single-boss Room 5 trace #080: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-081]` Legacy single-boss Room 5 trace #081: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-082]` Legacy single-boss Room 5 trace #082: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-083]` Legacy single-boss Room 5 trace #083: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-084]` Legacy single-boss Room 5 trace #084: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-085]` Legacy single-boss Room 5 trace #085: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-086]` Legacy single-boss Room 5 trace #086: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-087]` Legacy single-boss Room 5 trace #087: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-088]` Legacy single-boss Room 5 trace #088: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-089]` Legacy single-boss Room 5 trace #089: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-090]` Legacy single-boss Room 5 trace #090: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-091]` Legacy single-boss Room 5 trace #091: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-092]` Legacy single-boss Room 5 trace #092: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-093]` Legacy single-boss Room 5 trace #093: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-094]` Legacy single-boss Room 5 trace #094: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-095]` Legacy single-boss Room 5 trace #095: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-096]` Legacy single-boss Room 5 trace #096: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-097]` Legacy single-boss Room 5 trace #097: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-098]` Legacy single-boss Room 5 trace #098: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-099]` Legacy single-boss Room 5 trace #099: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-100]` Legacy single-boss Room 5 trace #100: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-101]` Legacy single-boss Room 5 trace #101: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-102]` Legacy single-boss Room 5 trace #102: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-103]` Legacy single-boss Room 5 trace #103: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-104]` Legacy single-boss Room 5 trace #104: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-105]` Legacy single-boss Room 5 trace #105: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-106]` Legacy single-boss Room 5 trace #106: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-107]` Legacy single-boss Room 5 trace #107: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-108]` Legacy single-boss Room 5 trace #108: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-109]` Legacy single-boss Room 5 trace #109: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-110]` Legacy single-boss Room 5 trace #110: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-111]` Legacy single-boss Room 5 trace #111: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-112]` Legacy single-boss Room 5 trace #112: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-113]` Legacy single-boss Room 5 trace #113: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-114]` Legacy single-boss Room 5 trace #114: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-115]` Legacy single-boss Room 5 trace #115: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-116]` Legacy single-boss Room 5 trace #116: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-117]` Legacy single-boss Room 5 trace #117: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-118]` Legacy single-boss Room 5 trace #118: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-119]` Legacy single-boss Room 5 trace #119: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-120]` Legacy single-boss Room 5 trace #120: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-121]` Legacy single-boss Room 5 trace #121: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-122]` Legacy single-boss Room 5 trace #122: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-123]` Legacy single-boss Room 5 trace #123: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-124]` Legacy single-boss Room 5 trace #124: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-125]` Legacy single-boss Room 5 trace #125: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-126]` Legacy single-boss Room 5 trace #126: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-127]` Legacy single-boss Room 5 trace #127: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-128]` Legacy single-boss Room 5 trace #128: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-129]` Legacy single-boss Room 5 trace #129: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-130]` Legacy single-boss Room 5 trace #130: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-131]` Legacy single-boss Room 5 trace #131: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-132]` Legacy single-boss Room 5 trace #132: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-133]` Legacy single-boss Room 5 trace #133: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-134]` Legacy single-boss Room 5 trace #134: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-135]` Legacy single-boss Room 5 trace #135: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-136]` Legacy single-boss Room 5 trace #136: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-137]` Legacy single-boss Room 5 trace #137: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-138]` Legacy single-boss Room 5 trace #138: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-139]` Legacy single-boss Room 5 trace #139: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-140]` Legacy single-boss Room 5 trace #140: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-141]` Legacy single-boss Room 5 trace #141: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-142]` Legacy single-boss Room 5 trace #142: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-143]` Legacy single-boss Room 5 trace #143: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-144]` Legacy single-boss Room 5 trace #144: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-145]` Legacy single-boss Room 5 trace #145: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-146]` Legacy single-boss Room 5 trace #146: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-147]` Legacy single-boss Room 5 trace #147: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-148]` Legacy single-boss Room 5 trace #148: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-149]` Legacy single-boss Room 5 trace #149: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
- `[SINGLE-DRAFT-150]` Legacy single-boss Room 5 trace #150: uncoordinated direct pursuit without partner reservation, superseded once `SetupWarehouseTrapsAndSquad` introduced the dual-guard `BossSquadMember` pair.
<!-- END_SINGLE_GUARD_DRAFTS -->

## 3. Overlap Penalty Scoring Trace (`BossSquadMember.FlankPenalty`)

`BossSquadMember.FlankPenalty(Vector3 candidate, FlankSide side)` discourages both guards from converging on the same flank pocket:
- **Wrong-Side Penalty:** `+8` m added to candidate score when `side != PreferredSide`.
- **Goal Overlap Penalty:** `+12` m added when `partner.ai.CurrentState == GuardState.Flank` and `Vector3.Distance(candidate, partner.ai.SelectedFlankPosition) < 3.0f`.

| Trace ID | Guard Slot | Candidate Side | Dist to Partner Goal (m) | Side Penalty | Overlap Penalty | Total `FlankPenalty` |
| --- | --- | --- | --- | --- | --- | --- |
| `OVLP-001` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-002` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-003` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-004` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-005` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-006` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-007` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-008` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-009` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-010` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-011` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-012` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-013` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-014` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-015` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-016` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-017` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-018` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-019` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-020` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-021` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-022` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-023` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-024` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-025` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-026` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-027` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-028` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-029` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-030` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-031` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-032` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-033` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-034` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-035` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-036` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-037` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-038` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-039` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-040` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-041` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-042` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-043` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-044` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-045` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-046` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-047` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-048` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-049` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-050` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-051` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-052` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-053` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-054` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-055` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-056` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-057` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-058` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-059` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-060` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-061` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-062` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-063` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-064` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-065` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-066` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-067` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-068` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-069` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-070` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-071` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-072` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-073` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-074` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-075` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-076` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-077` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-078` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-079` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-080` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-081` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-082` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-083` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-084` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-085` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-086` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-087` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-088` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-089` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-090` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-091` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-092` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-093` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-094` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-095` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-096` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-097` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-098` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-099` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-100` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-101` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-102` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-103` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-104` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-105` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-106` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-107` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-108` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-109` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-110` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-111` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-112` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-113` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-114` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-115` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-116` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-117` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-118` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-119` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-120` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-121` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-122` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-123` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-124` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-125` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-126` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-127` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-128` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-129` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-130` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-131` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-132` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-133` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-134` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-135` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-136` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-137` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-138` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-139` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-140` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-141` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-142` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-143` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-144` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-145` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-146` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-147` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-148` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-149` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-150` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-151` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-152` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-153` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-154` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-155` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-156` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-157` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-158` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-159` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-160` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-161` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-162` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |
| `OVLP-163` | `1` | `Right` | `3.30` | `+0` | `+0` | `+0` |
| `OVLP-164` | `0` | `Right` | `4.00` | `+8` | `+0` | `+8` |
| `OVLP-165` | `1` | `Right` | `4.70` | `+0` | `+0` | `+0` |
| `OVLP-166` | `0` | `Left` | `5.40` | `+0` | `+0` | `+0` |
| `OVLP-167` | `1` | `Right` | `6.10` | `+0` | `+0` | `+0` |
| `OVLP-168` | `0` | `Right` | `1.20` | `+8` | `+12` | `+20` |
| `OVLP-169` | `1` | `Right` | `1.90` | `+0` | `+12` | `+12` |
| `OVLP-170` | `0` | `Left` | `2.60` | `+0` | `+12` | `+12` |

## 4. Shared Confirmed Sighting Broadcasts

- `BossSquadMember.ShareConfirmedObservation()` executes in `Update()` at most twice per second (`nextReportAt = Time.time + 0.5f`).
- Broadcast requires `Available && ai.HasConfirmedVisualContact() && ai.PlayerTransform != null`; unseen live player coordinates are never shared.
- Receiving guard calls `ai.ReceiveSquadObservation(partner.LastSharedPosition)`, which:
  1. Ignores the report if the receiver already sees the player (`canSeePlayer`) or the point is outside `roomBounds`.
  2. Checks `Vector3.Distance(lastSquadNavigationGoal, point) > 2f` so small coordinate deltas accumulate properly without spamming.
  3. Sets `lastSeenPosition = point` and enters `GuardState.Search`, **never** granting `HasConfirmedVisualContact()` or firing permission.

- `[RADIO-001]` Squad observation broadcast check #001: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-002]` Squad observation broadcast check #002: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-003]` Squad observation broadcast check #003: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-004]` Squad observation broadcast check #004: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-005]` Squad observation broadcast check #005: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-006]` Squad observation broadcast check #006: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-007]` Squad observation broadcast check #007: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-008]` Squad observation broadcast check #008: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-009]` Squad observation broadcast check #009: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-010]` Squad observation broadcast check #010: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-011]` Squad observation broadcast check #011: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-012]` Squad observation broadcast check #012: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-013]` Squad observation broadcast check #013: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-014]` Squad observation broadcast check #014: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-015]` Squad observation broadcast check #015: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-016]` Squad observation broadcast check #016: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-017]` Squad observation broadcast check #017: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-018]` Squad observation broadcast check #018: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-019]` Squad observation broadcast check #019: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-020]` Squad observation broadcast check #020: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-021]` Squad observation broadcast check #021: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-022]` Squad observation broadcast check #022: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-023]` Squad observation broadcast check #023: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-024]` Squad observation broadcast check #024: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-025]` Squad observation broadcast check #025: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-026]` Squad observation broadcast check #026: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-027]` Squad observation broadcast check #027: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-028]` Squad observation broadcast check #028: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-029]` Squad observation broadcast check #029: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-030]` Squad observation broadcast check #030: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-031]` Squad observation broadcast check #031: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-032]` Squad observation broadcast check #032: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-033]` Squad observation broadcast check #033: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-034]` Squad observation broadcast check #034: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-035]` Squad observation broadcast check #035: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-036]` Squad observation broadcast check #036: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-037]` Squad observation broadcast check #037: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-038]` Squad observation broadcast check #038: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-039]` Squad observation broadcast check #039: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-040]` Squad observation broadcast check #040: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-041]` Squad observation broadcast check #041: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-042]` Squad observation broadcast check #042: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-043]` Squad observation broadcast check #043: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-044]` Squad observation broadcast check #044: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-045]` Squad observation broadcast check #045: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-046]` Squad observation broadcast check #046: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-047]` Squad observation broadcast check #047: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-048]` Squad observation broadcast check #048: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-049]` Squad observation broadcast check #049: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-050]` Squad observation broadcast check #050: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-051]` Squad observation broadcast check #051: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-052]` Squad observation broadcast check #052: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-053]` Squad observation broadcast check #053: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-054]` Squad observation broadcast check #054: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-055]` Squad observation broadcast check #055: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-056]` Squad observation broadcast check #056: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-057]` Squad observation broadcast check #057: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-058]` Squad observation broadcast check #058: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-059]` Squad observation broadcast check #059: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-060]` Squad observation broadcast check #060: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-061]` Squad observation broadcast check #061: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-062]` Squad observation broadcast check #062: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-063]` Squad observation broadcast check #063: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-064]` Squad observation broadcast check #064: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-065]` Squad observation broadcast check #065: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-066]` Squad observation broadcast check #066: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-067]` Squad observation broadcast check #067: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-068]` Squad observation broadcast check #068: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-069]` Squad observation broadcast check #069: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-070]` Squad observation broadcast check #070: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-071]` Squad observation broadcast check #071: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-072]` Squad observation broadcast check #072: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-073]` Squad observation broadcast check #073: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-074]` Squad observation broadcast check #074: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-075]` Squad observation broadcast check #075: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-076]` Squad observation broadcast check #076: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-077]` Squad observation broadcast check #077: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-078]` Squad observation broadcast check #078: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-079]` Squad observation broadcast check #079: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-080]` Squad observation broadcast check #080: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-081]` Squad observation broadcast check #081: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-082]` Squad observation broadcast check #082: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-083]` Squad observation broadcast check #083: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-084]` Squad observation broadcast check #084: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-085]` Squad observation broadcast check #085: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-086]` Squad observation broadcast check #086: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-087]` Squad observation broadcast check #087: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-088]` Squad observation broadcast check #088: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-089]` Squad observation broadcast check #089: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-090]` Squad observation broadcast check #090: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-091]` Squad observation broadcast check #091: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-092]` Squad observation broadcast check #092: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-093]` Squad observation broadcast check #093: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-094]` Squad observation broadcast check #094: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-095]` Squad observation broadcast check #095: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-096]` Squad observation broadcast check #096: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-097]` Squad observation broadcast check #097: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-098]` Squad observation broadcast check #098: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-099]` Squad observation broadcast check #099: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-100]` Squad observation broadcast check #100: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-101]` Squad observation broadcast check #101: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-102]` Squad observation broadcast check #102: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-103]` Squad observation broadcast check #103: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-104]` Squad observation broadcast check #104: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-105]` Squad observation broadcast check #105: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-106]` Squad observation broadcast check #106: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-107]` Squad observation broadcast check #107: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-108]` Squad observation broadcast check #108: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-109]` Squad observation broadcast check #109: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-110]` Squad observation broadcast check #110: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-111]` Squad observation broadcast check #111: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-112]` Squad observation broadcast check #112: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-113]` Squad observation broadcast check #113: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-114]` Squad observation broadcast check #114: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-115]` Squad observation broadcast check #115: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-116]` Squad observation broadcast check #116: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-117]` Squad observation broadcast check #117: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-118]` Squad observation broadcast check #118: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-119]` Squad observation broadcast check #119: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-120]` Squad observation broadcast check #120: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-121]` Squad observation broadcast check #121: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-122]` Squad observation broadcast check #122: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-123]` Squad observation broadcast check #123: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-124]` Squad observation broadcast check #124: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-125]` Squad observation broadcast check #125: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-126]` Squad observation broadcast check #126: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-127]` Squad observation broadcast check #127: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-128]` Squad observation broadcast check #128: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-129]` Squad observation broadcast check #129: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-130]` Squad observation broadcast check #130: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-131]` Squad observation broadcast check #131: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-132]` Squad observation broadcast check #132: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-133]` Squad observation broadcast check #133: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-134]` Squad observation broadcast check #134: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-135]` Squad observation broadcast check #135: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-136]` Squad observation broadcast check #136: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-137]` Squad observation broadcast check #137: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-138]` Squad observation broadcast check #138: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-139]` Squad observation broadcast check #139: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-140]` Squad observation broadcast check #140: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-141]` Squad observation broadcast check #141: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-142]` Squad observation broadcast check #142: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-143]` Squad observation broadcast check #143: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-144]` Squad observation broadcast check #144: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-145]` Squad observation broadcast check #145: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-146]` Squad observation broadcast check #146: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-147]` Squad observation broadcast check #147: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-148]` Squad observation broadcast check #148: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-149]` Squad observation broadcast check #149: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.
- `[RADIO-150]` Squad observation broadcast check #150: `0.5 s` throttle active, `ReportsReceived` incremented on confirmed visual only, and receiver `GuardShooting.HasVisualTarget` remains `false` until direct LOS is acquired.

## 5. Alternating Suppression Window Scheduling (`1.3 s` Cadence)

`BossSquadMember.OwnsSuppressionTurn(float clock, int slot)` evaluates `Mathf.FloorToInt(clock / 1.3f) % 2 == slot % 2`:
- When both guards are alive and both have confirmed visual contact (`partner.ai.HasConfirmedVisualContact()`), they alternate `1.3 s` firing windows.
- `CanFireNow` immediately yields `true` to the active guard if `partner == null`, `partner.shooting.IsReloading`, or `!partner.ai.HasConfirmedVisualContact()`.

| Window Index | Clock Interval (s) | `Slot 0` (`Left`) | `Slot 1` (`Right`) | Reload Cover Exception |
| --- | --- | --- | --- | --- |
| `WIN-000` | `[  0.0,   1.3)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-001` | `[  1.3,   2.6)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-002` | `[  2.6,   3.9)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-003` | `[  3.9,   5.2)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-004` | `[  5.2,   6.5)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-005` | `[  6.5,   7.8)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-006` | `[  7.8,   9.1)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-007` | `[  9.1,  10.4)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-008` | `[ 10.4,  11.7)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-009` | `[ 11.7,  13.0)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-010` | `[ 13.0,  14.3)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-011` | `[ 14.3,  15.6)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-012` | `[ 15.6,  16.9)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-013` | `[ 16.9,  18.2)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-014` | `[ 18.2,  19.5)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-015` | `[ 19.5,  20.8)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-016` | `[ 20.8,  22.1)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-017` | `[ 22.1,  23.4)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-018` | `[ 23.4,  24.7)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-019` | `[ 24.7,  26.0)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-020` | `[ 26.0,  27.3)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-021` | `[ 27.3,  28.6)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-022` | `[ 28.6,  29.9)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-023` | `[ 29.9,  31.2)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-024` | `[ 31.2,  32.5)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-025` | `[ 32.5,  33.8)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-026` | `[ 33.8,  35.1)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-027` | `[ 35.1,  36.4)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-028` | `[ 36.4,  37.7)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-029` | `[ 37.7,  39.0)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-030` | `[ 39.0,  40.3)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-031` | `[ 40.3,  41.6)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-032` | `[ 41.6,  42.9)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-033` | `[ 42.9,  44.2)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-034` | `[ 44.2,  45.5)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-035` | `[ 45.5,  46.8)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-036` | `[ 46.8,  48.1)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-037` | `[ 48.1,  49.4)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-038` | `[ 49.4,  50.7)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-039` | `[ 50.7,  52.0)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-040` | `[ 52.0,  53.3)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-041` | `[ 53.3,  54.6)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-042` | `[ 54.6,  55.9)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-043` | `[ 55.9,  57.2)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-044` | `[ 57.2,  58.5)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-045` | `[ 58.5,  59.8)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-046` | `[ 59.8,  61.1)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-047` | `[ 61.1,  62.4)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-048` | `[ 62.4,  63.7)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-049` | `[ 63.7,  65.0)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-050` | `[ 65.0,  66.3)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-051` | `[ 66.3,  67.6)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-052` | `[ 67.6,  68.9)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-053` | `[ 68.9,  70.2)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-054` | `[ 70.2,  71.5)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-055` | `[ 71.5,  72.8)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-056` | `[ 72.8,  74.1)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-057` | `[ 74.1,  75.4)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-058` | `[ 75.4,  76.7)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-059` | `[ 76.7,  78.0)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-060` | `[ 78.0,  79.3)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-061` | `[ 79.3,  80.6)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-062` | `[ 80.6,  81.9)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-063` | `[ 81.9,  83.2)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-064` | `[ 83.2,  84.5)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-065` | `[ 84.5,  85.8)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-066` | `[ 85.8,  87.1)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-067` | `[ 87.1,  88.4)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-068` | `[ 88.4,  89.7)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-069` | `[ 89.7,  91.0)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-070` | `[ 91.0,  92.3)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-071` | `[ 92.3,  93.6)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-072` | `[ 93.6,  94.9)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-073` | `[ 94.9,  96.2)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-074` | `[ 96.2,  97.5)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-075` | `[ 97.5,  98.8)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-076` | `[ 98.8, 100.1)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-077` | `[100.1, 101.4)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-078` | `[101.4, 102.7)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-079` | `[102.7, 104.0)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-080` | `[104.0, 105.3)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-081` | `[105.3, 106.6)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-082` | `[106.6, 107.9)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-083` | `[107.9, 109.2)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-084` | `[109.2, 110.5)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-085` | `[110.5, 111.8)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-086` | `[111.8, 113.1)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-087` | `[113.1, 114.4)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-088` | `[114.4, 115.7)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-089` | `[115.7, 117.0)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-090` | `[117.0, 118.3)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-091` | `[118.3, 119.6)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-092` | `[119.6, 120.9)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-093` | `[120.9, 122.2)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-094` | `[122.2, 123.5)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-095` | `[123.5, 124.8)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-096` | `[124.8, 126.1)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-097` | `[126.1, 127.4)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-098` | `[127.4, 128.7)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-099` | `[128.7, 130.0)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-100` | `[130.0, 131.3)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-101` | `[131.3, 132.6)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-102` | `[132.6, 133.9)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-103` | `[133.9, 135.2)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-104` | `[135.2, 136.5)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-105` | `[136.5, 137.8)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-106` | `[137.8, 139.1)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-107` | `[139.1, 140.4)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-108` | `[140.4, 141.7)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-109` | `[141.7, 143.0)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-110` | `[143.0, 144.3)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-111` | `[144.3, 145.6)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-112` | `[145.6, 146.9)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-113` | `[146.9, 148.2)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-114` | `[148.2, 149.5)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-115` | `[149.5, 150.8)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-116` | `[150.8, 152.1)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-117` | `[152.1, 153.4)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-118` | `[153.4, 154.7)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-119` | `[154.7, 156.0)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-120` | `[156.0, 157.3)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-121` | `[157.3, 158.6)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-122` | `[158.6, 159.9)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-123` | `[159.9, 161.2)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-124` | `[161.2, 162.5)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-125` | `[162.5, 163.8)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-126` | `[163.8, 165.1)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-127` | `[165.1, 166.4)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-128` | `[166.4, 167.7)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-129` | `[167.7, 169.0)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-130` | `[169.0, 170.3)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-131` | `[170.3, 171.6)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-132` | `[171.6, 172.9)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-133` | `[172.9, 174.2)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-134` | `[174.2, 175.5)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-135` | `[175.5, 176.8)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-136` | `[176.8, 178.1)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-137` | `[178.1, 179.4)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-138` | `[179.4, 180.7)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-139` | `[180.7, 182.0)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-140` | `[182.0, 183.3)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-141` | `[183.3, 184.6)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-142` | `[184.6, 185.9)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-143` | `[185.9, 187.2)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-144` | `[187.2, 188.5)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-145` | `[188.5, 189.8)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-146` | `[189.8, 191.1)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-147` | `[191.1, 192.4)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-148` | `[192.4, 193.7)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-149` | `[193.7, 195.0)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-150` | `[195.0, 196.3)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-151` | `[196.3, 197.6)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-152` | `[197.6, 198.9)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-153` | `[198.9, 200.2)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-154` | `[200.2, 201.5)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-155` | `[201.5, 202.8)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
| `WIN-156` | `[202.8, 204.1)` | FIRING | HOLD / REPOSITION | Immediate override if partner reloads or loses LOS |
| `WIN-157` | `[204.1, 205.4)` | HOLD / REPOSITION | FIRING | Immediate override if partner reloads or loses LOS |
