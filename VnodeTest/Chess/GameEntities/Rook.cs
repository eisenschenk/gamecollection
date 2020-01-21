using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.Chess;
using VnodeTest.Chess.GameEntities;

namespace VnodeTest.GameEntities
{
    class Rook : Piece
    {
        public Rook((int X, int Y) position, PieceColor color, PieceValue pieceValue, string sprite, (int X, int Y) startposition, bool hasmoved)
            : base(position, color, pieceValue, sprite, startposition, hasmoved)
        {
        }

        protected override IEnumerable<(int X, int Y)> GetPotentialMovements(ChessBoard gameboard)
        {
            return GetStraightLines(gameboard);
        }

        //public override Piece Copy() => new Rook(Position, Color);
    }
}
