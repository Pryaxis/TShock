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

using System.Diagnostics.CodeAnalysis;

namespace TShock.Logging.Themes {
    /// <summary>
    /// Specifies an entity styled by a player log theme.
    /// </summary>
    [SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "ignored")]
    public enum PlayerLogThemeStyle {
        /// <summary>
        /// Represents text.
        /// </summary>
        Text,

        /// <summary>
        /// Represents a separator in, e.g., a sequence or structure.
        /// </summary>
        Separator,

        /// <summary>
        /// Represents <see langword="null"/>.
        /// </summary>
        Null,

        /// <summary>
        /// Represents a <see langword="bool"/>.
        /// </summary>
        Boolean,

        /// <summary>
        /// Represents a <see langword="string"/>.
        /// </summary>
        String,

        /// <summary>
        /// Represents a <see langword="char"/>.
        /// </summary>
        Character,

        /// <summary>
        /// Represents a number.
        /// </summary>
        Number,

        /// <summary>
        /// Represents all other scalar values.
        /// </summary>
        Scalar,

        /// <summary>
        /// Represents a property identifier.
        /// </summary>
        Identifier,

        /// <summary>
        /// Represents a structure type.
        /// </summary>
        Type,

        /// <summary>
        /// Represents a timestamp.
        /// </summary>
        Timestamp,

        /// <summary>
        /// Represents an exception.
        /// </summary>
        Exception,

        /// <summary>
        /// Represents the verbose level indicator.
        /// </summary>
        VerboseLevel,

        /// <summary>
        /// Represents the debug level indicator.
        /// </summary>
        DebugLevel,

        /// <summary>
        /// Represents the information level indicator.
        /// </summary>
        InformationLevel,

        /// <summary>
        /// Represents the warning level indicator.
        /// </summary>
        WarningLevel,

        /// <summary>
        /// Represents the error level indicator.
        /// </summary>
        ErrorLevel,

        /// <summary>
        /// Represents the fatal level indicator.
        /// </summary>
        FatalLevel,

        /// <summary>
        /// Represents something invalid: e.g., an unknown level indicator or a missing property.
        /// </summary>
        Invalid
    }
}
