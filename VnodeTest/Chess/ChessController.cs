using ACL.ES;
using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VnodeTest.BC.Chess.Game;
using VnodeTest.BC.General.Account;
using VnodeTest.BC.General.Friendships;
using VnodeTest.Chess.GameEntities;
using VnodeTest.GameEntities;
using VnodeTest.Solitaire.GameEntities;
using static ACL.UI.React.DOM;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Chess.Game.Chessgame>;




namespace VnodeTest.Chess
{
    public class ChessController
    {
        private GameID GameID => GameProjection.GetGameID(AccountEntry.ID);
        public Game Game => GetCurrentGame();
        public Game LastGame { get; set; }
        private ChessBoard Gameboard => Game != null ? Game.ChessBoard : LastGame?.ChessBoard;
        private VNode RefreshReference;
        private PieceColor PlayerColor => GameProjection.GetSpecificPlayerColor(AccountEntry.ID, Game);
        private Piece Selected;
        private Piece[] PromotionSelect = new Piece[4];
        private (ChessBoard Board, (Piece start, (int X, int Y) target) LastMove) SelectedPreviousMove;
        private readonly FriendshipProjection FriendshipProjection;
        private readonly AccountProjection AccountProjection;
        public ChessgameProjection GameProjection { get; set; }
        private AccountEntry AccountEntry;
        private bool SelectMovePlanning;
        private ChessBoard MovePlanningBoard;
        public ChessController(AccountEntry accountEntry, AccountProjection accountProjection, ChessgameProjection chessgameProjection, FriendshipProjection friendshipProjection)
        {
            GameProjection = chessgameProjection;
            FriendshipProjection = friendshipProjection;
            AccountEntry = accountEntry;
            AccountProjection = accountProjection;
            ThreadPool.QueueUserWorkItem(o =>
            {
                while (true)
                {
                    while (Game != default ? Game.Pause : false)
                    {
                        Thread.Sleep(100);
                        Game.PauseClock();
                    }
                    Thread.Sleep(100);
                    Game?.UpdateClocks();
                    RefreshReference?.Refresh();
                }
            });
        }

        private Game GetCurrentGame()
        {
            if (GameID != default)
                LastGame = GameProjection[GameID].Game;
            return LastGame;
        }

        public VNode Render()
        {
            return RefreshReference = RenderGameBoard();
        }

        private VNode RenderGameBoard()
        {
            if (Game != default && Game.IsPromotable && PlayerColor == Game.CurrentPlayerColor && !AccountEntry.AutomaticPromotion)
                return RenderPromotionSelection();
            if (Game != default)
                return RenderBoard();
            return null;

        }

        private VNode GetBoardVNode(ChessBoard gameboard, (Piece start, (int X, int Y) target) lastmove)
        {
            int rowSize = 8;
            if (PlayerColor == PieceColor.White)
                return Fragment(Enumerable.Range(0, 8).Reverse()
                    .Select(row => Row(gameboard.Board
                        .Skip(rowSize * row)
                        .Take(rowSize)
                        .Select((p, col) => RenderTile(p, col, row, gameboard, lastmove)))));
            else
                return Fragment(Enumerable.Range(0, 8)
                    .Select(row => Row(gameboard.Board
                        .Skip(rowSize * row)
                        .Take(rowSize)
                        .Select((p, col) => RenderTile(p, col, row, gameboard, lastmove))
                        .Reverse()))
                    );
        }

        private ChessBoard GetMovePlanningBoard()
        {
            MovePlanningBoard = new ChessBoard(Gameboard.Board.CopyCurrentBoard(), Gameboard.EnPassantTarget);

            if (PlayerColor == PieceColor.White && Game.WhitePlannedMoves.Any())
                foreach ((Piece start, (int X, int Y) target) move in Game.WhitePlannedMoves)
                    MovePlanningBoard = new ChessBoard(MovePlanningBoard.Board.Move(move.start, move.target), MovePlanningBoard.EnPassantTarget);

            else if (PlayerColor == PieceColor.Black && Game.BlackPlannedMoves.Any())
                foreach ((Piece start, (int X, int Y) target) move in Game.BlackPlannedMoves)
                    MovePlanningBoard = new ChessBoard(MovePlanningBoard.Board.Move(move.start, move.target), MovePlanningBoard.EnPassantTarget);

            return MovePlanningBoard;
        }

