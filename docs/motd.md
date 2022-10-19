TShock provides a more configurable "message-of-the-day" (MOTD) system than the Terraria experience has by default. The "message-of-the-day" is displayed to users when the run the `/motd` command or when they join the server for the first time. You can configure this by adjusting the `motd.txt` file in the `tshock` configuration folder (near `config.json`). You can also use the `/reload` command to reload this configuration file after it has been changed, while the server is running.

The default message-of-the-day is this:

```
Welcome to [c/ffff00:%map%] on [c/7ddff8:T][c/81dbf6:S][c/86d7f4:h][c/8ad3f3:o][c/8ecef1:c][c/93caef:k] for [c/55d284:T][c/62d27a:e][c/6fd16f:r][c/7cd165:r][c/89d15a:a][c/95d150:r][c/a4d145:i][c/b1d03b:a].
[c/FFFFFF:Online players (%onlineplayers%/%serverslots%):] [c/FFFF00:%players%]
Type [c/55D284:%specifier%][c/62D27A:h][c/6FD16F:e][c/7CD165:l][c/89D15A:p] for a list of commands.
```

The following tokens are available for use in the MOTD:
* `%map%` - The name of the current map
* `%onlineplayers%` - The number of players currently online
* `%players%` - A comma-separated list of players currently online
* `%serverslots%` - The number of slots on the server
* `%specifier%` - The command specifier (e.g. `/` or `.`)

In addition, the Terraria color codes are supported. These are RGB colors in brackets. For example, `[c/FF0000:Red]` will display the word "Red" in red. [Terraria chat tags](https://terraria.fandom.com/wiki/Chat#Tags) are clientside rendered, meaning that they can be used too, even if more tags are added, without explicit TShock support.
