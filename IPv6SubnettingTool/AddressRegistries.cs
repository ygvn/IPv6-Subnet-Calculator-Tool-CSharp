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

using System.Numerics;
using System.Collections.Generic;
using System.Globalization;

namespace IPv6SubnettingTool
{
    public class AddressRegistries
    {
        public struct structSparAttribs
        {
            public string strAddressBlock;
            public short AssignedPrefixLength;

            #region fromRFC6890
            /// <summary>
            /// This structure is used for Special-Purpose IP Address Registries, SPARs.
            /// The following information structure is from:
            /// RFC6890: Special-Purpose IP Address Registries.
            /// https://tools.ietf.org/html/rfc6890
            /// </summary>
            public BigInteger AddressBlock;
            public string Name;
            public string RFC;
            public string AllocationDate;
            public string TerminationDate;
            public bool Source;
            public bool Destination;
            public bool Forwardable;
            public bool Global;
            public bool ReservedByProtocol;
            #endregion fromRFC6890
        }
        public struct structMcastAttribs
        {
            /// <summary>
            /// This structure is used for IPv6 Multicast Address Space Registry
            /// https://www.iana.org/assignments/ipv6-multicast-addresses/ipv6-multicast-addresses.xhtml 
            /// Reference RFCs: RFC3307, RFC4291
            /// </summary>
            public string strAddressBlock;
            public short AssignedPrefixLength;
            public BigInteger AddressBlock;
            public string Name;
            public string RFC;
            public string AllocationDate;
            public string TerminationDate;
            /* Excluded due to lack of standardized info:
            public bool Source;
            public bool Destination;
            public bool Forwardable;
            public bool Global;
            public bool ReservedByProtocol;
            */
        }

        public List<structSparAttribs> Spars;           /* List of Special-Purpose Address Registries */
        public List<structMcastAttribs> McastAddresses;  /* List of Multicast Address Space Registries */
        string s;

