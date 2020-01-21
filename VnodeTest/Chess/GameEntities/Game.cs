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
            var _engineMove = ParseEnginemoveToCoordinates(engineMove);
            if (ChessBoard.TryMove(ChessBoard.Board[_engineMove.start], _engineMove.target, out var newboard, this, engineControlled))
            {
                if (engineMove.Length >= 5)
                    newboard = new ChessBoard(newboard.Board.ReplacePiece(_engineMove.target, engineMove[4] switch
                    {
                        'q' => new Queen(_engineMove.target, CurrentPlayerColor, PieceValue.Queen, _engineMove.target, true),
                        'n' => new Knight(_engineMove.target, CurrentPlayerColor, PieceValue.Knight, _engineMove.target, true),
                        'b' => new Bishop(_engineMove.target, CurrentPlayerColor, PieceValue.Bishop, _engineMove.target, true),
                        'r' => new Rook(_engineMove.target, CurrentPlayerColor, PieceValue.Rook, _engineMove.target, true),
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
                    var piece = Gameboard[x, y];

                    if (x == 0 && y >= 1)
                    {
                        if (emptyCount != 0)
                        {
                            stringBuilder.Append(emptyCount.ToString());
                            emptyCount = 0;
                        }
                        stringBuilder.Append("/");
                    }
                    if (piece == null)
                        emptyCount++;

                    if (piece != null)
                    {
                        if (emptyCount != 0)
                            stringBuilder.Append(emptyCount.ToString());
                        emptyCount = 0;
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
            stringBuilder.Append(CurrentPlayerColor == PieceColor.White ? " w " : " b ");
            stringBuilder.Append(GetPossibleCastles());
            stringBuilder.Append(Gameboard.EnPassantTarget == -1 ? $" {Gameboard.ParseIntToString(Gameboard.EnPassantTarget)} " : " - ");
            stringBuilder.Append($"{HalfMoveCounter} ");
            stringBuilder.Append($"{MoveCounter}");
            return stringBuilder.ToString();
        }
    }
}