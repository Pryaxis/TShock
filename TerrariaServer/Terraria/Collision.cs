
using System;
namespace Terraria
{
	public class Collision
	{
		public static bool up;
		public static bool down;
		public static bool CanHit(Vector2 Position1, int Width1, int Height1, Vector2 Position2, int Width2, int Height2)
		{
			int num = (int)((Position1.X + (float)(Width1 / 2)) / 16f);
			int num2 = (int)((Position1.Y + (float)(Height1 / 2)) / 16f);
			int num3 = (int)((Position2.X + (float)(Width2 / 2)) / 16f);
			int num4 = (int)((Position2.Y + (float)(Height2 / 2)) / 16f);
			bool result;
			try
			{
				while (true)
				{
					int num5 = Math.Abs(num - num3);
					int num6 = Math.Abs(num2 - num4);
					if (num == num3 && num2 == num4)
					{
						break;
					}
					if (num5 > num6)
					{
						if (num < num3)
						{
							num++;
						}
						else
						{
							num--;
						}
						if (Main.tile[num, num2 - 1].active && Main.tileSolid[(int)Main.tile[num, num2 - 1].type] && !Main.tileSolidTop[(int)Main.tile[num, num2 - 1].type] && Main.tile[num, num2 + 1].active && Main.tileSolid[(int)Main.tile[num, num2 + 1].type] && !Main.tileSolidTop[(int)Main.tile[num, num2 + 1].type])
						{
							goto Block_13;
						}
					}
					else
					{
						if (num2 < num4)
						{
							num2++;
						}
						else
						{
							num2--;
						}
						if (Main.tile[num - 1, num2].active && Main.tileSolid[(int)Main.tile[num - 1, num2].type] && !Main.tileSolidTop[(int)Main.tile[num - 1, num2].type] && Main.tile[num + 1, num2].active && Main.tileSolid[(int)Main.tile[num + 1, num2].type] && !Main.tileSolidTop[(int)Main.tile[num + 1, num2].type])
						{
							goto Block_22;
						}
					}
					if (Main.tile[num, num2].active && Main.tileSolid[(int)Main.tile[num, num2].type] && !Main.tileSolidTop[(int)Main.tile[num, num2].type])
					{
						goto Block_26;
					}
				}
				result = true;
				return result;
				Block_13:
				result = false;
				return result;
				Block_22:
				result = false;
				return result;
				Block_26:
				result = false;
			}
			catch
			{
				result = false;
			}
			return result;
		}
		public static bool EmptyTile(int i, int j, bool ignoreTiles = false)
		{
			Rectangle rectangle = new Rectangle(i * 16, j * 16, 16, 16);
			if (Main.tile[i, j].active && !ignoreTiles)
			{
				return false;
			}
			for (int k = 0; k < 255; k++)
			{
				if (Main.player[k].active && rectangle.Intersects(new Rectangle((int)Main.player[k].position.X, (int)Main.player[k].position.Y, Main.player[k].width, Main.player[k].height)))
				{
					return false;
				}
			}
			for (int l = 0; l < 200; l++)
			{
				if (Main.npc[l].active && rectangle.Intersects(new Rectangle((int)Main.npc[l].position.X, (int)Main.npc[l].position.Y, Main.npc[l].width, Main.npc[l].height)))
				{
					return false;
				}
			}
			return true;
		}
		public static bool DrownCollision(Vector2 Position, int Width, int Height, float gravDir = -1f)
		{
			Vector2 vector = new Vector2(Position.X + (float)(Width / 2), Position.Y + (float)(Height / 2));
			int num = 10;
			int num2 = 12;
			if (num > Width)
			{
				num = Width;
			}
			if (num2 > Height)
			{
				num2 = Height;
			}
			vector = new Vector2(vector.X - (float)(num / 2), Position.Y + -2f);
			if (gravDir == -1f)
			{
				vector.Y += (float)(Height / 2 - 6);
			}
			int num3 = (int)(Position.X / 16f) - 1;
			int num4 = (int)((Position.X + (float)Width) / 16f) + 2;
			int num5 = (int)(Position.Y / 16f) - 1;
			int num6 = (int)((Position.Y + (float)Height) / 16f) + 2;
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
			for (int i = num3; i < num4; i++)
			{
				for (int j = num5; j < num6; j++)
				{
					if (Main.tile[i, j] != null && Main.tile[i, j].liquid > 0)
					{
						Vector2 vector2;
						vector2.X = (float)(i * 16);
						vector2.Y = (float)(j * 16);
						int num7 = 16;
						float num8 = (float)(256 - (int)Main.tile[i, j].liquid);
						num8 /= 32f;
						vector2.Y += num8 * 2f;
						num7 -= (int)(num8 * 2f);
						if (vector.X + (float)num > vector2.X && vector.X < vector2.X + 16f && vector.Y + (float)num2 > vector2.Y && vector.Y < vector2.Y + (float)num7)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		public static bool WetCollision(Vector2 Position, int Width, int Height)
		{
			Vector2 vector = new Vector2(Position.X + (float)(Width / 2), Position.Y + (float)(Height / 2));
			int num = 10;
			int num2 = Height / 2;
			if (num > Width)
			{
				num = Width;
			}
			if (num2 > Height)
			{
				num2 = Height;
			}
			vector = new Vector2(vector.X - (float)(num / 2), vector.Y - (float)(num2 / 2));
			int num3 = (int)(Position.X / 16f) - 1;
			int num4 = (int)((Position.X + (float)Width) / 16f) + 2;
			int num5 = (int)(Position.Y / 16f) - 1;
			int num6 = (int)((Position.Y + (float)Height) / 16f) + 2;
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
			for (int i = num3; i < num4; i++)
			{
				for (int j = num5; j < num6; j++)
				{
					if (Main.tile[i, j] != null && Main.tile[i, j].liquid > 0)
					{
						Vector2 vector2;
						vector2.X = (float)(i * 16);
						vector2.Y = (float)(j * 16);
						int num7 = 16;
						float num8 = (float)(256 - (int)Main.tile[i, j].liquid);
						num8 /= 32f;
						vector2.Y += num8 * 2f;
						num7 -= (int)(num8 * 2f);
						if (vector.X + (float)num > vector2.X && vector.X < vector2.X + 16f && vector.Y + (float)num2 > vector2.Y && vector.Y < vector2.Y + (float)num7)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		public static bool LavaCollision(Vector2 Position, int Width, int Height)
		{
			int num = (int)(Position.X / 16f) - 1;
			int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int num3 = (int)(Position.Y / 16f) - 1;
			int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
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
			for (int i = num; i < num2; i++)
			{
				for (int j = num3; j < num4; j++)
				{
					if (Main.tile[i, j] != null && Main.tile[i, j].liquid > 0 && Main.tile[i, j].lava)
					{
						Vector2 vector;
						vector.X = (float)(i * 16);
						vector.Y = (float)(j * 16);
						int num5 = 16;
						float num6 = (float)(256 - (int)Main.tile[i, j].liquid);
						num6 /= 32f;
						vector.Y += num6 * 2f;
						num5 -= (int)(num6 * 2f);
						if (Position.X + (float)Width > vector.X && Position.X < vector.X + 16f && Position.Y + (float)Height > vector.Y && Position.Y < vector.Y + (float)num5)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		public static Vector2 TileCollision(Vector2 Position, Vector2 Velocity, int Width, int Height, bool fallThrough = false, bool fall2 = false)
		{
			Collision.up = false;
			Collision.down = false;
			Vector2 result = Velocity;
			Vector2 vector = Velocity;
			Vector2 vector2 = Position + Velocity;
			Vector2 vector3 = Position;
			int num = (int)(Position.X / 16f) - 1;
			int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int num3 = (int)(Position.Y / 16f) - 1;
			int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			int num5 = -1;
			int num6 = -1;
			int num7 = -1;
			int num8 = -1;
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
			for (int i = num; i < num2; i++)
			{
				for (int j = num3; j < num4; j++)
				{
					if (Main.tile[i, j] != null && Main.tile[i, j].active && (Main.tileSolid[(int)Main.tile[i, j].type] || (Main.tileSolidTop[(int)Main.tile[i, j].type] && Main.tile[i, j].frameY == 0)))
					{
						Vector2 vector4;
						vector4.X = (float)(i * 16);
						vector4.Y = (float)(j * 16);
						if (vector2.X + (float)Width > vector4.X && vector2.X < vector4.X + 16f && vector2.Y + (float)Height > vector4.Y && vector2.Y < vector4.Y + 16f)
						{
							if (vector3.Y + (float)Height <= vector4.Y)
							{
								Collision.down = true;
								if (!Main.tileSolidTop[(int)Main.tile[i, j].type] || !fallThrough || (Velocity.Y > 1f && !fall2))
								{
									num7 = i;
									num8 = j;
									if (num7 != num5)
									{
										result.Y = vector4.Y - (vector3.Y + (float)Height);
									}
								}
							}
							else
							{
								if (vector3.X + (float)Width <= vector4.X && !Main.tileSolidTop[(int)Main.tile[i, j].type])
								{
									num5 = i;
									num6 = j;
									if (num6 != num8)
									{
										result.X = vector4.X - (vector3.X + (float)Width);
									}
									if (num7 == num5)
									{
										result.Y = vector.Y;
									}
								}
								else
								{
									if (vector3.X >= vector4.X + 16f && !Main.tileSolidTop[(int)Main.tile[i, j].type])
									{
										num5 = i;
										num6 = j;
										if (num6 != num8)
										{
											result.X = vector4.X + 16f - vector3.X;
										}
										if (num7 == num5)
										{
											result.Y = vector.Y;
										}
									}
									else
									{
										if (vector3.Y >= vector4.Y + 16f && !Main.tileSolidTop[(int)Main.tile[i, j].type])
										{
											Collision.up = true;
											num7 = i;
											num8 = j;
											result.Y = vector4.Y + 16f - vector3.Y;
											if (num8 == num6)
											{
												result.X = vector.X;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
		public static bool SolidCollision(Vector2 Position, int Width, int Height)
		{
			int num = (int)(Position.X / 16f) - 1;
			int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int num3 = (int)(Position.Y / 16f) - 1;
			int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
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
			for (int i = num; i < num2; i++)
			{
				for (int j = num3; j < num4; j++)
				{
					if (Main.tile[i, j] != null && Main.tile[i, j].active && Main.tileSolid[(int)Main.tile[i, j].type] && !Main.tileSolidTop[(int)Main.tile[i, j].type])
					{
						Vector2 vector;
						vector.X = (float)(i * 16);
						vector.Y = (float)(j * 16);
						if (Position.X + (float)Width > vector.X && Position.X < vector.X + 16f && Position.Y + (float)Height > vector.Y && Position.Y < vector.Y + 16f)
						{
							return true;
						}
					}
				}
			}
			return false;
		}
		public static Vector2 WaterCollision(Vector2 Position, Vector2 Velocity, int Width, int Height, bool fallThrough = false, bool fall2 = false)
		{
			Vector2 result = Velocity;
			Vector2 vector = Position + Velocity;
			Vector2 vector2 = Position;
			int num = (int)(Position.X / 16f) - 1;
			int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int num3 = (int)(Position.Y / 16f) - 1;
			int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
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
			for (int i = num; i < num2; i++)
			{
				for (int j = num3; j < num4; j++)
				{
					if (Main.tile[i, j] != null && Main.tile[i, j].liquid > 0)
					{
						int num5 = (int)Math.Round((double)((float)Main.tile[i, j].liquid / 32f)) * 2;
						Vector2 vector3;
						vector3.X = (float)(i * 16);
						vector3.Y = (float)(j * 16 + 16 - num5);
						if (vector.X + (float)Width > vector3.X && vector.X < vector3.X + 16f && vector.Y + (float)Height > vector3.Y && vector.Y < vector3.Y + (float)num5 && vector2.Y + (float)Height <= vector3.Y && !fallThrough)
						{
							result.Y = vector3.Y - (vector2.Y + (float)Height);
						}
					}
				}
			}
			return result;
		}
		public static Vector2 AnyCollision(Vector2 Position, Vector2 Velocity, int Width, int Height)
		{
			Vector2 result = Velocity;
			Vector2 vector = Velocity;
			Vector2 vector2 = Position + Velocity;
			Vector2 vector3 = Position;
			int num = (int)(Position.X / 16f) - 1;
			int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int num3 = (int)(Position.Y / 16f) - 1;
			int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			int num5 = -1;
			int num6 = -1;
			int num7 = -1;
			int num8 = -1;
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
			for (int i = num; i < num2; i++)
			{
				for (int j = num3; j < num4; j++)
				{
					if (Main.tile[i, j] != null && Main.tile[i, j].active)
					{
						Vector2 vector4;
						vector4.X = (float)(i * 16);
						vector4.Y = (float)(j * 16);
						if (vector2.X + (float)Width > vector4.X && vector2.X < vector4.X + 16f && vector2.Y + (float)Height > vector4.Y && vector2.Y < vector4.Y + 16f)
						{
							if (vector3.Y + (float)Height <= vector4.Y)
							{
								num7 = i;
								num8 = j;
								if (num7 != num5)
								{
									result.Y = vector4.Y - (vector3.Y + (float)Height);
								}
							}
							else
							{
								if (vector3.X + (float)Width <= vector4.X && !Main.tileSolidTop[(int)Main.tile[i, j].type])
								{
									num5 = i;
									num6 = j;
									if (num6 != num8)
									{
										result.X = vector4.X - (vector3.X + (float)Width);
									}
									if (num7 == num5)
									{
										result.Y = vector.Y;
									}
								}
								else
								{
									if (vector3.X >= vector4.X + 16f && !Main.tileSolidTop[(int)Main.tile[i, j].type])
									{
										num5 = i;
										num6 = j;
										if (num6 != num8)
										{
											result.X = vector4.X + 16f - vector3.X;
										}
										if (num7 == num5)
										{
											result.Y = vector.Y;
										}
									}
									else
									{
										if (vector3.Y >= vector4.Y + 16f && !Main.tileSolidTop[(int)Main.tile[i, j].type])
										{
											num7 = i;
											num8 = j;
											result.Y = vector4.Y + 16f - vector3.Y + 0.01f;
											if (num8 == num6)
											{
												result.X = vector.X + 0.01f;
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
		public static void HitTiles(Vector2 Position, Vector2 Velocity, int Width, int Height)
		{
			Vector2 vector = Position + Velocity;
			int num = (int)(Position.X / 16f) - 1;
			int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int num3 = (int)(Position.Y / 16f) - 1;
			int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
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
			for (int i = num; i < num2; i++)
			{
				for (int j = num3; j < num4; j++)
				{
					if (Main.tile[i, j] != null && Main.tile[i, j].active && (Main.tileSolid[(int)Main.tile[i, j].type] || (Main.tileSolidTop[(int)Main.tile[i, j].type] && Main.tile[i, j].frameY == 0)))
					{
						Vector2 vector2;
						vector2.X = (float)(i * 16);
						vector2.Y = (float)(j * 16);
						if (vector.X + (float)Width >= vector2.X && vector.X <= vector2.X + 16f && vector.Y + (float)Height >= vector2.Y && vector.Y <= vector2.Y + 16f)
						{
							WorldGen.KillTile(i, j, true, true, false);
						}
					}
				}
			}
		}
		public static Vector2 HurtTiles(Vector2 Position, Vector2 Velocity, int Width, int Height, bool fireImmune = false)
		{
			Vector2 vector = Position;
			int num = (int)(Position.X / 16f) - 1;
			int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int num3 = (int)(Position.Y / 16f) - 1;
			int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
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
			for (int i = num; i < num2; i++)
			{
				for (int j = num3; j < num4; j++)
				{
					if (Main.tile[i, j] != null && Main.tile[i, j].active && (Main.tile[i, j].type == 32 || Main.tile[i, j].type == 37 || Main.tile[i, j].type == 48 || Main.tile[i, j].type == 53 || Main.tile[i, j].type == 57 || Main.tile[i, j].type == 58 || Main.tile[i, j].type == 69 || Main.tile[i, j].type == 76 || Main.tile[i, j].type == 112 || Main.tile[i, j].type == 116 || Main.tile[i, j].type == 123))
					{
						Vector2 vector2;
						vector2.X = (float)(i * 16);
						vector2.Y = (float)(j * 16);
						int num5 = 0;
						int type = (int)Main.tile[i, j].type;
						if (type == 32 || type == 69 || type == 80)
						{
							if (vector.X + (float)Width > vector2.X && vector.X < vector2.X + 16f && vector.Y + (float)Height > vector2.Y && (double)vector.Y < (double)vector2.Y + 16.01)
							{
								int num6 = 1;
								if (vector.X + (float)(Width / 2) < vector2.X + 8f)
								{
									num6 = -1;
								}
								num5 = 10;
								if (type == 69)
								{
									num5 = 17;
								}
								else
								{
									if (type == 80)
									{
										num5 = 6;
									}
								}
								if (type == 32 || type == 69)
								{
									WorldGen.KillTile(i, j, false, false, false);
									if (Main.netMode == 1 && !Main.tile[i, j].active && Main.netMode == 1)
									{
										NetMessage.SendData(17, -1, -1, "", 4, (float)i, (float)j, 0f, 0);
									}
								}
								return new Vector2((float)num6, (float)num5);
							}
						}
						else
						{
							if (type == 53 || type == 112 || type == 116 || type == 123)
							{
								if (vector.X + (float)Width - 2f >= vector2.X && vector.X + 2f <= vector2.X + 16f && vector.Y + (float)Height - 2f >= vector2.Y && vector.Y + 2f <= vector2.Y + 16f)
								{
									int num7 = 1;
									if (vector.X + (float)(Width / 2) < vector2.X + 8f)
									{
										num7 = -1;
									}
									num5 = 20;
									return new Vector2((float)num7, (float)num5);
								}
							}
							else
							{
								if (vector.X + (float)Width >= vector2.X && vector.X <= vector2.X + 16f && vector.Y + (float)Height >= vector2.Y && (double)vector.Y <= (double)vector2.Y + 16.01)
								{
									int num8 = 1;
									if (vector.X + (float)(Width / 2) < vector2.X + 8f)
									{
										num8 = -1;
									}
									if (!fireImmune && (type == 37 || type == 58 || type == 76))
									{
										num5 = 20;
									}
									if (type == 48)
									{
										num5 = 40;
									}
									return new Vector2((float)num8, (float)num5);
								}
							}
						}
					}
				}
			}
			return default(Vector2);
		}
		public static bool SwitchTiles(Vector2 Position, int Width, int Height, Vector2 oldPosition)
		{
			int num = (int)(Position.X / 16f) - 1;
			int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int num3 = (int)(Position.Y / 16f) - 1;
			int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
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
			for (int i = num; i < num2; i++)
			{
				for (int j = num3; j < num4; j++)
				{
					if (Main.tile[i, j] != null && Main.tile[i, j].active && Main.tile[i, j].type == 135)
					{
						Vector2 vector;
						vector.X = (float)(i * 16);
						vector.Y = (float)(j * 16 + 12);
						if (Position.X + (float)Width > vector.X && Position.X < vector.X + 16f && Position.Y + (float)Height > vector.Y && (double)Position.Y < (double)vector.Y + 4.01 && (oldPosition.X + (float)Width <= vector.X || oldPosition.X >= vector.X + 16f || oldPosition.Y + (float)Height <= vector.Y || (double)oldPosition.Y >= (double)vector.Y + 16.01))
						{
							WorldGen.hitSwitch(i, j);
							NetMessage.SendData(59, -1, -1, "", i, (float)j, 0f, 0f, 0);
							return true;
						}
					}
				}
			}
			return false;
		}
		public static Vector2 StickyTiles(Vector2 Position, Vector2 Velocity, int Width, int Height)
		{
			Vector2 vector = Position;
			int num = (int)(Position.X / 16f) - 1;
			int num2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int num3 = (int)(Position.Y / 16f) - 1;
			int num4 = (int)((Position.Y + (float)Height) / 16f) + 2;
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
			for (int i = num; i < num2; i++)
			{
				for (int j = num3; j < num4; j++)
				{
					if (Main.tile[i, j] != null && Main.tile[i, j].active && Main.tile[i, j].type == 51)
					{
						Vector2 vector2;
						vector2.X = (float)(i * 16);
						vector2.Y = (float)(j * 16);
						if (vector.X + (float)Width > vector2.X && vector.X < vector2.X + 16f && vector.Y + (float)Height > vector2.Y && (double)vector.Y < (double)vector2.Y + 16.01)
						{
							if ((double)(Math.Abs(Velocity.X) + Math.Abs(Velocity.Y)) > 0.7 && Main.rand.Next(30) == 0)
							{
								Dust.NewDust(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16, 30, 0f, 0f, 0, default(Color), 1f);
							}
							return new Vector2((float)i, (float)j);
						}
					}
				}
			}
			return new Vector2(-1f, -1f);
		}
		public static bool SolidTiles(int startX, int endX, int startY, int endY)
		{
			if (startX < 0)
			{
				return true;
			}
			if (endX >= Main.maxTilesX)
			{
				return true;
			}
			if (startY < 0)
			{
				return true;
			}
			if (endY >= Main.maxTilesY)
			{
				return true;
			}
			for (int i = startX; i < endX + 1; i++)
			{
				for (int j = startY; j < endY + 1; j++)
				{
					if (Main.tile[i, j] == null)
					{
						return false;
					}
					if (Main.tile[i, j].active && Main.tileSolid[(int)Main.tile[i, j].type] && !Main.tileSolidTop[(int)Main.tile[i, j].type])
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
