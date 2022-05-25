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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Numerics;
using System.IO;
using System.Threading;

namespace IPv6SubnettingTool
{
    public partial class SaveAsText : Form
    {
        #region special initials/constants -yucel
        public const int ID = 3; // ID of this Form.
        public int incomingID;
        public BigInteger maxsubnet = BigInteger.Zero;
        SEaddress StartEnd = new SEaddress();
        BigInteger FromIndex = BigInteger.Zero;
        BigInteger ToIndex = BigInteger.Zero;
        BigInteger TotalBytes = BigInteger.Zero;

        CheckState is128Checked;
        bool SelectedRange = false;
        int input_subnetslash = 0;
        FileDialog saveDialog = new SaveFileDialog();
        DiskSpace diskspace = new DiskSpace();
        BigInteger count = BigInteger.Zero;
        BigInteger howmany = BigInteger.Zero;
        CultureInfo culture;

        public class CurrentState
        {
            public BigInteger SavedLines = BigInteger.Zero;
            public int percentage = 0;
        }
        CurrentState saveState = new CurrentState();
        #endregion

        public SaveAsText(SEaddress input, CheckState is128Checked, CultureInfo culture, bool selectedrange)
        {
            InitializeComponent();

            #region special initials -yucel
            this.StartEnd.ID = ID;
            this.incomingID = input.ID;
            this.culture = culture;
            this.SelectedRange = selectedrange;
            this.input_subnetslash = input.subnetslash;
            #endregion

            this.SwitchLanguage(this.culture);

            this.is128Checked = is128Checked;
            this.StartEnd.LowerLimitAddress = input.LowerLimitAddress;
            this.StartEnd.ResultIPAddr = input.ResultIPAddr;
            this.StartEnd.slash = input.slash;
            this.StartEnd.Start = input.Start;
            this.StartEnd.End = input.End;
            this.StartEnd.subnetidx = input.subnetidx;
            this.StartEnd.subnetslash = input.subnetslash;
            this.StartEnd.UpperLimitAddress = input.UpperLimitAddress;
            this.StartEnd.upto = input.upto;

            string ss = String.Format("{0:x}", input.Start);
            string se = String.Format("{0:x}", input.End);

            if (IPv6SubnettingTool.Form1.ipmode == "v6")
            {
                ss = ss.TrimStart('0'); se = se.TrimStart('0');

                if (this.is128Checked == CheckState.Unchecked)
                {
                    ss = ss.PadLeft(16, '0');
                    ss = v6ST.Kolonlar(ss, this.is128Checked) + "/" + input.subnetslash.ToString();
                    se = se.PadLeft(16, '0');
                    se = v6ST.Kolonlar(se, this.is128Checked) + "/" + input.subnetslash.ToString();
                }
                else if (this.is128Checked == CheckState.Checked)
                {
                    ss = ss.PadLeft(32, '0');
                    ss = v6ST.Kolonlar(ss, this.is128Checked) + "/" + input.subnetslash.ToString();
                    se = se.PadLeft(32, '0');
                    se = v6ST.Kolonlar(se, this.is128Checked) + "/" + input.subnetslash.ToString();
                }
            }
            else // v4
            {
                ss = v6ST.IPv4Format(ss) + "/" + input.subnetslash.ToString();
                se = v6ST.IPv4Format(se) + "/" + input.subnetslash.ToString();
            }
            this.label8.Text = ss;
            this.label9.Text = se;

            if (input.ID == 0 || input.ID == 2)
            {
                this.maxsubnet = (BigInteger.One << (this.StartEnd.subnetslash - this.StartEnd.slash));
            }
            else if (input.ID == 1)
            {
                if (IPv6SubnettingTool.Form1.ipmode == "v6")
                {
                    if (this.is128Checked == CheckState.Unchecked)
                    {
                        this.maxsubnet = (BigInteger.One << (64 - this.StartEnd.subnetslash));
                    }
                    else if (this.is128Checked == CheckState.Checked)
                    {
                        this.maxsubnet = (BigInteger.One << (128 - this.StartEnd.subnetslash));
                    }
                }
                else // v4
                {
                    this.maxsubnet = (BigInteger.One << (32 - this.StartEnd.subnetslash));
                }
            }
            else
            {
                return;
            }

            this.textBox4.Text = (maxsubnet - 1).ToString();

            ShowDiskInfo();

            if (IPv6SubnettingTool.Form1.ipmode == "v6")
            {
                if (this.is128Checked == CheckState.Unchecked && input.subnetslash == 64
                    ||
                    this.is128Checked == CheckState.Checked && input.subnetslash == 128
                    ||
                    this.incomingID == 1
                    ||
                    this.incomingID == 2
                    )
                {
                    this.checkBox1.Checked = false;
                    this.checkBox1.Enabled = false;
                }
                else
                    this.checkBox1.Enabled = true;
            }
            else // v4
            {
                if (input.subnetslash == 32 || this.incomingID == 1 || this.incomingID == 2)
                {
                    this.checkBox1.Checked = false;
                    this.checkBox1.Enabled = false;
                }
                else
                    this.checkBox1.Enabled = true;
            }

            if (input.ID == 0 || input.ID == 1)
                this.label1.Text = "(Prefixes)";
            else if (input.ID == 2)
                this.label1.Text = "(Reverse DNS)";

        }

