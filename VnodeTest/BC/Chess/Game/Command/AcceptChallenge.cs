using ACL.ES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Chess.Game.Chessgame>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.General.Account.Account>;

namespace VnodeTest.BC.Chess.Game.Command
{
    public class AcceptChallenge : AggregateCommand<Chessgame>
    {
        public AccountID AccountID { get; }
        public AccountID FriendID { get; }
        public AcceptChallenge(GameID id, AccountID accountID, AccountID friendID) : base(id)
        {
            AccountID = accountID;
            FriendID = friendID;
        }

    }
}
