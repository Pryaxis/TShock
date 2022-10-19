## BanCreateV3
Create a new ban entry.
* **Permissions**: `tshock.rest.bans.manage`


**Nouns**:
* `identifier` (Required) `[String]` - The identifier to ban.
* `reason` (Optional) `[String]` - The reason to assign to the ban.
* `start` (Optional) `[String]` - The datetime at which the ban should start.
* `end` (Optional) `[String]` - The datetime at which the ban should end.
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v3/bans/create?identifier=identifier&reason=reason&start=start&end=end&token=token`

## BanDestroyV3
Delete an existing ban entry.
* **Permissions**: `tshock.rest.bans.manage`


**Nouns**:
* `ticketNumber` (Required) `[String]` - The ticket number of the ban to delete.
* `fullDelete` (Optional) `[Boolean]` - Whether or not to completely remove the ban from the system.
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v3/bans/destroy?ticketNumber=ticketNumber&fullDelete=fullDelete&token=token`

## BanInfoV3
View the details of a specific ban.
* **Permissions**: `tshock.rest.bans.view`


**Nouns**:
* `ticketNumber` (Required) `[String]` - The ticket number to search for.
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v3/bans/read?ticketNumber=ticketNumber&token=token`

## BanListV3
View all bans in the TShock database.
* **Permissions**: `tshock.rest.bans.view`


**Nouns**:
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v3/bans/list?token=token`

## GroupCreate
Create a new group.
* **Permissions**: `tshock.rest.groups.manage`


**Nouns**:
* `group` (Required) `[String]` - The name of the new group.
* `parent` (Optional) `[String]` - The name of the parent group.
* `permissions` (Optional) `[String]` - A comma separated list of permissions for the new group.
* `chatcolor` (Optional) `[String]` - A r,g,b string representing the color for this groups chat.
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v2/groups/create?group=group&parent=parent&permissions=permissions&chatcolor=chatcolor&token=token`

## GroupDestroy
Delete a group.
* **Permissions**: `tshock.rest.groups.manage`


**Nouns**:
* `group` (Required) `[String]` - The group name to delete.
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v2/groups/destroy?group=group&token=token`

## GroupInfo
Display information of a group.
* **Permissions**: `tshock.rest.groups.view`


**Nouns**:
* `group` (Required) `[String]` - The group name to get information on.
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v2/groups/read?group=group&token=token`

## GroupList
View all groups in the TShock database.
* **Permissions**: `tshock.rest.groups.view`


**Nouns**:
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v2/groups/list?token=token`

## PlayerKickV2
Kick a player off the server.
* **Permissions**: `tshock.rest.kick`


**Nouns**:
* `player` (Required) `[String]` - The player to kick.
* `reason` (Optional) `[String]` - The reason the player was kicked.
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v2/players/kick?player=player&reason=reason&token=token`

## PlayerKill
Kill a player.
* **Permissions**: `tshock.rest.kill`


**Nouns**:
* `player` (Required) `[String]` - The player to kick.
* `from` (Optional) `[String]` - Who killed the player.
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v2/players/kill?player=player&from=from&token=token`

## PlayerList
List all player names that are currently on the server.
No special permissions are required for this route.


**Nouns**:
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/lists/players?token=token`

## PlayerListV2
Fetches detailed user information on all connected users, and can be filtered by specifying a key value pair filter users where the key is a field and the value is a users field value.
No special permissions are required for this route.


**Nouns**:
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v2/players/list?token=token`

## PlayerMute
Mute a player.
* **Permissions**: `tshock.rest.mute`


**Nouns**:
* `player` (Required) `[String]` - The player to mute.
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v2/players/mute?player=player&token=token`

## PlayerReadV3
Get information for a user.
* **Permissions**: `tshock.rest.users.info`


**Nouns**:
* `player` (Required) `[String]` - The player to lookup
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v3/players/read?player=player&token=token`

## PlayerReadV4
Get information for a user.
* **Permissions**: `tshock.rest.users.info`


**Nouns**:
* `player` (Required) `[String]` - The player to lookup
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v4/players/read?player=player&token=token`

## PlayerUnMute
Unmute a player.
* **Permissions**: `tshock.rest.mute`


**Nouns**:
* `player` (Required) `[String]` - The player to mute.
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v2/players/unmute?player=player&token=token`

## ServerBroadcast
Broadcast a server wide message.
No special permissions are required for this route.


