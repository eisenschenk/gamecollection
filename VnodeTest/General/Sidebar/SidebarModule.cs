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

namespace VnodeTest.General.Sidebar
{
    public class SidebarModule
    {
        public IEnumerable<SidebarBaseEntry> SidebarEntries { get; }

        public SidebarModule(IEnumerable<SidebarBaseEntry> sidebarEntries)
        {
            SidebarEntries = sidebarEntries;
        }

        public VNode Render()
        {
            return 
                Fragment(
                    Div(
                        Styles.Sidebar,
                        SidebarEntries.Select(s => s.Render())
                    )
                );
        }










        //old sidebar

        //private readonly AccountEntry AccountEntry;
        //private ChessGameID ChessGameID => ChessController.GameProjection.GetGameID(AccountEntry.ID);
        ////private readonly ChessGameID SolitaireGameID = default;
        //private readonly ChessController ChessController;
        //private readonly SolitaireController SolitaireController;
        //private readonly GameSelectionController GameSelectionController;
        //private readonly FriendshipController FriendshipController;

        //public SidebarModule(AccountEntry accountEntry, ChessController chessController, SolitaireController solitaireController,
        //    GameSelectionController gameSelectionController, FriendshipController friendshipController)
        //{
        //    AccountEntry = accountEntry;
        //    ChessController = chessController;
        //    SolitaireController = solitaireController;
        //    GameSelectionController = gameSelectionController;
        //    FriendshipController = friendshipController;
        //}


        //olf render

        //public VNode Render()
        //{
        //    return Div(
        //        Styles.Sidebar,
        //        //+pic
        //        Text(AccountEntry.Username, Styles.MainMenuItem),

        //        Text("Games", Styles.MainMenuItem),
        //        DOM.Icon("fas fa-chess-knight"),
        //        Text("Chess", Styles.SubMenuItem, () => CurrentContent = ChessGameID == default ? (Func<VNode>)GameSelectionController.Render : ChessController.Render), //TODO need rework
        //        Text("⁍ Solitaire", Styles.SubMenuItem, () => CurrentContent = SolitaireController.Render), //TODO need rework

        //        Text("Account", Styles.MainMenuItem),
        //        Text("⁍ Friends", Styles.SubMenuItem, () => CurrentContent = FriendshipController.Render), //TODO need rework
        //        Text("⁍ Settings", Styles.SubMenuItem /*, () => CurrentContent = sth.Render*/),

        //        Text("⁍ Logout", Styles.SubMenuItem /*, () => CurrentContent = sth.Render*/)
        //    );
        //}
    }
}
