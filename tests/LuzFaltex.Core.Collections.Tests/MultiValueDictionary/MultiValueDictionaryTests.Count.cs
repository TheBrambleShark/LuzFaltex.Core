//
//  MultiValueDictionaryTests.Count.cs
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
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.Count"/></item>
        /// </list>
        /// </summary>
        public sealed class Count : TestSectionBase
        {
            private readonly TypeBuilder _typeBuilder = new();

            [Theory]
            [ClassData(typeof(TypeData))]
            public void Count_EmptyCollection(Type key, Type value)
                => GetGenericMethod<Count>(nameof(CountEmptyCollection), key, value)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void Count_NonEmptyCollection(Type key, Type value)
                => GetGenericMethod<Count>(nameof(CountNonEmptyCollection), key, value)?.Invoke(this, []);

            private void CountEmptyCollection<TKey, TValue>()
                where TKey : notnull
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, List<TValue>>(_typeBuilder))
                {
                    mvd.Clear();
                    Assert.Empty(mvd);
                    Assert.Empty(mvd.Keys);
                    Assert.Empty(mvd.Values);
                    Assert.Equal(0, Helpers.PairsCount(mvd));
                }
            }

            private void CountNonEmptyCollection<TKey, TValue>()
                where TKey : notnull
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, List<TValue>>(_typeBuilder))
                {
                    TKey key1 = _typeBuilder.GetNext<TKey>();
                    TKey key2 = _typeBuilder.GetNext(key1);
                    TKey key3 = _typeBuilder.GetNext(key1, key2);

                    mvd.Clear();

                    mvd.Add(key1, _typeBuilder.GetNext<TValue>());
                    mvd.Add(key2, _typeBuilder.GetNext<TValue>());
                    mvd.Add(key3, _typeBuilder.GetNext<TValue>());

                    Assert.Equal(3, Helpers.PairsCount(mvd));
                }
            }
        }
    }
}
