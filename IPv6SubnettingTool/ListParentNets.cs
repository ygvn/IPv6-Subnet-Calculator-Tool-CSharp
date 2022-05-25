using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Data.Odbc;
using System.Numerics;
using System.Collections.Generic;

namespace IPv6SubnettingTool
{
    public partial class ListParentNets : Form
    {

        #region special initials/constants -yucel
        string parentNet = "";
        int db_TotalRecords = 0;
        int page_records = 0;
        int records_perpage = 32;
        int currentOffset = 0;
        //Boolean chks = Boolean.FALSE;
        public List<string> liste = new List<string>();
        //
        //Clipboard clipboard = Toolkit.getDefaultToolkit().getSystemClipboard();
        //private java.awt.Point lastPos = new java.awt.Point();
        
        //Database
        public DBServerInfo dbserverInfo = new DBServerInfo();
        OdbcConnection MySQLconnection = null;
        DBServerInfo ServerInfo = new DBServerInfo();
        OdbcDataReader MyDataReader;
        string currentMode = "";
        //
        public CultureInfo culture;
        public delegate void ChangeWinFormStringsDelegate(CultureInfo culture);
        public event ChangeWinFormStringsDelegate ChangeUILanguage = delegate { };
        //
        public delegate void ChangeDBState(OdbcConnection dbconn, int info);
        public event ChangeDBState changeDBstate = delegate { };
        #endregion
        public ListParentNets(OdbcConnection sqlcon, DBServerInfo servinfo, CultureInfo culture, string mode)
        {
            InitializeComponent();
            //
            this.MySQLconnection = sqlcon;
            this.ServerInfo = servinfo.ShallowCopy();
            this.culture = culture;
            this.currentMode = mode;

            if (this.MySQLconnection == null)
                this.toolStripStatusLabel2.Text = "db=Down";
            else
            {
                if (this.MySQLconnection.State == ConnectionState.Open)
                    this.toolStripStatusLabel2.Text = "db=Up";
                else if (this.MySQLconnection.State == ConnectionState.Closed)
                    this.toolStripStatusLabel2.Text = "db=Down";
            }
            
            this.FirstPage_Click(null, null);
            this.textBox3.Text = "[" + this.page_records + "]";
        }

