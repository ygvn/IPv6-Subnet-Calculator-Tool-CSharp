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
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Data.Odbc;
using System.Numerics;

namespace IPv6SubnettingTool
{
    public partial class StatsUsage : Form
    {
        #region special initials/constants -yucel
        string prefix = "";
        string end = "";
        short pflen = 0;
        short parentpflen = 0;
        //
        int result = 0;
        BigInteger rangetotal = BigInteger.Zero;
        float angle = 0;
        float percent = 0;
        float filledh = 0;
        string cons = "", rem = "";
        //
        CheckState chks = CheckState.Unchecked;
        OdbcConnection MySQLconnection = null;
        DBServerInfo ServerInfo = new DBServerInfo();
        OdbcDataReader MyDataReader;
        string currentMode = "";
        //
        Graphics graph;
        SolidBrush fillred = new SolidBrush(Color.Red);
        SolidBrush fillwhite = new SolidBrush(Color.White);
        //
        public CultureInfo culture;
        public delegate void ChangeWinFormStringsDelegate(CultureInfo culture);
        public event ChangeWinFormStringsDelegate ChangeUILanguage = delegate { };
        //
        public delegate void ChangeDBState(OdbcConnection dbconn, int info);
        public event ChangeDBState changeDBstate = delegate { };
        //
        public delegate void ChangeDatabaseDelegate(string dbname);
        public event ChangeDatabaseDelegate ChangeDB = delegate { };
        #endregion

        public StatsUsage(string prefix, string end, short parentpflen, short pflen, CheckState chks,
            OdbcConnection sqlcon, DBServerInfo servinfo, CultureInfo culture, string mode)
        {
            InitializeComponent();

            this.currentMode = mode;
            this.prefix = prefix;
            this.end = end;
            this.pflen = pflen;
            this.parentpflen = parentpflen;
            this.chks = chks;
            this.MySQLconnection = sqlcon;
            this.ServerInfo = servinfo.ShallowCopy();
            this.culture = culture;
            this.SwitchLanguage(culture);
            this.graph = this.CreateGraphics();

            this.label2.Text = prefix + " - " + "/" + pflen;
            this.groupBox2.Text = "/" + pflen
                + StringsDictionary.KeyValue("StatsUsageForm_groupBox2.Text", this.culture);

            if (this.currentMode == "v6")
            {
                this.toolStripStatusLabel1.Text = "Database: " + this.ServerInfo.DBname;
            }
            else // v4
            {
                this.toolStripStatusLabel1.Text = "Database: " + this.ServerInfo.DBname_v4;
            }

            if (this.MySQLconnection == null)
            {
                this.toolStripStatusLabel2.Text = "db=Down";

                this.toolStripStatusLabel1.Text = "Database: null";
            }
            else
            {
                if (this.MySQLconnection.State == ConnectionState.Open)
                {
                    this.toolStripStatusLabel2.Text = "db=Up";
                }
                else if (this.MySQLconnection.State == ConnectionState.Closed)
                {
                    this.toolStripStatusLabel2.Text = "db=Down";
                }
            }

            this.Calculate();
        }

        private void Calculate()
        {
            this.rangetotal = BigInteger.One << (this.pflen - this.parentpflen);
            
            this.result = this.MySQLquery();

            if (this.result >= 0)
            {
                float ratio = ((float)this.result / (float)this.rangetotal);

                this.angle = ratio * 360;
                this.percent = ratio * 100;
                this.filledh = ratio * 140;

                if (ratio > 0.01 || ratio == 0)
                {
                    cons = this.percent.ToString("0.00"); // consumed;
                    rem = ((float)100 - float.Parse(cons)).ToString("0.00"); // remaining
                }
                else
                {
                    cons = this.percent.ToString("0.00000"); // consumed;
                    rem = ((float)100 - float.Parse(cons)).ToString("0.00000"); // remaining
                }

                UpdateTextBox();

                this.StatsUsage_Paint(null, null);
            }
        }

        private void UpdateTextBox()
        {
            this.textBox1.Text = StringsDictionary.KeyValue("StatsUsageForm_textBox1.Text.p1", this.culture)
                + this.label2.Text + Environment.NewLine + Environment.NewLine;
            this.textBox1.Text += StringsDictionary.KeyValue("StatsUsageForm_textBox1.Text.p2", this.culture)
                + this.rangetotal.ToString() + Environment.NewLine;
            this.textBox1.Text += StringsDictionary.KeyValue("StatsUsageForm_textBox1.Text.p3", this.culture)
                + this.result.ToString() + Environment.NewLine;
            this.textBox1.Text += StringsDictionary.KeyValue("StatsUsageForm_textBox1.Text.p4", this.culture)
                + (this.rangetotal - this.result).ToString() + Environment.NewLine;

            this.textBox1.Text += StringsDictionary.KeyValue("StatsUsageForm_textBox1.Text.p5", this.culture)
                + this.cons + " %" + Environment.NewLine;
            this.textBox1.Text += StringsDictionary.KeyValue("StatsUsageForm_textBox1.Text.p6", this.culture)
                + this.rem + " %";

            this.label12.Text = cons + StringsDictionary.KeyValue("StatsUsageForm_label12.Text", this.culture);
            this.label13.Text = rem + StringsDictionary.KeyValue("StatsUsageForm_label13.Text", this.culture);

        }

