using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace Underworld
{
    /// <summary>
    /// Level template system for defining custom levels via ASCII maps + JSON metadata.
    ///
    /// ASCII Map Legend:
    ///   T or # = Solid rock
    ///   .      = Open floor (default height)
    ///   0-9    = Open floor at height 0-9
    ///   A-F    = Open floor at height 10-15
    ///   D      = Door tile
    ///   ~      = Water tile
    ///   *      = Lava tile
    ///   /      = Slope north
    ///   \      = Slope south
    ///   [      = Slope east
    ///   ]      = Slope west
    ///   S      = Start position (open floor)
    ///   &lt;      = Stairs up (open floor + marker)
    ///   &gt;      = Stairs down (open floor + marker)
    ///   (space)= Solid (same as T)
    ///
    /// The map is read top-to-bottom = Y=63 down to Y=0 (north at top).
    /// Lines are left-to-right = X=0 to X=63.
    /// </summary>
    public class LevelTemplate
    {
        /// <summary>
        /// The ASCII map lines (up to 64 lines, each up to 64 chars)
        /// </summary>
        public string[] MapLines { get; set; }

        /// <summary>
        /// Default floor height for '.' tiles
        /// </summary>
        public int DefaultFloorHeight { get; set; } = 2;

        /// <summary>
        /// Default floor texture index
        /// </summary>
        public int DefaultFloorTexture { get; set; } = 0;

        /// <summary>
        /// Default wall texture index
        /// </summary>
        public int DefaultWallTexture { get; set; } = 0;

        /// <summary>
        /// Water floor texture index
        /// </summary>
        public int WaterTexture { get; set; } = 6;

        /// <summary>
        /// Lava floor texture index
        /// </summary>
        public int LavaTexture { get; set; } = 7;

        /// <summary>
        /// Player start position (from 'S' in map, or explicit)
        /// </summary>
        public int StartX { get; set; } = 32;
        public int StartY { get; set; } = 32;

        /// <summary>
        /// Objects to spawn in the level
        /// </summary>
        public List<TemplateObject> Objects { get; set; } = new List<TemplateObject>();

        /// <summary>
        /// NPCs to spawn
        /// </summary>
        public List<TemplateNpc> Npcs { get; set; } = new List<TemplateNpc>();

        /// <summary>
        /// Level connections (stairs, teleporters)
        /// </summary>
        public List<TemplateLink> Links { get; set; } = new List<TemplateLink>();

        /// <summary>
        /// Room definitions with names (for documentation/navigation)
        /// </summary>
        public List<TemplateRoom> Rooms { get; set; } = new List<TemplateRoom>();

        /// <summary>
        /// Template name/description
        /// </summary>
        public string Name { get; set; } = "Custom Level";
        public string Description { get; set; } = "";

        /// <summary>
        /// Converts this template to a valid level buffer (0x8000 bytes).
        /// </summary>
        public byte[] ToLevelBuffer()
        {
            var data = LevelBufferGenerator.CreateBlankLevel();

            if (MapLines == null || MapLines.Length == 0)
            {
                Debug.Print("LevelTemplate: no map lines, returning blank level");
                return data;
            }

            // Parse the ASCII map
            // Map line 0 = top of map = Y=63 (north), last line = Y=0 (south)
            for (int lineIdx = 0; lineIdx < MapLines.Length && lineIdx < 64; lineIdx++)
            {
                int tileY = 63 - lineIdx; // flip so top of text = north = high Y
                string line = MapLines[lineIdx];

                for (int charIdx = 0; charIdx < line.Length && charIdx < 64; charIdx++)
                {
                    int tileX = charIdx;
                    char c = line[charIdx];
                    ParseTileChar(data, tileX, tileY, c);
                }
            }

            return data;
        }

        /// <summary>
        /// Parse a single character from the ASCII map into tile data.
        /// </summary>
        private void ParseTileChar(byte[] data, int x, int y, char c)
        {
            switch (c)
            {
                case 'T':
                case '#':
                case ' ':
                    // Solid rock (default zero = solid)
                    break;

                case '.':
                    LevelBufferGenerator.SetTileInBuffer(data, x, y,
                        UWTileMap.TILE_OPEN, DefaultFloorHeight, DefaultFloorTexture, DefaultWallTexture);
                    break;

                case 'S':
                    LevelBufferGenerator.SetTileInBuffer(data, x, y,
                        UWTileMap.TILE_OPEN, DefaultFloorHeight, DefaultFloorTexture, DefaultWallTexture);
                    StartX = x;
                    StartY = y;
                    break;

                case 'D':
                    LevelBufferGenerator.SetTileInBuffer(data, x, y,
                        UWTileMap.TILE_OPEN, DefaultFloorHeight, DefaultFloorTexture, DefaultWallTexture);
                    // Door bit is set in byte 1, bit 7
                    int doorPtr = x * 4 + y * 256;
                    data[doorPtr + 1] |= 0x80;
                    break;

                case '~':
                    LevelBufferGenerator.SetTileInBuffer(data, x, y,
                        UWTileMap.TILE_OPEN, 0, WaterTexture, DefaultWallTexture);
                    break;

                case '*':
                    LevelBufferGenerator.SetTileInBuffer(data, x, y,
                        UWTileMap.TILE_OPEN, 0, LavaTexture, DefaultWallTexture);
                    break;

                case '/':
                    LevelBufferGenerator.SetTileInBuffer(data, x, y,
                        UWTileMap.TILE_SLOPE_N, DefaultFloorHeight, DefaultFloorTexture, DefaultWallTexture);
                    break;
                case '\\':
                    LevelBufferGenerator.SetTileInBuffer(data, x, y,
                        UWTileMap.TILE_SLOPE_S, DefaultFloorHeight, DefaultFloorTexture, DefaultWallTexture);
                    break;
                case '[':
                    LevelBufferGenerator.SetTileInBuffer(data, x, y,
                        UWTileMap.TILE_SLOPE_E, DefaultFloorHeight, DefaultFloorTexture, DefaultWallTexture);
                    break;
                case ']':
                    LevelBufferGenerator.SetTileInBuffer(data, x, y,
                        UWTileMap.TILE_SLOPE_W, DefaultFloorHeight, DefaultFloorTexture, DefaultWallTexture);
                    break;

                case '<':
                case '>':
                    LevelBufferGenerator.SetTileInBuffer(data, x, y,
                        UWTileMap.TILE_OPEN, DefaultFloorHeight, DefaultFloorTexture, DefaultWallTexture);
                    break;

                // Height digits: 0-9 = open floor at that height
                case >= '0' and <= '9':
                    LevelBufferGenerator.SetTileInBuffer(data, x, y,
                        UWTileMap.TILE_OPEN, c - '0', DefaultFloorTexture, DefaultWallTexture);
                    break;

                // Height hex: A-F = heights 10-15
                case >= 'A' and <= 'F':
                    LevelBufferGenerator.SetTileInBuffer(data, x, y,
                        UWTileMap.TILE_OPEN, 10 + (c - 'A'), DefaultFloorTexture, DefaultWallTexture);
                    break;

                default:
                    // Unknown char = solid
                    break;
            }
        }

        /// <summary>
        /// Spawns all objects and NPCs defined in this template into the current tilemap.
        /// Call after the level buffer has been loaded via LoadCustomLevel.
        /// </summary>
        public void SpawnContent()
        {
            if (UWTileMap.current_tilemap == null)
            {
                Debug.Print("LevelTemplate.SpawnContent: no tilemap loaded");
                return;
            }

            foreach (var obj in Objects)
            {
                var spawned = ObjectCreator.spawnObjectInTile(
                    obj.ItemId, obj.TileX, obj.TileY,
                    (short)(obj.PosX), (short)(obj.PosY), (short)(obj.PosZ),
                    ObjectFreeLists.ObjectListType.StaticList);

                if (spawned != null && obj.Quality > 0)
                {
                    spawned.quality = (short)obj.Quality;
                }
                if (spawned != null && obj.Quantity > 0)
                {
                    spawned.link = (short)obj.Quantity;
                }
            }

            foreach (var npc in Npcs)
            {
                var spawned = ObjectCreator.spawnObjectInTile(
                    npc.ItemId, npc.TileX, npc.TileY,
                    (short)(npc.PosX), (short)(npc.PosY), (short)(npc.PosZ),
                    ObjectFreeLists.ObjectListType.MobileList);

                if (spawned != null)
                {
                    if (npc.WhoAmI >= 0) spawned.npc_whoami = (short)npc.WhoAmI;
                    if (npc.Hp > 0) spawned.npc_hp = (byte)npc.Hp;
                    if (npc.Attitude >= 0) spawned.npc_attitude = (short)npc.Attitude;
                }
            }

            Debug.Print($"LevelTemplate '{Name}': spawned {Objects.Count} objects, {Npcs.Count} NPCs");
        }

        /// <summary>
        /// Load a template from a JSON file.
        /// </summary>
        public static LevelTemplate LoadFromFile(string path)
        {
            if (!File.Exists(path))
            {
                Debug.Print($"Template file not found: {path}");
                return null;
            }

            var json = File.ReadAllText(path);
            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<LevelTemplate>(json, opts);
        }

        /// <summary>
        /// Save this template to a JSON file.
        /// </summary>
        public void SaveToFile(string path)
        {
            var opts = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(this, opts);
            File.WriteAllText(path, json);
            Debug.Print($"Template saved to: {path}");
        }

        /// <summary>
        /// Create a template from an inline ASCII map string.
        /// Each line is separated by newlines.
        /// </summary>
        public static LevelTemplate FromAsciiMap(string name, string asciiMap,
            int defaultFloorHeight = 2, int defaultFloorTexture = 0, int defaultWallTexture = 0)
        {
            var template = new LevelTemplate
            {
                Name = name,
                DefaultFloorHeight = defaultFloorHeight,
                DefaultFloorTexture = defaultFloorTexture,
                DefaultWallTexture = defaultWallTexture,
                MapLines = asciiMap.Split('\n', StringSplitOptions.None)
            };
            return template;
        }

        /// <summary>
        /// Export the current live tilemap as a template (for editing and reimporting).
        /// Only exports non-solid tiles to keep the output compact.
        /// </summary>
        public static LevelTemplate ExportCurrentLevel()
        {
            if (UWTileMap.current_tilemap == null) return null;

            var template = new LevelTemplate
            {
                Name = $"Level {UWTileMap.current_tilemap.thisLevelNo} Export",
                MapLines = new string[64]
            };

            for (int y = 63; y >= 0; y--)
            {
                var line = new char[64];
                for (int x = 0; x < 64; x++)
                {
                    var tile = UWTileMap.current_tilemap.Tiles[x, y];
                    if (tile == null || tile.tileType == UWTileMap.TILE_SOLID)
                    {
                        line[x] = '#';
                    }
                    else
                    {
                        int h = tile.floorHeight;
                        switch (tile.tileType)
                        {
                            case UWTileMap.TILE_OPEN:
                                line[x] = h <= 9 ? (char)('0' + h) : (char)('A' + h - 10);
                                break;
                            case UWTileMap.TILE_SLOPE_N: line[x] = '/'; break;
                            case UWTileMap.TILE_SLOPE_S: line[x] = '\\'; break;
                            case UWTileMap.TILE_SLOPE_E: line[x] = '['; break;
                            case UWTileMap.TILE_SLOPE_W: line[x] = ']'; break;
                            case UWTileMap.TILE_DIAG_SE:
                            case UWTileMap.TILE_DIAG_SW:
                            case UWTileMap.TILE_DIAG_NE:
                            case UWTileMap.TILE_DIAG_NW:
                                line[x] = 'X'; // diagonal walls
                                break;
                            default:
                                line[x] = '?';
                                break;
                        }

                        // Override for special tiles
                        if (tile.doorBit != 0) line[x] = 'D';
                    }
                }
                template.MapLines[63 - y] = new string(line);
            }

            // Record player start
            var po = playerdat.playerObject;
            template.StartX = po.tileX;
            template.StartY = po.tileY;

            return template;
        }
    }

    /// <summary>
    /// An object to spawn in a template level
    /// </summary>
    public class TemplateObject
    {
        public int ItemId { get; set; }
        public int TileX { get; set; }
        public int TileY { get; set; }
        public int PosX { get; set; } = 3;
        public int PosY { get; set; } = 3;
        public int PosZ { get; set; } = 0;
        public int Quality { get; set; } = 0;
        public int Quantity { get; set; } = 0;
    }

    /// <summary>
    /// An NPC to spawn in a template level
    /// </summary>
    public class TemplateNpc
    {
        public int ItemId { get; set; }
        public int TileX { get; set; }
        public int TileY { get; set; }
        public int PosX { get; set; } = 3;
        public int PosY { get; set; } = 3;
        public int PosZ { get; set; } = 0;
        public int WhoAmI { get; set; } = -1;
        public int Hp { get; set; } = 0;
        public int Attitude { get; set; } = -1; // -1 = default
    }

    /// <summary>
    /// A level connection (stairs, teleporter)
    /// </summary>
    public class TemplateLink
    {
        public int TileX { get; set; }
        public int TileY { get; set; }
        public string Type { get; set; } = "stairs"; // stairs, teleport, moongate
        public int ToLevel { get; set; }
        public int ToX { get; set; }
        public int ToY { get; set; }
    }

    /// <summary>
    /// Named room region (for documentation)
    /// </summary>
    public class TemplateRoom
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Description { get; set; }
    }
}
