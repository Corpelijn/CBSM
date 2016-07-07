using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSM.Database
{
    class ForeignKeyTableContentGenerator
    {
        private List<int> id;
        private List<string> table;

        public ForeignKeyTableContentGenerator()
        {
            this.id = new List<int>();
            this.table = new List<string>();
        }

        public void AddRecord(int id, string table)
        {
            this.id.Add(id);
            this.table.Add(table);
        }

        public Dictionary<string, object[]> InsertInstructions(Type currentInstanceType, int pk)
        {
            Dictionary<string, object[]> instructions = new Dictionary<string, object[]>();

            for (int i = 0; i < id.Count; i++)
            {
                string colname = table[i].Substring(table[i].LastIndexOf('_') + 1);
                DatabaseManager.ExecuteNonQuery("insert into " + table[i] + " (" + currentInstanceType.Name + "_id, " + colname + "_id) values (?,?)", pk, id[i]);
            }
            return instructions;
        }
    }
}
