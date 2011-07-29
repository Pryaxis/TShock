using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TShockAPI.DB;

namespace TShockAPI
{
    public class DBTools
    {
        internal static IDbConnection database;

        /// <summary>
        /// Creates a Table, within the current open DB
        /// </summary>
        /// <param name="name">Name of the Table</param>
        /// <param name="columns">The list of columns that the Table will have</param>
        /// <param name="IfNotExists">Only try create Table if it does not exist</param>
        public static void CreateTable(string name, List<Column> columns, bool IfNotExists =true)
        {
            //Build up Creation string :)
            StringBuilder sb = new StringBuilder();
            sb.Append("CREATE TABLE "); 

            if (IfNotExists)
                sb.Append("IF NOT EXISTS ");

            if (TShock.Config.StorageType.ToLower() == "sqlite")
                sb.Append("'" + name + "' (");
            else if (TShock.Config.StorageType.ToLower() == "mysql")
                sb.Append(name + " (");

            int count = 0;

            foreach (Column column in columns)
            {
                count++;
                if (column.Type.ToLower() == "int")
                {
                    if (TShock.Config.StorageType.ToLower() == "sqlite")
                        sb.Append(column.Name + " NUMERIC ");
                    else if (TShock.Config.StorageType.ToLower() == "mysql")
                        sb.Append(column.Name + " INT(255) ");
                }
                else if (column.Type.ToLower() == "string")
                {
                    if (TShock.Config.StorageType.ToLower() == "sqlite")
                        sb.Append(column.Name + " TEXT ");
                    else if (TShock.Config.StorageType.ToLower() == "mysql")
                        sb.Append(column.Name + " VARCHAR(255) ");
                }

                if (column.Unique)
                    sb.Append("UNIQUE");

                if (columns.Count == count)
                    sb.Append(")");
                else
                    sb.Append(", ");
            }

            database.Query(sb.ToString());
        }

        /// <summary>
        /// Inserts a list of values into a Table, if conditions are met
        /// </summary>
        /// <param name="tablename">Name of the Table</param>
        /// <param name="Ignore">Ignore insert if feild is unique and there is already a exact entry</param>
        /// <param name="Values">The list of values to enter into the table</param>
        /// <param name="WhereStatements">The list of where statements that must be met, can be an empty list</param>
        public static int InsertTable(string tablename, bool Ignore, List<ColumnData> Values, List<ColumnData> WhereStatements)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT ");

            if (Ignore)
            {
                if (TShock.Config.StorageType.ToLower() == "sqlite")
                    sb.Append("OR IGNORE ");
                else if (TShock.Config.StorageType.ToLower() == "mysql")
                    sb.Append("IGNORE ");
            }

            if (TShock.Config.StorageType.ToLower() == "sqlite")
                sb.Append("INTO '" + tablename + "' (");
            else if (TShock.Config.StorageType.ToLower() == "mysql")
                sb.Append("INTO " + tablename + " ");

            using (var com = database.CreateCommand())
            {
                //Values
                if (TShock.Config.StorageType.ToLower() == "sqlite")
                {
                    int count = 0;

                    foreach (ColumnData columnname in Values)
                    {
                        count++;
                        if (Values.Count != count)
                            sb.Append(columnname.Name + ", ");
                        else
                            sb.Append(columnname.Name + ") ");
                    }

                    sb.Append("VALUES (");
                    count = 0;

                    foreach (ColumnData columnname in Values)
                    {
                        count++;
                        if (Values.Count != count)
                        {
                            sb.Append("@" + columnname.Name.ToLower() + ", ");
                            com.AddParameter("@" + columnname.Name.ToLower(), columnname.Value);
                        }
                        else
                        {
                            sb.Append("@" + columnname.Name.ToLower() + ") ");
                            com.AddParameter("@" + columnname.Name.ToLower(), columnname.Value);
                        }
                    }
                }
                else if (TShock.Config.StorageType.ToLower() == "mysql")
                {
                    sb.Append("SET ");
                    int count = 0;

                    foreach (ColumnData columnname in Values)
                    {
                        count++;
                        if (Values.Count != count)
                        {
                            sb.Append("@" + columnname.Name.ToLower() + "=" + columnname.Value + ", ");
                            com.AddParameter("@" + columnname.Name.ToLower(), columnname.Value);
                        }
                        else
                        {
                            sb.Append("@" + columnname.Name.ToLower() + "=" + columnname.Value + ") ");
                            com.AddParameter("@" + columnname.Name.ToLower(), columnname.Value);
                        }
                    }
                }

                //Where Statement (if any)
                if (WhereStatements.Count > 0)
                {
                    sb.Append("WHERE ");
                    int count = 0;

                    foreach (ColumnData columnname in WhereStatements)
                    {
                        count++;
                        if (Values.Count != count)
                        {
                            sb.Append("@" + columnname.Name.ToLower() + "-where" + "=" + columnname.Value + " AND ");
                            com.AddParameter("@" + columnname.Name.ToLower() + "-where", columnname.Value);
                        }
                        else
                        {
                            sb.Append("@" + columnname.Name.ToLower() + "-where" + "=" + columnname.Value + ";");
                            com.AddParameter("@" + columnname.Name.ToLower() + "-where", columnname.Value);
                        }
                    }
                }

                com.CommandText = sb.ToString();

                using (var reader = com.ExecuteReader())
                    return reader.RecordsAffected;
            }
        }

