###### ID 7
###### Server -> Client
| Description | Type |
|-------------|------|
| Time                        | int |
| Day State                   | byte |
| Moon Phase                  | byte |
| Max Tiles X                 | short |
| Max Tiles Y                 | short |
| Spawn X                     | short |
| Spawn Y                     | short |
| Surface                     | short |
| Rock Layer                  | short |
| ID                          | int |
| Name                        | [[String]] |
| Game Mode                   | byte |
| UUID                        | `byte[16]`[^1] |
| Generator Version           | ulong |
| Moon Type                   | byte |
| Tree Backgrounds            | `byte[4]` |
| Corruption Background       | byte |
| Jungle Background           | byte |
| Snow Background             | byte |
| Hallow Background           | byte |
| Crimson Background          | byte |
| Desert Background           | byte |
| Ocean Background            | byte |
| Mushroom Background         | byte |
| Underworld Background       | byte |
| Ice Back Style              | byte |
| Jungle Back Style           | byte |
| Hell Back Style             | byte |
| Target Wind Speed           | float |
| Number of Clouds            | byte |
| Tree X                      | `int[3]` |
| Tree Style                  | `byte[4]` |
| Cave Back X                 | `int[3]` |
| Cave Back Style             | `byte[4]` |
| Tree Top Variations         | `byte[13]` |
| Max Raining                 | float |
| Flags 1                     | byte |
| Flags 2                     | byte |
| Flags 3                     | byte |
| Flags 4                     | byte |
| Flags 5                     | byte |
| Flags 6                     | byte |
| Flags 7                     | byte |
| Copper Ore Tier             | ushort |
| Iron Ore Tier               | ushort |
| Silver Ore Tier             | ushort |
| Cobalt Ore Tier             | ushort |
| Mythril Ore Tier            | ushort |
| Adamantite Ore Tier         | ushort |
| Invasion Type               | byte |
| Lobby ID                    | ulong[^2] |
| Intended Sandstorm Severity | float |


[^1]: These bytes represent a .NET `System.Guid`
[^2]: Usually a Steam lobby ID, otherwise `0`