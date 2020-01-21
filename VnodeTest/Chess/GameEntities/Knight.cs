using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.Chess;
using VnodeTest.Chess.GameEntities;

namespace VnodeTest.GameEntities
{
    //TODO:
    class Knight : Piece
    {
        public Knight((int X, int Y) position, PieceColor color, PieceValue pieceValue, string sprite, (int X, int Y) startposition, bool hasmoved)
            : base(position, color, pieceValue, sprite, startposition, hasmoved)
        {
        }

        protected override IEnumerable<(int X, int Y)> GetPotentialMovements(ChessBoard gameboard)
        {
            var returnValues = new List<(int X, int Y)>(8);
            for (int index = -1; index < 2; index += 2)
            {
                //getting moves to the left and then to the right of the knight
                returnValues.Add((Position.X + index, Position.Y - 2));
                returnValues.Add((Position.X + index, Position.Y + 2));
                returnValues.Add((Position.X - 2, Position.Y + index));
                returnValues.Add((Position.X + 2, Position.Y + index));
            }
            return returnValues.Where(p => p.X >= 0 && p.X < 8 && p.Y >= 0 && p.Y < 8
            && (gameboard[p] == null || gameboard[p].Color != Color));
        }

        //public override Piece Copy() => new Knight(Position, Color);
    }
}
