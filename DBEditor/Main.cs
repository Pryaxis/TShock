using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using Community.CsharpSqlite.SQLiteClient;
using MySql.Data.MySqlClient;

using System.Windows.Forms;

namespace TShockDBEditor
{
    public partial class TShockDBEditor : Form
    {
        public OpenFileDialog dialog = new OpenFileDialog();
        public List<Group> groups = new List<Group>();
        public IDbConnection DB;
        public string dbtype = "";

        public TShockDBEditor()
        {
            InitializeComponent();
            Itemlist.AddItems();
            Commandlist.AddCommands();
            dialog.FileOk += new CancelEventHandler(dialog_FileOk);
            dialog.Filter = "SQLite Database (*.sqlite)|*.sqlite";
        }

        public void LoadSqliteDatabase(string path)
        {
            string sql = dialog.FileName;
            DB = new SqliteConnection(string.Format("uri=file://{0},Version=3", sql));
            DB.Open();
            dbtype = "sqlite";
            itemListBanned.Items.Clear();
            lst_groupList.Items.Clear();
            lst_AvailableCmds.Items.Clear();
            lst_bannedCmds.Items.Clear();
            itemListAvailable.Items.Clear();

            using (var com = DB.CreateCommand())
            {
                com.CommandText =
                    "SELECT * FROM Itembans";

                using (var reader = com.ExecuteReader())
                {
                    while (reader.Read())
                        itemListBanned.Items.Add(reader.Get<string>("ItemName"));
                }
            }
            using (var com = DB.CreateCommand())
            {
                com.CommandText =
                    "SELECT * FROM GroupList";

                using (var reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lst_groupList.Items.Add(reader.Get<string>("GroupName"));
                        lst_inheritgrps.Items.Add(reader.Get<string>("GroupName"));
                    }
                }
            }

            for (int i = 0; i < Itemlist.ItemList.Count; i++)
            {
                if (!itemListBanned.Items.Contains(Itemlist.ItemList[i]))
                    itemListAvailable.Items.Add(Itemlist.ItemList[i]);
            }
        }

        public void LoadMySqlDatabase(string hostname = "localhost", string port = "3306", string database = "", string username = "", string password = "")
        {
            DB = new MySqlConnection();
            DB.ConnectionString =
                "Server='" + hostname +
                "';Port='" + port +
                "';Database='" + database +
                "';Uid='" + username +
                "';Pwd='" + password + "';";
            DB.Open();
            dbtype = "mysql";
            itemListBanned.Items.Clear();
            lst_groupList.Items.Clear();
            lst_AvailableCmds.Items.Clear();
            lst_bannedCmds.Items.Clear();
            itemListAvailable.Items.Clear();

            using (var com = DB.CreateCommand())
            {
                com.CommandText =
                    "SELECT * FROM Itembans";

                using (var reader = com.ExecuteReader())
                {
                    while (reader.Read())
                        itemListBanned.Items.Add(reader.Get<string>("ItemName"));
                }
            }
            using (var com = DB.CreateCommand())
            {
                com.CommandText =
                    "SELECT * FROM GroupList";

                using (var reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lst_groupList.Items.Add(reader.Get<string>("GroupName"));
                    }
                }
            }

