using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.Chess;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Chess.Game.Chessgame>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.General.Account.Account>;
using VnodeTest.Chess.GameEntities;

namespace VnodeTest.BC.Chess.Game
{
    public class ChessgameProjection : Projection
    {
        private readonly Dictionary<GameID, GameEntry> Dict = new Dictionary<GameID, GameEntry>();

        public GameEntry this[GameID id] => Dict[id];
        public IEnumerable<GameEntry> Games => Dict.Values;

        public ChessgameProjection(IEventStore store, IMessageBus bus) : base(store, bus)
        {
        }

        //private void On(event @event)
        //{
        //    action
        //}
    }

    public class GameEntry
    {
        public GameID ID { get; }
        public Gamemode Gamemode { get; }
        public string AllMoves;
        public bool LoggedIn;
        public DateTime Created = DateTime.Now;
        public int Timer = 30;
        public TimeSpan Elapsed => DateTime.Now - Created;
        public AccountID Challenger { get; set; }
        public AccountID Receiver { get; set; }
        public AccountID PlayerWhite { get; set; }
        public AccountID PlayerBlack { get; set; }
        public VnodeTest.Game Game { get; set; }
        public bool Closed { get; set; }
        public bool GameOver => Winner.HasValue;
        public PieceColor? Winner => Game?.Winner;
        //TODO: think about this again, too many different/same playerwhite/black
        //public bool HasBlackPlayer { get; set; }
        //public bool GameWasFullOnce;
        //public bool HasWhitePlayer { get; set; }
        //public bool HasOpenSpots => !HasBlackPlayer || !HasWhitePlayer;
        //public bool IsEmpty => GameEmpty();


        public GameEntry(GameID id, Gamemode gamemode, double playerClockTime = 50000)
        {
            ID = id;
            Gamemode = gamemode;
            Game = new VnodeTest.Game(id, gamemode, new ChessBoard(), playerClockTime);
        }
    }
}
