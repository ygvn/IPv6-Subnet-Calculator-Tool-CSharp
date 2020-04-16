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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Numerics;
using System.Data.Odbc;

namespace IPv6SubnettingTool
{
    public partial class ListSubnetRange : Form
    {

        #region special initials/constants -yucel
        public const int ID = 1; // ID of this Form.
        public int incomingID;
        SEaddress StartEnd = new SEaddress();
        short parentpflen = 0;
        SEaddress subnets = new SEaddress();
        SEaddress page = new SEaddress();
        public const int upto = 128;
        Graphics graph;
        BigInteger currentidx = BigInteger.Zero;
        BigInteger pix = BigInteger.Zero;
        public string findpfx = "";
        public string GotoForm_PrevValue = "";
        public BigInteger NumberOfSubnets = BigInteger.Zero;
        BigInteger gotovalue = BigInteger.Zero;
        BigInteger maxvalue  = BigInteger.Zero;
        CheckState is128Checked;
        int maxfontwidth = 0;
        public CultureInfo culture;
        string currentMode = "";
        Font font;
        bool SelectedRange = false;
        public delegate void ChangeWinFormStringsDelegate(CultureInfo culture);
        public event ChangeWinFormStringsDelegate ChangeUILanguage = delegate { };
        //DB
        OdbcConnection MySQLconnection;
        public DBServerInfo ServerInfo = new DBServerInfo();
        //
        public delegate void ChangeDBState(OdbcConnection dbconn, int info);
        public event ChangeDBState changeDBstate = delegate { };
        #endregion

        public string findprefix
        {
            get { return this.findpfx; }
            set { this.findpfx = value; }
        }

        public ListSubnetRange(SEaddress input, string sin, int slash, int subnetslash, CheckState is128Checked, 
            CultureInfo culture, OdbcConnection sqlcon, 
            DBServerInfo servinfo, string mode, Font font, bool selectedrange)
        {
            InitializeComponent();
            //
            this.font = font;
            this.SelectedRange = selectedrange;

            this.listBox1.Font = font;
            this.textBox1.Font = font;
            this.textBox2.Font = font;
            this.label5.Font = font;
            this.label6.Font = font;

            this.graph = this.CreateGraphics();
            this.graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

            this.ServerInfo = servinfo.ShallowCopy();
            this.parentpflen = (short)input.slash;
            this.StartEnd.ID = ID;
            this.incomingID = input.ID;
            this.culture = culture;
            this.currentMode = mode;

            this.SwitchLanguage(this.culture);

            this.is128Checked = is128Checked;
            this.Forwd.Enabled = false;
            this.Backwd.Enabled = false;
            this.LastPage.Enabled = false;

            this.MySQLconnection = sqlcon;

            if (this.currentMode == "v6")
            {
                string[] sa = sin.Split(' ');
                sa = sa[1].Split('/');

                string s = v6ST.FormalizeAddr(sa[0]);
                s = v6ST.Kolonlar(s, this.is128Checked);
                sa = s.Split(':');
                s = "";

                if (this.is128Checked == CheckState.Checked)
                {
                    s = "0" + sa[0] + sa[1] + sa[2] + sa[3] + sa[4] + sa[5] + sa[6] + sa[7];
                    StartEnd.ResultIPAddr = BigInteger.Parse(s, NumberStyles.AllowHexSpecifier);
                    if (s.Length > 32)
                        s = s.Substring(1, 32);

                    this.label3.Text = StringsDictionary.KeyValue("ListSubnetRange_label3.Text", this.culture);
                }
                else if (this.is128Checked == CheckState.Unchecked)
                {
                    s = "0" + sa[0] + sa[1] + sa[2] + sa[3];
                    StartEnd.ResultIPAddr = BigInteger.Parse(s, NumberStyles.AllowHexSpecifier);
                    if (s.Length > 16)
                        s = s.Substring(1, 16);

                    this.label3.Text = StringsDictionary.KeyValue("ListSubnetRange_label3.Text", this.culture);
                }

                StartEnd.slash = subnetslash;
                StartEnd.subnetslash = subnetslash;

                StartEnd = v6ST.StartEndAddresses(StartEnd, this.is128Checked);
                NumberOfSubnets = StartEnd.End - StartEnd.Start + BigInteger.One;

                String s1 = "", s2 = "";

                if (this.is128Checked == CheckState.Unchecked)
                {
                    this.DefaultView();

                    s1 = String.Format("{0:x}", StartEnd.Start);
                    if (s1.Length > 16)
                        s1 = s1.Substring(1, 16);
                    s1 = v6ST.Kolonlar(s1, this.is128Checked);
                    s1 = v6ST.CompressAddress(s1);

                    s2 = String.Format("{0:x}", StartEnd.End);
                    if (s2.Length > 16)
                        s2 = s2.Substring(1, 16);
                    s2 = v6ST.Kolonlar(s2, this.is128Checked);
                    s2 = v6ST.CompressAddress(s2);
                }
                else if (this.is128Checked == CheckState.Checked)
                {
                    this.ExpandView();

                    s1 = String.Format("{0:x}", StartEnd.Start);
                    if (s1.Length > 32)
                        s1 = s1.Substring(1, 32);
                    s1 = v6ST.Kolonlar(s1, this.is128Checked);
                    s1 = v6ST.CompressAddress(s1);

                    s2 = String.Format("{0:x}", StartEnd.End);
                    if (s2.Length > 32)
                        s2 = s2.Substring(1, 32);
                    s2 = v6ST.Kolonlar(s2, this.is128Checked);
                    s2 = v6ST.CompressAddress(s2);
                }

                this.textBox1.Text = String.Format("{0}", NumberOfSubnets);
                this.label5.Text = s1 + "/" + StartEnd.subnetslash;
                this.label6.Text = s2 + "/" + StartEnd.subnetslash;

                if (this.MySQLconnection != null)
                {
                    if (this.MySQLconnection.State == System.Data.ConnectionState.Open)
                    {
                        this.toolStripStatusLabel2.Text = "db=Up";

                        if (this.ServerInfo.DBname != "")
                            this.toolStripStatusLabel1.Text = "Database: " + this.ServerInfo.DBname;
                        else
                            this.toolStripStatusLabel1.Text = "Database: null";
                    }
                    else
                    {
                        this.toolStripStatusLabel2.Text = "db=Down";
                        this.toolStripStatusLabel1.Text = "Database: null";
                    }
                }
                else
                {
                    this.toolStripStatusLabel2.Text = "db=Down";
                    this.toolStripStatusLabel1.Text = "Database: null";
                }
            }
            else // v4
            {
                this.DefaultView();

                this.Forwd.Enabled = false;
                this.Backwd.Enabled = false;
                this.LastPage.Enabled = false;

                string[] sa = sin.Split(' ');
                sa = sa[1].Split('/');

                string s = v6ST.FormalizeAddr_v4(sa[0]);

                StartEnd.ResultIPAddr = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
                if (s.Length > 8)
                    s = s.Substring(1, 8);
                else if (s.Length < 8)
                {
                    s = s.PadLeft(8, '0');
                }

                this.label3.Text = StringsDictionary.KeyValue("ListSubnetRange_label3.Text", this.culture);

                StartEnd.slash = subnetslash;
                StartEnd.subnetslash = subnetslash;

                StartEnd = v6ST.StartEndAddresses_v4(StartEnd);
                NumberOfSubnets = StartEnd.End - StartEnd.Start + BigInteger.One;

                String s1 = "", s2 = "";

                s1 = String.Format("{0:x}", StartEnd.Start);

                s1 = v6ST.IPv4Format(s1);

                s2 = String.Format("{0:x}", StartEnd.End);

                s2 = v6ST.IPv4Format(s2);

                this.textBox1.Text = String.Format("{0}", NumberOfSubnets);
                this.label5.Text = s1 + "/" + StartEnd.subnetslash;
                this.label6.Text = s2 + "/" + StartEnd.subnetslash;

                if (this.MySQLconnection != null)
                {
                    if (this.MySQLconnection.State == System.Data.ConnectionState.Open)
                    {
                        this.toolStripStatusLabel2.Text = "db=Up";

                        if (this.ServerInfo.DBname_v4 != "")
                            this.toolStripStatusLabel1.Text = "Database: " + this.ServerInfo.DBname_v4;
                        else
                            this.toolStripStatusLabel1.Text = "Database: null";
                    }
                    else
                    {
                        this.toolStripStatusLabel2.Text = "db=Down";
                        this.toolStripStatusLabel1.Text = "Database: null";
                    }
                }
                else
                {
                    this.toolStripStatusLabel2.Text = "db=Down";
                    this.toolStripStatusLabel1.Text = "Database: null";
                }


            }

            this.FirstPage_Click(null, null);
        }

