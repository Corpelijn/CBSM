using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSM.Database
{
    class KnownObject
    {
        private int id;
        private Type table;
        private object _object;
        private DateTime created;

        public KnownObject(int id, Type table, object _object)
        {
            this.id = id;
            this.table = table;
            this._object = _object;
            this.created = DateTime.Now;
        }

        public int Id
        {
            get { return id; }
        }

        public Type Table
        {
            get { return table; }
        }

        public object Object
        {
            get { return _object; }
        }

        public DateTime CreationTime
        {
            get { return created; }
        }

        public void ResetTime()
        {
            this.created = DateTime.Now;
        }
    }
}
