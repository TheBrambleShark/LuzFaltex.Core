//
//  SocialSecurityNumber.Parse.cs
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LuzFaltex.Core.Numerics.Finance
{
    public readonly partial struct SocialSecurityNumber : IParsable<SocialSecurityNumber>
    {
        private const string RegexText = """^\s*(\d{3})[ -]?(\d{2})[ -]?(\d{4})\s*$""";

        [GeneratedRegex(RegexText, RegexOptions.Compiled)]
        private static partial Regex SocialsecurityNumberRegex();

        /// <inheritdoc cref="Parse(string, IFormatProvider?)"/>
        public static SocialSecurityNumber Parse(string ssn)
            => Parse(ssn, CultureInfo.CurrentCulture);

        /// <inheritdoc cref="TryParse(string?, IFormatProvider?, out SocialSecurityNumber)"/>
        /// <returns>The parsed Social Security Number.</returns>
        /// <exception cref="ArgumentException">Thrown when a parse error occurs.</exception>
        public static SocialSecurityNumber Parse(string ssn, IFormatProvider? provider)
        {
            if (!TryParse(ssn, provider, out SocialSecurityNumber result))
            {
                throw new ArgumentException("Provided value is not in a valid format.", nameof(ssn));
            }

            return result;
        }

        /// <inheritdoc cref="TryParse(string?, IFormatProvider?, out SocialSecurityNumber)"/>
        public static bool TryParse([NotNullWhen(true)] string? ssn, [MaybeNullWhen(false)] out SocialSecurityNumber result)
            => TryParse(ssn, CultureInfo.CurrentCulture, out result);

        /// <summary>
        /// Attempts to parse the provided string into an instance of <see cref="SocialSecurityNumber"/>.
        /// </summary>
        /// <param name="ssn">The Social Security Number to parse.</param>
        /// <param name="provider">Provides format information about <paramref name="ssn"/>.</param>
        /// <param name="result">When <see langword="true"/> is returned, this is the parsed <see cref="SocialSecurityNumber"/>.</param>
        /// <returns><see langword="true" /> if the value was successfully parsed; otherwise, <see langword="false"/>.</returns>
        public static bool TryParse([NotNullWhen(true)] string? ssn, IFormatProvider? provider, [MaybeNullWhen(false)] out SocialSecurityNumber result)
        {
            result = default;

            if (!TryParseInput(ssn, out int? area, out int? group, out int? serial))
            {
                return false;
            }

            if (Validate(area.Value, group.Value, serial.Value))
            {
                result = new SocialSecurityNumber(area.Value, group.Value, serial.Value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Attempts to deserialize a SSN string into its constituent parts.
        /// </summary>
        /// <param name="input">The string.</param>
        /// <param name="areaNumber">The area number.</param>
        /// <param name="groupNumber">The group number.</param>
        /// <param name="serialNumber">The serial number.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        internal static bool TryParseInput
        (
            string? input,
            [NotNullWhen(true)] out int? areaNumber,
            [NotNullWhen(true)] out int? groupNumber,
            [NotNullWhen(true)] out int? serialNumber
        )
        {
            areaNumber = groupNumber = serialNumber = null;
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            Match match = SocialsecurityNumberRegex().Match(input);

            if (match.Success)
            {
                areaNumber = int.Parse(match.Groups[1].Value);
                groupNumber = int.Parse(match.Groups[2].Value);
                serialNumber = int.Parse(match.Groups[3].Value);
                return true;
            }

            return false;
        }
    }
}
