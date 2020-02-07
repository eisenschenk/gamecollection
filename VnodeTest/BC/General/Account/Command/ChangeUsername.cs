using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.General.Account.Command
{
    public class ChangeUsername : AggregateCommand<Account>
    {
        public string NewUsername { get; }

        public ChangeUsername(AggregateID<Account> id, string newUsername) : base(id)
        {
            NewUsername = newUsername;
        }

    }
}
