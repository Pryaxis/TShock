using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Terraria;

namespace TShockAPI
{
    class ItemManager
    {
        public static List<ItemBan> BannedItems = new List<ItemBan>();

        public static void LoadBans()
        {
            try
            {
                if (!File.Exists(FileTools.ItemBansPath))
                    return;

                BannedItems.Clear();

                foreach (var line in File.ReadAllLines(FileTools.ItemBansPath))
                {
                    int ID = -1;
                    if (Int32.TryParse(line, out ID))
                    {
                        if (ID <= 326) //MUST CHANGE ON EACH UPDATE
                        {
                            var item = Tools.GetItemById(ID);
                            BannedItems.Add(new ItemBan(ID, item.name));
                            Log.Info("Item: " + item.name + " is banned");
                        }
                        else
                        {
                            Log.Warn("Invalid ID " + ID);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }

        public static bool ItemIsBanned(string ID)
        {
            foreach (ItemBan item in BannedItems)
            {
                if (ID == item.Name)
                    return true;
            }
            return false;
        }
    }

    public class ItemBan
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public ItemBan(int id, string name)
        {
            ID = id;
            Name = name;
        }

        public ItemBan()
        {
            ID = -1;
            Name = string.Empty;
        }
    }
}
