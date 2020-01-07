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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using System.Numerics;
using System.Threading;
using System.Data.Odbc;

namespace IPv6SubnettingTool
{
    public partial class Form1 : Form
    {
        #region special initials/constants -yucel
        public const int ID = 0; // ID of this Form. Form1 is the main Form
        public string findpfx = "";
        public string GotoForm_PrevValue = "";
        //
        SEaddress StartEnd = new SEaddress();
        SEaddress subnets = new SEaddress();
        SEaddress page = new SEaddress();
        const int upto = 128;
        Graphics graph;
        BigInteger currentidx = BigInteger.Zero;
        BigInteger pix = BigInteger.Zero;
        int delta = 0;
        string stotaddr = "";
        int updatecount = 0;
        BigInteger submax = BigInteger.Zero;
        BigInteger totmaxval = BigInteger.Zero;
        BigInteger submaxval = BigInteger.Zero;
        int maxfontwidth = 0;
        //
        AutoCompleteStringCollection autocomp =
            new AutoCompleteStringCollection();
        AutoCompleteStringCollection autocomp_v4 =
            new AutoCompleteStringCollection();
        //
        CultureInfo culture;
        public delegate void ChangeWinFormStringsDelegate(CultureInfo culture);
        public event ChangeWinFormStringsDelegate ChangeUILanguage = delegate { };
        //
        public delegate void ChangeDatabaseDelegate(string dbname);
        public event ChangeDatabaseDelegate ChangeDatabase = delegate { };
        //
        public DBServerInfo ServerInfo = new DBServerInfo();
        public delegate void ChangeDBState(OdbcConnection dbconn, int info);
        public event ChangeDBState changeDBstate = delegate { };
        public static OdbcConnection MySQLconnection = null;
        public static List<WindowsList> windowsList = new List<WindowsList>();
        public string xmlFilename = "IPv6SubnetCalculatorInfo.xml";  // last settings/info xml file name
        MyXMLFile xmlfile = new MyXMLFile();
        //
        public static string ipmode = "v6"; // default mode IPv6
        //
        Bitmap v6Bitmap = null;
        Bitmap v4Bitmap = null;
        #endregion

        public Form1()
        {
            InitializeComponent();

            label10.Text = label1.Text = trackBar1.Value.ToString();
            this.StartEnd.ID = ID; // ID of this Form. Form1 is the main Form.
            this.graph = this.CreateGraphics();
            this.graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            //
            v6Bitmap = Properties.Resources.v6Bt;
            v4Bitmap = Properties.Resources.v4Bt;
            v6Bitmap.Tag = "v6Bt";
            v4Bitmap.Tag = "v4Bt";
            pictureBox1.Image.Tag = "v6Bt";

            xmlfile.ReadValues();

            #region Load UI_Info
            // Load UI_Info
            try
            {
                // ScreenShotValues.Cultur
                if (ScreenShotValues.Cultur != null)
                    this.culture = Thread.CurrentThread.CurrentUICulture = ScreenShotValues.Cultur;

                if (this.culture.Name == "en-US")
                    this.EnglishToolStripMenuItem_Click(this.EnglishToolStripMenuItem, null);
                else if (this.culture.Name == "tr-TR")
                    this.TurkishToolStripMenuItem_Click(this.TurkishToolStripMenuItem, null);
                else if (this.culture.Name == "de-DE")
                    this.GermanToolStripMenuItem_Click(this.GermanToolStripMenuItem, null);
                else // default
                    this.EnglishToolStripMenuItem_Click(this.EnglishToolStripMenuItem, null);

                // ScreenShotValues.mode
                ipmode = ScreenShotValues.mode;

                this.Location = new Point(ScreenShotValues.LocX, ScreenShotValues.LocY);

                if (ipmode == "v6")
                {
                    this.iPv6ToolStripMenuItem.Checked = true;
                    this.iPv4ToolStripMenuItem.Checked = false;
                    this.iPv6ToolStripMenuItem_Click(this.iPv6ToolStripMenuItem, null);
                }
                else if (ipmode == "v4")
                {
                    this.iPv4ToolStripMenuItem.Checked = true;
                    this.iPv6ToolStripMenuItem.Checked = false;
                    this.iPv4ToolStripMenuItem_Click(this.iPv4ToolStripMenuItem, null);

                    this.DefaultView();
                }
                else // default
                {
                    this.iPv6ToolStripMenuItem.Checked = true;
                    this.iPv4ToolStripMenuItem.Checked = false;
                    this.iPv6ToolStripMenuItem_Click(this.iPv6ToolStripMenuItem, null);
                }
                this.listBox1.Items.Clear();
                this.label16.Text = "   ";

                // v6 part:
                switch (ipmode)
                {
                    case "v6":
                        {
                            // ScreenShotValues.Address
                            if (ScreenShotValues.Address != "")
                            {
                                this.textBox2.Text = ScreenShotValues.Address;
                                this.textBox9.Text = ">";

                                if (ScreenShotValues.ResetFlag)
                                    return;
                                else
                                    this.Find_Click(null, null);
                            }
                            else
                            {
                                ResetAllValues();
                                break;
                            }

                            // ScreenShotValues.is128Checked
                            this.checkBox2.Checked = ScreenShotValues.is128Checked;
                            if (ScreenShotValues.is128Checked)
                            {
                                ExpandView();
                            }

                            // ScreenShotValues.initialAddrSpaceNo : loaded in MyXMLFile.cs

                            // ScreenShotValues.TrackBar1Value
                            this.trackBar1.Value = ScreenShotValues.TrackBar1Value;
                            this.trackBar1_Scroll(null, null);

                            // ScreenShotValues.currentAddrSpaceNo
                            GoToAddrSpaceNumber(ScreenShotValues.currentAddrSpaceNo);

                            // ScreenShotValues.isSubnetChecked
                            this.checkBox1.Checked = ScreenShotValues.isSubnetChecked;

                            if (this.checkBox1.Checked && textBox2.Text.Trim() != "")
                                this.Subnets_Click(null, null);

                            // ScreenShotValues.TrackBar2Value
                            this.trackBar2.Value = ScreenShotValues.TrackBar2Value;
                            this.trackBar2_Scroll(null, null);

                            if (this.checkBox1.Checked && textBox2.Text.Trim() != "")
                                this.Subnets_Click(null, null);

                            // ScreenShotValues.isEndChecked
                            this.checkBox3.Checked = ScreenShotValues.isEndChecked;
                        }
                        break;

                    // v4 Part:
                    case "v4":
                        {
                            // ScreenShotValues.Address_v4
                            if (ScreenShotValues.Address_v4 != "")
                            {
                                this.textBox2.Text = ScreenShotValues.Address_v4;
                                this.textBox9.Text = ">";

                                if (ScreenShotValues.ResetFlag_v4)
                                    return;
                                else
                                    this.Find_Click(null, null);
                            }
                            else
                            {
                                ResetAllValues();
                                break;
                            }

                            // ScreenShotValues.initialAddrSpaceNo_v4

                            // ScreenShotValues.TrackBar1Value_v4
                            this.trackBar1.Value = ScreenShotValues.TrackBar1Value_v4;
                            this.trackBar1_Scroll(null, null);

                            // ScreenShotValues.currentAddrSpaceNo_v4
                            GoToAddrSpaceNumber(ScreenShotValues.currentAddrSpaceNo_v4);

                            // ScreenShotValues.isSubnetChecked_v4
                            this.checkBox1.Checked = ScreenShotValues.isSubnetChecked_v4;

                            if (this.checkBox1.Checked && textBox2.Text.Trim() != "")
                                this.Subnets_Click(null, null);

                            // ScreenShotValues.TrackBar2Value_v4
                            this.trackBar2.Value = ScreenShotValues.TrackBar2Value_v4;
                            this.trackBar2_Scroll(null, null);

                            if (this.checkBox1.Checked && textBox2.Text.Trim() != "")
                                this.Subnets_Click(null, null);

                            // ScreenShotValues.isEndChecked_v4 
                            this.checkBox3.Checked = ScreenShotValues.isEndChecked_v4;
                        }
                        break;
                    /* End of Load UI_Info */

                    default:
                        break;
                }
                
                /* Load Fonts */

                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));

                if (ScreenShotValues.textBox2Font != "")
                {
                    this.textBox2.Font = (Font)converter.ConvertFromString(ScreenShotValues.textBox2Font);
                }

                if (ScreenShotValues.textBox1Font != "")
                {
                    this.textBox1.Font = (Font)converter.ConvertFromString(ScreenShotValues.textBox1Font);
                }
                if (ScreenShotValues.textBox3Font != "")
                {
                    this.textBox3.Font = (Font)converter.ConvertFromString(ScreenShotValues.textBox3Font);
                }
                if (ScreenShotValues.textBox5Font != "")
                {
                    this.textBox5.Font = (Font)converter.ConvertFromString(ScreenShotValues.textBox5Font);
                }
                if (ScreenShotValues.textBox4Font != "")
                {
                    this.textBox4.Font = (Font)converter.ConvertFromString(ScreenShotValues.textBox4Font);
                }
                if (ScreenShotValues.textBox8Font != "")
                {
                    this.textBox8.Font = (Font)converter.ConvertFromString(ScreenShotValues.textBox8Font);
                }
                if (ScreenShotValues.Form1_listBox1Font != "")
                {
                    this.listBox1.Font = (Font)converter.ConvertFromString(ScreenShotValues.Form1_listBox1Font);
                    this.listBox1.ItemHeight = this.listBox1.Font.Height;
                    this.maxfontwidth = 0;
                    this.listBox1.HorizontalExtent = 0;
                }
                /* End of Load Fonts */

                /* Load DBServerInfo */
                if (ScreenShotValues.DriverName != "")
                    this.ServerInfo.DriverName = ScreenShotValues.DriverName;

                if (ScreenShotValues.ServerIP != null)
                    this.ServerInfo.ServerIP = ScreenShotValues.ServerIP;

                this.ServerInfo.PortNum = ScreenShotValues.PortNum;

                if (ScreenShotValues.DBname != "")
                    this.ServerInfo.DBname = ScreenShotValues.DBname;

                if (ScreenShotValues.Tablename != "")
                    this.ServerInfo.Tablename = ScreenShotValues.Tablename;

                if (ScreenShotValues.DBname_v4 != "")
                    this.ServerInfo.DBname_v4 = ScreenShotValues.DBname_v4;

                if (ScreenShotValues.Tablename_v4 != "")
                    this.ServerInfo.Tablename_v4 = ScreenShotValues.Tablename_v4;

                if (ScreenShotValues.Username != "")
                    this.ServerInfo.Username = ScreenShotValues.Username;

