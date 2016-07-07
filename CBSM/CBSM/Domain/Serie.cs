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
        //private Season season;
        private List<Season> seasons;
        //private List<int> seasons;

        public Serie(string name, string description)
        {
            this.name = name;
            this.description = description;
            //this.seasons = new List<int>(new int[] { 5, 7, 2, 5, 6, 4 });
            this.seasons = new List<Season>();
        }

        public Serie()
        {
        }

        public void AddSeason(Season season)
        {
            this.seasons.Add(season);
        }
    }
}