        /// <summary>
        /// Returns a list of values from a given Table, where conditions are met
        /// </summary>
        /// <param name="tablename">Name of the Table</param>
        /// <param name="getcolumn">The name of the column you are getting the values from</param>
        /// <param name="WhereStatements">The list of where statements that must be met, can be an empty list</param>
        public static List<object> ReadTable(string tablename, string getcolumn, List<ColumnData> WhereStatements)
        {
            StringBuilder sb = new StringBuilder();
            List<object> ReturnedValues = new List<object>();

            sb.Append("SELECT * FROM " + tablename + " ");

            using (var com = database.CreateCommand())
            {
                //Where Statement (if any)
                if (WhereStatements.Count > 0)
                {
                    sb.Append("WHERE ");
                    int count = 0;

                    foreach (ColumnData columnname in WhereStatements)
                    {
                        count++;
                        if (WhereStatements.Count != count)
                        {
                            sb.Append(columnname.Name + " =" + columnname.Value + " AND ");
                        }
                        else
                        {
                            sb.Append(columnname.Name + " =" + columnname.Value);
                        }
                    }
                }

                com.CommandText = sb.ToString();

                using (var reader = com.ExecuteReader())
                {
                    while (reader.Read())
                        ReturnedValues.Add(reader.Get<object>(getcolumn));
                }
            }
            return ReturnedValues;
        }

        /// <summary>
        /// Sets values in a Table, where statements are met
        /// </summary>
        /// <param name="tablename">Name of the Table</param>
        /// <param name="setcolumn">The column data you are setting</param>
        /// <param name="WhereStatements">The list of where statements that must be met, can be an empty list</param>
        public static int SetTable(string tablename, ColumnData setcolumn, List<ColumnData> WhereStatements)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("UPDATE " + tablename + " SET " + setcolumn.Name + "=@setcolumn ");

            using (var com = database.CreateCommand())
            {
                //Where Statement (if any)
                if (WhereStatements.Count > 0)
                {
                    sb.Append("WHERE ");
                    int count = 0;

                    foreach (ColumnData columnname in WhereStatements)
                    {
                        count++;
                        if (WhereStatements.Count != count)
                        {
                            sb.Append(columnname.Name + " =" + columnname.Value + " AND ");
                        }
                        else
                        {
                            sb.Append(columnname.Name + " =" + columnname.Value);
                        }
                    }
                }

                com.CommandText = sb.ToString();
                com.AddParameter("@setcolumn", setcolumn.Value);

                using (var reader = com.ExecuteReader())
                    return reader.RecordsAffected;
            }
        }
    }

    public class Column
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Unique { get; set; }
        public string Parameters { get; set; }

        /// <summary>
        /// The class for creating a new column type
        /// </summary>
        /// <param name="name">Name of the column</param>
        /// <param name="unique">Whether there can be more than one exact value in the column</param>
        /// <param name="type">The type of column, currently the api only supports "string" or "int"</param>
        /// <param name="parameters">Extra SQL parameters given, can cause errors cross different SQL (SQLite and MySql)</param>
        public Column(string name, bool unique, string type, string parameters = "")
        {
            Name = name;
            Type = type;
            Unique = unique;
            Parameters = parameters;
        }

        public Column()
        {
            Name = string.Empty;
            Type = string.Empty;
            Unique = false;
            Parameters = string.Empty;
        }
    }

    public class ColumnData
    {
        public string Name { get; set; }
        public object Value { get; set; }

        /// <summary>
        /// The class for testing, inserting or setting column data
        /// </summary>
        /// <param name="name">Column Name</param>
        /// <param name="value">Column Value</param>
        public ColumnData(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public ColumnData()
        {
            Name = string.Empty;
            Value = string.Empty;
        }
    }
}
