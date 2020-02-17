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
                        Thread.Sleep(100);
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
            if (Game != default && Game.IsPromotable && PlayerColor != Game.CurrentPlayerColor)
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
                        .Select((p, col) => RenderTile(p, col, row, lastmove)))));
            else
                return Fragment(Enumerable.Range(0, 8)
                    .Select(row => Row(gameboard.Board
                        .Skip(rowSize * row)
                        .Take(rowSize)
                        .Select((p, col) => RenderTile(p, col, row, lastmove))
                        .Reverse()))
                    );
        }

        private VNode RenderBoard()
        {
            void SetWinner()
            {
                if (PlayerColor == PieceColor.Black)
                    Game.Winner = PieceColor.White;
                else
                    Game.Winner = PieceColor.Black;
            }
            VNode SurrenderOrClose()
            {
                if (!Game.Winner.HasValue)
                    return Text("Surrender", Styles.AbortBtn & Styles.MP4, () =>
                    {
                        SetWinner();
                        Chessgame.Commands.EndGame(GameID, Allmoves());
                    });
                else
                    return Text("Game Over", Styles.AbortBtn & Styles.MP4, () =>
                    {
                        LastGame = default;
                    });
            }
            VNode board = default;
            if (Game != null)
                board = GetBoardVNode(Gameboard, Game.Lastmove);
            //else
            //{
            //    var lastgame = GameProjection.GetLastPlayedGame(AccountEntry.ID);
            //    if (LastGame != default)
            //        board = GetBoardVNode(lastgame.ChessBoard, lastgame.Lastmove);
            //}

            return Div(
                //top of the gameboard
                SurrenderOrClose(),
                //right side of gameboard
                Row(
                    Div(SelectedPreviousMove.Board != null ? GetBoardVNode(SelectedPreviousMove.Board, SelectedPreviousMove.LastMove) : board),
                    Div(Text("Pause", Styles.AbortBtn & Styles.MP4, PauseGame), RenderPreviousMoves())
                ),
                //below gameboard
                Text($"Time remaining White: {Game.WhiteClock:hh\\:mm\\:ss}"),
                Text($"Time remaining Black: {Game.BlackClock:hh\\:mm\\:ss}"),
                Text($"Gameroom: {Game.ID}"),
                Game.GameOver ? RenderGameOver() : null
            );
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
                        Styles.MP4 & (SelectedPreviousMove == g ? Styles.SelectedBtn : Styles.Btn),
                        () => SelectForRender(g))
                    : null
            ));
        }

        private VNode RenderTile(Piece piece, int col, int row, (Piece start, (int X, int Y) target) lastmove = default)
        {
            var target = (col, row);
            Style lastMove = null;
            //set style for lastmove
            if (lastmove.start != null
                && (target == lastmove.start.Position || target == lastmove.target) && !Game.IsPromotable)
                lastMove = Styles.BCred;
            if (SelectedPreviousMove.Board == null)
                return Div(
                    GetBaseStyle(target) & GetSelectedStyle(target) & lastMove,
                    () => Select(target),
                    piece != null
                        ? Text(GetSprite(piece), Styles.FontSize3)
                        : null
                );
            else
                return Div(
                    GetBaseStyle(target) & GetBorderStyle(target) & lastMove,
                    piece != null ? Text(GetSprite(piece), Styles.FontSize3) : null
                );
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
            return Text($"Gameover! {winner}");
        }

        private VNode RenderPromotionSelection()
        {
            Selected = Gameboard.Board[Game.Lastmove.target];
            PromotionSelect[0] = new Rook((0, 0), Selected.Color, PieceValue.Rook, (0, 0), true);
            PromotionSelect[1] = new Knight((1, 0), Selected.Color, PieceValue.Knight, (1, 0), true);
            PromotionSelect[2] = new Bishop((2, 0), Selected.Color, PieceValue.Bishop, (2, 0), true);
            PromotionSelect[3] = new Queen((3, 0), Selected.Color, PieceValue.Queen, (3, 0), true);
            return Div(
                Styles.M2,
                Text($"Select Piece you want the Pawn to be promoted to.", Styles.FontSize1p5),
                Row(Fragment(PromotionSelect.Select(x => RenderTile(x, x.Position.X, x.Position.Y))))
            );
        }

        private void Select((int X, int Y) target)
        {
            //promtotion
            if (Game.IsPromotable && PlayerColor != Game.CurrentPlayerColor)
            {
                Selected = Gameboard.Board[Game.Lastmove.target];
                Game.ReplacePiece(target, PromotionSelect[target.X].Move(target));
                Selected = null;
                Game.IsPromotable = false;
                return;
            }
            //enable playing only for the current player
            if (Game.CurrentPlayerColor == PieceColor.White && Game.PlayedByEngine.W == false && PlayerColor == PieceColor.White
                || Game.CurrentPlayerColor == PieceColor.Black && Game.PlayedByEngine.B == false && PlayerColor == PieceColor.Black)
            {
                //no selection -> select
                if (Selected == null && Gameboard.Board[target] != null && Gameboard.Board[target].Color == Game.CurrentPlayerColor)
                    Selected = Gameboard.Board[target];
                //deselect
                else if (Selected == Gameboard.Board[target])
                    Selected = null;
                //move piece
                else if (Selected != null && Game.TryMove(Selected, target))
                {
                    Selected = null;
                    //enginemove for PvE
                    ThreadPool.QueueUserWorkItem(o =>
                    {
                        if (Game.PlayedByEngine.B && Game.CurrentPlayerColor == PieceColor.Black)
                            Game.TryEngineMove(Game.Enginemove = Game.Engine.GetEngineMove(Game.GetFeNotation()));
                        else if (Game.PlayedByEngine.W && Game.CurrentPlayerColor == PieceColor.White)
                            Game.TryEngineMove(Game.Enginemove = Game.Engine.GetEngineMove(Game.GetFeNotation()));
                    });
                }
            }
        }
    }
}
