using System.Collections.Generic;

namespace Underworld
{
    /// <summary>
    /// Built-in custom level definitions for the fork.
    /// These levels extend the base game with new areas accessible
    /// via the debug console ('newlevel' command) or level connection system.
    ///
    /// Level slot assignments (UW2 has 80 slots, base game uses ~16):
    ///   Slot 70: The Forgotten Vault
    ///   Slot 71: Lich's Domain
    ///   Slot 72: The Sunken Temple
    ///   Slot 73: Arena of Trials
    ///   Slot 74: The Artificer's Workshop
    /// </summary>
    public static class CustomLevels
    {
        /// <summary>
        /// Load a custom level by name. Returns the level slot or -1 if not found.
        /// </summary>
        public static int LoadByName(string name)
        {
            var template = GetTemplate(name);
            if (template == null) return -1;

            int slot = GetSlot(name);
            var buffer = template.ToLevelBuffer();
            LevelBufferGenerator.LoadCustomLevel(slot, buffer);
            template.SpawnContent();
            return slot;
        }

        /// <summary>
        /// Get the level slot for a named level.
        /// </summary>
        public static int GetSlot(string name)
        {
            switch (name.ToLower())
            {
                case "vault": return 70;
                case "lich": return 71;
                case "temple": return 72;
                case "arena": return 73;
                case "workshop": return 74;
                default: return -1;
            }
        }

        /// <summary>
        /// Get a template by name.
        /// </summary>
        public static LevelTemplate GetTemplate(string name)
        {
            switch (name.ToLower())
            {
                case "vault": return CreateForgottenVault();
                case "lich": return CreateLichsDomain();
                case "temple": return CreateSunkenTemple();
                case "arena": return CreateArenaOfTrials();
                case "workshop": return CreateArtificersWorkshop();
                default: return null;
            }
        }

        /// <summary>
        /// Level 1: The Forgotten Vault
        /// A hidden treasure vault beneath the castle. Features a grand entry
        /// corridor leading to a columned great hall, side passages at varying
        /// heights, a water-filled cistern, secret alcoves, and a raised
        /// treasure vault behind a locked door.
        /// Difficulty: Easy. Focus: exploration, treasure.
        /// </summary>
        public static LevelTemplate CreateForgottenVault()
        {
            // Map coordinate reference:
            //   Line 0 = Y=63 (north/top), Line 63 = Y=0 (south/bottom)
            //   Column index = X directly
            //
            // Layout (north to south):
            //   Row 3-8 (Y60-55): Upper secret alcoves and cistern
            //   Row 9-14 (Y54-49): Grand columned hall (height 2) with side wings
            //   Row 15-20 (Y48-43): Entry corridor (east) and treasure vault (west)
            //   Row 21-26 (Y42-37): Lower passages at varying heights
            //   Row 27-34 (Y36-29): Winding lower tunnels and hidden chamber
            var map = @"
################################################################
################################################################
################################################################
#########...........############################################
#########.1.......1.############################################
#########...1...1...############################################
#########.1.......1.#####~~~~~~~~~~~############################
#########...1...1...#####~~~~~~~~~~~############################
#########.1.......1.#####~~~~~~~~~~~############################
####.........D..........D~~~~~~~~~~~############################
####.2222222222222222222.#####D#####....########################
####.2................2..#####.#####.33.########################
####.2.#2#.......#2#..2..#####.#####.33.########################
####.2................2..#####.#####.33.########################
####.2.#2#.......#2#..2..#####.#####....########################
####.2................2..######.####.33.########################
####.2.#2#.......#2#..2..#####..####....########################
####.2................2.......D.........########################
####.22222D222222222222..#####..################################
####.........S..........D#####.#################################
####.................1..####.......#############################
####..1..............1..####.444D4.#############################
####.................1..####.4...4.#############################
####..1...........D.....####.4...4.#############################
####.................3..####.44444.#############################
####..1..............3..####.......#############################
####.................3..########################################
####...D................########################################
########.......1........########################################
########..1.........1...########################################
########.......1........########################################
########..1.........1...########################################
########................########################################
########.......1........########################################
########..1..........D..########################################
################.............###################################
################..1..........###################################
################.............###################################
################..1.....1....###################################
################.............###################################
################.............###################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################".TrimStart('\n');

            var template = LevelTemplate.FromAsciiMap(
                "The Forgotten Vault", map,
                defaultFloorHeight: 2, defaultFloorTexture: 1, defaultWallTexture: 2);

            template.Description = "A hidden vault beneath the castle, long sealed behind a collapsed wall. " +
                "Ancient treasures lie within, guarded by restless spirits and cunning traps. " +
                "A grand columned hall dominates the center, with side passages at varying " +
                "heights and a water-filled cistern to the east. The inner vault, raised " +
                "high above the floor, holds the greatest prizes -- if you can find the key.";

            // Start position: entry corridor, line 19 col 13 => X=13, Y=63-19=44
            template.StartX = 13;
            template.StartY = 44;

            template.Objects = new List<TemplateObject>
            {
                // --- Entry corridor area (Y=44 region) ---
                // Torch near start for immediate light
                new TemplateObject { ItemId = 0x0090, TileX = 5, TileY = 44, PosX = 3, PosY = 3 },
                // Dagger on the ground near entrance
                new TemplateObject { ItemId = 0x0000, TileX = 10, TileY = 44, PosX = 3, PosY = 3 },
                // Food rations near entrance
                new TemplateObject { ItemId = 0x00C0, TileX = 7, TileY = 44, PosX = 5, PosY = 3 },

                // --- Grand Columned Hall (Y=49-54, X=4-23) ---
                // Sword on the hall floor
                new TemplateObject { ItemId = 0x0004, TileX = 10, TileY = 52, PosX = 3, PosY = 3 },
                // Chain armor behind a column
                new TemplateObject { ItemId = 0x0014, TileX = 18, TileY = 50, PosX = 3, PosY = 3 },
                // Scattered gold coins in the hall
                new TemplateObject { ItemId = 0x00A0, TileX = 8, TileY = 53, PosX = 2, PosY = 2, Quantity = 15 },
                new TemplateObject { ItemId = 0x00A0, TileX = 14, TileY = 51, PosX = 5, PosY = 5, Quantity = 25 },
                // Torch on hall wall
                new TemplateObject { ItemId = 0x0091, TileX = 5, TileY = 54, PosX = 3, PosY = 3 },
                // Healing potion near column
                new TemplateObject { ItemId = 0x0110, TileX = 12, TileY = 49, PosX = 3, PosY = 3 },

                // --- Cistern room (Y=54-57, X=25-35) water area ---
                // Gem hidden by water edge
                new TemplateObject { ItemId = 0x00A8, TileX = 30, TileY = 54, PosX = 3, PosY = 3 },

                // --- Side passage east at height 3 (Y=49-53, X=29-34) ---
                // Key for the vault door
                new TemplateObject { ItemId = 0x00D0, TileX = 30, TileY = 52, PosX = 3, PosY = 3 },
                // Another torch
                new TemplateObject { ItemId = 0x0092, TileX = 30, TileY = 49, PosX = 3, PosY = 3 },

                // --- Lower side passages (Y=42-38) ---
                // Mace in lower passage
                new TemplateObject { ItemId = 0x0005, TileX = 6, TileY = 42, PosX = 3, PosY = 3 },
                // Potion in lower passage
                new TemplateObject { ItemId = 0x0101, TileX = 8, TileY = 40, PosX = 3, PosY = 3 },
                // Food in lower area
                new TemplateObject { ItemId = 0x00C1, TileX = 12, TileY = 38, PosX = 3, PosY = 3 },

                // --- Treasure Vault (height 4 area, Y=39-41, X=29-33) ---
                // Gold piles in the vault
                new TemplateObject { ItemId = 0x00A0, TileX = 30, TileY = 41, PosX = 2, PosY = 2, Quantity = 75 },
                new TemplateObject { ItemId = 0x00A0, TileX = 32, TileY = 41, PosX = 5, PosY = 5, Quantity = 50 },
                // Gem in the vault
                new TemplateObject { ItemId = 0x00A8, TileX = 31, TileY = 40, PosX = 3, PosY = 3 },
                // Fine shield in vault
                new TemplateObject { ItemId = 0x0010, TileX = 31, TileY = 39, PosX = 3, PosY = 3 },

                // --- Winding tunnels (Y=29-36) ---
                // Scroll hidden in lower tunnel
                new TemplateObject { ItemId = 0x0110, TileX = 12, TileY = 34, PosX = 3, PosY = 3 },
                // Gold in hidden chamber
                new TemplateObject { ItemId = 0x00A0, TileX = 20, TileY = 27, PosX = 3, PosY = 3, Quantity = 30 },

                // --- Secret alcoves (Y=55-60, X=9-19) height 1 passages ---
                // Hidden gem in upper alcove
                new TemplateObject { ItemId = 0x00A8, TileX = 14, TileY = 58, PosX = 3, PosY = 3 },
                // Potion in secret niche
                new TemplateObject { ItemId = 0x0102, TileX = 10, TileY = 60, PosX = 3, PosY = 3 },
            };

            template.Npcs = new List<TemplateNpc>
            {
                // Goblin guards in the columned hall
                new TemplateNpc { ItemId = 0x0044, TileX = 7, TileY = 52, Hp = 25, Attitude = 0 },
                new TemplateNpc { ItemId = 0x0044, TileX = 19, TileY = 50, Hp = 25, Attitude = 0 },
                // Skeleton lurking in lower passage
                new TemplateNpc { ItemId = 0x0064, TileX = 6, TileY = 38, Hp = 35, Attitude = 0 },
                // Friendly ghost -- guardian spirit who warns the player
                new TemplateNpc { ItemId = 0x007C, TileX = 14, TileY = 44, Hp = 50, Attitude = 3 },
                // Goblin near the cistern entrance
                new TemplateNpc { ItemId = 0x0044, TileX = 22, TileY = 54, Hp = 20, Attitude = 0 },
            };

            template.Rooms = new List<TemplateRoom>
            {
                new TemplateRoom { Name = "Entry Corridor", X = 4, Y = 43, Width = 20, Height = 3,
                    Description = "A dusty passage with crumbling stone walls" },
                new TemplateRoom { Name = "Grand Columned Hall", X = 4, Y = 49, Width = 20, Height = 6,
                    Description = "A vast hall supported by carved stone columns at height 2" },
                new TemplateRoom { Name = "Cistern Chamber", X = 25, Y = 54, Width = 12, Height = 4,
                    Description = "A water-filled cistern with murky depths" },
                new TemplateRoom { Name = "East Side Passage", X = 29, Y = 49, Width = 6, Height = 5,
                    Description = "Narrow passage at height 3 overlooking the hall" },
                new TemplateRoom { Name = "Treasure Vault", X = 29, Y = 38, Width = 6, Height = 5,
                    Description = "Raised vault at height 4 behind a locked door" },
                new TemplateRoom { Name = "Secret Alcoves", X = 9, Y = 55, Width = 11, Height = 6,
                    Description = "Hidden alcoves at height 1, accessible from the hall" },
                new TemplateRoom { Name = "Lower Tunnels", X = 8, Y = 29, Width = 12, Height = 8,
                    Description = "Winding tunnels below the main level" },
                new TemplateRoom { Name = "Hidden Chamber", X = 16, Y = 23, Width = 14, Height = 6,
                    Description = "A secret chamber at the deepest point" },
            };

            return template;
        }

