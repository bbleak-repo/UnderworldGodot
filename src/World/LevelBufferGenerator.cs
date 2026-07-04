using System;
using System.Diagnostics;

namespace Underworld
{
    /// <summary>
    /// Generates valid level buffers for custom levels and provides the seam
    /// to load them into the tilemap pipeline without reading from LEV.ARK.
    ///
    /// Buffer layout (0x8000 = 32768 bytes for UW2):
    ///   0x0000-0x3FFF : Tile data (64x64 tiles, 4 bytes each)
    ///   0x4000-0x72FF : Object data (1024 objects, 8 bytes static / 27 bytes mobile)
    ///   0x7300-0x74FB : Mobile free list (256 entries x 2 bytes)
    ///   0x74FC-0x7AFB : Static free list (~768 entries x 2 bytes)
    ///   0x7AFC-0x7BFF : Active mobile list (256 entries x 1 byte)
    ///   0x7C00-0x7C01 : Number of active mobiles (16-bit)
    ///   0x7C02-0x7C03 : Mobile free list pointer (16-bit)
    ///   0x7C04-0x7C05 : Static free list pointer (16-bit)
    ///   0x7C06-0x7C07 : "uw" magic signature
    /// </summary>
    public static class LevelBufferGenerator
    {
        /// <summary>
        /// Size of a level data block
        /// </summary>
        public const int BLOCK_SIZE = 0x8000; // 32768

        /// <summary>
        /// Creates a blank, valid level buffer with all tiles set to solid rock.
        /// The buffer has properly initialized free lists, "uw" magic, and can be
        /// loaded through BuildTileMapUW without errors.
        /// </summary>
        /// <returns>A valid 0x8000-byte level buffer</returns>
        public static byte[] CreateBlankLevel()
        {
            var data = new byte[BLOCK_SIZE];

            // All tiles start as solid (type 0) with height 0, which is just zero bytes.
            // That's already the default from the zeroed array.

            // Initialize the mobile free list (indices 2-255, since 0=null, 1=player)
            // Free list entries at 0x7300, each 2 bytes (16-bit object index)
            int mobileFreeListBase = 0x7300;
            for (int i = 0; i < 254; i++) // 254 slots (indices 2-255)
            {
                int objectIndex = i + 2;
                data[mobileFreeListBase + i * 2] = (byte)(objectIndex & 0xFF);
                data[mobileFreeListBase + i * 2 + 1] = (byte)((objectIndex >> 8) & 0xFF);
            }

            // Initialize the static free list (indices 256-1023)
            // Free list entries at 0x74FC, each 2 bytes
            int staticFreeListBase = 0x74FC;
            for (int i = 0; i < 768; i++) // 768 slots (indices 256-1023)
            {
                int objectIndex = i + 256;
                data[staticFreeListBase + i * 2] = (byte)(objectIndex & 0xFF);
                data[staticFreeListBase + i * 2 + 1] = (byte)((objectIndex >> 8) & 0xFF);
            }

            // Active mobiles count = 0
            data[0x7C00] = 0;
            data[0x7C01] = 0;

            // Mobile free list pointer = 254 (all 254 slots available)
            data[0x7C02] = 254 & 0xFF;
            data[0x7C03] = 0;

            // Static free list pointer = 768 (all 768 slots available)
            data[0x7C04] = 0x00; // 768 & 0xFF = 0
            data[0x7C05] = 0x03; // 768 >> 8 = 3

            // "uw" magic signature (stored as "wu" in little-endian at 0x7C06-0x7C07)
            data[0x7C06] = (byte)'w';
            data[0x7C07] = (byte)'u';

            return data;
        }

