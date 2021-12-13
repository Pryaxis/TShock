###### ID
13

###### Direction
Server <-> Client[^1]

### Structure
| Description | Type |
|-------------|------|
| Player ID                             | byte |
| [[#Controls]]                         | byte |
| [[#Bitfield 2]]                       | byte |
| [[#Bitfield 3]]                       | byte |
| [[#Bitfield 4]]                       | byte |
| Selected [[Slots\|Slot]]              | byte |
| Position                              | [[Vector2]] |
| Velocity[^2]                           | [[Vector2]] |
| Potion of Return Original Position[^3] | [[Vector2]] |
| Potion of Return Home Position[^3]     | [[Vector2]] |

###### Controls
| Description | Value |
|-------------|------|
| Up        | `1 << 0` |
| Down      | `1 << 1` |
| Left      | `1 << 2` |
| Right     | `1 << 3` |
| Jump      | `1 << 4` |
| Use Item  | `1 << 5` |
| Direction | `1 << 6` |

###### Bitfield 2
| Description | Value |
|-------------|------|
| Has Pulley            | `1 << 0` |
| Pulley Direction      | `1 << 1` |
| Has Velocity          | `1 << 2` |
| Vortex Stealth Active | `1 << 3` |
| Gravity Direction     | `1 << 4` |
| Should Shield Guard   | `1 << 5` |
| Ghost                 | `1 << 6` |

###### Bitfield 3
| Description | Value |
|-------------|------|
| Try Keeping Hoverboard Up   | `1 << 0` |
| Is Void Vault Enabled       | `1 << 1` |
| Is Sitting                  | `1 << 2` |
| Downed Any DD2 Difficulty   | `1 << 3` |
| Is Petting Animal           | `1 << 4` |
| Is Animal Being Pet Small   | `1 << 5` |
| Has Potion of Return Data   | `1 << 6` |
| Try Keeping Hoverboard Down | `1 << 7` |

###### Bitfield 4
| Description | Value |
|-------------|------|
| Is Sleeping | `1 << 0` |

[^1]: If the receiving Player ID is themself, [[Server Side Characters]] must be enabled.
[^2]: Only present if `Has Velocity` bit is set on [[#Bitfield 2]]
[^3]: Only present if `Has Potion of Return Data` bit is set on [[#Bitfield 3]]