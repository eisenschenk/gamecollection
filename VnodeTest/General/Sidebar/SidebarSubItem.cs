using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACL.UI.React;
using static ACL.UI.React.DOM;

namespace VnodeTest.General.Sidebar
{
    public class SidebarSubItem : SidebarBaseEntry
    {
        public bool IsSelected { get; }
        public Action Action { get; }

        public SidebarSubItem(string name, string icon = null, bool isSelected = false, Action action = null) : base(name, icon)
        {
            IsSelected = isSelected;
            Action = action;
        }


        public override VNode Render()
        {
            return Row(
                    IsSelected ? Styles.SubMenuItemSelected : Styles.SubMenuItem,
                    Action,
                    Div(Styles.Wicon & Styles.MR0P5, DOM.Icon(Icon)),
                    Text(Name)
                );
        }
    }
}