        private void ExpandView()
        {
            this.Size = new Size(645, 500);
            this.groupBox1.Size = new Size(603, 44);
            this.listBox1.Size = new Size(603, 280);
            this.textBox1.Size = new Size(353, 20);
            this.textBox1.MaxLength = 45;
            this.textBox2.Size = new Size(353, 20);
            this.textBox2.MaxLength = 45;
            this.textBox3.Location = new Point(567, 77);
            this.buttonGoto.Location = new Point(509, 399);
            this.toolStripStatusLabel1.Size = new Size(527, 17);
            this.toolStripStatusLabel2.Size = new Size(90, 17);
            //
            graph.Clear(ListSubnetRange.DefaultBackColor);
        }

        private void DefaultView()
        {
            this.Size = new Size(428, 500);
            this.groupBox1.Size = new Size(386, 44);
            this.listBox1.Size = new Size(386, 280);
            this.textBox1.Size = new Size(136, 20);
            this.textBox1.MaxLength = 21;
            this.textBox2.Size = new Size(136, 20);
            this.textBox2.MaxLength = 20;
            this.textBox3.Location = new Point(350, 77);
            this.buttonGoto.Location = new Point(292, 399);
            this.toolStripStatusLabel1.Size = new Size(310, 17);
            this.toolStripStatusLabel2.Size = new Size(90, 17);
            //
            graph.Clear(ListSubnetRange.DefaultBackColor);
        }

        private void UpdateCount()
        {
            this.textBox3.Text = "[" + this.listBox1.Items.Count.ToString()
                + StringsDictionary.KeyValue("ListSubnetRange_UpdateCount_textBox3.Text", this.culture);

            if (this.listBox1.Items.Count > 0)
            {
                this.currentidx =
                    BigInteger.Parse(this.listBox1.Items[0].ToString().Split('>')[0].TrimStart('p'));

                if (this.NumberOfSubnets / 128 >= 1)
                {
                    this.pix = this.NumberOfSubnets / 128;
                }
                else
                {
                    this.pix = 128;
                }
            }

            this.ListSubnetRange_Paint(null, null);
        }

