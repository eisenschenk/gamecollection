using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.BC.General.Account;
using VnodeTest.BC.Chess.Game;
using VnodeTest.BC.Chess.Game;
using VnodeTest.BC.General.Account;
using VnodeTest.BC.General.Friendships;
using VnodeTest.Chess.GameEntities;
using VnodeTest.GameEntities;
using static ACL.UI.React.DOM;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Chess.Game.Chessgame>;
using VnodeTest.Chess;
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
        public Rendermode RenderMode { get; private set; }
        private RenderClockTimer RenderClockTimerMode;
        private PieceColor PlayerColor => GameProjection.GetOpenGamePlayerColor(AccountEntry.ID);

        public GameSelectionController(AccountEntry accountEntry, FriendshipProjection friendshipProjection, AccountProjection accountProjection, ChessgameProjection chessgameProjection)
        {
            AccountEntry = accountEntry;
            FriendshipProjection = friendshipProjection;
            AccountProjection = accountProjection;
            GameProjection = chessgameProjection;
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
            var challenges = GameProjection.Games.Where(x => x.Receiver == AccountEntry.ID);

            if (challenges.Any() && Game == default)
                return RenderChallenges(challenges);
            if (RenderMode == Rendermode.PlayFriend)
                return RenderChallengeFriend();
            if (GameID == default && RenderMode != Rendermode.WaitingForChallenged)
                return RenderGameModeSelection();
            if (Game != default)
                RenderMode = Rendermode.Gameboard;
            return RenderWaitingRoom();
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
        //TODO: kein watiting room, accrept -> back instead of showing game
        // suggest complete redo of rendermodes, too confusing
        private void SelectGameMode(Gamemode gamemode, double clocktimer = 0)
        {
            var gID = GameID.Create();
            RenderClockTimerMode = RenderClockTimer.Default;
            Chessgame.Commands.OpenGame(gID, gamemode, clocktimer);
            Chessgame.Commands.JoinGame(gID, AccountEntry.ID);
            RenderMode = Rendermode.Gameboard;
            Game.Engine = new EngineControl();
            //EvE loop
            if (gamemode == Gamemode.EvE)
                ThreadPool.QueueUserWorkItem(o =>
                {
                    while (!Game.GameOver)
                    {
                        while (Game.Pause)
                            Thread.Sleep(100);
                        Game.TryEngineMove(Game.Enginemove = Game.Engine.GetEngineMove(Game.GetFeNotation()), Game.PlayedByEngine);
                    }
                });
        }

        private VNode RenderWaitingRoom()
        {
            var challenges = GameProjection.Games.Where(g => g.Challenger == AccountEntry.ID);
            var activeGame = GameProjection.Games.Where(g => g.PlayerWhite == AccountEntry.ID && !g.Closed).FirstOrDefault();
            ////action if challenge was accepted 
            //if (activeGame != default)
            //    RenderMode = Rendermode.Gameboard;

            //waiting for anyone to accept challenge
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
                Text("back", Styles.Btn & Styles.MP4, () => { RenderMode = Rendermode.Default; })
            );
        }

        private VNode RenderChallenges(IEnumerable<GameEntry> challenges)
        {
            //TODO modes here
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
            VNode back = Text("back", Styles.Btn & Styles.MP4, () => RenderMode = Rendermode.Default);

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
            RenderMode = Rendermode.WaitingForChallenged;
            RenderClockTimerMode = RenderClockTimer.Default;
        }
    }
}
