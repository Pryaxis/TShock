###### ID 10
###### Server -> Client
| Description | Type |
|-------------|------|
| Compressed  | bool |

If the `Compressed` boolean is `true`, then all subsequent data is written with [deflate compression](https://wikipedia.org/wiki/Deflate).

| Description | Type |
|-------------|------|
| Starting X        | int |
| Starting Y        | int |
| Width             | ushort |
| Height            | ushort |
| Tiles             | [[#Tiles]]
| Chest Count       | ushort |
| Chests            | [[#Chests]]
| Sign Count        | ushort |
| Signs             | [[#Signs]]
| Tile Entity Count | ushort |
| Tile Entities     | [[#Tile Entities]]

##### Tiles
For every tile, **y by x**:

| Description | Type |
|-------------|------|
| [[#Header1]\|Header 1]]    | byte |
| [[#Header2]\|Header 2]][^1] | byte |
| [[#Header3]\|Header 3]][^2] | byte |
| Block ID[^3]                | byte or ushort[^4] |
| Frame X[^5]                 | short |
| Frame Y[^5]                 | short |
| Wall ID[^6]                 | byte |
| Liquid Amount[^7]           | byte |
| Upper Wall ID[^8]           | byte |

[^1]: Only present if `Has Header 2` bit is set on [[#Header1]]
[^2]: Only present if `Has Header 3` bit is set on [[#Header2]]
[^3]: Only present if `Has Block` bit is set on [[#Header1]]
[^4]: If present, and `Has Extended Block ID` bit is set on [[#Header1]], then `ushort`, otherwise `byte`
[^5]: Only present if `tileFrameImportant` (TODO explain what this means)
[^6]: Only present if `Has Wall` bit is set on [[#Header1]]
[^7]: Only present if `((Header 1 & Liquid Type) >> 3) != 0`.
[^8]: Only present if `Has Extended Wall ID` bit is set on [[#Header3]]. If present, this becomes the upper bits of the wall ID, and the previous bits become the lower bits.

##### Header1
| Description | Value |
|-------------|-------|
| Has Header 2                   | `1 << 0` |
| Has Block                      | `1 << 1` |
| Has Wall                       | `1 << 2` |
| Has Extended Block ID          | `1 << 5` |
| [[#Liquid Types\|Liquid Type]] | `0b0001'1000` |

##### Header2
| Description | Value |
|-------------|-------|
| Has Header 3       | `1 << 0` |
| Has Red Wire       | `1 << 1` |
| Has Blue Wire      | `1 << 2` |
| Has Green Wire     | `1 << 3` |
| [[#Shapes\|Shape]] | `0b0111'0000` |

##### Header3
| Description | Value |
|-------------|-------|
| Has Actuator         | `1 << 1` |
| Is Actuated          | `1 << 2` |
| Has Yellow Wire      | `1 << 5` |
| Has Extended Wall ID | `1 << 6` |

##### Liquid Types
| Description | Value |
|-------------|-------|
| Water | 1 |
| Lava  | 2 |
| Honey | 3 |

##### Shapes
| Description | Value |
|-------------|-------|
| Half Brick   | 1 |
| Top Right    | 2 |
| Top Left     | 3 |
| Bottom Right | 4 |
| Bottom Left  | 5 |