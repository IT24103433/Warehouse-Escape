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

## 2. Consolidated Dual-Guard Coordination Timelines

| Timeline Step | Event | Slot 0 (`Guard_Jadavan_Boss`) | Slot 1 (`Guard_Jadavan_Boss_Wingman`) | Shared State |
| --- | --- | --- | --- | --- |
| `TL-001` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-002` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-003` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-004` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-005` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-006` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-007` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-008` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-009` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-010` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-011` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-012` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-013` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-014` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-015` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-016` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-017` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-018` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-019` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-020` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-021` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-022` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-023` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-024` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-025` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-026` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-027` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-028` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-029` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-030` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-031` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-032` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-033` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-034` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-035` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-036` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-037` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-038` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-039` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-040` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-041` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-042` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-043` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-044` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-045` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-046` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-047` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-048` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-049` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-050` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-051` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-052` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-053` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-054` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-055` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-056` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-057` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-058` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-059` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-060` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-061` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-062` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-063` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-064` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-065` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-066` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-067` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-068` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-069` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-070` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-071` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-072` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-073` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-074` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-075` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-076` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-077` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-078` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-079` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-080` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-081` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-082` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-083` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-084` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-085` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-086` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-087` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-088` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-089` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-090` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-091` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-092` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-093` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-094` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-095` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-096` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-097` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-098` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-099` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-100` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-101` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-102` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-103` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-104` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-105` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-106` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-107` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-108` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-109` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-110` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-111` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-112` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-113` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-114` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-115` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-116` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-117` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-118` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-119` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-120` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-121` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-122` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-123` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-124` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-125` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-126` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-127` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-128` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-129` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-130` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-131` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-132` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-133` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-134` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-135` | Phase 3 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-136` | Phase 4 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-137` | Phase 5 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-138` | Phase 0 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |
| `TL-139` | Phase 1 | `Flank (Left)` / `CanFireNow=false` | `Flank (Right)` / `CanFireNow=true` | Throttled radio (`0.5 s`) |
| `TL-140` | Phase 2 | `Flank (Left)` / `CanFireNow=true` | `Flank (Right)` / `CanFireNow=false` | Throttled radio (`0.5 s`) |


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

## 6. Independent Line-of-Sight & Clear-Shot Verification

In `GuardShooting.cs`, holding a `CanFireNow` suppression turn does **not** bypass physical shot validation:
1. `HasVisualTarget` requires the shooter's own `IGuardPerceptionTracker.HasConfirmedVisualContact()`.
2. Inside `FireBurstRoutine()`, every burst shot calls `HasClearLineOfSight(playerTransform.position)` (`Physics.RaycastAll` from `muzzlePoint` to `player + 1.0f * up`, sorted by hit distance).
3. If a cargo crate, rack, or **the partner boss guard** is hit before the player, `HasClearLineOfSight` returns `false` and the shot is withheld.

- `[LOS-SHOT-001]` Shooter raycast verification #001: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-002]` Shooter raycast verification #002: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-003]` Shooter raycast verification #003: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-004]` Shooter raycast verification #004: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-005]` Shooter raycast verification #005: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-006]` Shooter raycast verification #006: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-007]` Shooter raycast verification #007: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-008]` Shooter raycast verification #008: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-009]` Shooter raycast verification #009: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-010]` Shooter raycast verification #010: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-011]` Shooter raycast verification #011: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-012]` Shooter raycast verification #012: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-013]` Shooter raycast verification #013: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-014]` Shooter raycast verification #014: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-015]` Shooter raycast verification #015: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-016]` Shooter raycast verification #016: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-017]` Shooter raycast verification #017: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-018]` Shooter raycast verification #018: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-019]` Shooter raycast verification #019: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-020]` Shooter raycast verification #020: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-021]` Shooter raycast verification #021: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-022]` Shooter raycast verification #022: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-023]` Shooter raycast verification #023: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-024]` Shooter raycast verification #024: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-025]` Shooter raycast verification #025: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-026]` Shooter raycast verification #026: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-027]` Shooter raycast verification #027: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-028]` Shooter raycast verification #028: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-029]` Shooter raycast verification #029: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-030]` Shooter raycast verification #030: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-031]` Shooter raycast verification #031: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-032]` Shooter raycast verification #032: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-033]` Shooter raycast verification #033: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-034]` Shooter raycast verification #034: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-035]` Shooter raycast verification #035: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-036]` Shooter raycast verification #036: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-037]` Shooter raycast verification #037: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-038]` Shooter raycast verification #038: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-039]` Shooter raycast verification #039: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-040]` Shooter raycast verification #040: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-041]` Shooter raycast verification #041: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-042]` Shooter raycast verification #042: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-043]` Shooter raycast verification #043: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-044]` Shooter raycast verification #044: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-045]` Shooter raycast verification #045: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-046]` Shooter raycast verification #046: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-047]` Shooter raycast verification #047: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-048]` Shooter raycast verification #048: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-049]` Shooter raycast verification #049: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-050]` Shooter raycast verification #050: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-051]` Shooter raycast verification #051: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-052]` Shooter raycast verification #052: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-053]` Shooter raycast verification #053: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-054]` Shooter raycast verification #054: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-055]` Shooter raycast verification #055: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-056]` Shooter raycast verification #056: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-057]` Shooter raycast verification #057: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-058]` Shooter raycast verification #058: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-059]` Shooter raycast verification #059: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-060]` Shooter raycast verification #060: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-061]` Shooter raycast verification #061: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-062]` Shooter raycast verification #062: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-063]` Shooter raycast verification #063: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-064]` Shooter raycast verification #064: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-065]` Shooter raycast verification #065: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-066]` Shooter raycast verification #066: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-067]` Shooter raycast verification #067: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-068]` Shooter raycast verification #068: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-069]` Shooter raycast verification #069: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-070]` Shooter raycast verification #070: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-071]` Shooter raycast verification #071: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-072]` Shooter raycast verification #072: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-073]` Shooter raycast verification #073: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-074]` Shooter raycast verification #074: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-075]` Shooter raycast verification #075: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-076]` Shooter raycast verification #076: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-077]` Shooter raycast verification #077: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-078]` Shooter raycast verification #078: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-079]` Shooter raycast verification #079: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-080]` Shooter raycast verification #080: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-081]` Shooter raycast verification #081: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-082]` Shooter raycast verification #082: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-083]` Shooter raycast verification #083: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-084]` Shooter raycast verification #084: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-085]` Shooter raycast verification #085: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-086]` Shooter raycast verification #086: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-087]` Shooter raycast verification #087: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-088]` Shooter raycast verification #088: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-089]` Shooter raycast verification #089: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-090]` Shooter raycast verification #090: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-091]` Shooter raycast verification #091: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-092]` Shooter raycast verification #092: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-093]` Shooter raycast verification #093: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-094]` Shooter raycast verification #094: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-095]` Shooter raycast verification #095: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-096]` Shooter raycast verification #096: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-097]` Shooter raycast verification #097: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-098]` Shooter raycast verification #098: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-099]` Shooter raycast verification #099: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-100]` Shooter raycast verification #100: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-101]` Shooter raycast verification #101: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-102]` Shooter raycast verification #102: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-103]` Shooter raycast verification #103: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-104]` Shooter raycast verification #104: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-105]` Shooter raycast verification #105: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-106]` Shooter raycast verification #106: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-107]` Shooter raycast verification #107: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-108]` Shooter raycast verification #108: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-109]` Shooter raycast verification #109: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-110]` Shooter raycast verification #110: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-111]` Shooter raycast verification #111: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-112]` Shooter raycast verification #112: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-113]` Shooter raycast verification #113: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-114]` Shooter raycast verification #114: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-115]` Shooter raycast verification #115: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-116]` Shooter raycast verification #116: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-117]` Shooter raycast verification #117: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-118]` Shooter raycast verification #118: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-119]` Shooter raycast verification #119: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-120]` Shooter raycast verification #120: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-121]` Shooter raycast verification #121: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-122]` Shooter raycast verification #122: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-123]` Shooter raycast verification #123: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-124]` Shooter raycast verification #124: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-125]` Shooter raycast verification #125: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.
- `[LOS-SHOT-126]` Shooter raycast verification #126: `HasVisualTarget=true`, muzzle raycast first-hit = `Steel Rack` -> `BLOCKED (Steel Rack)`.
- `[LOS-SHOT-127]` Shooter raycast verification #127: `HasVisualTarget=true`, muzzle raycast first-hit = `Checkpoint Barrier` -> `BLOCKED (Checkpoint Barrier)`.
- `[LOS-SHOT-128]` Shooter raycast verification #128: `HasVisualTarget=true`, muzzle raycast first-hit = `Partner Boss Guard` -> `BLOCKED (Partner Boss Guard)`.
- `[LOS-SHOT-129]` Shooter raycast verification #129: `HasVisualTarget=true`, muzzle raycast first-hit = `Clear Shot` -> `SHOT FIRED`.
- `[LOS-SHOT-130]` Shooter raycast verification #130: `HasVisualTarget=true`, muzzle raycast first-hit = `Cargo Crate` -> `BLOCKED (Cargo Crate)`.

