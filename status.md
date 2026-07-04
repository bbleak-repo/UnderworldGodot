# status.md — UnderworldGodot fork handoff (→ Mac-side Claude)

> **Purpose:** This is a handoff from a Claude Code session that ran on a **Windows** box (no Godot, no
> game data → could not run/verify anything) to a Claude Code session on the owner's **Apple-Silicon
> Mac** (which *can* build + run). If you're the Mac-side agent: read this top-to-bottom once, then
> you have everything you need to onboard without re-investigating the codebase.
>
> **Owner:** bbleak — loved *Ultima Underworld II* as a kid, wants to **build new levels / new
> sections with AI-assisted coding**, for personal enjoyment on this **personal MIT fork**
> (`bbleak-repo/UnderworldGodot`, forked from `hankmorgan/UnderworldGodot`).

---

## 0. The working agreement (read this first)

- **You write C#, the owner builds + runs in Godot and reports back.** You cannot assume a change
  works until they run it. Always tell them *what to look for* and *how to trigger it*. Flag anything
  that needs in-engine visual/behavioural verification.
- **Fidelity for the base game; freedom for new content.** This is a vanilla-faithful preservation
  project — when touching *existing* game logic, match the original DOS behaviour, don't "improve"
  constants (cross-ref the disassembly: https://github.com/hankmorgan/UWReverseEngineering). But
  **new levels / new content the owner designs are fair game** — that's the whole point of the fork.
- **AI-asset policy (upstream):** the README bans **AI-generated *assets*** (art/audio) to preserve
  the vanilla look. It is **silent on AI-assisted *code***. This is an **MIT** project, so the policy
  is the maintainer's *preference for upstream*, not a licence term binding this fork. Practical rule:
  **never upstream AI assets; keep the owner's personal content separate from any code PRs.**
- **Small, focused commits.** Work on branch `claude/setup-and-uw2-pass` or new `claude/*` branches.
  Commit message trailer: `Co-Authored-By: Claude Opus 4.8 (1M context) <noreply@anthropic.com>`.

---

## 1. Get it running on the Mac (do this first)

**Stack:** Godot **4.3** (.NET/Mono build) + **.NET 9** SDK, C#. All Apple-Silicon-native.

1. **Clone + branch**
   ```bash
   git clone https://github.com/bbleak-repo/UnderworldGodot.git
   cd UnderworldGodot
   git checkout claude/setup-and-uw2-pass   # to get our fixes + this file
   ```
2. **Install .NET 9 SDK (arm64):** https://dotnet.microsoft.com/download/dotnet/9.0
3. **Install Godot 4.3 (.NET / "Mono" build, arm64):**
   https://godotengine.org/download/archive/4.3-stable/ — pick the **.NET** macOS build.
   *Or* use the project's version manager: `dotnet tool run godotenv godot install 4.3-stable`
   then `dotnet tool run godotenv godot env setup`.
4. **Provide UW2 game data (owner supplies — it's legally theirs):** extract the GOG `game.gog`
   with a zip tool (7-Zip/Keka) and note the folder path. The repo ships **no** game data by design.
5. **Create `uwsettings.json`.** Loaded via Godot's `user://` path — on macOS that's
   `~/Library/Application Support/Godot/app_userdata/Underworld/`. (Confirm exact filename/location in
   `src/utility/config.cs` — README also allows it beside the project.) See `uwsettings.example.json`
   and `uwsettings.schema.json` in the repo root for the full schema. Minimal UW2 example:
   ```json
   {
     "pathuw2": "/Users/you/Games/UW2",
     "gametoload": "UW2",
     "level": 0,
     "lightlevel": 7,
     "levarkfolder": "DATA",
     "shader": "UWSHADER",
     "synth": "soundfont"
   }
   ```
6. **Build + run:** open the project in Godot → press **Build** (required!) → **Run**. It opens
   `LaunchScene`. Alternatively use VSCode with the "C# Tools for Godot" extension.
7. **Compile-check only (no game data needed):** `dotnet build Underworld.csproj`.

### Expected first-run friction (all normal for pre-alpha — paste errors to the Mac agent)
- **Native audio libs:** the synth engines use native interop (Munt.NET / AdlMidi.NET / MeltySynth).
  The code already branches on `.dll`/`.dylib`/`.so`, so macOS is intended — but a missing/む
  mismatched `.dylib` is the most likely first snag. If audio init fails it *silently falls back to
  OPL*, so a crash here is worth investigating, a missing sound is not fatal.
- **git-LFS warning** on ~54 files under `resources/`: `.gitattributes` declares them LFS but they
  were committed as **real blobs** upstream. The warning is **benign** — the content is present; a
  normal clone gets usable art. Don't "fix" the LFS setup without coordinating upstream.
- **Path/case quirks:** macOS is case-insensitive by default but data-file names (`LEV.ARK`,
  `STRINGS.PAK`) matter if the volume is case-sensitive — verify the extracted UW2 folder casing.

---

## 2. What's already been done (branch `claude/setup-and-uw2-pass`)

| Commit (newest→oldest) | What | Verified? |
|---|---|---|
| `CLAUDE.md` accuracy fix | Corrected AI-policy note (bans assets, silent on code; MIT governs fork) | n/a |
| survey notes update | `docs/uw2-first-pass-notes.md` — HP/Mana/EXP + automap findings | n/a |
| HP/Mana/EXP clip fix | `scenes/Underworld.tscn`: `autowrap_mode=0` on VIT/MANA/EXP labels | ⚠️ **needs visual check** |
| CharLevel clip fix | `scenes/Underworld.tscn`: stops `11th`→`11t` wrap-clip | ⚠️ **needs visual check** |
| GetOrdinal fix | `src/utility/StringLoader.cs`: `21th`→`21st` + teens rule. Clean **upstream candidate** | ✅ by reasoning |
| CLAUDE.md | Repo guidance for AI-assisted dev | n/a |

**➡️ FIRST TASK for the Mac agent:** load UW2, get a character to **level ≥10** (or 3-digit HP/mana)
and confirm the stats panel now shows `11th` (not `11t`) and full `112/120` HP without clipping.
Report back — that closes out the 3 "needs visual check" items. `~` (the `'`/Apostrophe key) is a
cheat that jumps you to level 16 + full mage (see §4d) — handy for triggering these.

---

## 3. Key findings you should not have to rediscover

1. **A whole level is ONE ~32 KB byte buffer** (`lev_ark_block.Data`, `0x8000` for UW2). The C# tile
   and object objects are **thin views** (bit-field accessors) into that buffer — not decoded structs.
2. **Editing a level and saving it already works and is proven by round-trip tests.** Because edits
   are in-place mutations of that buffer, `SaveGame.Save` captures them and reload is byte-identical.
3. **The "UW2 Repacker" (LZ compression) is a broken red herring — ignore it.** It's WIP/crashy and
   **not on the save path**; the writer emits *uncompressed* blocks, which both the port and real DOS
   UW2 accept. Compression is NOT required for custom levels.
4. **Spawning objects/NPCs, adding strings, and hooking new key commands are all one-liners that
   already exist** (see §4). There is **no debug console yet** — that's the recommended first tool.
5. **New levels are limited to 64×64**, but every level's full grid already exists (unreachable areas
   are just "solid rock" tiles), and UW2 supports **up to 80 level slots** — so you both *remodel in
   place* and *add + connect* new maps. See §5.
6. **Game art loads from original `.gr/.byt/.dat`** via loaders; there's no importer for *new* sprites,
   though Godot nodes accept arbitrary `Texture2D`. Keep new *visual* work minimal + policy-aware.

> Line numbers below are from investigation and are **approximate** (a couple shifted after our
> edits). Treat them as "start reading here," and confirm against current source.

---

## 4. Architecture map (the seams you'll use)

### 4a. Level load pipeline
- Entry: `UWTileMap.LoadTileMap(levelNo, datafolder, newSession)` — `src/World/tilemap.cs:~276`.
  Callers: `uimanager_mainmenu.cs` (new game/load) and `src/utility/teleportation.cs` (transitions).
- Flow: config picks game+path (`src/utility/config.cs`) → `LevArkLoader.LoadLevArkFileData`
  (`src/loaders/levarkloader.cs`) reads `DATA/LEV.ARK` → `new UWTileMap(levelNo)` loads 3 blocks via
  `DataLoader.LoadUWBlock` (`src/loaders/dataloader.cs:~142`, UW2 LZ-decompresses to `0x8000`) →
  `BuildTileMapUW` (`tilemap.cs:~525`) populates `TileInfo[65,65]` + `LevelObjects[1024]` →
  `ObjectCreator.GenerateObjects` → `tileMapRender.GenerateLevelFromTileMap`
  (`src/World/tilemaprender.cs:~88`) builds Godot `ArrayMesh` + collision per tile.
- **In-memory model:** fixed **64×64**; per-tile persistent state = **4 bytes** at
  `Ptr = tileX*4 + tileY*256` in `lev_ark_block.Data`. Accessors in `src/World/tileinfo.cs:~43-327`
  (`tileType` bits 0-3, `floorHeight` bits 4-7, `floorTexture`, `wallTexture`, `indexObjectList`,
  `doorBit`, `flags`, `noMagic`). Objects region @ `0x4000`; free-list ptrs @ `0x7C00-0x7C04`;
  active-mobile count @ `0x7C00`; `"uw"` magic @ `0x7C06`. `NO_OF_LEVELS`: UW1=9, UW2=80.
- **The injection seam for custom levels:** everything downstream reads only from the in-memory
  `UWTileMap` — the file is never re-read. So a hand-built buffer assigned to
  `dungeons[i].lev_ark_block.Data` + `BuildTileMapUW` + render = a playable custom level. The one
  missing piece is a "**build UWTileMap from an in-memory buffer**" entry (today the ctor is hard-wired
  to `LevArkLoader`). Add that seam. See §5.

### 4b. Editing & persisting levels
- Edit tiles via the `TileInfo` setters or `TileInfo.ChangeTile(...)` (`src/World/tileinfo.cs:~540`).
- Save: `SaveGame.Save(slot, desc)` (`src/savegame/SaveGame.cs:~13`) writes `PLAYER.DAT`,
  `BGLOBALS.DAT`, `LEV.ARK` (`src/savegame/LevArkWriter.cs`), `SCD.ARK` (UW2), and map notes.
- **UW2 save is UI-gated** pending DOS re-validation — the writer works + passes tests, but the
  Options "Save" button refuses for UW2. For level-persistence testing, call the writer directly or
  temporarily un-gate. Round-trip proof: `tests/Underworld.Save.Tests/LevArkRoundTripTests.cs`.

### 4c. Spawning objects & NPCs (already easy)
- **One call:** `ObjectCreator.spawnObjectInTile(itemid, tileX, tileY, xpos, ypos, zpos, WhichList,
  RenderImmediately=true)` — `src/utility/ObjectCreator.cs:~34`. Allocates a slot, links it into the
  tile's object list, and spawns the `Node3D`.
- **Which list:** items → `ObjectFreeLists.ObjectListType.StaticList`; NPCs/projectiles →
  `MobileList`. `majorclass = itemid >> 6`; `majorclass == 1` ⇒ NPC (auto AI init).
- **"Spawn at my feet":** read `playerdat.playerObject.tileX/tileY`. One-liner:
  ```csharp
  ObjectCreator.spawnObjectInTile(id,
      playerdat.playerObject.tileX, playerdat.playerObject.tileY, 3, 3, 0,
      (id >> 6) == 1 ? ObjectFreeLists.ObjectListType.MobileList
                     : ObjectFreeLists.ObjectListType.StaticList);
  ```
- **NPC with conversation:** set `obj.npc_whoami = X` (binds name + conversation). Working recipe:
  `talk.SpawnTemporaryTalker` (`src/interaction/talk.cs:~48`); drive with
  `ConversationVM.StartConversation(obj)`.
- **Gotchas:** player is **index 1** (never overwrite). Static vs mobile use separate free lists;
  `GetAvailableObjectSlot` returns 0 / spawn returns `null` when the list is full — check it. `link`
  doubles as quantity/container-chain; `next` is the tile linked-list pointer. Removal:
  `ObjectFreeLists.ReleaseFreeObject` + tile-unlink (`ObjectRemover`).

### 4d. Strings, conversations, input seams
- **Add a string / NPC line at runtime:** `GameStrings.AddString(blockNo, text)` —
  `src/utility/StringLoader.cs:~826` (already used by `babl_ask`, `identify_inv`,
  `conversationvariables`). Runtime-only (not persisted to `.pak`).
- **Conversation VM:** `ConversationVM.RunConversationVM(talker)` (`src/conversation/conversationvm.cs:~41`)
  — a stack-machine over original bytecode. `conversations[]` is a **mutable** static array; a
  `Conversation` is a plain object (`short[] instuctions`, `StringBlock`, ...). New dialogue-driven
  *commands* are best added as **imported functions** (`cnv_CALLI` → `run_imported_function`; see
  `src/conversation/conversation_functions/babl_hack.cs`).
- **Keyboard input seam:** `main._Input(...)` — `main.cs:~654`; in-game key `switch` at `main.cs:~687`
  (guarded by `!uimanager.blockinput && uimanager.InGame`). **This is where a debug-console launch key
  or new command hooks in.**
- **Proven cheat pattern to copy:** `case Key.Apostrophe:` (`main.cs:~758`) — the `~` full-mage cheat:
  sets `playerdat.max_mana/play_mana/Casting/ManaSkill/play_level`, `SetRune(r,true)` for all runes,
  then `playerdat.PlayerStatusUpdate()`. Recipe = *mutate `playerdat`/world → call
  `PlayerStatusUpdate()`*. Text output: `uimanager.AddToMessageScroll(...)` (see `printVersion` /
  `printPlayerLocation` in `main.cs`).

### 4e. Assets
- Art comes from original `.gr/.byt/.dat` via `GRLoader`/`BytLoader`/`ArtLoader`/`TextureLoader`/
  `PaletteLoader` (`src/loaders/`). Only 2 Godot scenes (`Launch.tscn`, `Underworld.tscn`);
  `resources/` = fonts, icon/splash, shaders, `placeholder_art/` PNGs. Custom PNGs can be assigned to
  Godot nodes directly, but there's no importer to add a new sprite into the `.gr` index. **Respect
  the AI-asset policy — prefer geometry/logic/items/text over generated textures.**

---

## 5. Playbook: building new levels/sections from the baseline

**Mental model:** a level is a **64×64, 2.5-D tile grid** (each tile has a floor *height* → real 3-D
geometry: ramps, pits, multi-height rooms). One level can't exceed 64×64, but that barely constrains
you because of the two facts below. There are **three modes**, and they compose:

| Mode | How | Difficulty |
|---|---|---|
| **A. Remodel in place** | Every level's full 64×64 grid already exists; unreachable areas are just "solid rock" tiles. Change solid→open, drop a door, carve new rooms/secret areas behind an existing wall. | 🟢 easiest |
| **B. Repopulate** | Fill the new space: `spawnObjectInTile` items, traps, and NPCs with custom dialogue (§4c/§4d). | 🟢 easy |
| **C. Add + connect new levels** | Build a fresh 64×64 map (new buffer) and link it to the world via stairs / teleport triggers / moongates (UW2 has room for 80 level slots). | 🟡 medium |

**Creative palette per tile:** layout, floor **heights** (verticality), **textures**,
**water/lava/ice** terrain, **doors/switches/traps/triggers**, **items**, **NPCs + new conversations**.

### Recommended tooling ladder (each step reuses the previous)
1. **Debug console** — hook a key at `main.cs:~687`, open a small text UI, parse commands, dispatch to
   `spawnObjectInTile` / `playerdat` mutators. `spawn <id>`, `give <id>`, `tp <x> <y>`, `heal`,
   `god`, `setlevel <n>`. *Additive, zero fidelity risk, foundation for everything else.*
2. **Live tile-edit commands** — `settile <x> <y> <type>`, `setheight`, `settex`, `putdoor`. Uses the
   `TileInfo` setters + a targeted re-render of the affected tile(s). *This is the "add a door, carve
   a room" tool the owner asked for — usable in-game, in real time.*
3. **Template ↔ level buffer** — an **export** (`current level → human-editable template`) and an
   **import** (`template → buffer`). This is what makes it *chattable*: export the baseline, the owner
   describes edits in chat, you regenerate + hot-reload. Suggested template = an **ASCII map** for tile
   types/heights plus a small **JSON** object/NPC list:
   ```
   # level.map  (T=solid rock, .=floor, D=door, <=stairs up, >=down, ~=water, S=start)
   TTTTTTTTTTTT       # level.json
   T....T.....T       { "start": {"x":2,"y":2},
   T..D<T..M..T   +     "objects":[ {"item":"a_sword","x":3,"y":4} ],
   T....T.....T         "npcs":[ {"item":64,"x":8,"y":6,"whoami":10,"hostile":true} ],
   TTTTTTTTTTTT         "links":[ {"tile":[4,2],"type":"stairs","to_level":5} ] }
   ```
4. **"Blank valid level" initializer** — allocate a zeroed `0x8000` buffer and write the *invariants*
   (free-list pointers @ `0x7C00-0x7C04`, object region @ `0x4000`, `"uw"` magic @ `0x7C06`, a
   `texture_map` — simplest is to copy an existing level's texture block). Zeros alone are not a valid
   level.
5. **"Load tilemap from buffer" seam** — the alternate entry into the load pipeline (bypass
   `LevArkLoader`), so a template-built buffer flows through `BuildTileMapUW` + render like a real
   level. This is the single missing architectural piece for whole-new levels.
6. **Connect** — place a stairs/teleport trigger in an existing level pointing at the new level index.

### The chat workflow the owner wants
> "Start UW2 at the baseline (level 0). *Add a hidden door in the east wall of the first room; behind
> it a corridor to a small treasure vault with a lich.*"
>
> → Mode A/B via the console/tile-edit tools live, **or** export level 0 → template, edit the ASCII map
> + JSON in chat, re-import, reload. Save persists it (round-trip is proven).

**Start with steps 1–2** (console + live tile edits): they deliver the owner's "new door → new section"
dream almost immediately and validate the spawn/tile APIs before investing in the template pipeline.

---

## 6. Quick file index

| File | What it is |
|---|---|
| `main.cs` | Bootstrap + input routing (`_Input` ~654, key switch ~687, `~` cheat ~758) |
| `src/World/tilemap.cs` | `UWTileMap`, `LoadTileMap`, buffer layout, `LevelObjects[1024]` |
| `src/World/tileinfo.cs` | Per-tile bit-field accessors + `ChangeTile` (~540) |
| `src/World/tilemaprender.cs` | Tile → Godot mesh/collision (`GenerateLevelFromTileMap` ~88) |
| `src/World/uwobject.cs` | `uwObject` — thin view over object bytes (item_id, pos, next, link, npc_*) |
| `src/utility/ObjectCreator.cs` | `spawnObjectInTile` (~34), `SpawnObjectInHand`, `GenerateObjects` |
| `src/utility/StringLoader.cs` | `GameStrings` — `AddString` (~826), `GetOrdinal` |
| `src/conversation/conversationvm.cs` | Conversation bytecode interpreter (~41) |
| `src/interaction/talk.cs` | `StartConversation`, `SpawnTemporaryTalker` (~48) |
| `src/savegame/SaveGame.cs` / `LevArkWriter.cs` | Save orchestration + lev.ark writer (uncompressed) |
| `src/loaders/levarkloader.cs` / `dataloader.cs` | lev.ark read + block (de)compression |
| `backlog.md` | Maintainer's living task/bug list (great task source) |
| `docs/uw2-first-pass-notes.md` | Our survey + decisions |
| `docs/save-architecture.md` / `audio-architecture.md` | Deep-dive docs worth reading |

---

## 7. Open decisions / next actions

- [ ] **Verify the 3 UI clip fixes** in-engine (§2) → clears the ⚠️ items.
- [ ] **Build the debug console** (ladder step 1) — recommended first real feature.
- [ ] **Godot 4.4 bump?** A contributor fork (Tzrlk) did it, but it's a big auto-generated diff and
      that fork is otherwise 544 commits behind + already upstreamed. *Recommendation: not now.*
- [ ] **Un-gate UW2 save** (or add a direct-writer test path) when you start persisting custom levels.
- [ ] Consider offering the **`GetOrdinal` fix as an upstream PR** (clean, self-contained, no assets).

## 8. Gotchas checklist
- ⚠️ Player is object **index 1** — never overwrite.
- ⚠️ Free lists return **0/null when full** — always check spawn results before use.
- ⚠️ Items → StaticList, NPCs/projectiles → MobileList (wrong list = uninitialised fields/warnings).
- ⚠️ A "blank" level buffer needs its **free-list pointers + `uw` magic + texture_map**, not just zeros.
- ⚠️ **Texture indices are global-atlas-bound** with per-game offsets; `terrain` (water/lava) is derived
  by looking the floor texture up in `TerrainDatLoader` — arbitrary textures can misbehave.
- ⚠️ Some **level indices trigger hard-coded scripted logic** (`src/World/specificworlds/*`, Ethereal
  ceiling special-case, etc.) — pick a sandbox slot carefully, or add a fresh index within `NO_OF_LEVELS`.
- ⚠️ **UW2 uses the `0x8000` block with an animation-overlay tail**; UW1 uses a separate overlay block.
- ⚠️ Ignore the **UWRepacker** (compression) — broken, not needed, not on the save path.
