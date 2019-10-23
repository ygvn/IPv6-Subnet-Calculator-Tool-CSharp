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
        v6ST v6st = new v6ST();
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
        public delegate void ChangeWinFormStringsDelegate(CultureInfo culture);
        public event ChangeWinFormStringsDelegate ChangeUILanguage = delegate { };
        //DB
        OdbcConnection MySQLconnection;
        public DBServerInfo ServerInfo = new DBServerInfo();
        //
        public delegate void ChangeDBState(OdbcConnection dbconn, DBServerInfo servinfo);
        public event ChangeDBState changeDBstate = delegate { };
        #endregion

        public string findprefix
        {
            get { return this.findpfx; }
            set { this.findpfx = value; }
        }

        public ListSubnetRange(SEaddress input, string sin, int slash, int subnetslash, CheckState is128Checked, 
            CultureInfo culture, OdbcConnection sqlcon, DBServerInfo servinfo)
        {
            InitializeComponent();
            this.graph = this.CreateGraphics();
            this.graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

            this.MySQLconnection = sqlcon;
            this.ServerInfo = servinfo;
            this.parentpflen = (short)input.slash;
            this.StartEnd.ID = ID;
            this.incomingID = input.ID;
            this.culture = culture;
            
            this.SwitchLanguage(this.culture);

            this.is128Checked = is128Checked;
            this.Forwd.Enabled = false;
            this.Backwd.Enabled = false;
            this.LastPage.Enabled = false;

            string[] sa = sin.Split(' ');
            sa = sa[1].Split('/');

            string s = this.v6st.FormalizeAddr(sa[0]);
            s = this.v6st.Kolonlar(s, this.is128Checked);
            sa = s.Split(':');
            s = "";

            if (this.is128Checked == CheckState.Checked)
            {
                s = "0" + sa[0] + sa[1] + sa[2] + sa[3] + sa[4] + sa[5] + sa[6] + sa[7];
                StartEnd.Resultv6 = BigInteger.Parse(s, NumberStyles.AllowHexSpecifier);
                if (s.Length > 32)
                    s = s.Substring(1, 32);

                this.label3.Text = StringsDictionary.KeyValue("ListSubnetRange_label3.Text", this.culture);
            }
            else if (this.is128Checked == CheckState.Unchecked)
            {
                s = "0" + sa[0] + sa[1] + sa[2] + sa[3];
                StartEnd.Resultv6 = BigInteger.Parse(s, NumberStyles.AllowHexSpecifier);
                if (s.Length > 16)
                    s = s.Substring(1, 16);

                this.label3.Text = StringsDictionary.KeyValue("ListSubnetRange_label3.Text", this.culture);
            }
           
            StartEnd.slash = subnetslash;
            StartEnd.subnetslash = subnetslash;
            
            StartEnd = v6st.StartEndAddresses(StartEnd, this.is128Checked);
            NumberOfSubnets = StartEnd.End - StartEnd.Start + BigInteger.One;
            
            String s1 = "", s2 = "";

            if (this.is128Checked == CheckState.Unchecked)
            {
                this.DefaultView();

                s1 = String.Format("{0:x}", StartEnd.Start);
                if (s1.Length > 16)
                    s1 = s1.Substring(1, 16);
                s1 = v6st.Kolonlar(s1, this.is128Checked);
                s1 = v6st.CompressAddress(s1);

                s2 = String.Format("{0:x}", StartEnd.End);
                if (s2.Length > 16)
                    s2 = s2.Substring(1, 16);
                s2 = v6st.Kolonlar(s2, this.is128Checked);
                s2 = v6st.CompressAddress(s2);
            }
            else if (this.is128Checked == CheckState.Checked)
            {
                this.ExpandView();

                s1 = String.Format("{0:x}", StartEnd.Start);
                if (s1.Length > 32)
                    s1 = s1.Substring(1, 32);
                s1 = v6st.Kolonlar(s1, this.is128Checked);
                s1 = v6st.CompressAddress(s1);

                s2 = String.Format("{0:x}", StartEnd.End);
                if (s2.Length > 32)
                    s2 = s2.Substring(1, 32);
                s2 = v6st.Kolonlar(s2, this.is128Checked);
                s2 = v6st.CompressAddress(s2);
            }

            this.textBox1.Text = String.Format("{0}", NumberOfSubnets);
            this.label5.Text = s1 + "/" + StartEnd.subnetslash;
            this.label6.Text = s2 + "/" + StartEnd.subnetslash;

            this.FirstPage_Click(null, null);
        }

        private void ExpandView()
        {
            this.Size = new Size(645, 455);
            this.groupBox1.Size = new Size(603, 44);
            this.listBox1.Size = new Size(603, 280);
            this.textBox1.Size = new Size(353, 20);
            this.textBox1.MaxLength = 45;
            this.textBox2.Size = new Size(353, 20);
            this.textBox2.MaxLength = 45;
            this.textBox3.Location = new Point(567, 61);
            this.buttonGoto.Location = new Point(509, 384);
            //
            graph.Clear(ListSubnetRange.DefaultBackColor);
        }

        private void DefaultView()
        {
            this.Size = new Size(428, 455);
            this.groupBox1.Size = new Size(386, 44);
            this.listBox1.Size = new Size(386, 280);
            this.textBox1.Size = new Size(136, 20);
            this.textBox1.MaxLength = 21;
            this.textBox2.Size = new Size(136, 20);
            this.textBox2.MaxLength = 20;
            this.textBox3.Location = new Point(350, 61);
            this.buttonGoto.Location = new Point(292, 384);
            //
            graph.Clear(ListSubnetRange.DefaultBackColor);
        }

        private void UpdateCount()
        {
            this.textBox3.Text = "[" + this.listBox1.Items.Count.ToString()
                + StringsDictionary.KeyValue("ListSubnetRange_UpdateCount_textBox3.Text", this.culture);

            /**/
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

            subnets = v6st.ListSubRangeFirstPage(subnets, is128Checked);
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
            
            UpdateCount();
        }

        private void Backwd_Click(object sender, EventArgs e)
        {
            this.maxfontwidth = 0;
            this.listBox1.HorizontalExtent = 0;

            subnets.Start = page.End = page.Start - BigInteger.One;
            subnets.subnetslash = StartEnd.subnetslash;
            subnets.upto = upto;
            
            subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
            subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;

            this.listBox1.Items.Clear();

            subnets = v6st.ListSubRangePageBackward(subnets, is128Checked);
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
            
            UpdateCount();
        }

        private void Forwd_Click(object sender, EventArgs e)
        {
            this.maxfontwidth = 0;
            this.listBox1.HorizontalExtent = 0;

            subnets.Start = page.Start = page.End + BigInteger.One;
            subnets.subnetslash = StartEnd.subnetslash;
            subnets.upto = upto;
            subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;
            subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;

            this.listBox1.Items.Clear();

            subnets = v6st.ListSubRangePageForward(subnets, is128Checked);

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

            UpdateCount();
        }

        private void Last_Click(object sender, EventArgs e)
        {
            this.maxfontwidth = 0;
            this.listBox1.HorizontalExtent = 0;

            subnets.Start = page.End = StartEnd.End;
            subnets.subnetslash = StartEnd.subnetslash;
            subnets.upto = upto;
            subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
            subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;

            this.listBox1.Items.Clear();
            subnets = v6st.ListSubRangeLastPage(subnets, is128Checked);
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
            int tmp = StartEnd.subnetslash;

            if (this.is128Checked == CheckState.Unchecked)
            {
                StartEnd.subnetslash = 64;
            }
            else if (this.is128Checked == CheckState.Checked)
            {
                StartEnd.subnetslash = 128;
            }

            ListDnsReverses dnsr = new ListDnsReverses(StartEnd, this.is128Checked, this.culture);
            StartEnd.subnetslash = tmp;
            dnsr.Show();
            ////
            IPv6SubnettingTool.Form1.windowsList.Add(new WindowsList(dnsr, dnsr.Name, dnsr.GetHashCode()));
            //AddMenuItem(dnsr.Name, dnsr.GetHashCode());

            this.ChangeUILanguage += dnsr.SwitchLanguage;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (this.listBox1.Items.Count > 0)
            {
                this.contextMenuStrip1.Items[2].Enabled = true; // findpfx
                this.contextMenuStrip1.Items[3].Enabled = true; // revdns
                this.contextMenuStrip1.Items[7].Enabled = true; // saveas
                this.contextMenuStrip1.Items[8].Enabled = true; // fonts

                if (this.listBox1.SelectedItem != null && this.listBox1.SelectedItem.ToString() != ""
                    && this.listBox1.SelectedIndex != -1 && this.MySQLconnection != null)
                {
                    this.contextMenuStrip1.Items[4].Enabled = true; // dbase
                    this.contextMenuStrip1.Items[5].Enabled = true; // getinfofromdb
                }
                else
                {
                    this.contextMenuStrip1.Items[4].Enabled = false; // dbase
                    this.contextMenuStrip1.Items[5].Enabled = false; // getinfofromdb
                }
            }
            else
            {
                this.contextMenuStrip1.Items[2].Enabled = false;
                this.contextMenuStrip1.Items[3].Enabled = false;
                this.contextMenuStrip1.Items[4].Enabled = false;
                this.contextMenuStrip1.Items[5].Enabled = false;
                this.contextMenuStrip1.Items[7].Enabled = false;
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

            if (e.Control && e.KeyCode == Keys.F)
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

            for (count = 0; count < upto; count++)
            {
                subnets = v6st.RangeIndex(subnets, this.is128Checked);

                if (this.is128Checked == CheckState.Checked)
                {
                    ss = String.Format("{0:x}", subnets.Start);
                    if (ss.Length > 32)
                        ss = ss.Substring(1, 32);
                    ss = v6st.Kolonlar(ss, this.is128Checked);
                    ss = v6st.CompressAddress(ss);
                    ss = "p" + subnets.subnetidx + "> " + ss + "/128";
                }
                else if (this.is128Checked == CheckState.Unchecked)
                {
                    ss = String.Format("{0:x}", subnets.Start);
                    if (ss.Length > 16)
                        ss = ss.Substring(1, 16);
                    ss = v6st.Kolonlar(ss, this.is128Checked);
                    ss = v6st.CompressAddress(ss);
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

            UpdateCount();
        }

        private void fontsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (this.fontDialog1)
            {
                DialogResult resfont = this.fontDialog1.ShowDialog();
                if (resfont == System.Windows.Forms.DialogResult.OK)
                {
                    this.listBox1.Font = fontDialog1.Font;
                    this.listBox1.ItemHeight = this.listBox1.Font.Height;
                    this.maxfontwidth = 0;
                    this.listBox1.HorizontalExtent = 0;
                }
            }
        }

        private void savetoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveAsText saveastxt = new SaveAsText(StartEnd, is128Checked, this.culture);
            saveastxt.Show();
            ////
            IPv6SubnettingTool.Form1.windowsList.Add(new WindowsList(saveastxt, saveastxt.Name, saveastxt.GetHashCode()));
            //AddMenuItem(saveastxt.Name, saveastxt.GetHashCode());


            this.ChangeUILanguage += saveastxt.SwitchLanguage;
        }

        private void sendtodatabasetoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex == -1)
                return;

            string selected = this.listBox1.SelectedItem.ToString().Split(' ')[1];
            string snet = selected.Split('/')[0];
            short plen = Convert.ToInt16(selected.Split('/')[1]);

            if (this.MySQLconnection == null)
            {
                MessageBox.Show(StringsDictionary.KeyValue("ListSubnetRange_sendtodatabasetoolStripMenuItem1_Click", this.culture),
                    StringsDictionary.KeyValue("ListSubnetRange_sendtodatabasetoolStripMenuItem1_Click_header", this.culture),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
           /* else if (this.MySQLconnection != null && this.MySQLconnection.State == ConnectionState.Closed)
            {
                MessageBox.Show("We've database connection\r\nBut its state is closed (maybe closed from the main form)\r\nWe'll try to connect now",
                    "DB connection state=closed", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.SendToDatabase(snet, plen);
            }*/
            else
            {
                this.SendToDatabase(snet, plen, this.parentpflen);
            }
        }

        private void SendToDatabase(string snet, short plen, short parentpflen)
        {
            DatabaseUI db = new DatabaseUI(snet, plen, parentpflen, this.MySQLconnection, this.ServerInfo, this.culture);
            db.Show();
            this.changeDBstate += db.DBStateChange;
            this.ChangeUILanguage += db.SwitchLanguage;
        }

        public void DBStateChange(OdbcConnection dbconn, DBServerInfo servinfo)
        {
            this.MySQLconnection = dbconn;
            this.ServerInfo = servinfo;
            this.changeDBstate.Invoke(this.MySQLconnection, this.ServerInfo);
        }

        public void SwitchLanguage(CultureInfo culture)
        {
            this.culture = culture;
            this.Text = StringsDictionary.KeyValue("ListSubnetRange_Form.Text", this.culture);
            //
            this.buttonGoto.Text = StringsDictionary.KeyValue("ListSubnetRange_buttonGoto.Text", this.culture);
            this.copyToolStripMenuItem.Text = StringsDictionary.KeyValue("ListSubnetRange_copyToolStripMenuItem.Text", this.culture);
            this.FirstPage.Text = StringsDictionary.KeyValue("ListSubnetRange_FirstPage.Text", this.culture);
            this.fontsToolStripMenuItem.Text = StringsDictionary.KeyValue("ListSubnetRange_fontsToolStripMenuItem.Text", this.culture);
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
            this.findprefixtoolStripMenuItem1.Text = StringsDictionary.KeyValue("Form1_findprefixtoolStripMenuItem1.Text", this.culture);
            //
            this.toolTip1.SetToolTip(this.FirstPage, StringsDictionary.KeyValue("ListSubnetRange_FirstPage.ToolTip", this.culture));
            this.toolTip1.SetToolTip(this.LastPage, StringsDictionary.KeyValue("ListSubnetRange_LastPage.ToolTip", this.culture));
            this.UpdateCount();
            this.ChangeUILanguage.Invoke(this.culture);
        }

        private void ListSubnetRange_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.graph.Dispose();
            IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
            this.graph = null;
        }

        private void ListSubnetRange_Paint(object sender, PaintEventArgs e)
        {
            if (graph != null)
            {
                graph.Clear(ListSubnetRange.DefaultBackColor);
                graph.DrawRectangle(new Pen(Color.Red), 210, 60, 128, 11);

                if (this.listBox1.Items.Count > 0)
                {
                    int count = 128;

                    if (this.pix > 0)
                    {
                        if (this.NumberOfSubnets - this.currentidx <= 128)
                            graph.FillRectangle(new SolidBrush(Color.Red), 210, 60, count, 11);
                        else
                        {
                            graph.FillRectangle(new SolidBrush(Color.Red), 210, 60,
                                (float)((this.currentidx + 128) / this.pix), 11);
                        }
                    }
                }
            }
        }

        private void findprefixtoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Goto findpfx = new Goto(this, 3, 0, ID, this.culture);
            findpfx.ShowDialog();
            this.ChangeUILanguage += findpfx.SwitchLanguage;

            if (this.findpfx == "")
            {
                if (findpfx is IDisposable)
                    findpfx.Dispose();
                return;
            }

            SEaddress seaddr = new SEaddress();
            seaddr.slash = this.StartEnd.slash;
            if (this.is128Checked == CheckState.Unchecked)
                seaddr.subnetslash = 64;
            else if (this.is128Checked == CheckState.Checked)
                seaddr.subnetslash = 128;
            
            String ss = "", se = "";
            int count = 0;

            string Resv6 = v6st.FormalizeAddr(this.findpfx);
            
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

            seaddr.Resultv6 = seaddr.Start = BigInteger.Parse(Resv6, NumberStyles.AllowHexSpecifier);

            if (seaddr.Resultv6 >= StartEnd.Start && seaddr.Resultv6 <= StartEnd.End)
            {
                // inside
                BigInteger before = seaddr.Resultv6;

                seaddr = v6st.FindPrefixIndex(seaddr, this.is128Checked);

                subnets.subnetidx = seaddr.subnetidx;
                subnets.slash = this.StartEnd.slash;

                if (this.is128Checked == CheckState.Unchecked)
                    subnets.subnetslash = 64;
                else if (this.is128Checked == CheckState.Checked)
                    subnets.subnetslash = 128;
                
                subnets.Start = StartEnd.Start;
                subnets.Resultv6 = StartEnd.Resultv6;

                subnets = v6st.GoToSubnet(subnets, this.is128Checked);

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
                        subnets = v6st.Subnetting(subnets, this.is128Checked);

                        if (this.is128Checked == CheckState.Checked)
                        {
                            ss = String.Format("{0:x}", subnets.Start);
                            if (ss.Length > 32)
                                ss = ss.Substring(1, 32);
                            ss = v6st.Kolonlar(ss, this.is128Checked);
                            ss = v6st.CompressAddress(ss);

                            se = String.Format("{0:x}", subnets.End);
                            if (se.Length > 32)
                                se = se.Substring(1, 32);
                            se = v6st.Kolonlar(se, this.is128Checked);

                            ss = "p" + subnets.subnetidx + "> " + ss + "/" + "128";

                            this.listBox1.Items.Add(ss);
                        }
                        else if (this.is128Checked == CheckState.Unchecked)
                        {
                            ss = String.Format("{0:x}", subnets.Start);
                            if (ss.Length > 16)
                                ss = ss.Substring(1, 16);
                            ss = v6st.Kolonlar(ss, this.is128Checked);
                            ss = v6st.CompressAddress(ss);

                            se = String.Format("{0:x}", subnets.End);
                            if (se.Length > 16)
                                se = se.Substring(1, 16);
                            se = v6st.Kolonlar(se, this.is128Checked);

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

        private void getPrefixInfoFromDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedItem != null && this.listBox1.SelectedItem.ToString() != ""
                && this.listBox1.SelectedIndex != -1 && this.MySQLconnection != null)
            {
                string pfx = this.listBox1.SelectedItem.ToString().Split(' ')[1];
                GetPrefixInfoFromDB getpfxinfo = new GetPrefixInfoFromDB(pfx, this.MySQLconnection, this.ServerInfo, this.culture);
                getpfxinfo.ShowDialog();

            }
        }
    }
}
