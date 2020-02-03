using ACL.ES;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Solitaire.Solitaire>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.General.Account.Account>;

namespace VnodeTest.BC.Solitaire.Command
{
    public class OpenGame : AggregateCommand<Solitaire>
    {
        public AccountID AccountID { get; }
        public OpenGame(GameID id, AccountID accountID) : base(id)
        {
            AccountID = accountID;
        }

    }
}
