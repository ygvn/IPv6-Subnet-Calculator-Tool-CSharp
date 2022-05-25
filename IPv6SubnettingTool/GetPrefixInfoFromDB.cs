/*
 * Copyright (c) 2010-2022 Yucel Guven
 * All rights reserved.
 * 
 * This file is part of IPv6 Subnetting Tool.
 * 
 * Version: 5.0
 * Release Date: 23 May 2022
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
using System.Data.Odbc;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace IPv6SubnettingTool
{
    public partial class GetPrefixInfoFromDB : Form
    {
        #region special initials/constants -yucel
        string prefix = null;
        short pflen = 0;
        string parentNet = "";

        public CultureInfo culture;
        OdbcConnection MySQLconnection;
        DBServerInfo ServerInfo = new DBServerInfo();
        OdbcDataReader MyDataReader;
        List<string> liste = new List<string>();
        string currentMode = "";

        #endregion
        public GetPrefixInfoFromDB(string prefix, OdbcConnection sqlcon, DBServerInfo servinfo, CultureInfo culture, string mode)
        {
            InitializeComponent();
            //
            this.prefix = prefix.Split('/')[0];
            this.pflen = short.Parse(prefix.Split('/')[1]);
            this.MySQLconnection = sqlcon;
            this.ServerInfo = servinfo;
            this.culture = culture;
            this.currentMode = mode;

            int r = this.MySQLquery();

            if (r < 0)
            {
                MessageBox.Show("Error: MySQLquery()", "MySQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
        }

        public int MySQLquery()
        {
            if (this.MySQLconnection == null)
            {
                MessageBox.Show("MySQLconnection = null", "MySQLconnection=null",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }

            if (this.MySQLconnection.State == ConnectionState.Closed)
            {
                MessageBox.Show(StringsDictionary.KeyValue("FormDB_MySQLquery_closed", this.culture),
                    StringsDictionary.KeyValue("FormDB_MySQLquery_closed_header", this.culture),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1;
            }

            try
            {
                int r = 0;
                string MySQLcmd = "";
                this.listBox1.Items.Clear();
                liste.Clear();

                string spfx1 = "", spfx2 = "", sDBName = "", sTableName = "";

                if (this.MySQLconnection.State != ConnectionState.Open)
                    this.MySQLconnection.Open();

                if (this.currentMode == "v6")
                {
                    this.MySQLconnection.ChangeDatabase(this.ServerInfo.DBname);

                    spfx1 = " inet6_ntoa(prefix)";
                    spfx2 = " inet6_aton('" + this.prefix + "') ";

                    sDBName = this.ServerInfo.DBname;
                    sTableName = this.ServerInfo.Tablename;

                    if (sDBName == "" || sTableName == "")
                        return -1;
                }
                else // v4
                {
                    this.MySQLconnection.ChangeDatabase(this.ServerInfo.DBname_v4);

                    spfx1 = " inet_ntoa(prefix)";
                    spfx2 = " inet_aton('" + this.prefix + "') ";

                    sDBName = this.ServerInfo.DBname_v4;
                    sTableName = this.ServerInfo.Tablename_v4;

                    if (sDBName == "" || sTableName == "")
                        return -1;
                }

                MySQLcmd = "SELECT "
                    + spfx1
                    + ", pflen, parentpflen, netname, person, organization, "
                    + "`as-num`, phone, email, status, created, `last-updated` FROM "
                    + "`" + sDBName + "`" + ".`" + sTableName + "`"
                    + " WHERE ( prefix = "
                    + spfx2
                    + " AND pflen = " + this.pflen + " )";

                OdbcCommand MyCommand = new OdbcCommand(MySQLcmd, this.MySQLconnection);
                MyDataReader = MyCommand.ExecuteReader();
                r = MyDataReader.RecordsAffected;

                if (r > 0)
                {
                    liste.Clear();
                    Boolean isParentinDB = false;

                    while (MyDataReader.Read())
                    {
                        liste.Add("prefix:\t\t " + MyDataReader.GetString(0) + "/" + MyDataReader.GetByte(1).ToString());

                        isParentinDB = isParentNetinDB(MyDataReader.GetString(0), Convert.ToInt16(MyDataReader.GetByte(2).ToString()), true);

                        if (isParentinDB)
                        {
                            liste.Add("parent:\t\t " + parentNet);
                        }
                        else
                        {
                            string sp = parentNet.Split('/')[1];
                            liste.Add("parent:\t\t " + parentNet + " (/" + sp + "-" + sp + " *Not_in_DB*)");
                        }

                        liste.Add("netname:\t " + MyDataReader.GetString(3));
                        liste.Add("person:\t\t " + MyDataReader.GetString(4));
                        liste.Add("organization:\t " + MyDataReader.GetString(5));
                        liste.Add("as-num:\t\t " + MyDataReader.GetString(6));
                        liste.Add("phone:\t\t " + MyDataReader.GetString(7));
                        liste.Add("email:\t\t " + MyDataReader.GetString(8));
                        liste.Add("status:\t\t " + MyDataReader.GetString(9));
                        liste.Add("created:\t\t " + MyDataReader.GetString(10));
                        liste.Add("last-updated:\t " + MyDataReader.GetString(11));
                        liste.Add(" ");
                    }

                    this.listBox1.Items.AddRange(liste.ToArray());
                }
                else
                {
                    liste.Add(" ");
                    liste.Add(StringsDictionary.KeyValue("Form1_prefixNotFoundinDB.Text", this.culture));
                    this.listBox1.Items.AddRange(liste.ToArray());
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

        public Boolean isParentNetinDB(String prefix, short len, Boolean withParentLength)
        {
            OdbcDataReader MyDataRdr;
            OdbcCommand MyCmd;
            string MySQLcmd;
            Boolean Found = false;
            int r;

            try
            {
                if (withParentLength)
                {
                    string[] sa;
                    string[] Qsa;
                    string sDBname = "", sTableName = "";

                    if (this.currentMode == "v6")
                    {
                        this.parentNet = v6ST.FindParentNet(prefix, len, CheckState.Checked);
                        Qsa = new String[2] { "inet6_ntoa(prefix)", "inet6_aton"};
                        sDBname = this.ServerInfo.DBname;
                        sTableName = this.ServerInfo.Tablename;
                    }
                    else
                    {
                        this.parentNet = v6ST.FindParentNet_v4(prefix, len);
                        Qsa = new String[2] { "inet_ntoa(prefix)", "inet_aton" };
                        sDBname = this.ServerInfo.DBname_v4;
                        sTableName = this.ServerInfo.Tablename_v4;
                    }

                    sa = this.parentNet.Split('/');

                    // is ParentPrefix in Database?:
                    MySQLcmd = "SELECT " + Qsa[0] + ", pflen, parentpflen FROM "
                        + "`" + sDBname + "`." + "`" + sTableName + "`"
                        + " WHERE prefix=" + Qsa[1] + "('" + sa[0] + "')"
                        + " AND pflen=" + sa[1] + " AND parentpflen=" + len.ToString();

                    MyCmd = new OdbcCommand(MySQLcmd, this.MySQLconnection);
                    MyDataRdr = MyCmd.ExecuteReader();
                    r = MyDataRdr.RecordsAffected;

                    string s = "";

                    if (MyDataRdr.Read())
                    {
                        s = MyDataRdr.GetByte(2).ToString();
                        //Console.WriteLine(s);
                        Found = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                /*
                else  // Try not to use, i.e. don't input 'false'.It works but gives same results if there's 2 or more records.
                      // To Do: Will develop in future.
                {     
                    // We don't have parentpflength. First get parentpflen of the input prefix from DB:
                    MySQLcmd = "SELECT parentpflen FROM "
                            + "`" + this.ServerInfo.DBname + "`." + "`" + this.ServerInfo.Tablename + "`"
                            + " WHERE prefix = inet6_aton('" + prefix + "')"
                            + " AND pflen = " + pflen + " ORDER BY parentpflen ASC";

                    MyCmd = new OdbcCommand(MySQLcmd, this.MySQLconnection);
                    MyDataRdr = MyCmd.ExecuteReader();
                    r = MyDataRdr.RecordsAffected;

                    short ppflen = -1;

                    if (MyDataRdr.Read())
                    {
                        ppflen = Convert.ToInt16(MyDataRdr.GetString(0));
                        //Console.WriteLine("FOUND> ", ppflen);
                    }
                    else
                    {
                        return false;
                    }

                    // with ppflen, find ParentNet using v6ST:
                    String[] sa;
                    this.parentNet = v6ST.FindParentNet(prefix, ppflen, CheckState.Checked);
                    sa = this.parentNet.Split('/');

                    // is ParentPrefix in Database?:
                    MySQLcmd = "SELECT inet6_ntoa(prefix), pflen, parentpflen FROM "
                            + "`" + this.ServerInfo.DBname + "`." + "`" + this.ServerInfo.Tablename + "`"
                            + " WHERE prefix=inet6_aton('" + sa[0] + "')"
                            + " AND pflen=" + sa[1] + " AND parentpflen=" + sa[1];

                    MyCmd = new OdbcCommand(MySQLcmd, this.MySQLconnection);
                    MyDataRdr = MyCmd.ExecuteReader();
                    r = MyDataRdr.RecordsAffected;

                    string s = "";

                    if (MyDataRdr.Read())
                    {
                        s = MyDataRdr.GetString(2);
                        //Console.WriteLine("FOUND> ", s);
                        Found = true;
                    }
                    else
                    {
                        return false;
                    }
                }*/

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message +
                    StringsDictionary.KeyValue("FormDB_MySQLquery_exception", this.culture),
                    StringsDictionary.KeyValue("FormDB_MySQLquery_exception_header", this.culture),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return Found;
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

            if (e.Index % 12 == 0)
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

        private void GetPrefixInfoFromDB_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.Escape:
                    {
                        IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
                        this.Close();
                        break;
                    }
                case Keys.F5:
                    {
                        this.refreshToolStripMenuItem_Click(null, null);
                        break;
                    }
                default:
                    break;
            }

            //if (e.KeyCode == Keys.Escape)
            //    IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
            //    this.Close();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox1.Visible = false;
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                listBox1.SetSelected(i, true);
            }
            listBox1.Visible = true;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "";
            foreach (object o in listBox1.SelectedItems)
            {
                s += o.ToString() + Environment.NewLine;
            }
            if (s != "")
                Clipboard.SetText(s);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int r = this.MySQLquery();

            if (r < 0)
            {
                MessageBox.Show("Error: MySQLquery()", "MySQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }
        }

        private void getPrefixInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex % 12 == 1)
            {
                if (MySQLconnection != null)
                {
                    if (this.listBox1.SelectedItem != null && this.listBox1.SelectedItem.ToString().Trim() != "")
                    {
                        String selected = this.listBox1.SelectedItem.ToString().Trim().Split(' ')[1];

                        GetPrefixInfoFromDB getpfxdbinfo =
                            new GetPrefixInfoFromDB(selected, this.MySQLconnection, this.ServerInfo, this.culture, this.currentMode);

                        //getpfxdbinfo.ShowDialog();

                        getpfxdbinfo.Show();
                        getpfxdbinfo.Location = new Point(this.Location.X + 20, this.Location.Y + 20);
                        IPv6SubnettingTool.Form1.windowsList.Add(
                            new WindowsList(getpfxdbinfo, getpfxdbinfo.Name, getpfxdbinfo.GetHashCode(), this.currentMode));
                    }
                }
            }

        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.listBox1.Items[0].ToString().Contains("prefix"))
            {
                switch (listBox1.SelectedIndex % 12)
                {
                    case 0:
                        { // prefix:
                            this.modifyPrefixToolStripMenuItem.Enabled = true;
                            this.getPrefixInfoToolStripMenuItem.Enabled = false;
                            break;
                        }
                    case 1:
                        { // parent:
                            this.modifyPrefixToolStripMenuItem.Enabled = false;
                            this.getPrefixInfoToolStripMenuItem.Enabled = true;
                            break;
                        }
                    default:
                        {
                            this.modifyPrefixToolStripMenuItem.Enabled = false;
                            this.getPrefixInfoToolStripMenuItem.Enabled = false;
                            break;
                        }
                }
            } else
            {
                this.modifyPrefixToolStripMenuItem.Enabled = false;
                this.getPrefixInfoToolStripMenuItem.Enabled = false;
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.listBox1.SelectedIndex % 12 == 1)
                {
                    getPrefixInfoToolStripMenuItem_Click(null, null);
                }
            }
        }

        private void modifyPrefixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MySQLconnection != null && (this.listBox1.SelectedIndex % 12 == 0))
            {
                string snet = "";
                short plen = 0;
                short parentpflen;


                if (this.listBox1.SelectedItem.ToString() != null && this.listBox1.SelectedItem.ToString().Trim() != "")
                {
                    string selected = this.listBox1.SelectedItem.ToString().Split(' ')[1].Trim();
                    int sIdx = this.listBox1.SelectedIndex;
                    snet = selected.Split('/')[0].Trim();
                    plen = Convert.ToInt16(selected.Split('/')[1].Trim());

                    parentpflen = Convert.ToInt16(this.listBox1.Items[sIdx + 1].ToString().Split(' ')[1].Split('/')[1]);

                    DatabaseUI dbui = new DatabaseUI(snet, plen, parentpflen, MySQLconnection,
                        this.ServerInfo, this.culture, this.currentMode, this.listBox1.Font);

                    if (!dbui.IsDisposed)
                    {
                        dbui.Show();
                        IPv6SubnettingTool.Form1.windowsList.Add(new WindowsList(dbui, dbui.Name, dbui.GetHashCode(), this.currentMode));

                        IPv6SubnettingTool.Form1.changeDBstate += dbui.DBStateChange;
                        IPv6SubnettingTool.Form1.ChangeUILanguage += dbui.SwitchLanguage;
                    }
                }
            }
        }

        private void GetPrefixInfoFromDB_FormClosing(object sender, FormClosingEventArgs e)
        {
            IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
        }
    }
}
