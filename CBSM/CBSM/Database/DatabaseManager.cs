using CBSM.Database.Columns;
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

        public static void CreateTable(string name, List<FieldToColumn> columns)
        {
            GetInstance().GetCurrentConnection().CreateTable(name, columns);
        }

        public static void AddForeignKeyReference(string table, string column, string destinationtable)
        {
            __ForeignKey fk = new __ForeignKey(table, column, destinationtable);
            fk.WriteToDatabase();
        }

        public static __ForeignKey[] GetForeignKeys(string table)
        {
            return __ForeignKey.GetFromDatabase("sourcetable=? or (sourcecolumn like ? and sourcetable like ?)", table, table + "%", "__link_%");
        }

        public static object GetObjectFromForeignKey(__ForeignKey fk, int id)
        {
            Type t = Type.GetType(fk.DestinationTable);
            MethodInfo mi = null;
            while(t != typeof(object) && mi == null)
            {
                mi = t.GetMethod("GetFromDatabaseById", BindingFlags.Static | BindingFlags.Public);
                t = t.BaseType;
            }
            
            object data = null;
            if(mi != null)
                data = mi.Invoke(null, new object[] {id});

            return data;
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
