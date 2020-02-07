using ACL.ES;
using ACL.MQ;
using System.Collections.Generic;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Solitaire.Solitaire>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.General.Account.Account>;
using VnodeTest.BC.Solitaire.Command;
using VnodeTest.BC.Solitaire.Event;

namespace VnodeTest.BC.Solitaire
{
    public class Solitaire : AggregateRoot<Solitaire>
    {
        public class Handler : AggregateCommandHandler<Solitaire>
        {
            public Handler(IRepository repository, IMessageBus bus) : base(repository, bus)
            {
            }
        }

        public static class Commands
        {
            public static void OpenGame(GameID id, AccountID accountID) =>
                            MessageBus.Instance.Send(new OpenGame(id, accountID));
            public static void EndGame(GameID id, AccountID accountID, int score) =>
                MessageBus.Instance.Send(new EndGame(id, accountID, score));
            public static void JoinGame(GameID id, AccountID accountID) =>
                MessageBus.Instance.Send(new JoinGame(id, accountID));
        }

        public IEnumerable<IEvent> On(OpenGame command)
        {
            yield return new GameOpened(command.ID, command.AccountID);
        }
        public IEnumerable<IEvent> On(EndGame command)
        {
            yield return new GameEnded(command.ID, command.AccountID, command.Score);
        }
        public IEnumerable<IEvent> On(JoinGame command)
        {
            yield return new Gamejoined(command.ID, command.AccountID);
        }

        public override void Apply(IEvent @event)
        {
            switch (@event)
            {

            }
        }
    }
}