using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using CBSM.Database.Attributes;
using System.Collections;
using CBSM.Database.Columns;

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

        private List<FieldToColumn> GetColumns()
        {
            List<FieldInfo> fields = GetColumnsAsFields();

            List<FieldToColumn> columns = new List<FieldToColumn>();
            foreach (FieldInfo field in fields)
            {
                if (!(field.GetCustomAttributes(typeof(DBMSIgnore), false).Length > 0))
                {
                    FieldToColumn col = new DataColumn(field.Name, field.FieldType);
                    //TableColumn tc = new TableColumn(field.Name, field.FieldType);
                    
                    //// Check for a collection
                    //if (field.FieldType.GetInterface("ICollection") != null)
                    //{
                    //    ICollection c = (ICollection)field.GetValue(this);
                    //    if (c.GetType().GenericTypeArguments[0].IsSubclassOf(typeof(DBMS)))
                    //    {
                    //        Console.WriteLine("linktable met vage klassen");
                    //    }
                    //    else
                    //    {

                    //    }
                    //}
                    //else
                    //{
                        if (field.FieldType.IsSubclassOf(typeof(DBMS)))
                        {
                            col = new ForeignKeyColumn(new __ForeignKey(this.GetType().Name, field.Name, field.FieldType.FullName));
                        }

                        if (field.GetCustomAttributes(typeof(DBMSPrimayKey), false).Length > 0)
                            col = new PrimaryKeyColumn(field.Name, field.FieldType);

                        if (field.GetCustomAttributes(typeof(DBMSDefaultValue), false).Length > 0)
                        {
                            List<CustomAttributeData> data = new List<CustomAttributeData>(field.GetCustomAttributesData());
                            CustomAttributeData val = data.Find(f => f.AttributeType == typeof(DBMSDefaultValue));
                            ((DataColumn)col).DefaultValue = val.ConstructorArguments[0].Value;
                        }

                        if (field.GetCustomAttributes(typeof(DBMSUnique), false).Length > 0)
                            ((DataColumn)col).Unique = true;

                        if (field.GetCustomAttributes(typeof(DBMSNotNull), false).Length > 0)
                            ((DataColumn)col).Nullable = false;
                    //}

                    columns.Add(col);
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
            List<FieldToColumn> columns = GetColumns();
            foreach (FieldToColumn column in columns)
            {
                int length = 0;
                if (column.ColumnType == typeof(string))
                {
                    Type baseClass = this.GetType();
                    FieldInfo field =null;
                    while (baseClass != typeof(object))
                    {
                        field = baseClass.GetField(column.ColumnName, BindingFlags.Instance | BindingFlags.NonPublic);
                        if (field != null)
                            break;
                        baseClass = baseClass.BaseType;
                    }
                    length = field.GetValue(this).ToString().Length;
                }
                DatabaseManager.CheckOrAlterColumn(this.GetType().Name, column.ColumnName, column.ColumnType, length);
            }
        }

        private void CreateTable()
        {
            List<FieldToColumn> columns = GetColumns();

            DatabaseManager.CreateTable(this.GetType().Name, columns);
        }

        private void InsertRecord()
        {
            List<object> data = new List<object>();
            StringBuilder query = new StringBuilder();

            query.Append("insert into ").Append(this.GetType().Name).Append(" (");
            foreach (FieldInfo fi in GetColumnsAsFields(new Type[] { typeof(DBMSPrimayKey), typeof(DBMSIgnore) }))
            {
                query.Append(fi.Name).Append(",");
                if (fi.GetValue(this).GetType().IsSubclassOf(typeof(DBMS)))
                {
                    DBMS obj = (DBMS)fi.GetValue(this);
                    data.Add(obj.GetIdForForeignKey());
                }
                else
                {
                    data.Add(fi.GetValue(this));
                }
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
            foreach (FieldInfo fi in GetColumnsAsFields(new Type[] { typeof(DBMSPrimayKey), typeof(DBMSIgnore) }))
            {
                query.Append(fi.Name).Append("=?,");
                if (fi.GetValue(this).GetType().IsSubclassOf(typeof(DBMS)))
                {
                    DBMS obj = (DBMS)fi.GetValue(this);
                    data.Add(obj.GetIdForForeignKey());
                }
                else
                {
                    data.Add(fi.GetValue(this));
                }
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

        private int GetIdForForeignKey()
        {
            if (id == -1)
            {
                WriteToDatabase();
            }
            return id;
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

        public static T[] GetFromDatabase(string whereclause, params object[] parameters)
        {
            DataTable dt = DatabaseManager.ExecuteQuery("select id from " + typeof(T).Name + " where " + whereclause, parameters);

            List<T> data = new List<T>();
            foreach (DataRow row in dt)
            {
                data.Add(GetFromDatabaseById(int.Parse(row.GetData("id").ToString())));
            }

            return data.ToArray();
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
                List<__ForeignKey> fk = new List<__ForeignKey>(DatabaseManager.GetForeignKeys(instance.GetType().Name));

                DataTable dt = DatabaseManager.ExecuteQuery("select * from " + instance.GetType().Name + " where id=?", id);
                foreach (FieldInfo fi in fields)
                {
                    __ForeignKey foreignKey = fk.Find(f => f.Column == fi.Name);
                    if (foreignKey != null)
                    {
                        fi.SetValue(instance, DatabaseManager.GetObjectFromForeignKey(foreignKey, int.Parse(dt.GetValueFromRow(0, fi.Name).ToString())));
                    }
                    else
                    {
                        fi.SetValue(instance, dt.GetValueFromRow(0, fi.Name));
                    }
                }
            }
            else
                return default(T);

            return instance;
        }
    }
}
