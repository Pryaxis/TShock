using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Terraria;

namespace TShock.Hooks.Player
{
	public class PlayerHooks
	{
		internal PlayerHooks()
		{
		    Connect = new HandlerList<PlayerEventArgs>();
			Join = new HandlerList<PlayerEventArgs>();
			Greet = new HandlerList<PlayerEventArgs>();
            Leave = new HandlerList<PlayerEventArgs>();
		    Chat = new HandlerList<PlayerChatEventArgs>();

		    TerrariaServer.Hooks.ServerHooks.Connect += ServerHooks_Connect;
			TerrariaServer.Hooks.ServerHooks.Join += ServerHooks_Join;
			TerrariaServer.Hooks.NetHooks.GreetPlayer += NetHooks_GreetPlayer;
		    TerrariaServer.Hooks.ServerHooks.Leave += ServerHooks_Leave;
            TerrariaServer.Hooks.ServerHooks.Chat += ServerHooks_Chat;
		}

        void ServerHooks_Connect( int who, HandledEventArgs args )
        {
            var e = new PlayerEventArgs(new TShockPlayer(who));
            Connect.Invoke(this, e);
            args.Handled = e.Handled;
        }

		void NetHooks_GreetPlayer(int who, HandledEventArgs arg2)
		{
			var e = new PlayerEventArgs(new TShockPlayer(who));
			Greet.Invoke(this, e);
			arg2.Handled = e.Handled;
		}

		void ServerHooks_Join(int arg1, HandledEventArgs arg2)
		{
			var e = new PlayerEventArgs(new TShockPlayer(arg1));
			Join.Invoke(this, e);
			arg2.Handled = e.Handled;
		}

        void ServerHooks_Leave(int who )
        {
            var e = new PlayerEventArgs(new TShockPlayer(who));
            Leave.Invoke(this, e);
        }

        void ServerHooks_Chat( messageBuffer msg, int who, string text, HandledEventArgs args )
        {
            if (!args.Handled)
            {
                var e = new PlayerChatEventArgs(new TShockPlayer(who), msg, text);
                Chat.Invoke(this, e);
                args.Handled = e.Handled;
            }
	    }

        /// <summary>
        /// Called when the player connects
        /// </summary>
        public HandlerList<PlayerEventArgs> Connect { get; set; }
        /// <summary>
		/// Called when the player first connects. They are not fully in the game yet, for that see Greet.
		/// </summary>
		public HandlerList<PlayerEventArgs> Join { get; set; }
		/// <summary>
		/// Called when the player is actually loaded into the game.
		/// </summary>
		public HandlerList<PlayerEventArgs> Greet { get; set; }
        /// <summary>
        /// Called when the player leaves the game.
        /// </summary>
        public HandlerList<PlayerEventArgs> Leave { get; set; }
        /// <summary>
        /// Called when the player chats.
        /// </summary>
        public HandlerList<PlayerChatEventArgs> Chat { get; set; }
	}


}
