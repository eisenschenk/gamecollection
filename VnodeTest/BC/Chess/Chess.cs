﻿using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.BC.Chess
{
    public class Chess : AggregateRoot<Chess>
    {
        public class Handler : AggregateCommandHandler<Chess>
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
