/*
 * Copyright (c) 2010-2020 Yucel Guven
 * All rights reserved.
 * 
 * This file is part of IPv6 Subnetting Tool.
 * 
 * Version: 4.5
 * Release Date: 16 April 2020
 *  
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted (subject to the limitations in the
 * disclaimer below) provided that the following conditions are met:
 * 
 * 1. Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 * notice, this list of conditions and the following disclaimer in the
 * documentation and/or other materials provided with the distribution.
 * 
 * NO EXPRESS OR IMPLIED LICENSES TO ANY PARTY'S PATENT RIGHTS ARE
 * GRANTED BY THIS LICENSE. THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS 
 * AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, 
 * BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS 
 * FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER
 * OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY,
 * OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
 * STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
 * OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.Globalization;
using Microsoft.Win32;
using System.Data.Odbc;
using System.Data;

namespace IPv6SubnettingTool
{
    public partial class DBinfo : Form
    {
        #region special variables - yucel
        public DBServerInfo ServerInfo = new DBServerInfo();
        DBServerInfo inputServerInfo = new DBServerInfo();
        OdbcConnection MySQLconnection = null;
        string currentMode = "";
        public CultureInfo culture;
        public delegate void ChangeWinFormStringsDelegate(CultureInfo culture);
        public event ChangeWinFormStringsDelegate ChangeUILanguage = delegate { };
        #endregion

        public DBinfo(CultureInfo culture, DBServerInfo servinfo, OdbcConnection sqlcon, string mode)
        {
            InitializeComponent();

            this.culture = culture;
            this.SwitchLanguage(this.culture);
            
            this.inputServerInfo = servinfo;
            this.ServerInfo = servinfo.ShallowCopy();
            this.currentMode = mode;
            
            this.MySQLconnection = sqlcon;

            if (sqlcon != null)
            {
                this.label16.Text = "Yes";
                this.label14.Text = sqlcon.Database;
            }
            else
            {
                this.label16.Text = "null";
                this.label14.Text = "null";
            }

            this.label10.Text = this.label11.Text = this.label12.Text = this.currentMode;

            if (this.inputServerInfo.DriverName != "" || this.inputServerInfo.DriverName != null)
            {
                this.comboBox1.Items.Add(this.inputServerInfo.DriverName);
                this.comboBox1.SelectedItem = this.comboBox1.Items[0];
            }
            if (this.inputServerInfo.ServerIP != null)
                this.textBox1.Text = this.inputServerInfo.ServerIP.ToString();
            if (this.inputServerInfo.PortNum.ToString() != null)
                this.textBox7.Text = this.inputServerInfo.PortNum.ToString();

            if (this.currentMode == "v6")
            {
                if (this.inputServerInfo.DBname != "" || this.inputServerInfo.DBname != null)
                    this.textBox2.Text = this.inputServerInfo.DBname;
                if (this.inputServerInfo.Tablename != "" || this.inputServerInfo.Tablename != null)
                    this.textBox5.Text = this.inputServerInfo.Tablename;
            }
            else // v4
            {
                if (this.inputServerInfo.DBname_v4 != "" || this.inputServerInfo.DBname_v4 != null)
                    this.textBox2.Text = this.inputServerInfo.DBname_v4;
                if (this.inputServerInfo.Tablename_v4 != "" || this.inputServerInfo.Tablename_v4 != null)
                    this.textBox5.Text = this.inputServerInfo.Tablename_v4;
            }
            
            if (this.inputServerInfo.Username != null || this.inputServerInfo.Username != "")
                this.textBox3.Text = this.inputServerInfo.Username;
            
            if (this.inputServerInfo.Password != null || this.inputServerInfo.Password != "")
                this.textBox4.Text = this.inputServerInfo.Password;

            if (this.MySQLconnection != null)
            {
                if (this.MySQLconnection.State == System.Data.ConnectionState.Open)
                {
                    this.button3.Enabled = false;
                    this.comboBox1.Enabled = false;
                    this.textBox1.Enabled = false;
                    this.textBox7.Enabled = false;
                    this.textBox3.Enabled = false;
                    this.textBox4.Enabled = false;
                }
            }
            else
            {
                this.button3.Enabled = true;
                this.comboBox1.Enabled = true;
                this.textBox1.Enabled = true;
                this.textBox7.Enabled = true;
                this.textBox3.Enabled = true;
                this.textBox4.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e) // Connect
        {
            /* Check all info: */

            if (this.comboBox1.SelectedItem != null)
            {
                if (this.comboBox1.SelectedItem.ToString().Trim() != "")
                {
                    this.ServerInfo.DriverName = this.comboBox1.SelectedItem.ToString().Trim();
                }
            }
            else
            {
                this.comboBox1.BackColor = Color.Yellow;
                return;
            }

            try
            {
                if (this.textBox1.Text.Trim() == "") // IP Addr
                {
                    this.textBox1.BackColor = Color.Yellow;
                    return;
                }

                IPHostEntry hostent = null;
                IPAddress ipaddr = null;

                if (!IPAddress.TryParse(this.textBox1.Text.Trim(), out ipaddr))
                {
                    hostent = Dns.GetHostEntry(this.textBox1.Text.Trim());
                    this.ServerInfo.ServerIP = hostent.AddressList[0];
                }
                else
                {
                    this.ServerInfo.ServerIP = ipaddr;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.textBox1.BackColor = Color.Yellow;
                return;
            }

            if (!UInt16.TryParse(this.textBox7.Text.Trim(), out this.ServerInfo.PortNum))  // Port Num
            {
                this.textBox7.BackColor = Color.Yellow;
                return;
            }

            if (this.textBox2.Text.Trim() == "") // Database name
            {
                this.textBox2.BackColor = Color.Yellow;
                return;
            }
            if (this.textBox5.Text.Trim() == "") // Tablename
            {
                this.textBox5.BackColor = Color.Yellow;
                return;
            }

            if (this.textBox3.Text.Trim() == "") // username
            {
                this.textBox3.BackColor = Color.Yellow;
                return;
            }
            else
            {
                this.ServerInfo.Username = this.textBox3.Text.Trim();
            }

            if (this.textBox4.Text == "") // password
            {
                this.textBox4.BackColor = Color.Yellow;
                return;
            }
            else
            {
                this.ServerInfo.Password = this.textBox4.Text;
            }

            string db = "";

            if (this.currentMode == "v6")
            {
                this.ServerInfo.DBname = this.textBox2.Text.Trim();
                this.ServerInfo.Tablename = this.textBox5.Text.Trim();
                db = this.ServerInfo.DBname;
            }
            else // v4
            {
                this.ServerInfo.DBname_v4 = this.textBox2.Text.Trim();
                this.ServerInfo.Tablename_v4 = this.textBox5.Text.Trim();
                db = this.ServerInfo.DBname_v4;
            }

            this.ServerInfo.ConnectionString =
                //"Driver={MySQL ODBC 5.3 Unicode Driver};"
                //"Driver={MySQL ODBC 8.0 Unicode Driver};"
                "Driver={" + this.ServerInfo.DriverName + "};"
                + "Server=" + this.ServerInfo.ServerIP.ToString() + ";"
                + "Port=" + this.ServerInfo.PortNum.ToString() + ";"
                + "User=" + this.ServerInfo.Username + ";"
                + "Password=" + this.ServerInfo.Password + ";"
                + "Option=3;";

            if (this.checkBox1.CheckState == CheckState.Checked)
                this.ServerInfo.launchDBUI = true;
            else
                this.ServerInfo.launchDBUI = false;

            /* END of checks */

            try
            {
                if (this.MySQLconnection == null)
                {
                    this.MySQLconnection = new OdbcConnection(this.ServerInfo.ConnectionString);
                    this.MySQLconnection.Open();
                    IPv6SubnettingTool.Form1.MySQLconnection = this.MySQLconnection;

                    this.label14.Text = db;
                }
                else if (this.MySQLconnection != null)
                {
                    if (this.MySQLconnection.State != System.Data.ConnectionState.Open)
                    {
                        this.label16.Text = "Yes";
                        this.MySQLconnection.ConnectionString = this.ServerInfo.ConnectionString;
                        this.MySQLconnection.Open();
                        IPv6SubnettingTool.Form1.MySQLconnection = this.MySQLconnection;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: " + Environment.NewLine + ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Does DB exist and Table type correct? v6? v4? etc.

            if (DBExist())
            {
                string t = isv6Table();

                if (t == this.currentMode)
                {
                    this.DialogResult = DialogResult.OK;

                    // this.ServerInfo is != this.inputServerInfo

                    CopyServerInfoValues();

                    if (this is IDisposable)
                        this.Dispose();
                    else
                        this.Close();
                }
                else
                {
                    MessageBox.Show(StringsDictionary.KeyValue("DBinfo_notDBtable1", this.culture) + this.currentMode
                        + StringsDictionary.KeyValue("DBinfo_notDBtable2", this.culture),
                        StringsDictionary.KeyValue("DBinfo_notDBtable_header", this.culture),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else // NO Database
            {
                string r = CreateDBandTable();

                if (r == null)
                {
                    this.DialogResult = DialogResult.OK;

                    CopyServerInfoValues();

                    string msg = "";

                    if (this.currentMode == "v6")
                        msg = StringsDictionary.KeyValue("DBinfo_newDB1", this.culture) 
                            + this.ServerInfo.DBname + StringsDictionary.KeyValue("DBinfo_newDB2", this.culture);
                    else
                        msg = StringsDictionary.KeyValue("DBinfo_newDB1", this.culture) + this.ServerInfo.DBname_v4
                            + StringsDictionary.KeyValue("DBinfo_newDB2", this.culture);

                    MessageBox.Show(msg, "DB:", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (this is IDisposable)
                        this.Dispose();
                    else
                        this.Close();
                }
                else
                {
                    MessageBox.Show("Exception: CreateDBandTable()" + Environment.NewLine + r, "Exception",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private bool DBExist()
        {
            string sDBName = "", sTableName = "";

            if (this.currentMode == "v6")
            {
                sDBName = this.ServerInfo.DBname;
                sTableName = this.ServerInfo.Tablename;
            }
            else // v4
            {
                sDBName = this.ServerInfo.DBname_v4;
                sTableName = this.ServerInfo.Tablename_v4;
            }
            try
            {
                if (this.MySQLconnection.State == ConnectionState.Closed)
                    this.MySQLconnection.Open();

                //database exist?
                OdbcCommand MyCommand = new OdbcCommand("SELECT SCHEMA_NAME FROM"
                    + " INFORMATION_SCHEMA.SCHEMATA"
                    + " WHERE SCHEMA_NAME='" + sDBName + "';" , this.MySQLconnection);

                int r = MyCommand.ExecuteNonQuery();
                if (r > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + Environment.NewLine + ex.Message, "Error: DBExist()", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private string isv6Table()
        {
            try
            {
                if (this.MySQLconnection != null)
                {
                    if (this.MySQLconnection.State != System.Data.ConnectionState.Open)
                        this.MySQLconnection.Open();

                    string MySQLcmd = "";

                    if (this.currentMode == "v6")
                        MySQLcmd = "SHOW FIELDS from `" + this.ServerInfo.DBname + "`.`" + this.ServerInfo.Tablename + "` WHERE Field='prefix'";
                    else //v4
                        MySQLcmd = "SHOW FIELDS from `" + this.ServerInfo.DBname_v4 + "`.`" + this.ServerInfo.Tablename_v4 + "` WHERE Field='prefix'";

                    OdbcCommand MyCommand = new OdbcCommand(MySQLcmd, MySQLconnection);
                    OdbcDataReader MyDataReader = MyCommand.ExecuteReader();
                    int r = MyDataReader.RecordsAffected;

                    if (r > 0)
                    {
                        MyDataReader.Read();
                        string Field = MyDataReader.GetString(0);
                        string Type = MyDataReader.GetString(1);

                        MyDataReader.Close();
                        if (MyDataReader is IDisposable)
                            MyDataReader.Dispose();

                        if (Type.ToUpper() == "VARBINARY(16)")
                        {
                            return "v6";
                        }
                        else if (Type.ToUpper() == "INT(10) UNSIGNED")
                        {
                            return "v4";
                        }
                    }
                    else
                    {
                        MyDataReader.Close();
                        if (MyDataReader is IDisposable)
                            MyDataReader.Dispose();
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Query Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return null;
        }

        public string CreateDBandTable()
        {
            string spfx = "", sDBName = "", sTableName = "";
            string sTrigInsert = "", sTrigUpdate = "", sIdxIndex = "";

            if (this.currentMode == "v6")
            {
                spfx = "prefix VARBINARY(16), ";

                sDBName = this.ServerInfo.DBname;
                sTableName = this.ServerInfo.Tablename;

                sTrigInsert = "trig_insert_" + sDBName;
                sTrigUpdate = "trig_update_" + sDBName;
                sIdxIndex = "index_" + sDBName;
            }
            else // v4
            {
                spfx = "prefix INT UNSIGNED, ";

                sDBName = this.ServerInfo.DBname_v4;
                sTableName = this.ServerInfo.Tablename_v4;

                sTrigInsert = "trig_insert_v4_" + sDBName;
                sTrigUpdate = "trig_update_v4_" + sDBName;
                sIdxIndex = "index_v4_" + sDBName;
            }

            try
            {
                if (this.MySQLconnection.State == ConnectionState.Closed)
                    this.MySQLconnection.Open();

                // create database if not exists:
                OdbcCommand MyCommand = new OdbcCommand("CREATE DATABASE IF NOT EXISTS "
                    + "`" + sDBName + "`"
                    + " DEFAULT CHARACTER SET utf8 DEFAULT COLLATE utf8_general_ci;", this.MySQLconnection);
                MyCommand.ExecuteNonQuery();

                // use database:
                MyCommand.CommandText = "USE " + "`" + sDBName + "`;";
                MyCommand.ExecuteNonQuery();

                // create table if not exists:
                MyCommand.CommandText = "CREATE TABLE IF NOT EXISTS "
                    + "`" + sDBName + "`.`" + sTableName
                    + "` ( "
                    + spfx
                    + "pflen TINYINT UNSIGNED, "
                    + "parentpflen TINYINT UNSIGNED, "
                    + "netname VARCHAR(40), "
                    + "person  VARCHAR(40), "
                    + "organization VARCHAR(60), "
                    + "`as-num` INT UNSIGNED, "
                    + "phone VARCHAR(40), "
                    + "email VARCHAR(40), "
                    + "status VARCHAR(40), "
                    // MySQL's INVALID-ZERO problems... Even Windows MySQL server behaves different from Linux MySQL server:
                    //+ "`created` TIMESTAMP NOT NULL default '0000-00-00 00:00:00', "
                    + "`created` TIMESTAMP NOT NULL default '1970-01-02 01:01:01', "  // <--seems OK for both Windows & Linux. Changed also TRIGGER below.
                    + "`last-updated` TIMESTAMP NOT NULL default NOW() ON UPDATE NOW(), "
                    + "PRIMARY KEY(prefix, pflen) "
                    + "); ";
                MyCommand.ExecuteNonQuery();

                // MySQL Version 8.0, For Triggers:
                // Read: https://dev.mysql.com/doc/refman/8.0/en/stored-programs-logging.html
                
                // For Linux MySQL Servers: First read the document link above.
                // Comment out log_bin and log_bin_index lines in your /etc/mysql/my.cnf and restart server.
                
                // trigger for timestamps:
                MyCommand.CommandText = "SELECT TRIGGER_NAME FROM information_schema.triggers where TRIGGER_NAME='" + sTrigInsert + "';";
                if (MyCommand.ExecuteNonQuery() == 0)
                {
                    MyCommand.CommandText = "CREATE TRIGGER " + sTrigInsert + " BEFORE INSERT ON "
                        + "`" + sDBName + "`.`" + sTableName + "` "
                        + " FOR EACH ROW BEGIN SET NEW.`created`=IF(ISNULL(NEW.`created`) OR "
                        //+ "NEW.`created`='0000-00-00 00:00:00', CURRENT_TIMESTAMP, "
                        + "NEW.`created`='1970-01-02 01:01:01', CURRENT_TIMESTAMP, "
                        + "IF(NEW.`created` < CURRENT_TIMESTAMP, NEW.`created`, "
                        + "CURRENT_TIMESTAMP));SET NEW.`last-updated`=NEW.`created`; END;";
                    MyCommand.ExecuteNonQuery();
                }

                MyCommand.CommandText = "SELECT TRIGGER_NAME FROM information_schema.triggers where TRIGGER_NAME='" + sTrigUpdate + "';";
                if (MyCommand.ExecuteNonQuery() == 0)
                {
                    MyCommand.CommandText = "CREATE trigger " + sTrigUpdate + " BEFORE UPDATE ON "
                        + "`" + sDBName + "`.`" + sTableName + "` "
                        + " FOR EACH ROW "
                        + "SET NEW.`last-updated` = IF(NEW.`last-updated` < OLD.`last-updated`, "
                        + "OLD.`last-updated`, CURRENT_TIMESTAMP);";
                    MyCommand.ExecuteNonQuery();
                }

                // and index:
                MyCommand.CommandText = "SHOW INDEX from "
                    + "`" + sDBName + "`.`" + sTableName + "` "
                    + " WHERE Key_name = '" + sIdxIndex + "';";

                if (MyCommand.ExecuteNonQuery() == 0)
                {
                    MyCommand.CommandText = " CREATE INDEX " + sIdxIndex + " ON "
                    + "`"
                    + sDBName
                    + "`" + ".`"
                    + sTableName
                    + "` "
                    + " (prefix, pflen) USING BTREE;";
                    MyCommand.ExecuteNonQuery();
                }
            }
            catch (OdbcException ex)
            {
                return ex.Message;
            }

            return null;
        }

        private void CopyServerInfoValues()
        {
            this.inputServerInfo.ServerIP = this.ServerInfo.ServerIP;
            ScreenShotValues.ServerIP = this.ServerInfo.ServerIP;

            this.inputServerInfo.PortNum = this.ServerInfo.PortNum;
            ScreenShotValues.PortNum = this.ServerInfo.PortNum;

            this.inputServerInfo.Trytoconnect = this.ServerInfo.Trytoconnect;
            this.inputServerInfo.launchDBUI = this.ServerInfo.launchDBUI;

            if (this.ServerInfo.DriverName != "")
            {
                this.inputServerInfo.DriverName = this.ServerInfo.DriverName;
                ScreenShotValues.DriverName = this.ServerInfo.DriverName;
            }
            if (this.ServerInfo.ConnectionString != "")
            {
                this.inputServerInfo.ConnectionString = this.ServerInfo.ConnectionString;
            }
            if (this.ServerInfo.DBname != "")
            {
                this.inputServerInfo.DBname = this.ServerInfo.DBname;
                ScreenShotValues.DBname = this.ServerInfo.DBname;
            }
            if (this.ServerInfo.Tablename != "")
            {
                this.inputServerInfo.Tablename = this.ServerInfo.Tablename;
                ScreenShotValues.Tablename = this.ServerInfo.Tablename;
            }
            if (this.ServerInfo.DBname_v4 != "")
            {
                this.inputServerInfo.DBname_v4 = this.ServerInfo.DBname_v4;
                ScreenShotValues.DBname_v4 = this.ServerInfo.DBname_v4;
            }
            if (this.ServerInfo.Tablename_v4 != "")
            {
                this.inputServerInfo.Tablename_v4 = this.ServerInfo.Tablename_v4;
                ScreenShotValues.Tablename_v4 = this.ServerInfo.Tablename_v4;
            }
            if (this.ServerInfo.Username != "")
            {
                this.inputServerInfo.Username = this.ServerInfo.Username;
                ScreenShotValues.Username = this.ServerInfo.Username;
            }
            if (this.ServerInfo.Password != "")
            {
                this.inputServerInfo.Password = this.ServerInfo.Password;
            }

            this.ServerInfo = this.inputServerInfo;  // callers will use 'this.ServerInfo'
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.ServerInfo.Trytoconnect = false;

            this.DialogResult = DialogResult.Cancel;
            this.ServerInfo = this.inputServerInfo;

            if (this is IDisposable)
                this.Dispose();
            else
                this.Close();
        }

        private void DBinfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.ServerInfo.Trytoconnect = false;
                this.DialogResult = DialogResult.Cancel;
                this.ServerInfo = this.inputServerInfo;

                if (this is IDisposable)
                    this.Dispose();
                else
                    this.Close();
            }
        }

        public void SwitchLanguage(CultureInfo culture)
        {
            this.culture = culture;
            this.Text = StringsDictionary.KeyValue("DBinfo_Form.Text", this.culture);
            this.button1.Text = StringsDictionary.KeyValue("DBinfo_button1.Text", this.culture);
            this.button2.Text = StringsDictionary.KeyValue("DBinfo_button2.Text", this.culture);
            this.label1.Text = StringsDictionary.KeyValue("DBinfo_label1.Text", this.culture);
            this.label2.Text = StringsDictionary.KeyValue("DBinfo_label2.Text", this.culture);
            this.label3.Text = StringsDictionary.KeyValue("DBinfo_label3.Text", this.culture);
            this.label4.Text = StringsDictionary.KeyValue("DBinfo_label4.Text", this.culture);
            this.label5.Text = StringsDictionary.KeyValue("DBinfo_label5.Text", this.culture);
            this.label6.Text = StringsDictionary.KeyValue("DBinfo_label6.Text", this.culture);
            this.label7.Text = StringsDictionary.KeyValue("DBinfo_label7.Text", this.culture);
            this.textBox6.Text = StringsDictionary.KeyValue("DBinfo_textBox6.Text", this.culture);
            this.checkBox1.Text = StringsDictionary.KeyValue("DBinfo_launchDBUI", this.culture);
            this.label13.Text = StringsDictionary.KeyValue("DBinfo_label13.Text", this.culture);
            this.label15.Text = StringsDictionary.KeyValue("DBinfo_label15.Text", this.culture);
            this.label9.Text = StringsDictionary.KeyValue("DBinfo_label9.Text", this.culture);
            this.ChangeUILanguage.Invoke(this.culture);
        }

        private void DBinfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.ServerInfo.Trytoconnect = false;
                this.DialogResult = DialogResult.Cancel;
                this.ServerInfo = this.inputServerInfo;

                if (this is IDisposable)
                    this.Dispose();
                else
                    this.Close();
            }
        }

        private void comboBox1_Enter(object sender, EventArgs e)
        {
            this.comboBox1.BackColor = Color.White;
        }

        private void comboBox1_Leave(object sender, EventArgs e)
        {
            string s = this.comboBox1.Text.Trim();
            if (!this.comboBox1.Items.Contains(s) && s.Trim() != "")
            {
                this.comboBox1.Items.Add(s);
                this.comboBox1.SelectedItem = this.comboBox1.Items[this.comboBox1.Items.Count - 1];
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            this.textBox1.BackColor = Color.White;
        }

        private void textBox7_Enter(object sender, EventArgs e)
        {
            this.textBox7.BackColor = Color.White;
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            this.textBox2.BackColor = Color.White;
        }

        private void textBox5_Enter(object sender, EventArgs e)
        {
            this.textBox5.BackColor = Color.White;
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            this.textBox3.BackColor = Color.White;
        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            this.textBox4.BackColor = Color.White;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.comboBox1.Items.Clear();
            
            List<string> odbcdriverNames = this.OdbcDriverNames();
            
            if (odbcdriverNames.Count != 0)
                this.comboBox1.Items.AddRange(odbcdriverNames.ToArray());

            this.comboBox1.SelectedItem = this.comboBox1.Items[0];
        }

        private List<string> OdbcDriverNames()
        {
            List<string> driverNames = new List<string>();

            // 64bit & 32bit installed drivers:
            string[] regkeys = { @"SOFTWARE\ODBC\ODBCINST.INI\ODBC Drivers", 
                                 @"SOFTWARE\WOW6432Node\ODBC\ODBCINST.INI\ODBC Drivers" };

            foreach (string s in regkeys)
            {
                using (RegistryKey HKLMRegistry = Registry.LocalMachine)
                using (RegistryKey odbcDrivers = HKLMRegistry.OpenSubKey(s))
                {
                    if (odbcDrivers != null)
                    {
                        driverNames.AddRange(odbcDrivers.GetValueNames());
                    }
                }
            }

            return driverNames;
        }
    }
}