                /* End of Load DBServerInfo */
            }
            catch (Exception ex)
            {
                MessageBox.Show(StringsDictionary.KeyValue("Form1_ExceptionReadingXML", this.culture) 
                    + ex.Message, "Exception: XML Values", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            #endregion
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.textBox2.AutoCompleteCustomSource = autocomp;

            this.Location = new Point(ScreenShotValues.LocX, ScreenShotValues.LocY);
        }

        private void UpdateStatus()
        {
            label1.Text = trackBar1.Value.ToString();
            label10.Text = trackBar2.Value.ToString();

            delta = trackBar2.Value - trackBar1.Value;
            label12.Text = delta.ToString();
            submax = (BigInteger.One << delta);
            submaxval = submax - BigInteger.One;

            BigInteger nxt = BigInteger.One;

            if (trackBar1.Value == 128)
            {
                stotaddr = BigInteger.Pow(2, 128).ToString("R");
                totmaxval = BigInteger.Pow(2, 128) - BigInteger.One;  //MaxValue;
            }
            else
            {
                nxt = (nxt << trackBar1.Value);
                totmaxval = nxt - BigInteger.One;
                stotaddr = nxt.ToString();
            }

            toolStripStatusLabel1.Text =
                StringsDictionary.KeyValue("Form1_UpdateStatus_delta", this.culture)
                + delta + StringsDictionary.KeyValue("Form1_UpdateStatus_subnets", this.culture)
                + submax.ToString() + StringsDictionary.KeyValue("Form1_UpdateStatus_addrs", this.culture)
                + stotaddr + "]";

            if (MySQLconnection == null)
            {
                toolStripStatusLabel2.Text = "db=Down";
            }
            else
            {
                if (MySQLconnection.State == ConnectionState.Open)
                {
                    toolStripStatusLabel2.Text = "db=Up";
                }
                else if (MySQLconnection.State == ConnectionState.Closed)
                {
                    toolStripStatusLabel2.Text = "db=Down";
                }
            }
        }

        private void UpdateCount()
        {
            if (this.checkBox3.CheckState == CheckState.Checked)
                this.updatecount = this.listBox1.Items.Count / 3;
            else
                this.updatecount = this.listBox1.Items.Count;

            this.textBox7.Text = "[" + this.updatecount.ToString()
                + StringsDictionary.KeyValue("Form1_UpdateCount_entries", culture);

            if (this.listBox1.Items.Count > 0)
            {
                this.currentidx =
                    BigInteger.Parse(this.listBox1.Items[0].ToString().Split('>')[0].TrimStart('p'));

                if (this.submax / 128 >= 1)
                {
                    this.pix = this.submax / 128;
                }
                else
                {
                    this.pix = 128;
                }

                this.Form1_Paint(null, null);
            }
        }

        private void UpdatePrintBin(SEaddress StartEnd, CheckState is128Checked)
        {
            richTextBox1.Text = "";
            richTextBox1.Text = v6ST.PrintBin(StartEnd, this.trackBar1.Value, is128Checked);

            int count1 = trackBar1.Value + (trackBar1.Value / 4);
            int count2 = trackBar2.Value + (trackBar2.Value / 4) - count1;

            richTextBox1.Select(0, count1);
            richTextBox1.SelectionBackColor = Color.Red;
            richTextBox1.SelectionColor = Color.White;

            richTextBox1.Select(count1, count2);
            richTextBox1.SelectionBackColor = Color.Turquoise;
            richTextBox1.SelectionColor = Color.Black;
        }

        private void UpdatePrintBin_v4(SEaddress StartEnd)
        {
            richTextBox1.Text = "";
            richTextBox1.Text = v6ST.PrintBin_v4(StartEnd, this.trackBar1.Value);

            int count1 = trackBar1.Value + (trackBar1.Value / 4);
            int count2 = trackBar2.Value + (trackBar2.Value / 4) - count1;

            richTextBox1.Select(0, count1);
            richTextBox1.SelectionBackColor = Color.Red;
            richTextBox1.SelectionColor = Color.White;

            richTextBox1.Select(count1, count2);
            richTextBox1.SelectionBackColor = Color.Turquoise;
            richTextBox1.SelectionColor = Color.Black;
        }

        private void UpdateBackColor()
        {
            textBox1.BackColor = Color.FromKnownColor(KnownColor.WhiteSmoke);
        }

        private void Calculate(string strinp)
        {
            if (ipmode == "v6")
                ScreenShotValues.ResetFlag = false;
            else
                ScreenShotValues.ResetFlag_v4 = false;

            if (ipmode == "v6")
            {
                if (v6ST.IsAddressCorrect(strinp))
                {
                    textBox9.Text = StringsDictionary.KeyValue("Form1_" + v6ST.errmsg, this.culture);
                    string Resv6 = v6ST.FormalizeAddr(strinp);
                    textBox1.Text = v6ST.Kolonlar(Resv6, this.checkBox2.CheckState);

                    if (this.checkBox2.CheckState == CheckState.Checked) // 128 bits
                    {
                        if (Resv6.Length == 32)
                            Resv6 = "0" + Resv6;

                        StartEnd.ResultIPAddr = BigInteger.Parse(Resv6, NumberStyles.AllowHexSpecifier);
                        StartEnd.slash = this.trackBar1.Value;

                        StartEnd = v6ST.StartEndAddresses(StartEnd, this.checkBox2.CheckState);
                        subnets.Start = StartEnd.Start;
                        subnets.End = BigInteger.Zero;
                        subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
                        subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;

                        textBox3.Text = textBox5.Text = "";
                        string s = String.Format("{0:x}", StartEnd.Start);
                        if (s.Length > 32)
                            s = s.Substring(1, 32);
                        s = v6ST.Kolonlar(s, this.checkBox2.CheckState) + "/" + this.trackBar1.Value;
                        textBox3.Text = s;

                        s = String.Format("{0:x}", StartEnd.End);
                        if (s.Length > 32)
                            s = s.Substring(1, 32);
                        s = v6ST.Kolonlar(s, this.checkBox2.CheckState) + "/" + this.trackBar1.Value;
                        textBox5.Text = s;
                    }
                    else if (this.checkBox2.CheckState == CheckState.Unchecked) // 64 bits
                    {
                        Resv6 = Resv6.Substring(0, 16); // From left First 64bits
                        if (Resv6.Length == 16)
                            Resv6 = "0" + Resv6;

                        StartEnd.ResultIPAddr = BigInteger.Parse(Resv6, NumberStyles.AllowHexSpecifier);
                        StartEnd.slash = trackBar1.Value;

                        StartEnd = v6ST.StartEndAddresses(StartEnd, this.checkBox2.CheckState);
                        subnets.Start = StartEnd.Start;
                        subnets.End = BigInteger.Zero;
                        subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
                        subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;

                        textBox3.Text = textBox5.Text = "";
                        string s = String.Format("{0:x}", StartEnd.Start);
                        if (s.Length > 16)
                            s = s.Substring(1, 16);
                        s = v6ST.Kolonlar(s, this.checkBox2.CheckState) + "/" + this.trackBar1.Value;
                        textBox3.Text = s;

                        s = String.Format("{0:x}", StartEnd.End);
                        if (s.Length > 16)
                            s = s.Substring(1, 16);
                        s = v6ST.Kolonlar(s, this.checkBox2.CheckState) + "/" + this.trackBar1.Value;
                        textBox5.Text = s;
                    }

                    StartEnd = v6ST.NextSpace(StartEnd, this.checkBox2.CheckState);
                    textBox4.Text = "#" + StartEnd.subnetidx.ToString();
                    ScreenShotValues.initialAddrSpaceNo = StartEnd.subnetidx;
                    ScreenShotValues.Address = this.textBox2.Text.Trim();

                    trackBar1.Enabled = true;
                    checkBox1.Enabled = true;
                    checkBox2.Enabled = true;
                    NextSpace.Enabled = true;
                    PrevSpace.Enabled = true;
                    ResetAll.Enabled = true;
                    //
                    Forwd.Enabled = false;
                    Backwd.Enabled = false;
                    Last.Enabled = false;
                    //
                    goToAddrSpaceNumberToolStripMenuItem.Enabled = true;

                    UpdatePrintBin(StartEnd, this.checkBox2.CheckState);
                    this.textBox2.AutoCompleteCustomSource = autocomp;
                    autocomp.Add(strinp);
                    MaskValue();
                    this.Find.Focus();
                    //
                    if (this.checkBox1.Enabled && this.checkBox1.CheckState == CheckState.Checked)
                        this.Subnets_Click(null, null);
                }
                else
                {
                    ResetAllValues();
                    ResetAll.Enabled = false;
                    textBox9.Text = StringsDictionary.KeyValue("Form1_" + v6ST.errmsg, this.culture);
                }
            }
            else // ipmode=v4
            {
                if (v6ST.IsAddressCorrect_v4(strinp))
                {
                    textBox9.Text = StringsDictionary.KeyValue("Form1_" + v6ST.errmsg, this.culture);

                    string Resv6 = v6ST.FormalizeAddr_v4(strinp);

                    textBox1.Text = v6ST.IPv4Format(Resv6);
                    textBox2.Text = textBox1.Text;

                    StartEnd.ResultIPAddr = BigInteger.Parse("0" + Resv6, NumberStyles.AllowHexSpecifier);
                    StartEnd.slash = this.trackBar1.Value;

                    StartEnd = v6ST.StartEndAddresses_v4(StartEnd);
                    subnets.Start = StartEnd.Start;
                    subnets.End = BigInteger.Zero;
                    subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
                    subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;

                    textBox3.Text = textBox5.Text = "";
                    string s = String.Format("{0:x}", StartEnd.Start);

                    textBox3.Text = v6ST.IPv4Format(s) + "/" + this.trackBar1.Value.ToString();

                    s = String.Format("{0:x}", StartEnd.End);

                    textBox5.Text = v6ST.IPv4Format(s) + "/" + this.trackBar1.Value.ToString();

                    StartEnd = v6ST.NextSpace_v4(StartEnd);
                    textBox4.Text = "#" + StartEnd.subnetidx.ToString();
                    ScreenShotValues.initialAddrSpaceNo_v4 = StartEnd.subnetidx;
                    ScreenShotValues.Address_v4 = this.textBox2.Text.Trim();

                    trackBar1.Enabled = true;
                    checkBox1.Enabled = true;
                    checkBox2.Enabled = false;
                    NextSpace.Enabled = true;
                    PrevSpace.Enabled = true;
                    ResetAll.Enabled = true;
                    //
                    Forwd.Enabled = false;
                    Backwd.Enabled = false;
                    Last.Enabled = false;
                    //
                    goToAddrSpaceNumberToolStripMenuItem.Enabled = true;

                    UpdatePrintBin_v4(StartEnd);
                    this.textBox2.AutoCompleteCustomSource = autocomp_v4;
                    autocomp_v4.Add(strinp);
                    MaskValue_v4();
                    this.Find.Focus();
                    //
                    if (this.checkBox1.Enabled && this.checkBox1.CheckState == CheckState.Checked)
                        this.Subnets_Click(null, null);
                }
                else
                {
                    ResetAllValues();
                    ResetAll.Enabled = false;
                    textBox9.Text = StringsDictionary.KeyValue("Form1_" + v6ST.errmsg, this.culture);
                }
            }

            UpdateStatus();
            UpdateBackColor();
        }

        private void Find_Click(object sender, EventArgs e)
        {
            textBox2.Text = textBox2.Text.Trim();
            textBox3.Text = textBox4.Text = textBox5.Text = richTextBox1.Text = this.textBox7.Text = textBox8.Text = "";
            this.Backwd.Enabled = this.Forwd.Enabled = this.Last.Enabled = false;

            listBox1.Items.Clear();
            this.label16.Text = "   ";

            Calculate(textBox2.Text);
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            textBox9.Text = StringsDictionary.KeyValue("Form1_textBox2_Enter", this.culture);
            
            //this.textBox1.Text = "";
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            textBox2.Text = textBox2.Text.Trim().ToLower();

            textBox3.Text = textBox5.Text = richTextBox1.Text = this.textBox7.Text = "";
            this.Backwd.Enabled = this.Forwd.Enabled = this.Last.Enabled = false;

            listBox1.Items.Clear();

            Calculate(textBox2.Text);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (ipmode == "v6")
            {
                Backwd.Enabled = Forwd.Enabled = Last.Enabled = this.checkBox3.Enabled = false;
                trackBar2.Value = trackBar1.Value;

                this.maxfontwidth = 0;
                this.listBox1.HorizontalExtent = 0;

                UpdateStatus();

                listBox1.Items.Clear();
                this.label16.Text = "   ";
                textBox7.Text = "";

                StartEnd.slash = StartEnd.subnetslash = trackBar1.Value;

                StartEnd = v6ST.StartEndAddresses(StartEnd, this.checkBox2.CheckState);
                subnets.Start = StartEnd.Start;
                subnets.End = BigInteger.Zero;
                subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
                subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;

                textBox3.Text = textBox5.Text = "";

                if (this.checkBox2.CheckState == CheckState.Checked)
                {
                    string s = String.Format("{0:x}", StartEnd.Start);
                    if (s.Length > 32)
                        s = s.Substring(1, 32);
                    s = v6ST.Kolonlar(s, this.checkBox2.CheckState) + "/" + this.trackBar1.Value;
                    textBox3.Text = s;

                    s = String.Format("{0:x}", StartEnd.End);
                    if (s.Length > 32)
                        s = s.Substring(1, 32);
                    s = v6ST.Kolonlar(s, this.checkBox2.CheckState) + "/" + this.trackBar1.Value;
                    textBox5.Text = s;
                }
                else if (this.checkBox2.CheckState == CheckState.Unchecked)
                {
                    string s = String.Format("{0:x}", StartEnd.Start);
                    if (s.Length > 16)
                        s = s.Substring(1, 16);
                    s = v6ST.Kolonlar(s, this.checkBox2.CheckState) + "/" + this.trackBar1.Value;
                    textBox3.Text = s;

                    s = String.Format("{0:x}", StartEnd.End);
                    if (s.Length > 16)
                        s = s.Substring(1, 16);
                    s = v6ST.Kolonlar(s, this.checkBox2.CheckState) + "/" + this.trackBar1.Value;
                    textBox5.Text = s;
                }


                StartEnd = v6ST.NextSpace(StartEnd, this.checkBox2.CheckState);
                textBox4.Text = "#" + StartEnd.subnetidx.ToString();
                ScreenShotValues.initialAddrSpaceNo = StartEnd.subnetidx;
                ScreenShotValues.TrackBar1Value = this.trackBar1.Value;

                UpdatePrintBin(StartEnd, this.checkBox2.CheckState);

                delta = trackBar2.Value - trackBar1.Value;

                // Mask:
                MaskValue();
            }
            else // v4
            {
                Backwd.Enabled = Forwd.Enabled = Last.Enabled = this.checkBox3.Enabled = false;
                trackBar2.Value = trackBar1.Value;

                this.maxfontwidth = 0;
                this.listBox1.HorizontalExtent = 0;

                UpdateStatus();

                listBox1.Items.Clear();
                this.label16.Text = "   ";
                textBox7.Text = "";

                StartEnd.slash = StartEnd.subnetslash = trackBar1.Value;

                StartEnd = v6ST.StartEndAddresses_v4(StartEnd);
                subnets.Start = StartEnd.Start;
                subnets.End = 0;
                subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
                subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;

                textBox3.Text = textBox5.Text = "";

                string s = String.Format("{0:x}", StartEnd.Start);
                textBox3.Text = v6ST.IPv4Format(s) + "/" + this.trackBar1.Value.ToString();

                s = String.Format("{0:x}", StartEnd.End);
                textBox5.Text = v6ST.IPv4Format(s) + "/" + this.trackBar1.Value.ToString();

                StartEnd = v6ST.NextSpace_v4(StartEnd);
                textBox4.Text = "#" + StartEnd.subnetidx.ToString();
                ScreenShotValues.initialAddrSpaceNo_v4 = StartEnd.subnetidx;
                ScreenShotValues.TrackBar1Value_v4 = this.trackBar1.Value;

                UpdatePrintBin_v4(StartEnd);

                delta = trackBar2.Value - trackBar1.Value;

                // Mask:
                MaskValue_v4();
            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            if (ipmode == "v6")
            {
                listBox1.Items.Clear();
                this.label16.Text = "   ";
                this.maxfontwidth = 0;
                this.listBox1.HorizontalExtent = 0;

                Backwd.Enabled = Forwd.Enabled = Last.Enabled = this.checkBox3.Enabled = false;
                textBox7.Text = "";

                delta = trackBar2.Value - trackBar1.Value;

                if (delta < 0)
                {
                    trackBar2.Value = trackBar1.Value;
                    label12.Text = "0";
                    delta = 0;
                }
                else
                {
                    label10.Text = trackBar2.Value.ToString();
                    label12.Text = delta.ToString();
                }

                StartEnd.subnetslash = trackBar2.Value;
                ScreenShotValues.TrackBar2Value = trackBar2.Value;

                UpdateStatus();
                UpdatePrintBin(StartEnd, this.checkBox2.CheckState);
                graph.FillRectangle(new SolidBrush(Form1.DefaultBackColor), 250, 256, 0, 11);
                Form1_Paint(null, null);

                // Mask:
                MaskValue();
            }
            else //v4
            {
                listBox1.Items.Clear();
                this.label16.Text = "   ";
                this.maxfontwidth = 0;
                this.listBox1.HorizontalExtent = 0;

                Backwd.Enabled = Forwd.Enabled = Last.Enabled = this.checkBox3.Enabled = false;

                textBox7.Text = "";

                delta = trackBar2.Value - trackBar1.Value;

                if (delta < 0)
                {
                    trackBar2.Value = trackBar1.Value;
                    label12.Text = "0";
                    delta = 0;
                }
                else
                {
                    label10.Text = trackBar2.Value.ToString();
                    label12.Text = delta.ToString();
                }

                StartEnd.subnetslash = trackBar2.Value;
                ScreenShotValues.TrackBar2Value_v4 = trackBar2.Value;

                UpdateStatus();
                UpdatePrintBin_v4(StartEnd);
                graph.FillRectangle(new SolidBrush(Form1.DefaultBackColor), 250, 256, 0, 11);
                Form1_Paint(null, null);

                // Mask:
                MaskValue_v4();
            }
        }

        private void MaskValue()
        {
            BigInteger mask = v6ST.PrepareMask((short)trackBar2.Value);
            textBox8.Text = v6ST.CompressAddress(v6ST.Kolonlar(mask.ToString("x").TrimStart('0'), CheckState.Checked));
        }

        private void MaskValue_v4()
        {
            BigInteger mask = v6ST.PrepareMask_v4((short)trackBar2.Value);
            textBox8.Text = v6ST.IPv4Format(mask.ToString("x"));
        }

        private void Subnets_Click(object sender, EventArgs e)
        {
            if (ipmode == "v6")
            {
                this.listBox1.Items.Clear();
                this.label16.Text = "   ";
                this.maxfontwidth = 0;
                this.listBox1.HorizontalExtent = 0;

                if (this.checkBox2.CheckState == CheckState.Unchecked && this.trackBar2.Value == 64
                    ||
                    this.checkBox2.CheckState == CheckState.Checked && this.trackBar2.Value == 128
                    )
                {
                    this.checkBox3.Checked = false;
                    this.checkBox3.Enabled = false;
                }
                else
                    this.checkBox3.Enabled = true;

                int delta = this.trackBar2.Value - this.trackBar1.Value;

                StartEnd.slash = this.trackBar1.Value;
                StartEnd.subnetslash = this.trackBar2.Value;
                StartEnd.upto = upto;

                subnets = v6ST.ListFirstPage(StartEnd, this.checkBox2.CheckState, this.checkBox3.CheckState);
                this.page.End = subnets.End;
                this.listBox1.Items.AddRange(subnets.liste.ToArray());

                BigInteger maxsub = (BigInteger.One << delta);

                if (maxsub <= upto)
                {
                    this.Backwd.Enabled = false;
                    this.Forwd.Enabled = false;
                    this.Last.Enabled = false;
                }
                else
                {
                    this.Backwd.Enabled = false;
                    this.Forwd.Enabled = true;
                    this.Last.Enabled = true;
                }
            }
            else // v4
            {
                this.listBox1.Items.Clear();
                this.label16.Text = "   ";
                this.maxfontwidth = 0;
                this.listBox1.HorizontalExtent = 0;


                if (this.trackBar2.Value == 32)
                {
                    this.checkBox3.Checked = false;
                    this.checkBox3.Enabled = false;
                }
                else
                {
                    this.checkBox3.Enabled = true;
                }

                int delta = this.trackBar2.Value - this.trackBar1.Value;

                StartEnd.slash = this.trackBar1.Value;
                StartEnd.subnetslash = this.trackBar2.Value;
                StartEnd.upto = upto;

                subnets = v6ST.ListFirstPage_v4(StartEnd, this.checkBox3.CheckState);
                this.page.End = subnets.End;
                this.listBox1.Items.AddRange(subnets.liste.ToArray());

                BigInteger maxsub = (BigInteger.One << delta);

                if (maxsub <= upto)
                {
                    this.Backwd.Enabled = false;
                    this.Forwd.Enabled = false;
                    this.Last.Enabled = false;
                }
                else
                {
                    this.Backwd.Enabled = false;
                    this.Forwd.Enabled = true;
                    this.Last.Enabled = true;
                }
            }

            UpdateCount();
        }

        private void Backwd_Click(object sender, EventArgs e)
        {
            if (ipmode == "v6")
            {
                this.listBox1.Items.Clear();
                this.label16.Text = "   ";
                this.maxfontwidth = 0;
                this.listBox1.HorizontalExtent = 0;

                subnets.slash = this.trackBar1.Value;
                subnets.subnetslash = this.trackBar2.Value;
                subnets.upto = upto;
                subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
                subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;

                subnets.End = page.End = page.Start - BigInteger.One;
                subnets = v6ST.ListPageBackward(subnets, this.checkBox2.CheckState, this.checkBox3.CheckState);
                page.Start = subnets.Start;

                this.listBox1.Items.AddRange(subnets.liste.ToArray());

                if (subnets.Start.Equals(StartEnd.Start))
                {
                    this.Backwd.Enabled = false;
                    this.Forwd.Enabled = true;
                    this.Last.Enabled = true;
                    UpdateCount();
                    return;
                }
                else
                {
                    this.Forwd.Enabled = true;
                    this.Last.Enabled = true;
                }
            }
            else //v4
            {
                this.listBox1.Items.Clear();
                this.label16.Text = "   ";
                this.maxfontwidth = 0;
                this.listBox1.HorizontalExtent = 0;

                subnets.slash = this.trackBar1.Value;
                subnets.subnetslash = this.trackBar2.Value;
                subnets.upto = upto;
                subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
                subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;

                subnets.End = page.End = page.Start - 1;
                subnets = v6ST.ListPageBackward_v4(subnets, this.checkBox3.CheckState);
                page.Start = subnets.Start;

                this.listBox1.Items.AddRange(subnets.liste.ToArray());

                if (subnets.Start.Equals(StartEnd.Start))
                {
                    this.Backwd.Enabled = false;
                    this.Forwd.Enabled = true;
                    this.Last.Enabled = true;
                    UpdateCount();
                    return;
                }
                else
                {
                    this.Forwd.Enabled = true;
                    this.Last.Enabled = true;
                }
            }

            UpdateCount();
        }

        private void Forwd_Click(object sender, EventArgs e)
        {
            if (ipmode == "v6")
            {
                this.listBox1.Items.Clear();
                this.label16.Text = "   ";
                this.maxfontwidth = 0;
                this.listBox1.HorizontalExtent = 0;

                subnets.slash = this.trackBar1.Value;
                subnets.subnetslash = this.trackBar2.Value;
                subnets.upto = upto;
                subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
                subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;

                subnets.Start = page.Start = page.End + BigInteger.One;
                subnets = v6ST.ListPageForward(subnets, this.checkBox2.CheckState, this.checkBox3.CheckState);
                this.page.End = subnets.End;

                this.listBox1.Items.AddRange(subnets.liste.ToArray());

                if (subnets.End.Equals(StartEnd.End))
                {
                    this.Forwd.Enabled = false;
                    this.Last.Enabled = false;
                    this.Backwd.Enabled = true;
                    UpdateCount();
                    return;
                }
                else
                {
                    this.Backwd.Enabled = true;
                    this.Last.Enabled = true;
                }
            }
            else //v4
            {
                this.listBox1.Items.Clear();
                this.label16.Text = "   ";
                this.maxfontwidth = 0;
                this.listBox1.HorizontalExtent = 0;

                subnets.slash = this.trackBar1.Value;
                subnets.subnetslash = this.trackBar2.Value;
                subnets.upto = upto;
                subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
                subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;

                subnets.Start = page.Start = page.End + 1;
                subnets = v6ST.ListPageForward_v4(subnets, this.checkBox3.CheckState);
                this.page.End = subnets.End;

                this.listBox1.Items.AddRange(subnets.liste.ToArray());

                if (subnets.End.Equals(StartEnd.End))
                {
                    this.Forwd.Enabled = false;
                    this.Last.Enabled = false;
                    this.Backwd.Enabled = true;
                    UpdateCount();
                    return;
                }
                else
                {
                    this.Backwd.Enabled = true;
                    this.Last.Enabled = true;
                }
            }

            UpdateCount();
        }

        private void Last_Click(object sender, EventArgs e)
        {
            if (ipmode == "v6")
            {
                this.listBox1.Items.Clear();
                this.label16.Text = "   ";
                this.maxfontwidth = 0;
                this.listBox1.HorizontalExtent = 0;

                subnets.slash = this.trackBar1.Value;
                subnets.subnetslash = this.trackBar2.Value;
                subnets.upto = upto;
                subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
                subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;


                subnets.End = page.End = StartEnd.UpperLimitAddress;
                subnets = v6ST.ListLastPage(subnets, this.checkBox2.CheckState, this.checkBox3.CheckState);
                page.Start = subnets.Start;

                this.listBox1.Items.AddRange(subnets.liste.ToArray());

                this.Backwd.Enabled = true;
                this.Forwd.Enabled = false;
                this.Last.Enabled = false;
            }
            else //v4
            {
                this.listBox1.Items.Clear();
                this.label16.Text = "   ";
                this.maxfontwidth = 0;
                this.listBox1.HorizontalExtent = 0;

                subnets.slash = this.trackBar1.Value;
                subnets.subnetslash = this.trackBar2.Value;
                subnets.upto = upto;
                subnets.LowerLimitAddress = StartEnd.LowerLimitAddress;
                subnets.UpperLimitAddress = StartEnd.UpperLimitAddress;


                subnets.End = page.End = StartEnd.UpperLimitAddress;
                subnets = v6ST.ListLastPage_v4(subnets, this.checkBox3.CheckState);
                page.Start = subnets.Start;

                this.listBox1.Items.AddRange(subnets.liste.ToArray());

                this.Backwd.Enabled = true;
                this.Forwd.Enabled = false;
                this.Last.Enabled = false;
            }

            UpdateCount();
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                if (ipmode == "v6")
                {
                    ScreenShotValues.isSubnetChecked = true;
                }
                else // v4
                {
                    ScreenShotValues.isSubnetChecked_v4 = true;
                }
            }
            else
            {
                if (ipmode == "v6")
                {
                    ScreenShotValues.isSubnetChecked = false;
                }
                else // v4
                {
                    ScreenShotValues.isSubnetChecked_v4 = false;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            this.label16.Text = "   ";

            if (checkBox1.Checked)
            {
                trackBar2.Enabled = true;
                trackBar2.Value = trackBar1.Value;
                label10.Enabled = label12.Enabled = true;
                label10.Text = trackBar1.Value.ToString();
                int delta = (trackBar2.Value - trackBar1.Value);
                label12.Text = delta.ToString();
                Subnets.Enabled = true;
                goToSubnetNumberToolStripMenuItem1.Enabled = true;
            }
            else
            {
                trackBar2.Enabled = false;
                trackBar2.Value = trackBar1.Value;
                label10.Enabled = label12.Enabled = false;
                label10.Text = label12.Text = "";
                Subnets.Enabled = Forwd.Enabled = Backwd.Enabled = Last.Enabled = false;
                goToSubnetNumberToolStripMenuItem1.Enabled = false;
            }

            if (ipmode == "v6")
                UpdatePrintBin(StartEnd, this.checkBox2.CheckState);
            else //v4
                UpdatePrintBin_v4(StartEnd);
        }

        private void ResetAll_Click(object sender, EventArgs e)
        {
            if (ipmode == "v6")
                ScreenShotValues.ResetFlag = true;
            else
                ScreenShotValues.ResetFlag_v4 = true;

            ResetAllValues();
        }

        private void ResetAllValues()
        {
            textBox1.Text = "";
            UpdateBackColor();
            trackBar1.Value = trackBar1.Minimum;
            trackBar2.Value = trackBar2.Minimum;
            label1.Text = trackBar1.Minimum.ToString();
            label10.Text = trackBar2.Minimum.ToString();
            label12.Text = "0";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            listBox1.Items.Clear();
            this.label16.Text = "   ";
            PrevSpace.Enabled = false;
            NextSpace.Enabled = false;
            trackBar1.Enabled = false;
            this.goToAddrSpaceNumberToolStripMenuItem.Enabled = false;

            checkBox1.Checked = false;
            checkBox1.Enabled = false;
            checkBox3.Checked = false;
            checkBox3.Enabled = false;


            richTextBox1.Text = " ";

            if (ipmode == "v4")
            {
                this.checkBox2.Checked = false;
                this.checkBox2.Enabled = false;
            }

            UpdateStatus();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(StringsDictionary.KeyValue("Form1_KeyDown_msg", this.culture),
                StringsDictionary.KeyValue("Form1_KeyDown_header", this.culture),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
            {
                Application.Exit();
            }
            else
                return;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 about = new AboutBox1();
            about.ShowDialog();
        }

        private void NextSpace_Click(object sender, EventArgs e)
        {
            if (ipmode == "v6")
            {
                listBox1.Items.Clear();
                this.label16.Text = "   ";
                this.Backwd.Enabled = false;
                this.Forwd.Enabled = false;
                this.Last.Enabled = false;

                StartEnd.Start = StartEnd.End + BigInteger.One;

                if (this.checkBox2.CheckState == CheckState.Checked)
                {
                    BigInteger big2 = BigInteger.One + BigInteger.One;
                    if (StartEnd.End == (BigInteger.Pow(big2, 128) - BigInteger.One))
                    {
                        StartEnd.Start = BigInteger.Zero;
                    }
                }
                else if (this.checkBox2.CheckState == CheckState.Unchecked)
                {
                    BigInteger big2 = BigInteger.One + BigInteger.One;
                    if (StartEnd.End == (BigInteger.Pow(big2, 64) - BigInteger.One))
                    {
                        StartEnd.Start = BigInteger.Zero;
                    }
                }

                StartEnd = v6ST.NextSpace(StartEnd, this.checkBox2.CheckState);

                textBox4.Text = "#" + StartEnd.subnetidx.ToString();
                ScreenShotValues.currentAddrSpaceNo = StartEnd.subnetidx;
                
                subnets.Start = StartEnd.Start;
                subnets.End = BigInteger.Zero;
                StartEnd.ResultIPAddr = StartEnd.Start;

                textBox3.Text = textBox5.Text = textBox7.Text = "";
                string s = String.Format("{0:x}", StartEnd.Start);

                if (this.checkBox2.CheckState == CheckState.Checked)
                {
                    if (s.Length > 32)
                        s = s.Substring(1, 32);
                }
                else if (this.checkBox2.CheckState == CheckState.Unchecked)
                {
                    if (s.Length > 16)
                        s = s.Substring(1, 16);
                }

                textBox1.Text = s = v6ST.Kolonlar(s, this.checkBox2.CheckState);
                s += "/" + this.trackBar1.Value;
                textBox3.Text = s;
                textBox1.Text = v6ST.FormalizeAddr(textBox1.Text);
                textBox1.Text = v6ST.Kolonlar(textBox1.Text, this.checkBox2.CheckState);

                textBox1.BackColor = Color.FromKnownColor(KnownColor.Info);

                s = String.Format("{0:x}", StartEnd.End);

                if (this.checkBox2.CheckState == CheckState.Checked)
                {
                    if (s.Length > 32)
                        s = s.Substring(1, 32);
                }
                else if (this.checkBox2.CheckState == CheckState.Unchecked)
                {
                    if (s.Length > 16)
                        s = s.Substring(1, 16);
                }

                s = v6ST.Kolonlar(s, this.checkBox2.CheckState) + "/" + this.trackBar1.Value;
                textBox5.Text = s;

                UpdatePrintBin(StartEnd, this.checkBox2.CheckState);

                if (ScreenShotValues.currentAddrSpaceNo == ScreenShotValues.initialAddrSpaceNo)
                {
                    this.Calculate(this.textBox2.Text.Trim());
                }
            }
            else // v4
            {
                listBox1.Items.Clear();
                this.label16.Text = "   ";
                this.Backwd.Enabled = false;
                this.Forwd.Enabled = false;
                this.Last.Enabled = false;

                StartEnd.Start = StartEnd.End + BigInteger.One;

                BigInteger big2 = BigInteger.One + BigInteger.One;
                if (StartEnd.End == (BigInteger.Pow(big2, 32) - BigInteger.One))
                {
                    StartEnd.Start = BigInteger.Zero;
                }

                StartEnd = v6ST.NextSpace_v4(StartEnd);

                textBox4.Text = "#" + StartEnd.subnetidx.ToString();
                ScreenShotValues.currentAddrSpaceNo_v4 = StartEnd.subnetidx;

                subnets.Start = StartEnd.Start;
                subnets.End = 0;
                StartEnd.ResultIPAddr = StartEnd.Start;

                textBox3.Text = textBox5.Text = textBox7.Text = "";

                string s = String.Format("{0:x}", StartEnd.Start);
                textBox3.Text = v6ST.IPv4Format(s) + "/" + this.trackBar1.Value.ToString();

                textBox1.Text = textBox3.Text.Split('/')[0];

                textBox1.BackColor = Color.FromKnownColor(KnownColor.Info);

                s = String.Format("{0:x}", StartEnd.End);
                textBox5.Text = v6ST.IPv4Format(s) + "/" + this.trackBar1.Value.ToString();

                UpdatePrintBin_v4(StartEnd);

                if (ScreenShotValues.currentAddrSpaceNo_v4 == ScreenShotValues.initialAddrSpaceNo_v4)
                {
                    this.Calculate(this.textBox2.Text.Trim());
                }
            }
        }

        private void PrevSpace_Click(object sender, EventArgs e)
        {
            if (ipmode == "v6")
            {
                listBox1.Items.Clear();
                this.label16.Text = "   ";

                this.Backwd.Enabled = false;
                this.Forwd.Enabled = false;
                this.Last.Enabled = false;

                StartEnd.End = StartEnd.Start - BigInteger.One;

                if (this.checkBox2.CheckState == CheckState.Checked)
                {
                    BigInteger big2 = BigInteger.One + BigInteger.One;

                    if (StartEnd.Start == BigInteger.Zero)
                    {
                        StartEnd.End = BigInteger.Pow(big2, 128) - BigInteger.One;
                    }
                }
                if (this.checkBox2.CheckState == CheckState.Unchecked)
                {
                    BigInteger big2 = BigInteger.One + BigInteger.One;

                    if (StartEnd.Start == BigInteger.Zero)
                    {
                        StartEnd.End = BigInteger.Pow(big2, 64) - BigInteger.One;
                    }
                }

                StartEnd = v6ST.PrevSpace(StartEnd, this.checkBox2.CheckState);
                textBox4.Text = "#" + StartEnd.subnetidx.ToString();
                ScreenShotValues.currentAddrSpaceNo = StartEnd.subnetidx;

                subnets.Start = StartEnd.Start;
                subnets.End = BigInteger.Zero;
                StartEnd.ResultIPAddr = StartEnd.Start;

                textBox3.Text = textBox5.Text = textBox7.Text = "";
                string s = String.Format("{0:x}", StartEnd.Start);

                if (this.checkBox2.CheckState == CheckState.Checked)
                {
                    if (s.Length > 32)
                        s = s.Substring(1, 32);
                }
                if (this.checkBox2.CheckState == CheckState.Unchecked)
                {
                    if (s.Length > 16)
                        s = s.Substring(1, 16);
                }

                textBox1.Text = s = v6ST.Kolonlar(s, this.checkBox2.CheckState);
                s += "/" + this.trackBar1.Value;
                textBox3.Text = s;
                textBox1.Text = v6ST.FormalizeAddr(textBox1.Text);
                textBox1.Text = v6ST.Kolonlar(textBox1.Text, this.checkBox2.CheckState);

                textBox1.BackColor = Color.FromKnownColor(KnownColor.Info);

                s = String.Format("{0:x}", StartEnd.End);

                if (this.checkBox2.CheckState == CheckState.Checked)
                {
                    if (s.Length > 32)
                        s = s.Substring(1, 32);
                }
                if (this.checkBox2.CheckState == CheckState.Unchecked)
                {
                    if (s.Length > 16)
                        s = s.Substring(1, 16);
                }

                s = v6ST.Kolonlar(s, this.checkBox2.CheckState) + "/" + this.trackBar1.Value;
                textBox5.Text = s;

                UpdatePrintBin(StartEnd, this.checkBox2.CheckState);

                if (ScreenShotValues.currentAddrSpaceNo == ScreenShotValues.initialAddrSpaceNo)
                {
                    this.Calculate(this.textBox2.Text.Trim());
                }
            }
            else // v4
            {
                listBox1.Items.Clear();
                this.label16.Text = "   ";

                this.Backwd.Enabled = false;
                this.Forwd.Enabled = false;
                this.Last.Enabled = false;

                StartEnd.End = StartEnd.Start - BigInteger.One;

                if (StartEnd.Start == BigInteger.Zero)
                {
                    BigInteger big2 = BigInteger.One + BigInteger.One;
                    StartEnd.End = BigInteger.Pow(big2, 32) - BigInteger.One;
                }

                StartEnd = v6ST.PrevSpace_v4(StartEnd);
                textBox4.Text = "#" + StartEnd.subnetidx.ToString();
                ScreenShotValues.currentAddrSpaceNo_v4 = StartEnd.subnetidx;

                subnets.Start = StartEnd.Start;
                subnets.End = 0;
                StartEnd.ResultIPAddr = StartEnd.Start;

                textBox3.Text = textBox5.Text = textBox7.Text = "";

                string s = String.Format("{0:x}", StartEnd.Start);
                textBox3.Text = v6ST.IPv4Format(s) + "/" + this.trackBar1.Value.ToString();

                textBox1.Text = textBox3.Text.Split('/')[0];

                textBox1.BackColor = Color.FromKnownColor(KnownColor.Info);

                s = String.Format("{0:x}", StartEnd.End);
                textBox5.Text = v6ST.IPv4Format(s) + "/" + this.trackBar1.Value.ToString();

                UpdatePrintBin_v4(StartEnd);

                if (ScreenShotValues.currentAddrSpaceNo_v4 == ScreenShotValues.initialAddrSpaceNo_v4)
                {
                    this.Calculate(this.textBox2.Text.Trim());
                }
            }
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

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox1.Visible = false;
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                listBox1.SetSelected(i, true);
            }
            listBox1.Visible = true;
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
            if (e.KeyCode == Keys.Enter)
            {
                if (ipmode == "v6")
                {
                    if ((this.checkBox2.CheckState == CheckState.Unchecked && this.trackBar2.Value == 64)
                        ||
                        (this.checkBox2.CheckState == CheckState.Checked && this.trackBar2.Value == 128)
                        ||
                        listBox1.SelectedItem == null
                        ||
                        (string)listBox1.SelectedItem == ""
                        )
                    {
                        return;
                    }
                }
                else // v4
                {
                    if (this.trackBar2.Value == 32
                        ||
                        listBox1.SelectedItem == null
                        ||
                        (string)listBox1.SelectedItem == ""
                        )
                    {
                        return;
                    }
                }

                ListSubnetRange lh = null;

                lh = new ListSubnetRange(this.StartEnd, listBox1.SelectedItem.ToString(),
                    this.trackBar1.Value, this.trackBar2.Value, this.checkBox2.CheckState, this.culture, MySQLconnection,
                    this.ServerInfo, ipmode, this.listBox1.Font);

                if (!lh.IsDisposed)
                {
                    lh.Show();
                    this.ChangeUILanguage += lh.SwitchLanguage;
                    this.changeDBstate += lh.DBStateChange;
                }
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (ipmode == "v6")
            {
                if ((this.checkBox2.CheckState == CheckState.Unchecked && this.trackBar2.Value == 64)
                    ||
                    (this.checkBox2.CheckState == CheckState.Checked && this.trackBar2.Value == 128)
                    ||
                    listBox1.SelectedItem == null
                    ||
                    (string)listBox1.SelectedItem == ""
                    )
                {
                    return;
                }
            }
            else // v4
            {
                if (this.trackBar2.Value == 32
                    ||
                    listBox1.SelectedItem == null
                    ||
                    (string)listBox1.SelectedItem == ""
                    )
                {
                    return;
                }
            }

            ListSubnetRange lh = null;


            lh = new ListSubnetRange(this.StartEnd, listBox1.SelectedItem.ToString(),
                this.trackBar1.Value, this.trackBar2.Value, this.checkBox2.CheckState, this.culture, MySQLconnection,
                this.ServerInfo, ipmode, this.listBox1.Font);


            if (!lh.IsDisposed)
            {
                lh.Show();
                //
                windowsList.Add(new WindowsList(lh, lh.Name, lh.GetHashCode(), ipmode));

                this.ChangeUILanguage += lh.SwitchLanguage;
                this.changeDBstate += lh.DBStateChange;
            }
        }

        private void listSubnetRangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((this.checkBox2.CheckState == CheckState.Unchecked && this.trackBar2.Value == 64)
                ||
                (this.checkBox2.CheckState == CheckState.Checked && this.trackBar2.Value == 128)
                ||
                listBox1.SelectedItem == null
                )
            {
                return;
            }
            else
            {
                ListSubnetRange lh = null;

                    lh = new ListSubnetRange(this.StartEnd, listBox1.SelectedItem.ToString(),
                        this.trackBar1.Value, this.trackBar2.Value, this.checkBox2.CheckState, this.culture, MySQLconnection,
                        this.ServerInfo, ipmode, this.listBox1.Font);
                

                if (!lh.IsDisposed)
                {
                    lh.Show();
                    //
                    windowsList.Add(new WindowsList(lh, lh.Name, lh.GetHashCode(), ipmode));

                    this.ChangeUILanguage += lh.SwitchLanguage;
                    this.changeDBstate += lh.DBStateChange;
                }
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

            if (this.checkBox3.CheckState == CheckState.Unchecked)
            {
                if (e.Index % 2 == 0)
                {
                    e.DrawBackground();
                    DrawItemState st = DrawItemState.Selected;

                    if ((e.State & st) != st)
                    {
                        // Turkuaz= FF40E0D0(ARGB)
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
                else
                {
                    e.DrawBackground();
                    g.FillRectangle(sback, e.Bounds);
                    g.DrawString(lb.Items[e.Index].ToString(), e.Font,
                        sfore, new PointF(e.Bounds.X, e.Bounds.Y));
                    e.DrawFocusRectangle();
                }
            }
            else if (this.checkBox3.CheckState == CheckState.Checked)
            {
                if (e.Index % 3 == 0)
                {
                    e.DrawBackground();
                    DrawItemState st = DrawItemState.Selected;

                    if ((e.State & st) != st)
                    {
                        // Turkuaz= FF40E0D0(ARGB)
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
                else
                {
                    e.DrawBackground();
                    g.FillRectangle(sback, e.Bounds);
                    g.DrawString(lb.Items[e.Index].ToString(), e.Font,
                        sfore, new PointF(e.Bounds.X, e.Bounds.Y));
                    e.DrawFocusRectangle();
                }
            }


            // Last item is the widest one due to its index but may not be depending on the font-width
            // e.g., '0' and 'f' does not have he same width if the font is not fixed-width.
            // PS: Try to use Fixed-width Font if available.

            lb.HorizontalScrollbar = true;
            int horzSize = (int)g.MeasureString(lb.Items[e.Index].ToString(), lb.Font).Width;
            if (horzSize > maxfontwidth)
            {
                maxfontwidth = horzSize;
                lb.HorizontalExtent = maxfontwidth;
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if ((this.trackBar2.Value - this.trackBar1.Value >= 0) && MySQLconnection != null)
                statsusagetoolStripMenuItem.Enabled = true;
            else
                statsusagetoolStripMenuItem.Enabled = false;

            if (this.listBox1.Items.Count > 0)
            {
                listDNSReverseToolStripMenuItem.Enabled = true;
                goToToolStripMenuItem1.Enabled = true;
                findprefixtoolStripMenuItem.Enabled = true;
                prefixsublevelstoolStripMenuItem1.Enabled = true;
                whoisQueryToolStripMenuItem1.Enabled = true;
                savetoolStripMenuItem1.Enabled = true;

                if (this.listBox1.SelectedItem != null && this.listBox1.SelectedItem.ToString() != ""
                    && this.listBox1.SelectedIndex != -1)
                {
                    workwithToolStripMenuItem.Enabled = true;

                    if (ipmode == "v6")
                    {
                        this.list32AddrToolStripMenuItem1.Enabled = false;
                        listSubnetRangeToolStripMenuItem.Enabled = true;

                        if ((this.trackBar2.Value == 64 && this.checkBox2.CheckState == CheckState.Unchecked)
                            ||
                            (this.trackBar2.Value == 128 && this.checkBox2.CheckState == CheckState.Checked)
                            )
                        {
                            listSubnetRangeToolStripMenuItem.Enabled = false;
                            list128SubnetsToolStripMenuItem.Enabled = false;
                        }
                        else
                        {
                            if (this.checkBox2.CheckState == CheckState.Unchecked)
                                listSubnetRangeToolStripMenuItem.Enabled = true;
                            else
                                listSubnetRangeToolStripMenuItem.Enabled = false;

                            if (this.checkBox2.CheckState == CheckState.Checked)
                                list128SubnetsToolStripMenuItem.Enabled = true;
                            else
                                list128SubnetsToolStripMenuItem.Enabled = false;

                        }
                    }
                    else // v4
                    {
                        this.list32AddrToolStripMenuItem1.Enabled = true;
                        listSubnetRangeToolStripMenuItem.Enabled = false;
                    }
                    
                    if (MySQLconnection != null)
                    {
                        sendToDatabaseToolStripMenuItem1.Enabled = true;
                        getPrefixInfoFromDBToolStripMenuItem.Enabled = true;
                        prefixsublevelstoolStripMenuItem1.Enabled = true;
                    }
                    else
                    {
                        sendToDatabaseToolStripMenuItem1.Enabled = false;
                        getPrefixInfoFromDBToolStripMenuItem.Enabled = false;
                        prefixsublevelstoolStripMenuItem1.Enabled = false;
                    }
                }
                else
                {
                    this.list32AddrToolStripMenuItem1.Enabled = false;
                    listSubnetRangeToolStripMenuItem.Enabled = false;
                    list128SubnetsToolStripMenuItem.Enabled = false;
                    workwithToolStripMenuItem.Enabled = false;
                    sendToDatabaseToolStripMenuItem1.Enabled = false;
                    getPrefixInfoFromDBToolStripMenuItem.Enabled = false;
                    prefixsublevelstoolStripMenuItem1.Enabled = false;
                }
            }
            else
            {
                this.list32AddrToolStripMenuItem1.Enabled = false;
                listSubnetRangeToolStripMenuItem.Enabled = false;
                list128SubnetsToolStripMenuItem.Enabled = false;
                listDNSReverseToolStripMenuItem.Enabled = false;
                workwithToolStripMenuItem.Enabled = false;
                sendToDatabaseToolStripMenuItem1.Enabled = false;
                getPrefixInfoFromDBToolStripMenuItem.Enabled = false;
                prefixsublevelstoolStripMenuItem1.Enabled = false;
                goToToolStripMenuItem1.Enabled = false;
                findprefixtoolStripMenuItem.Enabled = false;
                whoisQueryToolStripMenuItem1.Enabled = false;
                savetoolStripMenuItem1.Enabled = false;
            }
        }

        public BigInteger gotoaddrvalue
        {
            get { return BigInteger.Parse(textBox4.Text.TrimStart('#')); }
            set { textBox4.Text = "#" + value.ToString(); }
        }

        BigInteger gotoval = BigInteger.Zero;
        public BigInteger gotosubnetvalue
        {
            get { return gotoval; }
            set { gotoval = value; }
        }
        public string findprefix
        {
            get { return this.findpfx; }
            set { this.findpfx = value; }
        }

        private void GoToAddrSpaceNumber(BigInteger aspaceNo)
        {
            if (ipmode == "v6")
            {
                StartEnd.subnetidx = aspaceNo;

                StartEnd = v6ST.GoToAddrSpace(StartEnd, this.checkBox2.CheckState);
                StartEnd.ResultIPAddr = StartEnd.Start;

                listBox1.Items.Clear();
                this.label16.Text = "   ";

                textBox3.Text = textBox5.Text = textBox7.Text = "";

                string s = String.Format("{0:x}", StartEnd.Start);

                if (this.checkBox2.CheckState == CheckState.Checked)
                {
                    if (s.Length > 32)
                        s = s.Substring(1, 32);
                }
                if (this.checkBox2.CheckState == CheckState.Unchecked)
                {
                    if (s.Length > 16)
                        s = s.Substring(1, 16);
                }

                textBox1.Text = s = v6ST.Kolonlar(s, this.checkBox2.CheckState);
                s += "/" + this.trackBar1.Value;
                textBox3.Text = s;
                textBox1.Text = v6ST.FormalizeAddr(textBox1.Text);
                textBox1.Text = v6ST.Kolonlar(textBox1.Text, this.checkBox2.CheckState);

                textBox4.Text = "#" + aspaceNo.ToString();
                ScreenShotValues.currentAddrSpaceNo = aspaceNo;

                if (ScreenShotValues.currentAddrSpaceNo == ScreenShotValues.initialAddrSpaceNo)
                {
                    if (v6ST.IsAddressCorrect(this.textBox2.Text.Trim()))
                        this.Calculate(this.textBox2.Text.Trim());
                }
                else
                    textBox1.BackColor = Color.FromKnownColor(KnownColor.Info);

                s = String.Format("{0:x}", StartEnd.End);

                if (this.checkBox2.CheckState == CheckState.Checked)
                {
                    if (s.Length > 32)
                        s = s.Substring(1, 32);
                }
                if (this.checkBox2.CheckState == CheckState.Unchecked)
                {
                    if (s.Length > 16)
                        s = s.Substring(1, 16);
                }

                s = v6ST.Kolonlar(s, this.checkBox2.CheckState) + "/" + this.trackBar1.Value;
                textBox5.Text = s;

                UpdatePrintBin(StartEnd, this.checkBox2.CheckState);

                this.Forwd.Enabled = false;
                this.Backwd.Enabled = false;
                this.Last.Enabled = false;
            }
            else //v4
            {
                StartEnd.subnetidx = aspaceNo;

                StartEnd = v6ST.GoToAddrSpace_v4(StartEnd);
                StartEnd.ResultIPAddr = StartEnd.Start;

                listBox1.Items.Clear();
                this.label16.Text = "   ";

                textBox3.Text = textBox5.Text = textBox7.Text = "";

                string s = String.Format("{0:x}", StartEnd.Start);

                textBox3.Text = v6ST.IPv4Format(s) + "/" + this.trackBar1.Value; 
                textBox1.Text = textBox3.Text.Split('/')[0];

                textBox4.Text = "#" + aspaceNo.ToString();
                ScreenShotValues.currentAddrSpaceNo_v4 = aspaceNo;

                if (ScreenShotValues.currentAddrSpaceNo_v4 == ScreenShotValues.initialAddrSpaceNo_v4)
                {
                    if (v6ST.IsAddressCorrect_v4(this.textBox2.Text.Trim()))
                        this.Calculate(this.textBox2.Text.Trim());
                }
                else
                    textBox1.BackColor = Color.FromKnownColor(KnownColor.Info);

                s = String.Format("{0:x}", StartEnd.End);

                textBox5.Text = v6ST.IPv4Format(s) + "/" + this.trackBar1.Value;
                UpdatePrintBin_v4(StartEnd);

                this.Forwd.Enabled = false;
                this.Backwd.Enabled = false;
                this.Last.Enabled = false;
            }
        }
        private void goToAddrSpaceNumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string oldidx = this.textBox4.Text.TrimStart('#');

            Goto gasn = new Goto(this, goToToolStripMenuItem.DropDownItems.IndexOf(goToAddrSpaceNumberToolStripMenuItem),
                this.totmaxval, ID, this.culture, ipmode);

            gasn.ShowDialog();
            this.ChangeUILanguage += gasn.SwitchLanguage;

            string newidx = this.textBox4.Text.TrimStart('#');

            if (newidx == "" || newidx == oldidx)
                return;

            GoToAddrSpaceNumber(BigInteger.Parse(newidx));
        }

        private void goToSubnetNumberToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ipmode == "v6")
            {
                Goto gsn = new Goto(this, goToToolStripMenuItem.DropDownItems.IndexOf(goToSubnetNumberToolStripMenuItem1),
                    this.submaxval, ID, this.culture, ipmode);

                gsn.ShowDialog();
                this.ChangeUILanguage += gsn.SwitchLanguage;

                String ss = "", se = "";
                int count = 0;

                string newidx = gotoval.ToString();
                if (newidx == "")
                    return;

                subnets.subnetidx = BigInteger.Parse(newidx, NumberStyles.Number);
                subnets.slash = this.trackBar1.Value;
                subnets.subnetslash = this.trackBar2.Value;
                subnets.Start = StartEnd.Start;
                subnets.ResultIPAddr = StartEnd.ResultIPAddr;

                subnets = v6ST.GoToSubnet(subnets, this.checkBox2.CheckState);

                page.Start = subnets.Start;
                page.End = BigInteger.Zero;

                if (subnets.End.Equals(StartEnd.End))
                {
                    this.Forwd.Enabled = false;
                }

                this.listBox1.Items.Clear();
                this.label16.Text = "   ";

                for (count = 0; count < upto; count++)
                {
                    subnets = v6ST.Subnetting(subnets, this.checkBox2.CheckState);

                    if (this.checkBox2.CheckState == CheckState.Checked)
                    {
                        ss = String.Format("{0:x}", subnets.Start);
                        if (ss.Length > 32)
                            ss = ss.Substring(1, 32);
                        ss = v6ST.Kolonlar(ss, this.checkBox2.CheckState);
                        ss = v6ST.CompressAddress(ss);

                        se = String.Format("{0:x}", subnets.End);
                        if (se.Length > 32)
                            se = se.Substring(1, 32);
                        se = v6ST.Kolonlar(se, this.checkBox2.CheckState);
                        se = v6ST.CompressAddress(se);

                        //ss = "s" + subnets.subnetidx + "> " + ss + "/" +
                        ss = "p" + subnets.subnetidx + "> " + ss + "/" +
                            this.trackBar2.Value;
                        this.listBox1.Items.Add(ss);
                        /* No need to print two times if slash=128 since start=end */
                        if (this.trackBar2.Value != 128)
                        {
                            if (this.checkBox3.CheckState == CheckState.Checked)
                            {
                                se = "e" + subnets.subnetidx + "> " + se + "/"
                                    + this.trackBar2.Value;
                                this.listBox1.Items.Add(se);
                                this.listBox1.Items.Add("");
                            }
                        }
                    }
                    else if (this.checkBox2.CheckState == CheckState.Unchecked)
                    {
                        ss = String.Format("{0:x}", subnets.Start);
                        if (ss.Length > 16)
                            ss = ss.Substring(1, 16);
                        ss = v6ST.Kolonlar(ss, this.checkBox2.CheckState);
                        ss = v6ST.CompressAddress(ss);

                        se = String.Format("{0:x}", subnets.End);
                        if (se.Length > 16)
                            se = se.Substring(1, 16);
                        se = v6ST.Kolonlar(se, this.checkBox2.CheckState);
                        se = v6ST.CompressAddress(se);

                        //ss = "s" + subnets.subnetidx + "> " + ss + "/" +
                        ss = "p" + subnets.subnetidx + "> " + ss + "/" +
                            this.trackBar2.Value;
                        this.listBox1.Items.Add(ss);
                        /* No need to print two times if slash=64 since start=end */
                        if (this.trackBar2.Value != 64)
                        {
                            if (this.checkBox3.CheckState == CheckState.Checked)
                            {
                                se = "e" + subnets.subnetidx + "> " + se + "/"
                                    + this.trackBar2.Value;
                                this.listBox1.Items.Add(se);
                                this.listBox1.Items.Add("");
                            }
                        }
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

                if (BigInteger.Parse(newidx, NumberStyles.Number) == 0)
                {
                    this.Backwd.Enabled = false;
                }
                else
                    this.Backwd.Enabled = true;
                if (subnets.subnetidx == this.submaxval)
                {
                    this.Forwd.Enabled = false;
                    this.Last.Enabled = false;
                }
                else
                {
                    this.Forwd.Enabled = true;
                    this.Last.Enabled = true;
                }
                UpdateCount();
                UpdatePrintBin(StartEnd, this.checkBox2.CheckState);
            }
            else // v4
            {
                Goto gsn = new Goto(this, goToToolStripMenuItem.DropDownItems.IndexOf(goToSubnetNumberToolStripMenuItem1), 
                    this.submaxval, ID, this.culture, ipmode);

                gsn.ShowDialog();
                this.ChangeUILanguage += gsn.SwitchLanguage;

                String ss = "", se = "";
                int count = 0;

                string newidx = gotoval.ToString();
                if (newidx == "")
                    return;

                subnets.subnetidx = BigInteger.Parse(newidx, NumberStyles.Number);
                subnets.slash = this.trackBar1.Value;
                subnets.subnetslash = this.trackBar2.Value;
                subnets.Start = StartEnd.Start;
                subnets.ResultIPAddr = StartEnd.ResultIPAddr;

                subnets = v6ST.GoToSubnet_v4(subnets);

                page.Start = subnets.Start;
                page.End = 0;

                if (subnets.End.Equals(StartEnd.End))
                {
                    this.Forwd.Enabled = false;
                }

                this.listBox1.Items.Clear();
                this.label16.Text = "   ";

                for (count = 0; count < upto; count++)
                {
                    subnets = v6ST.Subnetting_v4(subnets);
                    ss = String.Format("{0:x}", subnets.Start);
                    
                    ss = v6ST.IPv4Format(ss);
                    ss = "p" + subnets.subnetidx + "> " + ss + "/" + this.trackBar2.Value;

                    if (this.trackBar2.Value == 32)
                    {
                        this.listBox1.Items.Add(ss);
                    }
                    else //Value < 32
                    {
                        this.listBox1.Items.Add(ss);

                        if (this.checkBox3.CheckState == CheckState.Checked)
                        {
                            se = String.Format("{0:x}", subnets.End);
                            
                            se = v6ST.IPv4Format(se);
                            se = "e" + subnets.subnetidx + "> " + se + "/" + this.trackBar2.Value;

                            this.listBox1.Items.Add(se);
                            this.listBox1.Items.Add("");
                        }
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

                if (BigInteger.Parse(newidx, NumberStyles.Number) == 0)
                {
                    this.Backwd.Enabled = false;
                }
                else
                    this.Backwd.Enabled = true;
                if (subnets.subnetidx == this.submaxval)
                {
                    this.Forwd.Enabled = false;
                    this.Last.Enabled = false;
                }
                else
                {
                    this.Forwd.Enabled = true;
                    this.Last.Enabled = true;
                }
                UpdateCount();
                UpdatePrintBin_v4(StartEnd);
            }
        }

        private void listDNSReverseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listAllDNSReverseZonesToolStripMenuItem_Click(null, null);
        }

        private void listAllDNSReverseZonesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listBox1.Items.Count > 0)
            {
                StartEnd.slash = this.trackBar1.Value;
                StartEnd.subnetslash = this.trackBar2.Value;
                ListDnsReverses dnsr = new ListDnsReverses(StartEnd, this.checkBox2.CheckState, this.culture, ipmode, this.listBox1.Font);
                dnsr.Show();
                //
                windowsList.Add(new WindowsList(dnsr, dnsr.Name, dnsr.GetHashCode(), ipmode));

                this.ChangeUILanguage += dnsr.SwitchLanguage;
            }
        }

        private void list32AddrToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.listBox1_MouseDoubleClick(null, null);
        }

        private void list32AddrToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.listBox1_MouseDoubleClick(null, null);
        }

        private void list64SubnetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((this.checkBox2.CheckState == CheckState.Unchecked && this.trackBar2.Value == 64)
                ||
                (this.checkBox2.CheckState == CheckState.Checked && this.trackBar2.Value == 128)
                ||
                listBox1.SelectedItem == null
                )
            {
                return;
            }
            else
            {
                ListSubnetRange lh = null;

                lh = new ListSubnetRange(this.StartEnd, listBox1.SelectedItem.ToString(),
                    this.trackBar1.Value, this.trackBar2.Value, this.checkBox2.CheckState, this.culture, MySQLconnection,
                    this.ServerInfo, ipmode, this.listBox1.Font);


                if (!lh.IsDisposed)
                {
                    lh.Show();
                    //
                    windowsList.Add(new WindowsList(lh, lh.Name, lh.GetHashCode(), ipmode));

                    this.ChangeUILanguage += lh.SwitchLanguage;
                    this.changeDBstate += lh.DBStateChange;
                }
            }
        }

        private void goToToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (this.listBox1.Items.Count > 0)
            {
                this.goToSubnetNumberToolStripMenuItem1.Enabled = true;
                this.findprefixtoolStripMenuItem1.Enabled = true;
            }
            else
            {
                this.goToSubnetNumberToolStripMenuItem1.Enabled = false;
                this.findprefixtoolStripMenuItem1.Enabled = false;
            }
        }

        private void toolsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (ipmode == "v4")
                this.AddressTypeInfotoolStripMenuItem.Enabled = false;
            else
                this.AddressTypeInfotoolStripMenuItem.Enabled = true;

            if (this.listBox1.Items.Count > 0)
            {
                this.listAllDNSReverseZonesToolStripMenuItem.Enabled = true;

                if (this.listBox1.SelectedItem != null && this.listBox1.SelectedItem.ToString() != ""
                    && this.listBox1.SelectedIndex != -1)
                {
                    this.workwithtoolStripMenuItem1.Enabled = true;

                    if (ipmode == "v6")
                    {
                        this.list32AddrToolStripMenuItem.Enabled = false;

                        if ((this.trackBar2.Value == 64 && this.checkBox2.CheckState == CheckState.Unchecked)
                            ||
                            (this.trackBar2.Value == 128 && this.checkBox2.CheckState == CheckState.Checked)
                            )
                        {
                            this.list64SubnetsToolStripMenuItem.Enabled = false;
                            this.list128SubnetsToolStripMenuItem1.Enabled = false;
                        }
                        else
                        {
                            if (this.checkBox2.CheckState == CheckState.Unchecked)
                                this.list64SubnetsToolStripMenuItem.Enabled = true;
                            else
                                this.list64SubnetsToolStripMenuItem.Enabled = false;

                            if (this.checkBox2.CheckState == CheckState.Checked)
                                this.list128SubnetsToolStripMenuItem1.Enabled = true;
                            else
                                this.list128SubnetsToolStripMenuItem1.Enabled = false;
                        }
                    }
                    else //v4
                    {
                        this.list32AddrToolStripMenuItem.Enabled = true;
                    }
                }
                else
                {
                    this.list32AddrToolStripMenuItem.Enabled = false;
                    this.list64SubnetsToolStripMenuItem.Enabled = false;
                    this.list128SubnetsToolStripMenuItem1.Enabled = false;
                    this.workwithtoolStripMenuItem1.Enabled = false;
                }
            }
            else
            {
                this.list32AddrToolStripMenuItem.Enabled = false;
                this.list64SubnetsToolStripMenuItem.Enabled = false;
                this.list128SubnetsToolStripMenuItem1.Enabled = false;
                this.listAllDNSReverseZonesToolStripMenuItem.Enabled = false;
                this.workwithtoolStripMenuItem1.Enabled = false;
            }
        }

        private void checkBox2_Click(object sender, EventArgs e)
        {
            // 128bits checkBox. Only for v6: 
            if (this.checkBox2.Checked)
            {
                if (ipmode == "v6")
                    ScreenShotValues.is128Checked = true;
            }
            else
            {
                if (ipmode == "v6")
                    ScreenShotValues.is128Checked = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox2.CheckState == CheckState.Unchecked)
            {
                this.DefaultView();
            }
            else if (this.checkBox2.CheckState == CheckState.Checked)
            {
                this.ExpandView();
            }

            textBox2.Text = textBox2.Text.Trim().ToLower();
            textBox3.Text = textBox5.Text = richTextBox1.Text = textBox7.Text = "";

            listBox1.Items.Clear();
            this.label16.Text = "   ";
            this.Form1_Paint(null, null);

            Calculate(textBox2.Text);
        }

        private void ExpandView()
        {
            this.Size = new Size(847, 537);
            this.textBox3.Size = new Size(365, 20);

            this.PrevSpace.Location = new Point(437, 92);
            this.NextSpace.Location = new Point(473, 92);

            this.PrevSpace.Size = new Size(30, 23);
            this.NextSpace.Size = new Size(30, 23);

            this.label14.Location = new Point(508, 101);

            this.textBox5.Size = new Size(365, 20);
            this.textBox4.Location = new Point(437, 117);
            this.textBox4.Size = new Size(347, 20);

            this.textBox6.Size = new Size(719, 20);
            this.richTextBox1.Size = new Size(710, 13);

            this.trackBar1.Size = new Size(730, 30);
            this.trackBar2.Size = new Size(730, 30);
            this.trackBar1.Maximum = 128;
            this.trackBar2.Maximum = 128;
            //
            this.label2.Location = new Point(790, 176);
            this.label1.Location = new Point(800, 176);
            this.label9.Location = new Point(790, 210);
            this.label10.Location = new Point(800, 210);
            this.label17.Location = new Point(797, 215);
            this.label11.Location = new Point(780, 230);
            this.label12.Location = new Point(800, 234);
            //
            this.textBox7.Location = new Point(786, 278);
            this.textBox7.Size = new Size(30, 13);
            this.textBox8.Size = new Size(365, 20);
            this.listBox1.Size = new Size(719, 184);
            this.label5.Location = new Point(545, 463);
            this.toolStripStatusLabel1.Size = new Size(687, 19);
            //
            graph.Clear(Form1.DefaultBackColor);
        }

        private void DefaultView()
        {
            this.Size = new Size(487, 537);
            this.textBox3.Size = new Size(197, 20);

            this.PrevSpace.Location = new Point(279, 92);
            this.PrevSpace.Size = new Size(30, 23);
            this.NextSpace.Location = new Point(315, 92);
            this.NextSpace.Size = new Size(30, 23);

            this.label14.Location = new Point(349, 101);

            this.textBox5.Size = new Size(197, 20);
            this.textBox4.Location = new Point(279, 117);
            this.textBox4.Size = new Size(152, 20);

            this.textBox6.Size = new Size(366, 20);
            this.richTextBox1.Size = new Size(357, 13);

            if (ipmode == "v6")
            {
                this.trackBar1.Size = new Size(376, 30);
                this.trackBar2.Size = new Size(376, 30);
                this.trackBar1.Maximum = 64;
                this.trackBar2.Maximum = 64;

                if (this.checkBox2.Checked)
                    this.textBox8.Size = new Size(365, 20);
                else
                    this.textBox8.Size = new Size(197, 20);
                //
                this.label2.Location = new Point(435, 176);
                this.label1.Location = new Point(445, 176);
                this.label1.Text = this.trackBar1.Value.ToString();
                this.label9.Location = new Point(435, 210);
                this.label10.Location = new Point(445, 210);
                this.label10.Text = this.trackBar2.Value.ToString();
                this.label17.Location = new Point(436, 215);
                this.label11.Location = new Point(425, 230);
                this.label12.Location = new Point(445, 234);
            }
            else //v4
            {
                this.trackBar1.Maximum = 32;
                this.trackBar2.Maximum = 32;
                this.trackBar1.Width = 202;
                this.trackBar2.Width = 202;
                this.textBox8.Size = new Size(197, 20);

                this.textBox6.Size = new Size(197, 20);
                this.richTextBox1.Size = new Size(190, 13);
                this.textBox2.Text = "";
                //
                this.label2.Location = new Point(274, 176);
                this.label1.Location = new Point(284, 176);
                this.label1.Text = this.trackBar1.Value.ToString();
                this.label9.Location = new Point(274, 210);
                this.label10.Location = new Point(284, 210);
                this.label10.Text = this.trackBar2.Value.ToString();
                this.label17.Location = new Point(275, 215);
                this.label11.Location = new Point(264, 230);
                this.label12.Location = new Point(284, 234);
            }

            this.textBox7.Location = new Point(434, 278);
            this.textBox7.Size = new Size(30, 13);
            this.listBox1.Size = new Size(367, 184);
            this.label5.Location = new Point(193, 463);
            this.toolStripStatusLabel1.Size = new Size(370, 19);
            //
            graph.Clear(Form1.DefaultBackColor);
        }

        private void list128SubnetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((this.checkBox2.CheckState == CheckState.Unchecked && this.trackBar2.Value == 64)
                ||
                (this.checkBox2.CheckState == CheckState.Checked && this.trackBar2.Value == 128)
                ||
                listBox1.SelectedItem == null
                )
            {
                return;
            }
            else
            {
                ListSubnetRange lh = null;

                    lh = new ListSubnetRange(this.StartEnd, listBox1.SelectedItem.ToString(),
                        this.trackBar1.Value, this.trackBar2.Value, this.checkBox2.CheckState, this.culture, MySQLconnection,
                        this.ServerInfo, ipmode, this.listBox1.Font);
                

                if (!lh.IsDisposed)
                {
                    lh.Show();
                    //
                    windowsList.Add(new WindowsList(lh, lh.Name, lh.GetHashCode(), ipmode));

                    this.ChangeUILanguage += lh.SwitchLanguage;
                    this.changeDBstate += lh.DBStateChange;
                }
            }
        }

        private void whoisQueryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listBox1.Items.Count > 0)
            {
                if (this.listBox1.SelectedItem != null && this.listBox1.SelectedItem.ToString() != ""
                    && this.listBox1.SelectedIndex != -1)
                {
                    try
                    {
                        string s = (listBox1.SelectedItem.ToString().Split(' ')[1]);

                        whoisQuery whoisquery = new whoisQuery(s, this.culture);
                        whoisquery.Show();
                        //
                        windowsList.Add(new WindowsList(whoisquery, whoisquery.Name, whoisquery.GetHashCode(), ipmode));

                        this.ChangeUILanguage += whoisquery.SwitchLanguage;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    whoisQuery whoisquery = new whoisQuery(v6ST.CompressAddress(this.textBox1.Text), this.culture);
                    whoisquery.Show();
                    //
                    windowsList.Add(new WindowsList(whoisquery, whoisquery.Name, whoisquery.GetHashCode(), ipmode));

                    this.ChangeUILanguage += whoisquery.SwitchLanguage;
                }
            }
            else
            {
                whoisQuery whoisquery = new whoisQuery(v6ST.CompressAddress(this.textBox1.Text), this.culture);
                whoisquery.Show();
                //
                windowsList.Add(new WindowsList(whoisquery, whoisquery.Name, whoisquery.GetHashCode(), ipmode));

                this.ChangeUILanguage += whoisquery.SwitchLanguage;
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (MessageBox.Show(StringsDictionary.KeyValue("Form1_KeyDown_msg", this.culture),
                    StringsDictionary.KeyValue("Form1_KeyDown_header", this.culture),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                {
                    if (MySQLconnection != null)
                    {
                        MySQLconnection.Close();

                        if (MySQLconnection is IDisposable)
                            MySQLconnection.Dispose();
                    }

                    Application.Exit();
                }
                else
                    return;
            }
            else if ((e.Control && e.KeyCode == Keys.F) || (e.KeyCode == Keys.F3))
            {
                if (this.listBox1.Items.Count > 0)
                {
                    this.findprefixtoolStripMenuItem1_Click(null, null);
                }
                else
                    return;
            }
            else if (e.KeyCode == Keys.F5)
            {
                this.whoisQueryToolStripMenuItem_Click(null, null);
            }
            else if (e.KeyCode == Keys.F6)
            {
                if (ipmode == "v6")
                    AddressTypeInfotoolStripMenuItem_Click(null, null);
            }
        }

        private void whoisQueryToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.whoisQueryToolStripMenuItem_Click(null, null);
        }

        private void fontsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (this.fontDialog1)
            {
                try
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));