        /// <summary>
        /// Level 2: Lich's Domain
        /// A sprawling dark dungeon complex ruled by a powerful undead mage.
        /// Features a descending structure: entry crypts, skeleton barracks,
        /// ghoul warrens, a prison block, and a grand throne room surrounded
        /// by a lava moat where the lich awaits.
        /// Difficulty: Hard. Focus: combat, puzzle-solving.
        /// </summary>
        public static LevelTemplate CreateLichsDomain()
        {
            // Layout (north to south):
            //   Row 2-9   (Y61-54): Entry crypts and skeleton barracks
            //   Row 10-18 (Y53-45): Ghoul warrens and connecting corridors
            //   Row 19-26 (Y44-37): Prison block and armory
            //   Row 27-40 (Y36-23): Throne room with lava moat, treasure chamber
            var map = @"
################################################################
################################################################
####..........##################################################
####.1......1.##....D....#######################################
####.1......1.##.222222..#######################################
####.1......1D##.2....2..#######################################
####.1......1.##.2....2..#######################################
####.1......1.##.222222..#######################################
####..........##.........#######################################
####....D.....##....D....#######################################
#####.####.####.####.###########################################
#####.####.####.####.###########################################
#####.####.####D####.###########################################
####...........S.........#######################################
####.33333333333333333...#######################################
####.3...............3...#######################################
####.3...3..3..3..3..3...####.........##########################
####.3...............3...####.........##########################
####.33333333333333333..D####.........##########################
####....................D####D........##########################
####.33333333333333333..D####.........##########################
####.3...............3...####.........##########################
####.3...3..3..3..3..3...####.........##########################
####.3...............3...###################D###################
####.33333333333333333...#######.........D......D.##############
####.........................###.1.......1......1.##############
####.........................###.1.......1......1.##############
####.........##.##...........###.1111D1111......1.##############
####........D##.##D.......D..###.1.......1......1.##############
####.........##.##...........###.1.......1......1.##############
####.........##.##...........###.........D......1.##############
####.........................###..........1111111.##############
####..........D..............###.................D##############
############################.###.........##########.............
############################.###D........##########.444.........
############################.###.........####......D...........S
############################.###.........####.444...............
############################.#####D######....D..................
############################....D........D..........44444......#
############################.***********...........*44444*.....#
############################.*...........*.........*.....*.....#
############################.*...........*.........*.....*.....#
############################.*....555....*.........*.....*.....#
############################.*...5...5...*.........*.....*.....#
############################.*...5...5...*.........*.....*.....#
############################.*....555....*.........*.....*.....#
############################.*...........*.........*.....*.....#
############################.*...........*.........*.....*.....#
############################.***********...........*44444*.....#
############################....D........D..........44444......#
############################.####D######....D..................#
############################.####........####.444...............
############################.####........####......D............
############################.####........####..............#####
############################..###........##########.....########
##############################.##........#######################
###############################D#........#######################
################################.........#######################
#################################........#######################
################################################################
################################################################
################################################################
################################################################
################################################################".TrimStart('\n');

            var template = LevelTemplate.FromAsciiMap(
                "Lich's Domain", map,
                defaultFloorHeight: 2, defaultFloorTexture: 3, defaultWallTexture: 5);

            template.Description = "The underground lair of an ancient lich, a sprawling complex of " +
                "crypts and corridors descending into darkness. Skeleton guards patrol " +
                "the upper barracks while ghouls lurk in the warrens below. Deep within, " +
                "past the abandoned prison block, the lich sits upon its throne surrounded " +
                "by a moat of molten lava. Only the bold -- or the foolish -- venture here.";

            // Start position: line 13 col 13 => X=13, Y=63-13=50
            template.StartX = 13;
            template.StartY = 50;

            template.Npcs = new List<TemplateNpc>
            {
                // --- Entry Crypts (Y=54-61) ---
                // Skeleton sentries in crypt alcoves
                new TemplateNpc { ItemId = 0x0064, TileX = 6, TileY = 60, Hp = 25, Attitude = 0 },
                new TemplateNpc { ItemId = 0x0064, TileX = 10, TileY = 58, Hp = 25, Attitude = 0 },
                // Skeleton in eastern barracks
                new TemplateNpc { ItemId = 0x0064, TileX = 12, TileY = 58, Hp = 30, Attitude = 0 },

                // --- Ghoul Warrens (Y=45-53) ---
                // Ghouls in the double-corridor warren
                new TemplateNpc { ItemId = 0x0068, TileX = 8, TileY = 48, Hp = 40, Attitude = 0 },
                new TemplateNpc { ItemId = 0x0068, TileX = 16, TileY = 46, Hp = 40, Attitude = 0 },
                new TemplateNpc { ItemId = 0x0068, TileX = 10, TileY = 42, Hp = 45, Attitude = 0 },

                // --- Prison Block (Y=37-40) ---
                // Skeleton jailer
                new TemplateNpc { ItemId = 0x0064, TileX = 12, TileY = 38, Hp = 35, Attitude = 0 },

                // --- Armory/Antechamber (Y=37-39, X=28-42) ---
                // Ghoul guard near armory
                new TemplateNpc { ItemId = 0x0068, TileX = 35, TileY = 38, Hp = 45, Attitude = 0 },
                new TemplateNpc { ItemId = 0x0064, TileX = 40, TileY = 36, Hp = 30, Attitude = 0 },

                // --- Throne Room (Y=15-23, lava moat) ---
                // Lich boss on its throne (height 5 platform inside lava moat)
                new TemplateNpc { ItemId = 0x007C, TileX = 35, TileY = 19, Hp = 120, Attitude = 0 },
                // Skeleton bodyguards near throne
                new TemplateNpc { ItemId = 0x0064, TileX = 33, TileY = 20, Hp = 35, Attitude = 0 },
                new TemplateNpc { ItemId = 0x0064, TileX = 37, TileY = 20, Hp = 35, Attitude = 0 },
            };

            template.Objects = new List<TemplateObject>
            {
                // --- Entry area (near start) ---
                new TemplateObject { ItemId = 0x0090, TileX = 11, TileY = 50, PosX = 3, PosY = 3 },  // torch
                new TemplateObject { ItemId = 0x00C0, TileX = 8, TileY = 50, PosX = 3, PosY = 3 },   // food

                // --- Crypts loot ---
                new TemplateObject { ItemId = 0x00A0, TileX = 6, TileY = 56, PosX = 3, PosY = 3, Quantity = 20 },  // gold in crypt
                new TemplateObject { ItemId = 0x0002, TileX = 12, TileY = 56, PosX = 3, PosY = 3 },  // axe in barracks

                // --- Ghoul warrens loot ---
                new TemplateObject { ItemId = 0x0091, TileX = 5, TileY = 48, PosX = 3, PosY = 3 },   // torch
                new TemplateObject { ItemId = 0x0110, TileX = 18, TileY = 44, PosX = 3, PosY = 3 },  // healing potion

                // --- Prison block ---
                new TemplateObject { ItemId = 0x00D0, TileX = 8, TileY = 36, PosX = 3, PosY = 3 },   // key (for doors)
                new TemplateObject { ItemId = 0x0110, TileX = 6, TileY = 34, PosX = 3, PosY = 3 },   // scroll

                // --- Pre-boss healing stash (Y=38, X=28-30) ---
                new TemplateObject { ItemId = 0x0110, TileX = 33, TileY = 38, PosX = 3, PosY = 3 },  // healing potion
                new TemplateObject { ItemId = 0x0101, TileX = 32, TileY = 38, PosX = 5, PosY = 3 },  // cure potion
                new TemplateObject { ItemId = 0x0110, TileX = 34, TileY = 36, PosX = 3, PosY = 3 },  // healing potion
                new TemplateObject { ItemId = 0x00C1, TileX = 36, TileY = 36, PosX = 3, PosY = 3 },  // food

                // --- Throne Room rewards (on the raised platform) ---
                new TemplateObject { ItemId = 0x0008, TileX = 35, TileY = 18, PosX = 3, PosY = 3 },  // magic sword
                new TemplateObject { ItemId = 0x00A0, TileX = 34, TileY = 19, PosX = 2, PosY = 2, Quantity = 100 }, // gold pile

                // --- Treasure chamber (east of throne, Y=14-25, X=44-57) ---
                new TemplateObject { ItemId = 0x00A8, TileX = 48, TileY = 21, PosX = 3, PosY = 3 },  // gem
                new TemplateObject { ItemId = 0x00A8, TileX = 50, TileY = 19, PosX = 3, PosY = 3 },  // gem
                new TemplateObject { ItemId = 0x00A0, TileX = 52, TileY = 21, PosX = 3, PosY = 3, Quantity = 60 }, // gold
                new TemplateObject { ItemId = 0x0016, TileX = 48, TileY = 17, PosX = 3, PosY = 3 },  // plate armor
                new TemplateObject { ItemId = 0x0112, TileX = 50, TileY = 17, PosX = 3, PosY = 3 },  // scroll
            };

            template.Rooms = new List<TemplateRoom>
            {
                new TemplateRoom { Name = "Entry Crypts", X = 4, Y = 54, Width = 10, Height = 8,
                    Description = "Dusty crypts with stone alcoves at height 1" },
                new TemplateRoom { Name = "Skeleton Barracks", X = 14, Y = 56, Width = 9, Height = 6,
                    Description = "A training hall for the undead garrison" },
                new TemplateRoom { Name = "Ghoul Warrens North", X = 4, Y = 45, Width = 20, Height = 5,
                    Description = "Narrow corridors with columns, stalked by ghouls" },
                new TemplateRoom { Name = "Ghoul Warrens South", X = 4, Y = 40, Width = 20, Height = 5,
                    Description = "Southern warren, mirror of the north corridors" },
                new TemplateRoom { Name = "Connecting Hall", X = 24, Y = 40, Width = 10, Height = 10,
                    Description = "Open hall connecting warrens to the lower areas" },
                new TemplateRoom { Name = "Prison Block", X = 4, Y = 33, Width = 24, Height = 7,
                    Description = "Cells for prisoners, most now empty" },
                new TemplateRoom { Name = "Armory", X = 28, Y = 35, Width = 16, Height = 5,
                    Description = "Rooms with height-1 weapon racks" },
                new TemplateRoom { Name = "Throne Antechamber", X = 28, Y = 25, Width = 17, Height = 4,
                    Description = "The corridor before the lava moat" },
                new TemplateRoom { Name = "Throne Room", X = 29, Y = 14, Width = 13, Height = 11,
                    Description = "Lava moat surrounding the lich's raised throne platform" },
                new TemplateRoom { Name = "Treasure Chamber", X = 44, Y = 14, Width = 14, Height = 14,
                    Description = "Vault of plundered riches behind the throne" },
            };

            return template;
        }

