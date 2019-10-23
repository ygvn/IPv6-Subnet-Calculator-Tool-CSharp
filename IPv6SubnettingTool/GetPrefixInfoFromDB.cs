/*
 * Copyright (c) 2010-2020 Yucel Guven
 * All rights reserved.
 * 
 * This file is part of IPv6 Subnetting Tool.
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
        #region specials-Yucel
        string prefix = null;
        short pflen = 0;

        public CultureInfo culture;
        OdbcConnection MySQLconnection;
        DBServerInfo ServerInfo = new DBServerInfo();
        OdbcDataReader MyDataReader;
        List<string> liste = new List<string>();

        #endregion
        public GetPrefixInfoFromDB(string prefix, OdbcConnection sqlcon, DBServerInfo servinfo, CultureInfo culture)
        {
            InitializeComponent();
            //
            this.prefix = prefix.Split('/')[0];
            this.pflen = short.Parse(prefix.Split('/')[1]);
            this.MySQLconnection = sqlcon;
            this.ServerInfo = servinfo;
            this.culture = culture;

            if (this.MySQLquery() < 0)
            {
                MessageBox.Show("Error occured in this.MySQLquery()", "MySQL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
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

            int r = 0;
            string MySQLcmd = "";
            this.listBox1.Items.Clear();

            MySQLcmd = "SELECT "
                + " INET6_NTOA(prefix), pflen, netname, person, organization, "
                + "`as-num`, phone, email, status, created, `last-updated` FROM "
                + this.ServerInfo.Tablename
                + " WHERE ( prefix = inet6_aton('" + this.prefix + "')"
                + " AND pflen = " + this.pflen + " )";

            OdbcCommand MyCommand = new OdbcCommand(MySQLcmd, this.MySQLconnection);

            try
            {
                MyDataReader = MyCommand.ExecuteReader();
                r = MyDataReader.RecordsAffected;

                if (r > 0)
                {
                    liste.Clear();

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

        private void GetPrefixInfoFromDB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
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
    }
}
