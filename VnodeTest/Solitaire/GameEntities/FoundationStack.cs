using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.Solitaire.GameEntities
{
    public class FoundationStack : BaseStack
    {
        public PipValue PipValue { get; }

        public FoundationStack(PipValue pipValue)
        {
            PipValue = pipValue;
        }

        public override bool CanPush(Card card)
        {
            if (card == null)
                return false;
            if (PipValue != card.PipValue)
                return false;
            if ((IsEmpty && card.CardValue == CardValue.Ace) || !IsEmpty && (Peek().CardValue == card.CardValue - 1))
                return true;
            return false;
        }

    }
}
