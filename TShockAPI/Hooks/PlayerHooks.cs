/*
TShock, a server mod for Terraria
Copyright (C) 2011-2013 Nyx Studios (fka. The TShock Team)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.ComponentModel;

namespace TShockAPI.Hooks
{
    public class PlayerPostLoginEventArgs
    {
        public TSPlayer Player { get; set; }
        public PlayerPostLoginEventArgs(TSPlayer ply)
        {
            Player = ply;
        }
    }

    public class PlayerPreLoginEventArgs : HandledEventArgs
    {
        public TSPlayer Player { get; set; }
        public string LoginName { get; set; }
        public string Password { get; set; }
    }

    public class PlayerCommandEventArgs : HandledEventArgs
    {
        public TSPlayer Player { get; set; }
        public string CommandName { get; set; }
        public string CommandText { get; set; }
        public List<string> Parameters { get; set; }
    }

    public static class PlayerHooks
    {
        public delegate void PlayerPostLoginD(PlayerPostLoginEventArgs e);
        public static event PlayerPostLoginD PlayerPostLogin;

        public delegate void PlayerPreLoginD(PlayerPreLoginEventArgs e);
        public static event PlayerPreLoginD PlayerPreLogin;

        public delegate void PlayerCommandD(PlayerCommandEventArgs e);
        public static event PlayerCommandD PlayerCommand;

        public static void OnPlayerPostLogin(TSPlayer ply)
        {
            if(PlayerPostLogin == null)
            {
                return;
            }

            PlayerPostLoginEventArgs args = new PlayerPostLoginEventArgs(ply);
            PlayerPostLogin(args);
        }

        public static bool OnPlayerCommand(TSPlayer player, string cmdName, string cmdText, List<string> args)
        {
            if (PlayerCommand == null)
            {
                return false;
            }
            PlayerCommandEventArgs playerCommandEventArgs = new PlayerCommandEventArgs()
            {
                Player = player,
                CommandName = cmdName,
                CommandText = cmdText,
                Parameters = args

            };
            PlayerCommand(playerCommandEventArgs);
            return playerCommandEventArgs.Handled;
        }

        public static bool OnPlayerPreLogin(TSPlayer ply, string name, string pass)
        {
            if (PlayerPreLogin == null)
                return false;

            var args = new PlayerPreLoginEventArgs {Player = ply, LoginName = name, Password = pass};
            PlayerPreLogin(args);
            return args.Handled;
        }
    }
}