        public void HowManyRecordsInDB()
        {
            if (MySQLconnection == null)
            {
                return;
            }

            // How many records do we have in DB?:

            string sDBname = "", sTableName = "";

            if (this.currentMode == "v6")
            {
                sDBname = this.ServerInfo.DBname;
                sTableName = this.ServerInfo.Tablename;
            }
            else //v4
            {
                sDBname = this.ServerInfo.DBname_v4;
                sTableName = this.ServerInfo.Tablename_v4;
            }

            string MySQLcmd = "SELECT COUNT(*) FROM "
                    + " `" + sDBname + "`.`" + sTableName + "` "
                    + " WHERE pflen=parentpflen";

            if (MySQLconnection != null)
            {
                try
                {
                    if (this.MySQLconnection.State == ConnectionState.Closed)
                    {
                        MessageBox.Show(StringsDictionary.KeyValue("FormDB_MySQLquery_closed", this.culture),
                            StringsDictionary.KeyValue("FormDB_MySQLquery_closed_header", this.culture),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (this.MySQLconnection.State != ConnectionState.Open)
                        this.MySQLconnection.Open();

                    if (this.currentMode == "v6")
                        this.MySQLconnection.ChangeDatabase(this.ServerInfo.DBname);
                    else // v4
                        this.MySQLconnection.ChangeDatabase(this.ServerInfo.DBname_v4);

                    OdbcCommand MyCmd = new OdbcCommand(MySQLcmd, this.MySQLconnection);

                    MyDataReader = MyCmd.ExecuteReader();
                    int r = MyDataReader.RecordsAffected;

                    if (MyDataReader.Read())
                    {
                        this.db_TotalRecords = Convert.ToInt32(MyDataReader.GetString(0));
                        this.toolStripStatusLabel1.Text = " Total=[" + this.db_TotalRecords.ToString() + "]";
                    }
                    else
                    {
                        MessageBox.Show("Not found in database", "Error:");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Exception:");
                    this.db_TotalRecords = 0;
                    //return;
                }
            }
        }

        public void DBStateChange(OdbcConnection dbconn, int info)
        {
            this.MySQLconnection = dbconn;

            if (info == -1)
            {
                this.changeDBstate.Invoke(this.MySQLconnection, info); // child

                if (this.MySQLconnection == null)
                    toolStripStatusLabel2.Text = "db=Down";
                else
                {
                    if (this.MySQLconnection.State == ConnectionState.Open)
                        toolStripStatusLabel2.Text = "db=Up";
                    else if (this.MySQLconnection.State == ConnectionState.Closed)
                        toolStripStatusLabel2.Text = "db=Down";
                }

                IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());

                if (this is IDisposable)
                    this.Dispose();
                else
                    this.Close();

            }
            else if (info == 1)
            {
                this.changeDBstate.Invoke(this.MySQLconnection, info); // child

                if (this.MySQLconnection == null)
                    toolStripStatusLabel2.Text = "db=Down";
                else
                {
                    if (this.MySQLconnection.State == ConnectionState.Open)
                        toolStripStatusLabel2.Text = "db=Up";
                    else if (this.MySQLconnection.State == ConnectionState.Closed)
                        toolStripStatusLabel2.Text = "db=Down";
                }
            }
        }

        public void SwitchLanguage(CultureInfo culture)
        {
            this.culture = culture;

            /*
            this.Text = StringsDictionary.KeyValue("StatsUsageForm.Text", this.culture);
            this.button1.Text = StringsDictionary.KeyValue("StatsUsageForm_button1.Text", this.culture);
            this.label1.Text = StringsDictionary.KeyValue("StatsUsageForm_label1.Text", this.culture);
            this.groupBox2.Text = "/" + pflen + " "
                + StringsDictionary.KeyValue("StatsUsageForm_groupBox2.Text", this.culture);
            //
            this.UpdateTextBox();

            this.ChangeUILanguage.Invoke(this.culture);
            */
        }

        public int MySQLquery(Boolean isRemainder)
        {
            if (MySQLconnection == null)
            {
                return -1;
            }

            int r = 0;
            string MySQLcmd = "";

            //this.prefixlist.getItems().clear();
            //this.jList1.setModel(new DefaultListModel<String>());
            this.listBox1.Items.Clear();

            string spfx = "", sDBname = "", sTableName = "";

            if (this.currentMode == "v6")
            {
                spfx = "INET6_NTOA";
                sDBname = this.ServerInfo.DBname;
                sTableName = this.ServerInfo.Tablename;
            }
            else //v4
            {
                spfx = "INET_NTOA";
                sDBname = this.ServerInfo.DBname_v4;
                sTableName = this.ServerInfo.Tablename_v4;
            }


            if (!isRemainder)
            {
                MySQLcmd = "SELECT "
                        + spfx + "(prefix), pflen, parentpflen, netname, person, organization, "
                        + " `as-num`, phone, email, status, created, `last-updated` FROM "
                        + " `" + sDBname + "`.`" + sTableName + "` "
                        + " WHERE pflen=parentpflen "
                        + " ORDER BY prefix "
                        + " LIMIT " + this.records_perpage.ToString()
                        + " OFFSET " + this.currentOffset.ToString();
            }
            else
            {
                MySQLcmd = "SELECT "
                        + spfx + "(prefix), pflen, parentpflen, netname, person, organization, "
                        + "`as-num`, phone, email, status, created, `last-updated` FROM "
                        + " `" + sDBname + "`.`" + sTableName + "` "
                        + " WHERE pflen=parentpflen "
                        + " ORDER BY prefix "
                        + " LIMIT " + this.currentOffset.ToString()
                        + " OFFSET 0";
            }

            try
            {
                //IPv6SubnetCalculator.UpdateDbStatus();
                //statement = MySQLconnection.createStatement(ResultSet.TYPE_SCROLL_INSENSITIVE, ResultSet.CONCUR_READ_ONLY);
                //resultSet = statement.executeQuery(MySQLcmd);


                OdbcCommand MyCommand = new OdbcCommand(MySQLcmd, this.MySQLconnection);
                MyDataReader = MyCommand.ExecuteReader();

                this.page_records = r = MyDataReader.RecordsAffected;
                liste.Clear();

                if (r > 0)
                {
                    liste.Clear();

                    while (MyDataReader.Read())
                    {
                        liste.Add("prefix:\t\t " + MyDataReader.GetString(0) + "/" + MyDataReader.GetByte(1).ToString());

                        // v6 // v4
                        if (this.currentMode == "v6")
                        {
                            this.parentNet = v6ST.FindParentNet(MyDataReader.GetString(0), Convert.ToInt16(MyDataReader.GetByte(2)), CheckState.Checked);
                        } else
                        {
                            this.parentNet = v6ST.FindParentNet_v4(MyDataReader.GetString(0), Convert.ToInt16(MyDataReader.GetByte(2)));
                        }
                        
                        liste.Add("parent:\t\t " + this.parentNet);
                        liste.Add("netname:\t " + MyDataReader.GetString(3));
                        liste.Add("person:\t\t " + MyDataReader.GetString(4));
                        liste.Add("organization:\t " + MyDataReader.GetString(5));
                        liste.Add("as-num:\t\t " + MyDataReader.GetString(6));
                        liste.Add("phone:\t\t " + MyDataReader.GetString(7));
                        liste.Add("email:\t\t " + MyDataReader.GetString(8));
                        liste.Add("status:\t\t " + MyDataReader.GetString(9));
                        liste.Add("created:\t\t " + MyDataReader.GetString(10));
                        liste.Add("last-updated:\t " + MyDataReader.GetString(11));
                        liste.Add(" ");
                    }
                    this.listBox1.Items.AddRange(liste.ToArray());
                }
                return r;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Exception:");
                //IPv6SubnetCalculator.UpdateDbStatus();
                return -1;
            }
        }

        private void FirstPage_Click(object sender, EventArgs e)
        {
            HowManyRecordsInDB();

            this.currentOffset = 0;

            int r = MySQLquery(false);

            if (r > 0)
            {
                if (r >= records_perpage)
                {
                    this.Backwd.Enabled = false;
                    this.Forwd.Enabled = true;
                    this.LastPage.Enabled = true;
                }
                else
                {
                    this.Backwd.Enabled = false;
                    this.Forwd.Enabled = false;
                    this.LastPage.Enabled = false;
                }
            }
            else
            {
                this.Backwd.Enabled = false;
                this.Forwd.Enabled = false;
                this.LastPage.Enabled = false;
            }
            this.currentOffset += page_records; //page_records is == r
            this.textBox3.Text = "[" + page_records + "]";
            //System.out.println("AFTER click FirstPage:>> currentOffset is: " + this.currentOffset);
        }

        private void Backwd_Click(object sender, EventArgs e)
        {
            Boolean remainder = false;
            int r = 0;

            this.currentOffset = this.currentOffset - page_records - this.records_perpage;

            if (this.currentOffset < 0)
            {
                remainder = true;
                this.currentOffset += this.records_perpage;
                MySQLquery(remainder);

                this.Backwd.Enabled = false;
                this.Forwd.Enabled = true;
                this.LastPage.Enabled = true;
                //System.out.println("RETURNING since NEGATIVE:>> currentOffset is: " + this.currentOffset);
                return;
            }

            r = MySQLquery(remainder);

            if (r > 0)
            {
                this.Forwd.Enabled = true;
                this.LastPage.Enabled = true;

                if (this.currentOffset <= 0)
                {
                    this.Backwd.Enabled = false;
                    this.Forwd.Enabled = true;

                    this.textBox3.Text = "[" + page_records + "]";
                }
            }
            else
            {
                this.Backwd.Enabled = false;
                this.Forwd.Enabled = true;
                this.textBox3.Text = "[" + page_records + "]";
            }
            this.currentOffset += page_records; // VEYA +r tane DENE!!            
            this.textBox3.Text = "[" + page_records + "]";
            //System.out.println("AFTER click BACK:>> currentOffset is: " + this.currentOffset);
        }

        private void Forwd_Click(object sender, EventArgs e)
        {
            int r = MySQLquery(false);

            if (r > 0)
            {
                this.currentOffset += page_records; // page_records is r.
                this.Backwd.Enabled = true;

                if (this.currentOffset >= this.db_TotalRecords)
                {
                    this.Forwd.Enabled = false;
                    this.LastPage.Enabled = false;
                    this.textBox3.Text = "[" + page_records + "]";
                }
            }
            else
            {
                this.Forwd.Enabled = false;
                this.LastPage.Enabled = false;
                this.textBox3.Text = "[" + page_records + "]";
            }
            this.textBox3.Text = "[" + page_records + "]";
            //System.out.println("AFTER click FORWARD:>> currentOffset is: " + this.currentOffset);
        }

        private void LastPage_Click(object sender, EventArgs e)
        {
            this.currentOffset = this.db_TotalRecords - this.records_perpage;
            int r = MySQLquery(false);

            if (r > 0)
            {
                this.Backwd.Enabled = true;
                this.Forwd.Enabled = false;
                this.LastPage.Enabled = false;
            }
            this.currentOffset = this.db_TotalRecords;
            this.textBox3.Text = "[" + page_records + "]";
            //System.out.println("AFTER click LASTPage:>> currentOffset is: " + this.currentOffset);
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // GetSelected() fonksiyonu ile ayni:

            if (MySQLconnection != null)
            {
                if (this.listBox1.SelectedIndex % 12 == 0)
                {
                    string selected = this.listBox1.SelectedItem.ToString().Split(' ')[1].Trim();
                    string snet = selected.Split('/')[0].Trim();
                    short plen = Convert.ToInt16(selected.Split('/')[1]);

                    DatabaseUI db = new DatabaseUI(snet, plen, plen, this.MySQLconnection,
                        this.ServerInfo, this.culture, this.currentMode, this.listBox1.Font);

                    if (!db.IsDisposed)
                    {
                        db.Show();
                        //
                        IPv6SubnettingTool.Form1.windowsList.Add(new WindowsList(db, db.Name, db.GetHashCode(), this.currentMode));

                        this.changeDBstate += db.DBStateChange;
                        this.ChangeUILanguage += db.SwitchLanguage;
                    }
                }
            }
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1)
                return;

            ListBox lb = (ListBox)sender;
            Graphics g = e.Graphics;
            SolidBrush sback = new SolidBrush(e.BackColor);
            SolidBrush sfore = new SolidBrush(e.ForeColor);

            if (e.Index % 12 == 0)
            {
                e.DrawBackground();
                DrawItemState st = DrawItemState.Selected;

                if ((e.State & st) != st)
                {
                    Color color = Color.FromArgb(30, 64, 224, 208);
                    g.FillRectangle(new SolidBrush(color), e.Bounds);
                    g.DrawString(lb.Items[e.Index].ToString(), e.Font,
                        sfore, new PointF(e.Bounds.X, e.Bounds.Y));
                }
                else
                {
                    g.FillRectangle(sback, e.Bounds);
                    g.DrawString(lb.Items[e.Index].ToString(), e.Font,
                        sfore, new PointF(e.Bounds.X, e.Bounds.Y));
                }
                e.DrawFocusRectangle();
            }
            else
            {
                e.DrawBackground();
                g.FillRectangle(sback, e.Bounds);
                g.DrawString(lb.Items[e.Index].ToString(), e.Font,
                    sfore, new PointF(e.Bounds.X, e.Bounds.Y));
                e.DrawFocusRectangle();
            }
        }

        private void ListParentNets_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
                this.Close();
            }
        }

        private void modifyPrefixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox1_MouseDoubleClick(null, null);
        }

        private void ListParentNets_FormClosing(object sender, FormClosingEventArgs e)
        {
            IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listBox1.Visible = false;
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                listBox1.SetSelected(i, true);
            }
            listBox1.Visible = true;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = "";
            foreach (object o in listBox1.SelectedItems)
            {
                s += o.ToString() + Environment.NewLine;
            }
            if (s != "")
                Clipboard.SetText(s);
        }

        private void contextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.listBox1.SelectedIndex % 12 == 0)
            {
                this.modifyPrefixToolStripMenuItem.Enabled = true;
            } else
            {
                this.modifyPrefixToolStripMenuItem.Enabled = false;
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                listBox1_MouseDoubleClick(null, null);
            }
        }
    }
}
