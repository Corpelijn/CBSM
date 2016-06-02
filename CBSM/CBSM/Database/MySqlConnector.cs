using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data.Common;
using CBSM.Database.Columns;

namespace CBSM.Database
{
    class MySqlConnector : Connector
    {
        private DbConnection connection;

        public MySqlConnector(string hostname, int port, string database, string username, string password)
        {
            this.hostname = hostname;
            this.port = port;
            this.database = database;
            this.username = username;
            this.password = password;
        }

        public MySqlConnector(string hostname, string database, string username, string password)
        {
            this.hostname = hostname;
            this.database = database;
            this.username = username;
            this.password = password;
        }

        public override bool OpenConnection()
        {
            connection = new MySqlConnection("SERVER=" + hostname + ";" + "DATABASE=" + database + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";");
            connection.Open();

            return IsOpen();
        }

        public override bool CloseConnection()
        {
            connection.Close();
            return IsOpen();
        }

        public override bool IsOpen()
        {
            while (connection.State == System.Data.ConnectionState.Connecting)
                System.Threading.Thread.Sleep(1);

            return connection.State == System.Data.ConnectionState.Open || connection.State == System.Data.ConnectionState.Executing || connection.State == System.Data.ConnectionState.Fetching;
        }

        private void InsertParameterToInstruction(DbCommand command, object data)
        {
            int index = command.CommandText.IndexOf('?');
            command.CommandText = command.CommandText.Remove(index, 1);
            command.CommandText = command.CommandText.Insert(index, "@" + index);

            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = "@" + index;
            parameter.Value = data;

            if (data.GetType() == typeof(string))
                parameter.DbType = System.Data.DbType.String;
            else if (data.GetType() == typeof(int) || data.GetType() == typeof(short))
                parameter.DbType = System.Data.DbType.Int32;
            else if (data.GetType() == typeof(long))
                parameter.DbType = System.Data.DbType.Int64;
            else if (data.GetType() == typeof(DateTime))
                parameter.DbType = System.Data.DbType.DateTime;
            else if (data.GetType() == typeof(double) || data.GetType() == typeof(float))
                parameter.DbType = System.Data.DbType.Double;
            else if (data.GetType() == typeof(bool))
                parameter.DbType = System.Data.DbType.Boolean;
            else
                parameter.DbType = System.Data.DbType.Binary;

            command.Parameters.Add(parameter);
        }

        public override DataTable ExecuteQuery(string query, params object[] data)
        {
            if (!IsOpen())
            {
                return null;
            }

            DbCommand command = connection.CreateCommand();
            command.CommandText = query;

            foreach (object param in data)
            {
                InsertParameterToInstruction(command, param);
            }

#warning
            Console.WriteLine(command.CommandText);

            command.Prepare();
            DbDataReader reader = command.ExecuteReader(System.Data.CommandBehavior.KeyInfo);
            command.Dispose();

            DataTable dt = new DataTable(reader);
            reader.Close();
            reader.Dispose();

            command.Dispose();

            return dt;
        }

        public override bool ExecuteNonQuery(string instruction, params object[] data)
        {
            if (!IsOpen())
            {
                return false;
            }

            DbCommand command = connection.CreateCommand();
            command.CommandText = instruction;

            foreach (object param in data)
            {
                InsertParameterToInstruction(command, param);
            }

#warning
            Console.WriteLine(command.CommandText);

            command.Prepare();
            command.ExecuteNonQuery();

            command.Dispose();

            return false;
        }

        public override bool DoesTableExist(string name)
        {
            DataTable dt = ExecuteQuery("SHOW TABLES like ?", name);
            if (dt.GetRowCount() == 0)
            {
                return false;
            }

            return true;
        }

        public override bool CreateTable(string name, List<FieldToColumn> columns)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("create table ").Append(name).Append(" (\n");
            foreach (FieldToColumn c in columns)
            {
                if (c.GetType() == typeof(CollectionColumn))
                {
                    CollectionColumn col = (CollectionColumn)c;
                    CreateTable(col.ColumnName, col.Columns);
                    continue;
                }

                sb.Append(c.ColumnName).Append("\t");

                if (c.ColumnType == typeof(string))
                    sb.Append("varchar(1)");
                else if (c.ColumnType == typeof(int) || c.ColumnType == typeof(short))
                    sb.Append("integer(11)");
                else if (c.ColumnType == typeof(long))
                    sb.Append("bigint");
                else if (c.ColumnType == typeof(DateTime))
                    sb.Append("datetime");
                else if (c.ColumnType == typeof(double) || c.ColumnType == typeof(float))
                    sb.Append("decimal(9,2)");
                else if (c.ColumnType == typeof(bool))
                    sb.Append("integer(11)");
                else
                    sb.Append("blob");

                if (c.GetType() == typeof(PrimaryKeyColumn))
                    sb.Append("\tauto_increment primary key");
                else
                {
                    if (c.GetType() == typeof(ForeignKeyColumn))
                        ((ForeignKeyColumn)c).ForeignKey.WriteToDatabase();

                    if (c.GetType() == typeof(DataColumn))
                    {
                        DataColumn col = (DataColumn)c;
                        if (col.Nullable)
                            sb.Append("\tnull");
                        else
                            sb.Append("\tnot null");

                        if (col.Unique)
                            sb.Append("\tunique");

                        if (col.DefaultValue != null)
                            sb.Append("\tdefault ").Append(col.DefaultValue);
                    }
                }

                sb.Append(",\n");
            }

            sb.Remove(sb.Length - 2, 2).Append(")");

            this.ExecuteNonQuery(sb.ToString());

            return false;
        }

        public override bool CheckOrAlterColumn(string table, string column, Type type, int length)
        {
            DataTable dt = ExecuteQuery("SHOW COLUMNS FROM " + table);

            foreach (DataRow row in dt)
            {
                if (row.GetData("field").ToString().ToLower() == column.ToLower())
                {
                    if (type == typeof(string))
                    {
                        string[] lengte = row.GetData("type").ToString().Split(new char[] { '(', ')' });
                        if (Convert.ToInt32(lengte[1]) < length)
                        {
                            ExecuteNonQuery("alter table " + table + " modify column " + column + " varchar(" + length + ")");
                        }
                    }

                    return true;
                }
            }

            string s_type = "";
            if (type == typeof(string))
                s_type = "varchar(" + length + ")";
            else if (type == typeof(int) || type == typeof(short))
                s_type = "integer(11)";
            else if (type == typeof(long))
                s_type = "bigint";
            else if (type == typeof(DateTime))
                s_type = "datetime";
            else if (type == typeof(double) || type == typeof(float))
                s_type = "decimal(9,2)";
            else if (type == typeof(bool))
                s_type = "integer(11)";
            else
                s_type = "blob";

            ExecuteNonQuery("alter table " + table + " add column " + column + " " + s_type);

            return true;
        }
    }
}
