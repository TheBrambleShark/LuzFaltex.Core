//
//  MultiValueDictionaryTests.ContainsValue.cs
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
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.ContainsValue(TValue)"/></item>
        /// </list>
        /// </summary>
        public sealed class ContainsValue : TestSectionBase
        {
            private readonly TypeBuilder _typeBuilder = new();

            [Theory]
            [ClassData(typeof(TypeData))]
            public void ContainsValue_NullValue(Type key, Type value)
                => GetGenericMethod<ContainsValue>(nameof(ContainsNullValue), key, value)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void ContainsValue_DoesNotContainValue(Type key, Type value, Type valueCollection)
                => GetGenericMethod<ContainsValue>(nameof(ContainsNonExistentValue), key, value, valueCollection)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void ContainsValue_ValidValue(Type key, Type value)
                => GetGenericMethod<ContainsValue>(nameof(ContainsValidValue), key, value)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void ContainsValue_DuplicateValues(Type key, Type value)
                => GetGenericMethod<ContainsValue>(nameof(ContainsValueMultipleTimes), key, value)?.Invoke(this, []);

            private void ContainsNullValue<TKey, TValue>()
                where TKey : notnull
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, List<TValue>>(_typeBuilder))
                {
                    TKey key = _typeBuilder.GetNext<TKey>();
                    TValue? value = default;
                    mvd.Add(key, value!);
                    Assert.True(mvd.Contains(key, value!));
                }
            }

            private void ContainsNonExistentValue<TKey, TValue, TValueCollection>()
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                var mvd = MultiValueDictionary<TKey, TValue>.Create<TValueCollection>();

                TKey key = _typeBuilder.GetNext<TKey>();
                TValue value1 = _typeBuilder.GetNext<TValue>();
                TValue value2 = _typeBuilder.GetNext(value1);

                mvd.Add(key, value1);
                Assert.True(mvd.ContainsKey(key));
                Assert.True(mvd.Contains(key, value1));

                Assert.False(mvd.Contains(key, value2), "Assert.False() Failure; Expected: Contains() returned True; Actual: Contains() returned False");
                Assert.False(mvd.ContainsValue(value2), "Assert.False() Failure; Expected: ContainsValue() returned True; Actual: ContainsValue() returned False");
            }

            private void ContainsValidValue<TKey, TValue>()
                where TKey : notnull
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, List<TValue>>(_typeBuilder))
                {
                    TKey newKey = _typeBuilder.GetNext<TKey>();
                    TValue newValue = _typeBuilder.GetNext<TValue>();
                    mvd.Add(newKey, newValue);
                    Assert.True(mvd.ContainsValue(newValue));
                }
            }

            private void ContainsValueMultipleTimes<TKey, TValue>()
                where TKey : notnull
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, List<TValue>>(_typeBuilder))
                {
                    TKey key1 = _typeBuilder.GetNext<TKey>();
                    TKey key2 = _typeBuilder.GetNext(key1);

                    TValue newValue = _typeBuilder.GetNext<TValue>();
                    mvd.Add(key1, newValue);
                    mvd.Add(key1, newValue);
                    mvd.Add(key2, newValue);
                    Assert.True(mvd.ContainsValue(newValue));
                }
            }
        }
    }
}