        /// <summary>
        /// Creates a simple starter level with a rectangular room at the centre.
        /// Good for testing and as a base for custom level design.
        /// </summary>
        /// <param name="roomX">Room left edge (tile X)</param>
        /// <param name="roomY">Room bottom edge (tile Y)</param>
        /// <param name="roomWidth">Room width in tiles</param>
        /// <param name="roomHeight">Room height in tiles</param>
        /// <param name="floorHeight">Floor height 0-15</param>
        /// <param name="floorTexture">Floor texture index</param>
        /// <param name="wallTexture">Wall texture index</param>
        /// <returns>A valid level buffer with the room carved out</returns>
        public static byte[] CreateRoomLevel(
            int roomX = 28, int roomY = 28,
            int roomWidth = 8, int roomHeight = 8,
            int floorHeight = 2,
            int floorTexture = 0, int wallTexture = 0)
        {
            var data = CreateBlankLevel();

            // Carve the room: set tiles to TILE_OPEN (type 1) with floor height
            for (int y = roomY; y < roomY + roomHeight && y <= 63; y++)
            {
                for (int x = roomX; x < roomX + roomWidth && x <= 63; x++)
                {
                    SetTileInBuffer(data, x, y,
                        tileType: UWTileMap.TILE_OPEN,
                        floorHeight: floorHeight,
                        floorTexture: floorTexture,
                        wallTexture: wallTexture);
                }
            }

            return data;
        }

        /// <summary>
        /// Sets a single tile's properties in a raw level buffer.
        ///
        /// Tile byte layout (4 bytes per tile at offset tileX*4 + tileY*256):
        ///   Byte 0: bits 0-3 = tile type, bits 4-7 = floor height
        ///   Byte 1: bits 0-3 = floor texture (low), bit 4-5 = ?, bit 6 = ?, bit 7 = door bit
        ///   Byte 2-3: bits 6-15 = wall texture (10 bits), bits 0-5 = first object index (low 6 bits)
        ///             (full object index is 10 bits spanning bytes 1-2)
        /// </summary>
        public static void SetTileInBuffer(byte[] data, int tileX, int tileY,
            int tileType, int floorHeight,
            int floorTexture = 0, int wallTexture = 0)
        {
            int ptr = tileX * 4 + tileY * 256;
            if (ptr + 3 >= data.Length) return;

            // Byte 0: tile type (bits 0-3) | floor height (bits 4-7)
            data[ptr] = (byte)((tileType & 0x0F) | ((floorHeight & 0x0F) << 4));

            // Byte 1: floor texture in lower 4 bits, keep upper bits (door, flags)
            data[ptr + 1] = (byte)(floorTexture & 0x0F);

            // Bytes 2-3: wall texture in upper 10 bits of the 16-bit value
            // Object list index starts at 0 (no objects)
            int wallVal = (wallTexture & 0x3F) | 0; // wall in lower 6 bits, obj index = 0
            data[ptr + 2] = (byte)(wallVal & 0xFF);
            data[ptr + 3] = (byte)((wallVal >> 8) & 0xFF);
        }

        /// <summary>
        /// Creates a default texture map block for a custom level.
        /// Maps texture indices 0-47 to wall textures and 48-57 to floor textures.
        /// This is a minimal identity mapping -- textures map to themselves.
        /// </summary>
        public static byte[] CreateDefaultTextureMap()
        {
            // Texture map block size varies by game:
            // UW1: 64 entries x 2 bytes = 128 bytes
            // UW2: 70 entries x 2 bytes = 140 bytes
            int mapSize = UWClass._RES == UWClass.GAME_UW2 ? 70 : 64;
            var texData = new byte[mapSize * 2];

            for (int i = 0; i < mapSize; i++)
            {
                // Identity mapping: texture index i maps to texture i
                texData[i * 2] = (byte)(i & 0xFF);
                texData[i * 2 + 1] = (byte)((i >> 8) & 0xFF);
            }

            return texData;
        }

