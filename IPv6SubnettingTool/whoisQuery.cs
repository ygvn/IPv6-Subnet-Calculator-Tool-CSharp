/*
 * Copyright (c) 2010-2020 Yucel Guven
 * All rights reserved.
 * 
 * This file is part of IPv6 Subnetting Tool.
 * 
 * Version: 4.3
 * Published Date: 28 January 2020
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
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.IO;
using System.Globalization;

namespace IPv6SubnettingTool
{
    public partial class whoisQuery : Form
    {
        #region special initials/constants -yucel
        private int defaultport = 43; // service name is 'nicname'
        private BackgroundWorker bgw = new BackgroundWorker();
        private string server = null;
        private string key = null;
        private StringBuilder whois_response = new StringBuilder();
        private AutoCompleteStringCollection autocomp = new AutoCompleteStringCollection();
        private CultureInfo culture;
        #endregion

        public whoisQuery(string sinput, CultureInfo culture)
        {
            InitializeComponent();
            //
            this.comboBox1.SelectedIndex = 0;
            this.textBox1.Text = sinput;
            this.culture = culture;

            this.SwitchLanguage(this.culture);
        }

        private void buttonWhoisQuery_Click(object sender, EventArgs e)
        {
            autocomp.Add(this.textBox1.Text);
            this.textBox2.Text = "";
            this.buttonWhoisQuery.Enabled = false;
            this.progressBar1.Visible = true;
            key = this.textBox1.Text;

            bgw.DoWork += new DoWorkEventHandler(bgw_DoWork);
            bgw.ProgressChanged += new ProgressChangedEventHandler(bgw_ProgressChanged);
            bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgw_RunWorkerCompleted);
            bgw.WorkerReportsProgress = true;
            bgw.RunWorkerAsync();
        }

        void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.progressBar1.Hide();
            this.textBox2.Text = StringsDictionary.KeyValue("whois_bgw_RunWorkerCompleted_textBox2.Text", this.culture)
                + this.textBox1.Text + "\r\n******\r\n" + whois_response.ToString();
            this.buttonWhoisQuery.Enabled = true;
        }

        void bgw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.MarqueeAnimationSpeed = 100;
        }

        void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            WhoisQ(server, defaultport);
        }

        private void WhoisQ(string server, int port)
        {
            whois_response.Clear();
            try
            {
                /* Create a tcp client for remote server connection: 
                 * (Note: If you don't have internet connection, you'll get exception at this point
                 *   since servername cannot be resolved, but this should not cause any problem.
                 *   Exception is handled/catched below and error message is displayed to the user)
                 */
                TcpClient tcp_client = new TcpClient(server, port);

                /* Streams for Reading/Writting purposes */
                NetworkStream ns = tcp_client.GetStream();
                BufferedStream buf = new BufferedStream(ns);
                StreamWriter writetostream = new StreamWriter(buf);

                writetostream.WriteLine(key);
                writetostream.Flush();

                StreamReader readfromstream = new StreamReader(buf);
                while (!readfromstream.EndOfStream)
                {
                    whois_response.AppendLine(readfromstream.ReadLine());
                }

                /* Close the streams */
                writetostream.Close();
                readfromstream.Close();
                buf.Close();
                ns.Close();
                tcp_client.Close();
            }
            catch (SocketException ex)
            {
                whois_response.AppendLine("\r\n" + StringsDictionary.KeyValue("whois_WhoisQ_appendline1", this.culture)
                    + "\r\n" + StringsDictionary.KeyValue("whois_WhoisQ_appendline2", this.culture)
                    + "\r\n\r\n" + StringsDictionary.KeyValue("whois_WhoisQ_appendline3", this.culture)
                    + "\r\n********\r\n");
                whois_response.AppendLine(ex.ToString());
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            server = this.comboBox1.SelectedItem.ToString().Split(' ')[1];
        }

        private void whoisQuery_Load(object sender, EventArgs e)
        {
            this.textBox1.AutoCompleteCustomSource = autocomp;
        }

        private void whoisQuery_KeyDown(object sender, KeyEventArgs e)
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

            this.Text = StringsDictionary.KeyValue("whois_Form.Text", this.culture);
            //
            this.buttonWhoisQuery.Text = StringsDictionary.KeyValue("whois_buttonWhoisQuery.Text", this.culture);
            this.label1.Text = StringsDictionary.KeyValue("whois_label1.Text", this.culture);
            this.label2.Text = StringsDictionary.KeyValue("whois_label2.Text", this.culture);
        }

        private void whoisQuery_FormClosing(object sender, FormClosingEventArgs e)
        {
            IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
        }
    }
}
