/*
 * Copyright (c) 2010-2025 Yucel Guven
 * All rights reserved.
 * 
 * This file is part of IPv6 Subnetting Tool.
 * 
 * Version: 5.1
 * Release Date: 01 January 2025
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
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Numerics;

namespace IPv6SubnettingTool
{
    public partial class Goto : Form
    {
        #region special variables - yucel
        public BigInteger maxval = BigInteger.Zero;
        public int Itemidx = 1;
        int FormID = 0;
        private Form1 mainForm = null;
        private ListSubnetRange mainFormListSub = null;
        public CultureInfo culture;
        string currentMode = "";
        #endregion

        public Goto(Form callingForm, int idx, BigInteger max, int FormID, CultureInfo culture, string mode)
        {
            InitializeComponent();

            this.maxval = max;
            this.Itemidx = idx;
            this.FormID = FormID;
            this.currentMode = mode;

            if (FormID == 1)
            {
                mainFormListSub = callingForm as ListSubnetRange;
                this.label3.Text = this.textBox2.Text = " ";
            }
            else if (FormID == 0)
            {
                mainForm = callingForm as Form1;

                if (idx == 3)
                {
                    this.label3.Text = this.textBox2.Text = " ";
                }
                else
                {
                    this.textBox2.Text = maxval.ToString();
                }
            }

            this.culture = culture;
            this.SwitchLanguage(this.culture);

            if (idx == 1)
            {
                label1.Text = StringsDictionary.KeyValue("Goto_label1.Text1", this.culture);
            }
            else if (idx == 2)
            {
                label1.Text = StringsDictionary.KeyValue("Goto_label1.Text2", this.culture);
            }
            else if (idx == 3)
            {
                label1.Text = StringsDictionary.KeyValue("Goto_label1.Text3", this.culture);
            }
            else
                label1.Text = "";

            if (this.FormID == 0)
                this.textBox1.Text = mainForm.GotoForm_PrevValue;
            else if (this.FormID == 1)
                this.textBox1.Text = mainFormListSub.GotoForm_PrevValue;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.Itemidx == 3)
                return;

            this.textBox1.BackColor = Color.White;
            BigInteger gotovalue = BigInteger.Zero;

            if (e.KeyChar == (char)Keys.Enter || e.KeyChar == (char) Keys.Return )
            {
                textBox1.Text = textBox1.Text.Trim();
                
                if (textBox1.Text == "")
                    return;

                for (int i = 0; i < textBox1.Text.Length; i++)
                {
                    if (textBox1.Text[i] < '0' || textBox1.Text[i] > '9')
                    {
                        this.textBox1.Text = "0";
                        return;
                    }
                }

                gotovalue = BigInteger.Parse(this.textBox1.Text, NumberStyles.Number);

                if (gotovalue > maxval)
                {
                    this.textBox1.Text = maxval.ToString();
                    this.textBox1.BackColor = Color.FromKnownColor(KnownColor.Info);
                    this.textBox1.SelectAll();
                    return;
                }
                else
                {
                    if ( this.Itemidx == 1)
                        this.mainForm.gotoaddrvalue = gotovalue; // Form1.gotoaddrvalue{get; set textbox4.text= }
                    if (this.Itemidx == 2)
                        this.mainForm.gotosubnetvalue = gotovalue;

                    this.Close();
                }
            }

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) )
            {
                e.Handled = true;
            }
        }

        private void Goto_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                if (this.FormID == 0)
                {
                    if (this.Itemidx == 3)
                        mainForm.GotoForm_PrevValue = textBox1.Text.Trim();
                }
                else if (this.FormID == 1)
                {
                    mainFormListSub.GotoForm_PrevValue = textBox1.Text.Trim();
                }

                e.Handled = true;
                this.Close();
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BigInteger gotovalue = BigInteger.Zero;

            textBox1.Text = textBox1.Text.Trim();

            if (this.FormID == 0)
                mainForm.GotoForm_PrevValue = textBox1.Text;
            else if (this.FormID == 1)
                mainFormListSub.GotoForm_PrevValue = textBox1.Text;

            if (textBox1.Text == "")
                return;

            if (this.Itemidx == 3) // idx=3 findprefix()
            {
                if (this.currentMode == "v6")
                {
                    if (v6ST.IsAddressCorrect(this.textBox1.Text))
                    {
                        if (this.FormID == 0)
                            mainForm.findpfx = this.textBox1.Text;
                        else if (this.FormID == 1)
                            mainFormListSub.findpfx = this.textBox1.Text;

                        this.Close();
                    }
                    else
                    {
                        label1.Text = StringsDictionary.KeyValue("Form1_" + v6ST.errmsg, this.culture);
                        return;
                    }
                }
                else // v4
                {
                    if (v6ST.IsAddressCorrect_v4(this.textBox1.Text))
                    {
                        if (this.FormID == 0)
                            mainForm.findpfx = this.textBox1.Text;
                        else if (this.FormID == 1)
                            mainFormListSub.findpfx = this.textBox1.Text;

                        this.Close();
                    }
                    else
                    {
                        label1.Text = StringsDictionary.KeyValue("Form1_" + v6ST.errmsg, this.culture);
                        return;
                    }
                }
            }

            else // idx=1 or 2
            {
                for (int i = 0; i < textBox1.Text.Length; i++)
                {
                    if (textBox1.Text[i] < '0' || textBox1.Text[i] > '9')
                    {
                        this.textBox1.Text = "0";
                        return;
                    }
                }

                gotovalue = BigInteger.Parse(this.textBox1.Text, NumberStyles.Number);

                if (gotovalue > maxval)
                {
                    this.textBox1.Text = maxval.ToString();
                    this.textBox1.BackColor = Color.FromKnownColor(KnownColor.Info);
                    this.textBox1.SelectAll();
                    return;
                }
                else
                {
                    if (this.Itemidx == 1)
                        this.mainForm.gotoaddrvalue = gotovalue; // Form1.gotoaddrvalue{get; set textbox4.text= }
                    if (this.Itemidx == 2)
                        this.mainForm.gotosubnetvalue = gotovalue;

                    this.Close();
                }
            }
        }

        public void SwitchLanguage(CultureInfo culture)
        {
            this.culture = culture;

            this.Text = StringsDictionary.KeyValue("Goto_Form.Text", this.culture);
            this.button1.Text = StringsDictionary.KeyValue("Goto_button1.Text", this.culture);
            
            if (this.Itemidx != 3)
                this.label3.Text = StringsDictionary.KeyValue("Goto_label3.Text", this.culture);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.FormID == 0)
            {
                mainForm.findpfx = "";
                mainForm.GotoForm_PrevValue = textBox1.Text;
            }
            else if (this.FormID == 1)
            {
                mainFormListSub.findpfx = "";
                mainFormListSub.GotoForm_PrevValue = textBox1.Text;
            }
            this.Close();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (this.Itemidx == 3 || FormID == 1)
                label1.Text = StringsDictionary.KeyValue("Form1_textBox2_Enter", this.culture);
        }
    }
}
