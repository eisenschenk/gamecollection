﻿using System;
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
        public King((int X, int Y) position, PieceColor color, PieceValue pieceValue, string sprite, (int X, int Y) startposition, bool hasmoved)
            : base(position, color, pieceValue, sprite, startposition, hasmoved)
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
                //checking value of Tile
                if (!(rookTile is Rook) || rookTile.HasMoved)
                    return false;
                //checking direction of castling and returns false when there are pieces blocking the castlingattempt
                if (direction < 0)
                {
                    for (int index = Position.X + direction; index >= rookTile.Position.X; index--)
                        if (gameboard.Board[index] != null)
                            return false;
                }
                else
                {
                    for (int index = Position.X + direction; index <= rookTile.Position.X; index++)
                        if (gameboard.Board[index] != null)
                            return false;
                }
                //checking if king is in check while doing the castling
                if (gameboard.CheckDetection(Color)
                    || HypotheticalMove(gameboard, (Position.X + direction, Position.Y)).CheckDetection(Color) 
                    || HypotheticalMove(gameboard, (Position.X + 2 * direction, Position.Y)).CheckDetection(Color))
                    return false;
                return true;
            }
            //hardcoded starting postions of both kings
            if (!HasMoved && (Position == (4, 0) || Position == (4, 7)))
            {

                var rookTileLeft = gameboard.Board[Position.X - 4];
                var rookTileRight = gameboard.Board[Position.X + 3];
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
