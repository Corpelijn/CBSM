using CBSM.Domain.Object;
using System;
using System.Collections.Generic;
using System.Text;

namespace CBSM.Domain
{
    public class Season : BaseClass<Season>
    {
        private int number;
        private string description;
        //private List<Episode> episodes;

        public Season(int number, string description)
        {
            this.number = number;
            this.description = description;
        }

        public Season()
        {
            
        }
    }
}
