# UW2 first-pass survey & enhancement notes

_Working notes from an AI-assisted pass on the fork. Not upstream canon — scratch for the fork owner._

## Fork / upstream situation
- This fork = `bbleak-repo/UnderworldGodot`, forked from `hankmorgan/UnderworldGodot` (310★, active, C#/Godot 4.3, .NET 9).
- **Tzrlk fork verdict: not worth merging.** It's 10 commits "ahead" but **544 commits behind** today's upstream, and its headline work (Godot 4.3 bump, .NET 6→9, settings schema/example) is **already in upstream independently**. The only novel piece is a **Godot 4.3→4.4 engine bump**, which is a deliberate high-diff decision (auto-generates hundreds of `.cs.uid` files) — left for the owner to decide, NOT auto-merged.
- ShortBeard / Abedegno / Tzrlk are credited upstream contributors — their good work reaches `main` via PRs, which is why the forks look "even."

## Codebase shape
- 438 C# files. Mature: conversations, combat, magic, save/load, NPC AI, SCD scheduled events, 4 audio synth engines.
- Direct port of the DOS disassembly — functions/constants mirror original segments (`Seg007_193`). **Fidelity > cleverness.**
- Not CLI-runnable: needs Godot 4.3 editor + copyrighted UW1/UW2 data files + `uwsettings.json`. AI changes need human play-testing to fully verify.

## Enhancements attempted this pass (branch `claude/setup-and-uw2-pass`)
| # | Item | Type | Verified? |
|---|------|------|-----------|
| 1 | `CLAUDE.md` added | docs/setup | n/a |
| 2 | `GetOrdinal()` teens bug (`21th`→`21st`) | C# logic | ✅ by reasoning (compiles, correct English ordinals) |
| 3 | `CharLevel` label `11th`→`11t` clip | scene tweak | ⚠️ needs in-engine visual check |
| 4 | HP/Mana/EXP labels clip at 3 digits (same class) | scene tweak | ⚠️ needs in-engine visual check |
| 5 | Automap note null-termination (backlog item) | investigation | ✅ already handled by `Serialize()` — no change needed |

### Detail: the `11th → 11t` bug (backlog "UW2 confirmed bug")
Root cause is **UI, not logic**. `GetOrdinal(11)` already returns `"th"` correctly. The
`CharLevel` `RichTextLabel` (scenes/Underworld.tscn) is only **84px wide at font size 64**;
a 3-char string (`9th`) fits, but a 4-char string (`11th`) word-wraps and the `h` lands on a
clipped second line → shows `11t`.
- **Fix applied:** `autowrap_mode = 0` on the node (surgical, revertible).
- **Alternative if that overflows left:** widen by lowering `offset_left` (50→~14) — but check it
  doesn't overlap `CharClass`. Verify by loading UW2 with a level ≥10 character.

## Ranked candidate next targets (from backlog.md)
Prioritised for **low risk + high reasoning-verifiability** (since we can't play-test here):
1. **`ordinal cut off` sibling checks** — audit other fixed-width `RichTextLabel`s for the same
   2-digit clip (EXP, stats when 3-digit). Same class of bug.
2. **In-game console commands** (backlog) — spawn items, teleport, set quest flags, god mode. Pure
   additive C#, testable in isolation, big QoL win for *your* modding.
3. **UW2 world-number calcs "may be incorrect"** — needs disassembly cross-ref; medium risk.
4. **Troll snowball animation bug / forcefields not reaching ground** — visual, need play-testing.
5. **Getting-lost mechanics (UW2, "what's missing")** — larger feature; scope later.

## Open decisions for the owner
- [ ] Take the Godot 4.4 bump from Tzrlk? (recommend: not now — do it as its own deliberate PR.)
- [ ] Push this branch to `origin` and/or open a PR upstream for the `GetOrdinal` fix? (it's a clean,
      self-contained correctness fix — good upstream candidate.)
