
using System;
namespace Terraria
{
	public class Chest
	{
		public static int maxItems = 20;
		public Item[] item = new Item[Chest.maxItems];
		public int x;
		public int y;
		public object Clone()
		{
			return base.MemberwiseClone();
		}
		public static void Unlock(int X, int Y)
		{
			Main.PlaySound(22, X * 16, Y * 16, 1);
			for (int i = X; i <= X + 1; i++)
			{
				for (int j = Y; j <= Y + 1; j++)
				{
					if ((Main.tile[i, j].frameX >= 72 && Main.tile[i, j].frameX <= 106) || (Main.tile[i, j].frameX >= 144 && Main.tile[i, j].frameX <= 178))
					{
                        Main.tile[i, j].frameX -= 36;
						for (int k = 0; k < 4; k++)
						{
							Dust.NewDust(new Vector2((float)(i * 16), (float)(j * 16)), 16, 16, 11, 0f, 0f, 0, default(Color), 1f);
						}
					}
				}
			}
		}
		public static int UsingChest(int i)
		{
			if (Main.chest[i] != null)
			{
				for (int j = 0; j < 255; j++)
				{
					if (Main.player[j].active && Main.player[j].chest == i)
					{
						return j;
					}
				}
			}
			return -1;
		}
		public static int FindChest(int X, int Y)
		{
			for (int i = 0; i < 1000; i++)
			{
				if (Main.chest[i] != null && Main.chest[i].x == X && Main.chest[i].y == Y)
				{
					return i;
				}
			}
			return -1;
		}
		public static int CreateChest(int X, int Y)
		{
			for (int i = 0; i < 1000; i++)
			{
				if (Main.chest[i] != null && Main.chest[i].x == X && Main.chest[i].y == Y)
				{
					return -1;
				}
			}
			for (int j = 0; j < 1000; j++)
			{
				if (Main.chest[j] == null)
				{
					Main.chest[j] = new Chest();
					Main.chest[j].x = X;
					Main.chest[j].y = Y;
					for (int k = 0; k < Chest.maxItems; k++)
					{
						Main.chest[j].item[k] = new Item();
					}
					return j;
				}
			}
			return -1;
		}
		public static bool DestroyChest(int X, int Y)
		{
			for (int i = 0; i < 1000; i++)
			{
				if (Main.chest[i] != null && Main.chest[i].x == X && Main.chest[i].y == Y)
				{
					for (int j = 0; j < Chest.maxItems; j++)
					{
						if (Main.chest[i].item[j].type > 0 && Main.chest[i].item[j].stack > 0)
						{
							return false;
						}
					}
					Main.chest[i] = null;
					return true;
				}
			}
			return true;
		}
		public void AddShop(Item newItem)
		{
			int i = 0;
			while (i < 19)
			{
				if (this.item[i] == null || this.item[i].type == 0)
				{
					this.item[i] = (Item)newItem.Clone();
					this.item[i].buyOnce = true;
					if (this.item[i].value <= 0)
					{
						break;
					}
					this.item[i].value = this.item[i].value / 5;
					if (this.item[i].value < 1)
					{
						this.item[i].value = 1;
						return;
					}
					break;
				}
				else
				{
					i++;
				}
			}
		}
		public void SetupShop(int type)
		{
			for (int i = 0; i < Chest.maxItems; i++)
			{
				this.item[i] = new Item();
			}
			if (type == 1)
			{
				int num = 0;
				this.item[num].SetDefaults("Mining Helmet");
				num++;
				this.item[num].SetDefaults("Piggy Bank");
				num++;
				this.item[num].SetDefaults("Iron Anvil");
				num++;
				this.item[num].SetDefaults("Copper Pickaxe");
				num++;
				this.item[num].SetDefaults("Copper Axe");
				num++;
				this.item[num].SetDefaults("Torch");
				num++;
				this.item[num].SetDefaults("Lesser Healing Potion");
				num++;
				if (Main.player[Main.myPlayer].statManaMax == 200)
				{
					this.item[num].SetDefaults("Lesser Mana Potion");
					num++;
				}
				this.item[num].SetDefaults("Wooden Arrow");
				num++;
				this.item[num].SetDefaults("Shuriken");
				num++;
				if (Main.bloodMoon)
				{
					this.item[num].SetDefaults("Throwing Knife");
					num++;
				}
				if (!Main.dayTime)
				{
					this.item[num].SetDefaults("Glowstick");
					num++;
				}
				if (NPC.downedBoss3)
				{
					this.item[num].SetDefaults("Safe");
					num++;
				}
				if (Main.hardMode)
				{
					this.item[num].SetDefaults(488, false);
					num++;
					return;
				}
			}
			else
			{
				if (type == 2)
				{
					int num2 = 0;
					this.item[num2].SetDefaults("Musket Ball");
					num2++;
					if (Main.bloodMoon || Main.hardMode)
					{
						this.item[num2].SetDefaults("Silver Bullet");
						num2++;
					}
					if ((NPC.downedBoss2 && !Main.dayTime) || Main.hardMode)
					{
						this.item[num2].SetDefaults(47, false);
						num2++;
					}
					this.item[num2].SetDefaults("Flintlock Pistol");
					num2++;
					this.item[num2].SetDefaults("Minishark");
					num2++;
					if (!Main.dayTime)
					{
						this.item[num2].SetDefaults(324, false);
						num2++;
					}
					if (Main.hardMode)
					{
						this.item[num2].SetDefaults(534, false);
					}
					num2++;
					return;
				}
				if (type == 3)
				{
					int num3 = 0;
					if (Main.bloodMoon)
					{
						this.item[num3].SetDefaults(67, false);
						num3++;
						this.item[num3].SetDefaults(59, false);
						num3++;
					}
					else
					{
						this.item[num3].SetDefaults("Purification Powder");
						num3++;
						this.item[num3].SetDefaults("Grass Seeds");
						num3++;
						this.item[num3].SetDefaults("Sunflower");
						num3++;
					}
					this.item[num3].SetDefaults("Acorn");
					num3++;
					this.item[num3].SetDefaults(114, false);
					num3++;
					if (Main.hardMode)
					{
						this.item[num3].SetDefaults(369, false);
					}
					num3++;
					return;
				}
				if (type == 4)
				{
					int num4 = 0;
					this.item[num4].SetDefaults("Grenade");
					num4++;
					this.item[num4].SetDefaults("Bomb");
					num4++;
					this.item[num4].SetDefaults("Dynamite");
					num4++;
					if (Main.hardMode)
					{
						this.item[num4].SetDefaults("Hellfire Arrow");
					}
					num4++;
					return;
				}
				if (type == 5)
				{
					int num5 = 0;
					this.item[num5].SetDefaults(254, false);
					num5++;
					if (Main.dayTime)
					{
						this.item[num5].SetDefaults(242, false);
						num5++;
					}
					if (Main.moonPhase == 0)
					{
						this.item[num5].SetDefaults(245, false);
						num5++;
						this.item[num5].SetDefaults(246, false);
						num5++;
					}
					else
					{
						if (Main.moonPhase == 1)
						{
							this.item[num5].SetDefaults(325, false);
							num5++;
							this.item[num5].SetDefaults(326, false);
							num5++;
						}
					}
					this.item[num5].SetDefaults(269, false);
					num5++;
					this.item[num5].SetDefaults(270, false);
					num5++;
					this.item[num5].SetDefaults(271, false);
					num5++;
					if (NPC.downedClown)
					{
						this.item[num5].SetDefaults(503, false);
						num5++;
						this.item[num5].SetDefaults(504, false);
						num5++;
						this.item[num5].SetDefaults(505, false);
						num5++;
					}
					if (Main.bloodMoon)
					{
						this.item[num5].SetDefaults(322, false);
						num5++;
						return;
					}
				}
				else
				{
					if (type == 6)
					{
						int num6 = 0;
						this.item[num6].SetDefaults(128, false);
						num6++;
						this.item[num6].SetDefaults(486, false);
						num6++;
						this.item[num6].SetDefaults(398, false);
						num6++;
						this.item[num6].SetDefaults(84, false);
						num6++;
						this.item[num6].SetDefaults(407, false);
						num6++;
						this.item[num6].SetDefaults(161, false);
						num6++;
						return;
					}
					if (type == 7)
					{
						int num7 = 0;
						this.item[num7].SetDefaults(487, false);
						num7++;
						this.item[num7].SetDefaults(496, false);
						num7++;
						this.item[num7].SetDefaults(500, false);
						num7++;
						this.item[num7].SetDefaults(507, false);
						num7++;
						this.item[num7].SetDefaults(508, false);
						num7++;
						this.item[num7].SetDefaults(531, false);
						num7++;
						this.item[num7].SetDefaults(576, false);
						num7++;
						return;
					}
					if (type == 8)
					{
						int num8 = 0;
						this.item[num8].SetDefaults(509, false);
						num8++;
						this.item[num8].SetDefaults(510, false);
						num8++;
						this.item[num8].SetDefaults(530, false);
						num8++;
						this.item[num8].SetDefaults(513, false);
						num8++;
						this.item[num8].SetDefaults(538, false);
						num8++;
						this.item[num8].SetDefaults(529, false);
						num8++;
						this.item[num8].SetDefaults(541, false);
						num8++;
						this.item[num8].SetDefaults(542, false);
						num8++;
						this.item[num8].SetDefaults(543, false);
						num8++;
						return;
					}
					if (type == 9)
					{
						int num9 = 0;
						this.item[num9].SetDefaults(588, false);
						num9++;
						this.item[num9].SetDefaults(589, false);
						num9++;
						this.item[num9].SetDefaults(590, false);
						num9++;
						this.item[num9].SetDefaults(597, false);
						num9++;
						this.item[num9].SetDefaults(598, false);
						num9++;
						this.item[num9].SetDefaults(596, false);
						num9++;
					}
				}
			}
		}
	}
}
