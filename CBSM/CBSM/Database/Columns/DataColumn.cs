using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSM.Database.Columns
{
    class DataColumn : FieldToColumn
    {
        private object defaultvalue;
        private bool nullable;
        private bool unique;

        public DataColumn(string columnname, Type type)
        {
            this.columnname = columnname;
            this.columnType = type;
        }

        public object DefaultValue
        {
            get { return defaultvalue; }
            set { defaultvalue = value; }
        }

        public bool Nullable
        {
            get { return nullable; }
            set { nullable = value; }
        }

        public bool Unique
        {
            get { return unique; }
            set { unique = value; }
        }
    }
}
