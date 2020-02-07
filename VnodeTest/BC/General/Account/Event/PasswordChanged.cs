using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACL.ES;

namespace VnodeTest.BC.General.Account.Event
{
    public class PasswordChanged : AggregateEvent<Account>
    {
        public string NewPassword { get; }

        public PasswordChanged(AggregateID<Account> id,string newPassword) : base(id)
        {
            NewPassword = newPassword;
        }

    }
}
