using GameID = ACL.ES.AggregateID<VnodeTest.BC.Solitaire.Solitaire>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.General.Account.Account>;

namespace VnodeTest.BC.Solitaire.Event
{
    public class GameEnded : AggregateEvent<Solitaire>
    {
        public AccountID AccountID { get; }
        public int Score { get; }

        public GameEnded(GameID id, AccountID accountID, int score) : base(id)
        {
            AccountID = accountID;
            Score = score;
        }

    }
}
