using GameID = ACL.ES.AggregateID<VnodeTest.BC.Solitaire.Solitaire>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.General.Account.Account>;
using ACL.ES;

namespace VnodeTest.BC.Solitaire.Command
{
    public class EndGame : AggregateCommand<Solitaire>
    {
        public AccountID AccountID { get; }
        public int Score { get; }

        public EndGame(GameID id, AccountID accountID, int score) : base(id)
        {
            AccountID = accountID;
            Score = score;
        }

    }
}