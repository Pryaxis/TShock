using System;
namespace Terraria
{
	public class Sign
	{
		public const int maxSigns = 1000;
		public int x;
		public int y;
		public string text;
		public object Clone()
		{
			return base.MemberwiseClone();
		}
		public static void KillSign(int x, int y)
		{
			for (int i = 0; i < 1000; i++)
			{
				if (Main.sign[i] != null && Main.sign[i].x == x && Main.sign[i].y == y)
				{
					Main.sign[i] = null;
				}
			}
		}
		public static int ReadSign(int i, int j)
		{
			int k = (int)(Main.tile[i, j].frameX / 18);
			int num = (int)(Main.tile[i, j].frameY / 18);
			while (k > 1)
			{
				k -= 2;
			}
			int num2 = i - k;
			int num3 = j - num;
			if (Main.tile[num2, num3].type != 55 && Main.tile[num2, num3].type != 85)
			{
				Sign.KillSign(num2, num3);
				return -1;
			}
			int num4 = -1;
			for (int l = 0; l < 1000; l++)
			{
				if (Main.sign[l] != null && Main.sign[l].x == num2 && Main.sign[l].y == num3)
				{
					num4 = l;
					break;
				}
			}
			if (num4 < 0)
			{
				for (int m = 0; m < 1000; m++)
				{
					if (Main.sign[m] == null)
					{
						num4 = m;
						Main.sign[m] = new Sign();
						Main.sign[m].x = num2;
						Main.sign[m].y = num3;
						Main.sign[m].text = "";
						break;
					}
				}
			}
			return num4;
		}
		public static void TextSign(int i, string text)
		{
			if (Main.tile[Main.sign[i].x, Main.sign[i].y] == null || !Main.tile[Main.sign[i].x, Main.sign[i].y].active || (Main.tile[Main.sign[i].x, Main.sign[i].y].type != 55 && Main.tile[Main.sign[i].x, Main.sign[i].y].type != 85))
			{
				Main.sign[i] = null;
				return;
			}
			Main.sign[i].text = text;
		}
	}
}
