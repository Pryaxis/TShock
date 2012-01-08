namespace Terraria
{
    public class Chest
    {
        public static int maxItems = 20;
        public Item[] item = new Item[maxItems];
        public int x;
        public int y;

        public object Clone()
        {
            return base.MemberwiseClone();
        }

        public static void Unlock(int X, int Y)
        {
            Main.PlaySound(22, X*16, Y*16, 1);
            for (int i = X; i <= X + 1; i++)
            {
                for (int j = Y; j <= Y + 1; j++)
                {
                    if ((Main.tile[i, j].frameX >= 72 && Main.tile[i, j].frameX <= 106) || (Main.tile[i, j].frameX >= 144 && Main.tile[i, j].frameX <= 178))
                    {
                        Main.tile[i, j].frameX -= 36;
                        for (int k = 0; k < 4; k++)
                        {
                            Dust.NewDust(new Vector2((i*16), (j*16)), 16, 16, 11, 0f, 0f, 0, default(Color), 1f);
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
            for (int i = 0; i < 1000; i++)
            {
                if (Main.chest[i] != null && Main.chest[i].x == X && Main.chest[i].y == Y)
                {
                    for (int j = 0; j < maxItems; j++)
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
                if (item[i] == null || item[i].type == 0)
                {
                    item[i] = (Item) newItem.Clone();
                    item[i].buyOnce = true;
                    if (item[i].value <= 0)
                    {
                        break;
                    }
                    item[i].value = item[i].value/5;
                    if (item[i].value < 1)
                    {
                        item[i].value = 1;
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
            for (int i = 0; i < maxItems; i++)
            {
                item[i] = new Item();
            }
            switch (type)
            {
                case 1:
                    {
                        int num = 0;
                        item[num].SetDefaults("Mining Helmet");
                        num++;
                        item[num].SetDefaults("Piggy Bank");
                        num++;
                        item[num].SetDefaults("Iron Anvil");
                        num++;
                        item[num].SetDefaults("Copper Pickaxe");
                        num++;
                        item[num].SetDefaults("Copper Axe");
                        num++;
                        item[num].SetDefaults("Torch");
                        num++;
                        item[num].SetDefaults("Lesser Healing Potion");
                        num++;
                        if (Main.player[Main.myPlayer].statManaMax == 200)
                        {
                            item[num].SetDefaults("Lesser Mana Potion");
                            num++;
                        }
                        item[num].SetDefaults("Wooden Arrow");
                        num++;
                        item[num].SetDefaults("Shuriken");
                        num++;
                        if (Main.bloodMoon)
                        {
                            item[num].SetDefaults("Throwing Knife");
                            num++;
                        }
                        if (!Main.dayTime)
                        {
                            item[num].SetDefaults("Glowstick");
                            num++;
                        }
                        if (NPC.downedBoss3)
                        {
                            item[num].SetDefaults("Safe");
                            num++;
                        }
                        if (Main.hardMode)
                        {
                            item[num].SetDefaults(488, false);
                            num++;
                            return;
                        }
                    }
                    break;
                case 2:
                    {
                        int num2 = 0;
                        item[num2].SetDefaults("Musket Ball");
                        num2++;
                        if (Main.bloodMoon || Main.hardMode)
                        {
                            item[num2].SetDefaults("Silver Bullet");
                            num2++;
                        }
                        if ((NPC.downedBoss2 && !Main.dayTime) || Main.hardMode)
                        {
                            item[num2].SetDefaults(47, false);
                            num2++;
                        }
                        item[num2].SetDefaults("Flintlock Pistol");
                        num2++;
                        item[num2].SetDefaults("Minishark");
                        num2++;
                        if (!Main.dayTime)
                        {
                            item[num2].SetDefaults(324, false);
                            num2++;
                        }
                        if (Main.hardMode)
                        {
                            item[num2].SetDefaults(534, false);
                        }
                        num2++;
                        return;
                    }
                case 3:
                    {
                        int num3 = 0;
                        if (Main.bloodMoon)
                        {
                            item[num3].SetDefaults(67, false);
                            num3++;
                            item[num3].SetDefaults(59, false);
                            num3++;
                        }
                        else
                        {
                            item[num3].SetDefaults("Purification Powder");
                            num3++;
                            item[num3].SetDefaults("Grass Seeds");
                            num3++;
                            item[num3].SetDefaults("Sunflower");
                            num3++;
                        }
                        item[num3].SetDefaults("Acorn");
                        num3++;
                        item[num3].SetDefaults(114, false);
                        num3++;
                        if (Main.hardMode)
                        {
                            item[num3].SetDefaults(369, false);
                        }
                        num3++;
                        return;
                    }
                case 4:
                    {
                        int num4 = 0;
                        item[num4].SetDefaults("Grenade");
                        num4++;
                        item[num4].SetDefaults("Bomb");
                        num4++;
                        item[num4].SetDefaults("Dynamite");
                        num4++;
                        if (Main.hardMode)
                        {
                            item[num4].SetDefaults("Hellfire Arrow");
                        }
                        num4++;
                        return;
                    }
                case 5:
                    {
                        int num5 = 0;
                        item[num5].SetDefaults(254, false);
                        num5++;
                        if (Main.dayTime)
                        {
                            item[num5].SetDefaults(242, false);
                            num5++;
                        }
                        if (Main.moonPhase == 0)
                        {
                            item[num5].SetDefaults(245, false);
                            num5++;
                            item[num5].SetDefaults(246, false);
                            num5++;
                        }
                        else
                        {
                            if (Main.moonPhase == 1)
                            {
                                item[num5].SetDefaults(325, false);
                                num5++;
                                item[num5].SetDefaults(326, false);
                                num5++;
                            }
                        }
                        item[num5].SetDefaults(269, false);
                        num5++;
                        item[num5].SetDefaults(270, false);
                        num5++;
                        item[num5].SetDefaults(271, false);
                        num5++;
                        if (NPC.downedClown)
                        {
                            item[num5].SetDefaults(503, false);
                            num5++;
                            item[num5].SetDefaults(504, false);
                            num5++;
                            item[num5].SetDefaults(505, false);
                            num5++;
                        }
                        if (Main.bloodMoon)
                        {
                            item[num5].SetDefaults(322, false);
                            num5++;
                            return;
                        }
                    }
                    break;
                case 6:
                    {
                        int num6 = 0;
                        item[num6].SetDefaults(128, false);
                        num6++;
                        item[num6].SetDefaults(486, false);
                        num6++;
                        item[num6].SetDefaults(398, false);
                        num6++;
                        item[num6].SetDefaults(84, false);
                        num6++;
                        item[num6].SetDefaults(407, false);
                        num6++;
                        item[num6].SetDefaults(161, false);
                        num6++;
                        return;
                    }
                case 7:
                    {
                        int num7 = 0;
                        item[num7].SetDefaults(487, false);
                        num7++;
                        item[num7].SetDefaults(496, false);
                        num7++;
                        item[num7].SetDefaults(500, false);
                        num7++;
                        item[num7].SetDefaults(507, false);
                        num7++;
                        item[num7].SetDefaults(508, false);
                        num7++;
                        item[num7].SetDefaults(531, false);
                        num7++;
                        item[num7].SetDefaults(576, false);
                        num7++;
                        return;
                    }
                case 8:
                    {
                        int num8 = 0;
                        item[num8].SetDefaults(509, false);
                        num8++;
                        item[num8].SetDefaults(510, false);
                        num8++;
                        item[num8].SetDefaults(530, false);
                        num8++;
                        item[num8].SetDefaults(513, false);
                        num8++;
                        item[num8].SetDefaults(538, false);
                        num8++;
                        item[num8].SetDefaults(529, false);
                        num8++;
                        item[num8].SetDefaults(541, false);
                        num8++;
                        item[num8].SetDefaults(542, false);
                        num8++;
                        item[num8].SetDefaults(543, false);
                        num8++;
                        return;
                    }
                case 9:
                    {
                        int num9 = 0;
                        item[num9].SetDefaults(588, false);
                        num9++;
                        item[num9].SetDefaults(589, false);
                        num9++;
                        item[num9].SetDefaults(590, false);
                        num9++;
                        item[num9].SetDefaults(597, false);
                        num9++;
                        item[num9].SetDefaults(598, false);
                        num9++;
                        item[num9].SetDefaults(596, false);
                        num9++;
                    }
                    break;
            }
        }
    }
}