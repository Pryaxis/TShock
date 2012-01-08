using System.ComponentModel;
using System.Text;
using Terraria;
using TerrariaServer.Hooks.Classes;

namespace TerrariaServer.Hooks
{
	public static class ClientHooks
	{
		public delegate void ChatReceivedEventHandler(byte playerID, Color color, string message);
		public delegate void OnChatD(ref string msg, HandledEventArgs e);
		public static event ClientHooks.ChatReceivedEventHandler ChatReceived;
		public static event ClientHooks.OnChatD Chat;
		static ClientHooks()
		{
			NetHooks.GetData += new NetHooks.GetDataD(ClientHooks.NetHooks_GetData);
			NetHooks.SendData += new NetHooks.SendDataD(ClientHooks.NetHooks_SendData);
		}
		private static void NetHooks_GetData(GetDataEventArgs e)
		{
			if (Main.netMode != 2 && e.MsgID == PacketTypes.ChatText && e.Length > 3)
			{
				byte playerID = e.Msg.readBuffer[e.Index];
				Color color = new Color((int)e.Msg.readBuffer[e.Index + 1] << 16, (int)e.Msg.readBuffer[e.Index + 2] << 8, (int)e.Msg.readBuffer[e.Index + 3]);
				string @string = Encoding.ASCII.GetString(e.Msg.readBuffer, e.Index + 4, e.Length - 5);
				ClientHooks.OnChatReceived(playerID, color, @string);
			}
		}
		public static void OnChatReceived(byte playerID, Color color, string message)
		{
			if (ClientHooks.ChatReceived != null)
			{
				ClientHooks.ChatReceived(playerID, color, message);
			}
		}
		private static void NetHooks_SendData(SendDataEventArgs e)
		{
			if (Main.netMode != 2 && e.MsgID == PacketTypes.ChatText)
			{
				string text = e.text;
				e.Handled = ClientHooks.OnChat(ref text);
				e.text = text;
			}
		}
		public static bool OnChat(ref string msg)
		{
			if (ClientHooks.Chat != null)
			{
				HandledEventArgs handledEventArgs = new HandledEventArgs();
				ClientHooks.Chat(ref msg, handledEventArgs);
				return handledEventArgs.Handled;
			}
			return false;
		}
	}
}
