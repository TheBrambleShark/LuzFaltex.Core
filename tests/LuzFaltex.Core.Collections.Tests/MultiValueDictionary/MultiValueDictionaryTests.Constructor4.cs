//
//  MultiValueDictionaryTests.Constructor4.cs
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

using static LuzFaltex.Core.Collections.Tests.MultiValueDictionaryTests.TestData;

#pragma warning disable SA1600 // Elements must be documented

namespace LuzFaltex.Core.Collections.Tests
{
    public partial class MultiValueDictionaryTests
    {
        /// <summary>
        /// Provides tests for the following methods:
        /// <list type="bullet">
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.MultiValueDictionary(int, IEqualityComparer{TKey}?)"/></item>
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.Create{TValueCollection}(int, IEqualityComparer{TKey}?)"/></item>
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.Create{TValueCollection}(int, IEqualityComparer{TKey}?, Func{TValueCollection})"/></item>
        /// </list>
        /// </summary>
        public sealed class Constructor4 : TestSectionBase
        {
            private readonly TypeBuilder _typeBuilder = new();

            [Theory]
            [ClassData(typeof(TypeData))]
            public void DummyComparer_Constructor_ValidCapacity(Type key, Type value)
                => GetGenericMethod<Constructor3>(nameof(MVD_Constructor4_DummyComparer), key, value, ListOfT(value))?.Invoke(this, [true, 0, false]);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void DummyComparer_Constructor_InvalidCapacity(Type key, Type value)
                => GetGenericMethod<Constructor3>(nameof(MVD_Constructor4_DummyComparer), key, value, ListOfT(value))?.Invoke(this, [true, -1, false]);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void NullComparer_Constructor_ValidCapacity(Type key, Type value)
                => GetGenericMethod<Constructor3>(nameof(MVD_Constructor4_NullComparer), key, value, ListOfT(value))?.Invoke(this, [true, 0, false]);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void NullComparer_Constructor_InvalidCapacity(Type key, Type value)
                => GetGenericMethod<Constructor3>(nameof(MVD_Constructor4_NullComparer), key, value, ListOfT(value))?.Invoke(this, [true, -1, false]);

            [Theory]
            [ClassData(typeof(CreateTypeDataWithFactory))]
            public void DummyComparer_Create_ValidCapacity(Type key, Type value, Type valueCollection, bool useFactory)
                => GetGenericMethod<Constructor3>(nameof(MVD_Constructor4_DummyComparer), key, value, valueCollection)?.Invoke(this, [false, 0, useFactory]);

            [Theory]
            [ClassData(typeof(CreateTypeDataWithFactory))]
            public void DummyComparer_Create_InvalidCapacity(Type key, Type value, Type valueCollection, bool useFactory)
                => GetGenericMethod<Constructor3>(nameof(MVD_Constructor4_DummyComparer), key, value, valueCollection)?.Invoke(this, [false, -1, useFactory]);

            [Theory]
            [ClassData(typeof(CreateTypeDataWithFactory))]
            public void NullComparer_Create_ValidCapacity(Type key, Type value, Type valueCollection, bool useFactory)
                => GetGenericMethod<Constructor3>(nameof(MVD_Constructor4_NullComparer), key, value, valueCollection)?.Invoke(this, [false, 0, useFactory]);

            [Theory]
            [ClassData(typeof(CreateTypeDataWithFactory))]
            public void NullComparer_Create_InvalidCapacity(Type key, Type value, Type valueCollection, bool useFactory)
                => GetGenericMethod<Constructor3>(nameof(MVD_Constructor4_NullComparer), key, value, valueCollection)?.Invoke(this, [false, -1, useFactory]);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void InvalidTCollection(Type tkey, Type tvalue)
                => GetGenericMethod<Constructor2>(nameof(MVD_Constructor4_InvalidTCollection), tkey, tvalue)?.Invoke(null, []);

            private void MVD_Constructor4_DummyComparer<TKey, TValue, TValueCollection>(bool useConstructor, int capacity, bool useFactory)
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                MultiValueDictionary<TKey, TValue> mvd;
                DummyComparer<TKey> comparer = new();

                if (useConstructor)
                {
                    mvd = new MultiValueDictionary<TKey, TValue>(capacity, comparer);
                }
                else if (useFactory)
                {
                    mvd = MultiValueDictionary<TKey, TValue>.Create<TValueCollection>(capacity, comparer, () => []);
                }
                else
                {
                    mvd = MultiValueDictionary<TKey, TValue>.Create<TValueCollection>(capacity, comparer);
                }

                if (capacity is >= 0)
                {
                    Assert.Throws<InvalidOperationException>(() => mvd.Add(_typeBuilder.GetNext<TKey>(), _typeBuilder.GetNext<TValue>()));
                }
                else
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() => mvd.Add(_typeBuilder.GetNext<TKey>(), _typeBuilder.GetNext<TValue>()));
                }
            }

            private void MVD_Constructor4_NullComparer<TKey, TValue, TValueCollection>(bool useConstructor, int capacity, bool useFactory)
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                MultiValueDictionary<TKey, TValue> mvd;
                IEqualityComparer<TKey>? comparer = null;

                if (useConstructor)
                {
                    mvd = new MultiValueDictionary<TKey, TValue>(capacity, comparer);
                }
                else if (useFactory)
                {
                    mvd = MultiValueDictionary<TKey, TValue>.Create<TValueCollection>(capacity, comparer, () => []);
                }
                else
                {
                    mvd = MultiValueDictionary<TKey, TValue>.Create<TValueCollection>(capacity, comparer);
                }

                Helpers.TestCollectionCount<TKey, TValue, TValueCollection>(mvd, _typeBuilder);
            }

            private static void MVD_Constructor4_InvalidTCollection<TKey, TValue>()
                where TKey : notnull
            {
                Assert.Throws<InvalidOperationException>(() => MultiValueDictionary<TKey, TValue>.Create<DummyReadOnlyCollection<TValue>>(1, (IEqualityComparer<TKey>?)null));
                Assert.Throws<InvalidOperationException>(() => MultiValueDictionary<TKey, TValue>.Create(1, null, () => new DummyReadOnlyCollection<TValue>()));
            }
        }
    }
}
