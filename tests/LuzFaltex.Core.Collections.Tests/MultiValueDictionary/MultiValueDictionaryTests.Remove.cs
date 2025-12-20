//
//  MultiValueDictionaryTests.Remove.cs
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
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.Remove(TKey)"/></item>
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.Remove(TKey, TValue)"/></item>
        /// </list>
        /// </summary>
        public sealed class Remove : TestSectionBase
        {
            private readonly TypeBuilder _typeBuilder = new();

            [Theory]
            [ClassData(typeof(NullKeyTests))]
            public void Remove_NullKey(Type key, Type value, Type valueCollection)
                => GetGenericMethod<Remove>(nameof(RemoveNullKey), key, value, valueCollection)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(NullKeyTests))]
            public void Remove_NullKeyValue(Type key, Type value, Type valueCollection)
                => GetGenericMethod<Remove>(nameof(RemoveNullKeyValue), key, value, valueCollection)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void Remove_NonexistentKey(Type key, Type value, Type valueCollection)
                => GetGenericMethod<Remove>(nameof(RemoveNonexistentKey), key, value, valueCollection)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void Remove_NonexistentKeyValue(Type key, Type value, Type valueCollection)
                => GetGenericMethod<Remove>(nameof(RemoveNonexistentKeyValue), key, value, valueCollection)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void Remove_ExisingKey(Type key, Type value, Type valueCollection)
                => GetGenericMethod<Remove>(nameof(RemoveKey), key, value, valueCollection)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void Remove_ValidKeyExistingValue(Type key, Type value, Type valueCollection)
                => GetGenericMethod<Remove>(nameof(RemoveKeyValue), key, value, valueCollection)?.Invoke(this, [true]);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void Remove_ValidKeyNonexistingValue(Type key, Type value, Type valueCollection)
                => GetGenericMethod<Remove>(nameof(RemoveKeyValue), key, value, valueCollection)?.Invoke(this, [false]);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void Remove_RemovesSingleValueInstance(Type key, Type value)
                => GetGenericMethod<Remove>(nameof(RemovesSingleValueInstance), key, value)?.Invoke(this, []);

            private void RemoveNullKey<TKey, TValue, TValueCollection>()
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    TKey? key = default;

                    int oldCount = mvd.Count;
                    Assert.Throws<ArgumentNullException>(() => mvd.Remove(key!));
                    Assert.Equal(oldCount, mvd.Count);
                }
            }

            private void RemoveNullKeyValue<TKey, TValue, TValueCollection>()
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    TKey? key = default;

                    int oldCount = mvd.Count;
                    Assert.Throws<ArgumentNullException>(() => mvd.Remove(key!, _typeBuilder.GetNext<TValue>()));
                    Assert.Equal(oldCount, mvd.Count);
                }
            }

            private void RemoveNonexistentKey<TKey, TValue, TValueCollection>()
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    TKey key = _typeBuilder.GetNext<TKey>();
                    mvd.Remove(key);
                    int oldCount = mvd.Count;

                    Assert.False(mvd.Remove(key));

                    Assert.Throws<KeyNotFoundException>(() => mvd[key]);
                    Assert.Equal(oldCount, mvd.Count);
                }
            }

            private void RemoveNonexistentKeyValue<TKey, TValue, TValueCollection>()
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    TKey key = _typeBuilder.GetNext<TKey>();
                    mvd.Remove(key);
                    int oldCount = mvd.Count;

                    Assert.False(mvd.Remove(key, _typeBuilder.GetNext<TValue>()));

                    Assert.Throws<KeyNotFoundException>(() => mvd[key]);
                    Assert.Equal(oldCount, mvd.Count);
                }
            }

            private void RemoveKey<TKey, TValue, TValueCollection>()
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    TKey key = _typeBuilder.GetNext<TKey>();
                    mvd.Add(key, _typeBuilder.GetNext<TValue>());
                    int keyCount = mvd[key].Count;
                    int oldCount = Helpers.PairsCount(mvd);

                    Assert.True(mvd.Remove(key));

                    Assert.Throws<KeyNotFoundException>(() => mvd[key]);
                    Assert.Equal(oldCount - keyCount, Helpers.PairsCount(mvd));
                }
            }

            private void RemoveKeyValue<TKey, TValue, TValueCollection>(bool validValue)
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    TKey key = _typeBuilder.GetNext<TKey>();
                    mvd.Remove(key);

                    TValue value1 = _typeBuilder.GetNext<TValue>();
                    TValue value2 = _typeBuilder.GetNext(value1);
                    mvd.Add(key, value1);

                    bool removed = validValue switch
                    {
                        true => RemoveValidValue(mvd, key, value2),
                        false => RemoveInvalidValue(mvd, key, value2),
                    };

                    Assert.Equal(validValue, removed);
                }

                static bool RemoveValidValue(MultiValueDictionary<TKey, TValue> mvd, TKey key, TValue value)
                {
                    mvd.Add(key, value);

                    int oldCount = Helpers.PairsCount(mvd);
                    int keyCount = mvd[key].Count;

                    Assert.True(mvd.Remove(key, value));

                    Assert.Equal(oldCount - 1, Helpers.PairsCount(mvd));
                    Assert.Equal(keyCount - 1, mvd[key].Count);
                    Assert.True(mvd[key].Count > 0);

                    return true;
                }

                static bool RemoveInvalidValue(MultiValueDictionary<TKey, TValue> mvd, TKey key, TValue value)
                {
                    while (mvd.Contains(key, value))
                    {
                        Assert.True(mvd.Remove(key, value));
                    }

                    int oldCount = Helpers.PairsCount(mvd);
                    int keyCount = mvd[key].Count;

                    Assert.False(mvd.Remove(key, value));
                    Assert.Equal(oldCount, Helpers.PairsCount(mvd));
                    Assert.Equal(keyCount, mvd[key].Count);

                    return false;
                }
            }

            private void RemovesSingleValueInstance<TKey, TValue>()
                where TKey : notnull
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, List<TValue>>(_typeBuilder))
                {
                    TKey key = _typeBuilder.GetNext<TKey>();
                    TValue value1 = _typeBuilder.GetNext<TValue>();
                    mvd.Add(key, value1);
                    mvd.Add(key, value1);

                    int oldCount = Helpers.PairsCount(mvd);
                    int keyCount = mvd[key].Count;

                    Assert.True(mvd.Remove(key, value1));

                    Assert.Equal(oldCount - 1, Helpers.PairsCount(mvd));
                    Assert.Equal(keyCount - 1, mvd[key].Count);
                    Assert.True(mvd[key].Count > 0);
                }
            }
        }
    }
}
