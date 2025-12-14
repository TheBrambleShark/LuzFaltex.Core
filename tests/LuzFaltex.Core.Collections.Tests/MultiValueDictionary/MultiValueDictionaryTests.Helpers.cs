//
//  MultiValueDictionaryTests.Helpers.cs
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

using System.Collections.Generic;

#pragma warning disable SA1600 // Elements must be documented

namespace LuzFaltex.Core.Collections.Tests
{
    public partial class MultiValueDictionaryTests
    {
        /// <summary>
        /// Provides a set of helper methods.
        /// </summary>
        public static class Helpers
        {
            public static MultiValueDictionary<TKey, TValue>[] Multi<TKey, TValue, TValueCollection>(TypeBuilder typeBuilder)
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                MultiValueDictionary<TKey, TValue>[] ret = new MultiValueDictionary<TKey, TValue>[6];
                var factory = MultiValueDictionary<TKey, TValue>.Factory<TValueCollection>();

                // Empty MultiValueDictionary
                ret[0] = MultiValueDictionary<TKey, TValue>.Create<TValueCollection>();

                // 1-element MultiValueDictionary
                factory.Values = GetKeyValuePairs<TKey, TValue>(typeBuilder, 1);
                ret[1] = factory.Build();

                // 2-element MultiValueDictionary
                factory.Values = GetKeyValuePairs<TKey, TValue>(typeBuilder, 2);
                ret[2] = factory.Build();

                // Lightly filled MultiValueDictionary
                factory.Values = GetKeyValuePairs<TKey, TValue>(typeBuilder, 20);
                ret[3] = factory.Build();

                // Moderately filled MultiValueDictionry
                factory.Values = GetKeyValuePairs<TKey, TValue>(typeBuilder, 200);
                ret[4] = factory.Build();

                // Heavily filled MultiValueDictionary
                factory.Values = GetKeyValuePairs<TKey, TValue>(typeBuilder, 5000);
                ret[5] = factory.Build();

                return ret;
            }

            public static void TestCollectionCount<TKey, TValue, TCollection>(MultiValueDictionary<TKey, TValue> mvd, TypeBuilder typeBuilder)
                where TKey : notnull
                where TCollection : ICollection<TValue>, new()
            {
                TCollection collection = [];
                var key = typeBuilder.GetNext<TKey>();
                mvd.Remove(key);
                int iterations = 50;
                for (int i = 0; i < iterations; i++)
                {
                    var value = typeBuilder.GetNext<TValue>();
                    mvd.Add(key, value);
                    collection.Add(value);
                }
                CompareEnumerables(collection, mvd[key]);
            }

            public static void TestCollectionType<TKey, TValue>(MultiValueDictionary<TKey, TValue> mvd, TypeBuilder typeBuilder)
                where TKey : notnull
            {
                ICollection<TValue> newCollection = [];
                var key = typeBuilder.GetNext<TKey>();
                mvd.Remove(key);
                int iterations = 50;
                for (int i = 0; i < iterations; i++)
                {
                    var value = typeBuilder.GetNext<TValue>();
                    mvd.Add(key, value);
                    newCollection.Add(value);
                }
                CompareEnumerables(mvd[key], newCollection, true);
            }

            public static int PairsCount<TKey, TValue>(MultiValueDictionary<TKey, TValue> mvd)
                where TKey : notnull
            {
                int count = 0;
                foreach (var pair in mvd)
                {
                    count += pair.Value.Count;
                }
                return count;
            }

            private static IEnumerable<KeyValuePair<TKey, IReadOnlyCollection<TValue>>> GetKeyValuePairs<TKey, TValue>(TypeBuilder typeBuilder, int keyCount, int valueCount = 1)
            {
                Assert.InRange(keyCount, 1, int.MaxValue);
                Assert.InRange(keyCount, 1, int.MaxValue);

                if (keyCount == valueCount && keyCount == 1)
                {
                    yield return new(typeBuilder.GetNext<TKey>(), [typeBuilder.GetNext<TValue>()]);
                }
                else if (valueCount == 1)
                {
                    for (int i = 0; i < keyCount; i++)
                    {
                        yield return new(typeBuilder.GetNext<TKey>(), [typeBuilder.GetNext<TValue>()]);
                    }

                    yield break;
                }
                else
                {
                    for (int i = 0; i < keyCount; i++)
                    {
                        yield return new(typeBuilder.GetNext<TKey>(), [.. GetValues(valueCount, typeBuilder)]);
                    }
                }

                static IEnumerable<TValue> GetValues(int valueCount, TypeBuilder typeBuilder)
                {
                    for (int i = 0; i < valueCount; i++)
                    {
                        yield return typeBuilder.GetNext<TValue>();
                    }
                }
            }
        }
    }
}
