using ACL.ES;
using ACL.MQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Chess.Game.Chessgame>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.General.Account.Account>;
using VnodeTest.Chess;
using VnodeTest.BC.Chess.Game.Command;
using VnodeTest.BC.Chess.Game.Event;

namespace VnodeTest.BC.Chess.Game
{
    public class Chessgame : AggregateRoot<Chessgame>
    {
        public class Handler : AggregateCommandHandler<Chessgame>
        {
            public Handler(IRepository repository, IMessageBus bus) : base(repository, bus)
            {
            }
        }

        public static class Commands
        {
            public static void OpenGame(GameID id, Gamemode gamemode, double clocktimer) =>
                            MessageBus.Instance.Send(new OpenGame(id, gamemode, clocktimer));
            public static void RequestChallenge(GameID id, AccountID accountID, AccountID friendID) =>
                MessageBus.Instance.Send(new RequestChallenge(id, accountID, friendID));
            public static void DenyChallenge(GameID id) =>
                MessageBus.Instance.Send(new DenyChallenge(id));
            public static void AcceptChallenge(GameID id, AccountID receiverID, AccountID senderID) => PM.AcceptChallengePM.PMacceptChallenge(id, receiverID, senderID);
            public static void DeleteUnwantedChallenges(GameID id, AccountID receiverId, AccountID senderID) =>
                MessageBus.Instance.Send(new DeleteUnwantedChallenges(id, receiverId, senderID));
            public static void DeleteGame(GameID id) =>
                MessageBus.Instance.Send(new DeleteGame(id));
            public static void EndGame(GameID id, string moves) =>
                MessageBus.Instance.Send(new EndGame(id, moves));
            public static void JoinGame(GameID id, AccountID accountID) =>
                MessageBus.Instance.Send(new GameJoined(id, accountID));
        }

        public IEnumerable<IEvent> On(OpenGame command)
        {
            yield return new GameOpened(command.ID, command.Gamemode, command.Clocktimer);
        }
        public IEnumerable<IEvent> On(RequestChallenge command)
        {
            yield return new ChallengeRequested(command.ID, command.AccountID, command.FriendID);
        }
        public IEnumerable<IEvent> On(AcceptChallenge command)
        {
            yield return new ChallengeAccepted(command.ID, command.AccountID, command.FriendID);
        }
        public IEnumerable<IEvent> On(DenyChallenge command)
        {
            yield return new ChallengeDenied(command.ID);
        }
        public IEnumerable<IEvent> On(DeleteGame command)
        {
            yield return new GameDeleted(command.ID);
        }
        public IEnumerable<IEvent> On(EndGame command)
        {
            yield return new GameEnded(command.ID, command.Moves);
        }
        public IEnumerable<IEvent> On(JoinGame command)
        {
            yield return new GameJoined(command.ID, command.AccountID);
        }
        public IEnumerable<IEvent> On(DeleteUnwantedChallenges command)
        {
            yield return new UnwantedChallengesDeleted(command.ID, command.Receiver, command.Challenger);
        }

        public override void Apply(IEvent @event)
        {
            switch (@event)
            {
                
            }
        }
    }
}