        private VNode RenderBoard()
        {
            void setWinner()
            {
                if (PlayerColor == PieceColor.Black)
                    Game.Winner = PieceColor.White;
                else
                    Game.Winner = PieceColor.Black;
            }
            VNode surrenderOrClose()
            {
                if (!Game.Winner.HasValue)
                    return Text("Surrender", Styles.TabButtonSelected & Styles.MP4, () =>
                    {
                        setWinner();
                        Chessgame.Commands.EndGame(GameID, Allmoves());
                    });
                else
                    return Text("Game Over", Styles.TabButtonSelected & Styles.MP4, () =>
                    {
                        LastGame = default;
                    });
            }
            VNode board()
            {
                if (SelectedPreviousMove.Board != null)
                    return GetBoardVNode(SelectedPreviousMove.Board, SelectedPreviousMove.LastMove);
                return GetBoardVNode(GetMovePlanningBoard(), Game.Lastmove);
            }
            return Div(
                //top of the gameboard
                Row(
                    surrenderOrClose(),
                    Game.Pause
                            ? Text("Resume", Styles.TabButtonSelected & Styles.MP4 & Styles.MY2, PauseGame)
                            : Text("Pause", Styles.TabButton & Styles.MP4 & Styles.MY2, PauseGame),
                    Text($"White: {Game.WhiteClock:hh\\:mm\\:ss} | Black: {Game.BlackClock:hh\\:mm\\:ss}", Styles.TabNameTagNoWidth)
                ),
                //gameboard + right side of gameboard
                Row(
                    Div(Styles.BorderChessBoard, board()),
                    Div(
                        Text("Last Moves:", Styles.TabNameTag),
                        RenderPreviousMoves()
                    )
                ),
                //below gameboard
                Row(
                    Text("Cancel Premoves", Styles.TabButtonSelected, PlayerColor == PieceColor.White ? (Action)Game.WhitePlannedMoves.Clear : Game.BlackPlannedMoves.Clear),
                    Text("Delete Last Premove", Styles.TabButtonSelected & Styles.FitContent, RemoveEntry)
                ),
                Game.GameOver ? RenderGameOver() : null
            );
        }

        private void RemoveEntry()
        {
            if (PlayerColor == PieceColor.White)
                Game.WhitePlannedMoves.Remove(Game.WhitePlannedMoves.Last());

            else
                Game.BlackPlannedMoves.Remove(Game.BlackPlannedMoves.Last());
        }

        private string Allmoves()
        {
            StringBuilder allmoves = new StringBuilder();
            foreach ((ChessBoard Board, (Piece start, (int x, int Y) target) Lastmove) entry in Game.Moves.Where(g => Game.Moves.IndexOf(g) >= 1))
            {
                allmoves.Append(Game.ChessBoard.ParseToAN(entry.Lastmove.start, entry.Lastmove.target, Game.Moves[Game.Moves.IndexOf(entry) - 1].Board));
                allmoves.Append(".");
            }
            return allmoves.ToString();
        }

        private void PauseGame()
        {
            Game.Pause = !Game.Pause;
        }
        private VNode RenderPreviousMoves()
        {
            void SelectForRender((ChessBoard Board, (Piece start, (int X, int Y) target) LastMove) move)
            {
                //deselect and return to livegame
                if (SelectedPreviousMove == move)
                    SelectedPreviousMove = (null, (null, (0, 0)));
                else
                    SelectedPreviousMove = move;
            }
            return Fragment(Game.Moves.ToArray().Select(g =>
                Game.Moves.IndexOf(g) >= 1
                    ? Text($"Show {Gameboard.ParseToAN(g.LastMove.start, g.LastMove.target, Game.Moves[Game.Moves.IndexOf(g) - 1].Board)}",
                        Styles.MP4 & (SelectedPreviousMove == g ? Styles.TabButtonSelected : Styles.TabButton),
                        () => SelectForRender(g))
                    : null
            ));
        }

