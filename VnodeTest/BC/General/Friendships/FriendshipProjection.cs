using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FriendshipID = ACL.ES.AggregateID<VnodeTest.BC.General.Friendships.Friendship>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.Account.Account>;



namespace VnodeTest.BC.General.Friendships
{
    public class FriendshipProjection : Projection
    {
        public FriendshipProjection(IEventStore store, IMessageBus bus) : base(store, bus)
        {
        }

        //private void On(event @event)
        //{
        //    action
        //}
    }
    public class FriendshipEntry
    {
        public FriendshipID ID { get; }
        public AccountID Sender;
        public AccountID Receiver;
        public bool Accepted;
        public bool Requested;

        public FriendshipEntry(FriendshipID id, AccountID friendA, AccountID friendB)
        {
            ID = id;
            Sender = friendA;
            Receiver = friendB;
        }

    }
}