                    DialogResult resfont = this.fontDialog1.ShowDialog();

                    if (resfont == System.Windows.Forms.DialogResult.OK)
                    {
                        // listBox1:
                        this.listBox1.Font = fontDialog1.Font;
                        ScreenShotValues.Form1_listBox1Font = converter.ConvertToString(this.listBox1.Font);

                        this.listBox1.ItemHeight = this.listBox1.Font.Height;
                        this.maxfontwidth = 0;
                        this.listBox1.HorizontalExtent = 0;

                        // textbox2:InputAddress
                        this.textBox2.Font = fontDialog1.Font;
                        ScreenShotValues.textBox2Font = converter.ConvertToString(this.textBox2.Font);

                        // textbox1:IPv6/v4 Address
                        this.textBox1.Font = fontDialog1.Font;
                        ScreenShotValues.textBox1Font = converter.ConvertToString(this.textBox1.Font);

                        // textbox4:AddrSpaceNo
                        this.textBox4.Font = fontDialog1.Font;
                        ScreenShotValues.textBox4Font = converter.ConvertToString(this.textBox4.Font);

                        // textbox3:Start
                        this.textBox3.Font = fontDialog1.Font;
                        ScreenShotValues.textBox3Font = converter.ConvertToString(this.textBox3.Font);

                        // textbox5:End
                        this.textBox5.Font = fontDialog1.Font;
                        ScreenShotValues.textBox5Font = converter.ConvertToString(this.textBox5.Font);

                        // textbox8:Mask
                        this.textBox8.Font = fontDialog1.Font;
                        ScreenShotValues.textBox8Font = converter.ConvertToString(this.textBox8.Font);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exception: " + ex.Message, "Font Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void goToToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.goToSubnetNumberToolStripMenuItem1_Click(null, null);
        }

