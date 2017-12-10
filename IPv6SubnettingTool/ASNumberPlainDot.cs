/*
 * Copyright (c) 2010-2018 Yucel Guven
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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace IPv6SubnettingTool
{
    public partial class ASNumberPlainDot : Form
    {
        public CultureInfo culture;
        public delegate void ChangeWinFormStringsDelegate(CultureInfo culture);
        public event ChangeWinFormStringsDelegate ChangeUILanguage = delegate { };

        public ASNumberPlainDot(CultureInfo culture)
        {
            InitializeComponent();

            this.culture = culture;
            this.SwitchLanguage(this.culture);

        }

        public void SwitchLanguage(CultureInfo culture)
        {
            this.culture = culture;
            this.label1.Text = StringsDictionary.KeyValue("ASNumberPlainDotForm_label1.Text", this.culture);
            this.label2.Text = StringsDictionary.KeyValue("ASNumberPlainDotForm_label2.Text", this.culture);
            this.label3.Text = StringsDictionary.KeyValue("ASNumberPlainDotForm_label3.Text", this.culture);
            this.groupBox1.Text = StringsDictionary.KeyValue("ASNumberPlainDotForm_grbox.Text", this.culture);
            this.radioButton1.Text = StringsDictionary.KeyValue("ASNumberPlainDotForm_rb1.Text", this.culture);
            this.radioButton2.Text = StringsDictionary.KeyValue("ASNumberPlainDotForm_rb2.Text", this.culture);
            this.button1.Text = StringsDictionary.KeyValue("ASNumberPlainDotForm_button1.Text", this.culture);
            this.button2.Text = StringsDictionary.KeyValue("ASNumberPlainDotForm_button2.Text", this.culture);

            this.ChangeUILanguage.Invoke(this.culture);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox2.Clear(); // result
            String res = null;

            if (this.radioButton1.Checked)
            {
                res = v6ST.ConvertASnum(this.textBox1.Text.Trim(), v6ST.toASdot);
                if (res != null)
                {
                    this.textBox2.Text = res;
                }
                else
                {
                    //tfinput.requestFocus();
                    //tfresult.setText(v6ST.errmsg);
                    this.textBox2.Text = v6ST.errmsg;
                    this.textBox1.Focus(); this.textBox1.SelectAll();
                }
            }
            else if (this.radioButton2.Checked)
            {
                res = v6ST.ConvertASnum(this.textBox1.Text.Trim(), v6ST.toASplain);
                if (res != null)
                {
                    //tfresult.setText(res);
                    this.textBox2.Text = res;
                }
                else
                {
                    //tfinput.requestFocus();
                    //tfresult.setText(v6ST.errmsg);
                    this.textBox2.Text = v6ST.errmsg;
                    this.textBox1.Focus(); this.textBox1.SelectAll();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ASNumberPlainDot_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

    }
}
