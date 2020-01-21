using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VnodeTest.Chess.GameEntities
{
    public class ImmutableBoard
    {
        private readonly Piece[] Board;


        public Piece this[(int X, int Y) position]
        {
            get => Board[position.Y * 8 + position.X];
        }

        public ImmutableBoard(Piece[] pieces)
        {
            Board = pieces;
        }

        public ImmutableBoard Move(Piece start, (int X, int Y) target)
        {
            Piece[] pieces = new Piece[64];
            Board.CopyTo(pieces, 0);

            pieces[To1D(target)] = start.Move(target);
            pieces[To1D(start.Position)] = null;

            return new ImmutableBoard(pieces);
        }


        public (int X, int Y) To2D(int index)
        {
            return (index % 8, index / 8);
        }
        public int To1D((int X, int Y) position)
        {
            return (position.X + position.Y * 8);
        }
    }

}
