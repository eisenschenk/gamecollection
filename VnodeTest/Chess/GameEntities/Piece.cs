using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VnodeTest.Chess.Enums;

namespace VnodeTest.Chess.GameEntities
{
    public abstract class Piece
    {
        public PieceColor Color { get; set; }
        public PieceValue Value { get; set; }
        public string Sprite => GetSprite();
        public (int X, int Y) StartPosition { get; set; } //von color abh. 
        public bool HasMoved { get; set; }
        private (int X, int Y) _Position;
        public (int X, int Y) Position
        {
            get
            {
                return _Position;
            }
            set
            {
                if (_Position != StartPosition)
                    HasMoved = true;
                _Position = value;
            }
        }

        public Piece((int X, int Y) position, PieceColor color)
        {
            Position = position;
            StartPosition = position;
            Color = color;
        }

        public IEnumerable<(int X, int Y)> GetDiagonals(ChessBoard chessboard, int distance = 7)
        {
            for (int directionX = -1; directionX < 2; directionX += 2)
                for (int directionY = -1; directionY < 2; directionY += 2)
                    foreach (var move in GetPotentialMoves((directionX, directionY), chessboard, distance))
                        yield return move;
        }

        public IEnumerable<(int X, int Y)> GetStraightLines(ChessBoard gameboard, int distance = 7)
        {
            var result = Enumerable.Empty<(int, int)>();
            for (int i = -1; i < 2; i += 2)
                result = result
                    .Concat(GetPotentialMoves((i, 0), gameboard, distance))
                    .Concat(GetPotentialMoves((0, i), gameboard, distance));
            return result;
        }

        private IEnumerable<(int X, int Y)> GetPotentialMoves((int X, int Y) direction, ChessBoard chessboard, int distance = 7)
        {
            var currentTarget = (X: Position.X + direction.X, Y: Position.Y + direction.Y);
            //distance enables the use of this method for all piecetypes and is a limitation to the allowed movement 
            while (distance > 0 && currentTarget.X < 8 && currentTarget.X >= 0 && currentTarget.Y < 8 && currentTarget.Y >= 0)
            {
                var notNull = chessboard[currentTarget] != null;
                var currentTargetColor = chessboard[currentTarget]?.Color;
                //cant capture friendly pieces
                if (notNull && currentTargetColor == Color)
                    yield break;
                //returns current targetposition which is either empty or an enemy piece
                yield return currentTarget;
                //breaks after 1 enemy has been potentially captured
                if (notNull && currentTargetColor != Color)
                    yield break;

                currentTarget.X += direction.X;
                currentTarget.Y += direction.Y;
                distance--;
            }
        }

        private string GetSprite()
        {
                return Value switch
                {
                    PieceValue.King => Color == PieceColor.White ? "\u2654" : "\u265A",
                    PieceValue.Queen => Color == PieceColor.White ? "\u2655" : "\u265B",
                    PieceValue.Rook => Color == PieceColor.White ? "\u2656" : "\u265C",
                    PieceValue.Bishop => Color == PieceColor.White ? "\u2657" : "\u265D",
                    PieceValue.Knight => Color == PieceColor.White ? "\u2658" : "\u265E",
                    PieceValue.Pawn => Color == PieceColor.White ? "\u2659" : "\u265F",
                    _ => ""
                };
        }

        public IEnumerable<(int X, int Y)> GetValidMovements(ChessBoard gameboard)
        {
            return GetPotentialMovements(gameboard).Where(m =>
            {
                var hypotheticalGameBoard = HypotheticalMove(gameboard, m);
                var kingSameColorPosition = hypotheticalGameBoard.Board
                    .Where(t => t != null && t.Color == Color && t is King)
                    .Single().Position;
                var enemyPieces = hypotheticalGameBoard.Board.Where(x => x != null && x.Color != Color);
                //checking if the king is in check after the hypothetical move, returning only valid moves
                return !enemyPieces.SelectMany(t => t.GetPotentialMovements(hypotheticalGameBoard)).Contains(kingSameColorPosition);
            });
        }

        public ChessBoard HypotheticalMove(ChessBoard gameboard, (int X, int Y) target)
        {
            //TODO: trymove only
            var futureGameBoard = gameboard.Copy();
            futureGameBoard.MovePieceInternal(this, target);
            return futureGameBoard;
        }

        public abstract Piece Copy();

        protected abstract IEnumerable<(int X, int Y)> GetPotentialMovements(ChessBoard gameboard);
    }
}
