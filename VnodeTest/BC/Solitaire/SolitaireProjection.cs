using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.Solitaire.GameEntities;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Solitaire.Solitaire>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.General.Account.Account>;
using VnodeTest.BC.Solitaire.Event;
using static VnodeTest.Solitaire.SolitaireController;

namespace VnodeTest.BC.Solitaire
{
    public class SolitaireProjection : Projection
    {
        private readonly Dictionary<GameID, GameEntry> Dict = new Dictionary<GameID, GameEntry>();

        public GameEntry this[GameID id] => id != default ? Dict[id] : null;
        public IEnumerable<GameEntry> Games => Dict.Values;

        public SolitaireProjection(IEventStore store, IMessageBus bus) : base(store, bus)
        {
        }
#pragma warning disable IDE0051

        private void On(GameOpened @event)
        {
            Dict.Add(@event.ID, new GameEntry(@event.ID, @event.Timestamp));
        }

        private void On(GameEnded @event)
        {
            Dict[@event.ID].Closed = true;
            Dict[@event.ID].FinalScore = @event.Score;
        }

        private void On(Gamejoined @event)
        {
            Dict[@event.ID].Player = @event.AccountID;
        }
#pragma warning restore

        public GameID GetGameID(AccountID accountID)
        {
            return Dict.Values.ToArray().Where(a => !a.Closed && a.Player == accountID).FirstOrDefault()?.ID ?? default;
        }

        public GameID GetLastPlayedGame(AccountID accountID)
        {
            var gameid = Dict.Values.ToArray().Where(v => v.Player == accountID).OrderByDescending(x => x.DateCreated).Select(g => g.ID).FirstOrDefault();
            if (!Dict.ContainsKey(gameid) || Dict[gameid].Closed)
                return default;
            return gameid;
        }

        public IEnumerable<GameEntry> GetGlobalHighscore(ScoreTimespan scoreTimespan, int count = 10)
        {
            return Dict?.Values.Where(t => t.DateCreated > GetTime(scoreTimespan)).OrderByDescending(s => s.FinalScore).Take(count);
        }

        public IEnumerable<GameEntry> GetPersonalHighscore(ScoreTimespan scoreTimespan, AccountID accountID, int count = 10)
        {
            return Dict?.Values.Where(g => g.Player == accountID && g.DateCreated > GetTime(scoreTimespan)).OrderByDescending(s => s.FinalScore).Take(count);
        }

        private DateTime GetTime(ScoreTimespan scoreTimespan)
        {
            return scoreTimespan switch
            {
                ScoreTimespan.Alltime => new DateTime(1900, 12, 24),
                ScoreTimespan.ThreeMonths => DateTime.Now - new TimeSpan(90, 0, 0, 0),
                ScoreTimespan.TwelveMonths => DateTime.Now - new TimeSpan(365, 0, 0, 0),
            };
        }
    }

    public class GameEntry
    {
        public Gameboard GameBoard { get; set; } = new Gameboard();
        public GameID ID { get; }
        public AccountID Player { get; set; }
        public bool Closed { get; set; }
        public DateTimeOffset DateCreated { get; }
        public int FinalScore { get; set; }

        public GameEntry(GameID gameID, DateTimeOffset dateCreated)
        {
            ID = gameID;
            DateCreated = dateCreated;
        }
    }
}
