The following parameters can be added to TShock to alter the way a server initializes. Options set on the command line override any of their counterparts in the config file. These can be used either for personal use or in a GSP environment for easier hosting without hassle:

* `-ip` - Starts the server bound to a given IPv4 address. For example: `-ip 0.0.0.0` will bind all interfaces. In contrast, `-ip 127.0.0.1` will bind only `127.0.0.1`. If your interface has another IP address assigned to it, you should be able to bind that.
* `-port` - Starts the server bound to a given port. For example: `-port 7777` will use the default port for Terraria. `-port 25565` will use the Minecraft default port. Ports below 1000 usually require administrator or root privileges to bind on most operating systems.
* `-maxplayers` - Starts the server with a given player count. For example: `-maxplayers 5` sets the maximum number of players to 5. The theoretical maximum is `255`. You can set `-maxplayers 255` for this.
* `-world` - Starts the server and immediately loads a given world file. Note: this parameter expects an absolute path. For example: `-world C:\Terraria\MyWorld.wld` or `-world /root/MyWorld.wld`.
* `-worldselectpath` - Starts the server and changes the location for worlds to be the specified path. The server will look in this path for worlds to load. For example: `-worldselectpath /root` or `-worldselectpath C:\Terraria`.
* `-worldname` - Starts the server using the world name that exists in the set `world select path`. For example, if `MyWorld.wld` is inside `C:\Terraria\` and `-worldselectpath C:\Terraria\` is set, then `-worldname MyWorld` will load that world.
* `-autocreate` - Starts the server and, if a world file isn't found, automatically create the world file with a given size, 1-3, 1 being small. For example, set `-autocreate 3` to create a large world.
* `-config` - Starts the server with a given config file.
* `-connperip` - Allows n number of connections per IP.
* `-killinactivesocket` - Kills connections which have not started the protocol handshake.
* `-ignoreversion` - Ignores API version checks for plugins allowing for old plugins to run.
* `-forceupdate` - Forces the server to continue running, and not hibernating when no players are on. This results in time passing, grass growing, and cpu running.
* `-configpath` - Specifies the path tshock uses to resolve configs, log files, and sqlite db.
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
