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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.Odbc;
using System.Globalization;

namespace IPv6SubnettingTool
{
    public partial class DatabaseUI : Form
    {

        #region special initials/constants -yucel
        string prefix = null;
        short pflen = 0;
        short parentpflen = 0;
        string parentNet = "";
        string selectedparentpflen = "";
        int RecordDisplayLimit = 100;

        NetInfo netinfo = new NetInfo();
        OdbcConnection MySQLconnection;
        DBServerInfo ServerInfo = new DBServerInfo();
        OdbcDataReader MyDataReader;
        List<string> liste = new List<string>();
        string currentMode = "";
        //
        public CultureInfo culture;
        public delegate void ChangeWinFormStringsDelegate(CultureInfo culture);
        public event ChangeWinFormStringsDelegate ChangeUILanguage = delegate { };
        //
        public delegate void ChangeDatabaseDelegate(string dbname);
        public event ChangeDatabaseDelegate ChangeDB = delegate { };
        #endregion

        public DatabaseUI(string prefix, short pflen, short parentpflen, OdbcConnection sqlcon,
            DBServerInfo servinfo, CultureInfo culture, string mode, Font font)
        {
            InitializeComponent();

            this.culture = culture;
            this.SwitchLanguage(this.culture);

            this.textBox1.Font = font;
            this.textBox2.Font = font;
            this.textBox3.Font = font;
            this.textBox4.Font = font;
            this.textBox5.Font = font;
            this.textBox6.Font = font;
            this.textBox7.Font = font;
            this.textBox8.Font = font;
            this.textBox10.Font = font;

            this.currentMode = mode;
            this.ServerInfo = servinfo.ShallowCopy();

            this.pflen = pflen;
            this.parentpflen = parentpflen;
            this.MySQLconnection = sqlcon;

            this.toolStripStatusLabel1.ForeColor = Color.FromKnownColor(KnownColor.RoyalBlue);
            this.textBox9.ForeColor = Color.FromKnownColor(KnownColor.RoyalBlue);
            this.textBox9.BackColor = Color.FromKnownColor(KnownColor.Control);
            this.textBox9.ReadOnly = true;

            this.comboBox1.SelectedIndex = 0;

            if (prefix != null && pflen != 0)
            {
                if (this.currentMode == "v6")
                {
                    this.prefix = v6ST.CompressAddress(prefix);

                    this.textBox10.Text = this.ServerInfo.DBname;
                }
                else // v4
                {
                    this.prefix = prefix;

                    this.textBox10.Text = this.ServerInfo.DBname_v4;
                }
                this.textBox7.Text = this.prefix + "/" + this.pflen;
                this.textBox8.Text = "/" + this.parentpflen.ToString();
            }
            else
            {
                if (this.currentMode == "v6")
                {
                    this.textBox10.Text = this.ServerInfo.DBname;
                }
                else
                {
                    this.textBox10.Text = this.ServerInfo.DBname_v4;
                }
                //this.textBox8.Text = "/" + this.parentpflen.ToString();
            }

            if (this.MySQLconnection == null)
                toolStripStatusLabel2.Text = "db=Down";
            else
            {
                if (this.MySQLconnection.State == ConnectionState.Open)
                    toolStripStatusLabel2.Text = "db=Up";
                else if (this.MySQLconnection.State == ConnectionState.Closed)
                    toolStripStatusLabel2.Text = "db=Down";
            }

            this.button2_Click(null, EventArgs.Empty);
        }

