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
    }

    public class GameEntry
    {
        public Gameboard GameBoard { get; private set; } = new Gameboard();
        public GameID ID { get; }
        public AccountID Player { get; set; }
        public bool Closed { get; set; }
        public DateTimeOffset DateCreated { get; }

        public GameEntry(GameID gameID, DateTimeOffset dateCreated)
        {
            ID = gameID;
            DateCreated = dateCreated;
        }
    }
}
