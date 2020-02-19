using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.BC.General.Account.Command;
using VnodeTest.BC.General.Account.Event;
using VnodeTest.General;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.General.Account.Account>;

namespace VnodeTest.BC.General.Account
{
    public class Account : AggregateRoot<Account>
    {
        private string Password;
        public class Handler : AggregateCommandHandler<Account>
        {
            public Handler(IRepository repository, IMessageBus bus) : base(repository, bus)
            {
            }
        }

        public static class Commands
        {
            public static void RegisterAccount(AccountID id, string username, string password) =>
               MessageBus.Instance.Send(new RegisterAccount(id, username, password));
            public static void LoginAccount(AccountID id, string password) =>
                MessageBus.Instance.Send(new LoginAccount(id, password));
            public static void LogoutAccount(AccountID id) => MessageBus.Instance.Send(new LogoutAccount(id));
            public static void ChangeUsername(AccountID id, string username) =>
                MessageBus.Instance.Send(new ChangeUsername(id, username));
            public static void ChangePassword(AccountID id, string oldPassword, string newPassword) =>
                MessageBus.Instance.Send(new ChangePassword(id, oldPassword, newPassword));
            public static void ChangeIcon(AccountID id, string newIcon) =>
                MessageBus.Instance.Send(new ChangeIcon(id, newIcon));
            public static void ChangeAutomaticPromotion(AccountID id, bool oldSetting) =>
                MessageBus.Instance.Send(new ChangeAutomaticPromotion(id, oldSetting));
        }

        public IEnumerable<IEvent> On(RegisterAccount command)
        {
            yield return new AccountRegistered(command.ID, command.Username, PasswordHelper.HashAndSalt(command.Password));
        }
        public IEnumerable<IEvent> On(LoginAccount command)
        {
            if (!PasswordHelper.IsPasswordMatch(command.Password, Password))
                throw new ArgumentException("password not correct");
            yield return new AccountLoggedIn(command.ID);
        }
        public IEnumerable<IEvent> On(LogoutAccount command)
        {
            yield return new AccountLoggedOut(command.ID);
        }

        public IEnumerable<IEvent> On(ChangeUsername command)
        {
            yield return new UsernameChanged(command.ID, command.NewUsername);
        }
        public IEnumerable<IEvent> On(ChangePassword command)
        {
            if (!PasswordHelper.IsPasswordMatch(command.OldPassword, Password))
                throw new ArgumentException("password not correct");
            yield return new PasswordChanged(command.ID, PasswordHelper.HashAndSalt(command.NewPassword));
        }
        public IEnumerable<IEvent> On(ChangeIcon command)
        {
            yield return new IconChanged(command.ID, command.NewIcon);
        }
        public IEnumerable<IEvent> On(ChangeAutomaticPromotion command)
        {
            yield return new AutomaticPromotionChanged(command.ID, command.OldSetting);
        }
        public override void Apply(IEvent @event)
        {
            switch (@event)
            {
                case AccountRegistered registered:
                    Password = registered.Password;
                    break;
                case PasswordChanged changedPW:
                    Password = changedPW.NewPassword;
                    break;
            }
        }
    }
}