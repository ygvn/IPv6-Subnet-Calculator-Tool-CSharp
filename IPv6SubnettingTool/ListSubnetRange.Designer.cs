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

namespace IPv6SubnettingTool
{
    partial class ListSubnetRange
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.FirstPage = new System.Windows.Forms.Button();
            this.Backwd = new System.Windows.Forms.Button();
            this.Forwd = new System.Windows.Forms.Button();
            this.LastPage = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SearchprefixtoolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.listAllDNSReverseZonesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sendtodatabasetoolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.getPrefixInfoFromDBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.savetoolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.label3 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonGoto = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // FirstPage
            // 
            this.FirstPage.Image = global::IPv6SubnettingTool.Properties.Resources._7019Prefixes;
            this.FirstPage.Location = new System.Drawing.Point(14, 66);
            this.FirstPage.Name = "FirstPage";
            this.FirstPage.Size = new System.Drawing.Size(78, 23);
            this.FirstPage.TabIndex = 0;
            this.FirstPage.Text = "   FirstPage";
            this.toolTip1.SetToolTip(this.FirstPage, "First Page");
            this.FirstPage.UseVisualStyleBackColor = true;
            this.FirstPage.Click += new System.EventHandler(this.FirstPage_Click);
            // 
            // Backwd
            // 
            this.Backwd.Image = global::IPv6SubnettingTool.Properties.Resources._2619Back;
            this.Backwd.Location = new System.Drawing.Point(98, 66);
            this.Backwd.Name = "Backwd";
            this.Backwd.Size = new System.Drawing.Size(30, 23);
            this.Backwd.TabIndex = 1;
            this.toolTip1.SetToolTip(this.Backwd, "-128");
            this.Backwd.UseVisualStyleBackColor = true;
            this.Backwd.Click += new System.EventHandler(this.Backwd_Click);
            // 
            // Forwd
            // 
            this.Forwd.Image = global::IPv6SubnettingTool.Properties.Resources._2619Fwd;
            this.Forwd.Location = new System.Drawing.Point(134, 66);
            this.Forwd.Name = "Forwd";
            this.Forwd.Size = new System.Drawing.Size(30, 23);
            this.Forwd.TabIndex = 2;
            this.toolTip1.SetToolTip(this.Forwd, "+128");
            this.Forwd.UseVisualStyleBackColor = true;
            this.Forwd.Click += new System.EventHandler(this.Forwd_Click);
            // 
            // LastPage
            // 
            this.LastPage.Image = global::IPv6SubnettingTool.Properties.Resources._2619Last;
            this.LastPage.Location = new System.Drawing.Point(170, 66);
            this.LastPage.Name = "LastPage";
            this.LastPage.Size = new System.Drawing.Size(32, 23);
            this.LastPage.TabIndex = 3;
            this.toolTip1.SetToolTip(this.LastPage, "Last Page");
            this.LastPage.UseVisualStyleBackColor = true;
            this.LastPage.Click += new System.EventHandler(this.Last_Click);
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.Color.White;
            this.listBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBox1.Font = new System.Drawing.Font("Lucida Sans Typewriter", 8.25F);
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.IntegralHeight = false;
            this.listBox1.Location = new System.Drawing.Point(14, 91);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox1.Size = new System.Drawing.Size(386, 280);
            this.listBox1.TabIndex = 4;
            this.listBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox1_DrawItem);
            this.listBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox1_KeyDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.SearchprefixtoolStripMenuItem1,
            this.listAllDNSReverseZonesToolStripMenuItem,
            this.sendtodatabasetoolStripMenuItem1,
            this.getPrefixInfoFromDBToolStripMenuItem,
            this.toolStripSeparator1,
            this.savetoolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(209, 164);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.selectAllToolStripMenuItem.Text = "Select &All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // SearchprefixtoolStripMenuItem1
            // 
            this.SearchprefixtoolStripMenuItem1.Enabled = false;
            this.SearchprefixtoolStripMenuItem1.Name = "SearchprefixtoolStripMenuItem1";
            this.SearchprefixtoolStripMenuItem1.Size = new System.Drawing.Size(208, 22);
            this.SearchprefixtoolStripMenuItem1.Text = "&Search prefix...           F3";
            this.SearchprefixtoolStripMenuItem1.Click += new System.EventHandler(this.findprefixtoolStripMenuItem1_Click);
            // 
            // listAllDNSReverseZonesToolStripMenuItem
            // 
            this.listAllDNSReverseZonesToolStripMenuItem.Enabled = false;
            this.listAllDNSReverseZonesToolStripMenuItem.Name = "listAllDNSReverseZonesToolStripMenuItem";
            this.listAllDNSReverseZonesToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.listAllDNSReverseZonesToolStripMenuItem.Text = "List All &DNS reverse zones";
            this.listAllDNSReverseZonesToolStripMenuItem.Click += new System.EventHandler(this.listAllDNSReverseZonesToolStripMenuItem_Click);
            // 
            // sendtodatabasetoolStripMenuItem1
            // 
            this.sendtodatabasetoolStripMenuItem1.Name = "sendtodatabasetoolStripMenuItem1";
            this.sendtodatabasetoolStripMenuItem1.Size = new System.Drawing.Size(208, 22);
            this.sendtodatabasetoolStripMenuItem1.Text = "&Send prefix to database...";
            this.sendtodatabasetoolStripMenuItem1.Click += new System.EventHandler(this.sendtodatabasetoolStripMenuItem1_Click);
            // 
            // getPrefixInfoFromDBToolStripMenuItem
            // 
            this.getPrefixInfoFromDBToolStripMenuItem.Name = "getPrefixInfoFromDBToolStripMenuItem";
            this.getPrefixInfoFromDBToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.getPrefixInfoFromDBToolStripMenuItem.Text = "&Get prefix info from DB...";
            this.getPrefixInfoFromDBToolStripMenuItem.Click += new System.EventHandler(this.getPrefixInfoFromDBToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(205, 6);
            // 
            // savetoolStripMenuItem1
            // 
            this.savetoolStripMenuItem1.Name = "savetoolStripMenuItem1";
            this.savetoolStripMenuItem1.Size = new System.Drawing.Size(208, 22);
            this.savetoolStripMenuItem1.Text = "Save As &Text...";
            this.savetoolStripMenuItem1.Click += new System.EventHandler(this.savetoolStripMenuItem1_Click);
            // 
            // label3
            // 
            this.label3.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label3.Location = new System.Drawing.Point(14, 379);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(130, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Total:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 100;
            this.toolTip1.AutoPopDelay = 1000;
            this.toolTip1.InitialDelay = 0;
            this.toolTip1.ReshowDelay = 20;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Lucida Sans Typewriter", 8.25F);
            this.label5.Location = new System.Drawing.Point(85, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(12, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "_";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Lucida Sans Typewriter", 8.25F);
            this.label6.Location = new System.Drawing.Point(85, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(12, 12);
            this.label6.TabIndex = 9;
            this.label6.Text = "_";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Control;
            this.textBox1.Location = new System.Drawing.Point(150, 376);
            this.textBox1.MaxLength = 21;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(136, 20);
            this.textBox1.TabIndex = 5;
            this.textBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label1.Location = new System.Drawing.Point(14, 401);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 16);
            this.label1.TabIndex = 10;
            this.label1.Text = "Go to Prefix Number:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonGoto
            // 
            this.buttonGoto.Image = global::IPv6SubnettingTool.Properties.Resources._7419Go;
            this.buttonGoto.Location = new System.Drawing.Point(292, 399);
            this.buttonGoto.Name = "buttonGoto";
            this.buttonGoto.Size = new System.Drawing.Size(78, 23);
            this.buttonGoto.TabIndex = 7;
            this.buttonGoto.Text = "   Go";
            this.buttonGoto.UseVisualStyleBackColor = true;
            this.buttonGoto.Click += new System.EventHandler(this.buttonGoto_Click);
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.White;
            this.textBox2.Location = new System.Drawing.Point(150, 401);
            this.textBox2.MaxLength = 20;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(136, 20);
            this.textBox2.TabIndex = 6;
            this.textBox2.Text = "0";
            this.textBox2.Enter += new System.EventHandler(this.textBox2_Enter);
            this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox2_KeyPress);
            this.textBox2.Leave += new System.EventHandler(this.textBox2_Leave);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label7.Location = new System.Drawing.Point(55, 11);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(18, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "s>";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.SystemColors.ActiveCaption;
            this.label8.Location = new System.Drawing.Point(55, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(19, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "e>";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(14, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(386, 44);
            this.groupBox1.TabIndex = 13;
            this.groupBox1.TabStop = false;
            // 
            // label4
            // 
            this.label4.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label4.Location = new System.Drawing.Point(7, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Range:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.SystemColors.Control;
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox3.ForeColor = System.Drawing.Color.RoyalBlue;
            this.textBox3.Location = new System.Drawing.Point(350, 76);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(50, 13);
            this.textBox3.TabIndex = 14;
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 439);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(412, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 20;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.RoyalBlue;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(320, 17);
            this.toolStripStatusLabel1.Text = "Database:";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.AutoSize = false;
            this.toolStripStatusLabel2.ForeColor = System.Drawing.Color.RoyalBlue;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(90, 17);
            this.toolStripStatusLabel2.Text = "db=";
            this.toolStripStatusLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ListSubnetRange
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(412, 461);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.buttonGoto);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.LastPage);
            this.Controls.Add(this.FirstPage);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Backwd);
            this.Controls.Add(this.Forwd);
            this.Controls.Add(this.groupBox1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "ListSubnetRange";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "IPv6 Subnet Calculator - List Subnet Range";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ListSubnetRange_FormClosing);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ListSubnetRange_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListSubnetRange_KeyDown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button FirstPage;
        private System.Windows.Forms.Button Backwd;
        private System.Windows.Forms.Button Forwd;
        private System.Windows.Forms.Button LastPage;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonGoto;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem listAllDNSReverseZonesToolStripMenuItem;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem savetoolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem sendtodatabasetoolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem SearchprefixtoolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem getPrefixInfoFromDBToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
    }
}