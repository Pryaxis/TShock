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
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Orion;
using Orion.Events;
using Orion.Players;
using Serilog;
using TShock.Commands.Parsers;
using TShock.Commands.Parsers.Attributes;
using TShock.Events.Commands;
using TShock.Properties;
using TShock.Utils.Extensions;

namespace TShock.Commands {
    internal sealed class TShockCommandService : OrionService, ICommandService {
        private readonly Lazy<IPlayerService> _playerService;

        private readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();
        private readonly Dictionary<Type, IArgumentParser> _parsers = new Dictionary<Type, IArgumentParser>();
        private readonly IDictionary<string, ISet<string>> _qualifiedNames = new Dictionary<string, ISet<string>>();
        private readonly Random _rand = new Random();

        public IReadOnlyDictionary<string, ICommand> Commands => _commands;
        public IReadOnlyDictionary<Type, IArgumentParser> Parsers => _parsers;
        public EventHandlerCollection<CommandRegisterEventArgs>? CommandRegister { get; set; }
        public EventHandlerCollection<CommandExecuteEventArgs>? CommandExecute { get; set; }
        public EventHandlerCollection<CommandUnregisterEventArgs>? CommandUnregister { get; set; }

        private IPlayerService PlayerService => _playerService.Value;

        public TShockCommandService(Lazy<IPlayerService> playerService) {
            _playerService = playerService;

            RegisterParser(new Int32Parser());
            RegisterParser(new StringParser());
            RegisterCommands(this);
        }

        public IReadOnlyCollection<ICommand> RegisterCommands(object handlerObject) {
            if (handlerObject is null) {
                throw new ArgumentNullException(nameof(handlerObject));
            }

            ICommand? RegisterCommand(ICommand command) {
                var args = new CommandRegisterEventArgs(command);
                CommandRegister?.Invoke(this, args);
                if (args.IsCanceled()) {
                    return null;
                }

                var qualifiedName = command.QualifiedName;
                var name = qualifiedName.Substring(qualifiedName.IndexOf(':', StringComparison.Ordinal) + 1);
                _qualifiedNames[name] = _qualifiedNames.GetValueOrDefault(name, () => new HashSet<string>());
                _qualifiedNames[name].Add(qualifiedName);

                _commands.Add(qualifiedName, command);
                return command;
            }

            return handlerObject.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .SelectMany(m => m.GetCustomAttributes<CommandHandlerAttribute>(),
                    (m, a) => RegisterCommand(new TShockCommand(this, a, handlerObject, m)))
                .Where(c => c != null).ToList()!;
        }

        public void RegisterParser<TParse>(IArgumentParser<TParse> parser) =>
            _parsers[typeof(TParse)] = parser ?? throw new ArgumentNullException(nameof(parser));

        public ICommand FindCommand(ref ReadOnlySpan<char> input) {
            string ProcessQualifiedName(ref ReadOnlySpan<char> input) {
                input = input.TrimStart();
                if (input.IsEmpty) {
                    throw new CommandParseException(Resources.CommandParse_MissingCommand);
                }

                var space = input.IndexOfOrEnd(' ');
                var maybeQualifiedName = input[..space].ToString();
                var isQualifiedName = maybeQualifiedName.IndexOf(':', StringComparison.Ordinal) >= 0;
                input = input[space..];
                if (isQualifiedName) {
                    return maybeQualifiedName;
                }

                var qualifiedNames = _qualifiedNames.GetValueOrDefault(maybeQualifiedName, () => new HashSet<string>());
                if (qualifiedNames.Count == 0) {
                    throw new CommandParseException(
                        string.Format(CultureInfo.InvariantCulture, Resources.CommandParse_UnrecognizedCommand,
                            maybeQualifiedName));
                }

                if (qualifiedNames.Count > 1) {
                    throw new CommandParseException(
                        string.Format(CultureInfo.InvariantCulture, Resources.CommandParse_AmbiguousName,
                            maybeQualifiedName, string.Join(", ", qualifiedNames.Select(n => $"\"/{n}\""))));
                }

                return qualifiedNames.Single();
            }

            var qualifiedName = ProcessQualifiedName(ref input);
            if (!_commands.TryGetValue(qualifiedName, out var command)) {
                throw new CommandParseException(
                    string.Format(CultureInfo.InvariantCulture, Resources.CommandParse_UnrecognizedCommand,
                        qualifiedName));
            }

            return command;
        }

