namespace Terraria
{
    using Microsoft.Xna.Framework;
    using System;

    public class Gore
    {
        public bool active;
        public int alpha;
        public static int goreTime = 600;
        public float light;
        public Vector2 position;
        public float rotation;
        public float scale;
        public bool sticky = true;
        public int timeLeft = goreTime;
        public int type;
        public Vector2 velocity;

        public Color GetAlpha(Color newColor)
        {
            int num;
            int num2;
            int num3;
            if ((this.type == 0x10) || (this.type == 0x11))
            {
                num = newColor.R - (this.alpha / 2);
                num2 = newColor.G - (this.alpha / 2);
                num3 = newColor.B - (this.alpha / 2);
            }
            else
            {
                num = newColor.R - this.alpha;
                num2 = newColor.G - this.alpha;
                num3 = newColor.B - this.alpha;
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
            return new Color(num, num2, num3, a);
        }

        public static int NewGore(Vector2 Position, Vector2 Velocity, int Type)
        {
            if (Main.rand == null)
            {
                Main.rand = new Random();
            }
            if (Main.netMode == 2)
            {
                return 0;
            }
            int index = 200;
            for (int i = 0; i < 200; i++)
            {
                if (!Main.gore[i].active)
                {
                    index = i;
                    break;
                }
            }
            if (index != 200)
            {
                Main.gore[index].light = 0f;
                Main.gore[index].position = Position;
                Main.gore[index].velocity = Velocity;
                Main.gore[index].velocity.Y -= Main.rand.Next(10, 0x1f) * 0.1f;
                Main.gore[index].velocity.X += Main.rand.Next(-20, 0x15) * 0.1f;
                Main.gore[index].type = Type;
                Main.gore[index].active = true;
                Main.gore[index].alpha = 0;
                Main.gore[index].rotation = 0f;
                Main.gore[index].scale = 1f;
                if ((((goreTime == 0) || (Type == 11)) || ((Type == 12) || (Type == 13))) || (((Type == 0x10) || (Type == 0x11)) || (((Type == 0x3d) || (Type == 0x3e)) || (Type == 0x3f))))
                {
                    Main.gore[index].sticky = false;
                }
                else
                {
                    Main.gore[index].sticky = true;
                    Main.gore[index].timeLeft = goreTime;
                }
                if ((Type == 0x10) || (Type == 0x11))
                {
                    Main.gore[index].alpha = 100;
                    Main.gore[index].scale = 0.7f;
                    Main.gore[index].light = 1f;
                }
            }
            return index;
        }

        public void Update()
        {
            if (this.active)
            {
                if ((((this.type == 11) || (this.type == 12)) || ((this.type == 13) || (this.type == 0x3d))) || ((this.type == 0x3e) || (this.type == 0x3f)))
                {
                    this.velocity.Y *= 0.98f;
                    this.velocity.X *= 0.98f;
                    this.scale -= 0.007f;
                    if (this.scale < 0.1)
                    {
                        this.scale = 0.1f;
                        this.alpha = 0xff;
                    }
                }
                else if ((this.type == 0x10) || (this.type == 0x11))
                {
                    this.velocity.Y *= 0.98f;
                    this.velocity.X *= 0.98f;
                    this.scale -= 0.01f;
                    if (this.scale < 0.1)
                    {
                        this.scale = 0.1f;
                        this.alpha = 0xff;
                    }
                }
                else
                {
                    this.velocity.Y += 0.2f;
                }
                this.rotation += this.velocity.X * 0.1f;
                if (this.sticky)
                {
                    int width = Main.goreTexture[this.type].Width;
                    if (Main.goreTexture[this.type].Height < width)
                    {
                        width = Main.goreTexture[this.type].Height;
                    }
                    width = (int) (width * 0.9f);
                    this.velocity = Collision.TileCollision(this.position, this.velocity, (int) (width * this.scale), (int) (width * this.scale), false, false);
                    if (this.velocity.Y == 0f)
                    {
                        this.velocity.X *= 0.97f;
                        if ((this.velocity.X > -0.01) && (this.velocity.X < 0.01))
                        {
                            this.velocity.X = 0f;
                        }
                    }
                    if (this.timeLeft > 0)
                    {
                        this.timeLeft--;
                    }
                    else
                    {
                        this.alpha++;
                    }
                }
                else
                {
                    this.alpha += 2;
                }
                this.position += this.velocity;
                if (this.alpha >= 0xff)
                {
                    this.active = false;
                }
                if (this.light > 0f)
                {
                    Lighting.addLight((int) ((this.position.X + ((Main.goreTexture[this.type].Width * this.scale) / 2f)) / 16f), (int) ((this.position.Y + ((Main.goreTexture[this.type].Height * this.scale) / 2f)) / 16f), this.light);
                }
            }
        }
    }
}

