using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Community.CsharpSqlite.SQLiteClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TShockAPI;
using TShockAPI.DB;

namespace UnitTests
{
    [TestClass]
    public class BanManagerTest
    {
        public static IDbConnection DB;
        private BanManager Bans;

        [TestInitialize]
        public void Initialize()
        {
            TShock.Config = new ConfigFile();
            TShock.Config.StorageType = "sqlite";

            DB = new SqliteConnection(string.Format("uri=file://{0},Version=3", "tshock.test.sqlite"));
            DB.Open();

            Bans = new BanManager(DB);
        }

        [TestMethod]
        public void TestDBNotNull()
        {
            Assert.IsNotNull(Bans);
        }

        [TestMethod]
        public void AddBanTest()
        {
            Assert.IsTrue(Bans.AddBan("127.0.0.1", "BanTest", "Ban Testing"));
        }

        [TestMethod]
        public void FindBanTest()
        {
            Assert.IsNotNull(Bans.GetBanByIp("127.0.0.1"));
            TShock.Config.EnableBanOnUsernames = true;
            Assert.IsNotNull(Bans.GetBanByName("BanTest"));
            TShock.Config.EnableBanOnUsernames = false;
            Assert.IsNull(Bans.GetBanByName("BanTest"));
        }
    }
}
