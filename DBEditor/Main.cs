using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
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

        public void LoadDB()
        {

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

                lst_groupList.Items.Add("superadmin");
                lst_usergrplist.Items.Add("superadmin");
                lst_newusergrplist.Items.Add("superadmin");
                using (var reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lst_groupList.Items.Add(reader.Get<string>("GroupName"));
                        lst_inheritgrps.Items.Add(reader.Get<string>("GroupName"));
                        lst_usergrplist.Items.Add(reader.Get<string>("GroupName"));
                        lst_newusergrplist.Items.Add(reader.Get<string>("GroupName"));
                    }
                }
            }
            using (var com = DB.CreateCommand())
            {
                com.CommandText =
                    "SELECT * FROM Users";

                using (var reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.Get<string>("UserName") != "")
                            lst_userlist.Items.Add(reader.Get<string>("UserName"));
                        else
                            lst_userlist.Items.Add(reader.Get<string>("IP"));
                    }
                }
            }
            using (var com = DB.CreateCommand())
            {
                com.CommandText =
                    "SELECT * FROM Bans";

                using (var reader = com.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lst_bans.Items.Add(reader.Get<string>("Name"));
                    }
                }
            }

            for (int i = 0; i < Itemlist.ItemList.Count; i++)
            {
                if (!itemListBanned.Items.Contains(Itemlist.ItemList[i]))
                    itemListAvailable.Items.Add(Itemlist.ItemList[i]);
            }
        }

        public void LoadSqliteDatabase(string path)
        {
            string sql = dialog.FileName;
            DB = new SqliteConnection(string.Format("uri=file://{0},Version=3", sql));
            DB.Open();
            dbtype = "sqlite";
            LoadDB();
        }

        public void LoadMySqlDatabase(string hostname = "localhost", string port = "3306", string database = "", string dbusername = "", string dbpassword = "")
        {
            DB = new MySqlConnection();
            DB.ConnectionString =
                "Server='" + hostname +
                "';Port='" + port +
                "';Database='" + database +
                "';Uid='" + dbusername +
                "';Pwd='" + dbpassword + "';";
            DB.Open();
            dbtype = "mysql";
            LoadDB();
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

                        if (lst_inheritgrps.SelectedIndex > -1)
                            com.AddParameter("@commands", lst_inheritgrps.Items[lst_inheritgrps.SelectedIndex]);
                        else
                            com.AddParameter("@commands", "");

                        using (var reader = com.ExecuteReader())
                        {
                            if (reader.RecordsAffected > 0)
                            {
                                lst_groupList.Items.Add(txt_grpname.Text);
                                lst_inheritgrps.Items.Add(txt_grpname.Text);
                                lst_usergrplist.Items.Add(txt_grpname.Text);
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
                    lst_usergrplist.Items.Clear();
                    com.CommandText =
                    "SELECT * FROM GroupList";
                    using (var reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lst_inheritgrps.Items.Add(reader.Get<string>("GroupName"));
                            lst_usergrplist.Items.Add(reader.Get<string>("GroupName"));
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
            LoadMySqlDatabase(txt_hostname.Text, txt_port.Text, txt_dbname.Text, txt_dbusername.Text, txt_dbpassword.Text);
            tabControl.Visible = true;
        }

        #endregion

        #region BansTab

        private void lst_bans_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (var com = DB.CreateCommand())
            {
                if (lst_bans.SelectedIndex > -1)
                {
                    com.CommandText =
                        "SELECT * FROM Bans WHERE Name=@name";
                    com.AddParameter("@name", lst_bans.Items[lst_bans.SelectedIndex]);

                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txt_banip.Text = reader.Get<string>("IP");
                            txt_banname.Text = reader.Get<string>("Name");
                            txt_banreason.Text = reader.Get<string>("Reason");
                        }
                    }
                }
            }
        }

        private void btn_bandelete_Click(object sender, EventArgs e)
        {
            using (var com = DB.CreateCommand())
            {
                com.CommandText =
                    "DELETE FROM Bans WHERE IP=@ip";
                com.AddParameter("@ip", txt_banip.Text);

                using (var reader = com.ExecuteReader())
                {
                    if (reader.RecordsAffected > 0)
                    {
                        txt_banip.Text = "";
                        txt_banname.Text = "";
                        txt_banreason.Text = "";
                        lst_bans.Items.Remove(lst_bans.Items[lst_bans.SelectedIndex]);
                    }
                }
            }
        }

        private void btn_bannew_Click(object sender, EventArgs e)
        {
            using (var com = DB.CreateCommand())
            {
                if (dbtype == "sqlite")
                    com.CommandText = "INSERT INTO Bans (IP, Name, Reason) VALUES (@ip, @name, @reason);";
                else if (dbtype == "mysql")
                    com.CommandText = "INSERT INTO Bans SET IP=@ip, Name=@name, Reason=@reason;";
                if (txt_newbanip.Text != "" && txt_newbanname.Text != "")
                {
                    com.AddParameter("@ip", txt_newbanip.Text);
                    com.AddParameter("@name", txt_newbanname.Text);
                    com.AddParameter("@reason", txt_newbanreason.Text);
                    com.ExecuteNonQuery();

                    lst_bans.Items.Add(txt_newbanname.Text);
                    txt_newbanip.Text = "";
                    txt_newbanname.Text = "";
                    txt_newbanreason.Text = "";                    
                    lbl_newbanstatus.Text = "Ban added successfully!";
                }
                else
                    lbl_newbanstatus.Text = "Required field('s) empty";

                lst_bans.Items.Add(txt_newbanname.Text);
                txt_newbanip.Text = "";
                txt_newbanname.Text = "";
            }
        }

        private void txt_banreason_TextChanged(object sender, EventArgs e)
        {
            if (txt_banip.Text != "")
            {
                using (var com = DB.CreateCommand())
                {
                    com.CommandText =
                        "UPDATE Bans SET Reason=@reason WHERE IP=@ip";
                    com.AddParameter("@reason", txt_banreason.Text);
                    com.AddParameter("@ip", txt_banip.Text);
                    com.ExecuteNonQuery();
                }
            }
        }

        #endregion

        private void lst_userlist_SelectedIndexChanged(object sender, EventArgs e)
        {
            txt_username.Text = "None Selected";
            txt_userip.Text = "None Selected";
            lst_usergrplist.SelectedIndex = -1;
            bool flag = false;


            if (lst_userlist.SelectedIndex > -1)
            {
                using (var com = DB.CreateCommand())
                {
                    com.CommandText =
                        "SELECT * FROM Users WHERE Username=@name";
                    com.AddParameter("@name", lst_userlist.Items[lst_userlist.SelectedIndex]);

                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txt_username.Text = reader.Get<string>("Username");

                            if (reader.Get<string>("IP") != "")
                                txt_userip.Text = reader.Get<string>("IP");
                            else
                                txt_userip.Text = "User does not have IP";

                            if (reader.Get<string>("Password") != "")
                                txt_userpass.Text = reader.Get<string>("Password");
                            else
                                txt_userpass.Text = "User does not have Pasword";

                            foreach (string name in lst_usergrplist.Items)
                            {
                                if (name == reader.Get<string>("Usergroup"))
                                {
                                    lst_usergrplist.SelectedItem = name;
                                    break;
                                }
                            }
                        }
                        else
                            flag = true;
                    }
                }

                if (flag)
                {
                    using (var com = DB.CreateCommand())
                    {
                        com.CommandText =
                            "SELECT * FROM Users WHERE IP=@ip";
                        com.AddParameter("@ip", lst_userlist.Items[lst_userlist.SelectedIndex]);

                        using (var reader = com.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader.Get<string>("Password") != "")
                                    txt_userpass.Text = reader.Get<string>("Password");
                                else
                                    txt_userpass.Text = "User does not have Pasword";

                                txt_userip.Text = lst_userlist.Items[lst_userlist.SelectedIndex].ToString();

                                foreach (string name in lst_usergrplist.Items)
                                {
                                    if (name == reader.Get<string>("Usergroup"))
                                    {
                                        lst_usergrplist.SelectedItem = name;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void lst_usergrplist_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lst_userlist.SelectedIndex > -1)
            {
                using (var com = DB.CreateCommand())
                {
                    com.CommandText =
                        "UPDATE Users SET Usergroup=@group WHERE Username=@user OR IP=@ip";
                    com.AddParameter("@group", lst_usergrplist.SelectedItem);
                    com.AddParameter("@user", txt_username.Text);
                    com.AddParameter("@ip", txt_userip.Text);
                    com.ExecuteNonQuery();
                }
            }
        }

        private void btn_deluser_Click(object sender, EventArgs e)
        {
            if (lst_userlist.SelectedIndex > -1)
            {
                using (var com = DB.CreateCommand())
                {
                    com.CommandText =
                        "DELETE FROM Users WHERE IP=@ip OR Username=@user";
                    com.AddParameter("@user", txt_username.Text);
                    com.AddParameter("@ip", txt_userip.Text);
                    com.ExecuteNonQuery();
                    lst_userlist.Items.Remove(lst_userlist.SelectedItem);
                    txt_userip.Text = "";
                    txt_username.Text = "";
                    txt_userpass.Text = "";
                    lst_usergrplist.SelectedIndex = -1;
                    lst_userlist.SelectedIndex = -1;
                }
            }
        }

        private void btn_adduser_Click(object sender, EventArgs e)
        {
            lbl_useraddstatus.Visible = true;

            for (int i = 0; i == 0; i++)
            {

                if ((txt_newusername.Text == "" && txt_newuserpass.Text != ""))
                {
                    lbl_useraddstatus.Text = "Username field cannot be empty";
                    break;
                }

                if ((txt_newusername.Text != "" && txt_newuserpass.Text == ""))
                {
                    lbl_useraddstatus.Text = "Password field cannot be empty";
                    break;
                }

                if (txt_newuserip.Text == "" && (txt_newusername.Text == "" || txt_newuserpass.Text == ""))
                {
                    lbl_useraddstatus.Text = "IP field cannot be empty";
                    break;
                }

                if (lst_newusergrplist.SelectedIndex == -1)
                {
                    lbl_useraddstatus.Text = "Group must be selected";
                    break;
                }

                using (var com = DB.CreateCommand())
                {
                    com.CommandText = "INSERT INTO Users (Username, Password, UserGroup, IP) VALUES (@name, @password, @group, @ip);";
                    com.AddParameter("@name", txt_newusername.Text);
                    com.AddParameter("@password", HashPassword(txt_newuserpass.Text));

                    com.AddParameter("@group", lst_newusergrplist.SelectedItem);
                    com.AddParameter("@ip", txt_newuserip.Text);

                    using (var reader = com.ExecuteReader())
                    {
                        if (reader.RecordsAffected > 0)
                        {
                            if (txt_newusername.Text != "")
                                lst_userlist.Items.Add(txt_newusername.Text);
                            else
                                lst_userlist.Items.Add(txt_newuserip.Text);

                            txt_newuserip.Text = "";
                            txt_newusername.Text = "";
                            txt_newuserpass.Text = "";
                            lst_newusergrplist.SelectedIndex = -1;

                            lbl_useraddstatus.Text = "User added successfully!";
                        }
                        else
                            lbl_useraddstatus.Text = "SQL error while adding user";
                    }
                }

            }
        }

        public static string HashPassword(string password)
        {
            using (var sha = new SHA512CryptoServiceProvider())
            {
                if (password == "")
                {
                    return "nonexistent-password";
                }
                var bytes = sha.ComputeHash(Encoding.ASCII.GetBytes(password));
                return bytes.Aggregate("", (s, b) => s + b.ToString("X2"));
            }
        }

        private void TShockDBEditor_Load(object sender, EventArgs e)
        {

        }
    }
}