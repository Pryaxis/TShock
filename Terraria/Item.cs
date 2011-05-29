namespace Terraria
{
    using Microsoft.Xna.Framework;
    using System;
    using System.Runtime.InteropServices;

    public class Item
    {
        public bool accessory;
        public bool active;
        public int alpha;
        public int ammo;
        public bool autoReuse;
        public int axe;
        public bool beingGrabbed;
        public int bodySlot = -1;
        public bool buy;
        public bool channel;
        public Color color;
        public bool consumable;
        public int createTile = -1;
        public int createWall = -1;
        public int damage;
        public int defense;
        public int hammer;
        public int headSlot = -1;
        public int healLife;
        public int healMana;
        public int height;
        public int holdStyle;
        public int keepTime;
        public float knockBack;
        public bool lavaWet;
        public int legSlot = -1;
        public int lifeRegen;
        public int mana;
        public int manaRegen;
        public int maxStack;
        public string name;
        public int noGrabDelay;
        public bool noMelee;
        public bool noUseGraphic;
        public int owner = 8;
        public int ownIgnore = -1;
        public int ownTime;
        public int pick;
        public Vector2 position;
        public bool potion;
        public static int potionDelay = 720;
        public int rare;
        public int release;
        public float scale = 1f;
        public int shoot;
        public float shootSpeed;
        public int spawnTime;
        public int stack;
        public int tileBoost;
        public string toolTip;
        public int type;
        public int useAmmo;
        public int useAnimation;
        public int useSound;
        public int useStyle;
        public int useTime;
        public bool useTurn;
        public int value;
        public Vector2 velocity;
        public bool wet;
        public byte wetCount;
        public int width;
        public bool wornArmor;

        public object Clone()
        {
            return base.MemberwiseClone();
        }

        public void FindOwner(int whoAmI)
        {
            if (this.keepTime <= 0)
            {
                int owner = this.owner;
                this.owner = 8;
                float num2 = -1f;
                for (int i = 0; i < 8; i++)
                {
                    if (((this.ownIgnore != i) && Main.player[i].active) && Main.player[i].ItemSpace(Main.item[whoAmI]))
                    {
                        float num4 = Math.Abs((float) (((Main.player[i].position.X + (Main.player[i].width / 2)) - this.position.X) - (this.width / 2))) + Math.Abs((float) (((Main.player[i].position.Y + (Main.player[i].height / 2)) - this.position.Y) - this.height));
                        if ((num4 < ((Main.screenWidth / 2) + (Main.screenHeight / 2))) && ((num2 == -1f) || (num4 < num2)))
                        {
                            num2 = num4;
                            this.owner = i;
                        }
                    }
                }
                if ((this.owner != owner) && ((((owner == Main.myPlayer) && (Main.netMode == 1)) || ((owner == 8) && (Main.netMode == 2))) || !Main.player[owner].active))
                {
                    NetMessage.SendData(0x15, -1, -1, "", whoAmI, 0f, 0f, 0f);
                    if (this.active)
                    {
                        NetMessage.SendData(0x16, -1, -1, "", whoAmI, 0f, 0f, 0f);
                    }
                }
            }
        }

        public Color GetAlpha(Color newColor)
        {
            int r = newColor.R - this.alpha;
            int g = newColor.G - this.alpha;
            int b = newColor.B - this.alpha;
            int a = newColor.A - this.alpha;
            if (a < 0)
            {
                a = 0;
            }
            if (a > 0xff)
            {
                a = 0xff;
            }
            if ((this.type >= 0xc6) && (this.type <= 0xcb))
            {
                return Color.White;
            }
            return new Color(r, g, b, a);
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

        public bool IsNotTheSameAs(Item compareItem)
        {
            if (!(this.name != compareItem.name) && (this.stack == compareItem.stack))
            {
                return false;
            }
            return true;
        }

        public bool IsTheSameAs(Item compareItem)
        {
            return (this.name == compareItem.name);
        }

        public static int NewItem(int X, int Y, int Width, int Height, int Type, int Stack = 1, bool noBroadcast = false)
        {
            if (WorldGen.gen)
            {
                return 0;
            }
            int index = 200;
            Main.item[200] = new Item();
            if (Main.netMode != 1)
            {
                for (int i = 0; i < 200; i++)
                {
                    if (!Main.item[i].active)
                    {
                        index = i;
                        break;
                    }
                }
            }
            if ((index == 200) && (Main.netMode != 1))
            {
                int spawnTime = 0;
                for (int j = 0; j < 200; j++)
                {
                    if (Main.item[j].spawnTime > spawnTime)
                    {
                        spawnTime = Main.item[j].spawnTime;
                        index = j;
                    }
                }
            }
            Main.item[index] = new Item();
            Main.item[index].SetDefaults(Type);
            Main.item[index].position.X = (X + (Width / 2)) - (Main.item[index].width / 2);
            Main.item[index].position.Y = (Y + (Height / 2)) - (Main.item[index].height / 2);
            Main.item[index].wet = Collision.WetCollision(Main.item[index].position, Main.item[index].width, Main.item[index].height);
            Main.item[index].velocity.X = Main.rand.Next(-20, 0x15) * 0.1f;
            Main.item[index].velocity.Y = Main.rand.Next(-30, -10) * 0.1f;
            Main.item[index].active = true;
            Main.item[index].spawnTime = 0;
            Main.item[index].stack = Stack;
            if ((Main.netMode == 2) && !noBroadcast)
            {
                NetMessage.SendData(0x15, -1, -1, "", index, 0f, 0f, 0f);
                Main.item[index].FindOwner(index);
                return index;
            }
            if (Main.netMode == 0)
            {
                Main.item[index].owner = Main.myPlayer;
            }
            return index;
        }

        public void SetDefaults(int Type)
        {
            if ((Main.netMode == 1) || (Main.netMode == 2))
            {
                this.owner = 8;
            }
            else
            {
                this.owner = Main.myPlayer;
            }
            this.mana = 0;
            this.wet = false;
            this.wetCount = 0;
            this.lavaWet = false;
            this.channel = false;
            this.manaRegen = 0;
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
            this.color = new Color();
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
            this.tileBoost = 0;
            this.type = Type;
            this.useStyle = 0;
            this.useSound = 0;
            this.useTime = 100;
            this.useAnimation = 100;
            this.value = 0;
            this.useTurn = false;
            this.buy = false;
            if (this.type == 1)
            {
                this.name = "Iron Pickaxe";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 20;
                this.useTime = 13;
                this.autoReuse = true;
                this.width = 0x18;
                this.height = 0x1c;
                this.damage = 5;
                this.pick = 0x2d;
                this.useSound = 1;
                this.knockBack = 2f;
                this.value = 0x7d0;
            }
            else if (this.type == 2)
            {
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
            }
            else if (this.type == 3)
            {
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
            }
            else if (this.type == 4)
            {
                this.name = "Iron Broadsword";
                this.useStyle = 1;
                this.useTurn = false;
                this.useAnimation = 0x15;
                this.useTime = 0x15;
                this.width = 0x18;
                this.height = 0x1c;
                this.damage = 10;
                this.knockBack = 5f;
                this.useSound = 1;
                this.scale = 1f;
                this.value = 0x708;
            }
            else if (this.type == 5)
            {
                this.name = "Mushroom";
                this.useStyle = 2;
                this.useSound = 2;
                this.useTurn = false;
                this.useAnimation = 0x11;
                this.useTime = 0x11;
                this.width = 0x10;
                this.height = 0x12;
                this.healLife = 20;
                this.maxStack = 0x63;
                this.consumable = true;
                this.potion = true;
                this.value = 50;
            }
            else if (this.type == 6)
            {
                this.name = "Iron Shortsword";
                this.useStyle = 3;
                this.useTurn = false;
                this.useAnimation = 12;
                this.useTime = 12;
                this.width = 0x18;
                this.height = 0x1c;
                this.damage = 8;
                this.knockBack = 4f;
                this.scale = 0.9f;
                this.useSound = 1;
                this.useTurn = true;
                this.value = 0x578;
            }
            else if (this.type == 7)
            {
                this.name = "Iron Hammer";
                this.autoReuse = true;
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 30;
                this.useTime = 20;
                this.hammer = 0x2d;
                this.width = 0x18;
                this.height = 0x1c;
                this.damage = 7;
                this.knockBack = 5.5f;
                this.scale = 1.2f;
                this.useSound = 1;
                this.value = 0x640;
            }
            else if (this.type == 8)
            {
                this.name = "Torch";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.holdStyle = 1;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 4;
                this.width = 10;
                this.height = 12;
                this.toolTip = "Provides light";
                this.value = 50;
            }
            else if (this.type == 9)
            {
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
            }
            else if (this.type == 10)
            {
                this.name = "Iron Axe";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 0x1b;
                this.knockBack = 4.5f;
                this.useTime = 0x13;
                this.autoReuse = true;
                this.width = 0x18;
                this.height = 0x1c;
                this.damage = 5;
                this.axe = 9;
                this.scale = 1.1f;
                this.useSound = 1;
                this.value = 0x640;
            }
            else if (this.type == 11)
            {
                this.name = "Iron Ore";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 6;
                this.width = 12;
                this.height = 12;
                this.value = 500;
            }
            else if (this.type == 12)
            {
                this.name = "Copper Ore";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 7;
                this.width = 12;
                this.height = 12;
                this.value = 250;
            }
            else if (this.type == 13)
            {
                this.name = "Gold Ore";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 8;
                this.width = 12;
                this.height = 12;
                this.value = 0x7d0;
            }
            else if (this.type == 14)
            {
                this.name = "Silver Ore";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 9;
                this.width = 12;
                this.height = 12;
                this.value = 0x3e8;
            }
            else if (this.type == 15)
            {
                this.name = "Copper Watch";
                this.width = 0x18;
                this.height = 0x1c;
                this.accessory = true;
                this.toolTip = "Tells the time";
                this.value = 0x3e8;
            }
            else if (this.type == 0x10)
            {
                this.name = "Silver Watch";
                this.width = 0x18;
                this.height = 0x1c;
                this.accessory = true;
                this.toolTip = "Tells the time";
                this.value = 0x1388;
            }
            else if (this.type == 0x11)
            {
                this.name = "Gold Watch";
                this.width = 0x18;
                this.height = 0x1c;
                this.accessory = true;
                this.rare = 1;
                this.toolTip = "Tells the time";
                this.value = 0x2710;
            }
            else if (this.type == 0x12)
            {
                this.name = "Depth Meter";
                this.width = 0x18;
                this.height = 0x12;
                this.accessory = true;
                this.rare = 1;
                this.toolTip = "Shows depth";
                this.value = 0x2710;
            }
            else if (this.type == 0x13)
            {
                this.name = "Gold Bar";
                this.width = 20;
                this.height = 20;
                this.maxStack = 0x63;
                this.value = 0x1770;
            }
            else if (this.type == 20)
            {
                this.name = "Copper Bar";
                this.width = 20;
                this.height = 20;
                this.maxStack = 0x63;
                this.value = 750;
            }
            else if (this.type == 0x15)
            {
                this.name = "Silver Bar";
                this.width = 20;
                this.height = 20;
                this.maxStack = 0x63;
                this.value = 0xbb8;
            }
            else if (this.type == 0x16)
            {
                this.name = "Iron Bar";
                this.width = 20;
                this.height = 20;
                this.maxStack = 0x63;
                this.value = 0x5dc;
            }
            else if (this.type == 0x17)
            {
                this.name = "Gel";
                this.width = 10;
                this.height = 12;
                this.maxStack = 0x63;
                this.alpha = 0xaf;
                this.color = new Color(0, 80, 0xff, 100);
                this.toolTip = "'Both tasty and flammable'";
                this.value = 5;
            }
            else if (this.type == 0x18)
            {
                this.name = "Wooden Sword";
                this.useStyle = 1;
                this.useTurn = false;
                this.useAnimation = 0x19;
                this.width = 0x18;
                this.height = 0x1c;
                this.damage = 7;
                this.knockBack = 4f;
                this.scale = 0.95f;
                this.useSound = 1;
                this.value = 100;
            }
            else if (this.type == 0x19)
            {
                this.name = "Wooden Door";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 10;
                this.width = 14;
                this.height = 0x1c;
                this.value = 200;
            }
            else if (this.type == 0x1a)
            {
                this.name = "Stone Wall";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createWall = 1;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0x1b)
            {
                this.name = "Acorn";
                this.useTurn = true;
                this.useStyle = 1;
                this.useAnimation = 15;
                this.useTime = 10;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 20;
                this.width = 0x12;
                this.height = 0x12;
                this.value = 10;
            }
            else if (this.type == 0x1c)
            {
                this.name = "Lesser Healing Potion";
                this.useSound = 3;
                this.healLife = 100;
                this.useStyle = 2;
                this.useTurn = true;
                this.useAnimation = 0x11;
                this.useTime = 0x11;
                this.maxStack = 30;
                this.consumable = true;
                this.width = 14;
                this.height = 0x18;
                this.potion = true;
                this.value = 200;
            }
            else if (this.type == 0x1d)
            {
                this.name = "Life Crystal";
                this.maxStack = 0x63;
                this.consumable = true;
                this.width = 0x12;
                this.height = 0x12;
                this.useStyle = 4;
                this.useTime = 30;
                this.useSound = 4;
                this.useAnimation = 30;
                this.toolTip = "Increases maximum life";
                this.rare = 2;
            }
            else if (this.type == 30)
            {
                this.name = "Dirt Wall";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createWall = 2;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0x1f)
            {
                this.name = "Bottle";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 13;
                this.width = 0x10;
                this.height = 0x18;
                this.value = 100;
            }
            else if (this.type == 0x20)
            {
                this.name = "Wooden Table";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 14;
                this.width = 0x1a;
                this.height = 20;
                this.value = 300;
            }
            else if (this.type == 0x21)
            {
                this.name = "Furnace";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 0x11;
                this.width = 0x1a;
                this.height = 0x18;
                this.value = 300;
            }
            else if (this.type == 0x22)
            {
                this.name = "Wooden Chair";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 15;
                this.width = 12;
                this.height = 30;
                this.value = 150;
            }
            else if (this.type == 0x23)
            {
                this.name = "Iron Anvil";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 0x10;
                this.width = 0x1c;
                this.height = 14;
                this.value = 0x1388;
            }
            else if (this.type == 0x24)
            {
                this.name = "Work Bench";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 0x12;
                this.width = 0x1c;
                this.height = 14;
                this.value = 150;
            }
            else if (this.type == 0x25)
            {
                this.name = "Goggles";
                this.width = 0x1c;
                this.height = 12;
                this.defense = 1;
                this.headSlot = 10;
                this.rare = 1;
                this.value = 0x3e8;
            }
            else if (this.type == 0x26)
            {
                this.name = "Lens";
                this.width = 12;
                this.height = 20;
                this.maxStack = 0x63;
                this.value = 500;
            }
            else if (this.type == 0x27)
            {
                this.useStyle = 5;
                this.useAnimation = 30;
                this.useTime = 30;
                this.name = "Wooden Bow";
                this.width = 12;
                this.height = 0x1c;
                this.shoot = 1;
                this.useAmmo = 1;
                this.useSound = 5;
                this.damage = 5;
                this.shootSpeed = 6.1f;
                this.noMelee = true;
                this.value = 100;
            }
            else if (this.type == 40)
            {
                this.name = "Wooden Arrow";
                this.shootSpeed = 3f;
                this.shoot = 1;
                this.damage = 5;
                this.width = 10;
                this.height = 0x1c;
                this.maxStack = 250;
                this.consumable = true;
                this.ammo = 1;
                this.knockBack = 2f;
                this.value = 10;
            }
            else if (this.type == 0x29)
            {
                this.name = "Flaming Arrow";
                this.shootSpeed = 3.5f;
                this.shoot = 2;
                this.damage = 7;
                this.width = 10;
                this.height = 0x1c;
                this.maxStack = 250;
                this.consumable = true;
                this.ammo = 1;
                this.knockBack = 2f;
                this.value = 15;
            }
            else if (this.type == 0x2a)
            {
                this.useStyle = 1;
                this.name = "Shuriken";
                this.shootSpeed = 9f;
                this.shoot = 3;
                this.damage = 10;
                this.width = 0x12;
                this.height = 20;
                this.maxStack = 250;
                this.consumable = true;
                this.useSound = 1;
                this.useAnimation = 15;
                this.useTime = 15;
                this.noUseGraphic = true;
                this.noMelee = true;
                this.value = 20;
            }
            else if (this.type == 0x2b)
            {
                this.useStyle = 4;
                this.name = "Suspicious Looking Eye";
                this.width = 0x16;
                this.height = 14;
                this.consumable = true;
                this.useAnimation = 0x2d;
                this.useTime = 0x2d;
                this.toolTip = "May cause terrible things to occur";
            }
            else if (this.type == 0x2c)
            {
                this.useStyle = 5;
                this.useAnimation = 0x19;
                this.useTime = 0x19;
                this.name = "Demon Bow";
                this.width = 12;
                this.height = 0x1c;
                this.shoot = 1;
                this.useAmmo = 1;
                this.useSound = 5;
                this.damage = 13;
                this.shootSpeed = 6.7f;
                this.knockBack = 1f;
                this.alpha = 30;
                this.rare = 1;
                this.noMelee = true;
                this.value = 0x4650;
            }
            else if (this.type == 0x2d)
            {
                this.name = "War Axe of the Night";
                this.autoReuse = true;
                this.useStyle = 1;
                this.useAnimation = 30;
                this.knockBack = 6f;
                this.useTime = 15;
                this.width = 0x18;
                this.height = 0x1c;
                this.damage = 0x15;
                this.axe = 15;
                this.scale = 1.2f;
                this.useSound = 1;
                this.rare = 1;
                this.value = 0x34bc;
            }
            else if (this.type == 0x2e)
            {
                this.name = "Light's Bane";
                this.useStyle = 1;
                this.useAnimation = 20;
                this.knockBack = 5f;
                this.width = 0x18;
                this.height = 0x1c;
                this.damage = 0x10;
                this.scale = 1.1f;
                this.useSound = 1;
                this.rare = 1;
                this.value = 0x34bc;
            }
            else if (this.type == 0x2f)
            {
                this.name = "Unholy Arrow";
                this.shootSpeed = 3.4f;
                this.shoot = 4;
                this.damage = 8;
                this.width = 10;
                this.height = 0x1c;
                this.maxStack = 250;
                this.consumable = true;
                this.ammo = 1;
                this.knockBack = 3f;
                this.alpha = 30;
                this.rare = 1;
                this.value = 40;
            }
            else if (this.type == 0x30)
            {
                this.name = "Chest";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 0x15;
                this.width = 0x1a;
                this.height = 0x16;
                this.value = 500;
            }
            else if (this.type == 0x31)
            {
                this.name = "Band of Regeneration";
                this.width = 0x16;
                this.height = 0x16;
                this.accessory = true;
                this.lifeRegen = 1;
                this.rare = 1;
                this.toolTip = "Slowly regenerates life";
                this.value = 0xc350;
            }
            else if (this.type == 50)
            {
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
                this.value = 0xc350;
            }
            else if (this.type == 0x33)
            {
                this.name = "Jester's Arrow";
                this.shootSpeed = 0.5f;
                this.shoot = 5;
                this.damage = 9;
                this.width = 10;
                this.height = 0x1c;
                this.maxStack = 250;
                this.consumable = true;
                this.ammo = 1;
                this.knockBack = 4f;
                this.rare = 1;
                this.value = 100;
            }
            else if (this.type == 0x34)
            {
                this.name = "Angel Statue";
                this.width = 0x18;
                this.height = 0x1c;
                this.toolTip = "It doesn't do anything";
                this.value = 1;
            }
            else if (this.type == 0x35)
            {
                this.name = "Cloud in a Bottle";
                this.width = 0x10;
                this.height = 0x18;
                this.accessory = true;
                this.rare = 1;
                this.toolTip = "Allows the holder to double jump";
                this.value = 0xc350;
            }
            else if (this.type == 0x36)
            {
                this.name = "Hermes Boots";
                this.width = 0x1c;
                this.height = 0x18;
                this.accessory = true;
                this.rare = 1;
                this.toolTip = "The wearer can run super fast";
                this.value = 0xc350;
            }
            else if (this.type == 0x37)
            {
                this.noMelee = true;
                this.useStyle = 1;
                this.name = "Enchanted Boomerang";
                this.shootSpeed = 10f;
                this.shoot = 6;
                this.damage = 13;
                this.knockBack = 8f;
                this.width = 14;
                this.height = 0x1c;
                this.useSound = 1;
                this.useAnimation = 15;
                this.useTime = 15;
                this.noUseGraphic = true;
                this.rare = 1;
                this.value = 0xc350;
            }
            else if (this.type == 0x38)
            {
                this.name = "Demonite Ore";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 0x16;
                this.width = 12;
                this.height = 12;
                this.rare = 1;
                this.toolTip = "Pulsing with dark energy";
                this.value = 0xfa0;
            }
            else if (this.type == 0x39)
            {
                this.name = "Demonite Bar";
                this.width = 20;
                this.height = 20;
                this.maxStack = 0x63;
                this.rare = 1;
                this.toolTip = "Pulsing with dark energy";
                this.value = 0x3e80;
            }
            else if (this.type == 0x3a)
            {
                this.name = "Heart";
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0x3b)
            {
                this.name = "Corrupt Seeds";
                this.useTurn = true;
                this.useStyle = 1;
                this.useAnimation = 15;
                this.useTime = 10;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 0x17;
                this.width = 14;
                this.height = 14;
                this.value = 500;
            }
            else if (this.type == 60)
            {
                this.name = "Vile Mushroom";
                this.width = 0x10;
                this.height = 0x12;
                this.maxStack = 0x63;
                this.value = 50;
            }
            else if (this.type == 0x3d)
            {
                this.name = "Ebonstone Block";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x19;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0x3e)
            {
                this.name = "Grass Seeds";
                this.useTurn = true;
                this.useStyle = 1;
                this.useAnimation = 15;
                this.useTime = 10;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 2;
                this.width = 14;
                this.height = 14;
                this.value = 20;
            }
            else if (this.type == 0x3f)
            {
                this.name = "Sunflower";
                this.useTurn = true;
                this.useStyle = 1;
                this.useAnimation = 15;
                this.useTime = 10;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 0x1b;
                this.width = 0x1a;
                this.height = 0x1a;
                this.value = 200;
            }
            else if (this.type == 0x40)
            {
                this.mana = 5;
                this.damage = 8;
                this.useStyle = 1;
                this.name = "Vilethorn";
                this.shootSpeed = 32f;
                this.shoot = 7;
                this.width = 0x1a;
                this.height = 0x1c;
                this.useSound = 8;
                this.useAnimation = 30;
                this.useTime = 30;
                this.rare = 1;
                this.noMelee = true;
                this.toolTip = "Summons a vile thorn";
                this.value = 0x2710;
            }
            else if (this.type == 0x41)
            {
                this.mana = 11;
                this.knockBack = 5f;
                this.alpha = 100;
                this.color = new Color(150, 150, 150, 0);
                this.damage = 15;
                this.useStyle = 1;
                this.scale = 1.15f;
                this.name = "Starfury";
                this.shootSpeed = 12f;
                this.shoot = 9;
                this.width = 14;
                this.height = 0x1c;
                this.useSound = 9;
                this.useAnimation = 0x19;
                this.useTime = 10;
                this.rare = 1;
                this.toolTip = "Forged with the fury of heaven";
                this.value = 0xc350;
            }
            else if (this.type == 0x42)
            {
                this.useStyle = 1;
                this.name = "Purification Powder";
                this.shootSpeed = 4f;
                this.shoot = 10;
                this.width = 0x10;
                this.height = 0x18;
                this.maxStack = 0x63;
                this.consumable = true;
                this.useSound = 1;
                this.useAnimation = 15;
                this.useTime = 15;
                this.noMelee = true;
                this.toolTip = "Cleanses the corruption";
                this.value = 0x4b;
            }
            else if (this.type == 0x43)
            {
                this.damage = 8;
                this.useStyle = 1;
                this.name = "Vile Powder";
                this.shootSpeed = 4f;
                this.shoot = 11;
                this.width = 0x10;
                this.height = 0x18;
                this.maxStack = 0x63;
                this.consumable = true;
                this.useSound = 1;
                this.useAnimation = 15;
                this.useTime = 15;
                this.noMelee = true;
                this.value = 100;
            }
            else if (this.type == 0x44)
            {
                this.name = "Rotten Chunk";
                this.width = 0x12;
                this.height = 20;
                this.maxStack = 0x63;
                this.toolTip = "Looks tasty!";
                this.value = 10;
            }
            else if (this.type == 0x45)
            {
                this.name = "Worm Tooth";
                this.width = 8;
                this.height = 20;
                this.maxStack = 0x63;
                this.value = 100;
            }
            else if (this.type == 70)
            {
                this.useStyle = 4;
                this.consumable = true;
                this.useAnimation = 0x2d;
                this.useTime = 0x2d;
                this.name = "Worm Food";
                this.width = 0x1c;
                this.height = 0x1c;
                this.toolTip = "May attract giant worms";
            }
            else if (this.type == 0x47)
            {
                this.name = "Copper Coin";
                this.width = 10;
                this.height = 12;
                this.maxStack = 100;
            }
            else if (this.type == 0x48)
            {
                this.name = "Silver Coin";
                this.width = 10;
                this.height = 12;
                this.maxStack = 100;
            }
            else if (this.type == 0x49)
            {
                this.name = "Gold Coin";
                this.width = 10;
                this.height = 12;
                this.maxStack = 100;
            }
            else if (this.type == 0x4a)
            {
                this.name = "Platinum Coin";
                this.width = 10;
                this.height = 12;
                this.maxStack = 100;
            }
            else if (this.type == 0x4b)
            {
                this.name = "Fallen Star";
                this.width = 0x12;
                this.height = 20;
                this.maxStack = 100;
                this.alpha = 0x4b;
                this.ammo = 15;
                this.toolTip = "Disappears after the sunrise";
                this.value = 500;
                this.useStyle = 4;
                this.useSound = 4;
                this.useTurn = false;
                this.useAnimation = 0x11;
                this.useTime = 0x11;
                this.healMana = 20;
                this.consumable = true;
                this.rare = 1;
                this.potion = true;
            }
            else if (this.type == 0x4c)
            {
                this.name = "Copper Greaves";
                this.width = 0x12;
                this.height = 0x1c;
                this.defense = 1;
                this.legSlot = 1;
                this.value = 750;
            }
            else if (this.type == 0x4d)
            {
                this.name = "Iron Greaves";
                this.width = 0x12;
                this.height = 0x1c;
                this.defense = 2;
                this.legSlot = 2;
                this.value = 0xbb8;
            }
            else if (this.type == 0x4e)
            {
                this.name = "Silver Greaves";
                this.width = 0x12;
                this.height = 0x1c;
                this.defense = 3;
                this.legSlot = 3;
                this.value = 0x1d4c;
            }
            else if (this.type == 0x4f)
            {
                this.name = "Gold Greaves";
                this.width = 0x12;
                this.height = 0x1c;
                this.defense = 4;
                this.legSlot = 4;
                this.value = 0x3a98;
            }
            else if (this.type == 80)
            {
                this.name = "Copper Chainmail";
                this.width = 0x1a;
                this.height = 0x1c;
                this.defense = 2;
                this.bodySlot = 1;
                this.value = 0x3e8;
            }
            else if (this.type == 0x51)
            {
                this.name = "Iron Chainmail";
                this.width = 0x1a;
                this.height = 0x1c;
                this.defense = 3;
                this.bodySlot = 2;
                this.value = 0xfa0;
            }
            else if (this.type == 0x52)
            {
                this.name = "Silver Chainmail";
                this.width = 0x1a;
                this.height = 0x1c;
                this.defense = 4;
                this.bodySlot = 3;
                this.value = 0x2710;
            }
            else if (this.type == 0x53)
            {
                this.name = "Gold Chainmail";
                this.width = 0x1a;
                this.height = 0x1c;
                this.defense = 5;
                this.bodySlot = 4;
                this.value = 0x4e20;
            }
            else if (this.type == 0x54)
            {
                this.noUseGraphic = true;
                this.damage = 0;
                this.knockBack = 7f;
                this.useStyle = 5;
                this.name = "Grappling Hook";
                this.shootSpeed = 11f;
                this.shoot = 13;
                this.width = 0x12;
                this.height = 0x1c;
                this.useSound = 1;
                this.useAnimation = 20;
                this.useTime = 20;
                this.rare = 1;
                this.noMelee = true;
                this.value = 0x4e20;
            }
            else if (this.type == 0x55)
            {
                this.name = "Iron Chain";
                this.width = 14;
                this.height = 20;
                this.maxStack = 0x63;
                this.value = 0x3e8;
            }
            else if (this.type == 0x56)
            {
                this.name = "Shadow Scale";
                this.width = 14;
                this.height = 0x12;
                this.maxStack = 0x63;
                this.rare = 1;
                this.value = 500;
            }
            else if (this.type == 0x57)
            {
                this.name = "Piggy Bank";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 0x1d;
                this.width = 20;
                this.height = 12;
                this.value = 0x2710;
            }
            else if (this.type == 0x58)
            {
                this.name = "Mining Helmet";
                this.width = 0x16;
                this.height = 0x10;
                this.defense = 1;
                this.headSlot = 11;
                this.rare = 1;
                this.value = 0x13880;
                this.toolTip = "Provides light when worn";
            }
            else if (this.type == 0x59)
            {
                this.name = "Copper Helmet";
                this.width = 0x16;
                this.height = 0x16;
                this.defense = 1;
                this.headSlot = 1;
                this.value = 0x4e2;
            }
            else if (this.type == 90)
            {
                this.name = "Iron Helmet";
                this.width = 0x16;
                this.height = 0x16;
                this.defense = 2;
                this.headSlot = 2;
                this.value = 0x1388;
            }
            else if (this.type == 0x5b)
            {
                this.name = "Silver Helmet";
                this.width = 0x16;
                this.height = 0x16;
                this.defense = 3;
                this.headSlot = 3;
                this.value = 0x30d4;
            }
            else if (this.type == 0x5c)
            {
                this.name = "Gold Helmet";
                this.width = 0x16;
                this.height = 0x16;
                this.defense = 4;
                this.headSlot = 4;
                this.value = 0x61a8;
            }
            else if (this.type == 0x5d)
            {
                this.name = "Wood Wall";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createWall = 4;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0x5e)
            {
                this.name = "Wood Platform";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 0x13;
                this.width = 8;
                this.height = 10;
            }
            else if (this.type == 0x5f)
            {
                this.useStyle = 5;
                this.useAnimation = 20;
                this.useTime = 20;
                this.name = "Flintlock Pistol";
                this.width = 0x18;
                this.height = 0x1c;
                this.shoot = 14;
                this.useAmmo = 14;
                this.useSound = 11;
                this.damage = 7;
                this.shootSpeed = 5f;
                this.noMelee = true;
                this.value = 0xc350;
                this.scale = 0.9f;
                this.rare = 1;
            }
            else if (this.type == 0x60)
            {
                this.useStyle = 5;
                this.autoReuse = true;
                this.useAnimation = 0x2d;
                this.useTime = 0x2d;
                this.name = "Musket";
                this.width = 0x2c;
                this.height = 14;
                this.shoot = 10;
                this.useAmmo = 14;
                this.useSound = 11;
                this.damage = 14;
                this.shootSpeed = 8f;
                this.noMelee = true;
                this.value = 0x186a0;
                this.knockBack = 4f;
                this.rare = 1;
            }
            else if (this.type == 0x61)
            {
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
                this.value = 8;
            }
            else if (this.type == 0x62)
            {
                this.useStyle = 5;
                this.autoReuse = true;
                this.useAnimation = 8;
                this.useTime = 8;
                this.name = "Minishark";
                this.width = 50;
                this.height = 0x12;
                this.shoot = 10;
                this.useAmmo = 14;
                this.useSound = 11;
                this.damage = 5;
                this.shootSpeed = 7f;
                this.noMelee = true;
                this.value = 0x7a120;
                this.rare = 2;
                this.toolTip = "Half shark, half gun, completely awesome.";
            }
            else if (this.type == 0x63)
            {
                this.useStyle = 5;
                this.useAnimation = 0x1c;
                this.useTime = 0x1c;
                this.name = "Iron Bow";
                this.width = 12;
                this.height = 0x1c;
                this.shoot = 1;
                this.useAmmo = 1;
                this.useSound = 5;
                this.damage = 9;
                this.shootSpeed = 6.6f;
                this.noMelee = true;
                this.value = 0x578;
            }
            else if (this.type == 100)
            {
                this.name = "Shadow Greaves";
                this.width = 0x12;
                this.height = 0x1c;
                this.defense = 6;
                this.legSlot = 5;
                this.rare = 1;
                this.value = 0x57e4;
            }
            else if (this.type == 0x65)
            {
                this.name = "Shadow Scalemail";
                this.width = 0x1a;
                this.height = 0x1c;
                this.defense = 7;
                this.bodySlot = 5;
                this.rare = 1;
                this.value = 0x7530;
            }
            else if (this.type == 0x66)
            {
                this.name = "Shadow Helmet";
                this.width = 0x16;
                this.height = 0x16;
                this.defense = 6;
                this.headSlot = 5;
                this.rare = 1;
                this.value = 0x927c;
            }
            else if (this.type == 0x67)
            {
                this.name = "Nightmare Pickaxe";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 20;
                this.useTime = 15;
                this.autoReuse = true;
                this.width = 0x18;
                this.height = 0x1c;
                this.damage = 11;
                this.pick = 0x41;
                this.useSound = 1;
                this.knockBack = 3f;
                this.rare = 1;
                this.value = 0x4650;
                this.scale = 1.15f;
            }
            else if (this.type == 0x68)
            {
                this.name = "The Breaker";
                this.autoReuse = true;
                this.useStyle = 1;
                this.useAnimation = 40;
                this.useTime = 0x13;
                this.hammer = 0x37;
                this.width = 0x18;
                this.height = 0x1c;
                this.damage = 0x1c;
                this.knockBack = 6.5f;
                this.scale = 1.3f;
                this.useSound = 1;
                this.rare = 1;
                this.value = 0x3a98;
            }
            else if (this.type == 0x69)
            {
                this.name = "Candle";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 0x21;
                this.width = 8;
                this.height = 0x12;
                this.holdStyle = 1;
            }
            else if (this.type == 0x6a)
            {
                this.name = "Copper Chandelier";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 0x22;
                this.width = 0x1a;
                this.height = 0x1a;
            }
            else if (this.type == 0x6b)
            {
                this.name = "Silver Chandelier";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 0x23;
                this.width = 0x1a;
                this.height = 0x1a;
            }
            else if (this.type == 0x6c)
            {
                this.name = "Gold Chandelier";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 0x24;
                this.width = 0x1a;
                this.height = 0x1a;
            }
            else if (this.type == 0x6d)
            {
                this.name = "Mana Crystal";
                this.maxStack = 0x63;
                this.consumable = true;
                this.width = 0x12;
                this.height = 0x12;
                this.useStyle = 4;
                this.useTime = 30;
                this.useSound = 4;
                this.useAnimation = 30;
                this.toolTip = "Increases maximum mana";
                this.rare = 2;
            }
            else if (this.type == 110)
            {
                this.name = "Lesser Mana Potion";
                this.useSound = 3;
                this.healMana = 100;
                this.useStyle = 2;
                this.useTurn = true;
                this.useAnimation = 0x11;
                this.useTime = 0x11;
                this.maxStack = 30;
                this.consumable = true;
                this.width = 14;
                this.height = 0x18;
                this.potion = true;
                this.value = 0x3e8;
            }
            else if (this.type == 0x6f)
            {
                this.name = "Band of Starpower";
                this.width = 0x16;
                this.height = 0x16;
                this.accessory = true;
                this.manaRegen = 3;
                this.rare = 1;
                this.toolTip = "Slowly regenerates mana";
                this.value = 0xc350;
            }
            else if (this.type == 0x70)
            {
                this.mana = 10;
                this.damage = 30;
                this.useStyle = 1;
                this.name = "Flower of Fire";
                this.shootSpeed = 6f;
                this.shoot = 15;
                this.width = 0x1a;
                this.height = 0x1c;
                this.useSound = 8;
                this.useAnimation = 30;
                this.useTime = 30;
                this.rare = 3;
                this.noMelee = true;
                this.knockBack = 5f;
                this.toolTip = "Throws balls of fire";
                this.value = 0x2710;
            }
            else if (this.type == 0x71)
            {
                this.mana = 0x12;
                this.channel = true;
                this.damage = 30;
                this.useStyle = 1;
                this.name = "Magic Missile";
                this.shootSpeed = 6f;
                this.shoot = 0x10;
                this.width = 0x1a;
                this.height = 0x1c;
                this.useSound = 9;
                this.useAnimation = 20;
                this.useTime = 20;
                this.rare = 2;
                this.noMelee = true;
                this.knockBack = 5f;
                this.toolTip = "Casts a controllable missile";
                this.value = 0x2710;
            }
            else if (this.type == 0x72)
            {
                this.mana = 5;
                this.channel = true;
                this.damage = 0;
                this.useStyle = 1;
                this.name = "Dirt Rod";
                this.shoot = 0x11;
                this.width = 0x1a;
                this.height = 0x1c;
                this.useSound = 8;
                this.useAnimation = 20;
                this.useTime = 20;
                this.rare = 1;
                this.noMelee = true;
                this.knockBack = 5f;
                this.toolTip = "Magically move dirt";
                this.value = 0x30d40;
            }
            else if (this.type == 0x73)
            {
                this.mana = 40;
                this.channel = true;
                this.damage = 0;
                this.useStyle = 4;
                this.name = "Orb of Light";
                this.shoot = 0x12;
                this.width = 0x18;
                this.height = 0x18;
                this.useSound = 8;
                this.useAnimation = 20;
                this.useTime = 20;
                this.rare = 1;
                this.noMelee = true;
                this.toolTip = "Creates a magical orb of light";
                this.value = 0x2710;
            }
            else if (this.type == 0x74)
            {
                this.name = "Meteorite";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x25;
                this.width = 12;
                this.height = 12;
                this.value = 0x3e8;
            }
            else if (this.type == 0x75)
            {
                this.name = "Meteorite Bar";
                this.width = 20;
                this.height = 20;
                this.maxStack = 0x63;
                this.rare = 1;
                this.toolTip = "Warm to the touch";
                this.value = 0x1b58;
            }
            else if (this.type == 0x76)
            {
                this.name = "Hook";
                this.maxStack = 0x63;
                this.width = 0x12;
                this.height = 0x12;
                this.value = 0x3e8;
                this.toolTip = "Combine with chains to making a grappling hook";
            }
            else if (this.type == 0x77)
            {
                this.noMelee = true;
                this.useStyle = 1;
                this.name = "Flamarang";
                this.shootSpeed = 11f;
                this.shoot = 0x13;
                this.damage = 0x20;
                this.knockBack = 8f;
                this.width = 14;
                this.height = 0x1c;
                this.useSound = 1;
                this.useAnimation = 15;
                this.useTime = 15;
                this.noUseGraphic = true;
                this.rare = 3;
                this.value = 0x186a0;
            }
            else if (this.type == 120)
            {
                this.useStyle = 5;
                this.useAnimation = 0x19;
                this.useTime = 0x19;
                this.name = "Molten Fury";
                this.width = 14;
                this.height = 0x20;
                this.shoot = 1;
                this.useAmmo = 1;
                this.useSound = 5;
                this.damage = 0x1d;
                this.shootSpeed = 8f;
                this.knockBack = 2f;
                this.alpha = 30;
                this.rare = 3;
                this.noMelee = true;
                this.scale = 1.1f;
                this.value = 0x6978;
                this.toolTip = "Lights wooden arrows ablaze";
            }
            else if (this.type == 0x79)
            {
                this.name = "Fiery Greatsword";
                this.useStyle = 1;
                this.useAnimation = 0x23;
                this.knockBack = 6.5f;
                this.width = 0x18;
                this.height = 0x1c;
                this.damage = 0x22;
                this.scale = 1.3f;
                this.useSound = 1;
                this.rare = 3;
                this.value = 0x6978;
                this.toolTip = "It's made out of fire!";
            }
            if (this.type == 0x7a)
            {
                this.name = "Molten Pickaxe";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 0x19;
                this.useTime = 0x19;
                this.autoReuse = true;
                this.width = 0x18;
                this.height = 0x1c;
                this.damage = 0x12;
                this.pick = 100;
                this.scale = 1.15f;
                this.useSound = 1;
                this.knockBack = 2f;
                this.rare = 3;
                this.value = 0x6978;
            }
            else if (this.type == 0x7b)
            {
                this.name = "Meteor Helmet";
                this.width = 0x16;
                this.height = 0x16;
                this.defense = 4;
                this.headSlot = 6;
                this.rare = 1;
                this.value = 0xafc8;
                this.manaRegen = 3;
                this.toolTip = "Slowly regenerates mana";
            }
            else if (this.type == 0x7c)
            {
                this.name = "Meteor Suit";
                this.width = 0x1a;
                this.height = 0x1c;
                this.defense = 5;
                this.bodySlot = 6;
                this.rare = 1;
                this.value = 0x7530;
                this.manaRegen = 3;
                this.toolTip = "Slowly regenerates mana";
            }
            else if (this.type == 0x7d)
            {
                this.name = "Meteor Leggings";
                this.width = 0x12;
                this.height = 0x1c;
                this.defense = 4;
                this.legSlot = 6;
                this.rare = 1;
                this.manaRegen = 3;
                this.value = 0x7530;
                this.toolTip = "Slowly regenerates mana";
            }
            else if (this.type == 0x7e)
            {
                this.name = "Angel Statue";
                this.width = 0x18;
                this.height = 0x1c;
                this.toolTip = "It doesn't do anything";
                this.value = 1;
            }
            else if (this.type == 0x7f)
            {
                this.autoReuse = true;
                this.useStyle = 5;
                this.useAnimation = 0x12;
                this.useTime = 0x12;
                this.name = "Space Gun";
                this.width = 0x18;
                this.height = 0x1c;
                this.shoot = 20;
                this.mana = 9;
                this.useSound = 12;
                this.knockBack = 1f;
                this.damage = 15;
                this.shootSpeed = 10f;
                this.noMelee = true;
                this.scale = 0.8f;
                this.rare = 1;
            }
            else if (this.type == 0x80)
            {
                this.mana = 7;
                this.name = "Rocket Boots";
                this.width = 0x1c;
                this.height = 0x18;
                this.accessory = true;
                this.rare = 3;
                this.toolTip = "Allows flight";
                this.value = 0xc350;
            }
            else if (this.type == 0x81)
            {
                this.name = "Gray Brick";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x26;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 130)
            {
                this.name = "Gray Brick Wall";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createWall = 5;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0x83)
            {
                this.name = "Red Brick";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x27;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0x84)
            {
                this.name = "Red Brick Wall";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createWall = 6;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0x85)
            {
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
            }
            else if (this.type == 0x86)
            {
                this.name = "Blue Brick";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x29;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0x87)
            {
                this.name = "Blue Brick Wall";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createWall = 7;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0x88)
            {
                this.name = "Chain Lantern";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x2a;
                this.width = 12;
                this.height = 0x1c;
            }
            else if (this.type == 0x89)
            {
                this.name = "Green Brick";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x2b;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0x8a)
            {
                this.name = "Green Brick Wall";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createWall = 8;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0x8b)
            {
                this.name = "Pink Brick";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x2c;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 140)
            {
                this.name = "Pink Brick Wall";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createWall = 9;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0x8d)
            {
                this.name = "Gold Brick";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x2d;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0x8e)
            {
                this.name = "Gold Brick Wall";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createWall = 10;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0x8f)
            {
                this.name = "Silver Brick";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x2e;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0x90)
            {
                this.name = "Silver Brick Wall";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createWall = 11;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0x91)
            {
                this.name = "Copper Brick";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x2f;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0x92)
            {
                this.name = "Copper Brick Wall";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createWall = 12;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0x93)
            {
                this.name = "Spike";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x30;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0x94)
            {
                this.name = "Water Candle";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 0x31;
                this.width = 8;
                this.height = 0x12;
                this.holdStyle = 1;
                this.toolTip = "Holding this may attract unwanted attention";
            }
            else if (this.type == 0x95)
            {
                this.name = "Book";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 50;
                this.width = 0x18;
                this.height = 0x1c;
                this.toolTip = "It contains strange symbols";
            }
            else if (this.type == 150)
            {
                this.name = "Cobweb";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x33;
                this.width = 20;
                this.height = 0x18;
                this.alpha = 100;
            }
            else if (this.type == 0x97)
            {
                this.name = "Necro Helmet";
                this.width = 0x16;
                this.height = 0x16;
                this.defense = 6;
                this.headSlot = 7;
                this.rare = 2;
                this.value = 0xafc8;
            }
            else if (this.type == 0x98)
            {
                this.name = "Necro Breastplate";
                this.width = 0x1a;
                this.height = 0x1c;
                this.defense = 7;
                this.bodySlot = 7;
                this.rare = 2;
                this.value = 0x7530;
            }
            else if (this.type == 0x99)
            {
                this.name = "Necro Greaves";
                this.width = 0x12;
                this.height = 0x1c;
                this.defense = 6;
                this.legSlot = 7;
                this.rare = 2;
                this.value = 0x7530;
            }
            else if (this.type == 0x9a)
            {
                this.name = "Bone";
                this.maxStack = 0x63;
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
                this.damage = 0x16;
                this.knockBack = 4f;
                this.shoot = 0x15;
            }
            else if (this.type == 0x9b)
            {
                this.autoReuse = true;
                this.useTurn = true;
                this.name = "Muramasa";
                this.useStyle = 1;
                this.useAnimation = 20;
                this.knockBack = 3f;
                this.width = 40;
                this.height = 40;
                this.damage = 0x16;
                this.scale = 1.2f;
                this.useSound = 1;
                this.rare = 2;
                this.value = 0x6978;
            }
            else if (this.type == 0x9c)
            {
                this.name = "Cobalt Shield";
                this.width = 0x18;
                this.height = 0x1c;
                this.rare = 2;
                this.value = 0x6978;
                this.accessory = true;
                this.defense = 2;
                this.toolTip = "Grants immunity to knockback";
            }
            else if (this.type == 0x9d)
            {
                this.mana = 12;
                this.autoReuse = true;
                this.name = "Aqua Scepter";
                this.useStyle = 5;
                this.useAnimation = 30;
                this.useTime = 5;
                this.knockBack = 3f;
                this.width = 0x26;
                this.height = 10;
                this.damage = 15;
                this.scale = 1f;
                this.shoot = 0x16;
                this.shootSpeed = 10f;
                this.useSound = 13;
                this.rare = 2;
                this.value = 0x6978;
                this.toolTip = "Sprays out a shower of water";
            }
            else if (this.type == 0x9e)
            {
                this.name = "Lucky Horseshoe";
                this.width = 20;
                this.height = 0x16;
                this.rare = 1;
                this.value = 0x6978;
                this.accessory = true;
                this.toolTip = "Negate fall damage";
            }
            else if (this.type == 0x9f)
            {
                this.name = "Shiny Red Balloon";
                this.width = 14;
                this.height = 0x1c;
                this.rare = 1;
                this.value = 0x6978;
                this.accessory = true;
                this.toolTip = "Increases jump height";
            }
            else if (this.type == 160)
            {
                this.autoReuse = true;
                this.name = "Harpoon";
                this.useStyle = 5;
                this.useAnimation = 30;
                this.useTime = 30;
                this.knockBack = 6f;
                this.width = 30;
                this.height = 10;
                this.damage = 15;
                this.scale = 1.1f;
                this.shoot = 0x17;
                this.shootSpeed = 10f;
                this.useSound = 10;
                this.rare = 2;
                this.value = 0x6978;
            }
            else if (this.type == 0xa1)
            {
                this.useStyle = 1;
                this.name = "Spiky Ball";
                this.shootSpeed = 5f;
                this.shoot = 0x18;
                this.knockBack = 1f;
                this.damage = 12;
                this.width = 10;
                this.height = 10;
                this.maxStack = 250;
                this.consumable = true;
                this.useSound = 1;
                this.useAnimation = 15;
                this.useTime = 15;
                this.noUseGraphic = true;
                this.noMelee = true;
                this.value = 20;
            }
            else if (this.type == 0xa2)
            {
                this.name = "Ball 'O Hurt";
                this.useStyle = 5;
                this.useAnimation = 30;
                this.useTime = 30;
                this.knockBack = 7f;
                this.width = 30;
                this.height = 10;
                this.damage = 15;
                this.scale = 1.1f;
                this.noUseGraphic = true;
                this.shoot = 0x19;
                this.shootSpeed = 12f;
                this.useSound = 1;
                this.rare = 1;
                this.value = 0x6978;
            }
            else if (this.type == 0xa3)
            {
                this.name = "Blue Moon";
                this.useStyle = 5;
                this.useAnimation = 30;
                this.useTime = 30;
                this.knockBack = 7f;
                this.width = 30;
                this.height = 10;
                this.damage = 30;
                this.scale = 1.1f;
                this.noUseGraphic = true;
                this.shoot = 0x1a;
                this.shootSpeed = 12f;
                this.useSound = 1;
                this.rare = 2;
                this.value = 0x6978;
            }
            else if (this.type == 0xa4)
            {
                this.autoReuse = false;
                this.useStyle = 5;
                this.useAnimation = 10;
                this.useTime = 10;
                this.name = "Handgun";
                this.width = 0x18;
                this.height = 0x1c;
                this.shoot = 14;
                this.knockBack = 3f;
                this.useAmmo = 14;
                this.useSound = 11;
                this.damage = 12;
                this.shootSpeed = 10f;
                this.noMelee = true;
                this.value = 0xc350;
                this.scale = 0.8f;
                this.rare = 2;
            }
            else if (this.type == 0xa5)
            {
                this.rare = 2;
                this.mana = 20;
                this.useSound = 8;
                this.name = "Water Bolt";
                this.useStyle = 5;
                this.damage = 15;
                this.useAnimation = 20;
                this.useTime = 20;
                this.width = 0x18;
                this.height = 0x1c;
                this.shoot = 0x1b;
                this.scale = 0.8f;
                this.shootSpeed = 4f;
                this.knockBack = 5f;
                this.toolTip = "Casts a slow moving bolt of water";
            }
            else if (this.type == 0xa6)
            {
                this.useStyle = 1;
                this.name = "Bomb";
                this.shootSpeed = 5f;
                this.shoot = 0x1c;
                this.width = 20;
                this.height = 20;
                this.maxStack = 20;
                this.consumable = true;
                this.useSound = 1;
                this.useAnimation = 0x19;
                this.useTime = 0x19;
                this.noUseGraphic = true;
                this.noMelee = true;
                this.value = 500;
                this.damage = 0;
                this.toolTip = "A small explosion that will destroy some tiles";
            }
            else if (this.type == 0xa7)
            {
                this.useStyle = 1;
                this.name = "Dynamite";
                this.shootSpeed = 4f;
                this.shoot = 0x1d;
                this.width = 8;
                this.height = 0x1c;
                this.maxStack = 3;
                this.consumable = true;
                this.useSound = 1;
                this.useAnimation = 40;
                this.useTime = 40;
                this.noUseGraphic = true;
                this.noMelee = true;
                this.value = 0x1388;
                this.rare = 1;
                this.toolTip = "A large explosion that will destroy most tiles";
            }
            else if (this.type == 0xa8)
            {
                this.useStyle = 1;
                this.name = "Grenade";
                this.shootSpeed = 5.5f;
                this.shoot = 30;
                this.width = 20;
                this.height = 20;
                this.maxStack = 20;
                this.consumable = true;
                this.useSound = 1;
                this.useAnimation = 60;
                this.useTime = 60;
                this.noUseGraphic = true;
                this.noMelee = true;
                this.value = 500;
                this.damage = 60;
                this.knockBack = 8f;
                this.toolTip = "A small explosion that will not destroy tiles";
            }
            else if (this.type == 0xa9)
            {
                this.name = "Sand Block";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x35;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 170)
            {
                this.name = "Glass";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x36;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0xab)
            {
                this.name = "Sign";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x37;
                this.width = 0x1c;
                this.height = 0x1c;
            }
            else if (this.type == 0xac)
            {
                this.name = "Ash Block";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x39;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0xad)
            {
                this.name = "Obsidian";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x38;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0xae)
            {
                this.name = "Hellstone";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x3a;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0xaf)
            {
                this.name = "Hellstone Bar";
                this.width = 20;
                this.height = 20;
                this.maxStack = 0x63;
                this.rare = 2;
                this.toolTip = "Hot to the touch";
                this.value = 0x4e20;
            }
            else if (this.type == 0xb0)
            {
                this.name = "Mud Block";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x3b;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0xb1)
            {
                this.name = "Sapphire";
                this.maxStack = 0x63;
                this.alpha = 50;
                this.width = 10;
                this.height = 14;
                this.value = 0x1b58;
            }
            else if (this.type == 0xb2)
            {
                this.name = "Ruby";
                this.maxStack = 0x63;
                this.alpha = 50;
                this.width = 10;
                this.height = 14;
                this.value = 0x4e20;
            }
            else if (this.type == 0xb3)
            {
                this.name = "Emerald";
                this.maxStack = 0x63;
                this.alpha = 50;
                this.width = 10;
                this.height = 14;
                this.value = 0x3a98;
            }
            else if (this.type == 180)
            {
                this.name = "Topaz";
                this.maxStack = 0x63;
                this.alpha = 50;
                this.width = 10;
                this.height = 14;
                this.value = 0x1388;
            }
            else if (this.type == 0xb5)
            {
                this.name = "Amethyst";
                this.maxStack = 0x63;
                this.alpha = 50;
                this.width = 10;
                this.height = 14;
                this.value = 0x9c4;
            }
            else if (this.type == 0xb6)
            {
                this.name = "Diamond";
                this.maxStack = 0x63;
                this.alpha = 50;
                this.width = 10;
                this.height = 14;
                this.value = 0x9c40;
            }
            else if (this.type == 0xb7)
            {
                this.name = "Glowing Mushroom";
                this.useStyle = 2;
                this.useSound = 2;
                this.useTurn = false;
                this.useAnimation = 0x11;
                this.useTime = 0x11;
                this.width = 0x10;
                this.height = 0x12;
                this.healLife = 50;
                this.maxStack = 0x63;
                this.consumable = true;
                this.potion = true;
                this.value = 50;
            }
            else if (this.type == 0xb8)
            {
                this.name = "Star";
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0xb9)
            {
                this.noUseGraphic = true;
                this.damage = 0;
                this.knockBack = 7f;
                this.useStyle = 5;
                this.name = "Ivy Whip";
                this.shootSpeed = 13f;
                this.shoot = 0x20;
                this.width = 0x12;
                this.height = 0x1c;
                this.useSound = 1;
                this.useAnimation = 20;
                this.useTime = 20;
                this.rare = 3;
                this.noMelee = true;
                this.value = 0x4e20;
            }
            else if (this.type == 0xba)
            {
                this.name = "Breathing Reed";
                this.width = 0x2c;
                this.height = 0x2c;
                this.rare = 1;
                this.value = 0x2710;
                this.holdStyle = 2;
            }
            else if (this.type == 0xbb)
            {
                this.name = "Flipper";
                this.width = 0x1c;
                this.height = 0x1c;
                this.rare = 1;
                this.value = 0x2710;
                this.accessory = true;
                this.toolTip = "Grants the ability to swim";
            }
            else if (this.type == 0xbc)
            {
                this.name = "Healing Potion";
                this.useSound = 3;
                this.healLife = 200;
                this.useStyle = 2;
                this.useTurn = true;
                this.useAnimation = 0x11;
                this.useTime = 0x11;
                this.maxStack = 30;
                this.consumable = true;
                this.width = 14;
                this.height = 0x18;
                this.rare = 1;
                this.potion = true;
                this.value = 0x3e8;
            }
            else if (this.type == 0xbd)
            {
                this.name = "Mana Potion";
                this.useSound = 3;
                this.healMana = 200;
                this.useStyle = 2;
                this.useTurn = true;
                this.useAnimation = 0x11;
                this.useTime = 0x11;
                this.maxStack = 30;
                this.consumable = true;
                this.width = 14;
                this.height = 0x18;
                this.rare = 1;
                this.potion = true;
                this.value = 0x3e8;
            }
            else if (this.type == 190)
            {
                this.name = "Blade of Grass";
                this.useStyle = 1;
                this.useAnimation = 30;
                this.knockBack = 3f;
                this.width = 40;
                this.height = 40;
                this.damage = 0x1c;
                this.scale = 1.4f;
                this.useSound = 1;
                this.rare = 3;
                this.value = 0x6978;
            }
            else if (this.type == 0xbf)
            {
                this.noMelee = true;
                this.useStyle = 1;
                this.name = "Thorn Chakrum";
                this.shootSpeed = 11f;
                this.shoot = 0x21;
                this.damage = 0x19;
                this.knockBack = 8f;
                this.width = 14;
                this.height = 0x1c;
                this.useSound = 1;
                this.useAnimation = 15;
                this.useTime = 15;
                this.noUseGraphic = true;
                this.rare = 3;
                this.value = 0xc350;
            }
            else if (this.type == 0xc0)
            {
                this.name = "Obsidian Brick";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x4b;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0xc1)
            {
                this.name = "Obsidian Skull";
                this.width = 20;
                this.height = 0x16;
                this.rare = 2;
                this.value = 0x6978;
                this.accessory = true;
                this.defense = 2;
                this.toolTip = "Grants immunity to fire blocks";
            }
            else if (this.type == 0xc2)
            {
                this.name = "Mushroom Grass Seeds";
                this.useTurn = true;
                this.useStyle = 1;
                this.useAnimation = 15;
                this.useTime = 10;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 70;
                this.width = 14;
                this.height = 14;
                this.value = 150;
            }
            else if (this.type == 0xc3)
            {
                this.name = "Jungle Grass Seeds";
                this.useTurn = true;
                this.useStyle = 1;
                this.useAnimation = 15;
                this.useTime = 10;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 60;
                this.width = 14;
                this.height = 14;
                this.value = 150;
            }
            else if (this.type == 0xc4)
            {
                this.name = "Wooden Hammer";
                this.autoReuse = true;
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 0x25;
                this.useTime = 0x19;
                this.hammer = 0x19;
                this.width = 0x18;
                this.height = 0x1c;
                this.damage = 2;
                this.knockBack = 5.5f;
                this.scale = 1.2f;
                this.useSound = 1;
                this.tileBoost = -1;
                this.value = 50;
            }
            else if (this.type == 0xc5)
            {
                this.autoReuse = true;
                this.useStyle = 5;
                this.useAnimation = 12;
                this.useTime = 12;
                this.name = "Star Cannon";
                this.width = 50;
                this.height = 0x12;
                this.shoot = 12;
                this.useAmmo = 15;
                this.useSound = 9;
                this.damage = 0x4b;
                this.shootSpeed = 14f;
                this.noMelee = true;
                this.value = 0x7a120;
                this.rare = 2;
                this.toolTip = "Shoots fallen stars";
            }
            else if (this.type == 0xc6)
            {
                this.name = "Blue Phaseblade";
                this.useStyle = 1;
                this.useAnimation = 0x19;
                this.knockBack = 3f;
                this.width = 40;
                this.height = 40;
                this.damage = 0x15;
                this.scale = 1f;
                this.useSound = 15;
                this.rare = 1;
                this.value = 0x6978;
            }
            else if (this.type == 0xc7)
            {
                this.name = "Red Phaseblade";
                this.useStyle = 1;
                this.useAnimation = 0x19;
                this.knockBack = 3f;
                this.width = 40;
                this.height = 40;
                this.damage = 0x15;
                this.scale = 1f;
                this.useSound = 15;
                this.rare = 1;
                this.value = 0x6978;
            }
            else if (this.type == 200)
            {
                this.name = "Green Phaseblade";
                this.useStyle = 1;
                this.useAnimation = 0x19;
                this.knockBack = 3f;
                this.width = 40;
                this.height = 40;
                this.damage = 0x15;
                this.scale = 1f;
                this.useSound = 15;
                this.rare = 1;
                this.value = 0x6978;
            }
            else if (this.type == 0xc9)
            {
                this.name = "Purple Phaseblade";
                this.useStyle = 1;
                this.useAnimation = 0x19;
                this.knockBack = 3f;
                this.width = 40;
                this.height = 40;
                this.damage = 0x15;
                this.scale = 1f;
                this.useSound = 15;
                this.rare = 1;
                this.value = 0x6978;
            }
            else if (this.type == 0xca)
            {
                this.name = "White Phaseblade";
                this.useStyle = 1;
                this.useAnimation = 0x19;
                this.knockBack = 3f;
                this.width = 40;
                this.height = 40;
                this.damage = 0x15;
                this.scale = 1f;
                this.useSound = 15;
                this.rare = 1;
                this.value = 0x6978;
            }
            else if (this.type == 0xcb)
            {
                this.name = "Yellow Phaseblade";
                this.useStyle = 1;
                this.useAnimation = 0x19;
                this.knockBack = 3f;
                this.width = 40;
                this.height = 40;
                this.damage = 0x15;
                this.scale = 1f;
                this.useSound = 15;
                this.rare = 1;
                this.value = 0x6978;
            }
            else if (this.type == 0xcc)
            {
                this.name = "Meteor Hamaxe";
                this.useTurn = true;
                this.autoReuse = true;
                this.useStyle = 1;
                this.useAnimation = 30;
                this.useTime = 0x10;
                this.hammer = 60;
                this.axe = 20;
                this.width = 0x18;
                this.height = 0x1c;
                this.damage = 20;
                this.knockBack = 7f;
                this.scale = 1.2f;
                this.useSound = 1;
                this.rare = 1;
                this.value = 0x3a98;
            }
            else if (this.type == 0xcd)
            {
                this.name = "Empty Bucket";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.width = 20;
                this.height = 20;
            }
            else if (this.type == 0xce)
            {
                this.name = "Water Bucket";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.width = 20;
                this.height = 20;
            }
            else if (this.type == 0xcf)
            {
                this.name = "Lava Bucket";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.width = 20;
                this.height = 20;
            }
            else if (this.type == 0xd0)
            {
                this.name = "Jungle Rose";
                this.width = 20;
                this.height = 20;
                this.maxStack = 0x63;
                this.value = 100;
            }
            else if (this.type == 0xd1)
            {
                this.name = "Stinger";
                this.width = 0x10;
                this.height = 0x12;
                this.maxStack = 0x63;
                this.value = 200;
            }
            else if (this.type == 210)
            {
                this.name = "Vine";
                this.width = 14;
                this.height = 20;
                this.maxStack = 0x63;
                this.value = 0x3e8;
            }
            else if (this.type == 0xd3)
            {
                this.name = "Feral Claws";
                this.width = 20;
                this.height = 20;
                this.accessory = true;
                this.rare = 3;
                this.toolTip = "10 % increased melee speed";
                this.value = 0xc350;
            }
            else if (this.type == 0xd4)
            {
                this.name = "Anklet of the Wind";
                this.width = 20;
                this.height = 20;
                this.accessory = true;
                this.rare = 3;
                this.toolTip = "10% increased movement speed";
                this.value = 0xc350;
            }
            if (this.type == 0xd5)
            {
                this.name = "Staff of Regrowth";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 20;
                this.useTime = 13;
                this.autoReuse = true;
                this.width = 0x18;
                this.height = 0x1c;
                this.damage = 20;
                this.createTile = 2;
                this.scale = 1.2f;
                this.useSound = 1;
                this.knockBack = 3f;
                this.rare = 3;
                this.value = 0x7d0;
                this.toolTip = "Creates grass on dirt";
            }
            else if (this.type == 0xd6)
            {
                this.name = "Hellstone Brick";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 250;
                this.consumable = true;
                this.createTile = 0x4c;
                this.width = 12;
                this.height = 12;
            }
            else if (this.type == 0xd7)
            {
                this.name = "Whoopie Cushion";
                this.width = 0x12;
                this.height = 0x12;
                this.useTurn = true;
                this.useTime = 30;
                this.useAnimation = 30;
                this.noUseGraphic = true;
                this.useStyle = 10;
                this.useSound = 0x10;
                this.rare = 2;
                this.toolTip = "May annoy others";
                this.value = 100;
            }
            else if (this.type == 0xd8)
            {
                this.name = "Shackle";
                this.width = 20;
                this.height = 20;
                this.rare = 1;
                this.value = 0x5dc;
                this.accessory = true;
                this.defense = 1;
            }
            else if (this.type == 0xd9)
            {
                this.name = "Molten Hamaxe";
                this.useTurn = true;
                this.autoReuse = true;
                this.useStyle = 1;
                this.useAnimation = 0x1b;
                this.useTime = 14;
                this.hammer = 70;
                this.axe = 30;
                this.width = 0x18;
                this.height = 0x1c;
                this.damage = 20;
                this.knockBack = 7f;
                this.scale = 1.4f;
                this.useSound = 1;
                this.rare = 3;
                this.value = 0x3a98;
            }
            else if (this.type == 0xda)
            {
                this.mana = 20;
                this.channel = true;
                this.damage = 0x23;
                this.useStyle = 1;
                this.name = "Flamelash";
                this.shootSpeed = 6f;
                this.shoot = 0x22;
                this.width = 0x1a;
                this.height = 0x1c;
                this.useSound = 8;
                this.useAnimation = 20;
                this.useTime = 20;
                this.rare = 3;
                this.noMelee = true;
                this.knockBack = 5f;
                this.toolTip = "Summons a controllable ball of fire";
                this.value = 0x2710;
            }
            else if (this.type == 0xdb)
            {
                this.autoReuse = false;
                this.useStyle = 5;
                this.useAnimation = 10;
                this.useTime = 10;
                this.name = "Phoenix Blaster";
                this.width = 0x18;
                this.height = 0x1c;
                this.shoot = 14;
                this.knockBack = 4f;
                this.useAmmo = 14;
                this.useSound = 11;
                this.damage = 0x1c;
                this.shootSpeed = 13f;
                this.noMelee = true;
                this.value = 0xc350;
                this.scale = 0.9f;
                this.rare = 3;
            }
            else if (this.type == 220)
            {
                this.name = "Sunfury";
                this.useStyle = 5;
                this.useAnimation = 30;
                this.useTime = 30;
                this.knockBack = 7f;
                this.width = 30;
                this.height = 10;
                this.damage = 40;
                this.scale = 1.1f;
                this.noUseGraphic = true;
                this.shoot = 0x23;
                this.shootSpeed = 12f;
                this.useSound = 1;
                this.rare = 3;
                this.value = 0x6978;
            }
            else if (this.type == 0xdd)
            {
                this.name = "Hellforge";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 0x4d;
                this.width = 0x1a;
                this.height = 0x18;
                this.value = 0xbb8;
            }
            else if (this.type == 0xde)
            {
                this.name = "Clay Pot";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.autoReuse = true;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 0x4e;
                this.width = 14;
                this.height = 14;
                this.value = 100;
            }
            else if (this.type == 0xdf)
            {
                this.name = "Nature's Gift";
                this.width = 20;
                this.height = 0x16;
                this.rare = 3;
                this.value = 0x6978;
                this.accessory = true;
                this.toolTip = "Spawn with max life and mana after death";
            }
            else if (this.type == 0xe0)
            {
                this.name = "Bed";
                this.useStyle = 1;
                this.useTurn = true;
                this.useAnimation = 15;
                this.useTime = 10;
                this.maxStack = 0x63;
                this.consumable = true;
                this.createTile = 0x4f;
                this.width = 0x1c;
                this.height = 20;
                this.value = 0x7d0;
            }
            else if (this.type == 0xe1)
            {
                this.name = "Silk";
                this.maxStack = 0x63;
                this.width = 0x16;
                this.height = 0x16;
                this.value = 0x3e8;
            }
            else if (this.type == 0xe2)
            {
                this.name = "Lesser Restoration Potion";
                this.useSound = 3;
                this.healMana = 100;
                this.healLife = 100;
                this.useStyle = 2;
                this.useTurn = true;
                this.useAnimation = 0x11;
                this.useTime = 0x11;
                this.maxStack = 20;
                this.consumable = true;
                this.width = 14;
                this.height = 0x18;
                this.potion = true;
                this.value = 0x7d0;
            }
            else if (this.type == 0xe3)
            {
                this.name = "Restoration Potion";
                this.useSound = 3;
                this.healMana = 200;
                this.healLife = 200;
                this.useStyle = 2;
                this.useTurn = true;
                this.useAnimation = 0x11;
                this.useTime = 0x11;
                this.maxStack = 20;
                this.consumable = true;
                this.width = 14;
                this.height = 0x18;
                this.potion = true;
                this.value = 0xfa0;
            }
            else if (this.type == 0xe4)
            {
                this.name = "Cobalt Helmet";
                this.width = 0x16;
                this.height = 0x16;
                this.defense = 6;
                this.headSlot = 8;
                this.rare = 3;
                this.value = 0xafc8;
                this.toolTip = "Slowly regenerates mana";
                this.manaRegen = 4;
            }
            else if (this.type == 0xe5)
            {
                this.name = "Cobalt Breastplate";
                this.width = 0x1a;
                this.height = 0x1c;
                this.defense = 7;
                this.bodySlot = 8;
                this.rare = 3;
                this.value = 0x7530;
                this.toolTip = "Slowly regenerates mana";
                this.manaRegen = 4;
            }
            else if (this.type == 230)
            {
                this.name = "Cobalt Greaves";
                this.width = 0x12;
                this.height = 0x1c;
                this.defense = 6;
                this.legSlot = 8;
                this.rare = 3;
                this.value = 0x7530;
                this.toolTip = "Slowly regenerates mana";
                this.manaRegen = 3;
            }
            else if (this.type == 0xe7)
            {
                this.name = "Molten Helmet";
                this.width = 0x16;
                this.height = 0x16;
                this.defense = 9;
                this.headSlot = 9;
                this.rare = 3;
                this.value = 0xafc8;
            }
            else if (this.type == 0xe8)
            {
                this.name = "Molten Breastplate";
                this.width = 0x1a;
                this.height = 0x1c;
                this.defense = 10;
                this.bodySlot = 9;
                this.rare = 3;
                this.value = 0x7530;
            }
            else if (this.type == 0xe9)
            {
                this.name = "Molten Greaves";
                this.width = 0x12;
                this.height = 0x1c;
                this.defense = 9;
                this.legSlot = 9;
                this.rare = 3;
                this.value = 0x7530;
            }
            else if (this.type == 0xea)
            {
                this.name = "Meteor Shot";
                this.shootSpeed = 3f;
                this.shoot = 0x24;
                this.damage = 9;
                this.width = 8;
                this.height = 8;
                this.maxStack = 250;
                this.consumable = true;
                this.ammo = 14;
                this.knockBack = 1f;
                this.value = 8;
                this.rare = 1;
            }
            else if (this.type == 0xeb)
            {
                this.useStyle = 1;
                this.name = "Sticky Bomb";
                this.shootSpeed = 5f;
                this.shoot = 0x25;
                this.width = 20;
                this.height = 20;
                this.maxStack = 20;
                this.consumable = true;
                this.useSound = 1;
                this.useAnimation = 0x19;
                this.useTime = 0x19;
                this.noUseGraphic = true;
                this.noMelee = true;
                this.value = 500;
                this.damage = 0;
                this.toolTip = "Tossing may be difficult.";
            }
        }

        public void SetDefaults(string ItemName)
        {
            this.name = "";
            if (ItemName == "Gold Pickaxe")
            {
                this.SetDefaults(1);
                this.color = new Color(210, 190, 0, 100);
                this.useTime = 0x11;
                this.pick = 0x37;
                this.useAnimation = 20;
                this.scale = 1.05f;
                this.damage = 6;
                this.value = 0x2710;
            }
            else if (ItemName == "Gold Broadsword")
            {
                this.SetDefaults(4);
                this.color = new Color(210, 190, 0, 100);
                this.useAnimation = 20;
                this.damage = 13;
                this.scale = 1.05f;
                this.value = 0x2328;
            }
            else if (ItemName == "Gold Shortsword")
            {
                this.SetDefaults(6);
                this.color = new Color(210, 190, 0, 100);
                this.damage = 11;
                this.useAnimation = 11;
                this.scale = 0.95f;
                this.value = 0x1b58;
            }
            else if (ItemName == "Gold Axe")
            {
                this.SetDefaults(10);
                this.color = new Color(210, 190, 0, 100);
                this.useTime = 0x12;
                this.axe = 11;
                this.useAnimation = 0x1a;
                this.scale = 1.15f;
                this.damage = 7;
                this.value = 0x1f40;
            }
            else if (ItemName == "Gold Hammer")
            {
                this.SetDefaults(7);
                this.color = new Color(210, 190, 0, 100);
                this.useAnimation = 0x1c;
                this.useTime = 0x17;
                this.scale = 1.25f;
                this.damage = 9;
                this.hammer = 0x37;
                this.value = 0x1f40;
            }
            else if (ItemName == "Gold Bow")
            {
                this.SetDefaults(0x63);
                this.useAnimation = 0x1a;
                this.useTime = 0x1a;
                this.color = new Color(210, 190, 0, 100);
                this.damage = 11;
                this.value = 0x1b58;
            }
            else if (ItemName == "Silver Pickaxe")
            {
                this.SetDefaults(1);
                this.color = new Color(180, 180, 180, 100);
                this.useTime = 11;
                this.pick = 0x2d;
                this.useAnimation = 0x13;
                this.scale = 1.05f;
                this.damage = 6;
                this.value = 0x1388;
            }
            else if (ItemName == "Silver Broadsword")
            {
                this.SetDefaults(4);
                this.color = new Color(180, 180, 180, 100);
                this.useAnimation = 0x15;
                this.damage = 11;
                this.value = 0x1194;
            }
            else if (ItemName == "Silver Shortsword")
            {
                this.SetDefaults(6);
                this.color = new Color(180, 180, 180, 100);
                this.damage = 9;
                this.useAnimation = 12;
                this.scale = 0.95f;
                this.value = 0xdac;
            }
            else if (ItemName == "Silver Axe")
            {
                this.SetDefaults(10);
                this.color = new Color(180, 180, 180, 100);
                this.useTime = 0x12;
                this.axe = 10;
                this.useAnimation = 0x1a;
                this.scale = 1.15f;
                this.damage = 6;
                this.value = 0xfa0;
            }
            else if (ItemName == "Silver Hammer")
            {
                this.SetDefaults(7);
                this.color = new Color(180, 180, 180, 100);
                this.useAnimation = 0x1d;
                this.useTime = 0x13;
                this.scale = 1.25f;
                this.damage = 9;
                this.hammer = 0x2d;
                this.value = 0xfa0;
            }
            else if (ItemName == "Silver Bow")
            {
                this.SetDefaults(0x63);
                this.useAnimation = 0x1b;
                this.useTime = 0x1b;
                this.color = new Color(180, 180, 180, 100);
                this.damage = 10;
                this.value = 0xdac;
            }
            else if (ItemName == "Copper Pickaxe")
            {
                this.SetDefaults(1);
                this.color = new Color(180, 100, 0x2d, 80);
                this.useTime = 15;
                this.pick = 0x23;
                this.useAnimation = 0x17;
                this.scale = 0.9f;
                this.tileBoost = -1;
                this.value = 500;
            }
            else if (ItemName == "Copper Broadsword")
            {
                this.SetDefaults(4);
                this.color = new Color(180, 100, 0x2d, 80);
                this.useAnimation = 0x17;
                this.damage = 8;
                this.value = 450;
            }
            else if (ItemName == "Copper Shortsword")
            {
                this.SetDefaults(6);
                this.color = new Color(180, 100, 0x2d, 80);
                this.damage = 6;
                this.useAnimation = 13;
                this.scale = 0.8f;
                this.value = 350;
            }
            else if (ItemName == "Copper Axe")
            {
                this.SetDefaults(10);
                this.color = new Color(180, 100, 0x2d, 80);
                this.useTime = 0x15;
                this.axe = 8;
                this.useAnimation = 30;
                this.scale = 1f;
                this.damage = 3;
                this.tileBoost = -1;
                this.value = 400;
            }
            else if (ItemName == "Copper Hammer")
            {
                this.SetDefaults(7);
                this.color = new Color(180, 100, 0x2d, 80);
                this.useAnimation = 0x21;
                this.useTime = 0x17;
                this.scale = 1.1f;
                this.damage = 4;
                this.hammer = 0x23;
                this.tileBoost = -1;
                this.value = 400;
            }
            else if (ItemName == "Copper Bow")
            {
                this.SetDefaults(0x63);
                this.useAnimation = 0x1d;
                this.useTime = 0x1d;
                this.color = new Color(180, 100, 0x2d, 80);
                this.damage = 8;
                this.value = 350;
            }
            else if (ItemName != "")
            {
                for (int i = 0; i < 0xec; i++)
                {
                    this.SetDefaults(i);
                    if (this.name == ItemName)
                    {
                        break;
                    }
                    if (i == 0xeb)
                    {
                        this.SetDefaults(0);
                        this.name = "";
                    }
                }
            }
            if (this.type != 0)
            {
                this.name = ItemName;
            }
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
                if (this.wet)
                {
                    num2 = 5f;
                    num = 0.08f;
                }
                Vector2 vector = (Vector2) (this.velocity * 0.5f);
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
                    this.velocity.Y += num;
                    if (this.velocity.Y > num2)
                    {
                        this.velocity.Y = num2;
                    }
                    this.velocity.X *= 0.95f;
                    if ((this.velocity.X < 0.1) && (this.velocity.X > -0.1))
                    {
                        this.velocity.X = 0f;
                    }
                    bool flag = Collision.LavaCollision(this.position, this.width, this.height);
                    if (flag)
                    {
                        this.lavaWet = true;
                    }
                    if (Collision.WetCollision(this.position, this.width, this.height))
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
                                    for (int k = 0; k < 5; k++)
                                    {
                                        Color color2 = new Color();
                                        int num6 = Dust.NewDust(new Vector2(this.position.X - 6f, (this.position.Y + (this.height / 2)) - 8f), this.width + 12, 0x18, 0x23, 0f, 0f, 0, color2, 1f);
                                        Main.dust[num6].velocity.Y -= 1.5f;
                                        Main.dust[num6].velocity.X *= 2.5f;
                                        Main.dust[num6].scale = 1.3f;
                                        Main.dust[num6].alpha = 100;
                                        Main.dust[num6].noGravity = true;
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
                    }
                    if (!this.wet)
                    {
                        this.lavaWet = false;
                    }
                    if (this.wetCount > 0)
                    {
                        this.wetCount = (byte) (this.wetCount - 1);
                    }
                    if (this.wet)
                    {
                        if (this.wet)
                        {
                            Vector2 velocity = this.velocity;
                            this.velocity = Collision.TileCollision(this.position, this.velocity, this.width, this.height, false, false);
                            if (this.velocity.X != velocity.X)
                            {
                                vector.X = this.velocity.X;
                            }
                            if (this.velocity.Y != velocity.Y)
                            {
                                vector.Y = this.velocity.Y;
                            }
                        }
                    }
                    else
                    {
                        this.velocity = Collision.TileCollision(this.position, this.velocity, this.width, this.height, false, false);
                    }
                    if ((this.owner == Main.myPlayer) && this.lavaWet)
                    {
                        this.active = false;
                        this.type = 0;
                        this.name = "";
                        this.stack = 0;
                        if (Main.netMode != 0)
                        {
                            NetMessage.SendData(0x15, -1, -1, "", i, 0f, 0f, 0f);
                        }
                    }
                    if ((this.type == 0x4b) && Main.dayTime)
                    {
                        for (int m = 0; m < 10; m++)
                        {
                            Color color5 = new Color();
                            Dust.NewDust(this.position, this.width, this.height, 15, this.velocity.X, this.velocity.Y, 150, color5, 1.2f);
                        }
                        for (int n = 0; n < 3; n++)
                        {
                            Gore.NewGore(this.position, new Vector2(this.velocity.X, this.velocity.Y), Main.rand.Next(0x10, 0x12));
                        }
                        this.active = false;
                        this.type = 0;
                        this.stack = 0;
                        if (Main.netMode == 2)
                        {
                            NetMessage.SendData(0x15, -1, -1, "", i, 0f, 0f, 0f);
                        }
                    }
                }
                else
                {
                    this.beingGrabbed = false;
                }
                if (((this.type == 8) || (this.type == 0x29)) || (((this.type == 0x4b) || (this.type == 0x69)) || (this.type == 0x74)))
                {
                    if (!this.wet)
                    {
                        Lighting.addLight((int) ((this.position.X - 7f) / 16f), (int) ((this.position.Y - 7f) / 16f), 1f);
                    }
                }
                else if (this.type == 0xb7)
                {
                    Lighting.addLight((int) ((this.position.X - 7f) / 16f), (int) ((this.position.Y - 7f) / 16f), 0.5f);
                }
                if (this.type == 0x4b)
                {
                    if (Main.rand.Next(0x19) == 0)
                    {
                        Dust.NewDust(this.position, this.width, this.height, 15, this.velocity.X * 0.5f, this.velocity.Y * 0.5f, 150, new Color(), 1.2f);
                    }
                    if (Main.rand.Next(50) == 0)
                    {
                        Gore.NewGore(this.position, new Vector2(this.velocity.X * 0.2f, this.velocity.Y * 0.2f), Main.rand.Next(0x10, 0x12));
                    }
                }
                if (this.spawnTime < 0x7ffffffe)
                {
                    this.spawnTime++;
                }
                if ((Main.netMode == 2) && (this.owner != Main.myPlayer))
                {
                    this.release++;
                    if (this.release >= 300)
                    {
                        this.release = 0;
                        NetMessage.SendData(0x27, this.owner, -1, "", i, 0f, 0f, 0f);
                    }
                }
                if (this.wet)
                {
                    this.position += vector;
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
    }
}

