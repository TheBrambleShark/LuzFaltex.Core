//
//  MultiValueDictionaryTests.Contains.cs
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
using System.Collections.Generic;

#pragma warning disable SA1600 // Elements must be documented

namespace LuzFaltex.Core.Collections.Tests
{
    public partial class MultiValueDictionaryTests
    {
        /// <summary>
        /// Provides tests for the following methods:
        /// <list type="bullet">
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.Contains(TKey, TValue)"/></item>
        /// </list>
        /// </summary>
        public sealed class Contains : TestSectionBase
        {
            private readonly TypeBuilder _typeBuilder = new();

            [Theory]
            [ClassData(typeof(NullKeyTests))]
            public void Contains_NullKey(Type key, Type value, Type valueCollection)
                => GetGenericMethod<Contains>(nameof(ContainsNullKey), key, value, valueCollection)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void Contains_ValidKeyInvalidValue(Type key, Type value, Type valueCollection)
                => GetGenericMethod<Contains>(nameof(ContainsValidKeyInvalidValue), key, value, valueCollection)?.Invoke(this, []);

            private void ContainsNullKey<TKey, TValue, TValueCollection>()
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    TKey? key = default;
                    Assert.Throws<ArgumentNullException>(() => mvd.Contains(key!, _typeBuilder.GetNext<TValue>()));
                }
            }

            private void ContainsValidKeyInvalidValue<TKey, TValue, TValueCollection>()
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    TKey key = _typeBuilder.GetNext<TKey>();
                    TValue newValue1 = _typeBuilder.GetNext<TValue>();
                    TValue newValue2 = _typeBuilder.GetNext(newValue1);

                    mvd.Add(key, newValue1);

                    while (mvd.Contains(key, newValue2))
                    {
                        mvd.Remove(key, newValue2);
                    }

                    Assert.True(mvd.Contains(key, newValue1));
                    Assert.False(mvd.Contains(key, newValue2));
                }
            }
        }
    }
}
