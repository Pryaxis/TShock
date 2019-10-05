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
using System.Text;
using TShock.Events.Commands;
using TShock.Utils.Extensions;

namespace TShock.Commands {
    internal sealed class CommandProcessor : IDisposable {
        private readonly ICommandService _commandService;

        // Lookup table that maps command name -> set of qualified command names.
        private readonly Dictionary<string, ISet<string>> _qualifiedCommandNameLookup =
            new Dictionary<string, ISet<string>>();

        public CommandProcessor(ICommandService commandService) {
            _commandService = commandService;

            _commandService.CommandRegister += CommandRegisterHandler;
            _commandService.CommandUnregister += CommandUnregisterHandler;
        }

        public void Dispose() {
            _commandService.CommandRegister -= CommandRegisterHandler;
            _commandService.CommandUnregister -= CommandUnregisterHandler;
        }

        private void CommandRegisterHandler(object sender, CommandRegisterEventArgs args) {
            var command = args.Command;

            // First, fill in our qualified command name lookup table.
            var colon = command.QualifiedName.IndexOf(':');
            var name = command.QualifiedName.Substring(colon + 1);
            var qualifiedCommandNames = _qualifiedCommandNameLookup.GetValueOrDefault(
                name, () => new HashSet<string>());

            if (!_qualifiedCommandNameLookup.TryGetValue(name, out var commands)) {
                _qualifiedCommandNameLookup[name] = commands = new HashSet<string>();
            }

            commands.Add(command.QualifiedName);

        }

        private void CommandUnregisterHandler(object sender, CommandUnregisterEventArgs args) {

        }
    }
}
