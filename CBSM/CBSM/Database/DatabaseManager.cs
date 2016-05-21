using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CBSM.Database
{
    public class DatabaseManager
    {
        private List<Connector> dbConnections;
        private int activeConnection;

        private static DatabaseManager instance;

        private DatabaseManager()
        {
            dbConnections = new List<Connector>();
            this.activeConnection = -1;
        }

        private static DatabaseManager GetInstance()
        {
            if(instance == null)
                instance =  new DatabaseManager();
            return instance;
        }

        public static void AddConnection(string hostname, string database, string username, string password, string provider = "MYSQL")
        {
            Connector connection = null;
            if (provider.ToUpper() == "MYSQL")
            {
                connection = new MySqlConnector(hostname, database, username, password);
                if (!connection.OpenConnection())
                {
                    return;
                }
            }

            if (!GetInstance().dbConnections.Contains(connection))
            {
                GetInstance().dbConnections.Add(connection);
                if (GetInstance().activeConnection == -1)
                {
                    GetInstance().activeConnection = 0;
                }
            }
        }

        private Connector GetCurrentConnection()
        {
            return dbConnections[activeConnection];
        }

        public static bool DoesTableExist(string name)
        {
            return GetInstance().GetCurrentConnection().DoesTableExist(name);
        }

        public static void CheckOrAlterColumn(string table, string column, Type type, int length)
        {
            GetInstance().GetCurrentConnection().CheckOrAlterColumn(table, column, type, length);
        }

        public static void CreateTable(string name, List<TableColumn> columns)
        {
            GetInstance().GetCurrentConnection().CreateTable(name, columns);
        }

        public void MoveTable()
        {
        }

        public void DeleteTable()
        {
        }

        public static DataTable ExecuteQuery(string query, params object[] data)
        {
            return GetInstance().GetCurrentConnection().ExecuteQuery(query, data);
        }

        public static bool ExecuteNonQuery(string instruction, params object[] data)
        {
            return GetInstance().GetCurrentConnection().ExecuteNonQuery(instruction, data);
        }
    }
}
