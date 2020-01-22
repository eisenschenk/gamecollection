using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Text;
using VnodeTest.General;
using VnodeTest.Chess;
using VnodeTest.BC.General.Account;

namespace VnodeTest
{
    public class RootController
    {
        private readonly Session Session;
        public AccountEntry AccountEntry { get; set; }
        private Func<VNode> CurrentContent;

        private VNode RenderSideMenu()
        {
            return Div(
                Text("Account", Styles.Btn & Styles.MP4, () => CurrentContent = UserController.Render),
                Text("Play Game", Styles.Btn & Styles.MP4, () => CurrentContent = GameboardController.Render),
                Text("Friends", Styles.Btn & Styles.MP4, () => CurrentContent = FriendshipController.Render)
            );
        }

        public RootController(Session session)
        {
            Session = session;
        }

        //public VNode Render() => CurrentContent();
        public VNode Render() => ChessController.Render();

        private ChessController _ChessController;
        private ChessController ChessController =>
        _ChessController ??
        (_ChessController = ((Application)Application.Instance).ChessContext.CreateChessController(AccountEntry, ));
    }

}
