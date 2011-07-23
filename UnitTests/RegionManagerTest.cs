using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using TShockAPI;
using Community.CsharpSqlite.SQLiteClient;
using TShockAPI.DB;
using Microsoft.Xna.Framework;

namespace UnitTests
{
    /// <summary>
    /// Summary description for RegionManagerTest
    /// </summary>
    [TestClass]
    public class RegionManagerTest
    {
        public static IDbConnection DB;
        public static RegionManager manager;
        [TestInitialize]
        public void Initialize()
        {
            TShock.Config = new ConfigFile();
            TShock.Config.StorageType = "sqlite";

            DB = new SqliteConnection(string.Format("uri=file://{0},Version=3", "tshock.test.sqlite"));
            DB.Open();

            manager = new RegionManager(DB);
        }


        [TestMethod]
        public void AddRegion()
        {
            Region r = new Region( new Rectangle(100,100,100,100), "test", 0, "test world");
            Assert.IsTrue(manager.AddRegion(r.RegionArea.X, r.RegionArea.Y, r.RegionArea.Width, r.RegionArea.Height, r.RegionName, r.RegionWorldID));
            Assert.AreEqual(1, manager.Regions.Count);

            Region r2 = new Region(new Rectangle(201, 201, 100, 100), "test2", 0, "test world");
            manager.AddRegion(r2.RegionArea.X, r2.RegionArea.Y, r2.RegionArea.Width, r2.RegionArea.Height, r2.RegionName, r2.RegionWorldID);
            Assert.AreEqual(2, manager.Regions.Count);
        }

        [TestMethod]
        public void DeleteRegion()
        {
            Assert.IsTrue(manager.DeleteRegion("test"));
            Assert.IsTrue(manager.DeleteRegion("test2"));
            Assert.AreEqual(0, manager.Regions.Count);
        }

        [TestMethod]
        public void InRegion()
        {
            //
            // TODO: Add test logic here
            //
        }

        [TestMethod]
        public void TestMethod2()
        {
            //
            // TODO: Add test logic here
            //
        }

        [TestMethod]
        public void TestMethod3()
        {
            //
            // TODO: Add test logic here
            //
        }

        [TestCleanup]
        public void Cleanup()
        {
            DB.Close();
        }
    }
}
