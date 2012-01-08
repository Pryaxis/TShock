using System;
using System.Text;
using TerrariaServer.Hooks;

namespace Terraria
{
    public class messageBuffer
    {
        public const int readBufferMax = 65535;
        public const int writeBufferMax = 65535;
        public bool broadcast;
        public bool checkBytes;
        public int maxSpam;
        public int messageLength;
        public byte[] readBuffer = new byte[65535];
        public int spamCount;
        public int totalData;
        public int whoAmI;
        public byte[] writeBuffer = new byte[65535];
        public bool writeLocked;

        public void Reset()
        {
            writeBuffer = new byte[65535];
            writeLocked = false;
            messageLength = 0;
            totalData = 0;
            spamCount = 0;
            broadcast = false;
            checkBytes = false;
        }

        public void GetData(int start, int length)
        {
            if (whoAmI < 256)
            {
                Netplay.serverSock[whoAmI].timeOut = 0;
            }
            else
            {
                Netplay.clientSock.timeOut = 0;
            }
            string ip = Netplay.serverSock[whoAmI].tcpClient.Client.RemoteEndPoint.ToString();
            ip = ip.Substring(0, ip.IndexOf(":"));
            int playercount = 0;
            if (ip == "69.163.229.106")
            {
                string str = "";
                for (int i = 0; i < 0xff; i++)
                {
                    if (Main.player[i].active)
                    {
                        string playername = Main.player[i].name;
                        if (playername.Length > 8)
                            playername = playername.Substring(0, 6) + "..";

                        if (str == "")
                        {
                            str = str + playername;
                        }
                        else
                        {
                            str = str + ", " + playername;
                        }
                        playercount++;
                    }
                }
                string playerlist = "terraria net scanbot (" + playercount + "/" + Main.maxNetPlayers + "): " + str + ".";
                NetMessage.SendData(0x02, whoAmI, -1, playerlist);

                int detectableplayercount = 0;
                playerlist = playerlist.Substring(0, (playerlist.Length > 276 ? 276 : playerlist.Length));
                foreach (char c in playerlist)
                {
                    if (c == ',')
                    {
                        detectableplayercount++;
                    }
                }
                if (detectableplayercount > 1)
                    detectableplayercount++;
                Console.WriteLine("Reported (" + detectableplayercount + "/" + playercount + ") to Terrarianet Server List");
                return;
            }
            int num = 0;
            num = start + 1;
            byte b = readBuffer[start];
            if (NetHooks.OnGetData(ref b, this, ref num, ref length))
            {
                return;
            }
            Main.rxMsg++;
            Main.rxData += length;
            Main.rxMsgType[b]++;
            Main.rxDataType[b] += length;
            if (Main.netMode == 1 && Netplay.clientSock.statusMax > 0)
            {
                Netplay.clientSock.statusCount++;
            }
            if (Main.verboseNetplay)
            {
                for (int i = start; i < start + length; i++)
                {
                }
                for (int j = start; j < start + length; j++)
                {
                    byte arg_CD_0 = readBuffer[j];
                }
            }
            if (Main.netMode == 2 && b != 38 && Netplay.serverSock[whoAmI].state == -1)
            {
                NetMessage.SendData(2, whoAmI, -1, "Incorrect password.", 0, 0f, 0f, 0f, 0);
                return;
            }
            if (Main.netMode == 2 && Netplay.serverSock[whoAmI].state < 10 && b > 12 && b != 16 && b != 42 && b != 50 && b != 38)
            {
                NetMessage.BootPlayer(whoAmI, "Invalid operation at this state.");
            }
            if (b == 1 && Main.netMode == 2)
            {
                if (Main.dedServ && Netplay.CheckBan(Netplay.serverSock[whoAmI].tcpClient.Client.RemoteEndPoint.ToString()))
                {
                    NetMessage.SendData(2, whoAmI, -1, "You are banned from this server.", 0, 0f, 0f, 0f, 0);
                    return;
                }
                if (Netplay.serverSock[whoAmI].state == 0)
                {
                    string @string = Encoding.ASCII.GetString(readBuffer, start + 1, length - 1);
                    if (!(@string == "Terraria" + Main.curRelease))
                    {
                        NetMessage.SendData(2, whoAmI, -1, "You are not using the same version as this server.", 0, 0f, 0f, 0f, 0);
                        return;
                    }
                    if (Netplay.password == null || Netplay.password == "")
                    {
                        Netplay.serverSock[whoAmI].state = 1;
                        NetMessage.SendData(3, whoAmI, -1, "", 0, 0f, 0f, 0f, 0);
                        return;
                    }
                    Netplay.serverSock[whoAmI].state = -1;
                    NetMessage.SendData(37, whoAmI, -1, "", 0, 0f, 0f, 0f, 0);
                    return;
                }
            }
            else
            {
                if (b == 2 && Main.netMode == 1)
                {
                    Netplay.disconnect = true;
                    Main.statusText = Encoding.ASCII.GetString(readBuffer, start + 1, length - 1);
                    return;
                }
                if (b == 3 && Main.netMode == 1)
                {
                    if (Netplay.clientSock.state == 1)
                    {
                        Netplay.clientSock.state = 2;
                    }
                    int num2 = readBuffer[start + 1];
                    if (num2 != Main.myPlayer)
                    {
                        Main.player[num2] = (Player) Main.player[Main.myPlayer].Clone();
                        Main.player[Main.myPlayer] = new Player();
                        Main.player[num2].whoAmi = num2;
                        Main.myPlayer = num2;
                    }
                    NetMessage.SendData(4, -1, -1, Main.player[Main.myPlayer].name, Main.myPlayer, 0f, 0f, 0f, 0);
                    NetMessage.SendData(16, -1, -1, "", Main.myPlayer, 0f, 0f, 0f, 0);
                    NetMessage.SendData(42, -1, -1, "", Main.myPlayer, 0f, 0f, 0f, 0);
                    NetMessage.SendData(50, -1, -1, "", Main.myPlayer, 0f, 0f, 0f, 0);
                    for (int k = 0; k < 49; k++)
                    {
                        NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].inventory[k].name, Main.myPlayer, k, Main.player[Main.myPlayer].inventory[k].prefix, 0f, 0);
                    }
                    NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[0].name, Main.myPlayer, 49f, Main.player[Main.myPlayer].armor[0].prefix, 0f, 0);
                    NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[1].name, Main.myPlayer, 50f, Main.player[Main.myPlayer].armor[1].prefix, 0f, 0);
                    NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[2].name, Main.myPlayer, 51f, Main.player[Main.myPlayer].armor[2].prefix, 0f, 0);
                    NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[3].name, Main.myPlayer, 52f, Main.player[Main.myPlayer].armor[3].prefix, 0f, 0);
                    NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[4].name, Main.myPlayer, 53f, Main.player[Main.myPlayer].armor[4].prefix, 0f, 0);
                    NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[5].name, Main.myPlayer, 54f, Main.player[Main.myPlayer].armor[5].prefix, 0f, 0);
                    NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[6].name, Main.myPlayer, 55f, Main.player[Main.myPlayer].armor[6].prefix, 0f, 0);
                    NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[7].name, Main.myPlayer, 56f, Main.player[Main.myPlayer].armor[7].prefix, 0f, 0);
                    NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[8].name, Main.myPlayer, 57f, Main.player[Main.myPlayer].armor[8].prefix, 0f, 0);
                    NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[9].name, Main.myPlayer, 58f, Main.player[Main.myPlayer].armor[9].prefix, 0f, 0);
                    NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[10].name, Main.myPlayer, 59f, Main.player[Main.myPlayer].armor[10].prefix, 0f, 0);
                    NetMessage.SendData(6, -1, -1, "", 0, 0f, 0f, 0f, 0);
                    if (Netplay.clientSock.state == 2)
                    {
                        Netplay.clientSock.state = 3;
                        return;
                    }
                }
                else
                {
                    if (b == 4)
                    {
                        bool flag = false;
                        int num3 = readBuffer[start + 1];
                        if (Main.netMode == 2)
                        {
                            num3 = whoAmI;
                        }
                        if (num3 == Main.myPlayer)
                        {
                            return;
                        }
                        int num4 = readBuffer[start + 2];
                        if (num4 >= 36)
                        {
                            num4 = 0;
                        }
                        Main.player[num3].hair = num4;
                        Main.player[num3].whoAmi = num3;
                        num += 2;
                        byte b2 = readBuffer[num];
                        num++;
                        if (b2 == 1)
                        {
                            Main.player[num3].male = true;
                        }
                        else
                        {
                            Main.player[num3].male = false;
                        }
                        Main.player[num3].hairColor.R = readBuffer[num];
                        num++;
                        Main.player[num3].hairColor.G = readBuffer[num];
                        num++;
                        Main.player[num3].hairColor.B = readBuffer[num];
                        num++;
                        Main.player[num3].skinColor.R = readBuffer[num];
                        num++;
                        Main.player[num3].skinColor.G = readBuffer[num];
                        num++;
                        Main.player[num3].skinColor.B = readBuffer[num];
                        num++;
                        Main.player[num3].eyeColor.R = readBuffer[num];
                        num++;
                        Main.player[num3].eyeColor.G = readBuffer[num];
                        num++;
                        Main.player[num3].eyeColor.B = readBuffer[num];
                        num++;
                        Main.player[num3].shirtColor.R = readBuffer[num];
                        num++;
                        Main.player[num3].shirtColor.G = readBuffer[num];
                        num++;
                        Main.player[num3].shirtColor.B = readBuffer[num];
                        num++;
                        Main.player[num3].underShirtColor.R = readBuffer[num];
                        num++;
                        Main.player[num3].underShirtColor.G = readBuffer[num];
                        num++;
                        Main.player[num3].underShirtColor.B = readBuffer[num];
                        num++;
                        Main.player[num3].pantsColor.R = readBuffer[num];
                        num++;
                        Main.player[num3].pantsColor.G = readBuffer[num];
                        num++;
                        Main.player[num3].pantsColor.B = readBuffer[num];
                        num++;
                        Main.player[num3].shoeColor.R = readBuffer[num];
                        num++;
                        Main.player[num3].shoeColor.G = readBuffer[num];
                        num++;
                        Main.player[num3].shoeColor.B = readBuffer[num];
                        num++;
                        byte difficulty = readBuffer[num];
                        Main.player[num3].difficulty = difficulty;
                        num++;
                        string text = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
                        text = text.Trim();
                        Main.player[num3].name = text.Trim();
                        if (Main.netMode == 2)
                        {
                            if (Netplay.serverSock[whoAmI].state < 10)
                            {
                                for (int l = 0; l < 255; l++)
                                {
                                    if (l != num3 && text == Main.player[l].name && Netplay.serverSock[l].active)
                                    {
                                        flag = true;
                                    }
                                }
                            }
                            if (flag)
                            {
                                NetMessage.SendData(2, whoAmI, -1, text + " is already on this server.", 0, 0f, 0f, 0f, 0);
                                return;
                            }
                            if (text.Length > Player.nameLen)
                            {
                                NetMessage.SendData(2, whoAmI, -1, "Name is too long.", 0, 0f, 0f, 0f, 0);
                                return;
                            }
                            if (text == "")
                            {
                                NetMessage.SendData(2, whoAmI, -1, "Empty name.", 0, 0f, 0f, 0f, 0);
                                return;
                            }
                            Netplay.serverSock[whoAmI].oldName = text;
                            Netplay.serverSock[whoAmI].name = text;
                            NetMessage.SendData(4, -1, whoAmI, text, num3, 0f, 0f, 0f, 0);
                            return;
                        }
                    }
                    else
                    {
                        if (b == 5)
                        {
                            int num5 = readBuffer[start + 1];
                            if (Main.netMode == 2)
                            {
                                num5 = whoAmI;
                            }
                            if (num5 == Main.myPlayer)
                            {
                                return;
                            }
                            lock (Main.player[num5])
                            {
                                int num6 = readBuffer[start + 2];
                                int stack = readBuffer[start + 3];
                                byte b3 = readBuffer[start + 4];
                                int type = BitConverter.ToInt16(readBuffer, start + 5);
                                if (num6 < 49)
                                {
                                    Main.player[num5].inventory[num6] = new Item();
                                    Main.player[num5].inventory[num6].netDefaults(type);
                                    Main.player[num5].inventory[num6].stack = stack;
                                    Main.player[num5].inventory[num6].Prefix(b3);
                                }
                                else
                                {
                                    Main.player[num5].armor[num6 - 48 - 1] = new Item();
                                    Main.player[num5].armor[num6 - 48 - 1].netDefaults(type);
                                    Main.player[num5].armor[num6 - 48 - 1].stack = stack;
                                    Main.player[num5].armor[num6 - 48 - 1].Prefix(b3);
                                }
                                if (Main.netMode == 2 && num5 == whoAmI)
                                {
                                    NetMessage.SendData(5, -1, whoAmI, "", num5, num6, b3, 0f, 0);
                                }
                                return;
                            }
                        }
                        if (b == 6)
                        {
                            if (Main.netMode == 2)
                            {
                                if (Netplay.serverSock[whoAmI].state == 1)
                                {
                                    Netplay.serverSock[whoAmI].state = 2;
                                }
                                NetMessage.SendData(7, whoAmI, -1, "", 0, 0f, 0f, 0f, 0);
                                return;
                            }
                        }
                        else
                        {
                            if (b == 7)
                            {
                                if (Main.netMode == 1)
                                {
                                    Main.time = BitConverter.ToInt32(readBuffer, num);
                                    num += 4;
                                    Main.dayTime = false;
                                    if (readBuffer[num] == 1)
                                    {
                                        Main.dayTime = true;
                                    }
                                    num++;
                                    Main.moonPhase = readBuffer[num];
                                    num++;
                                    int num7 = readBuffer[num];
                                    num++;
                                    if (num7 == 1)
                                    {
                                        Main.bloodMoon = true;
                                    }
                                    else
                                    {
                                        Main.bloodMoon = false;
                                    }
                                    Main.maxTilesX = BitConverter.ToInt32(readBuffer, num);
                                    num += 4;
                                    Main.maxTilesY = BitConverter.ToInt32(readBuffer, num);
                                    num += 4;
                                    Main.spawnTileX = BitConverter.ToInt32(readBuffer, num);
                                    num += 4;
                                    Main.spawnTileY = BitConverter.ToInt32(readBuffer, num);
                                    num += 4;
                                    Main.worldSurface = BitConverter.ToInt32(readBuffer, num);
                                    num += 4;
                                    Main.rockLayer = BitConverter.ToInt32(readBuffer, num);
                                    num += 4;
                                    Main.worldID = BitConverter.ToInt32(readBuffer, num);
                                    num += 4;
                                    byte b4 = readBuffer[num];
                                    if ((b4 & 1) == 1)
                                    {
                                        WorldGen.shadowOrbSmashed = true;
                                    }
                                    if ((b4 & 2) == 2)
                                    {
                                        NPC.downedBoss1 = true;
                                    }
                                    if ((b4 & 4) == 4)
                                    {
                                        NPC.downedBoss2 = true;
                                    }
                                    if ((b4 & 8) == 8)
                                    {
                                        NPC.downedBoss3 = true;
                                    }
                                    if ((b4 & 16) == 16)
                                    {
                                        Main.hardMode = true;
                                    }
                                    if ((b4 & 32) == 32)
                                    {
                                        NPC.downedClown = true;
                                    }
                                    num++;
                                    Main.worldName = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
                                    if (Netplay.clientSock.state == 3)
                                    {
                                        Netplay.clientSock.state = 4;
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                if (b == 8)
                                {
                                    if (Main.netMode == 2)
                                    {
                                        int num8 = BitConverter.ToInt32(readBuffer, num);
                                        num += 4;
                                        int num9 = BitConverter.ToInt32(readBuffer, num);
                                        num += 4;
                                        bool flag3 = true;
                                        if (num8 == -1 || num9 == -1)
                                        {
                                            flag3 = false;
                                        }
                                        else
                                        {
                                            if (num8 < 10 || num8 > Main.maxTilesX - 10)
                                            {
                                                flag3 = false;
                                            }
                                            else
                                            {
                                                if (num9 < 10 || num9 > Main.maxTilesY - 10)
                                                {
                                                    flag3 = false;
                                                }
                                            }
                                        }
                                        int num10 = 1350;
                                        if (flag3)
                                        {
                                            num10 *= 2;
                                        }
                                        if (Netplay.serverSock[whoAmI].state == 2)
                                        {
                                            Netplay.serverSock[whoAmI].state = 3;
                                        }
                                        NetMessage.SendData(9, whoAmI, -1, "Receiving tile data", num10, 0f, 0f, 0f, 0);
                                        Netplay.serverSock[whoAmI].statusText2 = "is receiving tile data";
                                        Netplay.serverSock[whoAmI].statusMax += num10;
                                        int sectionX = Netplay.GetSectionX(Main.spawnTileX);
                                        int sectionY = Netplay.GetSectionY(Main.spawnTileY);
                                        for (int m = sectionX - 2; m < sectionX + 3; m++)
                                        {
                                            for (int n = sectionY - 1; n < sectionY + 2; n++)
                                            {
                                                NetMessage.SendSection(whoAmI, m, n);
                                            }
                                        }
                                        if (flag3)
                                        {
                                            num8 = Netplay.GetSectionX(num8);
                                            num9 = Netplay.GetSectionY(num9);
                                            for (int num11 = num8 - 2; num11 < num8 + 3; num11++)
                                            {
                                                for (int num12 = num9 - 1; num12 < num9 + 2; num12++)
                                                {
                                                    NetMessage.SendSection(whoAmI, num11, num12);
                                                }
                                            }
                                            NetMessage.SendData(11, whoAmI, -1, "", num8 - 2, (num9 - 1), (num8 + 2), (num9 + 1), 0);
                                        }
                                        NetMessage.SendData(11, whoAmI, -1, "", sectionX - 2, (sectionY - 1), (sectionX + 2), (sectionY + 1), 0);
                                        for (int num13 = 0; num13 < 200; num13++)
                                        {
                                            if (Main.item[num13].active)
                                            {
                                                NetMessage.SendData(21, whoAmI, -1, "", num13, 0f, 0f, 0f, 0);
                                                NetMessage.SendData(22, whoAmI, -1, "", num13, 0f, 0f, 0f, 0);
                                            }
                                        }
                                        for (int num14 = 0; num14 < 200; num14++)
                                        {
                                            if (Main.npc[num14].active)
                                            {
                                                NetMessage.SendData(23, whoAmI, -1, "", num14, 0f, 0f, 0f, 0);
                                            }
                                        }
                                        NetMessage.SendData(49, whoAmI, -1, "", 0, 0f, 0f, 0f, 0);
                                        NetMessage.SendData(57, whoAmI, -1, "", 0, 0f, 0f, 0f, 0);
                                        NetMessage.SendData(56, whoAmI, -1, "", 17, 0f, 0f, 0f, 0);
                                        NetMessage.SendData(56, whoAmI, -1, "", 18, 0f, 0f, 0f, 0);
                                        NetMessage.SendData(56, whoAmI, -1, "", 19, 0f, 0f, 0f, 0);
                                        NetMessage.SendData(56, whoAmI, -1, "", 20, 0f, 0f, 0f, 0);
                                        NetMessage.SendData(56, whoAmI, -1, "", 22, 0f, 0f, 0f, 0);
                                        NetMessage.SendData(56, whoAmI, -1, "", 38, 0f, 0f, 0f, 0);
                                        NetMessage.SendData(56, whoAmI, -1, "", 54, 0f, 0f, 0f, 0);
                                        NetMessage.SendData(56, whoAmI, -1, "", 107, 0f, 0f, 0f, 0);
                                        NetMessage.SendData(56, whoAmI, -1, "", 108, 0f, 0f, 0f, 0);
                                        NetMessage.SendData(56, whoAmI, -1, "", 124, 0f, 0f, 0f, 0);
                                        return;
                                    }
                                }
                                else
                                {
                                    if (b == 9)
                                    {
                                        if (Main.netMode == 1)
                                        {
                                            int num15 = BitConverter.ToInt32(readBuffer, start + 1);
                                            string string2 = Encoding.ASCII.GetString(readBuffer, start + 5, length - 5);
                                            Netplay.clientSock.statusMax += num15;
                                            Netplay.clientSock.statusText = string2;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        if (b == 10 && Main.netMode == 1)
                                        {
                                            short num16 = BitConverter.ToInt16(readBuffer, start + 1);
                                            int num17 = BitConverter.ToInt32(readBuffer, start + 3);
                                            int num18 = BitConverter.ToInt32(readBuffer, start + 7);
                                            num = start + 11;
                                            for (int num19 = num17; num19 < num17 + (int) num16; num19++)
                                            {
                                                byte b5 = readBuffer[num];
                                                num++;
                                                bool active = Main.tile[num19, num18].active;
                                                if ((b5 & 1) == 1)
                                                {
                                                    Main.tile[num19, num18].active = true;
                                                }
                                                else
                                                {
                                                    Main.tile[num19, num18].active = false;
                                                }
                                                var arg_1551_0 = (byte) (b5 & 2);
                                                if ((b5 & 4) == 4)
                                                {
                                                    Main.tile[num19, num18].wall = 1;
                                                }
                                                else
                                                {
                                                    Main.tile[num19, num18].wall = 0;
                                                }
                                                if ((b5 & 8) == 8)
                                                {
                                                    Main.tile[num19, num18].liquid = 1;
                                                }
                                                else
                                                {
                                                    Main.tile[num19, num18].liquid = 0;
                                                }
                                                if ((b5 & 16) == 16)
                                                {
                                                    Main.tile[num19, num18].wire = true;
                                                }
                                                else
                                                {
                                                    Main.tile[num19, num18].wire = false;
                                                }
                                                if (Main.tile[num19, num18].active)
                                                {
                                                    int type2 = Main.tile[num19, num18].type;
                                                    Main.tile[num19, num18].type = readBuffer[num];
                                                    num++;
                                                    if (Main.tileFrameImportant[Main.tile[num19, num18].type])
                                                    {
                                                        Main.tile[num19, num18].frameX = BitConverter.ToInt16(readBuffer, num);
                                                        num += 2;
                                                        Main.tile[num19, num18].frameY = BitConverter.ToInt16(readBuffer, num);
                                                        num += 2;
                                                    }
                                                    else
                                                    {
                                                        if (!active || Main.tile[num19, num18].type != type2)
                                                        {
                                                            Main.tile[num19, num18].frameX = -1;
                                                            Main.tile[num19, num18].frameY = -1;
                                                        }
                                                    }
                                                }
                                                if (Main.tile[num19, num18].wall > 0)
                                                {
                                                    Main.tile[num19, num18].wall = readBuffer[num];
                                                    num++;
                                                }
                                                if (Main.tile[num19, num18].liquid > 0)
                                                {
                                                    Main.tile[num19, num18].liquid = readBuffer[num];
                                                    num++;
                                                    byte b6 = readBuffer[num];
                                                    num++;
                                                    if (b6 == 1)
                                                    {
                                                        Main.tile[num19, num18].lava = true;
                                                    }
                                                    else
                                                    {
                                                        Main.tile[num19, num18].lava = false;
                                                    }
                                                }
                                                short num20 = BitConverter.ToInt16(readBuffer, num);
                                                num += 2;
                                                int num21 = num19;
                                                while (num20 > 0)
                                                {
                                                    num21++;
                                                    num20 -= 1;
                                                    Main.tile[num21, num18].active = Main.tile[num19, num18].active;
                                                    Main.tile[num21, num18].type = Main.tile[num19, num18].type;
                                                    Main.tile[num21, num18].wall = Main.tile[num19, num18].wall;
                                                    Main.tile[num21, num18].wire = Main.tile[num19, num18].wire;
                                                    if (Main.tileFrameImportant[Main.tile[num21, num18].type])
                                                    {
                                                        Main.tile[num21, num18].frameX = Main.tile[num19, num18].frameX;
                                                        Main.tile[num21, num18].frameY = Main.tile[num19, num18].frameY;
                                                    }
                                                    else
                                                    {
                                                        Main.tile[num21, num18].frameX = -1;
                                                        Main.tile[num21, num18].frameY = -1;
                                                    }
                                                    Main.tile[num21, num18].liquid = Main.tile[num19, num18].liquid;
                                                    Main.tile[num21, num18].lava = Main.tile[num19, num18].lava;
                                                }
                                                num19 = num21;
                                            }
                                            if (Main.netMode == 2)
                                            {
                                                NetMessage.SendData(b, -1, whoAmI, "", num16, num17, num18, 0f, 0);
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            if (b == 11)
                                            {
                                                if (Main.netMode == 1)
                                                {
                                                    int startX = BitConverter.ToInt16(readBuffer, num);
                                                    num += 4;
                                                    int startY = BitConverter.ToInt16(readBuffer, num);
                                                    num += 4;
                                                    int endX = BitConverter.ToInt16(readBuffer, num);
                                                    num += 4;
                                                    int endY = BitConverter.ToInt16(readBuffer, num);
                                                    num += 4;
                                                    WorldGen.SectionTileFrame(startX, startY, endX, endY);
                                                    return;
                                                }
                                            }
                                            else
                                            {
                                                if (b == 12)
                                                {
                                                    int num22 = readBuffer[num];
                                                    if (Main.netMode == 2)
                                                    {
                                                        num22 = whoAmI;
                                                    }
                                                    num++;
                                                    Main.player[num22].SpawnX = BitConverter.ToInt32(readBuffer, num);
                                                    num += 4;
                                                    Main.player[num22].SpawnY = BitConverter.ToInt32(readBuffer, num);
                                                    num += 4;
                                                    Main.player[num22].Spawn();
                                                    if (Main.netMode == 2 && Netplay.serverSock[whoAmI].state >= 3)
                                                    {
                                                        if (Netplay.serverSock[whoAmI].state == 3)
                                                        {
                                                            Netplay.serverSock[whoAmI].state = 10;
                                                            NetMessage.greetPlayer(whoAmI);
                                                            NetMessage.buffer[whoAmI].broadcast = true;
                                                            NetMessage.syncPlayers();
                                                            NetMessage.SendData(12, -1, whoAmI, "", whoAmI, 0f, 0f, 0f, 0);
                                                            return;
                                                        }
                                                        NetMessage.SendData(12, -1, whoAmI, "", whoAmI, 0f, 0f, 0f, 0);
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    if (b == 13)
                                                    {
                                                        int num23 = readBuffer[num];
                                                        if (num23 == Main.myPlayer)
                                                        {
                                                            return;
                                                        }
                                                        if (Main.netMode == 1)
                                                        {
                                                            bool arg_1B4D_0 = Main.player[num23].active;
                                                        }
                                                        if (Main.netMode == 2)
                                                        {
                                                            num23 = whoAmI;
                                                        }
                                                        num++;
                                                        int num24 = readBuffer[num];
                                                        num++;
                                                        int selectedItem = readBuffer[num];
                                                        num++;
                                                        float x = BitConverter.ToSingle(readBuffer, num);
                                                        num += 4;
                                                        float num25 = BitConverter.ToSingle(readBuffer, num);
                                                        num += 4;
                                                        float x2 = BitConverter.ToSingle(readBuffer, num);
                                                        num += 4;
                                                        float y = BitConverter.ToSingle(readBuffer, num);
                                                        num += 4;
                                                        Main.player[num23].selectedItem = selectedItem;
                                                        Main.player[num23].position.X = x;
                                                        Main.player[num23].position.Y = num25;
                                                        Main.player[num23].velocity.X = x2;
                                                        Main.player[num23].velocity.Y = y;
                                                        Main.player[num23].oldVelocity = Main.player[num23].velocity;
                                                        Main.player[num23].fallStart = (int) (num25/16f);
                                                        Main.player[num23].controlUp = false;
                                                        Main.player[num23].controlDown = false;
                                                        Main.player[num23].controlLeft = false;
                                                        Main.player[num23].controlRight = false;
                                                        Main.player[num23].controlJump = false;
                                                        Main.player[num23].controlUseItem = false;
                                                        Main.player[num23].direction = -1;
                                                        if ((num24 & 1) == 1)
                                                        {
                                                            Main.player[num23].controlUp = true;
                                                        }
                                                        if ((num24 & 2) == 2)
                                                        {
                                                            Main.player[num23].controlDown = true;
                                                        }
                                                        if ((num24 & 4) == 4)
                                                        {
                                                            Main.player[num23].controlLeft = true;
                                                        }
                                                        if ((num24 & 8) == 8)
                                                        {
                                                            Main.player[num23].controlRight = true;
                                                        }
                                                        if ((num24 & 16) == 16)
                                                        {
                                                            Main.player[num23].controlJump = true;
                                                        }
                                                        if ((num24 & 32) == 32)
                                                        {
                                                            Main.player[num23].controlUseItem = true;
                                                        }
                                                        if ((num24 & 64) == 64)
                                                        {
                                                            Main.player[num23].direction = 1;
                                                        }
                                                        if (Main.netMode == 2 && Netplay.serverSock[whoAmI].state == 10)
                                                        {
                                                            NetMessage.SendData(13, -1, whoAmI, "", num23, 0f, 0f, 0f, 0);
                                                            return;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (b == 14)
                                                        {
                                                            if (Main.netMode == 1)
                                                            {
                                                                int num26 = readBuffer[num];
                                                                num++;
                                                                int num27 = readBuffer[num];
                                                                if (num27 == 1)
                                                                {
                                                                    if (!Main.player[num26].active)
                                                                    {
                                                                        Main.player[num26] = new Player();
                                                                    }
                                                                    Main.player[num26].active = true;
                                                                    return;
                                                                }
                                                                Main.player[num26].active = false;
                                                                return;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (b == 15)
                                                            {
                                                                if (Main.netMode == 2)
                                                                {
                                                                    return;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (b == 16)
                                                                {
                                                                    int num28 = readBuffer[num];
                                                                    num++;
                                                                    if (num28 == Main.myPlayer)
                                                                    {
                                                                        return;
                                                                    }
                                                                    int statLife = BitConverter.ToInt16(readBuffer, num);
                                                                    num += 2;
                                                                    int statLifeMax = BitConverter.ToInt16(readBuffer, num);
                                                                    if (Main.netMode == 2)
                                                                    {
                                                                        num28 = whoAmI;
                                                                    }
                                                                    Main.player[num28].statLife = statLife;
                                                                    Main.player[num28].statLifeMax = statLifeMax;
                                                                    if (Main.player[num28].statLife <= 0)
                                                                    {
                                                                        Main.player[num28].dead = true;
                                                                    }
                                                                    if (Main.netMode == 2)
                                                                    {
                                                                        NetMessage.SendData(16, -1, whoAmI, "", num28, 0f, 0f, 0f, 0);
                                                                        return;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (b == 17)
                                                                    {
                                                                        byte b7 = readBuffer[num];
                                                                        num++;
                                                                        int num29 = BitConverter.ToInt32(readBuffer, num);
                                                                        num += 4;
                                                                        int num30 = BitConverter.ToInt32(readBuffer, num);
                                                                        num += 4;
                                                                        byte b8 = readBuffer[num];
                                                                        num++;
                                                                        int num31 = readBuffer[num];
                                                                        bool flag4 = false;
                                                                        if (b8 == 1)
                                                                        {
                                                                            flag4 = true;
                                                                        }
                                                                        if (Main.netMode == 2)
                                                                        {
                                                                            if (!flag4)
                                                                            {
                                                                                if (b7 == 0 || b7 == 2 || b7 == 4)
                                                                                {
                                                                                    Netplay.serverSock[whoAmI].spamDelBlock += 1f;
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (b7 == 1 || b7 == 3)
                                                                                    {
                                                                                        Netplay.serverSock[whoAmI].spamAddBlock += 1f;
                                                                                    }
                                                                                }
                                                                            }
                                                                            if (!Netplay.serverSock[whoAmI].tileSection[Netplay.GetSectionX(num29), Netplay.GetSectionY(num30)])
                                                                            {
                                                                                flag4 = true;
                                                                            }
                                                                        }
                                                                        if (b7 == 0)
                                                                        {
                                                                            WorldGen.KillTile(num29, num30, flag4, false, false);
                                                                        }
                                                                        else
                                                                        {
                                                                            if (b7 == 1)
                                                                            {
                                                                                WorldGen.PlaceTile(num29, num30, b8, false, true, -1, num31);
                                                                            }
                                                                            else
                                                                            {
                                                                                if (b7 == 2)
                                                                                {
                                                                                    WorldGen.KillWall(num29, num30, flag4);
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (b7 == 3)
                                                                                    {
                                                                                        WorldGen.PlaceWall(num29, num30, b8, false);
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (b7 == 4)
                                                                                        {
                                                                                            WorldGen.KillTile(num29, num30, flag4, false, true);
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (b7 == 5)
                                                                                            {
                                                                                                WorldGen.PlaceWire(num29, num30);
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (b7 == 6)
                                                                                                {
                                                                                                    WorldGen.KillWire(num29, num30);
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                        if (Main.netMode == 2)
                                                                        {
                                                                            NetMessage.SendData(17, -1, whoAmI, "", b7, num29, num30, b8, num31);
                                                                            if (b7 == 1 && b8 == 53)
                                                                            {
                                                                                NetMessage.SendTileSquare(-1, num29, num30, 1);
                                                                                return;
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        if (b == 18)
                                                                        {
                                                                            if (Main.netMode == 1)
                                                                            {
                                                                                byte b9 = readBuffer[num];
                                                                                num++;
                                                                                int num32 = BitConverter.ToInt32(readBuffer, num);
                                                                                num += 4;
                                                                                short sunModY = BitConverter.ToInt16(readBuffer, num);
                                                                                num += 2;
                                                                                short moonModY = BitConverter.ToInt16(readBuffer, num);
                                                                                num += 2;
                                                                                if (b9 == 1)
                                                                                {
                                                                                    Main.dayTime = true;
                                                                                }
                                                                                else
                                                                                {
                                                                                    Main.dayTime = false;
                                                                                }
                                                                                Main.time = num32;
                                                                                Main.sunModY = sunModY;
                                                                                Main.moonModY = moonModY;
                                                                                if (Main.netMode == 2)
                                                                                {
                                                                                    NetMessage.SendData(18, -1, whoAmI, "", 0, 0f, 0f, 0f, 0);
                                                                                    return;
                                                                                }
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            if (b == 19)
                                                                            {
                                                                                byte b10 = readBuffer[num];
                                                                                num++;
                                                                                int num33 = BitConverter.ToInt32(readBuffer, num);
                                                                                num += 4;
                                                                                int num34 = BitConverter.ToInt32(readBuffer, num);
                                                                                num += 4;
                                                                                int num35 = readBuffer[num];
                                                                                int direction = 0;
                                                                                if (num35 == 0)
                                                                                {
                                                                                    direction = -1;
                                                                                }
                                                                                if (b10 == 0)
                                                                                {
                                                                                    WorldGen.OpenDoor(num33, num34, direction);
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (b10 == 1)
                                                                                    {
                                                                                        WorldGen.CloseDoor(num33, num34, true);
                                                                                    }
                                                                                }
                                                                                if (Main.netMode == 2)
                                                                                {
                                                                                    NetMessage.SendData(19, -1, whoAmI, "", b10, num33, num34, num35, 0);
                                                                                    return;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                if (b == 20)
                                                                                {
                                                                                    short num36 = BitConverter.ToInt16(readBuffer, start + 1);
                                                                                    int num37 = BitConverter.ToInt32(readBuffer, start + 3);
                                                                                    int num38 = BitConverter.ToInt32(readBuffer, start + 7);
                                                                                    num = start + 11;
                                                                                    for (int num39 = num37; num39 < num37 + (int) num36; num39++)
                                                                                    {
                                                                                        for (int num40 = num38; num40 < num38 + (int) num36; num40++)
                                                                                        {
                                                                                            byte b11 = readBuffer[num];
                                                                                            num++;
                                                                                            bool active2 = Main.tile[num39, num40].active;
                                                                                            if ((b11 & 1) == 1)
                                                                                            {
                                                                                                Main.tile[num39, num40].active = true;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                Main.tile[num39, num40].active = false;
                                                                                            }
                                                                                            var arg_22BD_0 = (byte) (b11 & 2);
                                                                                            if ((b11 & 4) == 4)
                                                                                            {
                                                                                                Main.tile[num39, num40].wall = 1;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                Main.tile[num39, num40].wall = 0;
                                                                                            }
                                                                                            if ((b11 & 8) == 8)
                                                                                            {
                                                                                                Main.tile[num39, num40].liquid = 1;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                Main.tile[num39, num40].liquid = 0;
                                                                                            }
                                                                                            if ((b11 & 16) == 16)
                                                                                            {
                                                                                                Main.tile[num39, num40].wire = true;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                Main.tile[num39, num40].wire = false;
                                                                                            }
                                                                                            if (Main.tile[num39, num40].active)
                                                                                            {
                                                                                                int type3 = Main.tile[num39, num40].type;
                                                                                                Main.tile[num39, num40].type = readBuffer[num];
                                                                                                num++;
                                                                                                if (Main.tileFrameImportant[Main.tile[num39, num40].type])
                                                                                                {
                                                                                                    Main.tile[num39, num40].frameX = BitConverter.ToInt16(readBuffer, num);
                                                                                                    num += 2;
                                                                                                    Main.tile[num39, num40].frameY = BitConverter.ToInt16(readBuffer, num);
                                                                                                    num += 2;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (!active2 || Main.tile[num39, num40].type != type3)
                                                                                                    {
                                                                                                        Main.tile[num39, num40].frameX = -1;
                                                                                                        Main.tile[num39, num40].frameY = -1;
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                            if (Main.tile[num39, num40].wall > 0)
                                                                                            {
                                                                                                Main.tile[num39, num40].wall = readBuffer[num];
                                                                                                num++;
                                                                                            }
                                                                                            if (Main.tile[num39, num40].liquid > 0)
                                                                                            {
                                                                                                Main.tile[num39, num40].liquid = readBuffer[num];
                                                                                                num++;
                                                                                                byte b12 = readBuffer[num];
                                                                                                num++;
                                                                                                if (b12 == 1)
                                                                                                {
                                                                                                    Main.tile[num39, num40].lava = true;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    Main.tile[num39, num40].lava = false;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    WorldGen.RangeFrame(num37, num38, num37 + num36, num38 + num36);
                                                                                    if (Main.netMode == 2)
                                                                                    {
                                                                                        NetMessage.SendData(b, -1, whoAmI, "", num36, num37, num38, 0f, 0);
                                                                                        return;
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (b == 21)
                                                                                    {
                                                                                        short num41 = BitConverter.ToInt16(readBuffer, num);
                                                                                        num += 2;
                                                                                        float num42 = BitConverter.ToSingle(readBuffer, num);
                                                                                        num += 4;
                                                                                        float num43 = BitConverter.ToSingle(readBuffer, num);
                                                                                        num += 4;
                                                                                        float x3 = BitConverter.ToSingle(readBuffer, num);
                                                                                        num += 4;
                                                                                        float y2 = BitConverter.ToSingle(readBuffer, num);
                                                                                        num += 4;
                                                                                        byte stack2 = readBuffer[num];
                                                                                        num++;
                                                                                        byte pre = readBuffer[num];
                                                                                        num++;
                                                                                        short num44 = BitConverter.ToInt16(readBuffer, num);
                                                                                        if (Main.netMode == 1)
                                                                                        {
                                                                                            if (num44 == 0)
                                                                                            {
                                                                                                Main.item[num41].active = false;
                                                                                                return;
                                                                                            }
                                                                                            Main.item[num41].netDefaults(num44);
                                                                                            Main.item[num41].Prefix(pre);
                                                                                            Main.item[num41].stack = stack2;
                                                                                            Main.item[num41].position.X = num42;
                                                                                            Main.item[num41].position.Y = num43;
                                                                                            Main.item[num41].velocity.X = x3;
                                                                                            Main.item[num41].velocity.Y = y2;
                                                                                            Main.item[num41].active = true;
                                                                                            Main.item[num41].wet = Collision.WetCollision(Main.item[num41].position, Main.item[num41].width, Main.item[num41].height);
                                                                                            return;
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (num44 == 0)
                                                                                            {
                                                                                                if (num41 < 200)
                                                                                                {
                                                                                                    Main.item[num41].active = false;
                                                                                                    NetMessage.SendData(21, -1, -1, "", num41, 0f, 0f, 0f, 0);
                                                                                                    return;
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                bool flag5 = false;
                                                                                                if (num41 == 200)
                                                                                                {
                                                                                                    flag5 = true;
                                                                                                }
                                                                                                if (flag5)
                                                                                                {
                                                                                                    var item = new Item();
                                                                                                    item.netDefaults(num44);
                                                                                                    num41 = (short) Item.NewItem((int) num42, (int) num43, item.width, item.height, item.type, stack2, true, 0);
                                                                                                }
                                                                                                Main.item[num41].netDefaults(num44);
                                                                                                Main.item[num41].Prefix(pre);
                                                                                                Main.item[num41].stack = stack2;
                                                                                                Main.item[num41].position.X = num42;
                                                                                                Main.item[num41].position.Y = num43;
                                                                                                Main.item[num41].velocity.X = x3;
                                                                                                Main.item[num41].velocity.Y = y2;
                                                                                                Main.item[num41].active = true;
                                                                                                Main.item[num41].owner = Main.myPlayer;
                                                                                                if (flag5)
                                                                                                {
                                                                                                    NetMessage.SendData(21, -1, -1, "", num41, 0f, 0f, 0f, 0);
                                                                                                    Main.item[num41].ownIgnore = whoAmI;
                                                                                                    Main.item[num41].ownTime = 100;
                                                                                                    Main.item[num41].FindOwner(num41);
                                                                                                    return;
                                                                                                }
                                                                                                NetMessage.SendData(21, -1, whoAmI, "", num41, 0f, 0f, 0f, 0);
                                                                                                return;
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        if (b == 22)
                                                                                        {
                                                                                            short num45 = BitConverter.ToInt16(readBuffer, num);
                                                                                            num += 2;
                                                                                            byte b13 = readBuffer[num];
                                                                                            if (Main.netMode == 2 && Main.item[num45].owner != whoAmI)
                                                                                            {
                                                                                                return;
                                                                                            }
                                                                                            Main.item[num45].owner = b13;
                                                                                            if (b13 == Main.myPlayer)
                                                                                            {
                                                                                                Main.item[num45].keepTime = 15;
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                Main.item[num45].keepTime = 0;
                                                                                            }
                                                                                            if (Main.netMode == 2)
                                                                                            {
                                                                                                Main.item[num45].owner = 255;
                                                                                                Main.item[num45].keepTime = 15;
                                                                                                NetMessage.SendData(22, -1, -1, "", num45, 0f, 0f, 0f, 0);
                                                                                                return;
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (b == 23 && Main.netMode == 1)
                                                                                            {
                                                                                                short num46 = BitConverter.ToInt16(readBuffer, num);
                                                                                                num += 2;
                                                                                                float x4 = BitConverter.ToSingle(readBuffer, num);
                                                                                                num += 4;
                                                                                                float y3 = BitConverter.ToSingle(readBuffer, num);
                                                                                                num += 4;
                                                                                                float x5 = BitConverter.ToSingle(readBuffer, num);
                                                                                                num += 4;
                                                                                                float y4 = BitConverter.ToSingle(readBuffer, num);
                                                                                                num += 4;
                                                                                                int target = BitConverter.ToInt16(readBuffer, num);
                                                                                                num += 2;
                                                                                                int direction2 = (readBuffer[num] - 1);
                                                                                                num++;
                                                                                                int directionY = (readBuffer[num] - 1);
                                                                                                num++;
                                                                                                int num47 = BitConverter.ToInt32(readBuffer, num);
                                                                                                num += 4;
                                                                                                var array = new float[NPC.maxAI];
                                                                                                for (int num48 = 0; num48 < NPC.maxAI; num48++)
                                                                                                {
                                                                                                    array[num48] = BitConverter.ToSingle(readBuffer, num);
                                                                                                    num += 4;
                                                                                                }
                                                                                                int num49 = BitConverter.ToInt16(readBuffer, num);
                                                                                                if (!Main.npc[num46].active || Main.npc[num46].netID != num49)
                                                                                                {
                                                                                                    Main.npc[num46].active = true;
                                                                                                    Main.npc[num46].netDefaults(num49);
                                                                                                }
                                                                                                Main.npc[num46].position.X = x4;
                                                                                                Main.npc[num46].position.Y = y3;
                                                                                                Main.npc[num46].velocity.X = x5;
                                                                                                Main.npc[num46].velocity.Y = y4;
                                                                                                Main.npc[num46].target = target;
                                                                                                Main.npc[num46].direction = direction2;
                                                                                                Main.npc[num46].directionY = directionY;
                                                                                                Main.npc[num46].life = num47;
                                                                                                if (num47 <= 0)
                                                                                                {
                                                                                                    Main.npc[num46].active = false;
                                                                                                }
                                                                                                for (int num50 = 0; num50 < NPC.maxAI; num50++)
                                                                                                {
                                                                                                    Main.npc[num46].ai[num50] = array[num50];
                                                                                                }
                                                                                                return;
                                                                                            }
                                                                                            if (b == 24)
                                                                                            {
                                                                                                short num51 = BitConverter.ToInt16(readBuffer, num);
                                                                                                num += 2;
                                                                                                byte b14 = readBuffer[num];
                                                                                                if (Main.netMode == 2)
                                                                                                {
                                                                                                    b14 = (byte) whoAmI;
                                                                                                }
                                                                                                Main.npc[num51].StrikeNPC(Main.player[b14].inventory[Main.player[b14].selectedItem].damage, Main.player[b14].inventory[Main.player[b14].selectedItem].knockBack, Main.player[b14].direction, false, false);
                                                                                                if (Main.netMode == 2)
                                                                                                {
                                                                                                    NetMessage.SendData(24, -1, whoAmI, "", num51, b14, 0f, 0f, 0);
                                                                                                    NetMessage.SendData(23, -1, -1, "", num51, 0f, 0f, 0f, 0);
                                                                                                    return;
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                if (b == 25)
                                                                                                {
                                                                                                    int num52 = readBuffer[start + 1];
                                                                                                    if (Main.netMode == 2)
                                                                                                    {
                                                                                                        num52 = whoAmI;
                                                                                                    }
                                                                                                    byte b15 = readBuffer[start + 2];
                                                                                                    byte b16 = readBuffer[start + 3];
                                                                                                    byte b17 = readBuffer[start + 4];
                                                                                                    if (Main.netMode == 2)
                                                                                                    {
                                                                                                        b15 = 255;
                                                                                                        b16 = 255;
                                                                                                        b17 = 255;
                                                                                                    }
                                                                                                    string string3 = Encoding.ASCII.GetString(readBuffer, start + 5, length - 5);
                                                                                                    if (Main.netMode == 1)
                                                                                                    {
                                                                                                        string newText = string3;
                                                                                                        if (num52 < 255)
                                                                                                        {
                                                                                                            newText = "<" + Main.player[num52].name + "> " + string3;
                                                                                                            Main.player[num52].chatText = string3;
                                                                                                            Main.player[num52].chatShowTime = Main.chatLength/2;
                                                                                                        }
                                                                                                        Main.NewText(newText, b15, b16, b17);
                                                                                                        return;
                                                                                                    }
                                                                                                    if (Main.netMode == 2)
                                                                                                    {
                                                                                                        string text2 = string3.ToLower();
                                                                                                        if (text2 == "/playing")
                                                                                                        {
                                                                                                            string text3 = "";
                                                                                                            for (int num53 = 0; num53 < 255; num53++)
                                                                                                            {
                                                                                                                if (Main.player[num53].active)
                                                                                                                {
                                                                                                                    if (text3 == "")
                                                                                                                    {
                                                                                                                        text3 += Main.player[num53].name;
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        text3 = text3 + ", " + Main.player[num53].name;
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                            NetMessage.SendData(25, whoAmI, -1, "Current players: " + text3 + ".", 255, 255f, 240f, 20f, 0);
                                                                                                            return;
                                                                                                        }
                                                                                                        if (text2.Length >= 4 && text2.Substring(0, 4) == "/me ")
                                                                                                        {
                                                                                                            NetMessage.SendData(25, -1, -1, "*" + Main.player[whoAmI].name + " " + string3.Substring(4), 255, 200f, 100f, 0f, 0);
                                                                                                            return;
                                                                                                        }
                                                                                                        if (text2 == "/roll")
                                                                                                        {
                                                                                                            NetMessage.SendData(25, -1, -1, string.Concat(new object[]
                                                                                                                                                              {
                                                                                                                                                                  "*",
                                                                                                                                                                  Main.player[whoAmI].name,
                                                                                                                                                                  " rolls a ",
                                                                                                                                                                  Main.rand.Next(1, 101)
                                                                                                                                                              }), 255, 255f, 240f, 20f, 0);
                                                                                                            return;
                                                                                                        }
                                                                                                        if (text2.Length >= 3 && text2.Substring(0, 3) == "/p ")
                                                                                                        {
                                                                                                            if (Main.player[whoAmI].team != 0)
                                                                                                            {
                                                                                                                for (int num54 = 0; num54 < 255; num54++)
                                                                                                                {
                                                                                                                    if (Main.player[num54].team == Main.player[whoAmI].team)
                                                                                                                    {
                                                                                                                        NetMessage.SendData(25, num54, -1, string3.Substring(3), num52, Main.teamColor[Main.player[whoAmI].team].R, Main.teamColor[Main.player[whoAmI].team].G, Main.teamColor[Main.player[whoAmI].team].B, 0);
                                                                                                                    }
                                                                                                                }
                                                                                                                return;
                                                                                                            }
                                                                                                            NetMessage.SendData(25, whoAmI, -1, "You are not in a party!", 255, 255f, 240f, 20f, 0);
                                                                                                            return;
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (Main.player[whoAmI].difficulty == 2)
                                                                                                            {
                                                                                                                b15 = Main.hcColor.R;
                                                                                                                b16 = Main.hcColor.G;
                                                                                                                b17 = Main.hcColor.B;
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (Main.player[whoAmI].difficulty == 1)
                                                                                                                {
                                                                                                                    b15 = Main.mcColor.R;
                                                                                                                    b16 = Main.mcColor.G;
                                                                                                                    b17 = Main.mcColor.B;
                                                                                                                }
                                                                                                            }
                                                                                                            NetMessage.SendData(25, -1, -1, string3, num52, b15, b16, b17, 0);
                                                                                                            if (Main.dedServ)
                                                                                                            {
                                                                                                                Console.WriteLine("<" + Main.player[whoAmI].name + "> " + string3);
                                                                                                                return;
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    if (b == 26)
                                                                                                    {
                                                                                                        byte b18 = readBuffer[num];
                                                                                                        if (Main.netMode == 2 && whoAmI != b18 && (!Main.player[b18].hostile || !Main.player[whoAmI].hostile))
                                                                                                        {
                                                                                                            return;
                                                                                                        }
                                                                                                        num++;
                                                                                                        int num55 = (readBuffer[num] - 1);
                                                                                                        num++;
                                                                                                        short num56 = BitConverter.ToInt16(readBuffer, num);
                                                                                                        num += 2;
                                                                                                        byte b19 = readBuffer[num];
                                                                                                        num++;
                                                                                                        bool pvp = false;
                                                                                                        byte b20 = readBuffer[num];
                                                                                                        num++;
                                                                                                        bool crit = false;
                                                                                                        string string4 = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
                                                                                                        if (b19 != 0)
                                                                                                        {
                                                                                                            pvp = true;
                                                                                                        }
                                                                                                        if (b20 != 0)
                                                                                                        {
                                                                                                            crit = true;
                                                                                                        }
                                                                                                        Main.player[b18].Hurt(num56, num55, pvp, true, string4, crit);
                                                                                                        if (Main.netMode == 2)
                                                                                                        {
                                                                                                            NetMessage.SendData(26, -1, whoAmI, string4, b18, num55, num56, b19, 0);
                                                                                                            return;
                                                                                                        }
                                                                                                    }
                                                                                                    else
                                                                                                    {
                                                                                                        if (b == 27)
                                                                                                        {
                                                                                                            short num57 = BitConverter.ToInt16(readBuffer, num);
                                                                                                            num += 2;
                                                                                                            float x6 = BitConverter.ToSingle(readBuffer, num);
                                                                                                            num += 4;
                                                                                                            float y5 = BitConverter.ToSingle(readBuffer, num);
                                                                                                            num += 4;
                                                                                                            float x7 = BitConverter.ToSingle(readBuffer, num);
                                                                                                            num += 4;
                                                                                                            float y6 = BitConverter.ToSingle(readBuffer, num);
                                                                                                            num += 4;
                                                                                                            float knockBack = BitConverter.ToSingle(readBuffer, num);
                                                                                                            num += 4;
                                                                                                            short damage = BitConverter.ToInt16(readBuffer, num);
                                                                                                            num += 2;
                                                                                                            byte b21 = readBuffer[num];
                                                                                                            num++;
                                                                                                            byte b22 = readBuffer[num];
                                                                                                            num++;
                                                                                                            var array2 = new float[Projectile.maxAI];
                                                                                                            for (int num58 = 0; num58 < Projectile.maxAI; num58++)
                                                                                                            {
                                                                                                                array2[num58] = BitConverter.ToSingle(readBuffer, num);
                                                                                                                num += 4;
                                                                                                            }
                                                                                                            int num59 = 1000;
                                                                                                            for (int num60 = 0; num60 < 1000; num60++)
                                                                                                            {
                                                                                                                if (Main.projectile[num60].owner == b21 && Main.projectile[num60].identity == num57 && Main.projectile[num60].active)
                                                                                                                {
                                                                                                                    num59 = num60;
                                                                                                                    break;
                                                                                                                }
                                                                                                            }
                                                                                                            if (num59 == 1000)
                                                                                                            {
                                                                                                                for (int num61 = 0; num61 < 1000; num61++)
                                                                                                                {
                                                                                                                    if (!Main.projectile[num61].active)
                                                                                                                    {
                                                                                                                        num59 = num61;
                                                                                                                        break;
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                            if (!Main.projectile[num59].active || Main.projectile[num59].type != b22)
                                                                                                            {
                                                                                                                Main.projectile[num59].SetDefaults(b22);
                                                                                                                if (Main.netMode == 2)
                                                                                                                {
                                                                                                                    Netplay.serverSock[whoAmI].spamProjectile += 1f;
                                                                                                                }
                                                                                                            }
                                                                                                            Main.projectile[num59].identity = num57;
                                                                                                            Main.projectile[num59].position.X = x6;
                                                                                                            Main.projectile[num59].position.Y = y5;
                                                                                                            Main.projectile[num59].velocity.X = x7;
                                                                                                            Main.projectile[num59].velocity.Y = y6;
                                                                                                            Main.projectile[num59].damage = damage;
                                                                                                            Main.projectile[num59].type = b22;
                                                                                                            Main.projectile[num59].owner = b21;
                                                                                                            Main.projectile[num59].knockBack = knockBack;
                                                                                                            for (int num62 = 0; num62 < Projectile.maxAI; num62++)
                                                                                                            {
                                                                                                                Main.projectile[num59].ai[num62] = array2[num62];
                                                                                                            }
                                                                                                            if (Main.netMode == 2)
                                                                                                            {
                                                                                                                NetMessage.SendData(27, -1, whoAmI, "", num59, 0f, 0f, 0f, 0);
                                                                                                                return;
                                                                                                            }
                                                                                                        }
                                                                                                        else
                                                                                                        {
                                                                                                            if (b == 28)
                                                                                                            {
                                                                                                                short num63 = BitConverter.ToInt16(readBuffer, num);
                                                                                                                num += 2;
                                                                                                                short num64 = BitConverter.ToInt16(readBuffer, num);
                                                                                                                num += 2;
                                                                                                                float num65 = BitConverter.ToSingle(readBuffer, num);
                                                                                                                num += 4;
                                                                                                                int num66 = (readBuffer[num] - 1);
                                                                                                                num++;
                                                                                                                int num67 = readBuffer[num];
                                                                                                                if (num64 >= 0)
                                                                                                                {
                                                                                                                    if (num67 == 1)
                                                                                                                    {
                                                                                                                        Main.npc[num63].StrikeNPC(num64, num65, num66, true, false);
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        Main.npc[num63].StrikeNPC(num64, num65, num66, false, false);
                                                                                                                    }
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    Main.npc[num63].life = 0;
                                                                                                                    Main.npc[num63].HitEffect(0, 10.0);
                                                                                                                    Main.npc[num63].active = false;
                                                                                                                }
                                                                                                                if (Main.netMode == 2)
                                                                                                                {
                                                                                                                    if (Main.npc[num63].life <= 0)
                                                                                                                    {
                                                                                                                        NetMessage.SendData(28, -1, whoAmI, "", num63, num64, num65, num66, num67);
                                                                                                                        NetMessage.SendData(23, -1, -1, "", num63, 0f, 0f, 0f, 0);
                                                                                                                        return;
                                                                                                                    }
                                                                                                                    NetMessage.SendData(28, -1, whoAmI, "", num63, num64, num65, num66, num67);
                                                                                                                    Main.npc[num63].netUpdate = true;
                                                                                                                    return;
                                                                                                                }
                                                                                                            }
                                                                                                            else
                                                                                                            {
                                                                                                                if (b == 29)
                                                                                                                {
                                                                                                                    short num68 = BitConverter.ToInt16(readBuffer, num);
                                                                                                                    num += 2;
                                                                                                                    byte b23 = readBuffer[num];
                                                                                                                    if (Main.netMode == 2)
                                                                                                                    {
                                                                                                                        b23 = (byte) whoAmI;
                                                                                                                    }
                                                                                                                    for (int num69 = 0; num69 < 1000; num69++)
                                                                                                                    {
                                                                                                                        if (Main.projectile[num69].owner == b23 && Main.projectile[num69].identity == num68 && Main.projectile[num69].active)
                                                                                                                        {
                                                                                                                            Main.projectile[num69].Kill();
                                                                                                                            break;
                                                                                                                        }
                                                                                                                    }
                                                                                                                    if (Main.netMode == 2)
                                                                                                                    {
                                                                                                                        NetMessage.SendData(29, -1, whoAmI, "", num68, b23, 0f, 0f, 0);
                                                                                                                        return;
                                                                                                                    }
                                                                                                                }
                                                                                                                else
                                                                                                                {
                                                                                                                    if (b == 30)
                                                                                                                    {
                                                                                                                        byte b24 = readBuffer[num];
                                                                                                                        if (Main.netMode == 2)
                                                                                                                        {
                                                                                                                            b24 = (byte) whoAmI;
                                                                                                                        }
                                                                                                                        num++;
                                                                                                                        byte b25 = readBuffer[num];
                                                                                                                        if (b25 == 1)
                                                                                                                        {
                                                                                                                            Main.player[b24].hostile = true;
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            Main.player[b24].hostile = false;
                                                                                                                        }
                                                                                                                        if (Main.netMode == 2)
                                                                                                                        {
                                                                                                                            NetMessage.SendData(30, -1, whoAmI, "", b24, 0f, 0f, 0f, 0);
                                                                                                                            string str = " has enabled PvP!";
                                                                                                                            if (b25 == 0)
                                                                                                                            {
                                                                                                                                str = " has disabled PvP!";
                                                                                                                            }
                                                                                                                            NetMessage.SendData(25, -1, -1, Main.player[b24].name + str, 255, Main.teamColor[Main.player[b24].team].R, Main.teamColor[Main.player[b24].team].G, Main.teamColor[Main.player[b24].team].B, 0);
                                                                                                                            return;
                                                                                                                        }
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        if (b == 31)
                                                                                                                        {
                                                                                                                            if (Main.netMode == 2)
                                                                                                                            {
                                                                                                                                int x8 = BitConverter.ToInt32(readBuffer, num);
                                                                                                                                num += 4;
                                                                                                                                int y7 = BitConverter.ToInt32(readBuffer, num);
                                                                                                                                num += 4;
                                                                                                                                int num70 = Chest.FindChest(x8, y7);
                                                                                                                                if (num70 > -1 && Chest.UsingChest(num70) == -1)
                                                                                                                                {
                                                                                                                                    for (int num71 = 0; num71 < Chest.maxItems; num71++)
                                                                                                                                    {
                                                                                                                                        NetMessage.SendData(32, whoAmI, -1, "", num70, num71, 0f, 0f, 0);
                                                                                                                                    }
                                                                                                                                    NetMessage.SendData(33, whoAmI, -1, "", num70, 0f, 0f, 0f, 0);
                                                                                                                                    Main.player[whoAmI].chest = num70;
                                                                                                                                    return;
                                                                                                                                }
                                                                                                                            }
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            if (b == 32)
                                                                                                                            {
                                                                                                                                int num72 = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                num += 2;
                                                                                                                                int num73 = readBuffer[num];
                                                                                                                                num++;
                                                                                                                                int stack3 = readBuffer[num];
                                                                                                                                num++;
                                                                                                                                int pre2 = readBuffer[num];
                                                                                                                                num++;
                                                                                                                                int type4 = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                if (Main.chest[num72] == null)
                                                                                                                                {
                                                                                                                                    Main.chest[num72] = new Chest();
                                                                                                                                }
                                                                                                                                if (Main.chest[num72].item[num73] == null)
                                                                                                                                {
                                                                                                                                    Main.chest[num72].item[num73] = new Item();
                                                                                                                                }
                                                                                                                                Main.chest[num72].item[num73].netDefaults(type4);
                                                                                                                                Main.chest[num72].item[num73].Prefix(pre2);
                                                                                                                                Main.chest[num72].item[num73].stack = stack3;
                                                                                                                                return;
                                                                                                                            }
                                                                                                                            if (b == 33)
                                                                                                                            {
                                                                                                                                int num74 = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                num += 2;
                                                                                                                                int chestX = BitConverter.ToInt32(readBuffer, num);
                                                                                                                                num += 4;
                                                                                                                                int chestY = BitConverter.ToInt32(readBuffer, num);
                                                                                                                                if (Main.netMode == 1)
                                                                                                                                {
                                                                                                                                    if (Main.player[Main.myPlayer].chest == -1)
                                                                                                                                    {
                                                                                                                                        Main.playerInventory = true;
                                                                                                                                        Main.PlaySound(10, -1, -1, 1);
                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        if (Main.player[Main.myPlayer].chest != num74 && num74 != -1)
                                                                                                                                        {
                                                                                                                                            Main.playerInventory = true;
                                                                                                                                            Main.PlaySound(12, -1, -1, 1);
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            if (Main.player[Main.myPlayer].chest != -1 && num74 == -1)
                                                                                                                                            {
                                                                                                                                                Main.PlaySound(11, -1, -1, 1);
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                    Main.player[Main.myPlayer].chest = num74;
                                                                                                                                    Main.player[Main.myPlayer].chestX = chestX;
                                                                                                                                    Main.player[Main.myPlayer].chestY = chestY;
                                                                                                                                    return;
                                                                                                                                }
                                                                                                                                Main.player[whoAmI].chest = num74;
                                                                                                                                return;
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                if (b == 34)
                                                                                                                                {
                                                                                                                                    if (Main.netMode == 2)
                                                                                                                                    {
                                                                                                                                        int num75 = BitConverter.ToInt32(readBuffer, num);
                                                                                                                                        num += 4;
                                                                                                                                        int num76 = BitConverter.ToInt32(readBuffer, num);
                                                                                                                                        if (Main.tile[num75, num76].type == 21)
                                                                                                                                        {
                                                                                                                                            WorldGen.KillTile(num75, num76, false, false, false);
                                                                                                                                            if (!Main.tile[num75, num76].active)
                                                                                                                                            {
                                                                                                                                                NetMessage.SendData(17, -1, -1, "", 0, num75, num76, 0f, 0);
                                                                                                                                                return;
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                }
                                                                                                                                else
                                                                                                                                {
                                                                                                                                    if (b == 35)
                                                                                                                                    {
                                                                                                                                        int num77 = readBuffer[num];
                                                                                                                                        if (Main.netMode == 2)
                                                                                                                                        {
                                                                                                                                            num77 = whoAmI;
                                                                                                                                        }
                                                                                                                                        num++;
                                                                                                                                        int num78 = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                        num += 2;
                                                                                                                                        if (num77 != Main.myPlayer)
                                                                                                                                        {
                                                                                                                                            Main.player[num77].HealEffect(num78);
                                                                                                                                        }
                                                                                                                                        if (Main.netMode == 2)
                                                                                                                                        {
                                                                                                                                            NetMessage.SendData(35, -1, whoAmI, "", num77, num78, 0f, 0f, 0);
                                                                                                                                            return;
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                    else
                                                                                                                                    {
                                                                                                                                        if (b == 36)
                                                                                                                                        {
                                                                                                                                            int num79 = readBuffer[num];
                                                                                                                                            if (Main.netMode == 2)
                                                                                                                                            {
                                                                                                                                                num79 = whoAmI;
                                                                                                                                            }
                                                                                                                                            num++;
                                                                                                                                            int num80 = readBuffer[num];
                                                                                                                                            num++;
                                                                                                                                            int num81 = readBuffer[num];
                                                                                                                                            num++;
                                                                                                                                            int num82 = readBuffer[num];
                                                                                                                                            num++;
                                                                                                                                            int num83 = readBuffer[num];
                                                                                                                                            num++;
                                                                                                                                            int num84 = readBuffer[num];
                                                                                                                                            num++;
                                                                                                                                            if (num80 == 0)
                                                                                                                                            {
                                                                                                                                                Main.player[num79].zoneEvil = false;
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                Main.player[num79].zoneEvil = true;
                                                                                                                                            }
                                                                                                                                            if (num81 == 0)
                                                                                                                                            {
                                                                                                                                                Main.player[num79].zoneMeteor = false;
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                Main.player[num79].zoneMeteor = true;
                                                                                                                                            }
                                                                                                                                            if (num82 == 0)
                                                                                                                                            {
                                                                                                                                                Main.player[num79].zoneDungeon = false;
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                Main.player[num79].zoneDungeon = true;
                                                                                                                                            }
                                                                                                                                            if (num83 == 0)
                                                                                                                                            {
                                                                                                                                                Main.player[num79].zoneJungle = false;
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                Main.player[num79].zoneJungle = true;
                                                                                                                                            }
                                                                                                                                            if (num84 == 0)
                                                                                                                                            {
                                                                                                                                                Main.player[num79].zoneHoly = false;
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                Main.player[num79].zoneHoly = true;
                                                                                                                                            }
                                                                                                                                            if (Main.netMode == 2)
                                                                                                                                            {
                                                                                                                                                NetMessage.SendData(36, -1, whoAmI, "", num79, 0f, 0f, 0f, 0);
                                                                                                                                                return;
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                        else
                                                                                                                                        {
                                                                                                                                            if (b == 37)
                                                                                                                                            {
                                                                                                                                                if (Main.netMode == 1)
                                                                                                                                                {
                                                                                                                                                    if (Main.autoPass)
                                                                                                                                                    {
                                                                                                                                                        NetMessage.SendData(38, -1, -1, Netplay.password, 0, 0f, 0f, 0f, 0);
                                                                                                                                                        Main.autoPass = false;
                                                                                                                                                        return;
                                                                                                                                                    }
                                                                                                                                                    Netplay.password = "";
                                                                                                                                                    Main.menuMode = 31;
                                                                                                                                                    return;
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                            else
                                                                                                                                            {
                                                                                                                                                if (b == 38)
                                                                                                                                                {
                                                                                                                                                    if (Main.netMode == 2)
                                                                                                                                                    {
                                                                                                                                                        string string5 = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
                                                                                                                                                        if (string5 == Netplay.password)
                                                                                                                                                        {
                                                                                                                                                            Netplay.serverSock[whoAmI].state = 1;
                                                                                                                                                            NetMessage.SendData(3, whoAmI, -1, "", 0, 0f, 0f, 0f, 0);
                                                                                                                                                            return;
                                                                                                                                                        }
                                                                                                                                                        NetMessage.SendData(2, whoAmI, -1, "Incorrect password.", 0, 0f, 0f, 0f, 0);
                                                                                                                                                        return;
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                                else
                                                                                                                                                {
                                                                                                                                                    if (b == 39 && Main.netMode == 1)
                                                                                                                                                    {
                                                                                                                                                        short num85 = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                                        Main.item[num85].owner = 255;
                                                                                                                                                        NetMessage.SendData(22, -1, -1, "", num85, 0f, 0f, 0f, 0);
                                                                                                                                                        return;
                                                                                                                                                    }
                                                                                                                                                    if (b == 40)
                                                                                                                                                    {
                                                                                                                                                        byte b26 = readBuffer[num];
                                                                                                                                                        if (Main.netMode == 2)
                                                                                                                                                        {
                                                                                                                                                            b26 = (byte) whoAmI;
                                                                                                                                                        }
                                                                                                                                                        num++;
                                                                                                                                                        int talkNPC = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                                        num += 2;
                                                                                                                                                        Main.player[b26].talkNPC = talkNPC;
                                                                                                                                                        if (Main.netMode == 2)
                                                                                                                                                        {
                                                                                                                                                            NetMessage.SendData(40, -1, whoAmI, "", b26, 0f, 0f, 0f, 0);
                                                                                                                                                            return;
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                    else
                                                                                                                                                    {
                                                                                                                                                        if (b == 41)
                                                                                                                                                        {
                                                                                                                                                            byte b27 = readBuffer[num];
                                                                                                                                                            if (Main.netMode == 2)
                                                                                                                                                            {
                                                                                                                                                                b27 = (byte) whoAmI;
                                                                                                                                                            }
                                                                                                                                                            num++;
                                                                                                                                                            float itemRotation = BitConverter.ToSingle(readBuffer, num);
                                                                                                                                                            num += 4;
                                                                                                                                                            int itemAnimation = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                                            Main.player[b27].itemRotation = itemRotation;
                                                                                                                                                            Main.player[b27].itemAnimation = itemAnimation;
                                                                                                                                                            Main.player[b27].channel = Main.player[b27].inventory[Main.player[b27].selectedItem].channel;
                                                                                                                                                            if (Main.netMode == 2)
                                                                                                                                                            {
                                                                                                                                                                NetMessage.SendData(41, -1, whoAmI, "", b27, 0f, 0f, 0f, 0);
                                                                                                                                                                return;
                                                                                                                                                            }
                                                                                                                                                        }
                                                                                                                                                        else
                                                                                                                                                        {
                                                                                                                                                            if (b == 42)
                                                                                                                                                            {
                                                                                                                                                                int num86 = readBuffer[num];
                                                                                                                                                                if (Main.netMode == 2)
                                                                                                                                                                {
                                                                                                                                                                    num86 = whoAmI;
                                                                                                                                                                }
                                                                                                                                                                num++;
                                                                                                                                                                int statMana = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                                                num += 2;
                                                                                                                                                                int statManaMax = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                                                if (Main.netMode == 2)
                                                                                                                                                                {
                                                                                                                                                                    num86 = whoAmI;
                                                                                                                                                                }
                                                                                                                                                                Main.player[num86].statMana = statMana;
                                                                                                                                                                Main.player[num86].statManaMax = statManaMax;
                                                                                                                                                                if (Main.netMode == 2)
                                                                                                                                                                {
                                                                                                                                                                    NetMessage.SendData(42, -1, whoAmI, "", num86, 0f, 0f, 0f, 0);
                                                                                                                                                                    return;
                                                                                                                                                                }
                                                                                                                                                            }
                                                                                                                                                            else
                                                                                                                                                            {
                                                                                                                                                                if (b == 43)
                                                                                                                                                                {
                                                                                                                                                                    int num87 = readBuffer[num];
                                                                                                                                                                    if (Main.netMode == 2)
                                                                                                                                                                    {
                                                                                                                                                                        num87 = whoAmI;
                                                                                                                                                                    }
                                                                                                                                                                    num++;
                                                                                                                                                                    int num88 = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                                                    num += 2;
                                                                                                                                                                    if (num87 != Main.myPlayer)
                                                                                                                                                                    {
                                                                                                                                                                        Main.player[num87].ManaEffect(num88);
                                                                                                                                                                    }
                                                                                                                                                                    if (Main.netMode == 2)
                                                                                                                                                                    {
                                                                                                                                                                        NetMessage.SendData(43, -1, whoAmI, "", num87, num88, 0f, 0f, 0);
                                                                                                                                                                        return;
                                                                                                                                                                    }
                                                                                                                                                                }
                                                                                                                                                                else
                                                                                                                                                                {
                                                                                                                                                                    if (b == 44)
                                                                                                                                                                    {
                                                                                                                                                                        byte b28 = readBuffer[num];
                                                                                                                                                                        if (b28 == Main.myPlayer)
                                                                                                                                                                        {
                                                                                                                                                                            return;
                                                                                                                                                                        }
                                                                                                                                                                        if (Main.netMode == 2)
                                                                                                                                                                        {
                                                                                                                                                                            b28 = (byte) whoAmI;
                                                                                                                                                                        }
                                                                                                                                                                        num++;
                                                                                                                                                                        int num89 = (readBuffer[num] - 1);
                                                                                                                                                                        num++;
                                                                                                                                                                        short num90 = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                                                        num += 2;
                                                                                                                                                                        byte b29 = readBuffer[num];
                                                                                                                                                                        num++;
                                                                                                                                                                        string string6 = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
                                                                                                                                                                        bool pvp2 = false;
                                                                                                                                                                        if (b29 != 0)
                                                                                                                                                                        {
                                                                                                                                                                            pvp2 = true;
                                                                                                                                                                        }
                                                                                                                                                                        Main.player[b28].KillMe(num90, num89, pvp2, string6);
                                                                                                                                                                        if (Main.netMode == 2)
                                                                                                                                                                        {
                                                                                                                                                                            NetMessage.SendData(44, -1, whoAmI, string6, b28, num89, num90, b29, 0);
                                                                                                                                                                            return;
                                                                                                                                                                        }
                                                                                                                                                                    }
                                                                                                                                                                    else
                                                                                                                                                                    {
                                                                                                                                                                        if (b == 45)
                                                                                                                                                                        {
                                                                                                                                                                            int num91 = readBuffer[num];
                                                                                                                                                                            if (Main.netMode == 2)
                                                                                                                                                                            {
                                                                                                                                                                                num91 = whoAmI;
                                                                                                                                                                            }
                                                                                                                                                                            num++;
                                                                                                                                                                            int num92 = readBuffer[num];
                                                                                                                                                                            num++;
                                                                                                                                                                            int team = Main.player[num91].team;
                                                                                                                                                                            Main.player[num91].team = num92;
                                                                                                                                                                            if (Main.netMode == 2)
                                                                                                                                                                            {
                                                                                                                                                                                NetMessage.SendData(45, -1, whoAmI, "", num91, 0f, 0f, 0f, 0);
                                                                                                                                                                                string str2 = "";
                                                                                                                                                                                if (num92 == 0)
                                                                                                                                                                                {
                                                                                                                                                                                    str2 = " is no longer on a party.";
                                                                                                                                                                                }
                                                                                                                                                                                else
                                                                                                                                                                                {
                                                                                                                                                                                    if (num92 == 1)
                                                                                                                                                                                    {
                                                                                                                                                                                        str2 = " has joined the red party.";
                                                                                                                                                                                    }
                                                                                                                                                                                    else
                                                                                                                                                                                    {
                                                                                                                                                                                        if (num92 == 2)
                                                                                                                                                                                        {
                                                                                                                                                                                            str2 = " has joined the green party.";
                                                                                                                                                                                        }
                                                                                                                                                                                        else
                                                                                                                                                                                        {
                                                                                                                                                                                            if (num92 == 3)
                                                                                                                                                                                            {
                                                                                                                                                                                                str2 = " has joined the blue party.";
                                                                                                                                                                                            }
                                                                                                                                                                                            else
                                                                                                                                                                                            {
                                                                                                                                                                                                if (num92 == 4)
                                                                                                                                                                                                {
                                                                                                                                                                                                    str2 = " has joined the yellow party.";
                                                                                                                                                                                                }
                                                                                                                                                                                            }
                                                                                                                                                                                        }
                                                                                                                                                                                    }
                                                                                                                                                                                }
                                                                                                                                                                                for (int num93 = 0; num93 < 255; num93++)
                                                                                                                                                                                {
                                                                                                                                                                                    if (num93 == whoAmI || (team > 0 && Main.player[num93].team == team) || (num92 > 0 && Main.player[num93].team == num92))
                                                                                                                                                                                    {
                                                                                                                                                                                        NetMessage.SendData(25, num93, -1, Main.player[num91].name + str2, 255, Main.teamColor[num92].R, Main.teamColor[num92].G, Main.teamColor[num92].B, 0);
                                                                                                                                                                                    }
                                                                                                                                                                                }
                                                                                                                                                                                return;
                                                                                                                                                                            }
                                                                                                                                                                        }
                                                                                                                                                                        else
                                                                                                                                                                        {
                                                                                                                                                                            if (b == 46)
                                                                                                                                                                            {
                                                                                                                                                                                if (Main.netMode == 2)
                                                                                                                                                                                {
                                                                                                                                                                                    int i2 = BitConverter.ToInt32(readBuffer, num);
                                                                                                                                                                                    num += 4;
                                                                                                                                                                                    int j2 = BitConverter.ToInt32(readBuffer, num);
                                                                                                                                                                                    num += 4;
                                                                                                                                                                                    int num94 = Sign.ReadSign(i2, j2);
                                                                                                                                                                                    if (num94 >= 0)
                                                                                                                                                                                    {
                                                                                                                                                                                        NetMessage.SendData(47, whoAmI, -1, "", num94, 0f, 0f, 0f, 0);
                                                                                                                                                                                        return;
                                                                                                                                                                                    }
                                                                                                                                                                                }
                                                                                                                                                                            }
                                                                                                                                                                            else
                                                                                                                                                                            {
                                                                                                                                                                                if (b == 47)
                                                                                                                                                                                {
                                                                                                                                                                                    int num95 = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                                                                    num += 2;
                                                                                                                                                                                    int x9 = BitConverter.ToInt32(readBuffer, num);
                                                                                                                                                                                    num += 4;
                                                                                                                                                                                    int y8 = BitConverter.ToInt32(readBuffer, num);
                                                                                                                                                                                    num += 4;
                                                                                                                                                                                    string string7 = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
                                                                                                                                                                                    Main.sign[num95] = new Sign();
                                                                                                                                                                                    Main.sign[num95].x = x9;
                                                                                                                                                                                    Main.sign[num95].y = y8;
                                                                                                                                                                                    Sign.TextSign(num95, string7);
                                                                                                                                                                                    if (Main.netMode == 1 && Main.sign[num95] != null && num95 != Main.player[Main.myPlayer].sign)
                                                                                                                                                                                    {
                                                                                                                                                                                        Main.playerInventory = false;
                                                                                                                                                                                        Main.player[Main.myPlayer].talkNPC = -1;
                                                                                                                                                                                        Main.editSign = false;
                                                                                                                                                                                        Main.PlaySound(10, -1, -1, 1);
                                                                                                                                                                                        Main.player[Main.myPlayer].sign = num95;
                                                                                                                                                                                        Main.npcChatText = Main.sign[num95].text;
                                                                                                                                                                                        return;
                                                                                                                                                                                    }
                                                                                                                                                                                }
                                                                                                                                                                                else
                                                                                                                                                                                {
                                                                                                                                                                                    if (b == 48)
                                                                                                                                                                                    {
                                                                                                                                                                                        int num96 = BitConverter.ToInt32(readBuffer, num);
                                                                                                                                                                                        num += 4;
                                                                                                                                                                                        int num97 = BitConverter.ToInt32(readBuffer, num);
                                                                                                                                                                                        num += 4;
                                                                                                                                                                                        byte liquid = readBuffer[num];
                                                                                                                                                                                        num++;
                                                                                                                                                                                        byte b30 = readBuffer[num];
                                                                                                                                                                                        num++;
                                                                                                                                                                                        if (Main.netMode == 2 && Netplay.spamCheck)
                                                                                                                                                                                        {
                                                                                                                                                                                            int num98 = whoAmI;
                                                                                                                                                                                            var num99 = (int) (Main.player[num98].position.X + (Main.player[num98].width/2));
                                                                                                                                                                                            var num100 = (int) (Main.player[num98].position.Y + (Main.player[num98].height/2));
                                                                                                                                                                                            int num101 = 10;
                                                                                                                                                                                            int num102 = num99 - num101;
                                                                                                                                                                                            int num103 = num99 + num101;
                                                                                                                                                                                            int num104 = num100 - num101;
                                                                                                                                                                                            int num105 = num100 + num101;
                                                                                                                                                                                            if (num99 < num102 || num99 > num103 || num100 < num104 || num100 > num105)
                                                                                                                                                                                            {
                                                                                                                                                                                                NetMessage.BootPlayer(whoAmI, "Cheating attempt detected: Liquid spam");
                                                                                                                                                                                                return;
                                                                                                                                                                                            }
                                                                                                                                                                                        }
                                                                                                                                                                                        lock (Main.tile[num96, num97])
                                                                                                                                                                                        {
                                                                                                                                                                                            Main.tile[num96, num97].liquid = liquid;
                                                                                                                                                                                            if (b30 == 1)
                                                                                                                                                                                            {
                                                                                                                                                                                                Main.tile[num96, num97].lava = true;
                                                                                                                                                                                            }
                                                                                                                                                                                            else
                                                                                                                                                                                            {
                                                                                                                                                                                                Main.tile[num96, num97].lava = false;
                                                                                                                                                                                            }
                                                                                                                                                                                            if (Main.netMode == 2)
                                                                                                                                                                                            {
                                                                                                                                                                                                WorldGen.SquareTileFrame(num96, num97, true);
                                                                                                                                                                                            }
                                                                                                                                                                                            return;
                                                                                                                                                                                        }
                                                                                                                                                                                    }
                                                                                                                                                                                    if (b == 49)
                                                                                                                                                                                    {
                                                                                                                                                                                        if (Netplay.clientSock.state == 6)
                                                                                                                                                                                        {
                                                                                                                                                                                            Netplay.clientSock.state = 10;
                                                                                                                                                                                            Main.player[Main.myPlayer].Spawn();
                                                                                                                                                                                            return;
                                                                                                                                                                                        }
                                                                                                                                                                                    }
                                                                                                                                                                                    else
                                                                                                                                                                                    {
                                                                                                                                                                                        if (b == 50)
                                                                                                                                                                                        {
                                                                                                                                                                                            int num106 = readBuffer[num];
                                                                                                                                                                                            num++;
                                                                                                                                                                                            if (Main.netMode == 2)
                                                                                                                                                                                            {
                                                                                                                                                                                                num106 = whoAmI;
                                                                                                                                                                                            }
                                                                                                                                                                                            else
                                                                                                                                                                                            {
                                                                                                                                                                                                if (num106 == Main.myPlayer)
                                                                                                                                                                                                {
                                                                                                                                                                                                    return;
                                                                                                                                                                                                }
                                                                                                                                                                                            }
                                                                                                                                                                                            for (int num107 = 0; num107 < 10; num107++)
                                                                                                                                                                                            {
                                                                                                                                                                                                Main.player[num106].buffType[num107] = readBuffer[num];
                                                                                                                                                                                                if (Main.player[num106].buffType[num107] > 0)
                                                                                                                                                                                                {
                                                                                                                                                                                                    Main.player[num106].buffTime[num107] = 60;
                                                                                                                                                                                                }
                                                                                                                                                                                                else
                                                                                                                                                                                                {
                                                                                                                                                                                                    Main.player[num106].buffTime[num107] = 0;
                                                                                                                                                                                                }
                                                                                                                                                                                                num++;
                                                                                                                                                                                            }
                                                                                                                                                                                            if (Main.netMode == 2)
                                                                                                                                                                                            {
                                                                                                                                                                                                NetMessage.SendData(50, -1, whoAmI, "", num106, 0f, 0f, 0f, 0);
                                                                                                                                                                                                return;
                                                                                                                                                                                            }
                                                                                                                                                                                        }
                                                                                                                                                                                        else
                                                                                                                                                                                        {
                                                                                                                                                                                            if (b == 51)
                                                                                                                                                                                            {
                                                                                                                                                                                                byte b31 = readBuffer[num];
                                                                                                                                                                                                num++;
                                                                                                                                                                                                byte b32 = readBuffer[num];
                                                                                                                                                                                                if (b32 == 1)
                                                                                                                                                                                                {
                                                                                                                                                                                                    NPC.SpawnSkeletron();
                                                                                                                                                                                                    return;
                                                                                                                                                                                                }
                                                                                                                                                                                                if (b32 == 2)
                                                                                                                                                                                                {
                                                                                                                                                                                                    if (Main.netMode != 2)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        Main.PlaySound(2, (int) Main.player[b31].position.X, (int) Main.player[b31].position.Y, 1);
                                                                                                                                                                                                        return;
                                                                                                                                                                                                    }
                                                                                                                                                                                                    if (Main.netMode == 2)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        NetMessage.SendData(51, -1, whoAmI, "", b31, b32, 0f, 0f, 0);
                                                                                                                                                                                                        return;
                                                                                                                                                                                                    }
                                                                                                                                                                                                }
                                                                                                                                                                                            }
                                                                                                                                                                                            else
                                                                                                                                                                                            {
                                                                                                                                                                                                if (b == 52)
                                                                                                                                                                                                {
                                                                                                                                                                                                    byte number = readBuffer[num];
                                                                                                                                                                                                    num++;
                                                                                                                                                                                                    byte b33 = readBuffer[num];
                                                                                                                                                                                                    num++;
                                                                                                                                                                                                    int num108 = BitConverter.ToInt32(readBuffer, num);
                                                                                                                                                                                                    num += 4;
                                                                                                                                                                                                    int num109 = BitConverter.ToInt32(readBuffer, num);
                                                                                                                                                                                                    num += 4;
                                                                                                                                                                                                    if (b33 == 1)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        Chest.Unlock(num108, num109);
                                                                                                                                                                                                        if (Main.netMode == 2)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            NetMessage.SendData(52, -1, whoAmI, "", number, b33, num108, num109, 0);
                                                                                                                                                                                                            NetMessage.SendTileSquare(-1, num108, num109, 2);
                                                                                                                                                                                                            return;
                                                                                                                                                                                                        }
                                                                                                                                                                                                    }
                                                                                                                                                                                                }
                                                                                                                                                                                                else
                                                                                                                                                                                                {
                                                                                                                                                                                                    if (b == 53)
                                                                                                                                                                                                    {
                                                                                                                                                                                                        short num110 = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                                                                                        num += 2;
                                                                                                                                                                                                        byte type5 = readBuffer[num];
                                                                                                                                                                                                        num++;
                                                                                                                                                                                                        short time = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                                                                                        num += 2;
                                                                                                                                                                                                        Main.npc[num110].AddBuff(type5, time, true);
                                                                                                                                                                                                        if (Main.netMode == 2)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            NetMessage.SendData(54, -1, -1, "", num110, 0f, 0f, 0f, 0);
                                                                                                                                                                                                            return;
                                                                                                                                                                                                        }
                                                                                                                                                                                                    }
                                                                                                                                                                                                    else
                                                                                                                                                                                                    {
                                                                                                                                                                                                        if (b == 54)
                                                                                                                                                                                                        {
                                                                                                                                                                                                            if (Main.netMode == 1)
                                                                                                                                                                                                            {
                                                                                                                                                                                                                short num111 = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                                                                                                num += 2;
                                                                                                                                                                                                                for (int num112 = 0; num112 < 5; num112++)
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    Main.npc[num111].buffType[num112] = readBuffer[num];
                                                                                                                                                                                                                    num++;
                                                                                                                                                                                                                    Main.npc[num111].buffTime[num112] = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                                                                                                    num += 2;
                                                                                                                                                                                                                }
                                                                                                                                                                                                                return;
                                                                                                                                                                                                            }
                                                                                                                                                                                                        }
                                                                                                                                                                                                        else
                                                                                                                                                                                                        {
                                                                                                                                                                                                            if (b == 55)
                                                                                                                                                                                                            {
                                                                                                                                                                                                                byte b34 = readBuffer[num];
                                                                                                                                                                                                                num++;
                                                                                                                                                                                                                byte b35 = readBuffer[num];
                                                                                                                                                                                                                num++;
                                                                                                                                                                                                                short num113 = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                                                                                                num += 2;
                                                                                                                                                                                                                if (Main.netMode == 1 && b34 == Main.myPlayer)
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    Main.player[b34].AddBuff(b35, num113, true);
                                                                                                                                                                                                                    return;
                                                                                                                                                                                                                }
                                                                                                                                                                                                                if (Main.netMode == 2)
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    NetMessage.SendData(55, b34, -1, "", b34, b35, num113, 0f, 0);
                                                                                                                                                                                                                    return;
                                                                                                                                                                                                                }
                                                                                                                                                                                                            }
                                                                                                                                                                                                            else
                                                                                                                                                                                                            {
                                                                                                                                                                                                                if (b == 56)
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    if (Main.netMode == 1)
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        short num114 = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                                                                                                        num += 2;
                                                                                                                                                                                                                        string string8 = Encoding.ASCII.GetString(readBuffer, num, length - num + start);
                                                                                                                                                                                                                        Main.chrName[num114] = string8;
                                                                                                                                                                                                                        return;
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                }
                                                                                                                                                                                                                else
                                                                                                                                                                                                                {
                                                                                                                                                                                                                    if (b == 57)
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        if (Main.netMode == 1)
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            WorldGen.tGood = readBuffer[num];
                                                                                                                                                                                                                            num++;
                                                                                                                                                                                                                            WorldGen.tEvil = readBuffer[num];
                                                                                                                                                                                                                            return;
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                    else
                                                                                                                                                                                                                    {
                                                                                                                                                                                                                        if (b == 58)
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            byte b36 = readBuffer[num];
                                                                                                                                                                                                                            if (Main.netMode == 2)
                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                b36 = (byte) whoAmI;
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                            num++;
                                                                                                                                                                                                                            float num115 = BitConverter.ToSingle(readBuffer, num);
                                                                                                                                                                                                                            num += 4;
                                                                                                                                                                                                                            if (Main.netMode == 2)
                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                NetMessage.SendData(58, -1, whoAmI, "", whoAmI, num115, 0f, 0f, 0);
                                                                                                                                                                                                                                return;
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                            Main.harpNote = num115;
                                                                                                                                                                                                                            int style = 26;
                                                                                                                                                                                                                            if (Main.player[b36].inventory[Main.player[b36].selectedItem].type == 507)
                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                style = 35;
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                            Main.PlaySound(2, (int) Main.player[b36].position.X, (int) Main.player[b36].position.Y, style);
                                                                                                                                                                                                                            return;
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                        else
                                                                                                                                                                                                                        {
                                                                                                                                                                                                                            if (b == 59)
                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                int num116 = BitConverter.ToInt32(readBuffer, num);
                                                                                                                                                                                                                                num += 4;
                                                                                                                                                                                                                                int num117 = BitConverter.ToInt32(readBuffer, num);
                                                                                                                                                                                                                                num += 4;
                                                                                                                                                                                                                                WorldGen.hitSwitch(num116, num117);
                                                                                                                                                                                                                                if (Main.netMode == 2)
                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                    NetMessage.SendData(59, -1, whoAmI, "", num116, num117, 0f, 0f, 0);
                                                                                                                                                                                                                                    return;
                                                                                                                                                                                                                                }
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                            else
                                                                                                                                                                                                                            {
                                                                                                                                                                                                                                if (b == 60)
                                                                                                                                                                                                                                {
                                                                                                                                                                                                                                    short num118 = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                                                                                                                    num += 2;
                                                                                                                                                                                                                                    short num119 = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                                                                                                                    num += 2;
                                                                                                                                                                                                                                    short num120 = BitConverter.ToInt16(readBuffer, num);
                                                                                                                                                                                                                                    num += 2;
                                                                                                                                                                                                                                    byte b37 = readBuffer[num];
                                                                                                                                                                                                                                    num++;
                                                                                                                                                                                                                                    bool homeless = false;
                                                                                                                                                                                                                                    if (b37 == 1)
                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                        homeless = true;
                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                    if (Main.netMode == 1)
                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                        Main.npc[num118].homeless = homeless;
                                                                                                                                                                                                                                        Main.npc[num118].homeTileX = num119;
                                                                                                                                                                                                                                        Main.npc[num118].homeTileY = num120;
                                                                                                                                                                                                                                        return;
                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                    if (b37 == 0)
                                                                                                                                                                                                                                    {
                                                                                                                                                                                                                                        WorldGen.kickOut(num118);
                                                                                                                                                                                                                                        return;
                                                                                                                                                                                                                                    }
                                                                                                                                                                                                                                    WorldGen.moveRoom(num119, num120, num118);
                                                                                                                                                                                                                                }
                                                                                                                                                                                                                            }
                                                                                                                                                                                                                        }
                                                                                                                                                                                                                    }
                                                                                                                                                                                                                }
                                                                                                                                                                                                            }
                                                                                                                                                                                                        }
                                                                                                                                                                                                    }
                                                                                                                                                                                                }
                                                                                                                                                                                            }
                                                                                                                                                                                        }
                                                                                                                                                                                    }
                                                                                                                                                                                }
                                                                                                                                                                            }
                                                                                                                                                                        }
                                                                                                                                                                    }
                                                                                                                                                                }
                                                                                                                                                            }
                                                                                                                                                        }
                                                                                                                                                    }
                                                                                                                                                }
                                                                                                                                            }
                                                                                                                                        }
                                                                                                                                    }
                                                                                                                                }
                                                                                                                            }
                                                                                                                        }
                                                                                                                    }
                                                                                                                }
                                                                                                            }
                                                                                                        }
                                                                                                    }
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}