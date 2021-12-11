###### ID
21[^1]

###### Direction
Server <-> Client

### Structure
| Description | Type |
|-------------|------|
| Item ID            | short |
| Position           | [[Vector2]] |
| Velocity           | [[Vector2]] |
| Stack              | short |
| Prefix             | byte |
| No Pickup Delay[^2] | byte |
| Type               | short |

[^1]: If `90` is used, this will become an instanced item. Instanced items only exist on the receiving client, and are completely unmanaged by the server.
[^2]: If `1`, this item will not have pickup delay.