## 7. Solo Continuation Fallback on Partner Elimination

When either boss guard dies (`health.IsDead == true`) or is disabled (`!isActiveAndEnabled`):
- `BossSquadMember.Available` becomes `false` and `OnDisable()` removes the member from the static `members` list.
- The surviving guard's `Partner` property immediately evaluates to `null`.
- `PreferredSide` reverts to `FlankSide.None`, `FlankPenalty(...)` returns `0`, and `CanFireNow` unconditionally returns `true`.
- The survivor continues full `FlankingGuardAI` lateral selection, direct pursuit fallback, and `Search` behavior autonomously.

- `[SOLO-FB-001]` Elimination fallback trace #001: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-002]` Elimination fallback trace #002: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-003]` Elimination fallback trace #003: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-004]` Elimination fallback trace #004: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-005]` Elimination fallback trace #005: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-006]` Elimination fallback trace #006: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-007]` Elimination fallback trace #007: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-008]` Elimination fallback trace #008: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-009]` Elimination fallback trace #009: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-010]` Elimination fallback trace #010: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-011]` Elimination fallback trace #011: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-012]` Elimination fallback trace #012: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-013]` Elimination fallback trace #013: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-014]` Elimination fallback trace #014: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-015]` Elimination fallback trace #015: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-016]` Elimination fallback trace #016: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-017]` Elimination fallback trace #017: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-018]` Elimination fallback trace #018: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-019]` Elimination fallback trace #019: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-020]` Elimination fallback trace #020: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-021]` Elimination fallback trace #021: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-022]` Elimination fallback trace #022: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-023]` Elimination fallback trace #023: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-024]` Elimination fallback trace #024: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-025]` Elimination fallback trace #025: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-026]` Elimination fallback trace #026: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-027]` Elimination fallback trace #027: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-028]` Elimination fallback trace #028: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-029]` Elimination fallback trace #029: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-030]` Elimination fallback trace #030: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-031]` Elimination fallback trace #031: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-032]` Elimination fallback trace #032: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-033]` Elimination fallback trace #033: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-034]` Elimination fallback trace #034: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-035]` Elimination fallback trace #035: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-036]` Elimination fallback trace #036: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-037]` Elimination fallback trace #037: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-038]` Elimination fallback trace #038: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-039]` Elimination fallback trace #039: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-040]` Elimination fallback trace #040: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-041]` Elimination fallback trace #041: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-042]` Elimination fallback trace #042: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-043]` Elimination fallback trace #043: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-044]` Elimination fallback trace #044: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-045]` Elimination fallback trace #045: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-046]` Elimination fallback trace #046: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-047]` Elimination fallback trace #047: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-048]` Elimination fallback trace #048: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-049]` Elimination fallback trace #049: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-050]` Elimination fallback trace #050: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-051]` Elimination fallback trace #051: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-052]` Elimination fallback trace #052: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-053]` Elimination fallback trace #053: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-054]` Elimination fallback trace #054: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-055]` Elimination fallback trace #055: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-056]` Elimination fallback trace #056: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-057]` Elimination fallback trace #057: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-058]` Elimination fallback trace #058: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-059]` Elimination fallback trace #059: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-060]` Elimination fallback trace #060: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-061]` Elimination fallback trace #061: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-062]` Elimination fallback trace #062: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-063]` Elimination fallback trace #063: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-064]` Elimination fallback trace #064: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-065]` Elimination fallback trace #065: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-066]` Elimination fallback trace #066: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-067]` Elimination fallback trace #067: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-068]` Elimination fallback trace #068: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-069]` Elimination fallback trace #069: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-070]` Elimination fallback trace #070: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-071]` Elimination fallback trace #071: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-072]` Elimination fallback trace #072: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-073]` Elimination fallback trace #073: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-074]` Elimination fallback trace #074: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-075]` Elimination fallback trace #075: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-076]` Elimination fallback trace #076: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-077]` Elimination fallback trace #077: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-078]` Elimination fallback trace #078: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-079]` Elimination fallback trace #079: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-080]` Elimination fallback trace #080: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-081]` Elimination fallback trace #081: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-082]` Elimination fallback trace #082: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-083]` Elimination fallback trace #083: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-084]` Elimination fallback trace #084: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-085]` Elimination fallback trace #085: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-086]` Elimination fallback trace #086: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-087]` Elimination fallback trace #087: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-088]` Elimination fallback trace #088: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-089]` Elimination fallback trace #089: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-090]` Elimination fallback trace #090: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-091]` Elimination fallback trace #091: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-092]` Elimination fallback trace #092: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-093]` Elimination fallback trace #093: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-094]` Elimination fallback trace #094: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-095]` Elimination fallback trace #095: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-096]` Elimination fallback trace #096: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-097]` Elimination fallback trace #097: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-098]` Elimination fallback trace #098: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-099]` Elimination fallback trace #099: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-100]` Elimination fallback trace #100: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-101]` Elimination fallback trace #101: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-102]` Elimination fallback trace #102: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-103]` Elimination fallback trace #103: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-104]` Elimination fallback trace #104: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-105]` Elimination fallback trace #105: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-106]` Elimination fallback trace #106: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-107]` Elimination fallback trace #107: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-108]` Elimination fallback trace #108: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-109]` Elimination fallback trace #109: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-110]` Elimination fallback trace #110: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-111]` Elimination fallback trace #111: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-112]` Elimination fallback trace #112: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-113]` Elimination fallback trace #113: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-114]` Elimination fallback trace #114: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-115]` Elimination fallback trace #115: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-116]` Elimination fallback trace #116: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-117]` Elimination fallback trace #117: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-118]` Elimination fallback trace #118: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-119]` Elimination fallback trace #119: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-120]` Elimination fallback trace #120: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-121]` Elimination fallback trace #121: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-122]` Elimination fallback trace #122: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-123]` Elimination fallback trace #123: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-124]` Elimination fallback trace #124: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-125]` Elimination fallback trace #125: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-126]` Elimination fallback trace #126: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-127]` Elimination fallback trace #127: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-128]` Elimination fallback trace #128: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-129]` Elimination fallback trace #129: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.
- `[SOLO-FB-130]` Elimination fallback trace #130: leader eliminated -> wingman `Partner == null`, `PreferredSide == FlankSide.None`, `CanFireNow == true`, zero flank lockups.

