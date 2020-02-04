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

namespace VnodeTest
{
    public class RootController
    {
        private readonly Session Session;
        public AccountEntry AccountEntry { get; set; }
        private Func<VNode> CurrentContent;
        private ChessGameID ChessGameID => ChessController.GameProjection.GetGameID(AccountEntry.ID);
        private ChessGameID SolitaireGameID = default;
        public Rendermode Rendermode { get; set; } = Rendermode.Default;

        private VNode RenderSideMenu()
        {
            return Div(
                Text("Play", Styles.Btn & Styles.MP4, () => Rendermode = ChessGameID == default && SolitaireGameID == default ? Rendermode.GameSelection : Rendermode.ChessGameboard),
                Text("Friends", Styles.Btn & Styles.MP4, () => Rendermode = Rendermode.Friendcontroller)
            );
        }

        public RootController(Session session)
        {
            Session = session;
        }

        public VNode Render()
        {
            if (AccountEntry == null)
                return LoginController.Render(this);

            CurrentContent = Rendermode switch
            {
                Rendermode.Default => null,
                Rendermode.ChessGameboard => ChessController.Render,
                Rendermode.Friendcontroller => FriendshipController.Render,
                Rendermode.GameSelection => GameSelectionController.Render,
                Rendermode.SolitaireGameboard => SolitaireController.Render,
                _ => null
            };
            return Row(
                RenderSideMenu(),
                CurrentContent?.Invoke()
            );
        }

        private ChessController _ChessController;
        private ChessController ChessController =>
        _ChessController ??
        (_ChessController = ((Application)Application.Instance).ChessContext.CreateChessController(AccountEntry, this));


        private AccountController _LoginController;
        private AccountController LoginController =>
            _LoginController ??= ((Application)Application.Instance).GeneralContext.CreateLoginController();


        private FriendshipController _FriendshipController;
        private FriendshipController FriendshipController =>
            _FriendshipController ??= ((Application)Application.Instance).GeneralContext.CreateFriendshipController(AccountEntry);

        private GameSelectionController _GameSelectionController;
        private GameSelectionController GameSelectionController =>
            _GameSelectionController ??= ((Application)Application.Instance).ChessContext.CreateGameSelectionController(AccountEntry, this);

        private SolitaireController _SolitaireController;
        private SolitaireController SolitaireController =>
            _SolitaireController ??= ((Application)Application.Instance).SolitaireContext.CreateSolitaireController(AccountEntry.ID, this);


    }

}
