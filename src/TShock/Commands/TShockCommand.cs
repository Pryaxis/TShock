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
using Orion.Events.Extensions;
using TShock.Commands.Extensions;
using TShock.Commands.Parsers;
using TShock.Events.Commands;
using TShock.Properties;

namespace TShock.Commands {
    internal class TShockCommand : ICommand {
        private static readonly ISet<char> EmptyShortFlags = new HashSet<char>();

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

        public void Invoke(ICommandSender sender, string inputString) {
            if (sender is null) throw new ArgumentNullException(nameof(sender));
            if (inputString is null) throw new ArgumentNullException(nameof(inputString));

            var args = new CommandExecuteEventArgs(this, sender, inputString);
            _commandService.CommandExecute?.Invoke(this, args);
            if (args.IsCanceled()) return;

            // Pass 1: Scan through the parameters and learn flags and optionals.
            var parameters = Handler.GetParameters();
            var validShortFlags = new HashSet<char>();
            var validLongFlags = new HashSet<string>();
            var validOptionals = new HashSet<string>();

            void PreprocessParameter(ParameterInfo parameter) {
                // Check for things we don't support: by-reference types.
                if (parameter.ParameterType.IsByRef) throw new ParseException(Resources.CommandParse_ArgIsByReference);

                var parameterType = parameter.ParameterType;

                // If the parameter is a bool and is marked with a [Flag], we'll take note of that.
                if (parameterType == typeof(bool)) {
                    var attribute = parameter.GetCustomAttribute<FlagAttribute>();
                    if (attribute != null) {
                        validShortFlags.Add(attribute.ShortFlag);
                        validLongFlags.Add(attribute.LongFlag);
                    }
                }

                // If the parameter is optional, we'll take note of that.
                if (parameter.IsOptional) {
                    validOptionals.Add(parameter.Name);
                }
            }

            foreach (var parameter in parameters) {
                PreprocessParameter(parameter);
            }


            // Pass 2, part 1: Parse flags and optionals.
            var input = inputString.AsSpan();
            var shortFlags = ParseShortFlags(ref input, validShortFlags);

            // Pass 2, part 2: Parse parameters.
            var parsers = _commandService.RegisteredParsers;
            var handlerArgs = new List<object>();

            void CoerceParameter(ParameterInfo parameter, ref ReadOnlySpan<char> input) {
                var parameterType = parameter.ParameterType;

                // Special case: parameter is an ICommandSender, in which case we inject sender.
                if (parameterType == typeof(ICommandSender)) {
                    handlerArgs.Add(sender);
                    return;
                }

                // Special case: parameter is a bool and is marked with a [Flag], in which case we look up shortFlags
                // and longFlags and inject that.
                if (parameterType == typeof(bool)) {
                    var attribute = parameter.GetCustomAttribute<FlagAttribute>();
                    if (attribute != null) {
                        handlerArgs.Add(shortFlags.Contains(attribute.ShortFlag));
                        return;
                    }
                }

                // If we can directly parse the parameter type, then do so.
                if (parsers.TryGetValue(parameterType, out var parser)) {
                    var options = parameter.GetCustomAttribute<ParseOptionsAttribute>()?.Options;
                    handlerArgs.Add(parser.Parse(input, out input, options));
                }
            }

            foreach (var parameter in Handler.GetParameters()) {
                CoerceParameter(parameter, ref input);
            }

            try {
                Handler.Invoke(HandlerObject, handlerArgs.ToArray());
            } catch (Exception ex) {
                sender.Log.Error(ex, Resources.CommandInvoke_Exception);
                throw new CommandException(Resources.CommandInvoke_Exception, ex);
            }
        }

        private static ISet<char> ParseShortFlags(ref ReadOnlySpan<char> input, ISet<char> validShortFlags) {
            // Quick return if there are no valid short flags.
            if (validShortFlags.Count == 0) return EmptyShortFlags;

            var start = input.ScanFor(c => !char.IsWhiteSpace(c));
            var end = input.ScanFor(char.IsWhiteSpace, start);
            if (start == end || input[start] != '-')
                return EmptyShortFlags;

            ++start;

            // Make sure we're not treating a long flag or an optional as a set of short flags.
            if (start == end || input[start] == '-') return EmptyShortFlags;

            var shortFlags = new HashSet<char>();
            foreach (var c in input[start..end]) {
                if (!validShortFlags.Contains(c)) {
                    throw new ParseException(string.Format(Resources.CommandParse_BadShortFlag, c));
                }

                shortFlags.Add(c);
            }

            input = input[end..];
            return shortFlags;
        }
    }
}
