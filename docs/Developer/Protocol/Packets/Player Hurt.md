###### ID
117

###### Direction
Server <-> Client

### Structure
| Description | Type |
|-------------|------|
| Player ID[^1]     | byte |
| Reason           | [[Player Death Reason]] |
| Damage           | short |
| Direction        | byte |
| [[#Flags]]       | byte |
| Cooldown Counter | sbyte |

[^1]: This represents who is getting attacked

##### Flags
| Description | Value |
|-------------|------|
| Crit | `1 << 0` |
| PvP  | `1 << 1` |