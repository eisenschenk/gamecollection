using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Solitaire
{
    class SolitaireProjection : Projection
    {
        public SolitaireProjection(IEventStore store, IMessageBus bus) : base(store, bus)
        {
        }
#pragma warning disable IDE0051

        //private void On(event @event)
        //{
        //    action
        //}
#pragma warning restore
    }
}
