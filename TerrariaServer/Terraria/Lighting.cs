
using System;
using System.Diagnostics;
namespace Terraria
{
	public class Lighting
	{
		public static int maxRenderCount = 4;
		public static int dirX;
		public static int dirY;
		public static float brightness = 1f;
		public static float defBrightness = 1f;
		public static int lightMode = 0;
		public static bool RGB = true;
		public static float oldSkyColor = 0f;
		public static float skyColor = 0f;
		private static float lightColor = 0f;
		private static float lightColorG = 0f;
		private static float lightColorB = 0f;
		public static int lightCounter = 0;
		public static int offScreenTiles = 45;
		public static int offScreenTiles2 = 35;
		private static int firstTileX;
		private static int lastTileX;
		private static int firstTileY;
		private static int lastTileY;
		public static float[,] color = new float[Main.screenWidth + Lighting.offScreenTiles * 2 + 10, Main.screenHeight + Lighting.offScreenTiles * 2 + 10];
		public static float[,] colorG = new float[Main.screenWidth + Lighting.offScreenTiles * 2 + 10, Main.screenHeight + Lighting.offScreenTiles * 2 + 10];
		public static float[,] colorB = new float[Main.screenWidth + Lighting.offScreenTiles * 2 + 10, Main.screenHeight + Lighting.offScreenTiles * 2 + 10];
		public static float[,] color2 = new float[Main.screenWidth + Lighting.offScreenTiles * 2 + 10, Main.screenHeight + Lighting.offScreenTiles * 2 + 10];
		public static float[,] colorG2 = new float[Main.screenWidth + Lighting.offScreenTiles * 2 + 10, Main.screenHeight + Lighting.offScreenTiles * 2 + 10];
		public static float[,] colorB2 = new float[Main.screenWidth + Lighting.offScreenTiles * 2 + 10, Main.screenHeight + Lighting.offScreenTiles * 2 + 10];
		public static bool[,] stopLight = new bool[Main.screenWidth + Lighting.offScreenTiles * 2 + 10, Main.screenHeight + Lighting.offScreenTiles * 2 + 10];
		public static bool[,] wetLight = new bool[Main.screenWidth + Lighting.offScreenTiles * 2 + 10, Main.screenHeight + Lighting.offScreenTiles * 2 + 10];
		public static int scrX;
		public static int scrY;
		public static int minX;
		public static int maxX;
		public static int minY;
		public static int maxY;
		private static int maxTempLights = 2000;
		private static int[] tempLightX = new int[Lighting.maxTempLights];
		private static int[] tempLightY = new int[Lighting.maxTempLights];
		private static float[] tempLight = new float[Lighting.maxTempLights];
		private static float[] tempLightG = new float[Lighting.maxTempLights];
		private static float[] tempLightB = new float[Lighting.maxTempLights];
		public static int tempLightCount;
		private static int firstToLightX;
		private static int firstToLightY;
		private static int lastToLightX;
		private static int lastToLightY;
		public static bool resize = false;
		private static float negLight = 0.04f;
		private static float negLight2 = 0.16f;
		private static float wetLightR = 0.16f;
		private static float wetLightG = 0.16f;
		private static float blueWave = 1f;
		private static int blueDir = 1;
		private static int minX7;
		private static int maxX7;
		private static int minY7;
		private static int maxY7;
		private static int firstTileX7;
		private static int lastTileX7;
		private static int lastTileY7;
		private static int firstTileY7;
		private static int firstToLightX7;
		private static int lastToLightX7;
		private static int firstToLightY7;
		private static int lastToLightY7;
		private static int firstToLightX27;
		private static int lastToLightX27;
		private static int firstToLightY27;
		private static int lastToLightY27;
		public static void LightTiles(int firstX, int lastX, int firstY, int lastY)
		{
			Main.render = true;
			Lighting.oldSkyColor = Lighting.skyColor;
			Lighting.skyColor = (float)((Main.tileColor.R + Main.tileColor.G + Main.tileColor.B) / 3) / 255f;
			if (Lighting.lightMode < 2)
			{
				Lighting.brightness = 1.2f;
				Lighting.offScreenTiles2 = 34;
				Lighting.offScreenTiles = 40;
			}
			else
			{
				Lighting.brightness = 1f;
				Lighting.offScreenTiles2 = 18;
				Lighting.offScreenTiles = 23;
			}
			if (Main.player[Main.myPlayer].blind)
			{
				Lighting.brightness = 1f;
			}
			Lighting.defBrightness = Lighting.brightness;
			Lighting.firstTileX = firstX;
			Lighting.lastTileX = lastX;
			Lighting.firstTileY = firstY;
			Lighting.lastTileY = lastY;
			Lighting.firstToLightX = Lighting.firstTileX - Lighting.offScreenTiles;
			Lighting.firstToLightY = Lighting.firstTileY - Lighting.offScreenTiles;
			Lighting.lastToLightX = Lighting.lastTileX + Lighting.offScreenTiles;
			Lighting.lastToLightY = Lighting.lastTileY + Lighting.offScreenTiles;
			if (Lighting.firstToLightX < 0)
			{
				Lighting.firstToLightX = 0;
			}
			if (Lighting.lastToLightX >= Main.maxTilesX)
			{
				Lighting.lastToLightX = Main.maxTilesX - 1;
			}
			if (Lighting.firstToLightY < 0)
			{
				Lighting.firstToLightY = 0;
			}
			if (Lighting.lastToLightY >= Main.maxTilesY)
			{
				Lighting.lastToLightY = Main.maxTilesY - 1;
			}
			int num = Lighting.firstTileX - Lighting.offScreenTiles2;
			int num2 = Lighting.firstTileY - Lighting.offScreenTiles2;
			int num3 = Lighting.lastTileX + Lighting.offScreenTiles2;
			int num4 = Lighting.lastTileY + Lighting.offScreenTiles2;
			if (num < 0)
			{
				num = 0;
			}
			if (num3 >= Main.maxTilesX)
			{
				num3 = Main.maxTilesX - 1;
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num4 >= Main.maxTilesY)
			{
				num4 = Main.maxTilesY - 1;
			}
			Lighting.lightCounter++;
			Main.renderCount++;
			int num5 = Main.screenWidth / 16 + Lighting.offScreenTiles * 2;
			int num6 = Main.screenHeight / 16 + Lighting.offScreenTiles * 2;
			Vector2 vector = Main.screenLastPosition;
			Lighting.doColors();
			if (Main.renderCount == 2)
			{
				vector = Main.screenPosition;
				int num7 = (int)(Main.screenPosition.X / 16f) - Lighting.scrX;
				int num8 = (int)(Main.screenPosition.Y / 16f) - Lighting.scrY;
				if (num7 > 4)
				{
					num7 = 0;
				}
				if (num8 > 4)
				{
					num8 = 0;
				}
				if (Lighting.RGB)
				{
					for (int i = 0; i < num5; i++)
					{
						if (i + num7 >= 0)
						{
							for (int j = 0; j < num6; j++)
							{
								if (j + num8 >= 0)
								{
									Lighting.color[i, j] = Lighting.color2[i + num7, j + num8];
									Lighting.colorG[i, j] = Lighting.colorG2[i + num7, j + num8];
									Lighting.colorB[i, j] = Lighting.colorB2[i + num7, j + num8];
								}
							}
						}
					}
				}
				else
				{
					for (int k = 0; k < num5; k++)
					{
						if (k + num7 >= 0)
						{
							for (int l = 0; l < num6; l++)
							{
								if (l + num8 >= 0)
								{
									Lighting.color[k, l] = Lighting.color2[k + num7, l + num8];
									Lighting.colorG[k, l] = Lighting.color2[k + num7, l + num8];
									Lighting.colorB[k, l] = Lighting.color2[k + num7, l + num8];
								}
							}
						}
					}
				}
			}
			if (Main.renderCount != 2 && !Lighting.resize && !Main.renderNow)
			{
				if (Math.Abs(Main.screenPosition.X / 16f - vector.X / 16f) < 5f)
				{
					while ((int)(Main.screenPosition.X / 16f) < (int)(vector.X / 16f))
					{
						vector.X -= 16f;
						for (int m = num5 - 1; m > 1; m--)
						{
							for (int n = 0; n < num6; n++)
							{
								Lighting.color[m, n] = Lighting.color[m - 1, n];
								Lighting.colorG[m, n] = Lighting.colorG[m - 1, n];
								Lighting.colorB[m, n] = Lighting.colorB[m - 1, n];
							}
						}
					}
					while ((int)(Main.screenPosition.X / 16f) > (int)(vector.X / 16f))
					{
						vector.X += 16f;
						for (int num9 = 0; num9 < num5 - 1; num9++)
						{
							for (int num10 = 0; num10 < num6; num10++)
							{
								Lighting.color[num9, num10] = Lighting.color[num9 + 1, num10];
								Lighting.colorG[num9, num10] = Lighting.colorG[num9 + 1, num10];
								Lighting.colorB[num9, num10] = Lighting.colorB[num9 + 1, num10];
							}
						}
					}
				}
				if (Math.Abs(Main.screenPosition.Y / 16f - vector.Y / 16f) < 5f)
				{
					while ((int)(Main.screenPosition.Y / 16f) < (int)(vector.Y / 16f))
					{
						vector.Y -= 16f;
						for (int num11 = num6 - 1; num11 > 1; num11--)
						{
							for (int num12 = 0; num12 < num5; num12++)
							{
								Lighting.color[num12, num11] = Lighting.color[num12, num11 - 1];
								Lighting.colorG[num12, num11] = Lighting.colorG[num12, num11 - 1];
								Lighting.colorB[num12, num11] = Lighting.colorB[num12, num11 - 1];
							}
						}
					}
					while ((int)(Main.screenPosition.Y / 16f) > (int)(vector.Y / 16f))
					{
						vector.Y += 16f;
						for (int num13 = 0; num13 < num6 - 1; num13++)
						{
							for (int num14 = 0; num14 < num5 - 1; num14++)
							{
								Lighting.color[num14, num13] = Lighting.color[num14, num13 + 1];
								Lighting.colorG[num14, num13] = Lighting.colorG[num14, num13 + 1];
								Lighting.colorB[num14, num13] = Lighting.colorB[num14, num13 + 1];
							}
						}
					}
				}
				if (Lighting.oldSkyColor != Lighting.skyColor)
				{
					for (int num15 = Lighting.firstToLightX; num15 < Lighting.lastToLightX; num15++)
					{
						for (int num16 = Lighting.firstToLightY; num16 < Lighting.lastToLightY; num16++)
						{
							if ((!Main.tile[num15, num16].active || !Main.tileNoSunLight[(int)Main.tile[num15, num16].type]) && Lighting.color[num15 - Lighting.firstToLightX, num16 - Lighting.firstToLightY] < Lighting.skyColor && (Main.tile[num15, num16].wall == 0 || Main.tile[num15, num16].wall == 21) && (double)num16 < Main.worldSurface && Main.tile[num15, num16].liquid < 200)
							{
								if (Lighting.color[num15 - Lighting.firstToLightX, num16 - Lighting.firstToLightY] < Lighting.skyColor)
								{
									Lighting.color[num15 - Lighting.firstToLightX, num16 - Lighting.firstToLightY] = (float)Main.tileColor.R / 255f;
								}
								if (Lighting.colorG[num15 - Lighting.firstToLightX, num16 - Lighting.firstToLightY] < Lighting.skyColor)
								{
									Lighting.colorG[num15 - Lighting.firstToLightX, num16 - Lighting.firstToLightY] = (float)Main.tileColor.G / 255f;
								}
								if (Lighting.colorB[num15 - Lighting.firstToLightX, num16 - Lighting.firstToLightY] < Lighting.skyColor)
								{
									Lighting.colorB[num15 - Lighting.firstToLightX, num16 - Lighting.firstToLightY] = (float)Main.tileColor.B / 255f;
								}
							}
						}
					}
				}
			}
			else
			{
				Lighting.lightCounter = 0;
			}
			if (Main.renderCount <= Lighting.maxRenderCount)
			{
				return;
			}
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			long arg_914_0 = stopwatch.ElapsedMilliseconds;
			Lighting.resize = false;
			Main.drawScene = true;
			Lighting.ResetRange();
			if (Lighting.lightMode == 0 || Lighting.lightMode == 3)
			{
				Lighting.RGB = true;
			}
			else
			{
				Lighting.RGB = false;
			}
			int num17 = 0;
			int num18 = Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10;
			int num19 = 0;
			int num20 = Main.screenHeight / 16 + Lighting.offScreenTiles * 2 + 10;
			for (int num21 = num17; num21 < num18; num21++)
			{
				for (int num22 = num19; num22 < num20; num22++)
				{
					Lighting.color2[num21, num22] = 0f;
					Lighting.colorG2[num21, num22] = 0f;
					Lighting.colorB2[num21, num22] = 0f;
					Lighting.stopLight[num21, num22] = false;
					Lighting.wetLight[num21, num22] = false;
				}
			}
			for (int num23 = 0; num23 < Lighting.tempLightCount; num23++)
			{
				if (Lighting.tempLightX[num23] - Lighting.firstTileX + Lighting.offScreenTiles >= 0 && Lighting.tempLightX[num23] - Lighting.firstTileX + Lighting.offScreenTiles < Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10 && Lighting.tempLightY[num23] - Lighting.firstTileY + Lighting.offScreenTiles >= 0 && Lighting.tempLightY[num23] - Lighting.firstTileY + Lighting.offScreenTiles < Main.screenHeight / 16 + Lighting.offScreenTiles * 2 + 10)
				{
					int num24 = Lighting.tempLightX[num23] - Lighting.firstTileX + Lighting.offScreenTiles;
					int num25 = Lighting.tempLightY[num23] - Lighting.firstTileY + Lighting.offScreenTiles;
					if (Lighting.color2[num24, num25] < Lighting.tempLight[num23])
					{
						Lighting.color2[num24, num25] = Lighting.tempLight[num23];
					}
					if (Lighting.colorG2[num24, num25] < Lighting.tempLightG[num23])
					{
						Lighting.colorG2[num24, num25] = Lighting.tempLightG[num23];
					}
					if (Lighting.colorB2[num24, num25] < Lighting.tempLightB[num23])
					{
						Lighting.colorB2[num24, num25] = Lighting.tempLightB[num23];
					}
				}
			}
			if (Main.wof >= 0 && Main.player[Main.myPlayer].gross)
			{
				try
				{
					int num26 = (int)Main.screenPosition.Y / 16 - 10;
					int num27 = (int)(Main.screenPosition.Y + (float)Main.screenHeight) / 16 + 10;
					int num28 = (int)Main.npc[Main.wof].position.X / 16;
					if (Main.npc[Main.wof].direction > 0)
					{
						num28 -= 3;
					}
					else
					{
						num28 += 2;
					}
					int num29 = num28 + 8;
					float num30 = 0.5f * Main.demonTorch + 1f * (1f - Main.demonTorch);
					float num31 = 0.3f;
					float num32 = 1f * Main.demonTorch + 0.5f * (1f - Main.demonTorch);
					num30 *= 0.2f;
					num31 *= 0.1f;
					num32 *= 0.3f;
					for (int num33 = num28; num33 <= num29; num33++)
					{
						for (int num34 = num26; num34 <= num27; num34++)
						{
							if (Lighting.color2[num33 - Lighting.firstToLightX, num34 - Lighting.firstToLightY] < num30)
							{
								Lighting.color2[num33 - Lighting.firstToLightX, num34 - Lighting.firstToLightY] = num30;
							}
							if (Lighting.colorG2[num33 - Lighting.firstToLightX, num34 - Lighting.firstToLightY] < num31)
							{
								Lighting.colorG2[num33 - Lighting.firstToLightX, num34 - Lighting.firstToLightY] = num31;
							}
							if (Lighting.colorB2[num33 - Lighting.firstToLightX, num34 - Lighting.firstToLightY] < num32)
							{
								Lighting.colorB2[num33 - Lighting.firstToLightX, num34 - Lighting.firstToLightY] = num32;
							}
						}
					}
				}
				catch
				{
				}
			}
			if (!Main.renderNow)
			{
				Main.oldTempLightCount = Lighting.tempLightCount;
				Lighting.tempLightCount = 0;
			}
			if (Main.gamePaused)
			{
				Lighting.tempLightCount = Main.oldTempLightCount;
			}
			Main.sandTiles = 0;
			Main.evilTiles = 0;
			Main.snowTiles = 0;
			Main.holyTiles = 0;
			Main.meteorTiles = 0;
			Main.jungleTiles = 0;
			Main.dungeonTiles = 0;
			Main.musicBox = -1;
			num17 = Lighting.firstToLightX;
			num18 = Lighting.lastToLightX;
			num19 = Lighting.firstToLightY;
			num20 = Lighting.lastToLightY;
			for (int num35 = num17; num35 < num18; num35++)
			{
				for (int num36 = num19; num36 < num20; num36++)
				{
					if ((!Main.tile[num35, num36].active || !Main.tileNoSunLight[(int)Main.tile[num35, num36].type]) && Lighting.color2[num35 - Lighting.firstToLightX, num36 - Lighting.firstToLightY] < Lighting.skyColor && (Main.tile[num35, num36].wall == 0 || Main.tile[num35, num36].wall == 21) && (double)num36 < Main.worldSurface && Main.tile[num35, num36].liquid < 200)
					{
						if (Lighting.color2[num35 - Lighting.firstToLightX, num36 - Lighting.firstToLightY] < Lighting.skyColor)
						{
							Lighting.color2[num35 - Lighting.firstToLightX, num36 - Lighting.firstToLightY] = (float)Main.tileColor.R / 255f;
						}
						if (Lighting.colorG2[num35 - Lighting.firstToLightX, num36 - Lighting.firstToLightY] < Lighting.skyColor)
						{
							Lighting.colorG2[num35 - Lighting.firstToLightX, num36 - Lighting.firstToLightY] = (float)Main.tileColor.G / 255f;
						}
						if (Lighting.colorB2[num35 - Lighting.firstToLightX, num36 - Lighting.firstToLightY] < Lighting.skyColor)
						{
							Lighting.colorB2[num35 - Lighting.firstToLightX, num36 - Lighting.firstToLightY] = (float)Main.tileColor.B / 255f;
						}
					}
				}
			}
			for (int num37 = num17; num37 < num18; num37++)
			{
				for (int num38 = num19; num38 < num20; num38++)
				{
					int num39 = num37 - Lighting.firstToLightX;
					int num40 = num38 - Lighting.firstToLightY;
					int zoneX = Main.zoneX;
					int zoneY = Main.zoneY;
					int num41 = (num18 - num17 - zoneX) / 2;
					int num42 = (num20 - num19 - zoneY) / 2;
					if (Main.tile[num37, num38].active)
					{
						if (num37 > num17 + num41 && num37 < num18 - num41 && num38 > num19 + num42 && num38 < num20 - num42)
						{
							byte type = Main.tile[num37, num38].type;
							if (type <= 53)
							{
								if (type <= 32)
								{
									switch (type)
									{
										case 23:
										case 24:
										case 25:
										{
											break;
										}
										case 26:
										{
											goto IL_122F;
										}
										case 27:
										{
											Main.evilTiles -= 5;
											goto IL_122F;
										}
										default:
										{
											if (type != 32)
											{
												goto IL_122F;
											}
											break;
										}
									}
									Main.evilTiles++;
								}
								else
								{
									if (type != 37)
									{
										switch (type)
										{
											case 41:
											case 43:
											case 44:
											{
												Main.dungeonTiles++;
												break;
											}
											case 42:
											{
												break;
											}
											default:
											{
												if (type == 53)
												{
													Main.sandTiles++;
												}
												break;
											}
										}
									}
									else
									{
										Main.meteorTiles++;
									}
								}
							}
							else
							{
								if (type <= 84)
								{
									switch (type)
									{
										case 60:
										case 61:
										case 62:
										{
											break;
										}
										default:
										{
											if (type != 84)
											{
												goto IL_122F;
											}
											break;
										}
									}
									Main.jungleTiles++;
								}
								else
								{
									switch (type)
									{
										case 109:
										case 110:
										case 113:
										case 117:
										{
											Main.holyTiles++;
											break;
										}
										case 111:
										case 114:
										case 115:
										{
											break;
										}
										case 112:
										{
											Main.sandTiles++;
											Main.evilTiles++;
											break;
										}
										case 116:
										{
											Main.sandTiles++;
											Main.holyTiles++;
											break;
										}
										default:
										{
											if (type != 139)
											{
												switch (type)
												{
													case 147:
													case 148:
													{
														Main.snowTiles++;
														break;
													}
												}
											}
											else
											{
												if (Main.tile[num37, num38].frameX >= 36)
												{
													int num43 = 0;
													for (int num44 = (int)(Main.tile[num37, num38].frameY / 18); num44 >= 2; num44 -= 2)
													{
														num43++;
													}
													Main.musicBox = num43;
												}
											}
											break;
										}
									}
								}
							}
						}
						IL_122F:
						if (Main.tileBlockLight[(int)Main.tile[num37, num38].type] && Main.tile[num37, num38].type != 131)
						{
							Lighting.stopLight[num39, num40] = true;
						}
						if (Main.tileLighted[(int)Main.tile[num37, num38].type])
						{
							if (Lighting.RGB)
							{
								byte type = Main.tile[num37, num38].type;
								if (type <= 61)
								{
									if (type <= 22)
									{
										if (type != 4)
										{
											if (type != 17)
											{
												if (type != 22)
												{
													goto IL_2DE7;
												}
												goto IL_1CAE;
											}
										}
										else
										{
											float num45 = 1f;
											float num46 = 0.95f;
											float num47 = 0.8f;
											if (Main.tile[num37, num38].frameX >= 66)
											{
												goto IL_2DE7;
											}
											int num48 = (int)(Main.tile[num37, num38].frameY / 22);
											if (num48 == 1)
											{
												num45 = 0f;
												num46 = 0.1f;
												num47 = 1.3f;
											}
											else
											{
												if (num48 == 2)
												{
													num45 = 1f;
													num46 = 0.1f;
													num47 = 0.1f;
												}
												else
												{
													if (num48 == 3)
													{
														num45 = 0f;
														num46 = 1f;
														num47 = 0.1f;
													}
													else
													{
														if (num48 == 4)
														{
															num45 = 0.9f;
															num46 = 0f;
															num47 = 0.9f;
														}
														else
														{
															if (num48 == 5)
															{
																num45 = 1.3f;
																num46 = 1.3f;
																num47 = 1.3f;
															}
															else
															{
																if (num48 == 6)
																{
																	num45 = 0.9f;
																	num46 = 0.9f;
																	num47 = 0f;
																}
																else
																{
																	if (num48 == 7)
																	{
																		num45 = 0.5f * Main.demonTorch + 1f * (1f - Main.demonTorch);
																		num46 = 0.3f;
																		num47 = 1f * Main.demonTorch + 0.5f * (1f - Main.demonTorch);
																	}
																	else
																	{
																		if (num48 == 8)
																		{
																			num47 = 0.7f;
																			num45 = 0.85f;
																			num46 = 1f;
																		}
																	}
																}
															}
														}
													}
												}
											}
											if (Lighting.color2[num39, num40] < num45)
											{
												Lighting.color2[num39, num40] = num45;
											}
											if (Lighting.colorG2[num39, num40] < num46)
											{
												Lighting.colorG2[num39, num40] = num46;
											}
											if (Lighting.colorB2[num39, num40] < num47)
											{
												Lighting.colorB2[num39, num40] = num47;
												goto IL_2DE7;
											}
											goto IL_2DE7;
										}
									}
									else
									{
										if (type <= 42)
										{
											switch (type)
											{
												case 26:
												case 31:
												{
													float num49 = (float)Main.rand.Next(-5, 6) * 0.0025f;
													if (Lighting.color2[num39, num40] < 0.31f + num49)
													{
														Lighting.color2[num39, num40] = 0.31f + num49;
													}
													if (Lighting.colorG2[num39, num40] < 0.1f + num49)
													{
														Lighting.colorG2[num39, num40] = 0.1f;
													}
													if (Lighting.colorB2[num39, num40] < 0.44f + num49 * 2f)
													{
														Lighting.colorB2[num39, num40] = 0.44f + num49 * 2f;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												case 27:
												case 28:
												case 29:
												case 30:
												case 32:
												{
													goto IL_2DE7;
												}
												case 33:
												{
													if (Main.tile[num37, num38].frameX != 0)
													{
														goto IL_2DE7;
													}
													if (Lighting.color2[num39, num40] < 1f)
													{
														Lighting.color2[num39, num40] = 1f;
													}
													if ((double)Lighting.colorG2[num39, num40] < 0.95)
													{
														Lighting.colorG2[num39, num40] = 0.95f;
													}
													if ((double)Lighting.colorB2[num39, num40] < 0.65)
													{
														Lighting.colorB2[num39, num40] = 0.65f;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												case 34:
												case 35:
												{
													if (Main.tile[num37, num38].frameX >= 54)
													{
														goto IL_2DE7;
													}
													if (Lighting.color2[num39, num40] < 1f)
													{
														Lighting.color2[num39, num40] = 1f;
													}
													if ((double)Lighting.colorG2[num39, num40] < 0.95)
													{
														Lighting.colorG2[num39, num40] = 0.95f;
													}
													if ((double)Lighting.colorB2[num39, num40] < 0.8)
													{
														Lighting.colorB2[num39, num40] = 0.8f;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												case 36:
												{
													if (Main.tile[num37, num38].frameX >= 54)
													{
														goto IL_2DE7;
													}
													if (Lighting.color2[num39, num40] < 1f)
													{
														Lighting.color2[num39, num40] = 1f;
													}
													if ((double)Lighting.colorG2[num39, num40] < 0.95)
													{
														Lighting.colorG2[num39, num40] = 0.95f;
													}
													if ((double)Lighting.colorB2[num39, num40] < 0.65)
													{
														Lighting.colorB2[num39, num40] = 0.65f;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												case 37:
												{
													if ((double)Lighting.color2[num39, num40] < 0.56)
													{
														Lighting.color2[num39, num40] = 0.56f;
													}
													if ((double)Lighting.colorG2[num39, num40] < 0.43)
													{
														Lighting.colorG2[num39, num40] = 0.43f;
													}
													if ((double)Lighting.colorB2[num39, num40] < 0.15)
													{
														Lighting.colorB2[num39, num40] = 0.15f;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												default:
												{
													if (type != 42)
													{
														goto IL_2DE7;
													}
													if (Main.tile[num37, num38].frameX != 0)
													{
														goto IL_2DE7;
													}
													if (Lighting.color2[num39, num40] < 0.65f)
													{
														Lighting.color2[num39, num40] = 0.65f;
													}
													if (Lighting.colorG2[num39, num40] < 0.8f)
													{
														Lighting.colorG2[num39, num40] = 0.8f;
													}
													if (Lighting.colorB2[num39, num40] < 0.54f)
													{
														Lighting.colorB2[num39, num40] = 0.54f;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
											}
										}
										else
										{
											if (type != 49)
											{
												if (type != 61)
												{
													goto IL_2DE7;
												}
												if (Main.tile[num37, num38].frameX != 144)
												{
													goto IL_2DE7;
												}
												if (Lighting.color2[num39, num40] < 0.42f)
												{
													Lighting.color2[num39, num40] = 0.42f;
												}
												if (Lighting.colorG2[num39, num40] < 0.81f)
												{
													Lighting.colorG2[num39, num40] = 0.81f;
												}
												if (Lighting.colorB2[num39, num40] < 0.52f)
												{
													Lighting.colorB2[num39, num40] = 0.52f;
													goto IL_2DE7;
												}
												goto IL_2DE7;
											}
											else
											{
												if (Lighting.color2[num39, num40] < 0.3f)
												{
													Lighting.color2[num39, num40] = 0.3f;
												}
												if (Lighting.colorG2[num39, num40] < 0.3f)
												{
													Lighting.colorG2[num39, num40] = 0.3f;
												}
												if (Lighting.colorB2[num39, num40] < 0.75f)
												{
													Lighting.colorB2[num39, num40] = 0.75f;
													goto IL_2DE7;
												}
												goto IL_2DE7;
											}
										}
									}
								}
								else
								{
									if (type <= 100)
									{
										if (type <= 77)
										{
											switch (type)
											{
												case 70:
												case 71:
												case 72:
												{
													float num50 = (float)Main.rand.Next(28, 42) * 0.005f;
													num50 += (float)(270 - (int)Main.mouseTextColor) / 500f;
													if (Lighting.color2[num39, num40] < 0.1f + num50)
													{
														Lighting.color2[num39, num40] = 0.1f;
													}
													if (Lighting.colorG2[num39, num40] < 0.3f + num50 / 2f)
													{
														Lighting.colorG2[num39, num40] = 0.3f + num50 / 2f;
													}
													if (Lighting.colorB2[num39, num40] < 0.6f + num50)
													{
														Lighting.colorB2[num39, num40] = 0.6f + num50;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												default:
												{
													if (type != 77)
													{
														goto IL_2DE7;
													}
													if (Lighting.color2[num39, num40] < 0.75f)
													{
														Lighting.color2[num39, num40] = 0.75f;
													}
													if (Lighting.colorG2[num39, num40] < 0.45f)
													{
														Lighting.colorG2[num39, num40] = 0.45f;
													}
													if (Lighting.colorB2[num39, num40] < 0.25f)
													{
														Lighting.colorB2[num39, num40] = 0.25f;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
											}
										}
										else
										{
											switch (type)
											{
												case 83:
												{
													if (Main.tile[num37, num38].frameX != 18 || Main.dayTime)
													{
														goto IL_2DE7;
													}
													if ((double)Lighting.color2[num39, num40] < 0.1)
													{
														Lighting.color2[num39, num40] = 0.1f;
													}
													if ((double)Lighting.colorG2[num39, num40] < 0.4)
													{
														Lighting.colorG2[num39, num40] = 0.4f;
													}
													if ((double)Lighting.colorB2[num39, num40] < 0.6)
													{
														Lighting.colorB2[num39, num40] = 0.6f;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												case 84:
												{
													int num51 = (int)(Main.tile[num37, num38].frameX / 18);
													if (num51 == 2)
													{
														float num52 = (float)(270 - (int)Main.mouseTextColor);
														num52 /= 800f;
														if (num52 > 1f)
														{
															num52 = 1f;
														}
														if (num52 < 0f)
														{
															num52 = 0f;
														}
														float num53 = num52;
														if (Lighting.color2[num39, num40] < num53 * 0.7f)
														{
															Lighting.color2[num39, num40] = num53 * 0.7f;
														}
														if (Lighting.colorG2[num39, num40] < num53)
														{
															Lighting.colorG2[num39, num40] = num53;
														}
														if (Lighting.colorB2[num39, num40] < num53 * 0.1f)
														{
															Lighting.colorB2[num39, num40] = num53 * 0.1f;
															goto IL_2DE7;
														}
														goto IL_2DE7;
													}
													else
													{
														if (num51 != 5)
														{
															goto IL_2DE7;
														}
														float num53 = 0.9f;
														if (Lighting.color2[num39, num40] < num53)
														{
															Lighting.color2[num39, num40] = num53;
														}
														if (Lighting.colorG2[num39, num40] < num53 * 0.8f)
														{
															Lighting.colorG2[num39, num40] = num53 * 0.8f;
														}
														if (Lighting.colorB2[num39, num40] < num53 * 0.2f)
														{
															Lighting.colorB2[num39, num40] = num53 * 0.2f;
															goto IL_2DE7;
														}
														goto IL_2DE7;
													}
													break;
												}
												default:
												{
													switch (type)
													{
														case 92:
														{
															if (Main.tile[num37, num38].frameY > 18 || Main.tile[num37, num38].frameX != 0)
															{
																goto IL_2DE7;
															}
															if (Lighting.color2[num39, num40] < 1f)
															{
																Lighting.color2[num39, num40] = 1f;
															}
															if (Lighting.colorG2[num39, num40] < 1f)
															{
																Lighting.colorG2[num39, num40] = 1f;
															}
															if (Lighting.colorB2[num39, num40] < 1f)
															{
																Lighting.colorB2[num39, num40] = 1f;
																goto IL_2DE7;
															}
															goto IL_2DE7;
														}
														case 93:
														{
															if (Main.tile[num37, num38].frameY != 0 || Main.tile[num37, num38].frameX != 0)
															{
																goto IL_2DE7;
															}
															if (Lighting.color2[num39, num40] < 1f)
															{
																Lighting.color2[num39, num40] = 1f;
															}
															if ((double)Lighting.colorG2[num39, num40] < 0.97)
															{
																Lighting.colorG2[num39, num40] = 0.97f;
															}
															if ((double)Lighting.colorB2[num39, num40] < 0.85)
															{
																Lighting.colorB2[num39, num40] = 0.85f;
																goto IL_2DE7;
															}
															goto IL_2DE7;
														}
														case 94:
														case 96:
														case 97:
														case 99:
														{
															goto IL_2DE7;
														}
														case 95:
														{
															if (Main.tile[num37, num38].frameX >= 36)
															{
																goto IL_2DE7;
															}
															if (Lighting.color2[num39, num40] < 1f)
															{
																Lighting.color2[num39, num40] = 1f;
															}
															if ((double)Lighting.colorG2[num39, num40] < 0.95)
															{
																Lighting.colorG2[num39, num40] = 0.95f;
															}
															if ((double)Lighting.colorB2[num39, num40] < 0.8)
															{
																Lighting.colorB2[num39, num40] = 0.8f;
																goto IL_2DE7;
															}
															goto IL_2DE7;
														}
														case 98:
														{
															if (Main.tile[num37, num38].frameY != 0)
															{
																goto IL_2DE7;
															}
															if (Lighting.color2[num39, num40] < 1f)
															{
																Lighting.color2[num39, num40] = 1f;
															}
															if ((double)Lighting.colorG2[num39, num40] < 0.97)
															{
																Lighting.colorG2[num39, num40] = 0.97f;
															}
															if ((double)Lighting.colorB2[num39, num40] < 0.85)
															{
																Lighting.colorB2[num39, num40] = 0.85f;
																goto IL_2DE7;
															}
															goto IL_2DE7;
														}
														case 100:
														{
															if (Main.tile[num37, num38].frameX >= 36)
															{
																goto IL_2DE7;
															}
															if (Lighting.color2[num39, num40] < 1f)
															{
																Lighting.color2[num39, num40] = 1f;
															}
															if ((double)Lighting.colorG2[num39, num40] < 0.95)
															{
																Lighting.colorG2[num39, num40] = 0.95f;
															}
															if ((double)Lighting.colorB2[num39, num40] < 0.65)
															{
																Lighting.colorB2[num39, num40] = 0.65f;
																goto IL_2DE7;
															}
															goto IL_2DE7;
														}
														default:
														{
															goto IL_2DE7;
														}
													}
													break;
												}
											}
										}
									}
									else
									{
										if (type <= 133)
										{
											switch (type)
											{
												case 125:
												{
													float num54 = (float)Main.rand.Next(28, 42) * 0.01f;
													num54 += (float)(270 - (int)Main.mouseTextColor) / 800f;
													if ((double)Lighting.colorG2[num39, num40] < 0.1 * (double)num54)
													{
														Lighting.colorG2[num39, num40] = 0.3f * num54;
													}
													if ((double)Lighting.colorB2[num39, num40] < 0.3 * (double)num54)
													{
														Lighting.colorB2[num39, num40] = 0.6f * num54;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												case 126:
												{
													if (Main.tile[num37, num38].frameX >= 36)
													{
														goto IL_2DE7;
													}
													if (Lighting.color2[num39, num40] < (float)Main.DiscoR / 255f)
													{
														Lighting.color2[num39, num40] = (float)Main.DiscoR / 255f;
													}
													if (Lighting.colorG2[num39, num40] < (float)Main.DiscoG / 255f)
													{
														Lighting.colorG2[num39, num40] = (float)Main.DiscoG / 255f;
													}
													if (Lighting.colorB2[num39, num40] < (float)Main.DiscoB / 255f)
													{
														Lighting.colorB2[num39, num40] = (float)Main.DiscoB / 255f;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												case 127:
												case 128:
												{
													goto IL_2DE7;
												}
												case 129:
												{
													float num45;
													float num46;
													float num47;
													if (Main.tile[num37, num38].frameX == 0 || Main.tile[num37, num38].frameX == 54 || Main.tile[num37, num38].frameX == 108)
													{
														num46 = 0.05f;
														num47 = 0.25f;
														num45 = 0f;
													}
													else
													{
														if (Main.tile[num37, num38].frameX == 18 || Main.tile[num37, num38].frameX == 72 || Main.tile[num37, num38].frameX == 126)
														{
															num45 = 0.2f;
															num47 = 0.15f;
															num46 = 0f;
														}
														else
														{
															num47 = 0.2f;
															num45 = 0.1f;
															num46 = 0f;
														}
													}
													if (Lighting.color2[num39, num40] < num45)
													{
														Lighting.color2[num39, num40] = num45 * (float)Main.rand.Next(970, 1031) * 0.001f;
													}
													if (Lighting.colorG2[num39, num40] < num46)
													{
														Lighting.colorG2[num39, num40] = num46 * (float)Main.rand.Next(970, 1031) * 0.001f;
													}
													if (Lighting.colorB2[num39, num40] < num47)
													{
														Lighting.colorB2[num39, num40] = num47 * (float)Main.rand.Next(970, 1031) * 0.001f;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												default:
												{
													if (type != 133)
													{
														goto IL_2DE7;
													}
													break;
												}
											}
										}
										else
										{
											if (type == 140)
											{
												goto IL_1CAE;
											}
											if (type != 149)
											{
												goto IL_2DE7;
											}
											float num45;
											float num46;
											float num47;
											if (Main.tile[num37, num38].frameX == 0)
											{
												num46 = 0.2f;
												num47 = 0.5f;
												num45 = 0.1f;
											}
											else
											{
												if (Main.tile[num37, num38].frameX == 18)
												{
													num45 = 0.5f;
													num47 = 0.1f;
													num46 = 0.1f;
												}
												else
												{
													num47 = 0.1f;
													num45 = 0.2f;
													num46 = 0.5f;
												}
											}
											if (Main.tile[num37, num38].frameX > 36)
											{
												goto IL_2DE7;
											}
											if (Lighting.color2[num39, num40] < num45)
											{
												Lighting.color2[num39, num40] = num45 * (float)Main.rand.Next(970, 1031) * 0.001f;
											}
											if (Lighting.colorG2[num39, num40] < num46)
											{
												Lighting.colorG2[num39, num40] = num46 * (float)Main.rand.Next(970, 1031) * 0.001f;
											}
											if (Lighting.colorB2[num39, num40] < num47)
											{
												Lighting.colorB2[num39, num40] = num47 * (float)Main.rand.Next(970, 1031) * 0.001f;
												goto IL_2DE7;
											}
											goto IL_2DE7;
										}
									}
								}
								if (Lighting.color2[num39, num40] < 0.83f)
								{
									Lighting.color2[num39, num40] = 0.83f;
								}
								if (Lighting.colorG2[num39, num40] < 0.6f)
								{
									Lighting.colorG2[num39, num40] = 0.6f;
								}
								if (Lighting.colorB2[num39, num40] < 0.5f)
								{
									Lighting.colorB2[num39, num40] = 0.5f;
									goto IL_2DE7;
								}
								goto IL_2DE7;
								IL_1CAE:
								if ((double)Lighting.color2[num39, num40] < 0.12)
								{
									Lighting.color2[num39, num40] = 0.12f;
								}
								if ((double)Lighting.colorG2[num39, num40] < 0.07)
								{
									Lighting.colorG2[num39, num40] = 0.07f;
								}
								if ((double)Lighting.colorB2[num39, num40] < 0.32)
								{
									Lighting.colorB2[num39, num40] = 0.32f;
								}
							}
							else
							{
								byte type = Main.tile[num37, num38].type;
								if (type <= 61)
								{
									if (type <= 22)
									{
										if (type != 4)
										{
											if (type != 17)
											{
												if (type != 22)
												{
													goto IL_2DE7;
												}
												if (Lighting.color2[num39, num40] < 0.2f)
												{
													Lighting.color2[num39, num40] = 0.2f;
													goto IL_2DE7;
												}
												goto IL_2DE7;
											}
										}
										else
										{
											if (Main.tile[num37, num38].frameX < 66)
											{
												Lighting.color2[num39, num40] = 1f;
												goto IL_2DE7;
											}
											goto IL_2DE7;
										}
									}
									else
									{
										if (type <= 42)
										{
											switch (type)
											{
												case 26:
												case 31:
												{
													float num55 = (float)Main.rand.Next(-5, 6) * 0.01f;
													if (Lighting.color2[num39, num40] < 0.4f + num55)
													{
														Lighting.color2[num39, num40] = 0.4f + num55;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												case 27:
												case 28:
												case 29:
												case 30:
												case 32:
												{
													goto IL_2DE7;
												}
												case 33:
												{
													if (Main.tile[num37, num38].frameX == 0)
													{
														Lighting.color2[num39, num40] = 1f;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												case 34:
												case 35:
												case 36:
												{
													if (Main.tile[num37, num38].frameX < 54)
													{
														Lighting.color2[num39, num40] = 1f;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												case 37:
												{
													if (Lighting.color2[num39, num40] < 0.5f)
													{
														Lighting.color2[num39, num40] = 0.5f;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												default:
												{
													if (type != 42)
													{
														goto IL_2DE7;
													}
													if (Main.tile[num37, num38].frameX == 0)
													{
														Lighting.color2[num39, num40] = 0.75f;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
											}
										}
										else
										{
											if (type != 49)
											{
												if (type != 61)
												{
													goto IL_2DE7;
												}
												if (Main.tile[num37, num38].frameX == 144 && Lighting.color2[num39, num40] < 0.75f)
												{
													Lighting.color2[num39, num40] = 0.75f;
													goto IL_2DE7;
												}
												goto IL_2DE7;
											}
											else
											{
												if (Lighting.color2[num39, num40] < 0.1f)
												{
													Lighting.color2[num39, num40] = 0.7f;
													goto IL_2DE7;
												}
												goto IL_2DE7;
											}
										}
									}
								}
								else
								{
									if (type <= 84)
									{
										switch (type)
										{
											case 70:
											case 71:
											case 72:
											{
												float num56 = (float)Main.rand.Next(38, 43) * 0.01f;
												if (Lighting.color2[num39, num40] < num56)
												{
													Lighting.color2[num39, num40] = num56;
													goto IL_2DE7;
												}
												goto IL_2DE7;
											}
											default:
											{
												if (type != 77)
												{
													if (type != 84)
													{
														goto IL_2DE7;
													}
													int num57 = (int)(Main.tile[num37, num38].frameX / 18);
													float num58 = 0f;
													if (num57 == 2)
													{
														float num59 = (float)(270 - (int)Main.mouseTextColor);
														num59 /= 500f;
														if (num59 > 1f)
														{
															num59 = 1f;
														}
														if (num59 < 0f)
														{
															num59 = 0f;
														}
														num58 = num59;
													}
													else
													{
														if (num57 == 5)
														{
															num58 = 0.7f;
														}
													}
													if (Lighting.color2[num39, num40] < num58)
													{
														Lighting.color2[num39, num40] = num58;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												else
												{
													if (Lighting.color2[num39, num40] < 0.6f)
													{
														Lighting.color2[num39, num40] = 0.6f;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												break;
											}
										}
									}
									else
									{
										if (type <= 129)
										{
											switch (type)
											{
												case 92:
												{
													if (Main.tile[num37, num38].frameY <= 18 && Main.tile[num37, num38].frameX == 0)
													{
														Lighting.color2[num39, num40] = 1f;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												case 93:
												{
													if (Main.tile[num37, num38].frameY == 0 && Main.tile[num37, num38].frameX == 0)
													{
														Lighting.color2[num39, num40] = 1f;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												case 94:
												case 96:
												case 97:
												case 99:
												{
													goto IL_2DE7;
												}
												case 95:
												{
													if (Main.tile[num37, num38].frameX < 36 && Lighting.color2[num39, num40] < 0.85f)
													{
														Lighting.color2[num39, num40] = 0.9f;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												case 98:
												{
													if (Main.tile[num37, num38].frameY == 0)
													{
														Lighting.color2[num39, num40] = 1f;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												case 100:
												{
													if (Main.tile[num37, num38].frameX < 36)
													{
														Lighting.color2[num39, num40] = 1f;
														goto IL_2DE7;
													}
													goto IL_2DE7;
												}
												default:
												{
													switch (type)
													{
														case 125:
														{
															float num60 = (float)Main.rand.Next(28, 42) * 0.007f;
															num60 += (float)(270 - (int)Main.mouseTextColor) / 900f;
															if ((double)Lighting.color2[num39, num40] < 0.5 * (double)num60)
															{
																Lighting.color2[num39, num40] = 0.3f * num60;
																goto IL_2DE7;
															}
															goto IL_2DE7;
														}
														case 126:
														{
															if (Main.tile[num37, num38].frameX < 36 && Lighting.color2[num39, num40] < 0.3f)
															{
																Lighting.color2[num39, num40] = 0.3f;
																goto IL_2DE7;
															}
															goto IL_2DE7;
														}
														case 127:
														case 128:
														{
															goto IL_2DE7;
														}
														case 129:
														{
															float num61 = 0.08f;
															if (Lighting.color2[num39, num40] < num61)
															{
																Lighting.color2[num39, num40] = num61 * (float)Main.rand.Next(970, 1031) * 0.001f;
																goto IL_2DE7;
															}
															goto IL_2DE7;
														}
														default:
														{
															goto IL_2DE7;
														}
													}
													break;
												}
											}
										}
										else
										{
											if (type != 133)
											{
												if (type != 149)
												{
													goto IL_2DE7;
												}
												if (Main.tile[num37, num38].frameX > 36)
												{
													goto IL_2DE7;
												}
												float num61 = 0.4f;
												if (Lighting.color2[num39, num40] < num61)
												{
													Lighting.color2[num39, num40] = num61 * (float)Main.rand.Next(970, 1031) * 0.001f;
													goto IL_2DE7;
												}
												goto IL_2DE7;
											}
										}
									}
								}
								if (Lighting.color2[num39, num40] < 0.75f)
								{
									Lighting.color2[num39, num40] = 0.75f;
								}
							}
						}
					}
					IL_2DE7:
					if (Main.tile[num37, num38].lava && Main.tile[num37, num38].liquid > 0)
					{
						if (Lighting.RGB)
						{
							float num62 = (float)(Main.tile[num37, num38].liquid / 255) * 0.41f + 0.14f;
							num62 = 0.55f;
							num62 += (float)(270 - (int)Main.mouseTextColor) / 900f;
							if (Lighting.color2[num39, num40] < num62)
							{
								Lighting.color2[num39, num40] = num62;
							}
							if (Lighting.colorG2[num39, num40] < num62)
							{
								Lighting.colorG2[num39, num40] = num62 * 0.6f;
							}
							if (Lighting.colorB2[num39, num40] < num62)
							{
								Lighting.colorB2[num39, num40] = num62 * 0.2f;
							}
						}
						else
						{
							float num63 = (float)(Main.tile[num37, num38].liquid / 255) * 0.38f + 0.08f;
							num63 += (float)(270 - (int)Main.mouseTextColor) / 2000f;
							if (Lighting.color2[num39, num40] < num63)
							{
								Lighting.color2[num39, num40] = num63;
							}
						}
					}
					else
					{
						if (Main.tile[num37, num38].liquid > 128)
						{
							Lighting.wetLight[num39, num40] = true;
						}
					}
					if (Lighting.RGB)
					{
						if (Lighting.color2[num39, num40] > 0f || Lighting.colorG2[num39, num40] > 0f || Lighting.colorB2[num39, num40] > 0f)
						{
							if (Lighting.minX > num39)
							{
								Lighting.minX = num39;
							}
							if (Lighting.maxX < num39 + 1)
							{
								Lighting.maxX = num39 + 1;
							}
							if (Lighting.minY > num40)
							{
								Lighting.minY = num40;
							}
							if (Lighting.maxY < num40 + 1)
							{
								Lighting.maxY = num40 + 1;
							}
						}
					}
					else
					{
						if (Lighting.color2[num39, num40] > 0f)
						{
							if (Lighting.minX > num39)
							{
								Lighting.minX = num39;
							}
							if (Lighting.maxX < num39 + 1)
							{
								Lighting.maxX = num39 + 1;
							}
							if (Lighting.minY > num40)
							{
								Lighting.minY = num40;
							}
							if (Lighting.maxY < num40 + 1)
							{
								Lighting.maxY = num40 + 1;
							}
						}
					}
				}
			}
			if (Main.holyTiles < 0)
			{
				Main.holyTiles = 0;
			}
			if (Main.evilTiles < 0)
			{
				Main.evilTiles = 0;
			}
			int holyTiles = Main.holyTiles;
			Main.holyTiles -= Main.evilTiles;
			Main.evilTiles -= holyTiles;
			if (Main.holyTiles < 0)
			{
				Main.holyTiles = 0;
			}
			if (Main.evilTiles < 0)
			{
				Main.evilTiles = 0;
			}
			Lighting.minX += Lighting.firstToLightX;
			Lighting.maxX += Lighting.firstToLightX;
			Lighting.minY += Lighting.firstToLightY;
			Lighting.maxY += Lighting.firstToLightY;
			Lighting.minX7 = Lighting.minX;
			Lighting.maxX7 = Lighting.maxX;
			Lighting.minY7 = Lighting.minY;
			Lighting.maxY7 = Lighting.maxY;
			Lighting.firstTileX7 = Lighting.firstTileX;
			Lighting.lastTileX7 = Lighting.lastTileX;
			Lighting.lastTileY7 = Lighting.lastTileY;
			Lighting.firstTileY7 = Lighting.firstTileY;
			Lighting.firstToLightX7 = Lighting.firstToLightX;
			Lighting.lastToLightX7 = Lighting.lastToLightX;
			Lighting.firstToLightY7 = Lighting.firstToLightY;
			Lighting.lastToLightY7 = Lighting.lastToLightY;
			Lighting.firstToLightX27 = num;
			Lighting.lastToLightX27 = num3;
			Lighting.firstToLightY27 = num2;
			Lighting.lastToLightY27 = num4;
			Lighting.scrX = (int)Main.screenPosition.X / 16;
			Lighting.scrY = (int)Main.screenPosition.Y / 16;
			Main.renderCount = 0;
			Main.lightTimer[0] = (float)stopwatch.ElapsedMilliseconds;
			Lighting.doColors();
		}
		public static void doColors()
		{
			Stopwatch stopwatch = new Stopwatch();
			if (Lighting.lightMode < 2)
			{
				Lighting.blueWave += (float)Lighting.blueDir * 0.0005f;
				if (Lighting.blueWave > 1f)
				{
					Lighting.blueWave = 1f;
					Lighting.blueDir = -1;
				}
				else
				{
					if (Lighting.blueWave < 0.97f)
					{
						Lighting.blueWave = 0.97f;
						Lighting.blueDir = 1;
					}
				}
				if (Lighting.RGB)
				{
					Lighting.negLight = 0.91f;
					Lighting.negLight2 = 0.56f;
					Lighting.wetLightG = 0.97f * Lighting.negLight * Lighting.blueWave;
					Lighting.wetLightR = 0.88f * Lighting.negLight * Lighting.blueWave;
				}
				else
				{
					Lighting.negLight = 0.9f;
					Lighting.negLight2 = 0.54f;
					Lighting.wetLightR = 0.95f * Lighting.negLight * Lighting.blueWave;
				}
				if (Main.player[Main.myPlayer].nightVision)
				{
					Lighting.negLight *= 1.03f;
					Lighting.negLight2 *= 1.03f;
				}
				if (Main.player[Main.myPlayer].blind)
				{
					Lighting.negLight *= 0.95f;
					Lighting.negLight2 *= 0.95f;
				}
				if (Lighting.RGB)
				{
					if (Main.renderCount == 0)
					{
						stopwatch.Restart();
						for (int i = Lighting.minX7; i < Lighting.maxX7; i++)
						{
							Lighting.lightColor = 0f;
							Lighting.lightColorG = 0f;
							Lighting.lightColorB = 0f;
							Lighting.dirX = 0;
							Lighting.dirY = 1;
							for (int j = Lighting.minY7; j < Lighting.lastToLightY27 + Lighting.maxRenderCount; j++)
							{
								Lighting.LightColor(i, j);
								Lighting.LightColorG(i, j);
								Lighting.LightColorB(i, j);
							}
							Lighting.lightColor = 0f;
							Lighting.lightColorG = 0f;
							Lighting.lightColorB = 0f;
							Lighting.dirX = 0;
							Lighting.dirY = -1;
							for (int k = Lighting.maxY7; k >= Lighting.firstTileY7 - Lighting.maxRenderCount; k--)
							{
								Lighting.LightColor(i, k);
								Lighting.LightColorG(i, k);
								Lighting.LightColorB(i, k);
							}
						}
						Main.lightTimer[1] = (float)stopwatch.ElapsedMilliseconds;
					}
					if (Main.renderCount == 1)
					{
						stopwatch.Restart();
						for (int l = Lighting.firstToLightY7; l < Lighting.lastToLightY7; l++)
						{
							Lighting.lightColor = 0f;
							Lighting.lightColorG = 0f;
							Lighting.lightColorB = 0f;
							Lighting.dirX = 1;
							Lighting.dirY = 0;
							for (int m = Lighting.minX7; m < Lighting.lastTileX7 + Lighting.maxRenderCount; m++)
							{
								Lighting.LightColor(m, l);
								Lighting.LightColorG(m, l);
								Lighting.LightColorB(m, l);
							}
							Lighting.lightColor = 0f;
							Lighting.lightColorG = 0f;
							Lighting.lightColorB = 0f;
							Lighting.dirX = -1;
							Lighting.dirY = 0;
							for (int n = Lighting.maxX7; n >= Lighting.firstTileX7 - Lighting.maxRenderCount; n--)
							{
								Lighting.LightColor(n, l);
								Lighting.LightColorG(n, l);
								Lighting.LightColorB(n, l);
							}
						}
						Main.lightTimer[2] = (float)stopwatch.ElapsedMilliseconds;
					}
					if (Main.renderCount == 1)
					{
						stopwatch.Restart();
						for (int num = Lighting.firstToLightX27; num < Lighting.lastToLightX27; num++)
						{
							Lighting.lightColor = 0f;
							Lighting.lightColorG = 0f;
							Lighting.lightColorB = 0f;
							Lighting.dirX = 0;
							Lighting.dirY = 1;
							for (int num2 = Lighting.firstToLightY27; num2 < Lighting.lastTileY7 + Lighting.maxRenderCount; num2++)
							{
								Lighting.LightColor(num, num2);
								Lighting.LightColorG(num, num2);
								Lighting.LightColorB(num, num2);
							}
							Lighting.lightColor = 0f;
							Lighting.lightColorG = 0f;
							Lighting.lightColorB = 0f;
							Lighting.dirX = 0;
							Lighting.dirY = -1;
							for (int num3 = Lighting.lastToLightY27; num3 >= Lighting.firstTileY7 - Lighting.maxRenderCount; num3--)
							{
								Lighting.LightColor(num, num3);
								Lighting.LightColorG(num, num3);
								Lighting.LightColorB(num, num3);
							}
						}
						Main.lightTimer[3] = (float)stopwatch.ElapsedMilliseconds;
					}
					if (Main.renderCount == 2)
					{
						stopwatch.Restart();
						for (int num4 = Lighting.firstToLightY27; num4 < Lighting.lastToLightY27; num4++)
						{
							Lighting.lightColor = 0f;
							Lighting.lightColorG = 0f;
							Lighting.lightColorB = 0f;
							Lighting.dirX = 1;
							Lighting.dirY = 0;
							for (int num5 = Lighting.firstToLightX27; num5 < Lighting.lastTileX7 + Lighting.maxRenderCount; num5++)
							{
								Lighting.LightColor(num5, num4);
								Lighting.LightColorG(num5, num4);
								Lighting.LightColorB(num5, num4);
							}
							Lighting.lightColor = 0f;
							Lighting.lightColorG = 0f;
							Lighting.lightColorB = 0f;
							Lighting.dirX = -1;
							Lighting.dirY = 0;
							for (int num6 = Lighting.lastToLightX27; num6 >= Lighting.firstTileX7 - Lighting.maxRenderCount; num6--)
							{
								Lighting.LightColor(num6, num4);
								Lighting.LightColorG(num6, num4);
								Lighting.LightColorB(num6, num4);
							}
						}
						Main.lightTimer[4] = (float)stopwatch.ElapsedMilliseconds;
						return;
					}
				}
				else
				{
					if (Main.renderCount == 0)
					{
						stopwatch.Restart();
						for (int num7 = Lighting.minX7; num7 < Lighting.maxX7; num7++)
						{
							Lighting.lightColor = 0f;
							Lighting.dirX = 0;
							Lighting.dirY = 1;
							for (int num8 = Lighting.minY7; num8 < Lighting.lastToLightY27 + Lighting.maxRenderCount; num8++)
							{
								Lighting.LightColor(num7, num8);
							}
							Lighting.lightColor = 0f;
							Lighting.dirX = 0;
							Lighting.dirY = -1;
							for (int num9 = Lighting.maxY7; num9 >= Lighting.firstTileY7 - Lighting.maxRenderCount; num9--)
							{
								Lighting.LightColor(num7, num9);
							}
						}
						Main.lightTimer[1] = (float)stopwatch.ElapsedMilliseconds;
					}
					if (Main.renderCount == 1)
					{
						stopwatch.Restart();
						for (int num10 = Lighting.firstToLightY7; num10 < Lighting.lastToLightY7; num10++)
						{
							Lighting.lightColor = 0f;
							Lighting.dirX = 1;
							Lighting.dirY = 0;
							for (int num11 = Lighting.minX7; num11 < Lighting.lastTileX7 + Lighting.maxRenderCount; num11++)
							{
								Lighting.LightColor(num11, num10);
							}
							Lighting.lightColor = 0f;
							Lighting.dirX = -1;
							Lighting.dirY = 0;
							for (int num12 = Lighting.maxX7; num12 >= Lighting.firstTileX7 - Lighting.maxRenderCount; num12--)
							{
								Lighting.LightColor(num12, num10);
							}
						}
						Main.lightTimer[2] = (float)stopwatch.ElapsedMilliseconds;
					}
					if (Main.renderCount == 1)
					{
						stopwatch.Restart();
						for (int num13 = Lighting.firstToLightX27; num13 < Lighting.lastToLightX27; num13++)
						{
							Lighting.lightColor = 0f;
							Lighting.dirX = 0;
							Lighting.dirY = 1;
							for (int num14 = Lighting.firstToLightY27; num14 < Lighting.lastTileY7 + Lighting.maxRenderCount; num14++)
							{
								Lighting.LightColor(num13, num14);
							}
							Lighting.lightColor = 0f;
							Lighting.dirX = 0;
							Lighting.dirY = -1;
							for (int num15 = Lighting.lastToLightY27; num15 >= Lighting.firstTileY7 - Lighting.maxRenderCount; num15--)
							{
								Lighting.LightColor(num13, num15);
							}
						}
						Main.lightTimer[3] = (float)stopwatch.ElapsedMilliseconds;
					}
					if (Main.renderCount == 2)
					{
						stopwatch.Restart();
						for (int num16 = Lighting.firstToLightY27; num16 < Lighting.lastToLightY27; num16++)
						{
							Lighting.lightColor = 0f;
							Lighting.dirX = 1;
							Lighting.dirY = 0;
							for (int num17 = Lighting.firstToLightX27; num17 < Lighting.lastTileX7 + Lighting.maxRenderCount; num17++)
							{
								Lighting.LightColor(num17, num16);
							}
							Lighting.lightColor = 0f;
							Lighting.dirX = -1;
							Lighting.dirY = 0;
							for (int num18 = Lighting.lastToLightX27; num18 >= Lighting.firstTileX7 - Lighting.maxRenderCount; num18--)
							{
								Lighting.LightColor(num18, num16);
							}
						}
						Main.lightTimer[4] = (float)stopwatch.ElapsedMilliseconds;
						return;
					}
				}
			}
			else
			{
				Lighting.negLight = 0.04f;
				Lighting.negLight2 = 0.16f;
				if (Main.player[Main.myPlayer].nightVision)
				{
					Lighting.negLight -= 0.013f;
					Lighting.negLight2 -= 0.04f;
				}
				if (Main.player[Main.myPlayer].blind)
				{
					Lighting.negLight += 0.03f;
					Lighting.negLight2 += 0.06f;
				}
				Lighting.wetLightR = Lighting.negLight * 1.2f;
				Lighting.wetLightG = Lighting.negLight * 1.1f;
				if (Lighting.RGB)
				{
					if (Main.renderCount == 0)
					{
						stopwatch.Restart();
						for (int num19 = Lighting.minX7; num19 < Lighting.maxX7; num19++)
						{
							Lighting.lightColor = 0f;
							Lighting.lightColorG = 0f;
							Lighting.lightColorB = 0f;
							Lighting.dirX = 0;
							Lighting.dirY = 1;
							for (int num20 = Lighting.minY7; num20 < Lighting.lastToLightY27 + Lighting.maxRenderCount; num20++)
							{
								Lighting.LightColor2(num19, num20);
								Lighting.LightColorG2(num19, num20);
								Lighting.LightColorB2(num19, num20);
							}
							Lighting.lightColor = 0f;
							Lighting.lightColorG = 0f;
							Lighting.lightColorB = 0f;
							Lighting.dirX = 0;
							Lighting.dirY = -1;
							for (int num21 = Lighting.maxY7; num21 >= Lighting.firstTileY7 - Lighting.maxRenderCount; num21--)
							{
								Lighting.LightColor2(num19, num21);
								Lighting.LightColorG2(num19, num21);
								Lighting.LightColorB2(num19, num21);
							}
						}
						Main.lightTimer[1] = (float)stopwatch.ElapsedMilliseconds;
					}
					if (Main.renderCount == 1)
					{
						stopwatch.Restart();
						for (int num22 = Lighting.firstToLightY7; num22 < Lighting.lastToLightY7; num22++)
						{
							Lighting.lightColor = 0f;
							Lighting.lightColorG = 0f;
							Lighting.lightColorB = 0f;
							Lighting.dirX = 1;
							Lighting.dirY = 0;
							for (int num23 = Lighting.minX7; num23 < Lighting.lastTileX7 + Lighting.maxRenderCount; num23++)
							{
								Lighting.LightColor2(num23, num22);
								Lighting.LightColorG2(num23, num22);
								Lighting.LightColorB2(num23, num22);
							}
							Lighting.lightColor = 0f;
							Lighting.lightColorG = 0f;
							Lighting.lightColorB = 0f;
							Lighting.dirX = -1;
							Lighting.dirY = 0;
							for (int num24 = Lighting.maxX7; num24 >= Lighting.firstTileX7 - Lighting.maxRenderCount; num24--)
							{
								Lighting.LightColor2(num24, num22);
								Lighting.LightColorG2(num24, num22);
								Lighting.LightColorB2(num24, num22);
							}
						}
						Main.lightTimer[2] = (float)stopwatch.ElapsedMilliseconds;
					}
					if (Main.renderCount == 1)
					{
						stopwatch.Restart();
						for (int num25 = Lighting.firstToLightX27; num25 < Lighting.lastToLightX27; num25++)
						{
							Lighting.lightColor = 0f;
							Lighting.lightColorG = 0f;
							Lighting.lightColorB = 0f;
							Lighting.dirX = 0;
							Lighting.dirY = 1;
							for (int num26 = Lighting.firstToLightY27; num26 < Lighting.lastTileY7 + Lighting.maxRenderCount; num26++)
							{
								Lighting.LightColor2(num25, num26);
								Lighting.LightColorG2(num25, num26);
								Lighting.LightColorB2(num25, num26);
							}
							Lighting.lightColor = 0f;
							Lighting.lightColorG = 0f;
							Lighting.lightColorB = 0f;
							Lighting.dirX = 0;
							Lighting.dirY = -1;
							for (int num27 = Lighting.lastToLightY27; num27 >= Lighting.firstTileY7 - Lighting.maxRenderCount; num27--)
							{
								Lighting.LightColor2(num25, num27);
								Lighting.LightColorG2(num25, num27);
								Lighting.LightColorB2(num25, num27);
							}
						}
						Main.lightTimer[3] = (float)stopwatch.ElapsedMilliseconds;
					}
					if (Main.renderCount == 2)
					{
						stopwatch.Restart();
						for (int num28 = Lighting.firstToLightY27; num28 < Lighting.lastToLightY27; num28++)
						{
							Lighting.lightColor = 0f;
							Lighting.lightColorG = 0f;
							Lighting.lightColorB = 0f;
							Lighting.dirX = 1;
							Lighting.dirY = 0;
							for (int num29 = Lighting.firstToLightX27; num29 < Lighting.lastTileX7 + Lighting.maxRenderCount; num29++)
							{
								Lighting.LightColor2(num29, num28);
								Lighting.LightColorG2(num29, num28);
								Lighting.LightColorB2(num29, num28);
							}
							Lighting.lightColor = 0f;
							Lighting.lightColorG = 0f;
							Lighting.lightColorB = 0f;
							Lighting.dirX = -1;
							Lighting.dirY = 0;
							for (int num30 = Lighting.lastToLightX27; num30 >= Lighting.firstTileX7 - Lighting.maxRenderCount; num30--)
							{
								Lighting.LightColor2(num30, num28);
								Lighting.LightColorG2(num30, num28);
								Lighting.LightColorB2(num30, num28);
							}
						}
						Main.lightTimer[4] = (float)stopwatch.ElapsedMilliseconds;
						return;
					}
				}
				else
				{
					if (Main.renderCount == 0)
					{
						stopwatch.Restart();
						for (int num31 = Lighting.minX7; num31 < Lighting.maxX7; num31++)
						{
							Lighting.lightColor = 0f;
							Lighting.dirX = 0;
							Lighting.dirY = 1;
							for (int num32 = Lighting.minY7; num32 < Lighting.lastToLightY27 + Lighting.maxRenderCount; num32++)
							{
								Lighting.LightColor2(num31, num32);
							}
							Lighting.lightColor = 0f;
							Lighting.dirX = 0;
							Lighting.dirY = -1;
							for (int num33 = Lighting.maxY7; num33 >= Lighting.firstTileY7 - Lighting.maxRenderCount; num33--)
							{
								Lighting.LightColor2(num31, num33);
							}
						}
						Main.lightTimer[1] = (float)stopwatch.ElapsedMilliseconds;
					}
					if (Main.renderCount == 1)
					{
						stopwatch.Restart();
						for (int num34 = Lighting.firstToLightY7; num34 < Lighting.lastToLightY7; num34++)
						{
							Lighting.lightColor = 0f;
							Lighting.dirX = 1;
							Lighting.dirY = 0;
							for (int num35 = Lighting.minX7; num35 < Lighting.lastTileX7 + Lighting.maxRenderCount; num35++)
							{
								Lighting.LightColor2(num35, num34);
							}
							Lighting.lightColor = 0f;
							Lighting.dirX = -1;
							Lighting.dirY = 0;
							for (int num36 = Lighting.maxX7; num36 >= Lighting.firstTileX7 - Lighting.maxRenderCount; num36--)
							{
								Lighting.LightColor2(num36, num34);
							}
						}
						Main.lightTimer[2] = (float)stopwatch.ElapsedMilliseconds;
					}
					if (Main.renderCount == 1)
					{
						stopwatch.Restart();
						for (int num37 = Lighting.firstToLightX27; num37 < Lighting.lastToLightX27; num37++)
						{
							Lighting.lightColor = 0f;
							Lighting.dirX = 0;
							Lighting.dirY = 1;
							for (int num38 = Lighting.firstToLightY27; num38 < Lighting.lastTileY7 + Lighting.maxRenderCount; num38++)
							{
								Lighting.LightColor2(num37, num38);
							}
							Lighting.lightColor = 0f;
							Lighting.dirX = 0;
							Lighting.dirY = -1;
							for (int num39 = Lighting.lastToLightY27; num39 >= Lighting.firstTileY7 - Lighting.maxRenderCount; num39--)
							{
								Lighting.LightColor2(num37, num39);
							}
						}
						Main.lightTimer[3] = (float)stopwatch.ElapsedMilliseconds;
					}
					if (Main.renderCount == 2)
					{
						stopwatch.Restart();
						for (int num40 = Lighting.firstToLightY27; num40 < Lighting.lastToLightY27; num40++)
						{
							Lighting.lightColor = 0f;
							Lighting.dirX = 1;
							Lighting.dirY = 0;
							for (int num41 = Lighting.firstToLightX27; num41 < Lighting.lastTileX7 + Lighting.maxRenderCount; num41++)
							{
								Lighting.LightColor2(num41, num40);
							}
							Lighting.lightColor = 0f;
							Lighting.dirX = -1;
							Lighting.dirY = 0;
							for (int num42 = Lighting.lastToLightX27; num42 >= Lighting.firstTileX7 - Lighting.maxRenderCount; num42--)
							{
								Lighting.LightColor2(num42, num40);
							}
						}
						Main.lightTimer[4] = (float)stopwatch.ElapsedMilliseconds;
					}
				}
			}
		}
		public static void addLight(int i, int j, float Lightness)
		{
			if (Main.netMode == 2)
			{
				return;
			}
			if (i - Lighting.firstTileX + Lighting.offScreenTiles >= 0 && i - Lighting.firstTileX + Lighting.offScreenTiles < Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10 && j - Lighting.firstTileY + Lighting.offScreenTiles >= 0 && j - Lighting.firstTileY + Lighting.offScreenTiles < Main.screenHeight / 16 + Lighting.offScreenTiles * 2 + 10)
			{
				if (Lighting.tempLightCount == Lighting.maxTempLights)
				{
					return;
				}
				if (!Lighting.RGB)
				{
					for (int k = 0; k < Lighting.tempLightCount; k++)
					{
						if (Lighting.tempLightX[k] == i && Lighting.tempLightY[k] == j && Lightness <= Lighting.tempLight[k])
						{
							return;
						}
					}
					Lighting.tempLightX[Lighting.tempLightCount] = i;
					Lighting.tempLightY[Lighting.tempLightCount] = j;
					Lighting.tempLight[Lighting.tempLightCount] = Lightness;
					Lighting.tempLightG[Lighting.tempLightCount] = Lightness;
					Lighting.tempLightB[Lighting.tempLightCount] = Lightness;
					Lighting.tempLightCount++;
					return;
				}
				Lighting.tempLight[Lighting.tempLightCount] = Lightness;
				Lighting.tempLightG[Lighting.tempLightCount] = Lightness;
				Lighting.tempLightB[Lighting.tempLightCount] = Lightness;
				Lighting.tempLightX[Lighting.tempLightCount] = i;
				Lighting.tempLightY[Lighting.tempLightCount] = j;
				Lighting.tempLightCount++;
			}
		}
		public static void addLight(int i, int j, float R, float G, float B)
		{
			if (Main.netMode == 2)
			{
				return;
			}
			if (!Lighting.RGB)
			{
				Lighting.addLight(i, j, (R + G + B) / 3f);
			}
			if (i - Lighting.firstTileX + Lighting.offScreenTiles >= 0 && i - Lighting.firstTileX + Lighting.offScreenTiles < Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10 && j - Lighting.firstTileY + Lighting.offScreenTiles >= 0 && j - Lighting.firstTileY + Lighting.offScreenTiles < Main.screenHeight / 16 + Lighting.offScreenTiles * 2 + 10)
			{
				if (Lighting.tempLightCount == Lighting.maxTempLights)
				{
					return;
				}
				for (int k = 0; k < Lighting.tempLightCount; k++)
				{
					if (Lighting.tempLightX[k] == i && Lighting.tempLightY[k] == j)
					{
						if (Lighting.tempLight[k] < R)
						{
							Lighting.tempLight[k] = R;
						}
						if (Lighting.tempLightG[k] < G)
						{
							Lighting.tempLightG[k] = G;
						}
						if (Lighting.tempLightB[k] < B)
						{
							Lighting.tempLightB[k] = B;
						}
						return;
					}
				}
				Lighting.tempLight[Lighting.tempLightCount] = R;
				Lighting.tempLightG[Lighting.tempLightCount] = G;
				Lighting.tempLightB[Lighting.tempLightCount] = B;
				Lighting.tempLightX[Lighting.tempLightCount] = i;
				Lighting.tempLightY[Lighting.tempLightCount] = j;
				Lighting.tempLightCount++;
			}
		}
		private static void ResetRange()
		{
			Lighting.minX = Main.screenWidth + Lighting.offScreenTiles * 2 + 10;
			Lighting.maxX = 0;
			Lighting.minY = Main.screenHeight + Lighting.offScreenTiles * 2 + 10;
			Lighting.maxY = 0;
		}
		private static void LightColor(int i, int j)
		{
			int num = i - Lighting.firstToLightX7;
			int num2 = j - Lighting.firstToLightY7;
			if (Lighting.color2[num, num2] > Lighting.lightColor)
			{
				Lighting.lightColor = Lighting.color2[num, num2];
			}
			else
			{
				if ((double)Lighting.lightColor <= 0.0185)
				{
					return;
				}
				if (Lighting.color2[num, num2] < Lighting.lightColor)
				{
					Lighting.color2[num, num2] = Lighting.lightColor;
				}
			}
			if (Lighting.color2[num + Lighting.dirX, num2 + Lighting.dirY] > Lighting.lightColor)
			{
				return;
			}
			if (Lighting.stopLight[num, num2])
			{
				Lighting.lightColor *= Lighting.negLight2;
				return;
			}
			if (Lighting.wetLight[num, num2])
			{
				Lighting.lightColor *= Lighting.wetLightR * (float)Main.rand.Next(98, 100) * 0.01f;
				return;
			}
			Lighting.lightColor *= Lighting.negLight;
		}
		private static void LightColorG(int i, int j)
		{
			int num = i - Lighting.firstToLightX7;
			int num2 = j - Lighting.firstToLightY7;
			if (Lighting.colorG2[num, num2] > Lighting.lightColorG)
			{
				Lighting.lightColorG = Lighting.colorG2[num, num2];
			}
			else
			{
				if ((double)Lighting.lightColorG <= 0.0185)
				{
					return;
				}
				Lighting.colorG2[num, num2] = Lighting.lightColorG;
			}
			if (Lighting.colorG2[num + Lighting.dirX, num2 + Lighting.dirY] > Lighting.lightColorG)
			{
				return;
			}
			if (Lighting.stopLight[num, num2])
			{
				Lighting.lightColorG *= Lighting.negLight2;
				return;
			}
			if (Lighting.wetLight[num, num2])
			{
				Lighting.lightColorG *= Lighting.wetLightG * (float)Main.rand.Next(97, 100) * 0.01f;
				return;
			}
			Lighting.lightColorG *= Lighting.negLight;
		}
		private static void LightColorB(int i, int j)
		{
			int num = i - Lighting.firstToLightX7;
			int num2 = j - Lighting.firstToLightY7;
			if (Lighting.colorB2[num, num2] > Lighting.lightColorB)
			{
				Lighting.lightColorB = Lighting.colorB2[num, num2];
			}
			else
			{
				if ((double)Lighting.lightColorB <= 0.0185)
				{
					return;
				}
				Lighting.colorB2[num, num2] = Lighting.lightColorB;
			}
			if (Lighting.colorB2[num + Lighting.dirX, num2 + Lighting.dirY] >= Lighting.lightColorB)
			{
				return;
			}
			if (Lighting.stopLight[num, num2])
			{
				Lighting.lightColorB *= Lighting.negLight2;
				return;
			}
			Lighting.lightColorB *= Lighting.negLight;
		}
		private static void LightColor2(int i, int j)
		{
			int num = i - Lighting.firstToLightX7;
			int num2 = j - Lighting.firstToLightY7;
			try
			{
				if (Lighting.color2[num, num2] > Lighting.lightColor)
				{
					Lighting.lightColor = Lighting.color2[num, num2];
				}
				else
				{
					if (Lighting.lightColor <= 0f)
					{
						return;
					}
					Lighting.color2[num, num2] = Lighting.lightColor;
				}
				if (Main.tile[i, j].active && Main.tileBlockLight[(int)Main.tile[i, j].type])
				{
					Lighting.lightColor -= Lighting.negLight2;
				}
				else
				{
					if (Lighting.wetLight[num, num2])
					{
						Lighting.lightColor -= Lighting.wetLightR;
					}
					else
					{
						Lighting.lightColor -= Lighting.negLight;
					}
				}
			}
			catch
			{
			}
		}
		private static void LightColorG2(int i, int j)
		{
			int num = i - Lighting.firstToLightX7;
			int num2 = j - Lighting.firstToLightY7;
			try
			{
				if (Lighting.colorG2[num, num2] > Lighting.lightColorG)
				{
					Lighting.lightColorG = Lighting.colorG2[num, num2];
				}
				else
				{
					if (Lighting.lightColorG <= 0f)
					{
						return;
					}
					Lighting.colorG2[num, num2] = Lighting.lightColorG;
				}
				if (Main.tile[i, j].active && Main.tileBlockLight[(int)Main.tile[i, j].type])
				{
					Lighting.lightColorG -= Lighting.negLight2;
				}
				else
				{
					if (Lighting.wetLight[num, num2])
					{
						Lighting.lightColorG -= Lighting.wetLightG;
					}
					else
					{
						Lighting.lightColorG -= Lighting.negLight;
					}
				}
			}
			catch
			{
			}
		}
		private static void LightColorB2(int i, int j)
		{
			int num = i - Lighting.firstToLightX7;
			int num2 = j - Lighting.firstToLightY7;
			try
			{
				if (Lighting.colorB2[num, num2] > Lighting.lightColorB)
				{
					Lighting.lightColorB = Lighting.colorB2[num, num2];
				}
				else
				{
					if (Lighting.lightColorB <= 0f)
					{
						return;
					}
					Lighting.colorB2[num, num2] = Lighting.lightColorB;
				}
				if (Main.tile[i, j].active && Main.tileBlockLight[(int)Main.tile[i, j].type])
				{
					Lighting.lightColorB -= Lighting.negLight2;
				}
				else
				{
					Lighting.lightColorB -= Lighting.negLight;
				}
			}
			catch
			{
			}
		}
		public static int LightingX(int lightX)
		{
			if (lightX < 0)
			{
				return 0;
			}
			if (lightX >= Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10)
			{
				return Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10 - 1;
			}
			return lightX;
		}
		public static int LightingY(int lightY)
		{
			if (lightY < 0)
			{
				return 0;
			}
			if (lightY >= Main.screenHeight / 16 + Lighting.offScreenTiles * 2 + 10)
			{
				return Main.screenHeight / 16 + Lighting.offScreenTiles * 2 + 10 - 1;
			}
			return lightY;
		}
		public static Color GetColor(int x, int y, Color oldColor)
		{
			int num = x - Lighting.firstTileX + Lighting.offScreenTiles;
			int num2 = y - Lighting.firstTileY + Lighting.offScreenTiles;
			if (Main.gameMenu)
			{
				return oldColor;
			}
			if (num < 0 || num2 < 0 || num >= Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10 || num2 >= Main.screenHeight / 16 + Lighting.offScreenTiles * 2 + 10)
			{
				return Color.Black;
			}
			Color white = Color.White;
			int num3 = (int)((float)oldColor.R * Lighting.color[num, num2] * Lighting.brightness);
			int num4 = (int)((float)oldColor.G * Lighting.colorG[num, num2] * Lighting.brightness);
			int num5 = (int)((float)oldColor.B * Lighting.colorB[num, num2] * Lighting.brightness);
			if (num3 > 255)
			{
				num3 = 255;
			}
			if (num4 > 255)
			{
				num4 = 255;
			}
			if (num5 > 255)
			{
				num5 = 255;
			}
			white.R = (byte)num3;
			white.G = (byte)num4;
			white.B = (byte)num5;
			return white;
		}
		public static Color GetColor(int x, int y)
		{
			int num = x - Lighting.firstTileX + Lighting.offScreenTiles;
			int num2 = y - Lighting.firstTileY + Lighting.offScreenTiles;
			if (num < 0 || num2 < 0 || num >= Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10 || num2 >= Main.screenHeight / 16 + Lighting.offScreenTiles * 2)
			{
				return Color.Black;
			}
			int num3 = (int)(255f * Lighting.color[num, num2] * Lighting.brightness);
			int num4 = (int)(255f * Lighting.colorG[num, num2] * Lighting.brightness);
			int num5 = (int)(255f * Lighting.colorB[num, num2] * Lighting.brightness);
			if (num3 > 255)
			{
				num3 = 255;
			}
			if (num4 > 255)
			{
				num4 = 255;
			}
			if (num5 > 255)
			{
				num5 = 255;
			}
			Color result = new Color((int)((byte)num3), (int)((byte)num4), (int)((byte)num5), 255);
			return result;
		}
		public static Color GetBlackness(int x, int y)
		{
			int num = x - Lighting.firstTileX + Lighting.offScreenTiles;
			int num2 = y - Lighting.firstTileY + Lighting.offScreenTiles;
			if (num < 0 || num2 < 0 || num >= Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10 || num2 >= Main.screenHeight / 16 + Lighting.offScreenTiles * 2 + 10)
			{
				return Color.Black;
			}
			Color result = new Color(0, 0, 0, (int)((byte)(255f - 255f * Lighting.color[num, num2])));
			return result;
		}
		public static float Brightness(int x, int y)
		{
			int num = x - Lighting.firstTileX + Lighting.offScreenTiles;
			int num2 = y - Lighting.firstTileY + Lighting.offScreenTiles;
			if (num < 0 || num2 < 0 || num >= Main.screenWidth / 16 + Lighting.offScreenTiles * 2 + 10 || num2 >= Main.screenHeight / 16 + Lighting.offScreenTiles * 2 + 10)
			{
				return 0f;
			}
			return (Lighting.color[num, num2] + Lighting.colorG[num, num2] + Lighting.colorB[num, num2]) / 3f;
		}
		public static bool Brighter(int x, int y, int x2, int y2)
		{
			int num = x - Lighting.firstTileX + Lighting.offScreenTiles;
			int num2 = y - Lighting.firstTileY + Lighting.offScreenTiles;
			int num3 = x2 - Lighting.firstTileX + Lighting.offScreenTiles;
			int num4 = y2 - Lighting.firstTileY + Lighting.offScreenTiles;
			bool result;
			try
			{
				if (Lighting.color[num, num2] + Lighting.colorG[num, num2] + Lighting.colorB[num, num2] >= Lighting.color[num3, num4] + Lighting.colorG[num3, num4] + Lighting.colorB[num3, num4])
				{
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
	}
}
