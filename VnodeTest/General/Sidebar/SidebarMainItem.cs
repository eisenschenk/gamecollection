using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACL.UI.React;
using static ACL.UI.React.DOM;

namespace VnodeTest.General.Sidebar
{
    public class SidebarMainItem : SidebarBaseEntry
    {
        public SidebarMainItem(string name, string icon = null) : base(name, icon)
        {
        }

        public override VNode Render()
        {
            return Row(
                    Styles.MainMenuItem,
                    Div(Styles.Wicon & Styles.MR0P5, DOM.Icon(Icon)),
                    Text(Name)
                );
        }
    }
}