        private void exportToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsText exptofile = new SaveAsText(this.StartEnd, this.checkBox2.CheckState, this.culture);
            exptofile.Show();
            //
            windowsList.Add(new WindowsList(exptofile, exptofile.Name, exptofile.GetHashCode(), ipmode));

            this.ChangeUILanguage += exptofile.SwitchLanguage;
        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (this.listBox1.Items.Count < 0 || this.listBox1.Items.Count == 0)
            {
                this.exportToFileToolStripMenuItem.Enabled = false;
            }
            else
            {
                this.exportToFileToolStripMenuItem.Enabled = true;
            }
        }

        private void savetoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveAsText saveas = new SaveAsText(this.StartEnd, this.checkBox2.CheckState, this.culture);
            saveas.Show();
            //
            windowsList.Add(new WindowsList(saveas, saveas.Name, saveas.GetHashCode(), ipmode));

            this.ChangeUILanguage += saveas.SwitchLanguage;
        }

        private void textBox1_MouseEnter(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim() != "" && ipmode == "v6")
            {
                string addrname = "";

                AttributeValues attribs =
                    v6ST.AddressType(BigInteger.Parse("0" + this.textBox1.Text.Replace(":", ""), NumberStyles.AllowHexSpecifier),
                    this.trackBar1.Value, this.checkBox2.CheckState);

                if (attribs.Name == "Global Unicast")
                {
                    addrname = "Name: " + attribs.Name + Environment.NewLine;
                    addrname += "SelectedPrefixLength: " + this.trackBar1.Value.ToString() + Environment.NewLine;
                    addrname += "RFC: " + attribs.RFC;
                }
                else if (attribs.isMulticast)
                {
                    addrname = "Name: " + attribs.Name + Environment.NewLine;
                    addrname += "Address Block:  " + attribs.strAddressBlock + Environment.NewLine;
                    addrname += "AssignedPrefixLength: " + attribs.AssignedPrefixLength.ToString() + Environment.NewLine;
                    addrname += "SelectedPrefixLength: " + this.trackBar1.Value.ToString() + Environment.NewLine;
                    addrname += "RFC: " + attribs.RFC + Environment.NewLine;
                    addrname += "Allocation Date: " + attribs.AllocationDate + Environment.NewLine;
                    addrname += "Termination Date: " + attribs.TerminationDate;
                }
                else
                {
                    addrname = "Name: " + attribs.Name + Environment.NewLine;
                    addrname += "Address Block:  " + attribs.strAddressBlock + Environment.NewLine;
                    addrname += "AssignedPrefixLength: " + attribs.AssignedPrefixLength.ToString() + Environment.NewLine;
                    addrname += "SelectedPrefixLength: " + this.trackBar1.Value.ToString() + Environment.NewLine;
                    addrname += "RFC: " + attribs.RFC + Environment.NewLine;
                    addrname += "Allocation Date: " + attribs.AllocationDate + Environment.NewLine;
                    addrname += "Termination Date: " + attribs.TerminationDate + Environment.NewLine;
                    addrname += "Source: " + attribs.Source + Environment.NewLine;
                    addrname += "Destination: " + attribs.Destination + Environment.NewLine;
                    addrname += "Forwardable: " + attribs.Forwardable + Environment.NewLine;
                    addrname += "Global: " + attribs.Global + Environment.NewLine;
                    addrname += "Reserved-by-Protocol: " + attribs.ReservedByProtocol;
                }
                this.toolTip1.SetToolTip(this.textBox1, addrname);
            }
            else
                this.toolTip1.SetToolTip(this.textBox1, "");
        }

        private void EnglishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.UncheckAllLang((ToolStripMenuItem)sender);
        }

        private void TurkishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.UncheckAllLang((ToolStripMenuItem)sender);
        }

        private void GermanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.UncheckAllLang((ToolStripMenuItem)sender);
        }

        private void UncheckAllLang(ToolStripMenuItem selected)
        {
            selected.Checked = true;

            foreach (var MenuItem in
                (from object item in selected.Owner.Items
                 let MenuItem = item as ToolStripMenuItem
                 where !item.Equals(selected)
                 where MenuItem != null
                 select MenuItem))
                MenuItem.Checked = false;

            this.SwitchLanguage();
        }

        private void SwitchLanguage()
        {
            if (this.EnglishToolStripMenuItem.Checked == true)
            {
                this.culture = Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
            }
            else if (this.TurkishToolStripMenuItem.Checked == true)
            {
                this.culture = Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("tr-TR");
            }
            else if (this.GermanToolStripMenuItem.Checked == true)
            {
                this.culture = Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de-DE");
            }

            if (ipmode == "v6")
            {
                v6ST.IsAddressCorrect(this.textBox2.Text);
            }
            else // v4
            {
                v6ST.IsAddressCorrect_v4(this.textBox2.Text);
            }
            //
            this.languageToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_languageToolStripMenuItem", this.culture);
            this.TurkishToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_TurkishToolStripMenuItem", this.culture);
            this.EnglishToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_EnglishToolStripMenuItem", this.culture);
            this.GermanToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_GermanToolStripMenuItem", this.culture);
            //
            this.Text = StringsDictionary.KeyValue("Form1_Text", this.culture);
            this.aboutToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_aboutToolStripMenuItem.Text", this.culture);
            this.Backwd.Text = StringsDictionary.KeyValue("Form1_Backwd.Text", this.culture);
            this.checkBox1.Text = StringsDictionary.KeyValue("Form1_checkBox1.Text", this.culture);
            this.checkBox2.Text = StringsDictionary.KeyValue("Form1_checkBox2.Text", this.culture);
            this.checkBox3.Text = StringsDictionary.KeyValue("Form1_checkBox3.Text", this.culture);
            this.copyToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_copyToolStripMenuItem.Text", this.culture);
            this.databasetoolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_databasetoolStripMenuItem.Text", this.culture);
            this.connectDBtoolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_connectDBtoolStripMenuItem.Text", this.culture);
            this.closeDBtoolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_closeDBtoolStripMenuItem.Text", this.culture);
            this.statusofDBtoolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_statusofDBtoolStripMenuItem.Text", this.culture);
            this.changeDatabasetoolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_changeDatabasetoolStripMenuItem.Text", this.culture);
            this.opendbformtoolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_opendbformtoolStripMenuItem.Text", this.culture);
            this.sendtoDBtoolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_sendtoDBtoolStripMenuItem.Text", this.culture);
            this.getPrefixInfoFromDatabaseToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_getPrefixInfoFromDB.Text", this.culture);
            this.getPrefixInfoFromDBToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_getPrefixInfoFromDB.Text", this.culture);
            this.exitToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_exitToolStripMenuItem.Text", this.culture);
            this.exportToFileToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_exportToFileToolStripMenuItem.Text", this.culture);
            this.fileToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_fileToolStripMenuItem.Text", this.culture);
            this.Find.Text = StringsDictionary.KeyValue("Form1_Find.Text", this.culture);
            this.fontsToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_fontsToolStripMenuItem.Text", this.culture);
            this.sendToDatabaseToolStripMenuItem1.Text = StringsDictionary.KeyValue("Form1_sendToDatabaseToolStripMenuItem1.Text", this.culture);
            this.Forwd.Text = StringsDictionary.KeyValue("Form1_Forwd.Text", this.culture);
            this.goToAddrSpaceNumberToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_goToAddrSpaceNumberToolStripMenuItem.Text", this.culture);
            this.goToSubnetNumberToolStripMenuItem1.Text = StringsDictionary.KeyValue("Form1_goToSubnetNumberToolStripMenuItem1.Text", this.culture);
            this.goToToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_goToToolStripMenuItem.Text", this.culture);
            this.goToToolStripMenuItem1.Text = StringsDictionary.KeyValue("Form1_goToToolStripMenuItem1.Text", this.culture);
            this.helpToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_helpToolStripMenuItem.Text", this.culture);
            this.label13.Text = StringsDictionary.KeyValue("Form1_label13.Text", this.culture);
            this.label14.Text = StringsDictionary.KeyValue("Form1_label14.Text", this.culture);
            this.label5.Text = StringsDictionary.KeyValue("Form1_label5.Text", this.culture);
            this.label3.Text = StringsDictionary.KeyValue("Form1_label3.Text", this.culture);
            this.label4.Text = StringsDictionary.KeyValue("Form1_label4.Text", this.culture);
            this.textBox9.Text = StringsDictionary.KeyValue("Form1_textBox9.Text", this.culture);
            this.label6.Text = StringsDictionary.KeyValue("Form1_label6.Text", this.culture);
            this.label7.Text = StringsDictionary.KeyValue("Form1_label7.Text", this.culture);
            this.label9.Text = StringsDictionary.KeyValue("Form1_label9.Text", this.culture);
            this.Last.Text = StringsDictionary.KeyValue("Form1_Last.Text", this.culture);
            this.list128SubnetsToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_list128SubnetsToolStripMenuItem.Text", this.culture);
            this.list128SubnetsToolStripMenuItem1.Text = StringsDictionary.KeyValue("Form1_list128SubnetsToolStripMenuItem1.Text", this.culture);
            this.list64SubnetsToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_list64SubnetsToolStripMenuItem.Text", this.culture);
            this.list32AddrToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_list32AddrToolStripMenuItem.Text", this.culture);
            this.list32AddrToolStripMenuItem1.Text = StringsDictionary.KeyValue("Form1_list32AddrToolStripMenuItem.Text", this.culture);
            this.listAllDNSReverseZonesToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_listAllDNSReverseZonesToolStripMenuItem.Text", this.culture);
            this.listDNSReverseToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_listDNSReverseToolStripMenuItem.Text", this.culture);
            this.listSubnetRangeToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_listSubnetRangeToolStripMenuItem.Text", this.culture);
            this.menuStrip1.Text = StringsDictionary.KeyValue("Form1_menuStrip1.Text", this.culture);
            this.ModetoolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_ModetoolStripMenuItem.Text", this.culture);
            this.NextSpace.Text = StringsDictionary.KeyValue("Form1_NextSpace.Text", this.culture);
            this.PrevSpace.Text = StringsDictionary.KeyValue("Form1_PrevSpace.Text", this.culture);
            this.ResetAll.Text = StringsDictionary.KeyValue("Form1_ResetAll.Text", this.culture);
            this.savetoolStripMenuItem1.Text = StringsDictionary.KeyValue("Form1_savetoolStripMenuItem1.Text", this.culture);
            this.selectAllToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_selectAllToolStripMenuItem.Text", this.culture);
            this.statusStrip1.Text = StringsDictionary.KeyValue("Form1_statusStrip1.Text", this.culture);
            this.Subnets.Text = StringsDictionary.KeyValue("Form1_Subnets.Text", this.culture);
            this.toolsToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_toolsToolStripMenuItem.Text", this.culture);
            this.whoisQueryToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_whoisQueryToolStripMenuItem.Text", this.culture);
            this.whoisQueryToolStripMenuItem1.Text = StringsDictionary.KeyValue("Form1_whoisQueryToolStripMenuItem1.Text", this.culture);
            this.AddressTypeInfotoolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_AddressTypeInfotoolStripMenuItem.Text", this.culture);
            this.workwithToolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_workwithToolStripMenuItem.Text", this.culture);
            this.workwithtoolStripMenuItem1.Text = StringsDictionary.KeyValue("Form1_workwithToolStripMenuItem.Text", this.culture);
            this.sublevelstoolStripMenuItem1.Text = StringsDictionary.KeyValue("Form1_sublevelstoolStripMenuItem1.Text", this.culture);
            this.prefixsublevelstoolStripMenuItem1.Text = StringsDictionary.KeyValue("Form1_prefixsublevelstoolStripMenuItem1.Text", this.culture);
            this.compressaddrtoolStripMenuItem1.Text = StringsDictionary.KeyValue("Form1_compressaddrtoolStripMenuItem1.Text", this.culture);
            this.findprefixtoolStripMenuItem1.Text = StringsDictionary.KeyValue("Form1_findprefixtoolStripMenuItem1.Text", this.culture);
            this.findprefixtoolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_findprefixtoolStripMenuItem1.Text", this.culture);
            this.statsusagetoolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_statsusagetoolStripMenuItem.Text", this.culture);
            this.statstoolStripMenuItem1.Text = StringsDictionary.KeyValue("Form1_statstoolStripMenuItem1.Text", this.culture);
            this.ASnumberToolStripMenuItem.Text = StringsDictionary.KeyValue("ASNumberPlainDotForm_ASnumberToolStripMenuItem.Text", this.culture);
            this.windowstoolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_windowstoolStripMenuItem.Text", this.culture);
            this.closeAlltoolStripMenuItem.Text = StringsDictionary.KeyValue("Form1_closeAlltoolStripMenuItem.Text", this.culture);

            // ToolTips
            this.toolTip1.SetToolTip(this.Backwd, StringsDictionary.KeyValue("Form1_Backwd.ToolTip", this.culture));
            this.toolTip1.SetToolTip(this.checkBox1, StringsDictionary.KeyValue("Form1_checkBox1.ToolTip", this.culture));
            this.toolTip1.SetToolTip(this.checkBox2, StringsDictionary.KeyValue("Form1_checkBox2.ToolTip", this.culture));
            this.toolTip1.SetToolTip(this.checkBox3, StringsDictionary.KeyValue("Form1_checkBox3.ToolTip", this.culture));
            this.toolTip1.SetToolTip(this.Forwd, StringsDictionary.KeyValue("Form1_Forwd.ToolTip", this.culture));
            this.toolTip1.SetToolTip(this.Last, StringsDictionary.KeyValue("Form1_Last.ToolTip", this.culture));
            this.toolTip1.SetToolTip(this.Subnets, StringsDictionary.KeyValue("Form1_Subnets.ToolTip", this.culture));
            this.toolTip1.SetToolTip(this.listBox1, StringsDictionary.KeyValue("Form1_listBox1.ToolTip", this.culture));

            // errmsg from checks. 
            if (v6ST.errmsg != "")
                textBox9.Text = StringsDictionary.KeyValue("Form1_" + v6ST.errmsg, this.culture);
            else
                textBox9.Text = StringsDictionary.KeyValue("Form1_textBox2_Enter", this.culture);

            //status bar
            toolStripStatusLabel1.Text = StringsDictionary.KeyValue("Form1_UpdateStatus_delta", this.culture)
                + delta + StringsDictionary.KeyValue("Form1_UpdateStatus_subnets", this.culture)
                + submax.ToString() + StringsDictionary.KeyValue("Form1_UpdateStatus_addrs", this.culture)
                + stotaddr + "]";

            //# of entries
            if (this.textBox7.Text != "")
                this.textBox7.Text = "[" + this.updatecount.ToString()
                    + StringsDictionary.KeyValue("Form1_UpdateCount_entries", culture);
            //
            this.ChangeUILanguage.Invoke(this.culture);
            //
            ScreenShotValues.Cultur = this.culture;
        }

        private void workwithToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WorkWithPrefix();
        }

        private void workwithtoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            WorkWithPrefix();
        }

        private void WorkWithPrefix()
        {
            string selected = this.listBox1.SelectedItem.ToString().Split(' ')[1];
            string snet = selected.Split('/')[0];
            int plen = Convert.ToInt16(selected.Split('/')[1]);
            this.textBox2.Text = snet;

            this.trackBar1.Value = this.trackBar2.Value;
            this.label1.Text = this.trackBar1.Value.ToString();

            Backwd.Enabled = Forwd.Enabled = Last.Enabled = false;
            listBox1.Items.Clear();
            this.label16.Text = "   ";
            textBox7.Text = "";

            Calculate(snet);

            StartEnd.slash = StartEnd.subnetslash = trackBar1.Value;
        }

        private void sendToDatabaseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex == -1)
                return;
            else
                this.opendbformtoolStripMenuItem_Click(null, null);
        }

        void MySQLconnection_StateChange(object sender, StateChangeEventArgs e)
        {
            // i =  0 Reserved refresh?
            // i = -1 CLOSE/down
            // i =  1 OPEN/up

            int i = 0;

            if (e.CurrentState == ConnectionState.Closed)
            {
                this.toolStripStatusLabel2.Text = "db=Down";
                i = -1;
            }
            if (e.CurrentState == ConnectionState.Open)
            {
                this.toolStripStatusLabel2.Text = "db=Up";
                i = 1;
            }

            this.changeDBstate.Invoke(MySQLconnection, i);
        }

        private void connectDBtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MySQLconnection != null && MySQLconnection.State == ConnectionState.Open)
            {
                MessageBox.Show(StringsDictionary.KeyValue("Form1_connectDBtoolStripMenuItem", this.culture)
                    + Environment.NewLine + "Status: " + MySQLconnection.State,
                    "DB Conn.:", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!OpenMySQLconnection(ipmode))
            {
                return;
            }
            else
            {
                this.toolStripStatusLabel2.Text = "db=Up";

                if (this.ServerInfo.launchDBUI)
                    this.opendbformtoolStripMenuItem_Click(null, null);

                MessageBox.Show(StringsDictionary.KeyValue("Form1_ConnectToDatabase", this.culture),
                    StringsDictionary.KeyValue("Form1_ConnectToDatabase_header", this.culture),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                MySQLconnection.StateChange += new StateChangeEventHandler(MySQLconnection_StateChange);

                this.changeDBstate.Invoke(MySQLconnection, 1);
            }
        }

        private void closeDBtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.CloseMySQLconnection();
        }

        private void statusofDBtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (MySQLconnection == null)
            //{
            //    MessageBox.Show(StringsDictionary.KeyValue("Form1_closeDBtoolStripMenuItem_Click_noDB", this.culture),
            //        StringsDictionary.KeyValue("Form1_closeDBtoolStripMenuItem_Click_noDB_header", this.culture),
            //        MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            string state = "null", dbase = "null", dbase_v6 = "null", dbase_v4 = "null",
                dbsource = "null", driver = "null", ver = "null", connstr = "null";

            if (MySQLconnection != null)
            {
                state = MySQLconnection.State.ToString();
                
                if (ipmode == "v6")
                    dbase = this.ServerInfo.DBname;
                else //v4
                    dbase = this.ServerInfo.DBname_v4;

                dbsource = MySQLconnection.DataSource;
                driver = MySQLconnection.Driver;
                connstr = MySQLconnection.ConnectionString;

                if (MySQLconnection.State == ConnectionState.Open)
                    ver = MySQLconnection.ServerVersion;
            }

            if (this.ServerInfo.DBname != "")
                dbase_v6 = this.ServerInfo.DBname;

            if (this.ServerInfo.DBname_v4 != "")
                dbase_v4 = this.ServerInfo.DBname_v4;

            MessageBox.Show(StringsDictionary.KeyValue("Form1_Status_connectionState", this.culture) + state + Environment.NewLine
                + StringsDictionary.KeyValue("Form1_Status_currentDB", this.culture) + dbase + Environment.NewLine
                + StringsDictionary.KeyValue("Form1_Status_currentMode", this.culture) + ipmode + Environment.NewLine
                + Environment.NewLine
                + StringsDictionary.KeyValue("Form1_Status_v6DB", this.culture) + dbase_v6 + Environment.NewLine
                + StringsDictionary.KeyValue("Form1_Status_v4DB", this.culture) + dbase_v4 + Environment.NewLine
                + Environment.NewLine
                + StringsDictionary.KeyValue("Form1_Status_dataSource", this.culture) + "'" + dbsource + "'" + Environment.NewLine
                + StringsDictionary.KeyValue("Form1_Status_serverVers", this.culture) + ver + Environment.NewLine
                + StringsDictionary.KeyValue("Form1_Status_driver", this.culture) + driver + Environment.NewLine
                + StringsDictionary.KeyValue("Form1_Status_connString", this.culture) + Environment.NewLine
                + connstr, StringsDictionary.KeyValue("Form1_Status_header", this.culture));
        }

        private void databasetoolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (this.listBox1.Items.Count > 0)
            {
                if (this.listBox1.SelectedItem != null && this.listBox1.SelectedItem.ToString() != ""
                    && this.listBox1.SelectedIndex != -1)
                {
                    if (MySQLconnection != null)
                    {
                        this.sendtoDBtoolStripMenuItem.Enabled = true;
                        this.getPrefixInfoFromDatabaseToolStripMenuItem.Enabled = true;
                        this.sublevelstoolStripMenuItem1.Enabled = true;
                    }
                    else
                    {
                        this.sendtoDBtoolStripMenuItem.Enabled = false;
                        this.getPrefixInfoFromDatabaseToolStripMenuItem.Enabled = false;
                        this.sublevelstoolStripMenuItem1.Enabled = false;
                    }
                }
                else
                {
                    this.sendtoDBtoolStripMenuItem.Enabled = false;
                    this.sublevelstoolStripMenuItem1.Enabled = false;
                    this.getPrefixInfoFromDatabaseToolStripMenuItem.Enabled = false;
                }
            }
            else
            {
                this.sendtoDBtoolStripMenuItem.Enabled = false;
                this.sublevelstoolStripMenuItem1.Enabled = false;
                this.getPrefixInfoFromDatabaseToolStripMenuItem.Enabled = false;
            }

            if ((this.trackBar2.Value - this.trackBar1.Value >= 0) && MySQLconnection != null)
                this.statstoolStripMenuItem1.Enabled = true;
            else
                this.statstoolStripMenuItem1.Enabled = false;

            if (MySQLconnection != null)
            {
                this.opendbformtoolStripMenuItem.Enabled = true;
                this.changeDatabasetoolStripMenuItem.Enabled = true;
            }
            else
            {
                this.opendbformtoolStripMenuItem.Enabled = false;
                this.changeDatabasetoolStripMenuItem.Enabled = false;
            }
        }

        private void prefixsublevelstoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex == -1)
                return;

            string selected = this.listBox1.SelectedItem.ToString().Split(' ')[1];
            string snet = selected.Split('/')[0];
            short plen = Convert.ToInt16(selected.Split('/')[1]);

            if (!OpenMySQLconnection(ipmode))
                return;

            PrefixSubLevels pflevels = null;

            pflevels = new PrefixSubLevels(snet, plen,
            this.checkBox2.CheckState, this.trackBar1.Value, this.trackBar2.Value, MySQLconnection,
            this.ServerInfo, this.culture, ipmode, this.listBox1.Font);


            if (!pflevels.IsDisposed)
            {
                pflevels.Show();
                //
                windowsList.Add(new WindowsList(pflevels, pflevels.Name, pflevels.GetHashCode(), ipmode));

                this.changeDBstate += pflevels.DBStateChange;
                this.ChangeUILanguage += pflevels.SwitchLanguage;
                //this.ChangeDatabase += pflevels.ChangeDatabase;
            }
        }

        private void compressaddrtoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CompressAddress compress = new CompressAddress(this.culture);
            compress.Show();
            //
            windowsList.Add(new WindowsList(compress, compress.Name, compress.GetHashCode(), ipmode));

            this.ChangeUILanguage += compress.SwitchLanguage;
        }

        private void findprefixtoolStripMenuItem1_Click(object sender, EventArgs e)
        {

            Goto findpfx = new Goto(this, goToToolStripMenuItem.DropDownItems.IndexOf(findprefixtoolStripMenuItem1),
                this.totmaxval, ID, this.culture, ipmode);

            findpfx.ShowDialog();
            this.ChangeUILanguage += findpfx.SwitchLanguage;

            if (this.findpfx == "")
            {
                if (findpfx is IDisposable)
                    findpfx.Dispose();
                return;
            }
            SEaddress seaddr = new SEaddress();
            seaddr.slash = this.trackBar1.Value;
            seaddr.subnetslash = this.trackBar2.Value;
            String ss = "", se = "";
            int count = 0;

            if (ipmode == "v6")
            {
                string Resv6 = v6ST.FormalizeAddr(this.findpfx);

                if (this.checkBox2.CheckState == CheckState.Checked) /* 128 bits */
                {
                    if (Resv6.Length == 32)
                        Resv6 = "0" + Resv6;
                }
                else if (this.checkBox2.CheckState == CheckState.Unchecked) /* 64 bits */
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

                    seaddr = v6ST.FindPrefixIndex(seaddr, this.checkBox2.CheckState);

                    subnets.subnetidx = seaddr.subnetidx;
                    subnets.slash = this.trackBar1.Value;
                    subnets.subnetslash = this.trackBar2.Value;
                    subnets.Start = StartEnd.Start;
                    subnets.ResultIPAddr = StartEnd.ResultIPAddr;

                    subnets = v6ST.GoToSubnet(subnets, this.checkBox2.CheckState);

                    if (before == subnets.Start)
                    {
                        page.Start = subnets.Start;
                        page.End = BigInteger.Zero;

                        if (subnets.End.Equals(StartEnd.End))
                        {
                            this.Forwd.Enabled = false;
                        }

                        this.listBox1.Items.Clear();
                        this.label16.Text = "   ";

                        for (count = 0; count < upto; count++)
                        {
                            subnets = v6ST.Subnetting(subnets, this.checkBox2.CheckState);

                            if (this.checkBox2.CheckState == CheckState.Checked)
                            {
                                ss = String.Format("{0:x}", subnets.Start);
                                if (ss.Length > 32)
                                    ss = ss.Substring(1, 32);
                                ss = v6ST.Kolonlar(ss, this.checkBox2.CheckState);
                                ss = v6ST.CompressAddress(ss);

                                se = String.Format("{0:x}", subnets.End);
                                if (se.Length > 32)
                                    se = se.Substring(1, 32);
                                se = v6ST.Kolonlar(se, this.checkBox2.CheckState);
                                se = v6ST.CompressAddress(se);

                                //ss = "s" + subnets.subnetidx + "> " + ss + "/" +
                                ss = "p" + subnets.subnetidx + "> " + ss + "/" +
                                    this.trackBar2.Value;
                                this.listBox1.Items.Add(ss);
                                /* No need to print two times if slash=128 since start=end */
                                if (this.trackBar2.Value != 128)
                                {
                                    if (this.checkBox3.CheckState == CheckState.Checked)
                                    {
                                        se = "e" + subnets.subnetidx + "> " + se + "/"
                                            + this.trackBar2.Value;
                                        this.listBox1.Items.Add(se);
                                        this.listBox1.Items.Add("");
                                    }
                                }
                            }
                            else if (this.checkBox2.CheckState == CheckState.Unchecked)
                            {
                                ss = String.Format("{0:x}", subnets.Start);
                                if (ss.Length > 16)
                                    ss = ss.Substring(1, 16);
                                ss = v6ST.Kolonlar(ss, this.checkBox2.CheckState);
                                ss = v6ST.CompressAddress(ss);

                                se = String.Format("{0:x}", subnets.End);
                                if (se.Length > 16)
                                    se = se.Substring(1, 16);
                                se = v6ST.Kolonlar(se, this.checkBox2.CheckState);
                                se = v6ST.CompressAddress(se);

                                //ss = "s" + subnets.subnetidx + "> " + ss + "/" +
                                ss = "p" + subnets.subnetidx + "> " + ss + "/" +
                                    this.trackBar2.Value;
                                this.listBox1.Items.Add(ss);
                                /* No need to print two times if slash=64 since start=end */
                                if (this.trackBar2.Value != 64)
                                {
                                    if (this.checkBox3.CheckState == CheckState.Checked)
                                    {
                                        se = "e" + subnets.subnetidx + "> " + se + "/"
                                            + this.trackBar2.Value;
                                        this.listBox1.Items.Add(se);
                                        this.listBox1.Items.Add("");
                                    }
                                }
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
                        if (subnets.subnetidx == this.submaxval)
                        {
                            this.Forwd.Enabled = false;
                            this.Last.Enabled = false;
                        }
                        else
                        {
                            this.Forwd.Enabled = true;
                            this.Last.Enabled = true;
                        }
                        UpdateCount();
                        UpdatePrintBin(StartEnd, this.checkBox2.CheckState);
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
            else //v4
            {
                string Resv4 = v6ST.FormalizeAddr_v4(this.findpfx);

                seaddr.ResultIPAddr = seaddr.Start = BigInteger.Parse("0" + Resv4, NumberStyles.AllowHexSpecifier);

                if (seaddr.ResultIPAddr >= StartEnd.Start && seaddr.ResultIPAddr <= StartEnd.End)
                {
                    // inside
                    BigInteger before = seaddr.ResultIPAddr;

                    seaddr = v6ST.FindPrefixIndex_v4(seaddr);

                    subnets.subnetidx = seaddr.subnetidx;
                    subnets.slash = this.trackBar1.Value;
                    subnets.subnetslash = this.trackBar2.Value;
                    subnets.Start = StartEnd.Start;
                    subnets.ResultIPAddr = StartEnd.ResultIPAddr;

                    subnets = v6ST.GoToSubnet_v4(subnets);

                    if (before == subnets.Start)
                    {
                        page.Start = subnets.Start;
                        page.End = 0;

                        if (subnets.End.Equals(StartEnd.End))
                        {
                            this.Forwd.Enabled = false;
                        }

                        this.listBox1.Items.Clear();
                        this.label16.Text = "   ";

                        for (count = 0; count < upto; count++)
                        {
                            subnets = v6ST.Subnetting_v4(subnets);

                            ss = String.Format("{0:x}", subnets.Start);
                            ss = v6ST.IPv4Format(ss);

                            se = String.Format("{0:x}", subnets.End);
                            se = v6ST.IPv4Format(se);

                            ss = "p" + subnets.subnetidx + "> " + ss + "/"
                                + this.trackBar2.Value;
                            this.listBox1.Items.Add(ss);

                            if (this.checkBox3.CheckState == CheckState.Checked)
                            {
                                se = "e" + subnets.subnetidx + "> " + se + "/"
                                    + this.trackBar2.Value;
                                this.listBox1.Items.Add(se);
                                this.listBox1.Items.Add("");
                            }

                            if (subnets.End.Equals(StartEnd.End))
                            {
                                this.Forwd.Enabled = false;
                                break;
                            }
                            else
                            {
                                subnets.Start = subnets.End + 1;
                            }
                        }
                        page.End = subnets.End;

                        if (seaddr.subnetidx == 0)
                        {
                            this.Backwd.Enabled = false;
                        }
                        else
                            this.Backwd.Enabled = true;
                        if (subnets.subnetidx == this.submaxval)
                        {
                            this.Forwd.Enabled = false;
                            this.Last.Enabled = false;
                        }
                        else
                        {
                            this.Forwd.Enabled = true;
                            this.Last.Enabled = true;
                        }
                        UpdateCount();
                        UpdatePrintBin_v4(StartEnd);
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

        private void findprefixtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.findprefixtoolStripMenuItem1_Click(null, null);
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                graph.Clear(Form1.DefaultBackColor);

                graph.DrawLine(new Pen(Color.Red), 279, 266, 279, 272); // en sol dikey   / leftmost vertical line
                graph.DrawLine(new Pen(Color.Red), 407, 266, 407, 272); // en sag dikey   / rightmost vertical line
                graph.DrawLine(new Pen(Color.Red), 343, 266, 343, 272); // ortadaki dikey / middle vertical line
                graph.DrawLine(new Pen(Color.Red), 311, 268, 311, 272); // 1.ceyrek dikey / 1st.quarter vertical line
                graph.DrawLine(new Pen(Color.Red), 375, 268, 375, 272); // 3.ceyrek dikey / 3rd.quarter vertical line
                graph.DrawLine(new Pen(Color.Red), 279, 272, 407, 272); // alt cizgi      / base line 

                if (this.listBox1.Items.Count > 0)
                {
                    int count = 128;

                    if (this.pix > 0)
                    {
                        if (this.submax - this.currentidx <= 128)
                        {
                            graph.FillRectangle(new SolidBrush(Color.Red), 279, 269, count, 3);
                            this.label16.Text = "100.0%"; // "100%"
                        }
                        else
                        {
                            graph.FillRectangle(new SolidBrush(Color.Red), 279, 269,
                                (float)((this.currentidx + 128) / this.pix), 3);

                            this.label16.Text = (((float)(this.currentidx + 128) * 100 / (float)this.submax)).ToString("0.0") + "%";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            /* Onceki dikdortgen  / Previously using rectangle
            try
            {
                graph.Clear(Form1.DefaultBackColor);
                graph.DrawRectangle(new Pen(Color.Red), 250, 256, 128, 11);

                if (this.listBox1.Items.Count > 0)
                {
                    int count = 128;

                    if (this.pix > 0)
                    {
                        if (this.submax - this.currentidx <= 128)
                            graph.FillRectangle(new SolidBrush(Color.Red), 250, 256, count, 11);
                        else
                        {
                            graph.FillRectangle(new SolidBrush(Color.Red), 250, 256,
                                (float)((this.currentidx + 128) / this.pix), 11);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            */
        }

        private void statsusagetoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.textBox3.Text != "" && this.textBox5.Text != "")  // Start: & End:
            {
                if (!OpenMySQLconnection(ipmode))
                    return;

                StatsUsage stats = null;

                if (ipmode == "v6")
                {
                    string[] sa = this.textBox3.Text.Split('/');
                    sa[0] = v6ST.CompressAddress(sa[0]);

                    stats = new StatsUsage(sa[0] + "/" + sa[1], this.textBox5.Text,
                        (short)this.trackBar1.Value, (short)this.trackBar2.Value,
                        this.checkBox2.CheckState, MySQLconnection, this.ServerInfo, this.culture, ipmode);
                }
                else // v4
                {
                    stats = new StatsUsage(this.textBox3.Text, this.textBox5.Text,
                        (short)this.trackBar1.Value, (short)this.trackBar2.Value,
                        this.checkBox2.CheckState, MySQLconnection, this.ServerInfo, this.culture, ipmode);
                }

                if (!stats.IsDisposed)
                {
                    stats.Show();
                    //
                    windowsList.Add(new WindowsList(stats, stats.Name, stats.GetHashCode(), ipmode));

                    this.ChangeUILanguage += stats.SwitchLanguage;
                    this.changeDBstate += stats.DBStateChange;
                    //this.ChangeDatabase += stats.ChangeDatabase;
                }
            }
            else
                return;
        }

        private void statsusagetoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.statsusagetoolStripMenuItem_Click(null, null);
        }

        private void statstoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.statsusagetoolStripMenuItem_Click(null, null);
        }

        private void opendbformtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            short parentpflen = (short)this.trackBar1.Value;

            string selected = "", snet = ""; short plen = 0;

            if (this.listBox1.SelectedIndex != -1)
            {
                selected = this.listBox1.SelectedItem.ToString().Split(' ')[1];
                snet = selected.Split('/')[0];
                plen = Convert.ToInt16(selected.Split('/')[1]);
            }
            else
            {
                snet = null;
                plen = 0;
            }

            if (!OpenMySQLconnection(ipmode))
                return;

            DatabaseUI dbui = null;

            dbui = new DatabaseUI(snet, plen, parentpflen, MySQLconnection,
                this.ServerInfo, this.culture, ipmode, this.listBox1.Font);

            if (!dbui.IsDisposed)
            {
                dbui.Show();
                //
                windowsList.Add(new WindowsList(dbui, dbui.Name, dbui.GetHashCode(), ipmode));

                this.changeDBstate += dbui.DBStateChange;
                this.ChangeUILanguage += dbui.SwitchLanguage;
                //this.ChangeDatabase += dbui.ChangeDatabase;
            }
        }

        private void ASnumberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ASNumberPlainDot asnum = new ASNumberPlainDot(this.culture);
            asnum.Show();
            //
            windowsList.Add(new WindowsList(asnum, asnum.Name, asnum.GetHashCode(), ipmode));

            this.ChangeUILanguage += asnum.SwitchLanguage;
        }

        #region WindowsRegion

        void tsi_Click(object sender, EventArgs e, int hc)
        {
            foreach (WindowsList wl in windowsList)
            {
                if (wl.hc == hc)
                {
                    if (wl.form.WindowState == FormWindowState.Minimized)
                        wl.form.WindowState = FormWindowState.Normal;
                    wl.form.BringToFront();
                    wl.form.Location = this.Location;
                }
            }
        }

        public static void RemoveForm(int hc)
        {
            WindowsList tmp = null;

            foreach (WindowsList i in windowsList)
            {
                if (i.hc == hc)
                {
                    tmp = i;
                }
            }
            if (tmp != null)
                windowsList.Remove(tmp);
        }

        private void windowstoolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            this.windowstoolStripMenuItem.DropDownItems[1].Text =
                StringsDictionary.KeyValue("Form1_closeAlltoolStripMenuItem.Text", this.culture)
                + " (" + (Application.OpenForms.Count - 1) + ")";

            int c = this.windowstoolStripMenuItem.DropDownItems.Count - 1;
            if (c >= 3)
            {
                for (int i = c; i > 2; i--)
                {
                    this.windowstoolStripMenuItem.DropDownItems.RemoveAt(i);
                }
            }

            foreach (WindowsList i in windowsList)
            {
                ToolStripMenuItem tsmi = new ToolStripMenuItem(i.name);
                tsmi.Name = i.name;
                windowstoolStripMenuItem.DropDownItems.Add(tsmi);
                tsmi.Click += new EventHandler((sender1, e1) => tsi_Click(sender1, e1, i.hc));
            }
        }

        private void closeAlltoolStripMenuItem_Click(object sender, EventArgs e)
        {
            int w = Application.OpenForms.Count;

            int c = this.windowstoolStripMenuItem.DropDownItems.Count - 1;
            if (c >= 3)
            {
                for (int i = c; i > 2; i--)
                {
                    Application.OpenForms[this.windowstoolStripMenuItem.DropDownItems[i].Text].Close();
                    this.windowstoolStripMenuItem.DropDownItems.RemoveAt(i);
                }
            }
        }
        #endregion WindowsRegion

        private void checkBox3_Click(object sender, EventArgs e)
        {
            // End checkBox
            if (checkBox3.Checked)
            {
                if (ipmode == "v6")
                {
                    ScreenShotValues.isEndChecked = true;
                }
                else // v4
                {
                    ScreenShotValues.isEndChecked_v4 = true;
                }
            }
            else
            {
                if (ipmode == "v6")
                {
                    ScreenShotValues.isEndChecked = false;
                }
                else // v4
                {
                    ScreenShotValues.isEndChecked_v4 = false;
                }
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (ipmode == "v6")
            {
                if (this.listBox1.Items.Count > 0)
                {
                    string first = this.listBox1.Items[0].ToString().Split(' ')[1].Split('/')[0].Trim();

                    if (this.checkBox2.CheckState == CheckState.Unchecked)
                        first = v6ST.FormalizeAddr(first).Substring(0, 16);
                    else
                        first = v6ST.FormalizeAddr(first);

                    SEaddress tmpse = new SEaddress();

                    tmpse.Start = BigInteger.Parse("0" + first, NumberStyles.AllowHexSpecifier);
                    tmpse = v6ST.Subnetting(tmpse, this.checkBox2.CheckState);

                    this.listBox1.Items.Clear();
                    this.label16.Text = "   ";
                    this.maxfontwidth = 0;
                    this.listBox1.HorizontalExtent = 0;

                    if (this.checkBox2.CheckState == CheckState.Unchecked && this.trackBar2.Value == 64
                        ||
                        this.checkBox2.CheckState == CheckState.Checked && this.trackBar2.Value == 128
                        )
                    {
                        this.checkBox3.Checked = false;
                        this.checkBox3.Enabled = false;
                    }
                    else
                        this.checkBox3.Enabled = true;

                    int delta = this.trackBar2.Value - this.trackBar1.Value;

                    tmpse.slash = this.trackBar1.Value;
                    tmpse.subnetslash = this.trackBar2.Value;
                    tmpse.upto = upto;
                    tmpse.UpperLimitAddress = StartEnd.End;

                    subnets = v6ST.ListFirstPage(tmpse, this.checkBox2.CheckState, this.checkBox3.CheckState);
                    this.page.End = subnets.End;
                    this.listBox1.Items.AddRange(subnets.liste.ToArray());
                }
            }
            else //v4
            {
                if (this.listBox1.Items.Count > 0)
                {
                    string first = this.listBox1.Items[0].ToString().Split(' ')[1].Split('/')[0].Trim();

                    first = v6ST.FormalizeAddr_v4(first);

                    SEaddress tmpse = new SEaddress();

                    tmpse.Start = BigInteger.Parse("0" + first, NumberStyles.AllowHexSpecifier);
                    tmpse = v6ST.Subnetting_v4(tmpse);

                    this.listBox1.Items.Clear();
                    this.label16.Text = "   ";
                    this.maxfontwidth = 0;
                    this.listBox1.HorizontalExtent = 0;

                    this.checkBox3.Enabled = true;
                    if (this.trackBar2.Value == 32)
                    {
                        this.checkBox3.Enabled = false;
                    }
                    else
                    {
                        this.checkBox3.Enabled = true;
                    }

                    int delta = this.trackBar2.Value - this.trackBar1.Value;

                    tmpse.slash = this.trackBar1.Value;
                    tmpse.subnetslash = this.trackBar2.Value;
                    tmpse.upto = upto;
                    tmpse.UpperLimitAddress = StartEnd.End;

                    subnets = v6ST.ListFirstPage_v4(tmpse, this.checkBox3.CheckState);
                    this.page.End = subnets.End;
                    this.listBox1.Items.AddRange(subnets.liste.ToArray());
                }
            }

            UpdateCount();
        }

        private void AddressTypeInfotoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim() != "")
            {
                AddressTypeInfo addrnameinfo =
                    new AddressTypeInfo(
                        v6ST.AddressType(
                        BigInteger.Parse("0" + this.textBox1.Text.Replace(":", ""), NumberStyles.AllowHexSpecifier),
                        this.trackBar1.Value, this.checkBox2.CheckState),
                        this.textBox1.Text
                        );
                addrnameinfo.Show();
                //
                windowsList.Add(new WindowsList(addrnameinfo, addrnameinfo.Name, addrnameinfo.GetHashCode(), ipmode));
            }
            else
                MessageBox.Show(StringsDictionary.KeyValue("Form1_AddressTypeInfotoolStripMenuItem", this.culture),
                    "Empty Address", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void getPrefixInfoFromDBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string pfx = this.listBox1.SelectedItem.ToString().Split(' ')[1];

            if (!OpenMySQLconnection(ipmode))
                return;

            GetPrefixInfoFromDB getPfxInfo = null;

            getPfxInfo = new GetPrefixInfoFromDB(pfx, MySQLconnection, this.ServerInfo, this.culture, ipmode);


            if (!getPfxInfo.IsDisposed)
                getPfxInfo.ShowDialog();
        }

        private void getPrefixInfoFromDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getPrefixInfoFromDBToolStripMenuItem_Click(null, null);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ScreenShotValues.LocX = this.Location.X;
            ScreenShotValues.LocY = this.Location.Y;

            TakeScreenShot();

            xmlfile.WriteValues();

            if (MySQLconnection != null)
            {
                MySQLconnection.Close();

                if (MySQLconnection is IDisposable)
                    MySQLconnection.Dispose();
            }
        }

        private void iPv6ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = (ToolStripMenuItem)sender;

            if (ipmode == "v6")
            {
                selected.Checked = true;
                this.label23.Text = "IPv6:";
                pictureBox1.Image = v6Bitmap;
                return;
            }

            TakeScreenShot();

            this.UncheckAllMode((ToolStripMenuItem)sender);
            
            this.SwitchMode();
            
            GetValuesBack();

            this.label23.Text = "IPv6:";

            pictureBox1.Image = v6Bitmap;

            if (MySQLconnection != null && MySQLconnection.State == ConnectionState.Open)
            {
                if (this.ServerInfo.DBname != "")
                {
                    try
                    {
                        MySQLconnection.ChangeDatabase(this.ServerInfo.DBname);
                        this.ChangeDatabase.Invoke(this.ServerInfo.DBname);
                        //this.statusofDBtoolStripMenuItem_Click(null, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: \r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
        }

        private void iPv4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selected = (ToolStripMenuItem)sender;

            if (ipmode == "v4")
            {
                selected.Checked = true;
                this.label23.Text = "IPv4:";
                pictureBox1.Image = v4Bitmap;
                return;
            }

            TakeScreenShot();

            this.UncheckAllMode((ToolStripMenuItem)sender);
            
            this.SwitchMode();
            
            GetValuesBack();

            this.label23.Text = "IPv4:";

            pictureBox1.Image = v4Bitmap;

            if (MySQLconnection != null && MySQLconnection.State == ConnectionState.Open)
            {
                if (this.ServerInfo.DBname_v4 != "")
                {
                    try
                    {
                        MySQLconnection.ChangeDatabase(this.ServerInfo.DBname_v4);
                        this.ChangeDatabase.Invoke(this.ServerInfo.DBname_v4);
                        //this.statusofDBtoolStripMenuItem_Click(null, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: \r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void TakeScreenShot()
        {
            if (ipmode == "v6")
            {
                ScreenShotValues.mode = "v6";
                ScreenShotValues.Address = this.textBox2.Text.Trim();
                ScreenShotValues.is128Checked = this.checkBox2.Checked;
                ScreenShotValues.isSubnetChecked = this.checkBox1.Checked;
                ScreenShotValues.isEndChecked = this.checkBox3.Checked;
                ScreenShotValues.TrackBar1Value = this.trackBar1.Value;
                ScreenShotValues.TrackBar2Value = this.trackBar2.Value;

                if (this.textBox4.Text.TrimStart('#') != "")
                    ScreenShotValues.currentAddrSpaceNo = BigInteger.Parse(this.textBox4.Text.TrimStart('#'));
            }
            else // v4
            {
                ScreenShotValues.mode = "v4";
                ScreenShotValues.Address_v4 = this.textBox2.Text.Trim();
                ScreenShotValues.isSubnetChecked_v4 = this.checkBox1.Checked;
                ScreenShotValues.isEndChecked_v4 = this.checkBox3.Checked;
                ScreenShotValues.TrackBar1Value_v4 = this.trackBar1.Value;
                ScreenShotValues.TrackBar2Value_v4 = this.trackBar2.Value;

                if (this.textBox4.Text.TrimStart('#') != "")
                    ScreenShotValues.currentAddrSpaceNo_v4 = BigInteger.Parse(this.textBox4.Text.TrimStart('#'));
            }
        }

        private void UncheckAllMode(ToolStripMenuItem selected)
        {
            selected.Checked = true;

            foreach (var MenuItem in
                (from object item in selected.Owner.Items
                 let MenuItem = item as ToolStripMenuItem
                 where !item.Equals(selected)
                 where MenuItem != null
                 select MenuItem))
                MenuItem.Checked = false;
        }

        private void SwitchMode()
        {
            if (this.iPv6ToolStripMenuItem.Checked == true)
            {
                ipmode = "v6";

                if (ScreenShotValues.is128Checked)
                {
                    this.ExpandView();
                }
                else
                {
                    this.DefaultView();
                }

                ResetAllValues();
                
                this.textBox9.Text = "v6 mode";

            }
            else if (this.iPv4ToolStripMenuItem.Checked == true)
            {
                ipmode = "v4";

                this.DefaultView();

                ResetAllValues();

                this.textBox9.Text = "v4 mode";

            }
        }

        private void GetValuesBack()
        {
            if (ipmode == "v6")
            {
                if (v6ST.IsAddressCorrect(ScreenShotValues.Address))
                {
                    this.textBox2.Text = ScreenShotValues.Address;

                    if (ScreenShotValues.ResetFlag)
                        return;
                    else
                        Find_Click(null, null);
                }
                else
                    return;

                this.checkBox2.Checked = ScreenShotValues.is128Checked;

                this.checkBox1.Checked = ScreenShotValues.isSubnetChecked;
                this.trackBar1.Value = ScreenShotValues.TrackBar1Value;
                this.trackBar1_Scroll(null, null);
                this.trackBar2.Value = ScreenShotValues.TrackBar2Value;
                this.trackBar2_Scroll(null, null);

                GoToAddrSpaceNumber(ScreenShotValues.currentAddrSpaceNo);

                if (this.checkBox1.Checked)
                {
                    this.Subnets_Click(null, null);
                    this.Subnets.Focus();
                }
                this.checkBox3.Checked = ScreenShotValues.isEndChecked;
            }
            else // v4
            {
                if (v6ST.IsAddressCorrect_v4(ScreenShotValues.Address_v4))
                {
                    this.textBox2.Text = ScreenShotValues.Address_v4;

                    if (ScreenShotValues.ResetFlag_v4)
                        return;
                    else
                        Find_Click(null, null);
                }
                else
                    return;

                this.checkBox1.Checked = ScreenShotValues.isSubnetChecked_v4;
                this.trackBar1.Value = ScreenShotValues.TrackBar1Value_v4;
                this.trackBar1_Scroll(null, null);
                this.trackBar2.Value = ScreenShotValues.TrackBar2Value_v4;
                this.trackBar2_Scroll(null, null);

                GoToAddrSpaceNumber(ScreenShotValues.currentAddrSpaceNo_v4);

                if (this.checkBox1.Checked)
                {
                    this.Subnets_Click(null, null);
                    this.Subnets.Focus();
                }
                this.checkBox3.Checked = ScreenShotValues.isEndChecked_v4;
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if ((string)pictureBox1.Image.Tag == "v6Bt")
            {
                pictureBox1.Image = v4Bitmap;
                iPv4ToolStripMenuItem_Click(iPv4ToolStripMenuItem, null);
            }
            else
            {
                pictureBox1.Image = v6Bitmap;
                iPv6ToolStripMenuItem_Click(iPv6ToolStripMenuItem, null);
            }
        }

        private void changeDatabasetoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MySQLconnection != null)
            {
                DBinfo dbinfo = new DBinfo(this.culture, this.ServerInfo, MySQLconnection, ipmode);
                this.ChangeUILanguage += dbinfo.SwitchLanguage;

                if (dbinfo.ShowDialog() == DialogResult.Cancel)
                    return;
                
                this.ServerInfo = dbinfo.ServerInfo;
                string currentDB = "null";

                try
                {
                    if (MySQLconnection.State != ConnectionState.Open)
                        MySQLconnection.Open();

                    if (ipmode == "v6")
                    {
                        if (this.ServerInfo.DBname == "")
                            return;

                        MySQLconnection.ChangeDatabase(this.ServerInfo.DBname);
                        this.ChangeDatabase.Invoke(this.ServerInfo.DBname);
                        currentDB = this.ServerInfo.DBname;
                    }
                    else // v4
                    {
                        if (this.ServerInfo.DBname_v4 == "")
                            return;

                        MySQLconnection.ChangeDatabase(this.ServerInfo.DBname_v4);
                        this.ChangeDatabase.Invoke(this.ServerInfo.DBname_v4);
                        currentDB = this.ServerInfo.DBname_v4;
                    }

                    if (this.ServerInfo.launchDBUI)
                        this.opendbformtoolStripMenuItem_Click(null, null);

                    MessageBox.Show(StringsDictionary.KeyValue("Form1_changeDatabasetoolStripMenuItem", this.culture) + currentDB,
                        "DB:", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: \r\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                MessageBox.Show(StringsDictionary.KeyValue("Form1_closeDBtoolStripMenuItem_Click_noDB", this.culture),
                    StringsDictionary.KeyValue("Form1_closeDBtoolStripMenuItem_Click_noDB_header", this.culture),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        private bool OpenMySQLconnection(string mode)
        {
            if (mode == "v6")
            {
                if (MySQLconnection == null || this.ServerInfo.DBname == "" || this.ServerInfo.Tablename == "")
                {
                    DBinfo dBinfo = new DBinfo(this.culture, this.ServerInfo, MySQLconnection, mode);

                    if (dBinfo.ShowDialog() == DialogResult.Cancel)
                        return false;

                    this.ServerInfo = dBinfo.ServerInfo;

                    if (this.ServerInfo.DBname == "" || this.ServerInfo.Tablename == "")
                        return false;

                    try
                    {
                        if (MySQLconnection == null)
                        {
                            return false;
                        }
                        else
                        {
                            if (MySQLconnection.State != ConnectionState.Open)
                                MySQLconnection.Open();

                            if (this.ServerInfo.DBname != "")
                                MySQLconnection.ChangeDatabase(this.ServerInfo.DBname);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: openDBForm" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else // v4
            {
                if (MySQLconnection == null || this.ServerInfo.DBname_v4 == "" || this.ServerInfo.Tablename_v4 == "")
                {
                    DBinfo dBinfo = new DBinfo(this.culture, this.ServerInfo, MySQLconnection, mode);

                    if (dBinfo.ShowDialog() == DialogResult.Cancel)
                        return false;

                    this.ServerInfo = dBinfo.ServerInfo;

                    if (this.ServerInfo.DBname_v4 == "" || this.ServerInfo.Tablename_v4 == "")
                        return false;

                    try
                    {
                        if (MySQLconnection == null)
                        {
                            return false;
                        }
                        else
                        {
                            if (MySQLconnection.State != ConnectionState.Open)
                                MySQLconnection.Open();

                            if (this.ServerInfo.DBname_v4 != "")
                                MySQLconnection.ChangeDatabase(this.ServerInfo.DBname_v4);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: OpenMySQLconnection()" + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            return true;
        }

        private void CloseMySQLconnection()
        {
            try
            {
                if (MySQLconnection != null)
                {
                    if (MessageBox.Show(
                        StringsDictionary.KeyValue("Form1_closeDBtoolStripMenuItem_Click_closeDB", this.culture),
                        StringsDictionary.KeyValue("Form1_closeDBtoolStripMenuItem_Click_closeDB_header", this.culture),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        == DialogResult.Yes
                        )
                    {

                        MySQLconnection.Close();

                        if (MySQLconnection is IDisposable)
                            MySQLconnection.Dispose();

                        MySQLconnection = null;

                        this.changeDBstate.Invoke(MySQLconnection, -1);

                        MessageBox.Show(StringsDictionary.KeyValue("Form1_DBclosed.Text", this.culture), "",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                        return;
                }
                else
                {
                    MessageBox.Show(StringsDictionary.KeyValue("Form1_closeDBtoolStripMenuItem_Click_noDB", this.culture),
                        StringsDictionary.KeyValue("Form1_closeDBtoolStripMenuItem_Click_noDB_header", this.culture),
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception: CloseMySQLconnection()" + Environment.NewLine + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                this.Find_Click(null, null);
                this.Find.Focus();
            }
        }
    }
}
