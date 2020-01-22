using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameID = ACL.ES.AggregateID<VnodeTest.BC.Chess.Game.Chessgame>;
using AccountID = ACL.ES.AggregateID<VnodeTest.BC.General.Account.Account>;
using VnodeTest.BC.Chess.Game;
using ACL.MQ;

namespace VnodeTest.PM
{
    public class AcceptChallengePM
    {



        public static void PMacceptChallenge(GameID gameID, AccountID senderID, AccountID receiverID)
        {
            Chessgame.Commands.JoinGame(gameID, senderID);
            Chessgame.Commands.JoinGame(gameID, receiverID);
            MessageBus.Instance.Send(new BC.Chess.Game.Command.AcceptChallenge(gameID, receiverID, senderID));
            Chessgame.Commands.DeleteUnwantedChallenges(gameID, senderID, receiverID);
        }
    }
}