//
//  MultiValueDictionaryTests.ContainsKey.cs
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
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.ContainsKey(TKey)"/></item>
        /// </list>
        /// </summary>
        public sealed class ContainsKey : TestSectionBase
        {
            private readonly TypeBuilder _typeBuilder = new();

            [Theory]
            [ClassData(typeof(NullKeyTests))]
            public void ContainsKey_NullKey(Type key, Type value, Type valueCollection)
                => GetGenericMethod<ContainsKey>(nameof(ContainsNullKey), key, value, valueCollection)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void ContainsKey_ValidKey(Type key, Type value)
                => GetGenericMethod<ContainsKey>(nameof(ContainsValidKey), key, value)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void ContainsKey_NoLongerContainsKey(Type key, Type value)
                => GetGenericMethod<ContainsKey>(nameof(NoLongerContainsKey), key, value)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void ContainsKey_DoesNotContainNewKey(Type key, Type value)
                => GetGenericMethod<ContainsKey>(nameof(DoesNotContainNewKey), key, value)?.Invoke(this, []);

            private void ContainsNullKey<TKey, TValue, TValueCollection>()
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    TKey? key = default;
                    Assert.Throws<ArgumentNullException>(() => mvd.ContainsKey(key!));
                }
            }

            private void ContainsValidKey<TKey, TValue>()
                where TKey : notnull
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, List<TValue>>(_typeBuilder))
                {
                    TKey newKey = _typeBuilder.GetNext<TKey>();
                    TValue newValue = _typeBuilder.GetNext<TValue>();
                    mvd.Add(newKey, newValue);

                    Assert.True(mvd.ContainsKey(newKey));
                }
            }

            private void NoLongerContainsKey<TKey, TValue>()
                where TKey : notnull
            {
                TKey key = _typeBuilder.GetNext<TKey>();
                TValue value = _typeBuilder.GetNext<TValue>();

                foreach (var mvd in Helpers.Multi<TKey, TValue, List<TValue>>(_typeBuilder))
                {
                    mvd.Add(key, value);
                    Assert.True(mvd.ContainsKey(key));

                    mvd.Remove(key);
                    Assert.False(mvd.ContainsKey(key));

                    mvd.Add(key, value);
                    mvd.Remove(key, value);
                    Assert.False(mvd.ContainsKey(key));
                }
            }

            private void DoesNotContainNewKey<TKey, TValue>()
                where TKey : notnull
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, List<TValue>>(_typeBuilder))
                {
                    TKey key = _typeBuilder.GetNext<TKey>();
                    mvd.Remove(key);
                    Assert.False(mvd.ContainsKey(key));
                }
            }
        }
    }
}
