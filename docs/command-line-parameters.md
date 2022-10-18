The following parameters can be added to TShock to alter the way a server initializes. Options set on the command line override any of their counterparts in the config file. These can be used either for personal use or in a GSP environment for easier hosting without hassle:

* `-ip` - Starts the server bound to a given IPv4 address.
* `-port` - Starts the server bound to a given port.
* `-maxplayers` - Starts the server with a given player count.
* `-world` <file.wld> - Starts the server and immediately loads a given world file. Note: this parameter expects an absolute path.
* `-worldselectpath` - Starts the server and changes the location for worlds to be the specified path. The server will look in this path for worlds to load.
* `-autocreate` <1/2/3> - Starts the server and, if a world file isn't found, automatically create the world file with a given size, 1-3, 1 being small.
* `-config` - Starts the server with a given config file.
* `-connperip` - Allows n number of connections per IP.
* `-killinactivesocket` - Kills connections which have not started the protocol handshake.
* `-ignoreversion` - Ignores API version checks for plugins allowing for old plugins to run.
* `-forceupdate` - Forces the server to continue running, and not hibernating when no players are on. This results in time passing, grass growing, and cpu running.
* `-configpath` - Specifies the path tshock uses to resolve configs, log files, and sqlite db.
* `-worldpath` - Specifies the path that Terraria Server uses to find all world files.
* `-logpath` - Overrides the default log path and saves logs here.
* `-logformat` - Changes the format of the name of log files, subject to C# date standard abbreviations.
* `-logclear` <true/false> - Overwrites old config if it exists.
* `-dump` - Dumps permissions and config file descriptions for wiki use.
* `-worldevil` - Sets the world's evil state (`-1` for `random`, `0` for `corrupt`, `1` for crimson). This only affects new worlds.
* `-heaptile` - Runs the server with the heaptile tile provider. HeapTile is an alternative ITile provider that uses less memory than the default. This is experimental and may cause issues.
* `-constileation` or `-c` - Runs the server with the Constileation tile provider. Constileation is an alternative ITile provider that uses less memory than the default. This is experimental and may cause issues.

These command line flags are in-addition to the ones that the Terraria server offers (for example, `-lang` is now a vanilla flag, and still works).

## Autostarting TShock

If you want to start TShock automatically though a script, and bypass the interactive startup prompt, you need to specify a `-world` path, a `-port`, and `-maxplayers`.

For example: `TShock.Server.exe -world C:\Terraria\worlds\MyWorld.wld -port 7777 -maxplayers 8`
