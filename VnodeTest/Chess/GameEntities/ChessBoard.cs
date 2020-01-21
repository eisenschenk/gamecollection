using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.GameEntities;

namespace VnodeTest.Chess.GameEntities
{
    public class ChessBoard
    {
        //TODO: my notaion -> AN implemented, AN -> my notation not implemented 
        //TODO: still dont like trymove...
        public ImmutableBoard Board { get; }
        public (int X, int Y) EnPassantTarget { get; }

        public Piece this[(int X, int Y) position]
        {
            get => Board[position];
        }

        public ChessBoard()
        {
            Board = PutPiecesInStartingPosition();
        }
        //@phil
        private ChessBoard(ImmutableBoard board, (int, int) enpassanttarget)
        {
            Board = board;
            EnPassantTarget = enpassanttarget;
        }

        private ImmutableBoard PutPiecesInStartingPosition()
        {
            Piece[] pieces = new Piece[64];
            //putting down black pieces
            PutPawns(pieces, PieceColor.White, 1);
            PutRoyalty(pieces, PieceColor.White, 0);
            //putting down white pieces
            PutPawns(pieces, PieceColor.Black, 6);
            PutRoyalty(pieces, PieceColor.Black, 7);
            return new ImmutableBoard(pieces);
        }
        private Piece[] PutPawns(Piece[] pieces, PieceColor color, int y)
        {
            for (int x = 0; x < 8; x++)
                pieces[x + y * 8] = new Pawn((x, y), color, PieceValue.Pawn, (x, y), false);
            return pieces;
        }

        private void PutRoyalty(Piece[] pieces, PieceColor color, int y)
        {
            foreach (int x in new[] { 0, 7 })
                pieces[x + y * 8] = new Rook((x, y), color, PieceValue.Rook, (x, y), false);
            foreach (int x in new[] { 1, 6 })
                pieces[x + y * 8] = new Knight((x, y), color, PieceValue.Rook, (x, y), false);
            foreach (int x in new[] { 2, 5 })
                pieces[x + y * 8] = new Bishop((x, y), color, PieceValue.Rook, (x, y), false);
            pieces[4 + y * 8] = new King((4, y), color, PieceValue.Rook, (4, y), false);
            pieces[3 + y * 8] = new Queen((3, y), color, PieceValue.Rook, (3, y), false);
        }

        //public ChessBoard Copy() => new ChessBoard(Board.ToArray(), EnPassantTarget);

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
                    MovePiece(start, (start.Position.X + 2 * direction, start.Position.Y), game);
                    //moving rook depending on queenside or kingside castle
                    if (direction > 0)
                        Board.Move(this[(startPosition.X + 3 * direction, startPosition.Y)], (startPosition.X + direction, startPosition.Y));
                    else
                        Board.Move(this[(startPosition.X + 4 * direction, startPosition.Y)], (startPosition.X + direction, startPosition.Y));
                    chessBoard = this.Copy();
                    return true;
                }
            }
            chessBoard = this.Copy();
            return false;
        }

        //public ChessBoard MovePieceInternal(Piece start, (int X, int Y) target)
        //{
        //    Piece[] pieces = new Piece[64];
        //    Board.CopyTo(pieces, 0);

        //    pieces[To1D(target)] = start.Move(target);
        //    pieces[To1D(start.Position)] = null;

        //    return new ChessBoard(pieces, EnPassantTarget);
        //}

        private void MovePiece(Piece start, (int X, int Y) target, out ChessBoard chessBoard, Game game = null, (bool, bool) engineControlled = default)
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
            chessBoard = new ChessBoard(Board.Move(start, target), EnPassantTarget);
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
