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
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Data.Odbc;
using System.Numerics;

namespace IPv6SubnettingTool
{
    public partial class PrefixSubLevels : Form
    {
        #region special initials/constants -yucel
        string prefix = null;
        short pflen = 0;
        string parentprefix = null;
        short parentpflen = 0;
        string end = null;
        CheckState chks = CheckState.Unchecked;
        int t1 = 0, t2 = 0;
        List<string[]> liste = new List<string[]>();

        OdbcConnection MySQLconnection;
        DBServerInfo ServerInfo = new DBServerInfo();
        OdbcDataReader MyDataReader;
        public CultureInfo culture;
        public delegate void ChangeWinFormStringsDelegate(CultureInfo culture);
        public event ChangeWinFormStringsDelegate ChangeUILanguage = delegate { };

        TreeNode tnode = new TreeNode();
        TreeNode root = new TreeNode();
        private const string plus = " ";
        //
        string currentMode = "";
        public delegate void ChangeDatabaseDelegate(string dbname);
        public event ChangeDatabaseDelegate ChangeDB = delegate { };
        #endregion

        public PrefixSubLevels(string snet, short plen,
            CheckState chks, int t1, int t2, OdbcConnection sqlcon, 
            DBServerInfo servinfo, CultureInfo culture, string mode, Font font)
        {
            InitializeComponent();

            this.currentMode = mode;
            this.ServerInfo = servinfo.ShallowCopy();
            this.culture = culture;
            this.SwitchLanguage(this.culture);
            this.chks = chks;
            this.t1 = t1; this.t2 = t2;

            this.treeView1.Font = font;
            this.textBox1.Font = font;
            this.textBox2.Font = font;

            this.MySQLconnection = sqlcon;

            short ppflen = QuerySelectedPrefix(snet, plen);

            if (ppflen == 0 || ppflen == -1)
            {
                this.Close();
                return;
            }

            this.pflen = plen;
            this.parentpflen = plen;
            string parentprefix = "";

            if (this.currentMode == "v6")
            {
                parentprefix = v6ST.FindParentNet(snet, ppflen, chks);
                this.prefix = v6ST.CompressAddress(snet);
                this.end = this.FindEnd(this.prefix, this.pflen, this.chks);

                this.toolStripStatusLabel1.Text = "Database: " + this.ServerInfo.DBname;
            }
            else // v4
            {
                parentprefix = v6ST.FindParentNet_v4(snet, ppflen);
                this.prefix = snet;
                this.end = this.FindEnd_v4(this.prefix, this.pflen);

                this.toolStripStatusLabel1.Text = "Database: " + this.ServerInfo.DBname_v4;
            }

            this.parentprefix = parentprefix;

            this.textBox2.ForeColor = Color.FromKnownColor(KnownColor.RoyalBlue);
            this.textBox2.BackColor = Color.FromKnownColor(KnownColor.Control);

            this.textBox1.Text = this.parentprefix;
            this.textBox2.Text = "└ " + this.prefix + "/" + this.pflen;

            this.treeView1.Nodes.Clear();
            this.root = this.treeView1.Nodes.Add(this.prefix + "/" + this.pflen);
            this.treeView1.Nodes[0].Nodes.Add(plus);

            if (this.MySQLconnection == null)
                this.toolStripStatusLabel2.Text = "db=Down";
            else
            {
                if (this.MySQLconnection.State == ConnectionState.Open)
                    this.toolStripStatusLabel2.Text = "db=Up";
                else if (this.MySQLconnection.State == ConnectionState.Closed)
                    this.toolStripStatusLabel2.Text = "db=Down";
            }

        }

        private string FindEnd(string snet, short pflen, CheckState chks) 
        {
            SEaddress se = new SEaddress();
            string end = "";
            string start = v6ST.FormalizeAddr(snet);
            
            if (this.chks == CheckState.Checked) /* 128 bits */
            {
                if (start.Length == 32)
                    start = "0" + start;
                se.Start = BigInteger.Parse(start, NumberStyles.AllowHexSpecifier);
                se.slash = this.t1;
                se.subnetslash = pflen;
                se = v6ST.Subnetting(se, this.chks);
                end = String.Format("{0:x}", se.End);
                if (end.Length > 32)
                    end = end.Substring(1, 32);
                end = v6ST.Kolonlar(end, this.chks);
            }
            else if (this.chks == CheckState.Unchecked) /* 64 bits */
            {
                if (start.Length == 16)
                    start = "0" + start;
                start = start.Substring(0, 16);
                se.Start = BigInteger.Parse(start, NumberStyles.AllowHexSpecifier);
                se.slash = this.t1;
                se.subnetslash = pflen;
                se = v6ST.Subnetting(se, this.chks);
                end = String.Format("{0:x}", se.End);
                if (end.Length > 16)
                    end = end.Substring(1, 16);
                end = v6ST.Kolonlar(end, this.chks);
            }
            end = v6ST.CompressAddress(end);
            
            return end;
        }

