//
//  MultiValueDictionaryTests.Add.cs
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
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.Add(TKey, TValue)"/></item>
        /// </list>
        /// </summary>
        public sealed class Add : TestSectionBase
        {
            private readonly TypeBuilder _typeBuilder = new();

            [Theory]
            [ClassData(typeof(NullKeyTests))]
            public void Add_NullKey(Type key, Type value, Type valueCollection)
                => GetGenericMethod<Add>(nameof(AddNullKey), key, value, valueCollection)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void Add_ValidKeyNullValue(Type key, Type value, Type valueCollection)
                => GetGenericMethod<Add>(nameof(AddKeyValue), key, value, valueCollection)?.Invoke(this, [true]);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void Add_ValidKeyValidValue(Type key, Type value, Type valueCollection)
                => GetGenericMethod<Add>(nameof(AddKeyValue), key, value, valueCollection)?.Invoke(this, [false]);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void Add_AlreadyPresentPair(Type key, Type value)
                => GetGenericMethod<Add>(nameof(AddDuplicate), key, value)?.Invoke(this, []);

            private void AddNullKey<TKey, TValue, TValueCollection>()
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                TKey? key = default;
                TValue value = _typeBuilder.GetNext<TValue>();
                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    Assert.Throws<ArgumentNullException>(() => mvd.Add(key!, value));
                }
            }

            private void AddKeyValue<TKey, TValue, TValueCollection>(bool nullValue)
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                TKey key = _typeBuilder.GetNext<TKey>();
                TValue? value = nullValue
                    ? default
                    : _typeBuilder.GetNext<TValue>();
                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    mvd.Remove(key, value!);
                    int oldCount = Helpers.PairsCount(mvd);
                    mvd.Add(key, value!);
                    Assert.Equal(oldCount + 1, Helpers.PairsCount(mvd));
                }
            }

            private void AddDuplicate<TKey, TValue>()
                where TKey : notnull
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, List<TValue>>(_typeBuilder))
                {
                    TKey newKey = _typeBuilder.GetNext<TKey>();
                    TValue newValue = _typeBuilder.GetNext<TValue>();

                    mvd.Remove(newKey, newValue);
                    int oldCount = Helpers.PairsCount(mvd);
                    mvd.Add(newKey, newValue);
                    mvd.Add(newKey, newValue);
                    mvd.Add(newKey, newValue);
                    Assert.Equal(oldCount + 3, Helpers.PairsCount(mvd));
                }

                foreach (var mvd in Helpers.Multi<TKey, TValue, HashSet<TValue>>(_typeBuilder))
                {
                    TKey newKey = _typeBuilder.GetNext<TKey>();
                    TValue newValue = _typeBuilder.GetNext<TValue>();

                    mvd.Remove(newKey, newValue);
                    int oldCount = Helpers.PairsCount(mvd);
                    mvd.Add(newKey, newValue);
                    mvd.Add(newKey, newValue);
                    mvd.Add(newKey, newValue);
                    Assert.Equal(oldCount + 1, Helpers.PairsCount(mvd));
                }
            }
        }
    }
}