        private void FirstPage_Click(object sender, EventArgs e)
        {
            this.maxfontwidth = 0;
            this.listBox1.HorizontalExtent = 0;

            if (this.currentMode == "v6")
            {
                subnets.Start = page.Start = StartEnd.Start;
                page.End = BigInteger.Zero;
                subnets.subnetslash = StartEnd.subnetslash;
                subnets.upto = upto;

                subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
                subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;

                if (subnets.Start.Equals(StartEnd.End))
                {
                    UpdateCount();
                    return;
                }

                this.listBox1.Items.Clear();

                subnets = v6ST.ListSubRangeFirstPage(subnets, is128Checked, this.SelectedRange);
                page.End = subnets.Start - BigInteger.One;
                this.listBox1.Items.AddRange(subnets.liste.ToArray());

                if (NumberOfSubnets <= upto)
                {
                    this.Backwd.Enabled = false;
                    this.Forwd.Enabled = false;
                    this.LastPage.Enabled = false;
                }
                else
                {
                    this.Backwd.Enabled = false;
                    this.Forwd.Enabled = true;
                    this.LastPage.Enabled = true;
                }
            }
            else // v4
            {
                subnets.Start = page.Start = StartEnd.Start;
                page.End = BigInteger.Zero;
                subnets.subnetslash = StartEnd.subnetslash;
                subnets.upto = upto;

                subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
                subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;

                if (subnets.Start.Equals(StartEnd.End))
                {
                    UpdateCount();
                    return;
                }

                this.listBox1.Items.Clear();

                subnets = v6ST.ListSubRangeFirstPage_v4(subnets, this.SelectedRange);
                page.End = subnets.Start - BigInteger.One;
                this.listBox1.Items.AddRange(subnets.liste.ToArray());

                if (NumberOfSubnets <= upto)
                {
                    this.Backwd.Enabled = false;
                    this.Forwd.Enabled = false;
                    this.LastPage.Enabled = false;
                }
                else
                {
                    this.Backwd.Enabled = false;
                    this.Forwd.Enabled = true;
                    this.LastPage.Enabled = true;
                }
            }

            UpdateCount();
        }

        private void Backwd_Click(object sender, EventArgs e)
        {
            this.maxfontwidth = 0;
            this.listBox1.HorizontalExtent = 0;

            if (this.currentMode == "v6")
            {
                subnets.Start = page.End = page.Start - BigInteger.One;
                subnets.subnetslash = StartEnd.subnetslash;
                subnets.upto = upto;

                subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
                subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;

                this.listBox1.Items.Clear();

                subnets = v6ST.ListSubRangePageBackward(subnets, is128Checked, this.SelectedRange);
                page.Start = subnets.Start + BigInteger.One;

                this.listBox1.Items.AddRange(subnets.liste.ToArray());

                if (subnets.subnetidx == BigInteger.Zero)
                {
                    this.Backwd.Enabled = false;
                    this.Forwd.Enabled = true;
                    this.LastPage.Enabled = true;
                    UpdateCount();
                    return;
                }
                else
                {
                    this.Forwd.Enabled = true;
                    this.LastPage.Enabled = true;
                }
            }
            else // v4
            {
                subnets.Start = page.End = page.Start - BigInteger.One;
                subnets.subnetslash = StartEnd.subnetslash;
                subnets.upto = upto;

                subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
                subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;

                this.listBox1.Items.Clear();

                subnets = v6ST.ListSubRangePageBackward_v4(subnets, this.SelectedRange);
                page.Start = subnets.Start + BigInteger.One;

                this.listBox1.Items.AddRange(subnets.liste.ToArray());

                if (subnets.subnetidx == BigInteger.Zero)
                {
                    this.Backwd.Enabled = false;
                    this.Forwd.Enabled = true;
                    this.LastPage.Enabled = true;
                    UpdateCount();
                    return;
                }
                else
                {
                    this.Forwd.Enabled = true;
                    this.LastPage.Enabled = true;
                }
            }
            
            UpdateCount();
        }

        private void Forwd_Click(object sender, EventArgs e)
        {
            this.maxfontwidth = 0;
            this.listBox1.HorizontalExtent = 0;
            this.maxfontwidth = 0;
            this.listBox1.HorizontalExtent = 0;

            if (this.currentMode == "v6")
            {
                subnets.Start = page.Start = page.End + BigInteger.One;
                subnets.subnetslash = StartEnd.subnetslash;
                subnets.upto = upto;
                subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;
                subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;

                this.listBox1.Items.Clear();

                subnets = v6ST.ListSubRangePageForward(subnets, is128Checked, this.SelectedRange);

                page.End = subnets.Start - BigInteger.One;
                this.listBox1.Items.AddRange(subnets.liste.ToArray());


                if (subnets.subnetidx == (NumberOfSubnets - BigInteger.One))
                {
                    this.Forwd.Enabled = false;
                    this.LastPage.Enabled = false;
                    this.Backwd.Enabled = true;
                    UpdateCount();
                    return;
                }
                else
                {
                    this.Backwd.Enabled = true;
                    this.LastPage.Enabled = true;
                }
            }
            else // v4
            {
                subnets.Start = page.Start = page.End + BigInteger.One;
                subnets.subnetslash = StartEnd.subnetslash;
                subnets.upto = upto;
                subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;
                subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;

                this.listBox1.Items.Clear();

                subnets = v6ST.ListSubRangePageForward_v4(subnets, this.SelectedRange);

                page.End = subnets.Start - BigInteger.One;
                this.listBox1.Items.AddRange(subnets.liste.ToArray());


                if (subnets.subnetidx == (NumberOfSubnets - BigInteger.One))
                {
                    this.Forwd.Enabled = false;
                    this.LastPage.Enabled = false;
                    this.Backwd.Enabled = true;
                    UpdateCount();
                    return;
                }
                else
                {
                    this.Backwd.Enabled = true;
                    this.LastPage.Enabled = true;
                }

            }

            UpdateCount();
        }

