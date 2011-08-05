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
using Community.CsharpSqlite.SQLiteClient;
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
