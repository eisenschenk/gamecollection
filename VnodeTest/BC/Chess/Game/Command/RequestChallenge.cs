using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Chess.Game.Chessgame>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.General.Account.Account>;
using System.Threading.Tasks;
using ACL.ES;

namespace VnodeTest.BC.Chess.Game.Command
{
    public class RequestChallenge : AggregateCommand<Chessgame>
    {
        public AccountID AccountID { get; }
        public AccountID FriendID { get; }

        public RequestChallenge(GameID id, AccountID accountID, AccountID friendID) : base(id)
        {
            AccountID = accountID;
            FriendID = friendID;
        }

    }
}