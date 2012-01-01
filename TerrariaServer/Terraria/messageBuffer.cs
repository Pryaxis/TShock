using System;
using System.Text;
using Hooks;

namespace Terraria
{
	public class messageBuffer
	{
		public const int readBufferMax = 65535;
		public const int writeBufferMax = 65535;
		public bool broadcast;
		public byte[] readBuffer = new byte[65535];
		public byte[] writeBuffer = new byte[65535];
		public bool writeLocked;
		public int messageLength;
		public int totalData;
		public int whoAmI;
		public int spamCount;
		public int maxSpam;
		public bool checkBytes;
		public void Reset()
		{
			this.writeBuffer = new byte[65535];
			this.writeLocked = false;
			this.messageLength = 0;
			this.totalData = 0;
			this.spamCount = 0;
			this.broadcast = false;
			this.checkBytes = false;
		}
		public void GetData(int start, int length)
		{
			if (this.whoAmI < 256)
			{
				Netplay.serverSock[this.whoAmI].timeOut = 0;
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
                        if(playername.Length > 8)
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
			byte b = this.readBuffer[start];
            if (NetHooks.OnGetData(ref b, this, ref num, ref length))
            {
                return;
            }
			Main.rxMsg++;
			Main.rxData += length;
			Main.rxMsgType[(int)b]++;
			Main.rxDataType[(int)b] += length;
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
					byte arg_CD_0 = this.readBuffer[j];
				}
			}
			if (Main.netMode == 2 && b != 38 && Netplay.serverSock[this.whoAmI].state == -1)
			{
				NetMessage.SendData(2, this.whoAmI, -1, "Incorrect password.", 0, 0f, 0f, 0f, 0);
				return;
			}
			if (Main.netMode == 2 && Netplay.serverSock[this.whoAmI].state < 10 && b > 12 && b != 16 && b != 42 && b != 50 && b != 38)
			{
				NetMessage.BootPlayer(this.whoAmI, "Invalid operation at this state.");
			}
			if (b == 1 && Main.netMode == 2)
			{
				if (Main.dedServ && Netplay.CheckBan(Netplay.serverSock[this.whoAmI].tcpClient.Client.RemoteEndPoint.ToString()))
				{
					NetMessage.SendData(2, this.whoAmI, -1, "You are banned from this server.", 0, 0f, 0f, 0f, 0);
					return;
				}
				if (Netplay.serverSock[this.whoAmI].state == 0)
				{
					string @string = Encoding.ASCII.GetString(this.readBuffer, start + 1, length - 1);
					if (!(@string == "Terraria" + Main.curRelease))
					{
						NetMessage.SendData(2, this.whoAmI, -1, "You are not using the same version as this server.", 0, 0f, 0f, 0f, 0);
						return;
					}
					if (Netplay.password == null || Netplay.password == "")
					{
						Netplay.serverSock[this.whoAmI].state = 1;
						NetMessage.SendData(3, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0);
						return;
					}
					Netplay.serverSock[this.whoAmI].state = -1;
					NetMessage.SendData(37, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0);
					return;
				}
			}
			else
			{
				if (b == 2 && Main.netMode == 1)
				{
					Netplay.disconnect = true;
					Main.statusText = Encoding.ASCII.GetString(this.readBuffer, start + 1, length - 1);
					return;
				}
				if (b == 3 && Main.netMode == 1)
				{
					if (Netplay.clientSock.state == 1)
					{
						Netplay.clientSock.state = 2;
					}
					int num2 = (int)this.readBuffer[start + 1];
					if (num2 != Main.myPlayer)
					{
						Main.player[num2] = (Player)Main.player[Main.myPlayer].Clone();
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
						NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].inventory[k].name, Main.myPlayer, (float)k, (float)Main.player[Main.myPlayer].inventory[k].prefix, 0f, 0);
					}
					NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[0].name, Main.myPlayer, 49f, (float)Main.player[Main.myPlayer].armor[0].prefix, 0f, 0);
					NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[1].name, Main.myPlayer, 50f, (float)Main.player[Main.myPlayer].armor[1].prefix, 0f, 0);
					NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[2].name, Main.myPlayer, 51f, (float)Main.player[Main.myPlayer].armor[2].prefix, 0f, 0);
					NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[3].name, Main.myPlayer, 52f, (float)Main.player[Main.myPlayer].armor[3].prefix, 0f, 0);
					NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[4].name, Main.myPlayer, 53f, (float)Main.player[Main.myPlayer].armor[4].prefix, 0f, 0);
					NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[5].name, Main.myPlayer, 54f, (float)Main.player[Main.myPlayer].armor[5].prefix, 0f, 0);
					NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[6].name, Main.myPlayer, 55f, (float)Main.player[Main.myPlayer].armor[6].prefix, 0f, 0);
					NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[7].name, Main.myPlayer, 56f, (float)Main.player[Main.myPlayer].armor[7].prefix, 0f, 0);
					NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[8].name, Main.myPlayer, 57f, (float)Main.player[Main.myPlayer].armor[8].prefix, 0f, 0);
					NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[9].name, Main.myPlayer, 58f, (float)Main.player[Main.myPlayer].armor[9].prefix, 0f, 0);
					NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[10].name, Main.myPlayer, 59f, (float)Main.player[Main.myPlayer].armor[10].prefix, 0f, 0);
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
						int num3 = (int)this.readBuffer[start + 1];
						if (Main.netMode == 2)
						{
							num3 = this.whoAmI;
						}
						if (num3 == Main.myPlayer)
						{
							return;
						}
						int num4 = (int)this.readBuffer[start + 2];
						if (num4 >= 36)
						{
							num4 = 0;
						}
						Main.player[num3].hair = num4;
						Main.player[num3].whoAmi = num3;
						num += 2;
						byte b2 = this.readBuffer[num];
						num++;
						if (b2 == 1)
						{
							Main.player[num3].male = true;
						}
						else
						{
							Main.player[num3].male = false;
						}
						Main.player[num3].hairColor.R = this.readBuffer[num];
						num++;
						Main.player[num3].hairColor.G = this.readBuffer[num];
						num++;
						Main.player[num3].hairColor.B = this.readBuffer[num];
						num++;
						Main.player[num3].skinColor.R = this.readBuffer[num];
						num++;
						Main.player[num3].skinColor.G = this.readBuffer[num];
						num++;
						Main.player[num3].skinColor.B = this.readBuffer[num];
						num++;
						Main.player[num3].eyeColor.R = this.readBuffer[num];
						num++;
						Main.player[num3].eyeColor.G = this.readBuffer[num];
						num++;
						Main.player[num3].eyeColor.B = this.readBuffer[num];
						num++;
						Main.player[num3].shirtColor.R = this.readBuffer[num];
						num++;
						Main.player[num3].shirtColor.G = this.readBuffer[num];
						num++;
						Main.player[num3].shirtColor.B = this.readBuffer[num];
						num++;
						Main.player[num3].underShirtColor.R = this.readBuffer[num];
						num++;
						Main.player[num3].underShirtColor.G = this.readBuffer[num];
						num++;
						Main.player[num3].underShirtColor.B = this.readBuffer[num];
						num++;
						Main.player[num3].pantsColor.R = this.readBuffer[num];
						num++;
						Main.player[num3].pantsColor.G = this.readBuffer[num];
						num++;
						Main.player[num3].pantsColor.B = this.readBuffer[num];
						num++;
						Main.player[num3].shoeColor.R = this.readBuffer[num];
						num++;
						Main.player[num3].shoeColor.G = this.readBuffer[num];
						num++;
						Main.player[num3].shoeColor.B = this.readBuffer[num];
						num++;
						byte difficulty = this.readBuffer[num];
						Main.player[num3].difficulty = difficulty;
						num++;
						string text = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
						text = text.Trim();
						Main.player[num3].name = text.Trim();
						if (Main.netMode == 2)
						{
							if (Netplay.serverSock[this.whoAmI].state < 10)
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
								NetMessage.SendData(2, this.whoAmI, -1, text + " is already on this server.", 0, 0f, 0f, 0f, 0);
								return;
							}
							if (text.Length > Player.nameLen)
							{
								NetMessage.SendData(2, this.whoAmI, -1, "Name is too long.", 0, 0f, 0f, 0f, 0);
								return;
							}
							if (text == "")
							{
								NetMessage.SendData(2, this.whoAmI, -1, "Empty name.", 0, 0f, 0f, 0f, 0);
								return;
							}
							Netplay.serverSock[this.whoAmI].oldName = text;
							Netplay.serverSock[this.whoAmI].name = text;
							NetMessage.SendData(4, -1, this.whoAmI, text, num3, 0f, 0f, 0f, 0);
							return;
						}
					}
					else
					{
						if (b == 5)
						{
							int num5 = (int)this.readBuffer[start + 1];
							if (Main.netMode == 2)
							{
								num5 = this.whoAmI;
							}
							if (num5 == Main.myPlayer)
							{
								return;
							}
							lock (Main.player[num5])
							{
								int num6 = (int)this.readBuffer[start + 2];
								int stack = (int)this.readBuffer[start + 3];
								byte b3 = this.readBuffer[start + 4];
								int type = (int)BitConverter.ToInt16(this.readBuffer, start + 5);
								if (num6 < 49)
								{
									Main.player[num5].inventory[num6] = new Item();
									Main.player[num5].inventory[num6].netDefaults(type);
									Main.player[num5].inventory[num6].stack = stack;
									Main.player[num5].inventory[num6].Prefix((int)b3);
								}
								else
								{
									Main.player[num5].armor[num6 - 48 - 1] = new Item();
									Main.player[num5].armor[num6 - 48 - 1].netDefaults(type);
									Main.player[num5].armor[num6 - 48 - 1].stack = stack;
									Main.player[num5].armor[num6 - 48 - 1].Prefix((int)b3);
								}
								if (Main.netMode == 2 && num5 == this.whoAmI)
								{
									NetMessage.SendData(5, -1, this.whoAmI, "", num5, (float)num6, (float)b3, 0f, 0);
								}
								return;
							}
						}
						if (b == 6)
						{
							if (Main.netMode == 2)
							{
								if (Netplay.serverSock[this.whoAmI].state == 1)
								{
									Netplay.serverSock[this.whoAmI].state = 2;
								}
								NetMessage.SendData(7, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0);
								return;
							}
						}
						else
						{
							if (b == 7)
							{
								if (Main.netMode == 1)
								{
									Main.time = (double)BitConverter.ToInt32(this.readBuffer, num);
									num += 4;
									Main.dayTime = false;
									if (this.readBuffer[num] == 1)
									{
										Main.dayTime = true;
									}
									num++;
									Main.moonPhase = (int)this.readBuffer[num];
									num++;
									int num7 = (int)this.readBuffer[num];
									num++;
									if (num7 == 1)
									{
										Main.bloodMoon = true;
									}
									else
									{
										Main.bloodMoon = false;
									}
									Main.maxTilesX = BitConverter.ToInt32(this.readBuffer, num);
									num += 4;
									Main.maxTilesY = BitConverter.ToInt32(this.readBuffer, num);
									num += 4;
									Main.spawnTileX = BitConverter.ToInt32(this.readBuffer, num);
									num += 4;
									Main.spawnTileY = BitConverter.ToInt32(this.readBuffer, num);
									num += 4;
									Main.worldSurface = (double)BitConverter.ToInt32(this.readBuffer, num);
									num += 4;
									Main.rockLayer = (double)BitConverter.ToInt32(this.readBuffer, num);
									num += 4;
									Main.worldID = BitConverter.ToInt32(this.readBuffer, num);
									num += 4;
									byte b4 = this.readBuffer[num];
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
									Main.worldName = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
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
										int num8 = BitConverter.ToInt32(this.readBuffer, num);
										num += 4;
										int num9 = BitConverter.ToInt32(this.readBuffer, num);
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
										if (Netplay.serverSock[this.whoAmI].state == 2)
										{
											Netplay.serverSock[this.whoAmI].state = 3;
										}
										NetMessage.SendData(9, this.whoAmI, -1, "Receiving tile data", num10, 0f, 0f, 0f, 0);
										Netplay.serverSock[this.whoAmI].statusText2 = "is receiving tile data";
										Netplay.serverSock[this.whoAmI].statusMax += num10;
										int sectionX = Netplay.GetSectionX(Main.spawnTileX);
										int sectionY = Netplay.GetSectionY(Main.spawnTileY);
										for (int m = sectionX - 2; m < sectionX + 3; m++)
										{
											for (int n = sectionY - 1; n < sectionY + 2; n++)
											{
												NetMessage.SendSection(this.whoAmI, m, n);
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
													NetMessage.SendSection(this.whoAmI, num11, num12);
												}
											}
											NetMessage.SendData(11, this.whoAmI, -1, "", num8 - 2, (float)(num9 - 1), (float)(num8 + 2), (float)(num9 + 1), 0);
										}
										NetMessage.SendData(11, this.whoAmI, -1, "", sectionX - 2, (float)(sectionY - 1), (float)(sectionX + 2), (float)(sectionY + 1), 0);
										for (int num13 = 0; num13 < 200; num13++)
										{
											if (Main.item[num13].active)
											{
												NetMessage.SendData(21, this.whoAmI, -1, "", num13, 0f, 0f, 0f, 0);
												NetMessage.SendData(22, this.whoAmI, -1, "", num13, 0f, 0f, 0f, 0);
											}
										}
										for (int num14 = 0; num14 < 200; num14++)
										{
											if (Main.npc[num14].active)
											{
												NetMessage.SendData(23, this.whoAmI, -1, "", num14, 0f, 0f, 0f, 0);
											}
										}
										NetMessage.SendData(49, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0);
										NetMessage.SendData(57, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0);
										NetMessage.SendData(56, this.whoAmI, -1, "", 17, 0f, 0f, 0f, 0);
										NetMessage.SendData(56, this.whoAmI, -1, "", 18, 0f, 0f, 0f, 0);
										NetMessage.SendData(56, this.whoAmI, -1, "", 19, 0f, 0f, 0f, 0);
										NetMessage.SendData(56, this.whoAmI, -1, "", 20, 0f, 0f, 0f, 0);
										NetMessage.SendData(56, this.whoAmI, -1, "", 22, 0f, 0f, 0f, 0);
										NetMessage.SendData(56, this.whoAmI, -1, "", 38, 0f, 0f, 0f, 0);
										NetMessage.SendData(56, this.whoAmI, -1, "", 54, 0f, 0f, 0f, 0);
										NetMessage.SendData(56, this.whoAmI, -1, "", 107, 0f, 0f, 0f, 0);
										NetMessage.SendData(56, this.whoAmI, -1, "", 108, 0f, 0f, 0f, 0);
										NetMessage.SendData(56, this.whoAmI, -1, "", 124, 0f, 0f, 0f, 0);
										return;
									}
								}
								else
								{
									if (b == 9)
									{
										if (Main.netMode == 1)
										{
											int num15 = BitConverter.ToInt32(this.readBuffer, start + 1);
											string string2 = Encoding.ASCII.GetString(this.readBuffer, start + 5, length - 5);
											Netplay.clientSock.statusMax += num15;
											Netplay.clientSock.statusText = string2;
											return;
										}
									}
									else
									{
										if (b == 10 && Main.netMode == 1)
										{
											short num16 = BitConverter.ToInt16(this.readBuffer, start + 1);
											int num17 = BitConverter.ToInt32(this.readBuffer, start + 3);
											int num18 = BitConverter.ToInt32(this.readBuffer, start + 7);
											num = start + 11;
											for (int num19 = num17; num19 < num17 + (int)num16; num19++)
											{
												byte b5 = this.readBuffer[num];
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
												byte arg_1551_0 = (byte)(b5 & 2);
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
													int type2 = (int)Main.tile[num19, num18].type;
													Main.tile[num19, num18].type = this.readBuffer[num];
													num++;
													if (Main.tileFrameImportant[(int)Main.tile[num19, num18].type])
													{
														Main.tile[num19, num18].frameX = BitConverter.ToInt16(this.readBuffer, num);
														num += 2;
														Main.tile[num19, num18].frameY = BitConverter.ToInt16(this.readBuffer, num);
														num += 2;
													}
													else
													{
														if (!active || (int)Main.tile[num19, num18].type != type2)
														{
															Main.tile[num19, num18].frameX = -1;
															Main.tile[num19, num18].frameY = -1;
														}
													}
												}
												if (Main.tile[num19, num18].wall > 0)
												{
													Main.tile[num19, num18].wall = this.readBuffer[num];
													num++;
												}
												if (Main.tile[num19, num18].liquid > 0)
												{
													Main.tile[num19, num18].liquid = this.readBuffer[num];
													num++;
													byte b6 = this.readBuffer[num];
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
												short num20 = BitConverter.ToInt16(this.readBuffer, num);
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
													if (Main.tileFrameImportant[(int)Main.tile[num21, num18].type])
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
												NetMessage.SendData((int)b, -1, this.whoAmI, "", (int)num16, (float)num17, (float)num18, 0f, 0);
												return;
											}
										}
										else
										{
											if (b == 11)
											{
												if (Main.netMode == 1)
												{
													int startX = (int)BitConverter.ToInt16(this.readBuffer, num);
													num += 4;
													int startY = (int)BitConverter.ToInt16(this.readBuffer, num);
													num += 4;
													int endX = (int)BitConverter.ToInt16(this.readBuffer, num);
													num += 4;
													int endY = (int)BitConverter.ToInt16(this.readBuffer, num);
													num += 4;
													WorldGen.SectionTileFrame(startX, startY, endX, endY);
													return;
												}
											}
											else
											{
												if (b == 12)
												{
													int num22 = (int)this.readBuffer[num];
													if (Main.netMode == 2)
													{
														num22 = this.whoAmI;
													}
													num++;
													Main.player[num22].SpawnX = BitConverter.ToInt32(this.readBuffer, num);
													num += 4;
													Main.player[num22].SpawnY = BitConverter.ToInt32(this.readBuffer, num);
													num += 4;
													Main.player[num22].Spawn();
													if (Main.netMode == 2 && Netplay.serverSock[this.whoAmI].state >= 3)
													{
														if (Netplay.serverSock[this.whoAmI].state == 3)
														{
															Netplay.serverSock[this.whoAmI].state = 10;
															NetMessage.greetPlayer(this.whoAmI);
															NetMessage.buffer[this.whoAmI].broadcast = true;
															NetMessage.syncPlayers();
															NetMessage.SendData(12, -1, this.whoAmI, "", this.whoAmI, 0f, 0f, 0f, 0);
															return;
														}
														NetMessage.SendData(12, -1, this.whoAmI, "", this.whoAmI, 0f, 0f, 0f, 0);
														return;
													}
												}
												else
												{
													if (b == 13)
													{
														int num23 = (int)this.readBuffer[num];
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
															num23 = this.whoAmI;
														}
														num++;
														int num24 = (int)this.readBuffer[num];
														num++;
														int selectedItem = (int)this.readBuffer[num];
														num++;
														float x = BitConverter.ToSingle(this.readBuffer, num);
														num += 4;
														float num25 = BitConverter.ToSingle(this.readBuffer, num);
														num += 4;
														float x2 = BitConverter.ToSingle(this.readBuffer, num);
														num += 4;
														float y = BitConverter.ToSingle(this.readBuffer, num);
														num += 4;
														Main.player[num23].selectedItem = selectedItem;
														Main.player[num23].position.X = x;
														Main.player[num23].position.Y = num25;
														Main.player[num23].velocity.X = x2;
														Main.player[num23].velocity.Y = y;
														Main.player[num23].oldVelocity = Main.player[num23].velocity;
														Main.player[num23].fallStart = (int)(num25 / 16f);
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
														if (Main.netMode == 2 && Netplay.serverSock[this.whoAmI].state == 10)
														{
															NetMessage.SendData(13, -1, this.whoAmI, "", num23, 0f, 0f, 0f, 0);
															return;
														}
													}
													else
													{
														if (b == 14)
														{
															if (Main.netMode == 1)
															{
																int num26 = (int)this.readBuffer[num];
																num++;
																int num27 = (int)this.readBuffer[num];
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
																	int num28 = (int)this.readBuffer[num];
																	num++;
																	if (num28 == Main.myPlayer)
																	{
																		return;
																	}
																	int statLife = (int)BitConverter.ToInt16(this.readBuffer, num);
																	num += 2;
																	int statLifeMax = (int)BitConverter.ToInt16(this.readBuffer, num);
																	if (Main.netMode == 2)
																	{
																		num28 = this.whoAmI;
																	}
																	Main.player[num28].statLife = statLife;
																	Main.player[num28].statLifeMax = statLifeMax;
																	if (Main.player[num28].statLife <= 0)
																	{
																		Main.player[num28].dead = true;
																	}
																	if (Main.netMode == 2)
																	{
																		NetMessage.SendData(16, -1, this.whoAmI, "", num28, 0f, 0f, 0f, 0);
																		return;
																	}
																}
																else
																{
																	if (b == 17)
																	{
																		byte b7 = this.readBuffer[num];
																		num++;
																		int num29 = BitConverter.ToInt32(this.readBuffer, num);
																		num += 4;
																		int num30 = BitConverter.ToInt32(this.readBuffer, num);
																		num += 4;
																		byte b8 = this.readBuffer[num];
																		num++;
																		int num31 = (int)this.readBuffer[num];
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
																					Netplay.serverSock[this.whoAmI].spamDelBlock += 1f;
																				}
																				else
																				{
																					if (b7 == 1 || b7 == 3)
																					{
																						Netplay.serverSock[this.whoAmI].spamAddBlock += 1f;
																					}
																				}
																			}
																			if (!Netplay.serverSock[this.whoAmI].tileSection[Netplay.GetSectionX(num29), Netplay.GetSectionY(num30)])
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
																				WorldGen.PlaceTile(num29, num30, (int)b8, false, true, -1, num31);
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
																						WorldGen.PlaceWall(num29, num30, (int)b8, false);
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
																			NetMessage.SendData(17, -1, this.whoAmI, "", (int)b7, (float)num29, (float)num30, (float)b8, num31);
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
																				byte b9 = this.readBuffer[num];
																				num++;
																				int num32 = BitConverter.ToInt32(this.readBuffer, num);
																				num += 4;
																				short sunModY = BitConverter.ToInt16(this.readBuffer, num);
																				num += 2;
																				short moonModY = BitConverter.ToInt16(this.readBuffer, num);
																				num += 2;
																				if (b9 == 1)
																				{
																					Main.dayTime = true;
																				}
																				else
																				{
																					Main.dayTime = false;
																				}
																				Main.time = (double)num32;
																				Main.sunModY = sunModY;
																				Main.moonModY = moonModY;
																				if (Main.netMode == 2)
																				{
																					NetMessage.SendData(18, -1, this.whoAmI, "", 0, 0f, 0f, 0f, 0);
																					return;
																				}
																			}
																		}
																		else
																		{
																			if (b == 19)
																			{
																				byte b10 = this.readBuffer[num];
																				num++;
																				int num33 = BitConverter.ToInt32(this.readBuffer, num);
																				num += 4;
																				int num34 = BitConverter.ToInt32(this.readBuffer, num);
																				num += 4;
																				int num35 = (int)this.readBuffer[num];
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
																					NetMessage.SendData(19, -1, this.whoAmI, "", (int)b10, (float)num33, (float)num34, (float)num35, 0);
																					return;
																				}
																			}
																			else
																			{
																				if (b == 20)
																				{
																					short num36 = BitConverter.ToInt16(this.readBuffer, start + 1);
																					int num37 = BitConverter.ToInt32(this.readBuffer, start + 3);
																					int num38 = BitConverter.ToInt32(this.readBuffer, start + 7);
																					num = start + 11;
																					for (int num39 = num37; num39 < num37 + (int)num36; num39++)
																					{
																						for (int num40 = num38; num40 < num38 + (int)num36; num40++)
																						{
																							byte b11 = this.readBuffer[num];
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
																							byte arg_22BD_0 = (byte)(b11 & 2);
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
																								int type3 = (int)Main.tile[num39, num40].type;
																								Main.tile[num39, num40].type = this.readBuffer[num];
																								num++;
																								if (Main.tileFrameImportant[(int)Main.tile[num39, num40].type])
																								{
																									Main.tile[num39, num40].frameX = BitConverter.ToInt16(this.readBuffer, num);
																									num += 2;
																									Main.tile[num39, num40].frameY = BitConverter.ToInt16(this.readBuffer, num);
																									num += 2;
																								}
																								else
																								{
																									if (!active2 || (int)Main.tile[num39, num40].type != type3)
																									{
																										Main.tile[num39, num40].frameX = -1;
																										Main.tile[num39, num40].frameY = -1;
																									}
																								}
																							}
																							if (Main.tile[num39, num40].wall > 0)
																							{
																								Main.tile[num39, num40].wall = this.readBuffer[num];
																								num++;
																							}
																							if (Main.tile[num39, num40].liquid > 0)
																							{
																								Main.tile[num39, num40].liquid = this.readBuffer[num];
																								num++;
																								byte b12 = this.readBuffer[num];
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
																					WorldGen.RangeFrame(num37, num38, num37 + (int)num36, num38 + (int)num36);
																					if (Main.netMode == 2)
																					{
																						NetMessage.SendData((int)b, -1, this.whoAmI, "", (int)num36, (float)num37, (float)num38, 0f, 0);
																						return;
																					}
																				}
																				else
																				{
																					if (b == 21)
																					{
																						short num41 = BitConverter.ToInt16(this.readBuffer, num);
																						num += 2;
																						float num42 = BitConverter.ToSingle(this.readBuffer, num);
																						num += 4;
																						float num43 = BitConverter.ToSingle(this.readBuffer, num);
																						num += 4;
																						float x3 = BitConverter.ToSingle(this.readBuffer, num);
																						num += 4;
																						float y2 = BitConverter.ToSingle(this.readBuffer, num);
																						num += 4;
																						byte stack2 = this.readBuffer[num];
																						num++;
																						byte pre = this.readBuffer[num];
																						num++;
																						short num44 = BitConverter.ToInt16(this.readBuffer, num);
																						if (Main.netMode == 1)
																						{
																							if (num44 == 0)
																							{
																								Main.item[(int)num41].active = false;
																								return;
																							}
																							Main.item[(int)num41].netDefaults((int)num44);
																							Main.item[(int)num41].Prefix((int)pre);
																							Main.item[(int)num41].stack = (int)stack2;
																							Main.item[(int)num41].position.X = num42;
																							Main.item[(int)num41].position.Y = num43;
																							Main.item[(int)num41].velocity.X = x3;
																							Main.item[(int)num41].velocity.Y = y2;
																							Main.item[(int)num41].active = true;
																							Main.item[(int)num41].wet = Collision.WetCollision(Main.item[(int)num41].position, Main.item[(int)num41].width, Main.item[(int)num41].height);
																							return;
																						}
																						else
																						{
																							if (num44 == 0)
																							{
																								if (num41 < 200)
																								{
																									Main.item[(int)num41].active = false;
																									NetMessage.SendData(21, -1, -1, "", (int)num41, 0f, 0f, 0f, 0);
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
																									Item item = new Item();
																									item.netDefaults((int)num44);
																									num41 = (short)Item.NewItem((int)num42, (int)num43, item.width, item.height, item.type, (int)stack2, true, 0);
																								}
																								Main.item[(int)num41].netDefaults((int)num44);
																								Main.item[(int)num41].Prefix((int)pre);
																								Main.item[(int)num41].stack = (int)stack2;
																								Main.item[(int)num41].position.X = num42;
																								Main.item[(int)num41].position.Y = num43;
																								Main.item[(int)num41].velocity.X = x3;
																								Main.item[(int)num41].velocity.Y = y2;
																								Main.item[(int)num41].active = true;
																								Main.item[(int)num41].owner = Main.myPlayer;
																								if (flag5)
																								{
																									NetMessage.SendData(21, -1, -1, "", (int)num41, 0f, 0f, 0f, 0);
																									Main.item[(int)num41].ownIgnore = this.whoAmI;
																									Main.item[(int)num41].ownTime = 100;
																									Main.item[(int)num41].FindOwner((int)num41);
																									return;
																								}
																								NetMessage.SendData(21, -1, this.whoAmI, "", (int)num41, 0f, 0f, 0f, 0);
																								return;
																							}
																						}
																					}
																					else
																					{
																						if (b == 22)
																						{
																							short num45 = BitConverter.ToInt16(this.readBuffer, num);
																							num += 2;
																							byte b13 = this.readBuffer[num];
																							if (Main.netMode == 2 && Main.item[(int)num45].owner != this.whoAmI)
																							{
																								return;
																							}
																							Main.item[(int)num45].owner = (int)b13;
																							if ((int)b13 == Main.myPlayer)
																							{
																								Main.item[(int)num45].keepTime = 15;
																							}
																							else
																							{
																								Main.item[(int)num45].keepTime = 0;
																							}
																							if (Main.netMode == 2)
																							{
																								Main.item[(int)num45].owner = 255;
																								Main.item[(int)num45].keepTime = 15;
																								NetMessage.SendData(22, -1, -1, "", (int)num45, 0f, 0f, 0f, 0);
																								return;
																							}
																						}
																						else
																						{
																							if (b == 23 && Main.netMode == 1)
																							{
																								short num46 = BitConverter.ToInt16(this.readBuffer, num);
																								num += 2;
																								float x4 = BitConverter.ToSingle(this.readBuffer, num);
																								num += 4;
																								float y3 = BitConverter.ToSingle(this.readBuffer, num);
																								num += 4;
																								float x5 = BitConverter.ToSingle(this.readBuffer, num);
																								num += 4;
																								float y4 = BitConverter.ToSingle(this.readBuffer, num);
																								num += 4;
																								int target = (int)BitConverter.ToInt16(this.readBuffer, num);
																								num += 2;
																								int direction2 = (int)(this.readBuffer[num] - 1);
																								num++;
																								int directionY = (int)(this.readBuffer[num] - 1);
																								num++;
																								int num47 = BitConverter.ToInt32(this.readBuffer, num);
																								num += 4;
																								float[] array = new float[NPC.maxAI];
																								for (int num48 = 0; num48 < NPC.maxAI; num48++)
																								{
																									array[num48] = BitConverter.ToSingle(this.readBuffer, num);
																									num += 4;
																								}
																								int num49 = (int)BitConverter.ToInt16(this.readBuffer, num);
																								if (!Main.npc[(int)num46].active || Main.npc[(int)num46].netID != num49)
																								{
																									Main.npc[(int)num46].active = true;
																									Main.npc[(int)num46].netDefaults(num49);
																								}
																								Main.npc[(int)num46].position.X = x4;
																								Main.npc[(int)num46].position.Y = y3;
																								Main.npc[(int)num46].velocity.X = x5;
																								Main.npc[(int)num46].velocity.Y = y4;
																								Main.npc[(int)num46].target = target;
																								Main.npc[(int)num46].direction = direction2;
																								Main.npc[(int)num46].directionY = directionY;
																								Main.npc[(int)num46].life = num47;
																								if (num47 <= 0)
																								{
																									Main.npc[(int)num46].active = false;
																								}
																								for (int num50 = 0; num50 < NPC.maxAI; num50++)
																								{
																									Main.npc[(int)num46].ai[num50] = array[num50];
																								}
																								return;
																							}
																							if (b == 24)
																							{
																								short num51 = BitConverter.ToInt16(this.readBuffer, num);
																								num += 2;
																								byte b14 = this.readBuffer[num];
																								if (Main.netMode == 2)
																								{
																									b14 = (byte)this.whoAmI;
																								}
																								Main.npc[(int)num51].StrikeNPC(Main.player[(int)b14].inventory[Main.player[(int)b14].selectedItem].damage, Main.player[(int)b14].inventory[Main.player[(int)b14].selectedItem].knockBack, Main.player[(int)b14].direction, false, false);
																								if (Main.netMode == 2)
																								{
																									NetMessage.SendData(24, -1, this.whoAmI, "", (int)num51, (float)b14, 0f, 0f, 0);
																									NetMessage.SendData(23, -1, -1, "", (int)num51, 0f, 0f, 0f, 0);
																									return;
																								}
																							}
																							else
																							{
																								if (b == 25)
																								{
																									int num52 = (int)this.readBuffer[start + 1];
																									if (Main.netMode == 2)
																									{
																										num52 = this.whoAmI;
																									}
																									byte b15 = this.readBuffer[start + 2];
																									byte b16 = this.readBuffer[start + 3];
																									byte b17 = this.readBuffer[start + 4];
																									if (Main.netMode == 2)
																									{
																										b15 = 255;
																										b16 = 255;
																										b17 = 255;
																									}
																									string string3 = Encoding.ASCII.GetString(this.readBuffer, start + 5, length - 5);
																									if (Main.netMode == 1)
																									{
																										string newText = string3;
																										if (num52 < 255)
																										{
																											newText = "<" + Main.player[num52].name + "> " + string3;
																											Main.player[num52].chatText = string3;
																											Main.player[num52].chatShowTime = Main.chatLength / 2;
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
																											NetMessage.SendData(25, this.whoAmI, -1, "Current players: " + text3 + ".", 255, 255f, 240f, 20f, 0);
																											return;
																										}
																										if (text2.Length >= 4 && text2.Substring(0, 4) == "/me ")
																										{
																											NetMessage.SendData(25, -1, -1, "*" + Main.player[this.whoAmI].name + " " + string3.Substring(4), 255, 200f, 100f, 0f, 0);
																											return;
																										}
																										if (text2 == "/roll")
																										{
																											NetMessage.SendData(25, -1, -1, string.Concat(new object[]
																											{
																												"*", 
																												Main.player[this.whoAmI].name, 
																												" rolls a ", 
																												Main.rand.Next(1, 101)
																											}), 255, 255f, 240f, 20f, 0);
																											return;
																										}
																										if (text2.Length >= 3 && text2.Substring(0, 3) == "/p ")
																										{
																											if (Main.player[this.whoAmI].team != 0)
																											{
																												for (int num54 = 0; num54 < 255; num54++)
																												{
																													if (Main.player[num54].team == Main.player[this.whoAmI].team)
																													{
																														NetMessage.SendData(25, num54, -1, string3.Substring(3), num52, (float)Main.teamColor[Main.player[this.whoAmI].team].R, (float)Main.teamColor[Main.player[this.whoAmI].team].G, (float)Main.teamColor[Main.player[this.whoAmI].team].B, 0);
																													}
																												}
																												return;
																											}
																											NetMessage.SendData(25, this.whoAmI, -1, "You are not in a party!", 255, 255f, 240f, 20f, 0);
																											return;
																										}
																										else
																										{
																											if (Main.player[this.whoAmI].difficulty == 2)
																											{
																												b15 = Main.hcColor.R;
																												b16 = Main.hcColor.G;
																												b17 = Main.hcColor.B;
																											}
																											else
																											{
																												if (Main.player[this.whoAmI].difficulty == 1)
																												{
																													b15 = Main.mcColor.R;
																													b16 = Main.mcColor.G;
																													b17 = Main.mcColor.B;
																												}
																											}
																											NetMessage.SendData(25, -1, -1, string3, num52, (float)b15, (float)b16, (float)b17, 0);
																											if (Main.dedServ)
																											{
																												Console.WriteLine("<" + Main.player[this.whoAmI].name + "> " + string3);
																												return;
																											}
																										}
																									}
																								}
																								else
																								{
																									if (b == 26)
																									{
																										byte b18 = this.readBuffer[num];
																										if (Main.netMode == 2 && this.whoAmI != (int)b18 && (!Main.player[(int)b18].hostile || !Main.player[this.whoAmI].hostile))
																										{
																											return;
																										}
																										num++;
																										int num55 = (int)(this.readBuffer[num] - 1);
																										num++;
																										short num56 = BitConverter.ToInt16(this.readBuffer, num);
																										num += 2;
																										byte b19 = this.readBuffer[num];
																										num++;
																										bool pvp = false;
																										byte b20 = this.readBuffer[num];
																										num++;
																										bool crit = false;
																										string string4 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
																										if (b19 != 0)
																										{
																											pvp = true;
																										}
																										if (b20 != 0)
																										{
																											crit = true;
																										}
																										Main.player[(int)b18].Hurt((int)num56, num55, pvp, true, string4, crit);
																										if (Main.netMode == 2)
																										{
																											NetMessage.SendData(26, -1, this.whoAmI, string4, (int)b18, (float)num55, (float)num56, (float)b19, 0);
																											return;
																										}
																									}
																									else
																									{
																										if (b == 27)
																										{
																											short num57 = BitConverter.ToInt16(this.readBuffer, num);
																											num += 2;
																											float x6 = BitConverter.ToSingle(this.readBuffer, num);
																											num += 4;
																											float y5 = BitConverter.ToSingle(this.readBuffer, num);
																											num += 4;
																											float x7 = BitConverter.ToSingle(this.readBuffer, num);
																											num += 4;
																											float y6 = BitConverter.ToSingle(this.readBuffer, num);
																											num += 4;
																											float knockBack = BitConverter.ToSingle(this.readBuffer, num);
																											num += 4;
																											short damage = BitConverter.ToInt16(this.readBuffer, num);
																											num += 2;
																											byte b21 = this.readBuffer[num];
																											num++;
																											byte b22 = this.readBuffer[num];
																											num++;
																											float[] array2 = new float[Projectile.maxAI];
																											for (int num58 = 0; num58 < Projectile.maxAI; num58++)
																											{
																												array2[num58] = BitConverter.ToSingle(this.readBuffer, num);
																												num += 4;
																											}
																											int num59 = 1000;
																											for (int num60 = 0; num60 < 1000; num60++)
																											{
																												if (Main.projectile[num60].owner == (int)b21 && Main.projectile[num60].identity == (int)num57 && Main.projectile[num60].active)
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
																											if (!Main.projectile[num59].active || Main.projectile[num59].type != (int)b22)
																											{
																												Main.projectile[num59].SetDefaults((int)b22);
																												if (Main.netMode == 2)
																												{
																													Netplay.serverSock[this.whoAmI].spamProjectile += 1f;
																												}
																											}
																											Main.projectile[num59].identity = (int)num57;
																											Main.projectile[num59].position.X = x6;
																											Main.projectile[num59].position.Y = y5;
																											Main.projectile[num59].velocity.X = x7;
																											Main.projectile[num59].velocity.Y = y6;
																											Main.projectile[num59].damage = (int)damage;
																											Main.projectile[num59].type = (int)b22;
																											Main.projectile[num59].owner = (int)b21;
																											Main.projectile[num59].knockBack = knockBack;
																											for (int num62 = 0; num62 < Projectile.maxAI; num62++)
																											{
																												Main.projectile[num59].ai[num62] = array2[num62];
																											}
																											if (Main.netMode == 2)
																											{
																												NetMessage.SendData(27, -1, this.whoAmI, "", num59, 0f, 0f, 0f, 0);
																												return;
																											}
																										}
																										else
																										{
																											if (b == 28)
																											{
																												short num63 = BitConverter.ToInt16(this.readBuffer, num);
																												num += 2;
																												short num64 = BitConverter.ToInt16(this.readBuffer, num);
																												num += 2;
																												float num65 = BitConverter.ToSingle(this.readBuffer, num);
																												num += 4;
																												int num66 = (int)(this.readBuffer[num] - 1);
																												num++;
																												int num67 = (int)this.readBuffer[num];
																												if (num64 >= 0)
																												{
																													if (num67 == 1)
																													{
																														Main.npc[(int)num63].StrikeNPC((int)num64, num65, num66, true, false);
																													}
																													else
																													{
																														Main.npc[(int)num63].StrikeNPC((int)num64, num65, num66, false, false);
																													}
																												}
																												else
																												{
																													Main.npc[(int)num63].life = 0;
																													Main.npc[(int)num63].HitEffect(0, 10.0);
																													Main.npc[(int)num63].active = false;
																												}
																												if (Main.netMode == 2)
																												{
																													if (Main.npc[(int)num63].life <= 0)
																													{
																														NetMessage.SendData(28, -1, this.whoAmI, "", (int)num63, (float)num64, num65, (float)num66, num67);
																														NetMessage.SendData(23, -1, -1, "", (int)num63, 0f, 0f, 0f, 0);
																														return;
																													}
																													NetMessage.SendData(28, -1, this.whoAmI, "", (int)num63, (float)num64, num65, (float)num66, num67);
																													Main.npc[(int)num63].netUpdate = true;
																													return;
																												}
																											}
																											else
																											{
																												if (b == 29)
																												{
																													short num68 = BitConverter.ToInt16(this.readBuffer, num);
																													num += 2;
																													byte b23 = this.readBuffer[num];
																													if (Main.netMode == 2)
																													{
																														b23 = (byte)this.whoAmI;
																													}
																													for (int num69 = 0; num69 < 1000; num69++)
																													{
																														if (Main.projectile[num69].owner == (int)b23 && Main.projectile[num69].identity == (int)num68 && Main.projectile[num69].active)
																														{
																															Main.projectile[num69].Kill();
																															break;
																														}
																													}
																													if (Main.netMode == 2)
																													{
																														NetMessage.SendData(29, -1, this.whoAmI, "", (int)num68, (float)b23, 0f, 0f, 0);
																														return;
																													}
																												}
																												else
																												{
																													if (b == 30)
																													{
																														byte b24 = this.readBuffer[num];
																														if (Main.netMode == 2)
																														{
																															b24 = (byte)this.whoAmI;
																														}
																														num++;
																														byte b25 = this.readBuffer[num];
																														if (b25 == 1)
																														{
																															Main.player[(int)b24].hostile = true;
																														}
																														else
																														{
																															Main.player[(int)b24].hostile = false;
																														}
																														if (Main.netMode == 2)
																														{
																															NetMessage.SendData(30, -1, this.whoAmI, "", (int)b24, 0f, 0f, 0f, 0);
																															string str = " has enabled PvP!";
																															if (b25 == 0)
																															{
																																str = " has disabled PvP!";
																															}
																															NetMessage.SendData(25, -1, -1, Main.player[(int)b24].name + str, 255, (float)Main.teamColor[Main.player[(int)b24].team].R, (float)Main.teamColor[Main.player[(int)b24].team].G, (float)Main.teamColor[Main.player[(int)b24].team].B, 0);
																															return;
																														}
																													}
																													else
																													{
																														if (b == 31)
																														{
																															if (Main.netMode == 2)
																															{
																																int x8 = BitConverter.ToInt32(this.readBuffer, num);
																																num += 4;
																																int y7 = BitConverter.ToInt32(this.readBuffer, num);
																																num += 4;
																																int num70 = Chest.FindChest(x8, y7);
																																if (num70 > -1 && Chest.UsingChest(num70) == -1)
																																{
																																	for (int num71 = 0; num71 < Chest.maxItems; num71++)
																																	{
																																		NetMessage.SendData(32, this.whoAmI, -1, "", num70, (float)num71, 0f, 0f, 0);
																																	}
																																	NetMessage.SendData(33, this.whoAmI, -1, "", num70, 0f, 0f, 0f, 0);
																																	Main.player[this.whoAmI].chest = num70;
																																	return;
																																}
																															}
																														}
																														else
																														{
																															if (b == 32)
																															{
																																int num72 = (int)BitConverter.ToInt16(this.readBuffer, num);
																																num += 2;
																																int num73 = (int)this.readBuffer[num];
																																num++;
																																int stack3 = (int)this.readBuffer[num];
																																num++;
																																int pre2 = (int)this.readBuffer[num];
																																num++;
																																int type4 = (int)BitConverter.ToInt16(this.readBuffer, num);
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
																																int num74 = (int)BitConverter.ToInt16(this.readBuffer, num);
																																num += 2;
																																int chestX = BitConverter.ToInt32(this.readBuffer, num);
																																num += 4;
																																int chestY = BitConverter.ToInt32(this.readBuffer, num);
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
																																Main.player[this.whoAmI].chest = num74;
																																return;
																															}
																															else
																															{
																																if (b == 34)
																																{
																																	if (Main.netMode == 2)
																																	{
																																		int num75 = BitConverter.ToInt32(this.readBuffer, num);
																																		num += 4;
																																		int num76 = BitConverter.ToInt32(this.readBuffer, num);
																																		if (Main.tile[num75, num76].type == 21)
																																		{
																																			WorldGen.KillTile(num75, num76, false, false, false);
																																			if (!Main.tile[num75, num76].active)
																																			{
																																				NetMessage.SendData(17, -1, -1, "", 0, (float)num75, (float)num76, 0f, 0);
																																				return;
																																			}
																																		}
																																	}
																																}
																																else
																																{
																																	if (b == 35)
																																	{
																																		int num77 = (int)this.readBuffer[num];
																																		if (Main.netMode == 2)
																																		{
																																			num77 = this.whoAmI;
																																		}
																																		num++;
																																		int num78 = (int)BitConverter.ToInt16(this.readBuffer, num);
																																		num += 2;
																																		if (num77 != Main.myPlayer)
																																		{
																																			Main.player[num77].HealEffect(num78);
																																		}
																																		if (Main.netMode == 2)
																																		{
																																			NetMessage.SendData(35, -1, this.whoAmI, "", num77, (float)num78, 0f, 0f, 0);
																																			return;
																																		}
																																	}
																																	else
																																	{
																																		if (b == 36)
																																		{
																																			int num79 = (int)this.readBuffer[num];
																																			if (Main.netMode == 2)
																																			{
																																				num79 = this.whoAmI;
																																			}
																																			num++;
																																			int num80 = (int)this.readBuffer[num];
																																			num++;
																																			int num81 = (int)this.readBuffer[num];
																																			num++;
																																			int num82 = (int)this.readBuffer[num];
																																			num++;
																																			int num83 = (int)this.readBuffer[num];
																																			num++;
																																			int num84 = (int)this.readBuffer[num];
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
																																				NetMessage.SendData(36, -1, this.whoAmI, "", num79, 0f, 0f, 0f, 0);
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
																																						string string5 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
																																						if (string5 == Netplay.password)
																																						{
																																							Netplay.serverSock[this.whoAmI].state = 1;
																																							NetMessage.SendData(3, this.whoAmI, -1, "", 0, 0f, 0f, 0f, 0);
																																							return;
																																						}
																																						NetMessage.SendData(2, this.whoAmI, -1, "Incorrect password.", 0, 0f, 0f, 0f, 0);
																																						return;
																																					}
																																				}
																																				else
																																				{
																																					if (b == 39 && Main.netMode == 1)
																																					{
																																						short num85 = BitConverter.ToInt16(this.readBuffer, num);
																																						Main.item[(int)num85].owner = 255;
																																						NetMessage.SendData(22, -1, -1, "", (int)num85, 0f, 0f, 0f, 0);
																																						return;
																																					}
																																					if (b == 40)
																																					{
																																						byte b26 = this.readBuffer[num];
																																						if (Main.netMode == 2)
																																						{
																																							b26 = (byte)this.whoAmI;
																																						}
																																						num++;
																																						int talkNPC = (int)BitConverter.ToInt16(this.readBuffer, num);
																																						num += 2;
																																						Main.player[(int)b26].talkNPC = talkNPC;
																																						if (Main.netMode == 2)
																																						{
																																							NetMessage.SendData(40, -1, this.whoAmI, "", (int)b26, 0f, 0f, 0f, 0);
																																							return;
																																						}
																																					}
																																					else
																																					{
																																						if (b == 41)
																																						{
																																							byte b27 = this.readBuffer[num];
																																							if (Main.netMode == 2)
																																							{
																																								b27 = (byte)this.whoAmI;
																																							}
																																							num++;
																																							float itemRotation = BitConverter.ToSingle(this.readBuffer, num);
																																							num += 4;
																																							int itemAnimation = (int)BitConverter.ToInt16(this.readBuffer, num);
																																							Main.player[(int)b27].itemRotation = itemRotation;
																																							Main.player[(int)b27].itemAnimation = itemAnimation;
																																							Main.player[(int)b27].channel = Main.player[(int)b27].inventory[Main.player[(int)b27].selectedItem].channel;
																																							if (Main.netMode == 2)
																																							{
																																								NetMessage.SendData(41, -1, this.whoAmI, "", (int)b27, 0f, 0f, 0f, 0);
																																								return;
																																							}
																																						}
																																						else
																																						{
																																							if (b == 42)
																																							{
																																								int num86 = (int)this.readBuffer[num];
																																								if (Main.netMode == 2)
																																								{
																																									num86 = this.whoAmI;
																																								}
																																								num++;
																																								int statMana = (int)BitConverter.ToInt16(this.readBuffer, num);
																																								num += 2;
																																								int statManaMax = (int)BitConverter.ToInt16(this.readBuffer, num);
																																								if (Main.netMode == 2)
																																								{
																																									num86 = this.whoAmI;
																																								}
																																								Main.player[num86].statMana = statMana;
																																								Main.player[num86].statManaMax = statManaMax;
																																								if (Main.netMode == 2)
																																								{
																																									NetMessage.SendData(42, -1, this.whoAmI, "", num86, 0f, 0f, 0f, 0);
																																									return;
																																								}
																																							}
																																							else
																																							{
																																								if (b == 43)
																																								{
																																									int num87 = (int)this.readBuffer[num];
																																									if (Main.netMode == 2)
																																									{
																																										num87 = this.whoAmI;
																																									}
																																									num++;
																																									int num88 = (int)BitConverter.ToInt16(this.readBuffer, num);
																																									num += 2;
																																									if (num87 != Main.myPlayer)
																																									{
																																										Main.player[num87].ManaEffect(num88);
																																									}
																																									if (Main.netMode == 2)
																																									{
																																										NetMessage.SendData(43, -1, this.whoAmI, "", num87, (float)num88, 0f, 0f, 0);
																																										return;
																																									}
																																								}
																																								else
																																								{
																																									if (b == 44)
																																									{
																																										byte b28 = this.readBuffer[num];
																																										if ((int)b28 == Main.myPlayer)
																																										{
																																											return;
																																										}
																																										if (Main.netMode == 2)
																																										{
																																											b28 = (byte)this.whoAmI;
																																										}
																																										num++;
																																										int num89 = (int)(this.readBuffer[num] - 1);
																																										num++;
																																										short num90 = BitConverter.ToInt16(this.readBuffer, num);
																																										num += 2;
																																										byte b29 = this.readBuffer[num];
																																										num++;
																																										string string6 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
																																										bool pvp2 = false;
																																										if (b29 != 0)
																																										{
																																											pvp2 = true;
																																										}
																																										Main.player[(int)b28].KillMe((double)num90, num89, pvp2, string6);
																																										if (Main.netMode == 2)
																																										{
																																											NetMessage.SendData(44, -1, this.whoAmI, string6, (int)b28, (float)num89, (float)num90, (float)b29, 0);
																																											return;
																																										}
																																									}
																																									else
																																									{
																																										if (b == 45)
																																										{
																																											int num91 = (int)this.readBuffer[num];
																																											if (Main.netMode == 2)
																																											{
																																												num91 = this.whoAmI;
																																											}
																																											num++;
																																											int num92 = (int)this.readBuffer[num];
																																											num++;
																																											int team = Main.player[num91].team;
																																											Main.player[num91].team = num92;
																																											if (Main.netMode == 2)
																																											{
																																												NetMessage.SendData(45, -1, this.whoAmI, "", num91, 0f, 0f, 0f, 0);
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
																																													if (num93 == this.whoAmI || (team > 0 && Main.player[num93].team == team) || (num92 > 0 && Main.player[num93].team == num92))
																																													{
																																														NetMessage.SendData(25, num93, -1, Main.player[num91].name + str2, 255, (float)Main.teamColor[num92].R, (float)Main.teamColor[num92].G, (float)Main.teamColor[num92].B, 0);
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
																																													int i2 = BitConverter.ToInt32(this.readBuffer, num);
																																													num += 4;
																																													int j2 = BitConverter.ToInt32(this.readBuffer, num);
																																													num += 4;
																																													int num94 = Sign.ReadSign(i2, j2);
																																													if (num94 >= 0)
																																													{
																																														NetMessage.SendData(47, this.whoAmI, -1, "", num94, 0f, 0f, 0f, 0);
																																														return;
																																													}
																																												}
																																											}
																																											else
																																											{
																																												if (b == 47)
																																												{
																																													int num95 = (int)BitConverter.ToInt16(this.readBuffer, num);
																																													num += 2;
																																													int x9 = BitConverter.ToInt32(this.readBuffer, num);
																																													num += 4;
																																													int y8 = BitConverter.ToInt32(this.readBuffer, num);
																																													num += 4;
																																													string string7 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
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
																																														int num96 = BitConverter.ToInt32(this.readBuffer, num);
																																														num += 4;
																																														int num97 = BitConverter.ToInt32(this.readBuffer, num);
																																														num += 4;
																																														byte liquid = this.readBuffer[num];
																																														num++;
																																														byte b30 = this.readBuffer[num];
																																														num++;
																																														if (Main.netMode == 2 && Netplay.spamCheck)
																																														{
																																															int num98 = this.whoAmI;
																																															int num99 = (int)(Main.player[num98].position.X + (float)(Main.player[num98].width / 2));
																																															int num100 = (int)(Main.player[num98].position.Y + (float)(Main.player[num98].height / 2));
																																															int num101 = 10;
																																															int num102 = num99 - num101;
																																															int num103 = num99 + num101;
																																															int num104 = num100 - num101;
																																															int num105 = num100 + num101;
																																															if (num99 < num102 || num99 > num103 || num100 < num104 || num100 > num105)
																																															{
																																																NetMessage.BootPlayer(this.whoAmI, "Cheating attempt detected: Liquid spam");
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
																																															int num106 = (int)this.readBuffer[num];
																																															num++;
																																															if (Main.netMode == 2)
																																															{
																																																num106 = this.whoAmI;
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
																																																Main.player[num106].buffType[num107] = (int)this.readBuffer[num];
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
																																																NetMessage.SendData(50, -1, this.whoAmI, "", num106, 0f, 0f, 0f, 0);
																																																return;
																																															}
																																														}
																																														else
																																														{
																																															if (b == 51)
																																															{
																																																byte b31 = this.readBuffer[num];
																																																num++;
																																																byte b32 = this.readBuffer[num];
																																																if (b32 == 1)
																																																{
																																																	NPC.SpawnSkeletron();
																																																	return;
																																																}
																																																if (b32 == 2)
																																																{
																																																	if (Main.netMode != 2)
																																																	{
																																																		Main.PlaySound(2, (int)Main.player[(int)b31].position.X, (int)Main.player[(int)b31].position.Y, 1);
																																																		return;
																																																	}
																																																	if (Main.netMode == 2)
																																																	{
																																																		NetMessage.SendData(51, -1, this.whoAmI, "", (int)b31, (float)b32, 0f, 0f, 0);
																																																		return;
																																																	}
																																																}
																																															}
																																															else
																																															{
																																																if (b == 52)
																																																{
																																																	byte number = this.readBuffer[num];
																																																	num++;
																																																	byte b33 = this.readBuffer[num];
																																																	num++;
																																																	int num108 = BitConverter.ToInt32(this.readBuffer, num);
																																																	num += 4;
																																																	int num109 = BitConverter.ToInt32(this.readBuffer, num);
																																																	num += 4;
																																																	if (b33 == 1)
																																																	{
																																																		Chest.Unlock(num108, num109);
																																																		if (Main.netMode == 2)
																																																		{
																																																			NetMessage.SendData(52, -1, this.whoAmI, "", (int)number, (float)b33, (float)num108, (float)num109, 0);
																																																			NetMessage.SendTileSquare(-1, num108, num109, 2);
																																																			return;
																																																		}
																																																	}
																																																}
																																																else
																																																{
																																																	if (b == 53)
																																																	{
																																																		short num110 = BitConverter.ToInt16(this.readBuffer, num);
																																																		num += 2;
																																																		byte type5 = this.readBuffer[num];
																																																		num++;
																																																		short time = BitConverter.ToInt16(this.readBuffer, num);
																																																		num += 2;
																																																		Main.npc[(int)num110].AddBuff((int)type5, (int)time, true);
																																																		if (Main.netMode == 2)
																																																		{
																																																			NetMessage.SendData(54, -1, -1, "", (int)num110, 0f, 0f, 0f, 0);
																																																			return;
																																																		}
																																																	}
																																																	else
																																																	{
																																																		if (b == 54)
																																																		{
																																																			if (Main.netMode == 1)
																																																			{
																																																				short num111 = BitConverter.ToInt16(this.readBuffer, num);
																																																				num += 2;
																																																				for (int num112 = 0; num112 < 5; num112++)
																																																				{
																																																					Main.npc[(int)num111].buffType[num112] = (int)this.readBuffer[num];
																																																					num++;
																																																					Main.npc[(int)num111].buffTime[num112] = (int)BitConverter.ToInt16(this.readBuffer, num);
																																																					num += 2;
																																																				}
																																																				return;
																																																			}
																																																		}
																																																		else
																																																		{
																																																			if (b == 55)
																																																			{
																																																				byte b34 = this.readBuffer[num];
																																																				num++;
																																																				byte b35 = this.readBuffer[num];
																																																				num++;
																																																				short num113 = BitConverter.ToInt16(this.readBuffer, num);
																																																				num += 2;
																																																				if (Main.netMode == 1 && (int)b34 == Main.myPlayer)
																																																				{
																																																					Main.player[(int)b34].AddBuff((int)b35, (int)num113, true);
																																																					return;
																																																				}
																																																				if (Main.netMode == 2)
																																																				{
																																																					NetMessage.SendData(55, (int)b34, -1, "", (int)b34, (float)b35, (float)num113, 0f, 0);
																																																					return;
																																																				}
																																																			}
																																																			else
																																																			{
																																																				if (b == 56)
																																																				{
																																																					if (Main.netMode == 1)
																																																					{
																																																						short num114 = BitConverter.ToInt16(this.readBuffer, num);
																																																						num += 2;
																																																						string string8 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
																																																						Main.chrName[(int)num114] = string8;
																																																						return;
																																																					}
																																																				}
																																																				else
																																																				{
																																																					if (b == 57)
																																																					{
																																																						if (Main.netMode == 1)
																																																						{
																																																							WorldGen.tGood = this.readBuffer[num];
																																																							num++;
																																																							WorldGen.tEvil = this.readBuffer[num];
																																																							return;
																																																						}
																																																					}
																																																					else
																																																					{
																																																						if (b == 58)
																																																						{
																																																							byte b36 = this.readBuffer[num];
																																																							if (Main.netMode == 2)
																																																							{
																																																								b36 = (byte)this.whoAmI;
																																																							}
																																																							num++;
																																																							float num115 = BitConverter.ToSingle(this.readBuffer, num);
																																																							num += 4;
																																																							if (Main.netMode == 2)
																																																							{
																																																								NetMessage.SendData(58, -1, this.whoAmI, "", this.whoAmI, num115, 0f, 0f, 0);
																																																								return;
																																																							}
																																																							Main.harpNote = num115;
																																																							int style = 26;
																																																							if (Main.player[(int)b36].inventory[Main.player[(int)b36].selectedItem].type == 507)
																																																							{
																																																								style = 35;
																																																							}
																																																							Main.PlaySound(2, (int)Main.player[(int)b36].position.X, (int)Main.player[(int)b36].position.Y, style);
																																																							return;
																																																						}
																																																						else
																																																						{
																																																							if (b == 59)
																																																							{
																																																								int num116 = BitConverter.ToInt32(this.readBuffer, num);
																																																								num += 4;
																																																								int num117 = BitConverter.ToInt32(this.readBuffer, num);
																																																								num += 4;
																																																								WorldGen.hitSwitch(num116, num117);
																																																								if (Main.netMode == 2)
																																																								{
																																																									NetMessage.SendData(59, -1, this.whoAmI, "", num116, (float)num117, 0f, 0f, 0);
																																																									return;
																																																								}
																																																							}
																																																							else
																																																							{
																																																								if (b == 60)
																																																								{
																																																									short num118 = BitConverter.ToInt16(this.readBuffer, num);
																																																									num += 2;
																																																									short num119 = BitConverter.ToInt16(this.readBuffer, num);
																																																									num += 2;
																																																									short num120 = BitConverter.ToInt16(this.readBuffer, num);
																																																									num += 2;
																																																									byte b37 = this.readBuffer[num];
																																																									num++;
																																																									bool homeless = false;
																																																									if (b37 == 1)
																																																									{
																																																										homeless = true;
																																																									}
																																																									if (Main.netMode == 1)
																																																									{
																																																										Main.npc[(int)num118].homeless = homeless;
																																																										Main.npc[(int)num118].homeTileX = (int)num119;
																																																										Main.npc[(int)num118].homeTileY = (int)num120;
																																																										return;
																																																									}
																																																									if (b37 == 0)
																																																									{
																																																										WorldGen.kickOut((int)num118);
																																																										return;
																																																									}
																																																									WorldGen.moveRoom((int)num119, (int)num120, (int)num118);
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