        public Boolean isParentNetinDB(String prefix, short len, Boolean withParentLength)
        {
            OdbcDataReader MyDataRdr;
            OdbcCommand MyCmd;
            string MySQLcmd;
            Boolean Found = false;
            int r;

            try
            {
                if (withParentLength)
                {
                    string[] sa;
                    string[] Qsa;
                    string sDBname, sTableName;

                    if (this.currentMode == "v6")
                    {
                        this.parentNet = v6ST.FindParentNet(prefix, len, CheckState.Checked);
                        Qsa = new string[2] { "inet6_ntoa(prefix)", "inet6_aton" };
                        sDBname = this.ServerInfo.DBname;
                        sTableName = this.ServerInfo.Tablename;
                    }
                    else
                    {
                        this.parentNet = v6ST.FindParentNet_v4(prefix, len);
                        Qsa = new string[2] { "inet_ntoa(prefix)", "inet_aton" };
                        sDBname = this.ServerInfo.DBname_v4;
                        sTableName = this.ServerInfo.Tablename_v4;
                    }

                    sa = this.parentNet.Split('/');

                    // is ParentPrefix in Database?:
                    MySQLcmd = "SELECT " + Qsa[0] + ", pflen, parentpflen FROM "
                        + "`" + sDBname + "`." + "`" + sTableName + "`"
                        + " WHERE ( prefix=" + Qsa[1] + "('" + sa[0] + "')"
                        + " AND pflen=" + sa[1] + " AND parentpflen=" + len.ToString() 
                        + " )";

                    MyCmd = new OdbcCommand(MySQLcmd, this.MySQLconnection);
                    MyDataRdr = MyCmd.ExecuteReader();
                    r = MyDataRdr.RecordsAffected;

                    string s = "";

                    if (MyDataRdr.Read())
                    {
                        s = MyDataRdr.GetByte(2).ToString();
                        //Console.WriteLine(s);
                        Found = true;
                    }
                    else
                    {
                        return false;
                    }
                }
                /*
                else  // Try not to use, i.e. don't input 'false'. (it works but gives same results)
                {     
                    // We don't have parentpflength. First get parentpflen of the input prefix from DB:
                    MySQLcmd = "SELECT parentpflen FROM "
                            + "`" + this.ServerInfo.DBname + "`." + "`" + this.ServerInfo.Tablename + "`"
                            + " WHERE prefix = inet6_aton('" + prefix + "')"
                            + " AND pflen = " + pflen + " ORDER BY parentpflen ASC";

                    MyCmd = new OdbcCommand(MySQLcmd, this.MySQLconnection);
                    MyDataRdr = MyCmd.ExecuteReader();
                    r = MyDataRdr.RecordsAffected;

                    short ppflen = -1;

                    if (MyDataRdr.Read())
                    {
                        ppflen = Convert.ToInt16(MyDataRdr.GetString(0));
                        //Console.WriteLine("FOUND> ", ppflen);
                    }
                    else
                    {
                        return false;
                    }

                    // with ppflen, find ParentNet using v6ST:
                    String[] sa;
                    this.parentNet = v6ST.FindParentNet(prefix, ppflen, CheckState.Checked);
                    sa = this.parentNet.Split('/');

                    // is ParentPrefix in Database?:
                    MySQLcmd = "SELECT inet6_ntoa(prefix), pflen, parentpflen FROM "
                            + "`" + this.ServerInfo.DBname + "`." + "`" + this.ServerInfo.Tablename + "`"
                            + " WHERE prefix=inet6_aton('" + sa[0] + "')"
                            + " AND pflen=" + sa[1] + " AND parentpflen=" + sa[1];

                    MyCmd = new OdbcCommand(MySQLcmd, this.MySQLconnection);
                    MyDataRdr = MyCmd.ExecuteReader();
                    r = MyDataRdr.RecordsAffected;

                    string s = "";

                    if (MyDataRdr.Read())
                    {
                        s = MyDataRdr.GetString(2);
                        //Console.WriteLine("FOUND> ", s);
                        Found = true;
                    }
                    else
                    {
                        return false;
                    }
                }*/

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message +
                    StringsDictionary.KeyValue("FormDB_MySQLquery_exception", this.culture),
                    StringsDictionary.KeyValue("FormDB_MySQLquery_exception_header", this.culture),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return Found;
        }

        public int MySQLquery(string[] sa, string MySQLcmd, int btn)
        {
            int r = 0;
            OdbcCommand MyCommand = new OdbcCommand(MySQLcmd, this.MySQLconnection);

            try
            {
                if (this.MySQLconnection.State == ConnectionState.Closed)
                {
                    MessageBox.Show(StringsDictionary.KeyValue("FormDB_MySQLquery_closed", this.culture),
                        StringsDictionary.KeyValue("FormDB_MySQLquery_closed_header", this.culture),
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return -1;
                }

                if (this.MySQLconnection.State != ConnectionState.Open)
                    this.MySQLconnection.Open();

                if (this.currentMode == "v6")
                    this.MySQLconnection.ChangeDatabase(this.ServerInfo.DBname);
                else // v4
                    this.MySQLconnection.ChangeDatabase(this.ServerInfo.DBname_v4);

                #region if btn=1=Insert/Update
                if (btn == 1)
                {
                    try
                    {
                        r = MyCommand.ExecuteNonQuery();
                        this.toolStripStatusLabel1.Text =
                            StringsDictionary.KeyValue("FormDB_insertrecord", this.culture);
                        return r;
                    }
                    catch (System.InvalidOperationException ex)
                    {
                        MessageBox.Show(ex.Message +
                            StringsDictionary.KeyValue("FormDB_MySQLquery_exception", this.culture),
                            StringsDictionary.KeyValue("FormDB_MySQLquery_exception_header", this.culture),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return -1;
                    }
                    catch (OdbcException ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return -1;
                    }
                }
                #endregion if btn=1
                
                #region if btn=2=Query
                else if (btn == 2)
                {
                    try
                    {
                        MyDataReader = MyCommand.ExecuteReader();

                        r = MyDataReader.RecordsAffected;
                        liste.Clear();

                        Boolean isFirstEntryDisplayed = false;

                        while (MyDataReader.Read())
                        {
                            liste.Add("prefix:\t\t " + MyDataReader.GetString(0) + "/" + MyDataReader.GetByte(1).ToString());

                            Boolean b = isParentNetinDB(MyDataReader.GetString(0), Convert.ToInt16(MyDataReader.GetByte(2)), true);
                            if (b)
                            {
                                liste.Add("parent:\t\t " + this.parentNet);
                            } else
                            {
                                string sp = this.parentNet.Split('/')[1];
                                liste.Add("parent:\t\t " + this.parentNet + " (/" + sp + "-" + sp + " *Not_in_DB*)");
                            }

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

                            if (!isFirstEntryDisplayed && sa[0] != "")
                            {
                                this.textBox7.Text = MyDataReader.GetString(0) + "/" + MyDataReader.GetByte(1).ToString();

                                if (this.selectedparentpflen == "")
                                {
                                    this.selectedparentpflen = MyDataReader.GetByte(2).ToString();
                                    this.textBox8.Text = "/" + this.selectedparentpflen;
                                }
                                this.textBox1.Text = MyDataReader.GetString(3);
                                this.textBox2.Text = MyDataReader.GetString(4);
                                this.textBox3.Text = MyDataReader.GetString(5);
                                this.textBox6.Text = MyDataReader.GetString(6);
                                this.textBox4.Text = MyDataReader.GetString(7);
                                this.textBox5.Text = MyDataReader.GetString(8);
                                this.comboBox1.SelectedItem = MyDataReader.GetString(9);

                                isFirstEntryDisplayed = true;

                            }
                        }
                        this.listBox1.Items.AddRange(liste.ToArray());

                        MyDataReader.Close();
                        if (MyDataReader is IDisposable)
                            MyDataReader.Dispose();
                        return r;
                    }
                    catch (System.InvalidOperationException ex)
                    {
                        MessageBox.Show(ex.Message +
                            StringsDictionary.KeyValue("FormDB_MySQLquery_exception", this.culture),
                            StringsDictionary.KeyValue("FormDB_MySQLquery_exception_header", this.culture),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return -1;
                    }
                    catch (OdbcException ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return -1;
                    }
                }
                #endregion btn2

                #region if btn=3=Delete
                else if (btn == 3)
                {
                    try
                    {
                        r = MyCommand.ExecuteNonQuery();
                    }
                    catch (InvalidOperationException ex)
                    {
                        MessageBox.Show(ex.Message +
                            StringsDictionary.KeyValue("FormDB_MySQLquery_exception", this.culture),
                            StringsDictionary.KeyValue("FormDB_MySQLquery_exception_header", this.culture),
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return -1;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return -1;
                    }

                }
                #endregion btn=3
            
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return r;
        }

        private void button1_Click(object sender, EventArgs e) //(1) Insert/Update 
        {
            string[] sa = this.CheckAll(1);
            int r = 0;

            string spfx = "", sDBName = "", sTableName = "", MySQLcmd = "";

            //if (sa != null)
            if (sa != null && sa[0] != null && sa[1] != null)
            {
                // First, we automatically insert Symbolic-ParentPrefix.
                // It's just a symbolic record showing initial boundary of the selected range.
                // ParentPrefix is defined by value of 'SliderPrefix'. -yucel

                Boolean b;
                if (this.selectedparentpflen == "")
                {
                    b = isParentNetinDB(sa[0], this.parentpflen, true); //parentpflen is == SliderPrefix value !
                }
                else
                {
                    b = isParentNetinDB(sa[0], Convert.ToInt16(this.selectedparentpflen), true);
                }

                string parent;
                if (this.selectedparentpflen == "")
                {
                    if (this.currentMode == "v6")
                        parent = v6ST.FindParentNet(sa[0], parentpflen, CheckState.Checked);
                    else
                        parent = v6ST.FindParentNet_v4(sa[0], parentpflen);
                }
                else
                {
                    if (this.currentMode == "v6")
                        parent = v6ST.FindParentNet(sa[0], Convert.ToInt16(this.selectedparentpflen), CheckState.Checked);
                    else
                        parent = v6ST.FindParentNet_v4(sa[0], parentpflen);
                }

                if (!b)
                {   // Insert PARENT as /xx-xx samevalues
                    string[] saparent = parent.Split('/');

                    string qs = "";
                    if (this.currentMode == "v6")
                    {
                        qs = "inet6_aton";
                        sDBName = this.ServerInfo.DBname;
                        sTableName = this.ServerInfo.Tablename;

                        if (sDBName == "" || sTableName == "")
                            return;
                    }
                    else
                    {
                        qs = "inet_aton";
                        sDBName = this.ServerInfo.DBname_v4;
                        sTableName = this.ServerInfo.Tablename_v4;

                        if (sDBName == "" || sTableName == "")
                            return;
                    }

                    MySQLcmd = "INSERT INTO "
                            + "`" + sDBName + "`.`" + sTableName + "` "
                            + "(prefix, pflen, parentpflen, netname, person, organization, `as-num`, phone, email, status) "
                            + "VALUES( " + qs + "('" + saparent[0] + "'), " + saparent[1] + ", " + saparent[1] + ", "
                            + "'*SymbolicParent for the Range: /" + saparent[1] + "-" + sa[1] + " *', "
                            + "'*AUTO-GENERATED (can be updated)*', "
                            + "'" + netinfo.organization + "', "
                            + "'" + netinfo.asnum + "', "
                            + "'" + netinfo.phone + "', "
                            + "'" + netinfo.email + "', "
                            + "'" + netinfo.status + "')";

                    if (MySQLconnection != null)
                    {
                        int re = 0;
                        OdbcCommand MyCommand = new OdbcCommand(MySQLcmd, this.MySQLconnection);

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

                            re = MyCommand.ExecuteNonQuery();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                // Then, insert the input prefix:
                String ss = "";
                if (this.selectedparentpflen == "")
                    ss = this.parentpflen.ToString();
                else
                    ss = this.selectedparentpflen;

                if (this.currentMode == "v6")
                {
                    spfx = " inet6_aton";

                    sDBName = this.ServerInfo.DBname;
                    sTableName = this.ServerInfo.Tablename;

                    if (sDBName == "" || sTableName == "")
                        return;
                }
                else // v4
                {
                    spfx = " inet_aton";

                    sDBName = this.ServerInfo.DBname_v4;
                    sTableName = this.ServerInfo.Tablename_v4;

                    if (sDBName == "" || sTableName == "")
                        return;
                }

                MySQLcmd = "INSERT INTO "
                    + "`" + sDBName + "`.`" + sTableName + "` "
                    + "(prefix, pflen, parentpflen, netname, person, organization, `as-num`, phone, email, status) "
                    + " VALUES( "
                    + spfx + "('" + sa[0] + "'), " + sa[1].ToString() + ", "
                    + ss + ", "
                    + "'" + this.netinfo.netname + "', "
                    + "'" + this.netinfo.person + "', "
                    + "'" + this.netinfo.organization + "', "
                    + "'" + this.netinfo.asnum + "', "
                    + "'" + this.netinfo.phone + "', "
                    + "'" + this.netinfo.email + "', "
                    + "'" + this.netinfo.status + "') "
                    + " ON DUPLICATE KEY UPDATE prefix="
                    + spfx + "('" + sa[0] + "'), "
                    + " pflen=" + sa[1].ToString() + ", "
                    + " parentpflen=" + ss + ", "
                    + " netname='" + this.netinfo.netname + "', "
                    + " person='" + this.netinfo.person + "', "
                    + " organization='" + this.netinfo.organization + "', "
                    + " `as-num`='" + this.netinfo.asnum + "', "
                    + " phone='" + this.netinfo.phone + "', "
                    + " email='" + this.netinfo.email + "', "
                    + " status='" + this.netinfo.status + "';"
                    ;
                r = this.MySQLquery(sa, MySQLcmd, 1);

                if (r == -1)
                {
                    this.toolStripStatusLabel1.Text = "[ Error ]";
                    this.textBox9.Text = "[ ]";
                    return;
                }
                else
                {
                    MessageBox.Show("Record inserted/updated", "Record inserted/updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (this.currentMode == "v6")
                {
                    spfx = " INET6_NTOA(prefix)";
                }
                else // v4
                {
                    spfx = " INET_NTOA(prefix)";
                }

                MySQLcmd = "SELECT "
                    + spfx
                    + ", pflen, parentpflen, netname, person, "
                    + "organization, `as-num`, phone, email, status, created, `last-updated` FROM "
                    + "`" + sDBName + "`.`" + sTableName + "` "
                    + " WHERE ("
                    + spfx + "='" + sa[0] + "' AND pflen=" + sa[1]
                    + "  AND parentpflen=" + ss
                    + " ) LIMIT " + this.RecordDisplayLimit;

                r = this.MySQLquery(sa, MySQLcmd, 2);

                if (r == -1)
                {
                    this.toolStripStatusLabel1.Text = "[ Error ]";
                    this.textBox9.Text = "[ ]";
                    return;
                }
                else if (r > 0)
                    this.textBox9.Text = "[ " + r + StringsDictionary.KeyValue("FormDB_records", this.culture);
            }
            if (this.listBox1.SelectedItem == null)
            {
                this.button3.Enabled = false;
            }
        }

        private void button2_Click(object sender, EventArgs e) //(2) Query 
        {
            string[] sa = this.CheckAll(2);
            
            string MySQLcmd = "";
            string scmd = "";
            int r = 0;

            string spfx = "", sDBName = "", sTableName = "";

            if (this.currentMode == "v6")
            {
                this.textBox10.Text = this.ServerInfo.DBname;
                sDBName = this.ServerInfo.DBname;
                sTableName = this.ServerInfo.Tablename;

                if (sDBName == "" || sTableName == "")
                    return;
            }
            else // v4
            {
                this.textBox10.Text = this.ServerInfo.DBname_v4;
                sDBName = this.ServerInfo.DBname_v4;
                sTableName = this.ServerInfo.Tablename_v4;

                if (sDBName == "" || sTableName == "")
                    return;
            }

            if (sa != null)
            {
                if (sa[0] != "")
                {
                    if (this.currentMode == "v6")
                    {
                        spfx = " INET6_NTOA(prefix)";
                    }
                    else // v4
                    {
                        spfx = " INET_NTOA(prefix)";
                    }

                    String mycmd = "";

                    if (this.selectedparentpflen == "")
                    {
                        mycmd = " AND pflen=" + sa[1] + " AND parentpflen=" + this.parentpflen.ToString();
                    }
                    else
                    {
                        mycmd = " AND pflen=" + sa[1] + " AND parentpflen=" + this.selectedparentpflen;
                    }

                    MySQLcmd = "SELECT "
                        + spfx
                        + ", pflen, parentpflen, netname, person, organization, "
                        + "`as-num`, phone, email, status, created, `last-updated` FROM "
                        + "`" + sDBName + "`.`" + sTableName + "` "
                        + " WHERE ("
                        + spfx + "='" + sa[0] + "' "
                        + mycmd
                        +" ) LIMIT " + this.RecordDisplayLimit;
                }
                else
                {
                    if (netinfo.netname == "" && netinfo.person == "")
                    {
                        scmd = "";
                        return;
                    }

                    else if (netinfo.netname != "" && netinfo.person == "")
                        scmd = " netname LIKE '%" + netinfo.netname + "%'";

                    else if (netinfo.netname == "" && netinfo.person != "")
                        scmd = " person LIKE '%" + netinfo.person + "%'";

                    else if (netinfo.netname != "" && netinfo.person != "")
                        scmd = " netname LIKE '%" + netinfo.netname + "%'"
                            + " AND person LIKE '%" + netinfo.person + "%'";

                    if (this.currentMode == "v6")
                    {
                        spfx = " INET6_NTOA(prefix)";
                    }
                    else // v4
                    {
                        spfx = " INET_NTOA(prefix)";
                    }

                    MySQLcmd = "SELECT "
                        + spfx
                        + ", pflen, parentpflen, netname, person, organization, "
                        + "`as-num`, phone, email, status, created, `last-updated` FROM "
                        + "`" + sDBName + "`.`" + sTableName + "` "
                        + " WHERE (" 
                        + scmd
                        + ") LIMIT " + this.RecordDisplayLimit;
                }

                r = this.MySQLquery(sa, MySQLcmd, 2);

                if (r == -1)
                {
                    this.toolStripStatusLabel1.Text = "[ Error ]";
                    this.textBox9.Text = "[ ]";
                    return;
                }
                else if (r > 0)
                {
                    this.textBox9.Text = "[ " + r
                        + StringsDictionary.KeyValue("FormDB_records", this.culture);
                    this.toolStripStatusLabel1.Text = 
                        StringsDictionary.KeyValue("FormDB_recordlimit", this.culture);
                }
                else if (r == 0)
                {
                    this.textBox9.Text = "[ " + r
                        + StringsDictionary.KeyValue("FormDB_records", this.culture);
                    this.toolStripStatusLabel1.Text = 
                        StringsDictionary.KeyValue("FormDB_norecord", this.culture);
                }
            }
            if (this.listBox1.SelectedItem == null)
            {
                this.button3.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e) //(3) Delete 
        {
            //string[] sa = this.CheckAll(3);

            if (this.listBox1.SelectedItem == null)
                return;

            string[] sa;

            try
            {
                string pfx = this.listBox1.SelectedItem.ToString().Split(' ')[1];
                sa = pfx.Split('/');

                string MySQLcmd = "";
                this.toolStripStatusLabel1.Text = "";

                string spfx = "", sDBName = "", sTableName = "";

                if (sa != null && sa[0] != null && sa[1] != null)
                {
                    if (this.currentMode == "v6")
                    {
                        spfx = "inet6_aton";
                        sDBName = this.ServerInfo.DBname;
                        sTableName = this.ServerInfo.Tablename;

                        if (sDBName == "" || sTableName == "")
                            return;
                    }
                    else // v4
                    {
                        spfx = "inet_aton";
                        sDBName = this.ServerInfo.DBname_v4;
                        sTableName = this.ServerInfo.Tablename_v4;

                        if (sDBName == "" || sTableName == "")
                            return;
                    }

                    int re = 0;
                    short ppflen = 0;
                    OdbcDataReader MyDataRdr;
                    OdbcCommand MyCmd;

                    MySQLcmd = "SELECT parentpflen FROM "
                        + "`" + sDBName + "`." + "`" + sTableName + "`"
                        + " WHERE prefix =" + spfx + "('" + sa[0] + "')"
                        + " AND pflen = " + sa[1]
                        + " ORDER BY parentpflen ASC";

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

                            MyCmd = new OdbcCommand(MySQLcmd, this.MySQLconnection);

                            MyDataRdr = MyCmd.ExecuteReader();
                            re = MyDataRdr.RecordsAffected;

                            if (MyDataRdr.Read())
                            {
                                ppflen = Convert.ToInt16(MyDataRdr.GetString(0));
                            }
                            else
                            {
                                MessageBox.Show(this, "parentpflen(parent prefix length):\r\nNot found in database", "Error:");
                                return;
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    MySQLcmd = "DELETE FROM "
                        + "`" + sDBName + "`.`" + sTableName + "` "
                        + " WHERE ( prefix="
                        + spfx + "('" + sa[0] + "') "
                        + " AND pflen=" + sa[1] + " AND parentpflen=" + ppflen
                        + " );";

                    if (MessageBox.Show(StringsDictionary.KeyValue("FormDB_DeleteConfirm", this.culture) + Environment.NewLine 
                        + sa[0] + "/" + sa[1] + Environment.NewLine,
                        StringsDictionary.KeyValue("FormDB_DeleteConfirm", this.culture),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        != DialogResult.Yes)
                    {
                        if (this.listBox1.SelectedItem == null)
                        {
                            this.button3.Enabled = false;
                        }
                        
                        return;
                    }

                    int r = this.MySQLquery(sa, MySQLcmd, 3);

                    if (r == 0)
                    {
                        this.toolStripStatusLabel1.Text =
                            StringsDictionary.KeyValue("FormDB_norecord", this.culture);
                    }
                    else if (r > 0)
                    {
                        this.toolStripStatusLabel1.Text =
                            StringsDictionary.KeyValue("FormDB_delrecord", this.culture);

                        this.textBox1.Text = this.textBox2.Text = this.textBox3.Text = this.textBox4.Text
                            = this.textBox5.Text = this.textBox6.Text = this.textBox7.Text = "";

                        this.listBox1.Items.Clear();
                        
                        MessageBox.Show("Record deleted", "Record deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (r == -1)
                    {
                        this.toolStripStatusLabel1.Text = "[ Error ]";
                        this.textBox9.Text = "[ ]";
                        return;
                    }
                    this.button2_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception");
            }
        }

        private void textBox7_Enter(object sender, EventArgs e)
        {
            this.textBox7.BackColor = Color.White;
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            this.textBox1.BackColor = Color.White;
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            this.textBox2.BackColor = Color.White;
        }

        public string[] CheckAll(int btn)
        {
            string[] sa = new string[2];
            string qprefix = "", qpflen = "";

            if (btn != 3) // 3=delete
                this.listBox1.Items.Clear();

            this.netinfo.netname = this.textBox1.Text = this.textBox1.Text.Trim();
            this.netinfo.person = this.textBox2.Text = this.textBox2.Text.Trim();
            this.netinfo.organization = this.textBox3.Text = this.textBox3.Text.Trim();
            this.netinfo.phone = this.textBox4.Text = this.textBox4.Text.Trim();
            this.netinfo.email = this.textBox5.Text = this.textBox5.Text.Trim();
            this.textBox7.Text = this.textBox7.Text.Trim();
            this.textBox7.BackColor = Color.White;
            this.textBox1.BackColor = Color.White;
            this.textBox2.BackColor = Color.White;
            this.textBox9.Text = "[ ]";
            this.netinfo.status = this.comboBox1.SelectedItem.ToString();

            this.textBox6.Text = this.textBox6.Text.Trim();
            if (this.textBox6.Text != "")
            {
                if (!UInt32.TryParse(this.textBox6.Text, out this.netinfo.asnum))
                {
                    this.textBox6.BackColor = Color.Yellow;
                    return null;
                }
            }

            int k = 0;
            for (int i = 0; i < this.textBox7.Text.Length; i++)
            {
                if (this.textBox7.Text[i] == '/')
                    k++;
            }
            if (k != 1 && this.textBox7.Text != "")
            {
                this.textBox7.BackColor = Color.Yellow;
                return null;
            }

            if (this.textBox7.Text != "")
            {
                qprefix = this.textBox7.Text.Split('/')[0].Trim();

                if (this.currentMode == "v6")
                {
                    if (!v6ST.IsAddressCorrect(qprefix))
                    {
                        this.textBox7.BackColor = Color.Yellow;
                        return null;
                    }

                    qprefix = v6ST.CompressAddress(qprefix);
                    if (qprefix == "::")
                    {
                        this.textBox7.Text = qprefix + "/" + qpflen;
                        this.textBox7.BackColor = Color.Yellow;
                        return null;
                    }
                }
                else // v4
                {
                    if (!v6ST.IsAddressCorrect_v4(qprefix))
                    {
                        this.textBox7.BackColor = Color.Yellow;
                        return null;
                    }
                }

                qpflen = this.textBox7.Text.Split('/')[1].Trim();

                UInt16 ui;
                bool b = UInt16.TryParse(qpflen, out ui);
                if (!b)
                {
                    this.textBox7.BackColor = Color.Yellow;
                    return null;
                }

                if (this.currentMode == "v6")
                {
                    if (b && ui > 128)
                    {
                        this.textBox7.BackColor = Color.Yellow;
                        return null;
                    }
                }
                else // v4
                {
                    if (b && ui > 32)
                    {
                        this.textBox7.BackColor = Color.Yellow;
                        return null;
                    }
                }
            }

            //btn3: Delete  -- NOT USED
            //if (btn == 3)
            //{
            //    if (qprefix == "")
            //    {
            //        this.textBox7.BackColor = Color.Yellow;
            //        return null;
            //    }
            //}

            //btn1 (update/insert)
            if (btn == 1)
            {
                if (qprefix == "" || netinfo.netname == "" || netinfo.person == "")
                {
                    if (qprefix == "")
                        this.textBox7.BackColor = Color.Yellow;
                    if (netinfo.netname == "")
                        this.textBox1.BackColor = Color.Yellow;
                    if (netinfo.person == "")
                        this.textBox2.BackColor = Color.Yellow;
                    
                    this.toolStripStatusLabel1.Text = "";
                    return null;
                }
            }
            //
            if (qprefix != "")
            {
                this.textBox7.Text = qprefix + "/" + qpflen;
            }

            sa[0] = qprefix;
            sa[1] = qpflen;
            return sa;
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

        private void button4_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";
            this.textBox2.Text = "";
            this.textBox3.Text = "";
            this.textBox4.Text = "";
            this.textBox5.Text = "";
            this.textBox6.Text = "";
            this.textBox7.Text = "";
            this.textBox9.Text = "[ ]";
            this.textBox1.BackColor = Color.White;
            this.textBox2.BackColor = Color.White;
            this.textBox7.BackColor = Color.White;
            this.listBox1.Items.Clear();
            this.toolStripStatusLabel1.Text = "";
            this.comboBox1.SelectedIndex = 0;
            this.button3.Enabled = false;
        }

        private void selectAlltoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.listBox1.Visible = false;
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                listBox1.SetSelected(i, true);
            }
            listBox1.Visible = true;
        }

        private void copytoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string s = "";
            foreach (object o in listBox1.SelectedItems)
            {
                s += o.ToString() + Environment.NewLine;
            }
            if (s != "")
                Clipboard.SetText(s);
        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;
            ListBox lb = (ListBox)sender;
            Graphics g = Graphics.FromHwnd(lb.Handle);

            Point mpos = ListBox.MousePosition;
            Point listBoxClientAreaPosition = lb.PointToClient(mpos);
            int itemIndex = lb.IndexFromPoint(listBoxClientAreaPosition);
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
            {
                listBox1.Visible = false;
                for (int i = 0; i < listBox1.Items.Count; i++)
                {
                    listBox1.SetSelected(i, true);
                }
                listBox1.Visible = true;
                listBox1.Focus();
            }
            if (e.Control && e.KeyCode == Keys.C)
            {
                string s = "";
                foreach (object o in listBox1.SelectedItems)
                {
                    s += o.ToString() + Environment.NewLine;
                }
                Clipboard.SetText(s);
            }
            if (e.KeyCode == Keys.Enter)
            {
                this.listBox1_MouseDoubleClick(null, null);
            }
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if ((this.listBox1.SelectedIndex % 12 == 0) || (this.listBox1.SelectedIndex % 12 == 1))
            {
                string s = this.listBox1.SelectedItem.ToString().Split(' ')[1];
                string[] sa = s.Split('/');

                String scmd = "";
                if (this.listBox1.SelectedIndex % 12 == 0)   // clicked on prefix:
                {
                    this.selectedparentpflen = this.listBox1.Items[this.listBox1.SelectedIndex + 1].ToString();
                    this.selectedparentpflen = this.selectedparentpflen.Split(' ')[1].Split('/')[1];

                    this.textBox8.Text = "/" + this.selectedparentpflen;
                    scmd = " AND pflen=" + sa[1] + " AND parentpflen=" + this.selectedparentpflen;
                }
                if (this.listBox1.SelectedIndex % 12 == 1)   // clicked on parentNet:
                {
                    String pfx = this.listBox1.Items[this.listBox1.SelectedIndex - 1].ToString();
                    pfx = pfx.Split(' ')[1].Split('/')[1];

                    if (pfx == sa[1])
                    {
                        this.selectedparentpflen = sa[1];
                        this.textBox8.Text = "/" + this.selectedparentpflen;
                        scmd = " AND pflen=" + sa[1] + " AND parentpflen=" + sa[1];
                    }
                    else
                    {
                        this.selectedparentpflen = "";
                        this.textBox8.Text = "No parent - using initial /" + this.parentpflen;
                        scmd = " AND pflen=" + sa[1]; // Normal Query. Yeni prefix gibi bakmaliyiz.Cunku onun altindakileri henuz bilmiyoruz.
                    }
                }

                this.listBox1.Items.Clear();
                string spfx = "", sDBName = "", sTableName = "";

                if (this.currentMode == "v6")
                {
                    spfx = " INET6_NTOA(prefix)";

                    sDBName = this.ServerInfo.DBname;
                    sTableName = this.ServerInfo.Tablename;

                    if (sDBName == "" || sTableName == "")
                        return;
                }
                else // v4
                {
                    spfx = " INET_NTOA(prefix)";

                    sDBName = this.ServerInfo.DBname_v4;
                    sTableName = this.ServerInfo.Tablename_v4;

                    if (sDBName == "" || sTableName == "")
                        return;
                }

                string MySQLcmd = "SELECT "
                    + spfx
                    + ", pflen, parentpflen, netname, person, organization, `as-num`, phone, email, status, created, `last-updated` FROM "
                    + "`" + sDBName + "`.`" + sTableName + "` "
                    + " WHERE ("
                    + spfx + "='" + sa[0] + "'"
                    + scmd
                    + " ) LIMIT " + this.RecordDisplayLimit + ";";

                int r = this.MySQLquery(sa, MySQLcmd, 2);

                if (r == -1)
                {
                    this.toolStripStatusLabel1.Text = "[ Error ]";
                    this.textBox9.Text = "[ ]";
                    return;
                }
                else if (r > 0)
                {
                    this.textBox9.Text = "[ " + r
                        + StringsDictionary.KeyValue("FormDB_records", this.culture);
                    this.toolStripStatusLabel1.Text =
                        StringsDictionary.KeyValue("FormDB_modifyrecord", this.culture);
                }
                else if (r == 0)
                {
                    this.textBox9.Text = "[ " + r
                        + StringsDictionary.KeyValue("FormDB_records", this.culture);
                    this.toolStripStatusLabel1.Text =
                        StringsDictionary.KeyValue("FormDB_norecord", this.culture);
                }
                if (this.listBox1.SelectedItem == null)
                {
                    this.button3.Enabled = false;
                }
            }
        }

        private void deletetoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            button3_Click(null, null);

            /*
            if (this.listBox1.SelectedIndex % 12 == 0)
            {
                string s = this.listBox1.SelectedItem.ToString().Split(' ')[1];
                string[] sa = s.Split('/');
                string MySQLcmd = "";
                int r = 0;

                if (sa[0] != null && sa[1] != null)
                {
                    if (MessageBox.Show("Deleting prefix: " + Environment.NewLine + sa[0] + "/" + sa[1]
                        + Environment.NewLine + Environment.NewLine + "Are you sure?", "Delete prefix",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                        != DialogResult.Yes)
                        return;

                    this.listBox1.Items.Clear();

                    string spfx1 = "", sDBName = "", sTableName = "";

                    if (this.currentMode == "v6")
                    {
                        spfx1 = " inet6_aton('" + sa[0] + "') ";

                        sDBName = this.ServerInfo.DBname;
                        sTableName = this.ServerInfo.Tablename;

                        if (sDBName == "" || sTableName == "")
                            return;
                    }
                    else // v4
                    {
                        spfx1 = " inet_aton('" + sa[0] + "') ";

                        sDBName = this.ServerInfo.DBname_v4;
                        sTableName = this.ServerInfo.Tablename_v4;

                        if (sDBName == "" || sTableName == "")
                            return;
                    }

                    MySQLcmd = "DELETE FROM "
                        + "`" + sDBName + "`.`" + sTableName + "` "
                        + " WHERE ( prefix="
                        + spfx1
                        + " AND pflen=" + sa[1] 
                        + ");";

                    r = this.MySQLquery(sa, MySQLcmd, 3);

                    if (r == -1)
                    {
                        this.toolStripStatusLabel1.Text = "[ Error ]";
                        this.textBox9.Text = "[ ]";
                        return;
                    }

                    else if (r > 0)
                    {
                        this.toolStripStatusLabel1.Text =
                            StringsDictionary.KeyValue("FormDB_delrecord", this.culture);

                        this.textBox1.Text = this.textBox2.Text = this.textBox3.Text = this.textBox4.Text
                            = this.textBox5.Text = this.textBox6.Text = this.textBox7.Text = "";

                        this.listBox1.Items.Clear();
                    }
                    else if (r == 0)
                    {
                        this.toolStripStatusLabel1.Text =
                            StringsDictionary.KeyValue("FormDB_norecord", this.culture);
                        return;
                    }
                    
                    this.button2_Click(null, null);
                }
            }*/
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (this.listBox1.SelectedIndex % 12 == 0)
            {
                this.contextMenuStrip1.Items[2].Enabled = true;
                this.contextMenuStrip1.Items[3].Enabled = true;
            }
            else if (this.listBox1.SelectedIndex % 12 == 1)
            {
                this.contextMenuStrip1.Items[2].Enabled = false;
                this.contextMenuStrip1.Items[3].Enabled = true;
            }
            else
            {
                this.contextMenuStrip1.Items[2].Enabled = false;
                this.contextMenuStrip1.Items[3].Enabled = false;
            }
        }

        private void modifytoolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.listBox1_MouseDoubleClick(null, null);
        }

        private void FormDB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
                
                this.Close();
            }
        }

        public void DBStateChange(OdbcConnection dbconn, int info)
        {
            this.MySQLconnection = dbconn;

            if (info == -1)
            {
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

        private void button5_Click(object sender, EventArgs e)
        {
            IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());

            this.Close();
        }

        public void SwitchLanguage(CultureInfo culture)
        {
            this.culture = culture;
            this.Text = StringsDictionary.KeyValue("FormDB_Form.Text", this.culture);
            this.button1.Text = StringsDictionary.KeyValue("FormDB_button1.Text", this.culture);
            this.button2.Text = StringsDictionary.KeyValue("FormDB_button2.Text", this.culture);
            this.button3.Text = StringsDictionary.KeyValue("FormDB_button3.Text", this.culture);
            this.button4.Text = StringsDictionary.KeyValue("FormDB_button4.Text", this.culture);
            this.button5.Text = StringsDictionary.KeyValue("FormDB_button5.Text", this.culture);
            this.copytoolStripMenuItem1.Text = StringsDictionary.KeyValue("FormDB_copytoolStripMenuItem1.Text", this.culture);
            this.deletetoolStripMenuItem1.Text = StringsDictionary.KeyValue("FormDB_deletetoolStripMenuItem1.Text", this.culture);
            this.label2.Text = StringsDictionary.KeyValue("FormDB_label2.Text", this.culture);
            this.label10.Text = StringsDictionary.KeyValue("FormDB_label10.Text", this.culture);
            this.modifytoolStripMenuItem1.Text = StringsDictionary.KeyValue("FormDB_modifytoolStripMenuItem1.Text", this.culture);
            this.selectAlltoolStripMenuItem1.Text = StringsDictionary.KeyValue("FormDB_selectAlltoolStripMenuItem1.Text", this.culture);
            this.textBox9.Text = StringsDictionary.KeyValue("FormDB_textBox9.Text", this.culture);
            this.toolStripStatusLabel1.Text = StringsDictionary.KeyValue("FormDB_toolStripStatusLabel1.Text", this.culture);
            
            this.ChangeUILanguage.Invoke(this.culture);
        }

        public void ChangeDatabase(string dbname)
        {
            /*  DON'T USE:
            this.toolStripStatusLabel1.Text = "DB changed! NewDB: " + dbname;
            //StringsDictionary.KeyValue("FormDB_insertrecord", this.culture);
            this.ChangeDB.Invoke(dbname);
            this.textBox8.Text = "";
            */
        }

        private void textBox6_Enter(object sender, EventArgs e)
        {
            this.textBox6.BackColor = Color.White;
        }

        private void FormDB_FormClosing(object sender, FormClosingEventArgs e)
        {
            IPv6SubnettingTool.Form1.RemoveForm(this.GetHashCode());
        }

        private void textBox7_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                this.button2_Click(null, null);
                e.SuppressKeyPress = true;
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                this.button2_Click(null, null);
                e.SuppressKeyPress = true;
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                this.button2_Click(null, null);
                e.SuppressKeyPress = true;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listBox1.SelectedIndex % 12 == 0)
            {
                this.button3.Enabled = true;
            }
            else
            {
                this.button3.Enabled = false;
            }
        }
    }
}
