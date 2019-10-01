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
        private static readonly ISet<string> EmptyLongFlags = new HashSet<string>();
        private static readonly IDictionary<string, object> EmptyOptionals = new Dictionary<string, object>();

        private readonly ICommandService _commandService;
        private readonly CommandHandlerAttribute _attribute;
        private readonly ISet<char> validShortFlags = new HashSet<char>();
        private readonly ISet<string> validLongFlags = new HashSet<string>();
        private readonly IDictionary<string, ParameterInfo> validOptionals = new Dictionary<string, ParameterInfo>();

        public string Name => _attribute.CommandName;
        public IEnumerable<string> SubNames => _attribute.CommandSubNames;
        public object HandlerObject { get; }
        public MethodBase Handler { get; }

        public TShockCommand(ICommandService commandService, CommandHandlerAttribute attribute, object handlerObject,
                             MethodBase handler) {
            Debug.Assert(commandService != null, "commandService != null");
            Debug.Assert(attribute != null, "attribute != null");
            Debug.Assert(handler != null, "handler != null");

            _commandService = commandService;
            _attribute = attribute;
            HandlerObject = handlerObject;
            Handler = handler;

            void PreprocessParameter(ParameterInfo parameterInfo) {
                var parameterType = parameterInfo.ParameterType;

                // Check for types we don't support, such as by-reference and pointers.
                if (parameterType.IsByRef) {
                    throw new NotSupportedException(
                        string.Format(Resources.CommandCtor_ArgIsByReference, parameterInfo));
                }

                if (parameterType.IsPointer) {
                    throw new NotSupportedException(string.Format(Resources.CommandCtor_ArgIsPointer, parameterInfo));
                }

                // If the parameter is a bool, then it should be marked with FlagAttribute and we'll note it.
                if (parameterType == typeof(bool)) {
                    var attribute = parameterInfo.GetCustomAttribute<FlagAttribute?>();
                    if (attribute != null) {
                        validShortFlags.Add(attribute.ShortFlag);
                        if (attribute.LongFlag != null) {
                            validLongFlags.Add(attribute.LongFlag);
                        }
                    }
                }

                // If the parameter is optional, we'll take note of that. We replace underscores with hyphens here
                // because hyphens are not valid in C# identifiers.
                if (parameterInfo.IsOptional) {
                    validOptionals.Add(parameterInfo.Name.Replace('_', '-'), parameterInfo);
                }
            }

            // Scan through the parameters and learn flags and optionals.
            foreach (var parameter in Handler.GetParameters()) {
                PreprocessParameter(parameter);
            }
        }

        public void Invoke(ICommandSender sender, string inputString) {
            if (sender is null) throw new ArgumentNullException(nameof(sender));
            if (inputString is null) throw new ArgumentNullException(nameof(inputString));

            var args = new CommandExecuteEventArgs(this, sender, inputString);
            _commandService.CommandExecute?.Invoke(this, args);
            if (args.IsCanceled()) return;

            var parsers = _commandService.RegisteredParsers;

            ISet<char> ParseShortFlags(ref ReadOnlySpan<char> input) {
                // Quick return if there are no valid short flags.
                if (validShortFlags.Count == 0) return EmptyShortFlags;

                var start = input.ScanFor(c => !char.IsWhiteSpace(c));
                if (start >= input.Length || input[start++] != '-') return EmptyShortFlags;
                if (start >= input.Length || input[start] == '-') return EmptyShortFlags;
                
                var end = input.ScanFor(char.IsWhiteSpace, start);
                var shortFlags = new HashSet<char>();
                for (var i = start; i < end; ++i) {
                    var c = input[i];
                    if (!validShortFlags.Contains(c)) {
                        throw new ParseException(string.Format(Resources.CommandParse_BadShortFlag, c));
                    }

                    shortFlags.Add(c);
                }

                input = input[end..];
                return shortFlags;
            }

            (ISet<string> longFlags, IDictionary<string, object> optionals) ParseLongFlagsAndOptionals(
                ref ReadOnlySpan<char> input) {
                // Quick return if there are no valid long flags and optionals.
                if (validLongFlags.Count == 0 && validOptionals.Count == 0) return (EmptyLongFlags, EmptyOptionals);

                var longFlags = new HashSet<string>();
                var optionals = new Dictionary<string, object>();
                while (true) {
                    var start = input.ScanFor(c => !char.IsWhiteSpace(c));
                    if (start >= input.Length || input[start++] != '-') break;
                    if (start >= input.Length || input[start++] != '-') break;

                    var end = input.ScanFor(c => char.IsWhiteSpace(c) || c == '=', start);
                    var isLongFlag = end >= input.Length || input[end] != '=';
                    if (isLongFlag) {
                        var longFlag = input[start..end].ToString();
                        if (!validLongFlags.Contains(longFlag)) {
                            throw new ParseException(string.Format(Resources.CommandParse_BadLongFlag, longFlag));
                        }

                        longFlags.Add(longFlag);
                        input = input[end..];
                    } else {
                        var optional = input[start..end].ToString();
                        if (!validOptionals.TryGetValue(optional, out var parameter)) {
                            throw new ParseException(string.Format(Resources.CommandParse_BadOptional, optional));
                        }

                        var parameterType = parameter.ParameterType;
                        if (!parsers.TryGetValue(parameterType, out var parser)) {
                            throw new ParseException(
                                string.Format(Resources.CommandParse_NoParserFound, parameterType));
                        }
                        
                        ++end;
                        input = input[end..];
                        var options = parameter.GetCustomAttribute<ParseOptionsAttribute>()?.Options;
                        optionals[optional] = parser.Parse(ref input, options);
                    }
                }

                return (longFlags, optionals);
            }

            var input = inputString.AsSpan();
            var shortFlags = ParseShortFlags(ref input);
            var (longFlags, optionals) = ParseLongFlagsAndOptionals(ref input);

            object? ProcessParameter(ParameterInfo parameterInfo, ref ReadOnlySpan<char> input) {
                var parameterType = parameterInfo.ParameterType;

                // Special case: parameter is an ICommandSender, in which case we inject sender.
                if (parameterType == typeof(ICommandSender)) return sender;

                // Special case: parameter is a bool and is marked with FlagAttribute, in which case we look up
                // shortFlags and longFlags and inject that.
                if (parameterType == typeof(bool)) {
                    var attribute = parameterInfo.GetCustomAttribute<FlagAttribute?>();
                    if (attribute != null) {
                        return shortFlags.Contains(attribute.ShortFlag) ||
                               attribute.LongFlag != null && longFlags.Contains(attribute.LongFlag);
                    }
                }

                // Special case: parameter is optional, in which case we look up optionals and try injecting that. If
                // that fails, then we just inject the default value.
                if (parameterInfo.IsOptional) {
                    var optional = parameterInfo.Name.Replace('_', '-');
                    return optionals.TryGetValue(optional, out var value) ? value : parameterInfo.DefaultValue;
                }

                // If we can directly parse the parameter type, then do so.
                if (parsers.TryGetValue(parameterType, out var parser)) {
                    var options = parameterInfo.GetCustomAttribute<ParseOptionsAttribute?>()?.Options;
                    return parser.Parse(ref input, options);
                }

                // Otherwise, it's impossible to parse.
                throw new ParseException(string.Format(Resources.CommandParse_NoParserFound, parameterType));
            }

            var parameterInfos = Handler.GetParameters();
            var parameters = new object?[parameterInfos.Length];
            for (var i = 0; i < parameters.Length; ++i) {
                parameters[i] = ProcessParameter(parameterInfos[i], ref input);
            }

            try {
                Handler.Invoke(HandlerObject, parameters);
            } catch (TargetInvocationException ex) {
                sender.Log.Error(ex.InnerException, Resources.CommandInvoke_Exception);
                throw new CommandException(Resources.CommandInvoke_Exception, ex.InnerException);
            }
        }
    }
}
