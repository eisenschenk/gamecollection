using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Chess.Game.Chessgame>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.General.Account.Account>;

namespace VnodeTest.BC.Chess.Game.Event
{
    public class UnwantedChallengesDeleted : AggregateEvent<Chessgame>
    {
        public UnwantedChallengesDeleted(GameID id, AccountID receiver, AccountID challenger) : base(id)
        {
            Receiver = receiver;
            Challenger = challenger;
        }

        public AccountID Receiver { get; }
        public AccountID Challenger { get; }
    }
}
