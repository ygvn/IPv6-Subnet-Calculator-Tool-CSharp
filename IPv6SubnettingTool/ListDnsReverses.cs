/*
 * Copyright (c) 2010-2020 Yucel Guven
 * All rights reserved.
 * 
 * This file is part of IPv6 Subnetting Tool.
 * 
 * Version: 4.1
 * Published Date: 6 January 2020
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

namespace IPv6SubnettingTool
{
    public partial class ListDnsReverses : Form
    {
        #region special initials/constants -yucel
        public const int ID = 2; // ID of this Form.
        public int incomingID;
        SEaddress StartEnd = new SEaddress();
        SEaddress subnets = new SEaddress();
        SEaddress page = new SEaddress();
        public int upto = 128;
        BigInteger NumberOfZones = BigInteger.Zero;
        public BigInteger zmaxval = BigInteger.Zero;
        public CheckState is128Checked;
        int maxfontwidth = 0;
        Graphics g = null;
        string currentMode = "";
        //
        public CultureInfo culture;
        public delegate void ChangeWinFormStringsDelegate(CultureInfo culture);
        public event ChangeWinFormStringsDelegate ChangeUILanguage = delegate { };
        #endregion

        public ListDnsReverses(SEaddress input, CheckState is128Checked, CultureInfo culture, string mode, Font font)
        {
            InitializeComponent();
            //
            this.listBox1.Font = font;
            this.textBox1.Font = font;
            this.textBox2.Font = font;
            this.label5.Font =   font;
            this.label6.Font =   font;

            this.currentMode = mode;
            this.StartEnd.ID = ID;
            this.incomingID = input.ID;
            this.is128Checked = is128Checked;
            this.culture = culture;

            this.SwitchLanguage(this.culture);

            if (this.currentMode == "v6")
            {
                if (input.subnetslash % 4 == 1 && is128Checked == CheckState.Checked)
                {
                    upto = 64;
                    this.toolTip1.SetToolTip(this.Backwd, "-64");
                    this.toolTip1.SetToolTip(this.Forwd, "+64");
                }

                StartEnd.Start = input.Start;
                StartEnd.End = input.End;
                StartEnd.ResultIPAddr = input.ResultIPAddr;
                StartEnd.LowerLimitAddress = input.LowerLimitAddress;
                StartEnd.UpperLimitAddress = input.UpperLimitAddress;
                StartEnd.upto = upto;
                StartEnd.slash = input.slash;
                StartEnd.subnetslash = input.subnetslash;
                StartEnd.subnetidx = input.subnetidx;

                subnets.Start = input.Start;
                subnets.End = input.End;
                subnets.slash = input.slash;
                subnets.subnetslash = input.subnetslash;
                subnets.LowerLimitAddress = input.LowerLimitAddress;
                subnets.UpperLimitAddress = input.UpperLimitAddress;


                BigInteger max = NumberOfZones =
                    (BigInteger.One << (input.subnetslash - input.slash));
                zmaxval = max - BigInteger.One;
                this.textBox1.Text = NumberOfZones.ToString();

                if (subnets.subnetslash % 4 != 0)
                {
                    this.textBox3.Text = StringsDictionary.KeyValue("ListDNSRev_textBox3.Text", this.culture);
                }

                if (this.is128Checked == CheckState.Unchecked)
                {
                    this.DefaultView();

                    string s = String.Format("{0:x}", StartEnd.Start);
                    if (s == "0")
                        s = "0000000000000000";
                    else if (s.Length > 16)
                        s = s.Substring(1, 16);

                    this.label5.Text = v6ST.CompressAddress(v6ST.Kolonlar(s, is128Checked))
                        + "/" + StartEnd.subnetslash.ToString();

                    s = String.Format("{0:x}", StartEnd.End);
                    if (s == "0")
                        s = "0000000000000000";
                    else if (s.Length > 16)
                        s = s.Substring(1, 16);

                    this.label6.Text = v6ST.CompressAddress(v6ST.Kolonlar(s, is128Checked))
                        + "/" + StartEnd.subnetslash.ToString();
                }
                else if (this.is128Checked == CheckState.Checked)
                {
                    this.ExpandView();

                    string s = String.Format("{0:x}", StartEnd.Start);
                    if (s == "0")
                        s = "00000000000000000000000000000000";
                    else if (s.Length > 32)
                        s = s.Substring(1, 32);
                    this.label5.Text = v6ST.CompressAddress(v6ST.Kolonlar(s, is128Checked))
                        + "/" + StartEnd.subnetslash.ToString();

                    s = String.Format("{0:x}", StartEnd.End);
                    if (s == "0")
                        s = "00000000000000000000000000000000";
                    else if (s.Length > 32)
                        s = s.Substring(1, 32);
                    this.label6.Text = v6ST.CompressAddress(v6ST.Kolonlar(s, is128Checked))
                        + "/" + StartEnd.subnetslash.ToString();
                }

                this.FirstPage_Click(null, null);
            }
            else //v4
            {
                if (input.subnetslash % 8 == 1)
                {
                    upto = 64;
                    this.toolTip1.SetToolTip(this.Backwd, "-64");
                    this.toolTip1.SetToolTip(this.Forwd, "+64");
                }

                StartEnd.Start = input.Start;
                StartEnd.End = input.End;
                StartEnd.ResultIPAddr = input.ResultIPAddr;
                StartEnd.LowerLimitAddress = input.LowerLimitAddress;
                StartEnd.UpperLimitAddress = input.UpperLimitAddress;
                StartEnd.upto = upto;
                StartEnd.slash = input.slash;
                StartEnd.subnetslash = input.subnetslash;
                StartEnd.subnetidx = input.subnetidx;

                subnets.Start = input.Start;
                subnets.End = input.End;
                subnets.slash = input.slash;
                subnets.subnetslash = input.subnetslash;
                subnets.LowerLimitAddress = input.LowerLimitAddress;
                subnets.UpperLimitAddress = input.UpperLimitAddress;


                BigInteger max = NumberOfZones =
                    (BigInteger.One << (input.subnetslash - input.slash));
                zmaxval = max - BigInteger.One;
                this.textBox1.Text = NumberOfZones.ToString();

                // non-nibble
                if (subnets.subnetslash % 8 != 0)
                {
                    this.textBox3.Text = StringsDictionary.KeyValue("ListDNSRev_textBox3.Text_v4", this.culture);
                }

                this.DefaultView();

                string s = String.Format("{0:x}", StartEnd.Start);
                
                s = v6ST.IPv4Format(s);
                this.label6.Text = s + "/" + StartEnd.subnetslash.ToString();

                this.FirstPage_Click(null, null);
            }
        }

        private void ExpandView()
        {
            this.Size = new Size(855, 455);
            this.groupBox1.Size = new Size(813, 44);
            this.listBox1.Size = new Size(813, 280);
            this.textBox1.Size = new Size(353, 20);
            this.textBox1.MaxLength = 45;
            this.textBox2.Size = new Size(353, 20);
            this.textBox2.MaxLength = 45;
            this.textBox3.Location = new Point(404, 14);
            this.textBox4.Location = new Point(727, 61);
            this.Goto.Location = new Point(509, 384);
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
            this.textBox3.Location = new Point(278, 14);
            this.textBox4.Location = new Point(301, 61);
            this.Goto.Location = new Point(292, 384);
        }

        private void UpdateCount()
        {
            if (this.currentMode == "v6")
            {
                if (StartEnd.subnetslash % 4 == 0)
                {
                    this.textBox4.Text = "[" + this.listBox1.Items.Count.ToString()
                        + StringsDictionary.KeyValue("ListDNSRev_UpdateCount_textBox4", this.culture);
                }
                else
                {
                    int nonnibble = (1 << (4 - StartEnd.subnetslash % 4));
                    this.textBox4.Text = "[" + (this.listBox1.Items.Count / nonnibble).ToString()
                        + StringsDictionary.KeyValue("ListDNSRev_UpdateCount_textBox4", this.culture);
                }
            }
            else // v4
            {
                if (StartEnd.subnetslash % 8 == 0)
                {
                    this.textBox4.Text = "[" + this.listBox1.Items.Count.ToString()
                        + StringsDictionary.KeyValue("ListDNSRev_UpdateCount_textBox4", this.culture);
                }
                else
                {
                    int nonnibble = (1 << (8 - StartEnd.subnetslash % 8));
                    this.textBox4.Text = "[" + (this.listBox1.Items.Count / nonnibble).ToString()
                        + StringsDictionary.KeyValue("ListDNSRev_UpdateCount_textBox4", this.culture);
                }
            }
        }

        private void FirstPage_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = NumberOfZones.ToString();
            this.maxfontwidth = 0;
            this.listBox1.HorizontalExtent = 0;

            subnets.Start = page.Start = StartEnd.Start;
            subnets.End = page.End = BigInteger.Zero;
            subnets.subnetidx = BigInteger.Zero;
            subnets.slash = StartEnd.slash;
            subnets.subnetslash = StartEnd.subnetslash;
            
            subnets.upto = upto;
            subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;
            subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;

            if (subnets.End.Equals(StartEnd.End))
            {
                UpdateCount(); // entries
                return;
            }

            this.listBox1.Items.Clear();

            if (this.currentMode == "v6")
            {
                if (subnets.subnetslash % 4 != 0)
                {
                    this.textBox3.Text = StringsDictionary.KeyValue("ListDNSRev_textBox3.Text", this.culture);
                }

                String[] sa;

                if ((this.is128Checked == CheckState.Unchecked && StartEnd.slash == 64)
                    || (this.is128Checked == CheckState.Checked && StartEnd.slash == 128))
                {
                    subnets.Start = StartEnd.ResultIPAddr;

                    sa = v6ST.DnsRev(subnets.Start, subnets.subnetslash, this.is128Checked);
                    //this.listBox1.Items.Add("s0> " + sa[0]);
                    this.listBox1.Items.Add("p0> " + sa[0]);
                    UpdateCount();
                    return;
                }

                subnets = v6ST.ListDnsRevFirstPage(subnets, this.is128Checked);
                this.listBox1.Items.AddRange(subnets.liste.ToArray());
                this.page.End = subnets.End;

                if (NumberOfZones <= upto)
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
                if (subnets.subnetslash % 8 != 0)
                {
                    this.textBox3.Text = StringsDictionary.KeyValue("ListDNSRev_textBox3.Text_v4", this.culture);
                }

                String[] sa;

                if (StartEnd.slash == 32)
                {
                    subnets.Start = StartEnd.ResultIPAddr;
                    sa = v6ST.DnsRev_v4(subnets.Start, subnets.subnetslash);
                    this.listBox1.Items.Add("p0> " + sa[0]);
                    UpdateCount();
                    return;
                }

                subnets = v6ST.ListDnsRevFirstPage_v4(subnets);
                this.listBox1.Items.AddRange(subnets.liste.ToArray());
                this.page.End = subnets.End;

                if (NumberOfZones <= upto)
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

            subnets.upto = upto;
            subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
            subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;
            
            this.listBox1.Items.Clear();
            subnets.End = page.End = page.Start - BigInteger.One;

            if (this.currentMode == "v6")
            {
                subnets = v6ST.ListDnsRevPageBackward(subnets, is128Checked);
            }
            else // v4
            {
                subnets = v6ST.ListDnsRevPageBackward_v4(subnets);
            }
            page.Start = subnets.Start;            

            this.listBox1.Items.AddRange(subnets.liste.ToArray());

            if (subnets.Start.Equals(StartEnd.Start))
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

            subnets.upto = upto;
            subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
            subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;

            subnets.Start = page.Start = page.End + BigInteger.One;

            this.listBox1.Items.Clear();

            if (this.currentMode == "v6")
            {
                subnets = v6ST.ListDnsRevPageForward(subnets, is128Checked);
            }
            else // v4
            {
                subnets = v6ST.ListDnsRevPageForward_v4(subnets);
            }

            page.End = subnets.End;
            
            this.listBox1.Items.AddRange(subnets.liste.ToArray());

            if (subnets.End.Equals(StartEnd.End))
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

        private void LastPage_Click(object sender, EventArgs e)
        {
            this.maxfontwidth = 0;
            this.listBox1.HorizontalExtent = 0;

            subnets.upto = upto;
            subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
            subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;

            this.listBox1.Items.Clear();

            subnets.subnetidx = BigInteger.Zero;
            subnets.End = page.End = StartEnd.End;

            if (this.currentMode == "v6")
            {
                subnets = v6ST.ListDnsRevLastPage(subnets, is128Checked);
            }
            else // v4
            {
                subnets = v6ST.ListDnsRevLastPage_v4(subnets);
            }

            page.Start = subnets.Start;
            this.listBox1.Items.AddRange(subnets.liste.ToArray());

            if (subnets.subnetidx == 0)
                this.Backwd.Enabled = false;
            else
                this.Backwd.Enabled = true;

            this.Forwd.Enabled = false;
            this.LastPage.Enabled = false;
            UpdateCount();
        }

        private void Goto_Click(object sender, EventArgs e)
        {
            this.maxfontwidth = 0;
            this.listBox1.HorizontalExtent = 0;

            String[] sa;
            List<string> liste = new List<string>(upto * 8);
            int count = 0;
            int spaces = 0;

            string newidx = this.textBox2.Text;
            if (newidx == "")
                return;

            subnets.subnetidx = BigInteger.Parse(newidx,NumberStyles.Number);
            subnets.slash = StartEnd.slash;
            subnets.subnetslash = StartEnd.subnetslash;
            subnets.Start = StartEnd.Start;
            subnets.ResultIPAddr = StartEnd.ResultIPAddr;

            if (this.currentMode == "v6")
            {
                subnets = v6ST.GoToSubnet(subnets, this.is128Checked);

                if (subnets.subnetslash % 4 != 0)
                {
                    this.textBox3.Text = StringsDictionary.KeyValue("ListDNSRev_textBox3.Text", this.culture);
                }

            }
            else // v4
            {
                subnets = v6ST.GoToSubnet_v4(subnets);

                if (subnets.subnetslash % 8 != 0)
                {
                    this.textBox3.Text = StringsDictionary.KeyValue("ListDNSRev_textBox3.Text_v4", this.culture);
                }

            }

            page.Start = subnets.Start;
            page.End = BigInteger.Zero;

            if (subnets.End.Equals(StartEnd.End))
            {
                this.Forwd.Enabled = false;
            }

            this.listBox1.Items.Clear();

            int remainder = subnets.subnetslash % 8;
            int nzones = (1 << (8 - remainder));

            for (count = 0; count < upto; count++)
            {
                if (this.currentMode == "v6")
                {
                    subnets = v6ST.Subnetting(subnets, this.is128Checked);
                    sa = v6ST.DnsRev(subnets.Start, subnets.subnetslash, this.is128Checked);
                }
                else // v4
                {
                    subnets = v6ST.Subnetting_v4(subnets);
                    sa = v6ST.DnsRev_v4(subnets.Start, subnets.subnetslash);
                }

                string sf = "p" + subnets.subnetidx + "> " + sa[0];
                liste.Add(sf);

                string[] sr = sf.Split(' ');
                spaces = sr[0].Length + 1;

                for (int i = 1; i < nzones; i++)
                {
                    if (sa[i] == null)
                        break;
                    sa[i] = sa[i].PadLeft(sa[i].Length + spaces, ' ');
                    liste.Add(sa[i]);
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
            this.listBox1.Items.AddRange(liste.ToArray());

            page.End = subnets.End;
            if (BigInteger.Parse(newidx) == 0)
            {
                this.Backwd.Enabled = false;
            }
            else
                this.Backwd.Enabled = true;
            if (subnets.subnetidx == this.zmaxval)
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

        private void textBox2_Enter(object sender, EventArgs e)
        {
            this.textBox2.BackColor = Color.White;
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

                BigInteger gotovalue = BigInteger.Parse(this.textBox2.Text);
                BigInteger maxvalue  = BigInteger.Parse(this.textBox1.Text);
                this.textBox2.Text = gotovalue.ToString();

                if (gotovalue > (maxvalue - 1))
                {
                    this.textBox2.Text = (maxvalue - 1).ToString();
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
                if ( BigInteger.Parse(this.textBox2.Text) > (NumberOfZones - 1))
                {
                    this.textBox2.Text = (NumberOfZones - 1).ToString();
                    this.textBox2.BackColor = Color.FromKnownColor(KnownColor.Info);
                }
            }
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

        private void LBoxDrawItems(object sender, DrawItemEventArgs e, int IdxRemainder)
        {
            ListBox lb = (ListBox)sender;
            g = e.Graphics;
            SolidBrush sback = new SolidBrush(e.BackColor);
            SolidBrush sfore = new SolidBrush(e.ForeColor);

            if (IdxRemainder == 0)
            {
                e.DrawBackground();
                DrawItemState st = DrawItemState.Selected;

                if ((e.State & st) != st)
                {
                    g.FillRectangle(new SolidBrush(Color.WhiteSmoke), e.Bounds);
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

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;

            ListBox lb = (ListBox)sender;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

            if (this.currentMode == "v6")
            {
                if (subnets.subnetslash % 4 == 0) //nibble
                {
                    if (e.Index % 2 == 0)
                    {
                        LBoxDrawItems(sender, e, 0);
                    }
                    else
                    {
                        LBoxDrawItems(sender, e, 1);
                    }
                }
                else //non-nible
                {
                    int sayi = (1 << (4 - (subnets.subnetslash % 4)));

                    if (sayi == 2)
                    {
                        if (e.Index % 2 == 0)
                        {
                            LBoxDrawItems(sender, e, 0);
                        }
                        else
                        {
                            LBoxDrawItems(sender, e, 1);
                        }
                    }
                    else if (sayi == 4)
                    {
                        if (e.Index % 4 == 0)
                        {
                            LBoxDrawItems(sender, e, 0);
                        }
                        else
                        {
                            LBoxDrawItems(sender, e, 1);
                        }
                    }
                    else if (sayi == 8)
                    {
                        if (e.Index % 8 == 0)
                        {
                            LBoxDrawItems(sender, e, 0);
                        }
                        else
                        {
                            LBoxDrawItems(sender, e, 1);
                        }
                    }
                }
            }
            else //v4
            {
                if (subnets.subnetslash % 8 == 0) // byte-boundary
                {
                    if (e.Index % 2 == 0)
                    {
                        LBoxDrawItems(sender, e, 0);
                    }
                    else
                    {
                        LBoxDrawItems(sender, e, 1);
                    }
                }
                else //not byte-boundary
                {
                    int sayi = (1 << (8 - (subnets.subnetslash % 8)));

                    if (sayi == 2)
                    {
                        if (e.Index % 2 == 0)
                        {
                            LBoxDrawItems(sender, e, 0);
                        }
                        else
                        {
                            LBoxDrawItems(sender, e, 1);
                        }
                    }
                    else if (sayi == 4)
                    {
                        if (e.Index % 4 == 0)
                        {
                            LBoxDrawItems(sender, e, 0);
                        }
                        else
                        {
                            LBoxDrawItems(sender, e, 1);
                        }
                    }
                    else if (sayi == 8)
                    {
                        if (e.Index % 8 == 0)
                        {
                            LBoxDrawItems(sender, e, 0);
                        }
                        else
                        {
                            LBoxDrawItems(sender, e, 1);
                        }
                    }
                    else if (sayi == 16)
                    {
                        if (e.Index % 16 == 0)
                        {
                            LBoxDrawItems(sender, e, 0);
                        }
                        else
                        {
                            LBoxDrawItems(sender, e, 1);
                        }
                    }
                    else if (sayi == 32)
                    {
                        if (e.Index % 32 == 0)
                        {
                            LBoxDrawItems(sender, e, 0);
                        }
                        else
                        {
                            LBoxDrawItems(sender, e, 1);
                        }
                    }
                    else if (sayi == 64)
                    {
                        if (e.Index % 64 == 0)
                        {
                            LBoxDrawItems(sender, e, 0);
                        }
                        else
                        {
                            LBoxDrawItems(sender, e, 1);
                        }
                    }
                    else if (sayi == 128)
                    {
                        if (e.Index % 128 == 0)
                        {
                            LBoxDrawItems(sender, e, 0);
                        }
                        else
                        {
                            LBoxDrawItems(sender, e, 1);
                        }
                    }
                }
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

        private void ListDnsReverses_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
                this.Close();
            }
        }

        private void savetoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveAsText saveastxt = new SaveAsText(StartEnd, this.is128Checked, this.culture);
            saveastxt.Show();

            IPv6SubnettingTool.Form1.windowsList.Add(new WindowsList(saveastxt, saveastxt.Name, saveastxt.GetHashCode(), null));

            this.ChangeUILanguage += saveastxt.SwitchLanguage;
        }

        public void SwitchLanguage(CultureInfo culture)
        {
            this.culture = culture;
            this.Text = StringsDictionary.KeyValue("ListDNSRev_Form.Text", this.culture);
            //
            this.copyToolStripMenuItem.Text = StringsDictionary.KeyValue("ListDNSRev_copyToolStripMenuItem.Text", this.culture);
            this.FirstPage.Text = StringsDictionary.KeyValue("ListDNSRev_FirstPage.Text", this.culture);
            this.Goto.Text = StringsDictionary.KeyValue("ListDNSRev_Goto.Text", this.culture);
            this.label1.Text = StringsDictionary.KeyValue("ListDNSRev_label1.Text", this.culture);
            this.label3.Text = StringsDictionary.KeyValue("ListDNSRev_label3.Text", this.culture);
            this.label4.Text = StringsDictionary.KeyValue("ListDNSRev_label4.Text", this.culture);
            this.label7.Text = StringsDictionary.KeyValue("ListDNSRev_label7.Text", this.culture);
            this.label8.Text = StringsDictionary.KeyValue("ListDNSRev_label8.Text", this.culture);
            this.savetoolStripMenuItem1.Text = StringsDictionary.KeyValue("ListDNSRev_savetoolStripMenuItem1.Text", this.culture);
            this.selectAllToolStripMenuItem.Text = StringsDictionary.KeyValue("ListDNSRev_selectAllToolStripMenuItem.Text", this.culture);
            if (this.textBox3.Text.Length > 3)
            {
                if (this.currentMode == "v6")
                    this.textBox3.Text = StringsDictionary.KeyValue("ListDNSRev_textBox3.Text", this.culture);
                else
                    this.textBox3.Text = StringsDictionary.KeyValue("ListDNSRev_textBox3.Text_v4", this.culture);
            }
            //
            this.toolTip1.SetToolTip(this.FirstPage, StringsDictionary.KeyValue("ListDNSRev_FirstPage.ToolTip", this.culture));
            this.toolTip1.SetToolTip(this.LastPage, StringsDictionary.KeyValue("ListDNSRev_LastPage.ToolTip", this.culture));
            this.toolTip1.SetToolTip(this.textBox3, StringsDictionary.KeyValue("ListDNSRev_textBox3.Tooltip", this.culture));
            //
            this.UpdateCount();
            this.ChangeUILanguage.Invoke(this.culture);
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (this.listBox1.Items.Count > 0)
            {
                this.contextMenuStrip1.Items[2].Enabled = true; // saveas
            }
            else
            {
                this.contextMenuStrip1.Items[2].Enabled = false; // saveas
            }
        }

        private void ListDnsReverses_FormClosing(object sender, FormClosingEventArgs e)
        {
            IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
        }
    }
}
