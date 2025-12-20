//
//  FileSize.cs
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
using JetBrains.Annotations;
using Remora.Results;
using Vogen;

namespace LuzFaltex.Core.Numerics
{
    /// <summary>
    /// A utility for managing file sizes.
    /// </summary>
    [PublicAPI]
    [ValueObject<Int128>(parsableForPrimitives: ParsableForPrimitives.GenerateNothing)]
    [Instance("Zero", 0L, "Represents a zero size file.")]
    public readonly partial struct FileSize
        : IParsable<FileSize>
    {
        private const long KilobyteBase = 1_000L;
        private const long MegabyteBase = 1_000_000L;
        private const long GigabyteBase = 1_000_000_000L;
        private const long TerabyteBase = 1_000_000_000_000L;

        private const long KibibyteBase = 1_024L;
        private const long MebibyteBase = 1_048_576L;
        private const long GibibyteBase = 1_073_741_824L;
        private const long TebibyteBase = 1_099_511_627_776L;

        /// <inheritdoc />
        public static FileSize Parse(string s, IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public static bool TryParse(string? s, IFormatProvider? provider, out FileSize result)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Attempts to parse the provided text into a <see cref="FileSize"/>.
        /// </summary>
        /// <inheritdoc cref="IParsable{TSelf}.Parse(string, IFormatProvider)"/>
        public static Result<FileSize> TryParseResult(string? s, IFormatProvider? provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Accepts the specified number of bytes to create a file size.
        /// </summary>
        /// <param name="bytes">The number of bytes.</param>
        /// <returns>A new instance of <see cref="FileSize"/>.</returns>
        public static FileSize FromBytes(long bytes) => From(bytes);

        /// <summary>
        /// Accepts the specified number of kilobytes to create a file size.
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes.</param>
        /// <returns>A new instance of <see cref="FileSize"/>.</returns>
        public static FileSize FromKilobytes(int kilobytes) => From((ulong)kilobytes * KilobyteBase);

        /// <summary>
        /// Accepts the specified number of <paramref name="megabytes"/> to create a <see cref="FileSize"/>.
        /// </summary>
        /// <param name="megabytes">The number of megabytes.</param>
        /// <returns>A new instance of <see cref="FileSize"/>.</returns>
        public static FileSize FromMegabytes(int megabytes) => From((ulong)megabytes * MegabyteBase);

        /// <summary>
        /// Accepts the specified number of <paramref name="gigabytes"/> to create a <see cref="FileSize"/>.
        /// </summary>
        /// <param name="gigabytes">The number of gigabytes.</param>
        /// <returns>A new instance of <see cref="FileSize"/>.</returns>
        public static FileSize FromGigabytes(int gigabytes) => From((ulong)gigabytes * GigabyteBase);

        /// <summary>
        /// Accepts the specified number of <paramref name="terabytes"/> to create a <see cref="FileSize"/>.
        /// </summary>
        /// <param name="terabytes">The number of terabytes.</param>
        /// <returns>A new instance of <see cref="FileSize"/>.</returns>
        public static FileSize FromTerabytes(int terabytes) => From((ulong)terabytes * TerabyteBase);

        /// <summary>
        /// Accepts the specified number of <paramref name="kibibytes"/> to create a <see cref="FileSize"/>.
        /// </summary>
        /// <param name="kibibytes">The number of kibibytes.</param>
        /// <returns>A new instance of <see cref="FileSize"/>.</returns>
        public static FileSize FromKibibytes(int kibibytes) => From((ulong)kibibytes * KibibyteBase);

        /// <summary>
        /// Accepts the specified number of <paramref name="mebibytes"/> to create a <see cref="FileSize"/>.
        /// </summary>
        /// <param name="mebibytes">The number of mebibytes.</param>
        /// <returns>A new instance of <see cref="FileSize"/>.</returns>
        public static FileSize FromMebibytes(int mebibytes) => From((ulong)mebibytes * MebibyteBase);

        /// <summary>
        /// Accepts the specified number of <paramref name="gibibytes"/> to create a <see cref="FileSize"/>.
        /// </summary>
        /// <param name="gibibytes">The number of gibibytes.</param>
        /// <returns>A new instance of <see cref="FileSize"/>.</returns>
        public static FileSize FromGibibytes(int gibibytes) => From((ulong)gibibytes * GibibyteBase);

        /// <summary>
        /// Accepts the specified number of <paramref name="tebibytes"/> to create a <see cref="FileSize"/>.
        /// </summary>
        /// <param name="tebibytes">The number of tebibytes.</param>
        /// <returns>A new instance of <see cref="FileSize"/>.</returns>
        public static FileSize FromTebibytes(int tebibytes) => From((ulong)tebibytes * TebibyteBase);

        private static Int128 NormalizeInput(Int128 input)
        {
            // todo: normalize (sanitize) your input;
            return input;
        }

        private static Validation Validate(Int128 input)
        {
            bool isValid = true; // todo: your validation
            return isValid ? Validation.Ok : Validation.Invalid("[todo: describe the validation]");
        }
    }
}
