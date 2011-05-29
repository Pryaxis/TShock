namespace Terraria
{
    using System;

    public class Chest
    {
        public Item[] item = new Item[maxItems];
        public static int maxItems = 20;
        public int x;
        public int y;

        public static int CreateChest(int X, int Y)
        {
            for (int i = 0; i < 0x3e8; i++)
            {
                if (((Main.chest[i] != null) && (Main.chest[i].x == X)) && (Main.chest[i].y == Y))
                {
                    return -1;
                }
            }
            for (int j = 0; j < 0x3e8; j++)
            {
                if (Main.chest[j] == null)
                {
                    Main.chest[j] = new Chest();
                    Main.chest[j].x = X;
                    Main.chest[j].y = Y;
                    for (int k = 0; k < maxItems; k++)
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
            for (int i = 0; i < 0x3e8; i++)
            {
                if (((Main.chest[i] != null) && (Main.chest[i].x == X)) && (Main.chest[i].y == Y))
                {
                    for (int j = 0; j < maxItems; j++)
                    {
                        if ((Main.chest[i].item[j].type > 0) && (Main.chest[i].item[j].stack > 0))
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

        public static int FindChest(int X, int Y)
        {
            for (int i = 0; i < 0x3e8; i++)
            {
                if (((Main.chest[i] != null) && (Main.chest[i].x == X)) && (Main.chest[i].y == Y))
                {
                    return i;
                }
            }
            return -1;
        }

        public void SetupShop(int type)
        {
            for (int i = 0; i < maxItems; i++)
            {
                this.item[i] = new Item();
            }
            if (type == 1)
            {
                int index = 0;
                this.item[index].SetDefaults("Mining Helmet");
                index++;
                this.item[index].SetDefaults("Piggy Bank");
                index++;
                this.item[index].SetDefaults("Iron Anvil");
                index++;
                this.item[index].SetDefaults("Copper Pickaxe");
                index++;
                this.item[index].SetDefaults("Copper Axe");
                index++;
                this.item[index].SetDefaults("Torch");
                index++;
                this.item[index].SetDefaults("Lesser Healing Potion");
                index++;
                this.item[index].SetDefaults("Wooden Arrow");
                index++;
                this.item[index].SetDefaults("Shuriken");
                index++;
            }
            else if (type == 2)
            {
                int num3 = 0;
                this.item[num3].SetDefaults("Musket Ball");
                num3++;
                this.item[num3].SetDefaults("Flintlock Pistol");
                num3++;
                this.item[num3].SetDefaults("Minishark");
                num3++;
            }
            else if (type == 3)
            {
                int num4 = 0;
                this.item[num4].SetDefaults("Purification Powder");
                num4++;
                this.item[num4].SetDefaults("Acorn");
                num4++;
                this.item[num4].SetDefaults("Grass Seeds");
                num4++;
                this.item[num4].SetDefaults("Sunflower");
                num4++;
                this.item[num4].SetDefaults(0x72);
                num4++;
            }
            else if (type == 4)
            {
                int num5 = 0;
                this.item[num5].SetDefaults("Grenade");
                num5++;
                this.item[num5].SetDefaults("Bomb");
                num5++;
                this.item[num5].SetDefaults("Dynamite");
                num5++;
            }
        }

        public static int UsingChest(int i)
        {
            if (Main.chest[i] != null)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Main.player[j].chest == i)
                    {
                        return j;
                    }
                }
            }
            return -1;
        }
    }
}

