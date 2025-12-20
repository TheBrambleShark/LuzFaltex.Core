//
//  MultiValueDictionaryTestBase.cs
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LuzFaltex.Core.Collections.Tests
{
    /// <summary>
    /// Provides a set of utilities for <see cref="MultiValueDictionary{TKey, TValue}"/> tests.
    /// </summary>
    public class MultiValueDictionaryTestBase
    {
        /// <inheritdoc cref="CompareEnumerables{TValue}(IEnumerable{TValue}, IEnumerable{TValue}, bool)"/>
        protected static void CompareEnumerables<TValue>(IEnumerable<TValue> expected, IEnumerable<TValue> actual)
        {
            CompareEnumerables(expected, actual, true);
        }

        /// <summary>
        /// Performs validation to ensure that the two enumerables are equal or not equal depending on <paramref name="areEqual"/>.
        /// </summary>
        /// <typeparam name="TValue">The type the enumerables contain.</typeparam>
        /// <param name="expected">The first collection.</param>
        /// <param name="actual">The second collection.</param>
        /// <param name="areEqual">If <see langword="true"/>, evaluates equality; otherwise, inequality.</param>
        protected static void CompareEnumerables<TValue>(IEnumerable<TValue> expected, IEnumerable<TValue> actual, bool areEqual)
        {
            var multiDictionary1 = new Dictionary<TValue, int>();
            var multiDictionary2 = new Dictionary<TValue, int>();
            int multiDictionary1NullCount = 0;
            int multiDictionary2NullCount = 0;

            foreach (TValue value in expected)
            {
                if (value is null)
                {
                    multiDictionary1NullCount++;
                    continue;
                }

                _ = multiDictionary1.TryGetValue(value, out int count);
                multiDictionary1[value] = ++count;
            }

            foreach (TValue value in actual)
            {
                if (value is null)
                {
                    multiDictionary2NullCount++;
                    continue;
                }

                _ = multiDictionary2.TryGetValue(value, out int count);
                multiDictionary2[value] = ++count;
            }

            if (areEqual)
            {
                Assert.Equal(multiDictionary1.Count, multiDictionary2.Count);
                Assert.Equal(multiDictionary1NullCount, multiDictionary2NullCount);
                foreach (TValue key in multiDictionary1.Keys)
                {
                    Assert.Equal(multiDictionary2[key], multiDictionary1[key]);
                }
            }
            else
            {
                if (multiDictionary1.Count != multiDictionary2.Count && multiDictionary1NullCount != multiDictionary2NullCount)
                {
                    foreach (TValue key in multiDictionary1.Keys)
                    {
                        if (multiDictionary2[key] != multiDictionary1[key])
                        {
                            return;
                        }

                        Assert.True(false);
                    }
                }
            }
        }

        /// <summary>
        /// Validates the two <see cref="MultiValueDictionary{TKey, TValue}"/>s contain all of the same elements.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="mvd1">The first dictionary.</param>
        /// <param name="mvd2">The second dictionary.</param>
        protected static void CompareMVDs<TKey, TValue>(MultiValueDictionary<TKey, TValue> mvd1, MultiValueDictionary<TKey, TValue> mvd2)
        {
            Assert.Equal(mvd1.Count, mvd2.Count);
            Assert.Equal(mvd2.Keys, mvd1.Keys);
            foreach (var key in mvd1.Keys)
            {
                var temp = mvd2[key];
                var countMap1 = new Dictionary<TValue, int>();
                var countMap2 = new Dictionary<TValue, int>();
                foreach (var value in mvd1[key])
                {
                    _ = countMap1.TryGetValue(value, out int count);
                    countMap1[value] = ++count;
                }
                foreach (var value in mvd2[key])
                {
                    _ = countMap2.TryGetValue(value, out int count);
                    countMap2[value] = ++count;
                }

                foreach (var pair in countMap1)
                {
                    Assert.Equal(pair.Value, countMap2[pair.Key]);
                }
            }
        }

        /// <summary>
        /// Dummy comparer. Does nothing.
        /// </summary>
        /// <typeparam name="TKey">The type to compare.</typeparam>
        protected class DummyComparer<TKey> : IEqualityComparer<TKey>
        {
            /// <inheritdoc/>
            public bool Equals(TKey? x, TKey? y)
            {
                throw new InvalidOperationException();
            }

            /// <inheritdoc/>
            public int GetHashCode([DisallowNull] TKey obj)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Dummy collection. Throws for most operations.
        /// </summary>
        /// <typeparam name="TValue">The value the collection stores.</typeparam>
        protected class DummyReadOnlyCollection<TValue> : ICollection<TValue>
        {
            /// <inheritdoc/>
            public int Count => throw new InvalidOperationException();

            /// <inheritdoc/>
            public bool IsReadOnly => true;

            /// <inheritdoc/>
            public void Add(TValue item)
            {
                throw new InvalidOperationException();
            }

            /// <inheritdoc/>
            public void Clear()
            {
                throw new InvalidOperationException();
            }

            /// <inheritdoc/>
            public bool Contains(TValue item)
            {
                throw new InvalidOperationException();
            }

            /// <inheritdoc/>
            public void CopyTo(TValue[] array, int arrayIndex)
            {
                throw new InvalidOperationException();
            }

            /// <inheritdoc/>
            public IEnumerator<TValue> GetEnumerator()
            {
                throw new InvalidOperationException();
            }

            /// <inheritdoc/>
            public bool Remove(TValue item)
            {
                throw new InvalidOperationException();
            }

            /// <inheritdoc/>
            IEnumerator IEnumerable.GetEnumerator()
            {
                throw new InvalidOperationException();
            }
        }
    }
}
