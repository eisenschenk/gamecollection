using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.BC.Chess;
using VnodeTest.Chess;

namespace VnodeTest
{
    public class ChessContext
    {
        private readonly ChessProjection ChessProjection;
        private readonly BC.Chess.Chess.Handler ChessHandler;

        private readonly IRepository Repository;

        public ChessContext()
        {
            string storePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "chess.db");

            Type tEvent = typeof(IEvent);
            Type tTO = typeof(ITransferObject);
            var allTypes = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            var knownTypes = allTypes
                .Where(t => t.Namespace.StartsWith($"{nameof(VnodeTest)}.{nameof(BC)}.{nameof(Chess)}"))
                .Where(t => !t.IsAbstract)
                .Where(t => t.IsClass || t.IsValueType)
                .Where(t => tEvent.IsAssignableFrom(t) || tTO.IsAssignableFrom(t)).ToArray();

            JSONFileEventStore store = new JSONFileEventStore(storePath, knownTypes);
            Repository = new Repository(store);
            var bus = MessageBus.Instance;

            ChessProjection = new ChessProjection(store, bus);
            ChessProjection.Init();
            ChessHandler = new BC.Chess.Chess.Handler(Repository, bus);
        }

        public ChessController CreateChessController() =>
            new ChessController();
    }
}
