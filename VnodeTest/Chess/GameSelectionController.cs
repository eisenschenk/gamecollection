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
using VnodeTest.General;

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
        private (Gamemode Mode, ClockTimer ClockTimer) GameSelection;

        //private RendermodeLocal RenderMode;

        public RootController RootController { get; set; }

        public GameSelectionController(AccountEntry accountEntry, FriendshipProjection friendshipProjection, AccountProjection accountProjection,
            ChessgameProjection chessgameProjection)
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
            var challengesFromFriends = GameProjection.Games.Where(g => !g.Closed && g.Receiver == AccountEntry.ID);

            if (challengesFromFriends.Any() && Game == default)
                return RenderChallenges(challengesFromFriends);
            return RenderGameModeSelection();
        }

        private VNode RenderGameModeSelection()
        {
            return Div(
                Row(
                    Text("Player vs. AI", GameSelection.Mode == Gamemode.PvE ? Styles.TabMenuItemSelected : Styles.TabMenuItem, () => GameSelection.Mode = Gamemode.PvE),
                    Text("AI vs. AI", GameSelection.Mode == Gamemode.EvE ? Styles.TabMenuItemSelected : Styles.TabMenuItem, () => GameSelection.Mode = Gamemode.EvE),
                    Text("Play vs. Friend", GameSelection.Mode == Gamemode.PvF ? Styles.TabMenuItemSelected : Styles.TabMenuItem, () => GameSelection.Mode = Gamemode.PvF)
                ),
                Row(
                    Text("normal", GameSelection.ClockTimer == ClockTimer.Normal ? Styles.TabMenuItemSelected : Styles.TabMenuItem, () => GameSelection.ClockTimer = ClockTimer.Normal),
                    Text("blitz", GameSelection.ClockTimer == ClockTimer.Blitz ? Styles.TabMenuItemSelected : Styles.TabMenuItem, () => GameSelection.ClockTimer = ClockTimer.Blitz),
                    Text("bullet", (GameSelection.ClockTimer == ClockTimer.Bullet ? Styles.TabMenuItemSelected : Styles.TabMenuItem) & Styles.MB2P5rem, () => GameSelection.ClockTimer = ClockTimer.Bullet)
                ),
                GameSelection.ClockTimer != default && GameSelection.Mode != default
                    ? GameSelection.Mode != Gamemode.PvF
                        ? Text("Start", Styles.TabButton, () => SelectGameMode())
                        : RenderChallengeFriend()
                    : null
            );
        }

        private double GetClocktimer()
        {
            return GameSelection.ClockTimer switch
            {
                ClockTimer.Normal => 3600,
                ClockTimer.Blitz => 500,
                ClockTimer.Bullet => 250,
                _ => throw new NotImplementedException(),
            };
        }

        private void SelectGameMode()
        {
            var gID = GameID.Create();
            Chessgame.Commands.OpenGame(gID, GameSelection.Mode, GetClocktimer());
            Chessgame.Commands.JoinGame(gID, AccountEntry.ID);
            //TODO here?
            //SidebarModule.CurrentContent = SidebarModule.ChessController.Render;
            Game.Engine = new EngineControl();
            //EvE loop
            if (GameSelection.Mode == Gamemode.EvE)
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
            VNode renderFriends(AccountEntry friend)
            {
                var friendChallenged = GameProjection.Games.Where(g => g.Challenger == AccountEntry.ID && g.Receiver == friend.ID && !g.Closed).FirstOrDefault();

                //friend already challenged -> render deny challenge option
                if (friendChallenged != default)
                    return GameProjection[friendChallenged.ID].HasOpenSpots && friendChallenged.Created.AddSeconds(friendChallenged.Timer) > DateTime.Now
                        ? Row(
                            Text($"placeholder - {friendChallenged.Timer - friendChallenged.Elapsed.Seconds}", Styles.TabNameTag),
                            Text("Abort Challenge!", Styles.TabButtonSelected, () => Chessgame.Commands.DenyChallenge(friendChallenged.ID))
                        )
                        : null;

                //render challenge friend option
                return Row(
                            Text(friend.Username, Styles.TabNameTag),
                            Text("Challenge", Styles.TabMenuItem, () => challengeFriend(friend))
                        );
            }

            void challengeFriend(AccountEntry friend)
            {
                var gID = GameID.Create();
                Chessgame.Commands.OpenGame(gID, Gamemode.PvF, GetClocktimer());
                Chessgame.Commands.RequestChallenge(gID, AccountEntry.ID, friend.ID);
            }
            var friends = FriendshipProjection.GetFriends(AccountEntry.ID)?.Select(id => AccountProjection[id.AccountID]);

            if (friends != default)
                return Fragment(friends.Select(f => renderFriends(f)));

            return Text("no friends -_-'");
        }


    }
}
