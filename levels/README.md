# Custom Levels

This directory contains level template files for the UnderworldGodot fork.

## Built-in Levels

Five custom levels are available via the debug console (toggle with backtick key):

| Command | Level | Slot | Difficulty | Focus |
|---------|-------|------|-----------|-------|
| `loadlevel vault` | The Forgotten Vault | 70 | Easy | Exploration, treasure |
| `loadlevel lich` | Lich's Domain | 71 | Hard | Combat, puzzles |
| `loadlevel temple` | The Sunken Temple | 72 | Medium | Swimming, exploration |
| `loadlevel arena` | Arena of Trials | 73 | Very Hard | Combat gauntlet |
| `loadlevel workshop` | The Artificer's Workshop | 74 | Medium | Puzzles, loot |

## Level Editing Commands

From the debug console:

- `settile <x> <y> <type>` - Change tile type (0=solid, 1=open, 2-5=diagonal, 6-9=slopes)
- `setheight <x> <y> <h>` - Change floor height (0-15)
- `settex <x> <y> <floor> [wall]` - Change textures
- `putdoor <x> <y>` - Place a door
- `carve <x> <y> <w> <h>` - Carve a room in the current level
- `corridor <x1> <y1> <x2> <y2>` - Carve a corridor
- `rerender` - Redraw the level after edits
- `exportlevel` - Export current level as ASCII art
- `newlevel <slot> [roomSize]` - Create a blank level with a starter room
- `spawn <id> [x y]` - Spawn objects/NPCs

## ASCII Map Format

Level templates use ASCII maps where each character represents a tile:

```
# or T = Solid rock
.      = Open floor (default height)
0-9    = Open floor at height 0-9
A-F    = Open floor at height 10-15
D      = Door
~      = Water
*      = Lava
/      = Slope north
\      = Slope south
[      = Slope east
]      = Slope west
S      = Player start
```

Maps are 64x64 max. Top of map = north (Y=63), bottom = south (Y=0).

## JSON Template Format

Templates can also be saved/loaded as JSON:

```json
{
  "Name": "My Level",
  "Description": "A custom dungeon",
  "DefaultFloorHeight": 2,
  "DefaultFloorTexture": 0,
  "DefaultWallTexture": 0,
  "StartX": 32,
  "StartY": 32,
  "MapLines": [
    "################",
    "###...........##",
    "###.S.........##",
    "###...........##",
    "################"
  ],
  "Objects": [
    { "ItemId": 256, "TileX": 10, "TileY": 60, "PosX": 3, "PosY": 3 }
  ],
  "Npcs": [
    { "ItemId": 100, "TileX": 12, "TileY": 60, "Hp": 30, "Attitude": 0 }
  ]
}
```
