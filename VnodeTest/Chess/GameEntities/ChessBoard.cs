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
        //TODO: my notaion -> AN implemented, AN -> my notation not implemented 
        //TODO: still dont like trymove...
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
            //trying to castle
            if (TryCastling(start, target, out chessBoard, game))
                return true;
            //checking if target is a valid move for the startpiece
            if (!start.GetValidMovements(this).Contains(target))
                return false;
            //movecounter for official counting of halfmoves (pawn moved or piece is captured resets the counter)
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
            //actual movement of the piece
            MovePieceInternal(start, target);
            //things that have to happen after the piece was moved
            game.ActionsAfterMoveSuccess(this.Copy()[target], game, engineControlled);
        }

        public bool CheckDetection(PieceColor color)
        {
            //checking if king is in check
            var king = Board.Where(p => p != null && p.Color == color && p is King).Single();
            var enemyMoves = Board.Where(p => p != null && p.Color != color).SelectMany(m => m.GetValidMovements(this));
            if (enemyMoves.Contains(king.Position))
                return true;
            return false;
        }

        public string ParseToAN(Piece start, (int X, int Y) target, ChessBoard gameboard)
        {
            //castling
            if (start is King && Math.Abs(start.Position.X - target.X) == 2)
            {
                if (start.Position.X > target.X)
                    return "O-O-O";
                return "O-O";
            }
            var _pieces = gameboard.Board.Where(p => p != null && p.Color == start.Color);

            //check & checkmate
            var kingDifferentColorPosition = gameboard.Board.Where(t => t != null && t.Color != start.Color && t is King).Single().Position;
            var check = !_pieces.SelectMany(t => t.GetValidMovements(gameboard)).Contains(kingDifferentColorPosition) ? string.Empty : "+";
            check = CheckMateDetection(gameboard, start.Color) ? "#" : check;

            //Correct piece type
            _pieces = _pieces.Where(s => s.Sprite == start.Sprite);

            //destination
            _pieces = _pieces.Where(d => d.GetValidMovements(gameboard).Contains(target));
            var pieceLetter = GetCorrectSpriteAbreviation(start.Value, start.Color);
            var promotionLetter = start is Pawn && (target.Y == 0 || target.Y == 7) ? GetCorrectSpriteAbreviation(_pieces.First().Value, start.Color) : string.Empty;
            var capturePiece = gameboard[target] == null ? string.Empty : "x";

            //xy-position the same
            var ypieces = _pieces.Where(n => n.Position.Y == start.Position.Y).ToArray();
            var xpieces = _pieces.Where(m => m.Position.X == start.Position.X).ToArray();
            string sourceX = ParseIntToString(start.Position)[0].ToString();
            string sourceY = ParseIntToString(start.Position)[1].ToString();
            string source;
            if (xpieces.Length == 1 && ypieces.Length == 1)
                source = string.Empty;
            else if (ypieces.Length > 1 && xpieces.Length == 1)
                source = sourceX;
            else if (xpieces.Length > 1 && ypieces.Length == 1)
                source = sourceY;
            else
                source = sourceX + sourceY;

            string targetX = ParseIntToString(target)[0].ToString();
            string targetY = ParseIntToString(target)[1].ToString();

            //game end


            return $"{pieceLetter}{source}{capturePiece}{targetX}{targetY}{promotionLetter}{check}";
        }

        public bool CheckMateDetection(ChessBoard gameboard, PieceColor color)
        {
            foreach (Piece piece in gameboard.Board.Where(t => t != null && t.Color == color))
                if (piece.GetValidMovements(gameboard).Any())
                    return false;
            return true;
        }

        private string GetCorrectSpriteAbreviation(PieceValue value, PieceColor color)
        {
            if (color == PieceColor.White)
                return value switch
                {
                    PieceValue.King => "K",
                    PieceValue.Queen => "Q",
                    PieceValue.Rook => "R",
                    PieceValue.Bishop => "B",
                    PieceValue.Knight => "N",
                    PieceValue.Pawn => "",
                    _ => throw new Exception("error in GetCorrectSpriteAbreviation White")
                };
            return value switch
            {
                PieceValue.King => "k",
                PieceValue.Queen => "q",
                PieceValue.Rook => "r",
                PieceValue.Bishop => "b",
                PieceValue.Knight => "n",
                PieceValue.Pawn => "",
                _ => throw new Exception("error in GetCorrectSpriteAbreviation White")
            };
        }

        public static string ParseIntToString((int X, int Y) index)
        {
            var yOut = (8 - index.Y).ToString();
            var xOut = (char)('a' + index.X);
            return xOut + yOut;
        }

    }
}