        /// <summary>
        /// Level 3: The Sunken Temple
        /// A partially flooded ancient temple with a vast water-filled atrium,
        /// dry platforms at varying heights, underwater treasures, narrow
        /// walkways connecting island chambers, and an elevated shrine
        /// at the center. Sea creatures lurk in the murky depths.
        /// Difficulty: Medium. Focus: exploration, swimming, environmental.
        /// </summary>
        public static LevelTemplate CreateSunkenTemple()
        {
            // Layout (north to south):
            //   Row 3-10  (Y60-53): Northern dry chambers and entry
            //   Row 11-22 (Y52-41): Great water atrium with platforms
            //   Row 23-30 (Y40-33): Southern chambers and underwater grotto
            //   Row 31-38 (Y32-25): Deep submerged treasure vaults
            var map = @"
################################################################
################################################################
################################################################
######..........#########.........##############################
######.2222222..#########.2222222.##############################
######.2......2.####.....D......2.##############################
######.2......2.####.333.2......2.##############################
######.2......2D####.3S3.2......2.##############################
######.2222222..####.333.D2222222.##############################
######..........####.....D........##############################
#########D########..D..D..D########D############################
######~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~######################
######~~~555~~~~~~~~~~~~~~~~~~~~~555~~~~~~######################
######~~5...5~~~~~~~~~~~~~~~~~~~5...5~~~~~######################
######~~5...5~~~~~~4444444~~~~~~5...5~~~~~######################
######~~5...5~~~~~4..44..4~~~~~5...5~~~~~#######################
######~~5...5~~~~~4..44..4~~~~~5...5~~~~~#######################
######~~55D55~~~~~44D44D44~~~~~55D55~~~~~#######################
######~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~#####################
######~~~~333333~~~~~~~~~~~~~~~~~~~~333333~~~~##################
######~~~~3....3~~~~~~~~~~~~~~~~~~~~3....3~~~~##################
######~~~~3....D~~~~~~~~~~~~~~~~~~~~D....3~~~~##################
######~~~~3....3~~~~~~~~~~~~~~~~~~~~3....3~~~~##################
######~~~~333333~~~~~44444444~~~~~~~333333~~~~##################
######~~~~~~~D~~~~~~~4......4~~~~~~~~D~~~~~~~###################
######~~~~333333~~~~~4......4~~~~~333333~~~~####################
######~~~~3....3~~~~~4..55..4~~~~~3....3~~~~####################
######~~~~3....3~~~~~4..55..4~~~~~3....3~~~~####################
######~~~~3....D~~~~~4......4~~~~~D....3~~~~####################
######~~~~333333~~~~~4......4~~~~~333333~~~~####################
######~~~~~~~D~~~~~~~44D44D44~~~~~~~~D~~~~~~~###################
######~~~~333333~~~~~~......~~~~~~333333~~~~####################
######~~~~3....3~~~~~~......~~~~~~3....3~~~~####################
######~~~~3....3~~~~............~~3....3~~~~####################
######~~~~3....D~~~~............~~D....3~~~~####################
######~~~~333333~~~~............~~333333~~~~####################
######~~~~~~~~~~~~~~............~~~~~~~~~~~~~~##################
######~~~~~~~~~~~~~~............~~~~~~~~~~~~~~##################
######~~~~~~~~~~~~~~~~......~~~~~~~~~~~~~~~~~~##################
######~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~######################
######~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~######################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################".TrimStart('\n');

            var template = LevelTemplate.FromAsciiMap(
                "The Sunken Temple", map,
                defaultFloorHeight: 2, defaultFloorTexture: 4, defaultWallTexture: 3);

            template.WaterTexture = 6;
            template.Description = "An ancient temple half-swallowed by rising waters. A vast flooded " +
                "atrium stretches across the interior, dotted with dry platforms at " +
                "varying heights connected by narrow stone walkways. The central " +
                "shrine rises on a raised island, while underwater passages lead to " +
                "submerged treasure vaults. Sea creatures patrol the murky depths.";

            // Start position: line 7 col 22 => X=22, Y=63-7=56
            template.StartX = 22;
            template.StartY = 56;

            template.Objects = new List<TemplateObject>
            {
                // --- Northern dry chambers (Y=56-60) ---
                new TemplateObject { ItemId = 0x0090, TileX = 9, TileY = 58, PosX = 3, PosY = 3 },   // torch
                new TemplateObject { ItemId = 0x0004, TileX = 10, TileY = 57, PosX = 3, PosY = 3 },  // sword
                new TemplateObject { ItemId = 0x00C0, TileX = 27, TileY = 58, PosX = 3, PosY = 3 },  // food
                new TemplateObject { ItemId = 0x0091, TileX = 28, TileY = 56, PosX = 3, PosY = 3 },  // torch

                // --- West platform towers (Y=46-51, height 5 areas) ---
                new TemplateObject { ItemId = 0x00A8, TileX = 9, TileY = 50, PosX = 3, PosY = 3 },   // gem on west tower
                new TemplateObject { ItemId = 0x0110, TileX = 10, TileY = 48, PosX = 3, PosY = 3 },  // potion on west tower

                // --- East platform towers (height 5 areas) ---
                new TemplateObject { ItemId = 0x00A0, TileX = 32, TileY = 50, PosX = 3, PosY = 3, Quantity = 30 }, // gold
                new TemplateObject { ItemId = 0x0110, TileX = 33, TileY = 48, PosX = 3, PosY = 3 },  // scroll

                // --- Central shrine (height 4 island, Y=40-47) ---
                new TemplateObject { ItemId = 0x0008, TileX = 22, TileY = 46, PosX = 3, PosY = 3 },  // magic sword on shrine
                new TemplateObject { ItemId = 0x00A8, TileX = 24, TileY = 46, PosX = 3, PosY = 3 },  // gem on shrine
                new TemplateObject { ItemId = 0x0092, TileX = 21, TileY = 45, PosX = 3, PosY = 3 },  // torch

                // --- Side chambers at height 3 (west and east, Y=33-43) ---
                new TemplateObject { ItemId = 0x0110, TileX = 11, TileY = 43, PosX = 3, PosY = 3 },  // potion west upper
                new TemplateObject { ItemId = 0x0014, TileX = 11, TileY = 41, PosX = 3, PosY = 3 },  // armor west
                new TemplateObject { ItemId = 0x00A0, TileX = 35, TileY = 41, PosX = 3, PosY = 3, Quantity = 25 }, // gold east
                new TemplateObject { ItemId = 0x0101, TileX = 35, TileY = 37, PosX = 3, PosY = 3 },  // cure potion east lower
                new TemplateObject { ItemId = 0x0010, TileX = 11, TileY = 37, PosX = 3, PosY = 3 },  // shield west lower

                // --- Underwater treasure (items on water tiles) ---
                new TemplateObject { ItemId = 0x00A0, TileX = 18, TileY = 44, PosX = 3, PosY = 3, Quantity = 50 }, // gold underwater
                new TemplateObject { ItemId = 0x00A8, TileX = 28, TileY = 44, PosX = 3, PosY = 3 },  // gem underwater
                new TemplateObject { ItemId = 0x00D0, TileX = 20, TileY = 38, PosX = 3, PosY = 3 },  // key underwater

                // --- Southern deep area (Y=25-32) ---
                new TemplateObject { ItemId = 0x00A0, TileX = 22, TileY = 30, PosX = 3, PosY = 3, Quantity = 40 }, // gold deep
                new TemplateObject { ItemId = 0x0112, TileX = 24, TileY = 28, PosX = 3, PosY = 3 },  // scroll deep

                // --- Submerged floor platforms (Y=26-27, height 5 center) ---
                new TemplateObject { ItemId = 0x00A8, TileX = 22, TileY = 36, PosX = 3, PosY = 3 },  // gem on inner shrine
            };

            template.Npcs = new List<TemplateNpc>
            {
                // Sea creature lurkers in the water
                new TemplateNpc { ItemId = 0x0050, TileX = 15, TileY = 51, Hp = 35, Attitude = 0 },
                new TemplateNpc { ItemId = 0x0050, TileX = 30, TileY = 44, Hp = 35, Attitude = 0 },
                new TemplateNpc { ItemId = 0x0054, TileX = 20, TileY = 32, Hp = 50, Attitude = 0 },
                // Larger creature guarding the deep area
                new TemplateNpc { ItemId = 0x0058, TileX = 22, TileY = 26, Hp = 60, Attitude = 0 },
            };

            template.Rooms = new List<TemplateRoom>
            {
                new TemplateRoom { Name = "Northern Entry Hall", X = 15, Y = 55, Width = 5, Height = 4,
                    Description = "Dry entry chamber with stairs down to the water" },
                new TemplateRoom { Name = "West Dry Chamber", X = 6, Y = 56, Width = 9, Height = 5,
                    Description = "Intact chamber above the waterline" },
                new TemplateRoom { Name = "East Dry Chamber", X = 25, Y = 56, Width = 9, Height = 5,
                    Description = "Eastern chamber with supply caches" },
                new TemplateRoom { Name = "Great Atrium", X = 6, Y = 23, Width = 36, Height = 30,
                    Description = "Vast flooded temple interior with island platforms" },
                new TemplateRoom { Name = "West Tower", X = 8, Y = 46, Width = 5, Height = 6,
                    Description = "Stone tower platform at height 5" },
                new TemplateRoom { Name = "East Tower", X = 31, Y = 46, Width = 5, Height = 6,
                    Description = "Stone tower platform at height 5" },
                new TemplateRoom { Name = "Central Shrine", X = 19, Y = 39, Width = 8, Height = 8,
                    Description = "Raised island shrine at height 4-5, the temple heart" },
                new TemplateRoom { Name = "West Chambers", X = 10, Y = 31, Width = 6, Height = 13,
                    Description = "Side chambers at height 3 connected by walkways" },
                new TemplateRoom { Name = "East Chambers", X = 31, Y = 31, Width = 6, Height = 13,
                    Description = "Side chambers at height 3 connected by walkways" },
                new TemplateRoom { Name = "Deep Grotto", X = 17, Y = 25, Width = 12, Height = 8,
                    Description = "Submerged passages with scattered treasure" },
            };

            return template;
        }

