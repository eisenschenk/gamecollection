using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using VnodeTest.BC.General;

namespace VnodeTest
{
    public class GeneralContext
    {
        private readonly GeneralProjection GeneralProjection;
        private readonly General.Handler GeneralHandler;

        private readonly IRepository Repository;

        public GeneralContext()
        {
            string storePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "general.db");

            Type tEvent = typeof(IEvent);
            Type tTO = typeof(ITransferObject);
            var allTypes = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            var knownTypes = allTypes
                .Where(t => !t.IsAbstract)
                .Where(t => t.IsClass || t.IsValueType)
                .Where(t => tEvent.IsAssignableFrom(t) || tTO.IsAssignableFrom(t)).ToArray();

            JSONFileEventStore store = new JSONFileEventStore(storePath, knownTypes);
            Repository = new Repository(store);
            var bus = MessageBus.Instance;

            GeneralProjection = new GeneralProjection(store, bus);
            GeneralProjection.Init();
            GeneralHandler = new General.Handler(Repository, bus);
        }

        public GeneralController CreateGeneralController() =>
            new GeneralController();
    }

}
