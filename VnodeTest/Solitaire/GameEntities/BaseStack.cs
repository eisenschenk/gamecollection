using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.Solitaire.GameEntities
{
    public abstract class BaseStack : Stack<Card>
    {
        public bool IsEmpty => !this.Any();

        public BaseStack() { }
        public BaseStack(IEnumerable<Card> collection) : base(collection) { }

        public bool TryPush(Card sourceCard, BaseStack sourceStack)
        {
            //tempstack are the selected card and all cards on top, only needed due to using stacks
            CardStack getTempStack(Card card)
            {
                var tempStack = new CardStack();
                do tempStack.Push(sourceStack.Pop());
                while (tempStack.Peek() != card);

                return tempStack;
            }
            if (CanPush(sourceCard))
            {
                var tempStack = getTempStack(sourceCard);
                while (tempStack.Count != 0)
                    Push(tempStack.Pop());
                return true;
            }
            return false;
        }

        public void PushToEmptyStack(Gameboard cards, Card selected)
        {
            TryPush(selected, cards.GetStack(selected));
        }

        public abstract bool CanPush(Card card);
    }
}
