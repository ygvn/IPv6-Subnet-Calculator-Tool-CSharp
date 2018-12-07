/*
 * Copyright (c) 2010-2019 Yucel Guven
 * All rights reserved.
 * 
 * This file is part of IPv6 Subnetting Tool.
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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Numerics;
using System.Windows.Forms;

namespace IPv6SubnettingTool
{
    public class v6ST
    {
        public v6ST() { }

        #region specific variables -yucel
        //
        public AddressRegistries addrRegs = new AddressRegistries();
        public AttributeValues attribs = new AttributeValues();
        public static String errmsg = "";
        public const string arpa = "ip6.arpa.";
        private BigInteger mask = BigInteger.Zero;

        /// <summary>
        /// Indicates AS number conversion type.
        /// <b>toASplain</b>: Input will be converted from as-dot to as-plain notation.
        /// Pass it like 'v6ST.toASplain'.
        /// </summary>
        public const Boolean toASplain = true;
        
        /// <summary>
        /// Indicates AS number conversion type.
        /// <b>toASdot</b>: Input will be converted from as-plain to as-dot notation.
        /// Pass it like 'v6ST.toASdot'.
        /// </summary>
        public const Boolean toASdot = false;
        
        private const long asMax = 4294967295;
        #endregion

        /// <summary>
        /// Checks the entered IPv6 address
        /// and returns 'true' or 'false'
        /// </summary>
        /// <param name="sin">input string</param>
        /// <returns>boolean</returns>
        public bool IsAddressCorrect(string sin)
        {
            if (sin == null)
            {
                errmsg = "e0";
                return false;
            }

            sin = sin.Trim();
            string s = sin;

            int nDC = 0;
            int nC = 0;

            /* 0. Error: Empty */
            if (s == "")
            {
                errmsg = "e0";
                return false;
            }

            /* 1. Error: Unspecified '::' */
            if (s == "::")
            {
                errmsg = "e1";
                return false;
            }

            /* 2. Error: Triple or more colons entered */
            if ((s.Length <= 1) || (s.Contains(":::")))
            {
                errmsg = "e2";
                return false;
            }

            /* 3. Error: Not valid hex */
            if (Regex.Matches(s, "^[0-9A-Fa-f:]+$").Count == 0)
            {
                errmsg = "e3";
                return false;
            }

            /* 4. Error: Cannot start or end with ':' */
            if (s[0] == ':' && s[1] != ':')
            {
                errmsg = "e4";
                return false;
            }
            if (s[s.Length - 1] == ':' && s[s.Length - 2] != ':')
            {
                errmsg = "e4";
                return false;
            }

            /* 5. Error: More than 2 Bytes */
            s = sin;
            string[] sa = s.Split(':');
            for (int j = 0; j < sa.Length; j++)
            {
                if (sa[j].Length > 4)
                {
                    errmsg = "e5";
                    return false;
                }
            }

            /* 6. Error: Number of double-colon and colon */
            s = sin;
            nDC = Regex.Matches(s, "::").Count;
            s = s.Replace("::", "**");
            nC = Regex.Matches(s, ":").Count;

            /* 6.Error: Case I. double-colon '::' can only appear once in an address - RFC4291 */
            if (nDC > 1)
            {
                errmsg = "e6_1";
                return false;
            }

            /* 6.Error: Case II. No double-colon means there must be 7 colons */
            if (nDC == 0 && nC != 7)
            {
                errmsg = "e6_2";
                return false;
            }

            /* 6.Error: Case III. If double-colon is at the start or end, max. colons must be 6 or less */
            s = sin;
            int sL = s.Length;
            if ((s[0] == ':' && s[1] == ':')
                 ||
                 (s[sL - 1] == ':' && s[sL - 2] == ':')
               )
            {
                if (nDC == 1 && nC > 6)
                {
                    errmsg = "e6_3";
                    return false;
                }
            }

              /* 6.Error: Case IV. If double-colon is in the middle, max. colons must be 5 or less */
            else if (nDC == 1 && nC > 5)
            {
                errmsg = "e6_4";
                return false;
            }

            errmsg = "e_ok";
            return true;
        }

        /// <summary>
        /// Formalizes a valid/correct IPv6 address
        /// to 16 bytes uncompressed form without colons.
        /// </summary>
        /// <param name="sin">input string</param>
        /// <returns>string IPv6 address</returns>
        public string FormalizeAddr(string sin)
        {
            if (sin == "" || sin == null)
                return "00000000000000000000000000000000";

            string[] Resv6 = new string[8] { "0000", "0000", "0000", "0000", "0000", "0000", "0000", "0000" };

            sin = sin.Trim();
            sin = sin.ToLower();
            string s = sin;

            string[] sa = s.Split(':');

            s = s.Replace("::", "**");
            int nC = Regex.Matches(s, ":").Count;

            /* Start of Building Result v6 address */
            for (int k = 0; k < sa.Length; k++)
            {
                if (sa[k].Length == 0)
                    continue;
                else sa[k] = sa[k].PadLeft(4, '0');
            }

            if ((sa[sa.Length - 1].Length == 0) &&
                 (sa[sa.Length - 2].Length == 0))
            {
                int t = nC + 1;
                for (int i = 0; i < t; i++)
                    Resv6[i] = sa[i];
            }
            else if (sa[0].Length == 0 && sa[1].Length == 0)
            {
                int t = nC + 1;
                for (int i = 0; i < t; i++)
                    Resv6[7 - i] = sa[sa.Length - 1 - i];
            }
            else
            {
                int idx = Array.IndexOf(sa, "");

                for (int i = 0; i < idx; i++)
                    Resv6[i] = sa[i];

                for (int i = 0; i < sa.Length - idx - 1; i++)
                    Resv6[7 - i] = sa[sa.Length - 1 - i];
            }
            /* End of Building Result v6 address */

            string sResultv6 = string.Empty;
            for (int i = 0; i < 8; i++)
                sResultv6 += Resv6[i];

            return sResultv6;
        }

        /// <summary>
        /// Inserts colons into the string of IPv6 address
        /// <remarks>(input IPv6 address must be 'formalized' and have no colons).</remarks>
        /// </summary>
        /// <param name="sin">input string</param>
        /// <param name="is128Checked">CheckState</param>
        /// <returns>string IPv6 address with colons</returns>
        public string Kolonlar(string sin, CheckState is128Checked)
        {
            sin = sin.Trim();
            string str = null;

            if (is128Checked == CheckState.Unchecked)
            {
                if (sin.Length > 16)
                {
                    sin = sin.PadRight(32, '0');
                    for (int i = 0; i < 32; i++)
                    {
                        if (i % 4 == 0)
                            str += ":";
                        str += sin.Substring(i, 1);
                    }
                    str = str.Trim(':');
                }
                else
                {
                    sin = sin.PadLeft(16, '0');
                    for (int i = 0; i < 16; i++)
                    {
                        if (i % 4 == 0)
                            str += ":";
                        str += sin.Substring(i, 1);
                    }
                    str = str.TrimStart(':');
                    str += "::";
                }
            }
            else if (is128Checked == CheckState.Checked)
            {
                if (sin.Length > 32)
                {
                    sin = sin.PadRight(32, '0');
                    for (int i = 0; i < 32; i++)
                    {
                        if (i % 4 == 0)
                            str += ":";
                        str += sin.Substring(i, 1);
                    }
                    str = str.Trim(':');
                }
                else
                {
                    sin = sin.PadLeft(32, '0');
                    for (int i = 0; i < 32; i++)
                    {
                        if (i % 4 == 0)
                            str += ":";
                        str += sin.Substring(i, 1);
                    }
                    str = str.TrimStart(':');
                }
            }

            return str;
        }

        /// <summary>
        /// Compresses a given valid IPv6 address. (Address types are not considered)
        /// </summary>
        /// <param name="sin">input string</param>
        /// <param name="is128Checked">CheckState</param>
        /// <returns>string compressed IPv6 address with colons</returns>
        public string CompressAddress(string sin)
        {
            string s = "";
            if (this.IsAddressCorrect(sin))
            {
                //sin = this.Kolonlar(this.FormalizeAddr(sin), CheckState.Checked);
                sin = this.Kolonlar(FormalizeAddr(sin), CheckState.Checked);

                string p = "";
                UInt16[] pref = new UInt16[8];
                int[] idx = new int[8];

                string[] sprfx = sin.Split(':');

                for (int i = 0; i < 8; i++)
                {
                    pref[i] = UInt16.Parse(sprfx[i], System.Globalization.NumberStyles.AllowHexSpecifier);
                }
                //Sıfır OLMAYAN bölümler: non-zero parts
                for (int i = 0; i < 8; i++)
                {
                    if (pref[i] > 0)
                        idx[i] = 1;
                }

                // Sıfır OLAN bölümlerin uzunlukları: Length of the zero parts
                int n = 0, z = 0, fark = 0, max = 0;
                int[] aralik = new int[2];
                for (int i = 0; i < 8; i++)
                {
                    if (idx[i] != 0)
                    {
                        n = z = i;
                        n++;
                    }
                    else
                    {
                        z = i;
                        fark = z - n;
                        if (fark > max)
                        {
                            max = fark;
                            aralik[0] = n;
                            aralik[1] = z;
                        }
                    }
                }

                if (aralik[0] == aralik[1])
                {
                    string st ="";
                    foreach (ushort nn in pref)
                    {
                        if (nn == 0)
                            st += "*" + ":";
                        else
                            st += nn.ToString("x") + ":";
                    }
                    st = st.TrimEnd(':');
                    
                    int c = st.Split('*').Length - 1;

                    if (c == 1)
                    {
                        st = st.Replace("*", ":");
                    }
                    else if (c > 1)
                    {
                        var regexp = new Regex(Regex.Escape("*"));
                        st = regexp.Replace(st, ":", 1);
                    }
                    st = st.Replace(":::", "::");
                    st = st.Replace('*', '0');

                    return st;
                }

                for (int i = 0; i < 8; i++)
                {
                    if (i == aralik[0])
                        p += "::";
                    if (i < aralik[0] || i > aralik[1])
                    {
                        if ( i != 7)
                            p += pref[i].ToString("x") + ":";
                        else
                            p += pref[i].ToString("x");
                    }
                }
                p = p.Replace(":::", "::");

                return p;
            }
            else
                return s;
        }

        /// <summary>
        /// Calculates Start, End, subnetindex, Upper and Lower limits of a given (SEaddress) IPv6 address
        /// by using 'Start' address and prefix.
        /// </summary>
        /// <param name="input">SEaddress</param>
        /// <param name="is128Checked">CheckState</param>
        /// <returns>SEaddress</returns>
        public SEaddress StartEndAddresses(SEaddress input, CheckState is128Checked)
        {
            input.subnetidx = BigInteger.Zero;
            int delta = input.subnetslash - input.slash;
            int count = 0;

            if (is128Checked == CheckState.Unchecked)
                count = 63;
            else
                count = 127;

            for (int i = (count - input.slash); i > (count - input.subnetslash); i--)
            {
                if (BitTest(input.Start, i, is128Checked) == 1)
                {
                    input.subnetidx = BitSet(input.subnetidx, delta, is128Checked);
                }
                else
                {
                    input.subnetidx = BitClear(input.subnetidx, delta, is128Checked);
                }
                delta--;
            }

            if (is128Checked == CheckState.Unchecked)
            {
                if (input.slash == 64)
                {
                    input.Start = input.End = input.Resultv6;
                    return input;
                }
            }
            else if (is128Checked == CheckState.Checked)
            {
                if (input.slash == 128)
                {
                    input.Start = input.End = input.Resultv6;
                    return input;
                }
            }

            mask = InitializeMask(is128Checked);
            mask = (mask >> (input.slash));
            input.End = (input.Resultv6 | mask);
            mask = ~mask;
            input.Start = (input.Resultv6 & mask);

            input.LowerLimitAddress = input.Start;
            input.UpperLimitAddress = input.End;

            return input;
        }

        /// <summary>
        /// Calculates Start, End and subnetindex of a given (SEaddress) IPv6 address
        /// by using 'End' address.
        /// </summary>
        /// <param name="input">SEaddress</param>
        /// <param name="is128Checked">CheckState</param>
        /// <returns>SEaddress</returns>
        public SEaddress EndStartAddresses(SEaddress input, CheckState is128Checked)
        {
            input.subnetidx = BigInteger.Zero;
            int delta = input.subnetslash - input.slash - 1;
            int count = 0;

            if (is128Checked == CheckState.Unchecked)
                count = 63;
            else
                count = 127;


            for (int i = (count - input.slash); i > (count - input.subnetslash); i--)
            {
                if (BitTest(input.End, i, is128Checked) == 1)
                {
                    input.subnetidx = BitSet(input.subnetidx, delta, is128Checked);
                }
                else
                {
                    input.subnetidx = BitClear(input.subnetidx, delta, is128Checked);
                }
                delta--;
            }


            if (is128Checked == CheckState.Unchecked)
            {
                if (input.slash == 64)
                {
                    input.Start = input.End;
                    return input;
                }
            }
            else if (is128Checked == CheckState.Checked)
            {
                if (input.slash == 128)
                {
                    input.Start = input.End;
                    return input;
                }
            }

            input.Start = BigInteger.Zero;
            mask = InitializeMask(is128Checked);
            mask = (mask >> input.subnetslash);
            mask = ~mask;
            input.Start = (input.End & mask);

            return input;
        }

        /// <summary>
        /// Calculates next adjacent Start address
        /// </summary>
        /// <param name="inStart">BigInteger</param>
        /// <param name="subnetslash">int</param>
        /// <param name="is128Checked">CheckState</param>
        /// <returns>BigInteger</returns>
        public BigInteger NextStart(BigInteger inStart, int subnetslash, CheckState is128Checked)
        {
            BigInteger tmpend = BigInteger.Zero;

            if (is128Checked == CheckState.Unchecked)
            {
                if (subnetslash == 64)
                    return inStart;
            }
            else if (is128Checked == CheckState.Checked)
            {
                if (subnetslash == 128)
                    return inStart;
            }

            mask = InitializeMask(is128Checked);
            mask = (mask >> subnetslash);
            tmpend = (inStart | mask);
            tmpend += BigInteger.One;
            inStart = tmpend;

            return inStart;
        }

        /// <summary>
        /// Calculates subnets (Start, End, subnetindex) of a given (SEaddress) IPv6 address
        /// by using 'Start' address and subnetprefix.
        /// </summary>
        /// <param name="input">SEaddress</param>
        /// <param name="is128Checked">CheckState</param>
        /// <returns>SEaddress</returns>
        public SEaddress Subnetting(SEaddress input, CheckState is128Checked)
        {
            input.subnetidx = BigInteger.Zero;
            int delta = input.subnetslash - input.slash - 1;
            int count = 0;

            if (is128Checked == CheckState.Unchecked)
                count = 63;
            else
                count = 127;

            for (int i = (count - input.slash); i > (count - input.subnetslash); i--)
            {
                if (BitTest(input.Start, i, is128Checked) == 1)
                {
                    input.subnetidx = BitSet(input.subnetidx, delta, is128Checked);
                }
                else
                {
                    input.subnetidx = BitClear(input.subnetidx, delta, is128Checked);
                }
                delta--;
            }

            if (is128Checked == CheckState.Unchecked)
            {
                if (input.subnetslash == 64)
                {
                    input.End = input.Start;
                    return input;
                }
            }
            else if (is128Checked == CheckState.Checked)
            {
                if (input.subnetslash == 128)
                {
                    input.End = input.Start;
                    return input;
                }
            }

            mask = InitializeMask(is128Checked);
            mask = (mask >> input.subnetslash);
            input.End = (input.Start | mask);

            return input;
        }

        /// <summary>
        /// Find the prefix of a given (SEaddress) IPv6 address
        /// 
        /// </summary>
        /// <param name="input">SEaddress</param>
        /// <param name="is128Checked">CheckState</param>
        /// <returns>SEaddress</returns>
        public SEaddress FindPrefixIndex(SEaddress input, CheckState is128Checked)
        {
            input.subnetidx = BigInteger.Zero;
            int delta = input.subnetslash - input.slash - 1;
            int count = 0;

            if (is128Checked == CheckState.Unchecked)
                count = 63;
            else
                count = 127;

            for (int i = (count - input.slash); i > (count - input.subnetslash); i--)
            {
                if (BitTest(input.Start, i, is128Checked) == 1)
                {
                    input.subnetidx = BitSet(input.subnetidx, delta, is128Checked);
                }
                else
                {
                    input.subnetidx = BitClear(input.subnetidx, delta, is128Checked);
                }
                delta--;
            }

            return input;
        }

        /// <summary>
        /// Calculates the prefix index number.
        /// </summary>
        /// <param name="input">SEaddress</param>
        /// <param name="is128Checked">CheckState</param>
        /// <returns>SEaddress</returns>
        public SEaddress RangeIndex(SEaddress input, CheckState is128Checked)
        {
            input.subnetidx = BigInteger.Zero;
            int count = 0;

            if (is128Checked == CheckState.Unchecked)
                count = 63;
            else if (is128Checked == CheckState.Checked)
                count = 127;

            int delta = count - input.subnetslash;

            for (int i = delta; i >= 0; i--)
            {
                if (BitTest(input.Start, i, is128Checked) == 1)
                {
                    input.subnetidx = BitSet(input.subnetidx, delta, is128Checked);
                }
                else
                {
                    input.subnetidx = BitClear(input.subnetidx, delta, is128Checked);
                }
                delta--;
            }

            return input;
        }

        /// <summary>
        /// Goes to user-entered address space number.
        /// </summary>
        /// <param name="input">SEaddress</param>
        /// <param name="is128Checked">CheckState</param>
        /// <returns>SEaddress</returns>
        public SEaddress GoToAddrSpace(SEaddress input, CheckState is128Checked)
        {
            int count = 0;

            if (is128Checked == CheckState.Unchecked)
                count = 64;
            else
                count = 128;

            int bitStart = count - input.slash;
            int bitUpto = count;

            for (int i = bitStart; i < bitUpto; i++)
            {
                if ((input.subnetidx & BigInteger.One) == BigInteger.One)
                    input.Start = BitSet(input.Start, i, is128Checked);
                else
                    input.Start = BitClear(input.Start, i, is128Checked);

                input.subnetidx >>= 1;
            }

            if (is128Checked == CheckState.Unchecked)
            {
                if (input.slash == 64)
                {
                    input.End = input.Start;
                    return input;
                }
            }
            else if (is128Checked == CheckState.Checked)
            {
                if (input.slash == 128)
                {
                    input.End = input.Start;
                    return input;
                }
            }

            mask = InitializeMask(is128Checked);
            mask = (mask >> (input.slash));
            input.End = (input.Start | mask);
            input.subnetidx = AddressSpaceNo(input.Start, input.slash, is128Checked);

            input.LowerLimitAddress = input.Start;
            input.UpperLimitAddress = input.End;

            return input;
        }

        /// <summary>
        /// Goes to user-entered subnet number (or index number).
        /// </summary>
        /// <param name="input">SEaddress</param>
        /// <param name="is128Checked">CheckState</param>
        /// <returns>SEaddress</returns>
        public SEaddress GoToSubnet(SEaddress input, CheckState is128Checked)
        {
            int count = 0;

            if (is128Checked == CheckState.Unchecked)
                count = 64;
            else
                count = 128;

            int bitStart = count - input.subnetslash;
            int bitUpto = count - input.slash;

            for (int i = bitStart; i < bitUpto; i++)
            {
                if ((input.subnetidx & 1) == 1)
                    input.Start = BitSet(input.Start, i, is128Checked);
                else
                    input.Start = BitClear(input.Start, i, is128Checked);

                input.subnetidx >>= 1;
            }

            if (is128Checked == CheckState.Unchecked)
            {
                if (input.slash == 64)
                {
                    input.End = input.Start;
                    return input;
                }
            }
            else if (is128Checked == CheckState.Checked)
            {
                if (input.slash == 128)
                {
                    input.End = input.Start;
                    return input;
                }
            }

            return input;
        }

        /// <summary>
        /// Goes to next address space when user clicks on NextSpace button.
        /// </summary>
        /// <param name="input">SEaddress</param>
        /// <param name="is128Checked">CheckState</param>
        /// <returns>SEaddress</returns>
        public SEaddress NextSpace(SEaddress input, CheckState is128Checked)
        {
            input.subnetidx = AddressSpaceNo(input.Start, input.slash, is128Checked);

            if (is128Checked == CheckState.Unchecked)
            {
                if (input.slash == 64)
                {
                    input.Start = input.End = input.End + 1;
                    return input;
                }
            }
            else if (is128Checked == CheckState.Checked)
            {
                if (input.slash == 128)
                {
                    if (input.End == BigInteger.Pow(2, 128) - BigInteger.One)
                    {
                        input.Start = input.End = BigInteger.Zero;
                        return input;
                    }
                    else
                    {
                        input.Start = input.End = input.End + BigInteger.One;
                        return input;
                    }
                }
            }

            mask = InitializeMask(is128Checked);
            mask = (mask >> (input.slash));
            input.End = (input.Start | mask);

            input.LowerLimitAddress = input.Start;
            input.UpperLimitAddress = input.End;

            return input;
        }

        /// <summary>
        /// Goes to previous address space when user clicks on PrevSpace button.
        /// </summary>
        /// <param name="input">SEaddress</param>
        /// <param name="is128Checked">CheckState</param>
        /// <returns>SEaddress</returns>
        public SEaddress PrevSpace(SEaddress input, CheckState is128Checked)
        {
            input.subnetidx = AddressSpaceNo(input.End, input.slash, is128Checked);

            if (is128Checked == CheckState.Unchecked)
            {
                if (input.slash == 64)
                {
                    if (input.Start == BigInteger.Zero)
                        input.Start = input.End = BigInteger.Pow(2, 64) - 1;
                    else
                        input.Start = input.End = input.Start - 1;

                    return input;
                }
            }
            else if (is128Checked == CheckState.Checked)
            {
                if (input.slash == 128)
                {
                    input.Start = input.End;
                    return input;
                }
            }

            mask = InitializeMask(is128Checked);
            mask = (mask >> input.slash);
            mask = ~mask;
            input.Start = (input.End & mask);

            input.LowerLimitAddress = input.Start;
            input.UpperLimitAddress = input.End;

            return input;
        }

        /// <summary>
        /// Finds the address space number (or index number).
        /// </summary>
        /// <param name="address">BigInteger</param>
        /// <param name="slash">prefix</param>
        /// <param name="is128Checked">CheckState</param>
        /// <returns></returns>
        private BigInteger AddressSpaceNo(BigInteger address, int slash, CheckState is128Checked)
        {
            BigInteger spaceno = BigInteger.Zero;
            int delta = slash - 1;

            int count = 0;

            if (is128Checked == CheckState.Unchecked)
                count = 63;
            else
                count = 127;


            for (int i = count; i > (count - slash); i--)
            {
                if (BitTest(address, i, is128Checked) == 1)
                {
                    spaceno = BitSet(spaceno, delta, is128Checked);
                }
                else
                {
                    spaceno = BitClear(spaceno, delta, is128Checked);
                }
                delta--;
            }

            return spaceno;
        }

        /// <summary>
        /// Returns the mask of given prefix-length value.
        /// </summary>
        /// <param name="pflen">int16(short)</param>
        /// <returns>BigInteger mask</returns>
        public BigInteger PrepareMask(short pflen)
        {
            if (pflen == 128)
                return (BigInteger.Pow(2, 128) - BigInteger.One);

            if (pflen > 127 || pflen < 0)
                return BigInteger.Zero;

            int delta = 127 - pflen;
            mask = BigInteger.Zero;

            for (int i = 127; i > delta; i--)
            {
                mask = (mask | (BigInteger.One << i));
            }

            return mask;
        }

        public string FindParentNet(string prefix, short parentpflen, CheckState is128Checked)
        {
            mask = PrepareMask(parentpflen);

            BigInteger Resultv6 = BigInteger.Zero;

            if (IsAddressCorrect(prefix))
            {
                string Resv6 = FormalizeAddr(prefix);
                Resv6 = "0" + Resv6;
                Resultv6 = BigInteger.Parse(Resv6, NumberStyles.AllowHexSpecifier);
                Resultv6 = Resultv6 & mask;
                string s = String.Format("{0:x}", Resultv6);

                if (is128Checked == CheckState.Checked) /* 128 bits */
                {
                    if (s.Length > 32)
                        s = s.Substring(1, 32);
                    s = Kolonlar(s, is128Checked);
                    s = CompressAddress(s);
                    s = s + "/" + parentpflen;
                    
                    return s;
                }
                else /* unchecked 64 bits*/
                {
                    s = s.Substring(0, 16); /* From left First 64bits */

                    if (s.Length > 16)
                        s = s.Substring(1, 16);
                    s = Kolonlar(s, is128Checked);
                    s = CompressAddress(s);
                    s = s + "/" + parentpflen;
                    return s;
                }
            }
            else
                return null;
        }

        /// <summary>
        /// Checks whether the bit at ith. position is 0 or 1.
        /// </summary>
        /// <param name="ui128">BigInteger</param>
        /// <param name="i">int</param>
        /// <param name="is128Checked">CheckState</param>
        /// <returns>0, 1, or -1</returns>
        public int BitTest(BigInteger ui128, int i, CheckState is128Checked)
        {
            /* if ith bit  is set, returns 1,
             * if ith bit not set, returns 0,
             * if i>127 or i<0 (or if i>63 or i<0) returns -1
             */

            if (is128Checked == CheckState.Unchecked)
            {
                if (i > 63 || i < 0)
                    return -1;
            }
            else if (is128Checked == CheckState.Checked)
            {
                if (i > 127 || i < 0)
                    return -1;
            }
            if ((ui128 & (BigInteger.One << i)) != 0)
                return 1;
            else
                return 0;
        }

        public BigInteger BitSet(BigInteger ui128, int i, CheckState is128Checked)
        {
            if (is128Checked == CheckState.Unchecked)
            {
                if (i > 63 || i < 0)
                    return BigInteger.Zero;
                ui128 = (ui128 | (BigInteger.One << i));
                return ui128;
            }
            else
            {
                if (i > 127 || i < 0)
                    return BigInteger.Zero;

                ui128 = (ui128 | (BigInteger.One << i));
                return ui128;
            }
        }

        public BigInteger BitClear(BigInteger ui128, int i, CheckState is128Checked)
        {
            if (is128Checked == CheckState.Unchecked)
            {
                if (i > 63 || i < 0)
                    return BigInteger.Zero;

                ui128 = (ui128 & ~(BigInteger.One << i));
                return ui128;
            }
            else
            {
                if (i > 127 || i < 0)
                    return BigInteger.Zero;

                ui128 = (ui128 & ~(BigInteger.One << i));
                return ui128;
            }
        }

        /// <summary>
        /// Returns the binary representation of the input number.
        /// </summary>
        /// <param name="n">BigInteger</param>
        /// <param name="border">int</param>
        /// <param name="StartEnd">SEaddress</param>
        /// <param name="is128Checked">CheckState</param>
        /// <returns>0, 1, or -1</returns>
        public string PrintBin(SEaddress StartEnd, int border, CheckState is128Checked)
        {
            BigInteger n = StartEnd.Start;

            string sOut = null;
            int count1 = 0, count2 = 0;

            if (is128Checked == CheckState.Unchecked)
            {
                count1 = 63;
                count2 = 61;

                if (border == 64)
                    n = StartEnd.Resultv6;

                if (n == 0)
                {
                    sOut = "0000 0000 0000 0000:0000 0000 0000 0000:"
                         + "0000 0000 0000 0000:0000 0000 0000 0000";
                    return sOut;
                }
            }

            if (is128Checked == CheckState.Checked)
            {
                count1 = 127;
                count2 = 141;

                if (border == 128)
                    n = StartEnd.Resultv6;

                if (n == 0)
                {
                    sOut = "0000 0000 0000 0000:0000 0000 0000 0000:"
                         + "0000 0000 0000 0000:0000 0000 0000 0000:"
                         + "0000 0000 0000 0000:0000 0000 0000 0000:"
                         + "0000 0000 0000 0000:0000 0000 0000 0000";

                    return sOut;
                }
            }

            for (int bitIndex = count1; bitIndex >= 0; bitIndex--)
            {
                sOut += ((n >> bitIndex) & 1) == 0 ? "0" : "1";
                if (bitIndex % 4 == 0)
                    sOut += " ";
            }

            for (int pos = 0; pos < count2; pos++) // no need to go further
            {
                if (pos % 20 == 0 && pos > 0)
                {
                    sOut = sOut.Remove(pos - 1, 1);
                    sOut = sOut.Insert(pos - 1, ":");
                }
            }

            return sOut;
        }

        public BigInteger InitializeMask(CheckState is128Checked)
        {
            if (is128Checked == CheckState.Unchecked)
            {
                mask = BigInteger.Pow(2, 64) - BigInteger.One;
            }
            else if (is128Checked == CheckState.Checked)
            {
                mask = BigInteger.Pow(2, 128) - BigInteger.One;
            }

            return mask;
        }

        public SEaddress ListFirstPage(SEaddress input, CheckState is128Checked, CheckState isEndChecked)
        {
            SEaddress subnets = new SEaddress();

            subnets.Start = input.Start;
            subnets.slash = input.slash; // this.trackBar1.Value;
            subnets.subnetslash = input.subnetslash; // this.trackBar2.Value;

            subnets.liste.Clear();

            subnets.liste = new List<string>(input.upto * 3);
            //subnets.liste = new List<string>(input.upto);

            subnets.End = BigInteger.Zero;
            subnets.subnetidx = BigInteger.Zero;

            String ss = "", se = "";
            int count = 0;

            if (input.slash.Equals(input.subnetslash))
                input.upto = 1;

            if (is128Checked == CheckState.Unchecked && input.slash == 64)
            {
                subnets.Start = subnets.End = input.Start = input.End = input.Resultv6;
                ss = String.Format("{0:x}", subnets.Start);
                if (ss.Length > 16)
                    ss = ss.Substring(1, 16);
                ss = this.Kolonlar(ss, is128Checked);
                ss = this.CompressAddress(ss);
                //ss = "s" + subnets.subnetidx + "> " + ss + "/"
                ss = "p" + subnets.subnetidx + "> " + ss + "/"
                    + input.slash.ToString();

                subnets.liste.Add(ss);
                return subnets;
            }
            else if (is128Checked == CheckState.Checked && input.slash == 128)
            {
                subnets.Start = subnets.End = input.Start = input.End = input.Resultv6;
                ss = String.Format("{0:x}", subnets.Start);
                if (ss.Length > 32)
                    ss = ss.Substring(1, 32);
                ss = Kolonlar(ss, is128Checked);
                ss = this.CompressAddress(ss);
                //ss = "s" + subnets.subnetidx + "> " + ss + "/" +
                ss = "p" + subnets.subnetidx + "> " + ss + "/" +
                    input.slash.ToString();

                subnets.liste.Add(ss);
                return subnets;
            }
            for (count = 0; count < input.upto; count++)
            {
                subnets = Subnetting(subnets, is128Checked);

                if (is128Checked == CheckState.Unchecked)
                {
                    ss = String.Format("{0:x}", subnets.Start);
                    if (ss.Length > 16)
                        ss = ss.Substring(1, 16);
                    ss = Kolonlar(ss, is128Checked);
                    ss = this.CompressAddress(ss);
                    //ss = "s" + subnets.subnetidx + "> " + ss + "/" +
                    ss = "p" + subnets.subnetidx + "> " + ss + "/" +
                        input.subnetslash.ToString();

                    if (input.subnetslash == 64)
                    {
                        subnets.liste.Add(ss);
                    }
                    else //Value < 64
                    {
                        subnets.liste.Add(ss);

                        if (isEndChecked == CheckState.Checked)
                        {
                            se = String.Format("{0:x}", subnets.End);
                            if (se.Length > 16)
                                se = se.Substring(1, 16);
                            se = Kolonlar(se, is128Checked);
                            se = this.CompressAddress(se);
                            se = "e" + subnets.subnetidx + "> " + se + "/"
                                + input.subnetslash.ToString();

                            //subnets.liste.Add(ss);
                            subnets.liste.Add(se);
                            subnets.liste.Add("");
                        }
                    }
                }
                else if (is128Checked == CheckState.Checked)
                {
                    ss = String.Format("{0:x}", subnets.Start);
                    if (ss.Length > 32)
                        ss = ss.Substring(1, 32);
                    ss = Kolonlar(ss, is128Checked);
                    ss = this.CompressAddress(ss);
                    //ss = "s" + subnets.subnetidx + "> " + ss + "/" +
                    ss = "p" + subnets.subnetidx + "> " + ss + "/" +
                        input.subnetslash.ToString();

                    if (input.subnetslash == 128) // since start=end, no need to print two times.
                    {
                        subnets.liste.Add(ss);
                    }
                    else //Value < 128
                    {
                        subnets.liste.Add(ss);

                        if (isEndChecked == CheckState.Checked)
                        {
                            se = String.Format("{0:x}", subnets.End);
                            if (se.Length > 32)
                                se = se.Substring(1, 32);
                            se = Kolonlar(se, is128Checked);
                            se = this.CompressAddress(se);
                            se = "e" + subnets.subnetidx + "> " + se + "/"
                                + input.subnetslash.ToString();

                            //subnets.liste.Add(ss);
                            subnets.liste.Add(se);
                            subnets.liste.Add("");
                        }
                    }
                }

                if (subnets.End.Equals(input.UpperLimitAddress))
                {
                    break;
                }
                else
                {
                    subnets.Start = subnets.End + BigInteger.One;
                }
            }

            return subnets;
        }

        public SEaddress ListPageBackward(SEaddress input, CheckState is128Checked, CheckState isEndChecked)
        {
            SEaddress subnets = new SEaddress();
            subnets = input;

            subnets.liste = new List<string>(input.upto * 3);
            //subnets.liste = new List<string>(input.upto);

            String ss = "", se = "";
            int count = 0;

            for (count = 0; count < input.upto; count++)
            {
                subnets = EndStartAddresses(subnets, is128Checked);

                if (is128Checked == CheckState.Unchecked)
                {
                    ss = String.Format("{0:x}", subnets.Start);
                    if (ss.Length > 16)
                        ss = ss.Substring(1, 16);
                    ss = Kolonlar(ss, is128Checked);
                    ss = this.CompressAddress(ss);
                    //ss = "s" + subnets.subnetidx + "> " + ss + "/" +
                    ss = "p" + subnets.subnetidx + "> " + ss + "/" +
                        input.subnetslash.ToString();

                    if (input.subnetslash == 64) // since start=end, no need to print two times.
                    {
                        subnets.liste.Add(ss);
                    }
                    else //Value < 64
                    {
                        if (isEndChecked == CheckState.Checked)
                        {
                            se = String.Format("{0:x}", subnets.End);
                            if (se.Length > 16)
                                se = se.Substring(1, 16);
                            se = Kolonlar(se, is128Checked);
                            se = this.CompressAddress(se);
                            se = "e" + subnets.subnetidx + "> " + se + "/" +
                                input.subnetslash.ToString();

                            subnets.liste.Add("");
                            subnets.liste.Add(se);
                        }
                        subnets.liste.Add(ss);

                    }

                }
                else if (is128Checked == CheckState.Checked)
                {
                    ss = String.Format("{0:x}", subnets.Start);
                    if (ss.Length > 32)
                        ss = ss.Substring(1, 32);
                    ss = Kolonlar(ss, is128Checked);
                    ss = this.CompressAddress(ss);
                    //ss = "s" + subnets.subnetidx + "> " + ss + "/" +
                    ss = "p" + subnets.subnetidx + "> " + ss + "/" +
                        input.subnetslash.ToString();

                    if (input.subnetslash == 128) // since start=end, no need to print two times.
                    {
                        subnets.liste.Add(ss);
                    }
                    else //Value < 128
                    {
                        if (isEndChecked == CheckState.Checked)
                        {
                            se = String.Format("{0:x}", subnets.End);
                            if (se.Length > 32)
                                se = se.Substring(1, 32);
                            se = Kolonlar(se, is128Checked);
                            se = this.CompressAddress(se);
                            se = "e" + subnets.subnetidx + "> " + se + "/" +
                                input.subnetslash.ToString();

                            subnets.liste.Add("");
                            subnets.liste.Add(se);
                        }
                        subnets.liste.Add(ss);
                    }
                }
                subnets.End = subnets.Start - BigInteger.One;

                if (subnets.Start.Equals(input.LowerLimitAddress))
                    break;
            }
            subnets.liste.Reverse();

            return subnets;
        }

        public SEaddress ListPageForward(SEaddress input, CheckState is128Checked, CheckState isEndChecked)
        {
            SEaddress subnets = new SEaddress();
            subnets = input;

            subnets.liste = new List<string>(input.upto * 3);
            //subnets.liste = new List<string>(input.upto);
            String ss, se;
            int count = 0;

            for (count = 0; count < input.upto; count++)
            {
                subnets = Subnetting(subnets, is128Checked);

                if (is128Checked == CheckState.Unchecked)
                {
                    ss = String.Format("{0:x}", subnets.Start);
                    if (ss.Length > 16)
                        ss = ss.Substring(1, 16);
                    ss = Kolonlar(ss, is128Checked);
                    ss = this.CompressAddress(ss);
                    //ss = "s" + subnets.subnetidx + "> " + ss + "/" +
                    ss = "p" + subnets.subnetidx + "> " + ss + "/" +
                        input.subnetslash.ToString();

                    if (input.subnetslash == 64) // since start=end, no need to print two times.
                    {
                        subnets.liste.Add(ss);
                    }
                    else //Value < 64
                    {
                        subnets.liste.Add(ss);

                        if (isEndChecked == CheckState.Checked)
                        {
                            se = String.Format("{0:x}", subnets.End);
                            if (se.Length > 16)
                                se = se.Substring(1, 16);
                            se = Kolonlar(se, is128Checked);
                            se = this.CompressAddress(se);
                            se = "e" + subnets.subnetidx + "> " + se + "/" +
                                input.subnetslash.ToString();

                            subnets.liste.Add(se);
                            subnets.liste.Add("");
                        }

                    }
                }
                else if (is128Checked == CheckState.Checked)
                {
                    ss = String.Format("{0:x}", subnets.Start);
                    if (ss.Length > 32)
                        ss = ss.Substring(1, 32);
                    ss = Kolonlar(ss, is128Checked);
                    ss = this.CompressAddress(ss);
                    //ss = "s" + subnets.subnetidx + "> " + ss + "/" +
                    ss = "p" + subnets.subnetidx + "> " + ss + "/" +
                        input.subnetslash.ToString();

                    if (input.subnetslash == 128) // since start=end, no need to print two times.
                    {
                        subnets.liste.Add(ss);
                    }
                    else //Value < 128
                    {
                        subnets.liste.Add(ss);

                        if (isEndChecked == CheckState.Checked)
                        {
                            se = String.Format("{0:x}", subnets.End);
                            if (se.Length > 32)
                                se = se.Substring(1, 32);
                            se = Kolonlar(se, is128Checked);
                            se = this.CompressAddress(se);
                            se = "e" + subnets.subnetidx + "> " + se + "/" +
                                input.subnetslash.ToString();

                            subnets.liste.Add(se);
                            subnets.liste.Add("");
                        }
                    }
                }

                if (subnets.End.Equals(input.UpperLimitAddress)
                    || subnets.subnetidx == BigInteger.Zero)
                {
                    break;
                }
                subnets.Start = subnets.End + BigInteger.One;
            }

            return subnets;
        }

        public SEaddress ListLastPage(SEaddress input, CheckState is128Checked, CheckState isEndChecked)
        {
            SEaddress subnets = new SEaddress();
            subnets = input;

            subnets.liste = new List<string>(input.upto * 3);

            String ss = "", se = "";
            int count = 0;

            subnets.subnetidx = BigInteger.Zero;


            for (count = 0; count < input.upto; count++)
            {
                subnets = EndStartAddresses(subnets, is128Checked);

                if (is128Checked == CheckState.Unchecked)
                {
                    ss = String.Format("{0:x}", subnets.Start);
                    if (ss.Length > 16)
                        ss = ss.Substring(1, 16);
                    ss = Kolonlar(ss, is128Checked);
                    ss = this.CompressAddress(ss);
                    //ss = "s" + subnets.subnetidx + "> " + ss + "/" +
                    ss = "p" + subnets.subnetidx + "> " + ss + "/" +
                        input.subnetslash.ToString();

                    if (input.subnetslash == 64) // since start=end, no need to print two times.
                    {
                        subnets.liste.Add(ss);
                    }
                    else //Value < 64
                    {
                        if (isEndChecked == CheckState.Checked)
                        {
                            se = String.Format("{0:x}", subnets.End);
                            if (se.Length > 16)
                                se = se.Substring(1, 16);
                            se = Kolonlar(se, is128Checked);
                            se = this.CompressAddress(se);
                            se = "e" + subnets.subnetidx + "> " + se + "/" +
                                input.subnetslash.ToString();

                            subnets.liste.Add("");
                            subnets.liste.Add(se);
                        }
                        subnets.liste.Add(ss);
                    }
                }
                else if (is128Checked == CheckState.Checked)
                {
                    ss = String.Format("{0:x}", subnets.Start);
                    if (ss.Length > 32)
                        ss = ss.Substring(1, 32);
                    ss = Kolonlar(ss, is128Checked);
                    ss = this.CompressAddress(ss);
                    //ss = "s" + subnets.subnetidx + "> " + ss + "/" +
                    ss = "p" + subnets.subnetidx + "> " + ss + "/" +
                        input.subnetslash.ToString();

                    if (input.subnetslash == 128) // since start=end, no need to print two times.
                    {
                        subnets.liste.Add(ss);
                    }
                    else //Value < 128
                    {
                        se = String.Format("{0:x}", subnets.End);
                        if (se.Length > 32)
                            se = se.Substring(1, 32);
                        se = Kolonlar(se, is128Checked);
                        se = this.CompressAddress(se);
                        se = "e" + subnets.subnetidx + "> " + se + "/" +
                            input.subnetslash.ToString();

                        subnets.liste.Add("");
                        subnets.liste.Add(se);
                        subnets.liste.Add(ss);
                    }
                }
                subnets.End = subnets.Start - BigInteger.One;
                if (subnets.subnetidx == BigInteger.Zero)
                {
                    break;
                }
            }
            subnets.liste.Reverse();

            return subnets;
        }

        public String[] DnsRev(BigInteger inv6, int subnetslash, CheckState is128Checked)
        {
            string s = String.Format("{0:x}", inv6);
            int count = 0, countarray = 0;

            if (is128Checked == CheckState.Unchecked)
            {
                count = 16;
                countarray = 8;

                if (s.Length > 16)
                    s = s.Substring(1, 16);
            }
            else if (is128Checked == CheckState.Checked)
            {
                count = 32;
                countarray = 16;

                if (s.Length > 32)
                    s = s.Substring(1, 32);
            }

            for (int i = s.Length; i < count; i++)
                s = "0" + s;

            String[] sa = new String[countarray];

            int remainder = subnetslash % 4;
            int len = (subnetslash + (4 - remainder)) / 4;
            int nzones = (1 << (4 - remainder));

            if (subnetslash % 4 == 0) // it's nibble-boundary 
            {                         // we can only work with sa[0]
                sa[0] = String.Format("{0:x}", inv6);

                if (is128Checked == CheckState.Unchecked)
                {
                    for (int i = sa[0].Length; i < 16; i++)
                        sa[0] = "0" + sa[0];
                    if (sa[0].Length > 16)
                        sa[0] = sa[0].Substring(1, 16);
                }
                else if (is128Checked == CheckState.Checked)
                {
                    for (int i = sa[0].Length; i < 32; i++)
                        sa[0] = "0" + sa[0];
                    if (sa[0].Length > 32)
                        sa[0] = sa[0].Substring(1, 32);
                }

                sa[0] = sa[0].Substring(0, len - 1);
                string stmp = null;

                for (int j = sa[0].Length - 1; j >= 0; j--)
                {
                    stmp += sa[0][j] + ".";
                }
                stmp += arpa;
                sa[0] = stmp;
            }
            else // non-nibble boundary
            {
                s = s.Substring(0, len);
                BigInteger zones = BigInteger.Parse("0" + s, NumberStyles.AllowHexSpecifier);

                for (int i = 0; i < nzones; i++)
                {
                    sa[i] = String.Format("{0:x}", zones);
                    sa[i] = sa[i].TrimStart('0');

                    if (sa[i].Length < len)
                        sa[i] = sa[i].PadLeft(len, '0');
                    string stmp = null;

                    for (int j = sa[i].Length - 1; j >= 0; j--)
                    {
                        stmp += sa[i][j] + ".";
                    }
                    stmp += arpa;
                    sa[i] = stmp;
                    zones++;
                }
            }

            return sa;
        }

        public SEaddress ListDnsRevFirstPage(SEaddress input, CheckState is128Checked)
        {
            SEaddress subnets = new SEaddress();

            subnets = input;
            subnets.subnetidx = BigInteger.Zero;
            subnets.liste = new List<string>(subnets.upto * 8);

            int count = 0;
            int spaces = 0;

            String[] sa;
            string sf;

            for (count = 0; count < subnets.upto; count++)
            {
                subnets = Subnetting(subnets, is128Checked);

                sa = DnsRev(subnets.Start, subnets.subnetslash, is128Checked);
                //sf = "s" + subnets.subnetidx + "> " + sa[0];
                sf = "p" + subnets.subnetidx + "> " + sa[0];
                subnets.liste.Add(sf);

                string[] sr = sf.Split(' ');
                spaces = sr[0].Length + 1;

                for (int i = 1; i < 8; i++)
                {
                    if (sa[i] == null)
                        break;
                    sa[i] = sa[i].PadLeft(sa[i].Length + spaces, ' ');
                    subnets.liste.Add(sa[i]);
                }

                if (subnets.End.Equals(input.UpperLimitAddress))
                {
                    break;
                }
                else
                {
                    subnets.Start = subnets.End + BigInteger.One;
                }
            }

            return subnets;
        }

        public SEaddress ListDnsRevPageBackward(SEaddress input, CheckState is128Checked)
        {
            SEaddress subnets = new SEaddress();
            subnets = input;
            subnets.liste = new List<string>(input.upto * 8);

            String[] sa;
            int count = 0;
            int spaces = 0;

            for (count = 0; count < input.upto; count++)
            {
                subnets = EndStartAddresses(subnets, is128Checked);

                sa = DnsRev(subnets.Start, subnets.subnetslash, is128Checked);
                //string sf = "s" + subnets.subnetidx + "> " + sa[0];
                string sf = "p" + subnets.subnetidx + "> " + sa[0];

                string[] sr = sf.Split(' ');
                spaces = sr[0].Length + 1;

                for (int i = 7; i > 0; i--)
                {
                    if (sa[i] == null)
                        continue;
                    sa[i] = sa[i].PadLeft(sa[i].Length + spaces, ' ');
                    subnets.liste.Add(sa[i]);
                }
                subnets.liste.Add(sf);

                subnets.End = subnets.Start - BigInteger.One;

                if (subnets.Start.Equals(input.LowerLimitAddress))
                    break;
            }
            subnets.liste.Reverse();

            return subnets;
        }

        public SEaddress ListDnsRevPageForward(SEaddress input, CheckState is128Checked)
        {
            SEaddress subnets = new SEaddress();
            subnets = input;
            subnets.liste = new List<string>(input.upto * 8);

            int count = 0;
            int spaces = 0;
            String[] sa;

            for (count = 0; count < input.upto; count++)
            {
                subnets = Subnetting(subnets, is128Checked);

                sa = DnsRev(subnets.Start, subnets.subnetslash, is128Checked);
                //string sf = "s" + subnets.subnetidx + "> " + sa[0];
                string sf = "p" + subnets.subnetidx + "> " + sa[0];
                subnets.liste.Add(sf);

                string[] sr = sf.Split(' ');
                spaces = sr[0].Length + 1;

                for (int i = 1; i < 8; i++)
                {
                    if (sa[i] == null)
                        break;
                    sa[i] = sa[i].PadLeft(sa[i].Length + spaces, ' ');
                    subnets.liste.Add(sa[i]);
                }

                if (subnets.End.Equals(input.UpperLimitAddress)
                    || subnets.subnetidx == BigInteger.Zero)
                {
                    break;
                }
                subnets.Start = subnets.End + BigInteger.One;
            }

            return subnets;
        }

        public SEaddress ListDnsRevLastPage(SEaddress input, CheckState is128Checked)
        {
            SEaddress subnets = new SEaddress();
            subnets = input;
            subnets.liste = new List<string>(input.upto * 8);
            subnets.subnetidx = BigInteger.Zero;

            String[] sa;
            int count = 0;
            int spaces = 0;

            for (count = 0; count < input.upto; count++)
            {
                subnets = EndStartAddresses(subnets, is128Checked);

                sa = DnsRev(subnets.Start, subnets.subnetslash, is128Checked);
                //string sf = "s" + subnets.subnetidx + "> " + sa[0];
                string sf = "p" + subnets.subnetidx + "> " + sa[0];

                string[] sr = sf.Split(' ');
                spaces = sr[0].Length + 1;

                for (int i = 7; i > 0; i--)
                {
                    if (sa[i] == null)
                        continue;
                    sa[i] = sa[i].PadLeft(sa[i].Length + spaces, ' ');
                    subnets.liste.Add(sa[i]);
                }
                subnets.liste.Add(sf);

                subnets.End = subnets.Start - BigInteger.One;
                if (subnets.subnetidx == BigInteger.Zero)
                {
                    break;
                }
            }
            subnets.liste.Reverse();

            return subnets;
        }

        public SEaddress ListSubRangeFirstPage(SEaddress input, CheckState is128Checked)
        {
            SEaddress subnets = new SEaddress();
            subnets = input;
            subnets.liste = new List<string>(input.upto);

            subnets.LowerLimitAddress = input.LowerLimitAddress;
            subnets.UpperLimitAddress = input.UpperLimitAddress;

            String ss = "";
            int count = 0;

            for (count = 0; count < input.upto; count++)
            {
                subnets = RangeIndex(subnets, is128Checked);

                if (is128Checked == CheckState.Unchecked)
                {
                    ss = String.Format("{0:x}", subnets.Start);
                    if (ss.Length > 16)
                        ss = ss.Substring(1, 16);
                    ss = Kolonlar(ss, is128Checked);
                    ss = this.CompressAddress(ss);
                    //ss = "s" + subnets.subnetidx + "> " + ss + "/64";
                    ss = "p" + subnets.subnetidx + "> " + ss + "/64";
                }
                else if (is128Checked == CheckState.Checked)
                {
                    ss = String.Format("{0:x}", subnets.Start);
                    if (ss.Length > 32)
                        ss = ss.Substring(1, 32);
                    ss = Kolonlar(ss, is128Checked);
                    ss = this.CompressAddress(ss);
                    //ss = "s" + subnets.subnetidx + "> " + ss + "/128";
                    ss = "p" + subnets.subnetidx + "> " + ss + "/128";
                }

                subnets.liste.Add(ss);

                if (subnets.Start.Equals(input.UpperLimitAddress))
                    break;
                subnets.Start += BigInteger.One;
            }

            return subnets;
        }

        public SEaddress ListSubRangePageBackward(SEaddress input, CheckState is128Checked)
        {
            SEaddress subnets = new SEaddress();
            subnets = input;
            subnets.liste = new List<string>(input.upto);

            String ss = "";
            int count = 0;


            for (count = 0; count < input.upto; count++)
            {
                subnets = RangeIndex(subnets, is128Checked);

                if (is128Checked == CheckState.Checked)
                {
                    ss = String.Format("{0:x}", subnets.Start);
                    if (ss.Length > 32)
                        ss = ss.Substring(1, 32);
                    ss = Kolonlar(ss, is128Checked);
                    ss = this.CompressAddress(ss);
                    //ss = "s" + subnets.subnetidx + "> " + ss + "/128";
                    ss = "p" + subnets.subnetidx + "> " + ss + "/128";
                }
                else if (is128Checked == CheckState.Unchecked)
                {
                    ss = String.Format("{0:x}", subnets.Start);
                    if (ss.Length > 16)
                        ss = ss.Substring(1, 16);
                    ss = Kolonlar(ss, is128Checked);
                    ss = this.CompressAddress(ss);
                    //ss = "s" + subnets.subnetidx + "> " + ss + "/64";
                    ss = "p" + subnets.subnetidx + "> " + ss + "/64";
                }

                subnets.liste.Add(ss);

                if (subnets.Start.Equals(input.LowerLimitAddress))
                    break;
                subnets.Start -= BigInteger.One;
            }
            subnets.liste.Reverse();

            return subnets;
        }

        public SEaddress ListSubRangePageForward(SEaddress input, CheckState is128Checked)
        {
            SEaddress subnets = new SEaddress();
            subnets = input;
            subnets.liste = new List<string>(input.upto);

            String ss = "";
            int count = 0;

            for (count = 0; count < input.upto; count++)
            {
                subnets = RangeIndex(subnets, is128Checked);

                if (is128Checked == CheckState.Checked)
                {
                    ss = String.Format("{0:x}", subnets.Start);
                    if (ss.Length > 32)
                        ss = ss.Substring(1, 32);
                    ss = Kolonlar(ss, is128Checked);
                    ss = this.CompressAddress(ss);
                    //ss = "s" + subnets.subnetidx + "> " + ss + "/128";
                    ss = "p" + subnets.subnetidx + "> " + ss + "/128";
                }
                else if (is128Checked == CheckState.Unchecked)
                {
                    ss = String.Format("{0:x}", subnets.Start);
                    if (ss.Length > 16)
                        ss = ss.Substring(1, 16);
                    ss = Kolonlar(ss, is128Checked);
                    ss = this.CompressAddress(ss);
                    //ss = "s" + subnets.subnetidx + "> " + ss + "/64";
                    ss = "p" + subnets.subnetidx + "> " + ss + "/64";
                }

                subnets.liste.Add(ss);
                BigInteger NumberOfSubnets = input.UpperLimitAddress - input.LowerLimitAddress + BigInteger.One;
                if (subnets.subnetidx == (NumberOfSubnets - BigInteger.One))
                {
                    break;
                }

                subnets.Start += BigInteger.One;
            }

            return subnets;
        }

        public SEaddress ListSubRangeLastPage(SEaddress input, CheckState is128Checked)
        {
            SEaddress subnets = new SEaddress();
            subnets = input;
            subnets.liste = new List<string>(input.upto);

            String ss = "";
            int count = 0;

            for (count = 0; count < input.upto; count++)
            {
                subnets = RangeIndex(subnets, is128Checked);

                if (is128Checked == CheckState.Checked)
                {
                    ss = String.Format("{0:x}", subnets.Start);
                    if (ss.Length > 32)
                        ss = ss.Substring(1, 32);
                    ss = Kolonlar(ss, is128Checked);
                    ss = this.CompressAddress(ss);
                    //ss = "s" + subnets.subnetidx + "> " + ss + "/128";
                    ss = "p" + subnets.subnetidx + "> " + ss + "/128";
                }
                else if (is128Checked == CheckState.Unchecked)
                {
                    ss = String.Format("{0:x}", subnets.Start);
                    if (ss.Length > 16)
                        ss = ss.Substring(1, 16);
                    ss = Kolonlar(ss, is128Checked);
                    ss = this.CompressAddress(ss);
                    //ss = "s" + subnets.subnetidx + "> " + ss + "/64";
                    ss = "p" + subnets.subnetidx + "> " + ss + "/64";
                }

                subnets.liste.Add(ss);

                if (subnets.Start.Equals(input.LowerLimitAddress))
                    break;
                else
                    subnets.Start -= BigInteger.One;
            }
            subnets.liste.Reverse();

            return subnets;
        }

        public BigInteger IndexRangeBytes(BigInteger FromIndex, BigInteger ToIndex)
        {
            BigInteger from = BigInteger.Zero, to = BigInteger.Zero;
            BigInteger[] fromto = { FromIndex, ToIndex };
            BigInteger[] res = { from, to };

            for (int i = 0; i < 2; i++)
            {
                int digits = fromto[i].ToString().Length - 1;
                BigInteger dd = BigInteger.One;
                for (int j = 0; j < digits; j++)
                {
                    dd += 9 * BigInteger.Pow(10, j) * (j + 1);
                }
                BigInteger k = fromto[i] - BigInteger.Pow(10, digits) + 1;
                BigInteger kalan = k * BigInteger.Pow(10, digits).ToString().Length;
                res[i] = dd + kalan;
            }

            BigInteger fin = res[1] - res[0] + FromIndex.ToString().Length;

            return fin;
        }

        public bool isOdd(BigInteger input) // :-)
        {
            if ((input & BigInteger.One) == BigInteger.One)
                return true;
            else
                return false;
        }

        public AttributeValues AddressType(BigInteger input, int inpfLen, CheckState is128Checked)
        {
            /* Reference RFCs: RFC4291, RFC6890, RFC8190
             */

            AttributeValues attribs = new AttributeValues();
            attribs.SelectedPrefixLength = inpfLen;

            BigInteger multicast_type = BigInteger.Zero;
            BigInteger linklocal_type = BigInteger.Zero;
            List<int> multicast_bits = new List<int>();
            List<int> linklocal_bits = new List<int>();
            BigInteger bitOne = (BigInteger.One << 127);

            multicast_bits = new List<int> { 127, 126, 125, 124, 123, 122, 121, 120 };
            linklocal_bits = new List<int> { 127, 126, 125, 124, 123, 122, 121, 119 };

            foreach (int i in multicast_bits)
                multicast_type = (multicast_type | (BigInteger.One << i));

            foreach (int i in linklocal_bits)
                linklocal_type = (linklocal_type | (BigInteger.One << i));

            bool eq = false;
            if ((input & linklocal_type) == linklocal_type)
                eq = true;

            if (!eq && (input & multicast_type) == multicast_type)
            {
                attribs.isMulticast = true;
                attribs.Name = "Multicast (IANA IPv6-Multicast-Addresses)";

                int i = 0, k = 0;
                bool found = false, inside = false;

                foreach (object o in this.addrRegs.McastAddresses)
                {
                    BigInteger mask = PrepareMask(this.addrRegs.McastAddresses[i].AssignedPrefixLength);
                    BigInteger invmask = BigInteger.Zero;
                    string sinvmask = String.Format("{0:X}", ~mask);
                    sinvmask = sinvmask.TrimStart('F');
                    invmask = BigInteger.Parse(sinvmask, NumberStyles.AllowHexSpecifier);

                    BigInteger start = (this.addrRegs.McastAddresses[i].AddressBlock & mask);
                    BigInteger end = (this.addrRegs.McastAddresses[i].AddressBlock | invmask);

                    if ((input ^ this.addrRegs.McastAddresses[i].AddressBlock) == 0  // exact match AND prefix=128?
                        &&
                        this.addrRegs.McastAddresses[i].AssignedPrefixLength == 128
                        )
                    {
                        found = true;
                        break;
                    }

                    if ((input ^ this.addrRegs.McastAddresses[i].AddressBlock) == 0) // exact match?
                    {
                        if (inpfLen == this.addrRegs.McastAddresses[i].AssignedPrefixLength)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (input >= start && input <= end) // inside the range?
                    {
                        if (inpfLen >= this.addrRegs.McastAddresses[i].AssignedPrefixLength)
                        {
                            inside = true;
                            k = i;
                            i++;
                            continue;
                        }
                    }

                    i++;
                }
                if (found)
                {
                    attribs.strAddressBlock = this.addrRegs.McastAddresses[i].strAddressBlock.ToLower();
                    attribs.Name = this.addrRegs.McastAddresses[i].Name;
                    attribs.AssignedPrefixLength = this.addrRegs.McastAddresses[i].AssignedPrefixLength;
                    attribs.RFC = this.addrRegs.McastAddresses[i].RFC;
                    attribs.AllocationDate = this.addrRegs.McastAddresses[i].AllocationDate;
                    attribs.TerminationDate = this.addrRegs.McastAddresses[i].TerminationDate;
                }
                else if (inside)
                {
                    attribs.strAddressBlock = this.addrRegs.McastAddresses[k].strAddressBlock.ToLower();
                    attribs.Name = this.addrRegs.McastAddresses[k].Name;
                    attribs.AssignedPrefixLength = this.addrRegs.McastAddresses[k].AssignedPrefixLength;
                    attribs.RFC = this.addrRegs.McastAddresses[k].RFC;
                    attribs.AllocationDate = this.addrRegs.McastAddresses[k].AllocationDate;
                    attribs.TerminationDate = this.addrRegs.McastAddresses[k].TerminationDate;
                }
                else
                {
                    attribs.Name = "Multicast (IANA IPv6-Multicast-Addresses)";
                }
            }
            else
            {
                int i = 0, k = 0;
                bool found = false, inside = false;

                foreach (object o in this.addrRegs.Spars)
                {
                    BigInteger mask = PrepareMask(this.addrRegs.Spars[i].AssignedPrefixLength);
                    BigInteger invmask = BigInteger.Zero;
                    string sinvmask = String.Format("{0:X}", ~mask);
                    sinvmask = sinvmask.TrimStart('F');
                    invmask = BigInteger.Parse(sinvmask, NumberStyles.AllowHexSpecifier);

                    BigInteger start = (this.addrRegs.Spars[i].AddressBlock & mask);
                    BigInteger end = (this.addrRegs.Spars[i].AddressBlock | invmask);

                    if ((input ^ this.addrRegs.Spars[i].AddressBlock) == 0) // exact match?
                    {
                        if (inpfLen == this.addrRegs.Spars[i].AssignedPrefixLength)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (input >= start && input <= end) // inside the range?
                    {
                        if (inpfLen >= this.addrRegs.Spars[i].AssignedPrefixLength)
                        {
                            inside = true;
                            k = i;
                            i++;
                            continue;
                        }
                    }

                    i++;
                }

                if (found)
                {
                    attribs.strAddressBlock = this.addrRegs.Spars[i].strAddressBlock.ToLower();
                    attribs.Name = this.addrRegs.Spars[i].Name;
                    attribs.AssignedPrefixLength = this.addrRegs.Spars[i].AssignedPrefixLength;
                    attribs.RFC = this.addrRegs.Spars[i].RFC;
                    attribs.AllocationDate = this.addrRegs.Spars[i].AllocationDate;
                    attribs.TerminationDate = this.addrRegs.Spars[i].TerminationDate;
                    attribs.Source = this.addrRegs.Spars[i].Source;
                    attribs.Destination = this.addrRegs.Spars[i].Destination;
                    attribs.Forwardable = this.addrRegs.Spars[i].Forwardable;
                    attribs.Global = this.addrRegs.Spars[i].Global;
                    attribs.ReservedByProtocol = this.addrRegs.Spars[i].ReservedByProtocol;
                }
                else if (inside)
                {
                    attribs.strAddressBlock = this.addrRegs.Spars[k].strAddressBlock.ToLower();
                    attribs.Name = this.addrRegs.Spars[k].Name;
                    attribs.AssignedPrefixLength = this.addrRegs.Spars[k].AssignedPrefixLength;
                    attribs.RFC = this.addrRegs.Spars[k].RFC;
                    attribs.AllocationDate = this.addrRegs.Spars[k].AllocationDate;
                    attribs.TerminationDate = this.addrRegs.Spars[k].TerminationDate;
                    attribs.Source = this.addrRegs.Spars[k].Source;
                    attribs.Destination = this.addrRegs.Spars[k].Destination;
                    attribs.Forwardable = this.addrRegs.Spars[k].Forwardable;
                    attribs.Global = this.addrRegs.Spars[k].Global;
                    attribs.ReservedByProtocol = this.addrRegs.Spars[k].ReservedByProtocol;
                }
                else
                {
                    if (input == BigInteger.Zero)
                        attribs.Name = "Unspecified Address";
                    else
                        attribs.Name = "Global Unicast";

                    attribs.SelectedPrefixLength = inpfLen;
                    attribs.RFC = "4291";
                }
            }
            return attribs;
        }

        /// <summary>
        /// This function is used to convert given AS number to requested notation,
        /// as-plain or as-dot.
        /// <br> 'null' is returned in case of exception/error.</br>
        /// <br> Maximum value for as-plain and as-dot is 4294967295 and 65535.65535, respectively.</br>
        /// <br><br></br></br>
        /// <b>toASplain</b>: Input will be converted from as-dot to as-plain notation.
        /// Pass it like 'v6ST.toASplain'.
        /// <br></br>
        /// <b>toASdot</b>: Input will be converted from as-plain to as-dot notation.
        /// Pass it like 'v6ST.toASdot'.
        /// <param name="asin"> String AS number input.</param>
        /// <param name="b"> Boolean toASplain (which is true) or toASdot (which is false).</param>
        /// <returns> String as-plain or as-dot or null.</returns>
        /// </summary>

        public static String ConvertASnum(String asin, Boolean b) {
            if (b) {
                char[] chars = asin.Trim().ToCharArray();
                if (chars.Length != 0) {
                    int c = asin.Trim().Split('.').Length - 1;

                if (chars[0] == '.' || chars[chars.Length - 1] == '.' || c != 1) {
                    v6ST.errmsg = "Input Error!";
                    return null;
                } else {
                    try {
                        long result
                                = long.Parse(asin.Trim().Split('.')[0]) * 65536
                                + long.Parse(asin.Trim().Split('.')[1]);
                        if (result > asMax) {
                            v6ST.errmsg = "Exceeded max! 65535.65535";
                            return null;
                        } else {
                            return result.ToString();
                        }
                    } catch (Exception ex) {
                        v6ST.errmsg = ex.ToString();
                        return null;
                    }
                }
            } else {
                v6ST.errmsg = "Error in input string length!";
                return null;
            }
        } else { //v6ST.toASdot / false
            if (asin.Trim().Length > 0) {
                try {
                    long asplain = long.Parse(asin.Trim());
                    if (asplain <= asMax) {
                        return ((asplain >> 16).ToString()
                                + "."
                                + (asplain % 65536).ToString());
                    } else {
                        v6ST.errmsg = "Exceeded max! 4294967295";
                        return null;
                    }
                } catch (Exception ex) {
                    v6ST.errmsg = ex.ToString();
                    return null;
                }
            } else {
                v6ST.errmsg = "Error in input string length!";
                return null;
            }
        }
    }





    } // END of Class
} //END of namespace
