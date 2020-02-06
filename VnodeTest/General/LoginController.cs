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
        public AccountEntry CurrentUser;
        private Rendermode CurrentRendermode = Rendermode.Default;
        private Func<VNode> CurrentContent;


        public LoginController(AccountProjection accountProjection)
        {
            AccountProjection = accountProjection;
        }

        public VNode Render()
        {
            return Div(
                RenderLoginRegisterSelection(),
                Div(Styles.MainWindow, CurrentContent?.Invoke())
            );
        }
        private VNode RenderLoginRegisterSelection()
        {
            return Div(
                Styles.Sidebar,
                Text("Login & Register", Styles.MainMenuItem),
                Text("Register Account", Styles.SubMenuItem, () => CurrentContent = RenderRegisterAccount),
                Text("Login", Styles.SubMenuItem, () => CurrentContent = RenderLogin)
            );
        }

        private VNode RenderLogin()
        {
            VNode wrongUsernamePassword()
            {
                return Text("Wrong Username/Password!", Styles.AbortBtn & Styles.MP4, () => CurrentContent = RenderLogin);
            }
            return Div(
                Styles.BorderWhiteSolid & Styles.BCMain & Styles.FitContent & Styles.M2,
                Input(Username, s => Username = s, Styles.MP2),
                Input(Password, s => Password = s, Styles.MP2).WithPassword(),
                Text("Login ", Styles.TabButton, () =>
                {
                    try
                    {
                        Account.Commands.LoginAccount(GetUser().ID, Password);
                    }
                    catch (ArgumentException)
                    {
                        CurrentContent = wrongUsernamePassword;
                    }
                    CurrentUser = GetUser();
                })
            );
        }

        private VNode RenderRegisterAccount()
        {
            VNode usernameTaken()
            {
                return Text("Username taken!", Styles.AbortBtn & Styles.MP4, () => CurrentContent = RenderRegisterAccount);
            }
            return Div(
                Styles.BorderWhiteSolid & Styles.BCMain & Styles.FitContent & Styles.M2,
                Input(Username, s => Username = s, Styles.MP2),
                Input(Password, s => Password = s, Styles.MP2).WithPassword(),
                Text("Register Account", Styles.TabButton, () =>
                {
                    if (!AccountProjection.Accounts.Select(x => x.Username).Contains(Username))
                    {
                        try
                        {
                            Account.Commands.RegisterAccount(ACL.ES.AggregateID<Account>.Create(), Username, Password);
                        }
                        catch (ArgumentException)
                        {
                            CurrentContent = usernameTaken;
                        }
                        Username = string.Empty;
                        Password = string.Empty;
                        CurrentRendermode = Rendermode.Default;
                    }
                    else
                        CurrentRendermode = Rendermode.Register;
                })
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
