using System;
using System.Collections.Generic;
using System.Text;

namespace CBSM.Database
{
    public class TableColumn
    {
        private string name;
        private Type type;
        private bool primarykey;
        private object defaultvalue;
        private bool nullable;
        private bool unique;
        private __ForeignKey foreignkey;

        public TableColumn(string name, Type type)
        {
            this.name = name;
            this.type = type;
            this.nullable = true;
            foreignkey = null;
        }

        protected TableColumn()
        {
        }

        public string Name
        {
            get { return name; }
        }

        public Type Type
        {
            get { return type; }
            set { type = value; }
        }

        public __ForeignKey ForeignKey
        {
            get { return foreignkey; }
            set { foreignkey = value; }
        }

        public bool PrimaryKey
        {
            get { return primarykey; }
            set { primarykey = value; }
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
