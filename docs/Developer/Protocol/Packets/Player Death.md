###### ID 118
###### Server <-> Client
| Description | Type |
|-------------|------|
| Player ID[^1] | byte |
| Reason       | [[Player Death Reason]] |
| Damage       | short |
| Direction    | byte |
| [[#Flags]]   | byte |

[^1]: This represents who is getting attacked

##### Flags
| Description | Value |
|-------------|------|
| PvP  | `1 << 0` |