using System;
using System.Text;
using System.Collections.Generic;

namespace CBSM_Web_UI.Domain
{
    public class NavbarMenuItem : NavbarItem
    {
        private string icon;
        private string action;
        private string controller;
        private int parent;
        private List<NavbarMenuItem> children;
        private bool isDrawn;

        private string labeltext;
        private string labelcolor;

        public NavbarMenuItem()
        {
            this.children = new List<NavbarMenuItem>();
            this.parent = -1;
            this.controller = "";
            this.action = "";
            this.isDrawn = false;
            this.labeltext = "";
            this.labelcolor = "";
        }

        public string Icon
        {
            get { return icon; }
            set { icon = value; }
        }

        public string Action
        {
            get { return action; }
            set { action = value; }
        }

        public string Controller
        {
            get { return controller; }
            set { controller = value; }
        }

        public string LabelText
        {
            get { return labeltext; }
            set { labeltext = value; }
        }

        public string LabelColor
        {
            get { return labelcolor; }
            set { labelcolor = value; }
        }

        public int Parent
        {
            get { return parent; }
            set
            {
                parent = value;
                NavbarItem parent_item = Navbar.FindItem(value);
                if (parent_item.GetType() == typeof(NavbarMenuItem))
                {
                    (parent_item as NavbarMenuItem).AddChild(this);
                }
            }
        }

        public IEnumerable<NavbarMenuItem> Children
        {
            get { return children; }
        }

        public void AddChild(NavbarMenuItem child)
        {
            if (!children.Contains(child))
            {
                children.Add(child);
            }
        }

        public override string ToHtml(string act, string con)
        {
            if (isDrawn)
                return "";
            isDrawn = true;

            StringBuilder sb = new StringBuilder();

            if (parent == -1)
            {
                sb.Append("<li class=\"treeview\">");
            }

            if (this.children.Count == 0)
            {
                sb.Append("<li class=\"");
                if (act == action && con == controller)
                    sb.Append("active");
                sb.Append("\"><a href=\"");
                if (this.controller != "" && this.action != "")
                    sb.Append("/").Append(this.controller).Append("/").Append(this.action);
                else if (this.controller != "")
                    sb.Append("/").Append(this.controller);
                else if (this.action != "")
                    sb.Append("./").Append(this.action);
                else
                    sb.Append("#");

                sb.Append("\"><i class=\"").Append(Icon).Append("\"></i> ").Append(Text);
                if (labeltext != "")
                {
                    string[] labels = labeltext.Split(new char[] { ';' });
                    string[] colors = labelcolor.Split(new char[] { ';' });
                    int colorindex = 0;

                    sb.Append("<span class=\"pull-right-container\">");
                    foreach (string label in labels)
                    {
                        sb.Append("<small class=\"label pull-right ").Append(colors[colorindex]).Append("\">").Append(label).Append("</small>");
                        colorindex++;
                        if (colorindex > colors.Length)
                            colorindex--;
                    }
                    sb.Append("</span>");
                }
                sb.Append("</a></li>");
            }
            else
            {
                sb.Append("<li><a href=\"#\">");
                sb.Append("<i class=\"").Append(Icon).Append("\"></i> <span>").Append(Text).Append("</span>");
                sb.Append("<span class=\"pull-right-container\">");
                sb.Append("<i class=\"fa fa-angle-left pull-right\"></i> ");
                if (labeltext != "")
                {
                    string[] labels = labeltext.Split(new char[] { ';' });
                    string[] colors = labelcolor.Split(new char[] { ';' });
                    int colorindex = 0;

                    foreach (string label in labels)
                    {
                        sb.Append("<small class=\"label pull-right ").Append(colors[colorindex]).Append("\">").Append(label).Append("</small>");
                        colorindex++;
                        if (colorindex > colors.Length)
                            colorindex--;
                    }
                }
                sb.Append("</span>").Append("</a>");
                sb.Append("<ul class=\"treeview-menu\">");

                foreach (NavbarMenuItem item in Children)
                {
                    sb.Append(item.ToHtml(act, con));
                }

                sb.Append("</ul>");
            }

            return sb.ToString();
        }
    }
}