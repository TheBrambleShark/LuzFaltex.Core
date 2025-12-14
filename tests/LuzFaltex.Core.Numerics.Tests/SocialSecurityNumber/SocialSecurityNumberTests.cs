//
//  SocialSecurityNumberTests.cs
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

using LuzFaltex.Core.Numerics.Finance;

#pragma warning disable SA1600, CS1591 // No need to comment.

namespace Luzfaltex.Core.Numerics.Tests
{
    public sealed class SocialSecurityNumberTests
    {
        private class SocialSecurityNumberArrays : TheoryData<int[], SocialSecurityNumber>
        {
            public SocialSecurityNumberArrays()
            {
                Add(new int[9], SocialSecurityNumber.Default);
                Add([0, 7, 8, 0, 5, 1, 1, 2, 0], SocialSecurityNumber.WoolsworthNumber);
                Add([2, 1, 9, 0, 9, 9, 9, 9, 9], SocialSecurityNumber.SampleNumber);
            }
        }

        private class SocialSecurityNumbers : TheoryData<SocialSecurityNumber>
        {
            public SocialSecurityNumbers()
            {
                Add(SocialSecurityNumber.Default);
                Add(SocialSecurityNumber.WoolsworthNumber);
                Add(SocialSecurityNumber.SampleNumber);
            }
        }

        private class SocialsecurityNumberParts : TheoryData<SocialSecurityNumber, ushort, byte, ushort>
        {
            public SocialsecurityNumberParts()
            {
                Add(SocialSecurityNumber.Default, 0, 0, 0);
                Add(SocialSecurityNumber.SampleNumber, 219, 9, 9999);
                Add(SocialSecurityNumber.WoolsworthNumber, 78, 5, 1120);
            }
        }

        private class SocialSecurityNumberStrings : TheoryData<SocialSecurityNumber, string, string>
        {
            public SocialSecurityNumberStrings()
            {
                Add(SocialSecurityNumber.Default, "000-00-0000", "G");
                Add(SocialSecurityNumber.Default, "000000000", "U");
                Add(SocialSecurityNumber.WoolsworthNumber, "078-05-1120", "G");
                Add(SocialSecurityNumber.WoolsworthNumber, "078051120", "U");
                Add(SocialSecurityNumber.SampleNumber, "219-09-9999", "G");
                Add(SocialSecurityNumber.SampleNumber, "219099999", "U");
            }
        }

        [Theory]
        [ClassData(typeof(SocialsecurityNumberParts))]
        public void SSN_BitPacking_OffsetsAreCorrect(SocialSecurityNumber ssn, ushort areaNumber, byte groupNumber, ushort serialNumber)
        {
            Assert.Equal(areaNumber, ssn.AreaNumber);
            Assert.Equal(groupNumber, ssn.GroupNumber);
            Assert.Equal(serialNumber, ssn.SerialNumber);
        }

        [Theory]
        [ClassData(typeof(SocialSecurityNumberArrays))]
        public void SSN_ToArray(int[] expectedArray, SocialSecurityNumber ssn)
        {
            Assert.Equal(expectedArray, ssn.AsArray<int>());
        }

        [Theory]
        [ClassData(typeof(SocialSecurityNumbers))]
        public void SSN_Helper_CanParse(SocialSecurityNumber ssn)
        {
            ParseAndAssert(ssn, ssn.ToString());
            ParseAndAssert(ssn, ssn.ToString("U"));

            static void ParseAndAssert(SocialSecurityNumber ssn, string text)
            {
                bool parseSuccess = SocialSecurityNumber.TryParseInput(text, out int? areaNumber, out int? groupNumber, out int? serialNumber);

                Assert.True(parseSuccess);
                Assert.NotNull(areaNumber);
                Assert.NotNull(groupNumber);
                Assert.NotNull(serialNumber);

                Assert.Equal(ssn.AreaNumber, areaNumber);
                Assert.Equal(ssn.GroupNumber, groupNumber);
                Assert.Equal(ssn.SerialNumber, serialNumber);
            }
        }

        [Theory]
        [InlineData(2096686863)] // 999-99-9999
        [InlineData(0000000000)] // 000-00-0000
        public void SSN_ValidateCondensed_Fails(int value)
        {
            var ssn = new SocialSecurityNumber(value);
            Assert.False(SocialSecurityNumber.Validate(ssn));
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(219, 9, 9999)]
        public void SSN_Parts_Fails(int areaNumber, int groupNumber, int serialNumber)
        {
            Assert.False(SocialSecurityNumber.Validate(areaNumber, groupNumber, serialNumber));
        }

        [Theory]
        [ClassData(typeof(SocialSecurityNumberStrings))]
        public void SSN_Serialize_Successful(SocialSecurityNumber ssn, string expectedText, string format)
        {
            string actualText = ssn.ToString(format);
            Assert.Equal(expectedText, actualText);
        }

        [Theory]
        [ClassData(typeof(SocialSecurityNumberStrings))]
        public void SSN_Deserialize_Successful(SocialSecurityNumber ssn, string value, string format)
        {
            SocialSecurityNumber? parsedValue = default;
            var exception = Record.Exception(() => parsedValue = SocialSecurityNumber.Parse(value));

            Assert.Null(exception);
            Assert.NotNull(parsedValue);

            Assert.Equal(ssn, parsedValue.Value);
        }
    }
}
