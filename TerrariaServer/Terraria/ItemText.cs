
using System;
namespace Terraria
{
	public class ItemText
	{
		public Vector2 position;
		public Vector2 velocity;
		public float alpha;
		public int alphaDir = 1;
		public string name;
		public int stack;
		public float scale = 1f;
		public float rotation;
		public Color color;
		public bool active;
		public int lifeTime;
		public static int activeTime = 60;
		public static int numActive;
		public static void NewText(Item newItem, int stack)
		{
			if (!Main.showItemText)
			{
				return;
			}
			if (newItem.name == null || !newItem.active)
			{
				return;
			}
			if (Main.netMode == 2)
			{
				return;
			}
		}
		public void Update(int whoAmI)
		{
		}
		public static void UpdateItemText()
		{
			int num = 0;
			for (int i = 0; i < 20; i++)
			{
				if (Main.itemText[i].active)
				{
					num++;
					Main.itemText[i].Update(i);
				}
			}
			ItemText.numActive = num;
		}
	}
}
