using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Underworld
{
    /// <summary>
    /// In-game debug console. Toggle with backtick (`) key.
    /// Type commands and press Enter to execute. Escape to close.
    /// </summary>
    public static class DebugConsole
    {
        /// <summary>
        /// Whether the console is currently accepting input
        /// </summary>
        public static bool IsActive = false;

        /// <summary>
        /// Current text being typed
        /// </summary>
        public static string InputBuffer = "";

        /// <summary>
        /// Command history for up/down arrow recall
        /// </summary>
        private static List<string> _history = new List<string>();
        private static int _historyIndex = -1;

        /// <summary>
        /// God mode flag - prevents damage to player
        /// </summary>
        public static bool GodMode = false;

        /// <summary>
        /// Noclip flag - disables collision
        /// </summary>
        public static bool NoclipMode = false;

        /// <summary>
        /// Toggle the console on/off
        /// </summary>
        public static void Toggle()
        {
            IsActive = !IsActive;
            if (IsActive)
            {
                InputBuffer = "";
                _historyIndex = -1;
                Print("Debug Console. Type 'help' for commands.");
            }
        }

        /// <summary>
        /// Print a message to the game's message scroll
        /// </summary>
        public static void Print(string message)
        {
            uimanager.AddToMessageScroll(message);
        }

        /// <summary>
        /// Handle a key character input while console is active
        /// </summary>
        public static void AppendChar(char c)
        {
            InputBuffer += c;
        }

        /// <summary>
        /// Handle backspace
        /// </summary>
        public static void Backspace()
        {
            if (InputBuffer.Length > 0)
            {
                InputBuffer = InputBuffer.Remove(InputBuffer.Length - 1);
            }
        }

        /// <summary>
        /// Navigate command history (up = -1, down = +1)
        /// </summary>
        public static void NavigateHistory(int direction)
        {
            if (_history.Count == 0) return;

            _historyIndex += direction;
            if (_historyIndex < 0) _historyIndex = 0;
            if (_historyIndex >= _history.Count)
            {
                _historyIndex = _history.Count;
                InputBuffer = "";
                return;
            }
            InputBuffer = _history[_historyIndex];
        }

        /// <summary>
        /// Submit the current input buffer as a command
        /// </summary>
        public static void Submit()
        {
            var input = InputBuffer.Trim();
            InputBuffer = "";

            if (string.IsNullOrEmpty(input)) return;

            // Add to history
            _history.Add(input);
            _historyIndex = _history.Count;

            // Parse and execute
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var command = parts[0].ToLower();
            var args = parts.Skip(1).ToArray();

            try
            {
                ExecuteCommand(command, args);
            }
            catch (Exception ex)
            {
                Print($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Execute a parsed command
        /// </summary>
        private static void ExecuteCommand(string command, string[] args)
        {
            switch (command)
            {
                case "help":
                    ShowHelp(args);
                    break;

                case "spawn":
                    CmdSpawn(args);
                    break;

                case "give":
                    CmdGive(args);
                    break;

                case "tp":
                case "teleport":
                    CmdTeleport(args);
                    break;

                case "heal":
                    CmdHeal();
                    break;

                case "god":
                    CmdGod();
                    break;

                case "noclip":
                    CmdNoclip();
                    break;

                case "kill":
                    CmdKill(args);
                    break;

                case "setlevel":
                case "level":
                    CmdSetLevel(args);
                    break;

                case "info":
                case "status":
                    CmdInfo();
                    break;

                case "pos":
                    CmdPos();
                    break;

                case "settile":
                    CmdSetTile(args);
                    break;

                case "setheight":
                    CmdSetHeight(args);
                    break;

                case "settex":
                    CmdSetTexture(args);
                    break;

                case "putdoor":
                    CmdPutDoor(args);
                    break;

                case "listnpcs":
                case "npcs":
                    CmdListNpcs();
                    break;

                case "listobjs":
                case "objects":
                    CmdListObjects(args);
                    break;

                case "mage":
                    CmdFullMage();
                    break;

                case "setskill":
                    CmdSetSkill(args);
                    break;

                case "sethp":
                    CmdSetHp(args);
                    break;

                case "setmana":
                    CmdSetMana(args);
                    break;

                case "hunger":
                    CmdSetHunger(args);
                    break;

                case "cure":
                    CmdCure();
                    break;

                case "tileinfo":
                    CmdTileInfo(args);
                    break;

                case "objinfo":
                    CmdObjInfo(args);
                    break;

                case "removeobj":
                    CmdRemoveObj(args);
                    break;

                case "setquest":
                    CmdSetQuest(args);
                    break;

                case "getquest":
                    CmdGetQuest(args);
                    break;

                case "newlevel":
                    CmdNewLevel(args);
                    break;

                case "carve":
                    CmdCarveRoom(args);
                    break;

                case "corridor":
                    CmdCarveCorridor(args);
                    break;

                case "rerender":
                    CmdRerender();
                    break;

                case "loadlevel":
                    CmdLoadCustomLevel(args);
                    break;

                case "exportlevel":
                    CmdExportLevel();
                    break;

                case "save":
                    CmdSave(args);
                    break;

                case "weather":
                    CmdWeather(args);
                    break;

                case "time":
                    CmdTime(args);
                    break;

                default:
                    Print($"Unknown command: {command}. Type 'help' for commands.");
                    break;
            }
        }

        // ---- COMMAND IMPLEMENTATIONS ----

        private static void ShowHelp(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "spawn":
                        Print("spawn <item_id> [x y] - Spawn object at tile (or player tile)");
                        return;
                    case "give":
                        Print("give <item_id> - Spawn object in hand");
                        return;
                    case "tp":
                    case "teleport":
                        Print("tp <x> <y> - Teleport to tile coordinates");
                        return;
                    case "settile":
                        Print("settile <x> <y> <type> - Set tile type (0=solid,1=open,2=diag_se,3=diag_sw,4=diag_ne,5=diag_nw,6=slope_n,7=slope_s,8=slope_e,9=slope_w)");
                        return;
                    case "setheight":
                        Print("setheight <x> <y> <h> - Set floor height 0-15");
                        return;
                    case "settex":
                        Print("settex <x> <y> <floor_idx> [wall_idx] - Set tile textures");
                        return;
                    case "setskill":
                        Print("setskill <skill#> <value> - Set skill (0=Attack,1=Defense,2=Unarmed,3=Sword,4=Axe,5=Mace,6=Missile,7=Mana,8=Lore,9=Casting,10=Traps,11=Search,12=Track,13=Sneak,14=Repair,15=Charm,16=Picklock,17=Acrobat,18=Appraise,19=Swimming)");
                        return;
                    case "setquest":
                        Print("setquest <var#> <value> - Set quest variable");
                        return;
                }
            }

            Print("-- Debug Console Commands --");
            Print("spawn <id> [x y] - Spawn object/NPC");
            Print("give <id> - Put item in hand");
            Print("tp <x> <y> - Teleport to tile");
            Print("heal - Full heal + cure");
            Print("god - Toggle god mode");
            Print("noclip - Toggle collision");
            Print("mage - Full mage abilities + runes");
            Print("kill <index> - Kill NPC by index");
            Print("setlevel <n> - Change dungeon level");
            Print("info - Player stats summary");
            Print("pos - Player position");
            Print("settile <x> <y> <type> - Change tile type");
            Print("setheight <x> <y> <h> - Change floor height");
            Print("settex <x> <y> <f> [w] - Change textures");
            Print("putdoor <x> <y> - Place door at tile");
            Print("npcs - List active NPCs");
            Print("objects [radius] - List nearby objects");
            Print("setskill <#> <val> - Set player skill");
            Print("sethp <val> - Set HP");
            Print("setmana <val> - Set mana");
            Print("hunger <val> - Set hunger (0=full)");
            Print("cure - Remove poison/fatigue");
            Print("tileinfo [x y] - Inspect tile");
            Print("objinfo <index> - Inspect object");
            Print("removeobj <index> - Remove object");
            Print("setquest <var> <val> - Set quest var");
            Print("getquest <var> - Read quest var");
            Print("newlevel <slot> [size] - Create blank level");
            Print("carve <x> <y> <w> <h> - Carve room in current");
            Print("corridor <x1> <y1> <x2> <y2> - Carve path");
            Print("rerender - Redraw current level");
            Print("loadlevel <name> - Load custom level");
            Print("exportlevel - Export current as ASCII");
            Print("save [slot] - Force save (1-4)");
            Print("weather [light] - Show/set light 0-7");
            Print("time [hours] - Show/advance time");
            Print("help <cmd> - Detail on a command");
        }

        private static void CmdSpawn(string[] args)
        {
            if (args.Length < 1) { Print("Usage: spawn <item_id> [tileX tileY]"); return; }
            if (!int.TryParse(args[0], out int itemId)) { Print("Invalid item_id"); return; }

            int tileX, tileY;
            if (args.Length >= 3 && int.TryParse(args[1], out tileX) && int.TryParse(args[2], out tileY))
            {
                // spawn at specified tile
            }
            else
            {
                // spawn at player tile
                tileX = playerdat.playerObject.tileX;
                tileY = playerdat.playerObject.tileY;
            }

            var whichList = (itemId >> 6) == 1
                ? ObjectFreeLists.ObjectListType.MobileList
                : ObjectFreeLists.ObjectListType.StaticList;

            var obj = ObjectCreator.spawnObjectInTile(
                itemId, tileX, tileY, 3, 3, 0, whichList);

            if (obj != null)
            {
                Print($"Spawned {obj.a_name} (id:{itemId}) at [{tileX},{tileY}] index:{obj.index}");
            }
            else
            {
                Print("Spawn failed - free list may be full");
            }
        }

        private static void CmdGive(string[] args)
        {
            if (args.Length < 1) { Print("Usage: give <item_id>"); return; }
            if (!int.TryParse(args[0], out int itemId)) { Print("Invalid item_id"); return; }

            int slot = ObjectCreator.SpawnObjectInHand(itemId);
            if (slot != 0)
            {
                var obj = UWTileMap.current_tilemap.LevelObjects[slot];
                Print($"Given {obj.a_name} (id:{itemId}) slot:{slot}");
            }
            else
            {
                Print("Give failed - free list may be full");
            }
        }

        private static void CmdTeleport(string[] args)
        {
            if (args.Length < 2) { Print("Usage: tp <tileX> <tileY>"); return; }
            if (!int.TryParse(args[0], out int tileX) || !int.TryParse(args[1], out int tileY))
            { Print("Invalid coordinates"); return; }

            if (tileX < 0 || tileX > 63 || tileY < 0 || tileY > 63)
            { Print("Coordinates must be 0-63"); return; }

            // Set player position to centre of the target tile
            playerdat.XCoordinate = (tileX << 8) | 0x80;
            playerdat.YCoordinate = (tileY << 8) | 0x80;

            // Update the player object tile references
            playerdat.playerObject.tileX = tileX;
            playerdat.playerObject.tileY = tileY;

            // Get the floor height of the target tile for Z positioning
            var tile = UWTileMap.current_tilemap.Tiles[tileX, tileY];
            playerdat.Z = tile.floorHeight << 3;

            Print($"Teleported to [{tileX},{tileY}] height:{tile.floorHeight}");
        }

        private static void CmdHeal()
        {
            playerdat.play_hp = playerdat.max_hp;
            playerdat.play_mana = playerdat.max_mana;
            playerdat.play_poison = 0;
            playerdat.play_fatigue = 0;
            playerdat.play_hunger = 0;
            playerdat.PlayerStatusUpdate();
            Print($"Healed: HP={playerdat.play_hp}/{playerdat.max_hp} Mana={playerdat.play_mana}/{playerdat.max_mana}");
        }

        private static void CmdGod()
        {
            GodMode = !GodMode;
            Print($"God mode: {(GodMode ? "ON" : "OFF")}");
        }

        private static void CmdNoclip()
        {
            NoclipMode = !NoclipMode;
            Print($"Noclip: {(NoclipMode ? "ON" : "OFF")}");
        }

        private static void CmdKill(string[] args)
        {
            if (args.Length < 1) { Print("Usage: kill <object_index>"); return; }
            if (!int.TryParse(args[0], out int index)) { Print("Invalid index"); return; }

            if (index <= 1 || index >= 1024)
            { Print("Index must be 2-1023 (player is 1, protected)"); return; }

            var obj = UWTileMap.current_tilemap.LevelObjects[index];
            if (obj == null || obj.item_id == 0)
            { Print($"No object at index {index}"); return; }

            obj.npc_hp = 0;
            Print($"Killed {obj.a_name} at index {index}");
        }

        private static void CmdSetLevel(string[] args)
        {
            if (args.Length < 1) { Print("Usage: setlevel <level_number>"); return; }
            if (!int.TryParse(args[0], out int level)) { Print("Invalid level"); return; }

            int maxLevel = UWClass._RES == UWClass.GAME_UW2 ? 79 : 8;
            if (level < 0 || level > maxLevel)
            { Print($"Level must be 0-{maxLevel}"); return; }

            Print($"Changing to level {level}...");
            playerdat.dungeon_level = level;
            // Trigger level load through the normal pipeline
            UWTileMap.LoadTileMap(level, playerdat.currentfolder, false);
        }

        private static void CmdInfo()
        {
            Print($"-- Player Status --");
            Print($"HP: {playerdat.play_hp}/{playerdat.max_hp} Mana: {playerdat.play_mana}/{playerdat.max_mana}");
            Print($"Level: {playerdat.play_level} Dungeon: {playerdat.dungeon_level}");
            Print($"Hunger: {playerdat.play_hunger} Fatigue: {playerdat.play_fatigue} Poison: {playerdat.play_poison}");
            var po = playerdat.playerObject;
            Print($"Tile: [{po.tileX},{po.tileY}] Pos: ({playerdat.XCoordinate},{playerdat.YCoordinate},{playerdat.Z})");
            Print($"God: {(GodMode ? "ON" : "OFF")} Noclip: {(NoclipMode ? "ON" : "OFF")}");
        }

        private static void CmdPos()
        {
            var po = playerdat.playerObject;
            Print($"Tile:[{po.tileX},{po.tileY}] XY:({playerdat.XCoordinate},{playerdat.YCoordinate}) Z:{playerdat.Z} Level:{playerdat.dungeon_level}");
        }

        private static void CmdSetTile(string[] args)
        {
            if (args.Length < 3) { Print("Usage: settile <x> <y> <type>"); return; }
            if (!int.TryParse(args[0], out int x) || !int.TryParse(args[1], out int y) || !int.TryParse(args[2], out int type))
            { Print("Invalid parameters"); return; }

            if (x < 0 || x > 63 || y < 0 || y > 63) { Print("Coords must be 0-63"); return; }
            if (type < 0 || type > 9) { Print("Type must be 0-9 (0=solid,1=open,2-5=diag,6-9=slope)"); return; }

            var tile = UWTileMap.current_tilemap.Tiles[x, y];
            tile.tileType = (short)type;
            Print($"Tile [{x},{y}] type set to {type}");
        }

        private static void CmdSetHeight(string[] args)
        {
            if (args.Length < 3) { Print("Usage: setheight <x> <y> <height>"); return; }
            if (!int.TryParse(args[0], out int x) || !int.TryParse(args[1], out int y) || !int.TryParse(args[2], out int h))
            { Print("Invalid parameters"); return; }

            if (x < 0 || x > 63 || y < 0 || y > 63) { Print("Coords must be 0-63"); return; }
            if (h < 0 || h > 15) { Print("Height must be 0-15"); return; }

            var tile = UWTileMap.current_tilemap.Tiles[x, y];
            tile.floorHeight = (short)h;
            Print($"Tile [{x},{y}] height set to {h}");
        }

        private static void CmdSetTexture(string[] args)
        {
            if (args.Length < 3) { Print("Usage: settex <x> <y> <floor_idx> [wall_idx]"); return; }
            if (!int.TryParse(args[0], out int x) || !int.TryParse(args[1], out int y) || !int.TryParse(args[2], out int floorTex))
            { Print("Invalid parameters"); return; }

            if (x < 0 || x > 63 || y < 0 || y > 63) { Print("Coords must be 0-63"); return; }

            var tile = UWTileMap.current_tilemap.Tiles[x, y];
            tile.floorTexture = (short)floorTex;

            if (args.Length >= 4 && int.TryParse(args[3], out int wallTex))
            {
                tile.wallTexture = (short)wallTex;
                Print($"Tile [{x},{y}] floor tex={floorTex} wall tex={wallTex}");
            }
            else
            {
                Print($"Tile [{x},{y}] floor tex={floorTex}");
            }
        }

        private static void CmdPutDoor(string[] args)
        {
            if (args.Length < 2) { Print("Usage: putdoor <x> <y>"); return; }
            if (!int.TryParse(args[0], out int x) || !int.TryParse(args[1], out int y))
            { Print("Invalid coordinates"); return; }

            if (x < 0 || x > 63 || y < 0 || y > 63) { Print("Coords must be 0-63"); return; }

            // Door item_ids: 0x0140-0x014F are doors in UW
            // Use a standard wooden door (0x0140 = 320)
            int doorId = 0x0140;
            var obj = ObjectCreator.spawnObjectInTile(
                doorId, x, y, 3, 3, 0, ObjectFreeLists.ObjectListType.StaticList);

            if (obj != null)
            {
                // Set the tile's door bit
                var tile = UWTileMap.current_tilemap.Tiles[x, y];
                tile.doorBit = 1;
                Print($"Door placed at [{x},{y}] index:{obj.index}");
            }
            else
            {
                Print("Door placement failed");
            }
        }

        private static void CmdListNpcs()
        {
            if (UWTileMap.current_tilemap == null) { Print("No tilemap loaded"); return; }

            int count = 0;
            for (int i = 2; i < 256; i++) // mobile list is typically indices 2-255
            {
                var obj = UWTileMap.current_tilemap.LevelObjects[i];
                if (obj != null && obj.item_id != 0 && (obj.item_id >> 6) == 1)
                {
                    Print($"  [{i}] {obj.a_name} id:{obj.item_id} hp:{obj.npc_hp} tile:[{obj.tileX},{obj.tileY}]");
                    count++;
                    if (count >= 20) { Print("  ... (truncated at 20)"); break; }
                }
            }
            if (count == 0) Print("No NPCs found on this level");
            else Print($"Total: {count} NPCs");
        }

        private static void CmdListObjects(string[] args)
        {
            if (UWTileMap.current_tilemap == null) { Print("No tilemap loaded"); return; }

            int radius = 3;
            if (args.Length >= 1) int.TryParse(args[0], out radius);

            var po = playerdat.playerObject;
            int px = po.tileX;
            int py = po.tileY;
            int count = 0;

            for (int i = 2; i < 1024; i++)
            {
                var obj = UWTileMap.current_tilemap.LevelObjects[i];
                if (obj != null && obj.item_id != 0)
                {
                    int dx = Math.Abs(obj.tileX - px);
                    int dy = Math.Abs(obj.tileY - py);
                    if (dx <= radius && dy <= radius)
                    {
                        Print($"  [{i}] {obj.a_name} id:{obj.item_id} [{obj.tileX},{obj.tileY}]");
                        count++;
                        if (count >= 30) { Print("  ... (truncated at 30)"); break; }
                    }
                }
            }
            if (count == 0) Print("No objects found nearby");
            else Print($"Total: {count} objects within {radius} tiles");
        }

        private static void CmdFullMage()
        {
            // Same as the apostrophe cheat but more thorough
            playerdat.max_mana = 60;
            playerdat.play_mana = 60;
            playerdat.Casting = 30;
            playerdat.ManaSkill = 30;
            playerdat.play_level = 16;
            for (int r = 0; r < 24; r++)
            {
                playerdat.SetRune(r, true);
            }
            // Also boost combat skills
            for (int s = 0; s <= 6; s++)
            {
                playerdat.SetSkillValue(s, 30);
            }
            playerdat.PlayerStatusUpdate();
            Print("Full mage + combat abilities granted. Level 16, all runes.");
        }

        private static void CmdSetSkill(string[] args)
        {
            if (args.Length < 2) { Print("Usage: setskill <skill#> <value>. Type 'help setskill' for skill list."); return; }
            if (!int.TryParse(args[0], out int skillNo) || !int.TryParse(args[1], out int value))
            { Print("Invalid parameters"); return; }

            if (skillNo < 0 || skillNo > 19) { Print("Skill# must be 0-19"); return; }
            if (value < 0 || value > 63) { Print("Value must be 0-63"); return; }

            playerdat.SetSkillValue(skillNo, value);
            playerdat.PlayerStatusUpdate();
            Print($"Skill {skillNo} set to {value}");
        }

        private static void CmdSetHp(string[] args)
        {
            if (args.Length < 1) { Print("Usage: sethp <value>"); return; }
            if (!int.TryParse(args[0], out int hp)) { Print("Invalid value"); return; }

            playerdat.play_hp = hp;
            playerdat.PlayerStatusUpdate();
            Print($"HP set to {playerdat.play_hp}/{playerdat.max_hp}");
        }

        private static void CmdSetMana(string[] args)
        {
            if (args.Length < 1) { Print("Usage: setmana <value>"); return; }
            if (!int.TryParse(args[0], out int mana)) { Print("Invalid value"); return; }

            playerdat.play_mana = mana;
            playerdat.PlayerStatusUpdate();
            Print($"Mana set to {playerdat.play_mana}/{playerdat.max_mana}");
        }

        private static void CmdSetHunger(string[] args)
        {
            if (args.Length < 1) { Print("Usage: hunger <value> (0=full, 255=starving)"); return; }
            if (!int.TryParse(args[0], out int h)) { Print("Invalid value"); return; }

            playerdat.play_hunger = (byte)h;
            Print($"Hunger set to {playerdat.play_hunger}");
        }

        private static void CmdCure()
        {
            playerdat.play_poison = 0;
            playerdat.play_fatigue = 0;
            playerdat.PlayerStatusUpdate();
            Print("Cured poison and fatigue");
        }

        private static void CmdTileInfo(string[] args)
        {
            int x, y;
            if (args.Length >= 2 && int.TryParse(args[0], out x) && int.TryParse(args[1], out y))
            {
                // use provided coords
            }
            else
            {
                // use player tile
                x = playerdat.playerObject.tileX;
                y = playerdat.playerObject.tileY;
            }

            if (x < 0 || x > 63 || y < 0 || y > 63) { Print("Coords must be 0-63"); return; }

            var tile = UWTileMap.current_tilemap.Tiles[x, y];
            Print($"Tile [{x},{y}]: type={tile.tileType} height={tile.floorHeight}");
            Print($"  floor_tex={tile.floorTexture} wall_tex={tile.wallTexture}");
            Print($"  door={tile.doorBit} objlist_head={tile.indexObjectList}");

            // Walk the object list
            int objIdx = tile.indexObjectList;
            int count = 0;
            while (objIdx != 0 && count < 10)
            {
                var obj = UWTileMap.current_tilemap.LevelObjects[objIdx];
                if (obj == null) break;
                Print($"  obj[{objIdx}]: {obj.a_name} id:{obj.item_id}");
                objIdx = obj.next;
                count++;
            }
        }

        private static void CmdObjInfo(string[] args)
        {
            if (args.Length < 1) { Print("Usage: objinfo <index>"); return; }
            if (!int.TryParse(args[0], out int index)) { Print("Invalid index"); return; }

            if (index < 0 || index >= 1024) { Print("Index must be 0-1023"); return; }

            var obj = UWTileMap.current_tilemap.LevelObjects[index];
            if (obj == null || obj.item_id == 0)
            { Print($"No object at index {index}"); return; }

            Print($"Object [{index}]: {obj.a_name}");
            Print($"  item_id={obj.item_id} tile=[{obj.tileX},{obj.tileY}] pos=({obj.xpos},{obj.ypos},{obj.zpos})");
            Print($"  quality={obj.quality} owner={obj.owner} next={obj.next} link={obj.link}");
            if ((obj.item_id >> 6) == 1) // NPC
            {
                Print($"  NPC: hp={obj.npc_hp} whoami={obj.npc_whoami} attitude={obj.npc_attitude}");
            }
        }

        private static void CmdRemoveObj(string[] args)
        {
            if (args.Length < 1) { Print("Usage: removeobj <index>"); return; }
            if (!int.TryParse(args[0], out int index)) { Print("Invalid index"); return; }

            if (index <= 1) { Print("Cannot remove player (index 1) or null (index 0)"); return; }
            if (index >= 1024) { Print("Index must be 2-1023"); return; }

            var obj = UWTileMap.current_tilemap.LevelObjects[index];
            if (obj == null || obj.item_id == 0)
            { Print($"No object at index {index}"); return; }

            string name = obj.a_name;

            // Unlink from tile's object list
            var tile = UWTileMap.current_tilemap.Tiles[obj.tileX, obj.tileY];
            if (tile.indexObjectList == index)
            {
                tile.indexObjectList = obj.next;
            }
            else
            {
                // Walk the list to find the previous entry
                int prevIdx = tile.indexObjectList;
                while (prevIdx != 0)
                {
                    var prevObj = UWTileMap.current_tilemap.LevelObjects[prevIdx];
                    if (prevObj.next == index)
                    {
                        prevObj.next = obj.next;
                        break;
                    }
                    prevIdx = prevObj.next;
                }
            }

            // Release the slot (also removes Godot node and zeroes item_id)
            ObjectFreeLists.ReleaseFreeObject(obj);

            Print($"Removed {name} from index {index}");
        }

        private static void CmdSetQuest(string[] args)
        {
            if (args.Length < 2) { Print("Usage: setquest <var#> <value>"); return; }
            if (!int.TryParse(args[0], out int varNo) || !int.TryParse(args[1], out int value))
            { Print("Invalid parameters"); return; }

            playerdat.SetQuest(varNo, value);
            Print($"Quest var {varNo} set to {value}");
        }

        private static void CmdGetQuest(string[] args)
        {
            if (args.Length < 1) { Print("Usage: getquest <var#>"); return; }
            if (!int.TryParse(args[0], out int varNo)) { Print("Invalid var#"); return; }

            int value = playerdat.GetQuest(varNo);
            Print($"Quest var {varNo} = {value}");
        }

        private static void CmdNewLevel(string[] args)
        {
            if (args.Length < 1) { Print("Usage: newlevel <slot> [roomSize] - Create blank level with centre room"); return; }
            if (!int.TryParse(args[0], out int slot)) { Print("Invalid slot"); return; }

            int roomSize = 10;
            if (args.Length >= 2) int.TryParse(args[1], out roomSize);

            int roomStart = 32 - roomSize / 2;
            var data = LevelBufferGenerator.CreateRoomLevel(
                roomX: roomStart, roomY: roomStart,
                roomWidth: roomSize, roomHeight: roomSize,
                floorHeight: 2);

            LevelBufferGenerator.LoadCustomLevel(slot, data);

            // Teleport player to centre of the new room
            int centreX = roomStart + roomSize / 2;
            int centreY = roomStart + roomSize / 2;
            playerdat.XCoordinate = (centreX << 8) | 0x80;
            playerdat.YCoordinate = (centreY << 8) | 0x80;
            playerdat.Z = 2 << 3;
            playerdat.playerObject.tileX = centreX;
            playerdat.playerObject.tileY = centreY;
            playerdat.dungeon_level = slot + 1;

            Print($"Created blank level at slot {slot} with {roomSize}x{roomSize} room. Teleported to centre.");
        }

        private static void CmdCarveRoom(string[] args)
        {
            if (args.Length < 4) { Print("Usage: carve <x> <y> <width> <height> [height] [floorTex] [wallTex]"); return; }
            if (!int.TryParse(args[0], out int x) || !int.TryParse(args[1], out int y) ||
                !int.TryParse(args[2], out int w) || !int.TryParse(args[3], out int h))
            { Print("Invalid parameters"); return; }

            int fh = 2, ft = 0, wt = 0;
            if (args.Length >= 5) int.TryParse(args[4], out fh);
            if (args.Length >= 6) int.TryParse(args[5], out ft);
            if (args.Length >= 7) int.TryParse(args[6], out wt);

            if (UWTileMap.current_tilemap == null) { Print("No level loaded"); return; }

            // Modify tiles in the current live tilemap
            for (int ty = y; ty < y + h && ty <= 63; ty++)
            {
                for (int tx = x; tx < x + w && tx <= 63; tx++)
                {
                    var tile = UWTileMap.current_tilemap.Tiles[tx, ty];
                    tile.tileType = (short)UWTileMap.TILE_OPEN;
                    tile.floorHeight = (short)fh;
                    tile.floorTexture = (short)ft;
                    tile.wallTexture = (short)wt;
                }
            }

            Print($"Carved room [{x},{y}] {w}x{h} h={fh} floor={ft} wall={wt}. Use 'rerender' to see changes.");
        }

        private static void CmdCarveCorridor(string[] args)
        {
            if (args.Length < 4) { Print("Usage: corridor <x1> <y1> <x2> <y2> [height] [width]"); return; }
            if (!int.TryParse(args[0], out int x1) || !int.TryParse(args[1], out int y1) ||
                !int.TryParse(args[2], out int x2) || !int.TryParse(args[3], out int y2))
            { Print("Invalid parameters"); return; }

            int fh = 2, cw = 1;
            if (args.Length >= 5) int.TryParse(args[4], out fh);
            if (args.Length >= 6) int.TryParse(args[5], out cw);

            if (UWTileMap.current_tilemap == null) { Print("No level loaded"); return; }

            // Carve horizontal then vertical
            int xMin = Math.Min(x1, x2);
            int xMax = Math.Max(x1, x2);
            for (int x = xMin; x <= xMax; x++)
            {
                for (int w = 0; w < cw; w++)
                {
                    int cy = y1 + w;
                    if (cy >= 0 && cy <= 63)
                    {
                        var tile = UWTileMap.current_tilemap.Tiles[x, cy];
                        tile.tileType = (short)UWTileMap.TILE_OPEN;
                        tile.floorHeight = (short)fh;
                    }
                }
            }
            int yMin = Math.Min(y1, y2);
            int yMax = Math.Max(y1, y2);
            for (int y = yMin; y <= yMax; y++)
            {
                for (int w = 0; w < cw; w++)
                {
                    int cx = x2 + w;
                    if (cx >= 0 && cx <= 63)
                    {
                        var tile = UWTileMap.current_tilemap.Tiles[cx, y];
                        tile.tileType = (short)UWTileMap.TILE_OPEN;
                        tile.floorHeight = (short)fh;
                    }
                }
            }

            Print($"Carved corridor [{x1},{y1}]->[{x2},{y2}] h={fh} w={cw}. Use 'rerender' to see.");
        }

        private static void CmdRerender()
        {
            if (UWTileMap.current_tilemap == null) { Print("No level loaded"); return; }
            UWTileMap.RedrawCurrentTileMap();
            Print("Level re-rendered.");
        }

        private static void CmdSave(string[] args)
        {
            int slot = 1;
            if (args.Length >= 1) int.TryParse(args[0], out slot);
            if (slot < 1 || slot > 4) { Print("Slot must be 1-4"); return; }

            try
            {
                SaveGame.Save(slot, $"Debug save slot {slot}");
                Print($"Game saved to slot {slot}");
            }
            catch (System.Exception ex)
            {
                Print($"Save failed: {ex.Message}");
            }
        }

        private static void CmdWeather(string[] args)
        {
            // Display current game state affecting "weather" (light level, palette)
            Print($"Light level: {playerdat.lightlevel}");
            Print($"Palette: {Palette.CurrentPalette}");
            if (args.Length >= 1 && int.TryParse(args[0], out int newLight))
            {
                if (newLight >= 0 && newLight <= 7)
                {
                    playerdat.lightlevel = newLight;
                    Print($"Light level set to {newLight}");
                }
            }
        }

        private static void CmdTime(string[] args)
        {
            Print($"Game time: {playerdat.game_time} (day {playerdat.game_days}, hour {playerdat.TwelveHourClock})");
            if (args.Length >= 1 && int.TryParse(args[0], out int hours))
            {
                // Advance time by N hours
                playerdat.ClockValue += hours * 0xE1000;
                Print($"Advanced {hours} hours. Now day {playerdat.game_days}, hour {playerdat.TwelveHourClock}");
            }
        }

        private static void CmdLoadCustomLevel(string[] args)
        {
            if (args.Length < 1)
            {
                Print("Usage: loadlevel <name>");
                Print("Available: vault, lich, temple, arena, workshop");
                return;
            }

            string name = args[0].ToLower();
            int slot = CustomLevels.LoadByName(name);
            if (slot < 0)
            {
                Print($"Unknown level: {name}. Try: vault, lich, temple, arena, workshop");
                return;
            }

            var template = CustomLevels.GetTemplate(name);
            if (template != null)
            {
                playerdat.XCoordinate = (template.StartX << 8) | 0x80;
                playerdat.YCoordinate = (template.StartY << 8) | 0x80;
                playerdat.Z = 2 << 3;
                playerdat.playerObject.tileX = template.StartX;
                playerdat.playerObject.tileY = template.StartY;
                playerdat.dungeon_level = slot + 1;
            }

            Print($"Loaded custom level '{name}' at slot {slot}");
        }

        private static void CmdExportLevel()
        {
            var template = LevelTemplate.ExportCurrentLevel();
            if (template == null) { Print("No level loaded to export"); return; }

            // Print a compact view of the map (just the non-solid portion)
            Print($"-- Exported: {template.Name} --");
            for (int i = 0; i < template.MapLines.Length; i++)
            {
                string line = template.MapLines[i].TrimEnd('#');
                if (line.Length > 0 && line.Replace("#", "").Length > 0)
                {
                    Print(line);
                }
            }
            Print($"Start: [{template.StartX},{template.StartY}]");
        }
    }
}
