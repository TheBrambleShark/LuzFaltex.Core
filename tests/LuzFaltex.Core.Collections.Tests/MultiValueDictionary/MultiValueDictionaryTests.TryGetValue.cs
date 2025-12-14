//
//  MultiValueDictionaryTests.TryGetValue.cs
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
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.TryGetValue(TKey, out IReadOnlyCollection{TValue}?)"/></item>
        /// </list>
        /// </summary>
        public sealed class TryGetValue : TestSectionBase
        {
            private readonly TypeBuilder _typeBuilder = new();

            [Theory]
            [ClassData(typeof(NullKeyTests))]
            public void TryGetValue_NullKey(Type key, Type value, Type valueCollection)
                => GetGenericMethod<Remove>(nameof(TryGetNullKey), key, value, valueCollection)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void TryGetValue_TryGetEmptyKey(Type key, Type value, Type valueCollection)
                => GetGenericMethod<Remove>(nameof(TryGetKeyValue), key, value, valueCollection)?.Invoke(this, [true]);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void TryGetValue_TryGetPresentKey(Type key, Type value, Type valueCollection)
                => GetGenericMethod<Remove>(nameof(TryGetKeyValue), key, value, valueCollection)?.Invoke(this, [false]);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void TryGetValue_CollectionAsView(Type key, Type value)
                => GetGenericMethod<Remove>(nameof(CollectionAsView), key, value)?.Invoke(this, []);

            private void TryGetNullKey<TKey, TValue, TValueCollection>()
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    TKey? key = default;
                    Assert.Throws<ArgumentNullException>(() => mvd.TryGetValue(key!, out _));
                }
            }

            private void TryGetKeyValue<TKey, TValue, TValueCollection>(bool emptyKey)
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                TKey key = _typeBuilder.GetNext<TKey>();
                TValue value = _typeBuilder.GetNext<TValue>();

                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    mvd.Remove(key);

                    if (emptyKey)
                    {
                        Assert.False(mvd.TryGetValue(key, out _));
                    }
                    else
                    {
                        mvd.Add(key, value);
                        Assert.True(mvd.TryGetValue(key, out IReadOnlyCollection<TValue>? retCol));
                        Assert.NotNull(retCol);
                        Assert.Contains(value, retCol);
                        CompareEnumerables(retCol, mvd[key], true);
                    }
                }
            }

            private void CollectionAsView<TKey, TValue>()
                where TKey : notnull
            {
                TKey key = _typeBuilder.GetNext<TKey>();
                foreach (var mvd in Helpers.Multi<TKey, TValue, List<TValue>>(_typeBuilder))
                {
                    mvd.Remove(key);
                    mvd.Add(key, _typeBuilder.GetNext<TValue>());

                    Assert.True(mvd.TryGetValue(key, out IReadOnlyCollection<TValue>? retCol));
                    Assert.NotNull(retCol);
                    mvd.Add(key, _typeBuilder.GetNext<TValue>());
                    mvd.Add(key, _typeBuilder.GetNext<TValue>());
                    mvd.Add(key, _typeBuilder.GetNext<TValue>());
                    mvd.Add(key, _typeBuilder.GetNext<TValue>());

                    CompareEnumerables(retCol, mvd[key], true);
                }
            }
        }
    }
}
