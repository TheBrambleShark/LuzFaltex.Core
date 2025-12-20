//
//  SocialSecurityNumber.Equality.cs
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
using System.Numerics;

namespace LuzFaltex.Core.Numerics.Finance
{
    public readonly partial struct SocialSecurityNumber :
        IEqualityOperators<SocialSecurityNumber, SocialSecurityNumber, bool>,
        IEquatable<SocialSecurityNumber>
    {
        /// <inheritdoc />
        public static bool operator ==(SocialSecurityNumber left, SocialSecurityNumber right)
            => left.Equals(right);

        /// <inheritdoc />
        public static bool operator !=(SocialSecurityNumber left, SocialSecurityNumber right)
            => !left.Equals(right);

        /// <inheritdoc/>
        public override readonly bool Equals(object? obj)
            => obj is SocialSecurityNumber number && Equals(number);

        /// <inheritdoc/>
        public readonly bool Equals(SocialSecurityNumber other)
            => _value == other._value;

        /// <inheritdoc/>
        public override readonly int GetHashCode()
            => _value.GetHashCode();
    }
}
