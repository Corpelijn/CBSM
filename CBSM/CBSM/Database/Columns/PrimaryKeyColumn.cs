using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSM.Database.Columns
{
    class PrimaryKeyColumn : FieldToColumn
    {
        public PrimaryKeyColumn(string name, Type type)
        {
            this.columnname = name;
            this.columnType = type;
        }
    }
}
