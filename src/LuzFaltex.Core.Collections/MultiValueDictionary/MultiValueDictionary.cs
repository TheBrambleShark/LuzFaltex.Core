//
//  MultiValueDictionary.cs
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
using System.Diagnostics.CodeAnalysis;

namespace LuzFaltex.Core.Collections
{
    /// <summary>
    /// A <see cref="MultiValueDictionary{TKey, TValue}"/> is a Dictionary that allows
    /// multiple values for any given unique key. While the <see cref="MultiValueDictionary{TKey, TValue}"/>
    /// API is mostly the same as that of a regular System.Collections.Dictionary, there
    /// is a distinction in that getting the value for a key returns a <see cref="IReadOnlyCollection{T}"/>
    /// of values rather than a single value associated with that key. Additionally, there
    /// is functionality to allow adding or removing more than a single value at once. The
    /// <see cref="MultiValueDictionary{TKey, TValue}"/> can also be viewed as a
    /// <c>IReadOnlyDictionary{TKey, IReadOnlyCollection{TValue}}"</c> where the
    /// <see cref="IReadOnlyCollection{T}"/> is abstracted from the view of the programmer.
    /// For a readonly MultiValueDictionary, see <see cref="System.Linq.ILookup{TKey, TElement}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    [DebuggerTypeProxy(typeof(MultiValueDictionary<,>.MultiValueDictionaryDebugView))]
    [DebuggerDisplay("Count = {Count}")]
    [Serializable]
    public partial class MultiValueDictionary<TKey, TValue>
        : IReadOnlyDictionary<TKey, IReadOnlyCollection<TValue>>,
          IEnumerable<KeyValuePair<TKey, IReadOnlyCollection<TValue>>>,
          IEnumerable,
          IReadOnlyCollection<KeyValuePair<TKey, IReadOnlyCollection<TValue>>>
        where TKey : notnull
    {
        private readonly Dictionary<TKey, InnerCollectionView> _dictionary;
        private Func<ICollection<TValue>> _newCollectionFactory = () => [];

        /// <summary>
        /// Gets the current version of this MultiValueDictionary used to determine modification during enumeration.
        /// </summary>
        private int _version;

        /// <inheritdoc/>
        public IEnumerable<TKey> Keys => _dictionary.Keys;

        /// <inheritdoc/>
        public IEnumerable<IReadOnlyCollection<TValue>> Values => _dictionary.Values;

        /// <summary>
        /// Gets a new <see cref="MultiValueDictionary{TKey, TValue}"/> factory.
        /// </summary>
        /// <typeparam name="TValueCollection">The type of the value collection to use.</typeparam>
        /// <returns>A <see cref="MultiValueDictionaryFactory{TValueCollection}"/>.</returns>
        public static MultiValueDictionaryFactory<TValueCollection> Factory<TValueCollection>()
            where TValueCollection : ICollection<TValue>, new()
            => new();

        /// <inheritdoc/>
        public IReadOnlyCollection<TValue> this[TKey key]
        {
            get
            {
                ArgumentNullException.ThrowIfNull(key);

                return _dictionary.TryGetValue(key, out var value)
                    ? (IReadOnlyCollection<TValue>)value
                    : throw new KeyNotFoundException();
            }
        }

        /// <inheritdoc/>
        public int Count => _dictionary.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiValueDictionary{TKey, TValue}"/> class.
        /// </summary>
        public MultiValueDictionary()
        {
            _dictionary = [];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiValueDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="capacity">Initial number of keys that the <see cref="MultiValueDictionary{TKey, TValue}"/>
        /// will allocate space for.</param>
        /// <exception cref="ArgumentOutOfRangeException">Capacity must be >= 0.</exception>
        public MultiValueDictionary(int capacity)
            : this(capacity, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiValueDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="comparer">Specified comparer to use for the <typeparamref name="TKey"/>s.</param>
        public MultiValueDictionary(IEqualityComparer<TKey>? comparer)
        {
            _dictionary = new(comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiValueDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="capacity">Initial number of keys that the <see cref="MultiValueDictionary{TKey, TValue}"/>
        /// will allocate space for.</param>
        /// <param name="comparer">Specified comparer to use for the <typeparamref name="TKey"/>s.</param>
        /// <exception cref="ArgumentOutOfRangeException">Capacity must be >= 0.</exception>
        public MultiValueDictionary(int capacity, IEqualityComparer<TKey>? comparer)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(capacity);
            _dictionary = new(capacity, comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiValueDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="enumerable">An IEnumerable to copy elements from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> must not be null.</exception>
        public MultiValueDictionary(IEnumerable<KeyValuePair<TKey, IReadOnlyCollection<TValue>>> enumerable)
            : this(enumerable, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiValueDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="enumerable">An IEnumerable to copy elements from.</param>
        /// <param name="comparer">Specified comparer to use for <typeparamref name="TKey"/>.</param>
        /// <exception cref="ArgumentNullException"><paramref name="enumerable"/> must not be null.</exception>
        /// <remarks>If <paramref name="comparer"/> is <see langword="null"/>, the default <see cref="IEqualityComparer"/> for <typeparamref name="TKey"/> is used.</remarks>
        public MultiValueDictionary(IEnumerable<KeyValuePair<TKey, IReadOnlyCollection<TValue>>> enumerable, IEqualityComparer<TKey>? comparer)
        {
            ArgumentNullException.ThrowIfNull(enumerable);

            _dictionary = new Dictionary<TKey, InnerCollectionView>(comparer);
            foreach (var pair in enumerable)
            {
                AddRange(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Adds the specified <paramref name="key"/> nad <paramref name="value"/> to the <see cref="MultiValueDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element.</param>
        /// <param name="value">Te value of the element.</param>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
        /// <remarks>
        /// Unlike <see cref="IDictionary{TKey, TValue}.Add(TKey, TValue)"/>, this method will not
        /// throw any exceptions. If the given <paramref name="key"/> is already in the collection,
        /// then the <paramref name="value"/> will be added to the collection associated with the key.
        /// </remarks>
        public void Add(TKey key, TValue value)
        {
            ArgumentNullException.ThrowIfNull(key);

            if (!_dictionary.TryGetValue(key, out var collection))
            {
                collection = new InnerCollectionView(key, _newCollectionFactory());
                _dictionary.Add(key, collection);
            }

            collection.Add(value);
            _version++;
        }

        /// <summary>
        /// Adds a numer of key-value pairs to this <see cref="MultiValueDictionary{TKey, TValue}"/>,
        /// where the key for each value is <paramref name="key"/>, and the value for a pair is an element from <paramref name="values"/>.
        /// </summary>
        /// <param name="key">The key of all entries.</param>
        /// <param name="values">A collection of values to add.</param>
        /// <exception cref="ArgumentNullException">Both <paramref name="key"/> and <paramref name="values"/> must be non-null.</exception>
        /// <remarks>
        /// A call to this <see cref="AddRange(TKey, IEnumerable{TValue})"/> method will always invalidate any currently running enumeration
        /// regardless of whether the AddRange method actually modified the <see cref="MultiValueDictionary{TKey, TValue}"/>.
        /// </remarks>
        public void AddRange(TKey key, IEnumerable<TValue> values)
        {
            ArgumentNullException.ThrowIfNull(key);
            ArgumentNullException.ThrowIfNull(values);

            if (!_dictionary.TryGetValue(key, out var collection))
            {
                collection = new InnerCollectionView(key, _newCollectionFactory());
                _dictionary.Add(key, collection);
            }

            foreach (TValue value in values)
            {
                collection.Add(value);
            }

            _version++;
        }

        /// <summary>
        /// Removes every value associated with the provided <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        /// <returns><see langword="true"/> if the removal was successful; otherwise, <see langword="false"/>.</returns>
        public bool Remove(TKey key)
        {
            ArgumentNullException.ThrowIfNull(key);

            if (_dictionary.TryGetValue(key, out var _) && _dictionary.Remove(key))
            {
                _version++;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the first instance (if any) of the given pair.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="value">The value of the element to remove.</param>
        /// <returns><see langword="true"/> if the removal was successful; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// If the <paramref name="value"/> being removed is the last one associated with its <paramref name="key"/>, then that
        /// key will also be removed and its associated <see cref="IReadOnlyCollection{T}"/> will be freed as if a call to
        /// <see cref="Remove(TKey)"/> had been made.</remarks>
        public bool Remove(TKey key, TValue value)
        {
            ArgumentNullException.ThrowIfNull(key);

            if (_dictionary.TryGetValue(key, out var collection) && collection.Remove(value))
            {
                if (collection.Count == 0)
                {
                    _dictionary.Remove(key);
                }

                _version++;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if the given <paramref name="key"/> and <paramref name="value"/> exist within this <see cref="MultiValueDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element.</param>
        /// <param name="value">The value of the element.</param>
        /// <returns><see langword="true"/> if the element was found; otherwise, <see langword="false"/>.</returns>
        public bool Contains(TKey key, TValue value)
        {
            ArgumentNullException.ThrowIfNull(key);

            if (_dictionary.TryGetValue(key, out var collection))
            {
                return collection.Contains(value);
            }

            return false;
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            ArgumentNullException.ThrowIfNull(key);
            return _dictionary.ContainsKey(key);
        }

        /// <summary>
        /// Determines if the given <paramref name="value"/> exists within this <see cref="MultiValueDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="value">The value to search for.</param>
        /// <returns><see langword="true"/> if the element was found; otherwise, <see langword="false"/>.</returns>
        public bool ContainsValue(TValue value)
        {
            ArgumentNullException.ThrowIfNull(value);

            foreach (InnerCollectionView view in _dictionary.Values)
            {
                if (view.Contains(value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, [NotNullWhen(true)] out IReadOnlyCollection<TValue>? value)
        {
            ArgumentNullException.ThrowIfNull(key);

            bool result = _dictionary.TryGetValue(key, out InnerCollectionView? view);
            value = view;
            return result;
        }

        /// <summary>
        /// Removes every value pair from the <see cref="MultiValueDictionary{TKey, TValue}"/>.
        /// </summary>
        public void Clear()
        {
            _dictionary.Clear();
            _version++;
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, IReadOnlyCollection<TValue>>> GetEnumerator()
            => new Enumerator(this);

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
