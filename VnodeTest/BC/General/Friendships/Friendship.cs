using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.General.Friendships
{
    public class Friendship : AggregateRoot<Friendship>
    {
        public class Handler : AggregateCommandHandler<Friendship>
        {
            public Handler(IRepository repository, IMessageBus bus) : base(repository, bus)
            {
            }
        }

        public static class Commands
        {

        }

        //public IEnumerable<IEvent> On(Command command)
        //{
        //    yield return new Event
        //}

        public override void Apply(IEvent @event)
        {
            switch (@event)
            {

            }
        }
    }
}