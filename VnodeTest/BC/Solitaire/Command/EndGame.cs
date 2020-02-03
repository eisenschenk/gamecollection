using GameID = ACL.ES.AggregateID<VnodeTest.BC.Solitaire.Solitaire>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.General.Account.Account>;
using ACL.ES;

namespace VnodeTest.BC.Solitaire.Command
{
    public class EndGame : AggregateCommand<Solitaire>
    {
        public AccountID AccountID { get; }
        public EndGame(GameID id, AccountID accountID) : base(id)
        {
            AccountID = accountID;
        }

    }
}