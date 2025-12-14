//
//  SocialSecurityNumber.Validate.cs
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

namespace LuzFaltex.Core.Numerics.Finance
{
    public readonly partial struct SocialSecurityNumber
    {
        /// <summary>
        /// Validates the provided Social Security Number string.
        /// </summary>
        /// <param name="ssn">The social security number to test.</param>
        /// <returns><see langword="true"/> if the provided value was valid; otherwise, <see langword="false"/>.</returns>
        public static bool Validate(string ssn)
        {
            if (!TryParseInput(ssn, out int? areaNumber, out int? groupNumber, out int? serialNumber))
            {
                return false;
            }

            return Validate(areaNumber.Value, groupNumber.Value, serialNumber.Value);
        }

        /// <summary>
        /// Validates the provided <see cref="SocialSecurityNumber"/>.
        /// </summary>
        /// <param name="ssn">The <see cref="SocialSecurityNumber"/> to test.</param>
        /// <returns><see langword="true"/> if the provided value was valid; otherwise, <see langword="false"/>.</returns>
        public static bool Validate(SocialSecurityNumber ssn)
            => Validate(ssn.AreaNumber, ssn.GroupNumber, ssn.SerialNumber);

        /// <summary>
        /// Validates the provided Social Security Number components.
        /// </summary>
        /// <param name="areaNumber">The area number.</param>
        /// <param name="groupNumber">The group number.</param>
        /// <param name="serialNumber">The serial number.</param>
        /// <returns><see langword="true"/> if the provided values were valid; otherwise, <see langword="false"/>.</returns>
        public static bool Validate(int areaNumber, int groupNumber, int serialNumber)
        {
            if (areaNumber is 0 or 666 or >= 999)
            {
                return false;
            }

            if (groupNumber is 0 or >= 99)
            {
                return false;
            }

            if (serialNumber is 0 or >= 9999)
            {
                return false;
            }

            return true;
        }
    }
}
