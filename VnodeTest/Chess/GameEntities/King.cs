using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.Chess;
using VnodeTest.Chess.GameEntities;

namespace VnodeTest.GameEntities
{
    class King : Piece
    {
        public King((int X, int Y) position, PieceColor color, PieceValue pieceValue, (int X, int Y) startposition, bool hasmoved)
            : base(position, color, pieceValue, startposition, hasmoved)
        {
        }

        protected override IEnumerable<(int X, int Y)> GetPotentialMovements(ChessBoard gameboard)
        {
            return GetDiagonals(gameboard, 1).Concat(GetStraightLines(gameboard, 1)).Concat(GetCastlingPositions(gameboard));
        }

        private IEnumerable<(int X, int Y)> GetCastlingPositions(ChessBoard gameboard)
        {
            bool EmptyAndNoCheck(int direction, Piece rookTile)
            {
                if (!(rookTile is Rook) || rookTile.HasMoved)
                    return false;
                //checking direction of castling and returns false when there are pieces blocking the castling attempt
                for (int index = Position.X + direction; index == rookTile.Position.X; index += direction)
                    if (gameboard.Board[(index, Position.Y)] != null)
                        return false;

                //checking if king is in check while doing the castling
                if (gameboard.CheckDetection(Color)
                    || gameboard.HypotheticalMove(gameboard, this, (Position.X + direction, Position.Y)).CheckDetection(Color)
                    || gameboard.HypotheticalMove(gameboard, this, (Position.X + 2 * direction, Position.Y)).CheckDetection(Color))
                    return false;
                return true;
            }
            //hardcoded starting positions of both kings
            if (!HasMoved && (Position == (4, 0) || Position == (4, 7)))
            {

                var rookTileLeft = gameboard.Board[(Position.X - 4, Position.Y)];
                var rookTileRight = gameboard.Board[(Position.X + 3, Position.Y)];
                //returning potential valid castling moves
                if (rookTileLeft != default && EmptyAndNoCheck(-1, rookTileLeft))
                    yield return (Position.X - 2, Position.Y);
                if (rookTileRight != null && EmptyAndNoCheck(1, rookTileRight))
                    yield return (Position.X + 2, Position.Y);
            }
        }

        //public override BasePiece Copy() => new King(Position, Color);
    }
}
