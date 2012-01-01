/*
TShock, a server mod for Terraria
Copyright (C) 2011 The TShock Team

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace TShockDBEditor
{
    public static class DbExt
    {
        public static IDbDataParameter AddParameter(this IDbCommand command, string name, object data)
        {
            var parm = command.CreateParameter();
            parm.ParameterName = name;
            parm.Value = data;
            command.Parameters.Add(parm);
            return parm;
        }

        static Dictionary<Type, Func<IDataReader, int, object>> ReadFuncs = new Dictionary<Type, Func<IDataReader, int, object>>()
        {
            {typeof(bool), (s, i) => s.GetBoolean(i)},
            {typeof(byte), (s, i) => s.GetByte(i)},
            {typeof(Int16), (s, i) => s.GetInt16(i)},
            {typeof(Int32), (s, i) => s.GetInt32(i)},
            {typeof(Int64), (s, i) => s.GetInt64(i)},
            {typeof(string), (s, i) => s.GetString(i)},
            {typeof(decimal), (s, i) => s.GetDecimal(i)},
            {typeof(float), (s, i) => s.GetFloat(i)},
            {typeof(double), (s, i) => s.GetDouble(i)},
        };

        public static T Get<T>(this IDataReader reader, string column)
        {
            return reader.Get<T>(reader.GetOrdinal(column));
        }

        public static T Get<T>(this IDataReader reader, int column)
        {
            if (ReadFuncs.ContainsKey(typeof(T)))
                return (T)ReadFuncs[typeof(T)](reader, column);

            throw new NotImplementedException();
        }
    }
}
