
using System;
namespace Terraria
{
	public class Gore
	{
		public static int goreTime = 600;
		public Vector2 position;
		public Vector2 velocity;
		public float rotation;
		public float scale;
		public int alpha;
		public int type;
		public float light;
		public bool active;
		public bool sticky = true;
		public int timeLeft = Gore.goreTime;
		public void Update()
        {
		}
		public static int NewGore(Vector2 Position, Vector2 Velocity, int Type, float Scale = 1f)
		{
			if (Main.rand == null)
			{
				Main.rand = new Random();
			}
			if (Main.netMode == 2)
			{
				return 0;
			}
			int num = 200;
			for (int i = 0; i < 200; i++)
			{
				if (!Main.gore[i].active)
				{
					num = i;
					break;
				}
			}
			if (num == 200)
			{
				return num;
			}
			Main.gore[num].light = 0f;
			Main.gore[num].position = Position;
			Main.gore[num].velocity = Velocity;
			Gore expr_84_cp_0 = Main.gore[num];
			expr_84_cp_0.velocity.Y = expr_84_cp_0.velocity.Y - (float)Main.rand.Next(10, 31) * 0.1f;
			Gore expr_B1_cp_0 = Main.gore[num];
			expr_B1_cp_0.velocity.X = expr_B1_cp_0.velocity.X + (float)Main.rand.Next(-20, 21) * 0.1f;
			Main.gore[num].type = Type;
			Main.gore[num].active = true;
			Main.gore[num].alpha = 0;
			Main.gore[num].rotation = 0f;
			Main.gore[num].scale = Scale;
			if (Gore.goreTime == 0 || Type == 11 || Type == 12 || Type == 13 || Type == 16 || Type == 17 || Type == 61 || Type == 62 || Type == 63 || Type == 99)
			{
				Main.gore[num].sticky = false;
			}
			else
			{
				Main.gore[num].sticky = true;
				Main.gore[num].timeLeft = Gore.goreTime;
			}
			if (Type == 16 || Type == 17)
			{
				Main.gore[num].alpha = 100;
				Main.gore[num].scale = 0.7f;
				Main.gore[num].light = 1f;
			}
			return num;
		}
		public Color GetAlpha(Color newColor)
		{
			float num = (float)(255 - this.alpha) / 255f;
			int r;
			int g;
			int b;
			if (this.type == 16 || this.type == 17)
			{
				r = (int)newColor.R;
				g = (int)newColor.G;
				b = (int)newColor.B;
			}
			else
			{
				r = (int)((float)newColor.R * num);
				g = (int)((float)newColor.G * num);
				b = (int)((float)newColor.B * num);
			}
			int num2 = (int)newColor.A - this.alpha;
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num2 > 255)
			{
				num2 = 255;
			}
			return new Color(r, g, b, num2);
		}
	}
}
