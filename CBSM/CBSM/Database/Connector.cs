using System;
using System.Collections.Generic;
using System.Text;

namespace CBSM.Database
{
    abstract class Connector
    {
        protected string hostname;
        protected int port;
        protected string database;
        protected string username;
        protected string password;

        protected Connector()
        {
        }

        public string Hostname
        {
            get { return hostname; }
            set { hostname = value; }
        }

        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        public string Database
        {
            get { return database; }
            set { database = value; }
        }

        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public abstract bool OpenConnection();

        public abstract bool CloseConnection();

        public abstract bool IsOpen();

        public abstract DataTable ExecuteQuery(string query, params object[] data);

        public abstract bool ExecuteNonQuery(string instruction, params object[] data);

        public abstract bool DoesTableExist(string name);

        public abstract bool CheckOrAlterColumn(string table, string column, Type type, int length);

        public abstract bool CreateTable(string name, List<TableColumn> columns);
    }
}
