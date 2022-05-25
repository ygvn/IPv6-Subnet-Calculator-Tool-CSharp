namespace IPv6SubnettingTool
{
    partial class ListParentNets
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ListParentNets));
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.FirstPage = new System.Windows.Forms.Button();
            this.Backwd = new System.Windows.Forms.Button();
            this.Forwd = new System.Windows.Forms.Button();
            this.LastPage = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modifyPrefixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.label2 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.RoyalBlue;
            this.label1.Location = new System.Drawing.Point(62, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(332, 24);
            this.label1.TabIndex = 6;
            this.label1.Text = "List of All Symbolic Parent Prefixes";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 413);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(484, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.RoyalBlue;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(370, 17);
            this.toolStripStatusLabel1.Text = "Total=[ ]";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.ForeColor = System.Drawing.Color.RoyalBlue;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(60, 17);
            this.toolStripStatusLabel2.Text = "db=Down";
            this.toolStripStatusLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FirstPage
            // 
            this.FirstPage.Image = global::IPv6SubnettingTool.Properties.Resources._7019Prefixes;
            this.FirstPage.Location = new System.Drawing.Point(12, 105);
            this.FirstPage.Name = "FirstPage";
            this.FirstPage.Size = new System.Drawing.Size(78, 23);
            this.FirstPage.TabIndex = 0;
            this.FirstPage.Text = "   FirstPage";
            this.FirstPage.UseVisualStyleBackColor = true;
            this.FirstPage.Click += new System.EventHandler(this.FirstPage_Click);
            // 
            // Backwd
            // 
            this.Backwd.Image = global::IPv6SubnettingTool.Properties.Resources._2619Back;
            this.Backwd.Location = new System.Drawing.Point(96, 105);
            this.Backwd.Name = "Backwd";
            this.Backwd.Size = new System.Drawing.Size(30, 23);
            this.Backwd.TabIndex = 1;
            this.Backwd.UseVisualStyleBackColor = true;
            this.Backwd.Click += new System.EventHandler(this.Backwd_Click);
            // 
            // Forwd
            // 
            this.Forwd.Image = global::IPv6SubnettingTool.Properties.Resources._2619Fwd;
            this.Forwd.Location = new System.Drawing.Point(132, 105);
            this.Forwd.Name = "Forwd";
            this.Forwd.Size = new System.Drawing.Size(30, 23);
            this.Forwd.TabIndex = 2;
            this.Forwd.UseVisualStyleBackColor = true;
            this.Forwd.Click += new System.EventHandler(this.Forwd_Click);
            // 
            // LastPage
            // 
            this.LastPage.Image = global::IPv6SubnettingTool.Properties.Resources._2619Last;
            this.LastPage.Location = new System.Drawing.Point(168, 105);
            this.LastPage.Name = "LastPage";
            this.LastPage.Size = new System.Drawing.Size(32, 23);
            this.LastPage.TabIndex = 3;
            this.LastPage.UseVisualStyleBackColor = true;
            this.LastPage.Click += new System.EventHandler(this.LastPage_Click);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.BackColor = System.Drawing.Color.White;
            this.listBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.IntegralHeight = false;
            this.listBox1.Location = new System.Drawing.Point(12, 134);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox1.Size = new System.Drawing.Size(460, 276);
            this.listBox1.TabIndex = 4;
            this.listBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox1_DrawItem);
            this.listBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listBox1_KeyDown);
            this.listBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDoubleClick);
            // 
            // textBox3
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox3.BackColor = System.Drawing.SystemColors.Control;
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox3.ForeColor = System.Drawing.Color.RoyalBlue;
            this.textBox3.Location = new System.Drawing.Point(432, 115);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(40, 13);
            this.textBox3.TabIndex = 8;
            this.textBox3.TabStop = false;
            this.textBox3.Text = "[ ]";
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.toolStripSeparator1,
            this.modifyPrefixToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(146, 76);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.selectAllToolStripMenuItem.Text = "Select &All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.copyToolStripMenuItem.Text = "&Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // modifyPrefixToolStripMenuItem
            // 
            this.modifyPrefixToolStripMenuItem.Name = "modifyPrefixToolStripMenuItem";
            this.modifyPrefixToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.modifyPrefixToolStripMenuItem.Text = "&Modify prefix";
            this.modifyPrefixToolStripMenuItem.Click += new System.EventHandler(this.modifyPrefixToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(142, 6);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(25, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(447, 45);
            this.label2.TabIndex = 10;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // ListParentNets
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 435);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.LastPage);
            this.Controls.Add(this.Forwd);
            this.Controls.Add(this.Backwd);
            this.Controls.Add(this.FirstPage);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label1);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(500, 474);
            this.Name = "ListParentNets";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ListParentNets";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ListParentNets_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ListParentNets_KeyDown);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Button FirstPage;
        private System.Windows.Forms.Button Backwd;
        private System.Windows.Forms.Button Forwd;
        private System.Windows.Forms.Button LastPage;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem modifyPrefixToolStripMenuItem;
        private System.Windows.Forms.Label label2;
    }
}