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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Microsoft.Xna.Framework;
using Orion;
using Orion.Events;
using Orion.Events.Players;
using Orion.Events.Server;
using Orion.Players;
using Serilog;
using TShock.Commands;
using TShock.Commands.Exceptions;
using TShock.Commands.Parsers;
using TShock.Commands.Parsers.Attributes;
using TShock.Events.Commands;
using TShock.Properties;
using TShock.Utils.Extensions;

namespace TShock.Modules {
    /// <summary>
    /// Represents TShock's command module. Provides command functionality and core commands.
    /// </summary>
    internal sealed class CommandModule : TShockModule {
        private readonly OrionKernel _kernel;
        private readonly ILogger _log;
        private readonly IPlayerService _playerService;
        private readonly ICommandService _commandService;

        private readonly IDictionary<string, string> _terrariaCommandToCommand = new Dictionary<string, string> {
            ["Say"] = string.Empty,
            ["Emote"] = Resources.TerrariaCommand_Me,
            ["Party"] = Resources.TerrariaCommand_P,
            ["Playing"] = Resources.TerrariaCommand_Playing,
            ["Roll"] = Resources.TerrariaCommand_Roll
        };

        private readonly IDictionary<string, ISet<string>> _nameToQualifiedName
            = new Dictionary<string, ISet<string>>();

        private readonly Random _rand = new Random();

        public CommandModule(
                OrionKernel kernel, ILogger log, IPlayerService playerService, ICommandService commandService) {
            Debug.Assert(kernel != null, "kernel should not be null");
            Debug.Assert(log != null, "log should not be null");
            Debug.Assert(playerService != null, "player service should not be null");
            Debug.Assert(commandService != null, "command service should not be null");

            _kernel = kernel;
            _log = log;
            _playerService = playerService;
            _commandService = commandService;

            _kernel.RegisterHandlers(this, _log);
        }

        public override void Initialize() {
            _commandService.RegisterParser(new Int32Parser());
            _commandService.RegisterParser(new DoubleParser());
            _commandService.RegisterParser(new StringParser());
            _commandService.RegisterCommands(this);
        }

        // Executes a command. input should not have the leading /.
        public void ExecuteCommand(ICommandSender commandSender, string input) {
            var space = input.IndexOf(' ', StringComparison.Ordinal);
            if (space < 0) {
                space = input.Length;
            }

            var commandName = input.Substring(0, space);
            ICommand command;
            try {
                var qualifiedName = GetQualifiedName(commandName);
                command = _commandService.Commands[qualifiedName];
            } catch (CommandNotFoundException ex) {
                commandSender.SendErrorMessage(ex.Message);
                return;
            }

            if (command.ShouldBeLogged) {
                Log.Information(Resources.Log_ExecutingCommand, commandSender.Name, input);
            }

            try {
                command.Invoke(commandSender, input.Substring(space));
            } catch (CommandParseException ex) {
                commandSender.SendErrorMessage(ex.Message);
                commandSender.SendInfoMessage(
                    string.Format(CultureInfo.InvariantCulture, command.UsageText, commandName));
            } catch (CommandExecuteException ex) {
                commandSender.SendErrorMessage(ex.Message);
            }
        }

