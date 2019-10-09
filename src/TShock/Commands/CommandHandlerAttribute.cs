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
using JetBrains.Annotations;
using TShock.Utils;

namespace TShock.Commands {
    /// <summary>
    /// Specifies that a method is a command handler. This controls many aspects of the command, and can be applied
    /// multiple times to provide aliasing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    [MeansImplicitUse]
    public sealed class CommandHandlerAttribute : Attribute {
        private readonly string _qualifiedName;
        private string? _helpText;
        private string? _usageText;
        private Type? _resourceType;

        /// <summary>
        /// Gets the qualified name. This includes the namespace: e.g., "tshock:kick".
        /// </summary>
        public string QualifiedName => GetResourceStringMaybe(_qualifiedName);

        /// <summary>
        /// Gets or sets the help text. If <see langword="null"/>, then no help text exists. This will show up in the
        /// /help command.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        [DisallowNull]
        public string? HelpText {
            get => GetResourceStringMaybe(_helpText);
            set => _helpText = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets or sets the usage text. If <see langword="null"/>, then no usage text exists. This will show up in the
        /// /help command and when invalid syntax is used.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
        [DisallowNull]
        public string? UsageText {
            get => GetResourceStringMaybe(_usageText);
            set => _usageText = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets or sets the resource type to load localizable strings from. If <see langword="null"/>, then no
        /// localization will occur.
        /// </summary>
        public Type? ResourceType {
            get => _resourceType;
            set => _resourceType = value ?? throw new ArgumentNullException(nameof(value));
        }

        [return: NotNullIfNotNull("str")]
        private string? GetResourceStringMaybe(string? str) {
            if (str is null) {
                return null;
            }

            return _resourceType != null ? ResourceHelper.LoadResource<string>(_resourceType, str) : str;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandHandlerAttribute"/> class with the specified qualified
        /// name.
        /// </summary>
        /// <param name="qualifiedName">
        /// The qualified name. This must include the namespace: e.g., "tshock:kick".
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="qualifiedName"/> is <see langword="null"/>.
        /// </exception>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters",
            Justification = "strings are not user-facing")]
        public CommandHandlerAttribute(string qualifiedName) {
            if (qualifiedName is null) {
                throw new ArgumentNullException(nameof(qualifiedName));
            }

            _qualifiedName = qualifiedName;
        }
    }
}
