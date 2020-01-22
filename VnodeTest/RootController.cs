using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Text;
using VnodeTest.General;
using VnodeTest.Chess;
using VnodeTest.BC.General.Account;
using static ACL.UI.React.DOM;


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
                Text("Play Game", Styles.Btn & Styles.MP4, () => CurrentContent = ChessController.Render),
                Text("Friends", Styles.Btn & Styles.MP4, () => CurrentContent = FriendshipController.Render)
            );
        }

        public RootController(Session session)
        {
            Session = session;
        }

        public VNode Render() => AccountEntry == null ? LoginController.Render(this) : Row(RenderSideMenu(), CurrentContent?.Invoke());


        private ChessController _ChessController;
        private ChessController ChessController =>
        _ChessController ??
        (_ChessController = ((Application)Application.Instance).ChessContext.CreateChessController(AccountEntry));


        private AccountController _LoginController;
        private AccountController LoginController =>
            _LoginController ??= ((Application)Application.Instance).GeneralContext.CreateLoginController();


        private FriendshipController _FriendshipController;
        private FriendshipController FriendshipController =>
            _FriendshipController ??= ((Application)Application.Instance).GeneralContext.CreateFriendshipController(AccountEntry);
    }

}
