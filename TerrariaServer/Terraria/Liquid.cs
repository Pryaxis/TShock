using System;
namespace Terraria
{
	public class Liquid
	{
		public static int skipCount = 0;
		public static int stuckCount = 0;
		public static int stuckAmount = 0;
		public static int cycles = 10;
		public static int resLiquid = 5000;
		public static int maxLiquid = 5000;
		public static int numLiquid;
		public static bool stuck = false;
		public static bool quickFall = false;
		public static bool quickSettle = false;
		private static int wetCounter;
		public static int panicCounter = 0;
		public static bool panicMode = false;
		public static int panicY = 0;
		public int x;
		public int y;
		public int kill;
		public int delay;
		public static double QuickWater(int verbose = 0, int minY = -1, int maxY = -1)
		{
			int num = 0;
			if (minY == -1)
			{
				minY = 3;
			}
			if (maxY == -1)
			{
				maxY = Main.maxTilesY - 3;
			}
			for (int i = maxY; i >= minY; i--)
			{
				if (verbose > 0)
				{
					float num2 = (float)(maxY - i) / (float)(maxY - minY + 1);
					num2 /= (float)verbose;
					Main.statusText = "Settling liquids: " + (int)(num2 * 100f + 1f) + "%";
				}
				else
				{
					if (verbose < 0)
					{
						float num3 = (float)(maxY - i) / (float)(maxY - minY + 1);
						num3 /= (float)(-(float)verbose);
						Main.statusText = "Creating underworld: " + (int)(num3 * 100f + 1f) + "%";
					}
				}
				for (int j = 0; j < 2; j++)
				{
					int num4 = 2;
					int num5 = Main.maxTilesX - 2;
					int num6 = 1;
					if (j == 1)
					{
						num4 = Main.maxTilesX - 2;
						num5 = 2;
						num6 = -1;
					}
					for (int num7 = num4; num7 != num5; num7 += num6)
					{
						if (Main.tile[num7, i].liquid > 0)
						{
							int num8 = -num6;
							bool flag = false;
							int num9 = num7;
							int num10 = i;
							bool flag2 = Main.tile[num7, i].lava;
							byte b = Main.tile[num7, i].liquid;
							Main.tile[num7, i].liquid = 0;
							bool flag3 = true;
							int num11 = 0;
							while (flag3 && num9 > 3 && num9 < Main.maxTilesX - 3 && num10 < Main.maxTilesY - 3)
							{
								flag3 = false;
								while (Main.tile[num9, num10 + 1].liquid == 0 && num10 < Main.maxTilesY - 5 && (!Main.tile[num9, num10 + 1].active || !Main.tileSolid[(int)Main.tile[num9, num10 + 1].type] || Main.tileSolidTop[(int)Main.tile[num9, num10 + 1].type]))
								{
									flag = true;
									num8 = num6;
									num11 = 0;
									flag3 = true;
									num10++;
									if (num10 > WorldGen.waterLine)
									{
										flag2 = true;
									}
								}
								if (Main.tile[num9, num10 + 1].liquid > 0 && Main.tile[num9, num10 + 1].liquid < 255 && Main.tile[num9, num10 + 1].lava == flag2)
								{
									int num12 = (int)(255 - Main.tile[num9, num10 + 1].liquid);
									if (num12 > (int)b)
									{
										num12 = (int)b;
									}
                                    Main.tile[num9, num10 + 1].liquid += (byte)num12;
									b -= (byte)num12;
									if (b <= 0)
									{
										num++;
										break;
									}
								}
								if (num11 == 0)
								{
									if (Main.tile[num9 + num8, num10].liquid == 0 && (!Main.tile[num9 + num8, num10].active || !Main.tileSolid[(int)Main.tile[num9 + num8, num10].type] || Main.tileSolidTop[(int)Main.tile[num9 + num8, num10].type]))
									{
										num11 = num8;
									}
									else
									{
										if (Main.tile[num9 - num8, num10].liquid == 0 && (!Main.tile[num9 - num8, num10].active || !Main.tileSolid[(int)Main.tile[num9 - num8, num10].type] || Main.tileSolidTop[(int)Main.tile[num9 - num8, num10].type]))
										{
											num11 = -num8;
										}
									}
								}
								if (num11 != 0 && Main.tile[num9 + num11, num10].liquid == 0 && (!Main.tile[num9 + num11, num10].active || !Main.tileSolid[(int)Main.tile[num9 + num11, num10].type] || Main.tileSolidTop[(int)Main.tile[num9 + num11, num10].type]))
								{
									flag3 = true;
									num9 += num11;
								}
								if (flag && !flag3)
								{
									flag = false;
									flag3 = true;
									num8 = -num6;
									num11 = 0;
								}
							}
							if (num7 != num9 && i != num10)
							{
								num++;
							}
							Main.tile[num9, num10].liquid = b;
							Main.tile[num9, num10].lava = flag2;
							if (Main.tile[num9 - 1, num10].liquid > 0 && Main.tile[num9 - 1, num10].lava != flag2)
							{
								if (flag2)
								{
									Liquid.LavaCheck(num9, num10);
								}
								else
								{
									Liquid.LavaCheck(num9 - 1, num10);
								}
							}
							else
							{
								if (Main.tile[num9 + 1, num10].liquid > 0 && Main.tile[num9 + 1, num10].lava != flag2)
								{
									if (flag2)
									{
										Liquid.LavaCheck(num9, num10);
									}
									else
									{
										Liquid.LavaCheck(num9 + 1, num10);
									}
								}
								else
								{
									if (Main.tile[num9, num10 - 1].liquid > 0 && Main.tile[num9, num10 - 1].lava != flag2)
									{
										if (flag2)
										{
											Liquid.LavaCheck(num9, num10);
										}
										else
										{
											Liquid.LavaCheck(num9, num10 - 1);
										}
									}
									else
									{
										if (Main.tile[num9, num10 + 1].liquid > 0 && Main.tile[num9, num10 + 1].lava != flag2)
										{
											if (flag2)
											{
												Liquid.LavaCheck(num9, num10);
											}
											else
											{
												Liquid.LavaCheck(num9, num10 + 1);
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return (double)num;
		}
		public void Update()
		{
			if (Main.tile[this.x, this.y].active && Main.tileSolid[(int)Main.tile[this.x, this.y].type] && !Main.tileSolidTop[(int)Main.tile[this.x, this.y].type])
			{
				byte arg_81_0 = Main.tile[this.x, this.y].type;
				this.kill = 9;
				return;
			}
			byte liquid = Main.tile[this.x, this.y].liquid;
			float num = 0f;
			if (this.y > Main.maxTilesY - 200 && !Main.tile[this.x, this.y].lava && Main.tile[this.x, this.y].liquid > 0)
			{
				byte b = 2;
				if (Main.tile[this.x, this.y].liquid < b)
				{
					b = Main.tile[this.x, this.y].liquid;
				}
                Main.tile[this.x, this.y].liquid -= b;
			}
			if (Main.tile[this.x, this.y].liquid == 0)
			{
				this.kill = 9;
				return;
			}
			if (Main.tile[this.x, this.y].lava)
			{
				Liquid.LavaCheck(this.x, this.y);
				if (!Liquid.quickFall)
				{
					if (this.delay < 5)
					{
						this.delay++;
						return;
					}
					this.delay = 0;
				}
			}
			else
			{
				if (Main.tile[this.x - 1, this.y].lava)
				{
					Liquid.AddWater(this.x - 1, this.y);
				}
				if (Main.tile[this.x + 1, this.y].lava)
				{
					Liquid.AddWater(this.x + 1, this.y);
				}
				if (Main.tile[this.x, this.y - 1].lava)
				{
					Liquid.AddWater(this.x, this.y - 1);
				}
				if (Main.tile[this.x, this.y + 1].lava)
				{
					Liquid.AddWater(this.x, this.y + 1);
				}
			}
			if ((!Main.tile[this.x, this.y + 1].active || !Main.tileSolid[(int)Main.tile[this.x, this.y + 1].type] || Main.tileSolidTop[(int)Main.tile[this.x, this.y + 1].type]) && (Main.tile[this.x, this.y + 1].liquid <= 0 || Main.tile[this.x, this.y + 1].lava == Main.tile[this.x, this.y].lava) && Main.tile[this.x, this.y + 1].liquid < 255)
			{
				num = (float)(255 - Main.tile[this.x, this.y + 1].liquid);
				if (num > (float)Main.tile[this.x, this.y].liquid)
				{
					num = (float)Main.tile[this.x, this.y].liquid;
				}
                Main.tile[this.x, this.y].liquid -= (byte)num;
                Main.tile[this.x, this.y + 1].liquid += (byte)num;
				Main.tile[this.x, this.y + 1].lava = Main.tile[this.x, this.y].lava;
				Liquid.AddWater(this.x, this.y + 1);
				Main.tile[this.x, this.y + 1].skipLiquid = true;
				Main.tile[this.x, this.y].skipLiquid = true;
				if (Main.tile[this.x, this.y].liquid > 250)
				{
					Main.tile[this.x, this.y].liquid = 255;
				}
				else
				{
					Liquid.AddWater(this.x - 1, this.y);
					Liquid.AddWater(this.x + 1, this.y);
				}
			}
			if (Main.tile[this.x, this.y].liquid > 0)
			{
				bool flag = true;
				bool flag2 = true;
				bool flag3 = true;
				bool flag4 = true;
				if (Main.tile[this.x - 1, this.y].active && Main.tileSolid[(int)Main.tile[this.x - 1, this.y].type] && !Main.tileSolidTop[(int)Main.tile[this.x - 1, this.y].type])
				{
					flag = false;
				}
				else
				{
					if (Main.tile[this.x - 1, this.y].liquid > 0 && Main.tile[this.x - 1, this.y].lava != Main.tile[this.x, this.y].lava)
					{
						flag = false;
					}
					else
					{
						if (Main.tile[this.x - 2, this.y].active && Main.tileSolid[(int)Main.tile[this.x - 2, this.y].type] && !Main.tileSolidTop[(int)Main.tile[this.x - 2, this.y].type])
						{
							flag3 = false;
						}
						else
						{
							if (Main.tile[this.x - 2, this.y].liquid == 0)
							{
								flag3 = false;
							}
							else
							{
								if (Main.tile[this.x - 2, this.y].liquid > 0 && Main.tile[this.x - 2, this.y].lava != Main.tile[this.x, this.y].lava)
								{
									flag3 = false;
								}
							}
						}
					}
				}
				if (Main.tile[this.x + 1, this.y].active && Main.tileSolid[(int)Main.tile[this.x + 1, this.y].type] && !Main.tileSolidTop[(int)Main.tile[this.x + 1, this.y].type])
				{
					flag2 = false;
				}
				else
				{
					if (Main.tile[this.x + 1, this.y].liquid > 0 && Main.tile[this.x + 1, this.y].lava != Main.tile[this.x, this.y].lava)
					{
						flag2 = false;
					}
					else
					{
						if (Main.tile[this.x + 2, this.y].active && Main.tileSolid[(int)Main.tile[this.x + 2, this.y].type] && !Main.tileSolidTop[(int)Main.tile[this.x + 2, this.y].type])
						{
							flag4 = false;
						}
						else
						{
							if (Main.tile[this.x + 2, this.y].liquid == 0)
							{
								flag4 = false;
							}
							else
							{
								if (Main.tile[this.x + 2, this.y].liquid > 0 && Main.tile[this.x + 2, this.y].lava != Main.tile[this.x, this.y].lava)
								{
									flag4 = false;
								}
							}
						}
					}
				}
				int num2 = 0;
				if (Main.tile[this.x, this.y].liquid < 3)
				{
					num2 = -1;
				}
				if (flag && flag2)
				{
					if (flag3 && flag4)
					{
						bool flag5 = true;
						bool flag6 = true;
						if (Main.tile[this.x - 3, this.y].active && Main.tileSolid[(int)Main.tile[this.x - 3, this.y].type] && !Main.tileSolidTop[(int)Main.tile[this.x - 3, this.y].type])
						{
							flag5 = false;
						}
						else
						{
							if (Main.tile[this.x - 3, this.y].liquid == 0)
							{
								flag5 = false;
							}
							else
							{
								if (Main.tile[this.x - 3, this.y].lava != Main.tile[this.x, this.y].lava)
								{
									flag5 = false;
								}
							}
						}
						if (Main.tile[this.x + 3, this.y].active && Main.tileSolid[(int)Main.tile[this.x + 3, this.y].type] && !Main.tileSolidTop[(int)Main.tile[this.x + 3, this.y].type])
						{
							flag6 = false;
						}
						else
						{
							if (Main.tile[this.x + 3, this.y].liquid == 0)
							{
								flag6 = false;
							}
							else
							{
								if (Main.tile[this.x + 3, this.y].lava != Main.tile[this.x, this.y].lava)
								{
									flag6 = false;
								}
							}
						}
						if (flag5 && flag6)
						{
							num = (float)((int)(Main.tile[this.x - 1, this.y].liquid + Main.tile[this.x + 1, this.y].liquid + Main.tile[this.x - 2, this.y].liquid + Main.tile[this.x + 2, this.y].liquid + Main.tile[this.x - 3, this.y].liquid + Main.tile[this.x + 3, this.y].liquid + Main.tile[this.x, this.y].liquid) + num2);
							num = (float)Math.Round((double)(num / 7f));
							int num3 = 0;
							Main.tile[this.x - 1, this.y].lava = Main.tile[this.x, this.y].lava;
							if (Main.tile[this.x - 1, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x - 1, this.y);
								Main.tile[this.x - 1, this.y].liquid = (byte)num;
							}
							else
							{
								num3++;
							}
							Main.tile[this.x + 1, this.y].lava = Main.tile[this.x, this.y].lava;
							if (Main.tile[this.x + 1, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x + 1, this.y);
								Main.tile[this.x + 1, this.y].liquid = (byte)num;
							}
							else
							{
								num3++;
							}
							Main.tile[this.x - 2, this.y].lava = Main.tile[this.x, this.y].lava;
							if (Main.tile[this.x - 2, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x - 2, this.y);
								Main.tile[this.x - 2, this.y].liquid = (byte)num;
							}
							else
							{
								num3++;
							}
							Main.tile[this.x + 2, this.y].lava = Main.tile[this.x, this.y].lava;
							if (Main.tile[this.x + 2, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x + 2, this.y);
								Main.tile[this.x + 2, this.y].liquid = (byte)num;
							}
							else
							{
								num3++;
							}
							Main.tile[this.x - 3, this.y].lava = Main.tile[this.x, this.y].lava;
							if (Main.tile[this.x - 3, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x - 3, this.y);
								Main.tile[this.x - 3, this.y].liquid = (byte)num;
							}
							else
							{
								num3++;
							}
							Main.tile[this.x + 3, this.y].lava = Main.tile[this.x, this.y].lava;
							if (Main.tile[this.x + 3, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x + 3, this.y);
								Main.tile[this.x + 3, this.y].liquid = (byte)num;
							}
							else
							{
								num3++;
							}
							if (Main.tile[this.x - 1, this.y].liquid != (byte)num || Main.tile[this.x, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x - 1, this.y);
							}
							if (Main.tile[this.x + 1, this.y].liquid != (byte)num || Main.tile[this.x, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x + 1, this.y);
							}
							if (Main.tile[this.x - 2, this.y].liquid != (byte)num || Main.tile[this.x, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x - 2, this.y);
							}
							if (Main.tile[this.x + 2, this.y].liquid != (byte)num || Main.tile[this.x, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x + 2, this.y);
							}
							if (Main.tile[this.x - 3, this.y].liquid != (byte)num || Main.tile[this.x, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x - 3, this.y);
							}
							if (Main.tile[this.x + 3, this.y].liquid != (byte)num || Main.tile[this.x, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x + 3, this.y);
							}
							if (num3 != 6 || Main.tile[this.x, this.y - 1].liquid <= 0)
							{
								Main.tile[this.x, this.y].liquid = (byte)num;
							}
						}
						else
						{
							int num4 = 0;
							num = (float)((int)(Main.tile[this.x - 1, this.y].liquid + Main.tile[this.x + 1, this.y].liquid + Main.tile[this.x - 2, this.y].liquid + Main.tile[this.x + 2, this.y].liquid + Main.tile[this.x, this.y].liquid) + num2);
							num = (float)Math.Round((double)(num / 5f));
							Main.tile[this.x - 1, this.y].lava = Main.tile[this.x, this.y].lava;
							if (Main.tile[this.x - 1, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x - 1, this.y);
								Main.tile[this.x - 1, this.y].liquid = (byte)num;
							}
							else
							{
								num4++;
							}
							Main.tile[this.x + 1, this.y].lava = Main.tile[this.x, this.y].lava;
							if (Main.tile[this.x + 1, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x + 1, this.y);
								Main.tile[this.x + 1, this.y].liquid = (byte)num;
							}
							else
							{
								num4++;
							}
							Main.tile[this.x - 2, this.y].lava = Main.tile[this.x, this.y].lava;
							if (Main.tile[this.x - 2, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x - 2, this.y);
								Main.tile[this.x - 2, this.y].liquid = (byte)num;
							}
							else
							{
								num4++;
							}
							Main.tile[this.x + 2, this.y].lava = Main.tile[this.x, this.y].lava;
							if (Main.tile[this.x + 2, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x + 2, this.y);
								Main.tile[this.x + 2, this.y].liquid = (byte)num;
							}
							else
							{
								num4++;
							}
							if (Main.tile[this.x - 1, this.y].liquid != (byte)num || Main.tile[this.x, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x - 1, this.y);
							}
							if (Main.tile[this.x + 1, this.y].liquid != (byte)num || Main.tile[this.x, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x + 1, this.y);
							}
							if (Main.tile[this.x - 2, this.y].liquid != (byte)num || Main.tile[this.x, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x - 2, this.y);
							}
							if (Main.tile[this.x + 2, this.y].liquid != (byte)num || Main.tile[this.x, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x + 2, this.y);
							}
							if (num4 != 4 || Main.tile[this.x, this.y - 1].liquid <= 0)
							{
								Main.tile[this.x, this.y].liquid = (byte)num;
							}
						}
					}
					else
					{
						if (flag3)
						{
							num = (float)((int)(Main.tile[this.x - 1, this.y].liquid + Main.tile[this.x + 1, this.y].liquid + Main.tile[this.x - 2, this.y].liquid + Main.tile[this.x, this.y].liquid) + num2);
							num = (float)Math.Round((double)(num / 4f) + 0.001);
							Main.tile[this.x - 1, this.y].lava = Main.tile[this.x, this.y].lava;
							if (Main.tile[this.x - 1, this.y].liquid != (byte)num || Main.tile[this.x, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x - 1, this.y);
								Main.tile[this.x - 1, this.y].liquid = (byte)num;
							}
							Main.tile[this.x + 1, this.y].lava = Main.tile[this.x, this.y].lava;
							if (Main.tile[this.x + 1, this.y].liquid != (byte)num || Main.tile[this.x, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x + 1, this.y);
								Main.tile[this.x + 1, this.y].liquid = (byte)num;
							}
							Main.tile[this.x - 2, this.y].lava = Main.tile[this.x, this.y].lava;
							if (Main.tile[this.x - 2, this.y].liquid != (byte)num || Main.tile[this.x, this.y].liquid != (byte)num)
							{
								Main.tile[this.x - 2, this.y].liquid = (byte)num;
								Liquid.AddWater(this.x - 2, this.y);
							}
							Main.tile[this.x, this.y].liquid = (byte)num;
						}
						else
						{
							if (flag4)
							{
								num = (float)((int)(Main.tile[this.x - 1, this.y].liquid + Main.tile[this.x + 1, this.y].liquid + Main.tile[this.x + 2, this.y].liquid + Main.tile[this.x, this.y].liquid) + num2);
								num = (float)Math.Round((double)(num / 4f) + 0.001);
								Main.tile[this.x - 1, this.y].lava = Main.tile[this.x, this.y].lava;
								if (Main.tile[this.x - 1, this.y].liquid != (byte)num || Main.tile[this.x, this.y].liquid != (byte)num)
								{
									Liquid.AddWater(this.x - 1, this.y);
									Main.tile[this.x - 1, this.y].liquid = (byte)num;
								}
								Main.tile[this.x + 1, this.y].lava = Main.tile[this.x, this.y].lava;
								if (Main.tile[this.x + 1, this.y].liquid != (byte)num || Main.tile[this.x, this.y].liquid != (byte)num)
								{
									Liquid.AddWater(this.x + 1, this.y);
									Main.tile[this.x + 1, this.y].liquid = (byte)num;
								}
								Main.tile[this.x + 2, this.y].lava = Main.tile[this.x, this.y].lava;
								if (Main.tile[this.x + 2, this.y].liquid != (byte)num || Main.tile[this.x, this.y].liquid != (byte)num)
								{
									Main.tile[this.x + 2, this.y].liquid = (byte)num;
									Liquid.AddWater(this.x + 2, this.y);
								}
								Main.tile[this.x, this.y].liquid = (byte)num;
							}
							else
							{
								num = (float)((int)(Main.tile[this.x - 1, this.y].liquid + Main.tile[this.x + 1, this.y].liquid + Main.tile[this.x, this.y].liquid) + num2);
								num = (float)Math.Round((double)(num / 3f) + 0.001);
								Main.tile[this.x - 1, this.y].lava = Main.tile[this.x, this.y].lava;
								if (Main.tile[this.x - 1, this.y].liquid != (byte)num)
								{
									Main.tile[this.x - 1, this.y].liquid = (byte)num;
								}
								if (Main.tile[this.x, this.y].liquid != (byte)num || Main.tile[this.x - 1, this.y].liquid != (byte)num)
								{
									Liquid.AddWater(this.x - 1, this.y);
								}
								Main.tile[this.x + 1, this.y].lava = Main.tile[this.x, this.y].lava;
								if (Main.tile[this.x + 1, this.y].liquid != (byte)num)
								{
									Main.tile[this.x + 1, this.y].liquid = (byte)num;
								}
								if (Main.tile[this.x, this.y].liquid != (byte)num || Main.tile[this.x + 1, this.y].liquid != (byte)num)
								{
									Liquid.AddWater(this.x + 1, this.y);
								}
								Main.tile[this.x, this.y].liquid = (byte)num;
							}
						}
					}
				}
				else
				{
					if (flag)
					{
						num = (float)((int)(Main.tile[this.x - 1, this.y].liquid + Main.tile[this.x, this.y].liquid) + num2);
						num = (float)Math.Round((double)(num / 2f) + 0.001);
						if (Main.tile[this.x - 1, this.y].liquid != (byte)num)
						{
							Main.tile[this.x - 1, this.y].liquid = (byte)num;
						}
						Main.tile[this.x - 1, this.y].lava = Main.tile[this.x, this.y].lava;
						if (Main.tile[this.x, this.y].liquid != (byte)num || Main.tile[this.x - 1, this.y].liquid != (byte)num)
						{
							Liquid.AddWater(this.x - 1, this.y);
						}
						Main.tile[this.x, this.y].liquid = (byte)num;
					}
					else
					{
						if (flag2)
						{
							num = (float)((int)(Main.tile[this.x + 1, this.y].liquid + Main.tile[this.x, this.y].liquid) + num2);
							num = (float)Math.Round((double)(num / 2f) + 0.001);
							if (Main.tile[this.x + 1, this.y].liquid != (byte)num)
							{
								Main.tile[this.x + 1, this.y].liquid = (byte)num;
							}
							Main.tile[this.x + 1, this.y].lava = Main.tile[this.x, this.y].lava;
							if (Main.tile[this.x, this.y].liquid != (byte)num || Main.tile[this.x + 1, this.y].liquid != (byte)num)
							{
								Liquid.AddWater(this.x + 1, this.y);
							}
							Main.tile[this.x, this.y].liquid = (byte)num;
						}
					}
				}
			}
			if (Main.tile[this.x, this.y].liquid == liquid)
			{
				this.kill++;
				return;
			}
			if (Main.tile[this.x, this.y].liquid == 254 && liquid == 255)
			{
				Main.tile[this.x, this.y].liquid = 255;
				this.kill++;
				return;
			}
			Liquid.AddWater(this.x, this.y - 1);
			this.kill = 0;
		}
		public static void StartPanic()
		{
			if (!Liquid.panicMode)
			{
				WorldGen.waterLine = Main.maxTilesY;
				Liquid.numLiquid = 0;
				LiquidBuffer.numLiquidBuffer = 0;
				Liquid.panicCounter = 0;
				Liquid.panicMode = true;
				Liquid.panicY = Main.maxTilesY - 3;
				if (Main.dedServ)
				{
					Console.WriteLine("Forcing water to settle.");
				}
			}
		}
		public static void UpdateLiquid()
		{
			if (Main.netMode == 2)
			{
				Liquid.cycles = 30;
				Liquid.maxLiquid = 6000;
			}
			if (!WorldGen.gen)
			{
				if (!Liquid.panicMode)
				{
					if (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > 4000)
					{
						Liquid.panicCounter++;
						if (Liquid.panicCounter > 1800 || Liquid.numLiquid + LiquidBuffer.numLiquidBuffer > 13500)
						{
							Liquid.StartPanic();
						}
					}
					else
					{
						Liquid.panicCounter = 0;
					}
				}
				if (Liquid.panicMode)
				{
					int num = 0;
					while (Liquid.panicY >= 3 && num < 5)
					{
						num++;
						Liquid.QuickWater(0, Liquid.panicY, Liquid.panicY);
						Liquid.panicY--;
						if (Liquid.panicY < 3)
						{
							Console.WriteLine("Water has been settled.");
							Liquid.panicCounter = 0;
							Liquid.panicMode = false;
							WorldGen.WaterCheck();
							if (Main.netMode == 2)
							{
								for (int i = 0; i < 255; i++)
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
						}
					}
					return;
				}
			}
			if (Liquid.quickSettle || Liquid.numLiquid > 2000)
			{
				Liquid.quickFall = true;
			}
			else
			{
				Liquid.quickFall = false;
			}
			Liquid.wetCounter++;
			int num2 = Liquid.maxLiquid / Liquid.cycles;
			int num3 = num2 * (Liquid.wetCounter - 1);
			int num4 = num2 * Liquid.wetCounter;
			if (Liquid.wetCounter == Liquid.cycles)
			{
				num4 = Liquid.numLiquid;
			}
			if (num4 > Liquid.numLiquid)
			{
				num4 = Liquid.numLiquid;
				int arg_19C_0 = Main.netMode;
				Liquid.wetCounter = Liquid.cycles;
			}
			if (Liquid.quickFall)
			{
				for (int l = num3; l < num4; l++)
				{
					Main.liquid[l].delay = 10;
					Main.liquid[l].Update();
					Main.tile[Main.liquid[l].x, Main.liquid[l].y].skipLiquid = false;
				}
			}
			else
			{
				for (int m = num3; m < num4; m++)
				{
					if (!Main.tile[Main.liquid[m].x, Main.liquid[m].y].skipLiquid)
					{
						Main.liquid[m].Update();
					}
					else
					{
						Main.tile[Main.liquid[m].x, Main.liquid[m].y].skipLiquid = false;
					}
				}
			}
			if (Liquid.wetCounter >= Liquid.cycles)
			{
				Liquid.wetCounter = 0;
				for (int n = Liquid.numLiquid - 1; n >= 0; n--)
				{
					if (Main.liquid[n].kill > 3)
					{
						Liquid.DelWater(n);
					}
				}
				int num5 = Liquid.maxLiquid - (Liquid.maxLiquid - Liquid.numLiquid);
				if (num5 > LiquidBuffer.numLiquidBuffer)
				{
					num5 = LiquidBuffer.numLiquidBuffer;
				}
				for (int num6 = 0; num6 < num5; num6++)
				{
					Main.tile[Main.liquidBuffer[0].x, Main.liquidBuffer[0].y].checkingLiquid = false;
					Liquid.AddWater(Main.liquidBuffer[0].x, Main.liquidBuffer[0].y);
					LiquidBuffer.DelBuffer(0);
				}
				if (Liquid.numLiquid > 0 && Liquid.numLiquid > Liquid.stuckAmount - 50 && Liquid.numLiquid < Liquid.stuckAmount + 50)
				{
					Liquid.stuckCount++;
					if (Liquid.stuckCount >= 10000)
					{
						Liquid.stuck = true;
						for (int num7 = Liquid.numLiquid - 1; num7 >= 0; num7--)
						{
							Liquid.DelWater(num7);
						}
						Liquid.stuck = false;
						Liquid.stuckCount = 0;
						return;
					}
				}
				else
				{
					Liquid.stuckCount = 0;
					Liquid.stuckAmount = Liquid.numLiquid;
				}
			}
		}
		public static void AddWater(int x, int y)
		{
			if (Main.tile[x, y].checkingLiquid)
			{
				return;
			}
			if (x >= Main.maxTilesX - 5 || y >= Main.maxTilesY - 5)
			{
				return;
			}
			if (x < 5 || y < 5)
			{
				return;
			}
			if (Main.tile[x, y] == null)
			{
				return;
			}
			if (Main.tile[x, y].liquid == 0)
			{
				return;
			}
			if (Liquid.numLiquid >= Liquid.maxLiquid - 1)
			{
				LiquidBuffer.AddBuffer(x, y);
				return;
			}
			Main.tile[x, y].checkingLiquid = true;
			Main.liquid[Liquid.numLiquid].kill = 0;
			Main.liquid[Liquid.numLiquid].x = x;
			Main.liquid[Liquid.numLiquid].y = y;
			Main.liquid[Liquid.numLiquid].delay = 0;
			Main.tile[x, y].skipLiquid = false;
			Liquid.numLiquid++;
			if (Main.netMode == 2 && Liquid.numLiquid < Liquid.maxLiquid / 3)
			{
				NetMessage.sendWater(x, y);
			}
			if (Main.tile[x, y].active && (Main.tileWaterDeath[(int)Main.tile[x, y].type] || (Main.tile[x, y].lava && Main.tileLavaDeath[(int)Main.tile[x, y].type])))
			{
				if (Main.tile[x, y].type == 4 && Main.tile[x, y].frameY == 176)
				{
					return;
				}
				if (WorldGen.gen)
				{
					Main.tile[x, y].active = false;
					return;
				}
				WorldGen.KillTile(x, y, false, false, false);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(17, -1, -1, "", 0, (float)x, (float)y, 0f, 0);
				}
			}
		}
		public static void LavaCheck(int x, int y)
		{
			if ((Main.tile[x - 1, y].liquid > 0 && !Main.tile[x - 1, y].lava) || (Main.tile[x + 1, y].liquid > 0 && !Main.tile[x + 1, y].lava) || (Main.tile[x, y - 1].liquid > 0 && !Main.tile[x, y - 1].lava))
			{
				int num = 0;
				if (!Main.tile[x - 1, y].lava)
				{
					num += (int)Main.tile[x - 1, y].liquid;
					Main.tile[x - 1, y].liquid = 0;
				}
				if (!Main.tile[x + 1, y].lava)
				{
					num += (int)Main.tile[x + 1, y].liquid;
					Main.tile[x + 1, y].liquid = 0;
				}
				if (!Main.tile[x, y - 1].lava)
				{
					num += (int)Main.tile[x, y - 1].liquid;
					Main.tile[x, y - 1].liquid = 0;
				}
				if (num >= 32 && !Main.tile[x, y].active)
				{
					Main.tile[x, y].liquid = 0;
					Main.tile[x, y].lava = false;
					WorldGen.PlaceTile(x, y, 56, true, true, -1, 0);
					WorldGen.SquareTileFrame(x, y, true);
					if (Main.netMode == 2)
					{
						NetMessage.SendTileSquare(-1, x - 1, y - 1, 3);
						return;
					}
				}
			}
			else
			{
				if (Main.tile[x, y + 1].liquid > 0 && !Main.tile[x, y + 1].lava && !Main.tile[x, y + 1].active)
				{
					Main.tile[x, y].liquid = 0;
					Main.tile[x, y].lava = false;
					Main.tile[x, y + 1].liquid = 0;
					WorldGen.PlaceTile(x, y + 1, 56, true, true, -1, 0);
					WorldGen.SquareTileFrame(x, y + 1, true);
					if (Main.netMode == 2)
					{
						NetMessage.SendTileSquare(-1, x - 1, y, 3);
					}
				}
			}
		}
		public static void NetAddWater(int x, int y)
		{
			if (x >= Main.maxTilesX - 5 || y >= Main.maxTilesY - 5)
			{
				return;
			}
			if (x < 5 || y < 5)
			{
				return;
			}
			if (Main.tile[x, y] == null)
			{
				return;
			}
			if (Main.tile[x, y].liquid == 0)
			{
				return;
			}
			for (int i = 0; i < Liquid.numLiquid; i++)
			{
				if (Main.liquid[i].x == x && Main.liquid[i].y == y)
				{
					Main.liquid[i].kill = 0;
					Main.tile[x, y].skipLiquid = true;
					return;
				}
			}
			if (Liquid.numLiquid >= Liquid.maxLiquid - 1)
			{
				LiquidBuffer.AddBuffer(x, y);
				return;
			}
			Main.tile[x, y].checkingLiquid = true;
			Main.tile[x, y].skipLiquid = true;
			Main.liquid[Liquid.numLiquid].kill = 0;
			Main.liquid[Liquid.numLiquid].x = x;
			Main.liquid[Liquid.numLiquid].y = y;
			Liquid.numLiquid++;
			int arg_10F_0 = Main.netMode;
			if (Main.tile[x, y].active && (Main.tileWaterDeath[(int)Main.tile[x, y].type] || (Main.tile[x, y].lava && Main.tileLavaDeath[(int)Main.tile[x, y].type])))
			{
				WorldGen.KillTile(x, y, false, false, false);
				if (Main.netMode == 2)
				{
					NetMessage.SendData(17, -1, -1, "", 0, (float)x, (float)y, 0f, 0);
				}
			}
		}
		public static void DelWater(int l)
		{
			int num = Main.liquid[l].x;
			int num2 = Main.liquid[l].y;
			if (Main.tile[num, num2].liquid < 2)
			{
				Main.tile[num, num2].liquid = 0;
				if (Main.tile[num - 1, num2].liquid < 2)
				{
					Main.tile[num - 1, num2].liquid = 0;
				}
				if (Main.tile[num + 1, num2].liquid < 2)
				{
					Main.tile[num + 1, num2].liquid = 0;
				}
			}
			else
			{
				if (Main.tile[num, num2].liquid < 20)
				{
					if ((Main.tile[num - 1, num2].liquid < Main.tile[num, num2].liquid && (!Main.tile[num - 1, num2].active || !Main.tileSolid[(int)Main.tile[num - 1, num2].type] || Main.tileSolidTop[(int)Main.tile[num - 1, num2].type])) || (Main.tile[num + 1, num2].liquid < Main.tile[num, num2].liquid && (!Main.tile[num + 1, num2].active || !Main.tileSolid[(int)Main.tile[num + 1, num2].type] || Main.tileSolidTop[(int)Main.tile[num + 1, num2].type])) || (Main.tile[num, num2 + 1].liquid < 255 && (!Main.tile[num, num2 + 1].active || !Main.tileSolid[(int)Main.tile[num, num2 + 1].type] || Main.tileSolidTop[(int)Main.tile[num, num2 + 1].type])))
					{
						Main.tile[num, num2].liquid = 0;
					}
				}
				else
				{
					if (Main.tile[num, num2 + 1].liquid < 255 && (!Main.tile[num, num2 + 1].active || !Main.tileSolid[(int)Main.tile[num, num2 + 1].type] || Main.tileSolidTop[(int)Main.tile[num, num2 + 1].type]) && !Liquid.stuck)
					{
						Main.liquid[l].kill = 0;
						return;
					}
				}
			}
			if (Main.tile[num, num2].liquid < 250 && Main.tile[num, num2 - 1].liquid > 0)
			{
				Liquid.AddWater(num, num2 - 1);
			}
			if (Main.tile[num, num2].liquid == 0)
			{
				Main.tile[num, num2].lava = false;
			}
			else
			{
				if ((Main.tile[num + 1, num2].liquid > 0 && Main.tile[num + 1, num2 + 1].liquid < 250 && !Main.tile[num + 1, num2 + 1].active) || (Main.tile[num - 1, num2].liquid > 0 && Main.tile[num - 1, num2 + 1].liquid < 250 && !Main.tile[num - 1, num2 + 1].active))
				{
					Liquid.AddWater(num - 1, num2);
					Liquid.AddWater(num + 1, num2);
				}
				if (Main.tile[num, num2].lava)
				{
					Liquid.LavaCheck(num, num2);
					for (int i = num - 1; i <= num + 1; i++)
					{
						for (int j = num2 - 1; j <= num2 + 1; j++)
						{
							if (Main.tile[i, j].active)
							{
								if (Main.tile[i, j].type == 2 || Main.tile[i, j].type == 23 || Main.tile[i, j].type == 109)
								{
									Main.tile[i, j].type = 0;
									WorldGen.SquareTileFrame(i, j, true);
									if (Main.netMode == 2)
									{
										NetMessage.SendTileSquare(-1, num, num2, 3);
									}
								}
								else
								{
									if (Main.tile[i, j].type == 60 || Main.tile[i, j].type == 70)
									{
										Main.tile[i, j].type = 59;
										WorldGen.SquareTileFrame(i, j, true);
										if (Main.netMode == 2)
										{
											NetMessage.SendTileSquare(-1, num, num2, 3);
										}
									}
								}
							}
						}
					}
				}
			}
			if (Main.netMode == 2)
			{
				NetMessage.sendWater(num, num2);
			}
			Liquid.numLiquid--;
			Main.tile[Main.liquid[l].x, Main.liquid[l].y].checkingLiquid = false;
			Main.liquid[l].x = Main.liquid[Liquid.numLiquid].x;
			Main.liquid[l].y = Main.liquid[Liquid.numLiquid].y;
			Main.liquid[l].kill = Main.liquid[Liquid.numLiquid].kill;
			if (Main.tileAlch[(int)Main.tile[num, num2].type])
			{
				WorldGen.CheckAlch(num, num2);
			}
		}
	}
}
