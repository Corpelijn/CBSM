using System;
using System.Linq;
using System.Collections.Generic;

namespace CBSM_Web_UI.Domain
{
    public class Navbar
    {
        private static List<NavbarItem> items;

        public static IEnumerable<NavbarItem> GetNavbarItems()
        {
            items = new List<NavbarItem>();

            items.Add(new NavbarHeader { Text = "MAIN NAVIGATION" });

            items.Add(new NavbarMenuItem { Id = 1, Text = "Dashboard", Icon = "fa fa-dashboard" });
            items.Add(new NavbarMenuItem { Text = "Dashboard v1", Icon = "fa fa-circle-o", Parent = 1, Action = "index", Controller = "default" });
            items.Add(new NavbarMenuItem { Text = "Dashboard v2", Icon = "fa fa-circle-o", Parent = 1, Action = "index2", Controller = "default"  });

            items.Add(new NavbarMenuItem { Id = 2, Text = "Layout Options", Icon = "fa fa-files-o", LabelText = "4", LabelColor = "bg-blue" });
            items.Add(new NavbarMenuItem { Text = "Top Navigation", Icon = "fa fa-circle-o", Parent = 2, Action = "top_nav" });
            items.Add(new NavbarMenuItem { Text = "Boxed", Icon = "fa fa-circle-o", Parent = 2 });
            items.Add(new NavbarMenuItem { Text = "Fixed", Icon = "fa fa-circle-o", Parent = 2 });
            items.Add(new NavbarMenuItem { Text = "Collapsed Sidebar", Icon = "fa fa-circle-o", Parent = 2 });

            items.Add(new NavbarMenuItem { Text = "Widgets", Icon = "fa fa-th", LabelText = "new", LabelColor = "bg-green", Action = "widgets" });

            items.Add(new NavbarMenuItem { Id = 3, Text = "Charts", Icon = "fa fa-pie-chart" });
            items.Add(new NavbarMenuItem { Text = "ChartJS", Icon = "fa fa-circle-o", Parent = 3 });
            items.Add(new NavbarMenuItem { Text = "Morris", Icon = "fa fa-circle-o", Parent = 3 });
            items.Add(new NavbarMenuItem { Text = "Flot", Icon = "fa fa-circle-o", Parent = 3 });
            items.Add(new NavbarMenuItem { Text = "Inline charts", Icon = "fa fa-circle-o", Parent = 3 });

            items.Add(new NavbarMenuItem { Id = 4, Text = "UI Elements", Icon = "fa fa-laptop" });
            items.Add(new NavbarMenuItem { Text = "General", Icon = "fa fa-circle-o", Parent = 4 });
            items.Add(new NavbarMenuItem { Text = "Icons", Icon = "fa fa-circle-o", Parent = 4 });
            items.Add(new NavbarMenuItem { Text = "Buttons", Icon = "fa fa-circle-o", Parent = 4 });
            items.Add(new NavbarMenuItem { Text = "Sliders", Icon = "fa fa-circle-o", Parent = 4 });
            items.Add(new NavbarMenuItem { Text = "Timeline", Icon = "fa fa-circle-o", Parent = 4 });
            items.Add(new NavbarMenuItem { Text = "Modals", Icon = "fa fa-circle-o", Parent = 4 });

            items.Add(new NavbarMenuItem { Id = 5, Text = "Forms", Icon = "fa fa-edit" });
            items.Add(new NavbarMenuItem { Text = "General Elements", Icon = "fa fa-circle-o", Parent = 5 });
            items.Add(new NavbarMenuItem { Text = "Advanced Elements", Icon = "fa fa-circle-o", Parent = 5 });
            items.Add(new NavbarMenuItem { Text = "Editors", Icon = "fa fa-circle-o", Parent = 5 });

            items.Add(new NavbarMenuItem { Id = 6, Text = "Tables", Icon = "fa fa-table" });
            items.Add(new NavbarMenuItem { Text = "Simple tables", Icon = "fa fa-circle-o", Parent = 6 });
            items.Add(new NavbarMenuItem { Text = "Data tables", Icon = "fa fa-circle-o", Parent = 6 });

            items.Add(new NavbarMenuItem { Text = "Calendar", Icon = "fa fa-calendar", LabelText = "3;17", LabelColor = "bg-red;bg-blue" });

            items.Add(new NavbarMenuItem { Text = "Mailbox", Icon = "fa fa-envelope", LabelText = "12;16;5", LabelColor = "bg-yellow;bg-green;bg-red" });

            items.Add(new NavbarMenuItem { Id = 7, Text = "Examples", Icon = "fa fa-folder" });
            items.Add(new NavbarMenuItem { Text = "Invoice", Icon = "fa fa-circle-o", Parent = 7 });
            items.Add(new NavbarMenuItem { Text = "Profile", Icon = "fa fa-circle-o", Parent = 7 });
            items.Add(new NavbarMenuItem { Text = "Login", Icon = "fa fa-circle-o", Parent = 7 });
            items.Add(new NavbarMenuItem { Text = "Register", Icon = "fa fa-circle-o", Parent = 7 });
            items.Add(new NavbarMenuItem { Text = "Lockscreen", Icon = "fa fa-circle-o", Parent = 7 });
            items.Add(new NavbarMenuItem { Text = "404 Error", Icon = "fa fa-circle-o", Parent = 7 });
            items.Add(new NavbarMenuItem { Text = "500 Error", Icon = "fa fa-circle-o", Parent = 7 });
            items.Add(new NavbarMenuItem { Text = "Blank Page", Icon = "fa fa-circle-o", Parent = 7 });
            items.Add(new NavbarMenuItem { Text = "Pace Page", Icon = "fa fa-circle-o", Parent = 7 });

            items.Add(new NavbarMenuItem { Id = 8, Text = "Multilevel", Icon = "fa fa-share" });
            items.Add(new NavbarMenuItem { Text = "Level One", Icon = "fa fa-circle-o", Parent = 8 });
            items.Add(new NavbarMenuItem { Id = 9, Text = "Level One", Icon = "fa fa-circle-o", Parent = 8 });
            items.Add(new NavbarMenuItem { Text = "Level One", Icon = "fa fa-circle-o", Parent = 8 });
            items.Add(new NavbarMenuItem { Text = "Level Two", Icon = "fa fa-circle-o", Parent = 9 });
            items.Add(new NavbarMenuItem { Id = 10, Text = "Level Two", Icon = "fa fa-circle-o", Parent = 9 });
            items.Add(new NavbarMenuItem { Text = "Level Three", Icon = "fa fa-circle-o", Parent = 10 });
            items.Add(new NavbarMenuItem { Text = "Level Three", Icon = "fa fa-circle-o", Parent = 10 });

            items.Add(new NavbarMenuItem { Text = "Documentation", Icon = "fa fa-book" });

            items.Add(new NavbarHeader { Text = "LABELS" });

            items.Add(new NavbarLabel { Text = "Important", Color = "text-red" });
            items.Add(new NavbarLabel { Text = "Warning", Color = "text-yellow" });
            items.Add(new NavbarLabel { Text = "Important", Color = "text-aqua" });

            return items;
        }

        public static NavbarItem FindItem(int id)
        {
            return items.Find(a => a.Id == id);
        }
    }
}