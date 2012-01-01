
using System;
namespace Terraria
{
	public class Dust
	{
		public Vector2 position;
		public Vector2 velocity;
		public static int lavaBubbles;
		public float fadeIn;
		public bool noGravity;
		public float scale;
		public float rotation;
		public bool noLight;
		public bool active;
		public int type;
		public Color color;
		public int alpha;
		public Rectangle frame;
		public static int NewDust(Vector2 Position, int Width, int Height, int Type, float SpeedX = 0f, float SpeedY = 0f, int Alpha = 0, Color newColor = default(Color), float Scale = 1f)
		{
			if (Main.gameMenu)
			{
				return 0;
			}
			if (Main.rand == null)
			{
				Main.rand = new Random((int)DateTime.Now.Ticks);
			}
			if (Main.gamePaused)
			{
				return 0;
			}
			if (WorldGen.gen)
			{
				return 0;
			}
			if (Main.netMode == 2)
			{
				return 0;
			}
			Rectangle rectangle = new Rectangle((int)(Main.player[Main.myPlayer].position.X - (float)(Main.screenWidth / 2) - 100f), (int)(Main.player[Main.myPlayer].position.Y - (float)(Main.screenHeight / 2) - 100f), Main.screenWidth + 200, Main.screenHeight + 200);
			Rectangle value = new Rectangle((int)Position.X, (int)Position.Y, 10, 10);
			if (!rectangle.Intersects(value))
			{
				return 2000;
			}
			int result = 2000;
			int i = 0;
			while (i < 2000)
			{
				if (!Main.dust[i].active)
				{
					int num = Width;
					int num2 = Height;
					if (num < 5)
					{
						num = 5;
					}
					if (num2 < 5)
					{
						num2 = 5;
					}
					result = i;
					Main.dust[i].fadeIn = 0f;
					Main.dust[i].active = true;
					Main.dust[i].type = Type;
					Main.dust[i].noGravity = false;
					Main.dust[i].color = newColor;
					Main.dust[i].alpha = Alpha;
					Main.dust[i].position.X = Position.X + (float)Main.rand.Next(num - 4) + 4f;
					Main.dust[i].position.Y = Position.Y + (float)Main.rand.Next(num2 - 4) + 4f;
					Main.dust[i].velocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + SpeedX;
					Main.dust[i].velocity.Y = (float)Main.rand.Next(-20, 21) * 0.1f + SpeedY;
					Main.dust[i].frame.X = 10 * Type;
					Main.dust[i].frame.Y = 10 * Main.rand.Next(3);
					Main.dust[i].frame.Width = 8;
					Main.dust[i].frame.Height = 8;
					Main.dust[i].rotation = 0f;
					Main.dust[i].scale = 1f + (float)Main.rand.Next(-20, 21) * 0.01f;
					Main.dust[i].scale *= Scale;
					Main.dust[i].noLight = false;
					if (Main.dust[i].type == 6 || Main.dust[i].type == 75 || Main.dust[i].type == 29 || (Main.dust[i].type >= 59 && Main.dust[i].type <= 65))
					{
						Main.dust[i].velocity.Y = (float)Main.rand.Next(-10, 6) * 0.1f;
						Dust expr_341_cp_0 = Main.dust[i];
						expr_341_cp_0.velocity.X = expr_341_cp_0.velocity.X * 0.3f;
						Main.dust[i].scale *= 0.7f;
					}
					if (Main.dust[i].type == 33 || Main.dust[i].type == 52)
					{
						Main.dust[i].alpha = 170;
						Dust expr_3A2 = Main.dust[i];
						expr_3A2.velocity *= 0.5f;
						Dust expr_3C3_cp_0 = Main.dust[i];
						expr_3C3_cp_0.velocity.Y = expr_3C3_cp_0.velocity.Y + 1f;
					}
					if (Main.dust[i].type == 41)
					{
						Dust expr_3EB = Main.dust[i];
						expr_3EB.velocity *= 0f;
					}
					if (Main.dust[i].type != 34 && Main.dust[i].type != 35)
					{
						break;
					}
					Dust expr_42A = Main.dust[i];
					expr_42A.velocity *= 0.1f;
					Main.dust[i].velocity.Y = -0.5f;
					if (Main.dust[i].type == 34 && !Collision.WetCollision(new Vector2(Main.dust[i].position.X, Main.dust[i].position.Y - 8f), 4, 4))
					{
						Main.dust[i].active = false;
						break;
					}
					break;
				}
				else
				{
					i++;
				}
			}
			return result;
		}
		public static void UpdateDust()
		{
			Dust.lavaBubbles = 0;
			Main.snowDust = 0;
			for (int i = 0; i < 2000; i++)
			{
				if (i < Main.numDust)
				{
					if (Main.dust[i].active)
					{
						if (Main.dust[i].type == 35)
						{
							Dust.lavaBubbles++;
						}
						Dust expr_52 = Main.dust[i];
						expr_52.position += Main.dust[i].velocity;
						if (Main.dust[i].type == 6 || Main.dust[i].type == 75 || Main.dust[i].type == 29 || (Main.dust[i].type >= 59 && Main.dust[i].type <= 65))
						{
							if (!Main.dust[i].noGravity)
							{
								Dust expr_DD_cp_0 = Main.dust[i];
								expr_DD_cp_0.velocity.Y = expr_DD_cp_0.velocity.Y + 0.05f;
							}
							if (!Main.dust[i].noLight)
							{
								float num = Main.dust[i].scale * 1.4f;
								if (Main.dust[i].type == 29)
								{
									if (num > 1f)
									{
										num = 1f;
									}
									Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num * 0.1f, num * 0.4f, num);
								}
								if (Main.dust[i].type == 75)
								{
									if (num > 1f)
									{
										num = 1f;
									}
									Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num * 0.7f, num, num * 0.2f);
								}
								else
								{
									if (Main.dust[i].type >= 59 && Main.dust[i].type <= 65)
									{
										if (num > 0.8f)
										{
											num = 0.8f;
										}
										int num2 = Main.dust[i].type - 58;
										float num3 = 1f;
										float num4 = 1f;
										float num5 = 1f;
										if (num2 == 1)
										{
											num3 = 0f;
											num4 = 0.1f;
											num5 = 1.3f;
										}
										else
										{
											if (num2 == 2)
											{
												num3 = 1f;
												num4 = 0.1f;
												num5 = 0.1f;
											}
											else
											{
												if (num2 == 3)
												{
													num3 = 0f;
													num4 = 1f;
													num5 = 0.1f;
												}
												else
												{
													if (num2 == 4)
													{
														num3 = 0.9f;
														num4 = 0f;
														num5 = 0.9f;
													}
													else
													{
														if (num2 == 5)
														{
															num3 = 1.3f;
															num4 = 1.3f;
															num5 = 1.3f;
														}
														else
														{
															if (num2 == 6)
															{
																num3 = 0.9f;
																num4 = 0.9f;
																num5 = 0f;
															}
															else
															{
																if (num2 == 7)
																{
																	num3 = 0.5f * Main.demonTorch + 1f * (1f - Main.demonTorch);
																	num4 = 0.3f;
																	num5 = 1f * Main.demonTorch + 0.5f * (1f - Main.demonTorch);
																}
															}
														}
													}
												}
											}
										}
										Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num * num3, num * num4, num * num5);
									}
									else
									{
										if (num > 0.6f)
										{
											num = 0.6f;
										}
										Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num, num * 0.65f, num * 0.4f);
									}
								}
							}
						}
						else
						{
							if (Main.dust[i].type == 14 || Main.dust[i].type == 16 || Main.dust[i].type == 31 || Main.dust[i].type == 46)
							{
								Dust expr_40B_cp_0 = Main.dust[i];
								expr_40B_cp_0.velocity.Y = expr_40B_cp_0.velocity.Y * 0.98f;
								Dust expr_428_cp_0 = Main.dust[i];
								expr_428_cp_0.velocity.X = expr_428_cp_0.velocity.X * 0.98f;
								if (Main.dust[i].type == 31 && Main.dust[i].noGravity)
								{
									Dust expr_464 = Main.dust[i];
									expr_464.velocity *= 1.02f;
									Main.dust[i].scale += 0.02f;
									Main.dust[i].alpha += 4;
									if (Main.dust[i].alpha > 255)
									{
										Main.dust[i].scale = 0.0001f;
										Main.dust[i].alpha = 255;
									}
								}
							}
							else
							{
								if (Main.dust[i].type == 32)
								{
									Main.dust[i].scale -= 0.01f;
									Dust expr_516_cp_0 = Main.dust[i];
									expr_516_cp_0.velocity.X = expr_516_cp_0.velocity.X * 0.96f;
									Dust expr_533_cp_0 = Main.dust[i];
									expr_533_cp_0.velocity.Y = expr_533_cp_0.velocity.Y + 0.1f;
								}
								else
								{
									if (Main.dust[i].type == 43)
									{
										Main.dust[i].rotation += 0.1f * Main.dust[i].scale;
										Color color = Lighting.GetColor((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f));
										float num6 = (float)color.R / 270f;
										float num7 = (float)color.G / 270f;
										float num8 = (float)color.B / 270f;
										num6 *= Main.dust[i].scale * 1.07f;
										num7 *= Main.dust[i].scale * 1.07f;
										num8 *= Main.dust[i].scale * 1.07f;
										if (Main.dust[i].alpha < 255)
										{
											Main.dust[i].scale += 0.09f;
											if (Main.dust[i].scale >= 1f)
											{
												Main.dust[i].scale = 1f;
												Main.dust[i].alpha = 255;
											}
										}
										else
										{
											if ((double)Main.dust[i].scale < 0.8)
											{
												Main.dust[i].scale -= 0.01f;
											}
											if ((double)Main.dust[i].scale < 0.5)
											{
												Main.dust[i].scale -= 0.01f;
											}
										}
										if ((double)num6 < 0.05 && (double)num7 < 0.05 && (double)num8 < 0.05)
										{
											Main.dust[i].active = false;
										}
										else
										{
											Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num6, num7, num8);
										}
									}
									else
									{
										if (Main.dust[i].type == 15 || Main.dust[i].type == 57 || Main.dust[i].type == 58)
										{
											Dust expr_7AD_cp_0 = Main.dust[i];
											expr_7AD_cp_0.velocity.Y = expr_7AD_cp_0.velocity.Y * 0.98f;
											Dust expr_7CA_cp_0 = Main.dust[i];
											expr_7CA_cp_0.velocity.X = expr_7CA_cp_0.velocity.X * 0.98f;
											float num9 = Main.dust[i].scale;
											if (Main.dust[i].type != 15)
											{
												num9 = Main.dust[i].scale * 0.8f;
											}
											if (Main.dust[i].noLight)
											{
												Dust expr_822 = Main.dust[i];
												expr_822.velocity *= 0.95f;
											}
											if (num9 > 1f)
											{
												num9 = 1f;
											}
											if (Main.dust[i].type == 15)
											{
												Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num9 * 0.45f, num9 * 0.55f, num9);
											}
											else
											{
												if (Main.dust[i].type == 57)
												{
													Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num9 * 0.95f, num9 * 0.95f, num9 * 0.45f);
												}
												else
												{
													if (Main.dust[i].type == 58)
													{
														Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num9, num9 * 0.55f, num9 * 0.75f);
													}
												}
											}
										}
										else
										{
											if (Main.dust[i].type == 66)
											{
												if (Main.dust[i].velocity.X < 0f)
												{
													Main.dust[i].rotation -= 1f;
												}
												else
												{
													Main.dust[i].rotation += 1f;
												}
												Dust expr_9CD_cp_0 = Main.dust[i];
												expr_9CD_cp_0.velocity.Y = expr_9CD_cp_0.velocity.Y * 0.98f;
												Dust expr_9EA_cp_0 = Main.dust[i];
												expr_9EA_cp_0.velocity.X = expr_9EA_cp_0.velocity.X * 0.98f;
												Main.dust[i].scale += 0.02f;
												float num10 = Main.dust[i].scale;
												if (Main.dust[i].type != 15)
												{
													num10 = Main.dust[i].scale * 0.8f;
												}
												if (num10 > 1f)
												{
													num10 = 1f;
												}
												Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num10 * ((float)Main.dust[i].color.R / 255f), num10 * ((float)Main.dust[i].color.G / 255f), num10 * ((float)Main.dust[i].color.B / 255f));
											}
											else
											{
												if (Main.dust[i].type == 20 || Main.dust[i].type == 21)
												{
													Main.dust[i].scale += 0.005f;
													Dust expr_B27_cp_0 = Main.dust[i];
													expr_B27_cp_0.velocity.Y = expr_B27_cp_0.velocity.Y * 0.94f;
													Dust expr_B44_cp_0 = Main.dust[i];
													expr_B44_cp_0.velocity.X = expr_B44_cp_0.velocity.X * 0.94f;
													float num11 = Main.dust[i].scale * 0.8f;
													if (num11 > 1f)
													{
														num11 = 1f;
													}
													if (Main.dust[i].type == 21)
													{
														num11 = Main.dust[i].scale * 0.4f;
														Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num11 * 0.8f, num11 * 0.3f, num11);
													}
													else
													{
														Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num11 * 0.3f, num11 * 0.6f, num11);
													}
												}
												else
												{
													if (Main.dust[i].type == 27 || Main.dust[i].type == 45)
													{
														Dust expr_C5F = Main.dust[i];
														expr_C5F.velocity *= 0.94f;
														Main.dust[i].scale += 0.002f;
														float num12 = Main.dust[i].scale;
														if (Main.dust[i].noLight)
														{
															num12 *= 0.1f;
															Main.dust[i].scale -= 0.06f;
															if (Main.dust[i].scale < 1f)
															{
																Main.dust[i].scale -= 0.06f;
															}
															if (Main.player[Main.myPlayer].wet)
															{
																Dust expr_D11 = Main.dust[i];
																expr_D11.position += Main.player[Main.myPlayer].velocity * 0.5f;
															}
															else
															{
																Dust expr_D44 = Main.dust[i];
																expr_D44.position += Main.player[Main.myPlayer].velocity;
															}
														}
														if (num12 > 1f)
														{
															num12 = 1f;
														}
														Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num12 * 0.6f, num12 * 0.2f, num12);
													}
													else
													{
														if (Main.dust[i].type == 55 || Main.dust[i].type == 56 || Main.dust[i].type == 73 || Main.dust[i].type == 74)
														{
															Dust expr_E0A = Main.dust[i];
															expr_E0A.velocity *= 0.98f;
															float num13 = Main.dust[i].scale * 0.8f;
															if (Main.dust[i].type == 55)
															{
																if (num13 > 1f)
																{
																	num13 = 1f;
																}
																Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num13, num13, num13 * 0.6f);
															}
															else
															{
																if (Main.dust[i].type == 73)
																{
																	if (num13 > 1f)
																	{
																		num13 = 1f;
																	}
																	Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num13, num13 * 0.35f, num13 * 0.5f);
																}
																else
																{
																	if (Main.dust[i].type == 74)
																	{
																		if (num13 > 1f)
																		{
																			num13 = 1f;
																		}
																		Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num13 * 0.35f, num13, num13 * 0.5f);
																	}
																	else
																	{
																		num13 = Main.dust[i].scale * 1.2f;
																		if (num13 > 1f)
																		{
																			num13 = 1f;
																		}
																		Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num13 * 0.35f, num13 * 0.5f, num13);
																	}
																}
															}
														}
														else
														{
															if (Main.dust[i].type == 71 || Main.dust[i].type == 72)
															{
																Dust expr_100B = Main.dust[i];
																expr_100B.velocity *= 0.98f;
																float num14 = Main.dust[i].scale;
																if (num14 > 1f)
																{
																	num14 = 1f;
																}
																Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num14 * 0.2f, 0f, num14 * 0.1f);
															}
															else
															{
																if (Main.dust[i].type == 76)
																{
																	Main.snowDust++;
																	Main.dust[i].scale += 0.009f;
																	Dust expr_10C8 = Main.dust[i];
																	expr_10C8.position += Main.player[Main.myPlayer].velocity * 0.2f;
																}
																else
																{
																	if (!Main.dust[i].noGravity && Main.dust[i].type != 41 && Main.dust[i].type != 44)
																	{
																		Dust expr_112E_cp_0 = Main.dust[i];
																		expr_112E_cp_0.velocity.Y = expr_112E_cp_0.velocity.Y + 0.1f;
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
						if (Main.dust[i].type == 5 && Main.dust[i].noGravity)
						{
							Main.dust[i].scale -= 0.04f;
						}
						if (Main.dust[i].type == 33 || Main.dust[i].type == 52)
						{
							if (Main.dust[i].velocity.X == 0f)
							{
								if (Collision.SolidCollision(Main.dust[i].position, 2, 2))
								{
									Main.dust[i].scale = 0f;
								}
								Main.dust[i].rotation += 0.5f;
								Main.dust[i].scale -= 0.01f;
							}
							bool flag = Collision.WetCollision(new Vector2(Main.dust[i].position.X, Main.dust[i].position.Y), 4, 4);
							if (flag)
							{
								Main.dust[i].alpha += 20;
								Main.dust[i].scale -= 0.1f;
							}
							Main.dust[i].alpha += 2;
							Main.dust[i].scale -= 0.005f;
							if (Main.dust[i].alpha > 255)
							{
								Main.dust[i].scale = 0f;
							}
							Dust expr_12C2_cp_0 = Main.dust[i];
							expr_12C2_cp_0.velocity.X = expr_12C2_cp_0.velocity.X * 0.93f;
							if (Main.dust[i].velocity.Y > 4f)
							{
								Main.dust[i].velocity.Y = 4f;
							}
							if (Main.dust[i].noGravity)
							{
								if (Main.dust[i].velocity.X < 0f)
								{
									Main.dust[i].rotation -= 0.2f;
								}
								else
								{
									Main.dust[i].rotation += 0.2f;
								}
								Main.dust[i].scale += 0.03f;
								Dust expr_1380_cp_0 = Main.dust[i];
								expr_1380_cp_0.velocity.X = expr_1380_cp_0.velocity.X * 1.05f;
								Dust expr_139D_cp_0 = Main.dust[i];
								expr_139D_cp_0.velocity.Y = expr_139D_cp_0.velocity.Y + 0.15f;
							}
						}
						if (Main.dust[i].type == 35 && Main.dust[i].noGravity)
						{
							Main.dust[i].scale += 0.03f;
							if (Main.dust[i].scale < 1f)
							{
								Dust expr_1409_cp_0 = Main.dust[i];
								expr_1409_cp_0.velocity.Y = expr_1409_cp_0.velocity.Y + 0.075f;
							}
							Dust expr_1426_cp_0 = Main.dust[i];
							expr_1426_cp_0.velocity.X = expr_1426_cp_0.velocity.X * 1.08f;
							if (Main.dust[i].velocity.X > 0f)
							{
								Main.dust[i].rotation += 0.01f;
							}
							else
							{
								Main.dust[i].rotation -= 0.01f;
							}
							float num15 = Main.dust[i].scale * 0.6f;
							if (num15 > 1f)
							{
								num15 = 1f;
							}
							Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f + 1f), num15, num15 * 0.3f, num15 * 0.1f);
						}
						else
						{
							if (Main.dust[i].type == 67)
							{
								float num16 = Main.dust[i].scale;
								if (num16 > 1f)
								{
									num16 = 1f;
								}
								Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), 0f, num16 * 0.8f, num16);
							}
							else
							{
								if (Main.dust[i].type == 34 || Main.dust[i].type == 35)
								{
									if (!Collision.WetCollision(new Vector2(Main.dust[i].position.X, Main.dust[i].position.Y - 8f), 4, 4))
									{
										Main.dust[i].scale = 0f;
									}
									else
									{
										Main.dust[i].alpha += Main.rand.Next(2);
										if (Main.dust[i].alpha > 255)
										{
											Main.dust[i].scale = 0f;
										}
										Main.dust[i].velocity.Y = -0.5f;
										if (Main.dust[i].type == 34)
										{
											Main.dust[i].scale += 0.005f;
										}
										else
										{
											Main.dust[i].alpha++;
											Main.dust[i].scale -= 0.01f;
											Main.dust[i].velocity.Y = -0.2f;
										}
										Dust expr_16B1_cp_0 = Main.dust[i];
										expr_16B1_cp_0.velocity.X = expr_16B1_cp_0.velocity.X + (float)Main.rand.Next(-10, 10) * 0.002f;
										if ((double)Main.dust[i].velocity.X < -0.25)
										{
											Main.dust[i].velocity.X = -0.25f;
										}
										if ((double)Main.dust[i].velocity.X > 0.25)
										{
											Main.dust[i].velocity.X = 0.25f;
										}
									}
									if (Main.dust[i].type == 35)
									{
										float num17 = Main.dust[i].scale * 0.3f + 0.4f;
										if (num17 > 1f)
										{
											num17 = 1f;
										}
										Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num17, num17 * 0.5f, num17 * 0.3f);
									}
								}
							}
						}
						if (Main.dust[i].type == 68)
						{
							float num18 = Main.dust[i].scale * 0.3f;
							if (num18 > 1f)
							{
								num18 = 1f;
							}
							Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num18 * 0.1f, num18 * 0.2f, num18);
						}
						if (Main.dust[i].type == 70)
						{
							float num19 = Main.dust[i].scale * 0.3f;
							if (num19 > 1f)
							{
								num19 = 1f;
							}
							Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num19 * 0.5f, 0f, num19);
						}
						if (Main.dust[i].type == 41)
						{
							Dust expr_18CB_cp_0 = Main.dust[i];
							expr_18CB_cp_0.velocity.X = expr_18CB_cp_0.velocity.X + (float)Main.rand.Next(-10, 11) * 0.01f;
							Dust expr_18F8_cp_0 = Main.dust[i];
							expr_18F8_cp_0.velocity.Y = expr_18F8_cp_0.velocity.Y + (float)Main.rand.Next(-10, 11) * 0.01f;
							if ((double)Main.dust[i].velocity.X > 0.75)
							{
								Main.dust[i].velocity.X = 0.75f;
							}
							if ((double)Main.dust[i].velocity.X < -0.75)
							{
								Main.dust[i].velocity.X = -0.75f;
							}
							if ((double)Main.dust[i].velocity.Y > 0.75)
							{
								Main.dust[i].velocity.Y = 0.75f;
							}
							if ((double)Main.dust[i].velocity.Y < -0.75)
							{
								Main.dust[i].velocity.Y = -0.75f;
							}
							Main.dust[i].scale += 0.007f;
							float num20 = Main.dust[i].scale * 0.7f;
							if (num20 > 1f)
							{
								num20 = 1f;
							}
							Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num20 * 0.4f, num20 * 0.9f, num20);
						}
						else
						{
							if (Main.dust[i].type == 44)
							{
								Dust expr_1A8C_cp_0 = Main.dust[i];
								expr_1A8C_cp_0.velocity.X = expr_1A8C_cp_0.velocity.X + (float)Main.rand.Next(-10, 11) * 0.003f;
								Dust expr_1AB9_cp_0 = Main.dust[i];
								expr_1AB9_cp_0.velocity.Y = expr_1AB9_cp_0.velocity.Y + (float)Main.rand.Next(-10, 11) * 0.003f;
								if ((double)Main.dust[i].velocity.X > 0.35)
								{
									Main.dust[i].velocity.X = 0.35f;
								}
								if ((double)Main.dust[i].velocity.X < -0.35)
								{
									Main.dust[i].velocity.X = -0.35f;
								}
								if ((double)Main.dust[i].velocity.Y > 0.35)
								{
									Main.dust[i].velocity.Y = 0.35f;
								}
								if ((double)Main.dust[i].velocity.Y < -0.35)
								{
									Main.dust[i].velocity.Y = -0.35f;
								}
								Main.dust[i].scale += 0.0085f;
								float num21 = Main.dust[i].scale * 0.7f;
								if (num21 > 1f)
								{
									num21 = 1f;
								}
								Lighting.addLight((int)(Main.dust[i].position.X / 16f), (int)(Main.dust[i].position.Y / 16f), num21 * 0.7f, num21, num21 * 0.8f);
							}
							else
							{
								Dust expr_1C37_cp_0 = Main.dust[i];
								expr_1C37_cp_0.velocity.X = expr_1C37_cp_0.velocity.X * 0.99f;
							}
						}
						if (Main.dust[i].type != 79)
						{
							Main.dust[i].rotation += Main.dust[i].velocity.X * 0.5f;
						}
						if (Main.dust[i].fadeIn > 0f)
						{
							if (Main.dust[i].type == 46)
							{
								Main.dust[i].scale += 0.1f;
							}
							else
							{
								Main.dust[i].scale += 0.03f;
							}
							if (Main.dust[i].scale > Main.dust[i].fadeIn)
							{
								Main.dust[i].fadeIn = 0f;
							}
						}
						else
						{
							Main.dust[i].scale -= 0.01f;
						}
						if (Main.dust[i].noGravity)
						{
							Dust expr_1D31 = Main.dust[i];
							expr_1D31.velocity *= 0.92f;
							if (Main.dust[i].fadeIn == 0f)
							{
								Main.dust[i].scale -= 0.04f;
							}
						}
						if (Main.dust[i].position.Y > Main.screenPosition.Y + (float)Main.screenHeight)
						{
							Main.dust[i].active = false;
						}
						if ((double)Main.dust[i].scale < 0.1)
						{
							Main.dust[i].active = false;
						}
					}
				}
				else
				{
					Main.dust[i].active = false;
				}
			}
		}
		public Color GetAlpha(Color newColor)
		{
			float num = (float)(255 - this.alpha) / 255f;
			if (this.type == 6 || this.type == 75 || this.type == 20 || this.type == 21)
			{
				return new Color((int)newColor.R, (int)newColor.G, (int)newColor.B, 25);
			}
			if ((this.type == 68 || this.type == 70) && this.noGravity)
			{
				return new Color(255, 255, 255, 0);
			}
			if (this.type == 15 || this.type == 20 || this.type == 21 || this.type == 29 || this.type == 35 || this.type == 41 || this.type == 44 || this.type == 27 || this.type == 45 || this.type == 55 || this.type == 56 || this.type == 57 || this.type == 58 || this.type == 73 || this.type == 74)
			{
				num = (num + 3f) / 4f;
			}
			else
			{
				if (this.type == 43)
				{
					num = (num + 9f) / 10f;
				}
				else
				{
					if (this.type == 66)
					{
						return new Color((int)newColor.R, (int)newColor.G, (int)newColor.B, 0);
					}
					if (this.type == 71)
					{
						return new Color(200, 200, 200, 0);
					}
					if (this.type == 72)
					{
						return new Color(200, 200, 200, 200);
					}
				}
			}
			int r = (int)((float)newColor.R * num);
			int g = (int)((float)newColor.G * num);
			int b = (int)((float)newColor.B * num);
			int num2 = (int)newColor.A - this.alpha;
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num2 > 255)
			{
				num2 = 255;
			}
			return new Color(r, g, b, num2);
		}
		public Color GetColor(Color newColor)
		{
			int num = (int)(this.color.R - (255 - newColor.R));
			int num2 = (int)(this.color.G - (255 - newColor.G));
			int num3 = (int)(this.color.B - (255 - newColor.B));
			int num4 = (int)(this.color.A - (255 - newColor.A));
			if (num < 0)
			{
				num = 0;
			}
			if (num > 255)
			{
				num = 255;
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num2 > 255)
			{
				num2 = 255;
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num3 > 255)
			{
				num3 = 255;
			}
			if (num4 < 0)
			{
				num4 = 0;
			}
			if (num4 > 255)
			{
				num4 = 255;
			}
			return new Color(num, num2, num3, num4);
		}
	}
}
