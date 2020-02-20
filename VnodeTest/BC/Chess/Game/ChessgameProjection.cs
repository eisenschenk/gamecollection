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
using System.Threading;
using VnodeTest.BC.Chess.Game.Event;

namespace VnodeTest.BC.Chess.Game
{
    public class ChessgameProjection : Projection
    {
        private readonly Dictionary<GameID, GameEntry> Dict = new Dictionary<GameID, GameEntry>();

        public GameEntry this[GameID id] => id != default ? Dict[id] : null;
        public IEnumerable<GameEntry> Games => Dict.Values;

        public ChessgameProjection(IEventStore store, IMessageBus bus) : base(store, bus)
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                while (true)
                {
                    foreach (GameEntry entry in Games.ToArray())
                        if (!entry.GameWasFullOnce && entry.Created.AddSeconds(entry.Timer) < DateTime.Now)
                            Chessgame.Commands.DeleteGame(entry.ID);
                    Thread.Sleep(1000);
                }
            });
        }

#pragma warning disable IDE0051
        private void On(GameOpened @event)
        {
            Dict.Add(@event.ID, new GameEntry(@event.ID, @event.Gamemode, this, @event.Clocktimer));
        }
        private void On(ChallengeRequested @event)
        {
            Dict[@event.ID].Challenger = @event.AccountID;
            Dict[@event.ID].Receiver = @event.FriendID;
        }
        private void On(ChallengeAccepted @event)
        {
            //TOASK: can i use commands here? no
            Dict[@event.ID].Receiver = default;
            Dict[@event.ID].Challenger = default;

        }
        private void On(UnwantedChallengesDeleted @event)
        {
            var gameIDs = Dict.Values.Where(x => x.ID != @event.ID && (x.Receiver == @event.Challenger || x.Receiver == @event.Receiver
                || x.Challenger == @event.Challenger || x.Challenger == @event.Receiver)).Select(f => f.ID);
            foreach (GameID id in gameIDs.ToArray())
                Dict.Remove(id);
        }
        private void On(GameDeleted @event)
        {
            Dict.Remove(@event.ID);
        }
        private void On(GameEnded @event)
        {
            Dict[@event.ID].AllMoves = @event.Moves;
            Dict[@event.ID].Closed = true;
            Dict[@event.ID].ClosedTimestamp = DateTime.Now;
        }
        private void On(ChallengeDenied @event)
        {
            Dict.Remove(@event.ID);
        }
        private void On(GameJoined @event)
        {
            var entry = Dict[@event.ID];
            if (@event.AccountID != entry.Receiver && entry.PlayerWhite == default)
                entry.PlayerWhite = @event.AccountID;
            else
                entry.PlayerBlack = @event.AccountID;
        }
#pragma warning restore
        public GameID GetGameID(AccountID accountID)
        {
            return Dict.Values.ToArray().Where(a => !a.Closed && (a.PlayerWhite == accountID || a.PlayerBlack == accountID)).FirstOrDefault()?.ID ?? default;
        }

        public PieceColor GetOpenGamePlayerColor(AccountID accountID)
        {
            var game = Dict.Values.Where(c => !c.Closed && (c.PlayerWhite == accountID || c.PlayerBlack == accountID)).FirstOrDefault();
            if (game != null)
                return game.PlayerWhite == accountID ? PieceColor.White : PieceColor.Black;
            return PieceColor.Default;
        }

        public PieceColor GetSpecificPlayerColor(AccountID accountID, VnodeTest.Game game)
        {
            if (game != default)
                return Dict[game.ID].PlayerBlack == accountID ? PieceColor.Black : PieceColor.White;
            return PieceColor.Default;
        }

        public VnodeTest.Game GetLastPlayedGame(AccountID accountID)
        {
            var timenow = DateTime.Now;
            var gameID = Dict.Values.Where(g => (g.PlayerBlack == accountID || g.PlayerWhite == accountID) && g.Closed)
                .OrderBy(o => timenow - o.ClosedTimestamp).FirstOrDefault().ID;
            return gameID != default ? Dict[gameID].Game : default;
        }
    }


    public class GameEntry
    {
        public GameID ID { get; }
        public Gamemode Gamemode { get; }
        public string AllMoves;
        public bool LoggedIn;
        public DateTime Created = DateTime.Now;
        public DateTime ClosedTimestamp;
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
        public bool HasBlackPlayer => Gamemode == Gamemode.PvE || Gamemode == Gamemode.EvE ? true : PlayerBlack != default;
        public bool HasWhitePlayer => Gamemode == Gamemode.EvE ? true : PlayerWhite != default;
        public bool GameWasFullOnce;
        public bool HasOpenSpots => GameHasOpenSpots();


        public GameEntry(GameID id, Gamemode gamemode, ChessgameProjection chessgameProjection, double playerClockTime = 50000)
        {
            ID = id;
            Gamemode = gamemode;
            Game = new VnodeTest.Game(id, gamemode, new ChessBoard(), playerClockTime, chessgameProjection);
        }

        private bool GameHasOpenSpots()
        {
            if (!HasBlackPlayer || !HasWhitePlayer)
                return true;
            GameWasFullOnce = true;
            return false;
        }
    }
}
