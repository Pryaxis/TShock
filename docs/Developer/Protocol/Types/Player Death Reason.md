| Description | Type |
|-------------|------|
| [[#Flags]]         | byte |
| Player ID[^1]       | byte |
| NPC ID[^2]          | short |
| Projectile ID[^3]   | short |
| Other[^4]           | byte |
| Projectile Type[^5] | short |
| Item Type[^6]       | short |
| Item Prefix[^7]     | byte |
| Custom Reason[^8]   | [[String]] |

[^1]: Only present if `Has Player` bit is set on [[#Flags]]
[^2]: Only present if `Has NPC` bit is set on [[#Flags]]
[^3]: Only present if `Has Projectile` bit is set on [[#Flags]]
[^4]: Only present if `Has Other` bit is set on [[#Flags]]
[^5]: Only present if `Has Projectile Type` bit is set on [[#Flags]]
[^6]: Only present if `Has Item Type` bit is set on [[#Flags]]
[^7]: Only present if `Has Item Prefix` bit is set on [[#Flags]]
[^8]: Only present if `Has Custom Reason` bit is set on [[#Flags]]

##### Flags
| Description | Value |
|-------------|------|
| Has Player          | `1 << 0` |
| Has NPC             | `1 << 1` |
| Has Projectile      | `1 << 2` |
| Has Other           | `1 << 3` |
| Has Projectile Type | `1 << 4` |
| Has Item Type       | `1 << 5` |
| Has Item Prefix     | `1 << 6` |
| Has Custom Reason   | `1 << 7` |
