###### ID
34

###### Direction
Server <-> Client

### Structure
| Description | Type |
|-------------|------|
| [[#Action]] | byte |
| Tile X      | short |
| Tile Y      | short |
| Style       | short |
| Chest ID[^1] | short |

###### Action
| Description | Value |
|-------------|------|
| Place Chest       | 1 |
| Kill Chest        | 2 |
| Place Dresser     | 3 |
| Kill Dresser      | 4 |
| Place Containers2 | 5 |
| Kill Containers2  | 6 |

[^1]: This value will always become `0` if a vanilla server receives this packet.