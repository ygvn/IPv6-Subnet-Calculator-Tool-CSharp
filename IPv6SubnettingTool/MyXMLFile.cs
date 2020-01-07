/*
 * Copyright (c) 2010-2020 Yucel Guven
 * All rights reserved.
 * 
 * This file is part of IPv6 Subnetting Tool.
 * 
 * Version: 4.1
 * Published Date: 6 January 2020
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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Windows.Forms;
using System.Xml;

namespace IPv6SubnettingTool
{
    public class MyXMLFile
    {
        public string xmlFilename = "IPv6SubnetCalculatorInfo.xml";

        public MyXMLFile() { }

        public bool ReadValues()
        {
            if (File.Exists(this.xmlFilename))
            {
                /* Read Info from the XML file. This file is used like Registry.
                 * PS: Order is significant!
                 */

                #region XML File Read
                try
                {
                    XmlDocument xmldoc = new XmlDocument();
                    xmldoc.Load(this.xmlFilename);

                    // Load UI_info

                    XmlNode node = xmldoc.SelectSingleNode("INFO/UI_Info");

                    foreach (XmlNode n in node.ChildNodes)
                    {
                        switch (n.Name)
                        {
                            case "Culture":
                                {
                                    if (n.InnerText == "en-US")
                                    {
                                        ScreenShotValues.Cultur = new System.Globalization.CultureInfo(n.InnerText);
                                    }
                                    else if (n.InnerText == "tr-TR")
                                    {
                                        ScreenShotValues.Cultur = new System.Globalization.CultureInfo(n.InnerText);
                                    }
                                    else if (n.InnerText == "de-DE")
                                    {
                                        ScreenShotValues.Cultur = new System.Globalization.CultureInfo(n.InnerText);
                                    }
                                    else // default
                                        ScreenShotValues.Cultur = new System.Globalization.CultureInfo("en-US");
                                    break;
                                }
                            //
                            case "LocX":
                                if (n.InnerText.Trim() != "")
                                {
                                    if (!Int32.TryParse(n.InnerText.Trim(), out ScreenShotValues.LocX))
                                        ScreenShotValues.LocX = 0;
                                }
                                break;
                            case "LocY":
                                if (n.InnerText.Trim() != "")
                                {
                                    if (!Int32.TryParse(n.InnerText.Trim(), out ScreenShotValues.LocY))
                                        ScreenShotValues.LocY = 0;
                                }
                                break;
                            //
                            case "ExitMode":
                                if (n.InnerText.Trim() == "v6" || n.InnerText.Trim() == "v4")
                                    ScreenShotValues.mode = n.InnerText.Trim();
                                else
                                    ScreenShotValues.mode = "v6"; // default
                                break;

                            case "ResetFlag":
                                if (n.InnerText.Trim() != "")
                                {
                                    if (n.InnerText.Trim() == "true")
                                        ScreenShotValues.ResetFlag = true;
                                    else
                                        ScreenShotValues.ResetFlag = false;
                                }
                                break;

                            case "ResetFlag_v4":
                                if (n.InnerText.Trim() != "")
                                {
                                    if (n.InnerText.Trim() == "true")
                                        ScreenShotValues.ResetFlag_v4 = true;
                                    else
                                        ScreenShotValues.ResetFlag_v4 = false;
                                }
                                break;

                            // v6 part:
                            case "Address":
                                if (n.InnerText.Trim() != "")
                                {
                                    string addr = n.InnerText.Trim();

                                    if (v6ST.IsAddressCorrect(addr))
                                    {
                                        ScreenShotValues.Address = addr;
                                    }
                                    else
                                    {
                                        ScreenShotValues.Address = "";
                                    }
                                }
                                break;

                            case "is128Checked":
                                if (n.InnerText.Trim() != "")
                                {
                                    if (!Boolean.TryParse(n.InnerText.Trim(), out ScreenShotValues.is128Checked))
                                    {
                                        ScreenShotValues.is128Checked = false;
                                    }
                                }
                                break;

                            case "initialAddrSpaceNo":
                                if (n.InnerText.Trim() != "")
                                {
                                    if (!BigInteger.TryParse(n.InnerText.Trim(), out ScreenShotValues.initialAddrSpaceNo))
                                        ScreenShotValues.initialAddrSpaceNo = BigInteger.Zero;
                                }
                                break;

                            case "TrackBar1Value":
                                if (n.InnerText.Trim() != "")
                                {
                                    if (!Int32.TryParse(n.InnerText.Trim(), out ScreenShotValues.TrackBar1Value))
                                        ScreenShotValues.TrackBar1Value = 1;
                                }
                                break;

                            case "currentAddrSpaceNo":
                                if (n.InnerText.Trim() != "")
                                {
                                    if (!BigInteger.TryParse(n.InnerText.Trim(), out ScreenShotValues.currentAddrSpaceNo))
                                        ScreenShotValues.currentAddrSpaceNo = BigInteger.Zero;
                                }
                                break;

                            case "isSubnetChecked":
                                if (n.InnerText.Trim() != "")
                                {
                                    if (!Boolean.TryParse(n.InnerText.Trim(), out ScreenShotValues.isSubnetChecked))
                                        ScreenShotValues.isSubnetChecked = false;
                                }
                                break;

                            case "TrackBar2Value":
                                if (n.InnerText.Trim() != "")
                                {
                                    if (!Int32.TryParse(n.InnerText.Trim(), out ScreenShotValues.TrackBar2Value))
                                        ScreenShotValues.TrackBar2Value = 1;
                                }
                                break;

                            case "isEndChecked":
                                if (n.InnerText.Trim() != "")
                                {
                                    if (!Boolean.TryParse(n.InnerText.Trim(), out ScreenShotValues.isEndChecked))
                                        ScreenShotValues.isEndChecked = false;
                                }
                                break;

                            // v4 Part:

                            case "Address_v4":
                                if (n.InnerText.Trim() != "")
                                {
                                    string addr = n.InnerText.Trim();

                                    if (v6ST.IsAddressCorrect_v4(addr))
                                    {
                                        ScreenShotValues.Address_v4 = addr;
                                    }
                                    else
                                    {
                                        ScreenShotValues.Address_v4 = "";
                                    }
                                }
                                break;

                            case "initialAddrSpaceNo_v4":
                                if (n.InnerText.Trim() != "")
                                {
                                    if (!BigInteger.TryParse(n.InnerText.Trim(), out ScreenShotValues.initialAddrSpaceNo_v4))
                                        ScreenShotValues.initialAddrSpaceNo_v4 = BigInteger.Zero;
                                }
                                break;

                            case "TrackBar1Value_v4":
                                if (n.InnerText.Trim() != "")
                                {
                                    if (!Int32.TryParse(n.InnerText.Trim(), out ScreenShotValues.TrackBar1Value_v4))
                                        ScreenShotValues.TrackBar1Value_v4 = 1;
                                }
                                break;

                            case "currentAddrSpaceNo_v4":
                                if (n.InnerText.Trim() != "")
                                {
                                    if (!BigInteger.TryParse(n.InnerText.Trim(), out ScreenShotValues.currentAddrSpaceNo_v4))
                                        ScreenShotValues.currentAddrSpaceNo_v4 = BigInteger.Zero;
                                }
                                break;

                            case "isSubnetChecked_v4":
                                if (n.InnerText.Trim() != "")
                                {
                                    if (!Boolean.TryParse(n.InnerText.Trim(), out ScreenShotValues.isSubnetChecked_v4))
                                    {
                                        ScreenShotValues.isSubnetChecked_v4 = false;
                                    }
                                }
                                break;

                            case "TrackBar2Value_v4":
                                if (n.InnerText.Trim() != "")
                                {
                                    if (!Int32.TryParse(n.InnerText.Trim(), out ScreenShotValues.TrackBar2Value_v4))
                                    {
                                        ScreenShotValues.TrackBar2Value_v4 = 1;
                                    }
                                }
                                break;

                            case "isEndChecked_v4":
                                if (n.InnerText.Trim() != "")
                                {
                                    if (!Boolean.TryParse(n.InnerText.Trim(), out ScreenShotValues.isEndChecked_v4))
                                    {
                                        ScreenShotValues.isEndChecked_v4 = false;
                                    }
                                }
                                break;
                            //
                            default:
                                break;
                        }
                    }

                    // Load Fonts

                    node = xmldoc.SelectSingleNode("INFO/Fonts_Form1");

                    foreach (XmlNode n in node.ChildNodes)
                    {
                        switch (n.Name)
                        {
                            case "textBox2Font":
                                if (n.InnerText.Trim() != "")
                                    ScreenShotValues.textBox2Font = n.InnerText.Trim();
                                break;
                            case "textBox1Font":
                                if (n.InnerText.Trim() != "")
                                    ScreenShotValues.textBox1Font = n.InnerText.Trim();
                                break;
                            case "textBox3Font":
                                if (n.InnerText.Trim() != "")
                                    ScreenShotValues.textBox3Font = n.InnerText.Trim();
                                break;
                            case "textBox5Font":
                                if (n.InnerText.Trim() != "")
                                    ScreenShotValues.textBox5Font = n.InnerText.Trim();
                                break;
                            case "textBox4Font":
                                if (n.InnerText.Trim() != "")
                                    ScreenShotValues.textBox4Font = n.InnerText.Trim();
                                break;
                            case "textBox8Font":
                                if (n.InnerText.Trim() != "")
                                    ScreenShotValues.textBox8Font = n.InnerText.Trim();
                                break;
                            case "Form1_listBox1Font":
                                if (n.InnerText.Trim() != "")
                                    ScreenShotValues.Form1_listBox1Font = n.InnerText.Trim();
                                break;
                            //
                            default:
                                break;
                        }
                    }

                    // Load DBServerInfo

                    node = xmldoc.SelectSingleNode("INFO/DBServerInfo");
                    foreach (XmlNode n in node.ChildNodes)
                    {
                        switch (n.Name)
                        {
                            case "DriverName":
                                if (n.InnerText.Trim() != "")
                                    ScreenShotValues.DriverName = n.InnerText.Trim();
                                break;
                            case "ServerIP":
                                if (n.InnerText.Trim() != "")
                                    try
                                    {
                                        ScreenShotValues.ServerIP = System.Net.IPAddress.Parse(n.InnerText.Trim());
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Exception: XMLFile.IPAddress.Parse()" + Environment.NewLine 
                                            + ex.Message, "IPAddressParse()", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                break;
                            case "PortNum":
                                if (n.InnerText.Trim() != "")
                                    try
                                    {
                                        ScreenShotValues.PortNum = UInt16.Parse(n.InnerText.Trim());
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Exception: XMLFile.UInt16.Parse()" + Environment.NewLine
                                            + ex.Message, "UInt16.Parse()", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                break;
                            case "DBname":
                                if (n.InnerText.Trim() != "")
                                    ScreenShotValues.DBname = n.InnerText.Trim();
                                break;
                            case "Tablename":
                                if (n.InnerText.Trim() != "")
                                    ScreenShotValues.Tablename = n.InnerText.Trim();
                                break;
                            case "DBname_v4":
                                if (n.InnerText.Trim() != "")
                                    ScreenShotValues.DBname_v4 = n.InnerText.Trim();
                                break;
                            case "Tablename_v4":
                                if (n.InnerText.Trim() != "")
                                    ScreenShotValues.Tablename_v4 = n.InnerText.Trim();
                                break;
                            case "Username":
                                if (n.InnerText.Trim() != "")
                                    ScreenShotValues.Username = n.InnerText.Trim();
                                break;
                            default:
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Exception:\r\n" + ex.Message, "Exception:XMLFile.ReadValues()",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                #endregion
            }
            else
            {
                ScreenShotValues.Cultur = new System.Globalization.CultureInfo("en-US");
            }

            return true;
        }

        public bool WriteValues()
        {
            /* Write Info into the XML file. This file is used like Registry.
             * PS: Order is significant!
             */

            #region XML File Write

            string currentMode = IPv6SubnettingTool.Form1.ipmode;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.Encoding = System.Text.Encoding.UTF8;
            settings.OmitXmlDeclaration = false;

            try
            {
                using (XmlWriter writer = XmlWriter.Create(this.xmlFilename, settings))
                {
                    writer.WriteComment("IPv6 Subnet Calculator generated XML file." + Environment.NewLine
                        + "    Modifying the contents is not recommended." + Environment.NewLine
                        + "    ProductVersion: " + Application.ProductVersion);

                    writer.WriteStartElement("INFO");

                    writer.WriteStartElement("UI_Info");
                    writer.WriteElementString("Culture", ScreenShotValues.Cultur.Name);

                    writer.WriteElementString("LocX", ScreenShotValues.LocX.ToString());
                    writer.WriteElementString("LocY", ScreenShotValues.LocY.ToString());

                    writer.WriteElementString("ExitMode", currentMode);

                    if (ScreenShotValues.ResetFlag)
                        writer.WriteElementString("ResetFlag", "true");
                    else
                        writer.WriteElementString("ResetFlag", "false");

                    if (ScreenShotValues.ResetFlag_v4)
                        writer.WriteElementString("ResetFlag_v4", "true");
                    else
                        writer.WriteElementString("ResetFlag_v4", "false");

                    if (currentMode == "v6") // if exiting in v6 mode:
                    {
                        // v6 values:
                        writer.WriteComment(" IPv6 Part: ");
                        if (v6ST.IsAddressCorrect(ScreenShotValues.Address))
                            writer.WriteElementString("Address", ScreenShotValues.Address);
                        else
                            writer.WriteElementString("Address", "");

                        if (ScreenShotValues.is128Checked)
                            writer.WriteElementString("is128Checked", "true");
                        else
                            writer.WriteElementString("is128Checked", "false");

                        writer.WriteElementString("initialAddrSpaceNo", ScreenShotValues.initialAddrSpaceNo.ToString());
                        writer.WriteElementString("TrackBar1Value", ScreenShotValues.TrackBar1Value.ToString());
                        writer.WriteElementString("currentAddrSpaceNo", ScreenShotValues.currentAddrSpaceNo.ToString());

                        if (ScreenShotValues.isSubnetChecked)
                            writer.WriteElementString("isSubnetChecked", "true");
                        else
                            writer.WriteElementString("isSubnetChecked", "false");

                        writer.WriteElementString("TrackBar2Value", ScreenShotValues.TrackBar2Value.ToString());

                        if (ScreenShotValues.isEndChecked)
                            writer.WriteElementString("isEndChecked", "true");
                        else
                            writer.WriteElementString("isEndChecked", "false");

                        // v4 values:
                        writer.WriteComment(" IPv4 Part: ");
                        if (v6ST.IsAddressCorrect_v4(ScreenShotValues.Address_v4))
                            writer.WriteElementString("Address_v4", ScreenShotValues.Address_v4);
                        else
                            writer.WriteElementString("Address_v4", "");

                        writer.WriteElementString("initialAddrSpaceNo_v4", ScreenShotValues.initialAddrSpaceNo_v4.ToString());
                        writer.WriteElementString("TrackBar1Value_v4", ScreenShotValues.TrackBar1Value_v4.ToString());
                        writer.WriteElementString("currentAddrSpaceNo_v4", ScreenShotValues.currentAddrSpaceNo_v4.ToString());
                        writer.WriteElementString("isSubnetChecked_v4", ScreenShotValues.isSubnetChecked_v4.ToString());
                        writer.WriteElementString("TrackBar2Value_v4", ScreenShotValues.TrackBar2Value_v4.ToString());
                        writer.WriteElementString("isEndChecked_v4", ScreenShotValues.isEndChecked_v4.ToString());
                    }
                    else // v4 : if exiting in v4 mode:
                    {
                        // v4 values:
                        writer.WriteComment(" IPv4 Part: ");
                        if (v6ST.IsAddressCorrect_v4(ScreenShotValues.Address_v4))
                            writer.WriteElementString("Address_v4", ScreenShotValues.Address_v4);
                        else
                            writer.WriteElementString("Address_v4", "");

                        writer.WriteElementString("initialAddrSpaceNo_v4", ScreenShotValues.initialAddrSpaceNo_v4.ToString());
                        writer.WriteElementString("TrackBar1Value_v4", ScreenShotValues.TrackBar1Value_v4.ToString());
                        writer.WriteElementString("currentAddrSpaceNo_v4", ScreenShotValues.currentAddrSpaceNo_v4.ToString());

                        if (ScreenShotValues.isSubnetChecked_v4)
                            writer.WriteElementString("isSubnetChecked_v4", "true");
                        else
                            writer.WriteElementString("isSubnetChecked_v4", "false");

                        writer.WriteElementString("TrackBar2Value_v4", ScreenShotValues.TrackBar2Value_v4.ToString());

                        if (ScreenShotValues.isEndChecked_v4)
                            writer.WriteElementString("isEndChecked_v4", "true");
                        else
                            writer.WriteElementString("isEndChecked_v4", "false");

                        // v6 values:
                        writer.WriteComment(" IPv6 Part: ");
                        if (v6ST.IsAddressCorrect(ScreenShotValues.Address))
                            writer.WriteElementString("Address", ScreenShotValues.Address);
                        else
                            writer.WriteElementString("Address", "");

                        writer.WriteElementString("is128Checked", ScreenShotValues.is128Checked.ToString());
                        writer.WriteElementString("initialAddrSpaceNo", ScreenShotValues.initialAddrSpaceNo.ToString());
                        writer.WriteElementString("TrackBar1Value", ScreenShotValues.TrackBar1Value.ToString());
                        writer.WriteElementString("currentAddrSpaceNo", ScreenShotValues.currentAddrSpaceNo.ToString());
                        writer.WriteElementString("isSubnetChecked", ScreenShotValues.isSubnetChecked.ToString());
                        writer.WriteElementString("TrackBar2Value", ScreenShotValues.TrackBar2Value.ToString());
                        writer.WriteElementString("isEndChecked", ScreenShotValues.isEndChecked.ToString());
                    }
                    writer.WriteEndElement();
                    //
                    writer.WriteStartElement("Fonts_Form1");

                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));

                    writer.WriteElementString("textBox2Font", ScreenShotValues.textBox2Font);
                    writer.WriteElementString("textBox1Font", ScreenShotValues.textBox1Font);
                    writer.WriteElementString("textBox3Font", ScreenShotValues.textBox3Font);
                    writer.WriteElementString("textBox5Font", ScreenShotValues.textBox5Font);
                    writer.WriteElementString("textBox4Font", ScreenShotValues.textBox4Font);
                    writer.WriteElementString("textBox8Font", ScreenShotValues.textBox8Font);
                    writer.WriteElementString("Form1_listBox1Font", ScreenShotValues.Form1_listBox1Font);

                    writer.WriteEndElement();
                    //
                    writer.WriteStartElement("DBServerInfo");

                    if (ScreenShotValues.DriverName != null)
                        writer.WriteElementString("DriverName", ScreenShotValues.DriverName);
                    else
                        writer.WriteElementString("DriverName", "");

                    if (ScreenShotValues.ServerIP != null)
                        writer.WriteElementString("ServerIP", ScreenShotValues.ServerIP.ToString());
                    else
                        writer.WriteElementString("ServerIP", "");

                    if (ScreenShotValues.PortNum.ToString() != null)
                        writer.WriteElementString("PortNum", ScreenShotValues.PortNum.ToString());
                    else
                        writer.WriteElementString("PortNum", "");

                    if (ScreenShotValues.DBname != null)
                        writer.WriteElementString("DBname", ScreenShotValues.DBname);
                    else
                        writer.WriteElementString("DBname", "");

                    if (ScreenShotValues.Tablename != null)
                        writer.WriteElementString("Tablename", ScreenShotValues.Tablename);
                    else
                        writer.WriteElementString("Tablename", "");

                    if (ScreenShotValues.DBname_v4 != null)
                        writer.WriteElementString("DBname_v4", ScreenShotValues.DBname_v4);
                    else
                        writer.WriteElementString("DBname_v4", "");

                    if (ScreenShotValues.Tablename_v4 != null)
                        writer.WriteElementString("Tablename_v4", ScreenShotValues.Tablename_v4);
                    else
                        writer.WriteElementString("Tablename_v4", "");

                    if (ScreenShotValues.Username != null)
                        writer.WriteElementString("Username", ScreenShotValues.Username);
                    else
                        writer.WriteElementString("Username", "");
                    writer.WriteEndElement();

                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception:\r\n" + ex.Message, "Exception:XMLFile.WriteValues()", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            #endregion

            return true;
        }
    }
}