using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace TShockAPI.Hooks
{
    public class PlayerLoginEventArgs
    {
        public TSPlayer Player { get; set; }
        public PlayerLoginEventArgs(TSPlayer ply)
        {
            Player = ply;
        }
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
        public delegate void PlayerLoginD(PlayerLoginEventArgs e);
        public static event PlayerLoginD PlayerLogin;
        public delegate void PlayerCommandD(PlayerCommandEventArgs e);
        public static event PlayerCommandD PlayerCommand;

        public static void OnPlayerLogin(TSPlayer ply)
        {
            if(PlayerLogin == null)
            {
                return;
            }

            PlayerLoginEventArgs args = new PlayerLoginEventArgs(ply);
            PlayerLogin(args);
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
    }
}
