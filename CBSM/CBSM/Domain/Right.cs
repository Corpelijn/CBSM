using CBSM.Domain.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSM.Domain
{
    public class Right : BaseClass<Right>
    {
        private string name;

        public Right()
        {
        }

        public Right(string name)
        {
            this.name = name;
        }
    }
}
