using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using CBSM.Database.Attributes;

namespace CBSM.Database
{
    public class DBMS
    {
        [DBMSPrimayKey]
        private int id;

        protected DBMS()
        {
            this.id = -1;

            SetDefaultValues();
        }

        public void WriteToDatabase()
        {
            // Check if the table exist
            if(!DatabaseManager.DoesTableExist(this.GetType().Name))
                CreateTable();

            // Check if the table has the correct columns
            CheckColumns();

            // Write the data to the table
            WriteData();
        }

        private List<TableColumn> GetColumns()
        {
            List<FieldInfo> fields = GetColumnsAsFields();

            List<TableColumn> columns = new List<TableColumn>();
            foreach (FieldInfo field in fields)
            {
                if (!(field.GetCustomAttributes(typeof(DBMSIgnore), false).Length > 0))
                {
                    TableColumn tc = new TableColumn(field.Name, field.FieldType);
                    if(field.GetCustomAttributes(typeof(DBMSPrimayKey), false).Length > 0)
                        tc.PrimaryKey = true;

                    if (field.GetCustomAttributes(typeof(DBMSDefaultValue), false).Length > 0)
                    {
                        List<CustomAttributeData> data = new List<CustomAttributeData>(field.GetCustomAttributesData());
                        CustomAttributeData val = data.Find(f => f.AttributeType == typeof(DBMSDefaultValue));
                        tc.DefaultValue = val.ConstructorArguments[0].Value;
                    }

                    if (field.GetCustomAttributes(typeof(DBMSUnique), false).Length > 0)
                        tc.Unique = true;

                    if (field.GetCustomAttributes(typeof(DBMSNotNull), false).Length > 0)
                        tc.Nullable = false;

                    columns.Add(tc);
                }
            }

            return columns;
        }

        private List<FieldInfo> GetColumnsAsFields(Type[] filter = null)
        {
            if (filter == null)
            {
                filter = new Type[] { };
            }

            List<FieldInfo> fields = new List<FieldInfo>();
            Type baseClass = this.GetType();

            while (baseClass != typeof(object))
            {
                fields.AddRange(baseClass.GetFields(BindingFlags.Instance | BindingFlags.NonPublic));
                baseClass = baseClass.BaseType;
            }

            for (int i = 0; i < fields.Count; i++)
            {
                for (int j = 0; j < filter.Length; j++)
                {
                    if (fields[i].GetCustomAttributes(filter[j], false).Length > 0)
                    {
                        fields.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }

            return fields;
        }

        private void CheckColumns()
        {
            List<TableColumn> columns = GetColumns();
            foreach (TableColumn column in columns)
            {
                int length = 0;
                if (column.Type == typeof(string))
                {
                    Type baseClass = this.GetType();
                    FieldInfo field =null;
                    while (baseClass != typeof(object))
                    {
                        field = baseClass.GetField(column.Name, BindingFlags.Instance | BindingFlags.NonPublic);
                        if (field != null)
                            break;
                        baseClass = baseClass.BaseType;
                    }
                    length = field.GetValue(this).ToString().Length;
                }
                DatabaseManager.CheckOrAlterColumn(this.GetType().Name, column.Name, column.Type, length);
            }
        }

        private void CreateTable()
        {
            List<TableColumn> columns = GetColumns();

            DatabaseManager.CreateTable(this.GetType().Name, columns);
        }

        private void InsertRecord()
        {
            List<object> data = new List<object>();
            StringBuilder query = new StringBuilder();

            query.Append("insert into ").Append(this.GetType().Name).Append(" (");
            foreach (FieldInfo fi in GetColumnsAsFields(new Type[] { typeof(DBMSPrimayKey) }))
            {
                query.Append(fi.Name).Append(",");
                data.Add(fi.GetValue(this));
            }

            query.Remove(query.Length - 1, 1).Append(") values (");

            for (int i = 0; i < data.Count; i++)
            {
                query.Append("?,");
            }

            query.Remove(query.Length - 1, 1).Append(")");

            DatabaseManager.ExecuteNonQuery(query.ToString(), data.ToArray());

            DataTable dt = DatabaseManager.ExecuteQuery("select max(id) as id from " + this.GetType().Name);
            this.id = int.Parse(dt.GetValueFromRow(0, "id").ToString());
        }

        private void UpdateRecord()
        {
            List<object> data = new List<object>();
            StringBuilder query = new StringBuilder();

            query.Append("update ").Append(this.GetType().Name).Append(" set ");
            foreach (FieldInfo fi in GetColumnsAsFields(new Type[] { typeof(DBMSPrimayKey) }))
            {
                query.Append(fi.Name).Append("=?,");
                data.Add(fi.GetValue(this));
            }

            query.Remove(query.Length - 1, 1);
            query.Append(" where id=?");
            data.Add(this.id);

            DatabaseManager.ExecuteNonQuery(query.ToString(), data.ToArray());
        }

        private void WriteData()
        {
            if (id == -1)
            {
                // Insert to the database
                InsertRecord();
            }
            else
            {
                // Update the existing record
                UpdateRecord();
            }
        }

        private void SetDefaultValues()
        {
            List<FieldInfo> fields = GetColumnsAsFields();
            foreach (FieldInfo field in fields)
            {
                if (field.GetCustomAttributes(typeof(DBMSDefaultValue), false).Length > 0)
                {
                    List<CustomAttributeData> data = new List<CustomAttributeData>(field.GetCustomAttributesData());
                    CustomAttributeData val = data.Find(f => f.AttributeType == typeof(DBMSDefaultValue));
                    field.SetValue(this, val.ConstructorArguments[0].Value);
                }
            }
        }
    }

    public class DBMS<T> : DBMS where T : new()
    {
        public static T[] GetAllFromDatabase()
        {
            DataTable dt = DatabaseManager.ExecuteQuery("select id from " + typeof(T).Name);

            List<T> data = new List<T>();
            foreach (DataRow row in dt)
            {
                data.Add(GetFromDatabaseById(int.Parse(row.GetData("id").ToString())));
            }

            return data.ToArray();
        }

        public static T[] GetFromDatabase(params object[] constraints)
        {
            return null;
        }

        public static T GetFromDatabaseById(int id)
        {
            T instance = new T();
            Type currentType = instance.GetType();

            MethodInfo mi = null;
            while (currentType != typeof(object) && mi == null)
            {
                mi = currentType.GetMethod("GetColumnsAsFields", BindingFlags.NonPublic | BindingFlags.Instance);
                currentType = currentType.BaseType;
            }

            if (mi != null)
            {
                List<FieldInfo> fields = (List<FieldInfo>)mi.Invoke(instance, new object[] {new Type[] {}});

                DataTable dt = DatabaseManager.ExecuteQuery("select * from " + instance.GetType().Name + " where id=?", id);
                foreach (FieldInfo fi in fields)
                {
                    fi.SetValue(instance, dt.GetValueFromRow(0, fi.Name));
                }
            }
            else
                return default(T);

            return instance;
        }
    }
}
