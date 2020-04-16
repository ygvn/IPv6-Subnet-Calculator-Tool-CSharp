/*
 * Copyright (c) 2010-2020 Yucel Guven
 * All rights reserved.
 * 
 * This file is part of IPv6 Subnetting Tool.
 * 
 * Version: 4.5
 * Release Date: 16 April 2020
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
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Xml;

namespace IPv6SubnettingTool
{
    public partial class ServiceNamesPortNumbers : Form
    {
        Form1 mainForm = null;
        public CultureInfo culture;

        XmlDocument xmldoc = new XmlDocument();
        XmlNodeList nodes;
        BindingSource source1 = new BindingSource();

        public string xmlstring = "";
        public string fileName = "service-names-port-numbers.xml"; // default file name from IANA web page

        public delegate void ChangeWinFormStringsDelegate(CultureInfo culture);
        public event ChangeWinFormStringsDelegate ChangeUILanguage = delegate { };

        public ServiceNamesPortNumbers(Form callingForm, CultureInfo culture)
        {
            InitializeComponent();
            //
            this.mainForm = callingForm as Form1;
            this.culture = culture;
            this.SwitchLanguage(this.culture);

            StartUp();
        }

        private void StartUp()
        {
            this.source1.RemoveFilter();

            try
            {
                this.toolStripStatusLabel2.Text = StringsDictionary.KeyValue("ServiceNamesPortNumbers_toolStripStatusLabel2.Text", this.culture)
                    + this.fileName;
                
                xmldoc.Load(this.fileName);
                nodes = xmldoc.GetElementsByTagName("record");

                DataTable dt = new DataTable();
                dt = CreateDatatableFromXmlList(nodes);
                DataView view1 = new DataView(dt);

                source1.DataSource = view1;

                dataGridView1.DataSource = source1;

                DataTableMakeUp();

                this.toolStripStatusLabel1.Text = StringsDictionary.KeyValue("ServiceNamesPortNumbers_toolStripStatusLabel1.Text", this.culture) 
                    + this.dataGridView1.RowCount.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(StringsDictionary.KeyValue("ServiceNamesPortNumbers_err1.Text", this.culture)
                    + this.fileName + "\r\n\r\n"
                    + StringsDictionary.KeyValue("ServiceNamesPortNumbers_err2.Text", this.culture) 
                    + "\r\n\r\n" + ex.Message + "\r\n\r\n",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
        }

        public static DataTable CreateDatatableFromXmlList(XmlNodeList xnl)
        {
            DataTable dt = new DataTable();
            DataColumn dc = null;

            // index=0
            dc = new DataColumn("ServiceName", typeof(string));
            dt.Columns.Add(dc);
            // index=1
            dc = new DataColumn("PortNumber", System.Type.GetType("System.String"));
            dt.Columns.Add(dc);
            // index=2
            dc = new DataColumn("TransportProtocol", System.Type.GetType("System.String"));
            dt.Columns.Add(dc);
            // index=3
            dc = new DataColumn("Description", System.Type.GetType("System.String"));
            dt.Columns.Add(dc);

            foreach (XmlNode node in xnl)
            {
                DataRow dr = dt.NewRow();

                foreach (XmlNode n in node.ChildNodes)
                {
                    switch (n.Name)
                    {
                        case "name":
                            {
                                dr[0] = n.InnerText;
                                break;
                            }
                        case "number":
                            {
                                dr[1] = n.InnerText;
                                break;
                            }
                        case "protocol":
                            {
                                dr[2] = n.InnerText;
                                break;
                            }
                        case "description":
                            {
                                dr[3] = n.InnerText;
                                break;
                            }
                        default:
                            break;
                    }
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        private void DataTableMakeUp()
        {
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView1.Columns[1].Width = 85;
            dataGridView1.Columns[3].Width = 280;

            dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.Font = new Font(dataGridView1.Font, FontStyle.Bold);

            int rowCount = dataGridView1.RowCount;

            for (int i = 0; i < rowCount; i++)
            {
                this.dataGridView1.Columns[1].DefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.dataGridView1.SelectAll();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SendKeys.Send("^c");
        }

        private void OnlineUpdatetoolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateServiceNamesPortNumbers updateInfo = new UpdateServiceNamesPortNumbers(this, this.culture);
            this.ChangeUILanguage += updateInfo.SwitchLanguage;
            updateInfo.ShowDialog();

            StartUp();
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            string rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string skey = this.textBox1.Text.Trim();
            if (skey == "" || skey == null)
            {
                source1.RemoveFilter();

                this.toolStripStatusLabel1.Text = StringsDictionary.KeyValue("ServiceNamesPortNumbers_toolStripStatusLabel1.Text", this.culture)
                    + this.dataGridView1.RowCount.ToString();

                return;
            }
            this.textBox2.Clear();

            string strFormat = "ServiceName LIKE '{0}%'"; // name Starts with

            if (this.checkBox1.Checked)
            {
                strFormat = "ServiceName LIKE '%{0}%'"; // including name
            }

            source1.Filter = string.Format(strFormat, skey);

            this.toolStripStatusLabel1.Text = StringsDictionary.KeyValue("ServiceNamesPortNumbers_toolStripStatusLabel1.Text", this.culture)
                + this.dataGridView1.RowCount.ToString();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            string skey = this.textBox2.Text.Trim();
            if (skey == "" || skey == null)
            {
                source1.RemoveFilter();

                this.toolStripStatusLabel1.Text = StringsDictionary.KeyValue("ServiceNamesPortNumbers_toolStripStatusLabel1.Text", this.culture)
                    + this.dataGridView1.RowCount.ToString();

                return;
            }
            this.textBox1.Clear();

            //string strFormat = "PortNumber LIKE '{0}%'"; // number Starts with
            string strFormat = "PortNumber = '{0}'";       // exact match

            if (this.checkBox1.Checked)
            {
                strFormat = "PortNumber LIKE '%{0}%'"; // including number
            }

            source1.Filter = string.Format(strFormat, skey);

            this.toolStripStatusLabel1.Text = StringsDictionary.KeyValue("ServiceNamesPortNumbers_toolStripStatusLabel1.Text", this.culture)
                + this.dataGridView1.RowCount.ToString();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim() == "")
                textBox2_TextChanged(null, null);
            else if (this.textBox2.Text.Trim() == "")
                textBox1_TextChanged(null, null);
        }

        public void SwitchLanguage(CultureInfo culture)
        {
            this.culture = culture;

            this.Text = StringsDictionary.KeyValue("ServiceNamesPortNumbers_header.Text", this.culture);
            this.fileToolStripMenuItem.Text = StringsDictionary.KeyValue("ServiceNamesPortNumbers_fileToolStripMenuItem.Text", this.culture);
            this.exitToolStripMenuItem.Text = StringsDictionary.KeyValue("ServiceNamesPortNumbers_exitToolStripMenuItem.Text", this.culture);
            this.updateToolStripMenuItem.Text = StringsDictionary.KeyValue("ServiceNamesPortNumbers_updateToolStripMenuItem.Text", this.culture);
            this.OnlineUpdatetoolStripMenuItem.Text = StringsDictionary.KeyValue("ServiceNamesPortNumbers_OnlineUpdatetoolStripMenuItem.Text", this.culture);
            this.checkBox1.Text = StringsDictionary.KeyValue("ServiceNamesPortNumbers_checkBox1.Text", this.culture);
            this.label2.Text = StringsDictionary.KeyValue("ServiceNamesPortNumbers_label2.Text", this.culture);
            this.toolStripStatusLabel1.Text = StringsDictionary.KeyValue("ServiceNamesPortNumbers_toolStripStatusLabel1.Text", this.culture)
                + this.dataGridView1.RowCount.ToString();
            this.toolStripStatusLabel2.Text = StringsDictionary.KeyValue("ServiceNamesPortNumbers_toolStripStatusLabel2.Text", this.culture)
                + this.fileName;

            this.ChangeUILanguage.Invoke(this.culture);
        }

        private void ServiceNamesPortNumbers_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Escape))
            {
                IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
                this.Close();
            }
        }

        private void ServiceNamesPortNumbers_FormClosing(object sender, FormClosingEventArgs e)
        {
            IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
            this.Close();
        }
    }
}
