﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.Solitaire.GameEntities
{
    public class TableauStack : BaseStack
    {
        public TableauStack(IEnumerable<Card> collection)
            : base(collection) { }

        public TableauStack() { }

        public override bool CanPush(Card card)
        {
            return false;
        }
    }
}
