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
using System.Globalization;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace IPv6SubnettingTool
{
    public partial class UpdateServiceNamesPortNumbers : Form
    {
        ServiceNamesPortNumbers mainForm = null;
        string default_url_iana = "https://www.iana.org/assignments/service-names-port-numbers/service-names-port-numbers.xml";
        string fileName = ""; // parsed below

        public CultureInfo culture;

        public UpdateServiceNamesPortNumbers(Form callingForm, CultureInfo culture)
        {
            InitializeComponent();
            //
            this.culture = culture;
            this.mainForm = callingForm as ServiceNamesPortNumbers;
            this.textBox1.Text = this.default_url_iana;

            this.SwitchLanguage(this.culture);
        }
        private void Button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim() == "")
            {
                MessageBox.Show("Address can not be blank", "Blank address", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string[] fname = this.textBox1.Text.Trim().Split('/');
            this.fileName = fname[fname.Length - 1];    // last part of the URL is the filename

            this.progressBar1.Visible = true;
            this.label3.Visible = true;
            this.label3.Text = StringsDictionary.KeyValue("UpdateServiceNamesPortNumbers_label3.Text", this.culture);

            this.backgroundWorker1.RunWorkerAsync();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.backgroundWorker1.CancelAsync();

            this.Close();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            bool r = false;

            do
            {
                if (this.backgroundWorker1.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                
                r = ConnectAndDownload();

            } while (r);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBar1.PerformStep();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.progressBar1.Value = 0;
            this.progressBar1.Visible = false;
            this.label3.Text = "";
            this.label3.Visible = false;


            this.Close();
        }

        private bool ConnectAndDownload()
        {
            try
            {
                HttpWebRequest webreq = WebRequest.Create(this.textBox1.Text.Trim()) as HttpWebRequest;
                webreq.UserAgent = @"Mozilla/5.0 Gecko/20100101 Firefox/75.0";
                webreq.ContentType = "text/xml;charset=\"utf-8\"";
                webreq.Accept = "text/xml";

                mainForm.xmlstring = "";

                using (HttpWebResponse response = webreq.GetResponse() as HttpWebResponse)
                {
                    StreamReader responseReader = new StreamReader(response.GetResponseStream());
                    mainForm.xmlstring = responseReader.ReadToEnd();
                }

                File.WriteAllText(this.fileName, mainForm.xmlstring);
                mainForm.fileName = this.fileName;
            }
            catch (Exception ex)
            {
                DialogResult resp =
                    MessageBox.Show("Error:\r\n" + ex.Message, "Error",
                    MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);

                if (resp == DialogResult.Retry)
                {
                    return true; // Retry
                }
                else
                {
                    return false; // Cancel
                }
            }

            return false;
        }

        private void UpdateInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        public void SwitchLanguage(CultureInfo culture)
        {
            this.culture = culture;
            this.Text = StringsDictionary.KeyValue("UpdateServiceNamesPortNumbers_header.Text", this.culture);
            this.Text = StringsDictionary.KeyValue("UpdateServiceNamesPortNumbers_header.Text", this.culture);
            this.label1.Text = StringsDictionary.KeyValue("UpdateServiceNamesPortNumbers_label1.Text", this.culture);
            this.label2.Text = StringsDictionary.KeyValue("UpdateServiceNamesPortNumbers_label2.Text", this.culture);
            this.label3.Text = StringsDictionary.KeyValue("UpdateServiceNamesPortNumbers_label3.Text", this.culture);
            this.button1.Text = StringsDictionary.KeyValue("UpdateServiceNamesPortNumbers_button1.Text", this.culture);
            this.button2.Text = StringsDictionary.KeyValue("UpdateServiceNamesPortNumbers_button2.Text", this.culture);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                this.Button1_Click(null, null);
                e.SuppressKeyPress = true;
            }
        }
    }
}
