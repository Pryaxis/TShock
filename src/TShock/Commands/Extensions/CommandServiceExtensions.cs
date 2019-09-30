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
using TShock.Commands.Parsers;

namespace TShock.Commands.Extensions {
    /// <summary>
    /// Provides extensions to the <see cref="ICommandService"/> interface.
    /// </summary>
    public static class CommandServiceExtensions {
        /// <summary>
        /// Registers the given strongly-typed parser as the definitive parser for the parse type.
        /// </summary>
        /// <typeparam name="TParse">The parse type.</typeparam>
        /// <param name="commandService">The command service.</param>
        /// <param name="parser">The parser.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="commandService"/> or <paramref name="parser"/> are <c>null</c>.
        /// </exception>
        public static void RegisterParser<TParse>(this ICommandService commandService, IArgumentParser<TParse> parser) {
            if (commandService is null) throw new ArgumentNullException(nameof(commandService));
            if (parser is null) throw new ArgumentNullException(nameof(parser));

            commandService.RegisterParser(typeof(TParse), parser);
        }
    }
}
