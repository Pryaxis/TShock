###### ID
14

###### Direction
Server -> Client

### Structure
| Description | Type |
|-------------|------|
| Player ID | byte |
| Active    | bool |

###### Description

If the player is going from active to not active, then the client will simply mark them as not active, and call `Player.Hooks.PlayerDisconnect`.

If the player is going from not active to active, then the client will reconstruct the entire `Terraria.Player` object (resetting everything about the player), and call `Player.Hooks.PlayerConnect`.