
using System;
namespace Terraria
{
	public class CombatText
	{
		public Vector2 position;
		public Vector2 velocity;
		public float alpha;
		public int alphaDir = 1;
		public string text;
		public float scale = 1f;
		public float rotation;
		public Color color;
		public bool active;
		public int lifeTime;
		public bool crit;
		public static void NewText(Rectangle location, Color color, string text, bool Crit = false)
		{
			if (Main.netMode == 2)
			{
				return;
			}
		}
		public void Update()
		{
			if (this.active)
			{
				this.alpha += (float)this.alphaDir * 0.05f;
				if ((double)this.alpha <= 0.6)
				{
					this.alphaDir = 1;
				}
				if (this.alpha >= 1f)
				{
					this.alpha = 1f;
					this.alphaDir = -1;
				}
				this.velocity.Y = this.velocity.Y * 0.92f;
				if (this.crit)
				{
					this.velocity.Y = this.velocity.Y * 0.92f;
				}
				this.velocity.X = this.velocity.X * 0.93f;
				this.position += this.velocity;
				this.lifeTime--;
				if (this.lifeTime <= 0)
				{
					this.scale -= 0.1f;
					if ((double)this.scale < 0.1)
					{
						this.active = false;
					}
					this.lifeTime = 0;
					if (this.crit)
					{
						this.alphaDir = -1;
						this.scale += 0.07f;
						return;
					}
				}
				else
				{
					if (this.crit)
					{
						if (this.velocity.X < 0f)
						{
							this.rotation += 0.001f;
						}
						else
						{
							this.rotation -= 0.001f;
						}
					}
					if (this.scale < 1f)
					{
						this.scale += 0.1f;
					}
					if (this.scale > 1f)
					{
						this.scale = 1f;
					}
				}
			}
		}
		public static void UpdateCombatText()
		{
			for (int i = 0; i < 100; i++)
			{
				if (Main.combatText[i].active)
				{
					Main.combatText[i].Update();
				}
			}
		}
	}
}
