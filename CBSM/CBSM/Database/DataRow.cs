using System;
using System.Collections.Generic;
using System.Text;

namespace CBSM.Database
{
    public class DataRow
    {
        private string[] columns;
        private object[] data;

        public DataRow(string[] columns, object[] data)
        {
            this.columns = columns;
            this.data = data;
        }

        public string GetColumn(int index)
        {
            return columns[index];
        }

        public object GetData(int index)
        {
            return data[index];
        }

        public int GetColumnCount()
        {
            return columns.Length;
        }

        public object GetData(string column)
        {
            int index = -1;
            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i].ToLower().EndsWith(column.ToLower()))
                {
                    index = i;
                }
            }

            if (index == -1)
                return null;

            return data[index];
        }
    }
}
