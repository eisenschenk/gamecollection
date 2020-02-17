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
    public class SettingsController
    {
        public AccountEntry AccountEntry { get; set; }
        public AccountProjection AccountProjection { get; }
        private Func<VNode> CurrentContent;
        private string Username;
        private string Icon;

        private string OldPassword;
        private string NewPassword;
        private string PasswordRepeat;

        private bool WrongPasswords;
        private bool OldPasswordWrong;


        public SettingsController(AccountEntry accountEntry, AccountProjection accountProjection)
        {
            AccountEntry = accountEntry;
            AccountProjection = accountProjection;
            CurrentContent = RenderPersonalInfo;
            Icon = accountEntry.Icon;
        }

        public VNode Render() => Div(RenderMenuItems(), CurrentContent());

        private VNode RenderMenuItems()
        {
            return Row(
                Text("Personal Info", CurrentContent == RenderPersonalInfo ? Styles.TabMenuItemSelected : Styles.TabMenuItem, () => CurrentContent = RenderPersonalInfo),
                Text("Game Settings", CurrentContent == RenderGameSettings ? Styles.TabMenuItemSelected : Styles.TabMenuItem, () => CurrentContent = RenderGameSettings),
                Text("Security", CurrentContent == RenderSecuritySettings ? Styles.TabMenuItemSelected : Styles.TabMenuItem, () => CurrentContent = RenderSecuritySettings)
            );
        }

        private VNode RenderPersonalInfo()
        {
            return Div(
                Div(
                    Styles.TabNameTagNoWidth & Styles.FitContent,
                    Text("Change Username", Styles.Fontcopperplate & Styles.Underline & Styles.MB2P5rem),
                    Row(
                        Text("Current Username:", Styles.W8C),
                        Text(AccountEntry.Username)
                    ),
                    Row(
                        Text("New Username:", Styles.W8C),
                        Input(Username, u => Username = u),
                        Text("Apply", Styles.BtnSettings, () => Account.Commands.ChangeUsername(AccountEntry.ID, Username))
                    )
                ),
                Div(
                    Styles.TabNameTagNoWidth & Styles.FitContent,
                    Text("Change Icon", Styles.Fontcopperplate & Styles.Underline & Styles.MB2P5rem),
                    Row(
                        DropdownComponent<string>.Render(AccountProjection.GetIcons(), i => Icon = i, "Change Icon", i => Div(DOM.Icon(i, Styles.TCblack))),
                        DOM.Icon(Icon != default ? Icon : "fas fa-question-circle", Styles.MX1),
                        Text("Apply", Styles.BtnSettings & Styles.MX1, () => Account.Commands.ChangeIcon(AccountEntry.ID, Icon))
                    )
                )
            );
        }

        private VNode RenderGameSettings()
        {
            return Text("random menu2");
        }

        private VNode RenderSecuritySettings()
        {
            return Div(
                Div(
                    Styles.TabNameTagNoWidth & Styles.FitContent,
                    Text("Change Password", Styles.Fontcopperplate & Styles.Underline & Styles.MB2P5rem),
                    Row(
                        Text("Old Password:", Styles.W8C),
                        Input(OldPassword, p => OldPassword = p).WithPassword()
                    ),
                    Row(
                        Text("New Password:", Styles.W8C),
                        Input(NewPassword, p => NewPassword = p).WithPassword()
                    ),
                    Row(
                        Text("Repeat Password:", Styles.W8C),
                        Input(PasswordRepeat, p => PasswordRepeat = p).WithPassword()
                    ),
                    Row(
                        Text("Apply Changes:", Styles.W8C),
                        Text("Apply", Styles.BtnSettings, () =>
                        {
                            if (NewPassword == PasswordRepeat)
                                try
                                {
                                    Account.Commands.ChangePassword(AccountEntry.ID, OldPassword, NewPassword);
                                }
                                catch
                                {
                                    OldPasswordWrong = true;
                                }
                            else
                                WrongPasswords = true;
                        })
                    ),
                    Div(
                        WrongPasswords
                        ? Div(
                            Text("Passwords are not the same!"),
                            Text("Ok", Styles.BtnSettings, () =>
                            {
                                 WrongPasswords = false;
                                 PasswordRepeat = string.Empty;
                                 NewPassword = string.Empty;
                            })
                        )
                        : null,
                        OldPasswordWrong
                        ? Div(
                            Text("Old Password is Wrong!"),
                            Text("Ok", Styles.BtnSettings, () =>
                            {
                                OldPasswordWrong = false;
                                OldPassword = string.Empty;
                            })
                        )
                        : null
                    )
                )
            );
        }
    }
}
