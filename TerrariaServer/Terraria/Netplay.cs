using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TerrariaServer.Hooks;

namespace Terraria
{
    public class Netplay
    {
        public const int bufferSize = 1024;
        public const int maxConnections = 256;
        public static bool stopListen = false;
        public static ServerSock[] serverSock = new ServerSock[256];
        public static ClientSock clientSock = new ClientSock();
        public static TcpListener tcpListener;
        public static IPAddress serverListenIP = IPAddress.Any;
        public static IPAddress serverIP = IPAddress.Any;
        public static int serverPort = 7777;
        public static bool disconnect = false;
        public static string password = "";
        public static string banFile = "banlist.txt";
        public static bool spamCheck = false;
        public static bool anyClients = false;
        public static bool ServerUp = false;
        public static int connectionLimit = 0;
        public static bool killInactive = false;
        public static void ResetNetDiag()
        {
            Main.rxMsg = 0;
            Main.rxData = 0;
            Main.txMsg = 0;
            Main.txData = 0;
            for (int i = 0; i < Main.maxMsg; i++)
            {
                Main.rxMsgType[i] = 0;
                Main.rxDataType[i] = 0;
                Main.txMsgType[i] = 0;
                Main.txDataType[i] = 0;
            }
        }
        public static void ResetSections()
        {
            for (int i = 0; i < 256; i++)
            {
                for (int j = 0; j < Main.maxSectionsX; j++)
                {
                    for (int k = 0; k < Main.maxSectionsY; k++)
                    {
                        Netplay.serverSock[i].tileSection[j, k] = false;
                    }
                }
            }
        }
        public static void AddBan(int plr)
        {
            string text = Netplay.serverSock[plr].tcpClient.Client.RemoteEndPoint.ToString();
            string value = text;
            for (int i = 0; i < text.Length; i++)
            {
                if (text.Substring(i, 1) == ":")
                {
                    value = text.Substring(0, i);
                }
            }
            using (StreamWriter streamWriter = new StreamWriter(Netplay.banFile, true))
            {
                streamWriter.WriteLine("//" + Main.player[plr].name);
                streamWriter.WriteLine(value);
            }
        }
        public static bool CheckBan(string ip)
        {
            try
            {
                string b = ip;
                for (int i = 0; i < ip.Length; i++)
                {
                    if (ip.Substring(i, 1) == ":")
                    {
                        b = ip.Substring(0, i);
                    }
                }
                if (File.Exists(Netplay.banFile))
                {
                    using (StreamReader streamReader = new StreamReader(Netplay.banFile))
                    {
                        string a;
                        while ((a = streamReader.ReadLine()) != null)
                        {
                            if (a == b)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return false;
        }
        public static void newRecent()
        {
            for (int i = 0; i < Main.maxMP; i++)
            {
                if (Main.recentIP[i] == Netplay.serverIP.ToString() && Main.recentPort[i] == Netplay.serverPort)
                {
                    for (int j = i; j < Main.maxMP - 1; j++)
                    {
                        Main.recentIP[j] = Main.recentIP[j + 1];
                        Main.recentPort[j] = Main.recentPort[j + 1];
                        Main.recentWorld[j] = Main.recentWorld[j + 1];
                    }
                }
            }
            for (int k = Main.maxMP - 1; k > 0; k--)
            {
                Main.recentIP[k] = Main.recentIP[k - 1];
                Main.recentPort[k] = Main.recentPort[k - 1];
                Main.recentWorld[k] = Main.recentWorld[k - 1];
            }
            Main.recentIP[0] = Netplay.serverIP.ToString();
            Main.recentPort[0] = Netplay.serverPort;
            Main.recentWorld[0] = Main.worldName;
        }
        public static void ServerLoop(object threadContext)
        {
            Netplay.ResetNetDiag();
            if (Main.rand == null)
            {
                Main.rand = new Random((int)DateTime.Now.Ticks);
            }
            if (WorldGen.genRand == null)
            {
                WorldGen.genRand = new Random((int)DateTime.Now.Ticks);
            }
            Main.myPlayer = 255;
            Netplay.serverIP = IPAddress.Any;
            //Netplay.serverListenIP = Netplay.serverIP;
            Main.menuMode = 14;
            Main.statusText = "Starting server...";
            Main.netMode = 2;
            Netplay.disconnect = false;
            for (int i = 0; i < 256; i++)
            {
                Netplay.serverSock[i] = new ServerSock();
                Netplay.serverSock[i].Reset();
                Netplay.serverSock[i].whoAmI = i;
                Netplay.serverSock[i].tcpClient = new TcpClient();
                Netplay.serverSock[i].tcpClient.NoDelay = true;
                Netplay.serverSock[i].readBuffer = new byte[1024];
                Netplay.serverSock[i].writeBuffer = new byte[1024];
            }
            Netplay.tcpListener = new TcpListener(Netplay.serverListenIP, Netplay.serverPort);
            try
            {
                Netplay.tcpListener.Start();
            }
            catch (Exception ex)
            {
                Main.menuMode = 15;
                Main.statusText = ex.ToString();
                Netplay.disconnect = true;
            }
            if (!Netplay.disconnect)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(Netplay.ListenForClients), 1);
                Main.statusText = "Server started";
            }
            int num = 0;
            while (!Netplay.disconnect)
            {
                if (Netplay.stopListen)
                {
                    int num2 = -1;
                    for (int j = 0; j < Main.maxNetPlayers; j++)
                    {
                        if (serverSock[j].tcpClient == null || !Netplay.serverSock[j].tcpClient.Connected)
                        {
                            num2 = j;
                            break;
                        }
                    }
                    if (num2 >= 0)
                    {
                        if (Main.ignoreErrors)
                        {
                            try
                            {
                                Netplay.tcpListener.Start();
                                Netplay.stopListen = false;
                                ThreadPool.QueueUserWorkItem(new WaitCallback(Netplay.ListenForClients), 1);
                                goto IL_208;
                            }
                            catch
                            {
                                goto IL_208;
                            }
                        }
                        Netplay.tcpListener.Start();
                        Netplay.stopListen = false;
                        ThreadPool.QueueUserWorkItem(new WaitCallback(Netplay.ListenForClients), 1);
                    }
                }
            IL_208:
                int num3 = 0;
                for (int k = 0; k < 256; k++)
                {
                    if (NetMessage.buffer[k].checkBytes)
                    {
                        NetMessage.CheckBytes(k);
                    }
                    if (killInactive && serverSock[k].active && serverSock[k].state == 0 && (DateTime.UtcNow - serverSock[k].connectTime).TotalSeconds > 5)
                        Netplay.serverSock[k].kill = true;

                    if (Netplay.serverSock[k].kill)
                    {
                        ServerHooks.OnLeave(Netplay.serverSock[k].whoAmI);
                        Netplay.serverSock[k].Reset();
                        NetMessage.syncPlayers();
                    }
                    else if (serverSock[k].tcpClient != null && Netplay.serverSock[k].tcpClient.Connected)
                    {
                        if (!Netplay.serverSock[k].active)
                        {
                            Netplay.serverSock[k].state = 0;
                        }
                        Netplay.serverSock[k].active = true;
                        num3++;
                        if (!Netplay.serverSock[k].locked)
                        {
                            try
                            {
                                Netplay.serverSock[k].networkStream = Netplay.serverSock[k].tcpClient.GetStream();
                                if (Netplay.serverSock[k].networkStream.DataAvailable)
                                {
                                    Netplay.serverSock[k].locked = true;
                                    Netplay.serverSock[k].networkStream.BeginRead(Netplay.serverSock[k].readBuffer, 0, Netplay.serverSock[k].readBuffer.Length, new AsyncCallback(Netplay.serverSock[k].ServerReadCallBack), Netplay.serverSock[k].networkStream);
                                }
                            }
                            catch
                            {
                                Netplay.serverSock[k].kill = true;
                            }
                        }
                        if (Netplay.serverSock[k].statusMax > 0 && Netplay.serverSock[k].statusText2 != "")
                        {
                            if (Netplay.serverSock[k].statusCount >= Netplay.serverSock[k].statusMax)
                            {
                                Netplay.serverSock[k].statusText2 = "";
                                Netplay.serverSock[k].statusMax = 0;
                                Netplay.serverSock[k].statusCount = 0;
                            }
                            else
                            {
                            }
                        }
                        else
                        {
                        }
                    }
                    else if (Netplay.serverSock[k].active)
                    {
                        Netplay.serverSock[k].kill = true;
                    }
                    else
                    {
                        Netplay.serverSock[k].statusText2 = "";
                        if (k < 255)
                        {
                            Main.player[k].active = false;
                        }
                    }
                }
                num++;
                if (num > 10)
                {
                    Thread.Sleep(1);
                    num = 0;
                }
                else
                {
                    Thread.Sleep(0);
                }
                if (num3 == 0)
                {
                    Netplay.anyClients = false;
                }
                else
                {
                    Netplay.anyClients = true;
                }
                Netplay.ServerUp = true;
            }
            Netplay.tcpListener.Stop();
            for (int l = 0; l < 256; l++)
            {
                Netplay.serverSock[l].Reset();
            }
            if (Main.menuMode != 15)
            {
                Main.netMode = 0;
                Main.menuMode = 10;
                WorldGen.saveWorld(false);
                while (WorldGen.saveLock)
                {
                }
                Main.menuMode = 0;
            }
            else
            {
                Main.netMode = 0;
            }
            Main.myPlayer = 0;
        }
        public static void ListenForClients(object threadContext)
        {
            while (!Netplay.disconnect && !Netplay.stopListen)
            {
                int num = -1;
                for (int i = 0; i < Main.maxNetPlayers; i++)
                {
                    if (serverSock[i].tcpClient == null || !Netplay.serverSock[i].tcpClient.Connected)
                    {
                        num = i;
                        break;
                    }
                }
                if (num >= 0)
                {
                    try
                    {
                        Netplay.serverSock[num].tcpClient = Netplay.tcpListener.AcceptTcpClient();
                        Netplay.serverSock[num].tcpClient.NoDelay = true;
                        Netplay.serverSock[num].connectTime = DateTime.UtcNow;
                        Console.WriteLine(Netplay.serverSock[num].tcpClient.Client.RemoteEndPoint + " is connecting...");
                        if (connectionLimit > 0 &&
                            CheckExistingIP(Netplay.serverSock[num].tcpClient.Client.RemoteEndPoint.ToString().Split(':')[0]) > connectionLimit)
                            serverSock[num].kill = true;
                        continue;
                    }
                    catch (Exception ex)
                    {
                        if (!Netplay.disconnect)
                        {
                            Main.menuMode = 15;
                            Main.statusText = ex.ToString();
                            Netplay.disconnect = true;
                        }
                        continue;
                    }
                }
                Netplay.stopListen = true;
                Netplay.tcpListener.Stop();
            }
        }
        public static int CheckExistingIP(string IP)
        {
            int hit = 0;
            for (int i = 0; i < Main.maxNetPlayers; i++)
                if (serverSock[i] != null && serverSock[i].tcpClient.Connected &&
                    serverSock[i].tcpClient.Client.RemoteEndPoint.ToString().Split(':')[0] == IP)
                    hit++;
            return hit;
        }
        public static void StartServer()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(Netplay.ServerLoop), 1);
        }
        public static bool SetIP(string newIP)
        {
            try
            {
                Netplay.serverIP = IPAddress.Parse(newIP);
            }
            catch
            {
                return false;
            }
            return true;
        }
        public static bool SetIP2(string newIP)
        {
            bool result;
            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(newIP);
                IPAddress[] addressList = hostEntry.AddressList;
                for (int i = 0; i < addressList.Length; i++)
                {
                    if (addressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        Netplay.serverIP = addressList[i];
                        result = true;
                        return result;
                    }
                }
                result = false;
            }
            catch
            {
                result = false;
            }
            return result;
        }
        public static void Init()
        {
            for (int i = 0; i < 257; i++)
            {
                if (i < 256)
                {
                    Netplay.serverSock[i] = new ServerSock();
                    Netplay.serverSock[i].tcpClient.NoDelay = true;
                }
                NetMessage.buffer[i] = new messageBuffer();
                NetMessage.buffer[i].whoAmI = i;
            }
            Netplay.clientSock.tcpClient.NoDelay = true;
        }
        public static int GetSectionX(int x)
        {
            return x / 200;
        }
        public static int GetSectionY(int y)
        {
            return y / 150;
        }
    }
}