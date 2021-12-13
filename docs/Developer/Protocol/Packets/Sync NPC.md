###### ID
23

###### Direction
Server -> Client

### Structure
| Description | Type |
|-------------|------|
| NPC ID                               | short |
| Position                             | [[Vector2]] |
| Velocity                             | [[Vector2]] |
| [[#Bitfield 1]]                      | byte |
| [[#Bitfield 2]]                      | byte |
| AI 1[^1]                              | float |
| AI 2[^1]                              | float |
| AI 3[^1]                              | float |
| AI 4[^1]                              | float |
| Type                                 | short |
| Scale Stats for This Many Players[^2] | byte |
| Strength Multiplier[^3]               | float |
| Health Size[^4]                       | byte |
| Health[^4][^5]                       | sbyte or short or int |
| Release Owner[^6]                     | byte |

###### Bitfield 1
| Description | Value |
|-------------|------|
| Direction        | `1 << 0` |
| Direction Y      | `1 << 1` |
| Has AI 1         | `1 << 2` |
| Has AI 2         | `1 << 3` |
| Has AI 3         | `1 << 4` |
| Has AI 4         | `1 << 5` |
| Sprite Direction | `1 << 6` |
| Is at Max Health | `1 << 7` |

###### Bitfield 2
| Description | Value |
|-------------|------|
| Has Scaled Stats for This Many Players | `1 << 0` |
| Spawned From Statue                    | `1 << 1` |
| Has Strength Multiplier                | `1 << 2` |

[^1]: Only present if its corresponding bit is set on [[#Bitfield 1]]
[^2]: Only present if `Has Scaled Stats for This Many Players` bit is set on [[#Bitfield 2]]
[^3]: Only present if `Has Strength Multiplier` bit is set on [[#Bitfield 1]]
[^4]: Not present if `Is at Max Health` bit is set on [[#Bitfield 1]]
[^5]: If `Health Size` is `1`, this is an sbyte. If it is `2`, then it is a short. If it is `4`, then it is an int.
[^6]: Only present if the sent `Type` is catchable
