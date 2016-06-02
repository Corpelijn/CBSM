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
        private List<int> seasons;

        public Serie(string name, string description)
        {
            this.name = name;
            this.description = description;
            this.seasons = new List<int>(new int[] { 5, 7, 2, 8, 4 });
        }

        public Serie()
        {
        }
    }
}
