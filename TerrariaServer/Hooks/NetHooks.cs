using System.ComponentModel;
using Terraria;
using TerrariaServer.Hooks.Classes;

namespace TerrariaServer.Hooks
{
	public static class NetHooks
	{
		public delegate void SendDataD(SendDataEventArgs e);
		public delegate void GetDataD(GetDataEventArgs e);
		public delegate void GreetPlayerD(int who, HandledEventArgs e);
		public delegate void SendBytesD(ServerSock socket, byte[] buffer, int offset, int count, HandledEventArgs e);
		public static event NetHooks.SendDataD SendData;
		public static event NetHooks.GetDataD GetData;
		public static event NetHooks.GreetPlayerD GreetPlayer;
		public static event NetHooks.SendBytesD SendBytes;
		public static bool OnSendData(ref int msgType, ref int remoteClient, ref int ignoreClient, ref string text, ref int number, ref float number2, ref float number3, ref float number4, ref int number5)
		{
			if (NetHooks.SendData == null)
			{
				return false;
			}
			SendDataEventArgs sendDataEventArgs = new SendDataEventArgs
			{
				MsgID = (PacketTypes)msgType, 
				remoteClient = remoteClient, 
				ignoreClient = ignoreClient, 
				text = text, 
				number = number, 
				number2 = number2, 
				number3 = number3, 
				number4 = number4,
                number5 = number5
			};
			NetHooks.SendData(sendDataEventArgs);
			msgType = (int)sendDataEventArgs.MsgID;
			remoteClient = sendDataEventArgs.remoteClient;
			ignoreClient = sendDataEventArgs.ignoreClient;
			text = sendDataEventArgs.text;
			number = sendDataEventArgs.number;
			number2 = sendDataEventArgs.number2;
			number3 = sendDataEventArgs.number3;
			number4 = sendDataEventArgs.number4;
			return sendDataEventArgs.Handled;
		}
		public static bool OnGetData(ref byte msgid, messageBuffer msg, ref int idx, ref int length)
		{
			if (NetHooks.GetData == null)
			{
				return false;
			}
			GetDataEventArgs getDataEventArgs = new GetDataEventArgs
			{
				MsgID = (PacketTypes)msgid, 
				Msg = msg, 
				Index = idx, 
				Length = length
			};
			NetHooks.GetData(getDataEventArgs);
			msgid = (byte)getDataEventArgs.MsgID;
			idx = getDataEventArgs.Index;
			length = getDataEventArgs.Length;
			return getDataEventArgs.Handled;
		}
		public static bool OnGreetPlayer(int who)
		{
			if (NetHooks.GreetPlayer == null)
			{
				return false;
			}
			HandledEventArgs handledEventArgs = new HandledEventArgs();
			NetHooks.GreetPlayer(who, handledEventArgs);
			return handledEventArgs.Handled;
		}
		public static bool OnSendBytes(ServerSock socket, byte[] buffer, int offset, int count)
		{
			if (NetHooks.SendBytes == null)
			{
				return false;
			}
			HandledEventArgs handledEventArgs = new HandledEventArgs();
			NetHooks.SendBytes(socket, buffer, offset, count, handledEventArgs);
			return handledEventArgs.Handled;
		}
	}
}
