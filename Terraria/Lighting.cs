namespace Terraria
{
    using Microsoft.Xna.Framework;
    using System;

    public class Lighting
    {
        public static float[,] color = new float[((Main.screenWidth / 0x10) + 0x2a) + 10, ((Main.screenHeight / 0x10) + 0x2a) + 10];
        private static int firstTileX;
        private static int firstTileY;
        private static int firstToLightX;
        private static int firstToLightY;
        private static int lastTileX;
        private static int lastTileY;
        private static int lastToLightX;
        private static int lastToLightY;
        private static float lightColor = 0f;
        public static int lightCounter = 0;
        public static int lightPasses = 2;
        public static int lightSkip = 1;
        private static int maxTempLights = 100;
        public const int offScreenTiles = 0x15;
        private static float[] tempLight = new float[maxTempLights];
        private static int tempLightCount;
        private static int[] tempLightX = new int[maxTempLights];
        private static int[] tempLightY = new int[maxTempLights];

        public static void addLight(int i, int j, float Lightness)
        {
            if ((tempLightCount != maxTempLights) && (((((i - firstTileX) + 0x15) >= 0) && (((i - firstTileX) + 0x15) < (((Main.screenWidth / 0x10) + 0x2a) + 10))) && ((((j - firstTileY) + 0x15) >= 0) && (((j - firstTileY) + 0x15) < (((Main.screenHeight / 0x10) + 0x2a) + 10)))))
            {
                for (int k = 0; k < tempLightCount; k++)
                {
                    if (((tempLightX[k] == i) && (tempLightY[k] == j)) && (Lightness <= tempLight[k]))
                    {
                        return;
                    }
                }
                tempLightX[tempLightCount] = i;
                tempLightY[tempLightCount] = j;
                tempLight[tempLightCount] = Lightness;
                tempLightCount++;
            }
        }

        public static float Brightness(int x, int y)
        {
            int num = (x - firstTileX) + 0x15;
            int num2 = (y - firstTileY) + 0x15;
            if (((num >= 0) && (num2 >= 0)) && ((num < (((Main.screenWidth / 0x10) + 0x2a) + 10)) && (num2 < (((Main.screenHeight / 0x10) + 0x2a) + 10))))
            {
                return color[num, num2];
            }
            return 0f;
        }

        public static Color GetBlackness(int x, int y)
        {
            int num = (x - firstTileX) + 0x15;
            int num2 = (y - firstTileY) + 0x15;
            if (((num < 0) || (num2 < 0)) || ((num >= (((Main.screenWidth / 0x10) + 0x2a) + 10)) || (num2 >= (((Main.screenHeight / 0x10) + 0x2a) + 10))))
            {
                return Color.Black;
            }
            return new Color(0, 0, 0, (byte) (255f - (255f * color[num, num2])));
        }

        public static Color GetColor(int x, int y)
        {
            int num = (x - firstTileX) + 0x15;
            int num2 = (y - firstTileY) + 0x15;
            if (((num < 0) || (num2 < 0)) || ((num >= (((Main.screenWidth / 0x10) + 0x2a) + 10)) || (num2 >= (((Main.screenHeight / 0x10) + 0x2a) + 10))))
            {
                return Color.Black;
            }
            return new Color((byte) (255f * color[num, num2]), (byte) (255f * color[num, num2]), (byte) (255f * color[num, num2]), 0xff);
        }

        public static Color GetColor(int x, int y, Color oldColor)
        {
            int num = (x - firstTileX) + 0x15;
            int num2 = (y - firstTileY) + 0x15;
            if (Main.gameMenu)
            {
                return oldColor;
            }
            if (((num < 0) || (num2 < 0)) || ((num >= (((Main.screenWidth / 0x10) + 0x2a) + 10)) || (num2 >= (((Main.screenHeight / 0x10) + 0x2a) + 10))))
            {
                return Color.Black;
            }
            Color white = Color.White;
            white.R = (byte) (oldColor.R * color[num, num2]);
            white.G = (byte) (oldColor.G * color[num, num2]);
            white.B = (byte) (oldColor.B * color[num, num2]);
            return white;
        }

        private static void LightColor(int i, int j)
        {
            int num = i - firstToLightX;
            int num2 = j - firstToLightY;
            try
            {
                if (color[num, num2] > lightColor)
                {
                    lightColor = color[num, num2];
                }
                else
                {
                    if (lightColor == 0f)
                    {
                        return;
                    }
                    color[num, num2] = lightColor;
                }
                float num3 = 0.04f;
                if (Main.tile[i, j].active && Main.tileBlockLight[Main.tile[i, j].type])
                {
                    num3 = 0.16f;
                }
                float num4 = lightColor - num3;
                if (num4 < 0f)
                {
                    lightColor = 0f;
                }
                else
                {
                    lightColor -= num3;
                    if (((lightColor > 0f) && (!Main.tile[i, j].active || !Main.tileSolid[Main.tile[i, j].type])) && (j < Main.worldSurface))
                    {
                        Main.tile[i, j].lighted = true;
                    }
                }
            }
            catch
            {
            }
        }

        public static int LightingX(int lightX)
        {
            if (lightX < 0)
            {
                return 0;
            }
            if (lightX >= (((Main.screenWidth / 0x10) + 0x2a) + 10))
            {
                return ((((Main.screenWidth / 0x10) + 0x2a) + 10) - 1);
            }
            return lightX;
        }

        public static int LightingY(int lightY)
        {
            if (lightY < 0)
            {
                return 0;
            }
            if (lightY >= (((Main.screenHeight / 0x10) + 0x2a) + 10))
            {
                return ((((Main.screenHeight / 0x10) + 0x2a) + 10) - 1);
            }
            return lightY;
        }

        public static void LightTiles(int firstX, int lastX, int firstY, int lastY)
        {
            firstTileX = firstX;
            lastTileX = lastX;
            firstTileY = firstY;
            lastTileY = lastY;
            lightCounter++;
            if (lightCounter <= lightSkip)
            {
                tempLightCount = 0;
                int num = ((Main.screenWidth / 0x10) + 0x2a) + 10;
                int num2 = ((Main.screenHeight / 0x10) + 0x2a) + 10;
                if (((int) (Main.screenPosition.X / 16f)) < ((int) (Main.screenLastPosition.X / 16f)))
                {
                    for (int i = num - 1; i > 1; i--)
                    {
                        for (int j = 0; j < num2; j++)
                        {
                            color[i, j] = color[i - 1, j];
                        }
                    }
                }
                else if (((int) (Main.screenPosition.X / 16f)) > ((int) (Main.screenLastPosition.X / 16f)))
                {
                    for (int k = 0; k < (num - 1); k++)
                    {
                        for (int m = 0; m < num2; m++)
                        {
                            color[k, m] = color[k + 1, m];
                        }
                    }
                }
                if (((int) (Main.screenPosition.Y / 16f)) < ((int) (Main.screenLastPosition.Y / 16f)))
                {
                    for (int n = num2 - 1; n > 1; n--)
                    {
                        for (int num8 = 0; num8 < num; num8++)
                        {
                            color[num8, n] = color[num8, n - 1];
                        }
                    }
                }
                else if (((int) (Main.screenPosition.Y / 16f)) > ((int) (Main.screenLastPosition.Y / 16f)))
                {
                    for (int num9 = 0; num9 < (num2 - 1); num9++)
                    {
                        for (int num10 = 0; num10 < num; num10++)
                        {
                            color[num10, num9] = color[num10, num9 + 1];
                        }
                    }
                }
            }
            else
            {
                lightCounter = 0;
                firstToLightX = firstTileX - 0x15;
                firstToLightY = firstTileY - 0x15;
                lastToLightX = lastTileX + 0x15;
                lastToLightY = lastTileY + 0x15;
                for (int num11 = 0; num11 < (((Main.screenWidth / 0x10) + 0x2a) + 10); num11++)
                {
                    for (int num12 = 0; num12 < (((Main.screenHeight / 0x10) + 0x2a) + 10); num12++)
                    {
                        color[num11, num12] = 0f;
                    }
                }
                for (int num13 = 0; num13 < tempLightCount; num13++)
                {
                    if (((((tempLightX[num13] - firstTileX) + 0x15) >= 0) && (((tempLightX[num13] - firstTileX) + 0x15) < (((Main.screenWidth / 0x10) + 0x2a) + 10))) && ((((tempLightY[num13] - firstTileY) + 0x15) >= 0) && (((tempLightY[num13] - firstTileY) + 0x15) < (((Main.screenHeight / 0x10) + 0x2a) + 10))))
                    {
                        color[(tempLightX[num13] - firstTileX) + 0x15, (tempLightY[num13] - firstTileY) + 0x15] = tempLight[num13];
                    }
                }
                tempLightCount = 0;
                Main.evilTiles = 0;
                Main.meteorTiles = 0;
                Main.jungleTiles = 0;
                Main.dungeonTiles = 0;
                for (int num14 = firstToLightX; num14 < lastToLightX; num14++)
                {
                    for (int num15 = firstToLightY; num15 < lastToLightY; num15++)
                    {
                        if (((num14 >= 0) && (num14 < Main.maxTilesX)) && ((num15 >= 0) && (num15 < Main.maxTilesY)))
                        {
                            if (Main.tile[num14, num15] == null)
                            {
                                Main.tile[num14, num15] = new Tile();
                            }
                            if (Main.tile[num14, num15].active)
                            {
                                if (((Main.tile[num14, num15].type == 0x17) || (Main.tile[num14, num15].type == 0x18)) || ((Main.tile[num14, num15].type == 0x19) || (Main.tile[num14, num15].type == 0x20)))
                                {
                                    Main.evilTiles++;
                                }
                                else if (Main.tile[num14, num15].type == 0x1b)
                                {
                                    Main.evilTiles -= 5;
                                }
                                else if (Main.tile[num14, num15].type == 0x25)
                                {
                                    Main.meteorTiles++;
                                }
                                else if (Main.tileDungeon[Main.tile[num14, num15].type])
                                {
                                    Main.dungeonTiles++;
                                }
                                else if ((Main.tile[num14, num15].type == 60) || (Main.tile[num14, num15].type == 0x3d))
                                {
                                    Main.jungleTiles++;
                                }
                            }
                            if (Main.tile[num14, num15] == null)
                            {
                                Main.tile[num14, num15] = new Tile();
                            }
                            if (Main.lightTiles)
                            {
                                color[(num14 - firstTileX) + 0x15, (num15 - firstTileY) + 0x15] = ((float) Main.tileColor.R) / 255f;
                            }
                            if (Main.tile[num14, num15].lava)
                            {
                                float num16 = ((Main.tile[num14, num15].liquid / 0xff) * 0.6f) + 0.1f;
                                if (color[num14 - firstToLightX, num15 - firstToLightY] < num16)
                                {
                                    color[num14 - firstToLightX, num15 - firstToLightY] = num16;
                                }
                            }
                            if ((((!Main.tile[num14, num15].active || !Main.tileSolid[Main.tile[num14, num15].type]) || ((Main.tile[num14, num15].type == 0x25) || (Main.tile[num14, num15].type == 0x3a))) || ((Main.tile[num14, num15].type == 70) || (Main.tile[num14, num15].type == 0x4c))) && ((((Main.tile[num14, num15].lighted || (Main.tile[num14, num15].type == 4)) || ((Main.tile[num14, num15].type == 0x11) || (Main.tile[num14, num15].type == 0x1f))) || (((Main.tile[num14, num15].type == 0x21) || (Main.tile[num14, num15].type == 0x22)) || ((Main.tile[num14, num15].type == 0x23) || (Main.tile[num14, num15].type == 0x24)))) || (((((Main.tile[num14, num15].type == 0x25) || (Main.tile[num14, num15].type == 0x2a)) || ((Main.tile[num14, num15].type == 0x31) || (Main.tile[num14, num15].type == 0x3a))) || (((Main.tile[num14, num15].type == 0x3d) || (Main.tile[num14, num15].type == 70)) || ((Main.tile[num14, num15].type == 0x47) || (Main.tile[num14, num15].type == 0x48)))) || (((Main.tile[num14, num15].type == 0x4c) || (Main.tile[num14, num15].type == 0x4d)) || ((Main.tile[num14, num15].type == 0x13) || (Main.tile[num14, num15].type == 0x1a))))))
                            {
                                if (((((color[num14 - firstToLightX, num15 - firstToLightY] * 255f) < Main.tileColor.R) && (Main.tileColor.R > (color[num14 - firstToLightX, num15 - firstToLightY] * 255f))) && ((Main.tile[num14, num15].wall == 0) && (num15 < Main.worldSurface))) && (Main.tile[num14, num15].liquid < 0xff))
                                {
                                    color[num14 - firstToLightX, num15 - firstToLightY] = ((float) Main.tileColor.R) / 255f;
                                }
                                if (((Main.tile[num14, num15].type == 4) || (Main.tile[num14, num15].type == 0x21)) || (((Main.tile[num14, num15].type == 0x22) || (Main.tile[num14, num15].type == 0x23)) || (Main.tile[num14, num15].type == 0x24)))
                                {
                                    color[num14 - firstToLightX, num15 - firstToLightY] = 1f;
                                }
                                else if ((Main.tile[num14, num15].type == 0x11) && (color[num14 - firstToLightX, num15 - firstToLightY] < 0.8f))
                                {
                                    color[num14 - firstToLightX, num15 - firstToLightY] = 0.8f;
                                }
                                else if ((Main.tile[num14, num15].type == 0x4d) && (color[num14 - firstToLightX, num15 - firstToLightY] < 0.8f))
                                {
                                    color[num14 - firstToLightX, num15 - firstToLightY] = 0.8f;
                                }
                                else if ((Main.tile[num14, num15].type == 0x25) && (color[num14 - firstToLightX, num15 - firstToLightY] < 0.6f))
                                {
                                    color[num14 - firstToLightX, num15 - firstToLightY] = 0.6f;
                                }
                                else if ((Main.tile[num14, num15].type == 0x3a) && (color[num14 - firstToLightX, num15 - firstToLightY] < 0.6f))
                                {
                                    color[num14 - firstToLightX, num15 - firstToLightY] = 0.6f;
                                }
                                else if ((Main.tile[num14, num15].type == 0x4c) && (color[num14 - firstToLightX, num15 - firstToLightY] < 0.6f))
                                {
                                    color[num14 - firstToLightX, num15 - firstToLightY] = 0.6f;
                                }
                                else if ((Main.tile[num14, num15].type == 0x2a) && (color[num14 - firstToLightX, num15 - firstToLightY] < 0.75f))
                                {
                                    color[num14 - firstToLightX, num15 - firstToLightY] = 0.75f;
                                }
                                else if ((Main.tile[num14, num15].type == 0x31) && (color[num14 - firstToLightX, num15 - firstToLightY] < 0.75f))
                                {
                                    color[num14 - firstToLightX, num15 - firstToLightY] = 0.75f;
                                }
                                else if (((Main.tile[num14, num15].type == 70) || (Main.tile[num14, num15].type == 0x47)) || (Main.tile[num14, num15].type == 0x48))
                                {
                                    float num17 = Main.rand.Next(0x30, 0x34) * 0.01f;
                                    if (color[num14 - firstToLightX, num15 - firstToLightY] < num17)
                                    {
                                        color[num14 - firstToLightX, num15 - firstToLightY] = num17;
                                    }
                                }
                                else if (((Main.tile[num14, num15].type == 0x3d) && (Main.tile[num14, num15].frameX == 0x90)) && (color[num14 - firstToLightX, num15 - firstToLightY] < 0.75f))
                                {
                                    color[num14 - firstToLightX, num15 - firstToLightY] = 0.75f;
                                }
                                else if ((Main.tile[num14, num15].type == 0x1f) || (Main.tile[num14, num15].type == 0x1a))
                                {
                                    float num18 = Main.rand.Next(-5, 6) * 0.01f;
                                    if (color[num14 - firstToLightX, num15 - firstToLightY] < (0.4f + num18))
                                    {
                                        color[num14 - firstToLightX, num15 - firstToLightY] = 0.4f + num18;
                                    }
                                }
                            }
                        }
                    }
                }
                for (int num19 = 0; num19 < lightPasses; num19++)
                {
                    for (int num20 = firstToLightX; num20 < lastToLightX; num20++)
                    {
                        lightColor = 0f;
                        for (int num21 = firstToLightY; num21 < lastToLightY; num21++)
                        {
                            LightColor(num20, num21);
                        }
                    }
                    for (int num22 = firstToLightX; num22 < lastToLightX; num22++)
                    {
                        lightColor = 0f;
                        for (int num23 = lastToLightY; num23 >= firstToLightY; num23--)
                        {
                            LightColor(num22, num23);
                        }
                    }
                    for (int num24 = firstToLightY; num24 < lastToLightY; num24++)
                    {
                        lightColor = 0f;
                        for (int num25 = firstToLightX; num25 < lastToLightX; num25++)
                        {
                            LightColor(num25, num24);
                        }
                    }
                    for (int num26 = firstToLightY; num26 < lastToLightY; num26++)
                    {
                        lightColor = 0f;
                        for (int num27 = lastToLightX; num27 >= firstToLightX; num27--)
                        {
                            LightColor(num27, num26);
                        }
                    }
                }
            }
        }
    }
}

