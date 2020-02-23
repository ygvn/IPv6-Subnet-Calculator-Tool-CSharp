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

namespace IPv6SubnettingTool
{
    public class AttributeValues
    {
        /// <summary>
        /// AttributeValues is a storage class for the Address Registries.
        /// </summary>
        public const int numberofvars = 15;    // Number of storage variables in this class
        public string strAddressBlock;         // AddressBlock as a string
        public short AssignedPrefixLength;     // Prefix-Length of AddressBlock
        public int SelectedPrefixLength;
        public bool isMulticast;

        #region RFC6890
        /// <summary>
        /// The following information structure is from:
        /// RFC6890, Special-Purpose IP Address Registries.
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
        #endregion RFC6890

        public AttributeValues()
        {
            this.strAddressBlock = "";
            this.AssignedPrefixLength = 0;
            this.SelectedPrefixLength = 1;
            this.isMulticast = false;

            this.AddressBlock = BigInteger.Zero;
            this.Name = "";
            this.RFC = "";
            this.AllocationDate = "";
            this.TerminationDate = "";
            this.Source = false;
            this.Destination = false;
            this.Forwardable = false;
            this.Global = false;
            this.ReservedByProtocol = false;
        }

        public void Initialize()
        {
            this.strAddressBlock = "";
            this.AssignedPrefixLength = 0;
            this.SelectedPrefixLength = 1;
            this.isMulticast = false;

            this.AddressBlock = BigInteger.Zero;
            this.Name = "";
            this.RFC = "";
            this.AllocationDate = "";
            this.TerminationDate = "";
            this.Source = false;
            this.Destination = false;
            this.Forwardable = false;
            this.Global = false;
            this.ReservedByProtocol = false;
        }
    }
}
