/*
TShock, a server mod for Terraria
Copyright (C) 2011-2012 The TShock Team

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
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Data;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading;
using Mono.Data.Sqlite;
using TShockAPI.DB;
using TShockAPI;

namespace UnitTests
{

    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ItemManagerTest
    {
        public static IDbConnection DB;
        public static ItemManager manager;
        [TestInitialize]
        public void Initialize()
        {
            TShock.Config = new ConfigFile();
            TShock.Config.StorageType = "sqlite";

            DB = new SqliteConnection(string.Format("uri=file://{0},Version=3", "tshock.test.sqlite"));
            DB.Open();
            manager = new ItemManager(DB);
        }

        [TestMethod]
        public void SQLiteItemTest_AddBan()
        {
            Assert.IsNotNull(manager);
            Assert.IsFalse( manager.ItemIsBanned("Dirt Block"), "Item isn't banned" );
            manager.AddNewBan("Dirt Block");
            Assert.IsTrue( manager.ItemIsBanned("Dirt Block"), "New item is added");
            Assert.IsFalse( manager.ItemIsBanned("Green Brick"), "Item isn't banned");
            manager.AddNewBan("Green Brick");
            Assert.IsTrue( manager.ItemIsBanned("Green Brick"), "New item is added");
            Assert.AreEqual(2, manager.ItemBans.Count, "Adding both items");
            manager.AddNewBan("Green Brick" );
            Assert.AreEqual(2, manager.ItemBans.Count, "Adding duplicate items");
        }

        [TestMethod]
        public void SQLiteItemTest_RemoveBan()
        {
            manager = new ItemManager(DB);
            Assert.IsNotNull(manager);
            Assert.AreEqual(2, manager.ItemBans.Count);
            manager.AddNewBan("Dirt Block");
            Assert.AreEqual(2, manager.ItemBans.Count);
            Assert.AreEqual(true, manager.ItemIsBanned("Dirt Block"));
            manager.RemoveBan("Dirt Block");
            manager.UpdateItemBans();
            Assert.AreEqual(1, manager.ItemBans.Count);
            Assert.AreEqual(false, manager.ItemIsBanned("Dirt Block"));
            manager.RemoveBan("Dirt Block");
            Assert.AreEqual(false, manager.ItemIsBanned("Dirt Block"));
            Assert.AreEqual(true, manager.ItemIsBanned("Green Brick"));
            manager.RemoveBan("Green Brick");
            Assert.AreEqual(false, manager.ItemIsBanned("Green Brick"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            DB.Close();
        }
    }
}
