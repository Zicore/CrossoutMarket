using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using ZicoreConnector.Zicore.Connector.Extensions;

namespace ZicoreConnector.Zicore.Connector.Base
{
    public enum ConnectionType
    {
        None = -1,
        MySql = 0,
        MicrosoftSql = 1,
    }

    /// <summary>
    /// SqlConnector is a highly optimized Connector for different SQL Datasources.
    /// It utilizes the underliying DbConnection and DbCommand for Data Accessing.
    /// </summary>
    public class SqlConnector
    {
        public DbConnection CreateConnection()
        {
            switch (ConnectionType)
            {
                case ConnectionType.MySql:
                    return new MySqlConnection(ConnectionString);
                case ConnectionType.MicrosoftSql:
                    return new SqlConnection(ConnectionString);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public ConnectionType ConnectionType { get; set; }

        public static String EsacepeInput(String str)
        {
            return Regex.Replace(str, @"[^A-Za-z0-9%&$§!\[\]{}=()/&@\._\-+]+", "");
        }

        public static String ToMySqlDateTimeStamp(DateTime dt)
        {
            String stamp = dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            return $"'{stamp}'";
        }

        private static object m_lock = new object();

        public SqlConnector(ConnectionType connectionType)
        {
            this.ConnectionType = connectionType;
        }

        private ConnectorDescription ConnectorDescription { get; set; }
        private String ConnectionString { get; set; }

        public void Open(ConnectorDescription c)
        {
            ConnectorDescription = c;
            ConnectionString = ConnectorDescription.CreateConnectionString(ConnectionType);
            DbConnection connection = CreateConnection();
            connection.Open(); // Test
            connection.Close();
        }

        public DbConnection Connect()
        {
            DbConnection connection = CreateConnection();
            connection.Open();
            return connection;
        }

        public QueryResult ExecuteSQL(String sql, List<Parameter> parameter = null)
        {
            QueryResult result = QueryResult.Default;
            try
            {
                DbConnection connection = CreateConnection();
                connection.Open();


                DbCommand command = connection.CreateCommand();
                command.CommandText = sql;

                if (parameter != null)
                {
                    for (int i = 0; i < parameter.Count; i++)
                    {
                        var p = parameter[i];
                        command.AddParameterWithValue(p.Identifier, p.Value);
                    }
                }

                result = new QueryResult(command.ExecuteNonQuery());
                connection.Close();
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }
            return result;
        }

        public QueryResult TableExists(String table)
        {
            QueryResult result = QueryResult.Default;
            try
            {
                DbConnection connection = CreateConnection();
                connection.Open();
                DbCommand command = connection.CreateCommand();
                String sql = String.Format("SHOW TABLES LIKE '{0}';", table);
                command.CommandText = sql;
                var reader = command.ExecuteReader(CommandBehavior.SingleResult);
                bool hasRows = reader.HasRows;
                result = new QueryResult(0, hasRows, false, null);

                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                result = new QueryResult(ex);
            }
            return result;
        }

        public QueryResult Insert(String table, String[] columns, Object[] arguments, String where)
        {
            QueryResult result = QueryResult.Default;
            DbConnection connection = CreateConnection();
            String sql = string.Empty;
            try
            {
                connection.Open();
                DbCommand command = connection.CreateCommand();
                sql = GenerateInsert(table, columns, arguments, where);

                command.CommandText = sql;
                for (int i = 0; i < columns.Length; i++)
                {
                    command.AddParameterWithValue(columns[i], arguments[i]);
                }

                result = new QueryResult(command.ExecuteNonQuery()) { Data = arguments, Query = sql, LastInsertedId = GetLastInsertedId(command) };
            }
            catch (Exception ex)
            {
                result = new QueryResult(ex) { Data = arguments, Query = sql };
            }
            finally
            {
                connection.Close();
            }
            return result;
        }

        public long GetLastInsertedId(DbCommand cmd)
        {
            if (ConnectionType == ConnectionType.MySql)
            {
                var mysqlCommand = (MySqlCommand) cmd;
                return mysqlCommand.LastInsertedId;
            }
            return 0;
        }

        public void CreateTableFromDefinition(String definition, String table)
        {
            DbConnection connection = CreateConnection();
            try
            {
                connection.Open();
                DbCommand command = connection.CreateCommand();
                String sql = GenerateCreateTable(definition, table);
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
            finally
            {
                connection.Close();
            }
        }

        public String GenerateCreateTable(String definition, String name)
        {
            return String.Format(definition, name);
        }

        public QueryListResult InsertBulk(DbConnection connection, String table, String[] columns, List<Object[]> arguments, bool insertIgnore = false)
        {
            var result = new QueryListResult();
            String sql = string.Empty;
            try
            {
                using (var transaction = connection.BeginTransaction())
                {
                    DbCommand command = connection.CreateCommand();
                    sql = GenerateInsert(table, columns, arguments, String.Empty, insertIgnore);

                    command.CommandText = sql;
                    command.Transaction = transaction;

                    foreach (var argument in arguments)
                    {
                        for (int i = 0; i < columns.Length; i++)
                        {
                            command.AddParameterWithValue(columns[i], argument[i]);
                        }
                        result.Add(new QueryResult(command.ExecuteNonQuery()) { Data = argument, Query = sql }); // Daten zuweisen die versucht worden sind zu übermitteln
                    }

                    transaction.Commit();
                }

            }
            catch (Exception ex)
            {
                result = new QueryListResult(ex);
            }
            finally
            {

            }
            return result;
        }

        public QueryListResult UpdateBulk(DbConnection connection, DbTransaction transaction, string table, string[] columns, List<Object[]> values, string where, List<Parameter> parameter = null)
        {
            var result = new QueryListResult();
            try
            {
                DbCommand command = connection.CreateCommand();

                var sql = GenerateUpdate(table, columns, @where);

                command.CommandText = sql;
                command.Transaction = transaction;

                for (int j = 0; j < values.Count; j++)
                {
                    var value = values[j];
                    for (int i = 0; i < columns.Length; i++)
                    {
                        command.AddParameterWithValue(columns[i], value[i]);
                    }

                    if (parameter != null)
                    {
                        var p = parameter[j];
                        command.AddParameterWithValue(p.Identifier, p.Value);
                    }
                    result.Add(new QueryResult(command.ExecuteNonQuery()) { Data = value, Query = sql });
                }

                return result;
            }
            catch (Exception ex)
            {
                result = new QueryListResult(ex);
            }
            return result;
        }

        public QueryResult Insert(String table, String[] columns, Object[] arguments)
        {
            return Insert(table, columns, arguments, String.Empty);
        }

        public QueryResult Update(string table, string[] columns, object[] arguments, string where, List<Parameter> parameter = null)
        {
            QueryResult result;
            DbConnection connection = CreateConnection();
            try
            {
                connection.Open();
                DbCommand command = connection.CreateCommand();

                String sql = GenerateUpdate(table, columns, where);
                command.CommandText = sql;

                for (int i = 0; i < columns.Length; i++)
                {
                    command.AddParameterWithValue(columns[i], arguments[i]);
                }

                if (parameter != null)
                {
                    for (int i = 0; i < parameter.Count; i++)
                    {
                        var p = parameter[i];
                        command.AddParameterWithValue(p.Identifier, p.Value);
                    }
                }

                result = new QueryResult(command.ExecuteNonQuery()) { Data = arguments, Query = sql };
                connection.Close();
                return result;
            }
            catch (Exception ex)
            {
                result = new QueryResult(ex);
            }
            return result;
        }

        public QueryResult IsInTable(String column, String table, String where, Object value)
        {
            QueryResult result;
            DbConnection connection = CreateConnection();
            try
            {
                String c = "";
                if (value is String)
                    c = "'";
                connection.Open();
                DbCommand command = connection.CreateCommand();
                String sql = $"SELECT {column} FROM {table} WHERE {@where} = {c}{value}{c};";
                command.CommandText = sql;
                var reader = command.ExecuteReader(CommandBehavior.SingleResult);
                bool hasRows = reader.HasRows;
                result = new QueryResult(0, hasRows, false, null);
                reader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                result = new QueryResult(ex);
            }
            return result;
        }

        public List<Object[]> SelectDataSet(String query, List<Parameter> parameter = null)
        {
            DbConnection connection = CreateConnection();
            
            try
            {
                connection.Open();
                DbCommand command = connection.CreateCommand();
                
                String sql = query;
                command.CommandText = sql;
                if (parameter != null)
                {
                    command.Prepare();
                    foreach (var p in parameter)
                    {
                        if (command is MySqlCommand)
                        {
                            var cmd = (MySqlCommand) command;
                            cmd.Parameters.AddWithValue(p.Identifier, p.Value);
                        }
                        else
                        {
                            command.AddParameterWithValue(p.Identifier, p.Value);
                        }
                    }
                }
                var reader = command.ExecuteReader();
                List<Object[]> data = new List<Object[]>();
                int i = 0;
                while (reader.Read())
                {
                    data.Add(new object[reader.FieldCount]);
                    reader.GetValues(data[i]);
                    i++;
                }
                reader.Close();
                connection.Close();
                return data;
            }
            finally
            {
                connection.Close();
            }

            ////return null;
        }

        public String GenerateInsert(String table, String[] columns, Object[] arguments, String where)
        {
            return GenerateInsert(table, columns, new List<object[]> { arguments }, where);
        }

        public String GenerateInsert(String table, String[] columns, List<Object[]> arguments, String where, bool insertIgnore = false)
        {
            string ignore = insertIgnore ? "IGNORE" : "";
            return $"INSERT {ignore} INTO {table} ({BuildColumns(columns, ", ", table)}) VALUES ({BuildInsertValues(columns)}) {@where};";
        }

        //public String GenerateInsertIgnore(String table, String[] columns, List<Object[]> arguments, String where)
        //{
        //    return String.Format("INSERT IGNORE INTO {0} ({1}) VALUES {2} {3};", table, BuildColumns(columns, ", ", table), BuildInsertValues(columns), where);
        //}

        public String GenerateUpdate(String table, String[] columns, String where)
        {
            return $"UPDATE {table} SET {BuildUpdatePairs(columns)} where {@where};";
        }

        public static String BuildUpdatePairs(String[] columns)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < columns.Length; i++)
            {
                if (i != columns.Length - 1)
                {
                    builder.Append($"{columns[i]} = @{columns[i]},");
                }
                else
                {
                    builder.Append($"{columns[i]} = @{columns[i]}");
                }
            }
            return builder.ToString();
        }

        public static String BuildInsertValues(string[] array)
        {
            StringBuilder builder = new StringBuilder();

            int i = 0;
            foreach (var column in array)
            {
                builder.Append($"@{column}");
                if (i < array.Length - 1)
                {
                    builder.Append(",");
                }
                i++;
            }
            return builder.ToString();
        }

        public static String BuildColumns(Object[] array, String splitter, String table)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < array.Length; i++)
            {
                if (i != array.Length - 1)
                {
                    builder.AppendFormat("{0}.{1}{2}", table, array[i], splitter);
                }
                else
                {
                    builder.AppendFormat("{0}.{1}", table, array[i]);
                }
            }
            return builder.ToString();
        }
    }
}
