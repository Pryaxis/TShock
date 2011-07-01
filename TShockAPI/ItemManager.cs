/*   
TShock, a server mod for Terraria
Copyright (C) 2011 The TShock Team

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

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
                        if (ID < Main.maxItemTypes && ID > 0)
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
