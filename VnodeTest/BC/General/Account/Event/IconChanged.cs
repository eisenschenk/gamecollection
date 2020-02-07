using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACL.ES;

namespace VnodeTest.BC.General.Account.Event
{
    public class IconChanged : AggregateEvent<Account>
    {
        public string NewIcon { get; }

        public IconChanged(AggregateID<Account> id, string newIcon) : base(id)
        {
            NewIcon = newIcon;
        }

    }
}
