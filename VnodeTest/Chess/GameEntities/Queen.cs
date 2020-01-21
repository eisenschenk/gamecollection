using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.Chess;
using VnodeTest.Chess.GameEntities;

namespace VnodeTest.GameEntities
{
    class Queen : Piece
    {
        public Queen((int X, int Y) position, PieceColor color, PieceValue pieceValue, (int X, int Y) startposition, bool hasmoved)
            : base(position, color, pieceValue, startposition, hasmoved)
        {
        }

        protected override IEnumerable<(int X, int Y)> GetPotentialMovements(ChessBoard gameboard)
        {
            return GetDiagonals(gameboard).Concat(GetStraightLines(gameboard));
        }

        //public override Piece Copy() => new Queen(Position, Color);
    }
}
