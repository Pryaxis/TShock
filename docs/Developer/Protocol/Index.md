## Endianness
Unless otherwise specified, all data types are little-endian ordered.

## Sending Packets Across the Network
When packets are actually written and read, they are prefixed with their length and ID.

| Description | Type |
|-------------|------|
| Length      | ushort |
| Packet ID   | byte |
| Data        | `byte[Length - 3]` |

## Connection Process
TODO: Document the packets used and process that goes into connecting to a server, or accepting a client (both sides).

## Packets
| ID  | Name                                   |
|-----|----------------------------------------|
| 1   | [[Connect Request]]                    |
| 2   | [[Disconnect]]                         |
| 3   | [[Set User Slot]]                      |
| 4   | [[Player Info]]                        |
| 5   | [[Sync Inventory Slot]]                |
| 6   | [[Request World Data]]                 |
| 7   | [[World Data]]                         |
| 8   | [[Spawn Data]]                         |
| 9   | [[Status Text]]                        |
| 10  | [[Tile Section]]                       |
| 11  | [[Tile Frame Section]]                 |
| 12  | [[Spawn Player]]                       |
| 13  | [[Sync Player]]                        |
| 14  | [[Player Active]]                      |
| 15  | Undefined                              |
| 16  | [[Player HP]]                          |
| 17  | [[Modify Tile]]                        |
| 18  | [[World Time]]                         |
| 19  | [[Toggle Door]]                        |
| 20  | [[Send Tile Square]]                   |
| 21  | [[Update Dropped Item]]                |
| 22  | [[Update Item Owner]]                  |
| 23  | [[Update NPC]]                         |
| 24  | [[Strike NPC with Held Item]]          |
| 25  | Undefined                              |
| 26  | Undefined                              |
| 27  | [[Update Projectile]]                  |
| 28  | [[Strike NPC]]                         |
| 29  | [[Destroy Projectile]]                 |
| 30  | [[Toggle PvP]]                         |
| 31  | [[Open Chest]]                         |
| 32  | [[Update Chest Item]]                  |
| 33  | [[Set Active Chest]]                   |
| 34  | [[Place Chest]]                        |
| 35  | [[Heal Effect]]                        |
| 36  | [[Player Zone]]                        |
| 37  | [[Password Prompt]]                    |
| 38  | [[Send Password]]                      |
| 39  | [[Remove Item Owner]]                  |
| 40  | [[Npc Talk]]                           |
| 41  | [[Player Item Animation]]              |
| 42  | [[Player Mana]]                        |
| 43  | [[Effect Mana]]                        |
| 44  | Undefined                              |
| 45  | [[Player Team]]                        |
| 46  | [[Sign Read]]                          |
| 47  | [[Sign New]]                           |
| 48  | [[Liquid Set]]                         |
| 49  | [[Player Spawn Self]]                  |
| 50  | [[Player Buff]]                        |
| 51  | [[Npc Special]]                        |
| 52  | [[Chest Unlock]]                       |
| 53  | [[Npc Add Buff]]                       |
| 54  | [[Npc Update Buff]]                    |
| 55  | [[Player Add Buff]]                    |
| 56  | [[Update NPC Name]]                    |
| 57  | [[Update Good Evil]]                   |
| 58  | [[Play Harp]]                          |
| 59  | [[Hit Switch]]                         |
| 60  | [[Update NPC Home]]                    |
| 61  | [[Spawn Bossor Invasion]]              |
| 62  | [[Player Dodge]]                       |
| 63  | [[Paint Tile]]                         |
| 64  | [[Paint Wall]]                         |
| 65  | [[Teleport]]                           |
| 66  | [[Player Heal Other]]                  |
| 67  | [[Placeholder]]                        |
| 68  | [[Client UUID]]                        |
| 69  | [[Chest Name]]                         |
| 70  | [[Catch NPC]]                          |
| 71  | [[Release NPC]]                        |
| 72  | [[Travelling Merchant Inventory]]      |
| 73  | [[Teleportation Potion]]               |
| 74  | [[Angler Quest]]                       |
| 75  | [[Complete Angler Quest]]              |
| 76  | [[Number Of Angler Quests Completed]]  |
| 77  | [[Create Temporary Animation]]         |
| 78  | [[Report Invasion Progress]]           |
| 79  | [[Place Object]]                       |
| 80  | [[Sync Player Chest Index]]            |
| 81  | [[Create Combat Text]]                 |
| 82  | [[Load Net Module]]                    |
| 83  | [[Npc Kill Count]]                     |
| 84  | [[Player Stealth]]                     |
| 85  | [[Force Item Into Nearest Chest]]      |
| 86  | [[Update Tile Entity]]                 |
| 87  | [[Place Tile Entity]]                  |
| 88  | [[Tweak Item]]                         |
| 89  | [[Place Item Frame]]                   |
| 90  | [[Update Item Drop]]                   |
| 91  | [[Emote Bubble]]                       |
| 92  | [[Sync Extra Value]]                   |
| 93  | [[Social Handshake]]                   |
| 94  | Deprecated                             |
| 95  | [[Kill Portal]]                        |
| 96  | [[Player Teleport Portal]]             |
| 97  | [[Notify Player Npc Killed]]           |
| 98  | [[Notify Player Of Event]]             |
| 99  | [[Update Minion Target]]               |
| 100 | [[Npc Teleport Portal]]                |
| 101 | [[Update Shield Strengths]]            |
| 102 | [[Nebula Level Up]]                    |
| 103 | [[Moon Lord Countdown]]                |
| 104 | [[Npc Shop Item]]                      |
| 105 | [[Gem Lock Toggle]]                    |
| 106 | [[Poof Of Smoke]]                      |
| 107 | [[Smart Text Message]]                 |
| 108 | [[Wired Cannon Shot]]                  |
| 109 | [[Mass Wire Operation]]                |
| 110 | [[Mass Wire Operation Pay]]            |
| 111 | [[Toggle Party]]                       |
| 112 | [[Tree Grow FX]]                       |
| 113 | [[Crystal Invasion Start]]             |
| 114 | [[Crystal Invasion Wipe All]]          |
| 115 | [[Minion Attack Target Update]]        |
| 116 | [[Crystal Invasion Send Wait Time]]    |
| 117 | [[Player Hurt]]                        |
| 118 | [[Player Death]]                       |
| 119 | [[Create Combat Text Extended]]        |
| 120 | [[Emoji]]                              |
| 121 | [[Tile Entity Display Doll Item Sync]] |
| 122 | [[Request Tile Entity Interaction]]    |
| 123 | [[Weapons Rack Try Placing]]           |
| 124 | [[Tile Entity Hat Rack Item Sync]]     |
| 125 | [[Sync Tile Picking]]                  |
| 126 | [[Sync Revenge Marker]]                |
| 127 | [[Remove Revenge Marker]]              |
| 128 | [[Land Golf Ball In Cup]]              |
| 129 | [[Finished Connecting To Server]]      |
| 130 | [[Fish Out NPC]]                       |
| 131 | [[Tamper With NPC]]                    |
| 132 | [[Play Legacy Sound]]                  |
| 133 | [[Food Platter Try Placing]]           |
| 134 | [[Update Player Luck Factors]]         |
| 135 | [[Dead Player]]                        |
| 136 | [[Sync Cavern Monster Type]]           |
| 137 | [[Request NPC Buff Removal]]           |
| 138 | [[Client Synced Inventory]]            |
| 139 | [[Set Counts As Host For Gameplay]]    |
| 140 | [[Set Misc Event Values]]              |
| 141 | [[Request Lucy Popup]]                 |
| 142 | [[Sync Projectile Trackers]]           |
