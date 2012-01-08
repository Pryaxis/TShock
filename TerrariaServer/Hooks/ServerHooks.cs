using System;
using System.ComponentModel;
using System.Text;
using Terraria;
using TerrariaServer.Hooks.Classes;

namespace TerrariaServer.Hooks
{
	public static class ServerHooks
	{
		public delegate void CommandD(string cmd, HandledEventArgs e);
		public delegate void SocketResetD(ServerSock socket);
		public static event ServerHooks.CommandD Command;
        public static event Action<int, HandledEventArgs> Connect;
		public static event Action<int, HandledEventArgs> Join;
		public static event Action<int> Leave;
		public static event Action<messageBuffer, int, string, HandledEventArgs> Chat;
		public static event ServerHooks.SocketResetD SocketReset;
		static ServerHooks()
		{
			NetHooks.GetData += new NetHooks.GetDataD(ServerHooks.NetHooks_GetData);
		}
		private static void NetHooks_GetData(GetDataEventArgs e)
		{
			if (Main.netMode != 2)
			{
				return;
			}
            if (e.MsgID == PacketTypes.ConnectRequest)
            {
                e.Handled = ServerHooks.OnConnect(e.Msg.whoAmI);
                if (e.Handled)
                {
                    Netplay.serverSock[e.Msg.whoAmI].kill = true;
                    return;
                }
            }
			else if (e.MsgID == PacketTypes.ContinueConnecting2)
			{
				e.Handled = ServerHooks.OnJoin(e.Msg.whoAmI);
				if (e.Handled)
				{
					Netplay.serverSock[e.Msg.whoAmI].kill = true;
					return;
				}
			}
			else
			{
				if (e.MsgID == PacketTypes.ChatText)
				{
					string @string = Encoding.ASCII.GetString(e.Msg.readBuffer, e.Index + 4, e.Length - 5);
					e.Handled = ServerHooks.OnChat(e.Msg, e.Msg.whoAmI, @string);
				}
			}
		}
		public static bool OnCommand(string cmd)
		{
			if (ServerHooks.Command == null)
			{
				return false;
			}
			HandledEventArgs handledEventArgs = new HandledEventArgs();
			ServerHooks.Command(cmd, handledEventArgs);
			return handledEventArgs.Handled;
		}
        public static bool OnConnect(int whoami)
        {
            if (ServerHooks.Connect == null)
            {
                return false;
            }
            HandledEventArgs handledEventArgs = new HandledEventArgs();
            ServerHooks.Connect(whoami, handledEventArgs);
            return handledEventArgs.Handled;
        }
		public static bool OnJoin(int whoami)
		{
			if (ServerHooks.Join == null)
			{
				return false;
			}
			HandledEventArgs handledEventArgs = new HandledEventArgs();
			ServerHooks.Join(whoami, handledEventArgs);
			return handledEventArgs.Handled;
		}
		public static void OnLeave(int whoami)
		{
			if (ServerHooks.Leave != null)
			{
				ServerHooks.Leave(whoami);
			}
		}
		public static bool OnChat(messageBuffer msg, int whoami, string text)
		{
			if (ServerHooks.Chat == null)
			{
				return false;
			}
			HandledEventArgs handledEventArgs = new HandledEventArgs();
			ServerHooks.Chat(msg, whoami, text, handledEventArgs);
			return handledEventArgs.Handled;
		}
		public static void OnSocketReset(ServerSock socket)
		{
			if (ServerHooks.SocketReset != null)
			{
				ServerHooks.SocketReset(socket);
			}
		}
	}
}