        /// <summary>
        /// Level 4: Arena of Trials
        /// A combat gauntlet with a long connecting corridor linking four
        /// distinct arena chambers of escalating size and difficulty.
        /// Rest areas between arenas provide healing supplies. A grand
        /// champion's vault at the end holds legendary rewards.
        /// Difficulty: Very Hard. Focus: combat.
        /// </summary>
        public static LevelTemplate CreateArenaOfTrials()
        {
            // Layout (north to south, since we enter from the north):
            //   Row 2-6   (Y61-57): Preparation room and entrance
            //   Row 7-8   (Y56-55): Corridor to Arena 1
            //   Row 9-17  (Y54-46): Arena 1 - The Pit (8x8, goblins)
            //   Row 18-20 (Y45-43): Rest area 1
            //   Row 21-31 (Y42-32): Arena 2 - The Gauntlet (10x10, skeletons)
            //   Row 32-34 (Y31-29): Rest area 2
            //   Row 35-47 (Y28-16): Arena 3 - The Crucible (12x12, ghouls+trolls)
            //   Row 48-50 (Y15-13): Rest area 3
            //   Row 51-63 (Y12-0):  Arena 4 + Champion's Vault
            var map = @"
################################################################
################################################################
#############........###########################################
#############.222222.###########################################
#############.2....2.###########################################
#############.2.S..2.###########################################
#############.222222.###########################################
#############...DD...###########################################
##############.####.############################################
###########.........D...................########################
###########.1...................1....#..########################
###########......................1..#..#########################
###########.1...................1...#..#########################
###########......................1..#..#########################
###########.1...................1...#..#########################
###########.......................#..###########################
###########.1...................1.#..###########################
###########.....................D..#..##########################
############..####D###########.####.############################
############..####.###########.####.############################
############..####.###########.####.############################
########.....D.............D.....D..############################
########.1.....................1....############################
########.1.....................1....############################
########.......................1....############################
########.1.....................1..##.###########################
########.............................###########################
########.1.....................1....############################
########.1.....................1....############################
########.......................1....############################
########.1.....................1....############################
########.....D.............D.....D..############################
########......####D###########.####.############################
########......####.###########.####.############################
########......####.###########.####.############################
#####............D..............D.....D.########################
#####.1.............................1...########################
#####....................................#######################
#####.1.............................1...########################
#####....................................#######################
#####.1.............................1...########################
#####....................................#######################
#####.1.............................1...########################
#####....................................#######################
#####.1.............................1...########################
#####....................................#######################
#####.1.............................1...########################
#####............D..............D.....D.########################
######.......####D##############.####.##########################
######.......####.##############.####.##########################
######.......####.##############.####.##########################
#####.............D..............D.....D..######################
#####.............1................1......######################
#####.1..........................1.......#######################
#####.............1................1......######################
#####.1..........................1.......#######################
#####.............1.......D......1...444444.####################
#####.1..........................1...4....4.####################
#####.............1..............1...4....4.####################
#####.1..........................1...4....4.####################
#####.............D..............D...444D44.####################
#####....................................D..####################
################################################################
################################################################".TrimStart('\n');

            var template = LevelTemplate.FromAsciiMap(
                "Arena of Trials", map,
                defaultFloorHeight: 2, defaultFloorTexture: 2, defaultWallTexture: 4);

            template.Description = "A gladiatorial complex with four arena chambers of escalating difficulty. " +
                "Begin in the preparation room, then fight through the Pit, the Gauntlet, " +
                "the Crucible, and the final Grand Arena. Healing supplies await between " +
                "each trial. Those who survive all four earn the champion's treasure.";

            // Start position: line 5 col 16 => X=16, Y=63-5=58
            template.StartX = 16;
            template.StartY = 58;

            template.Npcs = new List<TemplateNpc>
            {
                // --- Arena 1: The Pit (Y=46-54) - Goblins ---
                new TemplateNpc { ItemId = 0x0044, TileX = 14, TileY = 52, Hp = 20, Attitude = 0 },
                new TemplateNpc { ItemId = 0x0044, TileX = 20, TileY = 50, Hp = 20, Attitude = 0 },
                new TemplateNpc { ItemId = 0x0044, TileX = 17, TileY = 48, Hp = 25, Attitude = 0 },

                // --- Arena 2: The Gauntlet (Y=32-42) - Skeletons ---
                new TemplateNpc { ItemId = 0x0064, TileX = 12, TileY = 40, Hp = 30, Attitude = 0 },
                new TemplateNpc { ItemId = 0x0064, TileX = 18, TileY = 38, Hp = 30, Attitude = 0 },
                new TemplateNpc { ItemId = 0x0064, TileX = 10, TileY = 36, Hp = 35, Attitude = 0 },
                new TemplateNpc { ItemId = 0x0064, TileX = 16, TileY = 34, Hp = 35, Attitude = 0 },

                // --- Arena 3: The Crucible (Y=16-28) - Ghouls + trolls ---
                new TemplateNpc { ItemId = 0x0068, TileX = 8, TileY = 26, Hp = 45, Attitude = 0 },
                new TemplateNpc { ItemId = 0x0068, TileX = 20, TileY = 24, Hp = 45, Attitude = 0 },
                new TemplateNpc { ItemId = 0x0070, TileX = 14, TileY = 22, Hp = 60, Attitude = 0 },
                new TemplateNpc { ItemId = 0x0068, TileX = 10, TileY = 20, Hp = 50, Attitude = 0 },
                new TemplateNpc { ItemId = 0x0070, TileX = 22, TileY = 18, Hp = 65, Attitude = 0 },

                // --- Arena 4: Grand Arena (Y=1-12) - Elite enemies ---
                new TemplateNpc { ItemId = 0x0070, TileX = 10, TileY = 9, Hp = 70, Attitude = 0 },
                new TemplateNpc { ItemId = 0x0074, TileX = 18, TileY = 7, Hp = 60, Attitude = 0 },
                new TemplateNpc { ItemId = 0x0070, TileX = 12, TileY = 5, Hp = 75, Attitude = 0 },
                new TemplateNpc { ItemId = 0x0074, TileX = 22, TileY = 3, Hp = 65, Attitude = 0 },
            };

            template.Objects = new List<TemplateObject>
            {
                // --- Preparation room (Y=57-61) ---
                new TemplateObject { ItemId = 0x0004, TileX = 15, TileY = 59, PosX = 3, PosY = 3 },  // sword
                new TemplateObject { ItemId = 0x0010, TileX = 17, TileY = 59, PosX = 3, PosY = 3 },  // shield
                new TemplateObject { ItemId = 0x0090, TileX = 14, TileY = 60, PosX = 3, PosY = 3 },  // torch
                new TemplateObject { ItemId = 0x0110, TileX = 18, TileY = 60, PosX = 3, PosY = 3 },  // healing potion

                // --- Rest area 1 (Y=43-44, between Arena 1 and 2) ---
                new TemplateObject { ItemId = 0x0110, TileX = 12, TileY = 44, PosX = 3, PosY = 3 },  // healing potion
                new TemplateObject { ItemId = 0x00C0, TileX = 13, TileY = 43, PosX = 3, PosY = 3 },  // food

                // --- Rest area 2 (Y=29-31, between Arena 2 and 3) ---
                new TemplateObject { ItemId = 0x0110, TileX = 8, TileY = 30, PosX = 3, PosY = 3 },   // healing potion
                new TemplateObject { ItemId = 0x0101, TileX = 10, TileY = 30, PosX = 3, PosY = 3 },  // cure potion
                new TemplateObject { ItemId = 0x00C0, TileX = 12, TileY = 30, PosX = 3, PosY = 3 },  // food

                // --- Rest area 3 (Y=13-15, between Arena 3 and 4) ---
                new TemplateObject { ItemId = 0x0110, TileX = 7, TileY = 14, PosX = 3, PosY = 3 },   // healing potion
                new TemplateObject { ItemId = 0x0110, TileX = 9, TileY = 14, PosX = 3, PosY = 3 },   // healing potion
                new TemplateObject { ItemId = 0x0101, TileX = 10, TileY = 14, PosX = 3, PosY = 3 },  // cure potion
                new TemplateObject { ItemId = 0x00C1, TileX = 11, TileY = 14, PosX = 3, PosY = 3 },  // food

                // --- Champion's Vault (height 4 area, Y=2-6, X=33-38) ---
                new TemplateObject { ItemId = 0x000A, TileX = 35, TileY = 5, PosX = 3, PosY = 3 },   // magic weapon
                new TemplateObject { ItemId = 0x0016, TileX = 36, TileY = 5, PosX = 5, PosY = 3 },   // plate armor
                new TemplateObject { ItemId = 0x00A0, TileX = 35, TileY = 4, PosX = 2, PosY = 2, Quantity = 150 }, // gold
                new TemplateObject { ItemId = 0x00A8, TileX = 36, TileY = 4, PosX = 5, PosY = 5 },   // gem
                new TemplateObject { ItemId = 0x00A8, TileX = 35, TileY = 3, PosX = 3, PosY = 3 },   // gem
            };

            template.Rooms = new List<TemplateRoom>
            {
                new TemplateRoom { Name = "Preparation Room", X = 13, Y = 57, Width = 8, Height = 5,
                    Description = "Arm yourself before the trials begin" },
                new TemplateRoom { Name = "Arena 1: The Pit", X = 11, Y = 46, Width = 12, Height = 9,
                    Description = "A small pit with goblin opponents" },
                new TemplateRoom { Name = "Rest Area 1", X = 12, Y = 43, Width = 6, Height = 3,
                    Description = "Brief respite with healing supplies" },
                new TemplateRoom { Name = "Arena 2: The Gauntlet", X = 8, Y = 32, Width = 16, Height = 11,
                    Description = "A larger arena with skeleton warriors" },
                new TemplateRoom { Name = "Rest Area 2", X = 8, Y = 29, Width = 6, Height = 3,
                    Description = "Resupply before the harder fights" },
                new TemplateRoom { Name = "Arena 3: The Crucible", X = 5, Y = 16, Width = 20, Height = 13,
                    Description = "Wide arena with ghouls and trolls" },
                new TemplateRoom { Name = "Rest Area 3", X = 6, Y = 13, Width = 8, Height = 3,
                    Description = "Final chance to heal before the grand arena" },
                new TemplateRoom { Name = "Arena 4: Grand Arena", X = 5, Y = 1, Width = 24, Height = 12,
                    Description = "The ultimate challenge with elite foes" },
                new TemplateRoom { Name = "Champion's Vault", X = 33, Y = 2, Width = 6, Height = 6,
                    Description = "Raised vault at height 4 with legendary loot" },
            };

            return template;
        }

