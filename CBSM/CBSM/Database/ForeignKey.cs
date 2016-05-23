using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSM.Database
{
    public class __ForeignKey : DBMS<__ForeignKey>
    {
        private string sourcetable;
        private string sourcecolumn;
        private string destinationtable;

        public __ForeignKey(string table, string column, string destinationtable)
        {
            this.sourcetable = table;
            this.sourcecolumn = column;
            this.destinationtable = destinationtable;
        }

        public __ForeignKey()
        {

        }

        public string Table
        {
            get { return sourcetable; }
        }

        public string Column
        {
            get { return sourcecolumn; }
        }

        public string DestinationTable
        {
            get { return destinationtable; }
        }
    }
}
