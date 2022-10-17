###### ID
20

###### Direction
Server <-> Client

### Structure
| Description | Type |
|-------------|------|
| Starting X  | short |
| Starting Y  | short |
| Width       | byte |
| Height      | byte |
| Change Type | byte |
| Tiles       | [[#Tiles]]

###### Tiles
For `Width * Height` tiles, **x by y**:

| Description | Type |
|-------------|------|
| [[#Header 1]]    | byte |
| [[#Header 2]]    | byte |
| Block Color[^1]   | byte |
| Wall Color[^2]    | byte |
| Block ID[^3]      | ushort |
| Frame X[^4]       | short |
| Frame Y[^4]       | short |
| Wall ID[^5]       | ushort |
| Liquid Amount[^6] | byte |
| Liquid Type[^6]   | byte |

###### Header  1
| Description | Value |
|-------------|-------|
| Has Block     | `1 << 0` |
| Has Wall      | `1 << 2` |
| Has Liquid    | `1 << 3` |
| Has Red Wire  | `1 << 4` |
| Is Half Brick | `1 << 5` |
| Has Actuator  | `1 << 6` |
| Is Actuated   | `1 << 7` |

###### Header  2
| Description | Value |
|-------------|-------|
| Has Blue Wire      | `1 << 0` |
| Has Green Wire     | `1 << 1` |
| Has Block Color    | `1 << 2` |
| Has Wall Color     | `1 << 3` |
| [[#Shapes\|Shape]] | `0b0111'0000` |
| Has Yellow Wire    | `1 << 7` |

[^1]: Only present if `Has Block Color` bit is set on [[#Header 2]]
[^2]: Only present if `Has Wall Color` bit is set on [[#Header 2]]
[^3]: Only present if `Has Block` bit is set on [[#Header 1]]
[^4]: Only present if `tileFrameImportant` (TODO explain what this means)
[^5]: Only present if `Has Wall` bit is set on [[#Header 1]]
[^6]: Only present if `Has Liquid` bit is set on [[#Header 1]]