        private void SaveAs_Click(object sender, EventArgs e)
        {
            this.backgroundWorker1.CancelAsync();
            this.backgroundWorker2.CancelAsync();
            this.progressBar1.Value = 0;

            if (this.textBox1.Text.Trim() == "" || this.textBox2.Text.Trim() == "")
            {
                this.label5.ForeColor = Color.Red;
                this.label5.Text = StringsDictionary.KeyValue("SaveAs_Click_e0", this.culture);
                return;
            }

            try
            {
                this.FromIndex = BigInteger.Parse(this.textBox1.Text, NumberStyles.Number);
            }
            catch
            {
                this.textBox1.Text = "";
                this.label5.ForeColor = Color.Red;
                this.label5.Text = StringsDictionary.KeyValue("SaveAs_Click_e0", this.culture);
                this.textBox1.Focus();
                return;
            }
            try
            {
                this.ToIndex = BigInteger.Parse(this.textBox2.Text, NumberStyles.Number);
            }
            catch
            {
                this.textBox2.Text = "";
                this.label5.ForeColor = Color.Red;
                this.label5.Text = StringsDictionary.KeyValue("SaveAs_Click_e0", this.culture);
                this.textBox2.Focus();
                return;
            }

            if (this.ToIndex > (maxsubnet - 1))
            {
                this.textBox2.BackColor = Color.FromKnownColor(KnownColor.Info);
                this.textBox2.SelectAll();
                this.label5.ForeColor = Color.Red;
                this.label5.Text = StringsDictionary.KeyValue("SaveAs_Click_e1", this.culture);
                return;
            }
            else if (this.ToIndex < this.FromIndex)
            {
                this.textBox2.BackColor = Color.FromKnownColor(KnownColor.Info);
                this.textBox2.SelectAll();
                this.label5.ForeColor = Color.Red;
                this.label5.Text = StringsDictionary.KeyValue("SaveAs_Click_e2", this.culture);
                return;
            }
            else
            {
                this.SaveAs.Enabled = false;
                this.textBox1.Enabled = false;
                this.textBox2.Enabled = false;
                this.label5.Text = "";

                StartEnd.subnetidx = this.FromIndex;
                this.TotalBytes = BigInteger.Zero;
                StartEnd.Start = StartEnd.LowerLimitAddress;
                StartEnd.End = StartEnd.UpperLimitAddress;

                string fnamestart = "";
                BigInteger OnceTotalBytes = BigInteger.Zero;
                BigInteger OnceDnsTotalBytes = BigInteger.Zero;

                if (IPv6SubnettingTool.Form1.ipmode == "v6")
                {
                    StartEnd = v6ST.GoToSubnet(this.StartEnd, this.is128Checked);

                    fnamestart = String.Format("{0:x}", this.StartEnd.Start);
                    fnamestart = fnamestart.TrimStart('0');

                    if (this.is128Checked == CheckState.Unchecked)
                    {
                        fnamestart = fnamestart.PadLeft(16, '0');
                    }
                    else if (this.is128Checked == CheckState.Checked)
                    {
                        fnamestart = fnamestart.PadLeft(32, '0');
                    }
                }
                else // v4
                {
                    StartEnd = v6ST.GoToSubnet_v4(this.StartEnd);

                    fnamestart = v6ST.IPv4Format(String.Format("{0:x}", this.StartEnd.Start));
                }

                saveDialog.Filter = "Text (*.wordpad)|*.wordpad|Text (*.txt)|*.txt";

                if (this.incomingID == 0 || this.incomingID == 1)  // (Form1 || ListSubnetRange)
                {
                    saveDialog.FileName = fnamestart + StringsDictionary.KeyValue("SaveAs_FileName_prefix", this.culture)
                        + this.StartEnd.slash + StringsDictionary.KeyValue("SaveAs_FileName_to", this.culture)
                        + this.StartEnd.subnetslash.ToString()
                        + StringsDictionary.KeyValue("SaveAs_FileName_index", this.culture)
                        + this.FromIndex.ToString() + StringsDictionary.KeyValue("SaveAs_FileName_to", this.culture)
                        + this.ToIndex.ToString();
                }
                else if (this.incomingID == 2) // ListDnsReverses
                {
                    OnceTotalBytes = OnceDnsTotalBytes;
                    saveDialog.FileName = StringsDictionary.KeyValue("SaveAs_FileName_ReverseDNS", this.culture)
                        + fnamestart + StringsDictionary.KeyValue("SaveAs_FileName_prefix", this.culture)
                        + this.StartEnd.subnetslash.ToString()
                        + StringsDictionary.KeyValue("SaveAs_FileName_index", this.culture)
                        + this.FromIndex.ToString()
                        + StringsDictionary.KeyValue("SaveAs_FileName_to", this.culture) + this.ToIndex.ToString();
                }

                this.textBox1.BackColor = Color.White;
                this.textBox2.BackColor = Color.White;

                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    this.progressBar1.Visible = true;
                    this.textBox5.Visible = true;
                    this.cancelButton.Enabled = true;
                    this.label5.Text = "";
                    this.label5.ForeColor = Color.RoyalBlue;

                    backgroundWorker1.RunWorkerAsync();
                    Thread.Sleep(7);
                }
                else
                {
                    this.SaveAs.Enabled = true;
                    this.cancelButton.Enabled = false;
                    this.textBox1.Enabled = true;
                    this.textBox2.Enabled = true;
                }
            }
        }

