using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.GameEntities;
using static VnodeTest.Chess.Enums;

namespace VnodeTest.Chess.GameEntities
{
    public class ChessBoard
    {
        public Piece[] Board { get; set; } = new Piece[64];
        public (int X, int Y) EnPassantTarget { get; set; }

        public Piece this[(int X, int Y) position]
        {
            get => Board[position.Y * 8 + position.X];
            set => Board[position.Y * 8 + position.X] = value;
        }

        public ChessBoard()
        {
            PutPiecesInStartingPosition();
        }
        //@phil
        private ChessBoard(IEnumerable<Piece> collection, (int, int) enpassanttarget)
        {
            Board = collection.ToArray();
            EnPassantTarget = enpassanttarget;
        }

        private void PutPiecesInStartingPosition()
        {
            for (int pawns = 0; pawns < 8; pawns++)
                this[(pawns, 1)] = new Pawn((pawns, 1), PieceColor.Black);
            this[(0, 0)] = new Rook((0, 0), PieceColor.Black);
            this[(1, 0)] = new Knight((1, 0), PieceColor.Black);
            this[(2, 0)] = new Bishop((2, 0), PieceColor.Black);
            this[(3, 0)] = new Queen((3, 0), PieceColor.Black);
            this[(4, 0)] = new King((4, 0), PieceColor.Black);
            this[(5, 0)] = new Bishop((5, 0), PieceColor.Black);
            this[(6, 0)] = new Knight((6, 0), PieceColor.Black);
            this[(7, 0)] = new Rook((7, 0), PieceColor.Black);

            for (int pawns = 0; pawns < 8; pawns++)
                this[(pawns, 6)] = new Pawn((pawns, 6), PieceColor.White);
            this[(0, 7)] = new Rook((0, 7), PieceColor.White);
            this[(1, 7)] = new Knight((1, 7), PieceColor.White);
            this[(2, 7)] = new Bishop((2, 7), PieceColor.White);
            this[(3, 7)] = new Queen((3, 7), PieceColor.White);
            this[(4, 7)] = new King((4, 7), PieceColor.White);
            this[(5, 7)] = new Bishop((5, 7), PieceColor.White);
            this[(6, 7)] = new Knight((6, 7), PieceColor.White);
            this[(7, 7)] = new Rook((7, 7), PieceColor.White);
        }

        public ChessBoard Copy() => new ChessBoard(Board.ToArray(), EnPassantTarget);

        public bool TryMove(Piece start, (int X, int Y) target, out ChessBoard chessBoard, Game game, (bool, bool) engineControlled = default)
        {
            if (TryCastling(start, target, out chessBoard, game))
                return true;

            if (!start.GetValidMovements(this).Contains(target))
                return false;

            if (this[target] != null || start is Pawn)
                game.HalfMoveCounter = 0;
            else
                game.HalfMoveCounter++;

            MovePiece(start, target, game, engineControlled);
            chessBoard = this.Copy();
            return true;
        }

        private bool TryCastling(Piece start, (int X, int Y) target, out ChessBoard chessBoard, Game game)
        {
            if (start is King)
            {
                if (Math.Abs(target.X - start.Position.X) == 2)
                {
                    //direction hack => target either left or right rook
                    int direction = 1;
                    if (start.Position.X > target.X)
                        direction *= -1;
                    var startPosition = start.Position;
                    //moving king
                    MovePiece(start, start.Position.X + 2 * direction, game);
                    //moving rook depending on queenside or kingside castle
                    if (direction > 0)
                        MovePieceInternal(this[(startPosition.X + 3 * direction, startPosition.Y)], (startPosition.X + direction, startPosition.Y));
                    else
                        MovePieceInternal(this[(startPosition.X + 4 * direction, startPosition.Y)], (startPosition.X + direction, startPosition.Y));
                    chessBoard = this.Copy();
                    return true;
                }
            }
            chessBoard = this.Copy();
            return false;
        }

        public void MovePieceInternal(Piece start, (int X, int Y) target)
        {
            this[target] = start.Copy();
            this[start.Position] = null;
            this[target].Position = target;
        }

        private void MovePiece(Piece start, (int X, int Y) target, Game game = null, (bool, bool) engineControlled = default)
        {
            //enpassant check 
            if (start is Pawn)
            {
                var enPassant = EnPassantTarget;
                EnPassantTarget = (-1, -1);
                if (target == enPassant)
                    Board[game.Lastmove.target] = null;
                else if (Math.Abs(start.Position.Y - target.Y) == 2)
                    EnPassantTarget = (start.Position.X, start.Color == PieceColor.Black ? start.Position.Y + 1 : start.Position.Y - 1);
            }
            game.Lastmove = (start.Copy(), target);
            MovePieceInternal(start, target);
            game.ActionsAfterMoveSuccess(this.Copy()[target], game, engineControlled);
        }

    }
}