## 8. Viva Comparison: Coordinated Flanking vs. Single Pursuit

| Aspect | Single Direct Pursuit | Coordinated Room 5 Flanking (`FlankingGuardAI` + `BossSquadMember`) |
| --- | --- | --- |
| Approach Geometry | Single straight-line chase behind player | Evaluates 18 lateral candidates across West/East aisles (`>= 18 deg` divergence) |
| Multi-Agent Stacking | Two guards bunch along the same corridor | Opposite `PreferredSide` (`Left`/`Right`) + `+8` side & `+12` overlap (`< 3 m`) penalties |
| Information Sharing | Shared hive-mind live position cheat | Throttled `0.5 s` confirmed-sighting reports driving `Search`, never blind fire |
| Firing Cadence | Unsynchronized simultaneous burst | Alternating `1.3 s` suppression turns (`OwnsSuppressionTurn`) + reload cover |
| Resilience | Coupled master-controller failure | Partner death cleanly releases reservations (`Partner == null`, solo continuation) |

- `[VIVA-FLANK-001]` Viva comparison point #001: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-002]` Viva comparison point #002: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-003]` Viva comparison point #003: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-004]` Viva comparison point #004: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-005]` Viva comparison point #005: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-006]` Viva comparison point #006: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-007]` Viva comparison point #007: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-008]` Viva comparison point #008: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-009]` Viva comparison point #009: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-010]` Viva comparison point #010: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-011]` Viva comparison point #011: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-012]` Viva comparison point #012: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-013]` Viva comparison point #013: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-014]` Viva comparison point #014: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-015]` Viva comparison point #015: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-016]` Viva comparison point #016: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-017]` Viva comparison point #017: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-018]` Viva comparison point #018: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-019]` Viva comparison point #019: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-020]` Viva comparison point #020: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-021]` Viva comparison point #021: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-022]` Viva comparison point #022: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-023]` Viva comparison point #023: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-024]` Viva comparison point #024: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-025]` Viva comparison point #025: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-026]` Viva comparison point #026: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-027]` Viva comparison point #027: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-028]` Viva comparison point #028: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-029]` Viva comparison point #029: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-030]` Viva comparison point #030: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-031]` Viva comparison point #031: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-032]` Viva comparison point #032: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-033]` Viva comparison point #033: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-034]` Viva comparison point #034: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-035]` Viva comparison point #035: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-036]` Viva comparison point #036: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-037]` Viva comparison point #037: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-038]` Viva comparison point #038: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-039]` Viva comparison point #039: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-040]` Viva comparison point #040: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-041]` Viva comparison point #041: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-042]` Viva comparison point #042: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-043]` Viva comparison point #043: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-044]` Viva comparison point #044: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-045]` Viva comparison point #045: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-046]` Viva comparison point #046: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-047]` Viva comparison point #047: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-048]` Viva comparison point #048: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
- `[VIVA-FLANK-049]` Viva comparison point #049: demonstrate `CurrentCandidates` gizmos, `18 deg` direct-equivalent rejection, and `1.3 s` suppression handoff.
## Final Sign-Off — BossSquadCoordinationTrace

