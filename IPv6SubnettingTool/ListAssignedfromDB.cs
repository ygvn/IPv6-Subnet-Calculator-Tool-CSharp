/*
 * Copyright (c) 2010-2020 Yucel Guven
 * All rights reserved.
 * 
 * This file is part of IPv6 Subnetting Tool.
 * 
 * Version: 4.4
 * Published Date: 23 February 2020
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
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Data.Odbc;
using System.Numerics;

namespace IPv6SubnettingTool
{
    public partial class ListAssignedfromDB : Form
    {
        #region special initials/constants - Yücel
        SEaddress seaddr = new SEaddress();
        string prefix = "";
        string end = "";
        short pflen = 0;
        short parentpflen = 0;
        string tmp_first = "";
        string tmp_last = "";
        string last_start = "";
        string db_FirstItem = "";
        string db_LastItem = "";
        int db_ItemCount = 0;
        int page_records = 0;
        int records_perpage = 32;

        //DB
        OdbcConnection MySQLconnection = null;
        public DBServerInfo ServerInfo = new DBServerInfo();
        CheckState chks = CheckState.Unchecked;
        OdbcDataReader MyDataReader;
        List<string> liste = new List<string>();
        string currentMode = "";
        //
        public CultureInfo culture;
        //
        public delegate void ChangeDatabaseDelegate(string dbname);
        public event ChangeDatabaseDelegate ChangeDB = delegate { };
        #endregion

        public ListAssignedfromDB(string prefix, string end, short parentpflen, short pflen, CheckState chks,
            OdbcConnection sqlcon, DBServerInfo servinfo, CultureInfo culture, string mode)
        {
            InitializeComponent();

            this.currentMode = mode;
            this.parentpflen = parentpflen;
            this.pflen = pflen;
            this.chks = chks;
            this.MySQLconnection = sqlcon;
            this.ServerInfo = servinfo.ShallowCopy();
            this.culture = culture;
            this.SwitchLanguage(culture);

            if (this.currentMode == "v6")
            {
                this.prefix = v6ST.CompressAddress(prefix.Split('/')[0]);
                this.end = v6ST.CompressAddress(end.Split('/')[0]);

                this.toolStripStatusLabel1.Text = "Database: " + this.ServerInfo.DBname;
            }
            else // v4
            {
                this.prefix = prefix.Split('/')[0];
                this.end = end.Split('/')[0];

                this.toolStripStatusLabel1.Text = "Database: " + this.ServerInfo.DBname_v4;
            }

            this.label2.ForeColor = Color.RoyalBlue;
            this.label2.Text = prefix + " - " + "/" + pflen;

            this.groupBox1.Text = "/" + pflen
                + StringsDictionary.KeyValue("StatsUsageForm_groupBox2.Text", this.culture);

            if (this.MySQLconnection == null)
                this.toolStripStatusLabel2.Text = "db=Down";
            else
            {
                if (this.MySQLconnection.State == ConnectionState.Open)
                    this.toolStripStatusLabel2.Text = "db=Up";
                else if (this.MySQLconnection.State == ConnectionState.Closed)
                    this.toolStripStatusLabel2.Text = "db=Down";
            }

            this.PreCalc();
            this.FirstAndLastInDB();
            this.button1_Click(null, null);
            this.label4.Text = "[" + this.page_records
                + StringsDictionary.KeyValue("ListAssignedfromDBForm_label4.Text", this.culture);
        }

        private void PreCalc()
        {
            if (this.currentMode == "v6")
            {
                if (chks == CheckState.Checked) /* 128 bits */
                {
                    this.seaddr.End = BigInteger.Parse("0" + v6ST.FormalizeAddr(this.end),
                        NumberStyles.AllowHexSpecifier);
                }
                else if (chks == CheckState.Unchecked) /* 64 bits */
                {
                    this.seaddr.End = BigInteger.Parse("0" +
                        v6ST.FormalizeAddr(this.end).Substring(0, 16),
                        NumberStyles.AllowHexSpecifier);
                }
                this.seaddr.slash = this.parentpflen;
                this.seaddr.subnetslash = this.pflen;
                this.seaddr = v6ST.EndStartAddresses(this.seaddr, this.chks);

                if (chks == CheckState.Checked) /* 128 bits */
                {
                    string s = String.Format("{0:x}", this.seaddr.Start);
                    if (s.Length > 16)
                        s = s.Substring(1, 16);
                    this.last_start = v6ST.Kolonlar(s, this.chks);
                }
                else if (chks == CheckState.Unchecked) /* 64 bits */
                {
                    string s = String.Format("{0:x}", this.seaddr.Start);
                    if (s.Length > 16)
                        s = s.Substring(1, 16);
                    this.last_start = v6ST.Kolonlar(s, this.chks);
                }
                this.last_start = v6ST.CompressAddress(this.last_start);
            }
            else // v4
            {
                this.seaddr.End = BigInteger.Parse("0" + v6ST.FormalizeAddr_v4(this.end),
                    NumberStyles.AllowHexSpecifier);

                this.seaddr.slash = this.parentpflen;
                this.seaddr.subnetslash = this.pflen;
                this.seaddr = v6ST.EndStartAddresses_v4(this.seaddr);

                string s = String.Format("{0:x}", this.seaddr.Start);
                this.last_start = v6ST.IPv4Format(s);
            }
        }

        public int FirstAndLastInDB()
        {
            if (this.MySQLconnection == null)
                return -1;

            if (this.MySQLconnection.State == ConnectionState.Closed)
            {
                MessageBox.Show(StringsDictionary.KeyValue("FormDB_MySQLquery_closed", this.culture),
                    StringsDictionary.KeyValue("FormDB_MySQLquery_closed_header", this.culture),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }

            int r = 0;
            string spfx = "", spfx2 = "", sDBName = "", sTableName = "";

            try
            {                
                if (this.currentMode == "v6")
                {
                    this.MySQLconnection.ChangeDatabase(this.ServerInfo.DBname);

                    spfx = "inet6_aton('";
                    spfx2 = "INET6_NTOA";
                    sDBName = this.ServerInfo.DBname;
                    sTableName = this.ServerInfo.Tablename;
                }
                else // v4
                {
                    this.MySQLconnection.ChangeDatabase(this.ServerInfo.DBname_v4);

                    spfx = "inet_aton('";
                    spfx2 = "INET_NTOA";
                    sDBName = this.ServerInfo.DBname_v4;
                    sTableName = this.ServerInfo.Tablename_v4;
                }

                string MySQLcmd = "SELECT COUNT(*) FROM "
                    + "`" + sDBName + "`.`" + sTableName + "`"
                    + " WHERE ( prefix >= " + spfx + this.prefix + "')"
                    + " AND prefix <= " + spfx + this.end + "')"
                    + " AND parentpflen= " + parentpflen + " AND pflen= " + pflen + " ) ";

                OdbcCommand MyCommand = new OdbcCommand(MySQLcmd, this.MySQLconnection);
                MyDataReader = MyCommand.ExecuteReader();
                MyDataReader.Read();
                this.db_ItemCount = int.Parse(MyDataReader.GetString(0));

                MySQLcmd = "SELECT "
                    + spfx2 + "(prefix), pflen, netname, person, organization, "
                    + "`as-num`, phone, email, status, created, `last-updated` FROM "
                    + "`" + sDBName + "`.`" + sTableName + "`"
                    + " WHERE ( prefix >= " + spfx + this.prefix + "')"
                    + " AND prefix <= " + spfx + this.end + "')"
                    + " AND parentpflen= " + parentpflen + " AND pflen= " + pflen + " ) "
                    + " LIMIT " + this.records_perpage;

                MyCommand = new OdbcCommand(MySQLcmd, this.MySQLconnection);

                MyDataReader = MyCommand.ExecuteReader();
                this.page_records = r = MyDataReader.RecordsAffected;
                if (r > 0)
                {
                    MyDataReader.Read();
                    this.db_FirstItem = MyDataReader.GetString(0);
                }

                MySQLcmd = "SELECT "
                    + spfx2 + "(prefix), pflen, netname, person, organization, "
                    + "`as-num`, phone, email, status, created, `last-updated` FROM "
                    + "`" + sDBName + "`.`" + sTableName + "`"
                    + " WHERE ( prefix <= " + spfx + this.end + "')"
                    + " AND parentpflen= " + parentpflen + " AND pflen= " + pflen + " ) "
                    + " ORDER BY prefix DESC LIMIT " + this.records_perpage;

                MyCommand = new OdbcCommand(MySQLcmd, this.MySQLconnection);

                MyDataReader = MyCommand.ExecuteReader();
                this.page_records = r = MyDataReader.RecordsAffected;
                if (r > 0)
                {
                    MyDataReader.Read();
                    this.db_LastItem = MyDataReader.GetString(0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message +
                    StringsDictionary.KeyValue("FormDB_MySQLquery_exception", this.culture),
                    StringsDictionary.KeyValue("FormDB_MySQLquery_exception_header", this.culture),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }

            return r;
        }

        public int MySQLquery(int button)
        {
            if (this.MySQLconnection == null)
                return -1;

            if (this.MySQLconnection.State == ConnectionState.Closed)
            {
                MessageBox.Show(StringsDictionary.KeyValue("FormDB_MySQLquery_closed", this.culture),
                    StringsDictionary.KeyValue("FormDB_MySQLquery_closed_header", this.culture),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }

            if (this.tmp_last == "")
                this.tmp_last = this.prefix;
            
            int r = 0;
            string MySQLcmd = "";
            this.listBox1.Items.Clear();

            string spfx = "", spfx2 = "", sDBName = "", sTableName = "";

            try
            {
                if (this.currentMode == "v6")
                {
                    this.MySQLconnection.ChangeDatabase(this.ServerInfo.DBname);

                    spfx = "inet6_aton('";
                    spfx2 = "INET6_NTOA";
                    sDBName = this.ServerInfo.DBname;
                    sTableName = this.ServerInfo.Tablename;
                }
                else // v4
                {
                    this.MySQLconnection.ChangeDatabase(this.ServerInfo.DBname_v4);

                    spfx = "inet_aton('";
                    spfx2 = "INET_NTOA";
                    sDBName = this.ServerInfo.DBname_v4;
                    sTableName = this.ServerInfo.Tablename_v4;
                }

                if (button == 1) // // First page
                {
                    MySQLcmd = "SELECT "
                        + spfx2 + "(prefix), pflen, netname, person, organization, "
                        + "`as-num`, phone, email, status, created, `last-updated` FROM "
                        + "`" + sDBName + "`.`" + sTableName + "`"
                        + " WHERE ( prefix >= " + spfx + this.tmp_last + "')"
                        + " AND prefix <= " + spfx + this.end + "')"
                        + " AND parentpflen= " + parentpflen + " AND pflen= " + pflen + " ) "
                        //+ " LIMIT 4 ";
                        + " LIMIT " + this.records_perpage;
                }
                else if (button == 2) // Backwd page
                {
                    MySQLcmd = "SELECT "
                        + spfx2 + "(prefix), pflen, netname, person, organization, "
                        + "`as-num`, phone, email, status, created, `last-updated` FROM "
                        + "`" + sDBName + "`.`" + sTableName + "`"
                        + " WHERE ( prefix < " + spfx + this.tmp_first + "')"
                        + " AND prefix >= " + spfx + this.prefix + "')"
                        + " AND parentpflen= " + parentpflen + " AND pflen= " + pflen + " ) "
                        //+ " ORDER BY prefix LIMIT 4 ";
                        + " ORDER BY prefix DESC LIMIT " + this.records_perpage;
                }
                else if (button == 3) // Fwd page
                {
                    MySQLcmd = "SELECT "
                        + spfx2 + "(prefix), pflen, netname, person, organization, "
                        + "`as-num`, phone, email, status, created, `last-updated` FROM "
                        + "`" + sDBName + "`.`" + sTableName + "`"
                        + " WHERE ( prefix > " + spfx + this.tmp_last + "')"
                        + " AND prefix <= " + spfx + this.end + "')"
                        + " AND parentpflen= " + parentpflen + " AND pflen= " + pflen + " ) "
                        //+ " LIMIT 4 ";
                        + " LIMIT " + this.records_perpage;
                }
                else if (button == 4) // Last page
                {
                    MySQLcmd = "SELECT "
                        + spfx2 + "(prefix), pflen, netname, person, organization, "
                        + "`as-num`, phone, email, status, created, `last-updated` FROM "
                        + "`" + sDBName + "`.`" + sTableName + "`"
                        + " WHERE ( prefix <= " + spfx + this.tmp_first + "')"
                        + " AND parentpflen= " + parentpflen + " AND pflen= " + pflen + " ) "
                        //+ " ORDER BY prefix DESC LIMIT 4 ";
                        + " ORDER BY prefix DESC LIMIT " + this.records_perpage;
                }

                OdbcCommand MyCommand = new OdbcCommand(MySQLcmd, this.MySQLconnection);

                MyDataReader = MyCommand.ExecuteReader();
                this.page_records = r = MyDataReader.RecordsAffected;

                if (r > 0)
                {
                    liste.Clear();

                    if (button == 1 || button == 3)
                    {
                        while (MyDataReader.Read())
                        {
                            liste.Add("prefix:\t\t " + MyDataReader.GetString(0) + "/" + MyDataReader.GetByte(1).ToString());
                            liste.Add("netname:\t " + MyDataReader.GetString(2));
                            liste.Add("person:\t\t " + MyDataReader.GetString(3));
                            liste.Add("organization:\t " + MyDataReader.GetString(4));
                            liste.Add("as-num:\t\t " + MyDataReader.GetString(5));
                            liste.Add("phone:\t\t " + MyDataReader.GetString(6));
                            liste.Add("email:\t\t " + MyDataReader.GetString(7));
                            liste.Add("status:\t\t " + MyDataReader.GetString(8));
                            liste.Add("created:\t\t " + MyDataReader.GetString(9));
                            liste.Add("last-updated:\t " + MyDataReader.GetString(10));
                            liste.Add("");
                        }
                    }
                    else if (button == 2 || button == 4)
                    {
                        String[] fs = new String[10];
                        while (MyDataReader.Read())
                        {
                            liste.Add("");
                            fs[0] = "prefix:\t\t " + MyDataReader.GetString(0) + "/" + MyDataReader.GetByte(1).ToString();
                            fs[1] = "netname:\t " + MyDataReader.GetString(2);
                            fs[2] = "person:\t\t " + MyDataReader.GetString(3);
                            fs[3] = "organization:\t " + MyDataReader.GetString(4);
                            fs[4] = "as-num:\t\t " + MyDataReader.GetString(5);
                            fs[5] = "phone:\t\t " + MyDataReader.GetString(6);
                            fs[6] = "email:\t\t " + MyDataReader.GetString(7);
                            fs[7] = "status:\t\t " + MyDataReader.GetString(8);
                            fs[8] = "created:\t\t " + MyDataReader.GetString(9);
                            fs[9] = "last-updated:\t " + MyDataReader.GetString(10);

                            for (int i = 9; i > -1; i--)
                                liste.Add(fs[i]);
                        }

                        liste.Reverse();
                    }

                    this.listBox1.Items.AddRange(liste.ToArray());
                    this.tmp_first = liste[0].Split(' ')[1].Split('/')[0];
                    this.tmp_last = liste[liste.Count - 11].Split(' ')[1].Split('/')[0];
                }

                MyDataReader.Close();
                if (MyDataReader is IDisposable)
                    MyDataReader.Dispose();
                return r;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message +
                    StringsDictionary.KeyValue("FormDB_MySQLquery_exception", this.culture),
                    StringsDictionary.KeyValue("FormDB_MySQLquery_exception_header", this.culture),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public void DBStateChange(OdbcConnection dbconn, int info)
        {
            if (dbconn == this.MySQLconnection
                && dbconn.Database == this.MySQLconnection.Database
                && dbconn.ConnectionString == this.MySQLconnection.ConnectionString)
            {
                this.MySQLconnection = dbconn;

                if (info == -1)
                {
                    if (this.MySQLconnection == null)
                        toolStripStatusLabel2.Text = "db=Down";
                    else
                    {
                        if (this.MySQLconnection.State == ConnectionState.Open)
                            toolStripStatusLabel2.Text = "db=Up";
                        else if (this.MySQLconnection.State == ConnectionState.Closed)
                            toolStripStatusLabel2.Text = "db=Down";
                    }

                    IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());

                    if (this is IDisposable)
                        this.Dispose();
                    else
                        this.Close();

                }
                else if (info == 1)
                {
                    if (this.MySQLconnection == null)
                        toolStripStatusLabel2.Text = "db=Down";
                    else
                    {
                        if (this.MySQLconnection.State == ConnectionState.Open)
                            toolStripStatusLabel2.Text = "db=Up";
                        else if (this.MySQLconnection.State == ConnectionState.Closed)
                            toolStripStatusLabel2.Text = "db=Down";
                    }
                }
            }
            
        }

        public void SwitchLanguage(CultureInfo culture)
        {
            this.culture = culture;
            this.Text = StringsDictionary.KeyValue("ListAssignedfromDBForm.Text", this.culture);
            this.button1.Text = StringsDictionary.KeyValue("ListAssignedfromDBForm_button1.Text", this.culture);
            this.selectalltoolStripMenuItem1.Text = StringsDictionary.KeyValue("ListAssignedfromDBForm_selectalltoolStripMenuItem1.Text", this.culture);
            this.copytoolStripMenuItem1.Text = StringsDictionary.KeyValue("ListAssignedfromDBForm_copytoolStripMenuItem1.Text", this.culture);
            this.label1.Text = StringsDictionary.KeyValue("ListSubnetRange_label4.Text", this.culture);
            this.groupBox1.Text = StringsDictionary.KeyValue("StatsUsageForm_groupBox2.Text", this.culture);
            this.label4.Text = "[" + this.page_records
                + StringsDictionary.KeyValue("ListAssignedfromDBForm_label4.Text", this.culture);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.tmp_last = "";
            this.PreCalc();

            int r = this.MySQLquery(1);

            if (r > 0)
            {
                if (this.db_ItemCount > this.records_perpage)
                {
                    this.button2.Enabled = false;
                    this.button3.Enabled = true;
                    this.button4.Enabled = true;
                }
                else
                {
                    this.button2.Enabled = false;
                    this.button3.Enabled = false;
                    this.button4.Enabled = false;
                }
            }
            else
            {
                this.button2.Enabled = false;
                this.button3.Enabled = false;
                this.button4.Enabled = false;
            }
            this.label4.Text = "[" + this.page_records
                + StringsDictionary.KeyValue("ListAssignedfromDBForm_label4.Text", this.culture);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int r = this.MySQLquery(2);
            if (r > 0)
            {
                this.button3.Enabled = true;
                this.button4.Enabled = true;

                if (this.db_FirstItem == this.listBox1.Items[0].ToString().Split(' ')[1].Split('/')[0])
                {
                    this.button2.Enabled = false;
                    this.button3.Enabled = true;
                    this.label4.Text = "[" + this.page_records
                        + StringsDictionary.KeyValue("ListAssignedfromDBForm_label4.Text", this.culture);
                    return;
                }
            }
            this.label4.Text = "[" + this.page_records
                + StringsDictionary.KeyValue("ListAssignedfromDBForm_label4.Text", this.culture);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int r = this.MySQLquery(3);
            if (r > 0)
            {
                this.button2.Enabled = true;

                if (this.db_LastItem == this.tmp_last)
                {
                    this.button3.Enabled = false;
                    this.button4.Enabled = false;
                    this.label4.Text = "[" + this.page_records
                        + StringsDictionary.KeyValue("ListAssignedfromDBForm_label4.Text", this.culture);
                    return;
                }
            }
            this.label4.Text = "[" + this.page_records
                + StringsDictionary.KeyValue("ListAssignedfromDBForm_label4.Text", this.culture);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.tmp_first = this.last_start;
            int r = this.MySQLquery(4);
            if (r > 0)
            {
                this.button2.Enabled = true;
                this.button3.Enabled = false;
                this.button4.Enabled = false;
            }
            this.label4.Text = "[" + this.page_records
                + StringsDictionary.KeyValue("ListAssignedfromDBForm_label4.Text", this.culture);
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;

            ListBox lb = (ListBox)sender;
            Graphics g = e.Graphics;
            SolidBrush sback = new SolidBrush(e.BackColor);
            SolidBrush sfore = new SolidBrush(e.ForeColor);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

            if (e.Index % 11 == 0)
            {
                e.DrawBackground();
                DrawItemState st = DrawItemState.Selected;

                if ((e.State & st) != st)
                {
                    Color color = Color.FromArgb(30, 64, 224, 208);
                    g.FillRectangle(new SolidBrush(color), e.Bounds);
                    g.DrawString(lb.Items[e.Index].ToString(), e.Font,
                        sfore, new PointF(e.Bounds.X, e.Bounds.Y));
                }
                else
                {
                    g.FillRectangle(sback, e.Bounds);
                    g.DrawString(lb.Items[e.Index].ToString(), e.Font,
                        sfore, new PointF(e.Bounds.X, e.Bounds.Y));
                }
                e.DrawFocusRectangle();
            }
            else
            {
                e.DrawBackground();
                g.FillRectangle(sback, e.Bounds);
                g.DrawString(lb.Items[e.Index].ToString(), e.Font,
                    sfore, new PointF(e.Bounds.X, e.Bounds.Y));
                e.DrawFocusRectangle();
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;

            if (e.Control && e.KeyCode == Keys.A)
            {
                listBox1.Visible = false;
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    listBox1.SetSelected(i, true);
                }
                listBox1.Visible = true;
                listBox1.Focus();
            }
            if (e.Control && e.KeyCode == Keys.C)
            {
                string s = "";
                foreach (object o in listBox1.SelectedItems)
                {
                    s += o.ToString() + Environment.NewLine;
                }
                Clipboard.SetText(s);
            }
        }

        private void selectalltoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            listBox1.Visible = false;
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                listBox1.SetSelected(i, true);
            }
            listBox1.Visible = true;
        }

        private void copytoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string s = "";
            foreach (object o in listBox1.SelectedItems)
            {
                s += o.ToString() + Environment.NewLine;
            }
            if (s != "")
                Clipboard.SetText(s);
        }

        private void ListAssignedfromDB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
                this.Close();
            }
        }

        public void ChangeDatabase(string dbname)
        {
            /* DON'T USE:
            this.label5.Text = "DB changed! NewDB: " + dbname;
            //StringsDictionary.KeyValue("FormDB_insertrecord", this.culture);
            this.ChangeDB.Invoke(dbname);
            */
        }

        private void ListAssignedfromDB_FormClosing(object sender, FormClosingEventArgs e)
        {
            IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
        }
    }
}