**Nouns**:
* `msg` (Required) `[String]` - The message to broadcast.
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v2/server/broadcast?msg=msg&token=token`

## ServerCommandV3
Executes a remote command on the server, and returns the output of the command.
* **Permissions**: `tshock.rest.command`


**Nouns**:
* `cmd` (Required) `[String]` - The command and arguments to execute.
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v3/server/rawcmd?cmd=cmd&token=token`

## ServerMotd
Returns the motd, if it exists.
No special permissions are required for this route.


**Nouns**:
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v3/server/motd?token=token`

## ServerOff
Turn the server off.
* **Permissions**: `tshock.rest.maintenance`


**Nouns**:
* `confirm` (Required) `[Boolean]` - Required to confirm that actually want to turn the server off.
* `message` (Optional) `[String]` - The shutdown message.
* `nosave` (Optional) `[Boolean]` - Shutdown without saving.
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v2/server/off?confirm=confirm&message=message&nosave=nosave&token=token`

## ServerReload
Reload config files for the server.
* **Permissions**: `tshock.rest.cfg`


**Nouns**:
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v3/server/reload?token=token`

## ServerRules
Returns the rules, if they exist.
No special permissions are required for this route.


**Nouns**:
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v3/server/rules?token=token`

## ServerStatusV2
Get a list of information about the current TShock server.
No special permissions are required for this route.


**Nouns**:
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v2/server/status?token=token`

## ServerTokenTest
Test if a token is still valid.
No special permissions are required for this route.


**Nouns**:
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/tokentest?token=token`

## UserActiveListV2
Returns the list of user accounts that are currently in use on the server.
* **Permissions**: `tshock.rest.users.view`


**Nouns**:
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v2/users/activelist?token=token`

## UserCreateV2
Create a new TShock user account.
* **Permissions**: `tshock.rest.users.manage`


**Nouns**:
* `user` (Required) `[String]` - The user account name for the new account.
* `group` (Optional) `[String]` - The group the new account should be assigned.
* `password` (Required) `[String]` - The password for the new account.
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v2/users/create?user=user&group=group&password=password&token=token`

## UserDestroyV2
Destroy a TShock user account.
* **Permissions**: `tshock.rest.users.manage`


**Nouns**:
* `user` (Required) `[String]` - The search criteria (name or id of account to lookup).
* `type` (Required) `[String]` - The search criteria type (name for name lookup, id for id lookup).
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v2/users/destroy?user=user&type=type&token=token`

## UserInfoV2
List detailed information for a user account.
* **Permissions**: `tshock.rest.users.view`


**Nouns**:
* `user` (Required) `[String]` - The search criteria (name or id of account to lookup).
* `type` (Required) `[String]` - The search criteria type (name for name lookup, id for id lookup).
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v2/users/read?user=user&type=type&token=token`

## UserListV2
Lists all user accounts in the TShock database.
* **Permissions**: `tshock.rest.users.view`


**Nouns**:
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v2/users/list?token=token`

## UserUpdateV2
Update a users information.
* **Permissions**: `tshock.rest.users.manage`


**Nouns**:
* `user` (Required) `[String]` - The search criteria (name or id of account to lookup).
* `type` (Required) `[String]` - The search criteria type (name for name lookup, id for id lookup).
* `password` (Optional) `[String]` - The users new password, and at least this or group must be defined.
* `group` (Optional) `[String]` - The new group for the user, at least this or password must be defined.
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v2/users/update?user=user&type=type&password=password&group=group&token=token`

## WorldBloodmoon
Toggle the status of blood moon.
* **Permissions**: `tshock.rest.causeevents`

**Verbs**:
* `bloodmoon` (Required) `[Boolean]` - State of bloodmoon.

**Nouns**:
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/world/bloodmoon/{bloodmoon}?token=token`

## WorldBloodmoonV3
Toggle the status of blood moon.
* **Permissions**: `tshock.rest.causeevents`


**Nouns**:
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v3/world/bloodmoon?token=token`

## WorldButcher
Butcher npcs.
* **Permissions**: `tshock.rest.butcher`


**Nouns**:
* `killfriendly` (Optional) `[Boolean]` - Should friendly npcs be butchered.
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v2/world/butcher?killfriendly=killfriendly&token=token`

## WorldMeteor
Drops a meteor on the world.
* **Permissions**: `tshock.rest.causeevents`


**Nouns**:
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/world/meteor?token=token`

## WorldRead
Get information regarding the world.
No special permissions are required for this route.


**Nouns**:
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/world/read?token=token`

## WorldSave
Save the world.
* **Permissions**: `tshock.rest.cfg`


**Nouns**:
* `token` (Required) `[String]` - The REST authentication token.

**Example Usage**: `/v2/world/save?token=token`

