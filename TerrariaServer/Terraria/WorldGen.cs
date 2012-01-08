
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using TerrariaServer.Hooks;

namespace Terraria
{
	public class WorldGen
	{
		public static int c = 0;
		public static int m = 0;
		public static int a = 0;
		public static int co = 0;
		public static int ir = 0;
		public static int si = 0;
		public static int go = 0;
		public static int maxMech = 1000;
		public static int[] mechX = new int[WorldGen.maxMech];
		public static int[] mechY = new int[WorldGen.maxMech];
		public static int numMechs = 0;
		public static int[] mechTime = new int[WorldGen.maxMech];
		public static int maxWire = 1000;
		public static int[] wireX = new int[WorldGen.maxWire];
		public static int[] wireY = new int[WorldGen.maxWire];
		public static int numWire = 0;
		public static int[] noWireX = new int[WorldGen.maxWire];
		public static int[] noWireY = new int[WorldGen.maxWire];
		public static int numNoWire = 0;
		public static int maxPump = 20;
		public static int[] inPumpX = new int[WorldGen.maxPump];
		public static int[] inPumpY = new int[WorldGen.maxPump];
		public static int numInPump = 0;
		public static int[] outPumpX = new int[WorldGen.maxPump];
		public static int[] outPumpY = new int[WorldGen.maxPump];
		public static int numOutPump = 0;
		public static int totalEvil = 0;
		public static int totalGood = 0;
		public static int totalSolid = 0;
		public static int totalEvil2 = 0;
		public static int totalGood2 = 0;
		public static int totalSolid2 = 0;
		public static byte tEvil = 0;
		public static byte tGood = 0;
		public static int totalX = 0;
		public static int totalD = 0;
		public static bool hardLock = false;
		private static object padlock = new object();
		public static int lavaLine;
		public static int waterLine;
		public static bool noTileActions = false;
		public static bool spawnEye = false;
		public static bool gen = false;
		public static bool shadowOrbSmashed = false;
		public static int shadowOrbCount = 0;
		public static int altarCount = 0;
		public static bool spawnMeteor = false;
		public static bool loadFailed = false;
		public static bool loadSuccess = false;
		public static bool worldCleared = false;
		public static bool worldBackup = false;
		public static bool loadBackup = false;
		private static int lastMaxTilesX = 0;
		private static int lastMaxTilesY = 0;
		public static bool saveLock = false;
		private static bool mergeUp = false;
		private static bool mergeDown = false;
		private static bool mergeLeft = false;
		private static bool mergeRight = false;
		private static int tempMoonPhase = Main.moonPhase;
		private static bool tempDayTime = Main.dayTime;
		private static bool tempBloodMoon = Main.bloodMoon;
		private static double tempTime = Main.time;
		private static bool stopDrops = false;
		private static bool mudWall = false;
		private static int grassSpread = 0;
		public static bool noLiquidCheck = false;
		[ThreadStatic]
		public static Random genRand = new Random();
		public static string statusText = "";
		public static bool destroyObject = false;
		public static int spawnDelay = 0;
		public static int spawnNPC = 0;
		public static int maxRoomTiles = 1900;
		public static int numRoomTiles;
		public static int[] roomX = new int[WorldGen.maxRoomTiles];
		public static int[] roomY = new int[WorldGen.maxRoomTiles];
		public static int roomX1;
		public static int roomX2;
		public static int roomY1;
		public static int roomY2;
		public static bool canSpawn;
		public static bool[] houseTile = new bool[150];
		public static int bestX = 0;
		public static int bestY = 0;
		public static int hiScore = 0;
		public static int dungeonX;
		public static int dungeonY;
		public static Vector2 lastDungeonHall = default(Vector2);
		public static int maxDRooms = 100;
		public static int numDRooms = 0;
		public static int[] dRoomX = new int[WorldGen.maxDRooms];
		public static int[] dRoomY = new int[WorldGen.maxDRooms];
		public static int[] dRoomSize = new int[WorldGen.maxDRooms];
		private static bool[] dRoomTreasure = new bool[WorldGen.maxDRooms];
		private static int[] dRoomL = new int[WorldGen.maxDRooms];
		private static int[] dRoomR = new int[WorldGen.maxDRooms];
		private static int[] dRoomT = new int[WorldGen.maxDRooms];
		private static int[] dRoomB = new int[WorldGen.maxDRooms];
		private static int numDDoors;
		private static int[] DDoorX = new int[300];
		private static int[] DDoorY = new int[300];
		private static int[] DDoorPos = new int[300];
		private static int numDPlats;
		private static int[] DPlatX = new int[300];
		private static int[] DPlatY = new int[300];
		private static int[] JChestX = new int[100];
		private static int[] JChestY = new int[100];
		private static int numJChests = 0;
		public static int dEnteranceX = 0;
		public static bool dSurface = false;
		private static double dxStrength1;
		private static double dyStrength1;
		private static double dxStrength2;
		private static double dyStrength2;
		private static int dMinX;
		private static int dMaxX;
		private static int dMinY;
		private static int dMaxY;
		private static int numIslandHouses = 0;
		private static int houseCount = 0;
		private static int[] fihX = new int[300];
		private static int[] fihY = new int[300];
		private static int numMCaves = 0;
		private static int[] mCaveX = new int[300];
		private static int[] mCaveY = new int[300];
		private static int JungleX = 0;
		private static int hellChest = 0;
		private static bool roomTorch;
		private static bool roomDoor;
		private static bool roomChair;
		private static bool roomTable;
		private static bool roomOccupied;
		private static bool roomEvil;
		public static bool MoveNPC(int x, int y, int n)
		{
			if (!WorldGen.StartRoomCheck(x, y))
			{
				Main.NewText("This is not valid housing.", 255, 240, 20);
				return false;
			}
			if (!WorldGen.RoomNeeds(WorldGen.spawnNPC))
			{
				int num = 0;
				string[] array = new string[4];
				if (!WorldGen.roomTorch)
				{
					array[num] = "a light source";
					num++;
				}
				if (!WorldGen.roomDoor)
				{
					array[num] = "a door";
					num++;
				}
				if (!WorldGen.roomTable)
				{
					array[num] = "a table";
					num++;
				}
				if (!WorldGen.roomChair)
				{
					array[num] = "a chair";
					num++;
				}
				string text = "";
				for (int i = 0; i < num; i++)
				{
					if (num == 2 && i == 1)
					{
						text += " and ";
					}
					else
					{
						if (i > 0 && i != num - 1)
						{
							text += ", and ";
						}
						else
						{
							if (i > 0)
							{
								text += ", ";
							}
						}
					}
					text += array[i];
				}
				Main.NewText("This housing is missing " + text + ".", 255, 240, 20);
				return false;
			}
			WorldGen.ScoreRoom(-1);
			if (WorldGen.hiScore <= 0)
			{
				if (WorldGen.roomOccupied)
				{
					Main.NewText("This housing is already occupied.", 255, 240, 20);
				}
				else
				{
					if (WorldGen.roomEvil)
					{
						Main.NewText("This housing is corrupted.", 255, 240, 20);
					}
					else
					{
						Main.NewText("This housing is not suitable.", 255, 240, 20);
					}
				}
				return false;
			}
			return true;
		}
		public static void moveRoom(int x, int y, int n)
		{
			if (Main.netMode == 1)
			{
				NetMessage.SendData(60, -1, -1, "", n, (float)x, (float)y, 1f, 0);
				return;
			}
			WorldGen.spawnNPC = Main.npc[n].type;
			Main.npc[n].homeless = true;
			WorldGen.SpawnNPC(x, y);
		}
		public static void kickOut(int n)
		{
			if (Main.netMode == 1)
			{
				NetMessage.SendData(60, -1, -1, "", n, 0f, 0f, 0f, 0);
				return;
			}
			Main.npc[n].homeless = true;
		}
		public static void SpawnNPC(int x, int y)
		{
			if (Main.wallHouse[(int)Main.tile[x, y].wall])
			{
				WorldGen.canSpawn = true;
			}
			if (!WorldGen.canSpawn)
			{
				return;
			}
			if (!WorldGen.StartRoomCheck(x, y))
			{
				return;
			}
			if (!WorldGen.RoomNeeds(WorldGen.spawnNPC))
			{
				return;
			}
			WorldGen.ScoreRoom(-1);
			if (WorldGen.hiScore > 0)
			{
				int num = -1;
				for (int i = 0; i < 200; i++)
				{
					if (Main.npc[i].active && Main.npc[i].homeless && Main.npc[i].type == WorldGen.spawnNPC)
					{
						num = i;
						break;
					}
				}
				if (num == -1)
				{
					int num2 = WorldGen.bestX;
					int num3 = WorldGen.bestY;
					bool flag = false;
					if (!flag)
					{
						flag = true;
						Rectangle value = new Rectangle(num2 * 16 + 8 - NPC.sWidth / 2 - NPC.safeRangeX, num3 * 16 + 8 - NPC.sHeight / 2 - NPC.safeRangeY, NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
						for (int j = 0; j < 255; j++)
						{
							if (Main.player[j].active)
							{
								Rectangle rectangle = new Rectangle((int)Main.player[j].position.X, (int)Main.player[j].position.Y, Main.player[j].width, Main.player[j].height);
								if (rectangle.Intersects(value))
								{
									flag = false;
									break;
								}
							}
						}
					}
					if (!flag && (double)num3 <= Main.worldSurface)
					{
						for (int k = 1; k < 500; k++)
						{
							for (int l = 0; l < 2; l++)
							{
								if (l == 0)
								{
									num2 = WorldGen.bestX + k;
								}
								else
								{
									num2 = WorldGen.bestX - k;
								}
								if (num2 > 10 && num2 < Main.maxTilesX - 10)
								{
									int num4 = WorldGen.bestY - k;
									double num5 = (double)(WorldGen.bestY + k);
									if (num4 < 10)
									{
										num4 = 10;
									}
									if (num5 > Main.worldSurface)
									{
										num5 = Main.worldSurface;
									}
									int num6 = num4;
									while ((double)num6 < num5)
									{
										num3 = num6;
										if (Main.tile[num2, num3].active && Main.tileSolid[(int)Main.tile[num2, num3].type])
										{
											if (!Collision.SolidTiles(num2 - 1, num2 + 1, num3 - 3, num3 - 1))
											{
												flag = true;
												Rectangle value2 = new Rectangle(num2 * 16 + 8 - NPC.sWidth / 2 - NPC.safeRangeX, num3 * 16 + 8 - NPC.sHeight / 2 - NPC.safeRangeY, NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
												for (int m = 0; m < 255; m++)
												{
													if (Main.player[m].active)
													{
														Rectangle rectangle2 = new Rectangle((int)Main.player[m].position.X, (int)Main.player[m].position.Y, Main.player[m].width, Main.player[m].height);
														if (rectangle2.Intersects(value2))
														{
															flag = false;
															break;
														}
													}
												}
												break;
											}
											break;
										}
										else
										{
											num6++;
										}
									}
								}
								if (flag)
								{
									break;
								}
							}
							if (flag)
							{
								break;
							}
						}
					}
					int num7 = NPC.NewNPC(num2 * 16, num3 * 16, WorldGen.spawnNPC, 1);
					Main.npc[num7].homeTileX = WorldGen.bestX;
					Main.npc[num7].homeTileY = WorldGen.bestY;
					if (num2 < WorldGen.bestX)
					{
						Main.npc[num7].direction = 1;
					}
					else
					{
						if (num2 > WorldGen.bestX)
						{
							Main.npc[num7].direction = -1;
						}
					}
					Main.npc[num7].netUpdate = true;
					string str = Main.npc[num7].name;
					if (Main.chrName[Main.npc[num7].type] != "")
					{
						str = Main.chrName[Main.npc[num7].type] + " the " + Main.npc[num7].name;
					}
					if (Main.netMode == 0)
					{
						Main.NewText(str + " has arrived!", 50, 125, 255);
					}
					else
					{
						if (Main.netMode == 2)
						{
							NetMessage.SendData(25, -1, -1, str + " has arrived!", 255, 50f, 125f, 255f, 0);
						}
					}
				}
				else
				{
					WorldGen.spawnNPC = 0;
					Main.npc[num].homeTileX = WorldGen.bestX;
					Main.npc[num].homeTileY = WorldGen.bestY;
					Main.npc[num].homeless = false;
				}
				WorldGen.spawnNPC = 0;
			}
		}
		public static bool RoomNeeds(int npcType)
		{
			WorldGen.roomChair = false;
			WorldGen.roomDoor = false;
			WorldGen.roomTable = false;
			WorldGen.roomTorch = false;
			if (WorldGen.houseTile[15] || WorldGen.houseTile[79] || WorldGen.houseTile[89] || WorldGen.houseTile[102])
			{
				WorldGen.roomChair = true;
			}
			if (WorldGen.houseTile[14] || WorldGen.houseTile[18] || WorldGen.houseTile[87] || WorldGen.houseTile[88] || WorldGen.houseTile[90] || WorldGen.houseTile[101])
			{
				WorldGen.roomTable = true;
			}
			if (WorldGen.houseTile[4] || WorldGen.houseTile[33] || WorldGen.houseTile[34] || WorldGen.houseTile[35] || WorldGen.houseTile[36] || WorldGen.houseTile[42] || WorldGen.houseTile[49] || WorldGen.houseTile[93] || WorldGen.houseTile[95] || WorldGen.houseTile[98] || WorldGen.houseTile[100] || WorldGen.houseTile[149])
			{
				WorldGen.roomTorch = true;
			}
			if (WorldGen.houseTile[10] || WorldGen.houseTile[11] || WorldGen.houseTile[19])
			{
				WorldGen.roomDoor = true;
			}
			if (WorldGen.roomChair && WorldGen.roomTable && WorldGen.roomDoor && WorldGen.roomTorch)
			{
				WorldGen.canSpawn = true;
			}
			else
			{
				WorldGen.canSpawn = false;
			}
			return WorldGen.canSpawn;
		}
		public static void QuickFindHome(int npc)
		{
			if (Main.npc[npc].homeTileX > 10 && Main.npc[npc].homeTileY > 10 && Main.npc[npc].homeTileX < Main.maxTilesX - 10 && Main.npc[npc].homeTileY < Main.maxTilesY)
			{
				WorldGen.canSpawn = false;
				WorldGen.StartRoomCheck(Main.npc[npc].homeTileX, Main.npc[npc].homeTileY - 1);
				if (!WorldGen.canSpawn)
				{
					for (int i = Main.npc[npc].homeTileX - 1; i < Main.npc[npc].homeTileX + 2; i++)
					{
						int num = Main.npc[npc].homeTileY - 1;
						while (num < Main.npc[npc].homeTileY + 2 && !WorldGen.StartRoomCheck(i, num))
						{
							num++;
						}
					}
				}
				if (!WorldGen.canSpawn)
				{
					int num2 = 10;
					for (int j = Main.npc[npc].homeTileX - num2; j <= Main.npc[npc].homeTileX + num2; j += 2)
					{
						int num3 = Main.npc[npc].homeTileY - num2;
						while (num3 <= Main.npc[npc].homeTileY + num2 && !WorldGen.StartRoomCheck(j, num3))
						{
							num3 += 2;
						}
					}
				}
				if (WorldGen.canSpawn)
				{
					WorldGen.RoomNeeds(Main.npc[npc].type);
					if (WorldGen.canSpawn)
					{
						WorldGen.ScoreRoom(npc);
					}
					if (WorldGen.canSpawn && WorldGen.hiScore > 0)
					{
						Main.npc[npc].homeTileX = WorldGen.bestX;
						Main.npc[npc].homeTileY = WorldGen.bestY;
						Main.npc[npc].homeless = false;
						WorldGen.canSpawn = false;
						return;
					}
					Main.npc[npc].homeless = true;
					return;
				}
				else
				{
					Main.npc[npc].homeless = true;
				}
			}
		}
		public static void ScoreRoom(int ignoreNPC = -1)
		{
			WorldGen.roomOccupied = false;
			WorldGen.roomEvil = false;
			for (int i = 0; i < 200; i++)
			{
				if (Main.npc[i].active && Main.npc[i].townNPC && ignoreNPC != i && !Main.npc[i].homeless)
				{
					for (int j = 0; j < WorldGen.numRoomTiles; j++)
					{
						if (Main.npc[i].homeTileX == WorldGen.roomX[j] && Main.npc[i].homeTileY == WorldGen.roomY[j])
						{
							bool flag = false;
							for (int k = 0; k < WorldGen.numRoomTiles; k++)
							{
								if (Main.npc[i].homeTileX == WorldGen.roomX[k] && Main.npc[i].homeTileY - 1 == WorldGen.roomY[k])
								{
									flag = true;
									break;
								}
							}
							if (flag)
							{
								WorldGen.roomOccupied = true;
								WorldGen.hiScore = -1;
								return;
							}
						}
					}
				}
			}
			WorldGen.hiScore = 0;
			int num = 0;
			int num2 = 0;
			int num3 = WorldGen.roomX1 - Main.zoneX / 2 / 16 - 1 - Lighting.offScreenTiles;
			int num4 = WorldGen.roomX2 + Main.zoneX / 2 / 16 + 1 + Lighting.offScreenTiles;
			int num5 = WorldGen.roomY1 - Main.zoneY / 2 / 16 - 1 - Lighting.offScreenTiles;
			int num6 = WorldGen.roomY2 + Main.zoneY / 2 / 16 + 1 + Lighting.offScreenTiles;
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num4 >= Main.maxTilesX)
			{
				num4 = Main.maxTilesX - 1;
			}
			if (num5 < 0)
			{
				num5 = 0;
			}
			if (num6 > Main.maxTilesX)
			{
				num6 = Main.maxTilesX;
			}
			for (int l = num3 + 1; l < num4; l++)
			{
				for (int m = num5 + 2; m < num6 + 2; m++)
				{
					if (Main.tile[l, m].active)
					{
						if (Main.tile[l, m].type == 23 || Main.tile[l, m].type == 24 || Main.tile[l, m].type == 25 || Main.tile[l, m].type == 32 || Main.tile[l, m].type == 112)
						{
							num2++;
						}
						else
						{
							if (Main.tile[l, m].type == 27)
							{
								num2 -= 5;
							}
							else
							{
								if (Main.tile[l, m].type == 109 || Main.tile[l, m].type == 110 || Main.tile[l, m].type == 113 || Main.tile[l, m].type == 116)
								{
									num2--;
								}
							}
						}
					}
				}
			}
			if (num2 < 50)
			{
				num2 = 0;
			}
			int num7 = -num2;
			if (num7 <= -250)
			{
				WorldGen.hiScore = num7;
				WorldGen.roomEvil = true;
				return;
			}
			num3 = WorldGen.roomX1;
			num4 = WorldGen.roomX2;
			num5 = WorldGen.roomY1;
			num6 = WorldGen.roomY2;
			for (int n = num3 + 1; n < num4; n++)
			{
				for (int num8 = num5 + 2; num8 < num6 + 2; num8++)
				{
					if (Main.tile[n, num8].active)
					{
						num = num7;
						if (Main.tileSolid[(int)Main.tile[n, num8].type] && !Main.tileSolidTop[(int)Main.tile[n, num8].type] && !Collision.SolidTiles(n - 1, n + 1, num8 - 3, num8 - 1) && Main.tile[n - 1, num8].active && Main.tileSolid[(int)Main.tile[n - 1, num8].type] && Main.tile[n + 1, num8].active && Main.tileSolid[(int)Main.tile[n + 1, num8].type])
						{
							for (int num9 = n - 2; num9 < n + 3; num9++)
							{
								for (int num10 = num8 - 4; num10 < num8; num10++)
								{
									if (Main.tile[num9, num10].active)
									{
										if (num9 == n)
										{
											num -= 15;
										}
										else
										{
											if (Main.tile[num9, num10].type == 10 || Main.tile[num9, num10].type == 11)
											{
												num -= 20;
											}
											else
											{
												if (Main.tileSolid[(int)Main.tile[num9, num10].type])
												{
													num -= 5;
												}
												else
												{
													num += 5;
												}
											}
										}
									}
								}
							}
							if (num > WorldGen.hiScore)
							{
								bool flag2 = false;
								for (int num11 = 0; num11 < WorldGen.numRoomTiles; num11++)
								{
									if (WorldGen.roomX[num11] == n && WorldGen.roomY[num11] == num8)
									{
										flag2 = true;
										break;
									}
								}
								if (flag2)
								{
									WorldGen.hiScore = num;
									WorldGen.bestX = n;
									WorldGen.bestY = num8;
								}
							}
						}
					}
				}
			}
		}
		public static bool StartRoomCheck(int x, int y)
		{
			WorldGen.roomX1 = x;
			WorldGen.roomX2 = x;
			WorldGen.roomY1 = y;
			WorldGen.roomY2 = y;
			WorldGen.numRoomTiles = 0;
			for (int i = 0; i < 150; i++)
			{
				WorldGen.houseTile[i] = false;
			}
			WorldGen.canSpawn = true;
			if (Main.tile[x, y].active && Main.tileSolid[(int)Main.tile[x, y].type])
			{
				WorldGen.canSpawn = false;
			}
			WorldGen.CheckRoom(x, y);
			if (WorldGen.numRoomTiles < 60)
			{
				WorldGen.canSpawn = false;
			}
			return WorldGen.canSpawn;
		}
		public static void CheckRoom(int x, int y)
		{
			if (!WorldGen.canSpawn)
			{
				return;
			}
			if (x < 10 || y < 10 || x >= Main.maxTilesX - 10 || y >= WorldGen.lastMaxTilesY - 10)
			{
				WorldGen.canSpawn = false;
				return;
			}
			for (int i = 0; i < WorldGen.numRoomTiles; i++)
			{
				if (WorldGen.roomX[i] == x && WorldGen.roomY[i] == y)
				{
					return;
				}
			}
			WorldGen.roomX[WorldGen.numRoomTiles] = x;
			WorldGen.roomY[WorldGen.numRoomTiles] = y;
			WorldGen.numRoomTiles++;
			if (WorldGen.numRoomTiles >= WorldGen.maxRoomTiles)
			{
				WorldGen.canSpawn = false;
				return;
			}
			if (Main.tile[x, y].active)
			{
				WorldGen.houseTile[(int)Main.tile[x, y].type] = true;
				if (Main.tileSolid[(int)Main.tile[x, y].type] || Main.tile[x, y].type == 11)
				{
					return;
				}
			}
			if (x < WorldGen.roomX1)
			{
				WorldGen.roomX1 = x;
			}
			if (x > WorldGen.roomX2)
			{
				WorldGen.roomX2 = x;
			}
			if (y < WorldGen.roomY1)
			{
				WorldGen.roomY1 = y;
			}
			if (y > WorldGen.roomY2)
			{
				WorldGen.roomY2 = y;
			}
			bool flag = false;
			bool flag2 = false;
			for (int j = -2; j < 3; j++)
			{
				if (Main.wallHouse[(int)Main.tile[x + j, y].wall])
				{
					flag = true;
				}
				if (Main.tile[x + j, y].active && (Main.tileSolid[(int)Main.tile[x + j, y].type] || Main.tile[x + j, y].type == 11))
				{
					flag = true;
				}
				if (Main.wallHouse[(int)Main.tile[x, y + j].wall])
				{
					flag2 = true;
				}
				if (Main.tile[x, y + j].active && (Main.tileSolid[(int)Main.tile[x, y + j].type] || Main.tile[x, y + j].type == 11))
				{
					flag2 = true;
				}
			}
			if (!flag || !flag2)
			{
				WorldGen.canSpawn = false;
				return;
			}
			for (int k = x - 1; k < x + 2; k++)
			{
				for (int l = y - 1; l < y + 2; l++)
				{
					if ((k != x || l != y) && WorldGen.canSpawn)
					{
						WorldGen.CheckRoom(k, l);
					}
				}
			}
		}
		public static void dropMeteor()
		{
			bool flag = true;
			int num = 0;
			if (Main.netMode == 1)
			{
				return;
			}
			for (int i = 0; i < 255; i++)
			{
				if (Main.player[i].active)
				{
					flag = false;
					break;
				}
			}
			int num2 = 0;
			float num3 = (float)(Main.maxTilesX / 4200);
			int num4 = (int)(400f * num3);
			for (int j = 5; j < Main.maxTilesX - 5; j++)
			{
				int num5 = 5;
				while ((double)num5 < Main.worldSurface)
				{
					if (Main.tile[j, num5].active && Main.tile[j, num5].type == 37)
					{
						num2++;
						if (num2 > num4)
						{
							return;
						}
					}
					num5++;
				}
			}
			while (!flag)
			{
				float num6 = (float)Main.maxTilesX * 0.08f;
				int num7 = Main.rand.Next(50, Main.maxTilesX - 50);
				while ((float)num7 > (float)Main.spawnTileX - num6 && (float)num7 < (float)Main.spawnTileX + num6)
				{
					num7 = Main.rand.Next(50, Main.maxTilesX - 50);
				}
				for (int k = Main.rand.Next(100); k < Main.maxTilesY; k++)
				{
					if (Main.tile[num7, k].active && Main.tileSolid[(int)Main.tile[num7, k].type])
					{
						flag = WorldGen.meteor(num7, k);
						break;
					}
				}
				num++;
				if (num >= 100)
				{
					return;
				}
			}
		}
		public static bool meteor(int i, int j)
		{
			if (i < 50 || i > Main.maxTilesX - 50)
			{
				return false;
			}
			if (j < 50 || j > Main.maxTilesY - 50)
			{
				return false;
			}
			int num = 25;
			Rectangle rectangle = new Rectangle((i - num) * 16, (j - num) * 16, num * 2 * 16, num * 2 * 16);
			for (int k = 0; k < 255; k++)
			{
				if (Main.player[k].active)
				{
					Rectangle value = new Rectangle((int)(Main.player[k].position.X + (float)(Main.player[k].width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(Main.player[k].position.Y + (float)(Main.player[k].height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
					if (rectangle.Intersects(value))
					{
						return false;
					}
				}
			}
			for (int l = 0; l < 200; l++)
			{
				if (Main.npc[l].active)
				{
					Rectangle value2 = new Rectangle((int)Main.npc[l].position.X, (int)Main.npc[l].position.Y, Main.npc[l].width, Main.npc[l].height);
					if (rectangle.Intersects(value2))
					{
						return false;
					}
				}
			}
			for (int m = i - num; m < i + num; m++)
			{
				for (int n = j - num; n < j + num; n++)
				{
					if (Main.tile[m, n].active && Main.tile[m, n].type == 21)
					{
						return false;
					}
				}
			}
			WorldGen.stopDrops = true;
			num = 15;
			for (int num2 = i - num; num2 < i + num; num2++)
			{
				for (int num3 = j - num; num3 < j + num; num3++)
				{
					if (num3 > j + Main.rand.Next(-2, 3) - 5 && (double)(Math.Abs(i - num2) + Math.Abs(j - num3)) < (double)num * 1.5 + (double)Main.rand.Next(-5, 5))
					{
						if (!Main.tileSolid[(int)Main.tile[num2, num3].type])
						{
							Main.tile[num2, num3].active = false;
						}
						Main.tile[num2, num3].type = 37;
					}
				}
			}
			num = 10;
			for (int num4 = i - num; num4 < i + num; num4++)
			{
				for (int num5 = j - num; num5 < j + num; num5++)
				{
					if (num5 > j + Main.rand.Next(-2, 3) - 5 && Math.Abs(i - num4) + Math.Abs(j - num5) < num + Main.rand.Next(-3, 4))
					{
						Main.tile[num4, num5].active = false;
					}
				}
			}
			num = 16;
			for (int num6 = i - num; num6 < i + num; num6++)
			{
				for (int num7 = j - num; num7 < j + num; num7++)
				{
					if (Main.tile[num6, num7].type == 5 || Main.tile[num6, num7].type == 32)
					{
						WorldGen.KillTile(num6, num7, false, false, false);
					}
					WorldGen.SquareTileFrame(num6, num7, true);
					WorldGen.SquareWallFrame(num6, num7, true);
				}
			}
			num = 23;
			for (int num8 = i - num; num8 < i + num; num8++)
			{
				for (int num9 = j - num; num9 < j + num; num9++)
				{
					if (Main.tile[num8, num9].active && Main.rand.Next(10) == 0 && (double)(Math.Abs(i - num8) + Math.Abs(j - num9)) < (double)num * 1.3)
					{
						if (Main.tile[num8, num9].type == 5 || Main.tile[num8, num9].type == 32)
						{
							WorldGen.KillTile(num8, num9, false, false, false);
						}
						Main.tile[num8, num9].type = 37;
						WorldGen.SquareTileFrame(num8, num9, true);
					}
				}
			}
			WorldGen.stopDrops = false;
			if (Main.netMode == 0)
			{
				Main.NewText("A meteorite has landed!", 50, 255, 130);
			}
			else
			{
				if (Main.netMode == 2)
				{
					NetMessage.SendData(25, -1, -1, "A meteorite has landed!", 255, 50f, 255f, 130f, 0);
				}
			}
			if (Main.netMode != 1)
			{
				NetMessage.SendTileSquare(-1, i, j, 30);
			}
			return true;
		}
		public static void setWorldSize()
		{
			Main.bottomWorld = (float)(Main.maxTilesY * 16);
			Main.rightWorld = (float)(Main.maxTilesX * 16);
			Main.maxSectionsX = Main.maxTilesX / 200;
			Main.maxSectionsY = Main.maxTilesY / 150;
		}
		public static void worldGenCallBack(object threadContext)
		{
			Main.PlaySound(10, -1, -1, 1);
			WorldGen.clearWorld();
			WorldGen.generateWorld(-1);
			WorldGen.saveWorld(true);
			Main.LoadWorlds();
			if (Main.menuMode == 10)
			{
				Main.menuMode = 6;
			}
			Main.PlaySound(10, -1, -1, 1);
		}
		public static void CreateNewWorld()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.worldGenCallBack), 1);
		}
		public static void SaveAndQuitCallBack(object threadContext)
		{
			Main.menuMode = 10;
			Main.gameMenu = true;
			Player.SavePlayer(Main.player[Main.myPlayer], Main.playerPathName);
			if (Main.netMode == 0)
			{
				WorldGen.saveWorld(false);
				Main.PlaySound(10, -1, -1, 1);
			}
			else
			{
				Netplay.disconnect = true;
				Main.netMode = 0;
			}
			Main.menuMode = 0;
		}
		public static void SaveAndQuit()
		{
			Main.PlaySound(11, -1, -1, 1);
			ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.SaveAndQuitCallBack), 1);
		}
		public static void playWorldCallBack(object threadContext)
		{
			if (Main.rand == null)
			{
				Main.rand = new Random((int)DateTime.Now.Ticks);
			}
			for (int i = 0; i < 255; i++)
			{
				if (i != Main.myPlayer)
				{
					Main.player[i].active = false;
				}
			}
			WorldGen.loadWorld();
			if (WorldGen.loadFailed || !WorldGen.loadSuccess)
			{
				WorldGen.loadWorld();
				if (WorldGen.loadFailed || !WorldGen.loadSuccess)
				{
					if (File.Exists(Main.worldPathName + ".bak"))
					{
						WorldGen.worldBackup = true;
					}
					else
					{
						WorldGen.worldBackup = false;
					}
					if (!Main.dedServ)
					{
						if (WorldGen.worldBackup)
						{
							Main.menuMode = 200;
							return;
						}
						Main.menuMode = 201;
						return;
					}
					else
					{
						if (!WorldGen.worldBackup)
						{
							Console.WriteLine("Load failed!  No backup found.");
							return;
						}
						File.Copy(Main.worldPathName + ".bak", Main.worldPathName, true);
						File.Delete(Main.worldPathName + ".bak");
						WorldGen.loadWorld();
						if (WorldGen.loadFailed || !WorldGen.loadSuccess)
						{
							WorldGen.loadWorld();
							if (WorldGen.loadFailed || !WorldGen.loadSuccess)
							{
								Console.WriteLine("Load failed!");
								return;
							}
						}
					}
				}
			}
			WorldGen.EveryTileFrame();
			if (Main.gameMenu)
			{
				Main.gameMenu = false;
			}
			Main.player[Main.myPlayer].Spawn();
			Main.player[Main.myPlayer].UpdatePlayer(Main.myPlayer);
			Main.dayTime = WorldGen.tempDayTime;
			Main.time = WorldGen.tempTime;
			Main.moonPhase = WorldGen.tempMoonPhase;
			Main.bloodMoon = WorldGen.tempBloodMoon;
			Main.PlaySound(11, -1, -1, 1);
			Main.resetClouds = true;
		}
		public static void playWorld()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.playWorldCallBack), 1);
		}
		public static void saveAndPlayCallBack(object threadContext)
		{
			WorldGen.saveWorld(false);
		}
		public static void saveAndPlay()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.saveAndPlayCallBack), 1);
		}
		public static void saveToonWhilePlayingCallBack(object threadContext)
		{
			Player.SavePlayer(Main.player[Main.myPlayer], Main.playerPathName);
		}
		public static void saveToonWhilePlaying()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.saveToonWhilePlayingCallBack), 1);
		}
		public static void serverLoadWorldCallBack(object threadContext)
		{
			WorldGen.loadWorld();
			if (WorldGen.loadFailed || !WorldGen.loadSuccess)
			{
				WorldGen.loadWorld();
				if (WorldGen.loadFailed || !WorldGen.loadSuccess)
				{
					if (File.Exists(Main.worldPathName + ".bak"))
					{
						WorldGen.worldBackup = true;
					}
					else
					{
						WorldGen.worldBackup = false;
					}
					if (!Main.dedServ)
					{
						if (WorldGen.worldBackup)
						{
							Main.menuMode = 200;
							return;
						}
						Main.menuMode = 201;
						return;
					}
					else
					{
						if (!WorldGen.worldBackup)
						{
							Console.WriteLine("Load failed!  No backup found.");
							return;
						}
						File.Copy(Main.worldPathName + ".bak", Main.worldPathName, true);
						File.Delete(Main.worldPathName + ".bak");
						WorldGen.loadWorld();
						if (WorldGen.loadFailed || !WorldGen.loadSuccess)
						{
							WorldGen.loadWorld();
							if (WorldGen.loadFailed || !WorldGen.loadSuccess)
							{
								Console.WriteLine("Load failed!");
								return;
							}
						}
					}
				}
			}
			Main.PlaySound(10, -1, -1, 1);
			Netplay.StartServer();
			Main.dayTime = WorldGen.tempDayTime;
			Main.time = WorldGen.tempTime;
			Main.moonPhase = WorldGen.tempMoonPhase;
			Main.bloodMoon = WorldGen.tempBloodMoon;
		}
		public static void serverLoadWorld()
		{
			ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.serverLoadWorldCallBack), 1);
		}
		public static void clearWorld()
		{
			WorldGen.totalSolid2 = 0;
			WorldGen.totalGood2 = 0;
			WorldGen.totalEvil2 = 0;
			WorldGen.totalSolid = 0;
			WorldGen.totalGood = 0;
			WorldGen.totalEvil = 0;
			WorldGen.totalX = 0;
			WorldGen.totalD = 0;
			WorldGen.tEvil = 0;
			WorldGen.tGood = 0;
			NPC.clrNames();
			Main.trashItem = new Item();
			WorldGen.spawnEye = false;
			WorldGen.spawnNPC = 0;
			WorldGen.shadowOrbCount = 0;
			WorldGen.altarCount = 0;
			Main.hardMode = false;
			Main.helpText = 0;
			Main.dungeonX = 0;
			Main.dungeonY = 0;
			NPC.downedBoss1 = false;
			NPC.downedBoss2 = false;
			NPC.downedBoss3 = false;
			NPC.savedGoblin = false;
			NPC.savedWizard = false;
			NPC.savedMech = false;
			NPC.downedGoblins = false;
			NPC.downedClown = false;
			NPC.downedFrost = false;
			WorldGen.shadowOrbSmashed = false;
			WorldGen.spawnMeteor = false;
			WorldGen.stopDrops = false;
			Main.invasionDelay = 0;
			Main.invasionType = 0;
			Main.invasionSize = 0;
			Main.invasionWarn = 0;
			Main.invasionX = 0.0;
			WorldGen.noLiquidCheck = false;
			Liquid.numLiquid = 0;
			LiquidBuffer.numLiquidBuffer = 0;
            Main.tile.SetSize(Main.maxTilesX + 10, Main.maxTilesY + 10);
			if (Main.netMode == 1 || WorldGen.lastMaxTilesX > Main.maxTilesX || WorldGen.lastMaxTilesY > Main.maxTilesY)
			{
				for (int i = 0; i < WorldGen.lastMaxTilesX; i++)
				{
					float num = (float)i / (float)WorldGen.lastMaxTilesX;
					Main.statusText = "Freeing unused resources: " + (int)(num * 100f + 1f) + "%";
					for (int j = 0; j < WorldGen.lastMaxTilesY; j++)
					{
						Main.tile[i, j].Data = new TileData();
					}
				}
			}
			WorldGen.lastMaxTilesX = Main.maxTilesX;
			WorldGen.lastMaxTilesY = Main.maxTilesY;
			if (Main.netMode != 1)
			{
				for (int k = 0; k < Main.maxTilesX; k++)
				{
					float num2 = (float)k / (float)Main.maxTilesX;
					Main.statusText = "Resetting game objects: " + (int)(num2 * 100f + 1f) + "%";
					for (int l = 0; l < Main.maxTilesY; l++)
					{

					}
				}
			}
			for (int m = 0; m < 2000; m++)
			{
				Main.dust[m] = new Dust();
			}
			for (int n = 0; n < 200; n++)
			{
				Main.gore[n] = new Gore();
			}
			for (int num3 = 0; num3 < 200; num3++)
			{
				Main.item[num3] = new Item();
			}
			for (int num4 = 0; num4 < 200; num4++)
			{
				Main.npc[num4] = new NPC();
			}
			for (int num5 = 0; num5 < 1000; num5++)
			{
				Main.projectile[num5] = new Projectile();
			}
			for (int num6 = 0; num6 < 1000; num6++)
			{
				Main.chest[num6] = null;
			}
			for (int num7 = 0; num7 < 1000; num7++)
			{
				Main.sign[num7] = null;
			}
			for (int num8 = 0; num8 < Liquid.resLiquid; num8++)
			{
				Main.liquid[num8] = new Liquid();
			}
			for (int num9 = 0; num9 < 10000; num9++)
			{
				Main.liquidBuffer[num9] = new LiquidBuffer();
			}
			WorldGen.setWorldSize();
			WorldGen.worldCleared = true;
		}
		public static void saveWorld(bool resetTime = false)
		{
            if (WorldHooks.OnSaveWorld(resetTime))
            {
                return;
            }
			if (Main.worldName == "")
			{
				Main.worldName = "World";
			}
			if (WorldGen.saveLock)
			{
				return;
			}
			WorldGen.saveLock = true;
			while (WorldGen.hardLock)
			{
				Main.statusText = "Setting hard mode...";
			}
			lock (WorldGen.padlock)
			{
				try
				{
					Directory.CreateDirectory(Main.WorldPath);
				}
				catch
				{
				}
				if (!Main.skipMenu)
				{
					bool value = Main.dayTime;
					WorldGen.tempTime = Main.time;
					WorldGen.tempMoonPhase = Main.moonPhase;
					WorldGen.tempBloodMoon = Main.bloodMoon;
					if (resetTime)
					{
						value = true;
						WorldGen.tempTime = 13500.0;
						WorldGen.tempMoonPhase = 0;
						WorldGen.tempBloodMoon = false;
					}
					if (Main.worldPathName != null)
					{
						Stopwatch stopwatch = new Stopwatch();
						stopwatch.Start();
						string text = Main.worldPathName + ".sav";
						using (FileStream fileStream = new FileStream(text, FileMode.Create))
						{
							using (BinaryWriter binaryWriter = new BinaryWriter(fileStream))
							{
								binaryWriter.Write(Main.curRelease);
								binaryWriter.Write(Main.worldName);
								binaryWriter.Write(Main.worldID);
								binaryWriter.Write((int)Main.leftWorld);
								binaryWriter.Write((int)Main.rightWorld);
								binaryWriter.Write((int)Main.topWorld);
								binaryWriter.Write((int)Main.bottomWorld);
								binaryWriter.Write(Main.maxTilesY);
								binaryWriter.Write(Main.maxTilesX);
								binaryWriter.Write(Main.spawnTileX);
								binaryWriter.Write(Main.spawnTileY);
								binaryWriter.Write(Main.worldSurface);
								binaryWriter.Write(Main.rockLayer);
								binaryWriter.Write(WorldGen.tempTime);
								binaryWriter.Write(value);
								binaryWriter.Write(WorldGen.tempMoonPhase);
								binaryWriter.Write(WorldGen.tempBloodMoon);
								binaryWriter.Write(Main.dungeonX);
								binaryWriter.Write(Main.dungeonY);
								binaryWriter.Write(NPC.downedBoss1);
								binaryWriter.Write(NPC.downedBoss2);
								binaryWriter.Write(NPC.downedBoss3);
								binaryWriter.Write(NPC.savedGoblin);
								binaryWriter.Write(NPC.savedWizard);
								binaryWriter.Write(NPC.savedMech);
								binaryWriter.Write(NPC.downedGoblins);
								binaryWriter.Write(NPC.downedClown);
								binaryWriter.Write(NPC.downedFrost);
								binaryWriter.Write(WorldGen.shadowOrbSmashed);
								binaryWriter.Write(WorldGen.spawnMeteor);
								binaryWriter.Write((byte)WorldGen.shadowOrbCount);
								binaryWriter.Write(WorldGen.altarCount);
								binaryWriter.Write(Main.hardMode);
								binaryWriter.Write(Main.invasionDelay);
								binaryWriter.Write(Main.invasionSize);
								binaryWriter.Write(Main.invasionType);
								binaryWriter.Write(Main.invasionX);
								for (int i = 0; i < Main.maxTilesX; i++)
								{
									float num = (float)i / (float)Main.maxTilesX;
									Main.statusText = "Saving world data: " + (int)(num * 100f + 1f) + "%";
									for (int j = 0; j < Main.maxTilesY; j++)
									{
										if (Main.tile[i, j].type == 127 && Main.tile[i, j].active)
										{
											WorldGen.KillTile(i, j, false, false, false);
											WorldGen.KillTile(i, j, false, false, false);
											if (!Main.tile[i, j].active && Main.netMode != 0)
											{
												NetMessage.SendData(17, -1, -1, "", 0, (float)i, (float)j, 0f, 0);
											}
										}
										TileData tiledata = Main.tile[i, j].Data;
										binaryWriter.Write(tiledata.active);
										if (tiledata.active)
										{
											binaryWriter.Write(tiledata.type);
											if (Main.tileFrameImportant[(int)tiledata.type])
											{
												binaryWriter.Write(tiledata.frameX);
												binaryWriter.Write(tiledata.frameY);
											}
										}
										if (Main.tile[i, j].wall > 0)
										{
											binaryWriter.Write(true);
											binaryWriter.Write(tiledata.wall);
										}
										else
										{
											binaryWriter.Write(false);
										}
										if (tiledata.liquid > 0)
										{
											binaryWriter.Write(true);
											binaryWriter.Write(tiledata.liquid);
											binaryWriter.Write(tiledata.lava);
										}
										else
										{
											binaryWriter.Write(false);
										}
										binaryWriter.Write(tiledata.wire);
										int num2 = 1;
                                        while (j + num2 < Main.maxTilesY && Main.tile[i, j].isTheSameAs(Main.tile[i, j + num2]))
										{
											num2++;
										}
										num2--;
										binaryWriter.Write((short)num2);
										j += num2;
									}
								}
								for (int k = 0; k < 1000; k++)
								{
									if (Main.chest[k] == null)
									{
										binaryWriter.Write(false);
									}
									else
									{
										Chest chest = (Chest)Main.chest[k].Clone();
										binaryWriter.Write(true);
										binaryWriter.Write(chest.x);
										binaryWriter.Write(chest.y);
										for (int l = 0; l < Chest.maxItems; l++)
										{
											if (chest.item[l].type == 0)
											{
												chest.item[l].stack = 0;
											}
											binaryWriter.Write((byte)chest.item[l].stack);
											if (chest.item[l].stack > 0)
											{
												binaryWriter.Write(chest.item[l].name);
												binaryWriter.Write(chest.item[l].prefix);
											}
										}
									}
								}
								for (int m = 0; m < 1000; m++)
								{
									if (Main.sign[m] == null || Main.sign[m].text == null)
									{
										binaryWriter.Write(false);
									}
									else
									{
										Sign sign = (Sign)Main.sign[m].Clone();
										binaryWriter.Write(true);
										binaryWriter.Write(sign.text);
										binaryWriter.Write(sign.x);
										binaryWriter.Write(sign.y);
									}
								}
								for (int n = 0; n < 200; n++)
								{
									NPC nPC = (NPC)Main.npc[n].Clone();
									if (nPC.active && nPC.townNPC)
									{
										binaryWriter.Write(true);
										binaryWriter.Write(nPC.name);
										binaryWriter.Write(nPC.position.X);
										binaryWriter.Write(nPC.position.Y);
										binaryWriter.Write(nPC.homeless);
										binaryWriter.Write(nPC.homeTileX);
										binaryWriter.Write(nPC.homeTileY);
									}
								}
								binaryWriter.Write(false);
								binaryWriter.Write(Main.chrName[17]);
								binaryWriter.Write(Main.chrName[18]);
								binaryWriter.Write(Main.chrName[19]);
								binaryWriter.Write(Main.chrName[20]);
								binaryWriter.Write(Main.chrName[22]);
								binaryWriter.Write(Main.chrName[54]);
								binaryWriter.Write(Main.chrName[38]);
								binaryWriter.Write(Main.chrName[107]);
								binaryWriter.Write(Main.chrName[108]);
								binaryWriter.Write(Main.chrName[124]);
								binaryWriter.Write(true);
								binaryWriter.Write(Main.worldName);
								binaryWriter.Write(Main.worldID);
								binaryWriter.Close();
								fileStream.Close();
								if (File.Exists(Main.worldPathName))
								{
									Main.statusText = "Backing up world file...";
									string destFileName = Main.worldPathName + ".bak";
									File.Copy(Main.worldPathName, destFileName, true);
								}
								File.Copy(text, Main.worldPathName, true);
								File.Delete(text);
							}
						}
						WorldGen.saveLock = false;
					}
				}
			}
		}
		public static void loadWorld()
		{
			Main.checkXMas();
			if (!File.Exists(Main.worldPathName) && Main.autoGen)
			{
				for (int i = Main.worldPathName.Length - 1; i >= 0; i--)
				{
                    if (Main.worldPathName.Substring(i, 1) == "\\" || Main.worldPathName.Substring(i, 1) == "/")
					{
						string path = Main.worldPathName.Substring(0, i);
						Directory.CreateDirectory(path);
						break;
					}
				}
				WorldGen.clearWorld();
				WorldGen.generateWorld(-1);
				WorldGen.saveWorld(false);
			}
			if (WorldGen.genRand == null)
			{
				WorldGen.genRand = new Random((int)DateTime.Now.Ticks);
			}
			using (FileStream fileStream = new FileStream(Main.worldPathName, FileMode.Open))
			{
				using (BinaryReader binaryReader = new BinaryReader(fileStream))
				{
					try
					{
						WorldGen.loadFailed = false;
						WorldGen.loadSuccess = false;
						int num = binaryReader.ReadInt32();
						if (num > Main.curRelease)
						{
							WorldGen.loadFailed = true;
							WorldGen.loadSuccess = false;
							try
							{
								binaryReader.Close();
								fileStream.Close();
							}
							catch
							{
							}
						}
						else
						{
							Main.worldName = binaryReader.ReadString();
							Main.worldID = binaryReader.ReadInt32();
							Main.leftWorld = (float)binaryReader.ReadInt32();
							Main.rightWorld = (float)binaryReader.ReadInt32();
							Main.topWorld = (float)binaryReader.ReadInt32();
							Main.bottomWorld = (float)binaryReader.ReadInt32();
							Main.maxTilesY = binaryReader.ReadInt32();
							Main.maxTilesX = binaryReader.ReadInt32();
							WorldGen.clearWorld();
							Main.spawnTileX = binaryReader.ReadInt32();
							Main.spawnTileY = binaryReader.ReadInt32();
							Main.worldSurface = binaryReader.ReadDouble();
							Main.rockLayer = binaryReader.ReadDouble();
							WorldGen.tempTime = binaryReader.ReadDouble();
							WorldGen.tempDayTime = binaryReader.ReadBoolean();
							WorldGen.tempMoonPhase = binaryReader.ReadInt32();
							WorldGen.tempBloodMoon = binaryReader.ReadBoolean();
							Main.dungeonX = binaryReader.ReadInt32();
							Main.dungeonY = binaryReader.ReadInt32();
							NPC.downedBoss1 = binaryReader.ReadBoolean();
							NPC.downedBoss2 = binaryReader.ReadBoolean();
							NPC.downedBoss3 = binaryReader.ReadBoolean();
							if (num >= 29)
							{
								NPC.savedGoblin = binaryReader.ReadBoolean();
								NPC.savedWizard = binaryReader.ReadBoolean();
								if (num >= 34)
								{
									NPC.savedMech = binaryReader.ReadBoolean();
								}
								NPC.downedGoblins = binaryReader.ReadBoolean();
							}
							if (num >= 32)
							{
								NPC.downedClown = binaryReader.ReadBoolean();
							}
							if (num >= 37)
							{
								NPC.downedFrost = binaryReader.ReadBoolean();
							}
							WorldGen.shadowOrbSmashed = binaryReader.ReadBoolean();
							WorldGen.spawnMeteor = binaryReader.ReadBoolean();
							WorldGen.shadowOrbCount = (int)binaryReader.ReadByte();
							if (num >= 23)
							{
								WorldGen.altarCount = binaryReader.ReadInt32();
								Main.hardMode = binaryReader.ReadBoolean();
							}
							Main.invasionDelay = binaryReader.ReadInt32();
							Main.invasionSize = binaryReader.ReadInt32();
							Main.invasionType = binaryReader.ReadInt32();
							Main.invasionX = binaryReader.ReadDouble();
							for (int j = 0; j < Main.maxTilesX; j++)
							{
								float num2 = (float)j / (float)Main.maxTilesX;
								Main.statusText = "Loading world data: " + (int)(num2 * 100f + 1f) + "%";
								for (int k = 0; k < Main.maxTilesY; k++)
								{
									Main.tile[j, k].active = binaryReader.ReadBoolean();
									if (Main.tile[j, k].active)
									{
										Main.tile[j, k].type = binaryReader.ReadByte();
										if (Main.tile[j, k].type == 127)
										{
											Main.tile[j, k].active = false;
										}
										if (Main.tileFrameImportant[(int)Main.tile[j, k].type])
										{
											if (num < 28 && Main.tile[j, k].type == 4)
											{
												Main.tile[j, k].frameX = 0;
												Main.tile[j, k].frameY = 0;
											}
											else
											{
												Main.tile[j, k].frameX = binaryReader.ReadInt16();
												Main.tile[j, k].frameY = binaryReader.ReadInt16();
												if (Main.tile[j, k].type == 144)
												{
													Main.tile[j, k].frameY = 0;
												}
											}
										}
										else
										{
											Main.tile[j, k].frameX = -1;
											Main.tile[j, k].frameY = -1;
										}
									}
									if (num <= 25)
									{
										binaryReader.ReadBoolean();
									}
									if (binaryReader.ReadBoolean())
									{
										Main.tile[j, k].wall = binaryReader.ReadByte();
									}
									if (binaryReader.ReadBoolean())
									{
										Main.tile[j, k].liquid = binaryReader.ReadByte();
										Main.tile[j, k].lava = binaryReader.ReadBoolean();
									}
									if (num >= 33)
									{
										Main.tile[j, k].wire = binaryReader.ReadBoolean();
									}
									if (num >= 25)
									{
										int num3 = (int)binaryReader.ReadInt16();
										if (num3 > 0)
										{
											for (int l = k + 1; l < k + num3 + 1; l++)
											{
												Main.tile[j, l].active = Main.tile[j, k].active;
												Main.tile[j, l].type = Main.tile[j, k].type;
												Main.tile[j, l].wall = Main.tile[j, k].wall;
												Main.tile[j, l].frameX = Main.tile[j, k].frameX;
												Main.tile[j, l].frameY = Main.tile[j, k].frameY;
												Main.tile[j, l].liquid = Main.tile[j, k].liquid;
												Main.tile[j, l].lava = Main.tile[j, k].lava;
												Main.tile[j, l].wire = Main.tile[j, k].wire;
											}
											k += num3;
										}
									}
								}
							}
							for (int m = 0; m < 1000; m++)
							{
								if (binaryReader.ReadBoolean())
								{
									Main.chest[m] = new Chest();
									Main.chest[m].x = binaryReader.ReadInt32();
									Main.chest[m].y = binaryReader.ReadInt32();
									for (int n = 0; n < Chest.maxItems; n++)
									{
										Main.chest[m].item[n] = new Item();
										byte b = binaryReader.ReadByte();
										if (b > 0)
										{
											string defaults = Item.VersionName(binaryReader.ReadString(), num);
											Main.chest[m].item[n].SetDefaults(defaults);
											Main.chest[m].item[n].stack = (int)b;
											if (num >= 36)
											{
												Main.chest[m].item[n].Prefix((int)binaryReader.ReadByte());
											}
										}
									}
								}
							}
							for (int num4 = 0; num4 < 1000; num4++)
							{
								if (binaryReader.ReadBoolean())
								{
									string text = binaryReader.ReadString();
									int num5 = binaryReader.ReadInt32();
									int num6 = binaryReader.ReadInt32();
									if (Main.tile[num5, num6].active && (Main.tile[num5, num6].type == 55 || Main.tile[num5, num6].type == 85))
									{
										Main.sign[num4] = new Sign();
										Main.sign[num4].x = num5;
										Main.sign[num4].y = num6;
										Main.sign[num4].text = text;
									}
								}
							}
							bool flag = binaryReader.ReadBoolean();
							int num7 = 0;
							while (flag)
							{
								Main.npc[num7].SetDefaults(binaryReader.ReadString());
								Main.npc[num7].position.X = binaryReader.ReadSingle();
								Main.npc[num7].position.Y = binaryReader.ReadSingle();
								Main.npc[num7].homeless = binaryReader.ReadBoolean();
								Main.npc[num7].homeTileX = binaryReader.ReadInt32();
								Main.npc[num7].homeTileY = binaryReader.ReadInt32();
								flag = binaryReader.ReadBoolean();
								num7++;
							}
							if (num >= 31)
							{
								Main.chrName[17] = binaryReader.ReadString();
								Main.chrName[18] = binaryReader.ReadString();
								Main.chrName[19] = binaryReader.ReadString();
								Main.chrName[20] = binaryReader.ReadString();
								Main.chrName[22] = binaryReader.ReadString();
								Main.chrName[54] = binaryReader.ReadString();
								Main.chrName[38] = binaryReader.ReadString();
								Main.chrName[107] = binaryReader.ReadString();
								Main.chrName[108] = binaryReader.ReadString();
								if (num >= 35)
								{
									Main.chrName[124] = binaryReader.ReadString();
								}
							}
							if (num >= 7)
							{
								bool flag2 = binaryReader.ReadBoolean();
								string text2 = binaryReader.ReadString();
								int num8 = binaryReader.ReadInt32();
								if (!flag2 || !(text2 == Main.worldName) || num8 != Main.worldID)
								{
									WorldGen.loadSuccess = false;
									WorldGen.loadFailed = true;
									binaryReader.Close();
									fileStream.Close();
									return;
								}
								WorldGen.loadSuccess = true;
							}
							else
							{
								WorldGen.loadSuccess = true;
							}
							binaryReader.Close();
							fileStream.Close();
							if (!WorldGen.loadFailed && WorldGen.loadSuccess)
							{
								WorldGen.gen = true;
								for (int num9 = 0; num9 < Main.maxTilesX; num9++)
								{
									float num10 = (float)num9 / (float)Main.maxTilesX;
									Main.statusText = "Checking tile alignment: " + (int)(num10 * 100f + 1f) + "%";
									WorldGen.CountTiles(num9);
								}
								WorldGen.waterLine = Main.maxTilesY;
								NPC.setNames();
								Liquid.QuickWater(2, -1, -1);
								WorldGen.WaterCheck();
								int num11 = 0;
								Liquid.quickSettle = true;
								int num12 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
								float num13 = 0f;
								while (Liquid.numLiquid > 0 && num11 < 100000)
								{
									num11++;
									float num14 = (float)(num12 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer)) / (float)num12;
									if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > num12)
									{
										num12 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
									}
									if (num14 > num13)
									{
										num13 = num14;
									}
									else
									{
										num14 = num13;
									}
									Main.statusText = "Settling liquids: " + (int)(num14 * 100f / 2f + 50f) + "%";
									Liquid.UpdateLiquid();
								}
								Liquid.quickSettle = false;
								WorldGen.WaterCheck();
								WorldGen.gen = false;
							}
						}
					}
					catch
					{
						WorldGen.loadFailed = true;
						WorldGen.loadSuccess = false;
						try
						{
							binaryReader.Close();
							fileStream.Close();
						}
						catch
						{
						}
					}
				}
			}
		}
		private static void resetGen()
		{
			WorldGen.mudWall = false;
			WorldGen.hellChest = 0;
			WorldGen.JungleX = 0;
			WorldGen.numMCaves = 0;
			WorldGen.numIslandHouses = 0;
			WorldGen.houseCount = 0;
			WorldGen.dEnteranceX = 0;
			WorldGen.numDRooms = 0;
			WorldGen.numDDoors = 0;
			WorldGen.numDPlats = 0;
			WorldGen.numJChests = 0;
		}
		public static bool placeTrap(int x2, int y2, int type = -1)
		{
			int num = y2;
			while (!WorldGen.SolidTile(x2, num))
			{
				num++;
				if (num >= Main.maxTilesY - 300)
				{
					return false;
				}
			}
			num--;
			if (Main.tile[x2, num].liquid > 0 && Main.tile[x2, num].lava)
			{
				return false;
			}
			if (type == -1 && Main.rand.Next(20) == 0)
			{
				type = 2;
			}
			else
			{
				if (type == -1)
				{
					type = Main.rand.Next(2);
				}
			}
			if (Main.tile[x2, num].active || Main.tile[x2 - 1, num].active || Main.tile[x2 + 1, num].active || Main.tile[x2, num - 1].active || Main.tile[x2 - 1, num - 1].active || Main.tile[x2 + 1, num - 1].active || Main.tile[x2, num - 2].active || Main.tile[x2 - 1, num - 2].active || Main.tile[x2 + 1, num - 2].active)
			{
				return false;
			}
			if (Main.tile[x2, num + 1].type == 48)
			{
				return false;
			}
			if (type == 0)
			{
				int num2 = x2;
				int num3 = num;
				num3 -= WorldGen.genRand.Next(3);
				while (!WorldGen.SolidTile(num2, num3))
				{
					num2--;
				}
				int num4 = num2;
				num2 = x2;
				while (!WorldGen.SolidTile(num2, num3))
				{
					num2++;
				}
				int num5 = num2;
				int num6 = x2 - num4;
				int num7 = num5 - x2;
				bool flag = false;
				bool flag2 = false;
				if (num6 > 5 && num6 < 50)
				{
					flag = true;
				}
				if (num7 > 5 && num7 < 50)
				{
					flag2 = true;
				}
				if (flag && !WorldGen.SolidTile(num4, num3 + 1))
				{
					flag = false;
				}
				if (flag2 && !WorldGen.SolidTile(num5, num3 + 1))
				{
					flag2 = false;
				}
				if (flag && (Main.tile[num4, num3].type == 10 || Main.tile[num4, num3].type == 48 || Main.tile[num4, num3 + 1].type == 10 || Main.tile[num4, num3 + 1].type == 48))
				{
					flag = false;
				}
				if (flag2 && (Main.tile[num5, num3].type == 10 || Main.tile[num5, num3].type == 48 || Main.tile[num5, num3 + 1].type == 10 || Main.tile[num5, num3 + 1].type == 48))
				{
					flag2 = false;
				}
				int style = 0;
				if (flag && flag2)
				{
					style = 1;
					num2 = num4;
					if (WorldGen.genRand.Next(2) == 0)
					{
						num2 = num5;
						style = -1;
					}
				}
				else
				{
					if (flag2)
					{
						num2 = num5;
						style = -1;
					}
					else
					{
						if (!flag)
						{
							return false;
						}
						num2 = num4;
						style = 1;
					}
				}
				if (Main.tile[x2, num].wall > 0)
				{
					WorldGen.PlaceTile(x2, num, 135, true, true, -1, 2);
				}
				else
				{
					WorldGen.PlaceTile(x2, num, 135, true, true, -1, WorldGen.genRand.Next(2, 4));
				}
				WorldGen.KillTile(num2, num3, false, false, false);
				WorldGen.PlaceTile(num2, num3, 137, true, true, -1, style);
				int num8 = x2;
				int num9 = num;
				while (num8 != num2 || num9 != num3)
				{
					Main.tile[num8, num9].wire = true;
					if (num8 > num2)
					{
						num8--;
					}
					if (num8 < num2)
					{
						num8++;
					}
					Main.tile[num8, num9].wire = true;
					if (num9 > num3)
					{
						num9--;
					}
					if (num9 < num3)
					{
						num9++;
					}
					Main.tile[num8, num9].wire = true;
				}
				return true;
			}
			if (type != 1)
			{
				if (type == 2)
				{
					int num10 = Main.rand.Next(4, 7);
					int num11 = x2 + Main.rand.Next(-1, 2);
					int num12 = num;
					for (int i = 0; i < num10; i++)
					{
						num12++;
						if (!WorldGen.SolidTile(num11, num12))
						{
							return false;
						}
					}
					for (int j = num11 - 2; j <= num11 + 2; j++)
					{
						for (int k = num12 - 2; k <= num12 + 2; k++)
						{
							if (!WorldGen.SolidTile(j, k))
							{
								return false;
							}
						}
					}
					WorldGen.KillTile(num11, num12, false, false, false);
					Main.tile[num11, num12].active = true;
					Main.tile[num11, num12].type = 141;
					Main.tile[num11, num12].frameX = 0;
					Main.tile[num11, num12].frameY = (short)(18 * Main.rand.Next(2));
					WorldGen.PlaceTile(x2, num, 135, true, true, -1, WorldGen.genRand.Next(2, 4));
					int num13 = x2;
					int num14 = num;
					while (num13 != num11 || num14 != num12)
					{
						Main.tile[num13, num14].wire = true;
						if (num13 > num11)
						{
							num13--;
						}
						if (num13 < num11)
						{
							num13++;
						}
						Main.tile[num13, num14].wire = true;
						if (num14 > num12)
						{
							num14--;
						}
						if (num14 < num12)
						{
							num14++;
						}
						Main.tile[num13, num14].wire = true;
					}
				}
				return false;
			}
			int num15 = num - 8;
			int num16 = x2 + WorldGen.genRand.Next(-1, 2);
			bool flag3 = true;
			while (flag3)
			{
				bool flag4 = true;
				int num17 = 0;
				for (int l = num16 - 2; l <= num16 + 3; l++)
				{
					for (int m = num15; m <= num15 + 3; m++)
					{
						if (!WorldGen.SolidTile(l, m))
						{
							flag4 = false;
						}
						if (Main.tile[l, m].active && (Main.tile[l, m].type == 0 || Main.tile[l, m].type == 1 || Main.tile[l, m].type == 59))
						{
							num17++;
						}
					}
				}
				num15--;
				if ((double)num15 < Main.worldSurface)
				{
					return false;
				}
				if (flag4 && num17 > 2)
				{
					flag3 = false;
				}
			}
			if (num - num15 <= 5 || num - num15 >= 40)
			{
				return false;
			}
			for (int n = num16; n <= num16 + 1; n++)
			{
				for (int num18 = num15; num18 <= num; num18++)
				{
					if (WorldGen.SolidTile(n, num18))
					{
						WorldGen.KillTile(n, num18, false, false, false);
					}
				}
			}
			for (int num19 = num16 - 2; num19 <= num16 + 3; num19++)
			{
				for (int num20 = num15 - 2; num20 <= num15 + 3; num20++)
				{
					if (WorldGen.SolidTile(num19, num20))
					{
						Main.tile[num19, num20].type = 1;
					}
				}
			}
			WorldGen.PlaceTile(x2, num, 135, true, true, -1, WorldGen.genRand.Next(2, 4));
			WorldGen.PlaceTile(num16, num15 + 2, 130, true, false, -1, 0);
			WorldGen.PlaceTile(num16 + 1, num15 + 2, 130, true, false, -1, 0);
			WorldGen.PlaceTile(num16 + 1, num15 + 1, 138, true, false, -1, 0);
			num15 += 2;
			Main.tile[num16, num15].wire = true;
			Main.tile[num16 + 1, num15].wire = true;
			num15++;
			WorldGen.PlaceTile(num16, num15, 130, true, false, -1, 0);
			WorldGen.PlaceTile(num16 + 1, num15, 130, true, false, -1, 0);
			Main.tile[num16, num15].wire = true;
			Main.tile[num16 + 1, num15].wire = true;
			WorldGen.PlaceTile(num16, num15 + 1, 130, true, false, -1, 0);
			WorldGen.PlaceTile(num16 + 1, num15 + 1, 130, true, false, -1, 0);
			Main.tile[num16, num15 + 1].wire = true;
			Main.tile[num16 + 1, num15 + 1].wire = true;
			int num21 = x2;
			int num22 = num;
			while (num21 != num16 || num22 != num15)
			{
				Main.tile[num21, num22].wire = true;
				if (num21 > num16)
				{
					num21--;
				}
				if (num21 < num16)
				{
					num21++;
				}
				Main.tile[num21, num22].wire = true;
				if (num22 > num15)
				{
					num22--;
				}
				if (num22 < num15)
				{
					num22++;
				}
				Main.tile[num21, num22].wire = true;
			}
			return true;
		}
		public static void generateWorld(int seed = -1)
		{
			Main.checkXMas();
			NPC.clrNames();
			NPC.setNames();
			WorldGen.gen = true;
			WorldGen.resetGen();
			if (seed > 0)
			{
				WorldGen.genRand = new Random(seed);
			}
			else
			{
				WorldGen.genRand = new Random((int)DateTime.Now.Ticks);
			}
			Main.worldID = WorldGen.genRand.Next(2147483647);
			int num = 0;
			int num2 = 0;
			double num3 = (double)Main.maxTilesY * 0.3;
			num3 *= (double)WorldGen.genRand.Next(90, 110) * 0.005;
			double num4 = num3 + (double)Main.maxTilesY * 0.2;
			num4 *= (double)WorldGen.genRand.Next(90, 110) * 0.01;
			double num5 = num3;
			double num6 = num3;
			double num7 = num4;
			double num8 = num4;
			int num9 = 0;
			if (WorldGen.genRand.Next(2) == 0)
			{
				num9 = -1;
			}
			else
			{
				num9 = 1;
			}
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				float num10 = (float)i / (float)Main.maxTilesX;
				Main.statusText = "Generating world terrain: " + (int)(num10 * 100f + 1f) + "%";
				if (num3 < num5)
				{
					num5 = num3;
				}
				if (num3 > num6)
				{
					num6 = num3;
				}
				if (num4 < num7)
				{
					num7 = num4;
				}
				if (num4 > num8)
				{
					num8 = num4;
				}
				if (num2 <= 0)
				{
					num = WorldGen.genRand.Next(0, 5);
					num2 = WorldGen.genRand.Next(5, 40);
					if (num == 0)
					{
						num2 *= (int)((double)WorldGen.genRand.Next(5, 30) * 0.2);
					}
				}
				num2--;
				if (num == 0)
				{
					while (WorldGen.genRand.Next(0, 7) == 0)
					{
						num3 += (double)WorldGen.genRand.Next(-1, 2);
					}
				}
				else
				{
					if (num == 1)
					{
						while (WorldGen.genRand.Next(0, 4) == 0)
						{
							num3 -= 1.0;
						}
						while (WorldGen.genRand.Next(0, 10) == 0)
						{
							num3 += 1.0;
						}
					}
					else
					{
						if (num == 2)
						{
							while (WorldGen.genRand.Next(0, 4) == 0)
							{
								num3 += 1.0;
							}
							while (WorldGen.genRand.Next(0, 10) == 0)
							{
								num3 -= 1.0;
							}
						}
						else
						{
							if (num == 3)
							{
								while (WorldGen.genRand.Next(0, 2) == 0)
								{
									num3 -= 1.0;
								}
								while (WorldGen.genRand.Next(0, 6) == 0)
								{
									num3 += 1.0;
								}
							}
							else
							{
								if (num == 4)
								{
									while (WorldGen.genRand.Next(0, 2) == 0)
									{
										num3 += 1.0;
									}
									while (WorldGen.genRand.Next(0, 5) == 0)
									{
										num3 -= 1.0;
									}
								}
							}
						}
					}
				}
				if (num3 < (double)Main.maxTilesY * 0.17)
				{
					num3 = (double)Main.maxTilesY * 0.17;
					num2 = 0;
				}
				else
				{
					if (num3 > (double)Main.maxTilesY * 0.3)
					{
						num3 = (double)Main.maxTilesY * 0.3;
						num2 = 0;
					}
				}
				if ((i < 275 || i > Main.maxTilesX - 275) && num3 > (double)Main.maxTilesY * 0.25)
				{
					num3 = (double)Main.maxTilesY * 0.25;
					num2 = 1;
				}
				while (WorldGen.genRand.Next(0, 3) == 0)
				{
					num4 += (double)WorldGen.genRand.Next(-2, 3);
				}
				if (num4 < num3 + (double)Main.maxTilesY * 0.05)
				{
					num4 += 1.0;
				}
				if (num4 > num3 + (double)Main.maxTilesY * 0.35)
				{
					num4 -= 1.0;
				}
				int num11 = 0;
				while ((double)num11 < num3)
				{
					Main.tile[i, num11].active = false;
					Main.tile[i, num11].frameX = -1;
					Main.tile[i, num11].frameY = -1;
					num11++;
				}
				for (int j = (int)num3; j < Main.maxTilesY; j++)
				{
					if ((double)j < num4)
					{
						Main.tile[i, j].active = true;
						Main.tile[i, j].type = 0;
						Main.tile[i, j].frameX = -1;
						Main.tile[i, j].frameY = -1;
					}
					else
					{
						Main.tile[i, j].active = true;
						Main.tile[i, j].type = 1;
						Main.tile[i, j].frameX = -1;
						Main.tile[i, j].frameY = -1;
					}
				}
			}
			Main.worldSurface = num6 + 25.0;
			Main.rockLayer = num8;
			double num12 = (double)((int)((Main.rockLayer - Main.worldSurface) / 6.0) * 6);
			Main.rockLayer = Main.worldSurface + num12;
			WorldGen.waterLine = (int)(Main.rockLayer + (double)Main.maxTilesY) / 2;
			WorldGen.waterLine += WorldGen.genRand.Next(-100, 20);
			WorldGen.lavaLine = WorldGen.waterLine + WorldGen.genRand.Next(50, 80);
			int num13 = 0;
			for (int k = 0; k < (int)((double)Main.maxTilesX * 0.0015); k++)
			{
				int[] array = new int[10];
				int[] array2 = new int[10];
				int num14 = WorldGen.genRand.Next(450, Main.maxTilesX - 450);
				int num15 = 0;
				for (int l = 0; l < 10; l++)
				{
					while (!Main.tile[num14, num15].active)
					{
						num15++;
					}
					array[l] = num14;
					array2[l] = num15 - WorldGen.genRand.Next(11, 16);
					num14 += WorldGen.genRand.Next(5, 11);
				}
				for (int m = 0; m < 10; m++)
				{
					WorldGen.TileRunner(array[m], array2[m], (double)WorldGen.genRand.Next(5, 8), WorldGen.genRand.Next(6, 9), 0, true, -2f, -0.3f, false, true);
					WorldGen.TileRunner(array[m], array2[m], (double)WorldGen.genRand.Next(5, 8), WorldGen.genRand.Next(6, 9), 0, true, 2f, -0.3f, false, true);
				}
			}
			Main.statusText = "Adding sand...";
			int num16 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.0008), (int)((double)Main.maxTilesX * 0.0025));
			num16 += 2;
			for (int n = 0; n < num16; n++)
			{
				int num17 = WorldGen.genRand.Next(Main.maxTilesX);
				while ((float)num17 > (float)Main.maxTilesX * 0.4f && (float)num17 < (float)Main.maxTilesX * 0.6f)
				{
					num17 = WorldGen.genRand.Next(Main.maxTilesX);
				}
				int num18 = WorldGen.genRand.Next(35, 90);
				if (n == 1)
				{
					float num19 = (float)(Main.maxTilesX / 4200);
					num18 += (int)((float)WorldGen.genRand.Next(20, 40) * num19);
				}
				if (WorldGen.genRand.Next(3) == 0)
				{
					num18 *= 2;
				}
				if (n == 1)
				{
					num18 *= 2;
				}
				int num20 = num17 - num18;
				num18 = WorldGen.genRand.Next(35, 90);
				if (WorldGen.genRand.Next(3) == 0)
				{
					num18 *= 2;
				}
				if (n == 1)
				{
					num18 *= 2;
				}
				int num21 = num17 + num18;
				if (num20 < 0)
				{
					num20 = 0;
				}
				if (num21 > Main.maxTilesX)
				{
					num21 = Main.maxTilesX;
				}
				if (n == 0)
				{
					num20 = 0;
					num21 = WorldGen.genRand.Next(260, 300);
					if (num9 == 1)
					{
						num21 += 40;
					}
				}
				else
				{
					if (n == 2)
					{
						num20 = Main.maxTilesX - WorldGen.genRand.Next(260, 300);
						num21 = Main.maxTilesX;
						if (num9 == -1)
						{
							num20 -= 40;
						}
					}
				}
				int num22 = WorldGen.genRand.Next(50, 100);
				for (int num23 = num20; num23 < num21; num23++)
				{
					if (WorldGen.genRand.Next(2) == 0)
					{
						num22 += WorldGen.genRand.Next(-1, 2);
						if (num22 < 50)
						{
							num22 = 50;
						}
						if (num22 > 100)
						{
							num22 = 100;
						}
					}
					int num24 = 0;
					while ((double)num24 < Main.worldSurface)
					{
						if (Main.tile[num23, num24].active)
						{
							int num25 = num22;
							if (num23 - num20 < num25)
							{
								num25 = num23 - num20;
							}
							if (num21 - num23 < num25)
							{
								num25 = num21 - num23;
							}
							num25 += WorldGen.genRand.Next(5);
							for (int num26 = num24; num26 < num24 + num25; num26++)
							{
								if (num23 > num20 + WorldGen.genRand.Next(5) && num23 < num21 - WorldGen.genRand.Next(5))
								{
									Main.tile[num23, num26].type = 53;
								}
							}
							break;
						}
						num24++;
					}
				}
			}
			for (int num27 = 0; num27 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 8E-06); num27++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)Main.worldSurface, (int)Main.rockLayer), (double)WorldGen.genRand.Next(15, 70), WorldGen.genRand.Next(20, 130), 53, false, 0f, 0f, false, true);
			}
			WorldGen.numMCaves = 0;
			Main.statusText = "Generating hills...";
			for (int num28 = 0; num28 < (int)((double)Main.maxTilesX * 0.0008); num28++)
			{
				int num29 = 0;
				bool flag = false;
				bool flag2 = false;
				int num30 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.75));
				while (!flag2)
				{
					flag2 = true;
					while (num30 > Main.maxTilesX / 2 - 100 && num30 < Main.maxTilesX / 2 + 100)
					{
						num30 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.25), (int)((double)Main.maxTilesX * 0.75));
					}
					for (int num31 = 0; num31 < WorldGen.numMCaves; num31++)
					{
						if (num30 > WorldGen.mCaveX[num31] - 50 && num30 < WorldGen.mCaveX[num31] + 50)
						{
							num29++;
							flag2 = false;
							break;
						}
					}
					if (num29 >= 200)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					int num32 = 0;
					while ((double)num32 < Main.worldSurface)
					{
						if (Main.tile[num30, num32].active)
						{
							WorldGen.Mountinater(num30, num32);
							WorldGen.mCaveX[WorldGen.numMCaves] = num30;
							WorldGen.mCaveY[WorldGen.numMCaves] = num32;
							WorldGen.numMCaves++;
							break;
						}
						num32++;
					}
				}
			}
			bool flag3 = false;
			if (Main.xMas)
			{
				flag3 = true;
			}
			else
			{
				if (WorldGen.genRand.Next(3) == 0)
				{
					flag3 = true;
				}
			}
			if (flag3)
			{
				Main.statusText = "Adding snow...";
				int num33 = WorldGen.genRand.Next(Main.maxTilesX);
				while ((float)num33 < (float)Main.maxTilesX * 0.35f || (float)num33 > (float)Main.maxTilesX * 0.65f)
				{
					num33 = WorldGen.genRand.Next(Main.maxTilesX);
				}
				int num34 = WorldGen.genRand.Next(35, 90);
				float num35 = (float)(Main.maxTilesX / 4200);
				num34 += (int)((float)WorldGen.genRand.Next(20, 40) * num35);
				num34 += (int)((float)WorldGen.genRand.Next(20, 40) * num35);
				int num36 = num33 - num34;
				num34 = WorldGen.genRand.Next(35, 90);
				num34 += (int)((float)WorldGen.genRand.Next(20, 40) * num35);
				num34 += (int)((float)WorldGen.genRand.Next(20, 40) * num35);
				int num37 = num33 + num34;
				if (num36 < 0)
				{
					num36 = 0;
				}
				if (num37 > Main.maxTilesX)
				{
					num37 = Main.maxTilesX;
				}
				int num38 = WorldGen.genRand.Next(50, 100);
				for (int num39 = num36; num39 < num37; num39++)
				{
					if (WorldGen.genRand.Next(2) == 0)
					{
						num38 += WorldGen.genRand.Next(-1, 2);
						if (num38 < 50)
						{
							num38 = 50;
						}
						if (num38 > 100)
						{
							num38 = 100;
						}
					}
					int num40 = 0;
					while ((double)num40 < Main.worldSurface)
					{
						if (Main.tile[num39, num40].active)
						{
							int num41 = num38;
							if (num39 - num36 < num41)
							{
								num41 = num39 - num36;
							}
							if (num37 - num39 < num41)
							{
								num41 = num37 - num39;
							}
							num41 += WorldGen.genRand.Next(5);
							for (int num42 = num40; num42 < num40 + num41; num42++)
							{
								if (num39 > num36 + WorldGen.genRand.Next(5) && num39 < num37 - WorldGen.genRand.Next(5))
								{
									Main.tile[num39, num42].type = 147;
								}
							}
							break;
						}
						num40++;
					}
				}
			}
			for (int num43 = 1; num43 < Main.maxTilesX - 1; num43++)
			{
				float num44 = (float)num43 / (float)Main.maxTilesX;
				Main.statusText = "Puttin dirt behind dirt: " + (int)(num44 * 100f + 1f) + "%";
				bool flag4 = false;
				num13 += WorldGen.genRand.Next(-1, 2);
				if (num13 < 0)
				{
					num13 = 0;
				}
				if (num13 > 10)
				{
					num13 = 10;
				}
				int num45 = 0;
				while ((double)num45 < Main.worldSurface + 10.0 && (double)num45 <= Main.worldSurface + (double)num13)
				{
					if (flag4)
					{
						Main.tile[num43, num45].wall = 2;
					}
					if (Main.tile[num43, num45].active && Main.tile[num43 - 1, num45].active && Main.tile[num43 + 1, num45].active && Main.tile[num43, num45 + 1].active && Main.tile[num43 - 1, num45 + 1].active && Main.tile[num43 + 1, num45 + 1].active)
					{
						flag4 = true;
					}
					num45++;
				}
			}
			Main.statusText = "Placing rocks in the dirt...";
			for (int num46 = 0; num46 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); num46++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)num5 + 1), (double)WorldGen.genRand.Next(4, 15), WorldGen.genRand.Next(5, 40), 1, false, 0f, 0f, false, true);
			}
			for (int num47 = 0; num47 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); num47++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num5, (int)num6 + 1), (double)WorldGen.genRand.Next(4, 10), WorldGen.genRand.Next(5, 30), 1, false, 0f, 0f, false, true);
			}
			for (int num48 = 0; num48 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0045); num48++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, (int)num8 + 1), (double)WorldGen.genRand.Next(2, 7), WorldGen.genRand.Next(2, 23), 1, false, 0f, 0f, false, true);
			}
			Main.statusText = "Placing dirt in the rocks...";
			for (int num49 = 0; num49 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.005); num49++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(2, 6), WorldGen.genRand.Next(2, 40), 0, false, 0f, 0f, false, true);
			}
			Main.statusText = "Adding clay...";
			for (int num50 = 0; num50 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05); num50++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)num5), (double)WorldGen.genRand.Next(4, 14), WorldGen.genRand.Next(10, 50), 40, false, 0f, 0f, false, true);
			}
			for (int num51 = 0; num51 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 5E-05); num51++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num5, (int)num6 + 1), (double)WorldGen.genRand.Next(8, 14), WorldGen.genRand.Next(15, 45), 40, false, 0f, 0f, false, true);
			}
			for (int num52 = 0; num52 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05); num52++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, (int)num8 + 1), (double)WorldGen.genRand.Next(8, 15), WorldGen.genRand.Next(5, 50), 40, false, 0f, 0f, false, true);
			}
			for (int num53 = 5; num53 < Main.maxTilesX - 5; num53++)
			{
				int num54 = 1;
				while ((double)num54 < Main.worldSurface - 1.0)
				{
					if (Main.tile[num53, num54].active)
					{
						for (int num55 = num54; num55 < num54 + 5; num55++)
						{
							if (Main.tile[num53, num55].type == 40)
							{
								Main.tile[num53, num55].type = 0;
							}
						}
						break;
					}
					num54++;
				}
			}
			int num56 = 0;
			for (int num57 = 0; num57 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0015); num57++)
			{
				float num58 = (float)((double)num57 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.0015));
				Main.statusText = "Making random holes: " + (int)(num58 * 100f + 1f) + "%";
				int type = -1;
				if (WorldGen.genRand.Next(5) == 0)
				{
					type = -2;
				}
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, Main.maxTilesY), (double)WorldGen.genRand.Next(2, 5), WorldGen.genRand.Next(2, 20), type, false, 0f, 0f, false, true);
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, Main.maxTilesY), (double)WorldGen.genRand.Next(8, 15), WorldGen.genRand.Next(7, 30), type, false, 0f, 0f, false, true);
			}
			for (int num59 = 0; num59 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 3E-05); num59++)
			{
				float num60 = (float)((double)num59 / ((double)(Main.maxTilesX * Main.maxTilesY) * 3E-05));
				Main.statusText = "Generating small caves: " + (int)(num60 * 100f + 1f) + "%";
				if (num8 <= (double)Main.maxTilesY)
				{
					int type2 = -1;
					if (WorldGen.genRand.Next(6) == 0)
					{
						type2 = -2;
					}
					WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num5, (int)num8 + 1), (double)WorldGen.genRand.Next(5, 15), WorldGen.genRand.Next(30, 200), type2, false, 0f, 0f, false, true);
				}
			}
			for (int num61 = 0; num61 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013); num61++)
			{
				float num62 = (float)((double)num61 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.00013));
				Main.statusText = "Generating large caves: " + (int)(num62 * 100f + 1f) + "%";
				if (num8 <= (double)Main.maxTilesY)
				{
					int type3 = -1;
					if (WorldGen.genRand.Next(10) == 0)
					{
						type3 = -2;
					}
					WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num8, Main.maxTilesY), (double)WorldGen.genRand.Next(6, 20), WorldGen.genRand.Next(50, 300), type3, false, 0f, 0f, false, true);
				}
			}
			Main.statusText = "Generating surface caves...";
			for (int num63 = 0; num63 < (int)((double)Main.maxTilesX * 0.0025); num63++)
			{
				num56 = WorldGen.genRand.Next(0, Main.maxTilesX);
				int num64 = 0;
				while ((double)num64 < num6)
				{
					if (Main.tile[num56, num64].active)
					{
						WorldGen.TileRunner(num56, num64, (double)WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(5, 50), -1, false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 1f, false, true);
						break;
					}
					num64++;
				}
			}
			for (int num65 = 0; num65 < (int)((double)Main.maxTilesX * 0.0007); num65++)
			{
				num56 = WorldGen.genRand.Next(0, Main.maxTilesX);
				int num66 = 0;
				while ((double)num66 < num6)
				{
					if (Main.tile[num56, num66].active)
					{
						WorldGen.TileRunner(num56, num66, (double)WorldGen.genRand.Next(10, 15), WorldGen.genRand.Next(50, 130), -1, false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 2f, false, true);
						break;
					}
					num66++;
				}
			}
			for (int num67 = 0; num67 < (int)((double)Main.maxTilesX * 0.0003); num67++)
			{
				num56 = WorldGen.genRand.Next(0, Main.maxTilesX);
				int num68 = 0;
				while ((double)num68 < num6)
				{
					if (Main.tile[num56, num68].active)
					{
						WorldGen.TileRunner(num56, num68, (double)WorldGen.genRand.Next(12, 25), WorldGen.genRand.Next(150, 500), -1, false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 4f, false, true);
						WorldGen.TileRunner(num56, num68, (double)WorldGen.genRand.Next(8, 17), WorldGen.genRand.Next(60, 200), -1, false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 2f, false, true);
						WorldGen.TileRunner(num56, num68, (double)WorldGen.genRand.Next(5, 13), WorldGen.genRand.Next(40, 170), -1, false, (float)WorldGen.genRand.Next(-10, 11) * 0.1f, 2f, false, true);
						break;
					}
					num68++;
				}
			}
			for (int num69 = 0; num69 < (int)((double)Main.maxTilesX * 0.0004); num69++)
			{
				num56 = WorldGen.genRand.Next(0, Main.maxTilesX);
				int num70 = 0;
				while ((double)num70 < num6)
				{
					if (Main.tile[num56, num70].active)
					{
						WorldGen.TileRunner(num56, num70, (double)WorldGen.genRand.Next(7, 12), WorldGen.genRand.Next(150, 250), -1, false, 0f, 1f, true, true);
						break;
					}
					num70++;
				}
			}
			float num71 = (float)(Main.maxTilesX / 4200);
			int num72 = 0;
			while ((float)num72 < 5f * num71)
			{
				try
				{
					WorldGen.Caverer(WorldGen.genRand.Next(100, Main.maxTilesX - 100), WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 400));
				}
				catch
				{
				}
				num72++;
			}
			for (int num73 = 0; num73 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.002); num73++)
			{
				int num74 = WorldGen.genRand.Next(1, Main.maxTilesX - 1);
				int num75 = WorldGen.genRand.Next((int)num5, (int)num6);
				if (num75 >= Main.maxTilesY)
				{
					num75 = Main.maxTilesY - 2;
				}
				if (Main.tile[num74 - 1, num75].active && Main.tile[num74 - 1, num75].type == 0 && Main.tile[num74 + 1, num75].active && Main.tile[num74 + 1, num75].type == 0 && Main.tile[num74, num75 - 1].active && Main.tile[num74, num75 - 1].type == 0 && Main.tile[num74, num75 + 1].active && Main.tile[num74, num75 + 1].type == 0)
				{
					Main.tile[num74, num75].active = true;
					Main.tile[num74, num75].type = 2;
				}
				num74 = WorldGen.genRand.Next(1, Main.maxTilesX - 1);
				num75 = WorldGen.genRand.Next(0, (int)num5);
				if (num75 >= Main.maxTilesY)
				{
					num75 = Main.maxTilesY - 2;
				}
				if (Main.tile[num74 - 1, num75].active && Main.tile[num74 - 1, num75].type == 0 && Main.tile[num74 + 1, num75].active && Main.tile[num74 + 1, num75].type == 0 && Main.tile[num74, num75 - 1].active && Main.tile[num74, num75 - 1].type == 0 && Main.tile[num74, num75 + 1].active && Main.tile[num74, num75 + 1].type == 0)
				{
					Main.tile[num74, num75].active = true;
					Main.tile[num74, num75].type = 2;
				}
			}
			Main.statusText = "Generating jungle: 0%";
			float num76 = (float)(Main.maxTilesX / 4200);
			num76 *= 1.5f;
			int num77 = 0;
			float num78 = (float)WorldGen.genRand.Next(15, 30) * 0.01f;
			if (num9 == -1)
			{
				num78 = 1f - num78;
				num77 = (int)((float)Main.maxTilesX * num78);
			}
			else
			{
				num77 = (int)((float)Main.maxTilesX * num78);
			}
			int num79 = (int)((double)Main.maxTilesY + Main.rockLayer) / 2;
			num77 += WorldGen.genRand.Next((int)(-100f * num76), (int)(101f * num76));
			num79 += WorldGen.genRand.Next((int)(-100f * num76), (int)(101f * num76));
			int num80 = num77;
			int num81 = num79;
			WorldGen.TileRunner(num77, num79, (double)WorldGen.genRand.Next((int)(250f * num76), (int)(500f * num76)), WorldGen.genRand.Next(50, 150), 59, false, (float)(num9 * 3), 0f, false, true);
			int num82 = 0;
			while ((float)num82 < 6f * num76)
			{
				WorldGen.TileRunner(num77 + WorldGen.genRand.Next(-(int)(125f * num76), (int)(125f * num76)), num79 + WorldGen.genRand.Next(-(int)(125f * num76), (int)(125f * num76)), (double)WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(63, 65), false, 0f, 0f, false, true);
				num82++;
			}
			WorldGen.mudWall = true;
			Main.statusText = "Generating jungle: 15%";
			num77 += WorldGen.genRand.Next((int)(-250f * num76), (int)(251f * num76));
			num79 += WorldGen.genRand.Next((int)(-150f * num76), (int)(151f * num76));
			int num83 = num77;
			int num84 = num79;
			int num85 = num77;
			int num86 = num79;
			WorldGen.TileRunner(num77, num79, (double)WorldGen.genRand.Next((int)(250f * num76), (int)(500f * num76)), WorldGen.genRand.Next(50, 150), 59, false, 0f, 0f, false, true);
			WorldGen.mudWall = false;
			int num87 = 0;
			while ((float)num87 < 6f * num76)
			{
				WorldGen.TileRunner(num77 + WorldGen.genRand.Next(-(int)(125f * num76), (int)(125f * num76)), num79 + WorldGen.genRand.Next(-(int)(125f * num76), (int)(125f * num76)), (double)WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(65, 67), false, 0f, 0f, false, true);
				num87++;
			}
			WorldGen.mudWall = true;
			Main.statusText = "Generating jungle: 30%";
			num77 += WorldGen.genRand.Next((int)(-400f * num76), (int)(401f * num76));
			num79 += WorldGen.genRand.Next((int)(-150f * num76), (int)(151f * num76));
			int num88 = num77;
			int num89 = num79;
			WorldGen.TileRunner(num77, num79, (double)WorldGen.genRand.Next((int)(250f * num76), (int)(500f * num76)), WorldGen.genRand.Next(50, 150), 59, false, (float)(num9 * -3), 0f, false, true);
			WorldGen.mudWall = false;
			int num90 = 0;
			while ((float)num90 < 6f * num76)
			{
				WorldGen.TileRunner(num77 + WorldGen.genRand.Next(-(int)(125f * num76), (int)(125f * num76)), num79 + WorldGen.genRand.Next(-(int)(125f * num76), (int)(125f * num76)), (double)WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(67, 69), false, 0f, 0f, false, true);
				num90++;
			}
			WorldGen.mudWall = true;
			Main.statusText = "Generating jungle: 45%";
			num77 = (num80 + num83 + num88) / 3;
			num79 = (num81 + num84 + num89) / 3;
			WorldGen.TileRunner(num77, num79, (double)WorldGen.genRand.Next((int)(400f * num76), (int)(600f * num76)), 10000, 59, false, 0f, -20f, true, true);
			WorldGen.JungleRunner(num77, num79);
			Main.statusText = "Generating jungle: 60%";
			WorldGen.mudWall = false;
			for (int num91 = 0; num91 < Main.maxTilesX / 10; num91++)
			{
				num77 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
				num79 = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 200);
				while (Main.tile[num77, num79].wall != 15)
				{
					num77 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
					num79 = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 200);
				}
				WorldGen.MudWallRunner(num77, num79);
			}
			num77 = num85;
			num79 = num86;
			int num92 = 0;
			while ((float)num92 <= 20f * num76)
			{
				Main.statusText = "Generating jungle: " + (int)(60f + (float)num92 / num76) + "%";
				num77 += WorldGen.genRand.Next((int)(-5f * num76), (int)(6f * num76));
				num79 += WorldGen.genRand.Next((int)(-5f * num76), (int)(6f * num76));
				WorldGen.TileRunner(num77, num79, (double)WorldGen.genRand.Next(40, 100), WorldGen.genRand.Next(300, 500), 59, false, 0f, 0f, false, true);
				num92++;
			}
			int num93 = 0;
			while ((float)num93 <= 10f * num76)
			{
				Main.statusText = "Generating jungle: " + (int)(80f + (float)num93 / num76 * 2f) + "%";
				num77 = num85 + WorldGen.genRand.Next((int)(-600f * num76), (int)(600f * num76));
				num79 = num86 + WorldGen.genRand.Next((int)(-200f * num76), (int)(200f * num76));
				while (num77 < 1 || num77 >= Main.maxTilesX - 1 || num79 < 1 || num79 >= Main.maxTilesY - 1 || Main.tile[num77, num79].type != 59)
				{
					num77 = num85 + WorldGen.genRand.Next((int)(-600f * num76), (int)(600f * num76));
					num79 = num86 + WorldGen.genRand.Next((int)(-200f * num76), (int)(200f * num76));
				}
				int num94 = 0;
				while ((float)num94 < 8f * num76)
				{
					num77 += WorldGen.genRand.Next(-30, 31);
					num79 += WorldGen.genRand.Next(-30, 31);
					int type4 = -1;
					if (WorldGen.genRand.Next(7) == 0)
					{
						type4 = -2;
					}
					WorldGen.TileRunner(num77, num79, (double)WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(30, 70), type4, false, 0f, 0f, false, true);
					num94++;
				}
				num93++;
			}
			int num95 = 0;
			while ((float)num95 <= 300f * num76)
			{
				num77 = num85 + WorldGen.genRand.Next((int)(-600f * num76), (int)(600f * num76));
				num79 = num86 + WorldGen.genRand.Next((int)(-200f * num76), (int)(200f * num76));
				while (num77 < 1 || num77 >= Main.maxTilesX - 1 || num79 < 1 || num79 >= Main.maxTilesY - 1 || Main.tile[num77, num79].type != 59)
				{
					num77 = num85 + WorldGen.genRand.Next((int)(-600f * num76), (int)(600f * num76));
					num79 = num86 + WorldGen.genRand.Next((int)(-200f * num76), (int)(200f * num76));
				}
				WorldGen.TileRunner(num77, num79, (double)WorldGen.genRand.Next(4, 10), WorldGen.genRand.Next(5, 30), 1, false, 0f, 0f, false, true);
				if (WorldGen.genRand.Next(4) == 0)
				{
					int type5 = WorldGen.genRand.Next(63, 69);
					WorldGen.TileRunner(num77 + WorldGen.genRand.Next(-1, 2), num79 + WorldGen.genRand.Next(-1, 2), (double)WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(4, 8), type5, false, 0f, 0f, false, true);
				}
				num95++;
			}
			num77 = num85;
			num79 = num86;
			float num96 = (float)WorldGen.genRand.Next(6, 10);
			float num97 = (float)(Main.maxTilesX / 4200);
			num96 *= num97;
			int num98 = 0;
			while ((float)num98 < num96)
			{
				bool flag5 = true;
				while (flag5)
				{
					num77 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
					num79 = WorldGen.genRand.Next((int)(Main.worldSurface + Main.rockLayer) / 2, Main.maxTilesY - 300);
					if (Main.tile[num77, num79].type == 59)
					{
						flag5 = false;
						int num99 = WorldGen.genRand.Next(2, 4);
						int num100 = WorldGen.genRand.Next(2, 4);
						for (int num101 = num77 - num99 - 1; num101 <= num77 + num99 + 1; num101++)
						{
							for (int num102 = num79 - num100 - 1; num102 <= num79 + num100 + 1; num102++)
							{
								Main.tile[num101, num102].active = true;
								Main.tile[num101, num102].type = 45;
								Main.tile[num101, num102].liquid = 0;
								Main.tile[num101, num102].lava = false;
							}
						}
						for (int num103 = num77 - num99; num103 <= num77 + num99; num103++)
						{
							for (int num104 = num79 - num100; num104 <= num79 + num100; num104++)
							{
								Main.tile[num103, num104].active = false;
							}
						}
						bool flag6 = false;
						int num105 = 0;
						while (!flag6 && num105 < 100)
						{
							num105++;
							int num106 = WorldGen.genRand.Next(num77 - num99, num77 + num99 + 1);
							int num107 = WorldGen.genRand.Next(num79 - num100, num79 + num100 - 2);
							WorldGen.PlaceTile(num106, num107, 4, true, false, -1, 0);
							if (Main.tile[num106, num107].type == 4)
							{
								flag6 = true;
							}
						}
						for (int num108 = num77 - num99 - 1; num108 <= num77 + num99 + 1; num108++)
						{
							for (int num109 = num79 + num100 - 2; num109 <= num79 + num100; num109++)
							{
								Main.tile[num108, num109].active = false;
							}
						}
						for (int num110 = num77 - num99 - 1; num110 <= num77 + num99 + 1; num110++)
						{
							for (int num111 = num79 + num100 - 2; num111 <= num79 + num100 - 1; num111++)
							{
								Main.tile[num110, num111].active = false;
							}
						}
						for (int num112 = num77 - num99 - 1; num112 <= num77 + num99 + 1; num112++)
						{
							int num113 = 4;
							int num114 = num79 + num100 + 2;
							while (!Main.tile[num112, num114].active && num114 < Main.maxTilesY && num113 > 0)
							{
								Main.tile[num112, num114].active = true;
								Main.tile[num112, num114].type = 59;
								num114++;
								num113--;
							}
						}
						num99 -= WorldGen.genRand.Next(1, 3);
						int num115 = num79 - num100 - 2;
						while (num99 > -1)
						{
							for (int num116 = num77 - num99 - 1; num116 <= num77 + num99 + 1; num116++)
							{
								Main.tile[num116, num115].active = true;
								Main.tile[num116, num115].type = 45;
							}
							num99 -= WorldGen.genRand.Next(1, 3);
							num115--;
						}
						WorldGen.JChestX[WorldGen.numJChests] = num77;
						WorldGen.JChestY[WorldGen.numJChests] = num79;
						WorldGen.numJChests++;
					}
				}
				num98++;
			}
			for (int num117 = 0; num117 < Main.maxTilesX; num117++)
			{
				for (int num118 = 0; num118 < Main.maxTilesY; num118++)
				{
					if (Main.tile[num117, num118].active)
					{
						try
						{
							WorldGen.grassSpread = 0;
							WorldGen.SpreadGrass(num117, num118, 59, 60, true);
						}
						catch
						{
							WorldGen.grassSpread = 0;
							WorldGen.SpreadGrass(num117, num118, 59, 60, false);
						}
					}
				}
			}
			WorldGen.numIslandHouses = 0;
			WorldGen.houseCount = 0;
			Main.statusText = "Generating floating islands...";
			for (int num119 = 0; num119 < (int)((double)Main.maxTilesX * 0.0008); num119++)
			{
				int num120 = 0;
				bool flag7 = false;
				int num121 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.1), (int)((double)Main.maxTilesX * 0.9));
				bool flag8 = false;
				while (!flag8)
				{
					flag8 = true;
					while (num121 > Main.maxTilesX / 2 - 80 && num121 < Main.maxTilesX / 2 + 80)
					{
						num121 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.1), (int)((double)Main.maxTilesX * 0.9));
					}
					for (int num122 = 0; num122 < WorldGen.numIslandHouses; num122++)
					{
						if (num121 > WorldGen.fihX[num122] - 80 && num121 < WorldGen.fihX[num122] + 80)
						{
							num120++;
							flag8 = false;
							break;
						}
					}
					if (num120 >= 200)
					{
						flag7 = true;
						break;
					}
				}
				if (!flag7)
				{
					int num123 = 200;
					while ((double)num123 < Main.worldSurface)
					{
						if (Main.tile[num121, num123].active)
						{
							int num124 = num121;
							int num125 = WorldGen.genRand.Next(90, num123 - 100);
							while ((double)num125 > num5 - 50.0)
							{
								num125--;
							}
							WorldGen.FloatingIsland(num124, num125);
							WorldGen.fihX[WorldGen.numIslandHouses] = num124;
							WorldGen.fihY[WorldGen.numIslandHouses] = num125;
							WorldGen.numIslandHouses++;
							break;
						}
						num123++;
					}
				}
			}
			Main.statusText = "Adding mushroom patches...";
			for (int num126 = 0; num126 < Main.maxTilesX / 500; num126++)
			{
				int i2 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.3), (int)((double)Main.maxTilesX * 0.7));
				int j2 = WorldGen.genRand.Next((int)Main.rockLayer, Main.maxTilesY - 350);
				WorldGen.ShroomPatch(i2, j2);
			}
			for (int num127 = 0; num127 < Main.maxTilesX; num127++)
			{
				for (int num128 = (int)Main.worldSurface; num128 < Main.maxTilesY; num128++)
				{
					if (Main.tile[num127, num128].active)
					{
						WorldGen.grassSpread = 0;
						WorldGen.SpreadGrass(num127, num128, 59, 70, false);
					}
				}
			}
			Main.statusText = "Placing mud in the dirt...";
			for (int num129 = 0; num129 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.001); num129++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(2, 6), WorldGen.genRand.Next(2, 40), 59, false, 0f, 0f, false, true);
			}
			Main.statusText = "Adding silt...";
			for (int num130 = 0; num130 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0001); num130++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num8, Main.maxTilesY), (double)WorldGen.genRand.Next(5, 12), WorldGen.genRand.Next(15, 50), 123, false, 0f, 0f, false, true);
			}
			for (int num131 = 0; num131 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0005); num131++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num8, Main.maxTilesY), (double)WorldGen.genRand.Next(2, 5), WorldGen.genRand.Next(2, 5), 123, false, 0f, 0f, false, true);
			}
			Main.statusText = "Adding shinies...";
			for (int num132 = 0; num132 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 6E-05); num132++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num5, (int)num6), (double)WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(2, 6), 7, false, 0f, 0f, false, true);
			}
			for (int num133 = 0; num133 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 8E-05); num133++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, (int)num8), (double)WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(3, 7), 7, false, 0f, 0f, false, true);
			}
			for (int num134 = 0; num134 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); num134++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), 7, false, 0f, 0f, false, true);
			}
			for (int num135 = 0; num135 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 3E-05); num135++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num5, (int)num6), (double)WorldGen.genRand.Next(3, 7), WorldGen.genRand.Next(2, 5), 6, false, 0f, 0f, false, true);
			}
			for (int num136 = 0; num136 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 8E-05); num136++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, (int)num8), (double)WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(3, 6), 6, false, 0f, 0f, false, true);
			}
			for (int num137 = 0; num137 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0002); num137++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), 6, false, 0f, 0f, false, true);
			}
			for (int num138 = 0; num138 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2.6E-05); num138++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num6, (int)num8), (double)WorldGen.genRand.Next(3, 6), WorldGen.genRand.Next(3, 6), 9, false, 0f, 0f, false, true);
			}
			for (int num139 = 0; num139 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00015); num139++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), 9, false, 0f, 0f, false, true);
			}
			for (int num140 = 0; num140 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00017); num140++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)num5), (double)WorldGen.genRand.Next(4, 9), WorldGen.genRand.Next(4, 8), 9, false, 0f, 0f, false, true);
			}
			for (int num141 = 0; num141 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00012); num141++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(4, 8), WorldGen.genRand.Next(4, 8), 8, false, 0f, 0f, false, true);
			}
			for (int num142 = 0; num142 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.00012); num142++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(0, (int)num5 - 20), (double)WorldGen.genRand.Next(4, 8), WorldGen.genRand.Next(4, 8), 8, false, 0f, 0f, false, true);
			}
			for (int num143 = 0; num143 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05); num143++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next((int)num7, Main.maxTilesY), (double)WorldGen.genRand.Next(2, 4), WorldGen.genRand.Next(3, 6), 22, false, 0f, 0f, false, true);
			}
			Main.statusText = "Adding webs...";
			for (int num144 = 0; num144 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0006); num144++)
			{
				int num145 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
				int num146 = WorldGen.genRand.Next((int)num5, Main.maxTilesY - 20);
				if (num144 < WorldGen.numMCaves)
				{
					num145 = WorldGen.mCaveX[num144];
					num146 = WorldGen.mCaveY[num144];
				}
				if (!Main.tile[num145, num146].active)
				{
					if ((double)num146 <= Main.worldSurface)
					{
						if (Main.tile[num145, num146].wall <= 0)
						{
							goto IL_335A;
						}
					}
					while (!Main.tile[num145, num146].active && num146 > (int)num5)
					{
						num146--;
					}
					num146++;
					int num147 = 1;
					if (WorldGen.genRand.Next(2) == 0)
					{
						num147 = -1;
					}
					while (!Main.tile[num145, num146].active && num145 > 10 && num145 < Main.maxTilesX - 10)
					{
						num145 += num147;
					}
					num145 -= num147;
					if ((double)num146 > Main.worldSurface || Main.tile[num145, num146].wall > 0)
					{
						WorldGen.TileRunner(num145, num146, (double)WorldGen.genRand.Next(4, 11), WorldGen.genRand.Next(2, 4), 51, true, (float)num147, -1f, false, false);
					}
				}
				IL_335A:;
			}
			Main.statusText = "Creating underworld: 0%";
			int num148 = Main.maxTilesY - WorldGen.genRand.Next(150, 190);
			for (int num149 = 0; num149 < Main.maxTilesX; num149++)
			{
				num148 += WorldGen.genRand.Next(-3, 4);
				if (num148 < Main.maxTilesY - 190)
				{
					num148 = Main.maxTilesY - 190;
				}
				if (num148 > Main.maxTilesY - 160)
				{
					num148 = Main.maxTilesY - 160;
				}
				for (int num150 = num148 - 20 - WorldGen.genRand.Next(3); num150 < Main.maxTilesY; num150++)
				{
					if (num150 >= num148)
					{
						Main.tile[num149, num150].active = false;
						Main.tile[num149, num150].lava = false;
						Main.tile[num149, num150].liquid = 0;
					}
					else
					{
						Main.tile[num149, num150].type = 57;
					}
				}
			}
			int num151 = Main.maxTilesY - WorldGen.genRand.Next(40, 70);
			for (int num152 = 10; num152 < Main.maxTilesX - 10; num152++)
			{
				num151 += WorldGen.genRand.Next(-10, 11);
				if (num151 > Main.maxTilesY - 60)
				{
					num151 = Main.maxTilesY - 60;
				}
				if (num151 < Main.maxTilesY - 100)
				{
					num151 = Main.maxTilesY - 120;
				}
				for (int num153 = num151; num153 < Main.maxTilesY - 10; num153++)
				{
					if (!Main.tile[num152, num153].active)
					{
						Main.tile[num152, num153].lava = true;
						Main.tile[num152, num153].liquid = 255;
					}
				}
			}
			for (int num154 = 0; num154 < Main.maxTilesX; num154++)
			{
				if (WorldGen.genRand.Next(50) == 0)
				{
					int num155 = Main.maxTilesY - 65;
					while (!Main.tile[num154, num155].active && num155 > Main.maxTilesY - 135)
					{
						num155--;
					}
					WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), num155 + WorldGen.genRand.Next(20, 50), (double)WorldGen.genRand.Next(15, 20), 1000, 57, true, 0f, (float)WorldGen.genRand.Next(1, 3), true, true);
				}
			}
			Liquid.QuickWater(-2, -1, -1);
			for (int num156 = 0; num156 < Main.maxTilesX; num156++)
			{
				float num157 = (float)num156 / (float)(Main.maxTilesX - 1);
				Main.statusText = "Creating underworld: " + (int)(num157 * 100f / 2f + 50f) + "%";
				if (WorldGen.genRand.Next(13) == 0)
				{
					int num158 = Main.maxTilesY - 65;
					while ((Main.tile[num156, num158].liquid > 0 || Main.tile[num156, num158].active) && num158 > Main.maxTilesY - 140)
					{
						num158--;
					}
					WorldGen.TileRunner(num156, num158 - WorldGen.genRand.Next(2, 5), (double)WorldGen.genRand.Next(5, 30), 1000, 57, true, 0f, (float)WorldGen.genRand.Next(1, 3), true, true);
					float num159 = (float)WorldGen.genRand.Next(1, 3);
					if (WorldGen.genRand.Next(3) == 0)
					{
						num159 *= 0.5f;
					}
					if (WorldGen.genRand.Next(2) == 0)
					{
						WorldGen.TileRunner(num156, num158 - WorldGen.genRand.Next(2, 5), (double)((int)((float)WorldGen.genRand.Next(5, 15) * num159)), (int)((float)WorldGen.genRand.Next(10, 15) * num159), 57, true, 1f, 0.3f, false, true);
					}
					if (WorldGen.genRand.Next(2) == 0)
					{
						num159 = (float)WorldGen.genRand.Next(1, 3);
						WorldGen.TileRunner(num156, num158 - WorldGen.genRand.Next(2, 5), (double)((int)((float)WorldGen.genRand.Next(5, 15) * num159)), (int)((float)WorldGen.genRand.Next(10, 15) * num159), 57, true, -1f, 0.3f, false, true);
					}
					WorldGen.TileRunner(num156 + WorldGen.genRand.Next(-10, 10), num158 + WorldGen.genRand.Next(-10, 10), (double)WorldGen.genRand.Next(5, 15), WorldGen.genRand.Next(5, 10), -2, false, (float)WorldGen.genRand.Next(-1, 3), (float)WorldGen.genRand.Next(-1, 3), false, true);
					if (WorldGen.genRand.Next(3) == 0)
					{
						WorldGen.TileRunner(num156 + WorldGen.genRand.Next(-10, 10), num158 + WorldGen.genRand.Next(-10, 10), (double)WorldGen.genRand.Next(10, 30), WorldGen.genRand.Next(10, 20), -2, false, (float)WorldGen.genRand.Next(-1, 3), (float)WorldGen.genRand.Next(-1, 3), false, true);
					}
					if (WorldGen.genRand.Next(5) == 0)
					{
						WorldGen.TileRunner(num156 + WorldGen.genRand.Next(-15, 15), num158 + WorldGen.genRand.Next(-15, 10), (double)WorldGen.genRand.Next(15, 30), WorldGen.genRand.Next(5, 20), -2, false, (float)WorldGen.genRand.Next(-1, 3), (float)WorldGen.genRand.Next(-1, 3), false, true);
					}
				}
			}
			for (int num160 = 0; num160 < Main.maxTilesX; num160++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(20, Main.maxTilesX - 20), WorldGen.genRand.Next(Main.maxTilesY - 180, Main.maxTilesY - 10), (double)WorldGen.genRand.Next(2, 7), WorldGen.genRand.Next(2, 7), -2, false, 0f, 0f, false, true);
			}
			for (int num161 = 0; num161 < Main.maxTilesX; num161++)
			{
				if (!Main.tile[num161, Main.maxTilesY - 145].active)
				{
					Main.tile[num161, Main.maxTilesY - 145].liquid = 255;
					Main.tile[num161, Main.maxTilesY - 145].lava = true;
				}
				if (!Main.tile[num161, Main.maxTilesY - 144].active)
				{
					Main.tile[num161, Main.maxTilesY - 144].liquid = 255;
					Main.tile[num161, Main.maxTilesY - 144].lava = true;
				}
			}
			for (int num162 = 0; num162 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008); num162++)
			{
				WorldGen.TileRunner(WorldGen.genRand.Next(0, Main.maxTilesX), WorldGen.genRand.Next(Main.maxTilesY - 140, Main.maxTilesY), (double)WorldGen.genRand.Next(2, 7), WorldGen.genRand.Next(3, 7), 58, false, 0f, 0f, false, true);
			}
			WorldGen.AddHellHouses();
			int num163 = WorldGen.genRand.Next(2, (int)((double)Main.maxTilesX * 0.005));
			for (int num164 = 0; num164 < num163; num164++)
			{
				float num165 = (float)num164 / (float)num163;
				Main.statusText = "Adding water bodies: " + (int)(num165 * 100f) + "%";
				int num166 = WorldGen.genRand.Next(300, Main.maxTilesX - 300);
				while (num166 > Main.maxTilesX / 2 - 50 && num166 < Main.maxTilesX / 2 + 50)
				{
					num166 = WorldGen.genRand.Next(300, Main.maxTilesX - 300);
				}
				int num167 = (int)num5 - 20;
				while (!Main.tile[num166, num167].active)
				{
					num167++;
				}
				WorldGen.Lakinater(num166, num167);
			}
			int x = 0;
			if (num9 == -1)
			{
				x = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.05), (int)((double)Main.maxTilesX * 0.2));
				num9 = -1;
			}
			else
			{
				x = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.8), (int)((double)Main.maxTilesX * 0.95));
				num9 = 1;
			}
			int y = (int)((Main.rockLayer + (double)Main.maxTilesY) / 2.0) + WorldGen.genRand.Next(-200, 200);
			WorldGen.MakeDungeon(x, y, 41, 7);
			int num168 = 0;
			while ((double)num168 < (double)Main.maxTilesX * 0.00045)
			{
				float num169 = (float)((double)num168 / ((double)Main.maxTilesX * 0.00045));
				Main.statusText = "Making the world evil: " + (int)(num169 * 100f) + "%";
				bool flag9 = false;
				int num170 = 0;
				int num171 = 0;
				int num172 = 0;
				while (!flag9)
				{
					int num173 = 0;
					flag9 = true;
					int num174 = Main.maxTilesX / 2;
					int num175 = 200;
					num170 = WorldGen.genRand.Next(320, Main.maxTilesX - 320);
					num171 = num170 - WorldGen.genRand.Next(200) - 100;
					num172 = num170 + WorldGen.genRand.Next(200) + 100;
					if (num171 < 285)
					{
						num171 = 285;
					}
					if (num172 > Main.maxTilesX - 285)
					{
						num172 = Main.maxTilesX - 285;
					}
					if (num170 > num174 - num175 && num170 < num174 + num175)
					{
						flag9 = false;
					}
					if (num171 > num174 - num175 && num171 < num174 + num175)
					{
						flag9 = false;
					}
					if (num172 > num174 - num175 && num172 < num174 + num175)
					{
						flag9 = false;
					}
					for (int num176 = num171; num176 < num172; num176++)
					{
						for (int num177 = 0; num177 < (int)Main.worldSurface; num177 += 5)
						{
							if (Main.tile[num176, num177].active && Main.tileDungeon[(int)Main.tile[num176, num177].type])
							{
								flag9 = false;
								break;
							}
							if (!flag9)
							{
								break;
							}
						}
					}
					if (num173 < 200 && WorldGen.JungleX > num171 && WorldGen.JungleX < num172)
					{
						num173++;
						flag9 = false;
					}
				}
				int num178 = 0;
				for (int num179 = num171; num179 < num172; num179++)
				{
					if (num178 > 0)
					{
						num178--;
					}
					if (num179 == num170 || num178 == 0)
					{
						int num180 = (int)num5;
						while ((double)num180 < Main.worldSurface - 1.0)
						{
							if (Main.tile[num179, num180].active || Main.tile[num179, num180].wall > 0)
							{
								if (num179 == num170)
								{
									num178 = 20;
									WorldGen.ChasmRunner(num179, num180, WorldGen.genRand.Next(150) + 150, true);
									break;
								}
								if (WorldGen.genRand.Next(35) == 0 && num178 == 0)
								{
									num178 = 30;
									bool makeOrb = true;
									WorldGen.ChasmRunner(num179, num180, WorldGen.genRand.Next(50) + 50, makeOrb);
									break;
								}
								break;
							}
							else
							{
								num180++;
							}
						}
					}
					int num181 = (int)num5;
					while ((double)num181 < Main.worldSurface - 1.0)
					{
						if (Main.tile[num179, num181].active)
						{
							int num182 = num181 + WorldGen.genRand.Next(10, 14);
							for (int num183 = num181; num183 < num182; num183++)
							{
								if ((Main.tile[num179, num183].type == 59 || Main.tile[num179, num183].type == 60) && num179 >= num171 + WorldGen.genRand.Next(5) && num179 < num172 - WorldGen.genRand.Next(5))
								{
									Main.tile[num179, num183].type = 0;
								}
							}
							break;
						}
						num181++;
					}
				}
				double num184 = Main.worldSurface + 40.0;
				for (int num185 = num171; num185 < num172; num185++)
				{
					num184 += (double)WorldGen.genRand.Next(-2, 3);
					if (num184 < Main.worldSurface + 30.0)
					{
						num184 = Main.worldSurface + 30.0;
					}
					if (num184 > Main.worldSurface + 50.0)
					{
						num184 = Main.worldSurface + 50.0;
					}
					num56 = num185;
					bool flag10 = false;
					int num186 = (int)num5;
					while ((double)num186 < num184)
					{
						if (Main.tile[num56, num186].active)
						{
							if (Main.tile[num56, num186].type == 53 && num56 >= num171 + WorldGen.genRand.Next(5) && num56 <= num172 - WorldGen.genRand.Next(5))
							{
								Main.tile[num56, num186].type = 0;
							}
							if (Main.tile[num56, num186].type == 0 && (double)num186 < Main.worldSurface - 1.0 && !flag10)
							{
								WorldGen.grassSpread = 0;
								WorldGen.SpreadGrass(num56, num186, 0, 23, true);
							}
							flag10 = true;
							if (Main.tile[num56, num186].type == 1 && num56 >= num171 + WorldGen.genRand.Next(5) && num56 <= num172 - WorldGen.genRand.Next(5))
							{
								Main.tile[num56, num186].type = 25;
							}
							if (Main.tile[num56, num186].type == 2)
							{
								Main.tile[num56, num186].type = 23;
							}
						}
						num186++;
					}
				}
				for (int num187 = num171; num187 < num172; num187++)
				{
					for (int num188 = 0; num188 < Main.maxTilesY - 50; num188++)
					{
						if (Main.tile[num187, num188].active && Main.tile[num187, num188].type == 31)
						{
							int num189 = num187 - 13;
							int num190 = num187 + 13;
							int num191 = num188 - 13;
							int num192 = num188 + 13;
							for (int num193 = num189; num193 < num190; num193++)
							{
								if (num193 > 10 && num193 < Main.maxTilesX - 10)
								{
									for (int num194 = num191; num194 < num192; num194++)
									{
										if (Math.Abs(num193 - num187) + Math.Abs(num194 - num188) < 9 + WorldGen.genRand.Next(11) && WorldGen.genRand.Next(3) != 0 && Main.tile[num193, num194].type != 31)
										{
											Main.tile[num193, num194].active = true;
											Main.tile[num193, num194].type = 25;
											if (Math.Abs(num193 - num187) <= 1 && Math.Abs(num194 - num188) <= 1)
											{
												Main.tile[num193, num194].active = false;
											}
										}
										if (Main.tile[num193, num194].type != 31 && Math.Abs(num193 - num187) <= 2 + WorldGen.genRand.Next(3) && Math.Abs(num194 - num188) <= 2 + WorldGen.genRand.Next(3))
										{
											Main.tile[num193, num194].active = false;
										}
									}
								}
							}
						}
					}
				}
				num168++;
			}
			Main.statusText = "Generating mountain caves...";
			for (int num195 = 0; num195 < WorldGen.numMCaves; num195++)
			{
				int i3 = WorldGen.mCaveX[num195];
				int j3 = WorldGen.mCaveY[num195];
				WorldGen.CaveOpenater(i3, j3);
				WorldGen.Cavinator(i3, j3, WorldGen.genRand.Next(40, 50));
			}
			int num196 = 0;
			int num197 = 0;
			int num198 = 20;
			int num199 = Main.maxTilesX - 20;
			Main.statusText = "Creating beaches...";
			for (int num200 = 0; num200 < 2; num200++)
			{
				int num201 = 0;
				int num202 = 0;
				if (num200 == 0)
				{
					num201 = 0;
					num202 = WorldGen.genRand.Next(125, 200) + 50;
					if (num9 == 1)
					{
						num202 = 275;
					}
					int num203 = 0;
					float num204 = 1f;
					int num205 = 0;
					while (!Main.tile[num202 - 1, num205].active)
					{
						num205++;
					}
					num196 = num205;
					num205 += WorldGen.genRand.Next(1, 5);
					for (int num206 = num202 - 1; num206 >= num201; num206--)
					{
						num203++;
						if (num203 < 3)
						{
							num204 += (float)WorldGen.genRand.Next(10, 20) * 0.2f;
						}
						else
						{
							if (num203 < 6)
							{
								num204 += (float)WorldGen.genRand.Next(10, 20) * 0.15f;
							}
							else
							{
								if (num203 < 9)
								{
									num204 += (float)WorldGen.genRand.Next(10, 20) * 0.1f;
								}
								else
								{
									if (num203 < 15)
									{
										num204 += (float)WorldGen.genRand.Next(10, 20) * 0.07f;
									}
									else
									{
										if (num203 < 50)
										{
											num204 += (float)WorldGen.genRand.Next(10, 20) * 0.05f;
										}
										else
										{
											if (num203 < 75)
											{
												num204 += (float)WorldGen.genRand.Next(10, 20) * 0.04f;
											}
											else
											{
												if (num203 < 100)
												{
													num204 += (float)WorldGen.genRand.Next(10, 20) * 0.03f;
												}
												else
												{
													if (num203 < 125)
													{
														num204 += (float)WorldGen.genRand.Next(10, 20) * 0.02f;
													}
													else
													{
														if (num203 < 150)
														{
															num204 += (float)WorldGen.genRand.Next(10, 20) * 0.01f;
														}
														else
														{
															if (num203 < 175)
															{
																num204 += (float)WorldGen.genRand.Next(10, 20) * 0.005f;
															}
															else
															{
																if (num203 < 200)
																{
																	num204 += (float)WorldGen.genRand.Next(10, 20) * 0.001f;
																}
																else
																{
																	if (num203 < 230)
																	{
																		num204 += (float)WorldGen.genRand.Next(10, 20) * 0.01f;
																	}
																	else
																	{
																		if (num203 < 235)
																		{
																			num204 += (float)WorldGen.genRand.Next(10, 20) * 0.05f;
																		}
																		else
																		{
																			if (num203 < 240)
																			{
																				num204 += (float)WorldGen.genRand.Next(10, 20) * 0.1f;
																			}
																			else
																			{
																				if (num203 < 245)
																				{
																					num204 += (float)WorldGen.genRand.Next(10, 20) * 0.05f;
																				}
																				else
																				{
																					if (num203 < 255)
																					{
																						num204 += (float)WorldGen.genRand.Next(10, 20) * 0.01f;
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
						if (num203 == 235)
						{
							num199 = num206;
						}
						if (num203 == 235)
						{
							num198 = num206;
						}
						int num207 = WorldGen.genRand.Next(15, 20);
						int num208 = 0;
						while ((float)num208 < (float)num205 + num204 + (float)num207)
						{
							if ((float)num208 < (float)num205 + num204 * 0.75f - 3f)
							{
								Main.tile[num206, num208].active = false;
								if (num208 > num205)
								{
									Main.tile[num206, num208].liquid = 255;
								}
								else
								{
									if (num208 == num205)
									{
										Main.tile[num206, num208].liquid = 127;
									}
								}
							}
							else
							{
								if (num208 > num205)
								{
									Main.tile[num206, num208].type = 53;
									Main.tile[num206, num208].active = true;
								}
							}
							Main.tile[num206, num208].wall = 0;
							num208++;
						}
					}
				}
				else
				{
					num201 = Main.maxTilesX - WorldGen.genRand.Next(125, 200) - 50;
					num202 = Main.maxTilesX;
					if (num9 == -1)
					{
						num201 = Main.maxTilesX - 275;
					}
					float num209 = 1f;
					int num210 = 0;
					int num211 = 0;
					while (!Main.tile[num201, num211].active)
					{
						num211++;
					}
					num197 = num211;
					num211 += WorldGen.genRand.Next(1, 5);
					for (int num212 = num201; num212 < num202; num212++)
					{
						num210++;
						if (num210 < 3)
						{
							num209 += (float)WorldGen.genRand.Next(10, 20) * 0.2f;
						}
						else
						{
							if (num210 < 6)
							{
								num209 += (float)WorldGen.genRand.Next(10, 20) * 0.15f;
							}
							else
							{
								if (num210 < 9)
								{
									num209 += (float)WorldGen.genRand.Next(10, 20) * 0.1f;
								}
								else
								{
									if (num210 < 15)
									{
										num209 += (float)WorldGen.genRand.Next(10, 20) * 0.07f;
									}
									else
									{
										if (num210 < 50)
										{
											num209 += (float)WorldGen.genRand.Next(10, 20) * 0.05f;
										}
										else
										{
											if (num210 < 75)
											{
												num209 += (float)WorldGen.genRand.Next(10, 20) * 0.04f;
											}
											else
											{
												if (num210 < 100)
												{
													num209 += (float)WorldGen.genRand.Next(10, 20) * 0.03f;
												}
												else
												{
													if (num210 < 125)
													{
														num209 += (float)WorldGen.genRand.Next(10, 20) * 0.02f;
													}
													else
													{
														if (num210 < 150)
														{
															num209 += (float)WorldGen.genRand.Next(10, 20) * 0.01f;
														}
														else
														{
															if (num210 < 175)
															{
																num209 += (float)WorldGen.genRand.Next(10, 20) * 0.005f;
															}
															else
															{
																if (num210 < 200)
																{
																	num209 += (float)WorldGen.genRand.Next(10, 20) * 0.001f;
																}
																else
																{
																	if (num210 < 230)
																	{
																		num209 += (float)WorldGen.genRand.Next(10, 20) * 0.01f;
																	}
																	else
																	{
																		if (num210 < 235)
																		{
																			num209 += (float)WorldGen.genRand.Next(10, 20) * 0.05f;
																		}
																		else
																		{
																			if (num210 < 240)
																			{
																				num209 += (float)WorldGen.genRand.Next(10, 20) * 0.1f;
																			}
																			else
																			{
																				if (num210 < 245)
																				{
																					num209 += (float)WorldGen.genRand.Next(10, 20) * 0.05f;
																				}
																				else
																				{
																					if (num210 < 255)
																					{
																						num209 += (float)WorldGen.genRand.Next(10, 20) * 0.01f;
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
						if (num210 == 235)
						{
							num199 = num212;
						}
						int num213 = WorldGen.genRand.Next(15, 20);
						int num214 = 0;
						while ((float)num214 < (float)num211 + num209 + (float)num213)
						{
							if ((float)num214 < (float)num211 + num209 * 0.75f - 3f && (double)num214 < Main.worldSurface - 2.0)
							{
								Main.tile[num212, num214].active = false;
								if (num214 > num211)
								{
									Main.tile[num212, num214].liquid = 255;
								}
								else
								{
									if (num214 == num211)
									{
										Main.tile[num212, num214].liquid = 127;
									}
								}
							}
							else
							{
								if (num214 > num211)
								{
									Main.tile[num212, num214].type = 53;
									Main.tile[num212, num214].active = true;
								}
							}
							Main.tile[num212, num214].wall = 0;
							num214++;
						}
					}
				}
			}
			while (!Main.tile[num198, num196].active)
			{
				num196++;
			}
			num196++;
			while (!Main.tile[num199, num197].active)
			{
				num197++;
			}
			num197++;
			Main.statusText = "Adding gems...";
			for (int num215 = 63; num215 <= 68; num215++)
			{
				float num216 = 0f;
				switch (num215)
				{
				    case 67:
				        num216 = (float)Main.maxTilesX * 0.5f;
				        break;
				    case 66:
				        num216 = (float)Main.maxTilesX * 0.45f;
				        break;
				    case 63:
				        num216 = (float)Main.maxTilesX * 0.3f;
				        break;
				    case 65:
				        num216 = (float)Main.maxTilesX * 0.25f;
				        break;
				    case 64:
				        num216 = (float)Main.maxTilesX * 0.1f;
				        break;
				    case 68:
				        num216 = (float)Main.maxTilesX * 0.05f;
				        break;
				}
				num216 *= 0.2f;
				int num217 = 0;
				while ((float)num217 < num216)
				{
					int num218 = WorldGen.genRand.Next(0, Main.maxTilesX);
					int num219 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
					while (Main.tile[num218, num219].type != 1)
					{
						num218 = WorldGen.genRand.Next(0, Main.maxTilesX);
						num219 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY);
					}
					WorldGen.TileRunner(num218, num219, (double)WorldGen.genRand.Next(2, 6), WorldGen.genRand.Next(3, 7), num215, false, 0f, 0f, false, true);
					num217++;
				}
			}
			for (int num220 = 0; num220 < 2; num220++)
			{
				int num221 = 1;
				int num222 = 5;
				int num223 = Main.maxTilesX - 5;
				if (num220 == 1)
				{
					num221 = -1;
					num222 = Main.maxTilesX - 5;
					num223 = 5;
				}
				for (int num224 = num222; num224 != num223; num224 += num221)
				{
					for (int num225 = 10; num225 < Main.maxTilesY - 10; num225++)
					{
						if (Main.tile[num224, num225].active && Main.tile[num224, num225].type == 53 && Main.tile[num224, num225 + 1].active && Main.tile[num224, num225 + 1].type == 53)
						{
							int num226 = num224 + num221;
							int num227 = num225 + 1;
							if (!Main.tile[num226, num225].active && !Main.tile[num226, num225 + 1].active)
							{
								while (!Main.tile[num226, num227].active)
								{
									num227++;
								}
								num227--;
								Main.tile[num224, num225].active = false;
								Main.tile[num226, num227].active = true;
								Main.tile[num226, num227].type = 53;
							}
						}
					}
				}
			}
			for (int num228 = 0; num228 < Main.maxTilesX; num228++)
			{
				float num229 = (float)num228 / (float)(Main.maxTilesX - 1);
				Main.statusText = "Gravitating sand: " + (int)(num229 * 100f) + "%";
				for (int num230 = Main.maxTilesY - 5; num230 > 0; num230--)
				{
					if (Main.tile[num228, num230].active)
					{
						if (Main.tile[num228, num230].type == 53)
						{
							int num231 = num230;
							while (!Main.tile[num228, num231 + 1].active)
							{
								if (num231 >= Main.maxTilesY - 5)
								{
									break;
								}
								Main.tile[num228, num231 + 1].active = true;
								Main.tile[num228, num231 + 1].type = 53;
								num231++;
							}
						}
						else
						{
							if (Main.tile[num228, num230].type == 123)
							{
								int num232 = num230;
								while (!Main.tile[num228, num232 + 1].active && num232 < Main.maxTilesY - 5)
								{
									Main.tile[num228, num232 + 1].active = true;
									Main.tile[num228, num232 + 1].type = 123;
									Main.tile[num228, num232].active = false;
									num232++;
								}
							}
						}
					}
				}
			}
			for (int num233 = 3; num233 < Main.maxTilesX - 3; num233++)
			{
				float num234 = (float)num233 / (float)Main.maxTilesX;
				Main.statusText = "Cleaning up dirt backgrounds: " + (int)(num234 * 100f + 1f) + "%";
				bool flag11 = true;
				int num235 = 0;
				while ((double)num235 < Main.worldSurface)
				{
					if (flag11)
					{
						if (Main.tile[num233, num235].wall == 2)
						{
							Main.tile[num233, num235].wall = 0;
						}
						if (Main.tile[num233, num235].type != 53)
						{
							if (Main.tile[num233 - 1, num235].wall == 2)
							{
								Main.tile[num233 - 1, num235].wall = 0;
							}
							if (Main.tile[num233 - 2, num235].wall == 2 && WorldGen.genRand.Next(2) == 0)
							{
								Main.tile[num233 - 2, num235].wall = 0;
							}
							if (Main.tile[num233 - 3, num235].wall == 2 && WorldGen.genRand.Next(2) == 0)
							{
								Main.tile[num233 - 3, num235].wall = 0;
							}
							if (Main.tile[num233 + 1, num235].wall == 2)
							{
								Main.tile[num233 + 1, num235].wall = 0;
							}
							if (Main.tile[num233 + 2, num235].wall == 2 && WorldGen.genRand.Next(2) == 0)
							{
								Main.tile[num233 + 2, num235].wall = 0;
							}
							if (Main.tile[num233 + 3, num235].wall == 2 && WorldGen.genRand.Next(2) == 0)
							{
								Main.tile[num233 + 3, num235].wall = 0;
							}
							if (Main.tile[num233, num235].active)
							{
								flag11 = false;
							}
						}
					}
					else
					{
						if (Main.tile[num233, num235].wall == 0 && Main.tile[num233, num235 + 1].wall == 0 && Main.tile[num233, num235 + 2].wall == 0 && Main.tile[num233, num235 + 3].wall == 0 && Main.tile[num233, num235 + 4].wall == 0 && Main.tile[num233 - 1, num235].wall == 0 && Main.tile[num233 + 1, num235].wall == 0 && Main.tile[num233 - 2, num235].wall == 0 && Main.tile[num233 + 2, num235].wall == 0 && !Main.tile[num233, num235].active && !Main.tile[num233, num235 + 1].active && !Main.tile[num233, num235 + 2].active && !Main.tile[num233, num235 + 3].active)
						{
							flag11 = true;
						}
					}
					num235++;
				}
			}
			for (int num236 = 0; num236 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05); num236++)
			{
				float num237 = (float)((double)num236 / ((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05));
				Main.statusText = "Placing altars: " + (int)(num237 * 100f + 1f) + "%";
				bool flag12 = false;
				int num238 = 0;
				while (!flag12)
				{
					int num239 = WorldGen.genRand.Next(1, Main.maxTilesX);
					int num240 = (int)(num6 + 20.0);
					WorldGen.Place3x2(num239, num240, 26);
					if (Main.tile[num239, num240].type == 26)
					{
						flag12 = true;
					}
					else
					{
						num238++;
						if (num238 >= 10000)
						{
							flag12 = true;
						}
					}
				}
			}
			for (int num241 = 0; num241 < Main.maxTilesX; num241++)
			{
				num56 = num241;
				int num242 = (int)num5;
				while ((double)num242 < Main.worldSurface - 1.0)
				{
					if (Main.tile[num56, num242].active)
					{
						if (Main.tile[num56, num242].type == 60)
						{
							Main.tile[num56, num242 - 1].liquid = 255;
							Main.tile[num56, num242 - 2].liquid = 255;
							break;
						}
						break;
					}
					else
					{
						num242++;
					}
				}
			}
			for (int num243 = 400; num243 < Main.maxTilesX - 400; num243++)
			{
				num56 = num243;
				int num244 = (int)num5;
				while ((double)num244 < Main.worldSurface - 1.0)
				{
					if (Main.tile[num56, num244].active)
					{
						if (Main.tile[num56, num244].type == 53)
						{
							int num245 = num244;
							while ((double)num245 > num5)
							{
								num245--;
								Main.tile[num56, num245].liquid = 0;
							}
							break;
						}
						break;
					}
					else
					{
						num244++;
					}
				}
			}
			Liquid.QuickWater(3, -1, -1);
			WorldGen.WaterCheck();
			int num246 = 0;
			Liquid.quickSettle = true;
			while (num246 < 10)
			{
				int num247 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
				num246++;
				float num248 = 0f;
				while (Liquid.numLiquid > 0)
				{
					float num249 = (float)(num247 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer)) / (float)num247;
					if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > num247)
					{
						num247 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
					}
					if (num249 > num248)
					{
						num248 = num249;
					}
					else
					{
						num249 = num248;
					}
					if (num246 == 1)
					{
						Main.statusText = "Settling liquids: " + (int)(num249 * 100f / 3f + 33f) + "%";
					}
					int num250 = 10;
					if (num246 <= num250)
					{
						goto IL_582F;
					}
					IL_582F:
					Liquid.UpdateLiquid();
				}
				WorldGen.WaterCheck();
				Main.statusText = "Settling liquids: " + (int)((float)num246 * 10f / 3f + 66f) + "%";
			}
			Liquid.quickSettle = false;
			float num251 = (float)(Main.maxTilesX / 4200);
			for (int num252 = 0; num252 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05); num252++)
			{
				float num253 = (float)((double)num252 / ((double)(Main.maxTilesX * Main.maxTilesY) * 2E-05));
				Main.statusText = "Placing life crystals: " + (int)(num253 * 100f + 1f) + "%";
				bool flag13 = false;
				int num254 = 0;
				while (!flag13)
				{
					if (WorldGen.AddLifeCrystal(WorldGen.genRand.Next(1, Main.maxTilesX), WorldGen.genRand.Next((int)(num6 + 20.0), Main.maxTilesY)))
					{
						flag13 = true;
					}
					else
					{
						num254++;
						if (num254 >= 10000)
						{
							flag13 = true;
						}
					}
				}
			}
			int num255 = 0;
			int num256 = 0;
			while ((float)num256 < 82f * num251)
			{
				if (num255 > 41)
				{
					num255 = 0;
				}
				float num257 = (float)num256 / (200f * num251);
				Main.statusText = "Placing statues: " + (int)(num257 * 100f + 1f) + "%";
				bool flag14 = false;
				int num258 = 0;
				while (!flag14)
				{
					int num259 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
					int num260 = WorldGen.genRand.Next((int)(num6 + 20.0), Main.maxTilesY - 300);
					while (!Main.tile[num259, num260].active)
					{
						num260++;
					}
					num260--;
					WorldGen.PlaceTile(num259, num260, 105, true, true, -1, num255);
					if (Main.tile[num259, num260].active && Main.tile[num259, num260].type == 105)
					{
						flag14 = true;
						num255++;
					}
					else
					{
						num258++;
						if (num258 >= 10000)
						{
							flag14 = true;
						}
					}
				}
				num256++;
			}
			for (int num261 = 0; num261 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 1.6E-05); num261++)
			{
				float num262 = (float)((double)num261 / ((double)(Main.maxTilesX * Main.maxTilesY) * 1.6E-05));
				Main.statusText = "Hiding treasure: " + (int)(num262 * 100f + 1f) + "%";
				bool flag15 = false;
				int num263 = 0;
				while (!flag15)
				{
					int num264 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
					int num265 = WorldGen.genRand.Next((int)(num6 + 20.0), Main.maxTilesY - 230);
					if ((float)num261 <= 3f * num251)
					{
						num265 = WorldGen.genRand.Next(Main.maxTilesY - 200, Main.maxTilesY - 50);
					}
					while (Main.tile[num264, num265].wall == 7 || Main.tile[num264, num265].wall == 8 || Main.tile[num264, num265].wall == 9)
					{
						num264 = WorldGen.genRand.Next(1, Main.maxTilesX);
						num265 = WorldGen.genRand.Next((int)(num6 + 20.0), Main.maxTilesY - 230);
						if (num261 <= 3)
						{
							num265 = WorldGen.genRand.Next(Main.maxTilesY - 200, Main.maxTilesY - 50);
						}
					}
					if (WorldGen.AddBuriedChest(num264, num265, 0, false, -1))
					{
						flag15 = true;
						if (WorldGen.genRand.Next(2) == 0)
						{
							int num266 = num265;
							while (Main.tile[num264, num266].type != 21 && num266 < Main.maxTilesY - 300)
							{
								num266++;
							}
							if (num265 < Main.maxTilesY - 300)
							{
								WorldGen.MineHouse(num264, num266);
							}
						}
					}
					else
					{
						num263++;
						if (num263 >= 5000)
						{
							flag15 = true;
						}
					}
				}
			}
			for (int num267 = 0; num267 < (int)((double)Main.maxTilesX * 0.005); num267++)
			{
				float num268 = (float)((double)num267 / ((double)Main.maxTilesX * 0.005));
				Main.statusText = "Hiding more treasure: " + (int)(num268 * 100f + 1f) + "%";
				bool flag16 = false;
				int num269 = 0;
				while (!flag16)
				{
					int num270 = WorldGen.genRand.Next(300, Main.maxTilesX - 300);
					int num271 = WorldGen.genRand.Next((int)num5, (int)Main.worldSurface);
					bool flag17 = false;
					if (Main.tile[num270, num271].wall == 2 && !Main.tile[num270, num271].active)
					{
						flag17 = true;
					}
					if (flag17 && WorldGen.AddBuriedChest(num270, num271, 0, true, -1))
					{
						flag16 = true;
					}
					else
					{
						num269++;
						if (num269 >= 2000)
						{
							flag16 = true;
						}
					}
				}
			}
			int num272 = 0;
			for (int num273 = 0; num273 < WorldGen.numJChests; num273++)
			{
				float num274 = (float)(num273 / WorldGen.numJChests);
				Main.statusText = "Hiding jungle treasure: " + (int)(num274 * 100f + 1f) + "%";
				num272++;
				int contain = 211;
				if (num272 == 1)
				{
					contain = 211;
				}
				else
				{
					if (num272 == 2)
					{
						contain = 212;
					}
					else
					{
						if (num272 == 3)
						{
							contain = 213;
						}
					}
				}
				if (num272 > 3)
				{
					num272 = 0;
				}
				if (!WorldGen.AddBuriedChest(WorldGen.JChestX[num273] + WorldGen.genRand.Next(2), WorldGen.JChestY[num273], contain, false, -1))
				{
					for (int num275 = WorldGen.JChestX[num273]; num275 <= WorldGen.JChestX[num273] + 1; num275++)
					{
						for (int num276 = WorldGen.JChestY[num273]; num276 <= WorldGen.JChestY[num273] + 1; num276++)
						{
							WorldGen.KillTile(num275, num276, false, false, false);
						}
					}
					WorldGen.AddBuriedChest(WorldGen.JChestX[num273], WorldGen.JChestY[num273], contain, false, -1);
				}
			}
			int num277 = 0;
			int num278 = 0;
			while ((float)num278 < 9f * num251)
			{
				float num279 = (float)num278 / (9f * num251);
				Main.statusText = "Hiding water treasure: " + (int)(num279 * 100f + 1f) + "%";
				int contain2 = 0;
				num277++;
				if (num277 == 1)
				{
					contain2 = 186;
				}
				else
				{
					if (num277 == 2)
					{
						contain2 = 277;
					}
					else
					{
						contain2 = 187;
						num277 = 0;
					}
				}
				bool flag18 = false;
				while (!flag18)
				{
					int num280 = WorldGen.genRand.Next(1, Main.maxTilesX);
					int num281 = WorldGen.genRand.Next(1, Main.maxTilesY - 200);
					while (Main.tile[num280, num281].liquid < 200 || Main.tile[num280, num281].lava)
					{
						num280 = WorldGen.genRand.Next(1, Main.maxTilesX);
						num281 = WorldGen.genRand.Next(1, Main.maxTilesY - 200);
					}
					flag18 = WorldGen.AddBuriedChest(num280, num281, contain2, false, -1);
				}
				num278++;
			}
			for (int num282 = 0; num282 < WorldGen.numIslandHouses; num282++)
			{
				WorldGen.IslandHouse(WorldGen.fihX[num282], WorldGen.fihY[num282]);
			}
			for (int num283 = 0; num283 < (int)((double)Main.maxTilesX * 0.05); num283++)
			{
				float num284 = (float)((double)num283 / ((double)Main.maxTilesX * 0.05));
				Main.statusText = "Placing traps: " + (int)(num284 * 100f + 1f) + "%";
				for (int num285 = 0; num285 < 1000; num285++)
				{
					int num286 = Main.rand.Next(200, Main.maxTilesX - 200);
					int num287 = Main.rand.Next((int)Main.worldSurface, Main.maxTilesY - 300);
					if (Main.tile[num286, num287].wall == 0 && WorldGen.placeTrap(num286, num287, -1))
					{
						break;
					}
				}
			}
			for (int num288 = 0; num288 < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008); num288++)
			{
				float num289 = (float)((double)num288 / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008));
				Main.statusText = "Placing breakables: " + (int)(num289 * 100f + 1f) + "%";
				bool flag19 = false;
				int num290 = 0;
				while (!flag19)
				{
					int num291 = WorldGen.genRand.Next((int)num6, Main.maxTilesY - 10);
					if ((double)num289 > 0.93)
					{
						num291 = Main.maxTilesY - 150;
					}
					else
					{
						if ((double)num289 > 0.75)
						{
							num291 = (int)num5;
						}
					}
					int num292 = WorldGen.genRand.Next(1, Main.maxTilesX);
					bool flag20 = false;
					for (int num293 = num291; num293 < Main.maxTilesY; num293++)
					{
						if (!flag20)
						{
							if (Main.tile[num292, num293].active && Main.tileSolid[(int)Main.tile[num292, num293].type] && !Main.tile[num292, num293 - 1].lava)
							{
								flag20 = true;
							}
						}
						else
						{
							if (WorldGen.PlacePot(num292, num293, 28))
							{
								flag19 = true;
								break;
							}
							num290++;
							if (num290 >= 10000)
							{
								flag19 = true;
								break;
							}
						}
					}
				}
			}
			for (int num294 = 0; num294 < Main.maxTilesX / 200; num294++)
			{
				float num295 = (float)(num294 / (Main.maxTilesX / 200));
				Main.statusText = "Placing hellforges: " + (int)(num295 * 100f + 1f) + "%";
				bool flag21 = false;
				int num296 = 0;
				while (!flag21)
				{
					int num297 = WorldGen.genRand.Next(1, Main.maxTilesX);
					int num298 = WorldGen.genRand.Next(Main.maxTilesY - 250, Main.maxTilesY - 5);
					try
					{
						if (Main.tile[num297, num298].wall != 13)
						{
							if (Main.tile[num297, num298].wall != 14)
							{
								continue;
							}
						}
						while (!Main.tile[num297, num298].active)
						{
							num298++;
						}
						num298--;
						WorldGen.PlaceTile(num297, num298, 77, false, false, -1, 0);
						if (Main.tile[num297, num298].type == 77)
						{
							flag21 = true;
						}
						else
						{
							num296++;
							if (num296 >= 10000)
							{
								flag21 = true;
							}
						}
					}
					catch
					{
					}
				}
			}
			Main.statusText = "Spreading grass...";
			for (int num299 = 0; num299 < Main.maxTilesX; num299++)
			{
				num56 = num299;
				bool flag22 = true;
				int num300 = 0;
				while ((double)num300 < Main.worldSurface - 1.0)
				{
					if (Main.tile[num56, num300].active)
					{
						if (flag22 && Main.tile[num56, num300].type == 0)
						{
							try
							{
								WorldGen.grassSpread = 0;
								WorldGen.SpreadGrass(num56, num300, 0, 2, true);
							}
							catch
							{
								WorldGen.grassSpread = 0;
								WorldGen.SpreadGrass(num56, num300, 0, 2, false);
							}
						}
						if ((double)num300 > num6)
						{
							break;
						}
						flag22 = false;
					}
					else
					{
						if (Main.tile[num56, num300].wall == 0)
						{
							flag22 = true;
						}
					}
					num300++;
				}
			}
			Main.statusText = "Growing cacti...";
			for (int num301 = 5; num301 < Main.maxTilesX - 5; num301++)
			{
				if (WorldGen.genRand.Next(8) == 0)
				{
					int num302 = 0;
					while ((double)num302 < Main.worldSurface - 1.0)
					{
						if (Main.tile[num301, num302].active && Main.tile[num301, num302].type == 53 && !Main.tile[num301, num302 - 1].active && Main.tile[num301, num302 - 1].wall == 0)
						{
							if (num301 < 250 || num301 > Main.maxTilesX - 250)
							{
								if (Main.tile[num301, num302 - 2].liquid == 255 && Main.tile[num301, num302 - 3].liquid == 255 && Main.tile[num301, num302 - 4].liquid == 255)
								{
									WorldGen.PlaceTile(num301, num302 - 1, 81, true, false, -1, 0);
								}
							}
							else
							{
								if (num301 > 400 && num301 < Main.maxTilesX - 400)
								{
									WorldGen.PlantCactus(num301, num302);
								}
							}
						}
						num302++;
					}
				}
			}
			int num303 = 5;
			bool flag23 = true;
			while (flag23)
			{
				int num304 = Main.maxTilesX / 2 + WorldGen.genRand.Next(-num303, num303 + 1);
				for (int num305 = 0; num305 < Main.maxTilesY; num305++)
				{
					if (Main.tile[num304, num305].active)
					{
						Main.spawnTileX = num304;
						Main.spawnTileY = num305;
						break;
					}
				}
				flag23 = false;
				num303++;
				if ((double)Main.spawnTileY > Main.worldSurface)
				{
					flag23 = true;
				}
				if (Main.tile[Main.spawnTileX, Main.spawnTileY - 1].liquid > 0)
				{
					flag23 = true;
				}
			}
			int num306 = 10;
			while ((double)Main.spawnTileY > Main.worldSurface)
			{
				int num307 = WorldGen.genRand.Next(Main.maxTilesX / 2 - num306, Main.maxTilesX / 2 + num306);
				for (int num308 = 0; num308 < Main.maxTilesY; num308++)
				{
					if (Main.tile[num307, num308].active)
					{
						Main.spawnTileX = num307;
						Main.spawnTileY = num308;
						break;
					}
				}
				num306++;
			}
			int num309 = NPC.NewNPC(Main.spawnTileX * 16, Main.spawnTileY * 16, 22, 0);
			Main.npc[num309].homeTileX = Main.spawnTileX;
			Main.npc[num309].homeTileY = Main.spawnTileY;
			Main.npc[num309].direction = 1;
			Main.npc[num309].homeless = true;
			Main.statusText = "Planting sunflowers...";
			int num310 = 0;
			while ((double)num310 < (double)Main.maxTilesX * 0.002)
			{
				int num311 = 0;
				int num312 = 0;
				int arg_6A1D_0 = Main.maxTilesX / 2;
				int num313 = WorldGen.genRand.Next(Main.maxTilesX);
				num311 = num313 - WorldGen.genRand.Next(10) - 7;
				num312 = num313 + WorldGen.genRand.Next(10) + 7;
				if (num311 < 0)
				{
					num311 = 0;
				}
				if (num312 > Main.maxTilesX - 1)
				{
					num312 = Main.maxTilesX - 1;
				}
				for (int num314 = num311; num314 < num312; num314++)
				{
					int num315 = 1;
					while ((double)num315 < Main.worldSurface - 1.0)
					{
						if (Main.tile[num314, num315].type == 2 && Main.tile[num314, num315].active && !Main.tile[num314, num315 - 1].active)
						{
							WorldGen.PlaceTile(num314, num315 - 1, 27, true, false, -1, 0);
						}
						if (Main.tile[num314, num315].active)
						{
							break;
						}
						num315++;
					}
				}
				num310++;
			}
			Main.statusText = "Planting trees...";
			int num316 = 0;
			while ((double)num316 < (double)Main.maxTilesX * 0.003)
			{
				int num317 = WorldGen.genRand.Next(50, Main.maxTilesX - 50);
				int num318 = WorldGen.genRand.Next(25, 50);
				for (int num319 = num317 - num318; num319 < num317 + num318; num319++)
				{
					int num320 = 20;
					while ((double)num320 < Main.worldSurface)
					{
						WorldGen.GrowEpicTree(num319, num320);
						num320++;
					}
				}
				num316++;
			}
			WorldGen.AddTrees();
			Main.statusText = "Planting herbs...";
			int num321 = 0;
			while ((double)num321 < (double)Main.maxTilesX * 1.7)
			{
				WorldGen.PlantAlch();
				num321++;
			}
			Main.statusText = "Planting weeds...";
			WorldGen.AddPlants();
			for (int num322 = 0; num322 < Main.maxTilesX; num322++)
			{
				for (int num323 = 0; num323 < Main.maxTilesY; num323++)
				{
					if (Main.tile[num322, num323].active)
					{
						if (num323 >= (int)Main.worldSurface && Main.tile[num322, num323].type == 70 && !Main.tile[num322, num323 - 1].active)
						{
							WorldGen.GrowShroom(num322, num323);
							if (!Main.tile[num322, num323 - 1].active)
							{
								WorldGen.PlaceTile(num322, num323 - 1, 71, true, false, -1, 0);
							}
						}
						if (Main.tile[num322, num323].type == 60 && !Main.tile[num322, num323 - 1].active)
						{
							WorldGen.PlaceTile(num322, num323 - 1, 61, true, false, -1, 0);
						}
					}
				}
			}
			Main.statusText = "Growing vines...";
			for (int num324 = 0; num324 < Main.maxTilesX; num324++)
			{
				int num325 = 0;
				int num326 = 0;
				while ((double)num326 < Main.worldSurface)
				{
					if (num325 > 0 && !Main.tile[num324, num326].active)
					{
						Main.tile[num324, num326].active = true;
						Main.tile[num324, num326].type = 52;
						num325--;
					}
					else
					{
						num325 = 0;
					}
					if (Main.tile[num324, num326].active && Main.tile[num324, num326].type == 2 && WorldGen.genRand.Next(5) < 3)
					{
						num325 = WorldGen.genRand.Next(1, 10);
					}
					num326++;
				}
				num325 = 0;
				for (int num327 = 0; num327 < Main.maxTilesY; num327++)
				{
					if (num325 > 0 && !Main.tile[num324, num327].active)
					{
						Main.tile[num324, num327].active = true;
						Main.tile[num324, num327].type = 62;
						num325--;
					}
					else
					{
						num325 = 0;
					}
					if (Main.tile[num324, num327].active && Main.tile[num324, num327].type == 60 && WorldGen.genRand.Next(5) < 3)
					{
						num325 = WorldGen.genRand.Next(1, 10);
					}
				}
			}
			Main.statusText = "Planting flowers...";
			int num328 = 0;
			while ((double)num328 < (double)Main.maxTilesX * 0.005)
			{
				int num329 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
				int num330 = WorldGen.genRand.Next(5, 15);
				int num331 = WorldGen.genRand.Next(15, 30);
				int num332 = 1;
				while ((double)num332 < Main.worldSurface - 1.0)
				{
					if (Main.tile[num329, num332].active)
					{
						for (int num333 = num329 - num330; num333 < num329 + num330; num333++)
						{
							for (int num334 = num332 - num331; num334 < num332 + num331; num334++)
							{
								if (Main.tile[num333, num334].type == 3 || Main.tile[num333, num334].type == 24)
								{
									Main.tile[num333, num334].frameX = (short)(WorldGen.genRand.Next(6, 8) * 18);
								}
							}
						}
						break;
					}
					num332++;
				}
				num328++;
			}
			Main.statusText = "Planting mushrooms...";
			int num335 = 0;
			while ((double)num335 < (double)Main.maxTilesX * 0.002)
			{
				int num336 = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
				int num337 = WorldGen.genRand.Next(4, 10);
				int num338 = WorldGen.genRand.Next(15, 30);
				int num339 = 1;
				while ((double)num339 < Main.worldSurface - 1.0)
				{
					if (Main.tile[num336, num339].active)
					{
						for (int num340 = num336 - num337; num340 < num336 + num337; num340++)
						{
							for (int num341 = num339 - num338; num341 < num339 + num338; num341++)
							{
								if (Main.tile[num340, num341].type == 3 || Main.tile[num340, num341].type == 24)
								{
									Main.tile[num340, num341].frameX = 144;
								}
							}
						}
						break;
					}
					num339++;
				}
				num335++;
			}
			WorldGen.gen = false;
		}
		public static bool GrowEpicTree(int i, int y)
		{
			int num = y;
			while (Main.tile[i, num].type == 20)
			{
				num++;
			}
			if (Main.tile[i, num].active && Main.tile[i, num].type == 2 && Main.tile[i, num - 1].wall == 0 && Main.tile[i, num - 1].liquid == 0 && ((Main.tile[i - 1, num].active && (Main.tile[i - 1, num].type == 2 || Main.tile[i - 1, num].type == 23 || Main.tile[i - 1, num].type == 60 || Main.tile[i - 1, num].type == 109)) || (Main.tile[i + 1, num].active && (Main.tile[i + 1, num].type == 2 || Main.tile[i + 1, num].type == 23 || Main.tile[i + 1, num].type == 60 || Main.tile[i + 1, num].type == 109))))
			{
				int num2 = 1;
				if (WorldGen.EmptyTileCheck(i - num2, i + num2, num - 55, num - 1, 20))
				{
					bool flag = false;
					bool flag2 = false;
					int num3 = WorldGen.genRand.Next(20, 30);
					int num4;
					for (int j = num - num3; j < num; j++)
					{
						Main.tile[i, j].frameNumber = (byte)WorldGen.genRand.Next(3);
						Main.tile[i, j].active = true;
						Main.tile[i, j].type = 5;
						num4 = WorldGen.genRand.Next(3);
						int num5 = WorldGen.genRand.Next(10);
						if (j == num - 1 || j == num - num3)
						{
							num5 = 0;
						}
						while (((num5 == 5 || num5 == 7) && flag) || ((num5 == 6 || num5 == 7) && flag2))
						{
							num5 = WorldGen.genRand.Next(10);
						}
						flag = false;
						flag2 = false;
						if (num5 == 5 || num5 == 7)
						{
							flag = true;
						}
						if (num5 == 6 || num5 == 7)
						{
							flag2 = true;
						}
						if (num5 == 1)
						{
							if (num4 == 0)
							{
								Main.tile[i, j].frameX = 0;
								Main.tile[i, j].frameY = 66;
							}
							if (num4 == 1)
							{
								Main.tile[i, j].frameX = 0;
								Main.tile[i, j].frameY = 88;
							}
							if (num4 == 2)
							{
								Main.tile[i, j].frameX = 0;
								Main.tile[i, j].frameY = 110;
							}
						}
						else
						{
							if (num5 == 2)
							{
								if (num4 == 0)
								{
									Main.tile[i, j].frameX = 22;
									Main.tile[i, j].frameY = 0;
								}
								if (num4 == 1)
								{
									Main.tile[i, j].frameX = 22;
									Main.tile[i, j].frameY = 22;
								}
								if (num4 == 2)
								{
									Main.tile[i, j].frameX = 22;
									Main.tile[i, j].frameY = 44;
								}
							}
							else
							{
								if (num5 == 3)
								{
									if (num4 == 0)
									{
										Main.tile[i, j].frameX = 44;
										Main.tile[i, j].frameY = 66;
									}
									if (num4 == 1)
									{
										Main.tile[i, j].frameX = 44;
										Main.tile[i, j].frameY = 88;
									}
									if (num4 == 2)
									{
										Main.tile[i, j].frameX = 44;
										Main.tile[i, j].frameY = 110;
									}
								}
								else
								{
									if (num5 == 4)
									{
										if (num4 == 0)
										{
											Main.tile[i, j].frameX = 22;
											Main.tile[i, j].frameY = 66;
										}
										if (num4 == 1)
										{
											Main.tile[i, j].frameX = 22;
											Main.tile[i, j].frameY = 88;
										}
										if (num4 == 2)
										{
											Main.tile[i, j].frameX = 22;
											Main.tile[i, j].frameY = 110;
										}
									}
									else
									{
										if (num5 == 5)
										{
											if (num4 == 0)
											{
												Main.tile[i, j].frameX = 88;
												Main.tile[i, j].frameY = 0;
											}
											if (num4 == 1)
											{
												Main.tile[i, j].frameX = 88;
												Main.tile[i, j].frameY = 22;
											}
											if (num4 == 2)
											{
												Main.tile[i, j].frameX = 88;
												Main.tile[i, j].frameY = 44;
											}
										}
										else
										{
											if (num5 == 6)
											{
												if (num4 == 0)
												{
													Main.tile[i, j].frameX = 66;
													Main.tile[i, j].frameY = 66;
												}
												if (num4 == 1)
												{
													Main.tile[i, j].frameX = 66;
													Main.tile[i, j].frameY = 88;
												}
												if (num4 == 2)
												{
													Main.tile[i, j].frameX = 66;
													Main.tile[i, j].frameY = 110;
												}
											}
											else
											{
												if (num5 == 7)
												{
													if (num4 == 0)
													{
														Main.tile[i, j].frameX = 110;
														Main.tile[i, j].frameY = 66;
													}
													if (num4 == 1)
													{
														Main.tile[i, j].frameX = 110;
														Main.tile[i, j].frameY = 88;
													}
													if (num4 == 2)
													{
														Main.tile[i, j].frameX = 110;
														Main.tile[i, j].frameY = 110;
													}
												}
												else
												{
													if (num4 == 0)
													{
														Main.tile[i, j].frameX = 0;
														Main.tile[i, j].frameY = 0;
													}
													if (num4 == 1)
													{
														Main.tile[i, j].frameX = 0;
														Main.tile[i, j].frameY = 22;
													}
													if (num4 == 2)
													{
														Main.tile[i, j].frameX = 0;
														Main.tile[i, j].frameY = 44;
													}
												}
											}
										}
									}
								}
							}
						}
						if (num5 == 5 || num5 == 7)
						{
							Main.tile[i - 1, j].active = true;
							Main.tile[i - 1, j].type = 5;
							num4 = WorldGen.genRand.Next(3);
							if (WorldGen.genRand.Next(3) < 2)
							{
								if (num4 == 0)
								{
									Main.tile[i - 1, j].frameX = 44;
									Main.tile[i - 1, j].frameY = 198;
								}
								if (num4 == 1)
								{
									Main.tile[i - 1, j].frameX = 44;
									Main.tile[i - 1, j].frameY = 220;
								}
								if (num4 == 2)
								{
									Main.tile[i - 1, j].frameX = 44;
									Main.tile[i - 1, j].frameY = 242;
								}
							}
							else
							{
								if (num4 == 0)
								{
									Main.tile[i - 1, j].frameX = 66;
									Main.tile[i - 1, j].frameY = 0;
								}
								if (num4 == 1)
								{
									Main.tile[i - 1, j].frameX = 66;
									Main.tile[i - 1, j].frameY = 22;
								}
								if (num4 == 2)
								{
									Main.tile[i - 1, j].frameX = 66;
									Main.tile[i - 1, j].frameY = 44;
								}
							}
						}
						if (num5 == 6 || num5 == 7)
						{
							Main.tile[i + 1, j].active = true;
							Main.tile[i + 1, j].type = 5;
							num4 = WorldGen.genRand.Next(3);
							if (WorldGen.genRand.Next(3) < 2)
							{
								if (num4 == 0)
								{
									Main.tile[i + 1, j].frameX = 66;
									Main.tile[i + 1, j].frameY = 198;
								}
								if (num4 == 1)
								{
									Main.tile[i + 1, j].frameX = 66;
									Main.tile[i + 1, j].frameY = 220;
								}
								if (num4 == 2)
								{
									Main.tile[i + 1, j].frameX = 66;
									Main.tile[i + 1, j].frameY = 242;
								}
							}
							else
							{
								if (num4 == 0)
								{
									Main.tile[i + 1, j].frameX = 88;
									Main.tile[i + 1, j].frameY = 66;
								}
								if (num4 == 1)
								{
									Main.tile[i + 1, j].frameX = 88;
									Main.tile[i + 1, j].frameY = 88;
								}
								if (num4 == 2)
								{
									Main.tile[i + 1, j].frameX = 88;
									Main.tile[i + 1, j].frameY = 110;
								}
							}
						}
					}
					int num6 = WorldGen.genRand.Next(3);
					bool flag3 = false;
					bool flag4 = false;
					if (Main.tile[i - 1, num].active && (Main.tile[i - 1, num].type == 2 || Main.tile[i - 1, num].type == 23 || Main.tile[i - 1, num].type == 60 || Main.tile[i - 1, num].type == 109))
					{
						flag3 = true;
					}
					if (Main.tile[i + 1, num].active && (Main.tile[i + 1, num].type == 2 || Main.tile[i + 1, num].type == 23 || Main.tile[i + 1, num].type == 60 || Main.tile[i + 1, num].type == 109))
					{
						flag4 = true;
					}
					if (!flag3)
					{
						if (num6 == 0)
						{
							num6 = 2;
						}
						if (num6 == 1)
						{
							num6 = 3;
						}
					}
					if (!flag4)
					{
						if (num6 == 0)
						{
							num6 = 1;
						}
						if (num6 == 2)
						{
							num6 = 3;
						}
					}
					if (flag3 && !flag4)
					{
						num6 = 1;
					}
					if (flag4 && !flag3)
					{
						num6 = 2;
					}
					if (num6 == 0 || num6 == 1)
					{
						Main.tile[i + 1, num - 1].active = true;
						Main.tile[i + 1, num - 1].type = 5;
						num4 = WorldGen.genRand.Next(3);
						if (num4 == 0)
						{
							Main.tile[i + 1, num - 1].frameX = 22;
							Main.tile[i + 1, num - 1].frameY = 132;
						}
						if (num4 == 1)
						{
							Main.tile[i + 1, num - 1].frameX = 22;
							Main.tile[i + 1, num - 1].frameY = 154;
						}
						if (num4 == 2)
						{
							Main.tile[i + 1, num - 1].frameX = 22;
							Main.tile[i + 1, num - 1].frameY = 176;
						}
					}
					if (num6 == 0 || num6 == 2)
					{
						Main.tile[i - 1, num - 1].active = true;
						Main.tile[i - 1, num - 1].type = 5;
						num4 = WorldGen.genRand.Next(3);
						if (num4 == 0)
						{
							Main.tile[i - 1, num - 1].frameX = 44;
							Main.tile[i - 1, num - 1].frameY = 132;
						}
						if (num4 == 1)
						{
							Main.tile[i - 1, num - 1].frameX = 44;
							Main.tile[i - 1, num - 1].frameY = 154;
						}
						if (num4 == 2)
						{
							Main.tile[i - 1, num - 1].frameX = 44;
							Main.tile[i - 1, num - 1].frameY = 176;
						}
					}
					num4 = WorldGen.genRand.Next(3);
					if (num6 == 0)
					{
						if (num4 == 0)
						{
							Main.tile[i, num - 1].frameX = 88;
							Main.tile[i, num - 1].frameY = 132;
						}
						if (num4 == 1)
						{
							Main.tile[i, num - 1].frameX = 88;
							Main.tile[i, num - 1].frameY = 154;
						}
						if (num4 == 2)
						{
							Main.tile[i, num - 1].frameX = 88;
							Main.tile[i, num - 1].frameY = 176;
						}
					}
					else
					{
						if (num6 == 1)
						{
							if (num4 == 0)
							{
								Main.tile[i, num - 1].frameX = 0;
								Main.tile[i, num - 1].frameY = 132;
							}
							if (num4 == 1)
							{
								Main.tile[i, num - 1].frameX = 0;
								Main.tile[i, num - 1].frameY = 154;
							}
							if (num4 == 2)
							{
								Main.tile[i, num - 1].frameX = 0;
								Main.tile[i, num - 1].frameY = 176;
							}
						}
						else
						{
							if (num6 == 2)
							{
								if (num4 == 0)
								{
									Main.tile[i, num - 1].frameX = 66;
									Main.tile[i, num - 1].frameY = 132;
								}
								if (num4 == 1)
								{
									Main.tile[i, num - 1].frameX = 66;
									Main.tile[i, num - 1].frameY = 154;
								}
								if (num4 == 2)
								{
									Main.tile[i, num - 1].frameX = 66;
									Main.tile[i, num - 1].frameY = 176;
								}
							}
						}
					}
					if (WorldGen.genRand.Next(3) < 2)
					{
						num4 = WorldGen.genRand.Next(3);
						if (num4 == 0)
						{
							Main.tile[i, num - num3].frameX = 22;
							Main.tile[i, num - num3].frameY = 198;
						}
						if (num4 == 1)
						{
							Main.tile[i, num - num3].frameX = 22;
							Main.tile[i, num - num3].frameY = 220;
						}
						if (num4 == 2)
						{
							Main.tile[i, num - num3].frameX = 22;
							Main.tile[i, num - num3].frameY = 242;
						}
					}
					else
					{
						num4 = WorldGen.genRand.Next(3);
						if (num4 == 0)
						{
							Main.tile[i, num - num3].frameX = 0;
							Main.tile[i, num - num3].frameY = 198;
						}
						if (num4 == 1)
						{
							Main.tile[i, num - num3].frameX = 0;
							Main.tile[i, num - num3].frameY = 220;
						}
						if (num4 == 2)
						{
							Main.tile[i, num - num3].frameX = 0;
							Main.tile[i, num - num3].frameY = 242;
						}
					}
					WorldGen.RangeFrame(i - 2, num - num3 - 1, i + 2, num + 1);
					if (Main.netMode == 2)
					{
						NetMessage.SendTileSquare(-1, i, (int)((double)num - (double)num3 * 0.5), num3 + 1);
					}
					return true;
				}
			}
			return false;
		}
		public static void GrowTree(int i, int y)
		{
			int num = y;
			while (Main.tile[i, num].type == 20)
			{
				num++;
			}
			if ((Main.tile[i - 1, num - 1].liquid != 0 || Main.tile[i - 1, num - 1].liquid != 0 || Main.tile[i + 1, num - 1].liquid != 0) && Main.tile[i, num].type != 60)
			{
				return;
			}
			if (Main.tile[i, num].active && (Main.tile[i, num].type == 2 || Main.tile[i, num].type == 23 || Main.tile[i, num].type == 60 || Main.tile[i, num].type == 109 || Main.tile[i, num].type == 147) && Main.tile[i, num - 1].wall == 0 && ((Main.tile[i - 1, num].active && (Main.tile[i - 1, num].type == 2 || Main.tile[i - 1, num].type == 23 || Main.tile[i - 1, num].type == 60 || Main.tile[i - 1, num].type == 109 || Main.tile[i - 1, num].type == 147)) || (Main.tile[i + 1, num].active && (Main.tile[i + 1, num].type == 2 || Main.tile[i + 1, num].type == 23 || Main.tile[i + 1, num].type == 60 || Main.tile[i + 1, num].type == 109 || Main.tile[i + 1, num].type == 147))))
			{
				int num2 = 1;
				int num3 = 16;
				if (Main.tile[i, num].type == 60)
				{
					num3 += 5;
				}
				if (WorldGen.EmptyTileCheck(i - num2, i + num2, num - num3, num - 1, 20))
				{
					bool flag = false;
					bool flag2 = false;
					int num4 = WorldGen.genRand.Next(5, num3 + 1);
					int num5;
					for (int j = num - num4; j < num; j++)
					{
						Main.tile[i, j].frameNumber = (byte)WorldGen.genRand.Next(3);
						Main.tile[i, j].active = true;
						Main.tile[i, j].type = 5;
						num5 = WorldGen.genRand.Next(3);
						int num6 = WorldGen.genRand.Next(10);
						if (j == num - 1 || j == num - num4)
						{
							num6 = 0;
						}
						while (((num6 == 5 || num6 == 7) && flag) || ((num6 == 6 || num6 == 7) && flag2))
						{
							num6 = WorldGen.genRand.Next(10);
						}
						flag = false;
						flag2 = false;
						if (num6 == 5 || num6 == 7)
						{
							flag = true;
						}
						if (num6 == 6 || num6 == 7)
						{
							flag2 = true;
						}
						if (num6 == 1)
						{
							if (num5 == 0)
							{
								Main.tile[i, j].frameX = 0;
								Main.tile[i, j].frameY = 66;
							}
							if (num5 == 1)
							{
								Main.tile[i, j].frameX = 0;
								Main.tile[i, j].frameY = 88;
							}
							if (num5 == 2)
							{
								Main.tile[i, j].frameX = 0;
								Main.tile[i, j].frameY = 110;
							}
						}
						else
						{
							if (num6 == 2)
							{
								if (num5 == 0)
								{
									Main.tile[i, j].frameX = 22;
									Main.tile[i, j].frameY = 0;
								}
								if (num5 == 1)
								{
									Main.tile[i, j].frameX = 22;
									Main.tile[i, j].frameY = 22;
								}
								if (num5 == 2)
								{
									Main.tile[i, j].frameX = 22;
									Main.tile[i, j].frameY = 44;
								}
							}
							else
							{
								if (num6 == 3)
								{
									if (num5 == 0)
									{
										Main.tile[i, j].frameX = 44;
										Main.tile[i, j].frameY = 66;
									}
									if (num5 == 1)
									{
										Main.tile[i, j].frameX = 44;
										Main.tile[i, j].frameY = 88;
									}
									if (num5 == 2)
									{
										Main.tile[i, j].frameX = 44;
										Main.tile[i, j].frameY = 110;
									}
								}
								else
								{
									if (num6 == 4)
									{
										if (num5 == 0)
										{
											Main.tile[i, j].frameX = 22;
											Main.tile[i, j].frameY = 66;
										}
										if (num5 == 1)
										{
											Main.tile[i, j].frameX = 22;
											Main.tile[i, j].frameY = 88;
										}
										if (num5 == 2)
										{
											Main.tile[i, j].frameX = 22;
											Main.tile[i, j].frameY = 110;
										}
									}
									else
									{
										if (num6 == 5)
										{
											if (num5 == 0)
											{
												Main.tile[i, j].frameX = 88;
												Main.tile[i, j].frameY = 0;
											}
											if (num5 == 1)
											{
												Main.tile[i, j].frameX = 88;
												Main.tile[i, j].frameY = 22;
											}
											if (num5 == 2)
											{
												Main.tile[i, j].frameX = 88;
												Main.tile[i, j].frameY = 44;
											}
										}
										else
										{
											if (num6 == 6)
											{
												if (num5 == 0)
												{
													Main.tile[i, j].frameX = 66;
													Main.tile[i, j].frameY = 66;
												}
												if (num5 == 1)
												{
													Main.tile[i, j].frameX = 66;
													Main.tile[i, j].frameY = 88;
												}
												if (num5 == 2)
												{
													Main.tile[i, j].frameX = 66;
													Main.tile[i, j].frameY = 110;
												}
											}
											else
											{
												if (num6 == 7)
												{
													if (num5 == 0)
													{
														Main.tile[i, j].frameX = 110;
														Main.tile[i, j].frameY = 66;
													}
													if (num5 == 1)
													{
														Main.tile[i, j].frameX = 110;
														Main.tile[i, j].frameY = 88;
													}
													if (num5 == 2)
													{
														Main.tile[i, j].frameX = 110;
														Main.tile[i, j].frameY = 110;
													}
												}
												else
												{
													if (num5 == 0)
													{
														Main.tile[i, j].frameX = 0;
														Main.tile[i, j].frameY = 0;
													}
													if (num5 == 1)
													{
														Main.tile[i, j].frameX = 0;
														Main.tile[i, j].frameY = 22;
													}
													if (num5 == 2)
													{
														Main.tile[i, j].frameX = 0;
														Main.tile[i, j].frameY = 44;
													}
												}
											}
										}
									}
								}
							}
						}
						if (num6 == 5 || num6 == 7)
						{
							Main.tile[i - 1, j].active = true;
							Main.tile[i - 1, j].type = 5;
							num5 = WorldGen.genRand.Next(3);
							if (WorldGen.genRand.Next(3) < 2)
							{
								if (num5 == 0)
								{
									Main.tile[i - 1, j].frameX = 44;
									Main.tile[i - 1, j].frameY = 198;
								}
								if (num5 == 1)
								{
									Main.tile[i - 1, j].frameX = 44;
									Main.tile[i - 1, j].frameY = 220;
								}
								if (num5 == 2)
								{
									Main.tile[i - 1, j].frameX = 44;
									Main.tile[i - 1, j].frameY = 242;
								}
							}
							else
							{
								if (num5 == 0)
								{
									Main.tile[i - 1, j].frameX = 66;
									Main.tile[i - 1, j].frameY = 0;
								}
								if (num5 == 1)
								{
									Main.tile[i - 1, j].frameX = 66;
									Main.tile[i - 1, j].frameY = 22;
								}
								if (num5 == 2)
								{
									Main.tile[i - 1, j].frameX = 66;
									Main.tile[i - 1, j].frameY = 44;
								}
							}
						}
						if (num6 == 6 || num6 == 7)
						{
							Main.tile[i + 1, j].active = true;
							Main.tile[i + 1, j].type = 5;
							num5 = WorldGen.genRand.Next(3);
							if (WorldGen.genRand.Next(3) < 2)
							{
								if (num5 == 0)
								{
									Main.tile[i + 1, j].frameX = 66;
									Main.tile[i + 1, j].frameY = 198;
								}
								if (num5 == 1)
								{
									Main.tile[i + 1, j].frameX = 66;
									Main.tile[i + 1, j].frameY = 220;
								}
								if (num5 == 2)
								{
									Main.tile[i + 1, j].frameX = 66;
									Main.tile[i + 1, j].frameY = 242;
								}
							}
							else
							{
								if (num5 == 0)
								{
									Main.tile[i + 1, j].frameX = 88;
									Main.tile[i + 1, j].frameY = 66;
								}
								if (num5 == 1)
								{
									Main.tile[i + 1, j].frameX = 88;
									Main.tile[i + 1, j].frameY = 88;
								}
								if (num5 == 2)
								{
									Main.tile[i + 1, j].frameX = 88;
									Main.tile[i + 1, j].frameY = 110;
								}
							}
						}
					}
					int num7 = WorldGen.genRand.Next(3);
					bool flag3 = false;
					bool flag4 = false;
					if (Main.tile[i - 1, num].active && (Main.tile[i - 1, num].type == 2 || Main.tile[i - 1, num].type == 23 || Main.tile[i - 1, num].type == 60 || Main.tile[i - 1, num].type == 109 || Main.tile[i - 1, num].type == 147))
					{
						flag3 = true;
					}
					if (Main.tile[i + 1, num].active && (Main.tile[i + 1, num].type == 2 || Main.tile[i + 1, num].type == 23 || Main.tile[i + 1, num].type == 60 || Main.tile[i + 1, num].type == 109 || Main.tile[i + 1, num].type == 147))
					{
						flag4 = true;
					}
					if (!flag3)
					{
						if (num7 == 0)
						{
							num7 = 2;
						}
						if (num7 == 1)
						{
							num7 = 3;
						}
					}
					if (!flag4)
					{
						if (num7 == 0)
						{
							num7 = 1;
						}
						if (num7 == 2)
						{
							num7 = 3;
						}
					}
					if (flag3 && !flag4)
					{
						num7 = 1;
					}
					if (flag4 && !flag3)
					{
						num7 = 2;
					}
					if (num7 == 0 || num7 == 1)
					{
						Main.tile[i + 1, num - 1].active = true;
						Main.tile[i + 1, num - 1].type = 5;
						num5 = WorldGen.genRand.Next(3);
						if (num5 == 0)
						{
							Main.tile[i + 1, num - 1].frameX = 22;
							Main.tile[i + 1, num - 1].frameY = 132;
						}
						if (num5 == 1)
						{
							Main.tile[i + 1, num - 1].frameX = 22;
							Main.tile[i + 1, num - 1].frameY = 154;
						}
						if (num5 == 2)
						{
							Main.tile[i + 1, num - 1].frameX = 22;
							Main.tile[i + 1, num - 1].frameY = 176;
						}
					}
					if (num7 == 0 || num7 == 2)
					{
						Main.tile[i - 1, num - 1].active = true;
						Main.tile[i - 1, num - 1].type = 5;
						num5 = WorldGen.genRand.Next(3);
						if (num5 == 0)
						{
							Main.tile[i - 1, num - 1].frameX = 44;
							Main.tile[i - 1, num - 1].frameY = 132;
						}
						if (num5 == 1)
						{
							Main.tile[i - 1, num - 1].frameX = 44;
							Main.tile[i - 1, num - 1].frameY = 154;
						}
						if (num5 == 2)
						{
							Main.tile[i - 1, num - 1].frameX = 44;
							Main.tile[i - 1, num - 1].frameY = 176;
						}
					}
					num5 = WorldGen.genRand.Next(3);
					if (num7 == 0)
					{
						if (num5 == 0)
						{
							Main.tile[i, num - 1].frameX = 88;
							Main.tile[i, num - 1].frameY = 132;
						}
						if (num5 == 1)
						{
							Main.tile[i, num - 1].frameX = 88;
							Main.tile[i, num - 1].frameY = 154;
						}
						if (num5 == 2)
						{
							Main.tile[i, num - 1].frameX = 88;
							Main.tile[i, num - 1].frameY = 176;
						}
					}
					else
					{
						if (num7 == 1)
						{
							if (num5 == 0)
							{
								Main.tile[i, num - 1].frameX = 0;
								Main.tile[i, num - 1].frameY = 132;
							}
							if (num5 == 1)
							{
								Main.tile[i, num - 1].frameX = 0;
								Main.tile[i, num - 1].frameY = 154;
							}
							if (num5 == 2)
							{
								Main.tile[i, num - 1].frameX = 0;
								Main.tile[i, num - 1].frameY = 176;
							}
						}
						else
						{
							if (num7 == 2)
							{
								if (num5 == 0)
								{
									Main.tile[i, num - 1].frameX = 66;
									Main.tile[i, num - 1].frameY = 132;
								}
								if (num5 == 1)
								{
									Main.tile[i, num - 1].frameX = 66;
									Main.tile[i, num - 1].frameY = 154;
								}
								if (num5 == 2)
								{
									Main.tile[i, num - 1].frameX = 66;
									Main.tile[i, num - 1].frameY = 176;
								}
							}
						}
					}
					if (WorldGen.genRand.Next(4) < 3)
					{
						num5 = WorldGen.genRand.Next(3);
						if (num5 == 0)
						{
							Main.tile[i, num - num4].frameX = 22;
							Main.tile[i, num - num4].frameY = 198;
						}
						if (num5 == 1)
						{
							Main.tile[i, num - num4].frameX = 22;
							Main.tile[i, num - num4].frameY = 220;
						}
						if (num5 == 2)
						{
							Main.tile[i, num - num4].frameX = 22;
							Main.tile[i, num - num4].frameY = 242;
						}
					}
					else
					{
						num5 = WorldGen.genRand.Next(3);
						if (num5 == 0)
						{
							Main.tile[i, num - num4].frameX = 0;
							Main.tile[i, num - num4].frameY = 198;
						}
						if (num5 == 1)
						{
							Main.tile[i, num - num4].frameX = 0;
							Main.tile[i, num - num4].frameY = 220;
						}
						if (num5 == 2)
						{
							Main.tile[i, num - num4].frameX = 0;
							Main.tile[i, num - num4].frameY = 242;
						}
					}
					WorldGen.RangeFrame(i - 2, num - num4 - 1, i + 2, num + 1);
					if (Main.netMode == 2)
					{
						NetMessage.SendTileSquare(-1, i, (int)((double)num - (double)num4 * 0.5), num4 + 1);
					}
				}
			}
		}
		public static void GrowShroom(int i, int y)
		{
			if (Main.tile[i - 1, y - 1].lava || Main.tile[i - 1, y - 1].lava || Main.tile[i + 1, y - 1].lava)
			{
				return;
			}
			if (Main.tile[i, y].active && Main.tile[i, y].type == 70 && Main.tile[i, y - 1].wall == 0 && Main.tile[i - 1, y].active && Main.tile[i - 1, y].type == 70 && Main.tile[i + 1, y].active && Main.tile[i + 1, y].type == 70 && WorldGen.EmptyTileCheck(i - 2, i + 2, y - 13, y - 1, 71))
			{
				int num = WorldGen.genRand.Next(4, 11);
				int num2;
				for (int j = y - num; j < y; j++)
				{
					Main.tile[i, j].frameNumber = (byte)WorldGen.genRand.Next(3);
					Main.tile[i, j].active = true;
					Main.tile[i, j].type = 72;
					num2 = WorldGen.genRand.Next(3);
					if (num2 == 0)
					{
						Main.tile[i, j].frameX = 0;
						Main.tile[i, j].frameY = 0;
					}
					if (num2 == 1)
					{
						Main.tile[i, j].frameX = 0;
						Main.tile[i, j].frameY = 18;
					}
					if (num2 == 2)
					{
						Main.tile[i, j].frameX = 0;
						Main.tile[i, j].frameY = 36;
					}
				}
				num2 = WorldGen.genRand.Next(3);
				if (num2 == 0)
				{
					Main.tile[i, y - num].frameX = 36;
					Main.tile[i, y - num].frameY = 0;
				}
				if (num2 == 1)
				{
					Main.tile[i, y - num].frameX = 36;
					Main.tile[i, y - num].frameY = 18;
				}
				if (num2 == 2)
				{
					Main.tile[i, y - num].frameX = 36;
					Main.tile[i, y - num].frameY = 36;
				}
				WorldGen.RangeFrame(i - 2, y - num - 1, i + 2, y + 1);
				if (Main.netMode == 2)
				{
					NetMessage.SendTileSquare(-1, i, (int)((double)y - (double)num * 0.5), num + 1);
				}
			}
		}
		public static void AddTrees()
		{
			for (int i = 1; i < Main.maxTilesX - 1; i++)
			{
				int num = 20;
				while ((double)num < Main.worldSurface)
				{
					WorldGen.GrowTree(i, num);
					num++;
				}
				if (WorldGen.genRand.Next(3) == 0)
				{
					i++;
				}
				if (WorldGen.genRand.Next(4) == 0)
				{
					i++;
				}
			}
		}
		public static bool EmptyTileCheck(int startX, int endX, int startY, int endY, int ignoreStyle = -1)
		{
			if (startX < 0)
			{
				return false;
			}
			if (endX >= Main.maxTilesX)
			{
				return false;
			}
			if (startY < 0)
			{
				return false;
			}
			if (endY >= Main.maxTilesY)
			{
				return false;
			}
			for (int i = startX; i < endX + 1; i++)
			{
				for (int j = startY; j < endY + 1; j++)
				{
					if (Main.tile[i, j].active)
					{
						if (ignoreStyle == -1)
						{
							return false;
						}
						if (ignoreStyle == 11 && Main.tile[i, j].type != 11)
						{
							return false;
						}
						if (ignoreStyle == 20 && Main.tile[i, j].type != 20 && Main.tile[i, j].type != 3 && Main.tile[i, j].type != 24 && Main.tile[i, j].type != 61 && Main.tile[i, j].type != 32 && Main.tile[i, j].type != 69 && Main.tile[i, j].type != 73 && Main.tile[i, j].type != 74 && Main.tile[i, j].type != 110 && Main.tile[i, j].type != 113)
						{
							return false;
						}
						if (ignoreStyle == 71 && Main.tile[i, j].type != 71)
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		public static void smCallBack(object threadContext)
		{
			if (Main.hardMode)
			{
				return;
			}
			WorldGen.hardLock = true;
			Main.hardMode = true;
			if (WorldGen.genRand == null)
			{
				WorldGen.genRand = new Random((int)DateTime.Now.Ticks);
			}
			float num = (float)WorldGen.genRand.Next(300, 400) * 0.001f;
			int i = (int)((float)Main.maxTilesX * num);
			int i2 = (int)((float)Main.maxTilesX * (1f - num));
			int num2 = 1;
			if (WorldGen.genRand.Next(2) == 0)
			{
				i2 = (int)((float)Main.maxTilesX * num);
				i = (int)((float)Main.maxTilesX * (1f - num));
				num2 = -1;
			}
			WorldGen.GERunner(i, 0, (float)(3 * num2), 5f, true);
			WorldGen.GERunner(i2, 0, (float)(3 * -(float)num2), 5f, false);
			if (Main.netMode == 0)
			{
				Main.NewText("The ancient spirits of light and dark have been released.", 50, 255, 130);
			}
			else
			{
				if (Main.netMode == 2)
				{
					NetMessage.SendData(25, -1, -1, "The ancient spirits of light and dark have been released.", 255, 50f, 255f, 130f, 0);
				}
			}
			if (Main.netMode == 2)
			{
				Netplay.ResetSections();
			}
			WorldGen.hardLock = false;
		}
		public static void StartHardmode()
		{
			if (Main.netMode == 1)
			{
				return;
			}
            if (WorldHooks.OnStartHardMode())
                return;
			ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.smCallBack), 1);
		}
		public static bool PlaceDoor(int i, int j, int type)
		{
			bool result;
			try
			{
				if (Main.tile[i, j - 2].active && Main.tileSolid[(int)Main.tile[i, j - 2].type] && Main.tile[i, j + 2].active && Main.tileSolid[(int)Main.tile[i, j + 2].type])
				{
					Main.tile[i, j - 1].active = true;
					Main.tile[i, j - 1].type = 10;
					Main.tile[i, j - 1].frameY = 0;
					Main.tile[i, j - 1].frameX = (short)(WorldGen.genRand.Next(3) * 18);
					Main.tile[i, j].active = true;
					Main.tile[i, j].type = 10;
					Main.tile[i, j].frameY = 18;
					Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(3) * 18);
					Main.tile[i, j + 1].active = true;
					Main.tile[i, j + 1].type = 10;
					Main.tile[i, j + 1].frameY = 36;
					Main.tile[i, j + 1].frameX = (short)(WorldGen.genRand.Next(3) * 18);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}
		public static bool CloseDoor(int i, int j, bool forced = false)
		{
			int num = 0;
			int num2 = i;
			int num3 = j;
			int frameX = (int)Main.tile[i, j].frameX;
			int frameY = (int)Main.tile[i, j].frameY;
			if (frameX == 0)
			{
				num2 = i;
				num = 1;
			}
			else
			{
				if (frameX == 18)
				{
					num2 = i - 1;
					num = 1;
				}
				else
				{
					if (frameX == 36)
					{
						num2 = i + 1;
						num = -1;
					}
					else
					{
						if (frameX == 54)
						{
							num2 = i;
							num = -1;
						}
					}
				}
			}
			if (frameY == 0)
			{
				num3 = j;
			}
			else
			{
				if (frameY == 18)
				{
					num3 = j - 1;
				}
				else
				{
					if (frameY == 36)
					{
						num3 = j - 2;
					}
				}
			}
			int num4 = num2;
			if (num == -1)
			{
				num4 = num2 - 1;
			}
			if (!forced)
			{
				for (int k = num3; k < num3 + 3; k++)
				{
					if (!Collision.EmptyTile(num2, k, true))
					{
						return false;
					}
				}
			}
			for (int l = num4; l < num4 + 2; l++)
			{
				for (int m = num3; m < num3 + 3; m++)
				{
					if (l == num2)
					{
						Main.tile[l, m].type = 10;
						Main.tile[l, m].frameX = (short)(WorldGen.genRand.Next(3) * 18);
					}
					else
					{
						Main.tile[l, m].active = false;
					}
				}
			}
			if (Main.netMode != 1)
			{
				int num5 = num2;
				for (int n = num3; n <= num3 + 2; n++)
				{
					if (WorldGen.numNoWire < WorldGen.maxWire - 1)
					{
						WorldGen.noWireX[WorldGen.numNoWire] = num5;
						WorldGen.noWireY[WorldGen.numNoWire] = n;
						WorldGen.numNoWire++;
					}
				}
			}
			for (int num6 = num2 - 1; num6 <= num2 + 1; num6++)
			{
				for (int num7 = num3 - 1; num7 <= num3 + 2; num7++)
				{
					WorldGen.TileFrame(num6, num7, false, false);
				}
			}
			Main.PlaySound(9, i * 16, j * 16, 1);
			return true;
		}
		public static bool AddLifeCrystal(int i, int j)
		{
			int k = j;
			while (k < Main.maxTilesY)
			{
				if (Main.tile[i, k].active && Main.tileSolid[(int)Main.tile[i, k].type])
				{
					int num = k - 1;
					if (Main.tile[i, num - 1].lava || Main.tile[i - 1, num - 1].lava)
					{
						return false;
					}
					if (!WorldGen.EmptyTileCheck(i - 1, i, num - 1, num, -1))
					{
						return false;
					}
					Main.tile[i - 1, num - 1].active = true;
					Main.tile[i - 1, num - 1].type = 12;
					Main.tile[i - 1, num - 1].frameX = 0;
					Main.tile[i - 1, num - 1].frameY = 0;
					Main.tile[i, num - 1].active = true;
					Main.tile[i, num - 1].type = 12;
					Main.tile[i, num - 1].frameX = 18;
					Main.tile[i, num - 1].frameY = 0;
					Main.tile[i - 1, num].active = true;
					Main.tile[i - 1, num].type = 12;
					Main.tile[i - 1, num].frameX = 0;
					Main.tile[i - 1, num].frameY = 18;
					Main.tile[i, num].active = true;
					Main.tile[i, num].type = 12;
					Main.tile[i, num].frameX = 18;
					Main.tile[i, num].frameY = 18;
					return true;
				}
				else
				{
					k++;
				}
			}
			return false;
		}
		public static void AddShadowOrb(int x, int y)
		{
			if (x < 10 || x > Main.maxTilesX - 10)
			{
				return;
			}
			if (y < 10 || y > Main.maxTilesY - 10)
			{
				return;
			}
			for (int i = x - 1; i < x + 1; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (Main.tile[i, j].active && Main.tile[i, j].type == 31)
					{
						return;
					}
				}
			}
			Main.tile[x - 1, y - 1].active = true;
			Main.tile[x - 1, y - 1].type = 31;
			Main.tile[x - 1, y - 1].frameX = 0;
			Main.tile[x - 1, y - 1].frameY = 0;
			Main.tile[x, y - 1].active = true;
			Main.tile[x, y - 1].type = 31;
			Main.tile[x, y - 1].frameX = 18;
			Main.tile[x, y - 1].frameY = 0;
			Main.tile[x - 1, y].active = true;
			Main.tile[x - 1, y].type = 31;
			Main.tile[x - 1, y].frameX = 0;
			Main.tile[x - 1, y].frameY = 18;
			Main.tile[x, y].active = true;
			Main.tile[x, y].type = 31;
			Main.tile[x, y].frameX = 18;
			Main.tile[x, y].frameY = 18;
		}
		public static void AddHellHouses()
		{
			int num = (int)((double)Main.maxTilesX * 0.25);
			for (int i = num; i < Main.maxTilesX - num; i++)
			{
				int num2 = Main.maxTilesY - 40;
				while (Main.tile[i, num2].active || Main.tile[i, num2].liquid > 0)
				{
					num2--;
				}
				if (Main.tile[i, num2 + 1].active)
				{
					byte b = (byte)WorldGen.genRand.Next(75, 77);
					byte wall = 13;
					if (WorldGen.genRand.Next(5) > 0)
					{
						b = 75;
					}
					if (b == 75)
					{
						wall = 14;
					}
					WorldGen.HellHouse(i, num2, b, wall);
					i += WorldGen.genRand.Next(15, 80);
				}
			}
			float num3 = (float)(Main.maxTilesX / 4200);
			int num4 = 0;
			while ((float)num4 < 200f * num3)
			{
				int num5 = 0;
				bool flag = false;
				while (!flag)
				{
					num5++;
					int num6 = WorldGen.genRand.Next((int)((double)Main.maxTilesX * 0.2), (int)((double)Main.maxTilesX * 0.8));
					int num7 = WorldGen.genRand.Next(Main.maxTilesY - 300, Main.maxTilesY - 20);
					if (Main.tile[num6, num7].active && (Main.tile[num6, num7].type == 75 || Main.tile[num6, num7].type == 76))
					{
						int num8 = 0;
						if (Main.tile[num6 - 1, num7].wall > 0)
						{
							num8 = -1;
						}
						else
						{
							if (Main.tile[num6 + 1, num7].wall > 0)
							{
								num8 = 1;
							}
						}
						if (!Main.tile[num6 + num8, num7].active && !Main.tile[num6 + num8, num7 + 1].active)
						{
							bool flag2 = false;
							for (int j = num6 - 8; j < num6 + 8; j++)
							{
								for (int k = num7 - 8; k < num7 + 8; k++)
								{
									if (Main.tile[j, k].active && Main.tile[j, k].type == 4)
									{
										flag2 = true;
										break;
									}
								}
							}
							if (!flag2)
							{
								WorldGen.PlaceTile(num6 + num8, num7, 4, true, true, -1, 7);
								flag = true;
							}
						}
					}
					if (num5 > 1000)
					{
						flag = true;
					}
				}
				num4++;
			}
		}
		public static void HellHouse(int i, int j, byte type = 76, byte wall = 13)
		{
			int num = WorldGen.genRand.Next(8, 20);
			int num2 = WorldGen.genRand.Next(1, 3);
			int num3 = WorldGen.genRand.Next(4, 13);
			int num4 = j;
			for (int k = 0; k < num2; k++)
			{
				int num5 = WorldGen.genRand.Next(5, 9);
				WorldGen.HellRoom(i, num4, num, num5, type, wall);
				num4 -= num5;
			}
			num4 = j;
			for (int l = 0; l < num3; l++)
			{
				int num6 = WorldGen.genRand.Next(5, 9);
				num4 += num6;
				WorldGen.HellRoom(i, num4, num, num6, type, wall);
			}
			for (int m = i - num / 2; m <= i + num / 2; m++)
			{
				num4 = j;
				while (num4 < Main.maxTilesY && ((Main.tile[m, num4].active && (Main.tile[m, num4].type == 76 || Main.tile[m, num4].type == 75)) || Main.tile[i, num4].wall == 13 || Main.tile[i, num4].wall == 14))
				{
					num4++;
				}
				int num7 = 6 + WorldGen.genRand.Next(3);
				while (num4 < Main.maxTilesY && !Main.tile[m, num4].active)
				{
					num7--;
					Main.tile[m, num4].active = true;
					Main.tile[m, num4].type = 57;
					num4++;
					if (num7 <= 0)
					{
						break;
					}
				}
			}
			int num8 = 0;
			int num9 = 0;
			num4 = j;
			while (num4 < Main.maxTilesY && ((Main.tile[i, num4].active && (Main.tile[i, num4].type == 76 || Main.tile[i, num4].type == 75)) || Main.tile[i, num4].wall == 13 || Main.tile[i, num4].wall == 14))
			{
				num4++;
			}
			num4--;
			num9 = num4;
			while ((Main.tile[i, num4].active && (Main.tile[i, num4].type == 76 || Main.tile[i, num4].type == 75)) || Main.tile[i, num4].wall == 13 || Main.tile[i, num4].wall == 14)
			{
				num4--;
				if (Main.tile[i, num4].active && (Main.tile[i, num4].type == 76 || Main.tile[i, num4].type == 75))
				{
					int num10 = WorldGen.genRand.Next(i - num / 2 + 1, i + num / 2 - 1);
					int num11 = WorldGen.genRand.Next(i - num / 2 + 1, i + num / 2 - 1);
					if (num10 > num11)
					{
						int num12 = num10;
						num10 = num11;
						num11 = num12;
					}
					if (num10 == num11)
					{
						if (num10 < i)
						{
							num11++;
						}
						else
						{
							num10--;
						}
					}
					for (int n = num10; n <= num11; n++)
					{
						if (Main.tile[n, num4 - 1].wall == 13)
						{
							Main.tile[n, num4].wall = 13;
						}
						if (Main.tile[n, num4 - 1].wall == 14)
						{
							Main.tile[n, num4].wall = 14;
						}
						Main.tile[n, num4].type = 19;
						Main.tile[n, num4].active = true;
					}
					num4--;
				}
			}
			num8 = num4;
			float num13 = (float)((num9 - num8) * num);
			float num14 = num13 * 0.02f;
			int num15 = 0;
			while ((float)num15 < num14)
			{
				int num16 = WorldGen.genRand.Next(i - num / 2, i + num / 2 + 1);
				int num17 = WorldGen.genRand.Next(num8, num9);
				int num18 = WorldGen.genRand.Next(3, 8);
				for (int num19 = num16 - num18; num19 <= num16 + num18; num19++)
				{
					for (int num20 = num17 - num18; num20 <= num17 + num18; num20++)
					{
						float num21 = (float)Math.Abs(num19 - num16);
						float num22 = (float)Math.Abs(num20 - num17);
						double num23 = Math.Sqrt((double)(num21 * num21 + num22 * num22));
						if (num23 < (double)num18 * 0.4)
						{
							try
							{
								if (Main.tile[num19, num20].type == 76 || Main.tile[num19, num20].type == 19)
								{
									Main.tile[num19, num20].active = false;
								}
								Main.tile[num19, num20].wall = 0;
							}
							catch
							{
							}
						}
					}
				}
				num15++;
			}
		}
		public static void HellRoom(int i, int j, int width, int height, byte type = 76, byte wall = 13)
		{
			if (j > Main.maxTilesY - 40)
			{
				return;
			}
			for (int k = i - width / 2; k <= i + width / 2; k++)
			{
				for (int l = j - height; l <= j; l++)
				{
					try
					{
						Main.tile[k, l].active = true;
						Main.tile[k, l].type = type;
						Main.tile[k, l].liquid = 0;
						Main.tile[k, l].lava = false;
					}
					catch
					{
					}
				}
			}
			for (int m = i - width / 2 + 1; m <= i + width / 2 - 1; m++)
			{
				for (int n = j - height + 1; n <= j - 1; n++)
				{
					try
					{
						Main.tile[m, n].active = false;
						Main.tile[m, n].wall = wall;
						Main.tile[m, n].liquid = 0;
						Main.tile[m, n].lava = false;
					}
					catch
					{
					}
				}
			}
		}
		public static void MakeDungeon(int x, int y, int tileType = 41, int wallType = 7)
		{
			int num = WorldGen.genRand.Next(3);
			int num2 = WorldGen.genRand.Next(3);
			switch (num)
			{
			    case 1:
			        tileType = 43;
			        break;
			    case 2:
			        tileType = 44;
			        break;
			}
			switch (num2)
			{
			    case 1:
			        wallType = 8;
			        break;
			    case 2:
			        wallType = 9;
			        break;
			}
			WorldGen.numDDoors = 0;
			WorldGen.numDPlats = 0;
			WorldGen.numDRooms = 0;
			WorldGen.dungeonX = x;
			WorldGen.dungeonY = y;
			WorldGen.dMinX = x;
			WorldGen.dMaxX = x;
			WorldGen.dMinY = y;
			WorldGen.dMaxY = y;
			WorldGen.dxStrength1 = (double)WorldGen.genRand.Next(25, 30);
			WorldGen.dyStrength1 = (double)WorldGen.genRand.Next(20, 25);
			WorldGen.dxStrength2 = (double)WorldGen.genRand.Next(35, 50);
			WorldGen.dyStrength2 = (double)WorldGen.genRand.Next(10, 15);
			float num3 = (float)(Main.maxTilesX / 60);
			num3 += (float)WorldGen.genRand.Next(0, (int)(num3 / 3f));
			float num4 = num3;
			int num5 = 5;
			WorldGen.DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
			while (num3 > 0f)
			{
				if (WorldGen.dungeonX < WorldGen.dMinX)
				{
					WorldGen.dMinX = WorldGen.dungeonX;
				}
				if (WorldGen.dungeonX > WorldGen.dMaxX)
				{
					WorldGen.dMaxX = WorldGen.dungeonX;
				}
				if (WorldGen.dungeonY > WorldGen.dMaxY)
				{
					WorldGen.dMaxY = WorldGen.dungeonY;
				}
				num3 -= 1f;
				Main.statusText = "Creating dungeon: " + (int)((num4 - num3) / num4 * 60f) + "%";
				if (num5 > 0)
				{
					num5--;
				}
				if (num5 == 0 & WorldGen.genRand.Next(3) == 0)
				{
					num5 = 5;
					if (WorldGen.genRand.Next(2) == 0)
					{
						int num6 = WorldGen.dungeonX;
						int num7 = WorldGen.dungeonY;
						WorldGen.DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, false);
						if (WorldGen.genRand.Next(2) == 0)
						{
							WorldGen.DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, false);
						}
						WorldGen.DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
						WorldGen.dungeonX = num6;
						WorldGen.dungeonY = num7;
					}
					else
					{
						WorldGen.DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
					}
				}
				else
				{
					WorldGen.DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, false);
				}
			}
			WorldGen.DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
			int num8 = WorldGen.dRoomX[0];
			int num9 = WorldGen.dRoomY[0];
			for (int i = 0; i < WorldGen.numDRooms; i++)
			{
				if (WorldGen.dRoomY[i] < num9)
				{
					num8 = WorldGen.dRoomX[i];
					num9 = WorldGen.dRoomY[i];
				}
			}
			WorldGen.dungeonX = num8;
			WorldGen.dungeonY = num9;
			WorldGen.dEnteranceX = num8;
			WorldGen.dSurface = false;
			num5 = 5;
			while (!WorldGen.dSurface)
			{
				if (num5 > 0)
				{
					num5--;
				}
				if ((num5 == 0 & WorldGen.genRand.Next(5) == 0) && (double)WorldGen.dungeonY > Main.worldSurface + 50.0)
				{
					num5 = 10;
					int num10 = WorldGen.dungeonX;
					int num11 = WorldGen.dungeonY;
					WorldGen.DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, true);
					WorldGen.DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
					WorldGen.dungeonX = num10;
					WorldGen.dungeonY = num11;
				}
				WorldGen.DungeonStairs(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
			}
			WorldGen.DungeonEnt(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
			Main.statusText = "Creating dungeon: 65%";
			for (int j = 0; j < WorldGen.numDRooms; j++)
			{
				for (int k = WorldGen.dRoomL[j]; k <= WorldGen.dRoomR[j]; k++)
				{
					if (!Main.tile[k, WorldGen.dRoomT[j] - 1].active)
					{
						WorldGen.DPlatX[WorldGen.numDPlats] = k;
						WorldGen.DPlatY[WorldGen.numDPlats] = WorldGen.dRoomT[j] - 1;
						WorldGen.numDPlats++;
						break;
					}
				}
				for (int l = WorldGen.dRoomL[j]; l <= WorldGen.dRoomR[j]; l++)
				{
					if (!Main.tile[l, WorldGen.dRoomB[j] + 1].active)
					{
						WorldGen.DPlatX[WorldGen.numDPlats] = l;
						WorldGen.DPlatY[WorldGen.numDPlats] = WorldGen.dRoomB[j] + 1;
						WorldGen.numDPlats++;
						break;
					}
				}
				for (int m = WorldGen.dRoomT[j]; m <= WorldGen.dRoomB[j]; m++)
				{
					if (!Main.tile[WorldGen.dRoomL[j] - 1, m].active)
					{
						WorldGen.DDoorX[WorldGen.numDDoors] = WorldGen.dRoomL[j] - 1;
						WorldGen.DDoorY[WorldGen.numDDoors] = m;
						WorldGen.DDoorPos[WorldGen.numDDoors] = -1;
						WorldGen.numDDoors++;
						break;
					}
				}
				for (int n = WorldGen.dRoomT[j]; n <= WorldGen.dRoomB[j]; n++)
				{
					if (!Main.tile[WorldGen.dRoomR[j] + 1, n].active)
					{
						WorldGen.DDoorX[WorldGen.numDDoors] = WorldGen.dRoomR[j] + 1;
						WorldGen.DDoorY[WorldGen.numDDoors] = n;
						WorldGen.DDoorPos[WorldGen.numDDoors] = 1;
						WorldGen.numDDoors++;
						break;
					}
				}
			}
			Main.statusText = "Creating dungeon: 70%";
			int num12 = 0;
			int num13 = 1000;
			int num14 = 0;
			while (num14 < Main.maxTilesX / 100)
			{
				num12++;
				int num15 = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
				int num16 = WorldGen.genRand.Next((int)Main.worldSurface + 25, WorldGen.dMaxY);
				int num17 = num15;
				if ((int)Main.tile[num15, num16].wall == wallType && !Main.tile[num15, num16].active)
				{
					int num18 = 1;
					if (WorldGen.genRand.Next(2) == 0)
					{
						num18 = -1;
					}
					while (!Main.tile[num15, num16].active)
					{
						num16 += num18;
					}
					if (Main.tile[num15 - 1, num16].active && Main.tile[num15 + 1, num16].active && !Main.tile[num15 - 1, num16 - num18].active && !Main.tile[num15 + 1, num16 - num18].active)
					{
						num14++;
						int num19 = WorldGen.genRand.Next(5, 13);
						while (Main.tile[num15 - 1, num16].active && Main.tile[num15, num16 + num18].active && Main.tile[num15, num16].active && !Main.tile[num15, num16 - num18].active && num19 > 0)
						{
							Main.tile[num15, num16].type = 48;
							if (!Main.tile[num15 - 1, num16 - num18].active && !Main.tile[num15 + 1, num16 - num18].active)
							{
								Main.tile[num15, num16 - num18].type = 48;
								Main.tile[num15, num16 - num18].active = true;
							}
							num15--;
							num19--;
						}
						num19 = WorldGen.genRand.Next(5, 13);
						num15 = num17 + 1;
						while (Main.tile[num15 + 1, num16].active && Main.tile[num15, num16 + num18].active && Main.tile[num15, num16].active && !Main.tile[num15, num16 - num18].active && num19 > 0)
						{
							Main.tile[num15, num16].type = 48;
							if (!Main.tile[num15 - 1, num16 - num18].active && !Main.tile[num15 + 1, num16 - num18].active)
							{
								Main.tile[num15, num16 - num18].type = 48;
								Main.tile[num15, num16 - num18].active = true;
							}
							num15++;
							num19--;
						}
					}
				}
				if (num12 > num13)
				{
					num12 = 0;
					num14++;
				}
			}
			num12 = 0;
			num13 = 1000;
			num14 = 0;
			Main.statusText = "Creating dungeon: 75%";
			while (num14 < Main.maxTilesX / 100)
			{
				num12++;
				int num20 = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
				int num21 = WorldGen.genRand.Next((int)Main.worldSurface + 25, WorldGen.dMaxY);
				int num22 = num21;
				if ((int)Main.tile[num20, num21].wall == wallType && !Main.tile[num20, num21].active)
				{
					int num23 = 1;
					if (WorldGen.genRand.Next(2) == 0)
					{
						num23 = -1;
					}
					while (num20 > 5 && num20 < Main.maxTilesX - 5 && !Main.tile[num20, num21].active)
					{
						num20 += num23;
					}
					if (Main.tile[num20, num21 - 1].active && Main.tile[num20, num21 + 1].active && !Main.tile[num20 - num23, num21 - 1].active && !Main.tile[num20 - num23, num21 + 1].active)
					{
						num14++;
						int num24 = WorldGen.genRand.Next(5, 13);
						while (Main.tile[num20, num21 - 1].active && Main.tile[num20 + num23, num21].active && Main.tile[num20, num21].active && !Main.tile[num20 - num23, num21].active && num24 > 0)
						{
							Main.tile[num20, num21].type = 48;
							if (!Main.tile[num20 - num23, num21 - 1].active && !Main.tile[num20 - num23, num21 + 1].active)
							{
								Main.tile[num20 - num23, num21].type = 48;
								Main.tile[num20 - num23, num21].active = true;
							}
							num21--;
							num24--;
						}
						num24 = WorldGen.genRand.Next(5, 13);
						num21 = num22 + 1;
						while (Main.tile[num20, num21 + 1].active && Main.tile[num20 + num23, num21].active && Main.tile[num20, num21].active && !Main.tile[num20 - num23, num21].active && num24 > 0)
						{
							Main.tile[num20, num21].type = 48;
							if (!Main.tile[num20 - num23, num21 - 1].active && !Main.tile[num20 - num23, num21 + 1].active)
							{
								Main.tile[num20 - num23, num21].type = 48;
								Main.tile[num20 - num23, num21].active = true;
							}
							num21++;
							num24--;
						}
					}
				}
				if (num12 > num13)
				{
					num12 = 0;
					num14++;
				}
			}
			Main.statusText = "Creating dungeon: 80%";
			for (int num25 = 0; num25 < WorldGen.numDDoors; num25++)
			{
				int num26 = WorldGen.DDoorX[num25] - 10;
				int num27 = WorldGen.DDoorX[num25] + 10;
				int num28 = 100;
				int num29 = 0;
				for (int num30 = num26; num30 < num27; num30++)
				{
					bool flag = true;
					int num31 = WorldGen.DDoorY[num25];
					while (!Main.tile[num30, num31].active)
					{
						num31--;
					}
					if (!Main.tileDungeon[(int)Main.tile[num30, num31].type])
					{
						flag = false;
					}
					int num32 = num31;
					num31 = WorldGen.DDoorY[num25];
					while (!Main.tile[num30, num31].active)
					{
						num31++;
					}
					if (!Main.tileDungeon[(int)Main.tile[num30, num31].type])
					{
						flag = false;
					}
					int num33 = num31;
					if (num33 - num32 >= 3)
					{
						int num34 = num30 - 20;
						int num35 = num30 + 20;
						int num36 = num33 - 10;
						int num37 = num33 + 10;
						for (int num38 = num34; num38 < num35; num38++)
						{
							for (int num39 = num36; num39 < num37; num39++)
							{
								if (Main.tile[num38, num39].active && Main.tile[num38, num39].type == 10)
								{
									flag = false;
									break;
								}
							}
						}
						if (flag)
						{
							for (int num40 = num33 - 3; num40 < num33; num40++)
							{
								for (int num41 = num30 - 3; num41 <= num30 + 3; num41++)
								{
									if (Main.tile[num41, num40].active)
									{
										flag = false;
										break;
									}
								}
							}
						}
						if (flag && num33 - num32 < 20)
						{
							bool flag2 = false;
							if (WorldGen.DDoorPos[num25] == 0 && num33 - num32 < num28)
							{
								flag2 = true;
							}
							if (WorldGen.DDoorPos[num25] == -1 && num30 > num29)
							{
								flag2 = true;
							}
							if (WorldGen.DDoorPos[num25] == 1 && (num30 < num29 || num29 == 0))
							{
								flag2 = true;
							}
							if (flag2)
							{
								num29 = num30;
								num28 = num33 - num32;
							}
						}
					}
				}
				if (num28 < 20)
				{
					int num42 = num29;
					int num43 = WorldGen.DDoorY[num25];
					int num44 = num43;
					while (!Main.tile[num42, num43].active)
					{
						Main.tile[num42, num43].active = false;
						num43++;
					}
					while (!Main.tile[num42, num44].active)
					{
						num44--;
					}
					num43--;
					num44++;
					for (int num45 = num44; num45 < num43 - 2; num45++)
					{
						Main.tile[num42, num45].active = true;
						Main.tile[num42, num45].type = (byte)tileType;
					}
					WorldGen.PlaceTile(num42, num43, 10, true, false, -1, 0);
					num42--;
					int num46 = num43 - 3;
					while (!Main.tile[num42, num46].active)
					{
						num46--;
					}
					if (num43 - num46 < num43 - num44 + 5 && Main.tileDungeon[(int)Main.tile[num42, num46].type])
					{
						for (int num47 = num43 - 4 - WorldGen.genRand.Next(3); num47 > num46; num47--)
						{
							Main.tile[num42, num47].active = true;
							Main.tile[num42, num47].type = (byte)tileType;
						}
					}
					num42 += 2;
					num46 = num43 - 3;
					while (!Main.tile[num42, num46].active)
					{
						num46--;
					}
					if (num43 - num46 < num43 - num44 + 5 && Main.tileDungeon[(int)Main.tile[num42, num46].type])
					{
						for (int num48 = num43 - 4 - WorldGen.genRand.Next(3); num48 > num46; num48--)
						{
							Main.tile[num42, num48].active = true;
							Main.tile[num42, num48].type = (byte)tileType;
						}
					}
					num43++;
					num42--;
					Main.tile[num42 - 1, num43].active = true;
					Main.tile[num42 - 1, num43].type = (byte)tileType;
					Main.tile[num42 + 1, num43].active = true;
					Main.tile[num42 + 1, num43].type = (byte)tileType;
				}
			}
			Main.statusText = "Creating dungeon: 85%";
			for (int num49 = 0; num49 < WorldGen.numDPlats; num49++)
			{
				int num50 = WorldGen.DPlatX[num49];
				int num51 = WorldGen.DPlatY[num49];
				int num52 = Main.maxTilesX;
				int num53 = 10;
				for (int num54 = num51 - 5; num54 <= num51 + 5; num54++)
				{
					int num55 = num50;
					int num56 = num50;
					bool flag3 = false;
					if (Main.tile[num55, num54].active)
					{
						flag3 = true;
					}
					else
					{
						while (!Main.tile[num55, num54].active)
						{
							num55--;
							if (!Main.tileDungeon[(int)Main.tile[num55, num54].type])
							{
								flag3 = true;
							}
						}
						while (!Main.tile[num56, num54].active)
						{
							num56++;
							if (!Main.tileDungeon[(int)Main.tile[num56, num54].type])
							{
								flag3 = true;
							}
						}
					}
					if (!flag3 && num56 - num55 <= num53)
					{
						bool flag4 = true;
						int num57 = num50 - num53 / 2 - 2;
						int num58 = num50 + num53 / 2 + 2;
						int num59 = num54 - 5;
						int num60 = num54 + 5;
						for (int num61 = num57; num61 <= num58; num61++)
						{
							for (int num62 = num59; num62 <= num60; num62++)
							{
								if (Main.tile[num61, num62].active && Main.tile[num61, num62].type == 19)
								{
									flag4 = false;
									break;
								}
							}
						}
						for (int num63 = num54 + 3; num63 >= num54 - 5; num63--)
						{
							if (Main.tile[num50, num63].active)
							{
								flag4 = false;
								break;
							}
						}
						if (flag4)
						{
							num52 = num54;
							break;
						}
					}
				}
				if (num52 > num51 - 10 && num52 < num51 + 10)
				{
					int num64 = num50;
					int num65 = num52;
					int num66 = num50 + 1;
					while (!Main.tile[num64, num65].active)
					{
						Main.tile[num64, num65].active = true;
						Main.tile[num64, num65].type = 19;
						num64--;
					}
					while (!Main.tile[num66, num65].active)
					{
						Main.tile[num66, num65].active = true;
						Main.tile[num66, num65].type = 19;
						num66++;
					}
				}
			}
			Main.statusText = "Creating dungeon: 90%";
			num12 = 0;
			num13 = 1000;
			num14 = 0;
			while (num14 < Main.maxTilesX / 20)
			{
				num12++;
				int num67 = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
				int num68 = WorldGen.genRand.Next(WorldGen.dMinY, WorldGen.dMaxY);
				bool flag5 = true;
				if ((int)Main.tile[num67, num68].wall == wallType && !Main.tile[num67, num68].active)
				{
					int num69 = 1;
					if (WorldGen.genRand.Next(2) == 0)
					{
						num69 = -1;
					}
					while (flag5 && !Main.tile[num67, num68].active)
					{
						num67 -= num69;
						if (num67 < 5 || num67 > Main.maxTilesX - 5)
						{
							flag5 = false;
						}
						else
						{
							if (Main.tile[num67, num68].active && !Main.tileDungeon[(int)Main.tile[num67, num68].type])
							{
								flag5 = false;
							}
						}
					}
					if (flag5 && Main.tile[num67, num68].active && Main.tileDungeon[(int)Main.tile[num67, num68].type] && Main.tile[num67, num68 - 1].active && Main.tileDungeon[(int)Main.tile[num67, num68 - 1].type] && Main.tile[num67, num68 + 1].active && Main.tileDungeon[(int)Main.tile[num67, num68 + 1].type])
					{
						num67 += num69;
						for (int num70 = num67 - 3; num70 <= num67 + 3; num70++)
						{
							for (int num71 = num68 - 3; num71 <= num68 + 3; num71++)
							{
								if (Main.tile[num70, num71].active && Main.tile[num70, num71].type == 19)
								{
									flag5 = false;
									break;
								}
							}
						}
						if (flag5 && (!Main.tile[num67, num68 - 1].active & !Main.tile[num67, num68 - 2].active & !Main.tile[num67, num68 - 3].active))
						{
							int num72 = num67;
							int num73 = num67;
							while (num72 > WorldGen.dMinX && num72 < WorldGen.dMaxX && !Main.tile[num72, num68].active && !Main.tile[num72, num68 - 1].active && !Main.tile[num72, num68 + 1].active)
							{
								num72 += num69;
							}
							num72 = Math.Abs(num67 - num72);
							bool flag6 = false;
							if (WorldGen.genRand.Next(2) == 0)
							{
								flag6 = true;
							}
							if (num72 > 5)
							{
								for (int num74 = WorldGen.genRand.Next(1, 4); num74 > 0; num74--)
								{
									Main.tile[num67, num68].active = true;
									Main.tile[num67, num68].type = 19;
									if (flag6)
									{
										WorldGen.PlaceTile(num67, num68 - 1, 50, true, false, -1, 0);
										if (WorldGen.genRand.Next(50) == 0 && Main.tile[num67, num68 - 1].type == 50)
										{
											Main.tile[num67, num68 - 1].frameX = 90;
										}
									}
									num67 += num69;
								}
								num12 = 0;
								num14++;
								if (!flag6 && WorldGen.genRand.Next(2) == 0)
								{
									num67 = num73;
									num68--;
									int num75 = 0;
									if (WorldGen.genRand.Next(4) == 0)
									{
										num75 = 1;
									}
									if (num75 == 0)
									{
										num75 = 13;
									}
									else
									{
										if (num75 == 1)
										{
											num75 = 49;
										}
									}
									WorldGen.PlaceTile(num67, num68, num75, true, false, -1, 0);
									if (Main.tile[num67, num68].type == 13)
									{
										if (WorldGen.genRand.Next(2) == 0)
										{
											Main.tile[num67, num68].frameX = 18;
										}
										else
										{
											Main.tile[num67, num68].frameX = 36;
										}
									}
								}
							}
						}
					}
				}
				if (num12 > num13)
				{
					num12 = 0;
					num14++;
				}
			}
			Main.statusText = "Creating dungeon: 95%";
			int num76 = 0;
			for (int num77 = 0; num77 < WorldGen.numDRooms; num77++)
			{
				int num78 = 0;
				while (num78 < 1000)
				{
					int num79 = (int)((double)WorldGen.dRoomSize[num77] * 0.4);
					int i2 = WorldGen.dRoomX[num77] + WorldGen.genRand.Next(-num79, num79 + 1);
					int num80 = WorldGen.dRoomY[num77] + WorldGen.genRand.Next(-num79, num79 + 1);
					int num81 = 0;
					num76++;
					int style = 2;
					switch (num76)
					{
					    case 1:
					        num81 = 329;
					        break;
					    case 2:
					        num81 = 155;
					        break;
					    case 3:
					        num81 = 156;
					        break;
					    case 4:
					        num81 = 157;
					        break;
					    case 5:
					        num81 = 163;
					        break;
					    case 6:
					        num81 = 113;
					        break;
					    case 7:
					        num81 = 327;
					        style = 0;
					        break;
					    default:
					        num81 = 164;
					        num76 = 0;
					        break;
					}
					if ((double)num80 < Main.worldSurface + 50.0)
					{
						num81 = 327;
						style = 0;
					}
					if (num81 == 0 && WorldGen.genRand.Next(2) == 0)
					{
						num78 = 1000;
					}
					else
					{
						if (WorldGen.AddBuriedChest(i2, num80, num81, false, style))
						{
							num78 += 1000;
						}
						num78++;
					}
				}
			}
			WorldGen.dMinX -= 25;
			WorldGen.dMaxX += 25;
			WorldGen.dMinY -= 25;
			WorldGen.dMaxY += 25;
			if (WorldGen.dMinX < 0)
			{
				WorldGen.dMinX = 0;
			}
			if (WorldGen.dMaxX > Main.maxTilesX)
			{
				WorldGen.dMaxX = Main.maxTilesX;
			}
			if (WorldGen.dMinY < 0)
			{
				WorldGen.dMinY = 0;
			}
			if (WorldGen.dMaxY > Main.maxTilesY)
			{
				WorldGen.dMaxY = Main.maxTilesY;
			}
			num12 = 0;
			num13 = 1000;
			num14 = 0;
			while (num14 < Main.maxTilesX / 150)
			{
				num12++;
				int num82 = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
				int num83 = WorldGen.genRand.Next(WorldGen.dMinY, WorldGen.dMaxY);
				if ((int)Main.tile[num82, num83].wall == wallType)
				{
					int num84 = num83;
					while (num84 > WorldGen.dMinY)
					{
						if (Main.tile[num82, num84 - 1].active && (int)Main.tile[num82, num84 - 1].type == tileType)
						{
							bool flag7 = false;
							for (int num85 = num82 - 15; num85 < num82 + 15; num85++)
							{
								for (int num86 = num84 - 15; num86 < num84 + 15; num86++)
								{
									if (num85 > 0 && num85 < Main.maxTilesX && num86 > 0 && num86 < Main.maxTilesY && Main.tile[num85, num86].type == 42)
									{
										flag7 = true;
										break;
									}
								}
							}
							if (Main.tile[num82 - 1, num84].active || Main.tile[num82 + 1, num84].active || Main.tile[num82 - 1, num84 + 1].active || Main.tile[num82 + 1, num84 + 1].active || Main.tile[num82, num84 + 2].active)
							{
								flag7 = true;
							}
							if (flag7)
							{
								break;
							}
							WorldGen.Place1x2Top(num82, num84, 42);
							if (Main.tile[num82, num84].type == 42)
							{
								num12 = 0;
								num14++;
								for (int num87 = 0; num87 < 1000; num87++)
								{
									int num88 = num82 + WorldGen.genRand.Next(-12, 13);
									int num89 = num84 + WorldGen.genRand.Next(3, 21);
									if (!Main.tile[num88, num89].active && !Main.tile[num88, num89 + 1].active && Main.tile[num88 - 1, num89].type != 48 && Main.tile[num88 + 1, num89].type != 48 && Collision.CanHit(new Vector2((float)(num88 * 16), (float)(num89 * 16)), 16, 16, new Vector2((float)(num82 * 16), (float)(num84 * 16 + 1)), 16, 16))
									{
										WorldGen.PlaceTile(num88, num89, 136, true, false, -1, 0);
										if (Main.tile[num88, num89].active)
										{
											while (num88 != num82 || num89 != num84)
											{
												Main.tile[num88, num89].wire = true;
												if (num88 > num82)
												{
													num88--;
												}
												if (num88 < num82)
												{
													num88++;
												}
												Main.tile[num88, num89].wire = true;
												if (num89 > num84)
												{
													num89--;
												}
												if (num89 < num84)
												{
													num89++;
												}
												Main.tile[num88, num89].wire = true;
											}
											if (Main.rand.Next(3) > 0)
											{
												Main.tile[num82, num84].frameX = 18;
												Main.tile[num82, num84 + 1].frameX = 18;
												break;
											}
											break;
										}
									}
								}
								break;
							}
							break;
						}
						else
						{
							num84--;
						}
					}
				}
				if (num12 > num13)
				{
					num14++;
					num12 = 0;
				}
			}
			num12 = 0;
			num13 = 1000;
			num14 = 0;
			while (num14 < Main.maxTilesX / 500)
			{
				num12++;
				int num90 = WorldGen.genRand.Next(WorldGen.dMinX, WorldGen.dMaxX);
				int num91 = WorldGen.genRand.Next(WorldGen.dMinY, WorldGen.dMaxY);
				if ((int)Main.tile[num90, num91].wall == wallType && WorldGen.placeTrap(num90, num91, 0))
				{
					num12 = num13;
				}
				if (num12 > num13)
				{
					num14++;
					num12 = 0;
				}
			}
		}
		public static void DungeonStairs(int i, int j, int tileType, int wallType)
		{
			Vector2 value = default(Vector2);
			double num = (double)WorldGen.genRand.Next(5, 9);
			int num2 = 1;
			Vector2 value2;
			value2.X = (float)i;
			value2.Y = (float)j;
			int k = WorldGen.genRand.Next(10, 30);
			if (i > WorldGen.dEnteranceX)
			{
				num2 = -1;
			}
			else
			{
				num2 = 1;
			}
			if (i > Main.maxTilesX - 400)
			{
				num2 = -1;
			}
			else
			{
				if (i < 400)
				{
					num2 = 1;
				}
			}
			value.Y = -1f;
			value.X = (float)num2;
			if (WorldGen.genRand.Next(3) == 0)
			{
				value.X *= 0.5f;
			}
			else
			{
				if (WorldGen.genRand.Next(3) == 0)
				{
					value.Y *= 2f;
				}
			}
			while (k > 0)
			{
				k--;
				int num3 = (int)((double)value2.X - num - 4.0 - (double)WorldGen.genRand.Next(6));
				int num4 = (int)((double)value2.X + num + 4.0 + (double)WorldGen.genRand.Next(6));
				int num5 = (int)((double)value2.Y - num - 4.0);
				int num6 = (int)((double)value2.Y + num + 4.0 + (double)WorldGen.genRand.Next(6));
				if (num3 < 0)
				{
					num3 = 0;
				}
				if (num4 > Main.maxTilesX)
				{
					num4 = Main.maxTilesX;
				}
				if (num5 < 0)
				{
					num5 = 0;
				}
				if (num6 > Main.maxTilesY)
				{
					num6 = Main.maxTilesY;
				}
				int num7 = 1;
				if (value2.X > (float)(Main.maxTilesX / 2))
				{
					num7 = -1;
				}
				int num8 = (int)(value2.X + (float)WorldGen.dxStrength1 * 0.6f * (float)num7 + (float)WorldGen.dxStrength2 * (float)num7);
				int num9 = (int)(WorldGen.dyStrength2 * 0.5);
				if ((double)value2.Y < Main.worldSurface - 5.0 && Main.tile[num8, (int)((double)value2.Y - num - 6.0 + (double)num9)].wall == 0 && Main.tile[num8, (int)((double)value2.Y - num - 7.0 + (double)num9)].wall == 0 && Main.tile[num8, (int)((double)value2.Y - num - 8.0 + (double)num9)].wall == 0)
				{
					WorldGen.dSurface = true;
					WorldGen.TileRunner(num8, (int)((double)value2.Y - num - 6.0 + (double)num9), (double)WorldGen.genRand.Next(25, 35), WorldGen.genRand.Next(10, 20), -1, false, 0f, -1f, false, true);
				}
				for (int l = num3; l < num4; l++)
				{
					for (int m = num5; m < num6; m++)
					{
						Main.tile[l, m].liquid = 0;
						if ((int)Main.tile[l, m].wall != wallType)
						{
							Main.tile[l, m].wall = 0;
							Main.tile[l, m].active = true;
							Main.tile[l, m].type = (byte)tileType;
						}
					}
				}
				for (int n = num3 + 1; n < num4 - 1; n++)
				{
					for (int num10 = num5 + 1; num10 < num6 - 1; num10++)
					{
						WorldGen.PlaceWall(n, num10, wallType, true);
					}
				}
				int num11 = 0;
				if (WorldGen.genRand.Next((int)num) == 0)
				{
					num11 = WorldGen.genRand.Next(1, 3);
				}
				num3 = (int)((double)value2.X - num * 0.5 - (double)num11);
				num4 = (int)((double)value2.X + num * 0.5 + (double)num11);
				num5 = (int)((double)value2.Y - num * 0.5 - (double)num11);
				num6 = (int)((double)value2.Y + num * 0.5 + (double)num11);
				if (num3 < 0)
				{
					num3 = 0;
				}
				if (num4 > Main.maxTilesX)
				{
					num4 = Main.maxTilesX;
				}
				if (num5 < 0)
				{
					num5 = 0;
				}
				if (num6 > Main.maxTilesY)
				{
					num6 = Main.maxTilesY;
				}
				for (int num12 = num3; num12 < num4; num12++)
				{
					for (int num13 = num5; num13 < num6; num13++)
					{
						Main.tile[num12, num13].active = false;
						WorldGen.PlaceWall(num12, num13, wallType, true);
					}
				}
				if (WorldGen.dSurface)
				{
					k = 0;
				}
				value2 += value;
			}
			WorldGen.dungeonX = (int)value2.X;
			WorldGen.dungeonY = (int)value2.Y;
		}
		public static void DungeonHalls(int i, int j, int tileType, int wallType, bool forceX = false)
		{
			Vector2 value = default(Vector2);
			double num = (double)WorldGen.genRand.Next(4, 6);
			Vector2 vector = default(Vector2);
			Vector2 value2 = default(Vector2);
			int num2 = 1;
			Vector2 value3;
			value3.X = (float)i;
			value3.Y = (float)j;
			int k = WorldGen.genRand.Next(35, 80);
			if (forceX)
			{
				k += 20;
				WorldGen.lastDungeonHall = default(Vector2);
			}
			else
			{
				if (WorldGen.genRand.Next(5) == 0)
				{
					num *= 2.0;
					k /= 2;
				}
			}
			bool flag = false;
			while (!flag)
			{
				if (WorldGen.genRand.Next(2) == 0)
				{
					num2 = -1;
				}
				else
				{
					num2 = 1;
				}
				bool flag2 = false;
				if (WorldGen.genRand.Next(2) == 0)
				{
					flag2 = true;
				}
				if (forceX)
				{
					flag2 = true;
				}
				if (flag2)
				{
					vector.Y = 0f;
					vector.X = (float)num2;
					value2.Y = 0f;
					value2.X = (float)(-(float)num2);
					value.Y = 0f;
					value.X = (float)num2;
					if (WorldGen.genRand.Next(3) == 0)
					{
						if (WorldGen.genRand.Next(2) == 0)
						{
							value.Y = -0.2f;
						}
						else
						{
							value.Y = 0.2f;
						}
					}
				}
				else
				{
					num += 1.0;
					value.Y = (float)num2;
					value.X = 0f;
					vector.X = 0f;
					vector.Y = (float)num2;
					value2.X = 0f;
					value2.Y = (float)(-(float)num2);
					if (WorldGen.genRand.Next(2) == 0)
					{
						if (WorldGen.genRand.Next(2) == 0)
						{
							value.X = 0.3f;
						}
						else
						{
							value.X = -0.3f;
						}
					}
					else
					{
						k /= 2;
					}
				}
				if (WorldGen.lastDungeonHall != value2)
				{
					flag = true;
				}
			}
			if (!forceX)
			{
				if (value3.X > (float)(WorldGen.lastMaxTilesX - 200))
				{
					num2 = -1;
					vector.Y = 0f;
					vector.X = (float)num2;
					value.Y = 0f;
					value.X = (float)num2;
					if (WorldGen.genRand.Next(3) == 0)
					{
						if (WorldGen.genRand.Next(2) == 0)
						{
							value.Y = -0.2f;
						}
						else
						{
							value.Y = 0.2f;
						}
					}
				}
				else
				{
					if (value3.X < 200f)
					{
						num2 = 1;
						vector.Y = 0f;
						vector.X = (float)num2;
						value.Y = 0f;
						value.X = (float)num2;
						if (WorldGen.genRand.Next(3) == 0)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								value.Y = -0.2f;
							}
							else
							{
								value.Y = 0.2f;
							}
						}
					}
					else
					{
						if (value3.Y > (float)(WorldGen.lastMaxTilesY - 300))
						{
							num2 = -1;
							num += 1.0;
							value.Y = (float)num2;
							value.X = 0f;
							vector.X = 0f;
							vector.Y = (float)num2;
							if (WorldGen.genRand.Next(2) == 0)
							{
								if (WorldGen.genRand.Next(2) == 0)
								{
									value.X = 0.3f;
								}
								else
								{
									value.X = -0.3f;
								}
							}
						}
						else
						{
							if ((double)value3.Y < Main.rockLayer)
							{
								num2 = 1;
								num += 1.0;
								value.Y = (float)num2;
								value.X = 0f;
								vector.X = 0f;
								vector.Y = (float)num2;
								if (WorldGen.genRand.Next(2) == 0)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										value.X = 0.3f;
									}
									else
									{
										value.X = -0.3f;
									}
								}
							}
							else
							{
								if (value3.X < (float)(Main.maxTilesX / 2) && (double)value3.X > (double)Main.maxTilesX * 0.25)
								{
									num2 = -1;
									vector.Y = 0f;
									vector.X = (float)num2;
									value.Y = 0f;
									value.X = (float)num2;
									if (WorldGen.genRand.Next(3) == 0)
									{
										if (WorldGen.genRand.Next(2) == 0)
										{
											value.Y = -0.2f;
										}
										else
										{
											value.Y = 0.2f;
										}
									}
								}
								else
								{
									if (value3.X > (float)(Main.maxTilesX / 2) && (double)value3.X < (double)Main.maxTilesX * 0.75)
									{
										num2 = 1;
										vector.Y = 0f;
										vector.X = (float)num2;
										value.Y = 0f;
										value.X = (float)num2;
										if (WorldGen.genRand.Next(3) == 0)
										{
											if (WorldGen.genRand.Next(2) == 0)
											{
												value.Y = -0.2f;
											}
											else
											{
												value.Y = 0.2f;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			if (vector.Y == 0f)
			{
				WorldGen.DDoorX[WorldGen.numDDoors] = (int)value3.X;
				WorldGen.DDoorY[WorldGen.numDDoors] = (int)value3.Y;
				WorldGen.DDoorPos[WorldGen.numDDoors] = 0;
				WorldGen.numDDoors++;
			}
			else
			{
				WorldGen.DPlatX[WorldGen.numDPlats] = (int)value3.X;
				WorldGen.DPlatY[WorldGen.numDPlats] = (int)value3.Y;
				WorldGen.numDPlats++;
			}
			WorldGen.lastDungeonHall = vector;
			while (k > 0)
			{
				if (vector.X > 0f && value3.X > (float)(Main.maxTilesX - 100))
				{
					k = 0;
				}
				else
				{
					if (vector.X < 0f && value3.X < 100f)
					{
						k = 0;
					}
					else
					{
						if (vector.Y > 0f && value3.Y > (float)(Main.maxTilesY - 100))
						{
							k = 0;
						}
						else
						{
							if (vector.Y < 0f && (double)value3.Y < Main.rockLayer + 50.0)
							{
								k = 0;
							}
						}
					}
				}
				k--;
				int num3 = (int)((double)value3.X - num - 4.0 - (double)WorldGen.genRand.Next(6));
				int num4 = (int)((double)value3.X + num + 4.0 + (double)WorldGen.genRand.Next(6));
				int num5 = (int)((double)value3.Y - num - 4.0 - (double)WorldGen.genRand.Next(6));
				int num6 = (int)((double)value3.Y + num + 4.0 + (double)WorldGen.genRand.Next(6));
				if (num3 < 0)
				{
					num3 = 0;
				}
				if (num4 > Main.maxTilesX)
				{
					num4 = Main.maxTilesX;
				}
				if (num5 < 0)
				{
					num5 = 0;
				}
				if (num6 > Main.maxTilesY)
				{
					num6 = Main.maxTilesY;
				}
				for (int l = num3; l < num4; l++)
				{
					for (int m = num5; m < num6; m++)
					{
						Main.tile[l, m].liquid = 0;
						if (Main.tile[l, m].wall == 0)
						{
							Main.tile[l, m].active = true;
							Main.tile[l, m].type = (byte)tileType;
						}
					}
				}
				for (int n = num3 + 1; n < num4 - 1; n++)
				{
					for (int num7 = num5 + 1; num7 < num6 - 1; num7++)
					{
						WorldGen.PlaceWall(n, num7, wallType, true);
					}
				}
				int num8 = 0;
				if (value.Y == 0f && WorldGen.genRand.Next((int)num + 1) == 0)
				{
					num8 = WorldGen.genRand.Next(1, 3);
				}
				else
				{
					if (value.X == 0f && WorldGen.genRand.Next((int)num - 1) == 0)
					{
						num8 = WorldGen.genRand.Next(1, 3);
					}
					else
					{
						if (WorldGen.genRand.Next((int)num * 3) == 0)
						{
							num8 = WorldGen.genRand.Next(1, 3);
						}
					}
				}
				num3 = (int)((double)value3.X - num * 0.5 - (double)num8);
				num4 = (int)((double)value3.X + num * 0.5 + (double)num8);
				num5 = (int)((double)value3.Y - num * 0.5 - (double)num8);
				num6 = (int)((double)value3.Y + num * 0.5 + (double)num8);
				if (num3 < 0)
				{
					num3 = 0;
				}
				if (num4 > Main.maxTilesX)
				{
					num4 = Main.maxTilesX;
				}
				if (num5 < 0)
				{
					num5 = 0;
				}
				if (num6 > Main.maxTilesY)
				{
					num6 = Main.maxTilesY;
				}
				for (int num9 = num3; num9 < num4; num9++)
				{
					for (int num10 = num5; num10 < num6; num10++)
					{
						Main.tile[num9, num10].active = false;
						Main.tile[num9, num10].wall = (byte)wallType;
					}
				}
				value3 += value;
			}
			WorldGen.dungeonX = (int)value3.X;
			WorldGen.dungeonY = (int)value3.Y;
			if (vector.Y == 0f)
			{
				WorldGen.DDoorX[WorldGen.numDDoors] = (int)value3.X;
				WorldGen.DDoorY[WorldGen.numDDoors] = (int)value3.Y;
				WorldGen.DDoorPos[WorldGen.numDDoors] = 0;
				WorldGen.numDDoors++;
				return;
			}
			WorldGen.DPlatX[WorldGen.numDPlats] = (int)value3.X;
			WorldGen.DPlatY[WorldGen.numDPlats] = (int)value3.Y;
			WorldGen.numDPlats++;
		}
		public static void DungeonRoom(int i, int j, int tileType, int wallType)
		{
			double num = (double)WorldGen.genRand.Next(15, 30);
			Vector2 value;
			value.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
			value.Y = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
			Vector2 value2;
			value2.X = (float)i;
			value2.Y = (float)j - (float)num / 2f;
			int k = WorldGen.genRand.Next(10, 20);
			double num2 = (double)value2.X;
			double num3 = (double)value2.X;
			double num4 = (double)value2.Y;
			double num5 = (double)value2.Y;
			while (k > 0)
			{
				k--;
				int num6 = (int)((double)value2.X - num * 0.800000011920929 - 5.0);
				int num7 = (int)((double)value2.X + num * 0.800000011920929 + 5.0);
				int num8 = (int)((double)value2.Y - num * 0.800000011920929 - 5.0);
				int num9 = (int)((double)value2.Y + num * 0.800000011920929 + 5.0);
				if (num6 < 0)
				{
					num6 = 0;
				}
				if (num7 > Main.maxTilesX)
				{
					num7 = Main.maxTilesX;
				}
				if (num8 < 0)
				{
					num8 = 0;
				}
				if (num9 > Main.maxTilesY)
				{
					num9 = Main.maxTilesY;
				}
				for (int l = num6; l < num7; l++)
				{
					for (int m = num8; m < num9; m++)
					{
						Main.tile[l, m].liquid = 0;
						if (Main.tile[l, m].wall == 0)
						{
							Main.tile[l, m].active = true;
							Main.tile[l, m].type = (byte)tileType;
						}
					}
				}
				for (int n = num6 + 1; n < num7 - 1; n++)
				{
					for (int num10 = num8 + 1; num10 < num9 - 1; num10++)
					{
						WorldGen.PlaceWall(n, num10, wallType, true);
					}
				}
				num6 = (int)((double)value2.X - num * 0.5);
				num7 = (int)((double)value2.X + num * 0.5);
				num8 = (int)((double)value2.Y - num * 0.5);
				num9 = (int)((double)value2.Y + num * 0.5);
				if (num6 < 0)
				{
					num6 = 0;
				}
				if (num7 > Main.maxTilesX)
				{
					num7 = Main.maxTilesX;
				}
				if (num8 < 0)
				{
					num8 = 0;
				}
				if (num9 > Main.maxTilesY)
				{
					num9 = Main.maxTilesY;
				}
				if ((double)num6 < num2)
				{
					num2 = (double)num6;
				}
				if ((double)num7 > num3)
				{
					num3 = (double)num7;
				}
				if ((double)num8 < num4)
				{
					num4 = (double)num8;
				}
				if ((double)num9 > num5)
				{
					num5 = (double)num9;
				}
				for (int num11 = num6; num11 < num7; num11++)
				{
					for (int num12 = num8; num12 < num9; num12++)
					{
						Main.tile[num11, num12].active = false;
						Main.tile[num11, num12].wall = (byte)wallType;
					}
				}
				value2 += value;
				value.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
				value.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
				if (value.X > 1f)
				{
					value.X = 1f;
				}
				if (value.X < -1f)
				{
					value.X = -1f;
				}
				if (value.Y > 1f)
				{
					value.Y = 1f;
				}
				if (value.Y < -1f)
				{
					value.Y = -1f;
				}
			}
			WorldGen.dRoomX[WorldGen.numDRooms] = (int)value2.X;
			WorldGen.dRoomY[WorldGen.numDRooms] = (int)value2.Y;
			WorldGen.dRoomSize[WorldGen.numDRooms] = (int)num;
			WorldGen.dRoomL[WorldGen.numDRooms] = (int)num2;
			WorldGen.dRoomR[WorldGen.numDRooms] = (int)num3;
			WorldGen.dRoomT[WorldGen.numDRooms] = (int)num4;
			WorldGen.dRoomB[WorldGen.numDRooms] = (int)num5;
			WorldGen.dRoomTreasure[WorldGen.numDRooms] = false;
			WorldGen.numDRooms++;
		}
		public static void DungeonEnt(int i, int j, int tileType, int wallType)
		{
			int num = 60;
			for (int k = i - num; k < i + num; k++)
			{
				for (int l = j - num; l < j + num; l++)
				{
					Main.tile[k, l].liquid = 0;
					Main.tile[k, l].lava = false;
				}
			}
			double num2 = WorldGen.dxStrength1;
			double num3 = WorldGen.dyStrength1;
			Vector2 vector;
			vector.X = (float)i;
			vector.Y = (float)j - (float)num3 / 2f;
			WorldGen.dMinY = (int)vector.Y;
			int num4 = 1;
			if (i > Main.maxTilesX / 2)
			{
				num4 = -1;
			}
			int num5 = (int)((double)vector.X - num2 * 0.60000002384185791 - (double)WorldGen.genRand.Next(2, 5));
			int num6 = (int)((double)vector.X + num2 * 0.60000002384185791 + (double)WorldGen.genRand.Next(2, 5));
			int num7 = (int)((double)vector.Y - num3 * 0.60000002384185791 - (double)WorldGen.genRand.Next(2, 5));
			int num8 = (int)((double)vector.Y + num3 * 0.60000002384185791 + (double)WorldGen.genRand.Next(8, 16));
			if (num5 < 0)
			{
				num5 = 0;
			}
			if (num6 > Main.maxTilesX)
			{
				num6 = Main.maxTilesX;
			}
			if (num7 < 0)
			{
				num7 = 0;
			}
			if (num8 > Main.maxTilesY)
			{
				num8 = Main.maxTilesY;
			}
			for (int m = num5; m < num6; m++)
			{
				for (int n = num7; n < num8; n++)
				{
					Main.tile[m, n].liquid = 0;
					if ((int)Main.tile[m, n].wall != wallType)
					{
						Main.tile[m, n].wall = 0;
						if (m > num5 + 1 && m < num6 - 2 && n > num7 + 1 && n < num8 - 2)
						{
							WorldGen.PlaceWall(m, n, wallType, true);
						}
						Main.tile[m, n].active = true;
						Main.tile[m, n].type = (byte)tileType;
					}
				}
			}
			int num9 = num5;
			int num10 = num5 + 5 + WorldGen.genRand.Next(4);
			int num11 = num7 - 3 - WorldGen.genRand.Next(3);
			int num12 = num7;
			for (int num13 = num9; num13 < num10; num13++)
			{
				for (int num14 = num11; num14 < num12; num14++)
				{
					if ((int)Main.tile[num13, num14].wall != wallType)
					{
						Main.tile[num13, num14].active = true;
						Main.tile[num13, num14].type = (byte)tileType;
					}
				}
			}
			num9 = num6 - 5 - WorldGen.genRand.Next(4);
			num10 = num6;
			num11 = num7 - 3 - WorldGen.genRand.Next(3);
			num12 = num7;
			for (int num15 = num9; num15 < num10; num15++)
			{
				for (int num16 = num11; num16 < num12; num16++)
				{
					if ((int)Main.tile[num15, num16].wall != wallType)
					{
						Main.tile[num15, num16].active = true;
						Main.tile[num15, num16].type = (byte)tileType;
					}
				}
			}
			int num17 = 1 + WorldGen.genRand.Next(2);
			int num18 = 2 + WorldGen.genRand.Next(4);
			int num19 = 0;
			for (int num20 = num5; num20 < num6; num20++)
			{
				for (int num21 = num7 - num17; num21 < num7; num21++)
				{
					if ((int)Main.tile[num20, num21].wall != wallType)
					{
						Main.tile[num20, num21].active = true;
						Main.tile[num20, num21].type = (byte)tileType;
					}
				}
				num19++;
				if (num19 >= num18)
				{
					num20 += num18;
					num19 = 0;
				}
			}
			for (int num22 = num5; num22 < num6; num22++)
			{
				for (int num23 = num8; num23 < num8 + 100; num23++)
				{
					WorldGen.PlaceWall(num22, num23, 2, true);
				}
			}
			num5 = (int)((double)vector.X - num2 * 0.60000002384185791);
			num6 = (int)((double)vector.X + num2 * 0.60000002384185791);
			num7 = (int)((double)vector.Y - num3 * 0.60000002384185791);
			num8 = (int)((double)vector.Y + num3 * 0.60000002384185791);
			if (num5 < 0)
			{
				num5 = 0;
			}
			if (num6 > Main.maxTilesX)
			{
				num6 = Main.maxTilesX;
			}
			if (num7 < 0)
			{
				num7 = 0;
			}
			if (num8 > Main.maxTilesY)
			{
				num8 = Main.maxTilesY;
			}
			for (int num24 = num5; num24 < num6; num24++)
			{
				for (int num25 = num7; num25 < num8; num25++)
				{
					WorldGen.PlaceWall(num24, num25, wallType, true);
				}
			}
			num5 = (int)((double)vector.X - num2 * 0.6 - 1.0);
			num6 = (int)((double)vector.X + num2 * 0.6 + 1.0);
			num7 = (int)((double)vector.Y - num3 * 0.6 - 1.0);
			num8 = (int)((double)vector.Y + num3 * 0.6 + 1.0);
			if (num5 < 0)
			{
				num5 = 0;
			}
			if (num6 > Main.maxTilesX)
			{
				num6 = Main.maxTilesX;
			}
			if (num7 < 0)
			{
				num7 = 0;
			}
			if (num8 > Main.maxTilesY)
			{
				num8 = Main.maxTilesY;
			}
			for (int num26 = num5; num26 < num6; num26++)
			{
				for (int num27 = num7; num27 < num8; num27++)
				{
					Main.tile[num26, num27].wall = (byte)wallType;
				}
			}
			num5 = (int)((double)vector.X - num2 * 0.5);
			num6 = (int)((double)vector.X + num2 * 0.5);
			num7 = (int)((double)vector.Y - num3 * 0.5);
			num8 = (int)((double)vector.Y + num3 * 0.5);
			if (num5 < 0)
			{
				num5 = 0;
			}
			if (num6 > Main.maxTilesX)
			{
				num6 = Main.maxTilesX;
			}
			if (num7 < 0)
			{
				num7 = 0;
			}
			if (num8 > Main.maxTilesY)
			{
				num8 = Main.maxTilesY;
			}
			for (int num28 = num5; num28 < num6; num28++)
			{
				for (int num29 = num7; num29 < num8; num29++)
				{
					Main.tile[num28, num29].active = false;
					Main.tile[num28, num29].wall = (byte)wallType;
				}
			}
			WorldGen.DPlatX[WorldGen.numDPlats] = (int)vector.X;
			WorldGen.DPlatY[WorldGen.numDPlats] = num8;
			WorldGen.numDPlats++;
			vector.X += (float)num2 * 0.6f * (float)num4;
			vector.Y += (float)num3 * 0.5f;
			num2 = WorldGen.dxStrength2;
			num3 = WorldGen.dyStrength2;
			vector.X += (float)num2 * 0.55f * (float)num4;
			vector.Y -= (float)num3 * 0.5f;
			num5 = (int)((double)vector.X - num2 * 0.60000002384185791 - (double)WorldGen.genRand.Next(1, 3));
			num6 = (int)((double)vector.X + num2 * 0.60000002384185791 + (double)WorldGen.genRand.Next(1, 3));
			num7 = (int)((double)vector.Y - num3 * 0.60000002384185791 - (double)WorldGen.genRand.Next(1, 3));
			num8 = (int)((double)vector.Y + num3 * 0.60000002384185791 + (double)WorldGen.genRand.Next(6, 16));
			if (num5 < 0)
			{
				num5 = 0;
			}
			if (num6 > Main.maxTilesX)
			{
				num6 = Main.maxTilesX;
			}
			if (num7 < 0)
			{
				num7 = 0;
			}
			if (num8 > Main.maxTilesY)
			{
				num8 = Main.maxTilesY;
			}
			for (int num30 = num5; num30 < num6; num30++)
			{
				for (int num31 = num7; num31 < num8; num31++)
				{
					if ((int)Main.tile[num30, num31].wall != wallType)
					{
						bool flag = true;
						if (num4 < 0)
						{
							if ((double)num30 < (double)vector.X - num2 * 0.5)
							{
								flag = false;
							}
						}
						else
						{
							if ((double)num30 > (double)vector.X + num2 * 0.5 - 1.0)
							{
								flag = false;
							}
						}
						if (flag)
						{
							Main.tile[num30, num31].wall = 0;
							Main.tile[num30, num31].active = true;
							Main.tile[num30, num31].type = (byte)tileType;
						}
					}
				}
			}
			for (int num32 = num5; num32 < num6; num32++)
			{
				for (int num33 = num8; num33 < num8 + 100; num33++)
				{
					WorldGen.PlaceWall(num32, num33, 2, true);
				}
			}
			num5 = (int)((double)vector.X - num2 * 0.5);
			num6 = (int)((double)vector.X + num2 * 0.5);
			num9 = num5;
			if (num4 < 0)
			{
				num9++;
			}
			num10 = num9 + 5 + WorldGen.genRand.Next(4);
			num11 = num7 - 3 - WorldGen.genRand.Next(3);
			num12 = num7;
			for (int num34 = num9; num34 < num10; num34++)
			{
				for (int num35 = num11; num35 < num12; num35++)
				{
					if ((int)Main.tile[num34, num35].wall != wallType)
					{
						Main.tile[num34, num35].active = true;
						Main.tile[num34, num35].type = (byte)tileType;
					}
				}
			}
			num9 = num6 - 5 - WorldGen.genRand.Next(4);
			num10 = num6;
			num11 = num7 - 3 - WorldGen.genRand.Next(3);
			num12 = num7;
			for (int num36 = num9; num36 < num10; num36++)
			{
				for (int num37 = num11; num37 < num12; num37++)
				{
					if ((int)Main.tile[num36, num37].wall != wallType)
					{
						Main.tile[num36, num37].active = true;
						Main.tile[num36, num37].type = (byte)tileType;
					}
				}
			}
			num17 = 1 + WorldGen.genRand.Next(2);
			num18 = 2 + WorldGen.genRand.Next(4);
			num19 = 0;
			if (num4 < 0)
			{
				num6++;
			}
			for (int num38 = num5 + 1; num38 < num6 - 1; num38++)
			{
				for (int num39 = num7 - num17; num39 < num7; num39++)
				{
					if ((int)Main.tile[num38, num39].wall != wallType)
					{
						Main.tile[num38, num39].active = true;
						Main.tile[num38, num39].type = (byte)tileType;
					}
				}
				num19++;
				if (num19 >= num18)
				{
					num38 += num18;
					num19 = 0;
				}
			}
			num5 = (int)((double)vector.X - num2 * 0.6);
			num6 = (int)((double)vector.X + num2 * 0.6);
			num7 = (int)((double)vector.Y - num3 * 0.6);
			num8 = (int)((double)vector.Y + num3 * 0.6);
			if (num5 < 0)
			{
				num5 = 0;
			}
			if (num6 > Main.maxTilesX)
			{
				num6 = Main.maxTilesX;
			}
			if (num7 < 0)
			{
				num7 = 0;
			}
			if (num8 > Main.maxTilesY)
			{
				num8 = Main.maxTilesY;
			}
			for (int num40 = num5; num40 < num6; num40++)
			{
				for (int num41 = num7; num41 < num8; num41++)
				{
					Main.tile[num40, num41].wall = 0;
				}
			}
			num5 = (int)((double)vector.X - num2 * 0.5);
			num6 = (int)((double)vector.X + num2 * 0.5);
			num7 = (int)((double)vector.Y - num3 * 0.5);
			num8 = (int)((double)vector.Y + num3 * 0.5);
			if (num5 < 0)
			{
				num5 = 0;
			}
			if (num6 > Main.maxTilesX)
			{
				num6 = Main.maxTilesX;
			}
			if (num7 < 0)
			{
				num7 = 0;
			}
			if (num8 > Main.maxTilesY)
			{
				num8 = Main.maxTilesY;
			}
			for (int num42 = num5; num42 < num6; num42++)
			{
				for (int num43 = num7; num43 < num8; num43++)
				{
					Main.tile[num42, num43].active = false;
					Main.tile[num42, num43].wall = 0;
				}
			}
			for (int num44 = num5; num44 < num6; num44++)
			{
				if (!Main.tile[num44, num8].active)
				{
					Main.tile[num44, num8].active = true;
					Main.tile[num44, num8].type = 19;
				}
			}
			Main.dungeonX = (int)vector.X;
			Main.dungeonY = num8;
			int num45 = NPC.NewNPC(Main.dungeonX * 16 + 8, Main.dungeonY * 16, 37, 0);
			Main.npc[num45].homeless = false;
			Main.npc[num45].homeTileX = Main.dungeonX;
			Main.npc[num45].homeTileY = Main.dungeonY;
			if (num4 == 1)
			{
				int num46 = 0;
				for (int num47 = num6; num47 < num6 + 25; num47++)
				{
					num46++;
					for (int num48 = num8 + num46; num48 < num8 + 25; num48++)
					{
						Main.tile[num47, num48].active = true;
						Main.tile[num47, num48].type = (byte)tileType;
					}
				}
			}
			else
			{
				int num49 = 0;
				for (int num50 = num5; num50 > num5 - 25; num50--)
				{
					num49++;
					for (int num51 = num8 + num49; num51 < num8 + 25; num51++)
					{
						Main.tile[num50, num51].active = true;
						Main.tile[num50, num51].type = (byte)tileType;
					}
				}
			}
			num17 = 1 + WorldGen.genRand.Next(2);
			num18 = 2 + WorldGen.genRand.Next(4);
			num19 = 0;
			num5 = (int)((double)vector.X - num2 * 0.5);
			num6 = (int)((double)vector.X + num2 * 0.5);
			num5 += 2;
			num6 -= 2;
			for (int num52 = num5; num52 < num6; num52++)
			{
				for (int num53 = num7; num53 < num8; num53++)
				{
					WorldGen.PlaceWall(num52, num53, wallType, true);
				}
				num19++;
				if (num19 >= num18)
				{
					num52 += num18 * 2;
					num19 = 0;
				}
			}
			vector.X -= (float)num2 * 0.6f * (float)num4;
			vector.Y += (float)num3 * 0.5f;
			num2 = 15.0;
			num3 = 3.0;
			vector.Y -= (float)num3 * 0.5f;
			num5 = (int)((double)vector.X - num2 * 0.5);
			num6 = (int)((double)vector.X + num2 * 0.5);
			num7 = (int)((double)vector.Y - num3 * 0.5);
			num8 = (int)((double)vector.Y + num3 * 0.5);
			if (num5 < 0)
			{
				num5 = 0;
			}
			if (num6 > Main.maxTilesX)
			{
				num6 = Main.maxTilesX;
			}
			if (num7 < 0)
			{
				num7 = 0;
			}
			if (num8 > Main.maxTilesY)
			{
				num8 = Main.maxTilesY;
			}
			for (int num54 = num5; num54 < num6; num54++)
			{
				for (int num55 = num7; num55 < num8; num55++)
				{
					Main.tile[num54, num55].active = false;
				}
			}
			if (num4 < 0)
			{
				vector.X -= 1f;
			}
			WorldGen.PlaceTile((int)vector.X, (int)vector.Y + 1, 10, false, false, -1, 0);
		}
		public static bool AddBuriedChest(int i, int j, int contain = 0, bool notNearOtherChests = false, int Style = -1)
		{
			if (WorldGen.genRand == null)
			{
				WorldGen.genRand = new Random((int)DateTime.Now.Ticks);
			}
			int k = j;
			while (k < Main.maxTilesY)
			{
				if (Main.tile[i, k].active && Main.tileSolid[(int)Main.tile[i, k].type])
				{
					bool flag = false;
					int num = k;
					int style = 0;
					if ((double)num >= Main.worldSurface + 25.0 || contain > 0)
					{
						style = 1;
					}
					if (Style >= 0)
					{
						style = Style;
					}
					if (num > Main.maxTilesY - 205 && contain == 0)
					{
						if (WorldGen.hellChest == 0)
						{
							contain = 274;
							style = 4;
							flag = true;
						}
						else
						{
							if (WorldGen.hellChest == 1)
							{
								contain = 220;
								style = 4;
								flag = true;
							}
							else
							{
								if (WorldGen.hellChest == 2)
								{
									contain = 112;
									style = 4;
									flag = true;
								}
								else
								{
									if (WorldGen.hellChest == 3)
									{
										contain = 218;
										style = 4;
										flag = true;
										WorldGen.hellChest = 0;
									}
								}
							}
						}
					}
					int num2 = WorldGen.PlaceChest(i - 1, num - 1, 21, notNearOtherChests, style);
					if (num2 >= 0)
					{
						if (flag)
						{
							WorldGen.hellChest++;
						}
						int num3 = 0;
						while (num3 == 0)
						{
							if ((double)num < Main.worldSurface + 25.0)
							{
								if (contain > 0)
								{
									Main.chest[num2].item[num3].SetDefaults(contain, false);
									Main.chest[num2].item[num3].Prefix(-1);
									num3++;
								}
								else
								{
									int num4 = WorldGen.genRand.Next(6);
									if (num4 == 0)
									{
										Main.chest[num2].item[num3].SetDefaults(280, false);
										Main.chest[num2].item[num3].Prefix(-1);
									}
									if (num4 == 1)
									{
										Main.chest[num2].item[num3].SetDefaults(281, false);
										Main.chest[num2].item[num3].Prefix(-1);
									}
									if (num4 == 2)
									{
										Main.chest[num2].item[num3].SetDefaults(284, false);
										Main.chest[num2].item[num3].Prefix(-1);
									}
									if (num4 == 3)
									{
										Main.chest[num2].item[num3].SetDefaults(282, false);
										Main.chest[num2].item[num3].stack = WorldGen.genRand.Next(50, 75);
									}
									if (num4 == 4)
									{
										Main.chest[num2].item[num3].SetDefaults(279, false);
										Main.chest[num2].item[num3].stack = WorldGen.genRand.Next(25, 50);
									}
									if (num4 == 5)
									{
										Main.chest[num2].item[num3].SetDefaults(285, false);
										Main.chest[num2].item[num3].Prefix(-1);
									}
									num3++;
								}
								if (WorldGen.genRand.Next(3) == 0)
								{
									Main.chest[num2].item[num3].SetDefaults(168, false);
									Main.chest[num2].item[num3].stack = WorldGen.genRand.Next(3, 6);
									num3++;
								}
								if (WorldGen.genRand.Next(2) == 0)
								{
									int num5 = WorldGen.genRand.Next(2);
									int stack = WorldGen.genRand.Next(8) + 3;
									if (num5 == 0)
									{
										Main.chest[num2].item[num3].SetDefaults(20, false);
									}
									if (num5 == 1)
									{
										Main.chest[num2].item[num3].SetDefaults(22, false);
									}
									Main.chest[num2].item[num3].stack = stack;
									num3++;
								}
								if (WorldGen.genRand.Next(2) == 0)
								{
									int num6 = WorldGen.genRand.Next(2);
									int stack2 = WorldGen.genRand.Next(26) + 25;
									if (num6 == 0)
									{
										Main.chest[num2].item[num3].SetDefaults(40, false);
									}
									if (num6 == 1)
									{
										Main.chest[num2].item[num3].SetDefaults(42, false);
									}
									Main.chest[num2].item[num3].stack = stack2;
									num3++;
								}
								if (WorldGen.genRand.Next(2) == 0)
								{
									int num7 = WorldGen.genRand.Next(1);
									int stack3 = WorldGen.genRand.Next(3) + 3;
									if (num7 == 0)
									{
										Main.chest[num2].item[num3].SetDefaults(28, false);
									}
									Main.chest[num2].item[num3].stack = stack3;
									num3++;
								}
								if (WorldGen.genRand.Next(3) > 0)
								{
									int num8 = WorldGen.genRand.Next(4);
									int stack4 = WorldGen.genRand.Next(1, 3);
									if (num8 == 0)
									{
										Main.chest[num2].item[num3].SetDefaults(292, false);
									}
									if (num8 == 1)
									{
										Main.chest[num2].item[num3].SetDefaults(298, false);
									}
									if (num8 == 2)
									{
										Main.chest[num2].item[num3].SetDefaults(299, false);
									}
									if (num8 == 3)
									{
										Main.chest[num2].item[num3].SetDefaults(290, false);
									}
									Main.chest[num2].item[num3].stack = stack4;
									num3++;
								}
								if (WorldGen.genRand.Next(2) == 0)
								{
									int num9 = WorldGen.genRand.Next(2);
									int stack5 = WorldGen.genRand.Next(11) + 10;
									if (num9 == 0)
									{
										Main.chest[num2].item[num3].SetDefaults(8, false);
									}
									if (num9 == 1)
									{
										Main.chest[num2].item[num3].SetDefaults(31, false);
									}
									Main.chest[num2].item[num3].stack = stack5;
									num3++;
								}
								if (WorldGen.genRand.Next(2) == 0)
								{
									Main.chest[num2].item[num3].SetDefaults(72, false);
									Main.chest[num2].item[num3].stack = WorldGen.genRand.Next(10, 30);
									num3++;
								}
							}
							else
							{
								if ((double)num < Main.rockLayer)
								{
									if (contain > 0)
									{
										Main.chest[num2].item[num3].SetDefaults(contain, false);
										Main.chest[num2].item[num3].Prefix(-1);
										num3++;
									}
									else
									{
										int num10 = WorldGen.genRand.Next(7);
										if (num10 == 0)
										{
											Main.chest[num2].item[num3].SetDefaults(49, false);
											Main.chest[num2].item[num3].Prefix(-1);
										}
										if (num10 == 1)
										{
											Main.chest[num2].item[num3].SetDefaults(50, false);
											Main.chest[num2].item[num3].Prefix(-1);
										}
										if (num10 == 2)
										{
											Main.chest[num2].item[num3].SetDefaults(52, false);
										}
										if (num10 == 3)
										{
											Main.chest[num2].item[num3].SetDefaults(53, false);
											Main.chest[num2].item[num3].Prefix(-1);
										}
										if (num10 == 4)
										{
											Main.chest[num2].item[num3].SetDefaults(54, false);
											Main.chest[num2].item[num3].Prefix(-1);
										}
										if (num10 == 5)
										{
											Main.chest[num2].item[num3].SetDefaults(55, false);
											Main.chest[num2].item[num3].Prefix(-1);
										}
										if (num10 == 6)
										{
											Main.chest[num2].item[num3].SetDefaults(51, false);
											Main.chest[num2].item[num3].stack = WorldGen.genRand.Next(26) + 25;
										}
										num3++;
									}
									if (WorldGen.genRand.Next(3) == 0)
									{
										Main.chest[num2].item[num3].SetDefaults(166, false);
										Main.chest[num2].item[num3].stack = WorldGen.genRand.Next(10, 20);
										num3++;
									}
									if (WorldGen.genRand.Next(2) == 0)
									{
										int num11 = WorldGen.genRand.Next(2);
										int stack6 = WorldGen.genRand.Next(10) + 5;
										if (num11 == 0)
										{
											Main.chest[num2].item[num3].SetDefaults(22, false);
										}
										if (num11 == 1)
										{
											Main.chest[num2].item[num3].SetDefaults(21, false);
										}
										Main.chest[num2].item[num3].stack = stack6;
										num3++;
									}
									if (WorldGen.genRand.Next(2) == 0)
									{
										int num12 = WorldGen.genRand.Next(2);
										int stack7 = WorldGen.genRand.Next(25) + 25;
										if (num12 == 0)
										{
											Main.chest[num2].item[num3].SetDefaults(40, false);
										}
										if (num12 == 1)
										{
											Main.chest[num2].item[num3].SetDefaults(42, false);
										}
										Main.chest[num2].item[num3].stack = stack7;
										num3++;
									}
									if (WorldGen.genRand.Next(2) == 0)
									{
										int num13 = WorldGen.genRand.Next(1);
										int stack8 = WorldGen.genRand.Next(3) + 3;
										if (num13 == 0)
										{
											Main.chest[num2].item[num3].SetDefaults(28, false);
										}
										Main.chest[num2].item[num3].stack = stack8;
										num3++;
									}
									if (WorldGen.genRand.Next(3) > 0)
									{
										int num14 = WorldGen.genRand.Next(7);
										int stack9 = WorldGen.genRand.Next(1, 3);
										if (num14 == 0)
										{
											Main.chest[num2].item[num3].SetDefaults(289, false);
										}
										if (num14 == 1)
										{
											Main.chest[num2].item[num3].SetDefaults(298, false);
										}
										if (num14 == 2)
										{
											Main.chest[num2].item[num3].SetDefaults(299, false);
										}
										if (num14 == 3)
										{
											Main.chest[num2].item[num3].SetDefaults(290, false);
										}
										if (num14 == 4)
										{
											Main.chest[num2].item[num3].SetDefaults(303, false);
										}
										if (num14 == 5)
										{
											Main.chest[num2].item[num3].SetDefaults(291, false);
										}
										if (num14 == 6)
										{
											Main.chest[num2].item[num3].SetDefaults(304, false);
										}
										Main.chest[num2].item[num3].stack = stack9;
										num3++;
									}
									if (WorldGen.genRand.Next(2) == 0)
									{
										int stack10 = WorldGen.genRand.Next(11) + 10;
										Main.chest[num2].item[num3].SetDefaults(8, false);
										Main.chest[num2].item[num3].stack = stack10;
										num3++;
									}
									if (WorldGen.genRand.Next(2) == 0)
									{
										Main.chest[num2].item[num3].SetDefaults(72, false);
										Main.chest[num2].item[num3].stack = WorldGen.genRand.Next(50, 90);
										num3++;
									}
								}
								else
								{
									if (num < Main.maxTilesY - 250)
									{
										if (contain > 0)
										{
											Main.chest[num2].item[num3].SetDefaults(contain, false);
											Main.chest[num2].item[num3].Prefix(-1);
											num3++;
										}
										else
										{
											int num15 = WorldGen.genRand.Next(7);
											if (num15 == 2 && WorldGen.genRand.Next(2) == 0)
											{
												num15 = WorldGen.genRand.Next(7);
											}
											if (num15 == 0)
											{
												Main.chest[num2].item[num3].SetDefaults(49, false);
												Main.chest[num2].item[num3].Prefix(-1);
											}
											if (num15 == 1)
											{
												Main.chest[num2].item[num3].SetDefaults(50, false);
												Main.chest[num2].item[num3].Prefix(-1);
											}
											if (num15 == 2)
											{
												Main.chest[num2].item[num3].SetDefaults(52, false);
												Main.chest[num2].item[num3].Prefix(-1);
											}
											if (num15 == 3)
											{
												Main.chest[num2].item[num3].SetDefaults(53, false);
												Main.chest[num2].item[num3].Prefix(-1);
											}
											if (num15 == 4)
											{
												Main.chest[num2].item[num3].SetDefaults(54, false);
												Main.chest[num2].item[num3].Prefix(-1);
											}
											if (num15 == 5)
											{
												Main.chest[num2].item[num3].SetDefaults(55, false);
												Main.chest[num2].item[num3].Prefix(-1);
											}
											if (num15 == 6)
											{
												Main.chest[num2].item[num3].SetDefaults(51, false);
												Main.chest[num2].item[num3].stack = WorldGen.genRand.Next(26) + 25;
											}
											num3++;
										}
										if (WorldGen.genRand.Next(5) == 0)
										{
											Main.chest[num2].item[num3].SetDefaults(43, false);
											num3++;
										}
										if (WorldGen.genRand.Next(3) == 0)
										{
											Main.chest[num2].item[num3].SetDefaults(167, false);
											num3++;
										}
										if (WorldGen.genRand.Next(2) == 0)
										{
											int num16 = WorldGen.genRand.Next(2);
											int stack11 = WorldGen.genRand.Next(8) + 3;
											if (num16 == 0)
											{
												Main.chest[num2].item[num3].SetDefaults(19, false);
											}
											if (num16 == 1)
											{
												Main.chest[num2].item[num3].SetDefaults(21, false);
											}
											Main.chest[num2].item[num3].stack = stack11;
											num3++;
										}
										if (WorldGen.genRand.Next(2) == 0)
										{
											int num17 = WorldGen.genRand.Next(2);
											int stack12 = WorldGen.genRand.Next(26) + 25;
											if (num17 == 0)
											{
												Main.chest[num2].item[num3].SetDefaults(41, false);
											}
											if (num17 == 1)
											{
												Main.chest[num2].item[num3].SetDefaults(279, false);
											}
											Main.chest[num2].item[num3].stack = stack12;
											num3++;
										}
										if (WorldGen.genRand.Next(2) == 0)
										{
											int num18 = WorldGen.genRand.Next(1);
											int stack13 = WorldGen.genRand.Next(3) + 3;
											if (num18 == 0)
											{
												Main.chest[num2].item[num3].SetDefaults(188, false);
											}
											Main.chest[num2].item[num3].stack = stack13;
											num3++;
										}
										if (WorldGen.genRand.Next(3) > 0)
										{
											int num19 = WorldGen.genRand.Next(6);
											int stack14 = WorldGen.genRand.Next(1, 3);
											if (num19 == 0)
											{
												Main.chest[num2].item[num3].SetDefaults(296, false);
											}
											if (num19 == 1)
											{
												Main.chest[num2].item[num3].SetDefaults(295, false);
											}
											if (num19 == 2)
											{
												Main.chest[num2].item[num3].SetDefaults(299, false);
											}
											if (num19 == 3)
											{
												Main.chest[num2].item[num3].SetDefaults(302, false);
											}
											if (num19 == 4)
											{
												Main.chest[num2].item[num3].SetDefaults(303, false);
											}
											if (num19 == 5)
											{
												Main.chest[num2].item[num3].SetDefaults(305, false);
											}
											Main.chest[num2].item[num3].stack = stack14;
											num3++;
										}
										if (WorldGen.genRand.Next(3) > 1)
										{
											int num20 = WorldGen.genRand.Next(4);
											int stack15 = WorldGen.genRand.Next(1, 3);
											if (num20 == 0)
											{
												Main.chest[num2].item[num3].SetDefaults(301, false);
											}
											if (num20 == 1)
											{
												Main.chest[num2].item[num3].SetDefaults(302, false);
											}
											if (num20 == 2)
											{
												Main.chest[num2].item[num3].SetDefaults(297, false);
											}
											if (num20 == 3)
											{
												Main.chest[num2].item[num3].SetDefaults(304, false);
											}
											Main.chest[num2].item[num3].stack = stack15;
											num3++;
										}
										if (WorldGen.genRand.Next(2) == 0)
										{
											int num21 = WorldGen.genRand.Next(2);
											int stack16 = WorldGen.genRand.Next(15) + 15;
											if (num21 == 0)
											{
												Main.chest[num2].item[num3].SetDefaults(8, false);
											}
											if (num21 == 1)
											{
												Main.chest[num2].item[num3].SetDefaults(282, false);
											}
											Main.chest[num2].item[num3].stack = stack16;
											num3++;
										}
										if (WorldGen.genRand.Next(2) == 0)
										{
											Main.chest[num2].item[num3].SetDefaults(73, false);
											Main.chest[num2].item[num3].stack = WorldGen.genRand.Next(1, 3);
											num3++;
										}
									}
									else
									{
										if (contain > 0)
										{
											Main.chest[num2].item[num3].SetDefaults(contain, false);
											Main.chest[num2].item[num3].Prefix(-1);
											num3++;
										}
										else
										{
											int num22 = WorldGen.genRand.Next(4);
											if (num22 == 0)
											{
												Main.chest[num2].item[num3].SetDefaults(49, false);
												Main.chest[num2].item[num3].Prefix(-1);
											}
											if (num22 == 1)
											{
												Main.chest[num2].item[num3].SetDefaults(50, false);
												Main.chest[num2].item[num3].Prefix(-1);
											}
											if (num22 == 2)
											{
												Main.chest[num2].item[num3].SetDefaults(53, false);
												Main.chest[num2].item[num3].Prefix(-1);
											}
											if (num22 == 3)
											{
												Main.chest[num2].item[num3].SetDefaults(54, false);
												Main.chest[num2].item[num3].Prefix(-1);
											}
											num3++;
										}
										if (WorldGen.genRand.Next(3) == 0)
										{
											Main.chest[num2].item[num3].SetDefaults(167, false);
											num3++;
										}
										if (WorldGen.genRand.Next(2) == 0)
										{
											int num23 = WorldGen.genRand.Next(2);
											int stack17 = WorldGen.genRand.Next(15) + 15;
											if (num23 == 0)
											{
												Main.chest[num2].item[num3].SetDefaults(117, false);
											}
											if (num23 == 1)
											{
												Main.chest[num2].item[num3].SetDefaults(19, false);
											}
											Main.chest[num2].item[num3].stack = stack17;
											num3++;
										}
										if (WorldGen.genRand.Next(2) == 0)
										{
											int num24 = WorldGen.genRand.Next(2);
											int stack18 = WorldGen.genRand.Next(25) + 50;
											if (num24 == 0)
											{
												Main.chest[num2].item[num3].SetDefaults(265, false);
											}
											if (num24 == 1)
											{
												Main.chest[num2].item[num3].SetDefaults(278, false);
											}
											Main.chest[num2].item[num3].stack = stack18;
											num3++;
										}
										if (WorldGen.genRand.Next(2) == 0)
										{
											int num25 = WorldGen.genRand.Next(2);
											int stack19 = WorldGen.genRand.Next(15) + 15;
											if (num25 == 0)
											{
												Main.chest[num2].item[num3].SetDefaults(226, false);
											}
											if (num25 == 1)
											{
												Main.chest[num2].item[num3].SetDefaults(227, false);
											}
											Main.chest[num2].item[num3].stack = stack19;
											num3++;
										}
										if (WorldGen.genRand.Next(4) > 0)
										{
											int num26 = WorldGen.genRand.Next(7);
											int stack20 = WorldGen.genRand.Next(1, 3);
											if (num26 == 0)
											{
												Main.chest[num2].item[num3].SetDefaults(296, false);
											}
											if (num26 == 1)
											{
												Main.chest[num2].item[num3].SetDefaults(295, false);
											}
											if (num26 == 2)
											{
												Main.chest[num2].item[num3].SetDefaults(293, false);
											}
											if (num26 == 3)
											{
												Main.chest[num2].item[num3].SetDefaults(288, false);
											}
											if (num26 == 4)
											{
												Main.chest[num2].item[num3].SetDefaults(294, false);
											}
											if (num26 == 5)
											{
												Main.chest[num2].item[num3].SetDefaults(297, false);
											}
											if (num26 == 6)
											{
												Main.chest[num2].item[num3].SetDefaults(304, false);
											}
											Main.chest[num2].item[num3].stack = stack20;
											num3++;
										}
										if (WorldGen.genRand.Next(3) > 0)
										{
											int num27 = WorldGen.genRand.Next(5);
											int stack21 = WorldGen.genRand.Next(1, 3);
											if (num27 == 0)
											{
												Main.chest[num2].item[num3].SetDefaults(305, false);
											}
											if (num27 == 1)
											{
												Main.chest[num2].item[num3].SetDefaults(301, false);
											}
											if (num27 == 2)
											{
												Main.chest[num2].item[num3].SetDefaults(302, false);
											}
											if (num27 == 3)
											{
												Main.chest[num2].item[num3].SetDefaults(288, false);
											}
											if (num27 == 4)
											{
												Main.chest[num2].item[num3].SetDefaults(300, false);
											}
											Main.chest[num2].item[num3].stack = stack21;
											num3++;
										}
										if (WorldGen.genRand.Next(2) == 0)
										{
											int num28 = WorldGen.genRand.Next(2);
											int stack22 = WorldGen.genRand.Next(15) + 15;
											if (num28 == 0)
											{
												Main.chest[num2].item[num3].SetDefaults(8, false);
											}
											if (num28 == 1)
											{
												Main.chest[num2].item[num3].SetDefaults(282, false);
											}
											Main.chest[num2].item[num3].stack = stack22;
											num3++;
										}
										if (WorldGen.genRand.Next(2) == 0)
										{
											Main.chest[num2].item[num3].SetDefaults(73, false);
											Main.chest[num2].item[num3].stack = WorldGen.genRand.Next(2, 5);
											num3++;
										}
									}
								}
							}
						}
						return true;
					}
					return false;
				}
				else
				{
					k++;
				}
			}
			return false;
		}
		public static bool OpenDoor(int i, int j, int direction)
		{
			int num = 0;
			if (Main.tile[i, j - 1].frameY == 0 && Main.tile[i, j - 1].type == Main.tile[i, j].type)
			{
				num = j - 1;
			}
			else
			{
				if (Main.tile[i, j - 2].frameY == 0 && Main.tile[i, j - 2].type == Main.tile[i, j].type)
				{
					num = j - 2;
				}
				else
				{
					if (Main.tile[i, j + 1].frameY == 0 && Main.tile[i, j + 1].type == Main.tile[i, j].type)
					{
						num = j + 1;
					}
					else
					{
						num = j;
					}
				}
			}
			int num2 = i;
			short num3 = 0;
			int num4;
			if (direction == -1)
			{
				num2 = i - 1;
				num3 = 36;
				num4 = i - 1;
			}
			else
			{
				num2 = i;
				num4 = i + 1;
			}
			bool flag = true;
			for (int k = num; k < num + 3; k++)
			{

				if (Main.tile[num4, k].active)
				{
					if (!Main.tileCut[(int)Main.tile[num4, k].type] && Main.tile[num4, k].type != 3 && Main.tile[num4, k].type != 24 && Main.tile[num4, k].type != 52 && Main.tile[num4, k].type != 61 && Main.tile[num4, k].type != 62 && Main.tile[num4, k].type != 69 && Main.tile[num4, k].type != 71 && Main.tile[num4, k].type != 73 && Main.tile[num4, k].type != 74 && Main.tile[num4, k].type != 110 && Main.tile[num4, k].type != 113 && Main.tile[num4, k].type != 115)
					{
						flag = false;
						break;
					}
					WorldGen.KillTile(num4, k, false, false, false);
				}
			}
			if (flag)
			{
				if (Main.netMode != 1)
				{
					for (int l = num2; l <= num2 + 1; l++)
					{
						for (int m = num; m <= num + 2; m++)
						{
							if (WorldGen.numNoWire < WorldGen.maxWire - 1)
							{
								WorldGen.noWireX[WorldGen.numNoWire] = l;
								WorldGen.noWireY[WorldGen.numNoWire] = m;
								WorldGen.numNoWire++;
							}
						}
					}
				}
				Main.PlaySound(8, i * 16, j * 16, 1);
				Main.tile[num2, num].active = true;
				Main.tile[num2, num].type = 11;
				Main.tile[num2, num].frameY = 0;
				Main.tile[num2, num].frameX = num3;
				Main.tile[num2 + 1, num].active = true;
				Main.tile[num2 + 1, num].type = 11;
				Main.tile[num2 + 1, num].frameY = 0;
				Main.tile[num2 + 1, num].frameX = (short)(num3 + 18);
				Main.tile[num2, num + 1].active = true;
				Main.tile[num2, num + 1].type = 11;
				Main.tile[num2, num + 1].frameY = 18;
				Main.tile[num2, num + 1].frameX = num3;
				Main.tile[num2 + 1, num + 1].active = true;
				Main.tile[num2 + 1, num + 1].type = 11;
				Main.tile[num2 + 1, num + 1].frameY = 18;
				Main.tile[num2 + 1, num + 1].frameX = (short)(num3 + 18);
				Main.tile[num2, num + 2].active = true;
				Main.tile[num2, num + 2].type = 11;
				Main.tile[num2, num + 2].frameY = 36;
				Main.tile[num2, num + 2].frameX = num3;
				Main.tile[num2 + 1, num + 2].active = true;
				Main.tile[num2 + 1, num + 2].type = 11;
				Main.tile[num2 + 1, num + 2].frameY = 36;
				Main.tile[num2 + 1, num + 2].frameX = (short)(num3 + 18);
				for (int n = num2 - 1; n <= num2 + 2; n++)
				{
					for (int num5 = num - 1; num5 <= num + 2; num5++)
					{
						WorldGen.TileFrame(n, num5, false, false);
					}
				}
			}
			return flag;
		}
		public static void Check1xX(int x, int j, byte type)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
			int num = j - (int)(Main.tile[x, j].frameY / 18);
			int frameX = (int)Main.tile[x, j].frameX;
			int num2 = 3;
			if (type == 92)
			{
				num2 = 6;
			}
			bool flag = false;
			for (int i = 0; i < num2; i++)
			{
				if (!Main.tile[x, num + i].active)
				{
					flag = true;
				}
				else
				{
					if (Main.tile[x, num + i].type != type)
					{
						flag = true;
					}
					else
					{
						if ((int)Main.tile[x, num + i].frameY != i * 18)
						{
							flag = true;
						}
						else
						{
							if ((int)Main.tile[x, num + i].frameX != frameX)
							{
								flag = true;
							}
						}
					}
				}
			}
			if (!Main.tile[x, num + num2].active)
			{
				flag = true;
			}
			if (!Main.tileSolid[(int)Main.tile[x, num + num2].type])
			{
				flag = true;
			}
			if (flag)
			{
				WorldGen.destroyObject = true;
				for (int k = 0; k < num2; k++)
				{
					if (Main.tile[x, num + k].type == type)
					{
						WorldGen.KillTile(x, num + k, false, false, false);
					}
				}
				if (type == 92)
				{
					Item.NewItem(x * 16, j * 16, 32, 32, 341, 1, false, 0);
				}
				if (type == 93)
				{
					Item.NewItem(x * 16, j * 16, 32, 32, 342, 1, false, 0);
				}
				WorldGen.destroyObject = false;
			}
		}
		public static void Check2xX(int i, int j, byte type)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
			int num = i;
			int k;
			for (k = (int)Main.tile[i, j].frameX; k >= 36; k -= 36)
			{
			}
			if (k == 18)
			{
				num--;
			}
			int num2 = j - (int)(Main.tile[num, j].frameY / 18);
			int frameX = (int)Main.tile[num, j].frameX;
			int num3 = 3;
			if (type == 104)
			{
				num3 = 5;
			}
			bool flag = false;
			for (int l = 0; l < num3; l++)
			{
				if (!Main.tile[num, num2 + l].active)
				{
					flag = true;
				}
				else
				{
					if (Main.tile[num, num2 + l].type != type)
					{
						flag = true;
					}
					else
					{
						if ((int)Main.tile[num, num2 + l].frameY != l * 18)
						{
							flag = true;
						}
						else
						{
							if ((int)Main.tile[num, num2 + l].frameX != frameX)
							{
								flag = true;
							}
						}
					}
				}
				if (!Main.tile[num + 1, num2 + l].active)
				{
					flag = true;
				}
				else
				{
					if (Main.tile[num + 1, num2 + l].type != type)
					{
						flag = true;
					}
					else
					{
						if ((int)Main.tile[num + 1, num2 + l].frameY != l * 18)
						{
							flag = true;
						}
						else
						{
							if ((int)Main.tile[num + 1, num2 + l].frameX != frameX + 18)
							{
								flag = true;
							}
						}
					}
				}
			}
			if (!Main.tile[num, num2 + num3].active)
			{
				flag = true;
			}
			if (!Main.tileSolid[(int)Main.tile[num, num2 + num3].type])
			{
				flag = true;
			}
			if (!Main.tile[num + 1, num2 + num3].active)
			{
				flag = true;
			}
			if (!Main.tileSolid[(int)Main.tile[num + 1, num2 + num3].type])
			{
				flag = true;
			}
			if (flag)
			{
				WorldGen.destroyObject = true;
				for (int m = 0; m < num3; m++)
				{
					if (Main.tile[num, num2 + m].type == type)
					{
						WorldGen.KillTile(num, num2 + m, false, false, false);
					}
					if (Main.tile[num + 1, num2 + m].type == type)
					{
						WorldGen.KillTile(num + 1, num2 + m, false, false, false);
					}
				}
				if (type == 104)
				{
					Item.NewItem(num * 16, j * 16, 32, 32, 359, 1, false, 0);
				}
				if (type == 105)
				{
					int num4 = frameX / 36;
					if (num4 == 0)
					{
						num4 = 360;
					}
					else
					{
						if (num4 == 1)
						{
							num4 = 52;
						}
						else
						{
							num4 = 438 + num4 - 2;
						}
					}
					Item.NewItem(num * 16, j * 16, 32, 32, num4, 1, false, 0);
				}
				WorldGen.destroyObject = false;
			}
		}
		public static void Place1xX(int x, int y, int type, int style = 0)
		{
			int num = style * 18;
			int num2 = 3;
			if (type == 92)
			{
				num2 = 6;
			}
			bool flag = true;
			for (int i = y - num2 + 1; i < y + 1; i++)
			{
				if (Main.tile[x, i].active)
				{
					flag = false;
				}
				if (type == 93 && Main.tile[x, i].liquid > 0)
				{
					flag = false;
				}
			}
			if (flag && Main.tile[x, y + 1].active && Main.tileSolid[(int)Main.tile[x, y + 1].type])
			{
				for (int j = 0; j < num2; j++)
				{
					Main.tile[x, y - num2 + 1 + j].active = true;
					Main.tile[x, y - num2 + 1 + j].frameY = (short)(j * 18);
					Main.tile[x, y - num2 + 1 + j].frameX = (short)num;
					Main.tile[x, y - num2 + 1 + j].type = (byte)type;
				}
			}
		}
		public static void Place2xX(int x, int y, int type, int style = 0)
		{
			int num = style * 36;
			int num2 = 3;
			if (type == 104)
			{
				num2 = 5;
			}
			bool flag = true;
			for (int i = y - num2 + 1; i < y + 1; i++)
			{
				if (Main.tile[x, i].active)
				{
					flag = false;
				}
				if (Main.tile[x + 1, i].active)
				{
					flag = false;
				}
			}
			if (flag && Main.tile[x, y + 1].active && Main.tileSolid[(int)Main.tile[x, y + 1].type] && Main.tile[x + 1, y + 1].active && Main.tileSolid[(int)Main.tile[x + 1, y + 1].type])
			{
				for (int j = 0; j < num2; j++)
				{
					Main.tile[x, y - num2 + 1 + j].active = true;
					Main.tile[x, y - num2 + 1 + j].frameY = (short)(j * 18);
					Main.tile[x, y - num2 + 1 + j].frameX = (short)num;
					Main.tile[x, y - num2 + 1 + j].type = (byte)type;
					Main.tile[x + 1, y - num2 + 1 + j].active = true;
					Main.tile[x + 1, y - num2 + 1 + j].frameY = (short)(j * 18);
					Main.tile[x + 1, y - num2 + 1 + j].frameX = (short)(num + 18);
					Main.tile[x + 1, y - num2 + 1 + j].type = (byte)type;
				}
			}
		}
		public static void Check1x2(int x, int j, byte type)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
			int num = j;
			bool flag = true;
			int i = (int)Main.tile[x, num].frameY;
			int num2 = 0;
			while (i >= 40)
			{
				i -= 40;
				num2++;
			}
			if (i == 18)
			{
				num--;
			}
			if ((int)Main.tile[x, num].frameY == 40 * num2 && (int)Main.tile[x, num + 1].frameY == 40 * num2 + 18 && Main.tile[x, num].type == type && Main.tile[x, num + 1].type == type)
			{
				flag = false;
			}
			if (!Main.tile[x, num + 2].active || !Main.tileSolid[(int)Main.tile[x, num + 2].type])
			{
				flag = true;
			}
			if (Main.tile[x, num + 2].type != 2 && Main.tile[x, num + 2].type != 109 && Main.tile[x, num + 2].type != 147 && Main.tile[x, num].type == 20)
			{
				flag = true;
			}
			if (flag)
			{
				WorldGen.destroyObject = true;
				if (Main.tile[x, num].type == type)
				{
					WorldGen.KillTile(x, num, false, false, false);
				}
				if (Main.tile[x, num + 1].type == type)
				{
					WorldGen.KillTile(x, num + 1, false, false, false);
				}
				if (type == 15)
				{
					if (num2 == 1)
					{
						Item.NewItem(x * 16, num * 16, 32, 32, 358, 1, false, 0);
					}
					else
					{
						Item.NewItem(x * 16, num * 16, 32, 32, 34, 1, false, 0);
					}
				}
				else
				{
					if (type == 134)
					{
						Item.NewItem(x * 16, num * 16, 32, 32, 525, 1, false, 0);
					}
				}
				WorldGen.destroyObject = false;
			}
		}
		public static void CheckOnTable1x1(int x, int y, int type)
		{
			if (Main.tile[x, y + 1] != null && (!Main.tile[x, y + 1].active || !Main.tileTable[(int)Main.tile[x, y + 1].type]))
			{
				if (type == 78)
				{
					if (!Main.tile[x, y + 1].active || !Main.tileSolid[(int)Main.tile[x, y + 1].type])
					{
						WorldGen.KillTile(x, y, false, false, false);
						return;
					}
				}
				else
				{
					WorldGen.KillTile(x, y, false, false, false);
				}
			}
		}
		public static void CheckSign(int x, int y, int type)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
			int num = x - 2;
			int num2 = x + 3;
			int num3 = y - 2;
			int num4 = y + 3;
			if (num < 0)
			{
				return;
			}
			if (num2 > Main.maxTilesX)
			{
				return;
			}
			if (num3 < 0)
			{
				return;
			}
			if (num4 > Main.maxTilesY)
			{
				return;
			}
			bool flag = false;
			int k = (int)(Main.tile[x, y].frameX / 18);
			int num5 = (int)(Main.tile[x, y].frameY / 18);
			while (k > 1)
			{
				k -= 2;
			}
			int num6 = x - k;
			int num7 = y - num5;
			int num8 = (int)(Main.tile[num6, num7].frameX / 18 / 2);
			num = num6;
			num2 = num6 + 2;
			num3 = num7;
			num4 = num7 + 2;
			k = 0;
			for (int l = num; l < num2; l++)
			{
				num5 = 0;
				for (int m = num3; m < num4; m++)
				{
					if (!Main.tile[l, m].active || (int)Main.tile[l, m].type != type)
					{
						flag = true;
						break;
					}
					if ((int)(Main.tile[l, m].frameX / 18) != k + num8 * 2 || (int)(Main.tile[l, m].frameY / 18) != num5)
					{
						flag = true;
						break;
					}
					num5++;
				}
				k++;
			}
			if (!flag)
			{
				if (type == 85)
				{
					if (Main.tile[num6, num7 + 2].active && Main.tileSolid[(int)Main.tile[num6, num7 + 2].type] && Main.tile[num6 + 1, num7 + 2].active && Main.tileSolid[(int)Main.tile[num6 + 1, num7 + 2].type])
					{
						num8 = 0;
					}
					else
					{
						flag = true;
					}
				}
				else
				{
					if (Main.tile[num6, num7 + 2].active && Main.tileSolid[(int)Main.tile[num6, num7 + 2].type] && Main.tile[num6 + 1, num7 + 2].active && Main.tileSolid[(int)Main.tile[num6 + 1, num7 + 2].type])
					{
						num8 = 0;
					}
					else
					{
						if (Main.tile[num6, num7 - 1].active && Main.tileSolid[(int)Main.tile[num6, num7 - 1].type] && !Main.tileSolidTop[(int)Main.tile[num6, num7 - 1].type] && Main.tile[num6 + 1, num7 - 1].active && Main.tileSolid[(int)Main.tile[num6 + 1, num7 - 1].type] && !Main.tileSolidTop[(int)Main.tile[num6 + 1, num7 - 1].type])
						{
							num8 = 1;
						}
						else
						{
							if (Main.tile[num6 - 1, num7].active && Main.tileSolid[(int)Main.tile[num6 - 1, num7].type] && !Main.tileSolidTop[(int)Main.tile[num6 - 1, num7].type] && Main.tile[num6 - 1, num7 + 1].active && Main.tileSolid[(int)Main.tile[num6 - 1, num7 + 1].type] && !Main.tileSolidTop[(int)Main.tile[num6 - 1, num7 + 1].type])
							{
								num8 = 2;
							}
							else
							{
								if (Main.tile[num6 + 2, num7].active && Main.tileSolid[(int)Main.tile[num6 + 2, num7].type] && !Main.tileSolidTop[(int)Main.tile[num6 + 2, num7].type] && Main.tile[num6 + 2, num7 + 1].active && Main.tileSolid[(int)Main.tile[num6 + 2, num7 + 1].type] && !Main.tileSolidTop[(int)Main.tile[num6 + 2, num7 + 1].type])
								{
									num8 = 3;
								}
								else
								{
									flag = true;
								}
							}
						}
					}
				}
			}
			if (flag)
			{
				WorldGen.destroyObject = true;
				for (int n = num; n < num2; n++)
				{
					for (int num9 = num3; num9 < num4; num9++)
					{
						if ((int)Main.tile[n, num9].type == type)
						{
							WorldGen.KillTile(n, num9, false, false, false);
						}
					}
				}
				Sign.KillSign(num6, num7);
				if (type == 85)
				{
					Item.NewItem(x * 16, y * 16, 32, 32, 321, 1, false, 0);
				}
				else
				{
					Item.NewItem(x * 16, y * 16, 32, 32, 171, 1, false, 0);
				}
				WorldGen.destroyObject = false;
				return;
			}
			int num10 = 36 * num8;
			for (int num11 = 0; num11 < 2; num11++)
			{
				for (int num12 = 0; num12 < 2; num12++)
				{
					Main.tile[num6 + num11, num7 + num12].active = true;
					Main.tile[num6 + num11, num7 + num12].type = (byte)type;
					Main.tile[num6 + num11, num7 + num12].frameX = (short)(num10 + 18 * num11);
					Main.tile[num6 + num11, num7 + num12].frameY = (short)(18 * num12);
				}
			}
		}
		public static bool PlaceSign(int x, int y, int type)
		{
			int num = x - 2;
			int num2 = x + 3;
			int num3 = y - 2;
			int num4 = y + 3;
			if (num < 0)
			{
				return false;
			}
			if (num2 > Main.maxTilesX)
			{
				return false;
			}
			if (num3 < 0)
			{
				return false;
			}
			if (num4 > Main.maxTilesY)
			{
				return false;
			}
			int num5 = x;
			int num6 = y;
			int num7 = 0;
			if (type == 55)
			{
				if (Main.tile[x, y + 1].active && Main.tileSolid[(int)Main.tile[x, y + 1].type] && Main.tile[x + 1, y + 1].active && Main.tileSolid[(int)Main.tile[x + 1, y + 1].type])
				{
					num6--;
					num7 = 0;
				}
				else
				{
					if (Main.tile[x, y - 1].active && Main.tileSolid[(int)Main.tile[x, y - 1].type] && !Main.tileSolidTop[(int)Main.tile[x, y - 1].type] && Main.tile[x + 1, y - 1].active && Main.tileSolid[(int)Main.tile[x + 1, y - 1].type] && !Main.tileSolidTop[(int)Main.tile[x + 1, y - 1].type])
					{
						num7 = 1;
					}
					else
					{
						if (Main.tile[x - 1, y].active && Main.tileSolid[(int)Main.tile[x - 1, y].type] && !Main.tileSolidTop[(int)Main.tile[x - 1, y].type] && !Main.tileNoAttach[(int)Main.tile[x - 1, y].type] && Main.tile[x - 1, y + 1].active && Main.tileSolid[(int)Main.tile[x - 1, y + 1].type] && !Main.tileSolidTop[(int)Main.tile[x - 1, y + 1].type] && !Main.tileNoAttach[(int)Main.tile[x - 1, y + 1].type])
						{
							num7 = 2;
						}
						else
						{
							if (!Main.tile[x + 1, y].active || !Main.tileSolid[(int)Main.tile[x + 1, y].type] || Main.tileSolidTop[(int)Main.tile[x + 1, y].type] || Main.tileNoAttach[(int)Main.tile[x + 1, y].type] || !Main.tile[x + 1, y + 1].active || !Main.tileSolid[(int)Main.tile[x + 1, y + 1].type] || Main.tileSolidTop[(int)Main.tile[x + 1, y + 1].type] || Main.tileNoAttach[(int)Main.tile[x + 1, y + 1].type])
							{
								return false;
							}
							num5--;
							num7 = 3;
						}
					}
				}
			}
			else
			{
				if (type == 85)
				{
					if (!Main.tile[x, y + 1].active || !Main.tileSolid[(int)Main.tile[x, y + 1].type] || !Main.tile[x + 1, y + 1].active || !Main.tileSolid[(int)Main.tile[x + 1, y + 1].type])
					{
						return false;
					}
					num6--;
					num7 = 0;
				}
			}
			if (Main.tile[num5, num6].active || Main.tile[num5 + 1, num6].active || Main.tile[num5, num6 + 1].active || Main.tile[num5 + 1, num6 + 1].active)
			{
				return false;
			}
			int num8 = 36 * num7;
			for (int k = 0; k < 2; k++)
			{
				for (int l = 0; l < 2; l++)
				{
					Main.tile[num5 + k, num6 + l].active = true;
					Main.tile[num5 + k, num6 + l].type = (byte)type;
					Main.tile[num5 + k, num6 + l].frameX = (short)(num8 + 18 * k);
					Main.tile[num5 + k, num6 + l].frameY = (short)(18 * l);
				}
			}
			return true;
		}
		public static void Place1x1(int x, int y, int type, int style = 0)
		{
			if (WorldGen.SolidTile(x, y + 1) && !Main.tile[x, y].active)
			{
				Main.tile[x, y].active = true;
				Main.tile[x, y].type = (byte)type;
				if (type == 144)
				{
					Main.tile[x, y].frameX = (short)(style * 18);
					Main.tile[x, y].frameY = 0;
					return;
				}
				Main.tile[x, y].frameY = (short)(style * 18);
			}
		}
		public static void Check1x1(int x, int y, int type)
		{
			if (Main.tile[x, y + 1] != null && (!Main.tile[x, y + 1].active || !Main.tileSolid[(int)Main.tile[x, y + 1].type]))
			{
				WorldGen.KillTile(x, y, false, false, false);
			}
		}
		public static void PlaceOnTable1x1(int x, int y, int type, int style = 0)
		{
			bool flag = false;


			if (!Main.tile[x, y].active && Main.tile[x, y + 1].active && Main.tileTable[(int)Main.tile[x, y + 1].type])
			{
				flag = true;
			}
			if (type == 78 && !Main.tile[x, y].active && Main.tile[x, y + 1].active && Main.tileSolid[(int)Main.tile[x, y + 1].type])
			{
				flag = true;
			}
			if (flag)
			{
				Main.tile[x, y].active = true;
				Main.tile[x, y].frameX = (short)(style * 18);
				Main.tile[x, y].frameY = 0;
				Main.tile[x, y].type = (byte)type;
				if (type == 50)
				{
					Main.tile[x, y].frameX = (short)(18 * WorldGen.genRand.Next(5));
				}
			}
		}
		public static bool PlaceAlch(int x, int y, int style)
		{
			if (!Main.tile[x, y].active && Main.tile[x, y + 1].active)
			{
				bool flag = false;
				if (style == 0)
				{
					if (Main.tile[x, y + 1].type != 2 && Main.tile[x, y + 1].type != 78 && Main.tile[x, y + 1].type != 109)
					{
						flag = true;
					}
					if (Main.tile[x, y].liquid > 0)
					{
						flag = true;
					}
				}
				else
				{
					if (style == 1)
					{
						if (Main.tile[x, y + 1].type != 60 && Main.tile[x, y + 1].type != 78)
						{
							flag = true;
						}
						if (Main.tile[x, y].liquid > 0)
						{
							flag = true;
						}
					}
					else
					{
						if (style == 2)
						{
							if (Main.tile[x, y + 1].type != 0 && Main.tile[x, y + 1].type != 59 && Main.tile[x, y + 1].type != 78)
							{
								flag = true;
							}
							if (Main.tile[x, y].liquid > 0)
							{
								flag = true;
							}
						}
						else
						{
							if (style == 3)
							{
								if (Main.tile[x, y + 1].type != 23 && Main.tile[x, y + 1].type != 25 && Main.tile[x, y + 1].type != 78)
								{
									flag = true;
								}
								if (Main.tile[x, y].liquid > 0)
								{
									flag = true;
								}
							}
							else
							{
								if (style == 4)
								{
									if (Main.tile[x, y + 1].type != 53 && Main.tile[x, y + 1].type != 78 && Main.tile[x, y + 1].type != 116)
									{
										flag = true;
									}
									if (Main.tile[x, y].liquid > 0 && Main.tile[x, y].lava)
									{
										flag = true;
									}
								}
								else
								{
									if (style == 5)
									{
										if (Main.tile[x, y + 1].type != 57 && Main.tile[x, y + 1].type != 78)
										{
											flag = true;
										}
										if (Main.tile[x, y].liquid > 0 && !Main.tile[x, y].lava)
										{
											flag = true;
										}
									}
								}
							}
						}
					}
				}
				if (!flag)
				{
					Main.tile[x, y].active = true;
					Main.tile[x, y].type = 82;
					Main.tile[x, y].frameX = (short)(18 * style);
					Main.tile[x, y].frameY = 0;
					return true;
				}
			}
			return false;
		}
		public static void GrowAlch(int x, int y)
		{
			if (Main.tile[x, y].active)
			{
				if (Main.tile[x, y].type == 82 && WorldGen.genRand.Next(50) == 0)
				{
					Main.tile[x, y].type = 83;
					if (Main.netMode == 2)
					{
						NetMessage.SendTileSquare(-1, x, y, 1);
					}
					WorldGen.SquareTileFrame(x, y, true);
					return;
				}
				if (Main.tile[x, y].frameX == 36)
				{
					if (Main.tile[x, y].type == 83)
					{
						Main.tile[x, y].type = 84;
					}
					else
					{
						Main.tile[x, y].type = 83;
					}
					if (Main.netMode == 2)
					{
						NetMessage.SendTileSquare(-1, x, y, 1);
					}
				}
			}
		}
		public static void PlantAlch()
		{
			int num = WorldGen.genRand.Next(20, Main.maxTilesX - 20);
			int num2 = 0;
			if (WorldGen.genRand.Next(40) == 0)
			{
				num2 = WorldGen.genRand.Next((int)(Main.rockLayer + (double)Main.maxTilesY) / 2, Main.maxTilesY - 20);
			}
			else
			{
				if (WorldGen.genRand.Next(10) == 0)
				{
					num2 = WorldGen.genRand.Next(0, Main.maxTilesY - 20);
				}
				else
				{
					num2 = WorldGen.genRand.Next((int)Main.worldSurface, Main.maxTilesY - 20);
				}
			}
			while (num2 < Main.maxTilesY - 20 && !Main.tile[num, num2].active)
			{
				num2++;
			}
			if (Main.tile[num, num2].active && !Main.tile[num, num2 - 1].active && Main.tile[num, num2 - 1].liquid == 0)
			{
				if (Main.tile[num, num2].type == 2 || Main.tile[num, num2].type == 109)
				{
					WorldGen.PlaceAlch(num, num2 - 1, 0);
				}
				if (Main.tile[num, num2].type == 60)
				{
					WorldGen.PlaceAlch(num, num2 - 1, 1);
				}
				if (Main.tile[num, num2].type == 0 || Main.tile[num, num2].type == 59)
				{
					WorldGen.PlaceAlch(num, num2 - 1, 2);
				}
				if (Main.tile[num, num2].type == 23 || Main.tile[num, num2].type == 25)
				{
					WorldGen.PlaceAlch(num, num2 - 1, 3);
				}
				if (Main.tile[num, num2].type == 53 || Main.tile[num, num2].type == 116)
				{
					WorldGen.PlaceAlch(num, num2 - 1, 4);
				}
				if (Main.tile[num, num2].type == 57)
				{
					WorldGen.PlaceAlch(num, num2 - 1, 5);
				}
				if (Main.tile[num, num2 - 1].active && Main.netMode == 2)
				{
					NetMessage.SendTileSquare(-1, num, num2 - 1, 1);
				}
			}
		}
		public static void CheckAlch(int x, int y)
		{
			bool flag = false;
			if (!Main.tile[x, y + 1].active)
			{
				flag = true;
			}
			int num = (int)(Main.tile[x, y].frameX / 18);
			Main.tile[x, y].frameY = 0;
			if (!flag)
			{
				if (num == 0)
				{
					if (Main.tile[x, y + 1].type != 109 && Main.tile[x, y + 1].type != 2 && Main.tile[x, y + 1].type != 78)
					{
						flag = true;
					}
					if (Main.tile[x, y].liquid > 0 && Main.tile[x, y].lava)
					{
						flag = true;
					}
				}
				else
				{
					if (num == 1)
					{
						if (Main.tile[x, y + 1].type != 60 && Main.tile[x, y + 1].type != 78)
						{
							flag = true;
						}
						if (Main.tile[x, y].liquid > 0 && Main.tile[x, y].lava)
						{
							flag = true;
						}
					}
					else
					{
						if (num == 2)
						{
							if (Main.tile[x, y + 1].type != 0 && Main.tile[x, y + 1].type != 59 && Main.tile[x, y + 1].type != 78)
							{
								flag = true;
							}
							if (Main.tile[x, y].liquid > 0 && Main.tile[x, y].lava)
							{
								flag = true;
							}
						}
						else
						{
							if (num == 3)
							{
								if (Main.tile[x, y + 1].type != 23 && Main.tile[x, y + 1].type != 25 && Main.tile[x, y + 1].type != 78)
								{
									flag = true;
								}
								if (Main.tile[x, y].liquid > 0 && Main.tile[x, y].lava)
								{
									flag = true;
								}
							}
							else
							{
								if (num == 4)
								{
									if (Main.tile[x, y + 1].type != 53 && Main.tile[x, y + 1].type != 78 && Main.tile[x, y + 1].type != 116)
									{
										flag = true;
									}
									if (Main.tile[x, y].liquid > 0 && Main.tile[x, y].lava)
									{
										flag = true;
									}
									if (Main.tile[x, y].type != 82 && !Main.tile[x, y].lava && Main.netMode != 1)
									{
										if (Main.tile[x, y].liquid > 16)
										{
											if (Main.tile[x, y].type == 83)
											{
												Main.tile[x, y].type = 84;
												if (Main.netMode == 2)
												{
													NetMessage.SendTileSquare(-1, x, y, 1);
												}
											}
										}
										else
										{
											if (Main.tile[x, y].type == 84)
											{
												Main.tile[x, y].type = 83;
												if (Main.netMode == 2)
												{
													NetMessage.SendTileSquare(-1, x, y, 1);
												}
											}
										}
									}
								}
								else
								{
									if (num == 5)
									{
										if (Main.tile[x, y + 1].type != 57 && Main.tile[x, y + 1].type != 78)
										{
											flag = true;
										}
										if (Main.tile[x, y].liquid > 0 && !Main.tile[x, y].lava)
										{
											flag = true;
										}
										if (Main.tile[x, y].type != 82 && Main.tile[x, y].lava && Main.tile[x, y].type != 82 && Main.tile[x, y].lava && Main.netMode != 1)
										{
											if (Main.tile[x, y].liquid > 16)
											{
												if (Main.tile[x, y].type == 83)
												{
													Main.tile[x, y].type = 84;
													if (Main.netMode == 2)
													{
														NetMessage.SendTileSquare(-1, x, y, 1);
													}
												}
											}
											else
											{
												if (Main.tile[x, y].type == 84)
												{
													Main.tile[x, y].type = 83;
													if (Main.netMode == 2)
													{
														NetMessage.SendTileSquare(-1, x, y, 1);
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
			if (flag)
			{
				WorldGen.KillTile(x, y, false, false, false);
			}
		}
		public static void CheckBanner(int x, int j, byte type)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
			int num = j - (int)(Main.tile[x, j].frameY / 18);
			int frameX = (int)Main.tile[x, j].frameX;
			bool flag = false;
			for (int i = 0; i < 3; i++)
			{
				if (!Main.tile[x, num + i].active)
				{
					flag = true;
				}
				else
				{
					if (Main.tile[x, num + i].type != type)
					{
						flag = true;
					}
					else
					{
						if ((int)Main.tile[x, num + i].frameY != i * 18)
						{
							flag = true;
						}
						else
						{
							if ((int)Main.tile[x, num + i].frameX != frameX)
							{
								flag = true;
							}
						}
					}
				}
			}
			if (!Main.tile[x, num - 1].active)
			{
				flag = true;
			}
			if (!Main.tileSolid[(int)Main.tile[x, num - 1].type])
			{
				flag = true;
			}
			if (Main.tileSolidTop[(int)Main.tile[x, num - 1].type])
			{
				flag = true;
			}
			if (flag)
			{
				WorldGen.destroyObject = true;
				for (int k = 0; k < 3; k++)
				{
					if (Main.tile[x, num + k].type == type)
					{
						WorldGen.KillTile(x, num + k, false, false, false);
					}
				}
				if (type == 91)
				{
					int num2 = frameX / 18;
					Item.NewItem(x * 16, (num + 1) * 16, 32, 32, 337 + num2, 1, false, 0);
				}
				WorldGen.destroyObject = false;
			}
		}
		public static void PlaceBanner(int x, int y, int type, int style = 0)
		{
			int num = style * 18;
			if (Main.tile[x, y - 1].active && Main.tileSolid[(int)Main.tile[x, y - 1].type] && !Main.tileSolidTop[(int)Main.tile[x, y - 1].type] && !Main.tile[x, y].active && !Main.tile[x, y + 1].active && !Main.tile[x, y + 2].active)
			{
				Main.tile[x, y].active = true;
				Main.tile[x, y].frameY = 0;
				Main.tile[x, y].frameX = (short)num;
				Main.tile[x, y].type = (byte)type;
				Main.tile[x, y + 1].active = true;
				Main.tile[x, y + 1].frameY = 18;
				Main.tile[x, y + 1].frameX = (short)num;
				Main.tile[x, y + 1].type = (byte)type;
				Main.tile[x, y + 2].active = true;
				Main.tile[x, y + 2].frameY = 36;
				Main.tile[x, y + 2].frameX = (short)num;
				Main.tile[x, y + 2].type = (byte)type;
			}
		}
		public static void PlaceMan(int i, int j, int dir)
		{
			for (int k = i; k <= i + 1; k++)
			{
				for (int l = j - 2; l <= j; l++)
				{
					if (Main.tile[k, l].active)
					{
						return;
					}
				}
			}
			if (!WorldGen.SolidTile(i, j + 1) || !WorldGen.SolidTile(i + 1, j + 1))
			{
				return;
			}
			byte b = 0;
			if (dir == 1)
			{
				b = 36;
			}
			Main.tile[i, j - 2].active = true;
			Main.tile[i, j - 2].frameY = 0;
			Main.tile[i, j - 2].frameX = (short)b;
			Main.tile[i, j - 2].type = 128;
			Main.tile[i, j - 1].active = true;
			Main.tile[i, j - 1].frameY = 18;
			Main.tile[i, j - 1].frameX = (short)b;
			Main.tile[i, j - 1].type = 128;
			Main.tile[i, j].active = true;
			Main.tile[i, j].frameY = 36;
			Main.tile[i, j].frameX = (short)b;
			Main.tile[i, j].type = 128;
			Main.tile[i + 1, j - 2].active = true;
			Main.tile[i + 1, j - 2].frameY = 0;
			Main.tile[i + 1, j - 2].frameX = (short)(18 + b);
			Main.tile[i + 1, j - 2].type = 128;
			Main.tile[i + 1, j - 1].active = true;
			Main.tile[i + 1, j - 1].frameY = 18;
			Main.tile[i + 1, j - 1].frameX = (short)(18 + b);
			Main.tile[i + 1, j - 1].type = 128;
			Main.tile[i + 1, j].active = true;
			Main.tile[i + 1, j].frameY = 36;
			Main.tile[i + 1, j].frameX = (short)(18 + b);
			Main.tile[i + 1, j].type = 128;
		}
		public static void CheckMan(int i, int j)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
			int num = j - (int)(Main.tile[i, j].frameY / 18);
			int k;
			for (k = (int)Main.tile[i, j].frameX; k >= 100; k -= 100)
			{
			}
			while (k >= 36)
			{
				k -= 36;
			}
			int num2 = i - k / 18;
			bool flag = false;
			for (int l = 0; l <= 1; l++)
			{
				for (int m = 0; m <= 2; m++)
				{
					int num3 = num2 + l;
					int num4 = num + m;
					int n;
					for (n = (int)Main.tile[num3, num4].frameX; n >= 100; n -= 100)
					{
					}
					if (n >= 36)
					{
						n -= 36;
					}
					if (!Main.tile[num3, num4].active || Main.tile[num3, num4].type != 128 || (int)Main.tile[num3, num4].frameY != m * 18 || n != l * 18)
					{
						flag = true;
					}
				}
			}
			if (!WorldGen.SolidTile(num2, num + 3) || !WorldGen.SolidTile(num2 + 1, num + 3))
			{
				flag = true;
			}
			if (flag)
			{
				WorldGen.destroyObject = true;
				Item.NewItem(i * 16, j * 16, 32, 32, 498, 1, false, 0);
				for (int num5 = 0; num5 <= 1; num5++)
				{
					for (int num6 = 0; num6 <= 2; num6++)
					{
						int num7 = num2 + num5;
						int num8 = num + num6;
						if (Main.tile[num7, num8].active && Main.tile[num7, num8].type == 128)
						{
							WorldGen.KillTile(num7, num8, false, false, false);
						}
					}
				}
				WorldGen.destroyObject = false;
			}
		}
		public static void Place1x2(int x, int y, int type, int style)
		{
			short frameX = 0;
			if (type == 20)
			{
				frameX = (short)(WorldGen.genRand.Next(3) * 18);
			}
			if (Main.tile[x, y + 1].active && Main.tileSolid[(int)Main.tile[x, y + 1].type] && !Main.tile[x, y - 1].active)
			{
				short num = (short)(style * 40);
				Main.tile[x, y - 1].active = true;
				Main.tile[x, y - 1].frameY = num;
				Main.tile[x, y - 1].frameX = frameX;
				Main.tile[x, y - 1].type = (byte)type;
				Main.tile[x, y].active = true;
				Main.tile[x, y].frameY = (byte)(num + 18);
				Main.tile[x, y].frameX = frameX;
				Main.tile[x, y].type = (byte)type;
			}
		}
		public static void Place1x2Top(int x, int y, int type)
		{
			short frameX = 0;
			if (Main.tile[x, y - 1].active && Main.tileSolid[(int)Main.tile[x, y - 1].type] && !Main.tileSolidTop[(int)Main.tile[x, y - 1].type] && !Main.tile[x, y + 1].active)
			{
				Main.tile[x, y].active = true;
				Main.tile[x, y].frameY = 0;
				Main.tile[x, y].frameX = frameX;
				Main.tile[x, y].type = (byte)type;
				Main.tile[x, y + 1].active = true;
				Main.tile[x, y + 1].frameY = 18;
				Main.tile[x, y + 1].frameX = frameX;
				Main.tile[x, y + 1].type = (byte)type;
			}
		}
		public static void Check1x2Top(int x, int j, byte type)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
			int num = j;
			bool flag = true;
			if (Main.tile[x, num].frameY == 18)
			{
				num--;
			}
			if (Main.tile[x, num].frameY == 0 && Main.tile[x, num + 1].frameY == 18 && Main.tile[x, num].type == type && Main.tile[x, num + 1].type == type)
			{
				flag = false;
			}
			if (!Main.tile[x, num - 1].active || !Main.tileSolid[(int)Main.tile[x, num - 1].type] || Main.tileSolidTop[(int)Main.tile[x, num - 1].type])
			{
				flag = true;
			}
			if (flag)
			{
				WorldGen.destroyObject = true;
				if (Main.tile[x, num].type == type)
				{
					WorldGen.KillTile(x, num, false, false, false);
				}
				if (Main.tile[x, num + 1].type == type)
				{
					WorldGen.KillTile(x, num + 1, false, false, false);
				}
				if (type == 42)
				{
					Item.NewItem(x * 16, num * 16, 32, 32, 136, 1, false, 0);
				}
				WorldGen.destroyObject = false;
			}
		}
		public static void Check2x1(int i, int y, byte type)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
			int num = i;
			bool flag = true;
			if (Main.tile[num, y].frameX == 18)
			{
				num--;
			}
			if (Main.tile[num, y].frameX == 0 && Main.tile[num + 1, y].frameX == 18 && Main.tile[num, y].type == type && Main.tile[num + 1, y].type == type)
			{
				flag = false;
			}
			if (type == 29 || type == 103)
			{
				if (!Main.tile[num, y + 1].active || !Main.tileTable[(int)Main.tile[num, y + 1].type])
				{
					flag = true;
				}
				if (!Main.tile[num + 1, y + 1].active || !Main.tileTable[(int)Main.tile[num + 1, y + 1].type])
				{
					flag = true;
				}
			}
			else
			{
				if (!Main.tile[num, y + 1].active || !Main.tileSolid[(int)Main.tile[num, y + 1].type])
				{
					flag = true;
				}
				if (!Main.tile[num + 1, y + 1].active || !Main.tileSolid[(int)Main.tile[num + 1, y + 1].type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				WorldGen.destroyObject = true;
				if (Main.tile[num, y].type == type)
				{
					WorldGen.KillTile(num, y, false, false, false);
				}
				if (Main.tile[num + 1, y].type == type)
				{
					WorldGen.KillTile(num + 1, y, false, false, false);
				}
				if (type == 16)
				{
					Item.NewItem(num * 16, y * 16, 32, 32, 35, 1, false, 0);
				}
				if (type == 18)
				{
					Item.NewItem(num * 16, y * 16, 32, 32, 36, 1, false, 0);
				}
				if (type == 29)
				{
					Item.NewItem(num * 16, y * 16, 32, 32, 87, 1, false, 0);
					Main.PlaySound(13, i * 16, y * 16, 1);
				}
				if (type == 103)
				{
					Item.NewItem(num * 16, y * 16, 32, 32, 356, 1, false, 0);
					Main.PlaySound(13, i * 16, y * 16, 1);
				}
				if (type == 134)
				{
					Item.NewItem(num * 16, y * 16, 32, 32, 525, 1, false, 0);
				}
				WorldGen.destroyObject = false;
				WorldGen.SquareTileFrame(num, y, true);
				WorldGen.SquareTileFrame(num + 1, y, true);
			}
		}
		public static void Place2x1(int x, int y, int type)
		{
			bool flag = false;
			if (type != 29 && type != 103 && Main.tile[x, y + 1].active && Main.tile[x + 1, y + 1].active && Main.tileSolid[(int)Main.tile[x, y + 1].type] && Main.tileSolid[(int)Main.tile[x + 1, y + 1].type] && !Main.tile[x, y].active && !Main.tile[x + 1, y].active)
			{
				flag = true;
			}
			else
			{
				if ((type == 29 || type == 103) && Main.tile[x, y + 1].active && Main.tile[x + 1, y + 1].active && Main.tileTable[(int)Main.tile[x, y + 1].type] && Main.tileTable[(int)Main.tile[x + 1, y + 1].type] && !Main.tile[x, y].active && !Main.tile[x + 1, y].active)
				{
					flag = true;
				}
			}
			if (flag)
			{
				Main.tile[x, y].active = true;
				Main.tile[x, y].frameY = 0;
				Main.tile[x, y].frameX = 0;
				Main.tile[x, y].type = (byte)type;
				Main.tile[x + 1, y].active = true;
				Main.tile[x + 1, y].frameY = 0;
				Main.tile[x + 1, y].frameX = 18;
				Main.tile[x + 1, y].type = (byte)type;
			}
		}
		public static void Check4x2(int i, int j, int type)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
			bool flag = false;
			int num = i;
			num += (int)(Main.tile[i, j].frameX / 18 * -1);
			if ((type == 79 || type == 90) && Main.tile[i, j].frameX >= 72)
			{
				num += 4;
			}
			int num2 = j + (int)(Main.tile[i, j].frameY / 18 * -1);
			for (int k = num; k < num + 4; k++)
			{
				for (int l = num2; l < num2 + 2; l++)
				{
					int num3 = (k - num) * 18;
					if ((type == 79 || type == 90) && Main.tile[i, j].frameX >= 72)
					{
						num3 = (k - num + 4) * 18;
					}
					if (!Main.tile[k, l].active || (int)Main.tile[k, l].type != type || (int)Main.tile[k, l].frameX != num3 || (int)Main.tile[k, l].frameY != (l - num2) * 18)
					{
						flag = true;
					}
				}
				if (!Main.tile[k, num2 + 2].active || !Main.tileSolid[(int)Main.tile[k, num2 + 2].type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				WorldGen.destroyObject = true;
				for (int m = num; m < num + 4; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)Main.tile[m, n].type == type && Main.tile[m, n].active)
						{
							WorldGen.KillTile(m, n, false, false, false);
						}
					}
				}
				if (type == 79)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 224, 1, false, 0);
				}
				if (type == 90)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 336, 1, false, 0);
				}
				WorldGen.destroyObject = false;
				for (int num4 = num - 1; num4 < num + 4; num4++)
				{
					for (int num5 = num2 - 1; num5 < num2 + 4; num5++)
					{
						WorldGen.TileFrame(num4, num5, false, false);
					}
				}
			}
		}
		public static void Check2x2(int i, int j, int type)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
			bool flag = false;
			int num = i;
			int num2 = 0;
			num = (int)(Main.tile[i, j].frameX / 18 * -1);
			int num3 = (int)(Main.tile[i, j].frameY / 18 * -1);
			if (num < -1)
			{
				num += 2;
				num2 = 36;
			}
			num += i;
			num3 += j;
			for (int k = num; k < num + 2; k++)
			{
				for (int l = num3; l < num3 + 2; l++)
				{
					if (!Main.tile[k, l].active || (int)Main.tile[k, l].type != type || (int)Main.tile[k, l].frameX != (k - num) * 18 + num2 || (int)Main.tile[k, l].frameY != (l - num3) * 18)
					{
						flag = true;
					}
				}
				if (type == 95 || type == 126)
				{
					if (!Main.tile[k, num3 - 1].active || !Main.tileSolid[(int)Main.tile[k, num3 - 1].type] || Main.tileSolidTop[(int)Main.tile[k, num3 - 1].type])
					{
						flag = true;
					}
				}
				else
				{
					if (type != 138)
					{
						if (!Main.tile[k, num3 + 2].active || (!Main.tileSolid[(int)Main.tile[k, num3 + 2].type] && !Main.tileTable[(int)Main.tile[k, num3 + 2].type]))
						{
							flag = true;
						}
					}
				}
			}
			if (type == 138 && !WorldGen.SolidTile(num, num3 + 2) && !WorldGen.SolidTile(num + 1, num3 + 2))
			{
				flag = true;
			}
			if (flag)
			{
				WorldGen.destroyObject = true;
				for (int m = num; m < num + 2; m++)
				{
					for (int n = num3; n < num3 + 2; n++)
					{
						if ((int)Main.tile[m, n].type == type && Main.tile[m, n].active)
						{
							WorldGen.KillTile(m, n, false, false, false);
						}
					}
				}
				if (type == 85)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 321, 1, false, 0);
				}
				if (type == 94)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 352, 1, false, 0);
				}
				if (type == 95)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 344, 1, false, 0);
				}
				if (type == 96)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 345, 1, false, 0);
				}
				if (type == 97)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 346, 1, false, 0);
				}
				if (type == 98)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 347, 1, false, 0);
				}
				if (type == 99)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 348, 1, false, 0);
				}
				if (type == 100)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 349, 1, false, 0);
				}
				if (type == 125)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 487, 1, false, 0);
				}
				if (type == 126)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 488, 1, false, 0);
				}
				if (type == 132)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 513, 1, false, 0);
				}
				if (type == 142)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 581, 1, false, 0);
				}
				if (type == 143)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 582, 1, false, 0);
				}
				if (type == 138 && !WorldGen.gen && Main.netMode != 1)
				{
					Projectile.NewProjectile((float)(num * 16) + 15.5f, (float)(num3 * 16 + 16), 0f, 0f, 99, 70, 10f, Main.myPlayer);
				}
				WorldGen.destroyObject = false;
				for (int num4 = num - 1; num4 < num + 3; num4++)
				{
					for (int num5 = num3 - 1; num5 < num3 + 3; num5++)
					{
						WorldGen.TileFrame(num4, num5, false, false);
					}
				}
			}
		}
		public static void OreRunner(int i, int j, double strength, int steps, int type)
		{
			double num = strength;
			float num2 = (float)steps;
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j;
			Vector2 value2;
			value2.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
			value2.Y = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
			while (num > 0.0 && num2 > 0f)
			{
				if (value.Y < 0f && num2 > 0f && type == 59)
				{
					num2 = 0f;
				}
				num = strength * (double)(num2 / (float)steps);
				num2 -= 1f;
				int num3 = (int)((double)value.X - num * 0.5);
				int num4 = (int)((double)value.X + num * 0.5);
				int num5 = (int)((double)value.Y - num * 0.5);
				int num6 = (int)((double)value.Y + num * 0.5);
				if (num3 < 0)
				{
					num3 = 0;
				}
				if (num4 > Main.maxTilesX)
				{
					num4 = Main.maxTilesX;
				}
				if (num5 < 0)
				{
					num5 = 0;
				}
				if (num6 > Main.maxTilesY)
				{
					num6 = Main.maxTilesY;
				}
				for (int k = num3; k < num4; k++)
				{
					for (int l = num5; l < num6; l++)
					{
						if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < strength * 0.5 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015) && Main.tile[k, l].active && (Main.tile[k, l].type == 0 || Main.tile[k, l].type == 1 || Main.tile[k, l].type == 23 || Main.tile[k, l].type == 25 || Main.tile[k, l].type == 40 || Main.tile[k, l].type == 53 || Main.tile[k, l].type == 57 || Main.tile[k, l].type == 59 || Main.tile[k, l].type == 60 || Main.tile[k, l].type == 70 || Main.tile[k, l].type == 109 || Main.tile[k, l].type == 112 || Main.tile[k, l].type == 116 || Main.tile[k, l].type == 117))
						{
							Main.tile[k, l].type = (byte)type;
							WorldGen.SquareTileFrame(k, l, true);
							if (Main.netMode == 2)
							{
								NetMessage.SendTileSquare(-1, k, l, 1);
							}
						}
					}
				}
				value += value2;
				value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
				if (value2.X > 1f)
				{
					value2.X = 1f;
				}
				if (value2.X < -1f)
				{
					value2.X = -1f;
				}
			}
		}
		public static void SmashAltar(int i, int j)
		{
			if (Main.netMode == 1)
			{
				return;
			}
			if (!Main.hardMode)
			{
				return;
			}
			if (WorldGen.noTileActions)
			{
				return;
			}
			if (WorldGen.gen)
			{
				return;
			}
			int num = WorldGen.altarCount % 3;
			int num2 = WorldGen.altarCount / 3 + 1;
			float num3 = (float)(Main.maxTilesX / 4200);
			int num4 = 1 - num;
			num3 = num3 * 310f - (float)(85 * num);
			num3 *= 0.85f;
			num3 /= (float)num2;
			if (num == 0)
			{
				if (Main.netMode == 0)
				{
					Main.NewText("Your world has been blessed with Cobalt!", 50, 255, 130);
				}
				else
				{
					if (Main.netMode == 2)
					{
						NetMessage.SendData(25, -1, -1, "Your world has been blessed with Cobalt!", 255, 50f, 255f, 130f, 0);
					}
				}
				num = 107;
				num3 *= 1.05f;
			}
			else
			{
				if (num == 1)
				{
					if (Main.netMode == 0)
					{
						Main.NewText("Your world has been blessed with Mythril!", 50, 255, 130);
					}
					else
					{
						if (Main.netMode == 2)
						{
							NetMessage.SendData(25, -1, -1, "Your world has been blessed with Mythril!", 255, 50f, 255f, 130f, 0);
						}
					}
					num = 108;
				}
				else
				{
					if (Main.netMode == 0)
					{
						Main.NewText("Your world has been blessed with Adamantite!", 50, 255, 130);
					}
					else
					{
						if (Main.netMode == 2)
						{
							NetMessage.SendData(25, -1, -1, "Your world has been blessed with Adamantite!", 255, 50f, 255f, 130f, 0);
						}
					}
					num = 111;
				}
			}
			int num5 = 0;
			while ((float)num5 < num3)
			{
				int i2 = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
				double num6 = Main.worldSurface;
				if (num == 108)
				{
					num6 = Main.rockLayer;
				}
				if (num == 111)
				{
					num6 = (Main.rockLayer + Main.rockLayer + (double)Main.maxTilesY) / 3.0;
				}
				int j2 = WorldGen.genRand.Next((int)num6, Main.maxTilesY - 150);
				WorldGen.OreRunner(i2, j2, (double)WorldGen.genRand.Next(5, 9 + num4), WorldGen.genRand.Next(5, 9 + num4), num);
				num5++;
			}
			int num7 = WorldGen.genRand.Next(3);
			while (num7 != 2)
			{
				int num8 = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
				int num9 = WorldGen.genRand.Next((int)Main.rockLayer + 50, Main.maxTilesY - 300);
				if (Main.tile[num8, num9].active && Main.tile[num8, num9].type == 1)
				{
					if (num7 == 0)
					{
						Main.tile[num8, num9].type = 25;
					}
					else
					{
						Main.tile[num8, num9].type = 117;
					}
					if (Main.netMode == 2)
					{
						NetMessage.SendTileSquare(-1, num8, num9, 1);
						break;
					}
					break;
				}
			}
			if (Main.netMode != 1)
			{
				int num10 = Main.rand.Next(2) + 1;
				for (int k = 0; k < num10; k++)
				{
					NPC.SpawnOnPlayer((int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16), 82);
				}
			}
			WorldGen.altarCount++;
		}
		public static void Check3x2(int i, int j, int type)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
			bool flag = false;
			int num = i + (int)(Main.tile[i, j].frameX / 18 * -1);
			int num2 = j + (int)(Main.tile[i, j].frameY / 18 * -1);
			for (int k = num; k < num + 3; k++)
			{
				for (int l = num2; l < num2 + 2; l++)
				{
					if (!Main.tile[k, l].active || (int)Main.tile[k, l].type != type || (int)Main.tile[k, l].frameX != (k - num) * 18 || (int)Main.tile[k, l].frameY != (l - num2) * 18)
					{
						flag = true;
					}
				}
				if (!Main.tile[k, num2 + 2].active || !Main.tileSolid[(int)Main.tile[k, num2 + 2].type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				WorldGen.destroyObject = true;
				for (int m = num; m < num + 3; m++)
				{
					for (int n = num2; n < num2 + 3; n++)
					{
						if ((int)Main.tile[m, n].type == type && Main.tile[m, n].active)
						{
							WorldGen.KillTile(m, n, false, false, false);
						}
					}
				}
				if (type == 14)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 32, 1, false, 0);
				}
				else
				{
					if (type == 114)
					{
						Item.NewItem(i * 16, j * 16, 32, 32, 398, 1, false, 0);
					}
					else
					{
						if (type == 26)
						{
							if (!WorldGen.noTileActions)
							{
								WorldGen.SmashAltar(i, j);
							}
						}
						else
						{
							if (type == 17)
							{
								Item.NewItem(i * 16, j * 16, 32, 32, 33, 1, false, 0);
							}
							else
							{
								if (type == 77)
								{
									Item.NewItem(i * 16, j * 16, 32, 32, 221, 1, false, 0);
								}
								else
								{
									if (type == 86)
									{
										Item.NewItem(i * 16, j * 16, 32, 32, 332, 1, false, 0);
									}
									else
									{
										if (type == 87)
										{
											Item.NewItem(i * 16, j * 16, 32, 32, 333, 1, false, 0);
										}
										else
										{
											if (type == 88)
											{
												Item.NewItem(i * 16, j * 16, 32, 32, 334, 1, false, 0);
											}
											else
											{
												if (type == 89)
												{
													Item.NewItem(i * 16, j * 16, 32, 32, 335, 1, false, 0);
												}
												else
												{
													if (type == 133)
													{
														Item.NewItem(i * 16, j * 16, 32, 32, 524, 1, false, 0);
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
				WorldGen.destroyObject = false;
				for (int num3 = num - 1; num3 < num + 4; num3++)
				{
					for (int num4 = num2 - 1; num4 < num2 + 4; num4++)
					{
						WorldGen.TileFrame(num3, num4, false, false);
					}
				}
			}
		}
		public static void Check3x4(int i, int j, int type)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
			bool flag = false;
			int num = i + (int)(Main.tile[i, j].frameX / 18 * -1);
			int num2 = j + (int)(Main.tile[i, j].frameY / 18 * -1);
			for (int k = num; k < num + 3; k++)
			{
				for (int l = num2; l < num2 + 4; l++)
				{
					if (!Main.tile[k, l].active || (int)Main.tile[k, l].type != type || (int)Main.tile[k, l].frameX != (k - num) * 18 || (int)Main.tile[k, l].frameY != (l - num2) * 18)
					{
						flag = true;
					}
				}
				if (!Main.tile[k, num2 + 4].active || !Main.tileSolid[(int)Main.tile[k, num2 + 4].type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				WorldGen.destroyObject = true;
				for (int m = num; m < num + 3; m++)
				{
					for (int n = num2; n < num2 + 4; n++)
					{
						if ((int)Main.tile[m, n].type == type && Main.tile[m, n].active)
						{
							WorldGen.KillTile(m, n, false, false, false);
						}
					}
				}
				if (type == 101)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 354, 1, false, 0);
				}
				else
				{
					if (type == 102)
					{
						Item.NewItem(i * 16, j * 16, 32, 32, 355, 1, false, 0);
					}
				}
				WorldGen.destroyObject = false;
				for (int num3 = num - 1; num3 < num + 4; num3++)
				{
					for (int num4 = num2 - 1; num4 < num2 + 4; num4++)
					{
						WorldGen.TileFrame(num3, num4, false, false);
					}
				}
			}
		}
		public static void Place4x2(int x, int y, int type, int direction = -1)
		{
			if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
			{
				return;
			}
			bool flag = true;
			for (int i = x - 1; i < x + 3; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (Main.tile[i, j].active)
					{
						flag = false;
					}
				}
				if (!Main.tile[i, y + 1].active || !Main.tileSolid[(int)Main.tile[i, y + 1].type])
				{
					flag = false;
				}
			}
			short num = 0;
			if (direction == 1)
			{
				num = 72;
			}
			if (flag)
			{
				Main.tile[x - 1, y - 1].active = true;
				Main.tile[x - 1, y - 1].frameY = 0;
				Main.tile[x - 1, y - 1].frameX = num;
				Main.tile[x - 1, y - 1].type = (byte)type;
				Main.tile[x, y - 1].active = true;
				Main.tile[x, y - 1].frameY = 0;
				Main.tile[x, y - 1].frameX = (byte)(18 + num);
				Main.tile[x, y - 1].type = (byte)type;
				Main.tile[x + 1, y - 1].active = true;
				Main.tile[x + 1, y - 1].frameY = 0;
				Main.tile[x + 1, y - 1].frameX = (byte)(36 + num);
				Main.tile[x + 1, y - 1].type = (byte)type;
				Main.tile[x + 2, y - 1].active = true;
				Main.tile[x + 2, y - 1].frameY = 0;
				Main.tile[x + 2, y - 1].frameX = (byte)(54 + num);
				Main.tile[x + 2, y - 1].type = (byte)type;
				Main.tile[x - 1, y].active = true;
				Main.tile[x - 1, y].frameY = 18;
				Main.tile[x - 1, y].frameX = num;
				Main.tile[x - 1, y].type = (byte)type;
				Main.tile[x, y].active = true;
				Main.tile[x, y].frameY = 18;
				Main.tile[x, y].frameX = (byte)(18 + num);
				Main.tile[x, y].type = (byte)type;
				Main.tile[x + 1, y].active = true;
				Main.tile[x + 1, y].frameY = 18;
				Main.tile[x + 1, y].frameX = (byte)(36 + num);
				Main.tile[x + 1, y].type = (byte)type;
				Main.tile[x + 2, y].active = true;
				Main.tile[x + 2, y].frameY = 18;
				Main.tile[x + 2, y].frameX = (byte)(54 + num);
				Main.tile[x + 2, y].type = (byte)type;
			}
		}
		public static void SwitchMB(int i, int j)
		{
			int k;
			for (k = (int)(Main.tile[i, j].frameY / 18); k >= 2; k -= 2)
			{
			}
			int num = (int)(Main.tile[i, j].frameX / 18);
			if (num >= 2)
			{
				num -= 2;
			}
			int num2 = i - num;
			int num3 = j - k;
			for (int l = num2; l < num2 + 2; l++)
			{
				for (int m = num3; m < num3 + 2; m++)
				{
					if (Main.tile[l, m].active && Main.tile[l, m].type == 139)
					{
						if (Main.tile[l, m].frameX < 36)
						{
                            Main.tile[l, m].frameX += 36;
						}
						else
						{
                            Main.tile[l, m].frameX -= 36;
						}
						WorldGen.noWireX[WorldGen.numNoWire] = l;
						WorldGen.noWireY[WorldGen.numNoWire] = m;
						WorldGen.numNoWire++;
					}
				}
			}
			NetMessage.SendTileSquare(-1, num2, num3, 3);
		}
		public static void CheckMB(int i, int j, int type)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
			bool flag = false;
			int num = 0;
			int k;
			for (k = (int)(Main.tile[i, j].frameY / 18); k >= 2; k -= 2)
			{
				num++;
			}
			int num2 = (int)(Main.tile[i, j].frameX / 18);
			int num3 = 0;
			if (num2 >= 2)
			{
				num2 -= 2;
				num3++;
			}
			int num4 = i - num2;
			int num5 = j - k;
			for (int l = num4; l < num4 + 2; l++)
			{
				for (int m = num5; m < num5 + 2; m++)
				{
					if (!Main.tile[l, m].active || (int)Main.tile[l, m].type != type || (int)Main.tile[l, m].frameX != (l - num4) * 18 + num3 * 36 || (int)Main.tile[l, m].frameY != (m - num5) * 18 + num * 36)
					{
						flag = true;
					}
				}
				if (!Main.tileSolid[(int)Main.tile[l, num5 + 2].type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				WorldGen.destroyObject = true;
				for (int n = num4; n < num4 + 2; n++)
				{
					for (int num6 = num5; num6 < num5 + 3; num6++)
					{
						if ((int)Main.tile[n, num6].type == type && Main.tile[n, num6].active)
						{
							WorldGen.KillTile(n, num6, false, false, false);
						}
					}
				}
				Item.NewItem(i * 16, j * 16, 32, 32, 562 + num, 1, false, 0);
				for (int num7 = num4 - 1; num7 < num4 + 3; num7++)
				{
					for (int num8 = num5 - 1; num8 < num5 + 3; num8++)
					{
						WorldGen.TileFrame(num7, num8, false, false);
					}
				}
				WorldGen.destroyObject = false;
			}
		}
		public static void PlaceMB(int X, int y, int type, int style)
		{
			int num = X + 1;
			if (num < 5 || num > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
			{
				return;
			}
			bool flag = true;
			for (int i = num - 1; i < num + 1; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (Main.tile[i, j].active)
					{
						flag = false;
					}
				}
				if (!Main.tile[i, y + 1].active || (!Main.tileSolid[(int)Main.tile[i, y + 1].type] && !Main.tileTable[(int)Main.tile[i, y + 1].type]))
				{
					flag = false;
				}
			}
			if (flag)
			{
				Main.tile[num - 1, y - 1].active = true;
				Main.tile[num - 1, y - 1].frameY = (short)(style * 36);
				Main.tile[num - 1, y - 1].frameX = 0;
				Main.tile[num - 1, y - 1].type = (byte)type;
				Main.tile[num, y - 1].active = true;
				Main.tile[num, y - 1].frameY = (short)(style * 36);
				Main.tile[num, y - 1].frameX = 18;
				Main.tile[num, y - 1].type = (byte)type;
				Main.tile[num - 1, y].active = true;
				Main.tile[num - 1, y].frameY = (short)(style * 36 + 18);
				Main.tile[num - 1, y].frameX = 0;
				Main.tile[num - 1, y].type = (byte)type;
				Main.tile[num, y].active = true;
				Main.tile[num, y].frameY = (short)(style * 36 + 18);
				Main.tile[num, y].frameX = 18;
				Main.tile[num, y].type = (byte)type;
			}
		}
		public static void Place2x2(int x, int superY, int type)
		{
			int num = superY;
			if (type == 95 || type == 126)
			{
				num++;
			}
			if (x < 5 || x > Main.maxTilesX - 5 || num < 5 || num > Main.maxTilesY - 5)
			{
				return;
			}
			bool flag = true;
			for (int i = x - 1; i < x + 1; i++)
			{
				for (int j = num - 1; j < num + 1; j++)
				{
					if (Main.tile[i, j].active)
					{
						flag = false;
					}
					if (type == 98 && Main.tile[i, j].liquid > 0)
					{
						flag = false;
					}
				}
				if (type == 95 || type == 126)
				{
					if (!Main.tile[i, num - 2].active || !Main.tileSolid[(int)Main.tile[i, num - 2].type] || Main.tileSolidTop[(int)Main.tile[i, num - 2].type])
					{
						flag = false;
					}
				}
				else
				{
					if (!Main.tile[i, num + 1].active || (!Main.tileSolid[(int)Main.tile[i, num + 1].type] && !Main.tileTable[(int)Main.tile[i, num + 1].type]))
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				Main.tile[x - 1, num - 1].active = true;
				Main.tile[x - 1, num - 1].frameY = 0;
				Main.tile[x - 1, num - 1].frameX = 0;
				Main.tile[x - 1, num - 1].type = (byte)type;
				Main.tile[x, num - 1].active = true;
				Main.tile[x, num - 1].frameY = 0;
				Main.tile[x, num - 1].frameX = 18;
				Main.tile[x, num - 1].type = (byte)type;
				Main.tile[x - 1, num].active = true;
				Main.tile[x - 1, num].frameY = 18;
				Main.tile[x - 1, num].frameX = 0;
				Main.tile[x - 1, num].type = (byte)type;
				Main.tile[x, num].active = true;
				Main.tile[x, num].frameY = 18;
				Main.tile[x, num].frameX = 18;
				Main.tile[x, num].type = (byte)type;
			}
		}
		public static void Place3x4(int x, int y, int type)
		{
			if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
			{
				return;
			}
			bool flag = true;
			for (int i = x - 1; i < x + 2; i++)
			{
				for (int j = y - 3; j < y + 1; j++)
				{
					if (Main.tile[i, j].active)
					{
						flag = false;
					}
				}
				if (!Main.tile[i, y + 1].active || !Main.tileSolid[(int)Main.tile[i, y + 1].type])
				{
					flag = false;
				}
			}
			if (flag)
			{
				for (int k = -3; k <= 0; k++)
				{
					short frameY = (short)((3 + k) * 18);
					Main.tile[x - 1, y + k].active = true;
					Main.tile[x - 1, y + k].frameY = frameY;
					Main.tile[x - 1, y + k].frameX = 0;
					Main.tile[x - 1, y + k].type = (byte)type;
					Main.tile[x, y + k].active = true;
					Main.tile[x, y + k].frameY = frameY;
					Main.tile[x, y + k].frameX = 18;
					Main.tile[x, y + k].type = (byte)type;
					Main.tile[x + 1, y + k].active = true;
					Main.tile[x + 1, y + k].frameY = frameY;
					Main.tile[x + 1, y + k].frameX = 36;
					Main.tile[x + 1, y + k].type = (byte)type;
				}
			}
		}
		public static void Place3x2(int x, int y, int type)
		{
			if (x < 5 || x > Main.maxTilesX - 5 || y < 5 || y > Main.maxTilesY - 5)
			{
				return;
			}
			bool flag = true;
			for (int i = x - 1; i < x + 2; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (Main.tile[i, j].active)
					{
						flag = false;
					}
				}
				if (!Main.tile[i, y + 1].active || !Main.tileSolid[(int)Main.tile[i, y + 1].type])
				{
					flag = false;
				}
			}
			if (flag)
			{
				Main.tile[x - 1, y - 1].active = true;
				Main.tile[x - 1, y - 1].frameY = 0;
				Main.tile[x - 1, y - 1].frameX = 0;
				Main.tile[x - 1, y - 1].type = (byte)type;
				Main.tile[x, y - 1].active = true;
				Main.tile[x, y - 1].frameY = 0;
				Main.tile[x, y - 1].frameX = 18;
				Main.tile[x, y - 1].type = (byte)type;
				Main.tile[x + 1, y - 1].active = true;
				Main.tile[x + 1, y - 1].frameY = 0;
				Main.tile[x + 1, y - 1].frameX = 36;
				Main.tile[x + 1, y - 1].type = (byte)type;
				Main.tile[x - 1, y].active = true;
				Main.tile[x - 1, y].frameY = 18;
				Main.tile[x - 1, y].frameX = 0;
				Main.tile[x - 1, y].type = (byte)type;
				Main.tile[x, y].active = true;
				Main.tile[x, y].frameY = 18;
				Main.tile[x, y].frameX = 18;
				Main.tile[x, y].type = (byte)type;
				Main.tile[x + 1, y].active = true;
				Main.tile[x + 1, y].frameY = 18;
				Main.tile[x + 1, y].frameX = 36;
				Main.tile[x + 1, y].type = (byte)type;
			}
		}
		public static void Check3x3(int i, int j, int type)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
			bool flag = false;
			int num = i;
			num = (int)(Main.tile[i, j].frameX / 18);
			int num2 = i - num;
			if (num >= 3)
			{
				num -= 3;
			}
			num = i - num;
			int num3 = j + (int)(Main.tile[i, j].frameY / 18 * -1);
			for (int k = num; k < num + 3; k++)
			{
				for (int l = num3; l < num3 + 3; l++)
				{
					if (!Main.tile[k, l].active || (int)Main.tile[k, l].type != type || (int)Main.tile[k, l].frameX != (k - num2) * 18 || (int)Main.tile[k, l].frameY != (l - num3) * 18)
					{
						flag = true;
					}
				}
			}
			if (type == 106)
			{
				for (int m = num; m < num + 3; m++)
				{
					if (!Main.tile[m, num3 + 3].active || !Main.tileSolid[(int)Main.tile[m, num3 + 3].type])
					{
						flag = true;
						break;
					}
				}
			}
			else
			{
				if (!Main.tile[num + 1, num3 - 1].active || !Main.tileSolid[(int)Main.tile[num + 1, num3 - 1].type] || Main.tileSolidTop[(int)Main.tile[num + 1, num3 - 1].type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				WorldGen.destroyObject = true;
				for (int n = num; n < num + 3; n++)
				{
					for (int num4 = num3; num4 < num3 + 3; num4++)
					{
						if ((int)Main.tile[n, num4].type == type && Main.tile[n, num4].active)
						{
							WorldGen.KillTile(n, num4, false, false, false);
						}
					}
				}
				if (type == 34)
				{
					Item.NewItem(i * 16, j * 16, 32, 32, 106, 1, false, 0);
				}
				else
				{
					if (type == 35)
					{
						Item.NewItem(i * 16, j * 16, 32, 32, 107, 1, false, 0);
					}
					else
					{
						if (type == 36)
						{
							Item.NewItem(i * 16, j * 16, 32, 32, 108, 1, false, 0);
						}
						else
						{
							if (type == 106)
							{
								Item.NewItem(i * 16, j * 16, 32, 32, 363, 1, false, 0);
							}
						}
					}
				}
				WorldGen.destroyObject = false;
				for (int num5 = num - 1; num5 < num + 4; num5++)
				{
					for (int num6 = num3 - 1; num6 < num3 + 4; num6++)
					{
						WorldGen.TileFrame(num5, num6, false, false);
					}
				}
			}
		}
		public static void Place3x3(int x, int y, int type)
		{
			bool flag = true;
			int num = 0;
			if (type == 106)
			{
				num = -2;
				for (int i = x - 1; i < x + 2; i++)
				{
					for (int j = y - 2; j < y + 1; j++)
					{
						if (Main.tile[i, j].active)
						{
							flag = false;
						}
					}
				}
				for (int k = x - 1; k < x + 2; k++)
				{
					if (!Main.tile[k, y + 1].active || !Main.tileSolid[(int)Main.tile[k, y + 1].type])
					{
						flag = false;
						break;
					}
				}
			}
			else
			{
				for (int l = x - 1; l < x + 2; l++)
				{
					for (int m = y; m < y + 3; m++)
					{
						if (Main.tile[l, m].active)
						{
							flag = false;
						}
					}
				}
				if (!Main.tile[x, y - 1].active || !Main.tileSolid[(int)Main.tile[x, y - 1].type] || Main.tileSolidTop[(int)Main.tile[x, y - 1].type])
				{
					flag = false;
				}
			}
			if (flag)
			{
				Main.tile[x - 1, y + num].active = true;
				Main.tile[x - 1, y + num].frameY = 0;
				Main.tile[x - 1, y + num].frameX = 0;
				Main.tile[x - 1, y + num].type = (byte)type;
				Main.tile[x, y + num].active = true;
				Main.tile[x, y + num].frameY = 0;
				Main.tile[x, y + num].frameX = 18;
				Main.tile[x, y + num].type = (byte)type;
				Main.tile[x + 1, y + num].active = true;
				Main.tile[x + 1, y + num].frameY = 0;
				Main.tile[x + 1, y + num].frameX = 36;
				Main.tile[x + 1, y + num].type = (byte)type;
				Main.tile[x - 1, y + 1 + num].active = true;
				Main.tile[x - 1, y + 1 + num].frameY = 18;
				Main.tile[x - 1, y + 1 + num].frameX = 0;
				Main.tile[x - 1, y + 1 + num].type = (byte)type;
				Main.tile[x, y + 1 + num].active = true;
				Main.tile[x, y + 1 + num].frameY = 18;
				Main.tile[x, y + 1 + num].frameX = 18;
				Main.tile[x, y + 1 + num].type = (byte)type;
				Main.tile[x + 1, y + 1 + num].active = true;
				Main.tile[x + 1, y + 1 + num].frameY = 18;
				Main.tile[x + 1, y + 1 + num].frameX = 36;
				Main.tile[x + 1, y + 1 + num].type = (byte)type;
				Main.tile[x - 1, y + 2 + num].active = true;
				Main.tile[x - 1, y + 2 + num].frameY = 36;
				Main.tile[x - 1, y + 2 + num].frameX = 0;
				Main.tile[x - 1, y + 2 + num].type = (byte)type;
				Main.tile[x, y + 2 + num].active = true;
				Main.tile[x, y + 2 + num].frameY = 36;
				Main.tile[x, y + 2 + num].frameX = 18;
				Main.tile[x, y + 2 + num].type = (byte)type;
				Main.tile[x + 1, y + 2 + num].active = true;
				Main.tile[x + 1, y + 2 + num].frameY = 36;
				Main.tile[x + 1, y + 2 + num].frameX = 36;
				Main.tile[x + 1, y + 2 + num].type = (byte)type;
			}
		}
		public static void PlaceSunflower(int x, int y, int type = 27)
		{
			if ((double)y > Main.worldSurface - 1.0)
			{
				return;
			}
			bool flag = true;
			for (int i = x; i < x + 2; i++)
			{
				for (int j = y - 3; j < y + 1; j++)
				{
					if (Main.tile[i, j].active || Main.tile[i, j].wall > 0)
					{
						flag = false;
					}
				}
				if (!Main.tile[i, y + 1].active || (Main.tile[i, y + 1].type != 2 && Main.tile[i, y + 1].type != 109))
				{
					flag = false;
				}
			}
			if (flag)
			{
				for (int k = 0; k < 2; k++)
				{
					for (int l = -3; l < 1; l++)
					{
						int num = k * 18 + WorldGen.genRand.Next(3) * 36;
						int num2 = (l + 3) * 18;
						Main.tile[x + k, y + l].active = true;
						Main.tile[x + k, y + l].frameX = (short)num;
						Main.tile[x + k, y + l].frameY = (short)num2;
						Main.tile[x + k, y + l].type = (byte)type;
					}
				}
			}
		}
		public static void CheckSunflower(int i, int j, int type = 27)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
			bool flag = false;
			int k = 0;
			k += (int)(Main.tile[i, j].frameX / 18);
			int num = j + (int)(Main.tile[i, j].frameY / 18 * -1);
			while (k > 1)
			{
				k -= 2;
			}
			k *= -1;
			k += i;
			for (int l = k; l < k + 2; l++)
			{
				for (int m = num; m < num + 4; m++)
				{
					int n;
					for (n = (int)(Main.tile[l, m].frameX / 18); n > 1; n -= 2)
					{
					}
					if (!Main.tile[l, m].active || (int)Main.tile[l, m].type != type || n != l - k || (int)Main.tile[l, m].frameY != (m - num) * 18)
					{
						flag = true;
					}
				}
				if (!Main.tile[l, num + 4].active || (Main.tile[l, num + 4].type != 2 && Main.tile[l, num + 4].type != 109))
				{
					flag = true;
				}
			}
			if (flag)
			{
				WorldGen.destroyObject = true;
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 4; num3++)
					{
						if ((int)Main.tile[num2, num3].type == type && Main.tile[num2, num3].active)
						{
							WorldGen.KillTile(num2, num3, false, false, false);
						}
					}
				}
				Item.NewItem(i * 16, j * 16, 32, 32, 63, 1, false, 0);
				WorldGen.destroyObject = false;
			}
		}
		public static bool PlacePot(int x, int y, int type = 28)
		{
			bool flag = true;
			for (int i = x; i < x + 2; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (Main.tile[i, j].active)
					{
						flag = false;
					}
				}
				if (!Main.tile[i, y + 1].active || !Main.tileSolid[(int)Main.tile[i, y + 1].type])
				{
					flag = false;
				}
			}
			if (flag)
			{
				for (int k = 0; k < 2; k++)
				{
					for (int l = -1; l < 1; l++)
					{
						int num = k * 18 + WorldGen.genRand.Next(3) * 36;
						int num2 = (l + 1) * 18;
						Main.tile[x + k, y + l].active = true;
						Main.tile[x + k, y + l].frameX = (short)num;
						Main.tile[x + k, y + l].frameY = (short)num2;
						Main.tile[x + k, y + l].type = (byte)type;
					}
				}
				return true;
			}
			return false;
		}
		public static bool CheckCactus(int i, int j)
		{
			int num = j;
			int num2 = i;
			while (Main.tile[num2, num].active && Main.tile[num2, num].type == 80)
			{
				num++;
				if (!Main.tile[num2, num].active || Main.tile[num2, num].type != 80)
				{
					if (Main.tile[num2 - 1, num].active && Main.tile[num2 - 1, num].type == 80 && Main.tile[num2 - 1, num - 1].active && Main.tile[num2 - 1, num - 1].type == 80 && num2 >= i)
					{
						num2--;
					}
					if (Main.tile[num2 + 1, num].active && Main.tile[num2 + 1, num].type == 80 && Main.tile[num2 + 1, num - 1].active && Main.tile[num2 + 1, num - 1].type == 80 && num2 <= i)
					{
						num2++;
					}
				}
			}
			if (!Main.tile[num2, num].active || (Main.tile[num2, num].type != 53 && Main.tile[num2, num].type != 112 && Main.tile[num2, num].type != 116))
			{
				WorldGen.KillTile(i, j, false, false, false);
				return true;
			}
			if (i != num2)
			{
				if ((!Main.tile[i, j + 1].active || Main.tile[i, j + 1].type != 80) && (!Main.tile[i - 1, j].active || Main.tile[i - 1, j].type != 80) && (!Main.tile[i + 1, j].active || Main.tile[i + 1, j].type != 80))
				{
					WorldGen.KillTile(i, j, false, false, false);
					return true;
				}
			}
			else
			{
				if (i == num2 && (!Main.tile[i, j + 1].active || (Main.tile[i, j + 1].type != 80 && Main.tile[i, j + 1].type != 53 && Main.tile[i, j + 1].type != 112 && Main.tile[i, j + 1].type != 116)))
				{
					WorldGen.KillTile(i, j, false, false, false);
					return true;
				}
			}
			return false;
		}
		public static void PlantCactus(int i, int j)
		{
			WorldGen.GrowCactus(i, j);
			for (int k = 0; k < 150; k++)
			{
				int i2 = WorldGen.genRand.Next(i - 1, i + 2);
				int j2 = WorldGen.genRand.Next(j - 10, j + 2);
				WorldGen.GrowCactus(i2, j2);
			}
		}
		public static void CheckOrb(int i, int j, int type)
		{
			if (!WorldGen.destroyObject)
			{
				int num = i;
				int num2 = j;
				if (Main.tile[i, j].frameX == 0)
				{
					num = i;
				}
				else
				{
					num = i - 1;
				}
				if (Main.tile[i, j].frameY == 0)
				{
					num2 = j;
				}
				else
				{
					num2 = j - 1;
				}
				if (Main.tile[num, num2] != null && Main.tile[num + 1, num2] != null && Main.tile[num, num2 + 1] != null && Main.tile[num + 1, num2 + 1] != null && (!Main.tile[num, num2].active || (int)Main.tile[num, num2].type != type || !Main.tile[num + 1, num2].active || (int)Main.tile[num + 1, num2].type != type || !Main.tile[num, num2 + 1].active || (int)Main.tile[num, num2 + 1].type != type || !Main.tile[num + 1, num2 + 1].active || (int)Main.tile[num + 1, num2 + 1].type != type))
				{
					WorldGen.destroyObject = true;
					if ((int)Main.tile[num, num2].type == type)
					{
						WorldGen.KillTile(num, num2, false, false, false);
					}
					if ((int)Main.tile[num + 1, num2].type == type)
					{
						WorldGen.KillTile(num + 1, num2, false, false, false);
					}
					if ((int)Main.tile[num, num2 + 1].type == type)
					{
						WorldGen.KillTile(num, num2 + 1, false, false, false);
					}
					if ((int)Main.tile[num + 1, num2 + 1].type == type)
					{
						WorldGen.KillTile(num + 1, num2 + 1, false, false, false);
					}
					if (Main.netMode != 1 && !WorldGen.noTileActions)
					{
						if (type == 12)
						{
							Item.NewItem(num * 16, num2 * 16, 32, 32, 29, 1, false, 0);
						}
						else
						{
							if (type == 31)
							{
								if (WorldGen.genRand.Next(2) == 0)
								{
									WorldGen.spawnMeteor = true;
								}
								int num3 = Main.rand.Next(5);
								if (!WorldGen.shadowOrbSmashed)
								{
									num3 = 0;
								}
								if (num3 == 0)
								{
									Item.NewItem(num * 16, num2 * 16, 32, 32, 96, 1, false, -1);
									int stack = WorldGen.genRand.Next(25, 51);
									Item.NewItem(num * 16, num2 * 16, 32, 32, 97, stack, false, 0);
								}
								else
								{
									if (num3 == 1)
									{
										Item.NewItem(num * 16, num2 * 16, 32, 32, 64, 1, false, -1);
									}
									else
									{
										if (num3 == 2)
										{
											Item.NewItem(num * 16, num2 * 16, 32, 32, 162, 1, false, -1);
										}
										else
										{
											if (num3 == 3)
											{
												Item.NewItem(num * 16, num2 * 16, 32, 32, 115, 1, false, -1);
											}
											else
											{
												if (num3 == 4)
												{
													Item.NewItem(num * 16, num2 * 16, 32, 32, 111, 1, false, -1);
												}
											}
										}
									}
								}
								WorldGen.shadowOrbSmashed = true;
								WorldGen.shadowOrbCount++;
								if (WorldGen.shadowOrbCount >= 3)
								{
									WorldGen.shadowOrbCount = 0;
									float num4 = (float)(num * 16);
									float num5 = (float)(num2 * 16);
									float num6 = -1f;
									int num7 = 0;
									for (int k = 0; k < 255; k++)
									{
										float num8 = Math.Abs(Main.player[k].position.X - num4) + Math.Abs(Main.player[k].position.Y - num5);
										if (num8 < num6 || num6 == -1f)
										{
											num7 = 0;
											num6 = num8;
										}
									}
									if (Main.player[num7].zoneEvil)
									{
										NPC.SpawnOnPlayer(num7, 13);
									}
								}
								else
								{
									string text = "A horrible chill goes down your spine...";
									if (WorldGen.shadowOrbCount == 2)
									{
										text = "Screams echo around you...";
									}
									if (Main.netMode == 0)
									{
										Main.NewText(text, 50, 255, 130);
									}
									else
									{
										if (Main.netMode == 2)
										{
											NetMessage.SendData(25, -1, -1, text, 255, 50f, 255f, 130f, 0);
										}
									}
								}
							}
						}
					}
					Main.PlaySound(13, i * 16, j * 16, 1);
					WorldGen.destroyObject = false;
				}
			}
		}
		public static void CheckTree(int i, int j)
		{
			int num = -1;
			int num2 = -1;
			int num3 = -1;
			int num4 = -1;
			int num5 = -1;
			int num6 = -1;
			int num7 = -1;
			int num8 = -1;
			int type = (int)Main.tile[i, j].type;
			int frameX = (int)Main.tile[i, j].frameX;
			int frameY = (int)Main.tile[i, j].frameY;
			if (Main.tile[i - 1, j] != null && Main.tile[i - 1, j].active)
			{
				num4 = (int)Main.tile[i - 1, j].type;
			}
			if (Main.tile[i + 1, j] != null && Main.tile[i + 1, j].active)
			{
				num5 = (int)Main.tile[i + 1, j].type;
			}
			if (Main.tile[i, j - 1] != null && Main.tile[i, j - 1].active)
			{
				num2 = (int)Main.tile[i, j - 1].type;
			}
			if (Main.tile[i, j + 1] != null && Main.tile[i, j + 1].active)
			{
				num7 = (int)Main.tile[i, j + 1].type;
			}
			if (Main.tile[i - 1, j - 1] != null && Main.tile[i - 1, j - 1].active)
			{
				num = (int)Main.tile[i - 1, j - 1].type;
			}
			if (Main.tile[i + 1, j - 1] != null && Main.tile[i + 1, j - 1].active)
			{
				num3 = (int)Main.tile[i + 1, j - 1].type;
			}
			if (Main.tile[i - 1, j + 1] != null && Main.tile[i - 1, j + 1].active)
			{
				num6 = (int)Main.tile[i - 1, j + 1].type;
			}
			if (Main.tile[i + 1, j + 1] != null && Main.tile[i + 1, j + 1].active)
			{
				num8 = (int)Main.tile[i + 1, j + 1].type;
			}
			if (num4 >= 0 && Main.tileStone[num4])
			{
				num4 = 1;
			}
			if (num5 >= 0 && Main.tileStone[num5])
			{
				num5 = 1;
			}
			if (num2 >= 0 && Main.tileStone[num2])
			{
				num2 = 1;
			}
			if (num7 >= 0 && Main.tileStone[num7])
			{
				num7 = 1;
			}
			if (num >= 0 && Main.tileStone[num])
			{
				num = 1;
			}
			if (num3 >= 0 && Main.tileStone[num3])
			{
				num3 = 1;
			}
			if (num6 >= 0 && Main.tileStone[num6])
			{
				num6 = 1;
			}
			if (num8 >= 0 && Main.tileStone[num8])
			{
				num8 = 1;
			}
			if (num7 == 23)
			{
				num7 = 2;
			}
			if (num7 == 60)
			{
				num7 = 2;
			}
			if (num7 == 109)
			{
				num7 = 2;
			}
			if (num7 == 147)
			{
				num7 = 2;
			}
			if (Main.tile[i, j].frameX >= 22 && Main.tile[i, j].frameX <= 44 && Main.tile[i, j].frameY >= 132 && Main.tile[i, j].frameY <= 176)
			{
				if (num7 != 2)
				{
					WorldGen.KillTile(i, j, false, false, false);
				}
				else
				{
					if ((Main.tile[i, j].frameX != 22 || num4 != type) && (Main.tile[i, j].frameX != 44 || num5 != type))
					{
						WorldGen.KillTile(i, j, false, false, false);
					}
				}
			}
			else
			{
				if ((Main.tile[i, j].frameX == 88 && Main.tile[i, j].frameY >= 0 && Main.tile[i, j].frameY <= 44) || (Main.tile[i, j].frameX == 66 && Main.tile[i, j].frameY >= 66 && Main.tile[i, j].frameY <= 130) || (Main.tile[i, j].frameX == 110 && Main.tile[i, j].frameY >= 66 && Main.tile[i, j].frameY <= 110) || (Main.tile[i, j].frameX == 132 && Main.tile[i, j].frameY >= 0 && Main.tile[i, j].frameY <= 176))
				{
					if (num4 == type && num5 == type)
					{
						if (Main.tile[i, j].frameNumber == 0)
						{
							Main.tile[i, j].frameX = 110;
							Main.tile[i, j].frameY = 66;
						}
						if (Main.tile[i, j].frameNumber == 1)
						{
							Main.tile[i, j].frameX = 110;
							Main.tile[i, j].frameY = 88;
						}
						if (Main.tile[i, j].frameNumber == 2)
						{
							Main.tile[i, j].frameX = 110;
							Main.tile[i, j].frameY = 110;
						}
					}
					else
					{
						if (num4 == type)
						{
							if (Main.tile[i, j].frameNumber == 0)
							{
								Main.tile[i, j].frameX = 88;
								Main.tile[i, j].frameY = 0;
							}
							if (Main.tile[i, j].frameNumber == 1)
							{
								Main.tile[i, j].frameX = 88;
								Main.tile[i, j].frameY = 22;
							}
							if (Main.tile[i, j].frameNumber == 2)
							{
								Main.tile[i, j].frameX = 88;
								Main.tile[i, j].frameY = 44;
							}
						}
						else
						{
							if (num5 == type)
							{
								if (Main.tile[i, j].frameNumber == 0)
								{
									Main.tile[i, j].frameX = 66;
									Main.tile[i, j].frameY = 66;
								}
								if (Main.tile[i, j].frameNumber == 1)
								{
									Main.tile[i, j].frameX = 66;
									Main.tile[i, j].frameY = 88;
								}
								if (Main.tile[i, j].frameNumber == 2)
								{
									Main.tile[i, j].frameX = 66;
									Main.tile[i, j].frameY = 110;
								}
							}
							else
							{
								if (Main.tile[i, j].frameNumber == 0)
								{
									Main.tile[i, j].frameX = 0;
									Main.tile[i, j].frameY = 0;
								}
								if (Main.tile[i, j].frameNumber == 1)
								{
									Main.tile[i, j].frameX = 0;
									Main.tile[i, j].frameY = 22;
								}
								if (Main.tile[i, j].frameNumber == 2)
								{
									Main.tile[i, j].frameX = 0;
									Main.tile[i, j].frameY = 44;
								}
							}
						}
					}
				}
			}
			if (Main.tile[i, j].frameY >= 132 && Main.tile[i, j].frameY <= 176 && (Main.tile[i, j].frameX == 0 || Main.tile[i, j].frameX == 66 || Main.tile[i, j].frameX == 88))
			{
				if (num7 != 2)
				{
					WorldGen.KillTile(i, j, false, false, false);
				}
				if (num4 != type && num5 != type)
				{
					if (Main.tile[i, j].frameNumber == 0)
					{
						Main.tile[i, j].frameX = 0;
						Main.tile[i, j].frameY = 0;
					}
					if (Main.tile[i, j].frameNumber == 1)
					{
						Main.tile[i, j].frameX = 0;
						Main.tile[i, j].frameY = 22;
					}
					if (Main.tile[i, j].frameNumber == 2)
					{
						Main.tile[i, j].frameX = 0;
						Main.tile[i, j].frameY = 44;
					}
				}
				else
				{
					if (num4 != type)
					{
						if (Main.tile[i, j].frameNumber == 0)
						{
							Main.tile[i, j].frameX = 0;
							Main.tile[i, j].frameY = 132;
						}
						if (Main.tile[i, j].frameNumber == 1)
						{
							Main.tile[i, j].frameX = 0;
							Main.tile[i, j].frameY = 154;
						}
						if (Main.tile[i, j].frameNumber == 2)
						{
							Main.tile[i, j].frameX = 0;
							Main.tile[i, j].frameY = 176;
						}
					}
					else
					{
						if (num5 != type)
						{
							if (Main.tile[i, j].frameNumber == 0)
							{
								Main.tile[i, j].frameX = 66;
								Main.tile[i, j].frameY = 132;
							}
							if (Main.tile[i, j].frameNumber == 1)
							{
								Main.tile[i, j].frameX = 66;
								Main.tile[i, j].frameY = 154;
							}
							if (Main.tile[i, j].frameNumber == 2)
							{
								Main.tile[i, j].frameX = 66;
								Main.tile[i, j].frameY = 176;
							}
						}
						else
						{
							if (Main.tile[i, j].frameNumber == 0)
							{
								Main.tile[i, j].frameX = 88;
								Main.tile[i, j].frameY = 132;
							}
							if (Main.tile[i, j].frameNumber == 1)
							{
								Main.tile[i, j].frameX = 88;
								Main.tile[i, j].frameY = 154;
							}
							if (Main.tile[i, j].frameNumber == 2)
							{
								Main.tile[i, j].frameX = 88;
								Main.tile[i, j].frameY = 176;
							}
						}
					}
				}
			}
			if ((Main.tile[i, j].frameX == 66 && (Main.tile[i, j].frameY == 0 || Main.tile[i, j].frameY == 22 || Main.tile[i, j].frameY == 44)) || (Main.tile[i, j].frameX == 44 && (Main.tile[i, j].frameY == 198 || Main.tile[i, j].frameY == 220 || Main.tile[i, j].frameY == 242)))
			{
				if (num5 != type)
				{
					WorldGen.KillTile(i, j, false, false, false);
				}
			}
			else
			{
				if ((Main.tile[i, j].frameX == 88 && (Main.tile[i, j].frameY == 66 || Main.tile[i, j].frameY == 88 || Main.tile[i, j].frameY == 110)) || (Main.tile[i, j].frameX == 66 && (Main.tile[i, j].frameY == 198 || Main.tile[i, j].frameY == 220 || Main.tile[i, j].frameY == 242)))
				{
					if (num4 != type)
					{
						WorldGen.KillTile(i, j, false, false, false);
					}
				}
				else
				{
					if (num7 == -1 || num7 == 23)
					{
						WorldGen.KillTile(i, j, false, false, false);
					}
					else
					{
						if (num2 != type && Main.tile[i, j].frameY < 198 && ((Main.tile[i, j].frameX != 22 && Main.tile[i, j].frameX != 44) || Main.tile[i, j].frameY < 132))
						{
							if (num4 == type || num5 == type)
							{
								if (num7 == type)
								{
									if (num4 == type && num5 == type)
									{
										if (Main.tile[i, j].frameNumber == 0)
										{
											Main.tile[i, j].frameX = 132;
											Main.tile[i, j].frameY = 132;
										}
										if (Main.tile[i, j].frameNumber == 1)
										{
											Main.tile[i, j].frameX = 132;
											Main.tile[i, j].frameY = 154;
										}
										if (Main.tile[i, j].frameNumber == 2)
										{
											Main.tile[i, j].frameX = 132;
											Main.tile[i, j].frameY = 176;
										}
									}
									else
									{
										if (num4 == type)
										{
											if (Main.tile[i, j].frameNumber == 0)
											{
												Main.tile[i, j].frameX = 132;
												Main.tile[i, j].frameY = 0;
											}
											if (Main.tile[i, j].frameNumber == 1)
											{
												Main.tile[i, j].frameX = 132;
												Main.tile[i, j].frameY = 22;
											}
											if (Main.tile[i, j].frameNumber == 2)
											{
												Main.tile[i, j].frameX = 132;
												Main.tile[i, j].frameY = 44;
											}
										}
										else
										{
											if (num5 == type)
											{
												if (Main.tile[i, j].frameNumber == 0)
												{
													Main.tile[i, j].frameX = 132;
													Main.tile[i, j].frameY = 66;
												}
												if (Main.tile[i, j].frameNumber == 1)
												{
													Main.tile[i, j].frameX = 132;
													Main.tile[i, j].frameY = 88;
												}
												if (Main.tile[i, j].frameNumber == 2)
												{
													Main.tile[i, j].frameX = 132;
													Main.tile[i, j].frameY = 110;
												}
											}
										}
									}
								}
								else
								{
									if (num4 == type && num5 == type)
									{
										if (Main.tile[i, j].frameNumber == 0)
										{
											Main.tile[i, j].frameX = 154;
											Main.tile[i, j].frameY = 132;
										}
										if (Main.tile[i, j].frameNumber == 1)
										{
											Main.tile[i, j].frameX = 154;
											Main.tile[i, j].frameY = 154;
										}
										if (Main.tile[i, j].frameNumber == 2)
										{
											Main.tile[i, j].frameX = 154;
											Main.tile[i, j].frameY = 176;
										}
									}
									else
									{
										if (num4 == type)
										{
											if (Main.tile[i, j].frameNumber == 0)
											{
												Main.tile[i, j].frameX = 154;
												Main.tile[i, j].frameY = 0;
											}
											if (Main.tile[i, j].frameNumber == 1)
											{
												Main.tile[i, j].frameX = 154;
												Main.tile[i, j].frameY = 22;
											}
											if (Main.tile[i, j].frameNumber == 2)
											{
												Main.tile[i, j].frameX = 154;
												Main.tile[i, j].frameY = 44;
											}
										}
										else
										{
											if (num5 == type)
											{
												if (Main.tile[i, j].frameNumber == 0)
												{
													Main.tile[i, j].frameX = 154;
													Main.tile[i, j].frameY = 66;
												}
												if (Main.tile[i, j].frameNumber == 1)
												{
													Main.tile[i, j].frameX = 154;
													Main.tile[i, j].frameY = 88;
												}
												if (Main.tile[i, j].frameNumber == 2)
												{
													Main.tile[i, j].frameX = 154;
													Main.tile[i, j].frameY = 110;
												}
											}
										}
									}
								}
							}
							else
							{
								if (Main.tile[i, j].frameNumber == 0)
								{
									Main.tile[i, j].frameX = 110;
									Main.tile[i, j].frameY = 0;
								}
								if (Main.tile[i, j].frameNumber == 1)
								{
									Main.tile[i, j].frameX = 110;
									Main.tile[i, j].frameY = 22;
								}
								if (Main.tile[i, j].frameNumber == 2)
								{
									Main.tile[i, j].frameX = 110;
									Main.tile[i, j].frameY = 44;
								}
							}
						}
					}
				}
			}
			if ((int)Main.tile[i, j].frameX != frameX && (int)Main.tile[i, j].frameY != frameY && frameX >= 0 && frameY >= 0)
			{
				WorldGen.TileFrame(i - 1, j, false, false);
				WorldGen.TileFrame(i + 1, j, false, false);
				WorldGen.TileFrame(i, j - 1, false, false);
				WorldGen.TileFrame(i, j + 1, false, false);
			}
		}
		public static void CactusFrame(int i, int j)
		{
			try
			{
				int num = j;
				int num2 = i;
				if (!WorldGen.CheckCactus(i, j))
				{
					while (Main.tile[num2, num].active && Main.tile[num2, num].type == 80)
					{
						num++;
						if (!Main.tile[num2, num].active || Main.tile[num2, num].type != 80)
						{
							if (Main.tile[num2 - 1, num].active && Main.tile[num2 - 1, num].type == 80 && Main.tile[num2 - 1, num - 1].active && Main.tile[num2 - 1, num - 1].type == 80 && num2 >= i)
							{
								num2--;
							}
							if (Main.tile[num2 + 1, num].active && Main.tile[num2 + 1, num].type == 80 && Main.tile[num2 + 1, num - 1].active && Main.tile[num2 + 1, num - 1].type == 80 && num2 <= i)
							{
								num2++;
							}
						}
					}
					num--;
					int num3 = i - num2;
					num2 = i;
					num = j;
					int type = (int)Main.tile[i - 2, j].type;
					int num4 = (int)Main.tile[i - 1, j].type;
					int num5 = (int)Main.tile[i + 1, j].type;
					int num6 = (int)Main.tile[i, j - 1].type;
					int num7 = (int)Main.tile[i, j + 1].type;
					int num8 = (int)Main.tile[i - 1, j + 1].type;
					int num9 = (int)Main.tile[i + 1, j + 1].type;
					if (!Main.tile[i - 1, j].active)
					{
						num4 = -1;
					}
					if (!Main.tile[i + 1, j].active)
					{
						num5 = -1;
					}
					if (!Main.tile[i, j - 1].active)
					{
						num6 = -1;
					}
					if (!Main.tile[i, j + 1].active)
					{
						num7 = -1;
					}
					if (!Main.tile[i - 1, j + 1].active)
					{
						num8 = -1;
					}
					if (!Main.tile[i + 1, j + 1].active)
					{
						num9 = -1;
					}
					short num10 = Main.tile[i, j].frameX;
					short num11 = Main.tile[i, j].frameY;
					if (num3 == 0)
					{
						if (num6 != 80)
						{
							if (num4 == 80 && num5 == 80 && num8 != 80 && num9 != 80 && type != 80)
							{
								num10 = 90;
								num11 = 0;
							}
							else
							{
								if (num4 == 80 && num8 != 80 && type != 80)
								{
									num10 = 72;
									num11 = 0;
								}
								else
								{
									if (num5 == 80 && num9 != 80)
									{
										num10 = 18;
										num11 = 0;
									}
									else
									{
										num10 = 0;
										num11 = 0;
									}
								}
							}
						}
						else
						{
							if (num4 == 80 && num5 == 80 && num8 != 80 && num9 != 80 && type != 80)
							{
								num10 = 90;
								num11 = 36;
							}
							else
							{
								if (num4 == 80 && num8 != 80 && type != 80)
								{
									num10 = 72;
									num11 = 36;
								}
								else
								{
									if (num5 == 80 && num9 != 80)
									{
										num10 = 18;
										num11 = 36;
									}
									else
									{
										if (num7 >= 0 && Main.tileSolid[num7])
										{
											num10 = 0;
											num11 = 36;
										}
										else
										{
											num10 = 0;
											num11 = 18;
										}
									}
								}
							}
						}
					}
					else
					{
						if (num3 == -1)
						{
							if (num5 == 80)
							{
								if (num6 != 80 && num7 != 80)
								{
									num10 = 108;
									num11 = 36;
								}
								else
								{
									if (num7 != 80)
									{
										num10 = 54;
										num11 = 36;
									}
									else
									{
										if (num6 != 80)
										{
											num10 = 54;
											num11 = 0;
										}
										else
										{
											num10 = 54;
											num11 = 18;
										}
									}
								}
							}
							else
							{
								if (num6 != 80)
								{
									num10 = 54;
									num11 = 0;
								}
								else
								{
									num10 = 54;
									num11 = 18;
								}
							}
						}
						else
						{
							if (num3 == 1)
							{
								if (num4 == 80)
								{
									if (num6 != 80 && num7 != 80)
									{
										num10 = 108;
										num11 = 16;
									}
									else
									{
										if (num7 != 80)
										{
											num10 = 36;
											num11 = 36;
										}
										else
										{
											if (num6 != 80)
											{
												num10 = 36;
												num11 = 0;
											}
											else
											{
												num10 = 36;
												num11 = 18;
											}
										}
									}
								}
								else
								{
									if (num6 != 80)
									{
										num10 = 36;
										num11 = 0;
									}
									else
									{
										num10 = 36;
										num11 = 18;
									}
								}
							}
						}
					}
					if (num10 != Main.tile[i, j].frameX || num11 != Main.tile[i, j].frameY)
					{
						Main.tile[i, j].frameX = num10;
						Main.tile[i, j].frameY = num11;
						WorldGen.SquareTileFrame(i, j, true);
					}
				}
			}
			catch
			{
				Main.tile[i, j].frameX = 0;
				Main.tile[i, j].frameY = 0;
			}
		}
		public static void GrowCactus(int i, int j)
		{
			int num = j;
			int num2 = i;
			if (!Main.tile[i, j].active)
			{
				return;
			}
			if (Main.tile[i, j - 1].liquid > 0)
			{
				return;
			}
			if (Main.tile[i, j].type != 53 && Main.tile[i, j].type != 80 && Main.tile[i, j].type != 112 && Main.tile[i, j].type != 116)
			{
				return;
			}
			if (Main.tile[i, j].type == 53 || Main.tile[i, j].type == 112 || Main.tile[i, j].type == 116)
			{
				if (Main.tile[i, j - 1].active || Main.tile[i - 1, j - 1].active || Main.tile[i + 1, j - 1].active)
				{
					return;
				}
				int num3 = 0;
				int num4 = 0;
				for (int k = i - 6; k <= i + 6; k++)
				{
					for (int l = j - 3; l <= j + 1; l++)
					{
						try
						{
							if (Main.tile[k, l].active)
							{
								if (Main.tile[k, l].type == 80)
								{
									num3++;
									if (num3 >= 4)
									{
										return;
									}
								}
								if (Main.tile[k, l].type == 53 || Main.tile[k, l].type == 112 || Main.tile[k, l].type == 116)
								{
									num4++;
								}
							}
						}
						catch
						{
						}
					}
				}
				if (num4 > 10)
				{
					Main.tile[i, j - 1].active = true;
					Main.tile[i, j - 1].type = 80;
					if (Main.netMode == 2)
					{
						NetMessage.SendTileSquare(-1, i, j - 1, 1);
					}
					WorldGen.SquareTileFrame(num2, num - 1, true);
					return;
				}
				return;
			}
			else
			{
				if (Main.tile[i, j].type != 80)
				{
					return;
				}
				while (Main.tile[num2, num].active && Main.tile[num2, num].type == 80)
				{
					num++;
					if (!Main.tile[num2, num].active || Main.tile[num2, num].type != 80)
					{
						if (Main.tile[num2 - 1, num].active && Main.tile[num2 - 1, num].type == 80 && Main.tile[num2 - 1, num - 1].active && Main.tile[num2 - 1, num - 1].type == 80 && num2 >= i)
						{
							num2--;
						}
						if (Main.tile[num2 + 1, num].active && Main.tile[num2 + 1, num].type == 80 && Main.tile[num2 + 1, num - 1].active && Main.tile[num2 + 1, num - 1].type == 80 && num2 <= i)
						{
							num2++;
						}
					}
				}
				num--;
				int num5 = num - j;
				int num6 = i - num2;
				num2 = i - num6;
				num = j;
				int num7 = 11 - num5;
				int num8 = 0;
				for (int m = num2 - 2; m <= num2 + 2; m++)
				{
					for (int n = num - num7; n <= num + num5; n++)
					{
						if (Main.tile[m, n].active && Main.tile[m, n].type == 80)
						{
							num8++;
						}
					}
				}
				if (num8 < WorldGen.genRand.Next(11, 13))
				{
					num2 = i;
					num = j;
					if (num6 == 0)
					{
						if (num5 == 0)
						{
							if (Main.tile[num2, num - 1].active)
							{
								return;
							}
							Main.tile[num2, num - 1].active = true;
							Main.tile[num2, num - 1].type = 80;
							WorldGen.SquareTileFrame(num2, num - 1, true);
							if (Main.netMode == 2)
							{
								NetMessage.SendTileSquare(-1, num2, num - 1, 1);
								return;
							}
							return;
						}
						else
						{
							bool flag = false;
							bool flag2 = false;
							if (Main.tile[num2, num - 1].active && Main.tile[num2, num - 1].type == 80)
							{
								if (!Main.tile[num2 - 1, num].active && !Main.tile[num2 - 2, num + 1].active && !Main.tile[num2 - 1, num - 1].active && !Main.tile[num2 - 1, num + 1].active && !Main.tile[num2 - 2, num].active)
								{
									flag = true;
								}
								if (!Main.tile[num2 + 1, num].active && !Main.tile[num2 + 2, num + 1].active && !Main.tile[num2 + 1, num - 1].active && !Main.tile[num2 + 1, num + 1].active && !Main.tile[num2 + 2, num].active)
								{
									flag2 = true;
								}
							}
							int num9 = WorldGen.genRand.Next(3);
							if (num9 == 0 && flag)
							{
								Main.tile[num2 - 1, num].active = true;
								Main.tile[num2 - 1, num].type = 80;
								WorldGen.SquareTileFrame(num2 - 1, num, true);
								if (Main.netMode == 2)
								{
									NetMessage.SendTileSquare(-1, num2 - 1, num, 1);
									return;
								}
								return;
							}
							else
							{
								if (num9 == 1 && flag2)
								{
									Main.tile[num2 + 1, num].active = true;
									Main.tile[num2 + 1, num].type = 80;
									WorldGen.SquareTileFrame(num2 + 1, num, true);
									if (Main.netMode == 2)
									{
										NetMessage.SendTileSquare(-1, num2 + 1, num, 1);
										return;
									}
									return;
								}
								else
								{
									if (num5 >= WorldGen.genRand.Next(2, 8))
									{
										return;
									}
									if (Main.tile[num2 - 1, num - 1].active)
									{
										byte arg_66E_0 = Main.tile[num2 - 1, num - 1].type;
									}
									if (Main.tile[num2 + 1, num - 1].active && Main.tile[num2 + 1, num - 1].type == 80)
									{
										return;
									}
									if (Main.tile[num2, num - 1].active)
									{
										return;
									}
									Main.tile[num2, num - 1].active = true;
									Main.tile[num2, num - 1].type = 80;
									WorldGen.SquareTileFrame(num2, num - 1, true);
									if (Main.netMode == 2)
									{
										NetMessage.SendTileSquare(-1, num2, num - 1, 1);
										return;
									}
									return;
								}
							}
						}
					}
					else
					{
						if (Main.tile[num2, num - 1].active || Main.tile[num2, num - 2].active || Main.tile[num2 + num6, num - 1].active || !Main.tile[num2 - num6, num - 1].active || Main.tile[num2 - num6, num - 1].type != 80)
						{
							return;
						}
						Main.tile[num2, num - 1].active = true;
						Main.tile[num2, num - 1].type = 80;
						WorldGen.SquareTileFrame(num2, num - 1, true);
						if (Main.netMode == 2)
						{
							NetMessage.SendTileSquare(-1, num2, num - 1, 1);
							return;
						}
						return;
					}
				}
			}
		}
		public static void CheckPot(int i, int j, int type = 28)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
			bool flag = false;
			int k = 0;
			k += (int)(Main.tile[i, j].frameX / 18);
			int num = j + (int)(Main.tile[i, j].frameY / 18 * -1);
			while (k > 1)
			{
				k -= 2;
			}
			k *= -1;
			k += i;
			for (int l = k; l < k + 2; l++)
			{
				for (int m = num; m < num + 2; m++)
				{

					int n;
					for (n = (int)(Main.tile[l, m].frameX / 18); n > 1; n -= 2)
					{
					}
					if (!Main.tile[l, m].active || (int)Main.tile[l, m].type != type || n != l - k || (int)Main.tile[l, m].frameY != (m - num) * 18)
					{
						flag = true;
					}
				}
				if (!Main.tile[l, num + 2].active || !Main.tileSolid[(int)Main.tile[l, num + 2].type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				WorldGen.destroyObject = true;
				Main.PlaySound(13, i * 16, j * 16, 1);
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 2; num3++)
					{
						if ((int)Main.tile[num2, num3].type == type && Main.tile[num2, num3].active)
						{
							WorldGen.KillTile(num2, num3, false, false, false);
						}
					}
				}
				Gore.NewGore(new Vector2((float)(i * 16), (float)(j * 16)), default(Vector2), 51, 1f);
				Gore.NewGore(new Vector2((float)(i * 16), (float)(j * 16)), default(Vector2), 52, 1f);
				Gore.NewGore(new Vector2((float)(i * 16), (float)(j * 16)), default(Vector2), 53, 1f);
				if (WorldGen.genRand.Next(40) == 0 && (Main.tile[k, num].wall == 7 || Main.tile[k, num].wall == 8 || Main.tile[k, num].wall == 9))
				{
					Item.NewItem(i * 16, j * 16, 16, 16, 327, 1, false, 0);
				}
				else
				{
					if (WorldGen.genRand.Next(45) == 0)
					{
						if ((double)j < Main.worldSurface)
						{
							int num4 = WorldGen.genRand.Next(4);
							if (num4 == 0)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 292, 1, false, 0);
							}
							if (num4 == 1)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 298, 1, false, 0);
							}
							if (num4 == 2)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 299, 1, false, 0);
							}
							if (num4 == 3)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 290, 1, false, 0);
							}
						}
						else
						{
							if ((double)j < Main.rockLayer)
							{
								int num5 = WorldGen.genRand.Next(7);
								if (num5 == 0)
								{
									Item.NewItem(i * 16, j * 16, 16, 16, 289, 1, false, 0);
								}
								if (num5 == 1)
								{
									Item.NewItem(i * 16, j * 16, 16, 16, 298, 1, false, 0);
								}
								if (num5 == 2)
								{
									Item.NewItem(i * 16, j * 16, 16, 16, 299, 1, false, 0);
								}
								if (num5 == 3)
								{
									Item.NewItem(i * 16, j * 16, 16, 16, 290, 1, false, 0);
								}
								if (num5 == 4)
								{
									Item.NewItem(i * 16, j * 16, 16, 16, 303, 1, false, 0);
								}
								if (num5 == 5)
								{
									Item.NewItem(i * 16, j * 16, 16, 16, 291, 1, false, 0);
								}
								if (num5 == 6)
								{
									Item.NewItem(i * 16, j * 16, 16, 16, 304, 1, false, 0);
								}
							}
							else
							{
								if (j < Main.maxTilesY - 200)
								{
									int num6 = WorldGen.genRand.Next(10);
									if (num6 == 0)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 296, 1, false, 0);
									}
									if (num6 == 1)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 295, 1, false, 0);
									}
									if (num6 == 2)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 299, 1, false, 0);
									}
									if (num6 == 3)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 302, 1, false, 0);
									}
									if (num6 == 4)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 303, 1, false, 0);
									}
									if (num6 == 5)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 305, 1, false, 0);
									}
									if (num6 == 6)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 301, 1, false, 0);
									}
									if (num6 == 7)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 302, 1, false, 0);
									}
									if (num6 == 8)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 297, 1, false, 0);
									}
									if (num6 == 9)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 304, 1, false, 0);
									}
								}
								else
								{
									int num7 = WorldGen.genRand.Next(12);
									if (num7 == 0)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 296, 1, false, 0);
									}
									if (num7 == 1)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 295, 1, false, 0);
									}
									if (num7 == 2)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 293, 1, false, 0);
									}
									if (num7 == 3)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 288, 1, false, 0);
									}
									if (num7 == 4)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 294, 1, false, 0);
									}
									if (num7 == 5)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 297, 1, false, 0);
									}
									if (num7 == 6)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 304, 1, false, 0);
									}
									if (num7 == 7)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 305, 1, false, 0);
									}
									if (num7 == 8)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 301, 1, false, 0);
									}
									if (num7 == 9)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 302, 1, false, 0);
									}
									if (num7 == 10)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 288, 1, false, 0);
									}
									if (num7 == 11)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 300, 1, false, 0);
									}
								}
							}
						}
					}
					else
					{
						int num8 = Main.rand.Next(8);
						if (num8 == 0 && Main.player[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statLife < Main.player[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statLifeMax)
						{
							Item.NewItem(i * 16, j * 16, 16, 16, 58, 1, false, 0);
						}
						else
						{
							if (num8 == 1 && Main.player[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statMana < Main.player[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].statManaMax)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, 184, 1, false, 0);
							}
							else
							{
								if (num8 == 2)
								{
									int stack = Main.rand.Next(1, 6);
									if (Main.tile[i, j].liquid > 0)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 282, stack, false, 0);
									}
									else
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 8, stack, false, 0);
									}
								}
								else
								{
									if (num8 == 3)
									{
										int stack2 = Main.rand.Next(8) + 3;
										int type2 = 40;
										if ((double)j < Main.rockLayer && WorldGen.genRand.Next(2) == 0)
										{
											if (Main.hardMode)
											{
												type2 = 168;
											}
											else
											{
												type2 = 42;
											}
										}
										if (j > Main.maxTilesY - 200)
										{
											type2 = 265;
										}
										else
										{
											if (Main.hardMode)
											{
												if (Main.rand.Next(2) == 0)
												{
													type2 = 278;
												}
												else
												{
													type2 = 47;
												}
											}
										}
										Item.NewItem(i * 16, j * 16, 16, 16, type2, stack2, false, 0);
									}
									else
									{
										if (num8 == 4)
										{
											int type3 = 28;
											if (j > Main.maxTilesY - 200 || Main.hardMode)
											{
												type3 = 188;
											}
											Item.NewItem(i * 16, j * 16, 16, 16, type3, 1, false, 0);
										}
										else
										{
											if (num8 == 5 && (double)j > Main.rockLayer)
											{
												int stack3 = Main.rand.Next(4) + 1;
												Item.NewItem(i * 16, j * 16, 16, 16, 166, stack3, false, 0);
											}
											else
											{
												float num9 = (float)(200 + WorldGen.genRand.Next(-100, 101));
												if ((double)j < Main.worldSurface)
												{
													num9 *= 0.5f;
												}
												else
												{
													if ((double)j < Main.rockLayer)
													{
														num9 *= 0.75f;
													}
													else
													{
														if (j > Main.maxTilesY - 250)
														{
															num9 *= 1.25f;
														}
													}
												}
												num9 *= 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
												if (Main.rand.Next(5) == 0)
												{
													num9 *= 1f + (float)Main.rand.Next(5, 11) * 0.01f;
												}
												if (Main.rand.Next(10) == 0)
												{
													num9 *= 1f + (float)Main.rand.Next(10, 21) * 0.01f;
												}
												if (Main.rand.Next(15) == 0)
												{
													num9 *= 1f + (float)Main.rand.Next(20, 41) * 0.01f;
												}
												if (Main.rand.Next(20) == 0)
												{
													num9 *= 1f + (float)Main.rand.Next(40, 81) * 0.01f;
												}
												if (Main.rand.Next(25) == 0)
												{
													num9 *= 1f + (float)Main.rand.Next(50, 101) * 0.01f;
												}
												while ((int)num9 > 0)
												{
													if (num9 > 1000000f)
													{
														int num10 = (int)(num9 / 1000000f);
														if (num10 > 50 && Main.rand.Next(2) == 0)
														{
															num10 /= Main.rand.Next(3) + 1;
														}
														if (Main.rand.Next(2) == 0)
														{
															num10 /= Main.rand.Next(3) + 1;
														}
														num9 -= (float)(1000000 * num10);
														Item.NewItem(i * 16, j * 16, 16, 16, 74, num10, false, 0);
													}
													else
													{
														if (num9 > 10000f)
														{
															int num11 = (int)(num9 / 10000f);
															if (num11 > 50 && Main.rand.Next(2) == 0)
															{
																num11 /= Main.rand.Next(3) + 1;
															}
															if (Main.rand.Next(2) == 0)
															{
																num11 /= Main.rand.Next(3) + 1;
															}
															num9 -= (float)(10000 * num11);
															Item.NewItem(i * 16, j * 16, 16, 16, 73, num11, false, 0);
														}
														else
														{
															if (num9 > 100f)
															{
																int num12 = (int)(num9 / 100f);
																if (num12 > 50 && Main.rand.Next(2) == 0)
																{
																	num12 /= Main.rand.Next(3) + 1;
																}
																if (Main.rand.Next(2) == 0)
																{
																	num12 /= Main.rand.Next(3) + 1;
																}
																num9 -= (float)(100 * num12);
																Item.NewItem(i * 16, j * 16, 16, 16, 72, num12, false, 0);
															}
															else
															{
																int num13 = (int)num9;
																if (num13 > 50 && Main.rand.Next(2) == 0)
																{
																	num13 /= Main.rand.Next(3) + 1;
																}
																if (Main.rand.Next(2) == 0)
																{
																	num13 /= Main.rand.Next(4) + 1;
																}
																if (num13 < 1)
																{
																	num13 = 1;
																}
																num9 -= (float)num13;
																Item.NewItem(i * 16, j * 16, 16, 16, 71, num13, false, 0);
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
				WorldGen.destroyObject = false;
			}
		}
		public static int PlaceChest(int x, int y, int type = 21, bool notNearOtherChests = false, int style = 0)
		{
			bool flag = true;
			int num = -1;
			for (int i = x; i < x + 2; i++)
			{
				for (int j = y - 1; j < y + 1; j++)
				{
					if (Main.tile[i, j].active)
					{
						flag = false;
					}
					if (Main.tile[i, j].lava)
					{
						flag = false;
					}
				}
				if (!Main.tile[i, y + 1].active || !Main.tileSolid[(int)Main.tile[i, y + 1].type])
				{
					flag = false;
				}
			}
			if (flag && notNearOtherChests)
			{
				for (int k = x - 25; k < x + 25; k++)
				{
					for (int l = y - 8; l < y + 8; l++)
					{
						try
						{
							if (Main.tile[k, l].active && Main.tile[k, l].type == 21)
							{
								flag = false;
								return -1;
							}
						}
						catch
						{
						}
					}
				}
			}
			if (flag)
			{
				num = Chest.CreateChest(x, y - 1);
				if (num == -1)
				{
					flag = false;
				}
			}
			if (flag)
			{
				Main.tile[x, y - 1].active = true;
				Main.tile[x, y - 1].frameY = 0;
				Main.tile[x, y - 1].frameX = (short)(36 * style);
				Main.tile[x, y - 1].type = (byte)type;
				Main.tile[x + 1, y - 1].active = true;
				Main.tile[x + 1, y - 1].frameY = 0;
				Main.tile[x + 1, y - 1].frameX = (short)(18 + 36 * style);
				Main.tile[x + 1, y - 1].type = (byte)type;
				Main.tile[x, y].active = true;
				Main.tile[x, y].frameY = 18;
				Main.tile[x, y].frameX = (short)(36 * style);
				Main.tile[x, y].type = (byte)type;
				Main.tile[x + 1, y].active = true;
				Main.tile[x + 1, y].frameY = 18;
				Main.tile[x + 1, y].frameX = (short)(18 + 36 * style);
				Main.tile[x + 1, y].type = (byte)type;
			}
			return num;
		}
		public static void CheckChest(int i, int j, int type)
		{
			if (WorldGen.destroyObject)
			{
				return;
			}
			bool flag = false;
			int k = 0;
			k += (int)(Main.tile[i, j].frameX / 18);
			int num = j + (int)(Main.tile[i, j].frameY / 18 * -1);
			while (k > 1)
			{
				k -= 2;
			}
			k *= -1;
			k += i;
			for (int l = k; l < k + 2; l++)
			{
				for (int m = num; m < num + 2; m++)
				{

					int n;
					for (n = (int)(Main.tile[l, m].frameX / 18); n > 1; n -= 2)
					{
					}
					if (!Main.tile[l, m].active || (int)Main.tile[l, m].type != type || n != l - k || (int)Main.tile[l, m].frameY != (m - num) * 18)
					{
						flag = true;
					}
				}
				if (!Main.tile[l, num + 2].active || !Main.tileSolid[(int)Main.tile[l, num + 2].type])
				{
					flag = true;
				}
			}
			if (flag)
			{
				int type2 = 48;
				if (Main.tile[i, j].frameX >= 216)
				{
					type2 = 348;
				}
				else
				{
					if (Main.tile[i, j].frameX >= 180)
					{
						type2 = 343;
					}
					else
					{
						if (Main.tile[i, j].frameX >= 108)
						{
							type2 = 328;
						}
						else
						{
							if (Main.tile[i, j].frameX >= 36)
							{
								type2 = 306;
							}
						}
					}
				}
				WorldGen.destroyObject = true;
				for (int num2 = k; num2 < k + 2; num2++)
				{
					for (int num3 = num; num3 < num + 3; num3++)
					{
						if ((int)Main.tile[num2, num3].type == type && Main.tile[num2, num3].active)
						{
							Chest.DestroyChest(num2, num3);
							WorldGen.KillTile(num2, num3, false, false, false);
						}
					}
				}
				Item.NewItem(i * 16, j * 16, 32, 32, type2, 1, false, 0);
				WorldGen.destroyObject = false;
			}
		}
		public static bool PlaceWire(int i, int j)
		{
			if (!Main.tile[i, j].wire)
			{
				Main.PlaySound(0, i * 16, j * 16, 1);
				Main.tile[i, j].wire = true;
				return true;
			}
			return false;
		}
		public static bool KillWire(int i, int j)
		{
			if (Main.tile[i, j].wire)
			{
				Main.PlaySound(0, i * 16, j * 16, 1);
				Main.tile[i, j].wire = false;
				if (Main.netMode != 1)
				{
					Item.NewItem(i * 16, j * 16, 16, 16, 530, 1, false, 0);
				}
				for (int k = 0; k < 5; k++)
				{
					Dust.NewDust(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16, 50, 0f, 0f, 0, default(Color), 1f);
				}
				return true;
			}
			return false;
		}
		public static bool PlaceTile(int i, int j, int type, bool mute = false, bool forced = false, int plr = -1, int style = 0)
		{
			if (type >= 150)
			{
				return false;
			}
			bool result = false;
			if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY)
			{

				if (forced || Collision.EmptyTile(i, j, false) || !Main.tileSolid[type] || (type == 23 && Main.tile[i, j].type == 0 && Main.tile[i, j].active) || (type == 2 && Main.tile[i, j].type == 0 && Main.tile[i, j].active) || (type == 109 && Main.tile[i, j].type == 0 && Main.tile[i, j].active) || (type == 60 && Main.tile[i, j].type == 59 && Main.tile[i, j].active) || (type == 70 && Main.tile[i, j].type == 59 && Main.tile[i, j].active))
				{
					if (type == 23 && (Main.tile[i, j].type != 0 || !Main.tile[i, j].active))
					{
						return false;
					}
					if (type == 2 && (Main.tile[i, j].type != 0 || !Main.tile[i, j].active))
					{
						return false;
					}
					if (type == 109 && (Main.tile[i, j].type != 0 || !Main.tile[i, j].active))
					{
						return false;
					}
					if (type == 60 && (Main.tile[i, j].type != 59 || !Main.tile[i, j].active))
					{
						return false;
					}
					if (type == 81)
					{
						if (Main.tile[i - 1, j].active || Main.tile[i + 1, j].active || Main.tile[i, j - 1].active)
						{
							return false;
						}
						if (!Main.tile[i, j + 1].active || !Main.tileSolid[(int)Main.tile[i, j + 1].type])
						{
							return false;
						}
					}
					if (Main.tile[i, j].liquid > 0)
					{
						if (type == 4)
						{
							if (style != 8)
							{
								return false;
							}
						}
						else
						{
							if (type == 3 || type == 4 || type == 20 || type == 24 || type == 27 || type == 32 || type == 51 || type == 69 || type == 72)
							{
								return false;
							}
						}
					}
					Main.tile[i, j].frameY = 0;
					Main.tile[i, j].frameX = 0;
					if (type == 3 || type == 24 || type == 110)
					{
						if (j + 1 < Main.maxTilesY && Main.tile[i, j + 1].active && ((Main.tile[i, j + 1].type == 2 && type == 3) || (Main.tile[i, j + 1].type == 23 && type == 24) || (Main.tile[i, j + 1].type == 78 && type == 3) || (Main.tile[i, j + 1].type == 109 && type == 110)))
						{
							if (type == 24 && WorldGen.genRand.Next(13) == 0)
							{
								Main.tile[i, j].active = true;
								Main.tile[i, j].type = 32;
								WorldGen.SquareTileFrame(i, j, true);
							}
							else
							{
								if (Main.tile[i, j + 1].type == 78)
								{
									Main.tile[i, j].active = true;
									Main.tile[i, j].type = (byte)type;
									Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(2) * 18 + 108);
								}
								else
								{
									if (Main.tile[i, j].wall == 0 && Main.tile[i, j + 1].wall == 0)
									{
										if (WorldGen.genRand.Next(50) == 0 || (type == 24 && WorldGen.genRand.Next(40) == 0))
										{
											Main.tile[i, j].active = true;
											Main.tile[i, j].type = (byte)type;
											Main.tile[i, j].frameX = 144;
										}
										else
										{
											if (WorldGen.genRand.Next(35) == 0)
											{
												Main.tile[i, j].active = true;
												Main.tile[i, j].type = (byte)type;
												Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(2) * 18 + 108);
											}
											else
											{
												Main.tile[i, j].active = true;
												Main.tile[i, j].type = (byte)type;
												Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(6) * 18);
											}
										}
									}
								}
							}
						}
					}
					else
					{
						if (type == 61)
						{
							if (j + 1 < Main.maxTilesY && Main.tile[i, j + 1].active && Main.tile[i, j + 1].type == 60)
							{
								if (WorldGen.genRand.Next(16) == 0 && (double)j > Main.worldSurface)
								{
									Main.tile[i, j].active = true;
									Main.tile[i, j].type = 69;
									WorldGen.SquareTileFrame(i, j, true);
								}
								else
								{
									if (WorldGen.genRand.Next(60) == 0 && (double)j > Main.rockLayer)
									{
										Main.tile[i, j].active = true;
										Main.tile[i, j].type = (byte)type;
										Main.tile[i, j].frameX = 144;
									}
									else
									{
										if (WorldGen.genRand.Next(1000) == 0 && (double)j > Main.rockLayer)
										{
											Main.tile[i, j].active = true;
											Main.tile[i, j].type = (byte)type;
											Main.tile[i, j].frameX = 162;
										}
										else
										{
											if (WorldGen.genRand.Next(15) == 0)
											{
												Main.tile[i, j].active = true;
												Main.tile[i, j].type = (byte)type;
												Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(2) * 18 + 108);
											}
											else
											{
												Main.tile[i, j].active = true;
												Main.tile[i, j].type = (byte)type;
												Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(6) * 18);
											}
										}
									}
								}
							}
						}
						else
						{
							if (type == 71)
							{
								if (j + 1 < Main.maxTilesY && Main.tile[i, j + 1].active && Main.tile[i, j + 1].type == 70)
								{
									Main.tile[i, j].active = true;
									Main.tile[i, j].type = (byte)type;
									Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(5) * 18);
								}
							}
							else
							{
								if (type == 129)
								{
									if (WorldGen.SolidTile(i - 1, j) || WorldGen.SolidTile(i + 1, j) || WorldGen.SolidTile(i, j - 1) || WorldGen.SolidTile(i, j + 1))
									{
										Main.tile[i, j].active = true;
										Main.tile[i, j].type = (byte)type;
										Main.tile[i, j].frameX = (short)(WorldGen.genRand.Next(8) * 18);
										WorldGen.SquareTileFrame(i, j, true);
									}
								}
								else
								{
									if (type == 132 || type == 138 || type == 142 || type == 143)
									{
										WorldGen.Place2x2(i, j, type);
									}
									else
									{
										if (type == 137)
										{
											Main.tile[i, j].active = true;
											Main.tile[i, j].type = (byte)type;
											if (style == 1)
											{
												Main.tile[i, j].frameX = 18;
											}
										}
										else
										{
											if (type == 136)
											{



												if ((Main.tile[i - 1, j].active && (Main.tileSolid[(int)Main.tile[i - 1, j].type] || Main.tile[i - 1, j].type == 124 || (Main.tile[i - 1, j].type == 5 && Main.tile[i - 1, j - 1].type == 5 && Main.tile[i - 1, j + 1].type == 5))) || (Main.tile[i + 1, j].active && (Main.tileSolid[(int)Main.tile[i + 1, j].type] || Main.tile[i + 1, j].type == 124 || (Main.tile[i + 1, j].type == 5 && Main.tile[i + 1, j - 1].type == 5 && Main.tile[i + 1, j + 1].type == 5))) || (Main.tile[i, j + 1].active && Main.tileSolid[(int)Main.tile[i, j + 1].type]))
												{
													Main.tile[i, j].active = true;
													Main.tile[i, j].type = (byte)type;
													WorldGen.SquareTileFrame(i, j, true);
												}
											}
											else
											{
												if (type == 4)
												{
													if ((Main.tile[i - 1, j].active && (Main.tileSolid[(int)Main.tile[i - 1, j].type] || Main.tile[i - 1, j].type == 124 || (Main.tile[i - 1, j].type == 5 && Main.tile[i - 1, j - 1].type == 5 && Main.tile[i - 1, j + 1].type == 5))) || (Main.tile[i + 1, j].active && (Main.tileSolid[(int)Main.tile[i + 1, j].type] || Main.tile[i + 1, j].type == 124 || (Main.tile[i + 1, j].type == 5 && Main.tile[i + 1, j - 1].type == 5 && Main.tile[i + 1, j + 1].type == 5))) || (Main.tile[i, j + 1].active && Main.tileSolid[(int)Main.tile[i, j + 1].type]))
													{
														Main.tile[i, j].active = true;
														Main.tile[i, j].type = (byte)type;
														Main.tile[i, j].frameY = (short)(22 * style);
														WorldGen.SquareTileFrame(i, j, true);
													}
												}
												else
												{
													if (type == 10)
													{
														if (!Main.tile[i, j - 1].active && !Main.tile[i, j - 2].active && Main.tile[i, j - 3].active && Main.tileSolid[(int)Main.tile[i, j - 3].type])
														{
															WorldGen.PlaceDoor(i, j - 1, type);
															WorldGen.SquareTileFrame(i, j, true);
														}
														else
														{
															if (Main.tile[i, j + 1].active || Main.tile[i, j + 2].active || !Main.tile[i, j + 3].active || !Main.tileSolid[(int)Main.tile[i, j + 3].type])
															{
																return false;
															}
															WorldGen.PlaceDoor(i, j + 1, type);
															WorldGen.SquareTileFrame(i, j, true);
														}
													}
													else
													{
														if (type == 128)
														{
															WorldGen.PlaceMan(i, j, style);
															WorldGen.SquareTileFrame(i, j, true);
														}
														else
														{
															if (type == 149)
															{
																if (WorldGen.SolidTile(i - 1, j) || WorldGen.SolidTile(i + 1, j) || WorldGen.SolidTile(i, j - 1) || WorldGen.SolidTile(i, j + 1))
																{
																	Main.tile[i, j].frameX = (short)(18 * style);
																	Main.tile[i, j].active = true;
																	Main.tile[i, j].type = (byte)type;
																	WorldGen.SquareTileFrame(i, j, true);
																}
															}
															else
															{
																if (type == 139)
																{
																	WorldGen.PlaceMB(i, j, type, style);
																	WorldGen.SquareTileFrame(i, j, true);
																}
																else
																{
																	if (type == 34 || type == 35 || type == 36 || type == 106)
																	{
																		WorldGen.Place3x3(i, j, type);
																		WorldGen.SquareTileFrame(i, j, true);
																	}
																	else
																	{
																		if (type == 13 || type == 33 || type == 49 || type == 50 || type == 78)
																		{
																			WorldGen.PlaceOnTable1x1(i, j, type, style);
																			WorldGen.SquareTileFrame(i, j, true);
																		}
																		else
																		{
																			if (type == 14 || type == 26 || type == 86 || type == 87 || type == 88 || type == 89 || type == 114)
																			{
																				WorldGen.Place3x2(i, j, type);
																				WorldGen.SquareTileFrame(i, j, true);
																			}
																			else
																			{
																				if (type == 20)
																				{
																					if (Main.tile[i, j + 1].active && (Main.tile[i, j + 1].type == 2 || Main.tile[i, j + 1].type == 109 || Main.tile[i, j + 1].type == 147))
																					{
																						WorldGen.Place1x2(i, j, type, style);
																						WorldGen.SquareTileFrame(i, j, true);
																					}
																				}
																				else
																				{
																					if (type == 15)
																					{
																						WorldGen.Place1x2(i, j, type, style);
																						WorldGen.SquareTileFrame(i, j, true);
																					}
																					else
																					{
																						if (type == 16 || type == 18 || type == 29 || type == 103 || type == 134)
																						{
																							WorldGen.Place2x1(i, j, type);
																							WorldGen.SquareTileFrame(i, j, true);
																						}
																						else
																						{
																							if (type == 92 || type == 93)
																							{
																								WorldGen.Place1xX(i, j, type, 0);
																								WorldGen.SquareTileFrame(i, j, true);
																							}
																							else
																							{
																								if (type == 104 || type == 105)
																								{
																									WorldGen.Place2xX(i, j, type, style);
																									WorldGen.SquareTileFrame(i, j, true);
																								}
																								else
																								{
																									if (type == 17 || type == 77 || type == 133)
																									{
																										WorldGen.Place3x2(i, j, type);
																										WorldGen.SquareTileFrame(i, j, true);
																									}
																									else
																									{
																										if (type == 21)
																										{
																											WorldGen.PlaceChest(i, j, type, false, style);
																											WorldGen.SquareTileFrame(i, j, true);
																										}
																										else
																										{
																											if (type == 91)
																											{
																												WorldGen.PlaceBanner(i, j, type, style);
																												WorldGen.SquareTileFrame(i, j, true);
																											}
																											else
																											{
																												if (type == 135 || type == 141 || type == 144)
																												{
																													WorldGen.Place1x1(i, j, type, style);
																													WorldGen.SquareTileFrame(i, j, true);
																												}
																												else
																												{
																													if (type == 101 || type == 102)
																													{
																														WorldGen.Place3x4(i, j, type);
																														WorldGen.SquareTileFrame(i, j, true);
																													}
																													else
																													{
																														if (type == 27)
																														{
																															WorldGen.PlaceSunflower(i, j, 27);
																															WorldGen.SquareTileFrame(i, j, true);
																														}
																														else
																														{
																															if (type == 28)
																															{
																																WorldGen.PlacePot(i, j, 28);
																																WorldGen.SquareTileFrame(i, j, true);
																															}
																															else
																															{
																																if (type == 42)
																																{
																																	WorldGen.Place1x2Top(i, j, type);
																																	WorldGen.SquareTileFrame(i, j, true);
																																}
																																else
																																{
																																	if (type == 55 || type == 85)
																																	{
																																		WorldGen.PlaceSign(i, j, type);
																																	}
																																	else
																																	{
																																		if (Main.tileAlch[type])
																																		{
																																			WorldGen.PlaceAlch(i, j, style);
																																		}
																																		else
																																		{
																																			if (type == 94 || type == 95 || type == 96 || type == 97 || type == 98 || type == 99 || type == 100 || type == 125 || type == 126)
																																			{
																																				WorldGen.Place2x2(i, j, type);
																																			}
																																			else
																																			{
																																				if (type == 79 || type == 90)
																																				{
																																					int direction = 1;
																																					if (plr > -1)
																																					{
																																						direction = Main.player[plr].direction;
																																					}
																																					WorldGen.Place4x2(i, j, type, direction);
																																				}
																																				else
																																				{
																																					if (type == 81)
																																					{
																																						Main.tile[i, j].frameX = (short)(26 * WorldGen.genRand.Next(6));
																																						Main.tile[i, j].active = true;
																																						Main.tile[i, j].type = (byte)type;
																																					}
																																					else
																																					{
																																						Main.tile[i, j].active = true;
																																						Main.tile[i, j].type = (byte)type;
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
					if (Main.tile[i, j].active && !mute)
					{
						WorldGen.SquareTileFrame(i, j, true);
						result = true;
						if (type == 127)
						{
							Main.PlaySound(2, i * 16, j * 16, 30);
						}
						else
						{
							Main.PlaySound(0, i * 16, j * 16, 1);
						}
						if (type == 22 || type == 140)
						{
							for (int k = 0; k < 3; k++)
							{
								Dust.NewDust(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16, 14, 0f, 0f, 0, default(Color), 1f);
							}
						}
					}
				}
			}
			return result;
		}
		public static void UpdateMech()
		{
			for (int i = WorldGen.numMechs - 1; i >= 0; i--)
			{
				WorldGen.mechTime[i]--;
				if (Main.tile[WorldGen.mechX[i], WorldGen.mechY[i]].active && Main.tile[WorldGen.mechX[i], WorldGen.mechY[i]].type == 144)
				{
					if (Main.tile[WorldGen.mechX[i], WorldGen.mechY[i]].frameY == 0)
					{
						WorldGen.mechTime[i] = 0;
					}
					else
					{
						int num = (int)(Main.tile[WorldGen.mechX[i], WorldGen.mechY[i]].frameX / 18);
						if (num == 0)
						{
							num = 60;
						}
						else
						{
							if (num == 1)
							{
								num = 180;
							}
							else
							{
								if (num == 2)
								{
									num = 300;
								}
							}
						}
						if (Math.IEEERemainder((double)WorldGen.mechTime[i], (double)num) == 0.0)
						{
							WorldGen.mechTime[i] = 18000;
							WorldGen.TripWire(WorldGen.mechX[i], WorldGen.mechY[i]);
						}
					}
				}
				if (WorldGen.mechTime[i] <= 0)
				{
					if (Main.tile[WorldGen.mechX[i], WorldGen.mechY[i]].active && Main.tile[WorldGen.mechX[i], WorldGen.mechY[i]].type == 144)
					{
						Main.tile[WorldGen.mechX[i], WorldGen.mechY[i]].frameY = 0;
						NetMessage.SendTileSquare(-1, WorldGen.mechX[i], WorldGen.mechY[i], 1);
					}
					for (int j = i; j < WorldGen.numMechs; j++)
					{
						WorldGen.mechX[j] = WorldGen.mechX[j + 1];
						WorldGen.mechY[j] = WorldGen.mechY[j + 1];
						WorldGen.mechTime[j] = WorldGen.mechTime[j + 1];
					}
					WorldGen.numMechs--;
				}
			}
		}
		public static bool checkMech(int i, int j, int time)
		{
			for (int k = 0; k < WorldGen.numMechs; k++)
			{
				if (WorldGen.mechX[k] == i && WorldGen.mechY[k] == j)
				{
					return false;
				}
			}
			if (WorldGen.numMechs < WorldGen.maxMech - 1)
			{
				WorldGen.mechX[WorldGen.numMechs] = i;
				WorldGen.mechY[WorldGen.numMechs] = j;
				WorldGen.mechTime[WorldGen.numMechs] = time;
				WorldGen.numMechs++;
				return true;
			}
			return false;
		}
		public static void hitSwitch(int i, int j)
		{
			if (Main.tile[i, j] == null)
			{
				return;
			}
			if (Main.tile[i, j].type == 135)
			{
				Main.PlaySound(28, i * 16, j * 16, 0);
				WorldGen.TripWire(i, j);
				return;
			}
			if (Main.tile[i, j].type == 136)
			{
				if (Main.tile[i, j].frameY == 0)
				{
					Main.tile[i, j].frameY = 18;
				}
				else
				{
					Main.tile[i, j].frameY = 0;
				}
				Main.PlaySound(28, i * 16, j * 16, 0);
				WorldGen.TripWire(i, j);
				return;
			}
			if (Main.tile[i, j].type == 144)
			{
				if (Main.tile[i, j].frameY == 0)
				{
					Main.tile[i, j].frameY = 18;
					if (Main.netMode != 1)
					{
						WorldGen.checkMech(i, j, 18000);
					}
				}
				else
				{
					Main.tile[i, j].frameY = 0;
				}
				Main.PlaySound(28, i * 16, j * 16, 0);
				return;
			}
			if (Main.tile[i, j].type == 132)
			{
				int num = i;
				short num2 = 36;
				num = (int)(Main.tile[i, j].frameX / 18 * -1);
				int num3 = (int)(Main.tile[i, j].frameY / 18 * -1);
				if (num < -1)
				{
					num += 2;
					num2 = -36;
				}
				num += i;
				num3 += j;
				for (int k = num; k < num + 2; k++)
				{
					for (int l = num3; l < num3 + 2; l++)
					{
						if (Main.tile[k, l].type == 132)
						{
                            Main.tile[k, l].frameX += num2;
						}
					}
				}
				WorldGen.TileFrame(num, num3, false, false);
				Main.PlaySound(28, i * 16, j * 16, 0);
				for (int m = num; m < num + 2; m++)
				{
					for (int n = num3; n < num3 + 2; n++)
					{
						if (Main.tile[m, n].type == 132 && Main.tile[m, n].active && Main.tile[m, n].wire)
						{
							WorldGen.TripWire(m, n);
							return;
						}
					}
				}
			}
		}
		public static void TripWire(int i, int j)
		{
			if (Main.netMode == 1)
			{
				return;
			}
			WorldGen.numWire = 0;
			WorldGen.numNoWire = 0;
			WorldGen.numInPump = 0;
			WorldGen.numOutPump = 0;
			WorldGen.noWire(i, j);
			WorldGen.hitWire(i, j);
			if (WorldGen.numInPump > 0 && WorldGen.numOutPump > 0)
			{
				WorldGen.xferWater();
			}
		}
		public static void xferWater()
		{
			for (int i = 0; i < WorldGen.numInPump; i++)
			{
				int inX = WorldGen.inPumpX[i];
				int inY = WorldGen.inPumpY[i];
				int liquid = (int)Main.tile[inX, inY].liquid;
				if (liquid > 0)
				{
                    bool lava = Main.tile[inX, inY].lava;
					for (int j = 0; j < WorldGen.numOutPump; j++)
					{
						int outX = WorldGen.outPumpX[j];
						int outY = WorldGen.outPumpY[j];
						int liquid2 = (int)Main.tile[outX, outY].liquid;
						if (liquid2 < 255)
						{
							bool flag = Main.tile[outX, outY].lava;
							if (liquid2 == 0)
							{
								flag = lava;
							}
							if (lava == flag)
							{
								int num5 = liquid;
								if (num5 + liquid2 > 255)
								{
									num5 = 255 - liquid2;
								}
                                Main.tile[outX, outY].liquid += (byte)num5;
                                Main.tile[inX, inY].liquid -= (byte)num5;
                                liquid = (int)Main.tile[inX, inY].liquid;
								Main.tile[outX, outY].lava = lava;
								WorldGen.SquareTileFrame(outX, outY, true);
                                if (Main.tile[inX, inY].liquid == 0)
								{
									Main.tile[inX, inY].lava = false;
                                    WorldGen.SquareTileFrame(inX, inY, true);
									break;
								}
							}
						}
					}
                    WorldGen.SquareTileFrame(inX, inY, true);
				}
			}
		}
		public static void noWire(int i, int j)
		{
			if (WorldGen.numNoWire >= WorldGen.maxWire - 1)
			{
				return;
			}
			WorldGen.noWireX[WorldGen.numNoWire] = i;
			WorldGen.noWireY[WorldGen.numNoWire] = j;
			WorldGen.numNoWire++;
		}
		public static void hitWire(int i, int j)
		{
			if (WorldGen.numWire >= WorldGen.maxWire - 1)
			{
				return;
			}
			if (!Main.tile[i, j].wire)
			{
				return;
			}
			for (int k = 0; k < WorldGen.numWire; k++)
			{
				if (WorldGen.wireX[k] == i && WorldGen.wireY[k] == j)
				{
					return;
				}
			}
			WorldGen.wireX[WorldGen.numWire] = i;
			WorldGen.wireY[WorldGen.numWire] = j;
			WorldGen.numWire++;
			int type = (int)Main.tile[i, j].type;
			bool flag = true;
			for (int l = 0; l < WorldGen.numNoWire; l++)
			{
				if (WorldGen.noWireX[l] == i && WorldGen.noWireY[l] == j)
				{
					flag = false;
				}
			}
			if (flag && Main.tile[i, j].active)
			{
				switch (type)
				{
				    case 144:
				        WorldGen.hitSwitch(i, j);
				        WorldGen.SquareTileFrame(i, j, true);
				        NetMessage.SendTileSquare(-1, i, j, 1);
				        break;
				    case 130:
				        Main.tile[i, j].type = 131;
				        WorldGen.SquareTileFrame(i, j, true);
				        NetMessage.SendTileSquare(-1, i, j, 1);
				        break;
				    case 131:
				        Main.tile[i, j].type = 130;
				        WorldGen.SquareTileFrame(i, j, true);
				        NetMessage.SendTileSquare(-1, i, j, 1);
				        break;
				    case 11:
				        if (WorldGen.CloseDoor(i, j, true))
				        {
				            NetMessage.SendData(19, -1, -1, "", 1, (float)i, (float)j, 0f, 0);
				        }
				        break;
				    case 10:
				        {
				            int num = 1;
				            if (Main.rand.Next(2) == 0)
				            {
				                num = -1;
				            }
				            if (!WorldGen.OpenDoor(i, j, num))
				            {
				                if (WorldGen.OpenDoor(i, j, -num))
				                {
				                    NetMessage.SendData(19, -1, -1, "", 0, (float)i, (float)j, (float)(-(float)num), 0);
				                }
				            }
				            else
				            {
				                NetMessage.SendData(19, -1, -1, "", 0, (float)i, (float)j, (float)num, 0);
				            }
				        }
				        break;
				    case 4:
				        if (Main.tile[i, j].frameX < 66)
				        {
                            Main.tile[i, j].frameX += 66;
				        }
				        else
				        {
                            Main.tile[i, j].frameX -= 66;
				        }
				        NetMessage.SendTileSquare(-1, i, j, 1);
				        break;
				    case 149:
				        if (Main.tile[i, j].frameX < 54)
				        {
                            Main.tile[i, j].frameX += 54;
				        }
				        else
				        {
                            Main.tile[i, j].frameX -= 54;
				        }
				        NetMessage.SendTileSquare(-1, i, j, 1);
				        break;
				    case 42:
				        {
				            int num2 = j - (int)(Main.tile[i, j].frameY / 18);
				            short num3 = 18;
				            if (Main.tile[i, j].frameX > 0)
				            {
				                num3 = -18;
				            }
                            Main.tile[i, num2].frameX += num3;
                            Main.tile[i, num2 + 1].frameX += num3;
				            WorldGen.noWire(i, num2);
				            WorldGen.noWire(i, num2 + 1);
				            NetMessage.SendTileSquare(-1, i, j, 2);
				        }
				        break;
				    case 93:
				        {
				            int num4 = j - (int)(Main.tile[i, j].frameY / 18);
				            short num5 = 18;
				            if (Main.tile[i, j].frameX > 0)
				            {
				                num5 = -18;
				            }
                            Main.tile[i, num4].frameX += num5;
                            Main.tile[i, num4 + 1].frameX += num5;
                            Main.tile[i, num4 + 2].frameX += num5;
				            WorldGen.noWire(i, num4);
				            WorldGen.noWire(i, num4 + 1);
				            WorldGen.noWire(i, num4 + 2);
				            NetMessage.SendTileSquare(-1, i, num4 + 1, 3);
				        }
				        break;
				    case 95:
				    case 100:
				    case 126:
				        {
				            int num6 = j - (int)(Main.tile[i, j].frameY / 18);
				            int num7 = (int)(Main.tile[i, j].frameX / 18);
				            if (num7 > 1)
				            {
				                num7 -= 2;
				            }
				            num7 = i - num7;
				            short num8 = 36;
				            if (Main.tile[num7, num6].frameX > 0)
				            {
				                num8 = -36;
				            }
                            Main.tile[num7, num6].frameX += num8;
                            Main.tile[num7, num6 + 1].frameX += num8;
                            Main.tile[num7 + 1, num6].frameX += num8;
                            Main.tile[num7 + 1, num6 + 1].frameX += num8;
				            WorldGen.noWire(num7, num6);
				            WorldGen.noWire(num7, num6 + 1);
				            WorldGen.noWire(num7 + 1, num6);
				            WorldGen.noWire(num7 + 1, num6 + 1);
				            NetMessage.SendTileSquare(-1, num7, num6, 3);
				        }
				        break;
				    case 36:
				    case 35:
				    case 34:
				        {
				            int num9 = j - (int)(Main.tile[i, j].frameY / 18);
				            int num10 = (int)(Main.tile[i, j].frameX / 18);
				            if (num10 > 2)
				            {
				                num10 -= 3;
				            }
				            num10 = i - num10;
				            short num11 = 54;
				            if (Main.tile[num10, num9].frameX > 0)
				            {
				                num11 = -54;
				            }
				            for (int m = num10; m < num10 + 3; m++)
				            {
				                for (int n = num9; n < num9 + 3; n++)
				                {
                                    Main.tile[m, n].frameX += num11;
				                    WorldGen.noWire(m, n);
				                }
				            }
				            NetMessage.SendTileSquare(-1, num10 + 1, num9 + 1, 3);
				        }
				        break;
				    case 33:
				        {
				            short num12 = 18;
				            if (Main.tile[i, j].frameX > 0)
				            {
				                num12 = -18;
				            }
                            Main.tile[i, j].frameX += num12;
				            NetMessage.SendTileSquare(-1, i, j, 3);
				        }
				        break;
				    case 92:
				        {
				            int num13 = j - (int)(Main.tile[i, j].frameY / 18);
				            short num14 = 18;
				            if (Main.tile[i, j].frameX > 0)
				            {
				                num14 = -18;
				            }
                            Main.tile[i, num13].frameX += num14;
                            Main.tile[i, num13 + 1].frameX += num14;
                            Main.tile[i, num13 + 2].frameX += num14;
                            Main.tile[i, num13 + 3].frameX += num14;
                            Main.tile[i, num13 + 4].frameX += num14;
                            Main.tile[i, num13 + 5].frameX += num14;
				            WorldGen.noWire(i, num13);
				            WorldGen.noWire(i, num13 + 1);
				            WorldGen.noWire(i, num13 + 2);
				            WorldGen.noWire(i, num13 + 3);
				            WorldGen.noWire(i, num13 + 4);
				            WorldGen.noWire(i, num13 + 5);
				            NetMessage.SendTileSquare(-1, i, num13 + 3, 7);
				        }
				        break;
				    case 137:
				        if (WorldGen.checkMech(i, j, 180))
				        {
				            int num15 = -1;
				            if (Main.tile[i, j].frameX != 0)
				            {
				                num15 = 1;
				            }
				            float speedX = (float)(12 * num15);
				            int damage = 20;
				            int type2 = 98;
				            Vector2 vector = new Vector2((float)(i * 16 + 8), (float)(j * 16 + 7));
				            vector.X += (float)(10 * num15);
				            vector.Y += 2f;
				            Projectile.NewProjectile((float)((int)vector.X), (float)((int)vector.Y), speedX, 0f, type2, damage, 2f, Main.myPlayer);
				        }
				        break;
				    case 139:
				        WorldGen.SwitchMB(i, j);
				        break;
				    case 141:
				        WorldGen.KillTile(i, j, false, false, true);
				        NetMessage.SendTileSquare(-1, i, j, 1);
				        Projectile.NewProjectile((float)(i * 16 + 8), (float)(j * 16 + 8), 0f, 0f, 108, 250, 10f, Main.myPlayer);
				        break;
				    case 143:
				    case 142:
				        {
				            int num16 = j - (int)(Main.tile[i, j].frameY / 18);
				            int num17 = (int)(Main.tile[i, j].frameX / 18);
				            if (num17 > 1)
				            {
				                num17 -= 2;
				            }
				            num17 = i - num17;
				            WorldGen.noWire(num17, num16);
				            WorldGen.noWire(num17, num16 + 1);
				            WorldGen.noWire(num17 + 1, num16);
				            WorldGen.noWire(num17 + 1, num16 + 1);
				            if (type == 142)
				            {
				                int num18 = num17;
				                int num19 = num16;
				                for (int num20 = 0; num20 < 4; num20++)
				                {
				                    if (WorldGen.numInPump >= WorldGen.maxPump - 1)
				                    {
				                        break;
				                    }
				                    switch (num20)
				                    {
				                        case 0:
				                            num18 = num17;
				                            num19 = num16 + 1;
				                            break;
				                        case 1:
				                            num18 = num17 + 1;
				                            num19 = num16 + 1;
				                            break;
				                        case 2:
				                            num18 = num17;
				                            num19 = num16;
				                            break;
				                        default:
				                            num18 = num17 + 1;
				                            num19 = num16;
				                            break;
				                    }
				                    WorldGen.inPumpX[WorldGen.numInPump] = num18;
				                    WorldGen.inPumpY[WorldGen.numInPump] = num19;
				                    WorldGen.numInPump++;
				                }
				            }
				            else
				            {
				                int num21 = num17;
				                int num22 = num16;
				                for (int num23 = 0; num23 < 4; num23++)
				                {
				                    if (WorldGen.numOutPump >= WorldGen.maxPump - 1)
				                    {
				                        break;
				                    }
				                    switch (num23)
				                    {
				                        case 0:
				                            num21 = num17;
				                            num22 = num16 + 1;
				                            break;
				                        case 1:
				                            num21 = num17 + 1;
				                            num22 = num16 + 1;
				                            break;
				                        case 2:
				                            num21 = num17;
				                            num22 = num16;
				                            break;
				                        default:
				                            num21 = num17 + 1;
				                            num22 = num16;
				                            break;
				                    }
				                    WorldGen.outPumpX[WorldGen.numOutPump] = num21;
				                    WorldGen.outPumpY[WorldGen.numOutPump] = num22;
				                    WorldGen.numOutPump++;
				                }
				            }
				        }
				        break;
				    case 105:
				        {
				            int num24 = j - (int)(Main.tile[i, j].frameY / 18);
				            int num25 = (int)(Main.tile[i, j].frameX / 18);
				            int num26 = 0;
				            while (num25 >= 2)
				            {
				                num25 -= 2;
				                num26++;
				            }
				            num25 = i - num25;
				            WorldGen.noWire(num25, num24);
				            WorldGen.noWire(num25, num24 + 1);
				            WorldGen.noWire(num25, num24 + 2);
				            WorldGen.noWire(num25 + 1, num24);
				            WorldGen.noWire(num25 + 1, num24 + 1);
				            WorldGen.noWire(num25 + 1, num24 + 2);
				            int num27 = num25 * 16 + 16;
				            int num28 = (num24 + 3) * 16;
				            int num29 = -1;
				            switch (num26)
				            {
				                case 4:
				                    if (WorldGen.checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 1))
				                    {
				                        num29 = NPC.NewNPC(num27, num28 - 12, 1, 0);
				                    }
				                    break;
				                case 7:
				                    if (WorldGen.checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 49))
				                    {
				                        num29 = NPC.NewNPC(num27 - 4, num28 - 6, 49, 0);
				                    }
				                    break;
				                case 8:
                                    if (WorldGen.checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 55) && NPC.MechSpawn((float)num27, (float)num28, 57))
				                    {
				                        num29 = NPC.NewNPC(num27, num28 - 12, 55, 0);
				                    }
				                    break;
				                case 9:
                                    if (WorldGen.checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 46) && NPC.MechSpawn((float)num27, (float)num28, 47))
				                    {
				                        num29 = NPC.NewNPC(num27, num28 - 12, 46, 0);
				                    }
				                    break;
				                case 10:
				                    if (WorldGen.checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 21))
				                    {
				                        num29 = NPC.NewNPC(num27, num28, 21, 0);
				                    }
				                    break;
				                case 18:
				                    if (WorldGen.checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 67))
				                    {
				                        num29 = NPC.NewNPC(num27, num28 - 12, 67, 0);
				                    }
				                    break;
				                case 23:
				                    if (WorldGen.checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 63))
				                    {
				                        num29 = NPC.NewNPC(num27, num28 - 12, 63, 0);
				                    }
				                    break;
				                case 27:
				                    if (WorldGen.checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 85))
				                    {
				                        num29 = NPC.NewNPC(num27 - 9, num28, 85, 0);
				                    }
				                    break;
				                case 28:
				                    if (WorldGen.checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 74))
				                    {
				                        num29 = NPC.NewNPC(num27, num28 - 12, 74, 0);
				                    }
				                    break;
				                case 42:
				                    if (WorldGen.checkMech(i, j, 30) && NPC.MechSpawn((float)num27, (float)num28, 58))
				                    {
				                        num29 = NPC.NewNPC(num27, num28 - 12, 58, 0);
				                    }
				                    break;
				                case 37:
				                    if (WorldGen.checkMech(i, j, 600) && Item.MechSpawn((float)num27, (float)num28, 58))
				                    {
				                        Item.NewItem(num27, num28 - 16, 0, 0, 58, 1, false, 0);
				                    }
				                    break;
				                case 2:
				                    if (WorldGen.checkMech(i, j, 600) && Item.MechSpawn((float)num27, (float)num28, 184))
				                    {
				                        Item.NewItem(num27, num28 - 16, 0, 0, 184, 1, false, 0);
				                    }
				                    break;
				                case 17:
				                    if (WorldGen.checkMech(i, j, 600) && Item.MechSpawn((float)num27, (float)num28, 166))
				                    {
				                        Item.NewItem(num27, num28 - 20, 0, 0, 166, 1, false, 0);
				                    }
				                    break;
				                case 40:
				                    if (WorldGen.checkMech(i, j, 300))
				                    {
				                        int[] array = new int[10];
				                        int num30 = 0;
				                        for (int num31 = 0; num31 < 200; num31++)
				                        {
				                            if (Main.npc[num31].active && (Main.npc[num31].type == 17 || Main.npc[num31].type == 19 || Main.npc[num31].type == 22 || Main.npc[num31].type == 38 || Main.npc[num31].type == 54 || Main.npc[num31].type == 107 || Main.npc[num31].type == 108))
				                            {
				                                array[num30] = num31;
				                                num30++;
				                                if (num30 >= 9)
				                                {
				                                    break;
				                                }
				                            }
				                        }
				                        if (num30 > 0)
				                        {
				                            int num32 = array[Main.rand.Next(num30)];
				                            Main.npc[num32].position.X = (float)(num27 - Main.npc[num32].width / 2);
				                            Main.npc[num32].position.Y = (float)(num28 - Main.npc[num32].height - 1);
				                            NetMessage.SendData(23, -1, -1, "", num32, 0f, 0f, 0f, 0);
				                        }
				                    }
				                    break;
				                default:
				                    if (num26 == 41 && WorldGen.checkMech(i, j, 300))
				                    {
				                        int[] array2 = new int[10];
				                        int num33 = 0;
				                        for (int num34 = 0; num34 < 200; num34++)
				                        {
				                            if (Main.npc[num34].active && (Main.npc[num34].type == 18 || Main.npc[num34].type == 20 || Main.npc[num34].type == 124))
				                            {
				                                array2[num33] = num34;
				                                num33++;
				                                if (num33 >= 9)
				                                {
				                                    break;
				                                }
				                            }
				                        }
				                        if (num33 > 0)
				                        {
				                            int num35 = array2[Main.rand.Next(num33)];
				                            Main.npc[num35].position.X = (float)(num27 - Main.npc[num35].width / 2);
				                            Main.npc[num35].position.Y = (float)(num28 - Main.npc[num35].height - 1);
				                            NetMessage.SendData(23, -1, -1, "", num35, 0f, 0f, 0f, 0);
				                        }
				                    }
				                    break;
				            }
				            if (num29 >= 0)
				            {
				                Main.npc[num29].value = 0f;
				                Main.npc[num29].npcSlots = 0f;
				            }
				        }
				        break;
				}
			}
			WorldGen.hitWire(i - 1, j);
			WorldGen.hitWire(i + 1, j);
			WorldGen.hitWire(i, j - 1);
			WorldGen.hitWire(i, j + 1);
		}
		public static void KillWall(int i, int j, bool fail = false)
		{
			if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY)
			{
				if (Main.tile[i, j].wall > 0)
				{
					if (Main.tile[i, j].wall == 21)
					{
						Main.PlaySound(13, i * 16, j * 16, 1);
					}
					else
					{
						Main.PlaySound(0, i * 16, j * 16, 1);
					}
					int num = 10;
					if (fail)
					{
						num = 3;
					}
					for (int k = 0; k < num; k++)
					{
						int type = 0;
						if (Main.tile[i, j].wall == 1 || Main.tile[i, j].wall == 5 || Main.tile[i, j].wall == 6 || Main.tile[i, j].wall == 7 || Main.tile[i, j].wall == 8 || Main.tile[i, j].wall == 9)
						{
							type = 1;
						}
						if (Main.tile[i, j].wall == 3)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								type = 14;
							}
							else
							{
								type = 1;
							}
						}
						if (Main.tile[i, j].wall == 4)
						{
							type = 7;
						}
						if (Main.tile[i, j].wall == 12)
						{
							type = 9;
						}
						if (Main.tile[i, j].wall == 10)
						{
							type = 10;
						}
						if (Main.tile[i, j].wall == 11)
						{
							type = 11;
						}
						if (Main.tile[i, j].wall == 21)
						{
							type = 13;
						}
						if (Main.tile[i, j].wall == 22 || Main.tile[i, j].wall == 28)
						{
							type = 51;
						}
						if (Main.tile[i, j].wall == 23)
						{
							type = 38;
						}
						if (Main.tile[i, j].wall == 24)
						{
							type = 36;
						}
						if (Main.tile[i, j].wall == 25)
						{
							type = 48;
						}
						if (Main.tile[i, j].wall == 26 || Main.tile[i, j].wall == 30)
						{
							type = 49;
						}
						if (Main.tile[i, j].wall == 29)
						{
							type = 50;
						}
						if (Main.tile[i, j].wall == 31)
						{
							type = 51;
						}
						if (Main.tile[i, j].wall == 27)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								type = 7;
							}
							else
							{
								type = 1;
							}
						}
						Dust.NewDust(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16, type, 0f, 0f, 0, default(Color), 1f);
					}
					if (fail)
					{
						WorldGen.SquareWallFrame(i, j, true);
						return;
					}
					int num2 = 0;
					if (Main.tile[i, j].wall == 1)
					{
						num2 = 26;
					}
					if (Main.tile[i, j].wall == 4)
					{
						num2 = 93;
					}
					if (Main.tile[i, j].wall == 5)
					{
						num2 = 130;
					}
					if (Main.tile[i, j].wall == 6)
					{
						num2 = 132;
					}
					if (Main.tile[i, j].wall == 7)
					{
						num2 = 135;
					}
					if (Main.tile[i, j].wall == 8)
					{
						num2 = 138;
					}
					if (Main.tile[i, j].wall == 9)
					{
						num2 = 140;
					}
					if (Main.tile[i, j].wall == 10)
					{
						num2 = 142;
					}
					if (Main.tile[i, j].wall == 11)
					{
						num2 = 144;
					}
					if (Main.tile[i, j].wall == 12)
					{
						num2 = 146;
					}
					if (Main.tile[i, j].wall == 14)
					{
						num2 = 330;
					}
					if (Main.tile[i, j].wall == 16)
					{
						num2 = 30;
					}
					if (Main.tile[i, j].wall == 17)
					{
						num2 = 135;
					}
					if (Main.tile[i, j].wall == 18)
					{
						num2 = 138;
					}
					if (Main.tile[i, j].wall == 19)
					{
						num2 = 140;
					}
					if (Main.tile[i, j].wall == 20)
					{
						num2 = 330;
					}
					if (Main.tile[i, j].wall == 21)
					{
						num2 = 392;
					}
					if (Main.tile[i, j].wall == 22)
					{
						num2 = 417;
					}
					if (Main.tile[i, j].wall == 23)
					{
						num2 = 418;
					}
					if (Main.tile[i, j].wall == 24)
					{
						num2 = 419;
					}
					if (Main.tile[i, j].wall == 25)
					{
						num2 = 420;
					}
					if (Main.tile[i, j].wall == 26)
					{
						num2 = 421;
					}
					if (Main.tile[i, j].wall == 29)
					{
						num2 = 587;
					}
					if (Main.tile[i, j].wall == 30)
					{
						num2 = 592;
					}
					if (Main.tile[i, j].wall == 31)
					{
						num2 = 595;
					}
					if (Main.tile[i, j].wall == 27)
					{
						num2 = 479;
					}
					if (num2 > 0)
					{
						Item.NewItem(i * 16, j * 16, 16, 16, num2, 1, false, 0);
					}
					Main.tile[i, j].wall = 0;
					WorldGen.SquareWallFrame(i, j, true);
				}
			}
		}
		public static void KillTile(int i, int j, bool fail = false, bool effectOnly = false, bool noItem = false)
		{
			if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY)
			{
				if (Main.tile[i, j].active)
				{
					if (j >= 1 && Main.tile[i, j - 1] == null)
					{

					}
					if (j >= 1 && Main.tile[i, j - 1].active && ((Main.tile[i, j - 1].type == 5 && Main.tile[i, j].type != 5) || (Main.tile[i, j - 1].type == 21 && Main.tile[i, j].type != 21) || (Main.tile[i, j - 1].type == 26 && Main.tile[i, j].type != 26) || (Main.tile[i, j - 1].type == 72 && Main.tile[i, j].type != 72) || (Main.tile[i, j - 1].type == 12 && Main.tile[i, j].type != 12)) && (Main.tile[i, j - 1].type != 5 || ((Main.tile[i, j - 1].frameX != 66 || Main.tile[i, j - 1].frameY < 0 || Main.tile[i, j - 1].frameY > 44) && (Main.tile[i, j - 1].frameX != 88 || Main.tile[i, j - 1].frameY < 66 || Main.tile[i, j - 1].frameY > 110) && Main.tile[i, j - 1].frameY < 198)))
					{
						return;
					}
					if (!effectOnly && !WorldGen.stopDrops)
					{
						if (Main.tile[i, j].type == 127)
						{
							Main.PlaySound(2, i * 16, j * 16, 27);
						}
						else
						{
							if (Main.tile[i, j].type == 3 || Main.tile[i, j].type == 110)
							{
								Main.PlaySound(6, i * 16, j * 16, 1);
								if (Main.tile[i, j].frameX == 144)
								{
									Item.NewItem(i * 16, j * 16, 16, 16, 5, 1, false, 0);
								}
							}
							else
							{
								if (Main.tile[i, j].type == 24)
								{
									Main.PlaySound(6, i * 16, j * 16, 1);
									if (Main.tile[i, j].frameX == 144)
									{
										Item.NewItem(i * 16, j * 16, 16, 16, 60, 1, false, 0);
									}
								}
								else
								{
									if (Main.tileAlch[(int)Main.tile[i, j].type] || Main.tile[i, j].type == 32 || Main.tile[i, j].type == 51 || Main.tile[i, j].type == 52 || Main.tile[i, j].type == 61 || Main.tile[i, j].type == 62 || Main.tile[i, j].type == 69 || Main.tile[i, j].type == 71 || Main.tile[i, j].type == 73 || Main.tile[i, j].type == 74 || Main.tile[i, j].type == 113 || Main.tile[i, j].type == 115)
									{
										Main.PlaySound(6, i * 16, j * 16, 1);
									}
									else
									{
										if (Main.tile[i, j].type == 1 || Main.tile[i, j].type == 6 || Main.tile[i, j].type == 7 || Main.tile[i, j].type == 8 || Main.tile[i, j].type == 9 || Main.tile[i, j].type == 22 || Main.tile[i, j].type == 140 || Main.tile[i, j].type == 25 || Main.tile[i, j].type == 37 || Main.tile[i, j].type == 38 || Main.tile[i, j].type == 39 || Main.tile[i, j].type == 41 || Main.tile[i, j].type == 43 || Main.tile[i, j].type == 44 || Main.tile[i, j].type == 45 || Main.tile[i, j].type == 46 || Main.tile[i, j].type == 47 || Main.tile[i, j].type == 48 || Main.tile[i, j].type == 56 || Main.tile[i, j].type == 58 || Main.tile[i, j].type == 63 || Main.tile[i, j].type == 64 || Main.tile[i, j].type == 65 || Main.tile[i, j].type == 66 || Main.tile[i, j].type == 67 || Main.tile[i, j].type == 68 || Main.tile[i, j].type == 75 || Main.tile[i, j].type == 76 || Main.tile[i, j].type == 107 || Main.tile[i, j].type == 108 || Main.tile[i, j].type == 111 || Main.tile[i, j].type == 117 || Main.tile[i, j].type == 118 || Main.tile[i, j].type == 119 || Main.tile[i, j].type == 120 || Main.tile[i, j].type == 121 || Main.tile[i, j].type == 122)
										{
											Main.PlaySound(21, i * 16, j * 16, 1);
										}
										else
										{
											if (Main.tile[i, j].type != 138)
											{
												Main.PlaySound(0, i * 16, j * 16, 1);
											}
										}
									}
								}
							}
						}
						if (Main.tile[i, j].type == 129 && !fail)
						{
							Main.PlaySound(2, i * 16, j * 16, 27);
						}
					}
					int num = 10;
					if (Main.tile[i, j].type == 128)
					{
						int num2 = i;
						int k = (int)Main.tile[i, j].frameX;
						int l;
						for (l = (int)Main.tile[i, j].frameX; l >= 100; l -= 100)
						{
						}
						while (l >= 36)
						{
							l -= 36;
						}
						if (l == 18)
						{
							k = (int)Main.tile[i - 1, j].frameX;
							num2--;
						}
						if (k >= 100)
						{
							int num3 = 0;
							while (k >= 100)
							{
								k -= 100;
								num3++;
							}
							int num4 = (int)(Main.tile[num2, j].frameY / 18);
							if (num4 == 0)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, Item.headType[num3], 1, false, 0);
							}
							if (num4 == 1)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, Item.bodyType[num3], 1, false, 0);
							}
							if (num4 == 2)
							{
								Item.NewItem(i * 16, j * 16, 16, 16, Item.legType[num3], 1, false, 0);
							}
							for (k = (int)Main.tile[num2, j].frameX; k >= 100; k -= 100)
							{
							}
							Main.tile[num2, j].frameX = (short)k;
						}
					}
					if (fail)
					{
						num = 3;
					}
					if (Main.tile[i, j].type == 138)
					{
						num = 0;
					}
					for (int m = 0; m < num; m++)
					{
						int num5 = 0;
						if (Main.tile[i, j].type == 0)
						{
							num5 = 0;
						}
						if (Main.tile[i, j].type == 1 || Main.tile[i, j].type == 16 || Main.tile[i, j].type == 17 || Main.tile[i, j].type == 38 || Main.tile[i, j].type == 39 || Main.tile[i, j].type == 41 || Main.tile[i, j].type == 43 || Main.tile[i, j].type == 44 || Main.tile[i, j].type == 48 || Main.tileStone[(int)Main.tile[i, j].type] || Main.tile[i, j].type == 85 || Main.tile[i, j].type == 90 || Main.tile[i, j].type == 92 || Main.tile[i, j].type == 96 || Main.tile[i, j].type == 97 || Main.tile[i, j].type == 99 || Main.tile[i, j].type == 105 || Main.tile[i, j].type == 117 || Main.tile[i, j].type == 130 || Main.tile[i, j].type == 131 || Main.tile[i, j].type == 132 || Main.tile[i, j].type == 135 || Main.tile[i, j].type == 135 || Main.tile[i, j].type == 137 || Main.tile[i, j].type == 142 || Main.tile[i, j].type == 143 || Main.tile[i, j].type == 144)
						{
							num5 = 1;
						}
						if (Main.tile[i, j].type == 33 || Main.tile[i, j].type == 95 || Main.tile[i, j].type == 98 || Main.tile[i, j].type == 100)
						{
							num5 = 6;
						}
						if (Main.tile[i, j].type == 5 || Main.tile[i, j].type == 10 || Main.tile[i, j].type == 11 || Main.tile[i, j].type == 14 || Main.tile[i, j].type == 15 || Main.tile[i, j].type == 19 || Main.tile[i, j].type == 30 || Main.tile[i, j].type == 86 || Main.tile[i, j].type == 87 || Main.tile[i, j].type == 88 || Main.tile[i, j].type == 89 || Main.tile[i, j].type == 93 || Main.tile[i, j].type == 94 || Main.tile[i, j].type == 104 || Main.tile[i, j].type == 106 || Main.tile[i, j].type == 114 || Main.tile[i, j].type == 124 || Main.tile[i, j].type == 128 || Main.tile[i, j].type == 139)
						{
							num5 = 7;
						}
						if (Main.tile[i, j].type == 21)
						{
							if (Main.tile[i, j].frameX >= 108)
							{
								num5 = 37;
							}
							else
							{
								if (Main.tile[i, j].frameX >= 36)
								{
									num5 = 10;
								}
								else
								{
									num5 = 7;
								}
							}
						}
						if (Main.tile[i, j].type == 2)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								num5 = 0;
							}
							else
							{
								num5 = 2;
							}
						}
						if (Main.tile[i, j].type == 127)
						{
							num5 = 67;
						}
						if (Main.tile[i, j].type == 91)
						{
							num5 = -1;
						}
						if (Main.tile[i, j].type == 6 || Main.tile[i, j].type == 26)
						{
							num5 = 8;
						}
						if (Main.tile[i, j].type == 7 || Main.tile[i, j].type == 34 || Main.tile[i, j].type == 47)
						{
							num5 = 9;
						}
						if (Main.tile[i, j].type == 8 || Main.tile[i, j].type == 36 || Main.tile[i, j].type == 45 || Main.tile[i, j].type == 102)
						{
							num5 = 10;
						}
						if (Main.tile[i, j].type == 9 || Main.tile[i, j].type == 35 || Main.tile[i, j].type == 42 || Main.tile[i, j].type == 46 || Main.tile[i, j].type == 126 || Main.tile[i, j].type == 136)
						{
							num5 = 11;
						}
						if (Main.tile[i, j].type == 12)
						{
							num5 = 12;
						}
						if (Main.tile[i, j].type == 3 || Main.tile[i, j].type == 73)
						{
							num5 = 3;
						}
						if (Main.tile[i, j].type == 13 || Main.tile[i, j].type == 54)
						{
							num5 = 13;
						}
						if (Main.tile[i, j].type == 22 || Main.tile[i, j].type == 140)
						{
							num5 = 14;
						}
						if (Main.tile[i, j].type == 28 || Main.tile[i, j].type == 78)
						{
							num5 = 22;
						}
						if (Main.tile[i, j].type == 29)
						{
							num5 = 23;
						}
						if (Main.tile[i, j].type == 40 || Main.tile[i, j].type == 103)
						{
							num5 = 28;
						}
						if (Main.tile[i, j].type == 49)
						{
							num5 = 29;
						}
						if (Main.tile[i, j].type == 50)
						{
							num5 = 22;
						}
						if (Main.tile[i, j].type == 51)
						{
							num5 = 30;
						}
						if (Main.tile[i, j].type == 52)
						{
							num5 = 3;
						}
						if (Main.tile[i, j].type == 53 || Main.tile[i, j].type == 81)
						{
							num5 = 32;
						}
						if (Main.tile[i, j].type == 56 || Main.tile[i, j].type == 75)
						{
							num5 = 37;
						}
						if (Main.tile[i, j].type == 57 || Main.tile[i, j].type == 119 || Main.tile[i, j].type == 141)
						{
							num5 = 36;
						}
						if (Main.tile[i, j].type == 59 || Main.tile[i, j].type == 120)
						{
							num5 = 38;
						}
						if (Main.tile[i, j].type == 61 || Main.tile[i, j].type == 62 || Main.tile[i, j].type == 74 || Main.tile[i, j].type == 80)
						{
							num5 = 40;
						}
						if (Main.tile[i, j].type == 69)
						{
							num5 = 7;
						}
						if (Main.tile[i, j].type == 71 || Main.tile[i, j].type == 72)
						{
							num5 = 26;
						}
						if (Main.tile[i, j].type == 70)
						{
							num5 = 17;
						}
						if (Main.tile[i, j].type == 112)
						{
							num5 = 14;
						}
						if (Main.tile[i, j].type == 123)
						{
							num5 = 53;
						}
						if (Main.tile[i, j].type == 116 || Main.tile[i, j].type == 118 || Main.tile[i, j].type == 147 || Main.tile[i, j].type == 148)
						{
							num5 = 51;
						}
						if (Main.tile[i, j].type == 109)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								num5 = 0;
							}
							else
							{
								num5 = 47;
							}
						}
						if (Main.tile[i, j].type == 110 || Main.tile[i, j].type == 113 || Main.tile[i, j].type == 115)
						{
							num5 = 47;
						}
						if (Main.tile[i, j].type == 107 || Main.tile[i, j].type == 121)
						{
							num5 = 48;
						}
						if (Main.tile[i, j].type == 108 || Main.tile[i, j].type == 122 || Main.tile[i, j].type == 134 || Main.tile[i, j].type == 146)
						{
							num5 = 49;
						}
						if (Main.tile[i, j].type == 111 || Main.tile[i, j].type == 133 || Main.tile[i, j].type == 145)
						{
							num5 = 50;
						}
						if (Main.tile[i, j].type == 149)
						{
							num5 = 49;
						}
						if (Main.tileAlch[(int)Main.tile[i, j].type])
						{
							int num6 = (int)(Main.tile[i, j].frameX / 18);
							if (num6 == 0)
							{
								num5 = 3;
							}
							if (num6 == 1)
							{
								num5 = 3;
							}
							if (num6 == 2)
							{
								num5 = 7;
							}
							if (num6 == 3)
							{
								num5 = 17;
							}
							if (num6 == 4)
							{
								num5 = 3;
							}
							if (num6 == 5)
							{
								num5 = 6;
							}
						}
						if (Main.tile[i, j].type == 61)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								num5 = 38;
							}
							else
							{
								num5 = 39;
							}
						}
						if (Main.tile[i, j].type == 58 || Main.tile[i, j].type == 76 || Main.tile[i, j].type == 77)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								num5 = 6;
							}
							else
							{
								num5 = 25;
							}
						}
						if (Main.tile[i, j].type == 37)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								num5 = 6;
							}
							else
							{
								num5 = 23;
							}
						}
						if (Main.tile[i, j].type == 32)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								num5 = 14;
							}
							else
							{
								num5 = 24;
							}
						}
						if (Main.tile[i, j].type == 23 || Main.tile[i, j].type == 24)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								num5 = 14;
							}
							else
							{
								num5 = 17;
							}
						}
						if (Main.tile[i, j].type == 25 || Main.tile[i, j].type == 31)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								num5 = 14;
							}
							else
							{
								num5 = 1;
							}
						}
						if (Main.tile[i, j].type == 20)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								num5 = 7;
							}
							else
							{
								num5 = 2;
							}
						}
						if (Main.tile[i, j].type == 27)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								num5 = 3;
							}
							else
							{
								num5 = 19;
							}
						}
						if (Main.tile[i, j].type == 129)
						{
							if (Main.tile[i, j].frameX == 0 || Main.tile[i, j].frameX == 54 || Main.tile[i, j].frameX == 108)
							{
								num5 = 68;
							}
							else
							{
								if (Main.tile[i, j].frameX == 18 || Main.tile[i, j].frameX == 72 || Main.tile[i, j].frameX == 126)
								{
									num5 = 69;
								}
								else
								{
									num5 = 70;
								}
							}
						}
						if (Main.tile[i, j].type == 4)
						{
							int num7 = (int)(Main.tile[i, j].frameY / 22);
							if (num7 == 0)
							{
								num5 = 6;
							}
							else
							{
								if (num7 == 8)
								{
									num5 = 75;
								}
								else
								{
									num5 = 58 + num7;
								}
							}
						}
						if ((Main.tile[i, j].type == 34 || Main.tile[i, j].type == 35 || Main.tile[i, j].type == 36 || Main.tile[i, j].type == 42) && Main.rand.Next(2) == 0)
						{
							num5 = 6;
						}
						if (num5 >= 0)
						{
							Dust.NewDust(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16, num5, 0f, 0f, 0, default(Color), 1f);
						}
					}
					if (effectOnly)
					{
						return;
					}
					if (fail)
					{
						if (Main.tile[i, j].type == 2 || Main.tile[i, j].type == 23 || Main.tile[i, j].type == 109)
						{
							Main.tile[i, j].type = 0;
						}
						if (Main.tile[i, j].type == 60 || Main.tile[i, j].type == 70)
						{
							Main.tile[i, j].type = 59;
						}
						WorldGen.SquareTileFrame(i, j, true);
						return;
					}
					if (Main.tile[i, j].type == 21 && Main.netMode != 1)
					{
						int n = (int)(Main.tile[i, j].frameX / 18);
						int y = j - (int)(Main.tile[i, j].frameY / 18);
						while (n > 1)
						{
							n -= 2;
						}
						n = i - n;
						if (!Chest.DestroyChest(n, y))
						{
							return;
						}
					}
					if (!noItem && !WorldGen.stopDrops && Main.netMode != 1)
					{
						int num8 = 0;
						if (Main.tile[i, j].type == 0 || Main.tile[i, j].type == 2 || Main.tile[i, j].type == 109)
						{
							num8 = 2;
						}
						else
						{
							if (Main.tile[i, j].type == 1)
							{
								num8 = 3;
							}
							else
							{
								if (Main.tile[i, j].type == 3 || Main.tile[i, j].type == 73)
								{
									if (Main.rand.Next(2) == 0 && Main.player[(int)Player.FindClosest(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16)].HasItem(281))
									{
										num8 = 283;
									}
								}
								else
								{
									if (Main.tile[i, j].type == 4)
									{
										int num9 = (int)(Main.tile[i, j].frameY / 22);
										if (num9 == 0)
										{
											num8 = 8;
										}
										else
										{
											if (num9 == 8)
											{
												num8 = 523;
											}
											else
											{
												num8 = 426 + num9;
											}
										}
									}
									else
									{
										if (Main.tile[i, j].type == 5)
										{
											if (Main.tile[i, j].frameX >= 22 && Main.tile[i, j].frameY >= 198)
											{
												if (Main.netMode != 1)
												{
													if (WorldGen.genRand.Next(2) == 0)
													{
														int num10 = j;
														while (Main.tile[i, num10] != null && (!Main.tile[i, num10].active || !Main.tileSolid[(int)Main.tile[i, num10].type] || Main.tileSolidTop[(int)Main.tile[i, num10].type]))
														{
															num10++;
														}
														if (Main.tile[i, num10] != null)
														{
															if (Main.tile[i, num10].type == 2 || Main.tile[i, num10].type == 109)
															{
																num8 = 27;
															}
															else
															{
																num8 = 9;
															}
														}
													}
													else
													{
														num8 = 9;
													}
												}
											}
											else
											{
												num8 = 9;
											}
										}
										else
										{
											if (Main.tile[i, j].type == 6)
											{
												num8 = 11;
											}
											else
											{
												if (Main.tile[i, j].type == 7)
												{
													num8 = 12;
												}
												else
												{
													if (Main.tile[i, j].type == 8)
													{
														num8 = 13;
													}
													else
													{
														if (Main.tile[i, j].type == 9)
														{
															num8 = 14;
														}
														else
														{
															if (Main.tile[i, j].type == 123)
															{
																num8 = 424;
															}
															else
															{
																if (Main.tile[i, j].type == 124)
																{
																	num8 = 480;
																}
																else
																{
																	if (Main.tile[i, j].type == 149)
																	{
																		if (Main.tile[i, j].frameX == 0 || Main.tile[i, j].frameX == 54)
																		{
																			num8 = 596;
																		}
																		else
																		{
																			if (Main.tile[i, j].frameX == 18 || Main.tile[i, j].frameX == 72)
																			{
																				num8 = 597;
																			}
																			else
																			{
																				if (Main.tile[i, j].frameX == 36 || Main.tile[i, j].frameX == 90)
																				{
																					num8 = 598;
																				}
																			}
																		}
																	}
																	else
																	{
																		if (Main.tile[i, j].type == 13)
																		{
																			Main.PlaySound(13, i * 16, j * 16, 1);
																			if (Main.tile[i, j].frameX == 18)
																			{
																				num8 = 28;
																			}
																			else
																			{
																				if (Main.tile[i, j].frameX == 36)
																				{
																					num8 = 110;
																				}
																				else
																				{
																					if (Main.tile[i, j].frameX == 54)
																					{
																						num8 = 350;
																					}
																					else
																					{
																						if (Main.tile[i, j].frameX == 72)
																						{
																							num8 = 351;
																						}
																						else
																						{
																							num8 = 31;
																						}
																					}
																				}
																			}
																		}
																		else
																		{
																			if (Main.tile[i, j].type == 19)
																			{
																				num8 = 94;
																			}
																			else
																			{
																				if (Main.tile[i, j].type == 22)
																				{
																					num8 = 56;
																				}
																				else
																				{
																					if (Main.tile[i, j].type == 140)
																					{
																						num8 = 577;
																					}
																					else
																					{
																						if (Main.tile[i, j].type == 23)
																						{
																							num8 = 2;
																						}
																						else
																						{
																							if (Main.tile[i, j].type == 25)
																							{
																								num8 = 61;
																							}
																							else
																							{
																								if (Main.tile[i, j].type == 30)
																								{
																									num8 = 9;
																								}
																								else
																								{
																									if (Main.tile[i, j].type == 33)
																									{
																										num8 = 105;
																									}
																									else
																									{
																										if (Main.tile[i, j].type == 37)
																										{
																											num8 = 116;
																										}
																										else
																										{
																											if (Main.tile[i, j].type == 38)
																											{
																												num8 = 129;
																											}
																											else
																											{
																												if (Main.tile[i, j].type == 39)
																												{
																													num8 = 131;
																												}
																												else
																												{
																													if (Main.tile[i, j].type == 40)
																													{
																														num8 = 133;
																													}
																													else
																													{
																														if (Main.tile[i, j].type == 41)
																														{
																															num8 = 134;
																														}
																														else
																														{
																															if (Main.tile[i, j].type == 43)
																															{
																																num8 = 137;
																															}
																															else
																															{
																																if (Main.tile[i, j].type == 44)
																																{
																																	num8 = 139;
																																}
																																else
																																{
																																	if (Main.tile[i, j].type == 45)
																																	{
																																		num8 = 141;
																																	}
																																	else
																																	{
																																		if (Main.tile[i, j].type == 46)
																																		{
																																			num8 = 143;
																																		}
																																		else
																																		{
																																			if (Main.tile[i, j].type == 47)
																																			{
																																				num8 = 145;
																																			}
																																			else
																																			{
																																				if (Main.tile[i, j].type == 48)
																																				{
																																					num8 = 147;
																																				}
																																				else
																																				{
																																					if (Main.tile[i, j].type == 49)
																																					{
																																						num8 = 148;
																																					}
																																					else
																																					{
																																						if (Main.tile[i, j].type == 51)
																																						{
																																							num8 = 150;
																																						}
																																						else
																																						{
																																							if (Main.tile[i, j].type == 53)
																																							{
																																								num8 = 169;
																																							}
																																							else
																																							{
																																								if (Main.tile[i, j].type == 54)
																																								{
																																									num8 = 170;
																																									Main.PlaySound(13, i * 16, j * 16, 1);
																																								}
																																								else
																																								{
																																									if (Main.tile[i, j].type == 56)
																																									{
																																										num8 = 173;
																																									}
																																									else
																																									{
																																										if (Main.tile[i, j].type == 57)
																																										{
																																											num8 = 172;
																																										}
																																										else
																																										{
																																											if (Main.tile[i, j].type == 58)
																																											{
																																												num8 = 174;
																																											}
																																											else
																																											{
																																												if (Main.tile[i, j].type == 60)
																																												{
																																													num8 = 176;
																																												}
																																												else
																																												{
																																													if (Main.tile[i, j].type == 70)
																																													{
																																														num8 = 176;
																																													}
																																													else
																																													{
																																														if (Main.tile[i, j].type == 75)
																																														{
																																															num8 = 192;
																																														}
																																														else
																																														{
																																															if (Main.tile[i, j].type == 76)
																																															{
																																																num8 = 214;
																																															}
																																															else
																																															{
																																																if (Main.tile[i, j].type == 78)
																																																{
																																																	num8 = 222;
																																																}
																																																else
																																																{
																																																	if (Main.tile[i, j].type == 81)
																																																	{
																																																		num8 = 275;
																																																	}
																																																	else
																																																	{
																																																		if (Main.tile[i, j].type == 80)
																																																		{
																																																			num8 = 276;
																																																		}
																																																		else
																																																		{
																																																			if (Main.tile[i, j].type == 107)
																																																			{
																																																				num8 = 364;
																																																			}
																																																			else
																																																			{
																																																				if (Main.tile[i, j].type == 108)
																																																				{
																																																					num8 = 365;
																																																				}
																																																				else
																																																				{
																																																					if (Main.tile[i, j].type == 111)
																																																					{
																																																						num8 = 366;
																																																					}
																																																					else
																																																					{
																																																						if (Main.tile[i, j].type == 112)
																																																						{
																																																							num8 = 370;
																																																						}
																																																						else
																																																						{
																																																							if (Main.tile[i, j].type == 116)
																																																							{
																																																								num8 = 408;
																																																							}
																																																							else
																																																							{
																																																								if (Main.tile[i, j].type == 117)
																																																								{
																																																									num8 = 409;
																																																								}
																																																								else
																																																								{
																																																									if (Main.tile[i, j].type == 129)
																																																									{
																																																										num8 = 502;
																																																									}
																																																									else
																																																									{
																																																										if (Main.tile[i, j].type == 118)
																																																										{
																																																											num8 = 412;
																																																										}
																																																										else
																																																										{
																																																											if (Main.tile[i, j].type == 119)
																																																											{
																																																												num8 = 413;
																																																											}
																																																											else
																																																											{
																																																												if (Main.tile[i, j].type == 120)
																																																												{
																																																													num8 = 414;
																																																												}
																																																												else
																																																												{
																																																													if (Main.tile[i, j].type == 121)
																																																													{
																																																														num8 = 415;
																																																													}
																																																													else
																																																													{
																																																														if (Main.tile[i, j].type == 122)
																																																														{
																																																															num8 = 416;
																																																														}
																																																														else
																																																														{
																																																															if (Main.tile[i, j].type == 136)
																																																															{
																																																																num8 = 538;
																																																															}
																																																															else
																																																															{
																																																																if (Main.tile[i, j].type == 137)
																																																																{
																																																																	num8 = 539;
																																																																}
																																																																else
																																																																{
																																																																	if (Main.tile[i, j].type == 141)
																																																																	{
																																																																		num8 = 580;
																																																																	}
																																																																	else
																																																																	{
																																																																		if (Main.tile[i, j].type == 145)
																																																																		{
																																																																			num8 = 586;
																																																																		}
																																																																		else
																																																																		{
																																																																			if (Main.tile[i, j].type == 146)
																																																																			{
																																																																				num8 = 591;
																																																																			}
																																																																			else
																																																																			{
																																																																				if (Main.tile[i, j].type == 147)
																																																																				{
																																																																					num8 = 593;
																																																																				}
																																																																				else
																																																																				{
																																																																					if (Main.tile[i, j].type == 148)
																																																																					{
																																																																						num8 = 594;
																																																																					}
																																																																					else
																																																																					{
																																																																						if (Main.tile[i, j].type == 135)
																																																																						{
																																																																							if (Main.tile[i, j].frameY == 0)
																																																																							{
																																																																								num8 = 529;
																																																																							}
																																																																							if (Main.tile[i, j].frameY == 18)
																																																																							{
																																																																								num8 = 541;
																																																																							}
																																																																							if (Main.tile[i, j].frameY == 36)
																																																																							{
																																																																								num8 = 542;
																																																																							}
																																																																							if (Main.tile[i, j].frameY == 54)
																																																																							{
																																																																								num8 = 543;
																																																																							}
																																																																						}
																																																																						else
																																																																						{
																																																																							if (Main.tile[i, j].type == 144)
																																																																							{
																																																																								if (Main.tile[i, j].frameX == 0)
																																																																								{
																																																																									num8 = 583;
																																																																								}
																																																																								if (Main.tile[i, j].frameX == 18)
																																																																								{
																																																																									num8 = 584;
																																																																								}
																																																																								if (Main.tile[i, j].frameX == 36)
																																																																								{
																																																																									num8 = 585;
																																																																								}
																																																																							}
																																																																							else
																																																																							{
																																																																								if (Main.tile[i, j].type == 130)
																																																																								{
																																																																									num8 = 511;
																																																																								}
																																																																								else
																																																																								{
																																																																									if (Main.tile[i, j].type == 131)
																																																																									{
																																																																										num8 = 512;
																																																																									}
																																																																									else
																																																																									{
																																																																										if (Main.tile[i, j].type == 61 || Main.tile[i, j].type == 74)
																																																																										{
																																																																											if (Main.tile[i, j].frameX == 144)
																																																																											{
																																																																												Item.NewItem(i * 16, j * 16, 16, 16, 331, WorldGen.genRand.Next(2, 4), false, 0);
																																																																											}
																																																																											else
																																																																											{
																																																																												if (Main.tile[i, j].frameX == 162)
																																																																												{
																																																																													num8 = 223;
																																																																												}
																																																																												else
																																																																												{
																																																																													if (Main.tile[i, j].frameX >= 108 && Main.tile[i, j].frameX <= 126 && WorldGen.genRand.Next(100) == 0)
																																																																													{
																																																																														num8 = 208;
																																																																													}
																																																																													else
																																																																													{
																																																																														if (WorldGen.genRand.Next(100) == 0)
																																																																														{
																																																																															num8 = 195;
																																																																														}
																																																																													}
																																																																												}
																																																																											}
																																																																										}
																																																																										else
																																																																										{
																																																																											if (Main.tile[i, j].type == 59 || Main.tile[i, j].type == 60)
																																																																											{
																																																																												num8 = 176;
																																																																											}
																																																																											else
																																																																											{
																																																																												if (Main.tile[i, j].type == 71 || Main.tile[i, j].type == 72)
																																																																												{
																																																																													if (WorldGen.genRand.Next(50) == 0)
																																																																													{
																																																																														num8 = 194;
																																																																													}
																																																																													else
																																																																													{
																																																																														if (WorldGen.genRand.Next(2) == 0)
																																																																														{
																																																																															num8 = 183;
																																																																														}
																																																																													}
																																																																												}
																																																																												else
																																																																												{
																																																																													if (Main.tile[i, j].type >= 63 && Main.tile[i, j].type <= 68)
																																																																													{
																																																																														num8 = (int)(Main.tile[i, j].type - 63 + 177);
																																																																													}
																																																																													else
																																																																													{
																																																																														if (Main.tile[i, j].type == 50)
																																																																														{
																																																																															if (Main.tile[i, j].frameX == 90)
																																																																															{
																																																																																num8 = 165;
																																																																															}
																																																																															else
																																																																															{
																																																																																num8 = 149;
																																																																															}
																																																																														}
																																																																														else
																																																																														{
																																																																															if (Main.tileAlch[(int)Main.tile[i, j].type] && Main.tile[i, j].type > 82)
																																																																															{
																																																																																int num11 = (int)(Main.tile[i, j].frameX / 18);
																																																																																bool flag = false;
																																																																																if (Main.tile[i, j].type == 84)
																																																																																{
																																																																																	flag = true;
																																																																																}
																																																																																if (num11 == 0 && Main.dayTime)
																																																																																{
																																																																																	flag = true;
																																																																																}
																																																																																if (num11 == 1 && !Main.dayTime)
																																																																																{
																																																																																	flag = true;
																																																																																}
																																																																																if (num11 == 3 && Main.bloodMoon)
																																																																																{
																																																																																	flag = true;
																																																																																}
																																																																																num8 = 313 + num11;
																																																																																if (flag)
																																																																																{
																																																																																	Item.NewItem(i * 16, j * 16, 16, 16, 307 + num11, WorldGen.genRand.Next(1, 4), false, 0);
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
						if (num8 > 0)
						{
							Item.NewItem(i * 16, j * 16, 16, 16, num8, 1, false, -1);
						}
					}
					Main.tile[i, j].active = false;
					Main.tile[i, j].frameX = -1;
					Main.tile[i, j].frameY = -1;
					Main.tile[i, j].frameNumber = 0;
					if (Main.tile[i, j].type == 58 && j > Main.maxTilesY - 200)
					{
						Main.tile[i, j].lava = true;
						Main.tile[i, j].liquid = 128;
					}
					Main.tile[i, j].type = 0;
					WorldGen.SquareTileFrame(i, j, true);
				}
			}
		}
		public static bool PlayerLOS(int x, int y)
		{
			Rectangle rectangle = new Rectangle(x * 16, y * 16, 16, 16);
			for (int i = 0; i < 255; i++)
			{
				if (Main.player[i].active)
				{
					Rectangle value = new Rectangle((int)((double)Main.player[i].position.X + (double)Main.player[i].width * 0.5 - (double)NPC.sWidth * 0.6), (int)((double)Main.player[i].position.Y + (double)Main.player[i].height * 0.5 - (double)NPC.sHeight * 0.6), (int)((double)NPC.sWidth * 1.2), (int)((double)NPC.sHeight * 1.2));
					if (rectangle.Intersects(value))
					{
						return true;
					}
				}
			}
			return false;
		}
		public static void hardUpdateWorld(int i, int j)
		{
			if (Main.hardMode)
			{
				int type = (int)Main.tile[i, j].type;
				if (type == 117 && (double)j > Main.rockLayer && Main.rand.Next(110) == 0)
				{
					int num = WorldGen.genRand.Next(4);
					int num2 = 0;
					int num3 = 0;
					if (num == 0)
					{
						num2 = -1;
					}
					else
					{
						if (num == 1)
						{
							num2 = 1;
						}
						else
						{
							if (num == 0)
							{
								num3 = -1;
							}
							else
							{
								num3 = 1;
							}
						}
					}
					if (!Main.tile[i + num2, j + num3].active)
					{
						int num4 = 0;
						int num5 = 6;
						for (int k = i - num5; k <= i + num5; k++)
						{
							for (int l = j - num5; l <= j + num5; l++)
							{
								if (Main.tile[k, l].active && Main.tile[k, l].type == 129)
								{
									num4++;
								}
							}
						}
						if (num4 < 2)
						{
							WorldGen.PlaceTile(i + num2, j + num3, 129, true, false, -1, 0);
							NetMessage.SendTileSquare(-1, i + num2, j + num3, 1);
						}
					}
				}
				if (type == 23 || type == 25 || type == 32 || type == 112)
				{
					bool flag = true;
					while (flag)
					{
						flag = false;
						int num6 = i + WorldGen.genRand.Next(-3, 4);
						int num7 = j + WorldGen.genRand.Next(-3, 4);
						if (Main.tile[num6, num7].type == 2)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								flag = true;
							}
							Main.tile[num6, num7].type = 23;
							WorldGen.SquareTileFrame(num6, num7, true);
							NetMessage.SendTileSquare(-1, num6, num7, 1);
						}
						else
						{
							if (Main.tile[num6, num7].type == 1)
							{
								if (WorldGen.genRand.Next(2) == 0)
								{
									flag = true;
								}
								Main.tile[num6, num7].type = 25;
								WorldGen.SquareTileFrame(num6, num7, true);
								NetMessage.SendTileSquare(-1, num6, num7, 1);
							}
							else
							{
								if (Main.tile[num6, num7].type == 53)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										flag = true;
									}
									Main.tile[num6, num7].type = 112;
									WorldGen.SquareTileFrame(num6, num7, true);
									NetMessage.SendTileSquare(-1, num6, num7, 1);
								}
								else
								{
									if (Main.tile[num6, num7].type == 59)
									{
										if (WorldGen.genRand.Next(2) == 0)
										{
											flag = true;
										}
										Main.tile[num6, num7].type = 0;
										WorldGen.SquareTileFrame(num6, num7, true);
										NetMessage.SendTileSquare(-1, num6, num7, 1);
									}
									else
									{
										if (Main.tile[num6, num7].type == 60)
										{
											if (WorldGen.genRand.Next(2) == 0)
											{
												flag = true;
											}
											Main.tile[num6, num7].type = 23;
											WorldGen.SquareTileFrame(num6, num7, true);
											NetMessage.SendTileSquare(-1, num6, num7, 1);
										}
										else
										{
											if (Main.tile[num6, num7].type == 69)
											{
												if (WorldGen.genRand.Next(2) == 0)
												{
													flag = true;
												}
												Main.tile[num6, num7].type = 32;
												WorldGen.SquareTileFrame(num6, num7, true);
												NetMessage.SendTileSquare(-1, num6, num7, 1);
											}
										}
									}
								}
							}
						}
					}
				}
				if (type == 109 || type == 110 || type == 113 || type == 115 || type == 116 || type == 117 || type == 118)
				{
					bool flag2 = true;
					while (flag2)
					{
						flag2 = false;
						int num8 = i + WorldGen.genRand.Next(-3, 4);
						int num9 = j + WorldGen.genRand.Next(-3, 4);
						if (Main.tile[num8, num9].type == 2)
						{
							if (WorldGen.genRand.Next(2) == 0)
							{
								flag2 = true;
							}
							Main.tile[num8, num9].type = 109;
							WorldGen.SquareTileFrame(num8, num9, true);
							NetMessage.SendTileSquare(-1, num8, num9, 1);
						}
						else
						{
							if (Main.tile[num8, num9].type == 1)
							{
								if (WorldGen.genRand.Next(2) == 0)
								{
									flag2 = true;
								}
								Main.tile[num8, num9].type = 117;
								WorldGen.SquareTileFrame(num8, num9, true);
								NetMessage.SendTileSquare(-1, num8, num9, 1);
							}
							else
							{
								if (Main.tile[num8, num9].type == 53)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										flag2 = true;
									}
									Main.tile[num8, num9].type = 116;
									WorldGen.SquareTileFrame(num8, num9, true);
									NetMessage.SendTileSquare(-1, num8, num9, 1);
								}
							}
						}
					}
				}
			}
		}
		public static bool SolidTile(int i, int j)
		{
			try
			{
				if (Main.tile[i, j] == null)
				{
					bool result = true;
					return result;
				}
				if (Main.tile[i, j].active && Main.tileSolid[(int)Main.tile[i, j].type] && !Main.tileSolidTop[(int)Main.tile[i, j].type])
				{
					bool result = true;
					return result;
				}
			}
			catch
			{
			}
			return false;
		}
		public static void MineHouse(int i, int j)
		{
			if (i < 50 || i > Main.maxTilesX - 50 || j < 50 || j > Main.maxTilesY - 50)
			{
				return;
			}
			int num = WorldGen.genRand.Next(6, 12);
			int num2 = WorldGen.genRand.Next(3, 6);
			int num3 = WorldGen.genRand.Next(15, 30);
			int num4 = WorldGen.genRand.Next(15, 30);
			if (WorldGen.SolidTile(i, j))
			{
				return;
			}
			if (Main.tile[i, j].wall > 0)
			{
				return;
			}
			int num5 = j - num;
			int num6 = j + num2;
			for (int k = 0; k < 2; k++)
			{
				bool flag = true;
				int num7 = i;
				int num8 = j;
				int num9 = -1;
				int num10 = num3;
				if (k == 1)
				{
					num9 = 1;
					num10 = num4;
					num7++;
				}
				while (flag)
				{
					if (num8 - num < num5)
					{
						num5 = num8 - num;
					}
					if (num8 + num2 > num6)
					{
						num6 = num8 + num2;
					}
					for (int l = 0; l < 2; l++)
					{
						int num11 = num8;
						bool flag2 = true;
						int num12 = num;
						int num13 = -1;
						if (l == 1)
						{
							num11++;
							num12 = num2;
							num13 = 1;
						}
						while (flag2)
						{
							if (num7 != i && Main.tile[num7 - num9, num11].wall != 27 && (WorldGen.SolidTile(num7 - num9, num11) || !Main.tile[num7 - num9, num11].active))
							{
								Main.tile[num7 - num9, num11].active = true;
								Main.tile[num7 - num9, num11].type = 30;
							}
							if (WorldGen.SolidTile(num7 - 1, num11))
							{
								Main.tile[num7 - 1, num11].type = 30;
							}
							if (WorldGen.SolidTile(num7 + 1, num11))
							{
								Main.tile[num7 + 1, num11].type = 30;
							}
							if (WorldGen.SolidTile(num7, num11))
							{
								int num14 = 0;
								if (WorldGen.SolidTile(num7 - 1, num11))
								{
									num14++;
								}
								if (WorldGen.SolidTile(num7 + 1, num11))
								{
									num14++;
								}
								if (WorldGen.SolidTile(num7, num11 - 1))
								{
									num14++;
								}
								if (WorldGen.SolidTile(num7, num11 + 1))
								{
									num14++;
								}
								if (num14 < 2)
								{
									Main.tile[num7, num11].active = false;
								}
								else
								{
									flag2 = false;
									Main.tile[num7, num11].type = 30;
								}
							}
							else
							{
								Main.tile[num7, num11].wall = 27;
								Main.tile[num7, num11].liquid = 0;
								Main.tile[num7, num11].lava = false;
							}
							num11 += num13;
							num12--;
							if (num12 <= 0)
							{
								if (!Main.tile[num7, num11].active)
								{
									Main.tile[num7, num11].active = true;
									Main.tile[num7, num11].type = 30;
								}
								flag2 = false;
							}
						}
					}
					num10--;
					num7 += num9;
					if (WorldGen.SolidTile(num7, num8))
					{
						int num15 = 0;
						int num16 = 0;
						int num17 = num8;
						bool flag3 = true;
						while (flag3)
						{
							num17--;
							num15++;
							if (WorldGen.SolidTile(num7 - num9, num17))
							{
								num15 = 999;
								flag3 = false;
							}
							else
							{
								if (!WorldGen.SolidTile(num7, num17))
								{
									flag3 = false;
								}
							}
						}
						num17 = num8;
						flag3 = true;
						while (flag3)
						{
							num17++;
							num16++;
							if (WorldGen.SolidTile(num7 - num9, num17))
							{
								num16 = 999;
								flag3 = false;
							}
							else
							{
								if (!WorldGen.SolidTile(num7, num17))
								{
									flag3 = false;
								}
							}
						}
						if (num16 <= num15)
						{
							if (num16 > num2)
							{
								num10 = 0;
							}
							else
							{
								num8 += num16 + 1;
							}
						}
						else
						{
							if (num15 > num)
							{
								num10 = 0;
							}
							else
							{
								num8 -= num15 + 1;
							}
						}
					}
					if (num10 <= 0)
					{
						flag = false;
					}
				}
			}
			int num18 = i - num3 - 1;
			int num19 = i + num4 + 2;
			int num20 = num5 - 1;
			int num21 = num6 + 2;
			for (int m = num18; m < num19; m++)
			{
				for (int n = num20; n < num21; n++)
				{
					if (Main.tile[m, n].wall == 27 && !Main.tile[m, n].active)
					{
						if (Main.tile[m - 1, n].wall != 27 && m < i && !WorldGen.SolidTile(m - 1, n))
						{
							WorldGen.PlaceTile(m, n, 30, true, false, -1, 0);
							Main.tile[m, n].wall = 0;
						}
						if (Main.tile[m + 1, n].wall != 27 && m > i && !WorldGen.SolidTile(m + 1, n))
						{
							WorldGen.PlaceTile(m, n, 30, true, false, -1, 0);
							Main.tile[m, n].wall = 0;
						}
						for (int num22 = m - 1; num22 <= m + 1; num22++)
						{
							for (int num23 = n - 1; num23 <= n + 1; num23++)
							{
								if (WorldGen.SolidTile(num22, num23))
								{
									Main.tile[num22, num23].type = 30;
								}
							}
						}
					}
					if (Main.tile[m, n].type == 30 && Main.tile[m - 1, n].wall == 27 && Main.tile[m + 1, n].wall == 27 && (Main.tile[m, n - 1].wall == 27 || Main.tile[m, n - 1].active) && (Main.tile[m, n + 1].wall == 27 || Main.tile[m, n + 1].active))
					{
						Main.tile[m, n].active = false;
						Main.tile[m, n].wall = 27;
					}
				}
			}
			for (int num24 = num18; num24 < num19; num24++)
			{
				for (int num25 = num20; num25 < num21; num25++)
				{
					if (Main.tile[num24, num25].type == 30)
					{
						if (Main.tile[num24 - 1, num25].wall == 27 && Main.tile[num24 + 1, num25].wall == 27 && !Main.tile[num24 - 1, num25].active && !Main.tile[num24 + 1, num25].active)
						{
							Main.tile[num24, num25].active = false;
							Main.tile[num24, num25].wall = 27;
						}
						if (Main.tile[num24, num25 - 1].type != 21 && Main.tile[num24 - 1, num25].wall == 27 && Main.tile[num24 + 1, num25].type == 30 && Main.tile[num24 + 2, num25].wall == 27 && !Main.tile[num24 - 1, num25].active && !Main.tile[num24 + 2, num25].active)
						{
							Main.tile[num24, num25].active = false;
							Main.tile[num24, num25].wall = 27;
							Main.tile[num24 + 1, num25].active = false;
							Main.tile[num24 + 1, num25].wall = 27;
						}
						if (Main.tile[num24, num25 - 1].wall == 27 && Main.tile[num24, num25 + 1].wall == 27 && !Main.tile[num24, num25 - 1].active && !Main.tile[num24, num25 + 1].active)
						{
							Main.tile[num24, num25].active = false;
							Main.tile[num24, num25].wall = 27;
						}
					}
				}
			}
			for (int num26 = num18; num26 < num19; num26++)
			{
				for (int num27 = num21; num27 > num20; num27--)
				{
					bool flag4 = false;
					if (Main.tile[num26, num27].active && Main.tile[num26, num27].type == 30)
					{
						int num28 = -1;
						for (int num29 = 0; num29 < 2; num29++)
						{
							if (!WorldGen.SolidTile(num26 + num28, num27) && Main.tile[num26 + num28, num27].wall == 0)
							{
								int num30 = 0;
								int num31 = num27;
								int num32 = num27;
								while (Main.tile[num26, num31].active && Main.tile[num26, num31].type == 30 && !WorldGen.SolidTile(num26 + num28, num31) && Main.tile[num26 + num28, num31].wall == 0)
								{
									num31--;
									num30++;
								}
								num31++;
								int num33 = num31 + 1;
								if (num30 > 4)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										num31 = num32 - 1;
										bool flag5 = true;
										for (int num34 = num26 - 2; num34 <= num26 + 2; num34++)
										{
											for (int num35 = num31 - 2; num35 <= num31; num35++)
											{
												if (num34 != num26 && Main.tile[num34, num35].active)
												{
													flag5 = false;
												}
											}
										}
										if (flag5)
										{
											Main.tile[num26, num31].active = false;
											Main.tile[num26, num31 - 1].active = false;
											Main.tile[num26, num31 - 2].active = false;
											WorldGen.PlaceTile(num26, num31, 10, true, false, -1, 0);
											flag4 = true;
										}
									}
									if (!flag4)
									{
										for (int num36 = num33; num36 < num32; num36++)
										{
											Main.tile[num26, num36].type = 124;
										}
									}
								}
							}
							num28 = 1;
						}
					}
					if (flag4)
					{
						break;
					}
				}
			}
			for (int num37 = num18; num37 < num19; num37++)
			{
				bool flag6 = true;
				for (int num38 = num20; num38 < num21; num38++)
				{
					for (int num39 = num37 - 2; num39 <= num37 + 2; num39++)
					{
						if (Main.tile[num39, num38].active && (!WorldGen.SolidTile(num39, num38) || Main.tile[num39, num38].type == 10))
						{
							flag6 = false;
						}
					}
				}
				if (flag6)
				{
					for (int num40 = num20; num40 < num21; num40++)
					{
						if (Main.tile[num37, num40].wall == 27 && !Main.tile[num37, num40].active)
						{
							WorldGen.PlaceTile(num37, num40, 124, true, false, -1, 0);
						}
					}
				}
				num37 += WorldGen.genRand.Next(3);
			}
			for (int num41 = 0; num41 < 4; num41++)
			{
				int num42 = WorldGen.genRand.Next(num18 + 2, num19 - 1);
				int num43 = WorldGen.genRand.Next(num20 + 2, num21 - 1);
				while (Main.tile[num42, num43].wall != 27)
				{
					num42 = WorldGen.genRand.Next(num18 + 2, num19 - 1);
					num43 = WorldGen.genRand.Next(num20 + 2, num21 - 1);
				}
				while (Main.tile[num42, num43].active)
				{
					num43--;
				}
				while (!Main.tile[num42, num43].active)
				{
					num43++;
				}
				num43--;
				if (Main.tile[num42, num43].wall == 27)
				{
					if (WorldGen.genRand.Next(3) == 0)
					{
						int num44 = WorldGen.genRand.Next(9);
						if (num44 == 0)
						{
							num44 = 14;
						}
						if (num44 == 1)
						{
							num44 = 16;
						}
						if (num44 == 2)
						{
							num44 = 18;
						}
						if (num44 == 3)
						{
							num44 = 86;
						}
						if (num44 == 4)
						{
							num44 = 87;
						}
						if (num44 == 5)
						{
							num44 = 94;
						}
						if (num44 == 6)
						{
							num44 = 101;
						}
						if (num44 == 7)
						{
							num44 = 104;
						}
						if (num44 == 8)
						{
							num44 = 106;
						}
						WorldGen.PlaceTile(num42, num43, num44, true, false, -1, 0);
					}
					else
					{
						int style = WorldGen.genRand.Next(2, 43);
						WorldGen.PlaceTile(num42, num43, 105, true, true, -1, style);
					}
				}
			}
		}
		public static void CountTiles(int X)
		{
			if (X == 0)
			{
				WorldGen.totalEvil = WorldGen.totalEvil2;
				WorldGen.totalSolid = WorldGen.totalSolid2;
				WorldGen.totalGood = WorldGen.totalGood2;
				float num = (float)WorldGen.totalGood / (float)WorldGen.totalSolid;
				num = (float)Math.Round((double)(num * 100f));
				float num2 = (float)WorldGen.totalEvil / (float)WorldGen.totalSolid;
				num2 = (float)Math.Round((double)(num2 * 100f));
				WorldGen.tGood = (byte)num;
				WorldGen.tEvil = (byte)num2;
				if (Main.netMode == 2)
				{
					NetMessage.SendData(57, -1, -1, "", 0, 0f, 0f, 0f, 0);
				}
				WorldGen.totalEvil2 = 0;
				WorldGen.totalSolid2 = 0;
				WorldGen.totalGood2 = 0;
			}
			for (int i = 0; i < Main.maxTilesY; i++)
			{
				int num3 = 1;
				if ((double)i <= Main.worldSurface)
				{
					num3 *= 5;
				}
				if (WorldGen.SolidTile(X, i))
				{
					if (Main.tile[X, i].type == 109 || Main.tile[X, i].type == 116 || Main.tile[X, i].type == 117)
					{
						WorldGen.totalGood2 += num3;
					}
					else
					{
						if (Main.tile[X, i].type == 23 || Main.tile[X, i].type == 25 || Main.tile[X, i].type == 112)
						{
							WorldGen.totalEvil2 += num3;
						}
					}
					WorldGen.totalSolid2 += num3;
				}
			}
		}
		public static void UpdateWorld()
		{
			WorldGen.UpdateMech();
			WorldGen.totalD++;
			if (WorldGen.totalD >= 10)
			{
				WorldGen.totalD = 0;
				WorldGen.CountTiles(WorldGen.totalX);
				WorldGen.totalX++;
				if (WorldGen.totalX >= Main.maxTilesX)
				{
					WorldGen.totalX = 0;
				}
			}
			Liquid.skipCount++;
			if (Liquid.skipCount > 1)
			{
				Liquid.UpdateLiquid();
				Liquid.skipCount = 0;
			}
			float num = 3E-05f * (float)Main.worldRate;
			float num2 = 1.5E-05f * (float)Main.worldRate;
			bool flag = false;
			WorldGen.spawnDelay++;
			if (Main.invasionType > 0)
			{
				WorldGen.spawnDelay = 0;
			}
			if (WorldGen.spawnDelay >= 20)
			{
				flag = true;
				WorldGen.spawnDelay = 0;
				if (WorldGen.spawnNPC != 37)
				{
					for (int i = 0; i < 200; i++)
					{
						if (Main.npc[i].active && Main.npc[i].homeless && Main.npc[i].townNPC)
						{
							WorldGen.spawnNPC = Main.npc[i].type;
							break;
						}
					}
				}
			}
			int num3 = 0;
			while ((float)num3 < (float)(Main.maxTilesX * Main.maxTilesY) * num)
			{
				int num4 = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
				int num5 = WorldGen.genRand.Next(10, (int)Main.worldSurface - 1);
				int num6 = num4 - 1;
				int num7 = num4 + 2;
				int num8 = num5 - 1;
				int num9 = num5 + 2;
				if (num6 < 10)
				{
					num6 = 10;
				}
				if (num7 > Main.maxTilesX - 10)
				{
					num7 = Main.maxTilesX - 10;
				}
				if (num8 < 10)
				{
					num8 = 10;
				}
				if (num9 > Main.maxTilesY - 10)
				{
					num9 = Main.maxTilesY - 10;
				}
				if (Main.tile[num4, num5] != null)
				{
					if (Main.tileAlch[(int)Main.tile[num4, num5].type])
					{
						WorldGen.GrowAlch(num4, num5);
					}
					if (Main.tile[num4, num5].liquid > 32)
					{
						if (Main.tile[num4, num5].active && (Main.tile[num4, num5].type == 3 || Main.tile[num4, num5].type == 20 || Main.tile[num4, num5].type == 24 || Main.tile[num4, num5].type == 27 || Main.tile[num4, num5].type == 73))
						{
							WorldGen.KillTile(num4, num5, false, false, false);
							if (Main.netMode == 2)
							{
								NetMessage.SendData(17, -1, -1, "", 0, (float)num4, (float)num5, 0f, 0);
							}
						}
					}
					else
					{
						if (Main.tile[num4, num5].active)
						{
							WorldGen.hardUpdateWorld(num4, num5);
							if (Main.tile[num4, num5].type == 80)
							{
								if (WorldGen.genRand.Next(15) == 0)
								{
									WorldGen.GrowCactus(num4, num5);
								}
							}
							else
							{
								if (Main.tile[num4, num5].type == 53)
								{
									if (!Main.tile[num4, num8].active)
									{
										if (num4 < 250 || num4 > Main.maxTilesX - 250)
										{
											if (WorldGen.genRand.Next(500) == 0 && Main.tile[num4, num8].liquid == 255 && Main.tile[num4, num8 - 1].liquid == 255 && Main.tile[num4, num8 - 2].liquid == 255 && Main.tile[num4, num8 - 3].liquid == 255 && Main.tile[num4, num8 - 4].liquid == 255)
											{
												WorldGen.PlaceTile(num4, num8, 81, true, false, -1, 0);
												if (Main.netMode == 2 && Main.tile[num4, num8].active)
												{
													NetMessage.SendTileSquare(-1, num4, num8, 1);
												}
											}
										}
										else
										{
											if (num4 > 400 && num4 < Main.maxTilesX - 400 && WorldGen.genRand.Next(300) == 0)
											{
												WorldGen.GrowCactus(num4, num5);
											}
										}
									}
								}
								else
								{
									if (Main.tile[num4, num5].type == 116 || Main.tile[num4, num5].type == 112)
									{
										if (!Main.tile[num4, num8].active && num4 > 400 && num4 < Main.maxTilesX - 400 && WorldGen.genRand.Next(300) == 0)
										{
											WorldGen.GrowCactus(num4, num5);
										}
									}
									else
									{
										if (Main.tile[num4, num5].type == 78)
										{
											if (!Main.tile[num4, num8].active)
											{
												WorldGen.PlaceTile(num4, num8, 3, true, false, -1, 0);
												if (Main.netMode == 2 && Main.tile[num4, num8].active)
												{
													NetMessage.SendTileSquare(-1, num4, num8, 1);
												}
											}
										}
										else
										{
											if (Main.tile[num4, num5].type == 2 || Main.tile[num4, num5].type == 23 || Main.tile[num4, num5].type == 32 || Main.tile[num4, num5].type == 109)
											{
												int num10 = (int)Main.tile[num4, num5].type;
												if (!Main.tile[num4, num8].active && WorldGen.genRand.Next(12) == 0 && num10 == 2)
												{
													WorldGen.PlaceTile(num4, num8, 3, true, false, -1, 0);
													if (Main.netMode == 2 && Main.tile[num4, num8].active)
													{
														NetMessage.SendTileSquare(-1, num4, num8, 1);
													}
												}
												if (!Main.tile[num4, num8].active && WorldGen.genRand.Next(10) == 0 && num10 == 23)
												{
													WorldGen.PlaceTile(num4, num8, 24, true, false, -1, 0);
													if (Main.netMode == 2 && Main.tile[num4, num8].active)
													{
														NetMessage.SendTileSquare(-1, num4, num8, 1);
													}
												}
												if (!Main.tile[num4, num8].active && WorldGen.genRand.Next(10) == 0 && num10 == 109)
												{
													WorldGen.PlaceTile(num4, num8, 110, true, false, -1, 0);
													if (Main.netMode == 2 && Main.tile[num4, num8].active)
													{
														NetMessage.SendTileSquare(-1, num4, num8, 1);
													}
												}
												bool flag2 = false;
												for (int j = num6; j < num7; j++)
												{
													for (int k = num8; k < num9; k++)
													{
														if ((num4 != j || num5 != k) && Main.tile[j, k].active)
														{
															if (num10 == 32)
															{
																num10 = 23;
															}
															if (Main.tile[j, k].type == 0 || (num10 == 23 && Main.tile[j, k].type == 2) || (num10 == 23 && Main.tile[j, k].type == 109))
															{
																WorldGen.SpreadGrass(j, k, 0, num10, false);
																if (num10 == 23)
																{
																	WorldGen.SpreadGrass(j, k, 2, num10, false);
																}
																if (num10 == 23)
																{
																	WorldGen.SpreadGrass(j, k, 109, num10, false);
																}
																if ((int)Main.tile[j, k].type == num10)
																{
																	WorldGen.SquareTileFrame(j, k, true);
																	flag2 = true;
																}
															}
															if (Main.tile[j, k].type == 0 || (num10 == 109 && Main.tile[j, k].type == 2) || (num10 == 109 && Main.tile[j, k].type == 23))
															{
																WorldGen.SpreadGrass(j, k, 0, num10, false);
																if (num10 == 109)
																{
																	WorldGen.SpreadGrass(j, k, 2, num10, false);
																}
																if (num10 == 109)
																{
																	WorldGen.SpreadGrass(j, k, 23, num10, false);
																}
																if ((int)Main.tile[j, k].type == num10)
																{
																	WorldGen.SquareTileFrame(j, k, true);
																	flag2 = true;
																}
															}
														}
													}
												}
												if (Main.netMode == 2 && flag2)
												{
													NetMessage.SendTileSquare(-1, num4, num5, 3);
												}
											}
											else
											{
												if (Main.tile[num4, num5].type == 20 && WorldGen.genRand.Next(20) == 0 && !WorldGen.PlayerLOS(num4, num5))
												{
													WorldGen.GrowTree(num4, num5);
												}
											}
										}
									}
								}
							}
							if (Main.tile[num4, num5].type == 3 && WorldGen.genRand.Next(20) == 0 && Main.tile[num4, num5].frameX < 144)
							{
								Main.tile[num4, num5].type = 73;
								if (Main.netMode == 2)
								{
									NetMessage.SendTileSquare(-1, num4, num5, 3);
								}
							}
							if (Main.tile[num4, num5].type == 110 && WorldGen.genRand.Next(20) == 0 && Main.tile[num4, num5].frameX < 144)
							{
								Main.tile[num4, num5].type = 113;
								if (Main.netMode == 2)
								{
									NetMessage.SendTileSquare(-1, num4, num5, 3);
								}
							}
							if (Main.tile[num4, num5].type == 32 && WorldGen.genRand.Next(3) == 0)
							{
								int num11 = num4;
								int num12 = num5;
								int num13 = 0;
								if (Main.tile[num11 + 1, num12].active && Main.tile[num11 + 1, num12].type == 32)
								{
									num13++;
								}
								if (Main.tile[num11 - 1, num12].active && Main.tile[num11 - 1, num12].type == 32)
								{
									num13++;
								}
								if (Main.tile[num11, num12 + 1].active && Main.tile[num11, num12 + 1].type == 32)
								{
									num13++;
								}
								if (Main.tile[num11, num12 - 1].active && Main.tile[num11, num12 - 1].type == 32)
								{
									num13++;
								}
								if (num13 < 3 || Main.tile[num4, num5].type == 23)
								{
									int num14 = WorldGen.genRand.Next(4);
									if (num14 == 0)
									{
										num12--;
									}
									else
									{
										if (num14 == 1)
										{
											num12++;
										}
										else
										{
											if (num14 == 2)
											{
												num11--;
											}
											else
											{
												if (num14 == 3)
												{
													num11++;
												}
											}
										}
									}
									if (!Main.tile[num11, num12].active)
									{
										num13 = 0;
										if (Main.tile[num11 + 1, num12].active && Main.tile[num11 + 1, num12].type == 32)
										{
											num13++;
										}
										if (Main.tile[num11 - 1, num12].active && Main.tile[num11 - 1, num12].type == 32)
										{
											num13++;
										}
										if (Main.tile[num11, num12 + 1].active && Main.tile[num11, num12 + 1].type == 32)
										{
											num13++;
										}
										if (Main.tile[num11, num12 - 1].active && Main.tile[num11, num12 - 1].type == 32)
										{
											num13++;
										}
										if (num13 < 2)
										{
											int num15 = 7;
											int num16 = num11 - num15;
											int num17 = num11 + num15;
											int num18 = num12 - num15;
											int num19 = num12 + num15;
											bool flag3 = false;
											for (int l = num16; l < num17; l++)
											{
												for (int m = num18; m < num19; m++)
												{
													if (Math.Abs(l - num11) * 2 + Math.Abs(m - num12) < 9 && Main.tile[l, m].active && Main.tile[l, m].type == 23 && Main.tile[l, m - 1].active && Main.tile[l, m - 1].type == 32 && Main.tile[l, m - 1].liquid == 0)
													{
														flag3 = true;
														break;
													}
												}
											}
											if (flag3)
											{
												Main.tile[num11, num12].type = 32;
												Main.tile[num11, num12].active = true;
												WorldGen.SquareTileFrame(num11, num12, true);
												if (Main.netMode == 2)
												{
													NetMessage.SendTileSquare(-1, num11, num12, 3);
												}
											}
										}
									}
								}
							}
						}
						else
						{
							if (flag && WorldGen.spawnNPC > 0)
							{
								WorldGen.SpawnNPC(num4, num5);
							}
						}
					}
					if (Main.tile[num4, num5].active)
					{
						if ((Main.tile[num4, num5].type == 2 || Main.tile[num4, num5].type == 52) && WorldGen.genRand.Next(40) == 0 && !Main.tile[num4, num5 + 1].active && !Main.tile[num4, num5 + 1].lava)
						{
							bool flag4 = false;
							for (int n = num5; n > num5 - 10; n--)
							{
								if (Main.tile[num4, n].active && Main.tile[num4, n].type == 2)
								{
									flag4 = true;
									break;
								}
							}
							if (flag4)
							{
								int num20 = num4;
								int num21 = num5 + 1;
								Main.tile[num20, num21].type = 52;
								Main.tile[num20, num21].active = true;
								WorldGen.SquareTileFrame(num20, num21, true);
								if (Main.netMode == 2)
								{
									NetMessage.SendTileSquare(-1, num20, num21, 3);
								}
							}
						}
						if (Main.tile[num4, num5].type == 60)
						{
							int type = (int)Main.tile[num4, num5].type;
							if (!Main.tile[num4, num8].active && WorldGen.genRand.Next(7) == 0)
							{
								WorldGen.PlaceTile(num4, num8, 61, true, false, -1, 0);
								if (Main.netMode == 2 && Main.tile[num4, num8].active)
								{
									NetMessage.SendTileSquare(-1, num4, num8, 1);
								}
							}
							else
							{
								if (WorldGen.genRand.Next(500) == 0 && (!Main.tile[num4, num8].active || Main.tile[num4, num8].type == 61 || Main.tile[num4, num8].type == 74 || Main.tile[num4, num8].type == 69) && !WorldGen.PlayerLOS(num4, num5))
								{
									WorldGen.GrowTree(num4, num5);
								}
							}
							bool flag5 = false;
							for (int num22 = num6; num22 < num7; num22++)
							{
								for (int num23 = num8; num23 < num9; num23++)
								{
									if ((num4 != num22 || num5 != num23) && Main.tile[num22, num23].active && Main.tile[num22, num23].type == 59)
									{
										WorldGen.SpreadGrass(num22, num23, 59, type, false);
										if ((int)Main.tile[num22, num23].type == type)
										{
											WorldGen.SquareTileFrame(num22, num23, true);
											flag5 = true;
										}
									}
								}
							}
							if (Main.netMode == 2 && flag5)
							{
								NetMessage.SendTileSquare(-1, num4, num5, 3);
							}
						}
						if (Main.tile[num4, num5].type == 61 && WorldGen.genRand.Next(3) == 0 && Main.tile[num4, num5].frameX < 144)
						{
							Main.tile[num4, num5].type = 74;
							if (Main.netMode == 2)
							{
								NetMessage.SendTileSquare(-1, num4, num5, 3);
							}
						}
						if ((Main.tile[num4, num5].type == 60 || Main.tile[num4, num5].type == 62) && WorldGen.genRand.Next(15) == 0 && !Main.tile[num4, num5 + 1].active && !Main.tile[num4, num5 + 1].lava)
						{
							bool flag6 = false;
							for (int num24 = num5; num24 > num5 - 10; num24--)
							{
								if (Main.tile[num4, num24].active && Main.tile[num4, num24].type == 60)
								{
									flag6 = true;
									break;
								}
							}
							if (flag6)
							{
								int num25 = num4;
								int num26 = num5 + 1;
								Main.tile[num25, num26].type = 62;
								Main.tile[num25, num26].active = true;
								WorldGen.SquareTileFrame(num25, num26, true);
								if (Main.netMode == 2)
								{
									NetMessage.SendTileSquare(-1, num25, num26, 3);
								}
							}
						}
						if ((Main.tile[num4, num5].type == 109 || Main.tile[num4, num5].type == 115) && WorldGen.genRand.Next(15) == 0 && !Main.tile[num4, num5 + 1].active && !Main.tile[num4, num5 + 1].lava)
						{
							bool flag7 = false;
							for (int num27 = num5; num27 > num5 - 10; num27--)
							{
								if (Main.tile[num4, num27].active && Main.tile[num4, num27].type == 109)
								{
									flag7 = true;
									break;
								}
							}
							if (flag7)
							{
								int num28 = num4;
								int num29 = num5 + 1;
								Main.tile[num28, num29].type = 115;
								Main.tile[num28, num29].active = true;
								WorldGen.SquareTileFrame(num28, num29, true);
								if (Main.netMode == 2)
								{
									NetMessage.SendTileSquare(-1, num28, num29, 3);
								}
							}
						}
					}
				}
				num3++;
			}
			int num30 = 0;
			while ((float)num30 < (float)(Main.maxTilesX * Main.maxTilesY) * num2)
			{
				int num31 = WorldGen.genRand.Next(10, Main.maxTilesX - 10);
				int num32 = WorldGen.genRand.Next((int)Main.worldSurface - 1, Main.maxTilesY - 20);
				int num33 = num31 - 1;
				int num34 = num31 + 2;
				int num35 = num32 - 1;
				int num36 = num32 + 2;
				if (num33 < 10)
				{
					num33 = 10;
				}
				if (num34 > Main.maxTilesX - 10)
				{
					num34 = Main.maxTilesX - 10;
				}
				if (num35 < 10)
				{
					num35 = 10;
				}
				if (num36 > Main.maxTilesY - 10)
				{
					num36 = Main.maxTilesY - 10;
				}
				if (Main.tile[num31, num32] != null)
				{
					if (Main.tileAlch[(int)Main.tile[num31, num32].type])
					{
						WorldGen.GrowAlch(num31, num32);
					}
					if (Main.tile[num31, num32].liquid <= 32)
					{
						if (Main.tile[num31, num32].active)
						{
							WorldGen.hardUpdateWorld(num31, num32);
							if (Main.tile[num31, num32].type == 23 && !Main.tile[num31, num35].active && WorldGen.genRand.Next(1) == 0)
							{
								WorldGen.PlaceTile(num31, num35, 24, true, false, -1, 0);
								if (Main.netMode == 2 && Main.tile[num31, num35].active)
								{
									NetMessage.SendTileSquare(-1, num31, num35, 1);
								}
							}
							if (Main.tile[num31, num32].type == 32 && WorldGen.genRand.Next(3) == 0)
							{
								int num37 = num31;
								int num38 = num32;
								int num39 = 0;
								if (Main.tile[num37 + 1, num38].active && Main.tile[num37 + 1, num38].type == 32)
								{
									num39++;
								}
								if (Main.tile[num37 - 1, num38].active && Main.tile[num37 - 1, num38].type == 32)
								{
									num39++;
								}
								if (Main.tile[num37, num38 + 1].active && Main.tile[num37, num38 + 1].type == 32)
								{
									num39++;
								}
								if (Main.tile[num37, num38 - 1].active && Main.tile[num37, num38 - 1].type == 32)
								{
									num39++;
								}
								if (num39 < 3 || Main.tile[num31, num32].type == 23)
								{
									int num40 = WorldGen.genRand.Next(4);
									if (num40 == 0)
									{
										num38--;
									}
									else
									{
										if (num40 == 1)
										{
											num38++;
										}
										else
										{
											if (num40 == 2)
											{
												num37--;
											}
											else
											{
												if (num40 == 3)
												{
													num37++;
												}
											}
										}
									}
									if (!Main.tile[num37, num38].active)
									{
										num39 = 0;
										if (Main.tile[num37 + 1, num38].active && Main.tile[num37 + 1, num38].type == 32)
										{
											num39++;
										}
										if (Main.tile[num37 - 1, num38].active && Main.tile[num37 - 1, num38].type == 32)
										{
											num39++;
										}
										if (Main.tile[num37, num38 + 1].active && Main.tile[num37, num38 + 1].type == 32)
										{
											num39++;
										}
										if (Main.tile[num37, num38 - 1].active && Main.tile[num37, num38 - 1].type == 32)
										{
											num39++;
										}
										if (num39 < 2)
										{
											int num41 = 7;
											int num42 = num37 - num41;
											int num43 = num37 + num41;
											int num44 = num38 - num41;
											int num45 = num38 + num41;
											bool flag8 = false;
											for (int num46 = num42; num46 < num43; num46++)
											{
												for (int num47 = num44; num47 < num45; num47++)
												{
													if (Math.Abs(num46 - num37) * 2 + Math.Abs(num47 - num38) < 9 && Main.tile[num46, num47].active && Main.tile[num46, num47].type == 23 && Main.tile[num46, num47 - 1].active && Main.tile[num46, num47 - 1].type == 32 && Main.tile[num46, num47 - 1].liquid == 0)
													{
														flag8 = true;
														break;
													}
												}
											}
											if (flag8)
											{
												Main.tile[num37, num38].type = 32;
												Main.tile[num37, num38].active = true;
												WorldGen.SquareTileFrame(num37, num38, true);
												if (Main.netMode == 2)
												{
													NetMessage.SendTileSquare(-1, num37, num38, 3);
												}
											}
										}
									}
								}
							}
							if (Main.tile[num31, num32].type == 60)
							{
								int type2 = (int)Main.tile[num31, num32].type;
								if (!Main.tile[num31, num35].active && WorldGen.genRand.Next(10) == 0)
								{
									WorldGen.PlaceTile(num31, num35, 61, true, false, -1, 0);
									if (Main.netMode == 2 && Main.tile[num31, num35].active)
									{
										NetMessage.SendTileSquare(-1, num31, num35, 1);
									}
								}
								bool flag9 = false;
								for (int num48 = num33; num48 < num34; num48++)
								{
									for (int num49 = num35; num49 < num36; num49++)
									{
										if ((num31 != num48 || num32 != num49) && Main.tile[num48, num49].active && Main.tile[num48, num49].type == 59)
										{
											WorldGen.SpreadGrass(num48, num49, 59, type2, false);
											if ((int)Main.tile[num48, num49].type == type2)
											{
												WorldGen.SquareTileFrame(num48, num49, true);
												flag9 = true;
											}
										}
									}
								}
								if (Main.netMode == 2 && flag9)
								{
									NetMessage.SendTileSquare(-1, num31, num32, 3);
								}
							}
							if (Main.tile[num31, num32].type == 61 && WorldGen.genRand.Next(3) == 0 && Main.tile[num31, num32].frameX < 144)
							{
								Main.tile[num31, num32].type = 74;
								if (Main.netMode == 2)
								{
									NetMessage.SendTileSquare(-1, num31, num32, 3);
								}
							}
							if ((Main.tile[num31, num32].type == 60 || Main.tile[num31, num32].type == 62) && WorldGen.genRand.Next(5) == 0 && !Main.tile[num31, num32 + 1].active && !Main.tile[num31, num32 + 1].lava)
							{
								bool flag10 = false;
								for (int num50 = num32; num50 > num32 - 10; num50--)
								{
									if (Main.tile[num31, num50].active && Main.tile[num31, num50].type == 60)
									{
										flag10 = true;
										break;
									}
								}
								if (flag10)
								{
									int num51 = num31;
									int num52 = num32 + 1;
									Main.tile[num51, num52].type = 62;
									Main.tile[num51, num52].active = true;
									WorldGen.SquareTileFrame(num51, num52, true);
									if (Main.netMode == 2)
									{
										NetMessage.SendTileSquare(-1, num51, num52, 3);
									}
								}
							}
							if (Main.tile[num31, num32].type == 69 && WorldGen.genRand.Next(3) == 0)
							{
								int num53 = num31;
								int num54 = num32;
								int num55 = 0;
								if (Main.tile[num53 + 1, num54].active && Main.tile[num53 + 1, num54].type == 69)
								{
									num55++;
								}
								if (Main.tile[num53 - 1, num54].active && Main.tile[num53 - 1, num54].type == 69)
								{
									num55++;
								}
								if (Main.tile[num53, num54 + 1].active && Main.tile[num53, num54 + 1].type == 69)
								{
									num55++;
								}
								if (Main.tile[num53, num54 - 1].active && Main.tile[num53, num54 - 1].type == 69)
								{
									num55++;
								}
								if (num55 < 3 || Main.tile[num31, num32].type == 60)
								{
									int num56 = WorldGen.genRand.Next(4);
									if (num56 == 0)
									{
										num54--;
									}
									else
									{
										if (num56 == 1)
										{
											num54++;
										}
										else
										{
											if (num56 == 2)
											{
												num53--;
											}
											else
											{
												if (num56 == 3)
												{
													num53++;
												}
											}
										}
									}
									if (!Main.tile[num53, num54].active)
									{
										num55 = 0;
										if (Main.tile[num53 + 1, num54].active && Main.tile[num53 + 1, num54].type == 69)
										{
											num55++;
										}
										if (Main.tile[num53 - 1, num54].active && Main.tile[num53 - 1, num54].type == 69)
										{
											num55++;
										}
										if (Main.tile[num53, num54 + 1].active && Main.tile[num53, num54 + 1].type == 69)
										{
											num55++;
										}
										if (Main.tile[num53, num54 - 1].active && Main.tile[num53, num54 - 1].type == 69)
										{
											num55++;
										}
										if (num55 < 2)
										{
											int num57 = 7;
											int num58 = num53 - num57;
											int num59 = num53 + num57;
											int num60 = num54 - num57;
											int num61 = num54 + num57;
											bool flag11 = false;
											for (int num62 = num58; num62 < num59; num62++)
											{
												for (int num63 = num60; num63 < num61; num63++)
												{
													if (Math.Abs(num62 - num53) * 2 + Math.Abs(num63 - num54) < 9 && Main.tile[num62, num63].active && Main.tile[num62, num63].type == 60 && Main.tile[num62, num63 - 1].active && Main.tile[num62, num63 - 1].type == 69 && Main.tile[num62, num63 - 1].liquid == 0)
													{
														flag11 = true;
														break;
													}
												}
											}
											if (flag11)
											{
												Main.tile[num53, num54].type = 69;
												Main.tile[num53, num54].active = true;
												WorldGen.SquareTileFrame(num53, num54, true);
												if (Main.netMode == 2)
												{
													NetMessage.SendTileSquare(-1, num53, num54, 3);
												}
											}
										}
									}
								}
							}
							if (Main.tile[num31, num32].type == 70)
							{
								int type3 = (int)Main.tile[num31, num32].type;
								if (!Main.tile[num31, num35].active && WorldGen.genRand.Next(10) == 0)
								{
									WorldGen.PlaceTile(num31, num35, 71, true, false, -1, 0);
									if (Main.netMode == 2 && Main.tile[num31, num35].active)
									{
										NetMessage.SendTileSquare(-1, num31, num35, 1);
									}
								}
								if (WorldGen.genRand.Next(200) == 0 && !WorldGen.PlayerLOS(num31, num32))
								{
									WorldGen.GrowShroom(num31, num32);
								}
								bool flag12 = false;
								for (int num64 = num33; num64 < num34; num64++)
								{
									for (int num65 = num35; num65 < num36; num65++)
									{
										if ((num31 != num64 || num32 != num65) && Main.tile[num64, num65].active && Main.tile[num64, num65].type == 59)
										{
											WorldGen.SpreadGrass(num64, num65, 59, type3, false);
											if ((int)Main.tile[num64, num65].type == type3)
											{
												WorldGen.SquareTileFrame(num64, num65, true);
												flag12 = true;
											}
										}
									}
								}
								if (Main.netMode == 2 && flag12)
								{
									NetMessage.SendTileSquare(-1, num31, num32, 3);
								}
							}
						}
						else
						{
							if (flag && WorldGen.spawnNPC > 0)
							{
								WorldGen.SpawnNPC(num31, num32);
							}
						}
					}
				}
				num30++;
			}
			if (Main.rand.Next(100) == 0)
			{
				WorldGen.PlantAlch();
			}
			if (!Main.dayTime)
			{
				float num66 = (float)(Main.maxTilesX / 4200);
				if ((float)Main.rand.Next(8000) < 10f * num66)
				{
					int num67 = 12;
					int num68 = Main.rand.Next(Main.maxTilesX - 50) + 100;
					num68 *= 16;
					int num69 = Main.rand.Next((int)((double)Main.maxTilesY * 0.05));
					num69 *= 16;
					Vector2 vector = new Vector2((float)num68, (float)num69);
					float num70 = (float)Main.rand.Next(-100, 101);
					float num71 = (float)(Main.rand.Next(200) + 100);
					float num72 = (float)Math.Sqrt((double)(num70 * num70 + num71 * num71));
					num72 = (float)num67 / num72;
					num70 *= num72;
					num71 *= num72;
					Projectile.NewProjectile(vector.X, vector.Y, num70, num71, 12, 1000, 10f, Main.myPlayer);
				}
			}
		}
		public static void PlaceWall(int i, int j, int type, bool mute = false)
		{
			if (i <= 1 || j <= 1 || i >= Main.maxTilesX - 2 || j >= Main.maxTilesY - 2)
			{
				return;
			}
			if (Main.tile[i, j].wall == 0)
			{
				Main.tile[i, j].wall = (byte)type;
				WorldGen.SquareWallFrame(i, j, true);
				if (!mute)
				{
					Main.PlaySound(0, i * 16, j * 16, 1);
				}
			}
		}
		public static void AddPlants()
		{
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				for (int j = 1; j < Main.maxTilesY; j++)
				{
					if (Main.tile[i, j].type == 2 && Main.tile[i, j].active)
					{
						if (!Main.tile[i, j - 1].active)
						{
							WorldGen.PlaceTile(i, j - 1, 3, true, false, -1, 0);
						}
					}
					else
					{
						if (Main.tile[i, j].type == 23 && Main.tile[i, j].active && !Main.tile[i, j - 1].active)
						{
							WorldGen.PlaceTile(i, j - 1, 24, true, false, -1, 0);
						}
					}
				}
			}
		}
		public static void SpreadGrass(int i, int j, int dirt = 0, int grass = 2, bool repeat = true)
		{
			try
			{
				if ((int)Main.tile[i, j].type == dirt && Main.tile[i, j].active && ((double)j >= Main.worldSurface || grass != 70) && ((double)j < Main.worldSurface || dirt != 0))
				{
					int num = i - 1;
					int num2 = i + 2;
					int num3 = j - 1;
					int num4 = j + 2;
					if (num < 0)
					{
						num = 0;
					}
					if (num2 > Main.maxTilesX)
					{
						num2 = Main.maxTilesX;
					}
					if (num3 < 0)
					{
						num3 = 0;
					}
					if (num4 > Main.maxTilesY)
					{
						num4 = Main.maxTilesY;
					}
					bool flag = true;
					for (int k = num; k < num2; k++)
					{
						for (int l = num3; l < num4; l++)
						{
							if (!Main.tile[k, l].active || !Main.tileSolid[(int)Main.tile[k, l].type])
							{
								flag = false;
							}
							if (Main.tile[k, l].lava && Main.tile[k, l].liquid > 0)
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						if (grass != 23 || Main.tile[i, j - 1].type != 27)
						{
							Main.tile[i, j].type = (byte)grass;
							for (int m = num; m < num2; m++)
							{
								for (int n = num3; n < num4; n++)
								{
									if (Main.tile[m, n].active && (int)Main.tile[m, n].type == dirt)
									{
										try
										{
											if (repeat && WorldGen.grassSpread < 1000)
											{
												WorldGen.grassSpread++;
												WorldGen.SpreadGrass(m, n, dirt, grass, true);
												WorldGen.grassSpread--;
											}
										}
										catch
										{
										}
									}
								}
							}
						}
					}
				}
			}
			catch
			{
			}
		}
		public static void ChasmRunnerSideways(int i, int j, int direction, int steps)
		{
			float num = (float)steps;
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j;
			Vector2 value2;
			value2.X = (float)WorldGen.genRand.Next(10, 21) * 0.1f * (float)direction;
			value2.Y = (float)WorldGen.genRand.Next(-10, 10) * 0.01f;
			double num2 = (double)(WorldGen.genRand.Next(5) + 7);
			while (num2 > 0.0)
			{
				if (num > 0f)
				{
					num2 += (double)WorldGen.genRand.Next(3);
					num2 -= (double)WorldGen.genRand.Next(3);
					if (num2 < 7.0)
					{
						num2 = 7.0;
					}
					if (num2 > 20.0)
					{
						num2 = 20.0;
					}
					if (num == 1f && num2 < 10.0)
					{
						num2 = 10.0;
					}
				}
				else
				{
					num2 -= (double)WorldGen.genRand.Next(4);
				}
				if ((double)value.Y > Main.rockLayer && num > 0f)
				{
					num = 0f;
				}
				num -= 1f;
				int num3 = (int)((double)value.X - num2 * 0.5);
				int num4 = (int)((double)value.X + num2 * 0.5);
				int num5 = (int)((double)value.Y - num2 * 0.5);
				int num6 = (int)((double)value.Y + num2 * 0.5);
				if (num3 < 0)
				{
					num3 = 0;
				}
				if (num4 > Main.maxTilesX - 1)
				{
					num4 = Main.maxTilesX - 1;
				}
				if (num5 < 0)
				{
					num5 = 0;
				}
				if (num6 > Main.maxTilesY)
				{
					num6 = Main.maxTilesY;
				}
				for (int k = num3; k < num4; k++)
				{
					for (int l = num5; l < num6; l++)
					{
						if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < num2 * 0.5 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015) && Main.tile[k, l].type != 31 && Main.tile[k, l].type != 22)
						{
							Main.tile[k, l].active = false;
						}
					}
				}
				value += value2;
				value2.Y += (float)WorldGen.genRand.Next(-10, 10) * 0.1f;
				if (value.Y < (float)(j - 20))
				{
					value2.Y += (float)WorldGen.genRand.Next(20) * 0.01f;
				}
				if (value.Y > (float)(j + 20))
				{
					value2.Y -= (float)WorldGen.genRand.Next(20) * 0.01f;
				}
				if ((double)value2.Y < -0.5)
				{
					value2.Y = -0.5f;
				}
				if ((double)value2.Y > 0.5)
				{
					value2.Y = 0.5f;
				}
				value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.01f;
				if (direction == -1)
				{
					if ((double)value2.X > -0.5)
					{
						value2.X = -0.5f;
					}
					if (value2.X < -2f)
					{
						value2.X = -2f;
					}
				}
				else
				{
					if (direction == 1)
					{
						if ((double)value2.X < 0.5)
						{
							value2.X = 0.5f;
						}
						if (value2.X > 2f)
						{
							value2.X = 2f;
						}
					}
				}
				num3 = (int)((double)value.X - num2 * 1.1);
				num4 = (int)((double)value.X + num2 * 1.1);
				num5 = (int)((double)value.Y - num2 * 1.1);
				num6 = (int)((double)value.Y + num2 * 1.1);
				if (num3 < 1)
				{
					num3 = 1;
				}
				if (num4 > Main.maxTilesX - 1)
				{
					num4 = Main.maxTilesX - 1;
				}
				if (num5 < 0)
				{
					num5 = 0;
				}
				if (num6 > Main.maxTilesY)
				{
					num6 = Main.maxTilesY;
				}
				for (int m = num3; m < num4; m++)
				{
					for (int n = num5; n < num6; n++)
					{
						if ((double)(Math.Abs((float)m - value.X) + Math.Abs((float)n - value.Y)) < num2 * 1.1 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015) && Main.tile[m, n].wall != 3)
						{
							if (Main.tile[m, n].type != 25 && n > j + WorldGen.genRand.Next(3, 20))
							{
								Main.tile[m, n].active = true;
							}
							Main.tile[m, n].active = true;
							if (Main.tile[m, n].type != 31 && Main.tile[m, n].type != 22)
							{
								Main.tile[m, n].type = 25;
							}
							if (Main.tile[m, n].wall == 2)
							{
								Main.tile[m, n].wall = 0;
							}
						}
					}
				}
				for (int num7 = num3; num7 < num4; num7++)
				{
					for (int num8 = num5; num8 < num6; num8++)
					{
						if ((double)(Math.Abs((float)num7 - value.X) + Math.Abs((float)num8 - value.Y)) < num2 * 1.1 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015) && Main.tile[num7, num8].wall != 3)
						{
							if (Main.tile[num7, num8].type != 31 && Main.tile[num7, num8].type != 22)
							{
								Main.tile[num7, num8].type = 25;
							}
							Main.tile[num7, num8].active = true;
							WorldGen.PlaceWall(num7, num8, 3, true);
						}
					}
				}
			}
			if (WorldGen.genRand.Next(3) == 0)
			{
				int num9 = (int)value.X;
				int num10 = (int)value.Y;
				while (!Main.tile[num9, num10].active)
				{
					num10++;
				}
				WorldGen.TileRunner(num9, num10, (double)WorldGen.genRand.Next(2, 6), WorldGen.genRand.Next(3, 7), 22, false, 0f, 0f, false, true);
			}
		}
		public static void ChasmRunner(int i, int j, int steps, bool makeOrb = false)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (!makeOrb)
			{
				flag2 = true;
			}
			float num = (float)steps;
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j;
			Vector2 value2;
			value2.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
			value2.Y = (float)WorldGen.genRand.Next(11) * 0.2f + 0.5f;
			int num2 = 5;
			double num3 = (double)(WorldGen.genRand.Next(5) + 7);
			while (num3 > 0.0)
			{
				if (num > 0f)
				{
					num3 += (double)WorldGen.genRand.Next(3);
					num3 -= (double)WorldGen.genRand.Next(3);
					if (num3 < 7.0)
					{
						num3 = 7.0;
					}
					if (num3 > 20.0)
					{
						num3 = 20.0;
					}
					if (num == 1f && num3 < 10.0)
					{
						num3 = 10.0;
					}
				}
				else
				{
					if ((double)value.Y > Main.worldSurface + 45.0)
					{
						num3 -= (double)WorldGen.genRand.Next(4);
					}
				}
				if ((double)value.Y > Main.rockLayer && num > 0f)
				{
					num = 0f;
				}
				num -= 1f;
				if (!flag && (double)value.Y > Main.worldSurface + 20.0)
				{
					flag = true;
					WorldGen.ChasmRunnerSideways((int)value.X, (int)value.Y, -1, WorldGen.genRand.Next(20, 40));
					WorldGen.ChasmRunnerSideways((int)value.X, (int)value.Y, 1, WorldGen.genRand.Next(20, 40));
				}
				int num4;
				int num5;
				int num6;
				int num7;
				if (num > (float)num2)
				{
					num4 = (int)((double)value.X - num3 * 0.5);
					num5 = (int)((double)value.X + num3 * 0.5);
					num6 = (int)((double)value.Y - num3 * 0.5);
					num7 = (int)((double)value.Y + num3 * 0.5);
					if (num4 < 0)
					{
						num4 = 0;
					}
					if (num5 > Main.maxTilesX - 1)
					{
						num5 = Main.maxTilesX - 1;
					}
					if (num6 < 0)
					{
						num6 = 0;
					}
					if (num7 > Main.maxTilesY)
					{
						num7 = Main.maxTilesY;
					}
					for (int k = num4; k < num5; k++)
					{
						for (int l = num6; l < num7; l++)
						{
							if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < num3 * 0.5 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015) && Main.tile[k, l].type != 31 && Main.tile[k, l].type != 22)
							{
								Main.tile[k, l].active = false;
							}
						}
					}
				}
				if (num <= 2f && (double)value.Y < Main.worldSurface + 45.0)
				{
					num = 2f;
				}
				if (num <= 0f)
				{
					if (!flag2)
					{
						flag2 = true;
						WorldGen.AddShadowOrb((int)value.X, (int)value.Y);
					}
					else
					{
						if (!flag3)
						{
							flag3 = false;
							bool flag4 = false;
							int num8 = 0;
							while (!flag4)
							{
								int num9 = WorldGen.genRand.Next((int)value.X - 25, (int)value.X + 25);
								int num10 = WorldGen.genRand.Next((int)value.Y - 50, (int)value.Y);
								if (num9 < 5)
								{
									num9 = 5;
								}
								if (num9 > Main.maxTilesX - 5)
								{
									num9 = Main.maxTilesX - 5;
								}
								if (num10 < 5)
								{
									num10 = 5;
								}
								if (num10 > Main.maxTilesY - 5)
								{
									num10 = Main.maxTilesY - 5;
								}
								if ((double)num10 > Main.worldSurface)
								{
									WorldGen.Place3x2(num9, num10, 26);
									if (Main.tile[num9, num10].type == 26)
									{
										flag4 = true;
									}
									else
									{
										num8++;
										if (num8 >= 10000)
										{
											flag4 = true;
										}
									}
								}
								else
								{
									flag4 = true;
								}
							}
						}
					}
				}
				value += value2;
				value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.01f;
				if ((double)value2.X > 0.3)
				{
					value2.X = 0.3f;
				}
				if ((double)value2.X < -0.3)
				{
					value2.X = -0.3f;
				}
				num4 = (int)((double)value.X - num3 * 1.1);
				num5 = (int)((double)value.X + num3 * 1.1);
				num6 = (int)((double)value.Y - num3 * 1.1);
				num7 = (int)((double)value.Y + num3 * 1.1);
				if (num4 < 1)
				{
					num4 = 1;
				}
				if (num5 > Main.maxTilesX - 1)
				{
					num5 = Main.maxTilesX - 1;
				}
				if (num6 < 0)
				{
					num6 = 0;
				}
				if (num7 > Main.maxTilesY)
				{
					num7 = Main.maxTilesY;
				}
				for (int m = num4; m < num5; m++)
				{
					for (int n = num6; n < num7; n++)
					{
						if ((double)(Math.Abs((float)m - value.X) + Math.Abs((float)n - value.Y)) < num3 * 1.1 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015))
						{
							if (Main.tile[m, n].type != 25 && n > j + WorldGen.genRand.Next(3, 20))
							{
								Main.tile[m, n].active = true;
							}
							if (steps <= num2)
							{
								Main.tile[m, n].active = true;
							}
							if (Main.tile[m, n].type != 31)
							{
								Main.tile[m, n].type = 25;
							}
							if (Main.tile[m, n].wall == 2)
							{
								Main.tile[m, n].wall = 0;
							}
						}
					}
				}
				for (int num11 = num4; num11 < num5; num11++)
				{
					for (int num12 = num6; num12 < num7; num12++)
					{
						if ((double)(Math.Abs((float)num11 - value.X) + Math.Abs((float)num12 - value.Y)) < num3 * 1.1 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015))
						{
							if (Main.tile[num11, num12].type != 31)
							{
								Main.tile[num11, num12].type = 25;
							}
							if (steps <= num2)
							{
								Main.tile[num11, num12].active = true;
							}
							if (num12 > j + WorldGen.genRand.Next(3, 20))
							{
								WorldGen.PlaceWall(num11, num12, 3, true);
							}
						}
					}
				}
			}
		}
		public static void JungleRunner(int i, int j)
		{
			double num = (double)WorldGen.genRand.Next(5, 11);
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j;
			Vector2 value2;
			value2.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
			value2.Y = (float)WorldGen.genRand.Next(10, 20) * 0.1f;
			int num2 = 0;
			bool flag = true;
			while (flag)
			{
				if ((double)value.Y < Main.worldSurface)
				{
					int num3 = (int)value.X;
					int num4 = (int)value.Y;
					if (Main.tile[num3, num4].wall == 0 && !Main.tile[num3, num4].active && Main.tile[num3, num4 - 3].wall == 0 && !Main.tile[num3, num4 - 3].active && Main.tile[num3, num4 - 1].wall == 0 && !Main.tile[num3, num4 - 1].active && Main.tile[num3, num4 - 4].wall == 0 && !Main.tile[num3, num4 - 4].active && Main.tile[num3, num4 - 2].wall == 0 && !Main.tile[num3, num4 - 2].active && Main.tile[num3, num4 - 5].wall == 0 && !Main.tile[num3, num4 - 5].active)
					{
						flag = false;
					}
				}
				WorldGen.JungleX = (int)value.X;
				num += (double)((float)WorldGen.genRand.Next(-20, 21) * 0.1f);
				if (num < 5.0)
				{
					num = 5.0;
				}
				if (num > 10.0)
				{
					num = 10.0;
				}
				int num5 = (int)((double)value.X - num * 0.5);
				int num6 = (int)((double)value.X + num * 0.5);
				int num7 = (int)((double)value.Y - num * 0.5);
				int num8 = (int)((double)value.Y + num * 0.5);
				if (num5 < 0)
				{
					num5 = 0;
				}
				if (num6 > Main.maxTilesX)
				{
					num6 = Main.maxTilesX;
				}
				if (num7 < 0)
				{
					num7 = 0;
				}
				if (num8 > Main.maxTilesY)
				{
					num8 = Main.maxTilesY;
				}
				for (int k = num5; k < num6; k++)
				{
					for (int l = num7; l < num8; l++)
					{
						if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < num * 0.5 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015))
						{
							WorldGen.KillTile(k, l, false, false, false);
						}
					}
				}
				num2++;
				if (num2 > 10 && WorldGen.genRand.Next(50) < num2)
				{
					num2 = 0;
					int num9 = -2;
					if (WorldGen.genRand.Next(2) == 0)
					{
						num9 = 2;
					}
					WorldGen.TileRunner((int)value.X, (int)value.Y, (double)WorldGen.genRand.Next(3, 20), WorldGen.genRand.Next(10, 100), -1, false, (float)num9, 0f, false, true);
				}
				value += value2;
				value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.01f;
				if (value2.Y > 0f)
				{
					value2.Y = 0f;
				}
				if (value2.Y < -2f)
				{
					value2.Y = -2f;
				}
				value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
				if (value.X < (float)(i - 200))
				{
					value2.X += (float)WorldGen.genRand.Next(5, 21) * 0.1f;
				}
				if (value.X > (float)(i + 200))
				{
					value2.X -= (float)WorldGen.genRand.Next(5, 21) * 0.1f;
				}
				if ((double)value2.X > 1.5)
				{
					value2.X = 1.5f;
				}
				if ((double)value2.X < -1.5)
				{
					value2.X = -1.5f;
				}
			}
		}
		public static void GERunner(int i, int j, float speedX = 0f, float speedY = 0f, bool good = true)
		{
			int num = WorldGen.genRand.Next(200, 250);
			float num2 = (float)(Main.maxTilesX / 4200);
			num = (int)((float)num * num2);
			double num3 = (double)num;
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j;
			Vector2 value2;
			value2.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
			value2.Y = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
			if (speedX != 0f || speedY != 0f)
			{
				value2.X = speedX;
				value2.Y = speedY;
			}
			bool flag = true;
			while (flag)
			{
				int num4 = (int)((double)value.X - num3 * 0.5);
				int num5 = (int)((double)value.X + num3 * 0.5);
				int num6 = (int)((double)value.Y - num3 * 0.5);
				int num7 = (int)((double)value.Y + num3 * 0.5);
				if (num4 < 0)
				{
					num4 = 0;
				}
				if (num5 > Main.maxTilesX)
				{
					num5 = Main.maxTilesX;
				}
				if (num6 < 0)
				{
					num6 = 0;
				}
				if (num7 > Main.maxTilesY)
				{
					num7 = Main.maxTilesY;
				}
				for (int k = num4; k < num5; k++)
				{
					for (int l = num6; l < num7; l++)
					{
						if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < (double)num * 0.5 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015))
						{
							if (good)
							{
								if (Main.tile[k, l].wall == 3)
								{
									Main.tile[k, l].wall = 28;
								}
								if (Main.tile[k, l].type == 2)
								{
									Main.tile[k, l].type = 109;
									WorldGen.SquareTileFrame(k, l, true);
								}
								else
								{
									if (Main.tile[k, l].type == 1)
									{
										Main.tile[k, l].type = 117;
										WorldGen.SquareTileFrame(k, l, true);
									}
									else
									{
										if (Main.tile[k, l].type == 53 || Main.tile[k, l].type == 123)
										{
											Main.tile[k, l].type = 116;
											WorldGen.SquareTileFrame(k, l, true);
										}
										else
										{
											if (Main.tile[k, l].type == 23)
											{
												Main.tile[k, l].type = 109;
												WorldGen.SquareTileFrame(k, l, true);
											}
											else
											{
												if (Main.tile[k, l].type == 25)
												{
													Main.tile[k, l].type = 117;
													WorldGen.SquareTileFrame(k, l, true);
												}
												else
												{
													if (Main.tile[k, l].type == 112)
													{
														Main.tile[k, l].type = 116;
														WorldGen.SquareTileFrame(k, l, true);
													}
												}
											}
										}
									}
								}
							}
							else
							{
								if (Main.tile[k, l].type == 2)
								{
									Main.tile[k, l].type = 23;
									WorldGen.SquareTileFrame(k, l, true);
								}
								else
								{
									if (Main.tile[k, l].type == 1)
									{
										Main.tile[k, l].type = 25;
										WorldGen.SquareTileFrame(k, l, true);
									}
									else
									{
										if (Main.tile[k, l].type == 53 || Main.tile[k, l].type == 123)
										{
											Main.tile[k, l].type = 112;
											WorldGen.SquareTileFrame(k, l, true);
										}
										else
										{
											if (Main.tile[k, l].type == 109)
											{
												Main.tile[k, l].type = 23;
												WorldGen.SquareTileFrame(k, l, true);
											}
											else
											{
												if (Main.tile[k, l].type == 117)
												{
													Main.tile[k, l].type = 25;
													WorldGen.SquareTileFrame(k, l, true);
												}
												else
												{
													if (Main.tile[k, l].type == 116)
													{
														Main.tile[k, l].type = 112;
														WorldGen.SquareTileFrame(k, l, true);
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
				value += value2;
				value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
				if (value2.X > speedX + 1f)
				{
					value2.X = speedX + 1f;
				}
				if (value2.X < speedX - 1f)
				{
					value2.X = speedX - 1f;
				}
				if (value.X < (float)(-(float)num) || value.Y < (float)(-(float)num) || value.X > (float)(Main.maxTilesX + num) || value.Y > (float)(Main.maxTilesX + num))
				{
					flag = false;
				}
			}
		}
		public static void TileRunner(int i, int j, double strength, int steps, int type, bool addTile = false, float speedX = 0f, float speedY = 0f, bool noYChange = false, bool overRide = true)
		{
			double num = strength;
			float num2 = (float)steps;
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j;
			Vector2 value2;
			value2.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
			value2.Y = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
			if (speedX != 0f || speedY != 0f)
			{
				value2.X = speedX;
				value2.Y = speedY;
			}
			while (num > 0.0 && num2 > 0f)
			{
				if (value.Y < 0f && num2 > 0f && type == 59)
				{
					num2 = 0f;
				}
				num = strength * (double)(num2 / (float)steps);
				num2 -= 1f;
				int num3 = (int)((double)value.X - num * 0.5);
				int num4 = (int)((double)value.X + num * 0.5);
				int num5 = (int)((double)value.Y - num * 0.5);
				int num6 = (int)((double)value.Y + num * 0.5);
				if (num3 < 0)
				{
					num3 = 0;
				}
				if (num4 > Main.maxTilesX)
				{
					num4 = Main.maxTilesX;
				}
				if (num5 < 0)
				{
					num5 = 0;
				}
				if (num6 > Main.maxTilesY)
				{
					num6 = Main.maxTilesY;
				}
				for (int k = num3; k < num4; k++)
				{
					for (int l = num5; l < num6; l++)
					{
						if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < strength * 0.5 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015))
						{
							if (WorldGen.mudWall && (double)l > Main.worldSurface && l < Main.maxTilesY - 210 - WorldGen.genRand.Next(3))
							{
								WorldGen.PlaceWall(k, l, 15, true);
							}
							if (type < 0)
							{
								if (type == -2 && Main.tile[k, l].active && (l < WorldGen.waterLine || l > WorldGen.lavaLine))
								{
									Main.tile[k, l].liquid = 255;
									if (l > WorldGen.lavaLine)
									{
										Main.tile[k, l].lava = true;
									}
								}
								Main.tile[k, l].active = false;
							}
							else
							{
								if ((overRide || !Main.tile[k, l].active) && (type != 40 || Main.tile[k, l].type != 53) && (!Main.tileStone[type] || Main.tile[k, l].type == 1) && Main.tile[k, l].type != 45 && Main.tile[k, l].type != 147 && (Main.tile[k, l].type != 1 || type != 59 || (double)l >= Main.worldSurface + (double)WorldGen.genRand.Next(-50, 50)))
								{
									if (Main.tile[k, l].type != 53 || (double)l >= Main.worldSurface)
									{
										Main.tile[k, l].type = (byte)type;
									}
									else
									{
										if (type == 59)
										{
											Main.tile[k, l].type = (byte)type;
										}
									}
								}
								if (addTile)
								{
									Main.tile[k, l].active = true;
									Main.tile[k, l].liquid = 0;
									Main.tile[k, l].lava = false;
								}
								if (noYChange && (double)l < Main.worldSurface && type != 59)
								{
									Main.tile[k, l].wall = 2;
								}
								if (type == 59 && l > WorldGen.waterLine && Main.tile[k, l].liquid > 0)
								{
									Main.tile[k, l].lava = false;
									Main.tile[k, l].liquid = 0;
								}
							}
						}
					}
				}
				value += value2;
				if (num > 50.0)
				{
					value += value2;
					num2 -= 1f;
					value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
					value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
					if (num > 100.0)
					{
						value += value2;
						num2 -= 1f;
						value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
						value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
						if (num > 150.0)
						{
							value += value2;
							num2 -= 1f;
							value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
							value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
							if (num > 200.0)
							{
								value += value2;
								num2 -= 1f;
								value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
								value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
								if (num > 250.0)
								{
									value += value2;
									num2 -= 1f;
									value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
									value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
									if (num > 300.0)
									{
										value += value2;
										num2 -= 1f;
										value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
										value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
										if (num > 400.0)
										{
											value += value2;
											num2 -= 1f;
											value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
											value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
											if (num > 500.0)
											{
												value += value2;
												num2 -= 1f;
												value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
												value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
												if (num > 600.0)
												{
													value += value2;
													num2 -= 1f;
													value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
													value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
													if (num > 700.0)
													{
														value += value2;
														num2 -= 1f;
														value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
														value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
														if (num > 800.0)
														{
															value += value2;
															num2 -= 1f;
															value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
															value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
															if (num > 900.0)
															{
																value += value2;
																num2 -= 1f;
																value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
																value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
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
				value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
				if (value2.X > 1f)
				{
					value2.X = 1f;
				}
				if (value2.X < -1f)
				{
					value2.X = -1f;
				}
				if (!noYChange)
				{
					value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
					if (value2.Y > 1f)
					{
						value2.Y = 1f;
					}
					if (value2.Y < -1f)
					{
						value2.Y = -1f;
					}
				}
				else
				{
					if (type != 59 && num < 3.0)
					{
						if (value2.Y > 1f)
						{
							value2.Y = 1f;
						}
						if (value2.Y < -1f)
						{
							value2.Y = -1f;
						}
					}
				}
				if (type == 59 && !noYChange)
				{
					if ((double)value2.Y > 0.5)
					{
						value2.Y = 0.5f;
					}
					if ((double)value2.Y < -0.5)
					{
						value2.Y = -0.5f;
					}
					if ((double)value.Y < Main.rockLayer + 100.0)
					{
						value2.Y = 1f;
					}
					if (value.Y > (float)(Main.maxTilesY - 300))
					{
						value2.Y = -1f;
					}
				}
			}
		}
		public static void MudWallRunner(int i, int j)
		{
			double num = (double)WorldGen.genRand.Next(5, 15);
			float num2 = (float)WorldGen.genRand.Next(5, 20);
			float num3 = num2;
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j;
			Vector2 value2;
			value2.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
			value2.Y = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
			while (num > 0.0 && num3 > 0f)
			{
				double num4 = num * (double)(num3 / num2);
				num3 -= 1f;
				int num5 = (int)((double)value.X - num4 * 0.5);
				int num6 = (int)((double)value.X + num4 * 0.5);
				int num7 = (int)((double)value.Y - num4 * 0.5);
				int num8 = (int)((double)value.Y + num4 * 0.5);
				if (num5 < 0)
				{
					num5 = 0;
				}
				if (num6 > Main.maxTilesX)
				{
					num6 = Main.maxTilesX;
				}
				if (num7 < 0)
				{
					num7 = 0;
				}
				if (num8 > Main.maxTilesY)
				{
					num8 = Main.maxTilesY;
				}
				for (int k = num5; k < num6; k++)
				{
					for (int l = num7; l < num8; l++)
					{
						if ((double)(Math.Abs((float)k - value.X) + Math.Abs((float)l - value.Y)) < num * 0.5 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.015))
						{
							Main.tile[k, l].wall = 0;
						}
					}
				}
				value += value2;
				value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
				if (value2.X > 1f)
				{
					value2.X = 1f;
				}
				if (value2.X < -1f)
				{
					value2.X = -1f;
				}
				value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
				if (value2.Y > 1f)
				{
					value2.Y = 1f;
				}
				if (value2.Y < -1f)
				{
					value2.Y = -1f;
				}
			}
		}
		public static void FloatingIsland(int i, int j)
		{
			double num = (double)WorldGen.genRand.Next(80, 120);
			float num2 = (float)WorldGen.genRand.Next(20, 25);
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j;
			Vector2 value2;
			value2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
			while (value2.X > -2f && value2.X < 2f)
			{
				value2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
			}
			value2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.02f;
			while (num > 0.0 && num2 > 0f)
			{
				num -= (double)WorldGen.genRand.Next(4);
				num2 -= 1f;
				int num3 = (int)((double)value.X - num * 0.5);
				int num4 = (int)((double)value.X + num * 0.5);
				int num5 = (int)((double)value.Y - num * 0.5);
				int num6 = (int)((double)value.Y + num * 0.5);
				if (num3 < 0)
				{
					num3 = 0;
				}
				if (num4 > Main.maxTilesX)
				{
					num4 = Main.maxTilesX;
				}
				if (num5 < 0)
				{
					num5 = 0;
				}
				if (num6 > Main.maxTilesY)
				{
					num6 = Main.maxTilesY;
				}
				double num7 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01;
				float num8 = value.Y + 1f;
				for (int k = num3; k < num4; k++)
				{
					if (WorldGen.genRand.Next(2) == 0)
					{
						num8 += (float)WorldGen.genRand.Next(-1, 2);
					}
					if (num8 < value.Y)
					{
						num8 = value.Y;
					}
					if (num8 > value.Y + 2f)
					{
						num8 = value.Y + 2f;
					}
					for (int l = num5; l < num6; l++)
					{
						if ((float)l > num8)
						{
							float num9 = Math.Abs((float)k - value.X);
							float num10 = Math.Abs((float)l - value.Y) * 2f;
							double num11 = Math.Sqrt((double)(num9 * num9 + num10 * num10));
							if (num11 < num7 * 0.4)
							{
								Main.tile[k, l].active = true;
								if (Main.tile[k, l].type == 59)
								{
									Main.tile[k, l].type = 0;
								}
							}
						}
					}
				}
				WorldGen.TileRunner(WorldGen.genRand.Next(num3 + 10, num4 - 10), (int)((double)value.Y + num7 * 0.1 + 5.0), (double)WorldGen.genRand.Next(5, 10), WorldGen.genRand.Next(10, 15), 0, true, 0f, 2f, true, true);
				num3 = (int)((double)value.X - num * 0.4);
				num4 = (int)((double)value.X + num * 0.4);
				num5 = (int)((double)value.Y - num * 0.4);
				num6 = (int)((double)value.Y + num * 0.4);
				if (num3 < 0)
				{
					num3 = 0;
				}
				if (num4 > Main.maxTilesX)
				{
					num4 = Main.maxTilesX;
				}
				if (num5 < 0)
				{
					num5 = 0;
				}
				if (num6 > Main.maxTilesY)
				{
					num6 = Main.maxTilesY;
				}
				num7 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01;
				for (int m = num3; m < num4; m++)
				{
					for (int n = num5; n < num6; n++)
					{
						if ((float)n > value.Y + 2f)
						{
							float num12 = Math.Abs((float)m - value.X);
							float num13 = Math.Abs((float)n - value.Y) * 2f;
							double num14 = Math.Sqrt((double)(num12 * num12 + num13 * num13));
							if (num14 < num7 * 0.4)
							{
								Main.tile[m, n].wall = 2;
							}
						}
					}
				}
				value += value2;
				value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
				if (value2.X > 1f)
				{
					value2.X = 1f;
				}
				if (value2.X < -1f)
				{
					value2.X = -1f;
				}
				if ((double)value2.Y > 0.2)
				{
					value2.Y = -0.2f;
				}
				if ((double)value2.Y < -0.2)
				{
					value2.Y = -0.2f;
				}
			}
		}
		public static void Caverer(int X, int Y)
		{
			int num = WorldGen.genRand.Next(2);
			if (num == 0)
			{
				int num2 = WorldGen.genRand.Next(7, 9);
				float num3 = (float)WorldGen.genRand.Next(100) * 0.01f;
				float num4 = 1f - num3;
				if (WorldGen.genRand.Next(2) == 0)
				{
					num3 = -num3;
				}
				if (WorldGen.genRand.Next(2) == 0)
				{
					num4 = -num4;
				}
				Vector2 vector = new Vector2((float)X, (float)Y);
				for (int i = 0; i < num2; i++)
				{
					vector = WorldGen.digTunnel(vector.X, vector.Y, num3, num4, WorldGen.genRand.Next(6, 20), WorldGen.genRand.Next(4, 9), false);
					num3 += (float)WorldGen.genRand.Next(-20, 21) * 0.1f;
					num4 += (float)WorldGen.genRand.Next(-20, 21) * 0.1f;
					if ((double)num3 < -1.5)
					{
						num3 = -1.5f;
					}
					if ((double)num3 > 1.5)
					{
						num3 = 1.5f;
					}
					if ((double)num4 < -1.5)
					{
						num4 = -1.5f;
					}
					if ((double)num4 > 1.5)
					{
						num4 = 1.5f;
					}
					float num5 = (float)WorldGen.genRand.Next(100) * 0.01f;
					float num6 = 1f - num5;
					if (WorldGen.genRand.Next(2) == 0)
					{
						num5 = -num5;
					}
					if (WorldGen.genRand.Next(2) == 0)
					{
						num6 = -num6;
					}
					Vector2 vector2 = WorldGen.digTunnel(vector.X, vector.Y, num5, num6, WorldGen.genRand.Next(30, 50), WorldGen.genRand.Next(3, 6), false);
					WorldGen.TileRunner((int)vector2.X, (int)vector2.Y, (double)WorldGen.genRand.Next(10, 20), WorldGen.genRand.Next(5, 10), -1, false, 0f, 0f, false, true);
				}
				return;
			}
			if (num == 1)
			{
				int num7 = WorldGen.genRand.Next(15, 30);
				float num8 = (float)WorldGen.genRand.Next(100) * 0.01f;
				float num9 = 1f - num8;
				if (WorldGen.genRand.Next(2) == 0)
				{
					num8 = -num8;
				}
				if (WorldGen.genRand.Next(2) == 0)
				{
					num9 = -num9;
				}
				Vector2 vector3 = new Vector2((float)X, (float)Y);
				for (int j = 0; j < num7; j++)
				{
					vector3 = WorldGen.digTunnel(vector3.X, vector3.Y, num8, num9, WorldGen.genRand.Next(5, 15), WorldGen.genRand.Next(2, 6), true);
					num8 += (float)WorldGen.genRand.Next(-20, 21) * 0.1f;
					num9 += (float)WorldGen.genRand.Next(-20, 21) * 0.1f;
					if ((double)num8 < -1.5)
					{
						num8 = -1.5f;
					}
					if ((double)num8 > 1.5)
					{
						num8 = 1.5f;
					}
					if ((double)num9 < -1.5)
					{
						num9 = -1.5f;
					}
					if ((double)num9 > 1.5)
					{
						num9 = 1.5f;
					}
				}
			}
		}
		public static Vector2 digTunnel(float X, float Y, float xDir, float yDir, int Steps, int Size, bool Wet = false)
		{
			float num = X;
			float num2 = Y;
			try
			{
				float num3 = 0f;
				float num4 = 0f;
				float num5 = (float)Size;
				for (int i = 0; i < Steps; i++)
				{
					int num6 = (int)(num - num5);
					while ((float)num6 <= num + num5)
					{
						int num7 = (int)(num2 - num5);
						while ((float)num7 <= num2 + num5)
						{
							if ((double)(Math.Abs((float)num6 - num) + Math.Abs((float)num7 - num2)) < (double)num5 * (1.0 + (double)WorldGen.genRand.Next(-10, 11) * 0.005))
							{
								Main.tile[num6, num7].active = false;
								if (Wet)
								{
									Main.tile[num6, num7].liquid = 255;
								}
							}
							num7++;
						}
						num6++;
					}
					num5 += (float)WorldGen.genRand.Next(-50, 51) * 0.03f;
					if ((double)num5 < (double)Size * 0.6)
					{
						num5 = (float)Size * 0.6f;
					}
					if (num5 > (float)(Size * 2))
					{
						num5 = (float)Size * 2f;
					}
					num3 += (float)WorldGen.genRand.Next(-20, 21) * 0.01f;
					num4 += (float)WorldGen.genRand.Next(-20, 21) * 0.01f;
					if (num3 < -1f)
					{
						num3 = -1f;
					}
					if (num3 > 1f)
					{
						num3 = 1f;
					}
					if (num4 < -1f)
					{
						num4 = -1f;
					}
					if (num4 > 1f)
					{
						num4 = 1f;
					}
					num += (xDir + num3) * 0.6f;
					num2 += (yDir + num4) * 0.6f;
				}
			}
			catch
			{
			}
			return new Vector2(num, num2);
		}
		public static void IslandHouse(int i, int j)
		{
			byte type = (byte)WorldGen.genRand.Next(45, 48);
			byte wall = (byte)WorldGen.genRand.Next(10, 13);
			Vector2 vector = new Vector2((float)i, (float)j);
			int num = 1;
			if (WorldGen.genRand.Next(2) == 0)
			{
				num = -1;
			}
			int num2 = WorldGen.genRand.Next(7, 12);
			int num3 = WorldGen.genRand.Next(5, 7);
			vector.X = (float)(i + (num2 + 2) * num);
			for (int k = j - 15; k < j + 30; k++)
			{
				if (Main.tile[(int)vector.X, k].active)
				{
					vector.Y = (float)(k - 1);
					break;
				}
			}
			vector.X = (float)i;
			int num4 = (int)(vector.X - (float)num2 - 2f);
			int num5 = (int)(vector.X + (float)num2 + 2f);
			int num6 = (int)(vector.Y - (float)num3 - 2f);
			int num7 = (int)(vector.Y + 2f + (float)WorldGen.genRand.Next(3, 5));
			if (num4 < 0)
			{
				num4 = 0;
			}
			if (num5 > Main.maxTilesX)
			{
				num5 = Main.maxTilesX;
			}
			if (num6 < 0)
			{
				num6 = 0;
			}
			if (num7 > Main.maxTilesY)
			{
				num7 = Main.maxTilesY;
			}
			for (int l = num4; l <= num5; l++)
			{
				for (int m = num6; m < num7; m++)
				{
					Main.tile[l, m].active = true;
					Main.tile[l, m].type = type;
					Main.tile[l, m].wall = 0;
				}
			}
			num4 = (int)(vector.X - (float)num2);
			num5 = (int)(vector.X + (float)num2);
			num6 = (int)(vector.Y - (float)num3);
			num7 = (int)(vector.Y + 1f);
			if (num4 < 0)
			{
				num4 = 0;
			}
			if (num5 > Main.maxTilesX)
			{
				num5 = Main.maxTilesX;
			}
			if (num6 < 0)
			{
				num6 = 0;
			}
			if (num7 > Main.maxTilesY)
			{
				num7 = Main.maxTilesY;
			}
			for (int n = num4; n <= num5; n++)
			{
				for (int num8 = num6; num8 < num7; num8++)
				{
					if (Main.tile[n, num8].wall == 0)
					{
						Main.tile[n, num8].active = false;
						Main.tile[n, num8].wall = wall;
					}
				}
			}
			int num9 = i + (num2 + 1) * num;
			int num10 = (int)vector.Y;
			for (int num11 = num9 - 2; num11 <= num9 + 2; num11++)
			{
				Main.tile[num11, num10].active = false;
				Main.tile[num11, num10 - 1].active = false;
				Main.tile[num11, num10 - 2].active = false;
			}
			WorldGen.PlaceTile(num9, num10, 10, true, false, -1, 0);
			int contain = 0;
			int num12 = WorldGen.houseCount;
			if (num12 > 2)
			{
				num12 = WorldGen.genRand.Next(3);
			}
			if (num12 == 0)
			{
				contain = 159;
			}
			else
			{
				if (num12 == 1)
				{
					contain = 65;
				}
				else
				{
					if (num12 == 2)
					{
						contain = 158;
					}
				}
			}
			WorldGen.AddBuriedChest(i, num10 - 3, contain, false, 2);
			WorldGen.houseCount++;
		}
		public static void Mountinater(int i, int j)
		{
			double num = (double)WorldGen.genRand.Next(80, 120);
			float num2 = (float)WorldGen.genRand.Next(40, 55);
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j + num2 / 2f;
			Vector2 value2;
			value2.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
			value2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.1f;
			while (num > 0.0 && num2 > 0f)
			{
				num -= (double)WorldGen.genRand.Next(4);
				num2 -= 1f;
				int num3 = (int)((double)value.X - num * 0.5);
				int num4 = (int)((double)value.X + num * 0.5);
				int num5 = (int)((double)value.Y - num * 0.5);
				int num6 = (int)((double)value.Y + num * 0.5);
				if (num3 < 0)
				{
					num3 = 0;
				}
				if (num4 > Main.maxTilesX)
				{
					num4 = Main.maxTilesX;
				}
				if (num5 < 0)
				{
					num5 = 0;
				}
				if (num6 > Main.maxTilesY)
				{
					num6 = Main.maxTilesY;
				}
				double num7 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01;
				for (int k = num3; k < num4; k++)
				{
					for (int l = num5; l < num6; l++)
					{
						float num8 = Math.Abs((float)k - value.X);
						float num9 = Math.Abs((float)l - value.Y);
						double num10 = Math.Sqrt((double)(num8 * num8 + num9 * num9));
						if (num10 < num7 * 0.4 && !Main.tile[k, l].active)
						{
							Main.tile[k, l].active = true;
							Main.tile[k, l].type = 0;
						}
					}
				}
				value += value2;
				value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
				value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
				if ((double)value2.X > 0.5)
				{
					value2.X = 0.5f;
				}
				if ((double)value2.X < -0.5)
				{
					value2.X = -0.5f;
				}
				if ((double)value2.Y > -0.5)
				{
					value2.Y = -0.5f;
				}
				if ((double)value2.Y < -1.5)
				{
					value2.Y = -1.5f;
				}
			}
		}
		public static void Lakinater(int i, int j)
		{
			double num = (double)WorldGen.genRand.Next(25, 50);
			double num2 = num;
			float num3 = (float)WorldGen.genRand.Next(30, 80);
			if (WorldGen.genRand.Next(5) == 0)
			{
				num *= 1.5;
				num2 *= 1.5;
				num3 *= 1.2f;
			}
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j - num3 * 0.3f;
			Vector2 value2;
			value2.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
			value2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.1f;
			while (num > 0.0 && num3 > 0f)
			{
				if ((double)value.Y + num2 * 0.5 > Main.worldSurface)
				{
					num3 = 0f;
				}
				num -= (double)WorldGen.genRand.Next(3);
				num3 -= 1f;
				int num4 = (int)((double)value.X - num * 0.5);
				int num5 = (int)((double)value.X + num * 0.5);
				int num6 = (int)((double)value.Y - num * 0.5);
				int num7 = (int)((double)value.Y + num * 0.5);
				if (num4 < 0)
				{
					num4 = 0;
				}
				if (num5 > Main.maxTilesX)
				{
					num5 = Main.maxTilesX;
				}
				if (num6 < 0)
				{
					num6 = 0;
				}
				if (num7 > Main.maxTilesY)
				{
					num7 = Main.maxTilesY;
				}
				num2 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01;
				for (int k = num4; k < num5; k++)
				{
					for (int l = num6; l < num7; l++)
					{
						float num8 = Math.Abs((float)k - value.X);
						float num9 = Math.Abs((float)l - value.Y);
						double num10 = Math.Sqrt((double)(num8 * num8 + num9 * num9));
						if (num10 < num2 * 0.4)
						{
							if (Main.tile[k, l].active)
							{
								Main.tile[k, l].liquid = 255;
							}
							Main.tile[k, l].active = false;
						}
					}
				}
				value += value2;
				value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
				value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
				if ((double)value2.X > 0.5)
				{
					value2.X = 0.5f;
				}
				if ((double)value2.X < -0.5)
				{
					value2.X = -0.5f;
				}
				if ((double)value2.Y > 1.5)
				{
					value2.Y = 1.5f;
				}
				if ((double)value2.Y < 0.5)
				{
					value2.Y = 0.5f;
				}
			}
		}
		public static void ShroomPatch(int i, int j)
		{
			double num = (double)WorldGen.genRand.Next(40, 70);
			double num2 = num;
			float num3 = (float)WorldGen.genRand.Next(20, 30);
			if (WorldGen.genRand.Next(5) == 0)
			{
				num *= 1.5;
				num2 *= 1.5;
				num3 *= 1.2f;
			}
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j - num3 * 0.3f;
			Vector2 value2;
			value2.X = (float)WorldGen.genRand.Next(-10, 11) * 0.1f;
			value2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.1f;
			while (num > 0.0 && num3 > 0f)
			{
				num -= (double)WorldGen.genRand.Next(3);
				num3 -= 1f;
				int num4 = (int)((double)value.X - num * 0.5);
				int num5 = (int)((double)value.X + num * 0.5);
				int num6 = (int)((double)value.Y - num * 0.5);
				int num7 = (int)((double)value.Y + num * 0.5);
				if (num4 < 0)
				{
					num4 = 0;
				}
				if (num5 > Main.maxTilesX)
				{
					num5 = Main.maxTilesX;
				}
				if (num6 < 0)
				{
					num6 = 0;
				}
				if (num7 > Main.maxTilesY)
				{
					num7 = Main.maxTilesY;
				}
				num2 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01;
				for (int k = num4; k < num5; k++)
				{
					for (int l = num6; l < num7; l++)
					{
						float num8 = Math.Abs((float)k - value.X);
						float num9 = Math.Abs(((float)l - value.Y) * 2.3f);
						double num10 = Math.Sqrt((double)(num8 * num8 + num9 * num9));
						if (num10 < num2 * 0.4)
						{
							if ((double)l < (double)value.Y + num2 * 0.02)
							{
								if (Main.tile[k, l].type != 59)
								{
									Main.tile[k, l].active = false;
								}
							}
							else
							{
								Main.tile[k, l].type = 59;
							}
							Main.tile[k, l].liquid = 0;
							Main.tile[k, l].lava = false;
						}
					}
				}
				value += value2;
				value.X += value2.X;
				value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
				value2.Y -= (float)WorldGen.genRand.Next(11) * 0.05f;
				if ((double)value2.X > -0.5 && (double)value2.X < 0.5)
				{
					if (value2.X < 0f)
					{
						value2.X = -0.5f;
					}
					else
					{
						value2.X = 0.5f;
					}
				}
				if (value2.X > 2f)
				{
					value2.X = 1f;
				}
				if (value2.X < -2f)
				{
					value2.X = -1f;
				}
				if (value2.Y > 1f)
				{
					value2.Y = 1f;
				}
				if (value2.Y < -1f)
				{
					value2.Y = -1f;
				}
				for (int m = 0; m < 2; m++)
				{
					int num11 = (int)value.X + WorldGen.genRand.Next(-20, 20);
					int num12 = (int)value.Y + WorldGen.genRand.Next(0, 20);
					while (!Main.tile[num11, num12].active && Main.tile[num11, num12].type != 59)
					{
						num11 = (int)value.X + WorldGen.genRand.Next(-20, 20);
						num12 = (int)value.Y + WorldGen.genRand.Next(0, 20);
					}
					int num13 = WorldGen.genRand.Next(7, 10);
					int num14 = WorldGen.genRand.Next(7, 10);
					WorldGen.TileRunner(num11, num12, (double)num13, num14, 59, false, 0f, 2f, true, true);
					if (WorldGen.genRand.Next(3) == 0)
					{
						WorldGen.TileRunner(num11, num12, (double)(num13 - 3), num14 - 3, -1, false, 0f, 2f, true, true);
					}
				}
			}
		}
		public static void Cavinator(int i, int j, int steps)
		{
			double num = (double)WorldGen.genRand.Next(7, 15);
			int num2 = 1;
			if (WorldGen.genRand.Next(2) == 0)
			{
				num2 = -1;
			}
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j;
			int k = WorldGen.genRand.Next(20, 40);
			Vector2 value2;
			value2.Y = (float)WorldGen.genRand.Next(10, 20) * 0.01f;
			value2.X = (float)num2;
			while (k > 0)
			{
				k--;
				int num3 = (int)((double)value.X - num * 0.5);
				int num4 = (int)((double)value.X + num * 0.5);
				int num5 = (int)((double)value.Y - num * 0.5);
				int num6 = (int)((double)value.Y + num * 0.5);
				if (num3 < 0)
				{
					num3 = 0;
				}
				if (num4 > Main.maxTilesX)
				{
					num4 = Main.maxTilesX;
				}
				if (num5 < 0)
				{
					num5 = 0;
				}
				if (num6 > Main.maxTilesY)
				{
					num6 = Main.maxTilesY;
				}
				double num7 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01;
				for (int l = num3; l < num4; l++)
				{
					for (int m = num5; m < num6; m++)
					{
						float num8 = Math.Abs((float)l - value.X);
						float num9 = Math.Abs((float)m - value.Y);
						double num10 = Math.Sqrt((double)(num8 * num8 + num9 * num9));
						if (num10 < num7 * 0.4)
						{
							Main.tile[l, m].active = false;
						}
					}
				}
				value += value2;
				value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
				value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
				if (value2.X > (float)num2 + 0.5f)
				{
					value2.X = (float)num2 + 0.5f;
				}
				if (value2.X < (float)num2 - 0.5f)
				{
					value2.X = (float)num2 - 0.5f;
				}
				if (value2.Y > 2f)
				{
					value2.Y = 2f;
				}
				if (value2.Y < 0f)
				{
					value2.Y = 0f;
				}
			}
			if (steps > 0 && (double)((int)value.Y) < Main.rockLayer + 50.0)
			{
				WorldGen.Cavinator((int)value.X, (int)value.Y, steps - 1);
			}
		}
		public static void CaveOpenater(int i, int j)
		{
			double num = (double)WorldGen.genRand.Next(7, 12);
			int num2 = 1;
			if (WorldGen.genRand.Next(2) == 0)
			{
				num2 = -1;
			}
			Vector2 value;
			value.X = (float)i;
			value.Y = (float)j;
			int k = 100;
			Vector2 value2;
			value2.Y = 0f;
			value2.X = (float)num2;
			while (k > 0)
			{
				if (Main.tile[(int)value.X, (int)value.Y].wall == 0)
				{
					k = 0;
				}
				k--;
				int num3 = (int)((double)value.X - num * 0.5);
				int num4 = (int)((double)value.X + num * 0.5);
				int num5 = (int)((double)value.Y - num * 0.5);
				int num6 = (int)((double)value.Y + num * 0.5);
				if (num3 < 0)
				{
					num3 = 0;
				}
				if (num4 > Main.maxTilesX)
				{
					num4 = Main.maxTilesX;
				}
				if (num5 < 0)
				{
					num5 = 0;
				}
				if (num6 > Main.maxTilesY)
				{
					num6 = Main.maxTilesY;
				}
				double num7 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01;
				for (int l = num3; l < num4; l++)
				{
					for (int m = num5; m < num6; m++)
					{
						float num8 = Math.Abs((float)l - value.X);
						float num9 = Math.Abs((float)m - value.Y);
						double num10 = Math.Sqrt((double)(num8 * num8 + num9 * num9));
						if (num10 < num7 * 0.4)
						{
							Main.tile[l, m].active = false;
						}
					}
				}
				value += value2;
				value2.X += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
				value2.Y += (float)WorldGen.genRand.Next(-10, 11) * 0.05f;
				if (value2.X > (float)num2 + 0.5f)
				{
					value2.X = (float)num2 + 0.5f;
				}
				if (value2.X < (float)num2 - 0.5f)
				{
					value2.X = (float)num2 - 0.5f;
				}
				if (value2.Y > 0f)
				{
					value2.Y = 0f;
				}
				if ((double)value2.Y < -0.5)
				{
					value2.Y = -0.5f;
				}
			}
		}
		public static void SquareTileFrame(int i, int j, bool resetFrame = true)
		{
			WorldGen.TileFrame(i - 1, j - 1, false, false);
			WorldGen.TileFrame(i - 1, j, false, false);
			WorldGen.TileFrame(i - 1, j + 1, false, false);
			WorldGen.TileFrame(i, j - 1, false, false);
			WorldGen.TileFrame(i, j, resetFrame, false);
			WorldGen.TileFrame(i, j + 1, false, false);
			WorldGen.TileFrame(i + 1, j - 1, false, false);
			WorldGen.TileFrame(i + 1, j, false, false);
			WorldGen.TileFrame(i + 1, j + 1, false, false);
		}
		public static void SquareWallFrame(int i, int j, bool resetFrame = true)
		{
			WorldGen.WallFrame(i - 1, j - 1, false);
			WorldGen.WallFrame(i - 1, j, false);
			WorldGen.WallFrame(i - 1, j + 1, false);
			WorldGen.WallFrame(i, j - 1, false);
			WorldGen.WallFrame(i, j, resetFrame);
			WorldGen.WallFrame(i, j + 1, false);
			WorldGen.WallFrame(i + 1, j - 1, false);
			WorldGen.WallFrame(i + 1, j, false);
			WorldGen.WallFrame(i + 1, j + 1, false);
		}
		public static void SectionTileFrame(int startX, int startY, int endX, int endY)
		{
			int num = startX * 200;
			int num2 = (endX + 1) * 200;
			int num3 = startY * 150;
			int num4 = (endY + 1) * 150;
			if (num < 1)
			{
				num = 1;
			}
			if (num3 < 1)
			{
				num3 = 1;
			}
			if (num > Main.maxTilesX - 2)
			{
				num = Main.maxTilesX - 2;
			}
			if (num3 > Main.maxTilesY - 2)
			{
				num3 = Main.maxTilesY - 2;
			}
			for (int i = num - 1; i < num2 + 1; i++)
			{
				for (int j = num3 - 1; j < num4 + 1; j++)
				{
					WorldGen.TileFrame(i, j, true, true);
					WorldGen.WallFrame(i, j, true);
				}
			}
		}
		public static void RangeFrame(int startX, int startY, int endX, int endY)
		{
			int num = endX + 1;
			int num2 = endY + 1;
			for (int i = startX - 1; i < num + 1; i++)
			{
				for (int j = startY - 1; j < num2 + 1; j++)
				{
					WorldGen.TileFrame(i, j, false, false);
					WorldGen.WallFrame(i, j, false);
				}
			}
		}
		public static void WaterCheck()
		{
			Liquid.numLiquid = 0;
			LiquidBuffer.numLiquidBuffer = 0;
			for (int i = 1; i < Main.maxTilesX - 1; i++)
			{
				for (int j = Main.maxTilesY - 2; j > 0; j--)
				{
					Main.tile[i, j].checkingLiquid = false;
					if (Main.tile[i, j].liquid > 0 && Main.tile[i, j].active && Main.tileSolid[(int)Main.tile[i, j].type] && !Main.tileSolidTop[(int)Main.tile[i, j].type])
					{
						Main.tile[i, j].liquid = 0;
					}
					else
					{
						if (Main.tile[i, j].liquid > 0)
						{
							if (Main.tile[i, j].active)
							{
								if (Main.tileWaterDeath[(int)Main.tile[i, j].type])
								{
									WorldGen.KillTile(i, j, false, false, false);
								}
								if (Main.tile[i, j].lava && Main.tileLavaDeath[(int)Main.tile[i, j].type])
								{
									WorldGen.KillTile(i, j, false, false, false);
								}
							}
							if ((!Main.tile[i, j + 1].active || !Main.tileSolid[(int)Main.tile[i, j + 1].type] || Main.tileSolidTop[(int)Main.tile[i, j + 1].type]) && Main.tile[i, j + 1].liquid < 255)
							{
								if (Main.tile[i, j + 1].liquid > 250)
								{
									Main.tile[i, j + 1].liquid = 255;
								}
								else
								{
									Liquid.AddWater(i, j);
								}
							}
							if ((!Main.tile[i - 1, j].active || !Main.tileSolid[(int)Main.tile[i - 1, j].type] || Main.tileSolidTop[(int)Main.tile[i - 1, j].type]) && Main.tile[i - 1, j].liquid != Main.tile[i, j].liquid)
							{
								Liquid.AddWater(i, j);
							}
							else
							{
								if ((!Main.tile[i + 1, j].active || !Main.tileSolid[(int)Main.tile[i + 1, j].type] || Main.tileSolidTop[(int)Main.tile[i + 1, j].type]) && Main.tile[i + 1, j].liquid != Main.tile[i, j].liquid)
								{
									Liquid.AddWater(i, j);
								}
							}
							if (Main.tile[i, j].lava)
							{
								if (Main.tile[i - 1, j].liquid > 0 && !Main.tile[i - 1, j].lava)
								{
									Liquid.AddWater(i, j);
								}
								else
								{
									if (Main.tile[i + 1, j].liquid > 0 && !Main.tile[i + 1, j].lava)
									{
										Liquid.AddWater(i, j);
									}
									else
									{
										if (Main.tile[i, j - 1].liquid > 0 && !Main.tile[i, j - 1].lava)
										{
											Liquid.AddWater(i, j);
										}
										else
										{
											if (Main.tile[i, j + 1].liquid > 0 && !Main.tile[i, j + 1].lava)
											{
												Liquid.AddWater(i, j);
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
		public static void EveryTileFrame()
		{
			WorldGen.noLiquidCheck = true;
			WorldGen.noTileActions = true;
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				float num = (float)i / (float)Main.maxTilesX;
				Main.statusText = "Finding tile frames: " + (int)(num * 100f + 1f) + "%";
				for (int j = 0; j < Main.maxTilesY; j++)
				{
					if (Main.tile[i, j].active)
					{
						WorldGen.TileFrame(i, j, true, false);
					}
					if (Main.tile[i, j].wall > 0)
					{
						WorldGen.WallFrame(i, j, true);
					}
				}
			}
			WorldGen.noLiquidCheck = false;
			WorldGen.noTileActions = false;
		}
		public static void PlantCheck(int i, int j)
		{
			int num = -1;
			int num2 = (int)Main.tile[i, j].type;
			int arg_19_0 = i - 1;
			int arg_23_0 = i + 1;
			int arg_22_0 = Main.maxTilesX;
			int arg_29_0 = j - 1;
			if (j + 1 >= Main.maxTilesY)
			{
				num = num2;
			}
			if (i - 1 >= 0 && Main.tile[i - 1, j] != null && Main.tile[i - 1, j].active)
			{
				byte arg_74_0 = Main.tile[i - 1, j].type;
			}
			if (i + 1 < Main.maxTilesX && Main.tile[i + 1, j] != null && Main.tile[i + 1, j].active)
			{
				byte arg_B7_0 = Main.tile[i + 1, j].type;
			}
			if (j - 1 >= 0 && Main.tile[i, j - 1] != null && Main.tile[i, j - 1].active)
			{
				byte arg_F6_0 = Main.tile[i, j - 1].type;
			}
			if (j + 1 < Main.maxTilesY && Main.tile[i, j + 1] != null && Main.tile[i, j + 1].active)
			{
				num = (int)Main.tile[i, j + 1].type;
			}
			if (i - 1 >= 0 && j - 1 >= 0 && Main.tile[i - 1, j - 1] != null && Main.tile[i - 1, j - 1].active)
			{
				byte arg_184_0 = Main.tile[i - 1, j - 1].type;
			}
			if (i + 1 < Main.maxTilesX && j - 1 >= 0 && Main.tile[i + 1, j - 1] != null && Main.tile[i + 1, j - 1].active)
			{
				byte arg_1D3_0 = Main.tile[i + 1, j - 1].type;
			}
			if (i - 1 >= 0 && j + 1 < Main.maxTilesY && Main.tile[i - 1, j + 1] != null && Main.tile[i - 1, j + 1].active)
			{
				byte arg_222_0 = Main.tile[i - 1, j + 1].type;
			}
			if (i + 1 < Main.maxTilesX && j + 1 < Main.maxTilesY && Main.tile[i + 1, j + 1] != null && Main.tile[i + 1, j + 1].active)
			{
				byte arg_275_0 = Main.tile[i + 1, j + 1].type;
			}
			if ((num2 == 3 && num != 2 && num != 78) || (num2 == 24 && num != 23) || (num2 == 61 && num != 60) || (num2 == 71 && num != 70) || (num2 == 73 && num != 2 && num != 78) || (num2 == 74 && num != 60) || (num2 == 110 && num != 109) || (num2 == 113 && num != 109))
			{
				if (num == 23)
				{
					num2 = 24;
					if (Main.tile[i, j].frameX >= 162)
					{
						Main.tile[i, j].frameX = 126;
					}
				}
				else
				{
					if (num == 2)
					{
						if (num2 == 113)
						{
							num2 = 73;
						}
						else
						{
							num2 = 3;
						}
					}
					else
					{
						if (num == 109)
						{
							if (num2 == 73)
							{
								num2 = 113;
							}
							else
							{
								num2 = 110;
							}
						}
					}
				}
				if (num2 != (int)Main.tile[i, j].type)
				{
					Main.tile[i, j].type = (byte)num2;
					return;
				}
				WorldGen.KillTile(i, j, false, false, false);
			}
		}
		public static void WallFrame(int i, int j, bool resetFrame = false)
		{
			if (i >= 0 && j >= 0 && i < Main.maxTilesX && j < Main.maxTilesY && Main.tile[i, j] != null && Main.tile[i, j].wall > 0)
			{
				int num = -1;
				int num2 = -1;
				int num3 = -1;
				int num4 = -1;
				int num5 = -1;
				int num6 = -1;
				int num7 = -1;
				int num8 = -1;
				int wall = (int)Main.tile[i, j].wall;
				if (wall == 0)
				{
					return;
				}
				byte arg_89_0 = Main.tile[i, j].wallFrameX;
				byte arg_9B_0 = Main.tile[i, j].wallFrameY;
				Rectangle rectangle;
				rectangle.X = -1;
				rectangle.Y = -1;
				if (i - 1 < 0)
				{
					num = wall;
					num4 = wall;
					num6 = wall;
				}
				if (i + 1 >= Main.maxTilesX)
				{
					num3 = wall;
					num5 = wall;
					num8 = wall;
				}
				if (j - 1 < 0)
				{
					num = wall;
					num2 = wall;
					num3 = wall;
				}
				if (j + 1 >= Main.maxTilesY)
				{
					num6 = wall;
					num7 = wall;
					num8 = wall;
				}
				if (i - 1 >= 0 && Main.tile[i - 1, j] != null)
				{
					num4 = (int)Main.tile[i - 1, j].wall;
				}
				if (i + 1 < Main.maxTilesX && Main.tile[i + 1, j] != null)
				{
					num5 = (int)Main.tile[i + 1, j].wall;
				}
				if (j - 1 >= 0 && Main.tile[i, j - 1] != null)
				{
					num2 = (int)Main.tile[i, j - 1].wall;
				}
				if (j + 1 < Main.maxTilesY && Main.tile[i, j + 1] != null)
				{
					num7 = (int)Main.tile[i, j + 1].wall;
				}
				if (i - 1 >= 0 && j - 1 >= 0 && Main.tile[i - 1, j - 1] != null)
				{
					num = (int)Main.tile[i - 1, j - 1].wall;
				}
				if (i + 1 < Main.maxTilesX && j - 1 >= 0 && Main.tile[i + 1, j - 1] != null)
				{
					num3 = (int)Main.tile[i + 1, j - 1].wall;
				}
				if (i - 1 >= 0 && j + 1 < Main.maxTilesY && Main.tile[i - 1, j + 1] != null)
				{
					num6 = (int)Main.tile[i - 1, j + 1].wall;
				}
				if (i + 1 < Main.maxTilesX && j + 1 < Main.maxTilesY && Main.tile[i + 1, j + 1] != null)
				{
					num8 = (int)Main.tile[i + 1, j + 1].wall;
				}
				if (wall == 2)
				{
					if (j == (int)Main.worldSurface)
					{
						num7 = wall;
						num6 = wall;
						num8 = wall;
					}
					else
					{
						if (j >= (int)Main.worldSurface)
						{
							num7 = wall;
							num6 = wall;
							num8 = wall;
							num2 = wall;
							num = wall;
							num3 = wall;
							num4 = wall;
							num5 = wall;
						}
					}
				}
				if (num7 > 0)
				{
					num7 = wall;
				}
				if (num6 > 0)
				{
					num6 = wall;
				}
				if (num8 > 0)
				{
					num8 = wall;
				}
				if (num2 > 0)
				{
					num2 = wall;
				}
				if (num > 0)
				{
					num = wall;
				}
				if (num3 > 0)
				{
					num3 = wall;
				}
				if (num4 > 0)
				{
					num4 = wall;
				}
				if (num5 > 0)
				{
					num5 = wall;
				}
				int num9 = 0;
				if (resetFrame)
				{
					num9 = WorldGen.genRand.Next(0, 3);
					Main.tile[i, j].wallFrameNumber = (byte)num9;
				}
				else
				{
					num9 = (int)Main.tile[i, j].wallFrameNumber;
				}
				if (rectangle.X < 0 || rectangle.Y < 0)
				{
					if (num2 == wall && num7 == wall && (num4 == wall & num5 == wall))
					{
						if (num != wall && num3 != wall)
						{
							if (num9 == 0)
							{
								rectangle.X = 108;
								rectangle.Y = 18;
							}
							if (num9 == 1)
							{
								rectangle.X = 126;
								rectangle.Y = 18;
							}
							if (num9 == 2)
							{
								rectangle.X = 144;
								rectangle.Y = 18;
							}
						}
						else
						{
							if (num6 != wall && num8 != wall)
							{
								if (num9 == 0)
								{
									rectangle.X = 108;
									rectangle.Y = 36;
								}
								if (num9 == 1)
								{
									rectangle.X = 126;
									rectangle.Y = 36;
								}
								if (num9 == 2)
								{
									rectangle.X = 144;
									rectangle.Y = 36;
								}
							}
							else
							{
								if (num != wall && num6 != wall)
								{
									if (num9 == 0)
									{
										rectangle.X = 180;
										rectangle.Y = 0;
									}
									if (num9 == 1)
									{
										rectangle.X = 180;
										rectangle.Y = 18;
									}
									if (num9 == 2)
									{
										rectangle.X = 180;
										rectangle.Y = 36;
									}
								}
								else
								{
									if (num3 != wall && num8 != wall)
									{
										if (num9 == 0)
										{
											rectangle.X = 198;
											rectangle.Y = 0;
										}
										if (num9 == 1)
										{
											rectangle.X = 198;
											rectangle.Y = 18;
										}
										if (num9 == 2)
										{
											rectangle.X = 198;
											rectangle.Y = 36;
										}
									}
									else
									{
										if (num9 == 0)
										{
											rectangle.X = 18;
											rectangle.Y = 18;
										}
										if (num9 == 1)
										{
											rectangle.X = 36;
											rectangle.Y = 18;
										}
										if (num9 == 2)
										{
											rectangle.X = 54;
											rectangle.Y = 18;
										}
									}
								}
							}
						}
					}
					else
					{
						if (num2 != wall && num7 == wall && (num4 == wall & num5 == wall))
						{
							if (num9 == 0)
							{
								rectangle.X = 18;
								rectangle.Y = 0;
							}
							if (num9 == 1)
							{
								rectangle.X = 36;
								rectangle.Y = 0;
							}
							if (num9 == 2)
							{
								rectangle.X = 54;
								rectangle.Y = 0;
							}
						}
						else
						{
							if (num2 == wall && num7 != wall && (num4 == wall & num5 == wall))
							{
								if (num9 == 0)
								{
									rectangle.X = 18;
									rectangle.Y = 36;
								}
								if (num9 == 1)
								{
									rectangle.X = 36;
									rectangle.Y = 36;
								}
								if (num9 == 2)
								{
									rectangle.X = 54;
									rectangle.Y = 36;
								}
							}
							else
							{
								if (num2 == wall && num7 == wall && (num4 != wall & num5 == wall))
								{
									if (num9 == 0)
									{
										rectangle.X = 0;
										rectangle.Y = 0;
									}
									if (num9 == 1)
									{
										rectangle.X = 0;
										rectangle.Y = 18;
									}
									if (num9 == 2)
									{
										rectangle.X = 0;
										rectangle.Y = 36;
									}
								}
								else
								{
									if (num2 == wall && num7 == wall && (num4 == wall & num5 != wall))
									{
										if (num9 == 0)
										{
											rectangle.X = 72;
											rectangle.Y = 0;
										}
										if (num9 == 1)
										{
											rectangle.X = 72;
											rectangle.Y = 18;
										}
										if (num9 == 2)
										{
											rectangle.X = 72;
											rectangle.Y = 36;
										}
									}
									else
									{
										if (num2 != wall && num7 == wall && (num4 != wall & num5 == wall))
										{
											if (num9 == 0)
											{
												rectangle.X = 0;
												rectangle.Y = 54;
											}
											if (num9 == 1)
											{
												rectangle.X = 36;
												rectangle.Y = 54;
											}
											if (num9 == 2)
											{
												rectangle.X = 72;
												rectangle.Y = 54;
											}
										}
										else
										{
											if (num2 != wall && num7 == wall && (num4 == wall & num5 != wall))
											{
												if (num9 == 0)
												{
													rectangle.X = 18;
													rectangle.Y = 54;
												}
												if (num9 == 1)
												{
													rectangle.X = 54;
													rectangle.Y = 54;
												}
												if (num9 == 2)
												{
													rectangle.X = 90;
													rectangle.Y = 54;
												}
											}
											else
											{
												if (num2 == wall && num7 != wall && (num4 != wall & num5 == wall))
												{
													if (num9 == 0)
													{
														rectangle.X = 0;
														rectangle.Y = 72;
													}
													if (num9 == 1)
													{
														rectangle.X = 36;
														rectangle.Y = 72;
													}
													if (num9 == 2)
													{
														rectangle.X = 72;
														rectangle.Y = 72;
													}
												}
												else
												{
													if (num2 == wall && num7 != wall && (num4 == wall & num5 != wall))
													{
														if (num9 == 0)
														{
															rectangle.X = 18;
															rectangle.Y = 72;
														}
														if (num9 == 1)
														{
															rectangle.X = 54;
															rectangle.Y = 72;
														}
														if (num9 == 2)
														{
															rectangle.X = 90;
															rectangle.Y = 72;
														}
													}
													else
													{
														if (num2 == wall && num7 == wall && (num4 != wall & num5 != wall))
														{
															if (num9 == 0)
															{
																rectangle.X = 90;
																rectangle.Y = 0;
															}
															if (num9 == 1)
															{
																rectangle.X = 90;
																rectangle.Y = 18;
															}
															if (num9 == 2)
															{
																rectangle.X = 90;
																rectangle.Y = 36;
															}
														}
														else
														{
															if (num2 != wall && num7 != wall && (num4 == wall & num5 == wall))
															{
																if (num9 == 0)
																{
																	rectangle.X = 108;
																	rectangle.Y = 72;
																}
																if (num9 == 1)
																{
																	rectangle.X = 126;
																	rectangle.Y = 72;
																}
																if (num9 == 2)
																{
																	rectangle.X = 144;
																	rectangle.Y = 72;
																}
															}
															else
															{
																if (num2 != wall && num7 == wall && (num4 != wall & num5 != wall))
																{
																	if (num9 == 0)
																	{
																		rectangle.X = 108;
																		rectangle.Y = 0;
																	}
																	if (num9 == 1)
																	{
																		rectangle.X = 126;
																		rectangle.Y = 0;
																	}
																	if (num9 == 2)
																	{
																		rectangle.X = 144;
																		rectangle.Y = 0;
																	}
																}
																else
																{
																	if (num2 == wall && num7 != wall && (num4 != wall & num5 != wall))
																	{
																		if (num9 == 0)
																		{
																			rectangle.X = 108;
																			rectangle.Y = 54;
																		}
																		if (num9 == 1)
																		{
																			rectangle.X = 126;
																			rectangle.Y = 54;
																		}
																		if (num9 == 2)
																		{
																			rectangle.X = 144;
																			rectangle.Y = 54;
																		}
																	}
																	else
																	{
																		if (num2 != wall && num7 != wall && (num4 != wall & num5 == wall))
																		{
																			if (num9 == 0)
																			{
																				rectangle.X = 162;
																				rectangle.Y = 0;
																			}
																			if (num9 == 1)
																			{
																				rectangle.X = 162;
																				rectangle.Y = 18;
																			}
																			if (num9 == 2)
																			{
																				rectangle.X = 162;
																				rectangle.Y = 36;
																			}
																		}
																		else
																		{
																			if (num2 != wall && num7 != wall && (num4 == wall & num5 != wall))
																			{
																				if (num9 == 0)
																				{
																					rectangle.X = 216;
																					rectangle.Y = 0;
																				}
																				if (num9 == 1)
																				{
																					rectangle.X = 216;
																					rectangle.Y = 18;
																				}
																				if (num9 == 2)
																				{
																					rectangle.X = 216;
																					rectangle.Y = 36;
																				}
																			}
																			else
																			{
																				if (num2 != wall && num7 != wall && (num4 != wall & num5 != wall))
																				{
																					if (num9 == 0)
																					{
																						rectangle.X = 162;
																						rectangle.Y = 54;
																					}
																					if (num9 == 1)
																					{
																						rectangle.X = 180;
																						rectangle.Y = 54;
																					}
																					if (num9 == 2)
																					{
																						rectangle.X = 198;
																						rectangle.Y = 54;
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
				if (rectangle.X <= -1 || rectangle.Y <= -1)
				{
					if (num9 <= 0)
					{
						rectangle.X = 18;
						rectangle.Y = 18;
					}
					if (num9 == 1)
					{
						rectangle.X = 36;
						rectangle.Y = 18;
					}
					if (num9 >= 2)
					{
						rectangle.X = 54;
						rectangle.Y = 18;
					}
				}
				Main.tile[i, j].wallFrameX = (byte)rectangle.X;
				Main.tile[i, j].wallFrameY = (byte)rectangle.Y;
			}
		}
		public static void TileFrame(int i, int j, bool resetFrame = false, bool noBreak = false)
		{
			try
			{
				if (i > 5 && j > 5 && i < Main.maxTilesX - 5 && j < Main.maxTilesY - 5 && Main.tile[i, j] != null)
				{
					if (Main.tile[i, j].liquid > 0 && Main.netMode != 1 && !WorldGen.noLiquidCheck)
					{
						Liquid.AddWater(i, j);
					}
					if (Main.tile[i, j].active)
					{
						if (!noBreak || !Main.tileFrameImportant[(int)Main.tile[i, j].type] || Main.tile[i, j].type == 4)
						{
							int num = (int)Main.tile[i, j].type;
							if (Main.tileStone[num])
							{
								num = 1;
							}
							int frameX = (int)Main.tile[i, j].frameX;
							int frameY = (int)Main.tile[i, j].frameY;
							Rectangle rectangle = new Rectangle(-1, -1, 0, 0);
							if (Main.tileFrameImportant[(int)Main.tile[i, j].type])
							{
								if (num == 4)
								{
									short num2 = 0;
									if (Main.tile[i, j].frameX >= 66)
									{
										num2 = 66;
									}
									int num3 = -1;
									int num4 = -1;
									int num5 = -1;
									int num6 = -1;
									int num7 = -1;
									int num8 = -1;
									int num9 = -1;
									if (Main.tile[i, j - 1] != null && Main.tile[i, j - 1].active)
									{
										byte arg_186_0 = Main.tile[i, j - 1].type;
									}
									if (Main.tile[i, j + 1] != null && Main.tile[i, j + 1].active)
									{
										num3 = (int)Main.tile[i, j + 1].type;
									}
									if (Main.tile[i - 1, j] != null && Main.tile[i - 1, j].active)
									{
										num4 = (int)Main.tile[i - 1, j].type;
									}
									if (Main.tile[i + 1, j] != null && Main.tile[i + 1, j].active)
									{
										num5 = (int)Main.tile[i + 1, j].type;
									}
									if (Main.tile[i - 1, j + 1] != null && Main.tile[i - 1, j + 1].active)
									{
										num6 = (int)Main.tile[i - 1, j + 1].type;
									}
									if (Main.tile[i + 1, j + 1] != null && Main.tile[i + 1, j + 1].active)
									{
										num7 = (int)Main.tile[i + 1, j + 1].type;
									}
									if (Main.tile[i - 1, j - 1] != null && Main.tile[i - 1, j - 1].active)
									{
										num8 = (int)Main.tile[i - 1, j - 1].type;
									}
									if (Main.tile[i + 1, j - 1] != null && Main.tile[i + 1, j - 1].active)
									{
										num9 = (int)Main.tile[i + 1, j - 1].type;
									}
									if (num3 >= 0 && Main.tileSolid[num3] && !Main.tileNoAttach[num3])
									{
										Main.tile[i, j].frameX = num2;
									}
									else
									{
										if ((num4 >= 0 && Main.tileSolid[num4] && !Main.tileNoAttach[num4]) || num4 == 124 || (num4 == 5 && num8 == 5 && num6 == 5))
										{
											Main.tile[i, j].frameX = (byte)(22 + num2);
										}
										else
										{
											if ((num5 >= 0 && Main.tileSolid[num5] && !Main.tileNoAttach[num5]) || num5 == 124 || (num5 == 5 && num9 == 5 && num7 == 5))
											{
												Main.tile[i, j].frameX = (byte)(44 + num2);
											}
											else
											{
												WorldGen.KillTile(i, j, false, false, false);
											}
										}
									}
								}
								else
								{
									if (num == 136)
									{
										int num10 = -1;
										int num11 = -1;
										int num12 = -1;
										if (Main.tile[i, j - 1] != null && Main.tile[i, j - 1].active)
										{
											byte arg_450_0 = Main.tile[i, j - 1].type;
										}
										if (Main.tile[i, j + 1] != null && Main.tile[i, j + 1].active)
										{
											num10 = (int)Main.tile[i, j + 1].type;
										}
										if (Main.tile[i - 1, j] != null && Main.tile[i - 1, j].active)
										{
											num11 = (int)Main.tile[i - 1, j].type;
										}
										if (Main.tile[i + 1, j] != null && Main.tile[i + 1, j].active)
										{
											num12 = (int)Main.tile[i + 1, j].type;
										}
										if (num10 >= 0 && Main.tileSolid[num10] && !Main.tileNoAttach[num10])
										{
											Main.tile[i, j].frameX = 0;
										}
										else
										{
											if ((num11 >= 0 && Main.tileSolid[num11] && !Main.tileNoAttach[num11]) || num11 == 124 || num11 == 5)
											{
												Main.tile[i, j].frameX = 18;
											}
											else
											{
												if ((num12 >= 0 && Main.tileSolid[num12] && !Main.tileNoAttach[num12]) || num12 == 124 || num12 == 5)
												{
													Main.tile[i, j].frameX = 36;
												}
												else
												{
													WorldGen.KillTile(i, j, false, false, false);
												}
											}
										}
									}
									else
									{
										if (num == 129 || num == 149)
										{
											int num13 = -1;
											int num14 = -1;
											int num15 = -1;
											int num16 = -1;
											if (Main.tile[i, j - 1] != null && Main.tile[i, j - 1].active)
											{
												num14 = (int)Main.tile[i, j - 1].type;
											}
											if (Main.tile[i, j + 1] != null && Main.tile[i, j + 1].active)
											{
												num13 = (int)Main.tile[i, j + 1].type;
											}
											if (Main.tile[i - 1, j] != null && Main.tile[i - 1, j].active)
											{
												num15 = (int)Main.tile[i - 1, j].type;
											}
											if (Main.tile[i + 1, j] != null && Main.tile[i + 1, j].active)
											{
												num16 = (int)Main.tile[i + 1, j].type;
											}
											if (num13 >= 0 && Main.tileSolid[num13] && !Main.tileSolidTop[num13])
											{
												Main.tile[i, j].frameY = 0;
											}
											else
											{
												if (num15 >= 0 && Main.tileSolid[num15] && !Main.tileSolidTop[num15])
												{
													Main.tile[i, j].frameY = 54;
												}
												else
												{
													if (num16 >= 0 && Main.tileSolid[num16] && !Main.tileSolidTop[num16])
													{
														Main.tile[i, j].frameY = 36;
													}
													else
													{
														if (num14 >= 0 && Main.tileSolid[num14] && !Main.tileSolidTop[num14])
														{
															Main.tile[i, j].frameY = 18;
														}
														else
														{
															WorldGen.KillTile(i, j, false, false, false);
														}
													}
												}
											}
										}
										else
										{
											if (num == 3 || num == 24 || num == 61 || num == 71 || num == 73 || num == 74 || num == 110 || num == 113)
											{
												WorldGen.PlantCheck(i, j);
											}
											else
											{
												if (num == 12 || num == 31)
												{
													WorldGen.CheckOrb(i, j, num);
												}
												else
												{
													if (num == 10)
													{
														if (!WorldGen.destroyObject)
														{
															int frameY2 = (int)Main.tile[i, j].frameY;
															int num17 = j;
															bool flag = false;
															if (frameY2 == 0)
															{
																num17 = j;
															}
															if (frameY2 == 18)
															{
																num17 = j - 1;
															}
															if (frameY2 == 36)
															{
																num17 = j - 2;
															}
															if (!Main.tile[i, num17 - 1].active || !Main.tileSolid[(int)Main.tile[i, num17 - 1].type])
															{
																flag = true;
															}
															if (!Main.tile[i, num17 + 3].active || !Main.tileSolid[(int)Main.tile[i, num17 + 3].type])
															{
																flag = true;
															}
															if (!Main.tile[i, num17].active || (int)Main.tile[i, num17].type != num)
															{
																flag = true;
															}
															if (!Main.tile[i, num17 + 1].active || (int)Main.tile[i, num17 + 1].type != num)
															{
																flag = true;
															}
															if (!Main.tile[i, num17 + 2].active || (int)Main.tile[i, num17 + 2].type != num)
															{
																flag = true;
															}
															if (flag)
															{
																WorldGen.destroyObject = true;
																WorldGen.KillTile(i, num17, false, false, false);
																WorldGen.KillTile(i, num17 + 1, false, false, false);
																WorldGen.KillTile(i, num17 + 2, false, false, false);
																Item.NewItem(i * 16, j * 16, 16, 16, 25, 1, false, 0);
															}
															WorldGen.destroyObject = false;
														}
													}
													else
													{
														if (num == 11)
														{
															if (!WorldGen.destroyObject)
															{
																int num18 = 0;
																int num19 = i;
																int num20 = j;
																int frameX2 = (int)Main.tile[i, j].frameX;
																int frameY3 = (int)Main.tile[i, j].frameY;
																bool flag2 = false;
																if (frameX2 == 0)
																{
																	num19 = i;
																	num18 = 1;
																}
																else
																{
																	if (frameX2 == 18)
																	{
																		num19 = i - 1;
																		num18 = 1;
																	}
																	else
																	{
																		if (frameX2 == 36)
																		{
																			num19 = i + 1;
																			num18 = -1;
																		}
																		else
																		{
																			if (frameX2 == 54)
																			{
																				num19 = i;
																				num18 = -1;
																			}
																		}
																	}
																}
																if (frameY3 == 0)
																{
																	num20 = j;
																}
																else
																{
																	if (frameY3 == 18)
																	{
																		num20 = j - 1;
																	}
																	else
																	{
																		if (frameY3 == 36)
																		{
																			num20 = j - 2;
																		}
																	}
																}
																if (!Main.tile[num19, num20 - 1].active || !Main.tileSolid[(int)Main.tile[num19, num20 - 1].type] || !Main.tile[num19, num20 + 3].active || !Main.tileSolid[(int)Main.tile[num19, num20 + 3].type])
																{
																	flag2 = true;
																	WorldGen.destroyObject = true;
																	Item.NewItem(i * 16, j * 16, 16, 16, 25, 1, false, 0);
																}
																int num21 = num19;
																if (num18 == -1)
																{
																	num21 = num19 - 1;
																}
																for (int k = num21; k < num21 + 2; k++)
																{
																	for (int l = num20; l < num20 + 3; l++)
																	{
																		if (!flag2 && (Main.tile[k, l].type != 11 || !Main.tile[k, l].active))
																		{
																			WorldGen.destroyObject = true;
																			Item.NewItem(i * 16, j * 16, 16, 16, 25, 1, false, 0);
																			flag2 = true;
																			k = num21;
																			l = num20;
																		}
																		if (flag2)
																		{
																			WorldGen.KillTile(k, l, false, false, false);
																		}
																	}
																}
																WorldGen.destroyObject = false;
															}
														}
														else
														{
															if (num == 34 || num == 35 || num == 36 || num == 106)
															{
																WorldGen.Check3x3(i, j, (int)((byte)num));
															}
															else
															{
																if (num == 15 || num == 20)
																{
																	WorldGen.Check1x2(i, j, (byte)num);
																}
																else
																{
																	if (num == 14 || num == 17 || num == 26 || num == 77 || num == 86 || num == 87 || num == 88 || num == 89 || num == 114 || num == 133)
																	{
																		WorldGen.Check3x2(i, j, (int)((byte)num));
																	}
																	else
																	{
																		if (num == 135 || num == 144 || num == 141)
																		{
																			WorldGen.Check1x1(i, j, num);
																		}
																		else
																		{
																			if (num == 16 || num == 18 || num == 29 || num == 103 || num == 134)
																			{
																				WorldGen.Check2x1(i, j, (byte)num);
																			}
																			else
																			{
																				if (num == 13 || num == 33 || num == 50 || num == 78)
																				{
																					WorldGen.CheckOnTable1x1(i, j, (int)((byte)num));
																				}
																				else
																				{
																					if (num == 21)
																					{
																						WorldGen.CheckChest(i, j, (int)((byte)num));
																					}
																					else
																					{
																						if (num == 128)
																						{
																							WorldGen.CheckMan(i, j);
																						}
																						else
																						{
																							if (num == 27)
																							{
																								WorldGen.CheckSunflower(i, j, 27);
																							}
																							else
																							{
																								if (num == 28)
																								{
																									WorldGen.CheckPot(i, j, 28);
																								}
																								else
																								{
																									if (num == 132 || num == 138 || num == 142 || num == 143)
																									{
																										WorldGen.Check2x2(i, j, num);
																									}
																									else
																									{
																										if (num == 91)
																										{
																											WorldGen.CheckBanner(i, j, (byte)num);
																										}
																										else
																										{
																											if (num == 139)
																											{
																												WorldGen.CheckMB(i, j, (int)((byte)num));
																											}
																											else
																											{
																												if (num == 92 || num == 93)
																												{
																													WorldGen.Check1xX(i, j, (byte)num);
																												}
																												else
																												{
																													if (num == 104 || num == 105)
																													{
																														WorldGen.Check2xX(i, j, (byte)num);
																													}
																													else
																													{
																														if (num == 101 || num == 102)
																														{
																															WorldGen.Check3x4(i, j, (int)((byte)num));
																														}
																														else
																														{
																															if (num == 42)
																															{
																																WorldGen.Check1x2Top(i, j, (byte)num);
																															}
																															else
																															{
																																if (num == 55 || num == 85)
																																{
																																	WorldGen.CheckSign(i, j, num);
																																}
																																else
																																{
																																	if (num == 79 || num == 90)
																																	{
																																		WorldGen.Check4x2(i, j, num);
																																	}
																																	else
																																	{
																																		if (num == 85 || num == 94 || num == 95 || num == 96 || num == 97 || num == 98 || num == 99 || num == 100 || num == 125 || num == 126)
																																		{
																																			WorldGen.Check2x2(i, j, num);
																																		}
																																		else
																																		{
																																			if (num == 81)
																																			{
																																				int num22 = -1;
																																				int num23 = -1;
																																				int num24 = -1;
																																				int num25 = -1;
																																				if (Main.tile[i, j - 1] != null && Main.tile[i, j - 1].active)
																																				{
																																					num23 = (int)Main.tile[i, j - 1].type;
																																				}
																																				if (Main.tile[i, j + 1] != null && Main.tile[i, j + 1].active)
																																				{
																																					num22 = (int)Main.tile[i, j + 1].type;
																																				}
																																				if (Main.tile[i - 1, j] != null && Main.tile[i - 1, j].active)
																																				{
																																					num24 = (int)Main.tile[i - 1, j].type;
																																				}
																																				if (Main.tile[i + 1, j] != null && Main.tile[i + 1, j].active)
																																				{
																																					num25 = (int)Main.tile[i + 1, j].type;
																																				}
																																				if (num24 != -1 || num23 != -1 || num25 != -1)
																																				{
																																					WorldGen.KillTile(i, j, false, false, false);
																																				}
																																				else
																																				{
																																					if (num22 < 0 || !Main.tileSolid[num22])
																																					{
																																						WorldGen.KillTile(i, j, false, false, false);
																																					}
																																				}
																																			}
																																			else
																																			{
																																				if (Main.tileAlch[num])
																																				{
																																					WorldGen.CheckAlch(i, j);
																																				}
																																				else
																																				{
																																					if (num == 72)
																																					{
																																						int num26 = -1;
																																						int num27 = -1;
																																						if (Main.tile[i, j - 1] != null && Main.tile[i, j - 1].active)
																																						{
																																							num27 = (int)Main.tile[i, j - 1].type;
																																						}
																																						if (Main.tile[i, j + 1] != null && Main.tile[i, j + 1].active)
																																						{
																																							num26 = (int)Main.tile[i, j + 1].type;
																																						}
																																						if (num26 != num && num26 != 70)
																																						{
																																							WorldGen.KillTile(i, j, false, false, false);
																																						}
																																						else
																																						{
																																							if (num27 != num && Main.tile[i, j].frameX == 0)
																																							{
																																								Main.tile[i, j].frameNumber = (byte)WorldGen.genRand.Next(3);
																																								if (Main.tile[i, j].frameNumber == 0)
																																								{
																																									Main.tile[i, j].frameX = 18;
																																									Main.tile[i, j].frameY = 0;
																																								}
																																								if (Main.tile[i, j].frameNumber == 1)
																																								{
																																									Main.tile[i, j].frameX = 18;
																																									Main.tile[i, j].frameY = 18;
																																								}
																																								if (Main.tile[i, j].frameNumber == 2)
																																								{
																																									Main.tile[i, j].frameX = 18;
																																									Main.tile[i, j].frameY = 36;
																																								}
																																							}
																																						}
																																					}
																																					else
																																					{
																																						if (num == 5)
																																						{
																																							WorldGen.CheckTree(i, j);
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
							else
							{
								int num28 = -1;
								int num29 = -1;
								int num30 = -1;
								int num31 = -1;
								int num32 = -1;
								int num33 = -1;
								int num34 = -1;
								int num35 = -1;
								if (Main.tile[i - 1, j] != null && Main.tile[i - 1, j].active)
								{
									if (Main.tileStone[(int)Main.tile[i - 1, j].type])
									{
										num31 = 1;
									}
									else
									{
										num31 = (int)Main.tile[i - 1, j].type;
									}
								}
								if (Main.tile[i + 1, j] != null && Main.tile[i + 1, j].active)
								{
									if (Main.tileStone[(int)Main.tile[i + 1, j].type])
									{
										num32 = 1;
									}
									else
									{
										num32 = (int)Main.tile[i + 1, j].type;
									}
								}
								if (Main.tile[i, j - 1] != null && Main.tile[i, j - 1].active)
								{
									if (Main.tileStone[(int)Main.tile[i, j - 1].type])
									{
										num29 = 1;
									}
									else
									{
										num29 = (int)Main.tile[i, j - 1].type;
									}
								}
								if (Main.tile[i, j + 1] != null && Main.tile[i, j + 1].active)
								{
									if (Main.tileStone[(int)Main.tile[i, j + 1].type])
									{
										num34 = 1;
									}
									else
									{
										num34 = (int)Main.tile[i, j + 1].type;
									}
								}
								if (Main.tile[i - 1, j - 1] != null && Main.tile[i - 1, j - 1].active)
								{
									if (Main.tileStone[(int)Main.tile[i - 1, j - 1].type])
									{
										num28 = 1;
									}
									else
									{
										num28 = (int)Main.tile[i - 1, j - 1].type;
									}
								}
								if (Main.tile[i + 1, j - 1] != null && Main.tile[i + 1, j - 1].active)
								{
									if (Main.tileStone[(int)Main.tile[i + 1, j - 1].type])
									{
										num30 = 1;
									}
									else
									{
										num30 = (int)Main.tile[i + 1, j - 1].type;
									}
								}
								if (Main.tile[i - 1, j + 1] != null && Main.tile[i - 1, j + 1].active)
								{
									if (Main.tileStone[(int)Main.tile[i - 1, j + 1].type])
									{
										num33 = 1;
									}
									else
									{
										num33 = (int)Main.tile[i - 1, j + 1].type;
									}
								}
								if (Main.tile[i + 1, j + 1] != null && Main.tile[i + 1, j + 1].active)
								{
									if (Main.tileStone[(int)Main.tile[i + 1, j + 1].type])
									{
										num35 = 1;
									}
									else
									{
										num35 = (int)Main.tile[i + 1, j + 1].type;
									}
								}
								if (!Main.tileSolid[num])
								{
									if (num == 49)
									{
										WorldGen.CheckOnTable1x1(i, j, (int)((byte)num));
										return;
									}
									if (num == 80)
									{
										WorldGen.CactusFrame(i, j);
										return;
									}
								}
								else
								{
									if (num == 19)
									{
										if (num32 >= 0 && !Main.tileSolid[num32])
										{
											num32 = -1;
										}
										if (num31 >= 0 && !Main.tileSolid[num31])
										{
											num31 = -1;
										}
										if (num31 == num && num32 == num)
										{
											if (Main.tile[i, j].frameNumber == 0)
											{
												rectangle.X = 0;
												rectangle.Y = 0;
											}
											else
											{
												if (Main.tile[i, j].frameNumber == 1)
												{
													rectangle.X = 0;
													rectangle.Y = 18;
												}
												else
												{
													rectangle.X = 0;
													rectangle.Y = 36;
												}
											}
										}
										else
										{
											if (num31 == num && num32 == -1)
											{
												if (Main.tile[i, j].frameNumber == 0)
												{
													rectangle.X = 18;
													rectangle.Y = 0;
												}
												else
												{
													if (Main.tile[i, j].frameNumber == 1)
													{
														rectangle.X = 18;
														rectangle.Y = 18;
													}
													else
													{
														rectangle.X = 18;
														rectangle.Y = 36;
													}
												}
											}
											else
											{
												if (num31 == -1 && num32 == num)
												{
													if (Main.tile[i, j].frameNumber == 0)
													{
														rectangle.X = 36;
														rectangle.Y = 0;
													}
													else
													{
														if (Main.tile[i, j].frameNumber == 1)
														{
															rectangle.X = 36;
															rectangle.Y = 18;
														}
														else
														{
															rectangle.X = 36;
															rectangle.Y = 36;
														}
													}
												}
												else
												{
													if (num31 != num && num32 == num)
													{
														if (Main.tile[i, j].frameNumber == 0)
														{
															rectangle.X = 54;
															rectangle.Y = 0;
														}
														else
														{
															if (Main.tile[i, j].frameNumber == 1)
															{
																rectangle.X = 54;
																rectangle.Y = 18;
															}
															else
															{
																rectangle.X = 54;
																rectangle.Y = 36;
															}
														}
													}
													else
													{
														if (num31 == num && num32 != num)
														{
															if (Main.tile[i, j].frameNumber == 0)
															{
																rectangle.X = 72;
																rectangle.Y = 0;
															}
															else
															{
																if (Main.tile[i, j].frameNumber == 1)
																{
																	rectangle.X = 72;
																	rectangle.Y = 18;
																}
																else
																{
																	rectangle.X = 72;
																	rectangle.Y = 36;
																}
															}
														}
														else
														{
															if (num31 != num && num31 != -1 && num32 == -1)
															{
																if (Main.tile[i, j].frameNumber == 0)
																{
																	rectangle.X = 108;
																	rectangle.Y = 0;
																}
																else
																{
																	if (Main.tile[i, j].frameNumber == 1)
																	{
																		rectangle.X = 108;
																		rectangle.Y = 18;
																	}
																	else
																	{
																		rectangle.X = 108;
																		rectangle.Y = 36;
																	}
																}
															}
															else
															{
																if (num31 == -1 && num32 != num && num32 != -1)
																{
																	if (Main.tile[i, j].frameNumber == 0)
																	{
																		rectangle.X = 126;
																		rectangle.Y = 0;
																	}
																	else
																	{
																		if (Main.tile[i, j].frameNumber == 1)
																		{
																			rectangle.X = 126;
																			rectangle.Y = 18;
																		}
																		else
																		{
																			rectangle.X = 126;
																			rectangle.Y = 36;
																		}
																	}
																}
																else
																{
																	if (Main.tile[i, j].frameNumber == 0)
																	{
																		rectangle.X = 90;
																		rectangle.Y = 0;
																	}
																	else
																	{
																		if (Main.tile[i, j].frameNumber == 1)
																		{
																			rectangle.X = 90;
																			rectangle.Y = 18;
																		}
																		else
																		{
																			rectangle.X = 90;
																			rectangle.Y = 36;
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
								WorldGen.mergeUp = false;
								WorldGen.mergeDown = false;
								WorldGen.mergeLeft = false;
								WorldGen.mergeRight = false;
								int num36 = 0;
								if (resetFrame)
								{
									num36 = WorldGen.genRand.Next(0, 3);
									Main.tile[i, j].frameNumber = (byte)num36;
								}
								else
								{
									num36 = (int)Main.tile[i, j].frameNumber;
								}
								if (num == 0)
								{
									if (num29 >= 0 && Main.tileMergeDirt[num29])
									{
										WorldGen.TileFrame(i, j - 1, false, false);
										if (WorldGen.mergeDown)
										{
											num29 = num;
										}
									}
									if (num34 >= 0 && Main.tileMergeDirt[num34])
									{
										WorldGen.TileFrame(i, j + 1, false, false);
										if (WorldGen.mergeUp)
										{
											num34 = num;
										}
									}
									if (num31 >= 0 && Main.tileMergeDirt[num31])
									{
										WorldGen.TileFrame(i - 1, j, false, false);
										if (WorldGen.mergeRight)
										{
											num31 = num;
										}
									}
									if (num32 >= 0 && Main.tileMergeDirt[num32])
									{
										WorldGen.TileFrame(i + 1, j, false, false);
										if (WorldGen.mergeLeft)
										{
											num32 = num;
										}
									}
									if (num29 == 2 || num29 == 23 || num29 == 109)
									{
										num29 = num;
									}
									if (num34 == 2 || num34 == 23 || num34 == 109)
									{
										num34 = num;
									}
									if (num31 == 2 || num31 == 23 || num31 == 109)
									{
										num31 = num;
									}
									if (num32 == 2 || num32 == 23 || num32 == 109)
									{
										num32 = num;
									}
									if (num28 >= 0 && Main.tileMergeDirt[num28])
									{
										num28 = num;
									}
									else
									{
										if (num28 == 2 || num28 == 23 || num28 == 109)
										{
											num28 = num;
										}
									}
									if (num30 >= 0 && Main.tileMergeDirt[num30])
									{
										num30 = num;
									}
									else
									{
										if (num30 == 2 || num30 == 23 || num30 == 109)
										{
											num30 = num;
										}
									}
									if (num33 >= 0 && Main.tileMergeDirt[num33])
									{
										num33 = num;
									}
									else
									{
										if (num33 == 2 || num33 == 23 || num30 == 109)
										{
											num33 = num;
										}
									}
									if (num35 >= 0 && Main.tileMergeDirt[num35])
									{
										num35 = num;
									}
									else
									{
										if (num35 == 2 || num35 == 23 || num35 == 109)
										{
											num35 = num;
										}
									}
									if ((double)j < Main.rockLayer)
									{
										if (num29 == 59)
										{
											num29 = -2;
										}
										if (num34 == 59)
										{
											num34 = -2;
										}
										if (num31 == 59)
										{
											num31 = -2;
										}
										if (num32 == 59)
										{
											num32 = -2;
										}
										if (num28 == 59)
										{
											num28 = -2;
										}
										if (num30 == 59)
										{
											num30 = -2;
										}
										if (num33 == 59)
										{
											num33 = -2;
										}
										if (num35 == 59)
										{
											num35 = -2;
										}
									}
								}
								else
								{
									if (Main.tileMergeDirt[num])
									{
										if (num29 == 0)
										{
											num29 = -2;
										}
										if (num34 == 0)
										{
											num34 = -2;
										}
										if (num31 == 0)
										{
											num31 = -2;
										}
										if (num32 == 0)
										{
											num32 = -2;
										}
										if (num28 == 0)
										{
											num28 = -2;
										}
										if (num30 == 0)
										{
											num30 = -2;
										}
										if (num33 == 0)
										{
											num33 = -2;
										}
										if (num35 == 0)
										{
											num35 = -2;
										}
										if (num == 1)
										{
											if ((double)j > Main.rockLayer)
											{
												if (num29 == 59)
												{
													WorldGen.TileFrame(i, j - 1, false, false);
													if (WorldGen.mergeDown)
													{
														num29 = num;
													}
												}
												if (num34 == 59)
												{
													WorldGen.TileFrame(i, j + 1, false, false);
													if (WorldGen.mergeUp)
													{
														num34 = num;
													}
												}
												if (num31 == 59)
												{
													WorldGen.TileFrame(i - 1, j, false, false);
													if (WorldGen.mergeRight)
													{
														num31 = num;
													}
												}
												if (num32 == 59)
												{
													WorldGen.TileFrame(i + 1, j, false, false);
													if (WorldGen.mergeLeft)
													{
														num32 = num;
													}
												}
												if (num28 == 59)
												{
													num28 = num;
												}
												if (num30 == 59)
												{
													num30 = num;
												}
												if (num33 == 59)
												{
													num33 = num;
												}
												if (num35 == 59)
												{
													num35 = num;
												}
											}
											if (num29 == 57)
											{
												WorldGen.TileFrame(i, j - 1, false, false);
												if (WorldGen.mergeDown)
												{
													num29 = num;
												}
											}
											if (num34 == 57)
											{
												WorldGen.TileFrame(i, j + 1, false, false);
												if (WorldGen.mergeUp)
												{
													num34 = num;
												}
											}
											if (num31 == 57)
											{
												WorldGen.TileFrame(i - 1, j, false, false);
												if (WorldGen.mergeRight)
												{
													num31 = num;
												}
											}
											if (num32 == 57)
											{
												WorldGen.TileFrame(i + 1, j, false, false);
												if (WorldGen.mergeLeft)
												{
													num32 = num;
												}
											}
											if (num28 == 57)
											{
												num28 = num;
											}
											if (num30 == 57)
											{
												num30 = num;
											}
											if (num33 == 57)
											{
												num33 = num;
											}
											if (num35 == 57)
											{
												num35 = num;
											}
										}
									}
									else
									{
										if (num == 58 || num == 76 || num == 75)
										{
											if (num29 == 57)
											{
												num29 = -2;
											}
											if (num34 == 57)
											{
												num34 = -2;
											}
											if (num31 == 57)
											{
												num31 = -2;
											}
											if (num32 == 57)
											{
												num32 = -2;
											}
											if (num28 == 57)
											{
												num28 = -2;
											}
											if (num30 == 57)
											{
												num30 = -2;
											}
											if (num33 == 57)
											{
												num33 = -2;
											}
											if (num35 == 57)
											{
												num35 = -2;
											}
										}
										else
										{
											if (num == 59)
											{
												if ((double)j > Main.rockLayer)
												{
													if (num29 == 1)
													{
														num29 = -2;
													}
													if (num34 == 1)
													{
														num34 = -2;
													}
													if (num31 == 1)
													{
														num31 = -2;
													}
													if (num32 == 1)
													{
														num32 = -2;
													}
													if (num28 == 1)
													{
														num28 = -2;
													}
													if (num30 == 1)
													{
														num30 = -2;
													}
													if (num33 == 1)
													{
														num33 = -2;
													}
													if (num35 == 1)
													{
														num35 = -2;
													}
												}
												if (num29 == 60)
												{
													num29 = num;
												}
												if (num34 == 60)
												{
													num34 = num;
												}
												if (num31 == 60)
												{
													num31 = num;
												}
												if (num32 == 60)
												{
													num32 = num;
												}
												if (num28 == 60)
												{
													num28 = num;
												}
												if (num30 == 60)
												{
													num30 = num;
												}
												if (num33 == 60)
												{
													num33 = num;
												}
												if (num35 == 60)
												{
													num35 = num;
												}
												if (num29 == 70)
												{
													num29 = num;
												}
												if (num34 == 70)
												{
													num34 = num;
												}
												if (num31 == 70)
												{
													num31 = num;
												}
												if (num32 == 70)
												{
													num32 = num;
												}
												if (num28 == 70)
												{
													num28 = num;
												}
												if (num30 == 70)
												{
													num30 = num;
												}
												if (num33 == 70)
												{
													num33 = num;
												}
												if (num35 == 70)
												{
													num35 = num;
												}
												if ((double)j < Main.rockLayer)
												{
													if (num29 == 0)
													{
														WorldGen.TileFrame(i, j - 1, false, false);
														if (WorldGen.mergeDown)
														{
															num29 = num;
														}
													}
													if (num34 == 0)
													{
														WorldGen.TileFrame(i, j + 1, false, false);
														if (WorldGen.mergeUp)
														{
															num34 = num;
														}
													}
													if (num31 == 0)
													{
														WorldGen.TileFrame(i - 1, j, false, false);
														if (WorldGen.mergeRight)
														{
															num31 = num;
														}
													}
													if (num32 == 0)
													{
														WorldGen.TileFrame(i + 1, j, false, false);
														if (WorldGen.mergeLeft)
														{
															num32 = num;
														}
													}
													if (num28 == 0)
													{
														num28 = num;
													}
													if (num30 == 0)
													{
														num30 = num;
													}
													if (num33 == 0)
													{
														num33 = num;
													}
													if (num35 == 0)
													{
														num35 = num;
													}
												}
											}
											else
											{
												if (num == 57)
												{
													if (num29 == 1)
													{
														num29 = -2;
													}
													if (num34 == 1)
													{
														num34 = -2;
													}
													if (num31 == 1)
													{
														num31 = -2;
													}
													if (num32 == 1)
													{
														num32 = -2;
													}
													if (num28 == 1)
													{
														num28 = -2;
													}
													if (num30 == 1)
													{
														num30 = -2;
													}
													if (num33 == 1)
													{
														num33 = -2;
													}
													if (num35 == 1)
													{
														num35 = -2;
													}
													if (num29 == 58 || num29 == 76 || num29 == 75)
													{
														WorldGen.TileFrame(i, j - 1, false, false);
														if (WorldGen.mergeDown)
														{
															num29 = num;
														}
													}
													if (num34 == 58 || num34 == 76 || num34 == 75)
													{
														WorldGen.TileFrame(i, j + 1, false, false);
														if (WorldGen.mergeUp)
														{
															num34 = num;
														}
													}
													if (num31 == 58 || num31 == 76 || num31 == 75)
													{
														WorldGen.TileFrame(i - 1, j, false, false);
														if (WorldGen.mergeRight)
														{
															num31 = num;
														}
													}
													if (num32 == 58 || num32 == 76 || num32 == 75)
													{
														WorldGen.TileFrame(i + 1, j, false, false);
														if (WorldGen.mergeLeft)
														{
															num32 = num;
														}
													}
													if (num28 == 58 || num28 == 76 || num28 == 75)
													{
														num28 = num;
													}
													if (num30 == 58 || num30 == 76 || num30 == 75)
													{
														num30 = num;
													}
													if (num33 == 58 || num33 == 76 || num33 == 75)
													{
														num33 = num;
													}
													if (num35 == 58 || num35 == 76 || num35 == 75)
													{
														num35 = num;
													}
												}
												else
												{
													if (num == 32)
													{
														if (num34 == 23)
														{
															num34 = num;
														}
													}
													else
													{
														if (num == 69)
														{
															if (num34 == 60)
															{
																num34 = num;
															}
														}
														else
														{
															if (num == 51)
															{
																if (num29 > -1 && !Main.tileNoAttach[num29])
																{
																	num29 = num;
																}
																if (num34 > -1 && !Main.tileNoAttach[num34])
																{
																	num34 = num;
																}
																if (num31 > -1 && !Main.tileNoAttach[num31])
																{
																	num31 = num;
																}
																if (num32 > -1 && !Main.tileNoAttach[num32])
																{
																	num32 = num;
																}
																if (num28 > -1 && !Main.tileNoAttach[num28])
																{
																	num28 = num;
																}
																if (num30 > -1 && !Main.tileNoAttach[num30])
																{
																	num30 = num;
																}
																if (num33 > -1 && !Main.tileNoAttach[num33])
																{
																	num33 = num;
																}
																if (num35 > -1 && !Main.tileNoAttach[num35])
																{
																	num35 = num;
																}
															}
														}
													}
												}
											}
										}
									}
								}
								bool flag3 = false;
								if (num == 2 || num == 23 || num == 60 || num == 70 || num == 109)
								{
									flag3 = true;
									if (num29 > -1 && !Main.tileSolid[num29] && num29 != num)
									{
										num29 = -1;
									}
									if (num34 > -1 && !Main.tileSolid[num34] && num34 != num)
									{
										num34 = -1;
									}
									if (num31 > -1 && !Main.tileSolid[num31] && num31 != num)
									{
										num31 = -1;
									}
									if (num32 > -1 && !Main.tileSolid[num32] && num32 != num)
									{
										num32 = -1;
									}
									if (num28 > -1 && !Main.tileSolid[num28] && num28 != num)
									{
										num28 = -1;
									}
									if (num30 > -1 && !Main.tileSolid[num30] && num30 != num)
									{
										num30 = -1;
									}
									if (num33 > -1 && !Main.tileSolid[num33] && num33 != num)
									{
										num33 = -1;
									}
									if (num35 > -1 && !Main.tileSolid[num35] && num35 != num)
									{
										num35 = -1;
									}
									int num37 = 0;
									if (num == 60 || num == 70)
									{
										num37 = 59;
									}
									else
									{
										if (num == 2)
										{
											if (num29 == 23)
											{
												num29 = num37;
											}
											if (num34 == 23)
											{
												num34 = num37;
											}
											if (num31 == 23)
											{
												num31 = num37;
											}
											if (num32 == 23)
											{
												num32 = num37;
											}
											if (num28 == 23)
											{
												num28 = num37;
											}
											if (num30 == 23)
											{
												num30 = num37;
											}
											if (num33 == 23)
											{
												num33 = num37;
											}
											if (num35 == 23)
											{
												num35 = num37;
											}
										}
										else
										{
											if (num == 23)
											{
												if (num29 == 2)
												{
													num29 = num37;
												}
												if (num34 == 2)
												{
													num34 = num37;
												}
												if (num31 == 2)
												{
													num31 = num37;
												}
												if (num32 == 2)
												{
													num32 = num37;
												}
												if (num28 == 2)
												{
													num28 = num37;
												}
												if (num30 == 2)
												{
													num30 = num37;
												}
												if (num33 == 2)
												{
													num33 = num37;
												}
												if (num35 == 2)
												{
													num35 = num37;
												}
											}
										}
									}
									if (num29 != num && num29 != num37 && (num34 == num || num34 == num37))
									{
										if (num31 == num37 && num32 == num)
										{
											if (num36 == 0)
											{
												rectangle.X = 0;
												rectangle.Y = 198;
											}
											else
											{
												if (num36 == 1)
												{
													rectangle.X = 18;
													rectangle.Y = 198;
												}
												else
												{
													rectangle.X = 36;
													rectangle.Y = 198;
												}
											}
										}
										else
										{
											if (num31 == num && num32 == num37)
											{
												if (num36 == 0)
												{
													rectangle.X = 54;
													rectangle.Y = 198;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 72;
														rectangle.Y = 198;
													}
													else
													{
														rectangle.X = 90;
														rectangle.Y = 198;
													}
												}
											}
										}
									}
									else
									{
										if (num34 != num && num34 != num37 && (num29 == num || num29 == num37))
										{
											if (num31 == num37 && num32 == num)
											{
												if (num36 == 0)
												{
													rectangle.X = 0;
													rectangle.Y = 216;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 18;
														rectangle.Y = 216;
													}
													else
													{
														rectangle.X = 36;
														rectangle.Y = 216;
													}
												}
											}
											else
											{
												if (num31 == num && num32 == num37)
												{
													if (num36 == 0)
													{
														rectangle.X = 54;
														rectangle.Y = 216;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 72;
															rectangle.Y = 216;
														}
														else
														{
															rectangle.X = 90;
															rectangle.Y = 216;
														}
													}
												}
											}
										}
										else
										{
											if (num31 != num && num31 != num37 && (num32 == num || num32 == num37))
											{
												if (num29 == num37 && num34 == num)
												{
													if (num36 == 0)
													{
														rectangle.X = 72;
														rectangle.Y = 144;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 72;
															rectangle.Y = 162;
														}
														else
														{
															rectangle.X = 72;
															rectangle.Y = 180;
														}
													}
												}
												else
												{
													if (num34 == num && num32 == num29)
													{
														if (num36 == 0)
														{
															rectangle.X = 72;
															rectangle.Y = 90;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 72;
																rectangle.Y = 108;
															}
															else
															{
																rectangle.X = 72;
																rectangle.Y = 126;
															}
														}
													}
												}
											}
											else
											{
												if (num32 != num && num32 != num37 && (num31 == num || num31 == num37))
												{
													if (num29 == num37 && num34 == num)
													{
														if (num36 == 0)
														{
															rectangle.X = 90;
															rectangle.Y = 144;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 90;
																rectangle.Y = 162;
															}
															else
															{
																rectangle.X = 90;
																rectangle.Y = 180;
															}
														}
													}
													else
													{
														if (num34 == num && num32 == num29)
														{
															if (num36 == 0)
															{
																rectangle.X = 90;
																rectangle.Y = 90;
															}
															else
															{
																if (num36 == 1)
																{
																	rectangle.X = 90;
																	rectangle.Y = 108;
																}
																else
																{
																	rectangle.X = 90;
																	rectangle.Y = 126;
																}
															}
														}
													}
												}
												else
												{
													if (num29 == num && num34 == num && num31 == num && num32 == num)
													{
														if (num28 != num && num30 != num && num33 != num && num35 != num)
														{
															if (num35 == num37)
															{
																if (num36 == 0)
																{
																	rectangle.X = 108;
																	rectangle.Y = 324;
																}
																else
																{
																	if (num36 == 1)
																	{
																		rectangle.X = 126;
																		rectangle.Y = 324;
																	}
																	else
																	{
																		rectangle.X = 144;
																		rectangle.Y = 324;
																	}
																}
															}
															else
															{
																if (num30 == num37)
																{
																	if (num36 == 0)
																	{
																		rectangle.X = 108;
																		rectangle.Y = 342;
																	}
																	else
																	{
																		if (num36 == 1)
																		{
																			rectangle.X = 126;
																			rectangle.Y = 342;
																		}
																		else
																		{
																			rectangle.X = 144;
																			rectangle.Y = 342;
																		}
																	}
																}
																else
																{
																	if (num33 == num37)
																	{
																		if (num36 == 0)
																		{
																			rectangle.X = 108;
																			rectangle.Y = 360;
																		}
																		else
																		{
																			if (num36 == 1)
																			{
																				rectangle.X = 126;
																				rectangle.Y = 360;
																			}
																			else
																			{
																				rectangle.X = 144;
																				rectangle.Y = 360;
																			}
																		}
																	}
																	else
																	{
																		if (num28 == num37)
																		{
																			if (num36 == 0)
																			{
																				rectangle.X = 108;
																				rectangle.Y = 378;
																			}
																			else
																			{
																				if (num36 == 1)
																				{
																					rectangle.X = 126;
																					rectangle.Y = 378;
																				}
																				else
																				{
																					rectangle.X = 144;
																					rectangle.Y = 378;
																				}
																			}
																		}
																		else
																		{
																			if (num36 == 0)
																			{
																				rectangle.X = 144;
																				rectangle.Y = 234;
																			}
																			else
																			{
																				if (num36 == 1)
																				{
																					rectangle.X = 198;
																					rectangle.Y = 234;
																				}
																				else
																				{
																					rectangle.X = 252;
																					rectangle.Y = 234;
																				}
																			}
																		}
																	}
																}
															}
														}
														else
														{
															if (num28 != num && num35 != num)
															{
																if (num36 == 0)
																{
																	rectangle.X = 36;
																	rectangle.Y = 306;
																}
																else
																{
																	if (num36 == 1)
																	{
																		rectangle.X = 54;
																		rectangle.Y = 306;
																	}
																	else
																	{
																		rectangle.X = 72;
																		rectangle.Y = 306;
																	}
																}
															}
															else
															{
																if (num30 != num && num33 != num)
																{
																	if (num36 == 0)
																	{
																		rectangle.X = 90;
																		rectangle.Y = 306;
																	}
																	else
																	{
																		if (num36 == 1)
																		{
																			rectangle.X = 108;
																			rectangle.Y = 306;
																		}
																		else
																		{
																			rectangle.X = 126;
																			rectangle.Y = 306;
																		}
																	}
																}
																else
																{
																	if (num28 != num && num30 == num && num33 == num && num35 == num)
																	{
																		if (num36 == 0)
																		{
																			rectangle.X = 54;
																			rectangle.Y = 108;
																		}
																		else
																		{
																			if (num36 == 1)
																			{
																				rectangle.X = 54;
																				rectangle.Y = 144;
																			}
																			else
																			{
																				rectangle.X = 54;
																				rectangle.Y = 180;
																			}
																		}
																	}
																	else
																	{
																		if (num28 == num && num30 != num && num33 == num && num35 == num)
																		{
																			if (num36 == 0)
																			{
																				rectangle.X = 36;
																				rectangle.Y = 108;
																			}
																			else
																			{
																				if (num36 == 1)
																				{
																					rectangle.X = 36;
																					rectangle.Y = 144;
																				}
																				else
																				{
																					rectangle.X = 36;
																					rectangle.Y = 180;
																				}
																			}
																		}
																		else
																		{
																			if (num28 == num && num30 == num && num33 != num && num35 == num)
																			{
																				if (num36 == 0)
																				{
																					rectangle.X = 54;
																					rectangle.Y = 90;
																				}
																				else
																				{
																					if (num36 == 1)
																					{
																						rectangle.X = 54;
																						rectangle.Y = 126;
																					}
																					else
																					{
																						rectangle.X = 54;
																						rectangle.Y = 162;
																					}
																				}
																			}
																			else
																			{
																				if (num28 == num && num30 == num && num33 == num && num35 != num)
																				{
																					if (num36 == 0)
																					{
																						rectangle.X = 36;
																						rectangle.Y = 90;
																					}
																					else
																					{
																						if (num36 == 1)
																						{
																							rectangle.X = 36;
																							rectangle.Y = 126;
																						}
																						else
																						{
																							rectangle.X = 36;
																							rectangle.Y = 162;
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
													else
													{
														if (num29 == num && num34 == num37 && num31 == num && num32 == num && num28 == -1 && num30 == -1)
														{
															if (num36 == 0)
															{
																rectangle.X = 108;
																rectangle.Y = 18;
															}
															else
															{
																if (num36 == 1)
																{
																	rectangle.X = 126;
																	rectangle.Y = 18;
																}
																else
																{
																	rectangle.X = 144;
																	rectangle.Y = 18;
																}
															}
														}
														else
														{
															if (num29 == num37 && num34 == num && num31 == num && num32 == num && num33 == -1 && num35 == -1)
															{
																if (num36 == 0)
																{
																	rectangle.X = 108;
																	rectangle.Y = 36;
																}
																else
																{
																	if (num36 == 1)
																	{
																		rectangle.X = 126;
																		rectangle.Y = 36;
																	}
																	else
																	{
																		rectangle.X = 144;
																		rectangle.Y = 36;
																	}
																}
															}
															else
															{
																if (num29 == num && num34 == num && num31 == num37 && num32 == num && num30 == -1 && num35 == -1)
																{
																	if (num36 == 0)
																	{
																		rectangle.X = 198;
																		rectangle.Y = 0;
																	}
																	else
																	{
																		if (num36 == 1)
																		{
																			rectangle.X = 198;
																			rectangle.Y = 18;
																		}
																		else
																		{
																			rectangle.X = 198;
																			rectangle.Y = 36;
																		}
																	}
																}
																else
																{
																	if (num29 == num && num34 == num && num31 == num && num32 == num37 && num28 == -1 && num33 == -1)
																	{
																		if (num36 == 0)
																		{
																			rectangle.X = 180;
																			rectangle.Y = 0;
																		}
																		else
																		{
																			if (num36 == 1)
																			{
																				rectangle.X = 180;
																				rectangle.Y = 18;
																			}
																			else
																			{
																				rectangle.X = 180;
																				rectangle.Y = 36;
																			}
																		}
																	}
																	else
																	{
																		if (num29 == num && num34 == num37 && num31 == num && num32 == num)
																		{
																			if (num30 != -1)
																			{
																				if (num36 == 0)
																				{
																					rectangle.X = 54;
																					rectangle.Y = 108;
																				}
																				else
																				{
																					if (num36 == 1)
																					{
																						rectangle.X = 54;
																						rectangle.Y = 144;
																					}
																					else
																					{
																						rectangle.X = 54;
																						rectangle.Y = 180;
																					}
																				}
																			}
																			else
																			{
																				if (num28 != -1)
																				{
																					if (num36 == 0)
																					{
																						rectangle.X = 36;
																						rectangle.Y = 108;
																					}
																					else
																					{
																						if (num36 == 1)
																						{
																							rectangle.X = 36;
																							rectangle.Y = 144;
																						}
																						else
																						{
																							rectangle.X = 36;
																							rectangle.Y = 180;
																						}
																					}
																				}
																			}
																		}
																		else
																		{
																			if (num29 == num37 && num34 == num && num31 == num && num32 == num)
																			{
																				if (num35 != -1)
																				{
																					if (num36 == 0)
																					{
																						rectangle.X = 54;
																						rectangle.Y = 90;
																					}
																					else
																					{
																						if (num36 == 1)
																						{
																							rectangle.X = 54;
																							rectangle.Y = 126;
																						}
																						else
																						{
																							rectangle.X = 54;
																							rectangle.Y = 162;
																						}
																					}
																				}
																				else
																				{
																					if (num33 != -1)
																					{
																						if (num36 == 0)
																						{
																							rectangle.X = 36;
																							rectangle.Y = 90;
																						}
																						else
																						{
																							if (num36 == 1)
																							{
																								rectangle.X = 36;
																								rectangle.Y = 126;
																							}
																							else
																							{
																								rectangle.X = 36;
																								rectangle.Y = 162;
																							}
																						}
																					}
																				}
																			}
																			else
																			{
																				if (num29 == num && num34 == num && num31 == num && num32 == num37)
																				{
																					if (num28 != -1)
																					{
																						if (num36 == 0)
																						{
																							rectangle.X = 54;
																							rectangle.Y = 90;
																						}
																						else
																						{
																							if (num36 == 1)
																							{
																								rectangle.X = 54;
																								rectangle.Y = 126;
																							}
																							else
																							{
																								rectangle.X = 54;
																								rectangle.Y = 162;
																							}
																						}
																					}
																					else
																					{
																						if (num33 != -1)
																						{
																							if (num36 == 0)
																							{
																								rectangle.X = 54;
																								rectangle.Y = 108;
																							}
																							else
																							{
																								if (num36 == 1)
																								{
																									rectangle.X = 54;
																									rectangle.Y = 144;
																								}
																								else
																								{
																									rectangle.X = 54;
																									rectangle.Y = 180;
																								}
																							}
																						}
																					}
																				}
																				else
																				{
																					if (num29 == num && num34 == num && num31 == num37 && num32 == num)
																					{
																						if (num30 != -1)
																						{
																							if (num36 == 0)
																							{
																								rectangle.X = 36;
																								rectangle.Y = 90;
																							}
																							else
																							{
																								if (num36 == 1)
																								{
																									rectangle.X = 36;
																									rectangle.Y = 126;
																								}
																								else
																								{
																									rectangle.X = 36;
																									rectangle.Y = 162;
																								}
																							}
																						}
																						else
																						{
																							if (num35 != -1)
																							{
																								if (num36 == 0)
																								{
																									rectangle.X = 36;
																									rectangle.Y = 108;
																								}
																								else
																								{
																									if (num36 == 1)
																									{
																										rectangle.X = 36;
																										rectangle.Y = 144;
																									}
																									else
																									{
																										rectangle.X = 36;
																										rectangle.Y = 180;
																									}
																								}
																							}
																						}
																					}
																					else
																					{
																						if ((num29 == num37 && num34 == num && num31 == num && num32 == num) || (num29 == num && num34 == num37 && num31 == num && num32 == num) || (num29 == num && num34 == num && num31 == num37 && num32 == num) || (num29 == num && num34 == num && num31 == num && num32 == num37))
																						{
																							if (num36 == 0)
																							{
																								rectangle.X = 18;
																								rectangle.Y = 18;
																							}
																							else
																							{
																								if (num36 == 1)
																								{
																									rectangle.X = 36;
																									rectangle.Y = 18;
																								}
																								else
																								{
																									rectangle.X = 54;
																									rectangle.Y = 18;
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
									if ((num29 == num || num29 == num37) && (num34 == num || num34 == num37) && (num31 == num || num31 == num37) && (num32 == num || num32 == num37))
									{
										if (num28 != num && num28 != num37 && (num30 == num || num30 == num37) && (num33 == num || num33 == num37) && (num35 == num || num35 == num37))
										{
											if (num36 == 0)
											{
												rectangle.X = 54;
												rectangle.Y = 108;
											}
											else
											{
												if (num36 == 1)
												{
													rectangle.X = 54;
													rectangle.Y = 144;
												}
												else
												{
													rectangle.X = 54;
													rectangle.Y = 180;
												}
											}
										}
										else
										{
											if (num30 != num && num30 != num37 && (num28 == num || num28 == num37) && (num33 == num || num33 == num37) && (num35 == num || num35 == num37))
											{
												if (num36 == 0)
												{
													rectangle.X = 36;
													rectangle.Y = 108;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 36;
														rectangle.Y = 144;
													}
													else
													{
														rectangle.X = 36;
														rectangle.Y = 180;
													}
												}
											}
											else
											{
												if (num33 != num && num33 != num37 && (num28 == num || num28 == num37) && (num30 == num || num30 == num37) && (num35 == num || num35 == num37))
												{
													if (num36 == 0)
													{
														rectangle.X = 54;
														rectangle.Y = 90;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 54;
															rectangle.Y = 126;
														}
														else
														{
															rectangle.X = 54;
															rectangle.Y = 162;
														}
													}
												}
												else
												{
													if (num35 != num && num35 != num37 && (num28 == num || num28 == num37) && (num33 == num || num33 == num37) && (num30 == num || num30 == num37))
													{
														if (num36 == 0)
														{
															rectangle.X = 36;
															rectangle.Y = 90;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 36;
																rectangle.Y = 126;
															}
															else
															{
																rectangle.X = 36;
																rectangle.Y = 162;
															}
														}
													}
												}
											}
										}
									}
									if (num29 != num37 && num29 != num && num34 == num && num31 != num37 && num31 != num && num32 == num && num35 != num37 && num35 != num)
									{
										if (num36 == 0)
										{
											rectangle.X = 90;
											rectangle.Y = 270;
										}
										else
										{
											if (num36 == 1)
											{
												rectangle.X = 108;
												rectangle.Y = 270;
											}
											else
											{
												rectangle.X = 126;
												rectangle.Y = 270;
											}
										}
									}
									else
									{
										if (num29 != num37 && num29 != num && num34 == num && num31 == num && num32 != num37 && num32 != num && num33 != num37 && num33 != num)
										{
											if (num36 == 0)
											{
												rectangle.X = 144;
												rectangle.Y = 270;
											}
											else
											{
												if (num36 == 1)
												{
													rectangle.X = 162;
													rectangle.Y = 270;
												}
												else
												{
													rectangle.X = 180;
													rectangle.Y = 270;
												}
											}
										}
										else
										{
											if (num34 != num37 && num34 != num && num29 == num && num31 != num37 && num31 != num && num32 == num && num30 != num37 && num30 != num)
											{
												if (num36 == 0)
												{
													rectangle.X = 90;
													rectangle.Y = 288;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 108;
														rectangle.Y = 288;
													}
													else
													{
														rectangle.X = 126;
														rectangle.Y = 288;
													}
												}
											}
											else
											{
												if (num34 != num37 && num34 != num && num29 == num && num31 == num && num32 != num37 && num32 != num && num28 != num37 && num28 != num)
												{
													if (num36 == 0)
													{
														rectangle.X = 144;
														rectangle.Y = 288;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 162;
															rectangle.Y = 288;
														}
														else
														{
															rectangle.X = 180;
															rectangle.Y = 288;
														}
													}
												}
												else
												{
													if (num29 != num && num29 != num37 && num34 == num && num31 == num && num32 == num && num33 != num && num33 != num37 && num35 != num && num35 != num37)
													{
														if (num36 == 0)
														{
															rectangle.X = 144;
															rectangle.Y = 216;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 198;
																rectangle.Y = 216;
															}
															else
															{
																rectangle.X = 252;
																rectangle.Y = 216;
															}
														}
													}
													else
													{
														if (num34 != num && num34 != num37 && num29 == num && num31 == num && num32 == num && num28 != num && num28 != num37 && num30 != num && num30 != num37)
														{
															if (num36 == 0)
															{
																rectangle.X = 144;
																rectangle.Y = 252;
															}
															else
															{
																if (num36 == 1)
																{
																	rectangle.X = 198;
																	rectangle.Y = 252;
																}
																else
																{
																	rectangle.X = 252;
																	rectangle.Y = 252;
																}
															}
														}
														else
														{
															if (num31 != num && num31 != num37 && num34 == num && num29 == num && num32 == num && num30 != num && num30 != num37 && num35 != num && num35 != num37)
															{
																if (num36 == 0)
																{
																	rectangle.X = 126;
																	rectangle.Y = 234;
																}
																else
																{
																	if (num36 == 1)
																	{
																		rectangle.X = 180;
																		rectangle.Y = 234;
																	}
																	else
																	{
																		rectangle.X = 234;
																		rectangle.Y = 234;
																	}
																}
															}
															else
															{
																if (num32 != num && num32 != num37 && num34 == num && num29 == num && num31 == num && num28 != num && num28 != num37 && num33 != num && num33 != num37)
																{
																	if (num36 == 0)
																	{
																		rectangle.X = 162;
																		rectangle.Y = 234;
																	}
																	else
																	{
																		if (num36 == 1)
																		{
																			rectangle.X = 216;
																			rectangle.Y = 234;
																		}
																		else
																		{
																			rectangle.X = 270;
																			rectangle.Y = 234;
																		}
																	}
																}
																else
																{
																	if (num29 != num37 && num29 != num && (num34 == num37 || num34 == num) && num31 == num37 && num32 == num37)
																	{
																		if (num36 == 0)
																		{
																			rectangle.X = 36;
																			rectangle.Y = 270;
																		}
																		else
																		{
																			if (num36 == 1)
																			{
																				rectangle.X = 54;
																				rectangle.Y = 270;
																			}
																			else
																			{
																				rectangle.X = 72;
																				rectangle.Y = 270;
																			}
																		}
																	}
																	else
																	{
																		if (num34 != num37 && num34 != num && (num29 == num37 || num29 == num) && num31 == num37 && num32 == num37)
																		{
																			if (num36 == 0)
																			{
																				rectangle.X = 36;
																				rectangle.Y = 288;
																			}
																			else
																			{
																				if (num36 == 1)
																				{
																					rectangle.X = 54;
																					rectangle.Y = 288;
																				}
																				else
																				{
																					rectangle.X = 72;
																					rectangle.Y = 288;
																				}
																			}
																		}
																		else
																		{
																			if (num31 != num37 && num31 != num && (num32 == num37 || num32 == num) && num29 == num37 && num34 == num37)
																			{
																				if (num36 == 0)
																				{
																					rectangle.X = 0;
																					rectangle.Y = 270;
																				}
																				else
																				{
																					if (num36 == 1)
																					{
																						rectangle.X = 0;
																						rectangle.Y = 288;
																					}
																					else
																					{
																						rectangle.X = 0;
																						rectangle.Y = 306;
																					}
																				}
																			}
																			else
																			{
																				if (num32 != num37 && num32 != num && (num31 == num37 || num31 == num) && num29 == num37 && num34 == num37)
																				{
																					if (num36 == 0)
																					{
																						rectangle.X = 18;
																						rectangle.Y = 270;
																					}
																					else
																					{
																						if (num36 == 1)
																						{
																							rectangle.X = 18;
																							rectangle.Y = 288;
																						}
																						else
																						{
																							rectangle.X = 18;
																							rectangle.Y = 306;
																						}
																					}
																				}
																				else
																				{
																					if (num29 == num && num34 == num37 && num31 == num37 && num32 == num37)
																					{
																						if (num36 == 0)
																						{
																							rectangle.X = 198;
																							rectangle.Y = 288;
																						}
																						else
																						{
																							if (num36 == 1)
																							{
																								rectangle.X = 216;
																								rectangle.Y = 288;
																							}
																							else
																							{
																								rectangle.X = 234;
																								rectangle.Y = 288;
																							}
																						}
																					}
																					else
																					{
																						if (num29 == num37 && num34 == num && num31 == num37 && num32 == num37)
																						{
																							if (num36 == 0)
																							{
																								rectangle.X = 198;
																								rectangle.Y = 270;
																							}
																							else
																							{
																								if (num36 == 1)
																								{
																									rectangle.X = 216;
																									rectangle.Y = 270;
																								}
																								else
																								{
																									rectangle.X = 234;
																									rectangle.Y = 270;
																								}
																							}
																						}
																						else
																						{
																							if (num29 == num37 && num34 == num37 && num31 == num && num32 == num37)
																							{
																								if (num36 == 0)
																								{
																									rectangle.X = 198;
																									rectangle.Y = 306;
																								}
																								else
																								{
																									if (num36 == 1)
																									{
																										rectangle.X = 216;
																										rectangle.Y = 306;
																									}
																									else
																									{
																										rectangle.X = 234;
																										rectangle.Y = 306;
																									}
																								}
																							}
																							else
																							{
																								if (num29 == num37 && num34 == num37 && num31 == num37 && num32 == num)
																								{
																									if (num36 == 0)
																									{
																										rectangle.X = 144;
																										rectangle.Y = 306;
																									}
																									else
																									{
																										if (num36 == 1)
																										{
																											rectangle.X = 162;
																											rectangle.Y = 306;
																										}
																										else
																										{
																											rectangle.X = 180;
																											rectangle.Y = 306;
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
									if (num29 != num && num29 != num37 && num34 == num && num31 == num && num32 == num)
									{
										if ((num33 == num37 || num33 == num) && num35 != num37 && num35 != num)
										{
											if (num36 == 0)
											{
												rectangle.X = 0;
												rectangle.Y = 324;
											}
											else
											{
												if (num36 == 1)
												{
													rectangle.X = 18;
													rectangle.Y = 324;
												}
												else
												{
													rectangle.X = 36;
													rectangle.Y = 324;
												}
											}
										}
										else
										{
											if ((num35 == num37 || num35 == num) && num33 != num37 && num33 != num)
											{
												if (num36 == 0)
												{
													rectangle.X = 54;
													rectangle.Y = 324;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 72;
														rectangle.Y = 324;
													}
													else
													{
														rectangle.X = 90;
														rectangle.Y = 324;
													}
												}
											}
										}
									}
									else
									{
										if (num34 != num && num34 != num37 && num29 == num && num31 == num && num32 == num)
										{
											if ((num28 == num37 || num28 == num) && num30 != num37 && num30 != num)
											{
												if (num36 == 0)
												{
													rectangle.X = 0;
													rectangle.Y = 342;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 18;
														rectangle.Y = 342;
													}
													else
													{
														rectangle.X = 36;
														rectangle.Y = 342;
													}
												}
											}
											else
											{
												if ((num30 == num37 || num30 == num) && num28 != num37 && num28 != num)
												{
													if (num36 == 0)
													{
														rectangle.X = 54;
														rectangle.Y = 342;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 72;
															rectangle.Y = 342;
														}
														else
														{
															rectangle.X = 90;
															rectangle.Y = 342;
														}
													}
												}
											}
										}
										else
										{
											if (num31 != num && num31 != num37 && num29 == num && num34 == num && num32 == num)
											{
												if ((num30 == num37 || num30 == num) && num35 != num37 && num35 != num)
												{
													if (num36 == 0)
													{
														rectangle.X = 54;
														rectangle.Y = 360;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 72;
															rectangle.Y = 360;
														}
														else
														{
															rectangle.X = 90;
															rectangle.Y = 360;
														}
													}
												}
												else
												{
													if ((num35 == num37 || num35 == num) && num30 != num37 && num30 != num)
													{
														if (num36 == 0)
														{
															rectangle.X = 0;
															rectangle.Y = 360;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 18;
																rectangle.Y = 360;
															}
															else
															{
																rectangle.X = 36;
																rectangle.Y = 360;
															}
														}
													}
												}
											}
											else
											{
												if (num32 != num && num32 != num37 && num29 == num && num34 == num && num31 == num)
												{
													if ((num28 == num37 || num28 == num) && num33 != num37 && num33 != num)
													{
														if (num36 == 0)
														{
															rectangle.X = 0;
															rectangle.Y = 378;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 18;
																rectangle.Y = 378;
															}
															else
															{
																rectangle.X = 36;
																rectangle.Y = 378;
															}
														}
													}
													else
													{
														if ((num33 == num37 || num33 == num) && num28 != num37 && num28 != num)
														{
															if (num36 == 0)
															{
																rectangle.X = 54;
																rectangle.Y = 378;
															}
															else
															{
																if (num36 == 1)
																{
																	rectangle.X = 72;
																	rectangle.Y = 378;
																}
																else
																{
																	rectangle.X = 90;
																	rectangle.Y = 378;
																}
															}
														}
													}
												}
											}
										}
									}
									if ((num29 == num || num29 == num37) && (num34 == num || num34 == num37) && (num31 == num || num31 == num37) && (num32 == num || num32 == num37) && num28 != -1 && num30 != -1 && num33 != -1 && num35 != -1)
									{
										if (num36 == 0)
										{
											rectangle.X = 18;
											rectangle.Y = 18;
										}
										else
										{
											if (num36 == 1)
											{
												rectangle.X = 36;
												rectangle.Y = 18;
											}
											else
											{
												rectangle.X = 54;
												rectangle.Y = 18;
											}
										}
									}
									if (num29 == num37)
									{
										num29 = -2;
									}
									if (num34 == num37)
									{
										num34 = -2;
									}
									if (num31 == num37)
									{
										num31 = -2;
									}
									if (num32 == num37)
									{
										num32 = -2;
									}
									if (num28 == num37)
									{
										num28 = -2;
									}
									if (num30 == num37)
									{
										num30 = -2;
									}
									if (num33 == num37)
									{
										num33 = -2;
									}
									if (num35 == num37)
									{
										num35 = -2;
									}
								}
								if (rectangle.X == -1 && rectangle.Y == -1 && (Main.tileMergeDirt[num] || num == 0 || num == 2 || num == 57 || num == 58 || num == 59 || num == 60 || num == 70 || num == 109 || num == 76 || num == 75))
								{
									if (!flag3)
									{
										flag3 = true;
										if (num29 > -1 && !Main.tileSolid[num29] && num29 != num)
										{
											num29 = -1;
										}
										if (num34 > -1 && !Main.tileSolid[num34] && num34 != num)
										{
											num34 = -1;
										}
										if (num31 > -1 && !Main.tileSolid[num31] && num31 != num)
										{
											num31 = -1;
										}
										if (num32 > -1 && !Main.tileSolid[num32] && num32 != num)
										{
											num32 = -1;
										}
										if (num28 > -1 && !Main.tileSolid[num28] && num28 != num)
										{
											num28 = -1;
										}
										if (num30 > -1 && !Main.tileSolid[num30] && num30 != num)
										{
											num30 = -1;
										}
										if (num33 > -1 && !Main.tileSolid[num33] && num33 != num)
										{
											num33 = -1;
										}
										if (num35 > -1 && !Main.tileSolid[num35] && num35 != num)
										{
											num35 = -1;
										}
									}
									if (num29 >= 0 && num29 != num)
									{
										num29 = -1;
									}
									if (num34 >= 0 && num34 != num)
									{
										num34 = -1;
									}
									if (num31 >= 0 && num31 != num)
									{
										num31 = -1;
									}
									if (num32 >= 0 && num32 != num)
									{
										num32 = -1;
									}
									if (num29 != -1 && num34 != -1 && num31 != -1 && num32 != -1)
									{
										if (num29 == -2 && num34 == num && num31 == num && num32 == num)
										{
											if (num36 == 0)
											{
												rectangle.X = 144;
												rectangle.Y = 108;
											}
											else
											{
												if (num36 == 1)
												{
													rectangle.X = 162;
													rectangle.Y = 108;
												}
												else
												{
													rectangle.X = 180;
													rectangle.Y = 108;
												}
											}
											WorldGen.mergeUp = true;
										}
										else
										{
											if (num29 == num && num34 == -2 && num31 == num && num32 == num)
											{
												if (num36 == 0)
												{
													rectangle.X = 144;
													rectangle.Y = 90;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 162;
														rectangle.Y = 90;
													}
													else
													{
														rectangle.X = 180;
														rectangle.Y = 90;
													}
												}
												WorldGen.mergeDown = true;
											}
											else
											{
												if (num29 == num && num34 == num && num31 == -2 && num32 == num)
												{
													if (num36 == 0)
													{
														rectangle.X = 162;
														rectangle.Y = 126;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 162;
															rectangle.Y = 144;
														}
														else
														{
															rectangle.X = 162;
															rectangle.Y = 162;
														}
													}
													WorldGen.mergeLeft = true;
												}
												else
												{
													if (num29 == num && num34 == num && num31 == num && num32 == -2)
													{
														if (num36 == 0)
														{
															rectangle.X = 144;
															rectangle.Y = 126;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 144;
																rectangle.Y = 144;
															}
															else
															{
																rectangle.X = 144;
																rectangle.Y = 162;
															}
														}
														WorldGen.mergeRight = true;
													}
													else
													{
														if (num29 == -2 && num34 == num && num31 == -2 && num32 == num)
														{
															if (num36 == 0)
															{
																rectangle.X = 36;
																rectangle.Y = 90;
															}
															else
															{
																if (num36 == 1)
																{
																	rectangle.X = 36;
																	rectangle.Y = 126;
																}
																else
																{
																	rectangle.X = 36;
																	rectangle.Y = 162;
																}
															}
															WorldGen.mergeUp = true;
															WorldGen.mergeLeft = true;
														}
														else
														{
															if (num29 == -2 && num34 == num && num31 == num && num32 == -2)
															{
																if (num36 == 0)
																{
																	rectangle.X = 54;
																	rectangle.Y = 90;
																}
																else
																{
																	if (num36 == 1)
																	{
																		rectangle.X = 54;
																		rectangle.Y = 126;
																	}
																	else
																	{
																		rectangle.X = 54;
																		rectangle.Y = 162;
																	}
																}
																WorldGen.mergeUp = true;
																WorldGen.mergeRight = true;
															}
															else
															{
																if (num29 == num && num34 == -2 && num31 == -2 && num32 == num)
																{
																	if (num36 == 0)
																	{
																		rectangle.X = 36;
																		rectangle.Y = 108;
																	}
																	else
																	{
																		if (num36 == 1)
																		{
																			rectangle.X = 36;
																			rectangle.Y = 144;
																		}
																		else
																		{
																			rectangle.X = 36;
																			rectangle.Y = 180;
																		}
																	}
																	WorldGen.mergeDown = true;
																	WorldGen.mergeLeft = true;
																}
																else
																{
																	if (num29 == num && num34 == -2 && num31 == num && num32 == -2)
																	{
																		if (num36 == 0)
																		{
																			rectangle.X = 54;
																			rectangle.Y = 108;
																		}
																		else
																		{
																			if (num36 == 1)
																			{
																				rectangle.X = 54;
																				rectangle.Y = 144;
																			}
																			else
																			{
																				rectangle.X = 54;
																				rectangle.Y = 180;
																			}
																		}
																		WorldGen.mergeDown = true;
																		WorldGen.mergeRight = true;
																	}
																	else
																	{
																		if (num29 == num && num34 == num && num31 == -2 && num32 == -2)
																		{
																			if (num36 == 0)
																			{
																				rectangle.X = 180;
																				rectangle.Y = 126;
																			}
																			else
																			{
																				if (num36 == 1)
																				{
																					rectangle.X = 180;
																					rectangle.Y = 144;
																				}
																				else
																				{
																					rectangle.X = 180;
																					rectangle.Y = 162;
																				}
																			}
																			WorldGen.mergeLeft = true;
																			WorldGen.mergeRight = true;
																		}
																		else
																		{
																			if (num29 == -2 && num34 == -2 && num31 == num && num32 == num)
																			{
																				if (num36 == 0)
																				{
																					rectangle.X = 144;
																					rectangle.Y = 180;
																				}
																				else
																				{
																					if (num36 == 1)
																					{
																						rectangle.X = 162;
																						rectangle.Y = 180;
																					}
																					else
																					{
																						rectangle.X = 180;
																						rectangle.Y = 180;
																					}
																				}
																				WorldGen.mergeUp = true;
																				WorldGen.mergeDown = true;
																			}
																			else
																			{
																				if (num29 == -2 && num34 == num && num31 == -2 && num32 == -2)
																				{
																					if (num36 == 0)
																					{
																						rectangle.X = 198;
																						rectangle.Y = 90;
																					}
																					else
																					{
																						if (num36 == 1)
																						{
																							rectangle.X = 198;
																							rectangle.Y = 108;
																						}
																						else
																						{
																							rectangle.X = 198;
																							rectangle.Y = 126;
																						}
																					}
																					WorldGen.mergeUp = true;
																					WorldGen.mergeLeft = true;
																					WorldGen.mergeRight = true;
																				}
																				else
																				{
																					if (num29 == num && num34 == -2 && num31 == -2 && num32 == -2)
																					{
																						if (num36 == 0)
																						{
																							rectangle.X = 198;
																							rectangle.Y = 144;
																						}
																						else
																						{
																							if (num36 == 1)
																							{
																								rectangle.X = 198;
																								rectangle.Y = 162;
																							}
																							else
																							{
																								rectangle.X = 198;
																								rectangle.Y = 180;
																							}
																						}
																						WorldGen.mergeDown = true;
																						WorldGen.mergeLeft = true;
																						WorldGen.mergeRight = true;
																					}
																					else
																					{
																						if (num29 == -2 && num34 == -2 && num31 == num && num32 == -2)
																						{
																							if (num36 == 0)
																							{
																								rectangle.X = 216;
																								rectangle.Y = 144;
																							}
																							else
																							{
																								if (num36 == 1)
																								{
																									rectangle.X = 216;
																									rectangle.Y = 162;
																								}
																								else
																								{
																									rectangle.X = 216;
																									rectangle.Y = 180;
																								}
																							}
																							WorldGen.mergeUp = true;
																							WorldGen.mergeDown = true;
																							WorldGen.mergeRight = true;
																						}
																						else
																						{
																							if (num29 == -2 && num34 == -2 && num31 == -2 && num32 == num)
																							{
																								if (num36 == 0)
																								{
																									rectangle.X = 216;
																									rectangle.Y = 90;
																								}
																								else
																								{
																									if (num36 == 1)
																									{
																										rectangle.X = 216;
																										rectangle.Y = 108;
																									}
																									else
																									{
																										rectangle.X = 216;
																										rectangle.Y = 126;
																									}
																								}
																								WorldGen.mergeUp = true;
																								WorldGen.mergeDown = true;
																								WorldGen.mergeLeft = true;
																							}
																							else
																							{
																								if (num29 == -2 && num34 == -2 && num31 == -2 && num32 == -2)
																								{
																									if (num36 == 0)
																									{
																										rectangle.X = 108;
																										rectangle.Y = 198;
																									}
																									else
																									{
																										if (num36 == 1)
																										{
																											rectangle.X = 126;
																											rectangle.Y = 198;
																										}
																										else
																										{
																											rectangle.X = 144;
																											rectangle.Y = 198;
																										}
																									}
																									WorldGen.mergeUp = true;
																									WorldGen.mergeDown = true;
																									WorldGen.mergeLeft = true;
																									WorldGen.mergeRight = true;
																								}
																								else
																								{
																									if (num29 == num && num34 == num && num31 == num && num32 == num)
																									{
																										if (num28 == -2)
																										{
																											if (num36 == 0)
																											{
																												rectangle.X = 18;
																												rectangle.Y = 108;
																											}
																											else
																											{
																												if (num36 == 1)
																												{
																													rectangle.X = 18;
																													rectangle.Y = 144;
																												}
																												else
																												{
																													rectangle.X = 18;
																													rectangle.Y = 180;
																												}
																											}
																										}
																										if (num30 == -2)
																										{
																											if (num36 == 0)
																											{
																												rectangle.X = 0;
																												rectangle.Y = 108;
																											}
																											else
																											{
																												if (num36 == 1)
																												{
																													rectangle.X = 0;
																													rectangle.Y = 144;
																												}
																												else
																												{
																													rectangle.X = 0;
																													rectangle.Y = 180;
																												}
																											}
																										}
																										if (num33 == -2)
																										{
																											if (num36 == 0)
																											{
																												rectangle.X = 18;
																												rectangle.Y = 90;
																											}
																											else
																											{
																												if (num36 == 1)
																												{
																													rectangle.X = 18;
																													rectangle.Y = 126;
																												}
																												else
																												{
																													rectangle.X = 18;
																													rectangle.Y = 162;
																												}
																											}
																										}
																										if (num35 == -2)
																										{
																											if (num36 == 0)
																											{
																												rectangle.X = 0;
																												rectangle.Y = 90;
																											}
																											else
																											{
																												if (num36 == 1)
																												{
																													rectangle.X = 0;
																													rectangle.Y = 126;
																												}
																												else
																												{
																													rectangle.X = 0;
																													rectangle.Y = 162;
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
									else
									{
										if (num != 2 && num != 23 && num != 60 && num != 70 && num != 109)
										{
											if (num29 == -1 && num34 == -2 && num31 == num && num32 == num)
											{
												if (num36 == 0)
												{
													rectangle.X = 234;
													rectangle.Y = 0;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 252;
														rectangle.Y = 0;
													}
													else
													{
														rectangle.X = 270;
														rectangle.Y = 0;
													}
												}
												WorldGen.mergeDown = true;
											}
											else
											{
												if (num29 == -2 && num34 == -1 && num31 == num && num32 == num)
												{
													if (num36 == 0)
													{
														rectangle.X = 234;
														rectangle.Y = 18;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 252;
															rectangle.Y = 18;
														}
														else
														{
															rectangle.X = 270;
															rectangle.Y = 18;
														}
													}
													WorldGen.mergeUp = true;
												}
												else
												{
													if (num29 == num && num34 == num && num31 == -1 && num32 == -2)
													{
														if (num36 == 0)
														{
															rectangle.X = 234;
															rectangle.Y = 36;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 252;
																rectangle.Y = 36;
															}
															else
															{
																rectangle.X = 270;
																rectangle.Y = 36;
															}
														}
														WorldGen.mergeRight = true;
													}
													else
													{
														if (num29 == num && num34 == num && num31 == -2 && num32 == -1)
														{
															if (num36 == 0)
															{
																rectangle.X = 234;
																rectangle.Y = 54;
															}
															else
															{
																if (num36 == 1)
																{
																	rectangle.X = 252;
																	rectangle.Y = 54;
																}
																else
																{
																	rectangle.X = 270;
																	rectangle.Y = 54;
																}
															}
															WorldGen.mergeLeft = true;
														}
													}
												}
											}
										}
										if (num29 != -1 && num34 != -1 && num31 == -1 && num32 == num)
										{
											if (num29 == -2 && num34 == num)
											{
												if (num36 == 0)
												{
													rectangle.X = 72;
													rectangle.Y = 144;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 72;
														rectangle.Y = 162;
													}
													else
													{
														rectangle.X = 72;
														rectangle.Y = 180;
													}
												}
												WorldGen.mergeUp = true;
											}
											else
											{
												if (num34 == -2 && num29 == num)
												{
													if (num36 == 0)
													{
														rectangle.X = 72;
														rectangle.Y = 90;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 72;
															rectangle.Y = 108;
														}
														else
														{
															rectangle.X = 72;
															rectangle.Y = 126;
														}
													}
													WorldGen.mergeDown = true;
												}
											}
										}
										else
										{
											if (num29 != -1 && num34 != -1 && num31 == num && num32 == -1)
											{
												if (num29 == -2 && num34 == num)
												{
													if (num36 == 0)
													{
														rectangle.X = 90;
														rectangle.Y = 144;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 90;
															rectangle.Y = 162;
														}
														else
														{
															rectangle.X = 90;
															rectangle.Y = 180;
														}
													}
													WorldGen.mergeUp = true;
												}
												else
												{
													if (num34 == -2 && num29 == num)
													{
														if (num36 == 0)
														{
															rectangle.X = 90;
															rectangle.Y = 90;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 90;
																rectangle.Y = 108;
															}
															else
															{
																rectangle.X = 90;
																rectangle.Y = 126;
															}
														}
														WorldGen.mergeDown = true;
													}
												}
											}
											else
											{
												if (num29 == -1 && num34 == num && num31 != -1 && num32 != -1)
												{
													if (num31 == -2 && num32 == num)
													{
														if (num36 == 0)
														{
															rectangle.X = 0;
															rectangle.Y = 198;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 18;
																rectangle.Y = 198;
															}
															else
															{
																rectangle.X = 36;
																rectangle.Y = 198;
															}
														}
														WorldGen.mergeLeft = true;
													}
													else
													{
														if (num32 == -2 && num31 == num)
														{
															if (num36 == 0)
															{
																rectangle.X = 54;
																rectangle.Y = 198;
															}
															else
															{
																if (num36 == 1)
																{
																	rectangle.X = 72;
																	rectangle.Y = 198;
																}
																else
																{
																	rectangle.X = 90;
																	rectangle.Y = 198;
																}
															}
															WorldGen.mergeRight = true;
														}
													}
												}
												else
												{
													if (num29 == num && num34 == -1 && num31 != -1 && num32 != -1)
													{
														if (num31 == -2 && num32 == num)
														{
															if (num36 == 0)
															{
																rectangle.X = 0;
																rectangle.Y = 216;
															}
															else
															{
																if (num36 == 1)
																{
																	rectangle.X = 18;
																	rectangle.Y = 216;
																}
																else
																{
																	rectangle.X = 36;
																	rectangle.Y = 216;
																}
															}
															WorldGen.mergeLeft = true;
														}
														else
														{
															if (num32 == -2 && num31 == num)
															{
																if (num36 == 0)
																{
																	rectangle.X = 54;
																	rectangle.Y = 216;
																}
																else
																{
																	if (num36 == 1)
																	{
																		rectangle.X = 72;
																		rectangle.Y = 216;
																	}
																	else
																	{
																		rectangle.X = 90;
																		rectangle.Y = 216;
																	}
																}
																WorldGen.mergeRight = true;
															}
														}
													}
													else
													{
														if (num29 != -1 && num34 != -1 && num31 == -1 && num32 == -1)
														{
															if (num29 == -2 && num34 == -2)
															{
																if (num36 == 0)
																{
																	rectangle.X = 108;
																	rectangle.Y = 216;
																}
																else
																{
																	if (num36 == 1)
																	{
																		rectangle.X = 108;
																		rectangle.Y = 234;
																	}
																	else
																	{
																		rectangle.X = 108;
																		rectangle.Y = 252;
																	}
																}
																WorldGen.mergeUp = true;
																WorldGen.mergeDown = true;
															}
															else
															{
																if (num29 == -2)
																{
																	if (num36 == 0)
																	{
																		rectangle.X = 126;
																		rectangle.Y = 144;
																	}
																	else
																	{
																		if (num36 == 1)
																		{
																			rectangle.X = 126;
																			rectangle.Y = 162;
																		}
																		else
																		{
																			rectangle.X = 126;
																			rectangle.Y = 180;
																		}
																	}
																	WorldGen.mergeUp = true;
																}
																else
																{
																	if (num34 == -2)
																	{
																		if (num36 == 0)
																		{
																			rectangle.X = 126;
																			rectangle.Y = 90;
																		}
																		else
																		{
																			if (num36 == 1)
																			{
																				rectangle.X = 126;
																				rectangle.Y = 108;
																			}
																			else
																			{
																				rectangle.X = 126;
																				rectangle.Y = 126;
																			}
																		}
																		WorldGen.mergeDown = true;
																	}
																}
															}
														}
														else
														{
															if (num29 == -1 && num34 == -1 && num31 != -1 && num32 != -1)
															{
																if (num31 == -2 && num32 == -2)
																{
																	if (num36 == 0)
																	{
																		rectangle.X = 162;
																		rectangle.Y = 198;
																	}
																	else
																	{
																		if (num36 == 1)
																		{
																			rectangle.X = 180;
																			rectangle.Y = 198;
																		}
																		else
																		{
																			rectangle.X = 198;
																			rectangle.Y = 198;
																		}
																	}
																	WorldGen.mergeLeft = true;
																	WorldGen.mergeRight = true;
																}
																else
																{
																	if (num31 == -2)
																	{
																		if (num36 == 0)
																		{
																			rectangle.X = 0;
																			rectangle.Y = 252;
																		}
																		else
																		{
																			if (num36 == 1)
																			{
																				rectangle.X = 18;
																				rectangle.Y = 252;
																			}
																			else
																			{
																				rectangle.X = 36;
																				rectangle.Y = 252;
																			}
																		}
																		WorldGen.mergeLeft = true;
																	}
																	else
																	{
																		if (num32 == -2)
																		{
																			if (num36 == 0)
																			{
																				rectangle.X = 54;
																				rectangle.Y = 252;
																			}
																			else
																			{
																				if (num36 == 1)
																				{
																					rectangle.X = 72;
																					rectangle.Y = 252;
																				}
																				else
																				{
																					rectangle.X = 90;
																					rectangle.Y = 252;
																				}
																			}
																			WorldGen.mergeRight = true;
																		}
																	}
																}
															}
															else
															{
																if (num29 == -2 && num34 == -1 && num31 == -1 && num32 == -1)
																{
																	if (num36 == 0)
																	{
																		rectangle.X = 108;
																		rectangle.Y = 144;
																	}
																	else
																	{
																		if (num36 == 1)
																		{
																			rectangle.X = 108;
																			rectangle.Y = 162;
																		}
																		else
																		{
																			rectangle.X = 108;
																			rectangle.Y = 180;
																		}
																	}
																	WorldGen.mergeUp = true;
																}
																else
																{
																	if (num29 == -1 && num34 == -2 && num31 == -1 && num32 == -1)
																	{
																		if (num36 == 0)
																		{
																			rectangle.X = 108;
																			rectangle.Y = 90;
																		}
																		else
																		{
																			if (num36 == 1)
																			{
																				rectangle.X = 108;
																				rectangle.Y = 108;
																			}
																			else
																			{
																				rectangle.X = 108;
																				rectangle.Y = 126;
																			}
																		}
																		WorldGen.mergeDown = true;
																	}
																	else
																	{
																		if (num29 == -1 && num34 == -1 && num31 == -2 && num32 == -1)
																		{
																			if (num36 == 0)
																			{
																				rectangle.X = 0;
																				rectangle.Y = 234;
																			}
																			else
																			{
																				if (num36 == 1)
																				{
																					rectangle.X = 18;
																					rectangle.Y = 234;
																				}
																				else
																				{
																					rectangle.X = 36;
																					rectangle.Y = 234;
																				}
																			}
																			WorldGen.mergeLeft = true;
																		}
																		else
																		{
																			if (num29 == -1 && num34 == -1 && num31 == -1 && num32 == -2)
																			{
																				if (num36 == 0)
																				{
																					rectangle.X = 54;
																					rectangle.Y = 234;
																				}
																				else
																				{
																					if (num36 == 1)
																					{
																						rectangle.X = 72;
																						rectangle.Y = 234;
																					}
																					else
																					{
																						rectangle.X = 90;
																						rectangle.Y = 234;
																					}
																				}
																				WorldGen.mergeRight = true;
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
								if (rectangle.X < 0 || rectangle.Y < 0)
								{
									if (!flag3)
									{
										flag3 = true;
										if (num29 > -1 && !Main.tileSolid[num29] && num29 != num)
										{
											num29 = -1;
										}
										if (num34 > -1 && !Main.tileSolid[num34] && num34 != num)
										{
											num34 = -1;
										}
										if (num31 > -1 && !Main.tileSolid[num31] && num31 != num)
										{
											num31 = -1;
										}
										if (num32 > -1 && !Main.tileSolid[num32] && num32 != num)
										{
											num32 = -1;
										}
										if (num28 > -1 && !Main.tileSolid[num28] && num28 != num)
										{
											num28 = -1;
										}
										if (num30 > -1 && !Main.tileSolid[num30] && num30 != num)
										{
											num30 = -1;
										}
										if (num33 > -1 && !Main.tileSolid[num33] && num33 != num)
										{
											num33 = -1;
										}
										if (num35 > -1 && !Main.tileSolid[num35] && num35 != num)
										{
											num35 = -1;
										}
									}
									if (num == 2 || num == 23 || num == 60 || num == 70 || num == 109)
									{
										if (num29 == -2)
										{
											num29 = num;
										}
										if (num34 == -2)
										{
											num34 = num;
										}
										if (num31 == -2)
										{
											num31 = num;
										}
										if (num32 == -2)
										{
											num32 = num;
										}
										if (num28 == -2)
										{
											num28 = num;
										}
										if (num30 == -2)
										{
											num30 = num;
										}
										if (num33 == -2)
										{
											num33 = num;
										}
										if (num35 == -2)
										{
											num35 = num;
										}
									}
									if (num29 == num && num34 == num && (num31 == num & num32 == num))
									{
										if (num28 != num && num30 != num)
										{
											if (num36 == 0)
											{
												rectangle.X = 108;
												rectangle.Y = 18;
											}
											else
											{
												if (num36 == 1)
												{
													rectangle.X = 126;
													rectangle.Y = 18;
												}
												else
												{
													rectangle.X = 144;
													rectangle.Y = 18;
												}
											}
										}
										else
										{
											if (num33 != num && num35 != num)
											{
												if (num36 == 0)
												{
													rectangle.X = 108;
													rectangle.Y = 36;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 126;
														rectangle.Y = 36;
													}
													else
													{
														rectangle.X = 144;
														rectangle.Y = 36;
													}
												}
											}
											else
											{
												if (num28 != num && num33 != num)
												{
													if (num36 == 0)
													{
														rectangle.X = 180;
														rectangle.Y = 0;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 180;
															rectangle.Y = 18;
														}
														else
														{
															rectangle.X = 180;
															rectangle.Y = 36;
														}
													}
												}
												else
												{
													if (num30 != num && num35 != num)
													{
														if (num36 == 0)
														{
															rectangle.X = 198;
															rectangle.Y = 0;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 198;
																rectangle.Y = 18;
															}
															else
															{
																rectangle.X = 198;
																rectangle.Y = 36;
															}
														}
													}
													else
													{
														if (num36 == 0)
														{
															rectangle.X = 18;
															rectangle.Y = 18;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 36;
																rectangle.Y = 18;
															}
															else
															{
																rectangle.X = 54;
																rectangle.Y = 18;
															}
														}
													}
												}
											}
										}
									}
									else
									{
										if (num29 != num && num34 == num && (num31 == num & num32 == num))
										{
											if (num36 == 0)
											{
												rectangle.X = 18;
												rectangle.Y = 0;
											}
											else
											{
												if (num36 == 1)
												{
													rectangle.X = 36;
													rectangle.Y = 0;
												}
												else
												{
													rectangle.X = 54;
													rectangle.Y = 0;
												}
											}
										}
										else
										{
											if (num29 == num && num34 != num && (num31 == num & num32 == num))
											{
												if (num36 == 0)
												{
													rectangle.X = 18;
													rectangle.Y = 36;
												}
												else
												{
													if (num36 == 1)
													{
														rectangle.X = 36;
														rectangle.Y = 36;
													}
													else
													{
														rectangle.X = 54;
														rectangle.Y = 36;
													}
												}
											}
											else
											{
												if (num29 == num && num34 == num && (num31 != num & num32 == num))
												{
													if (num36 == 0)
													{
														rectangle.X = 0;
														rectangle.Y = 0;
													}
													else
													{
														if (num36 == 1)
														{
															rectangle.X = 0;
															rectangle.Y = 18;
														}
														else
														{
															rectangle.X = 0;
															rectangle.Y = 36;
														}
													}
												}
												else
												{
													if (num29 == num && num34 == num && (num31 == num & num32 != num))
													{
														if (num36 == 0)
														{
															rectangle.X = 72;
															rectangle.Y = 0;
														}
														else
														{
															if (num36 == 1)
															{
																rectangle.X = 72;
																rectangle.Y = 18;
															}
															else
															{
																rectangle.X = 72;
																rectangle.Y = 36;
															}
														}
													}
													else
													{
														if (num29 != num && num34 == num && (num31 != num & num32 == num))
														{
															if (num36 == 0)
															{
																rectangle.X = 0;
																rectangle.Y = 54;
															}
															else
															{
																if (num36 == 1)
																{
																	rectangle.X = 36;
																	rectangle.Y = 54;
																}
																else
																{
																	rectangle.X = 72;
																	rectangle.Y = 54;
																}
															}
														}
														else
														{
															if (num29 != num && num34 == num && (num31 == num & num32 != num))
															{
																if (num36 == 0)
																{
																	rectangle.X = 18;
																	rectangle.Y = 54;
																}
																else
																{
																	if (num36 == 1)
																	{
																		rectangle.X = 54;
																		rectangle.Y = 54;
																	}
																	else
																	{
																		rectangle.X = 90;
																		rectangle.Y = 54;
																	}
																}
															}
															else
															{
																if (num29 == num && num34 != num && (num31 != num & num32 == num))
																{
																	if (num36 == 0)
																	{
																		rectangle.X = 0;
																		rectangle.Y = 72;
																	}
																	else
																	{
																		if (num36 == 1)
																		{
																			rectangle.X = 36;
																			rectangle.Y = 72;
																		}
																		else
																		{
																			rectangle.X = 72;
																			rectangle.Y = 72;
																		}
																	}
																}
																else
																{
																	if (num29 == num && num34 != num && (num31 == num & num32 != num))
																	{
																		if (num36 == 0)
																		{
																			rectangle.X = 18;
																			rectangle.Y = 72;
																		}
																		else
																		{
																			if (num36 == 1)
																			{
																				rectangle.X = 54;
																				rectangle.Y = 72;
																			}
																			else
																			{
																				rectangle.X = 90;
																				rectangle.Y = 72;
																			}
																		}
																	}
																	else
																	{
																		if (num29 == num && num34 == num && (num31 != num & num32 != num))
																		{
																			if (num36 == 0)
																			{
																				rectangle.X = 90;
																				rectangle.Y = 0;
																			}
																			else
																			{
																				if (num36 == 1)
																				{
																					rectangle.X = 90;
																					rectangle.Y = 18;
																				}
																				else
																				{
																					rectangle.X = 90;
																					rectangle.Y = 36;
																				}
																			}
																		}
																		else
																		{
																			if (num29 != num && num34 != num && (num31 == num & num32 == num))
																			{
																				if (num36 == 0)
																				{
																					rectangle.X = 108;
																					rectangle.Y = 72;
																				}
																				else
																				{
																					if (num36 == 1)
																					{
																						rectangle.X = 126;
																						rectangle.Y = 72;
																					}
																					else
																					{
																						rectangle.X = 144;
																						rectangle.Y = 72;
																					}
																				}
																			}
																			else
																			{
																				if (num29 != num && num34 == num && (num31 != num & num32 != num))
																				{
																					if (num36 == 0)
																					{
																						rectangle.X = 108;
																						rectangle.Y = 0;
																					}
																					else
																					{
																						if (num36 == 1)
																						{
																							rectangle.X = 126;
																							rectangle.Y = 0;
																						}
																						else
																						{
																							rectangle.X = 144;
																							rectangle.Y = 0;
																						}
																					}
																				}
																				else
																				{
																					if (num29 == num && num34 != num && (num31 != num & num32 != num))
																					{
																						if (num36 == 0)
																						{
																							rectangle.X = 108;
																							rectangle.Y = 54;
																						}
																						else
																						{
																							if (num36 == 1)
																							{
																								rectangle.X = 126;
																								rectangle.Y = 54;
																							}
																							else
																							{
																								rectangle.X = 144;
																								rectangle.Y = 54;
																							}
																						}
																					}
																					else
																					{
																						if (num29 != num && num34 != num && (num31 != num & num32 == num))
																						{
																							if (num36 == 0)
																							{
																								rectangle.X = 162;
																								rectangle.Y = 0;
																							}
																							else
																							{
																								if (num36 == 1)
																								{
																									rectangle.X = 162;
																									rectangle.Y = 18;
																								}
																								else
																								{
																									rectangle.X = 162;
																									rectangle.Y = 36;
																								}
																							}
																						}
																						else
																						{
																							if (num29 != num && num34 != num && (num31 == num & num32 != num))
																							{
																								if (num36 == 0)
																								{
																									rectangle.X = 216;
																									rectangle.Y = 0;
																								}
																								else
																								{
																									if (num36 == 1)
																									{
																										rectangle.X = 216;
																										rectangle.Y = 18;
																									}
																									else
																									{
																										rectangle.X = 216;
																										rectangle.Y = 36;
																									}
																								}
																							}
																							else
																							{
																								if (num29 != num && num34 != num && (num31 != num & num32 != num))
																								{
																									if (num36 == 0)
																									{
																										rectangle.X = 162;
																										rectangle.Y = 54;
																									}
																									else
																									{
																										if (num36 == 1)
																										{
																											rectangle.X = 180;
																											rectangle.Y = 54;
																										}
																										else
																										{
																											rectangle.X = 198;
																											rectangle.Y = 54;
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
								if (rectangle.X <= -1 || rectangle.Y <= -1)
								{
									if (num36 <= 0)
									{
										rectangle.X = 18;
										rectangle.Y = 18;
									}
									else
									{
										if (num36 == 1)
										{
											rectangle.X = 36;
											rectangle.Y = 18;
										}
									}
									if (num36 >= 2)
									{
										rectangle.X = 54;
										rectangle.Y = 18;
									}
								}
								Main.tile[i, j].frameX = (short)rectangle.X;
								Main.tile[i, j].frameY = (short)rectangle.Y;
								if (num == 52 || num == 62 || num == 115)
								{
									if (Main.tile[i, j - 1] != null)
									{
										if (!Main.tile[i, j - 1].active)
										{
											num29 = -1;
										}
										else
										{
											num29 = (int)Main.tile[i, j - 1].type;
										}
									}
									else
									{
										num29 = num;
									}
									if (num == 52 && (num29 == 109 || num29 == 115))
									{
										Main.tile[i, j].type = 115;
										WorldGen.SquareTileFrame(i, j, true);
										return;
									}
									if (num == 115 && (num29 == 2 || num29 == 52))
									{
										Main.tile[i, j].type = 52;
										WorldGen.SquareTileFrame(i, j, true);
										return;
									}
									if (num29 != num)
									{
										bool flag4 = false;
										if (num29 == -1)
										{
											flag4 = true;
										}
										if (num == 52 && num29 != 2)
										{
											flag4 = true;
										}
										if (num == 62 && num29 != 60)
										{
											flag4 = true;
										}
										if (num == 115 && num29 != 109)
										{
											flag4 = true;
										}
										if (flag4)
										{
											WorldGen.KillTile(i, j, false, false, false);
										}
									}
								}
								if (!WorldGen.noTileActions && (num == 53 || num == 112 || num == 116 || num == 123))
								{
									if (Main.netMode == 0)
									{
										if (Main.tile[i, j + 1] != null && !Main.tile[i, j + 1].active)
										{
											bool flag5 = true;
											if (Main.tile[i, j - 1].active && Main.tile[i, j - 1].type == 21)
											{
												flag5 = false;
											}
											if (flag5)
											{
												int type = 31;
												if (num == 59)
												{
													type = 39;
												}
												if (num == 57)
												{
													type = 40;
												}
												if (num == 112)
												{
													type = 56;
												}
												if (num == 116)
												{
													type = 67;
												}
												if (num == 123)
												{
													type = 71;
												}
												Main.tile[i, j].active = false;
												int num38 = Projectile.NewProjectile((float)(i * 16 + 8), (float)(j * 16 + 8), 0f, 0.41f, type, 10, 0f, Main.myPlayer);
												Main.projectile[num38].ai[0] = 1f;
												WorldGen.SquareTileFrame(i, j, true);
											}
										}
									}
									else
									{
										if (Main.netMode == 2 && Main.tile[i, j + 1] != null && !Main.tile[i, j + 1].active)
										{
											bool flag6 = true;
											if (Main.tile[i, j - 1].active && Main.tile[i, j - 1].type == 21)
											{
												flag6 = false;
											}
											if (flag6)
											{
												int type2 = 31;
												if (num == 59)
												{
													type2 = 39;
												}
												if (num == 57)
												{
													type2 = 40;
												}
												if (num == 112)
												{
													type2 = 56;
												}
												if (num == 116)
												{
													type2 = 67;
												}
												if (num == 123)
												{
													type2 = 71;
												}
												Main.tile[i, j].active = false;
												int num39 = Projectile.NewProjectile((float)(i * 16 + 8), (float)(j * 16 + 8), 0f, 2.5f, type2, 10, 0f, Main.myPlayer);
												Main.projectile[num39].velocity.Y = 0.5f;
												Projectile expr_5ED7_cp_0 = Main.projectile[num39];
												expr_5ED7_cp_0.position.Y = expr_5ED7_cp_0.position.Y + 2f;
												Main.projectile[num39].netUpdate = true;
												NetMessage.SendTileSquare(-1, i, j, 1);
												WorldGen.SquareTileFrame(i, j, true);
											}
										}
									}
								}
								if (rectangle.X != frameX && rectangle.Y != frameY && frameX >= 0 && frameY >= 0)
								{
									bool flag7 = WorldGen.mergeUp;
									bool flag8 = WorldGen.mergeDown;
									bool flag9 = WorldGen.mergeLeft;
									bool flag10 = WorldGen.mergeRight;
									WorldGen.TileFrame(i - 1, j, false, false);
									WorldGen.TileFrame(i + 1, j, false, false);
									WorldGen.TileFrame(i, j - 1, false, false);
									WorldGen.TileFrame(i, j + 1, false, false);
									WorldGen.mergeUp = flag7;
									WorldGen.mergeDown = flag8;
									WorldGen.mergeLeft = flag9;
									WorldGen.mergeRight = flag10;
								}
							}
						}
					}
				}
			}
			catch
			{
			}
		}
	}
}
