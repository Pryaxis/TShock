namespace Terraria
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.InteropServices;

    public class NPC
    {
        public bool active;
        private static int activeRangeX = (Main.screenWidth * 2);
        private static int activeRangeY = (Main.screenHeight * 2);
        private static int activeTime = 0x3e8;

        public int aiAction;
        public int aiStyle;
        public int alpha;
        public bool behindTiles;
        public bool boss;
        public bool closeDoor;
        public bool collideX;
        public bool collideY;
        public Color color;
        public int damage;
        private static int defaultMaxSpawns = ShankShock.defaultMaxSpawns;
        private static int defaultSpawnRate = ShankShock.defaultSpawnRate;
        public int defense;
        public int direction = 1;
        public int directionY = 1;
        public int doorX;
        public int doorY;
        public static bool downedBoss1 = false;
        public static bool downedBoss2 = false;
        public static bool downedBoss3 = false;
        public Rectangle frame;
        public double frameCounter;
        public bool friendly;
        public int friendlyRegen;
        public int height;
        public bool homeless;
        public int homeTileX = -1;
        public int homeTileY = -1;
        public int[] immune = new int[9];
        public static int immuneTime = 20;
        public float knockBackResist = 1f;
        public bool lavaWet;
        public int life;
        public int lifeMax;
        public static int maxAI = 4;
        private static int maxSpawns = defaultMaxSpawns;
        public string name;
        public bool netUpdate;
        public bool noGravity;
        public bool noTileCollide;
        public int oldDirection;
        public int oldDirectionY;
        public Vector2 oldPosition;
        public int oldTarget;
        public Vector2 oldVelocity;
        public Vector2 position;
        public float rotation;
        public static int safeRangeX = ((int) ((Main.screenWidth / 0x10) * 0.55));
        public static int safeRangeY = ((int) ((Main.screenHeight / 0x10) * 0.55));
        public float scale = 1f;
        public int soundDelay;
        public int soundHit;
        public int soundKilled;
        private static int spawnRangeX = ((int) ((Main.screenWidth / 0x10) * 1.2));
        private static int spawnRangeY = ((int) ((Main.screenHeight / 0x10) * 1.2));
        private static int spawnRate = defaultSpawnRate;
        private static int spawnSpaceX = 4;
        private static int spawnSpaceY = 4;
        public int spriteDirection = -1;
        public int target = -1;
        public Rectangle targetRect;
        public int timeLeft;
        public bool townNPC;
        private static int townRangeX = (Main.screenWidth * 3);
        private static int townRangeY = (Main.screenHeight * 3);
        public int type;
        public float value;
        public Vector2 velocity;
        public bool wet;
        public byte wetCount;
        public int whoAmI;
        public int width;
        public float[] ai = new float[maxAI];
        public void AI()
        {
            int num66;
            int num67;
            bool flag7;
            bool flag8;
            Color color;
            if (this.aiStyle == 0)
            {
                this.velocity.X *= 0.93f;
                if ((this.velocity.X > -0.1) && (this.velocity.X < 0.1))
                {
                    this.velocity.X = 0f;
                    return;
                }
                return;
            }
            if (this.aiStyle == 1)
            {
                this.aiAction = 0;
                if (this.ai[2] == 0f)
                {
                    this.ai[0] = -100f;
                    this.ai[2] = 1f;
                    this.TargetClosest();
                }
                if (this.velocity.Y != 0f)
                {
                    if ((this.target < 8) && (((this.direction == 1) && (this.velocity.X < 3f)) || ((this.direction == -1) && (this.velocity.X > -3f))))
                    {
                        if (((this.direction == -1) && (this.velocity.X < 0.1)) || ((this.direction == 1) && (this.velocity.X > -0.1)))
                        {
                            this.velocity.X += 0.2f * this.direction;
                            return;
                        }
                        this.velocity.X *= 0.93f;
                        return;
                    }
                }
                else
                {
                    if (this.ai[3] == this.position.X)
                    {
                        this.direction *= -1;
                    }
                    this.ai[3] = 0f;
                    this.velocity.X *= 0.8f;
                    if ((this.velocity.X > -0.1) && (this.velocity.X < 0.1))
                    {
                        this.velocity.X = 0f;
                    }
                    if ((!Main.dayTime || (this.life != this.lifeMax)) || (this.position.Y > (Main.worldSurface * 16.0)))
                    {
                        this.ai[0]++;
                    }
                    this.ai[0]++;
                    if (this.ai[0] >= 0f)
                    {
                        if ((!Main.dayTime || (this.life != this.lifeMax)) || (this.position.Y > (Main.worldSurface * 16.0)))
                        {
                            this.TargetClosest();
                        }
                        if (this.ai[1] == 2f)
                        {
                            this.velocity.Y = -8f;
                            this.velocity.X += 3 * this.direction;
                            this.ai[0] = -200f;
                            this.ai[1] = 0f;
                            this.ai[3] = this.position.X;
                            return;
                        }
                        this.velocity.Y = -6f;
                        this.velocity.X += 2 * this.direction;
                        this.ai[0] = -120f;
                        this.ai[1]++;
                        return;
                    }
                    if (this.ai[0] >= -30f)
                    {
                        this.aiAction = 1;
                        return;
                    }
                }
                return;
            }
            if (this.aiStyle == 2)
            {
                this.noGravity = true;
                if (this.collideX)
                {
                    this.velocity.X = this.oldVelocity.X * -0.5f;
                    if (((this.direction == -1) && (this.velocity.X > 0f)) && (this.velocity.X < 2f))
                    {
                        this.velocity.X = 2f;
                    }
                    if (((this.direction == 1) && (this.velocity.X < 0f)) && (this.velocity.X > -2f))
                    {
                        this.velocity.X = -2f;
                    }
                }
                if (this.collideY)
                {
                    this.velocity.Y = this.oldVelocity.Y * -0.5f;
                    if ((this.velocity.Y > 0f) && (this.velocity.Y < 1f))
                    {
                        this.velocity.Y = 1f;
                    }
                    if ((this.velocity.Y < 0f) && (this.velocity.Y > -1f))
                    {
                        this.velocity.Y = -1f;
                    }
                }
                if ((Main.dayTime && (this.position.Y <= (Main.worldSurface * 16.0))) && (this.type == 2))
                {
                    if (this.timeLeft > 10)
                    {
                        this.timeLeft = 10;
                    }
                    this.directionY = -1;
                    if (this.velocity.Y > 0f)
                    {
                        this.direction = 1;
                    }
                    this.direction = -1;
                    if (this.velocity.X > 0f)
                    {
                        this.direction = 1;
                    }
                }
                else
                {
                    this.TargetClosest();
                }
                if ((this.direction == -1) && (this.velocity.X > -4f))
                {
                    this.velocity.X -= 0.1f;
                    if (this.velocity.X > 4f)
                    {
                        this.velocity.X -= 0.1f;
                    }
                    else if (this.velocity.X > 0f)
                    {
                        this.velocity.X += 0.05f;
                    }
                    if (this.velocity.X < -4f)
                    {
                        this.velocity.X = -4f;
                    }
                }
                else if ((this.direction == 1) && (this.velocity.X < 4f))
                {
                    this.velocity.X += 0.1f;
                    if (this.velocity.X < -4f)
                    {
                        this.velocity.X += 0.1f;
                    }
                    else if (this.velocity.X < 0f)
                    {
                        this.velocity.X -= 0.05f;
                    }
                    if (this.velocity.X > 4f)
                    {
                        this.velocity.X = 4f;
                    }
                }
                if ((this.directionY == -1) && (this.velocity.Y > -1.5))
                {
                    this.velocity.Y -= 0.04f;
                    if (this.velocity.Y > 1.5)
                    {
                        this.velocity.Y -= 0.05f;
                    }
                    else if (this.velocity.Y > 0f)
                    {
                        this.velocity.Y += 0.03f;
                    }
                    if (this.velocity.Y < -1.5)
                    {
                        this.velocity.Y = -1.5f;
                    }
                }
                else if ((this.directionY == 1) && (this.velocity.Y < 1.5))
                {
                    this.velocity.Y += 0.04f;
                    if (this.velocity.Y < -1.5)
                    {
                        this.velocity.Y += 0.05f;
                    }
                    else if (this.velocity.Y < 0f)
                    {
                        this.velocity.Y -= 0.03f;
                    }
                    if (this.velocity.Y > 1.5)
                    {
                        this.velocity.Y = 1.5f;
                    }
                }
                if ((this.type == 2) && (Main.rand.Next(40) == 0))
                {
                    int index = Dust.NewDust(new Vector2(this.position.X, this.position.Y + (this.height * 0.25f)), this.width, (int) (this.height * 0.5f), 5, this.velocity.X, 2f, 0, new Color(), 1f);
                    Main.dust[index].velocity.X *= 0.5f;
                    Main.dust[index].velocity.Y *= 0.1f;
                    return;
                }
                return;
            }
            if (this.aiStyle == 3)
            {
                int num2 = 60;
                bool flag = false;
                if ((this.velocity.Y == 0f) && (((this.velocity.X > 0f) && (this.direction < 0)) || ((this.velocity.X < 0f) && (this.direction > 0))))
                {
                    flag = true;
                }
                if (((this.position.X == this.oldPosition.X) || (this.ai[3] >= num2)) || flag)
                {
                    this.ai[3]++;
                }
                else if ((Math.Abs(this.velocity.X) > 0.9) && (this.ai[3] > 0f))
                {
                    this.ai[3]--;
                }
                if (this.ai[3] > (num2 * 10))
                {
                    this.ai[3] = 0f;
                }
                if (this.ai[3] == num2)
                {
                    this.netUpdate = true;
                }
                if ((((!Main.dayTime || (this.position.Y > (Main.worldSurface * 16.0))) || ((this.type == 0x1a) || (this.type == 0x1b))) || ((this.type == 0x1c) || (this.type == 0x1f))) && (this.ai[3] < num2))
                {
                    if ((((this.type == 3) || (this.type == 0x15)) || (this.type == 0x1f)) && (Main.rand.Next(0x3e8) == 0))
                    {
                        Main.PlaySound(14, (int) this.position.X, (int) this.position.Y, 1);
                    }
                    this.TargetClosest();
                }
                else
                {
                    if (this.timeLeft > 10)
                    {
                        this.timeLeft = 10;
                    }
                    if (this.velocity.X == 0f)
                    {
                        if (this.velocity.Y == 0f)
                        {
                            this.ai[0]++;
                            if (this.ai[0] >= 2f)
                            {
                                this.direction *= -1;
                                this.spriteDirection = this.direction;
                                this.ai[0] = 0f;
                            }
                        }
                    }
                    else
                    {
                        this.ai[0] = 0f;
                    }
                    if (this.direction == 0)
                    {
                        this.direction = 1;
                    }
                }
                if (this.type == 0x1b)
                {
                    if ((this.velocity.X < -2f) || (this.velocity.X > 2f))
                    {
                        if (this.velocity.Y == 0f)
                        {
                            this.velocity = (Vector2) (this.velocity * 0.8f);
                        }
                    }
                    else if ((this.velocity.X < 2f) && (this.direction == 1))
                    {
                        this.velocity.X += 0.07f;
                        if (this.velocity.X > 2f)
                        {
                            this.velocity.X = 2f;
                        }
                    }
                    else if ((this.velocity.X > -2f) && (this.direction == -1))
                    {
                        this.velocity.X -= 0.07f;
                        if (this.velocity.X < -2f)
                        {
                            this.velocity.X = -2f;
                        }
                    }
                }
                else if (((this.type == 0x15) || (this.type == 0x1a)) || (this.type == 0x1f))
                {
                    if ((this.velocity.X < -1.5f) || (this.velocity.X > 1.5f))
                    {
                        if (this.velocity.Y == 0f)
                        {
                            this.velocity = (Vector2) (this.velocity * 0.8f);
                        }
                    }
                    else if ((this.velocity.X < 1.5f) && (this.direction == 1))
                    {
                        this.velocity.X += 0.07f;
                        if (this.velocity.X > 1.5f)
                        {
                            this.velocity.X = 1.5f;
                        }
                    }
                    else if ((this.velocity.X > -1.5f) && (this.direction == -1))
                    {
                        this.velocity.X -= 0.07f;
                        if (this.velocity.X < -1.5f)
                        {
                            this.velocity.X = -1.5f;
                        }
                    }
                }
                else if ((this.velocity.X < -1f) || (this.velocity.X > 1f))
                {
                    if (this.velocity.Y == 0f)
                    {
                        this.velocity = (Vector2) (this.velocity * 0.8f);
                    }
                }
                else if ((this.velocity.X < 1f) && (this.direction == 1))
                {
                    this.velocity.X += 0.07f;
                    if (this.velocity.X > 1f)
                    {
                        this.velocity.X = 1f;
                    }
                }
                else if ((this.velocity.X > -1f) && (this.direction == -1))
                {
                    this.velocity.X -= 0.07f;
                    if (this.velocity.X < -1f)
                    {
                        this.velocity.X = -1f;
                    }
                }
                if (this.velocity.Y != 0f)
                {
                    this.ai[1] = 0f;
                    this.ai[2] = 0f;
                    return;
                }
                int i = (int) (((this.position.X + (this.width / 2)) + (15 * this.direction)) / 16f);
                int j = (int) (((this.position.Y + this.height) - 16f) / 16f);
                if (Main.tile[i, j] == null)
                {
                    Main.tile[i, j] = new Tile();
                }
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
                if (Main.tile[i + this.direction, j - 1] == null)
                {
                    Main.tile[i + this.direction, j - 1] = new Tile();
                }
                if (Main.tile[i + this.direction, j + 1] == null)
                {
                    Main.tile[i + this.direction, j + 1] = new Tile();
                }
                if (!Main.tile[i, j - 1].active || (Main.tile[i, j - 1].type != 10))
                {
                    if (((this.velocity.X < 0f) && (this.spriteDirection == -1)) || ((this.velocity.X > 0f) && (this.spriteDirection == 1)))
                    {
                        if (Main.tile[i, j - 2].active && Main.tileSolid[Main.tile[i, j - 2].type])
                        {
                            if (Main.tile[i, j - 3].active && Main.tileSolid[Main.tile[i, j - 3].type])
                            {
                                this.velocity.Y = -8f;
                                this.netUpdate = true;
                            }
                            else
                            {
                                this.velocity.Y = -7f;
                                this.netUpdate = true;
                            }
                        }
                        else if (Main.tile[i, j - 1].active && Main.tileSolid[Main.tile[i, j - 1].type])
                        {
                            this.velocity.Y = -6f;
                            this.netUpdate = true;
                        }
                        else if (Main.tile[i, j].active && Main.tileSolid[Main.tile[i, j].type])
                        {
                            this.velocity.Y = -5f;
                            this.netUpdate = true;
                        }
                        else if (((this.directionY < 0) && (!Main.tile[i, j + 1].active || !Main.tileSolid[Main.tile[i, j + 1].type])) && (!Main.tile[i + this.direction, j + 1].active || !Main.tileSolid[Main.tile[i + this.direction, j + 1].type]))
                        {
                            this.velocity.Y = -8f;
                            this.velocity.X *= 1.5f;
                            this.netUpdate = true;
                        }
                        else
                        {
                            this.ai[1] = 0f;
                            this.ai[2] = 0f;
                        }
                    }
                    if ((((this.type == 0x1f) && (this.velocity.Y == 0f)) && ((Math.Abs((float) ((this.position.X + (this.width / 2)) - (Main.player[this.target].position.X - (Main.player[this.target].width / 2)))) < 100f) && (Math.Abs((float) ((this.position.Y + (this.height / 2)) - (Main.player[this.target].position.Y - (Main.player[this.target].height / 2)))) < 50f))) && (((this.direction > 0) && (this.velocity.X > 1f)) || ((this.direction < 0) && (this.velocity.X < -1f))))
                    {
                        this.velocity.X *= 2f;
                        if (this.velocity.X > 3f)
                        {
                            this.velocity.X = 3f;
                        }
                        if (this.velocity.X < -3f)
                        {
                            this.velocity.X = -3f;
                        }
                        this.velocity.Y = -4f;
                        this.netUpdate = true;
                        return;
                    }
                }
                else
                {
                    this.ai[2]++;
                    this.ai[3] = 0f;
                    if (this.ai[2] >= 60f)
                    {
                        if (!Main.bloodMoon && (this.type == 3))
                        {
                            this.ai[1] = 0f;
                        }
                        this.velocity.X = 0.5f * -this.direction;
                        this.ai[1]++;
                        if (this.type == 0x1b)
                        {
                            this.ai[1]++;
                        }
                        if (this.type == 0x1f)
                        {
                            this.ai[1] += 6f;
                        }
                        this.ai[2] = 0f;
                        bool flag2 = false;
                        if (this.ai[1] >= 10f)
                        {
                            flag2 = true;
                            this.ai[1] = 10f;
                        }
                        WorldGen.KillTile(i, j - 1, true, false, false);
                        if (((Main.netMode != 1) || !flag2) && (flag2 && (Main.netMode != 1)))
                        {
                            if (this.type != 0x1a)
                            {
                                bool flag3 = WorldGen.OpenDoor(i, j, this.direction);
                                if (!flag3)
                                {
                                    this.ai[3] = num2;
                                    this.netUpdate = true;
                                }
                                if ((Main.netMode == 2) && flag3)
                                {
                                    NetMessage.SendData(0x13, -1, -1, "", 0, (float) i, (float) j, (float) this.direction);
                                    return;
                                }
                            }
                            else
                            {
                                WorldGen.KillTile(i, j - 1, false, false, false);
                                if (Main.netMode == 2)
                                {
                                    NetMessage.SendData(0x11, -1, -1, "", 0, (float) i, (float) (j - 1), 0f);
                                    return;
                                }
                            }
                        }
                    }
                }
                return;
            }
            if (this.aiStyle == 4)
            {
                if (((this.target < 0) || (this.target == 8)) || (Main.player[this.target].dead || !Main.player[this.target].active))
                {
                    this.TargetClosest();
                }
                bool dead = Main.player[this.target].dead;
                float num5 = ((this.position.X + (this.width / 2)) - Main.player[this.target].position.X) - (Main.player[this.target].width / 2);
                float num6 = (((this.position.Y + this.height) - 59f) - Main.player[this.target].position.Y) - (Main.player[this.target].height / 2);
                float num7 = ((float) Math.Atan2((double) num6, (double) num5)) + 1.57f;
                if (num7 < 0f)
                {
                    num7 += 6.283f;
                }
                else if (num7 > 6.283)
                {
                    num7 -= 6.283f;
                }
                float num8 = 0f;
                if ((this.ai[0] == 0f) && (this.ai[1] == 0f))
                {
                    num8 = 0.02f;
                }
                if (((this.ai[0] == 0f) && (this.ai[1] == 2f)) && (this.ai[2] > 40f))
                {
                    num8 = 0.05f;
                }
                if ((this.ai[0] == 3f) && (this.ai[1] == 0f))
                {
                    num8 = 0.05f;
                }
                if (((this.ai[0] == 3f) && (this.ai[1] == 2f)) && (this.ai[2] > 40f))
                {
                    num8 = 0.08f;
                }
                if (this.rotation < num7)
                {
                    if ((num7 - this.rotation) > 3.1415)
                    {
                        this.rotation -= num8;
                    }
                    else
                    {
                        this.rotation += num8;
                    }
                }
                else if (this.rotation > num7)
                {
                    if ((this.rotation - num7) > 3.1415)
                    {
                        this.rotation += num8;
                    }
                    else
                    {
                        this.rotation -= num8;
                    }
                }
                if ((this.rotation > (num7 - num8)) && (this.rotation < (num7 + num8)))
                {
                    this.rotation = num7;
                }
                if (this.rotation < 0f)
                {
                    this.rotation += 6.283f;
                }
                else if (this.rotation > 6.283)
                {
                    this.rotation -= 6.283f;
                }
                if ((this.rotation > (num7 - num8)) && (this.rotation < (num7 + num8)))
                {
                    this.rotation = num7;
                }
                if (Main.rand.Next(5) == 0)
                {
                    color = new Color();
                    int num9 = Dust.NewDust(new Vector2(this.position.X, this.position.Y + (this.height * 0.25f)), this.width, (int) (this.height * 0.5f), 5, this.velocity.X, 2f, 0, color, 1f);
                    Main.dust[num9].velocity.X *= 0.5f;
                    Main.dust[num9].velocity.Y *= 0.1f;
                }
                if (!Main.dayTime && !dead)
                {
                    if (this.ai[0] != 0f)
                    {
                        if ((this.ai[0] != 1f) && (this.ai[0] != 2f))
                        {
                            this.damage = 30;
                            this.defense = 6;
                            if (this.ai[1] != 0f)
                            {
                                if (this.ai[1] == 1f)
                                {
                                    Main.PlaySound(15, (int) this.position.X, (int) this.position.Y, 0);
                                    this.rotation = num7;
                                    float num33 = 8f;
                                    Vector2 vector6 = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                                    float num34 = (Main.player[this.target].position.X + (Main.player[this.target].width / 2)) - vector6.X;
                                    float num35 = (Main.player[this.target].position.Y + (Main.player[this.target].height / 2)) - vector6.Y;
                                    float num36 = (float) Math.Sqrt((double) ((num34 * num34) + (num35 * num35)));
                                    num36 = num33 / num36;
                                    this.velocity.X = num34 * num36;
                                    this.velocity.Y = num35 * num36;
                                    this.ai[1] = 2f;
                                    return;
                                }
                                if (this.ai[1] == 2f)
                                {
                                    this.ai[2]++;
                                    if (this.ai[2] >= 40f)
                                    {
                                        this.velocity.X *= 0.97f;
                                        this.velocity.Y *= 0.97f;
                                        if ((this.velocity.X > -0.1) && (this.velocity.X < 0.1))
                                        {
                                            this.velocity.X = 0f;
                                        }
                                        if ((this.velocity.Y > -0.1) && (this.velocity.Y < 0.1))
                                        {
                                            this.velocity.Y = 0f;
                                        }
                                    }
                                    else
                                    {
                                        this.rotation = ((float) Math.Atan2((double) this.velocity.Y, (double) this.velocity.X)) - 1.57f;
                                    }
                                    if (this.ai[2] >= 100f)
                                    {
                                        this.ai[3]++;
                                        this.ai[2] = 0f;
                                        this.target = 8;
                                        this.rotation = num7;
                                        if (this.ai[3] >= 3f)
                                        {
                                            this.ai[1] = 0f;
                                            this.ai[3] = 0f;
                                            return;
                                        }
                                        this.ai[1] = 1f;
                                        return;
                                    }
                                }
                            }
                            else
                            {
                                float num28 = 6f;
                                float num29 = 0.07f;
                                Vector2 vector5 = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                                float num30 = (Main.player[this.target].position.X + (Main.player[this.target].width / 2)) - vector5.X;
                                float num31 = ((Main.player[this.target].position.Y + (Main.player[this.target].height / 2)) - 120f) - vector5.Y;
                                float num32 = (float) Math.Sqrt((double) ((num30 * num30) + (num31 * num31)));
                                num32 = num28 / num32;
                                num30 *= num32;
                                num31 *= num32;
                                if (this.velocity.X < num30)
                                {
                                    this.velocity.X += num29;
                                    if ((this.velocity.X < 0f) && (num30 > 0f))
                                    {
                                        this.velocity.X += num29;
                                    }
                                }
                                else if (this.velocity.X > num30)
                                {
                                    this.velocity.X -= num29;
                                    if ((this.velocity.X > 0f) && (num30 < 0f))
                                    {
                                        this.velocity.X -= num29;
                                    }
                                }
                                if (this.velocity.Y < num31)
                                {
                                    this.velocity.Y += num29;
                                    if ((this.velocity.Y < 0f) && (num31 > 0f))
                                    {
                                        this.velocity.Y += num29;
                                    }
                                }
                                else if (this.velocity.Y > num31)
                                {
                                    this.velocity.Y -= num29;
                                    if ((this.velocity.Y > 0f) && (num31 < 0f))
                                    {
                                        this.velocity.Y -= num29;
                                    }
                                }
                                this.ai[2]++;
                                if (this.ai[2] >= 200f)
                                {
                                    this.ai[1] = 1f;
                                    this.ai[2] = 0f;
                                    this.ai[3] = 0f;
                                    this.target = 8;
                                    this.netUpdate = true;
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (this.ai[0] == 1f)
                            {
                                this.ai[2] += 0.005f;
                                if (this.ai[2] > 0.5)
                                {
                                    this.ai[2] = 0.5f;
                                }
                            }
                            else
                            {
                                this.ai[2] -= 0.005f;
                                if (this.ai[2] < 0f)
                                {
                                    this.ai[2] = 0f;
                                }
                            }
                            this.rotation += this.ai[2];
                            this.ai[1]++;
                            if (this.ai[1] == 100f)
                            {
                                this.ai[0]++;
                                this.ai[1] = 0f;
                                if (this.ai[0] == 3f)
                                {
                                    this.ai[2] = 0f;
                                }
                                else
                                {
                                    Main.PlaySound(3, (int) this.position.X, (int) this.position.Y, 1);
                                    for (int k = 0; k < 2; k++)
                                    {
                                        Gore.NewGore(this.position, new Vector2(Main.rand.Next(-30, 0x1f) * 0.2f, Main.rand.Next(-30, 0x1f) * 0.2f), 8);
                                        Gore.NewGore(this.position, new Vector2(Main.rand.Next(-30, 0x1f) * 0.2f, Main.rand.Next(-30, 0x1f) * 0.2f), 7);
                                        Gore.NewGore(this.position, new Vector2(Main.rand.Next(-30, 0x1f) * 0.2f, Main.rand.Next(-30, 0x1f) * 0.2f), 6);
                                    }
                                    for (int m = 0; m < 20; m++)
                                    {
                                        color = new Color();
                                        Dust.NewDust(this.position, this.width, this.height, 5, Main.rand.Next(-30, 0x1f) * 0.2f, Main.rand.Next(-30, 0x1f) * 0.2f, 0, color, 1f);
                                    }
                                    Main.PlaySound(15, (int) this.position.X, (int) this.position.Y, 0);
                                }
                            }
                            Dust.NewDust(this.position, this.width, this.height, 5, Main.rand.Next(-30, 0x1f) * 0.2f, Main.rand.Next(-30, 0x1f) * 0.2f, 0, new Color(), 1f);
                            this.velocity.X *= 0.98f;
                            this.velocity.Y *= 0.98f;
                            if ((this.velocity.X > -0.1) && (this.velocity.X < 0.1))
                            {
                                this.velocity.X = 0f;
                            }
                            if ((this.velocity.Y > -0.1) && (this.velocity.Y < 0.1))
                            {
                                this.velocity.Y = 0f;
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (this.ai[1] == 0f)
                        {
                            float num10 = 5f;
                            float num11 = 0.04f;
                            Vector2 vector = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                            float num12 = (Main.player[this.target].position.X + (Main.player[this.target].width / 2)) - vector.X;
                            float num13 = ((Main.player[this.target].position.Y + (Main.player[this.target].height / 2)) - 200f) - vector.Y;
                            float num14 = (float) Math.Sqrt((double) ((num12 * num12) + (num13 * num13)));
                            float num15 = num14;
                            num14 = num10 / num14;
                            num12 *= num14;
                            num13 *= num14;
                            if (this.velocity.X < num12)
                            {
                                this.velocity.X += num11;
                                if ((this.velocity.X < 0f) && (num12 > 0f))
                                {
                                    this.velocity.X += num11;
                                }
                            }
                            else if (this.velocity.X > num12)
                            {
                                this.velocity.X -= num11;
                                if ((this.velocity.X > 0f) && (num12 < 0f))
                                {
                                    this.velocity.X -= num11;
                                }
                            }
                            if (this.velocity.Y < num13)
                            {
                                this.velocity.Y += num11;
                                if ((this.velocity.Y < 0f) && (num13 > 0f))
                                {
                                    this.velocity.Y += num11;
                                }
                            }
                            else if (this.velocity.Y > num13)
                            {
                                this.velocity.Y -= num11;
                                if ((this.velocity.Y > 0f) && (num13 < 0f))
                                {
                                    this.velocity.Y -= num11;
                                }
                            }
                            this.ai[2]++;
                            if (this.ai[2] >= 600f)
                            {
                                this.ai[1] = 1f;
                                this.ai[2] = 0f;
                                this.ai[3] = 0f;
                                this.target = 8;
                                this.netUpdate = true;
                            }
                            else if (((this.position.Y + this.height) < Main.player[this.target].position.Y) && (num15 < 500f))
                            {
                                if (!Main.player[this.target].dead)
                                {
                                    this.ai[3]++;
                                }
                                if (this.ai[3] >= 90f)
                                {
                                    Vector2 vector3;
                                    this.ai[3] = 0f;
                                    this.rotation = num7;
                                    float num16 = 5f;
                                    float num17 = (Main.player[this.target].position.X + (Main.player[this.target].width / 2)) - vector.X;
                                    float num18 = (Main.player[this.target].position.Y + (Main.player[this.target].height / 2)) - vector.Y;
                                    float num19 = (float) Math.Sqrt((double) ((num17 * num17) + (num18 * num18)));
                                    num19 = num16 / num19;
                                    Vector2 position = vector;
                                    vector3.X = num17 * num19;
                                    vector3.Y = num18 * num19;
                                    position.X += vector3.X * 10f;
                                    position.Y += vector3.Y * 10f;
                                    if (Main.netMode != 1)
                                    {
                                        int num20 = NewNPC((int) position.X, (int) position.Y, 5, 0);
                                        Main.npc[num20].velocity.X = vector3.X;
                                        Main.npc[num20].velocity.Y = vector3.Y;
                                        if ((Main.netMode == 2) && (num20 < 0x3e8))
                                        {
                                            NetMessage.SendData(0x17, -1, -1, "", num20, 0f, 0f, 0f);
                                        }
                                    }
                                    Main.PlaySound(3, (int) position.X, (int) position.Y, 1);
                                    for (int n = 0; n < 10; n++)
                                    {
                                        color = new Color();
                                        Dust.NewDust(position, 20, 20, 5, vector3.X * 0.4f, vector3.Y * 0.4f, 0, color, 1f);
                                    }
                                }
                            }
                        }
                        else if (this.ai[1] == 1f)
                        {
                            this.rotation = num7;
                            float num22 = 7f;
                            Vector2 vector4 = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                            float num23 = (Main.player[this.target].position.X + (Main.player[this.target].width / 2)) - vector4.X;
                            float num24 = (Main.player[this.target].position.Y + (Main.player[this.target].height / 2)) - vector4.Y;
                            float num25 = (float) Math.Sqrt((double) ((num23 * num23) + (num24 * num24)));
                            num25 = num22 / num25;
                            this.velocity.X = num23 * num25;
                            this.velocity.Y = num24 * num25;
                            this.ai[1] = 2f;
                        }
                        else if (this.ai[1] == 2f)
                        {
                            this.ai[2]++;
                            if (this.ai[2] >= 40f)
                            {
                                this.velocity.X *= 0.98f;
                                this.velocity.Y *= 0.98f;
                                if ((this.velocity.X > -0.1) && (this.velocity.X < 0.1))
                                {
                                    this.velocity.X = 0f;
                                }
                                if ((this.velocity.Y > -0.1) && (this.velocity.Y < 0.1))
                                {
                                    this.velocity.Y = 0f;
                                }
                            }
                            else
                            {
                                this.rotation = ((float) Math.Atan2((double) this.velocity.Y, (double) this.velocity.X)) - 1.57f;
                            }
                            if (this.ai[2] >= 120f)
                            {
                                this.ai[3]++;
                                this.ai[2] = 0f;
                                this.target = 8;
                                this.rotation = num7;
                                if (this.ai[3] >= 3f)
                                {
                                    this.ai[1] = 0f;
                                    this.ai[3] = 0f;
                                }
                                else
                                {
                                    this.ai[1] = 1f;
                                }
                            }
                        }
                        if (this.life < (this.lifeMax * 0.5))
                        {
                            this.ai[0] = 1f;
                            this.ai[1] = 0f;
                            this.ai[2] = 0f;
                            this.ai[3] = 0f;
                            this.netUpdate = true;
                            return;
                        }
                    }
                }
                else
                {
                    this.velocity.Y -= 0.04f;
                    if (this.timeLeft > 10)
                    {
                        this.timeLeft = 10;
                        return;
                    }
                }
                return;
            }
            if (this.aiStyle == 5)
            {
                if (((this.target < 0) || (this.target == 8)) || Main.player[this.target].dead)
                {
                    this.TargetClosest();
                }
                float num37 = 6f;
                float num38 = 0.05f;
                if (this.type == 6)
                {
                    num37 = 4f;
                    num38 = 0.02f;
                }
                else if (this.type == 0x17)
                {
                    num37 = 2.5f;
                    num38 = 0.02f;
                }
                Vector2 vector7 = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                float num39 = (Main.player[this.target].position.X + (Main.player[this.target].width / 2)) - vector7.X;
                float num40 = (Main.player[this.target].position.Y + (Main.player[this.target].height / 2)) - vector7.Y;
                float num41 = (float) Math.Sqrt((double) ((num39 * num39) + (num40 * num40)));
                num41 = num37 / num41;
                num39 *= num41;
                num40 *= num41;
                if (Main.player[this.target].dead)
                {
                    num39 = (this.direction * num37) / 2f;
                    num40 = -num37 / 2f;
                }
                if (this.velocity.X < num39)
                {
                    this.velocity.X += num38;
                    if ((this.velocity.X < 0f) && (num39 > 0f))
                    {
                        this.velocity.X += num38;
                    }
                }
                else if (this.velocity.X > num39)
                {
                    this.velocity.X -= num38;
                    if ((this.velocity.X > 0f) && (num39 < 0f))
                    {
                        this.velocity.X -= num38;
                    }
                }
                if (this.velocity.Y < num40)
                {
                    this.velocity.Y += num38;
                    if ((this.velocity.Y < 0f) && (num40 > 0f))
                    {
                        this.velocity.Y += num38;
                    }
                }
                else if (this.velocity.Y > num40)
                {
                    this.velocity.Y -= num38;
                    if ((this.velocity.Y > 0f) && (num40 < 0f))
                    {
                        this.velocity.Y -= num38;
                    }
                }
                if (this.type == 0x17)
                {
                    this.rotation = (float) Math.Atan2((double) this.velocity.Y, (double) this.velocity.X);
                }
                else
                {
                    this.rotation = ((float) Math.Atan2((double) this.velocity.Y, (double) this.velocity.X)) - 1.57f;
                }
                if ((this.type == 6) || (this.type == 0x17))
                {
                    if (this.collideX)
                    {
                        this.netUpdate = true;
                        this.velocity.X = this.oldVelocity.X * -0.7f;
                        if (((this.direction == -1) && (this.velocity.X > 0f)) && (this.velocity.X < 2f))
                        {
                            this.velocity.X = 2f;
                        }
                        if (((this.direction == 1) && (this.velocity.X < 0f)) && (this.velocity.X > -2f))
                        {
                            this.velocity.X = -2f;
                        }
                    }
                    if (this.collideY)
                    {
                        this.netUpdate = true;
                        this.velocity.Y = this.oldVelocity.Y * -0.7f;
                        if ((this.velocity.Y > 0f) && (this.velocity.Y < 2f))
                        {
                            this.velocity.Y = 2f;
                        }
                        if ((this.velocity.Y < 0f) && (this.velocity.Y > -2f))
                        {
                            this.velocity.Y = -2f;
                        }
                    }
                    if (this.type == 0x17)
                    {
                        int num42 = Dust.NewDust(new Vector2(this.position.X - this.velocity.X, this.position.Y - this.velocity.Y), this.width, this.height, 6, this.velocity.X * 0.2f, this.velocity.Y * 0.2f, 100, new Color(), 2f);
                        Main.dust[num42].noGravity = true;
                        Main.dust[num42].velocity.X *= 0.3f;
                        Main.dust[num42].velocity.Y *= 0.3f;
                    }
                    else if (Main.rand.Next(20) == 0)
                    {
                        int num43 = Dust.NewDust(new Vector2(this.position.X, this.position.Y + (this.height * 0.25f)), this.width, (int) (this.height * 0.5f), 0x12, this.velocity.X, 2f, this.alpha, this.color, this.scale);
                        Main.dust[num43].velocity.X *= 0.5f;
                        Main.dust[num43].velocity.Y *= 0.1f;
                    }
                }
                else if (Main.rand.Next(40) == 0)
                {
                    int num44 = Dust.NewDust(new Vector2(this.position.X, this.position.Y + (this.height * 0.25f)), this.width, (int) (this.height * 0.5f), 5, this.velocity.X, 2f, 0, new Color(), 1f);
                    Main.dust[num44].velocity.X *= 0.5f;
                    Main.dust[num44].velocity.Y *= 0.1f;
                }
                if (((Main.dayTime && (this.type != 6)) && (this.type != 0x17)) || Main.player[this.target].dead)
                {
                    this.velocity.Y -= num38 * 2f;
                    if (this.timeLeft > 10)
                    {
                        this.timeLeft = 10;
                        return;
                    }
                }
                return;
            }
            if (this.aiStyle != 6)
            {
                if (this.aiStyle != 7)
                {
                    if (this.aiStyle != 8)
                    {
                        if (this.aiStyle == 9)
                        {
                            if (this.target == 8)
                            {
                                this.TargetClosest();
                                float num95 = 6f;
                                if (this.type == 30)
                                {
                                    maxSpawns = 8;
                                }
                                Vector2 vector10 = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                                float num96 = (Main.player[this.target].position.X + (Main.player[this.target].width / 2)) - vector10.X;
                                float num97 = (Main.player[this.target].position.Y + (Main.player[this.target].height / 2)) - vector10.Y;
                                float num98 = (float) Math.Sqrt((double) ((num96 * num96) + (num97 * num97)));
                                num98 = num95 / num98;
                                this.velocity.X = num96 * num98;
                                this.velocity.Y = num97 * num98;
                            }
                            if (this.timeLeft > 100)
                            {
                                this.timeLeft = 100;
                            }
                            for (int num99 = 0; num99 < 2; num99++)
                            {
                                if (this.type == 30)
                                {
                                    color = new Color();
                                    int num100 = Dust.NewDust(new Vector2(this.position.X, this.position.Y + 2f), this.width, this.height, 0x1b, this.velocity.X * 0.2f, this.velocity.Y * 0.2f, 100, color, 2f);
                                    Main.dust[num100].noGravity = true;
                                    Dust dust7 = Main.dust[num100];
                                    dust7.velocity = (Vector2) (dust7.velocity * 0.3f);
                                    Main.dust[num100].velocity.X -= this.velocity.X * 0.2f;
                                    Main.dust[num100].velocity.Y -= this.velocity.Y * 0.2f;
                                }
                                else if (this.type == 0x21)
                                {
                                    color = new Color();
                                    int num101 = Dust.NewDust(new Vector2(this.position.X, this.position.Y + 2f), this.width, this.height, 0x1d, this.velocity.X * 0.2f, this.velocity.Y * 0.2f, 100, color, 2f);
                                    Main.dust[num101].noGravity = true;
                                    Main.dust[num101].velocity.X *= 0.3f;
                                    Main.dust[num101].velocity.Y *= 0.3f;
                                }
                                else
                                {
                                    color = new Color();
                                    int num102 = Dust.NewDust(new Vector2(this.position.X, this.position.Y + 2f), this.width, this.height, 6, this.velocity.X * 0.2f, this.velocity.Y * 0.2f, 100, color, 2f);
                                    Main.dust[num102].noGravity = true;
                                    Main.dust[num102].velocity.X *= 0.3f;
                                    Main.dust[num102].velocity.Y *= 0.3f;
                                }
                            }
                            this.rotation += 0.4f * this.direction;
                            return;
                        }
                        if (this.aiStyle == 10)
                        {
                            if (this.collideX)
                            {
                                this.velocity.X = this.oldVelocity.X * -0.5f;
                                if (((this.direction == -1) && (this.velocity.X > 0f)) && (this.velocity.X < 2f))
                                {
                                    this.velocity.X = 2f;
                                }
                                if (((this.direction == 1) && (this.velocity.X < 0f)) && (this.velocity.X > -2f))
                                {
                                    this.velocity.X = -2f;
                                }
                            }
                            if (this.collideY)
                            {
                                this.velocity.Y = this.oldVelocity.Y * -0.5f;
                                if ((this.velocity.Y > 0f) && (this.velocity.Y < 1f))
                                {
                                    this.velocity.Y = 1f;
                                }
                                if ((this.velocity.Y < 0f) && (this.velocity.Y > -1f))
                                {
                                    this.velocity.Y = -1f;
                                }
                            }
                            this.TargetClosest();
                            if ((this.direction == -1) && (this.velocity.X > -4f))
                            {
                                this.velocity.X -= 0.1f;
                                if (this.velocity.X > 4f)
                                {
                                    this.velocity.X -= 0.1f;
                                }
                                else if (this.velocity.X > 0f)
                                {
                                    this.velocity.X += 0.05f;
                                }
                                if (this.velocity.X < -4f)
                                {
                                    this.velocity.X = -4f;
                                }
                            }
                            else if ((this.direction == 1) && (this.velocity.X < 4f))
                            {
                                this.velocity.X += 0.1f;
                                if (this.velocity.X < -4f)
                                {
                                    this.velocity.X += 0.1f;
                                }
                                else if (this.velocity.X < 0f)
                                {
                                    this.velocity.X -= 0.05f;
                                }
                                if (this.velocity.X > 4f)
                                {
                                    this.velocity.X = 4f;
                                }
                            }
                            if ((this.directionY == -1) && (this.velocity.Y > -1.5))
                            {
                                this.velocity.Y -= 0.04f;
                                if (this.velocity.Y > 1.5)
                                {
                                    this.velocity.Y -= 0.05f;
                                }
                                else if (this.velocity.Y > 0f)
                                {
                                    this.velocity.Y += 0.03f;
                                }
                                if (this.velocity.Y < -1.5)
                                {
                                    this.velocity.Y = -1.5f;
                                }
                            }
                            else if ((this.directionY == 1) && (this.velocity.Y < 1.5))
                            {
                                this.velocity.Y += 0.04f;
                                if (this.velocity.Y < -1.5)
                                {
                                    this.velocity.Y += 0.05f;
                                }
                                else if (this.velocity.Y < 0f)
                                {
                                    this.velocity.Y -= 0.03f;
                                }
                                if (this.velocity.Y > 1.5)
                                {
                                    this.velocity.Y = 1.5f;
                                }
                            }
                            this.rotation = ((float) Math.Atan2((double) this.velocity.Y, (double) this.velocity.X)) - 1.57f;
                            int num103 = Dust.NewDust(new Vector2(this.position.X - this.velocity.X, this.position.Y - this.velocity.Y), this.width, this.height, 6, this.velocity.X * 0.2f, this.velocity.Y * 0.2f, 100, new Color(), 2f);
                            Main.dust[num103].noGravity = true;
                            Main.dust[num103].noLight = true;
                            Main.dust[num103].velocity.X *= 0.3f;
                            Main.dust[num103].velocity.Y *= 0.3f;
                            return;
                        }
                        if (this.aiStyle == 11)
                        {
                            if ((this.ai[0] == 0f) && (Main.netMode != 1))
                            {
                                this.TargetClosest();
                                this.ai[0] = 1f;
                                int num104 = NewNPC(((int) this.position.X) + (this.width / 2), ((int) this.position.Y) + (this.height / 2), 0x24, this.whoAmI);
                                Main.npc[num104].ai[0] = -1f;
                                Main.npc[num104].ai[1] = this.whoAmI;
                                Main.npc[num104].target = this.target;
                                Main.npc[num104].netUpdate = true;
                                num104 = NewNPC(((int) this.position.X) + (this.width / 2), ((int) this.position.Y) + (this.height / 2), 0x24, this.whoAmI);
                                Main.npc[num104].ai[0] = 1f;
                                Main.npc[num104].ai[1] = this.whoAmI;
                                Main.npc[num104].ai[3] = 150f;
                                Main.npc[num104].target = this.target;
                                Main.npc[num104].netUpdate = true;
                            }
                            if ((Main.player[this.target].dead || (Math.Abs((float) (this.position.X - Main.player[this.target].position.X)) > 2000f)) || (Math.Abs((float) (this.position.Y - Main.player[this.target].position.Y)) > 2000f))
                            {
                                this.TargetClosest();
                                if ((Main.player[this.target].dead || (Math.Abs((float) (this.position.X - Main.player[this.target].position.X)) > 2000f)) || (Math.Abs((float) (this.position.Y - Main.player[this.target].position.Y)) > 2000f))
                                {
                                    this.ai[1] = 3f;
                                }
                            }
                            if ((Main.dayTime && (this.ai[1] != 3f)) && (this.ai[1] != 2f))
                            {
                                this.ai[1] = 2f;
                                Main.PlaySound(15, (int) this.position.X, (int) this.position.Y, 0);
                            }
                            if (this.ai[1] == 0f)
                            {
                                this.ai[2]++;
                                if (this.ai[2] >= 800f)
                                {
                                    this.ai[2] = 0f;
                                    this.ai[1] = 1f;
                                    this.TargetClosest();
                                    this.netUpdate = true;
                                }
                                this.rotation = this.velocity.X / 15f;
                                if (this.position.Y > (Main.player[this.target].position.Y - 250f))
                                {
                                    if (this.velocity.Y > 0f)
                                    {
                                        this.velocity.Y *= 0.98f;
                                    }
                                    this.velocity.Y -= 0.02f;
                                    if (this.velocity.Y > 2f)
                                    {
                                        this.velocity.Y = 2f;
                                    }
                                }
                                else if (this.position.Y < (Main.player[this.target].position.Y - 250f))
                                {
                                    if (this.velocity.Y < 0f)
                                    {
                                        this.velocity.Y *= 0.98f;
                                    }
                                    this.velocity.Y += 0.02f;
                                    if (this.velocity.Y < -2f)
                                    {
                                        this.velocity.Y = -2f;
                                    }
                                }
                                if ((this.position.X + (this.width / 2)) > (Main.player[this.target].position.X + (Main.player[this.target].width / 2)))
                                {
                                    if (this.velocity.X > 0f)
                                    {
                                        this.velocity.X *= 0.98f;
                                    }
                                    this.velocity.X -= 0.05f;
                                    if (this.velocity.X > 8f)
                                    {
                                        this.velocity.X = 8f;
                                    }
                                }
                                if ((this.position.X + (this.width / 2)) < (Main.player[this.target].position.X + (Main.player[this.target].width / 2)))
                                {
                                    if (this.velocity.X < 0f)
                                    {
                                        this.velocity.X *= 0.98f;
                                    }
                                    this.velocity.X += 0.05f;
                                    if (this.velocity.X < -8f)
                                    {
                                        this.velocity.X = -8f;
                                    }
                                }
                            }
                            else if (this.ai[1] == 1f)
                            {
                                this.ai[2]++;
                                if (this.ai[2] == 2f)
                                {
                                    Main.PlaySound(15, (int) this.position.X, (int) this.position.Y, 0);
                                }
                                if (this.ai[2] >= 400f)
                                {
                                    this.ai[2] = 0f;
                                    this.ai[1] = 0f;
                                }
                                this.rotation += this.direction * 0.3f;
                                Vector2 vector11 = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                                float num105 = (Main.player[this.target].position.X + (Main.player[this.target].width / 2)) - vector11.X;
                                float num106 = (Main.player[this.target].position.Y + (Main.player[this.target].height / 2)) - vector11.Y;
                                float num107 = (float) Math.Sqrt((double) ((num105 * num105) + (num106 * num106)));
                                num107 = 2.5f / num107;
                                this.velocity.X = num105 * num107;
                                this.velocity.Y = num106 * num107;
                            }
                            else if (this.ai[1] == 2f)
                            {
                                this.damage = 0x270f;
                                this.defense = 0x270f;
                                this.rotation += this.direction * 0.3f;
                                Vector2 vector12 = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                                float num108 = (Main.player[this.target].position.X + (Main.player[this.target].width / 2)) - vector12.X;
                                float num109 = (Main.player[this.target].position.Y + (Main.player[this.target].height / 2)) - vector12.Y;
                                float num110 = (float) Math.Sqrt((double) ((num108 * num108) + (num109 * num109)));
                                num110 = 8f / num110;
                                this.velocity.X = num108 * num110;
                                this.velocity.Y = num109 * num110;
                            }
                            else if (this.ai[1] == 3f)
                            {
                                this.velocity.Y -= 0.1f;
                                if (this.velocity.Y > 0f)
                                {
                                    this.velocity.Y *= 0.95f;
                                }
                                this.velocity.X *= 0.95f;
                                if (this.timeLeft > 50)
                                {
                                    this.timeLeft = 50;
                                }
                            }
                            if ((this.ai[1] != 2f) && (this.ai[1] != 3f))
                            {
                                color = new Color();
                                int num111 = Dust.NewDust(new Vector2(((this.position.X + (this.width / 2)) - 15f) - (this.velocity.X * 5f), (this.position.Y + this.height) - 2f), 30, 10, 5, -this.velocity.X * 0.2f, 3f, 0, color, 2f);
                                Main.dust[num111].noGravity = true;
                                Main.dust[num111].velocity.X *= 1.3f;
                                Main.dust[num111].velocity.X += this.velocity.X * 0.4f;
                                Main.dust[num111].velocity.Y += 2f + this.velocity.Y;
                                for (int num112 = 0; num112 < 2; num112++)
                                {
                                    color = new Color();
                                    num111 = Dust.NewDust(new Vector2(this.position.X, this.position.Y + 120f), this.width, 60, 5, this.velocity.X, this.velocity.Y, 0, color, 2f);
                                    Main.dust[num111].noGravity = true;
                                    Dust dust8 = Main.dust[num111];
                                    dust8.velocity -= this.velocity;
                                    Main.dust[num111].velocity.Y += 5f;
                                }
                                return;
                            }
                        }
                        else if (this.aiStyle == 12)
                        {
                            this.spriteDirection = -((int) this.ai[0]);
                            if (!Main.npc[(int) this.ai[1]].active || (Main.npc[(int) this.ai[1]].aiStyle != 11))
                            {
                                this.ai[2] += 10f;
                                if ((this.ai[2] > 50f) || (Main.netMode != 2))
                                {
                                    this.life = -1;
                                    this.HitEffect(0, 10.0);
                                    this.active = false;
                                }
                            }
                            if ((this.ai[2] == 0f) || (this.ai[2] == 3f))
                            {
                                if ((Main.npc[(int) this.ai[1]].ai[1] == 3f) && (this.timeLeft > 10))
                                {
                                    this.timeLeft = 10;
                                }
                                if (Main.npc[(int) this.ai[1]].ai[1] != 0f)
                                {
                                    if (this.position.Y > (Main.npc[(int) this.ai[1]].position.Y - 100f))
                                    {
                                        if (this.velocity.Y > 0f)
                                        {
                                            this.velocity.Y *= 0.96f;
                                        }
                                        this.velocity.Y -= 0.07f;
                                        if (this.velocity.Y > 6f)
                                        {
                                            this.velocity.Y = 6f;
                                        }
                                    }
                                    else if (this.position.Y < (Main.npc[(int) this.ai[1]].position.Y - 100f))
                                    {
                                        if (this.velocity.Y < 0f)
                                        {
                                            this.velocity.Y *= 0.96f;
                                        }
                                        this.velocity.Y += 0.07f;
                                        if (this.velocity.Y < -6f)
                                        {
                                            this.velocity.Y = -6f;
                                        }
                                    }
                                    if ((this.position.X + (this.width / 2)) > ((Main.npc[(int) this.ai[1]].position.X + (Main.npc[(int) this.ai[1]].width / 2)) - (120f * this.ai[0])))
                                    {
                                        if (this.velocity.X > 0f)
                                        {
                                            this.velocity.X *= 0.96f;
                                        }
                                        this.velocity.X -= 0.1f;
                                        if (this.velocity.X > 8f)
                                        {
                                            this.velocity.X = 8f;
                                        }
                                    }
                                    if ((this.position.X + (this.width / 2)) < ((Main.npc[(int) this.ai[1]].position.X + (Main.npc[(int) this.ai[1]].width / 2)) - (120f * this.ai[0])))
                                    {
                                        if (this.velocity.X < 0f)
                                        {
                                            this.velocity.X *= 0.96f;
                                        }
                                        this.velocity.X += 0.1f;
                                        if (this.velocity.X < -8f)
                                        {
                                            this.velocity.X = -8f;
                                        }
                                    }
                                }
                                else
                                {
                                    this.ai[3]++;
                                    if (this.ai[3] >= 300f)
                                    {
                                        this.ai[2]++;
                                        this.ai[3] = 0f;
                                        this.netUpdate = true;
                                    }
                                    if (this.position.Y > (Main.npc[(int) this.ai[1]].position.Y + 230f))
                                    {
                                        if (this.velocity.Y > 0f)
                                        {
                                            this.velocity.Y *= 0.96f;
                                        }
                                        this.velocity.Y -= 0.04f;
                                        if (this.velocity.Y > 3f)
                                        {
                                            this.velocity.Y = 3f;
                                        }
                                    }
                                    else if (this.position.Y < (Main.npc[(int) this.ai[1]].position.Y + 230f))
                                    {
                                        if (this.velocity.Y < 0f)
                                        {
                                            this.velocity.Y *= 0.96f;
                                        }
                                        this.velocity.Y += 0.04f;
                                        if (this.velocity.Y < -3f)
                                        {
                                            this.velocity.Y = -3f;
                                        }
                                    }
                                    if ((this.position.X + (this.width / 2)) > ((Main.npc[(int) this.ai[1]].position.X + (Main.npc[(int) this.ai[1]].width / 2)) - (200f * this.ai[0])))
                                    {
                                        if (this.velocity.X > 0f)
                                        {
                                            this.velocity.X *= 0.96f;
                                        }
                                        this.velocity.X -= 0.07f;
                                        if (this.velocity.X > 8f)
                                        {
                                            this.velocity.X = 8f;
                                        }
                                    }
                                    if ((this.position.X + (this.width / 2)) < ((Main.npc[(int) this.ai[1]].position.X + (Main.npc[(int) this.ai[1]].width / 2)) - (200f * this.ai[0])))
                                    {
                                        if (this.velocity.X < 0f)
                                        {
                                            this.velocity.X *= 0.96f;
                                        }
                                        this.velocity.X += 0.07f;
                                        if (this.velocity.X < -8f)
                                        {
                                            this.velocity.X = -8f;
                                        }
                                    }
                                }
                                Vector2 vector13 = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                                float num113 = ((Main.npc[(int) this.ai[1]].position.X + (Main.npc[(int) this.ai[1]].width / 2)) - (200f * this.ai[0])) - vector13.X;
                                float num114 = (Main.npc[(int) this.ai[1]].position.Y + 230f) - vector13.Y;
                                Math.Sqrt((double) ((num113 * num113) + (num114 * num114)));
                                this.rotation = ((float) Math.Atan2((double) num114, (double) num113)) + 1.57f;
                                return;
                            }
                            if (this.ai[2] != 1f)
                            {
                                if (this.ai[2] != 2f)
                                {
                                    if (this.ai[2] != 4f)
                                    {
                                        if ((this.ai[2] == 5f) && (((this.velocity.X > 0f) && ((this.position.X + (this.width / 2)) > (Main.player[this.target].position.X + (Main.player[this.target].width / 2)))) || ((this.velocity.X < 0f) && ((this.position.X + (this.width / 2)) < (Main.player[this.target].position.X + (Main.player[this.target].width / 2))))))
                                        {
                                            this.ai[2] = 0f;
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        Vector2 vector15 = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                                        float num118 = ((Main.npc[(int) this.ai[1]].position.X + (Main.npc[(int) this.ai[1]].width / 2)) - (200f * this.ai[0])) - vector15.X;
                                        float num119 = (Main.npc[(int) this.ai[1]].position.Y + 230f) - vector15.Y;
                                        float num120 = (float) Math.Sqrt((double) ((num118 * num118) + (num119 * num119)));
                                        this.rotation = ((float) Math.Atan2((double) num119, (double) num118)) + 1.57f;
                                        this.velocity.Y *= 0.95f;
                                        this.velocity.X += 0.1f * -this.ai[0];
                                        if (this.velocity.X < -8f)
                                        {
                                            this.velocity.X = -8f;
                                        }
                                        if (this.velocity.X > 8f)
                                        {
                                            this.velocity.X = 8f;
                                        }
                                        if (((this.position.X + (this.width / 2)) < ((Main.npc[(int) this.ai[1]].position.X + (Main.npc[(int) this.ai[1]].width / 2)) - 500f)) || ((this.position.X + (this.width / 2)) > ((Main.npc[(int) this.ai[1]].position.X + (Main.npc[(int) this.ai[1]].width / 2)) + 500f)))
                                        {
                                            this.TargetClosest();
                                            this.ai[2] = 5f;
                                            vector15 = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                                            num118 = (Main.player[this.target].position.X + (Main.player[this.target].width / 2)) - vector15.X;
                                            num119 = (Main.player[this.target].position.Y + (Main.player[this.target].height / 2)) - vector15.Y;
                                            num120 = (float) Math.Sqrt((double) ((num118 * num118) + (num119 * num119)));
                                            num120 = 20f / num120;
                                            this.velocity.X = num118 * num120;
                                            this.velocity.Y = num119 * num120;
                                            this.netUpdate = true;
                                            return;
                                        }
                                    }
                                }
                                else if ((this.position.Y > Main.player[this.target].position.Y) || (this.velocity.Y < 0f))
                                {
                                    this.ai[2] = 3f;
                                    return;
                                }
                            }
                            else
                            {
                                Vector2 vector14 = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                                float num115 = ((Main.npc[(int) this.ai[1]].position.X + (Main.npc[(int) this.ai[1]].width / 2)) - (200f * this.ai[0])) - vector14.X;
                                float num116 = (Main.npc[(int) this.ai[1]].position.Y + 230f) - vector14.Y;
                                float num117 = (float) Math.Sqrt((double) ((num115 * num115) + (num116 * num116)));
                                this.rotation = ((float) Math.Atan2((double) num116, (double) num115)) + 1.57f;
                                this.velocity.X *= 0.95f;
                                this.velocity.Y -= 0.1f;
                                if (this.velocity.Y < -8f)
                                {
                                    this.velocity.Y = -8f;
                                }
                                if (this.position.Y < (Main.npc[(int) this.ai[1]].position.Y - 200f))
                                {
                                    this.TargetClosest();
                                    this.ai[2] = 2f;
                                    vector14 = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                                    num115 = (Main.player[this.target].position.X + (Main.player[this.target].width / 2)) - vector14.X;
                                    num116 = (Main.player[this.target].position.Y + (Main.player[this.target].height / 2)) - vector14.Y;
                                    num117 = (float) Math.Sqrt((double) ((num115 * num115) + (num116 * num116)));
                                    num117 = 20f / num117;
                                    this.velocity.X = num115 * num117;
                                    this.velocity.Y = num116 * num117;
                                    this.netUpdate = true;
                                    return;
                                }
                            }
                        }
                        else if (this.aiStyle == 13)
                        {
                            if (Main.tile[(int) this.ai[0], (int) this.ai[1]] == null)
                            {
                                Main.tile[(int) this.ai[0], (int) this.ai[1]] = new Tile();
                            }
                            if (!Main.tile[(int) this.ai[0], (int) this.ai[1]].active)
                            {
                                this.life = -1;
                                this.HitEffect(0, 10.0);
                                this.active = false;
                                return;
                            }
                            this.TargetClosest();
                            float num121 = 0.05f;
                            Vector2 vector16 = new Vector2((this.ai[0] * 16f) + 8f, (this.ai[1] * 16f) + 8f);
                            float num122 = ((Main.player[this.target].position.X + (Main.player[this.target].width / 2)) - (this.width / 2)) - vector16.X;
                            float num123 = ((Main.player[this.target].position.Y + (Main.player[this.target].height / 2)) - (this.height / 2)) - vector16.Y;
                            float num124 = (float) Math.Sqrt((double) ((num122 * num122) + (num123 * num123)));
                            if (num124 > 150f)
                            {
                                num124 = 150f / num124;
                                num122 *= num124;
                                num123 *= num124;
                            }
                            if (this.position.X < (((this.ai[0] * 16f) + 8f) + num122))
                            {
                                this.velocity.X += num121;
                                if ((this.velocity.X < 0f) && (num122 > 0f))
                                {
                                    this.velocity.X += num121 * 2f;
                                }
                            }
                            else if (this.position.X > (((this.ai[0] * 16f) + 8f) + num122))
                            {
                                this.velocity.X -= num121;
                                if ((this.velocity.X > 0f) && (num122 < 0f))
                                {
                                    this.velocity.X -= num121 * 2f;
                                }
                            }
                            if (this.position.Y < (((this.ai[1] * 16f) + 8f) + num123))
                            {
                                this.velocity.Y += num121;
                                if ((this.velocity.Y < 0f) && (num123 > 0f))
                                {
                                    this.velocity.Y += num121 * 2f;
                                }
                            }
                            else if (this.position.Y > (((this.ai[1] * 16f) + 8f) + num123))
                            {
                                this.velocity.Y -= num121;
                                if ((this.velocity.Y > 0f) && (num123 < 0f))
                                {
                                    this.velocity.Y -= num121 * 2f;
                                }
                            }
                            if (this.velocity.X > 2f)
                            {
                                this.velocity.X = 2f;
                            }
                            if (this.velocity.X < -2f)
                            {
                                this.velocity.X = -2f;
                            }
                            if (this.velocity.Y > 2f)
                            {
                                this.velocity.Y = 2f;
                            }
                            if (this.velocity.Y < -2f)
                            {
                                this.velocity.Y = -2f;
                            }
                            if (num122 > 0f)
                            {
                                this.spriteDirection = 1;
                                this.rotation = (float) Math.Atan2((double) num123, (double) num122);
                            }
                            if (num122 < 0f)
                            {
                                this.spriteDirection = -1;
                                this.rotation = ((float) Math.Atan2((double) num123, (double) num122)) + 3.14f;
                            }
                            if (this.collideX)
                            {
                                this.netUpdate = true;
                                this.velocity.X = this.oldVelocity.X * -0.7f;
                                if ((this.velocity.X > 0f) && (this.velocity.X < 2f))
                                {
                                    this.velocity.X = 2f;
                                }
                                if ((this.velocity.X < 0f) && (this.velocity.X > -2f))
                                {
                                    this.velocity.X = -2f;
                                }
                            }
                            if (this.collideY)
                            {
                                this.netUpdate = true;
                                this.velocity.Y = this.oldVelocity.Y * -0.7f;
                                if ((this.velocity.Y > 0f) && (this.velocity.Y < 2f))
                                {
                                    this.velocity.Y = 2f;
                                }
                                if ((this.velocity.Y < 0f) && (this.velocity.Y > -2f))
                                {
                                    this.velocity.Y = -2f;
                                }
                            }
                        }
                        return;
                    }
                    this.TargetClosest();
                    this.velocity.X *= 0.93f;
                    if ((this.velocity.X > -0.1) && (this.velocity.X < 0.1))
                    {
                        this.velocity.X = 0f;
                    }
                    if (this.ai[0] == 0f)
                    {
                        this.ai[0] = 500f;
                    }
                    if ((this.ai[2] != 0f) && (this.ai[3] != 0f))
                    {
                        Main.PlaySound(2, (int) this.position.X, (int) this.position.Y, 8);
                        for (int num75 = 0; num75 < 50; num75++)
                        {
                            if (this.type == 0x1d)
                            {
                                color = new Color();
                                int num76 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 0x1b, 0f, 0f, 100, color, (float) Main.rand.Next(1, 3));
                                Dust dust1 = Main.dust[num76];
                                dust1.velocity = (Vector2) (dust1.velocity * 3f);
                                if (Main.dust[num76].scale > 1f)
                                {
                                    Main.dust[num76].noGravity = true;
                                }
                            }
                            else if (this.type == 0x20)
                            {
                                color = new Color();
                                int num77 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 0x1d, 0f, 0f, 100, color, 2.5f);
                                Dust dust2 = Main.dust[num77];
                                dust2.velocity = (Vector2) (dust2.velocity * 3f);
                                Main.dust[num77].noGravity = true;
                            }
                            else
                            {
                                color = new Color();
                                int num78 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, 0f, 0f, 100, color, 2.5f);
                                Dust dust3 = Main.dust[num78];
                                dust3.velocity = (Vector2) (dust3.velocity * 3f);
                                Main.dust[num78].noGravity = true;
                            }
                        }
                        this.position.X = ((this.ai[2] * 16f) - (this.width / 2)) + 8f;
                        this.position.Y = (this.ai[3] * 16f) - this.height;
                        this.velocity.X = 0f;
                        this.velocity.Y = 0f;
                        this.ai[2] = 0f;
                        this.ai[3] = 0f;
                        Main.PlaySound(2, (int) this.position.X, (int) this.position.Y, 8);
                        for (int num79 = 0; num79 < 50; num79++)
                        {
                            if (this.type == 0x1d)
                            {
                                color = new Color();
                                int num80 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 0x1b, 0f, 0f, 100, color, (float) Main.rand.Next(1, 3));
                                Dust dust4 = Main.dust[num80];
                                dust4.velocity = (Vector2) (dust4.velocity * 3f);
                                if (Main.dust[num80].scale > 1f)
                                {
                                    Main.dust[num80].noGravity = true;
                                }
                            }
                            else if (this.type == 0x20)
                            {
                                color = new Color();
                                int num81 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 0x1d, 0f, 0f, 100, color, 2.5f);
                                Dust dust5 = Main.dust[num81];
                                dust5.velocity = (Vector2) (dust5.velocity * 3f);
                                Main.dust[num81].noGravity = true;
                            }
                            else
                            {
                                color = new Color();
                                int num82 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, 0f, 0f, 100, color, 2.5f);
                                Dust dust6 = Main.dust[num82];
                                dust6.velocity = (Vector2) (dust6.velocity * 3f);
                                Main.dust[num82].noGravity = true;
                            }
                        }
                    }
                    this.ai[0]++;
                    if (((this.ai[0] == 75f) || (this.ai[0] == 150f)) || (this.ai[0] == 225f))
                    {
                        this.ai[1] = 30f;
                        this.netUpdate = true;
                        goto Label_5D99;
                    }
                    if ((this.ai[0] < 450f) || (Main.netMode == 1))
                    {
                        goto Label_5D99;
                    }
                    this.ai[0] = 1f;
                    int num83 = ((int) Main.player[this.target].position.X) / 0x10;
                    int num84 = ((int) Main.player[this.target].position.Y) / 0x10;
                    int num85 = ((int) this.position.X) / 0x10;
                    int num86 = ((int) this.position.Y) / 0x10;
                    int num87 = 20;
                    int num88 = 0;
                    bool flag12 = false;
                    if ((Math.Abs((float) (this.position.X - Main.player[this.target].position.X)) + Math.Abs((float) (this.position.Y - Main.player[this.target].position.Y))) > 2000f)
                    {
                        num88 = 100;
                        flag12 = true;
                    }
                Label_5D85:
                    while (!flag12 && (num88 < 100))
                    {
                        num88++;
                        int num89 = Main.rand.Next(num83 - num87, num83 + num87);
                        for (int num91 = Main.rand.Next(num84 - num87, num84 + num87); num91 < (num84 + num87); num91++)
                        {
                            if (((((num91 < (num84 - 4)) || (num91 > (num84 + 4))) || ((num89 < (num83 - 4)) || (num89 > (num83 + 4)))) && (((num91 < (num86 - 1)) || (num91 > (num86 + 1))) || ((num89 < (num85 - 1)) || (num89 > (num85 + 1))))) && Main.tile[num89, num91].active)
                            {
                                bool flag13 = true;
                                if ((this.type == 0x20) && (Main.tile[num89, num91 - 1].wall == 0))
                                {
                                    flag13 = false;
                                }
                                else if (Main.tile[num89, num91 - 1].lava)
                                {
                                    flag13 = false;
                                }
                                if ((flag13 && Main.tileSolid[Main.tile[num89, num91].type]) && !Collision.SolidTiles(num89 - 1, num89 + 1, num91 - 4, num91 - 1))
                                {
                                    this.ai[1] = 20f;
                                    this.ai[2] = num89;
                                    this.ai[3] = num91;
                                    flag12 = true;
                                    goto Label_5D85;
                                }
                            }
                        }
                    }
                    this.netUpdate = true;
                    goto Label_5D99;
                }
                num66 = (((int) this.position.X) + (this.width / 2)) / 0x10;
                num67 = ((int) ((this.position.Y + this.height) + 1f)) / 0x10;
                flag7 = false;
                this.directionY = -1;
                if (this.direction == 0)
                {
                    this.direction = 1;
                }
                for (int num68 = 0; num68 < 8; num68++)
                {
                    if (Main.player[num68].active && (Main.player[num68].talkNPC == this.whoAmI))
                    {
                        flag7 = true;
                        if (this.ai[0] != 0f)
                        {
                            this.netUpdate = true;
                        }
                        this.ai[0] = 0f;
                        this.ai[1] = 300f;
                        this.ai[2] = 100f;
                        if ((Main.player[num68].position.X + (Main.player[num68].width / 2)) < (this.position.X + (this.width / 2)))
                        {
                            this.direction = -1;
                        }
                        else
                        {
                            this.direction = 1;
                        }
                    }
                }
                if (this.ai[3] > 0f)
                {
                    this.life = -1;
                    this.HitEffect(0, 10.0);
                    this.active = false;
                    if (this.type == 0x25)
                    {
                        Main.PlaySound(15, (int) this.position.X, (int) this.position.Y, 0);
                    }
                }
                if ((this.type != 0x25) || (Main.netMode == 1))
                {
                    goto Label_4417;
                }
                this.homeless = false;
                this.homeTileX = Main.dungeonX;
                this.homeTileY = Main.dungeonY;
                if (downedBoss3)
                {
                    this.ai[3] = 1f;
                    this.netUpdate = true;
                }
                if ((Main.dayTime || !flag7) || (this.ai[3] != 0f))
                {
                    goto Label_4417;
                }
                flag8 = true;
                for (int num69 = 0; num69 < 0x3e8; num69++)
                {
                    if (Main.npc[num69].active && (Main.npc[num69].type == 0x23))
                    {
                        flag8 = false;
                        break;
                    }
                }
            }
            else
            {
                if (((this.target < 0) || (this.target == 8)) || Main.player[this.target].dead)
                {
                    this.TargetClosest();
                }
                if (Main.player[this.target].dead && (this.timeLeft > 10))
                {
                    this.timeLeft = 10;
                }
                if (Main.netMode != 1)
                {
                    if (((((this.type == 7) || (this.type == 8)) || ((this.type == 10) || (this.type == 11))) || (((this.type == 13) || (this.type == 14)) || ((this.type == 0x27) || (this.type == 40)))) && (this.ai[0] == 0f))
                    {
                        if (((this.type == 7) || (this.type == 10)) || ((this.type == 13) || (this.type == 0x27)))
                        {
                            this.ai[2] = 10f;
                            if (this.type == 10)
                            {
                                this.ai[2] = 5f;
                            }
                            if (this.type == 13)
                            {
                                this.ai[2] = 50f;
                            }
                            if (this.type == 0x27)
                            {
                                this.ai[2] = 15f;
                            }
                            this.ai[0] = NewNPC((int) this.position.X, (int) this.position.Y, this.type + 1, this.whoAmI);
                        }
                        else if ((((this.type == 8) || (this.type == 11)) || ((this.type == 14) || (this.type == 40))) && (this.ai[2] > 0f))
                        {
                            this.ai[0] = NewNPC((int) this.position.X, (int) this.position.Y, this.type, this.whoAmI);
                        }
                        else
                        {
                            this.ai[0] = NewNPC((int) this.position.X, (int) this.position.Y, this.type + 1, this.whoAmI);
                        }
                        Main.npc[(int) this.ai[0]].ai[1] = this.whoAmI;
                        Main.npc[(int) this.ai[0]].ai[2] = this.ai[2] - 1f;
                        this.netUpdate = true;
                    }
                    if (((((this.type == 8) || (this.type == 9)) || ((this.type == 11) || (this.type == 12))) || ((this.type == 40) || (this.type == 0x29))) && !Main.npc[(int) this.ai[1]].active)
                    {
                        this.life = 0;
                        this.HitEffect(0, 10.0);
                        this.active = false;
                    }
                    if (((((this.type == 7) || (this.type == 8)) || ((this.type == 10) || (this.type == 11))) || ((this.type == 0x27) || (this.type == 40))) && !Main.npc[(int) this.ai[0]].active)
                    {
                        this.life = 0;
                        this.HitEffect(0, 10.0);
                        this.active = false;
                    }
                    if (((this.type == 13) || (this.type == 14)) || (this.type == 15))
                    {
                        if (!Main.npc[(int) this.ai[1]].active && !Main.npc[(int) this.ai[0]].active)
                        {
                            this.life = 0;
                            this.HitEffect(0, 10.0);
                            this.active = false;
                        }
                        if ((this.type == 13) && !Main.npc[(int) this.ai[0]].active)
                        {
                            this.life = 0;
                            this.HitEffect(0, 10.0);
                            this.active = false;
                        }
                        if ((this.type == 15) && !Main.npc[(int) this.ai[1]].active)
                        {
                            this.life = 0;
                            this.HitEffect(0, 10.0);
                            this.active = false;
                        }
                        if ((this.type == 14) && !Main.npc[(int) this.ai[1]].active)
                        {
                            this.type = 13;
                            int whoAmI = this.whoAmI;
                            int life = this.life;
                            float num47 = this.ai[0];
                            this.SetDefaults(this.type);
                            this.life = life;
                            if (this.life > this.lifeMax)
                            {
                                this.life = this.lifeMax;
                            }
                            this.ai[0] = num47;
                            this.TargetClosest();
                            this.netUpdate = true;
                            this.whoAmI = whoAmI;
                        }
                        if ((this.type == 14) && !Main.npc[(int) this.ai[0]].active)
                        {
                            int num48 = this.life;
                            int num49 = this.whoAmI;
                            float num50 = this.ai[1];
                            this.SetDefaults(this.type);
                            this.life = num48;
                            if (this.life > this.lifeMax)
                            {
                                this.life = this.lifeMax;
                            }
                            this.ai[1] = num50;
                            this.TargetClosest();
                            this.netUpdate = true;
                            this.whoAmI = num49;
                        }
                        if (this.life == 0)
                        {
                            bool flag5 = true;
                            for (int num51 = 0; num51 < 0x3e8; num51++)
                            {
                                if (Main.npc[num51].active && (((Main.npc[num51].type == 13) || (Main.npc[num51].type == 14)) || (Main.npc[num51].type == 15)))
                                {
                                    flag5 = false;
                                    break;
                                }
                            }
                            if (flag5)
                            {
                                this.boss = true;
                                this.NPCLoot();
                            }
                        }
                    }
                    if (!this.active && (Main.netMode == 2))
                    {
                        NetMessage.SendData(0x1c, -1, -1, "", this.whoAmI, -1f, 0f, 0f);
                    }
                }
                int num52 = ((int) (this.position.X / 16f)) - 1;
                int maxTilesX = ((int) ((this.position.X + this.width) / 16f)) + 2;
                int num54 = ((int) (this.position.Y / 16f)) - 1;
                int maxTilesY = ((int) ((this.position.Y + this.height) / 16f)) + 2;
                if (num52 < 0)
                {
                    num52 = 0;
                }
                if (maxTilesX > Main.maxTilesX)
                {
                    maxTilesX = Main.maxTilesX;
                }
                if (num54 < 0)
                {
                    num54 = 0;
                }
                if (maxTilesY > Main.maxTilesY)
                {
                    maxTilesY = Main.maxTilesY;
                }
                bool flag6 = false;
                for (int num56 = num52; num56 < maxTilesX; num56++)
                {
                    for (int num57 = num54; num57 < maxTilesY; num57++)
                    {
                        if ((Main.tile[num56, num57] != null) && ((Main.tile[num56, num57].active && (Main.tileSolid[Main.tile[num56, num57].type] || (Main.tileSolidTop[Main.tile[num56, num57].type] && (Main.tile[num56, num57].frameY == 0)))) || (Main.tile[num56, num57].liquid > 0x40)))
                        {
                            Vector2 vector8;
                            vector8.X = num56 * 0x10;
                            vector8.Y = num57 * 0x10;
                            if ((((this.position.X + this.width) > vector8.X) && (this.position.X < (vector8.X + 16f))) && (((this.position.Y + this.height) > vector8.Y) && (this.position.Y < (vector8.Y + 16f))))
                            {
                                flag6 = true;
                                if ((Main.rand.Next(40) == 0) && Main.tile[num56, num57].active)
                                {
                                    WorldGen.KillTile(num56, num57, true, true, false);
                                }
                                if ((Main.netMode != 1) && (Main.tile[num56, num57].type == 2))
                                {
                                    byte type = Main.tile[num56, num57 - 1].type;
                                }
                            }
                        }
                    }
                }
                float num58 = 8f;
                float num59 = 0.07f;
                if (this.type == 10)
                {
                    num58 = 6f;
                    num59 = 0.05f;
                }
                if (this.type == 13)
                {
                    num58 = 11f;
                    num59 = 0.08f;
                }
                Vector2 vector9 = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                float num60 = (Main.player[this.target].position.X + (Main.player[this.target].width / 2)) - vector9.X;
                float num61 = (Main.player[this.target].position.Y + (Main.player[this.target].height / 2)) - vector9.Y;
                float num62 = (float) Math.Sqrt((double) ((num60 * num60) + (num61 * num61)));
                if (this.ai[1] > 0f)
                {
                    num60 = (Main.npc[(int) this.ai[1]].position.X + (Main.npc[(int) this.ai[1]].width / 2)) - vector9.X;
                    num61 = (Main.npc[(int) this.ai[1]].position.Y + (Main.npc[(int) this.ai[1]].height / 2)) - vector9.Y;
                    this.rotation = ((float) Math.Atan2((double) num61, (double) num60)) + 1.57f;
                    num62 = (float) Math.Sqrt((double) ((num60 * num60) + (num61 * num61)));
                    num62 = (num62 - this.width) / num62;
                    num60 *= num62;
                    num61 *= num62;
                    this.velocity = new Vector2();
                    this.position.X += num60;
                    this.position.Y += num61;
                    return;
                }
                if (!flag6)
                {
                    this.TargetClosest();
                    this.velocity.Y += 0.11f;
                    if (this.velocity.Y > num58)
                    {
                        this.velocity.Y = num58;
                    }
                    if ((Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y)) < (num58 * 0.4))
                    {
                        if (this.velocity.X < 0f)
                        {
                            this.velocity.X -= num59 * 1.1f;
                        }
                        else
                        {
                            this.velocity.X += num59 * 1.1f;
                        }
                    }
                    else if (this.velocity.Y == num58)
                    {
                        if (this.velocity.X < num60)
                        {
                            this.velocity.X += num59;
                        }
                        else if (this.velocity.X > num60)
                        {
                            this.velocity.X -= num59;
                        }
                    }
                    else if (this.velocity.Y > 4f)
                    {
                        if (this.velocity.X < 0f)
                        {
                            this.velocity.X += num59 * 0.9f;
                        }
                        else
                        {
                            this.velocity.X -= num59 * 0.9f;
                        }
                    }
                }
                else
                {
                    if (this.soundDelay == 0)
                    {
                        float num63 = num62 / 40f;
                        if (num63 < 10f)
                        {
                            num63 = 10f;
                        }
                        if (num63 > 20f)
                        {
                            num63 = 20f;
                        }
                        this.soundDelay = (int) num63;
                        Main.PlaySound(15, (int) this.position.X, (int) this.position.Y, 1);
                    }
                    num62 = (float) Math.Sqrt((double) ((num60 * num60) + (num61 * num61)));
                    float num64 = Math.Abs(num60);
                    float num65 = Math.Abs(num61);
                    num62 = num58 / num62;
                    num60 *= num62;
                    num61 *= num62;
                    if ((((this.velocity.X > 0f) && (num60 > 0f)) || ((this.velocity.X < 0f) && (num60 < 0f))) || (((this.velocity.Y > 0f) && (num61 > 0f)) || ((this.velocity.Y < 0f) && (num61 < 0f))))
                    {
                        if (this.velocity.X < num60)
                        {
                            this.velocity.X += num59;
                        }
                        else if (this.velocity.X > num60)
                        {
                            this.velocity.X -= num59;
                        }
                        if (this.velocity.Y < num61)
                        {
                            this.velocity.Y += num59;
                        }
                        else if (this.velocity.Y > num61)
                        {
                            this.velocity.Y -= num59;
                        }
                    }
                    else if (num64 > num65)
                    {
                        if (this.velocity.X < num60)
                        {
                            this.velocity.X += num59 * 1.1f;
                        }
                        else if (this.velocity.X > num60)
                        {
                            this.velocity.X -= num59 * 1.1f;
                        }
                        if ((Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y)) < (num58 * 0.5))
                        {
                            if (this.velocity.Y > 0f)
                            {
                                this.velocity.Y += num59;
                            }
                            else
                            {
                                this.velocity.Y -= num59;
                            }
                        }
                    }
                    else
                    {
                        if (this.velocity.Y < num61)
                        {
                            this.velocity.Y += num59 * 1.1f;
                        }
                        else if (this.velocity.Y > num61)
                        {
                            this.velocity.Y -= num59 * 1.1f;
                        }
                        if ((Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y)) < (num58 * 0.5))
                        {
                            if (this.velocity.X > 0f)
                            {
                                this.velocity.X += num59;
                            }
                            else
                            {
                                this.velocity.X -= num59;
                            }
                        }
                    }
                }
                this.rotation = ((float) Math.Atan2((double) this.velocity.Y, (double) this.velocity.X)) + 1.57f;
                return;
            }
            if (flag8)
            {
                int num70 = NewNPC(((int) this.position.X) + (this.width / 2), ((int) this.position.Y) + (this.height / 2), 0x23, 0);
                Main.npc[num70].netUpdate = true;
                string str = "Skeletron";
                if (Main.netMode == 0)
                {
                    Main.NewText(str + " has awoken!", 0xaf, 0x4b, 0xff);
                }
                else if (Main.netMode == 2)
                {
                    NetMessage.SendData(0x19, -1, -1, str + " has awoken!", 8, 175f, 75f, 255f);
                }
            }
            this.ai[3] = 1f;
            this.netUpdate = true;
        Label_4417:
            if ((((Main.netMode != 1) && !Main.dayTime) && ((num66 != this.homeTileX) || (num67 != this.homeTileY))) && !this.homeless)
            {
                bool flag9 = true;
                for (int num71 = 0; num71 < 2; num71++)
                {
                    Rectangle rectangle = new Rectangle(((((int) this.position.X) + (this.width / 2)) - (Main.screenWidth / 2)) - safeRangeX, ((((int) this.position.Y) + (this.height / 2)) - (Main.screenHeight / 2)) - safeRangeY, Main.screenWidth + (safeRangeX * 2), Main.screenHeight + (safeRangeY * 2));
                    if (num71 == 1)
                    {
                        rectangle = new Rectangle((((this.homeTileX * 0x10) + 8) - (Main.screenWidth / 2)) - safeRangeX, (((this.homeTileY * 0x10) + 8) - (Main.screenHeight / 2)) - safeRangeY, Main.screenWidth + (safeRangeX * 2), Main.screenHeight + (safeRangeY * 2));
                    }
                    for (int num72 = 0; num72 < 8; num72++)
                    {
                        if (Main.player[num72].active)
                        {
                            Rectangle rectangle2 = new Rectangle((int) Main.player[num72].position.X, (int) Main.player[num72].position.Y, Main.player[num72].width, Main.player[num72].height);
                            if (rectangle2.Intersects(rectangle))
                            {
                                flag9 = false;
                                break;
                            }
                        }
                        if (!flag9)
                        {
                            break;
                        }
                    }
                }
                if (flag9)
                {
                    if ((this.type == 0x25) || !Collision.SolidTiles(this.homeTileX - 1, this.homeTileX + 1, this.homeTileY - 3, this.homeTileY - 1))
                    {
                        this.velocity.X = 0f;
                        this.velocity.Y = 0f;
                        this.position.X = ((this.homeTileX * 0x10) + 8) - (this.width / 2);
                        this.position.Y = ((this.homeTileY * 0x10) - this.height) - 0.1f;
                        this.netUpdate = true;
                    }
                    else
                    {
                        this.homeless = true;
                        WorldGen.QuickFindHome(this.whoAmI);
                    }
                }
            }
            if (this.ai[0] == 0f)
            {
                if (this.ai[2] > 0f)
                {
                    this.ai[2]--;
                }
                if (!Main.dayTime && !flag7)
                {
                    if (Main.netMode != 1)
                    {
                        if ((num66 == this.homeTileX) && (num67 == this.homeTileY))
                        {
                            if (this.velocity.X != 0f)
                            {
                                this.netUpdate = true;
                            }
                            if (this.velocity.X > 0.1)
                            {
                                this.velocity.X -= 0.1f;
                            }
                            else if (this.velocity.X < -0.1)
                            {
                                this.velocity.X += 0.1f;
                            }
                            else
                            {
                                this.velocity.X = 0f;
                            }
                        }
                        else if (!flag7)
                        {
                            if (num66 > this.homeTileX)
                            {
                                this.direction = -1;
                            }
                            else
                            {
                                this.direction = 1;
                            }
                            this.ai[0] = 1f;
                            this.ai[1] = 200 + Main.rand.Next(200);
                            this.ai[2] = 0f;
                            this.netUpdate = true;
                        }
                    }
                }
                else
                {
                    if (this.velocity.X > 0.1)
                    {
                        this.velocity.X -= 0.1f;
                    }
                    else if (this.velocity.X < -0.1)
                    {
                        this.velocity.X += 0.1f;
                    }
                    else
                    {
                        this.velocity.X = 0f;
                    }
                    if (this.ai[1] > 0f)
                    {
                        this.ai[1]--;
                    }
                    if (this.ai[1] <= 0f)
                    {
                        this.ai[0] = 1f;
                        this.ai[1] = 200 + Main.rand.Next(200);
                        this.ai[2] = 0f;
                        this.netUpdate = true;
                    }
                }
                if ((Main.netMode != 1) && (Main.dayTime || ((num66 == this.homeTileX) && (num67 == this.homeTileY))))
                {
                    if ((num66 >= (this.homeTileX - 0x19)) && (num66 <= (this.homeTileX + 0x19)))
                    {
                        if ((Main.rand.Next(80) == 0) && (this.ai[2] == 0f))
                        {
                            this.ai[2] = 200f;
                            this.direction *= -1;
                            this.netUpdate = true;
                            return;
                        }
                    }
                    else if (this.ai[2] == 0f)
                    {
                        if ((num66 < (this.homeTileX - 50)) && (this.direction == -1))
                        {
                            this.direction = 1;
                            this.netUpdate = true;
                            return;
                        }
                        if ((num66 > (this.homeTileX + 50)) && (this.direction == 1))
                        {
                            this.direction = -1;
                            this.netUpdate = true;
                            return;
                        }
                    }
                }
                return;
            }
            if (this.ai[0] == 1f)
            {
                if (((Main.netMode != 1) && !Main.dayTime) && ((num66 == this.homeTileX) && (num67 == this.homeTileY)))
                {
                    this.ai[0] = 0f;
                    this.ai[1] = 200 + Main.rand.Next(200);
                    this.ai[2] = 60f;
                    this.netUpdate = true;
                    return;
                }
                if (((Main.netMode != 1) && !this.homeless) && ((num66 < (this.homeTileX - 0x23)) || (num66 > (this.homeTileX + 0x23))))
                {
                    if ((this.position.X < (this.homeTileX * 0x10)) && (this.direction == -1))
                    {
                        this.direction = 1;
                        this.velocity.X = 0.1f;
                        this.netUpdate = true;
                    }
                    else if ((this.position.X > (this.homeTileX * 0x10)) && (this.direction == 1))
                    {
                        this.direction = -1;
                        this.velocity.X = -0.1f;
                        this.netUpdate = true;
                    }
                }
                this.ai[1]--;
                if (this.ai[1] <= 0f)
                {
                    this.ai[0] = 0f;
                    this.ai[1] = 300 + Main.rand.Next(300);
                    this.ai[2] = 60f;
                    this.netUpdate = true;
                }
                if (this.closeDoor && ((((this.position.X + (this.width / 2)) / 16f) > (this.doorX + 2)) || (((this.position.X + (this.width / 2)) / 16f) < (this.doorX - 2))))
                {
                    if (WorldGen.CloseDoor(this.doorX, this.doorY, false))
                    {
                        this.closeDoor = false;
                        NetMessage.SendData(0x13, -1, -1, "", 1, (float) this.doorX, (float) this.doorY, (float) this.direction);
                    }
                    if (((((this.position.X + (this.width / 2)) / 16f) > (this.doorX + 4)) || (((this.position.X + (this.width / 2)) / 16f) < (this.doorX - 4))) || ((((this.position.Y + (this.height / 2)) / 16f) > (this.doorY + 4)) || (((this.position.Y + (this.height / 2)) / 16f) < (this.doorY - 4))))
                    {
                        this.closeDoor = false;
                    }
                }
                if ((this.velocity.X < -1f) || (this.velocity.X > 1f))
                {
                    if (this.velocity.Y == 0f)
                    {
                        this.velocity = (Vector2) (this.velocity * 0.8f);
                    }
                }
                else if ((this.velocity.X < 1.15) && (this.direction == 1))
                {
                    this.velocity.X += 0.07f;
                    if (this.velocity.X > 1f)
                    {
                        this.velocity.X = 1f;
                    }
                }
                else if ((this.velocity.X > -1f) && (this.direction == -1))
                {
                    this.velocity.X -= 0.07f;
                    if (this.velocity.X > 1f)
                    {
                        this.velocity.X = 1f;
                    }
                }
                if (this.velocity.Y != 0f)
                {
                    return;
                }
                if (this.position.X == this.ai[2])
                {
                    this.direction *= -1;
                }
                this.ai[2] = -1f;
                int num73 = (int) (((this.position.X + (this.width / 2)) + (15 * this.direction)) / 16f);
                int num74 = (int) (((this.position.Y + this.height) - 16f) / 16f);
                if (Main.tile[num73, num74] == null)
                {
                    Main.tile[num73, num74] = new Tile();
                }
                if (Main.tile[num73, num74 - 1] == null)
                {
                    Main.tile[num73, num74 - 1] = new Tile();
                }
                if (Main.tile[num73, num74 - 2] == null)
                {
                    Main.tile[num73, num74 - 2] = new Tile();
                }
                if (Main.tile[num73, num74 - 3] == null)
                {
                    Main.tile[num73, num74 - 3] = new Tile();
                }
                if (Main.tile[num73, num74 + 1] == null)
                {
                    Main.tile[num73, num74 + 1] = new Tile();
                }
                if (Main.tile[num73 + this.direction, num74 - 1] == null)
                {
                    Main.tile[num73 + this.direction, num74 - 1] = new Tile();
                }
                if (Main.tile[num73 + this.direction, num74 + 1] == null)
                {
                    Main.tile[num73 + this.direction, num74 + 1] = new Tile();
                }
                if ((Main.tile[num73, num74 - 2].active && (Main.tile[num73, num74 - 2].type == 10)) && ((Main.rand.Next(10) == 0) || !Main.dayTime))
                {
                    if (Main.netMode != 1)
                    {
                        if (WorldGen.OpenDoor(num73, num74 - 2, this.direction))
                        {
                            this.closeDoor = true;
                            this.doorX = num73;
                            this.doorY = num74 - 2;
                            NetMessage.SendData(0x13, -1, -1, "", 0, (float) num73, (float) (num74 - 2), (float) this.direction);
                            this.netUpdate = true;
                            this.ai[1] += 80f;
                            return;
                        }
                        if (WorldGen.OpenDoor(num73, num74 - 2, -this.direction))
                        {
                            this.closeDoor = true;
                            this.doorX = num73;
                            this.doorY = num74 - 2;
                            NetMessage.SendData(0x13, -1, -1, "", 0, (float) num73, (float) (num74 - 2), (float) -this.direction);
                            this.netUpdate = true;
                            this.ai[1] += 80f;
                            return;
                        }
                        this.direction *= -1;
                        this.netUpdate = true;
                        return;
                    }
                    return;
                }
                if (((this.velocity.X < 0f) && (this.spriteDirection == -1)) || ((this.velocity.X > 0f) && (this.spriteDirection == 1)))
                {
                    if ((Main.tile[num73, num74 - 2].active && Main.tileSolid[Main.tile[num73, num74 - 2].type]) && !Main.tileSolidTop[Main.tile[num73, num74 - 2].type])
                    {
                        if (((this.direction == 1) && !Collision.SolidTiles(num73 - 2, num73 - 1, num74 - 5, num74 - 1)) || ((this.direction == -1) && !Collision.SolidTiles(num73 + 1, num73 + 2, num74 - 5, num74 - 1)))
                        {
                            if (!Collision.SolidTiles(num73, num73, num74 - 5, num74 - 3))
                            {
                                this.velocity.Y = -6f;
                                this.netUpdate = true;
                            }
                            else
                            {
                                this.direction *= -1;
                                this.netUpdate = true;
                            }
                        }
                        else
                        {
                            this.direction *= -1;
                            this.netUpdate = true;
                        }
                    }
                    else if ((Main.tile[num73, num74 - 1].active && Main.tileSolid[Main.tile[num73, num74 - 1].type]) && !Main.tileSolidTop[Main.tile[num73, num74 - 1].type])
                    {
                        if (((this.direction == 1) && !Collision.SolidTiles(num73 - 2, num73 - 1, num74 - 4, num74 - 1)) || ((this.direction == -1) && !Collision.SolidTiles(num73 + 1, num73 + 2, num74 - 4, num74 - 1)))
                        {
                            if (!Collision.SolidTiles(num73, num73, num74 - 4, num74 - 2))
                            {
                                this.velocity.Y = -5f;
                                this.netUpdate = true;
                            }
                            else
                            {
                                this.direction *= -1;
                                this.netUpdate = true;
                            }
                        }
                        else
                        {
                            this.direction *= -1;
                            this.netUpdate = true;
                        }
                    }
                    else if ((Main.tile[num73, num74].active && Main.tileSolid[Main.tile[num73, num74].type]) && !Main.tileSolidTop[Main.tile[num73, num74].type])
                    {
                        if (((this.direction == 1) && !Collision.SolidTiles(num73 - 2, num73, num74 - 3, num74 - 1)) || ((this.direction == -1) && !Collision.SolidTiles(num73, num73 + 2, num74 - 3, num74 - 1)))
                        {
                            this.velocity.Y = -3.6f;
                            this.netUpdate = true;
                        }
                        else
                        {
                            this.direction *= -1;
                            this.netUpdate = true;
                        }
                    }
                    else if (((((((Main.netMode != 1) && (num66 >= (this.homeTileX - 0x23))) && (num66 <= (this.homeTileX + 0x23))) && (!Main.tile[num73, num74 + 1].active || !Main.tileSolid[Main.tile[num73, num74 + 1].type])) && (!Main.tile[num73 - this.direction, num74 + 1].active || !Main.tileSolid[Main.tile[num73 - this.direction, num74 + 1].type])) && ((!Main.tile[num73, num74 + 2].active || !Main.tileSolid[Main.tile[num73, num74 + 2].type]) && (!Main.tile[num73 - this.direction, num74 + 2].active || !Main.tileSolid[Main.tile[num73 - this.direction, num74 + 2].type]))) && (((!Main.tile[num73, num74 + 3].active || !Main.tileSolid[Main.tile[num73, num74 + 3].type]) && (!Main.tile[num73 - this.direction, num74 + 3].active || !Main.tileSolid[Main.tile[num73 - this.direction, num74 + 3].type])) && ((!Main.tile[num73, num74 + 4].active || !Main.tileSolid[Main.tile[num73, num74 + 4].type]) && (!Main.tile[num73 - this.direction, num74 + 4].active || !Main.tileSolid[Main.tile[num73 - this.direction, num74 + 4].type]))))
                    {
                        this.direction *= -1;
                        this.velocity.X *= -1f;
                        this.netUpdate = true;
                    }
                    if (this.velocity.Y < 0f)
                    {
                        this.ai[2] = this.position.X;
                    }
                }
                if ((this.velocity.Y >= 0f) || !this.wet)
                {
                    return;
                }
                this.velocity.Y *= 1.2f;
            }
            return;
        Label_5D99:
            if (this.ai[1] > 0f)
            {
                this.ai[1]--;
                if (this.ai[1] == 25f)
                {
                    Main.PlaySound(2, (int) this.position.X, (int) this.position.Y, 8);
                    if (Main.netMode != 1)
                    {
                        if (this.type == 0x1d)
                        {
                            NewNPC(((int) this.position.X) + (this.width / 2), ((int) this.position.Y) - 8, 30, 0);
                        }
                        else if (this.type == 0x20)
                        {
                            NewNPC(((int) this.position.X) + (this.width / 2), ((int) this.position.Y) - 8, 0x21, 0);
                        }
                        else
                        {
                            NewNPC((((int) this.position.X) + (this.width / 2)) + (this.direction * 8), ((int) this.position.Y) + 20, 0x19, 0);
                        }
                    }
                }
            }
            if (this.type == 0x1d)
            {
                if (Main.rand.Next(5) == 0)
                {
                    int num92 = Dust.NewDust(new Vector2(this.position.X, this.position.Y + 2f), this.width, this.height, 0x1b, this.velocity.X * 0.2f, this.velocity.Y * 0.2f, 100, new Color(), 1.5f);
                    Main.dust[num92].noGravity = true;
                    Main.dust[num92].velocity.X *= 0.5f;
                    Main.dust[num92].velocity.Y = -2f;
                }
            }
            else if (this.type == 0x20)
            {
                if (Main.rand.Next(2) == 0)
                {
                    int num93 = Dust.NewDust(new Vector2(this.position.X, this.position.Y + 2f), this.width, this.height, 0x1d, this.velocity.X * 0.2f, this.velocity.Y * 0.2f, 100, new Color(), 2f);
                    Main.dust[num93].noGravity = true;
                    Main.dust[num93].velocity.X *= 1f;
                    Main.dust[num93].velocity.Y *= 1f;
                }
            }
            else if (Main.rand.Next(2) == 0)
            {
                int num94 = Dust.NewDust(new Vector2(this.position.X, this.position.Y + 2f), this.width, this.height, 6, this.velocity.X * 0.2f, this.velocity.Y * 0.2f, 100, new Color(), 2f);
                Main.dust[num94].noGravity = true;
                Main.dust[num94].velocity.X *= 1f;
                Main.dust[num94].velocity.Y *= 1f;
            }
        }

        public void CheckActive()
        {
            if (this.active && ((((this.type != 8) && (this.type != 9)) && ((this.type != 11) && (this.type != 12))) && (((this.type != 14) && (this.type != 15)) && ((this.type != 40) && (this.type != 0x29)))))
            {
                if (this.townNPC)
                {
                    if (this.position.Y < (Main.worldSurface * 18.0))
                    {
                        Rectangle rectangle = new Rectangle((((int) this.position.X) + (this.width / 2)) - townRangeX, (((int) this.position.Y) + (this.height / 2)) - townRangeY, townRangeX * 2, townRangeY * 2);
                        for (int i = 0; i < 8; i++)
                        {
                            if (Main.player[i].active && rectangle.Intersects(new Rectangle((int) Main.player[i].position.X, (int) Main.player[i].position.Y, Main.player[i].width, Main.player[i].height)))
                            {
                                Player player1 = Main.player[i];
                                player1.townNPCs++;
                            }
                        }
                    }
                }
                else
                {
                    bool flag = false;
                    Rectangle rectangle2 = new Rectangle((((int) this.position.X) + (this.width / 2)) - activeRangeX, (((int) this.position.Y) + (this.height / 2)) - activeRangeY, activeRangeX * 2, activeRangeY * 2);
                    Rectangle rectangle3 = new Rectangle(((int) ((this.position.X + (this.width / 2)) - (Main.screenWidth * 0.5))) - this.width, ((int) ((this.position.Y + (this.height / 2)) - (Main.screenHeight * 0.5))) - this.height, Main.screenWidth + (this.width * 2), Main.screenHeight + (this.height * 2));
                    for (int j = 0; j < 8; j++)
                    {
                        if (Main.player[j].active)
                        {
                            if (rectangle2.Intersects(new Rectangle((int) Main.player[j].position.X, (int) Main.player[j].position.Y, Main.player[j].width, Main.player[j].height)))
                            {
                                flag = true;
                                if (((this.type != 0x19) && (this.type != 30)) && (this.type != 0x21))
                                {
                                    Player player2 = Main.player[j];
                                    player2.activeNPCs++;
                                }
                            }
                            if (rectangle3.Intersects(new Rectangle((int) Main.player[j].position.X, (int) Main.player[j].position.Y, Main.player[j].width, Main.player[j].height)))
                            {
                                this.timeLeft = activeTime;
                            }
                            if (((this.type == 7) || (this.type == 10)) || (this.type == 13))
                            {
                                flag = true;
                            }
                            if ((this.boss || (this.type == 0x23)) || (this.type == 0x24))
                            {
                                flag = true;
                            }
                        }
                    }
                    this.timeLeft--;
                    if (this.timeLeft <= 0)
                    {
                        flag = false;
                    }
                    if (!flag && (Main.netMode != 1))
                    {
                        this.active = false;
                        if (Main.netMode == 2)
                        {
                            this.life = 0;
                            NetMessage.SendData(0x17, -1, -1, "", this.whoAmI, 0f, 0f, 0f);
                        }
                    }
                }
            }
        }

        public void FindFrame()
        {
            int num = Main.npcTexture[this.type].Height / Main.npcFrameCount[this.type];
            int num2 = 0;
            if (this.aiAction == 0)
            {
                if (this.velocity.Y < 0f)
                {
                    num2 = 2;
                }
                else if (this.velocity.Y > 0f)
                {
                    num2 = 3;
                }
                else if (this.velocity.X != 0f)
                {
                    num2 = 1;
                }
                else
                {
                    num2 = 0;
                }
            }
            else if (this.aiAction == 1)
            {
                num2 = 4;
            }
            if ((this.type == 1) || (this.type == 0x10))
            {
                this.frameCounter++;
                if (num2 > 0)
                {
                    this.frameCounter++;
                }
                if (num2 == 4)
                {
                    this.frameCounter++;
                }
                if (this.frameCounter >= 8.0)
                {
                    this.frame.Y += num;
                    this.frameCounter = 0.0;
                }
                if (this.frame.Y >= (num * Main.npcFrameCount[this.type]))
                {
                    this.frame.Y = 0;
                }
            }
            if ((this.type == 2) || (this.type == 0x17))
            {
                if (this.velocity.X > 0f)
                {
                    this.spriteDirection = 1;
                    this.rotation = (float) Math.Atan2((double) this.velocity.Y, (double) this.velocity.X);
                }
                if (this.velocity.X < 0f)
                {
                    this.spriteDirection = -1;
                    this.rotation = ((float) Math.Atan2((double) this.velocity.Y, (double) this.velocity.X)) + 3.14f;
                }
                this.frameCounter++;
                if (this.frameCounter >= 8.0)
                {
                    this.frame.Y += num;
                    this.frameCounter = 0.0;
                }
                if (this.frame.Y >= (num * Main.npcFrameCount[this.type]))
                {
                    this.frame.Y = 0;
                }
            }
            if (this.type == 0x2a)
            {
                if (this.velocity.X > 0f)
                {
                    this.spriteDirection = 1;
                    this.rotation = (float) Math.Atan2((double) this.velocity.Y, (double) this.velocity.X);
                }
                if (this.velocity.X < 0f)
                {
                    this.spriteDirection = -1;
                    this.rotation = ((float) Math.Atan2((double) this.velocity.Y, (double) this.velocity.X)) + 3.14f;
                }
                this.frameCounter++;
                if (this.frameCounter < 4.0)
                {
                    this.frame.Y = 0;
                }
                else if (this.frameCounter < 8.0)
                {
                    this.frame.Y = num;
                }
                else if (this.frameCounter < 12.0)
                {
                    this.frame.Y = num * 2;
                }
                else if (this.frameCounter < 16.0)
                {
                    this.frame.Y = num;
                }
                if (this.frameCounter == 15.0)
                {
                    this.frameCounter = 0.0;
                }
            }
            if (this.type == 0x2b)
            {
                this.frameCounter++;
                if (this.frameCounter < 6.0)
                {
                    this.frame.Y = 0;
                }
                else if (this.frameCounter < 12.0)
                {
                    this.frame.Y = num;
                }
                else if (this.frameCounter < 18.0)
                {
                    this.frame.Y = num * 2;
                }
                else if (this.frameCounter < 24.0)
                {
                    this.frame.Y = num;
                }
                if (this.frameCounter == 23.0)
                {
                    this.frameCounter = 0.0;
                }
            }
            if ((((this.type == 0x11) || (this.type == 0x12)) || ((this.type == 0x13) || (this.type == 20))) || ((((this.type == 0x16) || (this.type == 0x26)) || ((this.type == 0x1a) || (this.type == 0x1b))) || (((this.type == 0x1c) || (this.type == 0x1f)) || (this.type == 0x15))))
            {
                if (this.velocity.Y == 0f)
                {
                    if (this.direction == 1)
                    {
                        this.spriteDirection = 1;
                    }
                    if (this.direction == -1)
                    {
                        this.spriteDirection = -1;
                    }
                    if (this.velocity.X == 0f)
                    {
                        this.frame.Y = 0;
                        this.frameCounter = 0.0;
                    }
                    else
                    {
                        this.frameCounter += Math.Abs(this.velocity.X) * 2f;
                        this.frameCounter++;
                        if (this.frameCounter > 6.0)
                        {
                            this.frame.Y += num;
                            this.frameCounter = 0.0;
                        }
                        if ((this.frame.Y / num) >= Main.npcFrameCount[this.type])
                        {
                            this.frame.Y = num * 2;
                        }
                    }
                }
                else
                {
                    this.frameCounter = 0.0;
                    this.frame.Y = num;
                }
            }
            else if ((((this.type == 3) || this.townNPC) || ((this.type == 0x15) || (this.type == 0x1a))) || (((this.type == 0x1b) || (this.type == 0x1c)) || (this.type == 0x1f)))
            {
                if (this.velocity.Y == 0f)
                {
                    if (this.direction == 1)
                    {
                        this.spriteDirection = 1;
                    }
                    if (this.direction == -1)
                    {
                        this.spriteDirection = -1;
                    }
                }
                if (((this.velocity.Y != 0f) || ((this.direction == -1) && (this.velocity.X > 0f))) || ((this.direction == 1) && (this.velocity.X < 0f)))
                {
                    this.frameCounter = 0.0;
                    this.frame.Y = num * 2;
                }
                else if (this.velocity.X == 0f)
                {
                    this.frameCounter = 0.0;
                    this.frame.Y = 0;
                }
                else
                {
                    this.frameCounter += Math.Abs(this.velocity.X);
                    if (this.frameCounter < 8.0)
                    {
                        this.frame.Y = 0;
                    }
                    else if (this.frameCounter < 16.0)
                    {
                        this.frame.Y = num;
                    }
                    else if (this.frameCounter < 24.0)
                    {
                        this.frame.Y = num * 2;
                    }
                    else if (this.frameCounter < 32.0)
                    {
                        this.frame.Y = num;
                    }
                    else
                    {
                        this.frameCounter = 0.0;
                    }
                }
            }
            else if (this.type == 4)
            {
                this.frameCounter++;
                if (this.frameCounter < 7.0)
                {
                    this.frame.Y = 0;
                }
                else if (this.frameCounter < 14.0)
                {
                    this.frame.Y = num;
                }
                else if (this.frameCounter < 21.0)
                {
                    this.frame.Y = num * 2;
                }
                else
                {
                    this.frameCounter = 0.0;
                    this.frame.Y = 0;
                }
                if (this.ai[0] > 1f)
                {
                    this.frame.Y += num * 3;
                }
            }
            else if (this.type == 5)
            {
                this.frameCounter++;
                if (this.frameCounter >= 8.0)
                {
                    this.frame.Y += num;
                    this.frameCounter = 0.0;
                }
                if (this.frame.Y >= (num * Main.npcFrameCount[this.type]))
                {
                    this.frame.Y = 0;
                }
            }
            else if (this.type == 6)
            {
                this.frameCounter++;
                if (this.frameCounter >= 8.0)
                {
                    this.frame.Y += num;
                    this.frameCounter = 0.0;
                }
                if (this.frame.Y >= (num * Main.npcFrameCount[this.type]))
                {
                    this.frame.Y = 0;
                }
            }
            else if (this.type == 0x18)
            {
                if (this.velocity.Y == 0f)
                {
                    if (this.direction == 1)
                    {
                        this.spriteDirection = 1;
                    }
                    if (this.direction == -1)
                    {
                        this.spriteDirection = -1;
                    }
                }
                if (this.ai[1] > 0f)
                {
                    if (this.frame.Y < 4)
                    {
                        this.frameCounter = 0.0;
                    }
                    this.frameCounter++;
                    if (this.frameCounter <= 4.0)
                    {
                        this.frame.Y = num * 4;
                    }
                    else if (this.frameCounter <= 8.0)
                    {
                        this.frame.Y = num * 5;
                    }
                    else if (this.frameCounter <= 12.0)
                    {
                        this.frame.Y = num * 6;
                    }
                    else if (this.frameCounter <= 16.0)
                    {
                        this.frame.Y = num * 7;
                    }
                    else if (this.frameCounter <= 20.0)
                    {
                        this.frame.Y = num * 8;
                    }
                    else
                    {
                        this.frame.Y = num * 9;
                        this.frameCounter = 100.0;
                    }
                }
                else
                {
                    this.frameCounter++;
                    if (this.frameCounter <= 4.0)
                    {
                        this.frame.Y = 0;
                    }
                    else if (this.frameCounter <= 8.0)
                    {
                        this.frame.Y = num;
                    }
                    else if (this.frameCounter <= 12.0)
                    {
                        this.frame.Y = num * 2;
                    }
                    else
                    {
                        this.frame.Y = num * 3;
                        if (this.frameCounter >= 16.0)
                        {
                            this.frameCounter = 0.0;
                        }
                    }
                }
            }
            else if ((this.type == 0x1d) || (this.type == 0x20))
            {
                if (this.velocity.Y == 0f)
                {
                    if (this.direction == 1)
                    {
                        this.spriteDirection = 1;
                    }
                    if (this.direction == -1)
                    {
                        this.spriteDirection = -1;
                    }
                }
                this.frame.Y = 0;
                if (this.velocity.Y != 0f)
                {
                    this.frame.Y += num;
                }
                else if (this.ai[1] > 0f)
                {
                    this.frame.Y += num * 2;
                }
            }
            if (this.type == 0x22)
            {
                if (this.velocity.X > 0f)
                {
                    this.spriteDirection = -1;
                    this.rotation = (float) Math.Atan2((double) this.velocity.Y, (double) this.velocity.X);
                }
                if (this.velocity.X < 0f)
                {
                    this.spriteDirection = 1;
                    this.rotation = ((float) Math.Atan2((double) this.velocity.Y, (double) this.velocity.X)) + 3.14f;
                }
                this.frameCounter++;
                if (this.frameCounter >= 4.0)
                {
                    this.frame.Y += num;
                    this.frameCounter = 0.0;
                }
                if (this.frame.Y >= (num * Main.npcFrameCount[this.type]))
                {
                    this.frame.Y = 0;
                }
            }
        }

        public Color GetAlpha(Color newColor)
        {
            int r = newColor.R - this.alpha;
            int g = newColor.G - this.alpha;
            int b = newColor.B - this.alpha;
            int a = newColor.A - this.alpha;
            if (((this.type == 0x19) || (this.type == 30)) || (this.type == 0x21))
            {
                r = newColor.R;
                g = newColor.G;
                b = newColor.B;
            }
            if (a < 0)
            {
                a = 0;
            }
            if (a > 0xff)
            {
                a = 0xff;
            }
            return new Color(r, g, b, a);
        }

        public string GetChat()
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            bool flag6 = false;
            for (int i = 0; i < 0x3e8; i++)
            {
                if (Main.npc[i].type == 0x11)
                {
                    flag = true;
                }
                else if (Main.npc[i].type == 0x12)
                {
                    flag2 = true;
                }
                else if (Main.npc[i].type == 0x13)
                {
                    flag3 = true;
                }
                else if (Main.npc[i].type == 20)
                {
                    flag4 = true;
                }
                else if (Main.npc[i].type == 0x25)
                {
                    flag5 = true;
                }
                else if (Main.npc[i].type == 0x26)
                {
                    flag6 = true;
                }
            }
            string str = "";
            if (this.type == 0x11)
            {
                if (Main.dayTime)
                {
                    if (Main.time < 16200.0)
                    {
                        if (Main.rand.Next(2) == 0)
                        {
                            return "Sword beats paper, get one today.";
                        }
                        return "Lovely morning, wouldn't you say? Was there something you needed?";
                    }
                    if (Main.time > 37800.0)
                    {
                        if (Main.rand.Next(2) == 0)
                        {
                            return "Night be upon us soon, friend. Make your choices while you can.";
                        }
                        return ("Ah, they will tell tales of " + Main.player[Main.myPlayer].name + " some day... good ones I'm sure.");
                    }
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            str = "Check out my dirt blocks, they are extra dirty.";
                            break;

                        case 1:
                            return "Boy, that sun is hot! I do have some perfectly ventilated armor.";
                    }
                    return "The sun is high, but my prices are not.";
                }
                if (Main.bloodMoon)
                {
                    if (Main.rand.Next(2) == 0)
                    {
                        return "Have you seen Chith...Shith.. Chat... The big eye?";
                    }
                    return "Keep your eye on the prize, buy a lense!";
                }
                if (Main.time < 9720.0)
                {
                    if (Main.rand.Next(2) == 0)
                    {
                        return "Kosh, kapleck Mog. Oh sorry, thats klingon for 'Buy something or die.'";
                    }
                    return (Main.player[Main.myPlayer].name + " is it? I've heard good things, friend!");
                }
                if (Main.time > 22680.0)
                {
                    if (Main.rand.Next(2) == 0)
                    {
                        return "I hear there's a secret treasure... oh never mind.";
                    }
                    return "Angel Statue you say? I'm sorry, I'm not a junk dealer.";
                }
                int num8 = Main.rand.Next(3);
                if (num8 == 0)
                {
                    str = "The last guy who was here left me some junk..er I mean.. treasures!";
                }
                if (num8 == 1)
                {
                    return "I wonder if the moon is made of cheese...huh, what? Oh yes, buy something!";
                }
                return "Did you say gold?  I'll take that off of ya'.";
            }
            if (this.type == 0x12)
            {
                if (flag6 && (Main.rand.Next(4) == 0))
                {
                    return "I wish that bomb maker would be more careful.  I'm getting tired of having to sew his limbs back on every day.";
                }
                if (Main.player[Main.myPlayer].statLife < (Main.player[Main.myPlayer].statLifeMax * 0.33))
                {
                    switch (Main.rand.Next(5))
                    {
                        case 0:
                            return "I think you look better this way.";

                        case 1:
                            return "Eww.. What happened to your face?";

                        case 2:
                            return "MY GOODNESS! I'm good but I'm not THAT good.";

                        case 3:
                            return "Dear friends we are gathered here today to bid farewell... oh, you'll be fine.";
                    }
                    return "You left your arm over there. Let me get that for you..";
                }
                if (Main.player[Main.myPlayer].statLife < (Main.player[Main.myPlayer].statLifeMax * 0.66))
                {
                    switch (Main.rand.Next(4))
                    {
                        case 0:
                            return "Quit being such a baby! I've seen worse.";

                        case 1:
                            return "That's gonna need stitches!";

                        case 2:
                            return "Trouble with those bullies again?";
                    }
                    return "You look half digested. Have you been chasing slimes again?";
                }
                switch (Main.rand.Next(3))
                {
                    case 0:
                        return "Turn your head and cough.";

                    case 1:
                        return "Thats not the biggest I've ever seen... Yes, I've seen bigger wounds for sure.";
                }
                return "Show me where it hurts.";
            }
            if (this.type == 0x13)
            {
                if (flag2 && (Main.rand.Next(4) == 0))
                {
                    return "Make it quick! I've got a date with the nurse in an hour.";
                }
                if (flag4 && (Main.rand.Next(4) == 0))
                {
                    return "That dryad is a looker.  Too bad she's such a prude.";
                }
                if (flag6 && (Main.rand.Next(4) == 0))
                {
                    return "Don't bother with that firework vendor, I've got all you need right here.";
                }
                if (Main.bloodMoon)
                {
                    return "I love nights like tonight.  There is never a shortage of things to kill!";
                }
                switch (Main.rand.Next(2))
                {
                    case 0:
                        return "I see you're eyeballin' the Minishark.. You really don't want to know how it was made.";

                    case 1:
                        str = "Keep your hands off my gun, buddy!";
                        break;
                }
                return str;
            }
            if (this.type == 20)
            {
                if (flag3 && (Main.rand.Next(4) == 0))
                {
                    return "I wish that gun seller would stop talking to me. Doesn't he realize I'm 500 years old?";
                }
                if (flag && (Main.rand.Next(4) == 0))
                {
                    return "That merchant keeps trying to sell me an angel statue. Everyone knows that they don't do anything.";
                }
                if (flag5 && (Main.rand.Next(4) == 0))
                {
                    return "Have you seen the old man walking around the dungeon? He doesn't look well at all...";
                }
                if (Main.bloodMoon)
                {
                    return "It is an evil moon tonight. Be careful.";
                }
                int num13 = Main.rand.Next(6);
                if (num13 == 0)
                {
                    return "You must cleanse the world of this corruption.";
                }
                if (num13 != 1)
                {
                    if (num13 == 2)
                    {
                        return "The sands of time are flowing. And well, you are not aging very gracefully.";
                    }
                    if (num13 == 3)
                    {
                        return "Whats this about me having more 'bark' than bite?";
                    }
                    if (num13 == 4)
                    {
                        return "So two goblins walk into a bar, and one says to the other, 'Want to get a Gobblet of beer?!'";
                    }
                }
                return "Be safe; Terraria needs you!";
            }
            if (this.type == 0x16)
            {
                if (Main.bloodMoon)
                {
                    return "You can tell a Blood Moon is out when the sky turns red. There is something about it that causes monsters to swarm.";
                }
                if (!Main.dayTime)
                {
                    return "You should stay indoors at night. It is very dangerous to be wandering around in the dark.";
                }
                switch (Main.rand.Next(3))
                {
                    case 0:
                        return ("Greetings, " + Main.player[Main.myPlayer].name + ". Is there something I can help you with?");

                    case 1:
                        return "I am here to give you advice on what to do next.  It is recommended that you talk with me anytime you get stuck.";

                    case 2:
                        str = "They say there is a person who will tell you how to survive in this land... oh wait. Thats me.";
                        break;
                }
                return str;
            }
            if (this.type == 0x25)
            {
                if (Main.dayTime)
                {
                    switch (Main.rand.Next(2))
                    {
                        case 0:
                            return "I cannot let you enter until you free me of my curse.";

                        case 1:
                            str = "Come back at night if you wish to enter.";
                            break;
                    }
                }
                return str;
            }
            if (this.type != 0x26)
            {
                return str;
            }
            if (Main.bloodMoon)
            {
                return "I've got something for them zombies alright!";
            }
            if (flag3 && (Main.rand.Next(4) == 0))
            {
                return "Even the gun dealer wants what I'm selling!";
            }
            if (flag2 && (Main.rand.Next(4) == 0))
            {
                return "I'm sure the nurse will help if you accidentally lose a limb to these.";
            }
            if (flag4 && (Main.rand.Next(4) == 0))
            {
                return "Why purify the world when you can just blow it up?";
            }
            int num16 = Main.rand.Next(4);
            if (num16 == 0)
            {
                return "Explosives are da' bomb these days.  Buy some now!";
            }
            if (num16 == 1)
            {
                return "It's a good day to die!";
            }
            if (num16 == 2)
            {
                return "I wonder what happens if I... (BOOM!)... Oh, sorry, did you need that leg?";
            }
            if (num16 == 3)
            {
                return "Dynamite, my own special cure-all for what ails ya.";
            }
            return "Check out my goods; they have explosive prices!";
        }

        public Color GetColor(Color newColor)
        {
            int r = this.color.R - (0xff - newColor.R);
            int g = this.color.G - (0xff - newColor.G);
            int b = this.color.B - (0xff - newColor.B);
            int a = this.color.A - (0xff - newColor.A);
            if (r < 0)
            {
                r = 0;
            }
            if (r > 0xff)
            {
                r = 0xff;
            }
            if (g < 0)
            {
                g = 0;
            }
            if (g > 0xff)
            {
                g = 0xff;
            }
            if (b < 0)
            {
                b = 0;
            }
            if (b > 0xff)
            {
                b = 0xff;
            }
            if (a < 0)
            {
                a = 0;
            }
            if (a > 0xff)
            {
                a = 0xff;
            }
            return new Color(r, g, b, a);
        }

        public void HitEffect(int hitDirection = 0, double dmg = 10.0)
        {
            if ((this.type == 1) || (this.type == 0x10))
            {
                if (this.life > 0)
                {
                    for (int i = 0; i < ((dmg / ((double) this.lifeMax)) * 100.0); i++)
                    {
                        Dust.NewDust(this.position, this.width, this.height, 4, (float) hitDirection, -1f, this.alpha, this.color, 1f);
                    }
                }
                else
                {
                    for (int j = 0; j < 50; j++)
                    {
                        Dust.NewDust(this.position, this.width, this.height, 4, (float) (2 * hitDirection), -2f, this.alpha, this.color, 1f);
                    }
                    if ((Main.netMode != 1) && (this.type == 0x10))
                    {
                        int num3 = Main.rand.Next(2) + 2;
                        for (int k = 0; k < num3; k++)
                        {
                            int index = NewNPC(((int) this.position.X) + (this.width / 2), ((int) this.position.Y) + this.height, 1, 0);
                            Main.npc[index].SetDefaults("Baby Slime");
                            Main.npc[index].velocity.X = this.velocity.X * 2f;
                            Main.npc[index].velocity.Y = this.velocity.Y;
                            Main.npc[index].velocity.X += (Main.rand.Next(-20, 20) * 0.1f) + ((k * this.direction) * 0.3f);
                            Main.npc[index].velocity.Y -= (Main.rand.Next(0, 10) * 0.1f) + k;
                            Main.npc[index].ai[1] = k;
                            if ((Main.netMode == 2) && (index < 0x3e8))
                            {
                                NetMessage.SendData(0x17, -1, -1, "", index, 0f, 0f, 0f);
                            }
                        }
                    }
                }
            }
            else
            {
                Color color;
                if (this.type == 2)
                {
                    if (this.life > 0)
                    {
                        for (int m = 0; m < ((dmg / ((double) this.lifeMax)) * 100.0); m++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, (float) hitDirection, -1f, 0, color, 1f);
                        }
                    }
                    else
                    {
                        for (int n = 0; n < 50; n++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, (float) (2 * hitDirection), -2f, 0, color, 1f);
                        }
                        Gore.NewGore(this.position, this.velocity, 1);
                        Gore.NewGore(new Vector2(this.position.X + 14f, this.position.Y), this.velocity, 2);
                    }
                }
                else if (this.type == 3)
                {
                    if (this.life > 0)
                    {
                        for (int num8 = 0; num8 < ((dmg / ((double) this.lifeMax)) * 100.0); num8++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, (float) hitDirection, -1f, 0, color, 1f);
                        }
                    }
                    else
                    {
                        for (int num9 = 0; num9 < 50; num9++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, 2.5f * hitDirection, -2.5f, 0, color, 1f);
                        }
                        Gore.NewGore(this.position, this.velocity, 3);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 4);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 4);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 5);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 5);
                    }
                }
                else if (this.type == 4)
                {
                    if (this.life > 0)
                    {
                        for (int num10 = 0; num10 < ((dmg / ((double) this.lifeMax)) * 100.0); num10++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, (float) hitDirection, -1f, 0, color, 1f);
                        }
                    }
                    else
                    {
                        for (int num11 = 0; num11 < 150; num11++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, (float) (2 * hitDirection), -2f, 0, color, 1f);
                        }
                        for (int num12 = 0; num12 < 2; num12++)
                        {
                            Gore.NewGore(this.position, new Vector2(Main.rand.Next(-30, 0x1f) * 0.2f, Main.rand.Next(-30, 0x1f) * 0.2f), 2);
                            Gore.NewGore(this.position, new Vector2(Main.rand.Next(-30, 0x1f) * 0.2f, Main.rand.Next(-30, 0x1f) * 0.2f), 7);
                            Gore.NewGore(this.position, new Vector2(Main.rand.Next(-30, 0x1f) * 0.2f, Main.rand.Next(-30, 0x1f) * 0.2f), 9);
                            Gore.NewGore(this.position, new Vector2(Main.rand.Next(-30, 0x1f) * 0.2f, Main.rand.Next(-30, 0x1f) * 0.2f), 10);
                        }
                        Main.PlaySound(15, (int) this.position.X, (int) this.position.Y, 0);
                    }
                }
                else if (this.type == 5)
                {
                    if (this.life > 0)
                    {
                        for (int num13 = 0; num13 < ((dmg / ((double) this.lifeMax)) * 50.0); num13++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, (float) hitDirection, -1f, 0, color, 1f);
                        }
                    }
                    else
                    {
                        for (int num14 = 0; num14 < 20; num14++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, (float) (2 * hitDirection), -2f, 0, color, 1f);
                        }
                        Gore.NewGore(this.position, this.velocity, 6);
                        Gore.NewGore(this.position, this.velocity, 7);
                    }
                }
                else if (this.type == 6)
                {
                    if (this.life > 0)
                    {
                        for (int num15 = 0; num15 < ((dmg / ((double) this.lifeMax)) * 100.0); num15++)
                        {
                            Dust.NewDust(this.position, this.width, this.height, 0x12, (float) hitDirection, -1f, this.alpha, this.color, this.scale);
                        }
                    }
                    else
                    {
                        for (int num16 = 0; num16 < 50; num16++)
                        {
                            Dust.NewDust(this.position, this.width, this.height, 0x12, (float) hitDirection, -2f, this.alpha, this.color, this.scale);
                        }
                        int num17 = Gore.NewGore(this.position, this.velocity, 14);
                        Main.gore[num17].alpha = this.alpha;
                        num17 = Gore.NewGore(this.position, this.velocity, 15);
                        Main.gore[num17].alpha = this.alpha;
                    }
                }
                else if (((this.type == 7) || (this.type == 8)) || (this.type == 9))
                {
                    if (this.life > 0)
                    {
                        for (int num18 = 0; num18 < ((dmg / ((double) this.lifeMax)) * 100.0); num18++)
                        {
                            Dust.NewDust(this.position, this.width, this.height, 0x12, (float) hitDirection, -1f, this.alpha, this.color, this.scale);
                        }
                    }
                    else
                    {
                        for (int num19 = 0; num19 < 50; num19++)
                        {
                            Dust.NewDust(this.position, this.width, this.height, 0x12, (float) hitDirection, -2f, this.alpha, this.color, this.scale);
                        }
                        int num20 = Gore.NewGore(this.position, this.velocity, (this.type - 7) + 0x12);
                        Main.gore[num20].alpha = this.alpha;
                    }
                }
                else if (((this.type == 10) || (this.type == 11)) || (this.type == 12))
                {
                    if (this.life > 0)
                    {
                        for (int num21 = 0; num21 < ((dmg / ((double) this.lifeMax)) * 50.0); num21++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, (float) hitDirection, -1f, 0, color, 1f);
                        }
                    }
                    else
                    {
                        for (int num22 = 0; num22 < 10; num22++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, 2.5f * hitDirection, -2.5f, 0, color, 1f);
                        }
                        Gore.NewGore(this.position, this.velocity, (this.type - 7) + 0x12);
                    }
                }
                else if (((this.type == 13) || (this.type == 14)) || (this.type == 15))
                {
                    if (this.life > 0)
                    {
                        for (int num23 = 0; num23 < ((dmg / ((double) this.lifeMax)) * 100.0); num23++)
                        {
                            Dust.NewDust(this.position, this.width, this.height, 0x12, (float) hitDirection, -1f, this.alpha, this.color, this.scale);
                        }
                    }
                    else
                    {
                        for (int num24 = 0; num24 < 50; num24++)
                        {
                            Dust.NewDust(this.position, this.width, this.height, 0x12, (float) hitDirection, -2f, this.alpha, this.color, this.scale);
                        }
                        if (this.type == 13)
                        {
                            Gore.NewGore(this.position, this.velocity, 0x18);
                            Gore.NewGore(this.position, this.velocity, 0x19);
                        }
                        else if (this.type == 14)
                        {
                            Gore.NewGore(this.position, this.velocity, 0x1a);
                            Gore.NewGore(this.position, this.velocity, 0x1b);
                        }
                        else
                        {
                            Gore.NewGore(this.position, this.velocity, 0x1c);
                            Gore.NewGore(this.position, this.velocity, 0x1d);
                        }
                    }
                }
                else if (this.type == 0x11)
                {
                    if (this.life > 0)
                    {
                        for (int num25 = 0; num25 < ((dmg / ((double) this.lifeMax)) * 100.0); num25++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, (float) hitDirection, -1f, 0, color, 1f);
                        }
                    }
                    else
                    {
                        for (int num26 = 0; num26 < 50; num26++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, 2.5f * hitDirection, -2.5f, 0, color, 1f);
                        }
                        Gore.NewGore(this.position, this.velocity, 30);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 0x1f);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 0x1f);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 0x20);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 0x20);
                    }
                }
                else if (this.type == 0x25)
                {
                    if (this.life > 0)
                    {
                        for (int num27 = 0; num27 < ((dmg / ((double) this.lifeMax)) * 100.0); num27++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, (float) hitDirection, -1f, 0, color, 1f);
                        }
                    }
                    else
                    {
                        for (int num28 = 0; num28 < 50; num28++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, 2.5f * hitDirection, -2.5f, 0, color, 1f);
                        }
                        Gore.NewGore(this.position, this.velocity, 0x3a);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 0x3b);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 0x3b);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 60);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 60);
                    }
                }
                else if (this.type == 0x12)
                {
                    if (this.life > 0)
                    {
                        for (int num29 = 0; num29 < ((dmg / ((double) this.lifeMax)) * 100.0); num29++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, (float) hitDirection, -1f, 0, color, 1f);
                        }
                    }
                    else
                    {
                        for (int num30 = 0; num30 < 50; num30++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, 2.5f * hitDirection, -2.5f, 0, color, 1f);
                        }
                        Gore.NewGore(this.position, this.velocity, 0x21);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 0x22);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 0x22);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 0x23);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 0x23);
                    }
                }
                else if (this.type == 0x13)
                {
                    if (this.life > 0)
                    {
                        for (int num31 = 0; num31 < ((dmg / ((double) this.lifeMax)) * 100.0); num31++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, (float) hitDirection, -1f, 0, color, 1f);
                        }
                    }
                    else
                    {
                        for (int num32 = 0; num32 < 50; num32++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, 2.5f * hitDirection, -2.5f, 0, color, 1f);
                        }
                        Gore.NewGore(this.position, this.velocity, 0x24);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 0x25);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 0x25);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 0x26);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 0x26);
                    }
                }
                else if (this.type == 0x26)
                {
                    if (this.life > 0)
                    {
                        for (int num33 = 0; num33 < ((dmg / ((double) this.lifeMax)) * 100.0); num33++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, (float) hitDirection, -1f, 0, color, 1f);
                        }
                    }
                    else
                    {
                        for (int num34 = 0; num34 < 50; num34++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, 2.5f * hitDirection, -2.5f, 0, color, 1f);
                        }
                        Gore.NewGore(this.position, this.velocity, 0x40);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 0x41);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 0x41);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 0x42);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 0x42);
                    }
                }
                else if (this.type == 20)
                {
                    if (this.life > 0)
                    {
                        for (int num35 = 0; num35 < ((dmg / ((double) this.lifeMax)) * 100.0); num35++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, (float) hitDirection, -1f, 0, color, 1f);
                        }
                    }
                    else
                    {
                        for (int num36 = 0; num36 < 50; num36++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, 2.5f * hitDirection, -2.5f, 0, color, 1f);
                        }
                        Gore.NewGore(this.position, this.velocity, 0x27);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 40);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 40);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 0x29);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 0x29);
                    }
                }
                else if (((this.type == 0x15) || (this.type == 0x1f)) || (this.type == 0x20))
                {
                    if (this.life > 0)
                    {
                        for (int num37 = 0; num37 < ((dmg / ((double) this.lifeMax)) * 50.0); num37++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 0x1a, (float) hitDirection, -1f, 0, color, 1f);
                        }
                    }
                    else
                    {
                        for (int num38 = 0; num38 < 20; num38++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 0x1a, 2.5f * hitDirection, -2.5f, 0, color, 1f);
                        }
                        Gore.NewGore(this.position, this.velocity, 0x2a);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 0x2b);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 0x2b);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 0x2c);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 0x2c);
                    }
                }
                else if (((this.type == 0x27) || (this.type == 40)) || (this.type == 0x29))
                {
                    if (this.life > 0)
                    {
                        for (int num39 = 0; num39 < ((dmg / ((double) this.lifeMax)) * 50.0); num39++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 0x1a, (float) hitDirection, -1f, 0, color, 1f);
                        }
                    }
                    else
                    {
                        for (int num40 = 0; num40 < 20; num40++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 0x1a, 2.5f * hitDirection, -2.5f, 0, color, 1f);
                        }
                        Gore.NewGore(this.position, this.velocity, (this.type - 0x27) + 0x43);
                    }
                }
                else if (this.type == 0x22)
                {
                    if (this.life > 0)
                    {
                        for (int num41 = 0; num41 < ((dmg / ((double) this.lifeMax)) * 50.0); num41++)
                        {
                            color = new Color();
                            int num42 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, -this.velocity.X * 0.2f, -this.velocity.Y * 0.2f, 100, color, 3f);
                            Main.dust[num42].noLight = true;
                            Main.dust[num42].noGravity = true;
                            Dust dust1 = Main.dust[num42];
                            dust1.velocity = (Vector2) (dust1.velocity * 2f);
                            color = new Color();
                            num42 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, -this.velocity.X * 0.2f, -this.velocity.Y * 0.2f, 100, color, 2f);
                            Main.dust[num42].noLight = true;
                            Dust dust2 = Main.dust[num42];
                            dust2.velocity = (Vector2) (dust2.velocity * 2f);
                        }
                    }
                    else
                    {
                        for (int num43 = 0; num43 < 20; num43++)
                        {
                            color = new Color();
                            int num44 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, -this.velocity.X * 0.2f, -this.velocity.Y * 0.2f, 100, color, 3f);
                            Main.dust[num44].noLight = true;
                            Main.dust[num44].noGravity = true;
                            Dust dust3 = Main.dust[num44];
                            dust3.velocity = (Vector2) (dust3.velocity * 2f);
                            color = new Color();
                            num44 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, -this.velocity.X * 0.2f, -this.velocity.Y * 0.2f, 100, color, 2f);
                            Main.dust[num44].noLight = true;
                            Dust dust4 = Main.dust[num44];
                            dust4.velocity = (Vector2) (dust4.velocity * 2f);
                        }
                    }
                }
                else if ((this.type == 0x23) || (this.type == 0x24))
                {
                    if (this.life > 0)
                    {
                        for (int num45 = 0; num45 < ((dmg / ((double) this.lifeMax)) * 100.0); num45++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 0x1a, (float) hitDirection, -1f, 0, color, 1f);
                        }
                    }
                    else
                    {
                        for (int num46 = 0; num46 < 150; num46++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 0x1a, 2.5f * hitDirection, -2.5f, 0, color, 1f);
                        }
                        if (this.type == 0x23)
                        {
                            Gore.NewGore(this.position, this.velocity, 0x36);
                            Gore.NewGore(this.position, this.velocity, 0x37);
                        }
                        else
                        {
                            Gore.NewGore(this.position, this.velocity, 0x38);
                            Gore.NewGore(this.position, this.velocity, 0x39);
                            Gore.NewGore(this.position, this.velocity, 0x39);
                            Gore.NewGore(this.position, this.velocity, 0x39);
                        }
                    }
                }
                else if (this.type == 0x17)
                {
                    if (this.life > 0)
                    {
                        for (int num47 = 0; num47 < ((dmg / ((double) this.lifeMax)) * 100.0); num47++)
                        {
                            int type = 0x19;
                            if (Main.rand.Next(2) == 0)
                            {
                                type = 6;
                            }
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, type, (float) hitDirection, -1f, 0, color, 1f);
                            color = new Color();
                            int num49 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, this.velocity.X * 0.2f, this.velocity.Y * 0.2f, 100, color, 2f);
                            Main.dust[num49].noGravity = true;
                        }
                    }
                    else
                    {
                        for (int num50 = 0; num50 < 50; num50++)
                        {
                            int num51 = 0x19;
                            if (Main.rand.Next(2) == 0)
                            {
                                num51 = 6;
                            }
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, num51, (float) (2 * hitDirection), -2f, 0, color, 1f);
                        }
                        for (int num52 = 0; num52 < 50; num52++)
                        {
                            color = new Color();
                            int num53 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, this.velocity.X * 0.2f, this.velocity.Y * 0.2f, 100, color, 2.5f);
                            Dust dust5 = Main.dust[num53];
                            dust5.velocity = (Vector2) (dust5.velocity * 6f);
                            Main.dust[num53].noGravity = true;
                        }
                    }
                }
                else if (this.type == 0x18)
                {
                    if (this.life > 0)
                    {
                        for (int num54 = 0; num54 < ((dmg / ((double) this.lifeMax)) * 100.0); num54++)
                        {
                            color = new Color();
                            int num55 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, this.velocity.X, this.velocity.Y, 100, color, 2.5f);
                            Main.dust[num55].noGravity = true;
                        }
                    }
                    else
                    {
                        for (int num56 = 0; num56 < 50; num56++)
                        {
                            color = new Color();
                            int num57 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, this.velocity.X, this.velocity.Y, 100, color, 2.5f);
                            Main.dust[num57].noGravity = true;
                            Dust dust6 = Main.dust[num57];
                            dust6.velocity = (Vector2) (dust6.velocity * 2f);
                        }
                        Gore.NewGore(this.position, this.velocity, 0x2d);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 0x2e);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 0x2e);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 0x2f);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 0x2f);
                    }
                }
                else if (this.type == 0x19)
                {
                    Main.PlaySound(2, (int) this.position.X, (int) this.position.Y, 10);
                    for (int num58 = 0; num58 < 20; num58++)
                    {
                        color = new Color();
                        int num59 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, -this.velocity.X * 0.2f, -this.velocity.Y * 0.2f, 100, color, 2f);
                        Main.dust[num59].noGravity = true;
                        Dust dust7 = Main.dust[num59];
                        dust7.velocity = (Vector2) (dust7.velocity * 2f);
                        color = new Color();
                        num59 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, -this.velocity.X * 0.2f, -this.velocity.Y * 0.2f, 100, color, 1f);
                        Dust dust8 = Main.dust[num59];
                        dust8.velocity = (Vector2) (dust8.velocity * 2f);
                    }
                }
                else if (this.type == 0x21)
                {
                    Main.PlaySound(2, (int) this.position.X, (int) this.position.Y, 10);
                    for (int num60 = 0; num60 < 20; num60++)
                    {
                        color = new Color();
                        int num61 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 0x1d, -this.velocity.X * 0.2f, -this.velocity.Y * 0.2f, 100, color, 2f);
                        Main.dust[num61].noGravity = true;
                        Dust dust9 = Main.dust[num61];
                        dust9.velocity = (Vector2) (dust9.velocity * 2f);
                        color = new Color();
                        num61 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 0x1d, -this.velocity.X * 0.2f, -this.velocity.Y * 0.2f, 100, color, 1f);
                        Dust dust10 = Main.dust[num61];
                        dust10.velocity = (Vector2) (dust10.velocity * 2f);
                    }
                }
                else if (((this.type == 0x1a) || (this.type == 0x1b)) || ((this.type == 0x1c) || (this.type == 0x1d)))
                {
                    if (this.life > 0)
                    {
                        for (int num62 = 0; num62 < ((dmg / ((double) this.lifeMax)) * 100.0); num62++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, (float) hitDirection, -1f, 0, color, 1f);
                        }
                    }
                    else
                    {
                        for (int num63 = 0; num63 < 50; num63++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 5, 2.5f * hitDirection, -2.5f, 0, color, 1f);
                        }
                        Gore.NewGore(this.position, this.velocity, 0x30);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 0x31);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 20f), this.velocity, 0x31);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 50);
                        Gore.NewGore(new Vector2(this.position.X, this.position.Y + 34f), this.velocity, 50);
                    }
                }
                else if (this.type == 30)
                {
                    Main.PlaySound(2, (int) this.position.X, (int) this.position.Y, 10);
                    for (int num64 = 0; num64 < 20; num64++)
                    {
                        color = new Color();
                        int num65 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 0x1b, -this.velocity.X * 0.2f, -this.velocity.Y * 0.2f, 100, color, 2f);
                        Main.dust[num65].noGravity = true;
                        Dust dust11 = Main.dust[num65];
                        dust11.velocity = (Vector2) (dust11.velocity * 2f);
                        color = new Color();
                        num65 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 0x1b, -this.velocity.X * 0.2f, -this.velocity.Y * 0.2f, 100, color, 1f);
                        Dust dust12 = Main.dust[num65];
                        dust12.velocity = (Vector2) (dust12.velocity * 2f);
                    }
                }
                else if (this.type == 0x2a)
                {
                    if (this.life > 0)
                    {
                        for (int num66 = 0; num66 < ((dmg / ((double) this.lifeMax)) * 100.0); num66++)
                        {
                            Dust.NewDust(this.position, this.width, this.height, 0x12, (float) hitDirection, -1f, this.alpha, this.color, this.scale);
                        }
                    }
                    else
                    {
                        for (int num67 = 0; num67 < 50; num67++)
                        {
                            Dust.NewDust(this.position, this.width, this.height, 0x12, (float) hitDirection, -2f, this.alpha, this.color, this.scale);
                        }
                        Gore.NewGore(this.position, this.velocity, 70);
                        Gore.NewGore(this.position, this.velocity, 0x47);
                    }
                }
                else if (this.type == 0x2b)
                {
                    if (this.life > 0)
                    {
                        for (int num68 = 0; num68 < ((dmg / ((double) this.lifeMax)) * 100.0); num68++)
                        {
                            Dust.NewDust(this.position, this.width, this.height, 40, (float) hitDirection, -1f, this.alpha, this.color, 1.2f);
                        }
                    }
                    else
                    {
                        for (int num69 = 0; num69 < 50; num69++)
                        {
                            Dust.NewDust(this.position, this.width, this.height, 40, (float) hitDirection, -2f, this.alpha, this.color, 1.2f);
                        }
                        Gore.NewGore(this.position, this.velocity, 0x48);
                        Gore.NewGore(this.position, this.velocity, 0x48);
                    }
                }
            }
        }

        public static int NewNPC(int X, int Y, int Type, int Start = 0)
        {
            int index = -1;
            for (int i = Start; i < 0x3e8; i++)
            {
                if (!Main.npc[i].active)
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                Main.npc[index] = new NPC();
                Main.npc[index].SetDefaults(Type);
                Main.npc[index].position.X = X - (Main.npc[index].width / 2);
                Main.npc[index].position.Y = Y - Main.npc[index].height;
                Main.npc[index].active = true;
                Main.npc[index].timeLeft = (int) (activeTime * 1.25);
                Main.npc[index].wet = Collision.WetCollision(Main.npc[index].position, Main.npc[index].width, Main.npc[index].height);
                return index;
            }
            return 0x3e8;
        }

        public void NPCLoot()
        {
            if ((this.type == 1) || (this.type == 0x10))
            {
                Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x17, Main.rand.Next(1, 3), false);
            }
            if ((this.type == 2) && (Main.rand.Next(3) == 0))
            {
                Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x26, 1, false);
            }
            if ((this.type == 3) && (Main.rand.Next(50) == 0))
            {
                Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0xd8, 1, false);
            }
            if (this.type == 4)
            {
                int stack = Main.rand.Next(30) + 20;
                Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x2f, stack, false);
                stack = Main.rand.Next(20) + 10;
                Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x38, stack, false);
                stack = Main.rand.Next(20) + 10;
                Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x38, stack, false);
                stack = Main.rand.Next(20) + 10;
                Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x38, stack, false);
                stack = Main.rand.Next(3) + 1;
                Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x3b, stack, false);
            }
            if ((this.type == 6) && (Main.rand.Next(3) == 0))
            {
                Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x44, 1, false);
            }
            if (((this.type == 7) || (this.type == 8)) || (this.type == 9))
            {
                if (Main.rand.Next(3) == 0)
                {
                    Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x44, Main.rand.Next(1, 3), false);
                }
                Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x45, Main.rand.Next(3, 9), false);
            }
            if ((((this.type == 10) || (this.type == 11)) || (this.type == 12)) && (Main.rand.Next(500) == 0))
            {
                Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0xd7, 1, false);
            }
            if (((this.type == 0x27) || (this.type == 40)) || (this.type == 0x29))
            {
                if (Main.rand.Next(100) == 0)
                {
                    Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 220, 1, false);
                }
                else if (Main.rand.Next(100) == 0)
                {
                    Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0xda, 1, false);
                }
            }
            if (((this.type == 13) || (this.type == 14)) || (this.type == 15))
            {
                int num2 = Main.rand.Next(1, 4);
                Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x56, num2, false);
                if (Main.rand.Next(2) == 0)
                {
                    num2 = Main.rand.Next(2, 6);
                    Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x38, num2, false);
                }
                if (this.boss)
                {
                    num2 = Main.rand.Next(15, 30);
                    Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x38, num2, false);
                    num2 = Main.rand.Next(15, 0x1f);
                    Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x38, num2, false);
                    int type = Main.rand.Next(100, 0x67);
                    Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, type, 1, false);
                }
            }
            if ((this.type == 0x15) && (Main.rand.Next(30) == 0))
            {
                Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x76, 1, false);
            }
            if (this.type == 0x17)
            {
                Main.rand.Next(3);
            }
            if ((this.type == 0x18) && (Main.rand.Next(50) == 0))
            {
                Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x70, 1, false);
            }
            if ((this.type == 0x1f) || (this.type == 0x20))
            {
                Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x9a, 1, false);
            }
            if (((this.type == 0x1a) || (this.type == 0x1b)) || ((this.type == 0x1c) || (this.type == 0x1d)))
            {
                if (Main.rand.Next(400) == 0)
                {
                    Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x80, 1, false);
                }
                else if (Main.rand.Next(200) == 0)
                {
                    Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 160, 1, false);
                }
                else if (Main.rand.Next(2) == 0)
                {
                    int num4 = Main.rand.Next(1, 6);
                    Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0xa1, num4, false);
                }
            }
            if (this.type == 0x2a)
            {
                Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0xd1, 1, false);
            }
            if ((this.type == 0x2b) && (Main.rand.Next(5) == 0))
            {
                Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 210, 1, false);
            }
            if (((this.type == 0x2a) || (this.type == 0x2b)) && (Main.rand.Next(150) == 0))
            {
                Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, Main.rand.Next(0xe4, 0xe7), 1, false);
            }
            if (this.boss)
            {
                if (this.type == 4)
                {
                    downedBoss1 = true;
                }
                if (((this.type == 13) || (this.type == 14)) || (this.type == 15))
                {
                    downedBoss2 = true;
                    this.name = "Eater of Worlds";
                }
                if (this.type == 0x23)
                {
                    downedBoss3 = true;
                    this.name = "Skeletron";
                }
                int num5 = Main.rand.Next(5, 0x10);
                Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x1c, num5, false);
                int num6 = Main.rand.Next(5) + 5;
                for (int i = 0; i < num6; i++)
                {
                    Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x3a, 1, false);
                }
                if (Main.netMode == 0)
                {
                    Main.NewText(this.name + " has been defeated!", 0xaf, 0x4b, 0xff);
                }
                else if (Main.netMode == 2)
                {
                    NetMessage.SendData(0x19, -1, -1, this.name + " has been defeated!", 8, 175f, 75f, 255f);
                }
            }
            if (Main.rand.Next(7) == 0)
            {
                if ((Main.rand.Next(2) == 0) && (Main.player[Player.FindClosest(this.position, this.width, this.height)].statMana < Main.player[Player.FindClosest(this.position, this.width, this.height)].statManaMax))
                {
                    Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0xb8, 1, false);
                }
                else if ((Main.rand.Next(2) == 0) && (Main.player[Player.FindClosest(this.position, this.width, this.height)].statLife < Main.player[Player.FindClosest(this.position, this.width, this.height)].statLifeMax))
                {
                    Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x3a, 1, false);
                }
            }
            float num8 = this.value * (1f + (Main.rand.Next(-20, 0x15) * 0.01f));
            if (Main.rand.Next(5) == 0)
            {
                num8 *= 1f + (Main.rand.Next(5, 11) * 0.01f);
            }
            if (Main.rand.Next(10) == 0)
            {
                num8 *= 1f + (Main.rand.Next(10, 0x15) * 0.01f);
            }
            if (Main.rand.Next(15) == 0)
            {
                num8 *= 1f + (Main.rand.Next(15, 0x1f) * 0.01f);
            }
            if (Main.rand.Next(20) == 0)
            {
                num8 *= 1f + (Main.rand.Next(20, 0x29) * 0.01f);
            }
            while (((int) num8) > 0)
            {
                if (num8 > 1000000f)
                {
                    int num9 = (int) (num8 / 1000000f);
                    if ((num9 > 50) && (Main.rand.Next(2) == 0))
                    {
                        num9 /= Main.rand.Next(3) + 1;
                    }
                    if (Main.rand.Next(2) == 0)
                    {
                        num9 /= Main.rand.Next(3) + 1;
                    }
                    num8 -= 0xf4240 * num9;
                    Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x4a, num9, false);
                }
                else
                {
                    if (num8 > 10000f)
                    {
                        int num10 = (int) (num8 / 10000f);
                        if ((num10 > 50) && (Main.rand.Next(2) == 0))
                        {
                            num10 /= Main.rand.Next(3) + 1;
                        }
                        if (Main.rand.Next(2) == 0)
                        {
                            num10 /= Main.rand.Next(3) + 1;
                        }
                        num8 -= 0x2710 * num10;
                        Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x49, num10, false);
                        continue;
                    }
                    if (num8 > 100f)
                    {
                        int num11 = (int) (num8 / 100f);
                        if ((num11 > 50) && (Main.rand.Next(2) == 0))
                        {
                            num11 /= Main.rand.Next(3) + 1;
                        }
                        if (Main.rand.Next(2) == 0)
                        {
                            num11 /= Main.rand.Next(3) + 1;
                        }
                        num8 -= 100 * num11;
                        Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x48, num11, false);
                        continue;
                    }
                    int num12 = (int) num8;
                    if ((num12 > 50) && (Main.rand.Next(2) == 0))
                    {
                        num12 /= Main.rand.Next(3) + 1;
                    }
                    if (Main.rand.Next(2) == 0)
                    {
                        num12 /= Main.rand.Next(4) + 1;
                    }
                    if (num12 < 1)
                    {
                        num12 = 1;
                    }
                    num8 -= num12;
                    Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x47, num12, false);
                }
            }
        }

        public void SetDefaults(int Type)
        {
            this.lavaWet = false;
            this.wetCount = 0;
            this.wet = false;
            this.townNPC = false;
            this.homeless = false;
            this.homeTileX = -1;
            this.homeTileY = -1;
            this.friendly = false;
            this.behindTiles = false;
            this.boss = false;
            this.noTileCollide = false;
            this.rotation = 0f;
            this.active = true;
            this.alpha = 0;
            this.color = new Color();
            this.collideX = false;
            this.collideY = false;
            this.direction = 0;
            this.oldDirection = this.direction;
            this.frameCounter = 0.0;
            this.netUpdate = false;
            this.knockBackResist = 1f;
            this.name = "";
            this.noGravity = false;
            this.scale = 1f;
            this.soundHit = 0;
            this.soundKilled = 0;
            this.spriteDirection = -1;
            this.target = 8;
            this.oldTarget = this.target;
            this.targetRect = new Rectangle();
            this.timeLeft = activeTime;
            this.type = Type;
            this.value = 0f;
            for (int i = 0; i < maxAI; i++)
            {
                this.ai[i] = 0f;
            }
            if (this.type == 1)
            {
                this.name = "Blue Slime";
                this.width = 0x18;
                this.height = 0x12;
                this.aiStyle = 1;
                this.damage = 7;
                this.defense = 2;
                this.lifeMax = 0x19;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.alpha = 0xaf;
                this.color = new Color(0, 80, 0xff, 100);
                this.value = 25f;
            }
            if (this.type == 2)
            {
                this.name = "Demon Eye";
                this.width = 30;
                this.height = 0x20;
                this.aiStyle = 2;
                this.damage = 0x12;
                this.defense = 2;
                this.lifeMax = 60;
                this.soundHit = 1;
                this.knockBackResist = 0.8f;
                this.soundKilled = 1;
                this.value = 75f;
            }
            if (this.type == 3)
            {
                this.name = "Zombie";
                this.width = 0x12;
                this.height = 40;
                this.aiStyle = 3;
                this.damage = 14;
                this.defense = 6;
                this.lifeMax = 0x2d;
                this.soundHit = 1;
                this.soundKilled = 2;
                this.knockBackResist = 0.5f;
                this.value = 60f;
            }
            if (this.type == 4)
            {
                this.name = "Eye of Cthulhu";
                this.width = 100;
                this.height = 110;
                this.aiStyle = 4;
                this.damage = 0x12;
                this.defense = 12;
                this.lifeMax = 0xbb8;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.knockBackResist = 0f;
                this.noGravity = true;
                this.noTileCollide = true;
                this.timeLeft = activeTime * 30;
                this.boss = true;
                this.value = 30000f;
            }
            if (this.type == 5)
            {
                this.name = "Servant of Cthulhu";
                this.width = 20;
                this.height = 20;
                this.aiStyle = 5;
                this.damage = 0x17;
                this.defense = 0;
                this.lifeMax = 8;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.noGravity = true;
                this.noTileCollide = true;
            }
            if (this.type == 6)
            {
                this.name = "Eater of Souls";
                this.width = 30;
                this.height = 30;
                this.aiStyle = 5;
                this.damage = 15;
                this.defense = 8;
                this.lifeMax = 40;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.noGravity = true;
                this.knockBackResist = 0.5f;
                this.value = 90f;
            }
            if (this.type == 7)
            {
                this.name = "Devourer Head";
                this.width = 0x16;
                this.height = 0x16;
                this.aiStyle = 6;
                this.damage = 0x1c;
                this.defense = 2;
                this.lifeMax = 40;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.noGravity = true;
                this.noTileCollide = true;
                this.knockBackResist = 0f;
                this.behindTiles = true;
                this.value = 300f;
            }
            if (this.type == 8)
            {
                this.name = "Devourer Body";
                this.width = 0x16;
                this.height = 0x16;
                this.aiStyle = 6;
                this.damage = 0x12;
                this.defense = 6;
                this.lifeMax = 60;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.noGravity = true;
                this.noTileCollide = true;
                this.knockBackResist = 0f;
                this.behindTiles = true;
                this.value = 300f;
            }
            if (this.type == 9)
            {
                this.name = "Devourer Tail";
                this.width = 0x16;
                this.height = 0x16;
                this.aiStyle = 6;
                this.damage = 13;
                this.defense = 10;
                this.lifeMax = 100;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.noGravity = true;
                this.noTileCollide = true;
                this.knockBackResist = 0f;
                this.behindTiles = true;
                this.value = 300f;
            }
            if (this.type == 10)
            {
                this.name = "Giant Worm Head";
                this.width = 14;
                this.height = 14;
                this.aiStyle = 6;
                this.damage = 8;
                this.defense = 0;
                this.lifeMax = 10;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.noGravity = true;
                this.noTileCollide = true;
                this.knockBackResist = 0f;
                this.behindTiles = true;
                this.value = 200f;
            }
            if (this.type == 11)
            {
                this.name = "Giant Worm Body";
                this.width = 14;
                this.height = 14;
                this.aiStyle = 6;
                this.damage = 4;
                this.defense = 4;
                this.lifeMax = 15;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.noGravity = true;
                this.noTileCollide = true;
                this.knockBackResist = 0f;
                this.behindTiles = true;
                this.value = 200f;
            }
            if (this.type == 12)
            {
                this.name = "Giant Worm Tail";
                this.width = 14;
                this.height = 14;
                this.aiStyle = 6;
                this.damage = 4;
                this.defense = 6;
                this.lifeMax = 20;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.noGravity = true;
                this.noTileCollide = true;
                this.knockBackResist = 0f;
                this.behindTiles = true;
                this.value = 200f;
            }
            if (this.type == 13)
            {
                this.name = "Eater of Worlds Head";
                this.width = 0x26;
                this.height = 0x26;
                this.aiStyle = 6;
                this.damage = 40;
                this.defense = 0;
                this.lifeMax = 120;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.noGravity = true;
                this.noTileCollide = true;
                this.knockBackResist = 0f;
                this.behindTiles = true;
                this.value = 300f;
            }
            if (this.type == 14)
            {
                this.name = "Eater of Worlds Body";
                this.width = 0x26;
                this.height = 0x26;
                this.aiStyle = 6;
                this.damage = 15;
                this.defense = 4;
                this.lifeMax = 200;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.noGravity = true;
                this.noTileCollide = true;
                this.knockBackResist = 0f;
                this.behindTiles = true;
                this.value = 300f;
            }
            if (this.type == 15)
            {
                this.name = "Eater of Worlds Tail";
                this.width = 0x26;
                this.height = 0x26;
                this.aiStyle = 6;
                this.damage = 10;
                this.defense = 8;
                this.lifeMax = 300;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.noGravity = true;
                this.noTileCollide = true;
                this.knockBackResist = 0f;
                this.behindTiles = true;
                this.value = 300f;
            }
            if (this.type == 0x10)
            {
                this.name = "Mother Slime";
                this.width = 0x24;
                this.height = 0x18;
                this.aiStyle = 1;
                this.damage = 20;
                this.defense = 7;
                this.lifeMax = 90;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.alpha = 120;
                this.color = new Color(0, 0, 0, 50);
                this.value = 75f;
                this.scale = 1.25f;
                this.knockBackResist = 0.6f;
            }
            if (this.type == 0x11)
            {
                this.townNPC = true;
                this.friendly = true;
                this.name = "Merchant";
                this.width = 0x12;
                this.height = 40;
                this.aiStyle = 7;
                this.damage = 10;
                this.defense = 15;
                this.lifeMax = 250;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.knockBackResist = 0.5f;
            }
            if (this.type == 0x12)
            {
                this.townNPC = true;
                this.friendly = true;
                this.name = "Nurse";
                this.width = 0x12;
                this.height = 40;
                this.aiStyle = 7;
                this.damage = 10;
                this.defense = 15;
                this.lifeMax = 250;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.knockBackResist = 0.5f;
            }
            if (this.type == 0x13)
            {
                this.townNPC = true;
                this.friendly = true;
                this.name = "Arms Dealer";
                this.width = 0x12;
                this.height = 40;
                this.aiStyle = 7;
                this.damage = 10;
                this.defense = 15;
                this.lifeMax = 250;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.knockBackResist = 0.5f;
            }
            if (this.type == 20)
            {
                this.townNPC = true;
                this.friendly = true;
                this.name = "Dryad";
                this.width = 0x12;
                this.height = 40;
                this.aiStyle = 7;
                this.damage = 10;
                this.defense = 15;
                this.lifeMax = 250;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.knockBackResist = 0.5f;
            }
            if (this.type == 0x15)
            {
                this.name = "Skeleton";
                this.width = 0x12;
                this.height = 40;
                this.aiStyle = 3;
                this.damage = 20;
                this.defense = 8;
                this.lifeMax = 60;
                this.soundHit = 2;
                this.soundKilled = 2;
                this.knockBackResist = 0.5f;
                this.value = 250f;
            }
            if (this.type == 0x16)
            {
                this.townNPC = true;
                if (ShankShock.killGuide)
                {
                    this.friendly = false;
                    this.damage = 0;
                    this.defense = 0;
                    this.lifeMax = 1;
                    this.name = "(Stupid) Guide";
                }
                else
                {
                    this.friendly = true;
                    this.damage = 10;
                    this.defense = 100;
                    this.lifeMax = 250;
                    this.name = "Guide";
                }
                this.width = 0x12;
                this.height = 40;
                this.aiStyle = 7;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.knockBackResist = 0.5f;
            }
            if (this.type == 0x17)
            {
                this.name = "Meteor Head";
                this.width = 0x16;
                this.height = 0x16;
                this.aiStyle = 5;
                this.damage = 0x19;
                this.defense = 10;
                this.lifeMax = 50;
                this.soundHit = 3;
                this.soundKilled = 3;
                this.noGravity = true;
                this.noTileCollide = true;
                this.value = 300f;
                this.knockBackResist = 0.8f;
            }
            else if (this.type == 0x18)
            {
                this.name = "Fire Imp";
                this.width = 0x12;
                this.height = 40;
                this.aiStyle = 8;
                this.damage = 30;
                this.defense = 20;
                this.lifeMax = 80;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.knockBackResist = 0.5f;
                this.value = 800f;
            }
            if (this.type == 0x19)
            {
                this.name = "Burning Sphere";
                this.width = 0x10;
                this.height = 0x10;
                this.aiStyle = 9;
                this.damage = 0x19;
                this.defense = 0;
                this.lifeMax = 1;
                this.soundHit = 3;
                this.soundKilled = 3;
                this.noGravity = true;
                this.noTileCollide = true;
                this.knockBackResist = 0f;
                this.alpha = 100;
            }
            if (this.type == 0x1a)
            {
                this.name = "Goblin Peon";
                this.width = 0x12;
                this.height = 40;
                this.aiStyle = 3;
                this.damage = 12;
                this.defense = 4;
                this.lifeMax = 60;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.knockBackResist = 0.8f;
                this.value = 250f;
            }
            if (this.type == 0x1b)
            {
                this.name = "Goblin Thief";
                this.width = 0x12;
                this.height = 40;
                this.aiStyle = 3;
                this.damage = 20;
                this.defense = 6;
                this.lifeMax = 80;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.knockBackResist = 0.7f;
                this.value = 600f;
            }
            if (this.type == 0x1c)
            {
                this.name = "Goblin Warrior";
                this.width = 0x12;
                this.height = 40;
                this.aiStyle = 3;
                this.damage = 0x19;
                this.defense = 8;
                this.lifeMax = 110;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.knockBackResist = 0.5f;
                this.value = 500f;
            }
            else if (this.type == 0x1d)
            {
                this.name = "Goblin Sorcerer";
                this.width = 0x12;
                this.height = 40;
                this.aiStyle = 8;
                this.damage = 20;
                this.defense = 2;
                this.lifeMax = 40;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.knockBackResist = 0.6f;
                this.value = 800f;
            }
            else if (this.type == 30)
            {
                this.name = "Chaos Ball";
                this.width = 0x10;
                this.height = 0x10;
                this.aiStyle = 9;
                this.damage = 20;
                this.defense = 0;
                this.lifeMax = 1;
                this.soundHit = 3;
                this.soundKilled = 3;
                this.noGravity = true;
                this.noTileCollide = true;
                this.alpha = 100;
                this.knockBackResist = 0f;
            }
            else if (this.type == 0x1f)
            {
                this.name = "Angry Bones";
                this.width = 0x12;
                this.height = 40;
                this.aiStyle = 3;
                this.damage = 30;
                this.defense = 10;
                this.lifeMax = 100;
                this.soundHit = 2;
                this.soundKilled = 2;
                this.knockBackResist = 0.7f;
                this.value = 500f;
            }
            else if (this.type == 0x20)
            {
                this.name = "Dark Caster";
                this.width = 0x12;
                this.height = 40;
                this.aiStyle = 8;
                this.damage = 20;
                this.defense = 4;
                this.lifeMax = 50;
                this.soundHit = 2;
                this.soundKilled = 2;
                this.knockBackResist = 0.6f;
                this.value = 800f;
            }
            else if (this.type == 0x21)
            {
                this.name = "Water Sphere";
                this.width = 0x10;
                this.height = 0x10;
                this.aiStyle = 9;
                this.damage = 20;
                this.defense = 0;
                this.lifeMax = 1;
                this.soundHit = 3;
                this.soundKilled = 3;
                this.noGravity = true;
                this.noTileCollide = true;
                this.alpha = 100;
                this.knockBackResist = 0f;
            }
            if (this.type == 0x22)
            {
                this.name = "Burning Skull";
                this.width = 0x1a;
                this.height = 0x1c;
                this.aiStyle = 10;
                this.damage = 0x19;
                this.defense = 30;
                this.lifeMax = 30;
                this.soundHit = 2;
                this.soundKilled = 2;
                this.noGravity = true;
                this.value = 300f;
                this.knockBackResist = 1.2f;
            }
            if (this.type == 0x23)
            {
                this.name = "Skeletron Head";
                this.width = 80;
                this.height = 0x66;
                this.aiStyle = 11;
                this.damage = 0x23;
                this.defense = 12;
                this.lifeMax = 0x1770;
                this.soundHit = 2;
                this.soundKilled = 2;
                this.noGravity = true;
                this.noTileCollide = true;
                this.value = 50000f;
                this.knockBackResist = 0f;
                this.boss = true;
            }
            if (this.type == 0x24)
            {
                this.name = "Skeletron Hand";
                this.width = 0x34;
                this.height = 0x34;
                this.aiStyle = 12;
                this.damage = 30;
                this.defense = 0x12;
                this.lifeMax = 0x4b0;
                this.soundHit = 2;
                this.soundKilled = 2;
                this.noGravity = true;
                this.noTileCollide = true;
                this.knockBackResist = 0f;
            }
            if (this.type == 0x25)
            {
                this.townNPC = true;
                this.friendly = true;
                this.name = "Old Man";
                this.width = 0x12;
                this.height = 40;
                this.aiStyle = 7;
                this.damage = 10;
                this.defense = 100;
                this.lifeMax = 250;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.knockBackResist = 0.5f;
            }
            if (this.type == 0x26)
            {
                this.townNPC = true;
                this.friendly = true;
                this.name = "Demolitionist";
                this.width = 0x12;
                this.height = 40;
                this.aiStyle = 7;
                this.damage = 10;
                this.defense = 15;
                this.lifeMax = 250;
                this.soundHit = 1;
                this.soundKilled = 1;
                this.knockBackResist = 0.5f;
            }
            if (this.type == 0x27)
            {
                this.name = "Bone Serpent Head";
                this.width = 0x16;
                this.height = 0x16;
                this.aiStyle = 6;
                this.damage = 40;
                this.defense = 10;
                this.lifeMax = 120;
                this.soundHit = 2;
                this.soundKilled = 2;
                this.noGravity = true;
                this.noTileCollide = true;
                this.knockBackResist = 0f;
                this.behindTiles = true;
                this.value = 1000f;
            }
            if (this.type == 40)
            {
                this.name = "Bone Serpent Body";
                this.width = 0x16;
                this.height = 0x16;
                this.aiStyle = 6;
                this.damage = 30;
                this.defense = 12;
                this.lifeMax = 150;
                this.soundHit = 2;
                this.soundKilled = 2;
                this.noGravity = true;
                this.noTileCollide = true;
                this.knockBackResist = 0f;
                this.behindTiles = true;
                this.value = 1000f;
            }
            if (this.type == 0x29)
            {
                this.name = "Bone Serpent Tail";
                this.width = 0x16;
                this.height = 0x16;
                this.aiStyle = 6;
                this.damage = 20;
                this.defense = 0x12;
                this.lifeMax = 200;
                this.soundHit = 2;
                this.soundKilled = 2;
                this.noGravity = true;
                this.noTileCollide = true;
                this.knockBackResist = 0f;
                this.behindTiles = true;
                this.value = 1000f;
            }
            if (this.type == 0x2a)
            {
                this.name = "Hornet";
                this.width = 0x22;
                this.height = 0x20;
                this.aiStyle = 2;
                this.damage = 40;
                this.defense = 14;
                this.lifeMax = 100;
                this.soundHit = 1;
                this.knockBackResist = 0.8f;
                this.soundKilled = 1;
                this.value = 750f;
            }
            if (this.type == 0x2b)
            {
                this.noGravity = true;
                this.name = "Man Eater";
                this.width = 30;
                this.height = 30;
                this.aiStyle = 13;
                this.damage = 60;
                this.defense = 0x12;
                this.lifeMax = 200;
                this.soundHit = 1;
                this.knockBackResist = 0.7f;
                this.soundKilled = 1;
                this.value = 750f;
            }
            this.frame = new Rectangle(0, 0, Main.npcTexture[this.type].Width, Main.npcTexture[this.type].Height / Main.npcFrameCount[this.type]);
            this.width = (int) (this.width * this.scale);
            this.height = (int) (this.height * this.scale);
            this.life = this.lifeMax;
            if (Main.dumbAI)
            {
                this.aiStyle = 0;
            }
        }

        public void SetDefaults(string Name)
        {
            this.SetDefaults(0);
            if (Name == "Green Slime")
            {
                this.SetDefaults(1);
                this.name = Name;
                this.scale = 0.9f;
                this.damage = 8;
                this.defense = 2;
                this.life = 15;
                this.knockBackResist = 1.1f;
                this.color = new Color(0, 220, 40, 100);
                this.value = 3f;
            }
            else if (Name == "Pinky")
            {
                this.SetDefaults(1);
                this.name = Name;
                this.scale = 0.6f;
                this.damage = 5;
                this.defense = 5;
                this.life = 150;
                this.knockBackResist = 1.4f;
                this.color = new Color(250, 30, 90, 90);
                this.value = 10000f;
            }
            else if (Name == "Baby Slime")
            {
                this.SetDefaults(1);
                this.name = Name;
                this.scale = 0.9f;
                this.damage = 13;
                this.defense = 4;
                this.life = 30;
                this.knockBackResist = 0.95f;
                this.alpha = 120;
                this.color = new Color(0, 0, 0, 50);
                this.value = 10f;
            }
            else if (Name == "Black Slime")
            {
                this.SetDefaults(1);
                this.name = Name;
                this.damage = 15;
                this.defense = 4;
                this.life = 0x2d;
                this.color = new Color(0, 0, 0, 50);
                this.value = 20f;
            }
            else if (Name == "Purple Slime")
            {
                this.SetDefaults(1);
                this.name = Name;
                this.scale = 1.2f;
                this.damage = 12;
                this.defense = 6;
                this.life = 40;
                this.knockBackResist = 0.9f;
                this.color = new Color(200, 0, 0xff, 150);
                this.value = 10f;
            }
            else if (Name == "Red Slime")
            {
                this.SetDefaults(1);
                this.name = Name;
                this.damage = 12;
                this.defense = 4;
                this.life = 0x23;
                this.color = new Color(0xff, 30, 0, 100);
                this.value = 8f;
            }
            else if (Name == "Yellow Slime")
            {
                this.SetDefaults(1);
                this.name = Name;
                this.scale = 1.2f;
                this.damage = 15;
                this.defense = 7;
                this.life = 0x2d;
                this.color = new Color(0xff, 0xff, 0, 100);
                this.value = 10f;
            }
            else if (Name != "")
            {
                for (int i = 1; i < 0x2c; i++)
                {
                    this.SetDefaults(i);
                    if (this.name == Name)
                    {
                        break;
                    }
                    if (i == 0x2b)
                    {
                        this.SetDefaults(0);
                        this.active = false;
                    }
                }
            }
            else
            {
                this.active = false;
            }
            this.lifeMax = this.life;
        }

        public static void SpawnNPC()
        {
            if (!Main.stopSpawns)
            {
                bool flag = false;
                int num = 0;
                int num2 = 0;
                int num3 = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (Main.player[i].active)
                    {
                        num3++;
                    }
                }
                for (int j = 0; j < 8; j++)
                {
                    bool flag2 = false;
                    if (((Main.player[j].active && (Main.invasionType > 0)) && ((Main.invasionDelay == 0) && (Main.invasionSize > 0))) && (Main.player[j].position.Y < ((Main.worldSurface * 16.0) + Main.screenHeight)))
                    {
                        int num6 = 0xbb8;
                        if ((Main.player[j].position.X > ((Main.invasionX * 16.0) - num6)) && (Main.player[j].position.X < ((Main.invasionX * 16.0) + num6)))
                        {
                            flag2 = true;
                        }
                    }
                    flag = false;
                    spawnRate = ShankShock.defaultSpawnRate;
                    maxSpawns = ShankShock.defaultMaxSpawns;
                    if (Main.player[j].position.Y > ((Main.maxTilesY - 200) * 0x10))
                    {
                        spawnRate = (int) (spawnRate * 1.5f);
                        maxSpawns = (int) (maxSpawns * 0.5f);
                    }
                    else if (Main.player[j].position.Y > ((Main.rockLayer * 16.0) + Main.screenHeight))
                    {
                        spawnRate = (int) (spawnRate * 0.7);
                        maxSpawns = (int) (maxSpawns * 1.35f);
                    }
                    else if (Main.player[j].position.Y > ((Main.worldSurface * 16.0) + Main.screenHeight))
                    {
                        spawnRate = (int) (spawnRate * 0.8);
                        maxSpawns = (int) (maxSpawns * 1.1f);
                    }
                    else if (!Main.dayTime)
                    {
                        spawnRate = (int) (spawnRate * 0.6);
                        maxSpawns = (int) (maxSpawns * 1.3f);
                        if (Main.bloodMoon)
                        {
                            spawnRate = (int) (spawnRate * 0.3);
                            maxSpawns = (int) (maxSpawns * 1.8f);
                        }
                    }
                    if (Main.player[j].zoneDungeon)
                    {
                        spawnRate = (int) (defaultSpawnRate * 0.1);
                        maxSpawns = (int) (defaultMaxSpawns * 2.1);
                    }
                    else if (Main.player[j].zoneEvil)
                    {
                        spawnRate = (int) (spawnRate * 0.5);
                        maxSpawns = (int) (maxSpawns * 1.4f);
                    }
                    else if (Main.player[j].zoneMeteor)
                    {
                        spawnRate = (int) (spawnRate * 0.5);
                    }
                    else if (Main.player[j].zoneJungle)
                    {
                        spawnRate = (int) (spawnRate * 0.3);
                        maxSpawns = (int) (maxSpawns * 1.6f);
                    }
                    if (spawnRate < (defaultSpawnRate * 0.1))
                    {
                        spawnRate = (int) (defaultSpawnRate * 0.1);
                    }
                    if (maxSpawns > (defaultMaxSpawns * 2.5))
                    {
                        maxSpawns = (int) (defaultMaxSpawns * 2.5);
                    }
                    if (Main.player[j].inventory[Main.player[j].selectedItem].type == 0x31)
                    {
                        spawnRate = (int) (spawnRate * 0.75);
                        maxSpawns = (int) (maxSpawns * 1.5f);
                    }
                    if (flag2)
                    {
                        maxSpawns = (int) (defaultMaxSpawns * (1.0 + (0.4 * num3)));
                        spawnRate = 30;
                    }
                    if ((!flag2 && (!Main.bloodMoon || Main.dayTime)) && ((!Main.player[j].zoneDungeon && !Main.player[j].zoneEvil) && !Main.player[j].zoneMeteor))
                    {
                        if (Main.player[j].townNPCs == 1)
                        {
                            maxSpawns = (int) (maxSpawns * 0.6);
                            spawnRate = (int) (spawnRate * 2f);
                        }
                        else if (Main.player[j].townNPCs == 2)
                        {
                            maxSpawns = (int) (maxSpawns * 0.3);
                            spawnRate = (int) (spawnRate * 3f);
                        }
                        else if (Main.player[j].townNPCs >= 3)
                        {
                            maxSpawns = 0;
                            spawnRate = 0x1869f;
                        }
                    }
                    if ((Main.player[j].active && !Main.player[j].dead) && ((Main.player[j].activeNPCs < maxSpawns) && (Main.rand.Next(spawnRate) == 0)))
                    {
                        int minValue = ((int) (Main.player[j].position.X / 16f)) - spawnRangeX;
                        int maxValue = ((int) (Main.player[j].position.X / 16f)) + spawnRangeX;
                        int num9 = ((int) (Main.player[j].position.Y / 16f)) - spawnRangeY;
                        int maxTilesY = ((int) (Main.player[j].position.Y / 16f)) + spawnRangeY;
                        int num11 = ((int) (Main.player[j].position.X / 16f)) - safeRangeX;
                        int num12 = ((int) (Main.player[j].position.X / 16f)) + safeRangeX;
                        int num13 = ((int) (Main.player[j].position.Y / 16f)) - safeRangeY;
                        int num14 = ((int) (Main.player[j].position.Y / 16f)) + safeRangeY;
                        if (minValue < 0)
                        {
                            minValue = 0;
                        }
                        if (maxValue > Main.maxTilesX)
                        {
                            maxValue = Main.maxTilesX;
                        }
                        if (num9 < 0)
                        {
                            num9 = 0;
                        }
                        if (maxTilesY > Main.maxTilesY)
                        {
                            maxTilesY = Main.maxTilesY;
                        }
                        for (int k = 0; k < 50; k++)
                        {
                            int num16 = Main.rand.Next(minValue, maxValue);
                            int num17 = Main.rand.Next(num9, maxTilesY);
                            if (!Main.tile[num16, num17].active || !Main.tileSolid[Main.tile[num16, num17].type])
                            {
                                if (Main.wallHouse[Main.tile[num16, num17].wall])
                                {
                                    goto Label_0846;
                                }
                                for (int m = num17; m < Main.maxTilesY; m++)
                                {
                                    if (Main.tile[num16, m].active && Main.tileSolid[Main.tile[num16, m].type])
                                    {
                                        if (((num16 < num11) || (num16 > num12)) || ((m < num13) || (m > num14)))
                                        {
                                            byte num1 = Main.tile[num16, m].type;
                                            num = num16;
                                            num2 = m;
                                            flag = true;
                                        }
                                        break;
                                    }
                                }
                                if (flag)
                                {
                                    int num19 = num - (spawnSpaceX / 2);
                                    int num20 = num + (spawnSpaceX / 2);
                                    int num21 = num2 - spawnSpaceY;
                                    int num22 = num2;
                                    if (num19 < 0)
                                    {
                                        flag = false;
                                    }
                                    if (num20 > Main.maxTilesX)
                                    {
                                        flag = false;
                                    }
                                    if (num21 < 0)
                                    {
                                        flag = false;
                                    }
                                    if (num22 > Main.maxTilesY)
                                    {
                                        flag = false;
                                    }
                                    if (flag)
                                    {
                                        for (int n = num19; n < num20; n++)
                                        {
                                            for (int num24 = num21; num24 < num22; num24++)
                                            {
                                                if (Main.tile[n, num24].active && Main.tileSolid[Main.tile[n, num24].type])
                                                {
                                                    flag = false;
                                                    break;
                                                }
                                                if (Main.tile[n, num24].lava && (num24 < (Main.maxTilesY - 200)))
                                                {
                                                    flag = false;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (flag || flag)
                            {
                                break;
                            }
                        Label_0846:;
                        }
                    }
                    if (flag)
                    {
                        Rectangle rectangle = new Rectangle(num * 0x10, num2 * 0x10, 0x10, 0x10);
                        for (int num25 = 0; num25 < 8; num25++)
                        {
                            if (Main.player[num25].active)
                            {
                                Rectangle rectangle2 = new Rectangle(((((int) Main.player[num25].position.X) + (Main.player[num25].width / 2)) - (Main.screenWidth / 2)) - safeRangeX, ((((int) Main.player[num25].position.Y) + (Main.player[num25].height / 2)) - (Main.screenHeight / 2)) - safeRangeY, Main.screenWidth + (safeRangeX * 2), Main.screenHeight + (safeRangeY * 2));
                                if (rectangle.Intersects(rectangle2))
                                {
                                    flag = false;
                                }
                            }
                        }
                    }
                    if ((flag && Main.player[j].zoneDungeon) && (!Main.tileDungeon[Main.tile[num, num2].type] || (Main.tile[num, num2 - 1].wall == 0)))
                    {
                        flag = false;
                    }
                    if (!flag)
                    {
                        continue;
                    }
                    flag = false;
                    int type = Main.tile[num, num2].type;
                    int index = 0x3e8;
                    if (flag2)
                    {
                        if (Main.rand.Next(9) == 0)
                        {
                            NewNPC((num * 0x10) + 8, num2 * 0x10, 0x1d, 0);
                        }
                        else if (Main.rand.Next(5) == 0)
                        {
                            NewNPC((num * 0x10) + 8, num2 * 0x10, 0x1a, 0);
                        }
                        else if (Main.rand.Next(3) == 0)
                        {
                            NewNPC((num * 0x10) + 8, num2 * 0x10, 0x1b, 0);
                        }
                        else
                        {
                            NewNPC((num * 0x10) + 8, num2 * 0x10, 0x1c, 0);
                        }
                    }
                    else if (Main.player[j].zoneDungeon)
                    {
                        if (!downedBoss3)
                        {
                            index = NewNPC((num * 0x10) + 8, num2 * 0x10, 0x23, 0);
                            Main.npc[index].ai[0] = 1f;
                            Main.npc[index].ai[2] = 2f;
                        }
                        else if (Main.rand.Next(4) == 0)
                        {
                            index = NewNPC((num * 0x10) + 8, num2 * 0x10, 0x22, 0);
                        }
                        else if (Main.rand.Next(5) == 0)
                        {
                            index = NewNPC((num * 0x10) + 8, num2 * 0x10, 0x20, 0);
                        }
                        else
                        {
                            index = NewNPC((num * 0x10) + 8, num2 * 0x10, 0x1f, 0);
                        }
                    }
                    else if (Main.player[j].zoneMeteor)
                    {
                        index = NewNPC((num * 0x10) + 8, num2 * 0x10, 0x17, 0);
                    }
                    else if (Main.player[j].zoneEvil && (Main.rand.Next(50) == 0))
                    {
                        index = NewNPC((num * 0x10) + 8, num2 * 0x10, 7, 1);
                    }
                    else if (type == 60)
                    {
                        if (Main.rand.Next(3) == 0)
                        {
                            index = NewNPC((num * 0x10) + 8, num2 * 0x10, 0x2b, 0);
                            Main.npc[index].ai[0] = num;
                            Main.npc[index].ai[1] = num2;
                            Main.npc[index].netUpdate = true;
                        }
                        else
                        {
                            index = NewNPC((num * 0x10) + 8, num2 * 0x10, 0x2a, 0);
                        }
                    }
                    else if (num2 <= Main.worldSurface)
                    {
                        switch (type)
                        {
                            case 0x17:
                            case 0x19:
                                index = NewNPC((num * 0x10) + 8, num2 * 0x10, 6, 0);
                                goto Label_0E3E;
                        }
                        if (Main.dayTime)
                        {
                            int num28 = Math.Abs((int) (num - Main.spawnTileX));
                            index = NewNPC((num * 0x10) + 8, num2 * 0x10, 1, 0);
                            if ((Main.rand.Next(3) == 0) || (num28 < 200))
                            {
                                Main.npc[index].SetDefaults("Green Slime");
                            }
                            else if ((Main.rand.Next(10) == 0) && (num28 > 400))
                            {
                                Main.npc[index].SetDefaults("Purple Slime");
                            }
                        }
                        else if ((Main.rand.Next(6) == 0) || ((Main.moonPhase == 4) && (Main.rand.Next(2) == 0)))
                        {
                            index = NewNPC((num * 0x10) + 8, num2 * 0x10, 2, 0);
                        }
                        else
                        {
                            NewNPC((num * 0x10) + 8, num2 * 0x10, 3, 0);
                        }
                    }
                    else if (num2 <= Main.rockLayer)
                    {
                        if (Main.rand.Next(30) == 0)
                        {
                            index = NewNPC((num * 0x10) + 8, num2 * 0x10, 10, 1);
                        }
                        else
                        {
                            index = NewNPC((num * 0x10) + 8, num2 * 0x10, 1, 0);
                            if (Main.rand.Next(5) == 0)
                            {
                                Main.npc[index].SetDefaults("Yellow Slime");
                            }
                            else if (Main.rand.Next(2) == 0)
                            {
                                Main.npc[index].SetDefaults("Blue Slime");
                            }
                            else
                            {
                                Main.npc[index].SetDefaults("Red Slime");
                            }
                        }
                    }
                    else if (num2 > (Main.maxTilesY - 190))
                    {
                        if (Main.rand.Next(5) == 0)
                        {
                            index = NewNPC((num * 0x10) + 8, num2 * 0x10, 0x27, 1);
                        }
                        else
                        {
                            index = NewNPC((num * 0x10) + 8, num2 * 0x10, 0x18, 0);
                        }
                    }
                    else if (Main.rand.Next(0x23) == 0)
                    {
                        index = NewNPC((num * 0x10) + 8, num2 * 0x10, 10, 1);
                    }
                    else if (Main.rand.Next(5) == 0)
                    {
                        index = NewNPC((num * 0x10) + 8, num2 * 0x10, 0x10, 0);
                    }
                    else if (Main.rand.Next(2) == 0)
                    {
                        index = NewNPC((num * 0x10) + 8, num2 * 0x10, 0x15, 0);
                    }
                    else
                    {
                        index = NewNPC((num * 0x10) + 8, num2 * 0x10, 1, 0);
                        Main.npc[index].SetDefaults("Black Slime");
                    }
                Label_0E3E:
                    if ((Main.npc[index].type == 1) && (Main.rand.Next(250) == 0))
                    {
                        Main.npc[index].SetDefaults("Pinky");
                    }
                    if ((Main.netMode != 2) || (index >= 0x3e8))
                    {
                        break;
                    }
                    NetMessage.SendData(0x17, -1, -1, "", index, 0f, 0f, 0f);
                    return;
                }
            }
        }

        public static void SpawnOnPlayer(int plr, int Type)
        {
            bool flag = false;
            int num = 0;
            int num2 = 0;
            int minValue = ((int) (Main.player[plr].position.X / 16f)) - (spawnRangeX * 3);
            int maxValue = ((int) (Main.player[plr].position.X / 16f)) + (spawnRangeX * 3);
            int num5 = ((int) (Main.player[plr].position.Y / 16f)) - (spawnRangeY * 3);
            int maxTilesY = ((int) (Main.player[plr].position.Y / 16f)) + (spawnRangeY * 3);
            int num7 = ((int) (Main.player[plr].position.X / 16f)) - safeRangeX;
            int num8 = ((int) (Main.player[plr].position.X / 16f)) + safeRangeX;
            int num9 = ((int) (Main.player[plr].position.Y / 16f)) - safeRangeY;
            int num10 = ((int) (Main.player[plr].position.Y / 16f)) + safeRangeY;
            if (minValue < 0)
            {
                minValue = 0;
            }
            if (maxValue > Main.maxTilesX)
            {
                maxValue = Main.maxTilesX;
            }
            if (num5 < 0)
            {
                num5 = 0;
            }
            if (maxTilesY > Main.maxTilesY)
            {
                maxTilesY = Main.maxTilesY;
            }
            for (int i = 0; i < 0x3e8; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    int num13 = Main.rand.Next(minValue, maxValue);
                    int num14 = Main.rand.Next(num5, maxTilesY);
                    if (!Main.tile[num13, num14].active || !Main.tileSolid[Main.tile[num13, num14].type])
                    {
                        if (Main.tile[num13, num14].wall == 1)
                        {
                            continue;
                        }
                        for (int k = num14; k < Main.maxTilesY; k++)
                        {
                            if (Main.tile[num13, k].active && Main.tileSolid[Main.tile[num13, k].type])
                            {
                                if (((num13 < num7) || (num13 > num8)) || ((k < num9) || (k > num10)))
                                {
                                    byte type = Main.tile[num13, k].type;
                                    num = num13;
                                    num2 = k;
                                    flag = true;
                                }
                                break;
                            }
                        }
                        if (flag)
                        {
                            int num16 = num - (spawnSpaceX / 2);
                            int num17 = num + (spawnSpaceX / 2);
                            int num18 = num2 - spawnSpaceY;
                            int num19 = num2;
                            if (num16 < 0)
                            {
                                flag = false;
                            }
                            if (num17 > Main.maxTilesX)
                            {
                                flag = false;
                            }
                            if (num18 < 0)
                            {
                                flag = false;
                            }
                            if (num19 > Main.maxTilesY)
                            {
                                flag = false;
                            }
                            if (flag)
                            {
                                for (int m = num16; m < num17; m++)
                                {
                                    for (int n = num18; n < num19; n++)
                                    {
                                        if (Main.tile[m, n].active && Main.tileSolid[Main.tile[m, n].type])
                                        {
                                            flag = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (flag || flag)
                    {
                        break;
                    }
                }
                if (flag)
                {
                    Rectangle rectangle = new Rectangle(num * 0x10, num2 * 0x10, 0x10, 0x10);
                    for (int num22 = 0; num22 < 8; num22++)
                    {
                        if (Main.player[num22].active)
                        {
                            Rectangle rectangle2 = new Rectangle(((((int) Main.player[num22].position.X) + (Main.player[num22].width / 2)) - (Main.screenWidth / 2)) - safeRangeX, ((((int) Main.player[num22].position.Y) + (Main.player[num22].height / 2)) - (Main.screenHeight / 2)) - safeRangeY, Main.screenWidth + (safeRangeX * 2), Main.screenHeight + (safeRangeY * 2));
                            if (rectangle.Intersects(rectangle2))
                            {
                                flag = false;
                            }
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
                int index = 0x3e8;
                index = NewNPC((num * 0x10) + 8, num2 * 0x10, Type, 1);
                Main.npc[index].target = plr;
                string name = Main.npc[index].name;
                if (Main.npc[index].type == 13)
                {
                    name = "Eater of Worlds";
                }
                if (Main.npc[index].type == 0x23)
                {
                    name = "Skeletron";
                }
                if ((Main.netMode == 2) && (index < 0x3e8))
                {
                    NetMessage.SendData(0x17, -1, -1, "", index, 0f, 0f, 0f);
                }
                if (Main.netMode == 0)
                {
                    Main.NewText(name + " has awoken!", 0xaf, 0x4b, 0xff);
                }
                else if (Main.netMode == 2)
                {
                    NetMessage.SendData(0x19, -1, -1, name + " has awoken!", 8, 175f, 75f, 255f);
                }
            }
        }

        public double StrikeNPC(int Damage, float knockBack, int hitDirection)
        {
            if (!this.active || (this.life <= 0))
            {
                return 0.0;
            }
            double dmg = Main.CalculateDamage(Damage, this.defense);
            if (this.friendly)
            {
                CombatText.NewText(new Rectangle((int) this.position.X, (int) this.position.Y, this.width, this.height), new Color(0xff, 80, 90, 0xff), ((int) dmg).ToString());
            }
            else
            {
                CombatText.NewText(new Rectangle((int) this.position.X, (int) this.position.Y, this.width, this.height), new Color(0xff, 160, 80, 0xff), ((int) dmg).ToString());
            }
            if (dmg < 1.0)
            {
                return 0.0;
            }
            if (this.townNPC)
            {
                this.ai[0] = 1f;
                this.ai[1] = 300 + Main.rand.Next(300);
                this.ai[2] = 0f;
                this.direction = hitDirection;
                this.netUpdate = true;
            }
            if ((this.aiStyle == 8) && (Main.netMode != 1))
            {
                this.ai[0] = 400f;
                this.TargetClosest();
            }
            this.life -= (int) dmg;
            if ((knockBack > 0f) && (this.knockBackResist > 0f))
            {
                if (!this.noGravity)
                {
                    this.velocity.Y = (-knockBack * 0.75f) * this.knockBackResist;
                }
                else
                {
                    this.velocity.Y = (-knockBack * 0.5f) * this.knockBackResist;
                }
                this.velocity.X = (knockBack * hitDirection) * this.knockBackResist;
            }
            this.HitEffect(hitDirection, dmg);
            if (this.soundHit > 0)
            {
                Main.PlaySound(3, (int) this.position.X, (int) this.position.Y, this.soundHit);
            }
            if (this.life <= 0)
            {
                if (this.townNPC && (this.type != 0x25))
                {
                    if (Main.netMode == 0)
                    {
                        Main.NewText(this.name + " was slain...", 0xff, 0x19, 0x19);
                    }
                    else if (Main.netMode == 2)
                    {
                        NetMessage.SendData(0x19, -1, -1, this.name + " was slain...", 8, 255f, 25f, 25f);
                    }
                }
                if ((this.townNPC && (Main.netMode != 1)) && (this.homeless && (WorldGen.spawnNPC == this.type)))
                {
                    WorldGen.spawnNPC = 0;
                }
                if (this.soundKilled > 0)
                {
                    Main.PlaySound(4, (int) this.position.X, (int) this.position.Y, this.soundKilled);
                }
                this.NPCLoot();
                this.active = false;
                if (((this.type != 0x1a) && (this.type != 0x1b)) && ((this.type != 0x1c) && (this.type != 0x1d)))
                {
                    return dmg;
                }
                if (!ShankShock.infinateInvasion)
                {
                    Main.invasionSize--;
                }
                if (ShankShock.infinateInvasion)
                {
                    ShankShock.incrementKills();
                }
            }
            return dmg;
        }

        public void TargetClosest()
        {
            float num = -1f;
            for (int i = 0; i < 8; i++)
            {
                if ((Main.player[i].active && !Main.player[i].dead) && ((num == -1f) || ((Math.Abs((float) (((Main.player[i].position.X + (Main.player[i].width / 2)) - this.position.X) + (this.width / 2))) + Math.Abs((float) (((Main.player[i].position.Y + (Main.player[i].height / 2)) - this.position.Y) + (this.height / 2)))) < num)))
                {
                    num = Math.Abs((float) (((Main.player[i].position.X + (Main.player[i].width / 2)) - this.position.X) + (this.width / 2))) + Math.Abs((float) (((Main.player[i].position.Y + (Main.player[i].height / 2)) - this.position.Y) + (this.height / 2)));
                    this.target = i;
                }
            }
            if ((this.target < 0) || (this.target >= 8))
            {
                this.target = 0;
            }
            this.targetRect = new Rectangle((int) Main.player[this.target].position.X, (int) Main.player[this.target].position.Y, Main.player[this.target].width, Main.player[this.target].height);
            this.direction = 1;
            if ((this.targetRect.X + (this.targetRect.Width / 2)) < (this.position.X + (this.width / 2)))
            {
                this.direction = -1;
            }
            this.directionY = 1;
            if ((this.targetRect.Y + (this.targetRect.Height / 2)) < (this.position.Y + (this.height / 2)))
            {
                this.directionY = -1;
            }
            if (((this.direction != this.oldDirection) || (this.directionY != this.oldDirectionY)) || (this.target != this.oldTarget))
            {
                this.netUpdate = true;
            }
        }

        public void UpdateNPC(int i)
        {
            this.whoAmI = i;
            if (this.active)
            {
                float num = 10f;
                float num2 = 0.3f;
                if (this.wet)
                {
                    num2 = 0.2f;
                    num = 7f;
                }
                if (this.soundDelay > 0)
                {
                    this.soundDelay--;
                }
                if (this.life <= 0)
                {
                    this.active = false;
                }
                this.oldTarget = this.target;
                this.oldDirection = this.direction;
                this.oldDirectionY = this.directionY;
                this.AI();
                for (int j = 0; j < 9; j++)
                {
                    if (this.immune[j] > 0)
                    {
                        this.immune[j]--;
                    }
                }
                if (!this.noGravity)
                {
                    this.velocity.Y += num2;
                    if (this.velocity.Y > num)
                    {
                        this.velocity.Y = num;
                    }
                }
                if ((this.velocity.X < 0.005) && (this.velocity.X > -0.005))
                {
                    this.velocity.X = 0f;
                }
                if (((Main.netMode != 1) && this.friendly) && ((this.type != 0x16) && (this.type != 0x25)))
                {
                    if (this.life < this.lifeMax)
                    {
                        this.friendlyRegen++;
                        if (this.friendlyRegen > 300)
                        {
                            this.friendlyRegen = 0;
                            this.life++;
                            this.netUpdate = true;
                        }
                    }
                    if (this.immune[8] == 0)
                    {
                        Rectangle rectangle = new Rectangle((int) this.position.X, (int) this.position.Y, this.width, this.height);
                        for (int k = 0; k < 0x3e8; k++)
                        {
                            if (Main.npc[k].active && !Main.npc[k].friendly)
                            {
                                Rectangle rectangle2 = new Rectangle((int) Main.npc[k].position.X, (int) Main.npc[k].position.Y, Main.npc[k].width, Main.npc[k].height);
                                if (rectangle.Intersects(rectangle2))
                                {
                                    int damage = Main.npc[k].damage;
                                    int num6 = 6;
                                    int hitDirection = 1;
                                    if ((Main.npc[k].position.X + (Main.npc[k].width / 2)) > (this.position.X + (this.width / 2)))
                                    {
                                        hitDirection = -1;
                                    }
                                    Main.npc[i].StrikeNPC(damage, (float) num6, hitDirection);
                                    if (Main.netMode != 0)
                                    {
                                        NetMessage.SendData(0x1c, -1, -1, "", i, (float) damage, (float) num6, (float) hitDirection);
                                    }
                                    this.netUpdate = true;
                                    this.immune[8] = 30;
                                }
                            }
                        }
                    }
                }
                if (!this.noTileCollide)
                {
                    bool flag = Collision.LavaCollision(this.position, this.width, this.height);
                    if (flag)
                    {
                        this.lavaWet = true;
                        if ((Main.netMode != 1) && (this.immune[8] == 0))
                        {
                            this.immune[8] = 30;
                            this.StrikeNPC(50, 0f, 0);
                            if ((Main.netMode == 2) && (Main.netMode != 0))
                            {
                                NetMessage.SendData(0x1c, -1, -1, "", this.whoAmI, 50f, 0f, 0f);
                            }
                        }
                    }
                    if (Collision.WetCollision(this.position, this.width, this.height))
                    {
                        if (!this.wet && (this.wetCount == 0))
                        {
                            this.wetCount = 10;
                            if (!flag)
                            {
                                for (int m = 0; m < 50; m++)
                                {
                                    Color newColor = new Color();
                                    int index = Dust.NewDust(new Vector2(this.position.X - 6f, (this.position.Y + (this.height / 2)) - 8f), this.width + 12, 0x18, 0x21, 0f, 0f, 0, newColor, 1f);
                                    Main.dust[index].velocity.Y -= 4f;
                                    Main.dust[index].velocity.X *= 2.5f;
                                    Main.dust[index].scale = 1.3f;
                                    Main.dust[index].alpha = 100;
                                    Main.dust[index].noGravity = true;
                                }
                                Main.PlaySound(0x13, (int) this.position.X, (int) this.position.Y, 0);
                            }
                            else
                            {
                                for (int n = 0; n < 20; n++)
                                {
                                    Color color2 = new Color();
                                    int num11 = Dust.NewDust(new Vector2(this.position.X - 6f, (this.position.Y + (this.height / 2)) - 8f), this.width + 12, 0x18, 0x23, 0f, 0f, 0, color2, 1f);
                                    Main.dust[num11].velocity.Y -= 1.5f;
                                    Main.dust[num11].velocity.X *= 2.5f;
                                    Main.dust[num11].scale = 1.3f;
                                    Main.dust[num11].alpha = 100;
                                    Main.dust[num11].noGravity = true;
                                }
                                Main.PlaySound(0x13, (int) this.position.X, (int) this.position.Y, 1);
                            }
                        }
                        this.wet = true;
                    }
                    else if (this.wet)
                    {
                        this.velocity.X *= 0.5f;
                        this.wet = false;
                        if (this.wetCount == 0)
                        {
                            this.wetCount = 10;
                            if (!this.lavaWet)
                            {
                                for (int num12 = 0; num12 < 50; num12++)
                                {
                                    Color color3 = new Color();
                                    int num13 = Dust.NewDust(new Vector2(this.position.X - 6f, this.position.Y + (this.height / 2)), this.width + 12, 0x18, 0x21, 0f, 0f, 0, color3, 1f);
                                    Main.dust[num13].velocity.Y -= 4f;
                                    Main.dust[num13].velocity.X *= 2.5f;
                                    Main.dust[num13].scale = 1.3f;
                                    Main.dust[num13].alpha = 100;
                                    Main.dust[num13].noGravity = true;
                                }
                                Main.PlaySound(0x13, (int) this.position.X, (int) this.position.Y, 0);
                            }
                            else
                            {
                                for (int num14 = 0; num14 < 20; num14++)
                                {
                                    Color color4 = new Color();
                                    int num15 = Dust.NewDust(new Vector2(this.position.X - 6f, (this.position.Y + (this.height / 2)) - 8f), this.width + 12, 0x18, 0x23, 0f, 0f, 0, color4, 1f);
                                    Main.dust[num15].velocity.Y -= 1.5f;
                                    Main.dust[num15].velocity.X *= 2.5f;
                                    Main.dust[num15].scale = 1.3f;
                                    Main.dust[num15].alpha = 100;
                                    Main.dust[num15].noGravity = true;
                                }
                                Main.PlaySound(0x13, (int) this.position.X, (int) this.position.Y, 1);
                            }
                        }
                    }
                    if (!this.wet)
                    {
                        this.lavaWet = false;
                    }
                    if (this.wetCount > 0)
                    {
                        this.wetCount = (byte) (this.wetCount - 1);
                    }
                    bool fallThrough = false;
                    if (this.aiStyle == 10)
                    {
                        fallThrough = true;
                    }
                    if ((this.aiStyle == 3) && (this.directionY == 1))
                    {
                        fallThrough = true;
                    }
                    this.oldVelocity = this.velocity;
                    this.collideX = false;
                    this.collideY = false;
                    if (this.wet)
                    {
                        Vector2 velocity = this.velocity;
                        this.velocity = Collision.TileCollision(this.position, this.velocity, this.width, this.height, fallThrough, fallThrough);
                        Vector2 vector2 = (Vector2) (this.velocity * 0.5f);
                        if (this.velocity.X != velocity.X)
                        {
                            vector2.X = this.velocity.X;
                            this.collideX = true;
                        }
                        if (this.velocity.Y != velocity.Y)
                        {
                            vector2.Y = this.velocity.Y;
                            this.collideY = true;
                        }
                        this.oldPosition = this.position;
                        this.position += vector2;
                    }
                    else
                    {
                        this.velocity = Collision.TileCollision(this.position, this.velocity, this.width, this.height, fallThrough, fallThrough);
                        if (this.oldVelocity.X != this.velocity.X)
                        {
                            this.collideX = true;
                        }
                        if (this.oldVelocity.Y != this.velocity.Y)
                        {
                            this.collideY = true;
                        }
                        this.oldPosition = this.position;
                        this.position += this.velocity;
                    }
                }
                else
                {
                    this.oldPosition = this.position;
                    this.position += this.velocity;
                }
                if (!this.active)
                {
                    this.netUpdate = true;
                }
                if ((Main.netMode == 2) && this.netUpdate)
                {
                    NetMessage.SendData(0x17, -1, -1, "", i, 0f, 0f, 0f);
                }
                this.FindFrame();
                this.CheckActive();
                this.netUpdate = false;
            }
        }
    }
}

