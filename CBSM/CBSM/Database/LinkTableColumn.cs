using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSM.Database
{
    public class LinkTableColumn : TableColumn
    {
        private static int __linktablenum = 1000;

        private string destinationTable;
        private string destinationColumn;
        private string tablename;

        private List<TableColumn> columns;

        public LinkTableColumn(string destinationTable, string destinationColumn) 
        {
            this.tablename = "__link_" + __linktablenum;
            __linktablenum++;
            this.destinationTable = destinationTable;
            this.destinationColumn = destinationColumn;
            columns = new List<TableColumn>();
        }
    }
}
