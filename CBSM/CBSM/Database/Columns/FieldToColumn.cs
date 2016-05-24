using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSM.Database.Columns
{
    public class FieldToColumn
    {
        protected string columnname;
        protected Type columnType;

        public string ColumnName
        {
            get { return columnname; }
        }

        public Type ColumnType
        {
            get { return columnType; }
        }

    }
}
