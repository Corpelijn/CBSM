using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CBSM_Web_UI.Domain
{
    public class NavbarLabel : NavbarItem
    {
        private string color;

        public string Color
        {
            get { return color; }
            set { color = value; }
        }

        public override string ToHtml(string act, string con)
        {
            return "<li><a href=\"#\"><i class=\"fa fa-circle-o " + color + "\"></i> <span>" + text + "</span></a></li>";
        }
    }
}