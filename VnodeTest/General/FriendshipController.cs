﻿using System.Collections.Generic;
using System.Linq;
using VnodeTest.BC.General.Account;
using VnodeTest.BC.Chess.Game;
using static ACL.UI.React.DOM;
using VnodeTest.BC.General.Friendships;
using FriendID = ACL.ES.AggregateID<VnodeTest.BC.General.Friendships.Friendship>;
using ACL.UI.React;

namespace VnodeTest.General
{
    public partial class FriendshipController
    {
        public AccountEntry AccountEntry { get; }
        public AccountProjection AccountProjection { get; }
        public ChessgameProjection GameProjection { get; }
        public FriendshipProjection FriendshipProjection { get; }
        private RenderMode Rendermode = RenderMode.Overview;

        public FriendshipController(AccountEntry accountEntry, AccountProjection accountProjection, ChessgameProjection gameProjection, FriendshipProjection friendshipProjection)
        {
            AccountEntry = accountEntry;
            AccountProjection = accountProjection;
            GameProjection = gameProjection;
            FriendshipProjection = friendshipProjection;
        }

        public VNode Render()
        {
            var friends = FriendshipProjection.GetFriends(AccountEntry.ID);
            var _friends = FriendshipProjection.GetFriends(AccountEntry.ID)?.Select(a => AccountProjection[a.AccountID]);

            return Rendermode switch
            {
                RenderMode.Overview => RenderOverview(_friends),
                RenderMode.AddFriend => RenderAddFriend(),
                RenderMode.DeleteFriend => RenderDeleteFriend(friends.Select(t => new BefriendedAccountEntrySearchWrapper(AccountProjection[t.AccountID], t.FriendshipID))),
                RenderMode.PendingRequests => RenderReceivedRequests(),
                _ => null,
            };
        }

        private VNode RenderOverview(IEnumerable<AccountEntry> friendAccounts)
        {
            return Div(
                Text($"Pending Friendrequests({FriendshipProjection.GetFriendshipRequestCount(AccountEntry.ID)})", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.PendingRequests),
                Text("Add Friend", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.AddFriend),
                Text("Remove Friend", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.DeleteFriend),
                friendAccounts.Any() ? Fragment(friendAccounts.Select(f => Text($"{f.Username}", !f.LoggedIn ? Styles.TCblack : Styles.TCgreen))) : Text("you got no friends ;(")
            );
        }

        private VNode RenderReceivedRequests()
        {
            return Div(
                Fragment(FriendshipProjection.GetFriendshipRequests(AccountEntry.ID).Select(p => Row(
                    Text(AccountProjection[p.Sender].Username),
                    Text("accept", Styles.Btn & Styles.MP4, () => Friendship.Commands.AcceptFriendRequest(p.ID)),
                    Text("deny", Styles.Btn & Styles.MP4, () => Friendship.Commands.DenyFriendRequest(p.ID))
                ))),
                Text("back", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.Overview)
            );
        }

        private VNode RenderAddFriend()
        {
            var friends = FriendshipProjection.GetFriends(AccountEntry.ID).Select(a => a.AccountID);
            return Div(
                SearchbarComponent<BefriendedAccountEntrySearchWrapper>.Render(AccountProjection.Accounts
                .Where(a => !friends.Contains(a.ID) && a.ID != AccountEntry.ID)
                .Select(a => new BefriendedAccountEntrySearchWrapper(a, default)),
                    w => Friendship.Commands.RequestFriend(FriendID.Create(), AccountEntry.ID, w.AccountEntry.ID)),

                Text("back", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.Overview)
            );
        }

        private VNode RenderDeleteFriend(IEnumerable<BefriendedAccountEntrySearchWrapper> friends)
        {
            return Div(
                SearchbarComponent<BefriendedAccountEntrySearchWrapper>.Render(friends, w =>
                    Friendship.Commands.AbortFriend(w.FriendshipID)),
                Text("back", Styles.Btn & Styles.MP4, () => Rendermode = RenderMode.Overview)
            );
        }

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
