//
//  MultiValueDictionaryTests.Constructor1.cs
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
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.MultiValueDictionary()"/></item>
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.Create{TValueCollection}()"/></item>
        /// <item><see cref="MultiValueDictionary{TKey, TValue}.Create{TValueCollection}(Func{TValueCollection})"/></item>
        /// </list>
        /// </summary>
        public sealed class Constructor1 : TestSectionBase
        {
            private readonly TypeBuilder _typeBuilder = new();

            [Theory]
            [ClassData(typeof(TypeData))]
            public void ValidKeyValue_Constructor(Type key, Type value)
                => GetGenericMethod<Constructor1>(nameof(MVD_Constructor1_Valid_Constructor), key, value)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void ValidKeyValue_Create(Type key, Type value, Type valueCollection)
                => GetGenericMethod<Constructor1>(nameof(MVD_Constructor1_Valid_Create), key, value, valueCollection)?.Invoke(this, []);

            [Theory]
            [ClassData(typeof(CreateTypeData))]
            public void ValidKeyValue_Create_Factory(Type key, Type value, Type valueCollection)
            {
                if (valueCollection == ListOfT(value))
                {
                    GetGenericMethod<Constructor1>(nameof(MVD_Constructor1_Valid_Create_Factory_List), key, value)?.Invoke(this, []);
                }
                else if (valueCollection == HashSetOfT(value))
                {
                    GetGenericMethod<Constructor1>(nameof(MVD_Constructor1_Valid_Create_Factory_HashSet), key, value)?.Invoke(this, []);
                }
                else
                {
                    throw new ArgumentException("Invalid type", nameof(valueCollection));
                }
            }

            [Theory]
            [ClassData(typeof(TypeData))]
            public void InvalidTCollection(Type key, Type value)
                => GetGenericMethod<Constructor1>(nameof(MVD_Constructor1_InvalidTCollection), key, value)?.Invoke(this, []);

            private void MVD_Constructor1_Valid_Constructor<TKey, TValue>()
                where TKey : notnull
            {
                MultiValueDictionary<TKey, TValue> mvd = [];
                Helpers.TestCollectionCount<TKey, TValue, List<TValue>>(mvd, _typeBuilder);
            }

            private void MVD_Constructor1_Valid_Create<TKey, TValue, TValueCollection>()
                where TKey : notnull
                where TValueCollection : ICollection<TValue>, new()
            {
                MultiValueDictionary<TKey, TValue> mvd = MultiValueDictionary<TKey, TValue>.Create<TValueCollection>();
                Helpers.TestCollectionCount<TKey, TValue, TValueCollection>(mvd, _typeBuilder);
            }

            private void MVD_Constructor1_Valid_Create_Factory_List<TKey, TValue>()
                where TKey : notnull
            {
                MultiValueDictionary<TKey, TValue> mvd = MultiValueDictionary<TKey, TValue>.Create<List<TValue>>(() => []);
                Helpers.TestCollectionCount<TKey, TValue, List<TValue>>(mvd, _typeBuilder);
            }

            private void MVD_Constructor1_Valid_Create_Factory_HashSet<TKey, TValue>()
                where TKey : notnull
            {
                MultiValueDictionary<TKey, TValue> mvd = MultiValueDictionary<TKey, TValue>.Create<HashSet<TValue>>(() => []);
                Helpers.TestCollectionCount<TKey, TValue, HashSet<TValue>>(mvd, _typeBuilder);
            }

            private static void MVD_Constructor1_InvalidTCollection<TKey, TValue>()
                where TKey : notnull
            {
                Assert.Throws<InvalidOperationException>(MultiValueDictionary<TKey, TValue>.Create<DummyReadOnlyCollection<TValue>>);
                Assert.Throws<InvalidOperationException>(() => MultiValueDictionary<TKey, TValue>.Create(() => new DummyReadOnlyCollection<TValue>()));
            }
        }
    }
}
