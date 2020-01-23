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
        public Game Game;
        private ChessBoard Gameboard => Game != null ? Game.ChessBoard : null;
        private VNode RefreshReference;
        private PieceColor PlayerColor;
        private IEngine Engine;
        private string Enginemove;
        private Piece Selected;
        private Piece[] PromotionSelect = new Piece[4];
        private (ChessBoard Board, (Piece start, (int X, int Y) target) LastMove) SelectedPreviousMove;
        private bool Pause;
        private Gamemode Gamemode;
        private Rendermode RenderMode;
        private RenderClockTimer RenderClockTimerMode = RenderClockTimer.Default;
        private readonly FriendshipProjection FriendshipProjection;
        private readonly AccountProjection AccountProjection;
        private readonly ChessgameProjection GameProjection;
        private AccountEntry AccountEntry;
        private GameID GameID;

        //TODO: maybe put challenge friend etc in its own controller
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
                    while (Pause)
                        Thread.Sleep(100);
                    Thread.Sleep(100);
                    Game?.UpdateClocks();
                    RefreshReference?.Refresh();
                }
            });
        }

        public VNode Render()
        {
            return RefreshReference = RenderGameBoard();
        }

        private VNode RenderGameBoard()
        {
            var challenges = GameProjection.Games.Where(x => x.Receiver == AccountEntry.ID);
            if (challenges.Any() && Game == default)
                return RenderChallenges(challenges);
            if (RenderMode == Rendermode.PlayFriend)
                return RenderChallengeFriend();
            if (Gameboard == default)
                return RenderGameModeSelection();
            if (Game.IsPromotable && PlayerColor != Game.CurrentPlayerColor)
                return RenderPromotionSelection();
            if (RenderMode == Rendermode.WaitingForChallenged)
                return RenderWaitingRoom();
            return RenderBoard();

        }

        private VNode RenderGameModeSelection()
        {
            VNode SelectMode(string buttontext, Gamemode gamemode, RenderClockTimer renderClockTimer)
            {
                return Row(
                       Text(buttontext, Styles.Btn & Styles.MP4, () => RenderClockTimerMode = renderClockTimer),
                       RenderClockTimerMode == renderClockTimer ? RenderClockTimerSelection(gamemode) : null
                );
            }
            return Div(
                SelectMode("Player vs. AI Start", Gamemode.PvE, RenderClockTimer.PvE),
                SelectMode("AI vs. AI Start", Gamemode.EvE, RenderClockTimer.EvE),
                Text("Play vs. Friend", Styles.MP4 & Styles.Btn, () => RenderMode = Rendermode.PlayFriend)
            );
        }

        private VNode RenderClockTimerSelection(Gamemode gamemode)
        {
            return Row(
                Text("normal", Styles.Btn & Styles.MP4, () => SelectGameMode(gamemode, 3600)),
                Text("blitz", Styles.Btn & Styles.MP4, () => SelectGameMode(gamemode, 500)),
                Text("bullet", Styles.Btn & Styles.MP4, () => SelectGameMode(gamemode, 250))
            );
        }

        private void SelectGameMode(Gamemode gamemode, double clocktimer = 0)
        {
            RenderClockTimerMode = RenderClockTimer.Default;
            GameID = GameID.Create();
            Chessgame.Commands.OpenGame(GameID, gamemode, clocktimer);
            Game = GameProjection[GameID].Game;
            Chessgame.Commands.JoinGame(GameID, AccountEntry.ID);

            Engine = new EngineControl();
            PlayerColor = PieceColor.White;
            //EvE loop
            if (gamemode == Gamemode.EvE)
                ThreadPool.QueueUserWorkItem(o =>
                {
                    while (!Game.GameOver)
                    {
                        while (Pause)
                            Thread.Sleep(100);
                        Game.TryEngineMove(Enginemove = Engine.GetEngineMove(Game.GetFeNotation()), Game.PlayedByEngine);
                    }
                });
        }

        private VNode RenderWaitingRoom()
        {
            var challenges = GameProjection.Games.Where(g => g.Challenger == AccountEntry.ID);
            var activeGame = GameProjection.Games.Where(g => g.PlayerWhite == AccountEntry.ID && !g.Closed).FirstOrDefault();
            //action if challenge was accepted 
            if (activeGame != default)
            {
                Game = GameProjection[GameID].Game;
                PlayerColor = PieceColor.White;
                RenderMode = Rendermode.Gameboard;
            }
            //waiting for anyone to accept challenge
            return Div(
                Fragment(challenges.Select(c =>
                    Div(
                        GameProjection[c.ID].HasOpenSpots && c.Created.AddSeconds(c.Timer) > DateTime.Now
                            ? Row(
                                Text($"Waiting for Friend: {c.Timer - c.Elapsed.Seconds}"),
                                Text("Abort Challenge!", Styles.AbortBtn & Styles.MP4, () => BC.Chess.Game.Chessgame.Commands.DenyChallenge(c.ID))
                            )
                            : null
                    )
                )),
                Text("back", Styles.Btn & Styles.MP4, () => { Game = null; RenderMode = Rendermode.Gameboard; })
            );
        }

        private VNode RenderChallenges(IEnumerable<BC.Chess.Game.GameEntry> challenges)
        {
            return Div(
                Fragment(challenges.Select(c =>
                      Row(
                          Text(AccountProjection[c.Challenger].Username),
                          Text("Accept", Styles.Btn & Styles.MP4, () =>
                          {
                              Chessgame.Commands.AcceptChallenge(c.ID, c.Challenger, c.Receiver);
                              GameID = c.ID;
                              Game = GameProjection[GameID].Game;
                              PlayerColor = PieceColor.Black;
                          }),
                          Text("Deny", Styles.Btn & Styles.MP4, () => BC.Chess.Game.Chessgame.Commands.DenyChallenge(c.ID))
                      )
                )));
        }

        private VNode RenderChallengeFriend()
        {
            var friends = FriendshipProjection.GetFriends(AccountEntry.ID)?.Select(id => AccountProjection[id.AccountID]);
            VNode back = Text("back", Styles.Btn & Styles.MP4, () => RenderMode = Rendermode.Gameboard);

            if (friends != default)
                return Div(
                    Fragment(friends.Select(f =>
                            Row(
                                Text(f.Username),
                                Text("Challenge", Styles.Btn & Styles.MP4, () => RenderClockTimerMode = RenderClockTimer.PvF),
                                RenderClockTimerMode == RenderClockTimer.PvF
                                    ? Row(
                                        Text("normal", Styles.Btn & Styles.MP4, () => ChallengeFriend(f, 3600)),
                                        Text("blitz", Styles.Btn & Styles.MP4, () => ChallengeFriend(f, 300)),
                                        Text("bullet", Styles.Btn & Styles.MP4, () => ChallengeFriend(f, 120)))
                                    : null
                            )
                    )),
                    back
                );
            return Div(
                Text("no friends -_-'"),
                back
            );
        }

        private void ChallengeFriend(AccountEntry accountEntry, double clocktimer)
        {
            GameID = GameID.Create();
            Chessgame.Commands.OpenGame(GameID, Gamemode.PvF, clocktimer);
            Chessgame.Commands.RequestChallenge(GameID, AccountEntry.ID, accountEntry.ID);
            Game = GameProjection[GameID].Game;
            PlayerColor = PieceColor.White;
            RenderMode = Rendermode.WaitingForChallenged;
            RenderClockTimerMode = RenderClockTimer.Default;
        }

        private VNode GetBoardVNode(ChessBoard gameboard, (Piece start, (int X, int Y) target) lastmove)
        {
            int rowSize = 8;
            if (PlayerColor == PieceColor.White)
                return Fragment(Enumerable.Range(0, 8).Reverse()
                    .Select(row => Row(gameboard.Board
                        .Skip(rowSize * row)
                        .Take(rowSize)
                        .Select((p, col) => RenderTile(p, col, row, lastmove))
                        .Reverse())));
            else
                return Fragment(Enumerable.Range(0, 8)
                    .Select(row => Row(gameboard.Board
                        .Skip(rowSize * row)
                        .Take(rowSize)
                        .Select((p, col) => RenderTile(p, col, row, lastmove)))));
        }

        private VNode RenderBoard()
        {
            //TODO: maybe rework
            VNode board = GetBoardVNode(Gameboard, Game.Lastmove);

            return Div(
                !Game.Winner.HasValue
                    ? Text("Surrender", Styles.AbortBtn & Styles.MP4, () =>
                    {
                        if (PlayerColor == PieceColor.Black)
                            Game.Winner = PieceColor.White;
                        else
                            Game.Winner = PieceColor.Black;
                        Chessgame.Commands.EndGame(GameID, Allmoves());
                    })
                    : Text("Close Game", Styles.AbortBtn & Styles.MP4, () =>
                    {
                        Game = default;
                        RenderMode = Rendermode.Gameboard;
                    }),
                Row(
                    Div(SelectedPreviousMove.Board != null ? GetBoardVNode(SelectedPreviousMove.Board, SelectedPreviousMove.LastMove) : board),
                    Div(Text("Pause", Styles.AbortBtn & Styles.MP4, PauseGame), RenderPreviousMoves())
                ),
                Game.PlayedByEngine.B == true || Game.PlayedByEngine.W == true ? Text($"EngineMove: {Enginemove}") : null,
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
            Pause = !Pause;
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
            return Fragment(Game.Moves.Select(g =>
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
                            Game.TryEngineMove(Enginemove = Engine.GetEngineMove(Game.GetFeNotation()));
                        else if (Game.PlayedByEngine.W && Game.CurrentPlayerColor == PieceColor.White)
                            Game.TryEngineMove(Enginemove = Engine.GetEngineMove(Game.GetFeNotation()));
                    });
                }
            }
        }
    }
}
