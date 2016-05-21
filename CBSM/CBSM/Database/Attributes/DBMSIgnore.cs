using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSM.Database.Attributes
{
    /// <summary>
    /// This attribute tells the DBMS to ignore the following field
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class DBMSIgnore : System.Attribute
    {

    }
}
