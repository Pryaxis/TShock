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
        public PlayerChatEventArgs(IPlayer player, messageBuffer msg, string text) : base( player )
        {
            Msg = msg;
            Text = text;
        }
		public IPlayer Player { get; protected set; }
        public messageBuffer Msg { get; protected set; }
        public string Text { get; protected set; }
    }
}
