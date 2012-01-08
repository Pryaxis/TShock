using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShock;
using Terraria;

namespace TShock.Hooks.Player
{
    public class PlayerChatEventArgs : PlayerEventArgs
    {
        public PlayerChatEventArgs(IPlayer player, string text) : base( player )
        {
            Text = text;
        }
        public string Text { get; protected set; }
    }
}