        /// <summary>
        /// Level 5: The Artificer's Workshop
        /// A puzzle and exploration-focused level with two wing workshops
        /// (east and west), a connecting corridor, a great hall with raised
        /// workbench platforms, and multiple storage rooms with varied loot.
        /// Height variation creates shelves, workbenches, and display areas.
        /// Difficulty: Medium. Focus: puzzles, exploration.
        /// </summary>
        public static LevelTemplate CreateArtificersWorkshop()
        {
            // Layout (north to south):
            //   Row 2-10  (Y61-53): Great Hall with raised workbench
            //   Row 11-14 (Y52-49): Connecting corridors north
            //   Row 15-26 (Y48-37): West and East wing workshops
            //   Row 27-30 (Y36-33): Connecting corridors south
            //   Row 31-42 (Y32-21): Storage rooms and display galleries
            //   Row 43-48 (Y20-15): Lower vault and entry
            var map = @"
################################################################
################################################################
####...........#################...........#####################
####.333333333.#################.333333333.#####################
####.3.......3.#################.3.......3.#####################
####.3..444..3.####.............D3.......3.#####################
####.3..4.4..3.####.2.........2.D3..444..3.#####################
####.3..444..3.####.2.........2.#3..4.4..3.#####################
####.3.......3.####.2..4.4.4..2.#3..444..3.#####################
####.3.......3D####.2.........2.#D3.......3.####################
####.333333333.####.2.........2.#.333333333.####################
####...........####.2.........2.#...........####################
####D##########.##.D2222D2222D2.D##########D####################
####.............##.............##.............#################
####.............##.............##.............#################
####...1.........D..............D...1..........#################
####..111........##.............##..111.........################
####...1.........##.............##...1..........################
####.............##.............##.............#################
####.333333333...##.............##...333333333.#################
####.3.......3...##.............##...3.......3.#################
####.3.......3.D.##.............##.D.3.......3.#################
####.3..555..3...##.............##...3..555..3.#################
####.3..5.5..3...##.............##...3..5.5..3.#################
####.3..555..3...##.............##...3..555..3.#################
####.3.......3...##.............##...3.......3.#################
####.333333333...##.............##...333333333.#################
####.............##.............##.............#################
####D####D####D####.............####D####D####D#################
####.....#....#.###.............###.#....#.....#################
####.222.#.22.#..##.............##..#.22.#.222.#################
####.2.2.#.22.#..D..............D..#.22.#.2.2.##################
####.222.#....#..##.............##..#....#.222.#################
####.....D....#.###.............###.#....D.....#################
####...........####.............####...........#################
####...........####.............####...........#################
####D#########D####.............####D#########D#################
####............###.............###............#################
####.1.........1.##.............##.1.........1.#################
####.1.........1.D......S......D..1.........1.##################
####.1.........1.##.............##.1.........1.#################
####............###.............###............#################
####D#########D####D#########D####D#########D###################
####.............##.............##.............#################
####.44444444444.##.............##.44444444444.#################
####.4.........4.D..............D.4.........4.##################
####.4.........4.##.............##.4.........4.#################
####.44444444444.##.............##.44444444444.#################
####.............##.............##.............#################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################
################################################################".TrimStart('\n');

            var template = LevelTemplate.FromAsciiMap(
                "The Artificer's Workshop", map,
                defaultFloorHeight: 2, defaultFloorTexture: 0, defaultWallTexture: 1);

            template.Description = "The sealed workshop of a legendary artificer, a master of both " +
                "craft and enchantment. The complex features two wing workshops with " +
                "raised workbenches, a grand central hall, display galleries with " +
                "prototype enchanted items, and deep storage vaults. Friendly artisan " +
                "spirits tend to their unfinished work, while guardian constructs patrol " +
                "the corridors to protect the artificer's secrets.";

            // Start position: line 39 col 20 => X=20, Y=63-39=24
            template.StartX = 20;
            template.StartY = 24;

            template.Npcs = new List<TemplateNpc>
            {
                // Friendly artisan ghosts in the workshops
                new TemplateNpc { ItemId = 0x007C, TileX = 8, TileY = 57, Hp = 40, Attitude = 3 },   // west workshop artisan
                new TemplateNpc { ItemId = 0x007C, TileX = 30, TileY = 57, Hp = 40, Attitude = 3 },  // east workshop artisan

                // Hostile guardian constructs patrolling corridors
                new TemplateNpc { ItemId = 0x0070, TileX = 10, TileY = 48, Hp = 55, Attitude = 0 },  // west corridor
                new TemplateNpc { ItemId = 0x0070, TileX = 28, TileY = 48, Hp = 55, Attitude = 0 },  // east corridor
            };

            template.Objects = new List<TemplateObject>
            {
                // --- Great Hall (Y=53-61) raised workbenches ---
                // Scrolls on the workbench (height 4)
                new TemplateObject { ItemId = 0x0110, TileX = 8, TileY = 57, PosX = 3, PosY = 3 },   // scroll west bench
                new TemplateObject { ItemId = 0x0112, TileX = 8, TileY = 56, PosX = 3, PosY = 3 },   // scroll
                new TemplateObject { ItemId = 0x0110, TileX = 30, TileY = 57, PosX = 3, PosY = 3 },  // scroll east bench
                // Gems as components on workbenches
                new TemplateObject { ItemId = 0x00A8, TileX = 9, TileY = 58, PosX = 3, PosY = 3 },   // gem
                new TemplateObject { ItemId = 0x00A8, TileX = 31, TileY = 56, PosX = 3, PosY = 3 },  // gem

                // --- Central corridor display area (Y=49-52) ---
                new TemplateObject { ItemId = 0x0092, TileX = 13, TileY = 54, PosX = 3, PosY = 3 },  // torch
                new TemplateObject { ItemId = 0x0008, TileX = 20, TileY = 55, PosX = 3, PosY = 3 },  // enchanted weapon display
                new TemplateObject { ItemId = 0x0091, TileX = 25, TileY = 54, PosX = 3, PosY = 3 },  // torch

                // --- West Wing workshops (Y=37-48) ---
                // Potions on shelves (height 1 shelves)
                new TemplateObject { ItemId = 0x0110, TileX = 7, TileY = 47, PosX = 3, PosY = 3 },   // potion
                new TemplateObject { ItemId = 0x0101, TileX = 8, TileY = 47, PosX = 3, PosY = 3 },   // potion
                new TemplateObject { ItemId = 0x0102, TileX = 9, TileY = 47, PosX = 3, PosY = 3 },   // potion
                // Workbench items (height 5)
                new TemplateObject { ItemId = 0x0110, TileX = 8, TileY = 41, PosX = 3, PosY = 3 },   // scroll
                new TemplateObject { ItemId = 0x00A8, TileX = 8, TileY = 40, PosX = 3, PosY = 3 },   // gem

                // --- East Wing workshops (Y=37-48) ---
                new TemplateObject { ItemId = 0x0103, TileX = 30, TileY = 47, PosX = 3, PosY = 3 },  // potion
                new TemplateObject { ItemId = 0x0110, TileX = 31, TileY = 47, PosX = 3, PosY = 3 },  // potion
                new TemplateObject { ItemId = 0x0112, TileX = 30, TileY = 41, PosX = 3, PosY = 3 },  // scroll
                new TemplateObject { ItemId = 0x00A8, TileX = 30, TileY = 40, PosX = 3, PosY = 3 },  // gem

                // --- Storage rooms (Y=29-34, small rooms with shelves) ---
                new TemplateObject { ItemId = 0x00A0, TileX = 6, TileY = 32, PosX = 3, PosY = 3, Quantity = 40 },   // gold
                new TemplateObject { ItemId = 0x00C0, TileX = 11, TileY = 32, PosX = 3, PosY = 3 },  // food
                new TemplateObject { ItemId = 0x00A0, TileX = 29, TileY = 32, PosX = 3, PosY = 3, Quantity = 40 },  // gold
                new TemplateObject { ItemId = 0x00C1, TileX = 34, TileY = 32, PosX = 3, PosY = 3 },  // food
                new TemplateObject { ItemId = 0x00D0, TileX = 6, TileY = 30, PosX = 3, PosY = 3 },   // key in storage

                // --- Display galleries (Y=22-27) ---
                new TemplateObject { ItemId = 0x0090, TileX = 6, TileY = 24, PosX = 3, PosY = 3 },   // torch
                new TemplateObject { ItemId = 0x000A, TileX = 7, TileY = 23, PosX = 3, PosY = 3 },   // magic weapon
                new TemplateObject { ItemId = 0x0090, TileX = 33, TileY = 24, PosX = 3, PosY = 3 },  // torch

                // --- Deep Vaults (Y=15-20, height 4 areas) ---
                new TemplateObject { ItemId = 0x00A0, TileX = 8, TileY = 18, PosX = 3, PosY = 3, Quantity = 80 },   // gold hoard
                new TemplateObject { ItemId = 0x00A8, TileX = 10, TileY = 18, PosX = 3, PosY = 3 },  // gem
                new TemplateObject { ItemId = 0x0016, TileX = 30, TileY = 18, PosX = 3, PosY = 3 },  // fine armor
                new TemplateObject { ItemId = 0x00A8, TileX = 32, TileY = 18, PosX = 3, PosY = 3 },  // gem
                new TemplateObject { ItemId = 0x0114, TileX = 20, TileY = 16, PosX = 3, PosY = 3 },  // scroll in vault corridor
            };

            template.Rooms = new List<TemplateRoom>
            {
                new TemplateRoom { Name = "Great Hall", X = 15, Y = 53, Width = 8, Height = 9,
                    Description = "Central hall with display columns and raised platforms" },
                new TemplateRoom { Name = "West Upper Workshop", X = 4, Y = 53, Width = 11, Height = 9,
                    Description = "Workshop with height 4 workbenches and tool racks" },
                new TemplateRoom { Name = "East Upper Workshop", X = 27, Y = 53, Width = 11, Height = 9,
                    Description = "Workshop with height 4 workbenches and enchanting tools" },
                new TemplateRoom { Name = "West Lower Workshop", X = 4, Y = 37, Width = 11, Height = 11,
                    Description = "Lower workshop with height 5 workbenches and shelves" },
                new TemplateRoom { Name = "East Lower Workshop", X = 27, Y = 37, Width = 11, Height = 11,
                    Description = "Lower workshop with height 5 workbenches and shelves" },
                new TemplateRoom { Name = "Central Corridor", X = 15, Y = 37, Width = 8, Height = 16,
                    Description = "Long corridor connecting all workshop areas" },
                new TemplateRoom { Name = "West Storage", X = 4, Y = 29, Width = 14, Height = 6,
                    Description = "Storage rooms with material shelves" },
                new TemplateRoom { Name = "East Storage", X = 25, Y = 29, Width = 14, Height = 6,
                    Description = "Storage rooms with component shelves" },
                new TemplateRoom { Name = "West Gallery", X = 4, Y = 21, Width = 12, Height = 7,
                    Description = "Display gallery with prototype items" },
                new TemplateRoom { Name = "East Gallery", X = 25, Y = 21, Width = 12, Height = 7,
                    Description = "Display gallery with enchanted items" },
                new TemplateRoom { Name = "West Vault", X = 4, Y = 15, Width = 13, Height = 5,
                    Description = "Secure vault with height 4 raised floor" },
                new TemplateRoom { Name = "East Vault", X = 25, Y = 15, Width = 13, Height = 5,
                    Description = "Secure vault with height 4 raised floor" },
                new TemplateRoom { Name = "Entry Hall", X = 15, Y = 15, Width = 8, Height = 14,
                    Description = "The main entry corridor" },
            };

            return template;
        }
    }
}
