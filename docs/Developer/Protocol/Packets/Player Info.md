###### ID 4
###### Server <-> Client
| Description | Type |
|-------------|------|
| Player ID             | byte |
| Skin Variant          | byte |
| Hair                  | byte |
| Name                  | [[String]] |
| Hide Visuals          | byte |
| Hide Visuals 2        | byte |
| Hide Misc             | byte |
| Hair Color            | [[Color]] |
| Skin Color            | [[Color]] |
| Eye Color             | [[Color]] |
| Shirt Color           | [[Color]] |
| Undershirt Color      | [[Color]] |
| Pants Color           | [[Color]] |
| Shoe Color            | [[Color]] |
| [[#Difficulty Flags]] | byte |
| [[#Torch Flags]]      | byte |

##### Difficulty Flags
| Description | Value |
|-------------|------|
| Softcore                  | `1 << 0` |
| Mediumcore                | `1 << 1` |
| Has Expert Accessory Slot | `1 << 2` |
| Hardcore                  | `1 << 3` |

##### Torch Flags
| Description | Value |
|-------------|-------|
| Using Biome Torches    | `1 << 0` |
| Happy Fun Torch Time   | `1 << 1` |
| Unlocked Biome Torches | `1 << 2` |