namespace Terraria
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.IO;

    public class NetMessage
    {
        public static messageBuffer[] buffer = new messageBuffer[10];

        public static void CheckBytes(int i = 9)
        {
            lock (buffer[i])
            {
                int startIndex = 0;
                if (buffer[i].totalData >= 4)
                {
                    if (buffer[i].messageLength == 0)
                    {
                        buffer[i].messageLength = BitConverter.ToInt32(buffer[i].readBuffer, 0) + 4;
                    }
                    while ((buffer[i].totalData >= (buffer[i].messageLength + startIndex)) && (buffer[i].messageLength > 0))
                    {
                        if (!Main.ignoreErrors)
                        {
                            buffer[i].GetData(startIndex + 4, buffer[i].messageLength - 4);
                        }
                        else
                        {
                            try
                            {
                                buffer[i].GetData(startIndex + 4, buffer[i].messageLength - 4);
                            }
                            catch
                            {
                            }
                        }
                        startIndex += buffer[i].messageLength;
                        if ((buffer[i].totalData - startIndex) >= 4)
                        {
                            buffer[i].messageLength = BitConverter.ToInt32(buffer[i].readBuffer, startIndex) + 4;
                        }
                        else
                        {
                            buffer[i].messageLength = 0;
                        }
                    }
                    if (startIndex == buffer[i].totalData)
                    {
                        buffer[i].totalData = 0;
                    }
                    else if (startIndex > 0)
                    {
                        Buffer.BlockCopy(buffer[i].readBuffer, startIndex, buffer[i].readBuffer, 0, buffer[i].totalData - startIndex);
                        messageBuffer buffer1 = buffer[i];
                        buffer1.totalData -= startIndex;
                    }
                    buffer[i].checkBytes = false;
                }
            }
        }



        public static void greetPlayer(int plr)
        {
            if (Main.player[plr].statLifeMax > 400 || Main.player[plr].statManaMax > 200 || Main.player[plr].statLife > 400 || Main.player[plr].statMana > 200)
            {
                ShankShock.handleCheater(plr);
            }
            ShankShock.showmotd(plr);
            ShankShock.showUpdateMinder(plr);
            if (ShankShock.isAdmin(Main.player[plr].name))
            {
                if (!ShankShock.shownOneTimeInvasionMinder && ShankShock.infinateInvasion)
                {
                    ShankShock.sendMessage(plr, "You must run /invade to start the infinite invasion.", new float[] { 255, 0, 0 });
                }
            }
            if (ShankShock.permaPvp)
            {
                Main.player[plr].hostile = true;
                NetMessage.SendData(30, -1, -1, "", plr);
            }
        }

        public static void RecieveBytes(byte[] bytes, int streamLength, int i = 9)
        {
            lock (buffer[i])
            {
                try
                {
                    Buffer.BlockCopy(bytes, 0, buffer[i].readBuffer, buffer[i].totalData, streamLength);
                    messageBuffer buffer1 = buffer[i];
                    buffer1.totalData += streamLength;
                    buffer[i].checkBytes = true;
                }
                catch
                {
                    if (Main.netMode == 1)
                    {
                        Main.menuMode = 15;
                        Main.statusText = "Bad header lead to a read buffer overflow.";
                        Netplay.disconnect = true;
                    }
                    else
                    {
                        Netplay.serverSock[i].kill = true;
                    }
                }
            }
        }

        public static void SendData(int msgType, int remoteClient = -1, int ignoreClient = -1, string text = "", int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f)
        {
            int index = 9;
            if ((Main.netMode == 2) && (remoteClient >= 0))
            {
                index = remoteClient;
            }
            lock (buffer[index])
            {
                int count = 5;
                int num3 = count;
                if (msgType == 1)
                {
                    byte[] bytes = BitConverter.GetBytes(msgType);
                    byte[] src = Encoding.ASCII.GetBytes("Terraria" + Main.curRelease);
                    count += src.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(bytes, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(src, 0, buffer[index].writeBuffer, 5, src.Length);
                }
                else if (msgType == 2)
                {
                    byte[] buffer4 = BitConverter.GetBytes(msgType);
                    byte[] buffer5 = Encoding.ASCII.GetBytes(text);
                    count += buffer5.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer4, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer5, 0, buffer[index].writeBuffer, 5, buffer5.Length);
                }
                else if (msgType == 3)
                {
                    byte[] buffer7 = BitConverter.GetBytes(msgType);
                    byte[] buffer8 = BitConverter.GetBytes(remoteClient);
                    count += buffer8.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer7, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer8, 0, buffer[index].writeBuffer, 5, buffer8.Length);
                }
                else if (msgType == 4)
                {
                    byte[] buffer10 = BitConverter.GetBytes(msgType);
                    byte num4 = (byte) number;
                    byte hair = (byte) Main.player[num4].hair;
                    byte[] buffer11 = Encoding.ASCII.GetBytes(text);
                    count += 0x17 + buffer11.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer10, 0, buffer[index].writeBuffer, 4, 1);
                    buffer[index].writeBuffer[5] = num4;
                    num3++;
                    buffer[index].writeBuffer[6] = hair;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].hairColor.R;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].hairColor.G;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].hairColor.B;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].skinColor.R;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].skinColor.G;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].skinColor.B;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].eyeColor.R;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].eyeColor.G;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].eyeColor.B;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].shirtColor.R;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].shirtColor.G;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].shirtColor.B;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].underShirtColor.R;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].underShirtColor.G;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].underShirtColor.B;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].pantsColor.R;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].pantsColor.G;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].pantsColor.B;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].shoeColor.R;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].shoeColor.G;
                    num3++;
                    buffer[index].writeBuffer[num3] = Main.player[num4].shoeColor.B;
                    num3++;
                    Buffer.BlockCopy(buffer11, 0, buffer[index].writeBuffer, num3, buffer11.Length);
                }
                else if (msgType == 5)
                {
                    byte stack;
                    byte[] buffer13 = BitConverter.GetBytes(msgType);
                    byte num6 = (byte) number;
                    byte num7 = (byte) number2;
                    if (number2 < 44f)
                    {
                        stack = (byte) Main.player[number].inventory[(int) number2].stack;
                        if (Main.player[number].inventory[(int) number2].stack < 0)
                        {
                            stack = 0;
                        }
                    }
                    else
                    {
                        stack = (byte) Main.player[number].armor[((int) number2) - 0x2c].stack;
                        if (Main.player[number].armor[((int) number2) - 0x2c].stack < 0)
                        {
                            stack = 0;
                        }
                    }
                    string s = text;
                    if (s == null)
                    {
                        s = "";
                    }
                    byte[] buffer14 = Encoding.ASCII.GetBytes(s);
                    count += 3 + buffer14.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer13, 0, buffer[index].writeBuffer, 4, 1);
                    buffer[index].writeBuffer[5] = num6;
                    num3++;
                    buffer[index].writeBuffer[6] = num7;
                    num3++;
                    buffer[index].writeBuffer[7] = stack;
                    num3++;
                    Buffer.BlockCopy(buffer14, 0, buffer[index].writeBuffer, num3, buffer14.Length);
                }
                else if (msgType == 6)
                {
                    byte[] buffer16 = BitConverter.GetBytes(msgType);
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer16, 0, buffer[index].writeBuffer, 4, 1);
                }
                else if (msgType == 7)
                {
                    byte[] buffer18 = BitConverter.GetBytes(msgType);
                    byte[] buffer19 = BitConverter.GetBytes((int) Main.time);
                    byte num9 = 0;
                    if (Main.dayTime)
                    {
                        num9 = 1;
                    }
                    byte moonPhase = (byte) Main.moonPhase;
                    byte num11 = 0;
                    if (Main.bloodMoon)
                    {
                        num11 = 1;
                    }
                    byte[] buffer20 = BitConverter.GetBytes(Main.maxTilesX);
                    byte[] buffer21 = BitConverter.GetBytes(Main.maxTilesY);
                    byte[] buffer22 = BitConverter.GetBytes(Main.spawnTileX);
                    byte[] buffer23 = BitConverter.GetBytes(Main.spawnTileY);
                    byte[] buffer24 = BitConverter.GetBytes((int) Main.worldSurface);
                    byte[] buffer25 = BitConverter.GetBytes((int) Main.rockLayer);
                    byte[] buffer26 = BitConverter.GetBytes(Main.worldID);
                    byte[] buffer27 = Encoding.ASCII.GetBytes(Main.worldName);
                    count += ((((((((((buffer19.Length + 1) + 1) + 1) + buffer20.Length) + buffer21.Length) + buffer22.Length) + buffer23.Length) + buffer24.Length) + buffer25.Length) + buffer26.Length) + buffer27.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer18, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer19, 0, buffer[index].writeBuffer, 5, buffer19.Length);
                    num3 += buffer19.Length;
                    buffer[index].writeBuffer[num3] = num9;
                    num3++;
                    buffer[index].writeBuffer[num3] = moonPhase;
                    num3++;
                    buffer[index].writeBuffer[num3] = num11;
                    num3++;
                    Buffer.BlockCopy(buffer20, 0, buffer[index].writeBuffer, num3, buffer20.Length);
                    num3 += buffer20.Length;
                    Buffer.BlockCopy(buffer21, 0, buffer[index].writeBuffer, num3, buffer21.Length);
                    num3 += buffer21.Length;
                    Buffer.BlockCopy(buffer22, 0, buffer[index].writeBuffer, num3, buffer22.Length);
                    num3 += buffer22.Length;
                    Buffer.BlockCopy(buffer23, 0, buffer[index].writeBuffer, num3, buffer23.Length);
                    num3 += buffer23.Length;
                    Buffer.BlockCopy(buffer24, 0, buffer[index].writeBuffer, num3, buffer24.Length);
                    num3 += buffer24.Length;
                    Buffer.BlockCopy(buffer25, 0, buffer[index].writeBuffer, num3, buffer25.Length);
                    num3 += buffer25.Length;
                    Buffer.BlockCopy(buffer26, 0, buffer[index].writeBuffer, num3, buffer26.Length);
                    num3 += buffer26.Length;
                    Buffer.BlockCopy(buffer27, 0, buffer[index].writeBuffer, num3, buffer27.Length);
                    num3 += buffer27.Length;
                }
                else if (msgType == 8)
                {
                    byte[] buffer29 = BitConverter.GetBytes(msgType);
                    byte[] buffer30 = BitConverter.GetBytes(number);
                    byte[] buffer31 = BitConverter.GetBytes((int) number2);
                    count += buffer30.Length + buffer31.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer29, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer30, 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                    Buffer.BlockCopy(buffer31, 0, buffer[index].writeBuffer, num3, 4);
                }
                else if (msgType == 9)
                {
                    byte[] buffer33 = BitConverter.GetBytes(msgType);
                    byte[] buffer34 = BitConverter.GetBytes(number);
                    byte[] buffer35 = Encoding.ASCII.GetBytes(text);
                    count += buffer34.Length + buffer35.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer33, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer34, 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                    Buffer.BlockCopy(buffer35, 0, buffer[index].writeBuffer, num3, buffer35.Length);
                }
                else if (msgType == 10)
                {
                    short num12 = (short) number;
                    int num13 = (int) number2;
                    int num14 = (int) number3;
                    Buffer.BlockCopy(BitConverter.GetBytes(msgType), 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(BitConverter.GetBytes(num12), 0, buffer[index].writeBuffer, num3, 2);
                    num3 += 2;
                    Buffer.BlockCopy(BitConverter.GetBytes(num13), 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                    Buffer.BlockCopy(BitConverter.GetBytes(num14), 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                    for (int i = num13; i < (num13 + num12); i++)
                    {
                        byte num16 = 0;
                        if (Main.tile[i, num14].active)
                        {
                            num16 = (byte) (num16 + 1);
                        }
                        if (Main.tile[i, num14].lighted)
                        {
                            num16 = (byte) (num16 + 2);
                        }
                        if (Main.tile[i, num14].wall > 0)
                        {
                            num16 = (byte) (num16 + 4);
                        }
                        if (Main.tile[i, num14].liquid > 0)
                        {
                            num16 = (byte) (num16 + 8);
                        }
                        buffer[index].writeBuffer[num3] = num16;
                        num3++;
                        byte[] buffer41 = BitConverter.GetBytes(Main.tile[i, num14].frameX);
                        byte[] buffer42 = BitConverter.GetBytes(Main.tile[i, num14].frameY);
                        byte wall = Main.tile[i, num14].wall;
                        if (Main.tile[i, num14].active)
                        {
                            buffer[index].writeBuffer[num3] = Main.tile[i, num14].type;
                            num3++;
                            if (Main.tileFrameImportant[Main.tile[i, num14].type])
                            {
                                Buffer.BlockCopy(buffer41, 0, buffer[index].writeBuffer, num3, 2);
                                num3 += 2;
                                Buffer.BlockCopy(buffer42, 0, buffer[index].writeBuffer, num3, 2);
                                num3 += 2;
                            }
                        }
                        if (wall > 0)
                        {
                            buffer[index].writeBuffer[num3] = wall;
                            num3++;
                        }
                        if (Main.tile[i, num14].liquid > 0)
                        {
                            buffer[index].writeBuffer[num3] = Main.tile[i, num14].liquid;
                            num3++;
                            byte num18 = 0;
                            if (Main.tile[i, num14].lava)
                            {
                                num18 = 1;
                            }
                            buffer[index].writeBuffer[num3] = num18;
                            num3++;
                        }
                    }
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (num3 - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    count = num3;
                }
                else if (msgType == 11)
                {
                    byte[] buffer44 = BitConverter.GetBytes(msgType);
                    byte[] buffer45 = BitConverter.GetBytes(number);
                    byte[] buffer46 = BitConverter.GetBytes((int) number2);
                    byte[] buffer47 = BitConverter.GetBytes((int) number3);
                    byte[] buffer48 = BitConverter.GetBytes((int) number4);
                    count += ((buffer45.Length + buffer46.Length) + buffer47.Length) + buffer48.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer44, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer45, 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                    Buffer.BlockCopy(buffer46, 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                    Buffer.BlockCopy(buffer47, 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                    Buffer.BlockCopy(buffer48, 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                }
                else if (msgType == 12)
                {
                    byte[] buffer50 = BitConverter.GetBytes(msgType);
                    byte num19 = (byte) number;
                    byte[] buffer51 = BitConverter.GetBytes(Main.player[num19].SpawnX);
                    byte[] buffer52 = BitConverter.GetBytes(Main.player[num19].SpawnY);
                    count += (1 + buffer51.Length) + buffer52.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer50, 0, buffer[index].writeBuffer, 4, 1);
                    buffer[index].writeBuffer[num3] = num19;
                    num3++;
                    Buffer.BlockCopy(buffer51, 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                    Buffer.BlockCopy(buffer52, 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                }
                else if (msgType == 13)
                {
                    byte[] buffer54 = BitConverter.GetBytes(msgType);
                    byte num20 = (byte) number;
                    byte num21 = 0;
                    if (Main.player[num20].controlUp)
                    {
                        num21 = (byte) (num21 + 1);
                    }
                    if (Main.player[num20].controlDown)
                    {
                        num21 = (byte) (num21 + 2);
                    }
                    if (Main.player[num20].controlLeft)
                    {
                        num21 = (byte) (num21 + 4);
                    }
                    if (Main.player[num20].controlRight)
                    {
                        num21 = (byte) (num21 + 8);
                    }
                    if (Main.player[num20].controlJump)
                    {
                        num21 = (byte) (num21 + 0x10);
                    }
                    if (Main.player[num20].controlUseItem)
                    {
                        num21 = (byte) (num21 + 0x20);
                    }
                    if (Main.player[num20].direction == 1)
                    {
                        num21 = (byte) (num21 + 0x40);
                    }
                    byte selectedItem = (byte) Main.player[num20].selectedItem;
                    byte[] buffer55 = BitConverter.GetBytes(Main.player[number].position.X);
                    byte[] buffer56 = BitConverter.GetBytes(Main.player[number].position.Y);
                    byte[] buffer57 = BitConverter.GetBytes(Main.player[number].velocity.X);
                    byte[] buffer58 = BitConverter.GetBytes(Main.player[number].velocity.Y);
                    count += (((3 + buffer55.Length) + buffer56.Length) + buffer57.Length) + buffer58.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer54, 0, buffer[index].writeBuffer, 4, 1);
                    buffer[index].writeBuffer[5] = num20;
                    num3++;
                    buffer[index].writeBuffer[6] = num21;
                    num3++;
                    buffer[index].writeBuffer[7] = selectedItem;
                    num3++;
                    Buffer.BlockCopy(buffer55, 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                    Buffer.BlockCopy(buffer56, 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                    Buffer.BlockCopy(buffer57, 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                    Buffer.BlockCopy(buffer58, 0, buffer[index].writeBuffer, num3, 4);
                }
                else if (msgType == 14)
                {
                    byte[] buffer60 = BitConverter.GetBytes(msgType);
                    byte num23 = (byte) number;
                    byte num24 = (byte) number2;
                    count += 2;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer60, 0, buffer[index].writeBuffer, 4, 1);
                    buffer[index].writeBuffer[5] = num23;
                    buffer[index].writeBuffer[6] = num24;
                }
                else if (msgType == 15)
                {
                    byte[] buffer62 = BitConverter.GetBytes(msgType);
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer62, 0, buffer[index].writeBuffer, 4, 1);
                }
                else if (msgType == 0x10)
                {
                    byte[] buffer64 = BitConverter.GetBytes(msgType);
                    byte num25 = (byte) number;
                    byte[] buffer65 = BitConverter.GetBytes((short) Main.player[num25].statLife);
                    byte[] buffer66 = BitConverter.GetBytes((short) Main.player[num25].statLifeMax);
                    count += (1 + buffer65.Length) + buffer66.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer64, 0, buffer[index].writeBuffer, 4, 1);
                    buffer[index].writeBuffer[5] = num25;
                    num3++;
                    Buffer.BlockCopy(buffer65, 0, buffer[index].writeBuffer, num3, 2);
                    num3 += 2;
                    Buffer.BlockCopy(buffer66, 0, buffer[index].writeBuffer, num3, 2);
                }
                else if (msgType == 0x11)
                {
                    byte[] buffer68 = BitConverter.GetBytes(msgType);
                    byte num26 = (byte)number;
                    byte[] buffer69 = BitConverter.GetBytes((int)number2);
                    byte[] buffer70 = BitConverter.GetBytes((int)number3);
                    byte num27 = (byte)number4;
                    count += ((1 + buffer69.Length) + buffer70.Length) + 1;
                    Buffer.BlockCopy(BitConverter.GetBytes((int)(count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer68, 0, buffer[index].writeBuffer, 4, 1);
                    buffer[index].writeBuffer[num3] = num26;
                    num3++;
                    Buffer.BlockCopy(buffer69, 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                    Buffer.BlockCopy(buffer70, 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                    buffer[index].writeBuffer[num3] = num27;
                }
                else if (msgType == 0x12)
                {
                    byte[] buffer72 = BitConverter.GetBytes(msgType);
                    BitConverter.GetBytes((int) Main.time);
                    byte num28 = 0;
                    if (Main.dayTime)
                    {
                        num28 = 1;
                    }
                    byte[] buffer73 = BitConverter.GetBytes((int) Main.time);
                    byte[] buffer74 = BitConverter.GetBytes(Main.sunModY);
                    byte[] buffer75 = BitConverter.GetBytes(Main.moonModY);
                    count += ((1 + buffer73.Length) + buffer74.Length) + buffer75.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer72, 0, buffer[index].writeBuffer, 4, 1);
                    buffer[index].writeBuffer[num3] = num28;
                    num3++;
                    Buffer.BlockCopy(buffer73, 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                    Buffer.BlockCopy(buffer74, 0, buffer[index].writeBuffer, num3, 2);
                    num3 += 2;
                    Buffer.BlockCopy(buffer75, 0, buffer[index].writeBuffer, num3, 2);
                    num3 += 2;
                }
                else if (msgType == 0x13)
                {
                    byte[] buffer77 = BitConverter.GetBytes(msgType);
                    byte num29 = (byte) number;
                    byte[] buffer78 = BitConverter.GetBytes((int) number2);
                    byte[] buffer79 = BitConverter.GetBytes((int) number3);
                    byte num30 = 0;
                    if (number4 == 1f)
                    {
                        num30 = 1;
                    }
                    count += ((1 + buffer78.Length) + buffer79.Length) + 1;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer77, 0, buffer[index].writeBuffer, 4, 1);
                    buffer[index].writeBuffer[num3] = num29;
                    num3++;
                    Buffer.BlockCopy(buffer78, 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                    Buffer.BlockCopy(buffer79, 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                    buffer[index].writeBuffer[num3] = num30;
                }
                else if (msgType == 20)
                {
                    short num31 = (short) number;
                    int num32 = (int) number2;
                    int num33 = (int) number3;
                    Buffer.BlockCopy(BitConverter.GetBytes(msgType), 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(BitConverter.GetBytes(num31), 0, buffer[index].writeBuffer, num3, 2);
                    num3 += 2;
                    Buffer.BlockCopy(BitConverter.GetBytes(num32), 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                    Buffer.BlockCopy(BitConverter.GetBytes(num33), 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                    for (int j = num32; j < (num32 + num31); j++)
                    {
                        for (int k = num33; k < (num33 + num31); k++)
                        {
                            byte num36 = 0;
                            if (Main.tile[j, k].active)
                            {
                                num36 = (byte) (num36 + 1);
                            }
                            if (Main.tile[j, k].lighted)
                            {
                                num36 = (byte) (num36 + 2);
                            }
                            if (Main.tile[j, k].wall > 0)
                            {
                                num36 = (byte) (num36 + 4);
                            }
                            if ((Main.tile[j, k].liquid > 0) && (Main.netMode == 2))
                            {
                                num36 = (byte) (num36 + 8);
                            }
                            buffer[index].writeBuffer[num3] = num36;
                            num3++;
                            byte[] buffer85 = BitConverter.GetBytes(Main.tile[j, k].frameX);
                            byte[] buffer86 = BitConverter.GetBytes(Main.tile[j, k].frameY);
                            byte num37 = Main.tile[j, k].wall;
                            if (Main.tile[j, k].active)
                            {
                                buffer[index].writeBuffer[num3] = Main.tile[j, k].type;
                                num3++;
                                if (Main.tileFrameImportant[Main.tile[j, k].type])
                                {
                                    Buffer.BlockCopy(buffer85, 0, buffer[index].writeBuffer, num3, 2);
                                    num3 += 2;
                                    Buffer.BlockCopy(buffer86, 0, buffer[index].writeBuffer, num3, 2);
                                    num3 += 2;
                                }
                            }
                            if (num37 > 0)
                            {
                                buffer[index].writeBuffer[num3] = num37;
                                num3++;
                            }
                            if ((Main.tile[j, k].liquid > 0) && (Main.netMode == 2))
                            {
                                buffer[index].writeBuffer[num3] = Main.tile[j, k].liquid;
                                num3++;
                                byte num38 = 0;
                                if (Main.tile[j, k].lava)
                                {
                                    num38 = 1;
                                }
                                buffer[index].writeBuffer[num3] = num38;
                                num3++;
                            }
                        }
                    }
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (num3 - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    count = num3;
                }
                else if (msgType == 0x15)
                {
                    byte[] buffer88 = BitConverter.GetBytes(msgType);
                    byte[] buffer89 = BitConverter.GetBytes((short) number);
                    byte[] buffer90 = BitConverter.GetBytes(Main.item[number].position.X);
                    byte[] buffer91 = BitConverter.GetBytes(Main.item[number].position.Y);
                    byte[] buffer92 = BitConverter.GetBytes(Main.item[number].velocity.X);
                    byte[] buffer93 = BitConverter.GetBytes(Main.item[number].velocity.Y);
                    byte num39 = (byte) Main.item[number].stack;
                    string name = "0";
                    if (Main.item[number].active && (Main.item[number].stack > 0))
                    {
                        name = Main.item[number].name;
                    }
                    if (name == null)
                    {
                        name = "0";
                    }
                    byte[] buffer94 = Encoding.ASCII.GetBytes(name);
                    count += (((((buffer89.Length + buffer90.Length) + buffer91.Length) + buffer92.Length) + buffer93.Length) + 1) + buffer94.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer88, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer89, 0, buffer[index].writeBuffer, num3, buffer89.Length);
                    num3 += 2;
                    Buffer.BlockCopy(buffer90, 0, buffer[index].writeBuffer, num3, buffer90.Length);
                    num3 += 4;
                    Buffer.BlockCopy(buffer91, 0, buffer[index].writeBuffer, num3, buffer91.Length);
                    num3 += 4;
                    Buffer.BlockCopy(buffer92, 0, buffer[index].writeBuffer, num3, buffer92.Length);
                    num3 += 4;
                    Buffer.BlockCopy(buffer93, 0, buffer[index].writeBuffer, num3, buffer93.Length);
                    num3 += 4;
                    buffer[index].writeBuffer[num3] = num39;
                    num3++;
                    Buffer.BlockCopy(buffer94, 0, buffer[index].writeBuffer, num3, buffer94.Length);
                }
                else if (msgType == 0x16)
                {
                    byte[] buffer96 = BitConverter.GetBytes(msgType);
                    byte[] buffer97 = BitConverter.GetBytes((short) number);
                    byte owner = (byte) Main.item[number].owner;
                    count += buffer97.Length + 1;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer96, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer97, 0, buffer[index].writeBuffer, num3, buffer97.Length);
                    num3 += 2;
                    buffer[index].writeBuffer[num3] = owner;
                }
                else if (msgType == 0x17)
                {
                    byte[] buffer99 = BitConverter.GetBytes(msgType);
                    byte[] buffer100 = BitConverter.GetBytes((short) number);
                    byte[] buffer101 = BitConverter.GetBytes(Main.npc[number].position.X);
                    byte[] buffer102 = BitConverter.GetBytes(Main.npc[number].position.Y);
                    byte[] buffer103 = BitConverter.GetBytes(Main.npc[number].velocity.X);
                    byte[] buffer104 = BitConverter.GetBytes(Main.npc[number].velocity.Y);
                    byte[] buffer105 = BitConverter.GetBytes((short) Main.npc[number].target);
                    byte[] buffer106 = BitConverter.GetBytes((short) Main.npc[number].life);
                    if (!Main.npc[number].active)
                    {
                        buffer106 = BitConverter.GetBytes((short) 0);
                    }
                    byte[] buffer107 = Encoding.ASCII.GetBytes(Main.npc[number].name);
                    count += (((((((((buffer100.Length + buffer101.Length) + buffer102.Length) + buffer103.Length) + buffer104.Length) + buffer105.Length) + buffer106.Length) + (NPC.maxAI * 4)) + buffer107.Length) + 1) + 1;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer99, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer100, 0, buffer[index].writeBuffer, num3, buffer100.Length);
                    num3 += 2;
                    Buffer.BlockCopy(buffer101, 0, buffer[index].writeBuffer, num3, buffer101.Length);
                    num3 += 4;
                    Buffer.BlockCopy(buffer102, 0, buffer[index].writeBuffer, num3, buffer102.Length);
                    num3 += 4;
                    Buffer.BlockCopy(buffer103, 0, buffer[index].writeBuffer, num3, buffer103.Length);
                    num3 += 4;
                    Buffer.BlockCopy(buffer104, 0, buffer[index].writeBuffer, num3, buffer104.Length);
                    num3 += 4;
                    Buffer.BlockCopy(buffer105, 0, buffer[index].writeBuffer, num3, buffer105.Length);
                    num3 += 2;
                    buffer[index].writeBuffer[num3] = (byte) (Main.npc[number].direction + 1);
                    num3++;
                    buffer[index].writeBuffer[num3] = (byte) (Main.npc[number].directionY + 1);
                    num3++;
                    Buffer.BlockCopy(buffer106, 0, buffer[index].writeBuffer, num3, buffer106.Length);
                    num3 += 2;
                    for (int m = 0; m < NPC.maxAI; m++)
                    {
                        byte[] buffer109 = BitConverter.GetBytes(Main.npc[number].ai[m]);
                        Buffer.BlockCopy(buffer109, 0, buffer[index].writeBuffer, num3, buffer109.Length);
                        num3 += 4;
                    }
                    Buffer.BlockCopy(buffer107, 0, buffer[index].writeBuffer, num3, buffer107.Length);
                }
                else if (msgType == 0x18)
                {
                    byte[] buffer110 = BitConverter.GetBytes(msgType);
                    byte[] buffer111 = BitConverter.GetBytes((short) number);
                    byte num42 = (byte) number2;
                    count += buffer111.Length + 1;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer110, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer111, 0, buffer[index].writeBuffer, num3, buffer111.Length);
                    num3 += 2;
                    buffer[index].writeBuffer[num3] = num42;
                }
                else if (msgType == 0x19)
                {
                    byte[] buffer113 = BitConverter.GetBytes(msgType);
                    byte num43 = (byte) number;
                    byte[] buffer114 = Encoding.ASCII.GetBytes(text);
                    byte num44 = (byte) number2;
                    byte num45 = (byte) number3;
                    byte num46 = (byte) number4;
                    count += (1 + buffer114.Length) + 3;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer113, 0, buffer[index].writeBuffer, 4, 1);
                    buffer[index].writeBuffer[num3] = num43;
                    num3++;
                    buffer[index].writeBuffer[num3] = num44;
                    num3++;
                    buffer[index].writeBuffer[num3] = num45;
                    num3++;
                    buffer[index].writeBuffer[num3] = num46;
                    num3++;
                    Buffer.BlockCopy(buffer114, 0, buffer[index].writeBuffer, num3, buffer114.Length);
                }
                else if (msgType == 0x1a)
                {
                    byte[] buffer116 = BitConverter.GetBytes(msgType);
                    byte num47 = (byte)number;
                    byte num48 = (byte)(number2 + 1f);
                    byte[] buffer117 = BitConverter.GetBytes((short)number3);
                    byte num49 = (byte)number4;
                    count += (2 + buffer117.Length) + 1;
                    Buffer.BlockCopy(BitConverter.GetBytes((int)(count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer116, 0, buffer[index].writeBuffer, 4, 1);
                    buffer[index].writeBuffer[num3] = num47;
                    num3++;
                    buffer[index].writeBuffer[num3] = num48;
                    num3++;
                    Buffer.BlockCopy(buffer117, 0, buffer[index].writeBuffer, num3, buffer117.Length);
                    num3 += 2;
                    buffer[index].writeBuffer[num3] = num49;
                }
                else if (msgType == 0x1b)
                {
                    byte[] buffer119 = BitConverter.GetBytes(msgType);
                    byte[] buffer120 = BitConverter.GetBytes((short)Main.projectile[number].identity);
                    byte[] buffer121 = BitConverter.GetBytes(Main.projectile[number].position.X);
                    byte[] buffer122 = BitConverter.GetBytes(Main.projectile[number].position.Y);
                    byte[] buffer123 = BitConverter.GetBytes(Main.projectile[number].velocity.X);
                    byte[] buffer124 = BitConverter.GetBytes(Main.projectile[number].velocity.Y);
                    byte[] buffer125 = BitConverter.GetBytes(Main.projectile[number].knockBack);
                    byte[] buffer126 = BitConverter.GetBytes((short)Main.projectile[number].damage);
                    Buffer.BlockCopy(buffer119, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer120, 0, buffer[index].writeBuffer, num3, buffer120.Length);
                    num3 += 2;
                    Buffer.BlockCopy(buffer121, 0, buffer[index].writeBuffer, num3, buffer121.Length);
                    num3 += 4;
                    Buffer.BlockCopy(buffer122, 0, buffer[index].writeBuffer, num3, buffer122.Length);
                    num3 += 4;
                    Buffer.BlockCopy(buffer123, 0, buffer[index].writeBuffer, num3, buffer123.Length);
                    num3 += 4;
                    Buffer.BlockCopy(buffer124, 0, buffer[index].writeBuffer, num3, buffer124.Length);
                    num3 += 4;
                    Buffer.BlockCopy(buffer125, 0, buffer[index].writeBuffer, num3, buffer125.Length);
                    num3 += 4;
                    Buffer.BlockCopy(buffer126, 0, buffer[index].writeBuffer, num3, buffer126.Length);
                    num3 += 2;
                    buffer[index].writeBuffer[num3] = (byte)Main.projectile[number].owner;
                    num3++;
                    buffer[index].writeBuffer[num3] = (byte)Main.projectile[number].type;
                    num3++;
                    for (int n = 0; n < Projectile.maxAI; n++)
                    {
                        byte[] buffer127 = BitConverter.GetBytes(Main.projectile[number].ai[n]);
                        Buffer.BlockCopy(buffer127, 0, buffer[index].writeBuffer, num3, buffer127.Length);
                        num3 += 4;
                    }
                    count += num3;
                    Buffer.BlockCopy(BitConverter.GetBytes((int)(count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                }
                else if (msgType == 0x1c)
                {
                    byte[] buffer129 = BitConverter.GetBytes(msgType);
                    byte[] buffer130 = BitConverter.GetBytes((short) number);
                    byte[] buffer131 = BitConverter.GetBytes((short) number2);
                    byte[] buffer132 = BitConverter.GetBytes(number3);
                    byte num51 = (byte) (number4 + 1f);
                    count += ((buffer130.Length + buffer131.Length) + buffer132.Length) + 1;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer129, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer130, 0, buffer[index].writeBuffer, num3, buffer130.Length);
                    num3 += 2;
                    Buffer.BlockCopy(buffer131, 0, buffer[index].writeBuffer, num3, buffer131.Length);
                    num3 += 2;
                    Buffer.BlockCopy(buffer132, 0, buffer[index].writeBuffer, num3, buffer132.Length);
                    num3 += 4;
                    buffer[index].writeBuffer[num3] = num51;
                }
                else if (msgType == 0x1d)
                {
                    byte[] buffer134 = BitConverter.GetBytes(msgType);
                    byte[] buffer135 = BitConverter.GetBytes((short) number);
                    byte num52 = (byte) number2;
                    count += buffer135.Length + 1;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer134, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer135, 0, buffer[index].writeBuffer, num3, buffer135.Length);
                    num3 += 2;
                    buffer[index].writeBuffer[num3] = num52;
                }
                else if (msgType == 30)
                {
                    byte[] buffer137 = BitConverter.GetBytes(msgType);
                    byte num53 = (byte) number;
                    byte num54 = 0;
                    if (Main.player[num53].hostile)
                    {
                        num54 = 1;
                    }
                    count += 2;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer137, 0, buffer[index].writeBuffer, 4, 1);
                    buffer[index].writeBuffer[num3] = num53;
                    num3++;
                    buffer[index].writeBuffer[num3] = num54;
                }
                else if (msgType == 0x1f)
                {
                    byte[] buffer139 = BitConverter.GetBytes(msgType);
                    byte[] buffer140 = BitConverter.GetBytes(number);
                    byte[] buffer141 = BitConverter.GetBytes((int) number2);
                    count += buffer140.Length + buffer141.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer139, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer140, 0, buffer[index].writeBuffer, num3, buffer140.Length);
                    num3 += 4;
                    Buffer.BlockCopy(buffer141, 0, buffer[index].writeBuffer, num3, buffer141.Length);
                }
                else if (msgType == 0x20)
                {
                    byte[] buffer145;
                    byte[] buffer143 = BitConverter.GetBytes(msgType);
                    byte[] buffer144 = BitConverter.GetBytes((short) number);
                    byte num55 = (byte) number2;
                    byte num56 = (byte) Main.chest[number].item[(int) number2].stack;
                    if (Main.chest[number].item[(int) number2].name == null)
                    {
                        buffer145 = Encoding.ASCII.GetBytes("");
                    }
                    else
                    {
                        buffer145 = Encoding.ASCII.GetBytes(Main.chest[number].item[(int) number2].name);
                    }
                    count += ((buffer144.Length + 1) + 1) + buffer145.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer143, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer144, 0, buffer[index].writeBuffer, num3, buffer144.Length);
                    num3 += 2;
                    buffer[index].writeBuffer[num3] = num55;
                    num3++;
                    buffer[index].writeBuffer[num3] = num56;
                    num3++;
                    Buffer.BlockCopy(buffer145, 0, buffer[index].writeBuffer, num3, buffer145.Length);
                }
                else if (msgType == 0x21)
                {
                    byte[] buffer149;
                    byte[] buffer150;
                    byte[] buffer147 = BitConverter.GetBytes(msgType);
                    byte[] buffer148 = BitConverter.GetBytes((short) number);
                    if (number > -1)
                    {
                        buffer149 = BitConverter.GetBytes(Main.chest[number].x);
                        buffer150 = BitConverter.GetBytes(Main.chest[number].y);
                    }
                    else
                    {
                        buffer149 = BitConverter.GetBytes(0);
                        buffer150 = BitConverter.GetBytes(0);
                    }
                    count += (buffer148.Length + buffer149.Length) + buffer150.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer147, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer148, 0, buffer[index].writeBuffer, num3, buffer148.Length);
                    num3 += 2;
                    Buffer.BlockCopy(buffer149, 0, buffer[index].writeBuffer, num3, buffer149.Length);
                    num3 += 4;
                    Buffer.BlockCopy(buffer150, 0, buffer[index].writeBuffer, num3, buffer150.Length);
                }
                else if (msgType == 0x22)
                {
                    byte[] buffer152 = BitConverter.GetBytes(msgType);
                    byte[] buffer153 = BitConverter.GetBytes(number);
                    byte[] buffer154 = BitConverter.GetBytes((int) number2);
                    count += buffer153.Length + buffer154.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer152, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer153, 0, buffer[index].writeBuffer, num3, buffer153.Length);
                    num3 += 4;
                    Buffer.BlockCopy(buffer154, 0, buffer[index].writeBuffer, num3, buffer154.Length);
                }
                else if (msgType == 0x23)
                {
                    byte[] buffer156 = BitConverter.GetBytes(msgType);
                    byte num57 = (byte) number;
                    byte[] buffer157 = BitConverter.GetBytes((short) number2);
                    count += 1 + buffer157.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer156, 0, buffer[index].writeBuffer, 4, 1);
                    buffer[index].writeBuffer[5] = num57;
                    num3++;
                    Buffer.BlockCopy(buffer157, 0, buffer[index].writeBuffer, num3, 2);
                }
                else if (msgType == 0x24)
                {
                    byte[] buffer159 = BitConverter.GetBytes(msgType);
                    byte num58 = (byte) number;
                    byte num59 = 0;
                    if (Main.player[num58].zoneEvil)
                    {
                        num59 = 1;
                    }
                    byte num60 = 0;
                    if (Main.player[num58].zoneMeteor)
                    {
                        num60 = 1;
                    }
                    byte num61 = 0;
                    if (Main.player[num58].zoneDungeon)
                    {
                        num61 = 1;
                    }
                    byte num62 = 0;
                    if (Main.player[num58].zoneJungle)
                    {
                        num62 = 1;
                    }
                    count += 4;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer159, 0, buffer[index].writeBuffer, 4, 1);
                    buffer[index].writeBuffer[num3] = num58;
                    num3++;
                    buffer[index].writeBuffer[num3] = num59;
                    num3++;
                    buffer[index].writeBuffer[num3] = num60;
                    num3++;
                    buffer[index].writeBuffer[num3] = num61;
                    num3++;
                    buffer[index].writeBuffer[num3] = num62;
                    num3++;
                }
                else if (msgType == 0x25)
                {
                    byte[] buffer161 = BitConverter.GetBytes(msgType);
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer161, 0, buffer[index].writeBuffer, 4, 1);
                }
                else if (msgType == 0x26)
                {
                    byte[] buffer163 = BitConverter.GetBytes(msgType);
                    byte[] buffer164 = Encoding.ASCII.GetBytes(text);
                    count += buffer164.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer163, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer164, 0, buffer[index].writeBuffer, num3, buffer164.Length);
                }
                else if (msgType == 0x27)
                {
                    byte[] buffer166 = BitConverter.GetBytes(msgType);
                    byte[] buffer167 = BitConverter.GetBytes((short) number);
                    count += buffer167.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer166, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer167, 0, buffer[index].writeBuffer, num3, buffer167.Length);
                }
                else if (msgType == 40)
                {
                    byte[] buffer169 = BitConverter.GetBytes(msgType);
                    byte num63 = (byte) number;
                    byte[] buffer170 = BitConverter.GetBytes((short) Main.player[num63].talkNPC);
                    count += 1 + buffer170.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer169, 0, buffer[index].writeBuffer, 4, 1);
                    buffer[index].writeBuffer[num3] = num63;
                    num3++;
                    Buffer.BlockCopy(buffer170, 0, buffer[index].writeBuffer, num3, buffer170.Length);
                    num3 += 2;
                }
                else if (msgType == 0x29)
                {
                    byte[] buffer172 = BitConverter.GetBytes(msgType);
                    byte num64 = (byte) number;
                    byte[] buffer173 = BitConverter.GetBytes(Main.player[num64].itemRotation);
                    byte[] buffer174 = BitConverter.GetBytes((short) Main.player[num64].itemAnimation);
                    count += (1 + buffer173.Length) + buffer174.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer172, 0, buffer[index].writeBuffer, 4, 1);
                    buffer[index].writeBuffer[num3] = num64;
                    num3++;
                    Buffer.BlockCopy(buffer173, 0, buffer[index].writeBuffer, num3, buffer173.Length);
                    num3 += 4;
                    Buffer.BlockCopy(buffer174, 0, buffer[index].writeBuffer, num3, buffer174.Length);
                }
                else if (msgType == 0x2a)
                {
                    byte[] buffer176 = BitConverter.GetBytes(msgType);
                    byte num65 = (byte) number;
                    byte[] buffer177 = BitConverter.GetBytes((short) Main.player[num65].statMana);
                    byte[] buffer178 = BitConverter.GetBytes((short) Main.player[num65].statManaMax);
                    count += (1 + buffer177.Length) + buffer178.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer176, 0, buffer[index].writeBuffer, 4, 1);
                    buffer[index].writeBuffer[5] = num65;
                    num3++;
                    Buffer.BlockCopy(buffer177, 0, buffer[index].writeBuffer, num3, 2);
                    num3 += 2;
                    Buffer.BlockCopy(buffer178, 0, buffer[index].writeBuffer, num3, 2);
                }
                else if (msgType == 0x2b)
                {
                    byte[] buffer180 = BitConverter.GetBytes(msgType);
                    byte num66 = (byte) number;
                    byte[] buffer181 = BitConverter.GetBytes((short) number2);
                    count += 1 + buffer181.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer180, 0, buffer[index].writeBuffer, 4, 1);
                    buffer[index].writeBuffer[5] = num66;
                    num3++;
                    Buffer.BlockCopy(buffer181, 0, buffer[index].writeBuffer, num3, 2);
                }
                else if (msgType == 0x2c)
                {
                    byte[] buffer183 = BitConverter.GetBytes(msgType);
                    byte num67 = (byte) number;
                    byte num68 = (byte) (number2 + 1f);
                    byte[] buffer184 = BitConverter.GetBytes((short) number3);
                    byte num69 = (byte) number4;
                    count += (2 + buffer184.Length) + 1;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer183, 0, buffer[index].writeBuffer, 4, 1);
                    buffer[index].writeBuffer[num3] = num67;
                    num3++;
                    buffer[index].writeBuffer[num3] = num68;
                    num3++;
                    Buffer.BlockCopy(buffer184, 0, buffer[index].writeBuffer, num3, buffer184.Length);
                    num3 += 2;
                    buffer[index].writeBuffer[num3] = num69;
                }
                else if (msgType == 0x2d)
                {
                    byte[] buffer186 = BitConverter.GetBytes(msgType);
                    byte num70 = (byte) number;
                    byte team = (byte) Main.player[num70].team;
                    count += 2;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer186, 0, buffer[index].writeBuffer, 4, 1);
                    buffer[index].writeBuffer[5] = num70;
                    num3++;
                    buffer[index].writeBuffer[num3] = team;
                }
                else if (msgType == 0x2e)
                {
                    byte[] buffer188 = BitConverter.GetBytes(msgType);
                    byte[] buffer189 = BitConverter.GetBytes(number);
                    byte[] buffer190 = BitConverter.GetBytes((int) number2);
                    count += buffer189.Length + buffer190.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer188, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer189, 0, buffer[index].writeBuffer, num3, buffer189.Length);
                    num3 += 4;
                    Buffer.BlockCopy(buffer190, 0, buffer[index].writeBuffer, num3, buffer190.Length);
                }
                else if (msgType == 0x2f)
                {
                    byte[] buffer192 = BitConverter.GetBytes(msgType);
                    byte[] buffer193 = BitConverter.GetBytes((short) number);
                    byte[] buffer194 = BitConverter.GetBytes(Main.sign[number].x);
                    byte[] buffer195 = BitConverter.GetBytes(Main.sign[number].y);
                    byte[] buffer196 = Encoding.ASCII.GetBytes(Main.sign[number].text);
                    count += ((buffer193.Length + buffer194.Length) + buffer195.Length) + buffer196.Length;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer192, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer193, 0, buffer[index].writeBuffer, num3, buffer193.Length);
                    num3 += buffer193.Length;
                    Buffer.BlockCopy(buffer194, 0, buffer[index].writeBuffer, num3, buffer194.Length);
                    num3 += buffer194.Length;
                    Buffer.BlockCopy(buffer195, 0, buffer[index].writeBuffer, num3, buffer195.Length);
                    num3 += buffer195.Length;
                    Buffer.BlockCopy(buffer196, 0, buffer[index].writeBuffer, num3, buffer196.Length);
                    num3 += buffer196.Length;
                }
                else if (msgType == 0x30)
                {
                    byte[] buffer198 = BitConverter.GetBytes(msgType);
                    byte[] buffer199 = BitConverter.GetBytes(number);
                    byte[] buffer200 = BitConverter.GetBytes((int) number2);
                    byte liquid = Main.tile[number, (int) number2].liquid;
                    byte num73 = 0;
                    if (Main.tile[number, (int) number2].lava)
                    {
                        num73 = 1;
                    }
                    count += ((buffer199.Length + buffer200.Length) + 1) + 1;
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer198, 0, buffer[index].writeBuffer, 4, 1);
                    Buffer.BlockCopy(buffer199, 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                    Buffer.BlockCopy(buffer200, 0, buffer[index].writeBuffer, num3, 4);
                    num3 += 4;
                    buffer[index].writeBuffer[num3] = liquid;
                    num3++;
                    buffer[index].writeBuffer[num3] = num73;
                    num3++;
                }
                else if (msgType == 0x31)
                {
                    byte[] buffer202 = BitConverter.GetBytes(msgType);
                    Buffer.BlockCopy(BitConverter.GetBytes((int) (count - 4)), 0, buffer[index].writeBuffer, 0, 4);
                    Buffer.BlockCopy(buffer202, 0, buffer[index].writeBuffer, 4, 1);
                }
                if (Main.netMode == 1)
                {
                    if (Netplay.clientSock.tcpClient.Connected)
                    {
                        try
                        {
                            messageBuffer buffer1 = buffer[index];
                            buffer1.spamCount++;
                            Netplay.clientSock.networkStream.BeginWrite(buffer[index].writeBuffer, 0, count, new AsyncCallback(Netplay.clientSock.ClientWriteCallBack), Netplay.clientSock.networkStream);
                        }
                        catch
                        {
                        }
                    }
                }
                else if (remoteClient == -1)
                {
                    for (int num74 = 0; num74 < 9; num74++)
                    {
                        if (((num74 != ignoreClient) && (buffer[num74].broadcast || ((Netplay.serverSock[num74].state >= 3) && (msgType == 10)))) && Netplay.serverSock[num74].tcpClient.Connected)
                        {
                            try
                            {
                                messageBuffer buffer205 = buffer[num74];
                                buffer205.spamCount++;
                                Netplay.serverSock[num74].networkStream.BeginWrite(buffer[index].writeBuffer, 0, count, new AsyncCallback(Netplay.serverSock[num74].ServerWriteCallBack), Netplay.serverSock[num74].networkStream);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                else if (Netplay.serverSock[remoteClient].tcpClient.Connected)
                {
                    try
                    {
                        messageBuffer buffer206 = buffer[remoteClient];
                        buffer206.spamCount++;
                        Netplay.serverSock[remoteClient].networkStream.BeginWrite(buffer[index].writeBuffer, 0, count, new AsyncCallback(Netplay.serverSock[remoteClient].ServerWriteCallBack), Netplay.serverSock[remoteClient].networkStream);
                    }
                    catch
                    {
                    }
                }
                if (Main.verboseNetplay)
                {
                    for (int num75 = 0; num75 < count; num75++)
                    {
                    }
                    for (int num76 = 0; num76 < count; num76++)
                    {
                        byte num1 = buffer[index].writeBuffer[num76];
                    }
                }
                buffer[index].writeLocked = false;
                if ((msgType == 0x13) && (Main.netMode == 1))
                {
                    int size = 5;
                    SendTileSquare(index, (int) number2, (int) number3, size);
                }
                if ((msgType == 2) && (Main.netMode == 2))
                {
                    Netplay.serverSock[index].kill = true;
                }
            }
        }

        public static void SendSection(int whoAmi, int sectionX, int sectionY)
        {
            Netplay.serverSock[whoAmi].tileSection[sectionX, sectionY] = true;
            int num = sectionX * 200;
            int num2 = sectionY * 150;
            for (int i = num2; i < (num2 + 150); i++)
            {
                SendData(10, whoAmi, -1, "", 200, (float) num, (float) i, 0f);
            }
        }

        public static void SendTileSquare(int whoAmi, int tileX, int tileY, int size)
        {
            int num = (size - 1) / 2;
            SendData(20, whoAmi, -1, "", size, (float) (tileX - num), (float) (tileY - num), 0f);
        }

        public static void sendWater(int x, int y)
        {
            if (Main.netMode == 1)
            {
                SendData(0x30, -1, -1, "", x, (float) y, 0f, 0f);
            }
            else
            {
                for (int i = 0; i < 9; i++)
                {
                    if ((buffer[i].broadcast || (Netplay.serverSock[i].state >= 3)) && Netplay.serverSock[i].tcpClient.Connected)
                    {
                        int num2 = x / 200;
                        int num3 = y / 150;
                        if (Netplay.serverSock[i].tileSection[num2, num3])
                        {
                            SendData(0x30, i, -1, "", x, (float) y, 0f, 0f);
                        }
                    }
                }
            }
        }

        public static void syncPlayers()
        {
            for (int i = 0; i < 8; i++)
            {
                int num2 = 0;
                if (Main.player[i].active)
                {
                    num2 = 1;
                }
                if (Netplay.serverSock[i].state == 10)
                {
                    SendData(14, -1, i, "", i, (float) num2, 0f, 0f);
                    SendData(13, -1, i, "", i, 0f, 0f, 0f);
                    SendData(0x10, -1, i, "", i, 0f, 0f, 0f);
                    SendData(30, -1, i, "", i, 0f, 0f, 0f);
                    SendData(0x2d, -1, i, "", i, 0f, 0f, 0f);
                    SendData(0x2a, -1, i, "", i, 0f, 0f, 0f);
                    SendData(4, -1, i, Main.player[i].name, i, 0f, 0f, 0f);
                    for (int j = 0; j < 0x2c; j++)
                    {
                        SendData(5, -1, i, Main.player[i].inventory[j].name, i, (float) j, 0f, 0f);
                    }
                    SendData(5, -1, i, Main.player[i].armor[0].name, i, 44f, 0f, 0f);
                    SendData(5, -1, i, Main.player[i].armor[1].name, i, 45f, 0f, 0f);
                    SendData(5, -1, i, Main.player[i].armor[2].name, i, 46f, 0f, 0f);
                    SendData(5, -1, i, Main.player[i].armor[3].name, i, 47f, 0f, 0f);
                    SendData(5, -1, i, Main.player[i].armor[4].name, i, 48f, 0f, 0f);
                    SendData(5, -1, i, Main.player[i].armor[5].name, i, 49f, 0f, 0f);
                    SendData(5, -1, i, Main.player[i].armor[6].name, i, 50f, 0f, 0f);
                    SendData(5, -1, i, Main.player[i].armor[7].name, i, 51f, 0f, 0f);
                    if (!Netplay.serverSock[i].announced)
                    {
                        Netplay.serverSock[i].announced = true;
                        SendData(0x19, -1, i, Main.player[i].name + " has joined.", 8, 255f, 240f, 20f);
                    }
                }
                else
                {
                    SendData(14, -1, i, "", i, (float) num2, 0f, 0f);
                    if (Netplay.serverSock[i].announced)
                    {
                        Netplay.serverSock[i].announced = false;
                        SendData(0x19, -1, i, Netplay.serverSock[i].oldName + " has left.", 8, 255f, 240f, 20f);
                    }
                }
            }
        }
    }
}

