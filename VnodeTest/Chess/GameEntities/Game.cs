using ACL.ES;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.Chess;
using VnodeTest.Chess.GameEntities;
using VnodeTest.GameEntities;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Chess.Game.Chessgame>;

namespace VnodeTest
{
    public class Game
    {
        public GameID ID { get; }
        public Gamemode Gamemode { get; }
        //TODO: hasopenspots needs to be accessable for the clokcs, maybe need chessprojection here?
        //TODO: move to gameentry
        //public bool HasBlackPlayer { get; set; }
        //public bool GameWasFullOnce;
        //public bool HasWhitePlayer { get; set; }
        //public bool HasOpenSpots => !HasBlackPlayer || !HasWhitePlayer;
        //public bool IsEmpty => GameEmpty();
        public ChessBoard ChessBoard { get; private set; }
        public (bool W, bool B) PlayedByEngine { get; set; }
        public bool IsPromotable { get; set; }
        public bool GameOver => Winner.HasValue;
        public PieceColor? Winner { get; set; }
        public PieceColor CurrentPlayerColor { get; set; } = PieceColor.White;
        public (Piece start, (int X, int Y) target) Lastmove { get; set; }
        public int MoveCounter { get; set; } = 1;
        public int HalfMoveCounter { get; set; }
        private TimeSpan _WhiteClock;
        public TimeSpan WhiteClock { get => _WhiteClock; private set => _WhiteClock = value; }

        private TimeSpan _BlackClock;
        public TimeSpan BlackClock { get => _BlackClock; private set => _BlackClock = value; }
        private DateTime LastClockUpdate;
        private bool ResetClocks;
        public readonly List<(ChessBoard Board, (Piece start, (int x, int y) target) LastMove)> Moves = new List<(ChessBoard Board, (Piece start, (int x, int y) target) LastMove)>();


        public Game(GameID id, Gamemode gamemode, ChessBoard gameboard, double playerClockTime)
        {
            ID = id;
            Gamemode = gamemode;
            ChessBoard = gameboard;
            Moves.Add((gameboard, (null, (0, 0))));
            LastClockUpdate = DateTime.Now;
            WhiteClock = TimeSpan.FromSeconds(playerClockTime);
            BlackClock = TimeSpan.FromSeconds(playerClockTime);
            PlayedByEngine = gamemode switch
            {
                Gamemode.PvP => (false, false),
                Gamemode.PvF => (false, false),
                Gamemode.PvE => (false, true),
                Gamemode.EvE => (true, true),
                _ => throw new Exception("error game switch")
            };
        }

        private static ((int X, int Y) start, (int X, int Y) target) ParseEnginemoveToCoordinates(string input)
        {
            var start = ParseStringToInt(input[0].ToString(), input[1].ToString());
            var target = ParseStringToInt(input[2].ToString(), input[3].ToString());
            return (start, target);
        }

        public void TryEngineMove(string engineMove, (bool, bool) engineControlled = default)
        {
            //translate enginemove to tuple
            var (start, target) = ParseEnginemoveToCoordinates(engineMove);
            //trymove with translated enginemove
            if (ChessBoard.TryMove(ChessBoard.Board[start], target, out var newboard, this, engineControlled))
            {
                //checking for castles
                if (engineMove.Length >= 5)
                    newboard = new ChessBoard(newboard.Board.ReplacePiece(target, engineMove[4] switch
                    {
                        'q' => new Queen(target, CurrentPlayerColor, PieceValue.Queen, target, true),
                        'n' => new Knight(target, CurrentPlayerColor, PieceValue.Knight, target, true),
                        'b' => new Bishop(target, CurrentPlayerColor, PieceValue.Bishop, target, true),
                        'r' => new Rook(target, CurrentPlayerColor, PieceValue.Rook, target, true),
                        _ => default
                    }), ChessBoard.EnPassantTarget);

                Moves.Add((newboard, Lastmove));
                ChessBoard = newboard;
            }
        }

        public bool TryMove(Piece start, (int X, int Y) target, (bool, bool) engineControlled = default)
        {
            if (ChessBoard.TryMove(start, target, out var newBoard, this, engineControlled))
            {
                Moves.Add((newBoard, Lastmove));
                ChessBoard = newBoard;
                return true;
            }
            return false;
        }

        public static (int X, int Y) ParseStringToInt(string inputX, string inputY)
        {
            return (ParseStringXToInt(inputX), ParseStringYToInt(inputY));
        }
        private static int ParseStringXToInt(string input)
        {
            var c = input[0];
            if (c < 'a' || c > 'h')
                throw new Exception("out of bounds X");
            return c - 'a';
        }


        private static int ParseStringYToInt(string input)
        {
            var c = input[input.Length - 1];
            if (c < '1' || c > '8')
                throw new Exception("out of bounds Y");
            return 55 - c + 1;
        }

