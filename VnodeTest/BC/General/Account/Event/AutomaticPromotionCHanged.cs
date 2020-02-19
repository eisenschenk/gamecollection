using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACL.ES;

namespace VnodeTest.BC.General.Account.Event
{
    public class AutomaticPromotionChanged : AggregateEvent<Account>
    {
        public bool OldSetting { get; }
        public bool NewSetting => !OldSetting;
        public AutomaticPromotionChanged(AggregateID<Account> id, bool currentSetting) : base(id)
        {
            OldSetting = currentSetting;
        }

    }
}
