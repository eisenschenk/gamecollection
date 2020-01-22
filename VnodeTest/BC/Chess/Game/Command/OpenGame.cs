using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Chess.Game.Chessgame>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.General.Account.Account>;
using VnodeTest.Chess;
using ACL.ES;

namespace VnodeTest.BC.Chess.Game.Command
{
    public class OpenGame : AggregateCommand<Chessgame>
    {
        public Gamemode Gamemode { get; }
        public double Clocktimer { get; }

        public OpenGame(GameID id, Gamemode gamemode, double clocktimer) : base(id)
        {
            Gamemode = gamemode;
            Clocktimer = clocktimer;
        }
    }
}
