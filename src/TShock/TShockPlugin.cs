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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Orion;
using Orion.Events;
using Orion.Events.Extensions;
using Orion.Events.Players;
using Orion.Events.Server;
using Orion.Players;
using Serilog;
using TShock.Commands;
using TShock.Properties;

namespace TShock {
    /// <summary>
    /// Represents the TShock plugin.
    /// </summary>
    public sealed class TShockPlugin : OrionPlugin {
        // Map from Terraria command -> canonical command. This is used to unify Terraria and TShock commands.
        private static readonly IDictionary<string, string> _canonicalCommands = new Dictionary<string, string> {
            ["Say"] = "",
            ["Emote"] = "/me ",
            ["Party"] = "/p ",
            ["Playing"] = "/playing ",
            ["Roll"] = "/roll "
        };

        private readonly Lazy<IPlayerService> _playerService;
        private readonly Lazy<ICommandService> _commandService;

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string Author => "Pryaxis";

        /// <inheritdoc />
        [ExcludeFromCodeCoverage]
        public override string Name => "TShock";

        private IPlayerService PlayerService => _playerService.Value;
        private ICommandService CommandService => _commandService.Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="TShockPlugin"/> class with the specified Orion kernel and
        /// services.
        /// </summary>
        /// <param name="kernel">The Orion kernel.</param>
        /// <param name="playerService">The player service.</param>
        /// <param name="commandService">The command service.</param>
        /// <exception cref="ArgumentNullException">Any of the services are <see langword="null"/>.</exception>
        public TShockPlugin(OrionKernel kernel, Lazy<IPlayerService> playerService,
                Lazy<ICommandService> commandService) : base(kernel) {
            Kernel.Bind<ICommandService>().To<TShockCommandService>();

            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        }

        /// <inheritdoc />
        protected override void Initialize() {
            Kernel.ServerCommand += ServerCommandHandler;

            PlayerService.PlayerChat += PlayerChatHandler;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposeManaged) {
            if (!disposeManaged) {
                return;
            }

            Kernel.ServerCommand -= ServerCommandHandler;
            
            PlayerService.PlayerChat -= PlayerChatHandler;
        }

        [EventHandler(EventPriority.Lowest)]
        private void ServerCommandHandler(object sender, ServerCommandEventArgs args) {
            args.Cancel("tshock: command executing");

            var input = args.Input;
            if (input.StartsWith('/')) {
                input = input.Substring(1);
            }

            ExecuteCommand(new ConsoleCommandSender(input), input);
        }

        [EventHandler(EventPriority.Lowest)]
        private void PlayerChatHandler(object sender, PlayerChatEventArgs args) {
            if (args.IsCanceled()) {
                return;
            }

            var chatCommand = args.ChatCommand;
            if (!_canonicalCommands.TryGetValue(chatCommand, out var canonicalCommand)) {
                args.Cancel("tshock: Terraria command is invalid");
                return;
            }

            var chat = canonicalCommand + args.ChatText;
            if (chat.StartsWith('/')) {
                args.Cancel("tshock: command executing");

                var input = chat.Substring(1);
                ExecuteCommand(new PlayerCommandSender(args.Player, input), input);
            }
        }

        // Executes a command. input should not have the leading /.
        private void ExecuteCommand(ICommandSender commandSender, ReadOnlySpan<char> input) {
            Log.Information("{Sender} is executing /{Command}", commandSender.Name, input.ToString());

            ICommand command;
            try {
                command = CommandService.FindCommand(ref input);
            } catch (CommandParseException ex) {
                commandSender.SendErrorMessage(
                    string.Format(CultureInfo.InvariantCulture, Resources.Command_BadCommand, ex.Message));
                return;
            }

            try {
                command.Invoke(commandSender, input);
            } catch (CommandParseException ex) {
                commandSender.SendErrorMessage(ex.Message);
            } catch (CommandExecuteException ex) {
                commandSender.SendErrorMessage(ex.Message);
            }
        }
    }
}