        private void Last_Click(object sender, EventArgs e)
        {
            this.maxfontwidth = 0;
            this.listBox1.HorizontalExtent = 0;

            if (this.currentMode == "v6")
            {
                subnets.Start = page.End = StartEnd.End;
                subnets.subnetslash = StartEnd.subnetslash;
                subnets.upto = upto;
                subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
                subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;

                this.listBox1.Items.Clear();
                subnets = v6ST.ListSubRangeLastPage(subnets, is128Checked, this.SelectedRange);
                this.listBox1.Items.AddRange(subnets.liste.ToArray());
                page.Start = subnets.Start + BigInteger.One;

                if (NumberOfSubnets > upto)
                {
                    this.Backwd.Enabled = true;
                    this.Forwd.Enabled = false;
                    this.LastPage.Enabled = false;
                }
                else
                {
                    this.Backwd.Enabled = false;
                    this.Forwd.Enabled = false;
                    this.LastPage.Enabled = false;
                }
            }
            else // v4
            {
                subnets.Start = page.End = StartEnd.End;
                subnets.subnetslash = StartEnd.subnetslash;
                subnets.upto = upto;
                subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
                subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;

                this.listBox1.Items.Clear();
                subnets = v6ST.ListSubRangeLastPage_v4(subnets, this.SelectedRange);
                this.listBox1.Items.AddRange(subnets.liste.ToArray());
                page.Start = subnets.Start + BigInteger.One;

                if (NumberOfSubnets > upto)
                {
                    this.Backwd.Enabled = true;
                    this.Forwd.Enabled = false;
                    this.LastPage.Enabled = false;
                }
                else
                {
                    this.Backwd.Enabled = false;
                    this.Forwd.Enabled = false;
                    this.LastPage.Enabled = false;
                }
            }

            UpdateCount();
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
            if ( s != "")
                Clipboard.SetText(s);
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
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

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;
            
            ListBox lb = (ListBox)sender;
            Graphics g = e.Graphics;
            SolidBrush sback = new SolidBrush(e.BackColor);
            SolidBrush sfore = new SolidBrush(e.ForeColor);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

            if (e.Index % 2 == 0 && this.StartEnd.subnetslash != 128)
            {
                e.DrawBackground();
                DrawItemState st = DrawItemState.Selected;

                if ((e.State & st) != st)
                {
                    // Turquaz= FF(A) 40E0D0(RGB)
                    Color color = Color.FromArgb(30, 64, 224, 208);
                    g.FillRectangle(new SolidBrush(color), e.Bounds);
                    //g.FillRectangle(new SolidBrush(Color.WhiteSmoke), e.Bounds);
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
            else if (e.Index % 2 == 0 && this.StartEnd.subnetslash == 128)
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

            // Last item is the widest one due to its index but may not be depending on the font-width
            // e.g., '0' and 'f' does not have he same width if the font is not fixed-width.
            // ps: Try to use Fixed-width Font if available.

            lb.HorizontalScrollbar = true;
            int horzSize = (int)g.MeasureString(lb.Items[e.Index].ToString(), lb.Font).Width;
            if (horzSize > maxfontwidth)
            {
                maxfontwidth = horzSize;
                lb.HorizontalExtent = maxfontwidth;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter 
                || e.KeyChar == (char)Keys.Return)
            {
                textBox2.Text = textBox2.Text.Trim();

                if (textBox2.Text == "")
                    return;

                for (int i = 0; i < textBox2.Text.Length; i++)
                {
                    if (textBox2.Text[i] < '0' || textBox2.Text[i] > '9')
                    {
                        this.textBox2.Text = "0";
                        return;
                    }
                }

                gotovalue = BigInteger.Parse(this.textBox2.Text);
                maxvalue  = BigInteger.Parse(this.textBox1.Text);
                this.textBox2.Text = gotovalue.ToString();

                if (gotovalue > (maxvalue - BigInteger.One))
                {
                    this.textBox2.Text = (maxvalue - BigInteger.One).ToString();
                    this.textBox2.BackColor = Color.FromKnownColor(KnownColor.Info);
                    this.textBox2.SelectAll();
                    return;
                }
            }

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (this.textBox2.Text != "")
            {
                if ( BigInteger.Parse(this.textBox2.Text, NumberStyles.Number) > (NumberOfSubnets - BigInteger.One))
                {
                    this.textBox2.Text = (NumberOfSubnets - BigInteger.One).ToString();
                    this.textBox2.BackColor = Color.FromKnownColor(KnownColor.Info);
                }
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            this.textBox2.BackColor = Color.White;
        }

        private void listAllDNSReverseZonesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.SelectedRange)
            {
                ListDnsReverses dnsr = new ListDnsReverses(StartEnd, this.is128Checked, this.culture, this.currentMode, this.listBox1.Font);
                if (!dnsr.IsDisposed)
                {
                    dnsr.Show();
                    //
                    IPv6SubnettingTool.Form1.windowsList.Add(new WindowsList(dnsr, dnsr.Name, dnsr.GetHashCode(), this.currentMode));
                    this.ChangeUILanguage += dnsr.SwitchLanguage;
                }
            }
            else
            {
                int tmp = StartEnd.subnetslash;

                if (this.currentMode == "v6")
                {
                    if (this.is128Checked == CheckState.Unchecked)
                    {
                        StartEnd.subnetslash = 64;
                    }
                    else if (this.is128Checked == CheckState.Checked)
                    {
                        StartEnd.subnetslash = 128;
                    }
                }
                else // v4
                {
                    StartEnd.subnetslash = 32;
                }

                ListDnsReverses dnsr = new ListDnsReverses(StartEnd, this.is128Checked, this.culture, this.currentMode, this.listBox1.Font);

                StartEnd.subnetslash = tmp;

                if (!dnsr.IsDisposed)
                {
                    dnsr.Show();
                    //
                    IPv6SubnettingTool.Form1.windowsList.Add(new WindowsList(dnsr, dnsr.Name, dnsr.GetHashCode(), this.currentMode));
                    this.ChangeUILanguage += dnsr.SwitchLanguage;
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (this.listBox1.Items.Count > 0)
            {
                this.SearchprefixtoolStripMenuItem1.Enabled = true;
                this.listAllDNSReverseZonesToolStripMenuItem.Enabled = true;
                this.savetoolStripMenuItem1.Enabled = true;

                if (this.listBox1.SelectedItem != null && this.listBox1.SelectedItem.ToString() != ""
                    && this.listBox1.SelectedIndex != -1  && this.MySQLconnection != null)
                {
                    this.sendtodatabasetoolStripMenuItem1.Enabled = true;
                    this.getPrefixInfoFromDBToolStripMenuItem.Enabled = true;
                }
                else
                {
                    this.sendtodatabasetoolStripMenuItem1.Enabled = false;
                    this.getPrefixInfoFromDBToolStripMenuItem.Enabled = false;
                }
            }
            else
            {
                this.SearchprefixtoolStripMenuItem1.Enabled = false;
                this.listAllDNSReverseZonesToolStripMenuItem.Enabled = false;
                this.sendtodatabasetoolStripMenuItem1.Enabled = false;
                this.getPrefixInfoFromDBToolStripMenuItem.Enabled = true;
                this.savetoolStripMenuItem1.Enabled = true;
            }
        }

        private void ListSubnetRange_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.graph.Dispose();
                IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
                this.Close();
            }

            if (e.Control && e.KeyCode == Keys.F || e.KeyCode == Keys.F3)
            {
                if (this.listBox1.Items.Count > 0)
                {
                    this.findprefixtoolStripMenuItem1_Click(null, null);
                }
                else
                    return;
            }
        }

        private void buttonGoto_Click(object sender, EventArgs e)
        {
            this.maxfontwidth = 0;
            this.listBox1.HorizontalExtent = 0;

            this.textBox2.Text = this.textBox2.Text.Trim();
            if (this.textBox2.Text == "")
            {
                this.textBox2.Text = "0";
                return;
            }

            gotovalue = BigInteger.Parse(this.textBox2.Text, NumberStyles.Number);

            if (gotovalue > (NumberOfSubnets - BigInteger.One))
            {
                UpdateCount();
                return;
            }

            String ss = "";
            int count = 0;
            List<string> liste = new List<string>(upto);

            subnets.slash = StartEnd.slash;
            subnets.subnetslash = StartEnd.subnetslash;
            subnets.Start = StartEnd.Start;

            subnets.Start += gotovalue;

            if (subnets.Start.Equals(StartEnd.Start))
                this.Backwd.Enabled = false;
            else
                this.Backwd.Enabled = true;

            page.Start = subnets.Start;
            page.End = BigInteger.Zero;

            this.listBox1.Items.Clear();

            if (this.currentMode == "v6")
            {
                for (count = 0; count < upto; count++)
                {
                    subnets = v6ST.RangeIndex(subnets, this.is128Checked);

                    if (this.is128Checked == CheckState.Checked)
                    {
                        ss = String.Format("{0:x}", subnets.Start);
                        if (ss.Length > 32)
                            ss = ss.Substring(1, 32);
                        ss = v6ST.Kolonlar(ss, this.is128Checked);
                        ss = v6ST.CompressAddress(ss);

                        if (this.SelectedRange)
                            ss = "p" + subnets.subnetidx + "> " + ss + "/" + subnets.subnetslash;
                        else
                            ss = "p" + subnets.subnetidx + "> " + ss + "/128";
                    }
                    else if (this.is128Checked == CheckState.Unchecked)
                    {
                        ss = String.Format("{0:x}", subnets.Start);
                        if (ss.Length > 16)
                            ss = ss.Substring(1, 16);
                        ss = v6ST.Kolonlar(ss, this.is128Checked);
                        ss = v6ST.CompressAddress(ss);

                        if (this.SelectedRange)
                            ss = "p" + subnets.subnetidx + "> " + ss + "/" + subnets.subnetslash;
                        else
                            ss = "p" + subnets.subnetidx + "> " + ss + "/64";
                    }
                    liste.Add(ss);

                    if (subnets.Start.Equals(StartEnd.End))
                    {
                        break;
                    }
                    else
                    {
                        subnets.Start += BigInteger.One;
                    }
                }
                page.End = subnets.Start - BigInteger.One;
                this.listBox1.Items.AddRange(liste.ToArray());

                if (count > (upto - 1))
                {
                    this.Forwd.Enabled = true;
                    this.LastPage.Enabled = true;
                }
                else
                {
                    this.Forwd.Enabled = false;
                    this.LastPage.Enabled = false;
                }
            }
            else // v4
            {
                for (count = 0; count < upto; count++)
                {
                    subnets = v6ST.RangeIndex_v4(subnets);

                    ss = String.Format("{0:x}", subnets.Start);
                    ss = v6ST.IPv4Format(ss);

                    if (this.SelectedRange)
                        ss = "p" + subnets.subnetidx + "> " + ss + "/" + subnets.subnetslash;
                    else
                        ss = "p" + subnets.subnetidx + "> " + ss + "/32";

                    liste.Add(ss);

                    if (subnets.Start.Equals(StartEnd.End))
                    {
                        break;
                    }
                    else
                    {
                        subnets.Start += BigInteger.One;
                    }
                }
                page.End = subnets.Start - BigInteger.One;
                this.listBox1.Items.AddRange(liste.ToArray());

                if (count > (upto - 1))
                {
                    this.Forwd.Enabled = true;
                    this.LastPage.Enabled = true;
                }
                else
                {
                    this.Forwd.Enabled = false;
                    this.LastPage.Enabled = false;
                }
            }

            UpdateCount();
        }

        private void savetoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveAsText saveastxt = new SaveAsText(StartEnd, is128Checked, this.culture, this.SelectedRange);
            saveastxt.Show();

            IPv6SubnettingTool.Form1.windowsList.Add(new WindowsList(saveastxt, saveastxt.Name, saveastxt.GetHashCode(), this.currentMode));

            this.ChangeUILanguage += saveastxt.SwitchLanguage;
        }

