# TShock for Terraria

This is the rolling changelog for TShock for Terraria. Use past tense when adding new entries; sign your name off when you add or change something. This should primarily be things like user changes, not necessarily codebase changes unless it's really relevant or large.

## Unreleased

* Auth system kicks players if system is disabled. (@nicatronTg)
* Fixed /login permitting multiple logins without a logout in between. (@nicatronTg)
* Allow[Hallow/Corruption/Crimson]Creep in config now work. (@WhiteXZ)
* API: Treasure bags are now named properly. (@WhiteXZ)
* API: Clients no longer close on disconnect. (@Wolfje)
* API: Add server broadcast hook. (@Patrikk)
* API: Fixed pressure plate hook triggering multiple times. (@Patrikk)
* API: Fixed issues with SendQ writes failing. (@Wolfje)

## TShock 4.3.6 (Pre-Release)

* API: NPCs shoot the right way (@WhiteXZ)
* API: The server config file works correctly with priority and port (@Patrikkk)
* API: Removed support for favorites and removed JSON dependencies. (@Enerdy)
* API: Removed support for clouds. (@Enerdy)
* API: Fixed a whole lot of bugs with wiring, and in general re-wrote some core bits that were bugged. (@WhiteXZ)
* API: Fixed projectile AI bugs. (@AndrioCelos)
* API: Fixed world saving problems. (WhiteXZ)
* API: Fixed server not accepting more connections once max slots was filled. (@WhiteXZ)
* API: Removed startup parameters and moved them to TShock. (@Cleant)
* API: Item.SetDefaults() no longer kills some tools. (@Enerdy)
* API: Restored chat bubbles. (@WhiteXZ)
* API: Updated to 1.3.0.6. (@Enerdy & @Patrikkk)
* API: Lots and I mean lots of network improvements in the SendQ department. (@tylerjwatson)
* API: Added NpcLootDrop and DropBossBag hooks. (@Patrikkk)
* API: Fixed hook: NpcTriggerPressurePlate (@Patrikkk)
* API: Fixed hook: ProjectileTriggerPressurePlate (@Patrikkk)
* API: Fixed hook: ItemSetDefaultsString (@Patrikkk)
* API: Fixed hook: ItemSetDefaultsInt (@Patrikkk)
* API: Fixed hook: ItemNetDefaults (@Patrikkk)
* API: Fixed hook: GameStatueSpawn (@Patrikkk)
* API: Fixed hook: NpcNetDefaults (@Patrikkk)
* API: Fixed hook: NpcNetSetDefaultsString (@Patrikkk)
* API: Fixed hook: NpcNetSetDefaultsInt (@Patrikkk)
* API: Fixed hook: NpcSpawn (@Patrikkk)
* API: Fixed hook: NpcTransformation (@Patrikkk)
* API: Fixed hook: NpcStrike (@Patrikkk)
* API: Updated AssemblyInfo to 1.3.0.6. (@nicatronTg)
* API: Moved to .NET Framework 4.5. (@tylerjwatson)
* API: Dedicated server input thread doesn't run if input is redirected/piped. (@tylerjwatson)
* API: Wiring.cs methods are now public. (@Stealownz)
* API: Added PlayerTriggerPressurePlate hook. (@Patrikkk)
* API: API Version Tick to 1.20.
* The config option disabling the DCU has been deprecated and will be removed in a future release. (@nicatronTg)
* Fixed bubble tile triggering noclip checks. (@Enerdy)
* Updated projectile handling in GetDataHandlers. (@WhiteXZ)
* Fixed issue #992. (@WhiteXZ)
* Teleport handler now handles wormholes. (@WhiteXZ)
* Fixed tall gates and trap doors (issue #998). (@WhiteXZ)
* Added monoliths to orientable tiles (issue #999). (@WhiteXZ)
* Fixed vortex stealth armor (issue #964). (@WhiteXZ)
* Added moon lord to spawn boss. (@WhiteXZ)
* Fixed serverpassword syntax error error message. (@JordyMoos)
* Fixed issue #1019. (@WhiteXZ)
* Fix: Region protection prevents placement of objects. (@Patrikkk)
* Moved all startup parameters to TShock. (@Cleant)
* Fix: Target dummies are no longer butchered. (@Denway)
* Added projectile 465 to the ignore list, which fixes some other issues. (@Enerdy)
* Fix: Logging out is now safe with SSC (/logout) (issue #1037). (@WhiteXZ)
* API/TShock: Removed -world parameter from TShock, put it back in the API. (@tylerjwatson)

## TShock 4.3.5 (Unreleased)

* Fix HandleSpawnBoss, and as a result the spawnboss command and boss spawning items. (@Ijwu)
* Rewrite SendQ for more network stack improvements (@tylerjwatson)
* Update to Terraria 1.3.0.5 (@Patrikkk)

## TShock 4.3.4

* Fix invasion progress messages (@WhiteXZ)
* Completely rewrite SendQ to have less deadlocks (@tylerjwatson)

## TShock 4.3.3

* Fix dihydrogen monoxide (@tylerjwatson)
* Whitelist another boss projectile (@Patrikkk, @WhiteXZ)

## TShock 4.3.2

* Fixed the issue where using the Super Absorbent Sponge would disable users (@WhiteXZ)
* Fixed an issue in NetGetData where e.Length - 1 would be -1 (@WhiteXZ)
* Fixed /who -i and /userinfo (@Enerdy)
* API: OnRegionEntered hook now returns the region entered (@Patrikkk)
* Support for Terraria 1.3.0.4 (@nicatronTg)
* Fixed dressers being unbreakable. (@nicatronTg)
* Fixed wall placement mechanics (@nicatronTg, @Ijwu, @WhiteXZ)
* Fixed Moon Lord projectiles disabling players (@k0rd, @nicatronTg)
* Fixed several potential crashes in server (@Patrikkk)
* Fixed -autocreate command line argument (@WhiteXZ, @nicatronTg)
* Added more world data to world load menu (@WhiteXZ)
* Moved server password to TShock config (@Enerdy)
* Fixed world delete in server (@benjiro)
* Fixed disappearing NPCs (@WhiteXZ)
* Added much more performant code, SendQ, to server module. Reduces downstream network overhead by at least 40% (@tylerjwatson)
* API: Updated TSPlayer.Disable to use new buffs (@Enerdy)
* Updated default max damage & projectile damage to 1,175 (based on 625 people)
* Fixed support for SSC (@WhiteXZ)

## TShock 4.3.1

* Fixed a bug where /user group failing would output no error. (@nicatronTg)
* Fixed a bug where /user group would fail. @(Enerdy)
* Added the ability to disable backup autosave messages. (@nicatronTg)
* Fixed /buff malfunctioning when entering an invalid buff name. (@Enerdy)
* Fixed projectiles 435-438 (martian invasion) freezing everyone under certain conditions. (@Enerdy)
* DisableTombstones now works properly with the new golden gravestones. (@Enerdy)
* REST module now properly catches exceptions during Start(). (@Patrikkk)
* Added /expert command to toggle expert mode. (@WhiteXZ)
* Fixed pirate invasions. (@patrik)
* Fixed worldinfo packet. (@WhiteXZ)
* Fixed server passwords. (@Enerdy)

## TShock 4.3.0.0

* API: Modifed NetItem so that it's actually useful. (@MarioE)
* Updated prebuilts (SQLite, JSON, MySQL) to latest versions. (@nicatronTg)
* Added a minimum password length to prevent blank passwords. (@nicatronTg)
* Modified item ban checks to provide which item is disabling a player in the logs. (@Enerdy)
* API: Modified TSPlayer to store a user, and deprecated calls to TSPlayer.User.ID. (@WhiteXZ)
* Modified chat color specs in config file to be int arrays rather than floats. (@nicatronTg)
* Modified verbiage for ```/auth``` and ```/auth-verify``` to make it clearer how they operate. (@nicatronTg)
* API: Added fuzzy name searching for users. (@WhiteXZ)
* API: Fixed ```OnPlayerLogout``` not being fired when a player disconnects. (@nicatronTg)
* API: Deprecated ```ValidString``` and ```SanitizeString``` methods in Utils. (@nicatronTg)
* Added BCrypt password hashing and related systems for it. BCrypt replaces the old system using non-password hashing algorithms for storing passwords. It breaks implementations of the login code that were manually recreated, but is otherwise seamless in transition. (@nicatronTg)
* API: Added ```User.VerifyPassword(string password)``` which verifies if the user's password matches their stored hash. It automatically upgrades a users' password to BCrypt if called and the password stored is not a BCrypt hash. (@nicatronTg)
* API: Deprecated ```Utils.HashPassword``` and related password hashing functions as those are no longer needed for plugin access. (@nicatronTg)
* Fixed ```UseServerName``` config option so that it correctly sends the config server name any time that Main.WorldName is used. (@Olink)
* Fixed a bug where people could ban themselves. (@nicatronTg)
* Fixed a bug where banning a player who never logged in caused problems. (@nicatronTg)
* Terraria 1.3.0.3 support.
