﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSM.Database.Attributes
{
    /// <summary>
    /// This attribute tells the DBMS that the following field is the primary key of the table
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class DBMSDefaultValue : System.Attribute
    {
        private object defaultValue;

        public DBMSDefaultValue(object value) : base()
        {
            this.defaultValue = value;
        }

        public object DefaultValue
        {
            get { return this.defaultValue; }
        }
    }
}
