using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSM.Database
{
    public class ForeignKey : DBMS<ForeignKey>
    {
        private string sourcetable;
        private string sourcecolumn;
        private string destinationtable;

        public ForeignKey(string table, string column, DBMS destinationtable)
        {
            this.sourcetable = table;
            this.sourcecolumn = column;
            this.destinationtable = destinationtable.GetType().FullName;
        }

        public ForeignKey()
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
