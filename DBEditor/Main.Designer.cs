namespace TShockDBEditor
{
    partial class TShockDBEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.itemListBanned = new System.Windows.Forms.ListBox();
            this.itemListAvailable = new System.Windows.Forms.ListBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btn_moveAllLeft = new System.Windows.Forms.Button();
            this.btn_moveAllRight = new System.Windows.Forms.Button();
            this.btn_moveLeft = new System.Windows.Forms.Button();
            this.btn_moveRight = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lst_inheritgrps = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txt_grpname = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btn_deletegroup = new System.Windows.Forms.Button();
            this.btn_newgroup = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_moveAllLeftCmd = new System.Windows.Forms.Button();
            this.btn_moveLeftCmd = new System.Windows.Forms.Button();
            this.btn_moveRightCmd = new System.Windows.Forms.Button();
            this.lst_AvailableCmds = new System.Windows.Forms.ListBox();
            this.btn_moveAllRightCmd = new System.Windows.Forms.Button();
            this.lst_bannedCmds = new System.Windows.Forms.ListBox();
            this.lst_groupList = new System.Windows.Forms.ListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.btn_OpenLocalDB = new System.Windows.Forms.Button();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.btn_connect = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.txt_password = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txt_username = new System.Windows.Forms.TextBox();
            this.txt_port = new System.Windows.Forms.TextBox();
            this.txt_dbname = new System.Windows.Forms.TextBox();
            this.txt_hostname = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lbl_grpchild = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.SuspendLayout();
            // 
            // itemListBanned
            // 
            this.itemListBanned.FormattingEnabled = true;
            this.itemListBanned.Location = new System.Drawing.Point(321, 19);
            this.itemListBanned.Name = "itemListBanned";
            this.itemListBanned.Size = new System.Drawing.Size(275, 290);
            this.itemListBanned.TabIndex = 1;
            // 
            // itemListAvailable
            // 
            this.itemListAvailable.FormattingEnabled = true;
            this.itemListAvailable.Location = new System.Drawing.Point(7, 19);
            this.itemListAvailable.Name = "itemListAvailable";
            this.itemListAvailable.Size = new System.Drawing.Size(275, 290);
            this.itemListAvailable.TabIndex = 2;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Location = new System.Drawing.Point(12, 143);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(610, 407);
            this.tabControl.TabIndex = 3;
            this.tabControl.Visible = false;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btn_moveAllLeft);
            this.tabPage1.Controls.Add(this.btn_moveAllRight);
            this.tabPage1.Controls.Add(this.btn_moveLeft);
            this.tabPage1.Controls.Add(this.btn_moveRight);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.itemListAvailable);
            this.tabPage1.Controls.Add(this.itemListBanned);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(602, 381);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Item Bans";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btn_moveAllLeft
            // 
            this.btn_moveAllLeft.Location = new System.Drawing.Point(288, 193);
            this.btn_moveAllLeft.Name = "btn_moveAllLeft";
            this.btn_moveAllLeft.Size = new System.Drawing.Size(27, 23);
            this.btn_moveAllLeft.TabIndex = 8;
            this.btn_moveAllLeft.Text = "<<";
            this.btn_moveAllLeft.UseVisualStyleBackColor = true;
            this.btn_moveAllLeft.Click += new System.EventHandler(this.btn_moveAllLeftItems_Click);
            // 
            // btn_moveAllRight
            // 
            this.btn_moveAllRight.Location = new System.Drawing.Point(288, 106);
            this.btn_moveAllRight.Name = "btn_moveAllRight";
            this.btn_moveAllRight.Size = new System.Drawing.Size(27, 23);
            this.btn_moveAllRight.TabIndex = 7;
            this.btn_moveAllRight.Text = ">>";
            this.btn_moveAllRight.UseVisualStyleBackColor = true;
            this.btn_moveAllRight.Click += new System.EventHandler(this.btn_moveAllRightItems_Click);
            // 
            // btn_moveLeft
            // 
            this.btn_moveLeft.Location = new System.Drawing.Point(288, 164);
            this.btn_moveLeft.Name = "btn_moveLeft";
            this.btn_moveLeft.Size = new System.Drawing.Size(27, 23);
            this.btn_moveLeft.TabIndex = 6;
            this.btn_moveLeft.Text = "<";
            this.btn_moveLeft.UseVisualStyleBackColor = true;
            this.btn_moveLeft.Click += new System.EventHandler(this.btn_moveLeftItems_Click);
            // 
            // btn_moveRight
            // 
            this.btn_moveRight.Location = new System.Drawing.Point(288, 135);
            this.btn_moveRight.Name = "btn_moveRight";
            this.btn_moveRight.Size = new System.Drawing.Size(27, 23);
            this.btn_moveRight.TabIndex = 5;
            this.btn_moveRight.Text = ">";
            this.btn_moveRight.UseVisualStyleBackColor = true;
            this.btn_moveRight.Click += new System.EventHandler(this.btn_moveRightItems_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(318, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Blacklisted Items";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Whitelisted Items";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lbl_grpchild);
            this.tabPage2.Controls.Add(this.label12);
            this.tabPage2.Controls.Add(this.lst_inheritgrps);
            this.tabPage2.Controls.Add(this.label11);
            this.tabPage2.Controls.Add(this.txt_grpname);
            this.tabPage2.Controls.Add(this.label10);
            this.tabPage2.Controls.Add(this.btn_deletegroup);
            this.tabPage2.Controls.Add(this.btn_newgroup);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.btn_moveAllLeftCmd);
            this.tabPage2.Controls.Add(this.btn_moveLeftCmd);
            this.tabPage2.Controls.Add(this.btn_moveRightCmd);
            this.tabPage2.Controls.Add(this.lst_AvailableCmds);
            this.tabPage2.Controls.Add(this.btn_moveAllRightCmd);
            this.tabPage2.Controls.Add(this.lst_bannedCmds);
            this.tabPage2.Controls.Add(this.lst_groupList);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(602, 381);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Group Manager";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lst_inheritgrps
            // 
            this.lst_inheritgrps.FormattingEnabled = true;
            this.lst_inheritgrps.Location = new System.Drawing.Point(375, 84);
            this.lst_inheritgrps.Name = "lst_inheritgrps";
            this.lst_inheritgrps.Size = new System.Drawing.Size(100, 21);
            this.lst_inheritgrps.TabIndex = 23;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(299, 87);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 13);
            this.label11.TabIndex = 22;
            this.label11.Text = "Inherit From:";
            // 
            // txt_grpname
            // 
            this.txt_grpname.Location = new System.Drawing.Point(375, 55);
            this.txt_grpname.Name = "txt_grpname";
            this.txt_grpname.Size = new System.Drawing.Size(100, 20);
            this.txt_grpname.TabIndex = 21;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(299, 58);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(70, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Group Name:";
            // 
            // btn_deletegroup
            // 
            this.btn_deletegroup.Location = new System.Drawing.Point(215, 82);
            this.btn_deletegroup.Name = "btn_deletegroup";
            this.btn_deletegroup.Size = new System.Drawing.Size(78, 23);
            this.btn_deletegroup.TabIndex = 19;
            this.btn_deletegroup.Text = "Delete Group";
            this.btn_deletegroup.UseVisualStyleBackColor = true;
            this.btn_deletegroup.Click += new System.EventHandler(this.btn_deletegroup_Click);
            // 
            // btn_newgroup
            // 
            this.btn_newgroup.Location = new System.Drawing.Point(215, 53);
            this.btn_newgroup.Name = "btn_newgroup";
            this.btn_newgroup.Size = new System.Drawing.Size(78, 23);
            this.btn_newgroup.TabIndex = 18;
            this.btn_newgroup.Text = "New Group";
            this.btn_newgroup.UseVisualStyleBackColor = true;
            this.btn_newgroup.Click += new System.EventHandler(this.btn_newgroup_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(324, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Whitelisted Commands";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 145);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Banned Commands";
            // 
            // btn_moveAllLeftCmd
            // 
            this.btn_moveAllLeftCmd.Location = new System.Drawing.Point(291, 300);
            this.btn_moveAllLeftCmd.Name = "btn_moveAllLeftCmd";
            this.btn_moveAllLeftCmd.Size = new System.Drawing.Size(27, 23);
            this.btn_moveAllLeftCmd.TabIndex = 15;
            this.btn_moveAllLeftCmd.Text = "<<";
            this.btn_moveAllLeftCmd.UseVisualStyleBackColor = true;
            this.btn_moveAllLeftCmd.Click += new System.EventHandler(this.btn_moveAllLeftCmd_Click);
            // 
            // btn_moveLeftCmd
            // 
            this.btn_moveLeftCmd.Location = new System.Drawing.Point(291, 270);
            this.btn_moveLeftCmd.Name = "btn_moveLeftCmd";
            this.btn_moveLeftCmd.Size = new System.Drawing.Size(27, 23);
            this.btn_moveLeftCmd.TabIndex = 14;
            this.btn_moveLeftCmd.Text = "<";
            this.btn_moveLeftCmd.UseVisualStyleBackColor = true;
            this.btn_moveLeftCmd.Click += new System.EventHandler(this.btn_moveLeftCmd_Click);
            // 
            // btn_moveRightCmd
            // 
            this.btn_moveRightCmd.Location = new System.Drawing.Point(291, 240);
            this.btn_moveRightCmd.Name = "btn_moveRightCmd";
            this.btn_moveRightCmd.Size = new System.Drawing.Size(27, 23);
            this.btn_moveRightCmd.TabIndex = 13;
            this.btn_moveRightCmd.Text = ">";
            this.btn_moveRightCmd.UseVisualStyleBackColor = true;
            this.btn_moveRightCmd.Click += new System.EventHandler(this.btn_moveRightCmd_Click);
            // 
            // lst_AvailableCmds
            // 
            this.lst_AvailableCmds.FormattingEnabled = true;
            this.lst_AvailableCmds.Location = new System.Drawing.Point(324, 161);
            this.lst_AvailableCmds.Name = "lst_AvailableCmds";
            this.lst_AvailableCmds.Size = new System.Drawing.Size(272, 212);
            this.lst_AvailableCmds.TabIndex = 12;
            // 
            // btn_moveAllRightCmd
            // 
            this.btn_moveAllRightCmd.Location = new System.Drawing.Point(291, 210);
            this.btn_moveAllRightCmd.Name = "btn_moveAllRightCmd";
            this.btn_moveAllRightCmd.Size = new System.Drawing.Size(27, 23);
            this.btn_moveAllRightCmd.TabIndex = 11;
            this.btn_moveAllRightCmd.Text = ">>";
            this.btn_moveAllRightCmd.UseVisualStyleBackColor = true;
            this.btn_moveAllRightCmd.Click += new System.EventHandler(this.btn_moveAllRightCmd_Click);
            // 
            // lst_bannedCmds
            // 
            this.lst_bannedCmds.FormattingEnabled = true;
            this.lst_bannedCmds.Location = new System.Drawing.Point(9, 161);
            this.lst_bannedCmds.Name = "lst_bannedCmds";
            this.lst_bannedCmds.Size = new System.Drawing.Size(275, 212);
            this.lst_bannedCmds.TabIndex = 10;
            // 
            // lst_groupList
            // 
            this.lst_groupList.FormattingEnabled = true;
            this.lst_groupList.Location = new System.Drawing.Point(9, 8);
            this.lst_groupList.Name = "lst_groupList";
            this.lst_groupList.Size = new System.Drawing.Size(200, 134);
            this.lst_groupList.TabIndex = 0;
            this.lst_groupList.SelectedIndexChanged += new System.EventHandler(this.lst_groupList_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(610, 125);
            this.tabControl1.TabIndex = 6;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.btn_OpenLocalDB);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(602, 99);
            this.tabPage3.TabIndex = 0;
            this.tabPage3.Text = "Local Database (SQLite)";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // btn_OpenLocalDB
            // 
            this.btn_OpenLocalDB.Location = new System.Drawing.Point(9, 6);
            this.btn_OpenLocalDB.Name = "btn_OpenLocalDB";
            this.btn_OpenLocalDB.Size = new System.Drawing.Size(96, 23);
            this.btn_OpenLocalDB.TabIndex = 0;
            this.btn_OpenLocalDB.Text = "Open Database";
            this.btn_OpenLocalDB.UseVisualStyleBackColor = true;
            this.btn_OpenLocalDB.Click += new System.EventHandler(this.btn_OpenLocalDB_Click);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.btn_connect);
            this.tabPage4.Controls.Add(this.label9);
            this.tabPage4.Controls.Add(this.txt_password);
            this.tabPage4.Controls.Add(this.label8);
            this.tabPage4.Controls.Add(this.txt_username);
            this.tabPage4.Controls.Add(this.txt_port);
            this.tabPage4.Controls.Add(this.txt_dbname);
            this.tabPage4.Controls.Add(this.txt_hostname);
            this.tabPage4.Controls.Add(this.label7);
            this.tabPage4.Controls.Add(this.label6);
            this.tabPage4.Controls.Add(this.label5);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(602, 99);
            this.tabPage4.TabIndex = 1;
            this.tabPage4.Text = "Remote Database (MySql)";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // btn_connect
            // 
            this.btn_connect.Location = new System.Drawing.Point(213, 63);
            this.btn_connect.Name = "btn_connect";
            this.btn_connect.Size = new System.Drawing.Size(75, 23);
            this.btn_connect.TabIndex = 10;
            this.btn_connect.Text = "Connect";
            this.btn_connect.UseVisualStyleBackColor = true;
            this.btn_connect.Click += new System.EventHandler(this.btn_connect_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(210, 36);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 13);
            this.label9.TabIndex = 9;
            this.label9.Text = "Password:";
            // 
            // txt_password
            // 
            this.txt_password.Location = new System.Drawing.Point(272, 33);
            this.txt_password.Name = "txt_password";
            this.txt_password.Size = new System.Drawing.Size(100, 20);
            this.txt_password.TabIndex = 8;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(208, 10);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(58, 13);
            this.label8.TabIndex = 7;
            this.label8.Text = "Username:";
            // 
            // txt_username
            // 
            this.txt_username.Location = new System.Drawing.Point(272, 7);
            this.txt_username.Name = "txt_username";
            this.txt_username.Size = new System.Drawing.Size(100, 20);
            this.txt_username.TabIndex = 6;
            // 
            // txt_port
            // 
            this.txt_port.Location = new System.Drawing.Point(102, 60);
            this.txt_port.Name = "txt_port";
            this.txt_port.Size = new System.Drawing.Size(100, 20);
            this.txt_port.TabIndex = 5;
            this.txt_port.Text = "3306";
            // 
            // txt_dbname
            // 
            this.txt_dbname.Location = new System.Drawing.Point(102, 33);
            this.txt_dbname.Name = "txt_dbname";
            this.txt_dbname.Size = new System.Drawing.Size(100, 20);
            this.txt_dbname.TabIndex = 4;
            // 
            // txt_hostname
            // 
            this.txt_hostname.Location = new System.Drawing.Point(102, 7);
            this.txt_hostname.Name = "txt_hostname";
            this.txt_hostname.Size = new System.Drawing.Size(100, 20);
            this.txt_hostname.TabIndex = 3;
            this.txt_hostname.Text = "localhost";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(70, 63);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(26, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Port";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Database Name:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Hostname:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(215, 8);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 13);
            this.label12.TabIndex = 24;
            this.label12.Text = "Group Child:";
            // 
            // lbl_grpchild
            // 
            this.lbl_grpchild.AutoSize = true;
            this.lbl_grpchild.Location = new System.Drawing.Point(286, 8);
            this.lbl_grpchild.Name = "lbl_grpchild";
            this.lbl_grpchild.Size = new System.Drawing.Size(16, 13);
            this.lbl_grpchild.TabIndex = 25;
            this.lbl_grpchild.Text = "   ";
            // 
            // TShockDBEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 562);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.tabControl);
            this.Name = "TShockDBEditor";
            this.Text = "TShockDBEditor";
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tabPage4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox itemListBanned;
        private System.Windows.Forms.ListBox itemListAvailable;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_moveAllLeft;
        private System.Windows.Forms.Button btn_moveAllRight;
        private System.Windows.Forms.Button btn_moveLeft;
        private System.Windows.Forms.Button btn_moveRight;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Button btn_OpenLocalDB;
        private System.Windows.Forms.Button btn_moveAllLeftCmd;
        private System.Windows.Forms.Button btn_moveLeftCmd;
        private System.Windows.Forms.Button btn_moveRightCmd;
        private System.Windows.Forms.ListBox lst_AvailableCmds;
        private System.Windows.Forms.Button btn_moveAllRightCmd;
        private System.Windows.Forms.ListBox lst_bannedCmds;
        private System.Windows.Forms.ListBox lst_groupList;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btn_connect;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txt_password;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txt_username;
        private System.Windows.Forms.TextBox txt_port;
        private System.Windows.Forms.TextBox txt_dbname;
        private System.Windows.Forms.TextBox txt_hostname;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btn_deletegroup;
        private System.Windows.Forms.Button btn_newgroup;
        private System.Windows.Forms.ComboBox lst_inheritgrps;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txt_grpname;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lbl_grpchild;
        private System.Windows.Forms.Label label12;
    }
}

