###### ID
5

###### Direction
Server <-> Client[^1]

### Structure
| Description | Type |
|-------------|------|
| Player ID       | byte |
| [[Slots\|Slot]] | short |
| Item Stack      | short |
| Item Prefix     | sbyte |
| Item ID         | short |

[^1]: If the receiving Player ID is themself, [[Server Side Characters]] must be enabled.