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
            if (!DatabaseManager.DoesTableExist(this.GetType().Name))
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

                    // Check for a collection
                    if (field.FieldType.GetInterface("ICollection") != null)
                    {
                        ICollection c = (ICollection)field.GetValue(this);
                        if (c.GetType().GenericTypeArguments[0].IsSubclassOf(typeof(DBMS)))
                        {
                            Console.WriteLine("linktable met vage klassen");
                        }
                        else
                        {
                            string tablename = "__link_" + this.GetType().Name + "_" + field.Name + "_" + c.GetType().GenericTypeArguments[0].Name;
                            CollectionColumn ccol = new CollectionColumn(tablename);
                            ccol.AddColumn(new PrimaryKeyColumn("id", typeof(int)));
                            ccol.AddColumn(new ForeignKeyColumn(new __ForeignKey(tablename, this.GetType().Name + "_id", this.GetType().FullName)));
                            ccol.AddColumn(new ForeignKeyColumn(new __ForeignKey(tablename, c.GetType().GenericTypeArguments[0].Name + "_id", c.GetType().GenericTypeArguments[0].FullName)));

                            if (!DatabaseManager.DoesTableExist(c.GetType().GenericTypeArguments[0].Name))
                            {
                                CollectionColumn typecol = new CollectionColumn(c.GetType().GenericTypeArguments[0].Name);
                                typecol.AddColumn(new PrimaryKeyColumn("id", typeof(int)));
                                typecol.AddColumn(new DataColumn("value", c.GetType().GenericTypeArguments[0]));
                                ccol.AddColumn(typecol);
                            }

                            col = ccol;
                        }
                    }
                    else
                    {
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
                    }

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
                if (column.GetType() == typeof(CollectionColumn))
                    continue;
                if (column.ColumnType == typeof(string))
                {
                    Type baseClass = this.GetType();
                    FieldInfo field = null;
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

        private int CheckIfValueExists(object value)
        {
            DataTable dt = DatabaseManager.ExecuteQuery("select id from " + value.GetType().Name + " where value=?", value);
            if (dt.GetRowCount() == 0)
                return -1;
            else
                return Convert.ToInt32(dt.GetValueFromRow(0, "id"));
        }

        private void InsertRecord()
        {
            List<object> data = new List<object>();
            StringBuilder query = new StringBuilder();

            Dictionary<int, string> foreignkeyid = new Dictionary<int, string>();

            query.Append("insert into ").Append(this.GetType().Name).Append(" (");
            foreach (FieldInfo fi in GetColumnsAsFields(new Type[] { typeof(DBMSPrimayKey), typeof(DBMSIgnore) }))
            {
                if (fi.FieldType.GetInterface("ICollection") != null)
                {
                    ICollection c = (ICollection)fi.GetValue(this);
                    if (c.GetType().GenericTypeArguments[0].IsSubclassOf(typeof(DBMS)))
                    {
                        Console.WriteLine("linktable met vage klassen");
                    }
                    else
                    {
                        string tablename = "__link_" + this.GetType().Name + "_" + fi.Name + "_" + c.GetType().GenericTypeArguments[0].Name;
                        //tablename = c.GetType().GenericTypeArguments[0].Name;
                        foreach (var item in c)
                        {
                            int itemid = -1;
                            if ((itemid = CheckIfValueExists(item)) == -1)
                            {
                                DatabaseManager.ExecuteNonQuery("insert into " + item.GetType().Name + "(value) values (?)", item);
                                DataTable linkdt = DatabaseManager.ExecuteQuery("select max(id) as id from " + item.GetType().Name);
                                itemid =Convert.ToInt32(linkdt.GetValueFromRow(0, "id"));
                            }
                            foreignkeyid.Add(itemid, tablename);
                        }
                    }
                    continue;
                }

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


            // Set the foreign key values
            foreach (KeyValuePair<int, string> kvp in foreignkeyid)
            {
                string colname = kvp.Value.Substring(kvp.Value.LastIndexOf('_') + 1);
                DatabaseManager.ExecuteNonQuery("insert into " + kvp.Value + " (" + this.GetType().Name + "_id, " + colname + "_id) values (?,?)", this.id, kvp.Key);
            }
        }

        private void UpdateRecord()
        {
            List<object> data = new List<object>();
            StringBuilder query = new StringBuilder();

            query.Append("update ").Append(this.GetType().Name).Append(" set ");
            foreach (FieldInfo fi in GetColumnsAsFields(new Type[] { typeof(DBMSPrimayKey), typeof(DBMSIgnore) }))
            {
                if (fi.FieldType.GetInterface("ICollection") != null)
                {
                    ICollection c = (ICollection)fi.GetValue(this);
                    if (c.GetType().GenericTypeArguments[0].IsSubclassOf(typeof(DBMS)))
                    {
                        Console.WriteLine("linktable met vage klassen");
                    }
                    else
                    {
                        string tablename = "__link_" + this.GetType().Name + "_" + fi.Name + "_" + c.GetType().GenericTypeArguments[0].Name;
                        //tablename = c.GetType().GenericTypeArguments[0].Name;
                        foreach (var item in c)
                        {
#warning Lijst bijwerken
                            int itemid = -1;
                            if ((itemid = CheckIfValueExists(item)) == -1)
                            {
                                DatabaseManager.ExecuteNonQuery("insert into " + item.GetType().Name + "(value) values (?)", item);
                                DataTable linkdt = DatabaseManager.ExecuteQuery("select max(id) as id from " + item.GetType().Name);
                                itemid = Convert.ToInt32(linkdt.GetValueFromRow(0, "id"));
                            }
                            foreignkeyid.Add(itemid, tablename);
                        }
                    }
                    continue;
                }

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

        private static int linktable = 0;
        private static int NextLinkTableId()
        {
            linktable++;
            return linktable;
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
                List<FieldInfo> fields = (List<FieldInfo>)mi.Invoke(instance, new object[] { new Type[] { } });
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
                        if (fi.FieldType.GetInterface("ICollection") != null)
                        {
                            Type collectiontype =fi.FieldType.GenericTypeArguments[0];
                            if (collectiontype.IsSubclassOf(typeof(DBMS)))
                            {
                                Console.WriteLine("linktable met vage klassen");
                            }
                            else
                            {
                                string table = "__link_" + instance.GetType().Name + "_" + fi.Name + "_" + collectiontype.Name;
                                var listType = typeof(List<>);
                                var constructedListType = listType.MakeGenericType(collectiontype);

                                IList c = (IList)Activator.CreateInstance(constructedListType);
                                DataTable collectiondata = DatabaseManager.ExecuteQuery("select " + collectiontype.Name + "_id as id from " + table + " where " + instance.GetType().Name + "_id=?", id);
                                foreach (DataRow row in collectiondata)
                                {
                                    DataTable collectionvalue = DatabaseManager.ExecuteQuery("select value from " + collectiontype.Name + " where id=?", row.GetData("id"));
                                    c.Add(collectionvalue.GetValueFromRow(0, "value"));
                                }

                                fi.SetValue(instance, c);
                                continue;
                            }
                        }

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