            for (int i = 0; i < Itemlist.ItemList.Count; i++)
            {
                if (!itemListBanned.Items.Contains(Itemlist.ItemList[i]))
                    itemListAvailable.Items.Add(Itemlist.ItemList[i]);
            }
        }

        #region BannedItemsTab
        public void btn_moveAllRightItems_Click(object sender, EventArgs e)
        {
            foreach (object item in itemListAvailable.Items)
            {
                itemListBanned.Items.Add(item);

                using (var com = DB.CreateCommand())
                {
                    com.CommandText = "INSERT INTO ItemBans (ItemName) VALUES (@itemname);";
                    com.AddParameter("@itemname", item.ToString());
                    com.ExecuteNonQuery();
                    com.Parameters.Clear();
                }
            }
            itemListAvailable.Items.Clear();
        }

        private void btn_moveAllLeftItems_Click(object sender, EventArgs e)
        {
            foreach (object item in itemListBanned.Items)
            {
                itemListAvailable.Items.Add(item);

                using (var com = DB.CreateCommand())
                {
                    com.CommandText = "DELETE FROM ItemBans WHERE ItemName=@itemname;";
                    com.AddParameter("@itemname", item.ToString());
                    com.ExecuteNonQuery();
                    com.Parameters.Clear();
                }
            }
            itemListBanned.Items.Clear();
        }

        private void btn_moveRightItems_Click(object sender, EventArgs e)
        {
            if (itemListAvailable.SelectedItem != null)
            {
                itemListBanned.Items.Add(itemListAvailable.SelectedItem);

                using (var com = DB.CreateCommand())
                {
                    com.CommandText = "INSERT INTO ItemBans (ItemName) VALUES (@itemname);";
                    com.AddParameter("@itemname", itemListAvailable.SelectedItem.ToString());
                    com.ExecuteNonQuery();
                }

                itemListAvailable.Items.Remove(itemListAvailable.SelectedItem);
            }
        }

        private void btn_moveLeftItems_Click(object sender, EventArgs e)
        {
            if (itemListBanned.SelectedItem != null)
            {
                itemListAvailable.Items.Add(itemListBanned.SelectedItem);

                using (var com = DB.CreateCommand())
                {
                    com.CommandText = "DELETE FROM ItemBans WHERE ItemName=@itemname;";
                    com.AddParameter("@itemname", itemListBanned.SelectedItem.ToString());
                    com.ExecuteNonQuery();
                }

                itemListBanned.Items.Remove(itemListBanned.SelectedItem);
            }
        }
        #endregion

        #region GroupTab

        private void lst_groupList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateGroupIndex(lst_groupList.SelectedIndex);
        }

        private void UpdateGroupIndex(int index)
        {
            try
            {
                using (var com = DB.CreateCommand())
                {
                    lst_AvailableCmds.Items.Clear();
                    lst_bannedCmds.Items.Clear();

                    com.CommandText =
                        "SELECT * FROM GroupList WHERE GroupName=@groupname";
                    com.AddParameter("@groupname", lst_groupList.Items[index].ToString());

                    lbl_grpchild.Text = "";

                    using (var reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            foreach (string command in reader.Get<string>("Commands").Split(','))
                            {
                                if (lst_groupList.Items.Contains(command) || command == "")
                                    lbl_grpchild.Text = command;
                                else
                                    lst_AvailableCmds.Items.Add(command);
                            }
                        }
                    }

                    if (lbl_grpchild.Text == "")
                        lbl_grpchild.Text = "none";

                    for (int i = 0; i < Commandlist.CommandList.Count; i++)
                    {
                        if (!lst_AvailableCmds.Items.Contains(Commandlist.CommandList[i]))
                            lst_bannedCmds.Items.Add(Commandlist.CommandList[i]);
                    }
                }
            }
            catch { }
        }

        private void btn_moveAllRightCmd_Click(object sender, EventArgs e)
        {
            try
            {
                var sb = new StringBuilder();

                foreach (object cmd in lst_bannedCmds.Items)
                {
                    lst_AvailableCmds.Items.Add(cmd);

                    if (string.IsNullOrEmpty(sb.ToString()))
                        sb.Append(cmd.ToString());
                    else
                        sb.Append(",").Append(cmd.ToString());
                }

                using (var com = DB.CreateCommand())
                {
                    com.CommandText = "UPDATE GroupList SET Commands=@cmds WHERE GroupName=@name;";
                    com.AddParameter("@name", lst_groupList.Items[lst_groupList.SelectedIndex].ToString());
                    com.AddParameter("@cmds", sb.ToString());
                    com.ExecuteNonQuery();
                }
                lst_bannedCmds.Items.Clear();
            }
            catch { }
        }

        private void btn_moveRightCmd_Click(object sender, EventArgs e)
        {
            try
            {
                lst_AvailableCmds.Items.Add(lst_bannedCmds.Items[lst_bannedCmds.SelectedIndex]);
                var sb = new StringBuilder();

                foreach (object cmd in lst_AvailableCmds.Items)
                {
                    if (string.IsNullOrEmpty(sb.ToString()))
                        sb.Append(cmd.ToString());
                    else
                        sb.Append(",").Append(cmd.ToString());
                }

                using (var com = DB.CreateCommand())
                {
                    com.CommandText = "UPDATE GroupList SET Commands=@cmds WHERE GroupName=@name;";
                    com.AddParameter("@name", lst_groupList.Items[lst_groupList.SelectedIndex].ToString());
                    com.AddParameter("@cmds", sb.ToString());
                    com.ExecuteNonQuery();
                }

                lst_bannedCmds.Items.Remove(lst_bannedCmds.Items[lst_bannedCmds.SelectedIndex]);
            }
            catch { }
        }

        private void btn_moveLeftCmd_Click(object sender, EventArgs e)
        {
            try
            {
                lst_bannedCmds.Items.Add(lst_AvailableCmds.Items[lst_AvailableCmds.SelectedIndex]);
                lst_AvailableCmds.Items.Remove(lst_AvailableCmds.Items[lst_AvailableCmds.SelectedIndex]);
                var sb = new StringBuilder();

                foreach (object cmd in lst_AvailableCmds.Items)
                {
                    if (string.IsNullOrEmpty(sb.ToString()))
                        sb.Append(cmd.ToString());
                    else
                        sb.Append(",").Append(cmd.ToString());
                }

                using (var com = DB.CreateCommand())
                {
                    com.CommandText = "UPDATE GroupList SET Commands=@cmds WHERE GroupName=@name;";
                    com.AddParameter("@name", lst_groupList.Items[lst_groupList.SelectedIndex].ToString());
                    com.AddParameter("@cmds", sb.ToString());
                    com.ExecuteNonQuery();
                }
            }
            catch { }
        }

        private void btn_moveAllLeftCmd_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (object cmd in lst_AvailableCmds.Items)
                {
                    lst_bannedCmds.Items.Add(cmd);
                }

                using (var com = DB.CreateCommand())
                {
                    com.CommandText = "UPDATE GroupList SET Commands=@cmds WHERE GroupName=@name;";
                    com.AddParameter("@name", lst_groupList.Items[lst_groupList.SelectedIndex].ToString());
                    com.AddParameter("@cmds", "");
                    com.ExecuteNonQuery();
                }

                lst_AvailableCmds.Items.Clear();
            }
            catch { }
        }

        private void btn_newgroup_Click(object sender, EventArgs e)
        {
            try
            {
                if (txt_grpname.Text != "")
                {
                    using (var com = DB.CreateCommand())
                    {
                        if (dbtype == "sqlite")
                            com.CommandText = "INSERT OR IGNORE INTO GroupList (GroupName, Commands) VALUES (@groupname, @commands);";
                        else if (dbtype == "mysql")
                            com.CommandText = "INSERT IGNORE INTO GroupList SET GroupName=@groupname, Commands=@commands;";
                        com.AddParameter("@groupname", txt_grpname.Text);

                        if(lst_inheritgrps.SelectedIndex > -1)
                            com.AddParameter("@commands", lst_inheritgrps.Items[lst_inheritgrps.SelectedIndex]);
                        else
                            com.AddParameter("@commands", "");

                        using (var reader = com.ExecuteReader())
                        {
                            if (reader.RecordsAffected > 0)
                            {
                                lst_groupList.Items.Add(txt_grpname.Text);
                                lst_inheritgrps.Items.Add(txt_grpname.Text);
                                txt_grpname.Text = "";
                            }
                        }

                        com.Parameters.Clear();
                    }
                }
            }
            catch { }
        }

        private void btn_deletegroup_Click(object sender, EventArgs e)
        {
            try
            {
                using (var com = DB.CreateCommand())
                {
                    com.CommandText = "DELETE FROM Grouplist WHERE GroupName = @groupname";
                    com.AddParameter("@groupname", lst_groupList.Items[lst_groupList.SelectedIndex]);
                    com.ExecuteNonQuery();

                    lst_groupList.Items.Remove(lst_groupList.Items[lst_groupList.SelectedIndex]);

                    lst_inheritgrps.Items.Clear();
                    com.CommandText =
                    "SELECT * FROM GroupList";
                    using (var reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lst_inheritgrps.Items.Add(reader.Get<string>("GroupName"));
                        }
                    }
                }
            }
            catch { }
        }

        #endregion

        #region FileOpenTabs

        private void btn_OpenLocalDB_Click(object sender, EventArgs e)
        {
            dialog.ShowDialog();
        }

        void dialog_FileOk(object sender, CancelEventArgs e)
        {
            LoadSqliteDatabase(dialog.FileName);
            tabControl.Visible = true;
        }

        private void btn_connect_Click(object sender, EventArgs e)
        {
            LoadMySqlDatabase(txt_hostname.Text, txt_port.Text, txt_dbname.Text, txt_username.Text, txt_password.Text);
            tabControl.Visible = true;
        }

        #endregion        
    }
}