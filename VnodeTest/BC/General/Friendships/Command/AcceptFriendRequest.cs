using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.General.Friendships.Command
{
    public class AcceptFriendRequest : AggregateCommand<Friendship>
    {
        public AcceptFriendRequest(AggregateID<Friendship> id) : base(id)
        {
        }

    }
}

