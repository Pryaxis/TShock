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
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Terraria;

namespace TShockAPI
{
    class RconHandler
    {
        public static string Password = "";
        private static DateTime lastRequest;
        public static int ListenPort;
        public static bool ContinueServer = true;
        public static string Response = "";

        public static void StartThread()
        {
            (new Thread(Start)).Start();
        }

        public static void Start()
        {
            Log.Info("Starting RconHandler.");
            try
            {
                Console.WriteLine(string.Format("RconHandler is running at UDP port {0} and password is {1}",
                    ListenPort.ToString(),
                    Password));
                Thread listen = new Thread(new ThreadStart(Listener));
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
            while (ContinueServer)
            {
                UdpClient listener = new UdpClient(ListenPort);
                try
                {
                    var listenEP = new IPEndPoint(IPAddress.Any, ListenPort);
                    lastRequest = DateTime.Now;
                    byte[] bytes = listener.Receive(ref listenEP);
                    Log.Info(string.Format("Recieved packet from {0}:{1}", listenEP.Address.ToString(), listenEP.Port.ToString()));
                    var packet = ParsePacket(bytes, listenEP);
                    listener.Send(packet, packet.Length, listenEP);
                    listener.Close();
                }
                catch (Exception e)
                {
                    Log.Error(e.ToString());
                    listener.Close();
                }
            }
        }

        private static string SendPacket(byte[] bytes, string hostname, int port)
        {
            var response = Encoding.UTF8.GetString(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }) + "disconnect";
            try
            {
                var EP = new IPEndPoint(IPAddress.Any, port);
                UdpClient client = new UdpClient();
                client.Connect(hostname, port);
                client.Client.ReceiveTimeout = 500;
                client.Send(bytes, bytes.Length);
                response = Encoding.UTF8.GetString(client.Receive(ref EP));
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
            if ((DateTime.Now - lastRequest).Milliseconds >= 100)
            {
                if (packetstring.StartsWith("rcon") || packetstring.Substring(4).StartsWith("rcon") || packetstring.Substring(5).StartsWith("rcon"))
                {
                    if (!string.IsNullOrEmpty(Password))
                    {
                        var args = packetstring.Split(' ');
                        if (args.Length >= 3)
                        {
                            if (args[1] == Password)
                            {
                                string command = "";
                                for (int i = 2; i < args.Length; i++)
                                    command += args[i] + " ";
                                command = command.TrimEnd(' ').TrimEnd('\0');
                                response = ExecuteCommand(command);
                                if (response == "" && Response != "")
                                {
                                    response = Response;
                                    Response = "";
                                }
                            }
                            else
                            {
                                response = "Invalid password.";
                                Log.ConsoleInfo("Bad rcon password from " + EP.ToString());
                            }
                        }
                        else
                            response = "";
                    }
                    else
                    {
                        response = "The server must set a password for clients to use rcon."; 
                        Log.Info("No password for rcon set");
                    }
                }
                /*else if (packetstring.StartsWith("getinfo")
                    || packetstring.Substring(4).StartsWith("getinfo")
                    || packetstring.Substring(5).StartsWith("getinfo"))
                {
                }
                else if (packetstring.StartsWith("getstatus")
                    || packetstring.Substring(4).StartsWith("getstatus")
                    || packetstring.Substring(5).StartsWith("getstatus"))
                {
                }*/
                else
                    redirect = true;
            }
            if (!redirect) 
                return (ConstructPacket(response, print));
            else
                return (ConstructPacket("", false));
        }

        private static string ExecuteCommand(string text)
        {
            if (Main.rand == null)
                Main.rand = new Random();
            if (WorldGen.genRand == null)
                WorldGen.genRand = new Random();
            if (text.StartsWith("exit"))
            {
                Tools.ForceKickAll("Server shutting down!");
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
                        TSPlayer.Server.SendMessage(string.Format("{0} ({1}) [{2}]", player.Name, player.IP, player.Group.Name));
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
                        Response += (string.Format("{0} 0 0 {1}({2})  {3} {4} 0 0", count, player.Name, player.Group.Name, Netplay.serverSock[player.Index].tcpClient.Client.RemoteEndPoint.ToString())) + "\n";
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
                if (!Commands.HandleCommand(TSPlayer.Server, text))
                    return "Invalid command.";
            else
                if (!Commands.HandleCommand(TSPlayer.Server, "/" + text))
                    return "Invalid command.";
            return "";
        }

        private static byte[] ConstructPacket(string response, bool print)
        {
            var oob = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
            MemoryStream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(oob);
            if (print)
                writer.Write(Encoding.UTF8.GetBytes(string.Format("print\n{0}", response)));
            else
                writer.Write(Encoding.UTF8.GetBytes("disconnect\n"));
            var packet = Encoding.UTF8.GetBytes(
                (Encoding.UTF8.GetString(stream.GetBuffer())
                .Substring(0, (int)stream.Length)));
            return packet;
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
    }
}
