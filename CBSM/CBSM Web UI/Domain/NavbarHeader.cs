using System;

namespace CBSM_Web_UI.Domain
{
    public class NavbarHeader : NavbarItem
    {
        public override string ToHtml(string act, string con)
        {
            return "<li class=\"header\">" + text + "</li>";
        }
    }
}