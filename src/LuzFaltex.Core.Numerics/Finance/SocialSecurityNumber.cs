//
//  SocialSecurityNumber.cs
//
//  Author:
//       LuzFaltex Contributors <support@luzfaltex.com>
//
//  Copyright (c) LuzFaltex, LLC.
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Vogen;

namespace LuzFaltex.Core.Numerics.Finance
{
    /// <summary>
    /// Represents a 9-digit United States social security number.
    /// </summary>
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    public readonly partial struct SocialSecurityNumber :
        IFormattable
    {
        private const int GroupNumberOffsetMask = 0x7F;
        private const int SerialNumberOffsetMask = 0x3FFF;

        /// <summary>
        /// Gets the raw, condensed value of this SSN.
        /// </summary>
        internal readonly int _value;

        /// <summary>
        /// Gets the first three digits of the SSN. Formerly used to denote geographic location.
        /// </summary>
        public readonly ushort AreaNumber => (ushort)(_value >>> 21);

        /// <summary>
        /// Gets the digits in position 4 and 5 of the SSN.
        /// </summary>
        public readonly byte GroupNumber => (byte)((_value >>> 14) & GroupNumberOffsetMask);

        /// <summary>
        /// Gets the last four digits of the SSN, a number which is incremented for each issued SSN.
        /// </summary>
        public readonly ushort SerialNumber => (ushort)(_value & SerialNumberOffsetMask);

        /// <summary>
        /// Gets the default <see cref="SocialSecurityNumber"/> of 000-00-0000.
        /// </summary>
        /// <remarks>
        /// This social security number is invalid.
        /// </remarks>
        public static SocialSecurityNumber Default => default;

        /// <summary>
        /// Gets a <see cref="SocialSecurityNumber"/> used as a sample for advertisements and documentation by the Socal Security Administration.
        /// </summary>
        /// <remarks>
        /// 219-09-9999.
        /// </remarks>
        public static SocialSecurityNumber SampleNumber => new(219, 09, 9999);

        /// <summary>
        /// Gets a decommissioned <see cref="SocialSecurityNumber"/> once issued to an employee of Woolsworth Wallets.
        /// </summary>
        /// <remarks>078-05-1120.</remarks>
        /// <seealso href="https://www.ssa.gov/history/ssn/misused.html"/>
        public static SocialSecurityNumber WoolsworthNumber => new(078, 05, 1120);

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialSecurityNumber"/> struct.
        /// </summary>
        /// <param name="areaNumber">The area number.</param>
        /// <param name="groupNumber">The group number.</param>
        /// <param name="serialNumber">The serial number.</param>
        /// <remarks>This function performs no validation except for basic range checks.</remarks>
        internal SocialSecurityNumber(int areaNumber, int groupNumber, int serialNumber)
            : this((areaNumber << 21) | (groupNumber << 14) | serialNumber)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(areaNumber, 999);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(groupNumber, 99);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(serialNumber, 9999);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SocialSecurityNumber"/> struct.
        /// </summary>
        /// <param name="value">The condensed value.</param>
        internal SocialSecurityNumber(int value)
        {
            _value = value;
        }

        /// <summary>
        /// Gets this <see cref="SocialSecurityNumber"/> as an array.
        /// </summary>
        /// <typeparam name="TNumber">The numeric type to convert into.</typeparam>
        /// <returns>An array of values for this SSN.</returns>
        public readonly TNumber[] AsArray<TNumber>()
            where TNumber : struct, INumber<TNumber>
        {
            var number = new TNumber[9];
            Subdivide(SerialNumber, ref number, count: 8);
            Subdivide(GroupNumber, ref number, count: 4);
            Subdivide(AreaNumber, ref number, count: 2);

            return number;

            static void Subdivide(ushort value, ref TNumber[] number, int count)
            {
                if (value == 0)
                {
                    return;
                }

                for (ushort x = value; x > 0; x /= 10)
                {
                    number[count--] = TNumber.CreateChecked(x % 10);
                }

                return;
            }
        }

        /// <inheritdoc/>
        public override readonly string ToString()
            => ToString("G", CultureInfo.CurrentCulture);

        /// <inheritdoc cref="ToString(string?, IFormatProvider?)"/>
        public readonly string ToString(string format)
            => ToString(format, CultureInfo.CurrentCulture);

        /// <summary>
        /// Returns a string representation of this <see cref="SocialSecurityNumber"/> in the requested format.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Format Code</term>
        ///         <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///         <term>G</term>
        ///         <description>General. Returns a hyphenated format. <c>219-09-9999</c>.</description>
        ///     </item>
        ///     <item>
        ///         <term>H</term>
        ///         <description>Hyphenated. Same as <strong>G</strong>.</description>
        ///     </item>
        ///     <item>
        ///         <term>U</term>
        ///         <description>Unhyphenated. <c>219099999</c>.</description>
        ///     </item>
        ///     <item>
        ///         <term>M</term>
        ///         <description>Masked and hyphenated. Shows only the last four digits. <c>***-**-9999</c>.</description>
        ///     </item>
        ///     <item>
        ///         <term>N</term>
        ///         <description>Masked and (Not) hyphenated. Shows only the last four digits. <c>*****9999</c>.</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="format">The format to use to represent the Social Security Number.</param>
        /// <param name="formatProvider">The format provider to use for this operation.</param>
        /// <returns>A string representation of this <see cref="SocialSecurityNumber"/>.</returns>
        public readonly string ToString(string? format, IFormatProvider? formatProvider)
        {
            format ??= "G";

            return format?.ToUpperInvariant() switch
            {
                "G" or "H" => $"{AreaNumber:D3}-{GroupNumber:D2}-{SerialNumber:D4}",
                "U" => $"{AreaNumber:D3}{GroupNumber:D2}{SerialNumber:D4}",
                "M" => $"***-**-{SerialNumber:D4}",
                "N" => $"*****{SerialNumber:D4}",
                _ => throw new ArgumentOutOfRangeException(format, $"The selected format '{format}' is not supported.")
            };
        }

        private readonly string GetDebuggerDisplay() => ToString();
    }
}