        private VNode RenderTile(Piece piece, int col, int row, ChessBoard gameboard, (Piece start, (int X, int Y) target) lastmove = default)
        {
            var target = (col, row);
            Style lastMove = null;
            Style premove = GetPremoveColor(piece, target);
            //set style for lastmove
            if (lastmove.start != null
                && (target == lastmove.start.Position || target == lastmove.target) && !Game.IsPromotable)
                lastMove = Styles.BCred;
            if (SelectedPreviousMove.Board == null)
                return Div(
                    GetBaseStyle(target) & GetSelectedStyle(target) & lastMove & premove,
                    () => Select(target, gameboard),
                    piece != null
                        ? Text(GetSprite(piece), Styles.FontSize3 & Styles.TextAlignC)
                        : null
                );
            else
                return Div(
                    GetBaseStyle(target) & GetBorderStyle(target) & lastMove,
                    piece != null ? Text(GetSprite(piece), Styles.FontSize3 & Styles.TextAlignC) : null
                );
        }

        private Style GetPremoveColor(Piece piece, (int X, int Y) target)
        {
            if ((PlayerColor == PieceColor.White && Game.WhitePlannedMoves.Where(i => i.target == target || i.start.Position == target).Any())
                || (PlayerColor == PieceColor.Black && Game.BlackPlannedMoves.Where(i => i.target == target || i.start.Position == target).Any()))
                return Styles.BCyellow;
            return null;
        }

        private Style GetSelectedStyle((int X, int Y) position)
        {
            return Gameboard.Board[position] != null && Gameboard.Board[position] == Selected
                ? Styles.Selected
                : GetBorderStyle(position);
        }

        private Style GetBaseStyle((int X, int Y) position)
        {
            return TileColor(position) switch
            {
                PieceColor.Black => Styles.TileBlack,
                PieceColor.White => Styles.TileWhite,
                _ => Styles.TCwhite
            };
        }

        private Style GetBorderStyle((int X, int Y) position)
        {
            return TileColor(position) switch
            {
                PieceColor.Black => Styles.BorderBlack,
                PieceColor.White => Styles.BorderWhite,
                _ => Styles.TCwhite
            };
        }

        private PieceColor TileColor((int X, int Y) position)
        {
            //Boolsche algebra ^ = XOR
            var rowEven = position.Y % 2 == 0;
            //checking if tile has even index
            return ((position.X + position.Y * 8) % 2) switch
            {
                0 => rowEven ? PieceColor.Black : PieceColor.White,
                1 => rowEven ? PieceColor.White : PieceColor.Black,
                _ => PieceColor.Zero
            };
        }

        public static string GetSprite(Piece piece)
        {
            return piece.Value switch
            {
                PieceValue.King => piece.Color == PieceColor.White ? "\u2654" : "\u265A",
                PieceValue.Queen => piece.Color == PieceColor.White ? "\u2655" : "\u265B",
                PieceValue.Rook => piece.Color == PieceColor.White ? "\u2656" : "\u265C",
                PieceValue.Bishop => piece.Color == PieceColor.White ? "\u2657" : "\u265D",
                PieceValue.Knight => piece.Color == PieceColor.White ? "\u2658" : "\u265E",
                PieceValue.Pawn => piece.Color == PieceColor.White ? "\u2659" : "\u265F",
                _ => ""
            };
        }

        private VNode RenderGameOver()
        {
            string winner;
            switch (Game.Winner)
            {
                case PieceColor.Black: winner = "Black won"; break;
                case PieceColor.White: winner = "White won"; break;
                case PieceColor.Zero: winner = "Draw"; break;
                default: winner = "error"; break;
            }
            return Text($"Gameover! {winner}", Styles.TabNameTagNoWidth & Styles.FitContent);
        }

