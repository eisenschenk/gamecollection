using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.Chess.GameEntities;
using static VnodeTest.Chess.Enums;

namespace VnodeTest.GameEntities
{
    class Rook : Piece
    {
        public Rook((int X, int Y) position, PieceColor color) : base(position, color)
        {
            Value = PieceValue.Rook;
        }

        protected override IEnumerable<(int X, int Y)> GetPotentialMovements(ChessBoard gameboard)
        {
            return GetStraightLines(gameboard);
        }

        public override Piece Copy() => new Rook(Position, Color);
    }
}
