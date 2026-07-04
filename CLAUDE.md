# CLAUDE.md — UnderworldGodot (Ultima Underworld 1 & 2 engine recreation)

> Guidance for Claude Code when working in this repo. This is a fork of
> `hankmorgan/UnderworldGodot`. Keep this file in sync with reality as the code moves.

## What this project is
A faithful, "vanilla / vanilla-plus" recreation of the *Ultima Underworld* and
*Ultima Underworld II* engines in **Godot 4.3** using **C# / .NET 9**. The goal is to
reproduce the ORIGINAL game logic as closely as possible — much of the code is a direct
port of the reverse-engineered DOS disassembly, so functions and comments often reference
original segment names (e.g. `Seg007_193`). Status: **pre-alpha** ("a glorified map viewer
with a lot of game logic implemented").

## 🚩 Golden rules
1. **Fidelity over cleverness.** When in doubt, match the original game's behaviour, not a
   "better" version. This is a preservation project. Cross-reference the disassembly notes at
   https://github.com/hankmorgan/UWReverseEngineering before changing game-logic constants.
2. **No AI-generated *assets*.** The project's AI Policy forbids AI-upscaled art / AI-generated
   game artifacts (to preserve the vanilla look & feel). The policy is *silent* on AI-*assisted
   code* — it addresses assets only — so writing code with AI isn't prohibited, but never commit
   generated textures, sprites, sounds, or models. (Note: this is an MIT-licensed project; the AI
   policy is the maintainer's preference for upstream, not a licence term binding a personal fork.)
3. **UW1 vs UW2 differences are everywhere.** Tons of logic branches on the active game. Before
   editing gameplay code, check whether it's UW1-only, UW2-only, or shared. The backlog is full
   of "check if this differs for UW1/UW2" notes — respect them.
4. **Small, reviewable commits.** The maintainer takes PRs from contributors (Tzrlk, ShortBeard,
   Abedegno). Keep changes focused and explain the vanilla behaviour you're matching.
5. **Don't reformat/upgrade wholesale.** Avoid engine-version bumps (e.g. 4.3→4.4) or mass
   reformatting unless explicitly asked — they create massive diffs (auto-generated `.uid` files,
   etc.) that are painful to review and merge upstream.

## Layout
```
main.cs                 Entry point / bootstrap (root, ~29KB)
LaunchScene.tscn        First scene Godot opens
src/
  World/                Level/tilemap loading, per-world logic (specificworlds/)
  loaders/              Binary format parsers (.ark, .dat, .gr, palettes) — the RE core
  conversation/         NPC dialogue VM + conversation_functions/ (opcodes)
  scd/                  SCD.ARK scheduled-events system (heavily UW2)
  npc/                  NPC AI, movement, pathfinding
  objects/ objectdata/  Game object model + object.dat data (largest dir, ~69 files)
  traps/ triggers/      Trap + trigger logic (~60 files)
  magic/                Spell casting + effects
  interaction/combat/   Player interaction modes + combat
  physics/              Collision, motion, swimming
  player/ chargen/      Player state, stats, character creation
  ui/                   Paperdoll, panels, automap, runes, spells, stats (~32 files)
  savegame/             Save/load + UW2 repacker
  audio/                XMI synth (MeltySynth/AdlMidi/Munt), VOC speech, SFX
  utility/              config, palette, RNG, string loader, helpers
docs/                   Architecture docs (e.g. docs/audio-architecture.md)
backlog.md              Maintainer's living TODO/bug list — READ THIS for task ideas
```

## Build & run
There is **no CLI build/run in this repo's flow** — it's a Godot editor project.
- Requires **.NET 9 SDK** and **Godot 4.3.0** (the .NET/Mono build).
- Version mgmt via godotenv: `dotnet tool run godotenv godot install 4.3-stable`.
- Needs **original UW1/UW2 game data files** (not included) + a `uwsettings.json` pointing at them
  (`pathuw1`/`pathuw2`, `gametoload`, `level`, ...). See README "UWsettings.json".
- Build in the Godot editor (Build button) or VSCode with the C# Tools for Godot extension.
- **Claude cannot fully run/verify the game here** (needs the engine + copyrighted game files).
  Verify changes by C# compilation reasoning + reading the disassembly; flag anything that needs a
  human to play-test.

### If you only need to compile-check
`dotnet build Underworld.csproj` works for type-checking C# once the Godot.NET SDK is restored,
but the game itself must be launched from Godot with data files present.

## Repo remotes (this fork)
- `origin`   → `bbleak-repo/UnderworldGodot` (this fork — push here)
- `upstream` → `hankmorgan/UnderworldGodot` (source of truth; sync from here, never force-push)
- `tzrlk`    → `Tzrlk/UnderworldGodot` (a contributor fork; mostly upstreamed already)

Keep `main` clean and tracking `upstream/main`. Do work on `claude/*` branches.

## Gotchas
- **git-LFS mismatch:** ~54 files under `resources/` are declared as LFS pointers in
  `.gitattributes` but were committed as raw blobs upstream. Checkout warns; it's harmless. Don't
  "fix" it by re-adding to LFS without coordinating with upstream.
- `.cs.uid` files are Godot-4.4 artifacts — this repo is 4.3, so they should NOT appear here.
- Many gameplay constants are magic numbers straight from the disassembly. Leave a comment citing
  the source rather than "cleaning them up."

## Where to find work
`backlog.md` is the canonical task list. UW2-flavoured quick wins currently include: ordinal suffix
bug at level >10 (`11th`→`11t`), UW2 panel slide-vs-rotate, troll snowball animation, forcefields
not reaching the ground in the prison tower, and UW2 world-number calculations.