        public void SwitchLanguage(CultureInfo culture)
        {
            this.culture = culture;
            this.Text = StringsDictionary.KeyValue("StatsUsageForm.Text", this.culture);
            this.button1.Text = StringsDictionary.KeyValue("StatsUsageForm_button1.Text", this.culture);
            this.label1.Text = StringsDictionary.KeyValue("StatsUsageForm_label1.Text", this.culture);
            this.groupBox2.Text = "/" + pflen + " "
                + StringsDictionary.KeyValue("StatsUsageForm_groupBox2.Text", this.culture);
            //
            this.UpdateTextBox();

            this.ChangeUILanguage.Invoke(this.culture);
        }

        private void StatsUsage_Paint(object sender, PaintEventArgs e)
        {
            this.graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

            this.graph.Clear(StatsUsage.DefaultBackColor);
            this.graph.FillRectangle(fillwhite, 13, 64, 383, 170);

            int x = 85, y = 80;
            int w = 140, h = 140;

            this.graph.FillPie(fillred, x, y, w, h, 0, (-1) * (float)this.angle);
            this.graph.DrawPie(Pens.Red, x, y, w, h, 0, (-1) * (float)this.angle);

            this.graph.DrawEllipse(Pens.Red, x, y, w, h);
            
            this.graph.DrawLine(Pens.RoyalBlue, 155, 150, 225, 150);

            this.graph.DrawRectangle(Pens.Red, 35, 80, 20, 140);
            this.graph.FillRectangle(fillred, 35, (220 - this.filledh), 20, this.filledh);
        }

        public int MySQLquery()
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
                int r = 0;
                string MySQLcmd = "";
                string spfx = "", sDBName = "", sTableName = "";

                if (this.MySQLconnection.State != ConnectionState.Open)
                    this.MySQLconnection.Open();

                if (this.currentMode == "v6")
                {
                    this.MySQLconnection.ChangeDatabase(this.ServerInfo.DBname);

                    spfx = "inet6_aton('";

                    sDBName = this.ServerInfo.DBname;
                    sTableName = this.ServerInfo.Tablename;

                    this.toolStripStatusLabel1.Text = "Database: " + sDBName;
                }
                else // v4
                {
                    this.MySQLconnection.ChangeDatabase(this.ServerInfo.DBname_v4);

                    spfx = "inet_aton('";

                    sDBName = this.ServerInfo.DBname_v4;
                    sTableName = this.ServerInfo.Tablename_v4;

                    this.toolStripStatusLabel1.Text = "Database: " + sDBName;
                }

                MySQLcmd = "SELECT COUNT(*) FROM "
                    + "`" + sDBName + "`." + "`" + sTableName + "`"
                    + " WHERE ( prefix BETWEEN " + spfx + this.prefix.Split('/')[0] + "')"
                    + " AND " + spfx + this.end.Split('/')[0] + "')"
                    + " AND parentpflen= " + parentpflen + " AND pflen= " + pflen + " ) ";

                OdbcCommand MyCommand = new OdbcCommand(MySQLcmd, this.MySQLconnection);

                MyDataReader = MyCommand.ExecuteReader();
                MyDataReader.Read();
                r = int.Parse(MyDataReader.GetString(0));

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

        private void StatsUsage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
                this.Close();
            }
        }

        public void DBStateChange(OdbcConnection dbconn, int info)
        {
            this.MySQLconnection = dbconn;

            if (info == -1)
            {
                this.changeDBstate.Invoke(this.MySQLconnection, info); // One child

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
                this.changeDBstate.Invoke(this.MySQLconnection, info); // One child

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

        private void button1_Click(object sender, EventArgs e)
        {
            ListAssignedfromDB listassigned = new ListAssignedfromDB(this.prefix, this.end, 
                this.parentpflen, this.pflen, this.chks, this.MySQLconnection, 
                this.ServerInfo, this.culture, this.currentMode);

            if (!listassigned.IsDisposed)
            {
                listassigned.Show();
                //
                IPv6SubnettingTool.Form1.windowsList.Add(new WindowsList(listassigned, listassigned.Name, listassigned.GetHashCode(), this.currentMode));

                this.ChangeUILanguage += listassigned.SwitchLanguage;
                this.changeDBstate += listassigned.DBStateChange;
                //this.ChangeDB += listassigned.ChangeDatabase;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Calculate();
        }

        private void StatsUsage_FormClosing(object sender, FormClosingEventArgs e)
        {
            IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
        }
        public void ChangeDatabase(string dbname)
        {
            /* DON'T USE:
            //this.toolStripStatusLabel1.Text = "Database: " + dbname;
            //StringsDictionary.KeyValue("FormDB_insertrecord", this.culture);
            //this.ChangeDB.Invoke(dbname);
            */
        }
    }
}
