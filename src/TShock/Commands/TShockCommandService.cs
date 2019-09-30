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
using System.Reflection;
using Orion;
using Orion.Events;
using TShock.Events.Commands;

namespace TShock.Commands {
    internal sealed class TShockCommandService : OrionService, ICommandService {
        private readonly ISet<ICommand> _commands = new HashSet<ICommand>();

        public EventHandlerCollection<CommandExecuteEventArgs>? CommandRegister { get; set; }
        public EventHandlerCollection<CommandExecuteEventArgs>? CommandExecute { get; set; }
        public EventHandlerCollection<CommandExecuteEventArgs>? CommandUnregister { get; set; }

        public IReadOnlyCollection<ICommand> RegisterCommands(object obj) {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<ICommand> RegisterCommands(Type type) {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<ICommand> FindCommands(string commandName, params string[] commandSubNames) {
            throw new NotImplementedException();
        }

        public bool UnregisterCommand(ICommand command) {
            throw new NotImplementedException();
        }

        private ICommand RegisterCommand(object commandHandlerObject, MethodBase commandHandler) {
            throw new NotImplementedException();
        }
    }
}
