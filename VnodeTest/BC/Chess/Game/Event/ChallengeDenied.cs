using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Chess.Game.Chessgame>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.General.Account.Account>;

namespace VnodeTest.BC.Chess.Game.Event
{
    public class ChallengeDenied : AggregateEvent<Chessgame>
    {
        public ChallengeDenied(GameID id) : base(id)
        {
        }

    }
}
