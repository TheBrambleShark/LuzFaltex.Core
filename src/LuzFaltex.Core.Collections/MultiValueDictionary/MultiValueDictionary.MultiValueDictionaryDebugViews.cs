//
//  MultiValueDictionary.MultiValueDictionaryDebugViews.cs
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
using System.Diagnostics;

#pragma warning disable SA1600 // Elements must be documented. Only exposed internally.

namespace LuzFaltex.Core.Collections
{
    public partial class MultiValueDictionary<TKey, TValue> where TKey : notnull
    {
        internal sealed class MultiValueDictionaryDebugView<TKey, TValue>(IDictionary<TKey, IReadOnlyCollection<TValue>> dictionary)
            where TKey : notnull
        {
            private readonly IDictionary<TKey, IReadOnlyCollection<TValue>> _dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public DebugViewMultiValueDictionaryItem<TKey, TValue>[] Items
            {
                get
                {
                    var keyValuePairs = new KeyValuePair<TKey, IReadOnlyCollection<TValue>>[_dictionary.Count];
                    _dictionary.CopyTo(keyValuePairs, 0);
                    var items = new DebugViewMultiValueDictionaryItem<TKey, TValue>[keyValuePairs.Length];
                    for (int i = 0; i < items.Length; i++)
                    {
                        items[i] = new DebugViewMultiValueDictionaryItem<TKey, TValue>(keyValuePairs[i]);
                    }
                    return items;
                }
            }
        }

        internal readonly struct DebugViewMultiValueDictionaryItem<TKey, TValue>(TKey key, IReadOnlyCollection<TValue> values)
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Collapsed)]
            public TKey Key { get; } = key;

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public IReadOnlyCollection<TValue> Values { get; } = values;

            public DebugViewMultiValueDictionaryItem(KeyValuePair<TKey, IReadOnlyCollection<TValue>> kvp)
                : this(kvp.Key, kvp.Value)
            {
            }
        }

        internal sealed class MultiValueDictionaryValueCollectionDebugView<TKey, TValue>(ICollection<TValue> collection)
        {
            private readonly ICollection<TValue> _collection = collection ?? throw new ArgumentNullException(nameof(collection));

            /// <summary>
            /// Gets the items in this collection.
            /// </summary>
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public ICollection<TValue> Items
            {
                get
                {
                    TValue[] items = new TValue[_collection.Count];
                    _collection.CopyTo(items, 0);
                    return items;
                }
            }
        }
    }
}
