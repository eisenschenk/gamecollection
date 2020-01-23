﻿using ACL.UI.React;
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

namespace VnodeTest.Chess
{
    public class PlayVsFriendController
    {
        public Game Game;
        private readonly FriendshipProjection FriendshipProjection;
        private readonly AccountProjection AccountProjection;
        private readonly ChessgameProjection GameProjection;
        private AccountEntry AccountEntry;
        private GameID GameID;
        private Rendermode RenderMode;
        private RenderClockTimer RenderClockTimerMode;

        public Chessgame Chessgame { get; }
        public ChessgameProjection ChessgameProjection { get; }

        public PlayVsFriendController(AccountEntry accountEntry, Chessgame chessgame, FriendshipProjection friendshipProjection, AccountProjection accountProjection, ChessgameProjection chessgameProjection)
        {
            AccountEntry = accountEntry;
            Chessgame = chessgame;
            FriendshipProjection = friendshipProjection;
            AccountProjection = accountProjection;
            ChessgameProjection = chessgameProjection;
        }

        public VNode Render()
        {
            return Div();
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

            Game.ChessBoard Engine = new EngineControl();
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
    }
}
