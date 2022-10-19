Unlike many games (e.g., Minecraft), Terraria, by default, allows players to bring characters from single-player or from other multiplayer servers with them server-to-server. This has benefits and drawbacks. While players can enjoy the freedom of finding items and working with their friends in co-op, it poses obvious security questions. After all, if a player can get items from anywhere, that means they can bring those items into worlds that haven't yet reached that point in game-progression yet. Worse still, they may have acquired the items through illicit means.

Introduced through a collaboration between Zidonuke and Re-Logic, Terraria supports the concept of server-side characters, with a compatible server replacement. The base game supports server-side characters, but does not implement the storage backend or synchronization system. When enabled, game clients no-longer persist data to disk, and give the server authority over many aspects (but not all) of the local data.

When enabled in TShock, SSC takes over control of inventory management for a player. In this mode, players are given a starting inventory, described by the server owner, and then they retain that inventory only as long as they're connected to the server. When they disconnect, their original character data will still be saved locally, ensuring they don't lose local data.

For developers, TShock's SSC implementation should be considered the reference implementation. More things are possible with this system than what TShock does.

## Setting up SSC

To setup SSC, simply change `Enabled` to `true` in `sscconfig.json` in the `tshock` config folder.

An example configuration file is provided:

```json
{
  "Settings": {
    "Enabled": true,
    "ServerSideCharacterSave": 5,
    "LogonDiscardThreshold": 250,
    "StartingHealth": 100,
    "StartingMana": 20,
    "StartingInventory": [
      {
        "netID": -15,
        "prefix": 0,
        "stack": 1
      },
      {
        "netID": -13,
        "prefix": 0,
        "stack": 1
      },
      {
        "netID": -16,
        "prefix": 0,
        "stack": 1
      }
    ],
    "WarnPlayersAboutBypassPermission": true
  }
}
```

In this example configuration, the `StartingInventory` manifest describes the starting items that each player has when they join. In this case, it's bronze equipment. You can customize this by adding additional entries. For example, the updated configuration file after this block adds the `Zenith` as a starting item for new players.

```json
{
  "Settings": {
    "Enabled": true,
    "ServerSideCharacterSave": 5,
    "LogonDiscardThreshold": 250,
    "StartingHealth": 100,
    "StartingMana": 20,
    "StartingInventory": [
      {
        "netID": -15,
        "prefix": 0,
        "stack": 1
      },
      {
        "netID": -13,
        "prefix": 0,
        "stack": 1
      },
      {
        "netID": -16,
        "prefix": 0,
        "stack": 1
      },
      {
        "netID": 4956,
        "prefix": 0,
        "stack": 1
      }
    ],
    "WarnPlayersAboutBypassPermission": true
  }
}
```

## Playing as an admin

If you're playing as an admin, make sure that you're in the `owner` group or a similar group. We really don't suggest playing as `superadmin`. `superadmin` and other users who have the `tshock.ignore.ssc` permission won't use server-side characters. This means that they'll be able to bring items in from their personal character files, and data won't be saved to the server at all.

If a TShock player has `tshock.ignore.ssc`, and `WarnPlayersAboutBypassPermission` is set to `true` in the configuration file, you'll see warnings in your server console indicating that players aren't being saved correctly.

## Uploading data

Sometimes, you want to import player data from players that join your server. For example, if you trust your friends not to bring hacked items in, you can import their data into the system. This is done with the `/uploadssc` command.

The `/overridessc` command can be used to upload SSC data from a given player. The difference between this command is that `/uploadssc` uploads their data from when they joined, whereas `/overridessc` will just save whatever their current state is to the database.

## Limitations

Currently, SSC does not support loadouts (1.4.4.x content).
