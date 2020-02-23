/*
 * Copyright (c) 2010-2020 Yucel Guven
 * All rights reserved.
 * 
 * This file is part of IPv6 Subnetting Tool.
 * 
 * Version: 4.4
 * Published Date: 23 February 2020
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
using System.Net;
using System.Numerics;

namespace IPv6SubnettingTool
{
    static class ScreenShotValues
    {
        public static string mode = "v6";             // default mode
        public static int LocX = 0;
        public static int LocY = 0;
        public static bool ResetFlag = false;
        public static bool ResetFlag_v4 = false;
        //
        public static CultureInfo Cultur = null;

        // v6
        public static string Address = "";            // "2001:db8:abcd:1234::";
        public static BigInteger initialAddrSpaceNo = BigInteger.Zero;
        public static BigInteger currentAddrSpaceNo = BigInteger.Zero;
        public static bool is128Checked = false;      // checkBox2
        public static bool isSubnetChecked = false;   // checkBox1
        public static bool isEndChecked = false;      // checkBox3
        public static int TrackBar1Value = 1;
        public static int TrackBar2Value = 1;

        // v4
        public static string Address_v4 = "";         // "192.168.10.0";
        public static BigInteger initialAddrSpaceNo_v4 = BigInteger.Zero;
        public static BigInteger currentAddrSpaceNo_v4 = BigInteger.Zero;
        public static bool isSubnetChecked_v4 = false;
        public static bool isEndChecked_v4 = false;
        public static int TrackBar1Value_v4 = 1;
        public static int TrackBar2Value_v4 = 1;

        // Fonts
        public static string textBox2Font = "";       // Address:
        public static string textBox1Font = "";       // IPv6:
        public static string textBox3Font = "";       // Start:
        public static string textBox5Font = "";       // End:
        public static string textBox4Font = "";       // AddrSpaceNo:
        public static string textBox8Font = "";       // Mask:
        public static string Form1_listBox1Font = ""; // ListBox1

        // DBInfo
        public static string DriverName = "";
        public static IPAddress ServerIP = null;
        public static UInt16 PortNum = 3306;
        public static string DBname = "";
        public static string Tablename = "";
        public static string DBname_v4 = "";
        public static string Tablename_v4 = "";
        public static string Username = "";

        /// <summary>
        /// Initialize all values - default or minValue
        /// </summary>
        // 
        public static void Initialize()
        {
            mode = "v6";                                             // default mode
            ResetFlag = false;                                       // Reset_Clicked()?
            ResetFlag_v4 = false;
            Cultur = new System.Globalization.CultureInfo("en-US");  // default lang.
            
            // v6
            Address = "";
            initialAddrSpaceNo = BigInteger.Zero;
            currentAddrSpaceNo = BigInteger.Zero;
            is128Checked = false;
            isSubnetChecked = false;
            isEndChecked = false;
            TrackBar1Value = 1;
            TrackBar2Value = 1;
            
            // v4
            Address_v4 = "";
            initialAddrSpaceNo_v4 = BigInteger.Zero;
            currentAddrSpaceNo_v4 = BigInteger.Zero;
            isSubnetChecked_v4 = false;
            isEndChecked_v4 = false;
            TrackBar1Value_v4 = 1;
            TrackBar2Value_v4 = 1;
            
            // Fonts
            textBox2Font = "";
            textBox1Font = "";
            textBox3Font = "";
            textBox5Font = "";
            textBox4Font = "";
            textBox8Font = "";
            Form1_listBox1Font = "";
            
            // DBInfo
            DriverName = "";
            ServerIP = null;
            PortNum = 3306;
            DBname = "";
            Tablename = "";
            DBname_v4 = "";
            Tablename_v4 = "";
            Username = "";
        }
    }
}
