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
using System.Windows.Forms;
using System.Numerics;
using System.Globalization;

namespace IPv6SubnettingTool
{
    public partial class CompressAddress : Form
    {
        SEaddress seaddress = new SEaddress();
        public CultureInfo culture;
        public delegate void ChangeWinFormStringsDelegate(CultureInfo culture);
        public event ChangeWinFormStringsDelegate ChangeUILanguage = delegate { };

        public CompressAddress(CultureInfo culture)
        {
            InitializeComponent();

            this.culture = culture;
            this.SwitchLanguage(this.culture);
        }

        public void SwitchLanguage(CultureInfo culture)
        {
            this.culture = culture;
            this.Text = StringsDictionary.KeyValue("CompressForm.Text", this.culture);
            this.label1.Text = StringsDictionary.KeyValue("CompressForm_label1.Text", this.culture);
            this.label2.Text = StringsDictionary.KeyValue("CompressForm_label2.Text", this.culture);
            this.label4.Text = StringsDictionary.KeyValue("CompressForm_label4.Text", this.culture);
            this.button1.Text = StringsDictionary.KeyValue("CompressForm_button1.Text", this.culture);
            //
            this.ChangeUILanguage.Invoke(this.culture);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            textBox1.Text = textBox1.Text.Trim();
            this.Calculate(textBox1.Text);
        }

        private void Calculate(string sin)
        {
            if (v6ST.IsAddressCorrect(textBox1.Text))
            {
                label3.Text = StringsDictionary.KeyValue("Form1_" + v6ST.errmsg, this.culture);
                string Resv6 = v6ST.FormalizeAddr(sin);
                
                // :-) full
                string veryformal = v6ST.Kolonlar(Resv6, CheckState.Checked);
                
                textBox2.Text = v6ST.CompressAddress(veryformal);
                
                string[] formal = veryformal.Split(':');
                textBox3.Clear();
                foreach (string s in formal)
                    textBox3.Text +=
                        UInt16.Parse(s, System.Globalization.NumberStyles.AllowHexSpecifier).ToString("x")
                        + ":";
                textBox3.Text = textBox3.Text.TrimEnd(':');

                seaddress.ResultIPAddr = seaddress.Start =
                    BigInteger.Parse("0" + Resv6, NumberStyles.AllowHexSpecifier);
                textBox4.Text = v6ST.PrintBin(seaddress, 128, CheckState.Checked).Replace(':', ' ');
                textBox5.Text = "0x" + Resv6;
                textBox6.Text = v6ST.DnsRev(seaddress.ResultIPAddr, 128, CheckState.Checked)[0];
                textBox8.Text = seaddress.ResultIPAddr.ToString();
                textBox9.Text = veryformal;
            }
            else
            {
                label3.Text = StringsDictionary.KeyValue("Form1_" + v6ST.errmsg, this.culture);
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
                textBox6.Clear();
                textBox8.Clear();
                textBox9.Clear();
            }
        }

        private void CompressAddressForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
                this.Close();
            }
        }

        private void CompressAddressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
        }
    }
}
