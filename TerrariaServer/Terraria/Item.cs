using Hooks;
using System;
namespace Terraria
{
	public class Item
	{
		public static int potionDelay = 3600;
		public static int[] headType = new int[45];
		public static int[] bodyType = new int[26];
		public static int[] legType = new int[25];
		public bool mech;
		public bool wet;
		public byte wetCount;
		public bool lavaWet;
		public Vector2 position;
		public Vector2 velocity;
		public int width;
		public int height;
		public bool active;
		public int noGrabDelay;
		public bool beingGrabbed;
		public int spawnTime;
		public bool wornArmor;
		public int ownIgnore = -1;
		public int ownTime;
		public int keepTime;
		public int type;
		public string name;
		public int holdStyle;
		public int useStyle;
		public bool channel;
		public bool accessory;
		public int useAnimation;
		public int useTime;
		public int stack;
		public int maxStack;
		public int pick;
		public int axe;
		public int hammer;
		public int tileBoost;
		public int createTile = -1;
		public int createWall = -1;
		public int placeStyle;
		public int damage;
		public float knockBack;
		public int healLife;
		public int healMana;
		public bool potion;
		public bool consumable;
		public bool autoReuse;
		public bool useTurn;
		public Color color;
		public int alpha;
		public float scale = 1f;
		public int useSound;
		public int defense;
		public int headSlot = -1;
		public int bodySlot = -1;
		public int legSlot = -1;
		public string toolTip;
		public string toolTip2;
		public int owner = 255;
		public int rare;
		public int shoot;
		public float shootSpeed;
		public int ammo;
		public int useAmmo;
		public int lifeRegen;
		public int manaIncrease;
		public bool buyOnce;
		public int mana;
		public bool noUseGraphic;
		public bool noMelee;
		public int release;
		public int value;
		public bool buy;
		public bool social;
		public bool vanity;
		public bool material;
		public bool noWet;
		public int buffType;
		public int buffTime;
		public int netID;
		public int crit;
		public byte prefix;
		public bool melee;
		public bool magic;
		public bool ranged;
		public int reuseDelay;
		public bool Prefix(int pre)
		{
			if (pre == 0 || this.type == 0)
			{
				return false;
			}
			int num = pre;
			float num2 = 1f;
			float num3 = 1f;
			float num4 = 1f;
			float num5 = 1f;
			float num6 = 1f;
			float num7 = 1f;
			int num8 = 0;
			bool flag = true;
			while (flag)
			{
				num2 = 1f;
				num3 = 1f;
				num4 = 1f;
				num5 = 1f;
				num6 = 1f;
				num7 = 1f;
				num8 = 0;
				flag = false;
				if (num == -1 && Main.rand.Next(4) == 0)
				{
					num = 0;
				}
				if (pre < -1)
				{
					num = -1;
				}
				if (num == -1 || num == -2 || num == -3)
				{
					switch (this.type)
					{
					    case 484:
					    case 483:
					    case 482:
					    case 426:
					    case 368:
					    case 367:
					    case 273:
					    case 217:
					    case 213:
					    case 204:
					    case 203:
					    case 202:
					    case 201:
					    case 200:
					    case 199:
					    case 198:
					    case 196:
					    case 190:
					    case 155:
					    case 122:
					    case 121:
					    case 104:
					    case 103:
					    case 46:
					    case 45:
					    case 24:
					    case 10:
					    case 7:
					    case 6:
					    case 4:
					    case 1:
					        {
					            int num9 = Main.rand.Next(40);
					            switch (num9)
					            {
					                case 0:
					                    num = 1;
					                    break;
					                case 1:
					                    num = 2;
					                    break;
					                case 2:
					                    num = 3;
					                    break;
					                case 3:
					                    num = 4;
					                    break;
					                case 4:
					                    num = 5;
					                    break;
					                case 5:
					                    num = 6;
					                    break;
					                case 6:
					                    num = 7;
					                    break;
					                case 7:
					                    num = 8;
					                    break;
					                case 8:
					                    num = 9;
					                    break;
					                case 9:
					                    num = 10;
					                    break;
					                case 10:
					                    num = 11;
					                    break;
					                case 11:
					                    num = 12;
					                    break;
					                case 12:
					                    num = 13;
					                    break;
					                case 13:
					                    num = 14;
					                    break;
					                case 14:
					                    num = 15;
					                    break;
					                case 15:
					                    num = 36;
					                    break;
					                case 16:
					                    num = 37;
					                    break;
					                case 17:
					                    num = 38;
					                    break;
					                case 18:
					                    num = 53;
					                    break;
					                case 19:
					                    num = 54;
					                    break;
					                case 20:
					                    num = 55;
					                    break;
					                case 21:
					                    num = 39;
					                    break;
					                case 22:
					                    num = 40;
					                    break;
					                case 23:
					                    num = 56;
					                    break;
					                case 24:
					                    num = 41;
					                    break;
					                case 25:
					                    num = 57;
					                    break;
					                case 26:
					                    num = 42;
					                    break;
					                case 27:
					                    num = 43;
					                    break;
					                case 28:
					                    num = 44;
					                    break;
					                case 29:
					                    num = 45;
					                    break;
					                case 30:
					                    num = 46;
					                    break;
					                case 31:
					                    num = 47;
					                    break;
					                case 32:
					                    num = 48;
					                    break;
					                case 33:
					                    num = 49;
					                    break;
					                case 34:
					                    num = 50;
					                    break;
					                case 35:
					                    num = 51;
					                    break;
					                case 36:
					                    num = 59;
					                    break;
					                case 37:
					                    num = 60;
					                    break;
					                case 38:
					                    num = 61;
					                    break;
					                case 39:
					                    num = 81;
					                    break;
					            }
					        }
					        break;
					    case 579:
					    case 550:
					    case 537:
					    case 406:
					    case 390:
					    case 389:
					    case 388:
					    case 387:
					    case 386:
					    case 385:
					    case 384:
					    case 383:
					    case 280:
					    case 277:
					    case 274:
					    case 220:
					    case 163:
					    case 160:
					    case 162:
					        {
					            int num10 = Main.rand.Next(14);
					            switch (num10)
					            {
					                case 0:
					                    num = 36;
					                    break;
					                case 1:
					                    num = 37;
					                    break;
					                case 2:
					                    num = 38;
					                    break;
					                case 3:
					                    num = 53;
					                    break;
					                case 4:
					                    num = 54;
					                    break;
					                case 5:
					                    num = 55;
					                    break;
					                case 6:
					                    num = 39;
					                    break;
					                case 7:
					                    num = 40;
					                    break;
					                case 8:
					                    num = 56;
					                    break;
					                case 9:
					                    num = 41;
					                    break;
					                case 10:
					                    num = 57;
					                    break;
					                case 11:
					                    num = 59;
					                    break;
					                case 12:
					                    num = 60;
					                    break;
					                case 13:
					                    num = 61;
					                    break;
					            }
					        }
					        break;
					    case 578:
					    case 534:
					    case 533:
					    case 506:
					    case 481:
					    case 436:
					    case 435:
					    case 434:
					    case 281:
					    case 266:
					    case 219:
					    case 197:
					    case 164:
					    case 120:
					    case 99:
					    case 98:
					    case 96:
					    case 95:
					    case 44:
					    case 39:
					        {
					            int num11 = Main.rand.Next(36);
					            switch (num11)
					            {
					                case 0:
					                    num = 16;
					                    break;
					                case 1:
					                    num = 17;
					                    break;
					                case 2:
					                    num = 18;
					                    break;
					                case 3:
					                    num = 19;
					                    break;
					                case 4:
					                    num = 20;
					                    break;
					                case 5:
					                    num = 21;
					                    break;
					                case 6:
					                    num = 22;
					                    break;
					                case 7:
					                    num = 23;
					                    break;
					                case 8:
					                    num = 24;
					                    break;
					                case 9:
					                    num = 25;
					                    break;
					                case 10:
					                    num = 58;
					                    break;
					                case 11:
					                    num = 36;
					                    break;
					                case 12:
					                    num = 37;
					                    break;
					                case 13:
					                    num = 38;
					                    break;
					                case 14:
					                    num = 53;
					                    break;
					                case 15:
					                    num = 54;
					                    break;
					                case 16:
					                    num = 55;
					                    break;
					                case 17:
					                    num = 39;
					                    break;
					                case 18:
					                    num = 40;
					                    break;
					                case 19:
					                    num = 56;
					                    break;
					                case 20:
					                    num = 41;
					                    break;
					                case 21:
					                    num = 57;
					                    break;
					                case 22:
					                    num = 42;
					                    break;
					                case 23:
					                    num = 43;
					                    break;
					                case 24:
					                    num = 44;
					                    break;
					                case 25:
					                    num = 45;
					                    break;
					                case 26:
					                    num = 46;
					                    break;
					                case 27:
					                    num = 47;
					                    break;
					                case 28:
					                    num = 48;
					                    break;
					                case 29:
					                    num = 49;
					                    break;
					                case 30:
					                    num = 50;
					                    break;
					                case 31:
					                    num = 51;
					                    break;
					                case 32:
					                    num = 59;
					                    break;
					                case 33:
					                    num = 60;
					                    break;
					                case 34:
					                    num = 61;
					                    break;
					                case 35:
					                    num = 82;
					                    break;
					            }
					        }
					        break;
					    case 519:
					    case 518:
					    case 517:
					    case 514:
					    case 496:
					    case 495:
					    case 494:
					    case 272:
					    case 218:
					    case 165:
					    case 157:
					    case 127:
					    case 113:
					    case 112:
					    case 65:
					    case 64:
					        {
					            int num12 = Main.rand.Next(36);
					            switch (num12)
					            {
					                case 0:
					                    num = 26;
					                    break;
					                case 1:
					                    num = 27;
					                    break;
					                case 2:
					                    num = 28;
					                    break;
					                case 3:
					                    num = 29;
					                    break;
					                case 4:
					                    num = 30;
					                    break;
					                case 5:
					                    num = 31;
					                    break;
					                case 6:
					                    num = 32;
					                    break;
					                case 7:
					                    num = 33;
					                    break;
					                case 8:
					                    num = 34;
					                    break;
					                case 9:
					                    num = 35;
					                    break;
					                case 10:
					                    num = 52;
					                    break;
					                case 11:
					                    num = 36;
					                    break;
					                case 12:
					                    num = 37;
					                    break;
					                case 13:
					                    num = 38;
					                    break;
					                case 14:
					                    num = 53;
					                    break;
					                case 15:
					                    num = 54;
					                    break;
					                case 16:
					                    num = 55;
					                    break;
					                case 17:
					                    num = 39;
					                    break;
					                case 18:
					                    num = 40;
					                    break;
					                case 19:
					                    num = 56;
					                    break;
					                case 20:
					                    num = 41;
					                    break;
					                case 21:
					                    num = 57;
					                    break;
					                case 22:
					                    num = 42;
					                    break;
					                case 23:
					                    num = 43;
					                    break;
					                case 24:
					                    num = 44;
					                    break;
					                case 25:
					                    num = 45;
					                    break;
					                case 26:
					                    num = 46;
					                    break;
					                case 27:
					                    num = 47;
					                    break;
					                case 28:
					                    num = 48;
					                    break;
					                case 29:
					                    num = 49;
					                    break;
					                case 30:
					                    num = 50;
					                    break;
					                case 31:
					                    num = 51;
					                    break;
					                case 32:
					                    num = 59;
					                    break;
					                case 33:
					                    num = 60;
					                    break;
					                case 34:
					                    num = 61;
					                    break;
					                case 35:
					                    num = 83;
					                    break;
					            }
					        }
					        break;
					    case 284:
					    case 191:
					    case 119:
					    case 55:
					        {
					            int num13 = Main.rand.Next(14);
					            switch (num13)
					            {
					                case 0:
					                    num = 36;
					                    break;
					                case 1:
					                    num = 37;
					                    break;
					                case 2:
					                    num = 38;
					                    break;
					                case 3:
					                    num = 53;
					                    break;
					                case 4:
					                    num = 54;
					                    break;
					                case 5:
					                    num = 55;
					                    break;
					                case 6:
					                    num = 39;
					                    break;
					                case 7:
					                    num = 40;
					                    break;
					                case 8:
					                    num = 56;
					                    break;
					                case 9:
					                    num = 41;
					                    break;
					                case 10:
					                    num = 57;
					                    break;
					                case 11:
					                    num = 59;
					                    break;
					                case 12:
					                    num = 60;
					                    break;
					                case 13:
					                    num = 61;
					                    break;
					            }
					        }
					        break;
					    default:
					        if (!this.accessory || this.type == 267 || this.type == 562 || this.type == 563 || this.type == 564 || this.type == 565 || this.type == 566 || this.type == 567 || this.type == 568 || this.type == 569 || this.type == 570 || this.type == 571 || this.type == 572 || this.type == 573 || this.type == 574 || this.type == 576)
					        {
					            return false;
					        }
					        num = Main.rand.Next(62, 81);
					        break;
					}
				}
				if (pre == -3)
				{
					return true;
				}
				if (pre == -1 && (num == 7 || num == 8 || num == 9 || num == 10 || num == 11 || num == 22 || num == 23 || num == 24 || num == 29 || num == 30 || num == 31 || num == 39 || num == 40 || num == 56 || num == 41 || num == 47 || num == 48 || num == 49) && Main.rand.Next(3) != 0)
				{
					num = 0;
				}
				switch (num)
				{
				    case 1:
				        num5 = 1.12f;
				        break;
				    case 2:
				        num5 = 1.18f;
				        break;
				    case 3:
				        num2 = 1.05f;
				        num8 = 2;
				        num5 = 1.05f;
				        break;
				    case 4:
				        num2 = 1.1f;
				        num5 = 1.1f;
				        num3 = 1.1f;
				        break;
				    case 5:
				        num2 = 1.15f;
				        break;
				    case 6:
				        num2 = 1.1f;
				        break;
				    case 81:
				        num3 = 1.15f;
				        num2 = 1.15f;
				        num8 = 5;
				        num4 = 0.9f;
				        num5 = 1.1f;
				        break;
				    case 7:
				        num5 = 0.82f;
				        break;
				    case 8:
				        num3 = 0.85f;
				        num2 = 0.85f;
				        num5 = 0.87f;
				        break;
				    case 9:
				        num5 = 0.9f;
				        break;
				    case 10:
				        num2 = 0.85f;
				        break;
				    case 11:
				        num4 = 1.1f;
				        num3 = 0.9f;
				        num5 = 0.9f;
				        break;
				    case 12:
				        num3 = 1.1f;
				        num2 = 1.05f;
				        num5 = 1.1f;
				        num4 = 1.15f;
				        break;
				    case 13:
				        num3 = 0.8f;
				        num2 = 0.9f;
				        num5 = 1.1f;
				        break;
				    case 14:
				        num3 = 1.15f;
				        num4 = 1.1f;
				        break;
				    case 15:
				        num3 = 0.9f;
				        num4 = 0.85f;
				        break;
				    case 16:
				        num2 = 1.1f;
				        num8 = 3;
				        break;
				    case 17:
				        num4 = 0.85f;
				        num6 = 1.1f;
				        break;
				    case 18:
				        num4 = 0.9f;
				        num6 = 1.15f;
				        break;
				    case 19:
				        num3 = 1.15f;
				        num6 = 1.05f;
				        break;
				    case 20:
				        num3 = 1.05f;
				        num6 = 1.05f;
				        num2 = 1.1f;
				        num4 = 0.95f;
				        num8 = 2;
				        break;
				    case 21:
				        num3 = 1.15f;
				        num2 = 1.1f;
				        break;
				    case 82:
				        num3 = 1.15f;
				        num2 = 1.15f;
				        num8 = 5;
				        num4 = 0.9f;
				        num6 = 1.1f;
				        break;
				    case 22:
				        num3 = 0.9f;
				        num6 = 0.9f;
				        num2 = 0.85f;
				        break;
				    case 23:
				        num4 = 1.15f;
				        num6 = 0.9f;
				        break;
				    case 24:
				        num4 = 1.1f;
				        num3 = 0.8f;
				        break;
				    case 25:
				        num4 = 1.1f;
				        num2 = 1.15f;
				        num8 = 1;
				        break;
				    case 58:
				        num4 = 0.85f;
				        num2 = 0.85f;
				        break;
				    case 26:
				        num7 = 0.85f;
				        num2 = 1.1f;
				        break;
				    case 27:
				        num7 = 0.85f;
				        break;
				    case 28:
				        num7 = 0.85f;
				        num2 = 1.15f;
				        num3 = 1.05f;
				        break;
				    case 83:
				        num3 = 1.15f;
				        num2 = 1.15f;
				        num8 = 5;
				        num4 = 0.9f;
				        num7 = 0.9f;
				        break;
				    case 29:
				        num7 = 1.1f;
				        break;
				    case 30:
				        num7 = 1.2f;
				        num2 = 0.9f;
				        break;
				    case 31:
				        num3 = 0.9f;
				        num2 = 0.9f;
				        break;
				    case 32:
				        num7 = 1.15f;
				        num2 = 1.1f;
				        break;
				    case 33:
				        num7 = 1.1f;
				        num3 = 1.1f;
				        num4 = 0.9f;
				        break;
				    case 34:
				        num7 = 0.9f;
				        num3 = 1.1f;
				        num4 = 1.1f;
				        num2 = 1.1f;
				        break;
				    case 35:
				        num7 = 1.2f;
				        num2 = 1.15f;
				        num3 = 1.15f;
				        break;
				    case 52:
				        num7 = 0.9f;
				        num2 = 0.9f;
				        num4 = 0.9f;
				        break;
				    case 36:
				        num8 = 3;
				        break;
				    case 37:
				        num2 = 1.1f;
				        num8 = 3;
				        num3 = 1.1f;
				        break;
				    case 38:
				        num3 = 1.15f;
				        break;
				    case 53:
				        num2 = 1.1f;
				        break;
				    case 54:
				        num3 = 1.15f;
				        break;
				    case 55:
				        num3 = 1.15f;
				        num2 = 1.05f;
				        break;
				    case 59:
				        num3 = 1.15f;
				        num2 = 1.15f;
				        num8 = 5;
				        break;
				    case 60:
				        num2 = 1.15f;
				        num8 = 5;
				        break;
				    case 61:
				        num8 = 5;
				        break;
				    case 39:
				        num2 = 0.7f;
				        num3 = 0.8f;
				        break;
				    case 40:
				        num2 = 0.85f;
				        break;
				    case 56:
				        num3 = 0.8f;
				        break;
				    case 41:
				        num3 = 0.85f;
				        num2 = 0.9f;
				        break;
				    case 57:
				        num3 = 0.9f;
				        num2 = 1.18f;
				        break;
				    case 42:
				        num4 = 0.9f;
				        break;
				    case 43:
				        num2 = 1.1f;
				        num4 = 0.9f;
				        break;
				    case 44:
				        num4 = 0.9f;
				        num8 = 3;
				        break;
				    case 45:
				        num4 = 0.95f;
				        break;
				    case 46:
				        num8 = 3;
				        num4 = 0.94f;
				        num2 = 1.07f;
				        break;
				    case 47:
				        num4 = 1.15f;
				        break;
				    case 48:
				        num4 = 1.2f;
				        break;
				    case 49:
				        num4 = 1.08f;
				        break;
				    case 50:
				        num2 = 0.8f;
				        num4 = 1.15f;
				        break;
				    case 51:
				        num3 = 0.9f;
				        num4 = 0.9f;
				        num2 = 1.05f;
				        num8 = 2;
				        break;
				}
				if (num2 != 1f && Math.Round((double)((float)this.damage * num2)) == (double)this.damage)
				{
					flag = true;
					num = -1;
				}
				if (num4 != 1f && Math.Round((double)((float)this.useAnimation * num4)) == (double)this.useAnimation)
				{
					flag = true;
					num = -1;
				}
				if (num7 != 1f && Math.Round((double)((float)this.mana * num7)) == (double)this.mana)
				{
					flag = true;
					num = -1;
				}
				if (num3 != 1f && this.knockBack == 0f)
				{
					flag = true;
					num = -1;
				}
				if (pre == -2 && num == 0)
				{
					num = -1;
					flag = true;
				}
			}
			this.damage = (int)Math.Round((double)((float)this.damage * num2));
			this.useAnimation = (int)Math.Round((double)((float)this.useAnimation * num4));
			this.useTime = (int)Math.Round((double)((float)this.useTime * num4));
			this.reuseDelay = (int)Math.Round((double)((float)this.reuseDelay * num4));
			this.mana = (int)Math.Round((double)((float)this.mana * num7));
			this.knockBack *= num3;
			this.scale *= num5;
			this.shootSpeed *= num6;
			this.crit += num8;
			float num14 = 1f * num2 * (2f - num4) * (2f - num7) * num5 * num3 * num6 * (1f + (float)this.crit * 0.02f);
			switch (num)
			{
			    case 77:
			    case 73:
			    case 69:
			    case 62:
			        num14 *= 1.05f;
			        break;
			    case 67:
			    case 78:
			    case 74:
			    case 70:
			    case 63:
			        num14 *= 1.1f;
			        break;
			    case 66:
			    case 79:
			    case 75:
			    case 71:
			    case 64:
			        num14 *= 1.15f;
			        break;
			    case 68:
			    case 80:
			    case 76:
			    case 72:
			    case 65:
			        num14 *= 1.2f;
			        break;
			}
			if ((double)num14 >= 1.2)
			{
				this.rare += 2;
			}
			else
			{
				if ((double)num14 >= 1.05)
				{
					this.rare++;
				}
				else
				{
					if ((double)num14 <= 0.8)
					{
						this.rare -= 2;
					}
					else
					{
						if ((double)num14 <= 0.95)
						{
							this.rare--;
						}
					}
				}
			}
			if (this.rare < -1)
			{
				this.rare = -1;
			}
			if (this.rare > 6)
			{
				this.rare = 6;
			}
			num14 *= num14;
			this.value = (int)((float)this.value * num14);
			this.prefix = (byte)num;
			return true;
		}
		public string AffixName()
		{
			string text = "";
			switch (this.prefix)
			{
			    case 1:
			        text = "Large";
			        break;
			    case 2:
			        text = "Massive";
			        break;
			    case 3:
			        text = "Dangerous";
			        break;
			    case 4:
			        text = "Savage";
			        break;
			    case 5:
			        text = "Sharp";
			        break;
			    case 6:
			        text = "Pointy";
			        break;
			    case 7:
			        text = "Tiny";
			        break;
			    case 8:
			        text = "Terrible";
			        break;
			    case 9:
			        text = "Small";
			        break;
			    case 10:
			        text = "Dull";
			        break;
			    case 11:
			        text = "Unhappy";
			        break;
			    case 12:
			        text = "Bulky";
			        break;
			    case 13:
			        text = "Shameful";
			        break;
			    case 14:
			        text = "Heavy";
			        break;
			    case 15:
			        text = "Light";
			        break;
			    case 16:
			        text = "Sighted";
			        break;
			    case 17:
			        text = "Rapid";
			        break;
			    case 18:
			        text = "Hasty";
			        break;
			    case 19:
			        text = "Intimidating";
			        break;
			    case 20:
			        text = "Deadly";
			        break;
			    case 21:
			        text = "Staunch";
			        break;
			    case 22:
			        text = "Awful";
			        break;
			    case 23:
			        text = "Lethargic";
			        break;
			    case 24:
			        text = "Awkward";
			        break;
			    case 25:
			        text = "Powerful";
			        break;
			    case 58:
			        text = "Frenzying";
			        break;
			    case 26:
			        text = "Mystic";
			        break;
			    case 27:
			        text = "Adept";
			        break;
			    case 28:
			        text = "Masterful";
			        break;
			    case 29:
			        text = "Inept";
			        break;
			    case 30:
			        text = "Ignorant";
			        break;
			    case 31:
			        text = "Deranged";
			        break;
			    case 32:
			        text = "Intense";
			        break;
			    case 33:
			        text = "Taboo";
			        break;
			    case 34:
			        text = "Celestial";
			        break;
			    case 35:
			        text = "Furious";
			        break;
			    case 52:
			        text = "Manic";
			        break;
			    case 36:
			        text = "Keen";
			        break;
			    case 37:
			        text = "Superior";
			        break;
			    case 38:
			        text = "Forceful";
			        break;
			    case 53:
			        text = "Hurtful";
			        break;
			    case 54:
			        text = "Strong";
			        break;
			    case 55:
			        text = "Unpleasant";
			        break;
			    case 39:
			        text = "Broken";
			        break;
			    case 40:
			        text = "Damaged";
			        break;
			    case 56:
			        text = "Weak";
			        break;
			    case 41:
			        text = "Shoddy";
			        break;
			    case 57:
			        text = "Ruthless";
			        break;
			    case 42:
			        text = "Quick";
			        break;
			    case 43:
			        text = "Deadly";
			        break;
			    case 44:
			        text = "Agile";
			        break;
			    case 45:
			        text = "Nimble";
			        break;
			    case 46:
			        text = "Murderous";
			        break;
			    case 47:
			        text = "Slow";
			        break;
			    case 48:
			        text = "Sluggish";
			        break;
			    case 49:
			        text = "Lazy";
			        break;
			    case 50:
			        text = "Annoying";
			        break;
			    case 51:
			        text = "Nasty";
			        break;
			    case 59:
			        text = "Godly";
			        break;
			    case 60:
			        text = "Demonic";
			        break;
			    case 61:
			        text = "Zealous";
			        break;
			    case 62:
			        text = "Hard";
			        break;
			    case 63:
			        text = "Guarding";
			        break;
			    case 64:
			        text = "Armored";
			        break;
			    case 65:
			        text = "Warding";
			        break;
			    case 66:
			        text = "Arcane";
			        break;
			    case 67:
			        text = "Precise";
			        break;
			    case 68:
			        text = "Lucky";
			        break;
			    case 69:
			        text = "Jagged";
			        break;
			    case 70:
			        text = "Spiked";
			        break;
			    case 71:
			        text = "Angry";
			        break;
			    case 72:
			        text = "Menacing";
			        break;
			    case 73:
			        text = "Brisk";
			        break;
			    case 74:
			        text = "Fleeting";
			        break;
			    case 75:
			        text = "Hasty";
			        break;
			    case 76:
			        text = "Quick";
			        break;
			    case 77:
			        text = "Wild";
			        break;
			    case 78:
			        text = "Rash";
			        break;
			    case 79:
			        text = "Intrepid";
			        break;
			    case 80:
			        text = "Violent";
			        break;
			    case 81:
			        text = "Legendary";
			        break;
			    case 82:
			        text = "Unreal";
			        break;
			    case 83:
			        text = "Mythical";
			        break;
			}
			string result = this.name;
			if (text != "")
			{
				result = text + " " + this.name;
			}
			return result;
		}
		public void RealSetDefaults(string ItemName)
		{
			this.name = "";
			bool flag = false;
			switch (ItemName)
			{
			    case "Gold Pickaxe":
			        this.SetDefaults(1, false);
			        this.color = new Color(210, 190, 0, 100);
			        this.useTime = 17;
			        this.pick = 55;
			        this.useAnimation = 20;
			        this.scale = 1.05f;
			        this.damage = 6;
			        this.value = 10000;
			        this.toolTip = "Can mine Meteorite";
			        this.netID = -1;
			        break;
			    case "Gold Broadsword":
			        this.SetDefaults(4, false);
			        this.color = new Color(210, 190, 0, 100);
			        this.useAnimation = 20;
			        this.damage = 13;
			        this.scale = 1.05f;
			        this.value = 9000;
			        this.netID = -2;
			        break;
			    case "Gold Shortsword":
			        this.SetDefaults(6, false);
			        this.color = new Color(210, 190, 0, 100);
			        this.damage = 11;
			        this.useAnimation = 11;
			        this.scale = 0.95f;
			        this.value = 7000;
			        this.netID = -3;
			        break;
			    case "Gold Axe":
			        this.SetDefaults(10, false);
			        this.color = new Color(210, 190, 0, 100);
			        this.useTime = 18;
			        this.axe = 11;
			        this.useAnimation = 26;
			        this.scale = 1.15f;
			        this.damage = 7;
			        this.value = 8000;
			        this.netID = -4;
			        break;
			    case "Gold Hammer":
			        this.SetDefaults(7, false);
			        this.color = new Color(210, 190, 0, 100);
			        this.useAnimation = 28;
			        this.useTime = 23;
			        this.scale = 1.25f;
			        this.damage = 9;
			        this.hammer = 55;
			        this.value = 8000;
			        this.netID = -5;
			        break;
			    case "Gold Bow":
			        this.SetDefaults(99, false);
			        this.useAnimation = 26;
			        this.useTime = 26;
			        this.color = new Color(210, 190, 0, 100);
			        this.damage = 11;
			        this.value = 7000;
			        this.netID = -6;
			        break;
			    case "Silver Pickaxe":
			        this.SetDefaults(1, false);
			        this.color = new Color(180, 180, 180, 100);
			        this.useTime = 11;
			        this.pick = 45;
			        this.useAnimation = 19;
			        this.scale = 1.05f;
			        this.damage = 6;
			        this.value = 5000;
			        this.netID = -7;
			        break;
			    case "Silver Broadsword":
			        this.SetDefaults(4, false);
			        this.color = new Color(180, 180, 180, 100);
			        this.useAnimation = 21;
			        this.damage = 11;
			        this.value = 4500;
			        this.netID = -8;
			        break;
			    case "Silver Shortsword":
			        this.SetDefaults(6, false);
			        this.color = new Color(180, 180, 180, 100);
			        this.damage = 9;
			        this.useAnimation = 12;
			        this.scale = 0.95f;
			        this.value = 3500;
			        this.netID = -9;
			        break;
			    case "Silver Axe":
			        this.SetDefaults(10, false);
			        this.color = new Color(180, 180, 180, 100);
			        this.useTime = 18;
			        this.axe = 10;
			        this.useAnimation = 26;
			        this.scale = 1.15f;
			        this.damage = 6;
			        this.value = 4000;
			        this.netID = -10;
			        break;
			    case "Silver Hammer":
			        this.SetDefaults(7, false);
			        this.color = new Color(180, 180, 180, 100);
			        this.useAnimation = 29;
			        this.useTime = 19;
			        this.scale = 1.25f;
			        this.damage = 9;
			        this.hammer = 45;
			        this.value = 4000;
			        this.netID = -11;
			        break;
			    case "Silver Bow":
			        this.SetDefaults(99, false);
			        this.useAnimation = 27;
			        this.useTime = 27;
			        this.color = new Color(180, 180, 180, 100);
			        this.damage = 9;
			        this.value = 3500;
			        this.netID = -12;
			        break;
			    case "Copper Pickaxe":
			        this.SetDefaults(1, false);
			        this.color = new Color(180, 100, 45, 80);
			        this.useTime = 15;
			        this.pick = 35;
			        this.useAnimation = 23;
			        this.damage = 4;
			        this.scale = 0.9f;
			        this.tileBoost = -1;
			        this.value = 500;
			        this.netID = -13;
			        break;
			    case "Copper Broadsword":
			        this.SetDefaults(4, false);
			        this.color = new Color(180, 100, 45, 80);
			        this.useAnimation = 23;
			        this.damage = 8;
			        this.value = 450;
			        this.netID = -14;
			        break;
			    case "Copper Shortsword":
			        this.SetDefaults(6, false);
			        this.color = new Color(180, 100, 45, 80);
			        this.damage = 5;
			        this.useAnimation = 13;
			        this.scale = 0.8f;
			        this.value = 350;
			        this.netID = -15;
			        break;
			    case "Copper Axe":
			        this.SetDefaults(10, false);
			        this.color = new Color(180, 100, 45, 80);
			        this.useTime = 21;
			        this.axe = 7;
			        this.useAnimation = 30;
			        this.scale = 1f;
			        this.damage = 3;
			        this.tileBoost = -1;
			        this.value = 400;
			        this.netID = -16;
			        break;
			    case "Copper Hammer":
			        this.SetDefaults(7, false);
			        this.color = new Color(180, 100, 45, 80);
			        this.useAnimation = 33;
			        this.useTime = 23;
			        this.scale = 1.1f;
			        this.damage = 4;
			        this.hammer = 35;
			        this.tileBoost = -1;
			        this.value = 400;
			        this.netID = -17;
			        break;
			    case "Copper Bow":
			        this.SetDefaults(99, false);
			        this.useAnimation = 29;
			        this.useTime = 29;
			        this.color = new Color(180, 100, 45, 80);
			        this.damage = 6;
			        this.value = 350;
			        this.netID = -18;
			        break;
			    case "Blue Phasesaber":
			        this.SetDefaults(198, false);
			        this.damage = 41;
			        this.scale = 1.15f;
			        flag = true;
			        this.autoReuse = true;
			        this.useTurn = true;
			        this.rare = 4;
			        this.netID = -19;
			        break;
			    case "Red Phasesaber":
			        this.SetDefaults(199, false);
			        this.damage = 41;
			        this.scale = 1.15f;
			        flag = true;
			        this.autoReuse = true;
			        this.useTurn = true;
			        this.rare = 4;
			        this.netID = -20;
			        break;
			    case "Green Phasesaber":
			        this.SetDefaults(200, false);
			        this.damage = 41;
			        this.scale = 1.15f;
			        flag = true;
			        this.autoReuse = true;
			        this.useTurn = true;
			        this.rare = 4;
			        this.netID = -21;
			        break;
			    case "Purple Phasesaber":
			        this.SetDefaults(201, false);
			        this.damage = 41;
			        this.scale = 1.15f;
			        flag = true;
			        this.autoReuse = true;
			        this.useTurn = true;
			        this.rare = 4;
			        this.netID = -22;
			        break;
			    case "White Phasesaber":
			        this.SetDefaults(202, false);
			        this.damage = 41;
			        this.scale = 1.15f;
			        flag = true;
			        this.autoReuse = true;
			        this.useTurn = true;
			        this.rare = 4;
			        this.netID = -23;
			        break;
			    case "Yellow Phasesaber":
			        this.SetDefaults(203, false);
			        this.damage = 41;
			        this.scale = 1.15f;
			        flag = true;
			        this.autoReuse = true;
			        this.useTurn = true;
			        this.rare = 4;
			        this.netID = -24;
			        break;
			    default:
			        if (ItemName != "")
			        {
			            for (int i = 0; i < 603; i++)
			            {
			                if (Main.itemName[i] == ItemName)
			                {
			                    this.SetDefaults(i, false);
			                    this.checkMat();
			                    return;
			                }
			            }
			            this.name = "";
			            this.stack = 0;
			            this.type = 0;
			        }
			        break;
			}
			if (this.type != 0)
			{
				if (flag)
				{
					this.material = false;
				}
				else
				{
					this.checkMat();
				}
				this.name = ItemName;
			}
		}
		public bool checkMat()
		{
			if (this.type >= 71 && this.type <= 74)
			{
				this.material = false;
				return false;
			}
			for (int i = 0; i < Recipe.numRecipes; i++)
			{
				int num = 0;
				while (Main.recipe[i].requiredItem[num].type > 0)
				{
					if (this.name == Main.recipe[i].requiredItem[num].name)
					{
						this.material = true;
						return true;
					}
					num++;
				}
			}
			this.material = false;
			return false;
		}
		public void RealnetDefaults(int type)
		{
			if (type < 0)
			{
				if (type == -1)
				{
					this.SetDefaults("Gold Pickaxe");
					return;
				}
				if (type == -2)
				{
					this.SetDefaults("Gold Broadsword");
					return;
				}
				if (type == -3)
				{
					this.SetDefaults("Gold Shortsword");
					return;
				}
				if (type == -4)
				{
					this.SetDefaults("Gold Axe");
					return;
				}
				if (type == -5)
				{
					this.SetDefaults("Gold Hammer");
					return;
				}
				if (type == -6)
				{
					this.SetDefaults("Gold Bow");
					return;
				}
				if (type == -7)
				{
					this.SetDefaults("Silver Pickaxe");
					return;
				}
				if (type == -8)
				{
					this.SetDefaults("Silver Broadsword");
					return;
				}
				if (type == -9)
				{
					this.SetDefaults("Silver Shortsword");
					return;
				}
				if (type == -10)
				{
					this.SetDefaults("Silver Axe");
					return;
				}
				if (type == -11)
				{
					this.SetDefaults("Silver Hammer");
					return;
				}
				if (type == -12)
				{
					this.SetDefaults("Silver Bow");
					return;
				}
				if (type == -13)
				{
					this.SetDefaults("Copper Pickaxe");
					return;
				}
				if (type == -14)
				{
					this.SetDefaults("Copper Broadsword");
					return;
				}
				if (type == -15)
				{
					this.SetDefaults("Copper Shortsword");
					return;
				}
				if (type == -16)
				{
					this.SetDefaults("Copper Axe");
					return;
				}
				if (type == -17)
				{
					this.SetDefaults("Copper Hammer");
					return;
				}
				if (type == -18)
				{
					this.SetDefaults("Copper Bow");
					return;
				}
				if (type == -19)
				{
					this.SetDefaults("Blue Phasesaber");
					return;
				}
				if (type == -20)
				{
					this.SetDefaults("Red Phasesaber");
					return;
				}
				if (type == -21)
				{
					this.SetDefaults("Green Phasesaber");
					return;
				}
				if (type == -22)
				{
					this.SetDefaults("Purple Phasesaber");
					return;
				}
				if (type == -23)
				{
					this.SetDefaults("White Phasesaber");
					return;
				}
				if (type == -24)
				{
					this.SetDefaults("Yellow Phasesaber");
					return;
				}
			}
			else
			{
				this.SetDefaults(type, false);
			}
		}
		public void RealSetDefaults(int Type, bool noMatCheck = false)
		{
			if (Main.netMode == 1 || Main.netMode == 2)
			{
				this.owner = 255;
			}
			else
			{
				this.owner = Main.myPlayer;
			}
			this.netID = 0;
			this.prefix = 0;
			this.crit = 0;
			this.mech = false;
			this.reuseDelay = 0;
			this.melee = false;
			this.magic = false;
			this.ranged = false;
			this.placeStyle = 0;
			this.buffTime = 0;
			this.buffType = 0;
			this.material = false;
			this.noWet = false;
			this.vanity = false;
			this.mana = 0;
			this.wet = false;
			this.wetCount = 0;
			this.lavaWet = false;
			this.channel = false;
			this.manaIncrease = 0;
			this.release = 0;
			this.noMelee = false;
			this.noUseGraphic = false;
			this.lifeRegen = 0;
			this.shootSpeed = 0f;
			this.active = true;
			this.alpha = 0;
			this.ammo = 0;
			this.useAmmo = 0;
			this.autoReuse = false;
			this.accessory = false;
			this.axe = 0;
			this.healMana = 0;
			this.bodySlot = -1;
			this.legSlot = -1;
			this.headSlot = -1;
			this.potion = false;
			this.color = default(Color);
			this.consumable = false;
			this.createTile = -1;
			this.createWall = -1;
			this.damage = -1;
			this.defense = 0;
			this.hammer = 0;
			this.healLife = 0;
			this.holdStyle = 0;
			this.knockBack = 0f;
			this.maxStack = 1;
			this.pick = 0;
			this.rare = 0;
			this.scale = 1f;
			this.shoot = 0;
			this.stack = 1;
			this.toolTip = null;
			this.toolTip2 = null;
			this.tileBoost = 0;
			this.type = Type;
			this.useStyle = 0;
			this.useSound = 0;
			this.useTime = 100;
			this.useAnimation = 100;
			this.value = 0;
			this.useTurn = false;
			this.buy = false;
            switch (this.type)
            {
                case 0:
                    this.name = "";
                    this.stack = 0;
                    break;
                case 1:
                    this.name = "Iron Pickaxe";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 20;
                    this.useTime = 13;
                    this.autoReuse = true;
                    this.width = 24;
                    this.height = 28;
                    this.damage = 5;
                    this.pick = 40;
                    this.useSound = 1;
                    this.knockBack = 2f;
                    this.value = 2000;
                    this.melee = true;
                    break;
                case 2:
                    this.name = "Dirt Block";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 0;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 3:
                    this.name = "Stone Block";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 1;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 4:
                    this.name = "Iron Broadsword";
                    this.useStyle = 1;
                    this.useTurn = false;
                    this.useAnimation = 21;
                    this.useTime = 21;
                    this.width = 24;
                    this.height = 28;
                    this.damage = 10;
                    this.knockBack = 5f;
                    this.useSound = 1;
                    this.scale = 1f;
                    this.value = 1800;
                    this.melee = true;
                    break;
                case 5:
                    this.name = "Mushroom";
                    this.useStyle = 2;
                    this.useSound = 2;
                    this.useTurn = false;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.width = 16;
                    this.height = 18;
                    this.healLife = 15;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.potion = true;
                    this.value = 25;
                    break;
                case 6:
                    this.name = "Iron Shortsword";
                    this.useStyle = 3;
                    this.useTurn = false;
                    this.useAnimation = 12;
                    this.useTime = 12;
                    this.width = 24;
                    this.height = 28;
                    this.damage = 8;
                    this.knockBack = 4f;
                    this.scale = 0.9f;
                    this.useSound = 1;
                    this.useTurn = true;
                    this.value = 1400;
                    this.melee = true;
                    break;
                case 7:
                    this.name = "Iron Hammer";
                    this.autoReuse = true;
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 30;
                    this.useTime = 20;
                    this.hammer = 45;
                    this.width = 24;
                    this.height = 28;
                    this.damage = 7;
                    this.knockBack = 5.5f;
                    this.scale = 1.2f;
                    this.useSound = 1;
                    this.value = 1600;
                    this.melee = true;
                    break;
                case 8:
                    this.noWet = true;
                    this.name = "Torch";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.holdStyle = 1;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 4;
                    this.width = 10;
                    this.height = 12;
                    this.toolTip = "Provides light";
                    this.value = 50;
                    break;
                case 9:
                    this.name = "Wood";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 30;
                    this.width = 8;
                    this.height = 10;
                    break;
                case 10:
                    this.name = "Iron Axe";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 27;
                    this.knockBack = 4.5f;
                    this.useTime = 19;
                    this.autoReuse = true;
                    this.width = 24;
                    this.height = 28;
                    this.damage = 5;
                    this.axe = 9;
                    this.scale = 1.1f;
                    this.useSound = 1;
                    this.value = 1600;
                    this.melee = true;
                    break;
                case 11:
                    this.name = "Iron Ore";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 6;
                    this.width = 12;
                    this.height = 12;
                    this.value = 500;
                    break;
                case 12:
                    this.name = "Copper Ore";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 7;
                    this.width = 12;
                    this.height = 12;
                    this.value = 250;
                    break;
                case 13:
                    this.name = "Gold Ore";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 8;
                    this.width = 12;
                    this.height = 12;
                    this.value = 2000;
                    break;
                case 14:
                    this.name = "Silver Ore";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 9;
                    this.width = 12;
                    this.height = 12;
                    this.value = 1000;
                    break;
                case 15:
                    this.name = "Copper Watch";
                    this.width = 24;
                    this.height = 28;
                    this.accessory = true;
                    this.toolTip = "Tells the time";
                    this.value = 1000;
                    break;
                case 16:
                    this.name = "Silver Watch";
                    this.width = 24;
                    this.height = 28;
                    this.accessory = true;
                    this.toolTip = "Tells the time";
                    this.value = 5000;
                    break;
                case 17:
                    this.name = "Gold Watch";
                    this.width = 24;
                    this.height = 28;
                    this.accessory = true;
                    this.rare = 1;
                    this.toolTip = "Tells the time";
                    this.value = 10000;
                    break;
                case 18:
                    this.name = "Depth Meter";
                    this.width = 24;
                    this.height = 18;
                    this.accessory = true;
                    this.rare = 1;
                    this.toolTip = "Shows depth";
                    this.value = 10000;
                    break;
                case 19:
                    this.name = "Gold Bar";
                    this.width = 20;
                    this.height = 20;
                    this.maxStack = 99;
                    this.value = 6000;
                    break;
                case 20:
                    this.name = "Copper Bar";
                    this.width = 20;
                    this.height = 20;
                    this.maxStack = 99;
                    this.value = 750;
                    break;
                case 21:
                    this.name = "Silver Bar";
                    this.width = 20;
                    this.height = 20;
                    this.maxStack = 99;
                    this.value = 3000;
                    break;
                case 22:
                    this.name = "Iron Bar";
                    this.width = 20;
                    this.height = 20;
                    this.maxStack = 99;
                    this.value = 1500;
                    break;
                case 23:
                    this.name = "Gel";
                    this.width = 10;
                    this.height = 12;
                    this.maxStack = 250;
                    this.alpha = 175;
                    this.ammo = 23;
                    this.color = new Color(0, 80, 255, 100);
                    this.toolTip = "'Both tasty and flammable'";
                    this.value = 5;
                    break;
                case 24:
                    this.name = "Wooden Sword";
                    this.useStyle = 1;
                    this.useTurn = false;
                    this.useAnimation = 25;
                    this.width = 24;
                    this.height = 28;
                    this.damage = 7;
                    this.knockBack = 4f;
                    this.scale = 0.95f;
                    this.useSound = 1;
                    this.value = 100;
                    this.melee = true;
                    break;
                case 25:
                    this.name = "Wooden Door";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 10;
                    this.width = 14;
                    this.height = 28;
                    this.value = 200;
                    break;
                case 26:
                    this.name = "Stone Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 7;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 1;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 27:
                    this.name = "Acorn";
                    this.useTurn = true;
                    this.useStyle = 1;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 20;
                    this.width = 18;
                    this.height = 18;
                    this.value = 10;
                    break;
                case 28:
                    this.name = "Lesser Healing Potion";
                    this.useSound = 3;
                    this.healLife = 50;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.potion = true;
                    this.value = 300;
                    break;
                case 29:
                    this.name = "Life Crystal";
                    this.maxStack = 99;
                    this.consumable = true;
                    this.width = 18;
                    this.height = 18;
                    this.useStyle = 4;
                    this.useTime = 30;
                    this.useSound = 4;
                    this.useAnimation = 30;
                    this.toolTip = "Permanently increases maximum life by 20";
                    this.rare = 2;
                    this.value = 75000;
                    break;
                case 30:
                    this.name = "Dirt Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 7;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 16;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 31:
                    this.name = "Bottle";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 13;
                    this.width = 16;
                    this.height = 24;
                    this.value = 20;
                    break;
                case 32:
                    this.name = "Wooden Table";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 14;
                    this.width = 26;
                    this.height = 20;
                    this.value = 300;
                    break;
                case 33:
                    this.name = "Furnace";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 17;
                    this.width = 26;
                    this.height = 24;
                    this.value = 300;
                    this.toolTip = "Used for smelting ore";
                    break;
                case 34:
                    this.name = "Wooden Chair";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 15;
                    this.width = 12;
                    this.height = 30;
                    this.value = 150;
                    break;
                case 35:
                    this.name = "Iron Anvil";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 16;
                    this.width = 28;
                    this.height = 14;
                    this.value = 5000;
                    this.toolTip = "Used to craft items from metal bars";
                    break;
                case 36:
                    this.name = "Work Bench";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 18;
                    this.width = 28;
                    this.height = 14;
                    this.value = 150;
                    this.toolTip = "Used for basic crafting";
                    break;
                case 37:
                    this.name = "Goggles";
                    this.width = 28;
                    this.height = 12;
                    this.defense = 1;
                    this.headSlot = 10;
                    this.rare = 1;
                    this.value = 1000;
                    break;
                case 38:
                    this.name = "Lens";
                    this.width = 12;
                    this.height = 20;
                    this.maxStack = 99;
                    this.value = 500;
                    break;
                case 39:
                    this.useStyle = 5;
                    this.useAnimation = 30;
                    this.useTime = 30;
                    this.name = "Wooden Bow";
                    this.width = 12;
                    this.height = 28;
                    this.shoot = 1;
                    this.useAmmo = 1;
                    this.useSound = 5;
                    this.damage = 4;
                    this.shootSpeed = 6.1f;
                    this.noMelee = true;
                    this.value = 100;
                    this.ranged = true;
                    break;
                case 40:
                    this.name = "Wooden Arrow";
                    this.shootSpeed = 3f;
                    this.shoot = 1;
                    this.damage = 4;
                    this.width = 10;
                    this.height = 28;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.ammo = 1;
                    this.knockBack = 2f;
                    this.value = 10;
                    this.ranged = true;
                    break;
                case 41:
                    this.name = "Flaming Arrow";
                    this.shootSpeed = 3.5f;
                    this.shoot = 2;
                    this.damage = 6;
                    this.width = 10;
                    this.height = 28;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.ammo = 1;
                    this.knockBack = 2f;
                    this.value = 15;
                    this.ranged = true;
                    break;
                case 42:
                    this.useStyle = 1;
                    this.name = "Shuriken";
                    this.shootSpeed = 9f;
                    this.shoot = 3;
                    this.damage = 10;
                    this.width = 18;
                    this.height = 20;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.useSound = 1;
                    this.useAnimation = 15;
                    this.useTime = 15;
                    this.noUseGraphic = true;
                    this.noMelee = true;
                    this.value = 20;
                    this.ranged = true;
                    break;
                case 43:
                    this.useStyle = 4;
                    this.name = "Suspicious Looking Eye";
                    this.width = 22;
                    this.height = 14;
                    this.consumable = true;
                    this.useAnimation = 45;
                    this.useTime = 45;
                    this.maxStack = 20;
                    this.toolTip = "Summons the Eye of Cthulhu";
                    break;
                case 44:
                    this.useStyle = 5;
                    this.useAnimation = 25;
                    this.useTime = 25;
                    this.name = "Demon Bow";
                    this.width = 12;
                    this.height = 28;
                    this.shoot = 1;
                    this.useAmmo = 1;
                    this.useSound = 5;
                    this.damage = 14;
                    this.shootSpeed = 6.7f;
                    this.knockBack = 1f;
                    this.alpha = 30;
                    this.rare = 1;
                    this.noMelee = true;
                    this.value = 18000;
                    this.ranged = true;
                    break;
                case 45:
                    this.name = "War Axe of the Night";
                    this.autoReuse = true;
                    this.useStyle = 1;
                    this.useAnimation = 30;
                    this.knockBack = 6f;
                    this.useTime = 15;
                    this.width = 24;
                    this.height = 28;
                    this.damage = 20;
                    this.axe = 15;
                    this.scale = 1.2f;
                    this.useSound = 1;
                    this.rare = 1;
                    this.value = 13500;
                    this.melee = true;
                    break;
                case 46:
                    this.name = "Light's Bane";
                    this.useStyle = 1;
                    this.useAnimation = 20;
                    this.knockBack = 5f;
                    this.width = 24;
                    this.height = 28;
                    this.damage = 17;
                    this.scale = 1.1f;
                    this.useSound = 1;
                    this.rare = 1;
                    this.value = 13500;
                    this.melee = true;
                    break;
                case 47:
                    this.name = "Unholy Arrow";
                    this.shootSpeed = 3.4f;
                    this.shoot = 4;
                    this.damage = 8;
                    this.width = 10;
                    this.height = 28;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.ammo = 1;
                    this.knockBack = 3f;
                    this.alpha = 30;
                    this.rare = 1;
                    this.value = 40;
                    this.ranged = true;
                    break;
                case 48:
                    this.name = "Chest";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 21;
                    this.width = 26;
                    this.height = 22;
                    this.value = 500;
                    break;
                case 49:
                    this.name = "Band of Regeneration";
                    this.width = 22;
                    this.height = 22;
                    this.accessory = true;
                    this.lifeRegen = 1;
                    this.rare = 1;
                    this.toolTip = "Slowly regenerates life";
                    this.value = 50000;
                    break;
                case 50:
                    this.mana = 20;
                    this.name = "Magic Mirror";
                    this.useTurn = true;
                    this.width = 20;
                    this.height = 20;
                    this.useStyle = 4;
                    this.useTime = 90;
                    this.useSound = 6;
                    this.useAnimation = 90;
                    this.toolTip = "Gaze in the mirror to return home";
                    this.rare = 1;
                    this.value = 50000;
                    break;
                case 51:
                    this.name = "Jester's Arrow";
                    this.shootSpeed = 0.5f;
                    this.shoot = 5;
                    this.damage = 9;
                    this.width = 10;
                    this.height = 28;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.ammo = 1;
                    this.knockBack = 4f;
                    this.rare = 1;
                    this.value = 100;
                    this.ranged = true;
                    break;
                case 52:
                    this.name = "Angel Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 1;
                    break;
                case 53:
                    this.name = "Cloud in a Bottle";
                    this.width = 16;
                    this.height = 24;
                    this.accessory = true;
                    this.rare = 1;
                    this.toolTip = "Allows the holder to double jump";
                    this.value = 50000;
                    break;
                case 54:
                    this.name = "Hermes Boots";
                    this.width = 28;
                    this.height = 24;
                    this.accessory = true;
                    this.rare = 1;
                    this.toolTip = "The wearer can run super fast";
                    this.value = 50000;
                    break;
                case 55:
                    this.noMelee = true;
                    this.useStyle = 1;
                    this.name = "Enchanted Boomerang";
                    this.shootSpeed = 10f;
                    this.shoot = 6;
                    this.damage = 13;
                    this.knockBack = 8f;
                    this.width = 14;
                    this.height = 28;
                    this.useSound = 1;
                    this.useAnimation = 15;
                    this.useTime = 15;
                    this.noUseGraphic = true;
                    this.rare = 1;
                    this.value = 50000;
                    this.melee = true;
                    break;
                case 56:
                    this.name = "Demonite Ore";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 22;
                    this.width = 12;
                    this.height = 12;
                    this.rare = 1;
                    this.toolTip = "'Pulsing with dark energy'";
                    this.value = 4000;
                    break;
                case 57:
                    this.name = "Demonite Bar";
                    this.width = 20;
                    this.height = 20;
                    this.maxStack = 99;
                    this.rare = 1;
                    this.toolTip = "'Pulsing with dark energy'";
                    this.value = 16000;
                    break;
                case 58:
                    this.name = "Heart";
                    this.width = 12;
                    this.height = 12;
                    break;
                case 59:
                    this.name = "Corrupt Seeds";
                    this.useTurn = true;
                    this.useStyle = 1;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 23;
                    this.width = 14;
                    this.height = 14;
                    this.value = 500;
                    break;
                case 60:
                    this.name = "Vile Mushroom";
                    this.width = 16;
                    this.height = 18;
                    this.maxStack = 99;
                    this.value = 50;
                    break;
                case 61:
                    this.name = "Ebonstone Block";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 25;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 62:
                    this.name = "Grass Seeds";
                    this.useTurn = true;
                    this.useStyle = 1;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 2;
                    this.width = 14;
                    this.height = 14;
                    this.value = 20;
                    break;
                case 63:
                    this.name = "Sunflower";
                    this.useTurn = true;
                    this.useStyle = 1;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 27;
                    this.width = 26;
                    this.height = 26;
                    this.value = 200;
                    break;
                case 64:
                    this.mana = 12;
                    this.damage = 8;
                    this.useStyle = 1;
                    this.name = "Vilethorn";
                    this.shootSpeed = 32f;
                    this.shoot = 7;
                    this.width = 26;
                    this.height = 28;
                    this.useSound = 8;
                    this.useAnimation = 30;
                    this.useTime = 30;
                    this.rare = 1;
                    this.noMelee = true;
                    this.knockBack = 1f;
                    this.toolTip = "Summons a vile thorn";
                    this.value = 10000;
                    this.magic = true;
                    break;
                case 65:
                    this.autoReuse = true;
                    this.mana = 16;
                    this.knockBack = 5f;
                    this.alpha = 100;
                    this.color = new Color(150, 150, 150, 0);
                    this.damage = 16;
                    this.useStyle = 1;
                    this.scale = 1.15f;
                    this.name = "Starfury";
                    this.shootSpeed = 12f;
                    this.shoot = 9;
                    this.width = 14;
                    this.height = 28;
                    this.useSound = 9;
                    this.useAnimation = 25;
                    this.useTime = 10;
                    this.rare = 1;
                    this.toolTip = "Causes stars to rain from the sky";
                    this.toolTip2 = "'Forged with the fury of heaven'";
                    this.value = 50000;
                    this.magic = true;
                    break;
                case 66:
                    this.useStyle = 1;
                    this.name = "Purification Powder";
                    this.shootSpeed = 4f;
                    this.shoot = 10;
                    this.width = 16;
                    this.height = 24;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.useSound = 1;
                    this.useAnimation = 15;
                    this.useTime = 15;
                    this.noMelee = true;
                    this.toolTip = "Cleanses the corruption";
                    this.value = 75;
                    break;
                case 67:
                    this.damage = 0;
                    this.useStyle = 1;
                    this.name = "Vile Powder";
                    this.shootSpeed = 4f;
                    this.shoot = 11;
                    this.width = 16;
                    this.height = 24;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.useSound = 1;
                    this.useAnimation = 15;
                    this.useTime = 15;
                    this.noMelee = true;
                    this.value = 100;
                    this.toolTip = "Removes the Hallow";
                    break;
                case 68:
                    this.name = "Rotten Chunk";
                    this.width = 18;
                    this.height = 20;
                    this.maxStack = 99;
                    this.toolTip = "'Looks tasty!'";
                    this.value = 10;
                    break;
                case 69:
                    this.name = "Worm Tooth";
                    this.width = 8;
                    this.height = 20;
                    this.maxStack = 99;
                    this.value = 100;
                    break;
                case 70:
                    this.useStyle = 4;
                    this.consumable = true;
                    this.useAnimation = 45;
                    this.useTime = 45;
                    this.name = "Worm Food";
                    this.width = 28;
                    this.height = 28;
                    this.maxStack = 20;
                    this.toolTip = "Summons the Eater of Worlds";
                    break;
                case 71:
                    this.name = "Copper Coin";
                    this.width = 10;
                    this.height = 12;
                    this.maxStack = 100;
                    this.value = 5;
                    break;
                case 72:
                    this.name = "Silver Coin";
                    this.width = 10;
                    this.height = 12;
                    this.maxStack = 100;
                    this.value = 500;
                    break;
                case 73:
                    this.name = "Gold Coin";
                    this.width = 10;
                    this.height = 12;
                    this.maxStack = 100;
                    this.value = 50000;
                    break;
                case 74:
                    this.name = "Platinum Coin";
                    this.width = 10;
                    this.height = 12;
                    this.maxStack = 100;
                    this.value = 5000000;
                    break;
                case 75:
                    this.name = "Fallen Star";
                    this.width = 18;
                    this.height = 20;
                    this.maxStack = 100;
                    this.alpha = 75;
                    this.ammo = 15;
                    this.toolTip = "Disappears after the sunrise";
                    this.value = 500;
                    this.useStyle = 4;
                    this.useSound = 4;
                    this.useTurn = false;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.consumable = true;
                    this.rare = 1;
                    break;
                case 76:
                    this.name = "Copper Greaves";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 1;
                    this.legSlot = 1;
                    this.value = 750;
                    break;
                case 77:
                    this.name = "Iron Greaves";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 2;
                    this.legSlot = 2;
                    this.value = 3000;
                    break;
                case 78:
                    this.name = "Silver Greaves";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 3;
                    this.legSlot = 3;
                    this.value = 7500;
                    break;
                case 79:
                    this.name = "Gold Greaves";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 4;
                    this.legSlot = 4;
                    this.value = 15000;
                    break;
                case 80:
                    this.name = "Copper Chainmail";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 2;
                    this.bodySlot = 1;
                    this.value = 1000;
                    break;
                case 81:
                    this.name = "Iron Chainmail";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 3;
                    this.bodySlot = 2;
                    this.value = 4000;
                    break;
                case 82:
                    this.name = "Silver Chainmail";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 4;
                    this.bodySlot = 3;
                    this.value = 10000;
                    break;
                case 83:
                    this.name = "Gold Chainmail";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 5;
                    this.bodySlot = 4;
                    this.value = 20000;
                    break;
                case 84:
                    this.noUseGraphic = true;
                    this.damage = 0;
                    this.knockBack = 7f;
                    this.useStyle = 5;
                    this.name = "Grappling Hook";
                    this.shootSpeed = 11f;
                    this.shoot = 13;
                    this.width = 18;
                    this.height = 28;
                    this.useSound = 1;
                    this.useAnimation = 20;
                    this.useTime = 20;
                    this.rare = 1;
                    this.noMelee = true;
                    this.value = 20000;
                    this.toolTip = "'Get over here!'";
                    break;
                case 85:
                    this.name = "Iron Chain";
                    this.width = 14;
                    this.height = 20;
                    this.maxStack = 99;
                    this.value = 1000;
                    break;
                case 86:
                    this.name = "Shadow Scale";
                    this.width = 14;
                    this.height = 18;
                    this.maxStack = 99;
                    this.rare = 1;
                    this.value = 500;
                    break;
                case 87:
                    this.name = "Piggy Bank";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 29;
                    this.width = 20;
                    this.height = 12;
                    this.value = 10000;
                    break;
                case 88:
                    this.name = "Mining Helmet";
                    this.width = 22;
                    this.height = 16;
                    this.defense = 1;
                    this.headSlot = 11;
                    this.rare = 1;
                    this.value = 80000;
                    this.toolTip = "Provides light when worn";
                    break;
                case 89:
                    this.name = "Copper Helmet";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 1;
                    this.headSlot = 1;
                    this.value = 1250;
                    break;
                case 90:
                    this.name = "Iron Helmet";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 2;
                    this.headSlot = 2;
                    this.value = 5000;
                    break;
                case 91:
                    this.name = "Silver Helmet";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 3;
                    this.headSlot = 3;
                    this.value = 12500;
                    break;
                case 92:
                    this.name = "Gold Helmet";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 4;
                    this.headSlot = 4;
                    this.value = 25000;
                    break;
                case 93:
                    this.name = "Wood Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 7;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 4;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 94:
                    this.name = "Wood Platform";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 19;
                    this.width = 8;
                    this.height = 10;
                    break;
                case 95:
                    this.useStyle = 5;
                    this.useAnimation = 16;
                    this.useTime = 16;
                    this.name = "Flintlock Pistol";
                    this.width = 24;
                    this.height = 28;
                    this.shoot = 14;
                    this.useAmmo = 14;
                    this.useSound = 11;
                    this.damage = 10;
                    this.shootSpeed = 5f;
                    this.noMelee = true;
                    this.value = 50000;
                    this.scale = 0.9f;
                    this.rare = 1;
                    this.ranged = true;
                    break;
                case 96:
                    this.useStyle = 5;
                    this.autoReuse = true;
                    this.useAnimation = 43;
                    this.useTime = 43;
                    this.name = "Musket";
                    this.width = 44;
                    this.height = 14;
                    this.shoot = 10;
                    this.useAmmo = 14;
                    this.useSound = 11;
                    this.damage = 23;
                    this.shootSpeed = 8f;
                    this.noMelee = true;
                    this.value = 100000;
                    this.knockBack = 4f;
                    this.rare = 1;
                    this.ranged = true;
                    break;
                case 97:
                    this.name = "Musket Ball";
                    this.shootSpeed = 4f;
                    this.shoot = 14;
                    this.damage = 7;
                    this.width = 8;
                    this.height = 8;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.ammo = 14;
                    this.knockBack = 2f;
                    this.value = 7;
                    this.ranged = true;
                    break;
                case 98:
                    this.useStyle = 5;
                    this.autoReuse = true;
                    this.useAnimation = 8;
                    this.useTime = 8;
                    this.name = "Minishark";
                    this.width = 50;
                    this.height = 18;
                    this.shoot = 10;
                    this.useAmmo = 14;
                    this.useSound = 11;
                    this.damage = 6;
                    this.shootSpeed = 7f;
                    this.noMelee = true;
                    this.value = 350000;
                    this.rare = 2;
                    this.toolTip = "33% chance to not consume ammo";
                    this.toolTip2 = "'Half shark, half gun, completely awesome.'";
                    this.ranged = true;
                    break;
                case 99:
                    this.useStyle = 5;
                    this.useAnimation = 28;
                    this.useTime = 28;
                    this.name = "Iron Bow";
                    this.width = 12;
                    this.height = 28;
                    this.shoot = 1;
                    this.useAmmo = 1;
                    this.useSound = 5;
                    this.damage = 8;
                    this.shootSpeed = 6.6f;
                    this.noMelee = true;
                    this.value = 1400;
                    this.ranged = true;
                    break;
                case 100:
                    this.name = "Shadow Greaves";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 6;
                    this.legSlot = 5;
                    this.rare = 1;
                    this.value = 22500;
                    this.toolTip = "7% increased melee speed";
                    break;
                case 101:
                    this.name = "Shadow Scalemail";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 7;
                    this.bodySlot = 5;
                    this.rare = 1;
                    this.value = 30000;
                    this.toolTip = "7% increased melee speed";
                    break;
                case 102:
                    this.name = "Shadow Helmet";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 6;
                    this.headSlot = 5;
                    this.rare = 1;
                    this.value = 37500;
                    this.toolTip = "7% increased melee speed";
                    break;
                case 103:
                    this.name = "Nightmare Pickaxe";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 20;
                    this.useTime = 15;
                    this.autoReuse = true;
                    this.width = 24;
                    this.height = 28;
                    this.damage = 9;
                    this.pick = 65;
                    this.useSound = 1;
                    this.knockBack = 3f;
                    this.rare = 1;
                    this.value = 18000;
                    this.scale = 1.15f;
                    this.toolTip = "Able to mine Hellstone";
                    this.melee = true;
                    break;
                case 104:
                    this.name = "The Breaker";
                    this.autoReuse = true;
                    this.useStyle = 1;
                    this.useAnimation = 45;
                    this.useTime = 19;
                    this.hammer = 55;
                    this.width = 24;
                    this.height = 28;
                    this.damage = 24;
                    this.knockBack = 6f;
                    this.scale = 1.3f;
                    this.useSound = 1;
                    this.rare = 1;
                    this.value = 15000;
                    this.melee = true;
                    break;
                case 105:
                    this.noWet = true;
                    this.name = "Candle";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 33;
                    this.width = 8;
                    this.height = 18;
                    this.holdStyle = 1;
                    break;
                case 106:
                    this.name = "Copper Chandelier";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 34;
                    this.width = 26;
                    this.height = 26;
                    break;
                case 107:
                    this.name = "Silver Chandelier";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 35;
                    this.width = 26;
                    this.height = 26;
                    break;
                case 108:
                    this.name = "Gold Chandelier";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 36;
                    this.width = 26;
                    this.height = 26;
                    break;
                case 109:
                    this.name = "Mana Crystal";
                    this.maxStack = 99;
                    this.consumable = true;
                    this.width = 18;
                    this.height = 18;
                    this.useStyle = 4;
                    this.useTime = 30;
                    this.useSound = 29;
                    this.useAnimation = 30;
                    this.toolTip = "Permanently increases maximum mana by 20";
                    this.rare = 2;
                    break;
                case 110:
                    this.name = "Lesser Mana Potion";
                    this.useSound = 3;
                    this.healMana = 50;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 20;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.value = 100;
                    break;
                case 111:
                    this.name = "Band of Starpower";
                    this.width = 22;
                    this.height = 22;
                    this.accessory = true;
                    this.rare = 1;
                    this.toolTip = "Increases maximum mana by 20";
                    this.value = 50000;
                    break;
                case 112:
                    this.mana = 17;
                    this.damage = 44;
                    this.useStyle = 1;
                    this.name = "Flower of Fire";
                    this.shootSpeed = 6f;
                    this.shoot = 15;
                    this.width = 26;
                    this.height = 28;
                    this.useSound = 20;
                    this.useAnimation = 20;
                    this.useTime = 20;
                    this.rare = 3;
                    this.noMelee = true;
                    this.knockBack = 5.5f;
                    this.toolTip = "Throws balls of fire";
                    this.value = 10000;
                    this.magic = true;
                    break;
                case 113:
                    this.mana = 10;
                    this.channel = true;
                    this.damage = 22;
                    this.useStyle = 1;
                    this.name = "Magic Missile";
                    this.shootSpeed = 6f;
                    this.shoot = 16;
                    this.width = 26;
                    this.height = 28;
                    this.useSound = 9;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.rare = 2;
                    this.noMelee = true;
                    this.knockBack = 5f;
                    this.toolTip = "Casts a controllable missile";
                    this.value = 10000;
                    this.magic = true;
                    break;
                case 114:
                    this.mana = 5;
                    this.channel = true;
                    this.damage = 0;
                    this.useStyle = 1;
                    this.name = "Dirt Rod";
                    this.shoot = 17;
                    this.width = 26;
                    this.height = 28;
                    this.useSound = 8;
                    this.useAnimation = 20;
                    this.useTime = 20;
                    this.rare = 1;
                    this.noMelee = true;
                    this.knockBack = 5f;
                    this.toolTip = "Magically moves dirt";
                    this.value = 200000;
                    break;
                case 115:
                    this.mana = 40;
                    this.channel = true;
                    this.damage = 0;
                    this.useStyle = 4;
                    this.name = "Orb of Light";
                    this.shoot = 18;
                    this.width = 24;
                    this.height = 24;
                    this.useSound = 8;
                    this.useAnimation = 20;
                    this.useTime = 20;
                    this.rare = 1;
                    this.noMelee = true;
                    this.toolTip = "Creates a magical orb of light";
                    this.value = 10000;
                    this.buffType = 19;
                    this.buffTime = 18000;
                    break;
                case 116:
                    this.name = "Meteorite";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 37;
                    this.width = 12;
                    this.height = 12;
                    this.value = 1000;
                    break;
                case 117:
                    this.name = "Meteorite Bar";
                    this.width = 20;
                    this.height = 20;
                    this.maxStack = 99;
                    this.rare = 1;
                    this.toolTip = "'Warm to the touch'";
                    this.value = 7000;
                    break;
                case 118:
                    this.name = "Hook";
                    this.maxStack = 99;
                    this.width = 18;
                    this.height = 18;
                    this.value = 1000;
                    this.toolTip = "Sometimes dropped by Skeletons and Piranha";
                    break;
                case 119:
                    this.noMelee = true;
                    this.useStyle = 1;
                    this.name = "Flamarang";
                    this.shootSpeed = 11f;
                    this.shoot = 19;
                    this.damage = 32;
                    this.knockBack = 8f;
                    this.width = 14;
                    this.height = 28;
                    this.useSound = 1;
                    this.useAnimation = 15;
                    this.useTime = 15;
                    this.noUseGraphic = true;
                    this.rare = 3;
                    this.value = 100000;
                    this.melee = true;
                    break;
                case 120:
                    this.useStyle = 5;
                    this.useAnimation = 25;
                    this.useTime = 25;
                    this.name = "Molten Fury";
                    this.width = 14;
                    this.height = 32;
                    this.shoot = 1;
                    this.useAmmo = 1;
                    this.useSound = 5;
                    this.damage = 29;
                    this.shootSpeed = 8f;
                    this.knockBack = 2f;
                    this.alpha = 30;
                    this.rare = 3;
                    this.noMelee = true;
                    this.scale = 1.1f;
                    this.value = 27000;
                    this.toolTip = "Lights wooden arrows ablaze";
                    this.ranged = true;
                    break;
                case 121:
                    this.name = "Fiery Greatsword";
                    this.useStyle = 1;
                    this.useAnimation = 34;
                    this.knockBack = 6.5f;
                    this.width = 24;
                    this.height = 28;
                    this.damage = 36;
                    this.scale = 1.3f;
                    this.useSound = 1;
                    this.rare = 3;
                    this.value = 27000;
                    this.toolTip = "'It's made out of fire!'";
                    this.melee = true;
                    break;
                case 122:
                    this.name = "Molten Pickaxe";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 25;
                    this.useTime = 25;
                    this.autoReuse = true;
                    this.width = 24;
                    this.height = 28;
                    this.damage = 12;
                    this.pick = 100;
                    this.scale = 1.15f;
                    this.useSound = 1;
                    this.knockBack = 2f;
                    this.rare = 3;
                    this.value = 27000;
                    this.melee = true;
                    break;
                case 123:
                    this.name = "Meteor Helmet";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 3;
                    this.headSlot = 6;
                    this.rare = 1;
                    this.value = 45000;
                    this.toolTip = "5% increased magic damage";
                    break;
                case 124:
                    this.name = "Meteor Suit";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 4;
                    this.bodySlot = 6;
                    this.rare = 1;
                    this.value = 30000;
                    this.toolTip = "5% increased magic damage";
                    break;
                case 125:
                    this.name = "Meteor Leggings";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 3;
                    this.legSlot = 6;
                    this.rare = 1;
                    this.value = 30000;
                    this.toolTip = "5% increased magic damage";
                    break;
                case 126:
                    this.name = "Bottled Water";
                    this.useSound = 3;
                    this.healLife = 20;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.potion = true;
                    this.value = 20;
                    break;
                case 127:
                    this.autoReuse = true;
                    this.useStyle = 5;
                    this.useAnimation = 19;
                    this.useTime = 19;
                    this.name = "Space Gun";
                    this.width = 24;
                    this.height = 28;
                    this.shoot = 20;
                    this.mana = 8;
                    this.useSound = 12;
                    this.knockBack = 0.5f;
                    this.damage = 17;
                    this.shootSpeed = 10f;
                    this.noMelee = true;
                    this.scale = 0.8f;
                    this.rare = 1;
                    this.magic = true;
                    this.value = 20000;
                    break;
                case 128:
                    this.name = "Rocket Boots";
                    this.width = 28;
                    this.height = 24;
                    this.accessory = true;
                    this.rare = 3;
                    this.toolTip = "Allows flight";
                    this.value = 50000;
                    break;
                case 129:
                    this.name = "Gray Brick";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 38;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 130:
                    this.name = "Gray Brick Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 7;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 5;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 131:
                    this.name = "Red Brick";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 39;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 132:
                    this.name = "Red Brick Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 7;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 6;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 133:
                    this.name = "Clay Block";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 40;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 134:
                    this.name = "Blue Brick";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 41;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 135:
                    this.name = "Blue Brick Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 7;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 17;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 136:
                    this.name = "Chain Lantern";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 42;
                    this.width = 12;
                    this.height = 28;
                    break;
                case 137:
                    this.name = "Green Brick";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 43;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 138:
                    this.name = "Green Brick Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 7;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 18;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 139:
                    this.name = "Pink Brick";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 44;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 140:
                    this.name = "Pink Brick Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 7;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 19;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 141:
                    this.name = "Gold Brick";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 45;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 142:
                    this.name = "Gold Brick Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 7;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 10;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 143:
                    this.name = "Silver Brick";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 46;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 144:
                    this.name = "Silver Brick Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 7;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 11;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 145:
                    this.name = "Copper Brick";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 47;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 146:
                    this.name = "Copper Brick Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 7;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 12;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 147:
                    this.name = "Spike";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 48;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 148:
                    this.noWet = true;
                    this.name = "Water Candle";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 49;
                    this.width = 8;
                    this.height = 18;
                    this.holdStyle = 1;
                    this.toolTip = "Holding this may attract unwanted attention";
                    break;
                case 149:
                    this.name = "Book";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 50;
                    this.width = 24;
                    this.height = 28;
                    this.toolTip = "'It contains strange symbols'";
                    break;
                case 150:
                    this.name = "Cobweb";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 51;
                    this.width = 20;
                    this.height = 24;
                    this.alpha = 100;
                    break;
                case 151:
                    this.name = "Necro Helmet";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 5;
                    this.headSlot = 7;
                    this.rare = 2;
                    this.value = 45000;
                    this.toolTip = "4% increased ranged damage.";
                    break;
                case 152:
                    this.name = "Necro Breastplate";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 6;
                    this.bodySlot = 7;
                    this.rare = 2;
                    this.value = 30000;
                    this.toolTip = "4% increased ranged damage.";
                    break;
                case 153:
                    this.name = "Necro Greaves";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 5;
                    this.legSlot = 7;
                    this.rare = 2;
                    this.value = 30000;
                    this.toolTip = "4% increased ranged damage.";
                    break;
                case 154:
                    this.name = "Bone";
                    this.maxStack = 99;
                    this.consumable = true;
                    this.width = 12;
                    this.height = 14;
                    this.value = 50;
                    this.useAnimation = 12;
                    this.useTime = 12;
                    this.useStyle = 1;
                    this.useSound = 1;
                    this.shootSpeed = 8f;
                    this.noUseGraphic = true;
                    this.damage = 22;
                    this.knockBack = 4f;
                    this.shoot = 21;
                    this.ranged = true;
                    break;
                case 155:
                    this.autoReuse = true;
                    this.useTurn = true;
                    this.name = "Muramasa";
                    this.useStyle = 1;
                    this.useAnimation = 20;
                    this.width = 40;
                    this.height = 40;
                    this.damage = 18;
                    this.scale = 1.1f;
                    this.useSound = 1;
                    this.rare = 2;
                    this.value = 27000;
                    this.knockBack = 1f;
                    this.melee = true;
                    break;
                case 156:
                    this.name = "Cobalt Shield";
                    this.width = 24;
                    this.height = 28;
                    this.rare = 2;
                    this.value = 27000;
                    this.accessory = true;
                    this.defense = 1;
                    this.toolTip = "Grants immunity to knockback";
                    break;
                case 157:
                    this.mana = 7;
                    this.autoReuse = true;
                    this.name = "Aqua Scepter";
                    this.useStyle = 5;
                    this.useAnimation = 16;
                    this.useTime = 8;
                    this.knockBack = 5f;
                    this.width = 38;
                    this.height = 10;
                    this.damage = 14;
                    this.scale = 1f;
                    this.shoot = 22;
                    this.shootSpeed = 11f;
                    this.useSound = 13;
                    this.rare = 2;
                    this.value = 27000;
                    this.toolTip = "Sprays out a shower of water";
                    this.magic = true;
                    break;
                case 158:
                    this.name = "Lucky Horseshoe";
                    this.width = 20;
                    this.height = 22;
                    this.rare = 1;
                    this.value = 27000;
                    this.accessory = true;
                    this.toolTip = "Negates fall damage";
                    break;
                case 159:
                    this.name = "Shiny Red Balloon";
                    this.width = 14;
                    this.height = 28;
                    this.rare = 1;
                    this.value = 27000;
                    this.accessory = true;
                    this.toolTip = "Increases jump height";
                    break;
                case 160:
                    this.autoReuse = true;
                    this.name = "Harpoon";
                    this.useStyle = 5;
                    this.useAnimation = 30;
                    this.useTime = 30;
                    this.knockBack = 6f;
                    this.width = 30;
                    this.height = 10;
                    this.damage = 25;
                    this.scale = 1.1f;
                    this.shoot = 23;
                    this.shootSpeed = 11f;
                    this.useSound = 10;
                    this.rare = 2;
                    this.value = 27000;
                    this.ranged = true;
                    break;
                case 161:
                    this.useStyle = 1;
                    this.name = "Spiky Ball";
                    this.shootSpeed = 5f;
                    this.shoot = 24;
                    this.knockBack = 1f;
                    this.damage = 15;
                    this.width = 10;
                    this.height = 10;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.useSound = 1;
                    this.useAnimation = 15;
                    this.useTime = 15;
                    this.noUseGraphic = true;
                    this.noMelee = true;
                    this.value = 80;
                    this.ranged = true;
                    break;
                case 162:
                    this.name = "Ball O' Hurt";
                    this.useStyle = 5;
                    this.useAnimation = 45;
                    this.useTime = 45;
                    this.knockBack = 6.5f;
                    this.width = 30;
                    this.height = 10;
                    this.damage = 15;
                    this.scale = 1.1f;
                    this.noUseGraphic = true;
                    this.shoot = 25;
                    this.shootSpeed = 12f;
                    this.useSound = 1;
                    this.rare = 1;
                    this.value = 27000;
                    this.melee = true;
                    this.channel = true;
                    this.noMelee = true;
                    break;
                case 163:
                    this.name = "Blue Moon";
                    this.useStyle = 5;
                    this.useAnimation = 45;
                    this.useTime = 45;
                    this.knockBack = 7f;
                    this.width = 30;
                    this.height = 10;
                    this.damage = 23;
                    this.scale = 1.1f;
                    this.noUseGraphic = true;
                    this.shoot = 26;
                    this.shootSpeed = 12f;
                    this.useSound = 1;
                    this.rare = 2;
                    this.value = 27000;
                    this.melee = true;
                    this.channel = true;
                    break;
                case 164:
                    this.autoReuse = false;
                    this.useStyle = 5;
                    this.useAnimation = 12;
                    this.useTime = 12;
                    this.name = "Handgun";
                    this.width = 24;
                    this.height = 24;
                    this.shoot = 14;
                    this.knockBack = 3f;
                    this.useAmmo = 14;
                    this.useSound = 11;
                    this.damage = 14;
                    this.shootSpeed = 10f;
                    this.noMelee = true;
                    this.value = 50000;
                    this.scale = 0.75f;
                    this.rare = 2;
                    this.ranged = true;
                    break;
                case 165:
                    this.autoReuse = true;
                    this.rare = 2;
                    this.mana = 14;
                    this.useSound = 21;
                    this.name = "Water Bolt";
                    this.useStyle = 5;
                    this.damage = 17;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.width = 24;
                    this.height = 28;
                    this.shoot = 27;
                    this.scale = 0.9f;
                    this.shootSpeed = 4.5f;
                    this.knockBack = 5f;
                    this.toolTip = "Casts a slow moving bolt of water";
                    this.magic = true;
                    this.value = 50000;
                    break;
                case 166:
                    this.useStyle = 1;
                    this.name = "Bomb";
                    this.shootSpeed = 5f;
                    this.shoot = 28;
                    this.width = 20;
                    this.height = 20;
                    this.maxStack = 50;
                    this.consumable = true;
                    this.useSound = 1;
                    this.useAnimation = 25;
                    this.useTime = 25;
                    this.noUseGraphic = true;
                    this.noMelee = true;
                    this.value = 500;
                    this.damage = 0;
                    this.toolTip = "A small explosion that will destroy some tiles";
                    break;
                case 167:
                    this.useStyle = 1;
                    this.name = "Dynamite";
                    this.shootSpeed = 4f;
                    this.shoot = 29;
                    this.width = 8;
                    this.height = 28;
                    this.maxStack = 5;
                    this.consumable = true;
                    this.useSound = 1;
                    this.useAnimation = 40;
                    this.useTime = 40;
                    this.noUseGraphic = true;
                    this.noMelee = true;
                    this.value = 5000;
                    this.rare = 1;
                    this.toolTip = "A large explosion that will destroy most tiles";
                    break;
                case 168:
                    this.useStyle = 5;
                    this.name = "Grenade";
                    this.shootSpeed = 5.5f;
                    this.shoot = 30;
                    this.width = 20;
                    this.height = 20;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.useSound = 1;
                    this.useAnimation = 45;
                    this.useTime = 45;
                    this.noUseGraphic = true;
                    this.noMelee = true;
                    this.value = 400;
                    this.damage = 60;
                    this.knockBack = 8f;
                    this.toolTip = "A small explosion that will not destroy tiles";
                    this.ranged = true;
                    break;
                case 169:
                    this.name = "Sand Block";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 53;
                    this.width = 12;
                    this.height = 12;
                    this.ammo = 42;
                    break;
                case 170:
                    this.name = "Glass";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 54;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 171:
                    this.name = "Sign";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 55;
                    this.width = 28;
                    this.height = 28;
                    break;
                case 172:
                    this.name = "Ash Block";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 57;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 173:
                    this.name = "Obsidian";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 56;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 174:
                    this.name = "Hellstone";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 58;
                    this.width = 12;
                    this.height = 12;
                    this.rare = 2;
                    break;
                case 175:
                    this.name = "Hellstone Bar";
                    this.width = 20;
                    this.height = 20;
                    this.maxStack = 99;
                    this.rare = 2;
                    this.toolTip = "'Hot to the touch'";
                    this.value = 20000;
                    break;
                case 176:
                    this.name = "Mud Block";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 59;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 181:
                    this.name = "Amethyst";
                    this.maxStack = 99;
                    this.alpha = 50;
                    this.width = 10;
                    this.height = 14;
                    this.value = 1875;
                    break;
                case 180:
                    this.name = "Topaz";
                    this.maxStack = 99;
                    this.alpha = 50;
                    this.width = 10;
                    this.height = 14;
                    this.value = 3750;
                    break;
                case 177:
                    this.name = "Sapphire";
                    this.maxStack = 99;
                    this.alpha = 50;
                    this.width = 10;
                    this.height = 14;
                    this.value = 5625;
                    break;
                case 179:
                    this.name = "Emerald";
                    this.maxStack = 99;
                    this.alpha = 50;
                    this.width = 10;
                    this.height = 14;
                    this.value = 7500;
                    break;
                case 178:
                    this.name = "Ruby";
                    this.maxStack = 99;
                    this.alpha = 50;
                    this.width = 10;
                    this.height = 14;
                    this.value = 11250;
                    break;
                case 182:
                    this.name = "Diamond";
                    this.maxStack = 99;
                    this.alpha = 50;
                    this.width = 10;
                    this.height = 14;
                    this.value = 15000;
                    break;
                case 183:
                    this.name = "Glowing Mushroom";
                    this.useStyle = 2;
                    this.useSound = 2;
                    this.useTurn = false;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.width = 16;
                    this.height = 18;
                    this.healLife = 25;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.potion = true;
                    this.value = 50;
                    break;
                case 184:
                    this.name = "Star";
                    this.width = 12;
                    this.height = 12;
                    break;
                case 185:
                    this.noUseGraphic = true;
                    this.damage = 0;
                    this.knockBack = 7f;
                    this.useStyle = 5;
                    this.name = "Ivy Whip";
                    this.shootSpeed = 13f;
                    this.shoot = 32;
                    this.width = 18;
                    this.height = 28;
                    this.useSound = 1;
                    this.useAnimation = 20;
                    this.useTime = 20;
                    this.rare = 3;
                    this.noMelee = true;
                    this.value = 20000;
                    break;
                case 186:
                    this.name = "Breathing Reed";
                    this.width = 44;
                    this.height = 44;
                    this.rare = 1;
                    this.value = 10000;
                    this.holdStyle = 2;
                    this.toolTip = "'Because not drowning is kinda nice'";
                    break;
                case 187:
                    this.name = "Flipper";
                    this.width = 28;
                    this.height = 28;
                    this.rare = 1;
                    this.value = 10000;
                    this.accessory = true;
                    this.toolTip = "Grants the ability to swim";
                    break;
                case 188:
                    this.name = "Healing Potion";
                    this.useSound = 3;
                    this.healLife = 100;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.rare = 1;
                    this.potion = true;
                    this.value = 1000;
                    break;
                case 189:
                    this.name = "Mana Potion";
                    this.useSound = 3;
                    this.healMana = 100;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 50;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.rare = 1;
                    this.value = 250;
                    break;
                case 190:
                    this.name = "Blade of Grass";
                    this.useStyle = 1;
                    this.useAnimation = 30;
                    this.knockBack = 3f;
                    this.width = 40;
                    this.height = 40;
                    this.damage = 28;
                    this.scale = 1.4f;
                    this.useSound = 1;
                    this.rare = 3;
                    this.value = 27000;
                    this.melee = true;
                    break;
                case 191:
                    this.noMelee = true;
                    this.useStyle = 1;
                    this.name = "Thorn Chakram";
                    this.shootSpeed = 11f;
                    this.shoot = 33;
                    this.damage = 25;
                    this.knockBack = 8f;
                    this.width = 14;
                    this.height = 28;
                    this.useSound = 1;
                    this.useAnimation = 15;
                    this.useTime = 15;
                    this.noUseGraphic = true;
                    this.rare = 3;
                    this.value = 50000;
                    this.melee = true;
                    break;
                case 192:
                    this.name = "Obsidian Brick";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 75;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 193:
                    this.name = "Obsidian Skull";
                    this.width = 20;
                    this.height = 22;
                    this.rare = 2;
                    this.value = 27000;
                    this.accessory = true;
                    this.defense = 1;
                    this.toolTip = "Grants immunity to fire blocks";
                    break;
                case 194:
                    this.name = "Mushroom Grass Seeds";
                    this.useTurn = true;
                    this.useStyle = 1;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 70;
                    this.width = 14;
                    this.height = 14;
                    this.value = 150;
                    break;
                case 195:
                    this.name = "Jungle Grass Seeds";
                    this.useTurn = true;
                    this.useStyle = 1;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 60;
                    this.width = 14;
                    this.height = 14;
                    this.value = 150;
                    break;
                case 196:
                    this.name = "Wooden Hammer";
                    this.autoReuse = true;
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 37;
                    this.useTime = 25;
                    this.hammer = 25;
                    this.width = 24;
                    this.height = 28;
                    this.damage = 2;
                    this.knockBack = 5.5f;
                    this.scale = 1.2f;
                    this.useSound = 1;
                    this.tileBoost = -1;
                    this.value = 50;
                    this.melee = true;
                    break;
                case 197:
                    this.autoReuse = true;
                    this.useStyle = 5;
                    this.useAnimation = 12;
                    this.useTime = 12;
                    this.name = "Star Cannon";
                    this.width = 50;
                    this.height = 18;
                    this.shoot = 12;
                    this.useAmmo = 15;
                    this.useSound = 9;
                    this.damage = 55;
                    this.shootSpeed = 14f;
                    this.noMelee = true;
                    this.value = 500000;
                    this.rare = 2;
                    this.toolTip = "Shoots fallen stars";
                    this.ranged = true;
                    break;
                case 198:
                    this.name = "Blue Phaseblade";
                    this.useStyle = 1;
                    this.useAnimation = 25;
                    this.knockBack = 3f;
                    this.width = 40;
                    this.height = 40;
                    this.damage = 21;
                    this.scale = 1f;
                    this.useSound = 15;
                    this.rare = 1;
                    this.value = 27000;
                    this.melee = true;
                    break;
                case 199:
                    this.name = "Red Phaseblade";
                    this.useStyle = 1;
                    this.useAnimation = 25;
                    this.knockBack = 3f;
                    this.width = 40;
                    this.height = 40;
                    this.damage = 21;
                    this.scale = 1f;
                    this.useSound = 15;
                    this.rare = 1;
                    this.value = 27000;
                    this.melee = true;
                    break;
                case 200:
                    this.name = "Green Phaseblade";
                    this.useStyle = 1;
                    this.useAnimation = 25;
                    this.knockBack = 3f;
                    this.width = 40;
                    this.height = 40;
                    this.damage = 21;
                    this.scale = 1f;
                    this.useSound = 15;
                    this.rare = 1;
                    this.value = 27000;
                    this.melee = true;
                    break;
                case 201:
                    this.name = "Purple Phaseblade";
                    this.useStyle = 1;
                    this.useAnimation = 25;
                    this.knockBack = 3f;
                    this.width = 40;
                    this.height = 40;
                    this.damage = 21;
                    this.scale = 1f;
                    this.useSound = 15;
                    this.rare = 1;
                    this.value = 27000;
                    this.melee = true;
                    break;
                case 202:
                    this.name = "White Phaseblade";
                    this.useStyle = 1;
                    this.useAnimation = 25;
                    this.knockBack = 3f;
                    this.width = 40;
                    this.height = 40;
                    this.damage = 21;
                    this.scale = 1f;
                    this.useSound = 15;
                    this.rare = 1;
                    this.value = 27000;
                    this.melee = true;
                    break;
                case 203:
                    this.name = "Yellow Phaseblade";
                    this.useStyle = 1;
                    this.useAnimation = 25;
                    this.knockBack = 3f;
                    this.width = 40;
                    this.height = 40;
                    this.damage = 21;
                    this.scale = 1f;
                    this.useSound = 15;
                    this.rare = 1;
                    this.value = 27000;
                    this.melee = true;
                    break;
                case 204:
                    this.name = "Meteor Hamaxe";
                    this.useTurn = true;
                    this.autoReuse = true;
                    this.useStyle = 1;
                    this.useAnimation = 30;
                    this.useTime = 16;
                    this.hammer = 60;
                    this.axe = 20;
                    this.width = 24;
                    this.height = 28;
                    this.damage = 20;
                    this.knockBack = 7f;
                    this.scale = 1.2f;
                    this.useSound = 1;
                    this.rare = 1;
                    this.value = 15000;
                    this.melee = true;
                    break;
                case 205:
                    this.name = "Empty Bucket";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.width = 20;
                    this.height = 20;
                    this.headSlot = 13;
                    this.defense = 1;
                    break;
                case 206:
                    this.name = "Water Bucket";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.width = 20;
                    this.height = 20;
                    break;
                case 207:
                    this.name = "Lava Bucket";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.width = 20;
                    this.height = 20;
                    break;
                case 208:
                    this.name = "Jungle Rose";
                    this.width = 20;
                    this.height = 20;
                    this.value = 100;
                    this.headSlot = 23;
                    this.toolTip = "'It's pretty, oh so pretty'";
                    this.vanity = true;
                    break;
                case 209:
                    this.name = "Stinger";
                    this.width = 16;
                    this.height = 18;
                    this.maxStack = 99;
                    this.value = 200;
                    break;
                case 210:
                    this.name = "Vine";
                    this.width = 14;
                    this.height = 20;
                    this.maxStack = 99;
                    this.value = 1000;
                    break;
                case 211:
                    this.name = "Feral Claws";
                    this.width = 20;
                    this.height = 20;
                    this.accessory = true;
                    this.rare = 3;
                    this.toolTip = "12% increased melee speed";
                    this.value = 50000;
                    break;
                case 212:
                    this.name = "Anklet of the Wind";
                    this.width = 20;
                    this.height = 20;
                    this.accessory = true;
                    this.rare = 3;
                    this.toolTip = "10% increased movement speed";
                    this.value = 50000;
                    break;
                case 213:
                    this.name = "Staff of Regrowth";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 25;
                    this.useTime = 13;
                    this.autoReuse = true;
                    this.width = 24;
                    this.height = 28;
                    this.damage = 7;
                    this.createTile = 2;
                    this.scale = 1.2f;
                    this.useSound = 1;
                    this.knockBack = 3f;
                    this.rare = 3;
                    this.value = 2000;
                    this.toolTip = "Creates grass on dirt";
                    this.melee = true;
                    break;
                case 214:
                    this.name = "Hellstone Brick";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 76;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 215:
                    this.name = "Whoopie Cushion";
                    this.width = 18;
                    this.height = 18;
                    this.useTurn = true;
                    this.useTime = 30;
                    this.useAnimation = 30;
                    this.noUseGraphic = true;
                    this.useStyle = 10;
                    this.useSound = 16;
                    this.rare = 2;
                    this.toolTip = "'May annoy others'";
                    this.value = 100;
                    break;
                case 216:
                    this.name = "Shackle";
                    this.width = 20;
                    this.height = 20;
                    this.rare = 1;
                    this.value = 1500;
                    this.accessory = true;
                    this.defense = 1;
                    break;
                case 217:
                    this.name = "Molten Hamaxe";
                    this.useTurn = true;
                    this.autoReuse = true;
                    this.useStyle = 1;
                    this.useAnimation = 27;
                    this.useTime = 14;
                    this.hammer = 70;
                    this.axe = 30;
                    this.width = 24;
                    this.height = 28;
                    this.damage = 20;
                    this.knockBack = 7f;
                    this.scale = 1.4f;
                    this.useSound = 1;
                    this.rare = 3;
                    this.value = 15000;
                    this.melee = true;
                    break;
                case 218:
                    this.mana = 16;
                    this.channel = true;
                    this.damage = 34;
                    this.useStyle = 1;
                    this.name = "Flamelash";
                    this.shootSpeed = 6f;
                    this.shoot = 34;
                    this.width = 26;
                    this.height = 28;
                    this.useSound = 20;
                    this.useAnimation = 20;
                    this.useTime = 20;
                    this.rare = 3;
                    this.noMelee = true;
                    this.knockBack = 6.5f;
                    this.toolTip = "Summons a controllable ball of fire";
                    this.value = 10000;
                    this.magic = true;
                    break;
                case 219:
                    this.autoReuse = false;
                    this.useStyle = 5;
                    this.useAnimation = 11;
                    this.useTime = 11;
                    this.name = "Phoenix Blaster";
                    this.width = 24;
                    this.height = 22;
                    this.shoot = 14;
                    this.knockBack = 2f;
                    this.useAmmo = 14;
                    this.useSound = 11;
                    this.damage = 23;
                    this.shootSpeed = 13f;
                    this.noMelee = true;
                    this.value = 50000;
                    this.scale = 0.75f;
                    this.rare = 3;
                    this.ranged = true;
                    break;
                case 220:
                    this.name = "Sunfury";
                    this.noMelee = true;
                    this.useStyle = 5;
                    this.useAnimation = 45;
                    this.useTime = 45;
                    this.knockBack = 7f;
                    this.width = 30;
                    this.height = 10;
                    this.damage = 33;
                    this.scale = 1.1f;
                    this.noUseGraphic = true;
                    this.shoot = 35;
                    this.shootSpeed = 12f;
                    this.useSound = 1;
                    this.rare = 3;
                    this.value = 27000;
                    this.melee = true;
                    this.channel = true;
                    break;
                case 221:
                    this.name = "Hellforge";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 77;
                    this.width = 26;
                    this.height = 24;
                    this.value = 3000;
                    break;
                case 222:
                    this.name = "Clay Pot";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 78;
                    this.width = 14;
                    this.height = 14;
                    this.value = 100;
                    this.toolTip = "Grows plants";
                    break;
                case 223:
                    this.name = "Nature's Gift";
                    this.width = 20;
                    this.height = 22;
                    this.rare = 3;
                    this.value = 27000;
                    this.accessory = true;
                    this.toolTip = "6% reduced mana usage";
                    break;
                case 224:
                    this.name = "Bed";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 79;
                    this.width = 28;
                    this.height = 20;
                    this.value = 2000;
                    break;
                case 225:
                    this.name = "Silk";
                    this.maxStack = 99;
                    this.width = 22;
                    this.height = 22;
                    this.value = 1000;
                    break;
                case 226:
                    this.name = "Lesser Restoration Potion";
                    this.useSound = 3;
                    this.healMana = 50;
                    this.healLife = 50;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 20;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.potion = true;
                    this.value = 2000;
                    break;
                case 227:
                    this.name = "Restoration Potion";
                    this.useSound = 3;
                    this.healMana = 100;
                    this.healLife = 100;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 20;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.potion = true;
                    this.value = 4000;
                    break;
                case 228:
                    this.name = "Jungle Hat";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 4;
                    this.headSlot = 8;
                    this.rare = 3;
                    this.value = 45000;
                    this.toolTip = "Increases maximum mana by 20";
                    this.toolTip2 = "3% increased magic critical strike chance";
                    break;
                case 229:
                    this.name = "Jungle Shirt";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 5;
                    this.bodySlot = 8;
                    this.rare = 3;
                    this.value = 30000;
                    this.toolTip = "Increases maximum mana by 20";
                    this.toolTip2 = "3% increased magic critical strike chance";
                    break;
                case 230:
                    this.name = "Jungle Pants";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 4;
                    this.legSlot = 8;
                    this.rare = 3;
                    this.value = 30000;
                    this.toolTip = "Increases maximum mana by 20";
                    this.toolTip2 = "3% increased magic critical strike chance";
                    break;
                case 231:
                    this.name = "Molten Helmet";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 8;
                    this.headSlot = 9;
                    this.rare = 3;
                    this.value = 45000;
                    break;
                case 232:
                    this.name = "Molten Breastplate";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 9;
                    this.bodySlot = 9;
                    this.rare = 3;
                    this.value = 30000;
                    break;
                case 233:
                    this.name = "Molten Greaves";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 8;
                    this.legSlot = 9;
                    this.rare = 3;
                    this.value = 30000;
                    break;
                case 234:
                    this.name = "Meteor Shot";
                    this.shootSpeed = 3f;
                    this.shoot = 36;
                    this.damage = 9;
                    this.width = 8;
                    this.height = 8;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.ammo = 14;
                    this.knockBack = 1f;
                    this.value = 8;
                    this.rare = 1;
                    this.ranged = true;
                    break;
                case 235:
                    this.useStyle = 1;
                    this.name = "Sticky Bomb";
                    this.shootSpeed = 5f;
                    this.shoot = 37;
                    this.width = 20;
                    this.height = 20;
                    this.maxStack = 50;
                    this.consumable = true;
                    this.useSound = 1;
                    this.useAnimation = 25;
                    this.useTime = 25;
                    this.noUseGraphic = true;
                    this.noMelee = true;
                    this.value = 500;
                    this.damage = 0;
                    this.toolTip = "'Tossing may be difficult.'";
                    break;
                case 236:
                    this.name = "Black Lens";
                    this.width = 12;
                    this.height = 20;
                    this.maxStack = 99;
                    this.value = 5000;
                    break;
                case 237:
                    this.name = "Sunglasses";
                    this.width = 28;
                    this.height = 12;
                    this.headSlot = 12;
                    this.rare = 2;
                    this.value = 10000;
                    this.toolTip = "'Makes you look cool!'";
                    this.vanity = true;
                    break;
                case 238:
                    this.name = "Wizard Hat";
                    this.width = 28;
                    this.height = 20;
                    this.headSlot = 14;
                    this.rare = 2;
                    this.value = 10000;
                    this.defense = 2;
                    this.toolTip = "15% increased magic damage";
                    break;
                case 239:
                    this.name = "Top Hat";
                    this.width = 18;
                    this.height = 18;
                    this.headSlot = 15;
                    this.value = 10000;
                    this.vanity = true;
                    break;
                case 240:
                    this.name = "Tuxedo Shirt";
                    this.width = 18;
                    this.height = 18;
                    this.bodySlot = 10;
                    this.value = 5000;
                    this.vanity = true;
                    break;
                case 241:
                    this.name = "Tuxedo Pants";
                    this.width = 18;
                    this.height = 18;
                    this.legSlot = 10;
                    this.value = 5000;
                    this.vanity = true;
                    break;
                case 242:
                    this.name = "Summer Hat";
                    this.width = 18;
                    this.height = 18;
                    this.headSlot = 16;
                    this.value = 10000;
                    this.vanity = true;
                    break;
                case 243:
                    this.name = "Bunny Hood";
                    this.width = 18;
                    this.height = 18;
                    this.headSlot = 17;
                    this.value = 20000;
                    this.vanity = true;
                    break;
                case 244:
                    this.name = "Plumber's Hat";
                    this.width = 18;
                    this.height = 12;
                    this.headSlot = 18;
                    this.value = 10000;
                    this.vanity = true;
                    break;
                case 245:
                    this.name = "Plumber's Shirt";
                    this.width = 18;
                    this.height = 18;
                    this.bodySlot = 11;
                    this.value = 250000;
                    this.vanity = true;
                    break;
                case 246:
                    this.name = "Plumber's Pants";
                    this.width = 18;
                    this.height = 18;
                    this.legSlot = 11;
                    this.value = 250000;
                    this.vanity = true;
                    break;
                case 247:
                    this.name = "Hero's Hat";
                    this.width = 18;
                    this.height = 12;
                    this.headSlot = 19;
                    this.value = 10000;
                    this.vanity = true;
                    break;
                case 248:
                    this.name = "Hero's Shirt";
                    this.width = 18;
                    this.height = 18;
                    this.bodySlot = 12;
                    this.value = 5000;
                    this.vanity = true;
                    break;
                case 249:
                    this.name = "Hero's Pants";
                    this.width = 18;
                    this.height = 18;
                    this.legSlot = 12;
                    this.value = 5000;
                    this.vanity = true;
                    break;
                case 250:
                    this.name = "Fish Bowl";
                    this.width = 18;
                    this.height = 18;
                    this.headSlot = 20;
                    this.value = 10000;
                    this.vanity = true;
                    break;
                case 251:
                    this.name = "Archaeologist's Hat";
                    this.width = 18;
                    this.height = 12;
                    this.headSlot = 21;
                    this.value = 10000;
                    this.vanity = true;
                    break;
                case 252:
                    this.name = "Archaeologist's Jacket";
                    this.width = 18;
                    this.height = 18;
                    this.bodySlot = 13;
                    this.value = 5000;
                    this.vanity = true;
                    break;
                case 253:
                    this.name = "Archaeologist's Pants";
                    this.width = 18;
                    this.height = 18;
                    this.legSlot = 13;
                    this.value = 5000;
                    this.vanity = true;
                    break;
                case 254:
                    this.name = "Black Dye";
                    this.maxStack = 99;
                    this.width = 12;
                    this.height = 20;
                    this.value = 10000;
                    break;
                case 255:
                    this.name = "Green Dye";
                    this.maxStack = 99;
                    this.width = 12;
                    this.height = 20;
                    this.value = 2000;
                    break;
                case 256:
                    this.name = "Ninja Hood";
                    this.width = 18;
                    this.height = 12;
                    this.headSlot = 22;
                    this.value = 10000;
                    this.vanity = true;
                    break;
                case 257:
                    this.name = "Ninja Shirt";
                    this.width = 18;
                    this.height = 18;
                    this.bodySlot = 14;
                    this.value = 5000;
                    this.vanity = true;
                    break;
                case 258:
                    this.name = "Ninja Pants";
                    this.width = 18;
                    this.height = 18;
                    this.legSlot = 14;
                    this.value = 5000;
                    this.vanity = true;
                    break;
                case 259:
                    this.name = "Leather";
                    this.width = 18;
                    this.height = 20;
                    this.maxStack = 99;
                    this.value = 50;
                    break;
                case 260:
                    this.name = "Red Hat";
                    this.width = 18;
                    this.height = 14;
                    this.headSlot = 24;
                    this.value = 1000;
                    this.vanity = true;
                    break;
                case 261:
                    this.name = "Goldfish";
                    this.useStyle = 2;
                    this.useSound = 2;
                    this.useTurn = false;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.width = 20;
                    this.height = 10;
                    this.maxStack = 99;
                    this.healLife = 20;
                    this.consumable = true;
                    this.value = 1000;
                    this.potion = true;
                    this.toolTip = "'It's smiling, might be a good snack'";
                    break;
                case 262:
                    this.name = "Robe";
                    this.width = 18;
                    this.height = 14;
                    this.bodySlot = 15;
                    this.value = 2000;
                    this.vanity = true;
                    break;
                case 263:
                    this.name = "Robot Hat";
                    this.width = 18;
                    this.height = 18;
                    this.headSlot = 25;
                    this.value = 10000;
                    this.vanity = true;
                    break;
                case 264:
                    this.name = "Gold Crown";
                    this.width = 18;
                    this.height = 18;
                    this.headSlot = 26;
                    this.value = 10000;
                    this.vanity = true;
                    break;
                case 265:
                    this.name = "Hellfire Arrow";
                    this.shootSpeed = 6.5f;
                    this.shoot = 41;
                    this.damage = 10;
                    this.width = 10;
                    this.height = 28;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.ammo = 1;
                    this.knockBack = 8f;
                    this.value = 100;
                    this.rare = 2;
                    this.ranged = true;
                    break;
                case 266:
                    this.useStyle = 5;
                    this.useAnimation = 16;
                    this.useTime = 16;
                    this.autoReuse = true;
                    this.name = "Sandgun";
                    this.width = 40;
                    this.height = 20;
                    this.shoot = 42;
                    this.useAmmo = 42;
                    this.useSound = 11;
                    this.damage = 30;
                    this.shootSpeed = 12f;
                    this.noMelee = true;
                    this.knockBack = 5f;
                    this.value = 10000;
                    this.rare = 2;
                    this.toolTip = "'This is a good idea!'";
                    this.ranged = true;
                    break;
                case 267:
                    this.accessory = true;
                    this.name = "Guide Voodoo Doll";
                    this.width = 14;
                    this.height = 26;
                    this.value = 1000;
                    this.toolTip = "'You are a terrible person.'";
                    break;
                case 268:
                    this.headSlot = 27;
                    this.defense = 2;
                    this.name = "Diving Helmet";
                    this.width = 20;
                    this.height = 20;
                    this.value = 1000;
                    this.rare = 2;
                    this.toolTip = "Greatly extends underwater breathing";
                    break;
                case 269:
                    this.name = "Familiar Shirt";
                    this.bodySlot = 0;
                    this.width = 20;
                    this.height = 20;
                    this.value = 10000;
                    this.color = Main.player[Main.myPlayer].shirtColor;
                    break;
                case 270:
                    this.name = "Familiar Pants";
                    this.legSlot = 0;
                    this.width = 20;
                    this.height = 20;
                    this.value = 10000;
                    this.color = Main.player[Main.myPlayer].pantsColor;
                    break;
                case 271:
                    this.name = "Familiar Wig";
                    this.headSlot = 0;
                    this.width = 20;
                    this.height = 20;
                    this.value = 10000;
                    this.color = Main.player[Main.myPlayer].hairColor;
                    break;
                case 272:
                    this.mana = 14;
                    this.damage = 35;
                    this.useStyle = 5;
                    this.name = "Demon Scythe";
                    this.shootSpeed = 0.2f;
                    this.shoot = 45;
                    this.width = 26;
                    this.height = 28;
                    this.useSound = 8;
                    this.useAnimation = 20;
                    this.useTime = 20;
                    this.rare = 3;
                    this.noMelee = true;
                    this.knockBack = 5f;
                    this.scale = 0.9f;
                    this.toolTip = "Casts a demon scythe";
                    this.value = 10000;
                    this.magic = true;
                    break;
                case 273:
                    this.name = "Night's Edge";
                    this.useStyle = 1;
                    this.useAnimation = 27;
                    this.useTime = 27;
                    this.knockBack = 4.5f;
                    this.width = 40;
                    this.height = 40;
                    this.damage = 42;
                    this.scale = 1.15f;
                    this.useSound = 1;
                    this.rare = 3;
                    this.value = 27000;
                    this.melee = true;
                    break;
                case 274:
                    this.name = "Dark Lance";
                    this.useStyle = 5;
                    this.useAnimation = 25;
                    this.useTime = 25;
                    this.shootSpeed = 5f;
                    this.knockBack = 4f;
                    this.width = 40;
                    this.height = 40;
                    this.damage = 27;
                    this.scale = 1.1f;
                    this.useSound = 1;
                    this.shoot = 46;
                    this.rare = 3;
                    this.value = 27000;
                    this.noMelee = true;
                    this.noUseGraphic = true;
                    this.melee = true;
                    break;
                case 275:
                    this.name = "Coral";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 81;
                    this.width = 20;
                    this.height = 22;
                    this.value = 400;
                    break;
                case 276:
                    this.name = "Cactus";
                    this.maxStack = 250;
                    this.width = 12;
                    this.height = 12;
                    this.value = 10;
                    break;
                case 277:
                    this.name = "Trident";
                    this.useStyle = 5;
                    this.useAnimation = 31;
                    this.useTime = 31;
                    this.shootSpeed = 4f;
                    this.knockBack = 5f;
                    this.width = 40;
                    this.height = 40;
                    this.damage = 10;
                    this.scale = 1.1f;
                    this.useSound = 1;
                    this.shoot = 47;
                    this.rare = 1;
                    this.value = 10000;
                    this.noMelee = true;
                    this.noUseGraphic = true;
                    this.melee = true;
                    break;
                case 278:
                    this.name = "Silver Bullet";
                    this.shootSpeed = 4.5f;
                    this.shoot = 14;
                    this.damage = 9;
                    this.width = 8;
                    this.height = 8;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.ammo = 14;
                    this.knockBack = 3f;
                    this.value = 15;
                    this.ranged = true;
                    break;
                case 279:
                    this.useStyle = 1;
                    this.name = "Throwing Knife";
                    this.shootSpeed = 10f;
                    this.shoot = 48;
                    this.damage = 12;
                    this.width = 18;
                    this.height = 20;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.useSound = 1;
                    this.useAnimation = 15;
                    this.useTime = 15;
                    this.noUseGraphic = true;
                    this.noMelee = true;
                    this.value = 50;
                    this.knockBack = 2f;
                    this.ranged = true;
                    break;
                case 280:
                    this.name = "Spear";
                    this.useStyle = 5;
                    this.useAnimation = 31;
                    this.useTime = 31;
                    this.shootSpeed = 3.7f;
                    this.knockBack = 6.5f;
                    this.width = 32;
                    this.height = 32;
                    this.damage = 8;
                    this.scale = 1f;
                    this.useSound = 1;
                    this.shoot = 49;
                    this.value = 1000;
                    this.noMelee = true;
                    this.noUseGraphic = true;
                    this.melee = true;
                    break;
                case 281:
                    this.useStyle = 5;
                    this.autoReuse = true;
                    this.useAnimation = 45;
                    this.useTime = 45;
                    this.name = "Blowpipe";
                    this.width = 38;
                    this.height = 6;
                    this.shoot = 10;
                    this.useAmmo = 51;
                    this.useSound = 5;
                    this.damage = 9;
                    this.shootSpeed = 11f;
                    this.noMelee = true;
                    this.value = 10000;
                    this.knockBack = 4f;
                    this.useAmmo = 51;
                    this.toolTip = "Allows the collection of seeds for ammo";
                    this.ranged = true;
                    break;
                case 282:
                    this.useStyle = 1;
                    this.name = "Glowstick";
                    this.shootSpeed = 6f;
                    this.shoot = 50;
                    this.width = 12;
                    this.height = 12;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.useSound = 1;
                    this.useAnimation = 15;
                    this.useTime = 15;
                    this.noMelee = true;
                    this.value = 10;
                    this.holdStyle = 1;
                    this.toolTip = "Works when wet";
                    break;
                case 283:
                    this.name = "Seed";
                    this.shoot = 51;
                    this.width = 8;
                    this.height = 8;
                    this.maxStack = 250;
                    this.ammo = 51;
                    this.toolTip = "For use with Blowpipe";
                    break;
                case 284:
                    this.noMelee = true;
                    this.useStyle = 1;
                    this.name = "Wooden Boomerang";
                    this.shootSpeed = 6.5f;
                    this.shoot = 52;
                    this.damage = 7;
                    this.knockBack = 5f;
                    this.width = 14;
                    this.height = 28;
                    this.useSound = 1;
                    this.useAnimation = 16;
                    this.useTime = 16;
                    this.noUseGraphic = true;
                    this.value = 5000;
                    this.melee = true;
                    break;
                case 285:
                    this.name = "Aglet";
                    this.width = 24;
                    this.height = 8;
                    this.accessory = true;
                    this.toolTip = "5% increased movement speed";
                    this.value = 5000;
                    break;
                case 286:
                    this.useStyle = 1;
                    this.name = "Sticky Glowstick";
                    this.shootSpeed = 6f;
                    this.shoot = 53;
                    this.width = 12;
                    this.height = 12;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.useSound = 1;
                    this.useAnimation = 15;
                    this.useTime = 15;
                    this.noMelee = true;
                    this.value = 20;
                    this.holdStyle = 1;
                    break;
                case 287:
                    this.useStyle = 1;
                    this.name = "Poisoned Knife";
                    this.shootSpeed = 11f;
                    this.shoot = 54;
                    this.damage = 13;
                    this.width = 18;
                    this.height = 20;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.useSound = 1;
                    this.useAnimation = 15;
                    this.useTime = 15;
                    this.noUseGraphic = true;
                    this.noMelee = true;
                    this.value = 60;
                    this.knockBack = 2f;
                    this.ranged = true;
                    break;
                case 288:
                    this.name = "Obsidian Skin Potion";
                    this.useSound = 3;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.buffType = 1;
                    this.buffTime = 14400;
                    this.toolTip = "Provides immunity to lava";
                    this.value = 1000;
                    this.rare = 1;
                    break;
                case 289:
                    this.name = "Regeneration Potion";
                    this.useSound = 3;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.buffType = 2;
                    this.buffTime = 18000;
                    this.toolTip = "Provides life regeneration";
                    this.value = 1000;
                    this.rare = 1;
                    break;
                case 290:
                    this.name = "Swiftness Potion";
                    this.useSound = 3;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.buffType = 3;
                    this.buffTime = 14400;
                    this.toolTip = "25% increased movement speed";
                    this.value = 1000;
                    this.rare = 1;
                    break;
                case 291:
                    this.name = "Gills Potion";
                    this.useSound = 3;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.buffType = 4;
                    this.buffTime = 7200;
                    this.toolTip = "Breathe water instead of air";
                    this.value = 1000;
                    this.rare = 1;
                    break;
                case 292:
                    this.name = "Ironskin Potion";
                    this.useSound = 3;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.buffType = 5;
                    this.buffTime = 18000;
                    this.toolTip = "Increase defense by 8";
                    this.value = 1000;
                    this.rare = 1;
                    break;
                case 293:
                    this.name = "Mana Regeneration Potion";
                    this.useSound = 3;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.buffType = 6;
                    this.buffTime = 7200;
                    this.toolTip = "Increased mana regeneration";
                    this.value = 1000;
                    this.rare = 1;
                    break;
                case 294:
                    this.name = "Magic Power Potion";
                    this.useSound = 3;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.buffType = 7;
                    this.buffTime = 7200;
                    this.toolTip = "20% increased magic damage";
                    this.value = 1000;
                    this.rare = 1;
                    break;
                case 295:
                    this.name = "Featherfall Potion";
                    this.useSound = 3;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.buffType = 8;
                    this.buffTime = 18000;
                    this.toolTip = "Slows falling speed";
                    this.value = 1000;
                    this.rare = 1;
                    break;
                case 296:
                    this.name = "Spelunker Potion";
                    this.useSound = 3;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.buffType = 9;
                    this.buffTime = 18000;
                    this.toolTip = "Shows the location of treasure and ore";
                    this.value = 1000;
                    this.rare = 1;
                    break;
                case 297:
                    this.name = "Invisibility Potion";
                    this.useSound = 3;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.buffType = 10;
                    this.buffTime = 7200;
                    this.toolTip = "Grants invisibility";
                    this.value = 1000;
                    this.rare = 1;
                    break;
                case 298:
                    this.name = "Shine Potion";
                    this.useSound = 3;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.buffType = 11;
                    this.buffTime = 18000;
                    this.toolTip = "Emits an aura of light";
                    this.value = 1000;
                    this.rare = 1;
                    break;
                case 299:
                    this.name = "Night Owl Potion";
                    this.useSound = 3;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.buffType = 12;
                    this.buffTime = 14400;
                    this.toolTip = "Increases night vision";
                    this.value = 1000;
                    this.rare = 1;
                    break;
                case 300:
                    this.name = "Battle Potion";
                    this.useSound = 3;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.buffType = 13;
                    this.buffTime = 25200;
                    this.toolTip = "Increases enemy spawn rate";
                    this.value = 1000;
                    this.rare = 1;
                    break;
                case 301:
                    this.name = "Thorns Potion";
                    this.useSound = 3;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.buffType = 14;
                    this.buffTime = 7200;
                    this.toolTip = "Attackers also take damage";
                    this.value = 1000;
                    this.rare = 1;
                    break;
                case 302:
                    this.name = "Water Walking Potion";
                    this.useSound = 3;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.buffType = 15;
                    this.buffTime = 18000;
                    this.toolTip = "Allows the ability to walk on water";
                    this.value = 1000;
                    this.rare = 1;
                    break;
                case 303:
                    this.name = "Archery Potion";
                    this.useSound = 3;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.buffType = 16;
                    this.buffTime = 14400;
                    this.toolTip = "20% increased arrow speed and damage";
                    this.value = 1000;
                    this.rare = 1;
                    break;
                case 304:
                    this.name = "Hunter Potion";
                    this.useSound = 3;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.buffType = 17;
                    this.buffTime = 18000;
                    this.toolTip = "Shows the location of enemies";
                    this.value = 1000;
                    this.rare = 1;
                    break;
                case 305:
                    this.name = "Gravitation Potion";
                    this.useSound = 3;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.buffType = 18;
                    this.buffTime = 10800;
                    this.toolTip = "Allows the control of gravity";
                    this.value = 1000;
                    this.rare = 1;
                    break;
                case 306:
                    this.name = "Gold Chest";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 21;
                    this.placeStyle = 1;
                    this.width = 26;
                    this.height = 22;
                    this.value = 5000;
                    break;
                case 307:
                    this.name = "Daybloom Seeds";
                    this.useTurn = true;
                    this.useStyle = 1;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 82;
                    this.placeStyle = 0;
                    this.width = 12;
                    this.height = 14;
                    this.value = 80;
                    break;
                case 308:
                    this.name = "Moonglow Seeds";
                    this.useTurn = true;
                    this.useStyle = 1;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 82;
                    this.placeStyle = 1;
                    this.width = 12;
                    this.height = 14;
                    this.value = 80;
                    break;
                case 309:
                    this.name = "Blinkroot Seeds";
                    this.useTurn = true;
                    this.useStyle = 1;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 82;
                    this.placeStyle = 2;
                    this.width = 12;
                    this.height = 14;
                    this.value = 80;
                    break;
                case 310:
                    this.name = "Deathweed Seeds";
                    this.useTurn = true;
                    this.useStyle = 1;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 82;
                    this.placeStyle = 3;
                    this.width = 12;
                    this.height = 14;
                    this.value = 80;
                    break;
                case 311:
                    this.name = "Waterleaf Seeds";
                    this.useTurn = true;
                    this.useStyle = 1;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 82;
                    this.placeStyle = 4;
                    this.width = 12;
                    this.height = 14;
                    this.value = 80;
                    break;
                case 312:
                    this.name = "Fireblossom Seeds";
                    this.useTurn = true;
                    this.useStyle = 1;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 82;
                    this.placeStyle = 5;
                    this.width = 12;
                    this.height = 14;
                    this.value = 80;
                    break;
                case 313:
                    this.name = "Daybloom";
                    this.maxStack = 99;
                    this.width = 12;
                    this.height = 14;
                    this.value = 100;
                    break;
                case 314:
                    this.name = "Moonglow";
                    this.maxStack = 99;
                    this.width = 12;
                    this.height = 14;
                    this.value = 100;
                    break;
                case 315:
                    this.name = "Blinkroot";
                    this.maxStack = 99;
                    this.width = 12;
                    this.height = 14;
                    this.value = 100;
                    break;
                case 316:
                    this.name = "Deathweed";
                    this.maxStack = 99;
                    this.width = 12;
                    this.height = 14;
                    this.value = 100;
                    break;
                case 317:
                    this.name = "Waterleaf";
                    this.maxStack = 99;
                    this.width = 12;
                    this.height = 14;
                    this.value = 100;
                    break;
                case 318:
                    this.name = "Fireblossom";
                    this.maxStack = 99;
                    this.width = 12;
                    this.height = 14;
                    this.value = 100;
                    break;
                case 319:
                    this.name = "Shark Fin";
                    this.maxStack = 99;
                    this.width = 16;
                    this.height = 14;
                    this.value = 200;
                    break;
                case 320:
                    this.name = "Feather";
                    this.maxStack = 99;
                    this.width = 16;
                    this.height = 14;
                    this.value = 50;
                    break;
                case 321:
                    this.name = "Tombstone";
                    this.useTurn = true;
                    this.useStyle = 1;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 85;
                    this.width = 20;
                    this.height = 20;
                    break;
                case 322:
                    this.name = "Mime Mask";
                    this.headSlot = 28;
                    this.width = 20;
                    this.height = 20;
                    this.value = 20000;
                    break;
                case 323:
                    this.name = "Antlion Mandible";
                    this.width = 10;
                    this.height = 20;
                    this.maxStack = 99;
                    this.value = 50;
                    break;
                case 324:
                    this.name = "Illegal Gun Parts";
                    this.width = 10;
                    this.height = 20;
                    this.maxStack = 99;
                    this.value = 750000;
                    this.toolTip = "'Banned in most places'";
                    break;
                case 325:
                    this.name = "The Doctor's Shirt";
                    this.width = 18;
                    this.height = 18;
                    this.bodySlot = 16;
                    this.value = 200000;
                    this.vanity = true;
                    break;
                case 326:
                    this.name = "The Doctor's Pants";
                    this.width = 18;
                    this.height = 18;
                    this.legSlot = 15;
                    this.value = 200000;
                    this.vanity = true;
                    break;
                case 327:
                    this.name = "Golden Key";
                    this.width = 14;
                    this.height = 20;
                    this.maxStack = 99;
                    this.toolTip = "Opens one Gold Chest";
                    break;
                case 328:
                    this.name = "Shadow Chest";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 21;
                    this.placeStyle = 3;
                    this.width = 26;
                    this.height = 22;
                    this.value = 5000;
                    break;
                case 329:
                    this.name = "Shadow Key";
                    this.width = 14;
                    this.height = 20;
                    this.maxStack = 1;
                    this.toolTip = "Opens all Shadow Chests";
                    this.value = 75000;
                    break;
                case 330:
                    this.name = "Obsidian Brick Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 7;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 20;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 331:
                    this.name = "Jungle Spores";
                    this.width = 18;
                    this.height = 16;
                    this.maxStack = 99;
                    this.value = 100;
                    break;
                case 332:
                    this.name = "Loom";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 86;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.toolTip = "Used for crafting cloth";
                    break;
                case 333:
                    this.name = "Piano";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 87;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    break;
                case 334:
                    this.name = "Dresser";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 88;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    break;
                case 335:
                    this.name = "Bench";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 89;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    break;
                case 336:
                    this.name = "Bathtub";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 90;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    break;
                case 337:
                    this.name = "Red Banner";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 91;
                    this.placeStyle = 0;
                    this.width = 10;
                    this.height = 24;
                    this.value = 500;
                    break;
                case 338:
                    this.name = "Green Banner";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 91;
                    this.placeStyle = 1;
                    this.width = 10;
                    this.height = 24;
                    this.value = 500;
                    break;
                case 339:
                    this.name = "Blue Banner";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 91;
                    this.placeStyle = 2;
                    this.width = 10;
                    this.height = 24;
                    this.value = 500;
                    break;
                case 340:
                    this.name = "Yellow Banner";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 91;
                    this.placeStyle = 3;
                    this.width = 10;
                    this.height = 24;
                    this.value = 500;
                    break;
                case 341:
                    this.name = "Lamp Post";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 92;
                    this.width = 10;
                    this.height = 24;
                    this.value = 500;
                    break;
                case 342:
                    this.name = "Tiki Torch";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 93;
                    this.width = 10;
                    this.height = 24;
                    this.value = 500;
                    break;
                case 343:
                    this.name = "Barrel";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 21;
                    this.placeStyle = 5;
                    this.width = 20;
                    this.height = 20;
                    this.value = 500;
                    break;
                case 344:
                    this.name = "Chinese Lantern";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 95;
                    this.width = 20;
                    this.height = 20;
                    this.value = 500;
                    break;
                case 345:
                    this.name = "Cooking Pot";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 96;
                    this.width = 20;
                    this.height = 20;
                    this.value = 500;
                    break;
                case 346:
                    this.name = "Safe";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 97;
                    this.width = 20;
                    this.height = 20;
                    this.value = 500000;
                    break;
                case 347:
                    this.name = "Skull Lantern";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 98;
                    this.width = 20;
                    this.height = 20;
                    this.value = 500;
                    break;
                case 348:
                    this.name = "Trash Can";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 21;
                    this.placeStyle = 6;
                    this.width = 20;
                    this.height = 20;
                    this.value = 1000;
                    break;
                case 349:
                    this.name = "Candelabra";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 100;
                    this.width = 20;
                    this.height = 20;
                    this.value = 1500;
                    break;
                case 350:
                    this.name = "Pink Vase";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 13;
                    this.placeStyle = 3;
                    this.width = 16;
                    this.height = 24;
                    this.value = 70;
                    break;
                case 351:
                    this.name = "Mug";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 13;
                    this.placeStyle = 4;
                    this.width = 16;
                    this.height = 24;
                    this.value = 20;
                    break;
                case 352:
                    this.name = "Keg";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 94;
                    this.width = 24;
                    this.height = 24;
                    this.value = 600;
                    this.toolTip = "Used for brewing ale";
                    break;
                case 353:
                    this.name = "Ale";
                    this.useSound = 3;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 10;
                    this.height = 10;
                    this.buffType = 25;
                    this.buffTime = 7200;
                    this.value = 100;
                    break;
                case 354:
                    this.name = "Bookcase";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 101;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    break;
                case 355:
                    this.name = "Throne";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 102;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    break;
                case 356:
                    this.name = "Bowl";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 103;
                    this.width = 16;
                    this.height = 24;
                    this.value = 20;
                    break;
                case 357:
                    this.name = "Bowl of Soup";
                    this.useSound = 3;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 10;
                    this.height = 10;
                    this.buffType = 26;
                    this.buffTime = 36000;
                    this.rare = 1;
                    this.toolTip = "Minor improvements to all stats";
                    this.value = 1000;
                    break;
                case 358:
                    this.name = "Toilet";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 15;
                    this.placeStyle = 1;
                    this.width = 12;
                    this.height = 30;
                    this.value = 150;
                    break;
                case 359:
                    this.name = "Grandfather Clock";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 104;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    break;
                case 360:
                    this.name = "Armor Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    break;
                case 361:
                    this.useStyle = 4;
                    this.consumable = true;
                    this.useAnimation = 45;
                    this.useTime = 45;
                    this.name = "Goblin Battle Standard";
                    this.width = 28;
                    this.height = 28;
                    this.toolTip = "Summons a Goblin Army";
                    break;
                case 362:
                    this.name = "Tattered Cloth";
                    this.maxStack = 99;
                    this.width = 24;
                    this.height = 24;
                    this.value = 30;
                    break;
                case 363:
                    this.name = "Sawmill";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 106;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.toolTip = "Used for advanced wood crafting";
                    break;
                case 364:
                    this.name = "Cobalt Ore";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 107;
                    this.width = 12;
                    this.height = 12;
                    this.value = 3500;
                    this.rare = 3;
                    break;
                case 365:
                    this.name = "Mythril Ore";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 108;
                    this.width = 12;
                    this.height = 12;
                    this.value = 5500;
                    this.rare = 3;
                    break;
                case 366:
                    this.name = "Adamantite Ore";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 111;
                    this.width = 12;
                    this.height = 12;
                    this.value = 7500;
                    this.rare = 3;
                    break;
                case 367:
                    this.name = "Pwnhammer";
                    this.useTurn = true;
                    this.autoReuse = true;
                    this.useStyle = 1;
                    this.useAnimation = 27;
                    this.useTime = 14;
                    this.hammer = 80;
                    this.width = 24;
                    this.height = 28;
                    this.damage = 26;
                    this.knockBack = 7.5f;
                    this.scale = 1.2f;
                    this.useSound = 1;
                    this.rare = 4;
                    this.value = 39000;
                    this.melee = true;
                    this.toolTip = "Strong enough to destroy Demon Altars";
                    break;
                case 368:
                    this.autoReuse = true;
                    this.name = "Excalibur";
                    this.useStyle = 1;
                    this.useAnimation = 25;
                    this.useTime = 25;
                    this.knockBack = 4.5f;
                    this.width = 40;
                    this.height = 40;
                    this.damage = 47;
                    this.scale = 1.15f;
                    this.useSound = 1;
                    this.rare = 5;
                    this.value = 230000;
                    this.melee = true;
                    break;
                case 369:
                    this.name = "Hallowed Seeds";
                    this.useTurn = true;
                    this.useStyle = 1;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 109;
                    this.width = 14;
                    this.height = 14;
                    this.value = 2000;
                    this.rare = 3;
                    break;
                case 370:
                    this.name = "Ebonsand Block";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 112;
                    this.width = 12;
                    this.height = 12;
                    this.ammo = 42;
                    break;
                case 371:
                    this.name = "Cobalt Hat";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 2;
                    this.headSlot = 29;
                    this.rare = 4;
                    this.value = 75000;
                    this.toolTip = "Increases maximum mana by 40";
                    this.toolTip2 = "9% increased magic critical strike chance";
                    break;
                case 372:
                    this.name = "Cobalt Helmet";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 11;
                    this.headSlot = 30;
                    this.rare = 4;
                    this.value = 75000;
                    this.toolTip = "7% increased movement speed";
                    this.toolTip2 = "12% increased melee speed";
                    break;
                case 373:
                    this.name = "Cobalt Mask";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 4;
                    this.headSlot = 31;
                    this.rare = 4;
                    this.value = 75000;
                    this.toolTip = "10% increased ranged damage";
                    this.toolTip2 = "6% increased ranged critical strike chance";
                    break;
                case 374:
                    this.name = "Cobalt Breastplate";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 8;
                    this.bodySlot = 17;
                    this.rare = 4;
                    this.value = 60000;
                    this.toolTip2 = "3% increased critical strike chance";
                    break;
                case 375:
                    this.name = "Cobalt Leggings";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 7;
                    this.legSlot = 16;
                    this.rare = 4;
                    this.value = 45000;
                    this.toolTip2 = "10% increased movement speed";
                    break;
                case 376:
                    this.name = "Mythril Hood";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 3;
                    this.headSlot = 32;
                    this.rare = 4;
                    this.value = 112500;
                    this.toolTip = "Increases maximum mana by 60";
                    this.toolTip2 = "15% increased magic damage";
                    break;
                case 377:
                    this.name = "Mythril Helmet";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 16;
                    this.headSlot = 33;
                    this.rare = 4;
                    this.value = 112500;
                    this.toolTip = "5% increased melee critical strike chance";
                    this.toolTip2 = "10% increased melee damage";
                    break;
                case 378:
                    this.name = "Mythril Hat";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 6;
                    this.headSlot = 34;
                    this.rare = 4;
                    this.value = 112500;
                    this.toolTip = "12% increased ranged damage";
                    this.toolTip2 = "7% increased ranged critical strike chance";
                    break;
                case 379:
                    this.name = "Mythril Chainmail";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 12;
                    this.bodySlot = 18;
                    this.rare = 4;
                    this.value = 90000;
                    this.toolTip2 = "5% increased damage";
                    break;
                case 380:
                    this.name = "Mythril Greaves";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 9;
                    this.legSlot = 17;
                    this.rare = 4;
                    this.value = 67500;
                    this.toolTip2 = "3% increased critical strike chance";
                    break;
                case 381:
                    this.name = "Cobalt Bar";
                    this.width = 20;
                    this.height = 20;
                    this.maxStack = 99;
                    this.value = 10500;
                    this.rare = 3;
                    break;
                case 382:
                    this.name = "Mythril Bar";
                    this.width = 20;
                    this.height = 20;
                    this.maxStack = 99;
                    this.value = 22000;
                    this.rare = 3;
                    break;
                case 383:
                    this.name = "Cobalt Chainsaw";
                    this.useStyle = 5;
                    this.useAnimation = 25;
                    this.useTime = 8;
                    this.shootSpeed = 40f;
                    this.knockBack = 2.75f;
                    this.width = 20;
                    this.height = 12;
                    this.damage = 23;
                    this.axe = 14;
                    this.useSound = 23;
                    this.shoot = 57;
                    this.rare = 4;
                    this.value = 54000;
                    this.noMelee = true;
                    this.noUseGraphic = true;
                    this.melee = true;
                    this.channel = true;
                    break;
                case 384:
                    this.name = "Mythril Chainsaw";
                    this.useStyle = 5;
                    this.useAnimation = 25;
                    this.useTime = 8;
                    this.shootSpeed = 40f;
                    this.knockBack = 3f;
                    this.width = 20;
                    this.height = 12;
                    this.damage = 29;
                    this.axe = 17;
                    this.useSound = 23;
                    this.shoot = 58;
                    this.rare = 4;
                    this.value = 81000;
                    this.noMelee = true;
                    this.noUseGraphic = true;
                    this.melee = true;
                    this.channel = true;
                    break;
                case 385:
                    this.name = "Cobalt Drill";
                    this.useStyle = 5;
                    this.useAnimation = 25;
                    this.useTime = 13;
                    this.shootSpeed = 32f;
                    this.knockBack = 0f;
                    this.width = 20;
                    this.height = 12;
                    this.damage = 10;
                    this.pick = 110;
                    this.useSound = 23;
                    this.shoot = 59;
                    this.rare = 4;
                    this.value = 54000;
                    this.noMelee = true;
                    this.noUseGraphic = true;
                    this.melee = true;
                    this.channel = true;
                    this.toolTip = "Can mine Mythril";
                    break;
                case 386:
                    this.name = "Mythril Drill";
                    this.useStyle = 5;
                    this.useAnimation = 25;
                    this.useTime = 10;
                    this.shootSpeed = 32f;
                    this.knockBack = 0f;
                    this.width = 20;
                    this.height = 12;
                    this.damage = 15;
                    this.pick = 150;
                    this.useSound = 23;
                    this.shoot = 60;
                    this.rare = 4;
                    this.value = 81000;
                    this.noMelee = true;
                    this.noUseGraphic = true;
                    this.melee = true;
                    this.channel = true;
                    this.toolTip = "Can mine Adamantite";
                    break;
                case 387:
                    this.name = "Adamantite Chainsaw";
                    this.useStyle = 5;
                    this.useAnimation = 25;
                    this.useTime = 6;
                    this.shootSpeed = 40f;
                    this.knockBack = 4.5f;
                    this.width = 20;
                    this.height = 12;
                    this.damage = 33;
                    this.axe = 20;
                    this.useSound = 23;
                    this.shoot = 61;
                    this.rare = 4;
                    this.value = 108000;
                    this.noMelee = true;
                    this.noUseGraphic = true;
                    this.melee = true;
                    this.channel = true;
                    break;
                case 388:
                    this.name = "Adamantite Drill";
                    this.useStyle = 5;
                    this.useAnimation = 25;
                    this.useTime = 7;
                    this.shootSpeed = 32f;
                    this.knockBack = 0f;
                    this.width = 20;
                    this.height = 12;
                    this.damage = 20;
                    this.pick = 180;
                    this.useSound = 23;
                    this.shoot = 62;
                    this.rare = 4;
                    this.value = 108000;
                    this.noMelee = true;
                    this.noUseGraphic = true;
                    this.melee = true;
                    this.channel = true;
                    break;
                case 389:
                    this.name = "Dao of Pow";
                    this.noMelee = true;
                    this.useStyle = 5;
                    this.useAnimation = 45;
                    this.useTime = 45;
                    this.knockBack = 7f;
                    this.width = 30;
                    this.height = 10;
                    this.damage = 49;
                    this.scale = 1.1f;
                    this.noUseGraphic = true;
                    this.shoot = 63;
                    this.shootSpeed = 15f;
                    this.useSound = 1;
                    this.rare = 5;
                    this.value = 144000;
                    this.melee = true;
                    this.channel = true;
                    this.toolTip = "Has a chance to confuse";
                    this.toolTip2 = "'Find your inner pieces'";
                    break;
                case 390:
                    this.name = "Mythril Halberd";
                    this.useStyle = 5;
                    this.useAnimation = 26;
                    this.useTime = 26;
                    this.shootSpeed = 4.5f;
                    this.knockBack = 5f;
                    this.width = 40;
                    this.height = 40;
                    this.damage = 35;
                    this.scale = 1.1f;
                    this.useSound = 1;
                    this.shoot = 64;
                    this.rare = 4;
                    this.value = 67500;
                    this.noMelee = true;
                    this.noUseGraphic = true;
                    this.melee = true;
                    break;
                case 391:
                    this.name = "Adamantite Bar";
                    this.width = 20;
                    this.height = 20;
                    this.maxStack = 99;
                    this.value = 37500;
                    this.rare = 3;
                    break;
                case 392:
                    this.name = "Glass Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 7;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 21;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 393:
                    this.name = "Compass";
                    this.width = 24;
                    this.height = 28;
                    this.rare = 3;
                    this.value = 100000;
                    this.accessory = true;
                    this.toolTip = "Shows horizontal position";
                    break;
                case 394:
                    this.name = "Diving Gear";
                    this.width = 24;
                    this.height = 28;
                    this.rare = 4;
                    this.value = 100000;
                    this.accessory = true;
                    this.toolTip = "Grants the ability to swim";
                    this.toolTip2 = "Greatly extends underwater breathing";
                    break;
                case 395:
                    this.name = "GPS";
                    this.width = 24;
                    this.height = 28;
                    this.rare = 4;
                    this.value = 150000;
                    this.accessory = true;
                    this.toolTip = "Shows position";
                    this.toolTip2 = "Tells the time";
                    break;
                case 396:
                    this.name = "Obsidian Horseshoe";
                    this.width = 24;
                    this.height = 28;
                    this.rare = 4;
                    this.value = 100000;
                    this.accessory = true;
                    this.toolTip = "Negates fall damage";
                    this.toolTip2 = "Grants immunity to fire blocks";
                    break;
                case 397:
                    this.name = "Obsidian Shield";
                    this.width = 24;
                    this.height = 28;
                    this.rare = 4;
                    this.value = 100000;
                    this.accessory = true;
                    this.defense = 2;
                    this.toolTip = "Grants immunity to knockback";
                    this.toolTip2 = "Grants immunity to fire blocks";
                    break;
                case 398:
                    this.name = "Tinkerer's Workshop";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 114;
                    this.width = 26;
                    this.height = 20;
                    this.value = 100000;
                    this.toolTip = "Allows the combining of some accessories";
                    break;
                case 399:
                    this.name = "Cloud in a Balloon";
                    this.width = 14;
                    this.height = 28;
                    this.rare = 4;
                    this.value = 150000;
                    this.accessory = true;
                    this.toolTip = "Allows the holder to double jump";
                    this.toolTip2 = "Increases jump height";
                    break;
                case 400:
                    this.name = "Adamantite Headgear";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 4;
                    this.headSlot = 35;
                    this.rare = 4;
                    this.value = 150000;
                    this.toolTip = "Increases maximum mana by 80";
                    this.toolTip2 = "11% increased magic damage and critical strike chance";
                    break;
                case 401:
                    this.name = "Adamantite Helmet";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 22;
                    this.headSlot = 36;
                    this.rare = 4;
                    this.value = 150000;
                    this.toolTip = "7% increased melee critical strike chance";
                    this.toolTip2 = "14% increased melee damage";
                    break;
                case 402:
                    this.name = "Adamantite Mask";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 8;
                    this.headSlot = 37;
                    this.rare = 4;
                    this.value = 150000;
                    this.toolTip = "14% increased ranged damage";
                    this.toolTip2 = "8% increased ranged critical strike chance";
                    break;
                case 403:
                    this.name = "Adamantite Breastplate";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 14;
                    this.bodySlot = 19;
                    this.rare = 4;
                    this.value = 120000;
                    this.toolTip = "6% increased damage";
                    break;
                case 404:
                    this.name = "Adamantite Leggings";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 10;
                    this.legSlot = 18;
                    this.rare = 4;
                    this.value = 90000;
                    this.toolTip = "4% increased critical strike chance";
                    this.toolTip2 = "5% increased movement speed";
                    break;
                case 405:
                    this.name = "Spectre Boots";
                    this.width = 28;
                    this.height = 24;
                    this.accessory = true;
                    this.rare = 4;
                    this.toolTip = "Allows flight";
                    this.toolTip2 = "The wearer can run super fast";
                    this.value = 100000;
                    break;
                case 406:
                    this.name = "Adamantite Glaive";
                    this.useStyle = 5;
                    this.useAnimation = 25;
                    this.useTime = 25;
                    this.shootSpeed = 5f;
                    this.knockBack = 6f;
                    this.width = 40;
                    this.height = 40;
                    this.damage = 38;
                    this.scale = 1.1f;
                    this.useSound = 1;
                    this.shoot = 66;
                    this.rare = 4;
                    this.value = 90000;
                    this.noMelee = true;
                    this.noUseGraphic = true;
                    this.melee = true;
                    break;
                case 407:
                    this.name = "Toolbelt";
                    this.width = 28;
                    this.height = 24;
                    this.accessory = true;
                    this.rare = 3;
                    this.toolTip = "Increases block placement range";
                    this.value = 100000;
                    break;
                case 408:
                    this.name = "Pearlsand Block";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 116;
                    this.width = 12;
                    this.height = 12;
                    this.ammo = 42;
                    break;
                case 409:
                    this.name = "Pearlstone Block";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 117;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 410:
                    this.name = "Mining Shirt";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 1;
                    this.bodySlot = 20;
                    this.value = 5000;
                    this.rare = 1;
                    break;
                case 411:
                    this.name = "Mining Pants";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 1;
                    this.legSlot = 19;
                    this.value = 5000;
                    this.rare = 1;
                    break;
                case 412:
                    this.name = "Pearlstone Brick";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 118;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 413:
                    this.name = "Iridescent Brick";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 119;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 414:
                    this.name = "Mudstone Block";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 120;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 415:
                    this.name = "Cobalt Brick";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 121;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 416:
                    this.name = "Mythril Brick";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 122;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 417:
                    this.name = "Pearlstone Brick Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 7;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 22;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 418:
                    this.name = "Iridescent Brick Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 7;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 23;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 419:
                    this.name = "Mudstone Brick Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 7;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 24;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 420:
                    this.name = "Cobalt Brick Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 7;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 25;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 421:
                    this.name = "Mythril Brick Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 7;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 26;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 422:
                    this.useStyle = 1;
                    this.name = "Holy Water";
                    this.shootSpeed = 9f;
                    this.rare = 3;
                    this.damage = 20;
                    this.shoot = 69;
                    this.width = 18;
                    this.height = 20;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.knockBack = 3f;
                    this.useSound = 1;
                    this.useAnimation = 15;
                    this.useTime = 15;
                    this.noUseGraphic = true;
                    this.noMelee = true;
                    this.value = 200;
                    this.toolTip = "Spreads the Hallow to some blocks";
                    break;
                case 423:
                    this.useStyle = 1;
                    this.name = "Unholy Water";
                    this.shootSpeed = 9f;
                    this.rare = 3;
                    this.damage = 20;
                    this.shoot = 70;
                    this.width = 18;
                    this.height = 20;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.knockBack = 3f;
                    this.useSound = 1;
                    this.useAnimation = 15;
                    this.useTime = 15;
                    this.noUseGraphic = true;
                    this.noMelee = true;
                    this.value = 200;
                    this.toolTip = "Spreads the corruption to some blocks";
                    break;
                case 424:
                    this.name = "Silt Block";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 123;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 425:
                    this.mana = 40;
                    this.channel = true;
                    this.damage = 0;
                    this.useStyle = 1;
                    this.name = "Fairy Bell";
                    this.shoot = 72;
                    this.width = 24;
                    this.height = 24;
                    this.useSound = 25;
                    this.useAnimation = 20;
                    this.useTime = 20;
                    this.rare = 5;
                    this.noMelee = true;
                    this.toolTip = "Summons a magical fairy";
                    this.value = (this.value = 250000);
                    this.buffType = 27;
                    this.buffTime = 18000;
                    break;
                case 426:
                    this.name = "Breaker Blade";
                    this.useStyle = 1;
                    this.useAnimation = 30;
                    this.knockBack = 8f;
                    this.width = 60;
                    this.height = 70;
                    this.damage = 39;
                    this.scale = 1.05f;
                    this.useSound = 1;
                    this.rare = 4;
                    this.value = 150000;
                    this.melee = true;
                    break;
                case 427:
                    this.noWet = true;
                    this.name = "Blue Torch";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.holdStyle = 1;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 4;
                    this.placeStyle = 1;
                    this.width = 10;
                    this.height = 12;
                    this.value = 200;
                    break;
                case 428:
                    this.noWet = true;
                    this.name = "Red Torch";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.holdStyle = 1;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 4;
                    this.placeStyle = 2;
                    this.width = 10;
                    this.height = 12;
                    this.value = 200;
                    break;
                case 429:
                    this.noWet = true;
                    this.name = "Green Torch";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.holdStyle = 1;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 4;
                    this.placeStyle = 3;
                    this.width = 10;
                    this.height = 12;
                    this.value = 200;
                    break;
                case 430:
                    this.noWet = true;
                    this.name = "Purple Torch";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.holdStyle = 1;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 4;
                    this.placeStyle = 4;
                    this.width = 10;
                    this.height = 12;
                    this.value = 200;
                    break;
                case 431:
                    this.noWet = true;
                    this.name = "White Torch";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.holdStyle = 1;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 4;
                    this.placeStyle = 5;
                    this.width = 10;
                    this.height = 12;
                    this.value = 500;
                    break;
                case 432:
                    this.noWet = true;
                    this.name = "Yellow Torch";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.holdStyle = 1;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 4;
                    this.placeStyle = 6;
                    this.width = 10;
                    this.height = 12;
                    this.value = 200;
                    break;
                case 433:
                    this.noWet = true;
                    this.name = "Demon Torch";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.holdStyle = 1;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 4;
                    this.placeStyle = 7;
                    this.width = 10;
                    this.height = 12;
                    this.value = 300;
                    break;
                case 434:
                    this.autoReuse = true;
                    this.useStyle = 5;
                    this.useAnimation = 12;
                    this.useTime = 4;
                    this.reuseDelay = 14;
                    this.name = "Clockwork Assault Rifle";
                    this.width = 50;
                    this.height = 18;
                    this.shoot = 10;
                    this.useAmmo = 14;
                    this.useSound = 31;
                    this.damage = 19;
                    this.shootSpeed = 7.75f;
                    this.noMelee = true;
                    this.value = 150000;
                    this.rare = 4;
                    this.ranged = true;
                    this.toolTip = "Three round burst";
                    this.toolTip2 = "Only the first shot consumes ammo";
                    break;
                case 435:
                    this.useStyle = 5;
                    this.autoReuse = true;
                    this.useAnimation = 25;
                    this.useTime = 25;
                    this.name = "Cobalt Repeater";
                    this.width = 50;
                    this.height = 18;
                    this.shoot = 1;
                    this.useAmmo = 1;
                    this.useSound = 5;
                    this.damage = 30;
                    this.shootSpeed = 9f;
                    this.noMelee = true;
                    this.value = 60000;
                    this.ranged = true;
                    this.rare = 4;
                    this.knockBack = 1.5f;
                    break;
                case 436:
                    this.useStyle = 5;
                    this.autoReuse = true;
                    this.useAnimation = 23;
                    this.useTime = 23;
                    this.name = "Mythril Repeater";
                    this.width = 50;
                    this.height = 18;
                    this.shoot = 1;
                    this.useAmmo = 1;
                    this.useSound = 5;
                    this.damage = 34;
                    this.shootSpeed = 9.5f;
                    this.noMelee = true;
                    this.value = 90000;
                    this.ranged = true;
                    this.rare = 4;
                    this.knockBack = 2f;
                    break;
                case 437:
                    this.noUseGraphic = true;
                    this.damage = 0;
                    this.knockBack = 7f;
                    this.useStyle = 5;
                    this.name = "Dual Hook";
                    this.shootSpeed = 14f;
                    this.shoot = 73;
                    this.width = 18;
                    this.height = 28;
                    this.useSound = 1;
                    this.useAnimation = 20;
                    this.useTime = 20;
                    this.rare = 4;
                    this.noMelee = true;
                    this.value = 200000;
                    break;
                case 438:
                    this.name = "Star Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 2;
                    break;
                case 439:
                    this.name = "Sword Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 3;
                    break;
                case 440:
                    this.name = "Slime Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 4;
                    break;
                case 441:
                    this.name = "Goblin Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 5;
                    break;
                case 442:
                    this.name = "Shield Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 6;
                    break;
                case 443:
                    this.name = "Bat Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 7;
                    break;
                case 444:
                    this.name = "Fish Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 8;
                    break;
                case 445:
                    this.name = "Bunny Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 9;
                    break;
                case 446:
                    this.name = "Skeleton Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 10;
                    break;
                case 447:
                    this.name = "Reaper Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 11;
                    break;
                case 448:
                    this.name = "Woman Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 12;
                    break;
                case 449:
                    this.name = "Imp Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 13;
                    break;
                case 450:
                    this.name = "Gargoyle Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 14;
                    break;
                case 451:
                    this.name = "Gloom Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 15;
                    break;
                case 452:
                    this.name = "Hornet Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 16;
                    break;
                case 453:
                    this.name = "Bomb Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 17;
                    break;
                case 454:
                    this.name = "Crab Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 18;
                    break;
                case 455:
                    this.name = "Hammer Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 19;
                    break;
                case 456:
                    this.name = "Potion Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 20;
                    break;
                case 457:
                    this.name = "Spear Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 21;
                    break;
                case 458:
                    this.name = "Cross Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 22;
                    break;
                case 459:
                    this.name = "Jellyfish Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 23;
                    break;
                case 460:
                    this.name = "Bow Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 24;
                    break;
                case 461:
                    this.name = "Boomerang Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 25;
                    break;
                case 462:
                    this.name = "Boot Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 26;
                    break;
                case 463:
                    this.name = "Chest Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 27;
                    break;
                case 464:
                    this.name = "Bird Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 28;
                    break;
                case 465:
                    this.name = "Axe Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 29;
                    break;
                case 466:
                    this.name = "Corrupt Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 30;
                    break;
                case 467:
                    this.name = "Tree Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 31;
                    break;
                case 468:
                    this.name = "Anvil Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 32;
                    break;
                case 469:
                    this.name = "Pickaxe Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 33;
                    break;
                case 470:
                    this.name = "Mushroom Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 34;
                    break;
                case 471:
                    this.name = "Eyeball Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 35;
                    break;
                case 472:
                    this.name = "Pillar Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 36;
                    break;
                case 473:
                    this.name = "Heart Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 37;
                    break;
                case 474:
                    this.name = "Pot Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 38;
                    break;
                case 475:
                    this.name = "Sunflower Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 39;
                    break;
                case 476:
                    this.name = "King Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 40;
                    break;
                case 477:
                    this.name = "Queen Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 41;
                    break;
                case 478:
                    this.name = "Pirahna Statue";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 105;
                    this.width = 20;
                    this.height = 20;
                    this.value = 300;
                    this.placeStyle = 42;
                    break;
                case 479:
                    this.name = "Planked Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 7;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 27;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 480:
                    this.name = "Wooden Beam";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 124;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 481:
                    this.useStyle = 5;
                    this.autoReuse = true;
                    this.useAnimation = 20;
                    this.useTime = 20;
                    this.name = "Adamantite Repeater";
                    this.width = 50;
                    this.height = 18;
                    this.shoot = 1;
                    this.useAmmo = 1;
                    this.useSound = 5;
                    this.damage = 37;
                    this.shootSpeed = 10f;
                    this.noMelee = true;
                    this.value = 120000;
                    this.ranged = true;
                    this.rare = 4;
                    this.knockBack = 2.5f;
                    break;
                case 482:
                    this.name = "Adamantite Sword";
                    this.useStyle = 1;
                    this.useAnimation = 27;
                    this.useTime = 27;
                    this.knockBack = 6f;
                    this.width = 40;
                    this.height = 40;
                    this.damage = 44;
                    this.scale = 1.2f;
                    this.useSound = 1;
                    this.rare = 4;
                    this.value = 138000;
                    this.melee = true;
                    break;
                case 483:
                    this.useTurn = true;
                    this.autoReuse = true;
                    this.name = "Cobalt Sword";
                    this.useStyle = 1;
                    this.useAnimation = 23;
                    this.useTime = 23;
                    this.knockBack = 3.85f;
                    this.width = 40;
                    this.height = 40;
                    this.damage = 34;
                    this.scale = 1.1f;
                    this.useSound = 1;
                    this.rare = 4;
                    this.value = 69000;
                    this.melee = true;
                    break;
                case 484:
                    this.name = "Mythril Sword";
                    this.useStyle = 1;
                    this.useAnimation = 26;
                    this.useTime = 26;
                    this.knockBack = 6f;
                    this.width = 40;
                    this.height = 40;
                    this.damage = 39;
                    this.scale = 1.15f;
                    this.useSound = 1;
                    this.rare = 4;
                    this.value = 103500;
                    this.melee = true;
                    break;
                case 485:
                    this.rare = 4;
                    this.name = "Moon Charm";
                    this.width = 24;
                    this.height = 28;
                    this.accessory = true;
                    this.toolTip = "Turns the holder into a werewolf on full moons";
                    this.value = 150000;
                    break;
                case 486:
                    this.name = "Ruler";
                    this.width = 10;
                    this.height = 26;
                    this.accessory = true;
                    this.toolTip = "Creates a grid on screen for block placement";
                    this.value = 10000;
                    this.rare = 1;
                    break;
                case 487:
                    this.name = "Crystal Ball";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 125;
                    this.width = 22;
                    this.height = 22;
                    this.value = 100000;
                    this.rare = 3;
                    break;
                case 488:
                    this.name = "Disco Ball";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 126;
                    this.width = 22;
                    this.height = 26;
                    this.value = 10000;
                    break;
                case 489:
                    this.name = "Sorcerer Emblem";
                    this.width = 24;
                    this.height = 24;
                    this.accessory = true;
                    this.toolTip = "15% increased magic damage";
                    this.value = 100000;
                    this.rare = 4;
                    break;
                case 491:
                    this.name = "Ranger Emblem";
                    this.width = 24;
                    this.height = 24;
                    this.accessory = true;
                    this.toolTip = "15% increased ranged damage";
                    this.value = 100000;
                    break;
                case 490:
                    this.name = "Warrior Emblem";
                    this.width = 24;
                    this.height = 24;
                    this.accessory = true;
                    this.toolTip = "15% increased melee damage";
                    this.value = 100000;
                    this.rare = 4;
                    break;
                case 492:
                    this.name = "Demon Wings";
                    this.width = 24;
                    this.height = 8;
                    this.accessory = true;
                    this.toolTip = "Allows flight and slow fall";
                    this.value = 400000;
                    this.rare = 5;
                    break;
                case 493:
                    this.name = "Angel Wings";
                    this.width = 24;
                    this.height = 8;
                    this.accessory = true;
                    this.toolTip = "Allows flight and slow fall";
                    this.value = 400000;
                    this.rare = 5;
                    break;
                case 494:
                    this.rare = 5;
                    this.useStyle = 5;
                    this.useAnimation = 12;
                    this.useTime = 12;
                    this.name = "Magical Harp";
                    this.width = 12;
                    this.height = 28;
                    this.shoot = 76;
                    this.holdStyle = 3;
                    this.autoReuse = true;
                    this.damage = 30;
                    this.shootSpeed = 4.5f;
                    this.noMelee = true;
                    this.value = 200000;
                    this.mana = 4;
                    this.magic = true;
                    break;
                case 495:
                    this.rare = 5;
                    this.mana = 10;
                    this.channel = true;
                    this.damage = 53;
                    this.useStyle = 1;
                    this.name = "Rainbow Rod";
                    this.shootSpeed = 6f;
                    this.shoot = 79;
                    this.width = 26;
                    this.height = 28;
                    this.useSound = 28;
                    this.useAnimation = 15;
                    this.useTime = 15;
                    this.noMelee = true;
                    this.knockBack = 5f;
                    this.toolTip = "Casts a controllable rainbow";
                    this.value = 200000;
                    this.magic = true;
                    break;
                case 496:
                    this.rare = 4;
                    this.mana = 7;
                    this.damage = 26;
                    this.useStyle = 1;
                    this.name = "Ice Rod";
                    this.shootSpeed = 12f;
                    this.shoot = 80;
                    this.width = 26;
                    this.height = 28;
                    this.useSound = 28;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.rare = 4;
                    this.autoReuse = true;
                    this.noMelee = true;
                    this.knockBack = 0f;
                    this.toolTip = "Summons a block of ice";
                    this.value = 1000000;
                    this.magic = true;
                    this.knockBack = 2f;
                    break;
                case 497:
                    this.name = "Neptune's Shell";
                    this.width = 24;
                    this.height = 28;
                    this.accessory = true;
                    this.toolTip = "Transforms the holder into merfolk when entering water";
                    this.value = 150000;
                    this.rare = 5;
                    break;
                case 498:
                    this.name = "Mannequin";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 128;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 499:
                    this.name = "Greater Healing Potion";
                    this.useSound = 3;
                    this.healLife = 150;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 30;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.rare = 3;
                    this.potion = true;
                    this.value = 5000;
                    break;
                case 500:
                    this.name = "Greater Mana Potion";
                    this.useSound = 3;
                    this.healMana = 200;
                    this.useStyle = 2;
                    this.useTurn = true;
                    this.useAnimation = 17;
                    this.useTime = 17;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.width = 14;
                    this.height = 24;
                    this.rare = 3;
                    this.value = 500;
                    break;
                case 501:
                    this.name = "Pixie Dust";
                    this.width = 16;
                    this.height = 14;
                    this.maxStack = 99;
                    this.value = 500;
                    this.rare = 1;
                    break;
                case 502:
                    this.name = "Crystal Shard";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 129;
                    this.width = 24;
                    this.height = 24;
                    this.value = 8000;
                    this.rare = 1;
                    break;
                case 503:
                    this.name = "Clown Hat";
                    this.width = 18;
                    this.height = 18;
                    this.headSlot = 40;
                    this.value = 20000;
                    this.vanity = true;
                    this.rare = 2;
                    break;
                case 504:
                    this.name = "Clown Shirt";
                    this.width = 18;
                    this.height = 18;
                    this.bodySlot = 23;
                    this.value = 10000;
                    this.vanity = true;
                    this.rare = 2;
                    break;
                case 505:
                    this.name = "Clown Pants";
                    this.width = 18;
                    this.height = 18;
                    this.legSlot = 22;
                    this.value = 10000;
                    this.vanity = true;
                    this.rare = 2;
                    break;
                case 506:
                    this.useStyle = 5;
                    this.autoReuse = true;
                    this.useAnimation = 30;
                    this.useTime = 6;
                    this.name = "Flamethrower";
                    this.width = 50;
                    this.height = 18;
                    this.shoot = 85;
                    this.useAmmo = 23;
                    this.useSound = 34;
                    this.damage = 27;
                    this.knockBack = 0.3f;
                    this.shootSpeed = 7f;
                    this.noMelee = true;
                    this.value = 500000;
                    this.rare = 5;
                    this.ranged = true;
                    this.toolTip = "Uses gel for ammo";
                    break;
                case 507:
                    this.rare = 3;
                    this.useStyle = 1;
                    this.useAnimation = 12;
                    this.useTime = 12;
                    this.name = "Bell";
                    this.width = 12;
                    this.height = 28;
                    this.autoReuse = true;
                    this.noMelee = true;
                    this.value = 10000;
                    break;
                case 508:
                    this.rare = 3;
                    this.useStyle = 5;
                    this.useAnimation = 12;
                    this.useTime = 12;
                    this.name = "Harp";
                    this.width = 12;
                    this.height = 28;
                    this.autoReuse = true;
                    this.noMelee = true;
                    this.value = 10000;
                    break;
                case 509:
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.name = "Wrench";
                    this.width = 24;
                    this.height = 28;
                    this.rare = 1;
                    this.toolTip = "Places wire";
                    this.value = 20000;
                    this.mech = true;
                    break;
                case 510:
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.name = "Wire Cutter";
                    this.width = 24;
                    this.height = 28;
                    this.rare = 1;
                    this.toolTip = "Removes wire";
                    this.value = 20000;
                    this.mech = true;
                    break;
                case 511:
                    this.name = "Active Stone Block";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 130;
                    this.width = 12;
                    this.height = 12;
                    this.value = 1000;
                    this.mech = true;
                    break;
                case 512:
                    this.name = "Inactive Stone Block";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 131;
                    this.width = 12;
                    this.height = 12;
                    this.value = 1000;
                    this.mech = true;
                    break;
                case 513:
                    this.name = "Lever";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 132;
                    this.width = 24;
                    this.height = 24;
                    this.value = 3000;
                    this.mech = true;
                    break;
                case 514:
                    this.autoReuse = true;
                    this.useStyle = 5;
                    this.useAnimation = 12;
                    this.useTime = 12;
                    this.name = "Laser Rifle";
                    this.width = 36;
                    this.height = 22;
                    this.shoot = 88;
                    this.mana = 8;
                    this.useSound = 12;
                    this.knockBack = 2.5f;
                    this.damage = 29;
                    this.shootSpeed = 17f;
                    this.noMelee = true;
                    this.rare = 4;
                    this.magic = true;
                    this.value = 500000;
                    break;
                case 515:
                    this.name = "Crystal Bullet";
                    this.shootSpeed = 5f;
                    this.shoot = 89;
                    this.damage = 9;
                    this.width = 8;
                    this.height = 8;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.ammo = 14;
                    this.knockBack = 1f;
                    this.value = 30;
                    this.ranged = true;
                    this.rare = 3;
                    this.toolTip = "Creates several crystal shards on impact";
                    break;
                case 516:
                    this.name = "Holy Arrow";
                    this.shootSpeed = 3.5f;
                    this.shoot = 91;
                    this.damage = 6;
                    this.width = 10;
                    this.height = 28;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.ammo = 1;
                    this.knockBack = 2f;
                    this.value = 80;
                    this.ranged = true;
                    this.rare = 3;
                    this.toolTip = "Summons falling stars on impact";
                    break;
                case 517:
                    this.useStyle = 1;
                    this.name = "Magic Dagger";
                    this.shootSpeed = 10f;
                    this.shoot = 93;
                    this.damage = 28;
                    this.width = 18;
                    this.height = 20;
                    this.mana = 7;
                    this.useSound = 1;
                    this.useAnimation = 15;
                    this.useTime = 15;
                    this.noUseGraphic = true;
                    this.noMelee = true;
                    this.value = 1000000;
                    this.knockBack = 2f;
                    this.magic = true;
                    this.rare = 4;
                    this.toolTip = "A magical returning dagger";
                    break;
                case 518:
                    this.autoReuse = true;
                    this.rare = 4;
                    this.mana = 5;
                    this.useSound = 9;
                    this.name = "Crystal Storm";
                    this.useStyle = 5;
                    this.damage = 26;
                    this.useAnimation = 7;
                    this.useTime = 7;
                    this.width = 24;
                    this.height = 28;
                    this.shoot = 94;
                    this.scale = 0.9f;
                    this.shootSpeed = 16f;
                    this.knockBack = 5f;
                    this.toolTip = "Summons rapid fire crystal shards";
                    this.magic = true;
                    this.value = 500000;
                    break;
                case 519:
                    this.autoReuse = true;
                    this.rare = 4;
                    this.mana = 14;
                    this.useSound = 20;
                    this.name = "Cursed Flames";
                    this.useStyle = 5;
                    this.damage = 35;
                    this.useAnimation = 20;
                    this.useTime = 20;
                    this.width = 24;
                    this.height = 28;
                    this.shoot = 95;
                    this.scale = 0.9f;
                    this.shootSpeed = 10f;
                    this.knockBack = 6.5f;
                    this.toolTip = "Summons unholy fire balls";
                    this.magic = true;
                    this.value = 500000;
                    break;
                case 520:
                    this.name = "Soul of Light";
                    this.width = 18;
                    this.height = 18;
                    this.maxStack = 250;
                    this.value = 1000;
                    this.rare = 3;
                    this.toolTip = "'The essence of light creatures'";
                    break;
                case 521:
                    this.name = "Soul of Night";
                    this.width = 18;
                    this.height = 18;
                    this.maxStack = 250;
                    this.value = 1000;
                    this.rare = 3;
                    this.toolTip = "'The essence of dark creatures'";
                    break;
                case 522:
                    this.name = "Cursed Flame";
                    this.width = 12;
                    this.height = 14;
                    this.maxStack = 99;
                    this.value = 4000;
                    this.rare = 3;
                    this.toolTip = "'Not even water can put the flame out'";
                    break;
                case 523:
                    this.name = "Cursed Torch";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.holdStyle = 1;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 4;
                    this.placeStyle = 8;
                    this.width = 10;
                    this.height = 12;
                    this.value = 300;
                    this.rare = 1;
                    this.toolTip = "Can be placed in water";
                    break;
                case 524:
                    this.name = "Adamantite Forge";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 133;
                    this.width = 44;
                    this.height = 30;
                    this.value = 50000;
                    this.toolTip = "Used to smelt adamantite ore";
                    this.rare = 3;
                    break;
                case 525:
                    this.name = "Mythril Anvil";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 134;
                    this.width = 28;
                    this.height = 14;
                    this.value = 25000;
                    this.toolTip = "Used to craft items from mythril and adamantite bars";
                    this.rare = 3;
                    break;
                case 526:
                    this.name = "Unicorn Horn";
                    this.width = 14;
                    this.height = 14;
                    this.maxStack = 99;
                    this.value = 15000;
                    this.rare = 1;
                    this.toolTip = "'Sharp and magical!'";
                    break;
                case 527:
                    this.name = "Dark Shard";
                    this.width = 14;
                    this.height = 14;
                    this.maxStack = 99;
                    this.value = 4500;
                    this.rare = 2;
                    this.toolTip = "'Sometimes carried by creatures in corrupt deserts'";
                    break;
                case 528:
                    this.name = "Light Shard";
                    this.width = 14;
                    this.height = 14;
                    this.maxStack = 99;
                    this.value = 4500;
                    this.rare = 2;
                    this.toolTip = "'Sometimes carried by creatures in light deserts'";
                    break;
                case 529:
                    this.name = "Red Pressure Plate";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 135;
                    this.width = 12;
                    this.height = 12;
                    this.placeStyle = 0;
                    this.mech = true;
                    this.value = 5000;
                    this.mech = true;
                    this.toolTip = "Activates when stepped on";
                    break;
                case 530:
                    this.name = "Wire";
                    this.width = 12;
                    this.height = 18;
                    this.maxStack = 250;
                    this.value = 500;
                    this.mech = true;
                    break;
                case 531:
                    this.name = "Spell Tome";
                    this.width = 12;
                    this.height = 18;
                    this.maxStack = 99;
                    this.value = 50000;
                    this.rare = 1;
                    this.toolTip = "Can be enchanted";
                    break;
                case 532:
                    this.name = "Star Cloak";
                    this.width = 20;
                    this.height = 24;
                    this.value = 100000;
                    this.toolTip = "Causes stars to fall when injured";
                    this.accessory = true;
                    this.rare = 4;
                    break;
                case 533:
                    this.useStyle = 5;
                    this.autoReuse = true;
                    this.useAnimation = 7;
                    this.useTime = 7;
                    this.name = "Megashark";
                    this.width = 50;
                    this.height = 18;
                    this.shoot = 10;
                    this.useAmmo = 14;
                    this.useSound = 11;
                    this.damage = 23;
                    this.shootSpeed = 10f;
                    this.noMelee = true;
                    this.value = 300000;
                    this.rare = 5;
                    this.toolTip = "50% chance to not consume ammo";
                    this.toolTip2 = "'Minishark's older brother'";
                    this.knockBack = 1f;
                    this.ranged = true;
                    break;
                case 534:
                    this.knockBack = 6.5f;
                    this.useStyle = 5;
                    this.useAnimation = 45;
                    this.useTime = 45;
                    this.name = "Shotgun";
                    this.width = 50;
                    this.height = 14;
                    this.shoot = 10;
                    this.useAmmo = 14;
                    this.useSound = 36;
                    this.damage = 18;
                    this.shootSpeed = 6f;
                    this.noMelee = true;
                    this.value = 700000;
                    this.rare = 4;
                    this.ranged = true;
                    this.toolTip = "Fires a spread of bullets";
                    break;
                case 535:
                    this.name = "Philosopher's Stone";
                    this.width = 12;
                    this.height = 18;
                    this.value = 100000;
                    this.toolTip = "Reduces the cooldown of healing potions";
                    this.accessory = true;
                    this.rare = 4;
                    break;
                case 536:
                    this.name = "Titan Glove";
                    this.width = 12;
                    this.height = 18;
                    this.value = 100000;
                    this.toolTip = "Increases melee knockback";
                    this.rare = 4;
                    this.accessory = true;
                    break;
                case 537:
                    this.name = "Cobalt Naginata";
                    this.useStyle = 5;
                    this.useAnimation = 28;
                    this.useTime = 28;
                    this.shootSpeed = 4.3f;
                    this.knockBack = 4f;
                    this.width = 40;
                    this.height = 40;
                    this.damage = 29;
                    this.scale = 1.1f;
                    this.useSound = 1;
                    this.shoot = 97;
                    this.rare = 4;
                    this.value = 45000;
                    this.noMelee = true;
                    this.noUseGraphic = true;
                    this.melee = true;
                    break;
                case 538:
                    this.name = "Switch";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 136;
                    this.width = 12;
                    this.height = 12;
                    this.value = 2000;
                    this.mech = true;
                    break;
                case 539:
                    this.name = "Dart Trap";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 137;
                    this.width = 12;
                    this.height = 12;
                    this.value = 10000;
                    this.mech = true;
                    break;
                case 540:
                    this.name = "Boulder";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 138;
                    this.width = 12;
                    this.height = 12;
                    this.mech = true;
                    break;
                case 541:
                    this.name = "Green Pressure Plate";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 135;
                    this.width = 12;
                    this.height = 12;
                    this.placeStyle = 1;
                    this.mech = true;
                    this.value = 5000;
                    this.toolTip = "Activates when stepped on";
                    break;
                case 542:
                    this.name = "Gray Pressure Plate";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 135;
                    this.width = 12;
                    this.height = 12;
                    this.placeStyle = 2;
                    this.mech = true;
                    this.value = 5000;
                    this.toolTip = "Activates when stepped on";
                    break;
                case 543:
                    this.name = "Brown Pressure Plate";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 135;
                    this.width = 12;
                    this.height = 12;
                    this.placeStyle = 3;
                    this.mech = true;
                    this.value = 5000;
                    this.toolTip = "Activates when stepped on";
                    break;
                case 544:
                    this.useStyle = 4;
                    this.name = "Mechanical Eye";
                    this.width = 22;
                    this.height = 14;
                    this.consumable = true;
                    this.useAnimation = 45;
                    this.useTime = 45;
                    this.maxStack = 20;
                    this.toolTip = "Summons The Twins";
                    this.rare = 3;
                    break;
                case 545:
                    this.name = "Cursed Arrow";
                    this.shootSpeed = 4f;
                    this.shoot = 103;
                    this.damage = 14;
                    this.width = 10;
                    this.height = 28;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.ammo = 1;
                    this.knockBack = 3f;
                    this.value = 80;
                    this.ranged = true;
                    this.rare = 3;
                    break;
                case 546:
                    this.name = "Cursed Bullet";
                    this.shootSpeed = 5f;
                    this.shoot = 104;
                    this.damage = 12;
                    this.width = 8;
                    this.height = 8;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.ammo = 14;
                    this.knockBack = 4f;
                    this.value = 30;
                    this.rare = 1;
                    this.ranged = true;
                    this.rare = 3;
                    break;
                case 547:
                    this.name = "Soul of Fright";
                    this.width = 18;
                    this.height = 18;
                    this.maxStack = 250;
                    this.value = 100000;
                    this.rare = 5;
                    this.toolTip = "'The essence of pure terror'";
                    break;
                case 548:
                    this.name = "Soul of Might";
                    this.width = 18;
                    this.height = 18;
                    this.maxStack = 250;
                    this.value = 100000;
                    this.rare = 5;
                    this.toolTip = "'The essence of the destroyer'";
                    break;
                case 549:
                    this.name = "Soul of Sight";
                    this.width = 18;
                    this.height = 18;
                    this.maxStack = 250;
                    this.value = 100000;
                    this.rare = 5;
                    this.toolTip = "'The essence of omniscient watchers'";
                    break;
                case 550:
                    this.name = "Gungnir";
                    this.useStyle = 5;
                    this.useAnimation = 22;
                    this.useTime = 22;
                    this.shootSpeed = 5.6f;
                    this.knockBack = 6.4f;
                    this.width = 40;
                    this.height = 40;
                    this.damage = 42;
                    this.scale = 1.1f;
                    this.useSound = 1;
                    this.shoot = 105;
                    this.rare = 5;
                    this.value = 1500000;
                    this.noMelee = true;
                    this.noUseGraphic = true;
                    this.melee = true;
                    break;
                case 551:
                    this.name = "Hallowed Plate Mail";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 15;
                    this.bodySlot = 24;
                    this.rare = 5;
                    this.value = 200000;
                    this.toolTip = "7% increased critical strike chance";
                    break;
                case 552:
                    this.name = "Hallowed Greaves";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 11;
                    this.legSlot = 23;
                    this.rare = 5;
                    this.value = 150000;
                    this.toolTip = "7% increased damage";
                    this.toolTip2 = "8% increased movement speed";
                    break;
                case 553:
                    this.name = "Hallowed Helmet";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 9;
                    this.headSlot = 41;
                    this.rare = 5;
                    this.value = 250000;
                    this.toolTip = "15% increased ranged damage";
                    this.toolTip2 = "8% increased ranged critical strike chance";
                    break;
                case 558:
                    this.name = "Hallowed Headgear";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 5;
                    this.headSlot = 42;
                    this.rare = 5;
                    this.value = 250000;
                    this.toolTip = "Increases maximum mana by 100";
                    this.toolTip2 = "12% increased magic damage and critical strike chance";
                    break;
                case 559:
                    this.name = "Hallowed Mask";
                    this.width = 18;
                    this.height = 18;
                    this.defense = 24;
                    this.headSlot = 43;
                    this.rare = 5;
                    this.value = 250000;
                    this.toolTip = "10% increased melee damage and critical strike chance";
                    this.toolTip2 = "10% increased melee haste";
                    break;
                case 554:
                    this.name = "Cross Necklace";
                    this.width = 20;
                    this.height = 24;
                    this.value = 1500;
                    this.toolTip = "Increases length of invincibility after taking damage";
                    this.accessory = true;
                    this.rare = 4;
                    break;
                case 555:
                    this.name = "Mana Flower";
                    this.width = 20;
                    this.height = 24;
                    this.value = 50000;
                    this.toolTip = "8% reduced mana usage";
                    this.toolTip2 = "Automatically use mana potions when needed";
                    this.accessory = true;
                    this.rare = 4;
                    break;
                case 556:
                    this.useStyle = 4;
                    this.name = "Mechanical Worm";
                    this.width = 22;
                    this.height = 14;
                    this.consumable = true;
                    this.useAnimation = 45;
                    this.useTime = 45;
                    this.maxStack = 20;
                    this.toolTip = "Summons Destroyer";
                    this.rare = 3;
                    break;
                case 557:
                    this.useStyle = 4;
                    this.name = "Mechanical Skull";
                    this.width = 22;
                    this.height = 14;
                    this.consumable = true;
                    this.useAnimation = 45;
                    this.useTime = 45;
                    this.maxStack = 20;
                    this.toolTip = "Summons Skeletron Prime";
                    this.rare = 3;
                    break;
                case 560:
                    this.useStyle = 4;
                    this.name = "Slime Crown";
                    this.width = 22;
                    this.height = 14;
                    this.consumable = true;
                    this.useAnimation = 45;
                    this.useTime = 45;
                    this.maxStack = 20;
                    this.toolTip = "Summons King Slime";
                    this.rare = 1;
                    break;
                case 561:
                    this.melee = true;
                    this.autoReuse = true;
                    this.noMelee = true;
                    this.useStyle = 1;
                    this.name = "Light Disc";
                    this.shootSpeed = 13f;
                    this.shoot = 106;
                    this.damage = 35;
                    this.knockBack = 8f;
                    this.width = 24;
                    this.height = 24;
                    this.useSound = 1;
                    this.useAnimation = 15;
                    this.useTime = 15;
                    this.noUseGraphic = true;
                    this.rare = 5;
                    this.maxStack = 5;
                    this.value = 500000;
                    this.toolTip = "Stacks up to 5";
                    break;
                case 562:
                    this.name = "Music Box (Overworld Day)";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.consumable = true;
                    this.createTile = 139;
                    this.placeStyle = 0;
                    this.width = 24;
                    this.height = 24;
                    this.rare = 4;
                    this.value = 100000;
                    this.accessory = true;
                    break;
                case 563:
                    this.name = "Music Box (Eerie)";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.consumable = true;
                    this.createTile = 139;
                    this.placeStyle = 1;
                    this.width = 24;
                    this.height = 24;
                    this.rare = 4;
                    this.value = 100000;
                    this.accessory = true;
                    break;
                case 564:
                    this.name = "Music Box (Night)";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.consumable = true;
                    this.createTile = 139;
                    this.placeStyle = 2;
                    this.width = 24;
                    this.height = 24;
                    this.rare = 4;
                    this.value = 100000;
                    this.accessory = true;
                    break;
                case 565:
                    this.name = "Music Box (Title)";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.consumable = true;
                    this.createTile = 139;
                    this.placeStyle = 3;
                    this.width = 24;
                    this.height = 24;
                    this.rare = 4;
                    this.value = 100000;
                    this.accessory = true;
                    break;
                case 566:
                    this.name = "Music Box (Underground)";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.consumable = true;
                    this.createTile = 139;
                    this.placeStyle = 4;
                    this.width = 24;
                    this.height = 24;
                    this.rare = 4;
                    this.value = 100000;
                    this.accessory = true;
                    break;
                case 567:
                    this.name = "Music Box (Boss 1)";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.consumable = true;
                    this.createTile = 139;
                    this.placeStyle = 5;
                    this.width = 24;
                    this.height = 24;
                    this.rare = 4;
                    this.value = 100000;
                    this.accessory = true;
                    break;
                case 568:
                    this.name = "Music Box (Jungle)";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.consumable = true;
                    this.createTile = 139;
                    this.placeStyle = 6;
                    this.width = 24;
                    this.height = 24;
                    this.rare = 4;
                    this.value = 100000;
                    this.accessory = true;
                    break;
                case 569:
                    this.name = "Music Box (Corruption)";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.consumable = true;
                    this.createTile = 139;
                    this.placeStyle = 7;
                    this.width = 24;
                    this.height = 24;
                    this.rare = 4;
                    this.value = 100000;
                    this.accessory = true;
                    break;
                case 570:
                    this.name = "Music Box (Underground Corruption)";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.consumable = true;
                    this.createTile = 139;
                    this.placeStyle = 8;
                    this.width = 24;
                    this.height = 24;
                    this.rare = 4;
                    this.value = 100000;
                    this.accessory = true;
                    break;
                case 571:
                    this.name = "Music Box (The Hallow)";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.consumable = true;
                    this.createTile = 139;
                    this.placeStyle = 9;
                    this.width = 24;
                    this.height = 24;
                    this.rare = 4;
                    this.value = 100000;
                    this.accessory = true;
                    break;
                case 572:
                    this.name = "Music Box (Boss 2)";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.consumable = true;
                    this.createTile = 139;
                    this.placeStyle = 10;
                    this.width = 24;
                    this.height = 24;
                    this.rare = 4;
                    this.value = 100000;
                    this.accessory = true;
                    break;
                case 573:
                    this.name = "Music Box (Underground Hallow)";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.consumable = true;
                    this.createTile = 139;
                    this.placeStyle = 11;
                    this.width = 24;
                    this.height = 24;
                    this.rare = 4;
                    this.value = 100000;
                    this.accessory = true;
                    break;
                case 574:
                    this.name = "Music Box (Boss 3)";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.consumable = true;
                    this.createTile = 139;
                    this.placeStyle = 12;
                    this.width = 24;
                    this.height = 24;
                    this.rare = 3;
                    this.value = 100000;
                    this.accessory = true;
                    break;
                case 575:
                    this.name = "Soul of Flight";
                    this.width = 18;
                    this.height = 18;
                    this.maxStack = 250;
                    this.value = 1000;
                    this.rare = 3;
                    this.toolTip = "'The essence of powerful flying creatures'";
                    break;
                case 576:
                    this.name = "Music Box";
                    this.width = 24;
                    this.height = 24;
                    this.rare = 3;
                    this.toolTip = "Has a chance to record songs";
                    this.value = 100000;
                    this.accessory = true;
                    break;
                case 577:
                    this.name = "Demonite Brick";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 140;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 578:
                    this.useStyle = 5;
                    this.autoReuse = true;
                    this.useAnimation = 19;
                    this.useTime = 19;
                    this.name = "Hallowed Repeater";
                    this.width = 50;
                    this.height = 18;
                    this.shoot = 1;
                    this.useAmmo = 1;
                    this.useSound = 5;
                    this.damage = 39;
                    this.shootSpeed = 11f;
                    this.noMelee = true;
                    this.value = 200000;
                    this.ranged = true;
                    this.rare = 4;
                    this.knockBack = 2.5f;
                    break;
                case 579:
                    this.name = "Hamdrax";
                    this.useStyle = 5;
                    this.useAnimation = 25;
                    this.useTime = 7;
                    this.shootSpeed = 36f;
                    this.knockBack = 4.75f;
                    this.width = 20;
                    this.height = 12;
                    this.damage = 35;
                    this.pick = 200;
                    this.axe = 22;
                    this.hammer = 85;
                    this.useSound = 23;
                    this.shoot = 107;
                    this.rare = 4;
                    this.value = 220000;
                    this.noMelee = true;
                    this.noUseGraphic = true;
                    this.melee = true;
                    this.channel = true;
                    this.toolTip = "'Not to be confused with a hamsaw'";
                    break;
                case 580:
                    this.mech = true;
                    this.name = "Explosives";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 141;
                    this.width = 12;
                    this.height = 12;
                    this.toolTip = "Explodes when activated";
                    break;
                case 581:
                    this.mech = true;
                    this.name = "Inlet Pump";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 142;
                    this.width = 12;
                    this.height = 12;
                    this.toolTip = "Sends water to outlet pumps";
                    break;
                case 582:
                    this.mech = true;
                    this.name = "Outlet Pump";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 143;
                    this.width = 12;
                    this.height = 12;
                    this.toolTip = "Receives water from inlet pumps";
                    break;
                case 583:
                    this.mech = true;
                    this.noWet = true;
                    this.name = "1 Second Timer";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 144;
                    this.placeStyle = 0;
                    this.width = 10;
                    this.height = 12;
                    this.value = 50;
                    this.toolTip = "Activates every second";
                    break;
                case 584:
                    this.mech = true;
                    this.noWet = true;
                    this.name = "3 Second Timer";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 144;
                    this.placeStyle = 1;
                    this.width = 10;
                    this.height = 12;
                    this.value = 50;
                    this.toolTip = "Activates every 3 seconds";
                    break;
                case 585:
                    this.mech = true;
                    this.noWet = true;
                    this.name = "5 Second Timer";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 99;
                    this.consumable = true;
                    this.createTile = 144;
                    this.placeStyle = 2;
                    this.width = 10;
                    this.height = 12;
                    this.value = 50;
                    this.toolTip = "Activates every 5 seconds";
                    break;
                case 586:
                    this.name = "Candy Cane Block";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 145;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 587:
                    this.name = "Candy Cane Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 29;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 588:
                    this.name = "Santa Hat";
                    this.width = 18;
                    this.height = 12;
                    this.headSlot = 44;
                    this.value = 150000;
                    this.vanity = true;
                    break;
                case 589:
                    this.name = "Santa Shirt";
                    this.width = 18;
                    this.height = 18;
                    this.bodySlot = 25;
                    this.value = 150000;
                    this.vanity = true;
                    break;
                case 590:
                    this.name = "Santa Pants";
                    this.width = 18;
                    this.height = 18;
                    this.legSlot = 24;
                    this.value = 150000;
                    this.vanity = true;
                    break;
                case 591:
                    this.name = "Green Candy Cane Block";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 146;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 592:
                    this.name = "Green Candy Cane Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 30;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 593:
                    this.name = "Snow Block";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 147;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 594:
                    this.name = "Snow Brick";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 148;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 595:
                    this.name = "Snow Brick Wall";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createWall = 31;
                    this.width = 12;
                    this.height = 12;
                    break;
                case 596:
                    this.name = "Blue Light";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 149;
                    this.placeStyle = 0;
                    this.width = 12;
                    this.height = 12;
                    this.value = 500;
                    break;
                case 597:
                    this.name = "Red Light";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 149;
                    this.placeStyle = 1;
                    this.width = 12;
                    this.height = 12;
                    this.value = 500;
                    break;
                case 598:
                    this.name = "Green Light";
                    this.useStyle = 1;
                    this.useTurn = true;
                    this.useAnimation = 15;
                    this.useTime = 10;
                    this.autoReuse = true;
                    this.maxStack = 250;
                    this.consumable = true;
                    this.createTile = 149;
                    this.placeStyle = 2;
                    this.width = 12;
                    this.height = 12;
                    this.value = 500;
                    break;
                case 599:
                    this.name = "Blue Present";
                    this.width = 12;
                    this.height = 12;
                    this.rare = 1;
                    this.toolTip = "Right click to open";
                    break;
                case 600:
                    this.name = "Green Present";
                    this.width = 12;
                    this.height = 12;
                    this.rare = 1;
                    this.toolTip = "Right click to open";
                    break;
                case 601:
                    this.name = "Yellow Present";
                    this.width = 12;
                    this.height = 12;
                    this.rare = 1;
                    this.toolTip = "Right click to open";
                    break;
                case 602:
                    this.name = "Snow Globe";
                    this.useStyle = 4;
                    this.consumable = true;
                    this.useAnimation = 45;
                    this.useTime = 45;
                    this.width = 28;
                    this.height = 28;
                    this.toolTip = "Summons the Frost Legion";
                    this.rare = 2;
                    break;
            }
		    if (!noMatCheck)
			{
				this.checkMat();
			}
			this.netID = this.type;
		}
        public void netDefaults(int Type)
        {
            RealnetDefaults(Type);
            ItemHooks.OnNetDefaults(ref Type, this);
        }
        public void SetDefaults(int Type, bool noMatCheck = false)
        {
            RealSetDefaults(Type, noMatCheck);
            ItemHooks.OnSetDefaultsInt(ref Type, this);
        }
        public void SetDefaults(string ItemName)
        {
            RealSetDefaults(ItemName);
            ItemHooks.OnSetDefaultsString(ref ItemName, this);
        }
		public static string VersionName(string oldName, int release)
		{
			string result = oldName;
			if (release <= 4)
			{
				switch (oldName)
				{
				    case "Cobalt Helmet":
				        result = "Jungle Hat";
				        break;
				    case "Cobalt Breastplate":
				        result = "Jungle Shirt";
				        break;
				    case "Cobalt Greaves":
				        result = "Jungle Pants";
				        break;
				}
			}
			if (release <= 13 && oldName == "Jungle Rose")
			{
				result = "Jungle Spores";
			}
			if (release <= 20)
			{
				switch (oldName)
				{
				    case "Gills potion":
				        result = "Gills Potion";
				        break;
				    case "Thorn Chakrum":
				        result = "Thorn Chakram";
				        break;
				    case "Ball 'O Hurt":
				        result = "Ball O' Hurt";
				        break;
				}
			}
			return result;
		}
		public Color GetAlpha(Color newColor)
		{
			if (this.type == 75)
			{
				return new Color(255, 255, 255, (int)newColor.A - this.alpha);
			}
			if (this.type == 121 || this.type == 122 || this.type == 217 || this.type == 218 || this.type == 219 || this.type == 220 || this.type == 120 || this.type == 119)
			{
				return new Color(255, 255, 255, 255);
			}
			if (this.type == 501)
			{
				return new Color(200, 200, 200, 50);
			}
			if (this.type == 520 || this.type == 521 || this.type == 522 || this.type == 547 || this.type == 548 || this.type == 549 || this.type == 575)
			{
				return new Color(255, 255, 255, 50);
			}
			if (this.type == 58 || this.type == 184)
			{
				return new Color(200, 200, 200, 2000);
			}
			float num = (float)(255 - this.alpha) / 255f;
			int r = (int)((float)newColor.R * num);
			int g = (int)((float)newColor.G * num);
			int b = (int)((float)newColor.B * num);
			int num2 = (int)newColor.A - this.alpha;
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num2 > 255)
			{
				num2 = 255;
			}
			if (this.type >= 198 && this.type <= 203)
			{
				return Color.White;
			}
			return new Color(r, g, b, num2);
		}
		public Color GetColor(Color newColor)
		{
			int num = (int)(this.color.R - (255 - newColor.R));
			int num2 = (int)(this.color.G - (255 - newColor.G));
			int num3 = (int)(this.color.B - (255 - newColor.B));
			int num4 = (int)(this.color.A - (255 - newColor.A));
			if (num < 0)
			{
				num = 0;
			}
			if (num > 255)
			{
				num = 255;
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			if (num2 > 255)
			{
				num2 = 255;
			}
			if (num3 < 0)
			{
				num3 = 0;
			}
			if (num3 > 255)
			{
				num3 = 255;
			}
			if (num4 < 0)
			{
				num4 = 0;
			}
			if (num4 > 255)
			{
				num4 = 255;
			}
			return new Color(num, num2, num3, num4);
		}
		public static bool MechSpawn(float x, float y, int type)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < 200; i++)
			{
				if (Main.item[i].active && Main.item[i].type == type)
				{
					num++;
					Vector2 vector = new Vector2(x, y);
					float num4 = Main.item[i].position.X - vector.X;
					float num5 = Main.item[i].position.Y - vector.Y;
					float num6 = (float)Math.Sqrt((double)(num4 * num4 + num5 * num5));
					if (num6 < 300f)
					{
						num2++;
					}
					if (num6 < 800f)
					{
						num3++;
					}
				}
			}
			return num2 < 3 && num3 < 6 && num < 10;
		}
		public void UpdateItem(int i)
		{
			if (this.active)
			{
				if (Main.netMode == 0)
				{
					this.owner = Main.myPlayer;
				}
				float num = 0.1f;
				float num2 = 7f;
				int num3 = (int)(this.position.X + (float)(this.width / 2)) / 16;
				int num4 = (int)(this.position.Y + (float)(this.height / 2)) / 16;
				if (Main.tile[num3, num4] == null)
				{
					num = 0f;
					this.velocity.X = 0f;
					this.velocity.Y = 0f;
				}
				if (this.wet)
				{
					num2 = 5f;
					num = 0.08f;
				}
				Vector2 value = this.velocity * 0.5f;
				if (this.ownTime > 0)
				{
					this.ownTime--;
				}
				else
				{
					this.ownIgnore = -1;
				}
				if (this.keepTime > 0)
				{
					this.keepTime--;
				}
				if (!this.beingGrabbed)
				{
					if (this.type == 520 || this.type == 521 || this.type == 547 || this.type == 548 || this.type == 549 || this.type == 575)
					{
						this.velocity.X = this.velocity.X * 0.95f;
						if ((double)this.velocity.X < 0.1 && (double)this.velocity.X > -0.1)
						{
							this.velocity.X = 0f;
						}
						this.velocity.Y = this.velocity.Y * 0.95f;
						if ((double)this.velocity.Y < 0.1 && (double)this.velocity.Y > -0.1)
						{
							this.velocity.Y = 0f;
						}
					}
					else
					{
						this.velocity.Y = this.velocity.Y + num;
						if (this.velocity.Y > num2)
						{
							this.velocity.Y = num2;
						}
						this.velocity.X = this.velocity.X * 0.95f;
						if ((double)this.velocity.X < 0.1 && (double)this.velocity.X > -0.1)
						{
							this.velocity.X = 0f;
						}
					}
					bool flag = Collision.LavaCollision(this.position, this.width, this.height);
					if (flag)
					{
						this.lavaWet = true;
					}
					bool flag2 = Collision.WetCollision(this.position, this.width, this.height);
					if (flag2)
					{
						if (!this.wet)
						{
							if (this.wetCount == 0)
							{
								this.wetCount = 20;
								if (!flag)
								{
									for (int j = 0; j < 10; j++)
									{
										int num5 = Dust.NewDust(new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f), this.width + 12, 24, 33, 0f, 0f, 0, default(Color), 1f);
										Dust expr_35E_cp_0 = Main.dust[num5];
										expr_35E_cp_0.velocity.Y = expr_35E_cp_0.velocity.Y - 4f;
										Dust expr_37C_cp_0 = Main.dust[num5];
										expr_37C_cp_0.velocity.X = expr_37C_cp_0.velocity.X * 2.5f;
										Main.dust[num5].scale = 1.3f;
										Main.dust[num5].alpha = 100;
										Main.dust[num5].noGravity = true;
									}
									Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 1);
								}
								else
								{
									for (int k = 0; k < 5; k++)
									{
										int num6 = Dust.NewDust(new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f), this.width + 12, 24, 35, 0f, 0f, 0, default(Color), 1f);
										Dust expr_464_cp_0 = Main.dust[num6];
										expr_464_cp_0.velocity.Y = expr_464_cp_0.velocity.Y - 1.5f;
										Dust expr_482_cp_0 = Main.dust[num6];
										expr_482_cp_0.velocity.X = expr_482_cp_0.velocity.X * 2.5f;
										Main.dust[num6].scale = 1.3f;
										Main.dust[num6].alpha = 100;
										Main.dust[num6].noGravity = true;
									}
									Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 1);
								}
							}
							this.wet = true;
						}
					}
					else
					{
						if (this.wet)
						{
							this.wet = false;
						}
					}
					if (!this.wet)
					{
						this.lavaWet = false;
					}
					if (this.wetCount > 0)
					{
						this.wetCount -= 1;
					}
					if (this.wet)
					{
						if (this.wet)
						{
							Vector2 vector = this.velocity;
							this.velocity = Collision.TileCollision(this.position, this.velocity, this.width, this.height, false, false);
							if (this.velocity.X != vector.X)
							{
								value.X = this.velocity.X;
							}
							if (this.velocity.Y != vector.Y)
							{
								value.Y = this.velocity.Y;
							}
						}
					}
					else
					{
						this.velocity = Collision.TileCollision(this.position, this.velocity, this.width, this.height, false, false);
					}
					if (this.lavaWet)
					{
						if (this.type == 267)
						{
							if (Main.netMode != 1)
							{
								this.active = false;
								this.type = 0;
								this.name = "";
								this.stack = 0;
								for (int l = 0; l < 200; l++)
								{
									if (Main.npc[l].active && Main.npc[l].type == 22)
									{
										if (Main.netMode == 2)
										{
											NetMessage.SendData(28, -1, -1, "", l, 9999f, 10f, (float)(-(float)Main.npc[l].direction), 0);
										}
										Main.npc[l].StrikeNPC(9999, 10f, -Main.npc[l].direction, false, false);
										NPC.SpawnWOF(this.position);
									}
								}
								NetMessage.SendData(21, -1, -1, "", i, 0f, 0f, 0f, 0);
							}
						}
						else
						{
							if (this.owner == Main.myPlayer && this.type != 312 && this.type != 318 && this.type != 173 && this.type != 174 && this.type != 175 && this.rare == 0)
							{
								this.active = false;
								this.type = 0;
								this.name = "";
								this.stack = 0;
								if (Main.netMode != 0)
								{
									NetMessage.SendData(21, -1, -1, "", i, 0f, 0f, 0f, 0);
								}
							}
						}
					}
					if (this.type == 520)
					{
						float num7 = (float)Main.rand.Next(90, 111) * 0.01f;
						num7 *= Main.essScale;
						Lighting.addLight((int)((this.position.X + (float)(this.width / 2)) / 16f), (int)((this.position.Y + (float)(this.height / 2)) / 16f), 0.5f * num7, 0.1f * num7, 0.25f * num7);
					}
					else
					{
						if (this.type == 521)
						{
							float num8 = (float)Main.rand.Next(90, 111) * 0.01f;
							num8 *= Main.essScale;
							Lighting.addLight((int)((this.position.X + (float)(this.width / 2)) / 16f), (int)((this.position.Y + (float)(this.height / 2)) / 16f), 0.25f * num8, 0.1f * num8, 0.5f * num8);
						}
						else
						{
							if (this.type == 547)
							{
								float num9 = (float)Main.rand.Next(90, 111) * 0.01f;
								num9 *= Main.essScale;
								Lighting.addLight((int)((this.position.X + (float)(this.width / 2)) / 16f), (int)((this.position.Y + (float)(this.height / 2)) / 16f), 0.5f * num9, 0.3f * num9, 0.05f * num9);
							}
							else
							{
								if (this.type == 548)
								{
									float num10 = (float)Main.rand.Next(90, 111) * 0.01f;
									num10 *= Main.essScale;
									Lighting.addLight((int)((this.position.X + (float)(this.width / 2)) / 16f), (int)((this.position.Y + (float)(this.height / 2)) / 16f), 0.1f * num10, 0.1f * num10, 0.6f * num10);
								}
								else
								{
									if (this.type == 575)
									{
										float num11 = (float)Main.rand.Next(90, 111) * 0.01f;
										num11 *= Main.essScale;
										Lighting.addLight((int)((this.position.X + (float)(this.width / 2)) / 16f), (int)((this.position.Y + (float)(this.height / 2)) / 16f), 0.1f * num11, 0.3f * num11, 0.5f * num11);
									}
									else
									{
										if (this.type == 549)
										{
											float num12 = (float)Main.rand.Next(90, 111) * 0.01f;
											num12 *= Main.essScale;
											Lighting.addLight((int)((this.position.X + (float)(this.width / 2)) / 16f), (int)((this.position.Y + (float)(this.height / 2)) / 16f), 0.1f * num12, 0.5f * num12, 0.2f * num12);
										}
										else
										{
											if (this.type == 58)
											{
												float num13 = (float)Main.rand.Next(90, 111) * 0.01f;
												num13 *= Main.essScale * 0.5f;
												Lighting.addLight((int)((this.position.X + (float)(this.width / 2)) / 16f), (int)((this.position.Y + (float)(this.height / 2)) / 16f), 0.5f * num13, 0.1f * num13, 0.1f * num13);
											}
											else
											{
												if (this.type == 184)
												{
													float num14 = (float)Main.rand.Next(90, 111) * 0.01f;
													num14 *= Main.essScale * 0.5f;
													Lighting.addLight((int)((this.position.X + (float)(this.width / 2)) / 16f), (int)((this.position.Y + (float)(this.height / 2)) / 16f), 0.1f * num14, 0.1f * num14, 0.5f * num14);
												}
												else
												{
													if (this.type == 522)
													{
														float num15 = (float)Main.rand.Next(90, 111) * 0.01f;
														num15 *= Main.essScale * 0.2f;
														Lighting.addLight((int)((this.position.X + (float)(this.width / 2)) / 16f), (int)((this.position.Y + (float)(this.height / 2)) / 16f), 0.5f * num15, 1f * num15, 0.1f * num15);
													}
												}
											}
										}
									}
								}
							}
						}
					}
					if (this.type == 75 && Main.dayTime)
					{
						for (int m = 0; m < 10; m++)
						{
							Dust.NewDust(this.position, this.width, this.height, 15, this.velocity.X, this.velocity.Y, 150, default(Color), 1.2f);
						}
						for (int n = 0; n < 3; n++)
						{
							Gore.NewGore(this.position, new Vector2(this.velocity.X, this.velocity.Y), Main.rand.Next(16, 18), 1f);
						}
						this.active = false;
						this.type = 0;
						this.stack = 0;
						if (Main.netMode == 2)
						{
							NetMessage.SendData(21, -1, -1, "", i, 0f, 0f, 0f, 0);
						}
					}
				}
				else
				{
					this.beingGrabbed = false;
				}
				if (this.type == 501)
				{
					if (Main.rand.Next(6) == 0)
					{
						int num16 = Dust.NewDust(this.position, this.width, this.height, 55, 0f, 0f, 200, this.color, 1f);
						Dust expr_DC0 = Main.dust[num16];
						expr_DC0.velocity *= 0.3f;
						Main.dust[num16].scale *= 0.5f;
					}
				}
				else
				{
					if (this.type == 8 || this.type == 105)
					{
						if (!this.wet)
						{
							Lighting.addLight((int)((this.position.X + (float)(this.width / 2)) / 16f), (int)((this.position.Y + (float)(this.height / 2)) / 16f), 1f, 0.95f, 0.8f);
						}
					}
					else
					{
						if (this.type == 523)
						{
							Lighting.addLight((int)((this.position.X + (float)(this.width / 2)) / 16f), (int)((this.position.Y + (float)(this.height / 2)) / 16f), 0.85f, 1f, 0.7f);
						}
						else
						{
							if (this.type >= 427 && this.type <= 432)
							{
								if (!this.wet)
								{
									float r = 0f;
									float g = 0f;
									float b = 0f;
									int num17 = this.type - 426;
									if (num17 == 1)
									{
										r = 0.1f;
										g = 0.2f;
										b = 1.1f;
									}
									if (num17 == 2)
									{
										r = 1f;
										g = 0.1f;
										b = 0.1f;
									}
									if (num17 == 3)
									{
										r = 0f;
										g = 1f;
										b = 0.1f;
									}
									if (num17 == 4)
									{
										r = 0.9f;
										g = 0f;
										b = 0.9f;
									}
									if (num17 == 5)
									{
										r = 1.3f;
										g = 1.3f;
										b = 1.3f;
									}
									if (num17 == 6)
									{
										r = 0.9f;
										g = 0.9f;
										b = 0f;
									}
									Lighting.addLight((int)((this.position.X + (float)(this.width / 2)) / 16f), (int)((this.position.Y + (float)(this.height / 2)) / 16f), r, g, b);
								}
							}
							else
							{
								if (this.type == 41)
								{
									if (!this.wet)
									{
										Lighting.addLight((int)((this.position.X + (float)this.width) / 16f), (int)((this.position.Y + (float)(this.height / 2)) / 16f), 1f, 0.75f, 0.55f);
									}
								}
								else
								{
									if (this.type == 282)
									{
										Lighting.addLight((int)((this.position.X + (float)this.width) / 16f), (int)((this.position.Y + (float)(this.height / 2)) / 16f), 0.7f, 1f, 0.8f);
									}
									else
									{
										if (this.type == 286)
										{
											Lighting.addLight((int)((this.position.X + (float)this.width) / 16f), (int)((this.position.Y + (float)(this.height / 2)) / 16f), 0.7f, 0.8f, 1f);
										}
										else
										{
											if (this.type == 331)
											{
												Lighting.addLight((int)((this.position.X + (float)this.width) / 16f), (int)((this.position.Y + (float)(this.height / 2)) / 16f), 0.55f, 0.75f, 0.6f);
											}
											else
											{
												if (this.type == 183)
												{
													Lighting.addLight((int)((this.position.X + (float)this.width) / 16f), (int)((this.position.Y + (float)(this.height / 2)) / 16f), 0.15f, 0.45f, 0.9f);
												}
												else
												{
													if (this.type == 75)
													{
														Lighting.addLight((int)((this.position.X + (float)this.width) / 16f), (int)((this.position.Y + (float)(this.height / 2)) / 16f), 0.8f, 0.7f, 0.1f);
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
				if (this.type == 75)
				{
					if (Main.rand.Next(25) == 0)
					{
						Dust.NewDust(this.position, this.width, this.height, 58, this.velocity.X * 0.5f, this.velocity.Y * 0.5f, 150, default(Color), 1.2f);
					}
					if (Main.rand.Next(50) == 0)
					{
						Gore.NewGore(this.position, new Vector2(this.velocity.X * 0.2f, this.velocity.Y * 0.2f), Main.rand.Next(16, 18), 1f);
					}
				}
				if (this.spawnTime < 2147483646)
				{
					this.spawnTime++;
				}
				if (Main.netMode == 2 && this.owner != Main.myPlayer)
				{
					this.release++;
					if (this.release >= 300)
					{
						this.release = 0;
						NetMessage.SendData(39, this.owner, -1, "", i, 0f, 0f, 0f, 0);
					}
				}
				if (this.wet)
				{
					this.position += value;
				}
				else
				{
					this.position += this.velocity;
				}
				if (this.noGrabDelay > 0)
				{
					this.noGrabDelay--;
				}
			}
		}
		public static int NewItem(int X, int Y, int Width, int Height, int Type, int Stack = 1, bool noBroadcast = false, int pfix = 0)
		{
			if (Main.rand == null)
			{
				Main.rand = new Random();
			}
			if (WorldGen.gen)
			{
				return 0;
			}
			int num = 200;
			Main.item[200] = new Item();
			if (Main.netMode != 1)
			{
				for (int i = 0; i < 200; i++)
				{
					if (!Main.item[i].active)
					{
						num = i;
						break;
					}
				}
			}
			if (num == 200 && Main.netMode != 1)
			{
				int num2 = 0;
				for (int j = 0; j < 200; j++)
				{
					if (Main.item[j].spawnTime > num2)
					{
						num2 = Main.item[j].spawnTime;
						num = j;
					}
				}
			}
			Main.item[num] = new Item();
			Main.item[num].SetDefaults(Type, false);
			Main.item[num].Prefix(pfix);
			Main.item[num].position.X = (float)(X + Width / 2 - Main.item[num].width / 2);
			Main.item[num].position.Y = (float)(Y + Height / 2 - Main.item[num].height / 2);
			Main.item[num].wet = Collision.WetCollision(Main.item[num].position, Main.item[num].width, Main.item[num].height);
			Main.item[num].velocity.X = (float)Main.rand.Next(-30, 31) * 0.1f;
			Main.item[num].velocity.Y = (float)Main.rand.Next(-40, -15) * 0.1f;
			if (Type == 520 || Type == 521)
			{
				Main.item[num].velocity.X = (float)Main.rand.Next(-30, 31) * 0.1f;
				Main.item[num].velocity.Y = (float)Main.rand.Next(-30, 31) * 0.1f;
			}
			Main.item[num].active = true;
			Main.item[num].spawnTime = 0;
			Main.item[num].stack = Stack;
			if (Main.netMode == 2 && !noBroadcast)
			{
				NetMessage.SendData(21, -1, -1, "", num, 0f, 0f, 0f, 0);
				Main.item[num].FindOwner(num);
			}
			else
			{
				if (Main.netMode == 0)
				{
					Main.item[num].owner = Main.myPlayer;
				}
			}
			return num;
		}
		public void FindOwner(int whoAmI)
		{
			if (this.keepTime > 0)
			{
				return;
			}
			int num = this.owner;
			this.owner = 255;
			float num2 = -1f;
			for (int i = 0; i < 255; i++)
			{
				if (this.ownIgnore != i && Main.player[i].active && Main.player[i].ItemSpace(Main.item[whoAmI]))
				{
					float num3 = Math.Abs(Main.player[i].position.X + (float)(Main.player[i].width / 2) - this.position.X - (float)(this.width / 2)) + Math.Abs(Main.player[i].position.Y + (float)(Main.player[i].height / 2) - this.position.Y - (float)this.height);
					if (num3 < (float)NPC.sWidth && (num2 == -1f || num3 < num2))
					{
						num2 = num3;
						this.owner = i;
					}
				}
			}
			if (this.owner != num && ((num == Main.myPlayer && Main.netMode == 1) || (num == 255 && Main.netMode == 2) || !Main.player[num].active))
			{
				NetMessage.SendData(21, -1, -1, "", whoAmI, 0f, 0f, 0f, 0);
				if (this.active)
				{
					NetMessage.SendData(22, -1, -1, "", whoAmI, 0f, 0f, 0f, 0);
				}
			}
		}
		public object Clone()
		{
			return base.MemberwiseClone();
		}
		public bool IsTheSameAs(Item compareItem)
		{
			return this.name == compareItem.name;
		}
		public bool IsNotTheSameAs(Item compareItem)
		{
			return this.name != compareItem.name || this.stack != compareItem.stack || this.prefix != compareItem.prefix;
		}
	}
}
