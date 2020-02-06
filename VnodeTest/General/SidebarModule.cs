using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VnodeTest.BC.General.Account;
using VnodeTest.Chess;
using VnodeTest.Solitaire;
using static ACL.UI.React.DOM;
using ChessGameID = ACL.ES.AggregateID<VnodeTest.BC.Chess.Game.Chessgame>;
using SolitaireGameID = ACL.ES.AggregateID<VnodeTest.BC.Solitaire.Solitaire>;

namespace VnodeTest.General
{
    public class SidebarModule
    {
        public Func<VNode> CurrentContent { get; set; }
        private readonly AccountEntry AccountEntry;
        private ChessGameID ChessGameID => ChessController.GameProjection.GetGameID(AccountEntry.ID);
        //private readonly ChessGameID SolitaireGameID = default;
        private readonly ChessController ChessController;
        private readonly SolitaireController SolitaireController;
        private readonly GameSelectionController GameSelectionController;
        private readonly FriendshipController FriendshipController;
        public SidebarModule(AccountEntry accountEntry, ChessController chessController, SolitaireController solitaireController,
            GameSelectionController gameSelectionController, FriendshipController friendshipController)
        {
            AccountEntry = accountEntry;
            ChessController = chessController;
            SolitaireController = solitaireController;
            GameSelectionController = gameSelectionController;
            FriendshipController = friendshipController;
        }

        public VNode Render()
        {
            //CurrentContent = SidebarModule.CurrentContent?.Invoke();
            //if (ChessGameID != default && !WasRefreshed)
            //{
            //    CurrentContent.Refresh();
            //    WasRefreshed = true;
            //}


            return Div(
                Styles.Sidebar,
                //+pic
                Text(AccountEntry.Username, Styles.MainMenuItem),

                Text("Games", Styles.MainMenuItem),
                Text("⁍ Chess", Styles.SubMenuItem, () => CurrentContent = ChessGameID == default ? (Func<VNode>)GameSelectionController.Render : ChessController.Render), //TODO need rework
                Text("⁍ Solitaire", Styles.SubMenuItem, () => CurrentContent = SolitaireController.Render), //TODO need rework

                Text("Account", Styles.MainMenuItem),
                Text("⁍ Friends", Styles.SubMenuItem, () => CurrentContent = FriendshipController.Render), //TODO need rework
                Text("⁍ Settings", Styles.SubMenuItem /*, () => CurrentContent = sth.Render*/),

                Text("⁍ Logout", Styles.SubMenuItem /*, () => CurrentContent = sth.Render*/)
            );
        }

        //private void PressPlay()
        //{
        //    if (ChessGameID != default)
        //        Rendermode = Rendermode.ChessGameboard;
        //    if (SolitaireGameID != default)
        //        Rendermode = Rendermode.SolitaireGameboard;

        //    BreadCrumbs.Clear();
        //    BreadCrumbs.Push(new Crumb("Gameselection", Rendermode = Rendermode.GameSelection, LocalRendermode = LocalRendermode.Default));
        //    Rendermode = Rendermode.GameSelection;
        //}

        //private void PressFriends()
        //{
        //    BreadCrumbs.Clear();
        //    BreadCrumbs.Push(new Crumb("Friends", Rendermode = Rendermode.Friendcontroller, LocalRendermode = LocalRendermode.Default));
        //    Rendermode = Rendermode.Friendcontroller;
        //}

        //public void ResetRendermode(string name)
        //{
        //    while (BreadCrumbs.Peek().Name != name)
        //        BreadCrumbs.Pop();
        //    Rendermode = BreadCrumbs.Peek().Rendermode;
        //    LocalRendermode = BreadCrumbs.Peek().LocalRendermode;
        //}


        //public void SaveRendermode(string name, LocalRendermode localRendermode, Rendermode rendermode)
        //{
        //    if (BreadCrumbs.Peek().Name != name)
        //        BreadCrumbs.Push(new Crumb(name, rendermode, localRendermode));
        //}

        //private VNode RenderCrumbMenu()
        //{
        //    if (Rendermode == Rendermode.ChessGameboard || Rendermode == Rendermode.SolitaireGameboard)
        //        return null;

        //    return Row(Fragment(BreadCrumbs.Select(c => Text(c.Name, Styles.SelectedBtn/*, ResetRendermode(c.Name)*/))));

        //}

    }
}
