//
//  MultiValueDictionaryTests.GetEnumerator.cs
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

#pragma warning disable SA1600 // Elements must be documented
#pragma warning disable SA1008 // Openening parenthesis should be preceded by a space.

namespace LuzFaltex.Core.Collections.Tests
{
    public partial class MultiValueDictionaryTests
    {
        /// <summary>
        /// Provides tests for the following methods:
        /// <list type="bullet">
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.GetEnumerator"/></item>
        /// </list>
        /// </summary>
        public sealed class GetEnumerator : TestSectionBase
        {
            private readonly TypeBuilder _typeBuilder = new();

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void GetEnumerator_ValidReset(Type key, Type value, Type valueCollection)
                => GetGenericMethod<GetEnumerator>(nameof(EnumeratorValidReset), key, value, valueCollection)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void GetEnumerator_ResetAfterModification(Type key, Type value, Type valueCollection)
            {
                GetGenericMethod<GetEnumerator>(nameof(EnumeratorInvalidOperation), key, value, valueCollection)?.Invoke(this, [(Func<IEnumerator, bool>)Reset]);

                static bool Reset(IEnumerator enumerator)
                {
                    enumerator.Reset();
                    return true; // Return will never be hit. Reset() throws.
                }
            }

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void GetEnumerator_MoveNextAfterModification(Type key, Type value, Type valueCollection)
                => GetGenericMethod<GetEnumerator>(nameof(EnumeratorInvalidOperation), key, value, valueCollection)?.Invoke(this, [(IEnumerator enumerator) => enumerator.MoveNext()]);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void GetEnumerator_Validity(Type key, Type value, Type valueCollection)
                => GetGenericMethod<GetEnumerator>(nameof(CompareEnumeratorValidity), key, value, valueCollection)?.Invoke(this, [false]);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void GetEnumerator_GetBeforeEnumerateThrows(Type key, Type value, Type valueCollection)
                => GetGenericMethod<GetEnumerator>(nameof(GetBeforeFirst), key, value, valueCollection)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void GetEnumerator_GetAfterEnumerateThrows(Type key, Type value, Type valueCollection)
                => GetGenericMethod<GetEnumerator>(nameof(GetAfterLast), key, value, valueCollection)?.Invoke(this, []);

            private void EnumeratorValidReset<TKey, TValue, TValueCollection>()
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    var enumerator = mvd.GetEnumerator();
                    for (int i = 0; i < mvd.Count; i++)
                    {
                        Assert.True(enumerator.MoveNext());
                    }

                    enumerator.Reset();

                    for (int i = 0; i < mvd.Count; i++)
                    {
                        Assert.True(enumerator.MoveNext());
                    }
                }
            }

            private void EnumeratorInvalidOperation<TKey, TValue, TValueCollection>(Func<IEnumerator, bool> action)
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    var enumerator = mvd.GetEnumerator();
                    mvd.Add(_typeBuilder.GetNext<TKey>(), _typeBuilder.GetNext<TValue>());
                    Assert.Throws<InvalidOperationException>(() => action.Invoke(enumerator));
                }
            }

            private void CompareEnumeratorValidity<TKey, TValue, TValueCollection>(bool useIEnumerator)
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    if (useIEnumerator)
                    {
                        IEnumerator enumerator = mvd.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            var current = (KeyValuePair<TKey, IReadOnlyCollection<TValue>>)enumerator.Current;
                            CompareEnumerables(current.Value, mvd[current.Key], true);
                        }
                    }
                    else
                    {
                        foreach (var pair in mvd)
                        {
                            CompareEnumerables(pair.Value, mvd[pair.Key], true);
                        }
                    }
                }
            }

            private void GetBeforeFirst<TKey, TValue, TValueCollection>()
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    IEnumerator enumerator = mvd.GetEnumerator();
                    Assert.Throws<InvalidOperationException>(() => enumerator.Current);
                }
            }

            private void GetAfterLast<TKey, TValue, TValueCollection>()
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                foreach (var mvd in Helpers.Multi<TKey, TValue, TValueCollection>(_typeBuilder))
                {
                    IEnumerator enumerator = mvd.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                    }

                    Assert.Throws<InvalidOperationException>(() => enumerator.Current);
                }
            }
        }
    }
}
