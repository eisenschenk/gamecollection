using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACL.UI.React;
using static ACL.UI.React.DOM;

namespace VnodeTest.General.Sidebar
{
    public abstract class SidebarBaseEntry
    {
        public string Icon { get; }
        public string Name { get; }


        public SidebarBaseEntry(string name, string icon = default)
        {
            Icon = icon;
            Name = name;
        }

        public abstract VNode Render();
    }
}
