using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACL.ES;

namespace VnodeTest.BC.General.Account.Event
{
    public class UsernameChanged : AggregateEvent<Account>
    {
        public string NewUsername { get; }

        public UsernameChanged(AggregateID<Account> id, string newUsername) : base(id)
        {
            NewUsername = newUsername;
        }

    }
}
