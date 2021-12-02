###### ID
17

###### Direction
Server <-> Client

### Structure
| Description | Type |
|-------------|------|
| [[#Action]] | byte |
| X           | short |
| Y           | short |
| Flags 1     | short |
| Flags 2     | byte |

##### Action
| Description | Value |
|-------------|------|
| Kill Block Without Drop | 0 |
| Place Block[^1]          | 1 |
| Kill Wall               | 2 |
| Place Wall              | 3 |
| Kill Block              | 4 |
| Place Red Wire          | 5 |
| Kill Red Wire           | 6 |
| Pound[^2] Block           | 7 |
| Place Actuator          | 8 |
| Kill Actuator           | 9 |
| Place Blue Wire         | 10 |
| Kill Blue Wire          | 11 |
| Place Green Wire        | 12 |
| Kill Green Wire         | 13 |
| Slope[^3] Block          | 14 |
| Place Yellow Wire       | 16 |
| Kill Yellow Wire        | 17 |
| Replace[^4] Block        | 21 |
| Replace[^4] Wall         | 22 |

[^1]: Flags 1 represents a block ID, and flags 2 represents a style, which may modify the Frame X or Frame Y.
[^2]: Makes a block into a [[#Developer/Protocol/Packets/Tile Section#Shapes|Half Brick]]
[^3]: `(Flags 1) + 1` represents a  [[Developer/Protocol/Packets/Tile Section#Shapes|Shape]]; except when `0`, then represents no shape.
[^4]: Block Swapping
