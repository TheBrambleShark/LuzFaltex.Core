//
//  MultiValueDictionaryTests.Values.cs
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
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.Values"/></item>
        /// </list>
        /// </summary>
        public sealed class Values : TestSectionBase
        {
            private readonly TypeBuilder _typeBuilder = new();

            [Theory]
            [ClassData(typeof(TypeData))]
            public void Values_Empty(Type key, Type value)
                => GetGenericMethod<Values>(nameof(EmptyMVD), key, value)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void Values_NonEmpty(Type key, Type value)
                => GetGenericMethod<Values>(nameof(NonEmptyMVD), key, value)?.Invoke(this, []);

            private void EmptyMVD<TKey, TValue>()
                where TKey : notnull
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, List<TValue>>(_typeBuilder))
                {
                    mvd.Clear();
                    Assert.Empty(mvd.Values);
                }
            }

            private void NonEmptyMVD<TKey, TValue>()
                where TKey : notnull
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, List<TValue>>(_typeBuilder))
                {
                    TKey key1 = _typeBuilder.GetNext<TKey>();
                    TKey key2 = _typeBuilder.GetNext<TKey>();
                    TValue value = _typeBuilder.GetNext<TValue>();
                    mvd.Add(key1, value);
                    mvd.Add(key2, value);

                    Assert.NotEmpty(mvd.Values);
                    var valueList = new List<IReadOnlyCollection<TValue>>();
                    foreach (var pair in mvd)
                    {
                        valueList.Add(pair.Value);
                    }

                    CompareEnumerables(mvd.Values, valueList, true);
                }
            }
        }
    }
}
