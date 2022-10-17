###### ID
33

###### Direction
Server <-> Client

### Structure
| Description | Type |
|-------------|------|
| Chest ID    | short |
| Chest X     | short |
| Chest Y     | short |
| Name Length | byte |
| Name[^1]     | [[String]] |

[^1]: Only present if `Name Length >= 0 && <= 20`

###### Description
A `Name Length` of 255 if used to specify removing a chest's name.