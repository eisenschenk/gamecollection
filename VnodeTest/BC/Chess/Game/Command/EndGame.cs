using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Chess.Game.Chessgame>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.General.Account.Account>;
using ACL.ES;

namespace VnodeTest.BC.Chess.Game.Command
{
    public class EndGame : AggregateCommand<Chessgame>
    {
        public string Moves { get; }

        public EndGame(GameID id, string moves) : base(id)
        {
            Moves = moves;
        }
    }
}