        private void sendtodatabasetoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex == -1)
                return;

            string selected = this.listBox1.SelectedItem.ToString().Split(' ')[1];
            string snet = selected.Split('/')[0];
            short plen = Convert.ToInt16(selected.Split('/')[1]);

            if (this.currentMode == "v6")
            {
                if (this.ServerInfo.DBname == "")
                {
                    MessageBox.Show("Database name is null.\r\nPlease close the window,\r\nselect your database and reopen from the Main Form",
                        "Database Null", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
            }
            else // v4
            {
                if (this.ServerInfo.DBname_v4 == "")
                {
                    MessageBox.Show("Database name is null.\r\nPlease close the window,\r\nselect your database and reopen from the Main Form",
                        "Database Null", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
            }

            this.SendToDatabase(snet, plen, this.parentpflen);
        }

        private void SendToDatabase(string snet, short plen, short parentpflen)
        {
            DatabaseUI db = new DatabaseUI(snet, plen, parentpflen, this.MySQLconnection, 
                this.ServerInfo, this.culture, this.currentMode, this.listBox1.Font);

            if (!db.IsDisposed)
            {
                db.Show();
                //
                IPv6SubnettingTool.Form1.windowsList.Add(new WindowsList(db, db.Name, db.GetHashCode(), this.currentMode));

                this.changeDBstate += db.DBStateChange;
                this.ChangeUILanguage += db.SwitchLanguage;
            }
        }

        public void DBStateChange(OdbcConnection dbconn, int info)
        {
            this.MySQLconnection = dbconn;

            if (info == -1)
            {
                IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());

                if (this is IDisposable)
                    this.Dispose();
                else
                    this.Close();

                this.changeDBstate.Invoke(this.MySQLconnection, info);
            }
            else if (info == 1)
            {
                if (this.MySQLconnection == null)
                {
                    toolStripStatusLabel2.Text = "db=Down";
                    this.toolStripStatusLabel1.Text = "Database: null";
                }
                else
                {
                    if (this.MySQLconnection.State == System.Data.ConnectionState.Open)
                    {
                        toolStripStatusLabel2.Text = "db=Up";
                        this.toolStripStatusLabel1.Text = "Database: " + this.MySQLconnection.Database;
                    }
                    else if (this.MySQLconnection.State == System.Data.ConnectionState.Closed)
                    {
                        toolStripStatusLabel2.Text = "db=Down";
                        this.toolStripStatusLabel1.Text = "Database: null";
                    }
                }
            }
        }

        public void SwitchLanguage(CultureInfo culture)
        {
            this.culture = culture;
            this.Text = StringsDictionary.KeyValue("ListSubnetRange_Form.Text", this.culture);
            //
            this.buttonGoto.Text = StringsDictionary.KeyValue("ListSubnetRange_buttonGoto.Text", this.culture);
            this.copyToolStripMenuItem.Text = StringsDictionary.KeyValue("ListSubnetRange_copyToolStripMenuItem.Text", this.culture);
            this.FirstPage.Text = StringsDictionary.KeyValue("ListSubnetRange_FirstPage.Text", this.culture);
            this.label1.Text = StringsDictionary.KeyValue("ListSubnetRange_label1.Text", this.culture);
            this.label3.Text = StringsDictionary.KeyValue("ListSubnetRange_label3.Text", this.culture);
            this.label4.Text = StringsDictionary.KeyValue("ListSubnetRange_label4.Text", this.culture);
            this.label7.Text = StringsDictionary.KeyValue("ListSubnetRange_label7.Text", this.culture);
            this.label8.Text = StringsDictionary.KeyValue("ListSubnetRange_label8.Text", this.culture);
            this.listAllDNSReverseZonesToolStripMenuItem.Text = StringsDictionary.KeyValue("ListSubnetRange_listAllDNSReverseZonesToolStripMenuItem.Text", this.culture);
            this.savetoolStripMenuItem1.Text = StringsDictionary.KeyValue("ListSubnetRange_savetoolStripMenuItem1.Text", this.culture);
            this.selectAllToolStripMenuItem.Text = StringsDictionary.KeyValue("ListSubnetRange_selectAllToolStripMenuItem.Text", this.culture);
            this.sendtodatabasetoolStripMenuItem1.Text = StringsDictionary.KeyValue("ListSubnetRange_sendtodatabasetoolStripMenuItem1.Text", this.culture);
            this.getPrefixInfoFromDBToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_getPrefixInfoFromDB.Text", this.culture);
            this.SearchprefixtoolStripMenuItem1.Text = StringsDictionary.KeyValue("Form1_findprefixtoolStripMenuItem1.Text", this.culture);
            //
            this.toolTip1.SetToolTip(this.FirstPage, StringsDictionary.KeyValue("ListSubnetRange_FirstPage.ToolTip", this.culture));
            this.toolTip1.SetToolTip(this.LastPage, StringsDictionary.KeyValue("ListSubnetRange_LastPage.ToolTip", this.culture));
            this.UpdateCount();
            this.ChangeUILanguage.Invoke(this.culture);
        }

        private void ListSubnetRange_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.graph.Dispose();
            this.graph = null;
            IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
        }

        private void ListSubnetRange_Paint(object sender, PaintEventArgs e)
        {
            if (graph != null)
            {
                graph.Clear(ListSubnetRange.DefaultBackColor);
                graph.DrawRectangle(new Pen(Color.Red), 210, 76, 128, 11);

                if (this.listBox1.Items.Count > 0)
                {
                    int count = 128;

                    if (this.pix > 0)
                    {
                        if (this.NumberOfSubnets - this.currentidx <= 128)
                            graph.FillRectangle(new SolidBrush(Color.Red), 210, 76, count, 11);
                        else
                        {
                            graph.FillRectangle(new SolidBrush(Color.Red), 210, 76,
                                (float)((this.currentidx + 128) / this.pix), 11);
                        }
                    }
                }
            }
        }

        private void findprefixtoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Goto findpfx = new Goto(this, 3, 0, ID, this.culture, this.currentMode);
            findpfx.ShowDialog();
            this.ChangeUILanguage += findpfx.SwitchLanguage;

            if (this.findpfx == "")
            {
                if (findpfx is IDisposable)
                    findpfx.Dispose();
                return;
            }

            SEaddress seaddr = new SEaddress();

            if (this.currentMode == "v6")
            {
                seaddr.slash = this.StartEnd.slash;
                if (this.is128Checked == CheckState.Unchecked)
                    seaddr.subnetslash = 64;
                else if (this.is128Checked == CheckState.Checked)
                    seaddr.subnetslash = 128;

                String ss = "", se = "";
                int count = 0;

                string Resv6 = v6ST.FormalizeAddr(this.findpfx);

                if (this.is128Checked == CheckState.Checked) /* 128 bits */
                {
                    if (Resv6.Length == 32)
                        Resv6 = "0" + Resv6;
                }
                else if (this.is128Checked == CheckState.Unchecked) /* 64 bits */
                {
                    Resv6 = Resv6.Substring(0, 16); /* From left First 64bits */
                    if (Resv6.Length == 16)
                        Resv6 = "0" + Resv6;
                }

                seaddr.ResultIPAddr = seaddr.Start = BigInteger.Parse(Resv6, NumberStyles.AllowHexSpecifier);

                if (seaddr.ResultIPAddr >= StartEnd.Start && seaddr.ResultIPAddr <= StartEnd.End)
                {
                    // inside
                    BigInteger before = seaddr.ResultIPAddr;

                    seaddr = v6ST.FindPrefixIndex(seaddr, this.is128Checked);

                    subnets.subnetidx = seaddr.subnetidx;
                    subnets.slash = this.StartEnd.slash;

                    if (this.is128Checked == CheckState.Unchecked)
                        subnets.subnetslash = 64;
                    else if (this.is128Checked == CheckState.Checked)
                        subnets.subnetslash = 128;

                    subnets.Start = StartEnd.Start;
                    subnets.ResultIPAddr = StartEnd.ResultIPAddr;

                    subnets = v6ST.GoToSubnet(subnets, this.is128Checked);

                    if (before == subnets.Start)
                    {
                        page.Start = subnets.Start;
                        page.End = BigInteger.Zero;

                        if (subnets.End.Equals(StartEnd.End))
                        {
                            this.Forwd.Enabled = false;
                        }

                        this.listBox1.Items.Clear();

                        for (count = 0; count < upto; count++)
                        {
                            subnets = v6ST.Subnetting(subnets, this.is128Checked);

                            if (this.is128Checked == CheckState.Checked)
                            {
                                ss = String.Format("{0:x}", subnets.Start);
                                if (ss.Length > 32)
                                    ss = ss.Substring(1, 32);
                                ss = v6ST.Kolonlar(ss, this.is128Checked);
                                ss = v6ST.CompressAddress(ss);

                                se = String.Format("{0:x}", subnets.End);
                                if (se.Length > 32)
                                    se = se.Substring(1, 32);
                                se = v6ST.Kolonlar(se, this.is128Checked);

                                ss = "p" + subnets.subnetidx + "> " + ss + "/" + "128";

                                this.listBox1.Items.Add(ss);
                            }
                            else if (this.is128Checked == CheckState.Unchecked)
                            {
                                ss = String.Format("{0:x}", subnets.Start);
                                if (ss.Length > 16)
                                    ss = ss.Substring(1, 16);
                                ss = v6ST.Kolonlar(ss, this.is128Checked);
                                ss = v6ST.CompressAddress(ss);

                                se = String.Format("{0:x}", subnets.End);
                                if (se.Length > 16)
                                    se = se.Substring(1, 16);
                                se = v6ST.Kolonlar(se, this.is128Checked);

                                ss = "p" + subnets.subnetidx + "> " + ss + "/" + "64";

                                this.listBox1.Items.Add(ss);
                            }

                            if (subnets.End.Equals(StartEnd.End))
                            {
                                this.Forwd.Enabled = false;
                                break;
                            }
                            else
                            {
                                subnets.Start = subnets.End + BigInteger.One;
                            }
                        }
                        page.End = subnets.End;

                        if (seaddr.subnetidx == 0)
                        {
                            this.Backwd.Enabled = false;
                        }
                        else
                            this.Backwd.Enabled = true;
                        if (subnets.subnetidx == this.NumberOfSubnets)
                        {
                            this.Forwd.Enabled = false;
                            this.LastPage.Enabled = false;
                        }
                        else
                        {
                            this.Forwd.Enabled = true;
                            this.LastPage.Enabled = true;
                        }
                        UpdateCount();
                    }
                    else
                    {
                        MessageBox.Show(StringsDictionary.KeyValue("Form1_MsgBoxprefixnotfound", this.culture),
                            "Not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // outside
                    MessageBox.Show("Out of [Start-End] interval", "Out of range",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else // v4
            {
                seaddr.slash = this.StartEnd.slash;

                seaddr.subnetslash = 32;

                String ss = "";
                int count = 0;

                string Resv6 = v6ST.FormalizeAddr_v4(this.findpfx);

                seaddr.ResultIPAddr = seaddr.Start = BigInteger.Parse("0" + Resv6, NumberStyles.AllowHexSpecifier);

                if (seaddr.ResultIPAddr >= StartEnd.Start && seaddr.ResultIPAddr <= StartEnd.End)
                {
                    // inside
                    BigInteger before = seaddr.ResultIPAddr;

                    seaddr = v6ST.FindPrefixIndex_v4(seaddr);

                    subnets.subnetidx = seaddr.subnetidx;
                    subnets.slash = this.StartEnd.slash;

                    subnets.subnetslash = 32;

                    subnets.Start = StartEnd.Start;
                    subnets.ResultIPAddr = StartEnd.ResultIPAddr;

                    subnets = v6ST.GoToSubnet_v4(subnets);

                    if (before == subnets.Start)
                    {
                        page.Start = subnets.Start;
                        page.End = BigInteger.Zero;

                        if (subnets.End.Equals(StartEnd.End))
                        {
                            this.Forwd.Enabled = false;
                        }

                        this.listBox1.Items.Clear();

                        for (count = 0; count < upto; count++)
                        {
                            subnets = v6ST.Subnetting_v4(subnets);

                            ss = String.Format("{0:x}", subnets.Start);

                            ss = v6ST.IPv4Format(ss);
                            ss = "p" + subnets.subnetidx + "> " + ss + "/" + "32";

                            this.listBox1.Items.Add(ss);

                            if (subnets.End.Equals(StartEnd.End))
                            {
                                this.Forwd.Enabled = false;
                                break;
                            }
                            else
                            {
                                subnets.Start = subnets.End + BigInteger.One;
                            }
                        }
                        page.End = subnets.End;

                        if (seaddr.subnetidx == 0)
                        {
                            this.Backwd.Enabled = false;
                        }
                        else
                            this.Backwd.Enabled = true;

                        if (subnets.subnetidx == this.NumberOfSubnets)
                        {
                            this.Forwd.Enabled = false;
                            this.LastPage.Enabled = false;
                        }
                        else
                        {
                            this.Forwd.Enabled = true;
                            this.LastPage.Enabled = true;
                        }
                        UpdateCount();
                    }
                    else
                    {
                        MessageBox.Show(StringsDictionary.KeyValue("Form1_MsgBoxprefixnotfound", this.culture),
                            "Not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // outside
                    MessageBox.Show("Out of [Start-End] interval", "Out of range",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void getPrefixInfoFromDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItem != null && this.listBox1.SelectedItem.ToString() != ""
                && this.listBox1.SelectedIndex != -1 && this.MySQLconnection != null)
            {
                string pfx = this.listBox1.SelectedItem.ToString().Split(' ')[1];

                if (this.currentMode == "v6")
                {
                    if (this.ServerInfo.DBname == "")
                    {
                        MessageBox.Show("Database name is null.\r\nPlease close the window,\r\nselect your database and reopen from the Main Form",
                            "Database Null", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }
                else // v4
                {
                    if (this.ServerInfo.DBname_v4 == "")
                    {
                        MessageBox.Show("Database name is null.\r\nPlease close the window,\r\nselect your database and reopen from the Main Form",
                            "Database Null", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                }

                GetPrefixInfoFromDB getpfxinfo = new GetPrefixInfoFromDB(pfx, this.MySQLconnection, this.ServerInfo, this.culture, this.currentMode);

                if (!getpfxinfo.IsDisposed)
                    getpfxinfo.ShowDialog();
            }
        }
    }
}
