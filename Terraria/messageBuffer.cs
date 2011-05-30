namespace Terraria
{
    using System;
    using System.Text;
    using System.IO;

    public class messageBuffer
    {
        public bool broadcast;
        public bool checkBytes;
        public int maxSpam;
        public int messageLength;
        public byte[] readBuffer = new byte[0xffff];
        public const int readBufferMax = 0xffff;
        public int spamCount;
        public int totalData;
        public int whoAmI;
        public byte[] writeBuffer = new byte[0xffff];
        public const int writeBufferMax = 0xffff;
        public bool writeLocked;

        public void GetData(int start, int length)
        {
            byte num50;
            int num51;
            int num52;
            byte num53;
            byte num58;
            int num59;
            int num60;
            int num61;
            int num158;
            int num159;
            int team;
            string str14;
            int num161;
            if (this.whoAmI < 9)
            {
                Netplay.serverSock[this.whoAmI].timeOut = 0;
            }
            else
            {
                Netplay.clientSock.timeOut = 0;
            }
            byte msgType = 0;
            int index = 0;
            index = start + 1;
            msgType = this.readBuffer[start];
            if ((Main.netMode == 1) && (Netplay.clientSock.statusMax > 0))
            {
                Netplay.clientSock.statusCount++;
            }
            if (Main.verboseNetplay)
            {
                for (int i = start; i < (start + length); i++)
                {
                }
                for (int j = start; j < (start + length); j++)
                {
                    byte num1 = this.readBuffer[j];
                }
            }
            if (((Main.netMode == 2) && (msgType != 0x26)) && (Netplay.serverSock[this.whoAmI].state == -1))
            {
                NetMessage.SendData(2, this.whoAmI, -1, "Incorrect password.", 0, 0f, 0f, 0f);
                return;
            }
            if ((msgType == 1) && (Main.netMode == 2))
            {
                if (Netplay.serverSock[this.whoAmI].state == 0)
                {
                    if (Encoding.ASCII.GetString(this.readBuffer, start + 1, length - 1) == ("Terraria" + Main.curRelease))
                    {
                        if ((Netplay.password == null) || (Netplay.password == ""))
                        {
                            Netplay.serverSock[this.whoAmI].state = 1;
                            NetMessage.SendData(3, this.whoAmI, -1, "", 0, 0f, 0f, 0f);
                            return;
                        }
                        Netplay.serverSock[this.whoAmI].state = -1;
                        NetMessage.SendData(0x25, this.whoAmI, -1, "", 0, 0f, 0f, 0f);
                        return;
                    }
                    NetMessage.SendData(2, this.whoAmI, -1, "You are not using the same version as this server.", 0, 0f, 0f, 0f);
                    return;
                }
                return;
            }
            if ((msgType == 2) && (Main.netMode == 1))
            {
                Netplay.disconnect = true;
                Main.statusText = Encoding.ASCII.GetString(this.readBuffer, start + 1, length - 1);
                return;
            }
            if ((msgType == 3) && (Main.netMode == 1))
            {
                if (Netplay.clientSock.state == 1)
                {
                    Netplay.clientSock.state = 2;
                }
                int num5 = this.readBuffer[start + 1];
                if (num5 != Main.myPlayer)
                {
                    Main.player[num5] = (Player) Main.player[Main.myPlayer].Clone();
                    Main.player[Main.myPlayer] = new Player();
                    Main.player[num5].whoAmi = num5;
                    Main.myPlayer = num5;
                }
                NetMessage.SendData(4, -1, -1, Main.player[Main.myPlayer].name, Main.myPlayer, 0f, 0f, 0f);
                NetMessage.SendData(0x10, -1, -1, "", Main.myPlayer, 0f, 0f, 0f);
                NetMessage.SendData(0x2a, -1, -1, "", Main.myPlayer, 0f, 0f, 0f);
                for (int k = 0; k < 0x2c; k++)
                {
                    NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].inventory[k].name, Main.myPlayer, (float) k, 0f, 0f);
                }
                NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[0].name, Main.myPlayer, 44f, 0f, 0f);
                NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[1].name, Main.myPlayer, 45f, 0f, 0f);
                NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[2].name, Main.myPlayer, 46f, 0f, 0f);
                NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[3].name, Main.myPlayer, 47f, 0f, 0f);
                NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[4].name, Main.myPlayer, 48f, 0f, 0f);
                NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[5].name, Main.myPlayer, 49f, 0f, 0f);
                NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[6].name, Main.myPlayer, 50f, 0f, 0f);
                NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[7].name, Main.myPlayer, 51f, 0f, 0f);
                NetMessage.SendData(6, -1, -1, "", 0, 0f, 0f, 0f);
                if (Netplay.clientSock.state == 2)
                {
                    Netplay.clientSock.state = 3;
                    return;
                }
                return;
            }
            switch (msgType)
            {
                case 9:
                    if (Main.netMode == 1)
                    {
                        int num25 = BitConverter.ToInt32(this.readBuffer, start + 1);
                        string str4 = Encoding.ASCII.GetString(this.readBuffer, start + 5, length - 5);
                        Netplay.clientSock.statusMax += num25;
                        Netplay.clientSock.statusText = str4;
                        return;
                    }
                    return;

                case 10:
                {
                    short number = BitConverter.ToInt16(this.readBuffer, start + 1);
                    int num27 = BitConverter.ToInt32(this.readBuffer, start + 3);
                    int num28 = BitConverter.ToInt32(this.readBuffer, start + 7);
                    index = start + 11;
                    for (int m = num27; m < (num27 + number); m++)
                    {
                        if (Main.tile[m, num28] == null)
                        {
                            Main.tile[m, num28] = new Tile();
                        }
                        byte num29 = this.readBuffer[index];
                        index++;
                        bool active = Main.tile[m, num28].active;
                        if ((num29 & 1) == 1)
                        {
                            Main.tile[m, num28].active = true;
                        }
                        else
                        {
                            Main.tile[m, num28].active = false;
                        }
                        if ((num29 & 2) == 2)
                        {
                            Main.tile[m, num28].lighted = true;
                        }
                        if ((num29 & 4) == 4)
                        {
                            Main.tile[m, num28].wall = 1;
                        }
                        else
                        {
                            Main.tile[m, num28].wall = 0;
                        }
                        if ((num29 & 8) == 8)
                        {
                            Main.tile[m, num28].liquid = 1;
                        }
                        else
                        {
                            Main.tile[m, num28].liquid = 0;
                        }
                        if (Main.tile[m, num28].active)
                        {
                            int type = Main.tile[m, num28].type;
                            Main.tile[m, num28].type = this.readBuffer[index];
                            index++;
                            if (Main.tileFrameImportant[Main.tile[m, num28].type])
                            {
                                Main.tile[m, num28].frameX = BitConverter.ToInt16(this.readBuffer, index);
                                index += 2;
                                Main.tile[m, num28].frameY = BitConverter.ToInt16(this.readBuffer, index);
                                index += 2;
                            }
                            else if (!active || (Main.tile[m, num28].type != type))
                            {
                                Main.tile[m, num28].frameX = -1;
                                Main.tile[m, num28].frameY = -1;
                            }
                        }
                        if (Main.tile[m, num28].wall > 0)
                        {
                            Main.tile[m, num28].wall = this.readBuffer[index];
                            index++;
                        }
                        if (Main.tile[m, num28].liquid > 0)
                        {
                            Main.tile[m, num28].liquid = this.readBuffer[index];
                            index++;
                            byte num32 = this.readBuffer[index];
                            index++;
                            if (num32 == 1)
                            {
                                Main.tile[m, num28].lava = true;
                            }
                            else
                            {
                                Main.tile[m, num28].lava = false;
                            }
                        }
                    }
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendData(msgType, -1, this.whoAmI, "", number, (float) num27, (float) num28, 0f);
                        return;
                    }
                    return;
                }
                case 8:
                    if (Main.netMode == 2)
                    {
                        int x = BitConverter.ToInt32(this.readBuffer, index);
                        index += 4;
                        int y = BitConverter.ToInt32(this.readBuffer, index);
                        index += 4;
                        bool flag2 = true;
                        if ((x == -1) || (y == -1))
                        {
                            flag2 = false;
                        }
                        else if ((x < 10) || (x > (Main.maxTilesX - 10)))
                        {
                            flag2 = false;
                        }
                        else if ((y < 10) || (y > (Main.maxTilesY - 10)))
                        {
                            flag2 = false;
                        }
                        int num16 = 0x546;
                        if (flag2)
                        {
                            num16 *= 2;
                        }
                        if (Netplay.serverSock[this.whoAmI].state == 2)
                        {
                            Netplay.serverSock[this.whoAmI].state = 3;
                        }
                        NetMessage.SendData(9, this.whoAmI, -1, "Receiving tile data", num16, 0f, 0f, 0f);
                        Netplay.serverSock[this.whoAmI].statusText2 = "is receiving tile data";
                        ServerSock sock1 = Netplay.serverSock[this.whoAmI];
                        sock1.statusMax += num16;
                        int sectionX = Netplay.GetSectionX(Main.spawnTileX);
                        int sectionY = Netplay.GetSectionY(Main.spawnTileY);
                        for (int n = sectionX - 2; n < (sectionX + 3); n++)
                        {
                            for (int num20 = sectionY - 1; num20 < (sectionY + 2); num20++)
                            {
                                NetMessage.SendSection(this.whoAmI, n, num20);
                            }
                        }
                        if (flag2)
                        {
                            x = Netplay.GetSectionX(x);
                            y = Netplay.GetSectionY(y);
                            for (int num21 = x - 2; num21 < (x + 3); num21++)
                            {
                                for (int num22 = y - 1; num22 < (y + 2); num22++)
                                {
                                    NetMessage.SendSection(this.whoAmI, num21, num22);
                                }
                            }
                            NetMessage.SendData(11, this.whoAmI, -1, "", x - 2, (float) (y - 1), (float) (x + 2), (float) (y + 1));
                        }
                        NetMessage.SendData(11, this.whoAmI, -1, "", sectionX - 2, (float) (sectionY - 1), (float) (sectionX + 2), (float) (sectionY + 1));
                        for (int num23 = 0; num23 < 200; num23++)
                        {
                            if (Main.item[num23].active)
                            {
                                NetMessage.SendData(0x15, this.whoAmI, -1, "", num23, 0f, 0f, 0f);
                                NetMessage.SendData(0x16, this.whoAmI, -1, "", num23, 0f, 0f, 0f);
                            }
                        }
                        for (int num24 = 0; num24 < 0x3e8; num24++)
                        {
                            if (Main.npc[num24].active)
                            {
                                NetMessage.SendData(0x17, this.whoAmI, -1, "", num24, 0f, 0f, 0f);
                            }
                        }
                        NetMessage.SendData(0x31, this.whoAmI, -1, "", 0, 0f, 0f, 0f);
                        return;
                    }
                    return;

                case 11:
                    if (Main.netMode == 1)
                    {
                        int startX = BitConverter.ToInt16(this.readBuffer, index);
                        index += 4;
                        int startY = BitConverter.ToInt16(this.readBuffer, index);
                        index += 4;
                        int endX = BitConverter.ToInt16(this.readBuffer, index);
                        index += 4;
                        int endY = BitConverter.ToInt16(this.readBuffer, index);
                        index += 4;
                        WorldGen.SectionTileFrame(startX, startY, endX, endY);
                        return;
                    }
                    return;

                case 12:
                {
                    int num37 = this.readBuffer[index];
                    index++;
                    Main.player[num37].SpawnX = BitConverter.ToInt32(this.readBuffer, index);
                    index += 4;
                    Main.player[num37].SpawnY = BitConverter.ToInt32(this.readBuffer, index);
                    index += 4;
                    Main.player[num37].Spawn();
                    if ((Main.netMode == 2) && (Netplay.serverSock[this.whoAmI].state >= 3))
                    {
                        NetMessage.buffer[this.whoAmI].broadcast = true;
                        NetMessage.SendData(12, -1, this.whoAmI, "", this.whoAmI, 0f, 0f, 0f);
                        if (Netplay.serverSock[this.whoAmI].state == 3)
                        {

                            Netplay.serverSock[this.whoAmI].state = 10;
                            NetMessage.greetPlayer(this.whoAmI);
                            NetMessage.syncPlayers();
                            return;
                        }
                    }
                    return;
                }
                case 13:
                {
                    int num38 = this.readBuffer[index];
                    if ((Main.netMode == 1) && !Main.player[num38].active)
                    {
                        NetMessage.SendData(15, -1, -1, "", 0, 0f, 0f, 0f);
                    }
                    index++;
                    int num39 = this.readBuffer[index];
                    index++;
                    int num40 = this.readBuffer[index];
                    index++;
                    float num41 = BitConverter.ToSingle(this.readBuffer, index);
                    index += 4;
                    float num42 = BitConverter.ToSingle(this.readBuffer, index);
                    index += 4;
                    float num43 = BitConverter.ToSingle(this.readBuffer, index);
                    index += 4;
                    float num44 = BitConverter.ToSingle(this.readBuffer, index);
                    index += 4;
                    Main.player[num38].selectedItem = num40;
                    Main.player[num38].position.X = num41;
                    Main.player[num38].position.Y = num42;
                    Main.player[num38].velocity.X = num43;
                    Main.player[num38].velocity.Y = num44;
                    Main.player[num38].oldVelocity = Main.player[num38].velocity;
                    Main.player[num38].fallStart = (int) (num42 / 16f);
                    Main.player[num38].controlUp = false;
                    Main.player[num38].controlDown = false;
                    Main.player[num38].controlLeft = false;
                    Main.player[num38].controlRight = false;
                    Main.player[num38].controlJump = false;
                    Main.player[num38].controlUseItem = false;
                    Main.player[num38].direction = -1;
                    if ((num39 & 1) == 1)
                    {
                        Main.player[num38].controlUp = true;
                    }
                    if ((num39 & 2) == 2)
                    {
                        Main.player[num38].controlDown = true;
                    }
                    if ((num39 & 4) == 4)
                    {
                        Main.player[num38].controlLeft = true;
                    }
                    if ((num39 & 8) == 8)
                    {
                        Main.player[num38].controlRight = true;
                    }
                    if ((num39 & 0x10) == 0x10)
                    {
                        Main.player[num38].controlJump = true;
                    }
                    if ((num39 & 0x20) == 0x20)
                    {
                        Main.player[num38].controlUseItem = true;
                    }
                    if ((num39 & 0x40) == 0x40)
                    {
                        Main.player[num38].direction = 1;
                    }
                    if ((Main.netMode == 2) && (Netplay.serverSock[this.whoAmI].state == 10))
                    {
                        NetMessage.SendData(13, -1, this.whoAmI, "", num38, 0f, 0f, 0f);
                        return;
                    }
                    return;
                }
                case 14:
                    if (Main.netMode == 1)
                    {
                        int num45 = this.readBuffer[index];
                        index++;
                        int num46 = this.readBuffer[index];
                        if (num46 == 1)
                        {
                            if (Main.player[num45].active)
                            {
                                Main.player[num45] = new Player();
                            }
                            Main.player[num45].active = true;
                            return;
                        }
                        Main.player[num45].active = false;
                        return;
                    }
                    return;

                case 15:
                    if (Main.netMode == 2)
                    {
                        NetMessage.syncPlayers();
                        return;
                    }
                    return;

                case 0x10:
                {
                    int whoAmI = this.readBuffer[index];
                    index++;
                    int num48 = BitConverter.ToInt16(this.readBuffer, index);
                    index += 2;
                    int num49 = BitConverter.ToInt16(this.readBuffer, index);
                    if (Main.netMode == 2)
                    {
                        whoAmI = this.whoAmI;
                    }
                    Main.player[whoAmI].statLife = num48;
                    Main.player[whoAmI].statLifeMax = num49;
                    if (Main.player[whoAmI].statLife <= 0)
                    {
                        Main.player[whoAmI].dead = true;
                    }
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendData(0x10, -1, this.whoAmI, "", whoAmI, 0f, 0f, 0f);
                        return;
                    }
                    return;
                }
                case 7:
                    if (Main.netMode == 1)
                    {
                        Main.time = BitConverter.ToInt32(this.readBuffer, index);
                        index += 4;
                        Main.dayTime = false;
                        if (this.readBuffer[index] == 1)
                        {
                            Main.dayTime = true;
                        }
                        index++;
                        Main.moonPhase = this.readBuffer[index];
                        index++;
                        int num13 = this.readBuffer[index];
                        index++;
                        if (num13 == 1)
                        {
                            Main.bloodMoon = true;
                        }
                        else
                        {
                            Main.bloodMoon = false;
                        }
                        Main.maxTilesX = BitConverter.ToInt32(this.readBuffer, index);
                        index += 4;
                        Main.maxTilesY = BitConverter.ToInt32(this.readBuffer, index);
                        index += 4;
                        Main.spawnTileX = BitConverter.ToInt32(this.readBuffer, index);
                        index += 4;
                        Main.spawnTileY = BitConverter.ToInt32(this.readBuffer, index);
                        index += 4;
                        Main.worldSurface = BitConverter.ToInt32(this.readBuffer, index);
                        index += 4;
                        Main.rockLayer = BitConverter.ToInt32(this.readBuffer, index);
                        index += 4;
                        Main.worldID = BitConverter.ToInt32(this.readBuffer, index);
                        index += 4;
                        Main.worldName = Encoding.ASCII.GetString(this.readBuffer, index, (length - index) + start);
                        if (Netplay.clientSock.state == 3)
                        {
                            Netplay.clientSock.state = 4;
                            return;
                        }
                    }
                    return;

                case 6:
                    if (Main.netMode == 2)
                    {
                        if (Netplay.serverSock[this.whoAmI].state == 1)
                        {
                            Netplay.serverSock[this.whoAmI].state = 2;
                        }
                        NetMessage.SendData(7, this.whoAmI, -1, "", 0, 0f, 0f, 0f);
                        return;
                    }
                    return;

                case 5:
                {
                    int num10 = this.readBuffer[start + 1];
                    if (Main.netMode == 2)
                    {
                        num10 = this.whoAmI;
                    }
                    int num11 = this.readBuffer[start + 2];
                    int num12 = this.readBuffer[start + 3];
                    string itemName = Encoding.ASCII.GetString(this.readBuffer, start + 4, length - 4);
                    if (num11 < 0x2c)
                    {
                        Main.player[num10].inventory[num11] = new Item();
                        Main.player[num10].inventory[num11].SetDefaults(itemName);
                        Main.player[num10].inventory[num11].stack = num12;
                    }
                    else
                    {
                        Main.player[num10].armor[num11 - 0x2c] = new Item();
                        Main.player[num10].armor[num11 - 0x2c].SetDefaults(itemName);
                        Main.player[num10].armor[num11 - 0x2c].stack = num12;
                    }
                    if ((Main.netMode == 2) && (num10 == this.whoAmI))
                    {
                        NetMessage.SendData(5, -1, this.whoAmI, itemName, num10, (float) num11, 0f, 0f);
                        return;
                    }
                    return;
                }
                case 4:
                {
                    bool flag = false;
                    int num7 = this.readBuffer[start + 1];
                    int num8 = this.readBuffer[start + 2];
                    if (Main.netMode == 2)
                    {
                        num7 = this.whoAmI;
                    }
                    Main.player[num7].hair = num8;
                    Main.player[num7].whoAmi = num7;
                    index += 2;
                    Main.player[num7].hairColor.R = this.readBuffer[index];
                    index++;
                    Main.player[num7].hairColor.G = this.readBuffer[index];
                    index++;
                    Main.player[num7].hairColor.B = this.readBuffer[index];
                    index++;
                    Main.player[num7].skinColor.R = this.readBuffer[index];
                    index++;
                    Main.player[num7].skinColor.G = this.readBuffer[index];
                    index++;
                    Main.player[num7].skinColor.B = this.readBuffer[index];
                    index++;
                    Main.player[num7].eyeColor.R = this.readBuffer[index];
                    index++;
                    Main.player[num7].eyeColor.G = this.readBuffer[index];
                    index++;
                    Main.player[num7].eyeColor.B = this.readBuffer[index];
                    index++;
                    Main.player[num7].shirtColor.R = this.readBuffer[index];
                    index++;
                    Main.player[num7].shirtColor.G = this.readBuffer[index];
                    index++;
                    Main.player[num7].shirtColor.B = this.readBuffer[index];
                    index++;
                    Main.player[num7].underShirtColor.R = this.readBuffer[index];
                    index++;
                    Main.player[num7].underShirtColor.G = this.readBuffer[index];
                    index++;
                    Main.player[num7].underShirtColor.B = this.readBuffer[index];
                    index++;
                    Main.player[num7].pantsColor.R = this.readBuffer[index];
                    index++;
                    Main.player[num7].pantsColor.G = this.readBuffer[index];
                    index++;
                    Main.player[num7].pantsColor.B = this.readBuffer[index];
                    index++;
                    Main.player[num7].shoeColor.R = this.readBuffer[index];
                    index++;
                    Main.player[num7].shoeColor.G = this.readBuffer[index];
                    index++;
                    Main.player[num7].shoeColor.B = this.readBuffer[index];
                    index++;
                    string text = Encoding.ASCII.GetString(this.readBuffer, index, (length - index) + start);
                    Main.player[num7].name = text;
                    if (Main.netMode == 2)
                    {
                        if (Netplay.serverSock[this.whoAmI].state < 10)
                        {
                            for (int num9 = 0; num9 < 8; num9++)
                            {
                                if (((num9 != num7) && (text == Main.player[num9].name)) && Netplay.serverSock[num9].active)
                                {
                                    flag = true;
                                }
                            }
                        }
                        if (flag)
                        {
                            NetMessage.SendData(2, this.whoAmI, -1, text + " is already on this server.", 0, 0f, 0f, 0f);
                            return;
                        }
                        Netplay.serverSock[this.whoAmI].oldName = text;
                        Netplay.serverSock[this.whoAmI].name = text;
                        NetMessage.SendData(4, -1, this.whoAmI, text, num7, 0f, 0f, 0f);
                        return;
                    }
                    return;
                }
                default:
                {
                    if (msgType != 0x11)
                    {
                        if (msgType == 0x12)
                        {
                            if (Main.netMode == 1)
                            {
                                byte num54 = this.readBuffer[index];
                                index++;
                                int num55 = BitConverter.ToInt32(this.readBuffer, index);
                                index += 4;
                                short num56 = BitConverter.ToInt16(this.readBuffer, index);
                                index += 2;
                                short num57 = BitConverter.ToInt16(this.readBuffer, index);
                                index += 2;
                                if (num54 == 1)
                                {
                                    Main.dayTime = true;
                                }
                                else
                                {
                                    Main.dayTime = false;
                                }
                                Main.time = num55;
                                Main.sunModY = num56;
                                Main.moonModY = num57;
                                if (Main.netMode == 2)
                                {
                                    NetMessage.SendData(0x12, -1, this.whoAmI, "", 0, 0f, 0f, 0f);
                                    return;
                                }
                            }
                            return;
                        }
                        if (msgType != 0x13)
                        {
                            switch (msgType)
                            {
                                case 20:
                                {
                                    short num63 = BitConverter.ToInt16(this.readBuffer, start + 1);
                                    int num64 = BitConverter.ToInt32(this.readBuffer, start + 3);
                                    int num65 = BitConverter.ToInt32(this.readBuffer, start + 7);
                                    index = start + 11;
                                    for (int num67 = num64; num67 < (num64 + num63); num67++)
                                    {
                                        for (int num68 = num65; num68 < (num65 + num63); num68++)
                                        {
                                            if (Main.tile[num67, num68] == null)
                                            {
                                                Main.tile[num67, num68] = new Tile();
                                            }
                                            byte num66 = this.readBuffer[index];
                                            index++;
                                            bool flag5 = Main.tile[num67, num68].active;
                                            if ((num66 & 1) == 1)
                                            {
                                                Main.tile[num67, num68].active = true;
                                            }
                                            else
                                            {
                                                Main.tile[num67, num68].active = false;
                                            }
                                            if ((num66 & 2) == 2)
                                            {
                                                Main.tile[num67, num68].lighted = true;
                                            }
                                            if ((num66 & 4) == 4)
                                            {
                                                Main.tile[num67, num68].wall = 1;
                                            }
                                            else
                                            {
                                                Main.tile[num67, num68].wall = 0;
                                            }
                                            if ((num66 & 8) == 8)
                                            {
                                                Main.tile[num67, num68].liquid = 1;
                                            }
                                            else
                                            {
                                                Main.tile[num67, num68].liquid = 0;
                                            }
                                            if (Main.tile[num67, num68].active)
                                            {
                                                int num69 = Main.tile[num67, num68].type;
                                                Main.tile[num67, num68].type = this.readBuffer[index];
                                                index++;
                                                if (Main.tileFrameImportant[Main.tile[num67, num68].type])
                                                {
                                                    Main.tile[num67, num68].frameX = BitConverter.ToInt16(this.readBuffer, index);
                                                    index += 2;
                                                    Main.tile[num67, num68].frameY = BitConverter.ToInt16(this.readBuffer, index);
                                                    index += 2;
                                                }
                                                else if (!flag5 || (Main.tile[num67, num68].type != num69))
                                                {
                                                    Main.tile[num67, num68].frameX = -1;
                                                    Main.tile[num67, num68].frameY = -1;
                                                }
                                            }
                                            if (Main.tile[num67, num68].wall > 0)
                                            {
                                                Main.tile[num67, num68].wall = this.readBuffer[index];
                                                index++;
                                            }
                                            if (Main.tile[num67, num68].liquid > 0)
                                            {
                                                Main.tile[num67, num68].liquid = this.readBuffer[index];
                                                index++;
                                                byte num70 = this.readBuffer[index];
                                                index++;
                                                if (num70 == 1)
                                                {
                                                    Main.tile[num67, num68].lava = true;
                                                }
                                                else
                                                {
                                                    Main.tile[num67, num68].lava = false;
                                                }
                                            }
                                        }
                                    }
                                    WorldGen.RangeFrame(num64, num65, num64 + num63, num65 + num63);
                                    if (Main.netMode == 2)
                                    {
                                        NetMessage.SendData(msgType, -1, this.whoAmI, "", num63, (float) num64, (float) num65, 0f);
                                        return;
                                    }
                                    return;
                                }
                                case 0x15:
                                {
                                    short num71 = BitConverter.ToInt16(this.readBuffer, index);
                                    index += 2;
                                    float num72 = BitConverter.ToSingle(this.readBuffer, index);
                                    index += 4;
                                    float num73 = BitConverter.ToSingle(this.readBuffer, index);
                                    index += 4;
                                    float num74 = BitConverter.ToSingle(this.readBuffer, index);
                                    index += 4;
                                    float num75 = BitConverter.ToSingle(this.readBuffer, index);
                                    index += 4;
                                    byte stack = this.readBuffer[index];
                                    index++;
                                    string str5 = Encoding.ASCII.GetString(this.readBuffer, index, (length - index) + start);
                                    if (Main.netMode == 1)
                                    {
                                        if (str5 == "0")
                                        {
                                            Main.item[num71].active = false;
                                            return;
                                        }
                                        Main.item[num71].SetDefaults(str5);
                                        Main.item[num71].stack = stack;
                                        Main.item[num71].position.X = num72;
                                        Main.item[num71].position.Y = num73;
                                        Main.item[num71].velocity.X = num74;
                                        Main.item[num71].velocity.Y = num75;
                                        Main.item[num71].active = true;
                                        Main.item[num71].wet = Collision.WetCollision(Main.item[num71].position, Main.item[num71].width, Main.item[num71].height);
                                        return;
                                    }
                                    if (str5 != "0")
                                    {
                                        bool flag6 = false;
                                        if (num71 == 200)
                                        {
                                            flag6 = true;
                                        }
                                        if (flag6)
                                        {
                                            Item item = new Item();
                                            item.SetDefaults(str5);
                                            num71 = (short) Item.NewItem((int) num72, (int) num73, item.width, item.height, item.type, stack, true);
                                        }
                                        Main.item[num71].SetDefaults(str5);
                                        Main.item[num71].stack = stack;
                                        Main.item[num71].position.X = num72;
                                        Main.item[num71].position.Y = num73;
                                        Main.item[num71].velocity.X = num74;
                                        Main.item[num71].velocity.Y = num75;
                                        Main.item[num71].active = true;
                                        Main.item[num71].owner = Main.myPlayer;
                                        if (flag6)
                                        {
                                            NetMessage.SendData(0x15, -1, -1, "", num71, 0f, 0f, 0f);
                                            Main.item[num71].ownIgnore = this.whoAmI;
                                            Main.item[num71].ownTime = 100;
                                            Main.item[num71].FindOwner(num71);
                                            return;
                                        }
                                        NetMessage.SendData(0x15, -1, this.whoAmI, "", num71, 0f, 0f, 0f);
                                        return;
                                    }
                                    if (num71 < 200)
                                    {
                                        Main.item[num71].active = false;
                                        NetMessage.SendData(0x15, -1, -1, "", num71, 0f, 0f, 0f);
                                        return;
                                    }
                                    return;
                                }
                                case 0x16:
                                {
                                    short num77 = BitConverter.ToInt16(this.readBuffer, index);
                                    index += 2;
                                    byte num78 = this.readBuffer[index];
                                    Main.item[num77].owner = num78;
                                    if (num78 == Main.myPlayer)
                                    {
                                        Main.item[num77].keepTime = 15;
                                    }
                                    else
                                    {
                                        Main.item[num77].keepTime = 0;
                                    }
                                    if (Main.netMode == 2)
                                    {
                                        Main.item[num77].owner = 8;
                                        Main.item[num77].keepTime = 15;
                                        NetMessage.SendData(0x16, -1, -1, "", num77, 0f, 0f, 0f);
                                        return;
                                    }
                                    return;
                                }
                                case 0x17:
                                {
                                    short num79 = BitConverter.ToInt16(this.readBuffer, index);
                                    index += 2;
                                    float num80 = BitConverter.ToSingle(this.readBuffer, index);
                                    index += 4;
                                    float num81 = BitConverter.ToSingle(this.readBuffer, index);
                                    index += 4;
                                    float num82 = BitConverter.ToSingle(this.readBuffer, index);
                                    index += 4;
                                    float num83 = BitConverter.ToSingle(this.readBuffer, index);
                                    index += 4;
                                    int num84 = BitConverter.ToInt16(this.readBuffer, index);
                                    index += 2;
                                    int num85 = this.readBuffer[index] - 1;
                                    index++;
                                    byte num172 = this.readBuffer[index];
                                    index++;
                                    int num86 = BitConverter.ToInt16(this.readBuffer, index);
                                    index += 2;
                                    float[] numArray = new float[NPC.maxAI];
                                    for (int num87 = 0; num87 < NPC.maxAI; num87++)
                                    {
                                        numArray[num87] = BitConverter.ToSingle(this.readBuffer, index);
                                        index += 4;
                                    }
                                    string name = Encoding.ASCII.GetString(this.readBuffer, index, (length - index) + start);
                                    if (!Main.npc[num79].active || (Main.npc[num79].name != name))
                                    {
                                        Main.npc[num79].active = true;
                                        Main.npc[num79].SetDefaults(name);
                                    }
                                    Main.npc[num79].position.X = num80;
                                    Main.npc[num79].position.Y = num81;
                                    Main.npc[num79].velocity.X = num82;
                                    Main.npc[num79].velocity.Y = num83;
                                    Main.npc[num79].target = num84;
                                    Main.npc[num79].direction = num85;
                                    Main.npc[num79].life = num86;
                                    if (num86 <= 0)
                                    {
                                        Main.npc[num79].active = false;
                                    }
                                    for (int num88 = 0; num88 < NPC.maxAI; num88++)
                                    {
                                        Main.npc[num79].ai[num88] = numArray[num88];
                                    }
                                    return;
                                }
                                case 0x18:
                                {
                                    short num89 = BitConverter.ToInt16(this.readBuffer, index);
                                    index += 2;
                                    byte num90 = this.readBuffer[index];
                                    Main.npc[num89].StrikeNPC(Main.player[num90].inventory[Main.player[num90].selectedItem].damage, Main.player[num90].inventory[Main.player[num90].selectedItem].knockBack, Main.player[num90].direction);
                                    if (Main.netMode == 2)
                                    {
                                        NetMessage.SendData(0x18, -1, this.whoAmI, "", num89, (float) num90, 0f, 0f);
                                        NetMessage.SendData(0x17, -1, -1, "", num89, 0f, 0f, 0f);
                                        return;
                                    }
                                    return;
                                }
                                case 0x19:
                                {
                                    int num91 = this.readBuffer[start + 1];
                                    if (Main.netMode == 2)
                                    {
                                        num91 = this.whoAmI;
                                    }
                                    byte r = this.readBuffer[start + 2];
                                    byte g = this.readBuffer[start + 3];
                                    byte b = this.readBuffer[start + 4];
                                    string str7 = Encoding.ASCII.GetString(this.readBuffer, start + 5, length - 5);
                                    if (Main.netMode == 1)
                                    {
                                        string newText = str7;
                                        if (num91 < 8)
                                        {
                                            newText = "<" + Main.player[num91].name + "> " + str7;
                                            Main.player[num91].chatText = str7;
                                            Main.player[num91].chatShowTime = Main.chatLength / 2;
                                        }
                                        Main.NewText(newText, r, g, b);
                                        return;
                                    }
                                    if (Main.netMode == 2)
                                    {
                                        string str9 = str7.ToLower();
                                        int x = (int)Main.player[this.whoAmI].position.X;
                                        int y = (int)Main.player[this.whoAmI].position.Y;
                                        //TODO: Clean this shit up
                                        if (str9 == "/hardcore")
                                        {
                                            if (!ShankShock.IsAdmin(this.whoAmI))
                                            {
                                                ShankShock.SendMessage(this.whoAmI, "You aren't allowed to do that.");
                                                return;
                                            }

                                            for (int i = 0; i <= 2; i++)
                                            {
                                                ShankShock.NewNPC(i, x, y, this.whoAmI);
                                            }
                                            Main.startInv();
                                            ShankShock.Broadcast(ShankShock.FindPlayer(this.whoAmI) + " has spawned all 3 bosses and started an invasion!!!");
                                            return;
                                        }
                                        if (str9 == "/off")
                                        {
                                            if (!ShankShock.IsAdmin(this.whoAmI))
                                            {
                                                ShankShock.SendMessage(this.whoAmI, "You aren't allowed to do that.");
                                                return;
                                            }
                                            Netplay.disconnect = true;
                                            return;
                                        }
                                        if (str9 == "/skeletron")
                                        {
                                            if (!ShankShock.IsAdmin(this.whoAmI))
                                            {
                                                ShankShock.SendMessage(this.whoAmI, "You aren't allowed to do that.");
                                                return;
                                            }
                                            ShankShock.NewNPC((int)ShankShock.NPCList.SKELETRON, x, y, this.whoAmI);
                                            ShankShock.Broadcast(ShankShock.FindPlayer(this.whoAmI) + " has spawned Skeletor!");
                                            return;
                                        }
                                        if (str9 == "/reload")
                                        {
                                            if (!ShankShock.IsAdmin(this.whoAmI))
                                            {
                                                ShankShock.SendMessage(this.whoAmI, "You aren't allowed to do that.");
                                                return;
                                            }

                                            ShankShock.SendMessage(this.whoAmI, "Reloaded the server configuration files.");
                                            return;
                                        }
                                        if (str9 == "/bloodmoon")
                                        {
                                            if (!ShankShock.IsAdmin(this.whoAmI))
                                            {
                                                ShankShock.SendMessage(this.whoAmI, "You aren't allowed to do that.");
                                                return;
                                            }
                                            ShankShock.Broadcast(ShankShock.FindPlayer(this.whoAmI) + " turned on blood moon.");
                                            Main.bloodMoon = true;
                                            Main.time = 0;
                                            Main.dayTime = false;
                                            Main.UpdateT();
                                            NetMessage.syncPlayers();
                                            
                                            return;
                                        }
                                        if (str9 == "/dropmeteor")
                                        {
                                            if (!ShankShock.IsAdmin(this.whoAmI))
                                            {
                                                ShankShock.SendMessage(this.whoAmI, "You aren't allowed to do that.");
                                                return;
                                            }
                                            WorldGen.spawnMeteor = false;
                                            WorldGen.dropMeteor();
                                            return;
                                        }
                                        if (str9 == "/star")
                                        {
                                            if (!ShankShock.IsAdmin(this.whoAmI))
                                            {
                                                ShankShock.SendMessage(this.whoAmI, "You aren't allowed to do that.");
                                                return;
                                            }
                                            Star.SpawnStars();
                                            Star.UpdateStars();
                                            return;
                                        }
                                        if (str9 == "/eye")
                                        {
                                            if (!ShankShock.IsAdmin(this.whoAmI))
                                            {
                                                ShankShock.SendMessage(this.whoAmI, "You aren't allowed to do that.");
                                                return;
                                            }
                                            ShankShock.NewNPC((int)ShankShock.NPCList.EYE, x, y, this.whoAmI);
                                            ShankShock.Broadcast(ShankShock.FindPlayer(this.whoAmI) + " has spawned an eye!");
                                            return;
                                        }
                                        if (str9 == "/invade")
                                        {
                                            if (!ShankShock.IsAdmin(this.whoAmI))
                                            {
                                                ShankShock.SendMessage(this.whoAmI, "You aren't allowed to do that.");
                                                return;
                                            }
                                            Main.startInv();
                                            return;
                                        }
                                        if (str9 == ("/help"))
                                        {
                                            ShankShock.SendMessage(this.whoAmI, "TShock Commands:");
                                            ShankShock.SendMessage(this.whoAmI, "/who - Who's online?");
                                            ShankShock.SendMessage(this.whoAmI, "/me - Talk in 3rd person");
                                            ShankShock.SendMessage(this.whoAmI, "/p - Talk in party chat");
                                            if (ShankShock.IsAdmin(this.whoAmI))
                                            {
                                                ShankShock.SendMessage(this.whoAmI, "/kick | /ban | /eater | /hardcore");
                                                ShankShock.SendMessage(this.whoAmI, "/invade | /dropmeteor | /bloodmoon | /eye");
                                            }
                                            return;

                                        }
                                        if (str9 == "/spawn")
                                        {
                                            Main.player[this.whoAmI].position.X = Main.player[this.whoAmI].SpawnX;
                                            Main.player[this.whoAmI].position.Y = Main.player[this.whoAmI].SpawnY;
                                            Main.player[this.whoAmI].Spawn();
                                            return;
                                        }
                                        if (str9 == "/kc")
                                        {
                                            if (!ShankShock.infinateInvasion) { return; }
                                            ShankShock.SendMessage(this.whoAmI, "Goblin kill count to date: " + ShankShock.killCount, new float[] { 255, 0, 0 });
                                            return;
                                        }
                                        if (str9 == "/eater")
                                        {
                                            if (!ShankShock.IsAdmin(this.whoAmI))
                                            {
                                                ShankShock.SendMessage(this.whoAmI, "You aren't allowed to do that.");
                                                return;
                                            }
                                            ShankShock.NewNPC((int)ShankShock.NPCList.WORLD_EATER, x, y, this.whoAmI);
                                            ShankShock.Broadcast(ShankShock.FindPlayer(this.whoAmI) + " has spawned an eater of worlds!");
                                            return;
                                        }
                                        if (str9.Length > 5 && str9.Substring(0, 5) == "/kick")
                                        {
                                            if (!ShankShock.IsAdmin(this.whoAmI)){
                                                ShankShock.SendMessage(this.whoAmI, "You aren't allowed to do that.");
                                                return;
                                            }
                                            string plStr = str9.Remove(0, 5).Trim();
                                            if (!(ShankShock.FindPlayer(plStr) == -1 || plStr == ""))
                                            {
                                                ShankShock.Kick(ShankShock.FindPlayer(plStr));
                                                ShankShock.Broadcast(plStr + " has been kicked.");
                                                return;
                                            }
                                            ShankShock.SendMessage(this.whoAmI, "Player doesn't exist.");
                                            return;
                                        }
                                        if (str9.Length > 4 && str9.Substring(0, 4) == "/ban"){
                                            if (!ShankShock.IsAdmin(this.whoAmI)){
                                                ShankShock.SendMessage(this.whoAmI, "You aren't allowed to do that.");
                                                return;
                                            }
                                            string plStr = str9.Remove(0, 4).Trim();
                                            if (!(ShankShock.FindPlayer(plStr) == -1 || plStr == ""))
                                            {
                                                ShankShock._writeban(ShankShock.FindPlayer(plStr));
                                                ShankShock.Kick(ShankShock.FindPlayer(plStr));
                                                ShankShock.Broadcast(plStr + " has been banned.");
                                                return;
                                            }
                                        }
                                        if (str9 == "/playing" || str9 == "/who")
                                        {
                                            string str10 = "";
                                            for (int num95 = 0; num95 < 8; num95++)
                                            {
                                                if (Main.player[num95].active)
                                                {
                                                    if (str10 == "")
                                                    {
                                                        str10 = str10 + Main.player[num95].name;
                                                    }
                                                    else
                                                    {
                                                        str10 = str10 + ", " + Main.player[num95].name;
                                                    }
                                                }
                                            }
                                            NetMessage.SendData(0x19, this.whoAmI, -1, "Current players: " + str10 + ".", 8, 255f, 240f, 20f);
                                            return;
                                        }
                                        if ((str9.Length >= 4) && (str9.Substring(0, 4) == "/me "))
                                        {
                                            NetMessage.SendData(0x19, -1, -1, "*" + Main.player[this.whoAmI].name + " " + str7.Substring(4), 8, 200f, 100f, 0f);
                                            return;
                                        }
                                        if ((str9.Length >= 3) && (str9.Substring(0, 3) == "/p "))
                                        {
                                            if (Main.player[this.whoAmI].team != 0)
                                            {
                                                for (int num96 = 0; num96 < 8; num96++)
                                                {
                                                    if (Main.player[num96].team == Main.player[this.whoAmI].team)
                                                    {
                                                        NetMessage.SendData(0x19, num96, -1, str7.Substring(3), num91, (float) Main.teamColor[Main.player[this.whoAmI].team].R, (float) Main.teamColor[Main.player[this.whoAmI].team].G, (float) Main.teamColor[Main.player[this.whoAmI].team].B);
                                                    }
                                                }
                                                return;
                                            }
                                            NetMessage.SendData(0x19, this.whoAmI, -1, "You are not in a party!", 8, 255f, 240f, 20f);
                                            return;
                                        }
                                        NetMessage.SendData(0x19, -1, -1, str7, num91, (float) r, (float) g, (float) b);
                                        return;
                                    }
                                    return;
                                }
                                case 0x1a:
                                {
                                    byte num97 = this.readBuffer[index];
                                    index++;
                                    int hitDirection = this.readBuffer[index] - 1;
                                    index++;
                                    short damage = BitConverter.ToInt16(this.readBuffer, index);
                                    index += 2;
                                    byte num100 = this.readBuffer[index];
                                    bool pvp = false;
                                    if (num100 != 0 || ShankShock.permaPvp)
                                    {
                                        pvp = true;
                                    }
                                    Main.player[num97].Hurt(damage, hitDirection, pvp, true);
                                    if (Main.netMode == 2)
                                    {
                                        NetMessage.SendData(0x1a, -1, this.whoAmI, "", num97, (float) hitDirection, (float) damage, (float) num100);
                                        return;
                                    }
                                    return;
                                }
                                case 0x1b:
                                {
                                    short num101 = BitConverter.ToInt16(this.readBuffer, index);
                                    index += 2;
                                    float num102 = BitConverter.ToSingle(this.readBuffer, index);
                                    index += 4;
                                    float num103 = BitConverter.ToSingle(this.readBuffer, index);
                                    index += 4;
                                    float num104 = BitConverter.ToSingle(this.readBuffer, index);
                                    index += 4;
                                    float num105 = BitConverter.ToSingle(this.readBuffer, index);
                                    index += 4;
                                    float num106 = BitConverter.ToSingle(this.readBuffer, index);
                                    index += 4;
                                    short num107 = BitConverter.ToInt16(this.readBuffer, index);
                                    index += 2;
                                    byte num108 = this.readBuffer[index];
                                    index++;
                                    byte num109 = this.readBuffer[index];
                                    index++;
                                    float[] numArray2 = new float[Projectile.maxAI];
                                    for (int num110 = 0; num110 < Projectile.maxAI; num110++)
                                    {
                                        numArray2[num110] = BitConverter.ToSingle(this.readBuffer, index);
                                        index += 4;
                                    }
                                    int num111 = 0x3e8;
                                    for (int num112 = 0; num112 < 0x3e8; num112++)
                                    {
                                        if (((Main.projectile[num112].owner == num108) && (Main.projectile[num112].identity == num101)) && Main.projectile[num112].active)
                                        {
                                            num111 = num112;
                                            break;
                                        }
                                    }
                                    if (num111 == 0x3e8)
                                    {
                                        for (int num113 = 0; num113 < 0x3e8; num113++)
                                        {
                                            if (!Main.projectile[num113].active)
                                            {
                                                num111 = num113;
                                                break;
                                            }
                                        }
                                    }
                                    if (!Main.projectile[num111].active || (Main.projectile[num111].type != num109))
                                    {
                                        Main.projectile[num111].SetDefaults(num109);
                                    }
                                    Main.projectile[num111].identity = num101;
                                    Main.projectile[num111].position.X = num102;
                                    Main.projectile[num111].position.Y = num103;
                                    Main.projectile[num111].velocity.X = num104;
                                    Main.projectile[num111].velocity.Y = num105;
                                    Main.projectile[num111].damage = num107;
                                    Main.projectile[num111].type = num109;
                                    Main.projectile[num111].owner = num108;
                                    Main.projectile[num111].knockBack = num106;
                                    for (int num114 = 0; num114 < Projectile.maxAI; num114++)
                                    {
                                        Main.projectile[num111].ai[num114] = numArray2[num114];
                                    }
                                    if (Main.netMode == 2)
                                    {
                                        NetMessage.SendData(0x1b, -1, this.whoAmI, "", num111, 0f, 0f, 0f);
                                    }
                                    return;
                                }
                                case 0x1c:
                                {
                                    short num115 = BitConverter.ToInt16(this.readBuffer, index);
                                    index += 2;
                                    short num116 = BitConverter.ToInt16(this.readBuffer, index);
                                    index += 2;
                                    float knockBack = BitConverter.ToSingle(this.readBuffer, index);
                                    index += 4;
                                    int num118 = this.readBuffer[index] - 1;
                                    if (num116 >= 0)
                                    {
                                        Main.npc[num115].StrikeNPC(num116, knockBack, num118);
                                    }
                                    else
                                    {
                                        Main.npc[num115].life = 0;
                                        Main.npc[num115].HitEffect(0, 10.0);
                                        Main.npc[num115].active = false;
                                    }
                                    if (Main.netMode == 2)
                                    {
                                        NetMessage.SendData(0x1c, -1, this.whoAmI, "", num115, (float) num116, knockBack, (float) num118);
                                        NetMessage.SendData(0x17, -1, -1, "", num115, 0f, 0f, 0f);
                                        return;
                                    }
                                    return;
                                }
                                case 0x1f:
                                    if (Main.netMode == 2)
                                    {
                                        int num124 = BitConverter.ToInt32(this.readBuffer, index);
                                        index += 4;
                                        int num125 = BitConverter.ToInt32(this.readBuffer, index);
                                        index += 4;
                                        int num126 = Chest.FindChest(num124, num125);
                                        if ((num126 > -1) && (Chest.UsingChest(num126) == -1))
                                        {
                                            for (int num127 = 0; num127 < Chest.maxItems; num127++)
                                            {
                                                NetMessage.SendData(0x20, this.whoAmI, -1, "", num126, (float) num127, 0f, 0f);
                                            }
                                            NetMessage.SendData(0x21, this.whoAmI, -1, "", num126, 0f, 0f, 0f);
                                            Main.player[this.whoAmI].chest = num126;
                                            return;
                                        }
                                    }
                                    return;

                                case 0x20:
                                {
                                    int num128 = BitConverter.ToInt16(this.readBuffer, index);
                                    index += 2;
                                    int num129 = this.readBuffer[index];
                                    index++;
                                    int num130 = this.readBuffer[index];
                                    index++;
                                    string str12 = Encoding.ASCII.GetString(this.readBuffer, index, (length - index) + start);
                                    if (Main.chest[num128] == null)
                                    {
                                        Main.chest[num128] = new Chest();
                                    }
                                    if (Main.chest[num128].item[num129] == null)
                                    {
                                        Main.chest[num128].item[num129] = new Item();
                                    }
                                    Main.chest[num128].item[num129].SetDefaults(str12);
                                    Main.chest[num128].item[num129].stack = num130;
                                    return;
                                }
                                case 30:
                                {
                                    byte num122 = this.readBuffer[index];
                                    if (ShankShock.permaPvp)
                                    {
                                        Main.player[num122].hostile = true;
                                        NetMessage.SendData(30, -1, -1, "", num122);
                                        return;
                                    }
                                    index++;
                                    byte num123 = this.readBuffer[index];
                                    if (num123 == 1)
                                    {
                                        Main.player[num122].hostile = true;
                                    }
                                    else
                                    {
                                        Main.player[num122].hostile = false;
                                    }
                                    if (Main.netMode == 2)
                                    {
                                        NetMessage.SendData(30, -1, this.whoAmI, "", num122, 0f, 0f, 0f);
                                        string str11 = " has enabled PvP!";
                                        if (num123 == 0)
                                        {
                                            str11 = " has disabled PvP!";
                                        }
                                        NetMessage.SendData(0x19, -1, -1, Main.player[num122].name + str11, 8, (float) Main.teamColor[Main.player[num122].team].R, (float) Main.teamColor[Main.player[num122].team].G, (float) Main.teamColor[Main.player[num122].team].B);
                                        return;
                                    }
                                    return;
                                }
                                case 0x21:
                                {
                                    int num131 = BitConverter.ToInt16(this.readBuffer, index);
                                    index += 2;
                                    int num132 = BitConverter.ToInt32(this.readBuffer, index);
                                    index += 4;
                                    int num133 = BitConverter.ToInt32(this.readBuffer, index);
                                    if (Main.netMode == 1)
                                    {
                                        if (Main.player[Main.myPlayer].chest == -1)
                                        {
                                            Main.playerInventory = true;
                                            Main.PlaySound(10, -1, -1, 1);
                                        }
                                        else if ((Main.player[Main.myPlayer].chest != num131) && (num131 != -1))
                                        {
                                            Main.playerInventory = true;
                                            Main.PlaySound(12, -1, -1, 1);
                                        }
                                        else if ((Main.player[Main.myPlayer].chest != -1) && (num131 == -1))
                                        {
                                            Main.PlaySound(11, -1, -1, 1);
                                        }
                                        Main.player[Main.myPlayer].chest = num131;
                                        Main.player[Main.myPlayer].chestX = num132;
                                        Main.player[Main.myPlayer].chestY = num133;
                                        return;
                                    }
                                    Main.player[this.whoAmI].chest = num131;
                                    return;
                                }
                                case 0x22:
                                    if (Main.netMode == 2)
                                    {
                                        int num134 = BitConverter.ToInt32(this.readBuffer, index);
                                        index += 4;
                                        int num135 = BitConverter.ToInt32(this.readBuffer, index);
                                        WorldGen.KillTile(num134, num135, false, false, false);
                                        if (!Main.tile[num134, num135].active)
                                        {
                                            NetMessage.SendData(0x11, -1, -1, "", 0, (float) num134, (float) num135, 0f);
                                            return;
                                        }
                                    }
                                    return;

                                case 0x23:
                                {
                                    int num136 = this.readBuffer[index];
                                    index++;
                                    int healAmount = BitConverter.ToInt16(this.readBuffer, index);
                                    index += 2;
                                    if (num136 != Main.myPlayer)
                                    {
                                        Main.player[num136].HealEffect(healAmount);
                                    }
                                    if (Main.netMode == 2)
                                    {
                                        NetMessage.SendData(0x23, -1, this.whoAmI, "", num136, (float) healAmount, 0f, 0f);
                                        return;
                                    }
                                    return;
                                }
                                case 0x24:
                                {
                                    int num138 = this.readBuffer[index];
                                    index++;
                                    int num139 = this.readBuffer[index];
                                    index++;
                                    int num140 = this.readBuffer[index];
                                    index++;
                                    int num141 = this.readBuffer[index];
                                    index++;
                                    int num142 = this.readBuffer[index];
                                    index++;
                                    if (num139 == 0)
                                    {
                                        Main.player[num138].zoneEvil = false;
                                    }
                                    else
                                    {
                                        Main.player[num138].zoneEvil = true;
                                    }
                                    if (num140 == 0)
                                    {
                                        Main.player[num138].zoneMeteor = false;
                                    }
                                    else
                                    {
                                        Main.player[num138].zoneMeteor = true;
                                    }
                                    if (num141 == 0)
                                    {
                                        Main.player[num138].zoneDungeon = false;
                                    }
                                    else
                                    {
                                        Main.player[num138].zoneDungeon = true;
                                    }
                                    if (num142 == 0)
                                    {
                                        Main.player[num138].zoneJungle = false;
                                        return;
                                    }
                                    Main.player[num138].zoneJungle = true;
                                    return;
                                }
                                case 0x25:
                                    if (Main.netMode == 1)
                                    {
                                        Netplay.password = "";
                                        Main.menuMode = 0x1f;
                                        return;
                                    }
                                    return;

                                case 0x26:
                                    if (Main.netMode == 2)
                                    {
                                        if (Encoding.ASCII.GetString(this.readBuffer, index, (length - index) + start) == Netplay.password)
                                        {
                                            Netplay.serverSock[this.whoAmI].state = 1;
                                            NetMessage.SendData(3, this.whoAmI, -1, "", 0, 0f, 0f, 0f);
                                            return;
                                        }
                                        NetMessage.SendData(2, this.whoAmI, -1, "Incorrect password.", 0, 0f, 0f, 0f);
                                        return;
                                    }
                                    return;

                                case 0x1d:
                                {
                                    short num119 = BitConverter.ToInt16(this.readBuffer, index);
                                    index += 2;
                                    byte num120 = this.readBuffer[index];
                                    for (int num121 = 0; num121 < 0x3e8; num121++)
                                    {
                                        if (((Main.projectile[num121].owner == num120) && (Main.projectile[num121].identity == num119)) && Main.projectile[num121].active)
                                        {
                                            Main.projectile[num121].Kill();
                                            break;
                                        }
                                    }
                                    if (Main.netMode == 2)
                                    {
                                        NetMessage.SendData(0x1d, -1, this.whoAmI, "", num119, (float) num120, 0f, 0f);
                                    }
                                    return;
                                }
                            }
                            if ((msgType == 0x27) && (Main.netMode == 1))
                            {
                                short num143 = BitConverter.ToInt16(this.readBuffer, index);
                                Main.item[num143].owner = 8;
                                NetMessage.SendData(0x16, -1, -1, "", num143, 0f, 0f, 0f);
                                return;
                            }
                            switch (msgType)
                            {
                                case 40:
                                {
                                    byte num144 = this.readBuffer[index];
                                    index++;
                                    int num145 = BitConverter.ToInt16(this.readBuffer, index);
                                    index += 2;
                                    Main.player[num144].talkNPC = num145;
                                    if (Main.netMode == 2)
                                    {
                                        NetMessage.SendData(40, -1, this.whoAmI, "", num144, 0f, 0f, 0f);
                                        return;
                                    }
                                    return;
                                }
                                case 0x29:
                                {
                                    byte num146 = this.readBuffer[index];
                                    index++;
                                    float num147 = BitConverter.ToSingle(this.readBuffer, index);
                                    index += 4;
                                    int num148 = BitConverter.ToInt16(this.readBuffer, index);
                                    Main.player[num146].itemRotation = num147;
                                    Main.player[num146].itemAnimation = num148;
                                    if (Main.netMode == 2)
                                    {
                                        NetMessage.SendData(0x29, -1, this.whoAmI, "", num146, 0f, 0f, 0f);
                                        return;
                                    }
                                    return;
                                }
                                case 0x2a:
                                {
                                    int num149 = this.readBuffer[index];
                                    index++;
                                    int num150 = BitConverter.ToInt16(this.readBuffer, index);
                                    index += 2;
                                    int num151 = BitConverter.ToInt16(this.readBuffer, index);
                                    if (Main.netMode == 2)
                                    {
                                        num149 = this.whoAmI;
                                    }
                                    Main.player[num149].statMana = num150;
                                    Main.player[num149].statManaMax = num151;
                                    if (Main.netMode == 2)
                                    {
                                        NetMessage.SendData(0x2a, -1, this.whoAmI, "", num149, 0f, 0f, 0f);
                                        return;
                                    }
                                    return;
                                }
                                case 0x2b:
                                {
                                    int num152 = this.readBuffer[index];
                                    index++;
                                    int manaAmount = BitConverter.ToInt16(this.readBuffer, index);
                                    index += 2;
                                    if (num152 != Main.myPlayer)
                                    {
                                        Main.player[num152].ManaEffect(manaAmount);
                                    }
                                    if (Main.netMode == 2)
                                    {
                                        NetMessage.SendData(0x2b, -1, this.whoAmI, "", num152, (float) manaAmount, 0f, 0f);
                                        return;
                                    }
                                    return;
                                }
                                case 0x2c:
                                {
                                    byte num154 = this.readBuffer[index];
                                    index++;
                                    int num155 = this.readBuffer[index] - 1;
                                    index++;
                                    short num156 = BitConverter.ToInt16(this.readBuffer, index);
                                    index += 2;
                                    byte num157 = this.readBuffer[index];
                                    bool flag8 = false;
                                    if (num157 != 0)
                                    {
                                        flag8 = true;
                                    }
                                    Main.player[num154].KillMe((double) num156, num155, flag8);
                                    if (Main.netMode == 2)
                                    {
                                        NetMessage.SendData(0x2c, -1, this.whoAmI, "", num154, (float) num155, (float) num156, (float) num157);
                                        return;
                                    }
                                    return;
                                }
                            }
                            if (msgType != 0x2d)
                            {
                                switch (msgType)
                                {
                                    case 0x2e:
                                        if (Main.netMode == 2)
                                        {
                                            int num162 = BitConverter.ToInt32(this.readBuffer, index);
                                            index += 4;
                                            int num163 = BitConverter.ToInt32(this.readBuffer, index);
                                            index += 4;
                                            int num164 = Sign.ReadSign(num162, num163);
                                            if (num164 >= 0)
                                            {
                                                NetMessage.SendData(0x2f, this.whoAmI, -1, "", num164, 0f, 0f, 0f);
                                                return;
                                            }
                                        }
                                        return;

                                    case 0x2f:
                                    {
                                        int num165 = BitConverter.ToInt16(this.readBuffer, index);
                                        index += 2;
                                        int num166 = BitConverter.ToInt32(this.readBuffer, index);
                                        index += 4;
                                        int num167 = BitConverter.ToInt32(this.readBuffer, index);
                                        index += 4;
                                        string str15 = Encoding.ASCII.GetString(this.readBuffer, index, (length - index) + start);
                                        Main.sign[num165] = new Sign();
                                        Main.sign[num165].x = num166;
                                        Main.sign[num165].y = num167;
                                        Sign.TextSign(num165, str15);
                                        if (((Main.netMode == 1) && (Main.sign[num165] != null)) && (num165 != Main.player[Main.myPlayer].sign))
                                        {
                                            Main.playerInventory = false;
                                            Main.player[Main.myPlayer].talkNPC = -1;
                                            Main.editSign = false;
                                            Main.PlaySound(10, -1, -1, 1);
                                            Main.player[Main.myPlayer].sign = num165;
                                            Main.npcChatText = Main.sign[num165].text;
                                            return;
                                        }
                                        return;
                                    }
                                }
                                if (msgType == 0x30)
                                {
                                    int num168 = BitConverter.ToInt32(this.readBuffer, index);
                                    index += 4;
                                    int num169 = BitConverter.ToInt32(this.readBuffer, index);
                                    index += 4;
                                    byte num170 = this.readBuffer[index];
                                    index++;
                                    byte num171 = this.readBuffer[index];
                                    index++;
                                    if (Main.tile[num168, num169] == null)
                                    {
                                        Main.tile[num168, num169] = new Tile();
                                    }
                                    lock (Main.tile[num168, num169])
                                    {
                                        Main.tile[num168, num169].liquid = num170;
                                        if (num171 == 1)
                                        {
                                            Main.tile[num168, num169].lava = true;
                                        }
                                        else
                                        {
                                            Main.tile[num168, num169].lava = false;
                                        }
                                        if (Main.netMode == 2)
                                        {
                                            WorldGen.SquareTileFrame(num168, num169, true);
                                        }
                                        return;
                                    }
                                }
                                if ((msgType == 0x31) && (Netplay.clientSock.state == 6))
                                {
                                    Netplay.clientSock.state = 10;
                                    Main.player[Main.myPlayer].Spawn();
                                }
                                return;
                            }
                            num158 = this.readBuffer[index];
                            index++;
                            num159 = this.readBuffer[index];
                            index++;
                            team = Main.player[num158].team;
                            Main.player[num158].team = num159;
                            if (Main.netMode != 2)
                            {
                                return;
                            }
                            NetMessage.SendData(0x2d, -1, this.whoAmI, "", num158, 0f, 0f, 0f);
                            str14 = "";
                            switch (num159)
                            {
                                case 0:
                                    str14 = " is no longer on a party.";
                                    goto Label_33CD;

                                case 1:
                                    str14 = " has joined the red party.";
                                    goto Label_33CD;

                                case 2:
                                    str14 = " has joined the green party.";
                                    goto Label_33CD;

                                case 3:
                                    str14 = " has joined the blue party.";
                                    goto Label_33CD;

                                case 4:
                                    str14 = " has joined the yellow party.";
                                    goto Label_33CD;
                            }
                            goto Label_33CD;
                        }
                        num58 = this.readBuffer[index];
                        index++;
                        num59 = BitConverter.ToInt32(this.readBuffer, index);
                        index += 4;
                        num60 = BitConverter.ToInt32(this.readBuffer, index);
                        index += 4;
                        num61 = this.readBuffer[index];
                        int direction = 0;
                        if (num61 == 0)
                        {
                            direction = -1;
                        }
                        switch (num58)
                        {
                            case 0:
                                WorldGen.OpenDoor(num59, num60, direction);
                                goto Label_1928;

                            case 1:
                                WorldGen.CloseDoor(num59, num60, true);
                                goto Label_1928;
                        }
                        goto Label_1928;
                    }
                    num50 = this.readBuffer[index];
                    index++;
                    num51 = BitConverter.ToInt32(this.readBuffer, index);
                    index += 4;
                    num52 = BitConverter.ToInt32(this.readBuffer, index);
                    index += 4;
                    num53 = this.readBuffer[index];
                    bool fail = false;
                    if (num53 == 1)
                    {
                        fail = true;
                    }
                    if (Main.tile[num51, num52] == null)
                    {
                        Main.tile[num51, num52] = new Tile();
                    }
                    if ((Main.netMode == 2) && !Netplay.serverSock[this.whoAmI].tileSection[Netplay.GetSectionX(num51), Netplay.GetSectionY(num52)])
                    {
                        fail = true;
                    }
                    switch (num50) //
                    {
                        case 0:
                            if (!ShankShock.TileOnWhitelist(Main.tile[num51, num52].type))
                            {
                                Main.player[this.whoAmI].breakTicks += 1;
                            }
                            WorldGen.KillTile(num51, num52, fail, false, false);
                            break;

                        case 1:
                            WorldGen.PlaceTile(num51, num52, num53, false, true, -1);
                            break;

                        case 2:
                            if (!ShankShock.TileOnWhitelist(Main.tile[num51, num52].type))
                            {
                                Main.player[this.whoAmI].breakTicks += 1;
                            }
                            WorldGen.KillWall(num51, num52, fail);
                            break;

                        case 3:
                            WorldGen.PlaceWall(num51, num52, num53, false);
                            break;

                        case 4:
                            if (!ShankShock.TileOnWhitelist(Main.tile[num51, num52].type))
                            {
                                Main.player[this.whoAmI].breakTicks += 1;
                            }
                            WorldGen.KillTile(num51, num52, fail, false, true);
                            break;
                    }
                    break;
                }
            }
            if (Main.netMode == 2)
            {
                NetMessage.SendData(0x11, -1, this.whoAmI, "", num50, (float) num51, (float) num52, (float) num53);
                if ((num50 != 1) || (num53 != 0x35))
                {
                    return;
                }
                NetMessage.SendTileSquare(-1, num51, num52, 1);
            }
            return;
        Label_1928:
            if (Main.netMode == 2)
            {
                NetMessage.SendData(0x13, -1, this.whoAmI, "", num58, (float) num59, (float) num60, (float) num61);
            }
            return;
        Label_33CD:
            num161 = 0;
            while (num161 < 8)
            {
                if (((num161 == this.whoAmI) || ((team > 0) && (Main.player[num161].team == team))) || ((num159 > 0) && (Main.player[num161].team == num159)))
                {
                    NetMessage.SendData(0x19, num161, -1, Main.player[num158].name + str14, 8, (float) Main.teamColor[num159].R, (float) Main.teamColor[num159].G, (float) Main.teamColor[num159].B);
                }
                num161++;
            }
        }

        public void Reset()
        {
            this.writeBuffer = new byte[0xffff];
            this.writeLocked = false;
            this.messageLength = 0;
            this.totalData = 0;
            this.spamCount = 0;
            this.broadcast = false;
            this.checkBytes = false;
        }
    }
}

