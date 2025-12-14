//
//  MultiValueDictionaryTests.Keys.cs
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
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.Keys"/></item>
        /// </list>
        /// </summary>
        public sealed class Keys : TestSectionBase
        {
            private readonly TypeBuilder _typeBuilder = new();

            [Theory]
            [ClassData(typeof(TypeData))]
            public void Keys_EmptyKeys(Type key, Type value)
                => GetGenericMethod<Keys>(nameof(EmptyKeys), key, value)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void Keys_NonEmptyKeys(Type key, Type value)
                => GetGenericMethod<Keys>(nameof(NonEmptyKeys), key, value)?.Invoke(this, []);

            private void EmptyKeys<TKey, TValue>()
                where TKey : notnull
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, List<TValue>>(_typeBuilder))
                {
                    mvd.Clear();
                    Assert.Empty(mvd.Keys);
                }
            }

            private void NonEmptyKeys<TKey, TValue>()
                where TKey : notnull
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, List<TValue>>(_typeBuilder))
                {
                    TKey key = _typeBuilder.GetNext<TKey>();
                    TValue value = _typeBuilder.GetNext<TValue>();
                    mvd.Add(key, value);

                    Assert.NotEmpty(mvd.Keys);

                    var keyList = new List<TKey>();
                    foreach (var pair in mvd)
                    {
                        keyList.Add(pair.Key);
                    }

                    CompareEnumerables(mvd.Keys, keyList, true);
                }
            }
        }
    }
}
