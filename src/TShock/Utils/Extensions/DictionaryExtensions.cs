// Copyright (c) 2019 Pryaxis & TShock Contributors
// 
// This file is part of TShock.
// 
// TShock is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// TShock is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with TShock.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;

namespace TShock.Utils.Extensions {
    /// <summary>
    /// Provides extensions for the <see cref="IDictionary{TKey, TValue}"/> interface.
    /// </summary>
    public static class DictionaryExtensions {
        /// <summary>
        /// Gets the value corresponding to the given <paramref name="key"/>, using the
        /// <paramref name="defaultValueProvider"/> if <paramref name="key"/> does not exist.
        /// </summary>
        /// <typeparam name="TKey">The type of key.</typeparam>
        /// <typeparam name="TValue">The type of value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValueProvider">
        /// The default value provider. If <see langword="null"/>, then the provider will return a default instance of
        /// <typeparamref name="TValue"/>.
        /// </param>
        /// <param name="createIfNotExists">
        /// <see langword="true"/> to create the annotation if it does not exist; otherwise, <see langword="false"/>.
        /// </param>
        /// <returns>
        /// The value, or a default instance provided by <paramref name="defaultValueProvider"/> if
        /// <paramref name="key"/> does not exist in the dictionary.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dictionary"/> or <paramref name="defaultValueProvider"/> are <see langword="null"/>.
        /// </exception>
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
                Func<TValue>? defaultValueProvider = null, bool createIfNotExists = false) {
            if (dictionary is null) {
                throw new ArgumentNullException(nameof(dictionary));
            }

            if (dictionary.TryGetValue(key, out var value)) {
                return value;
            }

            var provider = defaultValueProvider ?? (() => default!);
            return createIfNotExists ? (TValue)(dictionary[key] = provider())! : provider();
        }
    }
}
