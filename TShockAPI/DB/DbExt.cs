using System;
using System.Collections.Generic;
using System.Data;

namespace TShockAPI.DB
{
    public static class DbExt
    {    

        /// <summary>
        /// Executes a query on a database.
        /// </summary>
        /// <param name="db">Database to query</param>
        /// <param name="query">Query string with parameters as @0, @1, etc.</param>
        /// <param name="args">Parameters to be put in the query</param>
        /// <returns>Rows affected by query</returns>
        public static int Query(this IDbConnection db, string query, params object[] args)
        {
            using (var com = db.CreateCommand())
            {
                com.CommandText = query;
                for (int i = 0; i < args.Length; i++)
                    com.AddParameter("@" + i, args[i]);

                return com.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// Executes a query on a database.
        /// </summary>
        /// <param name="db">Database to query</param>
        /// <param name="query">Query string with parameters as @0, @1, etc.</param>
        /// <param name="args">Parameters to be put in the query</param>
        /// <returns>Query result as IDataReader</returns>
        public static IDataReader QueryReader(this IDbConnection db, string query, params object[] args)
        {
            using (var com = db.CreateCommand())
            {
                com.CommandText = query;
                for (int i = 0; i < args.Length; i++)
                    com.AddParameter("@" + i, args[i]);

                return com.ExecuteReader();
            }
        }

        public static IDbDataParameter AddParameter(this IDbCommand command, string name, object data)
        {
            var parm = command.CreateParameter();
            parm.ParameterName = name;
            parm.Value = data;
            command.Parameters.Add(parm);
            return parm;
        }

        static Dictionary<Type, Func<IDataReader, int, object>> ReadFuncs = new Dictionary<Type, Func<IDataReader, int, object>>
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
