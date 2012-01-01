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
using System.Data;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Mono.Data.Sqlite;
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
            Assert.IsTrue(TShock.Utils.GetGroup("test1").HasPermission("heal"));
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
