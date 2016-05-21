using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace CBSM.Database
{
    public class DataTable : IEnumerable
    {
        private string[] columns;
        private List<object[]> data;

        public DataTable(DbDataReader reader)
        {
            System.Data.DataTable dt = reader.GetSchemaTable();

            columns = new string[reader.FieldCount];
            for (int i = 0; i < columns.Length; i++)
            {
                columns[i] = dt.TableName + "." + reader.GetName(i);
            }

            dt.Dispose();

            data = new List<object[]>();
            while (reader.Read())
            {
                List<object> dat = new List<object>();
                for (int i = 0; i < columns.Length; i++)
                {
                    dat.Add(reader.GetValue(i));
                }

                data.Add(dat.ToArray());
            }

            reader.Close();
            reader.Dispose();
        }

        public object GetValueFromRow(int row, string column)
        {
            int index = 0;
            for(int i =0; i<columns.Length; i++)
            {
                if (columns[i].EndsWith(column))
                {
                    index = i;
                }
            }

            return data[row][index];
        }

        public DataRow GetRow(int row)
        {
            return new DataRow(columns, data[row]);
        }

        public int GetColumnCount()
        {
            return columns.Length;
        }

        public int GetRowCount()
        {
            return data.Count;
        }

        public IEnumerator GetEnumerator()
        {
            for (int i = 0; i < data.Count; i++)
            {
                yield return new DataRow(columns, data[i]);
            }
        }
    }
}
