using CBSM.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace CBSM.Domain.Object
{
    public class BaseClass<T> : DBMS<T> where T : new()
    {
        public BaseClass()
        {
        }
    }
}
