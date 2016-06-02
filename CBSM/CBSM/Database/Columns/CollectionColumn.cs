using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSM.Database.Columns
{
    class CollectionColumn : FieldToColumn
    {
        private List<FieldToColumn> columns;

        public CollectionColumn(string tablename)
        {
            this.columnname = tablename;
            columns = new List<FieldToColumn>();
        }

        public void AddColumn(FieldToColumn column)
        {
            columns.Add(column);
        }

        public List<FieldToColumn> Columns
        {
            get { return columns; }
        }
    }
}
