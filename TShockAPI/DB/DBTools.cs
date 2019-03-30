/*
TShock, a server mod for Terraria
Copyright (C) 2011-2019 Pryaxis & TShock Contributors

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

﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TShockAPI.DB;

namespace TShockAPI.DB
{
    public class DBTools
    {
        public static IDbConnection database;

        public static void CreateTable(string name, bool IfNotExists, List<Column> columns)
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
                {                    
                    if (columns.Count != count)
                    {
                        sb.Append("UNIQUE, ");
                    }
                    else
                        sb.Append("UNIQUE) ");
                }
                if (columns.Count == count)
                    sb.Append(")");
            }

            using (var com = database.CreateCommand())
            {
                com.CommandText = sb.ToString();
                com.ExecuteNonQuery();
            }
        }

        public static void InsertTable(string tablename, bool Ignore, List<ColumnData> Values, List<ColumnData> WhereAndStatements)
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
                            sb.Append("@" + columnname.Name + ", ");
                            com.AddParameter("@" + columnname.Name.ToLower(), columnname.Value);
                        }
                        else
                        {
                            sb.Append("@" + columnname.Name + ") ");
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
                            sb.Append("@" + columnname.Name + "=" + columnname.Value + ", ");
                            com.AddParameter("@" + columnname.Name.ToLower(), columnname.Value);
                        }
                        else
                        {
                            sb.Append("@" + columnname.Name + "=" + columnname.Value + ") ");
                            com.AddParameter("@" + columnname.Name.ToLower(), columnname.Value);
                        }
                    }
                }

                //Where Statement (if any)
                if (WhereAndStatements.Count > 0)
                {
                    sb.Append("WHERE ");
                    int count = 0;

                    foreach (ColumnData columnname in WhereAndStatements)
                    {
                        count++;
                        if (Values.Count != count)
                        {
                            sb.Append("@" + columnname.Name + "=" + columnname.Value + "-where" + " AND ");
                            com.AddParameter("@" + columnname.Name.ToLower() + "-where", columnname.Value);
                        }
                        else
                        {
                            sb.Append("@" + columnname.Name + "=" + columnname.Value + "-where" + ";");
                            com.AddParameter("@" + columnname.Name.ToLower() + "-where", columnname.Value);
                        }
                    }
                }
            }
        }
    }

    public class Column
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Unique { get; set; }
        public string Parameters { get; set; }

        public Column(string name, bool unique, string type, string parameters)
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
