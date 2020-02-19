using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.General.Account.Command
{
    public class ChangeAutomaticPromotion : AggregateCommand<Account>
    {
        public bool OldSetting { get; }
        public bool NewSetting => !OldSetting;
        public ChangeAutomaticPromotion(AggregateID<Account> id, bool currentSetting) : base(id)
        {
            OldSetting = currentSetting;
        }

    }
}