        // Gets the qualified command name for a possibly-qualified command name.
        public string GetQualifiedName(string maybeQualifiedName) {
            if (string.IsNullOrWhiteSpace(maybeQualifiedName)) {
                throw new CommandNotFoundException(Resources.CommandNotFound_MissingCommand);
            }

            var isQualifiedName = maybeQualifiedName.IndexOf(':', StringComparison.Ordinal) >= 0;
            if (isQualifiedName) {
                if (!_commandService.Commands.ContainsKey(maybeQualifiedName)) {
                    throw new CommandNotFoundException(
                        string.Format(CultureInfo.InvariantCulture, Resources.CommandNotFound_UnrecognizedCommand,
                            maybeQualifiedName));
                }

                return maybeQualifiedName;
            }

            var qualifiedNames = _nameToQualifiedName.GetValueOrDefault(maybeQualifiedName, () => new HashSet<string>());
            if (qualifiedNames.Count == 0) {
                throw new CommandNotFoundException(
                    string.Format(CultureInfo.InvariantCulture, Resources.CommandNotFound_UnrecognizedCommand,
                        maybeQualifiedName));
            }

            if (qualifiedNames.Count > 1) {
                throw new CommandNotFoundException(
                    string.Format(CultureInfo.InvariantCulture, Resources.CommandNotFound_AmbiguousName,
                        maybeQualifiedName, string.Join(", ", qualifiedNames.Select(n => $"\"/{n}\""))));
            }

            return qualifiedNames.Single();
        }

        protected override void Dispose(bool disposeManaged) => _kernel.UnregisterHandlers(this, _log);

