using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.General
{
    class GeneralProjection : Projection
    {
        public GeneralProjection(IEventStore store, IMessageBus bus) : base(store, bus)
        {
        }

        //private void On(event @event)
        //{
        //    action
        //}
    }
}
