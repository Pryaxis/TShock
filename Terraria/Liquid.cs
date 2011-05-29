namespace Terraria
{
    using System;
    using System.Runtime.InteropServices;

    public class Liquid
    {
        public static int cycles = 10;
        public int delay;
        public int kill;
        public static int maxLiquid = 0x1388;
        public static int numLiquid;
        public static int panicCounter = 0;
        public static bool panicMode = false;
        public static int panicY = 0;
        public static bool quickFall = false;
        public static bool quickSettle = false;
        public static int resLiquid = 0x1388;
        public static int skipCount = 0;
        public static bool stuck = false;
        public static int stuckAmount = 0;
        public static int stuckCount = 0;
        private static int wetCounter;
        public int x;
        public int y;

        public static void AddWater(int x, int y)
        {
            if ((((!Main.tile[x, y].checkingLiquid && ((x < (Main.maxTilesX - 5)) && (y < (Main.maxTilesY - 5)))) && ((x >= 5) && (y >= 5))) && (Main.tile[x, y] != null)) && (Main.tile[x, y].liquid != 0))
            {
                if (numLiquid >= (maxLiquid - 1))
                {
                    LiquidBuffer.AddBuffer(x, y);
                }
                else
                {
                    Main.tile[x, y].checkingLiquid = true;
                    Main.liquid[numLiquid].kill = 0;
                    Main.liquid[numLiquid].x = x;
                    Main.liquid[numLiquid].y = y;
                    Main.liquid[numLiquid].delay = 0;
                    Main.tile[x, y].skipLiquid = false;
                    numLiquid++;
                    if (Main.netMode == 2)
                    {
                        NetMessage.sendWater(x, y);
                    }
                    if (Main.tile[x, y].active && (Main.tileWaterDeath[Main.tile[x, y].type] || (Main.tile[x, y].lava && Main.tileLavaDeath[Main.tile[x, y].type])))
                    {
                        if (WorldGen.gen)
                        {
                            Main.tile[x, y].active = false;
                        }
                        else
                        {
                            WorldGen.KillTile(x, y, false, false, false);
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendData(0x11, -1, -1, "", 0, (float) x, (float) y, 0f);
                            }
                        }
                    }
                }
            }
        }

        public static void DelWater(int l)
        {
            int x = Main.liquid[l].x;
            int y = Main.liquid[l].y;
            if (Main.tile[x, y].liquid < 2)
            {
                Main.tile[x, y].liquid = 0;
            }
            else if (Main.tile[x, y].liquid < 20)
            {
                if ((((Main.tile[x - 1, y].liquid < Main.tile[x, y].liquid) && ((!Main.tile[x - 1, y].active || !Main.tileSolid[Main.tile[x - 1, y].type]) || Main.tileSolidTop[Main.tile[x - 1, y].type])) || ((Main.tile[x + 1, y].liquid < Main.tile[x, y].liquid) && ((!Main.tile[x + 1, y].active || !Main.tileSolid[Main.tile[x + 1, y].type]) || Main.tileSolidTop[Main.tile[x + 1, y].type]))) || ((Main.tile[x, y + 1].liquid < 0xff) && ((!Main.tile[x, y + 1].active || !Main.tileSolid[Main.tile[x, y + 1].type]) || Main.tileSolidTop[Main.tile[x, y + 1].type])))
                {
                    Main.tile[x, y].liquid = 0;
                }
            }
            else if (((Main.tile[x, y + 1].liquid < 0xff) && ((!Main.tile[x, y + 1].active || !Main.tileSolid[Main.tile[x, y + 1].type]) || Main.tileSolidTop[Main.tile[x, y + 1].type])) && !stuck)
            {
                Main.liquid[l].kill = 0;
                return;
            }
            if (Main.tile[x, y].liquid == 0)
            {
                Main.tile[x, y].lava = false;
            }
            else if (Main.tile[x, y].lava)
            {
                LavaCheck(x, y);
            }
            if (Main.netMode == 2)
            {
                NetMessage.sendWater(x, y);
            }
            numLiquid--;
            Main.tile[Main.liquid[l].x, Main.liquid[l].y].checkingLiquid = false;
            Main.liquid[l].x = Main.liquid[numLiquid].x;
            Main.liquid[l].y = Main.liquid[numLiquid].y;
            Main.liquid[l].kill = Main.liquid[numLiquid].kill;
        }

        public static void LavaCheck(int x, int y)
        {
            if ((((Main.tile[x - 1, y].liquid > 0) && !Main.tile[x - 1, y].lava) || ((Main.tile[x + 1, y].liquid > 0) && !Main.tile[x + 1, y].lava)) || ((Main.tile[x, y - 1].liquid > 0) && !Main.tile[x, y - 1].lava))
            {
                int num = 0;
                if (!Main.tile[x - 1, y].lava)
                {
                    num += Main.tile[x - 1, y].liquid;
                    Main.tile[x - 1, y].liquid = 0;
                }
                if (!Main.tile[x + 1, y].lava)
                {
                    num += Main.tile[x + 1, y].liquid;
                    Main.tile[x + 1, y].liquid = 0;
                }
                if (!Main.tile[x, y - 1].lava)
                {
                    num += Main.tile[x, y - 1].liquid;
                    Main.tile[x, y - 1].liquid = 0;
                }
                if (num >= 0x80)
                {
                    Main.tile[x, y].liquid = 0;
                    Main.tile[x, y].lava = false;
                    WorldGen.PlaceTile(x, y, 0x38, true, true, -1);
                    WorldGen.SquareTileFrame(x, y, true);
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendTileSquare(-1, x - 1, y - 1, 3);
                    }
                }
            }
            else if ((Main.tile[x, y + 1].liquid > 0) && !Main.tile[x, y + 1].lava)
            {
                Main.tile[x, y].liquid = 0;
                Main.tile[x, y].lava = false;
                WorldGen.PlaceTile(x, y + 1, 0x38, true, true, -1);
                WorldGen.SquareTileFrame(x, y, true);
                if (Main.netMode == 2)
                {
                    NetMessage.SendTileSquare(-1, x - 1, y, 3);
                }
            }
        }

        public static void NetAddWater(int x, int y)
        {
            if (((((x < (Main.maxTilesX - 5)) && (y < (Main.maxTilesY - 5))) && ((x >= 5) && (y >= 5))) && (Main.tile[x, y] != null)) && (Main.tile[x, y].liquid != 0))
            {
                for (int i = 0; i < numLiquid; i++)
                {
                    if ((Main.liquid[i].x == x) && (Main.liquid[i].y == y))
                    {
                        Main.liquid[i].kill = 0;
                        Main.tile[x, y].skipLiquid = true;
                        return;
                    }
                }
                if (numLiquid >= (maxLiquid - 1))
                {
                    LiquidBuffer.AddBuffer(x, y);
                }
                else
                {
                    Main.tile[x, y].checkingLiquid = true;
                    Main.tile[x, y].skipLiquid = true;
                    Main.liquid[numLiquid].kill = 0;
                    Main.liquid[numLiquid].x = x;
                    Main.liquid[numLiquid].y = y;
                    numLiquid++;
                    if (Main.netMode == 2)
                    {
                        NetMessage.sendWater(x, y);
                    }
                    if (Main.tile[x, y].active && (Main.tileWaterDeath[Main.tile[x, y].type] || (Main.tile[x, y].lava && Main.tileLavaDeath[Main.tile[x, y].type])))
                    {
                        WorldGen.KillTile(x, y, false, false, false);
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendData(0x11, -1, -1, "", 0, (float) x, (float) y, 0f);
                        }
                    }
                }
            }
        }

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
                    float num3 = ((float) (maxY - i)) / ((float) ((maxY - minY) + 1));
                    num3 /= (float) verbose;
                    Main.statusText = "Settling liquids: " + ((int) ((num3 * 100f) + 1f)) + "%";
                }
                else if (verbose < 0)
                {
                    float num4 = ((float) (maxY - i)) / ((float) ((maxY - minY) + 1));
                    num4 /= (float) -verbose;
                    Main.statusText = "Creating underworld: " + ((int) ((num4 * 100f) + 1f)) + "%";
                }
                for (int j = 0; j < 2; j++)
                {
                    int num6 = 2;
                    int num7 = Main.maxTilesX - 2;
                    int num8 = 1;
                    if (j == 1)
                    {
                        num6 = Main.maxTilesX - 2;
                        num7 = 2;
                        num8 = -1;
                    }
                    for (int k = num6; k != num7; k += num8)
                    {
                        if (Main.tile[k, i].liquid <= 0)
                        {
                            continue;
                        }
                        int num10 = -num8;
                        bool flag = false;
                        int x = k;
                        int y = i;
                        bool lava = Main.tile[k, i].lava;
                        byte liquid = Main.tile[k, i].liquid;
                        Main.tile[k, i].liquid = 0;
                        bool flag3 = true;
                        int num14 = 0;
                        while ((flag3 && (x > 3)) && ((x < (Main.maxTilesX - 3)) && (y < (Main.maxTilesY - 3))))
                        {
                            flag3 = false;
                            while (((Main.tile[x, y + 1].liquid == 0) && (y < (Main.maxTilesY - 5))) && ((!Main.tile[x, y + 1].active || !Main.tileSolid[Main.tile[x, y + 1].type]) || Main.tileSolidTop[Main.tile[x, y + 1].type]))
                            {
                                flag = true;
                                num10 = num8;
                                num14 = 0;
                                flag3 = true;
                                y++;
                                if (y > WorldGen.waterLine)
                                {
                                    lava = true;
                                }
                            }
                            if (((Main.tile[x, y + 1].liquid > 0) && (Main.tile[x, y + 1].liquid < 0xff)) && (Main.tile[x, y + 1].lava == lava))
                            {
                                int num15 = 0xff - Main.tile[x, y + 1].liquid;
                                if (num15 > liquid)
                                {
                                    num15 = liquid;
                                }
                                Tile tile1 = Main.tile[x, y + 1];
                                tile1.liquid = (byte) (tile1.liquid + ((byte) num15));
                                liquid = (byte) (liquid - ((byte) num15));
                                if (liquid <= 0)
                                {
                                    num++;
                                    break;
                                }
                            }
                            if (num14 == 0)
                            {
                                if ((Main.tile[x + num10, y].liquid == 0) && ((!Main.tile[x + num10, y].active || !Main.tileSolid[Main.tile[x + num10, y].type]) || Main.tileSolidTop[Main.tile[x + num10, y].type]))
                                {
                                    num14 = num10;
                                }
                                else if ((Main.tile[x - num10, y].liquid == 0) && ((!Main.tile[x - num10, y].active || !Main.tileSolid[Main.tile[x - num10, y].type]) || Main.tileSolidTop[Main.tile[x - num10, y].type]))
                                {
                                    num14 = -num10;
                                }
                            }
                            if (((num14 != 0) && (Main.tile[x + num14, y].liquid == 0)) && ((!Main.tile[x + num14, y].active || !Main.tileSolid[Main.tile[x + num14, y].type]) || Main.tileSolidTop[Main.tile[x + num14, y].type]))
                            {
                                flag3 = true;
                                x += num14;
                            }
                            if (flag && !flag3)
                            {
                                flag = false;
                                flag3 = true;
                                num10 = -num8;
                                num14 = 0;
                            }
                        }
                        if ((k != x) && (i != y))
                        {
                            num++;
                        }
                        Main.tile[x, y].liquid = liquid;
                        Main.tile[x, y].lava = lava;
                        if ((Main.tile[x - 1, y].liquid > 0) && (Main.tile[x - 1, y].lava != lava))
                        {
                            if (lava)
                            {
                                LavaCheck(x, y);
                            }
                            else
                            {
                                LavaCheck(x - 1, y);
                            }
                        }
                        else if ((Main.tile[x + 1, y].liquid > 0) && (Main.tile[x + 1, y].lava != lava))
                        {
                            if (lava)
                            {
                                LavaCheck(x, y);
                            }
                            else
                            {
                                LavaCheck(x + 1, y);
                            }
                        }
                        else if ((Main.tile[x, y - 1].liquid > 0) && (Main.tile[x, y - 1].lava != lava))
                        {
                            if (lava)
                            {
                                LavaCheck(x, y);
                            }
                            else
                            {
                                LavaCheck(x, y - 1);
                            }
                        }
                        else if ((Main.tile[x, y + 1].liquid > 0) && (Main.tile[x, y + 1].lava != lava))
                        {
                            if (lava)
                            {
                                LavaCheck(x, y);
                            }
                            else
                            {
                                LavaCheck(x, y + 1);
                            }
                        }
                    }
                }
            }
            return (double) num;
        }

        public void Update()
        {
            if ((Main.tile[this.x, this.y].active && Main.tileSolid[Main.tile[this.x, this.y].type]) && !Main.tileSolidTop[Main.tile[this.x, this.y].type])
            {
                if (Main.tile[this.x, this.y].type != 10)
                {
                    Main.tile[this.x, this.y].liquid = 0;
                }
                this.kill = 9;
            }
            else
            {
                byte liquid = Main.tile[this.x, this.y].liquid;
                float num2 = 0f;
                if (Main.tile[this.x, this.y].liquid == 0)
                {
                    this.kill = 9;
                }
                else
                {
                    if (Main.tile[this.x, this.y].lava)
                    {
                        LavaCheck(this.x, this.y);
                        if (!quickFall)
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
                            AddWater(this.x - 1, this.y);
                        }
                        if (Main.tile[this.x + 1, this.y].lava)
                        {
                            AddWater(this.x + 1, this.y);
                        }
                        if (Main.tile[this.x, this.y - 1].lava)
                        {
                            AddWater(this.x, this.y - 1);
                        }
                        if (Main.tile[this.x, this.y + 1].lava)
                        {
                            AddWater(this.x, this.y + 1);
                        }
                    }
                    if ((((!Main.tile[this.x, this.y + 1].active || !Main.tileSolid[Main.tile[this.x, this.y + 1].type]) || Main.tileSolidTop[Main.tile[this.x, this.y + 1].type]) && ((Main.tile[this.x, this.y + 1].liquid <= 0) || (Main.tile[this.x, this.y + 1].lava == Main.tile[this.x, this.y].lava))) && (Main.tile[this.x, this.y + 1].liquid < 0xff))
                    {
                        num2 = 0xff - Main.tile[this.x, this.y + 1].liquid;
                        if (num2 > Main.tile[this.x, this.y].liquid)
                        {
                            num2 = Main.tile[this.x, this.y].liquid;
                        }
                        Tile tile1 = Main.tile[this.x, this.y];
                        tile1.liquid = (byte) (tile1.liquid - ((byte) num2));
                        Tile tile2 = Main.tile[this.x, this.y + 1];
                        tile2.liquid = (byte) (tile2.liquid + ((byte) num2));
                        Main.tile[this.x, this.y + 1].lava = Main.tile[this.x, this.y].lava;
                        AddWater(this.x, this.y + 1);
                        Main.tile[this.x, this.y + 1].skipLiquid = true;
                        Main.tile[this.x, this.y].skipLiquid = true;
                        if (Main.tile[this.x, this.y].liquid > 250)
                        {
                            Main.tile[this.x, this.y].liquid = 0xff;
                        }
                        else
                        {
                            AddWater(this.x - 1, this.y);
                            AddWater(this.x + 1, this.y);
                        }
                    }
                    if (Main.tile[this.x, this.y].liquid > 0)
                    {
                        bool flag = true;
                        bool flag2 = true;
                        bool flag3 = true;
                        bool flag4 = true;
                        if ((Main.tile[this.x - 1, this.y].active && Main.tileSolid[Main.tile[this.x - 1, this.y].type]) && !Main.tileSolidTop[Main.tile[this.x - 1, this.y].type])
                        {
                            flag = false;
                        }
                        else if ((Main.tile[this.x - 1, this.y].liquid > 0) && (Main.tile[this.x - 1, this.y].lava != Main.tile[this.x, this.y].lava))
                        {
                            flag = false;
                        }
                        else if ((Main.tile[this.x - 2, this.y].active && Main.tileSolid[Main.tile[this.x - 2, this.y].type]) && !Main.tileSolidTop[Main.tile[this.x - 2, this.y].type])
                        {
                            flag3 = false;
                        }
                        else if (Main.tile[this.x - 2, this.y].liquid == 0)
                        {
                            flag3 = false;
                        }
                        else if ((Main.tile[this.x - 2, this.y].liquid > 0) && (Main.tile[this.x - 2, this.y].lava != Main.tile[this.x, this.y].lava))
                        {
                            flag3 = false;
                        }
                        if ((Main.tile[this.x + 1, this.y].active && Main.tileSolid[Main.tile[this.x + 1, this.y].type]) && !Main.tileSolidTop[Main.tile[this.x + 1, this.y].type])
                        {
                            flag2 = false;
                        }
                        else if ((Main.tile[this.x + 1, this.y].liquid > 0) && (Main.tile[this.x + 1, this.y].lava != Main.tile[this.x, this.y].lava))
                        {
                            flag2 = false;
                        }
                        else if ((Main.tile[this.x + 2, this.y].active && Main.tileSolid[Main.tile[this.x + 2, this.y].type]) && !Main.tileSolidTop[Main.tile[this.x + 2, this.y].type])
                        {
                            flag4 = false;
                        }
                        else if (Main.tile[this.x + 2, this.y].liquid == 0)
                        {
                            flag4 = false;
                        }
                        else if ((Main.tile[this.x + 2, this.y].liquid > 0) && (Main.tile[this.x + 2, this.y].lava != Main.tile[this.x, this.y].lava))
                        {
                            flag4 = false;
                        }
                        int num3 = 0;
                        if (Main.tile[this.x, this.y].liquid < 3)
                        {
                            num3 = -1;
                        }
                        if (flag && flag2)
                        {
                            if (flag3 && flag4)
                            {
                                bool flag5 = true;
                                bool flag6 = true;
                                if ((Main.tile[this.x - 3, this.y].active && Main.tileSolid[Main.tile[this.x - 3, this.y].type]) && !Main.tileSolidTop[Main.tile[this.x - 3, this.y].type])
                                {
                                    flag5 = false;
                                }
                                else if (Main.tile[this.x - 3, this.y].liquid == 0)
                                {
                                    flag5 = false;
                                }
                                else if (Main.tile[this.x - 3, this.y].lava != Main.tile[this.x, this.y].lava)
                                {
                                    flag5 = false;
                                }
                                if ((Main.tile[this.x + 3, this.y].active && Main.tileSolid[Main.tile[this.x + 3, this.y].type]) && !Main.tileSolidTop[Main.tile[this.x + 3, this.y].type])
                                {
                                    flag6 = false;
                                }
                                else if (Main.tile[this.x + 3, this.y].liquid == 0)
                                {
                                    flag6 = false;
                                }
                                else if (Main.tile[this.x + 3, this.y].lava != Main.tile[this.x, this.y].lava)
                                {
                                    flag6 = false;
                                }
                                if (flag5 && flag6)
                                {
                                    num2 = ((((((Main.tile[this.x - 1, this.y].liquid + Main.tile[this.x + 1, this.y].liquid) + Main.tile[this.x - 2, this.y].liquid) + Main.tile[this.x + 2, this.y].liquid) + Main.tile[this.x - 3, this.y].liquid) + Main.tile[this.x + 3, this.y].liquid) + Main.tile[this.x, this.y].liquid) + num3;
                                    num2 = (float) Math.Round((double) (num2 / 7f));
                                    int num4 = 0;
                                    if (Main.tile[this.x - 1, this.y].liquid != ((byte) num2))
                                    {
                                        AddWater(this.x - 1, this.y);
                                        Main.tile[this.x - 1, this.y].liquid = (byte) num2;
                                    }
                                    else
                                    {
                                        num4++;
                                    }
                                    Main.tile[this.x - 1, this.y].lava = Main.tile[this.x, this.y].lava;
                                    if (Main.tile[this.x + 1, this.y].liquid != ((byte) num2))
                                    {
                                        AddWater(this.x + 1, this.y);
                                        Main.tile[this.x + 1, this.y].liquid = (byte) num2;
                                    }
                                    else
                                    {
                                        num4++;
                                    }
                                    Main.tile[this.x + 1, this.y].lava = Main.tile[this.x, this.y].lava;
                                    if (Main.tile[this.x - 2, this.y].liquid != ((byte) num2))
                                    {
                                        AddWater(this.x - 2, this.y);
                                        Main.tile[this.x - 2, this.y].liquid = (byte) num2;
                                    }
                                    else
                                    {
                                        num4++;
                                    }
                                    Main.tile[this.x - 2, this.y].lava = Main.tile[this.x, this.y].lava;
                                    if (Main.tile[this.x + 2, this.y].liquid != ((byte) num2))
                                    {
                                        AddWater(this.x + 2, this.y);
                                        Main.tile[this.x + 2, this.y].liquid = (byte) num2;
                                    }
                                    else
                                    {
                                        num4++;
                                    }
                                    Main.tile[this.x + 2, this.y].lava = Main.tile[this.x, this.y].lava;
                                    if (Main.tile[this.x - 3, this.y].liquid != ((byte) num2))
                                    {
                                        AddWater(this.x - 3, this.y);
                                        Main.tile[this.x - 3, this.y].liquid = (byte) num2;
                                    }
                                    else
                                    {
                                        num4++;
                                    }
                                    Main.tile[this.x - 3, this.y].lava = Main.tile[this.x, this.y].lava;
                                    if (Main.tile[this.x + 3, this.y].liquid != ((byte) num2))
                                    {
                                        AddWater(this.x + 3, this.y);
                                        Main.tile[this.x + 3, this.y].liquid = (byte) num2;
                                    }
                                    else
                                    {
                                        num4++;
                                    }
                                    Main.tile[this.x + 3, this.y].lava = Main.tile[this.x, this.y].lava;
                                    if ((Main.tile[this.x - 1, this.y].liquid != ((byte) num2)) || (Main.tile[this.x, this.y].liquid != ((byte) num2)))
                                    {
                                        AddWater(this.x - 1, this.y);
                                    }
                                    if ((Main.tile[this.x + 1, this.y].liquid != ((byte) num2)) || (Main.tile[this.x, this.y].liquid != ((byte) num2)))
                                    {
                                        AddWater(this.x + 1, this.y);
                                    }
                                    if ((Main.tile[this.x - 2, this.y].liquid != ((byte) num2)) || (Main.tile[this.x, this.y].liquid != ((byte) num2)))
                                    {
                                        AddWater(this.x - 2, this.y);
                                    }
                                    if ((Main.tile[this.x + 2, this.y].liquid != ((byte) num2)) || (Main.tile[this.x, this.y].liquid != ((byte) num2)))
                                    {
                                        AddWater(this.x + 2, this.y);
                                    }
                                    if ((Main.tile[this.x - 3, this.y].liquid != ((byte) num2)) || (Main.tile[this.x, this.y].liquid != ((byte) num2)))
                                    {
                                        AddWater(this.x - 3, this.y);
                                    }
                                    if ((Main.tile[this.x + 3, this.y].liquid != ((byte) num2)) || (Main.tile[this.x, this.y].liquid != ((byte) num2)))
                                    {
                                        AddWater(this.x + 3, this.y);
                                    }
                                    if ((num4 != 6) || (Main.tile[this.x, this.y - 1].liquid <= 0))
                                    {
                                        Main.tile[this.x, this.y].liquid = (byte) num2;
                                    }
                                }
                                else
                                {
                                    int num5 = 0;
                                    num2 = ((((Main.tile[this.x - 1, this.y].liquid + Main.tile[this.x + 1, this.y].liquid) + Main.tile[this.x - 2, this.y].liquid) + Main.tile[this.x + 2, this.y].liquid) + Main.tile[this.x, this.y].liquid) + num3;
                                    num2 = (float) Math.Round((double) (num2 / 5f));
                                    if (Main.tile[this.x - 1, this.y].liquid != ((byte) num2))
                                    {
                                        AddWater(this.x - 1, this.y);
                                        Main.tile[this.x - 1, this.y].liquid = (byte) num2;
                                    }
                                    else
                                    {
                                        num5++;
                                    }
                                    Main.tile[this.x - 1, this.y].lava = Main.tile[this.x, this.y].lava;
                                    if (Main.tile[this.x + 1, this.y].liquid != ((byte) num2))
                                    {
                                        AddWater(this.x + 1, this.y);
                                        Main.tile[this.x + 1, this.y].liquid = (byte) num2;
                                    }
                                    else
                                    {
                                        num5++;
                                    }
                                    Main.tile[this.x + 1, this.y].lava = Main.tile[this.x, this.y].lava;
                                    if (Main.tile[this.x - 2, this.y].liquid != ((byte) num2))
                                    {
                                        AddWater(this.x - 2, this.y);
                                        Main.tile[this.x - 2, this.y].liquid = (byte) num2;
                                    }
                                    else
                                    {
                                        num5++;
                                    }
                                    Main.tile[this.x - 2, this.y].lava = Main.tile[this.x, this.y].lava;
                                    if (Main.tile[this.x + 2, this.y].liquid != ((byte) num2))
                                    {
                                        AddWater(this.x + 2, this.y);
                                        Main.tile[this.x + 2, this.y].liquid = (byte) num2;
                                    }
                                    else
                                    {
                                        num5++;
                                    }
                                    if ((Main.tile[this.x - 1, this.y].liquid != ((byte) num2)) || (Main.tile[this.x, this.y].liquid != ((byte) num2)))
                                    {
                                        AddWater(this.x - 1, this.y);
                                    }
                                    if ((Main.tile[this.x + 1, this.y].liquid != ((byte) num2)) || (Main.tile[this.x, this.y].liquid != ((byte) num2)))
                                    {
                                        AddWater(this.x + 1, this.y);
                                    }
                                    if ((Main.tile[this.x - 2, this.y].liquid != ((byte) num2)) || (Main.tile[this.x, this.y].liquid != ((byte) num2)))
                                    {
                                        AddWater(this.x - 2, this.y);
                                    }
                                    if ((Main.tile[this.x + 2, this.y].liquid != ((byte) num2)) || (Main.tile[this.x, this.y].liquid != ((byte) num2)))
                                    {
                                        AddWater(this.x + 2, this.y);
                                    }
                                    Main.tile[this.x + 2, this.y].lava = Main.tile[this.x, this.y].lava;
                                    if ((num5 != 4) || (Main.tile[this.x, this.y - 1].liquid <= 0))
                                    {
                                        Main.tile[this.x, this.y].liquid = (byte) num2;
                                    }
                                }
                            }
                            else if (flag3)
                            {
                                num2 = (((Main.tile[this.x - 1, this.y].liquid + Main.tile[this.x + 1, this.y].liquid) + Main.tile[this.x - 2, this.y].liquid) + Main.tile[this.x, this.y].liquid) + num3;
                                num2 = (float) Math.Round((double) ((num2 / 4f) + 0.001));
                                if ((Main.tile[this.x - 1, this.y].liquid != ((byte) num2)) || (Main.tile[this.x, this.y].liquid != ((byte) num2)))
                                {
                                    AddWater(this.x - 1, this.y);
                                    Main.tile[this.x - 1, this.y].liquid = (byte) num2;
                                }
                                Main.tile[this.x - 1, this.y].lava = Main.tile[this.x, this.y].lava;
                                if ((Main.tile[this.x + 1, this.y].liquid != ((byte) num2)) || (Main.tile[this.x, this.y].liquid != ((byte) num2)))
                                {
                                    AddWater(this.x + 1, this.y);
                                    Main.tile[this.x + 1, this.y].liquid = (byte) num2;
                                }
                                Main.tile[this.x + 1, this.y].lava = Main.tile[this.x, this.y].lava;
                                if ((Main.tile[this.x - 2, this.y].liquid != ((byte) num2)) || (Main.tile[this.x, this.y].liquid != ((byte) num2)))
                                {
                                    Main.tile[this.x - 2, this.y].liquid = (byte) num2;
                                    AddWater(this.x - 2, this.y);
                                }
                                Main.tile[this.x - 2, this.y].lava = Main.tile[this.x, this.y].lava;
                                Main.tile[this.x, this.y].liquid = (byte) num2;
                            }
                            else if (flag4)
                            {
                                num2 = (((Main.tile[this.x - 1, this.y].liquid + Main.tile[this.x + 1, this.y].liquid) + Main.tile[this.x + 2, this.y].liquid) + Main.tile[this.x, this.y].liquid) + num3;
                                num2 = (float) Math.Round((double) ((num2 / 4f) + 0.001));
                                if ((Main.tile[this.x - 1, this.y].liquid != ((byte) num2)) || (Main.tile[this.x, this.y].liquid != ((byte) num2)))
                                {
                                    AddWater(this.x - 1, this.y);
                                    Main.tile[this.x - 1, this.y].liquid = (byte) num2;
                                }
                                Main.tile[this.x - 1, this.y].lava = Main.tile[this.x, this.y].lava;
                                if ((Main.tile[this.x + 1, this.y].liquid != ((byte) num2)) || (Main.tile[this.x, this.y].liquid != ((byte) num2)))
                                {
                                    AddWater(this.x + 1, this.y);
                                    Main.tile[this.x + 1, this.y].liquid = (byte) num2;
                                }
                                Main.tile[this.x + 1, this.y].lava = Main.tile[this.x, this.y].lava;
                                if ((Main.tile[this.x + 2, this.y].liquid != ((byte) num2)) || (Main.tile[this.x, this.y].liquid != ((byte) num2)))
                                {
                                    Main.tile[this.x + 2, this.y].liquid = (byte) num2;
                                    AddWater(this.x + 2, this.y);
                                }
                                Main.tile[this.x + 2, this.y].lava = Main.tile[this.x, this.y].lava;
                                Main.tile[this.x, this.y].liquid = (byte) num2;
                            }
                            else
                            {
                                num2 = ((Main.tile[this.x - 1, this.y].liquid + Main.tile[this.x + 1, this.y].liquid) + Main.tile[this.x, this.y].liquid) + num3;
                                num2 = (float) Math.Round((double) ((num2 / 3f) + 0.001));
                                if (Main.tile[this.x - 1, this.y].liquid != ((byte) num2))
                                {
                                    Main.tile[this.x - 1, this.y].liquid = (byte) num2;
                                }
                                if ((Main.tile[this.x, this.y].liquid != ((byte) num2)) || (Main.tile[this.x - 1, this.y].liquid != ((byte) num2)))
                                {
                                    AddWater(this.x - 1, this.y);
                                }
                                Main.tile[this.x - 1, this.y].lava = Main.tile[this.x, this.y].lava;
                                if (Main.tile[this.x + 1, this.y].liquid != ((byte) num2))
                                {
                                    Main.tile[this.x + 1, this.y].liquid = (byte) num2;
                                }
                                if ((Main.tile[this.x, this.y].liquid != ((byte) num2)) || (Main.tile[this.x + 1, this.y].liquid != ((byte) num2)))
                                {
                                    AddWater(this.x + 1, this.y);
                                }
                                Main.tile[this.x + 1, this.y].lava = Main.tile[this.x, this.y].lava;
                                Main.tile[this.x, this.y].liquid = (byte) num2;
                            }
                        }
                        else if (flag)
                        {
                            num2 = (Main.tile[this.x - 1, this.y].liquid + Main.tile[this.x, this.y].liquid) + num3;
                            num2 = (float) Math.Round((double) ((num2 / 2f) + 0.001));
                            if (Main.tile[this.x - 1, this.y].liquid != ((byte) num2))
                            {
                                Main.tile[this.x - 1, this.y].liquid = (byte) num2;
                            }
                            if ((Main.tile[this.x, this.y].liquid != ((byte) num2)) || (Main.tile[this.x - 1, this.y].liquid != ((byte) num2)))
                            {
                                AddWater(this.x - 1, this.y);
                            }
                            Main.tile[this.x - 1, this.y].lava = Main.tile[this.x, this.y].lava;
                            Main.tile[this.x, this.y].liquid = (byte) num2;
                        }
                        else if (flag2)
                        {
                            num2 = (Main.tile[this.x + 1, this.y].liquid + Main.tile[this.x, this.y].liquid) + num3;
                            num2 = (float) Math.Round((double) ((num2 / 2f) + 0.001));
                            if (Main.tile[this.x + 1, this.y].liquid != ((byte) num2))
                            {
                                Main.tile[this.x + 1, this.y].liquid = (byte) num2;
                            }
                            if ((Main.tile[this.x, this.y].liquid != ((byte) num2)) || (Main.tile[this.x + 1, this.y].liquid != ((byte) num2)))
                            {
                                AddWater(this.x + 1, this.y);
                            }
                            Main.tile[this.x + 1, this.y].lava = Main.tile[this.x, this.y].lava;
                            Main.tile[this.x, this.y].liquid = (byte) num2;
                        }
                    }
                    if (Main.tile[this.x, this.y].liquid != liquid)
                    {
                        if ((Main.tile[this.x, this.y].liquid == 0xfe) && (liquid == 0xff))
                        {
                            Main.tile[this.x, this.y].liquid = 0xff;
                            this.kill++;
                        }
                        else
                        {
                            AddWater(this.x, this.y - 1);
                            this.kill = 0;
                        }
                    }
                    else
                    {
                        this.kill++;
                    }
                }
            }
        }

        public static void UpdateLiquid()
        {
            if (Main.netMode == 2)
            {
                cycles = 0x19;
                maxLiquid = 0x1388;
            }
            if (!WorldGen.gen)
            {
                if (!panicMode)
                {
                    if ((Liquid.numLiquid + LiquidBuffer.numLiquidBuffer) > 0xfa0)
                    {
                        panicCounter++;
                        if ((panicCounter > 0x708) || ((Liquid.numLiquid + LiquidBuffer.numLiquidBuffer) > 0x34bc))
                        {
                            WorldGen.waterLine = Main.maxTilesY;
                            Liquid.numLiquid = 0;
                            LiquidBuffer.numLiquidBuffer = 0;
                            panicCounter = 0;
                            panicMode = true;
                            panicY = Main.maxTilesY - 3;
                        }
                    }
                    else
                    {
                        panicCounter = 0;
                    }
                }
                if (panicMode)
                {
                    int num = 0;
                    while ((panicY >= 3) && (num < 5))
                    {
                        num++;
                        QuickWater(0, panicY, panicY);
                        panicY--;
                        if (panicY < 3)
                        {
                            panicCounter = 0;
                            panicMode = false;
                            WorldGen.WaterCheck();
                            if (Main.netMode == 2)
                            {
                                for (int i = 0; i < 8; i++)
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
            if (quickSettle || (Liquid.numLiquid > 0x7d0))
            {
                quickFall = true;
            }
            else
            {
                quickFall = false;
            }
            wetCounter++;
            int num5 = maxLiquid / cycles;
            int num6 = num5 * (wetCounter - 1);
            int numLiquid = num5 * wetCounter;
            if (wetCounter == cycles)
            {
                numLiquid = Liquid.numLiquid;
            }
            if (numLiquid > Liquid.numLiquid)
            {
                numLiquid = Liquid.numLiquid;
                int netMode = Main.netMode;
                wetCounter = cycles;
            }
            if (quickFall)
            {
                for (int m = num6; m < numLiquid; m++)
                {
                    Main.liquid[m].delay = 10;
                    Main.liquid[m].Update();
                    Main.tile[Main.liquid[m].x, Main.liquid[m].y].skipLiquid = false;
                }
            }
            else
            {
                for (int n = num6; n < numLiquid; n++)
                {
                    if (!Main.tile[Main.liquid[n].x, Main.liquid[n].y].skipLiquid)
                    {
                        Main.liquid[n].Update();
                    }
                    else
                    {
                        Main.tile[Main.liquid[n].x, Main.liquid[n].y].skipLiquid = false;
                    }
                }
            }
            if (wetCounter >= cycles)
            {
                wetCounter = 0;
                for (int num10 = Liquid.numLiquid - 1; num10 >= 0; num10--)
                {
                    if (Main.liquid[num10].kill > 3)
                    {
                        DelWater(num10);
                    }
                }
                int numLiquidBuffer = maxLiquid - (maxLiquid - Liquid.numLiquid);
                if (numLiquidBuffer > LiquidBuffer.numLiquidBuffer)
                {
                    numLiquidBuffer = LiquidBuffer.numLiquidBuffer;
                }
                for (int num12 = 0; num12 < numLiquidBuffer; num12++)
                {
                    Main.tile[Main.liquidBuffer[0].x, Main.liquidBuffer[0].y].checkingLiquid = false;
                    AddWater(Main.liquidBuffer[0].x, Main.liquidBuffer[0].y);
                    LiquidBuffer.DelBuffer(0);
                }
                if (((Liquid.numLiquid > 0) && (Liquid.numLiquid > (stuckAmount - 50))) && (Liquid.numLiquid < (stuckAmount + 50)))
                {
                    stuckCount++;
                    if (stuckCount >= 0x2710)
                    {
                        stuck = true;
                        for (int num13 = Liquid.numLiquid - 1; num13 >= 0; num13--)
                        {
                            DelWater(num13);
                        }
                        stuck = false;
                        stuckCount = 0;
                    }
                }
                else
                {
                    stuckCount = 0;
                    stuckAmount = Liquid.numLiquid;
                }
            }
        }
    }
}

