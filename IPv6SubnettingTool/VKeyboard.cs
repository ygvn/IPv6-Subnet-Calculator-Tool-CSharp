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
using System.Globalization;
using System.Windows.Forms;

namespace IPv6SubnettingTool
{
    public partial class VKeyboard : Form
    {
        Form1 mainForm = null;
        public CultureInfo culture;

        public VKeyboard(Form callingForm, CultureInfo culture)
        {
            InitializeComponent();
            //
            this.mainForm = callingForm as Form1;
            this.culture = culture;
            this.SwitchLanguage(this.culture);
        }

        private void AllButtons_Clicked(Object sender, EventArgs e)
        {
            if ((Button)sender == this.button13) // Backspace
            {
                if (this.mainForm.textBox2.Text.Length > 0)
                    this.mainForm.textBox2.Text = this.mainForm.textBox2.Text.Remove(this.mainForm.textBox2.Text.Length - 1, 1);
            }
            else if ((Button)sender == this.button14) // Clear All
            {
                this.mainForm.textBox2.Clear();
            }
            else if ((Button)sender == this.button21) // Close
            {
                IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
                this.Close();
            }
            else
            {
                if (this.mainForm.textBox2.Text.Length == 39)
                    return;
                else
                    this.mainForm.textBox2.Text += ((Button)sender).Text.ToLower()[0];
            }
        }

        public void SwitchLanguage(CultureInfo culture)
        {
            this.culture = culture;
            this.button13.Text = StringsDictionary.KeyValue("VKeyboard_button13", this.culture);
            this.button14.Text = StringsDictionary.KeyValue("VKeyboard_button14", this.culture);
            this.button21.Text = StringsDictionary.KeyValue("VKeyboard_button21", this.culture);
        }

        private void VKeyboard_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Escape) || (e.Alt && e.KeyCode == Keys.C))
            {
                IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
                this.Close();
            }
        }

        private void VKeyboard_FormClosing(object sender, FormClosingEventArgs e)
        {
            IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
        }
    }
}