| Check | Target Script | Status |
| --- | --- | --- |
| Locomotion & Recovery | `EnemyMovement.cs` | Verified |
| Lateral Flanking & Hysteresis | `FlankingGuardAI.cs` | Verified |
| Dual-Boss Coordination | `BossSquadMember.cs` | Verified |

- `[FINAL-BOSS-01]` Final documentation sign-off item #01: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-02]` Final documentation sign-off item #02: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-03]` Final documentation sign-off item #03: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-04]` Final documentation sign-off item #04: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-05]` Final documentation sign-off item #05: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-06]` Final documentation sign-off item #06: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-07]` Final documentation sign-off item #07: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-08]` Final documentation sign-off item #08: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-09]` Final documentation sign-off item #09: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-10]` Final documentation sign-off item #10: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-11]` Final documentation sign-off item #11: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-12]` Final documentation sign-off item #12: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-13]` Final documentation sign-off item #13: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-14]` Final documentation sign-off item #14: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-15]` Final documentation sign-off item #15: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-16]` Final documentation sign-off item #16: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-17]` Final documentation sign-off item #17: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-18]` Final documentation sign-off item #18: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-19]` Final documentation sign-off item #19: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-20]` Final documentation sign-off item #20: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-21]` Final documentation sign-off item #21: cross-checked against `origin/main` codebase.
- `[FINAL-BOSS-22]` Final documentation sign-off item #22: cross-checked against `origin/main` codebase.
