using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VnodeTest.Solitaire.GameEntities.Card;

namespace VnodeTest.Solitaire.GameEntities
{
    public class Gameboard
    {
        public Tableau Tableau { get; }
        public Foundations Foundations { get; } = new Foundations();
        public CardStack[] GamePiles { get; } = new CardStack[7];
        private readonly Random Random = new Random();
        public int Score => GetScore();

        public Gameboard()
        {
            var cards = Enumerable.Range(0, 52)
               .Select(x => (Weight: Random.Next(), Card: new Card(x)))
               .OrderBy(x => x.Weight)
               .Select(c => c.Card);

            Tableau = new Tableau(cards);

            for (int index = 0; index < GamePiles.Count(); index++)
                GamePiles[index] = new CardStack();

            DealCards();
        }

        public void DealCards()
        {
            for (int pileNumber = 0; pileNumber < 7; pileNumber++)
                for (int index = 0; index < pileNumber + 1; index++)
                {
                    Tableau.TableauSource.Peek().IsFaceUp = index == pileNumber;
                    GamePiles[pileNumber].Push(Tableau.TableauSource.Pop());
                }
        }
        public BaseStack GetStack(Card card)
        {
            //GamePiles
            foreach (CardStack stack in GamePiles)
                if (stack.Contains(card))
                    return stack;
            //Foundations
            if (Foundations.Club.Contains(card))
                return Foundations.Club;
            else if (Foundations.Spade.Contains(card))
                return Foundations.Spade;
            else if (Foundations.Heart.Contains(card))
                return Foundations.Heart;
            else if (Foundations.Diamond.Contains(card))
                return Foundations.Diamond;
            //Tableau
            if (Tableau.TableauGraveyard.Contains(card))
                return Tableau.TableauGraveyard;

            return null;
        }

        private int GetScore()
        {
            //score for all cards put to Foundations
            var score = Foundations.Club.Count + Foundations.Spade.Count + Foundations.Heart.Count + Foundations.Diamond.Count;
            //factor per card
            score *= 10;
            // point reduction per full cycle of cards shown from Tableau
            return score -= Tableau.GraveyardTurnedOverCounter * 20;
        }
    }
}
