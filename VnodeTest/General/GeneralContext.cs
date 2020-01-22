using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VnodeTest.BC.General.Account;
using VnodeTest.BC.General.Friendships;

namespace VnodeTest.General
{
    public class GeneralContext
    {
        public AccountProjection AccountProjection { get; }
        public Account.Handler AccountHandler { get; }
        public FriendshipProjection FriendshipProjection { get; }
        public Friendship.Handler FriendshipHandler { get; }

        private readonly IRepository Repository;

        public GeneralContext()
        {
            string storePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "general.db");

            Type tEvent = typeof(IEvent);
            Type tTO = typeof(ITransferObject);
            var allTypes = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            var knownTypes = allTypes
                .Where(t => t.Namespace.StartsWith($"{nameof(VnodeTest)}.{nameof(BC)}.{nameof(General)}"))
                .Where(t => !t.IsAbstract)
                .Where(t => t.IsClass || t.IsValueType)
                .Where(t => tEvent.IsAssignableFrom(t) || tTO.IsAssignableFrom(t)).ToArray();

            JSONFileEventStore store = new JSONFileEventStore(storePath, knownTypes);
            Repository = new Repository(store);
            var bus = MessageBus.Instance;

            AccountProjection = new AccountProjection(store, bus);
            AccountProjection.Init();
            AccountHandler = new Account.Handler(Repository, bus);

            FriendshipProjection = new FriendshipProjection(store, bus);
            FriendshipProjection.Init();
            FriendshipHandler = new Friendship.Handler(Repository, bus);
        }

        public AccountController CreateLoginController() =>
            new AccountController(AccountProjection);

        public FriendshipController CreateFriendshipController(AccountEntry accountEntry) =>
            new FriendshipController(accountEntry, AccountProjection, ((Application)Application.Instance).ChessContext.ChessProjection, FriendshipProjection);
    }

        //public (JSONFileEventStore Store, Repository Repository) CreateStore(string storename, string nameSpace)
        //{
        //    string storePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), storename);

        //    Type tEvent = typeof(IEvent);
        //    Type tTO = typeof(ITransferObject);
        //    var allTypes = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
        //    var knownTypes = allTypes
        //        .Where(t => t.Namespace.StartsWith($"{nameof(VnodeTest)}.{nameof(BC)}.{nameof(nameSpace)}"))
        //        .Where(t => !t.IsAbstract)
        //        .Where(t => t.IsClass || t.IsValueType)
        //        .Where(t => tEvent.IsAssignableFrom(t) || tTO.IsAssignableFrom(t)).ToArray();

        //    JSONFileEventStore store = new JSONFileEventStore(storePath, knownTypes);
        //    var repository = new Repository(store);
        //    var bus = MessageBus.Instance;

        //    return (store, repository);
        //}
        //public GeneralContext()
        //{
        //    var accountStore = CreateStore("accounts", nameof(General));
        //    AccountProjection = new AccountProjection(accountStore.Store, MessageBus.Instance);
        //    AccountProjection.Init();
        //    AccountHandler = new Account.Handler(accountStore.Repository, MessageBus.Instance);
        //}


}
