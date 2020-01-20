using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.Chess.GameEntities;
using static VnodeTest.Chess.Enums;

namespace VnodeTest.GameEntities
{
    class Queen : Piece
    {
        public Queen((int X, int Y) position, PieceColor color) : base(position, color)
        {
            Value = PieceValue.Queen;
        }

        protected override IEnumerable<(int X, int Y)> GetPotentialMovements(ChessBoard gameboard)
        {
            return GetDiagonals(gameboard).Concat(GetStraightLines(gameboard));
        }

        public override Piece Copy() => new Queen(Position, Color);
    }
}
