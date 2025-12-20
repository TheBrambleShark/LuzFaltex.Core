//
//  MultiValueDictionaryTests.Constructor2.cs
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
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.MultiValueDictionary(int)"/></item>
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.Create{TValueCollection}(int)"/></item>
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.Create{TValueCollection}(int, Func{TValueCollection})"/></item>
        /// </list>
        /// </summary>
        public sealed class Constructor2 : TestSectionBase
        {
            private readonly TypeBuilder _typeBuilder = new();

            [Theory]
            [ClassData(typeof(TypeData))]
            public void PositiveCapacity_Constructor(Type key, Type value)
                => GetGenericMethod<Constructor2>(nameof(MVD_Constructor2_PositiveCapacity), key, value, ListOfT(value))?.Invoke(this, [true, false]);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void ZeroCapacity_Constructor(Type key, Type value)
                => GetGenericMethod<Constructor2>(nameof(MVD_Constructor2_ZeroCapacity), key, value, ListOfT(value))?.Invoke(this, [true, false]);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void NegativeCapacity_Constructor(Type key, Type value)
                => GetGenericMethod<Constructor2>(nameof(MVD_Constructor2_NegativeCapacity), key, value, ListOfT(value))?.Invoke(null, [true, false]);

            [Theory]
            [ClassData(typeof(CreateTypeDataWithFactory))]
            public void PositiveCapacity(Type key, Type value, Type valueCollection, bool useFactory)
                => GetGenericMethod<Constructor2>(nameof(MVD_Constructor2_PositiveCapacity), key, value, valueCollection)?.Invoke(this, [false, useFactory]);

            [Theory]
            [ClassData(typeof(CreateTypeDataWithFactory))]
            public void ZeroCapacity(Type key, Type value, Type valueCollection, bool useFactory)
                => GetGenericMethod<Constructor2>(nameof(MVD_Constructor2_ZeroCapacity), key, value, valueCollection)?.Invoke(this, [false, useFactory]);

            [Theory]
            [ClassData(typeof(CreateTypeDataWithFactory))]
            public void NegativeCapacity(Type key, Type value, Type valueCollection, bool useFactory)
                => GetGenericMethod<Constructor2>(nameof(MVD_Constructor2_NegativeCapacity), key, value, valueCollection)?.Invoke(null, [false, useFactory]);

            [Theory]
            [ClassData(typeof(TypeData))]
            public void InvalidTCollection(Type key, Type value)
                => GetGenericMethod<Constructor2>(nameof(MVD_Constructor2_InvalidTCollection), key, value)?.Invoke(null, []);

            private void MVD_Constructor2_PositiveCapacity<TKey, TValue, TValueCollection>(bool useConstructor, bool useFactory)
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                MultiValueDictionary<TKey, TValue> mvd;

                if (useConstructor)
                {
                    mvd = new MultiValueDictionary<TKey, TValue>(1);
                }
                else if (useFactory)
                {
                    mvd = MultiValueDictionary<TKey, TValue>.Create<TValueCollection>(1, () => []);
                }
                else
                {
                    mvd = MultiValueDictionary<TKey, TValue>.Create<TValueCollection>(1);
                }

                Helpers.TestCollectionCount<TKey, TValue, TValueCollection>(mvd, _typeBuilder);
            }

            private void MVD_Constructor2_ZeroCapacity<TKey, TValue, TValueCollection>(bool useConstructor, bool useFactory)
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                MultiValueDictionary<TKey, TValue> mvd;

                if (useConstructor)
                {
                    mvd = [];
                }
                else if (useFactory)
                {
                    mvd = MultiValueDictionary<TKey, TValue>.Create<TValueCollection>(0, () => []);
                }
                else
                {
                    mvd = MultiValueDictionary<TKey, TValue>.Create<TValueCollection>(0);
                }

                Helpers.TestCollectionCount<TKey, TValue, TValueCollection>(mvd, _typeBuilder);
            }

            private static void MVD_Constructor2_NegativeCapacity<TKey, TValue, TValueCollection>(bool useConstructor, bool useFactory)
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                if (useConstructor)
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() => new MultiValueDictionary<TKey, TValue>(-1));
                }
                else if (useFactory)
                {
                    Assert.Empty(MultiValueDictionary<TKey, TValue>.Create<TValueCollection>(-1, () => []));
                }
                else
                {
                    Assert.Empty(MultiValueDictionary<TKey, TValue>.Create<TValueCollection>(-1));
                }
            }

            private static void MVD_Constructor2_InvalidTCollection<TKey, TValue>()
                where TKey : notnull
            {
                Assert.Throws<InvalidOperationException>(() => MultiValueDictionary<TKey, TValue>.Create<DummyReadOnlyCollection<TValue>>(1));
                Assert.Throws<InvalidOperationException>(() => MultiValueDictionary<TKey, TValue>.Create(1, () => new DummyReadOnlyCollection<TValue>()));
            }
        }
    }
}