        void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.progressBar1.Hide();
            this.textBox5.Visible = false;
            StartEnd.Start = StartEnd.LowerLimitAddress;
            StartEnd.End = StartEnd.UpperLimitAddress;

            if (e.Cancelled)
            {
                this.backgroundWorker2.CancelAsync();

                this.label5.ForeColor = Color.Red;
                this.label5.Text = StringsDictionary.KeyValue("SaveAs_bgw_RunWorkerCompleted_label5", this.culture)
                    + saveState.percentage + "% )";

                if (File.Exists(this.saveDialog.FileName))
                {
                    try
                    {
                        File.Delete(this.saveDialog.FileName);
                    }
                    catch (SystemException ioex)
                    {
                        MessageBox.Show(StringsDictionary.KeyValue("SaveAs_bgw_RunWorkerCompleted_error", this.culture)
                            + ioex.Message,
                            StringsDictionary.KeyValue("SaveAs_bgw_RunWorkerCompleted_head_file", this.culture),
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                }
            }
            else if (e.Error != null)
            {
                this.backgroundWorker2.CancelAsync();

                MessageBox.Show(StringsDictionary.KeyValue("SaveAs_bgw_RunWorkerCompleted_error", this.culture)
                    + e.Error.Message, StringsDictionary.KeyValue("SaveAs_bgw_RunWorkerCompleted_head_error", this.culture),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);

                if (File.Exists(this.saveDialog.FileName))
                {
                    try
                    {
                        File.Delete(this.saveDialog.FileName);
                    }
                    catch (SystemException ioex)
                    {
                        MessageBox.Show(StringsDictionary.KeyValue("SaveAs_bgw_RunWorkerCompleted_error", this.culture)
                            + ioex.Message,
                            StringsDictionary.KeyValue("SaveAs_bgw_RunWorkerCompleted_head_file", this.culture),
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        return;
                    }
                }
            }
            else
            {
                this.backgroundWorker2.CancelAsync();

                this.label5.Text = "";
                this.label5.ForeColor = Color.RoyalBlue;
                this.label5.Text = StringsDictionary.KeyValue("SaveAs_bgw_RunWorkerCompleted_label5_1", this.culture)
                    + Environment.NewLine + saveState.SavedLines.ToString() + Environment.NewLine
                    + StringsDictionary.KeyValue("SaveAs_bgw_RunWorkerCompleted_label5_2", this.culture)
                    + Environment.NewLine + "("
                    + String.Format(CultureInfo.InvariantCulture, "{0:0,0}", this.TotalBytes)
                    + StringsDictionary.KeyValue("SaveAs_bgw_RunWorkerCompleted_label5_3", this.culture);

                this.SaveAs.Enabled = true;
                this.cancelButton.Enabled = false;
                this.textBox1.Enabled = true;
                this.textBox2.Enabled = true;
            }

            ShowDiskInfo();
        }

        void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.Value = e.ProgressPercentage;
            this.textBox5.Text = e.ProgressPercentage.ToString() + "%";

            if (this.backgroundWorker2.IsBusy == false)
                this.backgroundWorker2.RunWorkerAsync();
        }

        void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            if (StartEnd.Start > StartEnd.UpperLimitAddress)
                return;

            var saveAsName = new StreamWriter(saveDialog.FileName);
            string ss = "", se = "";
            howmany = (this.ToIndex - this.FromIndex + 1);
            int perc = 0;
            this.count = 0;
            this.TotalBytes = BigInteger.Zero;

            StartEnd.subnetidx = this.FromIndex;

            if (this.incomingID == 1)
            {
                if (IPv6SubnettingTool.Form1.ipmode == "v6")
                {
                    if (this.is128Checked == CheckState.Unchecked)
                        StartEnd.subnetslash = 64;
                    else if (this.is128Checked == CheckState.Checked)
                        StartEnd.subnetslash = 128;
                }
                else // v4
                {
                    StartEnd.subnetslash = 32;
                }
            }

            if (IPv6SubnettingTool.Form1.ipmode == "v6")
            {
                StartEnd = v6ST.GoToSubnet(this.StartEnd, this.is128Checked);
            }
            else // v4
            {
                StartEnd = v6ST.GoToSubnet_v4(this.StartEnd);
            }

            BigInteger i;
            for (i = 0; i < howmany; i++)
            {
                this.count++;

                if (backgroundWorker1.CancellationPending)
                {
                    this.count--;
                    e.Cancel = true;
                    break;
                }
                else // no cancel
                {
                    if (IPv6SubnettingTool.Form1.ipmode == "v6")
                    {
                        StartEnd = v6ST.Subnetting(StartEnd, this.is128Checked);

                        if (this.is128Checked == CheckState.Unchecked)
                        {
                            if (this.incomingID == 0 || this.incomingID == 1)
                            {
                                ss = String.Format("{0:x}", StartEnd.Start);
                                if (ss.Length > 16)
                                    ss = ss.Substring(1, 16);
                                ss = v6ST.Kolonlar(ss, this.is128Checked);
                                ss = v6ST.CompressAddress(ss);

                                se = String.Format("{0:x}", StartEnd.End);
                                if (se.Length > 16)
                                    se = se.Substring(1, 16);
                                se = v6ST.Kolonlar(se, this.is128Checked);
                                se = v6ST.CompressAddress(se);

                                if (this.SelectedRange)
                                    ss = "p" + StartEnd.subnetidx + "> " + ss + "/" + this.input_subnetslash;
                                else
                                    ss = "p" + StartEnd.subnetidx + "> " + ss + "/" + StartEnd.subnetslash;

                                TotalBytes += ss.Length + 2;
                                saveAsName.WriteLine(ss);

                                if (StartEnd.subnetslash != 64)
                                {
                                    if (this.checkBox1.CheckState == CheckState.Checked)
                                    {
                                        if (this.SelectedRange)
                                            se = "e" + StartEnd.subnetidx + "> " + se + "/" + this.input_subnetslash;
                                        else
                                            se = "e" + StartEnd.subnetidx + "> " + se + "/" + StartEnd.subnetslash;

                                        TotalBytes += se.Length + 4;
                                        saveAsName.WriteLine(se);
                                        saveAsName.WriteLine("");
                                    }
                                }
                            }
                            else if (this.incomingID == 2)
                            {
                                String[] sa;
                                int spaces = 0;

                                sa = v6ST.DnsRev(StartEnd.Start, StartEnd.subnetslash, this.is128Checked);
                                sa[0] = "p" + StartEnd.subnetidx + "> " + sa[0];
                                spaces = sa[0].Split(' ')[0].Length + 1;

                                for (int n = 0; n < 8; n++)
                                {
                                    if (sa[n] == null)
                                        break;
                                    if (n > 0)
                                        sa[n] = sa[n].PadLeft(sa[n].Length + spaces, ' ');

                                    TotalBytes += sa[n].Length + 2;
                                    saveAsName.WriteLine(sa[n]);
                                }
                            }
                        }
                        else if (this.is128Checked == CheckState.Checked)
                        {
                            if (this.incomingID == 0 || this.incomingID == 1)
                            {
                                ss = String.Format("{0:x}", StartEnd.Start);
                                if (ss.Length > 32)
                                    ss = ss.Substring(1, 32);
                                ss = v6ST.Kolonlar(ss, this.is128Checked);
                                ss = v6ST.CompressAddress(ss);

                                se = String.Format("{0:x}", StartEnd.End);
                                if (se.Length > 32)
                                    se = se.Substring(1, 32);
                                se = v6ST.Kolonlar(se, this.is128Checked);
                                se = v6ST.CompressAddress(se);

                                if (this.SelectedRange)
                                    ss = "p" + StartEnd.subnetidx + "> " + ss + "/" + this.input_subnetslash;
                                else
                                    ss = "p" + StartEnd.subnetidx + "> " + ss + "/" + StartEnd.subnetslash;

                                TotalBytes += ss.Length + 2;
                                saveAsName.WriteLine(ss);

                                if (StartEnd.subnetslash != 128)
                                {
                                    if (this.checkBox1.CheckState == CheckState.Checked)
                                    {
                                        if (this.SelectedRange)
                                            se = "e" + StartEnd.subnetidx + "> " + se + "/" + this.input_subnetslash;
                                        else
                                            se = "e" + StartEnd.subnetidx + "> " + se + "/" + StartEnd.subnetslash;
                                        TotalBytes += se.Length + 4;
                                        saveAsName.WriteLine(se);
                                        saveAsName.WriteLine("");
                                    }
                                }
                            }
                            else if (this.incomingID == 2)
                            {
                                String[] sa;
                                int spaces = 0;

                                sa = v6ST.DnsRev(StartEnd.Start, StartEnd.subnetslash, this.is128Checked);

                                sa[0] = "s" + StartEnd.subnetidx + "> " + sa[0];
                                spaces = sa[0].Split(' ')[0].Length + 1;

                                for (int n = 0; n < 8; n++)
                                {
                                    if (sa[n] == null)
                                        break;
                                    if (n > 0)
                                        sa[n] = sa[n].PadLeft(sa[n].Length + spaces, ' ');

                                    TotalBytes += sa[n].Length + 2;
                                    saveAsName.WriteLine(sa[n]);
                                }
                            }
                        }
                    }
                    else // v4 
                    {
                        StartEnd = v6ST.Subnetting_v4(StartEnd);

                        if (this.incomingID == 0 || this.incomingID == 1)
                        {
                            ss = String.Format("{0:x}", StartEnd.Start);
                            ss = v6ST.IPv4Format(ss);

                            se = String.Format("{0:x}", StartEnd.End);
                            se = v6ST.IPv4Format(se);

                            if (this.SelectedRange)
                                ss = "p" + StartEnd.subnetidx + "> " + ss + "/" + this.input_subnetslash;
                            else
                                ss = "p" + StartEnd.subnetidx + "> " + ss + "/" + StartEnd.subnetslash;

                            TotalBytes += ss.Length + 2;
                            saveAsName.WriteLine(ss);

                            if (StartEnd.subnetslash != 32)
                            {
                                if (this.checkBox1.CheckState == CheckState.Checked)
                                {
                                    if (this.SelectedRange)
                                        se = "e" + StartEnd.subnetidx + "> " + se + "/" + this.input_subnetslash;
                                    else
                                        se = "e" + StartEnd.subnetidx + "> " + se + "/" + StartEnd.subnetslash;

                                    TotalBytes += se.Length + 4;
                                    saveAsName.WriteLine(se);
                                    saveAsName.WriteLine("");
                                }
                            }
                        }
                        else if (this.incomingID == 2)
                        {
                            String[] sa;
                            int spaces = 0;
                            
                            sa = v6ST.DnsRev_v4(StartEnd.Start, StartEnd.subnetslash);

                            if (sa[0] != null)
                            {
                                sa[0] = "p" + StartEnd.subnetidx + "> " + sa[0];
                                spaces = sa[0].Split(' ')[0].Length + 1;

                                TotalBytes += sa[0].Length + 2;
                                saveAsName.WriteLine(sa[0]);

                                for (int n = 1; n < sa.Length; n++)
                                {
                                    if (sa[n] == null)
                                        break;

                                    //if (n > 0)
                                    sa[n] = sa[n].PadLeft(sa[n].Length + spaces, ' ');

                                    TotalBytes += sa[n].Length + 2;
                                    saveAsName.WriteLine(sa[n]);
                                }
                            }
                        }
                    }

                    if (StartEnd.Start == StartEnd.UpperLimitAddress
                        || StartEnd.subnetidx == (maxsubnet - 1))
                    {
                        break;
                    }
                    StartEnd.Start = StartEnd.End + BigInteger.One;

                    perc = (int)(i * 100 / howmany);

                    saveState.SavedLines = this.count;
                    saveState.percentage = perc;
                    this.backgroundWorker1.ReportProgress(perc);
                }
            }

            perc = (int)(i * 100 / howmany);
            saveState.SavedLines = this.count;
            saveState.percentage = perc;
            this.backgroundWorker1.ReportProgress(perc);
            saveAsName.Close();
        }

