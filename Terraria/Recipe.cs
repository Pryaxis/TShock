namespace Terraria
{
    using System;

    public class Recipe
    {
        public Item createItem = new Item();
        public static int maxRecipes = 200;
        public static int maxRequirements = 10;
        private static Recipe newRecipe = new Recipe();
        public static int numRecipes = 0;
        public Item[] requiredItem = new Item[maxRequirements];
        public int[] requiredTile = new int[maxRequirements];

        public Recipe()
        {
            for (int i = 0; i < maxRequirements; i++)
            {
                this.requiredItem[i] = new Item();
                this.requiredTile[i] = -1;
            }
        }

        private static void addRecipe()
        {
            Main.recipe[numRecipes] = newRecipe;
            newRecipe = new Recipe();
            numRecipes++;
        }

        public void Create()
        {
            for (int i = 0; i < maxRequirements; i++)
            {
                if (this.requiredItem[i].type == 0)
                {
                    break;
                }
                int stack = this.requiredItem[i].stack;
                for (int j = 0; j < 0x2c; j++)
                {
                    if (Main.player[Main.myPlayer].inventory[j].IsTheSameAs(this.requiredItem[i]))
                    {
                        if (Main.player[Main.myPlayer].inventory[j].stack > stack)
                        {
                            Item item1 = Main.player[Main.myPlayer].inventory[j];
                            item1.stack -= stack;
                            stack = 0;
                        }
                        else
                        {
                            stack -= Main.player[Main.myPlayer].inventory[j].stack;
                            Main.player[Main.myPlayer].inventory[j] = new Item();
                        }
                    }
                    if (stack <= 0)
                    {
                        break;
                    }
                }
            }
            FindRecipes();
        }

        public static void FindRecipes()
        {
            int num = Main.availableRecipe[Main.focusRecipe];
            float num2 = Main.availableRecipeY[Main.focusRecipe];
            for (int i = 0; i < maxRecipes; i++)
            {
                Main.availableRecipe[i] = 0;
            }
            Main.numAvailableRecipes = 0;
            for (int j = 0; j < maxRecipes; j++)
            {
                if (Main.recipe[j].createItem.type == 0)
                {
                    break;
                }
                bool flag = true;
                for (int n = 0; n < maxRequirements; n++)
                {
                    if (Main.recipe[j].requiredItem[n].type == 0)
                    {
                        break;
                    }
                    int stack = Main.recipe[j].requiredItem[n].stack;
                    for (int num7 = 0; num7 < 0x2c; num7++)
                    {
                        if (Main.player[Main.myPlayer].inventory[num7].IsTheSameAs(Main.recipe[j].requiredItem[n]))
                        {
                            stack -= Main.player[Main.myPlayer].inventory[num7].stack;
                        }
                        if (stack <= 0)
                        {
                            break;
                        }
                    }
                    if (stack > 0)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    bool flag2 = true;
                    for (int num8 = 0; num8 < maxRequirements; num8++)
                    {
                        if (Main.recipe[j].requiredTile[num8] == -1)
                        {
                            break;
                        }
                        if (!Main.player[Main.myPlayer].adjTile[Main.recipe[j].requiredTile[num8]])
                        {
                            flag2 = false;
                            break;
                        }
                    }
                    if (flag2)
                    {
                        Main.availableRecipe[Main.numAvailableRecipes] = j;
                        Main.numAvailableRecipes++;
                    }
                }
            }
            for (int k = 0; k < Main.numAvailableRecipes; k++)
            {
                if (num == Main.availableRecipe[k])
                {
                    Main.focusRecipe = k;
                    break;
                }
            }
            if (Main.focusRecipe >= Main.numAvailableRecipes)
            {
                Main.focusRecipe = Main.numAvailableRecipes - 1;
            }
            if (Main.focusRecipe < 0)
            {
                Main.focusRecipe = 0;
            }
            float num10 = Main.availableRecipeY[Main.focusRecipe] - num2;
            for (int m = 0; m < maxRecipes; m++)
            {
                Main.availableRecipeY[m] -= num10;
            }
        }

        public static void SetupRecipes()
        {
            newRecipe.createItem.SetDefaults(0x1c);
            newRecipe.createItem.stack = 2;
            newRecipe.requiredItem[0].SetDefaults(5);
            newRecipe.requiredItem[1].SetDefaults(0x17);
            newRecipe.requiredItem[1].stack = 2;
            newRecipe.requiredItem[2].SetDefaults(0x1f);
            newRecipe.requiredItem[2].stack = 2;
            newRecipe.requiredTile[0] = 13;
            addRecipe();
            newRecipe.createItem.SetDefaults("Healing Potion");
            newRecipe.requiredItem[0].SetDefaults(0x1c);
            newRecipe.requiredItem[0].stack = 2;
            newRecipe.requiredItem[1].SetDefaults(0xb7);
            newRecipe.requiredTile[0] = 13;
            addRecipe();
            newRecipe.createItem.SetDefaults(110);
            newRecipe.createItem.stack = 2;
            newRecipe.requiredItem[0].SetDefaults(0x4b);
            newRecipe.requiredItem[1].SetDefaults(0x17);
            newRecipe.requiredItem[1].stack = 2;
            newRecipe.requiredItem[2].SetDefaults(0x1f);
            newRecipe.requiredItem[2].stack = 2;
            newRecipe.requiredTile[0] = 13;
            addRecipe();
            newRecipe.createItem.SetDefaults("Mana Potion");
            newRecipe.requiredItem[0].SetDefaults(110);
            newRecipe.requiredItem[0].stack = 2;
            newRecipe.requiredItem[1].SetDefaults(0xb7);
            newRecipe.requiredTile[0] = 13;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xe2);
            newRecipe.requiredItem[0].SetDefaults(0x1c);
            newRecipe.requiredItem[1].SetDefaults(110);
            newRecipe.requiredTile[0] = 13;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xe3);
            newRecipe.requiredItem[0].SetDefaults("Healing Potion");
            newRecipe.requiredItem[1].SetDefaults("Mana Potion");
            newRecipe.requiredTile[0] = 13;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x43);
            newRecipe.createItem.stack = 5;
            newRecipe.requiredItem[0].SetDefaults(60);
            newRecipe.requiredTile[0] = 13;
            addRecipe();
            newRecipe.createItem.SetDefaults("Bottle");
            newRecipe.createItem.stack = 2;
            newRecipe.requiredItem[0].SetDefaults("Glass");
            newRecipe.requiredTile[0] = 0x11;
            addRecipe();
            newRecipe.createItem.SetDefaults(8);
            newRecipe.createItem.stack = 3;
            newRecipe.requiredItem[0].SetDefaults(0x17);
            newRecipe.requiredItem[0].stack = 1;
            newRecipe.requiredItem[1].SetDefaults(9);
            addRecipe();
            newRecipe.createItem.SetDefaults(0xeb);
            newRecipe.requiredItem[0].SetDefaults(0xa6);
            newRecipe.requiredItem[1].SetDefaults(0x17);
            newRecipe.requiredItem[1].stack = 5;
            addRecipe();
            newRecipe.createItem.SetDefaults("Glass");
            newRecipe.createItem.stack = 1;
            newRecipe.requiredItem[0].SetDefaults(0xa9);
            newRecipe.requiredItem[0].stack = 2;
            newRecipe.requiredTile[0] = 0x11;
            addRecipe();
            newRecipe.createItem.SetDefaults("Clay Pot");
            newRecipe.requiredItem[0].SetDefaults(0x85);
            newRecipe.requiredItem[0].stack = 6;
            newRecipe.requiredTile[0] = 0x11;
            addRecipe();
            newRecipe.createItem.SetDefaults("Gray Brick");
            newRecipe.requiredItem[0].SetDefaults(3);
            newRecipe.requiredItem[0].stack = 2;
            newRecipe.requiredTile[0] = 0x11;
            addRecipe();
            newRecipe.createItem.SetDefaults("Gray Brick Wall");
            newRecipe.createItem.stack = 4;
            newRecipe.requiredItem[0].SetDefaults("Gray Brick");
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults("Red Brick");
            newRecipe.requiredItem[0].SetDefaults(0x85);
            newRecipe.requiredItem[0].stack = 2;
            newRecipe.requiredTile[0] = 0x11;
            addRecipe();
            newRecipe.createItem.SetDefaults("Red Brick Wall");
            newRecipe.createItem.stack = 4;
            newRecipe.requiredItem[0].SetDefaults("Red Brick");
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults("Copper Brick");
            newRecipe.requiredItem[0].SetDefaults(3);
            newRecipe.requiredItem[0].stack = 1;
            newRecipe.requiredItem[1].SetDefaults("Copper Ore");
            newRecipe.requiredItem[1].stack = 1;
            newRecipe.requiredTile[0] = 0x11;
            addRecipe();
            newRecipe.createItem.SetDefaults("Copper Brick Wall");
            newRecipe.createItem.stack = 4;
            newRecipe.requiredItem[0].SetDefaults("Copper Brick");
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults("Silver Brick Wall");
            newRecipe.createItem.stack = 4;
            newRecipe.requiredItem[0].SetDefaults("Silver Brick");
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults("Silver Brick");
            newRecipe.requiredItem[0].SetDefaults(3);
            newRecipe.requiredItem[0].stack = 1;
            newRecipe.requiredItem[1].SetDefaults("Silver Ore");
            newRecipe.requiredItem[1].stack = 1;
            newRecipe.requiredTile[0] = 0x11;
            addRecipe();
            newRecipe.createItem.SetDefaults("Gold Brick Wall");
            newRecipe.createItem.stack = 4;
            newRecipe.requiredItem[0].SetDefaults("Gold Brick");
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults("Gold Brick");
            newRecipe.requiredItem[0].SetDefaults(3);
            newRecipe.requiredItem[0].stack = 1;
            newRecipe.requiredItem[1].SetDefaults("Gold Ore");
            newRecipe.requiredItem[1].stack = 1;
            newRecipe.requiredTile[0] = 0x11;
            addRecipe();
            newRecipe.createItem.SetDefaults("Hellstone Brick");
            newRecipe.requiredItem[0].SetDefaults(0xae);
            newRecipe.requiredItem[1].SetDefaults(1);
            newRecipe.requiredTile[0] = 0x11;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xc0);
            newRecipe.requiredItem[0].SetDefaults(0xad);
            newRecipe.requiredItem[0].stack = 2;
            newRecipe.requiredTile[0] = 0x11;
            addRecipe();
            newRecipe.createItem.SetDefaults(30);
            newRecipe.createItem.stack = 4;
            newRecipe.requiredItem[0].SetDefaults(2);
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x1a);
            newRecipe.createItem.stack = 4;
            newRecipe.requiredItem[0].SetDefaults(3);
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x5d);
            newRecipe.createItem.stack = 4;
            newRecipe.requiredItem[0].SetDefaults(9);
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x5e);
            newRecipe.requiredItem[0].SetDefaults(9);
            addRecipe();
            newRecipe.createItem.SetDefaults(0x19);
            newRecipe.requiredItem[0].SetDefaults(9);
            newRecipe.requiredItem[0].stack = 6;
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x22);
            newRecipe.requiredItem[0].SetDefaults(9);
            newRecipe.requiredItem[0].stack = 4;
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults("Sign");
            newRecipe.requiredItem[0].SetDefaults(9);
            newRecipe.requiredItem[0].stack = 6;
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x30);
            newRecipe.requiredItem[0].SetDefaults(9);
            newRecipe.requiredItem[0].stack = 8;
            newRecipe.requiredItem[1].SetDefaults(0x16);
            newRecipe.requiredItem[1].stack = 2;
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x20);
            newRecipe.requiredItem[0].SetDefaults(9);
            newRecipe.requiredItem[0].stack = 8;
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x24);
            newRecipe.requiredItem[0].SetDefaults(9);
            newRecipe.requiredItem[0].stack = 10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x18);
            newRecipe.requiredItem[0].SetDefaults(9);
            newRecipe.requiredItem[0].stack = 7;
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xc4);
            newRecipe.requiredItem[0].SetDefaults(9);
            newRecipe.requiredItem[0].stack = 8;
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults(40);
            newRecipe.createItem.stack = 3;
            newRecipe.requiredItem[0].SetDefaults(9);
            newRecipe.requiredItem[1].SetDefaults(3);
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x27);
            newRecipe.requiredItem[0].SetDefaults(9);
            newRecipe.requiredItem[0].stack = 10;
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults("Bed");
            newRecipe.requiredItem[0].SetDefaults(9);
            newRecipe.requiredItem[0].stack = 15;
            newRecipe.requiredItem[1].SetDefaults("Silk");
            newRecipe.requiredItem[1].stack = 5;
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults("Silk");
            newRecipe.requiredItem[0].SetDefaults(150);
            newRecipe.requiredItem[0].stack = 10;
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults("Flaming Arrow");
            newRecipe.createItem.stack = 5;
            newRecipe.requiredItem[0].SetDefaults(40);
            newRecipe.requiredItem[0].stack = 5;
            newRecipe.requiredItem[1].SetDefaults(8);
            addRecipe();
            newRecipe.createItem.SetDefaults(0x21);
            newRecipe.requiredItem[0].SetDefaults(3);
            newRecipe.requiredItem[0].stack = 20;
            newRecipe.requiredItem[1].SetDefaults(9);
            newRecipe.requiredItem[1].stack = 4;
            newRecipe.requiredItem[2].SetDefaults(8);
            newRecipe.requiredItem[2].stack = 3;
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults(20);
            newRecipe.requiredItem[0].SetDefaults(12);
            newRecipe.requiredItem[0].stack = 3;
            newRecipe.requiredTile[0] = 0x11;
            addRecipe();
            newRecipe.createItem.SetDefaults("Copper Pickaxe");
            newRecipe.requiredItem[0].SetDefaults(20);
            newRecipe.requiredItem[0].stack = 12;
            newRecipe.requiredItem[1].SetDefaults(9);
            newRecipe.requiredItem[1].stack = 4;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Copper Axe");
            newRecipe.requiredItem[0].SetDefaults(20);
            newRecipe.requiredItem[0].stack = 9;
            newRecipe.requiredItem[1].SetDefaults(9);
            newRecipe.requiredItem[1].stack = 3;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Copper Hammer");
            newRecipe.requiredItem[0].SetDefaults(20);
            newRecipe.requiredItem[0].stack = 10;
            newRecipe.requiredItem[1].SetDefaults(9);
            newRecipe.requiredItem[1].stack = 3;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Copper Broadsword");
            newRecipe.requiredItem[0].SetDefaults(20);
            newRecipe.requiredItem[0].stack = 8;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Copper Shortsword");
            newRecipe.requiredItem[0].SetDefaults(20);
            newRecipe.requiredItem[0].stack = 7;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Copper Bow");
            newRecipe.requiredItem[0].SetDefaults(20);
            newRecipe.requiredItem[0].stack = 7;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Copper Helmet");
            newRecipe.requiredItem[0].SetDefaults(20);
            newRecipe.requiredItem[0].stack = 15;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Copper Chainmail");
            newRecipe.requiredItem[0].SetDefaults(20);
            newRecipe.requiredItem[0].stack = 0x19;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Copper Greaves");
            newRecipe.requiredItem[0].SetDefaults(20);
            newRecipe.requiredItem[0].stack = 20;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Copper Watch");
            newRecipe.requiredItem[0].SetDefaults(20);
            newRecipe.requiredItem[0].stack = 10;
            newRecipe.requiredItem[1].SetDefaults(0x55);
            newRecipe.requiredTile[0] = 14;
            newRecipe.requiredTile[1] = 15;
            addRecipe();
            newRecipe.createItem.SetDefaults("Copper Chandelier");
            newRecipe.requiredItem[0].SetDefaults(20);
            newRecipe.requiredItem[0].stack = 4;
            newRecipe.requiredItem[1].SetDefaults(8);
            newRecipe.requiredItem[1].stack = 4;
            newRecipe.requiredItem[2].SetDefaults(0x55);
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x16);
            newRecipe.requiredItem[0].SetDefaults(11);
            newRecipe.requiredItem[0].stack = 3;
            newRecipe.requiredTile[0] = 0x11;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x23);
            newRecipe.requiredItem[0].SetDefaults(0x16);
            newRecipe.requiredItem[0].stack = 5;
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xcd);
            newRecipe.requiredItem[0].SetDefaults(0x16);
            newRecipe.requiredItem[0].stack = 3;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(1);
            newRecipe.requiredItem[0].SetDefaults(0x16);
            newRecipe.requiredItem[0].stack = 12;
            newRecipe.requiredItem[1].SetDefaults(9);
            newRecipe.requiredItem[1].stack = 3;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(10);
            newRecipe.requiredItem[0].SetDefaults(0x16);
            newRecipe.requiredItem[0].stack = 9;
            newRecipe.requiredItem[1].SetDefaults(9);
            newRecipe.requiredItem[1].stack = 3;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(7);
            newRecipe.requiredItem[0].SetDefaults(0x16);
            newRecipe.requiredItem[0].stack = 10;
            newRecipe.requiredItem[1].SetDefaults(9);
            newRecipe.requiredItem[1].stack = 3;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(4);
            newRecipe.requiredItem[0].SetDefaults(0x16);
            newRecipe.requiredItem[0].stack = 8;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(6);
            newRecipe.requiredItem[0].SetDefaults(0x16);
            newRecipe.requiredItem[0].stack = 7;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Iron Bow");
            newRecipe.requiredItem[0].SetDefaults(0x16);
            newRecipe.requiredItem[0].stack = 7;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Iron Helmet");
            newRecipe.requiredItem[0].SetDefaults(0x16);
            newRecipe.requiredItem[0].stack = 20;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Iron Chainmail");
            newRecipe.requiredItem[0].SetDefaults(0x16);
            newRecipe.requiredItem[0].stack = 30;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Iron Greaves");
            newRecipe.requiredItem[0].SetDefaults(0x16);
            newRecipe.requiredItem[0].stack = 0x19;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Iron Chain");
            newRecipe.requiredItem[0].SetDefaults(0x16);
            newRecipe.requiredItem[0].stack = 3;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x15);
            newRecipe.requiredItem[0].SetDefaults(14);
            newRecipe.requiredItem[0].stack = 4;
            newRecipe.requiredTile[0] = 0x11;
            addRecipe();
            newRecipe.createItem.SetDefaults("Silver Pickaxe");
            newRecipe.requiredItem[0].SetDefaults(0x15);
            newRecipe.requiredItem[0].stack = 12;
            newRecipe.requiredItem[1].SetDefaults(9);
            newRecipe.requiredItem[1].stack = 4;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Silver Axe");
            newRecipe.requiredItem[0].SetDefaults(0x15);
            newRecipe.requiredItem[0].stack = 9;
            newRecipe.requiredItem[1].SetDefaults(9);
            newRecipe.requiredItem[1].stack = 3;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Silver Hammer");
            newRecipe.requiredItem[0].SetDefaults(0x15);
            newRecipe.requiredItem[0].stack = 10;
            newRecipe.requiredItem[1].SetDefaults(9);
            newRecipe.requiredItem[1].stack = 3;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Silver Broadsword");
            newRecipe.requiredItem[0].SetDefaults(0x15);
            newRecipe.requiredItem[0].stack = 8;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Silver Bow");
            newRecipe.requiredItem[0].SetDefaults(0x15);
            newRecipe.requiredItem[0].stack = 7;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Silver Helmet");
            newRecipe.requiredItem[0].SetDefaults(0x15);
            newRecipe.requiredItem[0].stack = 20;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Silver Chainmail");
            newRecipe.requiredItem[0].SetDefaults(0x15);
            newRecipe.requiredItem[0].stack = 30;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Silver Greaves");
            newRecipe.requiredItem[0].SetDefaults(0x15);
            newRecipe.requiredItem[0].stack = 0x19;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Silver Watch");
            newRecipe.requiredItem[0].SetDefaults(0x15);
            newRecipe.requiredItem[0].stack = 10;
            newRecipe.requiredItem[1].SetDefaults(0x55);
            newRecipe.requiredTile[0] = 14;
            newRecipe.requiredTile[1] = 15;
            addRecipe();
            newRecipe.createItem.SetDefaults("Silver Chandelier");
            newRecipe.requiredItem[0].SetDefaults(0x15);
            newRecipe.requiredItem[0].stack = 4;
            newRecipe.requiredItem[1].SetDefaults(8);
            newRecipe.requiredItem[1].stack = 4;
            newRecipe.requiredItem[2].SetDefaults(0x55);
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x13);
            newRecipe.requiredItem[0].SetDefaults(13);
            newRecipe.requiredItem[0].stack = 4;
            newRecipe.requiredTile[0] = 0x11;
            addRecipe();
            newRecipe.createItem.SetDefaults("Gold Pickaxe");
            newRecipe.requiredItem[0].SetDefaults(0x13);
            newRecipe.requiredItem[0].stack = 12;
            newRecipe.requiredItem[1].SetDefaults(9);
            newRecipe.requiredItem[1].stack = 4;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Gold Axe");
            newRecipe.requiredItem[0].SetDefaults(0x13);
            newRecipe.requiredItem[0].stack = 9;
            newRecipe.requiredItem[1].SetDefaults(9);
            newRecipe.requiredItem[1].stack = 3;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Gold Hammer");
            newRecipe.requiredItem[0].SetDefaults(0x13);
            newRecipe.requiredItem[0].stack = 10;
            newRecipe.requiredItem[1].SetDefaults(9);
            newRecipe.requiredItem[1].stack = 3;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Gold Broadsword");
            newRecipe.requiredItem[0].SetDefaults(0x13);
            newRecipe.requiredItem[0].stack = 8;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Gold Shortsword");
            newRecipe.requiredItem[0].SetDefaults(0x13);
            newRecipe.requiredItem[0].stack = 7;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Gold Bow");
            newRecipe.requiredItem[0].SetDefaults(0x13);
            newRecipe.requiredItem[0].stack = 7;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Gold Helmet");
            newRecipe.requiredItem[0].SetDefaults(0x13);
            newRecipe.requiredItem[0].stack = 0x19;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Gold Chainmail");
            newRecipe.requiredItem[0].SetDefaults(0x13);
            newRecipe.requiredItem[0].stack = 0x23;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Gold Greaves");
            newRecipe.requiredItem[0].SetDefaults(0x13);
            newRecipe.requiredItem[0].stack = 30;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Gold Watch");
            newRecipe.requiredItem[0].SetDefaults(0x13);
            newRecipe.requiredItem[0].stack = 10;
            newRecipe.requiredItem[1].SetDefaults(0x55);
            newRecipe.requiredTile[0] = 14;
            newRecipe.requiredTile[1] = 15;
            addRecipe();
            newRecipe.createItem.SetDefaults("Gold Chandelier");
            newRecipe.requiredItem[0].SetDefaults(0x13);
            newRecipe.requiredItem[0].stack = 4;
            newRecipe.requiredItem[1].SetDefaults(8);
            newRecipe.requiredItem[1].stack = 4;
            newRecipe.requiredItem[2].SetDefaults(0x55);
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Candle");
            newRecipe.requiredItem[0].SetDefaults(0x13);
            newRecipe.requiredItem[1].SetDefaults(8);
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x39);
            newRecipe.requiredItem[0].SetDefaults(0x38);
            newRecipe.requiredItem[0].stack = 4;
            newRecipe.requiredTile[0] = 0x11;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x2c);
            newRecipe.requiredItem[0].SetDefaults(0x39);
            newRecipe.requiredItem[0].stack = 8;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Unholy Arrow");
            newRecipe.createItem.stack = 2;
            newRecipe.requiredItem[0].SetDefaults(40);
            newRecipe.requiredItem[0].stack = 2;
            newRecipe.requiredItem[1].SetDefaults(0x45);
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x2d);
            newRecipe.requiredItem[0].SetDefaults(0x39);
            newRecipe.requiredItem[0].stack = 10;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x2e);
            newRecipe.requiredItem[0].SetDefaults(0x39);
            newRecipe.requiredItem[0].stack = 10;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Shadow Helmet");
            newRecipe.requiredItem[0].SetDefaults(0x39);
            newRecipe.requiredItem[0].stack = 15;
            newRecipe.requiredItem[1].SetDefaults(0x56);
            newRecipe.requiredItem[1].stack = 10;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Shadow Scalemail");
            newRecipe.requiredItem[0].SetDefaults(0x39);
            newRecipe.requiredItem[0].stack = 0x19;
            newRecipe.requiredItem[1].SetDefaults(0x56);
            newRecipe.requiredItem[1].stack = 20;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Shadow Greaves");
            newRecipe.requiredItem[0].SetDefaults(0x39);
            newRecipe.requiredItem[0].stack = 20;
            newRecipe.requiredItem[1].SetDefaults(0x56);
            newRecipe.requiredItem[1].stack = 15;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Nightmare Pickaxe");
            newRecipe.requiredItem[0].SetDefaults(0x39);
            newRecipe.requiredItem[0].stack = 12;
            newRecipe.requiredItem[1].SetDefaults(0x56);
            newRecipe.requiredItem[1].stack = 6;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("The Breaker");
            newRecipe.requiredItem[0].SetDefaults(0x39);
            newRecipe.requiredItem[0].stack = 10;
            newRecipe.requiredItem[1].SetDefaults(0x56);
            newRecipe.requiredItem[1].stack = 5;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Grappling Hook");
            newRecipe.requiredItem[0].SetDefaults(0x55);
            newRecipe.requiredItem[0].stack = 3;
            newRecipe.requiredItem[1].SetDefaults(0x76);
            newRecipe.requiredItem[1].stack = 1;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x75);
            newRecipe.requiredItem[0].SetDefaults(0x74);
            newRecipe.requiredItem[0].stack = 6;
            newRecipe.requiredTile[0] = 0x11;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xc6);
            newRecipe.requiredItem[0].SetDefaults(0x75);
            newRecipe.requiredItem[0].stack = 20;
            newRecipe.requiredItem[1].SetDefaults(0xb1);
            newRecipe.requiredItem[1].stack = 10;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xc7);
            newRecipe.requiredItem[0].SetDefaults(0x75);
            newRecipe.requiredItem[0].stack = 20;
            newRecipe.requiredItem[1].SetDefaults(0xb2);
            newRecipe.requiredItem[1].stack = 10;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(200);
            newRecipe.requiredItem[0].SetDefaults(0x75);
            newRecipe.requiredItem[0].stack = 20;
            newRecipe.requiredItem[1].SetDefaults(0xb3);
            newRecipe.requiredItem[1].stack = 10;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xc9);
            newRecipe.requiredItem[0].SetDefaults(0x75);
            newRecipe.requiredItem[0].stack = 20;
            newRecipe.requiredItem[1].SetDefaults(0xb5);
            newRecipe.requiredItem[1].stack = 10;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xca);
            newRecipe.requiredItem[0].SetDefaults(0x75);
            newRecipe.requiredItem[0].stack = 20;
            newRecipe.requiredItem[1].SetDefaults(0xb6);
            newRecipe.requiredItem[1].stack = 10;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xcb);
            newRecipe.requiredItem[0].SetDefaults(0x75);
            newRecipe.requiredItem[0].stack = 20;
            newRecipe.requiredItem[1].SetDefaults(180);
            newRecipe.requiredItem[1].stack = 10;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xcc);
            newRecipe.requiredItem[0].SetDefaults(0x75);
            newRecipe.requiredItem[0].stack = 0x23;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x7f);
            newRecipe.requiredItem[0].SetDefaults(0x5f);
            newRecipe.requiredItem[1].SetDefaults(0x75);
            newRecipe.requiredItem[1].stack = 30;
            newRecipe.requiredItem[2].SetDefaults(0x4b);
            newRecipe.requiredItem[2].stack = 10;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xc5);
            newRecipe.requiredItem[0].SetDefaults(0x62);
            newRecipe.requiredItem[1].SetDefaults(0x75);
            newRecipe.requiredItem[1].stack = 20;
            newRecipe.requiredItem[2].SetDefaults(0x4b);
            newRecipe.requiredItem[2].stack = 5;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Meteor Helmet");
            newRecipe.requiredItem[0].SetDefaults(0x75);
            newRecipe.requiredItem[0].stack = 0x19;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Meteor Suit");
            newRecipe.requiredItem[0].SetDefaults(0x75);
            newRecipe.requiredItem[0].stack = 0x23;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Meteor Leggings");
            newRecipe.requiredItem[0].SetDefaults(0x75);
            newRecipe.requiredItem[0].stack = 30;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Meteor Shot");
            newRecipe.createItem.stack = 100;
            newRecipe.requiredItem[0].SetDefaults(0x75);
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x97);
            newRecipe.requiredItem[0].SetDefaults(0x9a);
            newRecipe.requiredItem[0].stack = 0x19;
            newRecipe.requiredItem[1].SetDefaults(150);
            newRecipe.requiredItem[1].stack = 40;
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x98);
            newRecipe.requiredItem[0].SetDefaults(0x9a);
            newRecipe.requiredItem[0].stack = 0x23;
            newRecipe.requiredItem[1].SetDefaults(150);
            newRecipe.requiredItem[1].stack = 50;
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x99);
            newRecipe.requiredItem[0].SetDefaults(0x9a);
            newRecipe.requiredItem[0].stack = 30;
            newRecipe.requiredItem[1].SetDefaults(150);
            newRecipe.requiredItem[1].stack = 0x2d;
            newRecipe.requiredTile[0] = 0x12;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xaf);
            newRecipe.requiredItem[0].SetDefaults(0xae);
            newRecipe.requiredItem[0].stack = 6;
            newRecipe.requiredItem[1].SetDefaults(0xad);
            newRecipe.requiredItem[1].stack = 2;
            newRecipe.requiredTile[0] = 0x4d;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x77);
            newRecipe.requiredItem[0].SetDefaults(0xaf);
            newRecipe.requiredItem[0].stack = 15;
            newRecipe.requiredItem[1].SetDefaults(0x37);
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(120);
            newRecipe.requiredItem[0].SetDefaults(0xaf);
            newRecipe.requiredItem[0].stack = 0x19;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x79);
            newRecipe.requiredItem[0].SetDefaults(0xaf);
            newRecipe.requiredItem[0].stack = 0x23;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x7a);
            newRecipe.requiredItem[0].SetDefaults(0xaf);
            newRecipe.requiredItem[0].stack = 0x23;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xd9);
            newRecipe.requiredItem[0].SetDefaults(0xaf);
            newRecipe.requiredItem[0].stack = 0x23;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xdb);
            newRecipe.requiredItem[0].SetDefaults(0xaf);
            newRecipe.requiredItem[0].stack = 20;
            newRecipe.requiredItem[1].SetDefaults("Handgun");
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xe7);
            newRecipe.requiredItem[0].SetDefaults(0xaf);
            newRecipe.requiredItem[0].stack = 30;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xe8);
            newRecipe.requiredItem[0].SetDefaults(0xaf);
            newRecipe.requiredItem[0].stack = 40;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xe9);
            newRecipe.requiredItem[0].SetDefaults(0xaf);
            newRecipe.requiredItem[0].stack = 0x23;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(190);
            newRecipe.requiredItem[0].SetDefaults("Silver Broadsword");
            newRecipe.requiredItem[1].SetDefaults(0xd0);
            newRecipe.requiredItem[1].stack = 40;
            newRecipe.requiredItem[2].SetDefaults(0xd1);
            newRecipe.requiredItem[2].stack = 20;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xbf);
            newRecipe.requiredItem[0].SetDefaults(0xd0);
            newRecipe.requiredItem[0].stack = 40;
            newRecipe.requiredItem[1].SetDefaults(0xd1);
            newRecipe.requiredItem[1].stack = 30;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xb9);
            newRecipe.requiredItem[0].SetDefaults(0x54);
            newRecipe.requiredItem[1].SetDefaults(0xd0);
            newRecipe.requiredItem[1].stack = 30;
            newRecipe.requiredItem[2].SetDefaults(210);
            newRecipe.requiredItem[2].stack = 3;
            newRecipe.requiredTile[0] = 0x10;
            addRecipe();
            newRecipe.createItem.SetDefaults("Depth Meter");
            newRecipe.requiredItem[0].SetDefaults(20);
            newRecipe.requiredItem[0].stack = 10;
            newRecipe.requiredItem[1].SetDefaults(0x15);
            newRecipe.requiredItem[1].stack = 8;
            newRecipe.requiredItem[2].SetDefaults(0x13);
            newRecipe.requiredItem[2].stack = 6;
            newRecipe.requiredTile[0] = 14;
            newRecipe.requiredTile[1] = 15;
            addRecipe();
            newRecipe.createItem.SetDefaults(0xc1);
            newRecipe.requiredItem[0].SetDefaults(0xad);
            newRecipe.requiredItem[0].stack = 20;
            newRecipe.requiredTile[0] = 0x11;
            addRecipe();
            newRecipe.createItem.SetDefaults("Goggles");
            newRecipe.requiredItem[0].SetDefaults(0x26);
            newRecipe.requiredItem[0].stack = 2;
            newRecipe.requiredTile[0] = 0x12;
            newRecipe.requiredTile[1] = 15;
            addRecipe();
            newRecipe.createItem.SetDefaults("Mana Crystal");
            newRecipe.requiredItem[0].SetDefaults(0x4b);
            newRecipe.requiredItem[0].stack = 10;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x2b);
            newRecipe.requiredItem[0].SetDefaults(0x26);
            newRecipe.requiredItem[0].stack = 10;
            newRecipe.requiredTile[0] = 0x1a;
            addRecipe();
            newRecipe.createItem.SetDefaults(70);
            newRecipe.requiredItem[0].SetDefaults(0x43);
            newRecipe.requiredItem[0].stack = 30;
            newRecipe.requiredItem[1].SetDefaults(0x44);
            newRecipe.requiredItem[1].stack = 15;
            newRecipe.requiredTile[0] = 0x1a;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x47);
            newRecipe.createItem.stack = 100;
            newRecipe.requiredItem[0].SetDefaults(0x48);
            newRecipe.requiredItem[0].stack = 1;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x48);
            newRecipe.createItem.stack = 1;
            newRecipe.requiredItem[0].SetDefaults(0x47);
            newRecipe.requiredItem[0].stack = 100;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x48);
            newRecipe.createItem.stack = 100;
            newRecipe.requiredItem[0].SetDefaults(0x49);
            newRecipe.requiredItem[0].stack = 1;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x49);
            newRecipe.createItem.stack = 1;
            newRecipe.requiredItem[0].SetDefaults(0x48);
            newRecipe.requiredItem[0].stack = 100;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x49);
            newRecipe.createItem.stack = 100;
            newRecipe.requiredItem[0].SetDefaults(0x4a);
            newRecipe.requiredItem[0].stack = 1;
            addRecipe();
            newRecipe.createItem.SetDefaults(0x4a);
            newRecipe.createItem.stack = 1;
            newRecipe.requiredItem[0].SetDefaults(0x49);
            newRecipe.requiredItem[0].stack = 100;
            addRecipe();
        }
    }
}

