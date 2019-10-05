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

using System.Globalization;
using System.Linq;
using Serilog.Data;
using Serilog.Events;
using Unit = System.ValueTuple;

namespace TShock.Commands.Logging {
    internal sealed class PlayerLogValueFormatter : LogEventPropertyValueVisitor<Unit, string> {
        private const string NullFormat = "[c/33cc99:null]";
        private const string StringFormat = "[c/d69d85:\"{0}\"]";
        private const string BooleanFormat = "[c/33cc99:{0}]";
        private const string CharFormat = "[c/d69d85:'{0}']";
        private const string NumberFormat = "[c/b5cea8:{0}]";
        private const string ScalarFormat = "[c/86c691:{0}]";
        private const string TypeTagFormat = "[c/4ec9b0:{0}] ";

        public string Format(LogEventPropertyValue value) => Visit(default, value);

        protected override string VisitScalarValue(Unit _, ScalarValue scalar) =>
            string.Format(CultureInfo.InvariantCulture, scalar.Value switch {
                null => NullFormat,
                string _ => StringFormat,
                bool _ => BooleanFormat,
                char _ => CharFormat,
                var n when n.GetType().IsPrimitive || n is decimal => NumberFormat,
                _ => ScalarFormat
            }, scalar.Value);

        protected override string VisitSequenceValue(Unit _, SequenceValue sequence) =>
            $"[{string.Join(", ", sequence.Elements.Select(e => Visit(_, e)))}]";

        protected override string VisitStructureValue(Unit _, StructureValue structure) =>
            $"{(structure.TypeTag != null ? string.Format(CultureInfo.InvariantCulture, TypeTagFormat, structure.TypeTag) : "")}" +
            $"{{{string.Join(", ", structure.Properties.Select(p => $"{p.Name}={Visit(_, p.Value)}"))}}}";

        protected override string VisitDictionaryValue(Unit _, DictionaryValue dictionary) =>
            $"{{{string.Join(", ", dictionary.Elements.Select(e => $"[{Visit(_, e.Key)}]={Visit(_, e.Value)}"))}}}";
    }
}
