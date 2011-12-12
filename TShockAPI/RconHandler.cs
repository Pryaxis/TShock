/*   
TShock, a server mod for Terraria
Copyright (C) 2011 The TShock Team

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
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Terraria;
using System.IO.Streams;

namespace TShockAPI
{
    class RconHandler
    {
        public static string Password = "";
        private static DateTime LastRequest;
        private static DateTime LastHeartbeat;
        public static int ListenPort;
        public static bool ContinueServer = true;
        public static string Response = "";
        private static bool Started;
        private static UdpClient listener;
        private static Thread startThread;
        private static Thread heartbeat;
        private static Thread listen;

        public static void ShutdownAllThreads()
        {
            if (Started)
            {
                startThread.Abort();
                heartbeat.Abort();
                listen.Abort();
                Started = false;
            }
        }

        public static void StartThread()
        {
            if (!Started)
            {
                startThread = new Thread(Start);
                startThread.Start();

                heartbeat = new Thread(SendHeartbeat);
                heartbeat.Start();
            }
            Started = true;
        }

        public static void Start()
        {
            Log.Info("Starting RconHandler.");
            try
            {
                Console.WriteLine(string.Format("RconHandler is running at UDP port {0} and password is {1}",
                    ListenPort,
                    Password));
                listen = new Thread(Listener);
                listen.Start();
                while (true)
                {
                    if (listen.ThreadState != ThreadState.Running)
                    {
                        listen.Abort();
                        while (listen.ThreadState != ThreadState.Stopped)
                            continue;
                        listen.Start();
                    }
                    Thread.Sleep(3000);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        private static void Listener()
        {
            if (listener == null)
                try
                {
                    listener = new UdpClient(ListenPort);
                }
                catch (SocketException e)
                {
                    if (e.SocketErrorCode == SocketError.AddressAlreadyInUse)
                        Log.ConsoleError("Could not bind to " + ListenPort + ". Are you sure you don't have another instance running?");
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                }
            while (ContinueServer)
            {
                try
                {
                    var listenEP = new IPEndPoint(IPAddress.Any, ListenPort);
                    LastRequest = DateTime.Now;
                    byte[] bytes = listener.Receive(ref listenEP);
                    var packet = ParsePacket(bytes, listenEP);
                    listener.Send(packet, packet.Length, listenEP);
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                }
            }

        }

        private static string SendPacket(byte[] bytes, string hostname, int port)
        {
            var response = Encoding.UTF8.GetString(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }) + "disconnect";
            try
            {
                var EP = new IPEndPoint(IPAddress.Any, port);
                using (var client = new UdpClient())
                {
                    client.Connect(hostname, port);
                    client.Client.ReceiveTimeout = 500;
                    client.Send(bytes, bytes.Length);
                    response = Encoding.UTF8.GetString(client.Receive(ref EP));
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
            return response;
        }

        private static byte[] ParsePacket(byte[] bytes, IPEndPoint EP)
        {
            string response = "";
            var packetstring = Encoding.UTF8.GetString(PadPacket(bytes));
            var redirect = false;
            var print = true;
            if ((DateTime.Now - LastRequest).Milliseconds >= 100)
            {
                if (packetstring.StartsWith("rcon") || packetstring.Substring(4).StartsWith("rcon") || packetstring.Substring(5).StartsWith("rcon"))
                {
                    if (!string.IsNullOrEmpty(Password))
                    {
                        var args = ParseParameters(packetstring);
                        if (args.Count >= 3)
                        {
                            if (args[1] == Password)
                            {
                                args[1] = args[0] = "";
                                string command = string.Join(" ", args.ToArray());
                                command = command.TrimEnd(' ').TrimEnd('\0').TrimStart(' ');
                                Log.ConsoleInfo("Rcon from " + EP + ":" + command);
                                Response = "";
                                response = ExecuteCommand(command);
                                response += "\n" + Response;
                                Response = "";
                                response = response.TrimStart('\n');
                            }
                            else
                            {
                                response = "Bad rcon password.\n";
                                Log.ConsoleInfo("Bad rcon password from " + EP);
                            }
                        }
                        else
                            response = "";
                    }
                    else
                    {
                        response = "No rcon password set on the server.\n";
                        Log.Info("No password for rcon set");
                    }
                }
                else
                    redirect = true;
            }
            if (packetstring.StartsWith("getinfo")
                || packetstring.Substring(4).StartsWith("getinfo")
                || packetstring.Substring(5).StartsWith("getinfo"))
            {
                var challenge = "";
                if (packetstring.Split(' ').Length == 2)
                    challenge = packetstring.Split(' ')[1];
                response = "infoResponse\n";
                var infostring = string.Format(@"\_TShock_ver\{6}\mapname\{1}\sv_maxclients\{2}\clients\{3}\sv_privateClients\{4}\hconly\{5}\gamename\TERRARIA\protocol\100\sv_hostname\{0}\g_needPass\{7}",
                    TShock.Config.ServerName, Main.worldName, Main.maxNetPlayers,
                    TShock.Utils.ActivePlayers(), Main.maxNetPlayers - TShock.Config.MaxSlots,
                    TShock.Config.HardcoreOnly ? 1 : 0, TShock.VersionNum,
                    Netplay.password != "" ? 1 : 0);
                if (challenge != "")
                    infostring += @"\challenge\" + challenge;
                response += infostring;
                print = false;
                redirect = false;
            }
            else if (packetstring.StartsWith("getstatus")
                || packetstring.Substring(4).StartsWith("getstatus")
                || packetstring.Substring(5).StartsWith("getstatus"))
            {
                var challenge = "";
                if (packetstring.Split(' ').Length == 2)
                    challenge = packetstring.Split(' ')[1];
                response = "statusResponse\n";
                var statusstring = string.Format(@"\_TShock_ver\{6}\mapname\{1}\sv_maxclients\{2}\clients\{3}\sv_privateClients\{4}\hconly\{5}\gamename\TERRARIA\protocol\100\sv_hostname\{0}\g_needPass\{7}",
                    TShock.Config.ServerName, Main.worldName, Main.maxNetPlayers,
                    TShock.Utils.ActivePlayers(), Main.maxNetPlayers - TShock.Config.MaxSlots,
                    TShock.Config.HardcoreOnly ? 1 : 0, TShock.VersionNum,
                    Netplay.password != "" ? 1 : 0) + "\n";
                if (challenge != "")
                    statusstring += @"\challenge\" + challenge;
                foreach (TSPlayer player in TShock.Players)
                {
                    if (player != null && player.Active)
                        statusstring += (string.Format("0 0 {0}\n", player.Name));
                }
                response += statusstring;
                print = false;
                redirect = false;
            }
            if (!redirect)
                return (ConstructPacket(response, print));
            else
                return (ConstructPacket("disconnect", false));
        }

        private static string ExecuteCommand(string text)
        {
            if (Main.rand == null)
                Main.rand = new Random();
            if (WorldGen.genRand == null)
                WorldGen.genRand = new Random();
            if (text.StartsWith("exit"))
            {
                TShock.Utils.ForceKickAll("Server shutting down!");
                WorldGen.saveWorld(false);
                Netplay.disconnect = true;
                return "Server shutting down.";
            }
            else if (text.StartsWith("playing") || text.StartsWith("/playing"))
            {
                int count = 0;
                foreach (TSPlayer player in TShock.Players)
                {
                    if (player != null && player.Active)
                    {
                        count++;
                        TSPlayer.Server.SendMessage(string.Format("{0} ({1}) [{2}] <{3}>", player.Name, player.IP, player.Group.Name, player.UserAccountName));
                    }
                }
                TSPlayer.Server.SendMessage(string.Format("{0} players connected.", count));
            }
            else if (text.StartsWith("status"))
            {
                Response += "map: " + Main.worldName + "\n";
                Response += "num score ping name            lastmsg address            qport rate\n";
                int count = 0;
                foreach (TSPlayer player in TShock.Players)
                {
                    if (player != null && player.Active)
                    {
                        count++;
                        Response += (string.Format("{0} 0 0 {1}({2})  {3} {4} 0 0", count, player.Name, player.Group.Name, Netplay.serverSock[player.Index].tcpClient.Client.RemoteEndPoint, "")) + "\n";
                    }
                }
            }
            else if (text.StartsWith("say "))
            {
                Log.Info(string.Format("Server said: {0}", text.Remove(0, 4)));
                return string.Format("Server said: {0}", text.Remove(0, 4));
            }
            else if (text == "autosave")
            {
                Main.autoSave = TShock.Config.AutoSave = !TShock.Config.AutoSave;
                Log.ConsoleInfo("AutoSave " + (TShock.Config.AutoSave ? "Enabled" : "Disabled"));
                return "AutoSave " + (TShock.Config.AutoSave ? "Enabled" : "Disabled");
            }
            else if (text.StartsWith("/"))
            {
                if (!Commands.HandleCommand(TSPlayer.Server, text))
                    return "Invalid command.";
            }
            else
                if (!Commands.HandleCommand(TSPlayer.Server, "/" + text))
                    return "Invalid command.";
            return "";
        }

        private static byte[] ConstructPacket(string response, bool print)
        {
            var oob = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
            using (var stream = new MemoryStream())
            {
                stream.WriteBytes(oob);
                if (print)
                    stream.WriteBytes(Encoding.UTF8.GetBytes(string.Format("print\n{0}", response)));
                else
                    stream.WriteBytes(Encoding.UTF8.GetBytes(response));
                var trimmedpacket = new byte[(int)stream.Length];
                var packet = stream.GetBuffer();
                Array.Copy(packet, trimmedpacket, (int)stream.Length);
                return trimmedpacket;
            }
        }

        private static byte[] PadPacket(byte[] packet)
        {
            var returnpacket = new byte[(4 + packet.Length)];
            int h = 0;
            if (packet[0] != 0xFF)
            {
                for (int i = 0; i < 4; i++)
                    returnpacket[i] = 0xFF;
                for (int i = 4; i < returnpacket.Length; i++)
                    returnpacket[i] = packet[h++];
            }
            else
                returnpacket = packet;
            return returnpacket;
        }

        private static void SendHeartbeat()
        {
            LastHeartbeat = DateTime.UtcNow.Subtract(new TimeSpan(0, 0, 30));
            while (true)
            {
                if ((DateTime.UtcNow - LastHeartbeat).Seconds >= 30)
                {
                    var packet = ConstructPacket("heartbeat TerrariaShock", false);
                    if (listener == null)
                        try
                        {
                            listener = new UdpClient(ListenPort);
                        }
                        catch (SocketException e)
                        {
                            if (e.SocketErrorCode == SocketError.AddressAlreadyInUse)
                                Log.ConsoleError("Could not bind to " + ListenPort + ". Are you sure you don't have another instance running?");
                        }
                        catch (Exception e)
                        {
                            Log.Error(e.ToString());
                        }
                    listener.Send(packet, packet.Length, TShock.Config.MasterServer, 27950);
                    LastHeartbeat = DateTime.UtcNow;
                }
                Thread.Sleep(10000);
            }
        }

        #region ParseParams
        private static List<String> ParseParameters(string str)
        {
            var ret = new List<string>();
            var sb = new StringBuilder();
            bool instr = false;
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];

                if (instr)
                {
                    if (c == '\\')
                    {
                        if (i + 1 >= str.Length)
                            break;
                        c = GetEscape(str[++i]);
                    }
                    else if (c == '"')
                    {
                        ret.Add(sb.ToString());
                        sb.Clear();
                        instr = false;
                        continue;
                    }
                    sb.Append(c);
                }
                else
                {
                    if (IsWhiteSpace(c))
                    {
                        if (sb.Length > 0)
                        {
                            ret.Add(sb.ToString());
                            sb.Clear();
                        }
                    }
                    else if (c == '"')
                    {
                        if (sb.Length > 0)
                        {
                            ret.Add(sb.ToString());
                            sb.Clear();
                        }
                        instr = true;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            }
            if (sb.Length > 0)
                ret.Add(sb.ToString());

            return ret;
        }
        private static char GetEscape(char c)
        {
            switch (c)
            {
                case '\\':
                    return '\\';
                case '"':
                    return '"';
                case 't':
                    return '\t';
                default:
                    return c;
            }
        }
        private static bool IsWhiteSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n';
        }
        #endregion
    }
}
