using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Text;
using VnodeTest.General;
using VnodeTest.Chess;
using VnodeTest.BC.General.Account;
using static ACL.UI.React.DOM;
using ChessGameID = ACL.ES.AggregateID<VnodeTest.BC.Chess.Game.Chessgame>;
using SolitaireGameID = ACL.ES.AggregateID<VnodeTest.BC.Solitaire.Solitaire>;
using VnodeTest.Solitaire;
using static VnodeTest.General.FriendshipController;
using System.Linq;
using System.Threading;

namespace VnodeTest
{
    public class RootController
    {
        private readonly Session Session;
        public AccountEntry AccountEntry => LoginController.CurrentUser;
        private VNode CurrentContent;
        private ChessGameID ChessGameID => AccountEntry != null ? ChessController?.GameProjection.GetGameID(AccountEntry.ID) ?? default : default;
        private ChessGameID SolitaireGameID = default;
        public Rendermode Rendermode { get; set; } = Rendermode.Default;
        public LocalRendermode LocalRendermode { get; set; } = LocalRendermode.Default;
        public Stack<Crumb> BreadCrumbs { get; } = new Stack<Crumb>();
        public SidebarModule SidebarModule { get; set; } = default;
        private bool HasRefreshed;



        public RootController(Session session)
        {
            Session = session;
            ThreadPool.QueueUserWorkItem(o =>
            {
                while (true)
                {
                    if (AccountEntry != default && ChessGameID == default && ChessController.LastGame == default)
                        HasRefreshed = false;
                    if (AccountEntry != default && ChessGameID != default && HasRefreshed == false)
                    {
                        SidebarModule.CurrentContent = ChessController.Render;
                        HasRefreshed = true;
                    }
                }
            });
        }

        public VNode Render()
        {
            if (AccountEntry == null)
                return LoginController.Render();

            if (SidebarModule == default)
                SidebarModule = new SidebarModule(AccountEntry, ChessController, SolitaireController, GameSelectionController, FriendshipController);

            return Row(
                SidebarModule.Render(),
                Div(
                    Styles.MainWindow,
                    CurrentContent = SidebarModule.CurrentContent?.Invoke()
                )
            );
        }

        private ChessController _ChessController;
        private ChessController ChessController =>
        _ChessController ??
        (_ChessController = ((Application)Application.Instance).ChessContext.CreateChessController(AccountEntry));


        private LoginController _LoginController;
        private LoginController LoginController =>
            _LoginController ??= ((Application)Application.Instance).GeneralContext.CreateLoginController();


        private FriendshipController _FriendshipController;
        private FriendshipController FriendshipController =>
            _FriendshipController ??= ((Application)Application.Instance).GeneralContext.CreateFriendshipController(AccountEntry, this);

        private GameSelectionController _GameSelectionController;
        private GameSelectionController GameSelectionController =>
            _GameSelectionController ??= ((Application)Application.Instance).ChessContext.CreateGameSelectionController(AccountEntry);

        private SolitaireController _SolitaireController;
        private SolitaireController SolitaireController =>
            _SolitaireController ??= ((Application)Application.Instance).SolitaireContext.CreateSolitaireController(AccountEntry.ID, this);


    }

}
