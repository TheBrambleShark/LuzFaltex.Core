//
//  MultiValueDictionaryTests.Item.cs
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
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.this[TKey]"/></item>
        /// </list>
        /// </summary>
        public sealed class Item : TestSectionBase
        {
            private readonly TypeBuilder _typeBuilder = new();

            [Theory]
            [ClassData(typeof(NullKeyTests))]
            public void Item_NullKey(Type key, Type value, Type valueCollection)
                => GetGenericMethod<Item>(nameof(NullKey), key, value, valueCollection)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void Item_ExistingKey(Type key, Type value)
                => GetGenericMethod<Item>(nameof(ExistingKey), key, value)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void Item_NonexistentKey(Type key, Type value)
                => GetGenericMethod<Item>(nameof(NonexistentKey), key, value)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void Item_ActiveView(Type key, Type value)
                => GetGenericMethod<Item>(nameof(ActiveView), key, value)?.Invoke(this, []);

            private void NullKey<TKey, TValue, TValueCollection>()
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    TKey? key = default;
                    Assert.Throws<ArgumentNullException>(() => mvd[key!]);
                }
            }

            private void ExistingKey<TKey, TValue>()
                where TKey : notnull
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, List<TValue>>(_typeBuilder))
                {
                    TKey key = _typeBuilder.GetNext<TKey>();
                    TValue value = _typeBuilder.GetNext<TValue>();
                    mvd.Remove(key);
                    mvd.Add(key, value);
                    CompareEnumerables(mvd[key], [value], true);
                }
            }

            private void NonexistentKey<TKey, TValue>()
                where TKey : notnull
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, List<TValue>>(_typeBuilder))
                {
                    TKey key = _typeBuilder.GetNext<TKey>();
                    mvd.Remove(key);
                    Assert.Throws<KeyNotFoundException>(() => mvd[key]);
                }
            }

            private void ActiveView<TKey, TValue>()
                where TKey : notnull
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, List<TValue>>(_typeBuilder))
                {
                    TKey key = _typeBuilder.GetNext<TKey>();
                    TValue value = _typeBuilder.GetNext<TValue>();
                    mvd.Remove(key);
                    mvd.Add(key, value);

                    var retCol = mvd[key];

                    mvd.Add(key, _typeBuilder.GetNext<TValue>());
                    mvd.Add(key, _typeBuilder.GetNext<TValue>());
                    mvd.Add(key, _typeBuilder.GetNext<TValue>());
                    mvd.Add(key, _typeBuilder.GetNext<TValue>());
                    mvd.Add(key, _typeBuilder.GetNext<TValue>());

                    CompareEnumerables(mvd[key], retCol, true);
                    Assert.Equal(retCol, mvd[key]);
                }
            }
        }
    }
}
