###### ID
27

###### Direction
Server <-> Client

### Structure
| Description | Type |
|-------------|------|
| ID                         | short |
| Position                   | [[Vector2]] |
| Velocity                   | [[Vector2]] |
| Owner                      | byte |
| Type                       | short |
| [[#Flags]]                 | byte |
| AI 1[^1]                    | float |
| AI 2[^1]                    | float |
| Banner ID to Respond To[^2] | bruh |
| Damage[^3]                  | bruh |
| Knockback[^4]               | bruh |
| Original Damage[^5]         | bruh |
| UUID[^6]                    | bruh |

###### Flags
| Description | Value |
|-------------|------|
| Has AI 1                    | `1 << 0` |
| Has AI 2                    | `1 << 1` |
| Has Banner ID to Respond To | `1 << 3` |
| Has Damage                  | `1 << 4` |
| Has Knockback               | `1 << 5` |
| Has Original Damage         | `1 << 6` |
| Has UUID                    | `1 << 7` |

[^1]: Only present if its corresponding bit is set on [[#Flags]]
[^2]: Only present if `Has Banner ID to Respond To` bit is set on [[#Flags]]
[^3]: Only present if `Has Damage` bit is set on [[#Flags]]
[^4]: Only present if `Has Knockback` bit is set on [[#Flags]]
[^5]: Only present if `Has Original Damage` bit is set on [[#Flags]]
[^6]: Only present if `Has UUID` bit is set on [[#Flags]]