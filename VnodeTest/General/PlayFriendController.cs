using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.BC.General.Account;
using VnodeTest.BC.Chess.Game;

namespace VnodeTest.General
{
    public class PlayFriendController
    {
        //public PlayFriendController(AccountEntry accountEntry, Chessgame)
        //{

        //}

        //private VNode RenderWaitingRoom()
        //{
        //    var challenges = GameProjection.Games.Where(g => g.Challenger == AccountEntry.ID);
        //    var activeGame = GameProjection.Games.Where(g => g.PlayerWhite == AccountEntry.ID && !g.Closed).FirstOrDefault();
        //    //action if challenge was accepted 
        //    if (activeGame != default)
        //    {
        //        Game = GameProjection[GameID].Game;
        //        PlayerColor = PieceColor.White;
        //        Game.HasWhitePlayer = true;
        //        RenderMode = Rendermode.Gameboard;
        //    }
        //    //waiting for anyone to accept challenge
        //    return Div(
        //        Fragment(challenges.Select(c =>
        //            Div(
        //                Game.HasOpenSpots && c.Created.AddSeconds(c.Timer) > DateTime.Now
        //                    ? Row(
        //                        Text($"Waiting for Friend: {c.Timer - c.Elapsed.Seconds}"),
        //                        Text("Abort Challenge!", Styles.AbortBtn & Styles.MP4, () => BC.Chess.Game.Chessgame.Commands.DenyChallenge(c.ID))
        //                    )
        //                    : null
        //            )
        //        )),
        //        Text("back", Styles.Btn & Styles.MP4, () => { Game = null; RenderMode = Rendermode.Gameboard; })
        //    );
        //}

        //private VNode RenderChallenges(IEnumerable<BC.Chess.Game.GameEntry> challenges)
        //{
        //    return Div(
        //        Fragment(challenges.Select(c =>
        //              Row(
        //                  Text(AccountProjection[c.Challenger].Username),
        //                  Text("Accept", Styles.Btn & Styles.MP4, () =>
        //                  {
        //                      BC.Chess.Game.Chessgame.Commands.AcceptChallenge(c.ID, c.Challenger, c.Receiver);
        //                      GameID = c.ID;
        //                      Game = GameProjection[GameID].Game;
        //                      PlayerColor = PieceColor.Black;
        //                      //TODO: move to acceptchallenge
        //                      Game.HasBlackPlayer = true;
        //                  }),
        //                  Text("Deny", Styles.Btn & Styles.MP4, () => BC.Chess.Game.Chessgame.Commands.DenyChallenge(c.ID))
        //              )
        //        )));
        //}

        //private VNode RenderChallengeFriend()
        //{
        //    var friends = FriendshipProjection.GetFriends(AccountEntry.ID)?.Select(id => AccountProjection[id.AccountID]);
        //    VNode back = Text("back", Styles.Btn & Styles.MP4, () => RenderMode = Rendermode.Gameboard);

        //    if (friends != default)
        //        return Div(
        //            Fragment(friends.Select(f =>
        //                    Row(
        //                        Text(f.Username),
        //                        Text("Challenge", Styles.Btn & Styles.MP4, () => RenderClockTimerMode = RenderClockTimer.PvF),
        //                        RenderClockTimerMode == RenderClockTimer.PvF
        //                            ? Row(
        //                                Text("normal", Styles.Btn & Styles.MP4, () => ChallengeFriend(f, 3600)),
        //                                Text("blitz", Styles.Btn & Styles.MP4, () => ChallengeFriend(f, 300)),
        //                                Text("bullet", Styles.Btn & Styles.MP4, () => ChallengeFriend(f, 120)))
        //                            : null
        //                    )
        //            )),
        //            back
        //        );
        //    return Div(
        //        Text("no friends -_-'"),
        //        back
        //    );
        //}

        //private void ChallengeFriend(AccountEntry accountEntry, double clocktimer)
        //{
        //    GameID = GameID.Create();
        //    BC.Chess.Game.Chessgame.Commands.OpenGame(GameID, Gamemode.PvF, clocktimer);
        //    BC.Chess.Game.Chessgame.Commands.RequestChallenge(GameID, AccountEntry.ID, accountEntry.ID);
        //    Game = GameProjection[GameID].Game;
        //    PlayerColor = PieceColor.White;
        //    //TODO: move to requestchallenge
        //    Game.HasWhitePlayer = true;
        //    RenderMode = Rendermode.WaitingForChallenged;
        //    RenderClockTimerMode = RenderClockTimer.Default;
        //}
    }
}
