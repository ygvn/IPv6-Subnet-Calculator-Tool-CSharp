/*
 * Copyright (c) 2010-2020 Yucel Guven
 * All rights reserved.
 * 
 * This file is part of IPv6 Subnetting Tool.
 * 
 * Version: 4.2
 * Published Date: 7 January 2020
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
using System.Diagnostics;
using System.Windows.Forms;

namespace IPv6SubnettingTool
{
    public partial class AddressTypeInfo : Form
    {
        const string urlRFC = "http://tools.ietf.org/html/rfc";
        const string urlIANA = "http://www.iana.org/assignments/ipv6-multicast-addresses/ipv6-multicast-addresses.xhtml";
        const string urlLink = "http://tools.ietf.org/html/rfc6890";

        public AddressTypeInfo(AttributeValues info, string addr)
        {
            InitializeComponent();
            //

            this.label3.Text = v6ST.CompressAddress(addr) + "/" + info.SelectedPrefixLength.ToString();

            this.dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            this.dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridView1.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            if (info.Name == "Global Unicast")
            {
                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[0].Cells["Column1"].Value = "Name: ";
                this.dataGridView1.Rows[0].Cells["Column2"].Value = info.Name;
                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[1].Cells["Column1"].Value = "SelectedPrefixLength: ";
                this.dataGridView1.Rows[1].Cells["Column2"].Value = info.SelectedPrefixLength.ToString();
                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[2].Cells["Column1"].Value = "RFC: ";
                this.dataGridView1.Rows[2].Cells["Column2"] = new DataGridViewLinkCell();
                this.dataGridView1.Rows[2].Cells["Column2"].Value = info.RFC;
                this.dataGridView1.Rows[2].Cells["Column2"].ToolTipText = urlRFC + info.RFC;
            }
            else if (info.isMulticast)
            {
                this.dataGridView1.Rows.Add();
                
                this.dataGridView1.Rows[0].Cells["Column1"].Value = "Name: ";
                this.dataGridView1.Rows[0].Cells["Column2"].Value = info.Name;
                if (info.Name.Contains("IANA"))
                {
                    this.dataGridView1.Rows[0].Cells["Column2"] = new DataGridViewLinkCell();
                    this.dataGridView1.Rows[0].Cells["Column2"].Value = info.Name;
                    this.dataGridView1.Rows[0].Cells["Column2"].ToolTipText = urlIANA;
                }

                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[1].Cells["Column1"].Value = "Address Block: ";
                this.dataGridView1.Rows[1].Cells["Column2"].Value = info.strAddressBlock;

                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[2].Cells["Column1"].Value = "AssignedPrefix: ";
                this.dataGridView1.Rows[2].Cells["Column2"].Value = info.AssignedPrefixLength.ToString();


                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[3].Cells["Column1"].Value = "SelectedPrefixLength: ";
                this.dataGridView1.Rows[3].Cells["Column2"].Value = info.SelectedPrefixLength.ToString();
                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[4].Cells["Column1"].Value = "RFC: ";
                this.dataGridView1.Rows[4].Cells["Column2"] = new DataGridViewLinkCell();
                this.dataGridView1.Rows[4].Cells["Column2"].Value = info.RFC;
                this.dataGridView1.Rows[4].Cells["Column2"].ToolTipText = urlRFC + info.RFC;

                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[5].Cells["Column1"].Value = "Allocation Date: ";
                this.dataGridView1.Rows[5].Cells["Column2"].Value = info.AllocationDate;
                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[6].Cells["Column1"].Value = "Termination Date: ";
                this.dataGridView1.Rows[6].Cells["Column2"].Value = info.TerminationDate;
            }
            else
            {
                if (info.Name == "Unspecified Address" && info.AssignedPrefixLength != 128)
                {
                    this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[0].Cells["Column1"].Value = "Name: ";
                    this.dataGridView1.Rows[0].Cells["Column2"].Value = info.Name;
                    this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[1].Cells["Column1"].Value = "Address Block: ";
                    this.dataGridView1.Rows[1].Cells["Column2"].Value = info.strAddressBlock;

                    this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[2].Cells["Column1"].Value = "AssignedPrefix: ";
                    this.dataGridView1.Rows[2].Cells["Column2"].Value = info.AssignedPrefixLength.ToString();

                    this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[3].Cells["Column1"].Value = "SelectedPrefixLength: ";
                    this.dataGridView1.Rows[3].Cells["Column2"].Value = info.SelectedPrefixLength.ToString();

                    this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[4].Cells["Column1"].Value = "RFC: ";
                    this.dataGridView1.Rows[4].Cells["Column2"] = new DataGridViewLinkCell();
                    this.dataGridView1.Rows[4].Cells["Column2"].Value = info.RFC;
                    this.dataGridView1.Rows[4].Cells["Column2"].ToolTipText = urlRFC + info.RFC;
                }
                else
                {
                    this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[0].Cells["Column1"].Value = "Name: ";
                    this.dataGridView1.Rows[0].Cells["Column2"].Value = info.Name;
                    this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[1].Cells["Column1"].Value = "Address Block: ";
                    this.dataGridView1.Rows[1].Cells["Column2"].Value = info.strAddressBlock;

                    this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[2].Cells["Column1"].Value = "AssignedPrefix: ";
                    this.dataGridView1.Rows[2].Cells["Column2"].Value = info.AssignedPrefixLength.ToString();

                    this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[3].Cells["Column1"].Value = "SelectedPrefixLength: ";
                    this.dataGridView1.Rows[3].Cells["Column2"].Value = info.SelectedPrefixLength.ToString();

                    this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[4].Cells["Column1"].Value = "RFC: ";
                    this.dataGridView1.Rows[4].Cells["Column2"] = new DataGridViewLinkCell();
                    this.dataGridView1.Rows[4].Cells["Column2"].Value = info.RFC;
                    this.dataGridView1.Rows[4].Cells["Column2"].ToolTipText = urlRFC + info.RFC;
                    this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[5].Cells["Column1"].Value = "Allocation Date: ";
                    this.dataGridView1.Rows[5].Cells["Column2"].Value = info.AllocationDate;
                    this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[6].Cells["Column1"].Value = "Termination Date: ";
                    this.dataGridView1.Rows[6].Cells["Column2"].Value = info.TerminationDate;
                    this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[7].Cells["Column1"].Value = "Source: ";
                    this.dataGridView1.Rows[7].Cells["Column2"].Value = info.Source;
                    this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[8].Cells["Column1"].Value = "Destination: ";
                    this.dataGridView1.Rows[8].Cells["Column2"].Value = info.Destination;
                    this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[9].Cells["Column1"].Value = "Forwardable: ";
                    this.dataGridView1.Rows[9].Cells["Column2"].Value = info.Forwardable;
                    this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[10].Cells["Column1"].Value = "Global: ";
                    this.dataGridView1.Rows[10].Cells["Column2"].Value = info.Global;
                    this.dataGridView1.Rows.Add();
                    this.dataGridView1.Rows[11].Cells["Column1"].Value = "Reserved-by-Protocol: ";
                    this.dataGridView1.Rows[11].Cells["Column2"].Value = info.ReservedByProtocol;
                }
            }
        }

        private void AddressNameInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
                this.Close();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewLinkCell)
                {
                    if (this.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString().Contains("IANA"))
                        Process.Start(urlIANA);
                    else
                    {
                        string url = urlRFC;
                        url += this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                        Process.Start(url);
                    }
                }
            }
        }

        private void SelectAlltoolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dataGridView1.SelectAll();
        }

        private void CopytoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.GetCellCount(DataGridViewElementStates.Selected) > 0)
            {
                try
                {
                    Clipboard.SetDataObject(this.dataGridView1.GetClipboardContent());
                }
                catch (System.Runtime.InteropServices.ExternalException ex)
                {
                    MessageBox.Show("Clipboard error: " + ex.ToString(), "Clipboard Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void AddressTypeInfo_FormClosing(object sender, FormClosingEventArgs e)
        {
            IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(urlLink);
        }

    }
}
