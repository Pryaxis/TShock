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
using System.Diagnostics;
using System.Reflection;

namespace TShock.Commands {
    internal class TShockCommand : ICommand {
        private readonly ICommandService _commandService;
        private readonly CommandHandlerAttribute _attribute;

        public string Name => _attribute.CommandName;
        public IEnumerable<string> SubNames => _attribute.CommandSubNames;
        public object? HandlerObject { get; }
        public MethodBase Handler { get; }

        public TShockCommand(ICommandService commandService, CommandHandlerAttribute attribute, object? handlerObject,
                             MethodBase handler) {
            Debug.Assert(commandService != null, "commandService != null");
            Debug.Assert(attribute != null, "attribute != null");
            Debug.Assert(handler != null, "handler != null");

            _commandService = commandService;
            _attribute = attribute;
            HandlerObject = handlerObject;
            Handler = handler;
        }

        public void Invoke(ICommandSender sender, string input) {
            throw new NotImplementedException();
        }
    }
}
