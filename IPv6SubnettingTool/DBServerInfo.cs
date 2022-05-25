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

using System;
using System.Net;

namespace IPv6SubnettingTool
{

    public class DBServerInfo
    {
        public string DriverName = "MySQL ODBC 8.0 Unicode Driver";
        public IPAddress ServerIP = null;
        public UInt16 PortNum = 3306;
        public string DBname = "";
        public string Tablename = "";
        public string DBname_v4 = "";
        public string Tablename_v4 = "";
        public string Username = "";
        public string Password = "";
        public bool Trytoconnect = false;
        public string ConnectionString = "";
        public bool launchDBUI = false;

        public DBServerInfo() { }

        public DBServerInfo ShallowCopy()
        {
            return (DBServerInfo)this.MemberwiseClone();
        }

        public void Initialize()
        {
            this.DriverName = "";
            this.ServerIP = null;
            this.PortNum = 3306;
            this.DBname = "";
            this.Tablename = "";
            this.DBname_v4 = "";
            this.Tablename_v4 = "";
            this.Username = "";
            this.Password = "";
            this.Trytoconnect = false;
            this.ConnectionString = "";
            this.launchDBUI = false;
        }
    }
}