        void bgw2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        void bgw2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (this.backgroundWorker1.IsBusy == true)
                this.label5.Text = StringsDictionary.KeyValue("SaveAs_bgw2_ProgressChanged_label5", this.culture)
                    + this.count.ToString();
        }

        void bgw2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (this.backgroundWorker2.CancellationPending == true)
            {
                e.Cancel = true;
                return;
            }
            else
                backgroundWorker2.ReportProgress(1);
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim() == "")
            {
                this.label5.ForeColor = Color.Red;
                this.label5.Text = StringsDictionary.KeyValue("SaveAs_Click_e0", this.culture);
                return;
            }

            try
            {
                this.FromIndex = BigInteger.Parse(this.textBox1.Text, NumberStyles.Number);
            }
            catch
            {
                this.textBox1.Text = "";
                this.textBox1.Focus();
                this.label5.ForeColor = Color.Red;
                this.label5.Text = StringsDictionary.KeyValue("SaveAs_Click_e0", this.culture);
                return;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (this.textBox2.Text.Trim() == "")
            {
                this.label5.ForeColor = Color.Red;
                this.label5.Text = StringsDictionary.KeyValue("SaveAs_Click_e0", this.culture);
                return;
            }

            try
            {
                this.ToIndex = BigInteger.Parse(this.textBox2.Text, NumberStyles.Number);
            }
            catch
            {
                this.textBox2.Text = "";
                this.textBox2.Focus();
                this.label5.ForeColor = Color.Red;
                this.label5.Text = StringsDictionary.KeyValue("SaveAs_Click_e0", this.culture);
                return;
            }

            if (this.ToIndex > (maxsubnet - 1))
            {
                this.textBox2.BackColor = Color.FromKnownColor(KnownColor.Info);
                this.textBox4.BackColor = Color.FromKnownColor(KnownColor.Info);
                this.textBox2.SelectAll();
                this.label5.ForeColor = Color.Red;
                this.label5.Text = StringsDictionary.KeyValue("SaveAs_Click_e1", this.culture);
                return;
            }
            else if (this.ToIndex < this.FromIndex)
            {
                this.textBox2.BackColor = Color.FromKnownColor(KnownColor.Info);
                this.textBox2.SelectAll();
                this.label5.ForeColor = Color.Red;
                this.label5.Text = StringsDictionary.KeyValue("SaveAs_Click_e2", this.culture);
                return;
            }
            else
            {
                this.textBox4.BackColor = Color.FromKnownColor(KnownColor.Control);
                this.label5.Text = "";
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.textBox1.BackColor = Color.White;
            this.label5.Text = "";

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.textBox2.BackColor = Color.White;
            this.label5.Text = "";
            this.textBox4.BackColor = Color.FromKnownColor(KnownColor.Control);
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.backgroundWorker1.CancelAsync();

            this.SaveAs.Enabled = true;
            this.cancelButton.Enabled = false;
            this.textBox1.Enabled = true;
            this.textBox2.Enabled = true;

            ShowDiskInfo();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
            this.Close();
        }

        private void ExportToFile_FormClosing(object sender, FormClosingEventArgs e)
        {
            IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());

            if (this.backgroundWorker1.IsBusy || this.backgroundWorker2.IsBusy)
            {
                this.backgroundWorker1.CancelAsync();
                this.backgroundWorker2.CancelAsync();

                MessageBox.Show(StringsDictionary.KeyValue("SaveAs_ExportToFile_FormClosing_Msg", this.culture)
                    + saveState.percentage.ToString() + "% )",
                    StringsDictionary.KeyValue("SaveAs_ExportToFile_FormClosing_Msg_head", this.culture),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button1);

                if (File.Exists(this.saveDialog.FileName))
                {
                    try
                    {
                        File.Delete(this.saveDialog.FileName);
                    }
                    catch (SystemException ioex)
                    {
                        MessageBox.Show(StringsDictionary.KeyValue("SaveAs_bgw_RunWorkerCompleted_error", this.culture)
                            + ioex.Message,
                            StringsDictionary.KeyValue("SaveAs_bgw_RunWorkerCompleted_head_file", this.culture),
                            MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                        
                        return;
                    }
                }
            }
        }

        private void ShowDiskInfo()
        {
            this.listView1.Items.Clear();

            var alldisk = this.diskspace.GetDiskSpace();

            alldisk.ForEach(x =>
            {
                // Test:
                //string x2 = "970,345,344";
                //x2 = x2.Replace(",", "");

                string x2 = x.Item2.Replace(",", "");

                long size = long.Parse(x2) / (1024 * 1024); 

                if (size > (1000000))
                {
                    long Tera = 1099511627776; // = 1024 * 1024 * 1024 * 1024
                    size = long.Parse(x2) / Tera;
                    x2 = size.ToString() + " TB";
                }
                else if (size > 1000)
                {
                    size = long.Parse(x2) / (1024 * 1024 * 1024);
                    x2 = size.ToString() + " GB";
                }
                else
                    x2 = size.ToString() + " MB";

                string[] list = new string[] { x.Item1, x2 };
                this.listView1.Items.Add(new ListViewItem(list));
            });
        }

        public void SwitchLanguage(CultureInfo culture)
        {
            this.culture = culture;

            this.Text = StringsDictionary.KeyValue("SaveAsText_Form", this.culture);
            this.label4.Text = StringsDictionary.KeyValue("SaveAs_label4.Text", this.culture);
            //
            this.cancelButton.Text = StringsDictionary.KeyValue("SaveAs_cancelButton.Text", this.culture);
            this.columnHeader1.Text = StringsDictionary.KeyValue("SaveAs_columnHeader1.Text", this.culture);
            this.columnHeader2.Text = StringsDictionary.KeyValue("SaveAs_columnHeader2.Text", this.culture);
            this.exitButton.Text = StringsDictionary.KeyValue("SaveAs_exitButton.Text", this.culture);
            this.label10.Text = StringsDictionary.KeyValue("SaveAs_label10.Text", this.culture);
            this.label11.Text = StringsDictionary.KeyValue("SaveAs_label11.Text", this.culture);
            this.label12.Text = StringsDictionary.KeyValue("SaveAs_label12.Text", this.culture);
            this.label2.Text = StringsDictionary.KeyValue("SaveAs_label2.Text", this.culture);
            this.label3.Text = StringsDictionary.KeyValue("SaveAs_label3.Text", this.culture);
            this.label4.Text = StringsDictionary.KeyValue("SaveAs_label4.Text", this.culture);
            this.label5.Text = StringsDictionary.KeyValue("SaveAs_label5.Text", this.culture);
            this.label6.Text = StringsDictionary.KeyValue("SaveAs_label6.Text", this.culture);
            this.SaveAs.Text = StringsDictionary.KeyValue("SaveAs_SaveAs.Text", this.culture);
            this.textBox1.Text = StringsDictionary.KeyValue("SaveAs_textBox1.Text", this.culture);
            this.textBox2.Text = StringsDictionary.KeyValue("SaveAs_textBox2.Text", this.culture);
            this.checkBox1.Text = StringsDictionary.KeyValue("SaveAs_checkBox1.Text", this.culture);
            //
            this.toolTip1.SetToolTip(this.label8, StringsDictionary.KeyValue("SaveAs_label8.ToolTip", this.culture));
            this.toolTip1.SetToolTip(this.label9, StringsDictionary.KeyValue("SaveAs_label9.ToolTip", this.culture));
            this.toolTip1.SetToolTip(this.textBox4, StringsDictionary.KeyValue("SaveAs_textBox4.ToolTip", this.culture));
        }
    }
}
