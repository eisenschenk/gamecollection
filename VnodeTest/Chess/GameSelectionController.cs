using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using VnodeTest.BC.General.Account;
using VnodeTest.BC.Chess.Game;
using VnodeTest.BC.General.Friendships;
using VnodeTest.GameEntities;
using static ACL.UI.React.DOM;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Chess.Game.Chessgame>;
using System.Threading;

namespace VnodeTest.Chess
{
    public class GameSelectionController
    {
        private GameID GameID => GameProjection.GetGameID(AccountEntry.ID);
        private Game Game => GameProjection[GameID]?.Game;
        private readonly FriendshipProjection FriendshipProjection;
        private readonly AccountProjection AccountProjection;
        private readonly ChessgameProjection GameProjection;
        private AccountEntry AccountEntry;
        private VNode RefreshReference;
        private RenderClockTimer RenderClockTimerMode;
        private PieceColor PlayerColor => GameProjection.GetOpenGamePlayerColor(AccountEntry.ID);
        private enum RendermodeLocal { Default, PlayFriend, WaitingForChallenged, Solitaire, Chess }
        private RendermodeLocal RenderMode;

        public RootController RootController { get; set; }
        public GameSelectionController(AccountEntry accountEntry, FriendshipProjection friendshipProjection, AccountProjection accountProjection,
            ChessgameProjection chessgameProjection, RootController rootController)
        {
            AccountEntry = accountEntry;
            FriendshipProjection = friendshipProjection;
            AccountProjection = accountProjection;
            GameProjection = chessgameProjection;
            RootController = rootController;
            ThreadPool.QueueUserWorkItem(o =>
            {
                while (true)
                {
                    Thread.Sleep(100);
                    RefreshReference?.Refresh();
                }
            });
        }

        public VNode Render()
        {
            return RefreshReference = RenderMain();
        }

        private VNode RenderMain()
        {
            var challengesFromFriends = GameProjection.Games.Where(g => !g.Closed && g.Receiver == AccountEntry.ID);
            var challengesFromYou = GameProjection.Games.Where(g => !g.Closed && g.Challenger == AccountEntry.ID);

            if (challengesFromFriends.Any() && Game == default)
                return RenderChallenges(challengesFromFriends);
            if (RenderMode == RendermodeLocal.PlayFriend)
                return RenderChallengeFriend();
            if (GameID == default && RenderMode != RendermodeLocal.WaitingForChallenged)
                return RenderGameModeSelection();
            if (Game != default)
                RootController.Rendermode = Rendermode.ChessGameboard;
            if (!challengesFromYou.Any())
                RenderMode = RendermodeLocal.Default;
            return RenderWaitingRoom();
        }

        private VNode RenderGameModeSelection()
        {
            bool playChess = default;
            VNode SelectMode(string buttontext, Gamemode gamemode, RenderClockTimer renderClockTimer)
            {
                return Row(
                       Text(buttontext, Styles.Btn & Styles.MP4, () => RenderClockTimerMode = renderClockTimer),
                       RenderClockTimerMode == renderClockTimer ? RenderClockTimerSelection(gamemode) : null
                );
            }
            if (RenderMode != RendermodeLocal.Chess)
            {
                return Div(
                    Text("Play Chess!", Styles.Btn & Styles.MP4, () => RenderMode = RendermodeLocal.Chess),
                    Text("Play Solitaire!", Styles.Btn & Styles.MP4, () => RootController.Rendermode = Rendermode.SolitaireGameboard)
                );
            }
            return Div(
                Text("Play Chess:", Styles.MP4),
                SelectMode("Player vs. AI Start", Gamemode.PvE, RenderClockTimer.PvE),
                SelectMode("AI vs. AI Start", Gamemode.EvE, RenderClockTimer.EvE),
                Text("Play vs. Friend", Styles.MP4 & Styles.Btn, () => RenderMode = RendermodeLocal.PlayFriend),
                Text("Back", Styles.MP4 & Styles.Btn, () => RenderMode = RendermodeLocal.Default)
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
            var gID = GameID.Create();
            RenderClockTimerMode = RenderClockTimer.Default;
            Chessgame.Commands.OpenGame(gID, gamemode, clocktimer);
            Chessgame.Commands.JoinGame(gID, AccountEntry.ID);
            RootController.Rendermode = Rendermode.ChessGameboard;
            Game.Engine = new EngineControl();
            //EvE loop
            if (gamemode == Gamemode.EvE)
                ThreadPool.QueueUserWorkItem(o =>
                {
                    while (Game != default && !Game.GameOver)
                    {
                        var game = Game ?? default;
                        while (Game?.Pause ?? false)
                            Thread.Sleep(100);
                        if (game != default)
                            game.TryEngineMove(game.Engine.GetEngineMove(game.GetFeNotation()), game.PlayedByEngine);
                    }
                });
        }

        private VNode RenderWaitingRoom()
        {
            var challenges = GameProjection.Games.Where(g => g.Challenger == AccountEntry.ID);

            return Div(
                Fragment(challenges.Select(c =>
                    Div(
                        GameProjection[c.ID].HasOpenSpots && c.Created.AddSeconds(c.Timer) > DateTime.Now
                            ? Row(
                                Text($"Waiting for Friend: {c.Timer - c.Elapsed.Seconds}"),
                                Text("Abort Challenge!", Styles.AbortBtn & Styles.MP4, () => Chessgame.Commands.DenyChallenge(c.ID))
                            )
                            : null
                    )
                )),
                Text("back", Styles.Btn & Styles.MP4, () => RenderMode = RendermodeLocal.Default)
            );
        }

        private VNode RenderChallenges(IEnumerable<GameEntry> challenges)
        {
            return Div(
                Text("Chess Challenges:"),
                Fragment(challenges.Select(c =>
                      Row(
                          Text(AccountProjection[c.Challenger].Username),
                          Text("Accept", Styles.Btn & Styles.MP4, () => Chessgame.Commands.AcceptChallenge(c.ID, c.Challenger, c.Receiver)),
                          Text("Deny", Styles.Btn & Styles.MP4, () => Chessgame.Commands.DenyChallenge(c.ID))
                      )
                )));
        }

        private VNode RenderChallengeFriend()
        {
            var friends = FriendshipProjection.GetFriends(AccountEntry.ID)?.Select(id => AccountProjection[id.AccountID]);
            VNode back = Text("back", Styles.Btn & Styles.MP4, () => RenderMode = RendermodeLocal.Default);

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
            var gID = GameID.Create();
            Chessgame.Commands.OpenGame(gID, Gamemode.PvF, clocktimer);
            Chessgame.Commands.RequestChallenge(gID, AccountEntry.ID, accountEntry.ID);
            RenderMode = RendermodeLocal.WaitingForChallenged;
            RenderClockTimerMode = RenderClockTimer.Default;
        }
    }
}
