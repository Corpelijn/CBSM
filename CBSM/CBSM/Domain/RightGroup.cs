using CBSM.Domain.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBSM.Domain
{
    public class RightGroup : BaseClass<RightGroup>
    {
        private List<Right> rights;
        private string name;

        public RightGroup()
        {
        }

        public RightGroup(string name)
        {
            this.name = name;
            this.rights = new List<Right>();
        }

        public void AddRight(Right right)
        {
            if (!rights.Contains(right))
                rights.Add(right);
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<Right> Rights
        {
            get { return new List<Right>(rights); }
        }
    }
}
