//
//  MultiValueDictionaryTests.AddRange.cs
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
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.AddRange(TKey, IEnumerable{TValue})"/></item>
        /// </list>
        /// </summary>
        public sealed class AddRange : TestSectionBase
        {
            private readonly TypeBuilder _typeBuilder = new();

            private enum ValueType
            {
                Null,
                Empty,
                NonNull
            }

            [Theory]
            [ClassData(typeof(NullKeyTests))]
            public void AddRange_NullKey(Type key, Type value, Type valueCollection)
                => GetGenericMethod<AddRange>(nameof(AddNullKey), key, value, valueCollection)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void AddRange_ValidKeyNullValues(Type key, Type value, Type valueCollection)
                => GetGenericMethod<AddRange>(nameof(AddKeyValues), key, value, valueCollection)?.Invoke(this, [ValueType.Null]);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void AddRange_ValidKeyEmptyValues(Type key, Type value, Type valueCollection)
                => GetGenericMethod<AddRange>(nameof(AddKeyValues), key, value, valueCollection)?.Invoke(this, [ValueType.Empty]);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void AddRange_ValidKeyNonEmptyValues(Type key, Type value, Type valueCollection)
                => GetGenericMethod<AddRange>(nameof(AddKeyValues), key, value, valueCollection)?.Invoke(this, [ValueType.NonNull]);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void AddRange_ValidKeyDuplicateValues(Type key, Type value)
                => GetGenericMethod<AddRange>(nameof(AddKeyDuplicateValues), key, value)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void AddRange_AlreadyPresentKey(Type key, Type value, Type valueCollection)
                => GetGenericMethod<AddRange>(nameof(AddExistingKey), key, value, valueCollection)?.Invoke(this, []);

            private void AddNullKey<TKey, TValue, TValueCollection>()
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                TKey? key = default;
                ICollection<TValue> values = _typeBuilder.CreateRange<TValue, TValueCollection>(_typeBuilder.GetNext<int>());

                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    int oldCount = mvd.Count;
                    Assert.Throws<ArgumentNullException>(() => mvd.AddRange(key!, values));
                    Assert.Equal(oldCount, mvd.Count);
                }
            }

            private void AddKeyValues<TKey, TValue, TValueCollection>(ValueType valueType)
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                TKey key = _typeBuilder.GetNext<TKey>();
                TValueCollection? values = valueType switch
                {
                    ValueType.Null => default,
                    ValueType.Empty => [],
                    ValueType.NonNull => _typeBuilder.CreateRange<TValue, TValueCollection>(_typeBuilder.GetNext<int>()),
                    _ => throw new ArgumentOutOfRangeException(nameof(valueType)),
                };
                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    mvd.Remove(key);
                    int oldCount = Helpers.PairsCount(mvd);

                    if (valueType is ValueType.Null)
                    {
                        Assert.Null(values);
                        Assert.Throws<ArgumentNullException>(() => mvd.AddRange(key, values!));
                        Assert.Equal(oldCount, Helpers.PairsCount(mvd));
                    }
                    else if (valueType is ValueType.Empty)
                    {
                        Assert.NotNull(values);
                        mvd.AddRange(key, values);
                        Assert.Equal(oldCount, Helpers.PairsCount(mvd));
                    }
                    else
                    {
                        Assert.NotNull(values);
                        mvd.AddRange(key, values);
                        Assert.Equal(oldCount + values.Count, Helpers.PairsCount(mvd));
                    }
                }
            }

            private void AddKeyDuplicateValues<TKey, TValue>()
                where TKey : notnull
            {
                TKey key = _typeBuilder.GetNext<TKey>();
                TValue value = _typeBuilder.GetNext<TValue>();

                foreach (var mvd in Helpers.Multi<TKey, TValue, List<TValue>>(_typeBuilder))
                {
                    List<TValue> values = [value, value, value, value, value];
                    int oldCount = Helpers.PairsCount(mvd);
                    mvd.AddRange(key, values);
                    Assert.Equal(oldCount + values.Count, Helpers.PairsCount(mvd));
                }
                foreach (var mvd in Helpers.Multi<TKey, TValue, HashSet<TValue>>(_typeBuilder))
                {
                    mvd.Remove(key);

                    HashSet<TValue> values = [value, value, value, value, value];
                    int oldCount = Helpers.PairsCount(mvd);
                    mvd.AddRange(key, values);
                    Assert.Equal(oldCount + 1, Helpers.PairsCount(mvd));
                }
            }

            private void AddExistingKey<TKey, TValue, TValueCollection>()
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    TKey key = _typeBuilder.GetNext<TKey>();
                    mvd.Remove(key);
                    TValueCollection values = _typeBuilder.CreateRange<TValue, TValueCollection>(_typeBuilder.GetNext<int>());
                    TValue preValue;
                    do
                    {
                        preValue = _typeBuilder.GetNext<TValue>();
                    }
                    while (values.Contains(preValue));

                    mvd.Add(key, preValue);
                    int oldCount = Helpers.PairsCount(mvd);

                    mvd.AddRange(key, values);

                    Assert.Equal(oldCount + values.Count, Helpers.PairsCount(mvd));
                    CompareEnumerables(mvd[key], [.. values, preValue], true);
                }
            }
        }
    }
}
