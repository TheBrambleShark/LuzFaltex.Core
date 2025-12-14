//
//  MultiValueDictionary.InnerCollectionView.cs
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
using System.Diagnostics;

namespace LuzFaltex.Core.Collections
{
    public partial class MultiValueDictionary<TKey, TValue> where TKey : notnull
    {
        /// <summary>
        /// Represents a view over the value collection.
        /// </summary>
        /// <remarks>
        /// Initializes a new instance of the <see cref="InnerCollectionView"/> class.
        /// </remarks>
        /// <param name="key">The key for this value.</param>
        /// <param name="collection">A collection of values.</param>
        [DebuggerTypeProxy(typeof(MultiValueDictionaryValueCollectionDebugView<,>))]
        [DebuggerDisplay("Key = {Key}, Count = {Count}")]
        private sealed class InnerCollectionView(TKey key, ICollection<TValue> collection)
            : ICollection<TValue>,
              IEnumerable<TValue>,
              IEnumerable,
              IReadOnlyCollection<TValue>
        {
            private readonly ICollection<TValue> _collection = collection;

            /// <inheritdoc/>
            public int Count => _collection.Count;

            /// <inheritdoc/>
            public bool IsReadOnly => true;

            /// <summary>
            /// Gets the key which identifies this collection.
            /// </summary>
            public TKey Key { get; init; } = key;

            /// <inheritdoc/>
            public void Add(TValue item)
                => _collection.Add(item);

            /// <inheritdoc/>
            public bool Remove(TValue item)
                => _collection.Remove(item);

            /// <inheritdoc/>
            public bool Contains(TValue item)
                => _collection.Contains(item);

            /// <inheritdoc/>
            public void CopyTo(TValue[] array, int arrayIndex)
            {
                ArgumentNullException.ThrowIfNull(nameof(array));
#if NET8_0_OR_GREATER
                ArgumentOutOfRangeException.ThrowIfNegative(arrayIndex);
                ArgumentOutOfRangeException.ThrowIfGreaterThan(arrayIndex, array.Length);
                ArgumentOutOfRangeException.ThrowIfLessThan(array.Length - arrayIndex, _collection.Count);
#else
                if (arrayIndex < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Value must be a non-negative number.");
                }

                if (arrayIndex > array.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex), $"{nameof(arrayIndex)} must not be greater than the length of {nameof(array)}.");
                }

                if (array.Length - arrayIndex < _collection.Count)
                {
                    throw new ArgumentOutOfRangeException("Target array is too small!");
                }
#endif

                _collection.CopyTo(array, arrayIndex);
            }

            /// <inheritdoc/>
            public IEnumerator<TValue> GetEnumerator()
            {
                return _collection.GetEnumerator();
            }

            /// <inheritdoc/>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            /// <inheritdoc/>
            void ICollection<TValue>.Clear()
            {
                _collection.Clear();
            }
        }
    }
}
