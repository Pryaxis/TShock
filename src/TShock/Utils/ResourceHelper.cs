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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace TShock.Utils {
    /// <summary>
    /// Provides helper methods for loading resources.
    /// </summary>
    public static class ResourceHelper {
        /// <summary>
        /// Loads a resource from the <paramref name="resourceType"/> with the given <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="T">The type of resource.</typeparam>
        /// <param name="resourceType">The resource type.</param>
        /// <param name="name">The name.</param>
        /// <returns>The resource.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="name"/> is not a property of <paramref name="resourceType"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="resourceType"/> or <paramref name="name"/> are <see langword="null"/>.
        /// </exception>
        [SuppressMessage(
            "Globalization", "CA1303:Do not pass literals as localized parameters",
            Justification = "strings are not user-facing")]
        public static T LoadResource<T>(Type resourceType, string name) {
            if (resourceType is null) {
                throw new ArgumentNullException(nameof(resourceType));
            }

            if (name is null) {
                throw new ArgumentNullException(nameof(name));
            }

            var property = resourceType.GetProperty(name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (property is null) {
                throw new ArgumentException("Property does not exist.", nameof(name));
            }

            return (T)property.GetValue(null);
        }
    }
}
