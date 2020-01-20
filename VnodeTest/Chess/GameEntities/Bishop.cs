using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.Chess.GameEntities;
using static VnodeTest.Chess.Enums;

namespace VnodeTest.GameEntities
{
    class Bishop : Piece
    {
        public Bishop((int X, int Y) position, PieceColor color) : base(position, color)
        {
            Value = PieceValue.Bishop;
        }

        protected override IEnumerable<(int X, int Y)> GetPotentialMovements(ChessBoard gameboard)
        {
            return GetDiagonals(gameboard);
        }

        public override Piece Copy() => new Bishop(Position, Color);
    }
}
