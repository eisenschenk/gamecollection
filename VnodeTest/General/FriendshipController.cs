using System.Collections.Generic;
using System.Linq;
using VnodeTest.BC.General.Account;
using VnodeTest.BC.Chess.Game;
using static ACL.UI.React.DOM;
using VnodeTest.BC.General.Friendships;
using FriendID = ACL.ES.AggregateID<VnodeTest.BC.General.Friendships.Friendship>;
using ACL.UI.React;
using System;

namespace VnodeTest.General
{
    public partial class FriendshipController
    {
        public AccountEntry AccountEntry { get; }
        public AccountProjection AccountProjection { get; }
        public ChessgameProjection GameProjection { get; }
        public FriendshipProjection FriendshipProjection { get; }
        private VNode RefreshReference;
        private Func<VNode> RenderCurrentcontent;

        public FriendshipController(AccountEntry accountEntry, AccountProjection accountProjection, ChessgameProjection gameProjection,
            FriendshipProjection friendshipProjection, RootController rootController)
        {
            AccountEntry = accountEntry;
            AccountProjection = accountProjection;
            GameProjection = gameProjection;
            FriendshipProjection = friendshipProjection;
            RenderCurrentcontent = RenderOverview;
        }

        public VNode Render()
        {
            var friends = FriendshipProjection.GetFriends(AccountEntry.ID);
            var _friends = FriendshipProjection.GetFriends(AccountEntry.ID)?.Select(a => AccountProjection[a.AccountID]);

            return Div(
                Row(
                    Text("Friendlist", RenderCurrentcontent == RenderOverview ? Styles.TabMenuItemSelected : Styles.TabMenuItem, () => RenderCurrentcontent = RenderOverview),
                    Text("Add Friend", RenderCurrentcontent == RenderAddFriend ? Styles.TabMenuItemSelected : Styles.TabMenuItem, () => RenderCurrentcontent = RenderAddFriend),
                    Text($"Pending Friendrequests {GetStyledNumber()}",
                        (RenderCurrentcontent == RenderReceivedRequests ? Styles.TabMenuItemSelected : Styles.TabMenuItem) & Styles.W12C,
                        () => RenderCurrentcontent = RenderReceivedRequests)
                ),
                RefreshReference = RenderCurrentcontent?.Invoke()
            );
        }

        private string GetStyledNumber()
        {
            return FriendshipProjection.GetFriendshipRequestCount(AccountEntry.ID) switch
            {
                0 => "⓿",
                1 => "❶",
                2 => "❷",
                3 => "❸",
                4 => "❹",
                5 => "❺",
                6 => "❻",
                7 => "❼",
                8 => "❽",
                9 => "❾",
                10 => "❿",
                11 => "⓫",
                12 => "⓬",
                13 => "⓭",
                _ => "∞",
            };
        }

        private VNode RenderOverview()
        {
            var friends = FriendshipProjection.GetFriends(AccountEntry.ID)?.Select(a => AccountProjection[a.AccountID]);
            return Div(
                friends.Any()
                ? Fragment(friends.Select(f => Row(
                    Text($"{f.Username}", Styles.TabNameTag),
                    Text("Remove", Styles.TabButtonSelected, () =>
                        Friendship.Commands.AbortFriend(FriendshipProjection.GetSpecificFriendshipID(AccountEntry.ID, f.ID).FirstOrDefault()))
                )))
                : Text("you got no friends ;(")
            );
        }

        private VNode RenderReceivedRequests()
        {
            return Div(
                Fragment(FriendshipProjection.GetFriendshipRequests(AccountEntry.ID).Select(p => Row(
                    Text(AccountProjection[p.Sender].Username, Styles.TabNameTag),
                    Text("Accept", Styles.TabButton, () => Friendship.Commands.AcceptFriendRequest(p.ID)),
                    Text("Deny", Styles.TabButtonSelected, () => Friendship.Commands.DenyFriendRequest(p.ID))
                )))
            );
        }

        private VNode RenderAddFriend()
        {
            var friends = FriendshipProjection.GetFriends(AccountEntry.ID).Select(a => a.AccountID);
            return Div(
                SearchbarComponent<BefriendedAccountEntrySearchWrapper>.Render(AccountProjection.Accounts
                .Where(a => !friends.Contains(a.ID) && a.ID != AccountEntry.ID)
                .Select(a => new BefriendedAccountEntrySearchWrapper(a, default)),
                    w => Friendship.Commands.RequestFriend(FriendID.Create(), AccountEntry.ID, w.AccountEntry.ID))
            );
        }

        //private VNode RenderDeleteFriend()
        //{
        //    var friends = FriendshipProjection.GetFriends(AccountEntry.ID).Select(t =>
        //        new BefriendedAccountEntrySearchWrapper(AccountProjection[t.AccountID], t.FriendshipID));

        //    return Div(
        //        SearchbarComponent<BefriendedAccountEntrySearchWrapper>.Render(friends, w =>
        //            Friendship.Commands.AbortFriend(w.FriendshipID))
        //    );
        //}

        private class BefriendedAccountEntrySearchWrapper : ISearchable
        {
            public AccountEntry AccountEntry { get; }
            public FriendID FriendshipID { get; }

            public BefriendedAccountEntrySearchWrapper(AccountEntry accountEntry, FriendID friendshipID)
            {
                AccountEntry = accountEntry;
                FriendshipID = friendshipID;
            }

            VNode ISearchable.Render()
            {
                return Text(AccountEntry.Username);
            }
            bool ISearchable.IsMatch(string searchquery)
            {
                return AccountEntry.Username.Contains(searchquery);
            }
        }

    }
}