        private string FindEnd_v4(string snet, short pflen)
        {
            SEaddress se = new SEaddress();
            string end = "";
            string start = v6ST.FormalizeAddr_v4(snet);

            se.Start = BigInteger.Parse(start, NumberStyles.AllowHexSpecifier);
            se.slash = this.t1;
            se.subnetslash = pflen;
            se = v6ST.Subnetting_v4(se);
            end = String.Format("{0:x}", se.End);
            end = v6ST.IPv4Format(end);

            return end;
        }

        public int MySQLquery(string inprefix, string end, short pflen)
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
            try
            {
                this.liste.Clear();

                int r = 0;
                string MySQLcmd = "";
                string is128bits = "";

                string sDBName = "", sTableName = "";

                if (this.MySQLconnection.State != ConnectionState.Open)
                    this.MySQLconnection.Open();

                if (this.currentMode == "v6")
                {
                    this.MySQLconnection.ChangeDatabase(this.ServerInfo.DBname);

                    sDBName = this.ServerInfo.DBname;
                    sTableName = this.ServerInfo.Tablename;

                    if (this.chks == CheckState.Unchecked)
                    {
                        is128bits = " AND pflen > " + pflen + " AND pflen <= 64";
                    }
                    else if (this.chks == CheckState.Checked)
                    {
                        is128bits = " AND pflen > " + pflen;
                    }

                    MySQLcmd = "SELECT inet6_ntoa(prefix), pflen, netname, status "
                        + " from " + this.ServerInfo.Tablename
                        + " WHERE ( prefix BETWEEN inet6_aton('" + inprefix + "') "
                        + " AND inet6_aton('" + end + "') "
                        + is128bits + " AND parentpflen= " + pflen + ") "
                        + " LIMIT 32768; ";
                }
                else // v4 
                {
                    this.MySQLconnection.ChangeDatabase(this.ServerInfo.DBname_v4);

                    sDBName = this.ServerInfo.DBname_v4;
                    sTableName = this.ServerInfo.Tablename_v4;
                    string is32bits = " AND pflen > " + pflen;

                    MySQLcmd = "SELECT inet_ntoa(prefix), pflen, netname, status "
                        + " from " + "`" + sDBName + "`." + "`" + sTableName + "`"
                        + " WHERE ( prefix BETWEEN inet_aton('" + inprefix + "') "
                        + " AND inet_aton('" + end + "') "
                        + is32bits + " AND parentpflen= " + pflen + ") "
                        + " LIMIT 32768; ";
                }

                OdbcCommand MyCommand = new OdbcCommand(MySQLcmd, this.MySQLconnection);

                MyDataReader = MyCommand.ExecuteReader();

                r = MyDataReader.RecordsAffected;
                this.liste.Clear();

                while (MyDataReader.Read())
                {
                    this.liste.Add(new string[]{
                        MyDataReader.GetString(0),
                        MyDataReader.GetByte(1).ToString(),
                        MyDataReader.GetString(2),
                        MyDataReader.GetString(3)}
                        );
                }

                MyDataReader.Close();
                if (MyDataReader is IDisposable)
                    MyDataReader.Dispose();
                return r;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: MySQLquery()\r\n" + ex.Message +
                    StringsDictionary.KeyValue("FormDB_MySQLquery_exception", this.culture),
                    StringsDictionary.KeyValue("FormDB_MySQLquery_exception_header", this.culture),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }

        public void DBStateChange(OdbcConnection dbconn, int info)
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
                // this.changeDBstate.Invoke(this.MySQLconnection, info);

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

        private void AddNode(TreeNode node)
        {
            string inprefix = node.Text.Split('/')[0];
            short pflen = Convert.ToInt16(node.Text.Split('/')[1]);
            string end = "";

            if (this.currentMode == "v6")
            {
                end = this.FindEnd(inprefix, pflen, this.chks);
            }
            else // v4
            {
                end = this.FindEnd_v4(inprefix, pflen);
            }

            this.treeView1.BeginUpdate();

            int r = this.MySQLquery(inprefix, end, pflen);

            if (r > 0)
            {
                int i = 0;
                foreach (string[] s in this.liste)
                {
                    node.Nodes.Add(s[0] + "/" + s[1]);

                    if (this.chks == CheckState.Unchecked)
                    {
                        if (s[1] != "64")
                            node.Nodes[i].Nodes.Add(plus);
                    }
                    else if (this.chks == CheckState.Checked)
                    {
                        if (s[1] != "128")
                            node.Nodes[i].Nodes.Add(plus);
                    }
                    i++;
                }
            }
            this.treeView1.EndUpdate();
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            this.AddNode(e.Node);
        }

