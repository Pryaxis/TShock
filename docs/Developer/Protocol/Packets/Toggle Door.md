###### ID
19

###### Direction
Server <-> Client

### Structure
| Description | Type |
|-------------|------|
| Action[^1] | byte |
| Tile X    | short |
| Tile Y    | short |
| Direction | byte |

###### Action
| Description | Value |
|-------------|------|
| Open Door       | 0 |
| Close Door      | 1 |
| Open Trapdoor   | 2 |
| Close Trapdoor  | 3 |
| Open Tall Gate  | 4 |
| Close Tall Gate | 5 |