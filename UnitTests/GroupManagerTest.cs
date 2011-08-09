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
    public class GroupManagerTest
    {
        public static IDbConnection DB;
        private GroupManager Groups;

        [TestInitialize]
        public void Initialize()
        {
            TShock.Config = new ConfigFile();
            TShock.Config.StorageType = "sqlite";

            DB = new SqliteConnection(string.Format("uri=file://{0},Version=3", "tshock.test.sqlite"));
            DB.Open();

            Groups = new GroupManager(DB);
            TShock.Groups = this.Groups;
        }

        [TestMethod]
        public void TestGroupsDBNotNull()
        {
            Assert.IsNotNull(Groups);
        }

        [TestMethod]
        public void CreateGroup()
        {
            Assert.IsTrue(Groups.AddGroup("test1", "heal") != "");
            Assert.IsTrue(Groups.GroupExists("test1"));
            Assert.IsTrue(Tools.GetGroup("test1").HasPermission("heal"));
        }

        [TestMethod]
        public void DeleteGroup()
        {
            Assert.IsTrue(Groups.AddGroup("test1", "heal") != "");
            Assert.IsTrue(Groups.GroupExists("test1"));
            Assert.IsTrue(Groups.DeleteGroup("test1") != "");
            Assert.IsFalse( Groups.GroupExists( "test1") );
        }

        /*[TestMethod]
        public void CreateGroup()
        {
            Assert.IsTrue(Groups.AddGroup("test1", "heal") != "");
            Assert.IsTrue(Groups.GroupExists("test1"));
            Assert.IsTrue(Tools.GetGroup("test1").HasPermission("heal"));
        }*/
    }
}