        public bool UnregisterCommand(ICommand command) {
            if (command is null) {
                throw new ArgumentNullException(nameof(command));
            }

            var args = new CommandUnregisterEventArgs(command);
            CommandUnregister?.Invoke(this, args);
            if (args.IsCanceled()) {
                return false;
            }

            var qualifiedName = command.QualifiedName;
            if (!_commands.Remove(qualifiedName)) {
                return false;
            }

            var name = qualifiedName.Substring(qualifiedName.IndexOf(':', StringComparison.Ordinal) + 1);
            Debug.Assert(_qualifiedNames.ContainsKey(name), "qualified names should contain command name");
            _qualifiedNames[name].Remove(qualifiedName);
            return true;
        }
        
        [CommandHandler(nameof(Resources.Command_Help),
            HelpText = nameof(Resources.Command_Help_HelpText),
            UsageText = nameof(Resources.Command_Help_UsageText),
            ResourceType = typeof(Resources))]
        public void Help(ICommandSender sender) {
            string FormatCommandName(ICommand command) {
                var qualifiedName = command.QualifiedName;
                var name = qualifiedName.Substring(qualifiedName.IndexOf(':', StringComparison.Ordinal) + 1);
                Debug.Assert(_qualifiedNames.ContainsKey(name), "qualified names should contain command name");

                return "/" + (_qualifiedNames[name].Count > 1 ? qualifiedName : name);
            }

            sender.SendInfoMessage(string.Join(", ", _commands.Values.Select(FormatCommandName)));
        }
        
        [CommandHandler(nameof(Resources.Command_Playing),
            HelpText = nameof(Resources.Command_Playing_HelpText),
            UsageText = nameof(Resources.Command_Playing_UsageText),
            ResourceType = typeof(Resources))]
        public void Playing(ICommandSender sender, [Flag("i")] bool showIds) {
            var onlinePlayers = PlayerService.Players.Where(p => p.IsActive).ToList();
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
            PlayerService.BroadcastMessage(
                string.Format(CultureInfo.InvariantCulture, Resources.Command_Me_Message, sender.Name, text),
                new Color(0xc8, 0x64, 0x00));
            Log.Information("*{Name} {Text}", sender.Name, text);
        }
        
        [CommandHandler(nameof(Resources.Command_Roll),
            HelpText = nameof(Resources.Command_Roll_HelpText),
            UsageText = nameof(Resources.Command_Roll_UsageText),
            ResourceType = typeof(Resources))]
        public void Roll(ICommandSender sender) {
            var num = _rand.Next(1, 101);
            PlayerService.BroadcastMessage(
                string.Format(CultureInfo.InvariantCulture, Resources.Command_Roll_Message, sender.Name, num),
                new Color(0xff, 0xf0, 0x14));
            Log.Information("*{Name} rolls a {Num}", sender.Name, num);
        }
        
        [CommandHandler(nameof(Resources.Command_P),
            HelpText = nameof(Resources.Command_P_HelpText),
            UsageText = nameof(Resources.Command_P_UsageText),
            ResourceType = typeof(Resources))]
        public void P(ICommandSender sender, [RestOfInput] string text) {
            var player = sender.Player;
            if (player is null) {
                sender.SendErrorMessage(Resources.Command_Party_NotAPlayer);
                return;
            }

            var team = player.Team;
            if (team == PlayerTeam.None) {
                sender.SendErrorMessage(Resources.Command_Party_NotInTeam);
                return;
            }

            var teamColor = team.Color();
            var teamPlayers = PlayerService.Players.Where(p => p.IsActive && p.Team == team);
            foreach (var teamPlayer in teamPlayers) {
                teamPlayer.SendMessageFrom(player, text, teamColor);
            }
            Log.Information("<{Player} (to {Team} team)> {Text}", player.Name, team, text);
        }
    }
}
