using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.General.Account.Command
{
    public class ChangePassword : AggregateCommand<Account>
    {

        public string OldPassword { get; }
        public string NewPassword { get; }

        public ChangePassword(AggregateID<Account> id, string oldPassword, string newPassword) : base(id)
        {
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }
    }
}
