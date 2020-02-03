using GameID = ACL.ES.AggregateID<VnodeTest.BC.Solitaire.Solitaire>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.General.Account.Account>;
using ACL.ES;
using System;
using VnodeTest.BC.General.Account;

namespace VnodeTest.BC.Solitaire.Event
{
    public class GameOpened : AggregateEvent<Solitaire>
    {
       
        public AccountID AccountID { get; }

        public GameOpened(GameID id, AccountID accountID) : base(id)
        {
            AccountID = accountID;
        }

    }
}