        /// <summary>
        /// Loads a custom level from a raw buffer into a specified level slot,
        /// bypassing the normal LEV.ARK file loading.
        ///
        /// This is the key "seam" -- it plugs a hand-built buffer into the
        /// normal BuildTileMapUW + render pipeline so the result is a fully
        /// interactive, playable level.
        /// </summary>
        /// <param name="levelNo">The level slot to load into (0-based)</param>
        /// <param name="levelData">The 0x8000-byte level buffer</param>
        /// <param name="textureData">Optional texture map data (uses defaults if null)</param>
        public static void LoadCustomLevel(int levelNo, byte[] levelData, byte[] textureData = null)
        {
            if (levelData == null || levelData.Length < BLOCK_SIZE)
            {
                Debug.Print($"Invalid level data: expected {BLOCK_SIZE} bytes, got {levelData?.Length ?? 0}");
                return;
            }

            // Ensure dungeons array is initialized
            if (UWTileMap.dungeons == null)
            {
                UWTileMap.dungeons = new UWTileMap[UWTileMap.NO_OF_LEVELS];
            }

            // Create a new tilemap for this level slot, bypassing LevArkLoader
            var tilemap = new UWTileMap(levelNo, levelData, textureData);
            UWTileMap.dungeons[levelNo] = tilemap;
            UWTileMap.current_tilemap = tilemap;

            // Build the tilemap from the buffer
            tilemap.BuildTileMapUW(
                levelNo: levelNo,
                tex_ark: tilemap.tex_ark_block,
                ovl_ark: tilemap.ovl_ark_block);

            // Generate objects from the buffer's object data
            ObjectCreator.GenerateObjects(
                objects: tilemap.LevelObjects,
                a_tilemap: tilemap);

            Debug.Print($"Custom level {levelNo} loaded from buffer ({levelData.Length} bytes)");
        }

        /// <summary>
        /// Carves a corridor between two points in a level buffer.
        /// Uses simple L-shaped routing (horizontal then vertical).
        /// </summary>
        public static void CarveCorridor(byte[] data,
            int x1, int y1, int x2, int y2,
            int floorHeight = 2, int floorTexture = 0, int wallTexture = 0,
            int corridorWidth = 1)
        {
            // Horizontal segment
            int xMin = Math.Min(x1, x2);
            int xMax = Math.Max(x1, x2);
            for (int x = xMin; x <= xMax; x++)
            {
                for (int w = 0; w < corridorWidth; w++)
                {
                    int y = y1 + w;
                    if (y >= 0 && y <= 63)
                    {
                        SetTileInBuffer(data, x, y,
                            UWTileMap.TILE_OPEN, floorHeight, floorTexture, wallTexture);
                    }
                }
            }

            // Vertical segment
            int yMin = Math.Min(y1, y2);
            int yMax = Math.Max(y1, y2);
            for (int y = yMin; y <= yMax; y++)
            {
                for (int w = 0; w < corridorWidth; w++)
                {
                    int x = x2 + w;
                    if (x >= 0 && x <= 63)
                    {
                        SetTileInBuffer(data, x, y,
                            UWTileMap.TILE_OPEN, floorHeight, floorTexture, wallTexture);
                    }
                }
            }
        }

        /// <summary>
        /// Carves a rectangular room in a level buffer.
        /// </summary>
        public static void CarveRoom(byte[] data,
            int x, int y, int width, int height,
            int floorHeight = 2, int floorTexture = 0, int wallTexture = 0)
        {
            for (int ty = y; ty < y + height && ty <= 63; ty++)
            {
                for (int tx = x; tx < x + width && tx <= 63; tx++)
                {
                    SetTileInBuffer(data, tx, ty,
                        UWTileMap.TILE_OPEN, floorHeight, floorTexture, wallTexture);
                }
            }
        }

        /// <summary>
        /// Sets a tile to a slope type with start and end heights.
        /// The slope direction determines which edge is high vs low.
        /// </summary>
        public static void SetSlopeTile(byte[] data, int x, int y,
            int slopeDirection, int floorHeight,
            int floorTexture = 0, int wallTexture = 0)
        {
            // slopeDirection: 6=slope_n, 7=slope_s, 8=slope_e, 9=slope_w
            SetTileInBuffer(data, x, y, slopeDirection, floorHeight, floorTexture, wallTexture);
        }

        /// <summary>
        /// Sets a water/lava tile. In UW, water/lava is determined by the floor texture
        /// index matching entries in TerrainDatLoader.
        /// Texture indices for water/lava vary by game -- use the texture_map to find them.
        /// </summary>
        public static void SetWaterTile(byte[] data, int x, int y,
            int floorHeight = 0, int waterTextureIndex = 0, int wallTexture = 0)
        {
            SetTileInBuffer(data, x, y,
                UWTileMap.TILE_OPEN, floorHeight, waterTextureIndex, wallTexture);
        }
    }
}
