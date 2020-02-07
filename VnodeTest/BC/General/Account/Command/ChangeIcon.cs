using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.General.Account.Command
{
    public class ChangeIcon : AggregateCommand<Account>
    {
        public string NewIcon { get; }

        public ChangeIcon(AggregateID<Account> id, string newIcon) : base(id)
        {
            NewIcon = newIcon;
        }

    }
}
