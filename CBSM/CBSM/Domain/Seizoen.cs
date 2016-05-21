﻿using CBSM.Domain.Object;
using System;
using System.Collections.Generic;
using System.Text;

namespace CBSM.Domain
{
    public class Season : BaseClass<Season>
    {
        private int number;
        private string description;
        private Serie serie;
        //private List<Episode> episodes;

        public Season(int number, string description, Serie serie)
        {
            this.number = number;
            this.description = description;
            this.serie = serie;
        }

        public Season()
        {
            
        }
    }
}
