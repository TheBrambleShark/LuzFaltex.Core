//
//  MultiValueDictionaryTests.cs
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
    public sealed partial class MultiValueDictionaryTests : MultiValueDictionaryTestBase
    {
        /// <summary>
        /// Contains a collection of types used for tests.
        /// </summary>
        public static class TestData
        {
            /// <summary>
            /// Gets the name of the Create method.
            /// </summary>
            // See https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/unbound-generic-types-in-nameof
            public const string Create = nameof(MultiValueDictionary<,>.Create);

            /// <summary>
            /// Gets the testable types.
            /// </summary>
            public static readonly Type[] Types = [typeof(char), typeof(int), typeof(string)];

            /// <summary>
            /// Gets a type representing a <see cref="MultiValueDictionary{TKey, TValue}"/>.
            /// </summary>
            /// <param name="key">The type to use as the key.</param>
            /// <param name="value">The type to use as a value.</param>
            /// <returns>A type representing a <see cref="MultiValueDictionary{TKey, TValue}"/>.</returns>
            public static Type MultiValueDictionaryOf(Type key, Type value) => typeof(MultiValueDictionary<,>).MakeGenericType(key, value);

            /// <summary>
            /// Gets a type representing a list with the underlying type <paramref name="t"/>.
            /// </summary>
            /// <param name="t">The underlying type of the list.</param>
            /// <returns>A type representing a list of <paramref name="t"/>.</returns>
            public static Type ListOfT(Type t) => typeof(List<>).MakeGenericType(t);

            /// <summary>
            /// Gets a type representing a HashSet with the underlying type <paramref name="t"/>.
            /// </summary>
            /// <param name="t">The underlying type of the list.</param>
            /// <returns>A type representing a HashSet of <paramref name="t"/>.</returns>
            public static Type HashSetOfT(Type t) => typeof(HashSet<>).MakeGenericType(t);

            /// <summary>
            /// Gets a type representing a <c>Func&lt;List&lt;T&gt;&gt;</c>.
            /// </summary>
            /// <param name="t">The underlying type of the list.</param>
            /// <returns>A type representing a list factory.</returns>
            public static Type ListOfTFactory(Type t) => typeof(Func<>).MakeGenericType(ListOfT(t));

            /// <summary>
            /// Gets a type representing a <c>Func&lt;HashSet&lt;T&gt;&gt;</c>.
            /// </summary>
            /// <param name="t">The underlying type of the hashset.</param>
            /// <returns>A type representing a hashset factory.</returns>
            public static Type HashSetOfTFactory(Type t) => typeof(Func<>).MakeGenericType(HashSetOfT(t));
        }

        /// <summary>
        /// Gets type data for parameterless methods.
        /// </summary>
        public class TypeData : TheoryData<Type, Type>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TypeData"/> class.
            /// </summary>
            public TypeData()
            {
                Add(typeof(char), typeof(char));
                Add(typeof(char), typeof(int));
                Add(typeof(char), typeof(string));

                Add(typeof(int), typeof(char));
                Add(typeof(int), typeof(int));
                Add(typeof(int), typeof(string));

                Add(typeof(string), typeof(char));
                Add(typeof(string), typeof(int));
                Add(typeof(string), typeof(string));
            }
        }

        /// <summary>
        /// Gets test data for <see cref="Constructor1"/> tests.
        /// </summary>
        public class TypeDataWithFactory : TheoryData<Type, Type, bool>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TypeDataWithFactory"/> class.
            /// </summary>
            public TypeDataWithFactory()
            {
                foreach (var key in TestData.Types)
                {
                    foreach (var value in TestData.Types)
                    {
                        Add(key, value, true);
                        Add(key, value, false);
                    }
                }
            }
        }

        /// <summary>
        /// Gets test data for <see cref="Constructor1"/> tests.
        /// </summary>
        public class CreateTypeData : TheoryData<Type, Type, Type>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CreateTypeData"/> class.
            /// </summary>
            public CreateTypeData()
            {
                foreach (var key in TestData.Types)
                {
                    foreach (var value in TestData.Types)
                    {
                        Add(key, value, TestData.ListOfT(value));
                        Add(key, value, TestData.HashSetOfT(value));
                    }
                }
            }
        }

        /// <summary>
        /// Gets test data for <see cref="Constructor2"/> tests.
        /// </summary>
        public class CreateTypeDataWithFactory : TheoryData<Type, Type, Type, bool>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="CreateTypeDataWithFactory"/> class.
            /// </summary>
            public CreateTypeDataWithFactory()
            {
                foreach (var key in TestData.Types)
                {
                    foreach (var value in TestData.Types)
                    {
                        // Create(int)
                        Add(key, value, TestData.ListOfT(value), false);
                        Add(key, value, TestData.HashSetOfT(value), false);

                        // Create(int, Func<TValueCollection>)
                        Add(key, value, TestData.ListOfT(value), true);
                        Add(key, value, TestData.HashSetOfT(value), true);
                    }
                }
            }
        }

        /// <summary>
        /// Provides a set of test data with nullable keys.
        /// </summary>
        public class NullKeyTests : TheoryData<Type, Type, Type>
        {
            public sealed class Unit : IEquatable<Unit>, IComparable<Unit>
            {
                /// <summary>
                /// Gets the one and only instance of <see cref="Unit"/>.
                /// </summary>
                public static Unit Default => new();

                public override bool Equals(object? obj) => obj is Unit;

                public bool Equals(Unit? other) => other is not null;

                public int CompareTo(Unit? other)
                    => other is null ? 1 : 0;

                public override int GetHashCode() => 0;

                public static bool operator ==(Unit left, Unit right)
                {
                    return left.Equals(right);
                }

                public static bool operator !=(Unit left, Unit right)
                {
                    return !(left == right);
                }

                public static bool operator <(Unit left, Unit right)
                {
                    return left.CompareTo(right) < 0;
                }

                public static bool operator <=(Unit left, Unit right)
                {
                    return left.CompareTo(right) <= 0;
                }

                public static bool operator >(Unit left, Unit right)
                {
                    return left.CompareTo(right) > 0;
                }

                public static bool operator >=(Unit left, Unit right)
                {
                    return left.CompareTo(right) >= 0;
                }
            }

            public NullKeyTests()
            {
                foreach (var value in TestData.Types)
                {
                    // string
                    Add(typeof(string), value, TestData.ListOfT(value));
                    Add(typeof(string), value, TestData.HashSetOfT(value));

                    // object
                    Add(typeof(Unit), value, TestData.ListOfT(value));
                    Add(typeof(Unit), value, TestData.HashSetOfT(value));
                }
            }
        }
    }
}
