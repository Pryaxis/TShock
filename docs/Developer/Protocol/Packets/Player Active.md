###### ID 14
###### Server -> Client
| Description | Type |
|-------------|------|
| Player ID | byte |
| Active    | bool |

If the player is going from active to not active, then the client will simply mark them as not active, and call `Player.Hooks.PlayerDisconnect`.

If the player is going from not active to active, then the client will reconstruct the entire `Terraria.Player` object (resetting everything about the player), and call `Player.Hooks.PlayerConnect`.