        private void treeView1_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            e.Node.Nodes.Add(plus);
        }

        private void PrefixSubLevelsForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
                this.Close();
            }
        }

        public void SwitchLanguage(CultureInfo culture)
        {
            this.culture = culture;
            this.Text = StringsDictionary.KeyValue("PrefixSubLevelsForm.Text", this.culture);
            //
            this.label1.Text = StringsDictionary.KeyValue("PrefixSubLevelsForm_label1.Text", this.culture);
            this.label2.Text = StringsDictionary.KeyValue("PrefixSubLevelsForm_label2.Text", this.culture);
            this.dBInfoAboutPrefixToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_getPrefixInfoFromDB.Text", this.culture);

            this.ChangeUILanguage.Invoke(this.culture);
        }

        private void dBInfoAboutPrefixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeView1.SelectedNode != null)
            {
                if (this.treeView1.SelectedNode.Text != "")
                {
                    GetPrefixInfoFromDB getPfxInfo = new GetPrefixInfoFromDB(this.treeView1.SelectedNode.Text,
                        this.MySQLconnection, this.ServerInfo, this.culture, this.currentMode);

                    if (!getPfxInfo.IsDisposed)
                        getPfxInfo.ShowDialog();
                }
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeView1.SelectedNode != null)
            {
                string s = this.treeView1.SelectedNode.Text;
                Clipboard.SetText(s);
            }
        }

        private void PrefixSubLevelsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.MySQLconnection == null)
                dBInfoAboutPrefixToolStripMenuItem.Enabled = false;
            else
                dBInfoAboutPrefixToolStripMenuItem.Enabled = true;
        }

        public void ChangeDatabase(string dbname)
        {
            /* DON'T USE:
            this.label3.Text = "DB changed! NewDB: " + dbname;
            //StringsDictionary.KeyValue("xxxxx", this.culture);
            this.ChangeDB.Invoke(dbname);
            */
        }

        private short QuerySelectedPrefix(string inprefix, short pflen)
        {
            if (MySQLconnection.State == ConnectionState.Closed)
            {
                MessageBox.Show(StringsDictionary.KeyValue("FormDB_MySQLquery_closed", this.culture),
                    StringsDictionary.KeyValue("FormDB_MySQLquery_closed_header", this.culture),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }

            string MySQLcmd = "";

            if (this.currentMode == "v6")
            {
                this.toolStripStatusLabel1.Text = "Database: " + this.ServerInfo.DBname;

                MySQLcmd = "SELECT inet6_ntoa(prefix), pflen, parentpflen "
                    + " from " + "`" + this.ServerInfo.DBname + "`" + ".`" + this.ServerInfo.Tablename + "` "
                    + " WHERE ( prefix=inet6_aton('" + inprefix + "') "
                    + " AND pflen=" + pflen + " );";
            }
            else // v4
            {
                this.toolStripStatusLabel2.Text = this.ServerInfo.DBname_v4;

                MySQLcmd = "SELECT inet_ntoa(prefix), pflen, parentpflen "
                    + " from " + "`" + this.ServerInfo.DBname_v4 + "`" + ".`" + this.ServerInfo.Tablename_v4 + "` "
                    + " WHERE ( prefix=inet_aton('" + inprefix + "') "
                    + " AND pflen=" + pflen + " );";
            }

            OdbcCommand MyCommand = new OdbcCommand(MySQLcmd, MySQLconnection);
            try
            {
                if (this.MySQLconnection.State != ConnectionState.Open)
                    this.MySQLconnection.Open();

                if (this.currentMode == "v6")
                    this.MySQLconnection.ChangeDatabase(this.ServerInfo.DBname);
                else // v4
                    this.MySQLconnection.ChangeDatabase(this.ServerInfo.DBname_v4);

                OdbcDataReader MyDataReader = MyCommand.ExecuteReader();
                int r = MyDataReader.RecordsAffected;

                if (r > 0)
                {
                    MyDataReader.Read();
                    short parentpflen = MyDataReader.GetByte(2);
                    MyDataReader.Close();
                    if (MyDataReader is IDisposable)
                        MyDataReader.Dispose();
                    return parentpflen;
                }
                else  // not found
                {
                    MyDataReader.Close();
                    if (MyDataReader is IDisposable)
                        MyDataReader.Dispose();

                    MessageBox.Show(StringsDictionary.KeyValue("Form1_prefixnotfound", this.culture),
                        "No Prefix", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return -1;  // not found
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: \r\n" + ex.Message, "Error: QuerySelectedPrefix()", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }
        }
    }
}
