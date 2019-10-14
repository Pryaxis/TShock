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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Serilog.Data;
using Serilog.Events;
using TShock.Logging.Themes;
using Unit = System.ValueTuple;

namespace TShock.Logging {
    [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters",
        Justification = "symbols should not be localized")]
    [SuppressMessage("Design", "CA1062:Validate arguments of public methods",
        Justification = "validation has already occurred")]
    internal sealed class PlayerLogValueVisitor : LogEventPropertyValueVisitor<TextWriter, Unit> {
        private readonly PlayerLogTheme _theme;
        private readonly IFormatProvider? _formatProvider;

        public PlayerLogValueVisitor(PlayerLogTheme theme, IFormatProvider? formatProvider = null) {
            Debug.Assert(theme != null, "theme should not be null");

            _theme = theme ?? throw new ArgumentNullException(nameof(theme));
            _formatProvider = formatProvider;
        }

        public Unit Format(LogEventPropertyValue value, TextWriter output) {
            Debug.Assert(value != null, "value should not be null");
            Debug.Assert(output != null, "output should not be null");

            return Visit(output, value);
        }

        protected override Unit VisitScalarValue(TextWriter output, ScalarValue scalar) {
            var value = scalar.Value;
            var text = value is IFormattable f ? f.ToString(null, _formatProvider) : value?.ToString() ?? "null";
            var style = scalar.Value switch {
                null => PlayerLogThemeStyle.Null,
                string _ => PlayerLogThemeStyle.String,
                bool _ => PlayerLogThemeStyle.Boolean,
                char _ => PlayerLogThemeStyle.Character,
                var n when n.GetType().IsPrimitive || n is decimal => PlayerLogThemeStyle.Number,
                _ => PlayerLogThemeStyle.Scalar
            };
            output.Write(_theme.Stylize(text, style));
            return default;
        }

        protected override Unit VisitSequenceValue(TextWriter output, SequenceValue sequence) {
            var includeSeparator = false;
            output.Write(_theme.Stylize("[", PlayerLogThemeStyle.Text));
            foreach (var element in sequence.Elements) {
                if (includeSeparator) {
                    output.Write(_theme.Stylize(", ", PlayerLogThemeStyle.Separator));
                }

                Visit(output, element);
                includeSeparator = true;
            }

            output.Write(_theme.Stylize("]", PlayerLogThemeStyle.Text));
            return default;
        }

        protected override Unit VisitStructureValue(TextWriter output, StructureValue structure) {
            var typeTag = structure.TypeTag;
            if (typeTag != null) {
                output.Write(_theme.Stylize(typeTag, PlayerLogThemeStyle.Type));
            }

            var includeSeparator = false;
            output.Write(_theme.Stylize("{", PlayerLogThemeStyle.Text));
            foreach (var property in structure.Properties) {
                if (includeSeparator) {
                    output.Write(_theme.Stylize(", ", PlayerLogThemeStyle.Separator));
                }

                output.Write(_theme.Stylize(property.Name, PlayerLogThemeStyle.Identifier));
                output.Write(_theme.Stylize("=", PlayerLogThemeStyle.Separator));
                Visit(output, property.Value);
                includeSeparator = true;
            }

            output.Write(_theme.Stylize("}", PlayerLogThemeStyle.Text));
            return default;
        }

        protected override Unit VisitDictionaryValue(TextWriter output, DictionaryValue dictionary) {
            var includeSeparator = false;
            output.Write(_theme.Stylize("{", PlayerLogThemeStyle.Text));
            foreach (var kvp in dictionary.Elements) {
                if (includeSeparator) {
                    output.Write(_theme.Stylize(", ", PlayerLogThemeStyle.Separator));
                }

                output.Write(_theme.Stylize("[", PlayerLogThemeStyle.Text));
                Visit(output, kvp.Key);
                output.Write(_theme.Stylize("]", PlayerLogThemeStyle.Text));
                output.Write(_theme.Stylize("=", PlayerLogThemeStyle.Separator));
                Visit(output, kvp.Value);
                includeSeparator = true;
            }

            output.Write(_theme.Stylize("}", PlayerLogThemeStyle.Text));
            return default;
        }
    }
}
