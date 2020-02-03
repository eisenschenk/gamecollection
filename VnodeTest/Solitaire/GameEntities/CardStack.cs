using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.Solitaire.GameEntities;

namespace VnodeTest.Solitaire.GameEntities
{
    public class CardStack : BaseStack
    {
        public CardStack(IEnumerable<Card> collection)
            : base(collection) { }

        public CardStack() { }

        public override bool CanPush(Card card)
        {
            if (card == null)
                return false;
            if (IsEmpty && card.CardValue == CardValue.King)
                return true;
            if (!IsEmpty && Peek().Color != card.Color && Peek().CardValue == card.CardValue + 1)
                return true;

            return false;
        }
    }
}
