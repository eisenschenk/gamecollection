using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.Solitaire.GameEntities
{
    public class Tableau
    {
        public TableauStack TableauSource { get; }
        public TableauStack TableauGraveyard { get; }
        public int GraveyardTurnedOverCounter { get; private set; }
        public Tableau(IEnumerable<Card> collection)
        {
            TableauSource = new TableauStack(collection);
            TableauGraveyard = new TableauStack();
        }
        public void NextCard()
        {
            if (TableauSource.Count != 0)
            {
                //old topcard is put face down
                if (!TableauGraveyard.IsEmpty)
                    TableauGraveyard.Peek().IsFaceUp = false;
                //moving card from source to graveyard
                TableauGraveyard.Push(TableauSource.Pop());
                //new topcard is put face up
                TableauGraveyard.Peek().IsFaceUp = true;
            }
            else
            {
                //pushing until graveyard is empty, increasing GraveyardTurnedOver count to later reduce points based on this number
                while (TableauGraveyard.Count != 0)
                    TableauSource.Push(TableauGraveyard.Pop());
                GraveyardTurnedOverCounter++;
            }
        }
    }
}
