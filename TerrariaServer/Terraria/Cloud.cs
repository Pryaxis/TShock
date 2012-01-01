
using System;
namespace Terraria
{
	public class Cloud
	{
		public Vector2 position;
		public float scale;
		public float rotation;
		public float rSpeed;
		public float sSpeed;
		public bool active;
		public int type;
		public int width;
		public int height;
		private static Random rand = new Random();
		public static void resetClouds()
		{
			if (Main.cloudLimit < 10)
			{
				return;
			}
			Main.numClouds = Cloud.rand.Next(10, Main.cloudLimit);
			Main.windSpeed = 0f;
			while (Main.windSpeed == 0f)
			{
				Main.windSpeed = (float)Cloud.rand.Next(-100, 101) * 0.01f;
			}
			for (int i = 0; i < 100; i++)
			{
				Main.cloud[i].active = false;
			}
			for (int j = 0; j < Main.numClouds; j++)
			{
				Cloud.addCloud();
			}
			for (int k = 0; k < Main.numClouds; k++)
			{
				if (Main.windSpeed < 0f)
				{
					Cloud expr_9D_cp_0 = Main.cloud[k];
					expr_9D_cp_0.position.X = expr_9D_cp_0.position.X - (float)(Main.screenWidth * 2);
				}
				else
				{
					Cloud expr_BF_cp_0 = Main.cloud[k];
					expr_BF_cp_0.position.X = expr_BF_cp_0.position.X + (float)(Main.screenWidth * 2);
				}
			}
		}
		public static void addCloud()
		{
			int num = -1;
			for (int i = 0; i < 100; i++)
			{
				if (!Main.cloud[i].active)
				{
					num = i;
					break;
				}
			}
			if (num >= 0)
			{
				Main.cloud[num].rSpeed = 0f;
				Main.cloud[num].sSpeed = 0f;
				Main.cloud[num].type = Cloud.rand.Next(4);
				Main.cloud[num].scale = (float)Cloud.rand.Next(70, 131) * 0.01f;
				Main.cloud[num].rotation = (float)Cloud.rand.Next(-10, 11) * 0.01f;
				float num2 = Main.windSpeed;
				if (!Main.gameMenu)
				{
					num2 = Main.windSpeed - Main.player[Main.myPlayer].velocity.X * 0.1f;
				}
				if (num2 > 0f)
				{
				
				}
				else
				{
				
				}
				Main.cloud[num].position.Y = (float)Cloud.rand.Next((int)((float)(-(float)Main.screenHeight) * 0.25f), (int)((float)Main.screenHeight * 0.25f));
				Cloud expr_210_cp_0 = Main.cloud[num];
				expr_210_cp_0.position.Y = expr_210_cp_0.position.Y - (float)Cloud.rand.Next((int)((float)Main.screenHeight * 0.15f));
				Cloud expr_240_cp_0 = Main.cloud[num];
				expr_240_cp_0.position.Y = expr_240_cp_0.position.Y - (float)Cloud.rand.Next((int)((float)Main.screenHeight * 0.15f));
				if ((double)Main.cloud[num].scale > 1.3)
				{
					Main.cloud[num].scale = 1.3f;
				}
				if ((double)Main.cloud[num].scale < 0.7)
				{
					Main.cloud[num].scale = 0.7f;
				}
				Main.cloud[num].active = true;
				Rectangle rectangle = new Rectangle((int)Main.cloud[num].position.X, (int)Main.cloud[num].position.Y, Main.cloud[num].width, Main.cloud[num].height);
				for (int j = 0; j < 100; j++)
				{
					if (num != j && Main.cloud[j].active)
					{
						Rectangle value = new Rectangle((int)Main.cloud[j].position.X, (int)Main.cloud[j].position.Y, Main.cloud[j].width, Main.cloud[j].height);
						if (rectangle.Intersects(value))
						{
							Main.cloud[num].active = false;
						}
					}
				}
			}
		}
		public Color cloudColor(Color bgColor)
		{
			float num = (this.scale - 0.4f) * 0.9f;
			float num2 = 1.1f;
			float num3 = 255f - (float)(255 - bgColor.R) * num2;
			float num4 = 255f - (float)(255 - bgColor.G) * num2;
			float num5 = 255f - (float)(255 - bgColor.B) * num2;
			float num6 = 255f;
			num3 *= num;
			num4 *= num;
			num5 *= num;
			num6 *= num;
			if (num3 < 0f)
			{
				num3 = 0f;
			}
			if (num4 < 0f)
			{
				num4 = 0f;
			}
			if (num5 < 0f)
			{
				num5 = 0f;
			}
			if (num6 < 0f)
			{
				num6 = 0f;
			}
			return new Color((int)((byte)num3), (int)((byte)num4), (int)((byte)num5), (int)((byte)num6));
		}
		public object Clone()
		{
			return base.MemberwiseClone();
		}
		public static void UpdateClouds()
		{
			int num = 0;
			for (int i = 0; i < 100; i++)
			{
				if (Main.cloud[i].active)
				{
					Main.cloud[i].rotation = 0f;
					Main.cloud[i].Update();
					num++;
				}
			}
			for (int j = 0; j < 100; j++)
			{
				if (Main.cloud[j].active)
				{
					if (j > 1 && (!Main.cloud[j - 1].active || (double)Main.cloud[j - 1].scale > (double)Main.cloud[j].scale + 0.02))
					{
						Cloud cloud = (Cloud)Main.cloud[j - 1].Clone();
						Main.cloud[j - 1] = (Cloud)Main.cloud[j].Clone();
						Main.cloud[j] = cloud;
					}
					if (j < 99 && (!Main.cloud[j].active || (double)Main.cloud[j + 1].scale < (double)Main.cloud[j].scale - 0.02))
					{
						Cloud cloud2 = (Cloud)Main.cloud[j + 1].Clone();
						Main.cloud[j + 1] = (Cloud)Main.cloud[j].Clone();
						Main.cloud[j] = cloud2;
					}
				}
			}
			if (num < Main.numClouds)
			{
				Cloud.addCloud();
			}
		}
        public void Update()
        {
        }
	}
}
