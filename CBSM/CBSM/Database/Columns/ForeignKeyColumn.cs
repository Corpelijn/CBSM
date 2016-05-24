using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSM.Database.Columns
{
    class ForeignKeyColumn : FieldToColumn
    {
        private __ForeignKey fk;

        public ForeignKeyColumn(__ForeignKey fk)
        {
            this.fk = fk;
            this.columnname = fk.Column;
            this.columnType = typeof(int);
        }

        public __ForeignKey ForeignKey
        {
            get { return fk; }
        }
    }
}
