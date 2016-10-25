using System;

namespace CBSM_Web_UI.Domain
{
    public abstract class NavbarItem
    {
        protected int id;
        protected string text;
        protected int uniqueId;

        private static int nextUniqueId = 0;

        public NavbarItem()
        {
            uniqueId = nextUniqueId;
            nextUniqueId++;
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public int UniqueId
        {
            get { return uniqueId; }
        }

        public abstract string ToHtml(string act, string con);
    }
}