using ACL.UI.React;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.BC.General.Account;
using static ACL.UI.React.DOM;

namespace VnodeTest.General
{
    public class LoginController
    {
        private AccountProjection AccountProjection;
        private string Username;
        private string Password;
        private Rendermode CurrentRendermode = Rendermode.Default;


        public LoginController(AccountProjection accountProjection)
        {
            AccountProjection = accountProjection;
        }

        public VNode Render(RootController rootController)
        {
            return CurrentRendermode switch
            {
                Rendermode.Default => RenderLoginRegisterSelection(),
                Rendermode.Login => RenderLogin(rootController),
                Rendermode.Register => RenderRegisterAccount(),
                Rendermode.WrongUsernamePassword => Text("Wrong Username/Password!", Styles.AbortBtn & Styles.MP4, () => CurrentRendermode = Rendermode.Login),
                Rendermode.UsernameTaken => Text("Username taken!", Styles.AbortBtn & Styles.MP4, () => CurrentRendermode = Rendermode.Register)
            };
        }
        private VNode RenderLoginRegisterSelection()
        {
            return Div(
                Text("Register", Styles.Btn & Styles.MP4, () => CurrentRendermode = Rendermode.Register),
                Text("Login", Styles.Btn & Styles.MP4, () => CurrentRendermode = Rendermode.Login)
            );
        }

        private VNode RenderLogin(RootController rootController)
        {
            return Div(
                Input(Username, s => Username = s),
                Input(Password, s => Password = s).WithPassword(),
                Text("login ", Styles.Btn, () =>
                {
                    try
                    {
                        Account.Commands.LoginAccount(GetUser().ID, Password);
                    }
                    catch (ArgumentException)
                    {
                        CurrentRendermode = Rendermode.WrongUsernamePassword;
                    }
                    rootController.AccountEntry = GetUser();
                }),
                Text("back", Styles.Btn & Styles.MP4, () => CurrentRendermode = Rendermode.Default)
            );
        }

        private VNode RenderRegisterAccount()
        {

            return Div(
                Input(Username, s => Username = s),
                Input(Password, s => Password = s).WithPassword(),
                Text("register Account", Styles.Btn, () =>
                {
                    if (!AccountProjection.Accounts.Select(x => x.Username).Contains(Username))
                    {
                        //TODO: try catch so accountcreation doesnt go through
                        Account.Commands.RegisterAccount(ACL.ES.AggregateID<Account>.Create(), Username, Password);
                        Username = string.Empty;
                        Password = string.Empty;
                        CurrentRendermode = Rendermode.UsernameTaken;
                    }
                    else
                        CurrentRendermode = Rendermode.Register;
                }),
                Text("back", Styles.Btn & Styles.MP4, () => CurrentRendermode = Rendermode.Default)
            );
        }

        private AccountEntry GetUser() => AccountProjection.Accounts.Where(p => p.Username == Username).SingleOrDefault();

        private enum Rendermode
        {
            Default,
            Login,
            Register,
            WrongUsernamePassword,
            UsernameTaken
        }
    }
}
