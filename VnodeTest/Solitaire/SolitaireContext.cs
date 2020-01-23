using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.BC.Solitaire;
using VnodeTest.Solitaire;

namespace VnodeTest
{
    public class SolitaireContext
    {
        private readonly SolitaireProjection SolitaireProjection;
        private readonly BC.Solitaire.Solitaire.Handler SolitaireHandler;

        private readonly IRepository Repository;

        public SolitaireContext()
        {
            string storePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "solitaire.db");

            Type tEvent = typeof(IEvent);
            Type tTO = typeof(ITransferObject);
            var allTypes = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            var knownTypes = allTypes
                .Where(t => !t.IsAbstract)
                .Where(t => t.IsClass || t.IsValueType)
                .Where(t => tEvent.IsAssignableFrom(t) || tTO.IsAssignableFrom(t))
                .Where(t => t.Namespace.StartsWith($"{nameof(VnodeTest)}.{nameof(BC)}.{nameof(Solitaire)}")).ToArray();


            JSONFileEventStore store = new JSONFileEventStore(storePath, knownTypes);
            Repository = new Repository(store);
            var bus = MessageBus.Instance;

            SolitaireProjection = new SolitaireProjection(store, bus);
            SolitaireProjection.Init();
            SolitaireHandler = new BC.Solitaire.Solitaire.Handler(Repository, bus);
        }

        public SolitaireController CreateSolitaireController() =>
            new SolitaireController();
    }
}