        [EventHandler(Name = "tshock")]
        [SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Implicit usage")]
        private void ServerCommandHandler(ServerCommandEvent e) {
            if (e.IsCanceled()) {
                return;
            }

            e.Cancel("tshock: command executing");

            var input = e.Input;
            if (input.StartsWith('/')) {
                input = input.Substring(1);
            }

            ExecuteCommand(ConsoleCommandSender.Instance, input);
        }

        [EventHandler(Name = "tshock")]
        [SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Implicit usage")]
        private void PlayerChatHandler(PlayerChatEvent e) {
            if (e.IsCanceled()) {
                return;
            }

            var chatCommand = e.Command;
            if (!_terrariaCommandToCommand.TryGetValue(chatCommand, out var canonicalCommand)) {
                e.Cancel("tshock: Terraria command is invalid");
                return;
            }

            var chat = canonicalCommand + e.Text;
            if (chat.StartsWith('/')) {
                e.Cancel("tshock: command executing");

                ICommandSender commandSender = e.Player.GetAnnotationOrDefault(
                    "tshock:CommandSender",
                    () => new PlayerCommandSender(e.Player), true);
                var input = chat.Substring(1);
                ExecuteCommand(commandSender, input);
            }
        }

        [EventHandler(EventPriority.Monitor, Name = "tshock")]
        [SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Implicit usage")]
        private void CommandRegisterHandler(CommandRegisterEvent e) {
            var qualifiedName = e.Command.QualifiedName;
            var name = qualifiedName.Substring(qualifiedName.IndexOf(':', StringComparison.Ordinal) + 1);
            _nameToQualifiedName
                .GetValueOrDefault(name, () => new HashSet<string>(), true)
                .Add(qualifiedName);
        }

        [EventHandler(EventPriority.Monitor, Name = "tshock")]
        [SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Implicit usage")]
        private void CommandUnregisterHandler(CommandUnregisterEvent e) {
            var qualifiedName = e.Command.QualifiedName;
            var name = qualifiedName.Substring(qualifiedName.IndexOf(':', StringComparison.Ordinal) + 1);
            _nameToQualifiedName
                .GetValueOrDefault(name, () => new HashSet<string>(), true)
                .Remove(qualifiedName);
        }

        // =============================================================================================================
        // Core command implementations below:
        //

        [CommandHandler(nameof(Resources.Command_Help),
            HelpText = nameof(Resources.Command_Help_HelpText),
            UsageText = nameof(Resources.Command_Help_UsageText),
            ResourceType = typeof(Resources))]
        public void Help(ICommandSender sender, string? command_name = null) {
            if (command_name is null) {
                string FormatCommandName(ICommand command) {
                    var qualifiedName = command.QualifiedName;
                    var name = qualifiedName.Substring(qualifiedName.IndexOf(':', StringComparison.Ordinal) + 1);
                    return _nameToQualifiedName[name].Count > 1 ? qualifiedName : name;
                }

                sender.SendInfoMessage(Resources.Command_Help_Header);
                sender.SendInfoMessage(
                    string.Join(", ", _commandService.Commands.Values.Select(c => $"/{FormatCommandName(c)}")));
                return;
            }

            if (command_name.StartsWith('/')) {
                command_name = command_name.Substring(1);
            }

            string qualifiedName;
            try {
                qualifiedName = GetQualifiedName(command_name);
            } catch (CommandNotFoundException ex) {
                sender.SendErrorMessage(ex.Message);
                return;
            }

            var command = _commandService.Commands[qualifiedName];
            var usageText = string.Format(CultureInfo.InvariantCulture, command.UsageText, command_name);
            sender.SendInfoMessage($"/{command_name}:\n{command.HelpText}\n{usageText}");
        }

        [CommandHandler(nameof(Resources.Command_Playing),
            HelpText = nameof(Resources.Command_Playing_HelpText),
            UsageText = nameof(Resources.Command_Playing_UsageText),
            ResourceType = typeof(Resources))]
        public void Playing(ICommandSender sender, [Flag("i")] bool showIds) {
            var onlinePlayers = _playerService.Players.Where(p => p.IsActive).ToList();
            if (onlinePlayers.Count == 0) {
                sender.SendInfoMessage(Resources.Command_Playing_NoPlayers);
                return;
            }

            sender.SendInfoMessage(Resources.Command_Playing_Header);
            if (showIds) {
                sender.SendInfoMessage(string.Join(", ", onlinePlayers.Select(p => $"[{p.Index}] {p.Name}")));
            } else {
                sender.SendInfoMessage(string.Join(", ", onlinePlayers.Select(p => p.Name)));
            }
        }

        [CommandHandler(nameof(Resources.Command_Me),
            HelpText = nameof(Resources.Command_Me_HelpText),
            UsageText = nameof(Resources.Command_Me_UsageText),
            ResourceType = typeof(Resources))]
        public void Me(ICommandSender sender, [RestOfInput] string text) {
            _playerService.BroadcastMessage(
                string.Format(CultureInfo.InvariantCulture, Resources.Command_Me_Message, sender.Name, text),
                new Color(0xc8, 0x64, 0x00));
            Log.Information(Resources.Log_Command_Me_Message, sender.Name, text);
        }

        [CommandHandler(nameof(Resources.Command_Roll),
            HelpText = nameof(Resources.Command_Roll_HelpText),
            UsageText = nameof(Resources.Command_Roll_UsageText),
            ResourceType = typeof(Resources))]
        public void Roll(ICommandSender sender) {
            var num = _rand.Next(1, 101);
            _playerService.BroadcastMessage(
                string.Format(CultureInfo.InvariantCulture, Resources.Command_Roll_Message, sender.Name, num),
                new Color(0xff, 0xf0, 0x14));
            Log.Information(Resources.Log_Command_Roll_Message, sender.Name, num);
        }

        [CommandHandler(nameof(Resources.Command_P),
            HelpText = nameof(Resources.Command_P_HelpText),
            UsageText = nameof(Resources.Command_P_UsageText),
            ResourceType = typeof(Resources))]
        public void P(ICommandSender sender, [RestOfInput] string text) {
            var player = sender.Player;
            if (player is null) {
                sender.SendErrorMessage(Resources.Command_P_NotAPlayer);
                return;
            }

            var team = player.Team;
            if (team == PlayerTeam.None) {
                sender.SendErrorMessage(Resources.Command_P_NotInTeam);
                return;
            }

            var teamColor = team.Color();
            var teamPlayers = _playerService.Players.Where(p => p.IsActive && p.Team == team);
            foreach (var teamPlayer in teamPlayers) {
                teamPlayer.SendMessageFrom(player, text, teamColor);
            }
            Log.Information(Resources.Log_Command_P_Message, player.Name, team, text);
        }
    }
}
