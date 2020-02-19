using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.GameEntities;

namespace VnodeTest.Chess.GameEntities
{
    public abstract class Piece
    {
        public PieceColor Color { get; }
        public PieceValue Value { get; }
        public (int X, int Y) StartPosition { get; } //von color abh. 
        public bool HasMoved { get; }
        public (int X, int Y) Position { get; }
        public bool IsPreselectedMove;


        public Piece((int X, int Y) position, PieceColor color, PieceValue pieceValue, (int X, int Y) startposition, bool hasmoved)
        {
            Position = position;
            StartPosition = startposition;
            Color = color;
            Value = pieceValue;
            HasMoved = hasmoved;
        }
        public Piece(Piece piece)
        {
            Position = piece.Position;
            StartPosition = piece.StartPosition;
            Color = piece.Color;
            Value = piece.Value;
            HasMoved = piece.HasMoved;
        }
        //@phil
        public Piece Move((int X, int Y) position)
        {
            return Value switch
            {
                PieceValue.King => new King(position, Color, Value, StartPosition, true),
                PieceValue.Queen => new Queen(position, Color, Value, StartPosition, true),
                PieceValue.Rook => new Rook(position, Color, Value, StartPosition, true),
                PieceValue.Knight => new Knight(position, Color, Value, StartPosition, true),
                PieceValue.Bishop => new Bishop(position, Color, Value, StartPosition, true),
                PieceValue.Pawn => new Pawn(position, Color, Value, StartPosition, true)
            };
        }

        public IEnumerable<(int X, int Y)> GetDiagonals(ChessBoard chessboard, bool onlyValid, int distance = 7)
        {
            for (int directionX = -1; directionX < 2; directionX += 2)
                for (int directionY = -1; directionY < 2; directionY += 2)
                    foreach (var move in GetPotentialMoves((directionX, directionY), chessboard, onlyValid, distance))
                        yield return move;
        }

        public IEnumerable<(int X, int Y)> GetStraightLines(ChessBoard gameboard, bool onlyValid, int distance = 7)
        {
            var result = Enumerable.Empty<(int, int)>();
            for (int i = -1; i < 2; i += 2)
                result = result
                    .Concat(GetPotentialMoves((i, 0), gameboard, onlyValid, distance))
                    .Concat(GetPotentialMoves((0, i), gameboard, onlyValid, distance));
            return result;
        }

        private IEnumerable<(int X, int Y)> GetPotentialMoves((int X, int Y) direction, ChessBoard chessboard, bool onlyValid, int distance = 7)
        {
            var currentTarget = (X: Position.X + direction.X, Y: Position.Y + direction.Y);
            //distance enables the use of this method for all piecetypes and is a limitation to the allowed movement 
            while (distance > 0 && currentTarget.X < 8 && currentTarget.X >= 0 && currentTarget.Y < 8 && currentTarget.Y >= 0)
            {
                if (onlyValid)
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
                }
                yield return currentTarget;
                currentTarget.X += direction.X;
                currentTarget.Y += direction.Y;
                distance--;
            }
        }

        public IEnumerable<(int X, int Y)> GetAllowedMovements()
        {

            (int distanceStraight, int distanceDiagonal) distance = default;

            if (this is Knight knight)
            {
                return knight.GetMoves();
            }
            if (this is Pawn)
            {
                int possibleMove = (StartPosition == Position) ? 2 : 1;
                bool filterY(int y, int positionY) => Color == PieceColor.White ? y > positionY : y < positionY;
                return GetStraightLines(default, false, possibleMove).Where(p => p.X == Position.X && filterY(p.Y, Position.Y)).Concat(GetDiagonals(default, false, 1).Where(p => filterY(p.Y, Position.Y)));
            }
            else
            {

                distance = Value switch
                {
                    PieceValue.Bishop => (0, 7),
                    PieceValue.King => (2, 1),
                    PieceValue.Queen => (7, 7),
                    PieceValue.Rook => (7, 0),
                };
                return GetStraightLines(default, false, distance.distanceStraight).Concat(GetDiagonals(default, false, distance.distanceDiagonal));
            }
        }




        public IEnumerable<(int X, int Y)> GetValidMovements(ChessBoard gameboard)
        {
            return GetPotentialMovements(gameboard).Where(m =>
            {
                var hypotheticalGameBoard = ChessBoard.HypotheticalMove(gameboard, this, m);
                var kingSameColorPosition = hypotheticalGameBoard.Board
                    .Where(t => t != null && t.Color == Color && t is King)
                    .Single().Position;
                var enemyPieces = hypotheticalGameBoard.Board.Where(x => x != null && x.Color != Color);
                //checking if the king is in check after the hypothetical move, returning only valid moves
                return !enemyPieces.SelectMany(t => t.GetPotentialMovements(hypotheticalGameBoard)).Contains(kingSameColorPosition);
            });
        }



        // public abstract Piece Copy();

        protected abstract IEnumerable<(int X, int Y)> GetPotentialMovements(ChessBoard gameboard);
    }
}
