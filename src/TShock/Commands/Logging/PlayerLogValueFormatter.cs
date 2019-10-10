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

using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using Serilog.Data;
using Serilog.Events;
using Unit = System.ValueTuple;

namespace TShock.Commands.Logging {
    internal sealed class PlayerLogValueFormatter : LogEventPropertyValueVisitor<Unit, string> {
        [Pure]
        public string Format(LogEventPropertyValue value) => Visit(default, value);

        [Pure]
        protected override string VisitScalarValue(Unit _, ScalarValue scalar) =>
            string.Format(CultureInfo.InvariantCulture, scalar.Value switch {
                null => "[c/569cd6:null]",
                string _ => "[c/d69d85:{0}]",
                bool _ => "[c/569cd6:{0}]",
                char _ => "[c/d69d85:'{0}']",
                var n when n.GetType().IsPrimitive || n is decimal => "[c/b5cea8:{0}]",
                _ => "[c/86c691:{0}]"
            }, scalar.Value);

        [Pure]
        protected override string VisitSequenceValue(Unit _, SequenceValue sequence) =>
            $"[{string.Join(", ", sequence.Elements.Select(e => Visit(_, e)))}]";

        [Pure]
        protected override string VisitStructureValue(Unit _, StructureValue structure) =>
            FormatTypeTag(structure.TypeTag) +
            $"{{{string.Join(", ", structure.Properties.Select(FormatStructureElement))}}}";

        [Pure]
        protected override string VisitDictionaryValue(Unit _, DictionaryValue dictionary) =>
            $"{{{string.Join(", ", dictionary.Elements.Select(FormatDictionaryElement))}}}";

        [Pure]
        private string FormatTypeTag(string? typeTag) =>
            typeTag is null ? string.Empty : $"[c/4ec9b0:{typeTag}] ";

        [Pure]
        private string FormatStructureElement(LogEventProperty p) =>
            $"{p.Name}={Visit(default, p.Value)}";

        [Pure]
        private string FormatDictionaryElement(KeyValuePair<ScalarValue, LogEventPropertyValue> kvp) =>
            $"[{Visit(default, kvp.Key)}]={Visit(default, kvp.Value)}";
    }
}
