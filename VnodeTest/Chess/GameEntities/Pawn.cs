using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.Chess;
using VnodeTest.Chess.GameEntities;

namespace VnodeTest.GameEntities
{
    class Pawn : Piece
    {

        public Pawn((int X, int Y) position, PieceColor color, PieceValue pieceValue, (int X, int Y) startposition, bool hasmoved)
            : base(position, color, pieceValue, startposition, hasmoved)
        {
        }

        protected override IEnumerable<(int X, int Y)> GetPotentialMovements(ChessBoard gameboard)
        {
            int possibleMove = (StartPosition == Position) ? 2 : 1;
            bool enemyPiece((int X, int Y) position) => gameboard[position] != null && gameboard[position].Color != Color;
            bool filterY(int y, int positionY) => Color == PieceColor.White ? y > positionY : y < positionY;

            //preventing movement to the left/right, movement straight blocked by any piece, capturing only diagonal including enpassant
            var returnValues = GetStraightLines(gameboard, possibleMove).Where(p => p.X == Position.X && gameboard[p] == null);
            return returnValues.Concat(GetDiagonals(gameboard, 1).Where(p => filterY(p.Y, Position.Y) && enemyPiece(p) || p == gameboard.EnPassantTarget));

        }
    }
}

