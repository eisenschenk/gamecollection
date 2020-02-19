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
        public AccountEntry CurrentUser { get; set; }
        private Func<VNode> CurrentContent;


        public LoginController(AccountProjection accountProjection)
        {
            AccountProjection = accountProjection;
            CurrentContent = RenderLogin;
        }

        public VNode Render()
        {
            return Div(
                Styles.LoginRegister,
                RenderLoginRegisterSelection(),
                Div(CurrentContent?.Invoke())
            );
        }
        private VNode RenderLoginRegisterSelection()
        {
            return Row(
                Text("Register Account", CurrentContent == RenderRegisterAccount ? Styles.TabMenuItemSelected : Styles.TabMenuItem, () => CurrentContent = RenderRegisterAccount),
                Text("Login", CurrentContent == RenderLogin ? Styles.TabMenuItemSelected : Styles.TabMenuItem, () => CurrentContent = RenderLogin)
            );
        }

        private VNode RenderLogin()
        {
            VNode wrongUsernamePassword()
            {
                return Text("Wrong Username/Password!", Styles.AbortBtn & Styles.MP4, () => CurrentContent = RenderLogin);
            }
            return Div(
                Styles.BorderWhiteSolid & Styles.BCMain & Styles.FitContent & Styles.MY2,
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
                Styles.BorderWhiteSolid & Styles.BCMain & Styles.FitContent & Styles.MY2,
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
                    }
                    else
                        CurrentContent = usernameTaken;
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
