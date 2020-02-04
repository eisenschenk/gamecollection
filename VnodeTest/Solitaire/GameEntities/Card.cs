using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.Solitaire.GameEntities
{
    public partial class Card
    {
        public CardValue CardValue { get; }
        public PipValue PipValue { get; }
        public ColorValue Color { get; }
        public bool IsFaceUp { get; set; }

        public Card(int carDeckIndex)
        {
            CardValue = GetCardValue(carDeckIndex);
            PipValue = GetPipValue(carDeckIndex);
            Color = GetColorValue(PipValue);
        }

        private CardValue GetCardValue(int cardID)
        {
            cardID %= 13;
            return cardID switch
            {
                0 => CardValue.Ace,
                1 => CardValue.Two,
                2 => CardValue.Three,
                3 => CardValue.Four,
                4 => CardValue.Five,
                5 => CardValue.Six,
                6 => CardValue.Seven,
                7 => CardValue.Eight,
                8 => CardValue.Nine,
                9 => CardValue.Ten,
                10 => CardValue.Jack,
                11 => CardValue.Queen,
                12 => CardValue.King,
                _ => CardValue.Zero,
            };
        }

        private PipValue GetPipValue(int cardID)
        {
            cardID /= 13;
            return cardID switch
            {
                0 => PipValue.Club,
                1 => PipValue.Spade,
                2 => PipValue.Heart,
                3 => PipValue.Diamond,
                _ => PipValue.Zero,
            };
        }

        private ColorValue GetColorValue(PipValue pipValue)
        {
            return ((int)pipValue / 2 == 0) ? ColorValue.Black : ColorValue.Red;
        }
    }
}
