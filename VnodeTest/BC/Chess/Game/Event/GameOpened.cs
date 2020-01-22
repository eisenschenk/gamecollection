using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Chess.Game.Chessgame>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.General.Account.Account>;
using VnodeTest.Chess;

namespace VnodeTest.BC.Chess.Game.Event
{
    public class GameOpened : AggregateEvent<Chessgame>
    {
        public Gamemode Gamemode { get; }
        public double Clocktimer { get; }

        public GameOpened(GameID id, Gamemode gamemode, double clocktimer) : base(id)
        {
            Gamemode = gamemode;
            Clocktimer = clocktimer;
        }
    }
}