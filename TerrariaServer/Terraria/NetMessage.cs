using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TerrariaServer.Hooks;

namespace Terraria
{
    public class NetMessage
    {
        public static messageBuffer[] buffer = new messageBuffer[257];
        public static void SendBytes(ServerSock sock, byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            try
            {
                if (NetHooks.OnSendBytes(sock, buffer, offset, count))
                {
                    return;
                }

                if (Main.runningMono)
                    sock.networkStream.Write(buffer, offset, count);

                else
                    sock.networkStream.BeginWrite(buffer, offset, count, callback, state);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} had an exception thrown when trying to send data.", sock.clientSocket.RemoteEndPoint);
                Console.WriteLine(e);
                sock.kill = true;
            }
        }

        public static void SendData(int msgType, int remoteClient = -1, int ignoreClient = -1, string text = "", int number = 0, float number2 = 0f, float number3 = 0f, float number4 = 0f, int number5 = 0)
        {
            int num = 256;
            if (Main.netMode == 2 && remoteClient >= 0)
            {
                num = remoteClient;
            }
            lock (NetMessage.buffer[num])
            {
                if (!NetHooks.OnSendData(ref msgType, ref remoteClient, ref ignoreClient, ref text, ref number, ref number2, ref number3, ref number4, ref number5))
                {
                    int num2 = 5;
                    int num3 = num2;
                    switch (msgType)
                    {
                        case 1:
                            {
                                byte[] bytes = BitConverter.GetBytes(msgType);
                                byte[] bytes2 = Encoding.ASCII.GetBytes("Terraria" + Main.curRelease);
                                num2 += bytes2.Length;
                                byte[] bytes3 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes3, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes2, 0, NetMessage.buffer[num].writeBuffer, 5, bytes2.Length);
                            }
                            break;
                        case 2:
                            {
                                byte[] bytes4 = BitConverter.GetBytes(msgType);
                                byte[] bytes5 = Encoding.ASCII.GetBytes(text);
                                num2 += bytes5.Length;
                                byte[] bytes6 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes6, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes4, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes5, 0, NetMessage.buffer[num].writeBuffer, 5, bytes5.Length);
                                if (Main.dedServ)
                                {
                                    Console.WriteLine(Netplay.serverSock[num].tcpClient.Client.RemoteEndPoint.ToString() + " was booted: " + text);
                                }
                            }
                            break;
                        case 3:
                            {
                                byte[] bytes7 = BitConverter.GetBytes(msgType);
                                byte[] bytes8 = BitConverter.GetBytes(remoteClient);
                                num2 += bytes8.Length;
                                byte[] bytes9 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes9, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes7, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes8, 0, NetMessage.buffer[num].writeBuffer, 5, bytes8.Length);
                            }
                            break;
                        case 4:
                            {
                                byte[] bytes10 = BitConverter.GetBytes(msgType);
                                byte b = (byte)number;
                                byte b2 = (byte)Main.player[(int)b].hair;
                                byte b3 = 0;
                                if (Main.player[(int)b].male)
                                {
                                    b3 = 1;
                                }
                                byte[] bytes11 = Encoding.ASCII.GetBytes(text);
                                num2 += 24 + bytes11.Length + 1;
                                byte[] bytes12 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes12, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes10, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[5] = b;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[6] = b2;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[7] = b3;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].hairColor.R;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].hairColor.G;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].hairColor.B;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].skinColor.R;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].skinColor.G;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].skinColor.B;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].eyeColor.R;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].eyeColor.G;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].eyeColor.B;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].shirtColor.R;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].shirtColor.G;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].shirtColor.B;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].underShirtColor.R;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].underShirtColor.G;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].underShirtColor.B;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].pantsColor.R;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].pantsColor.G;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].pantsColor.B;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].shoeColor.R;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].shoeColor.G;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].shoeColor.B;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = Main.player[(int)b].difficulty;
                                num3++;
                                Buffer.BlockCopy(bytes11, 0, NetMessage.buffer[num].writeBuffer, num3, bytes11.Length);
                            }
                            break;
                        case 5:
                            {
                                byte[] bytes13 = BitConverter.GetBytes(msgType);
                                byte b4 = (byte)number;
                                byte b5 = (byte)number2;
                                byte b6;
                                byte[] bytes14;
                                if (number2 < 49f)
                                {
                                    if (Main.player[number].inventory[(int)number2].name == "" || Main.player[number].inventory[(int)number2].stack == 0 || Main.player[number].inventory[(int)number2].type == 0)
                                    {
                                        Main.player[number].inventory[(int)number2].netID = 0;
                                    }
                                    b6 = (byte)Main.player[number].inventory[(int)number2].stack;
                                    bytes14 = BitConverter.GetBytes((short)Main.player[number].inventory[(int)number2].netID);
                                    if (Main.player[number].inventory[(int)number2].stack < 0)
                                    {
                                        b6 = 0;
                                    }
                                }
                                else
                                {
                                    if (Main.player[number].armor[(int)number2 - 48 - 1].name == "" || Main.player[number].armor[(int)number2 - 48 - 1].stack == 0 || Main.player[number].armor[(int)number2 - 48 - 1].type == 0)
                                    {
                                        Main.player[number].armor[(int)number2 - 48 - 1].SetDefaults(0, false);
                                    }
                                    b6 = (byte)Main.player[number].armor[(int)number2 - 48 - 1].stack;
                                    bytes14 = BitConverter.GetBytes((short)Main.player[number].armor[(int)number2 - 48 - 1].netID);
                                    if (Main.player[number].armor[(int)number2 - 48 - 1].stack < 0)
                                    {
                                        b6 = 0;
                                    }
                                }
                                byte b7 = (byte)number3;
                                num2 += 4 + bytes14.Length;
                                byte[] bytes15 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes15, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes13, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[5] = b4;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[6] = b5;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[7] = b6;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[8] = b7;
                                num3++;
                                Buffer.BlockCopy(bytes14, 0, NetMessage.buffer[num].writeBuffer, num3, bytes14.Length);
                            }
                            break;
                        case 6:
                            {
                                byte[] bytes16 = BitConverter.GetBytes(msgType);
                                byte[] bytes17 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes17, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes16, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                            }
                            break;
                        case 7:
                            {
                                byte[] bytes18 = BitConverter.GetBytes(msgType);
                                byte[] bytes19 = BitConverter.GetBytes((int)Main.time);
                                byte b8 = 0;
                                if (Main.dayTime)
                                {
                                    b8 = 1;
                                }
                                byte b9 = (byte)Main.moonPhase;
                                byte b10 = 0;
                                if (Main.bloodMoon)
                                {
                                    b10 = 1;
                                }
                                byte[] bytes20 = BitConverter.GetBytes(Main.maxTilesX);
                                byte[] bytes21 = BitConverter.GetBytes(Main.maxTilesY);
                                byte[] bytes22 = BitConverter.GetBytes(Main.spawnTileX);
                                byte[] bytes23 = BitConverter.GetBytes(Main.spawnTileY);
                                byte[] bytes24 = BitConverter.GetBytes((int)Main.worldSurface);
                                byte[] bytes25 = BitConverter.GetBytes((int)Main.rockLayer);
                                byte[] bytes26 = BitConverter.GetBytes(Main.worldID);
                                byte[] bytes27 = Encoding.ASCII.GetBytes(Main.worldName);
                                byte b11 = 0;
                                if (WorldGen.shadowOrbSmashed)
                                {
                                    b11 += 1;
                                }
                                if (NPC.downedBoss1)
                                {
                                    b11 += 2;
                                }
                                if (NPC.downedBoss2)
                                {
                                    b11 += 4;
                                }
                                if (NPC.downedBoss3)
                                {
                                    b11 += 8;
                                }
                                if (Main.hardMode)
                                {
                                    b11 += 16;
                                }
                                if (NPC.downedClown)
                                {
                                    b11 += 32;
                                }
                                num2 += bytes19.Length + 1 + 1 + 1 + bytes20.Length + bytes21.Length + bytes22.Length + bytes23.Length + bytes24.Length + bytes25.Length + bytes26.Length + 1 + bytes27.Length;
                                byte[] bytes28 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes28, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes18, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes19, 0, NetMessage.buffer[num].writeBuffer, 5, bytes19.Length);
                                num3 += bytes19.Length;
                                NetMessage.buffer[num].writeBuffer[num3] = b8;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b9;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b10;
                                num3++;
                                Buffer.BlockCopy(bytes20, 0, NetMessage.buffer[num].writeBuffer, num3, bytes20.Length);
                                num3 += bytes20.Length;
                                Buffer.BlockCopy(bytes21, 0, NetMessage.buffer[num].writeBuffer, num3, bytes21.Length);
                                num3 += bytes21.Length;
                                Buffer.BlockCopy(bytes22, 0, NetMessage.buffer[num].writeBuffer, num3, bytes22.Length);
                                num3 += bytes22.Length;
                                Buffer.BlockCopy(bytes23, 0, NetMessage.buffer[num].writeBuffer, num3, bytes23.Length);
                                num3 += bytes23.Length;
                                Buffer.BlockCopy(bytes24, 0, NetMessage.buffer[num].writeBuffer, num3, bytes24.Length);
                                num3 += bytes24.Length;
                                Buffer.BlockCopy(bytes25, 0, NetMessage.buffer[num].writeBuffer, num3, bytes25.Length);
                                num3 += bytes25.Length;
                                Buffer.BlockCopy(bytes26, 0, NetMessage.buffer[num].writeBuffer, num3, bytes26.Length);
                                num3 += bytes26.Length;
                                NetMessage.buffer[num].writeBuffer[num3] = b11;
                                num3++;
                                Buffer.BlockCopy(bytes27, 0, NetMessage.buffer[num].writeBuffer, num3, bytes27.Length);
                                num3 += bytes27.Length;
                            }
                            break;
                        case 8:
                            {
                                byte[] bytes29 = BitConverter.GetBytes(msgType);
                                byte[] bytes30 = BitConverter.GetBytes(number);
                                byte[] bytes31 = BitConverter.GetBytes((int)number2);
                                num2 += bytes30.Length + bytes31.Length;
                                byte[] bytes32 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes32, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes29, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes30, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                Buffer.BlockCopy(bytes31, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                            }
                            break;
                        case 9:
                            {
                                byte[] bytes33 = BitConverter.GetBytes(msgType);
                                byte[] bytes34 = BitConverter.GetBytes(number);
                                byte[] bytes35 = Encoding.ASCII.GetBytes(text);
                                num2 += bytes34.Length + bytes35.Length;
                                byte[] bytes36 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes36, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes33, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes34, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                Buffer.BlockCopy(bytes35, 0, NetMessage.buffer[num].writeBuffer, num3, bytes35.Length);
                            }
                            break;
                        case 10:
                            {
                                short num4 = (short)number;
                                int num5 = (int)number2;
                                int num6 = (int)number3;
                                byte[] bytes37 = BitConverter.GetBytes(msgType);
                                Buffer.BlockCopy(bytes37, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                byte[] bytes38 = BitConverter.GetBytes(num4);
                                Buffer.BlockCopy(bytes38, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                num3 += 2;
                                byte[] bytes39 = BitConverter.GetBytes(num5);
                                Buffer.BlockCopy(bytes39, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                byte[] bytes40 = BitConverter.GetBytes(num6);
                                Buffer.BlockCopy(bytes40, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                for (int i = num5; i < num5 + (int)num4; i++)
                                {
                                    byte b12 = 0;
                                    if (Main.tile[i, num6].active)
                                    {
                                        b12 += 1;
                                    }
                                    if (Main.tile[i, num6].wall > 0)
                                    {
                                        b12 += 4;
                                    }
                                    if (Main.tile[i, num6].liquid > 0)
                                    {
                                        b12 += 8;
                                    }
                                    if (Main.tile[i, num6].wire)
                                    {
                                        b12 += 16;
                                    }
                                    NetMessage.buffer[num].writeBuffer[num3] = b12;
                                    num3++;
                                    byte[] bytes41 = BitConverter.GetBytes(Main.tile[i, num6].frameX);
                                    byte[] bytes42 = BitConverter.GetBytes(Main.tile[i, num6].frameY);
                                    byte wall = Main.tile[i, num6].wall;
                                    if (Main.tile[i, num6].active)
                                    {
                                        NetMessage.buffer[num].writeBuffer[num3] = Main.tile[i, num6].type;
                                        num3++;
                                        if (Main.tileFrameImportant[(int)Main.tile[i, num6].type])
                                        {
                                            Buffer.BlockCopy(bytes41, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                            num3 += 2;
                                            Buffer.BlockCopy(bytes42, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                            num3 += 2;
                                        }
                                    }
                                    if (wall > 0)
                                    {
                                        NetMessage.buffer[num].writeBuffer[num3] = wall;
                                        num3++;
                                    }
                                    if (Main.tile[i, num6].liquid > 0)
                                    {
                                        NetMessage.buffer[num].writeBuffer[num3] = Main.tile[i, num6].liquid;
                                        num3++;
                                        byte b13 = 0;
                                        if (Main.tile[i, num6].lava)
                                        {
                                            b13 = 1;
                                        }
                                        NetMessage.buffer[num].writeBuffer[num3] = b13;
                                        num3++;
                                    }
                                    short num7 = 1;
                                    while (i + (int)num7 < num5 + (int)num4 && Main.tile[i, num6].isTheSameAs(Main.tile[i + (int)num7, num6]))
                                    {
                                        num7 += 1;
                                    }
                                    num7 -= 1;
                                    byte[] bytes43 = BitConverter.GetBytes(num7);
                                    Buffer.BlockCopy(bytes43, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                    num3 += 2;
                                    i += (int)num7;
                                }
                                byte[] bytes44 = BitConverter.GetBytes(num3 - 4);
                                Buffer.BlockCopy(bytes44, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                num2 = num3;
                            }
                            break;
                        case 11:
                            {
                                byte[] bytes45 = BitConverter.GetBytes(msgType);
                                byte[] bytes46 = BitConverter.GetBytes(number);
                                byte[] bytes47 = BitConverter.GetBytes((int)number2);
                                byte[] bytes48 = BitConverter.GetBytes((int)number3);
                                byte[] bytes49 = BitConverter.GetBytes((int)number4);
                                num2 += bytes46.Length + bytes47.Length + bytes48.Length + bytes49.Length;
                                byte[] bytes50 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes50, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes45, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes46, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                Buffer.BlockCopy(bytes47, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                Buffer.BlockCopy(bytes48, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                Buffer.BlockCopy(bytes49, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                            }
                            break;
                        case 12:
                            {
                                byte[] bytes51 = BitConverter.GetBytes(msgType);
                                byte b14 = (byte)number;
                                byte[] bytes52 = BitConverter.GetBytes(Main.player[(int)b14].SpawnX);
                                byte[] bytes53 = BitConverter.GetBytes(Main.player[(int)b14].SpawnY);
                                num2 += 1 + bytes52.Length + bytes53.Length;
                                byte[] bytes54 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes54, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes51, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[num3] = b14;
                                num3++;
                                Buffer.BlockCopy(bytes52, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                Buffer.BlockCopy(bytes53, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                            }
                            break;
                        case 13:
                            {
                                byte[] bytes55 = BitConverter.GetBytes(msgType);
                                byte b15 = (byte)number;
                                byte b16 = 0;
                                if (Main.player[(int)b15].controlUp)
                                {
                                    b16 += 1;
                                }
                                if (Main.player[(int)b15].controlDown)
                                {
                                    b16 += 2;
                                }
                                if (Main.player[(int)b15].controlLeft)
                                {
                                    b16 += 4;
                                }
                                if (Main.player[(int)b15].controlRight)
                                {
                                    b16 += 8;
                                }
                                if (Main.player[(int)b15].controlJump)
                                {
                                    b16 += 16;
                                }
                                if (Main.player[(int)b15].controlUseItem)
                                {
                                    b16 += 32;
                                }
                                if (Main.player[(int)b15].direction == 1)
                                {
                                    b16 += 64;
                                }
                                byte b17 = (byte)Main.player[(int)b15].selectedItem;
                                byte[] bytes56 = BitConverter.GetBytes(Main.player[number].position.X);
                                byte[] bytes57 = BitConverter.GetBytes(Main.player[number].position.Y);
                                byte[] bytes58 = BitConverter.GetBytes(Main.player[number].velocity.X);
                                byte[] bytes59 = BitConverter.GetBytes(Main.player[number].velocity.Y);
                                num2 += 3 + bytes56.Length + bytes57.Length + bytes58.Length + bytes59.Length;
                                byte[] bytes60 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes60, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes55, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[5] = b15;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[6] = b16;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[7] = b17;
                                num3++;
                                Buffer.BlockCopy(bytes56, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                Buffer.BlockCopy(bytes57, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                Buffer.BlockCopy(bytes58, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                Buffer.BlockCopy(bytes59, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                            }
                            break;
                        case 14:
                            {
                                byte[] bytes61 = BitConverter.GetBytes(msgType);
                                byte b18 = (byte)number;
                                byte b19 = (byte)number2;
                                num2 += 2;
                                byte[] bytes62 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes62, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes61, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[5] = b18;
                                NetMessage.buffer[num].writeBuffer[6] = b19;
                            }
                            break;
                        case 15:
                            {
                                byte[] bytes63 = BitConverter.GetBytes(msgType);
                                byte[] bytes64 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes64, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes63, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                            }
                            break;
                        case 16:
                            {
                                byte[] bytes65 = BitConverter.GetBytes(msgType);
                                byte b20 = (byte)number;
                                byte[] bytes66 = BitConverter.GetBytes((short)Main.player[(int)b20].statLife);
                                byte[] bytes67 = BitConverter.GetBytes((short)Main.player[(int)b20].statLifeMax);
                                num2 += 1 + bytes66.Length + bytes67.Length;
                                byte[] bytes68 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes68, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes65, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[5] = b20;
                                num3++;
                                Buffer.BlockCopy(bytes66, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                num3 += 2;
                                Buffer.BlockCopy(bytes67, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                            }
                            break;
                        case 17:
                            {
                                byte[] bytes69 = BitConverter.GetBytes(msgType);
                                byte b21 = (byte)number;
                                byte[] bytes70 = BitConverter.GetBytes((int)number2);
                                byte[] bytes71 = BitConverter.GetBytes((int)number3);
                                byte b22 = (byte)number4;
                                num2 += 1 + bytes70.Length + bytes71.Length + 1 + 1;
                                byte[] bytes72 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes72, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes69, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[num3] = b21;
                                num3++;
                                Buffer.BlockCopy(bytes70, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                Buffer.BlockCopy(bytes71, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                NetMessage.buffer[num].writeBuffer[num3] = b22;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)number5;
                            }
                            break;
                        case 18:
                            {
                                byte[] bytes73 = BitConverter.GetBytes(msgType);
                                BitConverter.GetBytes((int)Main.time);
                                byte b23 = 0;
                                if (Main.dayTime)
                                {
                                    b23 = 1;
                                }
                                byte[] bytes74 = BitConverter.GetBytes((int)Main.time);
                                byte[] bytes75 = BitConverter.GetBytes(Main.sunModY);
                                byte[] bytes76 = BitConverter.GetBytes(Main.moonModY);
                                num2 += 1 + bytes74.Length + bytes75.Length + bytes76.Length;
                                byte[] bytes77 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes77, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes73, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[num3] = b23;
                                num3++;
                                Buffer.BlockCopy(bytes74, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                Buffer.BlockCopy(bytes75, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                num3 += 2;
                                Buffer.BlockCopy(bytes76, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                num3 += 2;
                            }
                            break;
                        case 19:
                            {
                                byte[] bytes78 = BitConverter.GetBytes(msgType);
                                byte b24 = (byte)number;
                                byte[] bytes79 = BitConverter.GetBytes((int)number2);
                                byte[] bytes80 = BitConverter.GetBytes((int)number3);
                                byte b25 = 0;
                                if (number4 == 1f)
                                {
                                    b25 = 1;
                                }
                                num2 += 1 + bytes79.Length + bytes80.Length + 1;
                                byte[] bytes81 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes81, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes78, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[num3] = b24;
                                num3++;
                                Buffer.BlockCopy(bytes79, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                Buffer.BlockCopy(bytes80, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                NetMessage.buffer[num].writeBuffer[num3] = b25;
                            }
                            break;
                        case 20:
                            {
                                short num8 = (short)number;
                                int num9 = (int)number2;
                                int num10 = (int)number3;
                                byte[] bytes82 = BitConverter.GetBytes(msgType);
                                Buffer.BlockCopy(bytes82, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                byte[] bytes83 = BitConverter.GetBytes(num8);
                                Buffer.BlockCopy(bytes83, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                num3 += 2;
                                byte[] bytes84 = BitConverter.GetBytes(num9);
                                Buffer.BlockCopy(bytes84, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                byte[] bytes85 = BitConverter.GetBytes(num10);
                                Buffer.BlockCopy(bytes85, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                for (int j = num9; j < num9 + (int)num8; j++)
                                {
                                    for (int k = num10; k < num10 + (int)num8; k++)
                                    {
                                        byte b26 = 0;
                                        if (Main.tile[j, k].active)
                                        {
                                            b26 += 1;
                                        }
                                        if (Main.tile[j, k].wall > 0)
                                        {
                                            b26 += 4;
                                        }
                                        if (Main.tile[j, k].liquid > 0 && Main.netMode == 2)
                                        {
                                            b26 += 8;
                                        }
                                        if (Main.tile[j, k].wire)
                                        {
                                            b26 += 16;
                                        }
                                        NetMessage.buffer[num].writeBuffer[num3] = b26;
                                        num3++;
                                        byte[] bytes86 = BitConverter.GetBytes(Main.tile[j, k].frameX);
                                        byte[] bytes87 = BitConverter.GetBytes(Main.tile[j, k].frameY);
                                        byte wall2 = Main.tile[j, k].wall;
                                        if (Main.tile[j, k].active)
                                        {
                                            NetMessage.buffer[num].writeBuffer[num3] = Main.tile[j, k].type;
                                            num3++;
                                            if (Main.tileFrameImportant[(int)Main.tile[j, k].type])
                                            {
                                                Buffer.BlockCopy(bytes86, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                                num3 += 2;
                                                Buffer.BlockCopy(bytes87, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                                num3 += 2;
                                            }
                                        }
                                        if (wall2 > 0)
                                        {
                                            NetMessage.buffer[num].writeBuffer[num3] = wall2;
                                            num3++;
                                        }
                                        if (Main.tile[j, k].liquid > 0 && Main.netMode == 2)
                                        {
                                            NetMessage.buffer[num].writeBuffer[num3] = Main.tile[j, k].liquid;
                                            num3++;
                                            byte b27 = 0;
                                            if (Main.tile[j, k].lava)
                                            {
                                                b27 = 1;
                                            }
                                            NetMessage.buffer[num].writeBuffer[num3] = b27;
                                            num3++;
                                        }
                                    }
                                }
                                byte[] bytes88 = BitConverter.GetBytes(num3 - 4);
                                Buffer.BlockCopy(bytes88, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                num2 = num3;
                            }
                            break;
                        case 21:
                            {
                                byte[] bytes89 = BitConverter.GetBytes(msgType);
                                byte[] bytes90 = BitConverter.GetBytes((short)number);
                                byte[] bytes91 = BitConverter.GetBytes(Main.item[number].position.X);
                                byte[] bytes92 = BitConverter.GetBytes(Main.item[number].position.Y);
                                byte[] bytes93 = BitConverter.GetBytes(Main.item[number].velocity.X);
                                byte[] bytes94 = BitConverter.GetBytes(Main.item[number].velocity.Y);
                                byte b28 = (byte)Main.item[number].stack;
                                byte prefix = Main.item[number].prefix;
                                short value = 0;
                                if (Main.item[number].active && Main.item[number].stack > 0)
                                {
                                    value = (short)Main.item[number].netID;
                                }
                                byte[] bytes95 = BitConverter.GetBytes(value);
                                num2 += bytes90.Length + bytes91.Length + bytes92.Length + bytes93.Length + bytes94.Length + 1 + bytes95.Length + 1;
                                byte[] bytes96 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes96, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes89, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes90, 0, NetMessage.buffer[num].writeBuffer, num3, bytes90.Length);
                                num3 += 2;
                                Buffer.BlockCopy(bytes91, 0, NetMessage.buffer[num].writeBuffer, num3, bytes91.Length);
                                num3 += 4;
                                Buffer.BlockCopy(bytes92, 0, NetMessage.buffer[num].writeBuffer, num3, bytes92.Length);
                                num3 += 4;
                                Buffer.BlockCopy(bytes93, 0, NetMessage.buffer[num].writeBuffer, num3, bytes93.Length);
                                num3 += 4;
                                Buffer.BlockCopy(bytes94, 0, NetMessage.buffer[num].writeBuffer, num3, bytes94.Length);
                                num3 += 4;
                                NetMessage.buffer[num].writeBuffer[num3] = b28;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = prefix;
                                num3++;
                                Buffer.BlockCopy(bytes95, 0, NetMessage.buffer[num].writeBuffer, num3, bytes95.Length);
                            }
                            break;
                        case 22:
                            {
                                byte[] bytes97 = BitConverter.GetBytes(msgType);
                                byte[] bytes98 = BitConverter.GetBytes((short)number);
                                byte b29 = (byte)Main.item[number].owner;
                                num2 += bytes98.Length + 1;
                                byte[] bytes99 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes99, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes97, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes98, 0, NetMessage.buffer[num].writeBuffer, num3, bytes98.Length);
                                num3 += 2;
                                NetMessage.buffer[num].writeBuffer[num3] = b29;
                            }
                            break;
                        case 23:
                            {
                                byte[] bytes100 = BitConverter.GetBytes(msgType);
                                byte[] bytes101 = BitConverter.GetBytes((short)number);
                                byte[] bytes102 = BitConverter.GetBytes(Main.npc[number].position.X);
                                byte[] bytes103 = BitConverter.GetBytes(Main.npc[number].position.Y);
                                byte[] bytes104 = BitConverter.GetBytes(Main.npc[number].velocity.X);
                                byte[] bytes105 = BitConverter.GetBytes(Main.npc[number].velocity.Y);
                                byte[] bytes106 = BitConverter.GetBytes((short)Main.npc[number].target);
                                byte[] bytes107 = BitConverter.GetBytes(Main.npc[number].life);
                                if (!Main.npc[number].active)
                                {
                                    bytes107 = BitConverter.GetBytes(0);
                                }
                                if (!Main.npc[number].active || Main.npc[number].life <= 0)
                                {
                                    Main.npc[number].netSkip = 0;
                                }
                                if (Main.npc[number].name == null)
                                {
                                    Main.npc[number].name = "";
                                }
                                byte[] bytes108 = BitConverter.GetBytes((short)Main.npc[number].netID);
                                num2 += bytes101.Length + bytes102.Length + bytes103.Length + bytes104.Length + bytes105.Length + bytes106.Length + bytes107.Length + NPC.maxAI * 4 + bytes108.Length + 1 + 1;
                                byte[] bytes109 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes109, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes100, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes101, 0, NetMessage.buffer[num].writeBuffer, num3, bytes101.Length);
                                num3 += 2;
                                Buffer.BlockCopy(bytes102, 0, NetMessage.buffer[num].writeBuffer, num3, bytes102.Length);
                                num3 += 4;
                                Buffer.BlockCopy(bytes103, 0, NetMessage.buffer[num].writeBuffer, num3, bytes103.Length);
                                num3 += 4;
                                Buffer.BlockCopy(bytes104, 0, NetMessage.buffer[num].writeBuffer, num3, bytes104.Length);
                                num3 += 4;
                                Buffer.BlockCopy(bytes105, 0, NetMessage.buffer[num].writeBuffer, num3, bytes105.Length);
                                num3 += 4;
                                Buffer.BlockCopy(bytes106, 0, NetMessage.buffer[num].writeBuffer, num3, bytes106.Length);
                                num3 += 2;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)(Main.npc[number].direction + 1);
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)(Main.npc[number].directionY + 1);
                                num3++;
                                Buffer.BlockCopy(bytes107, 0, NetMessage.buffer[num].writeBuffer, num3, bytes107.Length);
                                num3 += 4;
                                for (int l = 0; l < NPC.maxAI; l++)
                                {
                                    byte[] bytes110 = BitConverter.GetBytes(Main.npc[number].ai[l]);
                                    Buffer.BlockCopy(bytes110, 0, NetMessage.buffer[num].writeBuffer, num3, bytes110.Length);
                                    num3 += 4;
                                }
                                Buffer.BlockCopy(bytes108, 0, NetMessage.buffer[num].writeBuffer, num3, bytes108.Length);
                            }
                            break;
                        case 24:
                            {
                                byte[] bytes111 = BitConverter.GetBytes(msgType);
                                byte[] bytes112 = BitConverter.GetBytes((short)number);
                                byte b30 = (byte)number2;
                                num2 += bytes112.Length + 1;
                                byte[] bytes113 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes113, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes111, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes112, 0, NetMessage.buffer[num].writeBuffer, num3, bytes112.Length);
                                num3 += 2;
                                NetMessage.buffer[num].writeBuffer[num3] = b30;
                            }
                            break;
                        case 25:
                            {
                                byte[] bytes114 = BitConverter.GetBytes(msgType);
                                byte b31 = (byte)number;
                                byte[] bytes115 = Encoding.ASCII.GetBytes(text);
                                byte b32 = (byte)number2;
                                byte b33 = (byte)number3;
                                byte b34 = (byte)number4;
                                num2 += 1 + bytes115.Length + 3;
                                byte[] bytes116 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes116, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes114, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[num3] = b31;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b32;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b33;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b34;
                                num3++;
                                Buffer.BlockCopy(bytes115, 0, NetMessage.buffer[num].writeBuffer, num3, bytes115.Length);
                            }
                            break;
                        case 26:
                            {
                                byte[] bytes117 = BitConverter.GetBytes(msgType);
                                byte b35 = (byte)number;
                                byte b36 = (byte)(number2 + 1f);
                                byte[] bytes118 = BitConverter.GetBytes((short)number3);
                                byte[] bytes119 = Encoding.ASCII.GetBytes(text);
                                byte b37 = (byte)number4;
                                byte b38 = (byte)number5;
                                num2 += 2 + bytes118.Length + 1 + bytes119.Length + 1;
                                byte[] bytes120 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes120, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes117, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[num3] = b35;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b36;
                                num3++;
                                Buffer.BlockCopy(bytes118, 0, NetMessage.buffer[num].writeBuffer, num3, bytes118.Length);
                                num3 += 2;
                                NetMessage.buffer[num].writeBuffer[num3] = b37;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b38;
                                num3++;
                                Buffer.BlockCopy(bytes119, 0, NetMessage.buffer[num].writeBuffer, num3, bytes119.Length);
                            }
                            break;
                        case 27:
                            {
                                byte[] bytes121 = BitConverter.GetBytes(msgType);
                                byte[] bytes122 = BitConverter.GetBytes((short)Main.projectile[number].identity);
                                byte[] bytes123 = BitConverter.GetBytes(Main.projectile[number].position.X);
                                byte[] bytes124 = BitConverter.GetBytes(Main.projectile[number].position.Y);
                                byte[] bytes125 = BitConverter.GetBytes(Main.projectile[number].velocity.X);
                                byte[] bytes126 = BitConverter.GetBytes(Main.projectile[number].velocity.Y);
                                byte[] bytes127 = BitConverter.GetBytes(Main.projectile[number].knockBack);
                                byte[] bytes128 = BitConverter.GetBytes((short)Main.projectile[number].damage);
                                Buffer.BlockCopy(bytes121, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes122, 0, NetMessage.buffer[num].writeBuffer, num3, bytes122.Length);
                                num3 += 2;
                                Buffer.BlockCopy(bytes123, 0, NetMessage.buffer[num].writeBuffer, num3, bytes123.Length);
                                num3 += 4;
                                Buffer.BlockCopy(bytes124, 0, NetMessage.buffer[num].writeBuffer, num3, bytes124.Length);
                                num3 += 4;
                                Buffer.BlockCopy(bytes125, 0, NetMessage.buffer[num].writeBuffer, num3, bytes125.Length);
                                num3 += 4;
                                Buffer.BlockCopy(bytes126, 0, NetMessage.buffer[num].writeBuffer, num3, bytes126.Length);
                                num3 += 4;
                                Buffer.BlockCopy(bytes127, 0, NetMessage.buffer[num].writeBuffer, num3, bytes127.Length);
                                num3 += 4;
                                Buffer.BlockCopy(bytes128, 0, NetMessage.buffer[num].writeBuffer, num3, bytes128.Length);
                                num3 += 2;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)Main.projectile[number].owner;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = (byte)Main.projectile[number].type;
                                num3++;
                                for (int m = 0; m < Projectile.maxAI; m++)
                                {
                                    byte[] bytes129 = BitConverter.GetBytes(Main.projectile[number].ai[m]);
                                    Buffer.BlockCopy(bytes129, 0, NetMessage.buffer[num].writeBuffer, num3, bytes129.Length);
                                    num3 += 4;
                                }
                                num2 += num3;
                                byte[] bytes130 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes130, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                            }
                            break;
                        case 28:
                            {
                                byte[] bytes131 = BitConverter.GetBytes(msgType);
                                byte[] bytes132 = BitConverter.GetBytes((short)number);
                                byte[] bytes133 = BitConverter.GetBytes((short)number2);
                                byte[] bytes134 = BitConverter.GetBytes(number3);
                                byte b39 = (byte)(number4 + 1f);
                                byte b40 = (byte)number5;
                                num2 += bytes132.Length + bytes133.Length + bytes134.Length + 1 + 1;
                                byte[] bytes135 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes135, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes131, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes132, 0, NetMessage.buffer[num].writeBuffer, num3, bytes132.Length);
                                num3 += 2;
                                Buffer.BlockCopy(bytes133, 0, NetMessage.buffer[num].writeBuffer, num3, bytes133.Length);
                                num3 += 2;
                                Buffer.BlockCopy(bytes134, 0, NetMessage.buffer[num].writeBuffer, num3, bytes134.Length);
                                num3 += 4;
                                NetMessage.buffer[num].writeBuffer[num3] = b39;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b40;
                            }
                            break;
                        case 29:
                            {
                                byte[] bytes136 = BitConverter.GetBytes(msgType);
                                byte[] bytes137 = BitConverter.GetBytes((short)number);
                                byte b41 = (byte)number2;
                                num2 += bytes137.Length + 1;
                                byte[] bytes138 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes138, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes136, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes137, 0, NetMessage.buffer[num].writeBuffer, num3, bytes137.Length);
                                num3 += 2;
                                NetMessage.buffer[num].writeBuffer[num3] = b41;
                            }
                            break;
                        case 30:
                            {
                                byte[] bytes139 = BitConverter.GetBytes(msgType);
                                byte b42 = (byte)number;
                                byte b43 = 0;
                                if (Main.player[(int)b42].hostile)
                                {
                                    b43 = 1;
                                }
                                num2 += 2;
                                byte[] bytes140 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes140, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes139, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[num3] = b42;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b43;
                            }
                            break;
                        case 31:
                            {
                                byte[] bytes141 = BitConverter.GetBytes(msgType);
                                byte[] bytes142 = BitConverter.GetBytes(number);
                                byte[] bytes143 = BitConverter.GetBytes((int)number2);
                                num2 += bytes142.Length + bytes143.Length;
                                byte[] bytes144 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes144, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes141, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes142, 0, NetMessage.buffer[num].writeBuffer, num3, bytes142.Length);
                                num3 += 4;
                                Buffer.BlockCopy(bytes143, 0, NetMessage.buffer[num].writeBuffer, num3, bytes143.Length);
                            }
                            break;
                        case 32:
                            {
                                byte[] bytes145 = BitConverter.GetBytes(msgType);
                                byte[] bytes146 = BitConverter.GetBytes((short)number);
                                byte b44 = (byte)number2;
                                byte b45 = (byte)Main.chest[number].item[(int)number2].stack;
                                byte prefix2 = Main.chest[number].item[(int)number2].prefix;
                                byte[] bytes147;
                                if (Main.chest[number].item[(int)number2].name == null)
                                {
                                    bytes147 = BitConverter.GetBytes(0);
                                }
                                else
                                {
                                    bytes147 = BitConverter.GetBytes((short)Main.chest[number].item[(int)number2].netID);
                                }
                                num2 += bytes146.Length + 1 + 1 + 1 + bytes147.Length;
                                byte[] bytes148 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes148, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes145, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes146, 0, NetMessage.buffer[num].writeBuffer, num3, bytes146.Length);
                                num3 += 2;
                                NetMessage.buffer[num].writeBuffer[num3] = b44;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b45;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = prefix2;
                                num3++;
                                Buffer.BlockCopy(bytes147, 0, NetMessage.buffer[num].writeBuffer, num3, bytes147.Length);
                            }
                            break;
                        case 33:
                            {
                                byte[] bytes149 = BitConverter.GetBytes(msgType);
                                byte[] bytes150 = BitConverter.GetBytes((short)number);
                                byte[] bytes151;
                                byte[] bytes152;
                                if (number > -1)
                                {
                                    bytes151 = BitConverter.GetBytes(Main.chest[number].x);
                                    bytes152 = BitConverter.GetBytes(Main.chest[number].y);
                                }
                                else
                                {
                                    bytes151 = BitConverter.GetBytes(0);
                                    bytes152 = BitConverter.GetBytes(0);
                                }
                                num2 += bytes150.Length + bytes151.Length + bytes152.Length;
                                byte[] bytes153 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes153, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes149, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes150, 0, NetMessage.buffer[num].writeBuffer, num3, bytes150.Length);
                                num3 += 2;
                                Buffer.BlockCopy(bytes151, 0, NetMessage.buffer[num].writeBuffer, num3, bytes151.Length);
                                num3 += 4;
                                Buffer.BlockCopy(bytes152, 0, NetMessage.buffer[num].writeBuffer, num3, bytes152.Length);
                            }
                            break;
                        case 34:
                            {
                                byte[] bytes154 = BitConverter.GetBytes(msgType);
                                byte[] bytes155 = BitConverter.GetBytes(number);
                                byte[] bytes156 = BitConverter.GetBytes((int)number2);
                                num2 += bytes155.Length + bytes156.Length;
                                byte[] bytes157 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes157, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes154, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes155, 0, NetMessage.buffer[num].writeBuffer, num3, bytes155.Length);
                                num3 += 4;
                                Buffer.BlockCopy(bytes156, 0, NetMessage.buffer[num].writeBuffer, num3, bytes156.Length);
                            }
                            break;
                        case 35:
                            {
                                byte[] bytes158 = BitConverter.GetBytes(msgType);
                                byte b46 = (byte)number;
                                byte[] bytes159 = BitConverter.GetBytes((short)number2);
                                num2 += 1 + bytes159.Length;
                                byte[] bytes160 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes160, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes158, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[5] = b46;
                                num3++;
                                Buffer.BlockCopy(bytes159, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                            }
                            break;
                        case 36:
                            {
                                byte[] bytes161 = BitConverter.GetBytes(msgType);
                                byte b47 = (byte)number;
                                byte b48 = 0;
                                if (Main.player[(int)b47].zoneEvil)
                                {
                                    b48 = 1;
                                }
                                byte b49 = 0;
                                if (Main.player[(int)b47].zoneMeteor)
                                {
                                    b49 = 1;
                                }
                                byte b50 = 0;
                                if (Main.player[(int)b47].zoneDungeon)
                                {
                                    b50 = 1;
                                }
                                byte b51 = 0;
                                if (Main.player[(int)b47].zoneJungle)
                                {
                                    b51 = 1;
                                }
                                byte b52 = 0;
                                if (Main.player[(int)b47].zoneHoly)
                                {
                                    b52 = 1;
                                }
                                num2 += 6;
                                byte[] bytes162 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes162, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes161, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[num3] = b47;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b48;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b49;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b50;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b51;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b52;
                                num3++;
                            }
                            break;
                        case 37:
                            {
                                byte[] bytes163 = BitConverter.GetBytes(msgType);
                                byte[] bytes164 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes164, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes163, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                            }
                            break;
                        case 38:
                            {
                                byte[] bytes165 = BitConverter.GetBytes(msgType);
                                byte[] bytes166 = Encoding.ASCII.GetBytes(text);
                                num2 += bytes166.Length;
                                byte[] bytes167 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes167, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes165, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes166, 0, NetMessage.buffer[num].writeBuffer, num3, bytes166.Length);
                            }
                            break;
                        case 39:
                            {
                                byte[] bytes168 = BitConverter.GetBytes(msgType);
                                byte[] bytes169 = BitConverter.GetBytes((short)number);
                                num2 += bytes169.Length;
                                byte[] bytes170 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes170, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes168, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes169, 0, NetMessage.buffer[num].writeBuffer, num3, bytes169.Length);
                            }
                            break;
                        case 40:
                            {
                                byte[] bytes171 = BitConverter.GetBytes(msgType);
                                byte b53 = (byte)number;
                                byte[] bytes172 = BitConverter.GetBytes((short)Main.player[(int)b53].talkNPC);
                                num2 += 1 + bytes172.Length;
                                byte[] bytes173 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes173, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes171, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[num3] = b53;
                                num3++;
                                Buffer.BlockCopy(bytes172, 0, NetMessage.buffer[num].writeBuffer, num3, bytes172.Length);
                                num3 += 2;
                            }
                            break;
                        case 41:
                            {
                                byte[] bytes174 = BitConverter.GetBytes(msgType);
                                byte b54 = (byte)number;
                                byte[] bytes175 = BitConverter.GetBytes(Main.player[(int)b54].itemRotation);
                                byte[] bytes176 = BitConverter.GetBytes((short)Main.player[(int)b54].itemAnimation);
                                num2 += 1 + bytes175.Length + bytes176.Length;
                                byte[] bytes177 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes177, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes174, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[num3] = b54;
                                num3++;
                                Buffer.BlockCopy(bytes175, 0, NetMessage.buffer[num].writeBuffer, num3, bytes175.Length);
                                num3 += 4;
                                Buffer.BlockCopy(bytes176, 0, NetMessage.buffer[num].writeBuffer, num3, bytes176.Length);
                            }
                            break;
                        case 42:
                            {
                                byte[] bytes178 = BitConverter.GetBytes(msgType);
                                byte b55 = (byte)number;
                                byte[] bytes179 = BitConverter.GetBytes((short)Main.player[(int)b55].statMana);
                                byte[] bytes180 = BitConverter.GetBytes((short)Main.player[(int)b55].statManaMax);
                                num2 += 1 + bytes179.Length + bytes180.Length;
                                byte[] bytes181 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes181, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes178, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[5] = b55;
                                num3++;
                                Buffer.BlockCopy(bytes179, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                                num3 += 2;
                                Buffer.BlockCopy(bytes180, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                            }
                            break;
                        case 43:
                            {
                                byte[] bytes182 = BitConverter.GetBytes(msgType);
                                byte b56 = (byte)number;
                                byte[] bytes183 = BitConverter.GetBytes((short)number2);
                                num2 += 1 + bytes183.Length;
                                byte[] bytes184 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes184, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes182, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[5] = b56;
                                num3++;
                                Buffer.BlockCopy(bytes183, 0, NetMessage.buffer[num].writeBuffer, num3, 2);
                            }
                            break;
                        case 44:
                            {
                                byte[] bytes185 = BitConverter.GetBytes(msgType);
                                byte b57 = (byte)number;
                                byte b58 = (byte)(number2 + 1f);
                                byte[] bytes186 = BitConverter.GetBytes((short)number3);
                                byte b59 = (byte)number4;
                                byte[] bytes187 = Encoding.ASCII.GetBytes(text);
                                num2 += 2 + bytes186.Length + 1 + bytes187.Length;
                                byte[] bytes188 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes188, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes185, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[num3] = b57;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b58;
                                num3++;
                                Buffer.BlockCopy(bytes186, 0, NetMessage.buffer[num].writeBuffer, num3, bytes186.Length);
                                num3 += 2;
                                NetMessage.buffer[num].writeBuffer[num3] = b59;
                                num3++;
                                Buffer.BlockCopy(bytes187, 0, NetMessage.buffer[num].writeBuffer, num3, bytes187.Length);
                                num3 += bytes187.Length;
                            }
                            break;
                        case 45:
                            {
                                byte[] bytes189 = BitConverter.GetBytes(msgType);
                                byte b60 = (byte)number;
                                byte b61 = (byte)Main.player[(int)b60].team;
                                num2 += 2;
                                byte[] bytes190 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes190, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes189, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[5] = b60;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b61;
                            }
                            break;
                        case 46:
                            {
                                byte[] bytes191 = BitConverter.GetBytes(msgType);
                                byte[] bytes192 = BitConverter.GetBytes(number);
                                byte[] bytes193 = BitConverter.GetBytes((int)number2);
                                num2 += bytes192.Length + bytes193.Length;
                                byte[] bytes194 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes194, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes191, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes192, 0, NetMessage.buffer[num].writeBuffer, num3, bytes192.Length);
                                num3 += 4;
                                Buffer.BlockCopy(bytes193, 0, NetMessage.buffer[num].writeBuffer, num3, bytes193.Length);
                            }
                            break;
                        case 47:
                            {
                                byte[] bytes195 = BitConverter.GetBytes(msgType);
                                byte[] bytes196 = BitConverter.GetBytes((short)number);
                                byte[] bytes197 = BitConverter.GetBytes(Main.sign[number].x);
                                byte[] bytes198 = BitConverter.GetBytes(Main.sign[number].y);
                                byte[] bytes199 = Encoding.ASCII.GetBytes(Main.sign[number].text);
                                num2 += bytes196.Length + bytes197.Length + bytes198.Length + bytes199.Length;
                                byte[] bytes200 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes200, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes195, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes196, 0, NetMessage.buffer[num].writeBuffer, num3, bytes196.Length);
                                num3 += bytes196.Length;
                                Buffer.BlockCopy(bytes197, 0, NetMessage.buffer[num].writeBuffer, num3, bytes197.Length);
                                num3 += bytes197.Length;
                                Buffer.BlockCopy(bytes198, 0, NetMessage.buffer[num].writeBuffer, num3, bytes198.Length);
                                num3 += bytes198.Length;
                                Buffer.BlockCopy(bytes199, 0, NetMessage.buffer[num].writeBuffer, num3, bytes199.Length);
                                num3 += bytes199.Length;
                            }
                            break;
                        case 48:
                            {
                                byte[] bytes201 = BitConverter.GetBytes(msgType);
                                byte[] bytes202 = BitConverter.GetBytes(number);
                                byte[] bytes203 = BitConverter.GetBytes((int)number2);
                                byte liquid = Main.tile[number, (int)number2].liquid;
                                byte b62 = 0;
                                if (Main.tile[number, (int)number2].lava)
                                {
                                    b62 = 1;
                                }
                                num2 += bytes202.Length + bytes203.Length + 1 + 1;
                                byte[] bytes204 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes204, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes201, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes202, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                Buffer.BlockCopy(bytes203, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                NetMessage.buffer[num].writeBuffer[num3] = liquid;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b62;
                                num3++;
                            }
                            break;
                        case 49:
                            {
                                byte[] bytes205 = BitConverter.GetBytes(msgType);
                                byte[] bytes206 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes206, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes205, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                            }
                            break;
                        case 50:
                            {
                                byte[] bytes207 = BitConverter.GetBytes(msgType);
                                byte b63 = (byte)number;
                                num2 += 11;
                                byte[] bytes208 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes208, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes207, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[num3] = b63;
                                num3++;
                                for (int n = 0; n < 10; n++)
                                {
                                    NetMessage.buffer[num].writeBuffer[num3] = (byte)Main.player[(int)b63].buffType[n];
                                    num3++;
                                }
                            }
                            break;
                        case 51:
                            {
                                byte[] bytes209 = BitConverter.GetBytes(msgType);
                                num2 += 2;
                                byte b64 = (byte)number;
                                byte b65 = (byte)number2;
                                byte[] bytes210 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes210, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes209, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[num3] = b64;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b65;
                            }
                            break;
                        case 52:
                            {
                                byte[] bytes211 = BitConverter.GetBytes(msgType);
                                byte b66 = (byte)number;
                                byte b67 = (byte)number2;
                                byte[] bytes212 = BitConverter.GetBytes((int)number3);
                                byte[] bytes213 = BitConverter.GetBytes((int)number4);
                                num2 += 2 + bytes212.Length + bytes213.Length;
                                byte[] bytes214 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes214, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes211, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[num3] = b66;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b67;
                                num3++;
                                Buffer.BlockCopy(bytes212, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                                Buffer.BlockCopy(bytes213, 0, NetMessage.buffer[num].writeBuffer, num3, 4);
                                num3 += 4;
                            }
                            break;
                        case 53:
                            {
                                byte[] bytes215 = BitConverter.GetBytes(msgType);
                                byte[] bytes216 = BitConverter.GetBytes((short)number);
                                byte b68 = (byte)number2;
                                byte[] bytes217 = BitConverter.GetBytes((short)number3);
                                num2 += bytes216.Length + 1 + bytes217.Length;
                                byte[] bytes218 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes218, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes215, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes216, 0, NetMessage.buffer[num].writeBuffer, num3, bytes216.Length);
                                num3 += bytes216.Length;
                                NetMessage.buffer[num].writeBuffer[num3] = b68;
                                num3++;
                                Buffer.BlockCopy(bytes217, 0, NetMessage.buffer[num].writeBuffer, num3, bytes217.Length);
                                num3 += bytes217.Length;
                            }
                            break;
                        case 54:
                            {
                                byte[] bytes219 = BitConverter.GetBytes(msgType);
                                byte[] bytes220 = BitConverter.GetBytes((short)number);
                                num2 += bytes220.Length + 15;
                                byte[] bytes221 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes221, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes219, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes220, 0, NetMessage.buffer[num].writeBuffer, num3, bytes220.Length);
                                num3 += bytes220.Length;
                                for (int num11 = 0; num11 < 5; num11++)
                                {
                                    NetMessage.buffer[num].writeBuffer[num3] = (byte)Main.npc[(int)((short)number)].buffType[num11];
                                    num3++;
                                    byte[] bytes222 = BitConverter.GetBytes(Main.npc[(int)((short)number)].buffTime[num11]);
                                    Buffer.BlockCopy(bytes222, 0, NetMessage.buffer[num].writeBuffer, num3, bytes222.Length);
                                    num3 += bytes222.Length;
                                }
                            }
                            break;
                        case 55:
                            {
                                byte[] bytes223 = BitConverter.GetBytes(msgType);
                                byte b69 = (byte)number;
                                byte b70 = (byte)number2;
                                byte[] bytes224 = BitConverter.GetBytes((short)number3);
                                num2 += 2 + bytes224.Length;
                                byte[] bytes225 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes225, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes223, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[num3] = b69;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = b70;
                                num3++;
                                Buffer.BlockCopy(bytes224, 0, NetMessage.buffer[num].writeBuffer, num3, bytes224.Length);
                                num3 += bytes224.Length;
                            }
                            break;
                        case 56:
                            {
                                byte[] bytes226 = BitConverter.GetBytes(msgType);
                                byte[] bytes227 = BitConverter.GetBytes((short)number);
                                byte[] bytes228 = Encoding.ASCII.GetBytes(Main.chrName[number]);
                                num2 += bytes227.Length + bytes228.Length;
                                byte[] bytes229 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes229, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes226, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes227, 0, NetMessage.buffer[num].writeBuffer, num3, bytes227.Length);
                                num3 += bytes227.Length;
                                Buffer.BlockCopy(bytes228, 0, NetMessage.buffer[num].writeBuffer, num3, bytes228.Length);
                            }
                            break;
                        case 57:
                            {
                                byte[] bytes230 = BitConverter.GetBytes(msgType);
                                num2 += 2;
                                byte[] bytes231 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes231, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes230, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[num3] = WorldGen.tGood;
                                num3++;
                                NetMessage.buffer[num].writeBuffer[num3] = WorldGen.tEvil;
                            }
                            break;
                        case 58:
                            {
                                byte[] bytes232 = BitConverter.GetBytes(msgType);
                                byte b71 = (byte)number;
                                byte[] bytes233 = BitConverter.GetBytes(number2);
                                num2 += 1 + bytes233.Length;
                                byte[] bytes234 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes234, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes232, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                NetMessage.buffer[num].writeBuffer[num3] = b71;
                                num3++;
                                Buffer.BlockCopy(bytes233, 0, NetMessage.buffer[num].writeBuffer, num3, bytes233.Length);
                            }
                            break;
                        case 59:
                            {
                                byte[] bytes235 = BitConverter.GetBytes(msgType);
                                byte[] bytes236 = BitConverter.GetBytes(number);
                                byte[] bytes237 = BitConverter.GetBytes((int)number2);
                                num2 += bytes236.Length + bytes237.Length;
                                byte[] bytes238 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes238, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes235, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes236, 0, NetMessage.buffer[num].writeBuffer, num3, bytes236.Length);
                                num3 += 4;
                                Buffer.BlockCopy(bytes237, 0, NetMessage.buffer[num].writeBuffer, num3, bytes237.Length);
                            }
                            break;
                        case 60:
                            {
                                byte[] bytes239 = BitConverter.GetBytes(msgType);
                                byte[] bytes240 = BitConverter.GetBytes((short)number);
                                byte[] bytes241 = BitConverter.GetBytes((short)number2);
                                byte[] bytes242 = BitConverter.GetBytes((short)number3);
                                byte b72 = (byte)number4;
                                num2 += bytes240.Length + bytes241.Length + bytes242.Length + 1;
                                byte[] bytes243 = BitConverter.GetBytes(num2 - 4);
                                Buffer.BlockCopy(bytes243, 0, NetMessage.buffer[num].writeBuffer, 0, 4);
                                Buffer.BlockCopy(bytes239, 0, NetMessage.buffer[num].writeBuffer, 4, 1);
                                Buffer.BlockCopy(bytes240, 0, NetMessage.buffer[num].writeBuffer, num3, bytes240.Length);
                                num3 += 2;
                                Buffer.BlockCopy(bytes241, 0, NetMessage.buffer[num].writeBuffer, num3, bytes241.Length);
                                num3 += 2;
                                Buffer.BlockCopy(bytes242, 0, NetMessage.buffer[num].writeBuffer, num3, bytes242.Length);
                                num3 += 2;
                                NetMessage.buffer[num].writeBuffer[num3] = b72;
                                num3++;
                            }
                            break;
                    }
                    if (Main.netMode != 1)
                    {
                        goto IL_3F79;
                    }
                    if (Netplay.clientSock.tcpClient.Connected)
                    {
                        try
                        {
                            NetMessage.buffer[num].spamCount++;
                            Main.txMsg++;
                            Main.txData += num2;
                            Main.txMsgType[msgType]++;
                            Main.txDataType[msgType] += num2;
                            //NetMessage.SendBytes(Netplay.serverSock[num14], NetMessage.buffer[num].writeBuffer, 0, num2, new AsyncCallback(Netplay.serverSock[num14].ServerWriteCallBack), Netplay.serverSock[num14].networkStream);
                            Netplay.clientSock.networkStream.BeginWrite(NetMessage.buffer[num].writeBuffer, 0, num2, new AsyncCallback(Netplay.clientSock.ClientWriteCallBack), Netplay.clientSock.networkStream);
                            goto IL_4A0B;
                        }
                        catch
                        {
                            goto IL_4A0B;
                        }
                        goto IL_3F79;
                    }
                IL_4A0B:
                    if (Main.verboseNetplay)
                    {
                        for (int num12 = 0; num12 < num2; num12++)
                        {
                        }
                        for (int num13 = 0; num13 < num2; num13++)
                        {
                            byte arg_4A42_0 = NetMessage.buffer[num].writeBuffer[num13];
                        }
                        goto IL_4A54;
                    }
                    goto IL_4A54;
                IL_3F79:
                    if (remoteClient == -1)
                    {
                        if (msgType == 20)
                        {
                            for (int num14 = 0; num14 < 256; num14++)
                            {
                                if (num14 != ignoreClient && (NetMessage.buffer[num14].broadcast || (Netplay.serverSock[num14].state >= 3 && msgType == 10)) && Netplay.serverSock[num14].tcpClient.Connected && Netplay.serverSock[num14].SectionRange(number, (int)number2, (int)number3))
                                {
                                    try
                                    {
                                        NetMessage.buffer[num14].spamCount++;
                                        Main.txMsg++;
                                        Main.txData += num2;
                                        Main.txMsgType[msgType]++;
                                        Main.txDataType[msgType] += num2;
                                        NetMessage.SendBytes(Netplay.serverSock[num14], NetMessage.buffer[num].writeBuffer, 0, num2, new AsyncCallback(Netplay.serverSock[num14].ServerWriteCallBack), Netplay.serverSock[num14].networkStream);
                                        //Netplay.serverSock[num14].networkStream.BeginWrite(NetMessage.buffer[num].writeBuffer, 0, num2, new AsyncCallback(Netplay.serverSock[num14].ServerWriteCallBack), Netplay.serverSock[num14].networkStream);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            goto IL_4A0B;
                        }
                        if (msgType == 28)
                        {
                            for (int num15 = 0; num15 < 256; num15++)
                            {
                                if (num15 != ignoreClient && (NetMessage.buffer[num15].broadcast || (Netplay.serverSock[num15].state >= 3 && msgType == 10)) && Netplay.serverSock[num15].tcpClient.Connected)
                                {
                                    bool flag2 = false;
                                    if (Main.npc[number].life <= 0)
                                    {
                                        flag2 = true;
                                    }
                                    else
                                    {
                                        Rectangle rectangle = new Rectangle((int)Main.player[num15].position.X, (int)Main.player[num15].position.Y, Main.player[num15].width, Main.player[num15].height);
                                        Rectangle value2 = new Rectangle((int)Main.npc[number].position.X, (int)Main.npc[number].position.Y, Main.npc[number].width, Main.npc[number].height);
                                        value2.X -= 3000;
                                        value2.Y -= 3000;
                                        value2.Width += 6000;
                                        value2.Height += 6000;
                                        if (rectangle.Intersects(value2))
                                        {
                                            flag2 = true;
                                        }
                                    }
                                    if (flag2)
                                    {
                                        try
                                        {
                                            NetMessage.buffer[num15].spamCount++;
                                            Main.txMsg++;
                                            Main.txData += num2;
                                            Main.txMsgType[msgType]++;
                                            Main.txDataType[msgType] += num2;
                                            NetMessage.SendBytes(Netplay.serverSock[num15], NetMessage.buffer[num].writeBuffer, 0, num2, new AsyncCallback(Netplay.serverSock[num15].ServerWriteCallBack), Netplay.serverSock[num15].networkStream);
                                            //Netplay.serverSock[num15].networkStream.BeginWrite(NetMessage.buffer[num].writeBuffer, 0, num2, new AsyncCallback(Netplay.serverSock[num15].ServerWriteCallBack), Netplay.serverSock[num15].networkStream);
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                            }
                            goto IL_4A0B;
                        }
                        if (msgType == 13)
                        {
                            for (int num16 = 0; num16 < 256; num16++)
                            {
                                if (num16 != ignoreClient && (NetMessage.buffer[num16].broadcast || (Netplay.serverSock[num16].state >= 3 && msgType == 10)) && Netplay.serverSock[num16].tcpClient.Connected)
                                {
                                    bool flag3 = false;
                                    if (Main.player[number].netSkip > 0)
                                    {
                                        Rectangle rectangle2 = new Rectangle((int)Main.player[num16].position.X, (int)Main.player[num16].position.Y, Main.player[num16].width, Main.player[num16].height);
                                        Rectangle value3 = new Rectangle((int)Main.player[number].position.X, (int)Main.player[number].position.Y, Main.player[number].width, Main.player[number].height);
                                        value3.X -= 2500;
                                        value3.Y -= 2500;
                                        value3.Width += 5000;
                                        value3.Height += 5000;
                                        if (rectangle2.Intersects(value3))
                                        {
                                            flag3 = true;
                                        }
                                    }
                                    else
                                    {
                                        flag3 = true;
                                    }
                                    if (flag3)
                                    {
                                        try
                                        {
                                            NetMessage.buffer[num16].spamCount++;
                                            Main.txMsg++;
                                            Main.txData += num2;
                                            Main.txMsgType[msgType]++;
                                            Main.txDataType[msgType] += num2;
                                            NetMessage.SendBytes(Netplay.serverSock[num16], NetMessage.buffer[num].writeBuffer, 0, num2, new AsyncCallback(Netplay.serverSock[num16].ServerWriteCallBack), Netplay.serverSock[num16].networkStream);
                                            //Netplay.serverSock[num16].networkStream.BeginWrite(NetMessage.buffer[num].writeBuffer, 0, num2, new AsyncCallback(Netplay.serverSock[num16].ServerWriteCallBack), Netplay.serverSock[num16].networkStream);
                                        }
                                        catch
                                        {
                                        }
                                    }
                                }
                            }
                            Main.player[number].netSkip++;
                            if (Main.player[number].netSkip > 2)
                            {
                                Main.player[number].netSkip = 0;
                                goto IL_4A0B;
                            }
                            goto IL_4A0B;
                        }
                        else
                        {
                            if (msgType == 27)
                            {
                                for (int num17 = 0; num17 < 256; num17++)
                                {
                                    if (num17 != ignoreClient && (NetMessage.buffer[num17].broadcast || (Netplay.serverSock[num17].state >= 3 && msgType == 10)) && Netplay.serverSock[num17].tcpClient.Connected)
                                    {
                                        bool flag4 = false;
                                        if (Main.projectile[number].type == 12)
                                        {
                                            flag4 = true;
                                        }
                                        else
                                        {
                                            Rectangle rectangle3 = new Rectangle((int)Main.player[num17].position.X, (int)Main.player[num17].position.Y, Main.player[num17].width, Main.player[num17].height);
                                            Rectangle value4 = new Rectangle((int)Main.projectile[number].position.X, (int)Main.projectile[number].position.Y, Main.projectile[number].width, Main.projectile[number].height);
                                            value4.X -= 5000;
                                            value4.Y -= 5000;
                                            value4.Width += 10000;
                                            value4.Height += 10000;
                                            if (rectangle3.Intersects(value4))
                                            {
                                                flag4 = true;
                                            }
                                        }
                                        if (flag4)
                                        {
                                            try
                                            {
                                                NetMessage.buffer[num17].spamCount++;
                                                Main.txMsg++;
                                                Main.txData += num2;
                                                Main.txMsgType[msgType]++;
                                                Main.txDataType[msgType] += num2;
                                                NetMessage.SendBytes(Netplay.serverSock[num17], NetMessage.buffer[num].writeBuffer, 0, num2, new AsyncCallback(Netplay.serverSock[num17].ServerWriteCallBack), Netplay.serverSock[num17].networkStream);
                                                //Netplay.serverSock[num17].networkStream.BeginWrite(NetMessage.buffer[num].writeBuffer, 0, num2, new AsyncCallback(Netplay.serverSock[num17].ServerWriteCallBack), Netplay.serverSock[num17].networkStream);
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                                }
                                goto IL_4A0B;
                            }
                            for (int num18 = 0; num18 < 256; num18++)
                            {
                                if (num18 != ignoreClient && (NetMessage.buffer[num18].broadcast || (Netplay.serverSock[num18].state >= 3 && msgType == 10)) && Netplay.serverSock[num18].tcpClient.Connected)
                                {
                                    try
                                    {
                                        NetMessage.buffer[num18].spamCount++;
                                        Main.txMsg++;
                                        Main.txData += num2;
                                        Main.txMsgType[msgType]++;
                                        Main.txDataType[msgType] += num2;
                                        NetMessage.SendBytes(Netplay.serverSock[num18], NetMessage.buffer[num].writeBuffer, 0, num2, new AsyncCallback(Netplay.serverSock[num18].ServerWriteCallBack), Netplay.serverSock[num18].networkStream);
                                        //Netplay.serverSock[num18].networkStream.BeginWrite(NetMessage.buffer[num].writeBuffer, 0, num2, new AsyncCallback(Netplay.serverSock[num18].ServerWriteCallBack), Netplay.serverSock[num18].networkStream);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }
                            goto IL_4A0B;
                        }
                    }
                    else
                    {
                        if (Netplay.serverSock[remoteClient].tcpClient.Connected)
                        {
                            try
                            {
                                NetMessage.buffer[remoteClient].spamCount++;
                                Main.txMsg++;
                                Main.txData += num2;
                                Main.txMsgType[msgType]++;
                                Main.txDataType[msgType] += num2;
                                NetMessage.SendBytes(Netplay.serverSock[remoteClient], NetMessage.buffer[num].writeBuffer, 0, num2, new AsyncCallback(Netplay.serverSock[remoteClient].ServerWriteCallBack), Netplay.serverSock[remoteClient].networkStream);
                                //Netplay.serverSock[remoteClient].networkStream.BeginWrite(NetMessage.buffer[num].writeBuffer, 0, num2, new AsyncCallback(Netplay.serverSock[remoteClient].ServerWriteCallBack), Netplay.serverSock[remoteClient].networkStream);
                            }
                            catch
                            {
                            }
                            goto IL_4A0B;
                        }
                        goto IL_4A0B;
                    }
                IL_4A54:
                    NetMessage.buffer[num].writeLocked = false;
                    if (msgType == 19 && Main.netMode == 1)
                    {
                        int size = 5;
                        NetMessage.SendTileSquare(num, (int)number2, (int)number3, size);
                    }
                    if (msgType == 2 && Main.netMode == 2)
                    {
                        Netplay.serverSock[num].kill = true;
                    }
                }
            }
        }
        public static void RecieveBytes(byte[] bytes, int streamLength, int i = 256)
        {
            lock (NetMessage.buffer[i])
            {
                try
                {
                    Buffer.BlockCopy(bytes, 0, NetMessage.buffer[i].readBuffer, NetMessage.buffer[i].totalData, streamLength);
                    NetMessage.buffer[i].totalData += streamLength;
                    NetMessage.buffer[i].checkBytes = true;
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
        public static void CheckBytes(int i = 256)
        {
            lock (NetMessage.buffer[i])
            {
                int num = 0;
                if (NetMessage.buffer[i].totalData >= 4)
                {
                    if (NetMessage.buffer[i].messageLength == 0)
                    {
                        NetMessage.buffer[i].messageLength = BitConverter.ToInt32(NetMessage.buffer[i].readBuffer, 0) + 4;
                    }
                    while (NetMessage.buffer[i].totalData >= NetMessage.buffer[i].messageLength + num && NetMessage.buffer[i].messageLength > 0)
                    {
                        if (!Main.ignoreErrors)
                        {
                            NetMessage.buffer[i].GetData(num + 4, NetMessage.buffer[i].messageLength - 4);
                        }
                        else
                        {
                            try
                            {
                                NetMessage.buffer[i].GetData(num + 4, NetMessage.buffer[i].messageLength - 4);
                            }
                            catch
                            {
                            }
                        }
                        num += NetMessage.buffer[i].messageLength;
                        if (NetMessage.buffer[i].totalData - num >= 4)
                        {
                            NetMessage.buffer[i].messageLength = BitConverter.ToInt32(NetMessage.buffer[i].readBuffer, num) + 4;
                        }
                        else
                        {
                            NetMessage.buffer[i].messageLength = 0;
                        }
                    }
                    if (num == NetMessage.buffer[i].totalData)
                    {
                        NetMessage.buffer[i].totalData = 0;
                    }
                    else
                    {
                        if (num > 0)
                        {
                            Buffer.BlockCopy(NetMessage.buffer[i].readBuffer, num, NetMessage.buffer[i].readBuffer, 0, NetMessage.buffer[i].totalData - num);
                            NetMessage.buffer[i].totalData -= num;
                        }
                    }
                    NetMessage.buffer[i].checkBytes = false;
                }
            }
        }
        public static void BootPlayer(int plr, string msg)
        {
            NetMessage.SendData(2, plr, -1, msg, 0, 0f, 0f, 0f, 0);
        }
        public static void SendTileSquare(int whoAmi, int tileX, int tileY, int size)
        {
            int num = (size - 1) / 2;
            NetMessage.SendData(20, whoAmi, -1, "", size, (float)(tileX - num), (float)(tileY - num), 0f, 0);
        }
        public static void SendSection(int whoAmi, int sectionX, int sectionY)
        {
            if (Main.netMode != 2)
            {
                return;
            }
            try
            {
                if (sectionX >= 0 && sectionY >= 0 && sectionX < Main.maxSectionsX && sectionY < Main.maxSectionsY)
                {
                    Netplay.serverSock[whoAmi].tileSection[sectionX, sectionY] = true;
                    int num = sectionX * 200;
                    int num2 = sectionY * 150;
                    for (int i = num2; i < num2 + 150; i++)
                    {
                        NetMessage.SendData(10, whoAmi, -1, "", 200, (float)num, (float)i, 0f, 0);
                    }
                }
            }
            catch
            {
            }
        }
        public static void greetPlayer(int plr)
        {
            if (NetHooks.OnGreetPlayer(plr))
                return;
            if (Main.motd == "")
            {
                NetMessage.SendData(25, plr, -1, "Welcome to " + Main.worldName + "!", 255, 255f, 240f, 20f, 0);
            }
            else
            {
                NetMessage.SendData(25, plr, -1, Main.motd, 255, 255f, 240f, 20f, 0);
            }
            string text = "";
            for (int i = 0; i < 255; i++)
            {
                if (Main.player[i].active)
                {
                    if (text == "")
                    {
                        text += Main.player[i].name;
                    }
                    else
                    {
                        text = text + ", " + Main.player[i].name;
                    }
                }
            }
            NetMessage.SendData(25, plr, -1, "Current players: " + text + ".", 255, 255f, 240f, 20f, 0);
        }
        public static void sendWater(int x, int y)
        {
            if (Main.netMode == 1)
            {
                NetMessage.SendData(48, -1, -1, "", x, (float)y, 0f, 0f, 0);
                return;
            }
            for (int i = 0; i < 256; i++)
            {
                if ((NetMessage.buffer[i].broadcast || Netplay.serverSock[i].state >= 3) && Netplay.serverSock[i].tcpClient.Connected)
                {
                    int num = x / 200;
                    int num2 = y / 150;
                    if (Netplay.serverSock[i].tileSection[num, num2])
                    {
                        NetMessage.SendData(48, i, -1, "", x, (float)y, 0f, 0f, 0);
                    }
                }
            }
        }
        public static void syncPlayers()
        {
            bool flag = false;
            for (int i = 0; i < 255; i++)
            {
                int num = 0;
                if (Main.player[i].active)
                {
                    num = 1;
                }
                if (Netplay.serverSock[i].state == 10)
                {
                    if (Main.autoShutdown && !flag)
                    {
                        string text = Netplay.serverSock[i].tcpClient.Client.RemoteEndPoint.ToString();
                        string a = text;
                        for (int j = 0; j < text.Length; j++)
                        {
                            if (text.Substring(j, 1) == ":")
                            {
                                a = text.Substring(0, j);
                            }
                        }
                        if (a == "127.0.0.1")
                        {
                            flag = true;
                        }
                    }
                    NetMessage.SendData(14, -1, i, "", i, (float)num, 0f, 0f, 0);
                    NetMessage.SendData(4, -1, i, Main.player[i].name, i, 0f, 0f, 0f, 0);
                    NetMessage.SendData(13, -1, i, "", i, 0f, 0f, 0f, 0);
                    NetMessage.SendData(16, -1, i, "", i, 0f, 0f, 0f, 0);
                    NetMessage.SendData(30, -1, i, "", i, 0f, 0f, 0f, 0);
                    NetMessage.SendData(45, -1, i, "", i, 0f, 0f, 0f, 0);
                    NetMessage.SendData(42, -1, i, "", i, 0f, 0f, 0f, 0);
                    NetMessage.SendData(50, -1, i, "", i, 0f, 0f, 0f, 0);
                    for (int k = 0; k < 49; k++)
                    {
                        NetMessage.SendData(5, -1, i, Main.player[i].inventory[k].name, i, (float)k, (float)Main.player[i].inventory[k].prefix, 0f, 0);
                    }
                    NetMessage.SendData(5, -1, i, Main.player[i].armor[0].name, i, 49f, (float)Main.player[i].armor[0].prefix, 0f, 0);
                    NetMessage.SendData(5, -1, i, Main.player[i].armor[1].name, i, 50f, (float)Main.player[i].armor[1].prefix, 0f, 0);
                    NetMessage.SendData(5, -1, i, Main.player[i].armor[2].name, i, 51f, (float)Main.player[i].armor[2].prefix, 0f, 0);
                    NetMessage.SendData(5, -1, i, Main.player[i].armor[3].name, i, 52f, (float)Main.player[i].armor[3].prefix, 0f, 0);
                    NetMessage.SendData(5, -1, i, Main.player[i].armor[4].name, i, 53f, (float)Main.player[i].armor[4].prefix, 0f, 0);
                    NetMessage.SendData(5, -1, i, Main.player[i].armor[5].name, i, 54f, (float)Main.player[i].armor[5].prefix, 0f, 0);
                    NetMessage.SendData(5, -1, i, Main.player[i].armor[6].name, i, 55f, (float)Main.player[i].armor[6].prefix, 0f, 0);
                    NetMessage.SendData(5, -1, i, Main.player[i].armor[7].name, i, 56f, (float)Main.player[i].armor[7].prefix, 0f, 0);
                    NetMessage.SendData(5, -1, i, Main.player[i].armor[8].name, i, 57f, (float)Main.player[i].armor[8].prefix, 0f, 0);
                    NetMessage.SendData(5, -1, i, Main.player[i].armor[9].name, i, 58f, (float)Main.player[i].armor[9].prefix, 0f, 0);
                    NetMessage.SendData(5, -1, i, Main.player[i].armor[10].name, i, 59f, (float)Main.player[i].armor[10].prefix, 0f, 0);
                    if (!Netplay.serverSock[i].announced)
                    {
                        Netplay.serverSock[i].announced = true;
                    }
                }
                else
                {
                    num = 0;
                    NetMessage.SendData(14, -1, i, "", i, (float)num, 0f, 0f, 0);
                    if (Netplay.serverSock[i].announced)
                    {
                        Netplay.serverSock[i].announced = false;
                    }
                }
            }
            for (int l = 0; l < 200; l++)
            {
                if (Main.npc[l].active && Main.npc[l].townNPC && NPC.TypeToNum(Main.npc[l].type) != -1)
                {
                    int num2 = 0;
                    if (Main.npc[l].homeless)
                    {
                        num2 = 1;
                    }
                    NetMessage.SendData(60, -1, -1, "", l, (float)Main.npc[l].homeTileX, (float)Main.npc[l].homeTileY, (float)num2, 0);
                }
            }
            if (Main.autoShutdown && !flag)
            {
                WorldGen.saveWorld(false);
                Netplay.disconnect = true;
            }
        }
    }
}