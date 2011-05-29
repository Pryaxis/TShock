namespace Terraria
{
    using System;

    public class Sign
    {
        public const int maxSigns = 0x3e8;
        public string text;
        public int x;
        public int y;

        public static void KillSign(int x, int y)
        {
            for (int i = 0; i < 0x3e8; i++)
            {
                if (((Main.sign[i] != null) && (Main.sign[i].x == x)) && (Main.sign[i].y == y))
                {
                    Main.sign[i] = null;
                }
            }
        }

        public static int ReadSign(int i, int j)
        {
            int num = Main.tile[i, j].frameX / 0x12;
            int num2 = Main.tile[i, j].frameY / 0x12;
            while (num > 1)
            {
                num -= 2;
            }
            int x = i - num;
            int y = j - num2;
            if (Main.tile[x, y].type != 0x37)
            {
                KillSign(x, y);
                return -1;
            }
            int num5 = -1;
            for (int k = 0; k < 0x3e8; k++)
            {
                if (((Main.sign[k] != null) && (Main.sign[k].x == x)) && (Main.sign[k].y == y))
                {
                    num5 = k;
                    break;
                }
            }
            if (num5 < 0)
            {
                for (int m = 0; m < 0x3e8; m++)
                {
                    if (Main.sign[m] == null)
                    {
                        num5 = m;
                        Main.sign[m] = new Sign();
                        Main.sign[m].x = x;
                        Main.sign[m].y = y;
                        Main.sign[m].text = "";
                        return num5;
                    }
                }
            }
            return num5;
        }

        public static void TextSign(int i, string text)
        {
            if (((Main.tile[Main.sign[i].x, Main.sign[i].y] == null) || !Main.tile[Main.sign[i].x, Main.sign[i].y].active) || (Main.tile[Main.sign[i].x, Main.sign[i].y].type != 0x37))
            {
                Main.sign[i] = null;
            }
            else
            {
                Main.sign[i].text = text;
            }
        }
    }
}