        private VNode RenderPromotionSelection()
        {
            return Div(
                Text($"Select Piece you want the Pawn to be promoted to.", Styles.FontSize1p5),
                Row(
                    Text("\u2655", Styles.FontSize3, () => Promotion(new Queen(Game.Lastmove.target, PlayerColor, PieceValue.Queen, Game.Lastmove.target, true))),
                    Text("\u2656", Styles.FontSize3, () => Promotion(new Rook(Game.Lastmove.target, PlayerColor, PieceValue.Rook, Game.Lastmove.target, true))),
                    Text("\u2657", Styles.FontSize3, () => Promotion(new Bishop(Game.Lastmove.target, PlayerColor, PieceValue.Bishop, Game.Lastmove.target, true))),
                    Text("\u2658", Styles.FontSize3, () => Promotion(new Knight(Game.Lastmove.target, PlayerColor, PieceValue.Knight, Game.Lastmove.target, true)))
                )
            );
        }
        private void Promotion(Piece piece)
        {
            Game.ReplacePiece(Game.Lastmove.target, piece);
            Game.IsPromotable = false;
            Selected = null;
            Game.ActionsAfterPromotion(Game);
        }


        private void Select((int X, int Y) target, ChessBoard gameboard)
        {
            //enable playing only for the current player
            if (Game.CurrentPlayerColor == PieceColor.White && Game.PlayedByEngine.W == false && PlayerColor == PieceColor.White
                || Game.CurrentPlayerColor == PieceColor.Black && Game.PlayedByEngine.B == false && PlayerColor == PieceColor.Black)
            {
                //no selection -> select
                if (Selected == null && gameboard.Board[target] != null && gameboard.Board[target].Color == Game.CurrentPlayerColor)
                    Selected = gameboard.Board[target];
                //deselect
                else if (Selected == gameboard.Board[target])
                    Selected = null;
                //move piece
                else if (Selected != null && Game.TryMove(Selected, target))
                {
                    if (Game.GetPreselectedMoves().Any())
                        Game.TryPreselectedMove();

                    if (Game.IsPromotable && AccountEntry.AutomaticPromotion)
                        Promotion(new Queen(Selected.Position, PlayerColor, PieceValue.Queen, Selected.Position, true));
                    Selected = null;
                    //enginemove for PvE
                    if (!Game.IsPromotable)
                        ThreadPool.QueueUserWorkItem(o =>
                        {
                            do
                            {
                                if (Game.PlayedByEngine.B && Game.CurrentPlayerColor == PieceColor.Black)
                                    Game.TryEngineMove(Game.Enginemove = Game.Engine.GetEngineMove(Game.GetFeNotation()));
                                else if (Game.PlayedByEngine.W && Game.CurrentPlayerColor == PieceColor.White)
                                    Game.TryEngineMove(Game.Enginemove = Game.Engine.GetEngineMove(Game.GetFeNotation()));

                                if (Game.GetPreselectedMoves().Any())
                                    Game.TryPreselectedMove();

                            } while ((Game.PlayedByEngine.B && Game.CurrentPlayerColor == PieceColor.Black) || (Game.PlayedByEngine.W && Game.CurrentPlayerColor == PieceColor.White));
                        });
                }
            }
            else if (Game.CurrentPlayerColor != PlayerColor)
            {
                if (Selected == null && gameboard.Board[target] != null && gameboard.Board[target].Color == PlayerColor)
                    Selected = gameboard.Board[target];
                else if (Selected == gameboard.Board[target])
                    Selected = null;
                else if (Selected != null && Selected.GetAllowedMovements().Contains(target))
                {
                    if (PlayerColor == PieceColor.White)
                        Game.WhitePlannedMoves.Add((Selected, target));
                    else
                        Game.BlackPlannedMoves.Add((Selected, target));
                    Selected = null;
                }
            }
        }
    }
}