        public AddressRegistries() // Default Constructor
        {
            #region Special-Purpose Address Registries

            /* Special-Purpose IP Address Registries
             * From: https://tools.ietf.org/html/rfc6890
             * Reference RFCs: RFC4291, RFC6890, RFC8190
             */
            Spars = new List<structSparAttribs>();
            structSparAttribs SparsAttribs;
            s = "";

            // 1) ::1/128 , Loopback address
            SparsAttribs.strAddressBlock = "::1/128";
            s = "00000000000000000000000000000001";
            SparsAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            SparsAttribs.AssignedPrefixLength = 128;
            SparsAttribs.Name = "Loopback Address";
            SparsAttribs.RFC = "4291";
            SparsAttribs.AllocationDate = "February 2006";
            SparsAttribs.TerminationDate = "N/A";
            SparsAttribs.Source = false;
            SparsAttribs.Destination = false;
            SparsAttribs.Forwardable = false;
            SparsAttribs.Global = false;
            SparsAttribs.ReservedByProtocol = true;
            Spars.Add(SparsAttribs);
            // 2) ::/128 , Unspecified Address
            SparsAttribs.strAddressBlock = "::/128";
            s = "00000000000000000000000000000000";
            SparsAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            SparsAttribs.AssignedPrefixLength = 128;
            SparsAttribs.Name = "Unspecified Address";
            SparsAttribs.RFC = "4291";
            SparsAttribs.AllocationDate = "February 2006";
            SparsAttribs.TerminationDate = "N/A";
            SparsAttribs.Source = true;
            SparsAttribs.Destination = false;
            SparsAttribs.Forwardable = false;
            SparsAttribs.Global = false;
            SparsAttribs.ReservedByProtocol = true;
            Spars.Add(SparsAttribs);
            // 3) 64:ff9b::/96 , IPv4-IPv6 Translation Address
            SparsAttribs.strAddressBlock = "64:ff9b::/96";
            s = "0064ff9b000000000000000000000000";
            SparsAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            SparsAttribs.AssignedPrefixLength = 96;
            SparsAttribs.Name = "IPv4-IPv6 Translat.";
            SparsAttribs.RFC = "6052";
            SparsAttribs.AllocationDate = "October 2010";
            SparsAttribs.TerminationDate = "N/A";
            SparsAttribs.Source = true;
            SparsAttribs.Destination = true;
            SparsAttribs.Forwardable = true;
            SparsAttribs.Global = true;
            SparsAttribs.ReservedByProtocol = false;
            Spars.Add(SparsAttribs);
            // 4) ::ffff:0:0/96 , IPv4-mapped Address
            SparsAttribs.strAddressBlock = "::ffff:0:0/96";
            s = "00000000000000000000ffff00000000";
            SparsAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            SparsAttribs.AssignedPrefixLength = 96;
            SparsAttribs.Name = "IPv4-mapped Address";
            SparsAttribs.RFC = "4291";
            SparsAttribs.AllocationDate = "February 2006";
            SparsAttribs.TerminationDate = "N/A";
            SparsAttribs.Source = false;
            SparsAttribs.Destination = false;
            SparsAttribs.Forwardable = false;
            SparsAttribs.Global = false;
            SparsAttribs.ReservedByProtocol = true;
            Spars.Add(SparsAttribs);
            // 5) 100::/64 , Discard-Only Address Block
            SparsAttribs.strAddressBlock = "100::/64";            
            s = "01000000000000000000000000000000";
            SparsAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            SparsAttribs.AssignedPrefixLength = 64;
            SparsAttribs.Name = "Discard-Only Address Block";
            SparsAttribs.RFC = "6666";
            SparsAttribs.AllocationDate = "June 2012";
            SparsAttribs.TerminationDate = "N/A";
            SparsAttribs.Source = true;
            SparsAttribs.Destination = true;
            SparsAttribs.Forwardable = true;
            SparsAttribs.Global = false;
            SparsAttribs.ReservedByProtocol = false;
            Spars.Add(SparsAttribs);
            // 6) 2001::/23 , IETF Protocol Assignments
            SparsAttribs.strAddressBlock = "2001::/23";            
            s = "20010000000000000000000000000000";
            SparsAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            SparsAttribs.AssignedPrefixLength = 23;
            SparsAttribs.Name = "IETF Protocol Assignments";
            SparsAttribs.RFC = "2928";
            SparsAttribs.AllocationDate = "September 2000";
            SparsAttribs.TerminationDate = "N/A";
            SparsAttribs.Source = false;
            SparsAttribs.Destination = false;
            SparsAttribs.Forwardable = false;
            SparsAttribs.Global = false;
            SparsAttribs.ReservedByProtocol = false;
            Spars.Add(SparsAttribs);
            // 7) 2001::/32 , TEREDO
            SparsAttribs.strAddressBlock = "2001::/32";            
            s = "20010000000000000000000000000000";
            SparsAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            SparsAttribs.AssignedPrefixLength = 32;
            SparsAttribs.Name = "TEREDO";
            SparsAttribs.RFC = "4380";
            SparsAttribs.AllocationDate = "January 2006";
            SparsAttribs.TerminationDate = "N/A";
            SparsAttribs.Source = true;
            SparsAttribs.Destination = true;
            SparsAttribs.Forwardable = true;
            SparsAttribs.Global = false;
            SparsAttribs.ReservedByProtocol = false;
            Spars.Add(SparsAttribs);
            // 8) 2001:2::/48 , Benchmarking
            SparsAttribs.strAddressBlock = "2001:2::/48";            
            s = "20010002000000000000000000000000";
            SparsAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            SparsAttribs.AssignedPrefixLength = 48;
            SparsAttribs.Name = "Benchmarking";
            SparsAttribs.RFC = "5180";
            SparsAttribs.AllocationDate = "April 2008";
            SparsAttribs.TerminationDate = "N/A";
            SparsAttribs.Source = true;
            SparsAttribs.Destination = true;
            SparsAttribs.Forwardable = true;
            SparsAttribs.Global = false;
            SparsAttribs.ReservedByProtocol = false;
            Spars.Add(SparsAttribs);
            // 9) 2001:db8::/32 , Documentation
            SparsAttribs.strAddressBlock = "2001:db8::/32";            
            s = "20010db8000000000000000000000000";
            SparsAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            SparsAttribs.AssignedPrefixLength = 32;
            SparsAttribs.Name = "Documentation";
            SparsAttribs.RFC = "3849";
            SparsAttribs.AllocationDate = "July 2004";
            SparsAttribs.TerminationDate = "N/A";
            SparsAttribs.Source = false;
            SparsAttribs.Destination = false;
            SparsAttribs.Forwardable = false;
            SparsAttribs.Global = false;
            SparsAttribs.ReservedByProtocol = false;
            Spars.Add(SparsAttribs);
            // 10) 2001:10::/28 , ORCHID
            SparsAttribs.strAddressBlock = "2001:10::/28";            
            s = "20010010000000000000000000000000";
            SparsAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            SparsAttribs.AssignedPrefixLength = 28;
            SparsAttribs.Name = "ORCHID";
            SparsAttribs.RFC = "4843";
            SparsAttribs.AllocationDate = "March 2007";
            SparsAttribs.TerminationDate = "March 2014";
            SparsAttribs.Source = false;
            SparsAttribs.Destination = false;
            SparsAttribs.Forwardable = false;
            SparsAttribs.Global = false;
            SparsAttribs.ReservedByProtocol = false;
            Spars.Add(SparsAttribs);
            // 11) 2002::/16 , 6to4
            SparsAttribs.strAddressBlock = "2002::/16";            
            s = "20020000000000000000000000000000";
            SparsAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            SparsAttribs.AssignedPrefixLength = 16;
            SparsAttribs.Name = "6to4";
            SparsAttribs.RFC = "3056";
            SparsAttribs.AllocationDate = "February 2001";
            SparsAttribs.TerminationDate = "N/A";
            SparsAttribs.Source = true;
            SparsAttribs.Destination = true;
            SparsAttribs.Forwardable = true;
            SparsAttribs.Global = false;
            SparsAttribs.ReservedByProtocol = false;
            Spars.Add(SparsAttribs);
            // 12) fc00::/7 , ULA (Unique-Local Address)
            SparsAttribs.strAddressBlock = "fc00::/7";            
            s = "fc000000000000000000000000000000";
            SparsAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            SparsAttribs.AssignedPrefixLength = 7;
            SparsAttribs.Name = "ULA (Unique-Local Address)";
            SparsAttribs.RFC = "4193";
            SparsAttribs.AllocationDate = "October 2005";
            SparsAttribs.TerminationDate = "N/A";
            SparsAttribs.Source = true;
            SparsAttribs.Destination = true;
            SparsAttribs.Forwardable = true;
            SparsAttribs.Global = false;
            SparsAttribs.ReservedByProtocol = false;
            Spars.Add(SparsAttribs);
            // 13) fe80::/10 , Linked-Scoped Unicast
            SparsAttribs.strAddressBlock = "fe80::/10";
            s = "fe800000000000000000000000000000";
            SparsAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            SparsAttribs.AssignedPrefixLength = 10;
            SparsAttribs.Name = "Linked-Scoped Unicast";
            SparsAttribs.RFC = "4291";
            SparsAttribs.AllocationDate = "February 2006";
            SparsAttribs.TerminationDate = "N/A";
            SparsAttribs.Source = true;
            SparsAttribs.Destination = true;
            SparsAttribs.Forwardable = false;
            SparsAttribs.Global = false;
            SparsAttribs.ReservedByProtocol = true;
            Spars.Add(SparsAttribs);
            
            // 14) fe80::/64 , Link-Local IPv6 Unicast Addresses
            SparsAttribs.strAddressBlock = "fe80::/64";
            s = "fe800000000000000000000000000000";
            SparsAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            SparsAttribs.AssignedPrefixLength = 64;
            SparsAttribs.Name = "Link-Local IPv6 Unicast Addresses";
            SparsAttribs.RFC = "4291";
            SparsAttribs.AllocationDate = "";
            SparsAttribs.TerminationDate = "";
            SparsAttribs.Source = true;
            SparsAttribs.Destination = true;
            SparsAttribs.Forwardable = false;
            SparsAttribs.Global = false;
            SparsAttribs.ReservedByProtocol = false;
            Spars.Add(SparsAttribs);

            // add the new ones...

            #endregion Special-Purpose Address Registries

            #region IPv6 Multicast Address Space Registry

            /* IPv6 Multicast Address Space Registry - IANA
             * https://www.iana.org/assignments/ipv6-multicast-addresses/ipv6-multicast-addresses.xhtml
             * Reference RFCs: RFC3307, RFC4291
             */
            McastAddresses = new List<structMcastAttribs>();
            structMcastAttribs McastAttribs;
            s = "";

            // 1) 
            McastAttribs.strAddressBlock = "FF01:0:0:0:0:0:0:1";
            s = "FF010000000000000000000000000001";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "All Nodes interface-local";
            McastAttribs.RFC = "4291";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 2)
            McastAttribs.strAddressBlock = "FF01:0:0:0:0:0:0:2";
            s = "FF010000000000000000000000000002";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "All Routers interface-local";
            McastAttribs.RFC = "4291";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 3)
            McastAttribs.strAddressBlock = "FF01:0:0:0:0:0:0:3";
            s = "FF010000000000000000000000000003";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "Unassigned";
            McastAttribs.RFC = "Jon_Postel";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 4)
            McastAttribs.strAddressBlock = "FF01:0:0:0:0:0:0:FB";
            s = "FF0100000000000000000000000000FB";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "mDNSv6";
            McastAttribs.RFC = "6762";
            McastAttribs.AllocationDate = "2005-10-05";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 5)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:1";
            s = "FF020000000000000000000000000001";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "All Nodes link-local";
            McastAttribs.RFC = "4291";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 6)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:2";
            s = "FF020000000000000000000000000002";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "All Routers link-local";
            McastAttribs.RFC = "4291";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 7)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:4";
            s = "FF020000000000000000000000000004";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "DVMRP Routers";
            McastAttribs.RFC = "1075";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 8)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:5";
            s = "FF020000000000000000000000000005";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "OSPFv3 All SPF routers";
            McastAttribs.RFC = "2328";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 9)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:6";
            s = "FF020000000000000000000000000006";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "OSPFv3 All DR routers";
            McastAttribs.RFC = "2328";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 10)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:7";
            s = "FF020000000000000000000000000007";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "ST Routers (Exp. Internet Stream Protocol)";
            McastAttribs.RFC = "1190";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 11)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:8";
            s = "FF020000000000000000000000000008";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "ST Hosts (Exp. Internet Stream Protocol)";
            McastAttribs.RFC = "1190";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 12)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:9";
            s = "FF020000000000000000000000000009";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "RIP routers";
            McastAttribs.RFC = "2080";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 13)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:A";
            s = "FF02000000000000000000000000000A";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "EIGRP routers";
            McastAttribs.RFC = "7868";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 14)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:B";
            s = "FF02000000000000000000000000000B";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "Mobile-Agents";
            McastAttribs.RFC = "";
            McastAttribs.AllocationDate = "1994-11-01";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 15)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:C";
            s = "FF02000000000000000000000000000C";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "SSDP";
            McastAttribs.RFC = "";
            McastAttribs.AllocationDate = "2006-09-21";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 16)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:D";
            s = "FF02000000000000000000000000000D";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "All PIM routers";
            McastAttribs.RFC = "";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 17)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:E";
            s = "FF02000000000000000000000000000E";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "RSVP-ENCAPSULATION";
            McastAttribs.RFC = "";
            McastAttribs.AllocationDate = "1996-04-01";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 18)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:F";
            s = "FF02000000000000000000000000000F";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "UPnP";
            McastAttribs.RFC = "";
            McastAttribs.AllocationDate = "2006-09-21";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 19)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:10";
            s = "FF020000000000000000000000000010";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "All-BBF-Access-Nodes";
            McastAttribs.RFC = "6788";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 20)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:11";
            s = "FF020000000000000000000000000011";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "All-Homenet-Nodes";
            McastAttribs.RFC = "7788";
            McastAttribs.AllocationDate = "2016-01-05";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 21)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:12";
            s = "FF020000000000000000000000000012";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "VRRP";
            McastAttribs.RFC = "5798";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 22)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:13";
            s = "FF020000000000000000000000000013";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "ALL_GRASP_NEIGHBORS";
            McastAttribs.RFC = "RFC-ietf-anima-grasp-15";
            McastAttribs.AllocationDate = "2017-07-20";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 23)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:16";
            s = "FF020000000000000000000000000016";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "All MLDv2-capable routers";
            McastAttribs.RFC = "3810";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 24)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:1A";
            s = "FF02000000000000000000000000001A";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "All RPL nodes";
            McastAttribs.RFC = "6550";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 25)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:6A";
            s = "FF02000000000000000000000000006A";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "All-Snoopers";
            McastAttribs.RFC = "4286";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 26)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:6B";
            s = "FF02000000000000000000000000006B";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "PTP-pdelay";
            McastAttribs.RFC = "";
            McastAttribs.AllocationDate = "2007-02-02";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 27)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:6C";
            s = "FF02000000000000000000000000006C";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "Saratoga";
            McastAttribs.RFC = "";
            McastAttribs.AllocationDate = "2007-08-30";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 28)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:6D";
            s = "FF02000000000000000000000000006D";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "LL-MANET-Routers";
            McastAttribs.RFC = "5498";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 29)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:6E";
            s = "FF02000000000000000000000000006E";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "IGRS";
            McastAttribs.RFC = "";
            McastAttribs.AllocationDate = "2009-01-20";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 30)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:6F";
            s = "FF02000000000000000000000000006F";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "iADT Discovery";
            McastAttribs.RFC = "";
            McastAttribs.AllocationDate = "2009-05-12";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 31)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:0:FB";
            s = "FF0200000000000000000000000000FB";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "mDNSv6";
            McastAttribs.RFC = "6762";
            McastAttribs.AllocationDate = "2005-10-05";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 32)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:1:1";
            s = "FF020000000000000000000000010001";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "Link Name";
            McastAttribs.RFC = "6762";
            McastAttribs.AllocationDate = "1996-07-01";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 33)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:1:2";
            s = "FF020000000000000000000000010002";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "All_DHCP_Relay_Agents_and_Servers -local";
            McastAttribs.RFC = "RFC-ietf-dhc-rfc3315bis-13";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 34)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:1:3";
            s = "FF020000000000000000000000010003";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "All LLMNR/Link-local Mcast Name Resolution Hosts";
            McastAttribs.RFC = "4795";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 35)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:1:4";
            s = "FF020000000000000000000000010004";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "DTCP Announcement";
            McastAttribs.RFC = "";
            McastAttribs.AllocationDate = "2004-05-01";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 36)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:1:5";
            s = "FF020000000000000000000000010005";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "afore_vdp";
            McastAttribs.RFC = "";
            McastAttribs.AllocationDate = "2010-11-30";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 37)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:1:6";
            s = "FF020000000000000000000000010006";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "Babel";
            McastAttribs.RFC = "6126";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 38)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:0:1:7";
            s = "FF020000000000000000000000010007";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "DLEP Discovery";
            McastAttribs.RFC = "8175";
            McastAttribs.AllocationDate = "2017-04-03";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 39)
            McastAttribs.strAddressBlock = "FF02::1:FF00:0000/104";
            s = "FF0200000000000000000001FF000000";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 104;
            McastAttribs.Name = "Solicited-Node Address";
            McastAttribs.RFC = "4291";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 40)
            McastAttribs.strAddressBlock = "FF02:0:0:0:0:2:FF00::/104";
            s = "FF0200000000000000000002FF000000";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 104;
            McastAttribs.Name = "Node Information Queries";
            McastAttribs.RFC = "4620";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 41)
            McastAttribs.strAddressBlock = "FF05:0:0:0:0:0:0:2";
            s = "FF050000000000000000000000000002";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "All Routers Address";
            McastAttribs.RFC = "4291";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 42)
            McastAttribs.strAddressBlock = "FF05:0:0:0:0:0:0:FB";
            s = "FF0500000000000000000000000000FB";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "mDNSv6";
            McastAttribs.RFC = "6762";
            McastAttribs.AllocationDate = "2005-10-05";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 43)
            McastAttribs.strAddressBlock = "FF05:0:0:0:0:0:1:3";
            s = "FF050000000000000000000000010003";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "All DHCP Servers";
            McastAttribs.RFC = "3315";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);
            // 44)
            McastAttribs.strAddressBlock = "FF05:0:0:0:0:0:1:5";
            s = "FF050000000000000000000000010005";
            McastAttribs.AddressBlock = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);
            McastAttribs.AssignedPrefixLength = 128;
            McastAttribs.Name = "SL-MANET-ROUTERS";
            McastAttribs.RFC = "6621";
            McastAttribs.AllocationDate = "";
            McastAttribs.TerminationDate = "";
            //McastAttribs.Source = false;
            //McastAttribs.Destination = false;
            //McastAttribs.Forwardable = false;
            //McastAttribs.Global = false;
            //McastAttribs.ReservedByProtocol = false;
            McastAddresses.Add(McastAttribs);

            // add the new ones...

            #endregion IPv6 Multicast Address Space Registry

        }
    }
}
