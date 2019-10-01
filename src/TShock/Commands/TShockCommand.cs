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
        private readonly ISet<char> _validShortFlags = new HashSet<char>();
        private readonly ISet<string> _validLongFlags = new HashSet<string>();
        private readonly IDictionary<string, ParameterInfo> _validOptionals = new Dictionary<string, ParameterInfo>();
        private readonly ParameterInfo[] _parameterInfos;
        private readonly object?[] _parameters;

        public string Name { get; }
        public IEnumerable<string> SubNames { get; }
        public object HandlerObject { get; }
        public MethodBase Handler { get; }

        public TShockCommand(ICommandService commandService, CommandHandlerAttribute attribute, object handlerObject,
                             MethodBase handler) {
            Debug.Assert(commandService != null, "commandService != null");
            Debug.Assert(attribute != null, "attribute != null");
            Debug.Assert(handler != null, "handler != null");

            _commandService = commandService;
            Name = attribute.CommandName;
            SubNames = attribute.CommandSubNames;
            HandlerObject = handlerObject;
            Handler = handler;

            // Preprocessing parameters in the constructor allows us to learn the command's flags and optionals.
            void PreprocessParameter(ParameterInfo parameterInfo) {
                var parameterType = parameterInfo.ParameterType;

                // Check for types we don't support, such as by-reference and pointers.
                if (parameterType.IsByRef) {
                    throw new NotSupportedException(
                        string.Format(Resources.CommandCtor_ByRefArgType, parameterType));
                }

                if (parameterType.IsPointer) {
                    throw new NotSupportedException(
                        string.Format(Resources.CommandCtor_PointerArgType, parameterType));
                }

                // If the parameter is a bool and it is marked with FlagAttribute, we'll note it.
                if (parameterType == typeof(bool)) {
                    var attribute = parameterInfo.GetCustomAttribute<FlagAttribute?>();
                    if (attribute != null) {
                        _validShortFlags.Add(attribute.ShortFlag);
                        if (attribute.LongFlag != null) {
                            _validLongFlags.Add(attribute.LongFlag);
                        }
                    }
                }

                // If the parameter is optional, we'll note it. We replace underscores with hyphens here since hyphens
                // aren't valid in C# identifiers.
                if (parameterInfo.IsOptional) {
                    _validOptionals.Add(parameterInfo.Name.Replace('_', '-'), parameterInfo);
                }
            }

            _parameterInfos = Handler.GetParameters();
            _parameters = new object?[_parameterInfos.Length];
            foreach (var parameter in _parameterInfos) {
                PreprocessParameter(parameter);
            }
        }

        public void Invoke(ICommandSender sender, string inputString) {
            if (sender is null) throw new ArgumentNullException(nameof(sender));
            if (inputString is null) throw new ArgumentNullException(nameof(inputString));

            var args = new CommandExecuteEventArgs(this, sender, inputString);
            _commandService.CommandExecute?.Invoke(this, args);
            if (args.IsCanceled()) return;

            var shortFlags = new HashSet<char>();
            var longFlags = new HashSet<string>();
            var optionals = new Dictionary<string, object>();

            object ParseArgument(ref ReadOnlySpan<char> input, ParameterInfo parameterInfo) {
                var parameterType = parameterInfo.ParameterType;
                if (!_commandService.RegisteredParsers.TryGetValue(parameterType, out var parser)) {
                    throw new CommandParseException(
                        string.Format(Resources.CommandParse_UnrecognizedArgType, parameterType));
                }

                var options = parameterInfo.GetCustomAttribute<ParseOptionsAttribute>()?.Options;
                return parser.Parse(ref input, options);
            }

            void ParseShortFlags(ref ReadOnlySpan<char> input, int start) {
                var end = input.ScanFor(char.IsWhiteSpace, start);
                for (var i = start; i < end; ++i) {
                    var c = input[i];
                    if (!_validShortFlags.Contains(c)) {
                        throw new CommandParseException(
                            string.Format(Resources.CommandParse_UnrecognizedShortFlag, c));
                    }

                    shortFlags.Add(c);
                }

                input = input[end..];
            }

            void ParseLongFlag(ref ReadOnlySpan<char> input, int start, int end) {
                var longFlag = input[start..end].ToString();
                if (!_validLongFlags.Contains(longFlag)) {
                    throw new CommandParseException(
                        string.Format(Resources.CommandParse_UnrecognizedLongFlag, longFlag));
                }

                input = input[end..];
                longFlags.Add(longFlag);
            }

            void ParseOptional(ref ReadOnlySpan<char> input, int start, int end) {
                var optional = input[start..end].ToString();
                if (!_validOptionals.TryGetValue(optional, out var parameterInfo)) {
                    throw new CommandParseException(
                        string.Format(Resources.CommandParse_UnrecognizedOptional, optional));
                }

                // Skip over the '='.
                input = input[(end + 1)..];
                optionals[optional] = ParseArgument(ref input, parameterInfo);
            }

            /*
             * Parse all hyphenated arguments:
             * - Short flags are single-character flags and use one hyphen: "-f".
             * - Long flags are string flags and use two hyphens: "--force".
             * - Optionals specify values with two hyphens: "--depth=10".
             */
            void ParseHyphenatedArgs(ref ReadOnlySpan<char> input) {
                while (true) {
                    var start = input.ScanFor(c => !char.IsWhiteSpace(c));
                    if (start >= input.Length || input[start] != '-') {
                        input = input[start..];
                        break;
                    }

                    if (++start >= input.Length) {
                        throw new CommandParseException(Resources.CommandParse_InvalidHyphenatedArg);
                    }

                    if (input[start] == '-') {
                        if (++start >= input.Length) {
                            throw new CommandParseException(Resources.CommandParse_InvalidHyphenatedArg);
                        }

                        var end = input.ScanFor(c => char.IsWhiteSpace(c) || c == '=', start);
                        if (end >= input.Length || input[end] != '=') {
                            ParseLongFlag(ref input, start, end);
                        } else {
                            ParseOptional(ref input, start, end);
                        }
                    } else {
                        ParseShortFlags(ref input, start);
                    }
                }
            }

            /*
             * Parse a parameter:
             * - If the parameter is an ICommandSender, then inject sender.
             * - If the parameter is a bool and is marked with FlagAttribute, then inject the flag.
             * - If the parameter is optional, then inject the optional or else the default value.
             * - Otherwise, we parse the argument directly.
             */
            object? ParseParameter(ParameterInfo parameterInfo, ref ReadOnlySpan<char> input) {
                var parameterType = parameterInfo.ParameterType;

                if (parameterType == typeof(ICommandSender)) return sender;

                if (parameterType == typeof(bool)) {
                    var attribute = parameterInfo.GetCustomAttribute<FlagAttribute?>();
                    if (attribute != null) {
                        return shortFlags.Contains(attribute.ShortFlag) ||
                               attribute.LongFlag != null && longFlags.Contains(attribute.LongFlag);
                    }
                }

                if (parameterInfo.IsOptional) {
                    var optional = parameterInfo.Name.Replace('_', '-');
                    return optionals.TryGetValue(optional, out var value) ? value : parameterInfo.DefaultValue;
                }

                return ParseArgument(ref input, parameterInfo);
            }

            var input = inputString.AsSpan();
            if (_validShortFlags.Count > 0 || _validLongFlags.Count > 0 || _validOptionals.Count > 0) {
                ParseHyphenatedArgs(ref input);
            }

            for (var i = 0; i < _parameters.Length; ++i) {
                _parameters[i] = ParseParameter(_parameterInfos[i], ref input);
            }

            var end = input.ScanFor(c => !char.IsWhiteSpace(c));
            if (end < input.Length) throw new CommandParseException(Resources.CommandParse_TooManyArgs);

            try {
                Handler.Invoke(HandlerObject, _parameters);
            } catch (TargetInvocationException ex) {
                sender.Log.Error(ex.InnerException, Resources.CommandInvoke_Exception);
                throw new CommandException(Resources.CommandInvoke_Exception, ex.InnerException);
            }
        }
    }
}
