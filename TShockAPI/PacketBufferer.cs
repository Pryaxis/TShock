/*
TShock, a server mod for Terraria
Copyright (C) 2011-2015 Nyx Studios (fka. The TShock Team)

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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Terraria;
using TerrariaApi.Server;

namespace TShockAPI
{
	public class PacketBufferer : IDisposable
	{
		/// <summary>
		/// Maximum number of bytes to send per update per socket
		/// </summary>
		public int BytesPerUpdate { get; set; }

		private readonly TShock plugin;

		private PacketBuffer[] buffers = new PacketBuffer[Netplay.serverSock.Length];

		private int[] Bytes = new int[52];
		private int[] Packets = new int[52];
		private int[] Compressed = new int[52];

#if DEBUG_NET
		Command dump;
		Command flush;
#endif

		public PacketBufferer(TShock p)
		{
			plugin = p;
			BytesPerUpdate = 0xFFFF;
			for (int i = 0; i < buffers.Length; i++)
				buffers[i] = new PacketBuffer();

#if DEBUG_NET
			dump = new Command("superadmin", Dump, "netdump");
			flush = new Command("superadmin", Flush, "netflush");
			Commands.ChatCommands.Add(dump);
			Commands.ChatCommands.Add(flush);
#endif

			ServerApi.Hooks.NetSendBytes.Register(plugin, ServerHooks_SendBytes);
			ServerApi.Hooks.ServerSocketReset.Register(plugin, ServerHooks_SocketReset);
			ServerApi.Hooks.GamePostUpdate.Register(plugin, GameHooks_Update);
		}

		~PacketBufferer()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
#if DEBUG_NET
				Commands.ChatCommands.Remove(dump);
				Commands.ChatCommands.Remove(flush);
#endif
				ServerApi.Hooks.NetSendBytes.Deregister(plugin, ServerHooks_SendBytes);
				ServerApi.Hooks.ServerSocketReset.Deregister(plugin, ServerHooks_SocketReset);
				ServerApi.Hooks.GamePostUpdate.Deregister(plugin, GameHooks_Update);
			}
		}

		private void Dump(CommandArgs args)
		{
			var sb = new StringBuilder();
			sb.AppendLine("{0,-25}{1,-25}{2,-25}{3}".SFormat("Name:", "Packets", "Bytes", "Compression"));
			for (int i = 1; i < Bytes.Length; i++)
			{
				sb.AppendLine("{0,-25}{1,-25}{2,-25}{3}".SFormat(Enum.GetName(typeof (PacketTypes), i) + ":", Packets[i], Bytes[i],
																 Compressed[i]));
			}
			File.WriteAllText(Path.Combine(TShock.SavePath, "dmp.txt"), sb.ToString());
		}

		private void Flush(CommandArgs args)
		{
			Bytes = new int[52];
			Packets = new int[52];
			Compressed = new int[52];
		}

		private void GameHooks_Update(EventArgs args)
		{
			FlushAll();
		}

		public void FlushAll()
		{
			for (int i = 0; i < Netplay.serverSock.Length; i++)
			{
				Flush(Netplay.serverSock[i]);
			}
		}

		public bool Flush(ServerSock socket)
		{
		    try
		    {
		        if (socket == null || !socket.active)
		            return false;

		        if (buffers[socket.whoAmI].Count < 1)
		            return false;

		        byte[] buff = buffers[socket.whoAmI].GetBytes(BytesPerUpdate);
		        if (buff == null)
		            return false;

		        if (SendBytes(socket, buff))
		        {
		            buffers[socket.whoAmI].Pop(buff.Length);
		            return true;
		        }
		    }
			catch (Exception e)
			{
				Log.ConsoleError(e.ToString());
			}
			return false;
		}


		private void ServerHooks_SocketReset(SocketResetEventArgs args)
		{
			buffers[args.Socket.whoAmI] = new PacketBuffer();
		}

		public bool SendBytes(ServerSock socket, byte[] buffer)
		{
			return SendBytes(socket, buffer, 0, buffer.Length);
		}

		public void BufferBytes(ServerSock socket, byte[] buffer)
		{
			BufferBytes(socket, buffer, 0, buffer.Length);
		}

		public void BufferBytes(ServerSock socket, byte[] buffer, int offset, int count)
		{
			lock (buffers[socket.whoAmI])
			{
#if DEBUG_NET
				int size = (count - offset);
				var pt = buffer[offset + 4];

				Packets[pt]++;
				Bytes[pt] += size;
				Compressed[pt] += Compress(buffer, offset, count);
#endif
				using (var ms = new MemoryStream(buffer, offset, count))
				{
					buffers[socket.whoAmI].AddRange(ms.ToArray());
				}

				if (TShock.Config.EnableMaxBytesInBuffer && buffers[socket.whoAmI].Count > TShock.Config.MaxBytesInBuffer)
				{
					buffers[socket.whoAmI].Clear();
					socket.kill = true;
				}
			}
		}

		public bool SendBytes(ServerSock socket, byte[] buffer, int offset, int count)
		{
			try
			{
				if (socket.tcpClient.Client != null && socket.tcpClient.Client.Poll(0, SelectMode.SelectWrite))
				{
					if (ServerApi.RunningMono && !ServerApi.UseAsyncSocketsInMono)
						socket.networkStream.Write(buffer, offset, count);
					else
						socket.networkStream.BeginWrite(buffer, offset, count, socket.ServerWriteCallBack, socket.networkStream);
					return true;
				}
			}
			catch (ObjectDisposedException e)
			{
				Log.Warn(e.ToString());
			}
			catch (SocketException e)
			{
				switch ((uint)e.ErrorCode)
				{
					case 0x80004005:
					case 10053:
						break;
					default:
						Log.Warn(e.ToString());
						break;
				}
			}
			catch (IOException e)
			{
				if (e.InnerException is SocketException)
				{
					switch (((SocketException)e.InnerException).SocketErrorCode)
					{
						case SocketError.Shutdown:
						case SocketError.ConnectionReset:
							break;
						default:
							Log.Warn(e.ToString());
							break;
					}
				}
				else
					Log.Warn(e.ToString());
			}
			return false;
		}

		private void ServerHooks_SendBytes(SendBytesEventArgs args)
		{
			args.Handled = true;
			BufferBytes(args.Socket, args.Buffer, args.Offset, args.Count);
		}

#if DEBUG_NET
		static int Compress(byte[] buffer, int offset, int count)
		{
			using (var ms = new MemoryStream())
			{
				using (var gzip = new GZipStream(ms, CompressionMode.Compress, true))
				{
					gzip.Write(buffer, offset, count);
				}
				return (int)ms.Length;
			}
		}
#endif
	}

	public class PacketBuffer : List<byte>
	{
		public byte[] GetBytes(int max)
		{
			lock (this)
			{
				if (this.Count < 1)
					return null;

				var ret = new byte[Math.Min(max, this.Count)];
				this.CopyTo(0, ret, 0, ret.Length);
				return ret;
			}
		}

		public void Pop(int count)
		{
			lock (this)
			{
				this.RemoveRange(0, count);
			}
		}
	}
}