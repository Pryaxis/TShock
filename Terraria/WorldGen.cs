namespace Terraria
{
    using Microsoft.Xna.Framework;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class WorldGen
    {
        public static int bestX = 0;
        public static int bestY = 0;
        public static bool canSpawn;
        private static int[] DDoorPos = new int[300];
        private static int[] DDoorX = new int[300];
        private static int[] DDoorY = new int[300];
        public static int dEnteranceX = 0;
        private static bool destroyObject = false;
        private static int dMaxX;
        private static int dMaxY;
        private static int dMinX;
        private static int dMinY;
        private static int[] DPlatX = new int[300];
        private static int[] DPlatY = new int[300];

        public static bool dSurface = false;
        public static int dungeonX;
        public static int dungeonY;
        private static double dxStrength1;
        private static double dxStrength2;
        private static double dyStrength1;
        private static double dyStrength2;
        private static int[] fihX = new int[300];
        private static int[] fihY = new int[300];
        public static bool gen = false;
        [ThreadStatic]
        public static Random genRand = new Random();
        public static int hiScore = 0;
        private static int houseCount = 0;
        public static bool[] houseTile = new bool[80];
        private static int[] JChestX = new int[100];
        private static int[] JChestY = new int[100];
        public static Vector2 lastDungeonHall = new Vector2();
        private static int lastMaxTilesX = 0;
        private static int lastMaxTilesY = 0;
        public static int lavaLine;
        public static bool loadFailed = false;
        public static int maxDRooms = 100;
        private static int[] dRoomB = new int[maxDRooms];
        private static int[] dRoomL = new int[maxDRooms];
        private static int[] dRoomR = new int[maxDRooms];
        public static int[] dRoomSize = new int[maxDRooms];
        private static int[] dRoomT = new int[maxDRooms];
        private static bool[] dRoomTreasure = new bool[maxDRooms];
        public static int[] dRoomX = new int[maxDRooms];
        public static int[] dRoomY = new int[maxDRooms];
        public static int maxRoomTiles = 0x76c;
        private static int[] mCaveX = new int[300];
        private static int[] mCaveY = new int[300];
        private static bool mergeDown = false;
        private static bool mergeLeft = false;
        private static bool mergeRight = false;
        private static bool mergeUp = false;
        public static bool noLiquidCheck = false;
        private static int numDDoors;
        private static int numDPlats;
        public static int numDRooms = 0;
        private static int numIslandHouses = 0;
        private static int numJChests = 0;
        private static int numMCaves = 0;
        public static int numRoomTiles;
        public static int[] roomX = new int[maxRoomTiles];
        public static int roomX1;
        public static int roomX2;
        public static int[] roomY = new int[maxRoomTiles];
        public static int roomY1;
        public static int roomY2;
        public static bool saveLock = false;
        public static int shadowOrbCount = 0;
        public static bool shadowOrbSmashed = false;
        public static int spawnDelay = 0;
        public static bool spawnEye = false;
        public static bool spawnMeteor = false;
        public static int spawnNPC = 0;
        public static string statusText = "";
        private static bool stopDrops = false;
        private static bool tempBloodMoon = Main.bloodMoon;
        private static bool tempDayTime = Main.dayTime;
        private static int tempMoonPhase = Main.moonPhase;
        private static double tempTime = Main.time;
        public static int waterLine;
        public static bool worldCleared = false;

        public static bool AddBuriedChest(int i, int j, int contain = 0)
        {
            if (genRand == null)
            {
                genRand = new Random((int) DateTime.Now.Ticks);
            }
            for (int k = j; k < Main.maxTilesY; k++)
            {
                if (Main.tile[i, k].active && Main.tileSolid[Main.tile[i, k].type])
                {
                    int num2 = i;
                    int num3 = k;
                    int index = PlaceChest(num2 - 1, num3 - 1, 0x15);
                    if (index < 0)
                    {
                        return false;
                    }
                    int num5 = 0;
                    while (num5 == 0)
                    {
                        if (contain > 0)
                        {
                            Main.chest[index].item[num5].SetDefaults(contain);
                            num5++;
                        }
                        else
                        {
                            switch (genRand.Next(7))
                            {
                                case 0:
                                    Main.chest[index].item[num5].SetDefaults(0x31);
                                    break;

                                case 1:
                                    Main.chest[index].item[num5].SetDefaults(50);
                                    break;

                                case 2:
                                    Main.chest[index].item[num5].SetDefaults(0x34);
                                    break;

                                case 3:
                                    Main.chest[index].item[num5].SetDefaults(0x35);
                                    break;

                                case 4:
                                    Main.chest[index].item[num5].SetDefaults(0x36);
                                    break;

                                case 5:
                                    Main.chest[index].item[num5].SetDefaults(0x37);
                                    break;

                                case 6:
                                    Main.chest[index].item[num5].SetDefaults(0x33);
                                    Main.chest[index].item[num5].stack = genRand.Next(0x1a) + 0x19;
                                    break;
                            }
                            num5++;
                        }
                        if (genRand.Next(3) == 0)
                        {
                            Main.chest[index].item[num5].SetDefaults(0xa7);
                            num5++;
                        }
                        if (genRand.Next(2) == 0)
                        {
                            int num7 = genRand.Next(4);
                            int num8 = genRand.Next(8) + 3;
                            switch (num7)
                            {
                                case 0:
                                    Main.chest[index].item[num5].SetDefaults(0x13);
                                    break;

                                case 1:
                                    Main.chest[index].item[num5].SetDefaults(20);
                                    break;

                                case 2:
                                    Main.chest[index].item[num5].SetDefaults(0x15);
                                    break;

                                case 3:
                                    Main.chest[index].item[num5].SetDefaults(0x16);
                                    break;
                            }
                            Main.chest[index].item[num5].stack = num8;
                            num5++;
                        }
                        if (genRand.Next(2) == 0)
                        {
                            int num9 = genRand.Next(2);
                            int num10 = genRand.Next(0x1a) + 0x19;
                            switch (num9)
                            {
                                case 0:
                                    Main.chest[index].item[num5].SetDefaults(40);
                                    break;

                                case 1:
                                    Main.chest[index].item[num5].SetDefaults(0x2a);
                                    break;
                            }
                            Main.chest[index].item[num5].stack = num10;
                            num5++;
                        }
                        if (genRand.Next(2) == 0)
                        {
                            int num11 = genRand.Next(1);
                            int num12 = genRand.Next(3) + 3;
                            if (num11 == 0)
                            {
                                Main.chest[index].item[num5].SetDefaults(0x1c);
                            }
                            Main.chest[index].item[num5].stack = num12;
                            num5++;
                        }
                        if (genRand.Next(2) == 0)
                        {
                            int num13 = genRand.Next(2);
                            int num14 = genRand.Next(11) + 10;
                            switch (num13)
                            {
                                case 0:
                                    Main.chest[index].item[num5].SetDefaults(8);
                                    break;

                                case 1:
                                    Main.chest[index].item[num5].SetDefaults(0x1f);
                                    break;
                            }
                            Main.chest[index].item[num5].stack = num14;
                            num5++;
                        }
                        if (genRand.Next(2) == 0)
                        {
                            Main.chest[index].item[num5].SetDefaults(0x49);
                            Main.chest[index].item[num5].stack = genRand.Next(1, 3);
                            num5++;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public static void AddHellHouses()
        {
            int num = (int) (Main.maxTilesX * 0.25);
            for (int i = num; i < (Main.maxTilesX - num); i++)
            {
                int j = Main.maxTilesY - 40;
                while (Main.tile[i, j].active || (Main.tile[i, j].liquid > 0))
                {
                    j--;
                }
                if (Main.tile[i, j + 1].active)
                {
                    HellHouse(i, j);
                    i += genRand.Next(15, 80);
                }
            }
        }

        public static bool AddLifeCrystal(int i, int j)
        {
            for (int k = j; k < Main.maxTilesY; k++)
            {
                if (Main.tile[i, k].active && Main.tileSolid[Main.tile[i, k].type])
                {
                    int endX = i;
                    int endY = k - 1;
                    if (Main.tile[endX, endY - 1].lava || Main.tile[endX - 1, endY - 1].lava)
                    {
                        return false;
                    }
                    if (!EmptyTileCheck(endX - 1, endX, endY - 1, endY, -1))
                    {
                        return false;
                    }
                    Main.tile[endX - 1, endY - 1].active = true;
                    Main.tile[endX - 1, endY - 1].type = 12;
                    Main.tile[endX - 1, endY - 1].frameX = 0;
                    Main.tile[endX - 1, endY - 1].frameY = 0;
                    Main.tile[endX, endY - 1].active = true;
                    Main.tile[endX, endY - 1].type = 12;
                    Main.tile[endX, endY - 1].frameX = 0x12;
                    Main.tile[endX, endY - 1].frameY = 0;
                    Main.tile[endX - 1, endY].active = true;
                    Main.tile[endX - 1, endY].type = 12;
                    Main.tile[endX - 1, endY].frameX = 0;
                    Main.tile[endX - 1, endY].frameY = 0x12;
                    Main.tile[endX, endY].active = true;
                    Main.tile[endX, endY].type = 12;
                    Main.tile[endX, endY].frameX = 0x12;
                    Main.tile[endX, endY].frameY = 0x12;
                    return true;
                }
            }
            return false;
        }

        public static void AddPlants()
        {
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                for (int j = 1; j < Main.maxTilesY; j++)
                {
                    if ((Main.tile[i, j].type == 2) && Main.tile[i, j].active)
                    {
                        if (!Main.tile[i, j - 1].active)
                        {
                            PlaceTile(i, j - 1, 3, true, false, -1);
                        }
                    }
                    else if (((Main.tile[i, j].type == 0x17) && Main.tile[i, j].active) && !Main.tile[i, j - 1].active)
                    {
                        PlaceTile(i, j - 1, 0x18, true, false, -1);
                    }
                }
            }
        }

        public static void AddShadowOrb(int x, int y)
        {
            if (((x >= 10) && (x <= (Main.maxTilesX - 10))) && ((y >= 10) && (y <= (Main.maxTilesY - 10))))
            {
                Main.tile[x - 1, y - 1].active = true;
                Main.tile[x - 1, y - 1].type = 0x1f;
                Main.tile[x - 1, y - 1].frameX = 0;
                Main.tile[x - 1, y - 1].frameY = 0;
                Main.tile[x, y - 1].active = true;
                Main.tile[x, y - 1].type = 0x1f;
                Main.tile[x, y - 1].frameX = 0x12;
                Main.tile[x, y - 1].frameY = 0;
                Main.tile[x - 1, y].active = true;
                Main.tile[x - 1, y].type = 0x1f;
                Main.tile[x - 1, y].frameX = 0;
                Main.tile[x - 1, y].frameY = 0x12;
                Main.tile[x, y].active = true;
                Main.tile[x, y].type = 0x1f;
                Main.tile[x, y].frameX = 0x12;
                Main.tile[x, y].frameY = 0x12;
            }
        }

        public static void AddTrees()
        {
            for (int i = 1; i < (Main.maxTilesX - 1); i++)
            {
                for (int j = 20; j < Main.worldSurface; j++)
                {
                    GrowTree(i, j);
                }
            }
        }

        public static void CaveOpenater(int i, int j)
        {
            Vector2 vector;
            Vector2 vector2;
            double num5 = genRand.Next(7, 12);
            double num6 = num5;
            int num7 = 1;
            if (genRand.Next(2) == 0)
            {
                num7 = -1;
            }
            vector.X = i;
            vector.Y = j;
            int num8 = 100;
            vector2.Y = 0f;
            vector2.X = num7;
            while (num8 > 0)
            {
                if (Main.tile[(int) vector.X, (int) vector.Y].wall == 0)
                {
                    num8 = 0;
                }
                num8--;
                int num = (int) (vector.X - (num5 * 0.5));
                int maxTilesX = (int) (vector.X + (num5 * 0.5));
                int num2 = (int) (vector.Y - (num5 * 0.5));
                int maxTilesY = (int) (vector.Y + (num5 * 0.5));
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > Main.maxTilesX)
                {
                    maxTilesX = Main.maxTilesX;
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > Main.maxTilesY)
                {
                    maxTilesY = Main.maxTilesY;
                }
                num6 = (num5 * genRand.Next(80, 120)) * 0.01;
                for (int k = num; k < maxTilesX; k++)
                {
                    for (int m = num2; m < maxTilesY; m++)
                    {
                        float num11 = Math.Abs((float) (k - vector.X));
                        float num12 = Math.Abs((float) (m - vector.Y));
                        if (Math.Sqrt((double) ((num11 * num11) + (num12 * num12))) < (num6 * 0.4))
                        {
                            Main.tile[k, m].active = false;
                        }
                    }
                }
                vector += vector2;
                vector2.X += genRand.Next(-10, 11) * 0.05f;
                vector2.Y += genRand.Next(-10, 11) * 0.05f;
                if (vector2.X > (num7 + 0.5f))
                {
                    vector2.X = num7 + 0.5f;
                }
                if (vector2.X < (num7 - 0.5f))
                {
                    vector2.X = num7 - 0.5f;
                }
                if (vector2.Y > 0f)
                {
                    vector2.Y = 0f;
                }
                if (vector2.Y < -0.5)
                {
                    vector2.Y = -0.5f;
                }
            }
        }

        public static void Cavinator(int i, int j, int steps)
        {
            Vector2 vector;
            Vector2 vector2;
            double num5 = genRand.Next(7, 15);
            double num6 = num5;
            int num7 = 1;
            if (genRand.Next(2) == 0)
            {
                num7 = -1;
            }
            vector.X = i;
            vector.Y = j;
            int num8 = genRand.Next(20, 40);
            vector2.Y = genRand.Next(10, 20) * 0.01f;
            vector2.X = num7;
            while (num8 > 0)
            {
                num8--;
                int num = (int) (vector.X - (num5 * 0.5));
                int maxTilesX = (int) (vector.X + (num5 * 0.5));
                int num2 = (int) (vector.Y - (num5 * 0.5));
                int maxTilesY = (int) (vector.Y + (num5 * 0.5));
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > Main.maxTilesX)
                {
                    maxTilesX = Main.maxTilesX;
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > Main.maxTilesY)
                {
                    maxTilesY = Main.maxTilesY;
                }
                num6 = (num5 * genRand.Next(80, 120)) * 0.01;
                for (int k = num; k < maxTilesX; k++)
                {
                    for (int m = num2; m < maxTilesY; m++)
                    {
                        float num11 = Math.Abs((float) (k - vector.X));
                        float num12 = Math.Abs((float) (m - vector.Y));
                        if (Math.Sqrt((double) ((num11 * num11) + (num12 * num12))) < (num6 * 0.4))
                        {
                            Main.tile[k, m].active = false;
                        }
                    }
                }
                vector += vector2;
                vector2.X += genRand.Next(-10, 11) * 0.05f;
                vector2.Y += genRand.Next(-10, 11) * 0.05f;
                if (vector2.X > (num7 + 0.5f))
                {
                    vector2.X = num7 + 0.5f;
                }
                if (vector2.X < (num7 - 0.5f))
                {
                    vector2.X = num7 - 0.5f;
                }
                if (vector2.Y > 2f)
                {
                    vector2.Y = 2f;
                }
                if (vector2.Y < 0f)
                {
                    vector2.Y = 0f;
                }
            }
            if ((steps > 0) && (((int) vector.Y) < (Main.rockLayer + 50.0)))
            {
                Cavinator((int) vector.X, (int) vector.Y, steps - 1);
            }
        }

        public static void ChasmRunner(int i, int j, int steps, bool makeOrb = false)
        {
            Vector2 vector;
            Vector2 vector2;
            bool flag = false;
            bool flag2 = false;
            if (!makeOrb)
            {
                flag = true;
            }
            float num5 = steps;
            vector.X = i;
            vector.Y = j;
            vector2.X = genRand.Next(-10, 11) * 0.1f;
            vector2.Y = (genRand.Next(11) * 0.2f) + 0.5f;
            int num6 = 5;
            double num7 = genRand.Next(5) + 7;
            while (num7 > 0.0)
            {
                int num;
                int num2;
                int num3;
                int maxTilesY;
                if (num5 > 0f)
                {
                    num7 += genRand.Next(3);
                    num7 -= genRand.Next(3);
                    if (num7 < 7.0)
                    {
                        num7 = 7.0;
                    }
                    if (num7 > 20.0)
                    {
                        num7 = 20.0;
                    }
                    if ((num5 == 1f) && (num7 < 10.0))
                    {
                        num7 = 10.0;
                    }
                }
                else
                {
                    num7 -= genRand.Next(4);
                }
                if ((vector.Y > Main.rockLayer) && (num5 > 0f))
                {
                    num5 = 0f;
                }
                num5--;
                if (num5 > num6)
                {
                    num = (int) (vector.X - (num7 * 0.5));
                    num3 = (int) (vector.X + (num7 * 0.5));
                    num2 = (int) (vector.Y - (num7 * 0.5));
                    maxTilesY = (int) (vector.Y + (num7 * 0.5));
                    if (num < 0)
                    {
                        num = 0;
                    }
                    if (num3 > (Main.maxTilesX - 1))
                    {
                        num3 = Main.maxTilesX - 1;
                    }
                    if (num2 < 0)
                    {
                        num2 = 0;
                    }
                    if (maxTilesY > Main.maxTilesY)
                    {
                        maxTilesY = Main.maxTilesY;
                    }
                    for (int n = num; n < num3; n++)
                    {
                        for (int num9 = num2; num9 < maxTilesY; num9++)
                        {
                            if ((Math.Abs((float) (n - vector.X)) + Math.Abs((float) (num9 - vector.Y))) < ((num7 * 0.5) * (1.0 + (genRand.Next(-10, 11) * 0.015))))
                            {
                                Main.tile[n, num9].active = false;
                            }
                        }
                    }
                }
                if (num5 <= 0f)
                {
                    if (!flag)
                    {
                        flag = true;
                        AddShadowOrb((int) vector.X, (int) vector.Y);
                    }
                    else if (!flag2)
                    {
                        flag2 = false;
                        bool flag3 = false;
                        int num10 = 0;
                        while (!flag3)
                        {
                            int x = genRand.Next(((int) vector.X) - 0x19, ((int) vector.X) + 0x19);
                            int y = genRand.Next(((int) vector.Y) - 50, (int) vector.Y);
                            if (x < 5)
                            {
                                x = 5;
                            }
                            if (x > (Main.maxTilesX - 5))
                            {
                                x = Main.maxTilesX - 5;
                            }
                            if (y < 5)
                            {
                                y = 5;
                            }
                            if (y > (Main.maxTilesY - 5))
                            {
                                y = Main.maxTilesY - 5;
                            }
                            if (y > Main.worldSurface)
                            {
                                Place3x2(x, y, 0x1a);
                                if (Main.tile[x, y].type == 0x1a)
                                {
                                    flag3 = true;
                                }
                                else
                                {
                                    num10++;
                                    if (num10 >= 0x2710)
                                    {
                                        flag3 = true;
                                    }
                                }
                            }
                            else
                            {
                                flag3 = true;
                            }
                        }
                    }
                }
                vector += vector2;
                vector2.X += genRand.Next(-10, 11) * 0.01f;
                if (vector2.X > 0.3)
                {
                    vector2.X = 0.3f;
                }
                if (vector2.X < -0.3)
                {
                    vector2.X = -0.3f;
                }
                num = (int) (vector.X - (num7 * 1.1));
                num3 = (int) (vector.X + (num7 * 1.1));
                num2 = (int) (vector.Y - (num7 * 1.1));
                maxTilesY = (int) (vector.Y + (num7 * 1.1));
                if (num < 1)
                {
                    num = 1;
                }
                if (num3 > (Main.maxTilesX - 1))
                {
                    num3 = Main.maxTilesX - 1;
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > Main.maxTilesY)
                {
                    maxTilesY = Main.maxTilesY;
                }
                for (int k = num; k < num3; k++)
                {
                    for (int num14 = num2; num14 < maxTilesY; num14++)
                    {
                        if ((Math.Abs((float) (k - vector.X)) + Math.Abs((float) (num14 - vector.Y))) < ((num7 * 1.1) * (1.0 + (genRand.Next(-10, 11) * 0.015))))
                        {
                            if ((Main.tile[k, num14].type != 0x19) && (num14 > (j + genRand.Next(3, 20))))
                            {
                                Main.tile[k, num14].active = true;
                            }
                            if (steps <= num6)
                            {
                                Main.tile[k, num14].active = true;
                            }
                            if (Main.tile[k, num14].type != 0x1f)
                            {
                                Main.tile[k, num14].type = 0x19;
                            }
                            if (Main.tile[k, num14].wall == 2)
                            {
                                Main.tile[k, num14].wall = 0;
                            }
                        }
                    }
                }
                for (int m = num; m < num3; m++)
                {
                    for (int num16 = num2; num16 < maxTilesY; num16++)
                    {
                        if ((Math.Abs((float) (m - vector.X)) + Math.Abs((float) (num16 - vector.Y))) < ((num7 * 1.1) * (1.0 + (genRand.Next(-10, 11) * 0.015))))
                        {
                            if (Main.tile[m, num16].type != 0x1f)
                            {
                                Main.tile[m, num16].type = 0x19;
                            }
                            if (steps <= num6)
                            {
                                Main.tile[m, num16].active = true;
                            }
                            if (num16 > (j + genRand.Next(3, 20)))
                            {
                                PlaceWall(m, num16, 3, true);
                            }
                        }
                    }
                }
            }
        }

        public static void Check1x2(int x, int j, byte type)
        {
            if (!destroyObject)
            {
                int num = j;
                bool flag = true;
                if (Main.tile[x, num] == null)
                {
                    Main.tile[x, num] = new Tile();
                }
                if (Main.tile[x, num + 1] == null)
                {
                    Main.tile[x, num + 1] = new Tile();
                }
                if (Main.tile[x, num].frameY == 0x12)
                {
                    num--;
                }
                if (Main.tile[x, num] == null)
                {
                    Main.tile[x, num] = new Tile();
                }
                if (((Main.tile[x, num].frameY == 0) && (Main.tile[x, num + 1].frameY == 0x12)) && ((Main.tile[x, num].type == type) && (Main.tile[x, num + 1].type == type)))
                {
                    flag = false;
                }
                if (Main.tile[x, num + 2] == null)
                {
                    Main.tile[x, num + 2] = new Tile();
                }
                if (!Main.tile[x, num + 2].active || !Main.tileSolid[Main.tile[x, num + 2].type])
                {
                    flag = true;
                }
                if ((Main.tile[x, num + 2].type != 2) && (Main.tile[x, num].type == 20))
                {
                    flag = true;
                }
                if (flag)
                {
                    destroyObject = true;
                    if (Main.tile[x, num].type == type)
                    {
                        KillTile(x, num, false, false, false);
                    }
                    if (Main.tile[x, num + 1].type == type)
                    {
                        KillTile(x, num + 1, false, false, false);
                    }
                    if (type == 15)
                    {
                        Item.NewItem(x * 0x10, num * 0x10, 0x20, 0x20, 0x22, 1, false);
                    }
                    destroyObject = false;
                }
            }
        }

        public static void Check1x2Top(int x, int j, byte type)
        {
            if (!destroyObject)
            {
                int num = j;
                bool flag = true;
                if (Main.tile[x, num] == null)
                {
                    Main.tile[x, num] = new Tile();
                }
                if (Main.tile[x, num + 1] == null)
                {
                    Main.tile[x, num + 1] = new Tile();
                }
                if (Main.tile[x, num].frameY == 0x12)
                {
                    num--;
                }
                if (Main.tile[x, num] == null)
                {
                    Main.tile[x, num] = new Tile();
                }
                if (((Main.tile[x, num].frameY == 0) && (Main.tile[x, num + 1].frameY == 0x12)) && ((Main.tile[x, num].type == type) && (Main.tile[x, num + 1].type == type)))
                {
                    flag = false;
                }
                if (Main.tile[x, num - 1] == null)
                {
                    Main.tile[x, num - 1] = new Tile();
                }
                if ((!Main.tile[x, num - 1].active || !Main.tileSolid[Main.tile[x, num - 1].type]) || Main.tileSolidTop[Main.tile[x, num - 1].type])
                {
                    flag = true;
                }
                if (flag)
                {
                    destroyObject = true;
                    if (Main.tile[x, num].type == type)
                    {
                        KillTile(x, num, false, false, false);
                    }
                    if (Main.tile[x, num + 1].type == type)
                    {
                        KillTile(x, num + 1, false, false, false);
                    }
                    if (type == 0x2a)
                    {
                        Item.NewItem(x * 0x10, num * 0x10, 0x20, 0x20, 0x88, 1, false);
                    }
                    destroyObject = false;
                }
            }
        }

        public static void Check2x1(int i, int y, byte type)
        {
            if (!destroyObject)
            {
                int num = i;
                bool flag = true;
                if (Main.tile[num, y] == null)
                {
                    Main.tile[num, y] = new Tile();
                }
                if (Main.tile[num + 1, y] == null)
                {
                    Main.tile[num + 1, y] = new Tile();
                }
                if (Main.tile[num, y + 1] == null)
                {
                    Main.tile[num, y + 1] = new Tile();
                }
                if (Main.tile[num + 1, y + 1] == null)
                {
                    Main.tile[num + 1, y + 1] = new Tile();
                }
                if (Main.tile[num, y].frameX == 0x12)
                {
                    num--;
                }
                if (Main.tile[num, y] == null)
                {
                    Main.tile[num, y] = new Tile();
                }
                if (((Main.tile[num, y].frameX == 0) && (Main.tile[num + 1, y].frameX == 0x12)) && ((Main.tile[num, y].type == type) && (Main.tile[num + 1, y].type == type)))
                {
                    flag = false;
                }
                if (type == 0x1d)
                {
                    if (!Main.tile[num, y + 1].active || !Main.tileTable[Main.tile[num, y + 1].type])
                    {
                        flag = true;
                    }
                    if (!Main.tile[num + 1, y + 1].active || !Main.tileTable[Main.tile[num + 1, y + 1].type])
                    {
                        flag = true;
                    }
                }
                else
                {
                    if (!Main.tile[num, y + 1].active || !Main.tileSolid[Main.tile[num, y + 1].type])
                    {
                        flag = true;
                    }
                    if (!Main.tile[num + 1, y + 1].active || !Main.tileSolid[Main.tile[num + 1, y + 1].type])
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    destroyObject = true;
                    if (Main.tile[num, y].type == type)
                    {
                        KillTile(num, y, false, false, false);
                    }
                    if (Main.tile[num + 1, y].type == type)
                    {
                        KillTile(num + 1, y, false, false, false);
                    }
                    if (type == 0x10)
                    {
                        Item.NewItem(num * 0x10, y * 0x10, 0x20, 0x20, 0x23, 1, false);
                    }
                    if (type == 0x12)
                    {
                        Item.NewItem(num * 0x10, y * 0x10, 0x20, 0x20, 0x24, 1, false);
                    }
                    if (type == 0x1d)
                    {
                        Item.NewItem(num * 0x10, y * 0x10, 0x20, 0x20, 0x57, 1, false);
                        Main.PlaySound(13, i * 0x10, y * 0x10, 1);
                    }
                    destroyObject = false;
                }
            }
        }

        public static void Check3x2(int i, int j, int type)
        {
            if (!destroyObject)
            {
                bool flag = false;
                int num = i;
                int num2 = j;
                num += (Main.tile[i, j].frameX / 0x12) * -1;
                num2 += (Main.tile[i, j].frameY / 0x12) * -1;
                for (int k = num; k < (num + 3); k++)
                {
                    for (int m = num2; m < (num2 + 2); m++)
                    {
                        if (Main.tile[k, m] == null)
                        {
                            Main.tile[k, m] = new Tile();
                        }
                        if ((!Main.tile[k, m].active || (Main.tile[k, m].type != type)) || ((Main.tile[k, m].frameX != ((k - num) * 0x12)) || (Main.tile[k, m].frameY != ((m - num2) * 0x12))))
                        {
                            flag = true;
                        }
                    }
                    if (Main.tile[k, num2 + 2] == null)
                    {
                        Main.tile[k, num2 + 2] = new Tile();
                    }
                    if (!Main.tile[k, num2 + 2].active || !Main.tileSolid[Main.tile[k, num2 + 2].type])
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    destroyObject = true;
                    for (int n = num; n < (num + 3); n++)
                    {
                        for (int num6 = num2; num6 < (num2 + 3); num6++)
                        {
                            if ((Main.tile[n, num6].type == type) && Main.tile[n, num6].active)
                            {
                                KillTile(n, num6, false, false, false);
                            }
                        }
                    }
                    if (type == 14)
                    {
                        Item.NewItem(i * 0x10, j * 0x10, 0x20, 0x20, 0x20, 1, false);
                    }
                    else if (type == 0x11)
                    {
                        Item.NewItem(i * 0x10, j * 0x10, 0x20, 0x20, 0x21, 1, false);
                    }
                    else if (type == 0x4d)
                    {
                        Item.NewItem(i * 0x10, j * 0x10, 0x20, 0x20, 0xdd, 1, false);
                    }
                    destroyObject = false;
                    for (int num7 = num - 1; num7 < (num + 4); num7++)
                    {
                        for (int num8 = num2 - 1; num8 < (num2 + 4); num8++)
                        {
                            TileFrame(num7, num8, false, false);
                        }
                    }
                }
            }
        }

        public static void Check3x3(int i, int j, int type)
        {
            if (!destroyObject)
            {
                bool flag = false;
                int num = i;
                int num2 = j;
                num += (Main.tile[i, j].frameX / 0x12) * -1;
                num2 += (Main.tile[i, j].frameY / 0x12) * -1;
                for (int k = num; k < (num + 3); k++)
                {
                    for (int m = num2; m < (num2 + 3); m++)
                    {
                        if (Main.tile[k, m] == null)
                        {
                            Main.tile[k, m] = new Tile();
                        }
                        if ((!Main.tile[k, m].active || (Main.tile[k, m].type != type)) || ((Main.tile[k, m].frameX != ((k - num) * 0x12)) || (Main.tile[k, m].frameY != ((m - num2) * 0x12))))
                        {
                            flag = true;
                        }
                    }
                }
                if (Main.tile[num + 1, num2 - 1] == null)
                {
                    Main.tile[num + 1, num2 - 1] = new Tile();
                }
                if ((!Main.tile[num + 1, num2 - 1].active || !Main.tileSolid[Main.tile[num + 1, num2 - 1].type]) || Main.tileSolidTop[Main.tile[num + 1, num2 - 1].type])
                {
                    flag = true;
                }
                if (flag)
                {
                    destroyObject = true;
                    for (int n = num; n < (num + 3); n++)
                    {
                        for (int num6 = num2; num6 < (num2 + 3); num6++)
                        {
                            if ((Main.tile[n, num6].type == type) && Main.tile[n, num6].active)
                            {
                                KillTile(n, num6, false, false, false);
                            }
                        }
                    }
                    if (type == 0x22)
                    {
                        Item.NewItem(i * 0x10, j * 0x10, 0x20, 0x20, 0x6a, 1, false);
                    }
                    else if (type == 0x23)
                    {
                        Item.NewItem(i * 0x10, j * 0x10, 0x20, 0x20, 0x6b, 1, false);
                    }
                    else if (type == 0x24)
                    {
                        Item.NewItem(i * 0x10, j * 0x10, 0x20, 0x20, 0x6c, 1, false);
                    }
                    destroyObject = false;
                    for (int num7 = num - 1; num7 < (num + 4); num7++)
                    {
                        for (int num8 = num2 - 1; num8 < (num2 + 4); num8++)
                        {
                            TileFrame(num7, num8, false, false);
                        }
                    }
                }
            }
        }

        public static void Check4x2(int i, int j, int type)
        {
            if (!destroyObject)
            {
                bool flag = false;
                int num = i;
                int num2 = j;
                num += (Main.tile[i, j].frameX / 0x12) * -1;
                if ((type == 0x4f) && (Main.tile[i, j].frameX >= 0x48))
                {
                    num += 4;
                }
                num2 += (Main.tile[i, j].frameY / 0x12) * -1;
                for (int k = num; k < (num + 4); k++)
                {
                    for (int m = num2; m < (num2 + 2); m++)
                    {
                        int num5 = (k - num) * 0x12;
                        if ((type == 0x4f) && (Main.tile[i, j].frameX >= 0x48))
                        {
                            num5 = ((k - num) + 4) * 0x12;
                        }
                        if (Main.tile[k, m] == null)
                        {
                            Main.tile[k, m] = new Tile();
                        }
                        if ((!Main.tile[k, m].active || (Main.tile[k, m].type != type)) || ((Main.tile[k, m].frameX != num5) || (Main.tile[k, m].frameY != ((m - num2) * 0x12))))
                        {
                            flag = true;
                        }
                    }
                    if (Main.tile[k, num2 + 2] == null)
                    {
                        Main.tile[k, num2 + 2] = new Tile();
                    }
                    if (!Main.tile[k, num2 + 2].active || !Main.tileSolid[Main.tile[k, num2 + 2].type])
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    destroyObject = true;
                    for (int n = num; n < (num + 4); n++)
                    {
                        for (int num7 = num2; num7 < (num2 + 3); num7++)
                        {
                            if ((Main.tile[n, num7].type == type) && Main.tile[n, num7].active)
                            {
                                KillTile(n, num7, false, false, false);
                            }
                        }
                    }
                    if (type == 0x4f)
                    {
                        Item.NewItem(i * 0x10, j * 0x10, 0x20, 0x20, 0xe0, 1, false);
                    }
                    destroyObject = false;
                    for (int num8 = num - 1; num8 < (num + 4); num8++)
                    {
                        for (int num9 = num2 - 1; num9 < (num2 + 4); num9++)
                        {
                            TileFrame(num8, num9, false, false);
                        }
                    }
                }
            }
        }

        public static void CheckChest(int i, int j, int type)
        {
            if (!destroyObject)
            {
                bool flag = false;
                int num = i;
                int num2 = j;
                num += (Main.tile[i, j].frameX / 0x12) * -1;
                num2 += (Main.tile[i, j].frameY / 0x12) * -1;
                for (int k = num; k < (num + 2); k++)
                {
                    for (int m = num2; m < (num2 + 2); m++)
                    {
                        if (Main.tile[k, m] == null)
                        {
                            Main.tile[k, m] = new Tile();
                        }
                        if ((!Main.tile[k, m].active || (Main.tile[k, m].type != type)) || ((Main.tile[k, m].frameX != ((k - num) * 0x12)) || (Main.tile[k, m].frameY != ((m - num2) * 0x12))))
                        {
                            flag = true;
                        }
                    }
                    if (Main.tile[k, num2 + 2] == null)
                    {
                        Main.tile[k, num2 + 2] = new Tile();
                    }
                    if (!Main.tile[k, num2 + 2].active || !Main.tileSolid[Main.tile[k, num2 + 2].type])
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    destroyObject = true;
                    for (int n = num; n < (num + 2); n++)
                    {
                        for (int num6 = num2; num6 < (num2 + 3); num6++)
                        {
                            if ((Main.tile[n, num6].type == type) && Main.tile[n, num6].active)
                            {
                                KillTile(n, num6, false, false, false);
                            }
                        }
                    }
                    Item.NewItem(i * 0x10, j * 0x10, 0x20, 0x20, 0x30, 1, false);
                    destroyObject = false;
                }
            }
        }

        public static void CheckOnTable1x1(int x, int y, int type)
        {
            if ((Main.tile[x, y + 1] != null) && (!Main.tile[x, y + 1].active || !Main.tileTable[Main.tile[x, y + 1].type]))
            {
                if (type == 0x4e)
                {
                    if (!Main.tile[x, y + 1].active || !Main.tileSolid[Main.tile[x, y + 1].type])
                    {
                        KillTile(x, y, false, false, false);
                    }
                }
                else
                {
                    KillTile(x, y, false, false, false);
                }
            }
        }

        public static void CheckPot(int i, int j, int type = 0x1c)
        {
            if (!destroyObject)
            {
                bool flag = false;
                int num = 0;
                int num2 = j;
                num += Main.tile[i, j].frameX / 0x12;
                num2 += (Main.tile[i, j].frameY / 0x12) * -1;
                while (num > 1)
                {
                    num -= 2;
                }
                num *= -1;
                num += i;
                for (int k = num; k < (num + 2); k++)
                {
                    for (int m = num2; m < (num2 + 2); m++)
                    {
                        if (Main.tile[k, m] == null)
                        {
                            Main.tile[k, m] = new Tile();
                        }
                        int num5 = Main.tile[k, m].frameX / 0x12;
                        while (num5 > 1)
                        {
                            num5 -= 2;
                        }
                        if ((!Main.tile[k, m].active || (Main.tile[k, m].type != type)) || ((num5 != (k - num)) || (Main.tile[k, m].frameY != ((m - num2) * 0x12))))
                        {
                            flag = true;
                        }
                    }
                    if (Main.tile[k, num2 + 2] == null)
                    {
                        Main.tile[k, num2 + 2] = new Tile();
                    }
                    if (!Main.tile[k, num2 + 2].active || !Main.tileSolid[Main.tile[k, num2 + 2].type])
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    destroyObject = true;
                    Main.PlaySound(13, i * 0x10, j * 0x10, 1);
                    for (int n = num; n < (num + 2); n++)
                    {
                        for (int num7 = num2; num7 < (num2 + 2); num7++)
                        {
                            if ((Main.tile[n, num7].type == type) && Main.tile[n, num7].active)
                            {
                                KillTile(n, num7, false, false, false);
                            }
                        }
                    }
                    Gore.NewGore(new Vector2((float) (i * 0x10), (float) (j * 0x10)), new Vector2(), 0x33);
                    Gore.NewGore(new Vector2((float) (i * 0x10), (float) (j * 0x10)), new Vector2(), 0x34);
                    Gore.NewGore(new Vector2((float) (i * 0x10), (float) (j * 0x10)), new Vector2(), 0x35);
                    int num8 = Main.rand.Next(10);
                    if ((num8 == 0) && (Main.player[Player.FindClosest(new Vector2((float) (i * 0x10), (float) (j * 0x10)), 0x10, 0x10)].statLife < Main.player[Player.FindClosest(new Vector2((float) (i * 0x10), (float) (j * 0x10)), 0x10, 0x10)].statLifeMax))
                    {
                        Item.NewItem(i * 0x10, j * 0x10, 0x10, 0x10, 0x3a, 1, false);
                    }
                    else if ((num8 == 1) && (Main.player[Player.FindClosest(new Vector2((float) (i * 0x10), (float) (j * 0x10)), 0x10, 0x10)].statMana < Main.player[Player.FindClosest(new Vector2((float) (i * 0x10), (float) (j * 0x10)), 0x10, 0x10)].statManaMax))
                    {
                        Item.NewItem(i * 0x10, j * 0x10, 0x10, 0x10, 0xb8, 1, false);
                    }
                    else if (num8 == 2)
                    {
                        int stack = Main.rand.Next(3) + 1;
                        Item.NewItem(i * 0x10, j * 0x10, 0x10, 0x10, 8, stack, false);
                    }
                    else if (num8 == 3)
                    {
                        int num10 = Main.rand.Next(8) + 3;
                        Item.NewItem(i * 0x10, j * 0x10, 0x10, 0x10, 40, num10, false);
                    }
                    else if (num8 == 4)
                    {
                        Item.NewItem(i * 0x10, j * 0x10, 0x10, 0x10, 0x1c, 1, false);
                    }
                    else if (num8 == 5)
                    {
                        int num11 = Main.rand.Next(4) + 1;
                        Item.NewItem(i * 0x10, j * 0x10, 0x10, 0x10, 0xa6, num11, false);
                    }
                    else
                    {
                        float num12 = 200 + genRand.Next(-100, 0x65);
                        num12 *= 1f + (Main.rand.Next(-20, 0x15) * 0.01f);
                        if (Main.rand.Next(5) == 0)
                        {
                            num12 *= 1f + (Main.rand.Next(5, 11) * 0.01f);
                        }
                        if (Main.rand.Next(10) == 0)
                        {
                            num12 *= 1f + (Main.rand.Next(10, 0x15) * 0.01f);
                        }
                        if (Main.rand.Next(15) == 0)
                        {
                            num12 *= 1f + (Main.rand.Next(20, 0x29) * 0.01f);
                        }
                        if (Main.rand.Next(20) == 0)
                        {
                            num12 *= 1f + (Main.rand.Next(40, 0x51) * 0.01f);
                        }
                        if (Main.rand.Next(0x19) == 0)
                        {
                            num12 *= 1f + (Main.rand.Next(50, 0x65) * 0.01f);
                        }
                        while (((int) num12) > 0)
                        {
                            if (num12 > 1000000f)
                            {
                                int num13 = (int) (num12 / 1000000f);
                                if ((num13 > 50) && (Main.rand.Next(2) == 0))
                                {
                                    num13 /= Main.rand.Next(3) + 1;
                                }
                                if (Main.rand.Next(2) == 0)
                                {
                                    num13 /= Main.rand.Next(3) + 1;
                                }
                                num12 -= 0xf4240 * num13;
                                Item.NewItem(i * 0x10, j * 0x10, 0x10, 0x10, 0x4a, num13, false);
                            }
                            else
                            {
                                if (num12 > 10000f)
                                {
                                    int num14 = (int) (num12 / 10000f);
                                    if ((num14 > 50) && (Main.rand.Next(2) == 0))
                                    {
                                        num14 /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.Next(2) == 0)
                                    {
                                        num14 /= Main.rand.Next(3) + 1;
                                    }
                                    num12 -= 0x2710 * num14;
                                    Item.NewItem(i * 0x10, j * 0x10, 0x10, 0x10, 0x49, num14, false);
                                    continue;
                                }
                                if (num12 > 100f)
                                {
                                    int num15 = (int) (num12 / 100f);
                                    if ((num15 > 50) && (Main.rand.Next(2) == 0))
                                    {
                                        num15 /= Main.rand.Next(3) + 1;
                                    }
                                    if (Main.rand.Next(2) == 0)
                                    {
                                        num15 /= Main.rand.Next(3) + 1;
                                    }
                                    num12 -= 100 * num15;
                                    Item.NewItem(i * 0x10, j * 0x10, 0x10, 0x10, 0x48, num15, false);
                                    continue;
                                }
                                int num16 = (int) num12;
                                if ((num16 > 50) && (Main.rand.Next(2) == 0))
                                {
                                    num16 /= Main.rand.Next(3) + 1;
                                }
                                if (Main.rand.Next(2) == 0)
                                {
                                    num16 /= Main.rand.Next(4) + 1;
                                }
                                if (num16 < 1)
                                {
                                    num16 = 1;
                                }
                                num12 -= num16;
                                Item.NewItem(i * 0x10, j * 0x10, 0x10, 0x10, 0x47, num16, false);
                            }
                        }
                    }
                    destroyObject = false;
                }
            }
        }

        public static void CheckRoom(int x, int y)
        {
            if (canSpawn)
            {
                if (((x < 10) || (y < 10)) || ((x >= (Main.maxTilesX - 10)) || (y >= (lastMaxTilesY - 10))))
                {
                    canSpawn = false;
                }
                else
                {
                    for (int i = 0; i < numRoomTiles; i++)
                    {
                        if ((roomX[i] == x) && (roomY[i] == y))
                        {
                            return;
                        }
                    }
                    roomX[numRoomTiles] = x;
                    roomY[numRoomTiles] = y;
                    numRoomTiles++;
                    if (numRoomTiles >= maxRoomTiles)
                    {
                        canSpawn = false;
                    }
                    else
                    {
                        if (Main.tile[x, y].active)
                        {
                            houseTile[Main.tile[x, y].type] = true;
                            if (Main.tileSolid[Main.tile[x, y].type] || (Main.tile[x, y].type == 11))
                            {
                                return;
                            }
                        }
                        if (x < roomX1)
                        {
                            roomX1 = x;
                        }
                        if (x > roomX2)
                        {
                            roomX2 = x;
                        }
                        if (y < roomY1)
                        {
                            roomY1 = y;
                        }
                        if (y > roomY2)
                        {
                            roomY2 = y;
                        }
                        bool flag = false;
                        bool flag2 = false;
                        for (int j = -2; j < 3; j++)
                        {
                            if (Main.wallHouse[Main.tile[x + j, y].wall])
                            {
                                flag = true;
                            }
                            if (Main.tile[x + j, y].active && (Main.tileSolid[Main.tile[x + j, y].type] || (Main.tile[x + j, y].type == 11)))
                            {
                                flag = true;
                            }
                            if (Main.wallHouse[Main.tile[x, y + j].wall])
                            {
                                flag2 = true;
                            }
                            if (Main.tile[x, y + j].active && (Main.tileSolid[Main.tile[x, y + j].type] || (Main.tile[x, y + j].type == 11)))
                            {
                                flag2 = true;
                            }
                        }
                        if (!flag || !flag2)
                        {
                            canSpawn = false;
                        }
                        else
                        {
                            for (int k = x - 1; k < (x + 2); k++)
                            {
                                for (int m = y - 1; m < (y + 2); m++)
                                {
                                    if (((k != x) || (m != y)) && canSpawn)
                                    {
                                        CheckRoom(k, m);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void CheckSign(int x, int y, int type)
        {
            if (!destroyObject)
            {
                int num = x - 2;
                int num2 = x + 3;
                int num3 = y - 2;
                int num4 = y + 3;
                if ((((num >= 0) && (num2 <= Main.maxTilesX)) && (num3 >= 0)) && (num4 <= Main.maxTilesY))
                {
                    bool flag = false;
                    for (int i = num; i < num2; i++)
                    {
                        for (int k = num3; k < num4; k++)
                        {
                            if (Main.tile[i, k] == null)
                            {
                                Main.tile[i, k] = new Tile();
                            }
                        }
                    }
                    int num7 = Main.tile[x, y].frameX / 0x12;
                    int num8 = Main.tile[x, y].frameY / 0x12;
                    while (num7 > 1)
                    {
                        num7 -= 2;
                    }
                    int num9 = x - num7;
                    int num10 = y - num8;
                    int num11 = (Main.tile[num9, num10].frameX / 0x12) / 2;
                    num = num9;
                    num2 = num9 + 2;
                    num3 = num10;
                    num4 = num10 + 2;
                    num7 = 0;
                    for (int j = num; j < num2; j++)
                    {
                        num8 = 0;
                        for (int m = num3; m < num4; m++)
                        {
                            if (!Main.tile[j, m].active || (Main.tile[j, m].type != type))
                            {
                                flag = true;
                                goto Label_017B;
                            }
                            if (((Main.tile[j, m].frameX / 0x12) != (num7 + (num11 * 2))) || ((Main.tile[j, m].frameY / 0x12) != num8))
                            {
                                flag = true;
                                goto Label_017B;
                            }
                            num8++;
                        }
                    Label_017B:
                        num7++;
                    }
                    if (!flag)
                    {
                        if ((Main.tile[num9, num10 + 2].active && Main.tileSolid[Main.tile[num9, num10 + 2].type]) && (Main.tile[num9 + 1, num10 + 2].active && Main.tileSolid[Main.tile[num9 + 1, num10 + 2].type]))
                        {
                            num11 = 0;
                        }
                        else if (((Main.tile[num9, num10 - 1].active && Main.tileSolid[Main.tile[num9, num10 - 1].type]) && (!Main.tileSolidTop[Main.tile[num9, num10 - 1].type] && Main.tile[num9 + 1, num10 - 1].active)) && (Main.tileSolid[Main.tile[num9 + 1, num10 - 1].type] && !Main.tileSolidTop[Main.tile[num9 + 1, num10 - 1].type]))
                        {
                            num11 = 1;
                        }
                        else if (((Main.tile[num9 - 1, num10].active && Main.tileSolid[Main.tile[num9 - 1, num10].type]) && (!Main.tileSolidTop[Main.tile[num9 - 1, num10].type] && Main.tile[num9 - 1, num10 + 1].active)) && (Main.tileSolid[Main.tile[num9 - 1, num10 + 1].type] && !Main.tileSolidTop[Main.tile[num9 - 1, num10 + 1].type]))
                        {
                            num11 = 2;
                        }
                        else if (((Main.tile[num9 + 2, num10].active && Main.tileSolid[Main.tile[num9 + 2, num10].type]) && (!Main.tileSolidTop[Main.tile[num9 + 2, num10].type] && Main.tile[num9 + 2, num10 + 1].active)) && (Main.tileSolid[Main.tile[num9 + 2, num10 + 1].type] && !Main.tileSolidTop[Main.tile[num9 + 2, num10 + 1].type]))
                        {
                            num11 = 3;
                        }
                        else
                        {
                            flag = true;
                        }
                    }
                    if (flag)
                    {
                        destroyObject = true;
                        for (int n = num; n < num2; n++)
                        {
                            for (int num15 = num3; num15 < num4; num15++)
                            {
                                if (Main.tile[n, num15].type == type)
                                {
                                    KillTile(n, num15, false, false, false);
                                }
                            }
                        }
                        Sign.KillSign(num9, num10);
                        Item.NewItem(x * 0x10, y * 0x10, 0x20, 0x20, 0xab, 1, false);
                        destroyObject = false;
                    }
                    else
                    {
                        int num16 = 0x24 * num11;
                        for (int num17 = 0; num17 < 2; num17++)
                        {
                            for (int num18 = 0; num18 < 2; num18++)
                            {
                                Main.tile[num9 + num17, num10 + num18].active = true;
                                Main.tile[num9 + num17, num10 + num18].type = (byte) type;
                                Main.tile[num9 + num17, num10 + num18].frameX = (short) (num16 + (0x12 * num17));
                                Main.tile[num9 + num17, num10 + num18].frameY = (short) (0x12 * num18);
                            }
                        }
                    }
                }
            }
        }

        public static void CheckSunflower(int i, int j, int type = 0x1b)
        {
            if (!destroyObject)
            {
                bool flag = false;
                int num = 0;
                int num2 = j;
                num += Main.tile[i, j].frameX / 0x12;
                num2 += (Main.tile[i, j].frameY / 0x12) * -1;
                while (num > 1)
                {
                    num -= 2;
                }
                num *= -1;
                num += i;
                for (int k = num; k < (num + 2); k++)
                {
                    for (int m = num2; m < (num2 + 4); m++)
                    {
                        if (Main.tile[k, m] == null)
                        {
                            Main.tile[k, m] = new Tile();
                        }
                        int num5 = Main.tile[k, m].frameX / 0x12;
                        while (num5 > 1)
                        {
                            num5 -= 2;
                        }
                        if ((!Main.tile[k, m].active || (Main.tile[k, m].type != type)) || ((num5 != (k - num)) || (Main.tile[k, m].frameY != ((m - num2) * 0x12))))
                        {
                            flag = true;
                        }
                    }
                    if (Main.tile[k, num2 + 4] == null)
                    {
                        Main.tile[k, num2 + 4] = new Tile();
                    }
                    if (!Main.tile[k, num2 + 4].active || (Main.tile[k, num2 + 4].type != 2))
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    destroyObject = true;
                    for (int n = num; n < (num + 2); n++)
                    {
                        for (int num7 = num2; num7 < (num2 + 4); num7++)
                        {
                            if ((Main.tile[n, num7].type == type) && Main.tile[n, num7].active)
                            {
                                KillTile(n, num7, false, false, false);
                            }
                        }
                    }
                    Item.NewItem(i * 0x10, j * 0x10, 0x20, 0x20, 0x3f, 1, false);
                    destroyObject = false;
                }
            }
        }

        public static void clearWorld()
        {
            spawnEye = false;
            spawnNPC = 0;
            shadowOrbCount = 0;
            Main.helpText = 0;
            Main.dungeonX = 0;
            Main.dungeonY = 0;
            NPC.downedBoss1 = false;
            NPC.downedBoss2 = false;
            NPC.downedBoss3 = false;
            shadowOrbSmashed = false;
            spawnMeteor = false;
            stopDrops = false;
            Main.invasionDelay = 0;
            Main.invasionType = 0;
            Main.invasionSize = 0;
            Main.invasionWarn = 0;
            Main.invasionX = 0.0;
            noLiquidCheck = false;
            Liquid.numLiquid = 0;
            LiquidBuffer.numLiquidBuffer = 0;
            if (((Main.netMode == 1) || (lastMaxTilesX > Main.maxTilesX)) || (lastMaxTilesY > Main.maxTilesY))
            {
                for (int num = 0; num < lastMaxTilesX; num++)
                {
                    float num2 = ((float) num) / ((float) lastMaxTilesX);
                    Main.statusText = "Freeing unused resources: " + ((int) ((num2 * 100f) + 1f)) + "%";
                    for (int num3 = 0; num3 < lastMaxTilesY; num3++)
                    {
                        Main.tile[num, num3] = null;
                    }
                }
            }
            lastMaxTilesX = Main.maxTilesX;
            lastMaxTilesY = Main.maxTilesY;
            if (Main.netMode != 1)
            {
                for (int num4 = 0; num4 < Main.maxTilesX; num4++)
                {
                    float num5 = ((float) num4) / ((float) Main.maxTilesX);
                    Main.statusText = "Resetting game objects: " + ((int) ((num5 * 100f) + 1f)) + "%";
                    for (int num6 = 0; num6 < Main.maxTilesY; num6++)
                    {
                        Main.tile[num4, num6] = new Tile();
                    }
                }
            }
            for (int i = 0; i < 0x7d0; i++)
            {
                Main.dust[i] = new Dust();
            }
            for (int j = 0; j < 200; j++)
            {
                Main.gore[j] = new Gore();
            }
            for (int k = 0; k < 200; k++)
            {
                Main.item[k] = new Item();
            }
            for (int m = 0; m < 0x3e8; m++)
            {
                Main.npc[m] = new NPC();
            }
            for (int n = 0; n < 0x3e8; n++)
            {
                Main.projectile[n] = new Projectile();
            }
            for (int num12 = 0; num12 < 0x3e8; num12++)
            {
                Main.chest[num12] = null;
            }
            for (int num13 = 0; num13 < 0x3e8; num13++)
            {
                Main.sign[num13] = null;
            }
            for (int num14 = 0; num14 < Liquid.resLiquid; num14++)
            {
                Main.liquid[num14] = new Liquid();
            }
            for (int num15 = 0; num15 < 0x2710; num15++)
            {
                Main.liquidBuffer[num15] = new LiquidBuffer();
            }
            setWorldSize();
            worldCleared = true;
        }

        public static bool CloseDoor(int i, int j, bool forced = false)
        {
            int num = 0;
            int num2 = i;
            int num3 = j;
            if (Main.tile[i, j] == null)
            {
                Main.tile[i, j] = new Tile();
            }
            int frameX = Main.tile[i, j].frameX;
            int frameY = Main.tile[i, j].frameY;
            switch (frameX)
            {
                case 0:
                    num2 = i;
                    num = 1;
                    break;

                case 0x12:
                    num2 = i - 1;
                    num = 1;
                    break;

                case 0x24:
                    num2 = i + 1;
                    num = -1;
                    break;

                case 0x36:
                    num2 = i;
                    num = -1;
                    break;
            }
            switch (frameY)
            {
                case 0:
                    num3 = j;
                    break;

                case 0x12:
                    num3 = j - 1;
                    break;

                case 0x24:
                    num3 = j - 2;
                    break;
            }
            int num6 = num2;
            if (num == -1)
            {
                num6 = num2 - 1;
            }
            if (!forced)
            {
                for (int n = num3; n < (num3 + 3); n++)
                {
                    if (!Collision.EmptyTile(num2, n, true))
                    {
                        return false;
                    }
                }
            }
            for (int k = num6; k < (num6 + 2); k++)
            {
                for (int num9 = num3; num9 < (num3 + 3); num9++)
                {
                    if (k == num2)
                    {
                        if (Main.tile[k, num9] == null)
                        {
                            Main.tile[k, num9] = new Tile();
                        }
                        Main.tile[k, num9].type = 10;
                        Main.tile[k, num9].frameX = (short) (genRand.Next(3) * 0x12);
                    }
                    else
                    {
                        if (Main.tile[k, num9] == null)
                        {
                            Main.tile[k, num9] = new Tile();
                        }
                        Main.tile[k, num9].active = false;
                    }
                }
            }
            for (int m = num2 - 1; m <= (num2 + 1); m++)
            {
                for (int num11 = num3 - 1; num11 <= (num3 + 2); num11++)
                {
                    TileFrame(m, num11, false, false);
                }
            }
            Main.PlaySound(9, i * 0x10, j * 0x10, 1);
            return true;
        }

        public static void CreateNewWorld()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.worldGenCallBack), 1);
        }

        public static void dropMeteor()
        {
            bool flag = true;
            int num = 0;
            if (Main.netMode != 1)
            {
                for (int i = 0; i < 8; i++)
                {
                    if (Main.player[i].active)
                    {
                        flag = false;
                        break;
                    }
                }
                int num3 = 0;
                float num4 = Main.maxTilesX / 0x1068;
                int num5 = (int) (400f * num4);
                for (int j = 5; j < (Main.maxTilesX - 5); j++)
                {
                    for (int k = 5; k < Main.worldSurface; k++)
                    {
                        if (Main.tile[j, k].active && (Main.tile[j, k].type == 0x25))
                        {
                            num3++;
                            if (num3 > num5)
                            {
                                return;
                            }
                        }
                    }
                }
                while (!flag)
                {
                    float num8 = Main.maxTilesX * 0.08f;
                    int num9 = Main.rand.Next(50, Main.maxTilesX - 50);
                    while ((num9 > (Main.spawnTileX - num8)) && (num9 < (Main.spawnTileX + num8)))
                    {
                        num9 = Main.rand.Next(50, Main.maxTilesX - 50);
                    }
                    for (int m = Main.rand.Next(100); m < Main.maxTilesY; m++)
                    {
                        if (Main.tile[num9, m].active && Main.tileSolid[Main.tile[num9, m].type])
                        {
                            flag = meteor(num9, m);
                            break;
                        }
                    }
                    num++;
                    if (num >= 100)
                    {
                        return;
                    }
                }
            }
        }

        public static void DungeonEnt(int i, int j, int tileType, int wallType)
        {
            Vector2 vector;
            double num5 = dxStrength1;
            double num6 = dyStrength1;
            vector.X = i;
            vector.Y = j - (((float) num6) / 2f);
            dMinY = (int) vector.Y;
            int num7 = 1;
            if (i > (Main.maxTilesX / 2))
            {
                num7 = -1;
            }
            int num = ((int) (vector.X - (num5 * 0.60000002384185791))) - genRand.Next(2, 5);
            int maxTilesX = ((int) (vector.X + (num5 * 0.60000002384185791))) + genRand.Next(2, 5);
            int num2 = ((int) (vector.Y - (num6 * 0.60000002384185791))) - genRand.Next(2, 5);
            int maxTilesY = ((int) (vector.Y + (num6 * 0.60000002384185791))) + genRand.Next(8, 0x10);
            if (num < 0)
            {
                num = 0;
            }
            if (maxTilesX > Main.maxTilesX)
            {
                maxTilesX = Main.maxTilesX;
            }
            if (num2 < 0)
            {
                num2 = 0;
            }
            if (maxTilesY > Main.maxTilesY)
            {
                maxTilesY = Main.maxTilesY;
            }
            for (int k = num; k < maxTilesX; k++)
            {
                for (int num9 = num2; num9 < maxTilesY; num9++)
                {
                    Main.tile[k, num9].liquid = 0;
                    if (Main.tile[k, num9].wall != wallType)
                    {
                        Main.tile[k, num9].wall = 0;
                        if (((k > (num + 1)) && (k < (maxTilesX - 2))) && ((num9 > (num2 + 1)) && (num9 < (maxTilesY - 2))))
                        {
                            PlaceWall(k, num9, wallType, true);
                        }
                        Main.tile[k, num9].active = true;
                        Main.tile[k, num9].type = (byte) tileType;
                    }
                }
            }
            int num10 = num;
            int num11 = (num + 5) + genRand.Next(4);
            int num12 = (num2 - 3) - genRand.Next(3);
            int num13 = num2;
            for (int m = num10; m < num11; m++)
            {
                for (int num15 = num12; num15 < num13; num15++)
                {
                    if (Main.tile[m, num15].wall != wallType)
                    {
                        Main.tile[m, num15].active = true;
                        Main.tile[m, num15].type = (byte) tileType;
                    }
                }
            }
            num10 = (maxTilesX - 5) - genRand.Next(4);
            num11 = maxTilesX;
            num12 = (num2 - 3) - genRand.Next(3);
            num13 = num2;
            for (int n = num10; n < num11; n++)
            {
                for (int num17 = num12; num17 < num13; num17++)
                {
                    if (Main.tile[n, num17].wall != wallType)
                    {
                        Main.tile[n, num17].active = true;
                        Main.tile[n, num17].type = (byte) tileType;
                    }
                }
            }
            int num18 = 1 + genRand.Next(2);
            int num19 = 2 + genRand.Next(4);
            int num20 = 0;
            for (int num21 = num; num21 < maxTilesX; num21++)
            {
                for (int num22 = num2 - num18; num22 < num2; num22++)
                {
                    if (Main.tile[num21, num22].wall != wallType)
                    {
                        Main.tile[num21, num22].active = true;
                        Main.tile[num21, num22].type = (byte) tileType;
                    }
                }
                num20++;
                if (num20 >= num19)
                {
                    num21 += num19;
                    num20 = 0;
                }
            }
            for (int num23 = num; num23 < maxTilesX; num23++)
            {
                for (int num24 = maxTilesY; num24 < (maxTilesY + 100); num24++)
                {
                    PlaceWall(num23, num24, 2, true);
                }
            }
            num = (int) (vector.X - (num5 * 0.60000002384185791));
            maxTilesX = (int) (vector.X + (num5 * 0.60000002384185791));
            num2 = (int) (vector.Y - (num6 * 0.60000002384185791));
            maxTilesY = (int) (vector.Y + (num6 * 0.60000002384185791));
            if (num < 0)
            {
                num = 0;
            }
            if (maxTilesX > Main.maxTilesX)
            {
                maxTilesX = Main.maxTilesX;
            }
            if (num2 < 0)
            {
                num2 = 0;
            }
            if (maxTilesY > Main.maxTilesY)
            {
                maxTilesY = Main.maxTilesY;
            }
            for (int num25 = num; num25 < maxTilesX; num25++)
            {
                for (int num26 = num2; num26 < maxTilesY; num26++)
                {
                    PlaceWall(num25, num26, wallType, true);
                }
            }
            num = (int) ((vector.X - (num5 * 0.6)) - 1.0);
            maxTilesX = (int) ((vector.X + (num5 * 0.6)) + 1.0);
            num2 = (int) ((vector.Y - (num6 * 0.6)) - 1.0);
            maxTilesY = (int) ((vector.Y + (num6 * 0.6)) + 1.0);
            if (num < 0)
            {
                num = 0;
            }
            if (maxTilesX > Main.maxTilesX)
            {
                maxTilesX = Main.maxTilesX;
            }
            if (num2 < 0)
            {
                num2 = 0;
            }
            if (maxTilesY > Main.maxTilesY)
            {
                maxTilesY = Main.maxTilesY;
            }
            for (int num27 = num; num27 < maxTilesX; num27++)
            {
                for (int num28 = num2; num28 < maxTilesY; num28++)
                {
                    Main.tile[num27, num28].wall = (byte) wallType;
                }
            }
            num = (int) (vector.X - (num5 * 0.5));
            maxTilesX = (int) (vector.X + (num5 * 0.5));
            num2 = (int) (vector.Y - (num6 * 0.5));
            maxTilesY = (int) (vector.Y + (num6 * 0.5));
            if (num < 0)
            {
                num = 0;
            }
            if (maxTilesX > Main.maxTilesX)
            {
                maxTilesX = Main.maxTilesX;
            }
            if (num2 < 0)
            {
                num2 = 0;
            }
            if (maxTilesY > Main.maxTilesY)
            {
                maxTilesY = Main.maxTilesY;
            }
            for (int num29 = num; num29 < maxTilesX; num29++)
            {
                for (int num30 = num2; num30 < maxTilesY; num30++)
                {
                    Main.tile[num29, num30].active = false;
                    Main.tile[num29, num30].wall = (byte) wallType;
                }
            }
            DPlatX[numDPlats] = (int) vector.X;
            DPlatY[numDPlats] = maxTilesY;
            numDPlats++;
            vector.X += (((float) num5) * 0.6f) * num7;
            vector.Y += ((float) num6) * 0.5f;
            num5 = dxStrength2;
            num6 = dyStrength2;
            vector.X += (((float) num5) * 0.55f) * num7;
            vector.Y -= ((float) num6) * 0.5f;
            num = ((int) (vector.X - (num5 * 0.60000002384185791))) - genRand.Next(1, 3);
            maxTilesX = ((int) (vector.X + (num5 * 0.60000002384185791))) + genRand.Next(1, 3);
            num2 = ((int) (vector.Y - (num6 * 0.60000002384185791))) - genRand.Next(1, 3);
            maxTilesY = ((int) (vector.Y + (num6 * 0.60000002384185791))) + genRand.Next(6, 0x10);
            if (num < 0)
            {
                num = 0;
            }
            if (maxTilesX > Main.maxTilesX)
            {
                maxTilesX = Main.maxTilesX;
            }
            if (num2 < 0)
            {
                num2 = 0;
            }
            if (maxTilesY > Main.maxTilesY)
            {
                maxTilesY = Main.maxTilesY;
            }
            for (int num31 = num; num31 < maxTilesX; num31++)
            {
                for (int num32 = num2; num32 < maxTilesY; num32++)
                {
                    if (Main.tile[num31, num32].wall != wallType)
                    {
                        bool flag = true;
                        if (num7 < 0)
                        {
                            if (num31 < (vector.X - (num5 * 0.5)))
                            {
                                flag = false;
                            }
                        }
                        else if (num31 > ((vector.X + (num5 * 0.5)) - 1.0))
                        {
                            flag = false;
                        }
                        if (flag)
                        {
                            Main.tile[num31, num32].wall = 0;
                            Main.tile[num31, num32].active = true;
                            Main.tile[num31, num32].type = (byte) tileType;
                        }
                    }
                }
            }
            for (int num33 = num; num33 < maxTilesX; num33++)
            {
                for (int num34 = maxTilesY; num34 < (maxTilesY + 100); num34++)
                {
                    PlaceWall(num33, num34, 2, true);
                }
            }
            num = (int) (vector.X - (num5 * 0.5));
            maxTilesX = (int) (vector.X + (num5 * 0.5));
            num10 = num;
            if (num7 < 0)
            {
                num10++;
            }
            num11 = (num10 + 5) + genRand.Next(4);
            num12 = (num2 - 3) - genRand.Next(3);
            num13 = num2;
            for (int num35 = num10; num35 < num11; num35++)
            {
                for (int num36 = num12; num36 < num13; num36++)
                {
                    if (Main.tile[num35, num36].wall != wallType)
                    {
                        Main.tile[num35, num36].active = true;
                        Main.tile[num35, num36].type = (byte) tileType;
                    }
                }
            }
            num10 = (maxTilesX - 5) - genRand.Next(4);
            num11 = maxTilesX;
            num12 = (num2 - 3) - genRand.Next(3);
            num13 = num2;
            for (int num37 = num10; num37 < num11; num37++)
            {
                for (int num38 = num12; num38 < num13; num38++)
                {
                    if (Main.tile[num37, num38].wall != wallType)
                    {
                        Main.tile[num37, num38].active = true;
                        Main.tile[num37, num38].type = (byte) tileType;
                    }
                }
            }
            num18 = 1 + genRand.Next(2);
            num19 = 2 + genRand.Next(4);
            num20 = 0;
            if (num7 < 0)
            {
                maxTilesX++;
            }
            for (int num39 = num + 1; num39 < (maxTilesX - 1); num39++)
            {
                for (int num40 = num2 - num18; num40 < num2; num40++)
                {
                    if (Main.tile[num39, num40].wall != wallType)
                    {
                        Main.tile[num39, num40].active = true;
                        Main.tile[num39, num40].type = (byte) tileType;
                    }
                }
                num20++;
                if (num20 >= num19)
                {
                    num39 += num19;
                    num20 = 0;
                }
            }
            num = (int) (vector.X - (num5 * 0.6));
            maxTilesX = (int) (vector.X + (num5 * 0.6));
            num2 = (int) (vector.Y - (num6 * 0.6));
            maxTilesY = (int) (vector.Y + (num6 * 0.6));
            if (num < 0)
            {
                num = 0;
            }
            if (maxTilesX > Main.maxTilesX)
            {
                maxTilesX = Main.maxTilesX;
            }
            if (num2 < 0)
            {
                num2 = 0;
            }
            if (maxTilesY > Main.maxTilesY)
            {
                maxTilesY = Main.maxTilesY;
            }
            for (int num41 = num; num41 < maxTilesX; num41++)
            {
                for (int num42 = num2; num42 < maxTilesY; num42++)
                {
                    Main.tile[num41, num42].wall = 0;
                }
            }
            num = (int) (vector.X - (num5 * 0.5));
            maxTilesX = (int) (vector.X + (num5 * 0.5));
            num2 = (int) (vector.Y - (num6 * 0.5));
            maxTilesY = (int) (vector.Y + (num6 * 0.5));
            if (num < 0)
            {
                num = 0;
            }
            if (maxTilesX > Main.maxTilesX)
            {
                maxTilesX = Main.maxTilesX;
            }
            if (num2 < 0)
            {
                num2 = 0;
            }
            if (maxTilesY > Main.maxTilesY)
            {
                maxTilesY = Main.maxTilesY;
            }
            for (int num43 = num; num43 < maxTilesX; num43++)
            {
                for (int num44 = num2; num44 < maxTilesY; num44++)
                {
                    Main.tile[num43, num44].active = false;
                    Main.tile[num43, num44].wall = 0;
                }
            }
            for (int num45 = num; num45 < maxTilesX; num45++)
            {
                if (!Main.tile[num45, maxTilesY].active)
                {
                    Main.tile[num45, maxTilesY].active = true;
                    Main.tile[num45, maxTilesY].type = 0x13;
                }
            }
            Main.dungeonX = (int) vector.X;
            Main.dungeonY = maxTilesY;
            int index = NPC.NewNPC((dungeonX * 0x10) + 8, dungeonY * 0x10, 0x25, 0);
            Main.npc[index].homeless = false;
            Main.npc[index].homeTileX = Main.dungeonX;
            Main.npc[index].homeTileY = Main.dungeonY;
            if (num7 == 1)
            {
                int num47 = 0;
                for (int num48 = maxTilesX; num48 < (maxTilesX + 0x19); num48++)
                {
                    num47++;
                    for (int num49 = maxTilesY + num47; num49 < (maxTilesY + 0x19); num49++)
                    {
                        Main.tile[num48, num49].active = true;
                        Main.tile[num48, num49].type = (byte) tileType;
                    }
                }
            }
            else
            {
                int num50 = 0;
                for (int num51 = num; num51 > (num - 0x19); num51--)
                {
                    num50++;
                    for (int num52 = maxTilesY + num50; num52 < (maxTilesY + 0x19); num52++)
                    {
                        Main.tile[num51, num52].active = true;
                        Main.tile[num51, num52].type = (byte) tileType;
                    }
                }
            }
            num18 = 1 + genRand.Next(2);
            num19 = 2 + genRand.Next(4);
            num20 = 0;
            num = (int) (vector.X - (num5 * 0.5));
            maxTilesX = (int) (vector.X + (num5 * 0.5));
            num += 2;
            maxTilesX -= 2;
            for (int num53 = num; num53 < maxTilesX; num53++)
            {
                for (int num54 = num2; num54 < maxTilesY; num54++)
                {
                    PlaceWall(num53, num54, wallType, true);
                }
                num20++;
                if (num20 >= num19)
                {
                    num53 += num19 * 2;
                    num20 = 0;
                }
            }
            vector.X -= (((float) num5) * 0.6f) * num7;
            vector.Y += ((float) num6) * 0.5f;
            num5 = 15.0;
            num6 = 3.0;
            vector.Y -= ((float) num6) * 0.5f;
            num = (int) (vector.X - (num5 * 0.5));
            maxTilesX = (int) (vector.X + (num5 * 0.5));
            num2 = (int) (vector.Y - (num6 * 0.5));
            maxTilesY = (int) (vector.Y + (num6 * 0.5));
            if (num < 0)
            {
                num = 0;
            }
            if (maxTilesX > Main.maxTilesX)
            {
                maxTilesX = Main.maxTilesX;
            }
            if (num2 < 0)
            {
                num2 = 0;
            }
            if (maxTilesY > Main.maxTilesY)
            {
                maxTilesY = Main.maxTilesY;
            }
            for (int num55 = num; num55 < maxTilesX; num55++)
            {
                for (int num56 = num2; num56 < maxTilesY; num56++)
                {
                    Main.tile[num55, num56].active = false;
                }
            }
            if (num7 < 0)
            {
                vector.X--;
            }
            PlaceTile((int) vector.X, ((int) vector.Y) + 1, 10, false, false, -1);
        }

        public static void DungeonHalls(int i, int j, int tileType, int wallType, bool forceX = false)
        {
            Vector2 vector;
            Vector2 vector2 = new Vector2();
            double num5 = genRand.Next(4, 6);
            Vector2 vector3 = new Vector2();
            Vector2 vector4 = new Vector2();
            int num6 = 1;
            vector.X = i;
            vector.Y = j;
            int num7 = genRand.Next(0x23, 80);
            if (forceX)
            {
                num7 += 20;
                lastDungeonHall = new Vector2();
            }
            else if (genRand.Next(5) == 0)
            {
                num5 *= 2.0;
                num7 /= 2;
            }
            bool flag = false;
            while (!flag)
            {
                if (genRand.Next(2) == 0)
                {
                    num6 = -1;
                }
                else
                {
                    num6 = 1;
                }
                bool flag2 = false;
                if (genRand.Next(2) == 0)
                {
                    flag2 = true;
                }
                if (forceX)
                {
                    flag2 = true;
                }
                if (flag2)
                {
                    vector3.Y = 0f;
                    vector3.X = num6;
                    vector4.Y = 0f;
                    vector4.X = -num6;
                    vector2.Y = 0f;
                    vector2.X = num6;
                    if (genRand.Next(3) == 0)
                    {
                        if (genRand.Next(2) == 0)
                        {
                            vector2.Y = -0.2f;
                        }
                        else
                        {
                            vector2.Y = 0.2f;
                        }
                    }
                }
                else
                {
                    num5++;
                    vector2.Y = num6;
                    vector2.X = 0f;
                    vector3.X = 0f;
                    vector3.Y = num6;
                    vector4.X = 0f;
                    vector4.Y = -num6;
                    if (genRand.Next(2) == 0)
                    {
                        if (genRand.Next(2) == 0)
                        {
                            vector2.X = 0.3f;
                        }
                        else
                        {
                            vector2.X = -0.3f;
                        }
                    }
                    else
                    {
                        num7 /= 2;
                    }
                }
                if (lastDungeonHall != vector4)
                {
                    flag = true;
                }
            }
            if (!forceX)
            {
                if (vector.X > (lastMaxTilesX - 200))
                {
                    num6 = -1;
                    vector3.Y = 0f;
                    vector3.X = num6;
                    vector2.Y = 0f;
                    vector2.X = num6;
                    if (genRand.Next(3) == 0)
                    {
                        if (genRand.Next(2) == 0)
                        {
                            vector2.Y = -0.2f;
                        }
                        else
                        {
                            vector2.Y = 0.2f;
                        }
                    }
                }
                else if (vector.X < 200f)
                {
                    num6 = 1;
                    vector3.Y = 0f;
                    vector3.X = num6;
                    vector2.Y = 0f;
                    vector2.X = num6;
                    if (genRand.Next(3) == 0)
                    {
                        if (genRand.Next(2) == 0)
                        {
                            vector2.Y = -0.2f;
                        }
                        else
                        {
                            vector2.Y = 0.2f;
                        }
                    }
                }
                else if (vector.Y > (lastMaxTilesY + 200))
                {
                    num6 = -1;
                    num5++;
                    vector2.Y = num6;
                    vector2.X = 0f;
                    vector3.X = 0f;
                    vector3.Y = num6;
                    if (genRand.Next(2) == 0)
                    {
                        if (genRand.Next(2) == 0)
                        {
                            vector2.X = 0.3f;
                        }
                        else
                        {
                            vector2.X = -0.3f;
                        }
                    }
                }
                else if (vector.Y < Main.rockLayer)
                {
                    num6 = 1;
                    num5++;
                    vector2.Y = num6;
                    vector2.X = 0f;
                    vector3.X = 0f;
                    vector3.Y = num6;
                    if (genRand.Next(2) == 0)
                    {
                        if (genRand.Next(2) == 0)
                        {
                            vector2.X = 0.3f;
                        }
                        else
                        {
                            vector2.X = -0.3f;
                        }
                    }
                }
                else if ((vector.X < (Main.maxTilesX / 2)) && (vector.X > (Main.maxTilesX * 0.25)))
                {
                    num6 = -1;
                    vector3.Y = 0f;
                    vector3.X = num6;
                    vector2.Y = 0f;
                    vector2.X = num6;
                    if (genRand.Next(3) == 0)
                    {
                        if (genRand.Next(2) == 0)
                        {
                            vector2.Y = -0.2f;
                        }
                        else
                        {
                            vector2.Y = 0.2f;
                        }
                    }
                }
                else if ((vector.X > (Main.maxTilesX / 2)) && (vector.X < (Main.maxTilesX * 0.75)))
                {
                    num6 = 1;
                    vector3.Y = 0f;
                    vector3.X = num6;
                    vector2.Y = 0f;
                    vector2.X = num6;
                    if (genRand.Next(3) == 0)
                    {
                        if (genRand.Next(2) == 0)
                        {
                            vector2.Y = -0.2f;
                        }
                        else
                        {
                            vector2.Y = 0.2f;
                        }
                    }
                }
            }
            if (vector3.Y == 0f)
            {
                DDoorX[numDDoors] = (int) vector.X;
                DDoorY[numDDoors] = (int) vector.Y;
                DDoorPos[numDDoors] = 0;
                numDDoors++;
            }
            else
            {
                DPlatX[numDPlats] = (int) vector.X;
                DPlatY[numDPlats] = (int) vector.Y;
                numDPlats++;
            }
            lastDungeonHall = vector3;
            while (num7 > 0)
            {
                if ((vector3.X > 0f) && (vector.X > (Main.maxTilesX - 100)))
                {
                    num7 = 0;
                }
                else if ((vector3.X < 0f) && (vector.X < 100f))
                {
                    num7 = 0;
                }
                else if ((vector3.Y > 0f) && (vector.Y > (Main.maxTilesY - 100)))
                {
                    num7 = 0;
                }
                else if ((vector3.Y < 0f) && (vector.Y < (Main.rockLayer + 50.0)))
                {
                    num7 = 0;
                }
                num7--;
                int num = ((int) ((vector.X - num5) - 4.0)) - genRand.Next(6);
                int maxTilesX = ((int) ((vector.X + num5) + 4.0)) + genRand.Next(6);
                int num2 = ((int) ((vector.Y - num5) - 4.0)) - genRand.Next(6);
                int maxTilesY = ((int) ((vector.Y + num5) + 4.0)) + genRand.Next(6);
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > Main.maxTilesX)
                {
                    maxTilesX = Main.maxTilesX;
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > Main.maxTilesY)
                {
                    maxTilesY = Main.maxTilesY;
                }
                for (int k = num; k < maxTilesX; k++)
                {
                    for (int num9 = num2; num9 < maxTilesY; num9++)
                    {
                        Main.tile[k, num9].liquid = 0;
                        if (Main.tile[k, num9].wall == 0)
                        {
                            Main.tile[k, num9].active = true;
                            Main.tile[k, num9].type = (byte) tileType;
                        }
                    }
                }
                for (int m = num + 1; m < (maxTilesX - 1); m++)
                {
                    for (int num11 = num2 + 1; num11 < (maxTilesY - 1); num11++)
                    {
                        PlaceWall(m, num11, wallType, true);
                    }
                }
                int num12 = 0;
                if ((vector2.Y == 0f) && (genRand.Next(((int) num5) + 1) == 0))
                {
                    num12 = genRand.Next(1, 3);
                }
                else if ((vector2.X == 0f) && (genRand.Next(((int) num5) - 1) == 0))
                {
                    num12 = genRand.Next(1, 3);
                }
                else if (genRand.Next(((int) num5) * 3) == 0)
                {
                    num12 = genRand.Next(1, 3);
                }
                num = ((int) (vector.X - (num5 * 0.5))) - num12;
                maxTilesX = ((int) (vector.X + (num5 * 0.5))) + num12;
                num2 = ((int) (vector.Y - (num5 * 0.5))) - num12;
                maxTilesY = ((int) (vector.Y + (num5 * 0.5))) + num12;
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > Main.maxTilesX)
                {
                    maxTilesX = Main.maxTilesX;
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > Main.maxTilesY)
                {
                    maxTilesY = Main.maxTilesY;
                }
                for (int n = num; n < maxTilesX; n++)
                {
                    for (int num14 = num2; num14 < maxTilesY; num14++)
                    {
                        Main.tile[n, num14].active = false;
                        Main.tile[n, num14].wall = (byte) wallType;
                    }
                }
                vector += vector2;
            }
            dungeonX = (int) vector.X;
            dungeonY = (int) vector.Y;
            if (vector3.Y == 0f)
            {
                DDoorX[numDDoors] = (int) vector.X;
                DDoorY[numDDoors] = (int) vector.Y;
                DDoorPos[numDDoors] = 0;
                numDDoors++;
            }
            else
            {
                DPlatX[numDPlats] = (int) vector.X;
                DPlatY[numDPlats] = (int) vector.Y;
                numDPlats++;
            }
        }

        public static void DungeonRoom(int i, int j, int tileType, int wallType)
        {
            Vector2 vector;
            Vector2 vector2;
            double num5 = genRand.Next(15, 30);
            vector2.X = genRand.Next(-10, 11) * 0.1f;
            vector2.Y = genRand.Next(-10, 11) * 0.1f;
            vector.X = i;
            vector.Y = j - (((float) num5) / 2f);
            int num6 = genRand.Next(10, 20);
            double x = vector.X;
            double num8 = vector.X;
            double y = vector.Y;
            double num10 = vector.Y;
            while (num6 > 0)
            {
                num6--;
                int num = (int) ((vector.X - (num5 * 0.800000011920929)) - 5.0);
                int maxTilesX = (int) ((vector.X + (num5 * 0.800000011920929)) + 5.0);
                int num2 = (int) ((vector.Y - (num5 * 0.800000011920929)) - 5.0);
                int maxTilesY = (int) ((vector.Y + (num5 * 0.800000011920929)) + 5.0);
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > Main.maxTilesX)
                {
                    maxTilesX = Main.maxTilesX;
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > Main.maxTilesY)
                {
                    maxTilesY = Main.maxTilesY;
                }
                for (int k = num; k < maxTilesX; k++)
                {
                    for (int num12 = num2; num12 < maxTilesY; num12++)
                    {
                        Main.tile[k, num12].liquid = 0;
                        if (Main.tile[k, num12].wall == 0)
                        {
                            Main.tile[k, num12].active = true;
                            Main.tile[k, num12].type = (byte) tileType;
                        }
                    }
                }
                for (int m = num + 1; m < (maxTilesX - 1); m++)
                {
                    for (int num14 = num2 + 1; num14 < (maxTilesY - 1); num14++)
                    {
                        PlaceWall(m, num14, wallType, true);
                    }
                }
                num = (int) (vector.X - (num5 * 0.5));
                maxTilesX = (int) (vector.X + (num5 * 0.5));
                num2 = (int) (vector.Y - (num5 * 0.5));
                maxTilesY = (int) (vector.Y + (num5 * 0.5));
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > Main.maxTilesX)
                {
                    maxTilesX = Main.maxTilesX;
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > Main.maxTilesY)
                {
                    maxTilesY = Main.maxTilesY;
                }
                if (num < x)
                {
                    x = num;
                }
                if (maxTilesX > num8)
                {
                    num8 = maxTilesX;
                }
                if (num2 < y)
                {
                    y = num2;
                }
                if (maxTilesY > num10)
                {
                    num10 = maxTilesY;
                }
                for (int n = num; n < maxTilesX; n++)
                {
                    for (int num16 = num2; num16 < maxTilesY; num16++)
                    {
                        Main.tile[n, num16].active = false;
                        Main.tile[n, num16].wall = (byte) wallType;
                    }
                }
                vector += vector2;
                vector2.X += genRand.Next(-10, 11) * 0.05f;
                vector2.Y += genRand.Next(-10, 11) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if (vector2.Y > 1f)
                {
                    vector2.Y = 1f;
                }
                if (vector2.Y < -1f)
                {
                    vector2.Y = -1f;
                }
            }
            dRoomX[numDRooms] = (int) vector.X;
            dRoomY[numDRooms] = (int) vector.Y;
            dRoomSize[numDRooms] = (int) num5;
            dRoomL[numDRooms] = (int) x;
            dRoomR[numDRooms] = (int) num8;
            dRoomT[numDRooms] = (int) y;
            dRoomB[numDRooms] = (int) num10;
            dRoomTreasure[numDRooms] = false;
            numDRooms++;
        }

        public static void DungeonStairs(int i, int j, int tileType, int wallType)
        {
            Vector2 vector;
            Vector2 vector2 = new Vector2();
            double num5 = genRand.Next(5, 9);
            int num6 = 1;
            vector.X = i;
            vector.Y = j;
            int num7 = genRand.Next(10, 30);
            if (i > dEnteranceX)
            {
                num6 = -1;
            }
            else
            {
                num6 = 1;
            }
            vector2.Y = -1f;
            vector2.X = num6;
            if (genRand.Next(3) == 0)
            {
                vector2.X *= 0.5f;
            }
            else if (genRand.Next(3) == 0)
            {
                vector2.Y *= 2f;
            }
            while (num7 > 0)
            {
                num7--;
                int num = ((int) ((vector.X - num5) - 4.0)) - genRand.Next(6);
                int maxTilesX = ((int) ((vector.X + num5) + 4.0)) + genRand.Next(6);
                int num2 = (int) ((vector.Y - num5) - 4.0);
                int maxTilesY = ((int) ((vector.Y + num5) + 4.0)) + genRand.Next(6);
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > Main.maxTilesX)
                {
                    maxTilesX = Main.maxTilesX;
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > Main.maxTilesY)
                {
                    maxTilesY = Main.maxTilesY;
                }
                int num8 = 1;
                if (vector.X > (Main.maxTilesX / 2))
                {
                    num8 = -1;
                }
                int num9 = (int) ((vector.X + ((((float) dxStrength1) * 0.6f) * num8)) + (((float) dxStrength2) * num8));
                int num10 = (int) (dyStrength2 * 0.5);
                if (((vector.Y < (Main.worldSurface - 5.0)) && (Main.tile[num9, ((int) ((vector.Y - num5) - 6.0)) + num10].wall == 0)) && ((Main.tile[num9, ((int) ((vector.Y - num5) - 7.0)) + num10].wall == 0) && (Main.tile[num9, ((int) ((vector.Y - num5) - 8.0)) + num10].wall == 0)))
                {
                    dSurface = true;
                    TileRunner(num9, ((int) ((vector.Y - num5) - 6.0)) + num10, (double) genRand.Next(0x19, 0x23), genRand.Next(10, 20), -1, false, 0f, -1f, false, true);
                }
                for (int k = num; k < maxTilesX; k++)
                {
                    for (int num12 = num2; num12 < maxTilesY; num12++)
                    {
                        Main.tile[k, num12].liquid = 0;
                        if (Main.tile[k, num12].wall != wallType)
                        {
                            Main.tile[k, num12].wall = 0;
                            Main.tile[k, num12].active = true;
                            Main.tile[k, num12].type = (byte) tileType;
                        }
                    }
                }
                for (int m = num + 1; m < (maxTilesX - 1); m++)
                {
                    for (int num14 = num2 + 1; num14 < (maxTilesY - 1); num14++)
                    {
                        PlaceWall(m, num14, wallType, true);
                    }
                }
                int num15 = 0;
                if (genRand.Next((int) num5) == 0)
                {
                    num15 = genRand.Next(1, 3);
                }
                num = ((int) (vector.X - (num5 * 0.5))) - num15;
                maxTilesX = ((int) (vector.X + (num5 * 0.5))) + num15;
                num2 = ((int) (vector.Y - (num5 * 0.5))) - num15;
                maxTilesY = ((int) (vector.Y + (num5 * 0.5))) + num15;
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > Main.maxTilesX)
                {
                    maxTilesX = Main.maxTilesX;
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > Main.maxTilesY)
                {
                    maxTilesY = Main.maxTilesY;
                }
                for (int n = num; n < maxTilesX; n++)
                {
                    for (int num17 = num2; num17 < maxTilesY; num17++)
                    {
                        Main.tile[n, num17].active = false;
                        PlaceWall(n, num17, wallType, true);
                    }
                }
                if (dSurface)
                {
                    num7 = 0;
                }
                vector += vector2;
            }
            dungeonX = (int) vector.X;
            dungeonY = (int) vector.Y;
        }

        public static bool EmptyTileCheck(int startX, int endX, int startY, int endY, int ignoreStyle = -1)
        {
            if (startX < 0)
            {
                return false;
            }
            if (endX >= Main.maxTilesX)
            {
                return false;
            }
            if (startY < 0)
            {
                return false;
            }
            if (endY >= Main.maxTilesY)
            {
                return false;
            }
            for (int i = startX; i < (endX + 1); i++)
            {
                for (int j = startY; j < (endY + 1); j++)
                {
                    if (Main.tile[i, j].active)
                    {
                        if (ignoreStyle == -1)
                        {
                            return false;
                        }
                        if ((ignoreStyle == 11) && (Main.tile[i, j].type != 11))
                        {
                            return false;
                        }
                        if (((ignoreStyle == 20) && (Main.tile[i, j].type != 20)) && (Main.tile[i, j].type != 3))
                        {
                            return false;
                        }
                        if ((ignoreStyle == 0x47) && (Main.tile[i, j].type != 0x47))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public static void EveryTileFrame()
        {
            noLiquidCheck = true;
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                float num2 = ((float) i) / ((float) Main.maxTilesX);
                Main.statusText = "Finding tile frames: " + ((int) ((num2 * 100f) + 1f)) + "%";
                for (int j = 0; j < Main.maxTilesY; j++)
                {
                    TileFrame(i, j, true, false);
                    WallFrame(i, j, true);
                }
            }
            noLiquidCheck = false;
        }

        public static void FloatingIsland(int i, int j)
        {
            Vector2 vector;
            Vector2 vector2;
            double num5 = genRand.Next(80, 120);
            double num6 = num5;
            float num7 = genRand.Next(20, 0x19);
            vector.X = i;
            vector.Y = j;
            vector2.X = genRand.Next(-20, 0x15) * 0.2f;
            while ((vector2.X > -2f) && (vector2.X < 2f))
            {
                vector2.X = genRand.Next(-20, 0x15) * 0.2f;
            }
            vector2.Y = genRand.Next(-20, -10) * 0.02f;
            while ((num5 > 0.0) && (num7 > 0f))
            {
                num5 -= genRand.Next(4);
                num7--;
                int num = (int) (vector.X - (num5 * 0.5));
                int maxTilesX = (int) (vector.X + (num5 * 0.5));
                int num2 = (int) (vector.Y - (num5 * 0.5));
                int maxTilesY = (int) (vector.Y + (num5 * 0.5));
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > Main.maxTilesX)
                {
                    maxTilesX = Main.maxTilesX;
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > Main.maxTilesY)
                {
                    maxTilesY = Main.maxTilesY;
                }
                num6 = (num5 * genRand.Next(80, 120)) * 0.01;
                float y = vector.Y + 1f;
                for (int k = num; k < maxTilesX; k++)
                {
                    if (genRand.Next(2) == 0)
                    {
                        y += genRand.Next(-1, 2);
                    }
                    if (y < vector.Y)
                    {
                        y = vector.Y;
                    }
                    if (y > (vector.Y + 2f))
                    {
                        y = vector.Y + 2f;
                    }
                    for (int n = num2; n < maxTilesY; n++)
                    {
                        if (n > y)
                        {
                            float num11 = Math.Abs((float) (k - vector.X));
                            float num12 = Math.Abs((float) (n - vector.Y)) * 2f;
                            if (Math.Sqrt((double) ((num11 * num11) + (num12 * num12))) < (num6 * 0.4))
                            {
                                Main.tile[k, n].active = true;
                            }
                        }
                    }
                }
                TileRunner(genRand.Next(num + 10, maxTilesX - 10), (int) ((vector.Y + (num6 * 0.1)) + 5.0), (double) genRand.Next(5, 10), genRand.Next(10, 15), 0, true, 0f, 2f, true, true);
                num = (int) (vector.X - (num5 * 0.4));
                maxTilesX = (int) (vector.X + (num5 * 0.4));
                num2 = (int) (vector.Y - (num5 * 0.4));
                maxTilesY = (int) (vector.Y + (num5 * 0.4));
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > Main.maxTilesX)
                {
                    maxTilesX = Main.maxTilesX;
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > Main.maxTilesY)
                {
                    maxTilesY = Main.maxTilesY;
                }
                num6 = (num5 * genRand.Next(80, 120)) * 0.01;
                for (int m = num; m < maxTilesX; m++)
                {
                    for (int num15 = num2; num15 < maxTilesY; num15++)
                    {
                        if (num15 > (vector.Y + 2f))
                        {
                            float num16 = Math.Abs((float) (m - vector.X));
                            float num17 = Math.Abs((float) (num15 - vector.Y)) * 2f;
                            if (Math.Sqrt((double) ((num16 * num16) + (num17 * num17))) < (num6 * 0.4))
                            {
                                Main.tile[m, num15].wall = 2;
                            }
                        }
                    }
                }
                vector += vector2;
                vector2.Y += genRand.Next(-10, 11) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if (vector2.Y > 0.2)
                {
                    vector2.Y = -0.2f;
                }
                if (vector2.Y < -0.2)
                {
                    vector2.Y = -0.2f;
                }
            }
        }

        public static void generateWorld(int seed = -1)
        {
            gen = true;
            if (seed > 0)
            {
                genRand = new Random(seed);
            }
            else
            {
                genRand = new Random((int) DateTime.Now.Ticks);
            }
            Main.worldID = genRand.Next(0x7fffffff);
            int num7 = 0;
            int num8 = 0;
            double num = Main.maxTilesY * 0.3;
            num *= genRand.Next(90, 110) * 0.005;
            double num4 = num + (Main.maxTilesY * 0.2);
            num4 *= genRand.Next(90, 110) * 0.01;
            double num2 = num;
            double num3 = num;
            double num5 = num4;
            double num6 = num4;
            int num9 = 0;
            if (genRand.Next(2) == 0)
            {
                num9 = -1;
            }
            else
            {
                num9 = 1;
            }
            for (int i = 0; i < Main.maxTilesX; i++)
            {
                float num11 = ((float) i) / ((float) Main.maxTilesX);
                Main.statusText = "Generating world terrain: " + ((int) ((num11 * 100f) + 1f)) + "%";
                if (num < num2)
                {
                    num2 = num;
                }
                if (num > num3)
                {
                    num3 = num;
                }
                if (num4 < num5)
                {
                    num5 = num4;
                }
                if (num4 > num6)
                {
                    num6 = num4;
                }
                if (num8 <= 0)
                {
                    num7 = genRand.Next(0, 5);
                    num8 = genRand.Next(5, 40);
                    if (num7 == 0)
                    {
                        num8 *= (int) (genRand.Next(5, 30) * 0.2);
                    }
                }
                num8--;
                switch (num7)
                {
                    case 0:
                        while (genRand.Next(0, 7) == 0)
                        {
                            num += genRand.Next(-1, 2);
                        }
                        break;

                    case 1:
                        while (genRand.Next(0, 4) == 0)
                        {
                            num--;
                        }
                        while (genRand.Next(0, 10) == 0)
                        {
                            num++;
                        }
                        break;

                    case 2:
                        while (genRand.Next(0, 4) == 0)
                        {
                            num++;
                        }
                        while (genRand.Next(0, 10) == 0)
                        {
                            num--;
                        }
                        break;

                    case 3:
                        while (genRand.Next(0, 2) == 0)
                        {
                            num--;
                        }
                        while (genRand.Next(0, 6) == 0)
                        {
                            num++;
                        }
                        break;

                    case 4:
                        while (genRand.Next(0, 2) == 0)
                        {
                            num++;
                        }
                        while (genRand.Next(0, 5) == 0)
                        {
                            num--;
                        }
                        break;
                }
                if (num < (Main.maxTilesY * 0.15))
                {
                    num = Main.maxTilesY * 0.15;
                    num8 = 0;
                }
                else if (num > (Main.maxTilesY * 0.3))
                {
                    num = Main.maxTilesY * 0.3;
                    num8 = 0;
                }
                while (genRand.Next(0, 3) == 0)
                {
                    num4 += genRand.Next(-2, 3);
                }
                if (num4 < (num + (Main.maxTilesY * 0.05)))
                {
                    num4++;
                }
                if (num4 > (num + (Main.maxTilesY * 0.35)))
                {
                    num4--;
                }
                for (int num12 = 0; num12 < num; num12++)
                {
                    Main.tile[i, num12].active = false;
                    Main.tile[i, num12].lighted = true;
                    Main.tile[i, num12].frameX = -1;
                    Main.tile[i, num12].frameY = -1;
                }
                for (int num13 = (int) num; num13 < Main.maxTilesY; num13++)
                {
                    if (num13 < num4)
                    {
                        Main.tile[i, num13].active = true;
                        Main.tile[i, num13].type = 0;
                        Main.tile[i, num13].frameX = -1;
                        Main.tile[i, num13].frameY = -1;
                    }
                    else
                    {
                        Main.tile[i, num13].active = true;
                        Main.tile[i, num13].type = 1;
                        Main.tile[i, num13].frameX = -1;
                        Main.tile[i, num13].frameY = -1;
                    }
                }
            }
            Main.worldSurface = num3 + 5.0;
            Main.rockLayer = num6;
            double num14 = ((int) ((Main.rockLayer - Main.worldSurface) / 6.0)) * 6;
            Main.rockLayer = Main.worldSurface + num14;
            waterLine = (((int) Main.rockLayer) + Main.maxTilesY) / 2;
            waterLine += genRand.Next(-100, 20);
            lavaLine = waterLine + genRand.Next(50, 80);
            int num15 = 0;
            Main.statusText = "Adding sand...";
            int num16 = genRand.Next((int) (Main.maxTilesX * 0.0007), (int) (Main.maxTilesX * 0.002)) + 2;
            for (int j = 0; j < num16; j++)
            {
                int num18 = genRand.Next(Main.maxTilesX);
                while ((num18 > (Main.maxTilesX * 0.45f)) && (num18 < (Main.maxTilesX * 0.55f)))
                {
                    num18 = genRand.Next(Main.maxTilesX);
                }
                int num19 = genRand.Next(15, 90);
                if (genRand.Next(3) == 0)
                {
                    num19 *= 2;
                }
                int num20 = num18 - num19;
                num19 = genRand.Next(15, 90);
                if (genRand.Next(3) == 0)
                {
                    num19 *= 2;
                }
                int maxTilesX = num18 + num19;
                if (num20 < 0)
                {
                    num20 = 0;
                }
                if (maxTilesX > Main.maxTilesX)
                {
                    maxTilesX = Main.maxTilesX;
                }
                switch (j)
                {
                    case 0:
                        num20 = 0;
                        maxTilesX = genRand.Next(250, 300);
                        break;

                    case 2:
                        num20 = Main.maxTilesX - genRand.Next(250, 300);
                        maxTilesX = Main.maxTilesX;
                        break;
                }
                int num22 = genRand.Next(50, 100);
                for (int num23 = num20; num23 < maxTilesX; num23++)
                {
                    if (genRand.Next(2) == 0)
                    {
                        num22 += genRand.Next(-1, 2);
                        if (num22 < 50)
                        {
                            num22 = 50;
                        }
                        if (num22 > 100)
                        {
                            num22 = 100;
                        }
                    }
                    for (int num24 = 0; num24 < Main.worldSurface; num24++)
                    {
                        if (Main.tile[num23, num24].active)
                        {
                            int num25 = num22;
                            if ((num23 - num20) < num25)
                            {
                                num25 = num23 - num20;
                            }
                            if ((maxTilesX - num23) < num25)
                            {
                                num25 = maxTilesX - num23;
                            }
                            num25 += genRand.Next(5);
                            for (int num26 = num24; num26 < (num24 + num25); num26++)
                            {
                                if ((num23 > (num20 + genRand.Next(5))) && (num23 < (maxTilesX - genRand.Next(5))))
                                {
                                    Main.tile[num23, num26].type = 0x35;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            for (int k = 0; k < ((int) ((Main.maxTilesX * Main.maxTilesY) * 8E-06)); k++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next((int) Main.worldSurface, (int) Main.rockLayer), (double) genRand.Next(15, 70), genRand.Next(20, 130), 0x35, false, 0f, 0f, false, true);
            }
            numMCaves = 0;
            Main.statusText = "Generating hills...";
            for (int m = 0; m < ((int) (Main.maxTilesX * 0.0008)); m++)
            {
                int num29 = 0;
                bool flag = false;
                bool flag2 = false;
                int num30 = genRand.Next((int) (Main.maxTilesX * 0.25), (int) (Main.maxTilesX * 0.75));
                while (!flag2)
                {
                    flag2 = true;
                    while ((num30 > ((Main.maxTilesX / 2) - 100)) && (num30 < ((Main.maxTilesX / 2) + 100)))
                    {
                        num30 = genRand.Next((int) (Main.maxTilesX * 0.25), (int) (Main.maxTilesX * 0.75));
                    }
                    for (int num31 = 0; num31 < numMCaves; num31++)
                    {
                        if ((num30 > (mCaveX[num31] - 50)) && (num30 < (mCaveX[num31] + 50)))
                        {
                            num29++;
                            flag2 = false;
                            break;
                        }
                    }
                    if (num29 >= 200)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    for (int num32 = 0; num32 < Main.worldSurface; num32++)
                    {
                        if (Main.tile[num30, num32].active)
                        {
                            Mountinater(num30, num32);
                            mCaveX[numMCaves] = num30;
                            mCaveY[numMCaves] = num32;
                            numMCaves++;
                            break;
                        }
                    }
                }
            }
            for (int n = 1; n < (Main.maxTilesX - 1); n++)
            {
                float num34 = ((float) n) / ((float) Main.maxTilesX);
                Main.statusText = "Puttin dirt behind dirt: " + ((int) ((num34 * 100f) + 1f)) + "%";
                bool flag3 = false;
                num15 += genRand.Next(-1, 2);
                if (num15 < 0)
                {
                    num15 = 0;
                }
                if (num15 > 10)
                {
                    num15 = 10;
                }
                for (int num35 = 0; num35 < (Main.worldSurface + 10.0); num35++)
                {
                    if (num35 > (Main.worldSurface + num15))
                    {
                        break;
                    }
                    if (flag3)
                    {
                        Main.tile[n, num35].wall = 2;
                    }
                    if (((Main.tile[n, num35].active && Main.tile[n - 1, num35].active) && (Main.tile[n + 1, num35].active && Main.tile[n, num35 + 1].active)) && (Main.tile[n - 1, num35 + 1].active && Main.tile[n + 1, num35 + 1].active))
                    {
                        flag3 = true;
                    }
                }
            }
            numIslandHouses = 0;
            houseCount = 0;
            Main.statusText = "Generating floating islands...";
            for (int num36 = 0; num36 < ((int) (Main.maxTilesX * 0.0008)); num36++)
            {
                int num37 = 0;
                bool flag4 = false;
                int num38 = genRand.Next((int) (Main.maxTilesX * 0.1), (int) (Main.maxTilesX * 0.9));
                bool flag5 = false;
                while (!flag5)
                {
                    flag5 = true;
                    while ((num38 > ((Main.maxTilesX / 2) - 80)) && (num38 < ((Main.maxTilesX / 2) + 80)))
                    {
                        num38 = genRand.Next((int) (Main.maxTilesX * 0.1), (int) (Main.maxTilesX * 0.9));
                    }
                    for (int num39 = 0; num39 < numIslandHouses; num39++)
                    {
                        if ((num38 > (fihX[num39] - 80)) && (num38 < (fihX[num39] + 80)))
                        {
                            num37++;
                            flag5 = false;
                            break;
                        }
                    }
                    if (num37 >= 200)
                    {
                        flag4 = true;
                        break;
                    }
                }
                if (!flag4)
                {
                    for (int num40 = 200; num40 < Main.worldSurface; num40++)
                    {
                        if (Main.tile[num38, num40].active)
                        {
                            int num41 = num38;
                            int num42 = genRand.Next(100, num40 - 100);
                            while (num42 > (num2 - 50.0))
                            {
                                num42--;
                            }
                            FloatingIsland(num41, num42);
                            fihX[numIslandHouses] = num41;
                            fihY[numIslandHouses] = num42;
                            numIslandHouses++;
                            break;
                        }
                    }
                }
            }
            Main.statusText = "Placing rocks in the dirt...";
            for (int num43 = 0; num43 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 0.0002)); num43++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next(0, ((int) num2) + 1), (double) genRand.Next(4, 15), genRand.Next(5, 40), 1, false, 0f, 0f, false, true);
            }
            for (int num44 = 0; num44 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 0.0002)); num44++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next((int) num2, ((int) num3) + 1), (double) genRand.Next(4, 10), genRand.Next(5, 30), 1, false, 0f, 0f, false, true);
            }
            for (int num45 = 0; num45 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 0.0045)); num45++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next((int) num3, ((int) num6) + 1), (double) genRand.Next(2, 7), genRand.Next(2, 0x17), 1, false, 0f, 0f, false, true);
            }
            Main.statusText = "Placing dirt in the rocks...";
            for (int num46 = 0; num46 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 0.005)); num46++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next((int) num5, Main.maxTilesY), (double) genRand.Next(2, 6), genRand.Next(2, 40), 0, false, 0f, 0f, false, true);
            }
            Main.statusText = "Adding clay...";
            for (int num47 = 0; num47 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 2E-05)); num47++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next(0, (int) num2), (double) genRand.Next(4, 14), genRand.Next(10, 50), 40, false, 0f, 0f, false, true);
            }
            for (int num48 = 0; num48 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 5E-05)); num48++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next((int) num2, ((int) num3) + 1), (double) genRand.Next(8, 14), genRand.Next(15, 0x2d), 40, false, 0f, 0f, false, true);
            }
            for (int num49 = 0; num49 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 2E-05)); num49++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next((int) num3, ((int) num6) + 1), (double) genRand.Next(8, 15), genRand.Next(5, 50), 40, false, 0f, 0f, false, true);
            }
            for (int num50 = 5; num50 < (Main.maxTilesX - 5); num50++)
            {
                for (int num51 = 1; num51 < (Main.worldSurface - 1.0); num51++)
                {
                    if (Main.tile[num50, num51].active)
                    {
                        for (int num52 = num51; num52 < (num51 + 5); num52++)
                        {
                            if (Main.tile[num50, num52].type == 40)
                            {
                                Main.tile[num50, num52].type = 0;
                            }
                        }
                        break;
                    }
                }
            }
            for (int num53 = 0; num53 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 0.0015)); num53++)
            {
                float num54 = (float) (((double) num53) / ((Main.maxTilesX * Main.maxTilesY) * 0.0015));
                Main.statusText = "Making random holes: " + ((int) ((num54 * 100f) + 1f)) + "%";
                int type = -1;
                if (genRand.Next(5) == 0)
                {
                    type = -2;
                }
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next((int) num3, Main.maxTilesY), (double) genRand.Next(2, 5), genRand.Next(2, 20), type, false, 0f, 0f, false, true);
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next((int) num3, Main.maxTilesY), (double) genRand.Next(8, 15), genRand.Next(7, 30), type, false, 0f, 0f, false, true);
            }
            for (int num56 = 0; num56 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 3E-05)); num56++)
            {
                float num57 = (float) (((double) num56) / ((Main.maxTilesX * Main.maxTilesY) * 3E-05));
                Main.statusText = "Generating small caves: " + ((int) ((num57 * 100f) + 1f)) + "%";
                if (num6 <= Main.maxTilesY)
                {
                    int num58 = -1;
                    if (genRand.Next(6) == 0)
                    {
                        num58 = -2;
                    }
                    TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next((int) num2, ((int) num6) + 1), (double) genRand.Next(5, 15), genRand.Next(30, 200), num58, false, 0f, 0f, false, true);
                }
            }
            for (int num59 = 0; num59 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 0.00015)); num59++)
            {
                float num60 = (float) (((double) num59) / ((Main.maxTilesX * Main.maxTilesY) * 0.00015));
                Main.statusText = "Generating large caves: " + ((int) ((num60 * 100f) + 1f)) + "%";
                if (num6 <= Main.maxTilesY)
                {
                    int num61 = -1;
                    if (genRand.Next(10) == 0)
                    {
                        num61 = -2;
                    }
                    TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next((int) num6, Main.maxTilesY), (double) genRand.Next(6, 20), genRand.Next(50, 300), num61, false, 0f, 0f, false, true);
                }
            }
            int num62 = 0;
            Main.statusText = "Generating surface caves...";
            for (int num63 = 0; num63 < ((int) (Main.maxTilesX * 0.0025)); num63++)
            {
                num62 = genRand.Next(0, Main.maxTilesX);
                for (int num64 = 0; num64 < num3; num64++)
                {
                    if (Main.tile[num62, num64].active)
                    {
                        TileRunner(num62, num64, (double) genRand.Next(3, 6), genRand.Next(5, 50), -1, false, genRand.Next(-10, 11) * 0.1f, 1f, false, true);
                        break;
                    }
                }
            }
            for (int num65 = 0; num65 < ((int) (Main.maxTilesX * 0.0007)); num65++)
            {
                num62 = genRand.Next(0, Main.maxTilesX);
                for (int num66 = 0; num66 < num3; num66++)
                {
                    if (Main.tile[num62, num66].active)
                    {
                        TileRunner(num62, num66, (double) genRand.Next(10, 15), genRand.Next(50, 130), -1, false, genRand.Next(-10, 11) * 0.1f, 2f, false, true);
                        break;
                    }
                }
            }
            for (int num67 = 0; num67 < ((int) (Main.maxTilesX * 0.0003)); num67++)
            {
                num62 = genRand.Next(0, Main.maxTilesX);
                for (int num68 = 0; num68 < num3; num68++)
                {
                    if (Main.tile[num62, num68].active)
                    {
                        TileRunner(num62, num68, (double) genRand.Next(12, 0x19), genRand.Next(150, 500), -1, false, genRand.Next(-10, 11) * 0.1f, 4f, false, true);
                        TileRunner(num62, num68, (double) genRand.Next(8, 0x11), genRand.Next(60, 200), -1, false, genRand.Next(-10, 11) * 0.1f, 2f, false, true);
                        TileRunner(num62, num68, (double) genRand.Next(5, 13), genRand.Next(40, 170), -1, false, genRand.Next(-10, 11) * 0.1f, 2f, false, true);
                        break;
                    }
                }
            }
            for (int num69 = 0; num69 < ((int) (Main.maxTilesX * 0.0004)); num69++)
            {
                num62 = genRand.Next(0, Main.maxTilesX);
                for (int num70 = 0; num70 < num3; num70++)
                {
                    if (Main.tile[num62, num70].active)
                    {
                        TileRunner(num62, num70, (double) genRand.Next(7, 12), genRand.Next(150, 250), -1, false, 0f, 1f, true, true);
                        break;
                    }
                }
            }
            for (int num73 = 0; num73 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 0.002)); num73++)
            {
                int num71 = genRand.Next(1, Main.maxTilesX - 1);
                int num72 = genRand.Next((int) num2, (int) num3);
                if (num72 >= Main.maxTilesY)
                {
                    num72 = Main.maxTilesY - 2;
                }
                if (((Main.tile[num71 - 1, num72].active && (Main.tile[num71 - 1, num72].type == 0)) && (Main.tile[num71 + 1, num72].active && (Main.tile[num71 + 1, num72].type == 0))) && ((Main.tile[num71, num72 - 1].active && (Main.tile[num71, num72 - 1].type == 0)) && (Main.tile[num71, num72 + 1].active && (Main.tile[num71, num72 + 1].type == 0))))
                {
                    Main.tile[num71, num72].active = true;
                    Main.tile[num71, num72].type = 2;
                }
                num71 = genRand.Next(1, Main.maxTilesX - 1);
                num72 = genRand.Next(0, (int) num2);
                if (num72 >= Main.maxTilesY)
                {
                    num72 = Main.maxTilesY - 2;
                }
                if (((Main.tile[num71 - 1, num72].active && (Main.tile[num71 - 1, num72].type == 0)) && (Main.tile[num71 + 1, num72].active && (Main.tile[num71 + 1, num72].type == 0))) && ((Main.tile[num71, num72 - 1].active && (Main.tile[num71, num72 - 1].type == 0)) && (Main.tile[num71, num72 + 1].active && (Main.tile[num71, num72 + 1].type == 0))))
                {
                    Main.tile[num71, num72].active = true;
                    Main.tile[num71, num72].type = 2;
                }
            }
            Main.statusText = "Generating underground jungle: 0%";
            float num74 = Main.maxTilesX / 0x1068;
            num74 *= 1.5f;
            int num75 = 0;
            if (num9 == -1)
            {
                num75 = (int) (Main.maxTilesX * 0.8f);
            }
            else
            {
                num75 = (int) (Main.maxTilesX * 0.2f);
            }
            int num76 = (Main.maxTilesY + ((int) Main.rockLayer)) / 2;
            num75 += genRand.Next((int) (-100f * num74), (int) (101f * num74));
            num76 += genRand.Next((int) (-100f * num74), (int) (101f * num74));
            TileRunner(num75, num76, (double) genRand.Next((int) (250f * num74), (int) (500f * num74)), genRand.Next(50, 150), 0x3b, false, (float) (num9 * 3), 0f, false, true);
            Main.statusText = "Generating underground jungle: 20%";
            num75 += genRand.Next((int) (-250f * num74), (int) (251f * num74));
            num76 += genRand.Next((int) (-150f * num74), (int) (151f * num74));
            int num77 = num75;
            int num78 = num76;
            TileRunner(num75, num76, (double) genRand.Next((int) (250f * num74), (int) (500f * num74)), genRand.Next(50, 150), 0x3b, false, 0f, 0f, false, true);
            Main.statusText = "Generating underground jungle: 40%";
            num75 += genRand.Next((int) (-400f * num74), (int) (401f * num74));
            num76 += genRand.Next((int) (-150f * num74), (int) (151f * num74));
            TileRunner(num75, num76, (double) genRand.Next((int) (250f * num74), (int) (500f * num74)), genRand.Next(50, 150), 0x3b, false, (float) (num9 * -3), 0f, false, true);
            Main.statusText = "Generating underground jungle: 60%";
            num75 = num77;
            num76 = num78;
            for (int num79 = 0; num79 <= (20f * num74); num79++)
            {
                Main.statusText = "Generating underground jungle: " + ((int) (60f + (((float) num79) / num74))) + "%";
                num75 += genRand.Next((int) (-5f * num74), (int) (6f * num74));
                num76 += genRand.Next((int) (-5f * num74), (int) (6f * num74));
                TileRunner(num75, num76, (double) genRand.Next(40, 100), genRand.Next(300, 500), 0x3b, false, 0f, 0f, false, true);
            }
            for (int num80 = 0; num80 <= (10f * num74); num80++)
            {
                Main.statusText = "Generating underground jungle: " + ((int) (80f + ((((float) num80) / num74) * 2f))) + "%";
                num75 = num77 + genRand.Next((int) (-600f * num74), (int) (600f * num74));
                num76 = num78 + genRand.Next((int) (-200f * num74), (int) (200f * num74));
                while ((((num75 < 1) || (num75 >= (Main.maxTilesX - 1))) || ((num76 < 1) || (num76 >= (Main.maxTilesY - 1)))) || (Main.tile[num75, num76].type != 0x3b))
                {
                    num75 = num77 + genRand.Next((int) (-600f * num74), (int) (600f * num74));
                    num76 = num78 + genRand.Next((int) (-200f * num74), (int) (200f * num74));
                }
                for (int num81 = 0; num81 < (8f * num74); num81++)
                {
                    num75 += genRand.Next(-30, 0x1f);
                    num76 += genRand.Next(-30, 0x1f);
                    int num82 = -1;
                    if (genRand.Next(7) == 0)
                    {
                        num82 = -2;
                    }
                    TileRunner(num75, num76, (double) genRand.Next(10, 20), genRand.Next(30, 70), num82, false, 0f, 0f, false, true);
                }
            }
            for (int num83 = 0; num83 <= (300f * num74); num83++)
            {
                num75 = num77 + genRand.Next((int) (-600f * num74), (int) (600f * num74));
                num76 = num78 + genRand.Next((int) (-200f * num74), (int) (200f * num74));
                while ((((num75 < 1) || (num75 >= (Main.maxTilesX - 1))) || ((num76 < 1) || (num76 >= (Main.maxTilesY - 1)))) || (Main.tile[num75, num76].type != 0x3b))
                {
                    num75 = num77 + genRand.Next((int) (-600f * num74), (int) (600f * num74));
                    num76 = num78 + genRand.Next((int) (-200f * num74), (int) (200f * num74));
                }
                TileRunner(num75, num76, (double) genRand.Next(4, 10), genRand.Next(5, 30), 1, false, 0f, 0f, false, true);
                if (genRand.Next(4) == 0)
                {
                    int num84 = genRand.Next(0x3f, 0x45);
                    TileRunner(num75 + genRand.Next(-1, 2), num76 + genRand.Next(-1, 2), (double) genRand.Next(3, 7), genRand.Next(4, 8), num84, false, 0f, 0f, false, true);
                }
            }
            num75 = num77;
            num76 = num78;
            float num85 = genRand.Next(6, 10);
            float num86 = Main.maxTilesX / 0x1068;
            num85 *= num86;
            for (int num87 = 0; num87 < num85; num87++)
            {
                bool flag6 = true;
                while (flag6)
                {
                    num75 = genRand.Next(20, Main.maxTilesX - 20);
                    num76 = genRand.Next(20, Main.maxTilesY - 300);
                    if (Main.tile[num75, num76].type == 0x3b)
                    {
                        flag6 = false;
                        int num88 = genRand.Next(2, 4);
                        int num89 = genRand.Next(2, 4);
                        for (int num90 = (num75 - num88) - 1; num90 <= ((num75 + num88) + 1); num90++)
                        {
                            for (int num91 = (num76 - num89) - 1; num91 <= ((num76 + num89) + 1); num91++)
                            {
                                Main.tile[num90, num91].active = true;
                                Main.tile[num90, num91].type = 0x2d;
                                Main.tile[num90, num91].liquid = 0;
                                Main.tile[num90, num91].lava = false;
                            }
                        }
                        for (int num92 = num75 - num88; num92 <= (num75 + num88); num92++)
                        {
                            for (int num93 = num76 - num89; num93 <= (num76 + num89); num93++)
                            {
                                Main.tile[num92, num93].active = false;
                                Main.tile[num92, num93].wall = 10;
                            }
                        }
                        bool flag7 = false;
                        int num94 = 0;
                        while (!flag7 && (num94 < 100))
                        {
                            num94++;
                            int num95 = genRand.Next(num75 - num88, (num75 + num88) + 1);
                            int num96 = genRand.Next(num76 - num89, (num76 + num89) - 2);
                            PlaceTile(num95, num96, 4, true, false, -1);
                            if (Main.tile[num95, num96].type == 4)
                            {
                                flag7 = true;
                            }
                        }
                        for (int num97 = (num75 - num88) - 1; num97 <= ((num75 + num88) + 1); num97++)
                        {
                            for (int num98 = (num76 + num89) - 2; num98 <= (num76 + num89); num98++)
                            {
                                Main.tile[num97, num98].active = false;
                            }
                        }
                        for (int num99 = (num75 - num88) - 1; num99 <= ((num75 + num88) + 1); num99++)
                        {
                            for (int num100 = (num76 + num89) - 2; num100 <= ((num76 + num89) - 1); num100++)
                            {
                                Main.tile[num99, num100].active = false;
                            }
                        }
                        for (int num101 = (num75 - num88) - 1; num101 <= ((num75 + num88) + 1); num101++)
                        {
                            int num102 = 4;
                            int num103 = (num76 + num89) + 2;
                            while ((!Main.tile[num101, num103].active && (num103 < Main.maxTilesY)) && (num102 > 0))
                            {
                                Main.tile[num101, num103].active = true;
                                Main.tile[num101, num103].type = 0x3b;
                                num103++;
                                num102--;
                            }
                        }
                        num88 -= genRand.Next(1, 3);
                        for (int num104 = (num76 - num89) - 2; num88 > -1; num104--)
                        {
                            for (int num105 = (num75 - num88) - 1; num105 <= ((num75 + num88) + 1); num105++)
                            {
                                Main.tile[num105, num104].active = true;
                                Main.tile[num105, num104].type = 0x2d;
                            }
                            num88 -= genRand.Next(1, 3);
                        }
                        JChestX[numJChests] = num75;
                        JChestY[numJChests] = num76;
                        numJChests++;
                    }
                }
            }
            for (int num106 = 0; num106 < Main.maxTilesX; num106++)
            {
                for (int num107 = (int) Main.worldSurface; num107 < Main.maxTilesY; num107++)
                {
                    if (Main.tile[num106, num107].active)
                    {
                        SpreadGrass(num106, num107, 0x3b, 60, false);
                    }
                }
            }
            Main.statusText = "Adding mushroom patches...";
            for (int num108 = 0; num108 < (Main.maxTilesX / 300); num108++)
            {
                int num109 = genRand.Next((int) (Main.maxTilesX * 0.3), (int) (Main.maxTilesX * 0.7));
                int num110 = genRand.Next((int) Main.rockLayer, Main.maxTilesY - 300);
                ShroomPatch(num109, num110);
            }
            for (int num111 = 0; num111 < Main.maxTilesX; num111++)
            {
                for (int num112 = (int) Main.worldSurface; num112 < Main.maxTilesY; num112++)
                {
                    if (Main.tile[num111, num112].active)
                    {
                        SpreadGrass(num111, num112, 0x3b, 70, false);
                    }
                }
            }
            Main.statusText = "Placing mud in the dirt...";
            for (int num113 = 0; num113 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 0.001)); num113++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next((int) num5, Main.maxTilesY), (double) genRand.Next(2, 6), genRand.Next(2, 40), 0x3b, false, 0f, 0f, false, true);
            }
            Main.statusText = "Adding shinies...";
            for (int num114 = 0; num114 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 6E-05)); num114++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next((int) num2, (int) num3), (double) genRand.Next(3, 6), genRand.Next(2, 6), 7, false, 0f, 0f, false, true);
            }
            for (int num115 = 0; num115 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 8E-05)); num115++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next((int) num3, (int) num6), (double) genRand.Next(3, 7), genRand.Next(3, 7), 7, false, 0f, 0f, false, true);
            }
            for (int num116 = 0; num116 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 0.0002)); num116++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next((int) num5, Main.maxTilesY), (double) genRand.Next(4, 9), genRand.Next(4, 8), 7, false, 0f, 0f, false, true);
            }
            for (int num117 = 0; num117 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 3E-05)); num117++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next((int) num2, (int) num3), (double) genRand.Next(3, 7), genRand.Next(2, 5), 6, false, 0f, 0f, false, true);
            }
            for (int num118 = 0; num118 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 8E-05)); num118++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next((int) num3, (int) num6), (double) genRand.Next(3, 6), genRand.Next(3, 6), 6, false, 0f, 0f, false, true);
            }
            for (int num119 = 0; num119 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 0.0002)); num119++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next((int) num5, Main.maxTilesY), (double) genRand.Next(4, 9), genRand.Next(4, 8), 6, false, 0f, 0f, false, true);
            }
            for (int num120 = 0; num120 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 3E-05)); num120++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next((int) num3, (int) num6), (double) genRand.Next(3, 6), genRand.Next(3, 6), 9, false, 0f, 0f, false, true);
            }
            for (int num121 = 0; num121 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 0.00017)); num121++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next((int) num5, Main.maxTilesY), (double) genRand.Next(4, 9), genRand.Next(4, 8), 9, false, 0f, 0f, false, true);
            }
            for (int num122 = 0; num122 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 0.00017)); num122++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next(0, (int) num2), (double) genRand.Next(4, 9), genRand.Next(4, 8), 9, false, 0f, 0f, false, true);
            }
            for (int num123 = 0; num123 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 0.00012)); num123++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next((int) num5, Main.maxTilesY), (double) genRand.Next(4, 8), genRand.Next(4, 8), 8, false, 0f, 0f, false, true);
            }
            for (int num124 = 0; num124 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 0.00012)); num124++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next(0, ((int) num2) - 20), (double) genRand.Next(4, 8), genRand.Next(4, 8), 8, false, 0f, 0f, false, true);
            }
            Main.statusText = "Adding webs...";
            for (int num125 = 0; num125 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 0.001)); num125++)
            {
                int num126 = genRand.Next(20, Main.maxTilesX - 20);
                int num127 = genRand.Next((int) num2, Main.maxTilesY - 20);
                if (num125 < numMCaves)
                {
                    num126 = mCaveX[num125];
                    num127 = mCaveY[num125];
                }
                if (!Main.tile[num126, num127].active && ((num127 > Main.worldSurface) || (Main.tile[num126, num127].wall > 0)))
                {
                    while (!Main.tile[num126, num127].active && (num127 > ((int) num2)))
                    {
                        num127--;
                    }
                    num127++;
                    int num128 = 1;
                    if (genRand.Next(2) == 0)
                    {
                        num128 = -1;
                    }
                    while ((!Main.tile[num126, num127].active && (num126 > 10)) && (num126 < (Main.maxTilesX - 10)))
                    {
                        num126 += num128;
                    }
                    num126 -= num128;
                    if ((num127 > Main.worldSurface) || (Main.tile[num126, num127].wall > 0))
                    {
                        TileRunner(num126, num127, (double) genRand.Next(4, 13), genRand.Next(2, 5), 0x33, true, (float) num128, -1f, false, false);
                    }
                }
            }
            Main.statusText = "Creating underworld: 0%";
            int num129 = Main.maxTilesY - genRand.Next(150, 190);
            for (int num130 = 0; num130 < Main.maxTilesX; num130++)
            {
                num129 += genRand.Next(-3, 4);
                if (num129 < (Main.maxTilesY - 190))
                {
                    num129 = Main.maxTilesY - 190;
                }
                if (num129 > (Main.maxTilesY - 160))
                {
                    num129 = Main.maxTilesY - 160;
                }
                for (int num131 = (num129 - 20) - genRand.Next(3); num131 < Main.maxTilesY; num131++)
                {
                    if (num131 >= num129)
                    {
                        Main.tile[num130, num131].active = false;
                        Main.tile[num130, num131].lava = false;
                        Main.tile[num130, num131].liquid = 0;
                    }
                    else
                    {
                        Main.tile[num130, num131].type = 0x39;
                    }
                }
            }
            int num132 = Main.maxTilesY - genRand.Next(40, 70);
            for (int num133 = 10; num133 < (Main.maxTilesX - 10); num133++)
            {
                num132 += genRand.Next(-10, 11);
                if (num132 > (Main.maxTilesY - 60))
                {
                    num132 = Main.maxTilesY - 60;
                }
                if (num132 < (Main.maxTilesY - 100))
                {
                    num132 = Main.maxTilesY - 120;
                }
                for (int num134 = num132; num134 < (Main.maxTilesY - 10); num134++)
                {
                    if (!Main.tile[num133, num134].active)
                    {
                        Main.tile[num133, num134].lava = true;
                        Main.tile[num133, num134].liquid = 0xff;
                    }
                }
            }
            for (int num135 = 0; num135 < Main.maxTilesX; num135++)
            {
                if (genRand.Next(50) == 0)
                {
                    int num136 = Main.maxTilesY - 0x41;
                    while (!Main.tile[num135, num136].active && (num136 > (Main.maxTilesY - 0x87)))
                    {
                        num136--;
                    }
                    TileRunner(genRand.Next(0, Main.maxTilesX), num136 + genRand.Next(20, 50), (double) genRand.Next(15, 20), 0x3e8, 0x39, true, 0f, (float) genRand.Next(1, 3), true, true);
                }
            }
            Liquid.QuickWater(-2, -1, -1);
            for (int num137 = 0; num137 < Main.maxTilesX; num137++)
            {
                float num138 = ((float) num137) / ((float) (Main.maxTilesX - 1));
                Main.statusText = "Creating underworld: " + ((int) (((num138 * 100f) / 2f) + 50f)) + "%";
                if (genRand.Next(13) == 0)
                {
                    int num139 = Main.maxTilesY - 0x41;
                    while (((Main.tile[num137, num139].liquid > 0) || Main.tile[num137, num139].active) && (num139 > (Main.maxTilesY - 140)))
                    {
                        num139--;
                    }
                    TileRunner(num137, num139 - genRand.Next(2, 5), (double) genRand.Next(5, 30), 0x3e8, 0x39, true, 0f, (float) genRand.Next(1, 3), true, true);
                    float num140 = genRand.Next(1, 3);
                    if (genRand.Next(3) == 0)
                    {
                        num140 *= 0.5f;
                    }
                    if (genRand.Next(2) == 0)
                    {
                        TileRunner(num137, num139 - genRand.Next(2, 5), (double) ((int) (genRand.Next(5, 15) * num140)), (int) (genRand.Next(10, 15) * num140), 0x39, true, 1f, 0.3f, false, true);
                    }
                    if (genRand.Next(2) == 0)
                    {
                        num140 = genRand.Next(1, 3);
                        TileRunner(num137, num139 - genRand.Next(2, 5), (double) ((int) (genRand.Next(5, 15) * num140)), (int) (genRand.Next(10, 15) * num140), 0x39, true, -1f, 0.3f, false, true);
                    }
                    TileRunner(num137 + genRand.Next(-10, 10), num139 + genRand.Next(-10, 10), (double) genRand.Next(5, 15), genRand.Next(5, 10), -2, false, (float) genRand.Next(-1, 3), (float) genRand.Next(-1, 3), false, true);
                    if (genRand.Next(3) == 0)
                    {
                        TileRunner(num137 + genRand.Next(-10, 10), num139 + genRand.Next(-10, 10), (double) genRand.Next(10, 30), genRand.Next(10, 20), -2, false, (float) genRand.Next(-1, 3), (float) genRand.Next(-1, 3), false, true);
                    }
                    if (genRand.Next(5) == 0)
                    {
                        TileRunner(num137 + genRand.Next(-15, 15), num139 + genRand.Next(-15, 10), (double) genRand.Next(15, 30), genRand.Next(5, 20), -2, false, (float) genRand.Next(-1, 3), (float) genRand.Next(-1, 3), false, true);
                    }
                }
            }
            for (int num141 = 0; num141 < Main.maxTilesX; num141++)
            {
                if (!Main.tile[num141, Main.maxTilesY - 0x91].active)
                {
                    Main.tile[num141, Main.maxTilesY - 0x91].liquid = 0xff;
                    Main.tile[num141, Main.maxTilesY - 0x91].lava = true;
                }
                if (!Main.tile[num141, Main.maxTilesY - 0x90].active)
                {
                    Main.tile[num141, Main.maxTilesY - 0x90].liquid = 0xff;
                    Main.tile[num141, Main.maxTilesY - 0x90].lava = true;
                }
            }
            for (int num142 = 0; num142 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 0.002)); num142++)
            {
                TileRunner(genRand.Next(0, Main.maxTilesX), genRand.Next(Main.maxTilesY - 140, Main.maxTilesY), (double) genRand.Next(3, 8), genRand.Next(3, 8), 0x3a, false, 0f, 0f, false, true);
            }
            AddHellHouses();
            int num143 = genRand.Next(2, (int) (Main.maxTilesX * 0.005));
            for (int num144 = 0; num144 < num143; num144++)
            {
                float num145 = ((float) num144) / ((float) num143);
                Main.statusText = "Adding water bodies: " + ((int) (num145 * 100f)) + "%";
                int num146 = genRand.Next(300, Main.maxTilesX - 300);
                while ((num146 > ((Main.maxTilesX / 2) - 50)) && (num146 < ((Main.maxTilesX / 2) + 50)))
                {
                    num146 = genRand.Next(300, Main.maxTilesX - 300);
                }
                int num147 = ((int) num2) - 20;
                while (!Main.tile[num146, num147].active)
                {
                    num147++;
                }
                Lakinater(num146, num147);
            }
            int x = 0;
            if (num9 == -1)
            {
                x = genRand.Next((int) (Main.maxTilesX * 0.05), (int) (Main.maxTilesX * 0.2));
                num9 = -1;
            }
            else
            {
                x = genRand.Next((int) (Main.maxTilesX * 0.8), (int) (Main.maxTilesX * 0.95));
                num9 = 1;
            }
            int y = ((int) ((Main.rockLayer + Main.maxTilesY) / 2.0)) + genRand.Next(-200, 200);
            MakeDungeon(x, y, 0x29, 7);
            for (int num150 = 0; num150 < (Main.maxTilesX * 0.0004); num150++)
            {
                float num151 = (float) (((double) num150) / (Main.maxTilesX * 0.0004));
                Main.statusText = "Making the world evil: " + ((int) (num151 * 100f)) + "%";
                bool flag8 = false;
                int num152 = 0;
                int num153 = 0;
                int num154 = 0;
                while (!flag8)
                {
                    flag8 = true;
                    int num155 = Main.maxTilesX / 2;
                    int num156 = 200;
                    num152 = genRand.Next(Main.maxTilesX);
                    num153 = (num152 - genRand.Next(150)) - 0xaf;
                    num154 = (num152 + genRand.Next(150)) + 0xaf;
                    if (num153 < 0)
                    {
                        num153 = 0;
                    }
                    if (num154 > Main.maxTilesX)
                    {
                        num154 = Main.maxTilesX;
                    }
                    if ((num152 > (num155 - num156)) && (num152 < (num155 + num156)))
                    {
                        flag8 = false;
                    }
                    if ((num153 > (num155 - num156)) && (num153 < (num155 + num156)))
                    {
                        flag8 = false;
                    }
                    if ((num154 > (num155 - num156)) && (num154 < (num155 + num156)))
                    {
                        flag8 = false;
                    }
                    for (int num157 = num153; num157 < num154; num157++)
                    {
                        for (int num158 = 0; num158 < ((int) Main.worldSurface); num158 += 5)
                        {
                            if (Main.tile[num157, num158].active && Main.tileDungeon[Main.tile[num157, num158].type])
                            {
                                flag8 = false;
                                break;
                            }
                            if (!flag8)
                            {
                                break;
                            }
                        }
                    }
                }
                int num159 = 0;
                for (int num160 = num153; num160 < num154; num160++)
                {
                    if (num159 > 0)
                    {
                        num159--;
                    }
                    if ((num160 == num152) || (num159 == 0))
                    {
                        for (int num161 = (int) num2; num161 < (Main.worldSurface - 1.0); num161++)
                        {
                            if (Main.tile[num160, num161].active || (Main.tile[num160, num161].wall > 0))
                            {
                                if (num160 == num152)
                                {
                                    num159 = 20;
                                    ChasmRunner(num160, num161, genRand.Next(150) + 150, true);
                                }
                                else if ((genRand.Next(30) == 0) && (num159 == 0))
                                {
                                    num159 = 20;
                                    bool makeOrb = false;
                                    if (genRand.Next(2) == 0)
                                    {
                                        makeOrb = true;
                                    }
                                    ChasmRunner(num160, num161, genRand.Next(50) + 50, makeOrb);
                                }
                                break;
                            }
                        }
                    }
                }
                double num162 = Main.worldSurface + 40.0;
                for (int num163 = num153; num163 < num154; num163++)
                {
                    num162 += genRand.Next(-2, 3);
                    if (num162 < (Main.worldSurface + 30.0))
                    {
                        num162 = Main.worldSurface + 30.0;
                    }
                    if (num162 > (Main.worldSurface + 50.0))
                    {
                        num162 = Main.worldSurface + 50.0;
                    }
                    num62 = num163;
                    bool flag10 = false;
                    for (int num164 = (int) num2; num164 < num162; num164++)
                    {
                        if (Main.tile[num62, num164].active)
                        {
                            if (((Main.tile[num62, num164].type == 0) && (num164 < (Main.worldSurface - 1.0))) && !flag10)
                            {
                                SpreadGrass(num62, num164, 0, 0x17, true);
                            }
                            flag10 = true;
                            if (((Main.tile[num62, num164].type == 1) && (num62 >= (num153 + genRand.Next(5)))) && (num62 <= (num154 - genRand.Next(5))))
                            {
                                Main.tile[num62, num164].type = 0x19;
                            }
                            if (Main.tile[num62, num164].type == 2)
                            {
                                Main.tile[num62, num164].type = 0x17;
                            }
                        }
                    }
                }
                for (int num165 = num153; num165 < num154; num165++)
                {
                    for (int num166 = 0; num166 < (Main.maxTilesY - 50); num166++)
                    {
                        if (Main.tile[num165, num166].active && (Main.tile[num165, num166].type == 0x1f))
                        {
                            int num167 = num165 - 13;
                            int num168 = num165 + 13;
                            int num169 = num166 - 13;
                            int num170 = num166 + 13;
                            for (int num171 = num167; num171 < num168; num171++)
                            {
                                if ((num171 > 10) && (num171 < (Main.maxTilesX - 10)))
                                {
                                    for (int num172 = num169; num172 < num170; num172++)
                                    {
                                        if ((((Math.Abs((int) (num171 - num165)) + Math.Abs((int) (num172 - num166))) < (9 + genRand.Next(11))) && (genRand.Next(3) != 0)) && (Main.tile[num171, num172].type != 0x1f))
                                        {
                                            Main.tile[num171, num172].active = true;
                                            Main.tile[num171, num172].type = 0x19;
                                            if ((Math.Abs((int) (num171 - num165)) <= 1) && (Math.Abs((int) (num172 - num166)) <= 1))
                                            {
                                                Main.tile[num171, num172].active = false;
                                            }
                                        }
                                        if (((Main.tile[num171, num172].type != 0x1f) && (Math.Abs((int) (num171 - num165)) <= (2 + genRand.Next(3)))) && (Math.Abs((int) (num172 - num166)) <= (2 + genRand.Next(3))))
                                        {
                                            Main.tile[num171, num172].active = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Main.statusText = "Generating mountain caves...";
            for (int num173 = 0; num173 < numMCaves; num173++)
            {
                int num174 = mCaveX[num173];
                int num175 = mCaveY[num173];
                CaveOpenater(num174, num175);
                Cavinator(num174, num175, genRand.Next(40, 50));
            }
            Main.statusText = "Creating beaches...";
            for (int num176 = 0; num176 < 2; num176++)
            {
                int num177 = 0;
                int num178 = 0;
                if (num176 == 0)
                {
                    num177 = 0;
                    num178 = genRand.Next(0x7d, 200);
                    float num179 = 1f;
                    int num180 = 0;
                    while (!Main.tile[num178 - 1, num180].active)
                    {
                        num180++;
                    }
                    for (int num181 = num178 - 1; num181 >= num177; num181--)
                    {
                        num179 += genRand.Next(10, 20) * 0.05f;
                        for (int num182 = 0; num182 < (num180 + num179); num182++)
                        {
                            if (num182 < ((num180 + (num179 * 0.75f)) - 3f))
                            {
                                Main.tile[num181, num182].active = false;
                                if (num182 > num180)
                                {
                                    Main.tile[num181, num182].liquid = 0xff;
                                }
                                else if (num182 == num180)
                                {
                                    Main.tile[num181, num182].liquid = 0x7f;
                                }
                            }
                            else if (num182 > num180)
                            {
                                Main.tile[num181, num182].type = 0x35;
                                Main.tile[num181, num182].active = true;
                            }
                            Main.tile[num181, num182].wall = 0;
                        }
                    }
                }
                else
                {
                    num177 = Main.maxTilesX - genRand.Next(0x7d, 200);
                    num178 = Main.maxTilesX;
                    float num183 = 1f;
                    int num184 = 0;
                    while (!Main.tile[num177, num184].active)
                    {
                        num184++;
                    }
                    for (int num185 = num177; num185 < num178; num185++)
                    {
                        num183 += genRand.Next(10, 20) * 0.05f;
                        for (int num186 = 0; num186 < (num184 + num183); num186++)
                        {
                            if (num186 < ((num184 + (num183 * 0.75f)) - 3f))
                            {
                                Main.tile[num185, num186].active = false;
                                if (num186 > num184)
                                {
                                    Main.tile[num185, num186].liquid = 0xff;
                                }
                                else if (num186 == num184)
                                {
                                    Main.tile[num185, num186].liquid = 0x7f;
                                }
                            }
                            else if (num186 > num184)
                            {
                                Main.tile[num185, num186].type = 0x35;
                                Main.tile[num185, num186].active = true;
                            }
                            Main.tile[num185, num186].wall = 0;
                        }
                    }
                }
            }
            Main.statusText = "Adding gems...";
            for (int num187 = 0x3f; num187 <= 0x44; num187++)
            {
                float num188 = 0f;
                switch (num187)
                {
                    case 0x43:
                        num188 = Main.maxTilesX * 0.5f;
                        break;

                    case 0x42:
                        num188 = Main.maxTilesX * 0.45f;
                        break;

                    case 0x3f:
                        num188 = Main.maxTilesX * 0.3f;
                        break;

                    case 0x41:
                        num188 = Main.maxTilesX * 0.25f;
                        break;

                    case 0x40:
                        num188 = Main.maxTilesX * 0.1f;
                        break;

                    case 0x44:
                        num188 = Main.maxTilesX * 0.05f;
                        break;
                }
                num188 *= 0.2f;
                for (int num189 = 0; num189 < num188; num189++)
                {
                    int num190 = genRand.Next(0, Main.maxTilesX);
                    int num191 = genRand.Next((int) Main.worldSurface, Main.maxTilesY);
                    while (Main.tile[num190, num191].type != 1)
                    {
                        num190 = genRand.Next(0, Main.maxTilesX);
                        num191 = genRand.Next((int) Main.worldSurface, Main.maxTilesY);
                    }
                    TileRunner(num190, num191, (double) genRand.Next(2, 6), genRand.Next(3, 7), num187, false, 0f, 0f, false, true);
                }
            }
            for (int num192 = 0; num192 < Main.maxTilesX; num192++)
            {
                float num193 = ((float) num192) / ((float) (Main.maxTilesX - 1));
                Main.statusText = "Gravitating sand: " + ((int) (num193 * 100f)) + "%";
                for (int num194 = Main.maxTilesY - 5; num194 > 0; num194--)
                {
                    if (Main.tile[num192, num194].active && (Main.tile[num192, num194].type == 0x35))
                    {
                        for (int num195 = num194; !Main.tile[num192, num195 + 1].active && (num195 < (Main.maxTilesY - 5)); num195++)
                        {
                            Main.tile[num192, num195 + 1].active = true;
                            Main.tile[num192, num195 + 1].type = 0x35;
                        }
                    }
                }
            }
            for (int num196 = 3; num196 < (Main.maxTilesX - 3); num196++)
            {
                float num197 = ((float) num196) / ((float) Main.maxTilesX);
                Main.statusText = "Cleaning up dirt backgrounds: " + ((int) ((num197 * 100f) + 1f)) + "%";
                for (int num198 = 0; num198 < Main.worldSurface; num198++)
                {
                    if (Main.tile[num196, num198].wall == 2)
                    {
                        Main.tile[num196, num198].wall = 0;
                    }
                    if (Main.tile[num196, num198].type != 0x35)
                    {
                        if (Main.tile[num196 - 1, num198].wall == 2)
                        {
                            Main.tile[num196 - 1, num198].wall = 0;
                        }
                        if ((Main.tile[num196 - 2, num198].wall == 2) && (genRand.Next(2) == 0))
                        {
                            Main.tile[num196 - 2, num198].wall = 0;
                        }
                        if ((Main.tile[num196 - 3, num198].wall == 2) && (genRand.Next(2) == 0))
                        {
                            Main.tile[num196 - 3, num198].wall = 0;
                        }
                        if (Main.tile[num196 + 1, num198].wall == 2)
                        {
                            Main.tile[num196 + 1, num198].wall = 0;
                        }
                        if ((Main.tile[num196 + 2, num198].wall == 2) && (genRand.Next(2) == 0))
                        {
                            Main.tile[num196 + 2, num198].wall = 0;
                        }
                        if ((Main.tile[num196 + 3, num198].wall == 2) && (genRand.Next(2) == 0))
                        {
                            Main.tile[num196 + 3, num198].wall = 0;
                        }
                        if (Main.tile[num196, num198].active)
                        {
                            break;
                        }
                    }
                }
            }
            for (int num199 = 0; num199 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 2E-05)); num199++)
            {
                float num200 = (float) (((double) num199) / ((Main.maxTilesX * Main.maxTilesY) * 2E-05));
                Main.statusText = "Placing alters: " + ((int) ((num200 * 100f) + 1f)) + "%";
                bool flag11 = false;
                int num201 = 0;
                while (!flag11)
                {
                    int num202 = genRand.Next(1, Main.maxTilesX);
                    int num203 = (int) (num3 + 20.0);
                    Place3x2(num202, num203, 0x1a);
                    if (Main.tile[num202, num203].type == 0x1a)
                    {
                        flag11 = true;
                    }
                    else
                    {
                        num201++;
                        if (num201 >= 0x2710)
                        {
                            flag11 = true;
                        }
                    }
                }
            }
            Liquid.QuickWater(3, -1, -1);
            WaterCheck();
            int num204 = 0;
            Liquid.quickSettle = true;
            while (num204 < 10)
            {
                int num205 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                num204++;
                float num206 = 0f;
                while (Liquid.numLiquid > 0)
                {
                    float num207 = ((float) (num205 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer))) / ((float) num205);
                    if ((Liquid.numLiquid + LiquidBuffer.numLiquidBuffer) > num205)
                    {
                        num205 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                    }
                    if (num207 > num206)
                    {
                        num206 = num207;
                    }
                    else
                    {
                        num207 = num206;
                    }
                    if (num204 == 1)
                    {
                        Main.statusText = "Settling liquids: " + ((int) (((num207 * 100f) / 3f) + 33f)) + "%";
                    }
                    int num208 = 10;
                    if (num204 > num208)
                    {
                        num208 = num204;
                    }
                    Liquid.UpdateLiquid();
                }
                WaterCheck();
                Main.statusText = "Settling liquids: " + ((int) (((num204 * 10f) / 3f) + 66f)) + "%";
            }
            Liquid.quickSettle = false;
            for (int num209 = 0; num209 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 2.5E-05)); num209++)
            {
                float num210 = (float) (((double) num209) / ((Main.maxTilesX * Main.maxTilesY) * 2.5E-05));
                Main.statusText = "Placing life crystals: " + ((int) ((num210 * 100f) + 1f)) + "%";
                bool flag12 = false;
                int num211 = 0;
                while (!flag12)
                {
                    if (AddLifeCrystal(genRand.Next(1, Main.maxTilesX), genRand.Next((int) (num3 + 20.0), Main.maxTilesY)))
                    {
                        flag12 = true;
                    }
                    else
                    {
                        num211++;
                        if (num211 >= 0x2710)
                        {
                            flag12 = true;
                        }
                    }
                }
            }
            for (int num212 = 0; num212 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 1.8E-05)); num212++)
            {
                float num213 = (float) (((double) num212) / ((Main.maxTilesX * Main.maxTilesY) * 1.8E-05));
                Main.statusText = "Hiding treasure: " + ((int) ((num213 * 100f) + 1f)) + "%";
                bool flag13 = false;
                int num214 = 0;
                while (!flag13)
                {
                    if (AddBuriedChest(genRand.Next(1, Main.maxTilesX), genRand.Next((int) (num3 + 20.0), Main.maxTilesY), 0))
                    {
                        flag13 = true;
                    }
                    else
                    {
                        num214++;
                        if (num214 >= 0x2710)
                        {
                            flag13 = true;
                        }
                    }
                }
            }
            int num215 = 0;
            for (int num216 = 0; num216 < numJChests; num216++)
            {
                num215++;
                int contain = 0xd3;
                switch (num215)
                {
                    case 1:
                        contain = 0xd3;
                        break;

                    case 2:
                        contain = 0xd4;
                        break;

                    case 3:
                        contain = 0xd5;
                        break;
                }
                if (num215 > 3)
                {
                    num215 = 0;
                }
                if (!AddBuriedChest(JChestX[num216] + genRand.Next(2), JChestY[num216], contain))
                {
                    for (int num218 = JChestX[num216]; num218 <= (JChestX[num216] + 1); num218++)
                    {
                        for (int num219 = JChestY[num216]; num219 <= (JChestY[num216] + 1); num219++)
                        {
                            KillTile(num218, num219, false, false, false);
                        }
                    }
                    AddBuriedChest(JChestX[num216], JChestY[num216], contain);
                }
            }
            float num220 = Main.maxTilesX / 0x1068;
            int num221 = 0;
            for (int num222 = 0; num222 < (10f * num220); num222++)
            {
                int num224;
                int num225;
                int num223 = 0;
                num221++;
                if (num221 == 1)
                {
                    num223 = 0xba;
                }
                else
                {
                    num223 = 0xbb;
                    num221 = 0;
                }
                for (bool flag14 = false; !flag14; flag14 = AddBuriedChest(num224, num225, num223))
                {
                    num224 = genRand.Next(1, Main.maxTilesX);
                    for (num225 = genRand.Next(1, Main.maxTilesY - 200); (Main.tile[num224, num225].liquid < 200) || Main.tile[num224, num225].lava; num225 = genRand.Next(1, Main.maxTilesY - 200))
                    {
                        num224 = genRand.Next(1, Main.maxTilesX);
                    }
                }
            }
            for (int num226 = 0; num226 < numIslandHouses; num226++)
            {
                IslandHouse(fihX[num226], fihY[num226]);
            }
            for (int num227 = 0; num227 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 0.0008)); num227++)
            {
                float num228 = (float) (((double) num227) / ((Main.maxTilesX * Main.maxTilesY) * 0.0008));
                Main.statusText = "Placing breakables: " + ((int) ((num228 * 100f) + 1f)) + "%";
                bool flag15 = false;
                int num229 = 0;
            Label_46C1:
                while (!flag15)
                {
                    int num230 = genRand.Next((int) num3, Main.maxTilesY - 10);
                    if (num228 > 0.93)
                    {
                        num230 = Main.maxTilesY - 150;
                    }
                    else if (num228 > 0.75)
                    {
                        num230 = (int) num2;
                    }
                    int num231 = genRand.Next(1, Main.maxTilesX);
                    bool flag16 = false;
                    for (int num232 = num230; num232 < Main.maxTilesY; num232++)
                    {
                        if (!flag16)
                        {
                            if ((Main.tile[num231, num232].active && Main.tileSolid[Main.tile[num231, num232].type]) && !Main.tile[num231, num232 - 1].lava)
                            {
                                flag16 = true;
                            }
                        }
                        else
                        {
                            if (PlacePot(num231, num232, 0x1c))
                            {
                                flag15 = true;
                                goto Label_46C1;
                            }
                            num229++;
                            if (num229 >= 0x2710)
                            {
                                flag15 = true;
                                goto Label_46C1;
                            }
                        }
                    }
                }
            }
            for (int num233 = 0; num233 < ((int) ((Main.maxTilesX * Main.maxTilesY) * 1E-05)); num233++)
            {
                float num234 = (float) (((double) num233) / ((Main.maxTilesX * Main.maxTilesY) * 1E-05));
                Main.statusText = "Placing hellforges: " + ((int) ((num234 * 100f) + 1f)) + "%";
                bool flag17 = false;
                int num235 = 0;
                while (!flag17)
                {
                    int num236 = genRand.Next(1, Main.maxTilesX);
                    int num237 = genRand.Next(Main.maxTilesY - 250, Main.maxTilesY - 5);
                    if (Main.tile[num236, num237].wall == 13)
                    {
                        while (!Main.tile[num236, num237].active)
                        {
                            num237++;
                        }
                        num237--;
                        PlaceTile(num236, num237, 0x4d, false, false, -1);
                        if (Main.tile[num236, num237].type == 0x4d)
                        {
                            flag17 = true;
                        }
                        else
                        {
                            num235++;
                            if (num235 >= 0x2710)
                            {
                                flag17 = true;
                            }
                        }
                    }
                }
            }
            Main.statusText = "Spreading grass...";
            for (int num238 = 0; num238 < Main.maxTilesX; num238++)
            {
                num62 = num238;
                bool flag18 = true;
                for (int num239 = 0; num239 < (Main.worldSurface - 1.0); num239++)
                {
                    if (Main.tile[num62, num239].active)
                    {
                        if (flag18 && (Main.tile[num62, num239].type == 0))
                        {
                            SpreadGrass(num62, num239, 0, 2, true);
                        }
                        if (num239 > num3)
                        {
                            break;
                        }
                        flag18 = false;
                    }
                    else if (Main.tile[num62, num239].wall == 0)
                    {
                        flag18 = true;
                    }
                }
            }
            int num240 = 5;
            bool flag19 = true;
            while (flag19)
            {
                int num241 = (Main.maxTilesX / 2) + genRand.Next(-num240, num240 + 1);
                for (int num242 = 0; num242 < Main.maxTilesY; num242++)
                {
                    if (Main.tile[num241, num242].active)
                    {
                        Main.spawnTileX = num241;
                        Main.spawnTileY = num242;
                        Main.tile[num241, num242 - 1].lighted = true;
                        break;
                    }
                }
                flag19 = false;
                num240++;
                if (Main.spawnTileY > Main.worldSurface)
                {
                    flag19 = true;
                }
                if (Main.tile[Main.spawnTileX, Main.spawnTileY - 1].liquid > 0)
                {
                    flag19 = true;
                }
            }
            for (int num243 = 10; Main.spawnTileY > Main.worldSurface; num243++)
            {
                int num244 = genRand.Next((Main.maxTilesX / 2) - num243, (Main.maxTilesX / 2) + num243);
                for (int num245 = 0; num245 < Main.maxTilesY; num245++)
                {
                    if (Main.tile[num244, num245].active)
                    {
                        Main.spawnTileX = num244;
                        Main.spawnTileY = num245;
                        Main.tile[num244, num245 - 1].lighted = true;
                        break;
                    }
                }
            }
            int index = NPC.NewNPC(Main.spawnTileX * 0x10, Main.spawnTileY * 0x10, 0x16, 0);
            Main.npc[index].homeTileX = Main.spawnTileX;
            Main.npc[index].homeTileY = Main.spawnTileY;
            Main.npc[index].direction = 1;
            Main.npc[index].homeless = true;
            Main.statusText = "Planting sunflowers...";
            for (int num247 = 0; num247 < (Main.maxTilesX * 0.002); num247++)
            {
                int num248 = 0;
                int num249 = 0;
                int num250 = 0;
                int num1 = Main.maxTilesX / 2;
                num248 = genRand.Next(Main.maxTilesX);
                num249 = (num248 - genRand.Next(10)) - 7;
                num250 = (num248 + genRand.Next(10)) + 7;
                if (num249 < 0)
                {
                    num249 = 0;
                }
                if (num250 > (Main.maxTilesX - 1))
                {
                    num250 = Main.maxTilesX - 1;
                }
                for (int num251 = num249; num251 < num250; num251++)
                {
                    for (int num252 = 1; num252 < (Main.worldSurface - 1.0); num252++)
                    {
                        if ((Main.tile[num251, num252].type == 1) && Main.tile[num251, num252].active)
                        {
                            Main.tile[num251, num252].type = 2;
                        }
                        if ((Main.tile[num251 + 1, num252].type == 1) && Main.tile[num251 + 1, num252].active)
                        {
                            Main.tile[num251 + 1, num252].type = 2;
                        }
                        if (((Main.tile[num251, num252].type == 2) && Main.tile[num251, num252].active) && !Main.tile[num251, num252 - 1].active)
                        {
                            PlaceTile(num251, num252 - 1, 0x1b, true, false, -1);
                        }
                        if (Main.tile[num251, num252].active)
                        {
                            break;
                        }
                    }
                }
            }
            Main.statusText = "Planting trees...";
            for (int num253 = 0; num253 < (Main.maxTilesX * 0.003); num253++)
            {
                int num254 = genRand.Next(50, Main.maxTilesX - 50);
                int num255 = genRand.Next(0x19, 50);
                for (int num256 = num254 - num255; num256 < (num254 + num255); num256++)
                {
                    for (int num257 = 20; num257 < Main.worldSurface; num257++)
                    {
                        if (Main.tile[num256, num257].active)
                        {
                            if (Main.tile[num256, num257].type == 1)
                            {
                                Main.tile[num256, num257].type = 2;
                            }
                            if (Main.tile[num256, num257 + 1].type == 1)
                            {
                                Main.tile[num256, num257 + 1].type = 2;
                            }
                            break;
                        }
                    }
                }
                for (int num258 = num254 - num255; num258 < (num254 + num255); num258++)
                {
                    for (int num259 = 20; num259 < Main.worldSurface; num259++)
                    {
                        GrowEpicTree(num258, num259);
                    }
                }
            }
            AddTrees();
            Main.statusText = "Planting weeds...";
            AddPlants();
            for (int num260 = 0; num260 < Main.maxTilesX; num260++)
            {
                for (int num261 = (int) Main.worldSurface; num261 < Main.maxTilesY; num261++)
                {
                    if (Main.tile[num260, num261].active)
                    {
                        if ((Main.tile[num260, num261].type == 70) && !Main.tile[num260, num261 - 1].active)
                        {
                            GrowShroom(num260, num261);
                            if (!Main.tile[num260, num261 - 1].active)
                            {
                                PlaceTile(num260, num261 - 1, 0x47, true, false, -1);
                            }
                        }
                        if ((Main.tile[num260, num261].type == 60) && !Main.tile[num260, num261 - 1].active)
                        {
                            PlaceTile(num260, num261 - 1, 0x3d, true, false, -1);
                        }
                    }
                }
            }
            Main.statusText = "Growing vines...";
            for (int num262 = 0; num262 < Main.maxTilesX; num262++)
            {
                int num263 = 0;
                for (int num264 = 0; num264 < Main.worldSurface; num264++)
                {
                    if ((num263 > 0) && !Main.tile[num262, num264].active)
                    {
                        Main.tile[num262, num264].active = true;
                        Main.tile[num262, num264].type = 0x34;
                        num263--;
                    }
                    else
                    {
                        num263 = 0;
                    }
                    if ((Main.tile[num262, num264].active && (Main.tile[num262, num264].type == 2)) && (genRand.Next(5) < 3))
                    {
                        num263 = genRand.Next(1, 10);
                    }
                }
                num263 = 0;
                for (int num265 = (int) Main.worldSurface; num265 < Main.maxTilesY; num265++)
                {
                    if ((num263 > 0) && !Main.tile[num262, num265].active)
                    {
                        Main.tile[num262, num265].active = true;
                        Main.tile[num262, num265].type = 0x3e;
                        num263--;
                    }
                    else
                    {
                        num263 = 0;
                    }
                    if ((Main.tile[num262, num265].active && (Main.tile[num262, num265].type == 60)) && (genRand.Next(5) < 3))
                    {
                        num263 = genRand.Next(1, 10);
                    }
                }
            }
            Main.statusText = "Planting flowers...";
            for (int num266 = 0; num266 < (Main.maxTilesX * 0.005); num266++)
            {
                int num267 = genRand.Next(20, Main.maxTilesX - 20);
                int num268 = genRand.Next(5, 15);
                int num269 = genRand.Next(15, 30);
                for (int num270 = 1; num270 < (Main.worldSurface - 1.0); num270++)
                {
                    if (Main.tile[num267, num270].active)
                    {
                        for (int num271 = num267 - num268; num271 < (num267 + num268); num271++)
                        {
                            for (int num272 = num270 - num269; num272 < (num270 + num269); num272++)
                            {
                                if ((Main.tile[num271, num272].type == 3) || (Main.tile[num271, num272].type == 0x18))
                                {
                                    Main.tile[num271, num272].frameX = (short) (genRand.Next(6, 8) * 0x12);
                                }
                            }
                        }
                        break;
                    }
                }
            }
            Main.statusText = "Planting mushrooms...";
            for (int num273 = 0; num273 < (Main.maxTilesX * 0.002); num273++)
            {
                int num274 = genRand.Next(20, Main.maxTilesX - 20);
                int num275 = genRand.Next(4, 10);
                int num276 = genRand.Next(15, 30);
                for (int num277 = 1; num277 < (Main.worldSurface - 1.0); num277++)
                {
                    if (Main.tile[num274, num277].active)
                    {
                        for (int num278 = num274 - num275; num278 < (num274 + num275); num278++)
                        {
                            for (int num279 = num277 - num276; num279 < (num277 + num276); num279++)
                            {
                                if ((Main.tile[num278, num279].type == 3) || (Main.tile[num278, num279].type == 0x18))
                                {
                                    Main.tile[num278, num279].frameX = 0x90;
                                }
                            }
                        }
                        break;
                    }
                }
            }
            gen = false;
        }

        public static void GrowEpicTree(int i, int y)
        {
            int num2;
            int num = y;
            while (Main.tile[i, num].type == 20)
            {
                num++;
            }
            if ((((!Main.tile[i, num].active || (Main.tile[i, num].type != 2)) || ((Main.tile[i, num - 1].wall != 0) || (Main.tile[i, num - 1].liquid != 0))) || ((!Main.tile[i - 1, num].active || (Main.tile[i - 1, num].type != 2)) || (!Main.tile[i + 1, num].active || (Main.tile[i + 1, num].type != 2)))) || !EmptyTileCheck(i - 2, i + 2, num - 0x37, num - 1, 20))
            {
                return;
            }
            bool flag = false;
            bool flag2 = false;
            int num4 = genRand.Next(20, 30);
            for (int j = num - num4; j < num; j++)
            {
                Main.tile[i, j].frameNumber = (byte) genRand.Next(3);
                Main.tile[i, j].active = true;
                Main.tile[i, j].type = 5;
                num2 = genRand.Next(3);
                int num3 = genRand.Next(10);
                if ((j == (num - 1)) || (j == (num - num4)))
                {
                    num3 = 0;
                }
                while ((((num3 == 5) || (num3 == 7)) && flag) || (((num3 == 6) || (num3 == 7)) && flag2))
                {
                    num3 = genRand.Next(10);
                }
                flag = false;
                flag2 = false;
                if ((num3 == 5) || (num3 == 7))
                {
                    flag = true;
                }
                if ((num3 == 6) || (num3 == 7))
                {
                    flag2 = true;
                }
                if (num3 == 1)
                {
                    switch (num2)
                    {
                        case 0:
                            Main.tile[i, j].frameX = 0;
                            Main.tile[i, j].frameY = 0x42;
                            break;

                        case 1:
                            Main.tile[i, j].frameX = 0;
                            Main.tile[i, j].frameY = 0x58;
                            break;

                        case 2:
                            Main.tile[i, j].frameX = 0;
                            Main.tile[i, j].frameY = 110;
                            break;
                    }
                }
                else if (num3 == 2)
                {
                    switch (num2)
                    {
                        case 0:
                            Main.tile[i, j].frameX = 0x16;
                            Main.tile[i, j].frameY = 0;
                            break;

                        case 1:
                            Main.tile[i, j].frameX = 0x16;
                            Main.tile[i, j].frameY = 0x16;
                            break;

                        case 2:
                            Main.tile[i, j].frameX = 0x16;
                            Main.tile[i, j].frameY = 0x2c;
                            break;
                    }
                }
                else if (num3 == 3)
                {
                    switch (num2)
                    {
                        case 0:
                            Main.tile[i, j].frameX = 0x2c;
                            Main.tile[i, j].frameY = 0x42;
                            break;

                        case 1:
                            Main.tile[i, j].frameX = 0x2c;
                            Main.tile[i, j].frameY = 0x58;
                            break;

                        case 2:
                            Main.tile[i, j].frameX = 0x2c;
                            Main.tile[i, j].frameY = 110;
                            break;
                    }
                }
                else if (num3 == 4)
                {
                    switch (num2)
                    {
                        case 0:
                            Main.tile[i, j].frameX = 0x16;
                            Main.tile[i, j].frameY = 0x42;
                            break;

                        case 1:
                            Main.tile[i, j].frameX = 0x16;
                            Main.tile[i, j].frameY = 0x58;
                            break;

                        case 2:
                            Main.tile[i, j].frameX = 0x16;
                            Main.tile[i, j].frameY = 110;
                            break;
                    }
                }
                else if (num3 == 5)
                {
                    switch (num2)
                    {
                        case 0:
                            Main.tile[i, j].frameX = 0x58;
                            Main.tile[i, j].frameY = 0;
                            break;

                        case 1:
                            Main.tile[i, j].frameX = 0x58;
                            Main.tile[i, j].frameY = 0x16;
                            break;

                        case 2:
                            Main.tile[i, j].frameX = 0x58;
                            Main.tile[i, j].frameY = 0x2c;
                            break;
                    }
                }
                else if (num3 == 6)
                {
                    switch (num2)
                    {
                        case 0:
                            Main.tile[i, j].frameX = 0x42;
                            Main.tile[i, j].frameY = 0x42;
                            break;

                        case 1:
                            Main.tile[i, j].frameX = 0x42;
                            Main.tile[i, j].frameY = 0x58;
                            break;

                        case 2:
                            Main.tile[i, j].frameX = 0x42;
                            Main.tile[i, j].frameY = 110;
                            break;
                    }
                }
                else if (num3 == 7)
                {
                    switch (num2)
                    {
                        case 0:
                            Main.tile[i, j].frameX = 110;
                            Main.tile[i, j].frameY = 0x42;
                            break;

                        case 1:
                            Main.tile[i, j].frameX = 110;
                            Main.tile[i, j].frameY = 0x58;
                            break;

                        case 2:
                            Main.tile[i, j].frameX = 110;
                            Main.tile[i, j].frameY = 110;
                            break;
                    }
                }
                else
                {
                    switch (num2)
                    {
                        case 0:
                            Main.tile[i, j].frameX = 0;
                            Main.tile[i, j].frameY = 0;
                            break;

                        case 1:
                            Main.tile[i, j].frameX = 0;
                            Main.tile[i, j].frameY = 0x16;
                            break;

                        case 2:
                            Main.tile[i, j].frameX = 0;
                            Main.tile[i, j].frameY = 0x2c;
                            break;
                    }
                }
                if ((num3 == 5) || (num3 == 7))
                {
                    Main.tile[i - 1, j].active = true;
                    Main.tile[i - 1, j].type = 5;
                    num2 = genRand.Next(3);
                    if (genRand.Next(3) < 2)
                    {
                        switch (num2)
                        {
                            case 0:
                                Main.tile[i - 1, j].frameX = 0x2c;
                                Main.tile[i - 1, j].frameY = 0xc6;
                                break;

                            case 1:
                                Main.tile[i - 1, j].frameX = 0x2c;
                                Main.tile[i - 1, j].frameY = 220;
                                break;

                            case 2:
                                Main.tile[i - 1, j].frameX = 0x2c;
                                Main.tile[i - 1, j].frameY = 0xf2;
                                break;
                        }
                    }
                    else
                    {
                        switch (num2)
                        {
                            case 0:
                                Main.tile[i - 1, j].frameX = 0x42;
                                Main.tile[i - 1, j].frameY = 0;
                                break;

                            case 1:
                                Main.tile[i - 1, j].frameX = 0x42;
                                Main.tile[i - 1, j].frameY = 0x16;
                                break;

                            case 2:
                                Main.tile[i - 1, j].frameX = 0x42;
                                Main.tile[i - 1, j].frameY = 0x2c;
                                break;
                        }
                    }
                }
                if ((num3 == 6) || (num3 == 7))
                {
                    Main.tile[i + 1, j].active = true;
                    Main.tile[i + 1, j].type = 5;
                    num2 = genRand.Next(3);
                    if (genRand.Next(3) < 2)
                    {
                        switch (num2)
                        {
                            case 0:
                                Main.tile[i + 1, j].frameX = 0x42;
                                Main.tile[i + 1, j].frameY = 0xc6;
                                break;

                            case 1:
                                Main.tile[i + 1, j].frameX = 0x42;
                                Main.tile[i + 1, j].frameY = 220;
                                break;

                            case 2:
                                Main.tile[i + 1, j].frameX = 0x42;
                                Main.tile[i + 1, j].frameY = 0xf2;
                                break;
                        }
                    }
                    else
                    {
                        switch (num2)
                        {
                            case 0:
                                Main.tile[i + 1, j].frameX = 0x58;
                                Main.tile[i + 1, j].frameY = 0x42;
                                break;

                            case 1:
                                Main.tile[i + 1, j].frameX = 0x58;
                                Main.tile[i + 1, j].frameY = 0x58;
                                break;

                            case 2:
                                Main.tile[i + 1, j].frameX = 0x58;
                                Main.tile[i + 1, j].frameY = 110;
                                break;
                        }
                    }
                }
            }
            int num6 = genRand.Next(3);
            if ((num6 == 0) || (num6 == 1))
            {
                Main.tile[i + 1, num - 1].active = true;
                Main.tile[i + 1, num - 1].type = 5;
                switch (genRand.Next(3))
                {
                    case 0:
                        Main.tile[i + 1, num - 1].frameX = 0x16;
                        Main.tile[i + 1, num - 1].frameY = 0x84;
                        break;

                    case 1:
                        Main.tile[i + 1, num - 1].frameX = 0x16;
                        Main.tile[i + 1, num - 1].frameY = 0x9a;
                        break;

                    case 2:
                        Main.tile[i + 1, num - 1].frameX = 0x16;
                        Main.tile[i + 1, num - 1].frameY = 0xb0;
                        goto Label_0A36;
                }
            }
        Label_0A36:
            if ((num6 == 0) || (num6 == 2))
            {
                Main.tile[i - 1, num - 1].active = true;
                Main.tile[i - 1, num - 1].type = 5;
                switch (genRand.Next(3))
                {
                    case 0:
                        Main.tile[i - 1, num - 1].frameX = 0x2c;
                        Main.tile[i - 1, num - 1].frameY = 0x84;
                        break;

                    case 1:
                        Main.tile[i - 1, num - 1].frameX = 0x2c;
                        Main.tile[i - 1, num - 1].frameY = 0x9a;
                        break;

                    case 2:
                        Main.tile[i - 1, num - 1].frameX = 0x2c;
                        Main.tile[i - 1, num - 1].frameY = 0xb0;
                        break;
                }
            }
            num2 = genRand.Next(3);
            if (num6 == 0)
            {
                switch (num2)
                {
                    case 0:
                        Main.tile[i, num - 1].frameX = 0x58;
                        Main.tile[i, num - 1].frameY = 0x84;
                        break;

                    case 1:
                        Main.tile[i, num - 1].frameX = 0x58;
                        Main.tile[i, num - 1].frameY = 0x9a;
                        break;

                    case 2:
                        Main.tile[i, num - 1].frameX = 0x58;
                        Main.tile[i, num - 1].frameY = 0xb0;
                        break;
                }
            }
            else if (num6 == 1)
            {
                switch (num2)
                {
                    case 0:
                        Main.tile[i, num - 1].frameX = 0;
                        Main.tile[i, num - 1].frameY = 0x84;
                        break;

                    case 1:
                        Main.tile[i, num - 1].frameX = 0;
                        Main.tile[i, num - 1].frameY = 0x9a;
                        break;

                    case 2:
                        Main.tile[i, num - 1].frameX = 0;
                        Main.tile[i, num - 1].frameY = 0xb0;
                        break;
                }
            }
            else if (num6 == 2)
            {
                switch (num2)
                {
                    case 0:
                        Main.tile[i, num - 1].frameX = 0x42;
                        Main.tile[i, num - 1].frameY = 0x84;
                        break;

                    case 1:
                        Main.tile[i, num - 1].frameX = 0x42;
                        Main.tile[i, num - 1].frameY = 0x9a;
                        break;

                    case 2:
                        Main.tile[i, num - 1].frameX = 0x42;
                        Main.tile[i, num - 1].frameY = 0xb0;
                        break;
                }
            }
            if (genRand.Next(3) < 2)
            {
                switch (genRand.Next(3))
                {
                    case 0:
                        Main.tile[i, num - num4].frameX = 0x16;
                        Main.tile[i, num - num4].frameY = 0xc6;
                        break;

                    case 1:
                        Main.tile[i, num - num4].frameX = 0x16;
                        Main.tile[i, num - num4].frameY = 220;
                        break;

                    case 2:
                        Main.tile[i, num - num4].frameX = 0x16;
                        Main.tile[i, num - num4].frameY = 0xf2;
                        break;
                }
            }
            else
            {
                switch (genRand.Next(3))
                {
                    case 0:
                        Main.tile[i, num - num4].frameX = 0;
                        Main.tile[i, num - num4].frameY = 0xc6;
                        break;

                    case 1:
                        Main.tile[i, num - num4].frameX = 0;
                        Main.tile[i, num - num4].frameY = 220;
                        break;

                    case 2:
                        Main.tile[i, num - num4].frameX = 0;
                        Main.tile[i, num - num4].frameY = 0xf2;
                        break;
                }
            }
            RangeFrame(i - 2, (num - num4) - 1, i + 2, num + 1);
            if (Main.netMode == 2)
            {
                NetMessage.SendTileSquare(-1, i, num - ((int) (num4 * 0.5)), num4 + 1);
            }
        }

        public static void GrowShroom(int i, int y)
        {
            int num = y;
            if (((!Main.tile[i - 1, num - 1].lava && !Main.tile[i - 1, num - 1].lava) && !Main.tile[i + 1, num - 1].lava) && (((Main.tile[i, num].active && (Main.tile[i, num].type == 70)) && ((Main.tile[i, num - 1].wall == 0) && Main.tile[i - 1, num].active)) && (((Main.tile[i - 1, num].type == 70) && Main.tile[i + 1, num].active) && ((Main.tile[i + 1, num].type == 70) && EmptyTileCheck(i - 2, i + 2, num - 13, num - 1, 0x47)))))
            {
                int num3 = genRand.Next(4, 11);
                for (int j = num - num3; j < num; j++)
                {
                    Main.tile[i, j].frameNumber = (byte) genRand.Next(3);
                    Main.tile[i, j].active = true;
                    Main.tile[i, j].type = 0x48;
                    switch (genRand.Next(3))
                    {
                        case 0:
                            Main.tile[i, j].frameX = 0;
                            Main.tile[i, j].frameY = 0;
                            break;

                        case 1:
                            Main.tile[i, j].frameX = 0;
                            Main.tile[i, j].frameY = 0x12;
                            break;

                        case 2:
                            Main.tile[i, j].frameX = 0;
                            Main.tile[i, j].frameY = 0x24;
                            break;
                    }
                }
                switch (genRand.Next(3))
                {
                    case 0:
                        Main.tile[i, num - num3].frameX = 0x24;
                        Main.tile[i, num - num3].frameY = 0;
                        break;

                    case 1:
                        Main.tile[i, num - num3].frameX = 0x24;
                        Main.tile[i, num - num3].frameY = 0x12;
                        break;

                    case 2:
                        Main.tile[i, num - num3].frameX = 0x24;
                        Main.tile[i, num - num3].frameY = 0x24;
                        break;
                }
                RangeFrame(i - 2, (num - num3) - 1, i + 2, num + 1);
                if (Main.netMode == 2)
                {
                    NetMessage.SendTileSquare(-1, i, num - ((int) (num3 * 0.5)), num3 + 1);
                }
            }
        }

        public static void GrowTree(int i, int y)
        {
            int num2;
            int num = y;
            while (Main.tile[i, num].type == 20)
            {
                num++;
            }
            if (((Main.tile[i - 1, num - 1].liquid != 0) || (Main.tile[i - 1, num - 1].liquid != 0)) || (Main.tile[i + 1, num - 1].liquid != 0))
            {
                return;
            }
            if (((!Main.tile[i, num].active || (Main.tile[i, num].type != 2)) || ((Main.tile[i, num - 1].wall != 0) || !Main.tile[i - 1, num].active)) || (((Main.tile[i - 1, num].type != 2) || !Main.tile[i + 1, num].active) || ((Main.tile[i + 1, num].type != 2) || !EmptyTileCheck(i - 2, i + 2, num - 14, num - 1, 20))))
            {
                return;
            }
            bool flag = false;
            bool flag2 = false;
            int num4 = genRand.Next(5, 15);
            for (int j = num - num4; j < num; j++)
            {
                Main.tile[i, j].frameNumber = (byte) genRand.Next(3);
                Main.tile[i, j].active = true;
                Main.tile[i, j].type = 5;
                num2 = genRand.Next(3);
                int num3 = genRand.Next(10);
                if ((j == (num - 1)) || (j == (num - num4)))
                {
                    num3 = 0;
                }
                while ((((num3 == 5) || (num3 == 7)) && flag) || (((num3 == 6) || (num3 == 7)) && flag2))
                {
                    num3 = genRand.Next(10);
                }
                flag = false;
                flag2 = false;
                if ((num3 == 5) || (num3 == 7))
                {
                    flag = true;
                }
                if ((num3 == 6) || (num3 == 7))
                {
                    flag2 = true;
                }
                if (num3 == 1)
                {
                    switch (num2)
                    {
                        case 0:
                            Main.tile[i, j].frameX = 0;
                            Main.tile[i, j].frameY = 0x42;
                            break;

                        case 1:
                            Main.tile[i, j].frameX = 0;
                            Main.tile[i, j].frameY = 0x58;
                            break;

                        case 2:
                            Main.tile[i, j].frameX = 0;
                            Main.tile[i, j].frameY = 110;
                            break;
                    }
                }
                else if (num3 == 2)
                {
                    switch (num2)
                    {
                        case 0:
                            Main.tile[i, j].frameX = 0x16;
                            Main.tile[i, j].frameY = 0;
                            break;

                        case 1:
                            Main.tile[i, j].frameX = 0x16;
                            Main.tile[i, j].frameY = 0x16;
                            break;

                        case 2:
                            Main.tile[i, j].frameX = 0x16;
                            Main.tile[i, j].frameY = 0x2c;
                            break;
                    }
                }
                else if (num3 == 3)
                {
                    switch (num2)
                    {
                        case 0:
                            Main.tile[i, j].frameX = 0x2c;
                            Main.tile[i, j].frameY = 0x42;
                            break;

                        case 1:
                            Main.tile[i, j].frameX = 0x2c;
                            Main.tile[i, j].frameY = 0x58;
                            break;

                        case 2:
                            Main.tile[i, j].frameX = 0x2c;
                            Main.tile[i, j].frameY = 110;
                            break;
                    }
                }
                else if (num3 == 4)
                {
                    switch (num2)
                    {
                        case 0:
                            Main.tile[i, j].frameX = 0x16;
                            Main.tile[i, j].frameY = 0x42;
                            break;

                        case 1:
                            Main.tile[i, j].frameX = 0x16;
                            Main.tile[i, j].frameY = 0x58;
                            break;

                        case 2:
                            Main.tile[i, j].frameX = 0x16;
                            Main.tile[i, j].frameY = 110;
                            break;
                    }
                }
                else if (num3 == 5)
                {
                    switch (num2)
                    {
                        case 0:
                            Main.tile[i, j].frameX = 0x58;
                            Main.tile[i, j].frameY = 0;
                            break;

                        case 1:
                            Main.tile[i, j].frameX = 0x58;
                            Main.tile[i, j].frameY = 0x16;
                            break;

                        case 2:
                            Main.tile[i, j].frameX = 0x58;
                            Main.tile[i, j].frameY = 0x2c;
                            break;
                    }
                }
                else if (num3 == 6)
                {
                    switch (num2)
                    {
                        case 0:
                            Main.tile[i, j].frameX = 0x42;
                            Main.tile[i, j].frameY = 0x42;
                            break;

                        case 1:
                            Main.tile[i, j].frameX = 0x42;
                            Main.tile[i, j].frameY = 0x58;
                            break;

                        case 2:
                            Main.tile[i, j].frameX = 0x42;
                            Main.tile[i, j].frameY = 110;
                            break;
                    }
                }
                else if (num3 == 7)
                {
                    switch (num2)
                    {
                        case 0:
                            Main.tile[i, j].frameX = 110;
                            Main.tile[i, j].frameY = 0x42;
                            break;

                        case 1:
                            Main.tile[i, j].frameX = 110;
                            Main.tile[i, j].frameY = 0x58;
                            break;

                        case 2:
                            Main.tile[i, j].frameX = 110;
                            Main.tile[i, j].frameY = 110;
                            break;
                    }
                }
                else
                {
                    switch (num2)
                    {
                        case 0:
                            Main.tile[i, j].frameX = 0;
                            Main.tile[i, j].frameY = 0;
                            break;

                        case 1:
                            Main.tile[i, j].frameX = 0;
                            Main.tile[i, j].frameY = 0x16;
                            break;

                        case 2:
                            Main.tile[i, j].frameX = 0;
                            Main.tile[i, j].frameY = 0x2c;
                            break;
                    }
                }
                if ((num3 == 5) || (num3 == 7))
                {
                    Main.tile[i - 1, j].active = true;
                    Main.tile[i - 1, j].type = 5;
                    num2 = genRand.Next(3);
                    if (genRand.Next(3) < 2)
                    {
                        switch (num2)
                        {
                            case 0:
                                Main.tile[i - 1, j].frameX = 0x2c;
                                Main.tile[i - 1, j].frameY = 0xc6;
                                break;

                            case 1:
                                Main.tile[i - 1, j].frameX = 0x2c;
                                Main.tile[i - 1, j].frameY = 220;
                                break;

                            case 2:
                                Main.tile[i - 1, j].frameX = 0x2c;
                                Main.tile[i - 1, j].frameY = 0xf2;
                                break;
                        }
                    }
                    else
                    {
                        switch (num2)
                        {
                            case 0:
                                Main.tile[i - 1, j].frameX = 0x42;
                                Main.tile[i - 1, j].frameY = 0;
                                break;

                            case 1:
                                Main.tile[i - 1, j].frameX = 0x42;
                                Main.tile[i - 1, j].frameY = 0x16;
                                break;

                            case 2:
                                Main.tile[i - 1, j].frameX = 0x42;
                                Main.tile[i - 1, j].frameY = 0x2c;
                                break;
                        }
                    }
                }
                if ((num3 == 6) || (num3 == 7))
                {
                    Main.tile[i + 1, j].active = true;
                    Main.tile[i + 1, j].type = 5;
                    num2 = genRand.Next(3);
                    if (genRand.Next(3) < 2)
                    {
                        switch (num2)
                        {
                            case 0:
                                Main.tile[i + 1, j].frameX = 0x42;
                                Main.tile[i + 1, j].frameY = 0xc6;
                                break;

                            case 1:
                                Main.tile[i + 1, j].frameX = 0x42;
                                Main.tile[i + 1, j].frameY = 220;
                                break;

                            case 2:
                                Main.tile[i + 1, j].frameX = 0x42;
                                Main.tile[i + 1, j].frameY = 0xf2;
                                break;
                        }
                    }
                    else
                    {
                        switch (num2)
                        {
                            case 0:
                                Main.tile[i + 1, j].frameX = 0x58;
                                Main.tile[i + 1, j].frameY = 0x42;
                                break;

                            case 1:
                                Main.tile[i + 1, j].frameX = 0x58;
                                Main.tile[i + 1, j].frameY = 0x58;
                                break;

                            case 2:
                                Main.tile[i + 1, j].frameX = 0x58;
                                Main.tile[i + 1, j].frameY = 110;
                                break;
                        }
                    }
                }
            }
            int num6 = genRand.Next(3);
            if ((num6 == 0) || (num6 == 1))
            {
                Main.tile[i + 1, num - 1].active = true;
                Main.tile[i + 1, num - 1].type = 5;
                switch (genRand.Next(3))
                {
                    case 0:
                        Main.tile[i + 1, num - 1].frameX = 0x16;
                        Main.tile[i + 1, num - 1].frameY = 0x84;
                        break;

                    case 1:
                        Main.tile[i + 1, num - 1].frameX = 0x16;
                        Main.tile[i + 1, num - 1].frameY = 0x9a;
                        break;

                    case 2:
                        Main.tile[i + 1, num - 1].frameX = 0x16;
                        Main.tile[i + 1, num - 1].frameY = 0xb0;
                        goto Label_0A63;
                }
            }
        Label_0A63:
            if ((num6 == 0) || (num6 == 2))
            {
                Main.tile[i - 1, num - 1].active = true;
                Main.tile[i - 1, num - 1].type = 5;
                switch (genRand.Next(3))
                {
                    case 0:
                        Main.tile[i - 1, num - 1].frameX = 0x2c;
                        Main.tile[i - 1, num - 1].frameY = 0x84;
                        break;

                    case 1:
                        Main.tile[i - 1, num - 1].frameX = 0x2c;
                        Main.tile[i - 1, num - 1].frameY = 0x9a;
                        break;

                    case 2:
                        Main.tile[i - 1, num - 1].frameX = 0x2c;
                        Main.tile[i - 1, num - 1].frameY = 0xb0;
                        break;
                }
            }
            num2 = genRand.Next(3);
            if (num6 == 0)
            {
                switch (num2)
                {
                    case 0:
                        Main.tile[i, num - 1].frameX = 0x58;
                        Main.tile[i, num - 1].frameY = 0x84;
                        break;

                    case 1:
                        Main.tile[i, num - 1].frameX = 0x58;
                        Main.tile[i, num - 1].frameY = 0x9a;
                        break;

                    case 2:
                        Main.tile[i, num - 1].frameX = 0x58;
                        Main.tile[i, num - 1].frameY = 0xb0;
                        break;
                }
            }
            else if (num6 == 1)
            {
                switch (num2)
                {
                    case 0:
                        Main.tile[i, num - 1].frameX = 0;
                        Main.tile[i, num - 1].frameY = 0x84;
                        break;

                    case 1:
                        Main.tile[i, num - 1].frameX = 0;
                        Main.tile[i, num - 1].frameY = 0x9a;
                        break;

                    case 2:
                        Main.tile[i, num - 1].frameX = 0;
                        Main.tile[i, num - 1].frameY = 0xb0;
                        break;
                }
            }
            else if (num6 == 2)
            {
                switch (num2)
                {
                    case 0:
                        Main.tile[i, num - 1].frameX = 0x42;
                        Main.tile[i, num - 1].frameY = 0x84;
                        break;

                    case 1:
                        Main.tile[i, num - 1].frameX = 0x42;
                        Main.tile[i, num - 1].frameY = 0x9a;
                        break;

                    case 2:
                        Main.tile[i, num - 1].frameX = 0x42;
                        Main.tile[i, num - 1].frameY = 0xb0;
                        break;
                }
            }
            if (genRand.Next(3) < 2)
            {
                switch (genRand.Next(3))
                {
                    case 0:
                        Main.tile[i, num - num4].frameX = 0x16;
                        Main.tile[i, num - num4].frameY = 0xc6;
                        break;

                    case 1:
                        Main.tile[i, num - num4].frameX = 0x16;
                        Main.tile[i, num - num4].frameY = 220;
                        break;

                    case 2:
                        Main.tile[i, num - num4].frameX = 0x16;
                        Main.tile[i, num - num4].frameY = 0xf2;
                        break;
                }
            }
            else
            {
                switch (genRand.Next(3))
                {
                    case 0:
                        Main.tile[i, num - num4].frameX = 0;
                        Main.tile[i, num - num4].frameY = 0xc6;
                        break;

                    case 1:
                        Main.tile[i, num - num4].frameX = 0;
                        Main.tile[i, num - num4].frameY = 220;
                        break;

                    case 2:
                        Main.tile[i, num - num4].frameX = 0;
                        Main.tile[i, num - num4].frameY = 0xf2;
                        break;
                }
            }
            RangeFrame(i - 2, (num - num4) - 1, i + 2, num + 1);
            if (Main.netMode == 2)
            {
                NetMessage.SendTileSquare(-1, i, num - ((int) (num4 * 0.5)), num4 + 1);
            }
        }

        public static void HellHouse(int i, int j)
        {
            int width = genRand.Next(8, 20);
            int num2 = genRand.Next(3);
            int num3 = genRand.Next(7);
            int num4 = i;
            int num5 = j;
            for (int k = 0; k < num2; k++)
            {
                int height = genRand.Next(5, 9);
                HellRoom(num4, num5, width, height);
                num5 -= height;
            }
            num5 = j;
            for (int m = 0; m < num3; m++)
            {
                int num9 = genRand.Next(5, 9);
                num5 += num9;
                HellRoom(num4, num5, width, num9);
            }
            for (int n = i - (width / 2); n <= (i + (width / 2)); n++)
            {
                num5 = j;
                while ((num5 < Main.maxTilesY) && ((Main.tile[n, num5].active && (Main.tile[n, num5].type == 0x4c)) || (Main.tile[n, num5].wall == 13)))
                {
                    num5++;
                }
                int num11 = 6 + genRand.Next(3);
                while ((num5 < Main.maxTilesY) && !Main.tile[n, num5].active)
                {
                    num11--;
                    Main.tile[n, num5].active = true;
                    Main.tile[n, num5].type = 0x39;
                    num5++;
                    if (num11 <= 0)
                    {
                        break;
                    }
                }
            }
            int minValue = 0;
            int maxValue = 0;
            num5 = j;
            while ((num5 < Main.maxTilesY) && ((Main.tile[i, num5].active && (Main.tile[i, num5].type == 0x4c)) || (Main.tile[i, num5].wall == 13)))
            {
                num5++;
            }
            num5--;
            maxValue = num5;
            while ((Main.tile[i, num5].active && (Main.tile[i, num5].type == 0x4c)) || (Main.tile[i, num5].wall == 13))
            {
                num5--;
                if (Main.tile[i, num5].active && (Main.tile[i, num5].type == 0x4c))
                {
                    int num14 = genRand.Next((i - (width / 2)) + 1, (i + (width / 2)) - 1);
                    int num15 = genRand.Next((i - (width / 2)) + 1, (i + (width / 2)) - 1);
                    if (num14 > num15)
                    {
                        int num16 = num14;
                        num14 = num15;
                        num15 = num16;
                    }
                    if (num14 == num15)
                    {
                        if (num14 < i)
                        {
                            num15++;
                        }
                        else
                        {
                            num14--;
                        }
                    }
                    for (int num17 = num14; num17 <= num15; num17++)
                    {
                        if (Main.tile[num17, num5 - 1].wall == 13)
                        {
                            Main.tile[num17, num5].wall = 13;
                        }
                        Main.tile[num17, num5].type = 0x13;
                        Main.tile[num17, num5].active = true;
                    }
                    num5--;
                }
            }
            minValue = num5;
            float num18 = (maxValue - minValue) * width;
            float num19 = num18 * 0.02f;
            for (int num20 = 0; num20 < num19; num20++)
            {
                int num21 = genRand.Next(i - (width / 2), (i + (width / 2)) + 1);
                int num22 = genRand.Next(minValue, maxValue);
                int num23 = genRand.Next(3, 8);
                for (int num24 = num21 - num23; num24 <= (num21 + num23); num24++)
                {
                    for (int num25 = num22 - num23; num25 <= (num22 + num23); num25++)
                    {
                        float num26 = Math.Abs((int) (num24 - num21));
                        float num27 = Math.Abs((int) (num25 - num22));
                        if (Math.Sqrt((double) ((num26 * num26) + (num27 * num27))) < (num23 * 0.4))
                        {
                            if ((Main.tile[num24, num25].type == 0x4c) || (Main.tile[num24, num25].type == 0x13))
                            {
                                Main.tile[num24, num25].active = false;
                            }
                            Main.tile[num24, num25].wall = 0;
                        }
                    }
                }
            }
        }

        public static void HellRoom(int i, int j, int width, int height)
        {
            for (int k = i - (width / 2); k <= (i + (width / 2)); k++)
            {
                for (int n = j - height; n <= j; n++)
                {
                    Main.tile[k, n].active = true;
                    Main.tile[k, n].type = 0x4c;
                    Main.tile[k, n].liquid = 0;
                    Main.tile[k, n].lava = false;
                }
            }
            for (int m = (i - (width / 2)) + 1; m <= ((i + (width / 2)) - 1); m++)
            {
                for (int num4 = (j - height) + 1; num4 <= (j - 1); num4++)
                {
                    Main.tile[m, num4].active = false;
                    Main.tile[m, num4].wall = 13;
                    Main.tile[m, num4].liquid = 0;
                    Main.tile[m, num4].lava = false;
                }
            }
        }

        public static void IslandHouse(int i, int j)
        {
            byte num = (byte) genRand.Next(0x2d, 0x30);
            byte num2 = (byte) genRand.Next(10, 13);
            Vector2 vector = new Vector2((float) i, (float) j);
            int num7 = 1;
            if (genRand.Next(2) == 0)
            {
                num7 = -1;
            }
            int num8 = genRand.Next(7, 12);
            int num9 = genRand.Next(5, 7);
            vector.X = i + ((num8 + 2) * num7);
            for (int k = j - 15; k < (j + 30); k++)
            {
                if (Main.tile[(int) vector.X, k].active)
                {
                    vector.Y = k - 1;
                    break;
                }
            }
            vector.X = i;
            int num3 = (int) ((vector.X - num8) - 2f);
            int maxTilesX = (int) ((vector.X + num8) + 2f);
            int num4 = (int) ((vector.Y - num9) - 2f);
            int maxTilesY = ((int) (vector.Y + 2f)) + genRand.Next(3, 5);
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (maxTilesX > Main.maxTilesX)
            {
                maxTilesX = Main.maxTilesX;
            }
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (maxTilesY > Main.maxTilesY)
            {
                maxTilesY = Main.maxTilesY;
            }
            for (int m = num3; m <= maxTilesX; m++)
            {
                for (int num12 = num4; num12 < maxTilesY; num12++)
                {
                    Main.tile[m, num12].active = true;
                    Main.tile[m, num12].type = num;
                    Main.tile[m, num12].wall = 0;
                }
            }
            num3 = ((int) vector.X) - num8;
            maxTilesX = ((int) vector.X) + num8;
            num4 = ((int) vector.Y) - num9;
            maxTilesY = (int) (vector.Y + 1f);
            if (num3 < 0)
            {
                num3 = 0;
            }
            if (maxTilesX > Main.maxTilesX)
            {
                maxTilesX = Main.maxTilesX;
            }
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (maxTilesY > Main.maxTilesY)
            {
                maxTilesY = Main.maxTilesY;
            }
            for (int n = num3; n <= maxTilesX; n++)
            {
                for (int num14 = num4; num14 < maxTilesY; num14++)
                {
                    if (Main.tile[n, num14].wall == 0)
                    {
                        Main.tile[n, num14].active = false;
                        Main.tile[n, num14].wall = num2;
                    }
                }
            }
            int num15 = i + ((num8 + 1) * num7);
            int y = (int) vector.Y;
            for (int num17 = num15 - 2; num17 <= (num15 + 2); num17++)
            {
                Main.tile[num17, y].active = false;
                Main.tile[num17, y - 1].active = false;
                Main.tile[num17, y - 2].active = false;
            }
            PlaceTile(num15, y, 10, true, false, -1);
            int contain = 0;
            int houseCount = WorldGen.houseCount;
            if (houseCount > 2)
            {
                houseCount = genRand.Next(3);
            }
            switch (houseCount)
            {
                case 0:
                    contain = 0x9f;
                    break;

                case 1:
                    contain = 0x41;
                    break;

                case 2:
                    contain = 0x9e;
                    break;
            }
            AddBuriedChest(i, y - 3, contain);
            WorldGen.houseCount++;
        }

        public static void KillTile(int i, int j, bool fail = false, bool effectOnly = false, bool noItem = false)
        {
            if (((i >= 0) && (j >= 0)) && ((i < Main.maxTilesX) && (j < Main.maxTilesY)))
            {
                if (Main.tile[i, j] == null)
                {
                    Main.tile[i, j] = new Tile();
                }
                if (Main.tile[i, j].active)
                {
                    if ((j >= 1) && (Main.tile[i, j - 1] == null))
                    {
                        Main.tile[i, j - 1] = new Tile();
                    }
                    if ((((j < 1) || !Main.tile[i, j - 1].active) || ((((Main.tile[i, j - 1].type != 5) || (Main.tile[i, j].type == 5)) && ((Main.tile[i, j - 1].type != 0x15) || (Main.tile[i, j].type == 0x15))) && ((((Main.tile[i, j - 1].type != 0x1a) || (Main.tile[i, j].type == 0x1a)) && ((Main.tile[i, j - 1].type != 0x48) || (Main.tile[i, j].type == 0x48))) && ((Main.tile[i, j - 1].type != 12) || (Main.tile[i, j].type == 12))))) || ((Main.tile[i, j - 1].type == 5) && (((((Main.tile[i, j - 1].frameX == 0x42) && (Main.tile[i, j - 1].frameY >= 0)) && (Main.tile[i, j - 1].frameY <= 0x2c)) || (((Main.tile[i, j - 1].frameX == 0x58) && (Main.tile[i, j - 1].frameY >= 0x42)) && (Main.tile[i, j - 1].frameY <= 110))) || (Main.tile[i, j - 1].frameY >= 0xc6))))
                    {
                        if (!effectOnly && !stopDrops)
                        {
                            if (Main.tile[i, j].type == 3)
                            {
                                Main.PlaySound(6, i * 0x10, j * 0x10, 1);
                                if (Main.tile[i, j].frameX == 0x90)
                                {
                                    Item.NewItem(i * 0x10, j * 0x10, 0x10, 0x10, 5, 1, false);
                                }
                            }
                            else if (Main.tile[i, j].type == 0x18)
                            {
                                Main.PlaySound(6, i * 0x10, j * 0x10, 1);
                                if (Main.tile[i, j].frameX == 0x90)
                                {
                                    Item.NewItem(i * 0x10, j * 0x10, 0x10, 0x10, 60, 1, false);
                                }
                            }
                            else if ((((Main.tile[i, j].type == 0x20) || (Main.tile[i, j].type == 0x33)) || ((Main.tile[i, j].type == 0x34) || (Main.tile[i, j].type == 0x3d))) || (((Main.tile[i, j].type == 0x3e) || (Main.tile[i, j].type == 0x45)) || (((Main.tile[i, j].type == 0x47) || (Main.tile[i, j].type == 0x49)) || (Main.tile[i, j].type == 0x4a))))
                            {
                                Main.PlaySound(6, i * 0x10, j * 0x10, 1);
                            }
                            else if ((((((Main.tile[i, j].type == 1) || (Main.tile[i, j].type == 6)) || ((Main.tile[i, j].type == 7) || (Main.tile[i, j].type == 8))) || (((Main.tile[i, j].type == 9) || (Main.tile[i, j].type == 0x16)) || ((Main.tile[i, j].type == 0x19) || (Main.tile[i, j].type == 0x25)))) || ((((Main.tile[i, j].type == 0x26) || (Main.tile[i, j].type == 0x27)) || ((Main.tile[i, j].type == 0x29) || (Main.tile[i, j].type == 0x2b))) || (((Main.tile[i, j].type == 0x2c) || (Main.tile[i, j].type == 0x2d)) || ((Main.tile[i, j].type == 0x2e) || (Main.tile[i, j].type == 0x2f))))) || ((((Main.tile[i, j].type == 0x30) || (Main.tile[i, j].type == 0x38)) || ((Main.tile[i, j].type == 0x3a) || (Main.tile[i, j].type == 0x3f))) || ((((Main.tile[i, j].type == 0x40) || (Main.tile[i, j].type == 0x41)) || ((Main.tile[i, j].type == 0x42) || (Main.tile[i, j].type == 0x43))) || (((Main.tile[i, j].type == 0x44) || (Main.tile[i, j].type == 0x4b)) || (Main.tile[i, j].type == 0x4c)))))
                            {
                                Main.PlaySound(0x15, i * 0x10, j * 0x10, 1);
                            }
                            else
                            {
                                Main.PlaySound(0, i * 0x10, j * 0x10, 1);
                            }
                        }
                        int num = 10;
                        if (fail)
                        {
                            num = 3;
                        }
                        for (int k = 0; k < num; k++)
                        {
                            int type = 0;
                            if (Main.tile[i, j].type == 0)
                            {
                                type = 0;
                            }
                            if ((((Main.tile[i, j].type == 1) || (Main.tile[i, j].type == 0x10)) || ((Main.tile[i, j].type == 0x11) || (Main.tile[i, j].type == 0x26))) || ((((Main.tile[i, j].type == 0x27) || (Main.tile[i, j].type == 0x29)) || ((Main.tile[i, j].type == 0x2b) || (Main.tile[i, j].type == 0x2c))) || ((Main.tile[i, j].type == 0x30) || Main.tileStone[Main.tile[i, j].type])))
                            {
                                type = 1;
                            }
                            if ((Main.tile[i, j].type == 4) || (Main.tile[i, j].type == 0x21))
                            {
                                type = 6;
                            }
                            if ((((Main.tile[i, j].type == 5) || (Main.tile[i, j].type == 10)) || ((Main.tile[i, j].type == 11) || (Main.tile[i, j].type == 14))) || (((Main.tile[i, j].type == 15) || (Main.tile[i, j].type == 0x13)) || ((Main.tile[i, j].type == 0x15) || (Main.tile[i, j].type == 30))))
                            {
                                type = 7;
                            }
                            if (Main.tile[i, j].type == 2)
                            {
                                if (genRand.Next(2) == 0)
                                {
                                    type = 0;
                                }
                                else
                                {
                                    type = 2;
                                }
                            }
                            if ((Main.tile[i, j].type == 6) || (Main.tile[i, j].type == 0x1a))
                            {
                                type = 8;
                            }
                            if (((Main.tile[i, j].type == 7) || (Main.tile[i, j].type == 0x22)) || (Main.tile[i, j].type == 0x2f))
                            {
                                type = 9;
                            }
                            if (((Main.tile[i, j].type == 8) || (Main.tile[i, j].type == 0x24)) || (Main.tile[i, j].type == 0x2d))
                            {
                                type = 10;
                            }
                            if (((Main.tile[i, j].type == 9) || (Main.tile[i, j].type == 0x23)) || ((Main.tile[i, j].type == 0x2a) || (Main.tile[i, j].type == 0x2e)))
                            {
                                type = 11;
                            }
                            if (Main.tile[i, j].type == 12)
                            {
                                type = 12;
                            }
                            if ((Main.tile[i, j].type == 3) || (Main.tile[i, j].type == 0x49))
                            {
                                type = 3;
                            }
                            if ((Main.tile[i, j].type == 13) || (Main.tile[i, j].type == 0x36))
                            {
                                type = 13;
                            }
                            if (Main.tile[i, j].type == 0x16)
                            {
                                type = 14;
                            }
                            if ((Main.tile[i, j].type == 0x1c) || (Main.tile[i, j].type == 0x4e))
                            {
                                type = 0x16;
                            }
                            if (Main.tile[i, j].type == 0x1d)
                            {
                                type = 0x17;
                            }
                            if (Main.tile[i, j].type == 40)
                            {
                                type = 0x1c;
                            }
                            if (Main.tile[i, j].type == 0x31)
                            {
                                type = 0x1d;
                            }
                            if (Main.tile[i, j].type == 50)
                            {
                                type = 0x16;
                            }
                            if (Main.tile[i, j].type == 0x33)
                            {
                                type = 30;
                            }
                            if (Main.tile[i, j].type == 0x34)
                            {
                                type = 3;
                            }
                            if (Main.tile[i, j].type == 0x35)
                            {
                                type = 0x20;
                            }
                            if ((Main.tile[i, j].type == 0x38) || (Main.tile[i, j].type == 0x4b))
                            {
                                type = 0x25;
                            }
                            if (Main.tile[i, j].type == 0x39)
                            {
                                type = 0x24;
                            }
                            if (Main.tile[i, j].type == 0x3b)
                            {
                                type = 0x26;
                            }
                            if (((Main.tile[i, j].type == 0x3d) || (Main.tile[i, j].type == 0x3e)) || (Main.tile[i, j].type == 0x4a))
                            {
                                type = 40;
                            }
                            if (Main.tile[i, j].type == 0x45)
                            {
                                type = 7;
                            }
                            if ((Main.tile[i, j].type == 0x47) || (Main.tile[i, j].type == 0x48))
                            {
                                type = 0x1a;
                            }
                            if (Main.tile[i, j].type == 70)
                            {
                                type = 0x11;
                            }
                            if (Main.tile[i, j].type == 2)
                            {
                                if (genRand.Next(2) == 0)
                                {
                                    type = 0x26;
                                }
                                else
                                {
                                    type = 0x27;
                                }
                            }
                            if (((Main.tile[i, j].type == 0x3a) || (Main.tile[i, j].type == 0x4c)) || (Main.tile[i, j].type == 0x4d))
                            {
                                if (genRand.Next(2) == 0)
                                {
                                    type = 6;
                                }
                                else
                                {
                                    type = 0x19;
                                }
                            }
                            if (Main.tile[i, j].type == 0x25)
                            {
                                if (genRand.Next(2) == 0)
                                {
                                    type = 6;
                                }
                                else
                                {
                                    type = 0x17;
                                }
                            }
                            if (Main.tile[i, j].type == 0x20)
                            {
                                if (genRand.Next(2) == 0)
                                {
                                    type = 14;
                                }
                                else
                                {
                                    type = 0x18;
                                }
                            }
                            if ((Main.tile[i, j].type == 0x17) || (Main.tile[i, j].type == 0x18))
                            {
                                if (genRand.Next(2) == 0)
                                {
                                    type = 14;
                                }
                                else
                                {
                                    type = 0x11;
                                }
                            }
                            if ((Main.tile[i, j].type == 0x19) || (Main.tile[i, j].type == 0x1f))
                            {
                                if (genRand.Next(2) == 0)
                                {
                                    type = 14;
                                }
                                else
                                {
                                    type = 1;
                                }
                            }
                            if (Main.tile[i, j].type == 20)
                            {
                                if (genRand.Next(2) == 0)
                                {
                                    type = 7;
                                }
                                else
                                {
                                    type = 2;
                                }
                            }
                            if (Main.tile[i, j].type == 0x1b)
                            {
                                if (genRand.Next(2) == 0)
                                {
                                    type = 3;
                                }
                                else
                                {
                                    type = 0x13;
                                }
                            }
                            if ((((Main.tile[i, j].type == 0x22) || (Main.tile[i, j].type == 0x23)) || ((Main.tile[i, j].type == 0x24) || (Main.tile[i, j].type == 0x2a))) && (Main.rand.Next(2) == 0))
                            {
                                type = 6;
                            }
                            if (type >= 0)
                            {
                                Color newColor = new Color();
                                Dust.NewDust(new Vector2((float) (i * 0x10), (float) (j * 0x10)), 0x10, 0x10, type, 0f, 0f, 0, newColor, 1f);
                            }
                        }
                        if (!effectOnly)
                        {
                            if (fail)
                            {
                                if ((Main.tile[i, j].type == 2) || (Main.tile[i, j].type == 0x17))
                                {
                                    Main.tile[i, j].type = 0;
                                }
                                if (Main.tile[i, j].type == 60)
                                {
                                    Main.tile[i, j].type = 0x3b;
                                }
                                SquareTileFrame(i, j, true);
                            }
                            else
                            {
                                if ((Main.tile[i, j].type == 0x15) && (Main.netMode != 1))
                                {
                                    int x = i - (Main.tile[i, j].frameX / 0x12);
                                    int y = j - (Main.tile[i, j].frameY / 0x12);
                                    if (!Chest.DestroyChest(x, y))
                                    {
                                        return;
                                    }
                                }
                                if (!noItem && !stopDrops)
                                {
                                    int num6 = 0;
                                    if ((Main.tile[i, j].type == 0) || (Main.tile[i, j].type == 2))
                                    {
                                        num6 = 2;
                                    }
                                    else if (Main.tile[i, j].type == 1)
                                    {
                                        num6 = 3;
                                    }
                                    else if (Main.tile[i, j].type == 4)
                                    {
                                        num6 = 8;
                                    }
                                    else if (Main.tile[i, j].type == 5)
                                    {
                                        if ((Main.tile[i, j].frameX >= 0x16) && (Main.tile[i, j].frameY >= 0xc6))
                                        {
                                            if (genRand.Next(2) == 0)
                                            {
                                                num6 = 0x1b;
                                            }
                                            else
                                            {
                                                num6 = 9;
                                            }
                                        }
                                        else
                                        {
                                            num6 = 9;
                                        }
                                    }
                                    else if (Main.tile[i, j].type == 6)
                                    {
                                        num6 = 11;
                                    }
                                    else if (Main.tile[i, j].type == 7)
                                    {
                                        num6 = 12;
                                    }
                                    else if (Main.tile[i, j].type == 8)
                                    {
                                        num6 = 13;
                                    }
                                    else if (Main.tile[i, j].type == 9)
                                    {
                                        num6 = 14;
                                    }
                                    else if (Main.tile[i, j].type == 13)
                                    {
                                        Main.PlaySound(13, i * 0x10, j * 0x10, 1);
                                        if (Main.tile[i, j].frameX == 0x12)
                                        {
                                            num6 = 0x1c;
                                        }
                                        else if (Main.tile[i, j].frameX == 0x24)
                                        {
                                            num6 = 110;
                                        }
                                        else
                                        {
                                            num6 = 0x1f;
                                        }
                                    }
                                    else if (Main.tile[i, j].type == 0x13)
                                    {
                                        num6 = 0x5e;
                                    }
                                    else if (Main.tile[i, j].type == 0x16)
                                    {
                                        num6 = 0x38;
                                    }
                                    else if (Main.tile[i, j].type == 0x17)
                                    {
                                        num6 = 2;
                                    }
                                    else if (Main.tile[i, j].type == 0x19)
                                    {
                                        num6 = 0x3d;
                                    }
                                    else if (Main.tile[i, j].type == 30)
                                    {
                                        num6 = 9;
                                    }
                                    else if (Main.tile[i, j].type == 0x21)
                                    {
                                        num6 = 0x69;
                                    }
                                    else if (Main.tile[i, j].type == 0x25)
                                    {
                                        num6 = 0x74;
                                    }
                                    else if (Main.tile[i, j].type == 0x26)
                                    {
                                        num6 = 0x81;
                                    }
                                    else if (Main.tile[i, j].type == 0x27)
                                    {
                                        num6 = 0x83;
                                    }
                                    else if (Main.tile[i, j].type == 40)
                                    {
                                        num6 = 0x85;
                                    }
                                    else if (Main.tile[i, j].type == 0x29)
                                    {
                                        num6 = 0x86;
                                    }
                                    else if (Main.tile[i, j].type == 0x2b)
                                    {
                                        num6 = 0x89;
                                    }
                                    else if (Main.tile[i, j].type == 0x2c)
                                    {
                                        num6 = 0x8b;
                                    }
                                    else if (Main.tile[i, j].type == 0x2d)
                                    {
                                        num6 = 0x8d;
                                    }
                                    else if (Main.tile[i, j].type == 0x2e)
                                    {
                                        num6 = 0x8f;
                                    }
                                    else if (Main.tile[i, j].type == 0x2f)
                                    {
                                        num6 = 0x91;
                                    }
                                    else if (Main.tile[i, j].type == 0x30)
                                    {
                                        num6 = 0x93;
                                    }
                                    else if (Main.tile[i, j].type == 0x31)
                                    {
                                        num6 = 0x94;
                                    }
                                    else if (Main.tile[i, j].type == 0x33)
                                    {
                                        num6 = 150;
                                    }
                                    else if (Main.tile[i, j].type == 0x35)
                                    {
                                        num6 = 0xa9;
                                    }
                                    else if (Main.tile[i, j].type == 0x36)
                                    {
                                        Main.PlaySound(13, i * 0x10, j * 0x10, 1);
                                    }
                                    else if (Main.tile[i, j].type == 0x38)
                                    {
                                        num6 = 0xad;
                                    }
                                    else if (Main.tile[i, j].type == 0x39)
                                    {
                                        num6 = 0xac;
                                    }
                                    else if (Main.tile[i, j].type == 0x3a)
                                    {
                                        num6 = 0xae;
                                    }
                                    else if (Main.tile[i, j].type == 60)
                                    {
                                        num6 = 0xb0;
                                    }
                                    else if (Main.tile[i, j].type == 70)
                                    {
                                        num6 = 0xb0;
                                    }
                                    else if (Main.tile[i, j].type == 0x4b)
                                    {
                                        num6 = 0xc0;
                                    }
                                    else if (Main.tile[i, j].type == 0x4c)
                                    {
                                        num6 = 0xd6;
                                    }
                                    else if (Main.tile[i, j].type == 0x4e)
                                    {
                                        num6 = 0xde;
                                    }
                                    else if ((Main.tile[i, j].type == 0x3d) || (Main.tile[i, j].type == 0x4a))
                                    {
                                        if (Main.tile[i, j].frameX == 0xa2)
                                        {
                                            num6 = 0xdf;
                                        }
                                        else if (((Main.tile[i, j].frameX >= 0x6c) && (Main.tile[i, j].frameX <= 0x7e)) && (genRand.Next(2) == 0))
                                        {
                                            num6 = 0xd0;
                                        }
                                    }
                                    else if ((Main.tile[i, j].type == 0x3b) || (Main.tile[i, j].type == 60))
                                    {
                                        num6 = 0xb0;
                                    }
                                    else if ((Main.tile[i, j].type == 0x47) || (Main.tile[i, j].type == 0x48))
                                    {
                                        if (genRand.Next(50) == 0)
                                        {
                                            num6 = 0xc2;
                                        }
                                        else
                                        {
                                            num6 = 0xb7;
                                        }
                                    }
                                    else if ((Main.tile[i, j].type == 0x4a) && (genRand.Next(100) == 0))
                                    {
                                        num6 = 0xc3;
                                    }
                                    else if ((Main.tile[i, j].type >= 0x3f) && (Main.tile[i, j].type <= 0x44))
                                    {
                                        num6 = (Main.tile[i, j].type - 0x3f) + 0xb1;
                                    }
                                    else if (Main.tile[i, j].type == 50)
                                    {
                                        if (Main.tile[i, j].frameX == 90)
                                        {
                                            num6 = 0xa5;
                                        }
                                        else
                                        {
                                            num6 = 0x95;
                                        }
                                    }
                                    if (num6 > 0)
                                    {
                                        Item.NewItem(i * 0x10, j * 0x10, 0x10, 0x10, num6, 1, false);
                                    }
                                }
                                Main.tile[i, j].active = false;
                                if (Main.tileSolid[Main.tile[i, j].type])
                                {
                                    Main.tile[i, j].lighted = false;
                                }
                                Main.tile[i, j].frameX = -1;
                                Main.tile[i, j].frameY = -1;
                                Main.tile[i, j].frameNumber = 0;
                                Main.tile[i, j].type = 0;
                                SquareTileFrame(i, j, true);
                            }
                        }
                    }
                }
            }
        }

        public static void KillWall(int i, int j, bool fail = false)
        {
            if (((i >= 0) && (j >= 0)) && ((i < Main.maxTilesX) && (j < Main.maxTilesY)))
            {
                if (Main.tile[i, j] == null)
                {
                    Main.tile[i, j] = new Tile();
                }
                if (Main.tile[i, j].wall > 0)
                {
                    genRand.Next(3);
                    Main.PlaySound(0, i * 0x10, j * 0x10, 1);
                    int num = 10;
                    if (fail)
                    {
                        num = 3;
                    }
                    for (int k = 0; k < num; k++)
                    {
                        int type = 0;
                        if ((((Main.tile[i, j].wall == 1) || (Main.tile[i, j].wall == 5)) || ((Main.tile[i, j].wall == 6) || (Main.tile[i, j].wall == 7))) || ((Main.tile[i, j].wall == 8) || (Main.tile[i, j].wall == 9)))
                        {
                            type = 1;
                        }
                        if (Main.tile[i, j].wall == 3)
                        {
                            if (genRand.Next(2) == 0)
                            {
                                type = 14;
                            }
                            else
                            {
                                type = 1;
                            }
                        }
                        if (Main.tile[i, j].wall == 4)
                        {
                            type = 7;
                        }
                        if (Main.tile[i, j].wall == 12)
                        {
                            type = 9;
                        }
                        if (Main.tile[i, j].wall == 10)
                        {
                            type = 10;
                        }
                        if (Main.tile[i, j].wall == 11)
                        {
                            type = 11;
                        }
                        Color newColor = new Color();
                        Dust.NewDust(new Vector2((float) (i * 0x10), (float) (j * 0x10)), 0x10, 0x10, type, 0f, 0f, 0, newColor, 1f);
                    }
                    if (fail)
                    {
                        SquareWallFrame(i, j, true);
                    }
                    else
                    {
                        int num4 = 0;
                        if (Main.tile[i, j].wall == 1)
                        {
                            num4 = 0x1a;
                        }
                        if (Main.tile[i, j].wall == 4)
                        {
                            num4 = 0x5d;
                        }
                        if (Main.tile[i, j].wall == 5)
                        {
                            num4 = 130;
                        }
                        if (Main.tile[i, j].wall == 6)
                        {
                            num4 = 0x84;
                        }
                        if (Main.tile[i, j].wall == 7)
                        {
                            num4 = 0x87;
                        }
                        if (Main.tile[i, j].wall == 8)
                        {
                            num4 = 0x8a;
                        }
                        if (Main.tile[i, j].wall == 9)
                        {
                            num4 = 140;
                        }
                        if (Main.tile[i, j].wall == 10)
                        {
                            num4 = 0x8e;
                        }
                        if (Main.tile[i, j].wall == 11)
                        {
                            num4 = 0x90;
                        }
                        if (Main.tile[i, j].wall == 12)
                        {
                            num4 = 0x92;
                        }
                        if (num4 > 0)
                        {
                            Item.NewItem(i * 0x10, j * 0x10, 0x10, 0x10, num4, 1, false);
                        }
                        Main.tile[i, j].wall = 0;
                        SquareWallFrame(i, j, true);
                    }
                }
            }
        }

        public static void Lakinater(int i, int j)
        {
            Vector2 vector;
            Vector2 vector2;
            double num5 = genRand.Next(0x19, 50);
            double num6 = num5;
            float num7 = genRand.Next(30, 80);
            if (genRand.Next(5) == 0)
            {
                num5 *= 1.5;
                num6 *= 1.5;
                num7 *= 1.2f;
            }
            vector.X = i;
            vector.Y = j - (num7 * 0.3f);
            vector2.X = genRand.Next(-10, 11) * 0.1f;
            vector2.Y = genRand.Next(-20, -10) * 0.1f;
            while ((num5 > 0.0) && (num7 > 0f))
            {
                if ((vector.Y + (num6 * 0.5)) > Main.worldSurface)
                {
                    num7 = 0f;
                }
                num5 -= genRand.Next(3);
                num7--;
                int num = (int) (vector.X - (num5 * 0.5));
                int maxTilesX = (int) (vector.X + (num5 * 0.5));
                int num2 = (int) (vector.Y - (num5 * 0.5));
                int maxTilesY = (int) (vector.Y + (num5 * 0.5));
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > Main.maxTilesX)
                {
                    maxTilesX = Main.maxTilesX;
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > Main.maxTilesY)
                {
                    maxTilesY = Main.maxTilesY;
                }
                num6 = (num5 * genRand.Next(80, 120)) * 0.01;
                for (int k = num; k < maxTilesX; k++)
                {
                    for (int m = num2; m < maxTilesY; m++)
                    {
                        float num10 = Math.Abs((float) (k - vector.X));
                        float num11 = Math.Abs((float) (m - vector.Y));
                        if (Math.Sqrt((double) ((num10 * num10) + (num11 * num11))) < (num6 * 0.4))
                        {
                            if (Main.tile[k, m].active)
                            {
                                Main.tile[k, m].liquid = 0xff;
                            }
                            Main.tile[k, m].active = false;
                        }
                    }
                }
                vector += vector2;
                vector2.X += genRand.Next(-10, 11) * 0.05f;
                vector2.Y += genRand.Next(-10, 11) * 0.05f;
                if (vector2.X > 0.5)
                {
                    vector2.X = 0.5f;
                }
                if (vector2.X < -0.5)
                {
                    vector2.X = -0.5f;
                }
                if (vector2.Y > 1.5)
                {
                    vector2.Y = 1.5f;
                }
                if (vector2.Y < 0.5)
                {
                    vector2.Y = 0.5f;
                }
            }
        }

        public static void loadWorld()
        {
            if (genRand == null)
            {
                genRand = new Random((int) DateTime.Now.Ticks);
            }
            using (FileStream stream = new FileStream(Main.worldPathName, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    try
                    {
                        if (reader.ReadInt32() > Main.curRelease)
                        {
                            Main.menuMode = 15;
                            Main.statusText = "Incompatible world file!";
                            loadFailed = true;
                            reader.Close();
                            return;
                        }
                        Main.worldName = reader.ReadString();
                        Main.worldID = reader.ReadInt32();
                        Main.leftWorld = reader.ReadInt32();
                        Main.rightWorld = reader.ReadInt32();
                        Main.topWorld = reader.ReadInt32();
                        Main.bottomWorld = reader.ReadInt32();
                        Main.maxTilesY = reader.ReadInt32();
                        Main.maxTilesX = reader.ReadInt32();
                        clearWorld();
                        Main.spawnTileX = reader.ReadInt32();
                        Main.spawnTileY = reader.ReadInt32();
                        Main.worldSurface = reader.ReadDouble();
                        Main.rockLayer = reader.ReadDouble();
                        tempTime = reader.ReadDouble();
                        tempDayTime = reader.ReadBoolean();
                        tempMoonPhase = reader.ReadInt32();
                        tempBloodMoon = reader.ReadBoolean();
                        Main.dungeonX = reader.ReadInt32();
                        Main.dungeonY = reader.ReadInt32();
                        NPC.downedBoss1 = reader.ReadBoolean();
                        NPC.downedBoss2 = reader.ReadBoolean();
                        NPC.downedBoss3 = reader.ReadBoolean();
                        shadowOrbSmashed = reader.ReadBoolean();
                        spawnMeteor = reader.ReadBoolean();
                        shadowOrbCount = reader.ReadByte();
                        Main.invasionDelay = reader.ReadInt32();
                        Main.invasionSize = reader.ReadInt32();
                        Main.invasionType = reader.ReadInt32();
                        Main.invasionX = reader.ReadDouble();
                        for (int i = 0; i < Main.maxTilesX; i++)
                        {
                            float num3 = ((float) i) / ((float) Main.maxTilesX);
                            Main.statusText = "Loading world data: " + ((int) ((num3 * 100f) + 1f)) + "%";
                            for (int n = 0; n < Main.maxTilesY; n++)
                            {
                                Main.tile[i, n].active = reader.ReadBoolean();
                                if (Main.tile[i, n].active)
                                {
                                    Main.tile[i, n].type = reader.ReadByte();
                                    if (Main.tileFrameImportant[Main.tile[i, n].type])
                                    {
                                        Main.tile[i, n].frameX = reader.ReadInt16();
                                        Main.tile[i, n].frameY = reader.ReadInt16();
                                    }
                                    else
                                    {
                                        Main.tile[i, n].frameX = -1;
                                        Main.tile[i, n].frameY = -1;
                                    }
                                }
                                Main.tile[i, n].lighted = reader.ReadBoolean();
                                if (reader.ReadBoolean())
                                {
                                    Main.tile[i, n].wall = reader.ReadByte();
                                }
                                if (reader.ReadBoolean())
                                {
                                    Main.tile[i, n].liquid = reader.ReadByte();
                                    Main.tile[i, n].lava = reader.ReadBoolean();
                                }
                            }
                        }
                        for (int j = 0; j < 0x3e8; j++)
                        {
                            if (reader.ReadBoolean())
                            {
                                Main.chest[j] = new Chest();
                                Main.chest[j].x = reader.ReadInt32();
                                Main.chest[j].y = reader.ReadInt32();
                                for (int num6 = 0; num6 < Chest.maxItems; num6++)
                                {
                                    Main.chest[j].item[num6] = new Item();
                                    byte num7 = reader.ReadByte();
                                    if (num7 > 0)
                                    {
                                        string itemName = reader.ReadString();
                                        Main.chest[j].item[num6].SetDefaults(itemName);
                                        Main.chest[j].item[num6].stack = num7;
                                    }
                                }
                            }
                        }
                        for (int k = 0; k < 0x3e8; k++)
                        {
                            if (reader.ReadBoolean())
                            {
                                string str2 = reader.ReadString();
                                int num9 = reader.ReadInt32();
                                int num10 = reader.ReadInt32();
                                if (Main.tile[num9, num10].active && (Main.tile[num9, num10].type == 0x37))
                                {
                                    Main.sign[k] = new Sign();
                                    Main.sign[k].x = num9;
                                    Main.sign[k].y = num10;
                                    Main.sign[k].text = str2;
                                }
                            }
                        }
                        bool flag = reader.ReadBoolean();
                        for (int m = 0; flag; m++)
                        {
                            Main.npc[m].SetDefaults(reader.ReadString());
                            Main.npc[m].position.X = reader.ReadSingle();
                            Main.npc[m].position.Y = reader.ReadSingle();
                            Main.npc[m].homeless = reader.ReadBoolean();
                            Main.npc[m].homeTileX = reader.ReadInt32();
                            Main.npc[m].homeTileY = reader.ReadInt32();
                            flag = reader.ReadBoolean();
                        }
                        reader.Close();
                        gen = true;
                        waterLine = Main.maxTilesY;
                        Liquid.QuickWater(2, -1, -1);
                        WaterCheck();
                        int num12 = 0;
                        Liquid.quickSettle = true;
                        int num13 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                        float num14 = 0f;
                        while ((Liquid.numLiquid > 0) && (num12 < 0x186a0))
                        {
                            num12++;
                            float num15 = ((float) (num13 - (Liquid.numLiquid + LiquidBuffer.numLiquidBuffer))) / ((float) num13);
                            if ((Liquid.numLiquid + LiquidBuffer.numLiquidBuffer) > num13)
                            {
                                num13 = Liquid.numLiquid + LiquidBuffer.numLiquidBuffer;
                            }
                            if (num15 > num14)
                            {
                                num14 = num15;
                            }
                            else
                            {
                                num15 = num14;
                            }
                            Main.statusText = "Settling liquids: " + ((int) (((num15 * 100f) / 2f) + 50f)) + "%";
                            Liquid.UpdateLiquid();
                        }
                        Liquid.quickSettle = false;
                        WaterCheck();
                        gen = false;
                    }
                    catch (Exception exception)
                    {
                        Main.menuMode = 15;
                        Main.statusText = exception.ToString();
                        loadFailed = true;
                        try
                        {
                            reader.Close();
                        }
                        catch
                        {
                        }
                        return;
                    }
                    loadFailed = false;
                }
            }
        }

        public static void MakeDungeon(int x, int y, int tileType = 0x29, int wallType = 7)
        {
            int num = genRand.Next(3);
            int num2 = genRand.Next(3);
            switch (num)
            {
                case 1:
                    tileType = 0x2b;
                    break;

                case 2:
                    tileType = 0x2c;
                    break;
            }
            switch (num2)
            {
                case 1:
                    wallType = 8;
                    break;

                case 2:
                    wallType = 9;
                    break;
            }
            numDDoors = 0;
            numDPlats = 0;
            numDRooms = 0;
            WorldGen.dungeonX = x;
            WorldGen.dungeonY = y;
            dMinX = x;
            dMaxX = x;
            dMinY = y;
            dMaxY = y;
            dxStrength1 = genRand.Next(0x19, 30);
            dyStrength1 = genRand.Next(20, 0x19);
            dxStrength2 = genRand.Next(0x23, 50);
            dyStrength2 = genRand.Next(10, 15);
            float num3 = Main.maxTilesX / 60;
            num3 += genRand.Next(0, (int) (num3 / 3f));
            float num4 = num3;
            int num5 = 5;
            DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
            while (num3 > 0f)
            {
                if (WorldGen.dungeonX < dMinX)
                {
                    dMinX = WorldGen.dungeonX;
                }
                if (WorldGen.dungeonX > dMaxX)
                {
                    dMaxX = WorldGen.dungeonX;
                }
                if (WorldGen.dungeonY > dMaxY)
                {
                    dMaxY = WorldGen.dungeonY;
                }
                num3--;
                Main.statusText = "Creating dungeon: " + ((int) (((num4 - num3) / num4) * 60f)) + "%";
                if (num5 > 0)
                {
                    num5--;
                }
                if ((num5 == 0) & (genRand.Next(3) == 0))
                {
                    num5 = 5;
                    if (genRand.Next(2) == 0)
                    {
                        int dungeonX = WorldGen.dungeonX;
                        int dungeonY = WorldGen.dungeonY;
                        DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, false);
                        if (genRand.Next(2) == 0)
                        {
                            DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, false);
                        }
                        DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
                        WorldGen.dungeonX = dungeonX;
                        WorldGen.dungeonY = dungeonY;
                    }
                    else
                    {
                        DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
                    }
                }
                else
                {
                    DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, false);
                }
            }
            DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
            int num8 = dRoomX[0];
            int num9 = dRoomY[0];
            for (int i = 0; i < numDRooms; i++)
            {
                if (dRoomY[i] < num9)
                {
                    num8 = dRoomX[i];
                    num9 = dRoomY[i];
                }
            }
            WorldGen.dungeonX = num8;
            WorldGen.dungeonY = num9;
            dEnteranceX = num8;
            dSurface = false;
            num5 = 5;
            while (!dSurface)
            {
                if (num5 > 0)
                {
                    num5--;
                }
                if (((num5 == 0) & (genRand.Next(5) == 0)) && (WorldGen.dungeonY > (Main.worldSurface + 50.0)))
                {
                    num5 = 10;
                    int num11 = WorldGen.dungeonX;
                    int num12 = WorldGen.dungeonY;
                    DungeonHalls(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType, true);
                    DungeonRoom(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
                    WorldGen.dungeonX = num11;
                    WorldGen.dungeonY = num12;
                }
                DungeonStairs(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
            }
            DungeonEnt(WorldGen.dungeonX, WorldGen.dungeonY, tileType, wallType);
            Main.statusText = "Creating dungeon: 65%";
            for (int j = 0; j < numDRooms; j++)
            {
                for (int num14 = dRoomL[j]; num14 <= dRoomR[j]; num14++)
                {
                    if (!Main.tile[num14, dRoomT[j] - 1].active)
                    {
                        DPlatX[numDPlats] = num14;
                        DPlatY[numDPlats] = dRoomT[j] - 1;
                        numDPlats++;
                        break;
                    }
                }
                for (int num15 = dRoomL[j]; num15 <= dRoomR[j]; num15++)
                {
                    if (!Main.tile[num15, dRoomB[j] + 1].active)
                    {
                        DPlatX[numDPlats] = num15;
                        DPlatY[numDPlats] = dRoomB[j] + 1;
                        numDPlats++;
                        break;
                    }
                }
                for (int num16 = dRoomT[j]; num16 <= dRoomB[j]; num16++)
                {
                    if (!Main.tile[dRoomL[j] - 1, num16].active)
                    {
                        DDoorX[numDDoors] = dRoomL[j] - 1;
                        DDoorY[numDDoors] = num16;
                        DDoorPos[numDDoors] = -1;
                        numDDoors++;
                        break;
                    }
                }
                for (int num17 = dRoomT[j]; num17 <= dRoomB[j]; num17++)
                {
                    if (!Main.tile[dRoomR[j] + 1, num17].active)
                    {
                        DDoorX[numDDoors] = dRoomR[j] + 1;
                        DDoorY[numDDoors] = num17;
                        DDoorPos[numDDoors] = 1;
                        numDDoors++;
                        break;
                    }
                }
            }
            Main.statusText = "Creating dungeon: 70%";
            int num18 = 0;
            int num19 = 0x3e8;
            int num20 = 0;
            while (num20 < (Main.maxTilesX / 0x7d))
            {
                num18++;
                int num21 = genRand.Next(dMinX, dMaxX);
                int num22 = genRand.Next(((int) Main.worldSurface) + 0x19, dMaxY);
                int num23 = num21;
                if ((Main.tile[num21, num22].wall == wallType) && !Main.tile[num21, num22].active)
                {
                    int num24 = 1;
                    if (genRand.Next(2) == 0)
                    {
                        num24 = -1;
                    }
                    while (!Main.tile[num21, num22].active)
                    {
                        num22 += num24;
                    }
                    if ((Main.tile[num21 - 1, num22].active && Main.tile[num21 + 1, num22].active) && (!Main.tile[num21 - 1, num22 - num24].active && !Main.tile[num21 + 1, num22 - num24].active))
                    {
                        num20++;
                        int num25 = genRand.Next(5, 10);
                        while ((Main.tile[num21 - 1, num22].active && Main.tile[num21, num22 + num24].active) && ((Main.tile[num21, num22].active && !Main.tile[num21, num22 - num24].active) && (num25 > 0)))
                        {
                            Main.tile[num21, num22].type = 0x30;
                            if (!Main.tile[num21 - 1, num22 - num24].active && !Main.tile[num21 + 1, num22 - num24].active)
                            {
                                Main.tile[num21, num22 - num24].type = 0x30;
                                Main.tile[num21, num22 - num24].active = true;
                            }
                            num21--;
                            num25--;
                        }
                        num25 = genRand.Next(5, 10);
                        num21 = num23 + 1;
                        while ((Main.tile[num21 + 1, num22].active && Main.tile[num21, num22 + num24].active) && ((Main.tile[num21, num22].active && !Main.tile[num21, num22 - num24].active) && (num25 > 0)))
                        {
                            Main.tile[num21, num22].type = 0x30;
                            if (!Main.tile[num21 - 1, num22 - num24].active && !Main.tile[num21 + 1, num22 - num24].active)
                            {
                                Main.tile[num21, num22 - num24].type = 0x30;
                                Main.tile[num21, num22 - num24].active = true;
                            }
                            num21++;
                            num25--;
                        }
                    }
                }
                if (num18 > num19)
                {
                    num18 = 0;
                    num20++;
                }
            }
            num18 = 0;
            num19 = 0x3e8;
            num20 = 0;
            Main.statusText = "Creating dungeon: 75%";
            while (num20 < (Main.maxTilesX / 0x7d))
            {
                num18++;
                int num26 = genRand.Next(dMinX, dMaxX);
                int num27 = genRand.Next(((int) Main.worldSurface) + 0x19, dMaxY);
                int num28 = num27;
                if ((Main.tile[num26, num27].wall == wallType) && !Main.tile[num26, num27].active)
                {
                    int num29 = 1;
                    if (genRand.Next(2) == 0)
                    {
                        num29 = -1;
                    }
                    while (((num26 > 5) && (num26 < (Main.maxTilesX - 5))) && !Main.tile[num26, num27].active)
                    {
                        num26 += num29;
                    }
                    if ((Main.tile[num26, num27 - 1].active && Main.tile[num26, num27 + 1].active) && (!Main.tile[num26 - num29, num27 - 1].active && !Main.tile[num26 - num29, num27 + 1].active))
                    {
                        num20++;
                        int num30 = genRand.Next(5, 10);
                        while ((Main.tile[num26, num27 - 1].active && Main.tile[num26 + num29, num27].active) && ((Main.tile[num26, num27].active && !Main.tile[num26 - num29, num27].active) && (num30 > 0)))
                        {
                            Main.tile[num26, num27].type = 0x30;
                            if (!Main.tile[num26 - num29, num27 - 1].active && !Main.tile[num26 - num29, num27 + 1].active)
                            {
                                Main.tile[num26 - num29, num27].type = 0x30;
                                Main.tile[num26 - num29, num27].active = true;
                            }
                            num27--;
                            num30--;
                        }
                        num30 = genRand.Next(5, 10);
                        num27 = num28 + 1;
                        while ((Main.tile[num26, num27 + 1].active && Main.tile[num26 + num29, num27].active) && ((Main.tile[num26, num27].active && !Main.tile[num26 - num29, num27].active) && (num30 > 0)))
                        {
                            Main.tile[num26, num27].type = 0x30;
                            if (!Main.tile[num26 - num29, num27 - 1].active && !Main.tile[num26 - num29, num27 + 1].active)
                            {
                                Main.tile[num26 - num29, num27].type = 0x30;
                                Main.tile[num26 - num29, num27].active = true;
                            }
                            num27++;
                            num30--;
                        }
                    }
                }
                if (num18 > num19)
                {
                    num18 = 0;
                    num20++;
                }
            }
            Main.statusText = "Creating dungeon: 80%";
            for (int k = 0; k < numDDoors; k++)
            {
                int num32 = DDoorX[k] - 10;
                int num33 = DDoorX[k] + 10;
                int num34 = 100;
                int num35 = 0;
                int num36 = 0;
                int num37 = 0;
                for (int num38 = num32; num38 < num33; num38++)
                {
                    bool flag = true;
                    int num39 = DDoorY[k];
                    while (!Main.tile[num38, num39].active)
                    {
                        num39--;
                    }
                    if (!Main.tileDungeon[Main.tile[num38, num39].type])
                    {
                        flag = false;
                    }
                    num36 = num39;
                    num39 = DDoorY[k];
                    while (!Main.tile[num38, num39].active)
                    {
                        num39++;
                    }
                    if (!Main.tileDungeon[Main.tile[num38, num39].type])
                    {
                        flag = false;
                    }
                    num37 = num39;
                    if ((num37 - num36) >= 3)
                    {
                        int num40 = num38 - 20;
                        int num41 = num38 + 20;
                        int num42 = num37 - 10;
                        int num43 = num37 + 10;
                        for (int num44 = num40; num44 < num41; num44++)
                        {
                            for (int num45 = num42; num45 < num43; num45++)
                            {
                                if (Main.tile[num44, num45].active && (Main.tile[num44, num45].type == 10))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                        if (flag)
                        {
                            for (int num46 = num37 - 3; num46 < num37; num46++)
                            {
                                for (int num47 = num38 - 3; num47 <= (num38 + 3); num47++)
                                {
                                    if (Main.tile[num47, num46].active)
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (flag && ((num37 - num36) < 20))
                        {
                            bool flag2 = false;
                            if ((DDoorPos[k] == 0) && ((num37 - num36) < num34))
                            {
                                flag2 = true;
                            }
                            if ((DDoorPos[k] == -1) && (num38 > num35))
                            {
                                flag2 = true;
                            }
                            if ((DDoorPos[k] == 1) && ((num38 < num35) || (num35 == 0)))
                            {
                                flag2 = true;
                            }
                            if (flag2)
                            {
                                num35 = num38;
                                num34 = num37 - num36;
                            }
                        }
                    }
                }
                if (num34 < 20)
                {
                    int num48 = num35;
                    int num49 = DDoorY[k];
                    int num50 = num49;
                    while (!Main.tile[num48, num49].active)
                    {
                        Main.tile[num48, num49].active = false;
                        num49++;
                    }
                    while (!Main.tile[num48, num50].active)
                    {
                        num50--;
                    }
                    num49--;
                    num50++;
                    for (int num51 = num50; num51 < (num49 - 2); num51++)
                    {
                        Main.tile[num48, num51].active = true;
                        Main.tile[num48, num51].type = (byte) tileType;
                    }
                    PlaceTile(num48, num49, 10, true, false, -1);
                    num48--;
                    int num52 = num49 - 3;
                    while (!Main.tile[num48, num52].active)
                    {
                        num52--;
                    }
                    if (((num49 - num52) < ((num49 - num50) + 5)) && Main.tileDungeon[Main.tile[num48, num52].type])
                    {
                        for (int num53 = (num49 - 4) - genRand.Next(3); num53 > num52; num53--)
                        {
                            Main.tile[num48, num53].active = true;
                            Main.tile[num48, num53].type = (byte) tileType;
                        }
                    }
                    num48 += 2;
                    num52 = num49 - 3;
                    while (!Main.tile[num48, num52].active)
                    {
                        num52--;
                    }
                    if (((num49 - num52) < ((num49 - num50) + 5)) && Main.tileDungeon[Main.tile[num48, num52].type])
                    {
                        for (int num54 = (num49 - 4) - genRand.Next(3); num54 > num52; num54--)
                        {
                            Main.tile[num48, num54].active = true;
                            Main.tile[num48, num54].type = (byte) tileType;
                        }
                    }
                    num49++;
                    num48--;
                    Main.tile[num48 - 1, num49].active = true;
                    Main.tile[num48 - 1, num49].type = (byte) tileType;
                    Main.tile[num48 + 1, num49].active = true;
                    Main.tile[num48 + 1, num49].type = (byte) tileType;
                }
            }
            Main.statusText = "Creating dungeon: 85%";
            for (int m = 0; m < numDPlats; m++)
            {
                int num56 = DPlatX[m];
                int num57 = DPlatY[m];
                int maxTilesX = Main.maxTilesX;
                int num59 = 10;
                for (int num60 = num57 - 5; num60 <= (num57 + 5); num60++)
                {
                    int num61 = num56;
                    int num62 = num56;
                    bool flag3 = false;
                    if (!Main.tile[num61, num60].active)
                    {
                        goto Label_10D8;
                    }
                    flag3 = true;
                    goto Label_1128;
                Label_10B4:
                    num61--;
                    if (!Main.tileDungeon[Main.tile[num61, num60].type])
                    {
                        flag3 = true;
                    }
                Label_10D8:
                    if (!Main.tile[num61, num60].active)
                    {
                        goto Label_10B4;
                    }
                    while (!Main.tile[num62, num60].active)
                    {
                        num62++;
                        if (!Main.tileDungeon[Main.tile[num62, num60].type])
                        {
                            flag3 = true;
                        }
                    }
                Label_1128:
                    if (!flag3 && ((num62 - num61) <= num59))
                    {
                        bool flag4 = true;
                        int num63 = (num56 - (num59 / 2)) - 2;
                        int num64 = (num56 + (num59 / 2)) + 2;
                        int num65 = num60 - 5;
                        int num66 = num60 + 5;
                        for (int num67 = num63; num67 <= num64; num67++)
                        {
                            for (int num68 = num65; num68 <= num66; num68++)
                            {
                                if (Main.tile[num67, num68].active && (Main.tile[num67, num68].type == 0x13))
                                {
                                    flag4 = false;
                                    break;
                                }
                            }
                        }
                        for (int num69 = num60 + 3; num69 >= (num60 - 5); num69--)
                        {
                            if (Main.tile[num56, num69].active)
                            {
                                flag4 = false;
                                break;
                            }
                        }
                        if (flag4)
                        {
                            maxTilesX = num60;
                            break;
                        }
                    }
                }
                if ((maxTilesX > (num57 - 10)) && (maxTilesX < (num57 + 10)))
                {
                    int num70 = num56;
                    int num71 = maxTilesX;
                    int num72 = num56 + 1;
                    while (!Main.tile[num70, num71].active)
                    {
                        Main.tile[num70, num71].active = true;
                        Main.tile[num70, num71].type = 0x13;
                        num70--;
                    }
                    while (!Main.tile[num72, num71].active)
                    {
                        Main.tile[num72, num71].active = true;
                        Main.tile[num72, num71].type = 0x13;
                        num72++;
                    }
                }
            }
            Main.statusText = "Creating dungeon: 90%";
            num18 = 0;
            num19 = 0x3e8;
            num20 = 0;
            while (num20 < (Main.maxTilesX / 20))
            {
                num18++;
                int num73 = genRand.Next(dMinX, dMaxX);
                int num74 = genRand.Next(dMinY, dMaxY);
                bool flag5 = true;
                if ((Main.tile[num73, num74].wall == wallType) && !Main.tile[num73, num74].active)
                {
                    int num75 = 1;
                    if (genRand.Next(2) == 0)
                    {
                        num75 = -1;
                    }
                    while (flag5 && !Main.tile[num73, num74].active)
                    {
                        num73 -= num75;
                        if ((num73 < 5) || (num73 > (Main.maxTilesX - 5)))
                        {
                            flag5 = false;
                        }
                        else if (Main.tile[num73, num74].active && !Main.tileDungeon[Main.tile[num73, num74].type])
                        {
                            flag5 = false;
                        }
                    }
                    if (((flag5 && Main.tile[num73, num74].active) && (Main.tileDungeon[Main.tile[num73, num74].type] && Main.tile[num73, num74 - 1].active)) && ((Main.tileDungeon[Main.tile[num73, num74 - 1].type] && Main.tile[num73, num74 + 1].active) && Main.tileDungeon[Main.tile[num73, num74 + 1].type]))
                    {
                        num73 += num75;
                        for (int num76 = num73 - 3; num76 <= (num73 + 3); num76++)
                        {
                            for (int num77 = num74 - 3; num77 <= (num74 + 3); num77++)
                            {
                                if (Main.tile[num76, num77].active && (Main.tile[num76, num77].type == 0x13))
                                {
                                    flag5 = false;
                                    break;
                                }
                            }
                        }
                        if (flag5 && ((!Main.tile[num73, num74 - 1].active & !Main.tile[num73, num74 - 2].active) & !Main.tile[num73, num74 - 3].active))
                        {
                            int num78 = num73;
                            int num79 = num73;
                            while (((num78 > dMinX) && (num78 < dMaxX)) && ((!Main.tile[num78, num74].active && !Main.tile[num78, num74 - 1].active) && !Main.tile[num78, num74 + 1].active))
                            {
                                num78 += num75;
                            }
                            num78 = Math.Abs((int) (num73 - num78));
                            bool flag6 = false;
                            if (genRand.Next(2) == 0)
                            {
                                flag6 = true;
                            }
                            if (num78 > 5)
                            {
                                for (int num80 = genRand.Next(1, 4); num80 > 0; num80--)
                                {
                                    Main.tile[num73, num74].active = true;
                                    Main.tile[num73, num74].type = 0x13;
                                    if (flag6)
                                    {
                                        PlaceTile(num73, num74 - 1, 50, true, false, -1);
                                        if ((genRand.Next(50) == 0) && (Main.tile[num73, num74 - 1].type == 50))
                                        {
                                            Main.tile[num73, num74 - 1].frameX = 90;
                                        }
                                    }
                                    num73 += num75;
                                }
                                num18 = 0;
                                num20++;
                                if (!flag6 && (genRand.Next(2) == 0))
                                {
                                    num73 = num79;
                                    num74--;
                                    int type = genRand.Next(2);
                                    switch (type)
                                    {
                                        case 0:
                                            type = 13;
                                            break;

                                        case 1:
                                            type = 0x31;
                                            break;
                                    }
                                    PlaceTile(num73, num74, type, true, false, -1);
                                    if (Main.tile[num73, num74].type == 13)
                                    {
                                        if (genRand.Next(2) == 0)
                                        {
                                            Main.tile[num73, num74].frameX = 0x12;
                                        }
                                        else
                                        {
                                            Main.tile[num73, num74].frameX = 0x24;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (num18 > num19)
                {
                    num18 = 0;
                    num20++;
                }
            }
            Main.statusText = "Creating dungeon: 95%";
            for (int n = 0; n < numDRooms; n++)
            {
                int num83 = 0;
                while (num83 < 0x3e8)
                {
                    int num84 = (int) (dRoomSize[n] * 0.4);
                    int num85 = dRoomX[n] + genRand.Next(-num84, num84 + 1);
                    int num86 = dRoomY[n] + genRand.Next(-num84, num84 + 1);
                    int contain = 0;
                    switch (n)
                    {
                        case 0:
                            contain = 0x71;
                            break;

                        case 1:
                            contain = 0x9b;
                            break;

                        case 2:
                            contain = 0x9c;
                            break;

                        case 3:
                            contain = 0x9d;
                            break;

                        case 4:
                            contain = 0xa3;
                            break;

                        case 5:
                            contain = 0xa4;
                            break;
                    }
                    if ((contain == 0) && (genRand.Next(2) == 0))
                    {
                        num83 = 0x3e8;
                    }
                    else
                    {
                        if (AddBuriedChest(num85, num86, contain))
                        {
                            num83 += 0x3e8;
                        }
                        num83++;
                    }
                }
            }
            dMinX -= 0x19;
            dMaxX += 0x19;
            dMinY -= 0x19;
            dMaxY += 0x19;
            if (dMinX < 0)
            {
                dMinX = 0;
            }
            if (dMaxX > Main.maxTilesX)
            {
                dMaxX = Main.maxTilesX;
            }
            if (dMinY < 0)
            {
                dMinY = 0;
            }
            if (dMaxY > Main.maxTilesY)
            {
                dMaxY = Main.maxTilesY;
            }
            num18 = 0;
            num19 = 0x3e8;
            num20 = 0;
            while (num20 < (Main.maxTilesX / 20))
            {
                num18++;
                int num89 = genRand.Next(dMinX, dMaxX);
                int num90 = genRand.Next(dMinY, dMaxY);
                if (Main.tile[num89, num90].wall == wallType)
                {
                    for (int num91 = num90; num91 > dMinY; num91--)
                    {
                        if (Main.tile[num89, num91 - 1].active && (Main.tile[num89, num91 - 1].type == tileType))
                        {
                            bool flag7 = false;
                            for (int num92 = num89 - 15; num92 < (num89 + 15); num92++)
                            {
                                for (int num93 = num91 - 15; num93 < (num91 + 15); num93++)
                                {
                                    if ((((num92 > 0) && (num92 < Main.maxTilesX)) && ((num93 > 0) && (num93 < Main.maxTilesY))) && (Main.tile[num92, num93].type == 0x2a))
                                    {
                                        flag7 = true;
                                        break;
                                    }
                                }
                            }
                            if ((Main.tile[num89 - 1, num91].active || Main.tile[num89 + 1, num91].active) || ((Main.tile[num89 - 1, num91 + 1].active || Main.tile[num89 + 1, num91 + 1].active) || Main.tile[num89, num91 + 2].active))
                            {
                                flag7 = true;
                            }
                            if (!flag7)
                            {
                                Place1x2Top(num89, num91, 0x2a);
                                if (Main.tile[num89, num91].type == 0x2a)
                                {
                                    num18 = 0;
                                    num20++;
                                }
                            }
                            break;
                        }
                    }
                }
                if (num18 > num19)
                {
                    num20++;
                    num18 = 0;
                }
            }
        }

        public static bool meteor(int i, int j)
        {
            if ((i < 50) || (i > (Main.maxTilesX - 50)))
            {
                return false;
            }
            if ((j < 50) || (j > (Main.maxTilesY - 50)))
            {
                return false;
            }
            int num = 0x19;
            Rectangle rectangle = new Rectangle((i - num) * 0x10, (j - num) * 0x10, (num * 2) * 0x10, (num * 2) * 0x10);
            for (int k = 0; k < 8; k++)
            {
                if (Main.player[k].active)
                {
                    Rectangle rectangle2 = new Rectangle(((((int) Main.player[k].position.X) + (Main.player[k].width / 2)) - (Main.screenWidth / 2)) - NPC.safeRangeX, ((((int) Main.player[k].position.Y) + (Main.player[k].height / 2)) - (Main.screenHeight / 2)) - NPC.safeRangeY, Main.screenWidth + (NPC.safeRangeX * 2), Main.screenHeight + (NPC.safeRangeY * 2));
                    if (rectangle.Intersects(rectangle2))
                    {
                        return false;
                    }
                }
            }
            for (int m = 0; m < 0x3e8; m++)
            {
                if (Main.npc[m].active)
                {
                    Rectangle rectangle3 = new Rectangle((int) Main.npc[m].position.X, (int) Main.npc[m].position.Y, Main.npc[m].width, Main.npc[m].height);
                    if (rectangle.Intersects(rectangle3))
                    {
                        return false;
                    }
                }
            }
            for (int n = i - num; n < (i + num); n++)
            {
                for (int num5 = j - num; num5 < (j + num); num5++)
                {
                    if (Main.tile[n, num5].active && (Main.tile[n, num5].type == 0x15))
                    {
                        return false;
                    }
                }
            }
            stopDrops = true;
            num = 15;
            for (int num6 = i - num; num6 < (i + num); num6++)
            {
                for (int num7 = j - num; num7 < (j + num); num7++)
                {
                    if ((num7 > ((j + Main.rand.Next(-2, 3)) - 5)) && ((Math.Abs((int) (i - num6)) + Math.Abs((int) (j - num7))) < ((num * 1.5) + Main.rand.Next(-5, 5))))
                    {
                        if (!Main.tileSolid[Main.tile[num6, num7].type])
                        {
                            Main.tile[num6, num7].active = false;
                        }
                        Main.tile[num6, num7].type = 0x25;
                    }
                }
            }
            num = 10;
            for (int num8 = i - num; num8 < (i + num); num8++)
            {
                for (int num9 = j - num; num9 < (j + num); num9++)
                {
                    if ((num9 > ((j + Main.rand.Next(-2, 3)) - 5)) && ((Math.Abs((int) (i - num8)) + Math.Abs((int) (j - num9))) < (num + Main.rand.Next(-3, 4))))
                    {
                        Main.tile[num8, num9].active = false;
                    }
                }
            }
            num = 0x10;
            for (int num10 = i - num; num10 < (i + num); num10++)
            {
                for (int num11 = j - num; num11 < (j + num); num11++)
                {
                    if ((Main.tile[num10, num11].type == 5) || (Main.tile[num10, num11].type == 0x20))
                    {
                        KillTile(num10, num11, false, false, false);
                    }
                    SquareTileFrame(num10, num11, true);
                    SquareWallFrame(num10, num11, true);
                }
            }
            num = 0x17;
            for (int num12 = i - num; num12 < (i + num); num12++)
            {
                for (int num13 = j - num; num13 < (j + num); num13++)
                {
                    if ((Main.tile[num12, num13].active && (Main.rand.Next(10) == 0)) && ((Math.Abs((int) (i - num12)) + Math.Abs((int) (j - num13))) < (num * 1.3)))
                    {
                        if ((Main.tile[num12, num13].type == 5) || (Main.tile[num12, num13].type == 0x20))
                        {
                            KillTile(num12, num13, false, false, false);
                        }
                        Main.tile[num12, num13].type = 0x25;
                        SquareTileFrame(num12, num13, true);
                    }
                }
            }
            stopDrops = false;
            if (Main.netMode == 0)
            {
                Main.NewText("A meteorite has landed!", 50, 0xff, 130);
            }
            else if (Main.netMode == 2)
            {
                NetMessage.SendData(0x19, -1, -1, "A meteorite has landed!", 8, 50f, 255f, 130f);
            }
            if (Main.netMode != 1)
            {
                NetMessage.SendTileSquare(-1, i, j, 30);
            }
            return true;
        }

        public static void Mountinater(int i, int j)
        {
            Vector2 vector;
            Vector2 vector2;
            double num5 = genRand.Next(80, 120);
            double num6 = num5;
            float num7 = genRand.Next(40, 0x37);
            vector.X = i;
            vector.Y = j + (num7 / 2f);
            vector2.X = genRand.Next(-10, 11) * 0.1f;
            vector2.Y = genRand.Next(-20, -10) * 0.1f;
            while ((num5 > 0.0) && (num7 > 0f))
            {
                num5 -= genRand.Next(4);
                num7--;
                int num = (int) (vector.X - (num5 * 0.5));
                int maxTilesX = (int) (vector.X + (num5 * 0.5));
                int num2 = (int) (vector.Y - (num5 * 0.5));
                int maxTilesY = (int) (vector.Y + (num5 * 0.5));
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > Main.maxTilesX)
                {
                    maxTilesX = Main.maxTilesX;
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > Main.maxTilesY)
                {
                    maxTilesY = Main.maxTilesY;
                }
                num6 = (num5 * genRand.Next(80, 120)) * 0.01;
                for (int k = num; k < maxTilesX; k++)
                {
                    for (int m = num2; m < maxTilesY; m++)
                    {
                        float num10 = Math.Abs((float) (k - vector.X));
                        float num11 = Math.Abs((float) (m - vector.Y));
                        if ((Math.Sqrt((double) ((num10 * num10) + (num11 * num11))) < (num6 * 0.4)) && !Main.tile[k, m].active)
                        {
                            Main.tile[k, m].active = true;
                            Main.tile[k, m].type = 0;
                        }
                    }
                }
                vector += vector2;
                vector2.X += genRand.Next(-10, 11) * 0.05f;
                vector2.Y += genRand.Next(-10, 11) * 0.05f;
                if (vector2.X > 0.5)
                {
                    vector2.X = 0.5f;
                }
                if (vector2.X < -0.5)
                {
                    vector2.X = -0.5f;
                }
                if (vector2.Y > -0.5)
                {
                    vector2.Y = -0.5f;
                }
                if (vector2.Y < -1.5)
                {
                    vector2.Y = -1.5f;
                }
            }
        }

        public static bool OpenDoor(int i, int j, int direction)
        {
            int num3;
            int num = 0;
            if (Main.tile[i, j - 1] == null)
            {
                Main.tile[i, j - 1] = new Tile();
            }
            if (Main.tile[i, j - 2] == null)
            {
                Main.tile[i, j - 2] = new Tile();
            }
            if (Main.tile[i, j + 1] == null)
            {
                Main.tile[i, j + 1] = new Tile();
            }
            if (Main.tile[i, j] == null)
            {
                Main.tile[i, j] = new Tile();
            }
            if ((Main.tile[i, j - 1].frameY == 0) && (Main.tile[i, j - 1].type == Main.tile[i, j].type))
            {
                num = j - 1;
            }
            else if ((Main.tile[i, j - 2].frameY == 0) && (Main.tile[i, j - 2].type == Main.tile[i, j].type))
            {
                num = j - 2;
            }
            else if ((Main.tile[i, j + 1].frameY == 0) && (Main.tile[i, j + 1].type == Main.tile[i, j].type))
            {
                num = j + 1;
            }
            else
            {
                num = j;
            }
            int num2 = i;
            short num4 = 0;
            if (direction == -1)
            {
                num2 = i - 1;
                num4 = 0x24;
                num3 = i - 1;
            }
            else
            {
                num2 = i;
                num3 = i + 1;
            }
            bool flag = true;
            for (int k = num; k < (num + 3); k++)
            {
                if (Main.tile[num3, k] == null)
                {
                    Main.tile[num3, k] = new Tile();
                }
                if (Main.tile[num3, k].active)
                {
                    if ((((Main.tile[num3, k].type == 3) || (Main.tile[num3, k].type == 0x18)) || ((Main.tile[num3, k].type == 0x34) || (Main.tile[num3, k].type == 0x3d))) || (((Main.tile[num3, k].type == 0x3e) || (Main.tile[num3, k].type == 0x45)) || (((Main.tile[num3, k].type == 0x47) || (Main.tile[num3, k].type == 0x49)) || (Main.tile[num3, k].type == 0x4a))))
                    {
                        KillTile(num3, k, false, false, false);
                    }
                    else
                    {
                        flag = false;
                        break;
                    }
                }
            }
            if (flag)
            {
                Main.PlaySound(8, i * 0x10, j * 0x10, 1);
                Main.tile[num2, num].active = true;
                Main.tile[num2, num].type = 11;
                Main.tile[num2, num].frameY = 0;
                Main.tile[num2, num].frameX = num4;
                if (Main.tile[num2 + 1, num] == null)
                {
                    Main.tile[num2 + 1, num] = new Tile();
                }
                Main.tile[num2 + 1, num].active = true;
                Main.tile[num2 + 1, num].type = 11;
                Main.tile[num2 + 1, num].frameY = 0;
                Main.tile[num2 + 1, num].frameX = (short) (num4 + 0x12);
                if (Main.tile[num2, num + 1] == null)
                {
                    Main.tile[num2, num + 1] = new Tile();
                }
                Main.tile[num2, num + 1].active = true;
                Main.tile[num2, num + 1].type = 11;
                Main.tile[num2, num + 1].frameY = 0x12;
                Main.tile[num2, num + 1].frameX = num4;
                if (Main.tile[num2 + 1, num + 1] == null)
                {
                    Main.tile[num2 + 1, num + 1] = new Tile();
                }
                Main.tile[num2 + 1, num + 1].active = true;
                Main.tile[num2 + 1, num + 1].type = 11;
                Main.tile[num2 + 1, num + 1].frameY = 0x12;
                Main.tile[num2 + 1, num + 1].frameX = (short) (num4 + 0x12);
                if (Main.tile[num2, num + 2] == null)
                {
                    Main.tile[num2, num + 2] = new Tile();
                }
                Main.tile[num2, num + 2].active = true;
                Main.tile[num2, num + 2].type = 11;
                Main.tile[num2, num + 2].frameY = 0x24;
                Main.tile[num2, num + 2].frameX = num4;
                if (Main.tile[num2 + 1, num + 2] == null)
                {
                    Main.tile[num2 + 1, num + 2] = new Tile();
                }
                Main.tile[num2 + 1, num + 2].active = true;
                Main.tile[num2 + 1, num + 2].type = 11;
                Main.tile[num2 + 1, num + 2].frameY = 0x24;
                Main.tile[num2 + 1, num + 2].frameX = (short) (num4 + 0x12);
                for (int m = num2 - 1; m <= (num2 + 2); m++)
                {
                    for (int n = num - 1; n <= (num + 2); n++)
                    {
                        TileFrame(m, n, false, false);
                    }
                }
            }
            return flag;
        }

        public static void Place1x2(int x, int y, int type)
        {
            short num = 0;
            if (type == 20)
            {
                num = (short) (genRand.Next(3) * 0x12);
            }
            if (Main.tile[x, y - 1] == null)
            {
                Main.tile[x, y - 1] = new Tile();
            }
            if (Main.tile[x, y + 1] == null)
            {
                Main.tile[x, y + 1] = new Tile();
            }
            if ((Main.tile[x, y + 1].active && Main.tileSolid[Main.tile[x, y + 1].type]) && !Main.tile[x, y - 1].active)
            {
                Main.tile[x, y - 1].active = true;
                Main.tile[x, y - 1].frameY = 0;
                Main.tile[x, y - 1].frameX = num;
                Main.tile[x, y - 1].type = (byte) type;
                Main.tile[x, y].active = true;
                Main.tile[x, y].frameY = 0x12;
                Main.tile[x, y].frameX = num;
                Main.tile[x, y].type = (byte) type;
            }
        }

        public static void Place1x2Top(int x, int y, int type)
        {
            short num = 0;
            if (Main.tile[x, y - 1] == null)
            {
                Main.tile[x, y - 1] = new Tile();
            }
            if (Main.tile[x, y + 1] == null)
            {
                Main.tile[x, y + 1] = new Tile();
            }
            if ((Main.tile[x, y - 1].active && Main.tileSolid[Main.tile[x, y - 1].type]) && (!Main.tileSolidTop[Main.tile[x, y - 1].type] && !Main.tile[x, y + 1].active))
            {
                Main.tile[x, y].active = true;
                Main.tile[x, y].frameY = 0;
                Main.tile[x, y].frameX = num;
                Main.tile[x, y].type = (byte) type;
                Main.tile[x, y + 1].active = true;
                Main.tile[x, y + 1].frameY = 0x12;
                Main.tile[x, y + 1].frameX = num;
                Main.tile[x, y + 1].type = (byte) type;
            }
        }

        public static void Place2x1(int x, int y, int type)
        {
            if (Main.tile[x, y] == null)
            {
                Main.tile[x, y] = new Tile();
            }
            if (Main.tile[x + 1, y] == null)
            {
                Main.tile[x + 1, y] = new Tile();
            }
            if (Main.tile[x, y + 1] == null)
            {
                Main.tile[x, y + 1] = new Tile();
            }
            if (Main.tile[x + 1, y + 1] == null)
            {
                Main.tile[x + 1, y + 1] = new Tile();
            }
            bool flag = false;
            if ((((type != 0x1d) && Main.tile[x, y + 1].active) && (Main.tile[x + 1, y + 1].active && Main.tileSolid[Main.tile[x, y + 1].type])) && ((Main.tileSolid[Main.tile[x + 1, y + 1].type] && !Main.tile[x, y].active) && !Main.tile[x + 1, y].active))
            {
                flag = true;
            }
            else if ((((type == 0x1d) && Main.tile[x, y + 1].active) && (Main.tile[x + 1, y + 1].active && Main.tileTable[Main.tile[x, y + 1].type])) && ((Main.tileTable[Main.tile[x + 1, y + 1].type] && !Main.tile[x, y].active) && !Main.tile[x + 1, y].active))
            {
                flag = true;
            }
            if (flag)
            {
                Main.tile[x, y].active = true;
                Main.tile[x, y].frameY = 0;
                Main.tile[x, y].frameX = 0;
                Main.tile[x, y].type = (byte) type;
                Main.tile[x + 1, y].active = true;
                Main.tile[x + 1, y].frameY = 0;
                Main.tile[x + 1, y].frameX = 0x12;
                Main.tile[x + 1, y].type = (byte) type;
            }
        }

        public static void Place3x2(int x, int y, int type)
        {
            if (((x >= 5) && (x <= (Main.maxTilesX - 5))) && ((y >= 5) && (y <= (Main.maxTilesY - 5))))
            {
                bool flag = true;
                for (int i = x - 1; i < (x + 2); i++)
                {
                    for (int j = y - 1; j < (y + 1); j++)
                    {
                        if (Main.tile[i, j] == null)
                        {
                            Main.tile[i, j] = new Tile();
                        }
                        if (Main.tile[i, j].active)
                        {
                            flag = false;
                        }
                    }
                    if (Main.tile[i, y + 1] == null)
                    {
                        Main.tile[i, y + 1] = new Tile();
                    }
                    if (!Main.tile[i, y + 1].active || !Main.tileSolid[Main.tile[i, y + 1].type])
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    Main.tile[x - 1, y - 1].active = true;
                    Main.tile[x - 1, y - 1].frameY = 0;
                    Main.tile[x - 1, y - 1].frameX = 0;
                    Main.tile[x - 1, y - 1].type = (byte) type;
                    Main.tile[x, y - 1].active = true;
                    Main.tile[x, y - 1].frameY = 0;
                    Main.tile[x, y - 1].frameX = 0x12;
                    Main.tile[x, y - 1].type = (byte) type;
                    Main.tile[x + 1, y - 1].active = true;
                    Main.tile[x + 1, y - 1].frameY = 0;
                    Main.tile[x + 1, y - 1].frameX = 0x24;
                    Main.tile[x + 1, y - 1].type = (byte) type;
                    Main.tile[x - 1, y].active = true;
                    Main.tile[x - 1, y].frameY = 0x12;
                    Main.tile[x - 1, y].frameX = 0;
                    Main.tile[x - 1, y].type = (byte) type;
                    Main.tile[x, y].active = true;
                    Main.tile[x, y].frameY = 0x12;
                    Main.tile[x, y].frameX = 0x12;
                    Main.tile[x, y].type = (byte) type;
                    Main.tile[x + 1, y].active = true;
                    Main.tile[x + 1, y].frameY = 0x12;
                    Main.tile[x + 1, y].frameX = 0x24;
                    Main.tile[x + 1, y].type = (byte) type;
                }
            }
        }

        public static void Place3x3(int x, int y, int type)
        {
            bool flag = true;
            for (int i = x - 1; i < (x + 2); i++)
            {
                for (int j = y; j < (y + 3); j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                    }
                    if (Main.tile[i, j].active)
                    {
                        flag = false;
                    }
                }
            }
            if (Main.tile[x, y - 1] == null)
            {
                Main.tile[x, y - 1] = new Tile();
            }
            if ((!Main.tile[x, y - 1].active || !Main.tileSolid[Main.tile[x, y - 1].type]) || Main.tileSolidTop[Main.tile[x, y - 1].type])
            {
                flag = false;
            }
            if (flag)
            {
                Main.tile[x - 1, y].active = true;
                Main.tile[x - 1, y].frameY = 0;
                Main.tile[x - 1, y].frameX = 0;
                Main.tile[x - 1, y].type = (byte) type;
                Main.tile[x, y].active = true;
                Main.tile[x, y].frameY = 0;
                Main.tile[x, y].frameX = 0x12;
                Main.tile[x, y].type = (byte) type;
                Main.tile[x + 1, y].active = true;
                Main.tile[x + 1, y].frameY = 0;
                Main.tile[x + 1, y].frameX = 0x24;
                Main.tile[x + 1, y].type = (byte) type;
                Main.tile[x - 1, y + 1].active = true;
                Main.tile[x - 1, y + 1].frameY = 0x12;
                Main.tile[x - 1, y + 1].frameX = 0;
                Main.tile[x - 1, y + 1].type = (byte) type;
                Main.tile[x, y + 1].active = true;
                Main.tile[x, y + 1].frameY = 0x12;
                Main.tile[x, y + 1].frameX = 0x12;
                Main.tile[x, y + 1].type = (byte) type;
                Main.tile[x + 1, y + 1].active = true;
                Main.tile[x + 1, y + 1].frameY = 0x12;
                Main.tile[x + 1, y + 1].frameX = 0x24;
                Main.tile[x + 1, y + 1].type = (byte) type;
                Main.tile[x - 1, y + 2].active = true;
                Main.tile[x - 1, y + 2].frameY = 0x24;
                Main.tile[x - 1, y + 2].frameX = 0;
                Main.tile[x - 1, y + 2].type = (byte) type;
                Main.tile[x, y + 2].active = true;
                Main.tile[x, y + 2].frameY = 0x24;
                Main.tile[x, y + 2].frameX = 0x12;
                Main.tile[x, y + 2].type = (byte) type;
                Main.tile[x + 1, y + 2].active = true;
                Main.tile[x + 1, y + 2].frameY = 0x24;
                Main.tile[x + 1, y + 2].frameX = 0x24;
                Main.tile[x + 1, y + 2].type = (byte) type;
            }
        }

        public static void Place4x2(int x, int y, int type, int direction = -1)
        {
            if (((x >= 5) && (x <= (Main.maxTilesX - 5))) && ((y >= 5) && (y <= (Main.maxTilesY - 5))))
            {
                bool flag = true;
                for (int i = x - 1; i < (x + 3); i++)
                {
                    for (int j = y - 1; j < (y + 1); j++)
                    {
                        if (Main.tile[i, j] == null)
                        {
                            Main.tile[i, j] = new Tile();
                        }
                        if (Main.tile[i, j].active)
                        {
                            flag = false;
                        }
                    }
                    if (Main.tile[i, y + 1] == null)
                    {
                        Main.tile[i, y + 1] = new Tile();
                    }
                    if (!Main.tile[i, y + 1].active || !Main.tileSolid[Main.tile[i, y + 1].type])
                    {
                        flag = false;
                    }
                }
                short num3 = 0;
                if (direction == 1)
                {
                    num3 = 0x48;
                }
                if (flag)
                {
                    Main.tile[x - 1, y - 1].active = true;
                    Main.tile[x - 1, y - 1].frameY = 0;
                    Main.tile[x - 1, y - 1].frameX = num3;
                    Main.tile[x - 1, y - 1].type = (byte) type;
                    Main.tile[x, y - 1].active = true;
                    Main.tile[x, y - 1].frameY = 0;
                    Main.tile[x, y - 1].frameX = (short) (0x12 + num3);
                    Main.tile[x, y - 1].type = (byte) type;
                    Main.tile[x + 1, y - 1].active = true;
                    Main.tile[x + 1, y - 1].frameY = 0;
                    Main.tile[x + 1, y - 1].frameX = (short) (0x24 + num3);
                    Main.tile[x + 1, y - 1].type = (byte) type;
                    Main.tile[x + 2, y - 1].active = true;
                    Main.tile[x + 2, y - 1].frameY = 0;
                    Main.tile[x + 2, y - 1].frameX = (short) (0x36 + num3);
                    Main.tile[x + 2, y - 1].type = (byte) type;
                    Main.tile[x - 1, y].active = true;
                    Main.tile[x - 1, y].frameY = 0x12;
                    Main.tile[x - 1, y].frameX = num3;
                    Main.tile[x - 1, y].type = (byte) type;
                    Main.tile[x, y].active = true;
                    Main.tile[x, y].frameY = 0x12;
                    Main.tile[x, y].frameX = (short) (0x12 + num3);
                    Main.tile[x, y].type = (byte) type;
                    Main.tile[x + 1, y].active = true;
                    Main.tile[x + 1, y].frameY = 0x12;
                    Main.tile[x + 1, y].frameX = (short) (0x24 + num3);
                    Main.tile[x + 1, y].type = (byte) type;
                    Main.tile[x + 2, y].active = true;
                    Main.tile[x + 2, y].frameY = 0x12;
                    Main.tile[x + 2, y].frameX = (short) (0x36 + num3);
                    Main.tile[x + 2, y].type = (byte) type;
                }
            }
        }

        public static int PlaceChest(int x, int y, int type = 0x15)
        {
            bool flag = true;
            int num = -1;
            for (int i = x; i < (x + 2); i++)
            {
                for (int j = y - 1; j < (y + 1); j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                    }
                    if (Main.tile[i, j].active)
                    {
                        flag = false;
                    }
                    if (Main.tile[i, j].lava)
                    {
                        flag = false;
                    }
                }
                if (Main.tile[i, y + 1] == null)
                {
                    Main.tile[i, y + 1] = new Tile();
                }
                if (!Main.tile[i, y + 1].active || !Main.tileSolid[Main.tile[i, y + 1].type])
                {
                    flag = false;
                }
            }
            if (flag)
            {
                num = Chest.CreateChest(x, y - 1);
                if (num == -1)
                {
                    flag = false;
                }
            }
            if (flag)
            {
                Main.tile[x, y - 1].active = true;
                Main.tile[x, y - 1].frameY = 0;
                Main.tile[x, y - 1].frameX = 0;
                Main.tile[x, y - 1].type = (byte) type;
                Main.tile[x + 1, y - 1].active = true;
                Main.tile[x + 1, y - 1].frameY = 0;
                Main.tile[x + 1, y - 1].frameX = 0x12;
                Main.tile[x + 1, y - 1].type = (byte) type;
                Main.tile[x, y].active = true;
                Main.tile[x, y].frameY = 0x12;
                Main.tile[x, y].frameX = 0;
                Main.tile[x, y].type = (byte) type;
                Main.tile[x + 1, y].active = true;
                Main.tile[x + 1, y].frameY = 0x12;
                Main.tile[x + 1, y].frameX = 0x12;
                Main.tile[x + 1, y].type = (byte) type;
            }
            return num;
        }

        public static bool PlaceDoor(int i, int j, int type)
        {
            try
            {
                if ((Main.tile[i, j - 2].active && Main.tileSolid[Main.tile[i, j - 2].type]) && (Main.tile[i, j + 2].active && Main.tileSolid[Main.tile[i, j + 2].type]))
                {
                    Main.tile[i, j - 1].active = true;
                    Main.tile[i, j - 1].type = 10;
                    Main.tile[i, j - 1].frameY = 0;
                    Main.tile[i, j - 1].frameX = (short) (genRand.Next(3) * 0x12);
                    Main.tile[i, j].active = true;
                    Main.tile[i, j].type = 10;
                    Main.tile[i, j].frameY = 0x12;
                    Main.tile[i, j].frameX = (short) (genRand.Next(3) * 0x12);
                    Main.tile[i, j + 1].active = true;
                    Main.tile[i, j + 1].type = 10;
                    Main.tile[i, j + 1].frameY = 0x24;
                    Main.tile[i, j + 1].frameX = (short) (genRand.Next(3) * 0x12);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static void PlaceOnTable1x1(int x, int y, int type)
        {
            bool flag = false;
            if (Main.tile[x, y] == null)
            {
                Main.tile[x, y] = new Tile();
            }
            if (Main.tile[x, y + 1] == null)
            {
                Main.tile[x, y + 1] = new Tile();
            }
            if ((!Main.tile[x, y].active && Main.tile[x, y + 1].active) && Main.tileTable[Main.tile[x, y + 1].type])
            {
                flag = true;
            }
            if (((type == 0x4e) && !Main.tile[x, y].active) && (Main.tile[x, y + 1].active && Main.tileSolid[Main.tile[x, y + 1].type]))
            {
                flag = true;
            }
            if (flag)
            {
                Main.tile[x, y].active = true;
                Main.tile[x, y].frameX = 0;
                Main.tile[x, y].frameY = 0;
                Main.tile[x, y].type = (byte) type;
                if (type == 50)
                {
                    Main.tile[x, y].frameX = (short) (0x12 * genRand.Next(5));
                }
            }
        }

        public static bool PlacePot(int x, int y, int type = 0x1c)
        {
            bool flag = true;
            for (int i = x; i < (x + 2); i++)
            {
                for (int k = y - 1; k < (y + 1); k++)
                {
                    if (Main.tile[i, k] == null)
                    {
                        Main.tile[i, k] = new Tile();
                    }
                    if (Main.tile[i, k].active)
                    {
                        flag = false;
                    }
                }
                if (Main.tile[i, y + 1] == null)
                {
                    Main.tile[i, y + 1] = new Tile();
                }
                if (!Main.tile[i, y + 1].active || !Main.tileSolid[Main.tile[i, y + 1].type])
                {
                    flag = false;
                }
            }
            if (!flag)
            {
                return false;
            }
            for (int j = 0; j < 2; j++)
            {
                for (int m = -1; m < 1; m++)
                {
                    int num5 = (j * 0x12) + (genRand.Next(3) * 0x24);
                    int num6 = (m + 1) * 0x12;
                    Main.tile[x + j, y + m].active = true;
                    Main.tile[x + j, y + m].frameX = (short) num5;
                    Main.tile[x + j, y + m].frameY = (short) num6;
                    Main.tile[x + j, y + m].type = (byte) type;
                }
            }
            return true;
        }

        public static bool PlaceSign(int x, int y, int type)
        {
            int num7;
            int num8;
            int num9;
            int num = x - 2;
            int num2 = x + 3;
            int num3 = y - 2;
            int num4 = y + 3;
            if (num >= 0)
            {
                if (num2 > Main.maxTilesX)
                {
                    return false;
                }
                if (num3 < 0)
                {
                    return false;
                }
                if (num4 > Main.maxTilesY)
                {
                    return false;
                }
                for (int j = num; j < num2; j++)
                {
                    for (int k = num3; k < num4; k++)
                    {
                        if (Main.tile[j, k] == null)
                        {
                            Main.tile[j, k] = new Tile();
                        }
                    }
                }
                num7 = x;
                num8 = y;
                num9 = 0;
                if ((Main.tile[x, y + 1].active && Main.tileSolid[Main.tile[x, y + 1].type]) && (Main.tile[x + 1, y + 1].active && Main.tileSolid[Main.tile[x + 1, y + 1].type]))
                {
                    num8--;
                    num9 = 0;
                    goto Label_02E8;
                }
                if (((Main.tile[x, y - 1].active && Main.tileSolid[Main.tile[x, y - 1].type]) && (!Main.tileSolidTop[Main.tile[x, y - 1].type] && Main.tile[x + 1, y - 1].active)) && (Main.tileSolid[Main.tile[x + 1, y - 1].type] && !Main.tileSolidTop[Main.tile[x + 1, y - 1].type]))
                {
                    num9 = 1;
                    goto Label_02E8;
                }
                if (((Main.tile[x - 1, y].active && Main.tileSolid[Main.tile[x - 1, y].type]) && (!Main.tileSolidTop[Main.tile[x - 1, y].type] && Main.tile[x - 1, y + 1].active)) && (Main.tileSolid[Main.tile[x - 1, y + 1].type] && !Main.tileSolidTop[Main.tile[x - 1, y + 1].type]))
                {
                    num9 = 2;
                    goto Label_02E8;
                }
                if (((Main.tile[x + 1, y].active && Main.tileSolid[Main.tile[x + 1, y].type]) && (!Main.tileSolidTop[Main.tile[x + 1, y].type] && Main.tile[x + 1, y + 1].active)) && (Main.tileSolid[Main.tile[x + 1, y + 1].type] && !Main.tileSolidTop[Main.tile[x + 1, y + 1].type]))
                {
                    num7--;
                    num9 = 3;
                    goto Label_02E8;
                }
            }
            return false;
        Label_02E8:
            if ((Main.tile[num7, num8].active || Main.tile[num7 + 1, num8].active) || (Main.tile[num7, num8 + 1].active || Main.tile[num7 + 1, num8 + 1].active))
            {
                return false;
            }
            int num10 = 0x24 * num9;
            for (int i = 0; i < 2; i++)
            {
                for (int m = 0; m < 2; m++)
                {
                    Main.tile[num7 + i, num8 + m].active = true;
                    Main.tile[num7 + i, num8 + m].type = (byte) type;
                    Main.tile[num7 + i, num8 + m].frameX = (short) (num10 + (0x12 * i));
                    Main.tile[num7 + i, num8 + m].frameY = (short) (0x12 * m);
                }
            }
            return true;
        }

        public static void PlaceSunflower(int x, int y, int type = 0x1b)
        {
            if (y <= (Main.worldSurface - 1.0))
            {
                bool flag = true;
                for (int i = x; i < (x + 2); i++)
                {
                    for (int j = y - 3; j < (y + 1); j++)
                    {
                        if (Main.tile[i, j] == null)
                        {
                            Main.tile[i, j] = new Tile();
                        }
                        if (Main.tile[i, j].active || (Main.tile[i, j].wall > 0))
                        {
                            flag = false;
                        }
                    }
                    if (Main.tile[i, y + 1] == null)
                    {
                        Main.tile[i, y + 1] = new Tile();
                    }
                    if (!Main.tile[i, y + 1].active || (Main.tile[i, y + 1].type != 2))
                    {
                        flag = false;
                    }
                }
                if (flag)
                {
                    for (int k = 0; k < 2; k++)
                    {
                        for (int m = -3; m < 1; m++)
                        {
                            int num5 = (k * 0x12) + (genRand.Next(3) * 0x24);
                            int num6 = (m + 3) * 0x12;
                            Main.tile[x + k, y + m].active = true;
                            Main.tile[x + k, y + m].frameX = (short) num5;
                            Main.tile[x + k, y + m].frameY = (short) num6;
                            Main.tile[x + k, y + m].type = (byte) type;
                        }
                    }
                }
            }
        }

        public static bool PlaceTile(int i, int j, int type, bool mute = false, bool forced = false, int plr = -1)
        {
            bool flag = false;
            if (((i >= 0) && (j >= 0)) && ((i < Main.maxTilesX) && (j < Main.maxTilesY)))
            {
                if (Main.tile[i, j] == null)
                {
                    Main.tile[i, j] = new Tile();
                }
                if (((((!forced && !Collision.EmptyTile(i, j, false)) && Main.tileSolid[type]) && (((type != 0x17) || (Main.tile[i, j].type != 0)) || !Main.tile[i, j].active)) && (((type != 2) || (Main.tile[i, j].type != 0)) || !Main.tile[i, j].active)) && ((((type != 60) || (Main.tile[i, j].type != 0x3b)) || !Main.tile[i, j].active) && (((type != 70) || (Main.tile[i, j].type != 0x3b)) || !Main.tile[i, j].active)))
                {
                    return flag;
                }
                if ((type == 0x17) && ((Main.tile[i, j].type != 0) || !Main.tile[i, j].active))
                {
                    return false;
                }
                if ((type == 2) && ((Main.tile[i, j].type != 0) || !Main.tile[i, j].active))
                {
                    return false;
                }
                if ((type == 60) && ((Main.tile[i, j].type != 0x3b) || !Main.tile[i, j].active))
                {
                    return false;
                }
                if ((Main.tile[i, j].liquid > 0) && ((((type == 3) || (type == 4)) || ((type == 20) || (type == 0x18))) || ((((type == 0x1b) || (type == 0x20)) || ((type == 0x33) || (type == 0x3d))) || (((type == 0x45) || (type == 0x48)) || (type == 0x49)))))
                {
                    return false;
                }
                Main.tile[i, j].frameY = 0;
                Main.tile[i, j].frameX = 0;
                if ((type == 3) || (type == 0x18))
                {
                    if ((((j + 1) < Main.maxTilesY) && Main.tile[i, j + 1].active) && ((((Main.tile[i, j + 1].type == 2) && (type == 3)) || ((Main.tile[i, j + 1].type == 0x17) && (type == 0x18))) || ((Main.tile[i, j + 1].type == 0x4e) && (type == 3))))
                    {
                        if ((type == 0x18) && (genRand.Next(13) == 0))
                        {
                            Main.tile[i, j].active = true;
                            Main.tile[i, j].type = 0x20;
                            SquareTileFrame(i, j, true);
                        }
                        else if (Main.tile[i, j + 1].type == 0x4e)
                        {
                            Main.tile[i, j].active = true;
                            Main.tile[i, j].type = (byte) type;
                            Main.tile[i, j].frameX = (short) ((genRand.Next(2) * 0x12) + 0x6c);
                        }
                        else if ((Main.tile[i, j].wall == 0) && (Main.tile[i, j + 1].wall == 0))
                        {
                            if (genRand.Next(50) == 0)
                            {
                                Main.tile[i, j].active = true;
                                Main.tile[i, j].type = (byte) type;
                                Main.tile[i, j].frameX = 0x90;
                            }
                            else if (genRand.Next(0x23) == 0)
                            {
                                Main.tile[i, j].active = true;
                                Main.tile[i, j].type = (byte) type;
                                Main.tile[i, j].frameX = (short) ((genRand.Next(2) * 0x12) + 0x6c);
                            }
                            else
                            {
                                Main.tile[i, j].active = true;
                                Main.tile[i, j].type = (byte) type;
                                Main.tile[i, j].frameX = (short) (genRand.Next(6) * 0x12);
                            }
                        }
                    }
                }
                else if (type == 0x3d)
                {
                    if ((((j + 1) < Main.maxTilesY) && Main.tile[i, j + 1].active) && (Main.tile[i, j + 1].type == 60))
                    {
                        if (genRand.Next(10) == 0)
                        {
                            Main.tile[i, j].active = true;
                            Main.tile[i, j].type = 0x45;
                            SquareTileFrame(i, j, true);
                        }
                        else if (genRand.Next(15) == 0)
                        {
                            Main.tile[i, j].active = true;
                            Main.tile[i, j].type = (byte) type;
                            Main.tile[i, j].frameX = 0x90;
                        }
                        else if (genRand.Next(0x3e8) == 0)
                        {
                            Main.tile[i, j].active = true;
                            Main.tile[i, j].type = (byte) type;
                            Main.tile[i, j].frameX = 0xa2;
                        }
                        else
                        {
                            Main.tile[i, j].active = true;
                            Main.tile[i, j].type = (byte) type;
                            Main.tile[i, j].frameX = (short) (genRand.Next(8) * 0x12);
                        }
                    }
                }
                else if (type == 0x47)
                {
                    if ((((j + 1) < Main.maxTilesY) && Main.tile[i, j + 1].active) && (Main.tile[i, j + 1].type == 70))
                    {
                        Main.tile[i, j].active = true;
                        Main.tile[i, j].type = (byte) type;
                        Main.tile[i, j].frameX = (short) (genRand.Next(5) * 0x12);
                    }
                }
                else if (type == 4)
                {
                    if (Main.tile[i - 1, j] == null)
                    {
                        Main.tile[i - 1, j] = new Tile();
                    }
                    if (Main.tile[i + 1, j] == null)
                    {
                        Main.tile[i + 1, j] = new Tile();
                    }
                    if (Main.tile[i, j + 1] == null)
                    {
                        Main.tile[i, j + 1] = new Tile();
                    }
                    if (((Main.tile[i - 1, j].active && (Main.tileSolid[Main.tile[i - 1, j].type] || (((Main.tile[i - 1, j].type == 5) && (Main.tile[i - 1, j - 1].type == 5)) && (Main.tile[i - 1, j + 1].type == 5)))) || (Main.tile[i + 1, j].active && (Main.tileSolid[Main.tile[i + 1, j].type] || (((Main.tile[i + 1, j].type == 5) && (Main.tile[i + 1, j - 1].type == 5)) && (Main.tile[i + 1, j + 1].type == 5))))) || (Main.tile[i, j + 1].active && Main.tileSolid[Main.tile[i, j + 1].type]))
                    {
                        Main.tile[i, j].active = true;
                        Main.tile[i, j].type = (byte) type;
                        SquareTileFrame(i, j, true);
                    }
                }
                else if (type == 10)
                {
                    if (Main.tile[i, j - 1] == null)
                    {
                        Main.tile[i, j - 1] = new Tile();
                    }
                    if (Main.tile[i, j - 2] == null)
                    {
                        Main.tile[i, j - 2] = new Tile();
                    }
                    if (Main.tile[i, j - 3] == null)
                    {
                        Main.tile[i, j - 3] = new Tile();
                    }
                    if (Main.tile[i, j + 1] == null)
                    {
                        Main.tile[i, j + 1] = new Tile();
                    }
                    if (Main.tile[i, j + 2] == null)
                    {
                        Main.tile[i, j + 2] = new Tile();
                    }
                    if (Main.tile[i, j + 3] == null)
                    {
                        Main.tile[i, j + 3] = new Tile();
                    }
                    if ((Main.tile[i, j - 1].active || Main.tile[i, j - 2].active) || (!Main.tile[i, j - 3].active || !Main.tileSolid[Main.tile[i, j - 3].type]))
                    {
                        if ((Main.tile[i, j + 1].active || Main.tile[i, j + 2].active) || (!Main.tile[i, j + 3].active || !Main.tileSolid[Main.tile[i, j + 3].type]))
                        {
                            return false;
                        }
                        PlaceDoor(i, j + 1, type);
                        SquareTileFrame(i, j, true);
                    }
                    else
                    {
                        PlaceDoor(i, j - 1, type);
                        SquareTileFrame(i, j, true);
                    }
                }
                else if (((type == 0x22) || (type == 0x23)) || (type == 0x24))
                {
                    Place3x3(i, j, type);
                    SquareTileFrame(i, j, true);
                }
                else if (((type == 13) || (type == 0x21)) || (((type == 0x31) || (type == 50)) || (type == 0x4e)))
                {
                    PlaceOnTable1x1(i, j, type);
                    SquareTileFrame(i, j, true);
                }
                else if ((type == 14) || (type == 0x1a))
                {
                    Place3x2(i, j, type);
                    SquareTileFrame(i, j, true);
                }
                else if (type == 20)
                {
                    if (Main.tile[i, j + 1] == null)
                    {
                        Main.tile[i, j + 1] = new Tile();
                    }
                    if (Main.tile[i, j + 1].active && (Main.tile[i, j + 1].type == 2))
                    {
                        Place1x2(i, j, type);
                        SquareTileFrame(i, j, true);
                    }
                }
                else if (type == 15)
                {
                    if (Main.tile[i, j - 1] == null)
                    {
                        Main.tile[i, j - 1] = new Tile();
                    }
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                    }
                    Place1x2(i, j, type);
                    SquareTileFrame(i, j, true);
                }
                else if (((type == 0x10) || (type == 0x12)) || (type == 0x1d))
                {
                    Place2x1(i, j, type);
                    SquareTileFrame(i, j, true);
                }
                else if ((type == 0x11) || (type == 0x4d))
                {
                    Place3x2(i, j, type);
                    SquareTileFrame(i, j, true);
                }
                else if (type == 0x15)
                {
                    PlaceChest(i, j, type);
                    SquareTileFrame(i, j, true);
                }
                else if (type == 0x1b)
                {
                    PlaceSunflower(i, j, 0x1b);
                    SquareTileFrame(i, j, true);
                }
                else if (type == 0x1c)
                {
                    PlacePot(i, j, 0x1c);
                    SquareTileFrame(i, j, true);
                }
                else if (type == 0x2a)
                {
                    Place1x2Top(i, j, type);
                    SquareTileFrame(i, j, true);
                }
                else if (type == 0x37)
                {
                    PlaceSign(i, j, type);
                }
                else if (type == 0x4f)
                {
                    int direction = 1;
                    if (plr > -1)
                    {
                        direction = Main.player[plr].direction;
                    }
                    Place4x2(i, j, type, direction);
                }
                else
                {
                    Main.tile[i, j].active = true;
                    Main.tile[i, j].type = (byte) type;
                }
                if (Main.tile[i, j].active && !mute)
                {
                    SquareTileFrame(i, j, true);
                    flag = true;
                    Main.PlaySound(0, i * 0x10, j * 0x10, 1);
                    if (type != 0x16)
                    {
                        return flag;
                    }
                    for (int k = 0; k < 3; k++)
                    {
                        Color newColor = new Color();
                        Dust.NewDust(new Vector2((float) (i * 0x10), (float) (j * 0x10)), 0x10, 0x10, 14, 0f, 0f, 0, newColor, 1f);
                    }
                }
            }
            return flag;
        }

        public static void PlaceWall(int i, int j, int type, bool mute = false)
        {
            if (Main.tile[i, j] == null)
            {
                Main.tile[i, j] = new Tile();
            }
            if (Main.tile[i, j].wall != type)
            {
                for (int k = i - 1; k < (i + 2); k++)
                {
                    for (int m = j - 1; m < (j + 2); m++)
                    {
                        if (Main.tile[k, m] == null)
                        {
                            Main.tile[k, m] = new Tile();
                        }
                        if ((Main.tile[k, m].wall > 0) && (Main.tile[k, m].wall != type))
                        {
                            return;
                        }
                    }
                }
                Main.tile[i, j].wall = (byte) type;
                SquareWallFrame(i, j, true);
                if (!mute)
                {
                    Main.PlaySound(0, i * 0x10, j * 0x10, 1);
                }
            }
        }

        public static void PlantCheck(int i, int j)
        {
            int num = -1;
            int type = Main.tile[i, j].type;
            int num1 = i - 1;
            int maxTilesX = Main.maxTilesX;
            int num4 = i + 1;
            int num5 = j - 1;
            if ((j + 1) >= Main.maxTilesY)
            {
                num = type;
            }
            if ((((i - 1) >= 0) && (Main.tile[i - 1, j] != null)) && Main.tile[i - 1, j].active)
            {
                byte num6 = Main.tile[i - 1, j].type;
            }
            if ((((i + 1) < Main.maxTilesX) && (Main.tile[i + 1, j] != null)) && Main.tile[i + 1, j].active)
            {
                byte num7 = Main.tile[i + 1, j].type;
            }
            if ((((j - 1) >= 0) && (Main.tile[i, j - 1] != null)) && Main.tile[i, j - 1].active)
            {
                byte num8 = Main.tile[i, j - 1].type;
            }
            if ((((j + 1) < Main.maxTilesY) && (Main.tile[i, j + 1] != null)) && Main.tile[i, j + 1].active)
            {
                num = Main.tile[i, j + 1].type;
            }
            if ((((i - 1) >= 0) && ((j - 1) >= 0)) && ((Main.tile[i - 1, j - 1] != null) && Main.tile[i - 1, j - 1].active))
            {
                byte num9 = Main.tile[i - 1, j - 1].type;
            }
            if ((((i + 1) < Main.maxTilesX) && ((j - 1) >= 0)) && ((Main.tile[i + 1, j - 1] != null) && Main.tile[i + 1, j - 1].active))
            {
                byte num10 = Main.tile[i + 1, j - 1].type;
            }
            if ((((i - 1) >= 0) && ((j + 1) < Main.maxTilesY)) && ((Main.tile[i - 1, j + 1] != null) && Main.tile[i - 1, j + 1].active))
            {
                byte num11 = Main.tile[i - 1, j + 1].type;
            }
            if ((((i + 1) < Main.maxTilesX) && ((j + 1) < Main.maxTilesY)) && ((Main.tile[i + 1, j + 1] != null) && Main.tile[i + 1, j + 1].active))
            {
                byte num12 = Main.tile[i + 1, j + 1].type;
            }
            if ((((((type == 3) && (num != 2)) && (num != 0x4e)) || ((type == 0x18) && (num != 0x17))) || (((type == 0x3d) && (num != 60)) || ((type == 0x47) && (num != 70)))) || ((((type == 0x49) && (num != 2)) && (num != 0x4e)) || ((type == 0x4a) && (num != 60))))
            {
                KillTile(i, j, false, false, false);
            }
        }

        public static bool PlayerLOS(int x, int y)
        {
            Rectangle rectangle = new Rectangle(x * 0x10, y * 0x10, 0x10, 0x10);
            for (int i = 0; i < 8; i++)
            {
                if (Main.player[i].active)
                {
                    Rectangle rectangle2 = new Rectangle((int) ((Main.player[i].position.X + (Main.player[i].width * 0.5)) - (Main.screenWidth * 0.6)), (int) ((Main.player[i].position.Y + (Main.player[i].height * 0.5)) - (Main.screenHeight * 0.6)), (int) (Main.screenWidth * 1.2), (int) (Main.screenHeight * 1.2));
                    if (rectangle.Intersects(rectangle2))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void playWorld()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.playWorldCallBack), 1);
        }

        public static void playWorldCallBack(object threadContext)
        {
            if (Main.rand == null)
            {
                Main.rand = new Random((int) DateTime.Now.Ticks);
            }
            for (int i = 0; i < 8; i++)
            {
                if (i != Main.myPlayer)
                {
                    Main.player[i].active = false;
                }
            }
            loadWorld();
            if (!loadFailed)
            {
                EveryTileFrame();
                if (Main.gameMenu)
                {
                    Main.gameMenu = false;
                }
                Main.player[Main.myPlayer].Spawn();
                Main.player[Main.myPlayer].UpdatePlayer(Main.myPlayer);
                Main.dayTime = tempDayTime;
                Main.time = tempTime;
                Main.moonPhase = tempMoonPhase;
                Main.bloodMoon = tempBloodMoon;
                Main.PlaySound(11, -1, -1, 1);
                Main.resetClouds = true;
            }
        }

        public static void QuickFindHome(int npc)
        {
            if (((Main.npc[npc].homeTileX > 10) && (Main.npc[npc].homeTileY > 10)) && ((Main.npc[npc].homeTileX < (Main.maxTilesX - 10)) && (Main.npc[npc].homeTileY < Main.maxTilesY)))
            {
                canSpawn = false;
                StartRoomCheck(Main.npc[npc].homeTileX, Main.npc[npc].homeTileY - 1);
                if (!canSpawn)
                {
                    for (int i = Main.npc[npc].homeTileX - 1; i < (Main.npc[npc].homeTileX + 2); i++)
                    {
                        for (int j = Main.npc[npc].homeTileY - 1; j < (Main.npc[npc].homeTileY + 2); j++)
                        {
                            if (StartRoomCheck(i, j))
                            {
                                break;
                            }
                        }
                    }
                }
                if (!canSpawn)
                {
                    int num3 = 10;
                    for (int k = Main.npc[npc].homeTileX - num3; k <= (Main.npc[npc].homeTileX + num3); k += 2)
                    {
                        for (int m = Main.npc[npc].homeTileY - num3; m <= (Main.npc[npc].homeTileY + num3); m += 2)
                        {
                            if (StartRoomCheck(k, m))
                            {
                                break;
                            }
                        }
                    }
                }
                if (canSpawn)
                {
                    RoomNeeds(Main.npc[npc].type);
                    if (canSpawn)
                    {
                        ScoreRoom(npc);
                    }
                    if (canSpawn && (hiScore > 0))
                    {
                        Main.npc[npc].homeTileX = bestX;
                        Main.npc[npc].homeTileY = bestY;
                        Main.npc[npc].homeless = false;
                        canSpawn = false;
                    }
                    else
                    {
                        Main.npc[npc].homeless = true;
                    }
                }
                else
                {
                    Main.npc[npc].homeless = true;
                }
            }
        }

        public static void RangeFrame(int startX, int startY, int endX, int endY)
        {
            int num = startX;
            int num2 = endX + 1;
            int num3 = startY;
            int num4 = endY + 1;
            for (int i = num - 1; i < (num2 + 1); i++)
            {
                for (int j = num3 - 1; j < (num4 + 1); j++)
                {
                    TileFrame(i, j, false, false);
                    WallFrame(i, j, false);
                }
            }
        }

        public static bool RoomNeeds(int npcType)
        {
            if (((houseTile[15] && (houseTile[14] || houseTile[0x12])) && (((houseTile[4] || houseTile[0x21]) || (houseTile[0x22] || houseTile[0x23])) || ((houseTile[0x24] || houseTile[0x2a]) || houseTile[0x31]))) && ((houseTile[10] || houseTile[11]) || houseTile[0x13]))
            {
                canSpawn = true;
            }
            else
            {
                canSpawn = false;
            }
            return canSpawn;
        }

        public static void saveAndPlay()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.saveAndPlayCallBack), 1);
        }

        public static void saveAndPlayCallBack(object threadContext)
        {
            saveWorld(false);
        }

        public static void SaveAndQuit()
        {
            Main.PlaySound(11, -1, -1, 1);
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.SaveAndQuitCallBack), 1);
        }

        public static void SaveAndQuitCallBack(object threadContext)
        {
            Main.menuMode = 10;
            Main.gameMenu = true;
            Player.SavePlayer(Main.player[Main.myPlayer], Main.playerPathName);
            if (Main.netMode == 0)
            {
                saveWorld(false);
                Main.PlaySound(10, -1, -1, 1);
            }
            else
            {
                Netplay.disconnect = true;
                Main.netMode = 0;
            }
            Main.menuMode = 0;
        }

        public static void saveToonWhilePlaying()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.saveToonWhilePlayingCallBack), 1);
        }

        public static void saveToonWhilePlayingCallBack(object threadContext)
        {
            Player.SavePlayer(Main.player[Main.myPlayer], Main.playerPathName);
        }

        public static void saveWorld(bool resetTime = false)
        {
            if (!saveLock)
            {
                saveLock = true;
                if (!Main.skipMenu)
                {
                    bool dayTime = Main.dayTime;
                    tempTime = Main.time;
                    tempMoonPhase = Main.moonPhase;
                    tempBloodMoon = Main.bloodMoon;
                    if (resetTime)
                    {
                        dayTime = true;
                        tempTime = 13500.0;
                        tempMoonPhase = 0;
                        tempBloodMoon = false;
                    }
                    if (Main.worldPathName != null)
                    {
                        string path = Main.worldPathName + ".sav";
                        using (FileStream stream = new FileStream(path, FileMode.Create))
                        {
                            using (BinaryWriter writer = new BinaryWriter(stream))
                            {
                                writer.Write(Main.curRelease);
                                writer.Write(Main.worldName);
                                writer.Write(Main.worldID);
                                writer.Write((int) Main.leftWorld);
                                writer.Write((int) Main.rightWorld);
                                writer.Write((int) Main.topWorld);
                                writer.Write((int) Main.bottomWorld);
                                writer.Write(Main.maxTilesY);
                                writer.Write(Main.maxTilesX);
                                writer.Write(Main.spawnTileX);
                                writer.Write(Main.spawnTileY);
                                writer.Write(Main.worldSurface);
                                writer.Write(Main.rockLayer);
                                writer.Write(tempTime);
                                writer.Write(dayTime);
                                writer.Write(tempMoonPhase);
                                writer.Write(tempBloodMoon);
                                writer.Write(Main.dungeonX);
                                writer.Write(Main.dungeonY);
                                writer.Write(NPC.downedBoss1);
                                writer.Write(NPC.downedBoss2);
                                writer.Write(NPC.downedBoss3);
                                writer.Write(shadowOrbSmashed);
                                writer.Write(spawnMeteor);
                                writer.Write((byte) shadowOrbCount);
                                writer.Write(Main.invasionDelay);
                                writer.Write(Main.invasionSize);
                                writer.Write(Main.invasionType);
                                writer.Write(Main.invasionX);
                                for (int i = 0; i < Main.maxTilesX; i++)
                                {
                                    float num2 = ((float) i) / ((float) Main.maxTilesX);
                                    Main.statusText = "Saving world data: " + ((int) ((num2 * 100f) + 1f)) + "%";
                                    for (int n = 0; n < Main.maxTilesY; n++)
                                    {
                                        writer.Write(Main.tile[i, n].active);
                                        if (Main.tile[i, n].active)
                                        {
                                            writer.Write(Main.tile[i, n].type);
                                            if (Main.tileFrameImportant[Main.tile[i, n].type])
                                            {
                                                writer.Write(Main.tile[i, n].frameX);
                                                writer.Write(Main.tile[i, n].frameY);
                                            }
                                        }
                                        writer.Write(Main.tile[i, n].lighted);
                                        if (Main.tile[i, n].wall > 0)
                                        {
                                            writer.Write(true);
                                            writer.Write(Main.tile[i, n].wall);
                                        }
                                        else
                                        {
                                            writer.Write(false);
                                        }
                                        if (Main.tile[i, n].liquid > 0)
                                        {
                                            writer.Write(true);
                                            writer.Write(Main.tile[i, n].liquid);
                                            writer.Write(Main.tile[i, n].lava);
                                        }
                                        else
                                        {
                                            writer.Write(false);
                                        }
                                    }
                                }
                                for (int j = 0; j < 0x3e8; j++)
                                {
                                    if (Main.chest[j] == null)
                                    {
                                        writer.Write(false);
                                    }
                                    else
                                    {
                                        writer.Write(true);
                                        writer.Write(Main.chest[j].x);
                                        writer.Write(Main.chest[j].y);
                                        for (int num5 = 0; num5 < Chest.maxItems; num5++)
                                        {
                                            writer.Write((byte) Main.chest[j].item[num5].stack);
                                            if (Main.chest[j].item[num5].stack > 0)
                                            {
                                                writer.Write(Main.chest[j].item[num5].name != null ? Main.chest[j].item[num5].name : "");
                                            }
                                        }
                                    }
                                }
                                for (int k = 0; k < 0x3e8; k++)
                                {
                                    if ((Main.sign[k] == null) || (Main.sign[k].text == null))
                                    {
                                        writer.Write(false);
                                    }
                                    else
                                    {
                                        writer.Write(true);
                                        writer.Write(Main.sign[k].text);
                                        writer.Write(Main.sign[k].x);
                                        writer.Write(Main.sign[k].y);
                                    }
                                }
                                for (int m = 0; m < 0x3e8; m++)
                                {
                                    lock (Main.npc[m])
                                    {
                                        if (Main.npc[m].active && Main.npc[m].townNPC)
                                        {
                                            writer.Write(true);
                                            writer.Write(Main.npc[m].name);
                                            writer.Write(Main.npc[m].position.X);
                                            writer.Write(Main.npc[m].position.Y);
                                            writer.Write(Main.npc[m].homeless);
                                            writer.Write(Main.npc[m].homeTileX);
                                            writer.Write(Main.npc[m].homeTileY);
                                        }
                                    }
                                }
                                writer.Write(false);
                                writer.Close();
                                Main.statusText = "Backing up world file...";
                                string destFileName = Main.worldPathName + ".bak";
                                if (File.Exists(Main.worldPathName))
                                {
                                    File.Copy(Main.worldPathName, destFileName, true);
                                }
                                File.Copy(path, Main.worldPathName, true);
                                File.Delete(path);
                            }
                        }
                        saveLock = false;
                    }
                }
            }
        }

        public static void ScoreRoom(int ignoreNPC = -1)
        {
            for (int i = 0; i < 0x3e8; i++)
            {
                if ((Main.npc[i].active && Main.npc[i].townNPC) && ((ignoreNPC != i) && !Main.npc[i].homeless))
                {
                    for (int k = 0; k < numRoomTiles; k++)
                    {
                        if ((Main.npc[i].homeTileX == roomX[k]) && (Main.npc[i].homeTileY == roomY[k]))
                        {
                            bool flag = false;
                            for (int m = 0; m < numRoomTiles; m++)
                            {
                                if ((Main.npc[i].homeTileX == roomX[m]) && ((Main.npc[i].homeTileY - 1) == roomY[m]))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                            {
                                hiScore = -1;
                                return;
                            }
                        }
                    }
                }
            }
            hiScore = 0;
            int num4 = 0;
            int num5 = 0;
            int num6 = 0;
            int num7 = ((roomX1 - ((Main.screenWidth / 2) / 0x10)) - 1) - 0x15;
            int num8 = ((roomX2 + ((Main.screenWidth / 2) / 0x10)) + 1) + 0x15;
            int num9 = ((roomY1 - ((Main.screenHeight / 2) / 0x10)) - 1) - 0x15;
            int maxTilesX = ((roomY2 + ((Main.screenHeight / 2) / 0x10)) + 1) + 0x15;
            if (num7 < 0)
            {
                num7 = 0;
            }
            if (num8 >= Main.maxTilesX)
            {
                num8 = Main.maxTilesX - 1;
            }
            if (num9 < 0)
            {
                num9 = 0;
            }
            if (maxTilesX > Main.maxTilesX)
            {
                maxTilesX = Main.maxTilesX;
            }
            for (int j = num7 + 1; j < num8; j++)
            {
                for (int n = num9 + 2; n < (maxTilesX + 2); n++)
                {
                    if (Main.tile[j, n].active)
                    {
                        if (((Main.tile[j, n].type == 0x17) || (Main.tile[j, n].type == 0x18)) || ((Main.tile[j, n].type == 0x19) || (Main.tile[j, n].type == 0x20)))
                        {
                            Main.evilTiles++;
                        }
                        else if (Main.tile[j, n].type == 0x1b)
                        {
                            Main.evilTiles -= 5;
                        }
                    }
                }
            }
            if (num6 < 50)
            {
                num6 = 0;
            }
            num5 = -num6;
            if (num5 <= -250)
            {
                hiScore = num5;
            }
            else
            {
                num7 = roomX1;
                num8 = roomX2;
                num9 = roomY1;
                maxTilesX = roomY2;
                for (int num13 = num7 + 1; num13 < num8; num13++)
                {
                    for (int num14 = num9 + 2; num14 < (maxTilesX + 2); num14++)
                    {
                        if (!Main.tile[num13, num14].active)
                        {
                            continue;
                        }
                        num4 = num5;
                        if (((Main.tileSolid[Main.tile[num13, num14].type] && !Main.tileSolidTop[Main.tile[num13, num14].type]) && (!Collision.SolidTiles(num13 - 1, num13 + 1, num14 - 3, num14 - 1) && Main.tile[num13 - 1, num14].active)) && ((Main.tileSolid[Main.tile[num13 - 1, num14].type] && Main.tile[num13 + 1, num14].active) && Main.tileSolid[Main.tile[num13 + 1, num14].type]))
                        {
                            for (int num15 = num13 - 2; num15 < (num13 + 3); num15++)
                            {
                                for (int num16 = num14 - 4; num16 < num14; num16++)
                                {
                                    if (Main.tile[num15, num16].active)
                                    {
                                        if (num15 == num13)
                                        {
                                            num4 -= 15;
                                        }
                                        else if ((Main.tile[num15, num16].type == 10) || (Main.tile[num15, num16].type == 11))
                                        {
                                            num4 -= 20;
                                        }
                                        else if (Main.tileSolid[Main.tile[num15, num16].type])
                                        {
                                            num4 -= 5;
                                        }
                                        else
                                        {
                                            num4 += 5;
                                        }
                                    }
                                }
                            }
                            if (num4 > hiScore)
                            {
                                bool flag2 = false;
                                for (int num17 = 0; num17 < numRoomTiles; num17++)
                                {
                                    if ((roomX[num17] == num13) && (roomY[num17] == num14))
                                    {
                                        flag2 = true;
                                        break;
                                    }
                                }
                                if (flag2)
                                {
                                    hiScore = num4;
                                    bestX = num13;
                                    bestY = num14;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void SectionTileFrame(int startX, int startY, int endX, int endY)
        {
            int num = startX * 200;
            int num2 = (endX + 1) * 200;
            int num3 = startY * 150;
            int num4 = (endY + 1) * 150;
            if (num < 1)
            {
                num = 1;
            }
            if (num3 < 1)
            {
                num3 = 1;
            }
            if (num > (Main.maxTilesX - 2))
            {
                num = Main.maxTilesX - 2;
            }
            if (num3 > (Main.maxTilesY - 2))
            {
                num3 = Main.maxTilesY - 2;
            }
            for (int i = num - 1; i < (num2 + 1); i++)
            {
                for (int j = num3 - 1; j < (num4 + 1); j++)
                {
                    if (Main.tile[i, j] == null)
                    {
                        Main.tile[i, j] = new Tile();
                    }
                    TileFrame(i, j, true, true);
                    WallFrame(i, j, true);
                }
            }
        }

        public static void serverLoadWorld()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(WorldGen.serverLoadWorldCallBack), 1);
            if (ShankShock.infinateInvasion) { Main.startInv(); }
        }

        public static void serverLoadWorldCallBack(object threadContext)
        {
            loadWorld();
            if (!loadFailed)
            {
                Main.PlaySound(10, -1, -1, 1);
                Netplay.StartServer();
                Main.dayTime = tempDayTime;
                Main.time = tempTime;
                Main.moonPhase = tempMoonPhase;
                Main.bloodMoon = tempBloodMoon;
            }
        }

        public static void setWorldSize()
        {
            Main.bottomWorld = Main.maxTilesY * 0x10;
            Main.rightWorld = Main.maxTilesX * 0x10;
            Main.maxSectionsX = Main.maxTilesX / 200;
            Main.maxSectionsY = Main.maxTilesY / 150;
        }

        public static void ShroomPatch(int i, int j)
        {
            Vector2 vector;
            Vector2 vector2;
            double num5 = genRand.Next(40, 70);
            double num6 = num5;
            float num7 = genRand.Next(10, 20);
            if (genRand.Next(5) == 0)
            {
                num5 *= 1.5;
                num6 *= 1.5;
                num7 *= 1.2f;
            }
            vector.X = i;
            vector.Y = j - (num7 * 0.3f);
            vector2.X = genRand.Next(-10, 11) * 0.1f;
            vector2.Y = genRand.Next(-20, -10) * 0.1f;
            while ((num5 > 0.0) && (num7 > 0f))
            {
                num5 -= genRand.Next(3);
                num7--;
                int num = (int) (vector.X - (num5 * 0.5));
                int maxTilesX = (int) (vector.X + (num5 * 0.5));
                int num2 = (int) (vector.Y - (num5 * 0.5));
                int maxTilesY = (int) (vector.Y + (num5 * 0.5));
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > Main.maxTilesX)
                {
                    maxTilesX = Main.maxTilesX;
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > Main.maxTilesY)
                {
                    maxTilesY = Main.maxTilesY;
                }
                num6 = (num5 * genRand.Next(80, 120)) * 0.01;
                for (int k = num; k < maxTilesX; k++)
                {
                    for (int m = num2; m < maxTilesY; m++)
                    {
                        float num10 = Math.Abs((float) (k - vector.X));
                        float num11 = Math.Abs((float) ((m - vector.Y) * 2.3f));
                        if (Math.Sqrt((double) ((num10 * num10) + (num11 * num11))) < (num6 * 0.4))
                        {
                            if (m < (vector.Y + (num6 * 0.05)))
                            {
                                if (Main.tile[k, m].type != 0x3b)
                                {
                                    Main.tile[k, m].active = false;
                                }
                            }
                            else
                            {
                                Main.tile[k, m].type = 0x3b;
                            }
                            Main.tile[k, m].liquid = 0;
                            Main.tile[k, m].lava = false;
                        }
                    }
                }
                vector += vector2;
                vector2.X += genRand.Next(-10, 11) * 0.05f;
                vector2.Y += genRand.Next(-10, 11) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 0.1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if (vector2.Y > 1f)
                {
                    vector2.Y = 1f;
                }
                if (vector2.Y < -1f)
                {
                    vector2.Y = -1f;
                }
            }
        }

        public static void SpawnNPC(int x, int y)
        {
            if (Main.wallHouse[Main.tile[x, y].wall])
            {
                canSpawn = true;
            }
            if ((canSpawn && StartRoomCheck(x, y)) && RoomNeeds(spawnNPC))
            {
                ScoreRoom(-1);
                if (hiScore > 0)
                {
                    int index = -1;
                    for (int i = 0; i < 0x3e8; i++)
                    {
                        if ((Main.npc[i].active && Main.npc[i].homeless) && (Main.npc[i].type == spawnNPC))
                        {
                            index = i;
                            break;
                        }
                    }
                    if (index != -1)
                    {
                        spawnNPC = 0;
                        Main.npc[index].homeTileX = WorldGen.bestX;
                        Main.npc[index].homeTileY = WorldGen.bestY;
                        Main.npc[index].homeless = false;
                    }
                    else
                    {
                        int bestX = WorldGen.bestX;
                        int bestY = WorldGen.bestY;
                        bool flag = false;
                        if (!flag)
                        {
                            flag = true;
                            Rectangle rectangle = new Rectangle((((bestX * 0x10) + 8) - (Main.screenWidth / 2)) - NPC.safeRangeX, (((bestY * 0x10) + 8) - (Main.screenHeight / 2)) - NPC.safeRangeY, Main.screenWidth + (NPC.safeRangeX * 2), Main.screenHeight + (NPC.safeRangeY * 2));
                            for (int j = 0; j < 8; j++)
                            {
                                if (Main.player[j].active)
                                {
                                    Rectangle rectangle2 = new Rectangle((int) Main.player[j].position.X, (int) Main.player[j].position.Y, Main.player[j].width, Main.player[j].height);
                                    if (rectangle2.Intersects(rectangle))
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!flag)
                        {
                            for (int k = 1; k < 500; k++)
                            {
                                for (int m = 0; m < 2; m++)
                                {
                                    if (m == 0)
                                    {
                                        bestX = WorldGen.bestX + k;
                                    }
                                    else
                                    {
                                        bestX = WorldGen.bestX - k;
                                    }
                                    if ((bestX > 10) && (bestX < (Main.maxTilesX - 10)))
                                    {
                                        int num8 = WorldGen.bestY - k;
                                        double worldSurface = WorldGen.bestY + k;
                                        if (num8 < 10)
                                        {
                                            num8 = 10;
                                        }
                                        if (worldSurface > Main.worldSurface)
                                        {
                                            worldSurface = Main.worldSurface;
                                        }
                                        for (int n = num8; n < worldSurface; n++)
                                        {
                                            bestY = n;
                                            if (Main.tile[bestX, bestY].active && Main.tileSolid[Main.tile[bestX, bestY].type])
                                            {
                                                if (!Collision.SolidTiles(bestX - 1, bestX + 1, bestY - 3, bestY - 1))
                                                {
                                                    flag = true;
                                                    Rectangle rectangle3 = new Rectangle((((bestX * 0x10) + 8) - (Main.screenWidth / 2)) - NPC.safeRangeX, (((bestY * 0x10) + 8) - (Main.screenHeight / 2)) - NPC.safeRangeY, Main.screenWidth + (NPC.safeRangeX * 2), Main.screenHeight + (NPC.safeRangeY * 2));
                                                    for (int num11 = 0; num11 < 8; num11++)
                                                    {
                                                        if (Main.player[num11].active)
                                                        {
                                                            Rectangle rectangle4 = new Rectangle((int) Main.player[num11].position.X, (int) Main.player[num11].position.Y, Main.player[num11].width, Main.player[num11].height);
                                                            if (rectangle4.Intersects(rectangle3))
                                                            {
                                                                flag = false;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    if (flag)
                                    {
                                        break;
                                    }
                                }
                                if (flag)
                                {
                                    break;
                                }
                            }
                        }
                        int num12 = NPC.NewNPC(bestX * 0x10, bestY * 0x10, spawnNPC, 1);
                        Main.npc[num12].homeTileX = WorldGen.bestX;
                        Main.npc[num12].homeTileY = WorldGen.bestY;
                        if (bestX < WorldGen.bestX)
                        {
                            Main.npc[num12].direction = 1;
                        }
                        else if (bestX > WorldGen.bestX)
                        {
                            Main.npc[num12].direction = -1;
                        }
                        Main.npc[num12].netUpdate = true;
                        if (Main.netMode == 0)
                        {
                            Main.NewText(Main.npc[num12].name + " has arrived!", 50, 0x7d, 0xff);
                        }
                        else if (Main.netMode == 2)
                        {
                            if (Main.npc[num12].name == "") { return; }
                            NetMessage.SendData(0x19, -1, -1, Main.npc[num12].name + " has arrived!", 8, 50f, 125f, 255f);
                        }
                    }
                    spawnNPC = 0;
                }
            }
        }

        public static void SpreadGrass(int i, int j, int dirt = 0, int grass = 2, bool repeat = true)
        {
            if (((Main.tile[i, j].type == dirt) && Main.tile[i, j].active) && ((j < Main.worldSurface) || (dirt == 0x3b)))
            {
                int num = i - 1;
                int maxTilesX = i + 2;
                int num3 = j - 1;
                int maxTilesY = j + 2;
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > Main.maxTilesX)
                {
                    maxTilesX = Main.maxTilesX;
                }
                if (num3 < 0)
                {
                    num3 = 0;
                }
                if (maxTilesY > Main.maxTilesY)
                {
                    maxTilesY = Main.maxTilesY;
                }
                bool flag = true;
                for (int k = num; k < maxTilesX; k++)
                {
                    for (int m = num3; m < maxTilesY; m++)
                    {
                        if (!Main.tile[k, m].active || !Main.tileSolid[Main.tile[k, m].type])
                        {
                            flag = false;
                            goto Label_00BB;
                        }
                    }
                Label_00BB:;
                }
                if (!flag && ((grass != 0x17) || (Main.tile[i, j - 1].type != 0x1b)))
                {
                    Main.tile[i, j].type = (byte) grass;
                    for (int n = num; n < maxTilesX; n++)
                    {
                        for (int num8 = num3; num8 < maxTilesY; num8++)
                        {
                            if ((Main.tile[n, num8].active && (Main.tile[n, num8].type == dirt)) && repeat)
                            {
                                SpreadGrass(n, num8, dirt, grass, true);
                            }
                        }
                    }
                }
            }
        }

        public static void SquareTileFrame(int i, int j, bool resetFrame = true)
        {
            TileFrame(i - 1, j - 1, false, false);
            TileFrame(i - 1, j, false, false);
            TileFrame(i - 1, j + 1, false, false);
            TileFrame(i, j - 1, false, false);
            TileFrame(i, j, resetFrame, false);
            TileFrame(i, j + 1, false, false);
            TileFrame(i + 1, j - 1, false, false);
            TileFrame(i + 1, j, false, false);
            TileFrame(i + 1, j + 1, false, false);
        }

        public static void SquareWallFrame(int i, int j, bool resetFrame = true)
        {
            WallFrame(i - 1, j - 1, false);
            WallFrame(i - 1, j, false);
            WallFrame(i - 1, j + 1, false);
            WallFrame(i, j - 1, false);
            WallFrame(i, j, resetFrame);
            WallFrame(i, j + 1, false);
            WallFrame(i + 1, j - 1, false);
            WallFrame(i + 1, j, false);
            WallFrame(i + 1, j + 1, false);
        }

        public static bool StartRoomCheck(int x, int y)
        {
            roomX1 = x;
            roomX2 = x;
            roomY1 = y;
            roomY2 = y;
            numRoomTiles = 0;
            for (int i = 0; i < 80; i++)
            {
                houseTile[i] = false;
            }
            canSpawn = true;
            if (Main.tile[x, y].active && Main.tileSolid[Main.tile[x, y].type])
            {
                canSpawn = false;
            }
            CheckRoom(x, y);
            if (numRoomTiles < 60)
            {
                canSpawn = false;
            }
            return canSpawn;
        }

        public static void TileFrame(int i, int j, bool resetFrame = false, bool noBreak = false)
        {
            if ((((i >= 0) && (j >= 0)) && ((i < Main.maxTilesX) && (j < Main.maxTilesY))) && (Main.tile[i, j] != null))
            {
                if (((Main.tile[i, j].liquid > 0) && (Main.netMode != 1)) && !noLiquidCheck)
                {
                    Liquid.AddWater(i, j);
                }
                if (Main.tile[i, j].active && (!noBreak || !Main.tileFrameImportant[Main.tile[i, j].type]))
                {
                    Rectangle rectangle;
                    int index = -1;
                    int num2 = -1;
                    int num3 = -1;
                    int num4 = -1;
                    int num5 = -1;
                    int num6 = -1;
                    int num7 = -1;
                    int num8 = -1;
                    int type = Main.tile[i, j].type;
                    if (Main.tileStone[type])
                    {
                        type = 1;
                    }
                    int frameX = Main.tile[i, j].frameX;
                    int frameY = Main.tile[i, j].frameY;
                    rectangle.X = -1;
                    rectangle.Y = -1;
                    if ((((type == 3) || (type == 0x18)) || ((type == 0x3d) || (type == 0x47))) || ((type == 0x49) || (type == 0x4a)))
                    {
                        PlantCheck(i, j);
                    }
                    else
                    {
                        WorldGen.mergeUp = false;
                        WorldGen.mergeDown = false;
                        WorldGen.mergeLeft = false;
                        WorldGen.mergeRight = false;
                        if ((i - 1) < 0)
                        {
                            index = type;
                            num4 = type;
                            num6 = type;
                        }
                        if ((i + 1) >= Main.maxTilesX)
                        {
                            num3 = type;
                            num5 = type;
                            num8 = type;
                        }
                        if ((j - 1) < 0)
                        {
                            index = type;
                            num2 = type;
                            num3 = type;
                        }
                        if ((j + 1) >= Main.maxTilesY)
                        {
                            num6 = type;
                            num7 = type;
                            num8 = type;
                        }
                        if ((((i - 1) >= 0) && (Main.tile[i - 1, j] != null)) && Main.tile[i - 1, j].active)
                        {
                            num4 = Main.tile[i - 1, j].type;
                        }
                        if ((((i + 1) < Main.maxTilesX) && (Main.tile[i + 1, j] != null)) && Main.tile[i + 1, j].active)
                        {
                            num5 = Main.tile[i + 1, j].type;
                        }
                        if ((((j - 1) >= 0) && (Main.tile[i, j - 1] != null)) && Main.tile[i, j - 1].active)
                        {
                            num2 = Main.tile[i, j - 1].type;
                        }
                        if ((((j + 1) < Main.maxTilesY) && (Main.tile[i, j + 1] != null)) && Main.tile[i, j + 1].active)
                        {
                            num7 = Main.tile[i, j + 1].type;
                        }
                        if ((((i - 1) >= 0) && ((j - 1) >= 0)) && ((Main.tile[i - 1, j - 1] != null) && Main.tile[i - 1, j - 1].active))
                        {
                            index = Main.tile[i - 1, j - 1].type;
                        }
                        if ((((i + 1) < Main.maxTilesX) && ((j - 1) >= 0)) && ((Main.tile[i + 1, j - 1] != null) && Main.tile[i + 1, j - 1].active))
                        {
                            num3 = Main.tile[i + 1, j - 1].type;
                        }
                        if ((((i - 1) >= 0) && ((j + 1) < Main.maxTilesY)) && ((Main.tile[i - 1, j + 1] != null) && Main.tile[i - 1, j + 1].active))
                        {
                            num6 = Main.tile[i - 1, j + 1].type;
                        }
                        if ((((i + 1) < Main.maxTilesX) && ((j + 1) < Main.maxTilesY)) && ((Main.tile[i + 1, j + 1] != null) && Main.tile[i + 1, j + 1].active))
                        {
                            num8 = Main.tile[i + 1, j + 1].type;
                        }
                        if ((num4 >= 0) && Main.tileStone[num4])
                        {
                            num4 = 1;
                        }
                        if ((num5 >= 0) && Main.tileStone[num5])
                        {
                            num5 = 1;
                        }
                        if ((num2 >= 0) && Main.tileStone[num2])
                        {
                            num2 = 1;
                        }
                        if ((num7 >= 0) && Main.tileStone[num7])
                        {
                            num7 = 1;
                        }
                        if ((index >= 0) && Main.tileStone[index])
                        {
                            index = 1;
                        }
                        if ((num3 >= 0) && Main.tileStone[num3])
                        {
                            num3 = 1;
                        }
                        if ((num6 >= 0) && Main.tileStone[num6])
                        {
                            num6 = 1;
                        }
                        if ((num8 >= 0) && Main.tileStone[num8])
                        {
                            num8 = 1;
                        }
                        if (type == 4)
                        {
                            if (((num7 >= 0) && Main.tileSolid[num7]) && !Main.tileNoAttach[num7])
                            {
                                Main.tile[i, j].frameX = 0;
                            }
                            else if ((((num4 >= 0) && Main.tileSolid[num4]) && !Main.tileNoAttach[num4]) || (((num4 == 5) && (index == 5)) && (num6 == 5)))
                            {
                                Main.tile[i, j].frameX = 0x16;
                            }
                            else if ((((num5 >= 0) && Main.tileSolid[num5]) && !Main.tileNoAttach[num5]) || (((num5 == 5) && (num3 == 5)) && (num8 == 5)))
                            {
                                Main.tile[i, j].frameX = 0x2c;
                            }
                            else
                            {
                                KillTile(i, j, false, false, false);
                            }
                        }
                        else if ((type == 12) || (type == 0x1f))
                        {
                            if (!destroyObject)
                            {
                                int num12 = i;
                                int num13 = j;
                                if (Main.tile[i, j].frameX == 0)
                                {
                                    num12 = i;
                                }
                                else
                                {
                                    num12 = i - 1;
                                }
                                if (Main.tile[i, j].frameY == 0)
                                {
                                    num13 = j;
                                }
                                else
                                {
                                    num13 = j - 1;
                                }
                                if ((((Main.tile[num12, num13] != null) && (Main.tile[num12 + 1, num13] != null)) && ((Main.tile[num12, num13 + 1] != null) && (Main.tile[num12 + 1, num13 + 1] != null))) && (((!Main.tile[num12, num13].active || (Main.tile[num12, num13].type != type)) || (!Main.tile[num12 + 1, num13].active || (Main.tile[num12 + 1, num13].type != type))) || ((!Main.tile[num12, num13 + 1].active || (Main.tile[num12, num13 + 1].type != type)) || (!Main.tile[num12 + 1, num13 + 1].active || (Main.tile[num12 + 1, num13 + 1].type != type)))))
                                {
                                    destroyObject = true;
                                    if (Main.tile[num12, num13].type == type)
                                    {
                                        KillTile(num12, num13, false, false, false);
                                    }
                                    if (Main.tile[num12 + 1, num13].type == type)
                                    {
                                        KillTile(num12 + 1, num13, false, false, false);
                                    }
                                    if (Main.tile[num12, num13 + 1].type == type)
                                    {
                                        KillTile(num12, num13 + 1, false, false, false);
                                    }
                                    if (Main.tile[num12 + 1, num13 + 1].type == type)
                                    {
                                        KillTile(num12 + 1, num13 + 1, false, false, false);
                                    }
                                    if (type == 12)
                                    {
                                        Item.NewItem(num12 * 0x10, num13 * 0x10, 0x20, 0x20, 0x1d, 1, false);
                                    }
                                    else if (type == 0x1f)
                                    {
                                        if (genRand.Next(2) == 0)
                                        {
                                            spawnMeteor = true;
                                        }
                                        int num14 = Main.rand.Next(5);
                                        if (!shadowOrbSmashed)
                                        {
                                            num14 = 0;
                                        }
                                        if (num14 == 0)
                                        {
                                            Item.NewItem(num12 * 0x10, num13 * 0x10, 0x20, 0x20, 0x60, 1, false);
                                            int stack = genRand.Next(0x19, 0x33);
                                            Item.NewItem(num12 * 0x10, num13 * 0x10, 0x20, 0x20, 0x61, stack, false);
                                        }
                                        else if (num14 == 1)
                                        {
                                            Item.NewItem(num12 * 0x10, num13 * 0x10, 0x20, 0x20, 0x40, 1, false);
                                        }
                                        else if (num14 == 2)
                                        {
                                            Item.NewItem(num12 * 0x10, num13 * 0x10, 0x20, 0x20, 0xa2, 1, false);
                                        }
                                        else if (num14 == 3)
                                        {
                                            Item.NewItem(num12 * 0x10, num13 * 0x10, 0x20, 0x20, 0x73, 1, false);
                                        }
                                        else if (num14 == 4)
                                        {
                                            Item.NewItem(num12 * 0x10, num13 * 0x10, 0x20, 0x20, 0x6f, 1, false);
                                        }
                                        shadowOrbSmashed = true;
                                        shadowOrbCount++;
                                        if (shadowOrbCount >= 3)
                                        {
                                            shadowOrbCount = 0;
                                            float num16 = num12 * 0x10;
                                            float num17 = num13 * 0x10;
                                            float num18 = -1f;
                                            int plr = 0;
                                            for (int k = 0; k < 8; k++)
                                            {
                                                float num21 = Math.Abs((float) (Main.player[k].position.X - num16)) + Math.Abs((float) (Main.player[k].position.Y - num17));
                                                if ((num21 < num18) || (num18 == -1f))
                                                {
                                                    plr = 0;
                                                    num18 = num21;
                                                }
                                            }
                                            NPC.SpawnOnPlayer(plr, 13);
                                        }
                                        else
                                        {
                                            string newText = "A horrible chill goes down your spine...";
                                            if (shadowOrbCount == 2)
                                            {
                                                newText = "Screams echo around you...";
                                            }
                                            if (Main.netMode == 0)
                                            {
                                                Main.NewText(newText, 50, 0xff, 130);
                                            }
                                            else if (Main.netMode == 2)
                                            {
                                                NetMessage.SendData(0x19, -1, -1, newText, 8, 50f, 255f, 130f);
                                            }
                                        }
                                    }
                                    Main.PlaySound(13, i * 0x10, j * 0x10, 1);
                                    destroyObject = false;
                                }
                            }
                        }
                        else
                        {
                            if (type == 0x13)
                            {
                                if ((num4 == type) && (num5 == type))
                                {
                                    if (Main.tile[i, j].frameNumber == 0)
                                    {
                                        rectangle.X = 0;
                                        rectangle.Y = 0;
                                    }
                                    if (Main.tile[i, j].frameNumber == 1)
                                    {
                                        rectangle.X = 0;
                                        rectangle.Y = 0x12;
                                    }
                                    if (Main.tile[i, j].frameNumber == 2)
                                    {
                                        rectangle.X = 0;
                                        rectangle.Y = 0x24;
                                    }
                                }
                                else if ((num4 == type) && (num5 == -1))
                                {
                                    if (Main.tile[i, j].frameNumber == 0)
                                    {
                                        rectangle.X = 0x12;
                                        rectangle.Y = 0;
                                    }
                                    if (Main.tile[i, j].frameNumber == 1)
                                    {
                                        rectangle.X = 0x12;
                                        rectangle.Y = 0x12;
                                    }
                                    if (Main.tile[i, j].frameNumber == 2)
                                    {
                                        rectangle.X = 0x12;
                                        rectangle.Y = 0x24;
                                    }
                                }
                                else if ((num4 == -1) && (num5 == type))
                                {
                                    if (Main.tile[i, j].frameNumber == 0)
                                    {
                                        rectangle.X = 0x24;
                                        rectangle.Y = 0;
                                    }
                                    if (Main.tile[i, j].frameNumber == 1)
                                    {
                                        rectangle.X = 0x24;
                                        rectangle.Y = 0x12;
                                    }
                                    if (Main.tile[i, j].frameNumber == 2)
                                    {
                                        rectangle.X = 0x24;
                                        rectangle.Y = 0x24;
                                    }
                                }
                                else if ((num4 != type) && (num5 == type))
                                {
                                    if (Main.tile[i, j].frameNumber == 0)
                                    {
                                        rectangle.X = 0x36;
                                        rectangle.Y = 0;
                                    }
                                    if (Main.tile[i, j].frameNumber == 1)
                                    {
                                        rectangle.X = 0x36;
                                        rectangle.Y = 0x12;
                                    }
                                    if (Main.tile[i, j].frameNumber == 2)
                                    {
                                        rectangle.X = 0x36;
                                        rectangle.Y = 0x24;
                                    }
                                }
                                else if ((num4 == type) && (num5 != type))
                                {
                                    if (Main.tile[i, j].frameNumber == 0)
                                    {
                                        rectangle.X = 0x48;
                                        rectangle.Y = 0;
                                    }
                                    if (Main.tile[i, j].frameNumber == 1)
                                    {
                                        rectangle.X = 0x48;
                                        rectangle.Y = 0x12;
                                    }
                                    if (Main.tile[i, j].frameNumber == 2)
                                    {
                                        rectangle.X = 0x48;
                                        rectangle.Y = 0x24;
                                    }
                                }
                                else if (((num4 != type) && (num4 != -1)) && (num5 == -1))
                                {
                                    if (Main.tile[i, j].frameNumber == 0)
                                    {
                                        rectangle.X = 0x6c;
                                        rectangle.Y = 0;
                                    }
                                    if (Main.tile[i, j].frameNumber == 1)
                                    {
                                        rectangle.X = 0x6c;
                                        rectangle.Y = 0x12;
                                    }
                                    if (Main.tile[i, j].frameNumber == 2)
                                    {
                                        rectangle.X = 0x6c;
                                        rectangle.Y = 0x24;
                                    }
                                }
                                else if (((num4 == -1) && (num5 != type)) && (num5 != -1))
                                {
                                    if (Main.tile[i, j].frameNumber == 0)
                                    {
                                        rectangle.X = 0x7e;
                                        rectangle.Y = 0;
                                    }
                                    if (Main.tile[i, j].frameNumber == 1)
                                    {
                                        rectangle.X = 0x7e;
                                        rectangle.Y = 0x12;
                                    }
                                    if (Main.tile[i, j].frameNumber == 2)
                                    {
                                        rectangle.X = 0x7e;
                                        rectangle.Y = 0x24;
                                    }
                                }
                                else
                                {
                                    if (Main.tile[i, j].frameNumber == 0)
                                    {
                                        rectangle.X = 90;
                                        rectangle.Y = 0;
                                    }
                                    if (Main.tile[i, j].frameNumber == 1)
                                    {
                                        rectangle.X = 90;
                                        rectangle.Y = 0x12;
                                    }
                                    if (Main.tile[i, j].frameNumber == 2)
                                    {
                                        rectangle.X = 90;
                                        rectangle.Y = 0x24;
                                    }
                                }
                            }
                            else
                            {
                                if (type == 10)
                                {
                                    if (!destroyObject)
                                    {
                                        int num22 = Main.tile[i, j].frameY;
                                        int num23 = j;
                                        bool flag = false;
                                        switch (num22)
                                        {
                                            case 0:
                                                num23 = j;
                                                break;

                                            case 0x12:
                                                num23 = j - 1;
                                                break;

                                            case 0x24:
                                                num23 = j - 2;
                                                break;
                                        }
                                        if (Main.tile[i, num23 - 1] == null)
                                        {
                                            Main.tile[i, num23 - 1] = new Tile();
                                        }
                                        if (Main.tile[i, num23 + 3] == null)
                                        {
                                            Main.tile[i, num23 + 3] = new Tile();
                                        }
                                        if (Main.tile[i, num23 + 2] == null)
                                        {
                                            Main.tile[i, num23 + 2] = new Tile();
                                        }
                                        if (Main.tile[i, num23 + 1] == null)
                                        {
                                            Main.tile[i, num23 + 1] = new Tile();
                                        }
                                        if (Main.tile[i, num23] == null)
                                        {
                                            Main.tile[i, num23] = new Tile();
                                        }
                                        if (!Main.tile[i, num23 - 1].active || !Main.tileSolid[Main.tile[i, num23 - 1].type])
                                        {
                                            flag = true;
                                        }
                                        if (!Main.tile[i, num23 + 3].active || !Main.tileSolid[Main.tile[i, num23 + 3].type])
                                        {
                                            flag = true;
                                        }
                                        if (!Main.tile[i, num23].active || (Main.tile[i, num23].type != type))
                                        {
                                            flag = true;
                                        }
                                        if (!Main.tile[i, num23 + 1].active || (Main.tile[i, num23 + 1].type != type))
                                        {
                                            flag = true;
                                        }
                                        if (!Main.tile[i, num23 + 2].active || (Main.tile[i, num23 + 2].type != type))
                                        {
                                            flag = true;
                                        }
                                        if (flag)
                                        {
                                            destroyObject = true;
                                            KillTile(i, num23, false, false, false);
                                            KillTile(i, num23 + 1, false, false, false);
                                            KillTile(i, num23 + 2, false, false, false);
                                            Item.NewItem(i * 0x10, j * 0x10, 0x10, 0x10, 0x19, 1, false);
                                        }
                                        destroyObject = false;
                                    }
                                    return;
                                }
                                if (type == 11)
                                {
                                    if (!destroyObject)
                                    {
                                        int num24 = 0;
                                        int num25 = i;
                                        int num26 = j;
                                        int num27 = Main.tile[i, j].frameX;
                                        int num28 = Main.tile[i, j].frameY;
                                        bool flag2 = false;
                                        switch (num27)
                                        {
                                            case 0:
                                                num25 = i;
                                                num24 = 1;
                                                break;

                                            case 0x12:
                                                num25 = i - 1;
                                                num24 = 1;
                                                break;

                                            case 0x24:
                                                num25 = i + 1;
                                                num24 = -1;
                                                break;

                                            case 0x36:
                                                num25 = i;
                                                num24 = -1;
                                                break;
                                        }
                                        if (num28 == 0)
                                        {
                                            num26 = j;
                                        }
                                        else if (num28 == 0x12)
                                        {
                                            num26 = j - 1;
                                        }
                                        else if (num28 == 0x24)
                                        {
                                            num26 = j - 2;
                                        }
                                        if (Main.tile[num25, num26 + 3] == null)
                                        {
                                            Main.tile[num25, num26 + 3] = new Tile();
                                        }
                                        if (Main.tile[num25, num26 - 1] == null)
                                        {
                                            Main.tile[num25, num26 - 1] = new Tile();
                                        }
                                        if ((!Main.tile[num25, num26 - 1].active || !Main.tileSolid[Main.tile[num25, num26 - 1].type]) || (!Main.tile[num25, num26 + 3].active || !Main.tileSolid[Main.tile[num25, num26 + 3].type]))
                                        {
                                            flag2 = true;
                                            destroyObject = true;
                                            Item.NewItem(i * 0x10, j * 0x10, 0x10, 0x10, 0x19, 1, false);
                                        }
                                        int num29 = num25;
                                        if (num24 == -1)
                                        {
                                            num29 = num25 - 1;
                                        }
                                        for (int m = num29; m < (num29 + 2); m++)
                                        {
                                            for (int n = num26; n < (num26 + 3); n++)
                                            {
                                                if (!flag2 && ((Main.tile[m, n].type != 11) || !Main.tile[m, n].active))
                                                {
                                                    destroyObject = true;
                                                    Item.NewItem(i * 0x10, j * 0x10, 0x10, 0x10, 0x19, 1, false);
                                                    flag2 = true;
                                                    m = num29;
                                                    n = num26;
                                                }
                                                if (flag2)
                                                {
                                                    KillTile(m, n, false, false, false);
                                                }
                                            }
                                        }
                                        destroyObject = false;
                                    }
                                    return;
                                }
                                if (((type == 0x22) || (type == 0x23)) || (type == 0x24))
                                {
                                    Check3x3(i, j, (byte) type);
                                    return;
                                }
                                if ((type == 15) || (type == 20))
                                {
                                    Check1x2(i, j, (byte) type);
                                    return;
                                }
                                if (((type == 14) || (type == 0x11)) || ((type == 0x1a) || (type == 0x4d)))
                                {
                                    Check3x2(i, j, (byte) type);
                                    return;
                                }
                                if (((type == 0x10) || (type == 0x12)) || (type == 0x1d))
                                {
                                    Check2x1(i, j, (byte) type);
                                    return;
                                }
                                if (((type == 13) || (type == 0x21)) || (((type == 0x31) || (type == 50)) || (type == 0x4e)))
                                {
                                    CheckOnTable1x1(i, j, (byte) type);
                                    return;
                                }
                                if (type == 0x15)
                                {
                                    CheckChest(i, j, (byte) type);
                                    return;
                                }
                                if (type == 0x1b)
                                {
                                    CheckSunflower(i, j, 0x1b);
                                    return;
                                }
                                if (type == 0x1c)
                                {
                                    CheckPot(i, j, 0x1c);
                                    return;
                                }
                                if (type == 0x2a)
                                {
                                    Check1x2Top(i, j, (byte) type);
                                    return;
                                }
                                if (type == 0x37)
                                {
                                    CheckSign(i, j, type);
                                    return;
                                }
                                if (type == 0x4f)
                                {
                                    Check4x2(i, j, type);
                                    return;
                                }
                            }
                            if (type == 0x48)
                            {
                                if ((num7 != type) && (num7 != 70))
                                {
                                    KillTile(i, j, false, false, false);
                                }
                                else if ((num2 != type) && (Main.tile[i, j].frameX == 0))
                                {
                                    Main.tile[i, j].frameNumber = (byte) genRand.Next(3);
                                    if (Main.tile[i, j].frameNumber == 0)
                                    {
                                        Main.tile[i, j].frameX = 0x12;
                                        Main.tile[i, j].frameY = 0;
                                    }
                                    if (Main.tile[i, j].frameNumber == 1)
                                    {
                                        Main.tile[i, j].frameX = 0x12;
                                        Main.tile[i, j].frameY = 0x12;
                                    }
                                    if (Main.tile[i, j].frameNumber == 2)
                                    {
                                        Main.tile[i, j].frameX = 0x12;
                                        Main.tile[i, j].frameY = 0x24;
                                    }
                                }
                            }
                            if (type == 5)
                            {
                                if (((Main.tile[i, j].frameX >= 0x16) && (Main.tile[i, j].frameX <= 0x2c)) && ((Main.tile[i, j].frameY >= 0x84) && (Main.tile[i, j].frameY <= 0xb0)))
                                {
                                    if (((num4 != type) && (num5 != type)) || (num7 != 2))
                                    {
                                        KillTile(i, j, false, false, false);
                                    }
                                }
                                else if (((((Main.tile[i, j].frameX == 0x58) && (Main.tile[i, j].frameY >= 0)) && (Main.tile[i, j].frameY <= 0x2c)) || (((Main.tile[i, j].frameX == 0x42) && (Main.tile[i, j].frameY >= 0x42)) && (Main.tile[i, j].frameY <= 130))) || ((((Main.tile[i, j].frameX == 110) && (Main.tile[i, j].frameY >= 0x42)) && (Main.tile[i, j].frameY <= 110)) || (((Main.tile[i, j].frameX == 0x84) && (Main.tile[i, j].frameY >= 0)) && (Main.tile[i, j].frameY <= 0xb0))))
                                {
                                    if ((num4 == type) && (num5 == type))
                                    {
                                        if (Main.tile[i, j].frameNumber == 0)
                                        {
                                            Main.tile[i, j].frameX = 110;
                                            Main.tile[i, j].frameY = 0x42;
                                        }
                                        if (Main.tile[i, j].frameNumber == 1)
                                        {
                                            Main.tile[i, j].frameX = 110;
                                            Main.tile[i, j].frameY = 0x58;
                                        }
                                        if (Main.tile[i, j].frameNumber == 2)
                                        {
                                            Main.tile[i, j].frameX = 110;
                                            Main.tile[i, j].frameY = 110;
                                        }
                                    }
                                    else if (num4 == type)
                                    {
                                        if (Main.tile[i, j].frameNumber == 0)
                                        {
                                            Main.tile[i, j].frameX = 0x58;
                                            Main.tile[i, j].frameY = 0;
                                        }
                                        if (Main.tile[i, j].frameNumber == 1)
                                        {
                                            Main.tile[i, j].frameX = 0x58;
                                            Main.tile[i, j].frameY = 0x16;
                                        }
                                        if (Main.tile[i, j].frameNumber == 2)
                                        {
                                            Main.tile[i, j].frameX = 0x58;
                                            Main.tile[i, j].frameY = 0x2c;
                                        }
                                    }
                                    else if (num5 == type)
                                    {
                                        if (Main.tile[i, j].frameNumber == 0)
                                        {
                                            Main.tile[i, j].frameX = 0x42;
                                            Main.tile[i, j].frameY = 0x42;
                                        }
                                        if (Main.tile[i, j].frameNumber == 1)
                                        {
                                            Main.tile[i, j].frameX = 0x42;
                                            Main.tile[i, j].frameY = 0x58;
                                        }
                                        if (Main.tile[i, j].frameNumber == 2)
                                        {
                                            Main.tile[i, j].frameX = 0x42;
                                            Main.tile[i, j].frameY = 110;
                                        }
                                    }
                                    else
                                    {
                                        if (Main.tile[i, j].frameNumber == 0)
                                        {
                                            Main.tile[i, j].frameX = 0;
                                            Main.tile[i, j].frameY = 0;
                                        }
                                        if (Main.tile[i, j].frameNumber == 1)
                                        {
                                            Main.tile[i, j].frameX = 0;
                                            Main.tile[i, j].frameY = 0x16;
                                        }
                                        if (Main.tile[i, j].frameNumber == 2)
                                        {
                                            Main.tile[i, j].frameX = 0;
                                            Main.tile[i, j].frameY = 0x2c;
                                        }
                                    }
                                }
                                if (((Main.tile[i, j].frameY >= 0x84) && (Main.tile[i, j].frameY <= 0xb0)) && (((Main.tile[i, j].frameX == 0) || (Main.tile[i, j].frameX == 0x42)) || (Main.tile[i, j].frameX == 0x58)))
                                {
                                    if (num7 != 2)
                                    {
                                        KillTile(i, j, false, false, false);
                                    }
                                    if ((num4 != type) && (num5 != type))
                                    {
                                        if (Main.tile[i, j].frameNumber == 0)
                                        {
                                            Main.tile[i, j].frameX = 0;
                                            Main.tile[i, j].frameY = 0;
                                        }
                                        if (Main.tile[i, j].frameNumber == 1)
                                        {
                                            Main.tile[i, j].frameX = 0;
                                            Main.tile[i, j].frameY = 0x16;
                                        }
                                        if (Main.tile[i, j].frameNumber == 2)
                                        {
                                            Main.tile[i, j].frameX = 0;
                                            Main.tile[i, j].frameY = 0x2c;
                                        }
                                    }
                                    else if (num4 != type)
                                    {
                                        if (Main.tile[i, j].frameNumber == 0)
                                        {
                                            Main.tile[i, j].frameX = 0;
                                            Main.tile[i, j].frameY = 0x84;
                                        }
                                        if (Main.tile[i, j].frameNumber == 1)
                                        {
                                            Main.tile[i, j].frameX = 0;
                                            Main.tile[i, j].frameY = 0x9a;
                                        }
                                        if (Main.tile[i, j].frameNumber == 2)
                                        {
                                            Main.tile[i, j].frameX = 0;
                                            Main.tile[i, j].frameY = 0xb0;
                                        }
                                    }
                                    else if (num5 != type)
                                    {
                                        if (Main.tile[i, j].frameNumber == 0)
                                        {
                                            Main.tile[i, j].frameX = 0x42;
                                            Main.tile[i, j].frameY = 0x84;
                                        }
                                        if (Main.tile[i, j].frameNumber == 1)
                                        {
                                            Main.tile[i, j].frameX = 0x42;
                                            Main.tile[i, j].frameY = 0x9a;
                                        }
                                        if (Main.tile[i, j].frameNumber == 2)
                                        {
                                            Main.tile[i, j].frameX = 0x42;
                                            Main.tile[i, j].frameY = 0xb0;
                                        }
                                    }
                                    else
                                    {
                                        if (Main.tile[i, j].frameNumber == 0)
                                        {
                                            Main.tile[i, j].frameX = 0x58;
                                            Main.tile[i, j].frameY = 0x84;
                                        }
                                        if (Main.tile[i, j].frameNumber == 1)
                                        {
                                            Main.tile[i, j].frameX = 0x58;
                                            Main.tile[i, j].frameY = 0x9a;
                                        }
                                        if (Main.tile[i, j].frameNumber == 2)
                                        {
                                            Main.tile[i, j].frameX = 0x58;
                                            Main.tile[i, j].frameY = 0xb0;
                                        }
                                    }
                                }
                                if ((((Main.tile[i, j].frameX == 0x42) && (((Main.tile[i, j].frameY == 0) || (Main.tile[i, j].frameY == 0x16)) || (Main.tile[i, j].frameY == 0x2c))) || ((Main.tile[i, j].frameX == 0x58) && (((Main.tile[i, j].frameY == 0x42) || (Main.tile[i, j].frameY == 0x58)) || (Main.tile[i, j].frameY == 110)))) || (((Main.tile[i, j].frameX == 0x2c) && (((Main.tile[i, j].frameY == 0xc6) || (Main.tile[i, j].frameY == 220)) || (Main.tile[i, j].frameY == 0xf2))) || ((Main.tile[i, j].frameX == 0x42) && (((Main.tile[i, j].frameY == 0xc6) || (Main.tile[i, j].frameY == 220)) || (Main.tile[i, j].frameY == 0xf2)))))
                                {
                                    if ((num4 != type) && (num5 != type))
                                    {
                                        KillTile(i, j, false, false, false);
                                    }
                                }
                                else if ((num7 == -1) || (num7 == 0x17))
                                {
                                    KillTile(i, j, false, false, false);
                                }
                                else if (((num2 != type) && (Main.tile[i, j].frameY < 0xc6)) && (((Main.tile[i, j].frameX != 0x16) && (Main.tile[i, j].frameX != 0x2c)) || (Main.tile[i, j].frameY < 0x84)))
                                {
                                    if ((num4 == type) || (num5 == type))
                                    {
                                        if (num7 == type)
                                        {
                                            if ((num4 == type) && (num5 == type))
                                            {
                                                if (Main.tile[i, j].frameNumber == 0)
                                                {
                                                    Main.tile[i, j].frameX = 0x84;
                                                    Main.tile[i, j].frameY = 0x84;
                                                }
                                                if (Main.tile[i, j].frameNumber == 1)
                                                {
                                                    Main.tile[i, j].frameX = 0x84;
                                                    Main.tile[i, j].frameY = 0x9a;
                                                }
                                                if (Main.tile[i, j].frameNumber == 2)
                                                {
                                                    Main.tile[i, j].frameX = 0x84;
                                                    Main.tile[i, j].frameY = 0xb0;
                                                }
                                            }
                                            else if (num4 == type)
                                            {
                                                if (Main.tile[i, j].frameNumber == 0)
                                                {
                                                    Main.tile[i, j].frameX = 0x84;
                                                    Main.tile[i, j].frameY = 0;
                                                }
                                                if (Main.tile[i, j].frameNumber == 1)
                                                {
                                                    Main.tile[i, j].frameX = 0x84;
                                                    Main.tile[i, j].frameY = 0x16;
                                                }
                                                if (Main.tile[i, j].frameNumber == 2)
                                                {
                                                    Main.tile[i, j].frameX = 0x84;
                                                    Main.tile[i, j].frameY = 0x2c;
                                                }
                                            }
                                            else if (num5 == type)
                                            {
                                                if (Main.tile[i, j].frameNumber == 0)
                                                {
                                                    Main.tile[i, j].frameX = 0x84;
                                                    Main.tile[i, j].frameY = 0x42;
                                                }
                                                if (Main.tile[i, j].frameNumber == 1)
                                                {
                                                    Main.tile[i, j].frameX = 0x84;
                                                    Main.tile[i, j].frameY = 0x58;
                                                }
                                                if (Main.tile[i, j].frameNumber == 2)
                                                {
                                                    Main.tile[i, j].frameX = 0x84;
                                                    Main.tile[i, j].frameY = 110;
                                                }
                                            }
                                        }
                                        else if ((num4 == type) && (num5 == type))
                                        {
                                            if (Main.tile[i, j].frameNumber == 0)
                                            {
                                                Main.tile[i, j].frameX = 0x9a;
                                                Main.tile[i, j].frameY = 0x84;
                                            }
                                            if (Main.tile[i, j].frameNumber == 1)
                                            {
                                                Main.tile[i, j].frameX = 0x9a;
                                                Main.tile[i, j].frameY = 0x9a;
                                            }
                                            if (Main.tile[i, j].frameNumber == 2)
                                            {
                                                Main.tile[i, j].frameX = 0x9a;
                                                Main.tile[i, j].frameY = 0xb0;
                                            }
                                        }
                                        else if (num4 == type)
                                        {
                                            if (Main.tile[i, j].frameNumber == 0)
                                            {
                                                Main.tile[i, j].frameX = 0x9a;
                                                Main.tile[i, j].frameY = 0;
                                            }
                                            if (Main.tile[i, j].frameNumber == 1)
                                            {
                                                Main.tile[i, j].frameX = 0x9a;
                                                Main.tile[i, j].frameY = 0x16;
                                            }
                                            if (Main.tile[i, j].frameNumber == 2)
                                            {
                                                Main.tile[i, j].frameX = 0x9a;
                                                Main.tile[i, j].frameY = 0x2c;
                                            }
                                        }
                                        else if (num5 == type)
                                        {
                                            if (Main.tile[i, j].frameNumber == 0)
                                            {
                                                Main.tile[i, j].frameX = 0x9a;
                                                Main.tile[i, j].frameY = 0x42;
                                            }
                                            if (Main.tile[i, j].frameNumber == 1)
                                            {
                                                Main.tile[i, j].frameX = 0x9a;
                                                Main.tile[i, j].frameY = 0x58;
                                            }
                                            if (Main.tile[i, j].frameNumber == 2)
                                            {
                                                Main.tile[i, j].frameX = 0x9a;
                                                Main.tile[i, j].frameY = 110;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Main.tile[i, j].frameNumber == 0)
                                        {
                                            Main.tile[i, j].frameX = 110;
                                            Main.tile[i, j].frameY = 0;
                                        }
                                        if (Main.tile[i, j].frameNumber == 1)
                                        {
                                            Main.tile[i, j].frameX = 110;
                                            Main.tile[i, j].frameY = 0x16;
                                        }
                                        if (Main.tile[i, j].frameNumber == 2)
                                        {
                                            Main.tile[i, j].frameX = 110;
                                            Main.tile[i, j].frameY = 0x2c;
                                        }
                                    }
                                }
                                rectangle.X = Main.tile[i, j].frameX;
                                rectangle.Y = Main.tile[i, j].frameY;
                            }
                            if (!Main.tileFrameImportant[Main.tile[i, j].type])
                            {
                                int frameNumber = 0;
                                if (resetFrame)
                                {
                                    frameNumber = genRand.Next(0, 3);
                                    Main.tile[i, j].frameNumber = (byte) frameNumber;
                                }
                                else
                                {
                                    frameNumber = Main.tile[i, j].frameNumber;
                                }
                                if (type == 0)
                                {
                                    for (int num33 = 0; num33 < 80; num33++)
                                    {
                                        switch (num33)
                                        {
                                            case 1:
                                            case 6:
                                            case 7:
                                            case 8:
                                            case 9:
                                            case 0x16:
                                            case 0x19:
                                            case 0x25:
                                            case 40:
                                            case 0x35:
                                            case 0x38:
                                                if (num2 == num33)
                                                {
                                                    TileFrame(i, j - 1, false, false);
                                                    if (WorldGen.mergeDown)
                                                    {
                                                        num2 = type;
                                                    }
                                                }
                                                if (num7 == num33)
                                                {
                                                    TileFrame(i, j + 1, false, false);
                                                    if (WorldGen.mergeUp)
                                                    {
                                                        num7 = type;
                                                    }
                                                }
                                                if (num4 == num33)
                                                {
                                                    TileFrame(i - 1, j, false, false);
                                                    if (WorldGen.mergeRight)
                                                    {
                                                        num4 = type;
                                                    }
                                                }
                                                if (num5 == num33)
                                                {
                                                    TileFrame(i + 1, j, false, false);
                                                    if (WorldGen.mergeLeft)
                                                    {
                                                        num5 = type;
                                                    }
                                                }
                                                if (index == num33)
                                                {
                                                    index = type;
                                                }
                                                if (num3 == num33)
                                                {
                                                    num3 = type;
                                                }
                                                if (num6 == num33)
                                                {
                                                    num6 = type;
                                                }
                                                if (num8 == num33)
                                                {
                                                    num8 = type;
                                                }
                                                break;
                                        }
                                    }
                                    if (num2 == 2)
                                    {
                                        num2 = type;
                                    }
                                    if (num7 == 2)
                                    {
                                        num7 = type;
                                    }
                                    if (num4 == 2)
                                    {
                                        num4 = type;
                                    }
                                    if (num5 == 2)
                                    {
                                        num5 = type;
                                    }
                                    if (index == 2)
                                    {
                                        index = type;
                                    }
                                    if (num3 == 2)
                                    {
                                        num3 = type;
                                    }
                                    if (num6 == 2)
                                    {
                                        num6 = type;
                                    }
                                    if (num8 == 2)
                                    {
                                        num8 = type;
                                    }
                                    if (num2 == 0x17)
                                    {
                                        num2 = type;
                                    }
                                    if (num7 == 0x17)
                                    {
                                        num7 = type;
                                    }
                                    if (num4 == 0x17)
                                    {
                                        num4 = type;
                                    }
                                    if (num5 == 0x17)
                                    {
                                        num5 = type;
                                    }
                                    if (index == 0x17)
                                    {
                                        index = type;
                                    }
                                    if (num3 == 0x17)
                                    {
                                        num3 = type;
                                    }
                                    if (num6 == 0x17)
                                    {
                                        num6 = type;
                                    }
                                    if (num8 == 0x17)
                                    {
                                        num8 = type;
                                    }
                                }
                                else if (type == 0x39)
                                {
                                    if (num2 == 0x3a)
                                    {
                                        TileFrame(i, j - 1, false, false);
                                        if (WorldGen.mergeDown)
                                        {
                                            num2 = type;
                                        }
                                    }
                                    if (num7 == 0x3a)
                                    {
                                        TileFrame(i, j + 1, false, false);
                                        if (WorldGen.mergeUp)
                                        {
                                            num7 = type;
                                        }
                                    }
                                    if (num4 == 0x3a)
                                    {
                                        TileFrame(i - 1, j, false, false);
                                        if (WorldGen.mergeRight)
                                        {
                                            num4 = type;
                                        }
                                    }
                                    if (num5 == 0x3a)
                                    {
                                        TileFrame(i + 1, j, false, false);
                                        if (WorldGen.mergeLeft)
                                        {
                                            num5 = type;
                                        }
                                    }
                                    if (index == 0x3a)
                                    {
                                        index = type;
                                    }
                                    if (num3 == 0x3a)
                                    {
                                        num3 = type;
                                    }
                                    if (num6 == 0x3a)
                                    {
                                        num6 = type;
                                    }
                                    if (num8 == 0x3a)
                                    {
                                        num8 = type;
                                    }
                                }
                                else if (type == 0x3b)
                                {
                                    if (num2 == 60)
                                    {
                                        num2 = type;
                                    }
                                    if (num7 == 60)
                                    {
                                        num7 = type;
                                    }
                                    if (num4 == 60)
                                    {
                                        num4 = type;
                                    }
                                    if (num5 == 60)
                                    {
                                        num5 = type;
                                    }
                                    if (index == 60)
                                    {
                                        index = type;
                                    }
                                    if (num3 == 60)
                                    {
                                        num3 = type;
                                    }
                                    if (num6 == 60)
                                    {
                                        num6 = type;
                                    }
                                    if (num8 == 60)
                                    {
                                        num8 = type;
                                    }
                                    if (num2 == 70)
                                    {
                                        num2 = type;
                                    }
                                    if (num7 == 70)
                                    {
                                        num7 = type;
                                    }
                                    if (num4 == 70)
                                    {
                                        num4 = type;
                                    }
                                    if (num5 == 70)
                                    {
                                        num5 = type;
                                    }
                                    if (index == 70)
                                    {
                                        index = type;
                                    }
                                    if (num3 == 70)
                                    {
                                        num3 = type;
                                    }
                                    if (num6 == 70)
                                    {
                                        num6 = type;
                                    }
                                    if (num8 == 70)
                                    {
                                        num8 = type;
                                    }
                                }
                                else if (type == 1)
                                {
                                    if (num2 == 0x3b)
                                    {
                                        TileFrame(i, j - 1, false, false);
                                        if (WorldGen.mergeDown)
                                        {
                                            num2 = type;
                                        }
                                    }
                                    if (num7 == 0x3b)
                                    {
                                        TileFrame(i, j + 1, false, false);
                                        if (WorldGen.mergeUp)
                                        {
                                            num7 = type;
                                        }
                                    }
                                    if (num4 == 0x3b)
                                    {
                                        TileFrame(i - 1, j, false, false);
                                        if (WorldGen.mergeRight)
                                        {
                                            num4 = type;
                                        }
                                    }
                                    if (num5 == 0x3b)
                                    {
                                        TileFrame(i + 1, j, false, false);
                                        if (WorldGen.mergeLeft)
                                        {
                                            num5 = type;
                                        }
                                    }
                                    if (index == 0x3b)
                                    {
                                        index = type;
                                    }
                                    if (num3 == 0x3b)
                                    {
                                        num3 = type;
                                    }
                                    if (num6 == 0x3b)
                                    {
                                        num6 = type;
                                    }
                                    if (num8 == 0x3b)
                                    {
                                        num8 = type;
                                    }
                                }
                                if ((((type == 1) || (type == 6)) || ((type == 7) || (type == 8))) || ((((type == 9) || (type == 0x16)) || ((type == 0x19) || (type == 0x25))) || (((type == 40) || (type == 0x35)) || (type == 0x38))))
                                {
                                    for (int num34 = 0; num34 < 80; num34++)
                                    {
                                        switch (num34)
                                        {
                                            case 1:
                                            case 6:
                                            case 7:
                                            case 8:
                                            case 9:
                                            case 0x16:
                                            case 0x19:
                                            case 0x25:
                                            case 40:
                                            case 0x35:
                                            case 0x38:
                                                if (num2 == 0)
                                                {
                                                    num2 = -2;
                                                }
                                                if (num7 == 0)
                                                {
                                                    num7 = -2;
                                                }
                                                if (num4 == 0)
                                                {
                                                    num4 = -2;
                                                }
                                                if (num5 == 0)
                                                {
                                                    num5 = -2;
                                                }
                                                if (index == 0)
                                                {
                                                    index = -2;
                                                }
                                                if (num3 == 0)
                                                {
                                                    num3 = -2;
                                                }
                                                if (num6 == 0)
                                                {
                                                    num6 = -2;
                                                }
                                                if (num8 == 0)
                                                {
                                                    num8 = -2;
                                                }
                                                break;
                                        }
                                    }
                                }
                                else if (type == 0x3a)
                                {
                                    if (num2 == 0x39)
                                    {
                                        num2 = -2;
                                    }
                                    if (num7 == 0x39)
                                    {
                                        num7 = -2;
                                    }
                                    if (num4 == 0x39)
                                    {
                                        num4 = -2;
                                    }
                                    if (num5 == 0x39)
                                    {
                                        num5 = -2;
                                    }
                                    if (index == 0x39)
                                    {
                                        index = -2;
                                    }
                                    if (num3 == 0x39)
                                    {
                                        num3 = -2;
                                    }
                                    if (num6 == 0x39)
                                    {
                                        num6 = -2;
                                    }
                                    if (num8 == 0x39)
                                    {
                                        num8 = -2;
                                    }
                                }
                                else if (type == 0x3b)
                                {
                                    if (num2 == 1)
                                    {
                                        num2 = -2;
                                    }
                                    if (num7 == 1)
                                    {
                                        num7 = -2;
                                    }
                                    if (num4 == 1)
                                    {
                                        num4 = -2;
                                    }
                                    if (num5 == 1)
                                    {
                                        num5 = -2;
                                    }
                                    if (index == 1)
                                    {
                                        index = -2;
                                    }
                                    if (num3 == 1)
                                    {
                                        num3 = -2;
                                    }
                                    if (num6 == 1)
                                    {
                                        num6 = -2;
                                    }
                                    if (num8 == 1)
                                    {
                                        num8 = -2;
                                    }
                                }
                                if ((type == 0x20) && (num7 == 0x17))
                                {
                                    num7 = type;
                                }
                                if ((type == 0x45) && (num7 == 60))
                                {
                                    num7 = type;
                                }
                                if (type == 0x33)
                                {
                                    if ((num2 > -1) && !Main.tileNoAttach[num2])
                                    {
                                        num2 = type;
                                    }
                                    if ((num7 > -1) && !Main.tileNoAttach[num7])
                                    {
                                        num7 = type;
                                    }
                                    if ((num4 > -1) && !Main.tileNoAttach[num4])
                                    {
                                        num4 = type;
                                    }
                                    if ((num5 > -1) && !Main.tileNoAttach[num5])
                                    {
                                        num5 = type;
                                    }
                                    if ((index > -1) && !Main.tileNoAttach[index])
                                    {
                                        index = type;
                                    }
                                    if ((num3 > -1) && !Main.tileNoAttach[num3])
                                    {
                                        num3 = type;
                                    }
                                    if ((num6 > -1) && !Main.tileNoAttach[num6])
                                    {
                                        num6 = type;
                                    }
                                    if ((num8 > -1) && !Main.tileNoAttach[num8])
                                    {
                                        num8 = type;
                                    }
                                }
                                if (((num2 > -1) && !Main.tileSolid[num2]) && (num2 != type))
                                {
                                    num2 = -1;
                                }
                                if (((num7 > -1) && !Main.tileSolid[num7]) && (num7 != type))
                                {
                                    num7 = -1;
                                }
                                if (((num4 > -1) && !Main.tileSolid[num4]) && (num4 != type))
                                {
                                    num4 = -1;
                                }
                                if (((num5 > -1) && !Main.tileSolid[num5]) && (num5 != type))
                                {
                                    num5 = -1;
                                }
                                if (((index > -1) && !Main.tileSolid[index]) && (index != type))
                                {
                                    index = -1;
                                }
                                if (((num3 > -1) && !Main.tileSolid[num3]) && (num3 != type))
                                {
                                    num3 = -1;
                                }
                                if (((num6 > -1) && !Main.tileSolid[num6]) && (num6 != type))
                                {
                                    num6 = -1;
                                }
                                if (((num8 > -1) && !Main.tileSolid[num8]) && (num8 != type))
                                {
                                    num8 = -1;
                                }
                                if (((type == 2) || (type == 0x17)) || ((type == 60) || (type == 70)))
                                {
                                    int num35 = 0;
                                    if ((type == 60) || (type == 70))
                                    {
                                        num35 = 0x3b;
                                    }
                                    else if (type == 2)
                                    {
                                        if (num2 == 0x17)
                                        {
                                            num2 = num35;
                                        }
                                        if (num7 == 0x17)
                                        {
                                            num7 = num35;
                                        }
                                        if (num4 == 0x17)
                                        {
                                            num4 = num35;
                                        }
                                        if (num5 == 0x17)
                                        {
                                            num5 = num35;
                                        }
                                        if (index == 0x17)
                                        {
                                            index = num35;
                                        }
                                        if (num3 == 0x17)
                                        {
                                            num3 = num35;
                                        }
                                        if (num6 == 0x17)
                                        {
                                            num6 = num35;
                                        }
                                        if (num8 == 0x17)
                                        {
                                            num8 = num35;
                                        }
                                    }
                                    else if (type == 0x17)
                                    {
                                        if (num2 == 2)
                                        {
                                            num2 = num35;
                                        }
                                        if (num7 == 2)
                                        {
                                            num7 = num35;
                                        }
                                        if (num4 == 2)
                                        {
                                            num4 = num35;
                                        }
                                        if (num5 == 2)
                                        {
                                            num5 = num35;
                                        }
                                        if (index == 2)
                                        {
                                            index = num35;
                                        }
                                        if (num3 == 2)
                                        {
                                            num3 = num35;
                                        }
                                        if (num6 == 2)
                                        {
                                            num6 = num35;
                                        }
                                        if (num8 == 2)
                                        {
                                            num8 = num35;
                                        }
                                    }
                                    if (((num2 != type) && (num2 != num35)) && ((num7 == type) || (num7 == num35)))
                                    {
                                        if ((num4 == num35) && (num5 == type))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0;
                                                    rectangle.Y = 0xc6;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x12;
                                                    rectangle.Y = 0xc6;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0xc6;
                                                    break;
                                            }
                                        }
                                        else if ((num4 == type) && (num5 == num35))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0xc6;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x48;
                                                    rectangle.Y = 0xc6;
                                                    break;

                                                case 2:
                                                    rectangle.X = 90;
                                                    rectangle.Y = 0xc6;
                                                    break;
                                            }
                                        }
                                    }
                                    else if (((num7 != type) && (num7 != num35)) && ((num2 == type) || (num2 == num35)))
                                    {
                                        if ((num4 == num35) && (num5 == type))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0;
                                                    rectangle.Y = 0xd8;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x12;
                                                    rectangle.Y = 0xd8;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0xd8;
                                                    break;
                                            }
                                        }
                                        else if ((num4 == type) && (num5 == num35))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0xd8;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x48;
                                                    rectangle.Y = 0xd8;
                                                    break;

                                                case 2:
                                                    rectangle.X = 90;
                                                    rectangle.Y = 0xd8;
                                                    break;
                                            }
                                        }
                                    }
                                    else if (((num4 != type) && (num4 != num35)) && ((num5 == type) || (num5 == num35)))
                                    {
                                        if ((num2 == num35) && (num7 == type))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x48;
                                                    rectangle.Y = 0x90;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x48;
                                                    rectangle.Y = 0xa2;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x48;
                                                    rectangle.Y = 180;
                                                    break;
                                            }
                                        }
                                        else if ((num7 == type) && (num5 == num2))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x48;
                                                    rectangle.Y = 90;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x48;
                                                    rectangle.Y = 0x6c;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x48;
                                                    rectangle.Y = 0x7e;
                                                    break;
                                            }
                                        }
                                    }
                                    else if (((num5 != type) && (num5 != num35)) && ((num4 == type) || (num4 == num35)))
                                    {
                                        if ((num2 == num35) && (num7 == type))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 90;
                                                    rectangle.Y = 0x90;
                                                    break;

                                                case 1:
                                                    rectangle.X = 90;
                                                    rectangle.Y = 0xa2;
                                                    break;

                                                case 2:
                                                    rectangle.X = 90;
                                                    rectangle.Y = 180;
                                                    break;
                                            }
                                        }
                                        else if ((num7 == type) && (num5 == num2))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 90;
                                                    rectangle.Y = 90;
                                                    break;

                                                case 1:
                                                    rectangle.X = 90;
                                                    rectangle.Y = 0x6c;
                                                    break;

                                                case 2:
                                                    rectangle.X = 90;
                                                    rectangle.Y = 0x7e;
                                                    break;
                                            }
                                        }
                                    }
                                    else if (((num2 == type) && (num7 == type)) && ((num4 == type) && (num5 == type)))
                                    {
                                        if (((index != type) && (num3 != type)) && ((num6 != type) && (num8 != type)))
                                        {
                                            if (num8 == num35)
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0x6c;
                                                        rectangle.Y = 0x144;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x7e;
                                                        rectangle.Y = 0x144;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0x90;
                                                        rectangle.Y = 0x144;
                                                        break;
                                                }
                                            }
                                            else if (num3 == num35)
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0x6c;
                                                        rectangle.Y = 0x156;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x7e;
                                                        rectangle.Y = 0x156;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0x90;
                                                        rectangle.Y = 0x156;
                                                        break;
                                                }
                                            }
                                            else if (num6 == num35)
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0x6c;
                                                        rectangle.Y = 360;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x7e;
                                                        rectangle.Y = 360;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0x90;
                                                        rectangle.Y = 360;
                                                        break;
                                                }
                                            }
                                            else if (index == num35)
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0x6c;
                                                        rectangle.Y = 0x17a;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x7e;
                                                        rectangle.Y = 0x17a;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0x90;
                                                        rectangle.Y = 0x17a;
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0x90;
                                                        rectangle.Y = 0xea;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0xc6;
                                                        rectangle.Y = 0xea;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0xfc;
                                                        rectangle.Y = 0xea;
                                                        break;
                                                }
                                            }
                                        }
                                        else if ((index != type) && (num8 != type))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0x132;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0x132;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x48;
                                                    rectangle.Y = 0x132;
                                                    break;
                                            }
                                        }
                                        else if ((num3 != type) && (num6 != type))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 90;
                                                    rectangle.Y = 0x132;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x6c;
                                                    rectangle.Y = 0x132;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x7e;
                                                    rectangle.Y = 0x132;
                                                    break;
                                            }
                                        }
                                        else if (((index != type) && (num3 == type)) && ((num6 == type) && (num8 == type)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0x6c;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0x90;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 180;
                                                    break;
                                            }
                                        }
                                        else if (((index == type) && (num3 != type)) && ((num6 == type) && (num8 == type)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0x6c;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0x90;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 180;
                                                    break;
                                            }
                                        }
                                        else if (((index == type) && (num3 == type)) && ((num6 != type) && (num8 == type)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 90;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0x7e;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0xa2;
                                                    break;
                                            }
                                        }
                                        else if (((index == type) && (num3 == type)) && ((num6 == type) && (num8 != type)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 90;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0x7e;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0xa2;
                                                    break;
                                            }
                                        }
                                    }
                                    else if ((((num2 == type) && (num7 == num35)) && ((num4 == type) && (num5 == type))) && ((index == -1) && (num3 == -1)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x6c;
                                                rectangle.Y = 0x12;
                                                break;

                                            case 1:
                                                rectangle.X = 0x7e;
                                                rectangle.Y = 0x12;
                                                break;

                                            case 2:
                                                rectangle.X = 0x90;
                                                rectangle.Y = 0x12;
                                                break;
                                        }
                                    }
                                    else if ((((num2 == num35) && (num7 == type)) && ((num4 == type) && (num5 == type))) && ((num6 == -1) && (num8 == -1)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x6c;
                                                rectangle.Y = 0x24;
                                                break;

                                            case 1:
                                                rectangle.X = 0x7e;
                                                rectangle.Y = 0x24;
                                                break;

                                            case 2:
                                                rectangle.X = 0x90;
                                                rectangle.Y = 0x24;
                                                break;
                                        }
                                    }
                                    else if ((((num2 == type) && (num7 == type)) && ((num4 == num35) && (num5 == type))) && ((num3 == -1) && (num8 == -1)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0xc6;
                                                rectangle.Y = 0;
                                                break;

                                            case 1:
                                                rectangle.X = 0xc6;
                                                rectangle.Y = 0x12;
                                                break;

                                            case 2:
                                                rectangle.X = 0xc6;
                                                rectangle.Y = 0x24;
                                                break;
                                        }
                                    }
                                    else if ((((num2 == type) && (num7 == type)) && ((num4 == type) && (num5 == num35))) && ((index == -1) && (num6 == -1)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 180;
                                                rectangle.Y = 0;
                                                break;

                                            case 1:
                                                rectangle.X = 180;
                                                rectangle.Y = 0x12;
                                                break;

                                            case 2:
                                                rectangle.X = 180;
                                                rectangle.Y = 0x24;
                                                break;
                                        }
                                    }
                                    else if (((num2 == type) && (num7 == num35)) && ((num4 == type) && (num5 == type)))
                                    {
                                        if (num3 == -1)
                                        {
                                            if (index != -1)
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0x24;
                                                        rectangle.Y = 0x6c;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x24;
                                                        rectangle.Y = 0x90;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0x24;
                                                        rectangle.Y = 180;
                                                        break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0x6c;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0x90;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 180;
                                                    break;
                                            }
                                        }
                                    }
                                    else if (((num2 == num35) && (num7 == type)) && ((num4 == type) && (num5 == type)))
                                    {
                                        if (num8 == -1)
                                        {
                                            if (num6 != -1)
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0x24;
                                                        rectangle.Y = 90;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x24;
                                                        rectangle.Y = 0x7e;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0x24;
                                                        rectangle.Y = 0xa2;
                                                        break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 90;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0x7e;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0xa2;
                                                    break;
                                            }
                                        }
                                    }
                                    else if (((num2 == type) && (num7 == type)) && ((num4 == type) && (num5 == num35)))
                                    {
                                        if (index == -1)
                                        {
                                            if (num6 != -1)
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0x36;
                                                        rectangle.Y = 0x6c;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x36;
                                                        rectangle.Y = 0x90;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0x36;
                                                        rectangle.Y = 180;
                                                        break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 90;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0x7e;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0xa2;
                                                    break;
                                            }
                                        }
                                    }
                                    else if (((num2 == type) && (num7 == type)) && ((num4 == num35) && (num5 == type)))
                                    {
                                        if (num3 == -1)
                                        {
                                            if (num8 != -1)
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0x24;
                                                        rectangle.Y = 0x6c;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x24;
                                                        rectangle.Y = 0x90;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0x24;
                                                        rectangle.Y = 180;
                                                        break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 90;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0x7e;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0xa2;
                                                    break;
                                            }
                                        }
                                    }
                                    else if (((((num2 == num35) && (num7 == type)) && ((num4 == type) && (num5 == type))) || (((num2 == type) && (num7 == num35)) && ((num4 == type) && (num5 == type)))) || ((((num2 == type) && (num7 == type)) && ((num4 == num35) && (num5 == type))) || (((num2 == type) && (num7 == type)) && ((num4 == type) && (num5 == num35)))))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x12;
                                                rectangle.Y = 0x12;
                                                break;

                                            case 1:
                                                rectangle.X = 0x24;
                                                rectangle.Y = 0x12;
                                                break;

                                            case 2:
                                                rectangle.X = 0x36;
                                                rectangle.Y = 0x12;
                                                break;
                                        }
                                    }
                                    if ((((num2 == type) || (num2 == num35)) && ((num7 == type) || (num7 == num35))) && (((num4 == type) || (num4 == num35)) && ((num5 == type) || (num5 == num35))))
                                    {
                                        if (((((index != type) && (index != num35)) && ((num3 == type) || (num3 == num35))) && ((num6 == type) || (num6 == num35))) && ((num8 == type) || (num8 == num35)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0x6c;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0x90;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 180;
                                                    break;
                                            }
                                        }
                                        else if (((((num3 != type) && (num3 != num35)) && ((index == type) || (index == num35))) && ((num6 == type) || (num6 == num35))) && ((num8 == type) || (num8 == num35)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0x6c;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0x90;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 180;
                                                    break;
                                            }
                                        }
                                        else if (((((num6 != type) && (num6 != num35)) && ((index == type) || (index == num35))) && ((num3 == type) || (num3 == num35))) && ((num8 == type) || (num8 == num35)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 90;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0x7e;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0xa2;
                                                    break;
                                            }
                                        }
                                        else if (((((num8 != type) && (num8 != num35)) && ((index == type) || (index == num35))) && ((num6 == type) || (num6 == num35))) && ((num3 == type) || (num3 == num35)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 90;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0x7e;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0xa2;
                                                    break;
                                            }
                                        }
                                    }
                                    if ((((num2 != num35) && (num2 != type)) && ((num7 == type) && (num4 != num35))) && (((num4 != type) && (num5 == type)) && ((num8 != num35) && (num8 != type))))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 90;
                                                rectangle.Y = 270;
                                                break;

                                            case 1:
                                                rectangle.X = 0x6c;
                                                rectangle.Y = 270;
                                                break;

                                            case 2:
                                                rectangle.X = 0x7e;
                                                rectangle.Y = 270;
                                                break;
                                        }
                                    }
                                    else if ((((num2 != num35) && (num2 != type)) && ((num7 == type) && (num4 == type))) && (((num5 != num35) && (num5 != type)) && ((num6 != num35) && (num6 != type))))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x90;
                                                rectangle.Y = 270;
                                                break;

                                            case 1:
                                                rectangle.X = 0xa2;
                                                rectangle.Y = 270;
                                                break;

                                            case 2:
                                                rectangle.X = 180;
                                                rectangle.Y = 270;
                                                break;
                                        }
                                    }
                                    else if ((((num7 != num35) && (num7 != type)) && ((num2 == type) && (num4 != num35))) && (((num4 != type) && (num5 == type)) && ((num3 != num35) && (num3 != type))))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 90;
                                                rectangle.Y = 0x120;
                                                break;

                                            case 1:
                                                rectangle.X = 0x6c;
                                                rectangle.Y = 0x120;
                                                break;

                                            case 2:
                                                rectangle.X = 0x7e;
                                                rectangle.Y = 0x120;
                                                break;
                                        }
                                    }
                                    else if ((((num7 != num35) && (num7 != type)) && ((num2 == type) && (num4 == type))) && (((num5 != num35) && (num5 != type)) && ((index != num35) && (index != type))))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x90;
                                                rectangle.Y = 0x120;
                                                break;

                                            case 1:
                                                rectangle.X = 0xa2;
                                                rectangle.Y = 0x120;
                                                break;

                                            case 2:
                                                rectangle.X = 180;
                                                rectangle.Y = 0x120;
                                                break;
                                        }
                                    }
                                    else if (((((num2 != type) && (num2 != num35)) && ((num7 == type) && (num4 == type))) && (((num5 == type) && (num6 != type)) && ((num6 != num35) && (num8 != type)))) && (num8 != num35))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x90;
                                                rectangle.Y = 0xd8;
                                                break;

                                            case 1:
                                                rectangle.X = 0xc6;
                                                rectangle.Y = 0xd8;
                                                break;

                                            case 2:
                                                rectangle.X = 0xfc;
                                                rectangle.Y = 0xd8;
                                                break;
                                        }
                                    }
                                    else if (((((num7 != type) && (num7 != num35)) && ((num2 == type) && (num4 == type))) && (((num5 == type) && (index != type)) && ((index != num35) && (num3 != type)))) && (num3 != num35))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x90;
                                                rectangle.Y = 0xfc;
                                                break;

                                            case 1:
                                                rectangle.X = 0xc6;
                                                rectangle.Y = 0xfc;
                                                break;

                                            case 2:
                                                rectangle.X = 0xfc;
                                                rectangle.Y = 0xfc;
                                                break;
                                        }
                                    }
                                    else if (((((num4 != type) && (num4 != num35)) && ((num7 == type) && (num2 == type))) && (((num5 == type) && (num3 != type)) && ((num3 != num35) && (num8 != type)))) && (num8 != num35))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x7e;
                                                rectangle.Y = 0xea;
                                                break;

                                            case 1:
                                                rectangle.X = 180;
                                                rectangle.Y = 0xea;
                                                break;

                                            case 2:
                                                rectangle.X = 0xea;
                                                rectangle.Y = 0xea;
                                                break;
                                        }
                                    }
                                    else if (((((num5 != type) && (num5 != num35)) && ((num7 == type) && (num2 == type))) && (((num4 == type) && (index != type)) && ((index != num35) && (num6 != type)))) && (num6 != num35))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0xa2;
                                                rectangle.Y = 0xea;
                                                break;

                                            case 1:
                                                rectangle.X = 0xd8;
                                                rectangle.Y = 0xea;
                                                break;

                                            case 2:
                                                rectangle.X = 270;
                                                rectangle.Y = 0xea;
                                                break;
                                        }
                                    }
                                    else if ((((num2 != num35) && (num2 != type)) && ((num7 == num35) || (num7 == type))) && ((num4 == num35) && (num5 == num35)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x24;
                                                rectangle.Y = 270;
                                                break;

                                            case 1:
                                                rectangle.X = 0x36;
                                                rectangle.Y = 270;
                                                break;

                                            case 2:
                                                rectangle.X = 0x48;
                                                rectangle.Y = 270;
                                                break;
                                        }
                                    }
                                    else if ((((num7 != num35) && (num7 != type)) && ((num2 == num35) || (num2 == type))) && ((num4 == num35) && (num5 == num35)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x24;
                                                rectangle.Y = 0x120;
                                                break;

                                            case 1:
                                                rectangle.X = 0x36;
                                                rectangle.Y = 0x120;
                                                break;

                                            case 2:
                                                rectangle.X = 0x48;
                                                rectangle.Y = 0x120;
                                                break;
                                        }
                                    }
                                    else if ((((num4 != num35) && (num4 != type)) && ((num5 == num35) || (num5 == type))) && ((num2 == num35) && (num7 == num35)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0;
                                                rectangle.Y = 270;
                                                break;

                                            case 1:
                                                rectangle.X = 0;
                                                rectangle.Y = 0x120;
                                                break;

                                            case 2:
                                                rectangle.X = 0;
                                                rectangle.Y = 0x132;
                                                break;
                                        }
                                    }
                                    else if ((((num5 != num35) && (num5 != type)) && ((num4 == num35) || (num4 == type))) && ((num2 == num35) && (num7 == num35)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x12;
                                                rectangle.Y = 270;
                                                break;

                                            case 1:
                                                rectangle.X = 0x12;
                                                rectangle.Y = 0x120;
                                                break;

                                            case 2:
                                                rectangle.X = 0x12;
                                                rectangle.Y = 0x132;
                                                break;
                                        }
                                    }
                                    else if (((num2 == type) && (num7 == num35)) && ((num4 == num35) && (num5 == num35)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0xc6;
                                                rectangle.Y = 0x120;
                                                break;

                                            case 1:
                                                rectangle.X = 0xd8;
                                                rectangle.Y = 0x120;
                                                break;

                                            case 2:
                                                rectangle.X = 0xea;
                                                rectangle.Y = 0x120;
                                                break;
                                        }
                                    }
                                    else if (((num2 == num35) && (num7 == type)) && ((num4 == num35) && (num5 == num35)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0xc6;
                                                rectangle.Y = 270;
                                                break;

                                            case 1:
                                                rectangle.X = 0xd8;
                                                rectangle.Y = 270;
                                                break;

                                            case 2:
                                                rectangle.X = 0xea;
                                                rectangle.Y = 270;
                                                break;
                                        }
                                    }
                                    else if (((num2 == num35) && (num7 == num35)) && ((num4 == type) && (num5 == num35)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0xc6;
                                                rectangle.Y = 0x132;
                                                break;

                                            case 1:
                                                rectangle.X = 0xd8;
                                                rectangle.Y = 0x132;
                                                break;

                                            case 2:
                                                rectangle.X = 0xea;
                                                rectangle.Y = 0x132;
                                                break;
                                        }
                                    }
                                    else if (((num2 == num35) && (num7 == num35)) && ((num4 == num35) && (num5 == type)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x90;
                                                rectangle.Y = 0x132;
                                                break;

                                            case 1:
                                                rectangle.X = 0xa2;
                                                rectangle.Y = 0x132;
                                                break;

                                            case 2:
                                                rectangle.X = 180;
                                                rectangle.Y = 0x132;
                                                break;
                                        }
                                    }
                                    if ((((num2 != type) && (num2 != num35)) && ((num7 == type) && (num4 == type))) && (num5 == type))
                                    {
                                        if (((num6 == num35) || (num6 == type)) && ((num8 != num35) && (num8 != type)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0;
                                                    rectangle.Y = 0x144;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x12;
                                                    rectangle.Y = 0x144;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0x144;
                                                    break;
                                            }
                                        }
                                        else if (((num8 == num35) || (num8 == type)) && ((num6 != num35) && (num6 != type)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0x144;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x48;
                                                    rectangle.Y = 0x144;
                                                    break;

                                                case 2:
                                                    rectangle.X = 90;
                                                    rectangle.Y = 0x144;
                                                    break;
                                            }
                                        }
                                    }
                                    else if ((((num7 != type) && (num7 != num35)) && ((num2 == type) && (num4 == type))) && (num5 == type))
                                    {
                                        if (((index == num35) || (index == type)) && ((num3 != num35) && (num3 != type)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0;
                                                    rectangle.Y = 0x156;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x12;
                                                    rectangle.Y = 0x156;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0x156;
                                                    break;
                                            }
                                        }
                                        else if (((num3 == num35) || (num3 == type)) && ((index != num35) && (index != type)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0x156;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x48;
                                                    rectangle.Y = 0x156;
                                                    break;

                                                case 2:
                                                    rectangle.X = 90;
                                                    rectangle.Y = 0x156;
                                                    break;
                                            }
                                        }
                                    }
                                    else if ((((num4 != type) && (num4 != num35)) && ((num2 == type) && (num7 == type))) && (num5 == type))
                                    {
                                        if (((num3 == num35) || (num3 == type)) && ((num8 != num35) && (num8 != type)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 360;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x48;
                                                    rectangle.Y = 360;
                                                    break;

                                                case 2:
                                                    rectangle.X = 90;
                                                    rectangle.Y = 360;
                                                    break;
                                            }
                                        }
                                        else if (((num8 == num35) || (num8 == type)) && ((num3 != num35) && (num3 != type)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0;
                                                    rectangle.Y = 360;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x12;
                                                    rectangle.Y = 360;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 360;
                                                    break;
                                            }
                                        }
                                    }
                                    else if ((((num5 != type) && (num5 != num35)) && ((num2 == type) && (num7 == type))) && (num4 == type))
                                    {
                                        if (((index == num35) || (index == type)) && ((num6 != num35) && (num6 != type)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0;
                                                    rectangle.Y = 0x17a;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x12;
                                                    rectangle.Y = 0x17a;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0x17a;
                                                    break;
                                            }
                                        }
                                        else if (((num6 == num35) || (num6 == type)) && ((index != num35) && (index != type)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0x17a;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x48;
                                                    rectangle.Y = 0x17a;
                                                    break;

                                                case 2:
                                                    rectangle.X = 90;
                                                    rectangle.Y = 0x17a;
                                                    break;
                                            }
                                        }
                                    }
                                    if (((((num2 == type) || (num2 == num35)) && ((num7 == type) || (num7 == num35))) && (((num4 == type) || (num4 == num35)) && ((num5 == type) || (num5 == num35)))) && (((index != -1) && (num3 != -1)) && ((num6 != -1) && (num8 != -1))))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x12;
                                                rectangle.Y = 0x12;
                                                break;

                                            case 1:
                                                rectangle.X = 0x24;
                                                rectangle.Y = 0x12;
                                                break;

                                            case 2:
                                                rectangle.X = 0x36;
                                                rectangle.Y = 0x12;
                                                break;
                                        }
                                    }
                                    if (num2 == num35)
                                    {
                                        num2 = -2;
                                    }
                                    if (num7 == num35)
                                    {
                                        num7 = -2;
                                    }
                                    if (num4 == num35)
                                    {
                                        num4 = -2;
                                    }
                                    if (num5 == num35)
                                    {
                                        num5 = -2;
                                    }
                                    if (index == num35)
                                    {
                                        index = -2;
                                    }
                                    if (num3 == num35)
                                    {
                                        num3 = -2;
                                    }
                                    if (num6 == num35)
                                    {
                                        num6 = -2;
                                    }
                                    if (num8 == num35)
                                    {
                                        num8 = -2;
                                    }
                                }
                                if ((((((type == 1) || (type == 2)) || ((type == 6) || (type == 7))) || (((type == 8) || (type == 9)) || ((type == 0x16) || (type == 0x17)))) || ((((type == 0x19) || (type == 0x25)) || ((type == 40) || (type == 0x35))) || (((type == 0x38) || (type == 0x3a)) || (((type == 0x3b) || (type == 60)) || (type == 70))))) && ((rectangle.X == -1) && (rectangle.Y == -1)))
                                {
                                    if ((num2 >= 0) && (num2 != type))
                                    {
                                        num2 = -1;
                                    }
                                    if ((num7 >= 0) && (num7 != type))
                                    {
                                        num7 = -1;
                                    }
                                    if ((num4 >= 0) && (num4 != type))
                                    {
                                        num4 = -1;
                                    }
                                    if ((num5 >= 0) && (num5 != type))
                                    {
                                        num5 = -1;
                                    }
                                    if (((num2 != -1) && (num7 != -1)) && ((num4 != -1) && (num5 != -1)))
                                    {
                                        if (((num2 == -2) && (num7 == type)) && ((num4 == type) && (num5 == type)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x90;
                                                    rectangle.Y = 0x6c;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0xa2;
                                                    rectangle.Y = 0x6c;
                                                    break;

                                                case 2:
                                                    rectangle.X = 180;
                                                    rectangle.Y = 0x6c;
                                                    break;
                                            }
                                            WorldGen.mergeUp = true;
                                        }
                                        else if (((num2 == type) && (num7 == -2)) && ((num4 == type) && (num5 == type)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x90;
                                                    rectangle.Y = 90;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0xa2;
                                                    rectangle.Y = 90;
                                                    break;

                                                case 2:
                                                    rectangle.X = 180;
                                                    rectangle.Y = 90;
                                                    break;
                                            }
                                            WorldGen.mergeDown = true;
                                        }
                                        else if (((num2 == type) && (num7 == type)) && ((num4 == -2) && (num5 == type)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0xa2;
                                                    rectangle.Y = 0x7e;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0xa2;
                                                    rectangle.Y = 0x90;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0xa2;
                                                    rectangle.Y = 0xa2;
                                                    break;
                                            }
                                            WorldGen.mergeLeft = true;
                                        }
                                        else if (((num2 == type) && (num7 == type)) && ((num4 == type) && (num5 == -2)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x90;
                                                    rectangle.Y = 0x7e;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x90;
                                                    rectangle.Y = 0x90;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x90;
                                                    rectangle.Y = 0xa2;
                                                    break;
                                            }
                                            WorldGen.mergeRight = true;
                                        }
                                        else if (((num2 == -2) && (num7 == type)) && ((num4 == -2) && (num5 == type)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 90;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0x7e;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0xa2;
                                                    break;
                                            }
                                            WorldGen.mergeUp = true;
                                            WorldGen.mergeLeft = true;
                                        }
                                        else if (((num2 == -2) && (num7 == type)) && ((num4 == type) && (num5 == -2)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 90;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0x7e;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0xa2;
                                                    break;
                                            }
                                            WorldGen.mergeUp = true;
                                            WorldGen.mergeRight = true;
                                        }
                                        else if (((num2 == type) && (num7 == -2)) && ((num4 == -2) && (num5 == type)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0x6c;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0x90;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 180;
                                                    break;
                                            }
                                            WorldGen.mergeDown = true;
                                            WorldGen.mergeLeft = true;
                                        }
                                        else if (((num2 == type) && (num7 == -2)) && ((num4 == type) && (num5 == -2)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0x6c;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0x90;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 180;
                                                    break;
                                            }
                                            WorldGen.mergeDown = true;
                                            WorldGen.mergeRight = true;
                                        }
                                        else if (((num2 == type) && (num7 == type)) && ((num4 == -2) && (num5 == -2)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 180;
                                                    rectangle.Y = 0x7e;
                                                    break;

                                                case 1:
                                                    rectangle.X = 180;
                                                    rectangle.Y = 0x90;
                                                    break;

                                                case 2:
                                                    rectangle.X = 180;
                                                    rectangle.Y = 0xa2;
                                                    break;
                                            }
                                            WorldGen.mergeLeft = true;
                                            WorldGen.mergeRight = true;
                                        }
                                        else if (((num2 == -2) && (num7 == -2)) && ((num4 == type) && (num5 == type)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x90;
                                                    rectangle.Y = 180;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0xa2;
                                                    rectangle.Y = 180;
                                                    break;

                                                case 2:
                                                    rectangle.X = 180;
                                                    rectangle.Y = 180;
                                                    break;
                                            }
                                            WorldGen.mergeUp = true;
                                            WorldGen.mergeDown = true;
                                        }
                                        else if (((num2 == -2) && (num7 == type)) && ((num4 == -2) && (num5 == -2)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0xc6;
                                                    rectangle.Y = 90;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0xc6;
                                                    rectangle.Y = 0x6c;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0xc6;
                                                    rectangle.Y = 0x7e;
                                                    break;
                                            }
                                            WorldGen.mergeUp = true;
                                            WorldGen.mergeLeft = true;
                                            WorldGen.mergeRight = true;
                                        }
                                        else if (((num2 == type) && (num7 == -2)) && ((num4 == -2) && (num5 == -2)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0xc6;
                                                    rectangle.Y = 0x90;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0xc6;
                                                    rectangle.Y = 0xa2;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0xc6;
                                                    rectangle.Y = 180;
                                                    break;
                                            }
                                            WorldGen.mergeDown = true;
                                            WorldGen.mergeLeft = true;
                                            WorldGen.mergeRight = true;
                                        }
                                        else if (((num2 == -2) && (num7 == -2)) && ((num4 == type) && (num5 == -2)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0xd8;
                                                    rectangle.Y = 0x90;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0xd8;
                                                    rectangle.Y = 0xa2;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0xd8;
                                                    rectangle.Y = 180;
                                                    break;
                                            }
                                            WorldGen.mergeUp = true;
                                            WorldGen.mergeDown = true;
                                            WorldGen.mergeRight = true;
                                        }
                                        else if (((num2 == -2) && (num7 == -2)) && ((num4 == -2) && (num5 == type)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0xd8;
                                                    rectangle.Y = 90;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0xd8;
                                                    rectangle.Y = 0x6c;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0xd8;
                                                    rectangle.Y = 0x7e;
                                                    break;
                                            }
                                            WorldGen.mergeUp = true;
                                            WorldGen.mergeDown = true;
                                            WorldGen.mergeLeft = true;
                                        }
                                        else if (((num2 == -2) && (num7 == -2)) && ((num4 == -2) && (num5 == -2)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x6c;
                                                    rectangle.Y = 0xc6;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x7e;
                                                    rectangle.Y = 0xc6;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x90;
                                                    rectangle.Y = 0xc6;
                                                    break;
                                            }
                                            WorldGen.mergeUp = true;
                                            WorldGen.mergeDown = true;
                                            WorldGen.mergeLeft = true;
                                            WorldGen.mergeRight = true;
                                        }
                                        else if (((num2 == type) && (num7 == type)) && ((num4 == type) && (num5 == type)))
                                        {
                                            if (index == -2)
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0x12;
                                                        rectangle.Y = 0x6c;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x12;
                                                        rectangle.Y = 0x90;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0x12;
                                                        rectangle.Y = 180;
                                                        break;
                                                }
                                            }
                                            if (num3 == -2)
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0;
                                                        rectangle.Y = 0x6c;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0;
                                                        rectangle.Y = 0x90;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0;
                                                        rectangle.Y = 180;
                                                        break;
                                                }
                                            }
                                            if (num6 == -2)
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0x12;
                                                        rectangle.Y = 90;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x12;
                                                        rectangle.Y = 0x7e;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0x12;
                                                        rectangle.Y = 0xa2;
                                                        break;
                                                }
                                            }
                                            if (num8 == -2)
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0;
                                                        rectangle.Y = 90;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0;
                                                        rectangle.Y = 0x7e;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0;
                                                        rectangle.Y = 0xa2;
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (((type != 2) && (type != 0x17)) && ((type != 60) && (type != 70)))
                                        {
                                            if (((num2 == -1) && (num7 == -2)) && ((num4 == type) && (num5 == type)))
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0xea;
                                                        rectangle.Y = 0;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0xfc;
                                                        rectangle.Y = 0;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 270;
                                                        rectangle.Y = 0;
                                                        break;
                                                }
                                                WorldGen.mergeDown = true;
                                            }
                                            else if (((num2 == -2) && (num7 == -1)) && ((num4 == type) && (num5 == type)))
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0xea;
                                                        rectangle.Y = 0x12;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0xfc;
                                                        rectangle.Y = 0x12;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 270;
                                                        rectangle.Y = 0x12;
                                                        break;
                                                }
                                                WorldGen.mergeUp = true;
                                            }
                                            else if (((num2 == type) && (num7 == type)) && ((num4 == -1) && (num5 == -2)))
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0xea;
                                                        rectangle.Y = 0x24;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0xfc;
                                                        rectangle.Y = 0x24;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 270;
                                                        rectangle.Y = 0x24;
                                                        break;
                                                }
                                                WorldGen.mergeRight = true;
                                            }
                                            else if (((num2 == type) && (num7 == type)) && ((num4 == -2) && (num5 == -1)))
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0xea;
                                                        rectangle.Y = 0x36;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0xfc;
                                                        rectangle.Y = 0x36;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 270;
                                                        rectangle.Y = 0x36;
                                                        break;
                                                }
                                                WorldGen.mergeLeft = true;
                                            }
                                        }
                                        if (((num2 != -1) && (num7 != -1)) && ((num4 == -1) && (num5 == type)))
                                        {
                                            if ((num2 == -2) && (num7 == type))
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0x48;
                                                        rectangle.Y = 0x90;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x48;
                                                        rectangle.Y = 0xa2;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0x48;
                                                        rectangle.Y = 180;
                                                        break;
                                                }
                                                WorldGen.mergeUp = true;
                                            }
                                            else if ((num7 == -2) && (num2 == type))
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0x48;
                                                        rectangle.Y = 90;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x48;
                                                        rectangle.Y = 0x6c;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0x48;
                                                        rectangle.Y = 0x7e;
                                                        break;
                                                }
                                                WorldGen.mergeDown = true;
                                            }
                                        }
                                        else if (((num2 != -1) && (num7 != -1)) && ((num4 == type) && (num5 == -1)))
                                        {
                                            if ((num2 == -2) && (num7 == type))
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 90;
                                                        rectangle.Y = 0x90;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 90;
                                                        rectangle.Y = 0xa2;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 90;
                                                        rectangle.Y = 180;
                                                        break;
                                                }
                                                WorldGen.mergeUp = true;
                                            }
                                            else if ((num7 == -2) && (num2 == type))
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 90;
                                                        rectangle.Y = 90;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 90;
                                                        rectangle.Y = 0x6c;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 90;
                                                        rectangle.Y = 0x7e;
                                                        break;
                                                }
                                                WorldGen.mergeDown = true;
                                            }
                                        }
                                        else if (((num2 == -1) && (num7 == type)) && ((num4 != -1) && (num5 != -1)))
                                        {
                                            if ((num4 == -2) && (num5 == type))
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0;
                                                        rectangle.Y = 0xc6;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x12;
                                                        rectangle.Y = 0xc6;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0x24;
                                                        rectangle.Y = 0xc6;
                                                        break;
                                                }
                                                WorldGen.mergeLeft = true;
                                            }
                                            else if ((num5 == -2) && (num4 == type))
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0x36;
                                                        rectangle.Y = 0xc6;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x48;
                                                        rectangle.Y = 0xc6;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 90;
                                                        rectangle.Y = 0xc6;
                                                        break;
                                                }
                                                WorldGen.mergeRight = true;
                                            }
                                        }
                                        else if (((num2 == type) && (num7 == -1)) && ((num4 != -1) && (num5 != -1)))
                                        {
                                            if ((num4 == -2) && (num5 == type))
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0;
                                                        rectangle.Y = 0xd8;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x12;
                                                        rectangle.Y = 0xd8;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0x24;
                                                        rectangle.Y = 0xd8;
                                                        break;
                                                }
                                                WorldGen.mergeLeft = true;
                                            }
                                            else if ((num5 == -2) && (num4 == type))
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0x36;
                                                        rectangle.Y = 0xd8;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x48;
                                                        rectangle.Y = 0xd8;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 90;
                                                        rectangle.Y = 0xd8;
                                                        break;
                                                }
                                                WorldGen.mergeRight = true;
                                            }
                                        }
                                        else if (((num2 != -1) && (num7 != -1)) && ((num4 == -1) && (num5 == -1)))
                                        {
                                            if ((num2 == -2) && (num7 == -2))
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0x6c;
                                                        rectangle.Y = 0xd8;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x6c;
                                                        rectangle.Y = 0xea;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0x6c;
                                                        rectangle.Y = 0xfc;
                                                        break;
                                                }
                                                WorldGen.mergeUp = true;
                                                WorldGen.mergeDown = true;
                                            }
                                            else if (num2 == -2)
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0x7e;
                                                        rectangle.Y = 0x90;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x7e;
                                                        rectangle.Y = 0xa2;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0x7e;
                                                        rectangle.Y = 180;
                                                        break;
                                                }
                                                WorldGen.mergeUp = true;
                                            }
                                            else if (num7 == -2)
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0x7e;
                                                        rectangle.Y = 90;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x7e;
                                                        rectangle.Y = 0x6c;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0x7e;
                                                        rectangle.Y = 0x7e;
                                                        break;
                                                }
                                                WorldGen.mergeDown = true;
                                            }
                                        }
                                        else if (((num2 == -1) && (num7 == -1)) && ((num4 != -1) && (num5 != -1)))
                                        {
                                            if ((num4 == -2) && (num5 == -2))
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0xa2;
                                                        rectangle.Y = 0xc6;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 180;
                                                        rectangle.Y = 0xc6;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0xc6;
                                                        rectangle.Y = 0xc6;
                                                        break;
                                                }
                                                WorldGen.mergeLeft = true;
                                                WorldGen.mergeRight = true;
                                            }
                                            else if (num4 == -2)
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0;
                                                        rectangle.Y = 0xfc;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x12;
                                                        rectangle.Y = 0xfc;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 0x24;
                                                        rectangle.Y = 0xfc;
                                                        break;
                                                }
                                                WorldGen.mergeLeft = true;
                                            }
                                            else if (num5 == -2)
                                            {
                                                switch (frameNumber)
                                                {
                                                    case 0:
                                                        rectangle.X = 0x36;
                                                        rectangle.Y = 0xfc;
                                                        break;

                                                    case 1:
                                                        rectangle.X = 0x48;
                                                        rectangle.Y = 0xfc;
                                                        break;

                                                    case 2:
                                                        rectangle.X = 90;
                                                        rectangle.Y = 0xfc;
                                                        break;
                                                }
                                                WorldGen.mergeRight = true;
                                            }
                                        }
                                        else if (((num2 == -2) && (num7 == -1)) && ((num4 == -1) && (num5 == -1)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x6c;
                                                    rectangle.Y = 0x90;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x6c;
                                                    rectangle.Y = 0xa2;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x6c;
                                                    rectangle.Y = 180;
                                                    break;
                                            }
                                            WorldGen.mergeUp = true;
                                        }
                                        else if (((num2 == -1) && (num7 == -2)) && ((num4 == -1) && (num5 == -1)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x6c;
                                                    rectangle.Y = 90;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x6c;
                                                    rectangle.Y = 0x6c;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x6c;
                                                    rectangle.Y = 0x7e;
                                                    break;
                                            }
                                            WorldGen.mergeDown = true;
                                        }
                                        else if (((num2 == -1) && (num7 == -1)) && ((num4 == -2) && (num5 == -1)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0;
                                                    rectangle.Y = 0xea;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x12;
                                                    rectangle.Y = 0xea;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0xea;
                                                    break;
                                            }
                                            WorldGen.mergeLeft = true;
                                        }
                                        else if (((num2 == -1) && (num7 == -1)) && ((num4 == -1) && (num5 == -2)))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0xea;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x48;
                                                    rectangle.Y = 0xea;
                                                    break;

                                                case 2:
                                                    rectangle.X = 90;
                                                    rectangle.Y = 0xea;
                                                    break;
                                            }
                                            WorldGen.mergeRight = true;
                                        }
                                    }
                                }
                                if ((rectangle.X < 0) || (rectangle.Y < 0))
                                {
                                    if (((type == 2) || (type == 0x17)) || ((type == 60) || (type == 70)))
                                    {
                                        if (num2 == -2)
                                        {
                                            num2 = type;
                                        }
                                        if (num7 == -2)
                                        {
                                            num7 = type;
                                        }
                                        if (num4 == -2)
                                        {
                                            num4 = type;
                                        }
                                        if (num5 == -2)
                                        {
                                            num5 = type;
                                        }
                                        if (index == -2)
                                        {
                                            index = type;
                                        }
                                        if (num3 == -2)
                                        {
                                            num3 = type;
                                        }
                                        if (num6 == -2)
                                        {
                                            num6 = type;
                                        }
                                        if (num8 == -2)
                                        {
                                            num8 = type;
                                        }
                                    }
                                    if (((num2 == type) && (num7 == type)) && ((num4 == type) & (num5 == type)))
                                    {
                                        if ((index != type) && (num3 != type))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x6c;
                                                    rectangle.Y = 0x12;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x7e;
                                                    rectangle.Y = 0x12;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x90;
                                                    rectangle.Y = 0x12;
                                                    break;
                                            }
                                        }
                                        else if ((num6 != type) && (num8 != type))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x6c;
                                                    rectangle.Y = 0x24;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x7e;
                                                    rectangle.Y = 0x24;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x90;
                                                    rectangle.Y = 0x24;
                                                    break;
                                            }
                                        }
                                        else if ((index != type) && (num6 != type))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 180;
                                                    rectangle.Y = 0;
                                                    break;

                                                case 1:
                                                    rectangle.X = 180;
                                                    rectangle.Y = 0x12;
                                                    break;

                                                case 2:
                                                    rectangle.X = 180;
                                                    rectangle.Y = 0x24;
                                                    break;
                                            }
                                        }
                                        else if ((num3 != type) && (num8 != type))
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0xc6;
                                                    rectangle.Y = 0;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0xc6;
                                                    rectangle.Y = 0x12;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0xc6;
                                                    rectangle.Y = 0x24;
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            switch (frameNumber)
                                            {
                                                case 0:
                                                    rectangle.X = 0x12;
                                                    rectangle.Y = 0x12;
                                                    break;

                                                case 1:
                                                    rectangle.X = 0x24;
                                                    rectangle.Y = 0x12;
                                                    break;

                                                case 2:
                                                    rectangle.X = 0x36;
                                                    rectangle.Y = 0x12;
                                                    break;
                                            }
                                        }
                                    }
                                    else if (((num2 != type) && (num7 == type)) && ((num4 == type) & (num5 == type)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x12;
                                                rectangle.Y = 0;
                                                break;

                                            case 1:
                                                rectangle.X = 0x24;
                                                rectangle.Y = 0;
                                                break;

                                            case 2:
                                                rectangle.X = 0x36;
                                                rectangle.Y = 0;
                                                break;
                                        }
                                    }
                                    else if (((num2 == type) && (num7 != type)) && ((num4 == type) & (num5 == type)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x12;
                                                rectangle.Y = 0x24;
                                                break;

                                            case 1:
                                                rectangle.X = 0x24;
                                                rectangle.Y = 0x24;
                                                break;

                                            case 2:
                                                rectangle.X = 0x36;
                                                rectangle.Y = 0x24;
                                                break;
                                        }
                                    }
                                    else if (((num2 == type) && (num7 == type)) && ((num4 != type) & (num5 == type)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0;
                                                rectangle.Y = 0;
                                                break;

                                            case 1:
                                                rectangle.X = 0;
                                                rectangle.Y = 0x12;
                                                break;

                                            case 2:
                                                rectangle.X = 0;
                                                rectangle.Y = 0x24;
                                                break;
                                        }
                                    }
                                    else if (((num2 == type) && (num7 == type)) && ((num4 == type) & (num5 != type)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x48;
                                                rectangle.Y = 0;
                                                break;

                                            case 1:
                                                rectangle.X = 0x48;
                                                rectangle.Y = 0x12;
                                                break;

                                            case 2:
                                                rectangle.X = 0x48;
                                                rectangle.Y = 0x24;
                                                break;
                                        }
                                    }
                                    else if (((num2 != type) && (num7 == type)) && ((num4 != type) & (num5 == type)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0;
                                                rectangle.Y = 0x36;
                                                break;

                                            case 1:
                                                rectangle.X = 0x24;
                                                rectangle.Y = 0x36;
                                                break;

                                            case 2:
                                                rectangle.X = 0x48;
                                                rectangle.Y = 0x36;
                                                break;
                                        }
                                    }
                                    else if (((num2 != type) && (num7 == type)) && ((num4 == type) & (num5 != type)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x12;
                                                rectangle.Y = 0x36;
                                                break;

                                            case 1:
                                                rectangle.X = 0x36;
                                                rectangle.Y = 0x36;
                                                break;

                                            case 2:
                                                rectangle.X = 90;
                                                rectangle.Y = 0x36;
                                                break;
                                        }
                                    }
                                    else if (((num2 == type) && (num7 != type)) && ((num4 != type) & (num5 == type)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0;
                                                rectangle.Y = 0x48;
                                                break;

                                            case 1:
                                                rectangle.X = 0x24;
                                                rectangle.Y = 0x48;
                                                break;

                                            case 2:
                                                rectangle.X = 0x48;
                                                rectangle.Y = 0x48;
                                                break;
                                        }
                                    }
                                    else if (((num2 == type) && (num7 != type)) && ((num4 == type) & (num5 != type)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x12;
                                                rectangle.Y = 0x48;
                                                break;

                                            case 1:
                                                rectangle.X = 0x36;
                                                rectangle.Y = 0x48;
                                                break;

                                            case 2:
                                                rectangle.X = 90;
                                                rectangle.Y = 0x48;
                                                break;
                                        }
                                    }
                                    else if (((num2 == type) && (num7 == type)) && ((num4 != type) & (num5 != type)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 90;
                                                rectangle.Y = 0;
                                                break;

                                            case 1:
                                                rectangle.X = 90;
                                                rectangle.Y = 0x12;
                                                break;

                                            case 2:
                                                rectangle.X = 90;
                                                rectangle.Y = 0x24;
                                                break;
                                        }
                                    }
                                    else if (((num2 != type) && (num7 != type)) && ((num4 == type) & (num5 == type)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x6c;
                                                rectangle.Y = 0x48;
                                                break;

                                            case 1:
                                                rectangle.X = 0x7e;
                                                rectangle.Y = 0x48;
                                                break;

                                            case 2:
                                                rectangle.X = 0x90;
                                                rectangle.Y = 0x48;
                                                break;
                                        }
                                    }
                                    else if (((num2 != type) && (num7 == type)) && ((num4 != type) & (num5 != type)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x6c;
                                                rectangle.Y = 0;
                                                break;

                                            case 1:
                                                rectangle.X = 0x7e;
                                                rectangle.Y = 0;
                                                break;

                                            case 2:
                                                rectangle.X = 0x90;
                                                rectangle.Y = 0;
                                                break;
                                        }
                                    }
                                    else if (((num2 == type) && (num7 != type)) && ((num4 != type) & (num5 != type)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0x6c;
                                                rectangle.Y = 0x36;
                                                break;

                                            case 1:
                                                rectangle.X = 0x7e;
                                                rectangle.Y = 0x36;
                                                break;

                                            case 2:
                                                rectangle.X = 0x90;
                                                rectangle.Y = 0x36;
                                                break;
                                        }
                                    }
                                    else if (((num2 != type) && (num7 != type)) && ((num4 != type) & (num5 == type)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0xa2;
                                                rectangle.Y = 0;
                                                break;

                                            case 1:
                                                rectangle.X = 0xa2;
                                                rectangle.Y = 0x12;
                                                break;

                                            case 2:
                                                rectangle.X = 0xa2;
                                                rectangle.Y = 0x24;
                                                break;
                                        }
                                    }
                                    else if (((num2 != type) && (num7 != type)) && ((num4 == type) & (num5 != type)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0xd8;
                                                rectangle.Y = 0;
                                                break;

                                            case 1:
                                                rectangle.X = 0xd8;
                                                rectangle.Y = 0x12;
                                                break;

                                            case 2:
                                                rectangle.X = 0xd8;
                                                rectangle.Y = 0x24;
                                                break;
                                        }
                                    }
                                    else if (((num2 != type) && (num7 != type)) && ((num4 != type) & (num5 != type)))
                                    {
                                        switch (frameNumber)
                                        {
                                            case 0:
                                                rectangle.X = 0xa2;
                                                rectangle.Y = 0x36;
                                                break;

                                            case 1:
                                                rectangle.X = 180;
                                                rectangle.Y = 0x36;
                                                break;

                                            case 2:
                                                rectangle.X = 0xc6;
                                                rectangle.Y = 0x36;
                                                break;
                                        }
                                    }
                                }
                                if ((rectangle.X <= -1) || (rectangle.Y <= -1))
                                {
                                    if (frameNumber <= 0)
                                    {
                                        rectangle.X = 0x12;
                                        rectangle.Y = 0x12;
                                    }
                                    if (frameNumber == 1)
                                    {
                                        rectangle.X = 0x24;
                                        rectangle.Y = 0x12;
                                    }
                                    if (frameNumber >= 2)
                                    {
                                        rectangle.X = 0x36;
                                        rectangle.Y = 0x12;
                                    }
                                }
                                Main.tile[i, j].frameX = (short) rectangle.X;
                                Main.tile[i, j].frameY = (short) rectangle.Y;
                                if ((type == 0x34) || (type == 0x3e))
                                {
                                    if (Main.tile[i, j - 1] != null)
                                    {
                                        if (!Main.tile[i, j - 1].active)
                                        {
                                            num2 = -1;
                                        }
                                        else
                                        {
                                            num2 = Main.tile[i, j - 1].type;
                                        }
                                    }
                                    else
                                    {
                                        num2 = type;
                                    }
                                    if (((num2 != type) && (num2 != 2)) && (num2 != 60))
                                    {
                                        KillTile(i, j, false, false, false);
                                    }
                                }
                                if (type == 0x35)
                                {
                                    if (Main.netMode == 0)
                                    {
                                        if ((Main.tile[i, j + 1] != null) && !Main.tile[i, j + 1].active)
                                        {
                                            bool flag3 = true;
                                            if (Main.tile[i, j - 1].active && (Main.tile[i, j - 1].type == 0x15))
                                            {
                                                flag3 = false;
                                            }
                                            if (flag3)
                                            {
                                                Main.tile[i, j].active = false;
                                                Projectile.NewProjectile((float) ((i * 0x10) + 8), (float) ((j * 0x10) + 8), 0f, 0.41f, 0x1f, 10, 0f, Main.myPlayer);
                                                SquareTileFrame(i, j, true);
                                            }
                                        }
                                    }
                                    else if (((Main.netMode == 2) && (Main.tile[i, j + 1] != null)) && !Main.tile[i, j + 1].active)
                                    {
                                        bool flag4 = true;
                                        if (Main.tile[i, j - 1].active && (Main.tile[i, j - 1].type == 0x15))
                                        {
                                            flag4 = false;
                                        }
                                        if (flag4)
                                        {
                                            Main.tile[i, j].active = false;
                                            int num36 = Projectile.NewProjectile((float) ((i * 0x10) + 8), (float) ((j * 0x10) + 8), 0f, 0.41f, 0x1f, 10, 0f, Main.myPlayer);
                                            Main.projectile[num36].velocity.Y = 0.5f;
                                            Main.projectile[num36].position.Y += 2f;
                                            NetMessage.SendTileSquare(-1, i, j, 1);
                                            SquareTileFrame(i, j, true);
                                        }
                                    }
                                }
                                if (((rectangle.X != frameX) && (rectangle.Y != frameY)) && ((frameX >= 0) && (frameY >= 0)))
                                {
                                    bool mergeUp = WorldGen.mergeUp;
                                    bool mergeDown = WorldGen.mergeDown;
                                    bool mergeLeft = WorldGen.mergeLeft;
                                    bool mergeRight = WorldGen.mergeRight;
                                    TileFrame(i - 1, j, false, false);
                                    TileFrame(i + 1, j, false, false);
                                    TileFrame(i, j - 1, false, false);
                                    TileFrame(i, j + 1, false, false);
                                    WorldGen.mergeUp = mergeUp;
                                    WorldGen.mergeDown = mergeDown;
                                    WorldGen.mergeLeft = mergeLeft;
                                    WorldGen.mergeRight = mergeRight;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void TileRunner(int i, int j, double strength, int steps, int type, bool addTile = false, float speedX = 0f, float speedY = 0f, bool noYChange = false, bool overRide = true)
        {
            Vector2 vector;
            Vector2 vector2;
            double num5 = strength;
            float num6 = steps;
            vector.X = i;
            vector.Y = j;
            vector2.X = genRand.Next(-10, 11) * 0.1f;
            vector2.Y = genRand.Next(-10, 11) * 0.1f;
            if ((speedX != 0f) || (speedY != 0f))
            {
                vector2.X = speedX;
                vector2.Y = speedY;
            }
            while ((num5 > 0.0) && (num6 > 0f))
            {
                num5 = strength * (num6 / ((float) steps));
                num6--;
                int num = (int) (vector.X - (num5 * 0.5));
                int maxTilesX = (int) (vector.X + (num5 * 0.5));
                int num2 = (int) (vector.Y - (num5 * 0.5));
                int maxTilesY = (int) (vector.Y + (num5 * 0.5));
                if (num < 0)
                {
                    num = 0;
                }
                if (maxTilesX > Main.maxTilesX)
                {
                    maxTilesX = Main.maxTilesX;
                }
                if (num2 < 0)
                {
                    num2 = 0;
                }
                if (maxTilesY > Main.maxTilesY)
                {
                    maxTilesY = Main.maxTilesY;
                }
                for (int k = num; k < maxTilesX; k++)
                {
                    for (int m = num2; m < maxTilesY; m++)
                    {
                        if ((Math.Abs((float) (k - vector.X)) + Math.Abs((float) (m - vector.Y))) < ((strength * 0.5) * (1.0 + (genRand.Next(-10, 11) * 0.015))))
                        {
                            if (type < 0)
                            {
                                if (((type == -2) && Main.tile[k, m].active) && ((m < waterLine) || (m > lavaLine)))
                                {
                                    Main.tile[k, m].liquid = 0xff;
                                    if (m > lavaLine)
                                    {
                                        Main.tile[k, m].lava = true;
                                    }
                                }
                                Main.tile[k, m].active = false;
                            }
                            else
                            {
                                if (((overRide || !Main.tile[k, m].active) && ((type != 40) || (Main.tile[k, m].type != 0x35))) && ((!Main.tileStone[type] || (Main.tile[k, m].type == 1)) && (Main.tile[k, m].type != 0x2d)))
                                {
                                    Main.tile[k, m].type = (byte) type;
                                }
                                if (addTile)
                                {
                                    Main.tile[k, m].active = true;
                                    Main.tile[k, m].liquid = 0;
                                    Main.tile[k, m].lava = false;
                                }
                                if (noYChange && (m < Main.worldSurface))
                                {
                                    Main.tile[k, m].wall = 2;
                                }
                                if (((type == 0x3b) && (m > waterLine)) && (Main.tile[k, m].liquid > 0))
                                {
                                    Main.tile[k, m].lava = false;
                                    Main.tile[k, m].liquid = 0;
                                }
                            }
                        }
                    }
                }
                vector += vector2;
                if (num5 > 50.0)
                {
                    vector += vector2;
                    num6--;
                    vector2.Y += genRand.Next(-10, 11) * 0.05f;
                    vector2.X += genRand.Next(-10, 11) * 0.05f;
                    if (num5 > 100.0)
                    {
                        vector += vector2;
                        num6--;
                        vector2.Y += genRand.Next(-10, 11) * 0.05f;
                        vector2.X += genRand.Next(-10, 11) * 0.05f;
                        if (num5 > 150.0)
                        {
                            vector += vector2;
                            num6--;
                            vector2.Y += genRand.Next(-10, 11) * 0.05f;
                            vector2.X += genRand.Next(-10, 11) * 0.05f;
                            if (num5 > 200.0)
                            {
                                vector += vector2;
                                num6--;
                                vector2.Y += genRand.Next(-10, 11) * 0.05f;
                                vector2.X += genRand.Next(-10, 11) * 0.05f;
                                if (num5 > 250.0)
                                {
                                    vector += vector2;
                                    num6--;
                                    vector2.Y += genRand.Next(-10, 11) * 0.05f;
                                    vector2.X += genRand.Next(-10, 11) * 0.05f;
                                    if (num5 > 300.0)
                                    {
                                        vector += vector2;
                                        num6--;
                                        vector2.Y += genRand.Next(-10, 11) * 0.05f;
                                        vector2.X += genRand.Next(-10, 11) * 0.05f;
                                        if (num5 > 400.0)
                                        {
                                            vector += vector2;
                                            num6--;
                                            vector2.Y += genRand.Next(-10, 11) * 0.05f;
                                            vector2.X += genRand.Next(-10, 11) * 0.05f;
                                            if (num5 > 500.0)
                                            {
                                                vector += vector2;
                                                num6--;
                                                vector2.Y += genRand.Next(-10, 11) * 0.05f;
                                                vector2.X += genRand.Next(-10, 11) * 0.05f;
                                                if (num5 > 600.0)
                                                {
                                                    vector += vector2;
                                                    num6--;
                                                    vector2.Y += genRand.Next(-10, 11) * 0.05f;
                                                    vector2.X += genRand.Next(-10, 11) * 0.05f;
                                                    if (num5 > 700.0)
                                                    {
                                                        vector += vector2;
                                                        num6--;
                                                        vector2.Y += genRand.Next(-10, 11) * 0.05f;
                                                        vector2.X += genRand.Next(-10, 11) * 0.05f;
                                                        if (num5 > 800.0)
                                                        {
                                                            vector += vector2;
                                                            num6--;
                                                            vector2.Y += genRand.Next(-10, 11) * 0.05f;
                                                            vector2.X += genRand.Next(-10, 11) * 0.05f;
                                                            if (num5 > 900.0)
                                                            {
                                                                vector += vector2;
                                                                num6--;
                                                                vector2.Y += genRand.Next(-10, 11) * 0.05f;
                                                                vector2.X += genRand.Next(-10, 11) * 0.05f;
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
                vector2.X += genRand.Next(-10, 11) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if (!noYChange)
                {
                    vector2.Y += genRand.Next(-10, 11) * 0.05f;
                    if (vector2.Y > 1f)
                    {
                        vector2.Y = 1f;
                    }
                    if (vector2.Y < -1f)
                    {
                        vector2.Y = -1f;
                    }
                }
                else if (num5 < 3.0)
                {
                    if (vector2.Y > 1f)
                    {
                        vector2.Y = 1f;
                    }
                    if (vector2.Y < -1f)
                    {
                        vector2.Y = -1f;
                    }
                }
                if (type == 0x3b)
                {
                    if (vector2.Y > 0.5)
                    {
                        vector2.Y = 0.5f;
                    }
                    if (vector2.Y < -0.5)
                    {
                        vector2.Y = -0.5f;
                    }
                    if (vector.Y < (Main.rockLayer + 100.0))
                    {
                        vector2.Y = 1f;
                    }
                    if (vector.Y > (Main.maxTilesY - 300))
                    {
                        vector2.Y = -1f;
                    }
                }
            }
        }

        public static void UpdateWorld()
        {
            Liquid.skipCount++;
            if (Liquid.skipCount > 1)
            {
                Liquid.UpdateLiquid();
                Liquid.skipCount = 0;
            }
            float num = 4E-05f;
            float num2 = 2E-05f;
            bool flag = false;
            spawnDelay++;
            if (Main.invasionType > 0)
            {
                spawnDelay = 0;
            }
            if (spawnDelay >= 20)
            {
                flag = true;
                spawnDelay = 0;
                for (int k = 0; k < 0x3e8; k++)
                {
                    if ((Main.npc[k].active && Main.npc[k].homeless) && Main.npc[k].townNPC)
                    {
                        spawnNPC = Main.npc[k].type;
                        break;
                    }
                }
            }
            for (int i = 0; i < ((Main.maxTilesX * Main.maxTilesY) * num); i++)
            {
                int num5 = genRand.Next(10, Main.maxTilesX - 10);
                int num6 = genRand.Next(10, ((int) Main.worldSurface) - 1);
                int num7 = num5 - 1;
                int num8 = num5 + 2;
                int num9 = num6 - 1;
                int num10 = num6 + 2;
                if (num7 < 10)
                {
                    num7 = 10;
                }
                if (num8 > (Main.maxTilesX - 10))
                {
                    num8 = Main.maxTilesX - 10;
                }
                if (num9 < 10)
                {
                    num9 = 10;
                }
                if (num10 > (Main.maxTilesY - 10))
                {
                    num10 = Main.maxTilesY - 10;
                }
                if (Main.tile[num5, num6] != null)
                {
                    if (Main.tile[num5, num6].liquid > 0x20)
                    {
                        if (Main.tile[num5, num6].active && (((Main.tile[num5, num6].type == 3) || (Main.tile[num5, num6].type == 20)) || (((Main.tile[num5, num6].type == 0x18) || (Main.tile[num5, num6].type == 0x1b)) || (Main.tile[num5, num6].type == 0x49))))
                        {
                            KillTile(num5, num6, false, false, false);
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendData(0x11, -1, -1, "", 0, (float) num5, (float) num6, 0f);
                            }
                        }
                    }
                    else if (Main.tile[num5, num6].active)
                    {
                        if (Main.tile[num5, num6].type == 0x4e)
                        {
                            if (!Main.tile[num5, num9].active)
                            {
                                PlaceTile(num5, num9, 3, true, false, -1);
                                if ((Main.netMode == 2) && Main.tile[num5, num9].active)
                                {
                                    NetMessage.SendTileSquare(-1, num5, num9, 1);
                                }
                            }
                        }
                        else if (((Main.tile[num5, num6].type == 2) || (Main.tile[num5, num6].type == 0x17)) || (Main.tile[num5, num6].type == 0x20))
                        {
                            int type = Main.tile[num5, num6].type;
                            if ((!Main.tile[num5, num9].active && (genRand.Next(10) == 0)) && (type == 2))
                            {
                                PlaceTile(num5, num9, 3, true, false, -1);
                                if ((Main.netMode == 2) && Main.tile[num5, num9].active)
                                {
                                    NetMessage.SendTileSquare(-1, num5, num9, 1);
                                }
                            }
                            if ((!Main.tile[num5, num9].active && (genRand.Next(10) == 0)) && (type == 0x17))
                            {
                                PlaceTile(num5, num9, 0x18, true, false, -1);
                                if ((Main.netMode == 2) && Main.tile[num5, num9].active)
                                {
                                    NetMessage.SendTileSquare(-1, num5, num9, 1);
                                }
                            }
                            bool flag2 = false;
                            for (int m = num7; m < num8; m++)
                            {
                                for (int n = num9; n < num10; n++)
                                {
                                    if (((num5 != m) || (num6 != n)) && Main.tile[m, n].active)
                                    {
                                        if (type == 0x20)
                                        {
                                            type = 0x17;
                                        }
                                        if ((Main.tile[m, n].type == 0) || ((type == 0x17) && (Main.tile[m, n].type == 2)))
                                        {
                                            SpreadGrass(m, n, 0, type, false);
                                            if (type == 0x17)
                                            {
                                                SpreadGrass(m, n, 2, type, false);
                                            }
                                            if (Main.tile[m, n].type == type)
                                            {
                                                SquareTileFrame(m, n, true);
                                                flag2 = true;
                                            }
                                        }
                                    }
                                }
                            }
                            if ((Main.netMode == 2) && flag2)
                            {
                                NetMessage.SendTileSquare(-1, num5, num6, 3);
                            }
                        }
                        else if (((Main.tile[num5, num6].type == 20) && !PlayerLOS(num5, num6)) && (genRand.Next(5) == 0))
                        {
                            GrowTree(num5, num6);
                        }
                        if (((Main.tile[num5, num6].type == 3) && (genRand.Next(10) == 0)) && (Main.tile[num5, num6].frameX < 0x90))
                        {
                            Main.tile[num5, num6].type = 0x49;
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendTileSquare(-1, num5, num6, 3);
                            }
                        }
                        if ((Main.tile[num5, num6].type == 0x20) && (genRand.Next(3) == 0))
                        {
                            int num14 = num5;
                            int num15 = num6;
                            int num16 = 0;
                            if (Main.tile[num14 + 1, num15].active && (Main.tile[num14 + 1, num15].type == 0x20))
                            {
                                num16++;
                            }
                            if (Main.tile[num14 - 1, num15].active && (Main.tile[num14 - 1, num15].type == 0x20))
                            {
                                num16++;
                            }
                            if (Main.tile[num14, num15 + 1].active && (Main.tile[num14, num15 + 1].type == 0x20))
                            {
                                num16++;
                            }
                            if (Main.tile[num14, num15 - 1].active && (Main.tile[num14, num15 - 1].type == 0x20))
                            {
                                num16++;
                            }
                            if ((num16 < 3) || (Main.tile[num5, num6].type == 0x17))
                            {
                                switch (genRand.Next(4))
                                {
                                    case 0:
                                        num15--;
                                        break;

                                    case 1:
                                        num15++;
                                        break;

                                    case 2:
                                        num14--;
                                        break;

                                    case 3:
                                        num14++;
                                        break;
                                }
                                if (!Main.tile[num14, num15].active)
                                {
                                    num16 = 0;
                                    if (Main.tile[num14 + 1, num15].active && (Main.tile[num14 + 1, num15].type == 0x20))
                                    {
                                        num16++;
                                    }
                                    if (Main.tile[num14 - 1, num15].active && (Main.tile[num14 - 1, num15].type == 0x20))
                                    {
                                        num16++;
                                    }
                                    if (Main.tile[num14, num15 + 1].active && (Main.tile[num14, num15 + 1].type == 0x20))
                                    {
                                        num16++;
                                    }
                                    if (Main.tile[num14, num15 - 1].active && (Main.tile[num14, num15 - 1].type == 0x20))
                                    {
                                        num16++;
                                    }
                                    if (num16 < 2)
                                    {
                                        int num18 = 7;
                                        int num19 = num14 - num18;
                                        int num20 = num14 + num18;
                                        int num21 = num15 - num18;
                                        int num22 = num15 + num18;
                                        bool flag3 = false;
                                        for (int num23 = num19; num23 < num20; num23++)
                                        {
                                            for (int num24 = num21; num24 < num22; num24++)
                                            {
                                                if ((((((Math.Abs((int) (num23 - num14)) * 2) + Math.Abs((int) (num24 - num15))) < 9) && Main.tile[num23, num24].active) && ((Main.tile[num23, num24].type == 0x17) && Main.tile[num23, num24 - 1].active)) && ((Main.tile[num23, num24 - 1].type == 0x20) && (Main.tile[num23, num24 - 1].liquid == 0)))
                                                {
                                                    flag3 = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (flag3)
                                        {
                                            Main.tile[num14, num15].type = 0x20;
                                            Main.tile[num14, num15].active = true;
                                            SquareTileFrame(num14, num15, true);
                                            if (Main.netMode == 2)
                                            {
                                                NetMessage.SendTileSquare(-1, num14, num15, 3);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (((Main.tile[num5, num6].type == 2) || (Main.tile[num5, num6].type == 0x34)) && (((genRand.Next(5) == 0) && !Main.tile[num5, num6 + 1].active) && !Main.tile[num5, num6 + 1].lava))
                        {
                            bool flag4 = false;
                            for (int num25 = num6; num25 > (num6 - 10); num25--)
                            {
                                if (Main.tile[num5, num25].active && (Main.tile[num5, num25].type == 2))
                                {
                                    flag4 = true;
                                    break;
                                }
                            }
                            if (flag4)
                            {
                                int num26 = num5;
                                int num27 = num6 + 1;
                                Main.tile[num26, num27].type = 0x34;
                                Main.tile[num26, num27].active = true;
                                SquareTileFrame(num26, num27, true);
                                if (Main.netMode == 2)
                                {
                                    NetMessage.SendTileSquare(-1, num26, num27, 3);
                                }
                            }
                        }
                    }
                    else if (flag && (spawnNPC > 0))
                    {
                        SpawnNPC(num5, num6);
                    }
                }
            }
            for (int j = 0; j < ((Main.maxTilesX * Main.maxTilesY) * num2); j++)
            {
                int num29 = genRand.Next(10, Main.maxTilesX - 10);
                int num30 = genRand.Next(((int) Main.worldSurface) + 2, Main.maxTilesY - 200);
                int num31 = num29 - 1;
                int num32 = num29 + 2;
                int num33 = num30 - 1;
                int num34 = num30 + 2;
                if (num31 < 10)
                {
                    num31 = 10;
                }
                if (num32 > (Main.maxTilesX - 10))
                {
                    num32 = Main.maxTilesX - 10;
                }
                if (num33 < 10)
                {
                    num33 = 10;
                }
                if (num34 > (Main.maxTilesY - 10))
                {
                    num34 = Main.maxTilesY - 10;
                }
                if (Main.tile[num29, num30] != null)
                {
                    if (Main.tile[num29, num30].liquid > 0x20)
                    {
                        if (Main.tile[num29, num30].active && ((Main.tile[num29, num30].type == 0x3d) || (Main.tile[num29, num30].type == 0x4a)))
                        {
                            KillTile(num29, num30, false, false, false);
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendData(0x11, -1, -1, "", 0, (float) num29, (float) num30, 0f);
                            }
                        }
                        continue;
                    }
                    if (Main.tile[num29, num30].active)
                    {
                        if (Main.tile[num29, num30].type == 60)
                        {
                            int grass = Main.tile[num29, num30].type;
                            if (!Main.tile[num29, num33].active && (genRand.Next(10) == 0))
                            {
                                PlaceTile(num29, num33, 0x3d, true, false, -1);
                                if ((Main.netMode == 2) && Main.tile[num29, num33].active)
                                {
                                    NetMessage.SendTileSquare(-1, num29, num33, 1);
                                }
                            }
                            bool flag5 = false;
                            for (int num36 = num31; num36 < num32; num36++)
                            {
                                for (int num37 = num33; num37 < num34; num37++)
                                {
                                    if (((num29 != num36) || (num30 != num37)) && (Main.tile[num36, num37].active && (Main.tile[num36, num37].type == 0x3b)))
                                    {
                                        SpreadGrass(num36, num37, 0x3b, grass, false);
                                        if (Main.tile[num36, num37].type == grass)
                                        {
                                            SquareTileFrame(num36, num37, true);
                                            flag5 = true;
                                        }
                                    }
                                }
                            }
                            if ((Main.netMode == 2) && flag5)
                            {
                                NetMessage.SendTileSquare(-1, num29, num30, 3);
                            }
                        }
                        if (((Main.tile[num29, num30].type == 0x3d) && (genRand.Next(3) == 0)) && (Main.tile[num29, num30].frameX < 0x90))
                        {
                            Main.tile[num29, num30].type = 0x4a;
                            if (Main.netMode == 2)
                            {
                                NetMessage.SendTileSquare(-1, num29, num30, 3);
                            }
                        }
                        if (((Main.tile[num29, num30].type == 60) || (Main.tile[num29, num30].type == 0x3e)) && (((genRand.Next(5) == 0) && !Main.tile[num29, num30 + 1].active) && !Main.tile[num29, num30 + 1].lava))
                        {
                            bool flag6 = false;
                            for (int num38 = num30; num38 > (num30 - 10); num38--)
                            {
                                if (Main.tile[num29, num38].active && (Main.tile[num29, num38].type == 60))
                                {
                                    flag6 = true;
                                    break;
                                }
                            }
                            if (flag6)
                            {
                                int num39 = num29;
                                int num40 = num30 + 1;
                                Main.tile[num39, num40].type = 0x3e;
                                Main.tile[num39, num40].active = true;
                                SquareTileFrame(num39, num40, true);
                                if (Main.netMode == 2)
                                {
                                    NetMessage.SendTileSquare(-1, num39, num40, 3);
                                }
                            }
                        }
                        if ((Main.tile[num29, num30].type == 0x45) && (genRand.Next(3) == 0))
                        {
                            int num41 = num29;
                            int num42 = num30;
                            int num43 = 0;
                            if (Main.tile[num41 + 1, num42].active && (Main.tile[num41 + 1, num42].type == 0x45))
                            {
                                num43++;
                            }
                            if (Main.tile[num41 - 1, num42].active && (Main.tile[num41 - 1, num42].type == 0x45))
                            {
                                num43++;
                            }
                            if (Main.tile[num41, num42 + 1].active && (Main.tile[num41, num42 + 1].type == 0x45))
                            {
                                num43++;
                            }
                            if (Main.tile[num41, num42 - 1].active && (Main.tile[num41, num42 - 1].type == 0x45))
                            {
                                num43++;
                            }
                            if ((num43 < 3) || (Main.tile[num29, num30].type == 60))
                            {
                                switch (genRand.Next(4))
                                {
                                    case 0:
                                        num42--;
                                        break;

                                    case 1:
                                        num42++;
                                        break;

                                    case 2:
                                        num41--;
                                        break;

                                    case 3:
                                        num41++;
                                        break;
                                }
                                if (!Main.tile[num41, num42].active)
                                {
                                    num43 = 0;
                                    if (Main.tile[num41 + 1, num42].active && (Main.tile[num41 + 1, num42].type == 0x45))
                                    {
                                        num43++;
                                    }
                                    if (Main.tile[num41 - 1, num42].active && (Main.tile[num41 - 1, num42].type == 0x45))
                                    {
                                        num43++;
                                    }
                                    if (Main.tile[num41, num42 + 1].active && (Main.tile[num41, num42 + 1].type == 0x45))
                                    {
                                        num43++;
                                    }
                                    if (Main.tile[num41, num42 - 1].active && (Main.tile[num41, num42 - 1].type == 0x45))
                                    {
                                        num43++;
                                    }
                                    if (num43 < 2)
                                    {
                                        int num45 = 7;
                                        int num46 = num41 - num45;
                                        int num47 = num41 + num45;
                                        int num48 = num42 - num45;
                                        int num49 = num42 + num45;
                                        bool flag7 = false;
                                        for (int num50 = num46; num50 < num47; num50++)
                                        {
                                            for (int num51 = num48; num51 < num49; num51++)
                                            {
                                                if ((((((Math.Abs((int) (num50 - num41)) * 2) + Math.Abs((int) (num51 - num42))) < 9) && Main.tile[num50, num51].active) && ((Main.tile[num50, num51].type == 60) && Main.tile[num50, num51 - 1].active)) && ((Main.tile[num50, num51 - 1].type == 0x45) && (Main.tile[num50, num51 - 1].liquid == 0)))
                                                {
                                                    flag7 = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (flag7)
                                        {
                                            Main.tile[num41, num42].type = 0x45;
                                            Main.tile[num41, num42].active = true;
                                            SquareTileFrame(num41, num42, true);
                                            if (Main.netMode == 2)
                                            {
                                                NetMessage.SendTileSquare(-1, num41, num42, 3);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (Main.tile[num29, num30].type == 70)
                        {
                            int num52 = Main.tile[num29, num30].type;
                            if (!Main.tile[num29, num33].active && (genRand.Next(10) == 0))
                            {
                                PlaceTile(num29, num33, 0x47, true, false, -1);
                                if ((Main.netMode == 2) && Main.tile[num29, num33].active)
                                {
                                    NetMessage.SendTileSquare(-1, num29, num33, 1);
                                }
                            }
                            bool flag8 = false;
                            for (int num53 = num31; num53 < num32; num53++)
                            {
                                for (int num54 = num33; num54 < num34; num54++)
                                {
                                    if (((num29 != num53) || (num30 != num54)) && (Main.tile[num53, num54].active && (Main.tile[num53, num54].type == 0x3b)))
                                    {
                                        SpreadGrass(num53, num54, 0x3b, num52, false);
                                        if (Main.tile[num53, num54].type == num52)
                                        {
                                            SquareTileFrame(num53, num54, true);
                                            flag8 = true;
                                        }
                                    }
                                }
                            }
                            if ((Main.netMode == 2) && flag8)
                            {
                                NetMessage.SendTileSquare(-1, num29, num30, 3);
                            }
                        }
                        continue;
                    }
                    if (flag && (spawnNPC > 0))
                    {
                        SpawnNPC(num29, num30);
                    }
                }
            }
            if (!Main.dayTime)
            {
                float num55 = Main.maxTilesX / 0x1068;
                if (Main.rand.Next(0x1f40) < (10f * num55))
                {
                    int num56 = 12;
                    int num57 = Main.rand.Next(Main.maxTilesX - 50) + 100;
                    num57 *= 0x10;
                    int num58 = Main.rand.Next((int) (Main.maxTilesY * 0.05)) * 0x10;
                    Vector2 vector = new Vector2((float) num57, (float) num58);
                    float speedX = Main.rand.Next(-100, 0x65);
                    float speedY = Main.rand.Next(200) + 100;
                    float num61 = (float) Math.Sqrt((double) ((speedX * speedX) + (speedY * speedY)));
                    num61 = ((float) num56) / num61;
                    speedX *= num61;
                    speedY *= num61;
                    Projectile.NewProjectile(vector.X, vector.Y, speedX, speedY, 12, 0x3e8, 10f, Main.myPlayer);
                }
            }
        }

        public static void WallFrame(int i, int j, bool resetFrame = false)
        {
            if ((((i >= 0) && (j >= 0)) && ((i < Main.maxTilesX) && (j < Main.maxTilesY))) && ((Main.tile[i, j] != null) && (Main.tile[i, j].wall > 0)))
            {
                int num = -1;
                int num2 = -1;
                int num3 = -1;
                int num4 = -1;
                int num5 = -1;
                int num6 = -1;
                int num7 = -1;
                int num8 = -1;
                int wall = Main.tile[i, j].wall;
                if (wall != 0)
                {
                    Rectangle rectangle;
                    byte wallFrameX = Main.tile[i, j].wallFrameX;
                    byte wallFrameY = Main.tile[i, j].wallFrameY;
                    rectangle.X = -1;
                    rectangle.Y = -1;
                    if ((i - 1) < 0)
                    {
                        num = wall;
                        num4 = wall;
                        num6 = wall;
                    }
                    if ((i + 1) >= Main.maxTilesX)
                    {
                        num3 = wall;
                        num5 = wall;
                        num8 = wall;
                    }
                    if ((j - 1) < 0)
                    {
                        num = wall;
                        num2 = wall;
                        num3 = wall;
                    }
                    if ((j + 1) >= Main.maxTilesY)
                    {
                        num6 = wall;
                        num7 = wall;
                        num8 = wall;
                    }
                    if (((i - 1) >= 0) && (Main.tile[i - 1, j] != null))
                    {
                        num4 = Main.tile[i - 1, j].wall;
                    }
                    if (((i + 1) < Main.maxTilesX) && (Main.tile[i + 1, j] != null))
                    {
                        num5 = Main.tile[i + 1, j].wall;
                    }
                    if (((j - 1) >= 0) && (Main.tile[i, j - 1] != null))
                    {
                        num2 = Main.tile[i, j - 1].wall;
                    }
                    if (((j + 1) < Main.maxTilesY) && (Main.tile[i, j + 1] != null))
                    {
                        num7 = Main.tile[i, j + 1].wall;
                    }
                    if ((((i - 1) >= 0) && ((j - 1) >= 0)) && (Main.tile[i - 1, j - 1] != null))
                    {
                        num = Main.tile[i - 1, j - 1].wall;
                    }
                    if ((((i + 1) < Main.maxTilesX) && ((j - 1) >= 0)) && (Main.tile[i + 1, j - 1] != null))
                    {
                        num3 = Main.tile[i + 1, j - 1].wall;
                    }
                    if ((((i - 1) >= 0) && ((j + 1) < Main.maxTilesY)) && (Main.tile[i - 1, j + 1] != null))
                    {
                        num6 = Main.tile[i - 1, j + 1].wall;
                    }
                    if ((((i + 1) < Main.maxTilesX) && ((j + 1) < Main.maxTilesY)) && (Main.tile[i + 1, j + 1] != null))
                    {
                        num8 = Main.tile[i + 1, j + 1].wall;
                    }
                    if (wall == 2)
                    {
                        if (j == ((int) Main.worldSurface))
                        {
                            num7 = wall;
                            num6 = wall;
                            num8 = wall;
                        }
                        else if (j >= ((int) Main.worldSurface))
                        {
                            num7 = wall;
                            num6 = wall;
                            num8 = wall;
                            num2 = wall;
                            num = wall;
                            num3 = wall;
                            num4 = wall;
                            num5 = wall;
                        }
                    }
                    int wallFrameNumber = 0;
                    if (resetFrame)
                    {
                        wallFrameNumber = genRand.Next(0, 3);
                        Main.tile[i, j].wallFrameNumber = (byte) wallFrameNumber;
                    }
                    else
                    {
                        wallFrameNumber = Main.tile[i, j].wallFrameNumber;
                    }
                    if ((rectangle.X < 0) || (rectangle.Y < 0))
                    {
                        if (((num2 == wall) && (num7 == wall)) && ((num4 == wall) & (num5 == wall)))
                        {
                            if ((num != wall) && (num3 != wall))
                            {
                                switch (wallFrameNumber)
                                {
                                    case 0:
                                        rectangle.X = 0x6c;
                                        rectangle.Y = 0x12;
                                        break;

                                    case 1:
                                        rectangle.X = 0x7e;
                                        rectangle.Y = 0x12;
                                        break;

                                    case 2:
                                        rectangle.X = 0x90;
                                        rectangle.Y = 0x12;
                                        break;
                                }
                            }
                            else if ((num6 != wall) && (num8 != wall))
                            {
                                switch (wallFrameNumber)
                                {
                                    case 0:
                                        rectangle.X = 0x6c;
                                        rectangle.Y = 0x24;
                                        break;

                                    case 1:
                                        rectangle.X = 0x7e;
                                        rectangle.Y = 0x24;
                                        break;

                                    case 2:
                                        rectangle.X = 0x90;
                                        rectangle.Y = 0x24;
                                        break;
                                }
                            }
                            else if ((num != wall) && (num6 != wall))
                            {
                                switch (wallFrameNumber)
                                {
                                    case 0:
                                        rectangle.X = 180;
                                        rectangle.Y = 0;
                                        break;

                                    case 1:
                                        rectangle.X = 180;
                                        rectangle.Y = 0x12;
                                        break;

                                    case 2:
                                        rectangle.X = 180;
                                        rectangle.Y = 0x24;
                                        break;
                                }
                            }
                            else if ((num3 != wall) && (num8 != wall))
                            {
                                switch (wallFrameNumber)
                                {
                                    case 0:
                                        rectangle.X = 0xc6;
                                        rectangle.Y = 0;
                                        break;

                                    case 1:
                                        rectangle.X = 0xc6;
                                        rectangle.Y = 0x12;
                                        break;

                                    case 2:
                                        rectangle.X = 0xc6;
                                        rectangle.Y = 0x24;
                                        break;
                                }
                            }
                            else
                            {
                                switch (wallFrameNumber)
                                {
                                    case 0:
                                        rectangle.X = 0x12;
                                        rectangle.Y = 0x12;
                                        break;

                                    case 1:
                                        rectangle.X = 0x24;
                                        rectangle.Y = 0x12;
                                        break;

                                    case 2:
                                        rectangle.X = 0x36;
                                        rectangle.Y = 0x12;
                                        break;
                                }
                            }
                        }
                        else if (((num2 != wall) && (num7 == wall)) && ((num4 == wall) & (num5 == wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0x12;
                                    rectangle.Y = 0;
                                    break;

                                case 1:
                                    rectangle.X = 0x24;
                                    rectangle.Y = 0;
                                    break;

                                case 2:
                                    rectangle.X = 0x36;
                                    rectangle.Y = 0;
                                    break;
                            }
                        }
                        else if (((num2 == wall) && (num7 != wall)) && ((num4 == wall) & (num5 == wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0x12;
                                    rectangle.Y = 0x24;
                                    break;

                                case 1:
                                    rectangle.X = 0x24;
                                    rectangle.Y = 0x24;
                                    break;

                                case 2:
                                    rectangle.X = 0x36;
                                    rectangle.Y = 0x24;
                                    break;
                            }
                        }
                        else if (((num2 == wall) && (num7 == wall)) && ((num4 != wall) & (num5 == wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0;
                                    rectangle.Y = 0;
                                    break;

                                case 1:
                                    rectangle.X = 0;
                                    rectangle.Y = 0x12;
                                    break;

                                case 2:
                                    rectangle.X = 0;
                                    rectangle.Y = 0x24;
                                    break;
                            }
                        }
                        else if (((num2 == wall) && (num7 == wall)) && ((num4 == wall) & (num5 != wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0x48;
                                    rectangle.Y = 0;
                                    break;

                                case 1:
                                    rectangle.X = 0x48;
                                    rectangle.Y = 0x12;
                                    break;

                                case 2:
                                    rectangle.X = 0x48;
                                    rectangle.Y = 0x24;
                                    break;
                            }
                        }
                        else if (((num2 != wall) && (num7 == wall)) && ((num4 != wall) & (num5 == wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0;
                                    rectangle.Y = 0x36;
                                    break;

                                case 1:
                                    rectangle.X = 0x24;
                                    rectangle.Y = 0x36;
                                    break;

                                case 2:
                                    rectangle.X = 0x48;
                                    rectangle.Y = 0x36;
                                    break;
                            }
                        }
                        else if (((num2 != wall) && (num7 == wall)) && ((num4 == wall) & (num5 != wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0x12;
                                    rectangle.Y = 0x36;
                                    break;

                                case 1:
                                    rectangle.X = 0x36;
                                    rectangle.Y = 0x36;
                                    break;

                                case 2:
                                    rectangle.X = 90;
                                    rectangle.Y = 0x36;
                                    break;
                            }
                        }
                        else if (((num2 == wall) && (num7 != wall)) && ((num4 != wall) & (num5 == wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0;
                                    rectangle.Y = 0x48;
                                    break;

                                case 1:
                                    rectangle.X = 0x24;
                                    rectangle.Y = 0x48;
                                    break;

                                case 2:
                                    rectangle.X = 0x48;
                                    rectangle.Y = 0x48;
                                    break;
                            }
                        }
                        else if (((num2 == wall) && (num7 != wall)) && ((num4 == wall) & (num5 != wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0x12;
                                    rectangle.Y = 0x48;
                                    break;

                                case 1:
                                    rectangle.X = 0x36;
                                    rectangle.Y = 0x48;
                                    break;

                                case 2:
                                    rectangle.X = 90;
                                    rectangle.Y = 0x48;
                                    break;
                            }
                        }
                        else if (((num2 == wall) && (num7 == wall)) && ((num4 != wall) & (num5 != wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 90;
                                    rectangle.Y = 0;
                                    break;

                                case 1:
                                    rectangle.X = 90;
                                    rectangle.Y = 0x12;
                                    break;

                                case 2:
                                    rectangle.X = 90;
                                    rectangle.Y = 0x24;
                                    break;
                            }
                        }
                        else if (((num2 != wall) && (num7 != wall)) && ((num4 == wall) & (num5 == wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0x6c;
                                    rectangle.Y = 0x48;
                                    break;

                                case 1:
                                    rectangle.X = 0x7e;
                                    rectangle.Y = 0x48;
                                    break;

                                case 2:
                                    rectangle.X = 0x90;
                                    rectangle.Y = 0x48;
                                    break;
                            }
                        }
                        else if (((num2 != wall) && (num7 == wall)) && ((num4 != wall) & (num5 != wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0x6c;
                                    rectangle.Y = 0;
                                    break;

                                case 1:
                                    rectangle.X = 0x7e;
                                    rectangle.Y = 0;
                                    break;

                                case 2:
                                    rectangle.X = 0x90;
                                    rectangle.Y = 0;
                                    break;
                            }
                        }
                        else if (((num2 == wall) && (num7 != wall)) && ((num4 != wall) & (num5 != wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0x6c;
                                    rectangle.Y = 0x36;
                                    break;

                                case 1:
                                    rectangle.X = 0x7e;
                                    rectangle.Y = 0x36;
                                    break;

                                case 2:
                                    rectangle.X = 0x90;
                                    rectangle.Y = 0x36;
                                    break;
                            }
                        }
                        else if (((num2 != wall) && (num7 != wall)) && ((num4 != wall) & (num5 == wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0xa2;
                                    rectangle.Y = 0;
                                    break;

                                case 1:
                                    rectangle.X = 0xa2;
                                    rectangle.Y = 0x12;
                                    break;

                                case 2:
                                    rectangle.X = 0xa2;
                                    rectangle.Y = 0x24;
                                    break;
                            }
                        }
                        else if (((num2 != wall) && (num7 != wall)) && ((num4 == wall) & (num5 != wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0xd8;
                                    rectangle.Y = 0;
                                    break;

                                case 1:
                                    rectangle.X = 0xd8;
                                    rectangle.Y = 0x12;
                                    break;

                                case 2:
                                    rectangle.X = 0xd8;
                                    rectangle.Y = 0x24;
                                    break;
                            }
                        }
                        else if (((num2 != wall) && (num7 != wall)) && ((num4 != wall) & (num5 != wall)))
                        {
                            switch (wallFrameNumber)
                            {
                                case 0:
                                    rectangle.X = 0xa2;
                                    rectangle.Y = 0x36;
                                    break;

                                case 1:
                                    rectangle.X = 180;
                                    rectangle.Y = 0x36;
                                    break;

                                case 2:
                                    rectangle.X = 0xc6;
                                    rectangle.Y = 0x36;
                                    break;
                            }
                        }
                    }
                    if ((rectangle.X <= -1) || (rectangle.Y <= -1))
                    {
                        if (wallFrameNumber <= 0)
                        {
                            rectangle.X = 0x12;
                            rectangle.Y = 0x12;
                        }
                        if (wallFrameNumber == 1)
                        {
                            rectangle.X = 0x24;
                            rectangle.Y = 0x12;
                        }
                        if (wallFrameNumber >= 2)
                        {
                            rectangle.X = 0x36;
                            rectangle.Y = 0x12;
                        }
                    }
                    Main.tile[i, j].wallFrameX = (byte) rectangle.X;
                    Main.tile[i, j].wallFrameY = (byte) rectangle.Y;
                }
            }
        }

        public static void WaterCheck()
        {
            Liquid.numLiquid = 0;
            LiquidBuffer.numLiquidBuffer = 0;
            for (int i = 1; i < (Main.maxTilesX - 1); i++)
            {
                for (int j = Main.maxTilesY - 2; j > 0; j--)
                {
                    Main.tile[i, j].checkingLiquid = false;
                    if (((Main.tile[i, j].liquid > 0) && Main.tile[i, j].active) && (Main.tileSolid[Main.tile[i, j].type] && !Main.tileSolidTop[Main.tile[i, j].type]))
                    {
                        Main.tile[i, j].liquid = 0;
                    }
                    else if (Main.tile[i, j].liquid > 0)
                    {
                        if (Main.tile[i, j].active)
                        {
                            if (Main.tileWaterDeath[Main.tile[i, j].type])
                            {
                                KillTile(i, j, false, false, false);
                            }
                            if (Main.tile[i, j].lava && Main.tileLavaDeath[Main.tile[i, j].type])
                            {
                                KillTile(i, j, false, false, false);
                            }
                        }
                        if (((!Main.tile[i, j + 1].active || !Main.tileSolid[Main.tile[i, j + 1].type]) || Main.tileSolidTop[Main.tile[i, j + 1].type]) && (Main.tile[i, j + 1].liquid < 0xff))
                        {
                            if (Main.tile[i, j + 1].liquid > 250)
                            {
                                Main.tile[i, j + 1].liquid = 0xff;
                            }
                            else
                            {
                                Liquid.AddWater(i, j);
                            }
                        }
                        if (((!Main.tile[i - 1, j].active || !Main.tileSolid[Main.tile[i - 1, j].type]) || Main.tileSolidTop[Main.tile[i - 1, j].type]) && (Main.tile[i - 1, j].liquid != Main.tile[i, j].liquid))
                        {
                            Liquid.AddWater(i, j);
                        }
                        else if (((!Main.tile[i + 1, j].active || !Main.tileSolid[Main.tile[i + 1, j].type]) || Main.tileSolidTop[Main.tile[i + 1, j].type]) && (Main.tile[i + 1, j].liquid != Main.tile[i, j].liquid))
                        {
                            Liquid.AddWater(i, j);
                        }
                        if (Main.tile[i, j].lava)
                        {
                            if ((Main.tile[i - 1, j].liquid > 0) && !Main.tile[i - 1, j].lava)
                            {
                                Liquid.AddWater(i, j);
                            }
                            else if ((Main.tile[i + 1, j].liquid > 0) && !Main.tile[i + 1, j].lava)
                            {
                                Liquid.AddWater(i, j);
                            }
                            else if ((Main.tile[i, j - 1].liquid > 0) && !Main.tile[i, j - 1].lava)
                            {
                                Liquid.AddWater(i, j);
                            }
                            else if ((Main.tile[i, j + 1].liquid > 0) && !Main.tile[i, j + 1].lava)
                            {
                                Liquid.AddWater(i, j);
                            }
                        }
                    }
                }
            }
        }

        public static void worldGenCallBack(object threadContext)
        {
            Main.PlaySound(10, -1, -1, 1);
            clearWorld();
            generateWorld(-1);
            saveWorld(true);
            Main.LoadWorlds();
            if (Main.menuMode == 10)
            {
                Main.menuMode = 6;
            }
            Main.PlaySound(10, -1, -1, 1);
        }
    }
}

