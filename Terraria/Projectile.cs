namespace Terraria
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.InteropServices;

    public class Projectile
    {
        public bool active;

        public int aiStyle;
        public int alpha;
        public int damage;
        public int direction;
        public bool friendly;
        public int height;
        public bool hostile;
        public int identity;
        public bool ignoreWater;
        public float knockBack;
        public bool lavaWet;
        public float light;
        public static int maxAI = 2;
        public int maxUpdates;
        public string name = "";
        public bool netUpdate;
        public int numUpdates;
        public int owner = 8;
        public int penetrate = 1;
        public int[] playerImmune = new int[8];
        public Vector2 position;
        public int restrikeDelay;
        public float rotation;
        public float scale = 1f;
        public int soundDelay;
        public bool tileCollide;
        public int timeLeft;
        public int type;
        public Vector2 velocity;
        public bool wet;
        public byte wetCount;
        public int whoAmI;
        public int width;
        public float[] ai = new float[maxAI];
        public void AI()
        {
            if (this.aiStyle == 1)
            {
                if (((this.type == 20) || (this.type == 14)) || (this.type == 0x24))
                {
                    if (this.alpha > 0)
                    {
                        this.alpha -= 15;
                    }
                    if (this.alpha < 0)
                    {
                        this.alpha = 0;
                    }
                }
                if (((this.type != 5) && (this.type != 14)) && ((this.type != 20) && (this.type != 0x24)))
                {
                    this.ai[0]++;
                }
                if (this.ai[0] >= 15f)
                {
                    this.ai[0] = 15f;
                    this.velocity.Y += 0.1f;
                }
                this.rotation = ((float) Math.Atan2((double) this.velocity.Y, (double) this.velocity.X)) + 1.57f;
                if (this.velocity.Y > 16f)
                {
                    this.velocity.Y = 16f;
                }
            }
            else if (this.aiStyle == 2)
            {
                this.ai[0]++;
                if (this.ai[0] >= 20f)
                {
                    this.velocity.Y += 0.4f;
                    this.velocity.X *= 0.97f;
                }
                this.rotation += ((Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y)) * 0.03f) * this.direction;
                if (this.velocity.Y > 16f)
                {
                    this.velocity.Y = 16f;
                }
            }
            else
            {
                Color color;
                if (this.aiStyle == 3)
                {
                    if (this.soundDelay == 0)
                    {
                        this.soundDelay = 8;
                        Main.PlaySound(2, (int) this.position.X, (int) this.position.Y, 7);
                    }
                    if (this.type == 0x13)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            color = new Color();
                            int index = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, this.velocity.X * 0.2f, this.velocity.Y * 0.2f, 100, color, 2f);
                            Main.dust[index].noGravity = true;
                            Main.dust[index].velocity.X *= 0.3f;
                            Main.dust[index].velocity.Y *= 0.3f;
                        }
                    }
                    else if (this.type == 0x21)
                    {
                        if (Main.rand.Next(1) == 0)
                        {
                            int num3 = Dust.NewDust(this.position, this.width, this.height, 40, this.velocity.X * 0.25f, this.velocity.Y * 0.25f, 0, new Color(), 1.4f);
                            Main.dust[num3].noGravity = true;
                        }
                    }
                    else if (Main.rand.Next(5) == 0)
                    {
                        Dust.NewDust(this.position, this.width, this.height, 15, this.velocity.X * 0.5f, this.velocity.Y * 0.5f, 150, new Color(), 0.9f);
                    }
                    if (this.ai[0] == 0f)
                    {
                        this.ai[1]++;
                        if (this.ai[1] >= 30f)
                        {
                            this.ai[0] = 1f;
                            this.ai[1] = 0f;
                            this.netUpdate = true;
                        }
                    }
                    else
                    {
                        this.tileCollide = false;
                        float num4 = 9f;
                        float num5 = 0.4f;
                        if (this.type == 0x13)
                        {
                            num4 = 13f;
                            num5 = 0.6f;
                        }
                        else if (this.type == 0x21)
                        {
                            num4 = 15f;
                            num5 = 0.8f;
                        }
                        Vector2 vector = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                        float num6 = (Main.player[this.owner].position.X + (Main.player[this.owner].width / 2)) - vector.X;
                        float num7 = (Main.player[this.owner].position.Y + (Main.player[this.owner].height / 2)) - vector.Y;
                        float num8 = (float) Math.Sqrt((double) ((num6 * num6) + (num7 * num7)));
                        num8 = num4 / num8;
                        num6 *= num8;
                        num7 *= num8;
                        if (this.velocity.X < num6)
                        {
                            this.velocity.X += num5;
                            if ((this.velocity.X < 0f) && (num6 > 0f))
                            {
                                this.velocity.X += num5;
                            }
                        }
                        else if (this.velocity.X > num6)
                        {
                            this.velocity.X -= num5;
                            if ((this.velocity.X > 0f) && (num6 < 0f))
                            {
                                this.velocity.X -= num5;
                            }
                        }
                        if (this.velocity.Y < num7)
                        {
                            this.velocity.Y += num5;
                            if ((this.velocity.Y < 0f) && (num7 > 0f))
                            {
                                this.velocity.Y += num5;
                            }
                        }
                        else if (this.velocity.Y > num7)
                        {
                            this.velocity.Y -= num5;
                            if ((this.velocity.Y > 0f) && (num7 < 0f))
                            {
                                this.velocity.Y -= num5;
                            }
                        }
                        if (Main.myPlayer == this.owner)
                        {
                            Rectangle rectangle = new Rectangle((int) this.position.X, (int) this.position.Y, this.width, this.height);
                            Rectangle rectangle2 = new Rectangle((int) Main.player[this.owner].position.X, (int) Main.player[this.owner].position.Y, Main.player[this.owner].width, Main.player[this.owner].height);
                            if (rectangle.Intersects(rectangle2))
                            {
                                this.Kill();
                            }
                        }
                    }
                    this.rotation += 0.4f * this.direction;
                }
                else if (this.aiStyle == 4)
                {
                    this.rotation = ((float) Math.Atan2((double) this.velocity.Y, (double) this.velocity.X)) + 1.57f;
                    if (this.ai[0] != 0f)
                    {
                        if ((this.alpha < 170) && ((this.alpha + 5) >= 170))
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                color = new Color();
                                Dust.NewDust(this.position, this.width, this.height, 0x12, this.velocity.X * 0.025f, this.velocity.Y * 0.025f, 170, color, 1.2f);
                            }
                            Dust.NewDust(this.position, this.width, this.height, 14, 0f, 0f, 170, new Color(), 1.1f);
                        }
                        this.alpha += 5;
                        if (this.alpha >= 0xff)
                        {
                            this.Kill();
                        }
                    }
                    else
                    {
                        this.alpha -= 50;
                        if (this.alpha <= 0)
                        {
                            this.alpha = 0;
                            this.ai[0] = 1f;
                            if (this.ai[1] == 0f)
                            {
                                this.ai[1]++;
                                this.position += (Vector2) (this.velocity * 1f);
                            }
                            if ((this.type == 7) && (Main.myPlayer == this.owner))
                            {
                                int type = this.type;
                                if (this.ai[1] >= 6f)
                                {
                                    type++;
                                }
                                int num10 = NewProjectile((this.position.X + this.velocity.X) + (this.width / 2), (this.position.Y + this.velocity.Y) + (this.height / 2), this.velocity.X, this.velocity.Y, type, this.damage, this.knockBack, this.owner);
                                Main.projectile[num10].damage = this.damage;
                                Main.projectile[num10].ai[1] = this.ai[1] + 1f;
                                NetMessage.SendData(0x1b, -1, -1, "", num10, 0f, 0f, 0f);
                            }
                        }
                    }
                }
                else if (this.aiStyle == 5)
                {
                    if (this.soundDelay == 0)
                    {
                        this.soundDelay = 20 + Main.rand.Next(40);
                        Main.PlaySound(2, (int) this.position.X, (int) this.position.Y, 9);
                    }
                    if (this.ai[0] == 0f)
                    {
                        this.ai[0] = 1f;
                    }
                    this.alpha += (int) (25f * this.ai[0]);
                    if (this.alpha > 200)
                    {
                        this.alpha = 200;
                        this.ai[0] = -1f;
                    }
                    if (this.alpha < 0)
                    {
                        this.alpha = 0;
                        this.ai[0] = 1f;
                    }
                    this.rotation += ((Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y)) * 0.01f) * this.direction;
                    if (Main.rand.Next(10) == 0)
                    {
                        Dust.NewDust(this.position, this.width, this.height, 15, this.velocity.X * 0.5f, this.velocity.Y * 0.5f, 150, new Color(), 1.2f);
                    }
                    if (Main.rand.Next(20) == 0)
                    {
                        Gore.NewGore(this.position, new Vector2(this.velocity.X * 0.2f, this.velocity.Y * 0.2f), Main.rand.Next(0x10, 0x12));
                    }
                }
                else if (this.aiStyle == 6)
                {
                    this.velocity = (Vector2) (this.velocity * 0.95f);
                    this.ai[0]++;
                    if (this.ai[0] == 180f)
                    {
                        this.Kill();
                    }
                    if (this.ai[1] == 0f)
                    {
                        this.ai[1] = 1f;
                        for (int k = 0; k < 30; k++)
                        {
                            color = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 10 + this.type, this.velocity.X, this.velocity.Y, 50, color, 1f);
                        }
                    }
                    if (this.type == 10)
                    {
                        int num13 = ((int) (this.position.X / 16f)) - 1;
                        int maxTilesX = ((int) ((this.position.X + this.width) / 16f)) + 2;
                        int num15 = ((int) (this.position.Y / 16f)) - 1;
                        int maxTilesY = ((int) ((this.position.Y + this.height) / 16f)) + 2;
                        if (num13 < 0)
                        {
                            num13 = 0;
                        }
                        if (maxTilesX > Main.maxTilesX)
                        {
                            maxTilesX = Main.maxTilesX;
                        }
                        if (num15 < 0)
                        {
                            num15 = 0;
                        }
                        if (maxTilesY > Main.maxTilesY)
                        {
                            maxTilesY = Main.maxTilesY;
                        }
                        for (int m = num13; m < maxTilesX; m++)
                        {
                            for (int n = num15; n < maxTilesY; n++)
                            {
                                Vector2 vector2;
                                vector2.X = m * 0x10;
                                vector2.Y = n * 0x10;
                                if (((((this.position.X + this.width) > vector2.X) && (this.position.X < (vector2.X + 16f))) && (((this.position.Y + this.height) > vector2.Y) && (this.position.Y < (vector2.Y + 16f)))) && ((Main.myPlayer == this.owner) && Main.tile[m, n].active))
                                {
                                    if (Main.tile[m, n].type == 0x17)
                                    {
                                        Main.tile[m, n].type = 2;
                                        WorldGen.SquareTileFrame(m, n, true);
                                        if (Main.netMode == 1)
                                        {
                                            NetMessage.SendTileSquare(-1, m - 1, n - 1, 3);
                                        }
                                    }
                                    if (Main.tile[m, n].type == 0x19)
                                    {
                                        Main.tile[m, n].type = 1;
                                        WorldGen.SquareTileFrame(m, n, true);
                                        if (Main.netMode == 1)
                                        {
                                            NetMessage.SendTileSquare(-1, m - 1, n - 1, 3);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (this.aiStyle == 7)
                {
                    if (Main.player[this.owner].dead)
                    {
                        this.Kill();
                    }
                    else
                    {
                        Vector2 vector3 = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                        float num19 = (Main.player[this.owner].position.X + (Main.player[this.owner].width / 2)) - vector3.X;
                        float num20 = (Main.player[this.owner].position.Y + (Main.player[this.owner].height / 2)) - vector3.Y;
                        float num21 = (float) Math.Sqrt((double) ((num19 * num19) + (num20 * num20)));
                        this.rotation = ((float) Math.Atan2((double) num20, (double) num19)) - 1.57f;
                        if (this.ai[0] == 0f)
                        {
                            if (((num21 > 300f) && (this.type == 13)) || ((num21 > 400f) && (this.type == 0x20)))
                            {
                                this.ai[0] = 1f;
                            }
                            int num22 = ((int) (this.position.X / 16f)) - 1;
                            int num23 = ((int) ((this.position.X + this.width) / 16f)) + 2;
                            int num24 = ((int) (this.position.Y / 16f)) - 1;
                            int num25 = ((int) ((this.position.Y + this.height) / 16f)) + 2;
                            if (num22 < 0)
                            {
                                num22 = 0;
                            }
                            if (num23 > Main.maxTilesX)
                            {
                                num23 = Main.maxTilesX;
                            }
                            if (num24 < 0)
                            {
                                num24 = 0;
                            }
                            if (num25 > Main.maxTilesY)
                            {
                                num25 = Main.maxTilesY;
                            }
                            for (int num26 = num22; num26 < num23; num26++)
                            {
                                for (int num27 = num24; num27 < num25; num27++)
                                {
                                    Vector2 vector4;
                                    if (Main.tile[num26, num27] == null)
                                    {
                                        Main.tile[num26, num27] = new Tile();
                                    }
                                    vector4.X = num26 * 0x10;
                                    vector4.Y = num27 * 0x10;
                                    if (((((this.position.X + this.width) > vector4.X) && (this.position.X < (vector4.X + 16f))) && (((this.position.Y + this.height) > vector4.Y) && (this.position.Y < (vector4.Y + 16f)))) && (Main.tile[num26, num27].active && Main.tileSolid[Main.tile[num26, num27].type]))
                                    {
                                        if (Main.player[this.owner].grapCount < 10)
                                        {
                                            Main.player[this.owner].grappling[Main.player[this.owner].grapCount] = this.whoAmI;
                                            Player player1 = Main.player[this.owner];
                                            player1.grapCount++;
                                        }
                                        if (Main.myPlayer == this.owner)
                                        {
                                            int num28 = 0;
                                            int num29 = -1;
                                            int timeLeft = 0x186a0;
                                            for (int num31 = 0; num31 < 0x3e8; num31++)
                                            {
                                                if ((Main.projectile[num31].active && (Main.projectile[num31].owner == this.owner)) && (Main.projectile[num31].aiStyle == 7))
                                                {
                                                    if (Main.projectile[num31].timeLeft < timeLeft)
                                                    {
                                                        num29 = num31;
                                                        timeLeft = Main.projectile[num31].timeLeft;
                                                    }
                                                    num28++;
                                                }
                                            }
                                            if (num28 > 3)
                                            {
                                                Main.projectile[num29].Kill();
                                            }
                                        }
                                        WorldGen.KillTile(num26, num27, true, true, false);
                                        Main.PlaySound(0, num26 * 0x10, num27 * 0x10, 1);
                                        this.velocity.X = 0f;
                                        this.velocity.Y = 0f;
                                        this.ai[0] = 2f;
                                        this.position.X = ((num26 * 0x10) + 8) - (this.width / 2);
                                        this.position.Y = ((num27 * 0x10) + 8) - (this.height / 2);
                                        this.damage = 0;
                                        this.netUpdate = true;
                                        if (Main.myPlayer == this.owner)
                                        {
                                            NetMessage.SendData(13, -1, -1, "", this.owner, 0f, 0f, 0f);
                                        }
                                        break;
                                    }
                                }
                                if (this.ai[0] == 2f)
                                {
                                    return;
                                }
                            }
                        }
                        else if (this.ai[0] == 1f)
                        {
                            float num32 = 11f;
                            if (this.type == 0x20)
                            {
                                num32 = 15f;
                            }
                            if (num21 < 24f)
                            {
                                this.Kill();
                            }
                            num21 = num32 / num21;
                            num19 *= num21;
                            num20 *= num21;
                            this.velocity.X = num19;
                            this.velocity.Y = num20;
                        }
                        else if (this.ai[0] == 2f)
                        {
                            int num33 = ((int) (this.position.X / 16f)) - 1;
                            int num34 = ((int) ((this.position.X + this.width) / 16f)) + 2;
                            int num35 = ((int) (this.position.Y / 16f)) - 1;
                            int num36 = ((int) ((this.position.Y + this.height) / 16f)) + 2;
                            if (num33 < 0)
                            {
                                num33 = 0;
                            }
                            if (num34 > Main.maxTilesX)
                            {
                                num34 = Main.maxTilesX;
                            }
                            if (num35 < 0)
                            {
                                num35 = 0;
                            }
                            if (num36 > Main.maxTilesY)
                            {
                                num36 = Main.maxTilesY;
                            }
                            bool flag = true;
                            for (int num37 = num33; num37 < num34; num37++)
                            {
                                for (int num38 = num35; num38 < num36; num38++)
                                {
                                    Vector2 vector5;
                                    if (Main.tile[num37, num38] == null)
                                    {
                                        Main.tile[num37, num38] = new Tile();
                                    }
                                    vector5.X = num37 * 0x10;
                                    vector5.Y = num38 * 0x10;
                                    if (((((this.position.X + (this.width / 2)) > vector5.X) && ((this.position.X + (this.width / 2)) < (vector5.X + 16f))) && (((this.position.Y + (this.height / 2)) > vector5.Y) && ((this.position.Y + (this.height / 2)) < (vector5.Y + 16f)))) && (Main.tile[num37, num38].active && Main.tileSolid[Main.tile[num37, num38].type]))
                                    {
                                        flag = false;
                                    }
                                }
                            }
                            if (flag)
                            {
                                this.ai[0] = 1f;
                            }
                            else if (Main.player[this.owner].grapCount < 10)
                            {
                                Main.player[this.owner].grappling[Main.player[this.owner].grapCount] = this.whoAmI;
                                Player player2 = Main.player[this.owner];
                                player2.grapCount++;
                            }
                        }
                    }
                }
                else if (this.aiStyle == 8)
                {
                    if (this.type == 0x1b)
                    {
                        color = new Color();
                        int num39 = Dust.NewDust(new Vector2(this.position.X + this.velocity.X, this.position.Y + this.velocity.Y), this.width, this.height, 0x1d, this.velocity.X, this.velocity.Y, 100, color, 3f);
                        Main.dust[num39].noGravity = true;
                        num39 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 0x1d, this.velocity.X, this.velocity.Y, 100, new Color(), 1.5f);
                    }
                    else
                    {
                        for (int num40 = 0; num40 < 2; num40++)
                        {
                            color = new Color();
                            int num41 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, this.velocity.X * 0.2f, this.velocity.Y * 0.2f, 100, color, 2f);
                            Main.dust[num41].noGravity = true;
                            Main.dust[num41].velocity.X *= 0.3f;
                            Main.dust[num41].velocity.Y *= 0.3f;
                        }
                    }
                    if (this.type != 0x1b)
                    {
                        this.ai[1]++;
                    }
                    if (this.ai[1] >= 20f)
                    {
                        this.velocity.Y += 0.2f;
                    }
                    this.rotation += 0.3f * this.direction;
                    if (this.velocity.Y > 16f)
                    {
                        this.velocity.Y = 16f;
                    }
                }
                else if (this.aiStyle == 9)
                {
                    if (this.type == 0x22)
                    {
                        color = new Color();
                        int num42 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, this.velocity.X * 0.2f, this.velocity.Y * 0.2f, 100, color, 3.5f);
                        Main.dust[num42].noGravity = true;
                        Dust dust1 = Main.dust[num42];
                        dust1.velocity = (Vector2) (dust1.velocity * 1.4f);
                        num42 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, this.velocity.X * 0.2f, this.velocity.Y * 0.2f, 100, new Color(), 1.5f);
                    }
                    else
                    {
                        if ((this.soundDelay == 0) && ((Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y)) > 2f))
                        {
                            this.soundDelay = 10;
                            Main.PlaySound(2, (int) this.position.X, (int) this.position.Y, 9);
                        }
                        int num43 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 15, 0f, 0f, 100, new Color(), 2f);
                        Dust dust2 = Main.dust[num43];
                        dust2.velocity = (Vector2) (dust2.velocity * 0.3f);
                        Main.dust[num43].position.X = ((this.position.X + (this.width / 2)) + 4f) + Main.rand.Next(-4, 5);
                        Main.dust[num43].position.Y = (this.position.Y + (this.height / 2)) + Main.rand.Next(-4, 5);
                        Main.dust[num43].noGravity = true;
                    }
                    if ((Main.myPlayer == this.owner) && (this.ai[0] == 0f))
                    {
                        if (Main.player[this.owner].channel)
                        {
                            float num44 = 12f;
                            Vector2 vector6 = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                            float num45 = (Main.mouseState.X + Main.screenPosition.X) - vector6.X;
                            float num46 = (Main.mouseState.Y + Main.screenPosition.Y) - vector6.Y;
                            float num47 = (float) Math.Sqrt((double) ((num45 * num45) + (num46 * num46)));
                            num47 = (float) Math.Sqrt((double) ((num45 * num45) + (num46 * num46)));
                            if (num47 > num44)
                            {
                                num47 = num44 / num47;
                                num45 *= num47;
                                num46 *= num47;
                                if ((num45 != this.velocity.X) || (num46 != this.velocity.Y))
                                {
                                    this.netUpdate = true;
                                }
                                this.velocity.X = num45;
                                this.velocity.Y = num46;
                            }
                            else
                            {
                                if ((num45 != this.velocity.X) || (num46 != this.velocity.Y))
                                {
                                    this.netUpdate = true;
                                }
                                this.velocity.X = num45;
                                this.velocity.Y = num46;
                            }
                        }
                        else
                        {
                            this.Kill();
                        }
                    }
                    if (this.type == 0x22)
                    {
                        this.rotation += 0.3f * this.direction;
                    }
                    else if ((this.velocity.X != 0f) || (this.velocity.Y != 0f))
                    {
                        this.rotation = ((float) Math.Atan2((double) this.velocity.Y, (double) this.velocity.X)) - 2.355f;
                    }
                    if (this.velocity.Y > 16f)
                    {
                        this.velocity.Y = 16f;
                    }
                }
                else if (this.aiStyle == 10)
                {
                    if (this.type == 0x1f)
                    {
                        if (Main.rand.Next(2) == 0)
                        {
                            int num48 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 0x20, 0f, this.velocity.Y / 2f, 0, new Color(), 1f);
                            Main.dust[num48].velocity.X *= 0.4f;
                        }
                    }
                    else if (Main.rand.Next(20) == 0)
                    {
                        Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 0, 0f, 0f, 0, new Color(), 1f);
                    }
                    if ((Main.myPlayer == this.owner) && (this.ai[0] == 0f))
                    {
                        if (Main.player[this.owner].channel)
                        {
                            float num49 = 12f;
                            Vector2 vector7 = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                            float num50 = (Main.mouseState.X + Main.screenPosition.X) - vector7.X;
                            float num51 = (Main.mouseState.Y + Main.screenPosition.Y) - vector7.Y;
                            float num52 = (float) Math.Sqrt((double) ((num50 * num50) + (num51 * num51)));
                            num52 = (float) Math.Sqrt((double) ((num50 * num50) + (num51 * num51)));
                            if (num52 > num49)
                            {
                                num52 = num49 / num52;
                                num50 *= num52;
                                num51 *= num52;
                                if ((num50 != this.velocity.X) || (num51 != this.velocity.Y))
                                {
                                    this.netUpdate = true;
                                }
                                this.velocity.X = num50;
                                this.velocity.Y = num51;
                            }
                            else
                            {
                                if ((num50 != this.velocity.X) || (num51 != this.velocity.Y))
                                {
                                    this.netUpdate = true;
                                }
                                this.velocity.X = num50;
                                this.velocity.Y = num51;
                            }
                        }
                        else
                        {
                            this.ai[0] = 1f;
                            this.netUpdate = true;
                        }
                    }
                    if (this.ai[0] == 1f)
                    {
                        this.velocity.Y += 0.41f;
                    }
                    this.rotation += 0.1f;
                    if (this.velocity.Y > 10f)
                    {
                        this.velocity.Y = 10f;
                    }
                }
                else if (this.aiStyle == 11)
                {
                    this.rotation += 0.02f;
                    if (Main.myPlayer == this.owner)
                    {
                        if (!Main.player[this.owner].dead)
                        {
                            float num53 = 4f;
                            Vector2 vector8 = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                            float num54 = (Main.player[this.owner].position.X + (Main.player[this.owner].width / 2)) - vector8.X;
                            float num55 = (Main.player[this.owner].position.Y + (Main.player[this.owner].height / 2)) - vector8.Y;
                            float num56 = (float) Math.Sqrt((double) ((num54 * num54) + (num55 * num55)));
                            num56 = (float) Math.Sqrt((double) ((num54 * num54) + (num55 * num55)));
                            if (num56 > Main.screenWidth)
                            {
                                this.position.X = (Main.player[this.owner].position.X + (Main.player[this.owner].width / 2)) - (this.width / 2);
                                this.position.Y = (Main.player[this.owner].position.Y + (Main.player[this.owner].height / 2)) - (this.height / 2);
                            }
                            else if (num56 > 64f)
                            {
                                num56 = num53 / num56;
                                num54 *= num56;
                                num55 *= num56;
                                if ((num54 != this.velocity.X) || (num55 != this.velocity.Y))
                                {
                                    this.netUpdate = true;
                                }
                                this.velocity.X = num54;
                                this.velocity.Y = num55;
                            }
                            else
                            {
                                if ((this.velocity.X != 0f) || (this.velocity.Y != 0f))
                                {
                                    this.netUpdate = true;
                                }
                                this.velocity.X = 0f;
                                this.velocity.Y = 0f;
                            }
                        }
                        else
                        {
                            this.Kill();
                        }
                    }
                }
                else if (this.aiStyle == 12)
                {
                    this.scale -= 0.05f;
                    if (this.scale <= 0f)
                    {
                        this.Kill();
                    }
                    if (this.ai[0] > 4f)
                    {
                        this.alpha = 150;
                        this.light = 0.8f;
                        color = new Color();
                        int num57 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 0x1d, this.velocity.X, this.velocity.Y, 100, color, 2.5f);
                        Main.dust[num57].noGravity = true;
                        Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 0x1d, this.velocity.X, this.velocity.Y, 100, new Color(), 1.5f);
                    }
                    else
                    {
                        this.ai[0]++;
                    }
                    this.rotation += 0.3f * this.direction;
                }
                else if (this.aiStyle == 13)
                {
                    if (Main.player[this.owner].dead)
                    {
                        this.Kill();
                    }
                    else
                    {
                        Main.player[this.owner].itemAnimation = 5;
                        Main.player[this.owner].itemTime = 5;
                        if ((this.position.X + (this.width / 2)) > (Main.player[this.owner].position.X + (Main.player[this.owner].width / 2)))
                        {
                            Main.player[this.owner].direction = 1;
                        }
                        else
                        {
                            Main.player[this.owner].direction = -1;
                        }
                        Vector2 vector9 = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                        float num58 = (Main.player[this.owner].position.X + (Main.player[this.owner].width / 2)) - vector9.X;
                        float num59 = (Main.player[this.owner].position.Y + (Main.player[this.owner].height / 2)) - vector9.Y;
                        float num60 = (float) Math.Sqrt((double) ((num58 * num58) + (num59 * num59)));
                        if (this.ai[0] != 0f)
                        {
                            if (this.ai[0] == 1f)
                            {
                                this.tileCollide = false;
                                this.rotation = ((float) Math.Atan2((double) num59, (double) num58)) - 1.57f;
                                float num61 = 11f;
                                if (num60 < 50f)
                                {
                                    this.Kill();
                                }
                                num60 = num61 / num60;
                                num58 *= num60;
                                num59 *= num60;
                                this.velocity.X = num58;
                                this.velocity.Y = num59;
                            }
                        }
                        else
                        {
                            if (num60 > 600f)
                            {
                                this.ai[0] = 1f;
                            }
                            this.rotation = ((float) Math.Atan2((double) this.velocity.Y, (double) this.velocity.X)) + 1.57f;
                            this.ai[1]++;
                            if (this.ai[1] > 2f)
                            {
                                this.alpha = 0;
                            }
                            if (this.ai[1] >= 10f)
                            {
                                this.ai[1] = 15f;
                                this.velocity.Y += 0.3f;
                            }
                        }
                    }
                }
                else if (this.aiStyle == 14)
                {
                    this.ai[0]++;
                    if (this.ai[0] > 5f)
                    {
                        this.ai[0] = 5f;
                        if ((this.velocity.Y == 0f) && (this.velocity.X != 0f))
                        {
                            this.velocity.X *= 0.97f;
                            if ((this.velocity.X > -0.01) && (this.velocity.X < 0.01))
                            {
                                this.velocity.X = 0f;
                                this.netUpdate = true;
                            }
                        }
                        this.velocity.Y += 0.2f;
                    }
                    this.rotation += this.velocity.X * 0.1f;
                }
                else if (this.aiStyle == 15)
                {
                    if (this.type == 0x19)
                    {
                        if (Main.rand.Next(15) == 0)
                        {
                            Dust.NewDust(this.position, this.width, this.height, 14, 0f, 0f, 150, new Color(), 1.3f);
                        }
                    }
                    else if (this.type == 0x1a)
                    {
                        int num62 = Dust.NewDust(this.position, this.width, this.height, 0x1d, this.velocity.X * 0.4f, this.velocity.Y * 0.4f, 100, new Color(), 2.5f);
                        Main.dust[num62].noGravity = true;
                        Main.dust[num62].velocity.X /= 2f;
                        Main.dust[num62].velocity.Y /= 2f;
                    }
                    else if (this.type == 0x23)
                    {
                        int num63 = Dust.NewDust(this.position, this.width, this.height, 6, this.velocity.X * 0.4f, this.velocity.Y * 0.4f, 100, new Color(), 3f);
                        Main.dust[num63].noGravity = true;
                        Main.dust[num63].velocity.X *= 2f;
                        Main.dust[num63].velocity.Y *= 2f;
                    }
                    if (Main.player[this.owner].dead)
                    {
                        this.Kill();
                    }
                    else
                    {
                        Main.player[this.owner].itemAnimation = 5;
                        Main.player[this.owner].itemTime = 5;
                        if ((this.position.X + (this.width / 2)) > (Main.player[this.owner].position.X + (Main.player[this.owner].width / 2)))
                        {
                            Main.player[this.owner].direction = 1;
                        }
                        else
                        {
                            Main.player[this.owner].direction = -1;
                        }
                        Vector2 vector10 = new Vector2(this.position.X + (this.width * 0.5f), this.position.Y + (this.height * 0.5f));
                        float num64 = (Main.player[this.owner].position.X + (Main.player[this.owner].width / 2)) - vector10.X;
                        float num65 = (Main.player[this.owner].position.Y + (Main.player[this.owner].height / 2)) - vector10.Y;
                        float num66 = (float) Math.Sqrt((double) ((num64 * num64) + (num65 * num65)));
                        if (this.ai[0] == 0f)
                        {
                            this.tileCollide = true;
                            if (num66 > 300f)
                            {
                                this.ai[0] = 1f;
                            }
                            else
                            {
                                this.ai[1]++;
                                if (this.ai[1] > 2f)
                                {
                                    this.alpha = 0;
                                }
                                if (this.ai[1] >= 5f)
                                {
                                    this.ai[1] = 15f;
                                    this.velocity.Y += 0.5f;
                                    this.velocity.X *= 0.95f;
                                }
                            }
                        }
                        else if (this.ai[0] == 1f)
                        {
                            this.tileCollide = false;
                            float num67 = 11f;
                            if (num66 < 20f)
                            {
                                this.Kill();
                            }
                            num66 = num67 / num66;
                            num64 *= num66;
                            num65 *= num66;
                            this.velocity.X = num64;
                            this.velocity.Y = num65;
                        }
                        this.rotation += this.velocity.X * 0.03f;
                    }
                }
                else if (this.aiStyle == 0x10)
                {
                    if (((this.owner == Main.myPlayer) && (this.timeLeft <= 3)) && (this.ai[1] == 0f))
                    {
                        this.ai[1] = 1f;
                        this.netUpdate = true;
                    }
                    if (this.type == 0x25)
                    {
                        try
                        {
                            int num68 = ((int) (this.position.X / 16f)) - 1;
                            int num69 = ((int) ((this.position.X + this.width) / 16f)) + 2;
                            int num70 = ((int) (this.position.Y / 16f)) - 1;
                            int num71 = ((int) ((this.position.Y + this.height) / 16f)) + 2;
                            if (num68 < 0)
                            {
                                num68 = 0;
                            }
                            if (num69 > Main.maxTilesX)
                            {
                                num69 = Main.maxTilesX;
                            }
                            if (num70 < 0)
                            {
                                num70 = 0;
                            }
                            if (num71 > Main.maxTilesY)
                            {
                                num71 = Main.maxTilesY;
                            }
                            for (int num72 = num68; num72 < num69; num72++)
                            {
                                for (int num73 = num70; num73 < num71; num73++)
                                {
                                    if (((Main.tile[num72, num73] != null) && Main.tile[num72, num73].active) && (Main.tileSolid[Main.tile[num72, num73].type] || (Main.tileSolidTop[Main.tile[num72, num73].type] && (Main.tile[num72, num73].frameY == 0))))
                                    {
                                        Vector2 vector11;
                                        vector11.X = num72 * 0x10;
                                        vector11.Y = num73 * 0x10;
                                        if (((((this.position.X + this.width) - 4f) > vector11.X) && ((this.position.X + 4f) < (vector11.X + 16f))) && ((((this.position.Y + this.height) - 4f) > vector11.Y) && ((this.position.Y + 4f) < (vector11.Y + 16f))))
                                        {
                                            this.velocity.X = 0f;
                                            this.velocity.Y = -0.2f;
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                        }
                    }
                    if (this.ai[1] > 0f)
                    {
                        this.alpha = 0xff;
                        if ((this.type == 0x1c) || (this.type == 0x25))
                        {
                            this.position.X += this.width / 2;
                            this.position.Y += this.height / 2;
                            this.width = 0x80;
                            this.height = 0x80;
                            this.position.X -= this.width / 2;
                            this.position.Y -= this.height / 2;
                            this.damage = 100;
                            this.knockBack = 8f;
                        }
                        else if (this.type == 0x1d)
                        {
                            this.position.X += this.width / 2;
                            this.position.Y += this.height / 2;
                            this.width = 250;
                            this.height = 250;
                            this.position.X -= this.width / 2;
                            this.position.Y -= this.height / 2;
                            this.damage = 250;
                            this.knockBack = 10f;
                        }
                        else if (this.type == 30)
                        {
                            this.position.X += this.width / 2;
                            this.position.Y += this.height / 2;
                            this.width = 0x80;
                            this.height = 0x80;
                            this.position.X -= this.width / 2;
                            this.position.Y -= this.height / 2;
                            this.knockBack = 8f;
                        }
                    }
                    else if ((this.type != 30) && (Main.rand.Next(4) == 0))
                    {
                        if (this.type != 30)
                        {
                            this.damage = 0;
                        }
                        Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, 0f, 0f, 100, new Color(), 1f);
                    }
                    this.ai[0]++;
                    if (((this.type == 30) && (this.ai[0] > 10f)) || ((this.type != 30) && (this.ai[0] > 5f)))
                    {
                        this.ai[0] = 10f;
                        if ((this.velocity.Y == 0f) && (this.velocity.X != 0f))
                        {
                            this.velocity.X *= 0.97f;
                            if (this.type == 0x1d)
                            {
                                this.velocity.X *= 0.99f;
                            }
                            if ((this.velocity.X > -0.01) && (this.velocity.X < 0.01))
                            {
                                this.velocity.X = 0f;
                                this.netUpdate = true;
                            }
                        }
                        this.velocity.Y += 0.2f;
                    }
                    this.rotation += this.velocity.X * 0.1f;
                }
            }
        }

        public Color GetAlpha(Color newColor)
        {
            int r;
            int g;
            int b;
            if (((this.type == 9) || (this.type == 15)) || (this.type == 0x22))
            {
                r = newColor.R - (this.alpha / 3);
                g = newColor.G - (this.alpha / 3);
                b = newColor.B - (this.alpha / 3);
            }
            else if ((this.type == 0x10) || (this.type == 0x12))
            {
                r = newColor.R;
                g = newColor.G;
                b = newColor.B;
            }
            else
            {
                r = newColor.R - this.alpha;
                g = newColor.G - this.alpha;
                b = newColor.B - this.alpha;
            }
            int a = newColor.A - this.alpha;
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

        public void Kill()
        {
            if (this.active)
            {
                Color color;
                if (this.type == 1)
                {
                    Main.PlaySound(0, (int) this.position.X, (int) this.position.Y, 1);
                    for (int i = 0; i < 10; i++)
                    {
                        color = new Color();
                        Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 7, 0f, 0f, 0, color, 1f);
                    }
                }
                else if (this.type == 2)
                {
                    Main.PlaySound(0, (int) this.position.X, (int) this.position.Y, 1);
                    for (int j = 0; j < 20; j++)
                    {
                        Color newColor = new Color();
                        Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, 0f, 0f, 100, newColor, 1f);
                    }
                }
                else if (this.type == 3)
                {
                    Main.PlaySound(0, (int) this.position.X, (int) this.position.Y, 1);
                    for (int k = 0; k < 10; k++)
                    {
                        Color color3 = new Color();
                        Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 1, this.velocity.X * 0.1f, this.velocity.Y * 0.1f, 0, color3, 0.75f);
                    }
                }
                else if (this.type == 4)
                {
                    Main.PlaySound(0, (int) this.position.X, (int) this.position.Y, 1);
                    for (int m = 0; m < 10; m++)
                    {
                        Color color4 = new Color();
                        Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 14, 0f, 0f, 150, color4, 1.1f);
                    }
                }
                else if (this.type == 5)
                {
                    Main.PlaySound(2, (int) this.position.X, (int) this.position.Y, 10);
                    for (int n = 0; n < 60; n++)
                    {
                        Color color5 = new Color();
                        Dust.NewDust(this.position, this.width, this.height, 15, this.velocity.X * 0.5f, this.velocity.Y * 0.5f, 150, color5, 1.5f);
                    }
                }
                else if ((this.type == 9) || (this.type == 12))
                {
                    Main.PlaySound(2, (int) this.position.X, (int) this.position.Y, 10);
                    for (int num6 = 0; num6 < 10; num6++)
                    {
                        Color color6 = new Color();
                        Dust.NewDust(this.position, this.width, this.height, 15, this.velocity.X * 0.1f, this.velocity.Y * 0.1f, 150, color6, 1.2f);
                    }
                    for (int num7 = 0; num7 < 3; num7++)
                    {
                        Gore.NewGore(this.position, new Vector2(this.velocity.X * 0.05f, this.velocity.Y * 0.05f), Main.rand.Next(0x10, 0x12));
                    }
                    if ((this.type == 12) && (this.damage < 100))
                    {
                        for (int num8 = 0; num8 < 10; num8++)
                        {
                            Color color7 = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 15, this.velocity.X * 0.1f, this.velocity.Y * 0.1f, 150, color7, 1.2f);
                        }
                        for (int num9 = 0; num9 < 3; num9++)
                        {
                            Gore.NewGore(this.position, new Vector2(this.velocity.X * 0.05f, this.velocity.Y * 0.05f), Main.rand.Next(0x10, 0x12));
                        }
                    }
                }
                else if (((this.type == 14) || (this.type == 20)) || (this.type == 0x24))
                {
                    Collision.HitTiles(this.position, this.velocity, this.width, this.height);
                    Main.PlaySound(2, (int) this.position.X, (int) this.position.Y, 10);
                }
                else if ((this.type == 15) || (this.type == 0x22))
                {
                    Main.PlaySound(2, (int) this.position.X, (int) this.position.Y, 10);
                    for (int num10 = 0; num10 < 20; num10++)
                    {
                        Color color8 = new Color();
                        int index = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, -this.velocity.X * 0.2f, -this.velocity.Y * 0.2f, 100, color8, 2f);
                        Main.dust[index].noGravity = true;
                        Dust dust1 = Main.dust[index];
                        dust1.velocity = (Vector2) (dust1.velocity * 2f);
                        color = new Color();
                        index = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, -this.velocity.X * 0.2f, -this.velocity.Y * 0.2f, 100, color, 1f);
                        Dust dust2 = Main.dust[index];
                        dust2.velocity = (Vector2) (dust2.velocity * 2f);
                    }
                }
                else if (this.type == 0x10)
                {
                    Main.PlaySound(2, (int) this.position.X, (int) this.position.Y, 10);
                    for (int num12 = 0; num12 < 20; num12++)
                    {
                        color = new Color();
                        int num13 = Dust.NewDust(new Vector2(this.position.X - this.velocity.X, this.position.Y - this.velocity.Y), this.width, this.height, 15, 0f, 0f, 100, color, 2f);
                        Main.dust[num13].noGravity = true;
                        Dust dust3 = Main.dust[num13];
                        dust3.velocity = (Vector2) (dust3.velocity * 2f);
                        color = new Color();
                        num13 = Dust.NewDust(new Vector2(this.position.X - this.velocity.X, this.position.Y - this.velocity.Y), this.width, this.height, 15, 0f, 0f, 100, color, 1f);
                    }
                }
                else if (this.type == 0x11)
                {
                    Main.PlaySound(0, (int) this.position.X, (int) this.position.Y, 1);
                    for (int num14 = 0; num14 < 5; num14++)
                    {
                        color = new Color();
                        Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 0, 0f, 0f, 0, color, 1f);
                    }
                }
                else if (this.type == 0x1f)
                {
                    Main.PlaySound(0, (int) this.position.X, (int) this.position.Y, 1);
                    for (int num15 = 0; num15 < 5; num15++)
                    {
                        color = new Color();
                        int num16 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 0x20, 0f, 0f, 0, color, 1f);
                        Dust dust4 = Main.dust[num16];
                        dust4.velocity = (Vector2) (dust4.velocity * 0.6f);
                    }
                }
                else if (this.type == 0x15)
                {
                    Main.PlaySound(0, (int) this.position.X, (int) this.position.Y, 1);
                    for (int num17 = 0; num17 < 10; num17++)
                    {
                        color = new Color();
                        Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 0x1a, 0f, 0f, 0, color, 0.8f);
                    }
                }
                else if (this.type == 0x18)
                {
                    for (int num18 = 0; num18 < 10; num18++)
                    {
                        color = new Color();
                        Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 1, this.velocity.X * 0.1f, this.velocity.Y * 0.1f, 0, color, 0.75f);
                    }
                }
                else if (this.type == 0x1b)
                {
                    Main.PlaySound(2, (int) this.position.X, (int) this.position.Y, 10);
                    for (int num19 = 0; num19 < 30; num19++)
                    {
                        color = new Color();
                        int num20 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 0x1d, this.velocity.X * 0.1f, this.velocity.Y * 0.1f, 100, color, 3f);
                        Main.dust[num20].noGravity = true;
                        color = new Color();
                        Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 0x1d, this.velocity.X * 0.1f, this.velocity.Y * 0.1f, 100, color, 2f);
                    }
                }
                else
                {
                    Vector2 vector;
                    if (((this.type == 0x1c) || (this.type == 30)) || (this.type == 0x25))
                    {
                        Main.PlaySound(2, (int) this.position.X, (int) this.position.Y, 14);
                        this.position.X += this.width / 2;
                        this.position.Y += this.height / 2;
                        this.width = 0x16;
                        this.height = 0x16;
                        this.position.X -= this.width / 2;
                        this.position.Y -= this.height / 2;
                        for (int num21 = 0; num21 < 20; num21++)
                        {
                            color = new Color();
                            int num22 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 0x1f, 0f, 0f, 100, color, 1.5f);
                            Dust dust5 = Main.dust[num22];
                            dust5.velocity = (Vector2) (dust5.velocity * 1.4f);
                        }
                        for (int num23 = 0; num23 < 10; num23++)
                        {
                            color = new Color();
                            int num24 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, 0f, 0f, 100, color, 2.5f);
                            Main.dust[num24].noGravity = true;
                            Dust dust6 = Main.dust[num24];
                            dust6.velocity = (Vector2) (dust6.velocity * 5f);
                            color = new Color();
                            num24 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, 0f, 0f, 100, color, 1.5f);
                            Dust dust7 = Main.dust[num24];
                            dust7.velocity = (Vector2) (dust7.velocity * 3f);
                        }
                        vector = new Vector2();
                        int num25 = Gore.NewGore(new Vector2(this.position.X, this.position.Y), vector, Main.rand.Next(0x3d, 0x40));
                        Gore gore1 = Main.gore[num25];
                        gore1.velocity = (Vector2) (gore1.velocity * 0.4f);
                        Main.gore[num25].velocity.X++;
                        Main.gore[num25].velocity.Y++;
                        vector = new Vector2();
                        num25 = Gore.NewGore(new Vector2(this.position.X, this.position.Y), vector, Main.rand.Next(0x3d, 0x40));
                        Gore gore2 = Main.gore[num25];
                        gore2.velocity = (Vector2) (gore2.velocity * 0.4f);
                        Main.gore[num25].velocity.X--;
                        Main.gore[num25].velocity.Y++;
                        vector = new Vector2();
                        num25 = Gore.NewGore(new Vector2(this.position.X, this.position.Y), vector, Main.rand.Next(0x3d, 0x40));
                        Gore gore3 = Main.gore[num25];
                        gore3.velocity = (Vector2) (gore3.velocity * 0.4f);
                        Main.gore[num25].velocity.X++;
                        Main.gore[num25].velocity.Y--;
                        num25 = Gore.NewGore(new Vector2(this.position.X, this.position.Y), new Vector2(), Main.rand.Next(0x3d, 0x40));
                        Gore gore4 = Main.gore[num25];
                        gore4.velocity = (Vector2) (gore4.velocity * 0.4f);
                        Main.gore[num25].velocity.X--;
                        Main.gore[num25].velocity.Y--;
                    }
                    else if (this.type == 0x1d)
                    {
                        Main.PlaySound(2, (int) this.position.X, (int) this.position.Y, 14);
                        this.position.X += this.width / 2;
                        this.position.Y += this.height / 2;
                        this.width = 200;
                        this.height = 200;
                        this.position.X -= this.width / 2;
                        this.position.Y -= this.height / 2;
                        for (int num26 = 0; num26 < 50; num26++)
                        {
                            color = new Color();
                            int num27 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 0x1f, 0f, 0f, 100, color, 2f);
                            Dust dust8 = Main.dust[num27];
                            dust8.velocity = (Vector2) (dust8.velocity * 1.4f);
                        }
                        for (int num28 = 0; num28 < 80; num28++)
                        {
                            color = new Color();
                            int num29 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, 0f, 0f, 100, color, 3f);
                            Main.dust[num29].noGravity = true;
                            Dust dust9 = Main.dust[num29];
                            dust9.velocity = (Vector2) (dust9.velocity * 5f);
                            color = new Color();
                            num29 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, 0f, 0f, 100, color, 2f);
                            Dust dust10 = Main.dust[num29];
                            dust10.velocity = (Vector2) (dust10.velocity * 3f);
                        }
                        for (int num31 = 0; num31 < 2; num31++)
                        {
                            vector = new Vector2();
                            int num30 = Gore.NewGore(new Vector2((this.position.X + (this.width / 2)) - 24f, (this.position.Y + (this.height / 2)) - 24f), vector, Main.rand.Next(0x3d, 0x40));
                            Main.gore[num30].scale = 1.5f;
                            Main.gore[num30].velocity.X += 1.5f;
                            Main.gore[num30].velocity.Y += 1.5f;
                            vector = new Vector2();
                            num30 = Gore.NewGore(new Vector2((this.position.X + (this.width / 2)) - 24f, (this.position.Y + (this.height / 2)) - 24f), vector, Main.rand.Next(0x3d, 0x40));
                            Main.gore[num30].scale = 1.5f;
                            Main.gore[num30].velocity.X -= 1.5f;
                            Main.gore[num30].velocity.Y += 1.5f;
                            vector = new Vector2();
                            num30 = Gore.NewGore(new Vector2((this.position.X + (this.width / 2)) - 24f, (this.position.Y + (this.height / 2)) - 24f), vector, Main.rand.Next(0x3d, 0x40));
                            Main.gore[num30].scale = 1.5f;
                            Main.gore[num30].velocity.X += 1.5f;
                            Main.gore[num30].velocity.Y -= 1.5f;
                            vector = new Vector2();
                            num30 = Gore.NewGore(new Vector2((this.position.X + (this.width / 2)) - 24f, (this.position.Y + (this.height / 2)) - 24f), vector, Main.rand.Next(0x3d, 0x40));
                            Main.gore[num30].scale = 1.5f;
                            Main.gore[num30].velocity.X -= 1.5f;
                            Main.gore[num30].velocity.Y -= 1.5f;
                        }
                        this.position.X += this.width / 2;
                        this.position.Y += this.height / 2;
                        this.width = 10;
                        this.height = 10;
                        this.position.X -= this.width / 2;
                        this.position.Y -= this.height / 2;
                    }
                }
                if (this.owner == Main.myPlayer)
                {
                    if (((this.type == 0x1c) || (this.type == 0x1d)) || (this.type == 0x25) && (true == false))
                    {
                        int num32 = 3;
                        if (this.type == 0x1d)
                        {
                            num32 = 7;
                        }
                        int num33 = ((int) (this.position.X / 16f)) - num32;
                        int maxTilesX = ((int) (this.position.X / 16f)) + num32;
                        int num35 = ((int) (this.position.Y / 16f)) - num32;
                        int maxTilesY = ((int) (this.position.Y / 16f)) + num32;
                        if (num33 < 0)
                        {
                            num33 = 0;
                        }
                        if (maxTilesX > Main.maxTilesX)
                        {
                            maxTilesX = Main.maxTilesX;
                        }
                        if (num35 < 0)
                        {
                            num35 = 0;
                        }
                        if (maxTilesY > Main.maxTilesY)
                        {
                            maxTilesY = Main.maxTilesY;
                        }
                        bool flag = false;
                        for (int num37 = num33; num37 <= maxTilesX; num37++)
                        {
                            for (int num38 = num35; num38 <= maxTilesY; num38++)
                            {
                                float num39 = Math.Abs((float) (num37 - (this.position.X / 16f)));
                                float num40 = Math.Abs((float) (num38 - (this.position.Y / 16f)));
                                if (((Math.Sqrt((double) ((num39 * num39) + (num40 * num40))) < num32) && (Main.tile[num37, num38] != null)) && (Main.tile[num37, num38].wall == 0))
                                {
                                    flag = true;
                                    goto Label_1605;
                                }
                            }
                        Label_1605:;
                        }
                        for (int num42 = num33; num42 <= maxTilesX; num42++)
                        {
                            for (int num43 = num35; num43 <= maxTilesY; num43++)
                            {
                                float num44 = Math.Abs((float) (num42 - (this.position.X / 16f)));
                                float num45 = Math.Abs((float) (num43 - (this.position.Y / 16f)));
                                if (Math.Sqrt((double) ((num44 * num44) + (num45 * num45))) < num32)
                                {
                                    bool flag2 = true;
                                    if ((Main.tile[num42, num43] != null) && Main.tile[num42, num43].active)
                                    {
                                        flag2 = false;
                                        if ((this.type == 0x1c) || (this.type == 0x25))
                                        {
                                            if (((((!Main.tileSolid[Main.tile[num42, num43].type] || Main.tileSolidTop[Main.tile[num42, num43].type]) || ((Main.tile[num42, num43].type == 0) || (Main.tile[num42, num43].type == 1))) || (((Main.tile[num42, num43].type == 2) || (Main.tile[num42, num43].type == 0x17)) || ((Main.tile[num42, num43].type == 30) || (Main.tile[num42, num43].type == 40)))) || ((((Main.tile[num42, num43].type == 6) || (Main.tile[num42, num43].type == 7)) || ((Main.tile[num42, num43].type == 8) || (Main.tile[num42, num43].type == 9))) || (((Main.tile[num42, num43].type == 10) || (Main.tile[num42, num43].type == 0x35)) || ((Main.tile[num42, num43].type == 0x36) || (Main.tile[num42, num43].type == 0x39))))) || ((((Main.tile[num42, num43].type == 0x3b) || (Main.tile[num42, num43].type == 60)) || ((Main.tile[num42, num43].type == 0x3f) || (Main.tile[num42, num43].type == 0x40))) || ((((Main.tile[num42, num43].type == 0x41) || (Main.tile[num42, num43].type == 0x42)) || ((Main.tile[num42, num43].type == 0x43) || (Main.tile[num42, num43].type == 0x44))) || ((Main.tile[num42, num43].type == 70) || (Main.tile[num42, num43].type == 0x25)))))
                                            {
                                                flag2 = true;
                                            }
                                        }
                                        else if (this.type == 0x1d)
                                        {
                                            flag2 = true;
                                        }
                                        if ((Main.tileDungeon[Main.tile[num42, num43].type] || (Main.tile[num42, num43].type == 0x1a)) || ((Main.tile[num42, num43].type == 0x3a) || (Main.tile[num42, num43].type == 0x15)))
                                        {
                                            flag2 = false;
                                        }
                                        if (flag2)
                                        {
                                            WorldGen.KillTile(num42, num43, false, false, false);
                                            if (!Main.tile[num42, num43].active && (Main.netMode == 1))
                                            {
                                                NetMessage.SendData(0x11, -1, -1, "", 0, (float) num42, (float) num43, 0f);
                                            }
                                        }
                                    }
                                    if ((flag2 && (Main.tile[num42, num43] != null)) && ((Main.tile[num42, num43].wall > 0) && flag))
                                    {
                                        WorldGen.KillWall(num42, num43, false);
                                        if ((Main.tile[num42, num43].wall == 0) && (Main.netMode == 1))
                                        {
                                            NetMessage.SendData(0x11, -1, -1, "", 2, (float) num42, (float) num43, 0f);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (Main.netMode != 0)
                    {
                        NetMessage.SendData(0x1d, -1, -1, "", this.identity, (float) this.owner, 0f, 0f);
                    }
                    int number = -1;
                    if (this.aiStyle == 10)
                    {
                        int num48 = (((int) this.position.X) + (this.width / 2)) / 0x10;
                        int num49 = (((int) this.position.Y) + (this.width / 2)) / 0x10;
                        int type = 0;
                        int num51 = 2;
                        if (this.type == 0x1f)
                        {
                            type = 0x35;
                            num51 = 0xa9;
                        }
                        if (!Main.tile[num48, num49].active)
                        {
                            WorldGen.PlaceTile(num48, num49, type, false, true, -1);
                            if (Main.tile[num48, num49].active && (Main.tile[num48, num49].type == type))
                            {
                                NetMessage.SendData(0x11, -1, -1, "", 1, (float) num48, (float) num49, (float) type);
                            }
                            else
                            {
                                number = Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, num51, 1, false);
                            }
                        }
                        else
                        {
                            number = Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, num51, 1, false);
                        }
                    }
                    if ((this.type == 1) && (Main.rand.Next(3) < 2))
                    {
                        number = Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 40, 1, false);
                    }
                    if ((this.type == 2) && (Main.rand.Next(5) < 4))
                    {
                        if (Main.rand.Next(4) == 0)
                        {
                            number = Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x29, 1, false);
                        }
                        else
                        {
                            number = Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 40, 1, false);
                        }
                    }
                    if ((this.type == 3) && (Main.rand.Next(5) < 4))
                    {
                        number = Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x2a, 1, false);
                    }
                    if ((this.type == 4) && (Main.rand.Next(5) < 4))
                    {
                        number = Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x2f, 1, false);
                    }
                    if ((this.type == 12) && (this.damage > 100))
                    {
                        number = Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x4b, 1, false);
                    }
                    if ((this.type == 0x15) && (Main.rand.Next(5) < 4))
                    {
                        number = Item.NewItem((int) this.position.X, (int) this.position.Y, this.width, this.height, 0x9a, 1, false);
                    }
                    if ((Main.netMode == 1) && (number >= 0))
                    {
                        NetMessage.SendData(0x15, -1, -1, "", number, 0f, 0f, 0f);
                    }
                }
                this.active = false;
            }
        }

        public static int NewProjectile(float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner = 8)
        {
            int index = 0x3e8;
            for (int i = 0; i < 0x3e8; i++)
            {
                if (!Main.projectile[i].active)
                {
                    index = i;
                    break;
                }
            }
            if (index != 0x3e8)
            {
                Main.projectile[index].SetDefaults(Type);
                Main.projectile[index].position.X = X - (Main.projectile[index].width * 0.5f);
                Main.projectile[index].position.Y = Y - (Main.projectile[index].height * 0.5f);
                Main.projectile[index].owner = Owner;
                Main.projectile[index].velocity.X = SpeedX;
                Main.projectile[index].velocity.Y = SpeedY;
                Main.projectile[index].damage = Damage;
                Main.projectile[index].knockBack = KnockBack;
                Main.projectile[index].identity = index;
                Main.projectile[index].wet = Collision.WetCollision(Main.projectile[index].position, Main.projectile[index].width, Main.projectile[index].height);
                if ((Main.netMode != 0) && (Owner == Main.myPlayer))
                {
                    NetMessage.SendData(0x1b, -1, -1, "", index, 0f, 0f, 0f);
                }
                if (Owner != Main.myPlayer)
                {
                    return index;
                }
                if (Type == 0x1c)
                {
                    Main.projectile[index].timeLeft = 180;
                }
                if (Type == 0x1d)
                {
                    Main.projectile[index].timeLeft = 300;
                }
                if (Type == 30)
                {
                    Main.projectile[index].timeLeft = 180;
                }
                if (Type == 0x25)
                {
                    Main.projectile[index].timeLeft = 180;
                }
            }
            return index;
        }

        public void SetDefaults(int Type)
        {
            for (int i = 0; i < maxAI; i++)
            {
                this.ai[i] = 0f;
            }
            for (int j = 0; j < 8; j++)
            {
                this.playerImmune[j] = 0;
            }
            this.lavaWet = false;
            this.wetCount = 0;
            this.wet = false;
            this.ignoreWater = false;
            this.hostile = false;
            this.netUpdate = false;
            this.numUpdates = 0;
            this.maxUpdates = 0;
            this.identity = 0;
            this.restrikeDelay = 0;
            this.light = 0f;
            this.penetrate = 1;
            this.tileCollide = true;
            this.position = new Vector2();
            this.velocity = new Vector2();
            this.aiStyle = 0;
            this.alpha = 0;
            this.type = Type;
            this.active = true;
            this.rotation = 0f;
            this.scale = 1f;
            this.owner = 8;
            this.timeLeft = 0xe10;
            this.name = "";
            this.friendly = false;
            this.damage = 0;
            this.knockBack = 0f;
            if (this.type == 1)
            {
                this.name = "Wooden Arrow";
                this.width = 10;
                this.height = 10;
                this.aiStyle = 1;
                this.friendly = true;
            }
            else if (this.type == 2)
            {
                this.name = "Fire Arrow";
                this.width = 10;
                this.height = 10;
                this.aiStyle = 1;
                this.friendly = true;
                this.light = 1f;
            }
            else if (this.type == 3)
            {
                this.name = "Shuriken";
                this.width = 0x16;
                this.height = 0x16;
                this.aiStyle = 2;
                this.friendly = true;
                this.penetrate = 4;
            }
            else if (this.type == 4)
            {
                this.name = "Unholy Arrow";
                this.width = 10;
                this.height = 10;
                this.aiStyle = 1;
                this.friendly = true;
                this.light = 0.2f;
                this.penetrate = 3;
            }
            else if (this.type == 5)
            {
                this.name = "Jester's Arrow";
                this.width = 10;
                this.height = 10;
                this.aiStyle = 1;
                this.friendly = true;
                this.light = 0.4f;
                this.penetrate = -1;
                this.timeLeft = 40;
                this.alpha = 100;
                this.ignoreWater = true;
            }
            else if (this.type == 6)
            {
                this.name = "Enchanted Boomerang";
                this.width = 0x16;
                this.height = 0x16;
                this.aiStyle = 3;
                this.friendly = true;
                this.penetrate = -1;
            }
            else if ((this.type == 7) || (this.type == 8))
            {
                this.name = "Vilethorn";
                this.width = 0x1c;
                this.height = 0x1c;
                this.aiStyle = 4;
                this.friendly = true;
                this.penetrate = -1;
                this.tileCollide = false;
                this.alpha = 0xff;
                this.ignoreWater = true;
            }
            else if (this.type == 9)
            {
                this.name = "Starfury";
                this.width = 0x18;
                this.height = 0x18;
                this.aiStyle = 5;
                this.friendly = true;
                this.penetrate = 2;
                this.alpha = 50;
                this.scale = 0.8f;
                this.light = 1f;
            }
            else if (this.type == 10)
            {
                this.name = "Purification Powder";
                this.width = 0x40;
                this.height = 0x40;
                this.aiStyle = 6;
                this.friendly = true;
                this.tileCollide = false;
                this.penetrate = -1;
                this.alpha = 0xff;
                this.ignoreWater = true;
            }
            else if (this.type == 11)
            {
                this.name = "Vile Powder";
                this.width = 0x30;
                this.height = 0x30;
                this.aiStyle = 6;
                this.friendly = true;
                this.tileCollide = false;
                this.penetrate = -1;
                this.alpha = 0xff;
                this.ignoreWater = true;
            }
            else if (this.type == 12)
            {
                this.name = "Fallen Star";
                this.width = 0x10;
                this.height = 0x10;
                this.aiStyle = 5;
                this.friendly = true;
                this.penetrate = -1;
                this.alpha = 50;
                this.light = 1f;
            }
            else if (this.type == 13)
            {
                this.name = "Hook";
                this.width = 0x12;
                this.height = 0x12;
                this.aiStyle = 7;
                this.friendly = true;
                this.penetrate = -1;
                this.tileCollide = false;
            }
            else if (this.type == 14)
            {
                this.name = "Musket Ball";
                this.width = 4;
                this.height = 4;
                this.aiStyle = 1;
                this.friendly = true;
                this.penetrate = 1;
                this.light = 0.5f;
                this.alpha = 0xff;
                this.maxUpdates = 1;
                this.scale = 1.2f;
                this.timeLeft = 600;
            }
            else if (this.type == 15)
            {
                this.name = "Ball of Fire";
                this.width = 0x10;
                this.height = 0x10;
                this.aiStyle = 8;
                this.friendly = true;
                this.light = 0.8f;
                this.alpha = 100;
            }
            else if (this.type == 0x10)
            {
                this.name = "Magic Missile";
                this.width = 10;
                this.height = 10;
                this.aiStyle = 9;
                this.friendly = true;
                this.light = 0.8f;
                this.alpha = 100;
            }
            else if (this.type == 0x11)
            {
                this.name = "Dirt Ball";
                this.width = 10;
                this.height = 10;
                this.aiStyle = 10;
                this.friendly = true;
            }
            else if (this.type == 0x12)
            {
                this.name = "Orb of Light";
                this.width = 0x20;
                this.height = 0x20;
                this.aiStyle = 11;
                this.friendly = true;
                this.light = 1f;
                this.alpha = 150;
                this.tileCollide = false;
                this.penetrate = -1;
                this.timeLeft *= 5;
                this.ignoreWater = true;
            }
            else if (this.type == 0x13)
            {
                this.name = "Flamarang";
                this.width = 0x16;
                this.height = 0x16;
                this.aiStyle = 3;
                this.friendly = true;
                this.penetrate = -1;
                this.light = 1f;
            }
            else if (this.type == 20)
            {
                this.name = "Green Laser";
                this.width = 4;
                this.height = 4;
                this.aiStyle = 1;
                this.friendly = true;
                this.penetrate = -1;
                this.light = 0.75f;
                this.alpha = 0xff;
                this.maxUpdates = 2;
                this.scale = 1.4f;
                this.timeLeft = 600;
            }
            else if (this.type == 0x15)
            {
                this.name = "Bone";
                this.width = 0x10;
                this.height = 0x10;
                this.aiStyle = 2;
                this.scale = 1.2f;
                this.friendly = true;
            }
            else if (this.type == 0x16)
            {
                this.name = "Water Stream";
                this.width = 12;
                this.height = 12;
                this.aiStyle = 12;
                this.friendly = true;
                this.alpha = 0xff;
                this.penetrate = -1;
                this.maxUpdates = 1;
                this.ignoreWater = true;
            }
            else if (this.type == 0x17)
            {
                this.name = "Harpoon";
                this.width = 4;
                this.height = 4;
                this.aiStyle = 13;
                this.friendly = true;
                this.penetrate = -1;
                this.alpha = 0xff;
            }
            else if (this.type == 0x18)
            {
                this.name = "Spiky Ball";
                this.width = 14;
                this.height = 14;
                this.aiStyle = 14;
                this.friendly = true;
                this.penetrate = 3;
            }
            else if (this.type == 0x19)
            {
                this.name = "Ball 'O Hurt";
                this.width = 0x16;
                this.height = 0x16;
                this.aiStyle = 15;
                this.friendly = true;
                this.penetrate = -1;
            }
            else if (this.type == 0x1a)
            {
                this.name = "Blue Moon";
                this.width = 0x16;
                this.height = 0x16;
                this.aiStyle = 15;
                this.friendly = true;
                this.penetrate = -1;
            }
            else if (this.type == 0x1b)
            {
                this.name = "Water Bolt";
                this.width = 0x10;
                this.height = 0x10;
                this.aiStyle = 8;
                this.friendly = true;
                this.light = 0.8f;
                this.alpha = 200;
                this.timeLeft /= 2;
                this.penetrate = 10;
            }
            else if (this.type == 0x1c)
            {
                this.name = "Bomb";
                this.width = 0x16;
                this.height = 0x16;
                this.aiStyle = 0x10;
                this.friendly = true;
                this.penetrate = -1;
            }
            else if (this.type == 0x1d)
            {
                this.name = "Dynamite";
                this.width = 10;
                this.height = 10;
                this.aiStyle = 0x10;
                this.friendly = true;
                this.penetrate = -1;
            }
            else if (this.type == 30)
            {
                this.name = "Grenade";
                this.width = 14;
                this.height = 14;
                this.aiStyle = 0x10;
                this.friendly = true;
                this.penetrate = -1;
            }
            else if (this.type == 0x1f)
            {
                this.name = "Sand Ball";
                this.knockBack = 6f;
                this.width = 10;
                this.height = 10;
                this.aiStyle = 10;
                this.friendly = true;
                this.hostile = true;
                this.penetrate = -1;
            }
            else if (this.type == 0x20)
            {
                this.name = "Ivy Whip";
                this.width = 0x12;
                this.height = 0x12;
                this.aiStyle = 7;
                this.friendly = true;
                this.penetrate = -1;
                this.tileCollide = false;
            }
            else if (this.type == 0x21)
            {
                this.name = "Thorn Chakrum";
                this.width = 0x1c;
                this.height = 0x1c;
                this.aiStyle = 3;
                this.friendly = true;
                this.scale = 0.9f;
                this.penetrate = -1;
            }
            else if (this.type == 0x22)
            {
                this.name = "Flamelash";
                this.width = 14;
                this.height = 14;
                this.aiStyle = 9;
                this.friendly = true;
                this.light = 0.8f;
                this.alpha = 100;
                this.penetrate = 2;
            }
            else if (this.type == 0x23)
            {
                this.name = "Sunfury";
                this.width = 0x16;
                this.height = 0x16;
                this.aiStyle = 15;
                this.friendly = true;
                this.penetrate = -1;
            }
            else if (this.type == 0x24)
            {
                this.name = "Meteor Shot";
                this.width = 4;
                this.height = 4;
                this.aiStyle = 1;
                this.friendly = true;
                this.penetrate = 2;
                this.light = 0.6f;
                this.alpha = 0xff;
                this.maxUpdates = 1;
                this.scale = 1.4f;
                this.timeLeft = 600;
            }
            else if (this.type == 0x25)
            {
                this.name = "Bomb";
                this.width = 0x16;
                this.height = 0x16;
                this.aiStyle = 0x10;
                this.friendly = true;
                this.penetrate = -1;
                this.tileCollide = false;
            }
            else
            {
                this.active = false;
            }
            this.width = (int) (this.width * this.scale);
            this.height = (int) (this.height * this.scale);
        }

        public void Update(int i)
        {
            if (this.active)
            {
                Vector2 velocity = this.velocity;
                if (((this.position.X <= Main.leftWorld) || ((this.position.X + this.width) >= Main.rightWorld)) || ((this.position.Y <= Main.topWorld) || ((this.position.Y + this.height) >= Main.bottomWorld)))
                {
                    this.Kill();
                }
                else
                {
                    this.whoAmI = i;
                    if (this.soundDelay > 0)
                    {
                        this.soundDelay--;
                    }
                    this.netUpdate = false;
                    for (int j = 0; j < 8; j++)
                    {
                        if (this.playerImmune[j] > 0)
                        {
                            this.playerImmune[j]--;
                        }
                    }
                    this.AI();
                    if ((this.owner < 8) && !Main.player[this.owner].active)
                    {
                        this.Kill();
                    }
                    if (!this.ignoreWater)
                    {
                        bool flag = Collision.LavaCollision(this.position, this.width, this.height);
                        if (flag)
                        {
                            this.lavaWet = true;
                        }
                        if (Collision.WetCollision(this.position, this.width, this.height))
                        {
                            if (this.wetCount == 0)
                            {
                                this.wetCount = 10;
                                if (!this.wet)
                                {
                                    if (!flag)
                                    {
                                        for (int k = 0; k < 10; k++)
                                        {
                                            Color newColor = new Color();
                                            int index = Dust.NewDust(new Vector2(this.position.X - 6f, (this.position.Y + (this.height / 2)) - 8f), this.width + 12, 0x18, 0x21, 0f, 0f, 0, newColor, 1f);
                                            Main.dust[index].velocity.Y -= 4f;
                                            Main.dust[index].velocity.X *= 2.5f;
                                            Main.dust[index].scale = 1.3f;
                                            Main.dust[index].alpha = 100;
                                            Main.dust[index].noGravity = true;
                                        }
                                        Main.PlaySound(0x13, (int) this.position.X, (int) this.position.Y, 1);
                                    }
                                    else
                                    {
                                        for (int m = 0; m < 10; m++)
                                        {
                                            Color color2 = new Color();
                                            int num5 = Dust.NewDust(new Vector2(this.position.X - 6f, (this.position.Y + (this.height / 2)) - 8f), this.width + 12, 0x18, 0x23, 0f, 0f, 0, color2, 1f);
                                            Main.dust[num5].velocity.Y -= 1.5f;
                                            Main.dust[num5].velocity.X *= 2.5f;
                                            Main.dust[num5].scale = 1.3f;
                                            Main.dust[num5].alpha = 100;
                                            Main.dust[num5].noGravity = true;
                                        }
                                        Main.PlaySound(0x13, (int) this.position.X, (int) this.position.Y, 1);
                                    }
                                }
                                this.wet = true;
                            }
                        }
                        else if (this.wet)
                        {
                            this.wet = false;
                            if (this.wetCount == 0)
                            {
                                this.wetCount = 10;
                                if (!this.lavaWet)
                                {
                                    for (int n = 0; n < 10; n++)
                                    {
                                        Color color3 = new Color();
                                        int num7 = Dust.NewDust(new Vector2(this.position.X - 6f, this.position.Y + (this.height / 2)), this.width + 12, 0x18, 0x21, 0f, 0f, 0, color3, 1f);
                                        Main.dust[num7].velocity.Y -= 4f;
                                        Main.dust[num7].velocity.X *= 2.5f;
                                        Main.dust[num7].scale = 1.3f;
                                        Main.dust[num7].alpha = 100;
                                        Main.dust[num7].noGravity = true;
                                    }
                                    Main.PlaySound(0x13, (int) this.position.X, (int) this.position.Y, 1);
                                }
                                else
                                {
                                    for (int num8 = 0; num8 < 10; num8++)
                                    {
                                        Color color4 = new Color();
                                        int num9 = Dust.NewDust(new Vector2(this.position.X - 6f, (this.position.Y + (this.height / 2)) - 8f), this.width + 12, 0x18, 0x23, 0f, 0f, 0, color4, 1f);
                                        Main.dust[num9].velocity.Y -= 1.5f;
                                        Main.dust[num9].velocity.X *= 2.5f;
                                        Main.dust[num9].scale = 1.3f;
                                        Main.dust[num9].alpha = 100;
                                        Main.dust[num9].noGravity = true;
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
                    }
                    if (this.tileCollide)
                    {
                        Vector2 vector2 = this.velocity;
                        bool fallThrough = true;
                        if (((this.type == 9) || (this.type == 12)) || (((this.type == 15) || (this.type == 13)) || (this.type == 0x1f)))
                        {
                            fallThrough = false;
                        }
                        if (this.aiStyle == 10)
                        {
                            this.velocity = Collision.AnyCollision(this.position, this.velocity, this.width, this.height);
                        }
                        else if (this.wet)
                        {
                            Vector2 vector3 = this.velocity;
                            this.velocity = Collision.TileCollision(this.position, this.velocity, this.width, this.height, fallThrough, fallThrough);
                            velocity = (Vector2) (this.velocity * 0.5f);
                            if (this.velocity.X != vector3.X)
                            {
                                velocity.X = this.velocity.X;
                            }
                            if (this.velocity.Y != vector3.Y)
                            {
                                velocity.Y = this.velocity.Y;
                            }
                        }
                        else
                        {
                            this.velocity = Collision.TileCollision(this.position, this.velocity, this.width, this.height, fallThrough, fallThrough);
                        }
                        if (vector2 != this.velocity)
                        {
                            if (this.type == 0x24)
                            {
                                if (this.penetrate > 1)
                                {
                                    Collision.HitTiles(this.position, this.velocity, this.width, this.height);
                                    Main.PlaySound(2, (int) this.position.X, (int) this.position.Y, 10);
                                    this.penetrate--;
                                    if (this.velocity.X != vector2.X)
                                    {
                                        this.velocity.X = -vector2.X;
                                    }
                                    if (this.velocity.Y != vector2.Y)
                                    {
                                        this.velocity.Y = -vector2.Y;
                                    }
                                }
                                else
                                {
                                    this.Kill();
                                }
                            }
                            else if (((this.aiStyle == 3) || (this.aiStyle == 13)) || (this.aiStyle == 15))
                            {
                                Collision.HitTiles(this.position, this.velocity, this.width, this.height);
                                if (this.type == 0x21)
                                {
                                    if (this.velocity.X != vector2.X)
                                    {
                                        this.velocity.X = -vector2.X;
                                    }
                                    if (this.velocity.Y != vector2.Y)
                                    {
                                        this.velocity.Y = -vector2.Y;
                                    }
                                }
                                else
                                {
                                    this.ai[0] = 1f;
                                    if (this.aiStyle == 3)
                                    {
                                        this.velocity.X = -vector2.X;
                                        this.velocity.Y = -vector2.Y;
                                    }
                                }
                                this.netUpdate = true;
                                Main.PlaySound(0, (int) this.position.X, (int) this.position.Y, 1);
                            }
                            else if (this.aiStyle == 8)
                            {
                                Main.PlaySound(2, (int) this.position.X, (int) this.position.Y, 10);
                                this.ai[0]++;
                                if (this.ai[0] >= 5f)
                                {
                                    this.position += this.velocity;
                                    this.Kill();
                                }
                                else
                                {
                                    if (this.velocity.Y != vector2.Y)
                                    {
                                        this.velocity.Y = -vector2.Y;
                                    }
                                    if (this.velocity.X != vector2.X)
                                    {
                                        this.velocity.X = -vector2.X;
                                    }
                                }
                            }
                            else if (this.aiStyle == 14)
                            {
                                if (this.velocity.X != vector2.X)
                                {
                                    this.velocity.X = vector2.X * -0.5f;
                                }
                                if ((this.velocity.Y != vector2.Y) && (vector2.Y > 1f))
                                {
                                    this.velocity.Y = vector2.Y * -0.5f;
                                }
                            }
                            else if (this.aiStyle == 0x10)
                            {
                                if (this.velocity.X != vector2.X)
                                {
                                    this.velocity.X = vector2.X * -0.4f;
                                    if (this.type == 0x1d)
                                    {
                                        this.velocity.X *= 0.8f;
                                    }
                                }
                                if ((this.velocity.Y != vector2.Y) && (vector2.Y > 0.7))
                                {
                                    this.velocity.Y = vector2.Y * -0.4f;
                                    if (this.type == 0x1d)
                                    {
                                        this.velocity.Y *= 0.8f;
                                    }
                                }
                            }
                            else
                            {
                                this.position += this.velocity;
                                this.Kill();
                            }
                        }
                    }
                    if ((this.type != 7) && (this.type != 8))
                    {
                        if (this.wet)
                        {
                            this.position += velocity;
                        }
                        else
                        {
                            this.position += this.velocity;
                        }
                    }
                    if ((((this.aiStyle != 3) || (this.ai[0] != 1f)) && ((this.aiStyle != 7) || (this.ai[0] != 1f))) && (((this.aiStyle != 13) || (this.ai[0] != 1f)) && ((this.aiStyle != 15) || (this.ai[0] != 1f))))
                    {
                        if (this.velocity.X < 0f)
                        {
                            this.direction = -1;
                        }
                        else
                        {
                            this.direction = 1;
                        }
                    }
                    if (this.active)
                    {
                        if (this.light > 0f)
                        {
                            Lighting.addLight((int) ((this.position.X + (this.width / 2)) / 16f), (int) ((this.position.Y + (this.height / 2)) / 16f), this.light);
                        }
                        if (this.type == 2)
                        {
                            Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, 0f, 0f, 100, new Color(), 1f);
                        }
                        else if (this.type == 4)
                        {
                            if (Main.rand.Next(5) == 0)
                            {
                                Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 14, 0f, 0f, 150, new Color(), 1.1f);
                            }
                        }
                        else if (this.type == 5)
                        {
                            Dust.NewDust(this.position, this.width, this.height, 15, this.velocity.X * 0.5f, this.velocity.Y * 0.5f, 150, new Color(), 1.2f);
                        }
                        Rectangle rectangle = new Rectangle((int) this.position.X, (int) this.position.Y, this.width, this.height);
                        if ((this.hostile && (Main.myPlayer < 8)) && (this.damage > 0))
                        {
                            int myPlayer = Main.myPlayer;
                            if ((Main.player[myPlayer].active && !Main.player[myPlayer].dead) && !Main.player[myPlayer].immune)
                            {
                                Rectangle rectangle2 = new Rectangle((int) Main.player[myPlayer].position.X, (int) Main.player[myPlayer].position.Y, Main.player[myPlayer].width, Main.player[myPlayer].height);
                                if (rectangle.Intersects(rectangle2))
                                {
                                    int direction = this.direction;
                                    if ((Main.player[myPlayer].position.X + (Main.player[myPlayer].width / 2)) < (this.position.X + (this.width / 2)))
                                    {
                                        direction = -1;
                                    }
                                    else
                                    {
                                        direction = 1;
                                    }
                                    Main.player[myPlayer].Hurt(this.damage * 2, direction, false, false);
                                    if (Main.netMode != 0)
                                    {
                                        NetMessage.SendData(0x1a, -1, -1, "", myPlayer, (float) this.direction, (float) (this.damage * 2), 0f);
                                    }
                                }
                            }
                        }
                        if ((this.friendly && (this.type != 0x12)) && (this.owner == Main.myPlayer))
                        {
                            if ((this.aiStyle == 0x10) && (this.ai[1] > 0f))
                            {
                                for (int num12 = 0; num12 < 8; num12++)
                                {
                                    if ((Main.player[num12].active && !Main.player[num12].dead) && !Main.player[num12].immune)
                                    {
                                        Rectangle rectangle3 = new Rectangle((int) Main.player[num12].position.X, (int) Main.player[num12].position.Y, Main.player[num12].width, Main.player[num12].height);
                                        if (rectangle.Intersects(rectangle3))
                                        {
                                            if ((Main.player[num12].position.X + (Main.player[num12].width / 2)) < (this.position.X + (this.width / 2)))
                                            {
                                                this.direction = -1;
                                            }
                                            else
                                            {
                                                this.direction = 1;
                                            }
                                            Main.player[num12].Hurt(this.damage, this.direction, true, false);
                                            if (Main.netMode != 0)
                                            {
                                                NetMessage.SendData(0x1a, -1, -1, "", num12, (float) this.direction, (float) this.damage, 1f);
                                            }
                                        }
                                    }
                                }
                            }
                            int num13 = (int) (this.position.X / 16f);
                            int maxTilesX = ((int) ((this.position.X + this.width) / 16f)) + 1;
                            int num15 = (int) (this.position.Y / 16f);
                            int maxTilesY = ((int) ((this.position.Y + this.height) / 16f)) + 1;
                            if (num13 < 0)
                            {
                                num13 = 0;
                            }
                            if (maxTilesX > Main.maxTilesX)
                            {
                                maxTilesX = Main.maxTilesX;
                            }
                            if (num15 < 0)
                            {
                                num15 = 0;
                            }
                            if (maxTilesY > Main.maxTilesY)
                            {
                                maxTilesY = Main.maxTilesY;
                            }
                            for (int num17 = num13; num17 < maxTilesX; num17++)
                            {
                                for (int num18 = num15; num18 < maxTilesY; num18++)
                                {
                                    if ((Main.tile[num17, num18] != null) && (((((Main.tile[num17, num18].type == 3) || (Main.tile[num17, num18].type == 0x18)) || ((Main.tile[num17, num18].type == 0x1c) || (Main.tile[num17, num18].type == 0x20))) || (((Main.tile[num17, num18].type == 0x33) || (Main.tile[num17, num18].type == 0x34)) || ((Main.tile[num17, num18].type == 0x3d) || (Main.tile[num17, num18].type == 0x3e)))) || (((Main.tile[num17, num18].type == 0x45) || (Main.tile[num17, num18].type == 0x47)) || ((Main.tile[num17, num18].type == 0x49) || (Main.tile[num17, num18].type == 0x4a)))))
                                    {
                                        WorldGen.KillTile(num17, num18, false, false, false);
                                        if (Main.netMode == 1)
                                        {
                                            NetMessage.SendData(0x11, -1, -1, "", 0, (float) num17, (float) num18, 0f);
                                        }
                                    }
                                }
                            }
                            if (this.damage > 0)
                            {
                                for (int num19 = 0; num19 < 0x3e8; num19++)
                                {
                                    if ((Main.npc[num19].active && !Main.npc[num19].friendly) && ((this.owner < 0) || (Main.npc[num19].immune[this.owner] == 0)))
                                    {
                                        Rectangle rectangle4 = new Rectangle((int) Main.npc[num19].position.X, (int) Main.npc[num19].position.Y, Main.npc[num19].width, Main.npc[num19].height);
                                        if (rectangle.Intersects(rectangle4))
                                        {
                                            if (this.aiStyle == 3)
                                            {
                                                if (this.ai[0] == 0f)
                                                {
                                                    this.velocity.X = -this.velocity.X;
                                                    this.velocity.Y = -this.velocity.Y;
                                                    this.netUpdate = true;
                                                }
                                                this.ai[0] = 1f;
                                            }
                                            else if (this.aiStyle == 0x10)
                                            {
                                                if (this.timeLeft > 3)
                                                {
                                                    this.timeLeft = 3;
                                                }
                                                if ((Main.npc[num19].position.X + (Main.npc[num19].width / 2)) < (this.position.X + (this.width / 2)))
                                                {
                                                    this.direction = -1;
                                                }
                                                else
                                                {
                                                    this.direction = 1;
                                                }
                                            }
                                            Main.npc[num19].StrikeNPC(this.damage, this.knockBack, this.direction);
                                            if (Main.netMode != 0)
                                            {
                                                NetMessage.SendData(0x1c, -1, -1, "", num19, (float) this.damage, this.knockBack, (float) this.direction);
                                            }
                                            if (this.penetrate != 1)
                                            {
                                                Main.npc[num19].immune[this.owner] = 10;
                                            }
                                            if (this.penetrate > 0)
                                            {
                                                this.penetrate--;
                                                if (this.penetrate == 0)
                                                {
                                                    break;
                                                }
                                            }
                                            if (this.aiStyle == 7)
                                            {
                                                this.ai[0] = 1f;
                                                this.damage = 0;
                                                this.netUpdate = true;
                                            }
                                            else if (this.aiStyle == 13)
                                            {
                                                this.ai[0] = 1f;
                                                this.netUpdate = true;
                                            }
                                        }
                                    }
                                }
                            }
                            if ((this.damage > 0) && Main.player[Main.myPlayer].hostile)
                            {
                                for (int num20 = 0; num20 < 8; num20++)
                                {
                                    if ((((num20 != this.owner) && Main.player[num20].active) && (!Main.player[num20].dead && !Main.player[num20].immune)) && ((Main.player[num20].hostile && (this.playerImmune[num20] <= 0)) && ((Main.player[Main.myPlayer].team == 0) || (Main.player[Main.myPlayer].team != Main.player[num20].team))))
                                    {
                                        Rectangle rectangle5 = new Rectangle((int) Main.player[num20].position.X, (int) Main.player[num20].position.Y, Main.player[num20].width, Main.player[num20].height);
                                        if (rectangle.Intersects(rectangle5))
                                        {
                                            if (this.aiStyle == 3)
                                            {
                                                if (this.ai[0] == 0f)
                                                {
                                                    this.velocity.X = -this.velocity.X;
                                                    this.velocity.Y = -this.velocity.Y;
                                                    this.netUpdate = true;
                                                }
                                                this.ai[0] = 1f;
                                            }
                                            else if (this.aiStyle == 0x10)
                                            {
                                                if (this.timeLeft > 3)
                                                {
                                                    this.timeLeft = 3;
                                                }
                                                if ((Main.player[num20].position.X + (Main.player[num20].width / 2)) < (this.position.X + (this.width / 2)))
                                                {
                                                    this.direction = -1;
                                                }
                                                else
                                                {
                                                    this.direction = 1;
                                                }
                                            }
                                            Main.player[num20].Hurt(this.damage, this.direction, true, false);
                                            if (Main.netMode != 0)
                                            {
                                                NetMessage.SendData(0x1a, -1, -1, "", num20, (float) this.direction, (float) this.damage, 1f);
                                            }
                                            this.playerImmune[num20] = 40;
                                            if (this.penetrate > 0)
                                            {
                                                this.penetrate--;
                                                if (this.penetrate == 0)
                                                {
                                                    break;
                                                }
                                            }
                                            if (this.aiStyle == 7)
                                            {
                                                this.ai[0] = 1f;
                                                this.damage = 0;
                                                this.netUpdate = true;
                                            }
                                            else if (this.aiStyle == 13)
                                            {
                                                this.ai[0] = 1f;
                                                this.netUpdate = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        this.timeLeft--;
                        if (this.timeLeft <= 0)
                        {
                            this.Kill();
                        }
                        if (this.penetrate == 0)
                        {
                            this.Kill();
                        }
                        if ((this.active && this.netUpdate) && (this.owner == Main.myPlayer))
                        {
                            NetMessage.SendData(0x1b, -1, -1, "", i, 0f, 0f, 0f);
                        }
                        if (this.active && (this.maxUpdates > 0))
                        {
                            this.numUpdates--;
                            if (this.numUpdates >= 0)
                            {
                                this.Update(i);
                            }
                            else
                            {
                                this.numUpdates = this.maxUpdates;
                            }
                        }
                        this.netUpdate = false;
                    }
                }
            }
        }
    }
}

