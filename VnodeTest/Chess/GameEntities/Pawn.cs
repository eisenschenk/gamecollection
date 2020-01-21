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
            Func<(int X, int Y), bool> enemyPiece = position => gameboard[position] != null && gameboard[position].Color != Color;
            if (Color == PieceColor.White)
            {
                //preventing movement to the left/right, movement straight blocked by any piece, capturing only diagonal including enpassant
                var returnValues = GetStraightLines(gameboard, possibleMove).Where(p => p.Y == Position.Y && gameboard[p] == null);
                return returnValues.Concat(GetDiagonals(gameboard, 1).Where(x => x.Y < Position.Y && enemyPiece(x) || x == gameboard.EnPassantTarget));
            }
            else
            {
                //preventing movement to the left/right, movement straight blocked by any piece, capturing only diagonal including enpassant
                var returnValues = GetStraightLines(gameboard, possibleMove).Where(p => p.Y == Position.Y && gameboard[p] == null);
                return returnValues.Concat(GetDiagonals(gameboard, 1).Where(x => x.Y == Position.Y && enemyPiece(x) || x == gameboard.EnPassantTarget));
            }
        }

        //public override Piece Copy() => new Pawn(Position, Color);
    }
}
