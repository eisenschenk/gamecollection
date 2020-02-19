using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VnodeTest.BC.General.Account.Event;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.General.Account.Account>;


namespace VnodeTest.BC.General.Account
{
    public class AccountProjection : Projection
    {
        private readonly Dictionary<AccountID, AccountEntry> Dict = new Dictionary<AccountID, AccountEntry>();

        public AccountEntry this[AccountID id] => Dict[id];
        public IEnumerable<AccountEntry> Accounts => Dict.Values;

        public AccountProjection(IEventStore store, IMessageBus bus) : base(store, bus)
        {
        }

#pragma warning disable IDE0051
        private void On(AccountRegistered @event)
        {
            Dict.Add(@event.ID, new AccountEntry(@event.ID, @event.Username, @event.Password, @event.Timestamp));
        }
        private void On(AccountLoggedIn @event)
        {
            Dict[@event.ID].LoggedIn = true;
        }
        private void On(AccountLoggedOut @event)
        {
            Dict[@event.ID].LoggedIn = false;
        }
        private void On(UsernameChanged @event)
        {
            Dict[@event.ID].ChangeUsername(@event.NewUsername);
        }
        private void On(PasswordChanged @event)
        {
            Dict[@event.ID].ChangePassword(@event.NewPassword);
        }
        private void On(IconChanged @event)
        {
            Dict[@event.ID].ChangeIcon(@event.NewIcon);
        }
        private void On(AutomaticPromotionChanged @event)
        {
            Dict[@event.ID].AutomaticPromotion = !Dict[@event.ID].AutomaticPromotion;
        }
#pragma warning restore
        public void LogoutAllAccounts()
        {
            foreach (AccountEntry entry in Accounts)
                entry.LoggedIn = false;
        }

        public IEnumerable<string> GetIcons()
        {
            string[] icons =
           {
                "fas fa-question",
                "fas fa-otter",
                "fas fa-paw",
                "fas fa-hippo",
                "fas fa-dog",
                "fas fa-spider",
                "fas fa-horse",
                "fas fa-frog",
                "fas fa-fish",
                "fas fa-dragon",
                "fas fa-crow",
                "fas fa-cat",
                "fas fa-dove",
                "fas fa-kiwi-bird",
            };

            return icons;

        }
    }


    public class AccountEntry
    {
        public AccountID ID { get; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public DateTimeOffset CreatedAt { get; }
        public bool LoggedIn { get; set; }
        public bool HasUpdated { get; set; }
        public bool AutomaticPromotion { get; set; }
        private string _Icon;
        public string Icon
        {
            get
            {
                if (_Icon == default)
                    return "fas fa-question";
                return _Icon;
            }
            private set
            {
                if (_Icon == value)
                    return;
                _Icon = value;
                HasUpdated = true;
            }
        }

        public AccountEntry(AccountID id, string username, string password, DateTimeOffset createdAt)
        {
            ID = id;
            Username = username;
            Password = password;
            CreatedAt = createdAt;
        }

        public void ChangeUsername(string newUsername)
        {
            Username = newUsername;
        }

        public void ChangePassword(string newPassword)
        {
            Password = newPassword;
        }

        public void ChangeIcon(string newIcon)
        {
            Icon = newIcon;
        }
    }
}

