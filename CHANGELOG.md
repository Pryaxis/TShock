# TShock for Terraria

This is the rolling changelog for TShock for Terraria. Use past tense when adding new entries; sign your name off when you add or change something. This should primarily be things like user changes, not necessarily codebase changes unless it's really relevant or large.

## TShock 4.3.5 (Unreleased)

* Fix HandleSpawnBoss, and as a result the spawnboss command and boss spawning items. (@Ijwu)
* Rewrite SendQ for more network stack improvements (@tylerjwatson)
* Update to Terraria 1.3.0.5 (@Patrikkk)S

## TShock 4.3.4

* Fix invasion progress messages (@WhiteXZ)
* Completely rewrite SendQ to have less deadlocks (@tylerjwatson)

## TShock 4.3.3

* Fix dihydrogen monoxide (@Wolfje)
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
