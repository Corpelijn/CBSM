using CBSM.Domain.Object;
using System;
using System.Collections.Generic;
using System.Text;

namespace CBSM.Domain
{
    public class Serie : BaseClass<Serie>
    {
        private string name;
        private string description;
        //private List<Season> seasons;

        public Serie(string name, string description)
        {
            this.name = name;
            this.description = description;
        }

        public Serie()
        {
        }
    }
}
