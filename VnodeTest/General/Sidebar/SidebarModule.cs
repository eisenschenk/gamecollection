using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VnodeTest.BC.General.Account;
using VnodeTest.Chess;
using VnodeTest.Solitaire;
using static ACL.UI.React.DOM;

namespace VnodeTest.General.Sidebar
{
    public class SidebarModule
    {
        public IEnumerable<SidebarBaseEntry> SidebarEntries { get; }

        public SidebarModule(IEnumerable<SidebarBaseEntry> sidebarEntries)
        {
            SidebarEntries = sidebarEntries;
        }

        public VNode Render()
        {
            return 
                Fragment(
                    Div(
                        Styles.Sidebar,
                        SidebarEntries.Select(s => s.Render())
                    )
                );
        }
    }
}