        public string GetFeNotation()
        {
            int emptyCount = 0;
            StringBuilder stringBuilder = new StringBuilder();
            for (int y = 0; y < 8; y++)
                for (int x = 0; x < 8; x++)
                {
                    var piece = ChessBoard[(x, y)];
                    //checking if new line starts
                    if (x == 0 && y >= 1)
                    {
                        //writing counter before line end, reset counter
                        if (emptyCount != 0)
                        {
                            stringBuilder.Append(emptyCount.ToString());
                            emptyCount = 0;
                        }
                        //line ends
                        stringBuilder.Append("/");
                    }
                    //counting empty connected empty fields on the board
                    if (piece == null)
                        emptyCount++;
                    //checking if piece exists
                    if (piece != null)
                    {
                        //writing counter of connected fields before next piece, reset counter
                        if (emptyCount != 0)
                            stringBuilder.Append(emptyCount.ToString());
                        emptyCount = 0;
                        //writing abreviation of piece, white = uppercase, black = lowercase
                        stringBuilder.Append(piece.Value switch
                        {
                            PieceValue.King => piece.Color == PieceColor.White ? "K" : "k",
                            PieceValue.Queen => piece.Color == PieceColor.White ? "Q" : "q",
                            PieceValue.Bishop => piece.Color == PieceColor.White ? "B" : "b",
                            PieceValue.Knight => piece.Color == PieceColor.White ? "N" : "n",
                            PieceValue.Rook => piece.Color == PieceColor.White ? "R" : "r",
                            PieceValue.Pawn => piece.Color == PieceColor.White ? "P" : "p",
                            _ => throw new Exception("error FEN piece.value switch")
                        });
                    }
                }
            //currently active playercolor
            stringBuilder.Append(CurrentPlayerColor == PieceColor.White ? " w " : " b ");
            //all possible castles
            stringBuilder.Append(GetPossibleCastles());
            //enpassanttarget
            stringBuilder.Append(ChessBoard.EnPassantTarget == (-1, -1) ? $" {ChessBoard.ParseIntToString(ChessBoard.EnPassantTarget)} " : " - ");
            stringBuilder.Append($"{HalfMoveCounter} ");
            stringBuilder.Append($"{MoveCounter}");

            return stringBuilder.ToString();
        }

        private string GetPossibleCastles()
        {
            string CheckCastle((int X, int Y) king, (int X, int Y) rook)
            {
                string _output = string.Empty;
                bool comparison = king.X == rook.X;
                if (ChessBoard.Board[rook] != null && !ChessBoard.Board[rook].HasMoved
                    && ChessBoard.Board[king] != null && !ChessBoard.Board[king].HasMoved)
                    return _output + comparison switch
                    {
                        true => ChessBoard.Board[king].Color == PieceColor.White ? "Q" : "q",
                        false => ChessBoard.Board[king].Color == PieceColor.White ? "K" : "k",
                    };

                return _output;
            }
            string output = CheckCastle((4, 7), (7, 7));
            output += CheckCastle((4, 7), (0, 7));
            output += CheckCastle((4, 0), (7, 0));
            return output += CheckCastle((4, 0), (0, 0));
        }

        private void TryEnablePromotion(Piece piece, (bool W, bool B) engineControlled = default)
        {
            if (piece is Pawn && (piece.Position.Y == 7 || piece.Position.Y == 0))
            {
                if ((CurrentPlayerColor == PieceColor.White && !engineControlled.W) || (CurrentPlayerColor == PieceColor.Black && !engineControlled.B))
                    IsPromotable = true;
            }
        }

        public void ActionsAfterMoveSuccess(Piece target, Game game = null, (bool, bool) engineControlled = default)
        {
            TryEnablePromotion(target, engineControlled);
            game?.UpdateClocks(changeCurrentPlayer: true);
            if (CurrentPlayerColor == PieceColor.White)
                MoveCounter++;
            if (CheckForGameOver())
                Winner = InverseColor();
        }

        public bool CheckForGameOver()
        {
            if (HalfMoveCounter >= 50)
            {
                Winner = PieceColor.Zero;
                return true;
            }
            return ChessBoard.CheckMateDetection(ChessBoard, CurrentPlayerColor);
        }

        public PieceColor InverseColor()
        {
            if (CurrentPlayerColor == PieceColor.White)
                return PieceColor.Black;
            else return PieceColor.White;
        }

        private readonly object UpdateClockLock = new object();

        public void UpdateClocks() => UpdateClocks(false);

        public void UpdateClocks(bool changeCurrentPlayer)
        {
            lock (UpdateClockLock)
            {
                var now = DateTime.Now;
                void updateColor(PieceColor color, ref TimeSpan clock)
                {
                    if (CurrentPlayerColor != color)
                        return;
                    if (!HasOpenSpots)
                        clock -= now - LastClockUpdate;
                    if (clock <= TimeSpan.Zero)
                        Winner = color;
                }

                updateColor(PieceColor.Black, ref _BlackClock);
                updateColor(PieceColor.White, ref _WhiteClock);
                LastClockUpdate = now;
                if (changeCurrentPlayer)
                    CurrentPlayerColor = InverseColor();
            }
        }
    }
}